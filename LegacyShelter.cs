using ConVar;
using Facepunch;
using ProtoBuf;
using Rust;
using UnityEngine;

public class LegacyShelter : DecayEntity
{
	[Header("Shelter References")]
	public GameObjectRef smallPrivilegePrefab;

	public GameObjectRef includedDoorPrefab;

	public GameObjectRef includedLockPrefab;

	public EntityRef<EntityPrivilege> entityPrivilege;

	private EntityRef<LegacyShelterDoor> childDoorInstance;

	private EntityRef<BaseLock> lockEntityInstance;

	private BasePlayer owner;

	private Decay decayReference;

	private float lastShelterDecayTick;

	private float lastInteractedWithDoor;

	public override EntityPrivilege GetEntityBuildingPrivilege()
	{
		return GetEntityPrivilege();
	}

	public EntityPrivilege GetEntityPrivilege()
	{
		EntityPrivilege entityPrivilege = this.entityPrivilege.Get(base.isServer);
		if (entityPrivilege.IsValid())
		{
			return entityPrivilege;
		}
		return null;
	}

	protected override void OnChildAdded(BaseEntity child)
	{
		base.OnChildAdded(child);
		if (base.isServer && child.prefabID == includedDoorPrefab.GetEntity().prefabID && !Application.isLoadingSave)
		{
			Setup(child);
		}
		if (child.prefabID == smallPrivilegePrefab.GetEntity().prefabID)
		{
			EntityPrivilege entity = (EntityPrivilege)child;
			entityPrivilege.Set(entity);
		}
	}

	public override void DecayTick()
	{
		base.DecayTick();
		float num = Time.time - lastShelterDecayTick;
		lastShelterDecayTick = Time.time;
		float num2 = num * ConVar.Decay.scale;
		lastInteractedWithDoor += num2;
		UpdateDoorHp();
	}

	public void HasInteracted()
	{
		lastInteractedWithDoor = 0f;
	}

	public void SetupDecay()
	{
		decayReference = PrefabAttribute.server.Find<Decay>(prefabID);
	}

	public override float GetEntityDecayDuration()
	{
		if (lastInteractedWithDoor < 64800f)
		{
			return float.MaxValue;
		}
		return decayReference.GetDecayDuration(this);
	}

	public LegacyShelterDoor GetChildDoor()
	{
		LegacyShelterDoor legacyShelterDoor = childDoorInstance.Get(base.isServer);
		if (legacyShelterDoor.IsValid())
		{
			return legacyShelterDoor;
		}
		return null;
	}

	public override void Load(LoadInfo info)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		base.Load(info);
		if (info.msg.legacyShelter != null)
		{
			childDoorInstance = new EntityRef<LegacyShelterDoor>(info.msg.legacyShelter.doorID);
			lastInteractedWithDoor = info.msg.legacyShelter.timeSinceInteracted;
		}
	}

	public override void Save(SaveInfo info)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		base.Save(info);
		info.msg.legacyShelter = Pool.Get<LegacyShelter>();
		info.msg.legacyShelter.doorID = childDoorInstance.uid;
		info.msg.legacyShelter.timeSinceInteracted = lastInteractedWithDoor;
	}

	public override void OnPlaced(BasePlayer player)
	{
		owner = player;
	}

	public override void Hurt(HitInfo info)
	{
		base.Hurt(info);
		LegacyShelterDoor childDoor = GetChildDoor();
		if ((Object)(object)childDoor != (Object)null)
		{
			childDoor.ProtectedHurt(info);
		}
	}

	public override void OnKilled(HitInfo info)
	{
		base.OnKilled(info);
		LegacyShelterDoor childDoor = GetChildDoor();
		if ((Object)(object)childDoor != (Object)null && !childDoor.IsDead())
		{
			childDoor.Die();
		}
	}

	public override void OnRepair()
	{
		base.OnRepair();
		UpdateDoorHp();
	}

	public override void OnRepairFinished()
	{
		base.OnRepairFinished();
		UpdateDoorHp();
	}

	public void ProtectedHurt(HitInfo info)
	{
		info.HitEntity = this;
		base.Hurt(info);
	}

	public override void PostServerLoad()
	{
		base.PostServerLoad();
		LegacyShelterDoor childDoor = GetChildDoor();
		if (Object.op_Implicit((Object)(object)childDoor))
		{
			childDoor.SetupDoor(this);
			childDoor.SetMaxHealth(MaxHealth());
			UpdateDoorHp();
		}
		SetupDecay();
	}

	private void Setup(BaseEntity child)
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		LegacyShelterDoor legacyShelterDoor = (LegacyShelterDoor)child;
		childDoorInstance.Set(legacyShelterDoor);
		((Component)this).GetComponentInChildren<EntityPrivilege>().AddPlayer(owner);
		legacyShelterDoor.SetupDoor(this);
		legacyShelterDoor.SetMaxHealth(MaxHealth());
		UpdateDoorHp();
		BaseEntity baseEntity = GameManager.server.CreateEntity(includedLockPrefab.resourcePath);
		baseEntity.SetParent(legacyShelterDoor, legacyShelterDoor.GetSlotAnchorName(Slot.Lock));
		baseEntity.OwnerID = owner.userID;
		baseEntity.OnDeployed(legacyShelterDoor, owner, null);
		baseEntity.Spawn();
		BaseLock baseLock = (BaseLock)baseEntity;
		if ((Object)(object)baseLock != (Object)null)
		{
			baseLock.CanRemove = false;
		}
		legacyShelterDoor.SetSlot(Slot.Lock, baseEntity);
	}

	private void UpdateDoorHp()
	{
		LegacyShelterDoor childDoor = GetChildDoor();
		if ((Object)(object)childDoor != (Object)null)
		{
			childDoor.SetHealth(base.health);
		}
	}
}
