using UnityEngine;

public class PlaceDecorValueNoise : ProceduralComponent
{
	public SpawnFilter Filter = null;

	public string ResourceFolder = string.Empty;

	public NoiseParameters Cluster = new NoiseParameters(2, 0.5f, 1f, 0f);

	public float ObjectDensity = 100f;

	public override void Process(uint seed)
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
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
		int num = Mathf.RoundToInt(ObjectDensity * size.x * size.z * 1E-06f);
		float x = position.x;
		float z = position.z;
		float num2 = position.x + size.x;
		float num3 = position.z + size.z;
		float num4 = SeedRandom.Range(ref seed, -1000000f, 1000000f);
		float num5 = SeedRandom.Range(ref seed, -1000000f, 1000000f);
		int octaves = Cluster.Octaves;
		float offset = Cluster.Offset;
		float frequency = Cluster.Frequency * 0.01f;
		float amplitude = Cluster.Amplitude;
		Vector3 pos = default(Vector3);
		for (int i = 0; i < num; i++)
		{
			float num6 = SeedRandom.Range(ref seed, x, num2);
			float num7 = SeedRandom.Range(ref seed, z, num3);
			float normX = TerrainMeta.NormalizeX(num6);
			float normZ = TerrainMeta.NormalizeZ(num7);
			float num8 = SeedRandom.Value(ref seed);
			float factor = Filter.GetFactor(normX, normZ);
			Prefab random = array.GetRandom(ref seed);
			if (factor <= 0f)
			{
				continue;
			}
			float num9 = offset + Noise.Turbulence(num4 + num6, num5 + num7, octaves, frequency, amplitude);
			if (!(num9 * factor * factor < num8))
			{
				float height = heightMap.GetHeight(normX, normZ);
				((Vector3)(ref pos))._002Ector(num6, height, num7);
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
