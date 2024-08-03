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
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		return (DepthTextureMode)5;
	}

	public override void Render(PostProcessRenderContext context)
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0348: Unknown result type (might be due to invalid IL or missing references)
		//IL_034e: Unknown result type (might be due to invalid IL or missing references)
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
