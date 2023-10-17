using System;
using System.Collections.Generic;
using ConVar;
using Rust;
using UnityEngine;
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
	public bool applyCorrectionForces = true;

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
	public float gasPedal;

	[NonSerialized]
	public float steering;

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
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
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
		if (HasDriver())
		{
			return !IsFlipped();
		}
		return false;
	}

	public override void VehicleFixedUpdate()
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		base.VehicleFixedUpdate();
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
	}

	protected void ApplyCorrectionForces()
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0308: Unknown result type (might be due to invalid IL or missing references)
		//IL_030d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0315: Unknown result type (might be due to invalid IL or missing references)
		//IL_031f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0324: Unknown result type (might be due to invalid IL or missing references)
		//IL_0338: Unknown result type (might be due to invalid IL or missing references)
		if (applyCorrectionForces && planeFitPoints != null && planeFitPoints.Length == 3 && HasDriver() && !(buoyancy.submergedFraction < 0.5f))
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
			Quaternion desiredOrientation = Quaternion.LookRotation(Vector3.Cross(normal, val4), normal);
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
				Vector3 val5 = pidController.ComputeRequiredAngularAcceleration(((Component)this).transform.rotation, desiredOrientation, rigidBody.angularVelocity, Time.fixedDeltaTime);
				rigidBody.AddTorque(val5, (ForceMode)5);
			}
			else if (y2 > num + correctionRange.x)
			{
				pidController.Kp = wavePID.x;
				pidController.Ki = wavePID.y;
				pidController.Kd = wavePID.z;
				Vector3 val6 = pidController.ComputeRequiredAngularAcceleration(((Component)this).transform.rotation, desiredOrientation, rigidBody.angularVelocity, Time.fixedDeltaTime);
				val6.y = 0f;
				rigidBody.AddTorque(val6, (ForceMode)5);
			}
		}
	}

	public static void WaterVehicleDecay(BaseCombatEntity entity, float decayTickRate, float timeSinceLastUsed, float outsideDecayMinutes, float deepWaterDecayMinutes, float decayStartDelayMinutes, bool preventDecayIndoors)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		return TerrainMeta.WaterMap.GetHeight(thrustPoint.position) > thrustPoint.position.y;
	}

	public override float WaterFactorForPlayer(BasePlayer player)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		if (TerrainMeta.WaterMap.GetHeight(player.eyes.position) >= player.eyes.position.y)
		{
			return 1f;
		}
		return 0f;
	}

	public static float GetWaterDepth(Vector3 pos)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		float x = TerrainMeta.Size.x;
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
				if (Vector3.Distance(val6, val2) > num8 || Vector3.Distance(val6, val3) > num8)
				{
					continue;
				}
				bool flag2 = true;
				int num9 = 16;
				for (int l = 0; l < num9; l++)
				{
					float num10 = (float)l / (float)num9 * 360f;
					val5 = new Vector3(Mathf.Sin(num10 * ((float)Math.PI / 180f)), num5, Mathf.Cos(num10 * ((float)Math.PI / 180f)));
					Vector3 normalized2 = ((Vector3)(ref val5)).normalized;
					Vector3 val7 = val6 + normalized2 * 1f;
					GetWaterDepth(val7);
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
