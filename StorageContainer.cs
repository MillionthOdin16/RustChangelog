using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

public class StorageContainer : DecayEntity, IItemContainerEntity, IIdealSlotEntity, ILootableEntity, LootPanel.IHasLootPanel, IContainerSounds, PlayerInventory.ICanMoveFrom
{
	[Header("Storage Container")]
	public static readonly Phrase LockedMessage = new Phrase("storage.locked", "Can't loot right now");

	public static readonly Phrase InUseMessage = new Phrase("storage.in_use", "Already in use");

	public int inventorySlots = 6;

	public bool dropsLoot = true;

	public float dropLootDestroyPercent = 0f;

	public bool dropFloats = false;

	public bool isLootable = true;

	public bool isLockable = true;

	public bool isMonitorable = false;

	public string panelName = "generic";

	public Phrase panelTitle = new Phrase("loot", "Loot");

	public ItemContainer.ContentsType allowedContents;

	public ItemDefinition allowedItem;

	public ItemDefinition allowedItem2;

	public int maxStackSize = 0;

	public bool needsBuildingPrivilegeToUse = false;

	public bool mustBeMountedToUse = false;

	public SoundDefinition openSound;

	public SoundDefinition closeSound;

	[Header("Item Dropping")]
	public Vector3 dropPosition;

	public Vector3 dropVelocity = Vector3.forward;

	public ItemCategory onlyAcceptCategory = ItemCategory.All;

	public bool onlyOneUser = false;

	public Phrase LootPanelTitle => panelTitle;

	public ItemContainer inventory { get; private set; }

	public Transform Transform => ((Component)this).transform;

	public bool DropsLoot => dropsLoot;

	public bool DropFloats => dropFloats;

	public float DestroyLootPercent => dropLootDestroyPercent;

	public ulong LastLootedBy { get; set; }

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("StorageContainer.OnRpcMessage", 0);
		try
		{
			if (rpc == 331989034 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_OpenLoot "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_OpenLoot", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(331989034u, "RPC_OpenLoot", this, player, 3f))
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
						TimeWarning val4 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage rpc2 = rPCMessage;
							RPC_OpenLoot(rpc2);
						}
						finally
						{
							((IDisposable)val4)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in RPC_OpenLoot");
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

	public override void ResetState()
	{
		base.ResetState();
		if (base.isServer && inventory != null)
		{
			inventory.Clear();
			inventory = null;
		}
	}

	public virtual void OnDrawGizmos()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		Gizmos.matrix = ((Component)this).transform.localToWorldMatrix;
		Gizmos.color = Color.yellow;
		Gizmos.DrawCube(dropPosition, Vector3.one * 0.1f);
		Gizmos.color = Color.white;
		Gizmos.DrawRay(dropPosition, dropVelocity);
	}

	public bool MoveAllInventoryItems(ItemContainer from)
	{
		return MoveAllInventoryItems(from, inventory);
	}

	public static bool MoveAllInventoryItems(ItemContainer source, ItemContainer dest)
	{
		bool flag = true;
		for (int i = 0; i < Mathf.Min(source.capacity, dest.capacity); i++)
		{
			Item slot = source.GetSlot(i);
			if (slot != null)
			{
				bool flag2 = slot.MoveToContainer(dest);
				if (flag && !flag2)
				{
					flag = false;
				}
			}
		}
		return flag;
	}

	public virtual void ReceiveInventoryFromItem(Item item)
	{
		if (item.contents != null)
		{
			MoveAllInventoryItems(item.contents, inventory);
		}
	}

	public override bool CanPickup(BasePlayer player)
	{
		bool flag = (Object)(object)GetSlot(Slot.Lock) != (Object)null;
		if (base.isClient)
		{
			return base.CanPickup(player) && !flag;
		}
		return (!pickup.requireEmptyInv || inventory == null || inventory.itemList.Count == 0) && base.CanPickup(player) && !flag;
	}

	public override void OnPickedUp(Item createdItem, BasePlayer player)
	{
		base.OnPickedUp(createdItem, player);
		if (createdItem != null && createdItem.contents != null)
		{
			MoveAllInventoryItems(inventory, createdItem.contents);
			return;
		}
		for (int i = 0; i < inventory.capacity; i++)
		{
			Item slot = inventory.GetSlot(i);
			if (slot != null)
			{
				slot.RemoveFromContainer();
				player.GiveItem(slot, GiveItemReason.PickedUp);
			}
		}
	}

	public override void ServerInit()
	{
		if (inventory == null)
		{
			CreateInventory(giveUID: true);
			OnInventoryFirstCreated(inventory);
		}
		base.ServerInit();
	}

	public virtual void OnInventoryFirstCreated(ItemContainer container)
	{
	}

	public virtual void OnItemAddedOrRemoved(Item item, bool added)
	{
	}

	public virtual bool ItemFilter(Item item, int targetSlot)
	{
		if (onlyAcceptCategory == ItemCategory.All)
		{
			return true;
		}
		return item.info.category == onlyAcceptCategory;
	}

	public void CreateInventory(bool giveUID)
	{
		inventory = new ItemContainer();
		inventory.entityOwner = this;
		inventory.allowedContents = ((allowedContents == (ItemContainer.ContentsType)0) ? ItemContainer.ContentsType.Generic : allowedContents);
		inventory.SetOnlyAllowedItems(allowedItem, allowedItem2);
		inventory.maxStackSize = maxStackSize;
		inventory.ServerInitialize(null, inventorySlots);
		if (giveUID)
		{
			inventory.GiveUID();
		}
		inventory.onDirty += OnInventoryDirty;
		inventory.onItemAddedRemoved = OnItemAddedOrRemoved;
		inventory.canAcceptItem = ItemFilter;
	}

	public override void PreServerLoad()
	{
		base.PreServerLoad();
		CreateInventory(giveUID: false);
	}

	protected virtual void OnInventoryDirty()
	{
		InvalidateNetworkCache();
	}

	public override void PostServerLoad()
	{
		base.PostServerLoad();
		if (inventory != null && !((ItemContainerId)(ref inventory.uid)).IsValid)
		{
			inventory.GiveUID();
		}
		SetFlag(Flags.Open, b: false);
	}

	internal override void DoServerDestroy()
	{
		base.DoServerDestroy();
		if (inventory != null)
		{
			inventory.Kill();
			inventory = null;
		}
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	private void RPC_OpenLoot(RPCMessage rpc)
	{
		if (isLootable)
		{
			BasePlayer player = rpc.player;
			if (Object.op_Implicit((Object)(object)player) && player.CanInteract())
			{
				PlayerOpenLoot(player);
			}
		}
	}

	public virtual string GetPanelName()
	{
		return panelName;
	}

	public virtual bool CanMoveFrom(BasePlayer player, Item item)
	{
		return !inventory.IsLocked();
	}

	public virtual bool CanOpenLootPanel(BasePlayer player, string panelName)
	{
		if (!CanBeLooted(player))
		{
			return false;
		}
		BaseLock baseLock = GetSlot(Slot.Lock) as BaseLock;
		if ((Object)(object)baseLock != (Object)null && !baseLock.OnTryToOpen(player))
		{
			player.ChatMessage("It is locked...");
			return false;
		}
		return true;
	}

	public override bool CanBeLooted(BasePlayer player)
	{
		if (needsBuildingPrivilegeToUse && !player.CanBuild())
		{
			return false;
		}
		if (mustBeMountedToUse && !player.isMounted)
		{
			return false;
		}
		return base.CanBeLooted(player);
	}

	public virtual void AddContainers(PlayerLoot loot)
	{
		loot.AddContainer(inventory);
	}

	public virtual bool PlayerOpenLoot(BasePlayer player, string panelToOpen = "", bool doPositionChecks = true)
	{
		if (IsLocked())
		{
			player.ShowToast(GameTip.Styles.Red_Normal, LockedMessage);
			return false;
		}
		if (onlyOneUser && IsOpen())
		{
			player.ShowToast(GameTip.Styles.Red_Normal, InUseMessage);
			return false;
		}
		if (panelToOpen == "")
		{
			panelToOpen = panelName;
		}
		if (!CanOpenLootPanel(player, panelToOpen))
		{
			return false;
		}
		if (player.inventory.loot.StartLootingEntity(this, doPositionChecks))
		{
			SetFlag(Flags.Open, b: true);
			AddContainers(player.inventory.loot);
			player.inventory.loot.SendImmediate();
			player.ClientRPCPlayer(null, player, "RPC_OpenLootPanel", panelToOpen);
			SendNetworkUpdate();
			return true;
		}
		return false;
	}

	public virtual void PlayerStoppedLooting(BasePlayer player)
	{
		SetFlag(Flags.Open, b: false);
		SendNetworkUpdate();
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		if (info.forDisk)
		{
			if (inventory != null)
			{
				info.msg.storageBox = Pool.Get<StorageBox>();
				info.msg.storageBox.contents = inventory.Save();
			}
			else
			{
				Debug.LogWarning((object)("Storage container without inventory: " + ((object)this).ToString()));
			}
		}
	}

	public override void OnKilled(HitInfo info)
	{
		DropItems(info?.Initiator);
		base.OnKilled(info);
	}

	public void DropItems(BaseEntity initiator = null)
	{
		DropItems(this, initiator);
	}

	public static void DropItems(IItemContainerEntity containerEntity, BaseEntity initiator = null)
	{
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		ItemContainer itemContainer = containerEntity.inventory;
		if (itemContainer == null || itemContainer.itemList == null || itemContainer.itemList.Count == 0 || !containerEntity.DropsLoot)
		{
			return;
		}
		if (containerEntity.ShouldDropItemsIndividually() || (itemContainer.itemList.Count == 1 && !containerEntity.DropFloats))
		{
			if ((Object)(object)initiator != (Object)null)
			{
				containerEntity.DropBonusItems(initiator, itemContainer);
			}
			DropUtil.DropItems(itemContainer, containerEntity.GetDropPosition());
			return;
		}
		string prefab = (containerEntity.DropFloats ? "assets/prefabs/misc/item drop/item_drop_buoyant.prefab" : "assets/prefabs/misc/item drop/item_drop.prefab");
		DroppedItemContainer droppedItemContainer = itemContainer.Drop(prefab, containerEntity.GetDropPosition(), containerEntity.Transform.rotation, containerEntity.DestroyLootPercent);
		if (!((Object)(object)droppedItemContainer != (Object)null))
		{
		}
	}

	public virtual void DropBonusItems(BaseEntity initiator, ItemContainer container)
	{
	}

	public override Vector3 GetDropPosition()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		Matrix4x4 localToWorldMatrix = ((Component)this).transform.localToWorldMatrix;
		return ((Matrix4x4)(ref localToWorldMatrix)).MultiplyPoint(dropPosition);
	}

	public override Vector3 GetDropVelocity()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		Vector3 inheritedDropVelocity = GetInheritedDropVelocity();
		Matrix4x4 localToWorldMatrix = ((Component)this).transform.localToWorldMatrix;
		return inheritedDropVelocity + ((Matrix4x4)(ref localToWorldMatrix)).MultiplyVector(dropPosition);
	}

	public virtual bool ShouldDropItemsIndividually()
	{
		return false;
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (info.msg.storageBox != null)
		{
			if (inventory != null)
			{
				inventory.Load(info.msg.storageBox.contents);
				inventory.capacity = inventorySlots;
			}
			else
			{
				Debug.LogWarning((object)("Storage container without inventory: " + ((object)this).ToString()));
			}
		}
	}

	public virtual int GetIdealSlot(BasePlayer player, Item item)
	{
		return -1;
	}

	public virtual ItemContainerId GetIdealContainer(BasePlayer player, Item item, bool altMove)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		return default(ItemContainerId);
	}

	public override bool HasSlot(Slot slot)
	{
		if (isLockable && slot == Slot.Lock)
		{
			return true;
		}
		if (isMonitorable && slot == Slot.StorageMonitor)
		{
			return true;
		}
		return base.HasSlot(slot);
	}

	public bool OccupiedCheck(BasePlayer player = null)
	{
		if ((Object)(object)player != (Object)null && (Object)(object)player.inventory.loot.entitySource == (Object)(object)this)
		{
			return true;
		}
		return !onlyOneUser || !IsOpen();
	}

	protected bool HasAttachedStorageAdaptor()
	{
		if (children == null)
		{
			return false;
		}
		foreach (BaseEntity child in children)
		{
			if (child is IndustrialStorageAdaptor)
			{
				return true;
			}
		}
		return false;
	}
}
