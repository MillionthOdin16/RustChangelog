using System.Collections.Generic;
using UnityEngine.Assertions;

namespace UnityEngine.Rendering.PostProcessing;

internal class TextureLerper
{
	private static TextureLerper m_Instance;

	private CommandBuffer m_Command;

	private PropertySheetFactory m_PropertySheets;

	private PostProcessResources m_Resources;

	private List<RenderTexture> m_Recycled;

	private List<RenderTexture> m_Actives;

	internal static TextureLerper instance
	{
		get
		{
			if (m_Instance == null)
			{
				m_Instance = new TextureLerper();
			}
			return m_Instance;
		}
	}

	private TextureLerper()
	{
		m_Recycled = new List<RenderTexture>();
		m_Actives = new List<RenderTexture>();
	}

	internal void BeginFrame(PostProcessRenderContext context)
	{
		m_Command = context.command;
		m_PropertySheets = context.propertySheets;
		m_Resources = context.resources;
	}

	internal void EndFrame()
	{
		if (m_Recycled.Count > 0)
		{
			foreach (RenderTexture item in m_Recycled)
			{
				RuntimeUtilities.Destroy((Object)(object)item);
			}
			m_Recycled.Clear();
		}
		if (m_Actives.Count <= 0)
		{
			return;
		}
		foreach (RenderTexture active in m_Actives)
		{
			m_Recycled.Add(active);
		}
		m_Actives.Clear();
	}

	private RenderTexture Get(RenderTextureFormat format, int w, int h, int d = 1, bool enableRandomWrite = false, bool force3D = false)
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Expected O, but got Unknown
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Invalid comparison between Unknown and I4
		RenderTexture val = null;
		int count = m_Recycled.Count;
		int i;
		for (i = 0; i < count; i++)
		{
			RenderTexture val2 = m_Recycled[i];
			if (((Texture)val2).width == w && ((Texture)val2).height == h && val2.volumeDepth == d && val2.format == format && val2.enableRandomWrite == enableRandomWrite && (!force3D || (int)((Texture)val2).dimension == 3))
			{
				val = val2;
				break;
			}
		}
		if ((Object)(object)val == (Object)null)
		{
			TextureDimension dimension = (TextureDimension)((d > 1 || force3D) ? 3 : 2);
			val = new RenderTexture(w, h, 0, format)
			{
				dimension = dimension,
				filterMode = (FilterMode)1,
				wrapMode = (TextureWrapMode)1,
				anisoLevel = 0,
				volumeDepth = d,
				enableRandomWrite = enableRandomWrite
			};
			val.Create();
		}
		else
		{
			m_Recycled.RemoveAt(i);
		}
		m_Actives.Add(val);
		return val;
	}

	internal Texture Lerp(Texture from, Texture to, float t)
	{
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		Assert.IsNotNull<Texture>(from);
		Assert.IsNotNull<Texture>(to);
		Assert.AreEqual(from.width, to.width);
		Assert.AreEqual(from.height, to.height);
		if ((Object)(object)from == (Object)(object)to)
		{
			return from;
		}
		if (t <= 0f)
		{
			return from;
		}
		if (t >= 1f)
		{
			return to;
		}
		RenderTexture val;
		if (from is Texture3D || (from is RenderTexture && ((RenderTexture)from).volumeDepth > 1))
		{
			int num = ((from is Texture3D) ? ((Texture3D)from).depth : ((RenderTexture)from).volumeDepth);
			int num2 = Mathf.Max(Mathf.Max(from.width, from.height), num);
			val = Get((RenderTextureFormat)2, from.width, from.height, num, enableRandomWrite: true, force3D: true);
			ComputeShader texture3dLerp = m_Resources.computeShaders.texture3dLerp;
			int num3 = texture3dLerp.FindKernel("KTexture3DLerp");
			m_Command.SetComputeVectorParam(texture3dLerp, "_DimensionsAndLerp", new Vector4((float)from.width, (float)from.height, (float)num, t));
			m_Command.SetComputeTextureParam(texture3dLerp, num3, "_Output", RenderTargetIdentifier.op_Implicit((Texture)(object)val));
			m_Command.SetComputeTextureParam(texture3dLerp, num3, "_From", RenderTargetIdentifier.op_Implicit(from));
			m_Command.SetComputeTextureParam(texture3dLerp, num3, "_To", RenderTargetIdentifier.op_Implicit(to));
			uint num4 = default(uint);
			uint num5 = default(uint);
			uint num6 = default(uint);
			texture3dLerp.GetKernelThreadGroupSizes(num3, ref num4, ref num5, ref num6);
			Assert.AreEqual(num4, num5);
			int num7 = Mathf.CeilToInt((float)num2 / (float)num4);
			int num8 = Mathf.CeilToInt((float)num2 / (float)num6);
			m_Command.DispatchCompute(texture3dLerp, num3, num7, num7, num8);
			return (Texture)(object)val;
		}
		RenderTextureFormat uncompressedRenderTextureFormat = TextureFormatUtilities.GetUncompressedRenderTextureFormat(to);
		val = Get(uncompressedRenderTextureFormat, to.width, to.height);
		PropertySheet propertySheet = m_PropertySheets.Get(m_Resources.shaders.texture2dLerp);
		propertySheet.properties.SetTexture(ShaderIDs.To, to);
		propertySheet.properties.SetFloat(ShaderIDs.Interp, t);
		m_Command.BlitFullscreenTriangle(RenderTargetIdentifier.op_Implicit(from), RenderTargetIdentifier.op_Implicit((Texture)(object)val), propertySheet, 0);
		return (Texture)(object)val;
	}

	internal Texture Lerp(Texture from, Color to, float t)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		Assert.IsNotNull<Texture>(from);
		if ((double)t < 1E-05)
		{
			return from;
		}
		RenderTexture val;
		if (from is Texture3D || (from is RenderTexture && ((RenderTexture)from).volumeDepth > 1))
		{
			int num = ((from is Texture3D) ? ((Texture3D)from).depth : ((RenderTexture)from).volumeDepth);
			int num2 = Mathf.Max(Mathf.Max(from.width, from.height), num);
			val = Get((RenderTextureFormat)2, from.width, from.height, num, enableRandomWrite: true, force3D: true);
			ComputeShader texture3dLerp = m_Resources.computeShaders.texture3dLerp;
			int num3 = texture3dLerp.FindKernel("KTexture3DLerpToColor");
			m_Command.SetComputeVectorParam(texture3dLerp, "_DimensionsAndLerp", new Vector4((float)from.width, (float)from.height, (float)num, t));
			m_Command.SetComputeVectorParam(texture3dLerp, "_TargetColor", new Vector4(to.r, to.g, to.b, to.a));
			m_Command.SetComputeTextureParam(texture3dLerp, num3, "_Output", RenderTargetIdentifier.op_Implicit((Texture)(object)val));
			m_Command.SetComputeTextureParam(texture3dLerp, num3, "_From", RenderTargetIdentifier.op_Implicit(from));
			int num4 = Mathf.CeilToInt((float)num2 / 4f);
			m_Command.DispatchCompute(texture3dLerp, num3, num4, num4, num4);
			return (Texture)(object)val;
		}
		RenderTextureFormat uncompressedRenderTextureFormat = TextureFormatUtilities.GetUncompressedRenderTextureFormat(from);
		val = Get(uncompressedRenderTextureFormat, from.width, from.height);
		PropertySheet propertySheet = m_PropertySheets.Get(m_Resources.shaders.texture2dLerp);
		propertySheet.properties.SetVector(ShaderIDs.TargetColor, new Vector4(to.r, to.g, to.b, to.a));
		propertySheet.properties.SetFloat(ShaderIDs.Interp, t);
		m_Command.BlitFullscreenTriangle(RenderTargetIdentifier.op_Implicit(from), RenderTargetIdentifier.op_Implicit((Texture)(object)val), propertySheet, 1);
		return (Texture)(object)val;
	}

	internal void Clear()
	{
		foreach (RenderTexture active in m_Actives)
		{
			RuntimeUtilities.Destroy((Object)(object)active);
		}
		foreach (RenderTexture item in m_Recycled)
		{
			RuntimeUtilities.Destroy((Object)(object)item);
		}
		m_Actives.Clear();
		m_Recycled.Clear();
	}
}
