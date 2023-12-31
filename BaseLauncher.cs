using System;
using ConVar;
using Facepunch.Rust;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

public class BaseLauncher : BaseProjectile
{
	public float initialSpeedMultiplier = 1f;

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
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - SV_Launch "));
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
						val3 = TimeWarning.New("Call", 0);
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
							((IDisposable)val3)?.Dispose();
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
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
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
		if (!Object.op_Implicit((Object)(object)component.projectileObject.Get().GetComponent<ServerProjectile>()))
		{
			base.ServerUse(damageModifier, originOverride);
			return;
		}
		ModifyAmmoCount(-1);
		if (primaryMagazine.contents < 0)
		{
			SetAmmoCount(0);
		}
		Vector3 val = ((Component)MuzzlePoint).transform.forward;
		Vector3 position = ((Component)MuzzlePoint).transform.position;
		float num = GetAimCone() + component.projectileSpread;
		if (num > 0f)
		{
			val = AimConeUtil.GetModifiedAimConeDirection(num, val);
		}
		float num2 = 1f;
		RaycastHit val2 = default(RaycastHit);
		if (Physics.Raycast(position, val, ref val2, num2, 1237003025))
		{
			num2 = ((RaycastHit)(ref val2)).distance - 0.1f;
		}
		BaseEntity baseEntity = GameManager.server.CreateEntity(component.projectileObject.resourcePath, position + val * num2);
		if (!((Object)(object)baseEntity == (Object)null))
		{
			BasePlayer ownerPlayer = GetOwnerPlayer();
			bool flag = (Object)(object)ownerPlayer != (Object)null && ownerPlayer.IsNpc;
			ServerProjectile component2 = ((Component)baseEntity).GetComponent<ServerProjectile>();
			if (Object.op_Implicit((Object)(object)component2))
			{
				component2.InitializeVelocity(val * component2.speed * initialSpeedMultiplier);
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
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
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
			ModifyAmmoCount(-1);
		}
		SignalBroadcast(Signal.Attack, string.Empty, player.net.connection);
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
		ItemModProjectile component = ((Component)primaryMagazine.ammoType).GetComponent<ItemModProjectile>();
		if (!Object.op_Implicit((Object)(object)component))
		{
			AntiHack.Log(player, AntiHackType.ProjectileHack, "Item mod not found (" + base.ShortPrefabName + ")");
			player.stats.combat.LogInvalid(player, this, "mod_missing");
			return;
		}
		float num2 = GetAimCone() + component.projectileSpread;
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
		BaseEntity baseEntity = GameManager.server.CreateEntity(component.projectileObject.resourcePath, val + val3 * num3);
		if ((Object)(object)baseEntity == (Object)null)
		{
			return;
		}
		baseEntity.creatorEntity = player;
		ServerProjectile component2 = ((Component)baseEntity).GetComponent<ServerProjectile>();
		if (Object.op_Implicit((Object)(object)component2))
		{
			component2.InitializeVelocity(GetInheritedVelocity(player, val3) + val3 * component2.speed * initialSpeedMultiplier);
		}
		baseEntity.Spawn();
		ProjectileLaunched_Server(component2);
		Analytics.Azure.OnExplosiveLaunched(player, baseEntity, this);
		StartAttackCooldown(ScaleRepeatDelay(repeatDelay));
		Item ownerItem = GetOwnerItem();
		if (ownerItem != null)
		{
			if (!base.UsingInfiniteAmmoCheat)
			{
				ownerItem.LoseCondition(Random.Range(1f, 2f));
			}
			BaseMountable mounted2 = player.GetMounted();
			if ((Object)(object)mounted2 != (Object)null)
			{
				mounted2.OnWeaponFired(this);
			}
		}
	}

	public virtual void ProjectileLaunched_Server(ServerProjectile justLaunched)
	{
	}
}
