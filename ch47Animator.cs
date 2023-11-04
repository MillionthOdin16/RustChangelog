using UnityEngine;

public class ch47Animator : MonoBehaviour
{
	public Animator animator;

	public bool bottomDoorOpen;

	public bool landingGearDown;

	public bool leftDoorOpen;

	public bool rightDoorOpen;

	public bool rearDoorOpen;

	public bool rearDoorExtensionOpen;

	public Transform rearRotorBlade;

	public Transform frontRotorBlade;

	public float rotorBladeSpeed;

	public float wheelTurnSpeed;

	public float wheelTurnAngle;

	public SkinnedMeshRenderer[] blurredRotorBlades;

	public SkinnedMeshRenderer[] RotorBlades;

	private bool blurredRotorBladesEnabled;

	public float blurSpeedThreshold = 100f;

	private void Start()
	{
		EnableBlurredRotorBlades(enabled: false);
		animator.SetBool("rotorblade_stop", false);
	}

	public void SetDropDoorOpen(bool isOpen)
	{
		bottomDoorOpen = isOpen;
	}

	private void Update()
	{
		animator.SetBool("bottomdoor", bottomDoorOpen);
		animator.SetBool("landinggear", landingGearDown);
		animator.SetBool("leftdoor", leftDoorOpen);
		animator.SetBool("rightdoor", rightDoorOpen);
		animator.SetBool("reardoor", rearDoorOpen);
		animator.SetBool("reardoor_extension", rearDoorExtensionOpen);
		if (rotorBladeSpeed >= blurSpeedThreshold && !blurredRotorBladesEnabled)
		{
			EnableBlurredRotorBlades(enabled: true);
		}
		else if (rotorBladeSpeed < blurSpeedThreshold && blurredRotorBladesEnabled)
		{
			EnableBlurredRotorBlades(enabled: false);
		}
		if (rotorBladeSpeed <= 0f)
		{
			animator.SetBool("rotorblade_stop", true);
		}
		else
		{
			animator.SetBool("rotorblade_stop", false);
		}
	}

	private void LateUpdate()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		float num = Time.deltaTime * rotorBladeSpeed * 15f;
		Vector3 localEulerAngles = frontRotorBlade.localEulerAngles;
		frontRotorBlade.localEulerAngles = new Vector3(localEulerAngles.x, localEulerAngles.y + num, localEulerAngles.z);
		localEulerAngles = rearRotorBlade.localEulerAngles;
		rearRotorBlade.localEulerAngles = new Vector3(localEulerAngles.x, localEulerAngles.y - num, localEulerAngles.z);
	}

	private void EnableBlurredRotorBlades(bool enabled)
	{
		blurredRotorBladesEnabled = enabled;
		SkinnedMeshRenderer[] array = blurredRotorBlades;
		foreach (SkinnedMeshRenderer val in array)
		{
			((Renderer)val).enabled = enabled;
		}
		SkinnedMeshRenderer[] rotorBlades = RotorBlades;
		foreach (SkinnedMeshRenderer val2 in rotorBlades)
		{
			((Renderer)val2).enabled = !enabled;
		}
	}
}
