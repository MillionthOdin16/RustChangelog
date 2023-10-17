using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Assertions;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering.PostProcessing;

[ExecuteAlways]
[DisallowMultipleComponent]
[ImageEffectAllowedInSceneView]
[AddComponentMenu("Rendering/Post-process Layer", 1000)]
[RequireComponent(typeof(Camera))]
public class PostProcessLayer : MonoBehaviour
{
	private enum ScalingMode
	{
		NATIVE,
		BILINEAR,
		DLSS
	}

	public enum Antialiasing
	{
		None,
		FastApproximateAntialiasing,
		SubpixelMorphologicalAntialiasing,
		TemporalAntialiasing
	}

	[Serializable]
	public sealed class SerializedBundleRef
	{
		public string assemblyQualifiedName;

		public PostProcessBundle bundle;
	}

	public Transform volumeTrigger;

	public LayerMask volumeLayer;

	public bool stopNaNPropagation = true;

	public bool finalBlitToCameraTarget = false;

	public Antialiasing antialiasingMode = Antialiasing.None;

	public TemporalAntialiasing temporalAntialiasing;

	public SubpixelMorphologicalAntialiasing subpixelMorphologicalAntialiasing;

	public FastApproximateAntialiasing fastApproximateAntialiasing;

	public Fog fog;

	private Dithering dithering;

	public PostProcessDebugLayer debugLayer;

	public RenderTextureFormat intermediateFormat = (RenderTextureFormat)9;

	private RenderTextureFormat prevIntermediateFormat = (RenderTextureFormat)9;

	private bool supportsIntermediateFormat = true;

	[SerializeField]
	private PostProcessResources m_Resources;

	[Preserve]
	[SerializeField]
	private bool m_ShowToolkit;

	[Preserve]
	[SerializeField]
	private bool m_ShowCustomSorter;

	public bool breakBeforeColorGrading = false;

	[SerializeField]
	private List<SerializedBundleRef> m_BeforeTransparentBundles;

	[SerializeField]
	private List<SerializedBundleRef> m_BeforeStackBundles;

	[SerializeField]
	private List<SerializedBundleRef> m_AfterStackBundles;

	private Dictionary<Type, PostProcessBundle> m_Bundles;

	private PropertySheetFactory m_PropertySheetFactory;

	private CommandBuffer m_LegacyCmdBufferBeforeReflections;

	private CommandBuffer m_LegacyCmdBufferBeforeLighting;

	private CommandBuffer m_LegacyCmdBufferOpaque;

	private CommandBuffer m_LegacyCmdBuffer;

	private Camera m_Camera;

	private PostProcessRenderContext m_CurrentContext;

	private LogHistogram m_LogHistogram;

	private bool m_SettingsUpdateNeeded = true;

	private bool m_IsRenderingInSceneView = false;

	private TargetPool m_TargetPool;

	private bool m_NaNKilled = false;

	private readonly List<PostProcessEffectRenderer> m_ActiveEffects = new List<PostProcessEffectRenderer>();

	private readonly List<RenderTargetIdentifier> m_Targets = new List<RenderTargetIdentifier>();

	public Dictionary<PostProcessEvent, List<SerializedBundleRef>> sortedBundles { get; private set; }

	public bool haveBundlesBeenInited { get; private set; }

	private void OnEnable()
	{
		Init(null);
		if (!haveBundlesBeenInited)
		{
			InitBundles();
		}
		m_LogHistogram = new LogHistogram();
		m_PropertySheetFactory = new PropertySheetFactory();
		m_TargetPool = new TargetPool();
		debugLayer.OnEnable();
		if (!RuntimeUtilities.scriptableRenderPipelineActive)
		{
			InitLegacy();
		}
	}

	private void InitLegacy()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Expected O, but got Unknown
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Expected O, but got Unknown
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Expected O, but got Unknown
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Expected O, but got Unknown
		m_LegacyCmdBufferBeforeReflections = new CommandBuffer
		{
			name = "Deferred Ambient Occlusion"
		};
		m_LegacyCmdBufferBeforeLighting = new CommandBuffer
		{
			name = "Deferred Ambient Occlusion"
		};
		m_LegacyCmdBufferOpaque = new CommandBuffer
		{
			name = "Opaque Only Post-processing"
		};
		m_LegacyCmdBuffer = new CommandBuffer
		{
			name = "Post-processing"
		};
		m_Camera = ((Component)this).GetComponent<Camera>();
		m_Camera.AddCommandBuffer((CameraEvent)21, m_LegacyCmdBufferBeforeReflections);
		m_Camera.AddCommandBuffer((CameraEvent)6, m_LegacyCmdBufferBeforeLighting);
		m_Camera.AddCommandBuffer((CameraEvent)12, m_LegacyCmdBufferOpaque);
		m_Camera.AddCommandBuffer((CameraEvent)18, m_LegacyCmdBuffer);
		m_CurrentContext = new PostProcessRenderContext();
	}

	[ImageEffectUsesCommandBuffer]
	private void OnRenderImage(RenderTexture src, RenderTexture dst)
	{
		if (finalBlitToCameraTarget)
		{
			RenderTexture.active = dst;
		}
		else
		{
			Graphics.Blit((Texture)(object)src, dst);
		}
	}

	public void Init(PostProcessResources resources)
	{
		if ((Object)(object)resources != (Object)null)
		{
			m_Resources = resources;
		}
		RuntimeUtilities.CreateIfNull(ref temporalAntialiasing);
		RuntimeUtilities.CreateIfNull(ref subpixelMorphologicalAntialiasing);
		RuntimeUtilities.CreateIfNull(ref fastApproximateAntialiasing);
		RuntimeUtilities.CreateIfNull(ref dithering);
		RuntimeUtilities.CreateIfNull(ref fog);
		RuntimeUtilities.CreateIfNull(ref debugLayer);
	}

	public void InitBundles()
	{
		if (haveBundlesBeenInited)
		{
			return;
		}
		RuntimeUtilities.CreateIfNull(ref m_BeforeTransparentBundles);
		RuntimeUtilities.CreateIfNull(ref m_BeforeStackBundles);
		RuntimeUtilities.CreateIfNull(ref m_AfterStackBundles);
		m_Bundles = new Dictionary<Type, PostProcessBundle>();
		foreach (Type key in PostProcessManager.instance.settingsTypes.Keys)
		{
			PostProcessEffectSettings settings = (PostProcessEffectSettings)(object)ScriptableObject.CreateInstance(key);
			PostProcessBundle value = new PostProcessBundle(settings);
			m_Bundles.Add(key, value);
		}
		UpdateBundleSortList(m_BeforeTransparentBundles, PostProcessEvent.BeforeTransparent);
		UpdateBundleSortList(m_BeforeStackBundles, PostProcessEvent.BeforeStack);
		UpdateBundleSortList(m_AfterStackBundles, PostProcessEvent.AfterStack);
		sortedBundles = new Dictionary<PostProcessEvent, List<SerializedBundleRef>>(default(PostProcessEventComparer))
		{
			{
				PostProcessEvent.BeforeTransparent,
				m_BeforeTransparentBundles
			},
			{
				PostProcessEvent.BeforeStack,
				m_BeforeStackBundles
			},
			{
				PostProcessEvent.AfterStack,
				m_AfterStackBundles
			}
		};
		haveBundlesBeenInited = true;
	}

	private void UpdateBundleSortList(List<SerializedBundleRef> sortedList, PostProcessEvent evt)
	{
		List<PostProcessBundle> effects = (from kvp in m_Bundles
			where kvp.Value.attribute.eventType == evt && !kvp.Value.attribute.builtinEffect
			select kvp.Value).ToList();
		sortedList.RemoveAll(delegate(SerializedBundleRef x)
		{
			string searchStr = x.assemblyQualifiedName;
			return !effects.Exists((PostProcessBundle b) => ((object)b.settings).GetType().AssemblyQualifiedName == searchStr);
		});
		foreach (PostProcessBundle item2 in effects)
		{
			string typeName2 = ((object)item2.settings).GetType().AssemblyQualifiedName;
			if (!sortedList.Exists((SerializedBundleRef b) => b.assemblyQualifiedName == typeName2))
			{
				SerializedBundleRef item = new SerializedBundleRef
				{
					assemblyQualifiedName = typeName2
				};
				sortedList.Add(item);
			}
		}
		foreach (SerializedBundleRef sorted in sortedList)
		{
			string typeName = sorted.assemblyQualifiedName;
			PostProcessBundle bundle = effects.Find((PostProcessBundle b) => ((object)b.settings).GetType().AssemblyQualifiedName == typeName);
			sorted.bundle = bundle;
		}
	}

	private void OnDisable()
	{
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)m_Camera != (Object)null)
		{
			if (m_LegacyCmdBufferBeforeReflections != null)
			{
				m_Camera.RemoveCommandBuffer((CameraEvent)21, m_LegacyCmdBufferBeforeReflections);
			}
			if (m_LegacyCmdBufferBeforeLighting != null)
			{
				m_Camera.RemoveCommandBuffer((CameraEvent)6, m_LegacyCmdBufferBeforeLighting);
			}
			if (m_LegacyCmdBufferOpaque != null)
			{
				m_Camera.RemoveCommandBuffer((CameraEvent)12, m_LegacyCmdBufferOpaque);
			}
			if (m_LegacyCmdBuffer != null)
			{
				m_Camera.RemoveCommandBuffer((CameraEvent)18, m_LegacyCmdBuffer);
			}
		}
		temporalAntialiasing.Release();
		m_LogHistogram.Release();
		foreach (PostProcessBundle value in m_Bundles.Values)
		{
			value.Release();
		}
		m_Bundles.Clear();
		m_PropertySheetFactory.Release();
		if (debugLayer != null)
		{
			debugLayer.OnDisable();
		}
		TextureLerper.instance.Clear();
		m_Camera.ResetProjectionMatrix();
		m_Camera.nonJitteredProjectionMatrix = m_Camera.projectionMatrix;
		Shader.SetGlobalVector("_FrustumJitter", Vector4.op_Implicit(Vector2.zero));
		haveBundlesBeenInited = false;
	}

	private void Reset()
	{
		volumeTrigger = ((Component)this).transform;
	}

	private void OnPreCull()
	{
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		if (!RuntimeUtilities.scriptableRenderPipelineActive)
		{
			if ((Object)(object)m_Camera == (Object)null || m_CurrentContext == null)
			{
				InitLegacy();
			}
			if (!m_Camera.usePhysicalProperties)
			{
				m_Camera.ResetProjectionMatrix();
			}
			m_Camera.nonJitteredProjectionMatrix = m_Camera.projectionMatrix;
			if (m_Camera.stereoEnabled)
			{
				m_Camera.ResetStereoProjectionMatrices();
			}
			else
			{
				Shader.SetGlobalFloat(ShaderIDs.RenderViewportScaleFactor, 1f);
			}
			BuildCommandBuffers();
			Shader.SetGlobalVector("_FrustumJitter", Vector4.op_Implicit(temporalAntialiasing.jitter));
		}
	}

	private void OnPreRender()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Invalid comparison between Unknown and I4
		if (!RuntimeUtilities.scriptableRenderPipelineActive && (int)m_Camera.stereoActiveEye == 1)
		{
			BuildCommandBuffers();
		}
	}

	private RenderTextureFormat GetIntermediateFormat()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		if (intermediateFormat != prevIntermediateFormat)
		{
			supportsIntermediateFormat = SystemInfo.SupportsRenderTextureFormat(intermediateFormat);
			prevIntermediateFormat = intermediateFormat;
		}
		return (RenderTextureFormat)((!supportsIntermediateFormat) ? 9 : ((int)intermediateFormat));
	}

	private static bool RequiresInitialBlit(Camera camera, PostProcessRenderContext context)
	{
		if (camera.allowMSAA)
		{
			return true;
		}
		if (RuntimeUtilities.scriptableRenderPipelineActive)
		{
			return true;
		}
		return false;
	}

	private void UpdateSrcDstForOpaqueOnly(ref int src, ref int dst, PostProcessRenderContext context, RenderTargetIdentifier cameraTarget, int opaqueOnlyEffectsRemaining)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		if (src > -1)
		{
			context.command.ReleaseTemporaryRT(src);
		}
		context.source = context.destination;
		src = dst;
		if (opaqueOnlyEffectsRemaining == 1)
		{
			context.destination = cameraTarget;
			return;
		}
		dst = m_TargetPool.Get();
		context.destination = RenderTargetIdentifier.op_Implicit(dst);
		context.GetScreenSpaceTemporaryRT(context.command, dst, 0, context.sourceFormat, (RenderTextureReadWrite)0, (FilterMode)1);
	}

	private void BuildCommandBuffers()
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		PostProcessRenderContext currentContext = m_CurrentContext;
		RenderTextureFormat val = GetIntermediateFormat();
		RenderTextureFormat val2 = (RenderTextureFormat)((!m_Camera.allowHDR) ? 7 : ((int)val));
		if (!RuntimeUtilities.isFloatingPointFormat(val2))
		{
			m_NaNKilled = true;
		}
		currentContext.Reset();
		currentContext.camera = m_Camera;
		currentContext.sourceFormat = val2;
		m_LegacyCmdBufferBeforeReflections.Clear();
		m_LegacyCmdBufferBeforeLighting.Clear();
		m_LegacyCmdBufferOpaque.Clear();
		m_LegacyCmdBuffer.Clear();
		SetupContext(currentContext);
		currentContext.command = m_LegacyCmdBufferOpaque;
		TextureLerper.instance.BeginFrame(currentContext);
		UpdateVolumeSystem(currentContext.camera, currentContext.command);
		PostProcessBundle bundle = GetBundle<AmbientOcclusion>();
		AmbientOcclusion ambientOcclusion = bundle.CastSettings<AmbientOcclusion>();
		AmbientOcclusionRenderer ambientOcclusionRenderer = bundle.CastRenderer<AmbientOcclusionRenderer>();
		bool flag = ambientOcclusion.IsEnabledAndSupported(currentContext);
		bool flag2 = ambientOcclusionRenderer.IsAmbientOnly(currentContext);
		bool flag3 = flag && flag2;
		bool flag4 = flag && !flag2;
		PostProcessBundle bundle2 = GetBundle<ScreenSpaceReflections>();
		PostProcessEffectSettings settings = bundle2.settings;
		PostProcessEffectRenderer renderer = bundle2.renderer;
		bool flag5 = settings.IsEnabledAndSupported(currentContext);
		if (flag3)
		{
			IAmbientOcclusionMethod ambientOcclusionMethod = ambientOcclusionRenderer.Get();
			currentContext.command = m_LegacyCmdBufferBeforeReflections;
			ambientOcclusionMethod.RenderAmbientOnly(currentContext);
			currentContext.command = m_LegacyCmdBufferBeforeLighting;
			ambientOcclusionMethod.CompositeAmbientOnly(currentContext);
		}
		else if (flag4)
		{
			currentContext.command = m_LegacyCmdBufferOpaque;
			ambientOcclusionRenderer.Get().RenderAfterOpaque(currentContext);
		}
		bool flag6 = fog.IsEnabledAndSupported(currentContext);
		bool flag7 = HasOpaqueOnlyEffects(currentContext);
		int num = 0;
		num += (flag5 ? 1 : 0);
		num += (flag6 ? 1 : 0);
		num += (flag7 ? 1 : 0);
		RenderTargetIdentifier val3 = default(RenderTargetIdentifier);
		((RenderTargetIdentifier)(ref val3))._002Ector((BuiltinRenderTextureType)2);
		if (num > 0)
		{
			CommandBuffer val4 = (currentContext.command = m_LegacyCmdBufferOpaque);
			currentContext.source = val3;
			currentContext.destination = val3;
			int src = -1;
			int dst = -1;
			UpdateSrcDstForOpaqueOnly(ref src, ref dst, currentContext, val3, num + 1);
			if (RequiresInitialBlit(m_Camera, currentContext) || num == 1)
			{
				val4.BuiltinBlit(currentContext.source, currentContext.destination, RuntimeUtilities.copyStdMaterial, stopNaNPropagation ? 1 : 0);
				UpdateSrcDstForOpaqueOnly(ref src, ref dst, currentContext, val3, num);
			}
			if (flag5)
			{
				renderer.Render(currentContext);
				num--;
				UpdateSrcDstForOpaqueOnly(ref src, ref dst, currentContext, val3, num);
			}
			if (flag6)
			{
				fog.Render(currentContext);
				num--;
				UpdateSrcDstForOpaqueOnly(ref src, ref dst, currentContext, val3, num);
			}
			if (flag7)
			{
				RenderOpaqueOnly(currentContext);
			}
			val4.ReleaseTemporaryRT(src);
		}
		BuildPostEffectsOld(val2, currentContext, val3);
	}

	private void BuildPostEffectsOld(RenderTextureFormat sourceFormat, PostProcessRenderContext context, RenderTargetIdentifier cameraTarget)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		int num = -1;
		bool flag = !m_NaNKilled && stopNaNPropagation && RuntimeUtilities.isFloatingPointFormat(sourceFormat);
		if (RequiresInitialBlit(m_Camera, context) || flag)
		{
			num = m_TargetPool.Get();
			context.GetScreenSpaceTemporaryRT(m_LegacyCmdBuffer, num, 0, sourceFormat, (RenderTextureReadWrite)2, (FilterMode)1);
			m_LegacyCmdBuffer.BuiltinBlit(cameraTarget, RenderTargetIdentifier.op_Implicit(num), RuntimeUtilities.copyStdMaterial, stopNaNPropagation ? 1 : 0);
			if (!m_NaNKilled)
			{
				m_NaNKilled = stopNaNPropagation;
			}
			context.source = RenderTargetIdentifier.op_Implicit(num);
		}
		else
		{
			context.source = cameraTarget;
		}
		context.destination = cameraTarget;
		if (finalBlitToCameraTarget && !RuntimeUtilities.scriptableRenderPipelineActive)
		{
			if (Object.op_Implicit((Object)(object)m_Camera.targetTexture))
			{
				context.destination = RenderTargetIdentifier.op_Implicit(m_Camera.targetTexture.colorBuffer);
			}
			else
			{
				context.flip = true;
				context.destination = RenderTargetIdentifier.op_Implicit(Display.main.colorBuffer);
			}
		}
		context.command = m_LegacyCmdBuffer;
		Render(context);
		if (num > -1)
		{
			m_LegacyCmdBuffer.ReleaseTemporaryRT(num);
		}
	}

	private void OnPostRender()
	{
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Invalid comparison between Unknown and I4
		if (!RuntimeUtilities.scriptableRenderPipelineActive && m_CurrentContext.IsTemporalAntialiasingActive())
		{
			if (m_CurrentContext.physicalCamera)
			{
				m_Camera.usePhysicalProperties = true;
			}
			else
			{
				m_Camera.ResetProjectionMatrix();
			}
			if (m_CurrentContext.stereoActive && (RuntimeUtilities.isSinglePassStereoEnabled || (int)m_Camera.stereoActiveEye == 1))
			{
				m_Camera.ResetStereoProjectionMatrices();
			}
		}
	}

	public PostProcessBundle GetBundle<T>() where T : PostProcessEffectSettings
	{
		return GetBundle(typeof(T));
	}

	public PostProcessBundle GetBundle(Type settingsType)
	{
		Assert.IsTrue(m_Bundles.ContainsKey(settingsType), "Invalid type");
		return m_Bundles[settingsType];
	}

	public T GetSettings<T>() where T : PostProcessEffectSettings
	{
		return GetBundle<T>().CastSettings<T>();
	}

	public void BakeMSVOMap(CommandBuffer cmd, Camera camera, RenderTargetIdentifier destination, RenderTargetIdentifier? depthMap, bool invert, bool isMSAA = false)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		PostProcessBundle bundle = GetBundle<AmbientOcclusion>();
		MultiScaleVO multiScaleVO = bundle.CastRenderer<AmbientOcclusionRenderer>().GetMultiScaleVO();
		multiScaleVO.SetResources(m_Resources);
		multiScaleVO.GenerateAOMap(cmd, camera, destination, depthMap, invert, isMSAA);
	}

	internal void OverrideSettings(List<PostProcessEffectSettings> baseSettings, float interpFactor)
	{
		foreach (PostProcessEffectSettings baseSetting in baseSettings)
		{
			if (!baseSetting.active)
			{
				continue;
			}
			PostProcessEffectSettings settings = GetBundle(((object)baseSetting).GetType()).settings;
			int count = baseSetting.parameters.Count;
			for (int i = 0; i < count; i++)
			{
				ParameterOverride parameterOverride = baseSetting.parameters[i];
				if (parameterOverride.overrideState)
				{
					ParameterOverride parameterOverride2 = settings.parameters[i];
					parameterOverride2.Interp(parameterOverride2, parameterOverride, interpFactor);
				}
			}
		}
	}

	private void SetLegacyCameraFlags(PostProcessRenderContext context)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		DepthTextureMode val = context.camera.depthTextureMode;
		foreach (KeyValuePair<Type, PostProcessBundle> bundle in m_Bundles)
		{
			if (bundle.Value.settings.IsEnabledAndSupported(context))
			{
				val |= bundle.Value.renderer.GetCameraFlags();
			}
		}
		if (context.IsTemporalAntialiasingActive())
		{
			val |= temporalAntialiasing.GetCameraFlags();
		}
		if (fog.IsEnabledAndSupported(context))
		{
			val |= fog.GetCameraFlags();
		}
		if (debugLayer.debugOverlay != 0)
		{
			val |= debugLayer.GetCameraFlags();
		}
		context.camera.depthTextureMode = val;
	}

	public void ResetHistory()
	{
		foreach (KeyValuePair<Type, PostProcessBundle> bundle in m_Bundles)
		{
			bundle.Value.ResetHistory();
		}
		temporalAntialiasing.ResetHistory();
	}

	public bool HasOpaqueOnlyEffects(PostProcessRenderContext context)
	{
		return HasActiveEffects(PostProcessEvent.BeforeTransparent, context);
	}

	public bool HasActiveEffects(PostProcessEvent evt, PostProcessRenderContext context)
	{
		List<SerializedBundleRef> list = sortedBundles[evt];
		foreach (SerializedBundleRef item in list)
		{
			bool flag = item.bundle.settings.IsEnabledAndSupported(context);
			if (context.isSceneView)
			{
				if (item.bundle.attribute.allowInSceneView && flag)
				{
					return true;
				}
			}
			else if (flag)
			{
				return true;
			}
		}
		return false;
	}

	private void SetupContext(PostProcessRenderContext context)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Invalid comparison between Unknown and I4
		RuntimeUtilities.s_Resources = m_Resources;
		m_IsRenderingInSceneView = (int)context.camera.cameraType == 2;
		context.isSceneView = m_IsRenderingInSceneView;
		context.resources = m_Resources;
		context.propertySheets = m_PropertySheetFactory;
		context.debugLayer = debugLayer;
		context.antialiasing = antialiasingMode;
		context.temporalAntialiasing = temporalAntialiasing;
		context.logHistogram = m_LogHistogram;
		context.physicalCamera = context.camera.usePhysicalProperties;
		SetLegacyCameraFlags(context);
		debugLayer.SetFrameSize(context.width, context.height);
		m_CurrentContext = context;
	}

	public void UpdateVolumeSystem(Camera cam, CommandBuffer cmd)
	{
		if (m_SettingsUpdateNeeded)
		{
			cmd.BeginSample("VolumeBlending");
			PostProcessManager.instance.UpdateSettings(this, cam);
			cmd.EndSample("VolumeBlending");
			m_TargetPool.Reset();
			if (RuntimeUtilities.scriptableRenderPipelineActive)
			{
				Shader.SetGlobalFloat(ShaderIDs.RenderViewportScaleFactor, 1f);
			}
		}
		m_SettingsUpdateNeeded = false;
	}

	public void RenderOpaqueOnly(PostProcessRenderContext context)
	{
		if (RuntimeUtilities.scriptableRenderPipelineActive)
		{
			SetupContext(context);
		}
		TextureLerper.instance.BeginFrame(context);
		UpdateVolumeSystem(context.camera, context.command);
		RenderList(sortedBundles[PostProcessEvent.BeforeTransparent], context, "OpaqueOnly");
	}

	public void Render(PostProcessRenderContext context)
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Invalid comparison between Unknown and I4
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
		if (RuntimeUtilities.scriptableRenderPipelineActive)
		{
			SetupContext(context);
		}
		TextureLerper.instance.BeginFrame(context);
		CommandBuffer command = context.command;
		UpdateVolumeSystem(context.camera, context.command);
		int num = -1;
		RenderTargetIdentifier source = context.source;
		if (context.stereoActive && context.numberOfEyes > 1 && context.stereoRenderingMode == PostProcessRenderContext.StereoRenderingMode.SinglePass)
		{
			command.SetSinglePassStereo((SinglePassStereoMode)0);
			command.DisableShaderKeyword("UNITY_SINGLE_PASS_STEREO");
		}
		for (int i = 0; i < context.numberOfEyes; i++)
		{
			bool flag = false;
			if (stopNaNPropagation && !m_NaNKilled)
			{
				num = m_TargetPool.Get();
				context.GetScreenSpaceTemporaryRT(command, num, 0, context.sourceFormat, (RenderTextureReadWrite)0, (FilterMode)1);
				if (context.stereoActive && context.numberOfEyes > 1)
				{
					if (context.stereoRenderingMode == PostProcessRenderContext.StereoRenderingMode.SinglePassInstanced)
					{
						command.BlitFullscreenTriangleFromTexArray(context.source, RenderTargetIdentifier.op_Implicit(num), RuntimeUtilities.copyFromTexArraySheet, 1, clear: false, i);
						flag = true;
					}
					else if (context.stereoRenderingMode == PostProcessRenderContext.StereoRenderingMode.SinglePass)
					{
						command.BlitFullscreenTriangleFromDoubleWide(context.source, RenderTargetIdentifier.op_Implicit(num), RuntimeUtilities.copyStdFromDoubleWideMaterial, 1, i);
						flag = true;
					}
				}
				else
				{
					command.BlitFullscreenTriangle(context.source, RenderTargetIdentifier.op_Implicit(num), RuntimeUtilities.copySheet, 1);
				}
				context.source = RenderTargetIdentifier.op_Implicit(num);
				m_NaNKilled = true;
			}
			if (!flag && context.numberOfEyes > 1)
			{
				num = m_TargetPool.Get();
				context.GetScreenSpaceTemporaryRT(command, num, 0, context.sourceFormat, (RenderTextureReadWrite)0, (FilterMode)1);
				if (context.stereoActive)
				{
					if (context.stereoRenderingMode == PostProcessRenderContext.StereoRenderingMode.SinglePassInstanced)
					{
						command.BlitFullscreenTriangleFromTexArray(context.source, RenderTargetIdentifier.op_Implicit(num), RuntimeUtilities.copyFromTexArraySheet, 1, clear: false, i);
						flag = true;
					}
					else if (context.stereoRenderingMode == PostProcessRenderContext.StereoRenderingMode.SinglePass)
					{
						command.BlitFullscreenTriangleFromDoubleWide(context.source, RenderTargetIdentifier.op_Implicit(num), RuntimeUtilities.copyStdFromDoubleWideMaterial, stopNaNPropagation ? 1 : 0, i);
						flag = true;
					}
				}
				context.source = RenderTargetIdentifier.op_Implicit(num);
			}
			if (context.IsTemporalAntialiasingActive())
			{
				temporalAntialiasing.sampleCount = 8;
				if (!RuntimeUtilities.scriptableRenderPipelineActive)
				{
					if (context.stereoActive)
					{
						if ((int)context.camera.stereoActiveEye != 1)
						{
							temporalAntialiasing.ConfigureStereoJitteredProjectionMatrices(context);
						}
					}
					else
					{
						temporalAntialiasing.ConfigureJitteredProjectionMatrix(context);
					}
				}
				int num2 = m_TargetPool.Get();
				RenderTargetIdentifier destination = context.destination;
				context.GetScreenSpaceTemporaryRT(command, num2, 0, context.sourceFormat, (RenderTextureReadWrite)0, (FilterMode)1);
				context.destination = RenderTargetIdentifier.op_Implicit(num2);
				temporalAntialiasing.Render(context);
				context.source = RenderTargetIdentifier.op_Implicit(num2);
				context.destination = destination;
				if (num > -1)
				{
					command.ReleaseTemporaryRT(num);
				}
				num = num2;
			}
			bool flag2 = HasActiveEffects(PostProcessEvent.BeforeStack, context);
			bool flag3 = HasActiveEffects(PostProcessEvent.AfterStack, context) && !breakBeforeColorGrading;
			bool flag4 = (flag3 || antialiasingMode == Antialiasing.FastApproximateAntialiasing || (antialiasingMode == Antialiasing.SubpixelMorphologicalAntialiasing && subpixelMorphologicalAntialiasing.IsSupported())) && !breakBeforeColorGrading;
			if (flag2)
			{
				num = RenderInjectionPoint(PostProcessEvent.BeforeStack, context, "BeforeStack", num);
			}
			num = RenderBuiltins(context, !flag4, num, i);
			if (flag3)
			{
				num = RenderInjectionPoint(PostProcessEvent.AfterStack, context, "AfterStack", num);
			}
			if (flag4)
			{
				RenderFinalPass(context, num, i);
			}
			if (context.stereoActive)
			{
				context.source = source;
			}
		}
		if (context.stereoActive && context.numberOfEyes > 1 && context.stereoRenderingMode == PostProcessRenderContext.StereoRenderingMode.SinglePass)
		{
			command.SetSinglePassStereo((SinglePassStereoMode)1);
			command.EnableShaderKeyword("UNITY_SINGLE_PASS_STEREO");
		}
		debugLayer.RenderSpecialOverlays(context);
		debugLayer.RenderMonitors(context);
		TextureLerper.instance.EndFrame();
		debugLayer.EndFrame();
		m_SettingsUpdateNeeded = true;
		m_NaNKilled = false;
	}

	private int RenderInjectionPoint(PostProcessEvent evt, PostProcessRenderContext context, string marker, int releaseTargetAfterUse = -1)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		int num = m_TargetPool.Get();
		RenderTargetIdentifier destination = context.destination;
		CommandBuffer command = context.command;
		context.GetScreenSpaceTemporaryRT(command, num, 0, context.sourceFormat, (RenderTextureReadWrite)0, (FilterMode)1);
		context.destination = RenderTargetIdentifier.op_Implicit(num);
		RenderList(sortedBundles[evt], context, marker);
		context.source = RenderTargetIdentifier.op_Implicit(num);
		context.destination = destination;
		if (releaseTargetAfterUse > -1)
		{
			command.ReleaseTemporaryRT(releaseTargetAfterUse);
		}
		return num;
	}

	private void RenderList(List<SerializedBundleRef> list, PostProcessRenderContext context, string marker)
	{
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		CommandBuffer command = context.command;
		command.BeginSample(marker);
		m_ActiveEffects.Clear();
		for (int i = 0; i < list.Count; i++)
		{
			PostProcessBundle bundle = list[i].bundle;
			if (bundle.settings.IsEnabledAndSupported(context) && (!context.isSceneView || (context.isSceneView && bundle.attribute.allowInSceneView)))
			{
				m_ActiveEffects.Add(bundle.renderer);
			}
		}
		int count = m_ActiveEffects.Count;
		if (count == 1)
		{
			m_ActiveEffects[0].Render(context);
		}
		else
		{
			m_Targets.Clear();
			m_Targets.Add(context.source);
			int num = m_TargetPool.Get();
			int num2 = m_TargetPool.Get();
			for (int j = 0; j < count - 1; j++)
			{
				m_Targets.Add(RenderTargetIdentifier.op_Implicit((j % 2 == 0) ? num : num2));
			}
			m_Targets.Add(context.destination);
			context.GetScreenSpaceTemporaryRT(command, num, 0, context.sourceFormat, (RenderTextureReadWrite)0, (FilterMode)1);
			if (count > 2)
			{
				context.GetScreenSpaceTemporaryRT(command, num2, 0, context.sourceFormat, (RenderTextureReadWrite)0, (FilterMode)1);
			}
			for (int k = 0; k < count; k++)
			{
				context.source = m_Targets[k];
				context.destination = m_Targets[k + 1];
				m_ActiveEffects[k].Render(context);
			}
			command.ReleaseTemporaryRT(num);
			if (count > 2)
			{
				command.ReleaseTemporaryRT(num2);
			}
		}
		command.EndSample(marker);
	}

	private void ApplyFlip(PostProcessRenderContext context, MaterialPropertyBlock properties)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		if (context.flip && !context.isSceneView)
		{
			properties.SetVector(ShaderIDs.UVTransform, new Vector4(1f, 1f, 0f, 0f));
		}
		else
		{
			ApplyDefaultFlip(properties);
		}
	}

	private void ApplyDefaultFlip(MaterialPropertyBlock properties)
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		properties.SetVector(ShaderIDs.UVTransform, SystemInfo.graphicsUVStartsAtTop ? new Vector4(1f, -1f, 0f, 1f) : new Vector4(1f, 1f, 0f, 0f));
	}

	private int RenderBuiltins(PostProcessRenderContext context, bool isFinalPass, int releaseTargetAfterUse = -1, int eye = -1)
	{
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		PropertySheet propertySheet = context.propertySheets.Get(context.resources.shaders.uber);
		propertySheet.ClearKeywords();
		propertySheet.properties.Clear();
		context.uberSheet = propertySheet;
		context.bloomBufferNameID = -1;
		context.autoExposureTexture = (Texture)(object)RuntimeUtilities.whiteTexture;
		if (isFinalPass && context.stereoActive && context.stereoRenderingMode == PostProcessRenderContext.StereoRenderingMode.SinglePassInstanced)
		{
			propertySheet.EnableKeyword("STEREO_INSTANCING_ENABLED");
		}
		CommandBuffer command = context.command;
		command.BeginSample("BuiltinStack");
		int num = -1;
		RenderTargetIdentifier destination = context.destination;
		if (!isFinalPass)
		{
			num = m_TargetPool.Get();
			context.GetScreenSpaceTemporaryRT(command, num, 0, context.sourceFormat, (RenderTextureReadWrite)0, (FilterMode)1);
			context.destination = RenderTargetIdentifier.op_Implicit(num);
			if (antialiasingMode == Antialiasing.FastApproximateAntialiasing && !fastApproximateAntialiasing.keepAlpha)
			{
				propertySheet.properties.SetFloat(ShaderIDs.LumaInAlpha, 1f);
			}
		}
		int num2 = RenderEffect<DepthOfFieldEffect>(context, useTempTarget: true);
		int num3 = RenderEffect<MotionBlur>(context, useTempTarget: true);
		if (ShouldGenerateLogHistogram(context))
		{
			m_LogHistogram.Generate(context);
		}
		RenderEffect<AutoExposure>(context);
		propertySheet.properties.SetTexture(ShaderIDs.AutoExposureTex, context.autoExposureTexture);
		RenderEffect<LensDistortion>(context);
		RenderEffect<ChromaticAberration>(context);
		RenderEffect<Bloom>(context);
		RenderEffect<Vignette>(context);
		RenderEffect<Grain>(context);
		if (!breakBeforeColorGrading)
		{
			RenderEffect<ColorGrading>(context);
		}
		if (isFinalPass)
		{
			propertySheet.EnableKeyword("FINALPASS");
			dithering.Render(context);
			ApplyFlip(context, propertySheet.properties);
		}
		else
		{
			ApplyDefaultFlip(propertySheet.properties);
		}
		if (context.stereoActive && context.stereoRenderingMode == PostProcessRenderContext.StereoRenderingMode.SinglePassInstanced)
		{
			propertySheet.properties.SetFloat(ShaderIDs.DepthSlice, (float)eye);
			command.BlitFullscreenTriangleToTexArray(context.source, context.destination, propertySheet, 0, clear: false, eye);
		}
		else if (isFinalPass && context.stereoActive && context.numberOfEyes > 1 && context.stereoRenderingMode == PostProcessRenderContext.StereoRenderingMode.SinglePass)
		{
			command.BlitFullscreenTriangleToDoubleWide(context.source, context.destination, propertySheet, 0, eye);
		}
		else
		{
			command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, 0);
		}
		context.source = context.destination;
		context.destination = destination;
		if (releaseTargetAfterUse > -1)
		{
			command.ReleaseTemporaryRT(releaseTargetAfterUse);
		}
		if (num3 > -1)
		{
			command.ReleaseTemporaryRT(num3);
		}
		if (num2 > -1)
		{
			command.ReleaseTemporaryRT(num2);
		}
		if (context.bloomBufferNameID > -1)
		{
			command.ReleaseTemporaryRT(context.bloomBufferNameID);
		}
		command.EndSample("BuiltinStack");
		return num;
	}

	private void RenderFinalPass(PostProcessRenderContext context, int releaseTargetAfterUse = -1, int eye = -1)
	{
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		CommandBuffer command = context.command;
		command.BeginSample("FinalPass");
		if (breakBeforeColorGrading)
		{
			PropertySheet propertySheet = context.propertySheets.Get(context.resources.shaders.discardAlpha);
			if (context.stereoActive && context.stereoRenderingMode == PostProcessRenderContext.StereoRenderingMode.SinglePassInstanced)
			{
				propertySheet.EnableKeyword("STEREO_INSTANCING_ENABLED");
			}
			if (context.stereoActive && context.stereoRenderingMode == PostProcessRenderContext.StereoRenderingMode.SinglePassInstanced)
			{
				propertySheet.properties.SetFloat(ShaderIDs.DepthSlice, (float)eye);
				command.BlitFullscreenTriangleToTexArray(context.source, context.destination, propertySheet, 0, clear: false, eye);
			}
			else if (context.stereoActive && context.numberOfEyes > 1 && context.stereoRenderingMode == PostProcessRenderContext.StereoRenderingMode.SinglePass)
			{
				command.BlitFullscreenTriangleToDoubleWide(context.source, context.destination, propertySheet, 0, eye);
			}
			else
			{
				command.BlitFullscreenTriangle(context.source, context.destination, propertySheet, 0);
			}
		}
		else
		{
			PropertySheet propertySheet2 = context.propertySheets.Get(context.resources.shaders.finalPass);
			propertySheet2.ClearKeywords();
			propertySheet2.properties.Clear();
			context.uberSheet = propertySheet2;
			int num = -1;
			if (context.stereoActive && context.stereoRenderingMode == PostProcessRenderContext.StereoRenderingMode.SinglePassInstanced)
			{
				propertySheet2.EnableKeyword("STEREO_INSTANCING_ENABLED");
			}
			if (antialiasingMode == Antialiasing.FastApproximateAntialiasing)
			{
				propertySheet2.EnableKeyword(fastApproximateAntialiasing.fastMode ? "FXAA_LOW" : "FXAA");
				if (fastApproximateAntialiasing.keepAlpha)
				{
					propertySheet2.EnableKeyword("FXAA_KEEP_ALPHA");
				}
			}
			else if (antialiasingMode == Antialiasing.SubpixelMorphologicalAntialiasing && subpixelMorphologicalAntialiasing.IsSupported())
			{
				num = m_TargetPool.Get();
				RenderTargetIdentifier destination = context.destination;
				context.GetScreenSpaceTemporaryRT(context.command, num, 0, context.sourceFormat, (RenderTextureReadWrite)0, (FilterMode)1);
				context.destination = RenderTargetIdentifier.op_Implicit(num);
				subpixelMorphologicalAntialiasing.Render(context);
				context.source = RenderTargetIdentifier.op_Implicit(num);
				context.destination = destination;
			}
			dithering.Render(context);
			ApplyFlip(context, propertySheet2.properties);
			if (context.stereoActive && context.stereoRenderingMode == PostProcessRenderContext.StereoRenderingMode.SinglePassInstanced)
			{
				propertySheet2.properties.SetFloat(ShaderIDs.DepthSlice, (float)eye);
				command.BlitFullscreenTriangleToTexArray(context.source, context.destination, propertySheet2, 0, clear: false, eye);
			}
			else if (context.stereoActive && context.numberOfEyes > 1 && context.stereoRenderingMode == PostProcessRenderContext.StereoRenderingMode.SinglePass)
			{
				command.BlitFullscreenTriangleToDoubleWide(context.source, context.destination, propertySheet2, 0, eye);
			}
			else
			{
				command.BlitFullscreenTriangle(context.source, context.destination, propertySheet2, 0);
			}
			if (num > -1)
			{
				command.ReleaseTemporaryRT(num);
			}
		}
		if (releaseTargetAfterUse > -1)
		{
			command.ReleaseTemporaryRT(releaseTargetAfterUse);
		}
		command.EndSample("FinalPass");
	}

	private int RenderEffect<T>(PostProcessRenderContext context, bool useTempTarget = false) where T : PostProcessEffectSettings
	{
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		PostProcessBundle bundle = GetBundle<T>();
		if (!bundle.settings.IsEnabledAndSupported(context))
		{
			return -1;
		}
		if (m_IsRenderingInSceneView && !bundle.attribute.allowInSceneView)
		{
			return -1;
		}
		if (!useTempTarget)
		{
			bundle.renderer.Render(context);
			return -1;
		}
		RenderTargetIdentifier destination = context.destination;
		int num = m_TargetPool.Get();
		context.GetScreenSpaceTemporaryRT(context.command, num, 0, context.sourceFormat, (RenderTextureReadWrite)0, (FilterMode)1);
		context.destination = RenderTargetIdentifier.op_Implicit(num);
		bundle.renderer.Render(context);
		context.source = RenderTargetIdentifier.op_Implicit(num);
		context.destination = destination;
		return num;
	}

	private bool ShouldGenerateLogHistogram(PostProcessRenderContext context)
	{
		bool flag = GetBundle<AutoExposure>().settings.IsEnabledAndSupported(context);
		bool flag2 = debugLayer.lightMeter.IsRequestedAndSupported(context);
		return flag || flag2;
	}
}
