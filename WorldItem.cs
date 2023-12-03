using System;
using ConVar;
using Facepunch;
using Facepunch.Rust;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

public class WorldItem : BaseEntity, PlayerInventory.ICanMoveFrom
{
	[Header("WorldItem")]
	public bool allowPickup = true;

	[NonSerialized]
	public Item item;

	private bool _isInvokingSendItemUpdate;

	protected float eatSeconds = 10f;

	protected float caloriesPerSecond = 1f;

	public override TraitFlag Traits
	{
		get
		{
			if (item != null)
			{
				return item.Traits;
			}
			return base.Traits;
		}
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("WorldItem.OnRpcMessage", 0);
		try
		{
			if (rpc == 2778075470u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - Pickup "));
				}
				TimeWarning val2 = TimeWarning.New("Pickup", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(2778075470u, "Pickup", this, player, 3f))
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
							Pickup(msg2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in Pickup");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
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
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
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

	public override Item GetItem()
	{
		return item;
	}

	public void InitializeItem(Item in_item)
	{
		if (item != null)
		{
			RemoveItem();
		}
		item = in_item;
		if (item != null)
		{
			item.OnDirty += OnItemDirty;
			((Object)this).name = item.info.shortname + " (world)";
			item.SetWorldEntity(this);
			OnItemDirty(item);
		}
	}

	public void RemoveItem()
	{
		if (item != null)
		{
			item.OnDirty -= OnItemDirty;
			item = null;
		}
	}

	public void DestroyItem()
	{
		if (item != null)
		{
			item.OnDirty -= OnItemDirty;
			item.Remove();
			item = null;
		}
	}

	protected virtual void OnItemDirty(Item in_item)
	{
		Assert.IsTrue(item == in_item, "WorldItem:OnItemDirty - dirty item isn't ours!");
		if (item != null)
		{
			((Component)this).BroadcastMessage("OnItemChanged", (object)item, (SendMessageOptions)1);
		}
		DoItemNetworking();
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (info.msg.worldItem != null && info.msg.worldItem.item != null)
		{
			Item item = ItemManager.Load(info.msg.worldItem.item, this.item, base.isServer);
			if (item != null)
			{
				InitializeItem(item);
			}
		}
	}

	public override string ToString()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		if (_name == null)
		{
			if (base.isServer)
			{
				_name = string.Format("{1}[{0}] {2}", (object)(NetworkableId)(((_003F?)net?.ID) ?? default(NetworkableId)), base.ShortPrefabName, this.IsUnityNull() ? "NULL" : ((Object)this).name);
			}
			else
			{
				_name = base.ShortPrefabName;
			}
		}
		return _name;
	}

	public bool CanMoveFrom(BasePlayer player, Item item)
	{
		if ((Object)(object)((item != null) ? ((Component)item.info).GetComponent<ItemModBackpack>() : null) == (Object)null)
		{
			return true;
		}
		return item.parentItem?.parent == player.inventory.containerWear;
	}

	public override void ServerInit()
	{
		base.ServerInit();
		if (item != null)
		{
			((Component)this).BroadcastMessage("OnItemChanged", (object)item, (SendMessageOptions)1);
		}
	}

	private void DoItemNetworking()
	{
		if (!_isInvokingSendItemUpdate)
		{
			_isInvokingSendItemUpdate = true;
			((FacepunchBehaviour)this).Invoke((Action)SendItemUpdate, 0.1f);
		}
	}

	private void SendItemUpdate()
	{
		_isInvokingSendItemUpdate = false;
		if (item == null)
		{
			return;
		}
		UpdateItem val = Pool.Get<UpdateItem>();
		try
		{
			val.item = item.Save(bIncludeContainer: false, bIncludeOwners: false);
			ClientRPC<UpdateItem>(null, "UpdateItem", val);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	public void Pickup(RPCMessage msg)
	{
		if (msg.player.CanInteract() && this.item != null && allowPickup)
		{
			ClientRPC(null, "PickupSound");
			Item item = this.item;
			Analytics.Azure.OnItemPickup(msg.player, this);
			RemoveItem();
			msg.player.GiveItem(item, GiveItemReason.PickedUp);
			msg.player.SignalBroadcast(Signal.Gesture, "pickup_item");
		}
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		if (item != null)
		{
			bool forDisk = info.forDisk;
			info.msg.worldItem = Pool.Get<WorldItem>();
			info.msg.worldItem.item = item.Save(forDisk, bIncludeOwners: false);
		}
	}

	internal override void DoServerDestroy()
	{
		base.DoServerDestroy();
		DestroyItem();
	}

	public override void SwitchParent(BaseEntity ent)
	{
		SetParent(ent, parentBone);
	}

	public override void Eat(BaseNpc baseNpc, float timeSpent)
	{
		if (!(eatSeconds <= 0f))
		{
			eatSeconds -= timeSpent;
			baseNpc.AddCalories(caloriesPerSecond * timeSpent);
			if (eatSeconds < 0f)
			{
				DestroyItem();
				Kill();
			}
		}
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	private void RPC_OpenLoot(RPCMessage rpc)
	{
		if (item == null || item.contents == null)
		{
			return;
		}
		ItemModContainer component = ((Component)item.info).GetComponent<ItemModContainer>();
		if (!((Object)(object)component == (Object)null) && component.canLootInWorld)
		{
			BasePlayer player = rpc.player;
			if (Object.op_Implicit((Object)(object)player) && player.CanInteract() && player.inventory.loot.StartLootingEntity(this))
			{
				SetFlag(Flags.Open, b: true);
				player.inventory.loot.AddContainer(item.contents);
				player.inventory.loot.SendImmediate();
				player.ClientRPCPlayer(null, player, "RPC_OpenLootPanel", "generic_resizable");
				SendNetworkUpdate();
			}
		}
	}
}
