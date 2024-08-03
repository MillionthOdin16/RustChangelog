using System;
using ConVar;
using UnityEngine;
using UnityEngine.Profiling;

public class Buoyancy : ListComponent<Buoyancy>, IServerComponent
{
	private struct BuoyancyPointData
	{
		public Transform transform;

		public Vector3 localPosition;

		public Vector3 rootToPoint;

		public Vector3 position;
	}

	public BuoyancyPoint[] points;

	public GameObjectRef[] waterImpacts;

	public Rigidbody rigidBody;

	public float buoyancyScale = 1f;

	public bool doEffects = true;

	public float flowMovementScale = 1f;

	public float requiredSubmergedFraction = 0f;

	public bool useUnderwaterDrag = false;

	[Range(0f, 3f)]
	public float underwaterDrag = 2f;

	[Range(0f, 1f)]
	[Tooltip("How much this object will ignore the waves system, 0 = flat water, 1 = full waves (default 1)")]
	public float flatWaterLerp = 1f;

	public Action<bool> SubmergedChanged = null;

	public BaseEntity forEntity = null;

	[NonSerialized]
	public float submergedFraction = 0f;

	private BuoyancyPointData[] pointData;

	private Vector2[] pointPositionArray;

	private Vector2[] pointPositionUVArray;

	private Vector3[] pointShoreVectorArray;

	private float[] pointTerrainHeightArray;

	private float[] pointWaterHeightArray;

	private float defaultDrag;

	private float defaultAngularDrag;

	private float timeInWater = 0f;

	public float? ArtificialHeight;

	public float timeOutOfWater { get; private set; } = 0f;


	public static string DefaultWaterImpact()
	{
		return "assets/bundled/prefabs/fx/impacts/physics/water-enter-exit.prefab";
	}

	private void Awake()
	{
		((FacepunchBehaviour)this).InvokeRandomized((Action)CheckSleepState, 0.5f, 5f, 1f);
	}

	public void Sleep()
	{
		if ((Object)(object)rigidBody != (Object)null)
		{
			rigidBody.Sleep();
		}
		((Behaviour)this).enabled = false;
	}

	public void Wake()
	{
		if ((Object)(object)rigidBody != (Object)null)
		{
			rigidBody.WakeUp();
		}
		((Behaviour)this).enabled = true;
	}

	public void CheckSleepState()
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		if (!((Object)(object)((Component)this).transform == (Object)null) && !((Object)(object)rigidBody == (Object)null))
		{
			Vector3 position = ((Component)this).transform.position;
			bool flag = BaseNetworkable.HasCloseConnections(position, 100f);
			if (((Behaviour)this).enabled && (rigidBody.IsSleeping() || (!flag && timeInWater > 6f)))
			{
				((FacepunchBehaviour)this).Invoke((Action)Sleep, 0f);
			}
			else if (!((Behaviour)this).enabled && (!rigidBody.IsSleeping() || (flag && timeInWater > 0f)))
			{
				((FacepunchBehaviour)this).Invoke((Action)Wake, 0f);
			}
		}
	}

	protected void DoCycle()
	{
		bool flag = submergedFraction > 0f;
		BuoyancyFixedUpdate();
		bool flag2 = submergedFraction > 0f;
		if (flag == flag2)
		{
			return;
		}
		if (useUnderwaterDrag && (Object)(object)rigidBody != (Object)null)
		{
			if (flag2)
			{
				defaultDrag = rigidBody.drag;
				defaultAngularDrag = rigidBody.angularDrag;
				rigidBody.drag = underwaterDrag;
				rigidBody.angularDrag = underwaterDrag;
			}
			else
			{
				rigidBody.drag = defaultDrag;
				rigidBody.angularDrag = defaultAngularDrag;
			}
		}
		if (SubmergedChanged != null)
		{
			SubmergedChanged(flag2);
		}
	}

	public static void Cycle()
	{
		Buoyancy[] buffer = ListComponent<Buoyancy>.InstanceList.Values.Buffer;
		int count = ListComponent<Buoyancy>.InstanceList.Count;
		for (int i = 0; i < count; i++)
		{
			buffer[i].DoCycle();
		}
	}

	public Vector3 GetFlowDirection(Vector2 posUV)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)TerrainMeta.WaterMap == (Object)null)
		{
			return Vector3.zero;
		}
		Profiler.BeginSample("GetFlowDirection");
		Vector3 normalFast = TerrainMeta.WaterMap.GetNormalFast(posUV);
		float num = Mathf.Clamp01(Mathf.Abs(normalFast.y));
		normalFast.y = 0f;
		Vector3Ex.FastRenormalize(normalFast, num);
		Profiler.EndSample();
		return normalFast;
	}

	public void EnsurePointsInitialized()
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Expected O, but got Unknown
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		if (points == null || points.Length == 0)
		{
			Profiler.BeginSample("Buoyancy.EnsurePointsInitialized");
			Rigidbody component = ((Component)this).GetComponent<Rigidbody>();
			if ((Object)(object)component != (Object)null)
			{
				GameObject val = new GameObject("BuoyancyPoint");
				val.transform.parent = ((Component)component).gameObject.transform;
				val.transform.localPosition = component.centerOfMass;
				BuoyancyPoint buoyancyPoint = val.AddComponent<BuoyancyPoint>();
				buoyancyPoint.buoyancyForce = component.mass * (0f - Physics.gravity.y);
				buoyancyPoint.buoyancyForce *= 1.32f;
				buoyancyPoint.size = 0.2f;
				points = new BuoyancyPoint[1];
				points[0] = buoyancyPoint;
			}
			Profiler.EndSample();
		}
		if (pointData == null || pointData.Length != points.Length)
		{
			pointData = new BuoyancyPointData[points.Length];
			pointPositionArray = (Vector2[])(object)new Vector2[points.Length];
			pointPositionUVArray = (Vector2[])(object)new Vector2[points.Length];
			pointShoreVectorArray = (Vector3[])(object)new Vector3[points.Length];
			pointTerrainHeightArray = new float[points.Length];
			pointWaterHeightArray = new float[points.Length];
			for (int i = 0; i < points.Length; i++)
			{
				Transform transform = ((Component)points[i]).transform;
				Transform parent = transform.parent;
				transform.SetParent(((Component)this).transform);
				Vector3 localPosition = transform.localPosition;
				transform.SetParent(parent);
				pointData[i].transform = transform;
				pointData[i].localPosition = transform.localPosition;
				pointData[i].rootToPoint = localPosition;
			}
		}
	}

	public void BuoyancyFixedUpdate()
	{
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0356: Unknown result type (might be due to invalid IL or missing references)
		//IL_0358: Unknown result type (might be due to invalid IL or missing references)
		//IL_035d: Unknown result type (might be due to invalid IL or missing references)
		//IL_035f: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0374: Unknown result type (might be due to invalid IL or missing references)
		//IL_0376: Unknown result type (might be due to invalid IL or missing references)
		//IL_039a: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0448: Unknown result type (might be due to invalid IL or missing references)
		//IL_044a: Unknown result type (might be due to invalid IL or missing references)
		//IL_044f: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d4: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)TerrainMeta.WaterMap == (Object)null)
		{
			return;
		}
		Profiler.BeginSample("Buoyancy.BuoyancyFixedUpdate");
		EnsurePointsInitialized();
		if ((Object)(object)rigidBody == (Object)null)
		{
			Profiler.EndSample();
			return;
		}
		if (buoyancyScale == 0f)
		{
			Profiler.EndSample();
			((FacepunchBehaviour)this).Invoke((Action)Sleep, 0f);
			return;
		}
		float time = Time.time;
		float x = TerrainMeta.Position.x;
		float z = TerrainMeta.Position.z;
		float x2 = TerrainMeta.OneOverSize.x;
		float z2 = TerrainMeta.OneOverSize.z;
		Matrix4x4 localToWorldMatrix = ((Component)this).transform.localToWorldMatrix;
		for (int i = 0; i < pointData.Length; i++)
		{
			BuoyancyPoint buoyancyPoint = points[i];
			Vector3 val = ((Matrix4x4)(ref localToWorldMatrix)).MultiplyPoint3x4(pointData[i].rootToPoint);
			pointData[i].position = val;
			float num = (val.x - x) * x2;
			float num2 = (val.z - z) * z2;
			pointPositionArray[i] = new Vector2(val.x, val.z);
			pointPositionUVArray[i] = new Vector2(num, num2);
		}
		Profiler.BeginSample("WaterHeight");
		WaterSystem.GetHeightArray(pointPositionArray, pointPositionUVArray, pointShoreVectorArray, pointTerrainHeightArray, pointWaterHeightArray);
		Profiler.EndSample();
		bool flag = flatWaterLerp < 1f;
		int num3 = 0;
		Vector3 val2 = default(Vector3);
		Vector3 val3 = default(Vector3);
		for (int j = 0; j < points.Length; j++)
		{
			BuoyancyPoint buoyancyPoint2 = points[j];
			Vector3 position = pointData[j].position;
			Vector3 localPosition = pointData[j].localPosition;
			Vector2 posUV = pointPositionUVArray[j];
			float terrainHeight = pointTerrainHeightArray[j];
			float waterHeight = pointWaterHeightArray[j];
			if (ArtificialHeight.HasValue)
			{
				waterHeight = ArtificialHeight.Value;
			}
			bool doDeepwaterChecks = !ArtificialHeight.HasValue;
			WaterLevel.WaterInfo buoyancyWaterInfo = WaterLevel.GetBuoyancyWaterInfo(position, posUV, terrainHeight, waterHeight, doDeepwaterChecks, forEntity);
			if (flag && buoyancyWaterInfo.isValid)
			{
				float oceanlevel = Env.oceanlevel;
				buoyancyWaterInfo.currentDepth = (buoyancyWaterInfo.surfaceLevel = Mathf.Lerp(oceanlevel, buoyancyWaterInfo.surfaceLevel, flatWaterLerp)) - position.y;
			}
			bool flag2 = false;
			if (position.y < buoyancyWaterInfo.surfaceLevel && buoyancyWaterInfo.isValid)
			{
				Profiler.BeginSample("Pushing");
				flag2 = true;
				num3++;
				float currentDepth = buoyancyWaterInfo.currentDepth;
				float num4 = Mathf.InverseLerp(0f, buoyancyPoint2.size, currentDepth);
				float num5 = 1f + Mathf.PerlinNoise(buoyancyPoint2.randomOffset + time * buoyancyPoint2.waveFrequency, 0f) * buoyancyPoint2.waveScale;
				float num6 = buoyancyPoint2.buoyancyForce * buoyancyScale;
				((Vector3)(ref val2))._002Ector(0f, num5 * num4 * num6, 0f);
				Vector3 flowDirection = GetFlowDirection(posUV);
				if (flowDirection.y < 0.9999f && flowDirection != Vector3.up)
				{
					num6 *= 0.25f;
					val2.x += flowDirection.x * num6 * flowMovementScale;
					val2.y += flowDirection.y * num6 * flowMovementScale;
					val2.z += flowDirection.z * num6 * flowMovementScale;
				}
				rigidBody.AddForceAtPosition(val2, position, (ForceMode)0);
				Profiler.EndSample();
			}
			if (buoyancyPoint2.doSplashEffects && ((!buoyancyPoint2.wasSubmergedLastFrame && flag2) || (!flag2 && buoyancyPoint2.wasSubmergedLastFrame)) && doEffects)
			{
				Profiler.BeginSample("SplashEffects");
				Vector3 relativePointVelocity = rigidBody.GetRelativePointVelocity(localPosition);
				if (((Vector3)(ref relativePointVelocity)).magnitude > 1f)
				{
					string strName = ((waterImpacts != null && waterImpacts.Length != 0 && waterImpacts[0].isValid) ? waterImpacts[0].resourcePath : DefaultWaterImpact());
					((Vector3)(ref val3))._002Ector(Random.Range(-0.25f, 0.25f), 0f, Random.Range(-0.25f, 0.25f));
					Effect.server.Run(strName, position + val3, Vector3.up);
					buoyancyPoint2.nexSplashTime = Time.time + 0.25f;
				}
				Profiler.EndSample();
			}
			buoyancyPoint2.wasSubmergedLastFrame = flag2;
		}
		if (points.Length != 0)
		{
			submergedFraction = (float)num3 / (float)points.Length;
		}
		if (submergedFraction > requiredSubmergedFraction)
		{
			timeInWater += Time.fixedDeltaTime;
			timeOutOfWater = 0f;
		}
		else
		{
			timeOutOfWater += Time.fixedDeltaTime;
			timeInWater = 0f;
		}
		Profiler.EndSample();
	}
}
