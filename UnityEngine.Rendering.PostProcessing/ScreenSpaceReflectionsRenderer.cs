using UnityEngine.Assertions;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering.PostProcessing;

[Preserve]
internal sealed class ScreenSpaceReflectionsRenderer : PostProcessEffectRenderer<ScreenSpaceReflections>
{
	private class QualityPreset
	{
		public int maximumIterationCount;

		public float thickness;

		public ScreenSpaceReflectionResolution downsampling;
	}

	private enum Pass
	{
		Test,
		Resolve,
		Reproject,
		Composite
	}

	private RenderTexture m_Resolve;

	private RenderTexture m_History;

	private int[] m_MipIDs;

	private readonly QualityPreset[] m_Presets = new QualityPreset[7]
	{
		new QualityPreset
		{
			maximumIterationCount = 10,
			thickness = 32f,
			downsampling = ScreenSpaceReflectionResolution.Downsampled
		},
		new QualityPreset
		{
			maximumIterationCount = 16,
			thickness = 32f,
			downsampling = ScreenSpaceReflectionResolution.Downsampled
		},
		new QualityPreset
		{
			maximumIterationCount = 32,
			thickness = 16f,
			downsampling = ScreenSpaceReflectionResolution.Downsampled
		},
		new QualityPreset
		{
			maximumIterationCount = 48,
			thickness = 8f,
			downsampling = ScreenSpaceReflectionResolution.Downsampled
		},
		new QualityPreset
		{
			maximumIterationCount = 16,
			thickness = 32f,
			downsampling = ScreenSpaceReflectionResolution.FullSize
		},
		new QualityPreset
		{
			maximumIterationCount = 48,
			thickness = 16f,
			downsampling = ScreenSpaceReflectionResolution.FullSize
		},
		new QualityPreset
		{
			maximumIterationCount = 128,
			thickness = 12f,
			downsampling = ScreenSpaceReflectionResolution.Supersampled
		}
	};

	public override DepthTextureMode GetCameraFlags()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		return (DepthTextureMode)5;
	}

	internal void CheckRT(ref RenderTexture rt, int width, int height, FilterMode filterMode, bool useMipMap)
	{
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Expected O, but got Unknown
		if ((Object)(object)rt == (Object)null || !rt.IsCreated() || ((Texture)rt).width != width || ((Texture)rt).height != height)
		{
			if ((Object)(object)rt != (Object)null)
			{
				rt.Release();
				RuntimeUtilities.Destroy((Object)(object)rt);
			}
			rt = new RenderTexture(width, height, 0, RuntimeUtilities.defaultHDRRenderTextureFormat)
			{
				filterMode = filterMode,
				useMipMap = useMipMap,
				autoGenerateMips = false,
				hideFlags = (HideFlags)61
			};
			rt.Create();
		}
	}

	public override void Render(PostProcessRenderContext context)
	{
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0345: Unknown result type (might be due to invalid IL or missing references)
		//IL_035b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0368: Unknown result type (might be due to invalid IL or missing references)
		//IL_0372: Unknown result type (might be due to invalid IL or missing references)
		//IL_039a: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_041e: Unknown result type (might be due to invalid IL or missing references)
		//IL_042b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0435: Unknown result type (might be due to invalid IL or missing references)
		//IL_046c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0477: Unknown result type (might be due to invalid IL or missing references)
		//IL_0497: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_057d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0595: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_060b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0624: Unknown result type (might be due to invalid IL or missing references)
		//IL_0629: Unknown result type (might be due to invalid IL or missing references)
		//IL_0680: Unknown result type (might be due to invalid IL or missing references)
		//IL_0686: Unknown result type (might be due to invalid IL or missing references)
		CommandBuffer command = context.command;
		command.BeginSample("Screen-space Reflections");
		if (base.settings.preset.value != ScreenSpaceReflectionPreset.Custom)
		{
			int value = (int)base.settings.preset.value;
			base.settings.maximumIterationCount.value = m_Presets[value].maximumIterationCount;
			base.settings.thickness.value = m_Presets[value].thickness;
			base.settings.resolution.value = m_Presets[value].downsampling;
		}
		base.settings.maximumMarchDistance.value = Mathf.Max(0f, base.settings.maximumMarchDistance.value);
		int num = Mathf.ClosestPowerOfTwo(Mathf.Min(context.width, context.height));
		if (base.settings.resolution.value == ScreenSpaceReflectionResolution.Downsampled)
		{
			num >>= 1;
		}
		else if (base.settings.resolution.value == ScreenSpaceReflectionResolution.Supersampled)
		{
			num <<= 1;
		}
		int num2 = Mathf.FloorToInt(Mathf.Log((float)num, 2f) - 3f);
		num2 = Mathf.Min(num2, 12);
		CheckRT(ref m_Resolve, num, num, (FilterMode)2, useMipMap: true);
		Texture2D val = context.resources.blueNoise256[0];
		PropertySheet propertySheet = context.propertySheets.Get(context.resources.shaders.screenSpaceReflections);
		propertySheet.properties.SetTexture(ShaderIDs.Noise, (Texture)(object)val);
		Matrix4x4 val2 = default(Matrix4x4);
		((Matrix4x4)(ref val2)).SetRow(0, new Vector4((float)num * 0.5f, 0f, 0f, (float)num * 0.5f));
		((Matrix4x4)(ref val2)).SetRow(1, new Vector4(0f, (float)num * 0.5f, 0f, (float)num * 0.5f));
		((Matrix4x4)(ref val2)).SetRow(2, new Vector4(0f, 0f, 1f, 0f));
		((Matrix4x4)(ref val2)).SetRow(3, new Vector4(0f, 0f, 0f, 1f));
		Matrix4x4 gPUProjectionMatrix = GL.GetGPUProjectionMatrix(context.camera.projectionMatrix, false);
		val2 *= gPUProjectionMatrix;
		propertySheet.properties.SetMatrix(ShaderIDs.ViewMatrix, context.camera.worldToCameraMatrix);
		MaterialPropertyBlock properties = propertySheet.properties;
		int inverseViewMatrix = ShaderIDs.InverseViewMatrix;
		Matrix4x4 worldToCameraMatrix = context.camera.worldToCameraMatrix;
		properties.SetMatrix(inverseViewMatrix, ((Matrix4x4)(ref worldToCameraMatrix)).inverse);
		propertySheet.properties.SetMatrix(ShaderIDs.InverseProjectionMatrix, ((Matrix4x4)(ref gPUProjectionMatrix)).inverse);
		propertySheet.properties.SetMatrix(ShaderIDs.ScreenSpaceProjectionMatrix, val2);
		propertySheet.properties.SetVector(ShaderIDs.Params, new Vector4(base.settings.vignette.value, base.settings.distanceFade.value, base.settings.maximumMarchDistance.value, (float)num2));
		propertySheet.properties.SetVector(ShaderIDs.Params2, new Vector4((float)context.width / (float)context.height, (float)num / (float)((Texture)val).width, base.settings.thickness.value, (float)base.settings.maximumIterationCount.value));
		command.GetTemporaryRT(ShaderIDs.Test, num, num, 0, (FilterMode)0, context.sourceFormat);
		command.BlitFullscreenTriangle(context.source, RenderTargetIdentifier.op_Implicit(ShaderIDs.Test), propertySheet, 0);
		if (context.isSceneView)
		{
			command.BlitFullscreenTriangle(context.source, RenderTargetIdentifier.op_Implicit((Texture)(object)m_Resolve), propertySheet, 1);
		}
		else
		{
			CheckRT(ref m_History, num, num, (FilterMode)1, useMipMap: false);
			if (m_ResetHistory)
			{
				context.command.BlitFullscreenTriangle(context.source, RenderTargetIdentifier.op_Implicit((Texture)(object)m_History));
				m_ResetHistory = false;
			}
			command.GetTemporaryRT(ShaderIDs.SSRResolveTemp, num, num, 0, (FilterMode)1, context.sourceFormat);
			command.BlitFullscreenTriangle(context.source, RenderTargetIdentifier.op_Implicit(ShaderIDs.SSRResolveTemp), propertySheet, 1);
			propertySheet.properties.SetTexture(ShaderIDs.History, (Texture)(object)m_History);
			command.BlitFullscreenTriangle(RenderTargetIdentifier.op_Implicit(ShaderIDs.SSRResolveTemp), RenderTargetIdentifier.op_Implicit((Texture)(object)m_Resolve), propertySheet, 2);
			command.CopyTexture(RenderTargetIdentifier.op_Implicit((Texture)(object)m_Resolve), 0, 0, RenderTargetIdentifier.op_Implicit((Texture)(object)m_History), 0, 0);
			command.ReleaseTemporaryRT(ShaderIDs.SSRResolveTemp);
		}
		command.ReleaseTemporaryRT(ShaderIDs.Test);
		if (m_MipIDs == null || m_MipIDs.Length == 0)
		{
			m_MipIDs = new int[12];
			for (int i = 0; i < 12; i++)
			{
				m_MipIDs[i] = Shader.PropertyToID("_SSRGaussianMip" + i);
			}
		}
		ComputeShader gaussianDownsample = context.resources.computeShaders.gaussianDownsample;
		int num3 = gaussianDownsample.FindKernel("KMain");
		RenderTargetIdentifier val3 = default(RenderTargetIdentifier);
		((RenderTargetIdentifier)(ref val3))._002Ector((Texture)(object)m_Resolve);
		for (int j = 0; j < num2; j++)
		{
			num >>= 1;
			Assert.IsTrue(num > 0);
			command.GetTemporaryRT(m_MipIDs[j], num, num, 0, (FilterMode)1, context.sourceFormat, (RenderTextureReadWrite)0, 1, true);
			command.SetComputeTextureParam(gaussianDownsample, num3, "_Source", val3);
			command.SetComputeTextureParam(gaussianDownsample, num3, "_Result", RenderTargetIdentifier.op_Implicit(m_MipIDs[j]));
			command.SetComputeVectorParam(gaussianDownsample, "_Size", new Vector4((float)num, (float)num, 1f / (float)num, 1f / (float)num));
			command.DispatchCompute(gaussianDownsample, num3, num / 8, num / 8, 1);
			command.CopyTexture(RenderTargetIdentifier.op_Implicit(m_MipIDs[j]), 0, 0, RenderTargetIdentifier.op_Implicit((Texture)(object)m_Resolve), 0, j + 1);
			val3 = RenderTargetIdentifier.op_Implicit(m_MipIDs[j]);
		}
		for (int k = 0; k < num2; k++)
		{
			command.ReleaseTemporaryRT(m_MipIDs[k]);
		}
		propertySheet.properties.SetTexture(ShaderIDs.Resolve, (Texture)(object)m_Resolve);
		command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, 3);
		command.EndSample("Screen-space Reflections");
	}

	public override void Release()
	{
		RuntimeUtilities.Destroy((Object)(object)m_Resolve);
		RuntimeUtilities.Destroy((Object)(object)m_History);
		m_Resolve = null;
		m_History = null;
	}
}
