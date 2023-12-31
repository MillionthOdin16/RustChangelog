using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class PlaceMonumentsRoadside : ProceduralComponent
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

	public enum RoadMode
	{
		SideRoadOrRingRoad,
		SideRoad,
		RingRoad,
		SideRoadOrDesireTrail,
		DesireTrail
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

	public RoadMode RoadType;

	private const int GroupCandidates = 8;

	private const int IndividualCandidates = 8;

	private static Quaternion rot90 = Quaternion.Euler(0f, 90f, 0f);

	public override void Process(uint seed)
	{
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0806: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_0505: Unknown result type (might be due to invalid IL or missing references)
		//IL_050c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0513: Unknown result type (might be due to invalid IL or missing references)
		//IL_0325: Unknown result type (might be due to invalid IL or missing references)
		//IL_031a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0611: Unknown result type (might be due to invalid IL or missing references)
		//IL_0618: Unknown result type (might be due to invalid IL or missing references)
		//IL_032a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0330: Unknown result type (might be due to invalid IL or missing references)
		//IL_0335: Unknown result type (might be due to invalid IL or missing references)
		//IL_0631: Unknown result type (might be due to invalid IL or missing references)
		//IL_0638: Unknown result type (might be due to invalid IL or missing references)
		//IL_063f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0342: Unknown result type (might be due to invalid IL or missing references)
		//IL_0344: Unknown result type (might be due to invalid IL or missing references)
		//IL_0349: Unknown result type (might be due to invalid IL or missing references)
		//IL_034e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0353: Unknown result type (might be due to invalid IL or missing references)
		//IL_0355: Unknown result type (might be due to invalid IL or missing references)
		//IL_0357: Unknown result type (might be due to invalid IL or missing references)
		//IL_0365: Unknown result type (might be due to invalid IL or missing references)
		//IL_036a: Unknown result type (might be due to invalid IL or missing references)
		//IL_036c: Unknown result type (might be due to invalid IL or missing references)
		//IL_036e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0370: Unknown result type (might be due to invalid IL or missing references)
		//IL_0375: Unknown result type (might be due to invalid IL or missing references)
		//IL_037a: Unknown result type (might be due to invalid IL or missing references)
		//IL_037c: Unknown result type (might be due to invalid IL or missing references)
		//IL_037e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0380: Unknown result type (might be due to invalid IL or missing references)
		//IL_0382: Unknown result type (might be due to invalid IL or missing references)
		//IL_0387: Unknown result type (might be due to invalid IL or missing references)
		//IL_038c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0659: Unknown result type (might be due to invalid IL or missing references)
		//IL_0660: Unknown result type (might be due to invalid IL or missing references)
		//IL_0667: Unknown result type (might be due to invalid IL or missing references)
		//IL_0673: Unknown result type (might be due to invalid IL or missing references)
		//IL_0678: Unknown result type (might be due to invalid IL or missing references)
		//IL_067d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0682: Unknown result type (might be due to invalid IL or missing references)
		//IL_0687: Unknown result type (might be due to invalid IL or missing references)
		//IL_068b: Unknown result type (might be due to invalid IL or missing references)
		//IL_068d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0692: Unknown result type (might be due to invalid IL or missing references)
		//IL_069c: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_06aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_06af: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0709: Unknown result type (might be due to invalid IL or missing references)
		//IL_0710: Unknown result type (might be due to invalid IL or missing references)
		//IL_0717: Unknown result type (might be due to invalid IL or missing references)
		//IL_0727: Unknown result type (might be due to invalid IL or missing references)
		//IL_072e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0735: Unknown result type (might be due to invalid IL or missing references)
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
			foreach (string text in array2)
			{
				if (!text.Contains("tunnel-entrance") || World.Config.BelowGroundRails)
				{
					Prefab<MonumentInfo>[] array3 = Prefab.Load<MonumentInfo>(text, (GameManager)null, (PrefabAttribute.Library)null, useProbabilities: true, useWorldConfig: true);
					array3.Shuffle(ref seed);
					list.AddRange(array3);
				}
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
				TerrainPathConnect[] componentsInChildren = prefab3.Object.GetComponentsInChildren<TerrainPathConnect>(true);
				foreach (TerrainPathConnect terrainPathConnect in componentsInChildren)
				{
					if (terrainPathConnect.Type == InfrastructureType.Road)
					{
						val += ((Component)terrainPathConnect).transform.position;
						num++;
					}
				}
				Vector3 val2 = Vector3Ex.XZ3D(val);
				Vector3 val3 = -((Vector3)(ref val2)).normalized;
				Vector3 val4 = rot90 * val3;
				if (num > 1)
				{
					val /= (float)num;
				}
				foreach (PathList road in TerrainMeta.Path.Roads)
				{
					bool flag = false;
					switch (RoadType)
					{
					case RoadMode.SideRoadOrRingRoad:
						flag = road.Hierarchy == 0 || road.Hierarchy == 1;
						break;
					case RoadMode.SideRoad:
						flag = road.Hierarchy == 1;
						break;
					case RoadMode.RingRoad:
						flag = road.Hierarchy == 0;
						break;
					case RoadMode.SideRoadOrDesireTrail:
						flag = road.Hierarchy == 1 || road.Hierarchy == 2;
						break;
					case RoadMode.DesireTrail:
						flag = road.Hierarchy == 2;
						break;
					}
					if (!flag)
					{
						continue;
					}
					PathInterpolator path = road.Path;
					float num2 = 5f;
					float num3 = 5f;
					float num4 = path.StartOffset + num3;
					float num5 = path.Length - path.EndOffset - num3;
					for (float num6 = num4; num6 <= num5; num6 += num2)
					{
						Vector3 val5 = (road.Spline ? path.GetPointCubicHermite(num6) : path.GetPoint(num6));
						Vector3 tangent = path.GetTangent(num6);
						for (int m = -1; m <= 1; m += 2)
						{
							Quaternion val6 = Quaternion.LookRotation((float)m * Vector3Ex.XZ3D(tangent));
							Vector3 val7 = val5;
							Quaternion val8 = val6;
							Vector3 localScale = prefab3.Object.transform.localScale;
							val8 *= Quaternion.LookRotation(val4);
							val7 -= val8 * val;
							SpawnInfo item = default(SpawnInfo);
							item.prefab = prefab3;
							item.position = val7;
							item.rotation = val8;
							item.scale = localScale;
							spawnInfoGroup3.candidates.Add(item);
						}
					}
				}
				spawnInfoGroup3.processed = true;
			}
			int num7 = 0;
			List<SpawnInfo> list2 = new List<SpawnInfo>();
			int num8 = 0;
			List<SpawnInfo> list3 = new List<SpawnInfo>();
			for (int n = 0; n < 8; n++)
			{
				num7 = 0;
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
					int num9 = (int)((!Object.op_Implicit((Object)(object)prefab4.Parameters)) ? PrefabPriority.Low : (prefab4.Parameters.Priority + 1));
					int num10 = 100000 * num9 * num9 * num9 * num9;
					int num11 = 0;
					int num12 = 0;
					SpawnInfo item2 = default(SpawnInfo);
					ListEx.Shuffle<SpawnInfo>(spawnInfoGroup4.candidates, ref seed);
					for (int num13 = 0; num13 < spawnInfoGroup4.candidates.Count; num13++)
					{
						SpawnInfo spawnInfo = spawnInfoGroup4.candidates[num13];
						DistanceInfo distanceInfo = GetDistanceInfo(list2, prefab4, spawnInfo.position, spawnInfo.rotation, spawnInfo.scale);
						if (distanceInfo.minDistanceSameType < (float)MinDistanceSameType || distanceInfo.minDistanceDifferentType < (float)MinDistanceDifferentType)
						{
							continue;
						}
						int num14 = num10;
						if (distanceInfo.minDistanceSameType != float.MaxValue)
						{
							if (DistanceSameType == DistanceMode.Min)
							{
								num14 -= Mathf.RoundToInt(distanceInfo.minDistanceSameType * distanceInfo.minDistanceSameType * 2f);
							}
							else if (DistanceSameType == DistanceMode.Max)
							{
								num14 += Mathf.RoundToInt(distanceInfo.minDistanceSameType * distanceInfo.minDistanceSameType * 2f);
							}
						}
						if (distanceInfo.minDistanceDifferentType != float.MaxValue)
						{
							if (DistanceDifferentType == DistanceMode.Min)
							{
								num14 -= Mathf.RoundToInt(distanceInfo.minDistanceDifferentType * distanceInfo.minDistanceDifferentType);
							}
							else if (DistanceDifferentType == DistanceMode.Max)
							{
								num14 += Mathf.RoundToInt(distanceInfo.minDistanceDifferentType * distanceInfo.minDistanceDifferentType);
							}
						}
						if (num14 <= num12 || !prefab4.ApplyTerrainAnchors(ref spawnInfo.position, spawnInfo.rotation, spawnInfo.scale, Filter) || !component2.CheckPlacement(spawnInfo.position, spawnInfo.rotation, spawnInfo.scale))
						{
							continue;
						}
						if (Object.op_Implicit((Object)(object)dungeonEntrance))
						{
							Vector3 val9 = spawnInfo.position + spawnInfo.rotation * Vector3.Scale(spawnInfo.scale, ((Component)dungeonEntrance).transform.position);
							Vector3 val10 = dungeonEntrance.SnapPosition(val9);
							ref Vector3 position = ref spawnInfo.position;
							position += val10 - val9;
							if (!dungeonEntrance.IsValidSpawnPosition(val10))
							{
								continue;
							}
						}
						if (prefab4.ApplyTerrainChecks(spawnInfo.position, spawnInfo.rotation, spawnInfo.scale, Filter) && prefab4.ApplyTerrainFilters(spawnInfo.position, spawnInfo.rotation, spawnInfo.scale) && prefab4.ApplyWaterChecks(spawnInfo.position, spawnInfo.rotation, spawnInfo.scale) && !prefab4.CheckEnvironmentVolumes(spawnInfo.position, spawnInfo.rotation, spawnInfo.scale, EnvironmentType.Underground | EnvironmentType.TrainTunnels))
						{
							num12 = num14;
							item2 = spawnInfo;
							num11++;
							if (num11 >= 8 || DistanceDifferentType == DistanceMode.Any)
							{
								break;
							}
						}
					}
					if (num12 > 0)
					{
						list2.Add(item2);
						num7 += num12;
					}
					if (TargetCount > 0 && list2.Count >= TargetCount)
					{
						break;
					}
				}
				if (num7 > num8)
				{
					num8 = num7;
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
