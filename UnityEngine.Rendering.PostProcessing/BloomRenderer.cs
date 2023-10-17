using UnityEngine.Scripting;

namespace UnityEngine.Rendering.PostProcessing;

[Preserve]
internal sealed class BloomRenderer : PostProcessEffectRenderer<Bloom>
{
	private enum Pass
	{
		Prefilter13,
		Prefilter4,
		Downsample13,
		Downsample4,
		UpsampleTent,
		UpsampleBox,
		DebugOverlayThreshold,
		DebugOverlayTent,
		DebugOverlayBox
	}

	private struct Level
	{
		internal int down;

		internal int up;
	}

	private Level[] m_Pyramid;

	private const int k_MaxPyramidSize = 16;

	public override void Init()
	{
		m_Pyramid = new Level[16];
		for (int i = 0; i < 16; i++)
		{
			m_Pyramid[i] = new Level
			{
				down = Shader.PropertyToID("_BloomMipDown" + i),
				up = Shader.PropertyToID("_BloomMipUp" + i)
			};
		}
	}

	public override void Render(PostProcessRenderContext context)
	{
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Invalid comparison between Unknown and I4
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_0346: Unknown result type (might be due to invalid IL or missing references)
		//IL_0354: Unknown result type (might be due to invalid IL or missing references)
		//IL_035b: Unknown result type (might be due to invalid IL or missing references)
		//IL_039f: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0422: Unknown result type (might be due to invalid IL or missing references)
		//IL_0429: Unknown result type (might be due to invalid IL or missing references)
		//IL_0430: Unknown result type (might be due to invalid IL or missing references)
		//IL_0439: Unknown result type (might be due to invalid IL or missing references)
		//IL_0457: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_052e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0588: Unknown result type (might be due to invalid IL or missing references)
		//IL_059c: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d4: Unknown result type (might be due to invalid IL or missing references)
		CommandBuffer command = context.command;
		command.BeginSample("BloomPyramid");
		PropertySheet propertySheet = context.propertySheets.Get(context.resources.shaders.bloom);
		propertySheet.properties.SetTexture(ShaderIDs.AutoExposureTex, context.autoExposureTexture);
		float num = Mathf.Clamp((float)base.settings.anamorphicRatio, -1f, 1f);
		float num2 = ((num < 0f) ? (0f - num) : 0f);
		float num3 = ((num > 0f) ? num : 0f);
		int num4 = Mathf.FloorToInt((float)context.screenWidth / (2f - num2));
		int num5 = Mathf.FloorToInt((float)context.screenHeight / (2f - num3));
		bool flag = context.stereoActive && context.stereoRenderingMode == PostProcessRenderContext.StereoRenderingMode.SinglePass && (int)context.camera.stereoTargetEye == 3;
		int num6 = (flag ? (num4 * 2) : num4);
		int num7 = Mathf.Max(num4, num5);
		float num8 = Mathf.Log((float)num7, 2f) + Mathf.Min(base.settings.diffusion.value, 10f) - 10f;
		int num9 = Mathf.FloorToInt(num8);
		int num10 = Mathf.Clamp(num9, 1, 16);
		float num11 = 0.5f + num8 - (float)num9;
		propertySheet.properties.SetFloat(ShaderIDs.SampleScale, num11);
		float num12 = Mathf.GammaToLinearSpace(base.settings.threshold.value);
		float num13 = num12 * base.settings.softKnee.value + 1E-05f;
		Vector4 val = default(Vector4);
		((Vector4)(ref val))._002Ector(num12, num12 - num13, num13 * 2f, 0.25f / num13);
		propertySheet.properties.SetVector(ShaderIDs.Threshold, val);
		float num14 = Mathf.GammaToLinearSpace(base.settings.clamp.value);
		propertySheet.properties.SetVector(ShaderIDs.Params, new Vector4(num14, 0f, 0f, 0f));
		int num15 = (base.settings.fastMode ? 1 : 0);
		RenderTargetIdentifier source = context.source;
		for (int i = 0; i < num10; i++)
		{
			int down = m_Pyramid[i].down;
			int up = m_Pyramid[i].up;
			int pass = ((i == 0) ? num15 : (2 + num15));
			context.GetScreenSpaceTemporaryRT(command, down, 0, context.sourceFormat, (RenderTextureReadWrite)0, (FilterMode)1, num6, num5);
			context.GetScreenSpaceTemporaryRT(command, up, 0, context.sourceFormat, (RenderTextureReadWrite)0, (FilterMode)1, num6, num5);
			command.BlitFullscreenTriangle(source, RenderTargetIdentifier.op_Implicit(down), propertySheet, pass);
			source = RenderTargetIdentifier.op_Implicit(down);
			num6 = ((flag && num6 / 2 % 2 > 0) ? (1 + num6 / 2) : (num6 / 2));
			num6 = Mathf.Max(num6, 1);
			num5 = Mathf.Max(num5 / 2, 1);
		}
		int num16 = m_Pyramid[num10 - 1].down;
		for (int num17 = num10 - 2; num17 >= 0; num17--)
		{
			int down2 = m_Pyramid[num17].down;
			int up2 = m_Pyramid[num17].up;
			command.SetGlobalTexture(ShaderIDs.BloomTex, RenderTargetIdentifier.op_Implicit(down2));
			command.BlitFullscreenTriangle(RenderTargetIdentifier.op_Implicit(num16), RenderTargetIdentifier.op_Implicit(up2), propertySheet, 4 + num15);
			num16 = up2;
		}
		Color linear = ((Color)(ref base.settings.color.value)).linear;
		float num18 = RuntimeUtilities.Exp2(base.settings.intensity.value / 10f) - 1f;
		Vector4 val2 = default(Vector4);
		((Vector4)(ref val2))._002Ector(num11, num18, base.settings.dirtIntensity.value, (float)num10);
		if (context.IsDebugOverlayEnabled(DebugOverlay.BloomThreshold))
		{
			context.PushDebugOverlay(command, context.source, propertySheet, 6);
		}
		else if (context.IsDebugOverlayEnabled(DebugOverlay.BloomBuffer))
		{
			propertySheet.properties.SetVector(ShaderIDs.ColorIntensity, new Vector4(linear.r, linear.g, linear.b, num18));
			context.PushDebugOverlay(command, RenderTargetIdentifier.op_Implicit(m_Pyramid[0].up), propertySheet, 7 + num15);
		}
		Texture val3 = (Texture)(((Object)(object)base.settings.dirtTexture.value == (Object)null) ? ((object)RuntimeUtilities.blackTexture) : ((object)base.settings.dirtTexture.value));
		float num19 = (float)val3.width / (float)val3.height;
		float num20 = (float)context.screenWidth / (float)context.screenHeight;
		Vector4 val4 = default(Vector4);
		((Vector4)(ref val4))._002Ector(1f, 1f, 0f, 0f);
		if (num19 > num20)
		{
			val4.x = num20 / num19;
			val4.z = (1f - val4.x) * 0.5f;
		}
		else if (num20 > num19)
		{
			val4.y = num19 / num20;
			val4.w = (1f - val4.y) * 0.5f;
		}
		PropertySheet uberSheet = context.uberSheet;
		if ((bool)base.settings.fastMode)
		{
			uberSheet.EnableKeyword("BLOOM_LOW");
		}
		else
		{
			uberSheet.EnableKeyword("BLOOM");
		}
		uberSheet.properties.SetVector(ShaderIDs.Bloom_DirtTileOffset, val4);
		uberSheet.properties.SetVector(ShaderIDs.Bloom_Settings, val2);
		uberSheet.properties.SetColor(ShaderIDs.Bloom_Color, linear);
		uberSheet.properties.SetTexture(ShaderIDs.Bloom_DirtTex, val3);
		command.SetGlobalTexture(ShaderIDs.BloomTex, RenderTargetIdentifier.op_Implicit(num16));
		for (int j = 0; j < num10; j++)
		{
			if (m_Pyramid[j].down != num16)
			{
				command.ReleaseTemporaryRT(m_Pyramid[j].down);
			}
			if (m_Pyramid[j].up != num16)
			{
				command.ReleaseTemporaryRT(m_Pyramid[j].up);
			}
		}
		command.EndSample("BloomPyramid");
		context.bloomBufferNameID = num16;
	}
}
