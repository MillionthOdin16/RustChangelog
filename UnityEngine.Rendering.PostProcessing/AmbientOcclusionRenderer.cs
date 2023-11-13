using UnityEngine.Scripting;

namespace UnityEngine.Rendering.PostProcessing;

[Preserve]
internal sealed class AmbientOcclusionRenderer : PostProcessEffectRenderer<AmbientOcclusion>
{
	private IAmbientOcclusionMethod[] m_Methods;

	public override void Init()
	{
		if (m_Methods == null)
		{
			m_Methods = new IAmbientOcclusionMethod[2]
			{
				new ScalableAO(base.settings),
				new MultiScaleVO(base.settings)
			};
		}
	}

	public bool IsAmbientOnly(PostProcessRenderContext context)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Invalid comparison between Unknown and I4
		Camera camera = context.camera;
		return base.settings.ambientOnly.value && (int)camera.actualRenderingPath == 3 && camera.allowHDR;
	}

	public IAmbientOcclusionMethod Get()
	{
		return m_Methods[(int)base.settings.mode.value];
	}

	public override DepthTextureMode GetCameraFlags()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		return Get().GetCameraFlags();
	}

	public override void Release()
	{
		IAmbientOcclusionMethod[] methods = m_Methods;
		foreach (IAmbientOcclusionMethod ambientOcclusionMethod in methods)
		{
			ambientOcclusionMethod.Release();
		}
	}

	public ScalableAO GetScalableAO()
	{
		return (ScalableAO)m_Methods[0];
	}

	public MultiScaleVO GetMultiScaleVO()
	{
		return (MultiScaleVO)m_Methods[1];
	}

	public override void Render(PostProcessRenderContext context)
	{
	}
}
