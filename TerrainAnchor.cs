using System;
using UnityEngine;

public class TerrainAnchor : PrefabAttribute
{
	public float Extents = 1f;

	public float Offset = 0f;

	public float Radius = 0f;

	public void Apply(out float height, out float min, out float max, Vector3 pos, Vector3 scale)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		float num = Extents * scale.y;
		float num2 = Offset * scale.y;
		height = TerrainMeta.HeightMap.GetHeight(pos);
		min = height - num2 - num;
		max = height - num2 + num;
		if (!(Radius > 0f))
		{
			return;
		}
		int num3 = TerrainMeta.HeightMap.Index(TerrainMeta.NormalizeX(pos.x - Radius));
		int num4 = TerrainMeta.HeightMap.Index(TerrainMeta.NormalizeX(pos.x + Radius));
		int num5 = TerrainMeta.HeightMap.Index(TerrainMeta.NormalizeZ(pos.z - Radius));
		int num6 = TerrainMeta.HeightMap.Index(TerrainMeta.NormalizeZ(pos.z + Radius));
		for (int i = num5; i <= num6; i++)
		{
			if (max < min)
			{
				break;
			}
			for (int j = num3; j <= num4; j++)
			{
				if (max < min)
				{
					break;
				}
				float height2 = TerrainMeta.HeightMap.GetHeight(j, i);
				min = Mathf.Max(min, height2 - num2 - num);
				max = Mathf.Min(max, height2 - num2 + num);
			}
		}
	}

	protected override Type GetIndexedType()
	{
		return typeof(TerrainAnchor);
	}
}
