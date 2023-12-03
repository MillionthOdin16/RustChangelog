using UnityEngine;

public class DevTimeAdjust : MonoBehaviour
{
	private void Start()
	{
		if (Object.op_Implicit((Object)(object)TOD_Sky.Instance))
		{
			TOD_Sky.Instance.Cycle.Hour = PlayerPrefs.GetFloat("DevTime");
		}
	}

	private void OnGUI()
	{
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		if (Object.op_Implicit((Object)(object)TOD_Sky.Instance))
		{
			float num = (float)Screen.width * 0.2f;
			Rect val = default(Rect);
			((Rect)(ref val))._002Ector((float)Screen.width - (num + 20f), (float)Screen.height - 30f, num, 20f);
			float hour = TOD_Sky.Instance.Cycle.Hour;
			hour = GUI.HorizontalSlider(val, hour, 0f, 24f);
			((Rect)(ref val)).y = ((Rect)(ref val)).y - 20f;
			GUI.Label(val, "Time Of Day");
			if (hour != TOD_Sky.Instance.Cycle.Hour)
			{
				TOD_Sky.Instance.Cycle.Hour = hour;
				PlayerPrefs.SetFloat("DevTime", hour);
			}
		}
	}
}
