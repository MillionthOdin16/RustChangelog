using System;
using System.Collections.Generic;
using Facepunch;
using Facepunch.Rust;
using Network;
using Rust;
using UnityEngine;
using UnityEngine.Profiling;

public class FlameTurret : StorageContainer
{
	public class UpdateFlameTurretWorkQueue : ObjectWorkQueue<FlameTurret>
	{
		protected override void RunJob(FlameTurret entity)
		{
			if (((ObjectWorkQueue<FlameTurret>)this).ShouldAdd(entity))
			{
				entity.ServerThink();
			}
		}

		protected override bool ShouldAdd(FlameTurret entity)
		{
			return base.ShouldAdd(entity) && entity.IsValid();
		}
	}

	public static UpdateFlameTurretWorkQueue updateFlameTurretQueueServer = new UpdateFlameTurretWorkQueue();

	public Transform upper;

	public Vector3 aimDir;

	public float arc = 45f;

	public float triggeredDuration = 5f;

	public float flameRange = 7f;

	public float flameRadius = 4f;

	public float fuelPerSec = 1f;

	public Transform eyeTransform;

	public List<DamageTypeEntry> damagePerSec;

	public GameObjectRef triggeredEffect;

	public GameObjectRef fireballPrefab;

	public GameObjectRef explosionEffect;

	public TargetTrigger trigger;

	private float nextFireballTime = 0f;

	private int turnDir = 1;

	private float lastMovementUpdate = 0f;

	private float triggeredTime = 0f;

	private float lastServerThink = 0f;

	private float triggerCheckRate = 2f;

	private float nextTriggerCheckTime = 0f;

	private float pendingFuel = 0f;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("FlameTurret.OnRpcMessage", 0);
		try
		{
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	public bool IsTriggered()
	{
		return HasFlag(Flags.Reserved4);
	}

	public Vector3 GetEyePosition()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		return eyeTransform.position;
	}

	public override bool CanPickup(BasePlayer player)
	{
		return base.CanPickup(player) && !IsTriggered();
	}

	public void SetTriggered(bool triggered)
	{
		if (triggered && HasFuel())
		{
			triggeredTime = Time.realtimeSinceStartup;
		}
		SetFlag(Flags.Reserved4, triggered && HasFuel());
	}

	public override void ServerInit()
	{
		base.ServerInit();
		((FacepunchBehaviour)this).InvokeRepeating((Action)SendAimDir, 0f, 0.1f);
	}

	public void SendAimDir()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		float delta = Time.realtimeSinceStartup - lastMovementUpdate;
		lastMovementUpdate = Time.realtimeSinceStartup;
		MovementUpdate(delta);
		ClientRPC<Vector3>(null, "CLIENT_ReceiveAimDir", aimDir);
		((ObjectWorkQueue<FlameTurret>)updateFlameTurretQueueServer).Add(this);
	}

	public float GetSpinSpeed()
	{
		return IsTriggered() ? 180 : 45;
	}

	public override void OnAttacked(HitInfo info)
	{
		if (!base.isClient)
		{
			if (info.damageTypes.IsMeleeType())
			{
				SetTriggered(triggered: true);
			}
			base.OnAttacked(info);
		}
	}

	public void MovementUpdate(float delta)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		aimDir += new Vector3(0f, delta * GetSpinSpeed(), 0f) * (float)turnDir;
		if (aimDir.y >= arc || aimDir.y <= 0f - arc)
		{
			turnDir *= -1;
			aimDir.y = Mathf.Clamp(aimDir.y, 0f - arc, arc);
		}
	}

	public void ServerThink()
	{
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		bool flag = IsTriggered();
		float delta = Time.realtimeSinceStartup - lastServerThink;
		lastServerThink = Time.realtimeSinceStartup;
		Profiler.BeginSample("FlameTurret.ServerThink");
		if (IsTriggered() && (Time.realtimeSinceStartup - triggeredTime > triggeredDuration || !HasFuel()))
		{
			SetTriggered(triggered: false);
		}
		if (!IsTriggered() && HasFuel() && CheckTrigger())
		{
			SetTriggered(triggered: true);
			Effect.server.Run(triggeredEffect.resourcePath, ((Component)this).transform.position, Vector3.up);
		}
		if (flag != IsTriggered())
		{
			SendNetworkUpdateImmediate();
		}
		if (IsTriggered())
		{
			DoFlame(delta);
		}
		Profiler.EndSample();
	}

	public bool CheckTrigger()
	{
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		if (Time.realtimeSinceStartup < nextTriggerCheckTime)
		{
			return false;
		}
		nextTriggerCheckTime = Time.realtimeSinceStartup + 1f / triggerCheckRate;
		Profiler.BeginSample("FlameTurret.CheckTrigger");
		List<RaycastHit> list = Pool.GetList<RaycastHit>();
		HashSet<BaseEntity> entityContents = trigger.entityContents;
		bool flag = false;
		if (entityContents != null)
		{
			foreach (BaseEntity item in entityContents)
			{
				BasePlayer component = ((Component)item).GetComponent<BasePlayer>();
				if (component.IsSleeping() || !component.IsAlive() || !(((Component)component).transform.position.y <= GetEyePosition().y + 0.5f) || component.IsBuildingAuthed())
				{
					continue;
				}
				list.Clear();
				Vector3 position = component.eyes.position;
				Vector3 val = GetEyePosition() - component.eyes.position;
				GamePhysics.TraceAll(new Ray(position, ((Vector3)(ref val)).normalized), 0f, list, 9f, 1218519297, (QueryTriggerInteraction)0);
				for (int i = 0; i < list.Count; i++)
				{
					RaycastHit hit = list[i];
					BaseEntity entity = hit.GetEntity();
					if ((Object)(object)entity != (Object)null && ((Object)(object)entity == (Object)(object)this || entity.EqualNetID((BaseNetworkable)this)))
					{
						flag = true;
						break;
					}
					if (!((Object)(object)entity != (Object)null) || entity.ShouldBlockProjectiles())
					{
						break;
					}
				}
				if (flag)
				{
					break;
				}
			}
		}
		Pool.FreeList<RaycastHit>(ref list);
		Profiler.EndSample();
		return flag;
	}

	public override void OnKilled(HitInfo info)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		float num = (float)GetFuelAmount() / 500f;
		DamageUtil.RadiusDamage(this, LookupPrefab(), GetEyePosition(), 2f, 6f, damagePerSec, 133120, useLineOfSight: true);
		Effect.server.Run(explosionEffect.resourcePath, ((Component)this).transform.position, Vector3.up);
		int num2 = Mathf.CeilToInt(Mathf.Clamp(num * 8f, 1f, 8f));
		for (int i = 0; i < num2; i++)
		{
			BaseEntity baseEntity = GameManager.server.CreateEntity(fireballPrefab.resourcePath, ((Component)this).transform.position, ((Component)this).transform.rotation);
			if (Object.op_Implicit((Object)(object)baseEntity))
			{
				Vector3 onUnitSphere = Random.onUnitSphere;
				((Component)baseEntity).transform.position = ((Component)this).transform.position + new Vector3(0f, 1.5f, 0f) + onUnitSphere * Random.Range(-1f, 1f);
				baseEntity.Spawn();
				baseEntity.SetVelocity(onUnitSphere * (float)Random.Range(3, 10));
			}
		}
		base.OnKilled(info);
	}

	public int GetFuelAmount()
	{
		Item slot = base.inventory.GetSlot(0);
		if (slot == null || slot.amount < 1)
		{
			return 0;
		}
		return slot.amount;
	}

	public bool HasFuel()
	{
		return GetFuelAmount() > 0;
	}

	public bool UseFuel(float seconds)
	{
		Item slot = base.inventory.GetSlot(0);
		if (slot == null || slot.amount < 1)
		{
			return false;
		}
		pendingFuel += seconds * fuelPerSec;
		if (pendingFuel >= 1f)
		{
			int num = Mathf.FloorToInt(pendingFuel);
			slot.UseItem(num);
			Analytics.Azure.AddPendingItems(this, slot.info.shortname, num, "flame_turret");
			pendingFuel -= num;
		}
		return true;
	}

	public void DoFlame(float delta)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		if (!UseFuel(delta))
		{
			return;
		}
		Profiler.BeginSample("FlameTurret.DoFlame");
		Ray val = default(Ray);
		((Ray)(ref val))._002Ector(GetEyePosition(), ((Component)this).transform.TransformDirection(Quaternion.Euler(aimDir) * Vector3.forward));
		Vector3 origin = ((Ray)(ref val)).origin;
		RaycastHit val2 = default(RaycastHit);
		bool flag = Physics.SphereCast(val, 0.4f, ref val2, flameRange, 1218652417);
		if (!flag)
		{
			((RaycastHit)(ref val2)).point = origin + ((Ray)(ref val)).direction * flameRange;
		}
		float amount = damagePerSec[0].amount;
		damagePerSec[0].amount = amount * delta;
		DamageUtil.RadiusDamage(this, LookupPrefab(), ((RaycastHit)(ref val2)).point - ((Ray)(ref val)).direction * 0.1f, flameRadius * 0.5f, flameRadius, damagePerSec, 2230272, useLineOfSight: true);
		DamageUtil.RadiusDamage(this, LookupPrefab(), ((Component)this).transform.position + new Vector3(0f, 1.25f, 0f), 0.25f, 0.25f, damagePerSec, 133120, useLineOfSight: false);
		damagePerSec[0].amount = amount;
		if (Time.realtimeSinceStartup >= nextFireballTime)
		{
			nextFireballTime = Time.realtimeSinceStartup + Random.Range(1f, 2f);
			bool flag2 = Random.Range(0, 10) <= 7;
			Vector3 val3 = ((flag2 && flag) ? ((RaycastHit)(ref val2)).point : (((Ray)(ref val)).origin + ((Ray)(ref val)).direction * (flag ? ((RaycastHit)(ref val2)).distance : flameRange) * Random.Range(0.4f, 1f)));
			BaseEntity baseEntity = GameManager.server.CreateEntity(fireballPrefab.resourcePath, val3 - ((Ray)(ref val)).direction * 0.25f);
			if (Object.op_Implicit((Object)(object)baseEntity))
			{
				baseEntity.creatorEntity = this;
				baseEntity.Spawn();
			}
		}
		Profiler.EndSample();
	}
}
