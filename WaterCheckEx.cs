using UnityEngine;

public static class WaterCheckEx
{
	public static bool ApplyWaterChecks(this Transform transform, WaterCheck[] anchors, Vector3 pos, Quaternion rot, Vector3 scale)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		if (anchors.Length == 0)
		{
			return true;
		}
		foreach (WaterCheck waterCheck in anchors)
		{
			Vector3 worldPosition = waterCheck.worldPosition;
			Vector3 val = Vector3.Scale(worldPosition, scale);
			if (waterCheck.Rotate)
			{
				val = rot * val;
			}
			Vector3 pos2 = pos + val;
			if (!waterCheck.Check(pos2))
			{
				return false;
			}
		}
		return true;
	}
}
