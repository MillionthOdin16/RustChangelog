using System;
using UnityEngine;

public class FishingBobber : BaseCombatEntity
{
	public Transform centerOfMass;

	public Rigidbody myRigidBody;

	public Transform lineAttachPoint = null;

	public Transform bobberRoot = null;

	public const Flags CaughtFish = Flags.Reserved1;

	public float HorizontalMoveSpeed = 1f;

	public float PullAwayMoveSpeed = 1f;

	public float SidewaysInputForce = 1f;

	public float ReelInMoveSpeed = 1f;

	private float bobberForcePingPong = 0f;

	private Vector3 initialDirection;

	private Vector3 initialTargetPosition;

	private Vector3 spawnPosition;

	private TimeSince initialCastTime;

	private float initialDistance = 0f;

	public float TireAmount { get; private set; } = 0f;


	public override void ServerInit()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		myRigidBody.centerOfMass = centerOfMass.localPosition;
		base.ServerInit();
	}

	public void InitialiseBobber(BasePlayer forPlayer, WaterBody forBody, Vector3 targetPos)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		initialDirection = Vector3Ex.WithY(forPlayer.eyes.HeadForward(), 0f);
		spawnPosition = ((Component)this).transform.position;
		initialTargetPosition = targetPos;
		initialCastTime = TimeSince.op_Implicit(0f);
		initialDistance = Vector3.Distance(targetPos, Vector3Ex.WithY(((Component)forPlayer).transform.position, targetPos.y));
		((FacepunchBehaviour)this).InvokeRepeating((Action)ProcessInitialCast, 0f, 0f);
	}

	private void ProcessInitialCast()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		float num = 0.8f;
		if (TimeSince.op_Implicit(initialCastTime) > num)
		{
			((Component)this).transform.position = initialTargetPosition;
			((FacepunchBehaviour)this).CancelInvoke((Action)ProcessInitialCast);
			return;
		}
		float num2 = TimeSince.op_Implicit(initialCastTime) / num;
		Vector3 val = Vector3.Lerp(spawnPosition, initialTargetPosition, 0.5f);
		val.y += 1.5f;
		Vector3 position = Vector3.Lerp(Vector3.Lerp(spawnPosition, val, num2), Vector3.Lerp(val, initialTargetPosition, num2), num2);
		((Component)this).transform.position = position;
	}

	public void ServerMovementUpdate(bool inputLeft, bool inputRight, bool inputBack, ref BaseFishingRod.FishState state, Vector3 playerPos, ItemModFishable fishableModifier)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = playerPos - ((Component)this).transform.position;
		Vector3 normalized = ((Vector3)(ref val)).normalized;
		Vector3 val2 = Vector3.zero;
		bobberForcePingPong = Mathf.Clamp(Mathf.PingPong(Time.time, 2f), 0.2f, 2f);
		if (state.Contains(BaseFishingRod.FishState.PullingLeft))
		{
			val2 = ((Component)this).transform.right * (Time.deltaTime * HorizontalMoveSpeed * bobberForcePingPong * fishableModifier.MoveMultiplier * (inputRight ? 0.5f : 1f));
		}
		if (state.Contains(BaseFishingRod.FishState.PullingRight))
		{
			val2 = -((Component)this).transform.right * (Time.deltaTime * HorizontalMoveSpeed * bobberForcePingPong * fishableModifier.MoveMultiplier * (inputLeft ? 0.5f : 1f));
		}
		if (state.Contains(BaseFishingRod.FishState.PullingBack))
		{
			val2 += -((Component)this).transform.forward * (Time.deltaTime * PullAwayMoveSpeed * bobberForcePingPong * fishableModifier.MoveMultiplier * (inputBack ? 0.5f : 1f));
		}
		if (inputLeft || inputRight)
		{
			float num = 0.8f;
			if ((inputLeft && state == BaseFishingRod.FishState.PullingRight) || (inputRight && state == BaseFishingRod.FishState.PullingLeft))
			{
				num = 1.25f;
			}
			TireAmount += Time.deltaTime * num;
		}
		else
		{
			TireAmount -= Time.deltaTime * 0.1f;
		}
		if (inputLeft && !state.Contains(BaseFishingRod.FishState.PullingLeft))
		{
			val2 += ((Component)this).transform.right * (Time.deltaTime * SidewaysInputForce);
		}
		else if (inputRight && !state.Contains(BaseFishingRod.FishState.PullingRight))
		{
			val2 += -((Component)this).transform.right * (Time.deltaTime * SidewaysInputForce);
		}
		if (inputBack)
		{
			float num2 = Mathx.RemapValClamped(TireAmount, 0f, 5f, 1f, 3f);
			val2 += normalized * (ReelInMoveSpeed * fishableModifier.ReelInSpeedMultiplier * num2 * Time.deltaTime);
		}
		((Component)this).transform.LookAt(Vector3Ex.WithY(playerPos, ((Component)this).transform.position.y));
		Vector3 val3 = ((Component)this).transform.position + val2;
		if (!IsDirectionValid(val3, ((Vector3)(ref val2)).magnitude, playerPos))
		{
			state = state.FlipHorizontal();
		}
		else
		{
			((Component)this).transform.position = val3;
		}
	}

	private bool IsDirectionValid(Vector3 pos, float checkLength, Vector3 playerPos)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = pos - playerPos;
		float num = Vector3.Angle(Vector3Ex.WithY(((Vector3)(ref val)).normalized, 0f), initialDirection);
		if (num > 60f)
		{
			return false;
		}
		Vector3 position = ((Component)this).transform.position;
		val = pos - position;
		Ray ray = default(Ray);
		((Ray)(ref ray))._002Ector(position, ((Vector3)(ref val)).normalized);
		if (GamePhysics.Trace(ray, 0.1f, out var _, checkLength, 1218511105, (QueryTriggerInteraction)0))
		{
			return false;
		}
		return true;
	}
}
