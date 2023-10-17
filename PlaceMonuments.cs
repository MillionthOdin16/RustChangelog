using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class PlaceMonuments : ProceduralComponent
{
	private struct SpawnInfo
	{
		public Prefab<MonumentInfo> prefab;

		public Vector3 position;

		public Quaternion rotation;

		public Vector3 scale;

		public bool dungeonEntrance;

		public Vector3 dungeonEntrancePos;
	}

	private struct DistanceInfo
	{
		public float minDistanceSameType;

		public float maxDistanceSameType;

		public float minDistanceDifferentType;

		public float maxDistanceDifferentType;

		public float minDistanceDungeonEntrance;

		public float maxDistanceDungeonEntrance;
	}

	public enum DistanceMode
	{
		Any,
		Min,
		Max
	}

	public SpawnFilter Filter;

	public string ResourceFolder = string.Empty;

	public int TargetCount;

	[FormerlySerializedAs("MinDistance")]
	public int MinDistanceSameType = 500;

	public int MinDistanceDifferentType;

	[FormerlySerializedAs("MinSize")]
	public int MinWorldSize;

	[Tooltip("Distance to monuments of the same type")]
	public DistanceMode DistanceSameType = DistanceMode.Max;

	[Tooltip("Distance to monuments of a different type")]
	public DistanceMode DistanceDifferentType;

	private const int GroupCandidates = 8;

	private const int IndividualCandidates = 8;

	private const int Attempts = 10000;

	private const int MaxDepth = 100000;

	public override void Process(uint seed)
	{
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_066d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0674: Unknown result type (might be due to invalid IL or missing references)
		//IL_067b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_0366: Unknown result type (might be due to invalid IL or missing references)
		//IL_0368: Unknown result type (might be due to invalid IL or missing references)
		//IL_037c: Unknown result type (might be due to invalid IL or missing references)
		//IL_037e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0380: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0395: Unknown result type (might be due to invalid IL or missing references)
		//IL_0397: Unknown result type (might be due to invalid IL or missing references)
		//IL_0399: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0401: Unknown result type (might be due to invalid IL or missing references)
		//IL_0403: Unknown result type (might be due to invalid IL or missing references)
		//IL_0405: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0414: Unknown result type (might be due to invalid IL or missing references)
		//IL_0416: Unknown result type (might be due to invalid IL or missing references)
		//IL_0418: Unknown result type (might be due to invalid IL or missing references)
		//IL_0426: Unknown result type (might be due to invalid IL or missing references)
		//IL_0428: Unknown result type (might be due to invalid IL or missing references)
		//IL_042a: Unknown result type (might be due to invalid IL or missing references)
		//IL_058c: Unknown result type (might be due to invalid IL or missing references)
		//IL_058e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0595: Unknown result type (might be due to invalid IL or missing references)
		//IL_0597: Unknown result type (might be due to invalid IL or missing references)
		//IL_059e: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_051c: Unknown result type (might be due to invalid IL or missing references)
		//IL_051e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0520: Unknown result type (might be due to invalid IL or missing references)
		//IL_0529: Unknown result type (might be due to invalid IL or missing references)
		//IL_052e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0533: Unknown result type (might be due to invalid IL or missing references)
		//IL_0538: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ba: Unknown result type (might be due to invalid IL or missing references)
		string[] array = (from folder in ResourceFolder.Split(',', StringSplitOptions.None)
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
			PathFinder pathFinder = null;
			List<PathFinder.Point> endList = null;
			List<Prefab<MonumentInfo>> list = new List<Prefab<MonumentInfo>>();
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				Prefab<MonumentInfo>[] array3 = Prefab.Load<MonumentInfo>(array2[i], (GameManager)null, (PrefabAttribute.Library)null, useProbabilities: true);
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
			float x = position.x;
			float z = position.z;
			float num = position.x + size.x;
			float num2 = position.z + size.z;
			int num3 = 0;
			List<SpawnInfo> list2 = new List<SpawnInfo>();
			int num4 = 0;
			List<SpawnInfo> list3 = new List<SpawnInfo>();
			Vector3 pos = default(Vector3);
			for (int j = 0; j < 8; j++)
			{
				num3 = 0;
				list2.Clear();
				Prefab<MonumentInfo>[] array5 = array4;
				foreach (Prefab<MonumentInfo> prefab in array5)
				{
					MonumentInfo component = prefab.Component;
					if ((Object)(object)component == (Object)null || World.Size < component.MinWorldSize)
					{
						continue;
					}
					DungeonGridInfo dungeonEntrance = component.DungeonEntrance;
					int num5 = (int)((!Object.op_Implicit((Object)(object)prefab.Parameters)) ? PrefabPriority.Low : (prefab.Parameters.Priority + 1));
					int num6 = 100000 * num5 * num5 * num5 * num5;
					int num7 = 0;
					int num8 = 0;
					SpawnInfo item = default(SpawnInfo);
					for (int k = 0; k < 10000; k++)
					{
						float num9 = SeedRandom.Range(ref seed, x, num);
						float num10 = SeedRandom.Range(ref seed, z, num2);
						float normX = TerrainMeta.NormalizeX(num9);
						float normZ = TerrainMeta.NormalizeZ(num10);
						float num11 = SeedRandom.Value(ref seed);
						float factor = Filter.GetFactor(normX, normZ);
						if (factor * factor < num11)
						{
							continue;
						}
						float height = heightMap.GetHeight(normX, normZ);
						((Vector3)(ref pos))._002Ector(num9, height, num10);
						Quaternion rot = prefab.Object.transform.localRotation;
						Vector3 scale = prefab.Object.transform.localScale;
						Vector3 val = pos;
						prefab.ApplyDecorComponents(ref pos, ref rot, ref scale);
						DistanceInfo distanceInfo = GetDistanceInfo(list2, prefab, pos, rot, scale, val);
						if (distanceInfo.minDistanceSameType < (float)MinDistanceSameType || distanceInfo.minDistanceDifferentType < (float)MinDistanceDifferentType || (Object.op_Implicit((Object)(object)dungeonEntrance) && distanceInfo.minDistanceDungeonEntrance < dungeonEntrance.MinDistance))
						{
							continue;
						}
						int num12 = num6;
						if (distanceInfo.minDistanceSameType != float.MaxValue)
						{
							if (DistanceSameType == DistanceMode.Min)
							{
								num12 -= Mathf.RoundToInt(distanceInfo.minDistanceSameType * distanceInfo.minDistanceSameType * 2f);
							}
							else if (DistanceSameType == DistanceMode.Max)
							{
								num12 += Mathf.RoundToInt(distanceInfo.minDistanceSameType * distanceInfo.minDistanceSameType * 2f);
							}
						}
						if (distanceInfo.minDistanceDifferentType != float.MaxValue)
						{
							if (DistanceDifferentType == DistanceMode.Min)
							{
								num12 -= Mathf.RoundToInt(distanceInfo.minDistanceDifferentType * distanceInfo.minDistanceDifferentType);
							}
							else if (DistanceDifferentType == DistanceMode.Max)
							{
								num12 += Mathf.RoundToInt(distanceInfo.minDistanceDifferentType * distanceInfo.minDistanceDifferentType);
							}
						}
						if (num12 <= num8 || !prefab.ApplyTerrainAnchors(ref pos, rot, scale, Filter) || !component.CheckPlacement(pos, rot, scale))
						{
							continue;
						}
						if (Object.op_Implicit((Object)(object)dungeonEntrance))
						{
							Vector3 val2 = pos + rot * Vector3.Scale(scale, ((Component)dungeonEntrance).transform.position);
							Vector3 val3 = dungeonEntrance.SnapPosition(val2);
							pos += val3 - val2;
							if (!dungeonEntrance.IsValidSpawnPosition(val3))
							{
								continue;
							}
							val = val3;
						}
						if (!prefab.ApplyTerrainChecks(pos, rot, scale, Filter) || !prefab.ApplyTerrainFilters(pos, rot, scale) || !prefab.ApplyWaterChecks(pos, rot, scale) || prefab.CheckEnvironmentVolumes(pos, rot, scale, EnvironmentType.Underground | EnvironmentType.TrainTunnels))
						{
							continue;
						}
						bool flag = false;
						TerrainPathConnect[] componentsInChildren = prefab.Object.GetComponentsInChildren<TerrainPathConnect>(true);
						foreach (TerrainPathConnect terrainPathConnect in componentsInChildren)
						{
							if (terrainPathConnect.Type == InfrastructureType.Boat)
							{
								if (pathFinder == null)
								{
									int[,] array6 = TerrainPath.CreateBoatCostmap(2f);
									int length = array6.GetLength(0);
									pathFinder = new PathFinder(array6);
									endList = new List<PathFinder.Point>
									{
										new PathFinder.Point(0, 0),
										new PathFinder.Point(0, length / 2),
										new PathFinder.Point(0, length - 1),
										new PathFinder.Point(length / 2, 0),
										new PathFinder.Point(length / 2, length - 1),
										new PathFinder.Point(length - 1, 0),
										new PathFinder.Point(length - 1, length / 2),
										new PathFinder.Point(length - 1, length - 1)
									};
								}
								PathFinder.Point pathFinderPoint = terrainPathConnect.GetPathFinderPoint(pathFinder.GetResolution(0), pos + rot * Vector3.Scale(scale, ((Component)terrainPathConnect).transform.localPosition));
								if (pathFinder.FindPathUndirected(new List<PathFinder.Point> { pathFinderPoint }, endList, 100000) == null)
								{
									flag = true;
									break;
								}
							}
						}
						if (!flag)
						{
							SpawnInfo spawnInfo = default(SpawnInfo);
							spawnInfo.prefab = prefab;
							spawnInfo.position = pos;
							spawnInfo.rotation = rot;
							spawnInfo.scale = scale;
							if (Object.op_Implicit((Object)(object)dungeonEntrance))
							{
								spawnInfo.dungeonEntrance = true;
								spawnInfo.dungeonEntrancePos = val;
							}
							num8 = num12;
							item = spawnInfo;
							num7++;
							if (num7 >= 8 || DistanceDifferentType == DistanceMode.Any)
							{
								break;
							}
						}
					}
					if (num8 > 0)
					{
						list2.Add(item);
						num3 += num8;
					}
					if (TargetCount > 0 && list2.Count >= TargetCount)
					{
						break;
					}
				}
				if (num3 > num4)
				{
					num4 = num3;
					GenericsUtil.Swap<List<SpawnInfo>>(ref list2, ref list3);
				}
			}
			foreach (SpawnInfo item2 in list3)
			{
				World.AddPrefab("Monument", item2.prefab, item2.position, item2.rotation, item2.scale);
			}
		}
	}

	private DistanceInfo GetDistanceInfo(List<SpawnInfo> spawns, Prefab<MonumentInfo> prefab, Vector3 monumentPos, Quaternion monumentRot, Vector3 monumentScale, Vector3 dungeonPos)
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		DistanceInfo result = default(DistanceInfo);
		result.minDistanceSameType = float.MaxValue;
		result.maxDistanceSameType = float.MinValue;
		result.minDistanceDifferentType = float.MaxValue;
		result.maxDistanceDifferentType = float.MinValue;
		result.minDistanceDungeonEntrance = float.MaxValue;
		result.maxDistanceDungeonEntrance = float.MinValue;
		OBB val = default(OBB);
		((OBB)(ref val))._002Ector(monumentPos, monumentScale, monumentRot, prefab.Component.Bounds);
		if (spawns != null)
		{
			foreach (SpawnInfo spawn in spawns)
			{
				OBB val2 = new OBB(spawn.position, spawn.scale, spawn.rotation, spawn.prefab.Component.Bounds);
				float num = ((OBB)(ref val2)).SqrDistance(val);
				if (spawn.prefab.Folder == prefab.Folder)
				{
					if (num < result.minDistanceSameType)
					{
						result.minDistanceSameType = num;
					}
					if (num > result.maxDistanceSameType)
					{
						result.maxDistanceSameType = num;
					}
				}
				else
				{
					if (num < result.minDistanceDifferentType)
					{
						result.minDistanceDifferentType = num;
					}
					if (num > result.maxDistanceDifferentType)
					{
						result.maxDistanceDifferentType = num;
					}
				}
			}
			foreach (SpawnInfo spawn2 in spawns)
			{
				if (spawn2.dungeonEntrance)
				{
					Vector3 val3 = spawn2.dungeonEntrancePos - dungeonPos;
					float sqrMagnitude = ((Vector3)(ref val3)).sqrMagnitude;
					if (sqrMagnitude < result.minDistanceDungeonEntrance)
					{
						result.minDistanceDungeonEntrance = sqrMagnitude;
					}
					if (sqrMagnitude > result.maxDistanceDungeonEntrance)
					{
						result.maxDistanceDungeonEntrance = sqrMagnitude;
					}
				}
			}
		}
		if ((Object)(object)TerrainMeta.Path != (Object)null)
		{
			foreach (MonumentInfo monument in TerrainMeta.Path.Monuments)
			{
				float num2 = monument.SqrDistance(val);
				if (num2 < result.minDistanceDifferentType)
				{
					result.minDistanceDifferentType = num2;
				}
				if (num2 > result.maxDistanceDifferentType)
				{
					result.maxDistanceDifferentType = num2;
				}
			}
			foreach (DungeonGridInfo dungeonGridEntrance in TerrainMeta.Path.DungeonGridEntrances)
			{
				float num3 = dungeonGridEntrance.SqrDistance(dungeonPos);
				if (num3 < result.minDistanceDungeonEntrance)
				{
					result.minDistanceDungeonEntrance = num3;
				}
				if (num3 > result.maxDistanceDungeonEntrance)
				{
					result.maxDistanceDungeonEntrance = num3;
				}
			}
		}
		if (result.minDistanceSameType != float.MaxValue)
		{
			result.minDistanceSameType = Mathf.Sqrt(result.minDistanceSameType);
		}
		if (result.maxDistanceSameType != float.MinValue)
		{
			result.maxDistanceSameType = Mathf.Sqrt(result.maxDistanceSameType);
		}
		if (result.minDistanceDifferentType != float.MaxValue)
		{
			result.minDistanceDifferentType = Mathf.Sqrt(result.minDistanceDifferentType);
		}
		if (result.maxDistanceDifferentType != float.MinValue)
		{
			result.maxDistanceDifferentType = Mathf.Sqrt(result.maxDistanceDifferentType);
		}
		if (result.minDistanceDungeonEntrance != float.MaxValue)
		{
			result.minDistanceDungeonEntrance = Mathf.Sqrt(result.minDistanceDungeonEntrance);
		}
		if (result.maxDistanceDungeonEntrance != float.MinValue)
		{
			result.maxDistanceDungeonEntrance = Mathf.Sqrt(result.maxDistanceDungeonEntrance);
		}
		return result;
	}
}
