using UnityEngine;
using UnityEngine.Profiling;

namespace UnityStandardAssets.ImageEffects;

[ExecuteInEditMode]
[AddComponentMenu("Image Effects/Other/Scope Overlay")]
public class ScopeEffect : PostEffectsBase, IImageEffect
{
	public Material overlayMaterial = null;

	public override bool CheckResources()
	{
		return true;
	}

	public bool IsActive()
	{
		return ((Behaviour)this).enabled && ((PostEffectsBase)this).CheckResources();
	}

	public void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		Profiler.BeginSample("ScopeEffect");
		overlayMaterial.SetVector("_Screen", Vector4.op_Implicit(new Vector2((float)Screen.width, (float)Screen.height)));
		Graphics.Blit((Texture)(object)source, destination, overlayMaterial);
		Profiler.EndSample();
	}
}
