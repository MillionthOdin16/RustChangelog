using System;
using System.Collections.Generic;
using System.Linq;
using ConVar;
using Facepunch;
using Facepunch.Rust;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

public class PlayerInventory : EntityComponent<BasePlayer>, IAmmoContainer
{
	public enum Type
	{
		Main,
		Belt,
		Wear
	}

	public interface ICanMoveFrom
	{
		bool CanMoveFrom(BasePlayer player, Item item);
	}

	public ItemContainer containerMain;

	public ItemContainer containerBelt;

	public ItemContainer containerWear;

	public ItemCrafter crafting;

	public PlayerLoot loot;

	public static Phrase BackpackGroundedError = new Phrase("error.backpackGrounded", "You must be on a solid surface to equip a backpack");

	[ServerVar]
	public static bool forceBirthday = false;

	private static float nextCheckTime = 0f;

	private static bool wasBirthday = false;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("PlayerInventory.OnRpcMessage", 0);
		try
		{
			if (rpc == 3482449460u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - ItemCmd "));
				}
				TimeWarning val2 = TimeWarning.New("ItemCmd", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!BaseEntity.RPC_Server.FromOwner.Test(3482449460u, "ItemCmd", GetBaseEntity(), player))
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
							BaseEntity.RPCMessage rPCMessage = default(BaseEntity.RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							BaseEntity.RPCMessage msg2 = rPCMessage;
							ItemCmd(msg2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in ItemCmd");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 3041092525u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - MoveItem "));
				}
				TimeWarning val2 = TimeWarning.New("MoveItem", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!BaseEntity.RPC_Server.FromOwner.Test(3041092525u, "MoveItem", GetBaseEntity(), player))
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
							BaseEntity.RPCMessage rPCMessage = default(BaseEntity.RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							BaseEntity.RPCMessage msg3 = rPCMessage;
							MoveItem(msg3);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in MoveItem");
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

	protected void Initialize(BasePlayer owner)
	{
		containerMain = new ItemContainer();
		containerMain.SetFlag(ItemContainer.Flag.IsPlayer, b: true);
		containerBelt = new ItemContainer();
		containerBelt.SetFlag(ItemContainer.Flag.IsPlayer, b: true);
		containerBelt.SetFlag(ItemContainer.Flag.Belt, b: true);
		containerWear = new ItemContainer();
		containerWear.SetFlag(ItemContainer.Flag.IsPlayer, b: true);
		containerWear.SetFlag(ItemContainer.Flag.Clothing, b: true);
		crafting = ((Component)this).GetComponent<ItemCrafter>();
		if ((Object)(object)crafting != (Object)null)
		{
			crafting.owner = owner;
			crafting.AddContainer(containerMain);
			crafting.AddContainer(containerBelt);
		}
		loot = ((Component)this).GetComponent<PlayerLoot>();
		if (!Object.op_Implicit((Object)(object)loot))
		{
			loot = ((Component)this).gameObject.AddComponent<PlayerLoot>();
		}
	}

	public void DoDestroy()
	{
		if (containerMain != null)
		{
			containerMain.Kill();
			containerMain = null;
		}
		if (containerBelt != null)
		{
			containerBelt.Kill();
			containerBelt = null;
		}
		if (containerWear != null)
		{
			containerWear.Kill();
			containerWear = null;
		}
	}

	public void ServerInit(BasePlayer owner)
	{
		Initialize(owner);
		containerMain.ServerInitialize(null, 24);
		if (!((ItemContainerId)(ref containerMain.uid)).IsValid)
		{
			containerMain.GiveUID();
		}
		containerBelt.ServerInitialize(null, 6);
		if (!((ItemContainerId)(ref containerBelt.uid)).IsValid)
		{
			containerBelt.GiveUID();
		}
		containerWear.ServerInitialize(null, 8);
		if (!((ItemContainerId)(ref containerWear.uid)).IsValid)
		{
			containerWear.GiveUID();
		}
		containerMain.playerOwner = owner;
		containerBelt.playerOwner = owner;
		containerWear.playerOwner = owner;
		containerWear.onItemAddedRemoved = OnClothingChanged;
		containerWear.canAcceptItem = CanWearItem;
		containerBelt.canAcceptItem = CanEquipItem;
		containerMain.canAcceptItem = CanStoreInInventory;
		containerMain.onPreItemRemove = OnItemRemoved;
		containerWear.onPreItemRemove = OnItemRemoved;
		containerBelt.onPreItemRemove = OnItemRemoved;
		containerMain.onDirty += OnContentsDirty;
		containerBelt.onDirty += OnContentsDirty;
		containerWear.onDirty += OnContentsDirty;
		containerBelt.onItemAddedRemoved = OnItemAddedOrRemoved;
		containerMain.onItemAddedRemoved = OnMainInventoryItemAddedOrRemoved;
	}

	private void OnMainInventoryItemAddedOrRemoved(Item item, bool bAdded)
	{
		OnItemAddedOrRemoved(item, bAdded);
	}

	public void OnItemAddedOrRemoved(Item item, bool bAdded)
	{
		if (item.info.isHoldable)
		{
			((FacepunchBehaviour)this).Invoke((Action)UpdatedVisibleHolsteredItems, 0.1f);
		}
		if (bAdded)
		{
			BasePlayer basePlayer = base.baseEntity;
			if (!basePlayer.HasPlayerFlag(BasePlayer.PlayerFlags.DisplaySash) && basePlayer.IsHostileItem(item))
			{
				base.baseEntity.SetPlayerFlag(BasePlayer.PlayerFlags.DisplaySash, b: true);
			}
			if (bAdded)
			{
				basePlayer.ProcessMissionEvent(BaseMission.MissionEventType.ACQUIRE_ITEM, item.info.shortname, item.amount);
			}
		}
	}

	public void UpdatedVisibleHolsteredItems()
	{
		List<HeldEntity> list = Pool.GetList<HeldEntity>();
		List<Item> items = Pool.GetList<Item>();
		AllItemsNoAlloc(ref items);
		foreach (Item item in items)
		{
			if (item.info.isHoldable && !((Object)(object)item.GetHeldEntity() == (Object)null))
			{
				HeldEntity component = ((Component)item.GetHeldEntity()).GetComponent<HeldEntity>();
				if (!((Object)(object)component == (Object)null))
				{
					list.Add(component);
				}
			}
		}
		Pool.FreeList<Item>(ref items);
		IOrderedEnumerable<HeldEntity> orderedEnumerable = list.OrderByDescending((HeldEntity x) => x.hostileScore);
		bool flag = true;
		bool flag2 = true;
		bool flag3 = true;
		foreach (HeldEntity item2 in orderedEnumerable)
		{
			if (!((Object)(object)item2 == (Object)null) && item2.holsterInfo.displayWhenHolstered)
			{
				if (flag3 && !item2.IsDeployed() && item2.holsterInfo.slot == HeldEntity.HolsterInfo.HolsterSlot.BACK)
				{
					item2.SetVisibleWhileHolstered(visible: true);
					flag3 = false;
				}
				else if (flag2 && !item2.IsDeployed() && item2.holsterInfo.slot == HeldEntity.HolsterInfo.HolsterSlot.RIGHT_THIGH)
				{
					item2.SetVisibleWhileHolstered(visible: true);
					flag2 = false;
				}
				else if (flag && !item2.IsDeployed() && item2.holsterInfo.slot == HeldEntity.HolsterInfo.HolsterSlot.LEFT_THIGH)
				{
					item2.SetVisibleWhileHolstered(visible: true);
					flag = false;
				}
				else
				{
					item2.SetVisibleWhileHolstered(visible: false);
				}
			}
		}
		Pool.FreeList<HeldEntity>(ref list);
	}

	private void OnContentsDirty()
	{
		if ((Object)(object)base.baseEntity != (Object)null)
		{
			base.baseEntity.InvalidateNetworkCache();
		}
	}

	private bool CanMoveItemsFrom(BaseEntity entity, Item item)
	{
		if (entity is ICanMoveFrom canMoveFrom && !canMoveFrom.CanMoveFrom(base.baseEntity, item))
		{
			return false;
		}
		if (Object.op_Implicit((Object)(object)BaseGameMode.GetActiveGameMode(serverside: true)))
		{
			return BaseGameMode.GetActiveGameMode(serverside: true).CanMoveItemsFrom(this, entity, item);
		}
		return true;
	}

	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.FromOwner]
	private void ItemCmd(BaseEntity.RPCMessage msg)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		if (((Object)(object)msg.player != (Object)null && msg.player.IsWounded()) || base.baseEntity.IsTransferring())
		{
			return;
		}
		ItemId id = msg.read.ItemID();
		string text = msg.read.String(256, false);
		Item item = FindItemByUID(id);
		if (item == null || item.IsLocked() || (item.parent != null && item.parent.IsLocked()) || !CanMoveItemsFrom(item.GetEntityOwner(), item))
		{
			return;
		}
		if (text == "drop")
		{
			int num = item.amount;
			if (msg.read.Unread >= 4)
			{
				num = msg.read.Int32();
			}
			base.baseEntity.stats.Add("item_drop", 1, (Stats)5);
			if (num < item.amount)
			{
				Item item2 = item.SplitItem(num);
				if (item2 != null)
				{
					DroppedItem droppedItem = item2.Drop(base.baseEntity.GetDropPosition(), base.baseEntity.GetDropVelocity()) as DroppedItem;
					if ((Object)(object)droppedItem != (Object)null)
					{
						droppedItem.DropReason = DroppedItem.DropReasonEnum.Player;
						droppedItem.DroppedBy = base.baseEntity.userID;
						Analytics.Azure.OnItemDropped(base.baseEntity, droppedItem, DroppedItem.DropReasonEnum.Player);
					}
				}
			}
			else
			{
				DroppedItem droppedItem2 = item.Drop(base.baseEntity.GetDropPosition(), base.baseEntity.GetDropVelocity()) as DroppedItem;
				if ((Object)(object)droppedItem2 != (Object)null)
				{
					droppedItem2.DropReason = DroppedItem.DropReasonEnum.Player;
					droppedItem2.DroppedBy = base.baseEntity.userID;
					Analytics.Azure.OnItemDropped(base.baseEntity, droppedItem2, DroppedItem.DropReasonEnum.Player);
				}
			}
			base.baseEntity.SignalBroadcast(BaseEntity.Signal.Gesture, "drop_item");
		}
		else
		{
			item.ServerCommand(text, base.baseEntity);
			ItemManager.DoRemoves();
			ServerUpdate(0f);
		}
	}

	[BaseEntity.RPC_Server]
	[BaseEntity.RPC_Server.FromOwner]
	public void MoveItem(BaseEntity.RPCMessage msg)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		if (base.baseEntity.IsTransferring())
		{
			return;
		}
		ItemId val = msg.read.ItemID();
		ItemContainerId val2 = msg.read.ItemContainerID();
		int iTargetPos = msg.read.Int8();
		int num = (int)msg.read.UInt32();
		bool flag = msg.read.Bit();
		Item item = FindItemByUID(val);
		ItemId itemID;
		if (item == null)
		{
			BasePlayer player = msg.player;
			itemID = val;
			player.ChatMessage("Invalid item (" + ((object)(ItemId)(ref itemID)).ToString() + ")");
			return;
		}
		BaseEntity entityOwner = item.GetEntityOwner();
		if (!CanMoveItemsFrom(entityOwner, item))
		{
			msg.player.ChatMessage("Cannot move item!");
			return;
		}
		if (num <= 0)
		{
			num = item.amount;
		}
		num = Mathf.Clamp(num, 1, item.MaxStackable());
		if (msg.player.GetActiveItem() == item)
		{
			BasePlayer player2 = msg.player;
			itemID = default(ItemId);
			player2.UpdateActiveItem(itemID);
		}
		if (!((ItemContainerId)(ref val2)).IsValid)
		{
			BaseEntity baseEntity = entityOwner;
			if (loot.containers.Count > 0)
			{
				if ((Object)(object)entityOwner == (Object)(object)base.baseEntity)
				{
					if (!flag)
					{
						baseEntity = loot.entitySource;
					}
				}
				else
				{
					baseEntity = base.baseEntity;
				}
			}
			if (baseEntity is IIdealSlotEntity idealSlotEntity)
			{
				val2 = idealSlotEntity.GetIdealContainer(base.baseEntity, item, flag);
			}
			ItemContainer parent = item.parent;
			if (parent != null && parent.IsLocked())
			{
				msg.player.ChatMessage("Container is locked!");
				return;
			}
			if (!((ItemContainerId)(ref val2)).IsValid)
			{
				if ((Object)(object)baseEntity == (Object)(object)loot.entitySource)
				{
					foreach (ItemContainer container in loot.containers)
					{
						if (!container.PlayerItemInputBlocked() && !container.IsLocked() && item.MoveToContainer(container, -1, allowStack: true, ignoreStackLimit: false, base.baseEntity))
						{
							break;
						}
					}
					return;
				}
				if (!GiveItem(item, flag))
				{
					msg.player.ChatMessage("GiveItem failed!");
				}
				return;
			}
		}
		ItemContainer itemContainer = FindContainer(val2);
		if (itemContainer == null)
		{
			BasePlayer player3 = msg.player;
			ItemContainerId val3 = val2;
			player3.ChatMessage("Invalid container (" + ((object)(ItemContainerId)(ref val3)).ToString() + ")");
			return;
		}
		if (itemContainer.IsLocked())
		{
			msg.player.ChatMessage("Container is locked!");
			return;
		}
		if (itemContainer.PlayerItemInputBlocked())
		{
			msg.player.ChatMessage("Container does not accept player items!");
			return;
		}
		TimeWarning val4 = TimeWarning.New("Split", 0);
		try
		{
			if (item.amount > num)
			{
				int split_Amount = num;
				if (itemContainer.maxStackSize > 0)
				{
					split_Amount = Mathf.Min(num, itemContainer.maxStackSize);
				}
				Item item2 = item.SplitItem(split_Amount);
				if (!item2.MoveToContainer(itemContainer, iTargetPos, allowStack: true, ignoreStackLimit: false, base.baseEntity))
				{
					item.amount += item2.amount;
					item2.Remove();
				}
				ItemManager.DoRemoves();
				ServerUpdate(0f);
				return;
			}
		}
		finally
		{
			((IDisposable)val4)?.Dispose();
		}
		if (item.MoveToContainer(itemContainer, iTargetPos, allowStack: true, ignoreStackLimit: false, base.baseEntity))
		{
			ItemManager.DoRemoves();
			ServerUpdate(0f);
		}
	}

	private void OnClothingChanged(Item item, bool bAdded)
	{
		base.baseEntity.SV_ClothingChanged();
		ItemManager.DoRemoves();
		ServerUpdate(0f);
	}

	private void OnItemRemoved(Item item)
	{
		base.baseEntity.InvalidateNetworkCache();
	}

	private bool CanStoreInInventory(Item item, int targetSlot)
	{
		return true;
	}

	private bool CanEquipItem(Item item, int targetSlot)
	{
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		if ((item.info.flags & ItemDefinition.Flag.NotAllowedInBelt) != 0)
		{
			return false;
		}
		ItemModContainerRestriction component = ((Component)item.info).GetComponent<ItemModContainerRestriction>();
		if ((Object)(object)component == (Object)null)
		{
			return true;
		}
		Item[] array = containerBelt.itemList.ToArray();
		foreach (Item item2 in array)
		{
			if (item2 != item)
			{
				ItemModContainerRestriction component2 = ((Component)item2.info).GetComponent<ItemModContainerRestriction>();
				if (!((Object)(object)component2 == (Object)null) && !component.CanExistWith(component2) && !item2.MoveToContainer(containerMain))
				{
					item2.Drop(base.baseEntity.GetDropPosition(), base.baseEntity.GetDropVelocity());
				}
			}
		}
		return true;
	}

	private bool CanWearItem(Item item, int targetSlot)
	{
		return CanWearItem(item, canAdjustClothing: true, targetSlot);
	}

	private bool CanWearItem(Item item, bool canAdjustClothing, int targetSlot)
	{
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		ItemModWearable component = ((Component)item.info).GetComponent<ItemModWearable>();
		if ((Object)(object)component == (Object)null)
		{
			return false;
		}
		if (component.npcOnly && !Inventory.disableAttireLimitations)
		{
			BasePlayer basePlayer = base.baseEntity;
			if ((Object)(object)basePlayer != (Object)null && !basePlayer.IsNpc)
			{
				return false;
			}
		}
		bool flag = item.IsBackpack();
		if (flag && targetSlot != 7)
		{
			return false;
		}
		if (!flag && targetSlot == 7)
		{
			return false;
		}
		if (flag && !CanEquipBackpack())
		{
			base.baseEntity.ShowToast(GameTip.Styles.Red_Normal, BackpackGroundedError);
			return false;
		}
		Item[] array = containerWear.itemList.ToArray();
		foreach (Item item2 in array)
		{
			if (item2 == item)
			{
				continue;
			}
			ItemModWearable component2 = ((Component)item2.info).GetComponent<ItemModWearable>();
			if (!((Object)(object)component2 == (Object)null) && !Inventory.disableAttireLimitations && !component.CanExistWith(component2))
			{
				if (!canAdjustClothing)
				{
					return false;
				}
				bool flag2 = false;
				if (item.parent == containerBelt)
				{
					flag2 = item2.MoveToContainer(containerBelt);
				}
				if (!flag2 && !item2.MoveToContainer(containerMain))
				{
					item2.Drop(base.baseEntity.GetDropPosition(), base.baseEntity.GetDropVelocity());
				}
			}
		}
		return true;
	}

	public void ServerUpdate(float delta)
	{
		loot.Check();
		if (delta > 0f && !base.baseEntity.IsSleeping() && !base.baseEntity.IsTransferring())
		{
			crafting.ServerUpdate(delta);
		}
		float currentTemperature = base.baseEntity.currentTemperature;
		UpdateContainer(delta, Type.Main, containerMain, bSendInventoryToEveryone: false, currentTemperature);
		UpdateContainer(delta, Type.Belt, containerBelt, bSendInventoryToEveryone: true, currentTemperature);
		UpdateContainer(delta, Type.Wear, containerWear, bSendInventoryToEveryone: true, currentTemperature);
	}

	public void UpdateContainer(float delta, Type type, ItemContainer container, bool bSendInventoryToEveryone, float temperature)
	{
		if (container != null)
		{
			container.temperature = temperature;
			if (delta > 0f)
			{
				container.OnCycle(delta);
			}
			if (container.dirty)
			{
				SendUpdatedInventory(type, container, bSendInventoryToEveryone);
				base.baseEntity.InvalidateNetworkCache();
			}
		}
	}

	public void SendSnapshot()
	{
		TimeWarning val = TimeWarning.New("PlayerInventory.SendSnapshot", 0);
		try
		{
			SendUpdatedInventory(Type.Main, containerMain);
			SendUpdatedInventory(Type.Belt, containerBelt, bSendInventoryToEveryone: true);
			SendUpdatedInventory(Type.Wear, containerWear, bSendInventoryToEveryone: true);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void SendUpdatedInventory(Type type, ItemContainer container, bool bSendInventoryToEveryone = false)
	{
		UpdateItemContainer val = Pool.Get<UpdateItemContainer>();
		try
		{
			val.type = (int)type;
			if (container != null)
			{
				container.dirty = false;
				val.container = Pool.Get<List<ItemContainer>>();
				val.container.Add(container.Save());
			}
			if (bSendInventoryToEveryone)
			{
				base.baseEntity.ClientRPC<UpdateItemContainer>(null, "UpdatedItemContainer", val);
			}
			else
			{
				base.baseEntity.ClientRPCPlayer<UpdateItemContainer>(null, base.baseEntity, "UpdatedItemContainer", val);
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public Item FindItemByUID(ItemId id)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		if (!((ItemId)(ref id)).IsValid)
		{
			return null;
		}
		if (containerMain != null)
		{
			Item item = containerMain.FindItemByUID(id);
			if (item != null && item.IsValid())
			{
				return item;
			}
		}
		if (containerBelt != null)
		{
			Item item2 = containerBelt.FindItemByUID(id);
			if (item2 != null && item2.IsValid())
			{
				return item2;
			}
		}
		if (containerWear != null)
		{
			Item item3 = containerWear.FindItemByUID(id);
			if (item3 != null && item3.IsValid())
			{
				return item3;
			}
		}
		return loot.FindItem(id);
	}

	public Item FindItemByItemID(string itemName)
	{
		ItemDefinition itemDefinition = ItemManager.FindItemDefinition(itemName);
		if ((Object)(object)itemDefinition == (Object)null)
		{
			return null;
		}
		return FindItemByItemID(itemDefinition.itemid);
	}

	public Item FindItemByItemID(int id)
	{
		if (containerMain != null)
		{
			Item item = containerMain.FindItemByItemID(id);
			if (item != null && item.IsValid())
			{
				return item;
			}
		}
		if (containerBelt != null)
		{
			Item item2 = containerBelt.FindItemByItemID(id);
			if (item2 != null && item2.IsValid())
			{
				return item2;
			}
		}
		if (containerWear != null)
		{
			Item item3 = containerWear.FindItemByItemID(id);
			if (item3 != null && item3.IsValid())
			{
				return item3;
			}
		}
		return null;
	}

	public Item FindItemByItemName(string name)
	{
		if (containerMain != null)
		{
			Item item = containerMain.FindItemByItemName(name);
			if (item != null && item.IsValid())
			{
				return item;
			}
		}
		if (containerBelt != null)
		{
			Item item2 = containerBelt.FindItemByItemName(name);
			if (item2 != null && item2.IsValid())
			{
				return item2;
			}
		}
		if (containerWear != null)
		{
			Item item3 = containerWear.FindItemByItemName(name);
			if (item3 != null && item3.IsValid())
			{
				return item3;
			}
		}
		return null;
	}

	public Item FindBySubEntityID(NetworkableId subEntityID)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		if (containerMain != null)
		{
			Item item = containerMain.FindBySubEntityID(subEntityID);
			if (item != null && item.IsValid())
			{
				return item;
			}
		}
		if (containerBelt != null)
		{
			Item item2 = containerBelt.FindBySubEntityID(subEntityID);
			if (item2 != null && item2.IsValid())
			{
				return item2;
			}
		}
		if (containerWear != null)
		{
			Item item3 = containerWear.FindBySubEntityID(subEntityID);
			if (item3 != null && item3.IsValid())
			{
				return item3;
			}
		}
		return null;
	}

	public List<Item> FindItemsByItemID(int id)
	{
		List<Item> list = new List<Item>();
		if (containerMain != null)
		{
			list.AddRange(containerMain.FindItemsByItemID(id));
		}
		if (containerBelt != null)
		{
			list.AddRange(containerBelt.FindItemsByItemID(id));
		}
		if (containerWear != null)
		{
			list.AddRange(containerWear.FindItemsByItemID(id));
		}
		return list;
	}

	public ItemContainer FindContainer(ItemContainerId id)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		TimeWarning val = TimeWarning.New("FindContainer", 0);
		try
		{
			ItemContainer itemContainer = containerMain.FindContainer(id);
			if (itemContainer != null)
			{
				return itemContainer;
			}
			itemContainer = containerBelt.FindContainer(id);
			if (itemContainer != null)
			{
				return itemContainer;
			}
			itemContainer = containerWear.FindContainer(id);
			if (itemContainer != null)
			{
				return itemContainer;
			}
			return loot.FindContainer(id);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public ItemContainer GetContainer(Type id)
	{
		if (id == Type.Main)
		{
			return containerMain;
		}
		if (Type.Belt == id)
		{
			return containerBelt;
		}
		if (Type.Wear == id)
		{
			return containerWear;
		}
		return null;
	}

	public bool GiveItem(Item item, ItemContainer container = null)
	{
		return GiveItem(item, tryWearClothing: false, container);
	}

	public bool GiveItem(Item item, bool tryWearClothing, ItemContainer container = null)
	{
		if (item == null)
		{
			return false;
		}
		if (container == null)
		{
			GetIdealPickupContainer(item, ref container, tryWearClothing);
		}
		if (container != null && item.MoveToContainer(container))
		{
			return true;
		}
		if (item.MoveToContainer(containerMain))
		{
			return true;
		}
		if (item.MoveToContainer(containerBelt))
		{
			return true;
		}
		return false;
	}

	protected void GetIdealPickupContainer(Item item, ref ItemContainer container, bool tryWearClothing)
	{
		if (item.MaxStackable() > 1)
		{
			if (containerBelt != null && containerBelt.FindItemByItemID(item.info.itemid) != null)
			{
				container = containerBelt;
				return;
			}
			if (containerMain != null && containerMain.FindItemByItemID(item.info.itemid) != null)
			{
				container = containerMain;
				return;
			}
		}
		if (tryWearClothing && item.info.isWearable && CanWearItem(item, canAdjustClothing: false, item.IsBackpack() ? 7 : (-1)))
		{
			container = containerWear;
		}
		else if (item.info.isUsable && !item.info.HasFlag(ItemDefinition.Flag.NotStraightToBelt))
		{
			container = containerBelt;
		}
	}

	public void Strip()
	{
		containerMain.Clear();
		containerBelt.Clear();
		containerWear.Clear();
		ItemManager.DoRemoves();
	}

	public static bool IsBirthday()
	{
		if (forceBirthday)
		{
			return true;
		}
		if (Time.time < nextCheckTime)
		{
			return wasBirthday;
		}
		nextCheckTime = Time.time + 60f;
		DateTime now = DateTime.Now;
		wasBirthday = now.Day == 11 && now.Month == 12;
		return wasBirthday;
	}

	public static bool IsChristmas()
	{
		return XMas.enabled;
	}

	public void GiveDefaultItems()
	{
		Strip();
		BaseGameMode activeGameMode = BaseGameMode.GetActiveGameMode(serverside: true);
		if ((Object)(object)activeGameMode != (Object)null && activeGameMode.HasLoadouts())
		{
			BaseGameMode.GetActiveGameMode(serverside: true).LoadoutPlayer(base.baseEntity);
			return;
		}
		GiveDefaultItemWithSkin("client.rockskin", "rock");
		GiveDefaultItemWithSkin("client.torchskin", "torch");
		if (IsBirthday())
		{
			GiveItem(ItemManager.CreateByName("cakefiveyear", 1, 0uL), containerBelt);
			GiveItem(ItemManager.CreateByName("partyhat", 1, 0uL), containerWear);
		}
		if (IsChristmas())
		{
			GiveItem(ItemManager.CreateByName("snowball", 1, 0uL), containerBelt);
			GiveItem(ItemManager.CreateByName("snowball", 1, 0uL), containerBelt);
			GiveItem(ItemManager.CreateByName("snowball", 1, 0uL), containerBelt);
		}
		void GiveDefaultItemWithSkin(string convarSkinName, string itemShortName)
		{
			ulong num = 0uL;
			int infoInt = base.baseEntity.GetInfoInt(convarSkinName, 0);
			bool flag = false;
			bool flag2 = false;
			flag2 = base.baseEntity?.UnlockAllSkins ?? false;
			if (infoInt > 0 && (base.baseEntity.blueprints.CheckSkinOwnership(infoInt, base.baseEntity.userID) || flag2))
			{
				ItemDefinition itemDefinition = ItemManager.FindItemDefinition(itemShortName);
				if ((Object)(object)itemDefinition != (Object)null && ItemDefinition.FindSkin(itemDefinition.itemid, infoInt) != 0L)
				{
					IPlayerItemDefinition itemDefinition2 = PlatformService.Instance.GetItemDefinition(infoInt);
					if (itemDefinition2 != null)
					{
						num = itemDefinition2.WorkshopDownload;
					}
					if (num == 0L && itemDefinition.skins != null)
					{
						ItemSkinDirectory.Skin[] skins = itemDefinition.skins;
						for (int i = 0; i < skins.Length; i++)
						{
							ItemSkinDirectory.Skin skin = skins[i];
							if (skin.id == infoInt && (Object)(object)skin.invItem != (Object)null && skin.invItem is ItemSkin itemSkin && (Object)(object)itemSkin.Redirect != (Object)null)
							{
								GiveItem(ItemManager.CreateByName(itemSkin.Redirect.shortname, 1, 0uL), containerBelt);
								flag = true;
								break;
							}
						}
					}
				}
			}
			if (!flag)
			{
				GiveItem(ItemManager.CreateByName(itemShortName, 1, num), containerBelt);
			}
		}
	}

	private bool CanEquipBackpack()
	{
		if (ConVar.Server.canEquipBackpacksInAir || Parachute.BypassRepack)
		{
			return true;
		}
		if (base.baseEntity.WaterFactor() > 0.5f)
		{
			return true;
		}
		if (!base.baseEntity.IsOnGround())
		{
			return false;
		}
		if (base.baseEntity.isMounted && Object.op_Implicit((Object)(object)base.baseEntity.GetMounted()) && base.baseEntity.GetMounted().VehicleParent() is Parachute)
		{
			return false;
		}
		return true;
	}

	public PlayerInventory Save(bool bForDisk)
	{
		PlayerInventory val = Pool.Get<PlayerInventory>();
		if (bForDisk)
		{
			val.invMain = containerMain.Save();
		}
		val.invBelt = containerBelt.Save();
		val.invWear = containerWear.Save();
		return val;
	}

	public void Load(PlayerInventory msg)
	{
		if (msg.invMain != null)
		{
			containerMain.Load(msg.invMain);
		}
		if (msg.invBelt != null)
		{
			containerBelt.Load(msg.invBelt);
		}
		if (msg.invWear != null)
		{
			containerWear.Load(msg.invWear);
		}
		if (Object.op_Implicit((Object)(object)base.baseEntity) && base.baseEntity.isServer && containerWear.capacity == 7)
		{
			containerWear.capacity = 8;
		}
	}

	public int Take(List<Item> collect, int itemid, int amount)
	{
		int num = 0;
		if (containerMain != null)
		{
			int num2 = containerMain.Take(collect, itemid, amount);
			num += num2;
			amount -= num2;
		}
		if (amount <= 0)
		{
			return num;
		}
		if (containerBelt != null)
		{
			int num3 = containerBelt.Take(collect, itemid, amount);
			num += num3;
			amount -= num3;
		}
		if (amount <= 0)
		{
			return num;
		}
		if (containerWear != null)
		{
			int num4 = containerWear.Take(collect, itemid, amount);
			num += num4;
			amount -= num4;
		}
		return num;
	}

	public int GetAmount(int itemid)
	{
		if (itemid == 0)
		{
			return 0;
		}
		int num = 0;
		if (containerMain != null)
		{
			num += containerMain.GetAmount(itemid, onlyUsableAmounts: true);
		}
		if (containerBelt != null)
		{
			num += containerBelt.GetAmount(itemid, onlyUsableAmounts: true);
		}
		if (containerWear != null)
		{
			num += containerWear.GetAmount(itemid, onlyUsableAmounts: true);
		}
		return num;
	}

	public Item[] AllItems()
	{
		List<Item> list = new List<Item>();
		if (containerMain != null)
		{
			list.AddRange(containerMain.itemList);
		}
		if (containerBelt != null)
		{
			list.AddRange(containerBelt.itemList);
		}
		if (containerWear != null)
		{
			list.AddRange(containerWear.itemList);
		}
		return list.ToArray();
	}

	public int AllItemsNoAlloc(ref List<Item> items)
	{
		items.Clear();
		if (containerMain != null)
		{
			items.AddRange(containerMain.itemList);
		}
		if (containerBelt != null)
		{
			items.AddRange(containerBelt.itemList);
		}
		if (containerWear != null)
		{
			items.AddRange(containerWear.itemList);
		}
		return items.Count;
	}

	public void FindAmmo(List<Item> list, AmmoTypes ammoType)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (containerMain != null)
		{
			containerMain.FindAmmo(list, ammoType);
		}
		if (containerBelt != null)
		{
			containerBelt.FindAmmo(list, ammoType);
		}
	}

	public bool HasAmmo(AmmoTypes ammoType)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		if (!containerMain.HasAmmo(ammoType))
		{
			return containerBelt.HasAmmo(ammoType);
		}
		return true;
	}
}
