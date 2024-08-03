using System;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering.PostProcessing;

[Serializable]
[Preserve]
public sealed class Fog
{
	[Tooltip("Enables the internal deferred fog pass. Actual fog settings should be set in the Lighting panel.")]
	public bool enabled = true;

	[Tooltip("Mark true for the fog to ignore the skybox")]
	public bool excludeSkybox = true;

	internal DepthTextureMode GetCameraFlags()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		return (DepthTextureMode)1;
	}

	internal bool IsEnabledAndSupported(PostProcessRenderContext context)
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Invalid comparison between Unknown and I4
		return enabled && RenderSettings.fog && !RuntimeUtilities.scriptableRenderPipelineActive && Object.op_Implicit((Object)(object)context.resources.shaders.deferredFog) && context.resources.shaders.deferredFog.isSupported && (int)context.camera.actualRenderingPath == 3;
	}

	internal void Render(PostProcessRenderContext context)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		PropertySheet propertySheet = context.propertySheets.Get(context.resources.shaders.deferredFog);
		propertySheet.ClearKeywords();
		Color val;
		if (!RuntimeUtilities.isLinearColorSpace)
		{
			val = RenderSettings.fogColor;
		}
		else
		{
			Color fogColor = RenderSettings.fogColor;
			val = ((Color)(ref fogColor)).linear;
		}
		Color val2 = val;
		propertySheet.properties.SetVector(ShaderIDs.FogColor, Color.op_Implicit(val2));
		propertySheet.properties.SetVector(ShaderIDs.FogParams, Vector4.op_Implicit(new Vector3(RenderSettings.fogDensity, RenderSettings.fogStartDistance, RenderSettings.fogEndDistance)));
		CommandBuffer command = context.command;
		command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, excludeSkybox ? 1 : 0);
	}
}
