using System.Runtime.InteropServices;
using UnityEngine;

public class GenerateBiome : ProceduralComponent
{
	[DllImport("RustNative", EntryPoint = "generate_biome")]
	public static extern void Native_GenerateBiome(byte[] map, int res, Vector3 pos, Vector3 size, uint seed, float lootAngle, float biomeAngle, short[] heightmap, int heightres);

	public override void Process(uint seed)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		byte[] dst = TerrainMeta.BiomeMap.dst;
		int res = TerrainMeta.BiomeMap.res;
		Vector3 position = TerrainMeta.Position;
		Vector3 size = TerrainMeta.Size;
		float lootAxisAngle = TerrainMeta.LootAxisAngle;
		float biomeAxisAngle = TerrainMeta.BiomeAxisAngle;
		short[] src = TerrainMeta.HeightMap.src;
		int res2 = TerrainMeta.HeightMap.res;
		Native_GenerateBiome(dst, res, position, size, seed, lootAxisAngle, biomeAxisAngle, src, res2);
	}
}
