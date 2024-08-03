using System;
using System.Collections.Generic;
using UnityEngine;

public class GenerateRailBranching : ProceduralComponent
{
	public const float Width = 4f;

	public const float InnerPadding = 1f;

	public const float OuterPadding = 1f;

	public const float InnerFade = 1f;

	public const float OuterFade = 32f;

	public const float RandomScale = 1f;

	public const float MeshOffset = 0f;

	public const float TerrainOffset = -0.125f;

	private static Quaternion rot90 = Quaternion.Euler(0f, 90f, 0f);

	private const int MaxDepth = 250000;

	private PathList CreateSegment(int number, Vector3[] points)
	{
		PathList pathList = new PathList("Rail " + number, points);
		pathList.Spline = true;
		pathList.Width = 4f;
		pathList.InnerPadding = 1f;
		pathList.OuterPadding = 1f;
		pathList.InnerFade = 1f;
		pathList.OuterFade = 32f;
		pathList.RandomScale = 1f;
		pathList.MeshOffset = 0f;
		pathList.TerrainOffset = -0.125f;
		pathList.Topology = 524288;
		pathList.Splat = 128;
		pathList.Hierarchy = 1;
		return pathList;
	}

	public override void Process(uint seed)
	{
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0858: Unknown result type (might be due to invalid IL or missing references)
		//IL_0882: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_047a: Unknown result type (might be due to invalid IL or missing references)
		//IL_04be: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0500: Unknown result type (might be due to invalid IL or missing references)
		//IL_0509: Unknown result type (might be due to invalid IL or missing references)
		//IL_050e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0513: Unknown result type (might be due to invalid IL or missing references)
		//IL_0518: Unknown result type (might be due to invalid IL or missing references)
		//IL_051e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0534: Unknown result type (might be due to invalid IL or missing references)
		//IL_0539: Unknown result type (might be due to invalid IL or missing references)
		//IL_053e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0543: Unknown result type (might be due to invalid IL or missing references)
		//IL_0545: Unknown result type (might be due to invalid IL or missing references)
		//IL_0547: Unknown result type (might be due to invalid IL or missing references)
		//IL_0549: Unknown result type (might be due to invalid IL or missing references)
		//IL_0555: Unknown result type (might be due to invalid IL or missing references)
		//IL_0557: Unknown result type (might be due to invalid IL or missing references)
		//IL_0559: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0600: Unknown result type (might be due to invalid IL or missing references)
		//IL_065a: Unknown result type (might be due to invalid IL or missing references)
		//IL_065f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0663: Unknown result type (might be due to invalid IL or missing references)
		//IL_0668: Unknown result type (might be due to invalid IL or missing references)
		//IL_0683: Unknown result type (might be due to invalid IL or missing references)
		//IL_0688: Unknown result type (might be due to invalid IL or missing references)
		//IL_068c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0691: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d3: Unknown result type (might be due to invalid IL or missing references)
		if (World.Networked)
		{
			TerrainMeta.Path.Rails.Clear();
			TerrainMeta.Path.Rails.AddRange(World.GetPaths("Rail"));
			return;
		}
		int num = Mathf.RoundToInt(40f);
		int num2 = Mathf.RoundToInt(53.333332f);
		int num3 = Mathf.RoundToInt(40f);
		int num4 = Mathf.RoundToInt(120f);
		float num5 = 120f;
		float num6 = num5 * num5;
		List<PathList> list = new List<PathList>();
		int[,] array = TerrainPath.CreateRailCostmap(ref seed);
		PathFinder pathFinder = new PathFinder(array);
		int length = array.GetLength(0);
		List<Vector3> list2 = new List<Vector3>();
		List<Vector3> list3 = new List<Vector3>();
		List<Vector3> list4 = new List<Vector3>();
		HashSet<Vector3> hashSet = new HashSet<Vector3>();
		foreach (PathList rail2 in TerrainMeta.Path.Rails)
		{
			foreach (PathList rail3 in TerrainMeta.Path.Rails)
			{
				if (rail2 == rail3)
				{
					continue;
				}
				Vector3[] points = rail2.Path.Points;
				foreach (Vector3 val in points)
				{
					Vector3[] points2 = rail3.Path.Points;
					foreach (Vector3 val2 in points2)
					{
						Vector3 val3 = val - val2;
						if (((Vector3)(ref val3)).sqrMagnitude < num6)
						{
							hashSet.Add(val);
							break;
						}
					}
				}
			}
		}
		foreach (PathList rail4 in TerrainMeta.Path.Rails)
		{
			PathInterpolator path = rail4.Path;
			Vector3[] points3 = path.Points;
			Vector3[] tangents = path.Tangents;
			int num7 = path.MinIndex + 1 + 8;
			int num8 = path.MaxIndex - 1 - 8;
			for (int l = num7; l <= num8; l++)
			{
				list2.Clear();
				list3.Clear();
				list4.Clear();
				int num9 = SeedRandom.Range(ref seed, num3, num4);
				int num10 = SeedRandom.Range(ref seed, num, num2);
				int num11 = l;
				int num12 = l + num9;
				if (num12 >= num8)
				{
					continue;
				}
				Vector3 val4 = tangents[num11];
				Vector3 val5 = tangents[num12];
				if (Vector3.Angle(val4, val5) > 30f)
				{
					continue;
				}
				Vector3 val6 = points3[num11];
				Vector3 val7 = points3[num12];
				if (hashSet.Contains(val6) || hashSet.Contains(val7))
				{
					continue;
				}
				PathFinder.Point pathFinderPoint = GetPathFinderPoint(val6, length);
				PathFinder.Point pathFinderPoint2 = GetPathFinderPoint(val7, length);
				l += num10;
				PathFinder.Node node = pathFinder.FindPath(pathFinderPoint, pathFinderPoint2, 250000);
				if (node == null)
				{
					continue;
				}
				PathFinder.Node node2 = null;
				PathFinder.Node node3 = null;
				PathFinder.Node node4 = node;
				while (node4 != null && node4.next != null)
				{
					if (node4 == node.next)
					{
						node2 = node4;
					}
					if (node4.next.next == null)
					{
						node3 = node4;
					}
					node4 = node4.next;
				}
				if (node2 == null || node3 == null)
				{
					continue;
				}
				node = node2;
				node3.next = null;
				for (int m = 0; m < 8; m++)
				{
					int num13 = num11 + (m + 1 - 8);
					int num14 = num12 + m;
					list2.Add(points3[num13]);
					list3.Add(points3[num14]);
				}
				list4.AddRange(list2);
				for (PathFinder.Node node5 = node2; node5 != null; node5 = node5.next)
				{
					float normX = ((float)node5.point.x + 0.5f) / (float)length;
					float normZ = ((float)node5.point.y + 0.5f) / (float)length;
					float num15 = TerrainMeta.DenormalizeX(normX);
					float num16 = TerrainMeta.DenormalizeZ(normZ);
					float num17 = Mathf.Max(TerrainMeta.HeightMap.GetHeight(normX, normZ), 1f);
					list4.Add(new Vector3(num15, num17, num16));
				}
				list4.AddRange(list3);
				int num18 = 8;
				int num19 = list4.Count - 1 - 8;
				Vector3 val8 = Vector3.Normalize(list4[num18 + 16] - list4[num18]);
				Vector3 val9 = Vector3.Normalize(list4[num19] - list4[num19 - 16]);
				Vector3 val10 = Vector3.Normalize(points3[num11 + 16] - points3[num11]);
				Vector3 val11 = Vector3.Normalize(points3[num12] - points3[(num12 - 16 + points3.Length) % points3.Length]);
				float num20 = Vector3.SignedAngle(val10, val8, Vector3.up);
				float num21 = 0f - Vector3.SignedAngle(val11, val9, Vector3.up);
				if (Mathf.Sign(num20) != Mathf.Sign(num21) || Mathf.Abs(num20) > 60f || Mathf.Abs(num21) > 60f)
				{
					continue;
				}
				Vector3 val12 = rot90 * val10;
				Vector3 val13 = rot90 * val11;
				if (num20 < 0f)
				{
					val12 = -val12;
				}
				if (num21 < 0f)
				{
					val13 = -val13;
				}
				for (int n = 0; n < 16; n++)
				{
					int num22 = n;
					int num23 = list4.Count - n - 1;
					float num24 = Mathf.InverseLerp(0f, 8f, (float)n);
					float num25 = Mathf.SmoothStep(0f, 2f, num24) * 4f;
					List<Vector3> list5 = list4;
					int index = num22;
					list5[index] += val12 * num25;
					list5 = list4;
					index = num23;
					list5[index] += val13 * num25;
				}
				bool flag = false;
				bool flag2 = false;
				foreach (Vector3 item in list4)
				{
					bool blocked = TerrainMeta.PlacementMap.GetBlocked(item);
					if (!flag2 && !flag && !blocked)
					{
						flag = true;
					}
					if (flag && !flag2 && blocked)
					{
						flag2 = true;
					}
					if (flag && flag2 && !blocked)
					{
						list4.Clear();
						break;
					}
				}
				if (list4.Count != 0)
				{
					if (list4.Count >= 2)
					{
						int number = TerrainMeta.Path.Rails.Count + list.Count;
						PathList pathList = CreateSegment(number, list4.ToArray());
						pathList.Start = false;
						pathList.End = false;
						pathList.ProcgenStartNode = node2;
						pathList.ProcgenEndNode = node3;
						list.Add(pathList);
					}
					l += num9;
				}
			}
		}
		foreach (PathList rail in list)
		{
			Func<int, float> filter = delegate(int i)
			{
				float num26 = Mathf.InverseLerp(0f, 8f, (float)i);
				float num27 = Mathf.InverseLerp((float)rail.Path.DefaultMaxIndex, (float)(rail.Path.DefaultMaxIndex - 8), (float)i);
				return Mathf.SmoothStep(0f, 1f, Mathf.Min(num26, num27));
			};
			rail.Path.Smoothen(32, new Vector3(1f, 0f, 1f), filter);
			rail.Path.Smoothen(64, new Vector3(0f, 1f, 0f), filter);
			rail.Path.Resample(7.5f);
			rail.Path.RecalculateTangents();
			rail.AdjustPlacementMap(20f);
		}
		TerrainMeta.Path.Rails.AddRange(list);
	}

	public PathFinder.Point GetPathFinderPoint(Vector3 worldPos, int res)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		float num = TerrainMeta.NormalizeX(worldPos.x);
		float num2 = TerrainMeta.NormalizeZ(worldPos.z);
		PathFinder.Point result = default(PathFinder.Point);
		result.x = Mathf.Clamp((int)(num * (float)res), 0, res - 1);
		result.y = Mathf.Clamp((int)(num2 * (float)res), 0, res - 1);
		return result;
	}
}
