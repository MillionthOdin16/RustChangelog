using UnityEngine;
using UnityEngine.Rendering;

public class RendererInfo : ComponentInfo<Renderer>
{
	public ShadowCastingMode shadows;

	public Material material;

	public Mesh mesh;

	public MeshFilter meshFilter;

	public override void Reset()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		component.shadowCastingMode = shadows;
		if (Object.op_Implicit((Object)(object)material))
		{
			component.sharedMaterial = material;
		}
		Renderer obj = component;
		SkinnedMeshRenderer val;
		if ((val = (SkinnedMeshRenderer)(object)((obj is SkinnedMeshRenderer) ? obj : null)) != null)
		{
			val.sharedMesh = mesh;
			return;
		}
		Renderer obj2 = component;
		MeshRenderer val2;
		if ((val2 = (MeshRenderer)(object)((obj2 is MeshRenderer) ? obj2 : null)) != null)
		{
			meshFilter.sharedMesh = mesh;
		}
	}

	public override void Setup()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		shadows = component.shadowCastingMode;
		material = component.sharedMaterial;
		Renderer obj = component;
		SkinnedMeshRenderer val;
		if ((val = (SkinnedMeshRenderer)(object)((obj is SkinnedMeshRenderer) ? obj : null)) != null)
		{
			mesh = val.sharedMesh;
			return;
		}
		Renderer obj2 = component;
		MeshRenderer val2;
		if ((val2 = (MeshRenderer)(object)((obj2 is MeshRenderer) ? obj2 : null)) != null)
		{
			meshFilter = ((Component)this).GetComponent<MeshFilter>();
			mesh = meshFilter.sharedMesh;
		}
	}
}
