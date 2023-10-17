using UnityEngine;

public class BradleyMoveTest : MonoBehaviour
{
	public WheelCollider[] leftWheels;

	public WheelCollider[] rightWheels;

	public float moveForceMax = 2000f;

	public float brakeForce = 100f;

	public float throttle = 1f;

	public float turnForce = 2000f;

	public float sideStiffnessMax = 1f;

	public float sideStiffnessMin = 0.5f;

	public Transform centerOfMass;

	public float turning = 0f;

	public bool brake = false;

	public Rigidbody myRigidBody;

	public Vector3 destination;

	public float stoppingDist = 5f;

	public GameObject followTest;

	public void Awake()
	{
		Initialize();
	}

	public void Initialize()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		myRigidBody.centerOfMass = centerOfMass.localPosition;
		destination = ((Component)this).transform.position;
	}

	public void SetDestination(Vector3 dest)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		destination = dest;
	}

	public void FixedUpdate()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		Vector3 velocity = myRigidBody.velocity;
		SetDestination(followTest.transform.position);
		float num = Vector3.Distance(((Component)this).transform.position, destination);
		if (num > stoppingDist)
		{
			Vector3 zero = Vector3.zero;
			float num2 = Vector3.Dot(zero, ((Component)this).transform.right);
			float num3 = Vector3.Dot(zero, -((Component)this).transform.right);
			float num4 = Vector3.Dot(zero, ((Component)this).transform.right);
			float num5 = Vector3.Dot(zero, -((Component)this).transform.forward);
			if (num5 > num4)
			{
				if (num2 >= num3)
				{
					turning = 1f;
				}
				else
				{
					turning = -1f;
				}
			}
			else
			{
				turning = num4;
			}
			throttle = Mathf.InverseLerp(stoppingDist, 30f, num);
		}
		throttle = Mathf.Clamp(throttle, -1f, 1f);
		float num6 = throttle;
		float num7 = throttle;
		if (turning > 0f)
		{
			num7 = 0f - turning;
			num6 = turning;
		}
		else if (turning < 0f)
		{
			num6 = turning;
			num7 = turning * -1f;
		}
		ApplyBrakes(brake ? 1f : 0f);
		float num8 = throttle;
		num6 = Mathf.Clamp(num6 + num8, -1f, 1f);
		num7 = Mathf.Clamp(num7 + num8, -1f, 1f);
		AdjustFriction();
		float num9 = Mathf.InverseLerp(3f, 1f, ((Vector3)(ref velocity)).magnitude * Mathf.Abs(Vector3.Dot(((Vector3)(ref velocity)).normalized, ((Component)this).transform.forward)));
		float torqueAmount = Mathf.Lerp(moveForceMax, turnForce, num9);
		SetMotorTorque(num6, rightSide: false, torqueAmount);
		SetMotorTorque(num7, rightSide: true, torqueAmount);
	}

	public void ApplyBrakes(float amount)
	{
		ApplyBrakeTorque(amount, rightSide: true);
		ApplyBrakeTorque(amount, rightSide: false);
	}

	public float GetMotorTorque(bool rightSide)
	{
		float num = 0f;
		WheelCollider[] array = (rightSide ? rightWheels : leftWheels);
		foreach (WheelCollider val in array)
		{
			num += val.motorTorque;
		}
		return num / (float)rightWheels.Length;
	}

	public void SetMotorTorque(float newThrottle, bool rightSide, float torqueAmount)
	{
		newThrottle = Mathf.Clamp(newThrottle, -1f, 1f);
		float motorTorque = torqueAmount * newThrottle;
		WheelCollider[] array = (rightSide ? rightWheels : leftWheels);
		foreach (WheelCollider val in array)
		{
			val.motorTorque = motorTorque;
		}
	}

	public void ApplyBrakeTorque(float amount, bool rightSide)
	{
		WheelCollider[] array = (rightSide ? rightWheels : leftWheels);
		foreach (WheelCollider val in array)
		{
			val.brakeTorque = brakeForce * amount;
		}
	}

	public void AdjustFriction()
	{
	}
}
