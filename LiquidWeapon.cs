using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

public class LiquidWeapon : BaseLiquidVessel
{
	[Header("Liquid Weapon")]
	public float FireRate = 0.2f;

	public float MaxRange = 10f;

	public int FireAmountML = 100;

	public int MaxPressure = 100;

	public int PressureLossPerTick = 5;

	public int PressureGainedPerPump = 25;

	public float MinDmgRadius = 0.15f;

	public float MaxDmgRadius = 0.15f;

	public float SplashRadius = 2f;

	public GameObjectRef ImpactSplashEffect;

	public AnimationCurve PowerCurve;

	public List<DamageTypeEntry> Damage;

	public LiquidWeaponEffects EntityWeaponEffects;

	public bool RequiresPumping = false;

	public bool AutoPump = false;

	public bool WaitForFillAnim = false;

	public bool UseFalloffCurve = false;

	public AnimationCurve FalloffCurve;

	public float PumpingBlockDuration = 0.5f;

	public float StartFillingBlockDuration = 2f;

	public float StopFillingBlockDuration = 1f;

	private float cooldownTime = 0f;

	private int pressure = 0;

	public const string RadiationFightAchievement = "SUMMER_RADICAL";

	public const string SoakedAchievement = "SUMMER_SOAKED";

	public const string LiquidatorAchievement = "SUMMER_LIQUIDATOR";

	public const string NoPressureAchievement = "SUMMER_NO_PRESSURE";

	public float PressureFraction => (float)pressure / (float)MaxPressure;

	public float MinimumPressureFraction => (float)PressureGainedPerPump / (float)MaxPressure;

	public float CurrentRange => UseFalloffCurve ? (MaxRange * FalloffCurve.Evaluate((float)(MaxPressure - pressure) / (float)MaxPressure)) : MaxRange;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("LiquidWeapon.OnRpcMessage", 0);
		try
		{
			if (rpc == 1600824953 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - PumpWater "));
				}
				TimeWarning val2 = TimeWarning.New("PumpWater", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsActiveItem.Test(1600824953u, "PumpWater", this, player))
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
							PumpWater(msg2);
						}
						finally
						{
							((IDisposable)val4)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in PumpWater");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 3724096303u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - StartFiring "));
				}
				TimeWarning val5 = TimeWarning.New("StartFiring", 0);
				try
				{
					TimeWarning val6 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsActiveItem.Test(3724096303u, "StartFiring", this, player))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val6)?.Dispose();
					}
					try
					{
						TimeWarning val7 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage msg3 = rPCMessage;
							StartFiring(msg3);
						}
						finally
						{
							((IDisposable)val7)?.Dispose();
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in StartFiring");
					}
				}
				finally
				{
					((IDisposable)val5)?.Dispose();
				}
				return true;
			}
			if (rpc == 789289044 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - StopFiring "));
				}
				TimeWarning val8 = TimeWarning.New("StopFiring", 0);
				try
				{
					TimeWarning val9 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsActiveItem.Test(789289044u, "StopFiring", this, player))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val9)?.Dispose();
					}
					try
					{
						TimeWarning val10 = TimeWarning.New("Call", 0);
						try
						{
							StopFiring();
						}
						finally
						{
							((IDisposable)val10)?.Dispose();
						}
					}
					catch (Exception ex3)
					{
						Debug.LogException(ex3);
						player.Kick("RPC Error in StopFiring");
					}
				}
				finally
				{
					((IDisposable)val8)?.Dispose();
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

	[RPC_Server]
	[RPC_Server.IsActiveItem]
	private void StartFiring(RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if (OnCooldown())
		{
			return;
		}
		if (!RequiresPumping)
		{
			pressure = MaxPressure;
		}
		if (CanFire(player))
		{
			((MonoBehaviour)this).CancelInvoke("FireTick");
			((MonoBehaviour)this).InvokeRepeating("FireTick", 0f, FireRate);
			SetFlag(Flags.On, b: true);
			StartCooldown(FireRate);
			if (base.isServer)
			{
				SendNetworkUpdateImmediate();
			}
		}
	}

	[RPC_Server]
	[RPC_Server.IsActiveItem]
	private void StopFiring()
	{
		((MonoBehaviour)this).CancelInvoke("FireTick");
		if (!RequiresPumping)
		{
			pressure = MaxPressure;
		}
		SetFlag(Flags.On, b: false);
		if (base.isServer)
		{
			SendNetworkUpdateImmediate();
		}
	}

	private bool CanFire(BasePlayer player)
	{
		if (RequiresPumping && pressure < PressureLossPerTick)
		{
			return false;
		}
		if ((Object)(object)player == (Object)null)
		{
			return false;
		}
		if (HasFlag(Flags.Open))
		{
			return false;
		}
		if (AmountHeld() <= 0)
		{
			return false;
		}
		if (!player.CanInteract())
		{
			return false;
		}
		if (!player.CanAttack() || player.IsRunning())
		{
			return false;
		}
		Item item = GetItem();
		if (item == null || item.contents == null)
		{
			return false;
		}
		return true;
	}

	[RPC_Server]
	[RPC_Server.IsActiveItem]
	public void PumpWater(RPCMessage msg)
	{
		PumpWater();
	}

	private void PumpWater()
	{
		BasePlayer ownerPlayer = GetOwnerPlayer();
		if (!((Object)(object)ownerPlayer == (Object)null) && !OnCooldown() && !Firing())
		{
			pressure += PressureGainedPerPump;
			pressure = Mathf.Min(pressure, MaxPressure);
			StartCooldown(PumpingBlockDuration);
			GetOwnerPlayer().SignalBroadcast(Signal.Reload);
			SendNetworkUpdateImmediate();
		}
	}

	private void FireTick()
	{
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		BasePlayer ownerPlayer = GetOwnerPlayer();
		if (!CanFire(ownerPlayer))
		{
			StopFiring();
			return;
		}
		int num = Mathf.Min(FireAmountML, AmountHeld());
		if (num == 0)
		{
			StopFiring();
			return;
		}
		LoseWater(num);
		float currentRange = CurrentRange;
		pressure -= PressureLossPerTick;
		if (pressure <= 0)
		{
			StopFiring();
		}
		Ray val = ownerPlayer.eyes.BodyRay();
		Debug.DrawLine(((Ray)(ref val)).origin, ((Ray)(ref val)).origin + ((Ray)(ref val)).direction * currentRange, Color.blue, 1f);
		RaycastHit val2 = default(RaycastHit);
		if (Physics.Raycast(val, ref val2, currentRange, 1218652417))
		{
			DoSplash(ownerPlayer, ((RaycastHit)(ref val2)).point, ((Ray)(ref val)).direction, num);
		}
		SendNetworkUpdate();
	}

	private void DoSplash(BasePlayer attacker, Vector3 position, Vector3 direction, int amount)
	{
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		Item item = GetItem();
		if (item != null && item.contents != null)
		{
			Item slot = item.contents.GetSlot(0);
			if (slot != null && slot.amount > 0 && !((Object)(object)slot.info == (Object)null))
			{
				WaterBall.DoSplash(position, SplashRadius, slot.info, amount);
				DamageUtil.RadiusDamage(attacker, LookupPrefab(), position, MinDmgRadius, MaxDmgRadius, Damage, 131072, useLineOfSight: true);
			}
		}
	}

	public override void OnHeldChanged()
	{
		base.OnHeldChanged();
		StopFiring();
	}

	private void StartCooldown(float duration)
	{
		if (Time.realtimeSinceStartup + duration > cooldownTime)
		{
			cooldownTime = Time.realtimeSinceStartup + duration;
		}
	}

	private bool OnCooldown()
	{
		return Time.realtimeSinceStartup < cooldownTime;
	}

	private bool Firing()
	{
		return HasFlag(Flags.On);
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		info.msg.baseProjectile = Pool.Get<BaseProjectile>();
		info.msg.baseProjectile.primaryMagazine = Pool.Get<Magazine>();
		info.msg.baseProjectile.primaryMagazine.contents = pressure;
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (info.msg.baseProjectile != null && info.msg.baseProjectile.primaryMagazine != null)
		{
			pressure = info.msg.baseProjectile.primaryMagazine.contents;
		}
	}
}
