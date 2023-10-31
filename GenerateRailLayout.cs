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
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0400: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0411: Unknown result type (might be due to invalid IL or missing references)
		//IL_0418: Unknown result type (might be due to invalid IL or missing references)
		//IL_041a: Unknown result type (might be due to invalid IL or missing references)
		//IL_041c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0421: Unknown result type (might be due to invalid IL or missing references)
		//IL_0423: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_075c: Unknown result type (might be due to invalid IL or missing references)
		//IL_076a: Unknown result type (might be due to invalid IL or missing references)
		//IL_076f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0774: Unknown result type (might be due to invalid IL or missing references)
		//IL_0779: Unknown result type (might be due to invalid IL or missing references)
		//IL_077b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0780: Unknown result type (might be due to invalid IL or missing references)
		//IL_0782: Unknown result type (might be due to invalid IL or missing references)
		//IL_0787: Unknown result type (might be due to invalid IL or missing references)
		//IL_0709: Unknown result type (might be due to invalid IL or missing references)
		//IL_070e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0710: Unknown result type (might be due to invalid IL or missing references)
		//IL_0712: Unknown result type (might be due to invalid IL or missing references)
		//IL_0719: Unknown result type (might be due to invalid IL or missing references)
		//IL_071b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0720: Unknown result type (might be due to invalid IL or missing references)
		//IL_063d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0643: Unknown result type (might be due to invalid IL or missing references)
		//IL_0798: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_07bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_07da: Unknown result type (might be due to invalid IL or missing references)
		//IL_07df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0824: Unknown result type (might be due to invalid IL or missing references)
		//IL_0829: Unknown result type (might be due to invalid IL or missing references)
		//IL_082d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0832: Unknown result type (might be due to invalid IL or missing references)
		//IL_08eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0914: Unknown result type (might be due to invalid IL or missing references)
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
			PathFinder pathFinder2 = new PathFinder(array);
			int length = array.GetLength(0);
			new List<PathSegment>();
			List<PathFinder.Point> list2 = new List<PathFinder.Point>();
			List<PathFinder.Point> list3 = new List<PathFinder.Point>();
			List<PathFinder.Point> list4 = new List<PathFinder.Point>();
			List<Vector3> list5 = new List<Vector3>();
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
				TerrainPathConnect[] array2 = (from target in ((Component)monument).GetComponentsInChildren<TerrainPathConnect>(true)
					where target.Type == InfrastructureType.Rail
					orderby DistanceToRail(((Component)target).transform.position)
					select target).ToArray();
				TerrainPathConnect[] array3 = array2;
				foreach (TerrainPathConnect terrainPathConnect in array3)
				{
					pathFinder.PushPointsAdditional.Clear();
					pathFinder.BlockedPointsAdditional.Clear();
					Vector3 val = ((Component)terrainPathConnect).transform.position;
					TerrainPathConnect[] array4 = array2;
					foreach (TerrainPathConnect terrainPathConnect2 in array4)
					{
						if (!((Object)(object)terrainPathConnect == (Object)(object)terrainPathConnect2))
						{
							Vector3 position = ((Component)terrainPathConnect2).transform.position;
							PathFinder.Point point = PathFinder.GetPoint(((Component)terrainPathConnect2).transform.position, length);
							pathFinder.PushPointsAdditional.Add(point);
							val += position;
						}
					}
					val /= (float)array2.Length;
					Vector3 val2;
					if (array2.Length <= 1)
					{
						val2 = ((Component)terrainPathConnect).transform.forward;
					}
					else
					{
						Vector3 val3 = ((Component)terrainPathConnect).transform.position - val;
						val2 = ((Vector3)(ref val3)).normalized;
					}
					Vector3 val4 = val2;
					foreach (PathList item in list)
					{
						pathFinder.PushPointsAdditional.Add(PathFinder.GetPoint(item.Path.GetEndPoint(), length));
						PathFinder.Point point2 = PathFinder.GetPoint(item.Path.GetStartPoint(), length);
						Vector3[] points = item.Path.Points;
						for (int k = 0; k < points.Length; k++)
						{
							PathFinder.Point point3 = PathFinder.GetPoint(points[k], length);
							pathFinder.BlockedPointsAdditional.Add(point3);
							pathFinder.BlockedPointsAdditional.Add(new PathFinder.Point(point3.x, point2.y));
							pathFinder.BlockedPointsAdditional.Add(new PathFinder.Point(point2.x, point3.y));
							point2 = point3;
						}
						if (item.ProcgenStartNode != null)
						{
							PathFinder.Point point4 = item.ProcgenStartNode.point;
							for (PathFinder.Node node2 = item.ProcgenStartNode; node2 != null; node2 = node2.next)
							{
								PathFinder.Point point5 = node2.point;
								pathFinder.BlockedPointsAdditional.Add(point5);
								pathFinder.BlockedPointsAdditional.Add(new PathFinder.Point(point5.x, point4.y));
								pathFinder.BlockedPointsAdditional.Add(new PathFinder.Point(point4.x, point5.y));
								point4 = point5;
							}
						}
					}
					list5.Clear();
					Vector3 val5 = ((Component)terrainPathConnect).transform.position;
					Vector3 val6 = ((Component)terrainPathConnect).transform.forward * 7.5f;
					PathFinder.Point point6 = PathFinder.GetPoint(val5, length);
					for (int l = 0; (l < 8 && pathFinder.Heuristic(point6, list2) > 1) || (l < 16 && !pathFinder.IsWalkable(point6)); l++)
					{
						list5.Add(val5);
						val5 += val6;
						point6 = PathFinder.GetPoint(val5, length);
					}
					if (!pathFinder.IsWalkable(point6))
					{
						continue;
					}
					list3.Clear();
					list3.Add(point6);
					list4.Clear();
					list4.AddRange(list2);
					PathFinder.Node node3 = pathFinder.FindPathDirected(list3, list4, 250000);
					bool flag = false;
					if (node3 == null && list.Count > 0 && num != int.MaxValue)
					{
						PathList pathList = list[list.Count - 1];
						list4.Clear();
						for (int m = 0; m < pathList.Path.Points.Length; m++)
						{
							list4.Add(PathFinder.GetPoint(pathList.Path.Points[m], length));
						}
						node3 = pathFinder2.FindPathDirected(list3, list4, 250000);
						flag = true;
					}
					if (node3 == null)
					{
						continue;
					}
					PathFinder.Node node4 = null;
					PathFinder.Node node5 = null;
					PathFinder.Node node6 = node3;
					while (node6 != null && node6.next != null)
					{
						if (node6 == node3.next)
						{
							node4 = node6;
						}
						if (node6.next.next == null)
						{
							node5 = node6;
							node5.next = null;
						}
						node6 = node6.next;
					}
					for (PathFinder.Node node7 = node4; node7 != null; node7 = node7.next)
					{
						float normX = ((float)node7.point.x + 0.5f) / (float)length;
						float normZ = ((float)node7.point.y + 0.5f) / (float)length;
						float num2 = TerrainMeta.DenormalizeX(normX);
						float num3 = TerrainMeta.DenormalizeZ(normZ);
						float num4 = Mathf.Max(TerrainMeta.HeightMap.GetHeight(normX, normZ), 1f);
						list5.Add(new Vector3(num2, num4, num3));
					}
					Vector3 val7 = list5[list5.Count - 1];
					Vector3 val8 = val4;
					PathList pathList2 = null;
					float num5 = float.MaxValue;
					int num6 = -1;
					if (!flag)
					{
						foreach (PathList rail3 in TerrainMeta.Path.Rails)
						{
							Vector3[] points2 = rail3.Path.Points;
							for (int n = 0; n < points2.Length; n++)
							{
								float num7 = Vector3.Distance(val7, points2[n]);
								if (num7 < num5)
								{
									num5 = num7;
									pathList2 = rail3;
									num6 = n;
								}
							}
						}
					}
					else
					{
						foreach (PathList item2 in list)
						{
							Vector3[] points3 = item2.Path.Points;
							for (int num8 = 0; num8 < points3.Length; num8++)
							{
								float num9 = Vector3.Distance(val7, points3[num8]);
								if (num9 < num5)
								{
									num5 = num9;
									pathList2 = item2;
									num6 = num8;
								}
							}
						}
					}
					int num10 = 1;
					if (!flag)
					{
						Vector3 tangentByIndex = pathList2.Path.GetTangentByIndex(num6);
						num10 = ((Vector3.Angle(tangentByIndex, val8) < Vector3.Angle(-tangentByIndex, val8)) ? 1 : (-1));
						if (num != int.MaxValue)
						{
							GenericsUtil.Swap<int>(ref num, ref num10);
							num10 = -num10;
						}
						else
						{
							num = num10;
						}
					}
					Vector3 val9 = Vector3.Normalize(pathList2.Path.GetPointByIndex(num6 + num10 * 8 * 2) - pathList2.Path.GetPointByIndex(num6));
					Vector3 val10 = rot90 * val9;
					if (!flag)
					{
						Vector3 val11 = Vector3.Normalize(list5[list5.Count - 1] - list5[Mathf.Max(0, list5.Count - 1 - 16)]);
						if (0f - Vector3.SignedAngle(val9, val11, Vector3.up) < 0f)
						{
							val10 = -val10;
						}
					}
					for (int num11 = 0; num11 < 8; num11++)
					{
						float num12 = Mathf.InverseLerp(7f, 0f, (float)num11);
						float num13 = Mathf.SmoothStep(0f, 2f, num12) * 4f;
						list5.Add(pathList2.Path.GetPointByIndex(num6 + num10 * num11) + val10 * num13);
					}
					if (list5.Count >= 2)
					{
						int number = TerrainMeta.Path.Rails.Count + list.Count;
						PathList rail = CreateSegment(number, list5.ToArray());
						rail.Start = true;
						rail.End = false;
						rail.ProcgenStartNode = node4;
						rail.ProcgenEndNode = node5;
						Func<int, float> filter = delegate(int i)
						{
							float num16 = Mathf.InverseLerp(0f, 8f, (float)i);
							float num17 = Mathf.InverseLerp((float)rail.Path.DefaultMaxIndex, (float)(rail.Path.DefaultMaxIndex - 8), (float)i);
							return Mathf.SmoothStep(0f, 1f, Mathf.Min(num16, num17));
						};
						rail.Path.Smoothen(32, new Vector3(1f, 0f, 1f), filter);
						rail.Path.Smoothen(64, new Vector3(0f, 1f, 0f), filter);
						rail.Path.Resample(7.5f);
						rail.Path.RecalculateTangents();
						list.Add(rail);
					}
				}
			}
			foreach (PathList item3 in list)
			{
				item3.AdjustPlacementMap(20f);
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
			float num14 = float.MaxValue;
			foreach (PathList rail4 in TerrainMeta.Path.Rails)
			{
				Vector3[] points4 = rail4.Path.Points;
				foreach (Vector3 val12 in points4)
				{
					num14 = Mathf.Min(num14, Vector3Ex.Magnitude2D(vec - val12));
				}
			}
			return num14;
		}
	}
}
