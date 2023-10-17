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
			Hierarchy = 2
		};
	}

	public override void Process(uint seed)
	{
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0307: Unknown result type (might be due to invalid IL or missing references)
		//IL_0318: Unknown result type (might be due to invalid IL or missing references)
		//IL_031d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0321: Unknown result type (might be due to invalid IL or missing references)
		//IL_0341: Unknown result type (might be due to invalid IL or missing references)
		//IL_0346: Unknown result type (might be due to invalid IL or missing references)
		//IL_0348: Unknown result type (might be due to invalid IL or missing references)
		//IL_034a: Unknown result type (might be due to invalid IL or missing references)
		//IL_034c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0351: Unknown result type (might be due to invalid IL or missing references)
		//IL_0383: Unknown result type (might be due to invalid IL or missing references)
		//IL_0385: Unknown result type (might be due to invalid IL or missing references)
		//IL_0389: Unknown result type (might be due to invalid IL or missing references)
		//IL_038e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0393: Unknown result type (might be due to invalid IL or missing references)
		//IL_03db: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
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
		float num5 = 16f * 16f;
		List<PathList> list = new List<PathList>();
		int[,] array = TerrainPath.CreateRailCostmap(ref seed);
		new PathFinder(array);
		array.GetLength(0);
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
						if (((Vector3)(ref val3)).sqrMagnitude < num5)
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
			int num6 = path.MinIndex + 1 + 16;
			int num7 = path.MaxIndex - 1 - 16;
			for (int k = num6; k <= num7; k++)
			{
				list2.Clear();
				list3.Clear();
				int num8 = SeedRandom.Range(ref seed, num3, num4);
				int num9 = SeedRandom.Range(ref seed, num, num2);
				int num10 = k;
				int num11 = k + num8;
				if (num11 >= num7)
				{
					continue;
				}
				Vector3 val4 = tangents[num10];
				Vector3 val5 = tangents[num11];
				if (Vector3.Angle(val4, val5) > 30f)
				{
					continue;
				}
				Vector3 val6 = tangents[num10];
				Vector3 val7 = tangents[num11];
				Vector3 val8 = Vector3.Normalize(points3[num10 + 8] - points3[num10]);
				Vector3 val9 = Vector3.Normalize(points3[num11] - points3[num11 - 8]);
				float num12 = Vector3.SignedAngle(val8, val6, Vector3.up);
				float num13 = 0f - Vector3.SignedAngle(val9, val7, Vector3.up);
				if (Mathf.Sign(num12) != Mathf.Sign(num13) || Mathf.Abs(num12) > 60f || Mathf.Abs(num13) > 60f)
				{
					continue;
				}
				float num14 = 5f;
				Quaternion val10 = ((num12 > 0f) ? rotRight : rotLeft);
				for (int l = num10 - 8; l <= num11 + 8; l++)
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
					if (l < num10 + 8)
					{
						float num15 = Mathf.InverseLerp((float)(num10 - 8), (float)num10, (float)l);
						float num16 = Mathf.SmoothStep(0f, 1f, num15) * num14;
						val11 += val13 * num16;
					}
					else if (l > num11 - 8)
					{
						float num17 = Mathf.InverseLerp((float)(num11 + 8), (float)num11, (float)l);
						float num18 = Mathf.SmoothStep(0f, 1f, num17) * num14;
						val11 += val13 * num18;
					}
					else
					{
						val11 += val13 * num14;
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
					k += num8;
				}
				k += num9;
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
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		float num = TerrainMeta.NormalizeX(worldPos.x);
		float num2 = TerrainMeta.NormalizeZ(worldPos.z);
		PathFinder.Point result = default(PathFinder.Point);
		result.x = Mathf.Clamp((int)(num * (float)res), 0, res - 1);
		result.y = Mathf.Clamp((int)(num2 * (float)res), 0, res - 1);
		return result;
	}
}
