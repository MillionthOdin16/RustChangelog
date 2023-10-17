using System;
using UnityEngine;

public static class MapImageRenderer
{
	private readonly struct Array2D<T>
	{
		private readonly T[] _items;

		private readonly int _width;

		private readonly int _height;

		public ref T this[int x, int y]
		{
			get
			{
				int num = Mathf.Clamp(x, 0, _width - 1);
				int num2 = Mathf.Clamp(y, 0, _height - 1);
				return ref _items[num2 * _width + num];
			}
		}

		public Array2D(T[] items, int width, int height)
		{
			_items = items;
			_width = width;
			_height = height;
		}
	}

	private static readonly Vector3 StartColor = new Vector3(0.28627452f, 23f / 85f, 0.24705884f);

	private static readonly Vector4 WaterColor = new Vector4(0.16941601f, 0.31755757f, 0.36200002f, 1f);

	private static readonly Vector4 GravelColor = new Vector4(0.25f, 37f / 152f, 0.22039475f, 1f);

	private static readonly Vector4 DirtColor = new Vector4(0.6f, 0.47959462f, 0.33f, 1f);

	private static readonly Vector4 SandColor = new Vector4(0.7f, 0.65968585f, 0.5277487f, 1f);

	private static readonly Vector4 GrassColor = new Vector4(0.35486364f, 0.37f, 0.2035f, 1f);

	private static readonly Vector4 ForestColor = new Vector4(0.24843751f, 0.3f, 9f / 128f, 1f);

	private static readonly Vector4 RockColor = new Vector4(0.4f, 0.39379844f, 0.37519377f, 1f);

	private static readonly Vector4 SnowColor = new Vector4(0.86274517f, 0.9294118f, 0.94117653f, 1f);

	private static readonly Vector4 PebbleColor = new Vector4(7f / 51f, 0.2784314f, 0.2761563f, 1f);

	private static readonly Vector4 OffShoreColor = new Vector4(0.04090196f, 0.22060032f, 14f / 51f, 1f);

	private static readonly Vector3 SunDirection = Vector3.Normalize(new Vector3(0.95f, 2.87f, 2.37f));

	private const float SunPower = 0.65f;

	private const float Brightness = 1.05f;

	private const float Contrast = 0.94f;

	private const float OceanWaterLevel = 0f;

	private static readonly Vector3 Half = new Vector3(0.5f, 0.5f, 0.5f);

	public static byte[] Render(out int imageWidth, out int imageHeight, out Color background, float scale = 0.5f, bool lossy = true)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		imageWidth = 0;
		imageHeight = 0;
		background = Color.op_Implicit(OffShoreColor);
		TerrainTexturing instance = TerrainTexturing.Instance;
		if ((Object)(object)instance == (Object)null)
		{
			return null;
		}
		Terrain component = ((Component)instance).GetComponent<Terrain>();
		TerrainMeta component2 = ((Component)instance).GetComponent<TerrainMeta>();
		TerrainHeightMap terrainHeightMap = ((Component)instance).GetComponent<TerrainHeightMap>();
		TerrainSplatMap terrainSplatMap = ((Component)instance).GetComponent<TerrainSplatMap>();
		if ((Object)(object)component == (Object)null || (Object)(object)component2 == (Object)null || (Object)(object)terrainHeightMap == (Object)null || (Object)(object)terrainSplatMap == (Object)null)
		{
			return null;
		}
		int mapRes = (int)((float)World.Size * Mathf.Clamp(scale, 0.1f, 4f));
		float invMapRes = 1f / (float)mapRes;
		if (mapRes <= 0)
		{
			return null;
		}
		imageWidth = mapRes + 1000;
		imageHeight = mapRes + 1000;
		Color[] array = (Color[])(object)new Color[imageWidth * imageHeight];
		Array2D<Color> output = new Array2D<Color>(array, imageWidth, imageHeight);
		Parallel.For(0, imageHeight, (Action<int>)delegate(int y)
		{
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_012c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_0135: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0162: Unknown result type (might be due to invalid IL or missing references)
			//IL_017b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0180: Unknown result type (might be due to invalid IL or missing references)
			//IL_0182: Unknown result type (might be due to invalid IL or missing references)
			//IL_0184: Unknown result type (might be due to invalid IL or missing references)
			//IL_0189: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0225: Unknown result type (might be due to invalid IL or missing references)
			//IL_0235: Unknown result type (might be due to invalid IL or missing references)
			//IL_0237: Unknown result type (might be due to invalid IL or missing references)
			//IL_023c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0241: Unknown result type (might be due to invalid IL or missing references)
			//IL_0243: Unknown result type (might be due to invalid IL or missing references)
			//IL_0245: Unknown result type (might be due to invalid IL or missing references)
			//IL_024a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0254: Unknown result type (might be due to invalid IL or missing references)
			//IL_0259: Unknown result type (might be due to invalid IL or missing references)
			//IL_025e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0263: Unknown result type (might be due to invalid IL or missing references)
			//IL_0265: Unknown result type (might be due to invalid IL or missing references)
			//IL_026c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0271: Unknown result type (might be due to invalid IL or missing references)
			//IL_028c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0293: Unknown result type (might be due to invalid IL or missing references)
			//IL_029a: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
			//IL_0216: Unknown result type (might be due to invalid IL or missing references)
			//IL_021b: Unknown result type (might be due to invalid IL or missing references)
			y -= 500;
			float y2 = (float)y * invMapRes;
			int num = mapRes + 500;
			for (int i = -500; i < num; i++)
			{
				float x2 = (float)i * invMapRes;
				Vector3 startColor = StartColor;
				float height = GetHeight(x2, y2);
				Vector3 normal = GetNormal(x2, y2);
				float num2 = Math.Max(Vector3.Dot(normal, SunDirection), 0f);
				startColor = Vector3.Lerp(startColor, Vector4.op_Implicit(GravelColor), GetSplat(x2, y2, 128) * GravelColor.w);
				startColor = Vector3.Lerp(startColor, Vector4.op_Implicit(PebbleColor), GetSplat(x2, y2, 64) * PebbleColor.w);
				startColor = Vector3.Lerp(startColor, Vector4.op_Implicit(RockColor), GetSplat(x2, y2, 8) * RockColor.w);
				startColor = Vector3.Lerp(startColor, Vector4.op_Implicit(DirtColor), GetSplat(x2, y2, 1) * DirtColor.w);
				startColor = Vector3.Lerp(startColor, Vector4.op_Implicit(GrassColor), GetSplat(x2, y2, 16) * GrassColor.w);
				startColor = Vector3.Lerp(startColor, Vector4.op_Implicit(ForestColor), GetSplat(x2, y2, 32) * ForestColor.w);
				startColor = Vector3.Lerp(startColor, Vector4.op_Implicit(SandColor), GetSplat(x2, y2, 4) * SandColor.w);
				startColor = Vector3.Lerp(startColor, Vector4.op_Implicit(SnowColor), GetSplat(x2, y2, 2) * SnowColor.w);
				float num3 = 0f - height;
				if (num3 > 0f)
				{
					startColor = Vector3.Lerp(startColor, Vector4.op_Implicit(WaterColor), Mathf.Clamp(0.5f + num3 / 5f, 0f, 1f));
					startColor = Vector3.Lerp(startColor, Vector4.op_Implicit(OffShoreColor), Mathf.Clamp(num3 / 50f, 0f, 1f));
					num2 = 0.5f;
				}
				startColor += (num2 - 0.5f) * 0.65f * startColor;
				startColor = (startColor - Half) * 0.94f + Half;
				startColor *= 1.05f;
				output[i + 500, y + 500] = new Color(startColor.x, startColor.y, startColor.z);
			}
		});
		background = output[0, 0];
		return EncodeToFile(imageWidth, imageHeight, array, lossy);
		float GetHeight(float x, float y)
		{
			return terrainHeightMap.GetHeight(x, y);
		}
		Vector3 GetNormal(float x, float y)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			return terrainHeightMap.GetNormal(x, y);
		}
		float GetSplat(float x, float y, int mask)
		{
			return terrainSplatMap.GetSplat(x, y, mask);
		}
	}

	private static byte[] EncodeToFile(int width, int height, Color[] pixels, bool lossy)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Expected O, but got Unknown
		Texture2D val = null;
		try
		{
			val = new Texture2D(width, height);
			val.SetPixels(pixels);
			val.Apply();
			return lossy ? ImageConversion.EncodeToJPG(val, 85) : ImageConversion.EncodeToPNG(val);
		}
		finally
		{
			if ((Object)(object)val != (Object)null)
			{
				Object.Destroy((Object)(object)val);
			}
		}
	}

	private static Vector3 UnpackNormal(Vector4 value)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		value.x *= value.w;
		Vector3 val = default(Vector3);
		val.x = value.x * 2f - 1f;
		val.y = value.y * 2f - 1f;
		Vector2 val2 = default(Vector2);
		((Vector2)(ref val2))._002Ector(val.x, val.y);
		val.z = Mathf.Sqrt(1f - Mathf.Clamp(Vector2.Dot(val2, val2), 0f, 1f));
		return val;
	}
}
