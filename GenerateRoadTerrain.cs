using System.Linq;
using UnityEngine;

public class GenerateRoadTerrain : ProceduralComponent
{
	public const int SmoothenLoops = 2;

	public const int SmoothenIterations = 8;

	public const int SmoothenY = 16;

	public const int SmoothenXZ = 4;

	public override void Process(uint seed)
	{
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		TerrainHeightMap heightMap = TerrainMeta.HeightMap;
		TerrainTopologyMap topologymap = TerrainMeta.TopologyMap;
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
				path.Smoothen(8, Vector3.up, delegate(int i)
				{
					//IL_0018: Unknown result type (might be due to invalid IL or missing references)
					int topology = topologymap.GetTopology(path.Points[i]);
					if (((uint)topology & 0x80000u) != 0)
					{
						return 0f;
					}
					return (((uint)topology & 0x100000u) != 0) ? 0.5f : 1f;
				});
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
