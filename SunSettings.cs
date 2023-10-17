using ConVar;
using UnityEngine;

public class SunSettings : MonoBehaviour, IClientComponent
{
	private Light light;

	private void OnEnable()
	{
		light = ((Component)this).GetComponent<Light>();
	}

	private void Update()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		LightShadows val = (LightShadows)Mathf.Clamp(Graphics.shadowmode, 1, 2);
		if (light.shadows != val)
		{
			light.shadows = val;
		}
	}
}
