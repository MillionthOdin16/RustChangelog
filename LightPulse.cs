using UnityEngine;

public class LightPulse : MonoBehaviour, IClientComponent
{
	public Light TargetLight = null;

	public float PulseSpeed = 1f;

	public float Lifetime = 3f;

	public float MaxIntensity = 3f;

	public float FadeOutSpeed = 2f;
}
