using UnityEngine;

public class SprayCanSpray_Decal : SprayCanSpray, ICustomMaterialReplacer, IPropRenderNotify, INotifyLOD
{
	public DeferredDecal DecalComponent = null;

	public GameObject IconPreviewRoot = null;

	public Material DefaultMaterial = null;
}
