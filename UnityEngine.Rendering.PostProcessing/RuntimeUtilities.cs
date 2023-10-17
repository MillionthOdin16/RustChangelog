using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

namespace UnityEngine.Rendering.PostProcessing;

public static class RuntimeUtilities
{
	private static Texture2D m_WhiteTexture;

	private static Texture3D m_WhiteTexture3D;

	private static Texture2D m_BlackTexture;

	private static Texture3D m_BlackTexture3D;

	private static Texture2D m_TransparentTexture;

	private static Texture3D m_TransparentTexture3D;

	private static Dictionary<int, Texture2D> m_LutStrips = new Dictionary<int, Texture2D>();

	internal static PostProcessResources s_Resources;

	private static Mesh s_FullscreenTriangle;

	private static Material s_CopyStdMaterial;

	private static Material s_CopyStdFromDoubleWideMaterial;

	private static Material s_CopyMaterial;

	private static Material s_CopyFromTexArrayMaterial;

	private static PropertySheet s_CopySheet;

	private static PropertySheet s_CopyFromTexArraySheet;

	private static IEnumerable<Type> m_AssemblyTypes;

	public static Texture2D whiteTexture
	{
		get
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Expected O, but got Unknown
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			if ((Object)(object)m_WhiteTexture == (Object)null)
			{
				m_WhiteTexture = new Texture2D(1, 1, (TextureFormat)5, false)
				{
					name = "White Texture"
				};
				m_WhiteTexture.SetPixel(0, 0, Color.white);
				m_WhiteTexture.Apply();
			}
			return m_WhiteTexture;
		}
	}

	public static Texture3D whiteTexture3D
	{
		get
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Expected O, but got Unknown
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			if ((Object)(object)m_WhiteTexture3D == (Object)null)
			{
				m_WhiteTexture3D = new Texture3D(1, 1, 1, (TextureFormat)5, false)
				{
					name = "White Texture 3D"
				};
				m_WhiteTexture3D.SetPixels((Color[])(object)new Color[1] { Color.white });
				m_WhiteTexture3D.Apply();
			}
			return m_WhiteTexture3D;
		}
	}

	public static Texture2D blackTexture
	{
		get
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Expected O, but got Unknown
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			if ((Object)(object)m_BlackTexture == (Object)null)
			{
				m_BlackTexture = new Texture2D(1, 1, (TextureFormat)5, false)
				{
					name = "Black Texture"
				};
				m_BlackTexture.SetPixel(0, 0, Color.black);
				m_BlackTexture.Apply();
			}
			return m_BlackTexture;
		}
	}

	public static Texture3D blackTexture3D
	{
		get
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Expected O, but got Unknown
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			if ((Object)(object)m_BlackTexture3D == (Object)null)
			{
				m_BlackTexture3D = new Texture3D(1, 1, 1, (TextureFormat)5, false)
				{
					name = "Black Texture 3D"
				};
				m_BlackTexture3D.SetPixels((Color[])(object)new Color[1] { Color.black });
				m_BlackTexture3D.Apply();
			}
			return m_BlackTexture3D;
		}
	}

	public static Texture2D transparentTexture
	{
		get
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Expected O, but got Unknown
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			if ((Object)(object)m_TransparentTexture == (Object)null)
			{
				m_TransparentTexture = new Texture2D(1, 1, (TextureFormat)5, false)
				{
					name = "Transparent Texture"
				};
				m_TransparentTexture.SetPixel(0, 0, Color.clear);
				m_TransparentTexture.Apply();
			}
			return m_TransparentTexture;
		}
	}

	public static Texture3D transparentTexture3D
	{
		get
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Expected O, but got Unknown
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			if ((Object)(object)m_TransparentTexture3D == (Object)null)
			{
				m_TransparentTexture3D = new Texture3D(1, 1, 1, (TextureFormat)5, false)
				{
					name = "Transparent Texture 3D"
				};
				m_TransparentTexture3D.SetPixels((Color[])(object)new Color[1] { Color.clear });
				m_TransparentTexture3D.Apply();
			}
			return m_TransparentTexture3D;
		}
	}

	public static Mesh fullscreenTriangle
	{
		get
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Expected O, but got Unknown
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			if ((Object)(object)s_FullscreenTriangle != (Object)null)
			{
				return s_FullscreenTriangle;
			}
			s_FullscreenTriangle = new Mesh
			{
				name = "Fullscreen Triangle"
			};
			s_FullscreenTriangle.SetVertices(new List<Vector3>
			{
				new Vector3(-1f, -1f, 0f),
				new Vector3(-1f, 3f, 0f),
				new Vector3(3f, -1f, 0f)
			});
			s_FullscreenTriangle.SetIndices(new int[3] { 0, 1, 2 }, (MeshTopology)0, 0, false);
			s_FullscreenTriangle.UploadMeshData(false);
			return s_FullscreenTriangle;
		}
	}

	public static Material copyStdMaterial
	{
		get
		{
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Expected O, but got Unknown
			if ((Object)(object)s_CopyStdMaterial != (Object)null)
			{
				return s_CopyStdMaterial;
			}
			Assert.IsNotNull<PostProcessResources>(s_Resources);
			Shader copyStd = s_Resources.shaders.copyStd;
			s_CopyStdMaterial = new Material(copyStd)
			{
				name = "PostProcess - CopyStd",
				hideFlags = (HideFlags)61
			};
			return s_CopyStdMaterial;
		}
	}

	public static Material copyStdFromDoubleWideMaterial
	{
		get
		{
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Expected O, but got Unknown
			if ((Object)(object)s_CopyStdFromDoubleWideMaterial != (Object)null)
			{
				return s_CopyStdFromDoubleWideMaterial;
			}
			Assert.IsNotNull<PostProcessResources>(s_Resources);
			Shader copyStdFromDoubleWide = s_Resources.shaders.copyStdFromDoubleWide;
			s_CopyStdFromDoubleWideMaterial = new Material(copyStdFromDoubleWide)
			{
				name = "PostProcess - CopyStdFromDoubleWide",
				hideFlags = (HideFlags)61
			};
			return s_CopyStdFromDoubleWideMaterial;
		}
	}

	public static Material copyMaterial
	{
		get
		{
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Expected O, but got Unknown
			if ((Object)(object)s_CopyMaterial != (Object)null)
			{
				return s_CopyMaterial;
			}
			Assert.IsNotNull<PostProcessResources>(s_Resources);
			Shader copy = s_Resources.shaders.copy;
			s_CopyMaterial = new Material(copy)
			{
				name = "PostProcess - Copy",
				hideFlags = (HideFlags)61
			};
			return s_CopyMaterial;
		}
	}

	public static Material copyFromTexArrayMaterial
	{
		get
		{
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Expected O, but got Unknown
			if ((Object)(object)s_CopyFromTexArrayMaterial != (Object)null)
			{
				return s_CopyFromTexArrayMaterial;
			}
			Assert.IsNotNull<PostProcessResources>(s_Resources);
			Shader copyStdFromTexArray = s_Resources.shaders.copyStdFromTexArray;
			s_CopyFromTexArrayMaterial = new Material(copyStdFromTexArray)
			{
				name = "PostProcess - CopyFromTexArray",
				hideFlags = (HideFlags)61
			};
			return s_CopyFromTexArrayMaterial;
		}
	}

	public static PropertySheet copySheet
	{
		get
		{
			if (s_CopySheet == null)
			{
				s_CopySheet = new PropertySheet(copyMaterial);
			}
			return s_CopySheet;
		}
	}

	public static PropertySheet copyFromTexArraySheet
	{
		get
		{
			if (s_CopyFromTexArraySheet == null)
			{
				s_CopyFromTexArraySheet = new PropertySheet(copyFromTexArrayMaterial);
			}
			return s_CopyFromTexArraySheet;
		}
	}

	public static bool scriptableRenderPipelineActive => (Object)(object)GraphicsSettings.renderPipelineAsset != (Object)null;

	public static bool supportsDeferredShading => scriptableRenderPipelineActive || (int)GraphicsSettings.GetShaderMode((BuiltinShaderType)0) > 0;

	public static bool supportsDepthNormals => scriptableRenderPipelineActive || (int)GraphicsSettings.GetShaderMode((BuiltinShaderType)4) > 0;

	public static bool isSinglePassStereoEnabled => false;

	public static bool isVREnabled => false;

	public static bool isAndroidOpenGL => (int)Application.platform == 11 && (int)SystemInfo.graphicsDeviceType != 21;

	public static RenderTextureFormat defaultHDRRenderTextureFormat => (RenderTextureFormat)9;

	public static bool isLinearColorSpace => (int)QualitySettings.activeColorSpace == 1;

	public static Texture2D GetLutStrip(int size)
	{
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Expected O, but got Unknown
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		if (!m_LutStrips.TryGetValue(size, out var value))
		{
			int num = size * size;
			Color[] array = (Color[])(object)new Color[num * size];
			float num2 = 1f / ((float)size - 1f);
			for (int i = 0; i < size; i++)
			{
				int num3 = i * size;
				float num4 = (float)i * num2;
				for (int j = 0; j < size; j++)
				{
					float num5 = (float)j * num2;
					for (int k = 0; k < size; k++)
					{
						float num6 = (float)k * num2;
						array[j * num + num3 + k] = new Color(num6, num5, num4);
					}
				}
			}
			TextureFormat val = (TextureFormat)17;
			if (!val.IsSupported())
			{
				val = (TextureFormat)5;
			}
			value = new Texture2D(size * size, size, val, false, true)
			{
				name = "Strip Lut" + size,
				hideFlags = (HideFlags)52,
				filterMode = (FilterMode)1,
				wrapMode = (TextureWrapMode)1,
				anisoLevel = 0
			};
			value.SetPixels(array);
			value.Apply();
			m_LutStrips.Add(size, value);
		}
		return value;
	}

	public static void SetRenderTargetWithLoadStoreAction(this CommandBuffer cmd, RenderTargetIdentifier rt, RenderBufferLoadAction loadAction, RenderBufferStoreAction storeAction)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		cmd.SetRenderTarget(rt, loadAction, storeAction);
	}

	public static void SetRenderTargetWithLoadStoreAction(this CommandBuffer cmd, RenderTargetIdentifier color, RenderBufferLoadAction colorLoadAction, RenderBufferStoreAction colorStoreAction, RenderTargetIdentifier depth, RenderBufferLoadAction depthLoadAction, RenderBufferStoreAction depthStoreAction)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		cmd.SetRenderTarget(color, colorLoadAction, colorStoreAction, depth, depthLoadAction, depthStoreAction);
	}

	public static void BlitFullscreenTriangle(this CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier destination, bool clear = false, Rect? viewport = null)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		cmd.SetGlobalTexture(ShaderIDs.MainTex, source);
		cmd.SetRenderTargetWithLoadStoreAction(destination, (RenderBufferLoadAction)((!viewport.HasValue) ? 2 : 0), (RenderBufferStoreAction)0);
		if (viewport.HasValue)
		{
			cmd.SetViewport(viewport.Value);
		}
		if (clear)
		{
			cmd.ClearRenderTarget(true, true, Color.clear);
		}
		cmd.DrawMesh(fullscreenTriangle, Matrix4x4.identity, copyMaterial, 0, 0);
	}

	public static void BlitFullscreenTriangle(this CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier destination, PropertySheet propertySheet, int pass, RenderBufferLoadAction loadAction, Rect? viewport = null)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Invalid comparison between Unknown and I4
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Expected O, but got I4
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c->IL002c: Incompatible stack types: O vs I4
		//IL_002a->IL002c: Incompatible stack types: I4 vs O
		//IL_002a->IL002c: Incompatible stack types: O vs I4
		cmd.SetGlobalTexture(ShaderIDs.MainTex, source);
		bool flag = (int)loadAction == 1;
		if (flag)
		{
			loadAction = (RenderBufferLoadAction)2;
		}
		object obj = cmd;
		int num;
		if (!viewport.HasValue)
		{
			obj = loadAction;
			num = (int)obj;
		}
		else
		{
			num = 0;
			obj = num;
			num = (int)obj;
		}
		((CommandBuffer)(object)num).SetRenderTargetWithLoadStoreAction(destination, (RenderBufferLoadAction)obj, (RenderBufferStoreAction)0);
		if (viewport.HasValue)
		{
			cmd.SetViewport(viewport.Value);
		}
		if (flag)
		{
			cmd.ClearRenderTarget(true, true, Color.clear);
		}
		cmd.DrawMesh(fullscreenTriangle, Matrix4x4.identity, propertySheet.material, 0, pass, propertySheet.properties);
	}

	public static void BlitFullscreenTriangle(this CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier destination, PropertySheet propertySheet, int pass, bool clear = false, Rect? viewport = null)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		cmd.BlitFullscreenTriangle(source, destination, propertySheet, pass, (RenderBufferLoadAction)(clear ? 1 : 2), viewport);
	}

	public static void BlitFullscreenTriangleFromDoubleWide(this CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier destination, Material material, int pass, int eye)
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		Vector4 val = default(Vector4);
		((Vector4)(ref val))._002Ector(0.5f, 1f, 0f, 0f);
		if (eye == 1)
		{
			val.z = 0.5f;
		}
		cmd.SetGlobalVector(ShaderIDs.UVScaleOffset, val);
		cmd.BuiltinBlit(source, destination, material, pass);
	}

	public static void BlitFullscreenTriangleToDoubleWide(this CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier destination, PropertySheet propertySheet, int pass, int eye)
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		Vector4 val = default(Vector4);
		((Vector4)(ref val))._002Ector(0.5f, 1f, -0.5f, 0f);
		if (eye == 1)
		{
			val.z = 0.5f;
		}
		propertySheet.EnableKeyword("STEREO_DOUBLEWIDE_TARGET");
		propertySheet.properties.SetVector(ShaderIDs.PosScaleOffset, val);
		cmd.BlitFullscreenTriangle(source, destination, propertySheet, 0);
	}

	public static void BlitFullscreenTriangleFromTexArray(this CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier destination, PropertySheet propertySheet, int pass, bool clear = false, int depthSlice = -1)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		cmd.SetGlobalTexture(ShaderIDs.MainTex, source);
		cmd.SetGlobalFloat(ShaderIDs.DepthSlice, (float)depthSlice);
		cmd.SetRenderTargetWithLoadStoreAction(destination, (RenderBufferLoadAction)2, (RenderBufferStoreAction)0);
		if (clear)
		{
			cmd.ClearRenderTarget(true, true, Color.clear);
		}
		cmd.DrawMesh(fullscreenTriangle, Matrix4x4.identity, propertySheet.material, 0, pass, propertySheet.properties);
	}

	public static void BlitFullscreenTriangleToTexArray(this CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier destination, PropertySheet propertySheet, int pass, bool clear = false, int depthSlice = -1)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		cmd.SetGlobalTexture(ShaderIDs.MainTex, source);
		cmd.SetGlobalFloat(ShaderIDs.DepthSlice, (float)depthSlice);
		cmd.SetRenderTarget(destination, 0, (CubemapFace)(-1), -1);
		if (clear)
		{
			cmd.ClearRenderTarget(true, true, Color.clear);
		}
		cmd.DrawMesh(fullscreenTriangle, Matrix4x4.identity, propertySheet.material, 0, pass, propertySheet.properties);
	}

	public static void BlitFullscreenTriangle(this CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier destination, RenderTargetIdentifier depth, PropertySheet propertySheet, int pass, bool clear = false, Rect? viewport = null)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		cmd.SetGlobalTexture(ShaderIDs.MainTex, source);
		RenderBufferLoadAction val = (RenderBufferLoadAction)((!viewport.HasValue) ? 2 : 0);
		if (clear)
		{
			cmd.SetRenderTargetWithLoadStoreAction(destination, val, (RenderBufferStoreAction)0, depth, val, (RenderBufferStoreAction)0);
			cmd.ClearRenderTarget(true, true, Color.clear);
		}
		else
		{
			cmd.SetRenderTargetWithLoadStoreAction(destination, val, (RenderBufferStoreAction)0, depth, (RenderBufferLoadAction)0, (RenderBufferStoreAction)0);
		}
		if (viewport.HasValue)
		{
			cmd.SetViewport(viewport.Value);
		}
		cmd.DrawMesh(fullscreenTriangle, Matrix4x4.identity, propertySheet.material, 0, pass, propertySheet.properties);
	}

	public static void BlitFullscreenTriangle(this CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier[] destinations, RenderTargetIdentifier depth, PropertySheet propertySheet, int pass, bool clear = false, Rect? viewport = null)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		cmd.SetGlobalTexture(ShaderIDs.MainTex, source);
		cmd.SetRenderTarget(destinations, depth);
		if (viewport.HasValue)
		{
			cmd.SetViewport(viewport.Value);
		}
		if (clear)
		{
			cmd.ClearRenderTarget(true, true, Color.clear);
		}
		cmd.DrawMesh(fullscreenTriangle, Matrix4x4.identity, propertySheet.material, 0, pass, propertySheet.properties);
	}

	public static void BuiltinBlit(this CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier destination)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		cmd.SetRenderTarget(destination, (RenderBufferLoadAction)2, (RenderBufferStoreAction)0);
		destination = RenderTargetIdentifier.op_Implicit((BuiltinRenderTextureType)1);
		cmd.Blit(source, destination);
	}

	public static void BuiltinBlit(this CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier destination, Material mat, int pass = 0)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		cmd.SetRenderTarget(destination, (RenderBufferLoadAction)2, (RenderBufferStoreAction)0);
		destination = RenderTargetIdentifier.op_Implicit((BuiltinRenderTextureType)1);
		cmd.Blit(source, destination, mat, pass);
	}

	public static void CopyTexture(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier destination)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Invalid comparison between Unknown and I4
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		if ((int)SystemInfo.copyTextureSupport > 0)
		{
			cmd.CopyTexture(source, destination);
		}
		else
		{
			cmd.BlitFullscreenTriangle(source, destination);
		}
	}

	public static bool isFloatingPointFormat(RenderTextureFormat format)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Invalid comparison between Unknown and I4
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Invalid comparison between Unknown and I4
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Invalid comparison between Unknown and I4
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Invalid comparison between Unknown and I4
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Invalid comparison between Unknown and I4
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Invalid comparison between Unknown and I4
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Invalid comparison between Unknown and I4
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Invalid comparison between Unknown and I4
		return (int)format == 9 || (int)format == 2 || (int)format == 11 || (int)format == 12 || (int)format == 13 || (int)format == 14 || (int)format == 15 || (int)format == 22;
	}

	public static void Destroy(Object obj)
	{
		if (obj != (Object)null)
		{
			Object.Destroy(obj);
		}
	}

	public static bool IsResolvedDepthAvailable(Camera camera)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Invalid comparison between Unknown and I4
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Invalid comparison between Unknown and I4
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Invalid comparison between Unknown and I4
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Invalid comparison between Unknown and I4
		GraphicsDeviceType graphicsDeviceType = SystemInfo.graphicsDeviceType;
		return (int)camera.actualRenderingPath == 3 && ((int)graphicsDeviceType == 2 || (int)graphicsDeviceType == 18 || (int)graphicsDeviceType == 14);
	}

	public static void DestroyProfile(PostProcessProfile profile, bool destroyEffects)
	{
		if (destroyEffects)
		{
			foreach (PostProcessEffectSettings setting in profile.settings)
			{
				Destroy((Object)(object)setting);
			}
		}
		Destroy((Object)(object)profile);
	}

	public static void DestroyVolume(PostProcessVolume volume, bool destroyProfile, bool destroyGameObject = false)
	{
		if (destroyProfile)
		{
			DestroyProfile(volume.profileRef, destroyEffects: true);
		}
		GameObject gameObject = ((Component)volume).gameObject;
		Destroy((Object)(object)volume);
		if (destroyGameObject)
		{
			Destroy((Object)(object)gameObject);
		}
	}

	public static bool IsPostProcessingActive(PostProcessLayer layer)
	{
		return (Object)(object)layer != (Object)null && ((Behaviour)layer).enabled;
	}

	public static bool IsTemporalAntialiasingActive(PostProcessLayer layer)
	{
		return IsPostProcessingActive(layer) && layer.antialiasingMode == PostProcessLayer.Antialiasing.TemporalAntialiasing && layer.temporalAntialiasing.IsSupported();
	}

	public static IEnumerable<T> GetAllSceneObjects<T>() where T : Component
	{
		Queue<Transform> queue = new Queue<Transform>();
		Scene activeScene = SceneManager.GetActiveScene();
		GameObject[] roots = ((Scene)(ref activeScene)).GetRootGameObjects();
		GameObject[] array = roots;
		foreach (GameObject root in array)
		{
			queue.Enqueue(root.transform);
			T comp = root.GetComponent<T>();
			if ((Object)(object)comp != (Object)null)
			{
				yield return comp;
			}
		}
		while (queue.Count > 0)
		{
			foreach (Transform item in queue.Dequeue())
			{
				Transform child = item;
				queue.Enqueue(child);
				T comp2 = ((Component)child).GetComponent<T>();
				if ((Object)(object)comp2 != (Object)null)
				{
					yield return comp2;
				}
			}
		}
	}

	public static void CreateIfNull<T>(ref T obj) where T : class, new()
	{
		if (obj == null)
		{
			obj = new T();
		}
	}

	public static float Exp2(float x)
	{
		return Mathf.Exp(x * 0.6931472f);
	}

	public static Matrix4x4 GetJitteredPerspectiveProjectionMatrix(Camera camera, Vector2 offset)
	{
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		float nearClipPlane = camera.nearClipPlane;
		float farClipPlane = camera.farClipPlane;
		float num = Mathf.Tan((float)Math.PI / 360f * camera.fieldOfView) * nearClipPlane;
		float num2 = num * camera.aspect;
		offset.x *= num2 / (0.5f * (float)camera.pixelWidth);
		offset.y *= num / (0.5f * (float)camera.pixelHeight);
		Matrix4x4 projectionMatrix = camera.projectionMatrix;
		ref Matrix4x4 reference = ref projectionMatrix;
		((Matrix4x4)(ref reference))[0, 2] = ((Matrix4x4)(ref reference))[0, 2] + offset.x / num2;
		reference = ref projectionMatrix;
		((Matrix4x4)(ref reference))[1, 2] = ((Matrix4x4)(ref reference))[1, 2] + offset.y / num;
		return projectionMatrix;
	}

	public static Matrix4x4 GetJitteredOrthographicProjectionMatrix(Camera camera, Vector2 offset)
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		float orthographicSize = camera.orthographicSize;
		float num = orthographicSize * camera.aspect;
		offset.x *= num / (0.5f * (float)camera.pixelWidth);
		offset.y *= orthographicSize / (0.5f * (float)camera.pixelHeight);
		float num2 = offset.x - num;
		float num3 = offset.x + num;
		float num4 = offset.y + orthographicSize;
		float num5 = offset.y - orthographicSize;
		return Matrix4x4.Ortho(num2, num3, num5, num4, camera.nearClipPlane, camera.farClipPlane);
	}

	public static Matrix4x4 GenerateJitteredProjectionMatrixFromOriginal(PostProcessRenderContext context, Matrix4x4 origProj, Vector2 jitter)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		FrustumPlanes decomposeProjection = ((Matrix4x4)(ref origProj)).decomposeProjection;
		float num = Math.Abs(decomposeProjection.top) + Math.Abs(decomposeProjection.bottom);
		float num2 = Math.Abs(decomposeProjection.left) + Math.Abs(decomposeProjection.right);
		Vector2 val = default(Vector2);
		((Vector2)(ref val))._002Ector(jitter.x * num2 / (float)context.screenWidth, jitter.y * num / (float)context.screenHeight);
		decomposeProjection.left += val.x;
		decomposeProjection.right += val.x;
		decomposeProjection.top += val.y;
		decomposeProjection.bottom += val.y;
		return Matrix4x4.Frustum(decomposeProjection);
	}

	public static IEnumerable<Type> GetAllAssemblyTypes()
	{
		if (m_AssemblyTypes == null)
		{
			m_AssemblyTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(delegate(Assembly t)
			{
				Type[] result = new Type[0];
				try
				{
					result = t.GetTypes();
				}
				catch
				{
				}
				return result;
			});
		}
		return m_AssemblyTypes;
	}

	public static T GetAttribute<T>(this Type type) where T : Attribute
	{
		Assert.IsTrue(type.IsDefined(typeof(T), inherit: false), "Attribute not found");
		return (T)type.GetCustomAttributes(typeof(T), inherit: false)[0];
	}

	public static Attribute[] GetMemberAttributes<TType, TValue>(Expression<Func<TType, TValue>> expr)
	{
		Expression expression = expr;
		if (expression is LambdaExpression)
		{
			expression = ((LambdaExpression)expression).Body;
		}
		ExpressionType nodeType = expression.NodeType;
		if (nodeType == ExpressionType.MemberAccess)
		{
			FieldInfo fieldInfo = (FieldInfo)((MemberExpression)expression).Member;
			return fieldInfo.GetCustomAttributes(inherit: false).Cast<Attribute>().ToArray();
		}
		throw new InvalidOperationException();
	}

	public static string GetFieldPath<TType, TValue>(Expression<Func<TType, TValue>> expr)
	{
		ExpressionType nodeType = expr.Body.NodeType;
		if (nodeType == ExpressionType.MemberAccess)
		{
			MemberExpression memberExpression = expr.Body as MemberExpression;
			List<string> list = new List<string>();
			while (memberExpression != null)
			{
				list.Add(memberExpression.Member.Name);
				memberExpression = memberExpression.Expression as MemberExpression;
			}
			StringBuilder stringBuilder = new StringBuilder();
			for (int num = list.Count - 1; num >= 0; num--)
			{
				stringBuilder.Append(list[num]);
				if (num > 0)
				{
					stringBuilder.Append('.');
				}
			}
			return stringBuilder.ToString();
		}
		throw new InvalidOperationException();
	}
}
