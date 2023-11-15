using System;

namespace UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(ScreenSpaceReflectionsRenderer), "Unity/Screen-space reflections", true)]
public sealed class ScreenSpaceReflections : PostProcessEffectSettings
{
	[Tooltip("Choose a quality preset, or use \"Custom\" to create your own custom preset. Don't use a preset higher than \"Medium\" if you desire good performance on consoles.")]
	public ScreenSpaceReflectionPresetParameter preset = new ScreenSpaceReflectionPresetParameter
	{
		value = ScreenSpaceReflectionPreset.Medium
	};

	[Range(0f, 256f)]
	[Tooltip("Maximum number of steps in the raymarching pass. Higher values mean more reflections.")]
	public IntParameter maximumIterationCount = new IntParameter
	{
		value = 16
	};

	[Tooltip("Changes the size of the SSR buffer. Downsample it to maximize performances or supersample it for higher quality results with reduced performance.")]
	public ScreenSpaceReflectionResolutionParameter resolution = new ScreenSpaceReflectionResolutionParameter
	{
		value = ScreenSpaceReflectionResolution.Downsampled
	};

	[Range(1f, 64f)]
	[Tooltip("Ray thickness. Lower values are more expensive but allow the effect to detect smaller details.")]
	public FloatParameter thickness = new FloatParameter
	{
		value = 8f
	};

	[Tooltip("Maximum distance to traverse after which it will stop drawing reflections.")]
	public FloatParameter maximumMarchDistance = new FloatParameter
	{
		value = 100f
	};

	[Range(0f, 1f)]
	[Tooltip("Fades reflections close to the near planes.")]
	public FloatParameter distanceFade = new FloatParameter
	{
		value = 0.5f
	};

	[Range(0f, 1f)]
	[Tooltip("Fades reflections close to the screen edges.")]
	public FloatParameter vignette = new FloatParameter
	{
		value = 0.5f
	};

	public override bool IsEnabledAndSupported(PostProcessRenderContext context)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Invalid comparison between Unknown and I4
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Invalid comparison between Unknown and I4
		return (bool)enabled && (int)context.camera.actualRenderingPath == 3 && SystemInfo.supportsMotionVectors && SystemInfo.supportsComputeShaders && (int)SystemInfo.copyTextureSupport > 0 && Object.op_Implicit((Object)(object)context.resources.shaders.screenSpaceReflections) && context.resources.shaders.screenSpaceReflections.isSupported && Object.op_Implicit((Object)(object)context.resources.computeShaders.gaussianDownsample);
	}
}
