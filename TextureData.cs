using UnityEngine;

public struct TextureData
{
	public int width;

	public int height;

	public Color32[] colors;

	public TextureData(Texture2D tex)
	{
		if ((Object)(object)tex != (Object)null)
		{
			width = ((Texture)tex).width;
			height = ((Texture)tex).height;
			colors = tex.GetPixels32();
		}
		else
		{
			width = 0;
			height = 0;
			colors = null;
		}
	}

	public Color32 GetColor(int x, int y)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		return colors[y * width + x];
	}

	public int GetShort(int x, int y)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		return BitUtility.DecodeShort(GetColor(x, y));
	}

	public int GetInt(int x, int y)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		return BitUtility.DecodeInt(GetColor(x, y));
	}

	public float GetFloat(int x, int y)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		return BitUtility.DecodeFloat(GetColor(x, y));
	}

	public float GetHalf(int x, int y)
	{
		return BitUtility.Short2Float(GetShort(x, y));
	}

	public Vector4 GetVector(int x, int y)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		return BitUtility.DecodeVector(GetColor(x, y));
	}

	public Vector3 GetNormal(int x, int y)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		return BitUtility.DecodeNormal(Color32.op_Implicit(GetColor(x, y)));
	}

	public Color32 GetInterpolatedColor(float x, float y)
	{
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		float num = x * (float)(width - 1);
		float num2 = y * (float)(height - 1);
		int num3 = Mathf.Clamp((int)num, 1, width - 2);
		int num4 = Mathf.Clamp((int)num2, 1, height - 2);
		int x2 = Mathf.Min(num3 + 1, width - 2);
		int y2 = Mathf.Min(num4 + 1, height - 2);
		Color val = Color32.op_Implicit(GetColor(num3, num4));
		Color val2 = Color32.op_Implicit(GetColor(x2, num4));
		Color val3 = Color32.op_Implicit(GetColor(num3, y2));
		Color val4 = Color32.op_Implicit(GetColor(x2, y2));
		float num5 = num - (float)num3;
		float num6 = num2 - (float)num4;
		Color val5 = Color.Lerp(val, val2, num5);
		Color val6 = Color.Lerp(val3, val4, num5);
		return Color32.op_Implicit(Color.Lerp(val5, val6, num6));
	}

	public int GetInterpolatedInt(float x, float y)
	{
		float num = x * (float)(width - 1);
		float num2 = y * (float)(height - 1);
		int x2 = Mathf.Clamp(Mathf.RoundToInt(num), 1, width - 2);
		int y2 = Mathf.Clamp(Mathf.RoundToInt(num2), 1, height - 2);
		return GetInt(x2, y2);
	}

	public int GetInterpolatedShort(float x, float y)
	{
		float num = x * (float)(width - 1);
		float num2 = y * (float)(height - 1);
		int x2 = Mathf.Clamp(Mathf.RoundToInt(num), 1, width - 2);
		int y2 = Mathf.Clamp(Mathf.RoundToInt(num2), 1, height - 2);
		return GetShort(x2, y2);
	}

	public float GetInterpolatedFloat(float x, float y)
	{
		float num = x * (float)(width - 1);
		float num2 = y * (float)(height - 1);
		int num3 = Mathf.Clamp((int)num, 1, width - 2);
		int num4 = Mathf.Clamp((int)num2, 1, height - 2);
		int x2 = Mathf.Min(num3 + 1, width - 2);
		int y2 = Mathf.Min(num4 + 1, height - 2);
		float @float = GetFloat(num3, num4);
		float float2 = GetFloat(x2, num4);
		float float3 = GetFloat(num3, y2);
		float float4 = GetFloat(x2, y2);
		float num5 = num - (float)num3;
		float num6 = num2 - (float)num4;
		float num7 = Mathf.Lerp(@float, float2, num5);
		float num8 = Mathf.Lerp(float3, float4, num5);
		return Mathf.Lerp(num7, num8, num6);
	}

	public float GetInterpolatedHalf(float x, float y)
	{
		float num = x * (float)(width - 1);
		float num2 = y * (float)(height - 1);
		int num3 = Mathf.Clamp((int)num, 1, width - 2);
		int num4 = Mathf.Clamp((int)num2, 1, height - 2);
		int x2 = Mathf.Min(num3 + 1, width - 2);
		int y2 = Mathf.Min(num4 + 1, height - 2);
		float half = GetHalf(num3, num4);
		float half2 = GetHalf(x2, num4);
		float half3 = GetHalf(num3, y2);
		float half4 = GetHalf(x2, y2);
		float num5 = num - (float)num3;
		float num6 = num2 - (float)num4;
		float num7 = Mathf.Lerp(half, half2, num5);
		float num8 = Mathf.Lerp(half3, half4, num5);
		return Mathf.Lerp(num7, num8, num6);
	}

	public Vector4 GetInterpolatedVector(float x, float y)
	{
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		float num = x * (float)(width - 1);
		float num2 = y * (float)(height - 1);
		int num3 = Mathf.Clamp((int)num, 1, width - 2);
		int num4 = Mathf.Clamp((int)num2, 1, height - 2);
		int x2 = Mathf.Min(num3 + 1, width - 2);
		int y2 = Mathf.Min(num4 + 1, height - 2);
		Vector4 vector = GetVector(num3, num4);
		Vector4 vector2 = GetVector(x2, num4);
		Vector4 vector3 = GetVector(num3, y2);
		Vector4 vector4 = GetVector(x2, y2);
		float num5 = num - (float)num3;
		float num6 = num2 - (float)num4;
		Vector4 val = Vector4.Lerp(vector, vector2, num5);
		Vector4 val2 = Vector4.Lerp(vector3, vector4, num5);
		return Vector4.Lerp(val, val2, num6);
	}

	public Vector3 GetInterpolatedNormal(float x, float y)
	{
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		float num = x * (float)(width - 1);
		float num2 = y * (float)(height - 1);
		int num3 = Mathf.Clamp((int)num, 1, width - 2);
		int num4 = Mathf.Clamp((int)num2, 1, height - 2);
		int x2 = Mathf.Min(num3 + 1, width - 2);
		int y2 = Mathf.Min(num4 + 1, height - 2);
		Vector3 normal = GetNormal(num3, num4);
		Vector3 normal2 = GetNormal(x2, num4);
		Vector3 normal3 = GetNormal(num3, y2);
		Vector3 normal4 = GetNormal(x2, y2);
		float num5 = num - (float)num3;
		float num6 = num2 - (float)num4;
		Vector3 val = Vector3.Lerp(normal, normal2, num5);
		Vector3 val2 = Vector3.Lerp(normal3, normal4, num5);
		return Vector3.Lerp(val, val2, num6);
	}
}
