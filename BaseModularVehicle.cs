using System;
using System.Collections.Generic;
using Facepunch;
using ProtoBuf;
using Rust.Modular;
using UnityEngine;

public abstract class BaseModularVehicle : GroundVehicle, PlayerInventory.ICanMoveFrom, IPrefabPreProcess
{
	internal bool inEditableLocation;

	private bool prevEditable;

	internal bool immuneToDecay;

	protected Vector3 realLocalCOM;

	public Item AssociatedItemInstance;

	private bool disablePhysics;

	[Header("Modular Vehicle")]
	[SerializeField]
	private List<ModularVehicleSocket> moduleSockets;

	[SerializeField]
	private Transform centreOfMassTransform;

	[SerializeField]
	protected Transform waterSample;

	[SerializeField]
	private LODGroup lodGroup;

	public GameObjectRef keyEnterDialog;

	private float _mass = -1f;

	public const Flags FLAG_KINEMATIC = Flags.Reserved6;

	private Dictionary<BaseVehicleModule, Action> moduleAddActions = new Dictionary<BaseVehicleModule, Action>();

	public ModularVehicleInventory Inventory { get; private set; }

	public Vector3 CentreOfMass => centreOfMassTransform.localPosition;

	public int NumAttachedModules => AttachedModuleEntities.Count;

	public bool HasAnyModules => AttachedModuleEntities.Count > 0;

	public List<BaseVehicleModule> AttachedModuleEntities { get; } = new List<BaseVehicleModule>();


	public int TotalSockets => moduleSockets.Count;

	public int NumFreeSockets
	{
		get
		{
			int num = 0;
			for (int i = 0; i < NumAttachedModules; i++)
			{
				num += AttachedModuleEntities[i].GetNumSocketsTaken();
			}
			return TotalSockets - num;
		}
	}

	private float Mass
	{
		get
		{
			if (base.isServer)
			{
				return rigidBody.mass;
			}
			return _mass;
		}
	}

	public float TotalMass { get; private set; }

	public bool IsKinematic => HasFlag(Flags.Reserved6);

	public virtual bool IsLockable => false;

	public bool HasInited { get; private set; }

	private ItemDefinition AssociatedItemDef => repair.itemTarget;

	public bool IsEditableNow
	{
		get
		{
			if (base.isServer)
			{
				return inEditableLocation && CouldBeEdited();
			}
			return false;
		}
	}

	public override void ServerInit()
	{
		base.ServerInit();
		if (!disablePhysics)
		{
			rigidBody.isKinematic = false;
		}
		prevEditable = IsEditableNow;
		if (Inventory == null)
		{
			Inventory = new ModularVehicleInventory(this, AssociatedItemDef, giveUID: true);
		}
	}

	public override void PreServerLoad()
	{
		base.PreServerLoad();
		if (Inventory == null)
		{
			Inventory = new ModularVehicleInventory(this, AssociatedItemDef, giveUID: false);
		}
	}

	public override void PostServerLoad()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		base.PostServerLoad();
		if (Inventory != null)
		{
			ItemContainerId uID = Inventory.UID;
			if (!((ItemContainerId)(ref uID)).IsValid)
			{
				Inventory.GiveUIDs();
			}
		}
		SetFlag(Flags.Open, b: false);
	}

	internal override void DoServerDestroy()
	{
		base.DoServerDestroy();
		if (Inventory != null)
		{
			Inventory.Dispose();
		}
	}

	public override float MaxVelocity()
	{
		float num = GetMaxForwardSpeed() * 1.3f;
		return Mathf.Max(num, 30f);
	}

	public abstract bool IsComplete();

	public bool CouldBeEdited()
	{
		return !AnyMounted() && !IsDead();
	}

	public void DisablePhysics()
	{
		disablePhysics = true;
		rigidBody.isKinematic = true;
	}

	public void EnablePhysics()
	{
		disablePhysics = false;
		rigidBody.isKinematic = false;
	}

	public override void VehicleFixedUpdate()
	{
		base.VehicleFixedUpdate();
		if (IsEditableNow != prevEditable)
		{
			SendNetworkUpdate();
			prevEditable = IsEditableNow;
		}
		SetFlag(Flags.Reserved6, rigidBody.isKinematic);
	}

	public override bool MountEligable(BasePlayer player)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		if (!base.MountEligable(player))
		{
			return false;
		}
		if (IsDead())
		{
			return false;
		}
		if (HasDriver())
		{
			Vector3 velocity = base.Velocity;
			if (((Vector3)(ref velocity)).magnitude >= 2f)
			{
				return false;
			}
		}
		return true;
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		info.msg.modularVehicle = Pool.Get<ModularVehicle>();
		info.msg.modularVehicle.editable = IsEditableNow;
	}

	public bool CanMoveFrom(BasePlayer player, Item item)
	{
		BaseVehicleModule moduleForItem = GetModuleForItem(item);
		if ((Object)(object)moduleForItem != (Object)null)
		{
			return moduleForItem.CanBeMovedNow();
		}
		return true;
	}

	protected abstract Vector3 GetCOMMultiplier();

	public abstract void ModuleHurt(BaseVehicleModule hurtModule, HitInfo info);

	public abstract void ModuleReachedZeroHealth();

	public bool TryAddModule(Item moduleItem, int socketIndex)
	{
		if (!ModuleCanBeAdded(moduleItem, socketIndex, out var failureReason))
		{
			Debug.LogError((object)(((object)this).GetType().Name + ": Can't add module: " + failureReason));
			return false;
		}
		bool flag = Inventory.TryAddModuleItem(moduleItem, socketIndex);
		if (!flag)
		{
			Debug.LogError((object)(((object)this).GetType().Name + ": Couldn't add new item!"));
		}
		return flag;
	}

	public bool TryAddModule(Item moduleItem)
	{
		ItemModVehicleModule component = ((Component)moduleItem.info).GetComponent<ItemModVehicleModule>();
		if ((Object)(object)component == (Object)null)
		{
			return false;
		}
		int socketsTaken = component.socketsTaken;
		int num = Inventory.TryGetFreeSocket(socketsTaken);
		if (num < 0)
		{
			return false;
		}
		return TryAddModule(moduleItem, num);
	}

	public bool ModuleCanBeAdded(Item moduleItem, int socketIndex, out string failureReason)
	{
		if (!base.isServer)
		{
			failureReason = "Can only add modules on server";
			return false;
		}
		if (moduleItem == null)
		{
			failureReason = "Module item is null";
			return false;
		}
		if (moduleItem.info.category != ItemCategory.Component)
		{
			failureReason = "Not a component type item";
			return false;
		}
		ItemModVehicleModule component = ((Component)moduleItem.info).GetComponent<ItemModVehicleModule>();
		if ((Object)(object)component == (Object)null)
		{
			failureReason = "Not the right item module type";
			return false;
		}
		int socketsTaken = component.socketsTaken;
		if (socketIndex < 0)
		{
			socketIndex = Inventory.TryGetFreeSocket(socketsTaken);
		}
		if (!Inventory.SocketsAreFree(socketIndex, socketsTaken, moduleItem))
		{
			failureReason = "One or more desired sockets already in use";
			return false;
		}
		failureReason = string.Empty;
		return true;
	}

	public BaseVehicleModule CreatePhysicalModuleEntity(Item moduleItem, ItemModVehicleModule itemModModule, int socketIndex)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		Vector3 worldPosition = moduleSockets[socketIndex].WorldPosition;
		Quaternion worldRotation = moduleSockets[socketIndex].WorldRotation;
		BaseVehicleModule baseVehicleModule = itemModModule.CreateModuleEntity(this, worldPosition, worldRotation);
		baseVehicleModule.AssociatedItemInstance = moduleItem;
		SetUpModule(baseVehicleModule, moduleItem);
		return baseVehicleModule;
	}

	public void SetUpModule(BaseVehicleModule moduleEntity, Item moduleItem)
	{
		moduleEntity.InitializeHealth(moduleItem.condition, moduleItem.maxCondition);
		if (moduleItem.condition < moduleItem.maxCondition)
		{
			moduleEntity.SendNetworkUpdate();
		}
	}

	public Item GetVehicleItem(ItemId itemUID)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		Item item = Inventory.ChassisContainer.FindItemByUID(itemUID);
		if (item == null)
		{
			item = Inventory.ModuleContainer.FindItemByUID(itemUID);
		}
		return item;
	}

	public BaseVehicleModule GetModuleForItem(Item item)
	{
		if (item == null)
		{
			return null;
		}
		foreach (BaseVehicleModule attachedModuleEntity in AttachedModuleEntities)
		{
			if (attachedModuleEntity.AssociatedItemInstance == item)
			{
				return attachedModuleEntity;
			}
		}
		return null;
	}

	private void SetMass(float mass)
	{
		TotalMass = mass;
		rigidBody.mass = TotalMass;
	}

	private void SetCOM(Vector3 com)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		realLocalCOM = com;
		rigidBody.centerOfMass = Vector3.Scale(realLocalCOM, GetCOMMultiplier());
	}

	public override void InitShared()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		base.InitShared();
		AddMass(Mass, CentreOfMass, ((Component)this).transform.position);
		HasInited = true;
		foreach (BaseVehicleModule attachedModuleEntity in AttachedModuleEntities)
		{
			attachedModuleEntity.RefreshConditionals(canGib: false);
		}
	}

	public virtual bool PlayerCanUseThis(BasePlayer player, ModularCarCodeLock.LockType lockType)
	{
		return true;
	}

	public bool TryDeduceSocketIndex(BaseVehicleModule addedModule, out int index)
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		if (addedModule.FirstSocketIndex >= 0)
		{
			index = addedModule.FirstSocketIndex;
			return index >= 0;
		}
		index = -1;
		for (int i = 0; i < moduleSockets.Count; i++)
		{
			if (Vector3.SqrMagnitude(moduleSockets[i].WorldPosition - ((Component)addedModule).transform.position) < 0.1f)
			{
				index = i;
				return true;
			}
		}
		return false;
	}

	public void AddMass(float moduleMass, Vector3 moduleCOM, Vector3 moduleWorldPos)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		if (base.isServer)
		{
			Vector3 val = ((Component)this).transform.InverseTransformPoint(moduleWorldPos) + moduleCOM;
			if (TotalMass == 0f)
			{
				SetMass(moduleMass);
				SetCOM(val);
				return;
			}
			float num = TotalMass + moduleMass;
			Vector3 cOM = realLocalCOM * (TotalMass / num) + val * (moduleMass / num);
			SetMass(num);
			SetCOM(cOM);
		}
	}

	public void RemoveMass(float moduleMass, Vector3 moduleCOM, Vector3 moduleWorldPos)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		if (base.isServer)
		{
			float num = TotalMass - moduleMass;
			Vector3 val = ((Component)this).transform.InverseTransformPoint(moduleWorldPos) + moduleCOM;
			Vector3 cOM = (realLocalCOM - val * (moduleMass / TotalMass)) / (num / TotalMass);
			SetMass(num);
			SetCOM(cOM);
		}
	}

	public bool TryGetModuleAt(int socketIndex, out BaseVehicleModule result)
	{
		if (socketIndex < 0 || socketIndex >= moduleSockets.Count)
		{
			result = null;
			return false;
		}
		foreach (BaseVehicleModule attachedModuleEntity in AttachedModuleEntities)
		{
			int firstSocketIndex = attachedModuleEntity.FirstSocketIndex;
			int num = firstSocketIndex + attachedModuleEntity.GetNumSocketsTaken() - 1;
			if (firstSocketIndex <= socketIndex && num >= socketIndex)
			{
				result = attachedModuleEntity;
				return true;
			}
		}
		result = null;
		return false;
	}

	public ModularVehicleSocket GetSocket(int index)
	{
		if (index < 0 || index >= moduleSockets.Count)
		{
			return null;
		}
		return moduleSockets[index];
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (info.msg.modularVehicle == null)
		{
		}
	}

	protected override bool CanPushNow(BasePlayer pusher)
	{
		if (!base.CanPushNow(pusher))
		{
			return false;
		}
		return !IsKinematic && !IsEditableNow;
	}

	protected override void OnChildAdded(BaseEntity childEntity)
	{
		base.OnChildAdded(childEntity);
		BaseVehicleModule module;
		if ((module = childEntity as BaseVehicleModule) != null)
		{
			Action action = delegate
			{
				ModuleEntityAdded(module);
			};
			moduleAddActions[module] = action;
			((FacepunchBehaviour)module).Invoke(action, 0f);
		}
	}

	protected override void OnChildRemoved(BaseEntity childEntity)
	{
		base.OnChildRemoved(childEntity);
		if (childEntity is BaseVehicleModule removedModule)
		{
			ModuleEntityRemoved(removedModule);
		}
	}

	protected virtual void ModuleEntityAdded(BaseVehicleModule addedModule)
	{
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		if (AttachedModuleEntities.Contains(addedModule))
		{
			return;
		}
		if (base.isServer && ((Object)(object)this == (Object)null || IsDead() || base.IsDestroyed))
		{
			if ((Object)(object)addedModule != (Object)null && !addedModule.IsDestroyed)
			{
				addedModule.Kill();
			}
			return;
		}
		int index = -1;
		if (base.isServer && addedModule.AssociatedItemInstance != null)
		{
			index = addedModule.AssociatedItemInstance.position;
		}
		if (index == -1 && !TryDeduceSocketIndex(addedModule, out index))
		{
			string text = $"{((object)this).GetType().Name}: Couldn't get socket index from position ({((Component)addedModule).transform.position}).";
			for (int i = 0; i < moduleSockets.Count; i++)
			{
				text += $" Sqr dist to socket {i} at {moduleSockets[i].WorldPosition} is {Vector3.SqrMagnitude(moduleSockets[i].WorldPosition - ((Component)addedModule).transform.position)}.";
			}
			Debug.LogError((object)text, (Object)(object)((Component)addedModule).gameObject);
			return;
		}
		if (moduleAddActions.ContainsKey(addedModule))
		{
			moduleAddActions.Remove(addedModule);
		}
		AttachedModuleEntities.Add(addedModule);
		addedModule.ModuleAdded(this, index);
		AddMass(addedModule.Mass, addedModule.CentreOfMass, ((Component)addedModule).transform.position);
		if (base.isServer && !Inventory.TrySyncModuleInventory(addedModule, index))
		{
			Debug.LogError((object)$"{((object)this).GetType().Name}: Unable to add module {((Object)addedModule).name} to socket ({index}). Destroying it.", (Object)(object)((Component)this).gameObject);
			addedModule.Kill();
			AttachedModuleEntities.Remove(addedModule);
			return;
		}
		RefreshModulesExcept(addedModule);
		if (base.isServer)
		{
			UpdateMountFlags();
		}
	}

	protected virtual void ModuleEntityRemoved(BaseVehicleModule removedModule)
	{
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		if (base.IsDestroyed)
		{
			return;
		}
		if (moduleAddActions.ContainsKey(removedModule))
		{
			((FacepunchBehaviour)removedModule).CancelInvoke(moduleAddActions[removedModule]);
			moduleAddActions.Remove(removedModule);
		}
		if (AttachedModuleEntities.Contains(removedModule))
		{
			RemoveMass(removedModule.Mass, removedModule.CentreOfMass, ((Component)removedModule).transform.position);
			AttachedModuleEntities.Remove(removedModule);
			removedModule.ModuleRemoved();
			RefreshModulesExcept(removedModule);
			if (base.isServer)
			{
				UpdateMountFlags();
			}
		}
	}

	private void RefreshModulesExcept(BaseVehicleModule ignoredModule)
	{
		foreach (BaseVehicleModule attachedModuleEntity in AttachedModuleEntities)
		{
			if ((Object)(object)attachedModuleEntity != (Object)(object)ignoredModule)
			{
				attachedModuleEntity.OtherVehicleModulesChanged();
			}
		}
	}
}
