using System;
using System.Collections.Generic;
using System.Linq;
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
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0815: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0360: Unknown result type (might be due to invalid IL or missing references)
		//IL_0365: Unknown result type (might be due to invalid IL or missing references)
		//IL_036e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0378: Unknown result type (might be due to invalid IL or missing references)
		//IL_037d: Unknown result type (might be due to invalid IL or missing references)
		//IL_037f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0390: Unknown result type (might be due to invalid IL or missing references)
		//IL_0397: Unknown result type (might be due to invalid IL or missing references)
		//IL_0399: Unknown result type (might be due to invalid IL or missing references)
		//IL_039b: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0509: Unknown result type (might be due to invalid IL or missing references)
		//IL_050e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0510: Unknown result type (might be due to invalid IL or missing references)
		//IL_0512: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_05be: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_0550: Unknown result type (might be due to invalid IL or missing references)
		//IL_0556: Unknown result type (might be due to invalid IL or missing references)
		//IL_0605: Unknown result type (might be due to invalid IL or missing references)
		//IL_061e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0623: Unknown result type (might be due to invalid IL or missing references)
		//IL_0628: Unknown result type (might be due to invalid IL or missing references)
		//IL_062d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0644: Unknown result type (might be due to invalid IL or missing references)
		//IL_064d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0652: Unknown result type (might be due to invalid IL or missing references)
		//IL_0657: Unknown result type (might be due to invalid IL or missing references)
		//IL_065c: Unknown result type (might be due to invalid IL or missing references)
		//IL_065e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0660: Unknown result type (might be due to invalid IL or missing references)
		//IL_0662: Unknown result type (might be due to invalid IL or missing references)
		//IL_066d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0672: Unknown result type (might be due to invalid IL or missing references)
		//IL_0674: Unknown result type (might be due to invalid IL or missing references)
		//IL_0679: Unknown result type (might be due to invalid IL or missing references)
		//IL_0682: Unknown result type (might be due to invalid IL or missing references)
		//IL_0684: Unknown result type (might be due to invalid IL or missing references)
		//IL_0689: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e1: Unknown result type (might be due to invalid IL or missing references)
		if (World.Networked)
		{
			TerrainMeta.Path.Rails.Clear();
			TerrainMeta.Path.Rails.AddRange(World.GetPaths("Rail"));
		}
		else
		{
			if (!World.Config.AboveGroundRails)
			{
				return;
			}
			List<PathList> list = new List<PathList>();
			int[,] array = TerrainPath.CreateRailCostmap(ref seed);
			PathFinder pathFinder = new PathFinder(array);
			int length = array.GetLength(0);
			new List<PathSegment>();
			List<PathFinder.Point> list2 = new List<PathFinder.Point>();
			List<PathFinder.Point> list3 = new List<PathFinder.Point>();
			List<PathFinder.Point> list4 = new List<PathFinder.Point>();
			List<Vector3> list5 = new List<Vector3>();
			List<Vector3> list6 = new List<Vector3>();
			List<Vector3> list7 = new List<Vector3>();
			List<Vector3> list8 = new List<Vector3>();
			foreach (PathList rail2 in TerrainMeta.Path.Rails)
			{
				for (PathFinder.Node node = rail2.ProcgenStartNode; node != null; node = node.next)
				{
					list2.Add(node.point);
				}
			}
			foreach (MonumentInfo monument in TerrainMeta.Path.Monuments)
			{
				pathFinder.PushPoint = monument.GetPathFinderPoint(length);
				pathFinder.PushRadius = (pathFinder.PushDistance = monument.GetPathFinderRadius(length));
				pathFinder.PushMultiplier = 50000;
				int num = int.MaxValue;
				TerrainPathConnect[] componentsInChildren = ((Component)monument).GetComponentsInChildren<TerrainPathConnect>(true);
				foreach (TerrainPathConnect item in componentsInChildren.OrderBy((TerrainPathConnect target) => DistanceToRail(((Component)target).transform.position)))
				{
					pathFinder.PushPointsAdditional.Clear();
					pathFinder.BlockedPointsAdditional.Clear();
					Vector3 val = ((Component)item).transform.position;
					TerrainPathConnect[] array2 = componentsInChildren;
					foreach (TerrainPathConnect terrainPathConnect in array2)
					{
						if (!((Object)(object)item == (Object)(object)terrainPathConnect))
						{
							Vector3 position = ((Component)terrainPathConnect).transform.position;
							PathFinder.Point point = PathFinder.GetPoint(((Component)terrainPathConnect).transform.position, length);
							pathFinder.PushPointsAdditional.Add(point);
							val += position;
						}
					}
					val /= (float)componentsInChildren.Length;
					Vector3 val2;
					if (componentsInChildren.Length <= 1)
					{
						val2 = ((Component)item).transform.forward;
					}
					else
					{
						Vector3 val3 = ((Component)item).transform.position - val;
						val2 = ((Vector3)(ref val3)).normalized;
					}
					Vector3 val4 = val2;
					foreach (PathList item2 in list)
					{
						pathFinder.PushPointsAdditional.Add(PathFinder.GetPoint(item2.Path.GetEndPoint(), length));
						PathFinder.Point point2 = PathFinder.GetPoint(item2.Path.GetStartPoint(), length);
						Vector3[] points = item2.Path.Points;
						for (int j = 0; j < points.Length; j++)
						{
							PathFinder.Point point3 = PathFinder.GetPoint(points[j], length);
							pathFinder.BlockedPointsAdditional.Add(point3);
							pathFinder.BlockedPointsAdditional.Add(new PathFinder.Point(point3.x, point2.y));
							pathFinder.BlockedPointsAdditional.Add(new PathFinder.Point(point2.x, point3.y));
							point2 = point3;
						}
					}
					list5.Clear();
					list6.Clear();
					list7.Clear();
					list8.Clear();
					if (item.Type != InfrastructureType.Rail)
					{
						continue;
					}
					Vector3 val5 = ((Component)item).transform.position;
					Vector3 val6 = ((Component)item).transform.forward * 7.5f;
					PathFinder.Point point4 = PathFinder.GetPoint(val5, length);
					for (int k = 0; k < 8 || (k < 16 && !pathFinder.IsWalkable(point4)); k++)
					{
						list5.Add(val5);
						val5 += val6;
						point4 = PathFinder.GetPoint(val5, length);
					}
					if (!pathFinder.IsWalkable(point4))
					{
						continue;
					}
					list3.Clear();
					list3.Add(point4);
					list4.Clear();
					list4.AddRange(list2);
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
						float num2 = TerrainMeta.DenormalizeX(normX);
						float num3 = TerrainMeta.DenormalizeZ(normZ);
						float num4 = Mathf.Max(TerrainMeta.HeightMap.GetHeight(normX, normZ), 1f);
						list6.Add(new Vector3(num2, num4, num3));
					}
					if (list6.Count == 0)
					{
						continue;
					}
					_ = list5[0];
					Vector3 val7 = list6[list6.Count - 1];
					Vector3 val8 = val4;
					PathList pathList = null;
					float num5 = float.MaxValue;
					int num6 = -1;
					foreach (PathList rail3 in TerrainMeta.Path.Rails)
					{
						Vector3[] points2 = rail3.Path.Points;
						for (int l = 0; l < points2.Length; l++)
						{
							float num7 = Vector3.Distance(val7, points2[l]);
							if (num7 < num5)
							{
								num5 = num7;
								pathList = rail3;
								num6 = l;
							}
						}
					}
					Vector3[] points3 = pathList.Path.Points;
					Vector3 val9 = pathList.Path.Tangents[num6];
					int num8 = ((Vector3.Angle(val9, val8) < Vector3.Angle(-val9, val8)) ? 1 : (-1));
					if (num != int.MaxValue)
					{
						GenericsUtil.Swap<int>(ref num, ref num8);
						num8 = -num8;
					}
					else
					{
						num = num8;
					}
					Vector3 val10 = Vector3.Normalize(list6[list6.Count - 1] - list6[Mathf.Max(0, list6.Count - 1 - 16)]);
					Vector3 val11 = Vector3.Normalize(points3[(num6 + num8 * 8 * 2 + points3.Length) % points3.Length] - points3[num6]);
					float num9 = 0f - Vector3.SignedAngle(val11, val10, Vector3.up);
					Vector3 val12 = rot90 * val11;
					if (num9 < 0f)
					{
						val12 = -val12;
					}
					for (int m = 0; m < 8; m++)
					{
						float num10 = Mathf.InverseLerp(7f, 0f, (float)m);
						float num11 = Mathf.SmoothStep(0f, 2f, num10) * 4f;
						list7.Add(points3[(num6 + num8 * m + points3.Length) % points3.Length] + val12 * num11);
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
					float num13 = Mathf.InverseLerp(0f, 8f, (float)i);
					float num14 = Mathf.InverseLerp((float)rail.Path.DefaultMaxIndex, (float)(rail.Path.DefaultMaxIndex - 8), (float)i);
					return Mathf.SmoothStep(0f, 1f, Mathf.Min(num13, num14));
				};
				rail.Path.Smoothen(32, new Vector3(1f, 0f, 1f), filter);
				rail.Path.Smoothen(64, new Vector3(0f, 1f, 0f), filter);
				rail.Path.Resample(7.5f);
				rail.Path.RecalculateTangents();
				rail.AdjustPlacementMap(20f);
			}
			TerrainMeta.Path.Rails.AddRange(list);
		}
		static float DistanceToRail(Vector3 vec)
		{
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			float num12 = float.MaxValue;
			foreach (PathList rail4 in TerrainMeta.Path.Rails)
			{
				Vector3[] points4 = rail4.Path.Points;
				foreach (Vector3 val13 in points4)
				{
					num12 = Mathf.Min(num12, Vector3Ex.Magnitude2D(vec - val13));
				}
			}
			return num12;
		}
	}
}
