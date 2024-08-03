using System.Linq;
using UnityEngine;

public class GenerateRoadTerrain : ProceduralComponent
{
	public const int SmoothenLoops = 2;

	public const int SmoothenIterations = 8;

	public const int SmoothenY = 16;

	public const int SmoothenXZ = 4;

	private float SmoothenFilter(Vector3[] points, int index)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		int topology = TerrainMeta.TopologyMap.GetTopology(points[index]);
		if (((uint)topology & 0x80400u) != 0)
		{
			return 0f;
		}
		if (((uint)topology & 0x100000u) != 0)
		{
			return 0.5f;
		}
		return 1f;
	}

	public override void Process(uint seed)
	{
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		TerrainHeightMap heightMap = TerrainMeta.HeightMap;
		for (int j = 0; j < 2; j++)
		{
			foreach (PathList item in TerrainMeta.Path.Roads.AsEnumerable().Reverse())
			{
				PathInterpolator path = item.Path;
				Vector3[] points = path.Points;
				for (int k = 0; k < points.Length; k++)
				{
					Vector3 val = points[k];
					val.y = heightMap.GetHeight(val);
					points[k] = val;
				}
				path.Smoothen(8, Vector3.up, (int i) => SmoothenFilter(points, i));
				path.RecalculateTangents();
				heightMap.Push();
				float intensity = 1f;
				float fade = Mathf.InverseLerp(2f, 0f, (float)j);
				item.AdjustTerrainHeight(intensity, fade);
				heightMap.Pop();
			}
			foreach (PathList item2 in TerrainMeta.Path.Rails.AsEnumerable().Reverse())
			{
				heightMap.Push();
				float intensity2 = 1f;
				float num = Mathf.InverseLerp(2f, 0f, (float)j);
				item2.AdjustTerrainHeight(intensity2, num / 4f);
				heightMap.Pop();
			}
		}
		foreach (PathList road in TerrainMeta.Path.Roads)
		{
			PathInterpolator path2 = road.Path;
			Vector3[] points2 = path2.Points;
			for (int l = 0; l < points2.Length; l++)
			{
				Vector3 val2 = points2[l];
				val2.y = heightMap.GetHeight(val2);
				points2[l] = val2;
			}
			path2.RecalculateTangents();
		}
	}
}
