using System;
using Network;
using Rust;
using UnityEngine;

public class Kayak : BaseBoat, IPoolVehicle
{
	private enum PaddleDirection
	{
		Left,
		Right,
		LeftBack,
		RightBack
	}

	public ItemDefinition OarItem;

	public float maxPaddleFrequency = 0.5f;

	public float forwardPaddleForce = 5f;

	public float multiDriverPaddleForceMultiplier = 0.75f;

	public float rotatePaddleForce = 3f;

	public GameObjectRef forwardSplashEffect;

	public GameObjectRef backSplashEffect;

	public ParticleSystem moveSplashEffect;

	public float animationLerpSpeed = 6f;

	[Header("Audio")]
	public BlendedSoundLoops waterLoops;

	public float waterSoundSpeedDivisor = 10f;

	public GameObjectRef pushLandEffect;

	public GameObjectRef pushWaterEffect;

	public PlayerModel.MountPoses noPaddlePose;

	private TimeSince[] playerPaddleCooldowns = (TimeSince[])(object)new TimeSince[2];

	private TimeCachedValue<float> fixedDragUpdate;

	private TimeSince timeSinceLastUsed;

	private const float DECAY_TICK_TIME = 60f;

	private Vector3 lastTravelPos;

	private float distanceRemainder;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("Kayak.OnRpcMessage", 0);
		try
		{
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	public override void ServerInit()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		base.ServerInit();
		timeSinceLastUsed = TimeSince.op_Implicit(0f);
		((FacepunchBehaviour)this).InvokeRandomized((Action)BoatDecay, Random.Range(30f, 60f), 60f, 6f);
	}

	public override void OnPlayerMounted()
	{
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		base.OnPlayerMounted();
		if (((FacepunchBehaviour)this).IsInvoking((Action)TravelDistanceUpdate) || !GameInfo.HasAchievements)
		{
			return;
		}
		int num = 0;
		foreach (MountPointInfo allMountPoint in base.allMountPoints)
		{
			if ((Object)(object)allMountPoint.mountable != (Object)null && allMountPoint.mountable.AnyMounted())
			{
				num++;
			}
		}
		if (num == 2)
		{
			lastTravelPos = Vector3Ex.WithY(((Component)this).transform.position, 0f);
			((FacepunchBehaviour)this).InvokeRandomized((Action)TravelDistanceUpdate, 5f, 5f, 3f);
		}
	}

	public override void OnPlayerDismounted(BasePlayer player)
	{
		base.OnPlayerDismounted(player);
		if (((FacepunchBehaviour)this).IsInvoking((Action)TravelDistanceUpdate))
		{
			((FacepunchBehaviour)this).CancelInvoke((Action)TravelDistanceUpdate);
		}
	}

	public override void DriverInput(InputState inputState, BasePlayer player)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		timeSinceLastUsed = TimeSince.op_Implicit(0f);
		if (!IsPlayerHoldingPaddle(player))
		{
			return;
		}
		int playerSeat = GetPlayerSeat(player);
		if (!(TimeSince.op_Implicit(playerPaddleCooldowns[playerSeat]) > maxPaddleFrequency))
		{
			return;
		}
		bool flag = inputState.IsDown(BUTTON.BACKWARD);
		bool flag2 = false;
		Vector3 val = ((Component)this).transform.forward;
		if (flag)
		{
			val = -val;
		}
		float num = forwardPaddleForce;
		if (NumMounted() >= 2)
		{
			num *= multiDriverPaddleForceMultiplier;
		}
		if (inputState.IsDown(BUTTON.LEFT) || inputState.IsDown(BUTTON.FIRE_PRIMARY))
		{
			flag2 = true;
			rigidBody.AddForceAtPosition(val * num, GetPaddlePoint(playerSeat, PaddleDirection.Left), (ForceMode)1);
			Rigidbody obj = rigidBody;
			obj.angularVelocity += -((Component)this).transform.up * rotatePaddleForce;
			ClientRPC(null, "OnPaddled", flag ? 2 : 0, playerSeat);
		}
		else if (inputState.IsDown(BUTTON.RIGHT) || inputState.IsDown(BUTTON.FIRE_SECONDARY))
		{
			flag2 = true;
			rigidBody.AddForceAtPosition(val * num, GetPaddlePoint(playerSeat, PaddleDirection.Right), (ForceMode)1);
			Rigidbody obj2 = rigidBody;
			obj2.angularVelocity += ((Component)this).transform.up * rotatePaddleForce;
			ClientRPC(null, "OnPaddled", (!flag) ? 1 : 3, playerSeat);
		}
		if (flag2)
		{
			playerPaddleCooldowns[playerSeat] = TimeSince.op_Implicit(0f);
			if (!flag)
			{
				Vector3 velocity = rigidBody.velocity;
				rigidBody.velocity = Vector3.Lerp(velocity, val * ((Vector3)(ref velocity)).magnitude, 0.4f);
			}
		}
	}

	private void TravelDistanceUpdate()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = Vector3Ex.WithY(((Component)this).transform.position, 0f);
		if (GameInfo.HasAchievements)
		{
			float num = Vector3.Distance(lastTravelPos, val) + distanceRemainder;
			float num2 = Mathf.Max(Mathf.Floor(num), 0f);
			distanceRemainder = num - num2;
			foreach (MountPointInfo allMountPoint in base.allMountPoints)
			{
				if ((Object)(object)allMountPoint.mountable != (Object)null && allMountPoint.mountable.AnyMounted() && (int)num2 > 0)
				{
					allMountPoint.mountable.GetMounted().stats.Add("kayak_distance_travelled", (int)num2);
					allMountPoint.mountable.GetMounted().stats.Save(forceSteamSave: true);
				}
			}
		}
		lastTravelPos = val;
	}

	public override bool EngineOn()
	{
		return false;
	}

	protected override void DoPushAction(BasePlayer player)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		if (IsFlipped())
		{
			rigidBody.AddRelativeTorque(Vector3.forward * 8f, (ForceMode)2);
		}
		else
		{
			Vector3 val = Vector3Ex.Direction2D(((Component)player).transform.position + player.eyes.BodyForward() * 3f, ((Component)player).transform.position);
			Vector3 val2 = Vector3.up * 0.1f + val;
			val = ((Vector3)(ref val2)).normalized;
			Vector3 position = ((Component)this).transform.position;
			float num = 5f;
			if (IsInWater())
			{
				num *= 0.75f;
			}
			rigidBody.AddForceAtPosition(val * num, position, (ForceMode)2);
		}
		if (IsInWater())
		{
			if (pushWaterEffect.isValid)
			{
				Effect.server.Run(pushWaterEffect.resourcePath, this, 0u, Vector3.zero, Vector3.zero);
			}
		}
		else if (pushLandEffect.isValid)
		{
			Effect.server.Run(pushLandEffect.resourcePath, this, 0u, Vector3.zero, Vector3.zero);
		}
		WakeUp();
	}

	public override void VehicleFixedUpdate()
	{
		base.VehicleFixedUpdate();
		if (fixedDragUpdate == null)
		{
			fixedDragUpdate = new TimeCachedValue<float>
			{
				refreshCooldown = 0.5f,
				refreshRandomRange = 0.2f,
				updateValue = CalculateDesiredDrag
			};
		}
		rigidBody.drag = fixedDragUpdate.Get(force: false);
	}

	private float CalculateDesiredDrag()
	{
		int num = NumMounted();
		if (num == 0)
		{
			return 1f;
		}
		if (num < 2)
		{
			return 0.05f;
		}
		return 0.1f;
	}

	private void BoatDecay()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		BaseBoat.WaterVehicleDecay(this, 60f, TimeSince.op_Implicit(timeSinceLastUsed), MotorRowboat.outsidedecayminutes, MotorRowboat.deepwaterdecayminutes, MotorRowboat.decaystartdelayminutes, preventDecayIndoors);
	}

	public override bool CanPickup(BasePlayer player)
	{
		if (!HasDriver())
		{
			return base.CanPickup(player);
		}
		return false;
	}

	public bool IsPlayerHoldingPaddle(BasePlayer player)
	{
		if ((Object)(object)player.GetHeldEntity() != (Object)null)
		{
			return (Object)(object)player.GetHeldEntity().GetItem().info == (Object)(object)OarItem;
		}
		return false;
	}

	private Vector3 GetPaddlePoint(int index, PaddleDirection direction)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		index = Mathf.Clamp(index, 0, mountPoints.Count);
		Vector3 pos = mountPoints[index].pos;
		switch (direction)
		{
		case PaddleDirection.Left:
			pos.x -= 1f;
			break;
		case PaddleDirection.Right:
			pos.x += 1f;
			break;
		}
		pos.y -= 0.2f;
		return ((Component)this).transform.TransformPoint(pos);
	}

	private bool IsInWater()
	{
		if (base.isServer)
		{
			return buoyancy.timeOutOfWater < 0.1f;
		}
		return false;
	}
}
