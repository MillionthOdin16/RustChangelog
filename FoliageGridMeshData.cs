using System.Collections.Generic;
using Facepunch;
using UnityEngine;
using UnityEngine.Profiling;
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
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		if (meshGroup.data.Count == 0)
		{
			return;
		}
		bounds = new Bounds(meshGroup.data[0].position, Vector3.zero);
		Profiler.BeginSample("FoliageGridBatch.Combine");
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
			((Bounds)(ref bounds)).Encapsulate(new Bounds(meshInstance.position + ((Bounds)(ref meshInstance.data.bounds)).center, ((Bounds)(ref meshInstance.data.bounds)).size));
		}
		Profiler.EndSample();
		ref Bounds reference = ref bounds;
		((Bounds)(ref reference)).size = ((Bounds)(ref reference)).size + Vector3.one;
	}

	public void Apply(Mesh mesh)
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		Profiler.BeginSample("FoliageGridBatch.Apply");
		mesh.SetVertexBufferParams(vertices.Count, FoliageVertex.VertexLayout);
		mesh.SetVertexBufferData<FoliageVertex>(vertices, 0, 0, vertices.Count, 0, (MeshUpdateFlags)9);
		mesh.SetIndices(triangles, (MeshTopology)0, 0, false, 0);
		mesh.bounds = bounds;
		Profiler.EndSample();
	}
}
