using UnityEngine;

public static class TerrainPlacementEx
{
	public static void ApplyTerrainPlacements(this Transform transform, TerrainPlacement[] placements, Vector3 pos, Quaternion rot, Vector3 scale)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		if (placements.Length != 0)
		{
			Matrix4x4 localToWorld = Matrix4x4.TRS(pos, rot, scale);
			Matrix4x4 inverse = ((Matrix4x4)(ref localToWorld)).inverse;
			for (int i = 0; i < placements.Length; i++)
			{
				placements[i].Apply(localToWorld, inverse);
			}
		}
	}

	public static void ApplyTerrainPlacements(this Transform transform, TerrainPlacement[] placements)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		transform.ApplyTerrainPlacements(placements, transform.position, transform.rotation, transform.lossyScale);
	}
}
