using System;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering.PostProcessing;

[Serializable]
[Preserve]
internal sealed class ScalableAO : IAmbientOcclusionMethod
{
	private enum Pass
	{
		OcclusionEstimationForward,
		OcclusionEstimationDeferred,
		HorizontalBlurForward,
		HorizontalBlurDeferred,
		VerticalBlur,
		CompositionForward,
		CompositionDeferred,
		DebugOverlay
	}

	private RenderTexture m_Result;

	private PropertySheet m_PropertySheet;

	private AmbientOcclusion m_Settings;

	private readonly RenderTargetIdentifier[] m_MRT = (RenderTargetIdentifier[])(object)new RenderTargetIdentifier[2]
	{
		RenderTargetIdentifier.op_Implicit((BuiltinRenderTextureType)10),
		RenderTargetIdentifier.op_Implicit((BuiltinRenderTextureType)2)
	};

	private readonly int[] m_SampleCount = new int[5] { 4, 6, 10, 8, 12 };

	public ScalableAO(AmbientOcclusion settings)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		m_Settings = settings;
	}

	public DepthTextureMode GetCameraFlags()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		return (DepthTextureMode)3;
	}

	private void DoLazyInitialization(PostProcessRenderContext context)
	{
		m_PropertySheet = context.propertySheets.Get(context.resources.shaders.scalableAO);
		bool flag = false;
		if ((Object)(object)m_Result == (Object)null || !m_Result.IsCreated())
		{
			m_Result = context.GetScreenSpaceTemporaryRT(0, (RenderTextureFormat)0, (RenderTextureReadWrite)1);
			((Object)m_Result).hideFlags = (HideFlags)52;
			((Texture)m_Result).filterMode = (FilterMode)1;
			flag = true;
		}
		else if (((Texture)m_Result).width != context.width || ((Texture)m_Result).height != context.height)
		{
			m_Result.Release();
			((Texture)m_Result).width = context.width;
			((Texture)m_Result).height = context.height;
			flag = true;
		}
		if (flag)
		{
			m_Result.Create();
		}
	}

	private void Render(PostProcessRenderContext context, CommandBuffer cmd, int occlusionSource)
	{
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Invalid comparison between Unknown and I4
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		DoLazyInitialization(context);
		m_Settings.radius.value = Mathf.Max(m_Settings.radius.value, 0.0001f);
		bool flag = m_Settings.quality.value < AmbientOcclusionQuality.High;
		float value = m_Settings.intensity.value;
		float value2 = m_Settings.radius.value;
		float num = (flag ? 0.5f : 1f);
		float num2 = m_SampleCount[(int)m_Settings.quality.value];
		PropertySheet propertySheet = m_PropertySheet;
		propertySheet.ClearKeywords();
		propertySheet.properties.SetVector(ShaderIDs.AOParams, new Vector4(value, value2, num, num2));
		propertySheet.properties.SetVector(ShaderIDs.AOColor, Color.op_Implicit(Color.white - m_Settings.color.value));
		if ((int)context.camera.actualRenderingPath == 1 && RenderSettings.fog)
		{
			propertySheet.EnableKeyword("APPLY_FORWARD_FOG");
			propertySheet.properties.SetVector(ShaderIDs.FogParams, Vector4.op_Implicit(new Vector3(RenderSettings.fogDensity, RenderSettings.fogStartDistance, RenderSettings.fogEndDistance)));
		}
		int num3 = ((!flag) ? 1 : 2);
		int occlusionTexture = ShaderIDs.OcclusionTexture1;
		int widthOverride = context.width / num3;
		int heightOverride = context.height / num3;
		context.GetScreenSpaceTemporaryRT(cmd, occlusionTexture, 0, (RenderTextureFormat)0, (RenderTextureReadWrite)1, (FilterMode)1, widthOverride, heightOverride);
		cmd.BlitFullscreenTriangle(RenderTargetIdentifier.op_Implicit((BuiltinRenderTextureType)0), RenderTargetIdentifier.op_Implicit(occlusionTexture), propertySheet, occlusionSource);
		int occlusionTexture2 = ShaderIDs.OcclusionTexture2;
		context.GetScreenSpaceTemporaryRT(cmd, occlusionTexture2, 0, (RenderTextureFormat)0, (RenderTextureReadWrite)1, (FilterMode)1);
		cmd.BlitFullscreenTriangle(RenderTargetIdentifier.op_Implicit(occlusionTexture), RenderTargetIdentifier.op_Implicit(occlusionTexture2), propertySheet, 2 + occlusionSource);
		cmd.ReleaseTemporaryRT(occlusionTexture);
		cmd.BlitFullscreenTriangle(RenderTargetIdentifier.op_Implicit(occlusionTexture2), RenderTargetIdentifier.op_Implicit((Texture)(object)m_Result), propertySheet, 4);
		cmd.ReleaseTemporaryRT(occlusionTexture2);
		if (context.IsDebugOverlayEnabled(DebugOverlay.AmbientOcclusion))
		{
			context.PushDebugOverlay(cmd, RenderTargetIdentifier.op_Implicit((Texture)(object)m_Result), propertySheet, 7);
		}
	}

	public void RenderAfterOpaque(PostProcessRenderContext context)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		CommandBuffer command = context.command;
		command.BeginSample("Ambient Occlusion");
		Render(context, command, 0);
		command.SetGlobalTexture(ShaderIDs.SAOcclusionTexture, RenderTargetIdentifier.op_Implicit((Texture)(object)m_Result));
		command.BlitFullscreenTriangle(RenderTargetIdentifier.op_Implicit((BuiltinRenderTextureType)0), RenderTargetIdentifier.op_Implicit((BuiltinRenderTextureType)2), m_PropertySheet, 5, (RenderBufferLoadAction)0);
		command.EndSample("Ambient Occlusion");
	}

	public void RenderAmbientOnly(PostProcessRenderContext context)
	{
		CommandBuffer command = context.command;
		command.BeginSample("Ambient Occlusion Render");
		Render(context, command, 1);
		command.EndSample("Ambient Occlusion Render");
	}

	public void CompositeAmbientOnly(PostProcessRenderContext context)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		CommandBuffer command = context.command;
		command.BeginSample("Ambient Occlusion Composite");
		command.SetGlobalTexture(ShaderIDs.SAOcclusionTexture, RenderTargetIdentifier.op_Implicit((Texture)(object)m_Result));
		command.BlitFullscreenTriangle(RenderTargetIdentifier.op_Implicit((BuiltinRenderTextureType)0), m_MRT, RenderTargetIdentifier.op_Implicit((BuiltinRenderTextureType)2), m_PropertySheet, 6);
		command.EndSample("Ambient Occlusion Composite");
	}

	public void Release()
	{
		RuntimeUtilities.Destroy((Object)(object)m_Result);
		m_Result = null;
	}
}
