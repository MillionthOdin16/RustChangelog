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

	public ItemDefinition OarItem = null;

	public float maxPaddleFrequency = 0.5f;

	public float forwardPaddleForce = 5f;

	public float multiDriverPaddleForceMultiplier = 0.75f;

	public float rotatePaddleForce = 3f;

	public GameObjectRef forwardSplashEffect;

	public GameObjectRef backSplashEffect;

	public ParticleSystem moveSplashEffect = null;

	public float animationLerpSpeed = 6f;

	[Header("Audio")]
	public BlendedSoundLoops waterLoops;

	public float waterSoundSpeedDivisor = 10f;

	public GameObjectRef pushLandEffect;

	public GameObjectRef pushWaterEffect;

	public PlayerModel.MountPoses noPaddlePose = PlayerModel.MountPoses.Chair;

	private TimeSince[] playerPaddleCooldowns = (TimeSince[])(object)new TimeSince[2];

	private TimeCachedValue<float> fixedDragUpdate;

	private TimeSince timeSinceLastUsed;

	private const float DECAY_TICK_TIME = 60f;

	private Vector3 lastTravelPos;

	private float distanceRemainder = 0f;

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
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		base.ServerInit();
		timeSinceLastUsed = TimeSince.op_Implicit(0f);
		((FacepunchBehaviour)this).InvokeRandomized((Action)BoatDecay, Random.Range(30f, 60f), 60f, 6f);
	}

	public override void OnPlayerMounted()
	{
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
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
		return (num >= 2) ? 0.1f : 0.05f;
	}

	private void BoatDecay()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		BaseBoat.WaterVehicleDecay(this, 60f, TimeSince.op_Implicit(timeSinceLastUsed), MotorRowboat.outsidedecayminutes, MotorRowboat.deepwaterdecayminutes, MotorRowboat.decaystartdelayminutes, preventDecayIndoors);
	}

	public override bool CanPickup(BasePlayer player)
	{
		return !HasDriver() && base.CanPickup(player);
	}

	public bool IsPlayerHoldingPaddle(BasePlayer player)
	{
		return (Object)(object)player.GetHeldEntity() != (Object)null && (Object)(object)player.GetHeldEntity().GetItem().info == (Object)(object)OarItem;
	}

	private Vector3 GetPaddlePoint(int index, PaddleDirection direction)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
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
