using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;
using UnityEngine.AI;

public class AsyncTerrainNavMeshBake : CustomYieldInstruction
{
	private List<int> indices;

	private List<Vector3> vertices;

	private List<Vector3> normals;

	private List<int> triangles;

	private Vector3 pivot;

	private int width;

	private int height;

	private bool normal;

	private bool alpha;

	private Action worker;

	public override bool keepWaiting => worker != null;

	public bool isDone => worker == null;

	public Mesh mesh
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Expected O, but got Unknown
			Mesh val = new Mesh();
			if (vertices != null)
			{
				val.SetVertices(vertices);
				Pool.FreeList<Vector3>(ref vertices);
			}
			if (normals != null)
			{
				val.SetNormals(normals);
				Pool.FreeList<Vector3>(ref normals);
			}
			if (triangles != null)
			{
				val.SetTriangles(triangles, 0);
				Pool.FreeList<int>(ref triangles);
			}
			if (indices != null)
			{
				Pool.FreeList<int>(ref indices);
			}
			return val;
		}
	}

	public NavMeshBuildSource CreateNavMeshBuildSource()
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		NavMeshBuildSource result = default(NavMeshBuildSource);
		((NavMeshBuildSource)(ref result)).transform = Matrix4x4.TRS(pivot, Quaternion.identity, Vector3.one);
		((NavMeshBuildSource)(ref result)).shape = (NavMeshBuildSourceShape)0;
		((NavMeshBuildSource)(ref result)).sourceObject = (Object)(object)mesh;
		return result;
	}

	public NavMeshBuildSource CreateNavMeshBuildSource(int area)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		NavMeshBuildSource result = CreateNavMeshBuildSource();
		((NavMeshBuildSource)(ref result)).area = area;
		return result;
	}

	public AsyncTerrainNavMeshBake(Vector3 pivot, int width, int height, bool normal, bool alpha)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		this.pivot = pivot;
		this.width = width;
		this.height = height;
		this.normal = normal;
		this.alpha = alpha;
		indices = Pool.GetList<int>();
		vertices = Pool.GetList<Vector3>();
		normals = (normal ? Pool.GetList<Vector3>() : null);
		triangles = Pool.GetList<int>();
		Invoke();
	}

	private void DoWork()
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = default(Vector3);
		((Vector3)(ref val))._002Ector((float)(width / 2), 0f, (float)(height / 2));
		Vector3 val2 = default(Vector3);
		((Vector3)(ref val2))._002Ector(pivot.x - val.x, 0f, pivot.z - val.z);
		TerrainHeightMap heightMap = TerrainMeta.HeightMap;
		TerrainAlphaMap alphaMap = TerrainMeta.AlphaMap;
		int i = 0;
		for (int j = 0; j <= height; j++)
		{
			for (int k = 0; k <= width; k++, i++)
			{
				Vector3 worldPos = new Vector3((float)k, 0f, (float)j) + val2;
				Vector3 item = new Vector3((float)k, 0f, (float)j) - val;
				float num = heightMap.GetHeight(worldPos);
				if (num < -1f)
				{
					indices.Add(-1);
					continue;
				}
				if (alpha)
				{
					float num2 = alphaMap.GetAlpha(worldPos);
					if (num2 < 0.1f)
					{
						indices.Add(-1);
						continue;
					}
				}
				if (normal)
				{
					Vector3 item2 = heightMap.GetNormal(worldPos);
					normals.Add(item2);
				}
				worldPos.y = (item.y = num - pivot.y);
				indices.Add(vertices.Count);
				vertices.Add(item);
			}
		}
		int num3 = 0;
		int num4 = 0;
		while (num4 < height)
		{
			int num5 = 0;
			while (num5 < width)
			{
				int num6 = indices[num3];
				int num7 = indices[num3 + width + 1];
				int num8 = indices[num3 + 1];
				int num9 = indices[num3 + 1];
				int num10 = indices[num3 + width + 1];
				int num11 = indices[num3 + width + 2];
				if (num6 != -1 && num7 != -1 && num8 != -1)
				{
					triangles.Add(num6);
					triangles.Add(num7);
					triangles.Add(num8);
				}
				if (num9 != -1 && num10 != -1 && num11 != -1)
				{
					triangles.Add(num9);
					triangles.Add(num10);
					triangles.Add(num11);
				}
				num5++;
				num3++;
			}
			num4++;
			num3++;
		}
	}

	private void Invoke()
	{
		worker = DoWork;
		worker.BeginInvoke(Callback, null);
	}

	private void Callback(IAsyncResult result)
	{
		worker.EndInvoke(result);
		worker = null;
	}
}
