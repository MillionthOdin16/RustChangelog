using System;
using ConVar;
using Facepunch.Rust;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

public class BaseLauncher : BaseProjectile
{
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("BaseLauncher.OnRpcMessage", 0);
		try
		{
			if (rpc == 853319324 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - SV_Launch "));
				}
				TimeWarning val2 = TimeWarning.New("SV_Launch", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsActiveItem.Test(853319324u, "SV_Launch", this, player))
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
							SV_Launch(msg2);
						}
						finally
						{
							((IDisposable)val4)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in SV_Launch");
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

	public override bool ForceSendMagazine(SaveInfo saveInfo)
	{
		return true;
	}

	public override void ServerUse()
	{
		ServerUse(1f);
	}

	public override void ServerUse(float damageModifier, Transform originOverride = null)
	{
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		ItemModProjectile component = ((Component)primaryMagazine.ammoType).GetComponent<ItemModProjectile>();
		if (!Object.op_Implicit((Object)(object)component))
		{
			return;
		}
		if (primaryMagazine.contents <= 0)
		{
			SignalBroadcast(Signal.DryFire);
			StartAttackCooldown(1f);
			return;
		}
		GameObject val = component.projectileObject.Get();
		if (!Object.op_Implicit((Object)(object)val.GetComponent<ServerProjectile>()))
		{
			base.ServerUse(damageModifier, originOverride);
			return;
		}
		primaryMagazine.contents--;
		if (primaryMagazine.contents < 0)
		{
			primaryMagazine.contents = 0;
		}
		Vector3 val2 = ((Component)MuzzlePoint).transform.forward;
		Vector3 position = ((Component)MuzzlePoint).transform.position;
		float num = GetAimCone() + component.projectileSpread;
		if (num > 0f)
		{
			val2 = AimConeUtil.GetModifiedAimConeDirection(num, val2);
		}
		float num2 = 1f;
		RaycastHit val3 = default(RaycastHit);
		if (Physics.Raycast(position, val2, ref val3, num2, 1237003025))
		{
			num2 = ((RaycastHit)(ref val3)).distance - 0.1f;
		}
		BaseEntity baseEntity = GameManager.server.CreateEntity(component.projectileObject.resourcePath, position + val2 * num2);
		if (!((Object)(object)baseEntity == (Object)null))
		{
			BasePlayer ownerPlayer = GetOwnerPlayer();
			bool flag = (Object)(object)ownerPlayer != (Object)null && ownerPlayer.IsNpc;
			ServerProjectile component2 = ((Component)baseEntity).GetComponent<ServerProjectile>();
			if (Object.op_Implicit((Object)(object)component2))
			{
				component2.InitializeVelocity(val2 * component2.speed);
			}
			((Component)baseEntity).SendMessage("SetDamageScale", (object)(flag ? npcDamageScale : turretDamageScale));
			baseEntity.Spawn();
			StartAttackCooldown(ScaleRepeatDelay(repeatDelay));
			SignalBroadcast(Signal.Attack, string.Empty);
			GetOwnerItem()?.LoseCondition(Random.Range(1f, 2f));
		}
	}

	[RPC_Server]
	[RPC_Server.IsActiveItem]
	private void SV_Launch(RPCMessage msg)
	{
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
		BasePlayer player = msg.player;
		if (!VerifyClientAttack(player))
		{
			SendNetworkUpdate();
			return;
		}
		if (reloadFinished && HasReloadCooldown())
		{
			AntiHack.Log(player, AntiHackType.ProjectileHack, "Reloading (" + base.ShortPrefabName + ")");
			player.stats.combat.LogInvalid(player, this, "reload_cooldown");
			return;
		}
		reloadStarted = false;
		reloadFinished = false;
		if (!base.UsingInfiniteAmmoCheat)
		{
			if (primaryMagazine.contents <= 0)
			{
				AntiHack.Log(player, AntiHackType.ProjectileHack, "Magazine empty (" + base.ShortPrefabName + ")");
				player.stats.combat.LogInvalid(player, this, "magazine_empty");
				return;
			}
			primaryMagazine.contents--;
		}
		SignalBroadcast(Signal.Attack, string.Empty, player.net.connection);
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
		ItemModProjectile component = ((Component)primaryMagazine.ammoType).GetComponent<ItemModProjectile>();
		if (!Object.op_Implicit((Object)(object)component))
		{
			AntiHack.Log(player, AntiHackType.ProjectileHack, "Item mod not found (" + base.ShortPrefabName + ")");
			player.stats.combat.LogInvalid(player, this, "mod_missing");
			return;
		}
		float num = GetAimCone() + component.projectileSpread;
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
		BaseEntity baseEntity = GameManager.server.CreateEntity(component.projectileObject.resourcePath, val + val3 * num2);
		if (!((Object)(object)baseEntity == (Object)null))
		{
			baseEntity.creatorEntity = player;
			ServerProjectile component2 = ((Component)baseEntity).GetComponent<ServerProjectile>();
			if (Object.op_Implicit((Object)(object)component2))
			{
				component2.InitializeVelocity(GetInheritedVelocity(player, val3) + val3 * component2.speed);
			}
			baseEntity.Spawn();
			Analytics.Azure.OnExplosiveLaunched(player, baseEntity, this);
			StartAttackCooldown(ScaleRepeatDelay(repeatDelay));
			Item ownerItem = GetOwnerItem();
			if (ownerItem != null && !base.UsingInfiniteAmmoCheat)
			{
				ownerItem.LoseCondition(Random.Range(1f, 2f));
			}
		}
	}
}
