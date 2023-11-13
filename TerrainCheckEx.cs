using UnityEngine;

public static class TerrainCheckEx
{
	public static bool ApplyTerrainChecks(this Transform transform, TerrainCheck[] anchors, Vector3 pos, Quaternion rot, Vector3 scale, SpawnFilter filter = null)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		if (anchors.Length == 0)
		{
			return true;
		}
		foreach (TerrainCheck terrainCheck in anchors)
		{
			Vector3 worldPosition = terrainCheck.worldPosition;
			Vector3 val = Vector3.Scale(worldPosition, scale);
			if (terrainCheck.Rotate)
			{
				val = rot * val;
			}
			Vector3 val2 = pos + val;
			if (TerrainMeta.OutOfBounds(val2))
			{
				return false;
			}
			if (filter != null && filter.GetFactor(val2) == 0f)
			{
				return false;
			}
			if (!terrainCheck.Check(val2))
			{
				return false;
			}
		}
		return true;
	}
}
