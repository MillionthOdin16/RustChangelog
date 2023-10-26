using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

public class MiniCopter : BaseHelicopterVehicle, IEngineControllerUser, IEntity, SamSite.ISamSiteTarget
{
	[Header("Fuel")]
	public GameObjectRef fuelStoragePrefab;

	public float fuelPerSec = 0.25f;

	public float fuelGaugeMax = 100f;

	private float cachedFuelFraction = 0f;

	public Transform waterSample;

	public WheelCollider leftWheel;

	public WheelCollider rightWheel;

	public WheelCollider frontWheel;

	public Transform leftWheelTrans;

	public Transform rightWheelTrans;

	public Transform frontWheelTrans;

	public float cachedrotation_left;

	public float cachedrotation_right;

	public float cachedrotation_front;

	[Header("IK")]
	public Transform joystickPositionLeft;

	public Transform joystickPositionRight;

	public Transform leftFootPosition;

	public Transform rightFootPosition;

	public AnimationCurve bladeEngineCurve;

	public Animator animator;

	public float maxRotorSpeed = 10f;

	public float timeUntilMaxRotorSpeed = 7f;

	public float rotorBlurThreshold = 8f;

	public Transform mainRotorBlur;

	public Transform mainRotorBlades;

	public Transform rearRotorBlades;

	public Transform rearRotorBlur;

	public float motorForceConstant = 150f;

	public float brakeForceConstant = 500f;

	public GameObject preventBuildingObject;

	[ServerVar(Help = "Population active on the server", ShowInAdminUI = true)]
	public static float population = 0f;

	[ServerVar(Help = "How long before a minicopter loses all its health while outside")]
	public static float outsidedecayminutes = 480f;

	[ServerVar(Help = "How long before a minicopter loses all its health while indoors")]
	public static float insidedecayminutes = 2880f;

	private VehicleEngineController<MiniCopter> engineController;

	private bool isPushing = false;

	private float lastEngineOnTime;

	private float cachedPitch = 0f;

	private float cachedYaw = 0f;

	private float cachedRoll = 0f;

	public bool IsStartingUp => engineController != null && engineController.IsStarting;

	public VehicleEngineController<MiniCopter>.EngineState CurEngineState
	{
		get
		{
			if (engineController == null)
			{
				return VehicleEngineController<MiniCopter>.EngineState.Off;
			}
			return engineController.CurEngineState;
		}
	}

	public SamSite.SamTargetType SAMTargetType => SamSite.targetTypeVehicle;

	public float GetFuelFraction()
	{
		if (base.isServer)
		{
			cachedFuelFraction = Mathf.Clamp01((float)GetFuelSystem().GetFuelAmount() / fuelGaugeMax);
		}
		return cachedFuelFraction;
	}

	public override EntityFuelSystem GetFuelSystem()
	{
		return engineController.FuelSystem;
	}

	public override int StartingFuelUnits()
	{
		return 100;
	}

	protected override void OnChildAdded(BaseEntity child)
	{
		base.OnChildAdded(child);
		if (base.isServer && isSpawned)
		{
			GetFuelSystem().CheckNewChild(child);
		}
	}

	[RPC_Server]
	[RPC_Server.IsVisible(6f)]
	public void RPC_OpenFuel(RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if (!((Object)(object)player == (Object)null))
		{
			BasePlayer driver = GetDriver();
			if ((!((Object)(object)driver != (Object)null) || !((Object)(object)driver != (Object)(object)player)) && (!IsSafe() || !((Object)(object)player != (Object)(object)creatorEntity)))
			{
				engineController.FuelSystem.LootFuel(player);
			}
		}
	}

	public override void InitShared()
	{
		engineController = new VehicleEngineController<MiniCopter>(this, base.isServer, 5f, fuelStoragePrefab, waterSample, Flags.Reserved4);
	}

	public override float GetServiceCeiling()
	{
		return HotAirBalloon.serviceCeiling;
	}

	public bool IsValidSAMTarget(bool staticRespawn)
	{
		if (staticRespawn)
		{
			return true;
		}
		return !InSafeZone();
	}

	public override void PilotInput(InputState inputState, BasePlayer player)
	{
		base.PilotInput(inputState, player);
		if (!IsOn() && !IsStartingUp && inputState.IsDown(BUTTON.FORWARD) && !inputState.WasDown(BUTTON.FORWARD))
		{
			engineController.TryStartEngine(player);
		}
		currentInputState.groundControl = inputState.IsDown(BUTTON.DUCK);
		if (currentInputState.groundControl)
		{
			currentInputState.roll = 0f;
			currentInputState.throttle = (inputState.IsDown(BUTTON.FORWARD) ? 1f : 0f);
			currentInputState.throttle -= (inputState.IsDown(BUTTON.BACKWARD) ? 1f : 0f);
		}
		cachedRoll = currentInputState.roll;
		cachedYaw = currentInputState.yaw;
		cachedPitch = currentInputState.pitch;
	}

	public bool Grounded()
	{
		return leftWheel.isGrounded && rightWheel.isGrounded;
	}

	public override void SetDefaultInputState()
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		currentInputState.Reset();
		cachedRoll = 0f;
		cachedYaw = 0f;
		cachedPitch = 0f;
		if (Grounded())
		{
			return;
		}
		if (HasDriver())
		{
			float num = Vector3.Dot(Vector3.up, ((Component)this).transform.right);
			float num2 = Vector3.Dot(Vector3.up, ((Component)this).transform.forward);
			currentInputState.roll = ((num < 0f) ? 1f : 0f);
			currentInputState.roll -= ((num > 0f) ? 1f : 0f);
			if (num2 < -0f)
			{
				currentInputState.pitch = -1f;
			}
			else if (num2 > 0f)
			{
				currentInputState.pitch = 1f;
			}
		}
		else
		{
			currentInputState.throttle = -1f;
		}
	}

	private void ApplyForceAtWheels()
	{
		if (!((Object)(object)rigidBody == (Object)null))
		{
			float num = 50f;
			float num2 = 0f;
			float num3 = 0f;
			if (currentInputState.groundControl)
			{
				num = ((currentInputState.throttle == 0f) ? 50f : 0f);
				num2 = currentInputState.throttle;
				num3 = currentInputState.yaw;
			}
			else
			{
				num = 20f;
				num3 = 0f;
				num2 = 0f;
			}
			num2 *= (IsOn() ? 1f : 0f);
			if (isPushing)
			{
				num = 0f;
				num2 = 0.1f;
				num3 = 0f;
			}
			ApplyWheelForce(frontWheel, num2, num, num3);
			ApplyWheelForce(leftWheel, num2, num, 0f);
			ApplyWheelForce(rightWheel, num2, num, 0f);
		}
	}

	public void ApplyWheelForce(WheelCollider wheel, float gasScale, float brakeScale, float turning)
	{
		if (wheel.isGrounded)
		{
			float num = gasScale * motorForceConstant;
			float num2 = brakeScale * brakeForceConstant;
			float num3 = 45f * turning;
			if (!Mathf.Approximately(wheel.motorTorque, num))
			{
				wheel.motorTorque = num;
			}
			if (!Mathf.Approximately(wheel.brakeTorque, num2))
			{
				wheel.brakeTorque = num2;
			}
			if (!Mathf.Approximately(wheel.steerAngle, num3))
			{
				wheel.steerAngle = num3;
			}
		}
	}

	public override void MovementUpdate()
	{
		if (Grounded())
		{
			ApplyForceAtWheels();
		}
		if (IsOn() && (!currentInputState.groundControl || !Grounded()))
		{
			base.MovementUpdate();
		}
	}

	public override void ServerInit()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		base.ServerInit();
		lastEngineOnTime = Time.realtimeSinceStartup;
		rigidBody.inertiaTensor = rigidBody.inertiaTensor;
		preventBuildingObject.SetActive(true);
		((FacepunchBehaviour)this).InvokeRandomized((Action)UpdateNetwork, 0f, 0.2f, 0.05f);
		((FacepunchBehaviour)this).InvokeRandomized((Action)DecayTick, Random.Range(30f, 60f), 60f, 6f);
	}

	public void DecayTick()
	{
		if (base.healthFraction != 0f && !IsOn() && !(Time.time < lastEngineOnTime + 600f))
		{
			float num = 1f / (IsOutside() ? outsidedecayminutes : insidedecayminutes);
			Hurt(MaxHealth() * num, DamageType.Decay, this, useProtection: false);
		}
	}

	public override bool ShouldApplyHoverForce()
	{
		return IsOn();
	}

	public override bool IsEngineOn()
	{
		return IsOn();
	}

	public bool MeetsEngineRequirements()
	{
		if (engineController.IsOff)
		{
			return HasDriver();
		}
		return HasDriver() || Time.time <= lastPlayerInputTime + 1f;
	}

	public void OnEngineStartFailed()
	{
	}

	public override void OnFlagsChanged(Flags old, Flags next)
	{
		base.OnFlagsChanged(old, next);
		if (base.isServer && CurEngineState == VehicleEngineController<MiniCopter>.EngineState.Off)
		{
			lastEngineOnTime = Time.time;
		}
	}

	public override void VehicleFixedUpdate()
	{
		base.VehicleFixedUpdate();
		engineController.CheckEngineState();
		engineController.TickFuel(fuelPerSec);
	}

	public void UpdateNetwork()
	{
		Flags flags = base.flags;
		SetFlag(Flags.Reserved1, leftWheel.isGrounded, recursive: false, networkupdate: false);
		SetFlag(Flags.Reserved2, rightWheel.isGrounded, recursive: false, networkupdate: false);
		SetFlag(Flags.Reserved3, frontWheel.isGrounded, recursive: false, networkupdate: false);
		if (HasDriver())
		{
			SendNetworkUpdate();
		}
		else if (flags != base.flags)
		{
			SendNetworkUpdate_Flags();
		}
	}

	public void UpdateCOM()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		rigidBody.centerOfMass = com.localPosition;
	}

	public override void Save(SaveInfo info)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		base.Save(info);
		info.msg.miniCopter = Pool.Get<Minicopter>();
		info.msg.miniCopter.fuelStorageID = engineController.FuelSystem.fuelStorageInstance.uid;
		info.msg.miniCopter.fuelFraction = GetFuelFraction();
		info.msg.miniCopter.pitch = currentInputState.pitch;
		info.msg.miniCopter.roll = currentInputState.roll;
		info.msg.miniCopter.yaw = currentInputState.yaw;
	}

	public override void DismountAllPlayers()
	{
		foreach (MountPointInfo mountPoint in mountPoints)
		{
			if ((Object)(object)mountPoint.mountable != (Object)null)
			{
				BasePlayer mounted = mountPoint.mountable.GetMounted();
				if (Object.op_Implicit((Object)(object)mounted))
				{
					mounted.Hurt(10000f, DamageType.Explosion, this, useProtection: false);
				}
			}
		}
	}

	protected override void DoPushAction(BasePlayer player)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = Vector3Ex.Direction2D(((Component)player).transform.position, ((Component)this).transform.position);
		Vector3 val2 = player.eyes.BodyForward();
		val2.y = 0.25f;
		Vector3 val3 = ((Component)this).transform.position + val * 2f;
		float num = rigidBody.mass * 2f;
		rigidBody.AddForceAtPosition(val2 * num, val3, (ForceMode)1);
		rigidBody.AddForce(Vector3.up * 3f, (ForceMode)1);
		isPushing = true;
		((FacepunchBehaviour)this).Invoke((Action)DisablePushing, 0.5f);
	}

	private void DisablePushing()
	{
		isPushing = false;
	}

	public float RemapValue(float toUse, float maxRemap)
	{
		return toUse * maxRemap;
	}

	public override void Load(LoadInfo info)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		base.Load(info);
		if (info.msg.miniCopter != null)
		{
			engineController.FuelSystem.fuelStorageInstance.uid = info.msg.miniCopter.fuelStorageID;
			cachedFuelFraction = info.msg.miniCopter.fuelFraction;
			cachedPitch = RemapValue(info.msg.miniCopter.pitch, 0.5f);
			cachedRoll = RemapValue(info.msg.miniCopter.roll, 0.2f);
			cachedYaw = RemapValue(info.msg.miniCopter.yaw, 0.35f);
		}
	}

	protected override bool CanPushNow(BasePlayer pusher)
	{
		return base.CanPushNow(pusher) && pusher.IsOnGround() && !pusher.isMounted;
	}

	public override float InheritedVelocityScale()
	{
		return 1f;
	}

	public override bool InheritedVelocityDirection()
	{
		return false;
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("MiniCopter.OnRpcMessage", 0);
		try
		{
			if (rpc == 1851540757 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_OpenFuel "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_OpenFuel", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(1851540757u, "RPC_OpenFuel", this, player, 6f))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
					try
					{
						TimeWarning val4 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage msg2 = rPCMessage;
							RPC_OpenFuel(msg2);
						}
						finally
						{
							((IDisposable)val4)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in RPC_OpenFuel");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	void IEngineControllerUser.Invoke(Action action, float time)
	{
		((FacepunchBehaviour)this).Invoke(action, time);
	}

	void IEngineControllerUser.CancelInvoke(Action action)
	{
		((FacepunchBehaviour)this).CancelInvoke(action);
	}
}