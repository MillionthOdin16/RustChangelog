using Facepunch;
using ProtoBuf;
using Rust;
using UnityEngine;

public class LegacyShelter : DecayEntity
{
	[Header("Shelter Settings")]
	public GameObjectRef includedDoorPrefab;

	public GameObjectRef includedLockPrefab;

	private EntityRef<LegacyShelterDoor> childDoorInstance;

	private EntityRef<BaseLock> lockEntityInstance;

	private BasePlayer owner;

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
		}
	}

	public override void Save(SaveInfo info)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		base.Save(info);
		info.msg.legacyShelter = Pool.Get<LegacyShelter>();
		info.msg.legacyShelter.doorID = childDoorInstance.uid;
	}

	protected override void OnChildAdded(BaseEntity child)
	{
		base.OnChildAdded(child);
		if (base.isServer && child.prefabID == includedDoorPrefab.GetEntity().prefabID && !Application.isLoadingSave)
		{
			Setup(child);
		}
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

	public override void DecayTick()
	{
		base.DecayTick();
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
