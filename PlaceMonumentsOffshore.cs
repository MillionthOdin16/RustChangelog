using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class PlaceMonumentsOffshore : ProceduralComponent
{
	private struct SpawnInfo
	{
		public Prefab prefab;

		public Vector3 position;

		public Quaternion rotation;

		public Vector3 scale;
	}

	public string ResourceFolder = string.Empty;

	public int TargetCount = 0;

	public int MinDistanceFromTerrain = 100;

	public int MaxDistanceFromTerrain = 500;

	public int DistanceBetweenMonuments = 500;

	[FormerlySerializedAs("MinSize")]
	public int MinWorldSize = 0;

	private const int Candidates = 10;

	private const int Attempts = 10000;

	public override void Process(uint seed)
	{
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0451: Unknown result type (might be due to invalid IL or missing references)
		//IL_0458: Unknown result type (might be due to invalid IL or missing references)
		//IL_045f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		//IL_033c: Unknown result type (might be due to invalid IL or missing references)
		//IL_033e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0340: Unknown result type (might be due to invalid IL or missing references)
		//IL_0357: Unknown result type (might be due to invalid IL or missing references)
		//IL_0359: Unknown result type (might be due to invalid IL or missing references)
		//IL_035b: Unknown result type (might be due to invalid IL or missing references)
		//IL_037f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0381: Unknown result type (might be due to invalid IL or missing references)
		//IL_0388: Unknown result type (might be due to invalid IL or missing references)
		//IL_038a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0391: Unknown result type (might be due to invalid IL or missing references)
		//IL_0393: Unknown result type (might be due to invalid IL or missing references)
		string[] array = (from folder in ResourceFolder.Split(',')
			select "assets/bundled/prefabs/autospawn/" + folder + "/").ToArray();
		if (World.Networked)
		{
			World.Spawn("Monument", array);
		}
		else
		{
			if (World.Size < MinWorldSize)
			{
				return;
			}
			TerrainHeightMap heightMap = TerrainMeta.HeightMap;
			List<Prefab<MonumentInfo>> list = new List<Prefab<MonumentInfo>>();
			string[] array2 = array;
			foreach (string folder2 in array2)
			{
				Prefab<MonumentInfo>[] array3 = Prefab.Load<MonumentInfo>(folder2, (GameManager)null, (PrefabAttribute.Library)null, useProbabilities: true);
				array3.Shuffle(ref seed);
				list.AddRange(array3);
			}
			Prefab<MonumentInfo>[] array4 = list.ToArray();
			if (array4 == null || array4.Length == 0)
			{
				return;
			}
			array4.BubbleSort();
			Vector3 position = TerrainMeta.Position;
			Vector3 size = TerrainMeta.Size;
			float num = position.x - (float)MaxDistanceFromTerrain;
			float num2 = position.x - (float)MinDistanceFromTerrain;
			float num3 = position.x + size.x + (float)MinDistanceFromTerrain;
			float num4 = position.x + size.x + (float)MaxDistanceFromTerrain;
			float num5 = position.z - (float)MaxDistanceFromTerrain;
			float num6 = position.z - (float)MinDistanceFromTerrain;
			float num7 = position.z + size.z + (float)MinDistanceFromTerrain;
			float num8 = position.z + size.z + (float)MaxDistanceFromTerrain;
			int num9 = 0;
			List<SpawnInfo> list2 = new List<SpawnInfo>();
			int num10 = 0;
			List<SpawnInfo> list3 = new List<SpawnInfo>();
			Vector3 pos = default(Vector3);
			for (int j = 0; j < 10; j++)
			{
				num9 = 0;
				list2.Clear();
				Prefab<MonumentInfo>[] array5 = array4;
				foreach (Prefab<MonumentInfo> prefab in array5)
				{
					int num11 = (int)((!Object.op_Implicit((Object)(object)prefab.Parameters)) ? PrefabPriority.Low : (prefab.Parameters.Priority + 1));
					int num12 = num11 * num11 * num11 * num11;
					for (int l = 0; l < 10000; l++)
					{
						float num13 = 0f;
						float num14 = 0f;
						switch (seed % 4)
						{
						case 0u:
							num13 = SeedRandom.Range(ref seed, num, num2);
							num14 = SeedRandom.Range(ref seed, num5, num8);
							break;
						case 1u:
							num13 = SeedRandom.Range(ref seed, num3, num4);
							num14 = SeedRandom.Range(ref seed, num5, num8);
							break;
						case 2u:
							num13 = SeedRandom.Range(ref seed, num, num4);
							num14 = SeedRandom.Range(ref seed, num5, num5);
							break;
						case 3u:
							num13 = SeedRandom.Range(ref seed, num, num4);
							num14 = SeedRandom.Range(ref seed, num7, num8);
							break;
						}
						float normX = TerrainMeta.NormalizeX(num13);
						float normZ = TerrainMeta.NormalizeZ(num14);
						float height = heightMap.GetHeight(normX, normZ);
						((Vector3)(ref pos))._002Ector(num13, height, num14);
						Quaternion rot = prefab.Object.transform.localRotation;
						Vector3 scale = prefab.Object.transform.localScale;
						if (!CheckRadius(list2, pos, DistanceBetweenMonuments))
						{
							prefab.ApplyDecorComponents(ref pos, ref rot, ref scale);
							if ((!Object.op_Implicit((Object)(object)prefab.Component) || prefab.Component.CheckPlacement(pos, rot, scale)) && !prefab.CheckEnvironmentVolumes(pos, rot, scale, EnvironmentType.Underground | EnvironmentType.TrainTunnels))
							{
								SpawnInfo item = default(SpawnInfo);
								item.prefab = prefab;
								item.position = pos;
								item.rotation = rot;
								item.scale = scale;
								list2.Add(item);
								num9 += num12;
								break;
							}
						}
					}
					if (TargetCount > 0 && list2.Count >= TargetCount)
					{
						break;
					}
				}
				if (num9 > num10)
				{
					num10 = num9;
					GenericsUtil.Swap<List<SpawnInfo>>(ref list2, ref list3);
				}
			}
			foreach (SpawnInfo item2 in list3)
			{
				World.AddPrefab("Monument", item2.prefab, item2.position, item2.rotation, item2.scale);
			}
		}
	}

	private bool CheckRadius(List<SpawnInfo> spawns, Vector3 pos, float radius)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		float num = radius * radius;
		foreach (SpawnInfo spawn in spawns)
		{
			Vector3 val = spawn.position - pos;
			float sqrMagnitude = ((Vector3)(ref val)).sqrMagnitude;
			if (sqrMagnitude < num)
			{
				return true;
			}
		}
		return false;
	}
}
