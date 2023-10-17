using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Rust/Player Inventory Properties")]
public class PlayerInventoryProperties : ScriptableObject
{
	[Serializable]
	public class ItemAmountSkinned : ItemAmount
	{
		public ulong skinOverride = 0uL;

		public bool blueprint = false;
	}

	public string niceName;

	public int order = 100;

	public List<ItemAmountSkinned> belt;

	public List<ItemAmountSkinned> main;

	public List<ItemAmountSkinned> wear;

	public PlayerInventoryProperties giveBase;

	private static PlayerInventoryProperties[] allInventories = null;

	public void GiveToPlayer(BasePlayer player)
	{
		if ((Object)(object)player == (Object)null)
		{
			return;
		}
		player.inventory.Strip();
		if ((Object)(object)giveBase != (Object)null)
		{
			giveBase.GiveToPlayer(player);
		}
		foreach (ItemAmountSkinned item2 in belt)
		{
			CreateItem(item2, player.inventory.containerBelt);
		}
		foreach (ItemAmountSkinned item3 in main)
		{
			CreateItem(item3, player.inventory.containerMain);
		}
		foreach (ItemAmountSkinned item4 in wear)
		{
			CreateItem(item4, player.inventory.containerWear);
		}
		void CreateItem(ItemAmountSkinned toCreate, ItemContainer destination)
		{
			Item item = null;
			if (toCreate.blueprint)
			{
				item = ItemManager.Create(ItemManager.blueprintBaseDef, 1, 0uL);
				item.blueprintTarget = (((Object)(object)toCreate.itemDef.isRedirectOf != (Object)null) ? toCreate.itemDef.isRedirectOf.itemid : toCreate.itemDef.itemid);
			}
			else
			{
				item = ItemManager.Create(toCreate.itemDef, (int)toCreate.amount, toCreate.skinOverride);
			}
			player.inventory.GiveItem(item, destination);
		}
	}

	public static PlayerInventoryProperties GetInventoryConfig(string name)
	{
		if (allInventories == null)
		{
			allInventories = FileSystem.LoadAll<PlayerInventoryProperties>("assets/content/properties/playerinventory", "");
			Debug.Log((object)$"Found {allInventories.Length} inventories");
		}
		if (allInventories != null)
		{
			PlayerInventoryProperties[] array = allInventories;
			foreach (PlayerInventoryProperties playerInventoryProperties in array)
			{
				if (playerInventoryProperties.niceName.ToLower() == name.ToLower())
				{
					return playerInventoryProperties;
				}
			}
		}
		return null;
	}
}
