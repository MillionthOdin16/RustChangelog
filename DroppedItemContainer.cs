using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

public class DroppedItemContainer : BaseCombatEntity, LootPanel.IHasLootPanel, IContainerSounds, ILootableEntity
{
	public string lootPanelName = "generic";

	public int maxItemCount = 36;

	[NonSerialized]
	public ulong playerSteamID;

	[NonSerialized]
	public string _playerName;

	public bool ItemBasedDespawn;

	public bool onlyOwnerLoot;

	public SoundDefinition openSound;

	public SoundDefinition closeSound;

	public ItemContainer inventory;

	public Phrase LootPanelTitle => Phrase.op_Implicit(playerName);

	public string playerName
	{
		get
		{
			if (playerSteamID == 0L)
			{
				return "";
			}
			return NameHelper.Get(playerSteamID, _playerName, base.isClient);
		}
		set
		{
			_playerName = value;
		}
	}

	public ulong LastLootedBy { get; set; }

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("DroppedItemContainer.OnRpcMessage", 0);
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

	public override bool OnStartBeingLooted(BasePlayer baseEntity)
	{
		if ((baseEntity.InSafeZone() || InSafeZone()) && baseEntity.userID != playerSteamID)
		{
			return false;
		}
		if (onlyOwnerLoot && baseEntity.userID != playerSteamID)
		{
			return false;
		}
		return base.OnStartBeingLooted(baseEntity);
	}

	public override void ServerInit()
	{
		ResetRemovalTime();
		base.ServerInit();
	}

	public void RemoveMe()
	{
		if (IsOpen())
		{
			ResetRemovalTime();
		}
		else
		{
			Kill();
		}
	}

	public void ResetRemovalTime(float dur)
	{
		TimeWarning val = TimeWarning.New("ResetRemovalTime", 0);
		try
		{
			((FacepunchBehaviour)this).Invoke((Action)RemoveMe, dur);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void ResetRemovalTime()
	{
		ResetRemovalTime(CalculateRemovalTime());
	}

	public float CalculateRemovalTime()
	{
		if (!ItemBasedDespawn)
		{
			return Server.itemdespawn * 16f * Server.itemdespawn_container_scale;
		}
		float num = Server.itemdespawn_quick;
		if (inventory != null)
		{
			foreach (Item item in inventory.itemList)
			{
				num = Mathf.Max(num, item.GetDespawnDuration());
			}
		}
		return num;
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

	public void TakeFrom(ItemContainer[] source, float destroyPercent = 0f)
	{
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		Assert.IsTrue(inventory == null, "Initializing Twice");
		TimeWarning val = TimeWarning.New("DroppedItemContainer.TakeFrom", 0);
		try
		{
			int num = 0;
			ItemContainer[] array = source;
			foreach (ItemContainer itemContainer in array)
			{
				num += itemContainer.itemList.Count;
			}
			inventory = new ItemContainer();
			inventory.ServerInitialize(null, Mathf.Min(num, maxItemCount));
			inventory.GiveUID();
			inventory.entityOwner = this;
			inventory.SetFlag(ItemContainer.Flag.NoItemInput, b: true);
			List<Item> list = Pool.GetList<Item>();
			array = source;
			for (int i = 0; i < array.Length; i++)
			{
				Item[] array2 = array[i].itemList.ToArray();
				foreach (Item item in array2)
				{
					if (destroyPercent > 0f)
					{
						if (item.amount == 1)
						{
							list.Add(item);
							continue;
						}
						item.amount = Mathf.CeilToInt((float)item.amount * (1f - destroyPercent));
					}
					if (!item.MoveToContainer(inventory))
					{
						item.DropAndTossUpwards(((Component)this).transform.position);
					}
				}
			}
			if (list.Count > 0)
			{
				int num2 = Mathf.FloorToInt((float)list.Count * destroyPercent);
				int num3 = Mathf.Max(0, list.Count - num2);
				ListEx.Shuffle<Item>(list, (uint)Random.Range(0, int.MaxValue));
				for (int k = 0; k < num3; k++)
				{
					Item item2 = list[k];
					if (!item2.MoveToContainer(inventory))
					{
						item2.DropAndTossUpwards(((Component)this).transform.position);
					}
				}
			}
			Pool.FreeList<Item>(ref list);
			ResetRemovalTime();
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	private void RPC_OpenLoot(RPCMessage rpc)
	{
		if (inventory != null)
		{
			BasePlayer player = rpc.player;
			if (Object.op_Implicit((Object)(object)player) && player.CanInteract() && player.inventory.loot.StartLootingEntity(this))
			{
				SetFlag(Flags.Open, b: true);
				player.inventory.loot.AddContainer(inventory);
				player.inventory.loot.SendImmediate();
				player.ClientRPCPlayer(null, player, "RPC_OpenLootPanel", lootPanelName);
				SendNetworkUpdate();
			}
		}
	}

	public void PlayerStoppedLooting(BasePlayer player)
	{
		if (inventory == null || inventory.itemList == null || inventory.itemList.Count == 0)
		{
			Kill();
			return;
		}
		ResetRemovalTime();
		SetFlag(Flags.Open, b: false);
		SendNetworkUpdate();
	}

	public override void PreServerLoad()
	{
		base.PreServerLoad();
		inventory = new ItemContainer();
		inventory.entityOwner = this;
		inventory.ServerInitialize(null, 0);
		inventory.SetFlag(ItemContainer.Flag.NoItemInput, b: true);
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		info.msg.lootableCorpse = Pool.Get<LootableCorpse>();
		info.msg.lootableCorpse.playerName = playerName;
		info.msg.lootableCorpse.playerID = playerSteamID;
		if (info.forDisk)
		{
			if (inventory != null)
			{
				info.msg.storageBox = Pool.Get<StorageBox>();
				info.msg.storageBox.contents = inventory.Save();
			}
			else
			{
				Debug.LogWarning((object)("Dropped item container without inventory: " + ((object)this).ToString()));
			}
		}
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (info.msg.lootableCorpse != null)
		{
			playerName = info.msg.lootableCorpse.playerName;
			playerSteamID = info.msg.lootableCorpse.playerID;
		}
		if (info.msg.storageBox != null)
		{
			if (inventory != null)
			{
				inventory.Load(info.msg.storageBox.contents);
			}
			else
			{
				Debug.LogWarning((object)("Dropped item container without inventory: " + ((object)this).ToString()));
			}
		}
	}
}
