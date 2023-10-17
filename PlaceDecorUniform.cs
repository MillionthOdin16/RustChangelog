using UnityEngine;

public class PlaceDecorUniform : ProceduralComponent
{
	public SpawnFilter Filter = null;

	public string ResourceFolder = string.Empty;

	public float ObjectDistance = 10f;

	public float ObjectDithering = 5f;

	public override void Process(uint seed)
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		if (World.Networked)
		{
			World.Spawn("Decor", "assets/bundled/prefabs/autospawn/" + ResourceFolder + "/");
			return;
		}
		TerrainHeightMap heightMap = TerrainMeta.HeightMap;
		Prefab[] array = Prefab.Load("assets/bundled/prefabs/autospawn/" + ResourceFolder);
		if (array == null || array.Length == 0)
		{
			return;
		}
		Vector3 position = TerrainMeta.Position;
		Vector3 size = TerrainMeta.Size;
		float x = position.x;
		float z = position.z;
		float num = position.x + size.x;
		float num2 = position.z + size.z;
		Vector3 pos = default(Vector3);
		for (float num3 = z; num3 < num2; num3 += ObjectDistance)
		{
			for (float num4 = x; num4 < num; num4 += ObjectDistance)
			{
				float num5 = num4 + SeedRandom.Range(ref seed, 0f - ObjectDithering, ObjectDithering);
				float num6 = num3 + SeedRandom.Range(ref seed, 0f - ObjectDithering, ObjectDithering);
				float normX = TerrainMeta.NormalizeX(num5);
				float normZ = TerrainMeta.NormalizeZ(num6);
				float num7 = SeedRandom.Value(ref seed);
				float factor = Filter.GetFactor(normX, normZ);
				Prefab random = array.GetRandom(ref seed);
				if (!(factor * factor < num7))
				{
					float height = heightMap.GetHeight(normX, normZ);
					((Vector3)(ref pos))._002Ector(num5, height, num6);
					Quaternion rot = random.Object.transform.localRotation;
					Vector3 scale = random.Object.transform.localScale;
					random.ApplyDecorComponents(ref pos, ref rot, ref scale);
					if (random.ApplyTerrainAnchors(ref pos, rot, scale, Filter) && random.ApplyTerrainChecks(pos, rot, scale, Filter) && random.ApplyTerrainFilters(pos, rot, scale) && random.ApplyWaterChecks(pos, rot, scale))
					{
						World.AddPrefab("Decor", random, pos, rot, scale);
					}
				}
			}
		}
	}
}
