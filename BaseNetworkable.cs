using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ConVar;
using Facepunch;
using Facepunch.Rust;
using Network;
using Network.Visibility;
using ProtoBuf;
using Rust;
using Rust.Registry;
using UnityEngine;
using UnityEngine.Assertions;

public abstract class BaseNetworkable : BaseMonoBehaviour, IEntity, NetworkHandler, IPrefabPostProcess
{
	public struct SaveInfo
	{
		public Entity msg;

		public bool forDisk;

		public bool forTransfer;

		public Connection forConnection;

		internal bool SendingTo(Connection ownerConnection)
		{
			if (ownerConnection == null)
			{
				return false;
			}
			if (forConnection == null)
			{
				return false;
			}
			return forConnection == ownerConnection;
		}
	}

	public struct LoadInfo
	{
		public Entity msg;

		public bool fromDisk;

		public bool fromTransfer;
	}

	public class EntityRealmServer : EntityRealm
	{
		protected override Manager visibilityManager
		{
			get
			{
				if (Net.sv == null)
				{
					return null;
				}
				return Net.sv.visibility;
			}
		}
	}

	public abstract class EntityRealm : IEnumerable<BaseNetworkable>, IEnumerable
	{
		private ListDictionary<NetworkableId, BaseNetworkable> entityList = new ListDictionary<NetworkableId, BaseNetworkable>();

		public int Count => entityList.Count;

		protected abstract Manager visibilityManager { get; }

		public bool Contains(NetworkableId uid)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return entityList.Contains(uid);
		}

		public BaseNetworkable Find(NetworkableId uid)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			BaseNetworkable result = null;
			if (!entityList.TryGetValue(uid, ref result))
			{
				return null;
			}
			return result;
		}

		public void RegisterID(BaseNetworkable ent)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			if (ent.net != null)
			{
				if (entityList.Contains(ent.net.ID))
				{
					entityList[ent.net.ID] = ent;
				}
				else
				{
					entityList.Add(ent.net.ID, ent);
				}
			}
		}

		public void UnregisterID(BaseNetworkable ent)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			if (ent.net != null)
			{
				entityList.Remove(ent.net.ID);
			}
		}

		public Group FindGroup(uint uid)
		{
			Manager val = visibilityManager;
			if (val == null)
			{
				return null;
			}
			return val.Get(uid);
		}

		public Group TryFindGroup(uint uid)
		{
			Manager val = visibilityManager;
			if (val == null)
			{
				return null;
			}
			return val.TryGet(uid);
		}

		public void FindInGroup(uint uid, List<BaseNetworkable> list)
		{
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			Group val = TryFindGroup(uid);
			if (val == null)
			{
				return;
			}
			int count = val.networkables.Values.Count;
			Networkable[] buffer = val.networkables.Values.Buffer;
			for (int i = 0; i < count; i++)
			{
				Networkable val2 = buffer[i];
				BaseNetworkable baseNetworkable = Find(val2.ID);
				if (!((Object)(object)baseNetworkable == (Object)null) && baseNetworkable.net != null && baseNetworkable.net.group != null)
				{
					if (baseNetworkable.net.group.ID != uid)
					{
						Debug.LogWarning((object)("Group ID mismatch: " + ((object)baseNetworkable).ToString()));
					}
					else
					{
						list.Add(baseNetworkable);
					}
				}
			}
		}

		public Enumerator<BaseNetworkable> GetEnumerator()
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			return entityList.Values.GetEnumerator();
		}

		IEnumerator<BaseNetworkable> IEnumerable<BaseNetworkable>.GetEnumerator()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return (IEnumerator<BaseNetworkable>)(object)GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return (IEnumerator)(object)GetEnumerator();
		}

		public void Clear()
		{
			entityList.Clear();
		}
	}

	public enum DestroyMode : byte
	{
		None,
		Gib
	}

	[Header("BaseNetworkable")]
	[ReadOnly]
	public uint prefabID;

	[Tooltip("If enabled the entity will send to everyone on the server - regardless of position")]
	public bool globalBroadcast;

	[Tooltip("Global broadcast a cut down version of the entity to show buildings across the map")]
	public bool globalBuildingBlock;

	[NonSerialized]
	public Networkable net;

	private string _prefabName;

	private string _prefabNameWithoutExtension;

	public static EntityRealm serverEntities = new EntityRealmServer();

	private const bool isServersideEntity = true;

	private static List<Connection> connectionsInSphereList = new List<Connection>();

	public List<Component> postNetworkUpdateComponents = new List<Component>();

	private bool _limitedNetworking;

	[NonSerialized]
	public EntityRef parentEntity;

	[NonSerialized]
	public readonly List<BaseEntity> children = new List<BaseEntity>();

	[NonSerialized]
	public bool canTriggerParent = true;

	private int creationFrame;

	protected bool isSpawned;

	private MemoryStream _NetworkCache;

	public static Queue<MemoryStream> EntityMemoryStreamPool = new Queue<MemoryStream>();

	private MemoryStream _SaveCache;

	public bool IsDestroyed { get; private set; }

	public string PrefabName
	{
		get
		{
			if (_prefabName == null)
			{
				_prefabName = StringPool.Get(prefabID);
			}
			return _prefabName;
		}
	}

	public string ShortPrefabName
	{
		get
		{
			if (_prefabNameWithoutExtension == null)
			{
				_prefabNameWithoutExtension = Path.GetFileNameWithoutExtension(PrefabName);
			}
			return _prefabNameWithoutExtension;
		}
	}

	public bool isServer => true;

	public bool isClient => false;

	public bool limitNetworking
	{
		get
		{
			return _limitedNetworking;
		}
		set
		{
			if (value != _limitedNetworking)
			{
				_limitedNetworking = value;
				if (_limitedNetworking)
				{
					OnNetworkLimitStart();
				}
				else
				{
					OnNetworkLimitEnd();
				}
				UpdateNetworkGroup();
			}
		}
	}

	public GameManager gameManager
	{
		get
		{
			if (isServer)
			{
				return GameManager.server;
			}
			throw new NotImplementedException("Missing gameManager path");
		}
	}

	public PrefabAttribute.Library prefabAttribute
	{
		get
		{
			if (isServer)
			{
				return PrefabAttribute.server;
			}
			throw new NotImplementedException("Missing prefabAttribute path");
		}
	}

	public static Group GlobalNetworkGroup => Net.sv.visibility.Get(0u);

	public static Group LimboNetworkGroup => Net.sv.visibility.Get(1u);

	public virtual Vector3 GetNetworkPosition()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return ((Component)this).transform.localPosition;
	}

	public virtual Quaternion GetNetworkRotation()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return ((Component)this).transform.localRotation;
	}

	public string InvokeString()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		StringBuilder stringBuilder = new StringBuilder();
		List<InvokeAction> list = Pool.GetList<InvokeAction>();
		InvokeHandler.FindInvokes((Behaviour)(object)this, list);
		foreach (InvokeAction item in list)
		{
			if (stringBuilder.Length > 0)
			{
				stringBuilder.Append(", ");
			}
			stringBuilder.Append(item.action.Method.Name);
		}
		Pool.FreeList<InvokeAction>(ref list);
		return stringBuilder.ToString();
	}

	public BaseEntity LookupPrefab()
	{
		return gameManager.FindPrefab(PrefabName).ToBaseEntity();
	}

	public bool EqualNetID(BaseNetworkable other)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		if (!other.IsRealNull() && other.net != null && net != null)
		{
			return other.net.ID == net.ID;
		}
		return false;
	}

	public bool EqualNetID(NetworkableId otherID)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		if (net != null)
		{
			return otherID == net.ID;
		}
		return false;
	}

	public virtual void ResetState()
	{
		if (children.Count > 0)
		{
			children.Clear();
		}
		if (this is ILootableEntity lootableEntity)
		{
			lootableEntity.LastLootedBy = 0uL;
		}
	}

	public virtual void InitShared()
	{
	}

	public virtual void PreInitShared()
	{
	}

	public virtual void PostInitShared()
	{
	}

	public virtual void DestroyShared()
	{
	}

	public virtual void OnNetworkGroupEnter(Group group)
	{
	}

	public virtual void OnNetworkGroupLeave(Group group)
	{
	}

	public void OnNetworkGroupChange()
	{
		if (children == null)
		{
			return;
		}
		foreach (BaseEntity child in children)
		{
			if (child.ShouldInheritNetworkGroup())
			{
				child.net.SwitchGroup(net.group);
			}
			else if (isServer)
			{
				child.UpdateNetworkGroup();
			}
		}
	}

	public void OnNetworkSubscribersEnter(List<Connection> connections)
	{
		if (!((BaseNetwork)Net.sv).IsConnected())
		{
			return;
		}
		foreach (Connection connection in connections)
		{
			BasePlayer basePlayer = connection.player as BasePlayer;
			if (!((Object)(object)basePlayer == (Object)null))
			{
				basePlayer.QueueUpdate(BasePlayer.NetworkQueue.Update, this as BaseEntity);
			}
		}
	}

	public void OnNetworkSubscribersLeave(List<Connection> connections)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		if (((BaseNetwork)Net.sv).IsConnected())
		{
			LogEntry(LogEntryType.Network, 2, "LeaveVisibility");
			NetWrite obj = ((BaseNetwork)Net.sv).StartWrite();
			obj.PacketID((Type)6);
			obj.EntityID(net.ID);
			obj.UInt8((byte)0);
			obj.Send(new SendInfo(connections));
		}
	}

	private void EntityDestroy()
	{
		if (Object.op_Implicit((Object)(object)((Component)this).gameObject))
		{
			ResetState();
			gameManager.Retire(((Component)this).gameObject);
		}
	}

	private void DoEntityDestroy()
	{
		if (IsDestroyed)
		{
			return;
		}
		IsDestroyed = true;
		if (Application.isQuitting)
		{
			return;
		}
		DestroyShared();
		if (isServer)
		{
			DoServerDestroy();
		}
		TimeWarning val = TimeWarning.New("Registry.Entity.Unregister", 0);
		try
		{
			Entity.Unregister(((Component)this).gameObject);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private void SpawnShared()
	{
		IsDestroyed = false;
		TimeWarning val = TimeWarning.New("Registry.Entity.Register", 0);
		try
		{
			Entity.Register(((Component)this).gameObject, (IEntity)(object)this);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public virtual void Save(SaveInfo info)
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		if (prefabID == 0)
		{
			Debug.LogError((object)("PrefabID is 0! " + ((Component)this).transform.GetRecursiveName()), (Object)(object)((Component)this).gameObject);
		}
		info.msg.baseNetworkable = Pool.Get<BaseNetworkable>();
		info.msg.baseNetworkable.uid = net.ID;
		info.msg.baseNetworkable.prefabID = prefabID;
		if (net.group != null)
		{
			info.msg.baseNetworkable.group = net.group.ID;
		}
		if (!info.forDisk)
		{
			info.msg.createdThisFrame = creationFrame == Time.frameCount;
		}
	}

	public virtual void PostSave(SaveInfo info)
	{
	}

	public void InitLoad(NetworkableId entityID)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		net = Net.sv.CreateNetworkable(entityID);
		serverEntities.RegisterID(this);
		PreServerLoad();
	}

	public virtual void PreServerLoad()
	{
	}

	public virtual void Load(LoadInfo info)
	{
		if (info.msg.baseNetworkable != null)
		{
			BaseNetworkable baseNetworkable = info.msg.baseNetworkable;
			if (prefabID != baseNetworkable.prefabID)
			{
				Debug.LogError((object)("Prefab IDs don't match! " + prefabID + "/" + baseNetworkable.prefabID + " -> " + (object)((Component)this).gameObject), (Object)(object)((Component)this).gameObject);
			}
		}
	}

	public virtual void PostServerLoad()
	{
		((Component)this).gameObject.SendOnSendNetworkUpdate(this as BaseEntity);
	}

	public T ToServer<T>() where T : BaseNetworkable
	{
		if (isServer)
		{
			return this as T;
		}
		return null;
	}

	public virtual bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		return false;
	}

	public static List<Connection> GetConnectionsWithin(Vector3 position, float distance)
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		connectionsInSphereList.Clear();
		float num = distance * distance;
		List<Connection> subscribers = GlobalNetworkGroup.subscribers;
		for (int i = 0; i < subscribers.Count; i++)
		{
			Connection val = subscribers[i];
			if (val.active)
			{
				BasePlayer basePlayer = val.player as BasePlayer;
				if (!((Object)(object)basePlayer == (Object)null) && !(basePlayer.SqrDistance(position) > num))
				{
					connectionsInSphereList.Add(val);
				}
			}
		}
		return connectionsInSphereList;
	}

	public static void GetCloseConnections(Vector3 position, float distance, List<BasePlayer> players)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		if (Net.sv == null || Net.sv.visibility == null)
		{
			return;
		}
		float num = distance * distance;
		Group group = Net.sv.visibility.GetGroup(position);
		if (group == null)
		{
			return;
		}
		List<Connection> subscribers = group.subscribers;
		for (int i = 0; i < subscribers.Count; i++)
		{
			Connection val = subscribers[i];
			if (val.active)
			{
				BasePlayer basePlayer = val.player as BasePlayer;
				if (!((Object)(object)basePlayer == (Object)null) && !(basePlayer.SqrDistance(position) > num))
				{
					players.Add(basePlayer);
				}
			}
		}
	}

	public static bool HasCloseConnections(Vector3 position, float distance)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		if (Net.sv == null)
		{
			return false;
		}
		if (Net.sv.visibility == null)
		{
			return false;
		}
		float num = distance * distance;
		Group group = Net.sv.visibility.GetGroup(position);
		if (group == null)
		{
			return false;
		}
		List<Connection> subscribers = group.subscribers;
		for (int i = 0; i < subscribers.Count; i++)
		{
			Connection val = subscribers[i];
			if (val.active)
			{
				BasePlayer basePlayer = val.player as BasePlayer;
				if (!((Object)(object)basePlayer == (Object)null) && !(basePlayer.SqrDistance(position) > num))
				{
					return true;
				}
			}
		}
		return false;
	}

	public static bool HasConnections(Vector3 position)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		if (Net.sv == null)
		{
			return false;
		}
		if (Net.sv.visibility == null)
		{
			return false;
		}
		Group group = Net.sv.visibility.GetGroup(position);
		if (group == null)
		{
			return false;
		}
		List<Connection> subscribers = group.subscribers;
		for (int i = 0; i < subscribers.Count; i++)
		{
			Connection val = subscribers[i];
			if (val.active && !((Object)(object)(val.player as BasePlayer) == (Object)null))
			{
				return true;
			}
		}
		return false;
	}

	public void BroadcastOnPostNetworkUpdate(BaseEntity entity)
	{
		foreach (Component postNetworkUpdateComponent in postNetworkUpdateComponents)
		{
			(postNetworkUpdateComponent as IOnPostNetworkUpdate)?.OnPostNetworkUpdate(entity);
		}
		foreach (BaseEntity child in children)
		{
			child.BroadcastOnPostNetworkUpdate(entity);
		}
	}

	public virtual void PostProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		if (!serverside)
		{
			postNetworkUpdateComponents = ((Component)this).GetComponentsInChildren<IOnPostNetworkUpdate>(true).Cast<Component>().ToList();
		}
	}

	private void OnNetworkLimitStart()
	{
		LogEntry(LogEntryType.Network, 2, "OnNetworkLimitStart");
		List<Connection> subscribers = GetSubscribers();
		if (subscribers == null)
		{
			return;
		}
		subscribers = subscribers.ToList();
		subscribers.RemoveAll((Connection x) => ShouldNetworkTo(x.player as BasePlayer));
		OnNetworkSubscribersLeave(subscribers);
		if (children == null)
		{
			return;
		}
		foreach (BaseEntity child in children)
		{
			child.OnNetworkLimitStart();
		}
	}

	private void OnNetworkLimitEnd()
	{
		LogEntry(LogEntryType.Network, 2, "OnNetworkLimitEnd");
		List<Connection> subscribers = GetSubscribers();
		if (subscribers == null)
		{
			return;
		}
		OnNetworkSubscribersEnter(subscribers);
		if (children == null)
		{
			return;
		}
		foreach (BaseEntity child in children)
		{
			child.OnNetworkLimitEnd();
		}
	}

	public BaseEntity GetParentEntity()
	{
		return parentEntity.Get(isServer);
	}

	public bool HasParent()
	{
		return parentEntity.IsValid(isServer);
	}

	public void AddChild(BaseEntity child)
	{
		if (!children.Contains(child))
		{
			children.Add(child);
			OnChildAdded(child);
		}
	}

	protected virtual void OnChildAdded(BaseEntity child)
	{
	}

	public void RemoveChild(BaseEntity child)
	{
		children.Remove(child);
		OnChildRemoved(child);
	}

	protected virtual void OnChildRemoved(BaseEntity child)
	{
	}

	public virtual float GetNetworkTime()
	{
		return Time.time;
	}

	public virtual void Spawn()
	{
		SpawnShared();
		if (net == null)
		{
			net = Net.sv.CreateNetworkable();
		}
		creationFrame = Time.frameCount;
		PreInitShared();
		InitShared();
		ServerInit();
		PostInitShared();
		UpdateNetworkGroup();
		ServerInitPostNetworkGroupAssign();
		isSpawned = true;
		SendNetworkUpdateImmediate(justCreated: true);
		((FacepunchBehaviour)this).Invoke((Action)SendGlobalNetworkUpdate, 0f);
		if (Application.isLoading && !Application.isLoadingSave)
		{
			((Component)this).gameObject.SendOnSendNetworkUpdate(this as BaseEntity);
		}
	}

	private void SendGlobalNetworkUpdate()
	{
		GlobalNetworkHandler.server?.TrySendNetworkUpdate(this);
	}

	public bool IsFullySpawned()
	{
		return isSpawned;
	}

	public virtual void ServerInit()
	{
		serverEntities.RegisterID(this);
		if (net != null)
		{
			net.handler = (NetworkHandler)(object)this;
		}
	}

	public virtual void ServerInitPostNetworkGroupAssign()
	{
	}

	protected List<Connection> GetSubscribers()
	{
		if (net == null)
		{
			return null;
		}
		if (net.group == null)
		{
			return null;
		}
		return net.group.subscribers;
	}

	public void KillMessage()
	{
		Kill();
	}

	public virtual void AdminKill()
	{
		Kill(DestroyMode.Gib);
	}

	public void Kill(DestroyMode mode = DestroyMode.None)
	{
		if (IsDestroyed)
		{
			Debug.LogWarning((object)("Calling kill - but already IsDestroyed!? " + (object)this));
			return;
		}
		((Component)this).gameObject.BroadcastOnParentDestroying();
		DoEntityDestroy();
		TerminateOnClient(mode);
		TerminateOnServer();
		EntityDestroy();
	}

	private void TerminateOnClient(DestroyMode mode)
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		if (net != null && net.group != null && ((BaseNetwork)Net.sv).IsConnected())
		{
			LogEntry(LogEntryType.Network, 2, "Term {0}", mode);
			NetWrite obj = ((BaseNetwork)Net.sv).StartWrite();
			obj.PacketID((Type)6);
			obj.EntityID(net.ID);
			obj.UInt8((byte)mode);
			obj.Send(new SendInfo(net.group.subscribers));
			GlobalNetworkHandler.server?.OnEntityKilled(this);
		}
	}

	private void TerminateOnServer()
	{
		if (net != null)
		{
			InvalidateNetworkCache();
			serverEntities.UnregisterID(this);
			Net.sv.DestroyNetworkable(ref net);
			((MonoBehaviour)this).StopAllCoroutines();
			((Component)this).gameObject.SetActive(false);
		}
	}

	internal virtual void DoServerDestroy()
	{
		isSpawned = false;
		Analytics.Azure.OnEntityDestroyed(this);
	}

	public virtual bool ShouldNetworkTo(BasePlayer player)
	{
		if (net.group == null)
		{
			return true;
		}
		return player.net.subscriber.IsSubscribed(net.group);
	}

	protected void SendNetworkGroupChange()
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		if (isSpawned && ((BaseNetwork)Net.sv).IsConnected())
		{
			if (net.group == null)
			{
				Debug.LogWarning((object)(((object)this).ToString() + " changed its network group to null"));
				return;
			}
			NetWrite obj = ((BaseNetwork)Net.sv).StartWrite();
			obj.PacketID((Type)7);
			obj.EntityID(net.ID);
			obj.GroupID(net.group.ID);
			obj.Send(new SendInfo(net.group.subscribers));
		}
	}

	protected void SendAsSnapshot(Connection connection, bool justCreated = false)
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		NetWrite val = ((BaseNetwork)Net.sv).StartWrite();
		connection.validate.entityUpdates++;
		SaveInfo saveInfo = default(SaveInfo);
		saveInfo.forConnection = connection;
		saveInfo.forDisk = false;
		SaveInfo saveInfo2 = saveInfo;
		val.PacketID((Type)5);
		val.UInt32(connection.validate.entityUpdates);
		ToStreamForNetwork((Stream)(object)val, saveInfo2);
		val.Send(new SendInfo(connection));
	}

	public void SendNetworkUpdate(BasePlayer.NetworkQueue queue = BasePlayer.NetworkQueue.Update)
	{
		if (Application.isLoading || Application.isLoadingSave || IsDestroyed || net == null || !isSpawned)
		{
			return;
		}
		TimeWarning val = TimeWarning.New("SendNetworkUpdate", 0);
		try
		{
			LogEntry(LogEntryType.Network, 2, "SendNetworkUpdate");
			InvalidateNetworkCache();
			List<Connection> subscribers = GetSubscribers();
			if (subscribers != null && subscribers.Count > 0)
			{
				for (int i = 0; i < subscribers.Count; i++)
				{
					BasePlayer basePlayer = subscribers[i].player as BasePlayer;
					if (!((Object)(object)basePlayer == (Object)null) && ShouldNetworkTo(basePlayer))
					{
						basePlayer.QueueUpdate(queue, this);
					}
				}
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		((Component)this).gameObject.SendOnSendNetworkUpdate(this as BaseEntity);
	}

	public void SendNetworkUpdateImmediate(bool justCreated = false)
	{
		if (Application.isLoading || Application.isLoadingSave || IsDestroyed || net == null || !isSpawned)
		{
			return;
		}
		TimeWarning val = TimeWarning.New("SendNetworkUpdateImmediate", 0);
		try
		{
			LogEntry(LogEntryType.Network, 2, "SendNetworkUpdateImmediate");
			InvalidateNetworkCache();
			List<Connection> subscribers = GetSubscribers();
			if (subscribers != null && subscribers.Count > 0)
			{
				for (int i = 0; i < subscribers.Count; i++)
				{
					Connection val2 = subscribers[i];
					BasePlayer basePlayer = val2.player as BasePlayer;
					if (!((Object)(object)basePlayer == (Object)null) && ShouldNetworkTo(basePlayer))
					{
						SendAsSnapshot(val2, justCreated);
					}
				}
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		((Component)this).gameObject.SendOnSendNetworkUpdate(this as BaseEntity);
	}

	protected void SendNetworkUpdate_Position()
	{
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		if (Application.isLoading || Application.isLoadingSave || IsDestroyed || net == null || !isSpawned)
		{
			return;
		}
		TimeWarning val = TimeWarning.New("SendNetworkUpdate_Position", 0);
		try
		{
			LogEntry(LogEntryType.Network, 2, "SendNetworkUpdate_Position");
			List<Connection> subscribers = GetSubscribers();
			if (subscribers != null && subscribers.Count > 0)
			{
				NetWrite val2 = ((BaseNetwork)Net.sv).StartWrite();
				val2.PacketID((Type)10);
				val2.EntityID(net.ID);
				Vector3 networkPosition = GetNetworkPosition();
				val2.Vector3(ref networkPosition);
				Quaternion networkRotation = GetNetworkRotation();
				networkPosition = ((Quaternion)(ref networkRotation)).eulerAngles;
				val2.Vector3(ref networkPosition);
				val2.Float(GetNetworkTime());
				NetworkableId uid = parentEntity.uid;
				if (((NetworkableId)(ref uid)).IsValid)
				{
					val2.EntityID(uid);
				}
				SendInfo val3 = new SendInfo(subscribers);
				val3.method = (SendMethod)1;
				val3.priority = (Priority)0;
				SendInfo val4 = val3;
				val2.Send(val4);
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private void ToStream(Stream stream, SaveInfo saveInfo)
	{
		Entity val = (saveInfo.msg = Pool.Get<Entity>());
		try
		{
			Save(saveInfo);
			if (saveInfo.msg.baseEntity == null)
			{
				Debug.LogError((object)(((object)this)?.ToString() + ": ToStream - no BaseEntity!?"));
			}
			if (saveInfo.msg.baseNetworkable == null)
			{
				Debug.LogError((object)(((object)this)?.ToString() + ": ToStream - no baseNetworkable!?"));
			}
			saveInfo.msg.ToProto(stream);
			PostSave(saveInfo);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public virtual bool CanUseNetworkCache(Connection connection)
	{
		return ConVar.Server.netcache;
	}

	public void ToStreamForNetwork(Stream stream, SaveInfo saveInfo)
	{
		if (!CanUseNetworkCache(saveInfo.forConnection))
		{
			ToStream(stream, saveInfo);
			return;
		}
		if (_NetworkCache == null)
		{
			_NetworkCache = ((EntityMemoryStreamPool.Count > 0) ? (_NetworkCache = EntityMemoryStreamPool.Dequeue()) : new MemoryStream(8));
			ToStream(_NetworkCache, saveInfo);
			ConVar.Server.netcachesize += (int)_NetworkCache.Length;
		}
		_NetworkCache.WriteTo(stream);
	}

	public void InvalidateNetworkCache()
	{
		TimeWarning val = TimeWarning.New("InvalidateNetworkCache", 0);
		try
		{
			if (_SaveCache != null)
			{
				ConVar.Server.savecachesize -= (int)_SaveCache.Length;
				_SaveCache.SetLength(0L);
				_SaveCache.Position = 0L;
				EntityMemoryStreamPool.Enqueue(_SaveCache);
				_SaveCache = null;
			}
			if (_NetworkCache != null)
			{
				ConVar.Server.netcachesize -= (int)_NetworkCache.Length;
				_NetworkCache.SetLength(0L);
				_NetworkCache.Position = 0L;
				EntityMemoryStreamPool.Enqueue(_NetworkCache);
				_NetworkCache = null;
			}
			LogEntry(LogEntryType.Network, 3, "InvalidateNetworkCache");
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public MemoryStream GetSaveCache()
	{
		if (_SaveCache == null)
		{
			if (EntityMemoryStreamPool.Count > 0)
			{
				_SaveCache = EntityMemoryStreamPool.Dequeue();
			}
			else
			{
				_SaveCache = new MemoryStream(8);
			}
			SaveInfo saveInfo = default(SaveInfo);
			saveInfo.forDisk = true;
			SaveInfo saveInfo2 = saveInfo;
			ToStream(_SaveCache, saveInfo2);
			ConVar.Server.savecachesize += (int)_SaveCache.Length;
		}
		return _SaveCache;
	}

	public virtual void UpdateNetworkGroup()
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		Assert.IsTrue(isServer, "UpdateNetworkGroup called on clientside entity!");
		if (net == null)
		{
			return;
		}
		TimeWarning val = TimeWarning.New("UpdateGroups", 0);
		try
		{
			if (net.UpdateGroups(((Component)this).transform.position))
			{
				SendNetworkGroupChange();
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}
}
