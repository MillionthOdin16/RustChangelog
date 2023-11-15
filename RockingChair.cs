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

	[Header("Audio")]
	public SoundDefinition creakForwardSoundDef;

	public SoundDefinition creakBackwardSoundDef;

	public float creakForwardAngle = 0.1f;

	public float creakBackwardAngle = -0.1f;

	public float creakVelocityThreshold = 0.02f;

	public AnimationCurve creakGainCurve;

	private float initLocalY;

	private Vector3 initLocalRot;

	private float velocity;

	private float oppositePotentialVelocity;

	private TimeSince timeSinceInput;

	private float sineTime;

	private float timeUntilStartSine = 0.4f;

	private float t;

	private float angle;

	private Quaternion max;

	private Quaternion min;

	public override void ServerInit()
	{
		base.ServerInit();
		SaveBaseLocalPos();
		ResetChair();
	}

	public override void OnParentChanging(BaseEntity oldParent, BaseEntity newParent)
	{
		((FacepunchBehaviour)this).Invoke((Action)SaveBaseLocalPos, 0f);
	}

	private void SaveBaseLocalPos()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
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
		Quaternion localRotation = ((Component)this).transform.localRotation;
		initLocalRot = ((Quaternion)(ref localRotation)).eulerAngles;
		initLocalY = ((Component)this).transform.localPosition.y;
		max = Quaternion.Euler(initLocalRot) * Quaternion.AngleAxis(MaxRockingAngle, Vector3.right);
		min = Quaternion.Euler(initLocalRot) * Quaternion.AngleAxis(0f - MaxRockingAngle, Vector3.right);
	}

	public override void Save(SaveInfo info)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		base.Save(info);
		info.msg.rockingChair = Pool.Get<RockingChair>();
		info.msg.rockingChair.initEuler = initLocalRot;
		info.msg.rockingChair.initY = initLocalY;
	}

	public override void Load(LoadInfo info)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		base.Load(info);
		if (info.msg.rockingChair != null && base.isServer)
		{
			initLocalRot = info.msg.rockingChair.initEuler;
			((Component)this).transform.localRotation = Quaternion.Euler(initLocalRot);
			initLocalY = info.msg.rockingChair.initY;
			if (initLocalY == 0f)
			{
				initLocalY = ((Component)this).transform.localPosition.y;
			}
		}
	}

	public override void PlayerServerInput(InputState inputState, BasePlayer player)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		float timeSinceLastTick = player.timeSinceLastTick;
		Vector2 inputVector = GetInputVector(inputState);
		CalculateVelocity(inputVector);
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
		PreventClipping(flag);
		ApplyVelocity(timeSinceLastTick, flag);
	}

	public override void OnWeaponFired(BaseProjectile weapon)
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		if (!((Object)(object)weapon == (Object)null))
		{
			if ((Object)(object)weapon.recoil != (Object)null)
			{
				velocity += weapon.recoil.recoilPitchMax * WeaponFireImpact;
			}
			timeSinceInput = TimeSince.op_Implicit(0f);
			sineTime = 0f;
		}
	}

	public override void OnPlayerDismounted(BasePlayer player)
	{
		base.OnPlayerDismounted(player);
		ResetChair();
	}

	private void PreventClipping(bool hasInput)
	{
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		float num = initLocalY + 0.06f;
		float num2 = Mathx.RemapValClamped(Mathf.Abs(angle), 0f, MaxRockingAngle, 0f, 1f);
		if (num2 > 0.7f)
		{
			((Component)this).transform.localPosition = Mathx.Lerp(new Vector3(((Component)this).transform.localPosition.x, initLocalY, ((Component)this).transform.localPosition.z), new Vector3(((Component)this).transform.localPosition.x, num, ((Component)this).transform.localPosition.z), 1.5f, num2);
		}
		else
		{
			((Component)this).transform.localPosition = Mathx.Lerp(((Component)this).transform.localPosition, new Vector3(((Component)this).transform.localPosition.x, initLocalY, ((Component)this).transform.localPosition.z), 1.5f, Time.deltaTime);
		}
	}

	private void CalculateVelocity(Vector2 currentInput)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		velocity += currentInput.y * Acceleration;
		velocity = Mathf.Clamp(velocity, 0f - MaxRockVelocity, MaxRockVelocity);
		oppositePotentialVelocity = (0f - velocity) * AppliedVelocity;
		int signZero = Mathx.GetSignZero(currentInput.y, true);
		int signZero2 = Mathx.GetSignZero(velocity, true);
		if (ApplyVelocityBetweenSwings && Mathf.Abs(velocity) > 0.3f && Mathx.HasSignFlipped(signZero, signZero2))
		{
			velocity += oppositePotentialVelocity;
		}
	}

	private void ApplyVelocity(float delta, bool hasInput)
	{
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		t = Mathf.Sin(sineTime * ((float)Math.PI / 180f));
		t = Mathx.RemapValClamped(t, -1f, 1f, 0f, 1f);
		t = EaseOutCubicOvershoot(t, 0.2f);
		t = Mathf.Lerp(t, 0.5f, Mathf.Clamp01(TimeSince.op_Implicit(timeSinceInput) / 10f));
		angle += velocity;
		angle = Mathf.Clamp(angle, 0f - MaxRockingAngle, MaxRockingAngle);
		Quaternion val = Quaternion.Euler(initLocalRot) * Quaternion.AngleAxis(angle, Vector3.right);
		Quaternion val2 = Quaternion.Slerp(min, max, t);
		float num = ((!hasInput && TimeSince.op_Implicit(timeSinceInput) > timeUntilStartSine) ? 1 : 0);
		Quaternion val3 = Quaternion.Slerp(val, val2, num);
		((Component)this).transform.localRotation = Quaternion.Slerp(((Component)this).transform.localRotation, val3, delta * 3f);
	}

	private void ResetChair()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		((Component)this).transform.localRotation = Quaternion.Euler(initLocalRot);
		((Component)this).transform.localPosition = new Vector3(((Component)this).transform.localPosition.x, initLocalY, ((Component)this).transform.localPosition.z);
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

	private float EaseOutCubic(float value)
	{
		return 1f - Mathf.Pow(1f - Mathf.Clamp01(value), 3f);
	}

	private float EaseOutCubicOvershoot(float value, float overshoot)
	{
		return 1f - Mathf.Pow(1f - Mathf.Clamp01(value), 3f) * (1f + overshoot * (Mathf.Clamp01(value) - 1f));
	}
}
