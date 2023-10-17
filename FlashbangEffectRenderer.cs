using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

public class FlashbangEffectRenderer : PostProcessEffectRenderer<FlashbangEffect>
{
	public static bool needsCapture;

	private Shader flashbangEffectShader;

	private RenderTexture screenRT;

	public override void Init()
	{
		base.Init();
		flashbangEffectShader = Shader.Find("Hidden/PostProcessing/FlashbangEffect");
	}

	public override void Render(PostProcessRenderContext context)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		if (Application.isPlaying)
		{
			CommandBuffer command = context.command;
			CheckCreateRenderTexture(ref screenRT, "Flashbang", context.width, context.height, context.sourceFormat);
			command.BeginSample("FlashbangEffect");
			if (needsCapture)
			{
				command.CopyTexture(context.source, RenderTargetIdentifier.op_Implicit((Texture)(object)screenRT));
				needsCapture = false;
			}
			PropertySheet propertySheet = context.propertySheets.Get(flashbangEffectShader);
			propertySheet.properties.Clear();
			propertySheet.properties.SetFloat("_BurnIntensity", base.settings.burnIntensity.value);
			propertySheet.properties.SetFloat("_WhiteoutIntensity", base.settings.whiteoutIntensity.value);
			if (Object.op_Implicit((Object)(object)screenRT))
			{
				propertySheet.properties.SetTexture("_BurnOverlay", (Texture)(object)screenRT);
			}
			context.command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, 0);
			command.EndSample("FlashbangEffect");
		}
	}

	public override void Release()
	{
		base.Release();
		SafeDestroyRenderTexture(ref screenRT);
	}

	private static void CheckCreateRenderTexture(ref RenderTexture rt, string name, int width, int height, RenderTextureFormat format)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Expected O, but got Unknown
		if ((Object)(object)rt == (Object)null || ((Texture)rt).width != width || ((Texture)rt).height != height)
		{
			SafeDestroyRenderTexture(ref rt);
			rt = new RenderTexture(width, height, 0, format)
			{
				hideFlags = (HideFlags)52
			};
			((Object)rt).name = name;
			((Texture)rt).wrapMode = (TextureWrapMode)1;
			rt.Create();
		}
	}

	private static void SafeDestroyRenderTexture(ref RenderTexture rt)
	{
		if ((Object)(object)rt != (Object)null)
		{
			rt.Release();
			Object.DestroyImmediate((Object)(object)rt);
			rt = null;
		}
	}
}
