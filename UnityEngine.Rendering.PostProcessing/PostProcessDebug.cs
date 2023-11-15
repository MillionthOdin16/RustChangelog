namespace UnityEngine.Rendering.PostProcessing;

[ExecuteAlways]
[AddComponentMenu("Rendering/Post-process Debug", 1002)]
public sealed class PostProcessDebug : MonoBehaviour
{
	public PostProcessLayer postProcessLayer;

	private PostProcessLayer m_PreviousPostProcessLayer;

	public bool lightMeter;

	public bool histogram;

	public bool waveform;

	public bool vectorscope;

	public DebugOverlay debugOverlay = DebugOverlay.None;

	private Camera m_CurrentCamera;

	private CommandBuffer m_CmdAfterEverything;

	private void OnEnable()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Expected O, but got Unknown
		m_CmdAfterEverything = new CommandBuffer
		{
			name = "Post-processing Debug Overlay"
		};
	}

	private void OnDisable()
	{
		if ((Object)(object)m_CurrentCamera != (Object)null)
		{
			m_CurrentCamera.RemoveCommandBuffer((CameraEvent)19, m_CmdAfterEverything);
		}
		m_CurrentCamera = null;
		m_PreviousPostProcessLayer = null;
	}

	private void Update()
	{
		UpdateStates();
	}

	private void Reset()
	{
		postProcessLayer = ((Component)this).GetComponent<PostProcessLayer>();
	}

	private void UpdateStates()
	{
		if ((Object)(object)m_PreviousPostProcessLayer != (Object)(object)postProcessLayer)
		{
			if ((Object)(object)m_CurrentCamera != (Object)null)
			{
				m_CurrentCamera.RemoveCommandBuffer((CameraEvent)19, m_CmdAfterEverything);
				m_CurrentCamera = null;
			}
			m_PreviousPostProcessLayer = postProcessLayer;
			if ((Object)(object)postProcessLayer != (Object)null)
			{
				m_CurrentCamera = ((Component)postProcessLayer).GetComponent<Camera>();
				m_CurrentCamera.AddCommandBuffer((CameraEvent)19, m_CmdAfterEverything);
			}
		}
		if (!((Object)(object)postProcessLayer == (Object)null) && ((Behaviour)postProcessLayer).enabled)
		{
			if (lightMeter)
			{
				postProcessLayer.debugLayer.RequestMonitorPass(MonitorType.LightMeter);
			}
			if (histogram)
			{
				postProcessLayer.debugLayer.RequestMonitorPass(MonitorType.Histogram);
			}
			if (waveform)
			{
				postProcessLayer.debugLayer.RequestMonitorPass(MonitorType.Waveform);
			}
			if (vectorscope)
			{
				postProcessLayer.debugLayer.RequestMonitorPass(MonitorType.Vectorscope);
			}
			postProcessLayer.debugLayer.RequestDebugOverlay(debugOverlay);
		}
	}

	private void OnPostRender()
	{
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		m_CmdAfterEverything.Clear();
		if (!((Object)(object)postProcessLayer == (Object)null) && ((Behaviour)postProcessLayer).enabled && postProcessLayer.debugLayer.debugOverlayActive)
		{
			m_CmdAfterEverything.Blit((Texture)(object)postProcessLayer.debugLayer.debugOverlayTarget, RenderTargetIdentifier.op_Implicit((BuiltinRenderTextureType)2));
		}
	}

	private void OnGUI()
	{
		if (!((Object)(object)postProcessLayer == (Object)null) && ((Behaviour)postProcessLayer).enabled)
		{
			RenderTexture.active = null;
			Rect rect = default(Rect);
			((Rect)(ref rect))._002Ector(5f, 5f, 0f, 0f);
			PostProcessDebugLayer debugLayer = postProcessLayer.debugLayer;
			DrawMonitor(ref rect, debugLayer.lightMeter, lightMeter);
			DrawMonitor(ref rect, debugLayer.histogram, histogram);
			DrawMonitor(ref rect, debugLayer.waveform, waveform);
			DrawMonitor(ref rect, debugLayer.vectorscope, vectorscope);
		}
	}

	private void DrawMonitor(ref Rect rect, Monitor monitor, bool enabled)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		if (enabled && !((Object)(object)monitor.output == (Object)null))
		{
			((Rect)(ref rect)).width = ((Texture)monitor.output).width;
			((Rect)(ref rect)).height = ((Texture)monitor.output).height;
			GUI.DrawTexture(rect, (Texture)(object)monitor.output);
			((Rect)(ref rect)).x = ((Rect)(ref rect)).x + ((float)((Texture)monitor.output).width + 5f);
		}
	}
}
