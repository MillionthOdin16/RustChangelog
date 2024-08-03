using System;
using UnityEngine;
using UnityEngine.Serialization;

public class Buoyancy : ListComponent<Buoyancy>, IServerComponent, IPrefabPreProcess
{
	[Serializable]
	private struct BuoyancyPointData
	{
		[ReadOnly]
		public Vector3 localPosition;

		[ReadOnly]
		public Vector3 rootToPoint;

		[NonSerialized]
		public Vector3 position;
	}

	public BuoyancyPoint[] points;

	public GameObjectRef[] waterImpacts;

	public Rigidbody rigidBody;

	public float buoyancyScale = 1f;

	public bool scaleForceWithMass;

	public bool doEffects = true;

	public float flowMovementScale = 1f;

	public float requiredSubmergedFraction = 0.5f;

	public bool useUnderwaterDrag;

	[Range(0f, 3f)]
	public float underwaterDrag = 2f;

	[Range(0f, 1f)]
	[Tooltip("How much this object will pay attention to the wave system, 0 = flat water, 1 = full waves (default 1)")]
	[FormerlySerializedAs("flatWaterLerp")]
	public float wavesEffect = 1f;

	public Action<bool> SubmergedChanged;

	public BaseEntity forEntity;

	[NonSerialized]
	public float submergedFraction;

	[SerializeField]
	[ReadOnly]
	private BuoyancyPointData[] pointData;

	private bool initedPointArrays;

	private Vector2[] pointPositionArray;

	private Vector2[] pointPositionUVArray;

	private Vector3[] pointShoreVectorArray;

	private float[] pointTerrainHeightArray;

	private float[] pointWaterHeightArray;

	private float defaultDrag;

	private float defaultAngularDrag;

	private float timeInWater;

	[NonSerialized]
	public float? ArtificialHeight;

	private BaseVehicle forVehicle;

	private bool hasLocalPlayers;

	private bool hadLocalPlayers;

	public float timeOutOfWater { get; private set; }

	public bool InWater => submergedFraction > requiredSubmergedFraction;

	public void PreProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		if (!Application.isPlaying || serverside)
		{
			SavePointData();
		}
	}

	public void SavePointData()
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		if (points == null || points.Length == 0)
		{
			Rigidbody val = ((Component)this).GetComponent<Rigidbody>();
			if ((Object)(object)val == (Object)null)
			{
				val = ((Component)this).gameObject.AddComponent<Rigidbody>();
			}
			GameObject val2 = new GameObject("BuoyancyPoint");
			val2.transform.parent = ((Component)val).gameObject.transform;
			val2.transform.localPosition = val.centerOfMass;
			BuoyancyPoint buoyancyPoint = val2.AddComponent<BuoyancyPoint>();
			buoyancyPoint.buoyancyForce = val.mass * (0f - Physics.gravity.y);
			buoyancyPoint.buoyancyForce *= 1.32f;
			buoyancyPoint.size = 0.2f;
			points = new BuoyancyPoint[1];
			points[0] = buoyancyPoint;
		}
		if (pointData == null || pointData.Length != points.Length)
		{
			pointData = new BuoyancyPointData[points.Length];
			for (int i = 0; i < points.Length; i++)
			{
				Transform transform = ((Component)points[i]).transform;
				pointData[i].localPosition = transform.localPosition;
				pointData[i].rootToPoint = ((Component)this).transform.InverseTransformPoint(transform.position);
			}
		}
	}

	public static string DefaultWaterImpact()
	{
		return "assets/bundled/prefabs/fx/impacts/physics/water-enter-exit.prefab";
	}

	private void Awake()
	{
		forVehicle = forEntity as BaseVehicle;
		((FacepunchBehaviour)this).InvokeRandomized((Action)CheckSleepState, 0.5f, 5f, 1f);
	}

	public void Sleep()
	{
		if (((Object)(object)forEntity == (Object)null || !forEntity.BuoyancySleep(InWater)) && (Object)(object)rigidBody != (Object)null)
		{
			rigidBody.Sleep();
		}
		((Behaviour)this).enabled = false;
	}

	public void Wake()
	{
		if (((Object)(object)forEntity == (Object)null || !forEntity.BuoyancyWake()) && (Object)(object)rigidBody != (Object)null)
		{
			rigidBody.WakeUp();
		}
		((Behaviour)this).enabled = true;
	}

	public void CheckSleepState()
	{
		if ((Object)(object)((Component)this).transform == (Object)null || (Object)(object)rigidBody == (Object)null)
		{
			return;
		}
		hasLocalPlayers = HasLocalPlayers();
		bool flag = rigidBody.IsSleeping() || rigidBody.isKinematic;
		bool flag2 = flag || (!hasLocalPlayers && timeInWater > 6f);
		if ((Object)(object)forVehicle != (Object)null && forVehicle.IsOn())
		{
			flag2 = false;
		}
		if (((Behaviour)this).enabled && flag2)
		{
			((FacepunchBehaviour)this).Invoke((Action)Sleep, 0f);
			return;
		}
		if (!((Behaviour)this).enabled && hasLocalPlayers && !hadLocalPlayers)
		{
			DoCycle(forced: true);
		}
		bool flag3 = !flag || ShouldWake(hasLocalPlayers);
		if (!((Behaviour)this).enabled && flag3)
		{
			((FacepunchBehaviour)this).Invoke((Action)Wake, 0f);
		}
		hadLocalPlayers = hasLocalPlayers;
	}

	public bool ShouldWake()
	{
		return ShouldWake(HasLocalPlayers());
	}

	public bool ShouldWake(bool hasLocalPlayers)
	{
		if (hasLocalPlayers)
		{
			return submergedFraction > 0f;
		}
		return false;
	}

	private bool HasLocalPlayers()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return BaseNetworkable.HasCloseConnections(((Component)this).transform.position, 100f);
	}

	protected void DoCycle(bool forced = false)
	{
		if (!((Behaviour)this).enabled && !forced)
		{
			return;
		}
		bool num = submergedFraction > 0f;
		BuoyancyFixedUpdate();
		bool flag = submergedFraction > 0f;
		if (num == flag)
		{
			return;
		}
		if (useUnderwaterDrag && (Object)(object)rigidBody != (Object)null)
		{
			if (flag)
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
			SubmergedChanged(flag);
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
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)TerrainMeta.WaterMap == (Object)null)
		{
			return Vector3.zero;
		}
		Vector3 normalFast = TerrainMeta.WaterMap.GetNormalFast(posUV);
		float num = Mathf.Clamp01(Mathf.Abs(normalFast.y));
		normalFast.y = 0f;
		Vector3Ex.FastRenormalize(normalFast, num);
		return normalFast;
	}

	public void BuoyancyFixedUpdate()
	{
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0308: Unknown result type (might be due to invalid IL or missing references)
		//IL_030a: Unknown result type (might be due to invalid IL or missing references)
		//IL_030f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0311: Unknown result type (might be due to invalid IL or missing references)
		//IL_03da: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0391: Unknown result type (might be due to invalid IL or missing references)
		//IL_0393: Unknown result type (might be due to invalid IL or missing references)
		//IL_031f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0321: Unknown result type (might be due to invalid IL or missing references)
		//IL_0340: Unknown result type (might be due to invalid IL or missing references)
		//IL_035c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0378: Unknown result type (might be due to invalid IL or missing references)
		//IL_0452: Unknown result type (might be due to invalid IL or missing references)
		//IL_0454: Unknown result type (might be due to invalid IL or missing references)
		//IL_0456: Unknown result type (might be due to invalid IL or missing references)
		//IL_045b: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)TerrainMeta.WaterMap == (Object)null || (Object)(object)rigidBody == (Object)null)
		{
			return;
		}
		if (buoyancyScale == 0f)
		{
			((FacepunchBehaviour)this).Invoke((Action)Sleep, 0f);
			return;
		}
		if (!initedPointArrays)
		{
			InitPointArrays();
		}
		float time = Time.time;
		float x = TerrainMeta.Position.x;
		float z = TerrainMeta.Position.z;
		float x2 = TerrainMeta.OneOverSize.x;
		float z2 = TerrainMeta.OneOverSize.z;
		Matrix4x4 localToWorldMatrix = ((Component)this).transform.localToWorldMatrix;
		for (int i = 0; i < pointData.Length; i++)
		{
			Vector3 val = ((Matrix4x4)(ref localToWorldMatrix)).MultiplyPoint3x4(pointData[i].rootToPoint);
			pointData[i].position = val;
			float num = (val.x - x) * x2;
			float num2 = (val.z - z) * z2;
			pointPositionArray[i] = new Vector2(val.x, val.z);
			pointPositionUVArray[i] = new Vector2(num, num2);
		}
		WaterSystem.GetHeightArray(pointPositionArray, pointPositionUVArray, pointShoreVectorArray, pointTerrainHeightArray, pointWaterHeightArray);
		bool flag = wavesEffect < 1f;
		int num3 = 0;
		Vector3 val2 = default(Vector3);
		Vector3 val3 = default(Vector3);
		for (int j = 0; j < points.Length; j++)
		{
			BuoyancyPoint buoyancyPoint = points[j];
			Vector3 position = pointData[j].position;
			Vector3 localPosition = pointData[j].localPosition;
			Vector2 posUV = pointPositionUVArray[j];
			float terrainHeight = pointTerrainHeightArray[j];
			float num4 = pointWaterHeightArray[j];
			if (ArtificialHeight.HasValue)
			{
				num4 = ArtificialHeight.Value;
			}
			else if (flag)
			{
				num4 = Mathf.Lerp(0f, num4, wavesEffect);
			}
			bool doDeepwaterChecks = !ArtificialHeight.HasValue;
			WaterLevel.WaterInfo buoyancyWaterInfo = WaterLevel.GetBuoyancyWaterInfo(position, posUV, terrainHeight, num4, doDeepwaterChecks, forEntity);
			if (flag && buoyancyWaterInfo.isValid)
			{
				buoyancyWaterInfo.currentDepth = Mathf.Lerp(buoyancyWaterInfo.currentDepth, buoyancyWaterInfo.surfaceLevel - position.y, wavesEffect);
			}
			bool flag2 = false;
			if (position.y < buoyancyWaterInfo.surfaceLevel && buoyancyWaterInfo.isValid)
			{
				flag2 = true;
				num3++;
				float currentDepth = buoyancyWaterInfo.currentDepth;
				float num5 = Mathf.InverseLerp(0f, buoyancyPoint.size, currentDepth);
				float num6 = 1f + Mathf.PerlinNoise(buoyancyPoint.randomOffset + time * buoyancyPoint.waveFrequency, 0f) * buoyancyPoint.waveScale;
				float num7 = buoyancyPoint.buoyancyForce * buoyancyScale;
				if (scaleForceWithMass)
				{
					num7 *= rigidBody.mass;
				}
				((Vector3)(ref val2))._002Ector(0f, num6 * num5 * num7, 0f);
				Vector3 flowDirection = GetFlowDirection(posUV);
				if (flowDirection.y < 0.9999f && flowDirection != Vector3.up)
				{
					num7 *= 0.25f;
					val2.x += flowDirection.x * num7 * flowMovementScale;
					val2.y += flowDirection.y * num7 * flowMovementScale;
					val2.z += flowDirection.z * num7 * flowMovementScale;
				}
				rigidBody.AddForceAtPosition(val2, position, (ForceMode)0);
			}
			if (buoyancyPoint.doSplashEffects && ((!buoyancyPoint.wasSubmergedLastFrame && flag2) || (!flag2 && buoyancyPoint.wasSubmergedLastFrame)) && doEffects)
			{
				Vector3 relativePointVelocity = rigidBody.GetRelativePointVelocity(localPosition);
				if (((Vector3)(ref relativePointVelocity)).magnitude > 1f)
				{
					string strName = ((waterImpacts != null && waterImpacts.Length != 0 && waterImpacts[0].isValid) ? waterImpacts[0].resourcePath : DefaultWaterImpact());
					((Vector3)(ref val3))._002Ector(Random.Range(-0.25f, 0.25f), 0f, Random.Range(-0.25f, 0.25f));
					Effect.server.Run(strName, position + val3, Vector3.up);
					buoyancyPoint.nexSplashTime = Time.time + 0.25f;
				}
			}
			buoyancyPoint.wasSubmergedLastFrame = flag2;
		}
		if (points.Length != 0)
		{
			submergedFraction = (float)num3 / (float)points.Length;
		}
		if (InWater)
		{
			timeInWater += Time.fixedDeltaTime;
			timeOutOfWater = 0f;
		}
		else
		{
			timeOutOfWater += Time.fixedDeltaTime;
			timeInWater = 0f;
		}
	}

	private void InitPointArrays()
	{
		pointPositionArray = (Vector2[])(object)new Vector2[points.Length];
		pointPositionUVArray = (Vector2[])(object)new Vector2[points.Length];
		pointShoreVectorArray = (Vector3[])(object)new Vector3[points.Length];
		pointTerrainHeightArray = new float[points.Length];
		pointWaterHeightArray = new float[points.Length];
		initedPointArrays = true;
	}
}
