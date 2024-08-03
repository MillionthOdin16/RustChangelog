using UnityEngine;

public class AIHelicopterAnimation : MonoBehaviour
{
	public PatrolHelicopterAI _ai;

	public float swayAmount = 1f;

	public float lastStrafeScalar = 0f;

	public float lastForwardBackScalar = 0f;

	public float degreeMax = 90f;

	public Vector3 lastPosition = Vector3.zero;

	public float oldMoveSpeed = 0f;

	public float smoothRateOfChange = 0f;

	public float flareAmount = 0f;

	public void Awake()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		lastPosition = ((Component)this).transform.position;
	}

	public Vector3 GetMoveDirection()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		return _ai.GetMoveDirection();
	}

	public float GetMoveSpeed()
	{
		return _ai.GetMoveSpeed();
	}

	public void Update()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		lastPosition = ((Component)this).transform.position;
		Vector3 moveDirection = GetMoveDirection();
		float moveSpeed = GetMoveSpeed();
		float num = 0.25f + Mathf.Clamp01(moveSpeed / _ai.maxSpeed) * 0.75f;
		smoothRateOfChange = Mathf.Lerp(smoothRateOfChange, moveSpeed - oldMoveSpeed, Time.deltaTime * 5f);
		oldMoveSpeed = moveSpeed;
		float num2 = Vector3.Angle(moveDirection, ((Component)this).transform.forward);
		float num3 = Vector3.Angle(moveDirection, -((Component)this).transform.forward);
		float num4 = 1f - Mathf.Clamp01(num2 / degreeMax);
		float num5 = 1f - Mathf.Clamp01(num3 / degreeMax);
		float num6 = (num4 - num5) * num;
		float num7 = (lastForwardBackScalar = Mathf.Lerp(lastForwardBackScalar, num6, Time.deltaTime * 2f));
		float num8 = Vector3.Angle(moveDirection, ((Component)this).transform.right);
		float num9 = Vector3.Angle(moveDirection, -((Component)this).transform.right);
		float num10 = 1f - Mathf.Clamp01(num8 / degreeMax);
		float num11 = 1f - Mathf.Clamp01(num9 / degreeMax);
		float num12 = (num10 - num11) * num;
		float num13 = (lastStrafeScalar = Mathf.Lerp(lastStrafeScalar, num12, Time.deltaTime * 2f));
		Vector3 zero = Vector3.zero;
		zero.x += num7 * swayAmount;
		zero.z -= num13 * swayAmount;
		Quaternion identity = Quaternion.identity;
		identity = Quaternion.Euler(zero.x, zero.y, zero.z);
		_ai.helicopterBase.rotorPivot.transform.localRotation = identity;
	}
}
