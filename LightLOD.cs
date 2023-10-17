using UnityEngine;

public class LightLOD : MonoBehaviour, ILOD, IClientComponent
{
	public float DistanceBias = 0f;

	public bool ToggleLight = false;

	public bool ToggleShadows = true;

	protected void OnValidate()
	{
		LightEx.CheckConflict(((Component)this).gameObject);
	}
}
