using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

public class EasterBasket : AttackEntity
{
	public GameObjectRef eggProjectile;

	public ItemDefinition ammoType;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("EasterBasket.OnRpcMessage", 0);
		try
		{
			if (rpc == 3763591455u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - ThrowEgg "));
				}
				TimeWarning val2 = TimeWarning.New("ThrowEgg", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsActiveItem.Test(3763591455u, "ThrowEgg", this, player))
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
							ThrowEgg(msg2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in ThrowEgg");
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

	public override Vector3 GetInheritedVelocity(BasePlayer player, Vector3 direction)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return player.GetInheritedProjectileVelocity(direction);
	}

	public Item GetAmmo()
	{
		BasePlayer ownerPlayer = GetOwnerPlayer();
		if (!Object.op_Implicit((Object)(object)ownerPlayer))
		{
			return null;
		}
		Item item = ownerPlayer.inventory.containerMain.FindItemByItemID(ammoType.itemid);
		if (item == null)
		{
			item = ownerPlayer.inventory.containerBelt.FindItemByItemID(ammoType.itemid);
		}
		return item;
	}

	public bool HasAmmo()
	{
		return GetAmmo() != null;
	}

	public void UseAmmo()
	{
		GetAmmo()?.UseItem();
	}

	[RPC_Server]
	[RPC_Server.IsActiveItem]
	public void ThrowEgg(RPCMessage msg)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		BasePlayer player = msg.player;
		if (!VerifyClientAttack(player))
		{
			SendNetworkUpdate();
		}
		else
		{
			if (!HasAmmo())
			{
				return;
			}
			UseAmmo();
			Vector3 val = msg.read.Vector3();
			Vector3 val2 = msg.read.Vector3();
			Vector3 val3 = ((Vector3)(ref val2)).normalized;
			bool num = msg.read.Bit();
			BaseEntity mounted = player.GetParentEntity();
			if ((Object)(object)mounted == (Object)null)
			{
				mounted = player.GetMounted();
			}
			if (num)
			{
				if ((Object)(object)mounted != (Object)null)
				{
					val = ((Component)mounted).transform.TransformPoint(val);
					val3 = ((Component)mounted).transform.TransformDirection(val3);
				}
				else
				{
					val = player.eyes.position;
					val3 = player.eyes.BodyForward();
				}
			}
			if (!ValidateEyePos(player, val))
			{
				return;
			}
			float num2 = 2f;
			if (num2 > 0f)
			{
				val3 = AimConeUtil.GetModifiedAimConeDirection(num2, val3);
			}
			float num3 = 1f;
			RaycastHit val4 = default(RaycastHit);
			if (Physics.Raycast(val, val3, ref val4, num3, 1237003025))
			{
				num3 = ((RaycastHit)(ref val4)).distance - 0.1f;
			}
			BaseEntity baseEntity = GameManager.server.CreateEntity(eggProjectile.resourcePath, val + val3 * num3);
			if (!((Object)(object)baseEntity == (Object)null))
			{
				baseEntity.creatorEntity = player;
				ServerProjectile component = ((Component)baseEntity).GetComponent<ServerProjectile>();
				if (Object.op_Implicit((Object)(object)component))
				{
					component.InitializeVelocity(GetInheritedVelocity(player, val3) + val3 * component.speed);
				}
				baseEntity.Spawn();
				GetOwnerItem()?.LoseCondition(Random.Range(1f, 2f));
			}
		}
	}
}
