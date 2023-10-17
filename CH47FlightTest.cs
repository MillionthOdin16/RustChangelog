using UnityEngine;

public class CH47FlightTest : MonoBehaviour
{
	public struct HelicopterInputState_t
	{
		public float throttle;

		public float roll;

		public float yaw;

		public float pitch;
	}

	public Rigidbody rigidBody;

	public float engineThrustMax;

	public Vector3 torqueScale;

	public Transform com;

	public Transform[] GroundPoints;

	public Transform[] GroundEffects;

	public float currentThrottle = 0f;

	public float avgThrust = 0f;

	public float liftDotMax = 0.75f;

	public Transform AIMoveTarget;

	private static float altitudeTolerance = 1f;

	public void Awake()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		rigidBody.centerOfMass = com.localPosition;
	}

	public HelicopterInputState_t GetHelicopterInputState()
	{
		HelicopterInputState_t result = default(HelicopterInputState_t);
		result.throttle = (Input.GetKey((KeyCode)119) ? 1f : 0f);
		result.throttle -= (Input.GetKey((KeyCode)115) ? 1f : 0f);
		result.pitch = Input.GetAxis("Mouse Y");
		result.roll = 0f - Input.GetAxis("Mouse X");
		result.yaw = (Input.GetKey((KeyCode)100) ? 1f : 0f);
		result.yaw -= (Input.GetKey((KeyCode)97) ? 1f : 0f);
		result.pitch = Mathf.RoundToInt(result.pitch);
		result.roll = Mathf.RoundToInt(result.roll);
		return result;
	}

	public HelicopterInputState_t GetAIInputState()
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		HelicopterInputState_t result = default(HelicopterInputState_t);
		Vector3 val = Vector3.Cross(Vector3.up, ((Component)this).transform.right);
		Vector3 val2 = Vector3.Cross(Vector3.up, val);
		float num = Vector3.Dot(val2, Vector3Ex.Direction2D(AIMoveTarget.position, ((Component)this).transform.position));
		result.yaw = ((num < 0f) ? 1f : 0f);
		result.yaw -= ((num > 0f) ? 1f : 0f);
		float num2 = Vector3.Dot(Vector3.up, ((Component)this).transform.right);
		result.roll = ((num2 < 0f) ? 1f : 0f);
		result.roll -= ((num2 > 0f) ? 1f : 0f);
		float num3 = Vector3Ex.Distance2D(((Component)this).transform.position, AIMoveTarget.position);
		float num4 = Vector3.Dot(val, Vector3Ex.Direction2D(AIMoveTarget.position, ((Component)this).transform.position));
		float num5 = Vector3.Dot(Vector3.up, ((Component)this).transform.forward);
		if (num3 > 10f)
		{
			result.pitch = ((num4 > 0.8f) ? (-0.25f) : 0f);
			result.pitch -= ((num4 < -0.8f) ? (-0.25f) : 0f);
			if (num5 < -0.35f)
			{
				result.pitch = -1f;
			}
			else if (num5 > 0.35f)
			{
				result.pitch = 1f;
			}
		}
		else if (num5 < -0f)
		{
			result.pitch = -1f;
		}
		else if (num5 > 0f)
		{
			result.pitch = 1f;
		}
		float idealAltitude = GetIdealAltitude();
		float y = ((Component)this).transform.position.y;
		float num6 = 0f;
		num6 = ((y > idealAltitude + altitudeTolerance) ? (-1f) : ((y < idealAltitude - altitudeTolerance) ? 1f : ((!(num3 > 20f)) ? 0f : Mathf.Lerp(0f, 1f, num3 / 20f))));
		Debug.Log((object)("desiredThrottle : " + num6));
		result.throttle = num6 * 1f;
		return result;
	}

	public float GetIdealAltitude()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		return ((Component)AIMoveTarget).transform.position.y;
	}

	public void FixedUpdate()
	{
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		HelicopterInputState_t aIInputState = GetAIInputState();
		currentThrottle = Mathf.Lerp(currentThrottle, aIInputState.throttle, 2f * Time.fixedDeltaTime);
		currentThrottle = Mathf.Clamp(currentThrottle, -0.2f, 1f);
		rigidBody.AddRelativeTorque(new Vector3(aIInputState.pitch * torqueScale.x, aIInputState.yaw * torqueScale.y, aIInputState.roll * torqueScale.z) * Time.fixedDeltaTime, (ForceMode)0);
		avgThrust = Mathf.Lerp(avgThrust, engineThrustMax * currentThrottle, Time.fixedDeltaTime);
		float num = Mathf.Clamp01(Vector3.Dot(((Component)this).transform.up, Vector3.up));
		float num2 = Mathf.InverseLerp(liftDotMax, 1f, num);
		Vector3 val = Vector3.up * engineThrustMax * 0.5f * currentThrottle * num2;
		Vector3 val2 = ((Component)this).transform.up - Vector3.up;
		Vector3 val3 = ((Vector3)(ref val2)).normalized * engineThrustMax * currentThrottle * (1f - num2);
		float num3 = rigidBody.mass * (0f - Physics.gravity.y);
		rigidBody.AddForce(((Component)this).transform.up * num3 * num2 * 0.99f, (ForceMode)0);
		rigidBody.AddForce(val, (ForceMode)0);
		rigidBody.AddForce(val3, (ForceMode)0);
		RaycastHit val6 = default(RaycastHit);
		for (int i = 0; i < GroundEffects.Length; i++)
		{
			Transform val4 = GroundPoints[i];
			Transform val5 = GroundEffects[i];
			if (Physics.Raycast(((Component)val4).transform.position, Vector3.down, ref val6, 50f, 8388608))
			{
				((Component)val5).gameObject.SetActive(true);
				((Component)val5).transform.position = ((RaycastHit)(ref val6)).point + new Vector3(0f, 1f, 0f);
			}
			else
			{
				((Component)val5).gameObject.SetActive(false);
			}
		}
	}

	public void OnDrawGizmos()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		Gizmos.color = Color.yellow;
		Gizmos.DrawSphere(((Component)AIMoveTarget).transform.position, 1f);
		Vector3 val = Vector3.Cross(((Component)this).transform.right, Vector3.up);
		Vector3 val2 = Vector3.Cross(val, Vector3.up);
		Gizmos.color = Color.blue;
		Gizmos.DrawLine(((Component)this).transform.position, ((Component)this).transform.position + val * 10f);
		Gizmos.color = Color.red;
		Gizmos.DrawLine(((Component)this).transform.position, ((Component)this).transform.position + val2 * 10f);
	}
}
