using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

public class Snowmobile : GroundVehicle, CarPhysics<Snowmobile>.ICar, TriggerHurtNotChild.IHurtTriggerUser, VehicleChassisVisuals<Snowmobile>.IClientWheelUser, IPrefabPreProcess
{
	private CarPhysics<Snowmobile> carPhysics;

	private VehicleTerrainHandler serverTerrainHandler;

	private CarWheel[] wheels;

	private TimeSince timeSinceLastUsed;

	private const float DECAY_TICK_TIME = 60f;

	private float prevTerrainModDrag;

	private TimeSince timeSinceTerrainModCheck;

	[Header("Snowmobile")]
	[SerializeField]
	private Transform centreOfMassTransform;

	[SerializeField]
	private GameObjectRef itemStoragePrefab;

	[SerializeField]
	private VisualCarWheel wheelSkiFL;

	[SerializeField]
	private VisualCarWheel wheelSkiFR;

	[SerializeField]
	private VisualCarWheel wheelTreadFL;

	[SerializeField]
	private VisualCarWheel wheelTreadFR;

	[SerializeField]
	private VisualCarWheel wheelTreadRL;

	[SerializeField]
	private VisualCarWheel wheelTreadRR;

	[SerializeField]
	private CarSettings carSettings;

	[SerializeField]
	private int engineKW = 59;

	[SerializeField]
	private float idleFuelPerSec = 0.03f;

	[SerializeField]
	private float maxFuelPerSec = 0.15f;

	[SerializeField]
	private float airControlStability = 10f;

	[SerializeField]
	private float airControlPower = 40f;

	[SerializeField]
	private float badTerrainDrag = 1f;

	[SerializeField]
	private ProtectionProperties riderProtection;

	[SerializeField]
	private float hurtTriggerMinSpeed = 1f;

	[SerializeField]
	private TriggerHurtNotChild hurtTriggerFront;

	[SerializeField]
	private TriggerHurtNotChild hurtTriggerRear;

	[Header("Snowmobile Visuals")]
	public float minGroundFXSpeed;

	[SerializeField]
	private SnowmobileChassisVisuals chassisVisuals;

	[SerializeField]
	private VehicleLight[] lights;

	[SerializeField]
	private Transform steeringLeftIK;

	[SerializeField]
	private Transform steeringRightIK;

	[SerializeField]
	private Transform leftFootIK;

	[SerializeField]
	private Transform rightFootIK;

	[SerializeField]
	private Transform starterKey;

	[SerializeField]
	private Vector3 engineOffKeyRot;

	[SerializeField]
	private Vector3 engineOnKeyRot;

	[ServerVar(Help = "How long before a snowmobile loses all its health while outside")]
	public static float outsideDecayMinutes = 1440f;

	[ServerVar(Help = "Allow mounting as a passenger when there's no driver")]
	public static bool allowPassengerOnly = false;

	[ServerVar(Help = "If true, snowmobile goes fast on all terrain types")]
	public static bool allTerrain = false;

	private float _throttle;

	private float _brake;

	private float _steer;

	private float _mass = -1f;

	public const Flags Flag_Slowmode = Flags.Reserved8;

	private EntityRef<StorageContainer> itemStorageInstance;

	private float cachedFuelFraction;

	private const float FORCE_MULTIPLIER = 10f;

	public VehicleTerrainHandler.Surface OnSurface
	{
		get
		{
			if (serverTerrainHandler == null)
			{
				return VehicleTerrainHandler.Surface.Default;
			}
			return serverTerrainHandler.OnSurface;
		}
	}

	public float ThrottleInput
	{
		get
		{
			if (!engineController.IsOn)
			{
				return 0f;
			}
			return _throttle;
		}
		protected set
		{
			_throttle = Mathf.Clamp(value, -1f, 1f);
		}
	}

	public float BrakeInput
	{
		get
		{
			return _brake;
		}
		protected set
		{
			_brake = Mathf.Clamp(value, 0f, 1f);
		}
	}

	public bool IsBraking => BrakeInput > 0f;

	public float SteerInput
	{
		get
		{
			return _steer;
		}
		protected set
		{
			_steer = Mathf.Clamp(value, -1f, 1f);
		}
	}

	public float SteerAngle
	{
		get
		{
			if (base.isServer)
			{
				return carPhysics.SteerAngle;
			}
			return 0f;
		}
	}

	public override float DriveWheelVelocity
	{
		get
		{
			if (base.isServer)
			{
				return carPhysics.DriveWheelVelocity;
			}
			return 0f;
		}
	}

	public float DriveWheelSlip
	{
		get
		{
			if (base.isServer)
			{
				return carPhysics.DriveWheelSlip;
			}
			return 0f;
		}
	}

	public float MaxSteerAngle => carSettings.maxSteerAngle;

	public bool InSlowMode
	{
		get
		{
			return HasFlag(Flags.Reserved8);
		}
		private set
		{
			if (InSlowMode != value)
			{
				SetFlag(Flags.Reserved8, value);
			}
		}
	}

	private float Mass
	{
		get
		{
			if (base.isServer)
			{
				return rigidBody.mass;
			}
			return _mass;
		}
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("Snowmobile.OnRpcMessage", 0);
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
					TimeWarning val3 = TimeWarning.New("Call", 0);
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
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 924237371 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_OpenItemStorage "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_OpenItemStorage", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.MaxDistance.Test(924237371u, "RPC_OpenItemStorage", this, player, 3f))
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
							RPCMessage msg3 = rPCMessage;
							RPC_OpenItemStorage(msg3);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in RPC_OpenItemStorage");
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

	public override void ServerInit()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		base.ServerInit();
		timeSinceLastUsed = TimeSince.op_Implicit(0f);
		rigidBody.centerOfMass = centreOfMassTransform.localPosition;
		rigidBody.inertiaTensor = new Vector3(450f, 200f, 200f);
		carPhysics = new CarPhysics<Snowmobile>(this, ((Component)this).transform, rigidBody, carSettings);
		serverTerrainHandler = new VehicleTerrainHandler(this);
		((FacepunchBehaviour)this).InvokeRandomized((Action)UpdateClients, 0f, 0.15f, 0.02f);
		((FacepunchBehaviour)this).InvokeRandomized((Action)SnowmobileDecay, Random.Range(30f, 60f), 60f, 6f);
	}

	public override void VehicleFixedUpdate()
	{
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		base.VehicleFixedUpdate();
		float speed = GetSpeed();
		carPhysics.FixedUpdate(Time.fixedDeltaTime, speed);
		serverTerrainHandler.FixedUpdate();
		if (IsOn())
		{
			float fuelPerSecond = Mathf.Lerp(idleFuelPerSec, maxFuelPerSec, Mathf.Abs(ThrottleInput));
			engineController.TickFuel(fuelPerSecond);
		}
		engineController.CheckEngineState();
		RaycastHit val = default(RaycastHit);
		if (!carPhysics.IsGrounded() && Physics.Raycast(((Component)this).transform.position, Vector3.down, ref val, 10f, 1218511105, (QueryTriggerInteraction)1))
		{
			Vector3 normal = ((RaycastHit)(ref val)).normal;
			Vector3 right = ((Component)this).transform.right;
			right.y = 0f;
			normal = Vector3.ProjectOnPlane(normal, right);
			float num = Vector3.Angle(normal, Vector3.up);
			Vector3 angularVelocity = rigidBody.angularVelocity;
			float num2 = ((Vector3)(ref angularVelocity)).magnitude * 57.29578f * airControlStability / airControlPower;
			if (num <= 45f)
			{
				Vector3 val2 = Vector3.Cross(Quaternion.AngleAxis(num2, rigidBody.angularVelocity) * ((Component)this).transform.up, normal) * airControlPower * airControlPower;
				rigidBody.AddTorque(val2);
			}
		}
		((Component)hurtTriggerFront).gameObject.SetActive(speed > hurtTriggerMinSpeed);
		((Component)hurtTriggerRear).gameObject.SetActive(speed < 0f - hurtTriggerMinSpeed);
	}

	public override void PlayerServerInput(InputState inputState, BasePlayer player)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		if (!IsDriver(player))
		{
			return;
		}
		timeSinceLastUsed = TimeSince.op_Implicit(0f);
		if (inputState.IsDown(BUTTON.DUCK))
		{
			SteerInput += inputState.MouseDelta().x * 0.1f;
		}
		else
		{
			SteerInput = 0f;
			if (inputState.IsDown(BUTTON.LEFT))
			{
				SteerInput = -1f;
			}
			else if (inputState.IsDown(BUTTON.RIGHT))
			{
				SteerInput = 1f;
			}
		}
		float num = 0f;
		if (inputState.IsDown(BUTTON.FORWARD))
		{
			num = 1f;
		}
		else if (inputState.IsDown(BUTTON.BACKWARD))
		{
			num = -1f;
		}
		ThrottleInput = 0f;
		BrakeInput = 0f;
		if (GetSpeed() > 3f && num < -0.1f)
		{
			ThrottleInput = 0f;
			BrakeInput = 0f - num;
		}
		else
		{
			ThrottleInput = num;
			BrakeInput = 0f;
		}
		if (engineController.IsOff && ((inputState.IsDown(BUTTON.FORWARD) && !inputState.WasDown(BUTTON.FORWARD)) || (inputState.IsDown(BUTTON.BACKWARD) && !inputState.WasDown(BUTTON.BACKWARD))))
		{
			engineController.TryStartEngine(player);
		}
	}

	public float GetAdjustedDriveForce(float absSpeed, float topSpeed)
	{
		float maxDriveForce = GetMaxDriveForce();
		float num = Mathf.Lerp(0.3f, 0.75f, GetPerformanceFraction());
		float num2 = MathEx.BiasedLerp(1f - absSpeed / topSpeed, num);
		return maxDriveForce * num2;
	}

	public override float GetModifiedDrag()
	{
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		float num = base.GetModifiedDrag();
		if (!allTerrain)
		{
			VehicleTerrainHandler.Surface onSurface = serverTerrainHandler.OnSurface;
			if (serverTerrainHandler.IsGrounded && onSurface != VehicleTerrainHandler.Surface.Frictionless && onSurface != VehicleTerrainHandler.Surface.Sand && onSurface != VehicleTerrainHandler.Surface.Snow && onSurface != VehicleTerrainHandler.Surface.Ice)
			{
				float num2 = Mathf.Max(num, badTerrainDrag);
				num = (prevTerrainModDrag = ((!(num2 <= prevTerrainModDrag)) ? Mathf.MoveTowards(prevTerrainModDrag, num2, 0.33f * TimeSince.op_Implicit(timeSinceTerrainModCheck)) : prevTerrainModDrag));
			}
			else
			{
				prevTerrainModDrag = 0f;
			}
		}
		timeSinceTerrainModCheck = TimeSince.op_Implicit(0f);
		InSlowMode = num >= badTerrainDrag;
		return num;
	}

	public override float MaxVelocity()
	{
		return Mathf.Max(GetMaxForwardSpeed() * 1.3f, 30f);
	}

	public CarWheel[] GetWheels()
	{
		if (wheels == null)
		{
			wheels = new CarWheel[6] { wheelSkiFL, wheelSkiFR, wheelTreadFL, wheelTreadFR, wheelTreadRL, wheelTreadRR };
		}
		return wheels;
	}

	public float GetWheelsMidPos()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		return (((Component)wheelSkiFL.wheelCollider).transform.localPosition.z - ((Component)wheelTreadRL.wheelCollider).transform.localPosition.z) * 0.5f;
	}

	public override void Save(SaveInfo info)
	{
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		base.Save(info);
		info.msg.snowmobile = Pool.Get<Snowmobile>();
		info.msg.snowmobile.steerInput = SteerInput;
		info.msg.snowmobile.driveWheelVel = DriveWheelVelocity;
		info.msg.snowmobile.throttleInput = ThrottleInput;
		info.msg.snowmobile.brakeInput = BrakeInput;
		info.msg.snowmobile.storageID = itemStorageInstance.uid;
		info.msg.snowmobile.fuelStorageID = GetFuelSystem().fuelStorageInstance.uid;
	}

	public override int StartingFuelUnits()
	{
		return 0;
	}

	protected override void OnChildAdded(BaseEntity child)
	{
		base.OnChildAdded(child);
		if (base.isServer && isSpawned && child.prefabID == itemStoragePrefab.GetEntity().prefabID)
		{
			itemStorageInstance.Set((StorageContainer)child);
		}
	}

	internal override void DoServerDestroy()
	{
		if (vehicle.vehiclesdroploot)
		{
			StorageContainer storageContainer = itemStorageInstance.Get(base.isServer);
			if ((Object)(object)storageContainer != (Object)null && storageContainer.IsValid())
			{
				storageContainer.DropItems();
			}
		}
		base.DoServerDestroy();
	}

	public override bool MeetsEngineRequirements()
	{
		return HasDriver();
	}

	public override void AttemptMount(BasePlayer player, bool doMountChecks = true)
	{
		if (allowPassengerOnly)
		{
			base.AttemptMount(player, doMountChecks);
		}
		else if (MountEligable(player))
		{
			BaseMountable baseMountable = (HasDriver() ? GetIdealMountPointFor(player) : mountPoints[0].mountable);
			if ((Object)(object)baseMountable != (Object)null)
			{
				baseMountable.AttemptMount(player, doMountChecks);
			}
			if (PlayerIsMounted(player))
			{
				PlayerMounted(player, baseMountable);
			}
		}
	}

	public void SnowmobileDecay()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		if (!IsDead() && !(TimeSince.op_Implicit(timeSinceLastUsed) < 2700f))
		{
			float num = (IsOutside() ? outsideDecayMinutes : float.PositiveInfinity);
			if (!float.IsPositiveInfinity(num))
			{
				float num2 = 1f / num;
				Hurt(MaxHealth() * num2, DamageType.Decay, this, useProtection: false);
			}
		}
	}

	public StorageContainer GetItemContainer()
	{
		BaseEntity baseEntity = itemStorageInstance.Get(base.isServer);
		if ((Object)(object)baseEntity != (Object)null && baseEntity.IsValid())
		{
			return baseEntity as StorageContainer;
		}
		return null;
	}

	private void UpdateClients()
	{
		if (HasDriver())
		{
			byte num = (byte)((ThrottleInput + 1f) * 7f);
			byte b = (byte)(BrakeInput * 15f);
			byte arg = (byte)(num + (b << 4));
			ClientRPC(null, "SnowmobileUpdate", SteerInput, arg, DriveWheelVelocity, GetFuelFraction());
		}
	}

	public override void OnEngineStartFailed()
	{
		ClientRPC(null, "EngineStartFailed");
	}

	public override void ScaleDamageForPlayer(BasePlayer player, HitInfo info)
	{
		base.ScaleDamageForPlayer(player, info);
		riderProtection.Scale(info.damageTypes);
	}

	[RPC_Server]
	public void RPC_OpenFuel(RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if (CanBeLooted(player) && IsDriver(player))
		{
			GetFuelSystem().LootFuel(player);
		}
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	public void RPC_OpenItemStorage(RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if (CanBeLooted(player))
		{
			StorageContainer itemContainer = GetItemContainer();
			if ((Object)(object)itemContainer != (Object)null)
			{
				itemContainer.PlayerOpenLoot(player);
			}
		}
	}

	public override void Load(LoadInfo info)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		base.Load(info);
		if (info.msg.snowmobile != null)
		{
			itemStorageInstance.uid = info.msg.snowmobile.storageID;
			engineController.FuelSystem.fuelStorageInstance.uid = info.msg.snowmobile.fuelStorageID;
			cachedFuelFraction = info.msg.snowmobile.fuelFraction;
		}
	}

	public float GetMaxDriveForce()
	{
		return (float)engineKW * 10f * GetPerformanceFraction();
	}

	public override float GetMaxForwardSpeed()
	{
		return GetMaxDriveForce() / Mass * 15f;
	}

	public override float GetThrottleInput()
	{
		return ThrottleInput;
	}

	public override float GetBrakeInput()
	{
		return BrakeInput;
	}

	public float GetSteerInput()
	{
		return SteerInput;
	}

	public bool GetSteerModInput()
	{
		return false;
	}

	public float GetPerformanceFraction()
	{
		float num = Mathf.InverseLerp(0.25f, 0.5f, base.healthFraction);
		return Mathf.Lerp(0.5f, 1f, num);
	}

	public float GetFuelFraction()
	{
		if (base.isServer)
		{
			return engineController.FuelSystem.GetFuelFraction();
		}
		return cachedFuelFraction;
	}

	public override bool CanBeLooted(BasePlayer player)
	{
		if (!base.CanBeLooted(player))
		{
			return false;
		}
		if (!PlayerIsMounted(player))
		{
			return !IsOn();
		}
		return true;
	}

	public override void OnFlagsChanged(Flags old, Flags next)
	{
		base.OnFlagsChanged(old, next);
		if (base.isServer && GameInfo.HasAchievements && !old.HasFlag(Flags.On) && next.HasFlag(Flags.On))
		{
			BasePlayer driver = GetDriver();
			if ((Object)(object)driver != (Object)null && (Object)(object)driver.FindTrigger<TriggerSnowmobileAchievement>() != (Object)null)
			{
				driver.GiveAchievement("DRIVE_SNOWMOBILE");
			}
		}
	}
}
