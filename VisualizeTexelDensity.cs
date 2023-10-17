using System;
using UnityEngine;

[ExecuteInEditMode]
[ImageEffectAllowedInSceneView]
[AddComponentMenu("Rendering/Visualize Texture Density")]
public class VisualizeTexelDensity : MonoBehaviour
{
	public Shader shader;

	public string shaderTag = "RenderType";

	[Range(1f, 1024f)]
	public int texelsPerMeter = 256;

	[Range(0f, 1f)]
	public float overlayOpacity = 0.5f;

	public bool showHUD = true;

	private Camera mainCamera;

	private bool initialized;

	private int screenWidth;

	private int screenHeight;

	private Camera texelDensityCamera;

	private RenderTexture texelDensityRT;

	private Texture texelDensityGradTex;

	private Material texelDensityOverlayMat;

	private static VisualizeTexelDensity instance;

	public static VisualizeTexelDensity Instance => instance;

	private void Awake()
	{
		instance = this;
		mainCamera = ((Component)this).GetComponent<Camera>();
	}

	private void OnEnable()
	{
		mainCamera = ((Component)this).GetComponent<Camera>();
		screenWidth = Screen.width;
		screenHeight = Screen.height;
		LoadResources();
		initialized = true;
	}

	private void OnDisable()
	{
		SafeDestroyViewTexelDensity();
		SafeDestroyViewTexelDensityRT();
		initialized = false;
	}

	private void LoadResources()
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Expected O, but got Unknown
		if ((Object)(object)texelDensityGradTex == (Object)null)
		{
			ref Texture reference = ref texelDensityGradTex;
			Object obj = Resources.Load("TexelDensityGrad");
			reference = (Texture)(object)((obj is Texture) ? obj : null);
		}
		if ((Object)(object)texelDensityOverlayMat == (Object)null)
		{
			texelDensityOverlayMat = new Material(Shader.Find("Hidden/TexelDensityOverlay"))
			{
				hideFlags = (HideFlags)52
			};
		}
	}

	private void SafeDestroyViewTexelDensity()
	{
		if ((Object)(object)texelDensityCamera != (Object)null)
		{
			Object.DestroyImmediate((Object)(object)((Component)texelDensityCamera).gameObject);
			texelDensityCamera = null;
		}
		if ((Object)(object)texelDensityGradTex != (Object)null)
		{
			Resources.UnloadAsset((Object)(object)texelDensityGradTex);
			texelDensityGradTex = null;
		}
		if ((Object)(object)texelDensityOverlayMat != (Object)null)
		{
			Object.DestroyImmediate((Object)(object)texelDensityOverlayMat);
			texelDensityOverlayMat = null;
		}
	}

	private void SafeDestroyViewTexelDensityRT()
	{
		if ((Object)(object)texelDensityRT != (Object)null)
		{
			Graphics.SetRenderTarget((RenderTexture)null);
			texelDensityRT.Release();
			Object.DestroyImmediate((Object)(object)texelDensityRT);
			texelDensityRT = null;
		}
	}

	private void UpdateViewTexelDensity(bool screenResized)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Expected O, but got Unknown
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Expected O, but got Unknown
		if ((Object)(object)texelDensityCamera == (Object)null)
		{
			GameObject val = new GameObject("Texel Density Camera", new Type[1] { typeof(Camera) })
			{
				hideFlags = (HideFlags)61
			};
			val.transform.parent = ((Component)mainCamera).transform;
			val.transform.localPosition = Vector3.zero;
			val.transform.localRotation = Quaternion.identity;
			texelDensityCamera = val.GetComponent<Camera>();
			texelDensityCamera.CopyFrom(mainCamera);
			texelDensityCamera.renderingPath = (RenderingPath)1;
			texelDensityCamera.allowMSAA = false;
			texelDensityCamera.allowHDR = false;
			texelDensityCamera.clearFlags = (CameraClearFlags)1;
			texelDensityCamera.depthTextureMode = (DepthTextureMode)0;
			texelDensityCamera.SetReplacementShader(shader, shaderTag);
			((Behaviour)texelDensityCamera).enabled = false;
		}
		if ((Object)(object)texelDensityRT == (Object)null || screenResized || !texelDensityRT.IsCreated())
		{
			texelDensityCamera.targetTexture = null;
			SafeDestroyViewTexelDensityRT();
			texelDensityRT = new RenderTexture(screenWidth, screenHeight, 24, (RenderTextureFormat)0)
			{
				hideFlags = (HideFlags)52
			};
			((Object)texelDensityRT).name = "TexelDensityRT";
			((Texture)texelDensityRT).filterMode = (FilterMode)0;
			((Texture)texelDensityRT).wrapMode = (TextureWrapMode)1;
			texelDensityRT.Create();
		}
		if ((Object)(object)texelDensityCamera.targetTexture != (Object)(object)texelDensityRT)
		{
			texelDensityCamera.targetTexture = texelDensityRT;
		}
		Shader.SetGlobalFloat("global_TexelsPerMeter", (float)texelsPerMeter);
		Shader.SetGlobalTexture("global_TexelDensityGrad", texelDensityGradTex);
		texelDensityCamera.fieldOfView = mainCamera.fieldOfView;
		texelDensityCamera.nearClipPlane = mainCamera.nearClipPlane;
		texelDensityCamera.farClipPlane = mainCamera.farClipPlane;
		texelDensityCamera.cullingMask = mainCamera.cullingMask;
	}

	private bool CheckScreenResized(int width, int height)
	{
		if (screenWidth != width || screenHeight != height)
		{
			screenWidth = width;
			screenHeight = height;
			return true;
		}
		return false;
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (initialized)
		{
			UpdateViewTexelDensity(CheckScreenResized(((Texture)source).width, ((Texture)source).height));
			texelDensityCamera.Render();
			texelDensityOverlayMat.SetTexture("_TexelDensityMap", (Texture)(object)texelDensityRT);
			texelDensityOverlayMat.SetFloat("_Opacity", overlayOpacity);
			Graphics.Blit((Texture)(object)source, destination, texelDensityOverlayMat, 0);
		}
		else
		{
			Graphics.Blit((Texture)(object)source, destination);
		}
	}

	private void DrawGUIText(float x, float y, Vector2 size, string text, GUIStyle fontStyle)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		fontStyle.normal.textColor = Color.black;
		GUI.Label(new Rect(x - 1f, y + 1f, size.x, size.y), text, fontStyle);
		GUI.Label(new Rect(x + 1f, y - 1f, size.x, size.y), text, fontStyle);
		GUI.Label(new Rect(x + 1f, y + 1f, size.x, size.y), text, fontStyle);
		GUI.Label(new Rect(x - 1f, y - 1f, size.x, size.y), text, fontStyle);
		fontStyle.normal.textColor = Color.white;
		GUI.Label(new Rect(x, y, size.x, size.y), text, fontStyle);
	}

	private void OnGUI()
	{
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Expected O, but got Unknown
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Expected O, but got Unknown
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Expected O, but got Unknown
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Expected O, but got Unknown
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Expected O, but got Unknown
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		if (initialized && showHUD)
		{
			string text = "Texels Per Meter";
			string text2 = "0";
			string text3 = texelsPerMeter.ToString();
			string text4 = (texelsPerMeter << 1) + "+";
			float num = texelDensityGradTex.width;
			float num2 = texelDensityGradTex.height * 2;
			float num3 = (Screen.width - texelDensityGradTex.width) / 2;
			float num4 = 32f;
			GL.PushMatrix();
			GL.LoadPixelMatrix(0f, (float)Screen.width, (float)Screen.height, 0f);
			Graphics.DrawTexture(new Rect(num3 - 2f, num4 - 2f, num + 4f, num2 + 4f), (Texture)(object)Texture2D.whiteTexture);
			Graphics.DrawTexture(new Rect(num3, num4, num, num2), texelDensityGradTex);
			GL.PopMatrix();
			GUIStyle val = new GUIStyle();
			val.fontSize = 13;
			Vector2 val2 = val.CalcSize(new GUIContent(text));
			Vector2 size = val.CalcSize(new GUIContent(text2));
			Vector2 val3 = val.CalcSize(new GUIContent(text3));
			Vector2 val4 = val.CalcSize(new GUIContent(text4));
			DrawGUIText(((float)Screen.width - val2.x) / 2f, num4 - val2.y - 5f, val2, text, val);
			DrawGUIText(num3, num4 + num2 + 6f, size, text2, val);
			DrawGUIText(((float)Screen.width - val3.x) / 2f, num4 + num2 + 6f, val3, text3, val);
			DrawGUIText(num3 + num - val4.x, num4 + num2 + 6f, val4, text4, val);
		}
	}
}
