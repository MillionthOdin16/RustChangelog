using UnityEngine;

public class SeparableSSS
{
	private static Vector3 Gaussian(float variance, float r, Color falloffColor)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		Vector3 zero = Vector3.zero;
		for (int i = 0; i < 3; i++)
		{
			float num = r / (0.001f + ((Color)(ref falloffColor))[i]);
			((Vector3)(ref zero))[i] = Mathf.Exp((0f - num * num) / (2f * variance)) / (6.28f * variance);
		}
		return zero;
	}

	private static Vector3 Profile(float r, Color falloffColor)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		return 0.1f * Gaussian(0.0484f, r, falloffColor) + 0.118f * Gaussian(0.187f, r, falloffColor) + 0.113f * Gaussian(0.567f, r, falloffColor) + 0.358f * Gaussian(1.99f, r, falloffColor) + 0.078f * Gaussian(7.41f, r, falloffColor);
	}

	public static void CalculateKernel(Color[] target, int targetStart, int targetSize, Color subsurfaceColor, Color falloffColor)
	{
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		int num = targetSize * 2 - 1;
		float num2 = ((num > 20) ? 3f : 2f);
		float num3 = 2f;
		Color[] array = (Color[])(object)new Color[num];
		float num4 = 2f * num2 / (float)(num - 1);
		for (int i = 0; i < num; i++)
		{
			float num5 = 0f - num2 + (float)i * num4;
			float num6 = ((num5 < 0f) ? (-1f) : 1f);
			array[i].a = num2 * num6 * Mathf.Abs(Mathf.Pow(num5, num3)) / Mathf.Pow(num2, num3);
		}
		for (int j = 0; j < num; j++)
		{
			float num7 = ((j > 0) ? Mathf.Abs(array[j].a - array[j - 1].a) : 0f);
			float num8 = ((j < num - 1) ? Mathf.Abs(array[j].a - array[j + 1].a) : 0f);
			float num9 = (num7 + num8) / 2f;
			Vector3 val = num9 * Profile(array[j].a, falloffColor);
			array[j].r = val.x;
			array[j].g = val.y;
			array[j].b = val.z;
		}
		Color val2 = array[num / 2];
		for (int num10 = num / 2; num10 > 0; num10--)
		{
			array[num10] = array[num10 - 1];
		}
		array[0] = val2;
		Vector3 zero = Vector3.zero;
		for (int k = 0; k < num; k++)
		{
			zero.x += array[k].r;
			zero.y += array[k].g;
			zero.z += array[k].b;
		}
		for (int l = 0; l < num; l++)
		{
			array[l].r /= zero.x;
			array[l].g /= zero.y;
			array[l].b /= zero.z;
		}
		target[targetStart] = array[0];
		for (uint num11 = 0u; num11 < targetSize - 1; num11++)
		{
			target[targetStart + num11 + 1] = array[targetSize + num11];
		}
	}
}
