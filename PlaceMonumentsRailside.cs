using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class PlaceMonumentsRailside : ProceduralComponent
{
	private struct SpawnInfo
	{
		public Prefab<MonumentInfo> prefab;

		public Vector3 position;

		public Quaternion rotation;

		public Vector3 scale;
	}

	private class SpawnInfoGroup
	{
		public bool processed;

		public Prefab<MonumentInfo> prefab;

		public List<SpawnInfo> candidates;
	}

	private struct DistanceInfo
	{
		public float minDistanceSameType;

		public float maxDistanceSameType;

		public float minDistanceDifferentType;

		public float maxDistanceDifferentType;
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

	public int PositionOffset = 100;

	public int TangentInterval = 100;

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

	private static Quaternion rot90 = Quaternion.Euler(0f, 90f, 0f);

	public override void Process(uint seed)
	{
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0301: Unknown result type (might be due to invalid IL or missing references)
		//IL_0306: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_0315: Unknown result type (might be due to invalid IL or missing references)
		//IL_031a: Unknown result type (might be due to invalid IL or missing references)
		//IL_031f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0324: Unknown result type (might be due to invalid IL or missing references)
		//IL_0326: Unknown result type (might be due to invalid IL or missing references)
		//IL_0328: Unknown result type (might be due to invalid IL or missing references)
		//IL_0336: Unknown result type (might be due to invalid IL or missing references)
		//IL_033b: Unknown result type (might be due to invalid IL or missing references)
		//IL_033d: Unknown result type (might be due to invalid IL or missing references)
		//IL_033f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0341: Unknown result type (might be due to invalid IL or missing references)
		//IL_0346: Unknown result type (might be due to invalid IL or missing references)
		//IL_034b: Unknown result type (might be due to invalid IL or missing references)
		//IL_034d: Unknown result type (might be due to invalid IL or missing references)
		//IL_034f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0351: Unknown result type (might be due to invalid IL or missing references)
		//IL_0353: Unknown result type (might be due to invalid IL or missing references)
		//IL_0358: Unknown result type (might be due to invalid IL or missing references)
		//IL_035d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0372: Unknown result type (might be due to invalid IL or missing references)
		//IL_0374: Unknown result type (might be due to invalid IL or missing references)
		//IL_037b: Unknown result type (might be due to invalid IL or missing references)
		//IL_037d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0384: Unknown result type (might be due to invalid IL or missing references)
		//IL_0386: Unknown result type (might be due to invalid IL or missing references)
		//IL_0602: Unknown result type (might be due to invalid IL or missing references)
		//IL_0609: Unknown result type (might be due to invalid IL or missing references)
		//IL_0610: Unknown result type (might be due to invalid IL or missing references)
		//IL_0697: Unknown result type (might be due to invalid IL or missing references)
		//IL_069e: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_062a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0631: Unknown result type (might be due to invalid IL or missing references)
		//IL_0638: Unknown result type (might be due to invalid IL or missing references)
		//IL_0644: Unknown result type (might be due to invalid IL or missing references)
		//IL_0649: Unknown result type (might be due to invalid IL or missing references)
		//IL_064e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0653: Unknown result type (might be due to invalid IL or missing references)
		//IL_0658: Unknown result type (might be due to invalid IL or missing references)
		//IL_065c: Unknown result type (might be due to invalid IL or missing references)
		//IL_065e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0663: Unknown result type (might be due to invalid IL or missing references)
		//IL_066d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0672: Unknown result type (might be due to invalid IL or missing references)
		//IL_0674: Unknown result type (might be due to invalid IL or missing references)
		//IL_0676: Unknown result type (might be due to invalid IL or missing references)
		//IL_067b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0680: Unknown result type (might be due to invalid IL or missing references)
		//IL_0687: Unknown result type (might be due to invalid IL or missing references)
		//IL_06bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_06da: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0706: Unknown result type (might be due to invalid IL or missing references)
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
			_ = TerrainMeta.HeightMap;
			List<Prefab<MonumentInfo>> list = new List<Prefab<MonumentInfo>>();
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				Prefab<MonumentInfo>[] array3 = Prefab.Load<MonumentInfo>(array2[i], (GameManager)null, (PrefabAttribute.Library)null, useProbabilities: true, useWorldConfig: true);
				array3.Shuffle(ref seed);
				list.AddRange(array3);
			}
			Prefab<MonumentInfo>[] array4 = list.ToArray();
			if (array4 == null || array4.Length == 0)
			{
				return;
			}
			array4.BubbleSort();
			SpawnInfoGroup[] array5 = new SpawnInfoGroup[array4.Length];
			for (int j = 0; j < array4.Length; j++)
			{
				Prefab<MonumentInfo> prefab = array4[j];
				SpawnInfoGroup spawnInfoGroup = null;
				for (int k = 0; k < j; k++)
				{
					SpawnInfoGroup spawnInfoGroup2 = array5[k];
					Prefab<MonumentInfo> prefab2 = spawnInfoGroup2.prefab;
					if (prefab == prefab2)
					{
						spawnInfoGroup = spawnInfoGroup2;
						break;
					}
				}
				if (spawnInfoGroup == null)
				{
					spawnInfoGroup = new SpawnInfoGroup();
					spawnInfoGroup.prefab = array4[j];
					spawnInfoGroup.candidates = new List<SpawnInfo>();
				}
				array5[j] = spawnInfoGroup;
			}
			SpawnInfoGroup[] array6 = array5;
			foreach (SpawnInfoGroup spawnInfoGroup3 in array6)
			{
				if (spawnInfoGroup3.processed)
				{
					continue;
				}
				Prefab<MonumentInfo> prefab3 = spawnInfoGroup3.prefab;
				MonumentInfo component = prefab3.Component;
				if ((Object)(object)component == (Object)null || World.Size < component.MinWorldSize)
				{
					continue;
				}
				int num = 0;
				Vector3 val = Vector3.zero;
				Vector3 val2 = Vector3.zero;
				Vector3 val3 = Vector3.zero;
				TerrainPathConnect[] componentsInChildren = prefab3.Object.GetComponentsInChildren<TerrainPathConnect>(true);
				foreach (TerrainPathConnect terrainPathConnect in componentsInChildren)
				{
					if (terrainPathConnect.Type == InfrastructureType.Rail)
					{
						switch (num)
						{
						case 0:
							val2 = ((Component)terrainPathConnect).transform.position;
							break;
						case 1:
							val3 = ((Component)terrainPathConnect).transform.position;
							break;
						}
						val += ((Component)terrainPathConnect).transform.position;
						num++;
					}
				}
				Vector3 val4 = val3 - val2;
				Vector3 normalized = ((Vector3)(ref val4)).normalized;
				Vector3 val5 = rot90 * normalized;
				if (num > 1)
				{
					val /= (float)num;
				}
				if (PositionOffset > 0)
				{
					val += val5 * (float)PositionOffset;
				}
				foreach (PathList rail in TerrainMeta.Path.Rails)
				{
					PathInterpolator path = rail.Path;
					float num2 = TangentInterval / 2;
					float num3 = 5f;
					float num4 = 5f;
					float num5 = path.StartOffset + num4;
					float num6 = path.Length - path.EndOffset - num4;
					for (float num7 = num5; num7 <= num6; num7 += num3)
					{
						Vector3 val6 = (rail.Spline ? path.GetPointCubicHermite(num7) : path.GetPoint(num7));
						val4 = path.GetPoint(num7 + num2) - path.GetPoint(num7 - num2);
						Vector3 normalized2 = ((Vector3)(ref val4)).normalized;
						for (int m = -1; m <= 1; m += 2)
						{
							Quaternion val7 = Quaternion.LookRotation((float)m * Vector3Ex.XZ3D(normalized2));
							Vector3 val8 = val6;
							Quaternion val9 = val7;
							Vector3 localScale = prefab3.Object.transform.localScale;
							val9 *= Quaternion.LookRotation(normalized);
							val8 -= val9 * val;
							SpawnInfo item = default(SpawnInfo);
							item.prefab = prefab3;
							item.position = val8;
							item.rotation = val9;
							item.scale = localScale;
							spawnInfoGroup3.candidates.Add(item);
						}
					}
				}
				spawnInfoGroup3.processed = true;
			}
			int num8 = 0;
			List<SpawnInfo> list2 = new List<SpawnInfo>();
			int num9 = 0;
			List<SpawnInfo> list3 = new List<SpawnInfo>();
			for (int n = 0; n < 8; n++)
			{
				num8 = 0;
				list2.Clear();
				array5.Shuffle(ref seed);
				array6 = array5;
				foreach (SpawnInfoGroup spawnInfoGroup4 in array6)
				{
					Prefab<MonumentInfo> prefab4 = spawnInfoGroup4.prefab;
					MonumentInfo component2 = prefab4.Component;
					if ((Object)(object)component2 == (Object)null || World.Size < component2.MinWorldSize)
					{
						continue;
					}
					DungeonGridInfo dungeonEntrance = component2.DungeonEntrance;
					int num10 = (int)((!Object.op_Implicit((Object)(object)prefab4.Parameters)) ? PrefabPriority.Low : (prefab4.Parameters.Priority + 1));
					int num11 = 100000 * num10 * num10 * num10 * num10;
					int num12 = 0;
					int num13 = 0;
					SpawnInfo item2 = default(SpawnInfo);
					ListEx.Shuffle<SpawnInfo>(spawnInfoGroup4.candidates, ref seed);
					for (int num14 = 0; num14 < spawnInfoGroup4.candidates.Count; num14++)
					{
						SpawnInfo spawnInfo = spawnInfoGroup4.candidates[num14];
						DistanceInfo distanceInfo = GetDistanceInfo(list2, prefab4, spawnInfo.position, spawnInfo.rotation, spawnInfo.scale);
						if (distanceInfo.minDistanceSameType < (float)MinDistanceSameType || distanceInfo.minDistanceDifferentType < (float)MinDistanceDifferentType)
						{
							continue;
						}
						int num15 = num11;
						if (distanceInfo.minDistanceSameType != float.MaxValue)
						{
							if (DistanceSameType == DistanceMode.Min)
							{
								num15 -= Mathf.RoundToInt(distanceInfo.minDistanceSameType * distanceInfo.minDistanceSameType * 2f);
							}
							else if (DistanceSameType == DistanceMode.Max)
							{
								num15 += Mathf.RoundToInt(distanceInfo.minDistanceSameType * distanceInfo.minDistanceSameType * 2f);
							}
						}
						if (distanceInfo.minDistanceDifferentType != float.MaxValue)
						{
							if (DistanceDifferentType == DistanceMode.Min)
							{
								num15 -= Mathf.RoundToInt(distanceInfo.minDistanceDifferentType * distanceInfo.minDistanceDifferentType);
							}
							else if (DistanceDifferentType == DistanceMode.Max)
							{
								num15 += Mathf.RoundToInt(distanceInfo.minDistanceDifferentType * distanceInfo.minDistanceDifferentType);
							}
						}
						if (num15 <= num13 || !prefab4.ApplyTerrainAnchors(ref spawnInfo.position, spawnInfo.rotation, spawnInfo.scale, Filter) || !component2.CheckPlacement(spawnInfo.position, spawnInfo.rotation, spawnInfo.scale))
						{
							continue;
						}
						if (Object.op_Implicit((Object)(object)dungeonEntrance))
						{
							Vector3 val10 = spawnInfo.position + spawnInfo.rotation * Vector3.Scale(spawnInfo.scale, ((Component)dungeonEntrance).transform.position);
							Vector3 val11 = dungeonEntrance.SnapPosition(val10);
							ref Vector3 position = ref spawnInfo.position;
							position += val11 - val10;
							if (!dungeonEntrance.IsValidSpawnPosition(val11))
							{
								continue;
							}
						}
						if (prefab4.ApplyTerrainChecks(spawnInfo.position, spawnInfo.rotation, spawnInfo.scale, Filter) && prefab4.ApplyTerrainFilters(spawnInfo.position, spawnInfo.rotation, spawnInfo.scale) && prefab4.ApplyWaterChecks(spawnInfo.position, spawnInfo.rotation, spawnInfo.scale) && !prefab4.CheckEnvironmentVolumes(spawnInfo.position, spawnInfo.rotation, spawnInfo.scale, EnvironmentType.Underground | EnvironmentType.TrainTunnels))
						{
							num13 = num15;
							item2 = spawnInfo;
							num12++;
							if (num12 >= 8 || DistanceDifferentType == DistanceMode.Any)
							{
								break;
							}
						}
					}
					if (num13 > 0)
					{
						list2.Add(item2);
						num8 += num13;
					}
					if (TargetCount > 0 && list2.Count >= TargetCount)
					{
						break;
					}
				}
				if (num8 > num9)
				{
					num9 = num8;
					GenericsUtil.Swap<List<SpawnInfo>>(ref list2, ref list3);
				}
			}
			foreach (SpawnInfo item3 in list3)
			{
				World.AddPrefab("Monument", item3.prefab, item3.position, item3.rotation, item3.scale);
			}
		}
	}

	private DistanceInfo GetDistanceInfo(List<SpawnInfo> spawns, Prefab<MonumentInfo> prefab, Vector3 monumentPos, Quaternion monumentRot, Vector3 monumentScale)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		DistanceInfo result = default(DistanceInfo);
		result.minDistanceDifferentType = float.MaxValue;
		result.maxDistanceDifferentType = float.MinValue;
		result.minDistanceSameType = float.MaxValue;
		result.maxDistanceSameType = float.MinValue;
		OBB val = default(OBB);
		((OBB)(ref val))._002Ector(monumentPos, monumentScale, monumentRot, prefab.Component.Bounds);
		if ((Object)(object)TerrainMeta.Path != (Object)null)
		{
			foreach (MonumentInfo monument in TerrainMeta.Path.Monuments)
			{
				if (!prefab.Component.HasDungeonLink || (!monument.HasDungeonLink && monument.WantsDungeonLink))
				{
					float num = monument.SqrDistance(val);
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
			if (result.minDistanceDifferentType != float.MaxValue)
			{
				result.minDistanceDifferentType = Mathf.Sqrt(result.minDistanceDifferentType);
			}
			if (result.maxDistanceDifferentType != float.MinValue)
			{
				result.maxDistanceDifferentType = Mathf.Sqrt(result.maxDistanceDifferentType);
			}
		}
		if (spawns != null)
		{
			foreach (SpawnInfo spawn in spawns)
			{
				OBB val2 = new OBB(spawn.position, spawn.scale, spawn.rotation, spawn.prefab.Component.Bounds);
				float num2 = ((OBB)(ref val2)).SqrDistance(val);
				if (num2 < result.minDistanceSameType)
				{
					result.minDistanceSameType = num2;
				}
				if (num2 > result.maxDistanceSameType)
				{
					result.maxDistanceSameType = num2;
				}
			}
			if (prefab.Component.HasDungeonLink)
			{
				foreach (MonumentInfo monument2 in TerrainMeta.Path.Monuments)
				{
					if (monument2.HasDungeonLink || !monument2.WantsDungeonLink)
					{
						float num3 = monument2.SqrDistance(val);
						if (num3 < result.minDistanceSameType)
						{
							result.minDistanceSameType = num3;
						}
						if (num3 > result.maxDistanceSameType)
						{
							result.maxDistanceSameType = num3;
						}
					}
				}
				foreach (DungeonGridInfo dungeonGridEntrance in TerrainMeta.Path.DungeonGridEntrances)
				{
					float num4 = dungeonGridEntrance.SqrDistance(monumentPos);
					if (num4 < result.minDistanceSameType)
					{
						result.minDistanceSameType = num4;
					}
					if (num4 > result.maxDistanceSameType)
					{
						result.maxDistanceSameType = num4;
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
		}
		return result;
	}
}
