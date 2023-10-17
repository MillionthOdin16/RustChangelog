using System;
using System.Collections.Generic;
using System.Linq;
using ConVar;
using Facepunch;
using Facepunch.Rust;
using ProtoBuf;
using UnityEngine;

public class ItemCrafter : EntityComponent<BasePlayer>
{
	public List<ItemContainer> containers = new List<ItemContainer>();

	public LinkedList<ItemCraftTask> queue = new LinkedList<ItemCraftTask>();

	public int taskUID;

	[NonSerialized]
	public BasePlayer owner;

	public void AddContainer(ItemContainer container)
	{
		containers.Add(container);
	}

	public static float GetScaledDuration(ItemBlueprint bp, float workbenchLevel)
	{
		float num = workbenchLevel - (float)bp.workbenchLevelRequired;
		if (num == 1f)
		{
			return bp.time * 0.5f;
		}
		if (num >= 2f)
		{
			return bp.time * 0.25f;
		}
		return bp.time;
	}

	public void ServerUpdate(float delta)
	{
		if (queue.Count == 0)
		{
			return;
		}
		ItemCraftTask value = queue.First.Value;
		if (value.cancelled)
		{
			owner.Command("note.craft_done", value.taskUID, 0);
			queue.RemoveFirst();
			return;
		}
		float currentCraftLevel = owner.currentCraftLevel;
		if (value.endTime > Time.realtimeSinceStartup)
		{
			return;
		}
		if (value.endTime == 0f)
		{
			float scaledDuration = GetScaledDuration(value.blueprint, currentCraftLevel);
			value.endTime = Time.realtimeSinceStartup + scaledDuration;
			value.workbenchEntity = owner.GetCachedCraftLevelWorkbench();
			if ((Object)(object)owner != (Object)null)
			{
				owner.Command("note.craft_start", value.taskUID, scaledDuration, value.amount);
				if (owner.IsAdmin && Craft.instant)
				{
					value.endTime = Time.realtimeSinceStartup + 1f;
				}
			}
		}
		else
		{
			FinishCrafting(value);
			if (value.amount <= 0)
			{
				queue.RemoveFirst();
			}
			else
			{
				value.endTime = 0f;
			}
		}
	}

	private void CollectIngredient(int item, int amount, List<Item> collect)
	{
		foreach (ItemContainer container in containers)
		{
			amount -= container.Take(collect, item, amount);
			if (amount <= 0)
			{
				break;
			}
		}
	}

	private void CollectIngredients(ItemBlueprint bp, ItemCraftTask task, int amount = 1, BasePlayer player = null)
	{
		List<Item> list = new List<Item>();
		foreach (ItemAmount ingredient in bp.ingredients)
		{
			CollectIngredient(ingredient.itemid, (int)ingredient.amount * amount, list);
		}
		foreach (Item item in list)
		{
			item.CollectedForCrafting(player);
		}
		task.takenItems = list;
	}

	public bool CraftItem(ItemBlueprint bp, BasePlayer owner, InstanceData instanceData = null, int amount = 1, int skinID = 0, Item fromTempBlueprint = null, bool free = false)
	{
		if ((Object)(object)owner != (Object)null && owner.IsTransferring())
		{
			return false;
		}
		if (!CanCraft(bp, amount, free))
		{
			return false;
		}
		taskUID++;
		ItemCraftTask itemCraftTask = Pool.Get<ItemCraftTask>();
		itemCraftTask.blueprint = bp;
		if (!free)
		{
			CollectIngredients(bp, itemCraftTask, amount, owner);
		}
		itemCraftTask.endTime = 0f;
		itemCraftTask.taskUID = taskUID;
		itemCraftTask.instanceData = instanceData;
		if (itemCraftTask.instanceData != null)
		{
			itemCraftTask.instanceData.ShouldPool = false;
		}
		itemCraftTask.amount = amount;
		itemCraftTask.skinID = skinID;
		if (fromTempBlueprint != null && itemCraftTask.takenItems != null)
		{
			fromTempBlueprint.RemoveFromContainer();
			itemCraftTask.takenItems.Add(fromTempBlueprint);
			itemCraftTask.conditionScale = 0.5f;
		}
		queue.AddLast(itemCraftTask);
		if ((Object)(object)owner != (Object)null)
		{
			owner.Command("note.craft_add", itemCraftTask.taskUID, itemCraftTask.blueprint.targetItem.itemid, amount, itemCraftTask.skinID);
		}
		return true;
	}

	private void FinishCrafting(ItemCraftTask task)
	{
		//IL_0331: Unknown result type (might be due to invalid IL or missing references)
		//IL_0338: Unknown result type (might be due to invalid IL or missing references)
		//IL_033f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0345: Unknown result type (might be due to invalid IL or missing references)
		task.amount--;
		task.numCrafted++;
		ulong skin = ItemDefinition.FindSkin(task.blueprint.targetItem.itemid, task.skinID);
		Item item = ItemManager.CreateByItemID(task.blueprint.targetItem.itemid, 1, skin);
		item.amount = task.blueprint.amountToCreate;
		int amount = item.amount;
		_ = owner.currentCraftLevel;
		bool inSafezone = owner.InSafeZone();
		if (item.hasCondition && task.conditionScale != 1f)
		{
			item.maxCondition *= task.conditionScale;
			item.condition = item.maxCondition;
		}
		item.OnVirginSpawn();
		foreach (ItemAmount ingredient in task.blueprint.ingredients)
		{
			int num = (int)ingredient.amount;
			if (task.takenItems == null)
			{
				continue;
			}
			foreach (Item takenItem in task.takenItems)
			{
				if ((Object)(object)takenItem.info == (Object)(object)ingredient.itemDef)
				{
					int num2 = Mathf.Min(takenItem.amount, num);
					Analytics.Azure.OnCraftMaterialConsumed(takenItem.info.shortname, num, base.baseEntity, task.workbenchEntity, inSafezone, item.info.shortname);
					takenItem.UseItem(num);
					num -= num2;
				}
				_ = 0;
			}
		}
		Analytics.Server.Crafting(task.blueprint.targetItem.shortname, task.skinID);
		Analytics.Azure.OnCraftItem(item.info.shortname, item.amount, base.baseEntity, task.workbenchEntity, inSafezone);
		owner.Command("note.craft_done", task.taskUID, 1, task.amount);
		if (task.instanceData != null)
		{
			item.instanceData = task.instanceData;
		}
		if (!string.IsNullOrEmpty(task.blueprint.UnlockAchievment))
		{
			owner.GiveAchievement(task.blueprint.UnlockAchievment);
		}
		owner.ProcessMissionEvent(BaseMission.MissionEventType.CRAFT_ITEM, item.info.itemid, amount);
		if (owner.inventory.GiveItem(item))
		{
			owner.Command("note.inv", item.info.itemid, amount);
			return;
		}
		ItemContainer itemContainer = containers.First();
		owner.Command("note.inv", item.info.itemid, amount);
		owner.Command("note.inv", item.info.itemid, -item.amount);
		item.Drop(itemContainer.dropPosition, itemContainer.dropVelocity);
	}

	public bool CancelTask(int iID, bool ReturnItems)
	{
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		if (queue.Count == 0)
		{
			return false;
		}
		if ((Object)(object)owner != (Object)null && owner.IsTransferring())
		{
			return false;
		}
		ItemCraftTask itemCraftTask = queue.FirstOrDefault((ItemCraftTask x) => x.taskUID == iID && !x.cancelled);
		if (itemCraftTask == null)
		{
			return false;
		}
		itemCraftTask.cancelled = true;
		if ((Object)(object)owner == (Object)null)
		{
			return true;
		}
		owner.Command("note.craft_done", itemCraftTask.taskUID, 0);
		if (itemCraftTask.takenItems != null && itemCraftTask.takenItems.Count > 0 && ReturnItems)
		{
			foreach (Item takenItem in itemCraftTask.takenItems)
			{
				if (takenItem != null && takenItem.amount > 0)
				{
					if (takenItem.IsBlueprint() && (Object)(object)takenItem.blueprintTargetDef == (Object)(object)itemCraftTask.blueprint.targetItem)
					{
						takenItem.UseItem(itemCraftTask.numCrafted);
					}
					if (takenItem.amount > 0 && !takenItem.MoveToContainer(owner.inventory.containerMain))
					{
						takenItem.Drop(owner.inventory.containerMain.dropPosition + Random.value * Vector3.down + Random.insideUnitSphere, owner.inventory.containerMain.dropVelocity);
						owner.Command("note.inv", takenItem.info.itemid, -takenItem.amount);
					}
				}
			}
		}
		return true;
	}

	public bool CancelBlueprint(int itemid)
	{
		if (queue.Count == 0)
		{
			return false;
		}
		if ((Object)(object)owner != (Object)null && owner.IsTransferring())
		{
			return false;
		}
		ItemCraftTask itemCraftTask = queue.FirstOrDefault((ItemCraftTask x) => x.blueprint.targetItem.itemid == itemid && !x.cancelled);
		if (itemCraftTask == null)
		{
			return false;
		}
		return CancelTask(itemCraftTask.taskUID, ReturnItems: true);
	}

	public void CancelAll(bool returnItems)
	{
		foreach (ItemCraftTask item in queue)
		{
			CancelTask(item.taskUID, returnItems);
		}
	}

	private bool DoesHaveUsableItem(int item, int iAmount)
	{
		int num = 0;
		foreach (ItemContainer container in containers)
		{
			num += container.GetAmount(item, onlyUsableAmounts: true);
		}
		return num >= iAmount;
	}

	public bool CanCraft(ItemBlueprint bp, int amount = 1, bool free = false)
	{
		float num = (float)amount / (float)bp.targetItem.craftingStackable;
		foreach (ItemCraftTask item in queue)
		{
			if (!item.cancelled)
			{
				num += (float)item.amount / (float)item.blueprint.targetItem.craftingStackable;
			}
		}
		if (num > 8f)
		{
			return false;
		}
		if (amount < 1 || amount > bp.targetItem.craftingStackable)
		{
			return false;
		}
		foreach (ItemAmount ingredient in bp.ingredients)
		{
			if (!DoesHaveUsableItem(ingredient.itemid, (int)ingredient.amount * amount))
			{
				return false;
			}
		}
		return true;
	}

	public bool CanCraft(ItemDefinition def, int amount = 1, bool free = false)
	{
		ItemBlueprint component = ((Component)def).GetComponent<ItemBlueprint>();
		if (CanCraft(component, amount, free))
		{
			return true;
		}
		return false;
	}

	public bool FastTrackTask(int taskID)
	{
		if (queue.Count == 0)
		{
			return false;
		}
		if ((Object)(object)owner != (Object)null && owner.IsTransferring())
		{
			return false;
		}
		ItemCraftTask value = queue.First.Value;
		if (value == null)
		{
			return false;
		}
		ItemCraftTask itemCraftTask = queue.FirstOrDefault((ItemCraftTask x) => x.taskUID == taskID && !x.cancelled);
		if (itemCraftTask == null)
		{
			return false;
		}
		if (itemCraftTask == value)
		{
			return false;
		}
		value.endTime = 0f;
		queue.Remove(itemCraftTask);
		queue.AddFirst(itemCraftTask);
		owner.Command("note.craft_fasttracked", taskID);
		return true;
	}

	public ItemCrafter Save()
	{
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		ItemCrafter val = Pool.Get<ItemCrafter>();
		val.queue = Pool.GetList<Task>();
		foreach (ItemCraftTask item in queue)
		{
			Task val2 = Pool.Get<Task>();
			val2.itemID = item.blueprint.targetItem.itemid;
			val2.remainingTime = ((item.endTime > 0f) ? (item.endTime - Time.realtimeSinceStartup) : 0f);
			val2.taskUID = item.taskUID;
			val2.cancelled = item.cancelled;
			InstanceData instanceData = item.instanceData;
			val2.instanceData = ((instanceData != null) ? instanceData.Copy() : null);
			val2.amount = item.amount;
			val2.skinID = item.skinID;
			val2.takenItems = SaveItems(item.takenItems);
			val2.numCrafted = item.numCrafted;
			val2.conditionScale = item.conditionScale;
			val2.workbenchEntity = (NetworkableId)(item.workbenchEntity.IsValid() ? item.workbenchEntity.net.ID : default(NetworkableId));
			val.queue.Add(val2);
		}
		return val;
		static List<Item> SaveItems(List<Item> items)
		{
			List<Item> list = Pool.GetList<Item>();
			if (items != null)
			{
				foreach (Item item2 in items)
				{
					list.Add(item2.Save(bIncludeContainer: true));
				}
			}
			return list;
		}
	}

	public void Load(ItemCrafter proto)
	{
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		if (proto?.queue == null)
		{
			return;
		}
		queue.Clear();
		ItemBlueprint blueprint = default(ItemBlueprint);
		foreach (Task item in proto.queue)
		{
			ItemDefinition itemDefinition = ItemManager.FindItemDefinition(item.itemID);
			if ((Object)(object)itemDefinition == (Object)null || !((Component)itemDefinition).TryGetComponent<ItemBlueprint>(ref blueprint))
			{
				Debug.LogWarning((object)$"ItemCrafter has queue task for item ID {item.itemID}, but it was not found or has no blueprint. Skipping it");
				continue;
			}
			ItemCraftTask itemCraftTask = Pool.Get<ItemCraftTask>();
			itemCraftTask.blueprint = blueprint;
			itemCraftTask.endTime = ((item.remainingTime > 0f) ? (Time.realtimeSinceStartup + item.remainingTime) : 0f);
			itemCraftTask.taskUID = item.taskUID;
			itemCraftTask.cancelled = item.cancelled;
			InstanceData instanceData = item.instanceData;
			itemCraftTask.instanceData = ((instanceData != null) ? instanceData.Copy() : null);
			itemCraftTask.amount = item.amount;
			itemCraftTask.skinID = item.skinID;
			itemCraftTask.takenItems = LoadItems(item.takenItems);
			itemCraftTask.numCrafted = item.numCrafted;
			itemCraftTask.conditionScale = item.conditionScale;
			itemCraftTask.workbenchEntity = new EntityRef<BaseEntity>
			{
				uid = item.workbenchEntity
			}.Get(serverside: true);
			queue.AddLast(itemCraftTask);
			taskUID = Mathf.Max(taskUID, itemCraftTask.taskUID);
		}
		static List<Item> LoadItems(List<Item> itemProtos)
		{
			List<Item> list = new List<Item>();
			if (itemProtos != null)
			{
				foreach (Item itemProto in itemProtos)
				{
					list.Add(ItemManager.Load(itemProto, null, isServer: true));
				}
			}
			return list;
		}
	}

	public void SendToOwner()
	{
		if (!owner.IsValid() || !owner.IsConnected)
		{
			return;
		}
		foreach (ItemCraftTask item in queue)
		{
			owner.Command("note.craft_add", item.taskUID, item.blueprint.targetItem.itemid, item.amount, item.skinID);
		}
	}
}
