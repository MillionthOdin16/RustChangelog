using ConVar;
using UnityEngine;
using UnityEngine.Rendering;

public class MeshDataBatch : MeshBatch
{
	private Vector3 position;

	private Mesh meshBatch;

	private MeshFilter meshFilter;

	private MeshRenderer meshRenderer;

	private MeshData meshData;

	private MeshGroup meshGroup;

	public override int VertexCapacity => Batching.renderer_capacity;

	public override int VertexCutoff => Batching.renderer_vertices;

	protected void Awake()
	{
		meshFilter = ((Component)this).GetComponent<MeshFilter>();
		meshRenderer = ((Component)this).GetComponent<MeshRenderer>();
		meshData = new MeshData();
		meshGroup = new MeshGroup();
	}

	public void Setup(Vector3 position, Material material, ShadowCastingMode shadows, int layer)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Invalid comparison between Unknown and I4
		Vector3 val2 = (((Component)this).transform.position = position);
		this.position = val2;
		((Component)this).gameObject.layer = layer;
		((Renderer)meshRenderer).sharedMaterial = material;
		((Renderer)meshRenderer).shadowCastingMode = shadows;
		if ((int)shadows == 3)
		{
			((Renderer)meshRenderer).receiveShadows = false;
			((Renderer)meshRenderer).motionVectors = false;
			((Renderer)meshRenderer).lightProbeUsage = (LightProbeUsage)0;
			((Renderer)meshRenderer).reflectionProbeUsage = (ReflectionProbeUsage)0;
		}
		else
		{
			((Renderer)meshRenderer).receiveShadows = true;
			((Renderer)meshRenderer).motionVectors = true;
			((Renderer)meshRenderer).lightProbeUsage = (LightProbeUsage)1;
			((Renderer)meshRenderer).reflectionProbeUsage = (ReflectionProbeUsage)1;
		}
	}

	public void Add(MeshInstance instance)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		ref Vector3 reference = ref instance.position;
		reference -= position;
		meshGroup.data.Add(instance);
		AddVertices(instance.mesh.vertexCount);
	}

	protected override void AllocMemory()
	{
		meshGroup.Alloc();
		meshData.Alloc();
	}

	protected override void FreeMemory()
	{
		meshGroup.Free();
		meshData.Free();
	}

	protected override void RefreshMesh()
	{
		meshData.Clear();
		meshData.Combine(meshGroup);
	}

	protected override void ApplyMesh()
	{
		if (!Object.op_Implicit((Object)(object)meshBatch))
		{
			meshBatch = AssetPool.Get<Mesh>();
		}
		meshData.Apply(meshBatch);
		meshBatch.UploadMeshData(false);
	}

	protected override void ToggleMesh(bool state)
	{
		if (state)
		{
			if (Object.op_Implicit((Object)(object)meshFilter))
			{
				meshFilter.sharedMesh = meshBatch;
			}
			if (Object.op_Implicit((Object)(object)meshRenderer))
			{
				((Renderer)meshRenderer).enabled = true;
			}
		}
		else
		{
			if (Object.op_Implicit((Object)(object)meshFilter))
			{
				meshFilter.sharedMesh = null;
			}
			if (Object.op_Implicit((Object)(object)meshRenderer))
			{
				((Renderer)meshRenderer).enabled = false;
			}
		}
	}

	protected override void OnPooled()
	{
		if (Object.op_Implicit((Object)(object)meshFilter))
		{
			meshFilter.sharedMesh = null;
		}
		if (Object.op_Implicit((Object)(object)meshBatch))
		{
			AssetPool.Free(ref meshBatch);
		}
		meshData.Free();
		meshGroup.Free();
	}
}
