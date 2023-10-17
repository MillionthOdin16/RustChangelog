using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(CommandBufferManager))]
public class DeferredExtension : MonoBehaviour
{
	public ExtendGBufferParams extendGBuffer = ExtendGBufferParams.Default;

	public SubsurfaceScatteringParams subsurfaceScattering = SubsurfaceScatteringParams.Default;

	public ScreenSpaceRefractionParams screenSpaceRefraction = ScreenSpaceRefractionParams.Default;

	public Texture2D blueNoise;

	public Texture preintegratedFGD_GGX;

	public Texture envBrdfLut;

	public float depthScale = 100f;

	public bool debug;

	public bool forceToCameraResolution;
}
