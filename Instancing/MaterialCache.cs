using System.Collections.Generic;
using UnityEngine;

namespace Instancing;

public class MaterialCache
{
	public Dictionary<Material, Material> modifiedMaterials = new Dictionary<Material, Material>();

	public Material EnableProceduralInstancing(Material material)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Expected O, but got Unknown
		if (modifiedMaterials.TryGetValue(material, out var value))
		{
			return value;
		}
		value = new Material(material);
		value.enableInstancing = true;
		value.EnableKeyword("RUST_PROCEDURAL_INSTANCING");
		modifiedMaterials.Add(material, value);
		RequestTextureStreaming(material);
		return value;
	}

	private void RequestTextureStreaming(Material material)
	{
		RequestMipmap(material, "_MainTex");
		RequestMipmap(material, "_MetallicGlossMap");
		RequestMipmap(material, "_SpecGlossMap");
		RequestMipmap(material, "_BumpMap");
		RequestMipmap(material, "_OcclusionMap");
		RequestMipmap(material, "_EmissionMap");
		RequestMipmap(material, "_TransmissionMap");
		RequestMipmap(material, "_SubsurfaceMaskMap");
		RequestMipmap(material, "_TransmissionMaskMap");
		RequestMipmap(material, "_DetailMask");
		RequestMipmap(material, "_DetailOcclusionMap");
		RequestMipmap(material, "_BiomeLayer_TintMask");
		RequestMipmap(material, "_WetnessLayer_Mask");
		RequestMipmap(material, "_DetailAlbedoMap");
		RequestMipmap(material, "_DetailMetallicGlossMap");
		RequestMipmap(material, "_DetailNormalMap");
		RequestMipmap(material, "_DetailTintMap");
		RequestMipmap(material, "_DetailBlendMaskMap");
	}

	private void RequestMipmap(Material material, string textureName)
	{
		if (material.HasTexture(textureName))
		{
			Texture texture = material.GetTexture(textureName);
			Texture2D val = (Texture2D)(object)((texture is Texture2D) ? texture : null);
			if ((Object)(object)val != (Object)null)
			{
				val.requestedMipmapLevel = 0;
			}
		}
	}

	public void FreeMemory()
	{
		foreach (Material value in modifiedMaterials.Values)
		{
			Object.DestroyImmediate((Object)(object)value);
		}
		modifiedMaterials = new Dictionary<Material, Material>();
	}
}
