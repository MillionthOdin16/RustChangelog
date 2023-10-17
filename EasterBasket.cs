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
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - ThrowEgg "));
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
						TimeWarning val4 = TimeWarning.New("Call", 0);
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
							((IDisposable)val4)?.Dispose();
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
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
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
			bool flag = msg.read.Bit();
			BaseEntity mounted = player.GetParentEntity();
			if ((Object)(object)mounted == (Object)null)
			{
				mounted = player.GetMounted();
			}
			if (flag)
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
			float num = 2f;
			if (num > 0f)
			{
				val3 = AimConeUtil.GetModifiedAimConeDirection(num, val3);
			}
			float num2 = 1f;
			RaycastHit val4 = default(RaycastHit);
			if (Physics.Raycast(val, val3, ref val4, num2, 1237003025))
			{
				num2 = ((RaycastHit)(ref val4)).distance - 0.1f;
			}
			BaseEntity baseEntity = GameManager.server.CreateEntity(eggProjectile.resourcePath, val + val3 * num2);
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
