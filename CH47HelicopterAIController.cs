using System;
using Rust;
using UnityEngine;

public class CH47HelicopterAIController : CH47Helicopter
{
	public GameObjectRef scientistPrefab;

	public GameObjectRef dismountablePrefab;

	public GameObjectRef weakDismountablePrefab;

	public float maxTiltAngle = 0.3f;

	public float AiAltitudeForce = 10000f;

	public GameObjectRef lockedCratePrefab;

	public const Flags Flag_Damaged = Flags.Reserved7;

	public const Flags Flag_NearDeath = Flags.OnFire;

	public const Flags Flag_DropDoorOpen = Flags.Reserved8;

	public GameObject triggerHurt;

	public Vector3 landingTarget;

	private int numCrates = 1;

	private bool shouldLand = false;

	private bool aimDirOverride = false;

	private Vector3 _aimDirection = Vector3.forward;

	private Vector3 _moveTarget = Vector3.zero;

	private int lastAltitudeCheckFrame = 0;

	private float altOverride;

	private float currentDesiredAltitude = 0f;

	private bool altitudeProtection = true;

	private float hoverHeight = 30f;

	public void DropCrate()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		if (numCrates > 0)
		{
			Vector3 pos = ((Component)this).transform.position + Vector3.down * 5f;
			Quaternion rot = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
			BaseEntity baseEntity = GameManager.server.CreateEntity(lockedCratePrefab.resourcePath, pos, rot);
			if (Object.op_Implicit((Object)(object)baseEntity))
			{
				((Component)baseEntity).SendMessage("SetWasDropped");
				baseEntity.Spawn();
			}
			numCrates--;
		}
	}

	public bool OutOfCrates()
	{
		return numCrates <= 0;
	}

	public bool CanDropCrate()
	{
		return numCrates > 0;
	}

	public bool IsDropDoorOpen()
	{
		return HasFlag(Flags.Reserved8);
	}

	public void SetDropDoorOpen(bool open)
	{
		SetFlag(Flags.Reserved8, open);
	}

	public bool ShouldLand()
	{
		return shouldLand;
	}

	public void SetLandingTarget(Vector3 target)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		shouldLand = true;
		landingTarget = target;
		numCrates = 0;
	}

	public void ClearLandingTarget()
	{
		shouldLand = false;
	}

	public void TriggeredEventSpawn()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		float x = TerrainMeta.Size.x;
		float y = 30f;
		Vector3 val = Vector3Ex.Range(-1f, 1f);
		val.y = 0f;
		((Vector3)(ref val)).Normalize();
		val *= x * 1f;
		val.y = y;
		((Component)this).transform.position = val;
	}

	public override void AttemptMount(BasePlayer player, bool doMountChecks = true)
	{
		if (player.IsNpc || player.IsAdmin)
		{
			base.AttemptMount(player, doMountChecks);
		}
	}

	public override void ServerInit()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		base.ServerInit();
		((FacepunchBehaviour)this).Invoke((Action)SpawnScientists, 0.25f);
		SetMoveTarget(((Component)this).transform.position);
	}

	public void SpawnPassenger(Vector3 spawnPos, string prefabPath)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		Quaternion identity = Quaternion.identity;
		HumanNPC component = ((Component)GameManager.server.CreateEntity(prefabPath, spawnPos, identity)).GetComponent<HumanNPC>();
		component.Spawn();
		AttemptMount(component);
	}

	public void SpawnPassenger(Vector3 spawnPos)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		Quaternion identity = Quaternion.identity;
		HumanNPC component = ((Component)GameManager.server.CreateEntity(dismountablePrefab.resourcePath, spawnPos, identity)).GetComponent<HumanNPC>();
		component.Spawn();
		AttemptMount(component);
	}

	public void SpawnScientist(Vector3 spawnPos)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		Quaternion identity = Quaternion.identity;
		HumanNPC component = ((Component)GameManager.server.CreateEntity(scientistPrefab.resourcePath, spawnPos, identity)).GetComponent<HumanNPC>();
		component.Spawn();
		AttemptMount(component);
		component.Brain.SetEnabled(flag: false);
	}

	public void SpawnScientists()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		if (shouldLand)
		{
			CH47LandingZone closest = CH47LandingZone.GetClosest(landingTarget);
			float dropoffScale = closest.dropoffScale;
			int num = mountPoints.Count - 2;
			int num2 = Mathf.FloorToInt((float)num * dropoffScale);
			for (int i = 0; i < num2; i++)
			{
				Vector3 spawnPos = ((Component)this).transform.position + ((Component)this).transform.forward * 10f;
				SpawnPassenger(spawnPos, dismountablePrefab.resourcePath);
			}
			for (int j = 0; j < 1; j++)
			{
				Vector3 spawnPos2 = ((Component)this).transform.position - ((Component)this).transform.forward * 15f;
				SpawnPassenger(spawnPos2);
			}
		}
		else
		{
			for (int k = 0; k < 4; k++)
			{
				Vector3 spawnPos3 = ((Component)this).transform.position + ((Component)this).transform.forward * 10f;
				SpawnScientist(spawnPos3);
			}
			for (int l = 0; l < 1; l++)
			{
				Vector3 spawnPos4 = ((Component)this).transform.position - ((Component)this).transform.forward * 15f;
				SpawnScientist(spawnPos4);
			}
		}
	}

	public void EnableFacingOverride(bool enabled)
	{
		aimDirOverride = enabled;
	}

	public void SetMoveTarget(Vector3 position)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		_moveTarget = position;
	}

	public Vector3 GetMoveTarget()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		return _moveTarget;
	}

	public void SetAimDirection(Vector3 dir)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		_aimDirection = dir;
	}

	public Vector3 GetAimDirectionOverride()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		return _aimDirection;
	}

	public Vector3 GetPosition()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		return ((Component)this).transform.position;
	}

	public override void MounteeTookDamage(BasePlayer mountee, HitInfo info)
	{
		InitiateAnger();
	}

	public void CancelAnger()
	{
		if (base.SecondsSinceAttacked > 120f)
		{
			UnHostile();
			((FacepunchBehaviour)this).CancelInvoke((Action)UnHostile);
		}
	}

	public void InitiateAnger()
	{
		((FacepunchBehaviour)this).CancelInvoke((Action)UnHostile);
		((FacepunchBehaviour)this).Invoke((Action)UnHostile, 120f);
		foreach (MountPointInfo mountPoint in mountPoints)
		{
			if (!((Object)(object)mountPoint.mountable != (Object)null))
			{
				continue;
			}
			BasePlayer mounted = mountPoint.mountable.GetMounted();
			if (Object.op_Implicit((Object)(object)mounted))
			{
				ScientistNPC scientistNPC = mounted as ScientistNPC;
				if ((Object)(object)scientistNPC != (Object)null)
				{
					scientistNPC.Brain.SetEnabled(flag: true);
				}
			}
		}
	}

	public void UnHostile()
	{
		foreach (MountPointInfo mountPoint in mountPoints)
		{
			if (!((Object)(object)mountPoint.mountable != (Object)null))
			{
				continue;
			}
			BasePlayer mounted = mountPoint.mountable.GetMounted();
			if (Object.op_Implicit((Object)(object)mounted))
			{
				ScientistNPC scientistNPC = mounted as ScientistNPC;
				if ((Object)(object)scientistNPC != (Object)null)
				{
					scientistNPC.Brain.SetEnabled(flag: false);
				}
			}
		}
	}

	public override void OnKilled(HitInfo info)
	{
		if (!OutOfCrates())
		{
			DropCrate();
		}
		base.OnKilled(info);
	}

	public override void OnAttacked(HitInfo info)
	{
		base.OnAttacked(info);
		InitiateAnger();
		SetFlag(Flags.Reserved7, base.healthFraction <= 0.8f);
		SetFlag(Flags.OnFire, base.healthFraction <= 0.33f);
	}

	public void DelayedKill()
	{
		foreach (MountPointInfo mountPoint in mountPoints)
		{
			if ((Object)(object)mountPoint.mountable != (Object)null)
			{
				BasePlayer mounted = mountPoint.mountable.GetMounted();
				if (Object.op_Implicit((Object)(object)mounted) && (Object)(object)((Component)mounted).transform != (Object)null && !mounted.IsDestroyed && !mounted.IsDead() && mounted.IsNpc)
				{
					mounted.Kill();
				}
			}
		}
		Kill();
	}

	public override void DismountAllPlayers()
	{
		foreach (MountPointInfo mountPoint in mountPoints)
		{
			if ((Object)(object)mountPoint.mountable != (Object)null)
			{
				BasePlayer mounted = mountPoint.mountable.GetMounted();
				if (Object.op_Implicit((Object)(object)mounted))
				{
					mounted.Hurt(10000f, DamageType.Explosion, this, useProtection: false);
				}
			}
		}
	}

	public void SetAltitudeProtection(bool on)
	{
		altitudeProtection = on;
	}

	public void CalculateDesiredAltitude()
	{
		CalculateOverrideAltitude();
		if (altOverride > currentDesiredAltitude)
		{
			currentDesiredAltitude = altOverride;
		}
		else
		{
			currentDesiredAltitude = Mathf.MoveTowards(currentDesiredAltitude, altOverride, Time.fixedDeltaTime * 5f);
		}
	}

	public void SetMinHoverHeight(float newHeight)
	{
		hoverHeight = newHeight;
	}

	public float CalculateOverrideAltitude()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		if (Time.frameCount == lastAltitudeCheckFrame)
		{
			return altOverride;
		}
		lastAltitudeCheckFrame = Time.frameCount;
		float y = GetMoveTarget().y;
		float num = Mathf.Max(TerrainMeta.WaterMap.GetHeight(GetMoveTarget()), TerrainMeta.HeightMap.GetHeight(GetMoveTarget()));
		float num2 = Mathf.Max(y, num + hoverHeight);
		if (altitudeProtection)
		{
			Vector3 val = rigidBody.velocity;
			Vector3 val2;
			if (!(((Vector3)(ref val)).magnitude < 0.1f))
			{
				val = rigidBody.velocity;
				val2 = ((Vector3)(ref val)).normalized;
			}
			else
			{
				val2 = ((Component)this).transform.forward;
			}
			Vector3 val3 = val2;
			Vector3 val4 = Vector3.Cross(((Component)this).transform.up, val3);
			Vector3 val5 = Vector3.Cross(val4, Vector3.up);
			val = val5 + Vector3.down * 0.3f;
			Vector3 normalized = ((Vector3)(ref val)).normalized;
			RaycastHit val6 = default(RaycastHit);
			RaycastHit val7 = default(RaycastHit);
			if (Physics.SphereCast(((Component)this).transform.position - normalized * 20f, 20f, normalized, ref val6, 75f, 1218511105) && Physics.SphereCast(((RaycastHit)(ref val6)).point + Vector3.up * 200f, 20f, Vector3.down, ref val7, 200f, 1218511105))
			{
				num2 = ((RaycastHit)(ref val7)).point.y + hoverHeight;
			}
		}
		altOverride = num2;
		return altOverride;
	}

	public override void SetDefaultInputState()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0346: Unknown result type (might be due to invalid IL or missing references)
		//IL_032e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0339: Unknown result type (might be due to invalid IL or missing references)
		//IL_033e: Unknown result type (might be due to invalid IL or missing references)
		//IL_034b: Unknown result type (might be due to invalid IL or missing references)
		//IL_036e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0356: Unknown result type (might be due to invalid IL or missing references)
		//IL_0361: Unknown result type (might be due to invalid IL or missing references)
		//IL_0366: Unknown result type (might be due to invalid IL or missing references)
		//IL_0373: Unknown result type (might be due to invalid IL or missing references)
		//IL_0375: Unknown result type (might be due to invalid IL or missing references)
		//IL_0376: Unknown result type (might be due to invalid IL or missing references)
		//IL_037f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0380: Unknown result type (might be due to invalid IL or missing references)
		currentInputState.Reset();
		Vector3 moveTarget = GetMoveTarget();
		Vector3 val = Vector3.Cross(((Component)this).transform.right, Vector3.up);
		Vector3 val2 = Vector3.Cross(Vector3.up, val);
		float num = 0f - Vector3.Dot(Vector3.up, ((Component)this).transform.right);
		float num2 = Vector3.Dot(Vector3.up, ((Component)this).transform.forward);
		float num3 = Vector3Ex.Distance2D(((Component)this).transform.position, moveTarget);
		float y = ((Component)this).transform.position.y;
		float num4 = currentDesiredAltitude;
		Vector3 val3 = ((Component)this).transform.position + ((Component)this).transform.forward * 10f;
		val3.y = num4;
		Vector3 val4 = Vector3Ex.Direction2D(moveTarget, ((Component)this).transform.position);
		float num5 = 0f - Vector3.Dot(val4, val2);
		float num6 = Vector3.Dot(val4, val);
		float num7 = Mathf.InverseLerp(0f, 25f, num3);
		if (num6 > 0f)
		{
			float num8 = Mathf.InverseLerp(0f - maxTiltAngle, 0f, num2);
			currentInputState.pitch = 1f * num6 * num8 * num7;
		}
		else
		{
			float num9 = 1f - Mathf.InverseLerp(0f, maxTiltAngle, num2);
			currentInputState.pitch = 1f * num6 * num9 * num7;
		}
		if (num5 > 0f)
		{
			float num10 = Mathf.InverseLerp(0f - maxTiltAngle, 0f, num);
			currentInputState.roll = 1f * num5 * num10 * num7;
		}
		else
		{
			float num11 = 1f - Mathf.InverseLerp(0f, maxTiltAngle, num);
			currentInputState.roll = 1f * num5 * num11 * num7;
		}
		float num12 = Mathf.Abs(num4 - y);
		float num13 = 1f - Mathf.InverseLerp(10f, 30f, num12);
		currentInputState.pitch *= num13;
		currentInputState.roll *= num13;
		float num14 = maxTiltAngle;
		float num15 = Mathf.InverseLerp(0f + Mathf.Abs(currentInputState.pitch) * num14, num14 + Mathf.Abs(currentInputState.pitch) * num14, Mathf.Abs(num2));
		currentInputState.pitch += num15 * ((num2 < 0f) ? (-1f) : 1f);
		float num16 = Mathf.InverseLerp(0f + Mathf.Abs(currentInputState.roll) * num14, num14 + Mathf.Abs(currentInputState.roll) * num14, Mathf.Abs(num));
		currentInputState.roll += num16 * ((num < 0f) ? (-1f) : 1f);
		if (aimDirOverride || num3 > 30f)
		{
			Vector3 val5 = (aimDirOverride ? GetAimDirectionOverride() : Vector3Ex.Direction2D(GetMoveTarget(), ((Component)this).transform.position));
			Vector3 val6 = (aimDirOverride ? GetAimDirectionOverride() : Vector3Ex.Direction2D(GetMoveTarget(), ((Component)this).transform.position));
			float num17 = Vector3.Dot(val2, val5);
			float num18 = Vector3.Angle(val, val6);
			float num19 = Mathf.InverseLerp(0f, 70f, Mathf.Abs(num18));
			currentInputState.yaw = ((num17 > 0f) ? 1f : 0f);
			currentInputState.yaw -= ((num17 < 0f) ? 1f : 0f);
			currentInputState.yaw *= num19;
		}
		float throttle = Mathf.InverseLerp(5f, 30f, num3);
		currentInputState.throttle = throttle;
	}

	public void MaintainAIAltutide()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = ((Component)this).transform.position + rigidBody.velocity;
		float num = currentDesiredAltitude;
		float y = val.y;
		float num2 = Mathf.Abs(num - y);
		bool flag = num > y;
		float num3 = Mathf.InverseLerp(0f, 10f, num2) * AiAltitudeForce * (flag ? 1f : (-1f));
		rigidBody.AddForce(Vector3.up * num3, (ForceMode)0);
	}

	public override void VehicleFixedUpdate()
	{
		hoverForceScale = 1f;
		base.VehicleFixedUpdate();
		SetFlag(Flags.Reserved5, TOD_Sky.Instance.IsNight);
		CalculateDesiredAltitude();
		MaintainAIAltutide();
	}

	public override void DestroyShared()
	{
		if (base.isServer)
		{
			foreach (MountPointInfo mountPoint in mountPoints)
			{
				if ((Object)(object)mountPoint.mountable != (Object)null)
				{
					BasePlayer mounted = mountPoint.mountable.GetMounted();
					if (Object.op_Implicit((Object)(object)mounted) && (Object)(object)((Component)mounted).transform != (Object)null && !mounted.IsDestroyed && !mounted.IsDead() && mounted.IsNpc)
					{
						mounted.Kill();
					}
				}
			}
		}
		base.DestroyShared();
	}
}
