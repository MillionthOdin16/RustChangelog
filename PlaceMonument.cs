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

	public SpawnFilter Filter = null;

	public GameObjectRef Monument;

	private const int Attempts = 10000;

	public override void Process(uint seed)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
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
