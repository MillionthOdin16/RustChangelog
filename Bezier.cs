using System.Collections.Generic;
using Facepunch;
using UnityEngine;

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
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		if (positions.Length < 2 || slackLevels.Length == 0)
		{
			return;
		}
		bool flag = false;
		for (int i = 0; i < slackLevels.Length; i++)
		{
			if (slackLevels[i] > 0f)
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
					float num = (float)k / (float)tesselationLevel;
					num = Mathx.RemapValClamped(num, 0f, 1f, 0.1f, 0.9f);
					Vector3 item = Vector3.Lerp(Vector3.Lerp(val, val3, num), Vector3.Lerp(val3, val2, num), num);
					result.Add(item);
				}
			}
			else
			{
				result.Add(positions[j]);
			}
		}
	}
}
