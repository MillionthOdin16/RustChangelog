using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace Instancing;

public struct MeshRenderKey : IEquatable<MeshRenderKey>
{
	public Mesh Mesh;

	public Material[] Materials;

	public ShadowCastingMode CastShadows;

	public bool RecieveShadows;

	public LightProbeUsage LightProbeUsages;

	public MeshRenderKey(Mesh mesh, Material[] materials, ShadowCastingMode castShadows, bool recieveShadows, LightProbeUsage lightProbes)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		Mesh = mesh;
		Materials = materials;
		CastShadows = castShadows;
		RecieveShadows = recieveShadows;
		LightProbeUsages = lightProbes;
	}

	public bool Equals(MeshRenderKey other)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)Mesh != (Object)(object)other.Mesh || CastShadows != other.CastShadows || RecieveShadows != other.RecieveShadows || LightProbeUsages != other.LightProbeUsages)
		{
			return false;
		}
		if (Materials == null || other.Materials == null)
		{
			return Materials == other.Materials;
		}
		for (int i = 0; i < Materials.Length; i++)
		{
			if ((Object)(object)Materials[i] != (Object)(object)other.Materials[i])
			{
				return false;
			}
		}
		return true;
	}

	public override bool Equals(object obj)
	{
		if (obj is MeshRenderKey other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return (17 * 31 + ((object)Mesh)?.GetHashCode()).GetValueOrDefault();
	}
}
