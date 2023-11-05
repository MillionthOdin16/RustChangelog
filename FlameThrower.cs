using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

public class FlameThrower : AttackEntity
{
	[Header("Flame Thrower")]
	public int maxAmmo = 100;

	public int ammo = 100;

	public ItemDefinition fuelType;

	public float timeSinceLastAttack = 0f;

	[FormerlySerializedAs("nextAttackTime")]
	public float nextReadyTime = 0f;

	public float flameRange = 10f;

	public float flameRadius = 2.5f;

	public ParticleSystem[] flameEffects;

	public FlameJet jet;

	public GameObjectRef fireballPrefab;

	public List<DamageTypeEntry> damagePerSec;

	public SoundDefinition flameStart3P;

	public SoundDefinition flameLoop3P;

	public SoundDefinition flameStop3P;

	public SoundDefinition pilotLoopSoundDef;

	private float tickRate = 0.25f;

	private float lastFlameTick;

	public float fuelPerSec;

	private float ammoRemainder = 0f;

	public float reloadDuration = 3.5f;

	private float lastReloadTime = -10f;

	private float nextFlameTime = 0f;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("FlameThrower.OnRpcMessage", 0);
		try
		{
			if (rpc == 3381353917u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - DoReload "));
				}
				TimeWarning val2 = TimeWarning.New("DoReload", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsActiveItem.Test(3381353917u, "DoReload", this, player))
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
							DoReload(msg2);
						}
						finally
						{
							((IDisposable)val4)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in DoReload");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 3749570935u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - SetFiring "));
				}
				TimeWarning val5 = TimeWarning.New("SetFiring", 0);
				try
				{
					TimeWarning val6 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsActiveItem.Test(3749570935u, "SetFiring", this, player))
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
							RPCMessage firing = rPCMessage;
							SetFiring(firing);
						}
						finally
						{
							((IDisposable)val7)?.Dispose();
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in SetFiring");
					}
				}
				finally
				{
					((IDisposable)val5)?.Dispose();
				}
				return true;
			}
			if (rpc == 1057268396 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - TogglePilotLight "));
				}
				TimeWarning val8 = TimeWarning.New("TogglePilotLight", 0);
				try
				{
					TimeWarning val9 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsActiveItem.Test(1057268396u, "TogglePilotLight", this, player))
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
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage msg3 = rPCMessage;
							TogglePilotLight(msg3);
						}
						finally
						{
							((IDisposable)val10)?.Dispose();
						}
					}
					catch (Exception ex3)
					{
						Debug.LogException(ex3);
						player.Kick("RPC Error in TogglePilotLight");
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

	private bool IsWeaponBusy()
	{
		return Time.realtimeSinceStartup < nextReadyTime;
	}

	private void SetBusyFor(float dur)
	{
		nextReadyTime = Time.realtimeSinceStartup + dur;
	}

	private void ClearBusy()
	{
		nextReadyTime = Time.realtimeSinceStartup - 1f;
	}

	public void ReduceAmmo(float firingTime)
	{
		if (base.UsingInfiniteAmmoCheat)
		{
			return;
		}
		ammoRemainder += fuelPerSec * firingTime;
		if (ammoRemainder >= 1f)
		{
			int num = Mathf.FloorToInt(ammoRemainder);
			ammoRemainder -= num;
			if (ammoRemainder >= 1f)
			{
				num++;
				ammoRemainder -= 1f;
			}
			ammo -= num;
			if (ammo <= 0)
			{
				ammo = 0;
			}
		}
	}

	public void PilotLightToggle_Shared()
	{
		SetFlag(Flags.On, !HasFlag(Flags.On));
		if (base.isServer)
		{
			SendNetworkUpdateImmediate();
		}
	}

	public bool IsPilotOn()
	{
		return HasFlag(Flags.On);
	}

	public bool IsFlameOn()
	{
		return HasFlag(Flags.OnFire);
	}

	public bool HasAmmo()
	{
		return GetAmmo() != null;
	}

	public Item GetAmmo()
	{
		BasePlayer ownerPlayer = GetOwnerPlayer();
		if (!Object.op_Implicit((Object)(object)ownerPlayer))
		{
			return null;
		}
		Item item = ownerPlayer.inventory.containerMain.FindItemsByItemName(fuelType.shortname);
		if (item == null)
		{
			item = ownerPlayer.inventory.containerBelt.FindItemsByItemName(fuelType.shortname);
		}
		return item;
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (info.msg.baseProjectile != null && info.msg.baseProjectile.primaryMagazine != null)
		{
			ammo = info.msg.baseProjectile.primaryMagazine.contents;
		}
	}

	public override void CollectedForCrafting(Item item, BasePlayer crafter)
	{
		ServerCommand(item, "unload_ammo", crafter);
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		info.msg.baseProjectile = Pool.Get<BaseProjectile>();
		info.msg.baseProjectile.primaryMagazine = Pool.Get<Magazine>();
		info.msg.baseProjectile.primaryMagazine.contents = ammo;
	}

	[RPC_Server]
	[RPC_Server.IsActiveItem]
	public void SetFiring(RPCMessage msg)
	{
		bool flameState = msg.read.Bit();
		SetFlameState(flameState);
	}

	public override void ServerUse()
	{
		if (!IsOnFire())
		{
			SetFlameState(wantsOn: true);
			((FacepunchBehaviour)this).Invoke((Action)StopFlameState, 0.2f);
			base.ServerUse();
		}
	}

	public override void TopUpAmmo()
	{
		ammo = maxAmmo;
	}

	public override float AmmoFraction()
	{
		return (float)ammo / (float)maxAmmo;
	}

	public override bool ServerIsReloading()
	{
		return Time.time < lastReloadTime + reloadDuration;
	}

	public override bool CanReload()
	{
		return ammo < maxAmmo;
	}

	public override void ServerReload()
	{
		if (!ServerIsReloading())
		{
			lastReloadTime = Time.time;
			StartAttackCooldown(reloadDuration);
			GetOwnerPlayer().SignalBroadcast(Signal.Reload);
			ammo = maxAmmo;
		}
	}

	public void StopFlameState()
	{
		SetFlameState(wantsOn: false);
	}

	[RPC_Server]
	[RPC_Server.IsActiveItem]
	public void DoReload(RPCMessage msg)
	{
		BasePlayer ownerPlayer = GetOwnerPlayer();
		if (!((Object)(object)ownerPlayer == (Object)null))
		{
			Item item = null;
			while (ammo < maxAmmo && (item = GetAmmo()) != null && item.amount > 0)
			{
				int num = Mathf.Min(maxAmmo - ammo, item.amount);
				ammo += num;
				item.UseItem(num);
			}
			SendNetworkUpdateImmediate();
			ItemManager.DoRemoves();
			ownerPlayer.inventory.ServerUpdate(0f);
		}
	}

	public void SetFlameState(bool wantsOn)
	{
		if (wantsOn)
		{
			if (!base.UsingInfiniteAmmoCheat)
			{
				ammo--;
			}
			if (ammo < 0)
			{
				ammo = 0;
			}
		}
		if (wantsOn && ammo <= 0)
		{
			wantsOn = false;
		}
		if (wantsOn)
		{
		}
		SetFlag(Flags.OnFire, wantsOn);
		if (IsFlameOn())
		{
			nextFlameTime = Time.realtimeSinceStartup + 1f;
			lastFlameTick = Time.realtimeSinceStartup;
			((FacepunchBehaviour)this).InvokeRepeating((Action)FlameTick, tickRate, tickRate);
		}
		else
		{
			((FacepunchBehaviour)this).CancelInvoke((Action)FlameTick);
		}
	}

	[RPC_Server]
	[RPC_Server.IsActiveItem]
	public void TogglePilotLight(RPCMessage msg)
	{
		PilotLightToggle_Shared();
	}

	public override void OnHeldChanged()
	{
		SetFlameState(wantsOn: false);
		base.OnHeldChanged();
	}

	public void FlameTick()
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		float num = Time.realtimeSinceStartup - lastFlameTick;
		lastFlameTick = Time.realtimeSinceStartup;
		BasePlayer ownerPlayer = GetOwnerPlayer();
		if (!Object.op_Implicit((Object)(object)ownerPlayer))
		{
			return;
		}
		ReduceAmmo(num);
		SendNetworkUpdate();
		Ray val = ownerPlayer.eyes.BodyRay();
		Vector3 origin = ((Ray)(ref val)).origin;
		RaycastHit val2 = default(RaycastHit);
		bool flag = Physics.SphereCast(val, 0.3f, ref val2, flameRange, 1218652417);
		if (!flag)
		{
			((RaycastHit)(ref val2)).point = origin + ((Ray)(ref val)).direction * flameRange;
		}
		float num2 = (ownerPlayer.IsNpc ? npcDamageScale : 1f);
		float amount = damagePerSec[0].amount;
		damagePerSec[0].amount = amount * num * num2;
		DamageUtil.RadiusDamage(ownerPlayer, LookupPrefab(), ((RaycastHit)(ref val2)).point - ((Ray)(ref val)).direction * 0.1f, flameRadius * 0.5f, flameRadius, damagePerSec, 2279681, useLineOfSight: true);
		damagePerSec[0].amount = amount;
		if (flag && Time.realtimeSinceStartup >= nextFlameTime && ((RaycastHit)(ref val2)).distance > 1.1f)
		{
			nextFlameTime = Time.realtimeSinceStartup + 0.45f;
			Vector3 point = ((RaycastHit)(ref val2)).point;
			BaseEntity baseEntity = GameManager.server.CreateEntity(fireballPrefab.resourcePath, point - ((Ray)(ref val)).direction * 0.25f);
			if (Object.op_Implicit((Object)(object)baseEntity))
			{
				baseEntity.creatorEntity = ownerPlayer;
				baseEntity.Spawn();
			}
		}
		if (ammo == 0)
		{
			SetFlameState(wantsOn: false);
		}
		Item ownerItem = GetOwnerItem();
		if (ownerItem != null && !base.UsingInfiniteAmmoCheat)
		{
			ownerItem.LoseCondition(num);
		}
	}

	public override void ServerCommand(Item item, string command, BasePlayer player)
	{
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		if (item == null || !(command == "unload_ammo"))
		{
			return;
		}
		int num = ammo;
		if (num > 0)
		{
			ammo = 0;
			SendNetworkUpdateImmediate();
			Item item2 = ItemManager.Create(fuelType, num, 0uL);
			if (!item2.MoveToContainer(player.inventory.containerMain))
			{
				item2.Drop(player.eyes.position, player.eyes.BodyForward() * 2f);
			}
		}
	}
}
