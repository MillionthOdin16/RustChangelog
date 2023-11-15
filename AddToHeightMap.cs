using UnityEngine;

public class AddToHeightMap : ProceduralObject
{
	public bool DestroyGameObject = false;

	public void Apply()
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		Collider component = ((Component)this).GetComponent<Collider>();
		Bounds bounds = component.bounds;
		int num = TerrainMeta.HeightMap.Index(TerrainMeta.NormalizeX(((Bounds)(ref bounds)).min.x));
		int num2 = TerrainMeta.HeightMap.Index(TerrainMeta.NormalizeZ(((Bounds)(ref bounds)).max.x));
		int num3 = TerrainMeta.HeightMap.Index(TerrainMeta.NormalizeX(((Bounds)(ref bounds)).min.z));
		int num4 = TerrainMeta.HeightMap.Index(TerrainMeta.NormalizeZ(((Bounds)(ref bounds)).max.z));
		Vector3 val = default(Vector3);
		Ray val2 = default(Ray);
		RaycastHit val3 = default(RaycastHit);
		for (int i = num3; i <= num4; i++)
		{
			float normZ = TerrainMeta.HeightMap.Coordinate(i);
			for (int j = num; j <= num2; j++)
			{
				float normX = TerrainMeta.HeightMap.Coordinate(j);
				((Vector3)(ref val))._002Ector(TerrainMeta.DenormalizeX(normX), ((Bounds)(ref bounds)).max.y, TerrainMeta.DenormalizeZ(normZ));
				((Ray)(ref val2))._002Ector(val, Vector3.down);
				if (component.Raycast(val2, ref val3, ((Bounds)(ref bounds)).size.y))
				{
					float y = ((RaycastHit)(ref val3)).point.y;
					float num5 = TerrainMeta.NormalizeY(y);
					float height = TerrainMeta.HeightMap.GetHeight01(j, i);
					if (num5 > height)
					{
						TerrainMeta.HeightMap.SetHeight(j, i, num5);
					}
				}
			}
		}
	}

	public override void Process()
	{
		Apply();
		if (DestroyGameObject)
		{
			GameManager.Destroy(((Component)this).gameObject);
		}
		else
		{
			GameManager.Destroy((Component)(object)this);
		}
	}
}
