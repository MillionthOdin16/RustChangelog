using UnityEngine;
using UnityEngine.Rendering;

namespace Instancing;

public class InstancedMeshRenderer
{
	public int RendererId { get; }

	public int DrawCallIndex { get; }

	public int DrawCallCount { get; }

	public string MeshName => ((Object)Mesh).name;

	public Mesh Mesh { get; }

	public Material[] Materials { get; private set; }

	public Material[] MultidrawMaterials { get; private set; }

	public ShadowCastingMode CastShadows { get; }

	public bool RecieveShadows { get; }

	public LightProbeUsage LightProbes { get; }

	public int Verticies { get; }

	public int Triangles { get; }

	public int VertexStart { get; private set; }

	public int IndexStart { get; private set; }

	public int LodLevel { get; }

	public int TotalLodLevels { get; }

	public bool IsLastLod { get; }

	public InstancedMeshCategory MeshCategory { get; }

	public MultidrawMeshInfo[] MultidrawSubmeshes { get; }

	public bool HasShadow => (int)CastShadows > 0;

	public bool HasMesh => (int)CastShadows != 3;

	public Vector3[] BoundsPoints { get; }

	public InstancedMeshRenderer(int rendererIndex, int drawCallIndex, MeshRenderKey key, Material[] multidrawMaterials, int lodLevel, int lodLevels, InstancedMeshCategory meshCategory, GeometryBuffers buffers)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		RendererId = rendererIndex;
		DrawCallIndex = drawCallIndex;
		Mesh = key.Mesh;
		Materials = key.Materials;
		MultidrawMaterials = multidrawMaterials;
		CastShadows = key.CastShadows;
		RecieveShadows = key.RecieveShadows;
		LightProbes = key.LightProbeUsages;
		Verticies = Mesh.vertexCount;
		LodLevel = lodLevel;
		TotalLodLevels = lodLevels;
		IsLastLod = lodLevel == lodLevels - 1;
		MeshCategory = meshCategory;
		DrawCallCount = Mathf.Min(Mesh.subMeshCount, Materials.Length);
		if (Materials.Length > Mesh.subMeshCount)
		{
			string name = ((Object)Mesh).name;
			Debug.LogError((object)("More submesh than material for mesh " + name));
		}
		if (Mesh.subMeshCount > Materials.Length)
		{
			string name2 = ((Object)Mesh).name;
			Debug.LogWarning((object)("More materials than submesh for mesh " + name2));
		}
		for (int i = 0; i < Mesh.subMeshCount; i++)
		{
			Triangles += (int)Mesh.GetIndexCount(i) / 3;
		}
		Bounds bounds = Mesh.bounds;
		BoundsPoints = (Vector3[])(object)new Vector3[8]
		{
			((Bounds)(ref bounds)).min,
			((Bounds)(ref bounds)).max,
			new Vector3(((Bounds)(ref bounds)).max.x, ((Bounds)(ref bounds)).min.y, ((Bounds)(ref bounds)).min.z),
			new Vector3(((Bounds)(ref bounds)).min.x, ((Bounds)(ref bounds)).max.y, ((Bounds)(ref bounds)).min.z),
			new Vector3(((Bounds)(ref bounds)).min.x, ((Bounds)(ref bounds)).min.y, ((Bounds)(ref bounds)).max.z),
			new Vector3(((Bounds)(ref bounds)).max.x, ((Bounds)(ref bounds)).max.y, ((Bounds)(ref bounds)).min.z),
			new Vector3(((Bounds)(ref bounds)).min.x, ((Bounds)(ref bounds)).max.y, ((Bounds)(ref bounds)).max.z),
			new Vector3(((Bounds)(ref bounds)).max.x, ((Bounds)(ref bounds)).min.y, ((Bounds)(ref bounds)).max.z)
		};
		MultidrawSubmeshes = buffers.CopyMesh(Mesh);
	}

	public void SetMaterials(Material[] materials)
	{
		Materials = materials;
	}

	public void SetPlaceholderMaterials(Material[] materials)
	{
		Materials = materials;
	}

	public int GetDrawCallIndex(int submesh)
	{
		return DrawCallIndex + submesh;
	}

	public int GetIndirectArgIndex(int submesh)
	{
		return GetDrawCallIndex(submesh) * 5;
	}

	public int GetIndirectArgByteIndex(int submesh)
	{
		return GetIndirectArgIndex(submesh) * 4;
	}
}
