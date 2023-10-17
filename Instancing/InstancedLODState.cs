using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace Instancing;

[Serializable]
public class InstancedLODState
{
	public Mesh Mesh;

	public Material[] Materials;

	public Matrix4x4 LocalToWorld;

	public ShadowCastingMode CastShadows;

	public bool RecieveShadows;

	public LightProbeUsage LightProbes;

	public int LodLevel;

	public int TotalLodLevels;

	public InstancedMeshCategory MeshCategory;

	public float MinimumDistance;

	public float MaximumDistance;

	public InstancedLODState(Matrix4x4 localToWorld, MeshRenderer renderer, float minDistance, float maxDistance, int lodLevel, int lodLevels, InstancedMeshCategory category)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		MeshCull component = ((Component)renderer).GetComponent<MeshCull>();
		MeshFilter component2 = ((Component)renderer).GetComponent<MeshFilter>();
		Mesh = component2.sharedMesh;
		Materials = ((Renderer)renderer).sharedMaterials;
		LocalToWorld = localToWorld;
		CastShadows = ((Renderer)renderer).shadowCastingMode;
		RecieveShadows = ((Renderer)renderer).receiveShadows;
		LightProbes = ((Renderer)renderer).lightProbeUsage;
		MinimumDistance = minDistance;
		MaximumDistance = component?.Distance ?? maxDistance;
		MeshCategory = category;
		LodLevel = lodLevel;
		TotalLodLevels = lodLevels;
	}
}
