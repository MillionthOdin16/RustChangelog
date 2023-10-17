using UnityEngine;

public class LightEx : UpdateBehaviour, IClientComponent
{
	public bool alterColor;

	public float colorTimeScale = 1f;

	public Color colorA = Color.red;

	public Color colorB = Color.yellow;

	public AnimationCurve blendCurve = new AnimationCurve();

	public bool loopColor = true;

	public bool alterIntensity;

	public float intensityTimeScale = 1f;

	public AnimationCurve intenseCurve = new AnimationCurve();

	public float intensityCurveScale = 3f;

	public bool loopIntensity = true;

	public bool randomOffset;

	public float randomIntensityStartScale = -1f;

	protected void OnValidate()
	{
		CheckConflict(((Component)this).gameObject);
	}

	public static bool CheckConflict(GameObject go)
	{
		return false;
	}
}
