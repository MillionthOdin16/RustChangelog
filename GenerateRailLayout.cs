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
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0400: Unknown result type (might be due to invalid IL or missing references)
		//IL_0402: Unknown result type (might be due to invalid IL or missing references)
		//IL_0404: Unknown result type (might be due to invalid IL or missing references)
		//IL_0409: Unknown result type (might be due to invalid IL or missing references)
		//IL_040b: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0689: Unknown result type (might be due to invalid IL or missing references)
		//IL_068f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0737: Unknown result type (might be due to invalid IL or missing references)
		//IL_0745: Unknown result type (might be due to invalid IL or missing references)
		//IL_074a: Unknown result type (might be due to invalid IL or missing references)
		//IL_074f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0754: Unknown result type (might be due to invalid IL or missing references)
		//IL_0756: Unknown result type (might be due to invalid IL or missing references)
		//IL_075b: Unknown result type (might be due to invalid IL or missing references)
		//IL_075d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0762: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_06eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0618: Unknown result type (might be due to invalid IL or missing references)
		//IL_061e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0773: Unknown result type (might be due to invalid IL or missing references)
		//IL_078c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0791: Unknown result type (might be due to invalid IL or missing references)
		//IL_0796: Unknown result type (might be due to invalid IL or missing references)
		//IL_079b: Unknown result type (might be due to invalid IL or missing references)
		//IL_079d: Unknown result type (might be due to invalid IL or missing references)
		//IL_079f: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0804: Unknown result type (might be due to invalid IL or missing references)
		//IL_0808: Unknown result type (might be due to invalid IL or missing references)
		//IL_080d: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ef: Unknown result type (might be due to invalid IL or missing references)
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
						if (item2.ProcgenStartNode != null)
						{
							PathFinder.Point point4 = item2.ProcgenStartNode.point;
							for (PathFinder.Node node2 = item2.ProcgenStartNode; node2 != null; node2 = node2.next)
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
					if (item.Type != InfrastructureType.Rail)
					{
						continue;
					}
					Vector3 val5 = ((Component)item).transform.position;
					Vector3 val6 = ((Component)item).transform.forward * 7.5f;
					PathFinder.Point point6 = PathFinder.GetPoint(val5, length);
					for (int k = 0; k < 8 || (k < 16 && !pathFinder.IsWalkable(point6)); k++)
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
						for (int l = 0; l < pathList.Path.Points.Length; l++)
						{
							list4.Add(PathFinder.GetPoint(pathList.Path.Points[l], length));
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
							for (int m = 0; m < points2.Length; m++)
							{
								float num7 = Vector3.Distance(val7, points2[m]);
								if (num7 < num5)
								{
									num5 = num7;
									pathList2 = rail3;
									num6 = m;
								}
							}
						}
					}
					else
					{
						foreach (PathList item3 in list)
						{
							Vector3[] points3 = item3.Path.Points;
							for (int n = 0; n < points3.Length; n++)
							{
								float num8 = Vector3.Distance(val7, points3[n]);
								if (num8 < num5)
								{
									num5 = num8;
									pathList2 = item3;
									num6 = n;
								}
							}
						}
					}
					int num9 = 1;
					if (!flag)
					{
						Vector3 tangentByIndex = pathList2.Path.GetTangentByIndex(num6);
						num9 = ((Vector3.Angle(tangentByIndex, val8) < Vector3.Angle(-tangentByIndex, val8)) ? 1 : (-1));
						if (num != int.MaxValue)
						{
							GenericsUtil.Swap<int>(ref num, ref num9);
							num9 = -num9;
						}
						else
						{
							num = num9;
						}
					}
					Vector3 val9 = Vector3.Normalize(pathList2.Path.GetPointByIndex(num6 + num9 * 8 * 2) - pathList2.Path.GetPointByIndex(num6));
					Vector3 val10 = rot90 * val9;
					if (!flag)
					{
						Vector3 val11 = Vector3.Normalize(list5[list5.Count - 1] - list5[Mathf.Max(0, list5.Count - 1 - 16)]);
						if (0f - Vector3.SignedAngle(val9, val11, Vector3.up) < 0f)
						{
							val10 = -val10;
						}
					}
					for (int num10 = 0; num10 < 8; num10++)
					{
						float num11 = Mathf.InverseLerp(7f, 0f, (float)num10);
						float num12 = Mathf.SmoothStep(0f, 2f, num11) * 4f;
						list5.Add(pathList2.Path.GetPointByIndex(num6 + num9 * num10) + val10 * num12);
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
							float num15 = Mathf.InverseLerp(0f, 8f, (float)i);
							float num16 = Mathf.InverseLerp((float)rail.Path.DefaultMaxIndex, (float)(rail.Path.DefaultMaxIndex - 8), (float)i);
							return Mathf.SmoothStep(0f, 1f, Mathf.Min(num15, num16));
						};
						rail.Path.Smoothen(32, new Vector3(1f, 0f, 1f), filter);
						rail.Path.Smoothen(64, new Vector3(0f, 1f, 0f), filter);
						rail.Path.Resample(7.5f);
						rail.Path.RecalculateTangents();
						list.Add(rail);
					}
				}
			}
			foreach (PathList item4 in list)
			{
				item4.AdjustPlacementMap(20f);
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
			float num13 = float.MaxValue;
			foreach (PathList rail4 in TerrainMeta.Path.Rails)
			{
				Vector3[] points4 = rail4.Path.Points;
				foreach (Vector3 val12 in points4)
				{
					num13 = Mathf.Min(num13, Vector3Ex.Magnitude2D(vec - val12));
				}
			}
			return num13;
		}
	}
}
