using System.Collections.Generic;
using Facepunch;
using UnityEngine;
using UnityEngine.Rendering;

public class FoliageGridMeshData
{
	public struct FoliageVertex
	{
		public Vector3 position;

		public Vector3 normal;

		public Vector4 tangent;

		public Color32 color;

		public Vector2 uv;

		public Vector4 uv2;

		public static readonly VertexAttributeDescriptor[] VertexLayout = (VertexAttributeDescriptor[])(object)new VertexAttributeDescriptor[6]
		{
			new VertexAttributeDescriptor((VertexAttribute)0, (VertexAttributeFormat)0, 3, 0),
			new VertexAttributeDescriptor((VertexAttribute)1, (VertexAttributeFormat)0, 3, 0),
			new VertexAttributeDescriptor((VertexAttribute)2, (VertexAttributeFormat)0, 4, 0),
			new VertexAttributeDescriptor((VertexAttribute)3, (VertexAttributeFormat)2, 4, 0),
			new VertexAttributeDescriptor((VertexAttribute)4, (VertexAttributeFormat)0, 2, 0),
			new VertexAttributeDescriptor((VertexAttribute)6, (VertexAttributeFormat)0, 4, 0)
		};
	}

	public List<FoliageVertex> vertices;

	public List<int> triangles;

	public Bounds bounds;

	public void Alloc()
	{
		if (triangles == null)
		{
			triangles = Pool.GetList<int>();
		}
		if (vertices == null)
		{
			vertices = Pool.GetList<FoliageVertex>();
		}
	}

	public void Free()
	{
		if (triangles != null)
		{
			Pool.FreeList<int>(ref triangles);
		}
		if (vertices != null)
		{
			Pool.FreeList<FoliageVertex>(ref vertices);
		}
	}

	public void Clear()
	{
		triangles?.Clear();
		vertices?.Clear();
	}

	public void Combine(MeshGroup meshGroup)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		if (meshGroup.data.Count == 0)
		{
			return;
		}
		bounds = new Bounds(meshGroup.data[0].position, Vector3.one);
		Vector3 val3 = default(Vector3);
		for (int i = 0; i < meshGroup.data.Count; i++)
		{
			MeshInstance meshInstance = meshGroup.data[i];
			Matrix4x4 val = Matrix4x4.TRS(meshInstance.position, meshInstance.rotation, meshInstance.scale);
			int count = vertices.Count;
			for (int j = 0; j < meshInstance.data.triangles.Length; j++)
			{
				triangles.Add(count + meshInstance.data.triangles[j]);
			}
			for (int k = 0; k < meshInstance.data.vertices.Length; k++)
			{
				Vector4 val2 = meshInstance.data.tangents[k];
				((Vector3)(ref val3))._002Ector(val2.x, val2.y, val2.z);
				Vector3 val4 = ((Matrix4x4)(ref val)).MultiplyVector(val3);
				FoliageVertex item = default(FoliageVertex);
				item.position = ((Matrix4x4)(ref val)).MultiplyPoint3x4(meshInstance.data.vertices[k]);
				item.normal = ((Matrix4x4)(ref val)).MultiplyVector(meshInstance.data.normals[k]);
				item.uv = meshInstance.data.uv[k];
				item.uv2 = Vector4.op_Implicit(meshInstance.position);
				item.tangent = new Vector4(val4.x, val4.y, val4.z, val2.w);
				if (meshInstance.data.colors32.Length != 0)
				{
					item.color = meshInstance.data.colors32[k];
				}
				vertices.Add(item);
			}
			((Bounds)(ref bounds)).Encapsulate(meshInstance.position);
		}
		ref Bounds reference = ref bounds;
		((Bounds)(ref reference)).size = ((Bounds)(ref reference)).size + Vector3.one * 2f;
	}

	public void Apply(Mesh mesh)
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		mesh.SetVertexBufferParams(vertices.Count, FoliageVertex.VertexLayout);
		mesh.SetVertexBufferData<FoliageVertex>(vertices, 0, 0, vertices.Count, 0, (MeshUpdateFlags)9);
		mesh.SetIndices(triangles, (MeshTopology)0, 0, false, 0);
		mesh.bounds = bounds;
	}
}
