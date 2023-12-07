using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ConVar;
using Facepunch;
using Facepunch.Extend;
using Network;
using ProtoBuf;
using Rust;
using Rust.Ai;
using Rust.Workshop;
using Spatial;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public class BaseEntity : BaseNetworkable, IOnParentSpawning, IPrefabPreProcess
{
	public class Menu : Attribute
	{
		[Serializable]
		public struct Option
		{
			public Phrase name;

			public Phrase description;

			public Sprite icon;

			public int order;

			public bool usableWhileWounded;
		}

		public class Description : Attribute
		{
			public string token;

			public string english;

			public Description(string t, string e)
			{
				token = t;
				english = e;
			}
		}

		public class Icon : Attribute
		{
			public string icon;

			public Icon(string i)
			{
				icon = i;
			}
		}

		public class ShowIf : Attribute
		{
			public string functionName;

			public ShowIf(string testFunc)
			{
				functionName = testFunc;
			}
		}

		public class Priority : Attribute
		{
			public string functionName;

			public Priority(string priorityFunc)
			{
				functionName = priorityFunc;
			}
		}

		public class UsableWhileWounded : Attribute
		{
		}

		public string TitleToken;

		public string TitleEnglish;

		public string UseVariable;

		public int Order;

		public string ProxyFunction;

		public float Time;

		public string OnStart;

		public string OnProgress;

		public bool LongUseOnly;

		public bool PrioritizeIfNotWhitelisted;

		public bool PrioritizeIfUnlocked;

		public Menu()
		{
		}

		public Menu(string menuTitleToken, string menuTitleEnglish)
		{
			TitleToken = menuTitleToken;
			TitleEnglish = menuTitleEnglish;
		}
	}

	[Serializable]
	public struct MovementModify
	{
		public float drag;
	}

	public enum GiveItemReason
	{
		Generic,
		ResourceHarvested,
		PickedUp,
		Crafted
	}

	[Flags]
	public enum Flags
	{
		Placeholder = 1,
		On = 2,
		OnFire = 4,
		Open = 8,
		Locked = 0x10,
		Debugging = 0x20,
		Disabled = 0x40,
		Reserved1 = 0x80,
		Reserved2 = 0x100,
		Reserved3 = 0x200,
		Reserved4 = 0x400,
		Reserved5 = 0x800,
		Broken = 0x1000,
		Busy = 0x2000,
		Reserved6 = 0x4000,
		Reserved7 = 0x8000,
		Reserved8 = 0x10000,
		Reserved9 = 0x20000,
		Reserved10 = 0x40000,
		Reserved11 = 0x80000,
		InUse = 0x100000,
		Reserved12 = 0x200000,
		Reserved13 = 0x400000,
		Unused23 = 0x800000,
		Protected = 0x1000000,
		Transferring = 0x2000000
	}

	private readonly struct QueuedFileRequest : IEquatable<QueuedFileRequest>
	{
		public readonly BaseEntity Entity;

		public readonly FileStorage.Type Type;

		public readonly uint Part;

		public readonly uint Crc;

		public readonly uint ResponseFunction;

		public readonly bool? RespondIfNotFound;

		public QueuedFileRequest(BaseEntity entity, FileStorage.Type type, uint part, uint crc, uint responseFunction, bool? respondIfNotFound)
		{
			Entity = entity;
			Type = type;
			Part = part;
			Crc = crc;
			ResponseFunction = responseFunction;
			RespondIfNotFound = respondIfNotFound;
		}

		public bool Equals(QueuedFileRequest other)
		{
			if (object.Equals(Entity, other.Entity) && Type == other.Type && Part == other.Part && Crc == other.Crc && ResponseFunction == other.ResponseFunction)
			{
				return RespondIfNotFound == other.RespondIfNotFound;
			}
			return false;
		}

		public override bool Equals(object obj)
		{
			if (obj is QueuedFileRequest other)
			{
				return Equals(other);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return (int)(((((((((uint)((((Object)(object)Entity != (Object)null) ? ((object)Entity).GetHashCode() : 0) * 397) ^ (uint)Type) * 397) ^ Part) * 397) ^ Crc) * 397) ^ ResponseFunction) * 397) ^ RespondIfNotFound.GetHashCode();
		}
	}

	private readonly struct PendingFileRequest : IEquatable<PendingFileRequest>
	{
		public readonly FileStorage.Type Type;

		public readonly uint NumId;

		public readonly uint Crc;

		public readonly IServerFileReceiver Receiver;

		public readonly float Time;

		public PendingFileRequest(FileStorage.Type type, uint numId, uint crc, IServerFileReceiver receiver)
		{
			Type = type;
			NumId = numId;
			Crc = crc;
			Receiver = receiver;
			Time = Time.realtimeSinceStartup;
		}

		public bool Equals(PendingFileRequest other)
		{
			if (Type == other.Type && NumId == other.NumId && Crc == other.Crc)
			{
				return object.Equals(Receiver, other.Receiver);
			}
			return false;
		}

		public override bool Equals(object obj)
		{
			if (obj is PendingFileRequest other)
			{
				return Equals(other);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return (int)(((((uint)((int)Type * 397) ^ NumId) * 397) ^ Crc) * 397) ^ ((Receiver != null) ? Receiver.GetHashCode() : 0);
		}
	}

	public static class Query
	{
		public class EntityTree
		{
			private Grid<BaseEntity> Grid;

			private Grid<BasePlayer> PlayerGrid;

			private Grid<BaseEntity> BrainGrid;

			public EntityTree(float worldSize)
			{
				Grid = new Grid<BaseEntity>(32, worldSize);
				PlayerGrid = new Grid<BasePlayer>(32, worldSize);
				BrainGrid = new Grid<BaseEntity>(32, worldSize);
			}

			public void Add(BaseEntity ent)
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_000b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0013: Unknown result type (might be due to invalid IL or missing references)
				//IL_0019: Unknown result type (might be due to invalid IL or missing references)
				Vector3 position = ((Component)ent).transform.position;
				Grid.Add(ent, position.x, position.z);
			}

			public void AddPlayer(BasePlayer player)
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_000b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0013: Unknown result type (might be due to invalid IL or missing references)
				//IL_0019: Unknown result type (might be due to invalid IL or missing references)
				Vector3 position = ((Component)player).transform.position;
				PlayerGrid.Add(player, position.x, position.z);
			}

			public void AddBrain(BaseEntity entity)
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_000b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0013: Unknown result type (might be due to invalid IL or missing references)
				//IL_0019: Unknown result type (might be due to invalid IL or missing references)
				Vector3 position = ((Component)entity).transform.position;
				BrainGrid.Add(entity, position.x, position.z);
			}

			public void Remove(BaseEntity ent, bool isPlayer = false)
			{
				Grid.Remove(ent);
				if (isPlayer)
				{
					BasePlayer basePlayer = ent as BasePlayer;
					if ((Object)(object)basePlayer != (Object)null)
					{
						PlayerGrid.Remove(basePlayer);
					}
				}
			}

			public void RemovePlayer(BasePlayer player)
			{
				PlayerGrid.Remove(player);
			}

			public void RemoveBrain(BaseEntity entity)
			{
				if (!((Object)(object)entity == (Object)null))
				{
					BrainGrid.Remove(entity);
				}
			}

			public void Move(BaseEntity ent)
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_000b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0013: Unknown result type (might be due to invalid IL or missing references)
				//IL_0019: Unknown result type (might be due to invalid IL or missing references)
				Vector3 position = ((Component)ent).transform.position;
				Grid.Move(ent, position.x, position.z);
				BasePlayer basePlayer = ent as BasePlayer;
				if ((Object)(object)basePlayer != (Object)null)
				{
					MovePlayer(basePlayer);
				}
				if (ent.HasBrain)
				{
					MoveBrain(ent);
				}
			}

			public void MovePlayer(BasePlayer player)
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_000b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0013: Unknown result type (might be due to invalid IL or missing references)
				//IL_0019: Unknown result type (might be due to invalid IL or missing references)
				Vector3 position = ((Component)player).transform.position;
				PlayerGrid.Move(player, position.x, position.z);
			}

			public void MoveBrain(BaseEntity entity)
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_000b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0013: Unknown result type (might be due to invalid IL or missing references)
				//IL_0019: Unknown result type (might be due to invalid IL or missing references)
				Vector3 position = ((Component)entity).transform.position;
				BrainGrid.Move(entity, position.x, position.z);
			}

			public int GetInSphere(Vector3 position, float distance, BaseEntity[] results, Func<BaseEntity, bool> filter = null)
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				return Grid.Query(position.x, position.z, distance, results, filter);
			}

			public int GetPlayersInSphere(Vector3 position, float distance, BasePlayer[] results, Func<BasePlayer, bool> filter = null)
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				return PlayerGrid.Query(position.x, position.z, distance, results, filter);
			}

			public int GetBrainsInSphere(Vector3 position, float distance, BaseEntity[] results, Func<BaseEntity, bool> filter = null)
			{
				//IL_0006: Unknown result type (might be due to invalid IL or missing references)
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				return BrainGrid.Query(position.x, position.z, distance, results, filter);
			}
		}

		public static EntityTree Server;
	}

	public class RPC_Shared : Attribute
	{
	}

	public struct RPCMessage
	{
		public Connection connection;

		public BasePlayer player;

		public NetRead read;
	}

	public class RPC_Server : RPC_Shared
	{
		public abstract class Conditional : Attribute
		{
			public virtual string GetArgs()
			{
				return null;
			}
		}

		public class MaxDistance : Conditional
		{
			private float maximumDistance;

			public MaxDistance(float maxDist)
			{
				maximumDistance = maxDist;
			}

			public override string GetArgs()
			{
				return maximumDistance.ToString("0.00f");
			}

			public static bool Test(uint id, string debugName, BaseEntity ent, BasePlayer player, float maximumDistance)
			{
				//IL_001b: Unknown result type (might be due to invalid IL or missing references)
				if ((Object)(object)ent == (Object)null || (Object)(object)player == (Object)null)
				{
					return false;
				}
				return ent.Distance(player.eyes.position) <= maximumDistance;
			}
		}

		public class IsVisible : Conditional
		{
			private float maximumDistance;

			public IsVisible(float maxDist)
			{
				maximumDistance = maxDist;
			}

			public override string GetArgs()
			{
				return maximumDistance.ToString("0.00f");
			}

			public static bool Test(uint id, string debugName, BaseEntity ent, BasePlayer player, float maximumDistance)
			{
				//IL_001a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0025: Unknown result type (might be due to invalid IL or missing references)
				//IL_003e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0058: Unknown result type (might be due to invalid IL or missing references)
				if ((Object)(object)ent == (Object)null || (Object)(object)player == (Object)null)
				{
					return false;
				}
				if (GamePhysics.LineOfSight(player.eyes.center, player.eyes.position, 1218519041))
				{
					if (!ent.IsVisible(player.eyes.HeadRay(), 1218519041, maximumDistance))
					{
						return ent.IsVisible(player.eyes.position, maximumDistance);
					}
					return true;
				}
				return false;
			}
		}

		public class FromOwner : Conditional
		{
			public static bool Test(uint id, string debugName, BaseEntity ent, BasePlayer player)
			{
				//IL_002c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0037: Unknown result type (might be due to invalid IL or missing references)
				//IL_004b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0056: Unknown result type (might be due to invalid IL or missing references)
				if ((Object)(object)ent == (Object)null || (Object)(object)player == (Object)null)
				{
					return false;
				}
				if (ent.net == null || player.net == null)
				{
					return false;
				}
				if (ent.net.ID == player.net.ID)
				{
					return true;
				}
				if (ent.parentEntity.uid != player.net.ID)
				{
					return false;
				}
				return true;
			}
		}

		public class IsActiveItem : Conditional
		{
			public static bool Test(uint id, string debugName, BaseEntity ent, BasePlayer player)
			{
				//IL_002c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0037: Unknown result type (might be due to invalid IL or missing references)
				//IL_004b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0056: Unknown result type (might be due to invalid IL or missing references)
				if ((Object)(object)ent == (Object)null || (Object)(object)player == (Object)null)
				{
					return false;
				}
				if (ent.net == null || player.net == null)
				{
					return false;
				}
				if (ent.net.ID == player.net.ID)
				{
					return true;
				}
				if (ent.parentEntity.uid != player.net.ID)
				{
					return false;
				}
				Item activeItem = player.GetActiveItem();
				if (activeItem == null)
				{
					return false;
				}
				if ((Object)(object)activeItem.GetHeldEntity() != (Object)(object)ent)
				{
					return false;
				}
				return true;
			}
		}

		public class CallsPerSecond : Conditional
		{
			private ulong callsPerSecond;

			public CallsPerSecond(ulong limit)
			{
				callsPerSecond = limit;
			}

			public override string GetArgs()
			{
				return callsPerSecond.ToString();
			}

			public static bool Test(uint id, string debugName, BaseEntity ent, BasePlayer player, ulong callsPerSecond)
			{
				if ((Object)(object)ent == (Object)null || (Object)(object)player == (Object)null)
				{
					return false;
				}
				return player.rpcHistory.TryIncrement(id, callsPerSecond);
			}
		}
	}

	public enum Signal
	{
		Attack,
		Alt_Attack,
		DryFire,
		Reload,
		Deploy,
		Flinch_Head,
		Flinch_Chest,
		Flinch_Stomach,
		Flinch_RearHead,
		Flinch_RearTorso,
		Throw,
		Relax,
		Gesture,
		PhysImpact,
		Eat,
		Startled,
		Admire
	}

	public enum Slot
	{
		Lock,
		FireMod,
		UpperModifier,
		MiddleModifier,
		LowerModifier,
		CenterDecoration,
		LowerCenterDecoration,
		StorageMonitor,
		Count
	}

	[Flags]
	public enum TraitFlag
	{
		None = 0,
		Alive = 1,
		Animal = 2,
		Human = 4,
		Interesting = 8,
		Food = 0x10,
		Meat = 0x20,
		Water = 0x20
	}

	public static class Util
	{
		public static BaseEntity[] FindTargets(string strFilter, bool onlyPlayers)
		{
			return (from x in BaseNetworkable.serverEntities.Where(delegate(BaseNetworkable x)
				{
					if (x is BasePlayer)
					{
						BasePlayer basePlayer = x as BasePlayer;
						if (string.IsNullOrEmpty(strFilter))
						{
							return true;
						}
						if (strFilter == "!alive" && basePlayer.IsAlive())
						{
							return true;
						}
						if (strFilter == "!sleeping" && basePlayer.IsSleeping())
						{
							return true;
						}
						if (strFilter[0] != '!' && !StringEx.Contains(basePlayer.displayName, strFilter, CompareOptions.IgnoreCase) && !basePlayer.UserIDString.Contains(strFilter))
						{
							return false;
						}
						return true;
					}
					if (onlyPlayers)
					{
						return false;
					}
					if (string.IsNullOrEmpty(strFilter))
					{
						return false;
					}
					return x.ShortPrefabName.Contains(strFilter) ? true : false;
				})
				select x as BaseEntity).ToArray();
		}

		public static BaseEntity[] FindTargetsOwnedBy(ulong ownedBy, string strFilter)
		{
			bool hasFilter = !string.IsNullOrEmpty(strFilter);
			return (from x in BaseNetworkable.serverEntities.Where(delegate(BaseNetworkable x)
				{
					if (x is BaseEntity baseEntity)
					{
						if (baseEntity.OwnerID != ownedBy)
						{
							return false;
						}
						if (!hasFilter || baseEntity.ShortPrefabName.Contains(strFilter))
						{
							return true;
						}
					}
					return false;
				})
				select x as BaseEntity).ToArray();
		}

		public static BaseEntity[] FindTargetsAuthedTo(ulong authId, string strFilter)
		{
			bool hasFilter = !string.IsNullOrEmpty(strFilter);
			return (from x in BaseNetworkable.serverEntities.Where(delegate(BaseNetworkable x)
				{
					if (x is BuildingPrivlidge buildingPrivlidge)
					{
						if (!buildingPrivlidge.IsAuthed(authId))
						{
							return false;
						}
						if (!hasFilter || x.ShortPrefabName.Contains(strFilter))
						{
							return true;
						}
					}
					else if (x is AutoTurret autoTurret)
					{
						if (!autoTurret.IsAuthed(authId))
						{
							return false;
						}
						if (!hasFilter || x.ShortPrefabName.Contains(strFilter))
						{
							return true;
						}
					}
					else if (x is CodeLock codeLock)
					{
						if (!codeLock.whitelistPlayers.Contains(authId))
						{
							return false;
						}
						if (!hasFilter || x.ShortPrefabName.Contains(strFilter))
						{
							return true;
						}
					}
					return false;
				})
				select x as BaseEntity).ToArray();
		}

		public static T[] FindAll<T>() where T : BaseEntity
		{
			return BaseNetworkable.serverEntities.OfType<T>().ToArray();
		}
	}

	[Header("BaseEntity")]
	public Bounds bounds;

	public GameObjectRef impactEffect;

	public bool enableSaving = true;

	public bool syncPosition;

	public Model model;

	[InspectorFlags]
	public Flags flags;

	[NonSerialized]
	public uint parentBone;

	[NonSerialized]
	public ulong skinID;

	private EntityComponentBase[] _components;

	[HideInInspector]
	public bool HasBrain;

	[NonSerialized]
	protected string _name;

	private static Queue<BaseEntity> globalBroadcastQueue = new Queue<BaseEntity>();

	private static uint globalBroadcastProtocol = 0u;

	private uint broadcastProtocol;

	private List<EntityLink> links = new List<EntityLink>();

	private bool linkedToNeighbours;

	private TimeUntil _transferProtectionRemaining;

	private Action _disableTransferProtectionAction;

	private Spawnable _spawnable;

	public static HashSet<BaseEntity> saveList = new HashSet<BaseEntity>();

	[NonSerialized]
	public BaseEntity creatorEntity;

	private int ticksSinceStopped;

	private int doneMovingWithoutARigidBodyCheck = 1;

	private bool isCallingUpdateNetworkGroup;

	private EntityRef[] entitySlots = new EntityRef[8];

	protected List<TriggerBase> triggers;

	protected bool isVisible = true;

	protected bool isAnimatorVisible = true;

	protected bool isShadowVisible = true;

	protected OccludeeSphere localOccludee = new OccludeeSphere(-1);

	public virtual float RealisticMass => 100f;

	public EntityComponentBase[] Components => _components ?? (_components = ((Component)this).GetComponentsInChildren<EntityComponentBase>(true));

	public virtual bool IsNpc => false;

	public ulong OwnerID { get; set; }

	protected float TransferProtectionRemaining => TimeUntil.op_Implicit(_transferProtectionRemaining);

	protected Action DisableTransferProtectionAction => _disableTransferProtectionAction ?? (_disableTransferProtectionAction = DisableTransferProtection);

	public virtual bool ShouldTransferAssociatedFiles => false;

	protected virtual float PositionTickRate => 0.1f;

	protected virtual bool PositionTickFixedTime => false;

	public virtual Vector3 ServerPosition
	{
		get
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return ((Component)this).transform.localPosition;
		}
		set
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			if (!(((Component)this).transform.localPosition == value))
			{
				((Component)this).transform.localPosition = value;
				((Component)this).transform.hasChanged = true;
			}
		}
	}

	public virtual Quaternion ServerRotation
	{
		get
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return ((Component)this).transform.localRotation;
		}
		set
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			if (!(((Component)this).transform.localRotation == value))
			{
				((Component)this).transform.localRotation = value;
				((Component)this).transform.hasChanged = true;
			}
		}
	}

	public float radiationLevel
	{
		get
		{
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			if (triggers == null)
			{
				return 0f;
			}
			float num = 0f;
			for (int i = 0; i < triggers.Count; i++)
			{
				TriggerRadiation triggerRadiation = triggers[i] as TriggerRadiation;
				if (!((Object)(object)triggerRadiation == (Object)null))
				{
					Vector3 val = GetNetworkPosition();
					BaseEntity baseEntity = GetParentEntity();
					if ((Object)(object)baseEntity != (Object)null)
					{
						val = ((Component)baseEntity).transform.TransformPoint(val);
					}
					num = Mathf.Max(num, triggerRadiation.GetRadiation(val, RadiationProtection()));
				}
			}
			return num;
		}
	}

	public float currentTemperature
	{
		get
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			float num = Climate.GetTemperature(((Component)this).transform.position);
			if (triggers == null)
			{
				return num;
			}
			for (int i = 0; i < triggers.Count; i++)
			{
				TriggerTemperature triggerTemperature = triggers[i] as TriggerTemperature;
				if (!((Object)(object)triggerTemperature == (Object)null))
				{
					num = triggerTemperature.WorkoutTemperature(((Component)this).transform.position, num);
				}
			}
			return num;
		}
	}

	public float currentEnvironmentalWetness
	{
		get
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			if (triggers == null)
			{
				return 0f;
			}
			float num = 0f;
			Vector3 networkPosition = GetNetworkPosition();
			foreach (TriggerBase trigger in triggers)
			{
				if (trigger is TriggerWetness triggerWetness)
				{
					num += triggerWetness.WorkoutWetness(networkPosition);
				}
			}
			return Mathf.Clamp01(num);
		}
	}

	public virtual TraitFlag Traits => TraitFlag.None;

	public float Weight { get; protected set; }

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("BaseEntity.OnRpcMessage", 0);
		try
		{
			if (rpc == 1552640099 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - BroadcastSignalFromClient "));
				}
				TimeWarning val2 = TimeWarning.New("BroadcastSignalFromClient", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.FromOwner.Test(1552640099u, "BroadcastSignalFromClient", this, player))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
					try
					{
						val3 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage msg2 = rPCMessage;
							BroadcastSignalFromClient(msg2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in BroadcastSignalFromClient");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 3645147041u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - SV_RequestFile "));
				}
				TimeWarning val2 = TimeWarning.New("SV_RequestFile", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Call", 0);
					try
					{
						RPCMessage rPCMessage = default(RPCMessage);
						rPCMessage.connection = msg.connection;
						rPCMessage.player = player;
						rPCMessage.read = msg.read;
						RPCMessage msg3 = rPCMessage;
						SV_RequestFile(msg3);
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
				}
				catch (Exception ex2)
				{
					Debug.LogException(ex2);
					player.Kick("RPC Error in SV_RequestFile");
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	public virtual void OnCollision(Collision collision, BaseEntity hitEntity)
	{
		throw new NotImplementedException();
	}

	protected void ReceiveCollisionMessages(bool b)
	{
		if (b)
		{
			TransformEx.GetOrAddComponent<EntityCollisionMessage>(((Component)this).gameObject.transform);
		}
		else
		{
			((Component)this).gameObject.transform.RemoveComponent<EntityCollisionMessage>();
		}
	}

	public virtual BasePlayer ToPlayer()
	{
		return null;
	}

	public override void InitShared()
	{
		base.InitShared();
		InitEntityLinks();
	}

	public override void DestroyShared()
	{
		base.DestroyShared();
		FreeEntityLinks();
	}

	public override void ResetState()
	{
		base.ResetState();
		parentBone = 0u;
		OwnerID = 0uL;
		flags = (Flags)0;
		parentEntity = default(EntityRef);
		if (base.isServer)
		{
			_spawnable = null;
		}
	}

	public virtual float InheritedVelocityScale()
	{
		return 0f;
	}

	public virtual bool InheritedVelocityDirection()
	{
		return true;
	}

	public virtual Vector3 GetInheritedProjectileVelocity(Vector3 direction)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		BaseEntity baseEntity = parentEntity.Get(base.isServer);
		if ((Object)(object)baseEntity == (Object)null)
		{
			return Vector3.zero;
		}
		if (baseEntity.InheritedVelocityDirection())
		{
			return GetParentVelocity() * baseEntity.InheritedVelocityScale();
		}
		return Mathf.Max(Vector3.Dot(GetParentVelocity() * baseEntity.InheritedVelocityScale(), direction), 0f) * direction;
	}

	public virtual Vector3 GetInheritedThrowVelocity(Vector3 direction)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return GetParentVelocity();
	}

	public virtual Vector3 GetInheritedDropVelocity()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		BaseEntity baseEntity = parentEntity.Get(base.isServer);
		if (!((Object)(object)baseEntity != (Object)null))
		{
			return Vector3.zero;
		}
		return baseEntity.GetWorldVelocity();
	}

	public Vector3 GetParentVelocity()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		BaseEntity baseEntity = parentEntity.Get(base.isServer);
		if (!((Object)(object)baseEntity != (Object)null))
		{
			return Vector3.zero;
		}
		return baseEntity.GetWorldVelocity() + (baseEntity.GetAngularVelocity() * ((Component)this).transform.localPosition - ((Component)this).transform.localPosition);
	}

	public Vector3 GetWorldVelocity()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		BaseEntity baseEntity = parentEntity.Get(base.isServer);
		if (!((Object)(object)baseEntity != (Object)null))
		{
			return GetLocalVelocity();
		}
		return baseEntity.GetWorldVelocity() + (baseEntity.GetAngularVelocity() * ((Component)this).transform.localPosition - ((Component)this).transform.localPosition) + ((Component)baseEntity).transform.TransformDirection(GetLocalVelocity());
	}

	public Vector3 GetLocalVelocity()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		if (base.isServer)
		{
			return GetLocalVelocityServer();
		}
		return Vector3.zero;
	}

	public Quaternion GetAngularVelocity()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		if (base.isServer)
		{
			return GetAngularVelocityServer();
		}
		return Quaternion.identity;
	}

	public virtual OBB WorldSpaceBounds()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		return new OBB(((Component)this).transform.position, ((Component)this).transform.lossyScale, ((Component)this).transform.rotation, bounds);
	}

	public Vector3 PivotPoint()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return ((Component)this).transform.position;
	}

	public Vector3 CenterPoint()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return WorldSpaceBounds().position;
	}

	public Vector3 ClosestPoint(Vector3 position)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		OBB val = WorldSpaceBounds();
		return ((OBB)(ref val)).ClosestPoint(position);
	}

	public virtual Vector3 TriggerPoint()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return CenterPoint();
	}

	public float Distance(Vector3 position)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = ClosestPoint(position) - position;
		return ((Vector3)(ref val)).magnitude;
	}

	public float SqrDistance(Vector3 position)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = ClosestPoint(position) - position;
		return ((Vector3)(ref val)).sqrMagnitude;
	}

	public float Distance(BaseEntity other)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return Distance(((Component)other).transform.position);
	}

	public float SqrDistance(BaseEntity other)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return SqrDistance(((Component)other).transform.position);
	}

	public float Distance2D(Vector3 position)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		return Vector3Ex.Magnitude2D(ClosestPoint(position) - position);
	}

	public float SqrDistance2D(Vector3 position)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		return Vector3Ex.SqrMagnitude2D(ClosestPoint(position) - position);
	}

	public float Distance2D(BaseEntity other)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return Distance(((Component)other).transform.position);
	}

	public float SqrDistance2D(BaseEntity other)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return SqrDistance(((Component)other).transform.position);
	}

	public bool IsVisible(Ray ray, int layerMask, float maxDistance)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		if (Vector3Ex.IsNaNOrInfinity(((Ray)(ref ray)).origin))
		{
			return false;
		}
		if (Vector3Ex.IsNaNOrInfinity(((Ray)(ref ray)).direction))
		{
			return false;
		}
		if (((Ray)(ref ray)).direction == Vector3.zero)
		{
			return false;
		}
		OBB val = WorldSpaceBounds();
		RaycastHit val2 = default(RaycastHit);
		if (!((OBB)(ref val)).Trace(ray, ref val2, maxDistance))
		{
			return false;
		}
		if (GamePhysics.Trace(ray, 0f, out var hitInfo, maxDistance, layerMask, (QueryTriggerInteraction)0))
		{
			BaseEntity entity = hitInfo.GetEntity();
			if ((Object)(object)entity == (Object)(object)this)
			{
				return true;
			}
			if ((Object)(object)entity != (Object)null && Object.op_Implicit((Object)(object)GetParentEntity()) && GetParentEntity().EqualNetID((BaseNetworkable)entity) && hitInfo.IsOnLayer((Layer)13))
			{
				return true;
			}
			if (((RaycastHit)(ref hitInfo)).distance <= ((RaycastHit)(ref val2)).distance)
			{
				return false;
			}
		}
		return true;
	}

	public bool IsVisibleSpecificLayers(Vector3 position, Vector3 target, int layerMask, float maxDistance = float.PositiveInfinity)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = target - position;
		float magnitude = ((Vector3)(ref val)).magnitude;
		if (magnitude < Mathf.Epsilon)
		{
			return true;
		}
		Vector3 val2 = val / magnitude;
		Vector3 val3 = val2 * Mathf.Min(magnitude, 0.01f);
		return IsVisible(new Ray(position + val3, val2), layerMask, maxDistance);
	}

	public bool IsVisible(Vector3 position, Vector3 target, float maxDistance = float.PositiveInfinity)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = target - position;
		float magnitude = ((Vector3)(ref val)).magnitude;
		if (magnitude < Mathf.Epsilon)
		{
			return true;
		}
		Vector3 val2 = val / magnitude;
		Vector3 val3 = val2 * Mathf.Min(magnitude, 0.01f);
		return IsVisible(new Ray(position + val3, val2), 1218519041, maxDistance);
	}

	public bool IsVisible(Vector3 position, float maxDistance = float.PositiveInfinity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		Vector3 target = CenterPoint();
		if (IsVisible(position, target, maxDistance))
		{
			return true;
		}
		Vector3 target2 = ClosestPoint(position);
		if (IsVisible(position, target2, maxDistance))
		{
			return true;
		}
		return false;
	}

	public bool IsVisibleAndCanSee(Vector3 position)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = CenterPoint();
		if (IsVisible(position, val) && CanSee(val, position))
		{
			return true;
		}
		Vector3 val2 = ClosestPoint(position);
		if (IsVisible(position, val2) && CanSee(val2, position))
		{
			return true;
		}
		return false;
	}

	public bool CanSee(Vector3 fromPos, Vector3 targetPos)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return GamePhysics.LineOfSight(fromPos, targetPos, 1218519041, this);
	}

	public bool IsOlderThan(BaseEntity other)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)other == (Object)null)
		{
			return true;
		}
		_003F val = ((_003F?)net?.ID) ?? default(NetworkableId);
		NetworkableId val2 = (NetworkableId)(((_003F?)other.net?.ID) ?? default(NetworkableId));
		return ((NetworkableId)val).Value < val2.Value;
	}

	public virtual bool IsOutside()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		OBB val = WorldSpaceBounds();
		return IsOutside(val.position);
	}

	public bool IsOutside(Vector3 position)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		bool result = true;
		RaycastHit val = default(RaycastHit);
		if (Physics.Raycast(position + Vector3.up * 100f, Vector3.down, ref val, 100f, 161546513))
		{
			BaseEntity baseEntity = ((RaycastHit)(ref val)).collider.ToBaseEntity();
			if ((Object)(object)baseEntity == (Object)null || !baseEntity.HasEntityInParents(this))
			{
				result = false;
			}
		}
		return result;
	}

	public virtual float WaterFactor()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		OBB val = WorldSpaceBounds();
		return WaterLevel.Factor(((OBB)(ref val)).ToBounds(), waves: true, volumes: true, this);
	}

	public virtual float AirFactor()
	{
		if (!(WaterFactor() > 0.85f))
		{
			return 1f;
		}
		return 0f;
	}

	public bool WaterTestFromVolumes(Vector3 pos, out WaterLevel.WaterInfo info)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		if (triggers == null)
		{
			info = default(WaterLevel.WaterInfo);
			return false;
		}
		for (int i = 0; i < triggers.Count; i++)
		{
			if (triggers[i] is WaterVolume waterVolume && waterVolume.Test(pos, out info))
			{
				return true;
			}
		}
		info = default(WaterLevel.WaterInfo);
		return false;
	}

	public bool IsInWaterVolume(Vector3 pos)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (triggers == null)
		{
			return false;
		}
		for (int i = 0; i < triggers.Count; i++)
		{
			if (triggers[i] is WaterVolume waterVolume && waterVolume.Test(pos, out var _))
			{
				return true;
			}
		}
		return false;
	}

	public bool WaterTestFromVolumes(Bounds bounds, out WaterLevel.WaterInfo info)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		if (triggers == null)
		{
			info = default(WaterLevel.WaterInfo);
			return false;
		}
		for (int i = 0; i < triggers.Count; i++)
		{
			if (triggers[i] is WaterVolume waterVolume && waterVolume.Test(bounds, out info))
			{
				return true;
			}
		}
		info = default(WaterLevel.WaterInfo);
		return false;
	}

	public bool WaterTestFromVolumes(Vector3 start, Vector3 end, float radius, out WaterLevel.WaterInfo info)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		if (triggers == null)
		{
			info = default(WaterLevel.WaterInfo);
			return false;
		}
		for (int i = 0; i < triggers.Count; i++)
		{
			if (triggers[i] is WaterVolume waterVolume && waterVolume.Test(start, end, radius, out info))
			{
				return true;
			}
		}
		info = default(WaterLevel.WaterInfo);
		return false;
	}

	public virtual bool BlocksWaterFor(BasePlayer player)
	{
		return false;
	}

	public virtual float Health()
	{
		return 0f;
	}

	public virtual float MaxHealth()
	{
		return 0f;
	}

	public virtual float MaxVelocity()
	{
		return 0f;
	}

	public virtual float BoundsPadding()
	{
		return 0.1f;
	}

	public virtual float PenetrationResistance(HitInfo info)
	{
		return 100f;
	}

	public virtual GameObjectRef GetImpactEffect(HitInfo info)
	{
		return impactEffect;
	}

	public virtual void OnAttacked(HitInfo info)
	{
	}

	public virtual Item GetItem()
	{
		return null;
	}

	public virtual Item GetItem(ItemId itemId)
	{
		return null;
	}

	public virtual void GiveItem(Item item, GiveItemReason reason = GiveItemReason.Generic)
	{
		item.Remove();
	}

	public virtual bool CanBeLooted(BasePlayer player)
	{
		return !IsTransferring();
	}

	public virtual BaseEntity GetEntity()
	{
		return this;
	}

	public override string ToString()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		if (_name == null)
		{
			if (base.isServer)
			{
				_name = string.Format("{1}[{0}]", (object)(NetworkableId)(((_003F?)net?.ID) ?? default(NetworkableId)), base.ShortPrefabName);
			}
			else
			{
				_name = base.ShortPrefabName;
			}
		}
		return _name;
	}

	public virtual string Categorize()
	{
		return "entity";
	}

	public void Log(string str)
	{
		if (base.isClient)
		{
			Debug.Log((object)("<color=#ffa>[" + ((object)this).ToString() + "] " + str + "</color>"), (Object)(object)((Component)this).gameObject);
		}
		else
		{
			Debug.Log((object)("<color=#aff>[" + ((object)this).ToString() + "] " + str + "</color>"), (Object)(object)((Component)this).gameObject);
		}
	}

	public void SetModel(Model mdl)
	{
		if (!((Object)(object)model == (Object)(object)mdl))
		{
			model = mdl;
		}
	}

	public Model GetModel()
	{
		return model;
	}

	public virtual Transform[] GetBones()
	{
		if (Object.op_Implicit((Object)(object)model))
		{
			return model.GetBones();
		}
		return null;
	}

	public virtual Transform FindBone(string strName)
	{
		if (Object.op_Implicit((Object)(object)model))
		{
			return model.FindBone(strName);
		}
		return ((Component)this).transform;
	}

	public virtual uint FindBoneID(Transform boneTransform)
	{
		if (Object.op_Implicit((Object)(object)model))
		{
			return model.FindBoneID(boneTransform);
		}
		return StringPool.closest;
	}

	public virtual Transform FindClosestBone(Vector3 worldPos)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		if (Object.op_Implicit((Object)(object)model))
		{
			return model.FindClosestBone(worldPos);
		}
		return ((Component)this).transform;
	}

	public virtual bool ShouldBlockProjectiles()
	{
		return true;
	}

	public virtual bool ShouldInheritNetworkGroup()
	{
		return true;
	}

	public virtual bool SupportsChildDeployables()
	{
		return false;
	}

	public virtual bool ForceDeployableSetParent()
	{
		return false;
	}

	public bool IsOnMovingObject()
	{
		if (syncPosition)
		{
			return true;
		}
		BaseEntity baseEntity = GetParentEntity();
		if (!((Object)(object)baseEntity != (Object)null))
		{
			return false;
		}
		return baseEntity.IsOnMovingObject();
	}

	public void BroadcastEntityMessage(string msg, float radius = 20f, int layerMask = 1218652417)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		if (base.isClient)
		{
			return;
		}
		List<BaseEntity> list = Pool.GetList<BaseEntity>();
		Vis.Entities(((Component)this).transform.position, radius, list, layerMask, (QueryTriggerInteraction)2);
		foreach (BaseEntity item in list)
		{
			if (item.isServer)
			{
				item.OnEntityMessage(this, msg);
			}
		}
		Pool.FreeList<BaseEntity>(ref list);
	}

	public virtual void OnEntityMessage(BaseEntity from, string msg)
	{
	}

	public virtual void DebugServer(int rep, float time)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		DebugText(((Component)this).transform.position + Vector3.up * 1f, $"{net?.ID.Value ?? 0}: {((Object)this).name}\n{DebugText()}", Color.white, time);
	}

	public virtual string DebugText()
	{
		return "";
	}

	public void OnDebugStart()
	{
		EntityDebug entityDebug = ((Component)this).gameObject.GetComponent<EntityDebug>();
		if ((Object)(object)entityDebug == (Object)null)
		{
			entityDebug = ((Component)this).gameObject.AddComponent<EntityDebug>();
		}
		((Behaviour)entityDebug).enabled = true;
	}

	protected void DebugText(Vector3 pos, string str, Color color, float time)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		if (base.isServer)
		{
			ConsoleNetwork.BroadcastToAllClients("ddraw.text", time, color, pos, str);
		}
	}

	public bool HasFlag(Flags f)
	{
		return (flags & f) == f;
	}

	public bool HasAny(Flags f)
	{
		return (flags & f) > (Flags)0;
	}

	public bool ParentHasFlag(Flags f)
	{
		BaseEntity baseEntity = GetParentEntity();
		if ((Object)(object)baseEntity == (Object)null)
		{
			return false;
		}
		return baseEntity.HasFlag(f);
	}

	public void SetFlag(Flags f, bool b, bool recursive = false, bool networkupdate = true)
	{
		Flags flags = this.flags;
		if (b)
		{
			if (HasFlag(f))
			{
				return;
			}
			this.flags |= f;
		}
		else
		{
			if (!HasFlag(f))
			{
				return;
			}
			this.flags &= ~f;
		}
		OnFlagsChanged(flags, this.flags);
		if (networkupdate)
		{
			SendNetworkUpdate();
			if (flags != this.flags)
			{
				GlobalNetworkHandler.server?.TrySendNetworkUpdate(this);
			}
		}
		else
		{
			InvalidateNetworkCache();
		}
		if (recursive && children != null)
		{
			for (int i = 0; i < children.Count; i++)
			{
				children[i].SetFlag(f, b, recursive: true);
			}
		}
	}

	public bool IsOn()
	{
		return HasFlag(Flags.On);
	}

	public bool IsOpen()
	{
		return HasFlag(Flags.Open);
	}

	public bool IsOnFire()
	{
		return HasFlag(Flags.OnFire);
	}

	public bool IsLocked()
	{
		return HasFlag(Flags.Locked);
	}

	public override bool IsDebugging()
	{
		return HasFlag(Flags.Debugging);
	}

	public bool IsDisabled()
	{
		if (!HasFlag(Flags.Disabled))
		{
			return ParentHasFlag(Flags.Disabled);
		}
		return true;
	}

	public bool IsBroken()
	{
		return HasFlag(Flags.Broken);
	}

	public bool IsBusy()
	{
		return HasFlag(Flags.Busy);
	}

	public bool IsTransferProtected()
	{
		return HasFlag(Flags.Protected);
	}

	public bool IsTransferring()
	{
		return HasFlag(Flags.Transferring);
	}

	public override string GetLogColor()
	{
		if (base.isServer)
		{
			return "cyan";
		}
		return "yellow";
	}

	public virtual void OnFlagsChanged(Flags old, Flags next)
	{
		if (IsDebugging() && (old & Flags.Debugging) != (next & Flags.Debugging))
		{
			OnDebugStart();
		}
	}

	protected void SendNetworkUpdate_Flags()
	{
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		if (Application.isLoading || Application.isLoadingSave || base.IsDestroyed || net == null || !isSpawned)
		{
			return;
		}
		TimeWarning val = TimeWarning.New("SendNetworkUpdate_Flags", 0);
		try
		{
			LogEntry(LogEntryType.Network, 2, "SendNetworkUpdate_Flags");
			List<Connection> subscribers = GetSubscribers();
			if (subscribers != null && subscribers.Count > 0)
			{
				NetWrite obj = ((BaseNetwork)Net.sv).StartWrite();
				obj.PacketID((Type)23);
				obj.EntityID(net.ID);
				obj.Int32((int)flags);
				SendInfo val2 = default(SendInfo);
				((SendInfo)(ref val2))._002Ector(subscribers);
				obj.Send(val2);
			}
			((Component)this).gameObject.SendOnSendNetworkUpdate(this);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public bool IsOccupied(Socket_Base socket)
	{
		return FindLink(socket)?.IsOccupied() ?? false;
	}

	public bool IsOccupied(string socketName)
	{
		return FindLink(socketName)?.IsOccupied() ?? false;
	}

	public EntityLink FindLink(Socket_Base socket)
	{
		List<EntityLink> entityLinks = GetEntityLinks();
		for (int i = 0; i < entityLinks.Count; i++)
		{
			if (entityLinks[i].socket == socket)
			{
				return entityLinks[i];
			}
		}
		return null;
	}

	public EntityLink FindLink(string socketName)
	{
		List<EntityLink> entityLinks = GetEntityLinks();
		for (int i = 0; i < entityLinks.Count; i++)
		{
			if (entityLinks[i].socket.socketName == socketName)
			{
				return entityLinks[i];
			}
		}
		return null;
	}

	public EntityLink FindLink(string[] socketNames)
	{
		List<EntityLink> entityLinks = GetEntityLinks();
		for (int i = 0; i < entityLinks.Count; i++)
		{
			for (int j = 0; j < socketNames.Length; j++)
			{
				if (entityLinks[i].socket.socketName == socketNames[j])
				{
					return entityLinks[i];
				}
			}
		}
		return null;
	}

	public T FindLinkedEntity<T>() where T : BaseEntity
	{
		List<EntityLink> entityLinks = GetEntityLinks();
		for (int i = 0; i < entityLinks.Count; i++)
		{
			EntityLink entityLink = entityLinks[i];
			for (int j = 0; j < entityLink.connections.Count; j++)
			{
				EntityLink entityLink2 = entityLink.connections[j];
				if (entityLink2.owner is T)
				{
					return entityLink2.owner as T;
				}
			}
		}
		return null;
	}

	public void EntityLinkMessage<T>(Action<T> action) where T : BaseEntity
	{
		List<EntityLink> entityLinks = GetEntityLinks();
		for (int i = 0; i < entityLinks.Count; i++)
		{
			EntityLink entityLink = entityLinks[i];
			for (int j = 0; j < entityLink.connections.Count; j++)
			{
				EntityLink entityLink2 = entityLink.connections[j];
				if (entityLink2.owner is T)
				{
					action(entityLink2.owner as T);
				}
			}
		}
	}

	public void EntityLinkBroadcast<T, S>(Action<T> action, Func<S, bool> canTraverseSocket) where T : BaseEntity where S : Socket_Base
	{
		globalBroadcastProtocol++;
		globalBroadcastQueue.Clear();
		broadcastProtocol = globalBroadcastProtocol;
		globalBroadcastQueue.Enqueue(this);
		if (this is T)
		{
			action(this as T);
		}
		while (globalBroadcastQueue.Count > 0)
		{
			List<EntityLink> entityLinks = globalBroadcastQueue.Dequeue().GetEntityLinks();
			for (int i = 0; i < entityLinks.Count; i++)
			{
				EntityLink entityLink = entityLinks[i];
				if (!(entityLink.socket is S) || !canTraverseSocket(entityLink.socket as S))
				{
					continue;
				}
				for (int j = 0; j < entityLink.connections.Count; j++)
				{
					BaseEntity owner = entityLink.connections[j].owner;
					if (owner.broadcastProtocol != globalBroadcastProtocol)
					{
						owner.broadcastProtocol = globalBroadcastProtocol;
						globalBroadcastQueue.Enqueue(owner);
						if (owner is T)
						{
							action(owner as T);
						}
					}
				}
			}
		}
	}

	public void EntityLinkBroadcast<T>(Action<T> action) where T : BaseEntity
	{
		globalBroadcastProtocol++;
		globalBroadcastQueue.Clear();
		broadcastProtocol = globalBroadcastProtocol;
		globalBroadcastQueue.Enqueue(this);
		if (this is T)
		{
			action(this as T);
		}
		while (globalBroadcastQueue.Count > 0)
		{
			List<EntityLink> entityLinks = globalBroadcastQueue.Dequeue().GetEntityLinks();
			for (int i = 0; i < entityLinks.Count; i++)
			{
				EntityLink entityLink = entityLinks[i];
				for (int j = 0; j < entityLink.connections.Count; j++)
				{
					BaseEntity owner = entityLink.connections[j].owner;
					if (owner.broadcastProtocol != globalBroadcastProtocol)
					{
						owner.broadcastProtocol = globalBroadcastProtocol;
						globalBroadcastQueue.Enqueue(owner);
						if (owner is T)
						{
							action(owner as T);
						}
					}
				}
			}
		}
	}

	public void EntityLinkBroadcast()
	{
		globalBroadcastProtocol++;
		globalBroadcastQueue.Clear();
		broadcastProtocol = globalBroadcastProtocol;
		globalBroadcastQueue.Enqueue(this);
		while (globalBroadcastQueue.Count > 0)
		{
			List<EntityLink> entityLinks = globalBroadcastQueue.Dequeue().GetEntityLinks();
			for (int i = 0; i < entityLinks.Count; i++)
			{
				EntityLink entityLink = entityLinks[i];
				for (int j = 0; j < entityLink.connections.Count; j++)
				{
					BaseEntity owner = entityLink.connections[j].owner;
					if (owner.broadcastProtocol != globalBroadcastProtocol)
					{
						owner.broadcastProtocol = globalBroadcastProtocol;
						globalBroadcastQueue.Enqueue(owner);
					}
				}
			}
		}
	}

	public bool ReceivedEntityLinkBroadcast()
	{
		return broadcastProtocol == globalBroadcastProtocol;
	}

	public List<EntityLink> GetEntityLinks(bool linkToNeighbours = true)
	{
		if (Application.isLoadingSave)
		{
			return links;
		}
		if (!linkedToNeighbours && linkToNeighbours)
		{
			LinkToNeighbours();
		}
		return links;
	}

	private void LinkToEntity(BaseEntity other)
	{
		if ((Object)(object)this == (Object)(object)other || links.Count == 0 || other.links.Count == 0)
		{
			return;
		}
		TimeWarning val = TimeWarning.New("LinkToEntity", 0);
		try
		{
			for (int i = 0; i < links.Count; i++)
			{
				EntityLink entityLink = links[i];
				for (int j = 0; j < other.links.Count; j++)
				{
					EntityLink entityLink2 = other.links[j];
					if (entityLink.CanConnect(entityLink2))
					{
						if (!entityLink.Contains(entityLink2))
						{
							entityLink.Add(entityLink2);
						}
						if (!entityLink2.Contains(entityLink))
						{
							entityLink2.Add(entityLink);
						}
					}
				}
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private void LinkToNeighbours()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		if (links.Count == 0)
		{
			return;
		}
		linkedToNeighbours = true;
		TimeWarning val = TimeWarning.New("LinkToNeighbours", 0);
		try
		{
			List<BaseEntity> list = Pool.GetList<BaseEntity>();
			OBB val2 = WorldSpaceBounds();
			Vis.Entities(val2.position, ((Vector3)(ref val2.extents)).magnitude + 1f, list, -1, (QueryTriggerInteraction)2);
			for (int i = 0; i < list.Count; i++)
			{
				BaseEntity baseEntity = list[i];
				if (baseEntity.isServer == base.isServer)
				{
					LinkToEntity(baseEntity);
				}
			}
			Pool.FreeList<BaseEntity>(ref list);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private void InitEntityLinks()
	{
		TimeWarning val = TimeWarning.New("InitEntityLinks", 0);
		try
		{
			if (base.isServer)
			{
				links.AddLinks(this, PrefabAttribute.server.FindAll<Socket_Base>(prefabID));
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private void FreeEntityLinks()
	{
		TimeWarning val = TimeWarning.New("FreeEntityLinks", 0);
		try
		{
			links.FreeLinks();
			linkedToNeighbours = false;
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void RefreshEntityLinks()
	{
		TimeWarning val = TimeWarning.New("RefreshEntityLinks", 0);
		try
		{
			links.ClearLinks();
			LinkToNeighbours();
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	[RPC_Server]
	public void SV_RequestFile(RPCMessage msg)
	{
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		uint num = msg.read.UInt32();
		FileStorage.Type type = (FileStorage.Type)msg.read.UInt8();
		string funcName = StringPool.Get(msg.read.UInt32());
		uint num2 = ((msg.read.Unread > 0) ? msg.read.UInt32() : 0u);
		bool flag = msg.read.Unread > 0 && msg.read.Bit();
		byte[] array = FileStorage.server.Get(num, type, net.ID, num2);
		if (array == null)
		{
			if (!flag)
			{
				return;
			}
			array = Array.Empty<byte>();
		}
		SendInfo val = default(SendInfo);
		((SendInfo)(ref val))._002Ector(msg.connection);
		val.channel = 2;
		val.method = (SendMethod)0;
		SendInfo sendInfo = val;
		ClientRPCEx(sendInfo, null, funcName, num, (uint)array.Length, array, num2, (byte)type);
	}

	public virtual void EnableTransferProtection()
	{
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		if (!IsTransferProtected())
		{
			SetFlag(Flags.Protected, b: true);
			List<Connection> subscribers = GetSubscribers();
			if (subscribers != null)
			{
				List<Connection> list = Pool.GetList<Connection>();
				foreach (Connection item in subscribers)
				{
					if (!ShouldNetworkTo(item.player as BasePlayer))
					{
						list.Add(item);
					}
				}
				OnNetworkSubscribersLeave(list);
				Pool.FreeList<Connection>(ref list);
			}
			float protectionDuration = Nexus.protectionDuration;
			_transferProtectionRemaining = TimeUntil.op_Implicit(protectionDuration);
			((FacepunchBehaviour)this).Invoke(DisableTransferProtectionAction, protectionDuration);
		}
		foreach (BaseEntity child in children)
		{
			child.EnableTransferProtection();
		}
	}

	public virtual void DisableTransferProtection()
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		if (IsTransferProtected())
		{
			SetFlag(Flags.Protected, b: false);
			List<Connection> subscribers = GetSubscribers();
			if (subscribers != null)
			{
				OnNetworkSubscribersEnter(subscribers);
			}
			_transferProtectionRemaining = TimeUntil.op_Implicit(0f);
			((FacepunchBehaviour)this).CancelInvoke(DisableTransferProtectionAction);
		}
		foreach (BaseEntity child in children)
		{
			child.DisableTransferProtection();
		}
	}

	public void SetParent(BaseEntity entity, bool worldPositionStays = false, bool sendImmediate = false)
	{
		SetParent(entity, 0u, worldPositionStays, sendImmediate);
	}

	public void SetParent(BaseEntity entity, string strBone, bool worldPositionStays = false, bool sendImmediate = false)
	{
		SetParent(entity, (!string.IsNullOrEmpty(strBone)) ? StringPool.Get(strBone) : 0u, worldPositionStays, sendImmediate);
	}

	public bool HasChild(BaseEntity c)
	{
		if ((Object)(object)c == (Object)(object)this)
		{
			return true;
		}
		BaseEntity baseEntity = c.GetParentEntity();
		if ((Object)(object)baseEntity != (Object)null)
		{
			return HasChild(baseEntity);
		}
		return false;
	}

	public void SetParent(BaseEntity entity, uint boneID, bool worldPositionStays = false, bool sendImmediate = false)
	{
		if ((Object)(object)entity != (Object)null)
		{
			if ((Object)(object)entity == (Object)(object)this)
			{
				Debug.LogError((object)("Trying to parent to self " + (object)this), (Object)(object)((Component)this).gameObject);
				return;
			}
			if (HasChild(entity))
			{
				Debug.LogError((object)("Trying to parent to child " + (object)this), (Object)(object)((Component)this).gameObject);
				return;
			}
		}
		LogEntry(LogEntryType.Hierarchy, 2, "SetParent {0} {1}", entity, boneID);
		BaseEntity baseEntity = GetParentEntity();
		if (Object.op_Implicit((Object)(object)baseEntity))
		{
			baseEntity.RemoveChild(this);
		}
		if (base.limitNetworking && (Object)(object)baseEntity != (Object)null && (Object)(object)baseEntity != (Object)(object)entity)
		{
			BasePlayer basePlayer = baseEntity as BasePlayer;
			if (basePlayer.IsValid())
			{
				DestroyOnClient(basePlayer.net.connection);
			}
		}
		if ((Object)(object)entity == (Object)null)
		{
			OnParentChanging(baseEntity, null);
			parentEntity.Set(null);
			((Component)this).transform.SetParent((Transform)null, worldPositionStays);
			parentBone = 0u;
			UpdateNetworkGroup();
			if (sendImmediate)
			{
				SendNetworkUpdateImmediate();
				SendChildrenNetworkUpdateImmediate();
			}
			else
			{
				SendNetworkUpdate();
				SendChildrenNetworkUpdate();
			}
			return;
		}
		Debug.Assert(entity.isServer, "SetParent - child should be a SERVER entity");
		Debug.Assert(entity.net != null, "Setting parent to entity that hasn't spawned yet! (net is null)");
		Debug.Assert(((NetworkableId)(ref entity.net.ID)).IsValid, "Setting parent to entity that hasn't spawned yet! (id = 0)");
		entity.AddChild(this);
		OnParentChanging(baseEntity, entity);
		parentEntity.Set(entity);
		if (boneID != 0 && boneID != StringPool.closest)
		{
			((Component)this).transform.SetParent(entity.FindBone(StringPool.Get(boneID)), worldPositionStays);
		}
		else
		{
			((Component)this).transform.SetParent(((Component)entity).transform, worldPositionStays);
		}
		parentBone = boneID;
		UpdateNetworkGroup();
		if (sendImmediate)
		{
			SendNetworkUpdateImmediate();
			SendChildrenNetworkUpdateImmediate();
		}
		else
		{
			SendNetworkUpdate();
			SendChildrenNetworkUpdate();
		}
	}

	private void DestroyOnClient(Connection connection)
	{
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		if (children != null)
		{
			foreach (BaseEntity child in children)
			{
				child.DestroyOnClient(connection);
			}
		}
		if (((BaseNetwork)Net.sv).IsConnected())
		{
			NetWrite obj = ((BaseNetwork)Net.sv).StartWrite();
			obj.PacketID((Type)6);
			obj.EntityID(net.ID);
			obj.UInt8((byte)0);
			obj.Send(new SendInfo(connection));
			LogEntry(LogEntryType.Network, 2, "EntityDestroy");
		}
	}

	private void SendChildrenNetworkUpdate()
	{
		if (children == null)
		{
			return;
		}
		foreach (BaseEntity child in children)
		{
			child.UpdateNetworkGroup();
			child.SendNetworkUpdate();
		}
	}

	private void SendChildrenNetworkUpdateImmediate()
	{
		if (children == null)
		{
			return;
		}
		foreach (BaseEntity child in children)
		{
			child.UpdateNetworkGroup();
			child.SendNetworkUpdateImmediate();
		}
	}

	public virtual void SwitchParent(BaseEntity ent)
	{
		Log("SwitchParent Missed " + (object)ent);
	}

	public virtual void OnParentChanging(BaseEntity oldParent, BaseEntity newParent)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		Rigidbody component = ((Component)this).GetComponent<Rigidbody>();
		if (Object.op_Implicit((Object)(object)component))
		{
			if ((Object)(object)oldParent != (Object)null && (Object)(object)((Component)oldParent).GetComponent<Rigidbody>() == (Object)null)
			{
				component.velocity += oldParent.GetWorldVelocity();
			}
			if ((Object)(object)newParent != (Object)null && (Object)(object)((Component)newParent).GetComponent<Rigidbody>() == (Object)null)
			{
				component.velocity -= newParent.GetWorldVelocity();
			}
		}
	}

	public virtual BuildingPrivlidge GetBuildingPrivilege()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return GetBuildingPrivilege(WorldSpaceBounds());
	}

	public BuildingPrivlidge GetBuildingPrivilege(OBB obb)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		BuildingBlock other = null;
		BuildingPrivlidge result = null;
		List<BuildingBlock> list = Pool.GetList<BuildingBlock>();
		Vis.Entities(obb.position, 16f + ((Vector3)(ref obb.extents)).magnitude, list, 2097152, (QueryTriggerInteraction)2);
		for (int i = 0; i < list.Count; i++)
		{
			BuildingBlock buildingBlock = list[i];
			if (buildingBlock.isServer != base.isServer || !buildingBlock.IsOlderThan(other) || ((OBB)(ref obb)).Distance(buildingBlock.WorldSpaceBounds()) > 16f)
			{
				continue;
			}
			BuildingManager.Building building = buildingBlock.GetBuilding();
			if (building != null)
			{
				BuildingPrivlidge dominatingBuildingPrivilege = building.GetDominatingBuildingPrivilege();
				if (!((Object)(object)dominatingBuildingPrivilege == (Object)null))
				{
					other = buildingBlock;
					result = dominatingBuildingPrivilege;
				}
			}
		}
		Pool.FreeList<BuildingBlock>(ref list);
		return result;
	}

	public void SV_RPCMessage(uint nameID, Message message)
	{
		Assert.IsTrue(base.isServer, "Should be server!");
		BasePlayer basePlayer = message.Player();
		if (!basePlayer.IsValid())
		{
			if (Global.developer > 0)
			{
				Debug.Log((object)("SV_RPCMessage: From invalid player " + (object)basePlayer));
			}
		}
		else if (basePlayer.isStalled)
		{
			if (Global.developer > 0)
			{
				Debug.Log((object)("SV_RPCMessage: player is stalled " + (object)basePlayer));
			}
		}
		else if (!OnRpcMessage(basePlayer, nameID, message))
		{
			for (int i = 0; i < Components.Length && !Components[i].OnRpcMessage(basePlayer, nameID, message); i++)
			{
			}
		}
	}

	public void ClientRPCPlayer<T1, T2, T3, T4, T5>(Connection sourceConnection, BasePlayer player, string funcName, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		if (((BaseNetwork)Net.sv).IsConnected() && net != null && player.net.connection != null)
		{
			ClientRPCEx(new SendInfo(player.net.connection), sourceConnection, funcName, arg1, arg2, arg3, arg4, arg5);
		}
	}

	public void ClientRPCPlayer<T1, T2, T3, T4>(Connection sourceConnection, BasePlayer player, string funcName, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		if (((BaseNetwork)Net.sv).IsConnected() && net != null && player.net.connection != null)
		{
			ClientRPCEx(new SendInfo(player.net.connection), sourceConnection, funcName, arg1, arg2, arg3, arg4);
		}
	}

	public void ClientRPCPlayer<T1, T2, T3>(Connection sourceConnection, BasePlayer player, string funcName, T1 arg1, T2 arg2, T3 arg3)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		if (((BaseNetwork)Net.sv).IsConnected() && net != null && player.net.connection != null)
		{
			ClientRPCEx(new SendInfo(player.net.connection), sourceConnection, funcName, arg1, arg2, arg3);
		}
	}

	public void ClientRPCPlayer<T1, T2>(Connection sourceConnection, BasePlayer player, string funcName, T1 arg1, T2 arg2)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		if (((BaseNetwork)Net.sv).IsConnected() && net != null && player.net.connection != null)
		{
			ClientRPCEx(new SendInfo(player.net.connection), sourceConnection, funcName, arg1, arg2);
		}
	}

	public void ClientRPCPlayer<T1>(Connection sourceConnection, BasePlayer player, string funcName, T1 arg1)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		if (((BaseNetwork)Net.sv).IsConnected() && net != null && player.net.connection != null)
		{
			ClientRPCEx(new SendInfo(player.net.connection), sourceConnection, funcName, arg1);
		}
	}

	public void ClientRPCPlayer(Connection sourceConnection, BasePlayer player, string funcName)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		if (((BaseNetwork)Net.sv).IsConnected() && net != null && player.net.connection != null)
		{
			ClientRPCEx(new SendInfo(player.net.connection), sourceConnection, funcName);
		}
	}

	public void ClientRPC<T1, T2, T3, T4, T5>(Connection sourceConnection, string funcName, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		if (((BaseNetwork)Net.sv).IsConnected() && net != null && net.group != null)
		{
			ClientRPCEx(new SendInfo(net.group.subscribers), sourceConnection, funcName, arg1, arg2, arg3, arg4, arg5);
		}
	}

	public void ClientRPC<T1, T2, T3, T4>(Connection sourceConnection, string funcName, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		if (((BaseNetwork)Net.sv).IsConnected() && net != null && net.group != null)
		{
			ClientRPCEx(new SendInfo(net.group.subscribers), sourceConnection, funcName, arg1, arg2, arg3, arg4);
		}
	}

	public void ClientRPC<T1, T2, T3>(Connection sourceConnection, string funcName, T1 arg1, T2 arg2, T3 arg3)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		if (((BaseNetwork)Net.sv).IsConnected() && net != null && net.group != null)
		{
			ClientRPCEx(new SendInfo(net.group.subscribers), sourceConnection, funcName, arg1, arg2, arg3);
		}
	}

	public void ClientRPC<T1, T2>(Connection sourceConnection, string funcName, T1 arg1, T2 arg2)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		if (((BaseNetwork)Net.sv).IsConnected() && net != null && net.group != null)
		{
			ClientRPCEx(new SendInfo(net.group.subscribers), sourceConnection, funcName, arg1, arg2);
		}
	}

	public void ClientRPC<T1>(Connection sourceConnection, string funcName, T1 arg1)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		if (((BaseNetwork)Net.sv).IsConnected() && net != null && net.group != null)
		{
			ClientRPCEx(new SendInfo(net.group.subscribers), sourceConnection, funcName, arg1);
		}
	}

	public void ClientRPC(Connection sourceConnection, string funcName)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		if (((BaseNetwork)Net.sv).IsConnected() && net != null && net.group != null)
		{
			ClientRPCEx(new SendInfo(net.group.subscribers), sourceConnection, funcName);
		}
	}

	public void ClientRPCEx<T1, T2, T3, T4, T5>(SendInfo sendInfo, Connection sourceConnection, string funcName, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		if (((BaseNetwork)Net.sv).IsConnected() && net != null)
		{
			NetWrite write = ClientRPCStart(sourceConnection, funcName);
			ClientRPCWrite(write, arg1);
			ClientRPCWrite(write, arg2);
			ClientRPCWrite(write, arg3);
			ClientRPCWrite(write, arg4);
			ClientRPCWrite(write, arg5);
			ClientRPCSend(write, sendInfo);
		}
	}

	public void ClientRPCEx<T1, T2, T3, T4>(SendInfo sendInfo, Connection sourceConnection, string funcName, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		if (((BaseNetwork)Net.sv).IsConnected() && net != null)
		{
			NetWrite write = ClientRPCStart(sourceConnection, funcName);
			ClientRPCWrite(write, arg1);
			ClientRPCWrite(write, arg2);
			ClientRPCWrite(write, arg3);
			ClientRPCWrite(write, arg4);
			ClientRPCSend(write, sendInfo);
		}
	}

	public void ClientRPCEx<T1, T2, T3>(SendInfo sendInfo, Connection sourceConnection, string funcName, T1 arg1, T2 arg2, T3 arg3)
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		if (((BaseNetwork)Net.sv).IsConnected() && net != null)
		{
			NetWrite write = ClientRPCStart(sourceConnection, funcName);
			ClientRPCWrite(write, arg1);
			ClientRPCWrite(write, arg2);
			ClientRPCWrite(write, arg3);
			ClientRPCSend(write, sendInfo);
		}
	}

	public void ClientRPCEx<T1, T2>(SendInfo sendInfo, Connection sourceConnection, string funcName, T1 arg1, T2 arg2)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		if (((BaseNetwork)Net.sv).IsConnected() && net != null)
		{
			NetWrite write = ClientRPCStart(sourceConnection, funcName);
			ClientRPCWrite(write, arg1);
			ClientRPCWrite(write, arg2);
			ClientRPCSend(write, sendInfo);
		}
	}

	public void ClientRPCEx<T1>(SendInfo sendInfo, Connection sourceConnection, string funcName, T1 arg1)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		if (((BaseNetwork)Net.sv).IsConnected() && net != null)
		{
			NetWrite write = ClientRPCStart(sourceConnection, funcName);
			ClientRPCWrite(write, arg1);
			ClientRPCSend(write, sendInfo);
		}
	}

	public void ClientRPCEx(SendInfo sendInfo, Connection sourceConnection, string funcName)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		if (((BaseNetwork)Net.sv).IsConnected() && net != null)
		{
			NetWrite write = ClientRPCStart(sourceConnection, funcName);
			ClientRPCSend(write, sendInfo);
		}
	}

	public void ClientRPCPlayerAndSpectators(Connection sourceConnection, BasePlayer player, string funcName)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		if (!((BaseNetwork)Net.sv).IsConnected() || player.net == null || player.net.connection == null)
		{
			return;
		}
		ClientRPCEx(new SendInfo(player.net.connection), sourceConnection, funcName);
		if (!player.IsBeingSpectated || player.children == null)
		{
			return;
		}
		foreach (BaseEntity child in player.children)
		{
			if (child is BasePlayer player2)
			{
				ClientRPCPlayer(sourceConnection, player2, funcName);
			}
		}
	}

	public void ClientRPCPlayerAndSpectators<T1>(Connection sourceConnection, BasePlayer player, string funcName, T1 arg1)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		if (!((BaseNetwork)Net.sv).IsConnected() || player.net == null || player.net.connection == null)
		{
			return;
		}
		ClientRPCEx(new SendInfo(player.net.connection), sourceConnection, funcName, arg1);
		if (!player.IsBeingSpectated || player.children == null)
		{
			return;
		}
		foreach (BaseEntity child in player.children)
		{
			if (child is BasePlayer player2)
			{
				ClientRPCPlayer(sourceConnection, player2, funcName, arg1);
			}
		}
	}

	public void ClientRPCPlayerAndSpectators<T1, T2>(Connection sourceConnection, BasePlayer player, string funcName, T1 arg1, T2 arg2)
	{
		if (!((BaseNetwork)Net.sv).IsConnected() || player.net == null || player.net.connection == null)
		{
			return;
		}
		ClientRPCPlayer(sourceConnection, player, funcName, arg1, arg2);
		if (!player.IsBeingSpectated || player.children == null)
		{
			return;
		}
		foreach (BaseEntity child in children)
		{
			if (child is BasePlayer player2)
			{
				ClientRPCPlayer(sourceConnection, player2, funcName, arg1, arg2);
			}
		}
	}

	public void ClientRPCPlayerAndSpectators<T1, T2, T3>(Connection sourceConnection, BasePlayer player, string funcName, T1 arg1, T2 arg2, T3 arg3)
	{
		if (!((BaseNetwork)Net.sv).IsConnected() || player.net == null || player.net.connection == null)
		{
			return;
		}
		ClientRPCPlayer(sourceConnection, player, funcName, arg1, arg2, arg3);
		if (!player.IsBeingSpectated || player.children == null)
		{
			return;
		}
		foreach (BaseEntity child in player.children)
		{
			if (child is BasePlayer player2)
			{
				ClientRPCPlayer(sourceConnection, player2, funcName, arg1, arg2, arg3);
			}
		}
	}

	protected NetWrite ClientRPCStart(Connection sourceConnection, string funcName)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		NetWrite obj = ((BaseNetwork)Net.sv).StartWrite();
		obj.PacketID((Type)9);
		obj.EntityID(net.ID);
		obj.UInt32(StringPool.Get(funcName));
		obj.UInt64(sourceConnection?.userid ?? 0);
		return obj;
	}

	private void ClientRPCWrite<T>(NetWrite write, T arg)
	{
		write.WriteObject(arg);
	}

	protected void ClientRPCSend(NetWrite write, SendInfo sendInfo)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		write.Send(sendInfo);
	}

	public void ClientRPCPlayerList<T1>(Connection sourceConnection, BasePlayer player, string funcName, List<T1> list)
	{
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		if (!((BaseNetwork)Net.sv).IsConnected() || net == null || player.net.connection == null)
		{
			return;
		}
		NetWrite write = ClientRPCStart(sourceConnection, funcName);
		ClientRPCWrite(write, list.Count);
		foreach (T1 item in list)
		{
			ClientRPCWrite(write, item);
		}
		SendInfo sendInfo = default(SendInfo);
		((SendInfo)(ref sendInfo))._002Ector(player.net.connection);
		sendInfo.priority = (Priority)0;
		ClientRPCSend(write, sendInfo);
	}

	public override void Save(SaveInfo info)
	{
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0317: Unknown result type (might be due to invalid IL or missing references)
		//IL_031c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0409: Unknown result type (might be due to invalid IL or missing references)
		base.Save(info);
		BaseEntity baseEntity = parentEntity.Get(base.isServer);
		info.msg.baseEntity = Pool.Get<BaseEntity>();
		Quaternion val;
		if (info.forDisk)
		{
			if (this is BasePlayer)
			{
				if ((Object)(object)baseEntity == (Object)null || baseEntity.enableSaving)
				{
					info.msg.baseEntity.pos = ((Component)this).transform.localPosition;
					BaseEntity baseEntity2 = info.msg.baseEntity;
					val = ((Component)this).transform.localRotation;
					baseEntity2.rot = ((Quaternion)(ref val)).eulerAngles;
				}
				else
				{
					info.msg.baseEntity.pos = ((Component)this).transform.position;
					BaseEntity baseEntity3 = info.msg.baseEntity;
					val = ((Component)this).transform.rotation;
					baseEntity3.rot = ((Quaternion)(ref val)).eulerAngles;
				}
			}
			else
			{
				info.msg.baseEntity.pos = ((Component)this).transform.localPosition;
				BaseEntity baseEntity4 = info.msg.baseEntity;
				val = ((Component)this).transform.localRotation;
				baseEntity4.rot = ((Quaternion)(ref val)).eulerAngles;
			}
		}
		else
		{
			info.msg.baseEntity.pos = GetNetworkPosition();
			BaseEntity baseEntity5 = info.msg.baseEntity;
			val = GetNetworkRotation();
			baseEntity5.rot = ((Quaternion)(ref val)).eulerAngles;
			info.msg.baseEntity.time = GetNetworkTime();
		}
		info.msg.baseEntity.flags = (int)flags;
		info.msg.baseEntity.skinid = skinID;
		if (info.forDisk && this is BasePlayer)
		{
			if ((Object)(object)baseEntity != (Object)null && baseEntity.enableSaving)
			{
				info.msg.parent = Pool.Get<ParentInfo>();
				info.msg.parent.uid = parentEntity.uid;
				info.msg.parent.bone = parentBone;
			}
		}
		else if ((Object)(object)baseEntity != (Object)null)
		{
			info.msg.parent = Pool.Get<ParentInfo>();
			info.msg.parent.uid = parentEntity.uid;
			info.msg.parent.bone = parentBone;
		}
		if (HasAnySlot())
		{
			info.msg.entitySlots = Pool.Get<EntitySlots>();
			info.msg.entitySlots.slotLock = entitySlots[0].uid;
			info.msg.entitySlots.slotFireMod = entitySlots[1].uid;
			info.msg.entitySlots.slotUpperModification = entitySlots[2].uid;
			info.msg.entitySlots.centerDecoration = entitySlots[5].uid;
			info.msg.entitySlots.lowerCenterDecoration = entitySlots[6].uid;
			info.msg.entitySlots.storageMonitor = entitySlots[7].uid;
		}
		if (info.forDisk && Object.op_Implicit((Object)(object)_spawnable))
		{
			_spawnable.Save(info);
		}
		if (OwnerID != 0L && (info.forDisk || ShouldNetworkOwnerInfo()))
		{
			info.msg.ownerInfo = Pool.Get<OwnerInfo>();
			info.msg.ownerInfo.steamid = OwnerID;
		}
		if (Components != null)
		{
			for (int i = 0; i < Components.Length; i++)
			{
				if (!((Object)(object)Components[i] == (Object)null))
				{
					Components[i].SaveComponent(info);
				}
			}
		}
		if (info.forTransfer && ShouldTransferAssociatedFiles)
		{
			info.msg.associatedFiles = Pool.Get<AssociatedFiles>();
			info.msg.associatedFiles.files = Pool.GetList<AssociatedFile>();
			info.msg.associatedFiles.files.AddRange(FileStorage.server.QueryAllByEntity(net.ID));
		}
	}

	public virtual bool ShouldNetworkOwnerInfo()
	{
		return false;
	}

	public override void Load(LoadInfo info)
	{
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0398: Unknown result type (might be due to invalid IL or missing references)
		//IL_039d: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		base.Load(info);
		if (info.msg.baseEntity != null)
		{
			BaseEntity baseEntity = info.msg.baseEntity;
			Flags old = flags;
			if (base.isServer)
			{
				baseEntity.flags &= -33554433;
			}
			flags = (Flags)baseEntity.flags;
			OnFlagsChanged(old, flags);
			OnSkinChanged(skinID, info.msg.baseEntity.skinid);
			if (info.fromDisk)
			{
				if (Vector3Ex.IsNaNOrInfinity(baseEntity.pos))
				{
					string text = ((object)this).ToString();
					Vector3 pos = baseEntity.pos;
					Debug.LogWarning((object)(text + " has broken position - " + ((object)(Vector3)(ref pos)).ToString()));
					baseEntity.pos = Vector3.zero;
				}
				((Component)this).transform.localPosition = baseEntity.pos;
				((Component)this).transform.localRotation = Quaternion.Euler(baseEntity.rot);
			}
		}
		if (info.msg.entitySlots != null)
		{
			entitySlots[0].uid = info.msg.entitySlots.slotLock;
			entitySlots[1].uid = info.msg.entitySlots.slotFireMod;
			entitySlots[2].uid = info.msg.entitySlots.slotUpperModification;
			entitySlots[5].uid = info.msg.entitySlots.centerDecoration;
			entitySlots[6].uid = info.msg.entitySlots.lowerCenterDecoration;
			entitySlots[7].uid = info.msg.entitySlots.storageMonitor;
		}
		if (info.msg.parent != null)
		{
			if (base.isServer)
			{
				BaseEntity entity = BaseNetworkable.serverEntities.Find(info.msg.parent.uid) as BaseEntity;
				SetParent(entity, info.msg.parent.bone);
			}
			parentEntity.uid = info.msg.parent.uid;
			parentBone = info.msg.parent.bone;
		}
		else
		{
			parentEntity.uid = default(NetworkableId);
			parentBone = 0u;
		}
		if (info.msg.ownerInfo != null)
		{
			OwnerID = info.msg.ownerInfo.steamid;
		}
		if (Object.op_Implicit((Object)(object)_spawnable))
		{
			_spawnable.Load(info);
		}
		if (info.fromTransfer && ShouldTransferAssociatedFiles && info.msg.associatedFiles != null && info.msg.associatedFiles.files != null)
		{
			foreach (AssociatedFile file in info.msg.associatedFiles.files)
			{
				if (FileStorage.server.Store(file.data, (FileStorage.Type)file.type, net.ID, file.numID) != file.crc)
				{
					Debug.LogWarning((object)"Associated file has a different CRC after transfer!");
				}
			}
		}
		if (info.fromDisk && info.msg.baseEntity != null && IsTransferProtected())
		{
			float num = ((info.msg.baseEntity.protection > 0f) ? info.msg.baseEntity.protection : Nexus.protectionDuration);
			_transferProtectionRemaining = TimeUntil.op_Implicit(num);
			((FacepunchBehaviour)this).Invoke(DisableTransferProtectionAction, num);
		}
		if (Components == null)
		{
			return;
		}
		for (int i = 0; i < Components.Length; i++)
		{
			if (!((Object)(object)Components[i] == (Object)null))
			{
				Components[i].LoadComponent(info);
			}
		}
	}

	public virtual void SetCreatorEntity(BaseEntity newCreatorEntity)
	{
		creatorEntity = newCreatorEntity;
	}

	public virtual Vector3 GetLocalVelocityServer()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		return Vector3.zero;
	}

	public virtual Quaternion GetAngularVelocityServer()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		return Quaternion.identity;
	}

	public void EnableGlobalBroadcast(bool wants)
	{
		if (globalBroadcast != wants)
		{
			globalBroadcast = wants;
			UpdateNetworkGroup();
		}
	}

	public void EnableSaving(bool wants)
	{
		if (enableSaving == wants)
		{
			return;
		}
		enableSaving = wants;
		if (enableSaving)
		{
			if (!saveList.Contains(this))
			{
				saveList.Add(this);
			}
		}
		else
		{
			saveList.Remove(this);
		}
	}

	public override void ServerInit()
	{
		_spawnable = ((Component)this).GetComponent<Spawnable>();
		base.ServerInit();
		if (!base.isServer)
		{
			return;
		}
		if (enableSaving && !saveList.Contains(this))
		{
			saveList.Add(this);
		}
		if (flags != 0)
		{
			OnFlagsChanged((Flags)0, flags);
		}
		if (syncPosition && PositionTickRate >= 0f)
		{
			if (PositionTickFixedTime)
			{
				((FacepunchBehaviour)this).InvokeRepeatingFixedTime((Action)NetworkPositionTick);
			}
			else
			{
				((FacepunchBehaviour)this).InvokeRandomized((Action)NetworkPositionTick, PositionTickRate, PositionTickRate - PositionTickRate * 0.05f, PositionTickRate * 0.05f);
			}
		}
		Query.Server.Add(this);
	}

	public virtual void OnSensation(Sensation sensation)
	{
	}

	protected virtual bool TransformHasMoved()
	{
		return ((Component)this).transform.hasChanged;
	}

	protected void NetworkPositionTick()
	{
		if (!TransformHasMoved())
		{
			if (ticksSinceStopped >= 6)
			{
				return;
			}
			ticksSinceStopped++;
		}
		else
		{
			ticksSinceStopped = 0;
		}
		TransformChanged();
		((Component)this).transform.hasChanged = false;
	}

	private void TransformChanged()
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		if (Query.Server != null)
		{
			Query.Server.Move(this);
		}
		if (net == null)
		{
			return;
		}
		InvalidateNetworkCache();
		if (!globalBroadcast && !ValidBounds.Test(((Component)this).transform.position))
		{
			OnInvalidPosition();
		}
		else if (syncPosition)
		{
			if (!isCallingUpdateNetworkGroup)
			{
				((FacepunchBehaviour)this).Invoke((Action)UpdateNetworkGroup, 5f);
				isCallingUpdateNetworkGroup = true;
			}
			SendNetworkUpdate_Position();
			OnPositionalNetworkUpdate();
		}
	}

	public virtual void OnPositionalNetworkUpdate()
	{
	}

	public void DoMovingWithoutARigidBodyCheck()
	{
		if (doneMovingWithoutARigidBodyCheck <= 10)
		{
			doneMovingWithoutARigidBodyCheck++;
			if (doneMovingWithoutARigidBodyCheck >= 10 && !((Object)(object)((Component)this).GetComponent<Collider>() == (Object)null) && (Object)(object)((Component)this).GetComponent<Rigidbody>() == (Object)null)
			{
				Debug.LogWarning((object)("Entity moving without a rigid body! (" + ((object)((Component)this).gameObject)?.ToString() + ")"), (Object)(object)this);
			}
		}
	}

	public override void Spawn()
	{
		base.Spawn();
		if (base.isServer)
		{
			((Component)this).gameObject.BroadcastOnParentSpawning();
		}
	}

	public void OnParentSpawning()
	{
		if (net != null || base.IsDestroyed)
		{
			return;
		}
		if (Application.isLoadingSave)
		{
			Object.Destroy((Object)(object)((Component)this).gameObject);
			return;
		}
		if (GameManager.server.preProcessed.NeedsProcessing(((Component)this).gameObject))
		{
			GameManager.server.preProcessed.ProcessObject(null, ((Component)this).gameObject, resetLocalTransform: false);
		}
		BaseEntity baseEntity = (((Object)(object)((Component)this).transform.parent != (Object)null) ? ((Component)((Component)this).transform.parent).GetComponentInParent<BaseEntity>() : null);
		Spawn();
		if ((Object)(object)baseEntity != (Object)null)
		{
			SetParent(baseEntity, worldPositionStays: true);
		}
	}

	public void SpawnAsMapEntity()
	{
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		if (net == null && !base.IsDestroyed && (Object)(object)(((Object)(object)((Component)this).transform.parent != (Object)null) ? ((Component)((Component)this).transform.parent).GetComponentInParent<BaseEntity>() : null) == (Object)null)
		{
			if (GameManager.server.preProcessed.NeedsProcessing(((Component)this).gameObject))
			{
				GameManager.server.preProcessed.ProcessObject(null, ((Component)this).gameObject, resetLocalTransform: false);
			}
			((Component)this).transform.parent = null;
			SceneManager.MoveGameObjectToScene(((Component)this).gameObject, Rust.Server.EntityScene);
			((Component)this).gameObject.SetActive(true);
			Spawn();
		}
	}

	public virtual void PostMapEntitySpawn()
	{
	}

	internal override void DoServerDestroy()
	{
		((FacepunchBehaviour)this).CancelInvoke((Action)NetworkPositionTick);
		saveList.Remove(this);
		RemoveFromTriggers();
		if (children != null)
		{
			BaseEntity[] array = children.ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].OnParentRemoved();
			}
		}
		SetParent(null, worldPositionStays: true);
		Query.Server.Remove(this);
		base.DoServerDestroy();
	}

	internal virtual void OnParentRemoved()
	{
		Kill();
	}

	public virtual void OnInvalidPosition()
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		string[] obj = new string[5]
		{
			"Invalid Position: ",
			((object)this)?.ToString(),
			" ",
			null,
			null
		};
		Vector3 position = ((Component)this).transform.position;
		obj[3] = ((object)(Vector3)(ref position)).ToString();
		obj[4] = " (destroying)";
		Debug.Log((object)string.Concat(obj));
		Kill();
	}

	public BaseCorpse DropCorpse(string strCorpsePrefab)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		Assert.IsTrue(base.isServer, "DropCorpse called on client!");
		if (!ConVar.Server.corpses)
		{
			return null;
		}
		if (string.IsNullOrEmpty(strCorpsePrefab))
		{
			return null;
		}
		BaseCorpse baseCorpse = GameManager.server.CreateEntity(strCorpsePrefab) as BaseCorpse;
		if ((Object)(object)baseCorpse == (Object)null)
		{
			Debug.LogWarning((object)("Error creating corpse: " + ((object)((Component)this).gameObject)?.ToString() + " - " + strCorpsePrefab));
			return null;
		}
		baseCorpse.InitCorpse(this);
		return baseCorpse;
	}

	public override void UpdateNetworkGroup()
	{
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		Assert.IsTrue(base.isServer, "UpdateNetworkGroup called on clientside entity!");
		isCallingUpdateNetworkGroup = false;
		if (net == null || Net.sv == null || Net.sv.visibility == null)
		{
			return;
		}
		TimeWarning val = TimeWarning.New("UpdateNetworkGroup", 0);
		try
		{
			if (globalBroadcast)
			{
				if (net.SwitchGroup(BaseNetworkable.GlobalNetworkGroup))
				{
					SendNetworkGroupChange();
				}
			}
			else if (ShouldInheritNetworkGroup() && parentEntity.IsSet())
			{
				BaseEntity baseEntity = GetParentEntity();
				if (!baseEntity.IsValid())
				{
					if (!Application.isLoadingSave)
					{
						NetworkableId uid = parentEntity.uid;
						Debug.LogWarning((object)("UpdateNetworkGroup: Missing parent entity " + ((object)(NetworkableId)(ref uid)).ToString()));
						((FacepunchBehaviour)this).Invoke((Action)UpdateNetworkGroup, 2f);
						isCallingUpdateNetworkGroup = true;
					}
				}
				else if ((Object)(object)baseEntity != (Object)null)
				{
					if (net.SwitchGroup(baseEntity.net.group))
					{
						SendNetworkGroupChange();
					}
				}
				else
				{
					Debug.LogWarning((object)(((object)((Component)this).gameObject)?.ToString() + ": has parent id - but couldn't find parent! " + parentEntity));
				}
			}
			else if (base.limitNetworking)
			{
				if (net.SwitchGroup(BaseNetworkable.LimboNetworkGroup))
				{
					SendNetworkGroupChange();
				}
			}
			else
			{
				base.UpdateNetworkGroup();
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public virtual void Eat(BaseNpc baseNpc, float timeSpent)
	{
		baseNpc.AddCalories(100f);
	}

	public virtual void OnDeployed(BaseEntity parent, BasePlayer deployedBy, Item fromItem)
	{
	}

	public override bool ShouldNetworkTo(BasePlayer player)
	{
		if ((Object)(object)player == (Object)(object)this)
		{
			return true;
		}
		if (IsTransferProtected())
		{
			return false;
		}
		BaseEntity baseEntity = GetParentEntity();
		if (base.limitNetworking)
		{
			if ((Object)(object)baseEntity == (Object)null)
			{
				return false;
			}
			if ((Object)(object)baseEntity != (Object)(object)player)
			{
				return false;
			}
		}
		if ((Object)(object)baseEntity != (Object)null)
		{
			return baseEntity.ShouldNetworkTo(player);
		}
		return base.ShouldNetworkTo(player);
	}

	public virtual void AttackerInfo(DeathInfo info)
	{
		info.attackerName = base.ShortPrefabName;
		info.attackerSteamID = 0uL;
		info.inflictorName = "";
	}

	public virtual void Push(Vector3 velocity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		SetVelocity(velocity);
	}

	public virtual void ApplyInheritedVelocity(Vector3 velocity)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		Rigidbody component = ((Component)this).GetComponent<Rigidbody>();
		if (Object.op_Implicit((Object)(object)component))
		{
			component.velocity = Vector3.Lerp(component.velocity, velocity, 10f * Time.fixedDeltaTime);
			component.angularVelocity *= Mathf.Clamp01(1f - 10f * Time.fixedDeltaTime);
			component.AddForce(-Physics.gravity * Mathf.Clamp01(0.9f), (ForceMode)5);
		}
	}

	public virtual void SetVelocity(Vector3 velocity)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		Rigidbody component = ((Component)this).GetComponent<Rigidbody>();
		if (Object.op_Implicit((Object)(object)component))
		{
			component.velocity = velocity;
		}
	}

	public virtual void SetAngularVelocity(Vector3 velocity)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		Rigidbody component = ((Component)this).GetComponent<Rigidbody>();
		if (Object.op_Implicit((Object)(object)component))
		{
			component.angularVelocity = velocity;
		}
	}

	public virtual Vector3 GetDropPosition()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return ((Component)this).transform.position;
	}

	public virtual Vector3 GetDropVelocity()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		return GetInheritedDropVelocity() + Vector3.up;
	}

	public virtual bool OnStartBeingLooted(BasePlayer baseEntity)
	{
		return true;
	}

	public virtual string Admin_Who()
	{
		return $"Owner ID: {OwnerID}";
	}

	public virtual float RadiationProtection()
	{
		return 0f;
	}

	public virtual float RadiationExposureFraction()
	{
		return 1f;
	}

	[RPC_Server]
	[RPC_Server.FromOwner]
	private void BroadcastSignalFromClient(RPCMessage msg)
	{
		uint num = StringPool.Get("BroadcastSignalFromClient");
		if (num != 0)
		{
			BasePlayer player = msg.player;
			if (!((Object)(object)player == (Object)null) && player.rpcHistory.TryIncrement(num, (ulong)ConVar.Server.maxpacketspersecond_rpc_signal))
			{
				Signal signal = (Signal)msg.read.Int32();
				string arg = msg.read.String(256);
				SignalBroadcast(signal, arg, msg.connection);
			}
		}
	}

	public void SignalBroadcast(Signal signal, string arg, Connection sourceConnection = null)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		if (net != null && net.group != null)
		{
			SendInfo sendInfo = default(SendInfo);
			((SendInfo)(ref sendInfo))._002Ector(net.group.subscribers);
			sendInfo.method = (SendMethod)2;
			sendInfo.priority = (Priority)0;
			ClientRPCEx(sendInfo, sourceConnection, "SignalFromServerEx", (int)signal, arg);
		}
	}

	public void SignalBroadcast(Signal signal, Connection sourceConnection = null)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		if (net != null && net.group != null)
		{
			SendInfo sendInfo = default(SendInfo);
			((SendInfo)(ref sendInfo))._002Ector(net.group.subscribers);
			sendInfo.method = (SendMethod)2;
			sendInfo.priority = (Priority)0;
			ClientRPCEx(sendInfo, sourceConnection, "SignalFromServer", (int)signal);
		}
	}

	protected virtual void OnSkinChanged(ulong oldSkinID, ulong newSkinID)
	{
		if (oldSkinID != newSkinID)
		{
			skinID = newSkinID;
		}
	}

	protected virtual void OnSkinPreProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		if (clientside && Skinnable.All != null && (Object)(object)Skinnable.FindForEntity(name) != (Object)null)
		{
			WorkshopSkin.Prepare(rootObj);
			MaterialReplacement.Prepare(rootObj);
		}
	}

	public virtual void PreProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		OnSkinPreProcess(preProcess, rootObj, name, serverside, clientside, bundling);
	}

	public bool HasAnySlot()
	{
		for (int i = 0; i < entitySlots.Length; i++)
		{
			if (entitySlots[i].IsValid(base.isServer))
			{
				return true;
			}
		}
		return false;
	}

	public BaseEntity GetSlot(Slot slot)
	{
		return entitySlots[(int)slot].Get(base.isServer);
	}

	public string GetSlotAnchorName(Slot slot)
	{
		return slot.ToString().ToLower();
	}

	public void SetSlot(Slot slot, BaseEntity ent)
	{
		entitySlots[(int)slot].Set(ent);
		SendNetworkUpdate();
	}

	public EntityRef[] GetSlots()
	{
		return entitySlots;
	}

	public void SetSlots(EntityRef[] newSlots)
	{
		entitySlots = newSlots;
	}

	public virtual bool HasSlot(Slot slot)
	{
		return false;
	}

	public bool HasTrait(TraitFlag f)
	{
		return (Traits & f) == f;
	}

	public bool HasAnyTrait(TraitFlag f)
	{
		return (Traits & f) != 0;
	}

	public virtual bool EnterTrigger(TriggerBase trigger)
	{
		if (triggers == null)
		{
			triggers = Pool.Get<List<TriggerBase>>();
		}
		triggers.Add(trigger);
		return true;
	}

	public virtual void LeaveTrigger(TriggerBase trigger)
	{
		if (triggers != null)
		{
			triggers.Remove(trigger);
			if (triggers.Count == 0)
			{
				Pool.FreeList<TriggerBase>(ref triggers);
			}
		}
	}

	public void RemoveFromTriggers()
	{
		if (triggers == null)
		{
			return;
		}
		TimeWarning val = TimeWarning.New("RemoveFromTriggers", 0);
		try
		{
			TriggerBase[] array = triggers.ToArray();
			foreach (TriggerBase triggerBase in array)
			{
				if (Object.op_Implicit((Object)(object)triggerBase))
				{
					triggerBase.RemoveEntity(this);
				}
			}
			if (triggers != null && triggers.Count == 0)
			{
				Pool.FreeList<TriggerBase>(ref triggers);
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public T FindTrigger<T>() where T : TriggerBase
	{
		if (triggers == null)
		{
			return null;
		}
		foreach (TriggerBase trigger in triggers)
		{
			if (!((Object)(object)(trigger as T) == (Object)null))
			{
				return trigger as T;
			}
		}
		return null;
	}

	public bool FindTrigger<T>(out T result) where T : TriggerBase
	{
		result = FindTrigger<T>();
		return (Object)(object)result != (Object)null;
	}

	private void ForceUpdateTriggersAction()
	{
		if (!base.IsDestroyed)
		{
			ForceUpdateTriggers(enter: false, exit: true, invoke: false);
		}
	}

	public void ForceUpdateTriggers(bool enter = true, bool exit = true, bool invoke = true)
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		List<TriggerBase> list = Pool.GetList<TriggerBase>();
		List<TriggerBase> list2 = Pool.GetList<TriggerBase>();
		if (triggers != null)
		{
			list.AddRange(triggers);
		}
		Collider componentInChildren = ((Component)this).GetComponentInChildren<Collider>();
		if (componentInChildren is CapsuleCollider)
		{
			CapsuleCollider val = (CapsuleCollider)(object)((componentInChildren is CapsuleCollider) ? componentInChildren : null);
			Vector3 point = ((Component)this).transform.position + new Vector3(0f, val.radius, 0f);
			Vector3 point2 = ((Component)this).transform.position + new Vector3(0f, val.height - val.radius, 0f);
			GamePhysics.OverlapCapsule<TriggerBase>(point, point2, val.radius, list2, 262144, (QueryTriggerInteraction)2);
		}
		else if (componentInChildren is BoxCollider)
		{
			BoxCollider val2 = (BoxCollider)(object)((componentInChildren is BoxCollider) ? componentInChildren : null);
			GamePhysics.OverlapOBB<TriggerBase>(new OBB(((Component)this).transform.position, ((Component)this).transform.lossyScale, ((Component)this).transform.rotation, new Bounds(val2.center, val2.size)), list2, 262144, (QueryTriggerInteraction)2);
		}
		else if (componentInChildren is SphereCollider)
		{
			SphereCollider val3 = (SphereCollider)(object)((componentInChildren is SphereCollider) ? componentInChildren : null);
			GamePhysics.OverlapSphere<TriggerBase>(((Component)this).transform.TransformPoint(val3.center), val3.radius, list2, 262144, (QueryTriggerInteraction)2);
		}
		else
		{
			list2.AddRange(list);
		}
		if (exit)
		{
			foreach (TriggerBase item in list)
			{
				if (!list2.Contains(item))
				{
					item.OnTriggerExit(componentInChildren);
				}
			}
		}
		if (enter)
		{
			foreach (TriggerBase item2 in list2)
			{
				if (!list.Contains(item2))
				{
					item2.OnTriggerEnter(componentInChildren);
				}
			}
		}
		Pool.FreeList<TriggerBase>(ref list);
		Pool.FreeList<TriggerBase>(ref list2);
		if (invoke)
		{
			((FacepunchBehaviour)this).Invoke((Action)ForceUpdateTriggersAction, Time.time - Time.fixedTime + Time.fixedDeltaTime * 1.5f);
		}
	}

	public virtual bool InSafeZone()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		BaseGameMode activeGameMode = BaseGameMode.GetActiveGameMode(serverside: true);
		if ((Object)(object)activeGameMode != (Object)null && !activeGameMode.safeZone)
		{
			return false;
		}
		float num = 0f;
		Vector3 position = ((Component)this).transform.position;
		if (triggers != null)
		{
			for (int i = 0; i < triggers.Count; i++)
			{
				TriggerSafeZone triggerSafeZone = triggers[i] as TriggerSafeZone;
				if (!((Object)(object)triggerSafeZone == (Object)null))
				{
					float safeLevel = triggerSafeZone.GetSafeLevel(position);
					if (safeLevel > num)
					{
						num = safeLevel;
					}
				}
			}
		}
		return num > 0f;
	}

	public TriggerParent FindSuitableParent()
	{
		if (triggers == null)
		{
			return null;
		}
		foreach (TriggerBase trigger in triggers)
		{
			if (trigger is TriggerParent triggerParent && triggerParent.ShouldParent(this, bypassOtherTriggerCheck: true))
			{
				return triggerParent;
			}
		}
		return null;
	}
}
