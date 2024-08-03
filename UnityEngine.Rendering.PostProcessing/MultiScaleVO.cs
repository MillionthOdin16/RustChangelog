using System;
using UnityEngine.Scripting;

namespace UnityEngine.Rendering.PostProcessing;

[Serializable]
[Preserve]
internal sealed class MultiScaleVO : IAmbientOcclusionMethod
{
	internal enum MipLevel
	{
		Original,
		L1,
		L2,
		L3,
		L4,
		L5,
		L6
	}

	private enum Pass
	{
		DepthCopy,
		CompositionDeferred,
		CompositionForward,
		DebugOverlay
	}

	private readonly float[] m_SampleThickness = new float[12]
	{
		Mathf.Sqrt(0.96f),
		Mathf.Sqrt(0.84f),
		Mathf.Sqrt(0.64f),
		Mathf.Sqrt(0.35999995f),
		Mathf.Sqrt(0.91999996f),
		Mathf.Sqrt(0.79999995f),
		Mathf.Sqrt(0.59999996f),
		Mathf.Sqrt(0.31999993f),
		Mathf.Sqrt(0.67999995f),
		Mathf.Sqrt(0.47999996f),
		Mathf.Sqrt(0.19999993f),
		Mathf.Sqrt(0.27999997f)
	};

	private readonly float[] m_InvThicknessTable = new float[12];

	private readonly float[] m_SampleWeightTable = new float[12];

	private readonly int[] m_Widths = new int[7];

	private readonly int[] m_Heights = new int[7];

	private AmbientOcclusion m_Settings;

	private PropertySheet m_PropertySheet;

	private PostProcessResources m_Resources;

	private RenderTexture m_AmbientOnlyAO;

	private readonly RenderTargetIdentifier[] m_MRT = (RenderTargetIdentifier[])(object)new RenderTargetIdentifier[2]
	{
		RenderTargetIdentifier.op_Implicit((BuiltinRenderTextureType)10),
		RenderTargetIdentifier.op_Implicit((BuiltinRenderTextureType)2)
	};

	public MultiScaleVO(AmbientOcclusion settings)
	{
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		m_Settings = settings;
	}

	public DepthTextureMode GetCameraFlags()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		return (DepthTextureMode)1;
	}

	public void SetResources(PostProcessResources resources)
	{
		m_Resources = resources;
	}

	private void Alloc(CommandBuffer cmd, int id, MipLevel size, RenderTextureFormat format, bool uav)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		RenderTextureDescriptor val = default(RenderTextureDescriptor);
		((RenderTextureDescriptor)(ref val)).width = m_Widths[(int)size];
		((RenderTextureDescriptor)(ref val)).height = m_Heights[(int)size];
		((RenderTextureDescriptor)(ref val)).colorFormat = format;
		((RenderTextureDescriptor)(ref val)).depthBufferBits = 0;
		((RenderTextureDescriptor)(ref val)).volumeDepth = 1;
		((RenderTextureDescriptor)(ref val)).autoGenerateMips = false;
		((RenderTextureDescriptor)(ref val)).msaaSamples = 1;
		((RenderTextureDescriptor)(ref val)).enableRandomWrite = uav;
		((RenderTextureDescriptor)(ref val)).dimension = (TextureDimension)2;
		((RenderTextureDescriptor)(ref val)).sRGB = false;
		cmd.GetTemporaryRT(id, val, (FilterMode)0);
	}

	private void AllocArray(CommandBuffer cmd, int id, MipLevel size, RenderTextureFormat format, bool uav)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		RenderTextureDescriptor val = default(RenderTextureDescriptor);
		((RenderTextureDescriptor)(ref val)).width = m_Widths[(int)size];
		((RenderTextureDescriptor)(ref val)).height = m_Heights[(int)size];
		((RenderTextureDescriptor)(ref val)).colorFormat = format;
		((RenderTextureDescriptor)(ref val)).depthBufferBits = 0;
		((RenderTextureDescriptor)(ref val)).volumeDepth = 16;
		((RenderTextureDescriptor)(ref val)).autoGenerateMips = false;
		((RenderTextureDescriptor)(ref val)).msaaSamples = 1;
		((RenderTextureDescriptor)(ref val)).enableRandomWrite = uav;
		((RenderTextureDescriptor)(ref val)).dimension = (TextureDimension)5;
		((RenderTextureDescriptor)(ref val)).sRGB = false;
		cmd.GetTemporaryRT(id, val, (FilterMode)0);
	}

	private void Release(CommandBuffer cmd, int id)
	{
		cmd.ReleaseTemporaryRT(id);
	}

	private Vector4 CalculateZBufferParams(Camera camera)
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		float num = camera.farClipPlane / camera.nearClipPlane;
		if (SystemInfo.usesReversedZBuffer)
		{
			return new Vector4(num - 1f, 1f, 0f, 0f);
		}
		return new Vector4(1f - num, num, 0f, 0f);
	}

	private float CalculateTanHalfFovHeight(Camera camera)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		Matrix4x4 projectionMatrix = camera.projectionMatrix;
		return 1f / ((Matrix4x4)(ref projectionMatrix))[0, 0];
	}

	private Vector2 GetSize(MipLevel mip)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		return new Vector2((float)m_Widths[(int)mip], (float)m_Heights[(int)mip]);
	}

	private Vector3 GetSizeArray(MipLevel mip)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		return new Vector3((float)m_Widths[(int)mip], (float)m_Heights[(int)mip], 16f);
	}

	public void GenerateAOMap(CommandBuffer cmd, Camera camera, RenderTargetIdentifier destination, RenderTargetIdentifier? depthMap, bool invert, bool isMSAA)
	{
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		m_Widths[0] = camera.pixelWidth * ((!RuntimeUtilities.isSinglePassStereoEnabled) ? 1 : 2);
		m_Heights[0] = camera.pixelHeight;
		for (int i = 1; i < 7; i++)
		{
			int num = 1 << i;
			m_Widths[i] = (m_Widths[0] + (num - 1)) / num;
			m_Heights[i] = (m_Heights[0] + (num - 1)) / num;
		}
		PushAllocCommands(cmd, isMSAA);
		PushDownsampleCommands(cmd, camera, depthMap, isMSAA);
		float tanHalfFovH = CalculateTanHalfFovHeight(camera);
		PushRenderCommands(cmd, ShaderIDs.TiledDepth1, ShaderIDs.Occlusion1, GetSizeArray(MipLevel.L3), tanHalfFovH, isMSAA);
		PushRenderCommands(cmd, ShaderIDs.TiledDepth2, ShaderIDs.Occlusion2, GetSizeArray(MipLevel.L4), tanHalfFovH, isMSAA);
		PushRenderCommands(cmd, ShaderIDs.TiledDepth3, ShaderIDs.Occlusion3, GetSizeArray(MipLevel.L5), tanHalfFovH, isMSAA);
		PushRenderCommands(cmd, ShaderIDs.TiledDepth4, ShaderIDs.Occlusion4, GetSizeArray(MipLevel.L6), tanHalfFovH, isMSAA);
		PushUpsampleCommands(cmd, ShaderIDs.LowDepth4, ShaderIDs.Occlusion4, ShaderIDs.LowDepth3, ShaderIDs.Occlusion3, RenderTargetIdentifier.op_Implicit(ShaderIDs.Combined3), Vector2.op_Implicit(GetSize(MipLevel.L4)), GetSize(MipLevel.L3), isMSAA);
		PushUpsampleCommands(cmd, ShaderIDs.LowDepth3, ShaderIDs.Combined3, ShaderIDs.LowDepth2, ShaderIDs.Occlusion2, RenderTargetIdentifier.op_Implicit(ShaderIDs.Combined2), Vector2.op_Implicit(GetSize(MipLevel.L3)), GetSize(MipLevel.L2), isMSAA);
		PushUpsampleCommands(cmd, ShaderIDs.LowDepth2, ShaderIDs.Combined2, ShaderIDs.LowDepth1, ShaderIDs.Occlusion1, RenderTargetIdentifier.op_Implicit(ShaderIDs.Combined1), Vector2.op_Implicit(GetSize(MipLevel.L2)), GetSize(MipLevel.L1), isMSAA);
		PushUpsampleCommands(cmd, ShaderIDs.LowDepth1, ShaderIDs.Combined1, ShaderIDs.LinearDepth, null, destination, Vector2.op_Implicit(GetSize(MipLevel.L1)), GetSize(MipLevel.Original), isMSAA, invert);
		PushReleaseCommands(cmd);
	}

	private void PushAllocCommands(CommandBuffer cmd, bool isMSAA)
	{
		if (isMSAA)
		{
			Alloc(cmd, ShaderIDs.LinearDepth, MipLevel.Original, (RenderTextureFormat)13, uav: true);
			Alloc(cmd, ShaderIDs.LowDepth1, MipLevel.L1, (RenderTextureFormat)12, uav: true);
			Alloc(cmd, ShaderIDs.LowDepth2, MipLevel.L2, (RenderTextureFormat)12, uav: true);
			Alloc(cmd, ShaderIDs.LowDepth3, MipLevel.L3, (RenderTextureFormat)12, uav: true);
			Alloc(cmd, ShaderIDs.LowDepth4, MipLevel.L4, (RenderTextureFormat)12, uav: true);
			AllocArray(cmd, ShaderIDs.TiledDepth1, MipLevel.L3, (RenderTextureFormat)13, uav: true);
			AllocArray(cmd, ShaderIDs.TiledDepth2, MipLevel.L4, (RenderTextureFormat)13, uav: true);
			AllocArray(cmd, ShaderIDs.TiledDepth3, MipLevel.L5, (RenderTextureFormat)13, uav: true);
			AllocArray(cmd, ShaderIDs.TiledDepth4, MipLevel.L6, (RenderTextureFormat)13, uav: true);
			Alloc(cmd, ShaderIDs.Occlusion1, MipLevel.L1, (RenderTextureFormat)25, uav: true);
			Alloc(cmd, ShaderIDs.Occlusion2, MipLevel.L2, (RenderTextureFormat)25, uav: true);
			Alloc(cmd, ShaderIDs.Occlusion3, MipLevel.L3, (RenderTextureFormat)25, uav: true);
			Alloc(cmd, ShaderIDs.Occlusion4, MipLevel.L4, (RenderTextureFormat)25, uav: true);
			Alloc(cmd, ShaderIDs.Combined1, MipLevel.L1, (RenderTextureFormat)25, uav: true);
			Alloc(cmd, ShaderIDs.Combined2, MipLevel.L2, (RenderTextureFormat)25, uav: true);
			Alloc(cmd, ShaderIDs.Combined3, MipLevel.L3, (RenderTextureFormat)25, uav: true);
		}
		else
		{
			Alloc(cmd, ShaderIDs.LinearDepth, MipLevel.Original, (RenderTextureFormat)15, uav: true);
			Alloc(cmd, ShaderIDs.LowDepth1, MipLevel.L1, (RenderTextureFormat)14, uav: true);
			Alloc(cmd, ShaderIDs.LowDepth2, MipLevel.L2, (RenderTextureFormat)14, uav: true);
			Alloc(cmd, ShaderIDs.LowDepth3, MipLevel.L3, (RenderTextureFormat)14, uav: true);
			Alloc(cmd, ShaderIDs.LowDepth4, MipLevel.L4, (RenderTextureFormat)14, uav: true);
			AllocArray(cmd, ShaderIDs.TiledDepth1, MipLevel.L3, (RenderTextureFormat)15, uav: true);
			AllocArray(cmd, ShaderIDs.TiledDepth2, MipLevel.L4, (RenderTextureFormat)15, uav: true);
			AllocArray(cmd, ShaderIDs.TiledDepth3, MipLevel.L5, (RenderTextureFormat)15, uav: true);
			AllocArray(cmd, ShaderIDs.TiledDepth4, MipLevel.L6, (RenderTextureFormat)15, uav: true);
			Alloc(cmd, ShaderIDs.Occlusion1, MipLevel.L1, (RenderTextureFormat)16, uav: true);
			Alloc(cmd, ShaderIDs.Occlusion2, MipLevel.L2, (RenderTextureFormat)16, uav: true);
			Alloc(cmd, ShaderIDs.Occlusion3, MipLevel.L3, (RenderTextureFormat)16, uav: true);
			Alloc(cmd, ShaderIDs.Occlusion4, MipLevel.L4, (RenderTextureFormat)16, uav: true);
			Alloc(cmd, ShaderIDs.Combined1, MipLevel.L1, (RenderTextureFormat)16, uav: true);
			Alloc(cmd, ShaderIDs.Combined2, MipLevel.L2, (RenderTextureFormat)16, uav: true);
			Alloc(cmd, ShaderIDs.Combined3, MipLevel.L3, (RenderTextureFormat)16, uav: true);
		}
	}

	private void PushDownsampleCommands(CommandBuffer cmd, Camera camera, RenderTargetIdentifier? depthMap, bool isMSAA)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		bool flag = false;
		RenderTargetIdentifier val = default(RenderTargetIdentifier);
		if (depthMap.HasValue)
		{
			val = depthMap.Value;
		}
		else if (!RuntimeUtilities.IsResolvedDepthAvailable(camera))
		{
			Alloc(cmd, ShaderIDs.DepthCopy, MipLevel.Original, (RenderTextureFormat)14, uav: false);
			((RenderTargetIdentifier)(ref val))._002Ector(ShaderIDs.DepthCopy);
			cmd.BlitFullscreenTriangle(RenderTargetIdentifier.op_Implicit((BuiltinRenderTextureType)0), val, m_PropertySheet, 0);
			flag = true;
		}
		else
		{
			val = RenderTargetIdentifier.op_Implicit((BuiltinRenderTextureType)5);
		}
		ComputeShader multiScaleAODownsample = m_Resources.computeShaders.multiScaleAODownsample1;
		int num = multiScaleAODownsample.FindKernel(isMSAA ? "MultiScaleVODownsample1_MSAA" : "MultiScaleVODownsample1");
		cmd.SetComputeTextureParam(multiScaleAODownsample, num, "LinearZ", RenderTargetIdentifier.op_Implicit(ShaderIDs.LinearDepth));
		cmd.SetComputeTextureParam(multiScaleAODownsample, num, "DS2x", RenderTargetIdentifier.op_Implicit(ShaderIDs.LowDepth1));
		cmd.SetComputeTextureParam(multiScaleAODownsample, num, "DS4x", RenderTargetIdentifier.op_Implicit(ShaderIDs.LowDepth2));
		cmd.SetComputeTextureParam(multiScaleAODownsample, num, "DS2xAtlas", RenderTargetIdentifier.op_Implicit(ShaderIDs.TiledDepth1));
		cmd.SetComputeTextureParam(multiScaleAODownsample, num, "DS4xAtlas", RenderTargetIdentifier.op_Implicit(ShaderIDs.TiledDepth2));
		cmd.SetComputeVectorParam(multiScaleAODownsample, "ZBufferParams", CalculateZBufferParams(camera));
		cmd.SetComputeTextureParam(multiScaleAODownsample, num, "Depth", val);
		cmd.DispatchCompute(multiScaleAODownsample, num, m_Widths[4], m_Heights[4], 1);
		if (flag)
		{
			Release(cmd, ShaderIDs.DepthCopy);
		}
		multiScaleAODownsample = m_Resources.computeShaders.multiScaleAODownsample2;
		num = (isMSAA ? multiScaleAODownsample.FindKernel("MultiScaleVODownsample2_MSAA") : multiScaleAODownsample.FindKernel("MultiScaleVODownsample2"));
		cmd.SetComputeTextureParam(multiScaleAODownsample, num, "DS4x", RenderTargetIdentifier.op_Implicit(ShaderIDs.LowDepth2));
		cmd.SetComputeTextureParam(multiScaleAODownsample, num, "DS8x", RenderTargetIdentifier.op_Implicit(ShaderIDs.LowDepth3));
		cmd.SetComputeTextureParam(multiScaleAODownsample, num, "DS16x", RenderTargetIdentifier.op_Implicit(ShaderIDs.LowDepth4));
		cmd.SetComputeTextureParam(multiScaleAODownsample, num, "DS8xAtlas", RenderTargetIdentifier.op_Implicit(ShaderIDs.TiledDepth3));
		cmd.SetComputeTextureParam(multiScaleAODownsample, num, "DS16xAtlas", RenderTargetIdentifier.op_Implicit(ShaderIDs.TiledDepth4));
		cmd.DispatchCompute(multiScaleAODownsample, num, m_Widths[6], m_Heights[6], 1);
	}

	private void PushRenderCommands(CommandBuffer cmd, int source, int destination, Vector3 sourceSize, float tanHalfFovH, bool isMSAA)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		//IL_031b: Unknown result type (might be due to invalid IL or missing references)
		//IL_032b: Unknown result type (might be due to invalid IL or missing references)
		float num = 2f * tanHalfFovH * 10f / sourceSize.x;
		if (RuntimeUtilities.isSinglePassStereoEnabled)
		{
			num *= 2f;
		}
		float num2 = 1f / num;
		for (int i = 0; i < 12; i++)
		{
			m_InvThicknessTable[i] = num2 / m_SampleThickness[i];
		}
		m_SampleWeightTable[0] = 4f * m_SampleThickness[0];
		m_SampleWeightTable[1] = 4f * m_SampleThickness[1];
		m_SampleWeightTable[2] = 4f * m_SampleThickness[2];
		m_SampleWeightTable[3] = 4f * m_SampleThickness[3];
		m_SampleWeightTable[4] = 4f * m_SampleThickness[4];
		m_SampleWeightTable[5] = 8f * m_SampleThickness[5];
		m_SampleWeightTable[6] = 8f * m_SampleThickness[6];
		m_SampleWeightTable[7] = 8f * m_SampleThickness[7];
		m_SampleWeightTable[8] = 4f * m_SampleThickness[8];
		m_SampleWeightTable[9] = 8f * m_SampleThickness[9];
		m_SampleWeightTable[10] = 8f * m_SampleThickness[10];
		m_SampleWeightTable[11] = 4f * m_SampleThickness[11];
		m_SampleWeightTable[0] = 0f;
		m_SampleWeightTable[2] = 0f;
		m_SampleWeightTable[5] = 0f;
		m_SampleWeightTable[7] = 0f;
		m_SampleWeightTable[9] = 0f;
		float num3 = 0f;
		float[] sampleWeightTable = m_SampleWeightTable;
		foreach (float num4 in sampleWeightTable)
		{
			num3 += num4;
		}
		for (int k = 0; k < m_SampleWeightTable.Length; k++)
		{
			m_SampleWeightTable[k] /= num3;
		}
		ComputeShader multiScaleAORender = m_Resources.computeShaders.multiScaleAORender;
		int num5 = (isMSAA ? multiScaleAORender.FindKernel("MultiScaleVORender_MSAA_interleaved") : multiScaleAORender.FindKernel("MultiScaleVORender_interleaved"));
		cmd.SetComputeFloatParams(multiScaleAORender, "gInvThicknessTable", m_InvThicknessTable);
		cmd.SetComputeFloatParams(multiScaleAORender, "gSampleWeightTable", m_SampleWeightTable);
		cmd.SetComputeVectorParam(multiScaleAORender, "gInvSliceDimension", Vector4.op_Implicit(new Vector2(1f / sourceSize.x, 1f / sourceSize.y)));
		cmd.SetComputeVectorParam(multiScaleAORender, "AdditionalParams", Vector4.op_Implicit(new Vector2(-1f / m_Settings.thicknessModifier.value, m_Settings.intensity.value)));
		cmd.SetComputeTextureParam(multiScaleAORender, num5, "DepthTex", RenderTargetIdentifier.op_Implicit(source));
		cmd.SetComputeTextureParam(multiScaleAORender, num5, "Occlusion", RenderTargetIdentifier.op_Implicit(destination));
		uint num6 = default(uint);
		uint num7 = default(uint);
		uint num8 = default(uint);
		multiScaleAORender.GetKernelThreadGroupSizes(num5, ref num6, ref num7, ref num8);
		cmd.DispatchCompute(multiScaleAORender, num5, ((int)sourceSize.x + (int)num6 - 1) / (int)num6, ((int)sourceSize.y + (int)num7 - 1) / (int)num7, ((int)sourceSize.z + (int)num8 - 1) / (int)num8);
	}

	private void PushUpsampleCommands(CommandBuffer cmd, int lowResDepth, int interleavedAO, int highResDepth, int? highResAO, RenderTargetIdentifier dest, Vector3 lowResDepthSize, Vector2 highResDepthSize, bool isMSAA, bool invert = false)
	{
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		ComputeShader multiScaleAOUpsample = m_Resources.computeShaders.multiScaleAOUpsample;
		int num = 0;
		num = (isMSAA ? multiScaleAOUpsample.FindKernel(highResAO.HasValue ? "MultiScaleVOUpSample_MSAA_blendout" : (invert ? "MultiScaleVOUpSample_MSAA_invert" : "MultiScaleVOUpSample_MSAA")) : multiScaleAOUpsample.FindKernel(highResAO.HasValue ? "MultiScaleVOUpSample_blendout" : (invert ? "MultiScaleVOUpSample_invert" : "MultiScaleVOUpSample")));
		float num2 = 1920f / lowResDepthSize.x;
		float num3 = 1f - Mathf.Pow(10f, m_Settings.blurTolerance.value) * num2;
		num3 *= num3;
		float num4 = Mathf.Pow(10f, m_Settings.upsampleTolerance.value);
		float num5 = 1f / (Mathf.Pow(10f, m_Settings.noiseFilterTolerance.value) + num4);
		cmd.SetComputeVectorParam(multiScaleAOUpsample, "InvLowResolution", Vector4.op_Implicit(new Vector2(1f / lowResDepthSize.x, 1f / lowResDepthSize.y)));
		cmd.SetComputeVectorParam(multiScaleAOUpsample, "InvHighResolution", Vector4.op_Implicit(new Vector2(1f / highResDepthSize.x, 1f / highResDepthSize.y)));
		cmd.SetComputeVectorParam(multiScaleAOUpsample, "AdditionalParams", new Vector4(num5, num2, num3, num4));
		cmd.SetComputeTextureParam(multiScaleAOUpsample, num, "LoResDB", RenderTargetIdentifier.op_Implicit(lowResDepth));
		cmd.SetComputeTextureParam(multiScaleAOUpsample, num, "HiResDB", RenderTargetIdentifier.op_Implicit(highResDepth));
		cmd.SetComputeTextureParam(multiScaleAOUpsample, num, "LoResAO1", RenderTargetIdentifier.op_Implicit(interleavedAO));
		if (highResAO.HasValue)
		{
			cmd.SetComputeTextureParam(multiScaleAOUpsample, num, "HiResAO", RenderTargetIdentifier.op_Implicit(highResAO.Value));
		}
		cmd.SetComputeTextureParam(multiScaleAOUpsample, num, "AoResult", dest);
		int num6 = ((int)highResDepthSize.x + 17) / 16;
		int num7 = ((int)highResDepthSize.y + 17) / 16;
		cmd.DispatchCompute(multiScaleAOUpsample, num, num6, num7, 1);
	}

	private void PushReleaseCommands(CommandBuffer cmd)
	{
		Release(cmd, ShaderIDs.LinearDepth);
		Release(cmd, ShaderIDs.LowDepth1);
		Release(cmd, ShaderIDs.LowDepth2);
		Release(cmd, ShaderIDs.LowDepth3);
		Release(cmd, ShaderIDs.LowDepth4);
		Release(cmd, ShaderIDs.TiledDepth1);
		Release(cmd, ShaderIDs.TiledDepth2);
		Release(cmd, ShaderIDs.TiledDepth3);
		Release(cmd, ShaderIDs.TiledDepth4);
		Release(cmd, ShaderIDs.Occlusion1);
		Release(cmd, ShaderIDs.Occlusion2);
		Release(cmd, ShaderIDs.Occlusion3);
		Release(cmd, ShaderIDs.Occlusion4);
		Release(cmd, ShaderIDs.Combined1);
		Release(cmd, ShaderIDs.Combined2);
		Release(cmd, ShaderIDs.Combined3);
	}

	private void PreparePropertySheet(PostProcessRenderContext context)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		PropertySheet propertySheet = context.propertySheets.Get(m_Resources.shaders.multiScaleAO);
		propertySheet.ClearKeywords();
		propertySheet.properties.SetVector(ShaderIDs.AOColor, Color.op_Implicit(Color.white - m_Settings.color.value));
		m_PropertySheet = propertySheet;
	}

	private void CheckAOTexture(PostProcessRenderContext context)
	{
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Expected O, but got Unknown
		if ((Object)(object)m_AmbientOnlyAO == (Object)null || !m_AmbientOnlyAO.IsCreated() || ((Texture)m_AmbientOnlyAO).width != context.width || ((Texture)m_AmbientOnlyAO).height != context.height)
		{
			RuntimeUtilities.Destroy((Object)(object)m_AmbientOnlyAO);
			m_AmbientOnlyAO = new RenderTexture(context.width, context.height, 0, (RenderTextureFormat)16, (RenderTextureReadWrite)1)
			{
				hideFlags = (HideFlags)52,
				filterMode = (FilterMode)0,
				enableRandomWrite = true
			};
			m_AmbientOnlyAO.Create();
		}
	}

	private void PushDebug(PostProcessRenderContext context)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if (context.IsDebugOverlayEnabled(DebugOverlay.AmbientOcclusion))
		{
			context.PushDebugOverlay(context.command, RenderTargetIdentifier.op_Implicit((Texture)(object)m_AmbientOnlyAO), m_PropertySheet, 3);
		}
	}

	public void RenderAfterOpaque(PostProcessRenderContext context)
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Invalid comparison between Unknown and I4
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		CommandBuffer command = context.command;
		command.BeginSample("Ambient Occlusion");
		SetResources(context.resources);
		PreparePropertySheet(context);
		CheckAOTexture(context);
		if ((int)context.camera.actualRenderingPath == 1 && RenderSettings.fog)
		{
			m_PropertySheet.EnableKeyword("APPLY_FORWARD_FOG");
			m_PropertySheet.properties.SetVector(ShaderIDs.FogParams, Vector4.op_Implicit(new Vector3(RenderSettings.fogDensity, RenderSettings.fogStartDistance, RenderSettings.fogEndDistance)));
		}
		GenerateAOMap(command, context.camera, RenderTargetIdentifier.op_Implicit((Texture)(object)m_AmbientOnlyAO), null, invert: false, isMSAA: false);
		PushDebug(context);
		command.SetGlobalTexture(ShaderIDs.MSVOcclusionTexture, RenderTargetIdentifier.op_Implicit((Texture)(object)m_AmbientOnlyAO));
		command.BlitFullscreenTriangle(RenderTargetIdentifier.op_Implicit((BuiltinRenderTextureType)0), RenderTargetIdentifier.op_Implicit((BuiltinRenderTextureType)2), m_PropertySheet, 2, (RenderBufferLoadAction)0);
		command.EndSample("Ambient Occlusion");
	}

	public void RenderAmbientOnly(PostProcessRenderContext context)
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		CommandBuffer command = context.command;
		command.BeginSample("Ambient Occlusion Render");
		SetResources(context.resources);
		PreparePropertySheet(context);
		CheckAOTexture(context);
		GenerateAOMap(command, context.camera, RenderTargetIdentifier.op_Implicit((Texture)(object)m_AmbientOnlyAO), null, invert: false, isMSAA: false);
		PushDebug(context);
		command.EndSample("Ambient Occlusion Render");
	}

	public void CompositeAmbientOnly(PostProcessRenderContext context)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		CommandBuffer command = context.command;
		command.BeginSample("Ambient Occlusion Composite");
		command.SetGlobalTexture(ShaderIDs.MSVOcclusionTexture, RenderTargetIdentifier.op_Implicit((Texture)(object)m_AmbientOnlyAO));
		command.BlitFullscreenTriangle(RenderTargetIdentifier.op_Implicit((BuiltinRenderTextureType)0), m_MRT, RenderTargetIdentifier.op_Implicit((BuiltinRenderTextureType)2), m_PropertySheet, 1);
		command.EndSample("Ambient Occlusion Composite");
	}

	public void Release()
	{
		RuntimeUtilities.Destroy((Object)(object)m_AmbientOnlyAO);
		m_AmbientOnlyAO = null;
	}
}
