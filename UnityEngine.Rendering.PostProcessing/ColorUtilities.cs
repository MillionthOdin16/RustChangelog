namespace UnityEngine.Rendering.PostProcessing;

public static class ColorUtilities
{
	private const float logC_cut = 0.011361f;

	private const float logC_a = 5.555556f;

	private const float logC_b = 0.047996f;

	private const float logC_c = 0.244161f;

	private const float logC_d = 0.386036f;

	private const float logC_e = 5.301883f;

	private const float logC_f = 0.092819f;

	public static float StandardIlluminantY(float x)
	{
		return 2.87f * x - 3f * x * x - 0.27509508f;
	}

	public static Vector3 CIExyToLMS(float x, float y)
	{
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		float num = 1f;
		float num2 = num * x / y;
		float num3 = num * (1f - x - y) / y;
		float num4 = 0.7328f * num2 + 0.4296f * num - 0.1624f * num3;
		float num5 = -0.7036f * num2 + 1.6975f * num + 0.0061f * num3;
		float num6 = 0.003f * num2 + 0.0136f * num + 0.9834f * num3;
		return new Vector3(num4, num5, num6);
	}

	public static Vector3 ComputeColorBalance(float temperature, float tint)
	{
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		float num = temperature / 60f;
		float num2 = tint / 60f;
		float x = 0.31271f - num * ((num < 0f) ? 0.1f : 0.05f);
		float y = StandardIlluminantY(x) + num2 * 0.05f;
		Vector3 val = default(Vector3);
		((Vector3)(ref val))._002Ector(0.949237f, 1.03542f, 1.08728f);
		Vector3 val2 = CIExyToLMS(x, y);
		return new Vector3(val.x / val2.x, val.y / val2.y, val.z / val2.z);
	}

	public static Vector3 ColorToLift(Vector4 color)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = default(Vector3);
		((Vector3)(ref val))._002Ector(color.x, color.y, color.z);
		float num = val.x * 0.2126f + val.y * 0.7152f + val.z * 0.0722f;
		((Vector3)(ref val))._002Ector(val.x - num, val.y - num, val.z - num);
		float w = color.w;
		return new Vector3(val.x + w, val.y + w, val.z + w);
	}

	public static Vector3 ColorToInverseGamma(Vector4 color)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = default(Vector3);
		((Vector3)(ref val))._002Ector(color.x, color.y, color.z);
		float num = val.x * 0.2126f + val.y * 0.7152f + val.z * 0.0722f;
		((Vector3)(ref val))._002Ector(val.x - num, val.y - num, val.z - num);
		float num2 = color.w + 1f;
		return new Vector3(1f / Mathf.Max(val.x + num2, 0.001f), 1f / Mathf.Max(val.y + num2, 0.001f), 1f / Mathf.Max(val.z + num2, 0.001f));
	}

	public static Vector3 ColorToGain(Vector4 color)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = default(Vector3);
		((Vector3)(ref val))._002Ector(color.x, color.y, color.z);
		float num = val.x * 0.2126f + val.y * 0.7152f + val.z * 0.0722f;
		((Vector3)(ref val))._002Ector(val.x - num, val.y - num, val.z - num);
		float num2 = color.w + 1f;
		return new Vector3(val.x + num2, val.y + num2, val.z + num2);
	}

	public static float LogCToLinear(float x)
	{
		return (x > 0.1530537f) ? ((Mathf.Pow(10f, (x - 0.386036f) / 0.244161f) - 0.047996f) / 5.555556f) : ((x - 0.092819f) / 5.301883f);
	}

	public static float LinearToLogC(float x)
	{
		return (x > 0.011361f) ? (0.244161f * Mathf.Log10(5.555556f * x + 0.047996f) + 0.386036f) : (5.301883f * x + 0.092819f);
	}

	public static uint ToHex(Color c)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		return ((uint)(c.a * 255f) << 24) | ((uint)(c.r * 255f) << 16) | ((uint)(c.g * 255f) << 8) | (uint)(c.b * 255f);
	}

	public static Color ToRGBA(uint hex)
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		return new Color((float)((hex >> 16) & 0xFFu) / 255f, (float)((hex >> 8) & 0xFFu) / 255f, (float)(hex & 0xFFu) / 255f, (float)((hex >> 24) & 0xFFu) / 255f);
	}
}
