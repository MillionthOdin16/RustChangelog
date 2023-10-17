using ProtoBuf;
using Rust;
using UnityEngine;

public class WeaponRackSlot
{
	public bool Used;

	public ItemDefinition ItemDef;

	public int ClientItemID;

	public ulong ClientItemSkinID;

	public ItemDefinition AmmoItemDef;

	public int AmmoItemID;

	public int AmmoCount;

	public int AmmoMax;

	public float Condition;

	public int InventoryIndex;

	public int GridSlotIndex;

	public int Rotation;

	[InspectorFlags]
	public AmmoTypes AmmoTypes;

	public WeaponRackItem SaveToProto(Item item, WeaponRackItem proto)
	{
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Expected I4, but got Unknown
		proto.itemID = item?.info.itemid ?? 0;
		proto.skinid = item?.skin ?? 0;
		proto.inventorySlot = InventoryIndex;
		proto.gridSlotIndex = GridSlotIndex;
		proto.rotation = Rotation;
		proto.ammoCount = AmmoCount;
		proto.ammoMax = AmmoMax;
		proto.ammoID = AmmoItemID;
		proto.condition = Condition;
		proto.ammoTypes = (int)AmmoTypes;
		return proto;
	}

	public void InitFromProto(WeaponRackItem item)
	{
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		ClientItemID = item.itemID;
		ClientItemSkinID = item.skinid;
		ItemDef = ItemManager.FindItemDefinition(ClientItemID);
		InventoryIndex = item.inventorySlot;
		GridSlotIndex = item.gridSlotIndex;
		AmmoCount = item.ammoCount;
		AmmoMax = item.ammoMax;
		AmmoItemID = item.ammoID;
		AmmoItemDef = ((AmmoItemID != 0) ? ItemManager.FindItemDefinition(AmmoItemID) : null);
		Condition = item.condition;
		Rotation = item.rotation;
		AmmoTypes = (AmmoTypes)item.ammoTypes;
	}

	public void SetItem(Item item, ItemDefinition updatedItemDef, int gridCellIndex, int rotation)
	{
		InventoryIndex = item.position;
		GridSlotIndex = gridCellIndex;
		Condition = item.conditionNormalized;
		Rotation = rotation;
		SetAmmoDetails(item);
		ItemDef = updatedItemDef;
	}

	public void SetAmmoDetails(Item item)
	{
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		BaseEntity heldEntity = item.GetHeldEntity();
		if (!((Object)(object)heldEntity == (Object)null))
		{
			BaseProjectile component = ((Component)heldEntity).GetComponent<BaseProjectile>();
			if (!((Object)(object)component == (Object)null))
			{
				AmmoItemDef = component.primaryMagazine.ammoType;
				AmmoItemID = (((Object)(object)AmmoItemDef != (Object)null) ? AmmoItemDef.itemid : 0);
				AmmoCount = component.primaryMagazine.contents;
				AmmoMax = component.primaryMagazine.capacity;
				AmmoTypes = component.primaryMagazine.definition.ammoTypes;
			}
		}
	}
}
