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
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		FindFrames(time, out var num, out var num2, out var num3);
		FindSpectra(beaufort, out var num4, out var num5, out var spectrumT);
		return GetDisplacement(simData, position, num, num2, num3, num4, num5, spectrumT);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3 GetDisplacement(Vector3[,,] simData, Vector3 position, int frame0, int frame1, float frameBlend, int spectrum0, int spectrum1, float spectrumBlend)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		float normX = position.x * oneOverOctave0Scale;
		float normZ = position.z * oneOverOctave0Scale;
		return GetDisplacement(simData, normX, normZ, frame0, frame1, frameBlend, spectrum0, spectrum1, spectrumBlend);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3 GetDisplacement(Vector3[,,] simData, float normX, float normZ, int frame0, int frame1, float frameBlend, int spectrum0, int spectrum1, float spectrumBlend)
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		Vector3 zero = Vector3.zero;
		zero = GetDisplacement(position);
		zero = GetDisplacement(position - zero);
		zero = GetDisplacement(position - zero);
		return GetDisplacement(position - zero).y;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Vector3 GetDisplacement(Vector3 position)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		float normX = position.x * oneOverOctave0Scale;
		float normZ = position.z * oneOverOctave0Scale;
		return GetDisplacement(normX, normZ);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public Vector3 GetDisplacement(float normX, float normZ)
	{
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		normX -= Mathf.Floor(normX);
		normZ -= Mathf.Floor(normZ);
		float num = normX * 256f - 0.5f;
		float num2 = normZ * 256f - 0.5f;
		int num3 = Mathf.FloorToInt(num);
		int num4 = Mathf.FloorToInt(num2);
		float num5 = num - (float)num3;
		float num6 = num2 - (float)num4;
		int num7 = num3 % 256;
		int num8 = num4 % 256;
		int x = (num7 + 256) % 256;
		int z = (num8 + 256) % 256;
		int x2 = (num7 + 1 + 256) % 256;
		int z2 = (num8 + 1 + 256) % 256;
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
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		int num = x * 256 + z;
		Vector3 val = Vector3.LerpUnclamped((Vector3)simData[spectrum0, frame0, num], (Vector3)simData[spectrum1, frame0, num], spectrumBlend);
		Vector3 val2 = Vector3.LerpUnclamped((Vector3)simData[spectrum0, frame1, num], (Vector3)simData[spectrum1, frame1, num], spectrumBlend);
		return Vector3.LerpUnclamped(val, val2, frameBlend);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public float GetHeight(Vector3 position)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		float heightAttenuation = GetHeightAttenuation(position);
		return GetHeightRaw(position) * heightAttenuation;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public float GetHeightAttenuation(Vector3 position)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
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
