using UnityEngine;

[CreateAssetMenu(menuName = "Rust/Recoil Properties")]
public class RecoilProperties : ScriptableObject
{
	public float recoilYawMin = 0f;

	public float recoilYawMax = 0f;

	public float recoilPitchMin = 0f;

	public float recoilPitchMax = 0f;

	public float timeToTakeMin = 0f;

	public float timeToTakeMax = 0.1f;

	public float ADSScale = 0.5f;

	public float movementPenalty = 0f;

	public float clampPitch = float.NegativeInfinity;

	public AnimationCurve pitchCurve = new AnimationCurve((Keyframe[])(object)new Keyframe[2]
	{
		new Keyframe(0f, 1f),
		new Keyframe(1f, 1f)
	});

	public AnimationCurve yawCurve = new AnimationCurve((Keyframe[])(object)new Keyframe[2]
	{
		new Keyframe(0f, 1f),
		new Keyframe(1f, 1f)
	});

	public bool useCurves = false;

	public bool curvesAsScalar = false;

	public int shotsUntilMax = 30;

	public float maxRecoilRadius = 5f;

	[Header("AimCone")]
	public bool overrideAimconeWithCurve = false;

	public float aimconeCurveScale = 1f;

	[Tooltip("How much to scale aimcone by based on how far into the shot sequence we are (shots v shotsUntilMax)")]
	public AnimationCurve aimconeCurve = new AnimationCurve((Keyframe[])(object)new Keyframe[2]
	{
		new Keyframe(0f, 1f),
		new Keyframe(1f, 1f)
	});

	[Tooltip("Randomly select how much to scale final aimcone by per shot, you can use this to weigh a fraction of shots closer to the center")]
	public AnimationCurve aimconeProbabilityCurve = new AnimationCurve((Keyframe[])(object)new Keyframe[3]
	{
		new Keyframe(0f, 1f),
		new Keyframe(0.5f, 0f),
		new Keyframe(1f, 1f)
	});

	public RecoilProperties newRecoilOverride;

	public RecoilProperties GetRecoil()
	{
		return ((Object)(object)newRecoilOverride != (Object)null) ? newRecoilOverride : this;
	}
}
