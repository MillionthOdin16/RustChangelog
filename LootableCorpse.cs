using System;
using ConVar;
using Facepunch;
using Facepunch.Rust;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Profiling;

public class LootableCorpse : BaseCorpse, LootPanel.IHasLootPanel
{
	public string lootPanelName = "generic";

	[NonSerialized]
	public ulong playerSteamID;

	[NonSerialized]
	public string _playerName;

	[NonSerialized]
	public ItemContainer[] containers;

	[NonSerialized]
	private bool firstLooted;

	public virtual string playerName
	{
		get
		{
			return NameHelper.Get(playerSteamID, _playerName, base.isClient);
		}
		set
		{
			_playerName = value;
		}
	}

	public virtual string streamerName { get; set; }

	public Phrase LootPanelTitle => Phrase.op_Implicit(playerName);

	public Phrase LootPanelName => Phrase.op_Implicit("N/A");

	public bool blockBagDrop { get; set; }

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("LootableCorpse.OnRpcMessage", 0);
		try
		{
			if (rpc == 2278459738u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_LootCorpse "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_LootCorpse", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(2278459738u, "RPC_LootCorpse", this, player, 3f))
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
							RPC_LootCorpse(rpc2);
						}
						finally
						{
							((IDisposable)val4)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in RPC_LootCorpse");
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
		firstLooted = false;
		base.ResetState();
	}

	public override void ServerInit()
	{
		base.ServerInit();
	}

	internal override void DoServerDestroy()
	{
		base.DoServerDestroy();
		if (!blockBagDrop)
		{
			PreDropItems();
			DropItems();
		}
		blockBagDrop = false;
		if (containers != null)
		{
			ItemContainer[] array = containers;
			foreach (ItemContainer itemContainer in array)
			{
				itemContainer.Kill();
			}
		}
		containers = null;
	}

	public void TakeFrom(params ItemContainer[] source)
	{
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		Assert.IsTrue(containers == null, "Initializing Twice");
		TimeWarning val = TimeWarning.New("Corpse.TakeFrom", 0);
		try
		{
			containers = new ItemContainer[source.Length];
			for (int i = 0; i < source.Length; i++)
			{
				containers[i] = new ItemContainer();
				containers[i].ServerInitialize(null, source[i].capacity);
				containers[i].GiveUID();
				containers[i].entityOwner = this;
				Profiler.BeginSample("LootableCorpse.TakeFromIter");
				Item[] array = source[i].itemList.ToArray();
				foreach (Item item in array)
				{
					if (!item.MoveToContainer(containers[i]))
					{
						item.DropAndTossUpwards(((Component)this).transform.position);
					}
				}
				Profiler.EndSample();
			}
			ResetRemovalTime();
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public override bool CanRemove()
	{
		return !IsOpen();
	}

	public virtual bool CanLoot()
	{
		return true;
	}

	public override bool OnStartBeingLooted(BasePlayer baseEntity)
	{
		if (!firstLooted)
		{
			if (playerSteamID <= 10000000)
			{
				Analytics.Azure.OnFirstLooted(this, baseEntity);
			}
			firstLooted = true;
		}
		return base.OnStartBeingLooted(baseEntity);
	}

	protected virtual bool CanLootContainer(ItemContainer c, int index)
	{
		return true;
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	private void RPC_LootCorpse(RPCMessage rpc)
	{
		BasePlayer player = rpc.player;
		if (!Object.op_Implicit((Object)(object)player) || !player.CanInteract() || !CanLoot() || containers == null || !player.inventory.loot.StartLootingEntity(this))
		{
			return;
		}
		SetFlag(Flags.Open, b: true);
		for (int i = 0; i < containers.Length; i++)
		{
			ItemContainer itemContainer = containers[i];
			if (CanLootContainer(itemContainer, i))
			{
				player.inventory.loot.AddContainer(itemContainer);
			}
		}
		player.inventory.loot.SendImmediate();
		ClientRPCPlayer(null, player, "RPC_ClientLootCorpse");
		SendNetworkUpdate();
	}

	public void PlayerStoppedLooting(BasePlayer player)
	{
		ResetRemovalTime();
		SetFlag(Flags.Open, b: false);
		SendNetworkUpdate();
	}

	protected virtual void PreDropItems()
	{
	}

	public void DropItems()
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		if (!Global.disableBagDropping && containers != null)
		{
			DroppedItemContainer droppedItemContainer = ItemContainer.Drop("assets/prefabs/misc/item drop/item_drop_backpack.prefab", ((Component)this).transform.position, Quaternion.identity, containers);
			if ((Object)(object)droppedItemContainer != (Object)null)
			{
				droppedItemContainer.playerName = playerName;
				droppedItemContainer.playerSteamID = playerSteamID;
			}
		}
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		info.msg.lootableCorpse = Pool.Get<LootableCorpse>();
		info.msg.lootableCorpse.playerName = playerName;
		info.msg.lootableCorpse.playerID = playerSteamID;
		info.msg.lootableCorpse.streamerName = streamerName;
		if (!info.forDisk || containers == null)
		{
			return;
		}
		info.msg.lootableCorpse.privateData = Pool.Get<Private>();
		info.msg.lootableCorpse.privateData.container = Pool.GetList<ItemContainer>();
		ItemContainer[] array = containers;
		foreach (ItemContainer itemContainer in array)
		{
			if (itemContainer != null)
			{
				ItemContainer val = itemContainer.Save();
				if (val != null)
				{
					info.msg.lootableCorpse.privateData.container.Add(val);
				}
			}
		}
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (info.msg.lootableCorpse == null)
		{
			return;
		}
		playerName = info.msg.lootableCorpse.playerName;
		streamerName = info.msg.lootableCorpse.streamerName;
		playerSteamID = info.msg.lootableCorpse.playerID;
		if (info.fromDisk && info.msg.lootableCorpse.privateData != null && info.msg.lootableCorpse.privateData.container != null)
		{
			int count = info.msg.lootableCorpse.privateData.container.Count;
			containers = new ItemContainer[count];
			for (int i = 0; i < count; i++)
			{
				containers[i] = new ItemContainer();
				containers[i].ServerInitialize(null, info.msg.lootableCorpse.privateData.container[i].slots);
				containers[i].GiveUID();
				containers[i].entityOwner = this;
				containers[i].Load(info.msg.lootableCorpse.privateData.container[i]);
			}
		}
	}
}
