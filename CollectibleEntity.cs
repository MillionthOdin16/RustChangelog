using System;
using ConVar;
using Facepunch.Rust;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

public class CollectibleEntity : BaseEntity, IPrefabPreProcess
{
	public Phrase itemName;

	public ItemAmount[] itemList;

	public GameObjectRef pickupEffect;

	public float xpScale = 1f;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("CollectibleEntity.OnRpcMessage", 0);
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
						if (!RPC_Server.MaxDistance.Test(2778075470u, "Pickup", this, player, 3f))
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
			if (rpc == 3528769075u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - PickupEat "));
				}
				TimeWarning val2 = TimeWarning.New("PickupEat", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.MaxDistance.Test(3528769075u, "PickupEat", this, player, 3f))
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
							PickupEat(msg3);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in PickupEat");
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

	public bool IsFood(bool checkConsumeMod = false)
	{
		for (int i = 0; i < itemList.Length; i++)
		{
			if (itemList[i].itemDef.category == ItemCategory.Food && (!checkConsumeMod || (Object)(object)((Component)itemList[i].itemDef).GetComponent<ItemModConsume>() != (Object)null))
			{
				return true;
			}
		}
		return false;
	}

	public void DoPickup(BasePlayer reciever, bool eat = false)
	{
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		if (itemList == null)
		{
			return;
		}
		ItemAmount[] array = itemList;
		foreach (ItemAmount itemAmount in array)
		{
			Item item = ItemManager.Create(itemAmount.itemDef, (int)itemAmount.amount, 0uL);
			if (item == null)
			{
				continue;
			}
			if (eat && item.info.category == ItemCategory.Food && (Object)(object)reciever != (Object)null)
			{
				ItemModConsume component = ((Component)item.info).GetComponent<ItemModConsume>();
				if ((Object)(object)component != (Object)null)
				{
					component.DoAction(item, reciever);
					continue;
				}
			}
			if (Object.op_Implicit((Object)(object)reciever))
			{
				Analytics.Azure.OnGatherItem(item.info.shortname, item.amount, this, reciever);
				reciever.GiveItem(item, GiveItemReason.ResourceHarvested);
			}
			else
			{
				item.Drop(((Component)this).transform.position + Vector3.up * 0.5f, Vector3.up);
			}
		}
		itemList = null;
		if (pickupEffect.isValid)
		{
			Effect.server.Run(pickupEffect.resourcePath, ((Component)this).transform.position, ((Component)this).transform.up);
		}
		RandomItemDispenser randomItemDispenser = PrefabAttribute.server.Find<RandomItemDispenser>(prefabID);
		if (randomItemDispenser != null)
		{
			randomItemDispenser.DistributeItems(reciever, ((Component)this).transform.position);
		}
		Kill();
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	public void Pickup(RPCMessage msg)
	{
		if (msg.player.CanInteract())
		{
			DoPickup(msg.player);
		}
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	public void PickupEat(RPCMessage msg)
	{
		if (msg.player.CanInteract())
		{
			DoPickup(msg.player, eat: true);
		}
	}

	public override void PreProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		base.PreProcess(preProcess, rootObj, name, serverside, clientside, bundling);
		if (serverside)
		{
			preProcess.RemoveComponent((Component)(object)((Component)this).GetComponent<Collider>());
		}
	}
}
