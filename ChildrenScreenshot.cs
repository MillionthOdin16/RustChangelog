using System;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEngine;

public class ChildrenScreenshot : MonoBehaviour
{
	public Vector3 offsetAngle = new Vector3(0f, 0f, 1f);

	public int width = 512;

	public int height = 512;

	public float fieldOfView = 70f;

	[Tooltip("0 = full recursive name, 1 = object name")]
	public string folder = "screenshots/{0}.png";

	[ContextMenu("Create Screenshots")]
	public void CreateScreenshots()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Expected O, but got Unknown
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Expected O, but got Unknown
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Expected O, but got Unknown
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		RenderTexture val = new RenderTexture(width, height, 0);
		GameObject val2 = new GameObject();
		Camera val3 = val2.AddComponent<Camera>();
		val3.targetTexture = val;
		val3.orthographic = false;
		val3.fieldOfView = fieldOfView;
		val3.nearClipPlane = 0.1f;
		val3.farClipPlane = 2000f;
		val3.cullingMask = LayerMask.GetMask(new string[1] { "TransparentFX" });
		val3.clearFlags = (CameraClearFlags)2;
		val3.backgroundColor = new Color(0f, 0f, 0f, 0f);
		val3.renderingPath = (RenderingPath)3;
		Texture2D val4 = new Texture2D(((Texture)val).width, ((Texture)val).height, (TextureFormat)5, false);
		foreach (Transform item in ((IEnumerable)((Component)this).transform).Cast<Transform>())
		{
			PositionCamera(val3, ((Component)item).gameObject);
			int layer = ((Component)item).gameObject.layer;
			((Component)item).gameObject.SetLayerRecursive(1);
			val3.Render();
			((Component)item).gameObject.SetLayerRecursive(layer);
			string recursiveName = item.GetRecursiveName();
			recursiveName = recursiveName.Replace('/', '.');
			RenderTexture.active = val;
			val4.ReadPixels(new Rect(0f, 0f, (float)((Texture)val).width, (float)((Texture)val).height), 0, 0, false);
			RenderTexture.active = null;
			byte[] bytes = ImageConversion.EncodeToPNG(val4);
			string path = string.Format(folder, recursiveName, ((Object)item).name);
			string directoryName = Path.GetDirectoryName(path);
			if (!Directory.Exists(directoryName))
			{
				Directory.CreateDirectory(directoryName);
			}
			File.WriteAllBytes(path, bytes);
		}
		Object.DestroyImmediate((Object)(object)val4, true);
		Object.DestroyImmediate((Object)(object)val, true);
		Object.DestroyImmediate((Object)(object)val2, true);
	}

	public void PositionCamera(Camera cam, GameObject obj)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		Bounds bounds = default(Bounds);
		((Bounds)(ref bounds))._002Ector(obj.transform.position, Vector3.zero * 0.1f);
		bool flag = true;
		Renderer[] componentsInChildren = obj.GetComponentsInChildren<Renderer>();
		foreach (Renderer val in componentsInChildren)
		{
			if (flag)
			{
				bounds = val.bounds;
				flag = false;
			}
			else
			{
				((Bounds)(ref bounds)).Encapsulate(val.bounds);
			}
		}
		Vector3 size = ((Bounds)(ref bounds)).size;
		float num = ((Vector3)(ref size)).magnitude * 0.5f / Mathf.Tan(cam.fieldOfView * 0.5f * ((float)Math.PI / 180f));
		((Component)cam).transform.position = ((Bounds)(ref bounds)).center + obj.transform.TransformVector(((Vector3)(ref offsetAngle)).normalized) * num;
		((Component)cam).transform.LookAt(((Bounds)(ref bounds)).center);
	}
}
