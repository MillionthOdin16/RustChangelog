using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

public class CathodeRenderer : PostProcessEffectRenderer<Cathode>
{
	private Texture2D noiseTex;

	private RenderTexture temporalRT;

	private Shader grayShader = Shader.Find("Hidden/Shader/Gray");

	private Shader primaryShader = Shader.Find("Hidden/Shader/PrimaryTransform");

	private Shader tvShader = Shader.Find("Hidden/Shader/TV");

	private Shader postTVShader = Shader.Find("Hidden/Shader/PostTV");

	private Shader trailShader = Shader.Find("Hidden/Shader/Trail");

	private readonly int _CathodeRT1 = Shader.PropertyToID("CathodeRT1");

	private readonly int _CathodeRT2 = Shader.PropertyToID("CathodeRT2");

	private readonly int _Intensity = Shader.PropertyToID("_Intensity");

	private readonly int _SizeX = Shader.PropertyToID("_SizeX");

	private readonly int _SizeY = Shader.PropertyToID("_SizeY");

	private readonly int _ChromaSubsampling = Shader.PropertyToID("_ChromaSubsampling");

	private readonly int _Sharpen = Shader.PropertyToID("_Sharpen");

	private readonly int _SharpenRadius = Shader.PropertyToID("_SharpenRadius");

	private readonly int _ColorNoise = Shader.PropertyToID("_ColorNoise");

	private readonly int _RestlessFoot = Shader.PropertyToID("_RestlessFoot");

	private readonly int _FootAmplitude = Shader.PropertyToID("_FootAmplitude");

	private readonly int _ChromaOffset = Shader.PropertyToID("_ChromaOffset");

	private readonly int _ChromaIntensity = Shader.PropertyToID("_ChromaIntensity");

	private readonly int _ChromaInstability = Shader.PropertyToID("_ChromaInstability");

	private readonly int _BurnIn = Shader.PropertyToID("_BurnIn");

	private readonly int _TapeDust = Shader.PropertyToID("_TapeDust");

	private readonly int _TrailTex = Shader.PropertyToID("_TrailTex");

	private readonly int _NoiseTex = Shader.PropertyToID("_NoiseTex");

	private readonly int _Gamma = Shader.PropertyToID("_Gamma");

	private readonly int _ResponseCurve = Shader.PropertyToID("_ResponseCurve");

	private readonly int _Saturation = Shader.PropertyToID("_Saturation");

	private readonly int _Wobble = Shader.PropertyToID("_Wobble");

	private readonly int _Black = Shader.PropertyToID("_Black");

	private readonly int _White = Shader.PropertyToID("_White");

	private readonly int _DynamicRangeMin = Shader.PropertyToID("_DynamicRangeMin");

	private readonly int _DynamicRangeMax = Shader.PropertyToID("_DynamicRangeMax");

	private readonly int _ScreenWhiteBal = Shader.PropertyToID("_ScreenWhiteBal");

	private readonly int _Trailing = Shader.PropertyToID("_Trailing");

	public override void Init()
	{
		base.Init();
		grayShader = Shader.Find("Hidden/Shader/Gray");
		primaryShader = Shader.Find("Hidden/Shader/PrimaryTransform");
		tvShader = Shader.Find("Hidden/Shader/TV");
		postTVShader = Shader.Find("Hidden/Shader/PostTV");
		trailShader = Shader.Find("Hidden/Shader/Trail");
		noiseTex = Resources.Load<Texture2D>("Noise");
	}

	public override void Release()
	{
		if ((Object)(object)noiseTex != (Object)null)
		{
			Resources.UnloadAsset((Object)(object)noiseTex);
			noiseTex = null;
		}
		if ((Object)(object)temporalRT != (Object)null)
		{
			Object.DestroyImmediate((Object)(object)temporalRT);
			temporalRT = null;
		}
		base.Release();
	}

	public override void Render(PostProcessRenderContext context)
	{
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Expected O, but got Unknown
		//IL_0818: Unknown result type (might be due to invalid IL or missing references)
		//IL_081e: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0713: Unknown result type (might be due to invalid IL or missing references)
		//IL_0715: Unknown result type (might be due to invalid IL or missing references)
		//IL_071e: Unknown result type (might be due to invalid IL or missing references)
		//IL_072d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0737: Unknown result type (might be due to invalid IL or missing references)
		//IL_0742: Unknown result type (might be due to invalid IL or missing references)
		//IL_0760: Unknown result type (might be due to invalid IL or missing references)
		//IL_076b: Unknown result type (might be due to invalid IL or missing references)
		//IL_078a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0795: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_07bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_07de: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e4: Unknown result type (might be due to invalid IL or missing references)
		CommandBuffer command = context.command;
		command.BeginSample("CathodeAnalogueVideo");
		int num = context.width / (int)base.settings.downscaleTemporal;
		int num2 = context.height / (int)base.settings.downscaleTemporal;
		if ((Object)(object)temporalRT == (Object)null || ((Texture)temporalRT).width != num || ((Texture)temporalRT).height != num2)
		{
			if ((Object)(object)temporalRT != (Object)null)
			{
				Object.DestroyImmediate((Object)(object)temporalRT);
			}
			temporalRT = new RenderTexture(num, num2, 0, (RenderTextureFormat)9);
		}
		if ((float)base.settings.intensity > 0f)
		{
			PropertySheet propertySheet = context.propertySheets.Get(grayShader);
			PropertySheet propertySheet2 = context.propertySheets.Get(primaryShader);
			PropertySheet propertySheet3 = context.propertySheets.Get(tvShader);
			PropertySheet propertySheet4 = context.propertySheets.Get(postTVShader);
			PropertySheet propertySheet5 = context.propertySheets.Get(trailShader);
			propertySheet.properties.Clear();
			propertySheet2.properties.Clear();
			propertySheet3.properties.Clear();
			propertySheet4.properties.Clear();
			propertySheet5.properties.Clear();
			propertySheet.properties.SetFloat(_Intensity, (float)base.settings.intensity);
			propertySheet.properties.SetFloat(_SizeX, (float)base.settings.horizontalBlur);
			propertySheet.properties.SetFloat(_SizeY, (float)base.settings.verticalBlur);
			propertySheet2.properties.SetFloat(_Intensity, (float)base.settings.intensity);
			propertySheet2.properties.SetFloat(_ChromaSubsampling, (float)base.settings.chromaSubsampling * (float)base.settings.intensity);
			propertySheet2.properties.SetFloat(_Sharpen, (float)base.settings.sharpen * (float)base.settings.intensity);
			propertySheet2.properties.SetFloat(_SharpenRadius, (float)base.settings.sharpenRadius * (float)base.settings.intensity);
			propertySheet2.properties.SetFloat(_ColorNoise, (float)base.settings.colorNoise * (float)base.settings.intensity);
			propertySheet2.properties.SetFloat(_RestlessFoot, (float)base.settings.restlessFoot * (float)base.settings.intensity);
			propertySheet2.properties.SetFloat(_FootAmplitude, (float)base.settings.footAmplitude * (float)base.settings.intensity);
			propertySheet2.properties.SetFloat(_ChromaOffset, (float)base.settings.chromaOffset * (float)base.settings.intensity);
			propertySheet2.properties.SetFloat(_ChromaIntensity, Mathf.Lerp(1f, (float)base.settings.chromaIntensity, (float)base.settings.intensity));
			propertySheet2.properties.SetFloat(_ChromaInstability, (float)base.settings.chromaInstability * (float)base.settings.intensity);
			propertySheet2.properties.SetFloat(_BurnIn, (float)base.settings.burnIn * (float)base.settings.intensity);
			propertySheet2.properties.SetFloat(_TapeDust, 1f - (float)base.settings.tapeDust * (float)base.settings.intensity);
			propertySheet2.properties.SetTexture(_TrailTex, (Texture)(object)temporalRT);
			propertySheet2.properties.SetTexture(_NoiseTex, (Texture)(object)noiseTex);
			propertySheet3.properties.SetFloat(_Intensity, (float)base.settings.intensity);
			propertySheet3.properties.SetFloat(_Gamma, 1f);
			propertySheet4.properties.SetFloat(_Intensity, (float)base.settings.intensity);
			propertySheet4.properties.SetFloat(_ResponseCurve, (float)base.settings.responseCurve * (float)base.settings.intensity);
			propertySheet4.properties.SetFloat(_Saturation, Mathf.Lerp(1f, (float)base.settings.saturation, (float)base.settings.intensity));
			propertySheet4.properties.SetFloat(_Wobble, (float)base.settings.wobble * (float)base.settings.intensity);
			propertySheet4.properties.SetFloat(_Black, base.settings.blackWhiteLevels.value.x * (float)base.settings.intensity);
			propertySheet4.properties.SetFloat(_White, 1f - (1f - base.settings.blackWhiteLevels.value.y) * (float)base.settings.intensity);
			propertySheet4.properties.SetFloat(_DynamicRangeMin, base.settings.dynamicRange.value.x * (float)base.settings.intensity);
			propertySheet4.properties.SetFloat(_DynamicRangeMax, 1f - (1f - base.settings.dynamicRange.value.y) * (float)base.settings.intensity);
			propertySheet4.properties.SetFloat(_ScreenWhiteBal, (float)base.settings.whiteBallance * (float)base.settings.intensity);
			propertySheet5.properties.SetFloat(_Trailing, 1f - (float)base.settings.cometTrailing * (float)base.settings.intensity);
			RenderTextureDescriptor val = default(RenderTextureDescriptor);
			((RenderTextureDescriptor)(ref val)).dimension = (TextureDimension)2;
			((RenderTextureDescriptor)(ref val)).width = context.width / (int)base.settings.downscale;
			((RenderTextureDescriptor)(ref val)).height = context.height / (int)base.settings.downscale;
			((RenderTextureDescriptor)(ref val)).depthBufferBits = 0;
			((RenderTextureDescriptor)(ref val)).colorFormat = (RenderTextureFormat)9;
			((RenderTextureDescriptor)(ref val)).useMipMap = true;
			((RenderTextureDescriptor)(ref val)).autoGenerateMips = true;
			((RenderTextureDescriptor)(ref val)).msaaSamples = 1;
			RenderTextureDescriptor val2 = val;
			command.GetTemporaryRT(_CathodeRT1, val2, (FilterMode)2);
			command.GetTemporaryRT(_CathodeRT2, val2, (FilterMode)2);
			command.BlitFullscreenTriangle(context.source, RenderTargetIdentifier.op_Implicit(_CathodeRT1), propertySheet, 0);
			command.BlitFullscreenTriangle(RenderTargetIdentifier.op_Implicit(_CathodeRT1), RenderTargetIdentifier.op_Implicit(_CathodeRT2), propertySheet2, 0);
			command.BlitFullscreenTriangle(RenderTargetIdentifier.op_Implicit(_CathodeRT1), RenderTargetIdentifier.op_Implicit((Texture)(object)temporalRT), propertySheet5, 0);
			command.BlitFullscreenTriangle(RenderTargetIdentifier.op_Implicit(_CathodeRT2), RenderTargetIdentifier.op_Implicit(_CathodeRT1), propertySheet4, 0);
			command.BlitFullscreenTriangle(RenderTargetIdentifier.op_Implicit(_CathodeRT1), context.destination, propertySheet3, 0);
			command.ReleaseTemporaryRT(_CathodeRT1);
			command.ReleaseTemporaryRT(_CathodeRT2);
		}
		else
		{
			command.BlitFullscreenTriangle(context.source, context.destination);
		}
		command.EndSample("CathodeAnalogueVideo");
	}
}
