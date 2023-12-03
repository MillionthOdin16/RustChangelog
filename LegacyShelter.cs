using Rust;
using UnityEngine;

public class LegacyShelter : DecayEntity
{
	[Header("Shelter Settings")]
	public GameObjectRef includedDoorPrefab;

	public GameObjectRef includedLockPrefab;

	private LegacyShelterDoor childDoor;

	private EntityRef<BaseLock> lockEntityInstance;

	private BasePlayer owner;

	protected override void OnChildAdded(BaseEntity child)
	{
		base.OnChildAdded(child);
		if (base.isServer && child.prefabID == includedDoorPrefab.GetEntity().prefabID)
		{
			childDoor = child as LegacyShelterDoor;
			if (!Application.isLoadingSave)
			{
				Setup();
			}
		}
	}

	public override void OnPlaced(BasePlayer player)
	{
		owner = player;
	}

	public override void Hurt(HitInfo info)
	{
		base.Hurt(info);
		childDoor.ProtectedHurt(info);
	}

	public override void OnKilled(HitInfo info)
	{
		base.OnKilled(info);
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

	private void Setup()
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		((Component)this).GetComponentInChildren<EntityPrivilege>().AddPlayer(owner);
		childDoor.SetupDoor(this);
		childDoor.SetMaxHealth(MaxHealth());
		UpdateDoorHp();
		BaseEntity baseEntity = GameManager.server.CreateEntity(includedLockPrefab.resourcePath);
		baseEntity.SetParent(childDoor, childDoor.GetSlotAnchorName(Slot.Lock));
		baseEntity.OwnerID = owner.userID;
		baseEntity.OnDeployed(childDoor, owner, null);
		baseEntity.Spawn();
		childDoor.SetSlot(Slot.Lock, baseEntity);
	}

	private void UpdateDoorHp()
	{
		if ((Object)(object)childDoor != (Object)null)
		{
			childDoor.SetHealth(base.health);
		}
	}
}
