using UnityEngine;

public class AddToWaterMap : ProceduralObject
{
	public bool isOcean;

	public override void Process()
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = default(Vector3);
		((Vector3)(ref val))._002Ector(10000f, 1000f, 10000f);
		Collider component = ((Component)this).GetComponent<Collider>();
		Bounds val2 = (Bounds)(isOcean ? new Bounds(-val / 2f, val) : component.bounds);
		int num = TerrainMeta.WaterMap.Index(TerrainMeta.NormalizeX(((Bounds)(ref val2)).min.x));
		int num2 = TerrainMeta.WaterMap.Index(TerrainMeta.NormalizeZ(((Bounds)(ref val2)).max.x));
		int num3 = TerrainMeta.WaterMap.Index(TerrainMeta.NormalizeX(((Bounds)(ref val2)).min.z));
		int num4 = TerrainMeta.WaterMap.Index(TerrainMeta.NormalizeZ(((Bounds)(ref val2)).max.z));
		if (isOcean || (component is BoxCollider && ((Component)this).transform.rotation == Quaternion.identity))
		{
			float num5 = TerrainMeta.NormalizeY(((Bounds)(ref val2)).max.y);
			for (int i = num3; i <= num4; i++)
			{
				for (int j = num; j <= num2; j++)
				{
					float height = TerrainMeta.WaterMap.GetHeight01(j, i);
					if (num5 > height)
					{
						TerrainMeta.WaterMap.SetHeight(j, i, num5);
					}
				}
			}
		}
		else
		{
			Vector3 val3 = default(Vector3);
			Ray val4 = default(Ray);
			RaycastHit val5 = default(RaycastHit);
			for (int k = num3; k <= num4; k++)
			{
				float normZ = TerrainMeta.WaterMap.Coordinate(k);
				for (int l = num; l <= num2; l++)
				{
					float normX = TerrainMeta.WaterMap.Coordinate(l);
					((Vector3)(ref val3))._002Ector(TerrainMeta.DenormalizeX(normX), ((Bounds)(ref val2)).max.y + 1f, TerrainMeta.DenormalizeZ(normZ));
					((Ray)(ref val4))._002Ector(val3, Vector3.down);
					if (component.Raycast(val4, ref val5, ((Bounds)(ref val2)).size.y + 1f + 1f))
					{
						float num6 = TerrainMeta.NormalizeY(((RaycastHit)(ref val5)).point.y);
						float height2 = TerrainMeta.WaterMap.GetHeight01(l, k);
						if (num6 > height2)
						{
							TerrainMeta.WaterMap.SetHeight(l, k, num6);
						}
					}
				}
			}
		}
		GameManager.Destroy((Component)(object)this);
	}
}
