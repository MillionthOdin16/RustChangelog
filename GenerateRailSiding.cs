using System.Collections.Generic;
using UnityEngine;

public class GenerateRailSiding : ProceduralComponent
{
	public const float Width = 4f;

	public const float InnerPadding = 1f;

	public const float OuterPadding = 1f;

	public const float InnerFade = 1f;

	public const float OuterFade = 32f;

	public const float RandomScale = 1f;

	public const float MeshOffset = 0f;

	public const float TerrainOffset = -0.125f;

	private static Quaternion rotRight = Quaternion.Euler(0f, 90f, 0f);

	private static Quaternion rotLeft = Quaternion.Euler(0f, -90f, 0f);

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
		pathList.Hierarchy = 2;
		return pathList;
	}

	public override void Process(uint seed)
	{
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0304: Unknown result type (might be due to invalid IL or missing references)
		//IL_0306: Unknown result type (might be due to invalid IL or missing references)
		//IL_0308: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_037c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0375: Unknown result type (might be due to invalid IL or missing references)
		//IL_0381: Unknown result type (might be due to invalid IL or missing references)
		//IL_0393: Unknown result type (might be due to invalid IL or missing references)
		//IL_0398: Unknown result type (might be due to invalid IL or missing references)
		//IL_039c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_040c: Unknown result type (might be due to invalid IL or missing references)
		//IL_040e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0412: Unknown result type (might be due to invalid IL or missing references)
		//IL_0417: Unknown result type (might be due to invalid IL or missing references)
		//IL_041c: Unknown result type (might be due to invalid IL or missing references)
		//IL_046e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0470: Unknown result type (might be due to invalid IL or missing references)
		//IL_0474: Unknown result type (might be due to invalid IL or missing references)
		//IL_0479: Unknown result type (might be due to invalid IL or missing references)
		//IL_047e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0458: Unknown result type (might be due to invalid IL or missing references)
		//IL_045a: Unknown result type (might be due to invalid IL or missing references)
		//IL_045e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0463: Unknown result type (might be due to invalid IL or missing references)
		//IL_0468: Unknown result type (might be due to invalid IL or missing references)
		//IL_0483: Unknown result type (might be due to invalid IL or missing references)
		//IL_048d: Unknown result type (might be due to invalid IL or missing references)
		if (World.Networked)
		{
			TerrainMeta.Path.Rails.Clear();
			TerrainMeta.Path.Rails.AddRange(World.GetPaths("Rail"));
			return;
		}
		int num = Mathf.RoundToInt(40f);
		int num2 = Mathf.RoundToInt(53.333332f);
		int num3 = Mathf.RoundToInt(13.333333f);
		int num4 = Mathf.RoundToInt(20f);
		float num5 = 16f;
		float num6 = num5 * num5;
		List<PathList> list = new List<PathList>();
		int[,] array = TerrainPath.CreateRailCostmap(ref seed);
		PathFinder pathFinder = new PathFinder(array);
		int length = array.GetLength(0);
		List<Vector3> list2 = new List<Vector3>();
		List<Vector3> list3 = new List<Vector3>();
		HashSet<Vector3> hashSet = new HashSet<Vector3>();
		foreach (PathList rail in TerrainMeta.Path.Rails)
		{
			foreach (PathList rail2 in TerrainMeta.Path.Rails)
			{
				if (rail == rail2)
				{
					continue;
				}
				Vector3[] points = rail.Path.Points;
				foreach (Vector3 val in points)
				{
					Vector3[] points2 = rail2.Path.Points;
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
		foreach (PathList rail3 in TerrainMeta.Path.Rails)
		{
			PathInterpolator path = rail3.Path;
			Vector3[] points3 = path.Points;
			Vector3[] tangents = path.Tangents;
			int num7 = path.MinIndex + 1 + 16;
			int num8 = path.MaxIndex - 1 - 16;
			for (int k = num7; k <= num8; k++)
			{
				list2.Clear();
				list3.Clear();
				int num9 = SeedRandom.Range(ref seed, num3, num4);
				int num10 = SeedRandom.Range(ref seed, num, num2);
				int num11 = k;
				int num12 = k + num9;
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
				Vector3 val6 = tangents[num11];
				Vector3 val7 = tangents[num12];
				Vector3 val8 = Vector3.Normalize(points3[num11 + 8] - points3[num11]);
				Vector3 val9 = Vector3.Normalize(points3[num12] - points3[num12 - 8]);
				float num13 = Vector3.SignedAngle(val8, val6, Vector3.up);
				float num14 = 0f - Vector3.SignedAngle(val9, val7, Vector3.up);
				if (Mathf.Sign(num13) != Mathf.Sign(num14) || Mathf.Abs(num13) > 60f || Mathf.Abs(num14) > 60f)
				{
					continue;
				}
				float num15 = 5f;
				Quaternion val10 = ((num13 > 0f) ? rotRight : rotLeft);
				for (int l = num11 - 8; l <= num12 + 8; l++)
				{
					Vector3 val11 = points3[l];
					if (hashSet.Contains(val11))
					{
						list2.Clear();
						list3.Clear();
						break;
					}
					Vector3 val12 = tangents[l];
					Vector3 val13 = val10 * val12;
					if (l < num11 + 8)
					{
						float num16 = Mathf.InverseLerp((float)(num11 - 8), (float)num11, (float)l);
						float num17 = Mathf.SmoothStep(0f, 1f, num16) * num15;
						val11 += val13 * num17;
					}
					else if (l > num12 - 8)
					{
						float num18 = Mathf.InverseLerp((float)(num12 + 8), (float)num12, (float)l);
						float num19 = Mathf.SmoothStep(0f, 1f, num18) * num15;
						val11 += val13 * num19;
					}
					else
					{
						val11 += val13 * num15;
					}
					list2.Add(val11);
					list3.Add(val12);
				}
				if (list2.Count >= 2)
				{
					int number = TerrainMeta.Path.Rails.Count + list.Count;
					PathList pathList = CreateSegment(number, list2.ToArray());
					pathList.Start = false;
					pathList.End = false;
					list.Add(pathList);
					k += num9;
				}
				k += num10;
			}
		}
		foreach (PathList item in list)
		{
			item.Path.Resample(7.5f);
			item.Path.RecalculateTangents();
			item.AdjustPlacementMap(20f);
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
