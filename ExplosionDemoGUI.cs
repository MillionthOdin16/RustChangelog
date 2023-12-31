using UnityEngine;

public class ExplosionDemoGUI : MonoBehaviour
{
	public GameObject[] Prefabs;

	public float reactivateTime = 4f;

	public Light Sun;

	private int currentNomber;

	private GameObject currentInstance;

	private GUIStyle guiStyleHeader = new GUIStyle();

	private float sunIntensity;

	private float dpiScale;

	private void Start()
	{
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		if (Screen.dpi < 1f)
		{
			dpiScale = 1f;
		}
		if (Screen.dpi < 200f)
		{
			dpiScale = 1f;
		}
		else
		{
			dpiScale = Screen.dpi / 200f;
		}
		guiStyleHeader.fontSize = (int)(15f * dpiScale);
		guiStyleHeader.normal.textColor = new Color(0.15f, 0.15f, 0.15f);
		currentInstance = Object.Instantiate<GameObject>(Prefabs[currentNomber], ((Component)this).transform.position, default(Quaternion));
		currentInstance.AddComponent<ExplosionDemoReactivator>().TimeDelayToReactivate = reactivateTime;
		sunIntensity = Sun.intensity;
	}

	private void OnGUI()
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		if (GUI.Button(new Rect(10f * dpiScale, 15f * dpiScale, 135f * dpiScale, 37f * dpiScale), "PREVIOUS EFFECT"))
		{
			ChangeCurrent(-1);
		}
		if (GUI.Button(new Rect(160f * dpiScale, 15f * dpiScale, 135f * dpiScale, 37f * dpiScale), "NEXT EFFECT"))
		{
			ChangeCurrent(1);
		}
		sunIntensity = GUI.HorizontalSlider(new Rect(10f * dpiScale, 70f * dpiScale, 285f * dpiScale, 15f * dpiScale), sunIntensity, 0f, 0.6f);
		Sun.intensity = sunIntensity;
		GUI.Label(new Rect(300f * dpiScale, 70f * dpiScale, 30f * dpiScale, 30f * dpiScale), "SUN INTENSITY", guiStyleHeader);
		GUI.Label(new Rect(400f * dpiScale, 15f * dpiScale, 100f * dpiScale, 20f * dpiScale), "Prefab name is \"" + ((Object)Prefabs[currentNomber]).name + "\"  \r\nHold any mouse button that would move the camera", guiStyleHeader);
	}

	private void ChangeCurrent(int delta)
	{
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		currentNomber += delta;
		if (currentNomber > Prefabs.Length - 1)
		{
			currentNomber = 0;
		}
		else if (currentNomber < 0)
		{
			currentNomber = Prefabs.Length - 1;
		}
		if ((Object)(object)currentInstance != (Object)null)
		{
			Object.Destroy((Object)(object)currentInstance);
		}
		currentInstance = Object.Instantiate<GameObject>(Prefabs[currentNomber], ((Component)this).transform.position, default(Quaternion));
		currentInstance.AddComponent<ExplosionDemoReactivator>().TimeDelayToReactivate = reactivateTime;
	}
}
