using UnityEngine;

public class AmbientLightLOD : FacepunchBehaviour, ILOD, IClientComponent
{
	public bool isDynamic = false;

	public float enabledRadius = 20f;

	public bool toggleFade = false;

	public float toggleFadeDuration = 0.5f;

	protected void OnValidate()
	{
		LightEx.CheckConflict(((Component)this).gameObject);
	}
}
