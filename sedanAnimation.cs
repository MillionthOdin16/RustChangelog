using UnityEngine;

public class sedanAnimation : MonoBehaviour
{
	public Transform[] frontAxles;

	public Transform FL_shock;

	public Transform FL_wheel;

	public Transform FR_shock;

	public Transform FR_wheel;

	public Transform RL_shock;

	public Transform RL_wheel;

	public Transform RR_shock;

	public Transform RR_wheel;

	public WheelCollider FL_wheelCollider;

	public WheelCollider FR_wheelCollider;

	public WheelCollider RL_wheelCollider;

	public WheelCollider RR_wheelCollider;

	public Transform steeringWheel;

	public float motorForceConstant = 150f;

	public float brakeForceConstant = 500f;

	public float brakePedal = 0f;

	public float gasPedal = 0f;

	public float steering = 0f;

	private Rigidbody myRigidbody;

	public float GasLerpTime = 20f;

	public float SteeringLerpTime = 20f;

	private float wheelSpinConstant = 120f;

	private float shockRestingPosY = -0.27f;

	private float shockDistance = 0.3f;

	private float traceDistanceNeutralPoint = 0.7f;

	private void Start()
	{
		myRigidbody = ((Component)this).GetComponent<Rigidbody>();
	}

	private void Update()
	{
		DoSteering();
		ApplyForceAtWheels();
		UpdateTireAnimation();
		InputPlayer();
	}

	private void InputPlayer()
	{
		if (Input.GetKey((KeyCode)119))
		{
			gasPedal = Mathf.Clamp(gasPedal + Time.deltaTime * GasLerpTime, -100f, 100f);
			brakePedal = Mathf.Lerp(brakePedal, 0f, Time.deltaTime * GasLerpTime);
		}
		else if (Input.GetKey((KeyCode)115))
		{
			gasPedal = Mathf.Clamp(gasPedal - Time.deltaTime * GasLerpTime, -100f, 100f);
			brakePedal = Mathf.Lerp(brakePedal, 0f, Time.deltaTime * GasLerpTime);
		}
		else
		{
			gasPedal = Mathf.Lerp(gasPedal, 0f, Time.deltaTime * GasLerpTime);
			brakePedal = Mathf.Lerp(brakePedal, 100f, Time.deltaTime * GasLerpTime / 5f);
		}
		if (Input.GetKey((KeyCode)97))
		{
			steering = Mathf.Clamp(steering - Time.deltaTime * SteeringLerpTime, -60f, 60f);
		}
		else if (Input.GetKey((KeyCode)100))
		{
			steering = Mathf.Clamp(steering + Time.deltaTime * SteeringLerpTime, -60f, 60f);
		}
		else
		{
			steering = Mathf.Lerp(steering, 0f, Time.deltaTime * SteeringLerpTime);
		}
	}

	private void DoSteering()
	{
		FL_wheelCollider.steerAngle = steering;
		FR_wheelCollider.steerAngle = steering;
	}

	private void ApplyForceAtWheels()
	{
		if (FL_wheelCollider.isGrounded)
		{
			FL_wheelCollider.motorTorque = gasPedal * motorForceConstant;
			FL_wheelCollider.brakeTorque = brakePedal * brakeForceConstant;
		}
		if (FR_wheelCollider.isGrounded)
		{
			FR_wheelCollider.motorTorque = gasPedal * motorForceConstant;
			FR_wheelCollider.brakeTorque = brakePedal * brakeForceConstant;
		}
		if (RL_wheelCollider.isGrounded)
		{
			RL_wheelCollider.motorTorque = gasPedal * motorForceConstant;
			RL_wheelCollider.brakeTorque = brakePedal * brakeForceConstant;
		}
		if (RR_wheelCollider.isGrounded)
		{
			RR_wheelCollider.motorTorque = gasPedal * motorForceConstant;
			RR_wheelCollider.brakeTorque = brakePedal * brakeForceConstant;
		}
	}

	private void UpdateTireAnimation()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0409: Unknown result type (might be due to invalid IL or missing references)
		//IL_0337: Unknown result type (might be due to invalid IL or missing references)
		//IL_035a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0364: Unknown result type (might be due to invalid IL or missing references)
		//IL_037b: Unknown result type (might be due to invalid IL or missing references)
		//IL_038b: Unknown result type (might be due to invalid IL or missing references)
		//IL_039b: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0433: Unknown result type (might be due to invalid IL or missing references)
		//IL_043f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0449: Unknown result type (might be due to invalid IL or missing references)
		float num = Vector3.Dot(myRigidbody.velocity, ((Component)myRigidbody).transform.forward);
		if (FL_wheelCollider.isGrounded)
		{
			FL_shock.localPosition = new Vector3(FL_shock.localPosition.x, shockRestingPosY + GetShockHeightDelta(FL_wheelCollider), FL_shock.localPosition.z);
			FL_wheel.localEulerAngles = new Vector3(FL_wheel.localEulerAngles.x, FL_wheel.localEulerAngles.y, FL_wheel.localEulerAngles.z - num * Time.deltaTime * wheelSpinConstant);
		}
		else
		{
			FL_shock.localPosition = Vector3.Lerp(FL_shock.localPosition, new Vector3(FL_shock.localPosition.x, shockRestingPosY, FL_shock.localPosition.z), Time.deltaTime * 2f);
		}
		if (FR_wheelCollider.isGrounded)
		{
			FR_shock.localPosition = new Vector3(FR_shock.localPosition.x, shockRestingPosY + GetShockHeightDelta(FR_wheelCollider), FR_shock.localPosition.z);
			FR_wheel.localEulerAngles = new Vector3(FR_wheel.localEulerAngles.x, FR_wheel.localEulerAngles.y, FR_wheel.localEulerAngles.z - num * Time.deltaTime * wheelSpinConstant);
		}
		else
		{
			FR_shock.localPosition = Vector3.Lerp(FR_shock.localPosition, new Vector3(FR_shock.localPosition.x, shockRestingPosY, FR_shock.localPosition.z), Time.deltaTime * 2f);
		}
		if (RL_wheelCollider.isGrounded)
		{
			RL_shock.localPosition = new Vector3(RL_shock.localPosition.x, shockRestingPosY + GetShockHeightDelta(RL_wheelCollider), RL_shock.localPosition.z);
			RL_wheel.localEulerAngles = new Vector3(RL_wheel.localEulerAngles.x, RL_wheel.localEulerAngles.y, RL_wheel.localEulerAngles.z - num * Time.deltaTime * wheelSpinConstant);
		}
		else
		{
			RL_shock.localPosition = Vector3.Lerp(RL_shock.localPosition, new Vector3(RL_shock.localPosition.x, shockRestingPosY, RL_shock.localPosition.z), Time.deltaTime * 2f);
		}
		if (RR_wheelCollider.isGrounded)
		{
			RR_shock.localPosition = new Vector3(RR_shock.localPosition.x, shockRestingPosY + GetShockHeightDelta(RR_wheelCollider), RR_shock.localPosition.z);
			RR_wheel.localEulerAngles = new Vector3(RR_wheel.localEulerAngles.x, RR_wheel.localEulerAngles.y, RR_wheel.localEulerAngles.z - num * Time.deltaTime * wheelSpinConstant);
		}
		else
		{
			RR_shock.localPosition = Vector3.Lerp(RR_shock.localPosition, new Vector3(RR_shock.localPosition.x, shockRestingPosY, RR_shock.localPosition.z), Time.deltaTime * 2f);
		}
		Transform[] array = frontAxles;
		foreach (Transform val in array)
		{
			val.localEulerAngles = new Vector3(steering, val.localEulerAngles.y, val.localEulerAngles.z);
		}
	}

	private float GetShockHeightDelta(WheelCollider wheel)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		int mask = LayerMask.GetMask(new string[3] { "Terrain", "World", "Construction" });
		RaycastHit val = default(RaycastHit);
		Physics.Linecast(((Component)wheel).transform.position, ((Component)wheel).transform.position - Vector3.up * 10f, ref val, mask);
		return Mathx.RemapValClamped(((RaycastHit)(ref val)).distance, traceDistanceNeutralPoint - shockDistance, traceDistanceNeutralPoint + shockDistance, shockDistance * 0.75f, -0.75f * shockDistance);
	}
}
