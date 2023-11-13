using System;
using System.Collections.Generic;
using ConVar;
using Rust;
using UnityEngine;
using UnityEngine.Profiling;
using VacuumBreather;

public class BaseBoat : BaseVehicle
{
	[Header("Boat")]
	public float engineThrust = 10f;

	public float steeringScale = 0.1f;

	public Transform thrustPoint;

	public Transform centerOfMass;

	public Buoyancy buoyancy;

	public bool preventDecayIndoors = true;

	[Header("Correction Forces")]
	public Transform[] planeFitPoints;

	public Vector3 inAirPID;

	public float inAirDesiredPitch = -15f;

	public Vector3 wavePID;

	public MinMax correctionRange;

	public float correctionSpringForce;

	public float correctionSpringDamping;

	private Vector3[] worldAnchors;

	private PidQuaternionController pidController;

	[ServerVar]
	public static bool generate_paths = true;

	[NonSerialized]
	public float gasPedal = 0f;

	[NonSerialized]
	public float steering = 0f;

	public bool InDryDock()
	{
		return (Object)(object)GetParentEntity() != (Object)null;
	}

	public override float MaxVelocity()
	{
		return 25f;
	}

	public override void ServerInit()
	{
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		base.ServerInit();
		rigidBody.isKinematic = false;
		if ((Object)(object)rigidBody == (Object)null)
		{
			Debug.LogWarning((object)"Boat rigidbody null");
			return;
		}
		if ((Object)(object)centerOfMass == (Object)null)
		{
			Debug.LogWarning((object)"boat COM null");
			return;
		}
		rigidBody.centerOfMass = centerOfMass.localPosition;
		if (planeFitPoints == null || planeFitPoints.Length != 3)
		{
			Debug.LogWarning((object)"Boats require 3 plane fit points");
			return;
		}
		worldAnchors = (Vector3[])(object)new Vector3[3];
		pidController = new PidQuaternionController(wavePID.x, wavePID.y, wavePID.z);
	}

	public override void PlayerServerInput(InputState inputState, BasePlayer player)
	{
		if (IsDriver(player))
		{
			DriverInput(inputState, player);
		}
	}

	public virtual void DriverInput(InputState inputState, BasePlayer player)
	{
		if (inputState.IsDown(BUTTON.FORWARD))
		{
			gasPedal = 1f;
		}
		else if (inputState.IsDown(BUTTON.BACKWARD))
		{
			gasPedal = -0.5f;
		}
		else
		{
			gasPedal = 0f;
		}
		if (inputState.IsDown(BUTTON.LEFT))
		{
			steering = 1f;
		}
		else if (inputState.IsDown(BUTTON.RIGHT))
		{
			steering = -1f;
		}
		else
		{
			steering = 0f;
		}
	}

	public void OnPoolDestroyed()
	{
		Kill(DestroyMode.Gib);
	}

	public void WakeUp()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)rigidBody != (Object)null)
		{
			rigidBody.WakeUp();
			rigidBody.AddForce(Vector3.up * 0.1f, (ForceMode)1);
		}
	}

	protected override void OnServerWake()
	{
		if ((Object)(object)buoyancy != (Object)null)
		{
			buoyancy.Wake();
		}
	}

	public virtual bool EngineOn()
	{
		return HasDriver() && !IsFlipped();
	}

	public override void VehicleFixedUpdate()
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		base.VehicleFixedUpdate();
		Profiler.BeginSample("BaseBoat.VehicleFixedUpdate");
		if (!EngineOn())
		{
			gasPedal = 0f;
			steering = 0f;
		}
		base.VehicleFixedUpdate();
		ApplyCorrectionForces();
		bool flag = WaterLevel.Test(thrustPoint.position, waves: true, volumes: true, this);
		if (gasPedal != 0f && flag && buoyancy.submergedFraction > 0.3f)
		{
			Vector3 val = ((Component)this).transform.forward + ((Component)this).transform.right * steering * steeringScale;
			Vector3 val2 = ((Vector3)(ref val)).normalized * gasPedal * engineThrust;
			rigidBody.AddForceAtPosition(val2, thrustPoint.position, (ForceMode)0);
		}
		if (AnyMounted() && IsFlipped())
		{
			DismountAllPlayers();
		}
		Profiler.EndSample();
	}

	protected void ApplyCorrectionForces()
	{
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_0357: Unknown result type (might be due to invalid IL or missing references)
		//IL_035c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0364: Unknown result type (might be due to invalid IL or missing references)
		//IL_036e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0373: Unknown result type (might be due to invalid IL or missing references)
		//IL_0387: Unknown result type (might be due to invalid IL or missing references)
		if (planeFitPoints != null && planeFitPoints.Length == 3 && HasDriver() && !(buoyancy.submergedFraction < 0.5f))
		{
			Matrix4x4 val = Matrix4x4.TRS(((Component)this).transform.position, Quaternion.Euler(0f, ((Component)this).transform.eulerAngles.y, 0f), Vector3.one);
			for (int i = 0; i < planeFitPoints.Length; i++)
			{
				Vector3 val2 = ((Matrix4x4)(ref val)).MultiplyPoint(planeFitPoints[i].localPosition);
				val2.y = WaterSystem.GetHeight(val2);
				worldAnchors[i] = val2;
			}
			Plane val3 = default(Plane);
			((Plane)(ref val3))._002Ector(worldAnchors[0], worldAnchors[1], worldAnchors[2]);
			Vector3 normal = ((Plane)(ref val3)).normal;
			Vector3 val4 = Vector3.Normalize(worldAnchors[2] - worldAnchors[1]);
			Vector3 val5 = Vector3.Cross(normal, val4);
			Quaternion desiredOrientation = Quaternion.LookRotation(val5, normal);
			float y = planeFitPoints[0].localPosition.y;
			float num = (worldAnchors[0].y + worldAnchors[1].y + worldAnchors[2].y) / 3f - y;
			float y2 = ((Component)this).transform.position.y;
			float num2 = num - y2;
			Vector3 velocity = rigidBody.velocity;
			if (y2 > num + correctionRange.x && y2 < num + correctionRange.y)
			{
				float num3 = num2 * correctionSpringForce;
				float num4 = (0f - velocity.y) * correctionSpringDamping;
				rigidBody.AddForce(0f, num3 + num4, 0f, (ForceMode)0);
			}
			if (y2 > num + correctionRange.y)
			{
				desiredOrientation = Quaternion.Euler(inAirDesiredPitch, ((Component)this).transform.eulerAngles.y, 0f);
				pidController.Kp = inAirPID.x;
				pidController.Ki = inAirPID.y;
				pidController.Kd = inAirPID.z;
				Vector3 val6 = pidController.ComputeRequiredAngularAcceleration(((Component)this).transform.rotation, desiredOrientation, rigidBody.angularVelocity, Time.fixedDeltaTime);
				rigidBody.AddTorque(val6, (ForceMode)5);
			}
			else if (y2 > num + correctionRange.x)
			{
				pidController.Kp = wavePID.x;
				pidController.Ki = wavePID.y;
				pidController.Kd = wavePID.z;
				Vector3 val7 = pidController.ComputeRequiredAngularAcceleration(((Component)this).transform.rotation, desiredOrientation, rigidBody.angularVelocity, Time.fixedDeltaTime);
				val7.y = 0f;
				rigidBody.AddTorque(val7, (ForceMode)5);
			}
		}
	}

	public static void WaterVehicleDecay(BaseCombatEntity entity, float decayTickRate, float timeSinceLastUsed, float outsideDecayMinutes, float deepWaterDecayMinutes, float decayStartDelayMinutes, bool preventDecayIndoors)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		if (entity.healthFraction != 0f && !(timeSinceLastUsed < 60f * decayStartDelayMinutes))
		{
			float overallWaterDepth = WaterLevel.GetOverallWaterDepth(((Component)entity).transform.position, waves: true, volumes: false);
			float num = outsideDecayMinutes;
			if (preventDecayIndoors && !entity.IsOutside())
			{
				num = float.PositiveInfinity;
			}
			if (overallWaterDepth > 12f)
			{
				float num2 = Mathf.InverseLerp(12f, 16f, overallWaterDepth);
				float num3 = Mathf.Lerp(0.1f, 1f, num2);
				num = Mathf.Min(num, deepWaterDecayMinutes / num3);
			}
			if (!float.IsPositiveInfinity(num))
			{
				float num4 = decayTickRate / 60f / num;
				entity.Hurt(entity.MaxHealth() * num4, DamageType.Decay, entity, useProtection: false);
			}
		}
	}

	public virtual bool EngineInWater()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		return TerrainMeta.WaterMap.GetHeight(thrustPoint.position) > thrustPoint.position.y;
	}

	public override float WaterFactorForPlayer(BasePlayer player)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		if (TerrainMeta.WaterMap.GetHeight(player.eyes.position) >= player.eyes.position.y)
		{
			return 1f;
		}
		return 0f;
	}

	public static float GetWaterDepth(Vector3 pos)
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		if (!Application.isPlaying || (Object)(object)TerrainMeta.WaterMap == (Object)null)
		{
			RaycastHit val = default(RaycastHit);
			if (!Physics.Raycast(pos, Vector3.down, ref val, 100f, 8388608))
			{
				return 100f;
			}
			return ((RaycastHit)(ref val)).distance;
		}
		return TerrainMeta.WaterMap.GetDepth(pos);
	}

	public static List<Vector3> GenerateOceanPatrolPath(float minDistanceFromShore = 50f, float minWaterDepth = 8f)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		Vector3 size = TerrainMeta.Size;
		float x = size.x;
		float num = x * 2f * (float)Math.PI;
		float num2 = 30f;
		int num3 = Mathf.CeilToInt(num / num2);
		List<Vector3> list = new List<Vector3>();
		float num4 = x;
		float num5 = 0f;
		for (int i = 0; i < num3; i++)
		{
			float num6 = (float)i / (float)num3 * 360f;
			list.Add(new Vector3(Mathf.Sin(num6 * ((float)Math.PI / 180f)) * num4, num5, Mathf.Cos(num6 * ((float)Math.PI / 180f)) * num4));
		}
		float num7 = 4f;
		float num8 = 200f;
		bool flag = true;
		RaycastHit val9 = default(RaycastHit);
		for (int j = 0; j < AI.ocean_patrol_path_iterations && flag; j++)
		{
			flag = false;
			for (int k = 0; k < num3; k++)
			{
				Vector3 val = list[k];
				int index = ((k == 0) ? (num3 - 1) : (k - 1));
				int index2 = ((k != num3 - 1) ? (k + 1) : 0);
				Vector3 val2 = list[index2];
				Vector3 val3 = list[index];
				Vector3 val4 = val;
				Vector3 val5 = Vector3.zero - val;
				Vector3 normalized = ((Vector3)(ref val5)).normalized;
				Vector3 val6 = val + normalized * num7;
				float num9 = Vector3.Distance(val6, val2);
				if (num9 > num8 || Vector3.Distance(val6, val3) > num8)
				{
					continue;
				}
				bool flag2 = true;
				int num10 = 16;
				for (int l = 0; l < num10; l++)
				{
					float num11 = (float)l / (float)num10 * 360f;
					val5 = new Vector3(Mathf.Sin(num11 * ((float)Math.PI / 180f)), num5, Mathf.Cos(num11 * ((float)Math.PI / 180f)));
					Vector3 normalized2 = ((Vector3)(ref val5)).normalized;
					Vector3 val7 = val6 + normalized2 * 1f;
					float waterDepth = GetWaterDepth(val7);
					if (waterDepth < minWaterDepth)
					{
					}
					Vector3 val8 = normalized;
					if (val7 != Vector3.zero)
					{
						val5 = val7 - val6;
						val8 = ((Vector3)(ref val5)).normalized;
					}
					if (Physics.SphereCast(val4, 3f, val8, ref val9, minDistanceFromShore, 1218511105))
					{
						flag2 = false;
						break;
					}
				}
				if (flag2)
				{
					flag = true;
					list[k] = val6;
				}
			}
		}
		if (flag)
		{
			Debug.LogWarning((object)"Failed to generate ocean patrol path");
			return null;
		}
		List<int> list2 = new List<int>();
		LineUtility.Simplify(list, 5f, list2);
		List<Vector3> list3 = list;
		list = new List<Vector3>();
		foreach (int item in list2)
		{
			list.Add(list3[item]);
		}
		Debug.Log((object)("Generated ocean patrol path with node count: " + list.Count));
		return list;
	}
}
