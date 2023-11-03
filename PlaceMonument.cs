using UnityEngine;

public class PlaceMonument : ProceduralComponent
{
	private struct SpawnInfo
	{
		public Prefab prefab;

		public Vector3 position;

		public Quaternion rotation;

		public Vector3 scale;
	}

	public SpawnFilter Filter;

	public GameObjectRef Monument;

	private const int Attempts = 10000;

	public override void Process(uint seed)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		if (!Monument.isValid)
		{
			return;
		}
		TerrainHeightMap heightMap = TerrainMeta.HeightMap;
		Vector3 position = TerrainMeta.Position;
		Vector3 size = TerrainMeta.Size;
		float x = position.x;
		float z = position.z;
		float num = position.x + size.x;
		float num2 = position.z + size.z;
		SpawnInfo spawnInfo = default(SpawnInfo);
		int num3 = int.MinValue;
		Prefab<MonumentInfo> prefab = Prefab.Load<MonumentInfo>(Monument.resourceID, (GameManager)null, (PrefabAttribute.Library)null);
		Vector3 pos = default(Vector3);
		for (int i = 0; i < 10000; i++)
		{
			float num4 = SeedRandom.Range(ref seed, x, num);
			float num5 = SeedRandom.Range(ref seed, z, num2);
			float normX = TerrainMeta.NormalizeX(num4);
			float normZ = TerrainMeta.NormalizeZ(num5);
			float num6 = SeedRandom.Value(ref seed);
			float factor = Filter.GetFactor(normX, normZ);
			if (factor * factor < num6)
			{
				continue;
			}
			float height = heightMap.GetHeight(normX, normZ);
			((Vector3)(ref pos))._002Ector(num4, height, num5);
			Quaternion rot = prefab.Object.transform.localRotation;
			Vector3 scale = prefab.Object.transform.localScale;
			prefab.ApplyDecorComponents(ref pos, ref rot, ref scale);
			if ((!Object.op_Implicit((Object)(object)prefab.Component) || prefab.Component.CheckPlacement(pos, rot, scale)) && prefab.ApplyTerrainAnchors(ref pos, rot, scale, Filter) && prefab.ApplyTerrainChecks(pos, rot, scale, Filter) && prefab.ApplyTerrainFilters(pos, rot, scale) && prefab.ApplyWaterChecks(pos, rot, scale) && !prefab.CheckEnvironmentVolumes(pos, rot, scale, EnvironmentType.Underground | EnvironmentType.TrainTunnels))
			{
				SpawnInfo spawnInfo2 = default(SpawnInfo);
				spawnInfo2.prefab = prefab;
				spawnInfo2.position = pos;
				spawnInfo2.rotation = rot;
				spawnInfo2.scale = scale;
				int num7 = -Mathf.RoundToInt(Vector3Ex.Magnitude2D(pos));
				if (num7 > num3)
				{
					num3 = num7;
					spawnInfo = spawnInfo2;
				}
			}
		}
		if (num3 != int.MinValue)
		{
			World.AddPrefab("Monument", spawnInfo.prefab, spawnInfo.position, spawnInfo.rotation, spawnInfo.scale);
		}
	}
}
