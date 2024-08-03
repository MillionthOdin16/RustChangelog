using System;

namespace UnityEngine.Rendering.PostProcessing;

[Serializable]
public sealed class WaveformMonitor : Monitor
{
	public float exposure = 0.12f;

	public int height = 256;

	private ComputeBuffer m_Data;

	private const int k_ThreadGroupSize = 256;

	private const int k_ThreadGroupSizeX = 16;

	private const int k_ThreadGroupSizeY = 16;

	internal override void OnDisable()
	{
		base.OnDisable();
		if (m_Data != null)
		{
			m_Data.Release();
		}
		m_Data = null;
	}

	internal override bool NeedsHalfRes()
	{
		return true;
	}

	internal override bool ShaderResourcesAvailable(PostProcessRenderContext context)
	{
		return Object.op_Implicit((Object)(object)context.resources.computeShaders.waveform);
	}

	internal override void Render(PostProcessRenderContext context)
	{
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Expected O, but got Unknown
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Expected O, but got Unknown
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		float num = (float)context.width / 2f / ((float)context.height / 2f);
		int num2 = Mathf.FloorToInt((float)height * num);
		CheckOutput(num2, height);
		exposure = Mathf.Max(0f, exposure);
		int num3 = num2 * height;
		if (m_Data == null)
		{
			m_Data = new ComputeBuffer(num3, 16);
		}
		else if (m_Data.count < num3)
		{
			m_Data.Release();
			m_Data = new ComputeBuffer(num3, 16);
		}
		ComputeShader waveform = context.resources.computeShaders.waveform;
		CommandBuffer command = context.command;
		command.BeginSample("Waveform");
		Vector4 val = default(Vector4);
		((Vector4)(ref val))._002Ector((float)num2, (float)height, (float)(RuntimeUtilities.isLinearColorSpace ? 1 : 0), 0f);
		int num4 = waveform.FindKernel("KWaveformClear");
		command.SetComputeBufferParam(waveform, num4, "_WaveformBuffer", m_Data);
		command.SetComputeVectorParam(waveform, "_Params", val);
		command.DispatchCompute(waveform, num4, Mathf.CeilToInt((float)num2 / 16f), Mathf.CeilToInt((float)height / 16f), 1);
		command.GetTemporaryRT(ShaderIDs.WaveformSource, num2, height, 0, (FilterMode)1, context.sourceFormat);
		command.BlitFullscreenTriangle(RenderTargetIdentifier.op_Implicit(ShaderIDs.HalfResFinalCopy), RenderTargetIdentifier.op_Implicit(ShaderIDs.WaveformSource));
		num4 = waveform.FindKernel("KWaveformGather");
		command.SetComputeBufferParam(waveform, num4, "_WaveformBuffer", m_Data);
		command.SetComputeTextureParam(waveform, num4, "_Source", RenderTargetIdentifier.op_Implicit(ShaderIDs.WaveformSource));
		command.SetComputeVectorParam(waveform, "_Params", val);
		command.DispatchCompute(waveform, num4, num2, Mathf.CeilToInt((float)height / 256f), 1);
		command.ReleaseTemporaryRT(ShaderIDs.WaveformSource);
		PropertySheet propertySheet = context.propertySheets.Get(context.resources.shaders.waveform);
		propertySheet.properties.SetVector(ShaderIDs.Params, new Vector4((float)num2, (float)height, exposure, 0f));
		propertySheet.properties.SetBuffer(ShaderIDs.WaveformBuffer, m_Data);
		command.BlitFullscreenTriangle(RenderTargetIdentifier.op_Implicit((BuiltinRenderTextureType)0), RenderTargetIdentifier.op_Implicit((Texture)(object)base.output), propertySheet, 0);
		command.EndSample("Waveform");
	}
}
