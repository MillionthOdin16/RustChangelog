using UnityEngine.Experimental.Rendering;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering.PostProcessing;

[Preserve]
internal sealed class ColorGradingRenderer : PostProcessEffectRenderer<ColorGrading>
{
	private enum Pass
	{
		LutGenLDRFromScratch,
		LutGenLDR,
		LutGenHDR2D
	}

	private Texture2D m_GradingCurves;

	private readonly Color[] m_Pixels = (Color[])(object)new Color[256];

	private RenderTexture m_InternalLdrLut;

	private RenderTexture m_InternalLogLut;

	private const int k_Lut2DSize = 32;

	private const int k_Lut3DSize = 33;

	private readonly HableCurve m_HableCurve = new HableCurve();

	public override void Render(PostProcessRenderContext context)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Invalid comparison between Unknown and I4
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Invalid comparison between Unknown and I4
		GradingMode value = base.settings.gradingMode.value;
		bool flag = SystemInfo.supports3DRenderTextures && SystemInfo.supportsComputeShaders && (Object)(object)context.resources.computeShaders.lut3DBaker != (Object)null && (int)SystemInfo.graphicsDeviceType != 17 && (int)SystemInfo.graphicsDeviceType != 11;
		if (value == GradingMode.External)
		{
			RenderExternalPipeline3D(context);
		}
		else if (value == GradingMode.HighDefinitionRange && flag)
		{
			RenderHDRPipeline3D(context);
		}
		else if (value == GradingMode.HighDefinitionRange)
		{
			RenderHDRPipeline2D(context);
		}
		else
		{
			RenderLDRPipeline2D(context);
		}
	}

	private void RenderExternalPipeline3D(PostProcessRenderContext context)
	{
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		Texture value = base.settings.externalLut.value;
		if (!((Object)(object)value == (Object)null))
		{
			PropertySheet uberSheet = context.uberSheet;
			uberSheet.EnableKeyword("COLOR_GRADING_HDR_3D");
			uberSheet.properties.SetTexture(ShaderIDs.Lut3D, value);
			uberSheet.properties.SetVector(ShaderIDs.Lut3D_Params, Vector4.op_Implicit(new Vector2(1f / (float)value.width, (float)value.width - 1f)));
			uberSheet.properties.SetFloat(ShaderIDs.PostExposure, RuntimeUtilities.Exp2(base.settings.postExposure.value));
			context.logLut = value;
		}
	}

	private void RenderHDRPipeline3D(PostProcessRenderContext context)
	{
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0306: Unknown result type (might be due to invalid IL or missing references)
		//IL_030d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0314: Unknown result type (might be due to invalid IL or missing references)
		//IL_0320: Unknown result type (might be due to invalid IL or missing references)
		//IL_0333: Unknown result type (might be due to invalid IL or missing references)
		//IL_033a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0341: Unknown result type (might be due to invalid IL or missing references)
		//IL_034d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0360: Unknown result type (might be due to invalid IL or missing references)
		//IL_0367: Unknown result type (might be due to invalid IL or missing references)
		//IL_036e: Unknown result type (might be due to invalid IL or missing references)
		//IL_037a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0395: Unknown result type (might be due to invalid IL or missing references)
		//IL_058c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0591: Unknown result type (might be due to invalid IL or missing references)
		//IL_043c: Unknown result type (might be due to invalid IL or missing references)
		//IL_045a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0478: Unknown result type (might be due to invalid IL or missing references)
		//IL_0496: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f0: Unknown result type (might be due to invalid IL or missing references)
		CheckInternalLogLut();
		ComputeShader lut3DBaker = context.resources.computeShaders.lut3DBaker;
		int num = 0;
		switch (base.settings.tonemapper.value)
		{
		case Tonemapper.None:
			num = lut3DBaker.FindKernel("KGenLut3D_NoTonemap");
			break;
		case Tonemapper.Neutral:
			num = lut3DBaker.FindKernel("KGenLut3D_NeutralTonemap");
			break;
		case Tonemapper.ACES:
			num = lut3DBaker.FindKernel("KGenLut3D_AcesTonemap");
			break;
		case Tonemapper.Custom:
			num = lut3DBaker.FindKernel("KGenLut3D_CustomTonemap");
			break;
		}
		CommandBuffer command = context.command;
		command.SetComputeTextureParam(lut3DBaker, num, "_Output", RenderTargetIdentifier.op_Implicit((Texture)(object)m_InternalLogLut));
		command.SetComputeVectorParam(lut3DBaker, "_Size", new Vector4(33f, 1f / 32f, 0f, 0f));
		Vector3 val = ColorUtilities.ComputeColorBalance(base.settings.temperature.value, base.settings.tint.value);
		command.SetComputeVectorParam(lut3DBaker, "_ColorBalance", Vector4.op_Implicit(val));
		command.SetComputeVectorParam(lut3DBaker, "_ColorFilter", Color.op_Implicit(base.settings.colorFilter.value));
		float num2 = base.settings.hueShift.value / 360f;
		float num3 = base.settings.saturation.value / 100f + 1f;
		float num4 = base.settings.contrast.value / 100f + 1f;
		command.SetComputeVectorParam(lut3DBaker, "_HueSatCon", new Vector4(num2, num3, num4, 0f));
		Vector4 val2 = default(Vector4);
		((Vector4)(ref val2))._002Ector((float)base.settings.mixerRedOutRedIn, (float)base.settings.mixerRedOutGreenIn, (float)base.settings.mixerRedOutBlueIn, 0f);
		Vector4 val3 = default(Vector4);
		((Vector4)(ref val3))._002Ector((float)base.settings.mixerGreenOutRedIn, (float)base.settings.mixerGreenOutGreenIn, (float)base.settings.mixerGreenOutBlueIn, 0f);
		Vector4 val4 = default(Vector4);
		((Vector4)(ref val4))._002Ector((float)base.settings.mixerBlueOutRedIn, (float)base.settings.mixerBlueOutGreenIn, (float)base.settings.mixerBlueOutBlueIn, 0f);
		command.SetComputeVectorParam(lut3DBaker, "_ChannelMixerRed", val2 / 100f);
		command.SetComputeVectorParam(lut3DBaker, "_ChannelMixerGreen", val3 / 100f);
		command.SetComputeVectorParam(lut3DBaker, "_ChannelMixerBlue", val4 / 100f);
		Vector3 val5 = ColorUtilities.ColorToLift(base.settings.lift.value * 0.2f);
		Vector3 val6 = ColorUtilities.ColorToGain(base.settings.gain.value * 0.8f);
		Vector3 val7 = ColorUtilities.ColorToInverseGamma(base.settings.gamma.value * 0.8f);
		command.SetComputeVectorParam(lut3DBaker, "_Lift", new Vector4(val5.x, val5.y, val5.z, 0f));
		command.SetComputeVectorParam(lut3DBaker, "_InvGamma", new Vector4(val7.x, val7.y, val7.z, 0f));
		command.SetComputeVectorParam(lut3DBaker, "_Gain", new Vector4(val6.x, val6.y, val6.z, 0f));
		command.SetComputeTextureParam(lut3DBaker, num, "_Curves", RenderTargetIdentifier.op_Implicit((Texture)(object)GetCurveTexture(hdr: true)));
		if (base.settings.tonemapper.value == Tonemapper.Custom)
		{
			m_HableCurve.Init(base.settings.toneCurveToeStrength.value, base.settings.toneCurveToeLength.value, base.settings.toneCurveShoulderStrength.value, base.settings.toneCurveShoulderLength.value, base.settings.toneCurveShoulderAngle.value, base.settings.toneCurveGamma.value);
			command.SetComputeVectorParam(lut3DBaker, "_CustomToneCurve", m_HableCurve.uniforms.curve);
			command.SetComputeVectorParam(lut3DBaker, "_ToeSegmentA", m_HableCurve.uniforms.toeSegmentA);
			command.SetComputeVectorParam(lut3DBaker, "_ToeSegmentB", m_HableCurve.uniforms.toeSegmentB);
			command.SetComputeVectorParam(lut3DBaker, "_MidSegmentA", m_HableCurve.uniforms.midSegmentA);
			command.SetComputeVectorParam(lut3DBaker, "_MidSegmentB", m_HableCurve.uniforms.midSegmentB);
			command.SetComputeVectorParam(lut3DBaker, "_ShoSegmentA", m_HableCurve.uniforms.shoSegmentA);
			command.SetComputeVectorParam(lut3DBaker, "_ShoSegmentB", m_HableCurve.uniforms.shoSegmentB);
		}
		context.command.BeginSample("HdrColorGradingLut3D");
		int num5 = Mathf.CeilToInt(8.25f);
		command.DispatchCompute(lut3DBaker, num, num5, num5, num5);
		context.command.EndSample("HdrColorGradingLut3D");
		RenderTexture internalLogLut = m_InternalLogLut;
		PropertySheet uberSheet = context.uberSheet;
		uberSheet.EnableKeyword("COLOR_GRADING_HDR_3D");
		uberSheet.properties.SetTexture(ShaderIDs.Lut3D, (Texture)(object)internalLogLut);
		uberSheet.properties.SetVector(ShaderIDs.Lut3D_Params, Vector4.op_Implicit(new Vector2(1f / (float)((Texture)internalLogLut).width, (float)((Texture)internalLogLut).width - 1f)));
		uberSheet.properties.SetFloat(ShaderIDs.PostExposure, RuntimeUtilities.Exp2(base.settings.postExposure.value));
		context.logLut = (Texture)(object)internalLogLut;
	}

	private void RenderHDRPipeline2D(PostProcessRenderContext context)
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_040e: Unknown result type (might be due to invalid IL or missing references)
		//IL_042f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0450: Unknown result type (might be due to invalid IL or missing references)
		//IL_0471: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0548: Unknown result type (might be due to invalid IL or missing references)
		//IL_054d: Unknown result type (might be due to invalid IL or missing references)
		CheckInternalStripLut();
		PropertySheet propertySheet = context.propertySheets.Get(context.resources.shaders.lut2DBaker);
		propertySheet.ClearKeywords();
		propertySheet.properties.SetVector(ShaderIDs.Lut2D_Params, new Vector4(32f, 0.00048828125f, 1f / 64f, 1.032258f));
		Vector3 val = ColorUtilities.ComputeColorBalance(base.settings.temperature.value, base.settings.tint.value);
		propertySheet.properties.SetVector(ShaderIDs.ColorBalance, Vector4.op_Implicit(val));
		propertySheet.properties.SetVector(ShaderIDs.ColorFilter, Color.op_Implicit(base.settings.colorFilter.value));
		float num = base.settings.hueShift.value / 360f;
		float num2 = base.settings.saturation.value / 100f + 1f;
		float num3 = base.settings.contrast.value / 100f + 1f;
		propertySheet.properties.SetVector(ShaderIDs.HueSatCon, Vector4.op_Implicit(new Vector3(num, num2, num3)));
		Vector3 val2 = default(Vector3);
		((Vector3)(ref val2))._002Ector((float)base.settings.mixerRedOutRedIn, (float)base.settings.mixerRedOutGreenIn, (float)base.settings.mixerRedOutBlueIn);
		Vector3 val3 = default(Vector3);
		((Vector3)(ref val3))._002Ector((float)base.settings.mixerGreenOutRedIn, (float)base.settings.mixerGreenOutGreenIn, (float)base.settings.mixerGreenOutBlueIn);
		Vector3 val4 = default(Vector3);
		((Vector3)(ref val4))._002Ector((float)base.settings.mixerBlueOutRedIn, (float)base.settings.mixerBlueOutGreenIn, (float)base.settings.mixerBlueOutBlueIn);
		propertySheet.properties.SetVector(ShaderIDs.ChannelMixerRed, Vector4.op_Implicit(val2 / 100f));
		propertySheet.properties.SetVector(ShaderIDs.ChannelMixerGreen, Vector4.op_Implicit(val3 / 100f));
		propertySheet.properties.SetVector(ShaderIDs.ChannelMixerBlue, Vector4.op_Implicit(val4 / 100f));
		Vector3 val5 = ColorUtilities.ColorToLift(base.settings.lift.value * 0.2f);
		Vector3 val6 = ColorUtilities.ColorToGain(base.settings.gain.value * 0.8f);
		Vector3 val7 = ColorUtilities.ColorToInverseGamma(base.settings.gamma.value * 0.8f);
		propertySheet.properties.SetVector(ShaderIDs.Lift, Vector4.op_Implicit(val5));
		propertySheet.properties.SetVector(ShaderIDs.InvGamma, Vector4.op_Implicit(val7));
		propertySheet.properties.SetVector(ShaderIDs.Gain, Vector4.op_Implicit(val6));
		propertySheet.properties.SetTexture(ShaderIDs.Curves, (Texture)(object)GetCurveTexture(hdr: true));
		switch (base.settings.tonemapper.value)
		{
		case Tonemapper.Custom:
			propertySheet.EnableKeyword("TONEMAPPING_CUSTOM");
			m_HableCurve.Init(base.settings.toneCurveToeStrength.value, base.settings.toneCurveToeLength.value, base.settings.toneCurveShoulderStrength.value, base.settings.toneCurveShoulderLength.value, base.settings.toneCurveShoulderAngle.value, base.settings.toneCurveGamma.value);
			propertySheet.properties.SetVector(ShaderIDs.CustomToneCurve, m_HableCurve.uniforms.curve);
			propertySheet.properties.SetVector(ShaderIDs.ToeSegmentA, m_HableCurve.uniforms.toeSegmentA);
			propertySheet.properties.SetVector(ShaderIDs.ToeSegmentB, m_HableCurve.uniforms.toeSegmentB);
			propertySheet.properties.SetVector(ShaderIDs.MidSegmentA, m_HableCurve.uniforms.midSegmentA);
			propertySheet.properties.SetVector(ShaderIDs.MidSegmentB, m_HableCurve.uniforms.midSegmentB);
			propertySheet.properties.SetVector(ShaderIDs.ShoSegmentA, m_HableCurve.uniforms.shoSegmentA);
			propertySheet.properties.SetVector(ShaderIDs.ShoSegmentB, m_HableCurve.uniforms.shoSegmentB);
			break;
		case Tonemapper.ACES:
			propertySheet.EnableKeyword("TONEMAPPING_ACES");
			break;
		case Tonemapper.Neutral:
			propertySheet.EnableKeyword("TONEMAPPING_NEUTRAL");
			break;
		}
		context.command.BeginSample("HdrColorGradingLut2D");
		context.command.BlitFullscreenTriangle(RenderTargetIdentifier.op_Implicit((BuiltinRenderTextureType)0), RenderTargetIdentifier.op_Implicit((Texture)(object)m_InternalLdrLut), propertySheet, 2);
		context.command.EndSample("HdrColorGradingLut2D");
		RenderTexture internalLdrLut = m_InternalLdrLut;
		PropertySheet uberSheet = context.uberSheet;
		uberSheet.EnableKeyword("COLOR_GRADING_HDR_2D");
		uberSheet.properties.SetVector(ShaderIDs.Lut2D_Params, Vector4.op_Implicit(new Vector3(1f / (float)((Texture)internalLdrLut).width, 1f / (float)((Texture)internalLdrLut).height, (float)((Texture)internalLdrLut).height - 1f)));
		uberSheet.properties.SetTexture(ShaderIDs.Lut2D, (Texture)(object)internalLdrLut);
		uberSheet.properties.SetFloat(ShaderIDs.PostExposure, RuntimeUtilities.Exp2(base.settings.postExposure.value));
	}

	private void RenderLDRPipeline2D(PostProcessRenderContext context)
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0364: Unknown result type (might be due to invalid IL or missing references)
		//IL_036f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0465: Unknown result type (might be due to invalid IL or missing references)
		//IL_046a: Unknown result type (might be due to invalid IL or missing references)
		CheckInternalStripLut();
		PropertySheet propertySheet = context.propertySheets.Get(context.resources.shaders.lut2DBaker);
		propertySheet.ClearKeywords();
		propertySheet.properties.SetVector(ShaderIDs.Lut2D_Params, new Vector4(32f, 0.00048828125f, 1f / 64f, 1.032258f));
		Vector3 val = ColorUtilities.ComputeColorBalance(base.settings.temperature.value, base.settings.tint.value);
		propertySheet.properties.SetVector(ShaderIDs.ColorBalance, Vector4.op_Implicit(val));
		propertySheet.properties.SetVector(ShaderIDs.ColorFilter, Color.op_Implicit(base.settings.colorFilter.value));
		float num = base.settings.hueShift.value / 360f;
		float num2 = base.settings.saturation.value / 100f + 1f;
		float num3 = base.settings.contrast.value / 100f + 1f;
		propertySheet.properties.SetVector(ShaderIDs.HueSatCon, Vector4.op_Implicit(new Vector3(num, num2, num3)));
		Vector3 val2 = default(Vector3);
		((Vector3)(ref val2))._002Ector((float)base.settings.mixerRedOutRedIn, (float)base.settings.mixerRedOutGreenIn, (float)base.settings.mixerRedOutBlueIn);
		Vector3 val3 = default(Vector3);
		((Vector3)(ref val3))._002Ector((float)base.settings.mixerGreenOutRedIn, (float)base.settings.mixerGreenOutGreenIn, (float)base.settings.mixerGreenOutBlueIn);
		Vector3 val4 = default(Vector3);
		((Vector3)(ref val4))._002Ector((float)base.settings.mixerBlueOutRedIn, (float)base.settings.mixerBlueOutGreenIn, (float)base.settings.mixerBlueOutBlueIn);
		propertySheet.properties.SetVector(ShaderIDs.ChannelMixerRed, Vector4.op_Implicit(val2 / 100f));
		propertySheet.properties.SetVector(ShaderIDs.ChannelMixerGreen, Vector4.op_Implicit(val3 / 100f));
		propertySheet.properties.SetVector(ShaderIDs.ChannelMixerBlue, Vector4.op_Implicit(val4 / 100f));
		Vector3 val5 = ColorUtilities.ColorToLift(base.settings.lift.value);
		Vector3 val6 = ColorUtilities.ColorToGain(base.settings.gain.value);
		Vector3 val7 = ColorUtilities.ColorToInverseGamma(base.settings.gamma.value);
		propertySheet.properties.SetVector(ShaderIDs.Lift, Vector4.op_Implicit(val5));
		propertySheet.properties.SetVector(ShaderIDs.InvGamma, Vector4.op_Implicit(val7));
		propertySheet.properties.SetVector(ShaderIDs.Gain, Vector4.op_Implicit(val6));
		propertySheet.properties.SetFloat(ShaderIDs.Brightness, (base.settings.brightness.value + 100f) / 100f);
		propertySheet.properties.SetTexture(ShaderIDs.Curves, (Texture)(object)GetCurveTexture(hdr: false));
		context.command.BeginSample("LdrColorGradingLut2D");
		Texture value = base.settings.ldrLut.value;
		if ((Object)(object)value == (Object)null || value.width != value.height * value.height)
		{
			context.command.BlitFullscreenTriangle(RenderTargetIdentifier.op_Implicit((BuiltinRenderTextureType)0), RenderTargetIdentifier.op_Implicit((Texture)(object)m_InternalLdrLut), propertySheet, 0);
		}
		else
		{
			propertySheet.properties.SetVector(ShaderIDs.UserLut2D_Params, new Vector4(1f / (float)value.width, 1f / (float)value.height, (float)value.height - 1f, (float)base.settings.ldrLutContribution));
			context.command.BlitFullscreenTriangle(RenderTargetIdentifier.op_Implicit(value), RenderTargetIdentifier.op_Implicit((Texture)(object)m_InternalLdrLut), propertySheet, 1);
		}
		context.command.EndSample("LdrColorGradingLut2D");
		RenderTexture internalLdrLut = m_InternalLdrLut;
		PropertySheet uberSheet = context.uberSheet;
		uberSheet.EnableKeyword("COLOR_GRADING_LDR_2D");
		uberSheet.properties.SetVector(ShaderIDs.Lut2D_Params, Vector4.op_Implicit(new Vector3(1f / (float)((Texture)internalLdrLut).width, 1f / (float)((Texture)internalLdrLut).height, (float)((Texture)internalLdrLut).height - 1f)));
		uberSheet.properties.SetTexture(ShaderIDs.Lut2D, (Texture)(object)internalLdrLut);
	}

	private void CheckInternalLogLut()
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Expected O, but got Unknown
		if ((Object)(object)m_InternalLogLut == (Object)null || !m_InternalLogLut.IsCreated())
		{
			RuntimeUtilities.Destroy((Object)(object)m_InternalLogLut);
			RenderTextureFormat lutFormat = GetLutFormat();
			m_InternalLogLut = new RenderTexture(33, 33, 0, lutFormat, (RenderTextureReadWrite)1)
			{
				name = "Color Grading Log Lut",
				dimension = (TextureDimension)3,
				hideFlags = (HideFlags)52,
				filterMode = (FilterMode)1,
				wrapMode = (TextureWrapMode)1,
				anisoLevel = 0,
				enableRandomWrite = true,
				volumeDepth = 33,
				autoGenerateMips = false,
				useMipMap = false
			};
			m_InternalLogLut.Create();
		}
	}

	private void CheckInternalStripLut()
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Expected O, but got Unknown
		if ((Object)(object)m_InternalLdrLut == (Object)null || !m_InternalLdrLut.IsCreated())
		{
			RuntimeUtilities.Destroy((Object)(object)m_InternalLdrLut);
			RenderTextureFormat lutFormat = GetLutFormat();
			m_InternalLdrLut = new RenderTexture(1024, 32, 0, lutFormat, (RenderTextureReadWrite)1)
			{
				name = "Color Grading Strip Lut",
				hideFlags = (HideFlags)52,
				filterMode = (FilterMode)1,
				wrapMode = (TextureWrapMode)1,
				anisoLevel = 0,
				autoGenerateMips = false,
				useMipMap = false
			};
			m_InternalLdrLut.Create();
		}
	}

	private Texture2D GetCurveTexture(bool hdr)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Expected O, but got Unknown
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)m_GradingCurves == (Object)null)
		{
			TextureFormat curveFormat = GetCurveFormat();
			m_GradingCurves = new Texture2D(128, 2, curveFormat, false, true)
			{
				name = "Internal Curves Texture",
				hideFlags = (HideFlags)52,
				anisoLevel = 0,
				wrapMode = (TextureWrapMode)1,
				filterMode = (FilterMode)1
			};
		}
		Spline value = base.settings.hueVsHueCurve.value;
		Spline value2 = base.settings.hueVsSatCurve.value;
		Spline value3 = base.settings.satVsSatCurve.value;
		Spline value4 = base.settings.lumVsSatCurve.value;
		Spline value5 = base.settings.masterCurve.value;
		Spline value6 = base.settings.redCurve.value;
		Spline value7 = base.settings.greenCurve.value;
		Spline value8 = base.settings.blueCurve.value;
		Color[] pixels = m_Pixels;
		for (int i = 0; i < 128; i++)
		{
			float num = value.cachedData[i];
			float num2 = value2.cachedData[i];
			float num3 = value3.cachedData[i];
			float num4 = value4.cachedData[i];
			pixels[i] = new Color(num, num2, num3, num4);
			if (!hdr)
			{
				float num5 = value5.cachedData[i];
				float num6 = value6.cachedData[i];
				float num7 = value7.cachedData[i];
				float num8 = value8.cachedData[i];
				pixels[i + 128] = new Color(num6, num7, num8, num5);
			}
		}
		m_GradingCurves.SetPixels(pixels);
		m_GradingCurves.Apply(false, false);
		return m_GradingCurves;
	}

	private static bool IsRenderTextureFormatSupportedForLinearFiltering(RenderTextureFormat format)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		GraphicsFormat graphicsFormat = GraphicsFormatUtility.GetGraphicsFormat(format, (RenderTextureReadWrite)1);
		return SystemInfo.IsFormatSupported(graphicsFormat, (FormatUsage)1);
	}

	private static RenderTextureFormat GetLutFormat()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		RenderTextureFormat val = (RenderTextureFormat)2;
		if (!IsRenderTextureFormatSupportedForLinearFiltering(val))
		{
			val = (RenderTextureFormat)8;
			if (!IsRenderTextureFormatSupportedForLinearFiltering(val))
			{
				val = (RenderTextureFormat)0;
			}
		}
		return val;
	}

	private static TextureFormat GetCurveFormat()
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		TextureFormat val = (TextureFormat)17;
		if (!SystemInfo.SupportsTextureFormat(val))
		{
			val = (TextureFormat)5;
		}
		return val;
	}

	public override void Release()
	{
		RuntimeUtilities.Destroy((Object)(object)m_InternalLdrLut);
		m_InternalLdrLut = null;
		RuntimeUtilities.Destroy((Object)(object)m_InternalLogLut);
		m_InternalLogLut = null;
		RuntimeUtilities.Destroy((Object)(object)m_GradingCurves);
		m_GradingCurves = null;
	}
}
