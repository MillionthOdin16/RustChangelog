using System;
using Facepunch;
using ProtoBuf;
using UnityEngine;

public class RockingChair : BaseChair
{
	[Header("Rocking Settings")]
	public float Acceleration = 0.8f;

	public float MaxRockingAngle = 9f;

	public float MaxRockVelocity = 4f;

	[Tooltip("Preserve and apply some existing velocity when swinging back and forth.")]
	public bool ApplyVelocityBetweenSwings = true;

	[Range(0f, 2f)]
	public float AppliedVelocity = 1f;

	[Range(0f, 2f)]
	public float WeaponFireImpact = 3f;

	private Vector3 initEuler = Vector3.zero;

	private float velocity;

	private float oppositePotentialVelocity;

	private TimeSince timeSinceInput;

	private float sineTime;

	private float timeUntilStartSine = 0.7f;

	private float t;

	private float angle;

	private Quaternion max;

	private Quaternion min;

	public override void ServerInit()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		base.ServerInit();
		Quaternion rotation = ((Component)this).transform.rotation;
		initEuler = ((Quaternion)(ref rotation)).eulerAngles;
		initEuler.x = 0f;
		max = Quaternion.Euler(initEuler) * Quaternion.AngleAxis(MaxRockingAngle, Vector3.right);
		min = Quaternion.Euler(initEuler) * Quaternion.AngleAxis(0f - MaxRockingAngle, Vector3.right);
		ResetChair();
	}

	public override void Save(SaveInfo info)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		base.Save(info);
		info.msg.rockingChair = Pool.Get<RockingChair>();
		info.msg.rockingChair.initEuler = initEuler;
	}

	public override void Load(LoadInfo info)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		base.Load(info);
		if (info.msg.rockingChair != null && base.isServer)
		{
			initEuler = info.msg.rockingChair.initEuler;
			((Component)this).transform.rotation = Quaternion.Euler(initEuler);
		}
	}

	public override void PlayerServerInput(InputState inputState, BasePlayer player)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		float timeSinceLastTick = player.timeSinceLastTick;
		Vector2 inputVector = GetInputVector(inputState);
		velocity += inputVector.y * Acceleration;
		velocity = Mathf.Clamp(velocity, 0f - MaxRockVelocity, MaxRockVelocity);
		oppositePotentialVelocity = (0f - velocity) * AppliedVelocity;
		int signZero = GetSignZero(inputVector.y);
		int signZero2 = GetSignZero(velocity);
		if (ApplyVelocityBetweenSwings && Mathf.Abs(velocity) > 0.3f && HasSignFlipped(signZero, signZero2))
		{
			velocity += oppositePotentialVelocity;
		}
		bool flag = !Mathf.Approximately(inputVector.y, 0f);
		if (flag)
		{
			timeSinceInput = TimeSince.op_Implicit(0f);
			sineTime = 0f;
		}
		else if (TimeSince.op_Implicit(timeSinceInput) > timeUntilStartSine)
		{
			angle = Mathf.Lerp(0f - MaxRockingAngle, MaxRockingAngle, t);
		}
		sineTime += player.timeSinceLastTick * 180f;
		ApplyVelocity(timeSinceLastTick, flag);
	}

	public override void OnWeaponFired(BaseProjectile weapon)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)weapon != (Object)null)
		{
			velocity += weapon.recoil.recoilPitchMax * WeaponFireImpact;
			timeSinceInput = TimeSince.op_Implicit(0f);
			sineTime = 0f;
		}
	}

	public override void OnPlayerDismounted(BasePlayer player)
	{
		base.OnPlayerDismounted(player);
		ResetChair();
	}

	private void ApplyVelocity(float delta, bool hasInput)
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		t = Mathf.Sin(sineTime * ((float)Math.PI / 180f));
		t = Mathx.RemapValClamped(t, -1f, 1f, 0f, 1f);
		t = Mathf.Lerp(t, 0.5f, Mathf.Clamp01(TimeSince.op_Implicit(timeSinceInput) / 10f));
		angle += velocity;
		angle = Mathf.Clamp(angle, 0f - MaxRockingAngle, MaxRockingAngle);
		Quaternion val = Quaternion.Euler(initEuler) * Quaternion.AngleAxis(angle, Vector3.right);
		Quaternion val2 = Quaternion.Lerp(min, max, t);
		float num = ((!hasInput && TimeSince.op_Implicit(timeSinceInput) > timeUntilStartSine) ? 1 : 0);
		Quaternion val3 = val;
		val3 = Quaternion.Slerp(val, val2, num);
		((Component)this).transform.rotation = Quaternion.Slerp(((Component)this).transform.rotation, val3, delta * 3f);
	}

	private bool HasSignFlipped(int signA, int signB)
	{
		if (signA == 0 || signB == 0)
		{
			return false;
		}
		if (signA == signB || signB == signA)
		{
			return false;
		}
		return true;
	}

	private int GetSignZero(float input, bool useApproximate = true)
	{
		if (useApproximate)
		{
			if (Mathf.Approximately(input, 0f))
			{
				return 0;
			}
			if (input == 0f)
			{
				return 0;
			}
		}
		if (!(input < 0f))
		{
			return 1;
		}
		return -1;
	}

	private void ResetChair()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		initEuler.x = 0f;
		((Component)this).transform.rotation = Quaternion.Euler(initEuler);
	}

	private Vector2 GetInputVector(InputState inputState)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		bool rightDown = false;
		bool forwardDown = inputState.IsDown(BUTTON.FORWARD);
		bool backDown = inputState.IsDown(BUTTON.BACKWARD);
		return ProcessInputVector(leftDown: false, rightDown, forwardDown, backDown);
	}

	private static Vector2 ProcessInputVector(bool leftDown, bool rightDown, bool forwardDown, bool backDown)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		Vector2 zero = Vector2.zero;
		if (leftDown && rightDown)
		{
			leftDown = (rightDown = false);
		}
		if (forwardDown && backDown)
		{
			forwardDown = (backDown = false);
		}
		if (forwardDown)
		{
			zero.y = 1f;
		}
		else if (backDown)
		{
			zero.y = -1f;
		}
		if (rightDown)
		{
			zero.x = 1f;
		}
		else if (leftDown)
		{
			zero.x = -1f;
		}
		return zero;
	}
}
