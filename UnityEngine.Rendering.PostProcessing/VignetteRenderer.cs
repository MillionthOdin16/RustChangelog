using UnityEngine.Scripting;

namespace UnityEngine.Rendering.PostProcessing;

[Preserve]
internal sealed class VignetteRenderer : PostProcessEffectRenderer<Vignette>
{
	public override void Render(PostProcessRenderContext context)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		PropertySheet uberSheet = context.uberSheet;
		uberSheet.EnableKeyword("VIGNETTE");
		uberSheet.properties.SetColor(ShaderIDs.Vignette_Color, base.settings.color.value);
		if ((VignetteMode)base.settings.mode == VignetteMode.Classic)
		{
			uberSheet.properties.SetFloat(ShaderIDs.Vignette_Mode, 0f);
			uberSheet.properties.SetVector(ShaderIDs.Vignette_Center, Vector4.op_Implicit(base.settings.center.value));
			float num = (1f - base.settings.roundness.value) * 6f + base.settings.roundness.value;
			uberSheet.properties.SetVector(ShaderIDs.Vignette_Settings, new Vector4(base.settings.intensity.value * 3f, base.settings.smoothness.value * 5f, num, base.settings.rounded.value ? 1f : 0f));
		}
		else
		{
			uberSheet.properties.SetFloat(ShaderIDs.Vignette_Mode, 1f);
			uberSheet.properties.SetTexture(ShaderIDs.Vignette_Mask, base.settings.mask.value);
			uberSheet.properties.SetFloat(ShaderIDs.Vignette_Opacity, Mathf.Clamp01(base.settings.opacity.value));
		}
	}
}
