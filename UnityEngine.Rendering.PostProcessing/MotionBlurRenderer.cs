using UnityEngine.Scripting;

namespace UnityEngine.Rendering.PostProcessing;

[Preserve]
internal sealed class MotionBlurRenderer : PostProcessEffectRenderer<MotionBlur>
{
	private enum Pass
	{
		VelocitySetup,
		TileMax1,
		TileMax2,
		TileMaxV,
		NeighborMax,
		Reconstruction
	}

	public override DepthTextureMode GetCameraFlags()
	{
		return (DepthTextureMode)5;
	}

	public override void Render(PostProcessRenderContext context)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_0326: Unknown result type (might be due to invalid IL or missing references)
		//IL_032c: Unknown result type (might be due to invalid IL or missing references)
		CommandBuffer command = context.command;
		if (m_ResetHistory)
		{
			command.BlitFullscreenTriangle(context.source, context.destination);
			m_ResetHistory = false;
			return;
		}
		RenderTextureFormat val = (RenderTextureFormat)13;
		RenderTextureFormat val2 = (RenderTextureFormat)(((RenderTextureFormat)8).IsSupported() ? 8 : 0);
		PropertySheet propertySheet = context.propertySheets.Get(context.resources.shaders.motionBlur);
		command.BeginSample("MotionBlur");
		int num = (int)(5f * (float)context.height / 100f);
		int num2 = ((num - 1) / 8 + 1) * 8;
		float num3 = (float)base.settings.shutterAngle / 360f;
		propertySheet.properties.SetFloat(ShaderIDs.VelocityScale, num3);
		propertySheet.properties.SetFloat(ShaderIDs.MaxBlurRadius, (float)num);
		propertySheet.properties.SetFloat(ShaderIDs.RcpMaxBlurRadius, 1f / (float)num);
		int velocityTex = ShaderIDs.VelocityTex;
		command.GetTemporaryRT(velocityTex, context.width, context.height, 0, (FilterMode)0, val2, (RenderTextureReadWrite)1);
		command.BlitFullscreenTriangle(RenderTargetIdentifier.op_Implicit((BuiltinRenderTextureType)0), RenderTargetIdentifier.op_Implicit(velocityTex), propertySheet, 0);
		int tile2RT = ShaderIDs.Tile2RT;
		command.GetTemporaryRT(tile2RT, context.width / 2, context.height / 2, 0, (FilterMode)0, val, (RenderTextureReadWrite)1);
		command.BlitFullscreenTriangle(RenderTargetIdentifier.op_Implicit(velocityTex), RenderTargetIdentifier.op_Implicit(tile2RT), propertySheet, 1);
		int tile4RT = ShaderIDs.Tile4RT;
		command.GetTemporaryRT(tile4RT, context.width / 4, context.height / 4, 0, (FilterMode)0, val, (RenderTextureReadWrite)1);
		command.BlitFullscreenTriangle(RenderTargetIdentifier.op_Implicit(tile2RT), RenderTargetIdentifier.op_Implicit(tile4RT), propertySheet, 2);
		command.ReleaseTemporaryRT(tile2RT);
		int tile8RT = ShaderIDs.Tile8RT;
		command.GetTemporaryRT(tile8RT, context.width / 8, context.height / 8, 0, (FilterMode)0, val, (RenderTextureReadWrite)1);
		command.BlitFullscreenTriangle(RenderTargetIdentifier.op_Implicit(tile4RT), RenderTargetIdentifier.op_Implicit(tile8RT), propertySheet, 2);
		command.ReleaseTemporaryRT(tile4RT);
		Vector2 val3 = Vector2.one * ((float)num2 / 8f - 1f) * -0.5f;
		propertySheet.properties.SetVector(ShaderIDs.TileMaxOffs, Vector4.op_Implicit(val3));
		propertySheet.properties.SetFloat(ShaderIDs.TileMaxLoop, (float)(int)((float)num2 / 8f));
		int tileVRT = ShaderIDs.TileVRT;
		command.GetTemporaryRT(tileVRT, context.width / num2, context.height / num2, 0, (FilterMode)0, val, (RenderTextureReadWrite)1);
		command.BlitFullscreenTriangle(RenderTargetIdentifier.op_Implicit(tile8RT), RenderTargetIdentifier.op_Implicit(tileVRT), propertySheet, 3);
		command.ReleaseTemporaryRT(tile8RT);
		int neighborMaxTex = ShaderIDs.NeighborMaxTex;
		int num4 = context.width / num2;
		int num5 = context.height / num2;
		command.GetTemporaryRT(neighborMaxTex, num4, num5, 0, (FilterMode)0, val, (RenderTextureReadWrite)1);
		command.BlitFullscreenTriangle(RenderTargetIdentifier.op_Implicit(tileVRT), RenderTargetIdentifier.op_Implicit(neighborMaxTex), propertySheet, 4);
		command.ReleaseTemporaryRT(tileVRT);
		propertySheet.properties.SetFloat(ShaderIDs.LoopCount, (float)Mathf.Clamp((int)base.settings.sampleCount / 2, 1, 64));
		command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, 5);
		command.ReleaseTemporaryRT(velocityTex);
		command.ReleaseTemporaryRT(neighborMaxTex);
		command.EndSample("MotionBlur");
	}
}
