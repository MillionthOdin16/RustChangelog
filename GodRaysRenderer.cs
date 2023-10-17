using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

public class GodRaysRenderer : PostProcessEffectRenderer<GodRays>
{
	private const int PASS_SCREEN = 0;

	private const int PASS_ADD = 1;

	public Shader GodRayShader;

	public Shader ScreenClearShader;

	public Shader SkyMaskShader;

	public override void Init()
	{
		if (!Object.op_Implicit((Object)(object)GodRayShader))
		{
			GodRayShader = Shader.Find("Hidden/PostProcessing/GodRays");
		}
		if (!Object.op_Implicit((Object)(object)ScreenClearShader))
		{
			ScreenClearShader = Shader.Find("Hidden/PostProcessing/ScreenClear");
		}
		if (!Object.op_Implicit((Object)(object)SkyMaskShader))
		{
			SkyMaskShader = Shader.Find("Hidden/PostProcessing/SkyMask");
		}
	}

	private void DrawBorder(PostProcessRenderContext context, RenderTargetIdentifier buffer1)
	{
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		PropertySheet propertySheet = context.propertySheets.Get(ScreenClearShader);
		Rect value = default(Rect);
		((Rect)(ref value))._002Ector(0f, (float)(context.height - 1), (float)context.width, 1f);
		Rect value2 = default(Rect);
		((Rect)(ref value2))._002Ector(0f, 0f, (float)context.width, 1f);
		Rect value3 = default(Rect);
		((Rect)(ref value3))._002Ector(0f, 0f, 1f, (float)context.height);
		Rect value4 = default(Rect);
		((Rect)(ref value4))._002Ector((float)(context.width - 1), 0f, 1f, (float)context.height);
		context.command.BlitFullscreenTriangle(RenderTargetIdentifier.op_Implicit((BuiltinRenderTextureType)0), buffer1, propertySheet, 0, clear: false, value);
		context.command.BlitFullscreenTriangle(RenderTargetIdentifier.op_Implicit((BuiltinRenderTextureType)0), buffer1, propertySheet, 0, clear: false, value2);
		context.command.BlitFullscreenTriangle(RenderTargetIdentifier.op_Implicit((BuiltinRenderTextureType)0), buffer1, propertySheet, 0, clear: false, value3);
		context.command.BlitFullscreenTriangle(RenderTargetIdentifier.op_Implicit((BuiltinRenderTextureType)0), buffer1, propertySheet, 0, clear: false, value4);
	}

	private int GetSkyMask(PostProcessRenderContext context, ResolutionType resolution, Vector3 lightPos, int blurIterations, float blurRadius, float maxRadius)
	{
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Invalid comparison between Unknown and I4
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Invalid comparison between Unknown and I4
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		CommandBuffer command = context.command;
		Camera camera = context.camera;
		PropertySheet propertySheet = context.propertySheets.Get(SkyMaskShader);
		command.BeginSample("GodRays");
		int num;
		int num2;
		int num3;
		switch (resolution)
		{
		case ResolutionType.High:
			num = context.screenWidth;
			num2 = context.screenHeight;
			num3 = 0;
			break;
		case ResolutionType.Normal:
			num = context.screenWidth / 2;
			num2 = context.screenHeight / 2;
			num3 = 0;
			break;
		default:
			num = context.screenWidth / 4;
			num2 = context.screenHeight / 4;
			num3 = 0;
			break;
		}
		int num4 = Shader.PropertyToID("buffer1");
		int num5 = Shader.PropertyToID("buffer2");
		command.GetTemporaryRT(num4, num, num2, num3);
		propertySheet.properties.SetVector("_BlurRadius4", new Vector4(1f, 1f, 0f, 0f) * blurRadius);
		propertySheet.properties.SetVector("_LightPosition", new Vector4(lightPos.x, lightPos.y, lightPos.z, maxRadius));
		if ((camera.depthTextureMode & 1) > 0)
		{
			command.BlitFullscreenTriangle(context.source, RenderTargetIdentifier.op_Implicit(num4), propertySheet, 1);
		}
		else
		{
			command.BlitFullscreenTriangle(context.source, RenderTargetIdentifier.op_Implicit(num4), propertySheet, 2);
		}
		if ((int)camera.stereoActiveEye == 2)
		{
			DrawBorder(context, RenderTargetIdentifier.op_Implicit(num4));
		}
		float num6 = blurRadius * 0.0013020834f;
		propertySheet.properties.SetVector("_BlurRadius4", new Vector4(num6, num6, 0f, 0f));
		propertySheet.properties.SetVector("_LightPosition", new Vector4(lightPos.x, lightPos.y, lightPos.z, maxRadius));
		for (int i = 0; i < blurIterations; i++)
		{
			command.GetTemporaryRT(num5, num, num2, num3);
			command.BlitFullscreenTriangle(RenderTargetIdentifier.op_Implicit(num4), RenderTargetIdentifier.op_Implicit(num5), propertySheet, 0);
			command.ReleaseTemporaryRT(num4);
			num6 = blurRadius * (((float)i * 2f + 1f) * 6f) / 768f;
			propertySheet.properties.SetVector("_BlurRadius4", new Vector4(num6, num6, 0f, 0f));
			command.GetTemporaryRT(num4, num, num2, num3);
			command.BlitFullscreenTriangle(RenderTargetIdentifier.op_Implicit(num5), RenderTargetIdentifier.op_Implicit(num4), propertySheet, 0);
			command.ReleaseTemporaryRT(num5);
			num6 = blurRadius * (((float)i * 2f + 2f) * 6f) / 768f;
			propertySheet.properties.SetVector("_BlurRadius4", new Vector4(num6, num6, 0f, 0f));
		}
		command.EndSample("GodRays");
		return num4;
	}

	public override void Render(PostProcessRenderContext context)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		Camera camera = context.camera;
		TOD_Sky instance = TOD_Sky.Instance;
		if (!((Object)(object)instance == (Object)null))
		{
			Vector3 val = camera.WorldToViewportPoint(instance.Components.LightTransform.position);
			CommandBuffer command = context.command;
			PropertySheet propertySheet = context.propertySheets.Get(GodRayShader);
			int skyMask = GetSkyMask(context, base.settings.Resolution.value, val, base.settings.BlurIterations.value, base.settings.BlurRadius.value, base.settings.MaxRadius.value);
			Color val2 = Color.black;
			if ((double)val.z >= 0.0)
			{
				val2 = ((!instance.IsDay) ? (base.settings.Intensity.value * instance.MoonVisibility * instance.MoonRayColor) : (base.settings.Intensity.value * instance.SunVisibility * instance.SunRayColor));
			}
			propertySheet.properties.SetColor("_LightColor", val2);
			command.SetGlobalTexture("_SkyMask", RenderTargetIdentifier.op_Implicit(skyMask));
			if (base.settings.BlendMode.value == BlendModeType.Screen)
			{
				context.command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, 0);
			}
			else
			{
				context.command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, 1);
			}
			command.ReleaseTemporaryRT(skyMask);
		}
	}
}
