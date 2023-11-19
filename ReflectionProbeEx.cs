using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode]
public class ReflectionProbeEx : MonoBehaviour
{
	private struct CubemapSkyboxVertex
	{
		public float x;

		public float y;

		public float z;

		public Color color;

		public float tu;

		public float tv;

		public float tw;
	}

	private struct CubemapFaceMatrices
	{
		public Matrix4x4 worldToView;

		public Matrix4x4 viewToWorld;

		public CubemapFaceMatrices(Vector3 x, Vector3 y, Vector3 z)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			worldToView = Matrix4x4.identity;
			((Matrix4x4)(ref worldToView))[0, 0] = ((Vector3)(ref x))[0];
			((Matrix4x4)(ref worldToView))[0, 1] = ((Vector3)(ref x))[1];
			((Matrix4x4)(ref worldToView))[0, 2] = ((Vector3)(ref x))[2];
			((Matrix4x4)(ref worldToView))[1, 0] = ((Vector3)(ref y))[0];
			((Matrix4x4)(ref worldToView))[1, 1] = ((Vector3)(ref y))[1];
			((Matrix4x4)(ref worldToView))[1, 2] = ((Vector3)(ref y))[2];
			((Matrix4x4)(ref worldToView))[2, 0] = ((Vector3)(ref z))[0];
			((Matrix4x4)(ref worldToView))[2, 1] = ((Vector3)(ref z))[1];
			((Matrix4x4)(ref worldToView))[2, 2] = ((Vector3)(ref z))[2];
			viewToWorld = ((Matrix4x4)(ref worldToView)).inverse;
		}
	}

	[Serializable]
	public enum ConvolutionQuality
	{
		Lowest,
		Low,
		Medium,
		High,
		VeryHigh
	}

	[Serializable]
	public struct RenderListEntry
	{
		public Renderer renderer;

		public bool alwaysEnabled;

		public RenderListEntry(Renderer renderer, bool alwaysEnabled)
		{
			this.renderer = renderer;
			this.alwaysEnabled = alwaysEnabled;
		}
	}

	private Mesh blitMesh = null;

	private Mesh skyboxMesh = null;

	private static float[] octaVerts = new float[72]
	{
		0f, 1f, 0f, 0f, 0f, -1f, 1f, 0f, 0f, 0f,
		1f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, 0f, 1f,
		0f, 0f, 0f, 1f, -1f, 0f, 0f, 0f, 1f, 0f,
		-1f, 0f, 0f, 0f, 0f, -1f, 0f, -1f, 0f, 1f,
		0f, 0f, 0f, 0f, -1f, 0f, -1f, 0f, 0f, 0f,
		1f, 1f, 0f, 0f, 0f, -1f, 0f, -1f, 0f, 0f,
		0f, 0f, 1f, 0f, -1f, 0f, 0f, 0f, -1f, -1f,
		0f, 0f
	};

	private static readonly CubemapFaceMatrices[] cubemapFaceMatrices = new CubemapFaceMatrices[6]
	{
		new CubemapFaceMatrices(new Vector3(0f, 0f, -1f), new Vector3(0f, -1f, 0f), new Vector3(-1f, 0f, 0f)),
		new CubemapFaceMatrices(new Vector3(0f, 0f, 1f), new Vector3(0f, -1f, 0f), new Vector3(1f, 0f, 0f)),
		new CubemapFaceMatrices(new Vector3(1f, 0f, 0f), new Vector3(0f, 0f, 1f), new Vector3(0f, -1f, 0f)),
		new CubemapFaceMatrices(new Vector3(1f, 0f, 0f), new Vector3(0f, 0f, -1f), new Vector3(0f, 1f, 0f)),
		new CubemapFaceMatrices(new Vector3(1f, 0f, 0f), new Vector3(0f, -1f, 0f), new Vector3(0f, 0f, -1f)),
		new CubemapFaceMatrices(new Vector3(-1f, 0f, 0f), new Vector3(0f, -1f, 0f), new Vector3(0f, 0f, 1f))
	};

	private static readonly CubemapFaceMatrices[] cubemapFaceMatricesD3D11 = new CubemapFaceMatrices[6]
	{
		new CubemapFaceMatrices(new Vector3(0f, 0f, -1f), new Vector3(0f, 1f, 0f), new Vector3(-1f, 0f, 0f)),
		new CubemapFaceMatrices(new Vector3(0f, 0f, 1f), new Vector3(0f, 1f, 0f), new Vector3(1f, 0f, 0f)),
		new CubemapFaceMatrices(new Vector3(1f, 0f, 0f), new Vector3(0f, 0f, -1f), new Vector3(0f, -1f, 0f)),
		new CubemapFaceMatrices(new Vector3(1f, 0f, 0f), new Vector3(0f, 0f, 1f), new Vector3(0f, 1f, 0f)),
		new CubemapFaceMatrices(new Vector3(1f, 0f, 0f), new Vector3(0f, 1f, 0f), new Vector3(0f, 0f, -1f)),
		new CubemapFaceMatrices(new Vector3(-1f, 0f, 0f), new Vector3(0f, 1f, 0f), new Vector3(0f, 0f, 1f))
	};

	private static readonly CubemapFaceMatrices[] shadowCubemapFaceMatrices = new CubemapFaceMatrices[6]
	{
		new CubemapFaceMatrices(new Vector3(0f, 0f, 1f), new Vector3(0f, -1f, 0f), new Vector3(-1f, 0f, 0f)),
		new CubemapFaceMatrices(new Vector3(0f, 0f, -1f), new Vector3(0f, -1f, 0f), new Vector3(1f, 0f, 0f)),
		new CubemapFaceMatrices(new Vector3(1f, 0f, 0f), new Vector3(0f, 0f, 1f), new Vector3(0f, 1f, 0f)),
		new CubemapFaceMatrices(new Vector3(1f, 0f, 0f), new Vector3(0f, 0f, -1f), new Vector3(0f, -1f, 0f)),
		new CubemapFaceMatrices(new Vector3(1f, 0f, 0f), new Vector3(0f, -1f, 0f), new Vector3(0f, 0f, 1f)),
		new CubemapFaceMatrices(new Vector3(-1f, 0f, 0f), new Vector3(0f, -1f, 0f), new Vector3(0f, 0f, -1f))
	};

	private CubemapFaceMatrices[] platformCubemapFaceMatrices = null;

	private static readonly int[] tab32 = new int[32]
	{
		0, 9, 1, 10, 13, 21, 2, 29, 11, 14,
		16, 18, 22, 25, 3, 30, 8, 12, 20, 28,
		15, 17, 24, 7, 19, 27, 23, 6, 26, 5,
		4, 31
	};

	public ReflectionProbeRefreshMode refreshMode = (ReflectionProbeRefreshMode)1;

	public bool timeSlicing = false;

	public int resolution = 128;

	[InspectorName("HDR")]
	public bool hdr = true;

	public float shadowDistance = 0f;

	public ReflectionProbeClearFlags clearFlags = (ReflectionProbeClearFlags)1;

	public Color background = new Color(0.192f, 0.301f, 0.474f);

	public float nearClip = 0.3f;

	public float farClip = 1000f;

	public Transform attachToTarget = null;

	public Light directionalLight = null;

	public float textureMipBias = 2f;

	public bool highPrecision = false;

	public bool enableShadows = false;

	public ConvolutionQuality convolutionQuality = ConvolutionQuality.Lowest;

	public List<RenderListEntry> staticRenderList = new List<RenderListEntry>();

	public Cubemap reflectionCubemap = null;

	public float reflectionIntensity = 1f;

	private void CreateMeshes()
	{
		if ((Object)(object)blitMesh == (Object)null)
		{
			blitMesh = CreateBlitMesh();
		}
		if ((Object)(object)skyboxMesh == (Object)null)
		{
			skyboxMesh = CreateSkyboxMesh();
		}
	}

	private void DestroyMeshes()
	{
		if ((Object)(object)blitMesh != (Object)null)
		{
			Object.DestroyImmediate((Object)(object)blitMesh);
			blitMesh = null;
		}
		if ((Object)(object)skyboxMesh != (Object)null)
		{
			Object.DestroyImmediate((Object)(object)skyboxMesh);
			skyboxMesh = null;
		}
	}

	private static Mesh CreateBlitMesh()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		Mesh val = new Mesh();
		val.vertices = (Vector3[])(object)new Vector3[4]
		{
			new Vector3(-1f, -1f, 0f),
			new Vector3(-1f, 1f, 0f),
			new Vector3(1f, 1f, 0f),
			new Vector3(1f, -1f, 0f)
		};
		val.uv = (Vector2[])(object)new Vector2[4]
		{
			new Vector2(0f, 0f),
			new Vector2(0f, 1f),
			new Vector2(1f, 1f),
			new Vector2(1f, 0f)
		};
		val.triangles = new int[6] { 0, 1, 2, 0, 2, 3 };
		return val;
	}

	private static CubemapSkyboxVertex SubDivVert(CubemapSkyboxVertex v1, CubemapSkyboxVertex v2)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = default(Vector3);
		((Vector3)(ref val))._002Ector(v1.x, v1.y, v1.z);
		Vector3 val2 = default(Vector3);
		((Vector3)(ref val2))._002Ector(v2.x, v2.y, v2.z);
		Vector3 val3 = Vector3.Normalize(Vector3.Lerp(val, val2, 0.5f));
		CubemapSkyboxVertex result = default(CubemapSkyboxVertex);
		result.x = (result.tu = val3.x);
		result.y = (result.tv = val3.y);
		result.z = (result.tw = val3.z);
		result.color = Color.white;
		return result;
	}

	private static void Subdivide(List<CubemapSkyboxVertex> destArray, CubemapSkyboxVertex v1, CubemapSkyboxVertex v2, CubemapSkyboxVertex v3)
	{
		CubemapSkyboxVertex item = SubDivVert(v1, v2);
		CubemapSkyboxVertex item2 = SubDivVert(v2, v3);
		CubemapSkyboxVertex item3 = SubDivVert(v1, v3);
		destArray.Add(v1);
		destArray.Add(item);
		destArray.Add(item3);
		destArray.Add(item);
		destArray.Add(v2);
		destArray.Add(item2);
		destArray.Add(item2);
		destArray.Add(item3);
		destArray.Add(item);
		destArray.Add(v3);
		destArray.Add(item3);
		destArray.Add(item2);
	}

	private static void SubdivideYOnly(List<CubemapSkyboxVertex> destArray, CubemapSkyboxVertex v1, CubemapSkyboxVertex v2, CubemapSkyboxVertex v3)
	{
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		float num = Mathf.Abs(v2.y - v1.y);
		float num2 = Mathf.Abs(v2.y - v3.y);
		float num3 = Mathf.Abs(v3.y - v1.y);
		CubemapSkyboxVertex cubemapSkyboxVertex;
		CubemapSkyboxVertex cubemapSkyboxVertex2;
		CubemapSkyboxVertex cubemapSkyboxVertex3;
		if (num < num2 && num < num3)
		{
			cubemapSkyboxVertex = v3;
			cubemapSkyboxVertex2 = v1;
			cubemapSkyboxVertex3 = v2;
		}
		else if (num2 < num && num2 < num3)
		{
			cubemapSkyboxVertex = v1;
			cubemapSkyboxVertex2 = v2;
			cubemapSkyboxVertex3 = v3;
		}
		else
		{
			cubemapSkyboxVertex = v2;
			cubemapSkyboxVertex2 = v3;
			cubemapSkyboxVertex3 = v1;
		}
		CubemapSkyboxVertex item = SubDivVert(cubemapSkyboxVertex, cubemapSkyboxVertex2);
		CubemapSkyboxVertex item2 = SubDivVert(cubemapSkyboxVertex, cubemapSkyboxVertex3);
		destArray.Add(cubemapSkyboxVertex);
		destArray.Add(item);
		destArray.Add(item2);
		Vector3 val = default(Vector3);
		((Vector3)(ref val))._002Ector(item2.x - cubemapSkyboxVertex2.x, item2.y - cubemapSkyboxVertex2.y, item2.z - cubemapSkyboxVertex2.z);
		Vector3 val2 = default(Vector3);
		((Vector3)(ref val2))._002Ector(item.x - cubemapSkyboxVertex3.x, item.y - cubemapSkyboxVertex3.y, item.z - cubemapSkyboxVertex3.z);
		if (val.x * val.x + val.y * val.y + val.z * val.z > val2.x * val2.x + val2.y * val2.y + val2.z * val2.z)
		{
			destArray.Add(item);
			destArray.Add(cubemapSkyboxVertex2);
			destArray.Add(cubemapSkyboxVertex3);
			destArray.Add(item2);
			destArray.Add(item);
			destArray.Add(cubemapSkyboxVertex3);
		}
		else
		{
			destArray.Add(item2);
			destArray.Add(item);
			destArray.Add(cubemapSkyboxVertex2);
			destArray.Add(item2);
			destArray.Add(cubemapSkyboxVertex2);
			destArray.Add(cubemapSkyboxVertex3);
		}
	}

	private static Mesh CreateSkyboxMesh()
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Expected O, but got Unknown
		//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0304: Unknown result type (might be due to invalid IL or missing references)
		//IL_0309: Unknown result type (might be due to invalid IL or missing references)
		//IL_030e: Unknown result type (might be due to invalid IL or missing references)
		List<CubemapSkyboxVertex> list = new List<CubemapSkyboxVertex>();
		for (int i = 0; i < 24; i++)
		{
			CubemapSkyboxVertex item = default(CubemapSkyboxVertex);
			Vector3 val = Vector3.Normalize(new Vector3(octaVerts[i * 3], octaVerts[i * 3 + 1], octaVerts[i * 3 + 2]));
			item.x = (item.tu = val.x);
			item.y = (item.tv = val.y);
			item.z = (item.tw = val.z);
			item.color = Color.white;
			list.Add(item);
		}
		for (int j = 0; j < 3; j++)
		{
			List<CubemapSkyboxVertex> list2 = new List<CubemapSkyboxVertex>(list.Count);
			list2.AddRange(list);
			int count = list2.Count;
			list.Clear();
			list.Capacity = count * 4;
			for (int k = 0; k < count; k += 3)
			{
				Subdivide(list, list2[k], list2[k + 1], list2[k + 2]);
			}
		}
		for (int l = 0; l < 2; l++)
		{
			List<CubemapSkyboxVertex> list3 = new List<CubemapSkyboxVertex>(list.Count);
			list3.AddRange(list);
			int count2 = list3.Count;
			float num = Mathf.Pow(0.5f, (float)l + 1f);
			list.Clear();
			list.Capacity = count2 * 4;
			for (int m = 0; m < count2; m += 3)
			{
				float num2 = Mathf.Max(Mathf.Max(Mathf.Abs(list3[m].y), Mathf.Abs(list3[m + 1].y)), Mathf.Abs(list3[m + 2].y));
				if (num2 > num)
				{
					list.Add(list3[m]);
					list.Add(list3[m + 1]);
					list.Add(list3[m + 2]);
				}
				else
				{
					SubdivideYOnly(list, list3[m], list3[m + 1], list3[m + 2]);
				}
			}
		}
		Mesh val2 = new Mesh();
		Vector3[] array = (Vector3[])(object)new Vector3[list.Count];
		Vector2[] array2 = (Vector2[])(object)new Vector2[list.Count];
		int[] array3 = new int[list.Count];
		for (int n = 0; n < list.Count; n++)
		{
			array[n] = new Vector3(list[n].x, list[n].y, list[n].z);
			array2[n] = Vector2.op_Implicit(new Vector3(list[n].tu, list[n].tv));
			array3[n] = n;
		}
		val2.vertices = array;
		val2.uv = array2;
		val2.triangles = array3;
		return val2;
	}

	private bool InitializeCubemapFaceMatrices()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Invalid comparison between Unknown and I4
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Expected I4, but got Unknown
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		GraphicsDeviceType graphicsDeviceType = SystemInfo.graphicsDeviceType;
		if ((int)graphicsDeviceType != 2)
		{
			switch (graphicsDeviceType - 16)
			{
			case 1:
				platformCubemapFaceMatrices = cubemapFaceMatrices;
				break;
			case 2:
				platformCubemapFaceMatrices = cubemapFaceMatricesD3D11;
				break;
			case 5:
				platformCubemapFaceMatrices = cubemapFaceMatricesD3D11;
				break;
			case 0:
				platformCubemapFaceMatrices = cubemapFaceMatricesD3D11;
				break;
			default:
				platformCubemapFaceMatrices = null;
				break;
			}
		}
		else
		{
			platformCubemapFaceMatrices = cubemapFaceMatricesD3D11;
		}
		if (platformCubemapFaceMatrices == null)
		{
			Debug.LogError((object)("[ReflectionProbeEx] Initialization failed. No cubemap ortho basis defined for " + SystemInfo.graphicsDeviceType));
			return false;
		}
		return true;
	}

	private int FastLog2(int value)
	{
		value |= value >> 1;
		value |= value >> 2;
		value |= value >> 4;
		value |= value >> 8;
		value |= value >> 16;
		return tab32[(uint)((long)value * 130329821L) >> 27];
	}

	private uint ReverseBits(uint bits)
	{
		bits = (bits << 16) | (bits >> 16);
		bits = ((bits & 0xFF00FF) << 8) | ((bits & 0xFF00FF00u) >> 8);
		bits = ((bits & 0xF0F0F0F) << 4) | ((bits & 0xF0F0F0F0u) >> 4);
		bits = ((bits & 0x33333333) << 2) | ((bits & 0xCCCCCCCCu) >> 2);
		bits = ((bits & 0x55555555) << 1) | ((bits & 0xAAAAAAAAu) >> 1);
		return bits;
	}

	private void SafeCreateMaterial(ref Material mat, Shader shader)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		if ((Object)(object)mat == (Object)null)
		{
			mat = new Material(shader);
		}
	}

	private void SafeCreateMaterial(ref Material mat, string shaderName)
	{
		if ((Object)(object)mat == (Object)null)
		{
			SafeCreateMaterial(ref mat, Shader.Find(shaderName));
		}
	}

	private void SafeCreateCubeRT(ref RenderTexture rt, string name, int size, int depth, bool mips, TextureDimension dim, FilterMode filter, RenderTextureFormat format, RenderTextureReadWrite readWrite = 1)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Expected O, but got Unknown
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Invalid comparison between Unknown and I4
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)rt == (Object)null || !rt.IsCreated())
		{
			SafeDestroy<RenderTexture>(ref rt);
			rt = new RenderTexture(size, size, depth, format, readWrite)
			{
				hideFlags = (HideFlags)52
			};
			((Object)rt).name = name;
			((Texture)rt).dimension = dim;
			if ((int)dim == 5)
			{
				rt.volumeDepth = 6;
			}
			rt.useMipMap = mips;
			rt.autoGenerateMips = false;
			((Texture)rt).filterMode = filter;
			((Texture)rt).anisoLevel = 0;
			rt.Create();
		}
	}

	private void SafeCreateCB(ref CommandBuffer cb, string name)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Expected O, but got Unknown
		if (cb == null)
		{
			cb = new CommandBuffer();
			cb.name = name;
		}
	}

	private void SafeDestroy<T>(ref T obj) where T : Object
	{
		if ((Object)(object)obj != (Object)null)
		{
			Object.DestroyImmediate((Object)(object)obj);
			obj = default(T);
		}
	}

	private void SafeDispose<T>(ref T obj) where T : IDisposable
	{
		if (obj != null)
		{
			obj.Dispose();
			obj = default(T);
		}
	}
}
