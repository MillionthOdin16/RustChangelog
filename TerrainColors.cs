using UnityEngine;

public class TerrainColors : TerrainExtension
{
	private TerrainSplatMap splatMap;

	private TerrainBiomeMap biomeMap;

	public override void Setup()
	{
		splatMap = ((Component)terrain).GetComponent<TerrainSplatMap>();
		biomeMap = ((Component)terrain).GetComponent<TerrainBiomeMap>();
	}

	public Color GetColor(Vector3 worldPos, int mask = -1)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		return GetColor(normX, normZ, mask);
	}

	public Color GetColor(float normX, float normZ, int mask = -1)
	{
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		float biome = biomeMap.GetBiome(normX, normZ, 1);
		float biome2 = biomeMap.GetBiome(normX, normZ, 2);
		float biome3 = biomeMap.GetBiome(normX, normZ, 4);
		float biome4 = biomeMap.GetBiome(normX, normZ, 8);
		int num = TerrainSplat.TypeToIndex(splatMap.GetSplatMaxType(normX, normZ, mask));
		TerrainConfig.SplatType splatType = config.Splats[num];
		return biome * splatType.AridColor + biome2 * splatType.TemperateColor + biome3 * splatType.TundraColor + biome4 * splatType.ArcticColor;
	}
}
