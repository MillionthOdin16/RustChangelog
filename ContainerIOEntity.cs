using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

public class ContainerIOEntity : IOEntity, IItemContainerEntity, IIdealSlotEntity, ILootableEntity, LootPanel.IHasLootPanel, IContainerSounds
{
	public ItemDefinition onlyAllowedItem;

	public ItemContainer.ContentsType allowedContents = ItemContainer.ContentsType.Generic;

	public int maxStackSize = 1;

	public int numSlots;

	public string lootPanelName = "generic";

	public Phrase panelTitle = new Phrase("loot", "Loot");

	public bool needsBuildingPrivilegeToUse;

	public bool isLootable = true;

	public bool dropsLoot;

	public bool dropFloats;

	public bool onlyOneUser;

	public SoundDefinition openSound;

	public SoundDefinition closeSound;

	public Phrase LootPanelTitle => panelTitle;

	public ItemContainer inventory { get; private set; }

	public Transform Transform => ((Component)this).transform;

	public bool DropsLoot => dropsLoot;

	public bool DropFloats => dropFloats;

	public float DestroyLootPercent => 0f;

	public ulong LastLootedBy { get; set; }

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("ContainerIOEntity.OnRpcMessage", 0);
		try
		{
			if (rpc == 331989034 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - RPC_OpenLoot "));
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
						val3 = TimeWarning.New("Call", 0);
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
							((IDisposable)val3)?.Dispose();
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

	public override bool CanPickup(BasePlayer player)
	{
		if (!pickup.requireEmptyInv || inventory == null || inventory.itemList.Count == 0)
		{
			return base.CanPickup(player);
		}
		return false;
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

	public override void PreServerLoad()
	{
		base.PreServerLoad();
		CreateInventory(giveUID: false);
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

	public void CreateInventory(bool giveUID)
	{
		inventory = new ItemContainer();
		inventory.entityOwner = this;
		inventory.allowedContents = ((allowedContents == (ItemContainer.ContentsType)0) ? ItemContainer.ContentsType.Generic : allowedContents);
		inventory.SetOnlyAllowedItem(onlyAllowedItem);
		inventory.maxStackSize = maxStackSize;
		inventory.ServerInitialize(null, numSlots);
		if (giveUID)
		{
			inventory.GiveUID();
		}
		inventory.onItemAddedRemoved = OnItemAddedOrRemoved;
		inventory.onDirty += OnInventoryDirty;
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

	public virtual void OnInventoryFirstCreated(ItemContainer container)
	{
	}

	public virtual void OnItemAddedOrRemoved(Item item, bool added)
	{
	}

	protected virtual void OnInventoryDirty()
	{
	}

	public override void OnKilled(HitInfo info)
	{
		DropItems();
		base.OnKilled(info);
	}

	public void DropItems(BaseEntity initiator = null)
	{
		StorageContainer.DropItems(this, initiator);
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	private void RPC_OpenLoot(RPCMessage rpc)
	{
		if (inventory != null)
		{
			BasePlayer player = rpc.player;
			if (Object.op_Implicit((Object)(object)player) && player.CanInteract())
			{
				PlayerOpenLoot(player, lootPanelName);
			}
		}
	}

	public virtual bool PlayerOpenLoot(BasePlayer player, string panelToOpen = "", bool doPositionChecks = true)
	{
		if (needsBuildingPrivilegeToUse && !player.CanBuild())
		{
			return false;
		}
		if ((onlyOneUser && IsOpen()) || IsTransferring())
		{
			player.ShowToast(GameTip.Styles.Red_Normal, StorageContainer.LockedMessage);
			return false;
		}
		if (panelToOpen == "")
		{
			panelToOpen = lootPanelName;
		}
		if (player.inventory.loot.StartLootingEntity(this, doPositionChecks))
		{
			SetFlag(Flags.Open, b: true);
			player.inventory.loot.AddContainer(inventory);
			player.inventory.loot.SendImmediate();
			player.ClientRPCPlayer(null, player, "RPC_OpenLootPanel", lootPanelName);
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

	public bool ShouldDropItemsIndividually()
	{
		return false;
	}

	public virtual int GetIdealSlot(BasePlayer player, Item item)
	{
		return -1;
	}

	public virtual ItemContainerId GetIdealContainer(BasePlayer player, Item item, bool altMove)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		return default(ItemContainerId);
	}

	public virtual void DropBonusItems(BaseEntity initiator, ItemContainer container)
	{
	}

	public bool OccupiedCheck(BasePlayer player = null)
	{
		if ((Object)(object)player != (Object)null && (Object)(object)player.inventory.loot.entitySource == (Object)(object)this)
		{
			return true;
		}
		if (onlyOneUser)
		{
			return !IsOpen();
		}
		return true;
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (info.fromDisk && info.msg.storageBox != null)
		{
			if (inventory != null)
			{
				inventory.Load(info.msg.storageBox.contents);
				inventory.capacity = numSlots;
			}
			else
			{
				Debug.LogWarning((object)("Storage container without inventory: " + ((object)this).ToString()));
			}
		}
	}
}
