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

	private float SmoothenFilterStart(int index)
	{
		return Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(0f, 8f, (float)index));
	}

	public override void Process(uint seed)
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		TerrainHeightMap heightMap = TerrainMeta.HeightMap;
		for (int i = 0; i < 8; i++)
		{
			foreach (PathList item in TerrainMeta.Path.Rails.AsEnumerable().Reverse())
			{
				PathInterpolator path = item.Path;
				Vector3[] points = path.Points;
				for (int j = 0; j < points.Length; j++)
				{
					Vector3 val = points[j];
					float height = heightMap.GetHeight(val);
					if (item.Start)
					{
						val.y = Mathf.SmoothStep(val.y, height, SmoothenFilterStart(j));
					}
					else
					{
						val.y = height;
					}
					points[j] = val;
				}
				path.Smoothen(8, Vector3.up, item.Start ? new Func<int, float>(SmoothenFilterStart) : null);
				path.RecalculateTangents();
				heightMap.Push();
				float intensity = 1f;
				float fade = Mathf.InverseLerp(8f, 0f, (float)i);
				item.AdjustTerrainHeight(intensity, fade);
				heightMap.Pop();
			}
		}
		foreach (PathList rail in TerrainMeta.Path.Rails)
		{
			PathInterpolator path2 = rail.Path;
			Vector3[] points2 = path2.Points;
			for (int k = 0; k < points2.Length; k++)
			{
				Vector3 val2 = points2[k];
				float height2 = heightMap.GetHeight(val2);
				if (rail.Start)
				{
					val2.y = Mathf.SmoothStep(val2.y, height2, SmoothenFilterStart(k));
				}
				else
				{
					val2.y = height2;
				}
				points2[k] = val2;
			}
			path2.RecalculateTangents();
		}
	}
}
