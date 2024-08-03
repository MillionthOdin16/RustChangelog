using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(CommandBufferManager))]
public class DeferredExtension : MonoBehaviour
{
	public ExtendGBufferParams extendGBuffer = ExtendGBufferParams.Default;

	public SubsurfaceScatteringParams subsurfaceScattering = SubsurfaceScatteringParams.Default;

	public Texture2D blueNoise = null;

	public Texture preintegratedFGD_GGX = null;

	public Texture envBrdfLut = null;

	public float depthScale = 100f;

	public bool debug = false;

	public bool forceToCameraResolution = false;
}
