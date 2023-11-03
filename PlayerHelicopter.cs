using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

public class PlayerHelicopter : BaseHelicopter, IEngineControllerUser, IEntity, SamSite.ISamSiteTarget
{
	[Serializable]
	private class Wheel
	{
		public WheelCollider wheelCollider;

		public Transform visualBone;

		public bool steering;

		public bool groundedTest = true;

		public Flags groundedFlag = Flags.Reserved1;

		[NonSerialized]
		public float wheelVel;

		[NonSerialized]
		public Vector3 wheelRot = Vector3.zero;

		public bool IsGrounded(PlayerHelicopter parent)
		{
			if (parent.isServer)
			{
				return wheelCollider.isGrounded;
			}
			return parent.HasFlag(groundedFlag);
		}
	}

	[Header("Player Helicopter")]
	[SerializeField]
	private Wheel[] wheels;

	[SerializeField]
	private Transform waterSample;

	public PlayerHeliSounds playerHeliSounds;

	[SerializeField]
	private Transform joystickPositionLeft;

	[SerializeField]
	private Transform joystickPositionRight;

	[SerializeField]
	private Transform leftFootPosition;

	[SerializeField]
	private Transform rightFootPosition;

	[SerializeField]
	private Transform passengerJoystickPositionRight;

	[SerializeField]
	protected Animator animator;

	[SerializeField]
	private float maxRotorSpeed = 10f;

	[SerializeField]
	private float timeUntilMaxRotorSpeed = 7f;

	[SerializeField]
	private float rotorBlurThreshold = 8f;

	[SerializeField]
	private Transform mainRotorBlurBone;

	[SerializeField]
	private Renderer mainRotorBlurMesh;

	[SerializeField]
	private Transform mainRotorBladesBone;

	[SerializeField]
	private Renderer[] mainRotorBladeMeshes;

	[SerializeField]
	private Transform rearRotorBladesBone;

	[SerializeField]
	private Renderer[] rearRotorBladeMeshes;

	[SerializeField]
	private Transform rearRotorBlurBone;

	[SerializeField]
	private Renderer rearRotorBlurMesh;

	[SerializeField]
	private float motorForceConstant = 150f;

	[SerializeField]
	private float brakeForceConstant = 500f;

	[SerializeField]
	private GameObject preventBuildingObject;

	[SerializeField]
	private float maxPitchAnim = 1f;

	[SerializeField]
	private float maxRollAnim = 1f;

	[SerializeField]
	private float maxYawAnim = 1f;

	[Header("Fuel")]
	[SerializeField]
	private GameObjectRef fuelStoragePrefab;

	[SerializeField]
	private float fuelPerSec = 0.25f;

	[SerializeField]
	private float fuelGaugeMax = 100f;

	[ServerVar(Help = "How long before a player helicopter loses all its health while outside")]
	public static float outsidedecayminutes = 480f;

	[ServerVar(Help = "How long before a player helicopter loses all its health while indoors")]
	public static float insidedecayminutes = 2880f;

	protected VehicleEngineController<PlayerHelicopter> engineController;

	protected const Flags WHEEL_GROUNDED_LR = Flags.Reserved1;

	protected const Flags WHEEL_GROUNDED_RR = Flags.Reserved2;

	protected const Flags WHEEL_GROUNDED_FRONT = Flags.Reserved3;

	protected const Flags RADAR_WARNING_FLAG = Flags.Reserved12;

	protected const Flags RADAR_LOCK_FLAG = Flags.Reserved13;

	private TimeSince timeSinceCachedFuelFraction;

	private float cachedFuelFraction;

	private bool isPushing;

	private float lastEngineOnTime;

	public bool IsStartingUp
	{
		get
		{
			if (engineController != null)
			{
				return engineController.IsStarting;
			}
			return false;
		}
	}

	public VehicleEngineController<PlayerHelicopter>.EngineState CurEngineState
	{
		get
		{
			if (engineController == null)
			{
				return VehicleEngineController<PlayerHelicopter>.EngineState.Off;
			}
			return engineController.CurEngineState;
		}
	}

	public float cachedPitch { get; private set; }

	public float cachedYaw { get; private set; }

	public float cachedRoll { get; private set; }

	public SamSite.SamTargetType SAMTargetType => SamSite.targetTypeVehicle;

	protected override bool ForceMovementHandling
	{
		get
		{
			if (isPushing)
			{
				return wheels.Length != 0;
			}
			return false;
		}
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("PlayerHelicopter.OnRpcMessage", 0);
		try
		{
			if (rpc == 1851540757 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - RPC_OpenFuel "));
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
						val3 = TimeWarning.New("Call", 0);
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
							((IDisposable)val3)?.Dispose();
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

	public override void InitShared()
	{
		base.InitShared();
		engineController = new VehicleEngineController<PlayerHelicopter>(this, base.isServer, 5f, fuelStoragePrefab, waterSample, Flags.Reserved4);
	}

	public float GetFuelFraction(bool force = false)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		if (base.isServer && (TimeSince.op_Implicit(timeSinceCachedFuelFraction) > 1f || force))
		{
			cachedFuelFraction = Mathf.Clamp01((float)GetFuelSystem().GetFuelAmount() / fuelGaugeMax);
			timeSinceCachedFuelFraction = TimeSince.op_Implicit(0f);
		}
		return cachedFuelFraction;
	}

	protected override bool CanPushNow(BasePlayer pusher)
	{
		if (base.CanPushNow(pusher) && pusher.IsOnGround())
		{
			return !pusher.isMounted;
		}
		return false;
	}

	public override float InheritedVelocityScale()
	{
		return 1f;
	}

	public override bool InheritedVelocityDirection()
	{
		return false;
	}

	public override void Load(LoadInfo info)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		base.Load(info);
		if (info.msg.miniCopter != null)
		{
			engineController.FuelSystem.fuelStorageInstance.uid = info.msg.miniCopter.fuelStorageID;
			cachedFuelFraction = info.msg.miniCopter.fuelFraction;
			cachedPitch = info.msg.miniCopter.pitch * maxPitchAnim;
			cachedRoll = info.msg.miniCopter.roll * maxRollAnim;
			cachedYaw = info.msg.miniCopter.yaw * maxYawAnim;
		}
	}

	public override void OnFlagsChanged(Flags old, Flags next)
	{
		base.OnFlagsChanged(old, next);
		if (base.isServer && CurEngineState == VehicleEngineController<PlayerHelicopter>.EngineState.Off)
		{
			lastEngineOnTime = Time.time;
		}
	}

	protected override void OnChildAdded(BaseEntity child)
	{
		base.OnChildAdded(child);
		if (base.isServer && isSpawned)
		{
			GetFuelSystem().CheckNewChild(child);
		}
	}

	public override float GetServiceCeiling()
	{
		return HotAirBalloon.serviceCeiling;
	}

	public override EntityFuelSystem GetFuelSystem()
	{
		return engineController.FuelSystem;
	}

	public override int StartingFuelUnits()
	{
		return 100;
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
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		if (wheels.Length == 0)
		{
			return Physics.Raycast(((Component)this).transform.position + Vector3.up * 0.1f, Vector3.down, 0.5f);
		}
		Wheel[] array = wheels;
		foreach (Wheel wheel in array)
		{
			if (wheel.groundedTest && !wheel.wheelCollider.isGrounded)
			{
				return false;
			}
		}
		return true;
	}

	public override void SetDefaultInputState()
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
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
			float brakeScale;
			float num2;
			float num;
			if (currentInputState.groundControl)
			{
				brakeScale = ((currentInputState.throttle == 0f) ? 50f : 0f);
				num = currentInputState.throttle;
				num2 = currentInputState.yaw;
			}
			else
			{
				brakeScale = 20f;
				num2 = 0f;
				num = 0f;
			}
			num *= (IsOn() ? 1f : 0f);
			if (isPushing)
			{
				brakeScale = 0f;
				num = 0.1f;
				num2 = 0f;
			}
			Wheel[] array = wheels;
			foreach (Wheel wheel in array)
			{
				ApplyWheelForce(wheel.wheelCollider, num, brakeScale, wheel.steering ? num2 : 0f);
			}
		}
	}

	private void ApplyForceWithoutWheels()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		if (currentInputState.groundControl)
		{
			if (currentInputState.throttle != 0f)
			{
				rigidBody.AddRelativeForce(Vector3.forward * currentInputState.throttle * motorForceConstant * 15f, (ForceMode)0);
			}
			if (currentInputState.yaw != 0f)
			{
				rigidBody.AddRelativeTorque(new Vector3(0f, currentInputState.yaw * torqueScale.y, 0f), (ForceMode)0);
			}
			float num = rigidBody.mass * (0f - Physics.gravity.y);
			rigidBody.AddForce(((Component)this).transform.up * num * hoverForceScale, (ForceMode)0);
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
			if (wheels.Length != 0)
			{
				ApplyForceAtWheels();
			}
			else
			{
				ApplyForceWithoutWheels();
			}
		}
		if (!currentInputState.groundControl || !Grounded())
		{
			base.MovementUpdate();
		}
	}

	public override void ServerInit()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
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

	public override bool IsEngineOn()
	{
		return IsOn();
	}

	protected override void TryStartEngine(BasePlayer player)
	{
		engineController.TryStartEngine(player);
	}

	public bool MeetsEngineRequirements()
	{
		if (base.autoHover)
		{
			return true;
		}
		if (engineController.IsOff)
		{
			return HasDriver();
		}
		if (!HasDriver())
		{
			return Time.time <= lastPlayerInputTime + 1f;
		}
		return true;
	}

	public void OnEngineStartFailed()
	{
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
		Wheel[] array = wheels;
		foreach (Wheel wheel in array)
		{
			SetFlag(wheel.groundedFlag, wheel.wheelCollider.isGrounded, recursive: false, networkupdate: false);
		}
		if (HasDriver())
		{
			SendNetworkUpdate();
		}
		else if (flags != base.flags)
		{
			SendNetworkUpdate_Flags();
		}
	}

	public override void OnEntityMessage(BaseEntity from, string msg)
	{
		if (msg == "RadarLock")
		{
			SetFlag(Flags.Reserved13, b: true);
			((FacepunchBehaviour)this).Invoke((Action)ClearRadarLock, 1f);
		}
		else if (msg == "RadarWarning")
		{
			SetFlag(Flags.Reserved12, b: true);
			((FacepunchBehaviour)this).Invoke((Action)ClearRadarWarning, 1f);
		}
		else
		{
			base.OnEntityMessage(from, msg);
		}
	}

	private void ClearRadarLock()
	{
		SetFlag(Flags.Reserved13, b: false);
	}

	private void ClearRadarWarning()
	{
		SetFlag(Flags.Reserved12, b: false);
	}

	public void UpdateCOM()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		rigidBody.centerOfMass = com.localPosition;
	}

	public override void Save(SaveInfo info)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		base.Save(info);
		info.msg.miniCopter = Pool.Get<Minicopter>();
		info.msg.miniCopter.fuelStorageID = engineController.FuelSystem.fuelStorageInstance.uid;
		info.msg.miniCopter.fuelFraction = GetFuelFraction(force: true);
		info.msg.miniCopter.pitch = currentInputState.pitch;
		info.msg.miniCopter.roll = currentInputState.roll;
		info.msg.miniCopter.yaw = currentInputState.yaw;
	}

	public override void OnKilled(HitInfo info)
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		foreach (MountPointInfo mountPoint in mountPoints)
		{
			if ((Object)(object)mountPoint.mountable != (Object)null)
			{
				BasePlayer mounted = mountPoint.mountable.GetMounted();
				if (Object.op_Implicit((Object)(object)mounted))
				{
					HitInfo hitInfo = new HitInfo(info.Initiator, this, DamageType.Explosion, 1000f, ((Component)this).transform.position);
					hitInfo.Weapon = info.Weapon;
					hitInfo.WeaponPrefab = info.WeaponPrefab;
					mounted.Hurt(hitInfo);
				}
			}
		}
		base.OnKilled(info);
	}

	protected override void DoPushAction(BasePlayer player)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
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
		Wheel[] array = wheels;
		foreach (Wheel wheel in array)
		{
			ApplyWheelForce(wheel.wheelCollider, 0f, 20f, 0f);
		}
	}

	public override bool IsValidHomingTarget()
	{
		return IsOn();
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

	void IEngineControllerUser.Invoke(Action action, float time)
	{
		((FacepunchBehaviour)this).Invoke(action, time);
	}

	void IEngineControllerUser.CancelInvoke(Action action)
	{
		((FacepunchBehaviour)this).CancelInvoke(action);
	}
}
