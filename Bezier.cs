using System.Collections.Generic;
using Facepunch;
using UnityEngine;
using UnityEngine.Profiling;

public static class Bezier
{
	public static void ApplyLineSlack(ref Vector3[] positions, float[] slackLevels, int tesselationLevel)
	{
		ApplyLineSlack(positions, slackLevels, ref positions, tesselationLevel);
	}

	public static void ApplyLineSlack(Vector3[] positions, float[] slackLevels, ref Vector3[] result, int tesselationLevel)
	{
		List<Vector3> result2 = Pool.GetList<Vector3>();
		ApplyLineSlack(positions, slackLevels, ref result2, tesselationLevel);
		if (result.Length != result2.Count)
		{
			result = (Vector3[])(object)new Vector3[result2.Count];
		}
		result2.CopyTo(result);
		Pool.FreeList<Vector3>(ref result2);
	}

	public static void ApplyLineSlack(Vector3[] positions, float[] slackLevels, ref List<Vector3> result, int tesselationLevel)
	{
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		if (positions.Length < 2 || slackLevels.Length == 0)
		{
			return;
		}
		bool flag = false;
		foreach (float num in slackLevels)
		{
			if (num > 0f)
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			result.AddRange(positions);
			return;
		}
		Profiler.BeginSample("Bezier.ApplySlack");
		for (int j = 0; j < positions.Length; j++)
		{
			if (j < positions.Length - 1)
			{
				Vector3 val = positions[j];
				Vector3 val2 = positions[j + 1];
				Vector3 val3 = Vector3.Lerp(val, val2, 0.5f);
				if (j < slackLevels.Length)
				{
					val3.y -= slackLevels[j];
				}
				result.Add(val);
				for (int k = 0; k < tesselationLevel; k++)
				{
					float num2 = (float)k / (float)tesselationLevel;
					num2 = Mathx.RemapValClamped(num2, 0f, 1f, 0.1f, 0.9f);
					Vector3 item = Vector3.Lerp(Vector3.Lerp(val, val3, num2), Vector3.Lerp(val3, val2, num2), num2);
					result.Add(item);
				}
			}
			else
			{
				result.Add(positions[j]);
			}
		}
		Profiler.EndSample();
	}
}
