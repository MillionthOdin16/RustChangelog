using System;
using System.Linq;
using UnityEngine;

public class GenerateRailTerrain : ProceduralComponent
{
	public const int SmoothenLoops = 8;

	public const int SmoothenIterations = 8;

	public const int SmoothenY = 64;

	public const int SmoothenXZ = 32;

	public const int TransitionSteps = 8;

	public override void Process(uint seed)
	{
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		TerrainHeightMap heightMap = TerrainMeta.HeightMap;
		Func<int, float> func = (int i) => Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(0f, 8f, (float)i));
		for (int j = 0; j < 8; j++)
		{
			foreach (PathList item in TerrainMeta.Path.Rails.AsEnumerable().Reverse())
			{
				PathInterpolator path = item.Path;
				Vector3[] points = path.Points;
				for (int k = 0; k < points.Length; k++)
				{
					Vector3 val = points[k];
					float num = (item.Start ? func(k) : 1f);
					val.y = Mathf.SmoothStep(val.y, heightMap.GetHeight(val), num);
					points[k] = val;
				}
				path.Smoothen(8, Vector3.up, item.Start ? func : null);
				path.RecalculateTangents();
				heightMap.Push();
				float intensity = 1f;
				float fade = Mathf.InverseLerp(8f, 0f, (float)j);
				item.AdjustTerrainHeight(intensity, fade);
				heightMap.Pop();
			}
		}
		foreach (PathList rail in TerrainMeta.Path.Rails)
		{
			PathInterpolator path2 = rail.Path;
			Vector3[] points2 = path2.Points;
			for (int l = 0; l < points2.Length; l++)
			{
				Vector3 val2 = points2[l];
				float num2 = (rail.Start ? func(l) : 1f);
				val2.y = Mathf.SmoothStep(val2.y, heightMap.GetHeight(val2), num2);
				points2[l] = val2;
			}
			path2.RecalculateTangents();
		}
	}
}
