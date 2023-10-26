using System;
using System.Collections.Generic;
using System.Linq;
using ConVar;
using Facepunch;
using Facepunch.Rust;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

public class RepairBench : StorageContainer
{
	public float maxConditionLostOnRepair = 0.2f;

	public GameObjectRef skinchangeEffect;

	public const float REPAIR_COST_FRACTION = 0.2f;

	private float nextSkinChangeTime;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("RepairBench.OnRpcMessage", 0);
		try
		{
			if (rpc == 1942825351 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - ChangeSkin "));
				}
				TimeWarning val2 = TimeWarning.New("ChangeSkin", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(1942825351u, "ChangeSkin", this, player, 3f))
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
							ChangeSkin(msg2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in ChangeSkin");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 1178348163 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - RepairItem "));
				}
				TimeWarning val2 = TimeWarning.New("RepairItem", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(1178348163u, "RepairItem", this, player, 3f))
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
							RPCMessage msg3 = rPCMessage;
							RepairItem(msg3);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in RepairItem");
					}
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

	public static float GetRepairFraction(Item itemToRepair)
	{
		return 1f - itemToRepair.condition / itemToRepair.maxCondition;
	}

	public static float RepairCostFraction(Item itemToRepair)
	{
		return GetRepairFraction(itemToRepair) * 0.2f;
	}

	public static void GetRepairCostList(ItemBlueprint bp, List<ItemAmount> allIngredients)
	{
		ItemDefinition targetItem = bp.targetItem;
		ItemModRepair itemModRepair = ((targetItem != null) ? ((Component)targetItem).GetComponent<ItemModRepair>() : null);
		if ((Object)(object)itemModRepair != (Object)null && itemModRepair.canUseRepairBench)
		{
			return;
		}
		foreach (ItemAmount ingredient in bp.ingredients)
		{
			allIngredients.Add(new ItemAmount(ingredient.itemDef, ingredient.amount));
		}
		StripComponentRepairCost(allIngredients);
	}

	public static void StripComponentRepairCost(List<ItemAmount> allIngredients)
	{
		if (allIngredients == null)
		{
			return;
		}
		for (int i = 0; i < allIngredients.Count; i++)
		{
			ItemAmount itemAmount = allIngredients[i];
			if (itemAmount.itemDef.category != ItemCategory.Component)
			{
				continue;
			}
			if ((Object)(object)itemAmount.itemDef.Blueprint != (Object)null)
			{
				bool flag = false;
				ItemAmount itemAmount2 = itemAmount.itemDef.Blueprint.ingredients[0];
				foreach (ItemAmount allIngredient in allIngredients)
				{
					if ((Object)(object)allIngredient.itemDef == (Object)(object)itemAmount2.itemDef)
					{
						allIngredient.amount += itemAmount2.amount * itemAmount.amount;
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					allIngredients.Add(new ItemAmount(itemAmount2.itemDef, itemAmount2.amount * itemAmount.amount));
				}
			}
			allIngredients.RemoveAt(i);
			i--;
		}
	}

	public void debugprint(string toPrint)
	{
		if (Global.developer > 0)
		{
			Debug.LogWarning((object)toPrint);
		}
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	public void ChangeSkin(RPCMessage msg)
	{
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0503: Unknown result type (might be due to invalid IL or missing references)
		bool flag = Time.realtimeSinceStartup < nextSkinChangeTime;
		BasePlayer player = msg.player;
		int num = msg.read.Int32();
		ItemId val = default(ItemId);
		((ItemId)(ref val))._002Ector(msg.read.UInt64());
		bool isValid = ((ItemId)(ref val)).IsValid;
		Item slot = base.inventory.GetSlot(0);
		if (slot == null || (isValid && slot.uid != val))
		{
			return;
		}
		bool flag2 = false;
		if (msg.player.UnlockAllSkins)
		{
			flag2 = true;
		}
		if (num != 0 && !flag2 && !player.blueprints.CheckSkinOwnership(num, player.userID))
		{
			debugprint("RepairBench.ChangeSkin player does not have item :" + num + ":");
			return;
		}
		ulong Skin = ItemDefinition.FindSkin(slot.info.itemid, num);
		if (Skin == slot.skin && (Object)(object)slot.info.isRedirectOf == (Object)null)
		{
			debugprint("RepairBench.ChangeSkin cannot apply same skin twice : " + Skin + ": " + slot.skin);
			return;
		}
		nextSkinChangeTime = Time.realtimeSinceStartup + 0.75f;
		ItemSkinDirectory.Skin skin = slot.info.skins.FirstOrDefault((ItemSkinDirectory.Skin x) => (ulong)x.id == Skin);
		if ((Object)(object)slot.info.isRedirectOf != (Object)null)
		{
			Skin = ItemDefinition.FindSkin(slot.info.isRedirectOf.itemid, num);
			skin = slot.info.isRedirectOf.skins.FirstOrDefault((ItemSkinDirectory.Skin x) => (ulong)x.id == Skin);
		}
		ItemSkin itemSkin = ((skin.id == 0) ? null : (skin.invItem as ItemSkin));
		if ((Object.op_Implicit((Object)(object)itemSkin) && ((Object)(object)itemSkin.Redirect != (Object)null || (Object)(object)slot.info.isRedirectOf != (Object)null)) || (!Object.op_Implicit((Object)(object)itemSkin) && (Object)(object)slot.info.isRedirectOf != (Object)null))
		{
			ItemDefinition template = (((Object)(object)itemSkin != (Object)null) ? itemSkin.Redirect : slot.info.isRedirectOf);
			bool flag3 = false;
			if ((Object)(object)itemSkin != (Object)null && (Object)(object)itemSkin.Redirect == (Object)null && (Object)(object)slot.info.isRedirectOf != (Object)null)
			{
				template = slot.info.isRedirectOf;
				flag3 = num != 0;
			}
			float condition = slot.condition;
			float maxCondition = slot.maxCondition;
			int amount = slot.amount;
			int contents = 0;
			ItemDefinition ammoType = null;
			if ((Object)(object)slot.GetHeldEntity() != (Object)null && slot.GetHeldEntity() is BaseProjectile baseProjectile && baseProjectile.primaryMagazine != null)
			{
				contents = baseProjectile.primaryMagazine.contents;
				ammoType = baseProjectile.primaryMagazine.ammoType;
			}
			List<Item> list = Pool.GetList<Item>();
			if (slot.contents != null && slot.contents.itemList != null && slot.contents.itemList.Count > 0)
			{
				foreach (Item item2 in slot.contents.itemList)
				{
					list.Add(item2);
				}
				foreach (Item item3 in list)
				{
					item3.RemoveFromContainer();
				}
			}
			slot.Remove();
			ItemManager.DoRemoves();
			Item item = ItemManager.Create(template, 1, 0uL);
			item.MoveToContainer(base.inventory, 0, allowStack: false);
			item.maxCondition = maxCondition;
			item.condition = condition;
			item.amount = amount;
			if ((Object)(object)item.GetHeldEntity() != (Object)null && item.GetHeldEntity() is BaseProjectile baseProjectile2)
			{
				if (baseProjectile2.primaryMagazine != null)
				{
					baseProjectile2.primaryMagazine.contents = contents;
					baseProjectile2.primaryMagazine.ammoType = ammoType;
				}
				baseProjectile2.ForceModsChanged();
			}
			if (list.Count > 0 && item.contents != null)
			{
				foreach (Item item4 in list)
				{
					item4.MoveToContainer(item.contents);
				}
			}
			Pool.FreeList<Item>(ref list);
			if (flag3)
			{
				ApplySkinToItem(item, Skin);
			}
			Analytics.Server.SkinUsed(item.info.shortname, num);
			Analytics.Azure.OnSkinChanged(player, this, item, Skin);
		}
		else
		{
			ApplySkinToItem(slot, Skin);
			Analytics.Server.SkinUsed(slot.info.shortname, num);
			Analytics.Azure.OnSkinChanged(player, this, slot, Skin);
		}
		if (flag && skinchangeEffect.isValid)
		{
			Effect.server.Run(skinchangeEffect.resourcePath, this, 0u, new Vector3(0f, 1.5f, 0f), Vector3.zero);
		}
	}

	private void ApplySkinToItem(Item item, ulong Skin)
	{
		item.skin = Skin;
		item.MarkDirty();
		BaseEntity heldEntity = item.GetHeldEntity();
		if ((Object)(object)heldEntity != (Object)null)
		{
			heldEntity.skinID = Skin;
			heldEntity.SendNetworkUpdate();
		}
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	public void RepairItem(RPCMessage msg)
	{
		Item slot = base.inventory.GetSlot(0);
		BasePlayer player = msg.player;
		float conditionLost = maxConditionLostOnRepair;
		ItemModRepair component = ((Component)slot.info).GetComponent<ItemModRepair>();
		if ((Object)(object)component != (Object)null)
		{
			conditionLost = component.conditionLost;
		}
		RepairAnItem(slot, player, this, conditionLost, mustKnowBlueprint: true);
	}

	public override int GetIdealSlot(BasePlayer player, Item item)
	{
		return 0;
	}

	public static void RepairAnItem(Item itemToRepair, BasePlayer player, BaseEntity repairBenchEntity, float maxConditionLostOnRepair, bool mustKnowBlueprint)
	{
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		if (itemToRepair == null)
		{
			return;
		}
		ItemDefinition info = itemToRepair.info;
		ItemBlueprint component = ((Component)info).GetComponent<ItemBlueprint>();
		if (!Object.op_Implicit((Object)(object)component))
		{
			return;
		}
		ItemModRepair component2 = ((Component)itemToRepair.info).GetComponent<ItemModRepair>();
		if (!info.condition.repairable || itemToRepair.condition == itemToRepair.maxCondition)
		{
			return;
		}
		if (mustKnowBlueprint)
		{
			ItemDefinition itemDefinition = (((Object)(object)info.isRedirectOf != (Object)null) ? info.isRedirectOf : info);
			if (!player.blueprints.HasUnlocked(itemDefinition) && (!((Object)(object)itemDefinition.Blueprint != (Object)null) || itemDefinition.Blueprint.isResearchable))
			{
				return;
			}
		}
		float num = RepairCostFraction(itemToRepair);
		bool flag = false;
		List<ItemAmount> list = Pool.GetList<ItemAmount>();
		GetRepairCostList(component, list);
		foreach (ItemAmount item in list)
		{
			if (item.itemDef.category != ItemCategory.Component)
			{
				int amount = player.inventory.GetAmount(item.itemDef.itemid);
				if (Mathf.CeilToInt(item.amount * num) > amount)
				{
					flag = true;
					break;
				}
			}
		}
		if (flag)
		{
			Pool.FreeList<ItemAmount>(ref list);
			return;
		}
		foreach (ItemAmount item2 in list)
		{
			if (item2.itemDef.category != ItemCategory.Component)
			{
				int amount2 = Mathf.CeilToInt(item2.amount * num);
				player.inventory.Take(null, item2.itemid, amount2);
				Analytics.Azure.LogResource(Analytics.Azure.ResourceMode.Consumed, "repair", item2.itemDef.shortname, amount2, repairBenchEntity, null, safezone: false, null, 0uL, null, itemToRepair);
			}
		}
		Pool.FreeList<ItemAmount>(ref list);
		float conditionNormalized = itemToRepair.conditionNormalized;
		float maxConditionNormalized = itemToRepair.maxConditionNormalized;
		itemToRepair.DoRepair(maxConditionLostOnRepair);
		Analytics.Azure.OnItemRepaired(player, repairBenchEntity, itemToRepair, conditionNormalized, maxConditionNormalized);
		if (Global.developer > 0)
		{
			Debug.Log((object)("Item repaired! condition : " + itemToRepair.condition + "/" + itemToRepair.maxCondition));
		}
		string strName = "assets/bundled/prefabs/fx/repairbench/itemrepair.prefab";
		if ((Object)(object)component2 != (Object)null && (Object)(object)component2.successEffect?.Get() != (Object)null)
		{
			strName = component2.successEffect.resourcePath;
		}
		Effect.server.Run(strName, repairBenchEntity, 0u, Vector3.zero, Vector3.zero);
	}
}
