using UnityEngine;

public class BucketVMFluidSim : MonoBehaviour
{
	public Animator waterbucketAnim;

	public ParticleSystem waterPour;

	public ParticleSystem waterTurbulence;

	public ParticleSystem waterFill;

	public float waterLevel = 0f;

	public float targetWaterLevel = 0f;

	public AudioSource waterSpill;

	private float PlayerEyePitch;

	private float turb_forward;

	private float turb_side;

	private Vector3 lastPosition;

	protected Vector3 groundSpeedLast;

	private Vector3 lastAngle;

	protected Vector3 vecAngleSpeedLast;

	private Vector3 initialPosition;
}
