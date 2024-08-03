using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ConVar;
using Rust;
using Rust.Water5;
using UnityEngine;

[ExecuteInEditMode]
public class WaterSystem : MonoBehaviour
{
	[Serializable]
	public class RenderingSettings
	{
		[Serializable]
		public class SkyProbe
		{
			public float ProbeUpdateInterval = 1f;

			public bool TimeSlicing = true;
		}

		[Serializable]
		public class SSR
		{
			public float FresnelCutoff = 0.02f;

			public float ThicknessMin = 1f;

			public float ThicknessMax = 20f;

			public float ThicknessStartDist = 40f;

			public float ThicknessEndDist = 100f;
		}

		public Vector4[] TessellationQuality;

		public SkyProbe SkyReflections;

		public SSR ScreenSpaceReflections;
	}

	private static float oceanLevel = 0f;

	[Header("Ocean Settings")]
	public OceanSettings oceanSettings;

	public OceanSimulation oceanSimulation;

	public WaterQuality Quality = WaterQuality.High;

	public Material oceanMaterial;

	public RenderingSettings Rendering = new RenderingSettings();

	public int patchSize = 100;

	public int patchCount = 4;

	public float patchScale = 1f;

	public static WaterSystem Instance { get; private set; }

	public static WaterCollision Collision { get; private set; }

	public static WaterBody Ocean { get; private set; }

	public static Material OceanMaterial => Instance?.oceanMaterial;

	public static ListHashSet<WaterCamera> WaterCameras { get; } = new ListHashSet<WaterCamera>(8);


	public static HashSet<WaterBody> WaterBodies { get; } = new HashSet<WaterBody>();


	public static HashSet<WaterDepthMask> DepthMasks { get; } = new HashSet<WaterDepthMask>();


	public static float WaveTime { get; private set; }

	public static float OceanLevel
	{
		get
		{
			return oceanLevel;
		}
		set
		{
			value = Mathf.Max(value, 0f);
			if (!Mathf.Approximately(oceanLevel, value))
			{
				oceanLevel = value;
				UpdateOceanLevel();
			}
		}
	}

	public bool IsInitialized { get; private set; }

	public int Layer => ((Component)this).gameObject.layer;

	public int Reflections => Water.reflections;

	public float WindowDirection => oceanSettings.windDirection;

	public float[] OctaveScales => oceanSettings.octaveScales;

	private void CheckInstance()
	{
		Instance = (((Object)(object)Instance != (Object)null) ? Instance : this);
		Collision = (((Object)(object)Collision != (Object)null) ? Collision : ((Component)this).GetComponent<WaterCollision>());
	}

	private void Awake()
	{
		CheckInstance();
	}

	private void OnEnable()
	{
		CheckInstance();
		oceanSimulation = new OceanSimulation(oceanSettings);
		IsInitialized = true;
	}

	private void OnDisable()
	{
		if (!Application.isPlaying || !Application.isQuitting)
		{
			oceanSimulation.Dispose();
			oceanSimulation = null;
			IsInitialized = false;
			Instance = null;
		}
	}

	private void Update()
	{
		TimeWarning val = TimeWarning.New("UpdateWaves", 0);
		try
		{
			UpdateOceanSimulation();
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public static bool Trace(Ray ray, out Vector3 position, float maxDist = 100f)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)Instance == (Object)null)
		{
			position = Vector3.zero;
			return false;
		}
		return Instance.oceanSimulation.Trace(ray, maxDist, out position);
	}

	public static bool Trace(Ray ray, out Vector3 position, out Vector3 normal, float maxDist = 100f)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)Instance == (Object)null)
		{
			position = Vector3.zero;
			normal = Vector3.zero;
			return false;
		}
		normal = Vector3.up;
		return Instance.oceanSimulation.Trace(ray, maxDist, out position);
	}

	public static void GetHeightArray_Managed(Vector2[] pos, Vector2[] posUV, Vector3[] shore, float[] terrainHeight, float[] waterHeight)
	{
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)TerrainTexturing.Instance != (Object)null)
		{
			for (int i = 0; i < posUV.Length; i++)
			{
				shore[i] = (((Object)(object)TerrainTexturing.Instance != (Object)null) ? TerrainTexturing.Instance.GetCoarseVectorToShore(posUV[i]) : Vector3.zero);
			}
		}
		for (int j = 0; j < pos.Length; j++)
		{
			terrainHeight[j] = (((Object)(object)TerrainMeta.HeightMap != (Object)null) ? TerrainMeta.HeightMap.GetHeightFast(posUV[j]) : 0f);
			if ((Object)(object)Instance != (Object)null)
			{
				waterHeight[j] = Instance.oceanSimulation.GetHeight(Vector3Ex.XZ3D(pos[j]));
			}
		}
		float num = OceanLevel;
		for (int k = 0; k < posUV.Length; k++)
		{
			Vector2 uv = posUV[k];
			float num2 = (((Object)(object)TerrainMeta.WaterMap != (Object)null) ? TerrainMeta.WaterMap.GetHeightFast(uv) : 0f);
			if ((Object)(object)Instance != (Object)null && (double)num2 <= (double)num + 0.01)
			{
				waterHeight[k] = num + waterHeight[k];
			}
			else
			{
				waterHeight[k] = num2;
			}
		}
	}

	public static void GetHeightArray(Vector2[] pos, Vector2[] posUV, Vector3[] shore, float[] terrainHeight, float[] waterHeight)
	{
		GetHeightArray_Managed(pos, posUV, shore, terrainHeight, waterHeight);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float GetHeight(Vector3 pos)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)Instance == (Object)null)
		{
			return OceanLevel;
		}
		return Instance.oceanSimulation.GetHeight(pos) + OceanLevel;
	}

	public static Vector3 GetNormal(Vector3 pos)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		return Vector3.up;
	}

	public static float MinLevel()
	{
		if ((Object)(object)Instance == (Object)null)
		{
			return OceanLevel;
		}
		return Instance.oceanSimulation.MinLevel() + OceanLevel;
	}

	public static float MaxLevel()
	{
		if ((Object)(object)Instance == (Object)null)
		{
			return OceanLevel;
		}
		return Instance.oceanSimulation.MaxLevel() + OceanLevel;
	}

	public static void RegisterBody(WaterBody body)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		if (body.Type == WaterBodyType.Ocean)
		{
			if ((Object)(object)Ocean == (Object)null)
			{
				Ocean = body;
				body.Transform.position = Vector3Ex.WithY(body.Transform.position, OceanLevel);
			}
			else if ((Object)(object)Ocean != (Object)(object)body)
			{
				Debug.LogWarning((object)"[Water] Ocean body is already registered. Ignoring call because only one is allowed.");
				return;
			}
		}
		WaterBodies.Add(body);
	}

	public static void UnregisterBody(WaterBody body)
	{
		if ((Object)(object)body == (Object)(object)Ocean)
		{
			Ocean = null;
		}
		WaterBodies.Remove(body);
	}

	private static void UpdateOceanLevel()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)Ocean != (Object)null)
		{
			Ocean.Transform.position = Vector3Ex.WithY(Ocean.Transform.position, OceanLevel);
		}
		foreach (WaterBody waterBody in WaterBodies)
		{
			waterBody.OnOceanLevelChanged(OceanLevel);
		}
	}

	private void UpdateOceanSimulation()
	{
		if (Water.scaled_time)
		{
			WaveTime += Time.deltaTime;
		}
		else
		{
			WaveTime = Time.realtimeSinceStartup;
		}
		if (Weather.ocean_time >= 0f)
		{
			WaveTime = Weather.ocean_time;
		}
		float beaufort = (Object.op_Implicit((Object)(object)SingletonComponent<Climate>.Instance) ? SingletonComponent<Climate>.Instance.WeatherState.OceanScale : 4f);
		oceanSimulation?.Update(WaveTime, Time.deltaTime, beaufort);
	}

	public void Refresh()
	{
		oceanSimulation.Dispose();
		oceanSimulation = new OceanSimulation(oceanSettings);
	}

	private void EditorInitialize()
	{
	}

	private void EditorShutdown()
	{
	}
}
