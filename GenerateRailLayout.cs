using System;
using System.Collections.Generic;
using UnityEngine;

public class GenerateRailLayout : ProceduralComponent
{
	private class PathNode
	{
		public MonumentInfo monument;

		public TerrainPathConnect target;

		public PathFinder.Node node;
	}

	private class PathSegment
	{
		public PathFinder.Node start;

		public PathFinder.Node end;

		public TerrainPathConnect origin;

		public TerrainPathConnect target;
	}

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
		return new PathList("Rail " + number, points)
		{
			Spline = true,
			Width = 4f,
			InnerPadding = 1f,
			OuterPadding = 1f,
			InnerFade = 1f,
			OuterFade = 32f,
			RandomScale = 1f,
			MeshOffset = 0f,
			TerrainOffset = -0.125f,
			Topology = 524288,
			Splat = 128,
			Hierarchy = 1
		};
	}

	public override void Process(uint seed)
	{
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_063d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0665: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0337: Unknown result type (might be due to invalid IL or missing references)
		//IL_035d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0362: Unknown result type (might be due to invalid IL or missing references)
		//IL_036f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0374: Unknown result type (might be due to invalid IL or missing references)
		//IL_0376: Unknown result type (might be due to invalid IL or missing references)
		//IL_0378: Unknown result type (might be due to invalid IL or missing references)
		//IL_037a: Unknown result type (might be due to invalid IL or missing references)
		//IL_037f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0383: Unknown result type (might be due to invalid IL or missing references)
		//IL_0388: Unknown result type (might be due to invalid IL or missing references)
		//IL_042d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0432: Unknown result type (might be due to invalid IL or missing references)
		//IL_0434: Unknown result type (might be due to invalid IL or missing references)
		//IL_0436: Unknown result type (might be due to invalid IL or missing references)
		//IL_043d: Unknown result type (might be due to invalid IL or missing references)
		//IL_043f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0444: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_045e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0477: Unknown result type (might be due to invalid IL or missing references)
		//IL_047c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0481: Unknown result type (might be due to invalid IL or missing references)
		//IL_0486: Unknown result type (might be due to invalid IL or missing references)
		//IL_049d: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04db: Unknown result type (might be due to invalid IL or missing references)
		//IL_04dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_052c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0531: Unknown result type (might be due to invalid IL or missing references)
		//IL_0535: Unknown result type (might be due to invalid IL or missing references)
		//IL_053a: Unknown result type (might be due to invalid IL or missing references)
		if (World.Networked)
		{
			TerrainMeta.Path.Rails.Clear();
			TerrainMeta.Path.Rails.AddRange(World.GetPaths("Rail"));
			return;
		}
		List<PathList> list = new List<PathList>();
		int[,] array = TerrainPath.CreateRailCostmap(ref seed);
		PathFinder pathFinder = new PathFinder(array);
		int length = array.GetLength(0);
		new List<PathSegment>();
		List<PathFinder.Node> list2 = new List<PathFinder.Node>();
		List<PathFinder.Point> list3 = new List<PathFinder.Point>();
		List<PathFinder.Point> list4 = new List<PathFinder.Point>();
		List<Vector3> list5 = new List<Vector3>();
		List<Vector3> list6 = new List<Vector3>();
		List<Vector3> list7 = new List<Vector3>();
		List<Vector3> list8 = new List<Vector3>();
		foreach (PathList rail2 in TerrainMeta.Path.Rails)
		{
			if (rail2.ProcgenStartNode != null && rail2.ProcgenEndNode != null)
			{
				for (PathFinder.Node node = rail2.ProcgenStartNode; node != null; node = node.next)
				{
					list2.Add(node);
				}
			}
		}
		foreach (MonumentInfo monument in TerrainMeta.Path.Monuments)
		{
			pathFinder.PushPoint = monument.GetPathFinderPoint(length);
			pathFinder.PushRadius = monument.GetPathFinderRadius(length);
			pathFinder.PushDistance = 60;
			pathFinder.PushMultiplier = 1;
			TerrainPathConnect[] componentsInChildren = ((Component)monument).GetComponentsInChildren<TerrainPathConnect>(true);
			foreach (TerrainPathConnect terrainPathConnect in componentsInChildren)
			{
				list5.Clear();
				list6.Clear();
				list7.Clear();
				list8.Clear();
				if (terrainPathConnect.Type != InfrastructureType.Rail)
				{
					continue;
				}
				Vector3 val = ((Component)terrainPathConnect).transform.position;
				Vector3 val2 = ((Component)terrainPathConnect).transform.forward * 7.5f;
				PathFinder.Point pathFinderPoint = terrainPathConnect.GetPathFinderPoint(length, val);
				for (int k = 0; k < 8 || !pathFinder.IsWalkable(pathFinderPoint); k++)
				{
					list5.Add(val);
					val += val2;
					pathFinderPoint = terrainPathConnect.GetPathFinderPoint(length, val);
				}
				list3.Clear();
				list3.Add(pathFinderPoint);
				list4.Clear();
				foreach (PathFinder.Node item in list2)
				{
					if (!(pathFinder.Distance(item.point, pathFinder.PushPoint) < (float)(pathFinder.PushRadius + pathFinder.PushDistance / 2)))
					{
						list4.Add(item.point);
					}
				}
				PathFinder.Node node2 = pathFinder.FindPathDirected(list3, list4, 250000);
				if (node2 == null)
				{
					continue;
				}
				PathFinder.Node node3 = null;
				PathFinder.Node node4 = null;
				PathFinder.Node node5 = node2;
				while (node5 != null && node5.next != null)
				{
					if (node5 == node2.next)
					{
						node3 = node5;
					}
					if (node5.next.next == null)
					{
						node4 = node5;
					}
					node5 = node5.next;
				}
				if (node3 == null || node4 == null)
				{
					continue;
				}
				node2 = node3;
				node4.next = null;
				for (PathFinder.Node node6 = node3; node6 != null; node6 = node6.next)
				{
					float normX = ((float)node6.point.x + 0.5f) / (float)length;
					float normZ = ((float)node6.point.y + 0.5f) / (float)length;
					float num = TerrainMeta.DenormalizeX(normX);
					float num2 = TerrainMeta.DenormalizeZ(normZ);
					float num3 = Mathf.Max(TerrainMeta.HeightMap.GetHeight(normX, normZ), 1f);
					list6.Add(new Vector3(num, num3, num2));
				}
				if (list6.Count == 0)
				{
					continue;
				}
				Vector3 val3 = list5[0];
				Vector3 val4 = list6[list6.Count - 1];
				Vector3 val5 = val4 - val3;
				Vector3 normalized = ((Vector3)(ref val5)).normalized;
				PathList pathList = null;
				float num4 = float.MaxValue;
				int num5 = -1;
				foreach (PathList rail3 in TerrainMeta.Path.Rails)
				{
					Vector3[] points = rail3.Path.Points;
					for (int l = 0; l < points.Length; l++)
					{
						float num6 = Vector3.Distance(val4, points[l]);
						if (num6 < num4)
						{
							num4 = num6;
							pathList = rail3;
							num5 = l;
						}
					}
				}
				Vector3[] points2 = pathList.Path.Points;
				Vector3 val6 = pathList.Path.Tangents[num5];
				int num7 = ((Vector3.Angle(val6, normalized) < Vector3.Angle(-val6, normalized)) ? 1 : (-1));
				Vector3 val7 = Vector3.Normalize(list6[list6.Count - 1] - list6[Mathf.Max(0, list6.Count - 1 - 16)]);
				Vector3 val8 = Vector3.Normalize(points2[(num5 + num7 * 8 * 2 + points2.Length) % points2.Length] - points2[num5]);
				float num8 = 0f - Vector3.SignedAngle(val8, val7, Vector3.up);
				Vector3 val9 = rot90 * val8;
				if (num8 < 0f)
				{
					val9 = -val9;
				}
				for (int m = 0; m < 8; m++)
				{
					float num9 = Mathf.InverseLerp(7f, 0f, (float)m);
					float num10 = Mathf.SmoothStep(0f, 2f, num9) * 4f;
					list7.Add(points2[(num5 + num7 * m + points2.Length) % points2.Length] + val9 * num10);
				}
				list8.AddRange(list5);
				list8.AddRange(list6);
				list8.AddRange(list7);
				if (list8.Count >= 2)
				{
					int number = TerrainMeta.Path.Rails.Count + list.Count;
					PathList pathList2 = CreateSegment(number, list8.ToArray());
					pathList2.Start = true;
					pathList2.End = false;
					pathList2.ProcgenStartNode = node3;
					pathList2.ProcgenEndNode = node4;
					list.Add(pathList2);
				}
			}
		}
		foreach (PathList rail in list)
		{
			Func<int, float> filter = delegate(int i)
			{
				float num11 = Mathf.InverseLerp(0f, 8f, (float)i);
				float num12 = Mathf.InverseLerp((float)rail.Path.DefaultMaxIndex, (float)(rail.Path.DefaultMaxIndex - 8), (float)i);
				return Mathf.SmoothStep(0f, 1f, Mathf.Min(num11, num12));
			};
			rail.Path.Smoothen(32, new Vector3(1f, 0f, 1f), filter);
			rail.Path.Smoothen(64, new Vector3(0f, 1f, 0f), filter);
			rail.Path.Resample(7.5f);
			rail.Path.RecalculateTangents();
			rail.AdjustPlacementMap(20f);
		}
		TerrainMeta.Path.Rails.AddRange(list);
	}
}
