using System;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;

namespace Rust.Water5;

public class OceanSimulation : IDisposable
{
	public const int octaveCount = 3;

	public const int simulationSize = 256;

	public const int physicsSimulationSize = 256;

	public const int physicsFrameRate = 4;

	public const int physicsLooptime = 18;

	public const int physicsFrameCount = 72;

	public const float phsyicsDeltaTime = 0.25f;

	public const float oneOverPhysicsSimulationSize = 0.00390625f;

	public const int physicsFrameSize = 65536;

	public const int physicsSpectrumOffset = 4718592;

	private OceanSettings oceanSettings;

	private float[] spectrumRanges;

	private float distanceAttenuationFactor;

	private float depthAttenuationFactor;

	private static float oneOverOctave0Scale;

	private static float[] beaufortValues;

	private int spectrum0;

	private int spectrum1;

	private float spectrumBlend;

	private int frame0;

	private int frame1;

	private float frameBlend;

	private float currentTime;

	private float prevUpdateComputeTime;

	private float deltaTime;

	public OceanDisplacementShort3[,,] simData;

	public int Spectrum0 => spectrum0;

	public int Spectrum1 => spectrum1;

	public float SpectrumBlend => spectrumBlend;

	public int Frame0 => frame0;

	public int Frame1 => frame1;

	public float FrameBlend => frameBlend;

	public OceanSimulation(OceanSettings oceanSettings)
	{
		this.oceanSettings = oceanSettings;
		oneOverOctave0Scale = 1f / oceanSettings.octaveScales[0];
		beaufortValues = new float[oceanSettings.spectrumSettings.Length];
		for (int i = 0; i < oceanSettings.spectrumSettings.Length; i++)
		{
			beaufortValues[i] = oceanSettings.spectrumSettings[i].beaufort;
		}
		simData = oceanSettings.LoadSimData();
		spectrumRanges = oceanSettings.spectrumRanges;
		depthAttenuationFactor = oceanSettings.depthAttenuationFactor;
		distanceAttenuationFactor = oceanSettings.distanceAttenuationFactor;
	}

	public void Update(float time, float dt, float beaufort)
	{
		currentTime = time % 18f;
		deltaTime = dt;
		FindFrames(currentTime, out frame0, out frame1, out frameBlend);
		FindSpectra(beaufort, out spectrum0, out spectrum1, out spectrumBlend);
	}

	private static void FindSpectra(float beaufort, out int spectrum0, out int spectrum1, out float spectrumT)
	{
		beaufort = Mathf.Clamp(beaufort, 0f, 10f);
		spectrum0 = (spectrum1 = 0);
		spectrumT = 0f;
		for (int i = 1; i < beaufortValues.Length; i++)
		{
			float num = beaufortValues[i - 1];
			float num2 = beaufortValues[i];
			if (beaufort >= num && beaufort <= num2)
			{
				spectrum0 = i - 1;
				spectrum1 = i;
				spectrumT = math.remap(num, num2, 0f, 1f, beaufort);
				break;
			}
		}
	}

	public static void FindFrames(float time, out int frame0, out int frame1, out float frameBlend)
	{
		frame0 = (int)math.floor(time * 4f);
		frame1 = (int)math.floor(time * 4f);
		frame1 = (frame1 + 1) % 72;
		frameBlend = math.remap((float)frame0 * 0.25f, (float)(frame0 + 1) * 0.25f, 0f, 1f, time);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Trace(Ray ray, float maxDist, out Vector3 result)
	{
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0361: Unknown result type (might be due to invalid IL or missing references)
		//IL_0362: Unknown result type (might be due to invalid IL or missing references)
		//IL_0322: Unknown result type (might be due to invalid IL or missing references)
		//IL_0323: Unknown result type (might be due to invalid IL or missing references)
		//IL_031b: Unknown result type (might be due to invalid IL or missing references)
		//IL_031c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0326: Unknown result type (might be due to invalid IL or missing references)
		float num = Mathf.Lerp(spectrumRanges[spectrum0], spectrumRanges[spectrum1], spectrumBlend);
		if (num <= 0.1f)
		{
			Plane val = default(Plane);
			((Plane)(ref val))._002Ector(Vector3.up, -0f);
			float num2 = default(float);
			if (((Plane)(ref val)).Raycast(ray, ref num2) && num2 < maxDist)
			{
				result = ((Ray)(ref ray)).GetPoint(num2);
				return true;
			}
			result = Vector3.zero;
			return false;
		}
		float num3 = 0f - num;
		Vector3 point = ((Ray)(ref ray)).GetPoint(maxDist);
		if (((Ray)(ref ray)).origin.y > num && point.y > num)
		{
			result = Vector3.zero;
			return false;
		}
		if (((Ray)(ref ray)).origin.y < num3 && point.y < num3)
		{
			result = Vector3.zero;
			return false;
		}
		Vector3 val2 = ((Ray)(ref ray)).origin;
		Vector3 direction = ((Ray)(ref ray)).direction;
		float num4 = 0f;
		float num5 = 0f;
		float num6 = 2f / (math.abs(direction.y) + 1f);
		result = val2;
		if (direction.y <= -0.99f)
		{
			result.y = GetHeight(val2);
			return math.lengthsq(float3.op_Implicit(result - val2)) < maxDist * maxDist;
		}
		if (val2.y >= num + 0f)
		{
			num5 = (num4 = (0f - (val2.y - num - 0f)) / direction.y);
			val2 += num4 * direction;
			if (num5 >= maxDist)
			{
				result = Vector3.zero;
				return false;
			}
		}
		int num7 = 0;
		while (true)
		{
			float height = GetHeight(val2);
			num4 = num6 * (val2.y - height - 0f);
			val2 += num4 * direction;
			num5 += num4;
			if (num7 >= 16 || num4 < 0.1f)
			{
				break;
			}
			if (num5 >= maxDist)
			{
				return false;
			}
			num7++;
		}
		if (num4 < 0.1f)
		{
			result = val2;
			return true;
		}
		if (direction.y < 0f)
		{
			num4 = (0f - (val2.y + num - 0f)) / direction.y;
			Vector3 val3 = val2;
			Vector3 val4 = val2 + num4 * ((Ray)(ref ray)).direction;
			for (int i = 0; i < 16; i++)
			{
				val2 = (val3 + val4) * 0.5f;
				float height2 = GetHeight(val2);
				if (val2.y - height2 - 0f > 0f)
				{
					val3 = val2;
				}
				else
				{
					val4 = val2;
				}
				if (math.abs(val2.y - height2) < 0.1f)
				{
					val2.y = height2;
					break;
				}
			}
			result = val2;
			return true;
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public float MinLevel()
	{
		return 0f - Mathf.Lerp(spectrumRanges[spectrum0], spectrumRanges[spectrum1], spectrumBlend);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public float MaxLevel()
	{
		return Mathf.Lerp(spectrumRanges[spectrum0], spectrumRanges[spectrum1], spectrumBlend);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float GetHeight(Vector3[,,] simData, Vector3 position, float time, float beaufort, float distAttenFactor, float depthAttenFactor)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		float x = TerrainMeta.Position.x;
		float z = TerrainMeta.Position.z;
		float x2 = TerrainMeta.OneOverSize.x;
		float z2 = TerrainMeta.OneOverSize.z;
		float num = (position.x - x) * x2;
		float num2 = (position.z - z) * z2;
		Vector2 uv = default(Vector2);
		((Vector2)(ref uv))._002Ector(num, num2);
		float num3 = (((Object)(object)TerrainTexturing.Instance != (Object)null) ? TerrainTexturing.Instance.GetCoarseDistanceToShore(uv) : 0f);
		float num4 = (((Object)(object)TerrainMeta.HeightMap != (Object)null) ? TerrainMeta.HeightMap.GetHeightFast(uv) : 0f);
		float num5 = Mathf.Clamp01(num3 / distAttenFactor);
		float num6 = Mathf.Clamp01(Mathf.Abs(num4) / depthAttenFactor);
		Vector3 zero = Vector3.zero;
		zero = GetDisplacement(simData, position, time, beaufort);
		zero = GetDisplacement(simData, position - zero, time, beaufort);
		zero = GetDisplacement(simData, position - zero, time, beaufort);
		return GetDisplacement(simData, position - zero, time, beaufort).y * num5 * num6;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3 GetDisplacement(Vector3[,,] simData, Vector3 position, float time, float beaufort)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		FindFrames(time, out var num, out var num2, out var num3);
		FindSpectra(beaufort, out var num4, out var num5, out var spectrumT);
		return GetDisplacement(simData, position, num, num2, num3, num4, num5, spectrumT);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3 GetDisplacement(Vector3[,,] simData, Vector3 position, int frame0, int frame1, float frameBlend, int spectrum0, int spectrum1, float spectrumBlend)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		float normX = position.x * oneOverOctave0Scale;
		float normZ = position.z * oneOverOctave0Scale;
		return GetDisplacement(simData, normX, normZ, frame0, frame1, frameBlend, spectrum0, spectrum1, spectrumBlend);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3 GetDisplacement(Vector3[,,] simData, float normX, float normZ, int frame0, int frame1, float frameBlend, int spectrum0, int spectrum1, float spectrumBlend)
	{
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		normX -= Mathf.Floor(normX);
		normZ -= Mathf.Floor(normZ);
		float num = normX * 256f - 0.5f;
		float num2 = normZ * 256f - 0.5f;
		int num3 = Mathf.FloorToInt(num);
		int num4 = Mathf.FloorToInt(num2);
		float num5 = num - (float)num3;
		float num6 = num2 - (float)num4;
		int x = num3;
		int y = num4;
		int x2 = num3 + 1;
		int y2 = num4 + 1;
		Vector3 displacement = GetDisplacement(simData, x, y, frame0, frame1, frameBlend, spectrum0, spectrum1, spectrumBlend);
		Vector3 displacement2 = GetDisplacement(simData, x2, y, frame0, frame1, frameBlend, spectrum0, spectrum1, spectrumBlend);
		Vector3 displacement3 = GetDisplacement(simData, x, y2, frame0, frame1, frameBlend, spectrum0, spectrum1, spectrumBlend);
		Vector3 displacement4 = GetDisplacement(simData, x2, y2, frame0, frame1, frameBlend, spectrum0, spectrum1, spectrumBlend);
		Vector3 val = Vector3.LerpUnclamped(displacement, displacement2, num5);
		Vector3 val2 = Vector3.LerpUnclamped(displacement3, displacement4, num5);
		return Vector3.LerpUnclamped(val, val2, num6);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3 GetDisplacement(Vector3[,,] simData, int x, int y, int frame0, int frame1, float frameBlend, int spectrum0, int spectrum1, float spectrumBlend)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		int num = x * 256 + y;
		Vector3 val = Vector3.LerpUnclamped(simData[spectrum0, frame0, num], simData[spectrum1, frame0, num], spectrumBlend);
		Vector3 val2 = Vector3.LerpUnclamped(simData[spectrum0, frame1, num], simData[spectrum1, frame1, num], spectrumBlend);
		return Vector3.LerpUnclamped(val, val2, frameBlend);
	}

	public void Dispose()
	{
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public float GetHeightRaw(Vector3 position)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		Vector3 zero = Vector3.zero;
		zero = GetDisplacement(position);
		zero = GetDisplacement(position - zero);
		zero = GetDisplacement(position - zero);
		return GetDisplacement(position - zero).y;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Vector3 GetDisplacement(Vector3 position)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		float normX = position.x * oneOverOctave0Scale;
		float normZ = position.z * oneOverOctave0Scale;
		return GetDisplacement(normX, normZ);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Vector3 GetDisplacement(float normX, float normZ)
	{
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		normX -= Mathf.Floor(normX);
		normZ -= Mathf.Floor(normZ);
		float num = normX * 256f - 0.5f;
		float num2 = normZ * 256f - 0.5f;
		int num3 = Mathf.FloorToInt(num);
		int num4 = Mathf.FloorToInt(num2);
		float num5 = num - (float)num3;
		float num6 = num2 - (float)num4;
		int x = (num3 + 256) % 256;
		int z = (num4 + 256) % 256;
		int x2 = (num3 + 1 + 256) % 256;
		int z2 = (num4 + 1 + 256) % 256;
		Vector3 displacement = GetDisplacement(x, z);
		Vector3 displacement2 = GetDisplacement(x2, z);
		Vector3 displacement3 = GetDisplacement(x, z2);
		Vector3 displacement4 = GetDisplacement(x2, z2);
		Vector3 val = Vector3.LerpUnclamped(displacement, displacement2, num5);
		Vector3 val2 = Vector3.LerpUnclamped(displacement3, displacement4, num5);
		return Vector3.LerpUnclamped(val, val2, num6);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Vector3 GetDisplacement(int x, int z)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		int num = x * 256 + z;
		Vector3 val = Vector3.LerpUnclamped((Vector3)simData[spectrum0, frame0, num], (Vector3)simData[spectrum1, frame0, num], spectrumBlend);
		Vector3 val2 = Vector3.LerpUnclamped((Vector3)simData[spectrum0, frame1, num], (Vector3)simData[spectrum1, frame1, num], spectrumBlend);
		return Vector3.LerpUnclamped(val, val2, frameBlend);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public float GetHeight(Vector3 position)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		float heightAttenuation = GetHeightAttenuation(position);
		return GetHeightRaw(position) * heightAttenuation;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public float GetHeightAttenuation(Vector3 position)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		float x = TerrainMeta.Position.x;
		float z = TerrainMeta.Position.z;
		float x2 = TerrainMeta.OneOverSize.x;
		float z2 = TerrainMeta.OneOverSize.z;
		float num = (position.x - x) * x2;
		float num2 = (position.z - z) * z2;
		Vector2 uv = default(Vector2);
		((Vector2)(ref uv))._002Ector(num, num2);
		float num3 = (((Object)(object)TerrainTexturing.Instance != (Object)null) ? TerrainTexturing.Instance.GetCoarseDistanceToShore(uv) : 0f);
		float num4 = (((Object)(object)TerrainMeta.HeightMap != (Object)null) ? TerrainMeta.HeightMap.GetHeightFast(uv) : 0f);
		float num5 = Mathf.Clamp01(num3 / distanceAttenuationFactor);
		float num6 = Mathf.Clamp01(Mathf.Abs(num4) / depthAttenuationFactor);
		return num5 * num6;
	}
}
