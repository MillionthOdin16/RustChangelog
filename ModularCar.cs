using System;
using System.Collections.Generic;
using System.Linq;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using Rust.Modular;
using UnityEngine;
using UnityEngine.Assertions;

public class ModularCar : BaseModularVehicle, IVehicleLockUser, VehicleChassisVisuals<ModularCar>.IClientWheelUser, TakeCollisionDamage.ICanRestoreVelocity, CarPhysics<ModularCar>.ICar
{
	[Serializable]
	public class SpawnSettings
	{
		public enum AdminBonus
		{
			None,
			T1PlusFuel,
			T2PlusFuel,
			T3PlusFuel
		}

		[Tooltip("Must be true to use any of these settings.")]
		public bool useSpawnSettings;

		[Tooltip("Specify a list of possible module configurations that'll automatically spawn with this vehicle.")]
		public ModularCarPresetConfig[] configurationOptions;

		[Tooltip("Min health % at spawn for any modules that spawn with this chassis.")]
		public float minStartHealthPercent = 0.15f;

		[Tooltip("Max health  % at spawn for any modules that spawn with this chassis.")]
		public float maxStartHealthPercent = 0.5f;

		public AdminBonus adminBonus;
	}

	private class DriverSeatInputs
	{
		public float steerInput;

		public bool steerMod;

		public float brakeInput;

		public float throttleInput;
	}

	[Header("Modular Car")]
	public ModularCarChassisVisuals chassisVisuals;

	public VisualCarWheel wheelFL;

	public VisualCarWheel wheelFR;

	public VisualCarWheel wheelRL;

	public VisualCarWheel wheelRR;

	[SerializeField]
	private CarSettings carSettings;

	[SerializeField]
	private float hurtTriggerMinSpeed = 1f;

	[SerializeField]
	private TriggerHurtNotChild hurtTriggerFront;

	[SerializeField]
	private TriggerHurtNotChild hurtTriggerRear;

	[SerializeField]
	private ProtectionProperties immortalProtection;

	[SerializeField]
	private ProtectionProperties mortalProtection;

	[SerializeField]
	private BoxCollider mainChassisCollider;

	[SerializeField]
	private SpawnSettings spawnSettings;

	[SerializeField]
	[HideInInspector]
	private MeshRenderer[] damageShowingRenderers;

	[ServerVar(Help = "Population active on the server", ShowInAdminUI = true)]
	public static float population = 3f;

	[ServerVar(Help = "How many minutes before a ModularCar loses all its health while outside")]
	public static float outsidedecayminutes = 864f;

	public const BUTTON RapidSteerButton = BUTTON.SPRINT;

	private VehicleEngineController<GroundVehicle>.EngineState lastSetEngineState;

	private float cachedFuelFraction;

	public static HashSet<ModularCar> allCarsList = new HashSet<ModularCar>();

	private readonly ListDictionary<BaseMountable, DriverSeatInputs> driverSeatInputs = new ListDictionary<BaseMountable, DriverSeatInputs>();

	private CarPhysics<ModularCar> carPhysics;

	private VehicleTerrainHandler serverTerrainHandler;

	private CarWheel[] wheels;

	private float lastEngineOnTime;

	private const float DECAY_TICK_TIME = 60f;

	private const float INSIDE_DECAY_MULTIPLIER = 0.1f;

	private const float CORPSE_DECAY_MINUTES = 5f;

	private Vector3 prevPosition;

	private Quaternion prevRotation;

	private Bounds collisionCheckBounds;

	private Vector3 lastGoodPos;

	private Quaternion lastGoodRot;

	private bool lastPosWasBad;

	private float deathDamageCounter;

	private const float DAMAGE_TO_GIB = 600f;

	private TimeSince timeSinceDeath;

	private const float IMMUNE_TIME = 1f;

	protected readonly Vector3 groundedCOMMultiplier = new Vector3(0.25f, 0.3f, 0.25f);

	protected readonly Vector3 airbourneCOMMultiplier = new Vector3(0.25f, 0.75f, 0.25f);

	private Vector3 prevCOMMultiplier;

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

	public ItemDefinition AssociatedItemDef => repair.itemTarget;

	public float MaxSteerAngle => carSettings.maxSteerAngle;

	public override bool IsLockable => CarLock.HasALock;

	public ModularCarCodeLock CarLock { get; private set; }

	public override bool AlwaysAllowBradleyTargeting => true;

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

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("ModularCar.OnRpcMessage", 0);
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
			if (rpc == 1382140449 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - RPC_OpenFuelWithKeycode "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_OpenFuelWithKeycode", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Call", 0);
					try
					{
						RPCMessage rPCMessage = default(RPCMessage);
						rPCMessage.connection = msg.connection;
						rPCMessage.player = player;
						rPCMessage.read = msg.read;
						RPCMessage msg3 = rPCMessage;
						RPC_OpenFuelWithKeycode(msg3);
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
				}
				catch (Exception ex2)
				{
					Debug.LogException(ex2);
					player.Kick("RPC Error in RPC_OpenFuelWithKeycode");
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 2818660542u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - RPC_TryMountWithKeycode "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_TryMountWithKeycode", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Call", 0);
					try
					{
						RPCMessage rPCMessage = default(RPCMessage);
						rPCMessage.connection = msg.connection;
						rPCMessage.player = player;
						rPCMessage.read = msg.read;
						RPCMessage msg4 = rPCMessage;
						RPC_TryMountWithKeycode(msg4);
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
				}
				catch (Exception ex3)
				{
					Debug.LogException(ex3);
					player.Kick("RPC Error in RPC_TryMountWithKeycode");
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

	public override void PreProcess(IPrefabProcessor process, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		base.PreProcess(process, rootObj, name, serverside, clientside, bundling);
		damageShowingRenderers = ((Component)this).GetComponentsInChildren<MeshRenderer>();
	}

	public override void InitShared()
	{
		base.InitShared();
		if (CarLock == null)
		{
			CarLock = new ModularCarCodeLock(this, base.isServer);
		}
	}

	public override float MaxHealth()
	{
		return AssociatedItemDef.condition.max;
	}

	public override float StartHealth()
	{
		return AssociatedItemDef.condition.max;
	}

	public float TotalHealth()
	{
		float num = 0f;
		for (int i = 0; i < base.AttachedModuleEntities.Count; i++)
		{
			num += base.AttachedModuleEntities[i].Health();
		}
		return num;
	}

	public float TotalMaxHealth()
	{
		float num = 0f;
		for (int i = 0; i < base.AttachedModuleEntities.Count; i++)
		{
			num += base.AttachedModuleEntities[i].MaxHealth();
		}
		return num;
	}

	public override float GetMaxForwardSpeed()
	{
		float num = GetMaxDriveForce() / base.TotalMass * 30f;
		return Mathf.Pow(0.9945f, num) * num;
	}

	public override void Load(LoadInfo info)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		base.Load(info);
		if (info.msg.modularCar == null)
		{
			return;
		}
		engineController.FuelSystem.fuelStorageInstance.uid = info.msg.modularCar.fuelStorageID;
		cachedFuelFraction = info.msg.modularCar.fuelFraction;
		bool hasALock = CarLock.HasALock;
		CarLock.Load(info);
		if (CarLock.HasALock != hasALock)
		{
			for (int i = 0; i < base.AttachedModuleEntities.Count; i++)
			{
				base.AttachedModuleEntities[i].RefreshConditionals(canGib: true);
			}
		}
	}

	public override void OnFlagsChanged(Flags old, Flags next)
	{
		base.OnFlagsChanged(old, next);
		if (old != next)
		{
			RefreshEngineState();
		}
	}

	public override float GetThrottleInput()
	{
		if (base.isServer)
		{
			float num = 0f;
			BufferList<DriverSeatInputs> values = driverSeatInputs.Values;
			for (int i = 0; i < values.Count; i++)
			{
				num += values[i].throttleInput;
			}
			return Mathf.Clamp(num, -1f, 1f);
		}
		return 0f;
	}

	public override float GetBrakeInput()
	{
		if (base.isServer)
		{
			float num = 0f;
			BufferList<DriverSeatInputs> values = driverSeatInputs.Values;
			for (int i = 0; i < values.Count; i++)
			{
				num += values[i].brakeInput;
			}
			return Mathf.Clamp01(num);
		}
		return 0f;
	}

	public float GetMaxDriveForce()
	{
		float num = 0f;
		for (int i = 0; i < base.AttachedModuleEntities.Count; i++)
		{
			num += base.AttachedModuleEntities[i].GetMaxDriveForce();
		}
		return RollOffDriveForce(num);
	}

	public float GetFuelFraction()
	{
		if (base.isServer)
		{
			return engineController.FuelSystem.GetFuelFraction();
		}
		return cachedFuelFraction;
	}

	public bool PlayerHasUnlockPermission(BasePlayer player)
	{
		return CarLock.HasLockPermission(player);
	}

	public bool KeycodeEntryBlocked(BasePlayer player)
	{
		return CarLock.CodeEntryBlocked(player);
	}

	public override bool PlayerCanUseThis(BasePlayer player, ModularCarCodeLock.LockType lockType)
	{
		return CarLock.PlayerCanUseThis(player, lockType);
	}

	public bool PlayerCanDestroyLock(BasePlayer player, BaseVehicleModule viaModule)
	{
		return CarLock.PlayerCanDestroyLock(viaModule);
	}

	public override bool CanBeLooted(BasePlayer player)
	{
		if ((Object)(object)player == (Object)null)
		{
			return false;
		}
		if (PlayerIsMounted(player))
		{
			return true;
		}
		if (!PlayerCanUseThis(player, ModularCarCodeLock.LockType.General))
		{
			return false;
		}
		if (!IsOn())
		{
			return base.CanBeLooted(player);
		}
		return false;
	}

	protected override bool CanPushNow(BasePlayer pusher)
	{
		if (!base.CanPushNow(pusher))
		{
			return false;
		}
		if (pusher.InSafeZone() && !CarLock.HasLockPermission(pusher))
		{
			return false;
		}
		return true;
	}

	protected bool RefreshEngineState()
	{
		if (lastSetEngineState == base.CurEngineState)
		{
			return false;
		}
		if (base.isServer && base.CurEngineState == VehicleEngineController<GroundVehicle>.EngineState.Off)
		{
			lastEngineOnTime = Time.time;
		}
		foreach (BaseVehicleModule attachedModuleEntity in base.AttachedModuleEntities)
		{
			attachedModuleEntity.OnEngineStateChanged(lastSetEngineState, base.CurEngineState);
		}
		if (base.isServer && GameInfo.HasAchievements && NumMounted() >= 5)
		{
			foreach (MountPointInfo allMountPoint in base.allMountPoints)
			{
				if ((Object)(object)allMountPoint.mountable != (Object)null && (Object)(object)allMountPoint.mountable.GetMounted() != (Object)null)
				{
					allMountPoint.mountable.GetMounted().GiveAchievement("BATTLE_BUS");
				}
			}
		}
		lastSetEngineState = base.CurEngineState;
		return true;
	}

	private float RollOffDriveForce(float driveForce)
	{
		return Mathf.Pow(0.9999175f, driveForce) * driveForce;
	}

	private void RefreshChassisProtectionState()
	{
		if (base.HasAnyModules)
		{
			baseProtection = immortalProtection;
			if (base.isServer)
			{
				SetHealth(MaxHealth());
			}
		}
		else
		{
			baseProtection = mortalProtection;
		}
	}

	protected override void ModuleEntityAdded(BaseVehicleModule addedModule)
	{
		base.ModuleEntityAdded(addedModule);
		RefreshChassisProtectionState();
	}

	protected override void ModuleEntityRemoved(BaseVehicleModule removedModule)
	{
		base.ModuleEntityRemoved(removedModule);
		RefreshChassisProtectionState();
	}

	public override void ServerInit()
	{
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		base.ServerInit();
		carPhysics = new CarPhysics<ModularCar>(this, ((Component)this).transform, rigidBody, carSettings);
		serverTerrainHandler = new VehicleTerrainHandler(this);
		if (!Application.isLoadingSave)
		{
			SpawnPreassignedModules();
		}
		lastEngineOnTime = Time.realtimeSinceStartup;
		allCarsList.Add(this);
		collisionCheckBounds = new Bounds(mainChassisCollider.center, new Vector3(mainChassisCollider.size.x - 0.5f, 0.05f, mainChassisCollider.size.z - 0.5f));
		((FacepunchBehaviour)this).InvokeRandomized((Action)UpdateClients, 0f, 0.15f, 0.02f);
		((FacepunchBehaviour)this).InvokeRandomized((Action)DecayTick, Random.Range(30f, 60f), 60f, 6f);
	}

	internal override void DoServerDestroy()
	{
		base.DoServerDestroy();
		allCarsList.Remove(this);
	}

	public override void PostServerLoad()
	{
		base.PostServerLoad();
		CarLock.PostServerLoad();
		if (IsDead())
		{
			Kill();
		}
	}

	public float GetSteerInput()
	{
		float num = 0f;
		BufferList<DriverSeatInputs> values = driverSeatInputs.Values;
		for (int i = 0; i < values.Count; i++)
		{
			num += values[i].steerInput;
		}
		return Mathf.Clamp(num, -1f, 1f);
	}

	public bool GetSteerModInput()
	{
		BufferList<DriverSeatInputs> values = driverSeatInputs.Values;
		for (int i = 0; i < values.Count; i++)
		{
			if (values[i].steerMod)
			{
				return true;
			}
		}
		return false;
	}

	public override void VehicleFixedUpdate()
	{
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		base.VehicleFixedUpdate();
		float speed = GetSpeed();
		carPhysics.FixedUpdate(Time.fixedDeltaTime, speed);
		engineController.CheckEngineState();
		((Component)hurtTriggerFront).gameObject.SetActive(speed > hurtTriggerMinSpeed);
		((Component)hurtTriggerRear).gameObject.SetActive(speed < 0f - hurtTriggerMinSpeed);
		serverTerrainHandler.FixedUpdate();
		float num = Mathf.Abs(speed);
		if (lastPosWasBad || num > 15f)
		{
			if (GamePhysics.CheckOBB(new OBB(((Component)mainChassisCollider).transform, collisionCheckBounds), 1084293377, (QueryTriggerInteraction)1))
			{
				rigidBody.position = lastGoodPos;
				rigidBody.rotation = lastGoodRot;
				((Component)this).transform.position = lastGoodPos;
				((Component)this).transform.rotation = lastGoodRot;
				rigidBody.velocity = Vector3.zero;
				rigidBody.angularVelocity = Vector3.zero;
				lastPosWasBad = true;
			}
			else
			{
				lastGoodPos = rigidBody.position;
				lastGoodRot = rigidBody.rotation;
				lastPosWasBad = false;
			}
		}
		else
		{
			lastGoodPos = rigidBody.position;
			lastGoodRot = rigidBody.rotation;
			lastPosWasBad = false;
		}
		if (IsMoving())
		{
			Vector3 cOMMultiplier = GetCOMMultiplier();
			if (cOMMultiplier != prevCOMMultiplier)
			{
				rigidBody.centerOfMass = Vector3.Scale(realLocalCOM, cOMMultiplier);
				prevCOMMultiplier = cOMMultiplier;
			}
		}
	}

	protected override bool DetermineIfStationary()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		bool result = rigidBody.position == prevPosition && rigidBody.rotation == prevRotation;
		prevPosition = rigidBody.position;
		prevRotation = rigidBody.rotation;
		return result;
	}

	public override void PlayerServerInput(InputState inputState, BasePlayer player)
	{
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		MountPointInfo playerSeatInfo = GetPlayerSeatInfo(player);
		if (playerSeatInfo == null || !playerSeatInfo.isDriver)
		{
			return;
		}
		if (!this.driverSeatInputs.Contains(playerSeatInfo.mountable))
		{
			this.driverSeatInputs.Add(playerSeatInfo.mountable, new DriverSeatInputs());
		}
		DriverSeatInputs driverSeatInputs = this.driverSeatInputs[playerSeatInfo.mountable];
		if (inputState.IsDown(BUTTON.DUCK))
		{
			driverSeatInputs.steerInput += inputState.MouseDelta().x * 0.1f;
		}
		else
		{
			driverSeatInputs.steerInput = 0f;
			if (inputState.IsDown(BUTTON.LEFT))
			{
				driverSeatInputs.steerInput = -1f;
			}
			else if (inputState.IsDown(BUTTON.RIGHT))
			{
				driverSeatInputs.steerInput = 1f;
			}
		}
		driverSeatInputs.steerMod = inputState.IsDown(BUTTON.SPRINT);
		float num = 0f;
		if (inputState.IsDown(BUTTON.FORWARD))
		{
			num = 1f;
		}
		else if (inputState.IsDown(BUTTON.BACKWARD))
		{
			num = -1f;
		}
		driverSeatInputs.throttleInput = 0f;
		driverSeatInputs.brakeInput = 0f;
		if (GetSpeed() > 3f && num < -0.1f)
		{
			driverSeatInputs.throttleInput = 0f;
			driverSeatInputs.brakeInput = 0f - num;
		}
		else
		{
			driverSeatInputs.throttleInput = num;
			driverSeatInputs.brakeInput = 0f;
		}
		for (int i = 0; i < base.NumAttachedModules; i++)
		{
			base.AttachedModuleEntities[i].PlayerServerInput(inputState, player);
		}
		if (engineController.IsOff && ((inputState.IsDown(BUTTON.FORWARD) && !inputState.WasDown(BUTTON.FORWARD)) || (inputState.IsDown(BUTTON.BACKWARD) && !inputState.WasDown(BUTTON.BACKWARD))))
		{
			engineController.TryStartEngine(player);
		}
	}

	public override void PlayerDismounted(BasePlayer player, BaseMountable seat)
	{
		base.PlayerDismounted(player, seat);
		DriverSeatInputs driverSeatInputs = default(DriverSeatInputs);
		if (this.driverSeatInputs.TryGetValue(seat, ref driverSeatInputs))
		{
			this.driverSeatInputs.Remove(seat);
		}
		foreach (BaseVehicleModule attachedModuleEntity in base.AttachedModuleEntities)
		{
			if ((Object)(object)attachedModuleEntity != (Object)null)
			{
				attachedModuleEntity.OnPlayerDismountedVehicle(player);
			}
		}
		CarLock.CheckEnableCentralLocking();
	}

	public override void Save(SaveInfo info)
	{
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		base.Save(info);
		info.msg.modularCar = Pool.Get<ModularCar>();
		info.msg.modularCar.steerAngle = SteerAngle;
		info.msg.modularCar.driveWheelVel = DriveWheelVelocity;
		info.msg.modularCar.throttleInput = GetThrottleInput();
		info.msg.modularCar.brakeInput = GetBrakeInput();
		info.msg.modularCar.fuelStorageID = GetFuelSystem().fuelStorageInstance.uid;
		info.msg.modularCar.fuelFraction = GetFuelFraction();
		CarLock.Save(info);
	}

	public override void Hurt(HitInfo info)
	{
		if (!IsDead() && !IsTransferProtected() && info.damageTypes.Get(DamageType.Decay) == 0f)
		{
			PropagateDamageToModules(info, 0.5f / (float)base.NumAttachedModules, 0.9f / (float)base.NumAttachedModules, null);
		}
		base.Hurt(info);
	}

	public void TickFuel(float fuelUsedPerSecond)
	{
		engineController.TickFuel(fuelUsedPerSecond);
	}

	public override bool MountEligable(BasePlayer player)
	{
		if (!base.MountEligable(player))
		{
			return false;
		}
		ModularCarSeat modularCarSeat = GetIdealMountPointFor(player) as ModularCarSeat;
		if ((Object)(object)modularCarSeat != (Object)null && !modularCarSeat.associatedSeatingModule.DoorsAreLockable)
		{
			return true;
		}
		return PlayerCanUseThis(player, ModularCarCodeLock.LockType.Door);
	}

	public override bool IsComplete()
	{
		if (HasAnyEngines() && HasDriverMountPoints())
		{
			return !IsDead();
		}
		return false;
	}

	public void DoDecayDamage(float damage)
	{
		foreach (BaseVehicleModule attachedModuleEntity in base.AttachedModuleEntities)
		{
			if (!attachedModuleEntity.IsDestroyed)
			{
				attachedModuleEntity.Hurt(damage, DamageType.Decay);
			}
		}
		if (!base.HasAnyModules)
		{
			Hurt(damage, DamageType.Decay);
		}
	}

	public float GetAdjustedDriveForce(float absSpeed, float topSpeed)
	{
		float num = 0f;
		for (int i = 0; i < base.AttachedModuleEntities.Count; i++)
		{
			num += base.AttachedModuleEntities[i].GetAdjustedDriveForce(absSpeed, topSpeed);
		}
		return RollOffDriveForce(num);
	}

	public bool HasAnyEngines()
	{
		for (int i = 0; i < base.AttachedModuleEntities.Count; i++)
		{
			if (base.AttachedModuleEntities[i].HasAnEngine)
			{
				return true;
			}
		}
		return false;
	}

	public bool HasAnyWorkingEngines()
	{
		return GetMaxDriveForce() > 0f;
	}

	public override bool MeetsEngineRequirements()
	{
		if (HasAnyWorkingEngines())
		{
			return HasDriver();
		}
		return false;
	}

	public override void OnEngineStartFailed()
	{
		bool arg = !HasAnyWorkingEngines() || engineController.IsWaterlogged();
		ClientRPC(null, "EngineStartFailed", arg);
	}

	public CarWheel[] GetWheels()
	{
		if (wheels == null)
		{
			wheels = new CarWheel[4] { wheelFL, wheelFR, wheelRL, wheelRR };
		}
		return wheels;
	}

	public float GetWheelsMidPos()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		return (((Component)wheels[0].wheelCollider).transform.localPosition.z - ((Component)wheels[2].wheelCollider).transform.localPosition.z) * 0.5f;
	}

	public override bool AdminFixUp(int tier)
	{
		if (!base.AdminFixUp(tier))
		{
			return false;
		}
		foreach (BaseVehicleModule attachedModuleEntity in base.AttachedModuleEntities)
		{
			attachedModuleEntity.AdminFixUp(tier);
		}
		SendNetworkUpdate();
		return true;
	}

	public override void ModuleHurt(BaseVehicleModule hurtModule, HitInfo info)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		base.ModuleHurt(hurtModule, info);
		if (IsDead())
		{
			if (TimeSince.op_Implicit(timeSinceDeath) > 1f)
			{
				for (int i = 0; i < info.damageTypes.types.Length; i++)
				{
					deathDamageCounter += info.damageTypes.types[i];
				}
			}
			if (deathDamageCounter > 600f && !base.IsDestroyed)
			{
				Kill(DestroyMode.Gib);
			}
		}
		else if (hurtModule.PropagateDamage && info.damageTypes.Get(DamageType.Decay) == 0f)
		{
			PropagateDamageToModules(info, 0.15f, 0.4f, hurtModule);
		}
	}

	private void PropagateDamageToModules(HitInfo info, float minPropagationPercent, float maxPropagationPercent, BaseVehicleModule ignoreModule)
	{
		foreach (BaseVehicleModule attachedModuleEntity in base.AttachedModuleEntities)
		{
			if ((Object)(object)attachedModuleEntity == (Object)(object)ignoreModule || attachedModuleEntity.Health() <= 0f)
			{
				continue;
			}
			if (IsDead())
			{
				break;
			}
			float num = Random.Range(minPropagationPercent, maxPropagationPercent);
			for (int i = 0; i < info.damageTypes.types.Length; i++)
			{
				float num2 = info.damageTypes.types[i];
				if (num2 > 0f)
				{
					attachedModuleEntity.AcceptPropagatedDamage(num2 * num, (DamageType)i, info.Initiator, info.UseProtection);
				}
				if (IsDead())
				{
					break;
				}
			}
		}
	}

	public override void ModuleReachedZeroHealth()
	{
		if (IsDead())
		{
			return;
		}
		bool flag = true;
		foreach (BaseVehicleModule attachedModuleEntity in base.AttachedModuleEntities)
		{
			if (attachedModuleEntity.health > 0f)
			{
				flag = false;
				break;
			}
		}
		if (flag)
		{
			Die();
		}
	}

	public override void OnKilled(HitInfo info)
	{
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		DismountAllPlayers();
		foreach (BaseVehicleModule attachedModuleEntity in base.AttachedModuleEntities)
		{
			attachedModuleEntity.repair.enabled = false;
		}
		if (CarLock != null)
		{
			CarLock.RemoveLock();
		}
		timeSinceDeath = TimeSince.op_Implicit(0f);
		if (vehicle.carwrecks)
		{
			if (!base.HasAnyModules)
			{
				Kill(DestroyMode.Gib);
			}
			else
			{
				SendNetworkUpdate();
			}
		}
		else
		{
			Kill(DestroyMode.Gib);
		}
	}

	public void RemoveLock()
	{
		CarLock.RemoveLock();
	}

	public void RestoreVelocity(Vector3 vel)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		Vector3 velocity = rigidBody.velocity;
		if (((Vector3)(ref velocity)).sqrMagnitude < ((Vector3)(ref vel)).sqrMagnitude)
		{
			vel.y = rigidBody.velocity.y;
			rigidBody.velocity = vel;
		}
	}

	protected override Vector3 GetCOMMultiplier()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		if (carPhysics == null || !carPhysics.IsGrounded() || !IsOn())
		{
			return airbourneCOMMultiplier;
		}
		return groundedCOMMultiplier;
	}

	private void UpdateClients()
	{
		if (HasDriver())
		{
			byte num = (byte)((GetThrottleInput() + 1f) * 7f);
			byte b = (byte)(GetBrakeInput() * 15f);
			byte arg = (byte)(num + (b << 4));
			byte arg2 = (byte)(GetFuelFraction() * 255f);
			ClientRPC(null, "ModularCarUpdate", SteerAngle, arg, DriveWheelVelocity, arg2);
		}
	}

	private void DecayTick()
	{
		if (base.IsDestroyed || IsOn() || immuneToDecay || Time.time < lastEngineOnTime + 600f)
		{
			return;
		}
		float num = 1f;
		if (IsDead())
		{
			int num2 = Mathf.Max(1, base.AttachedModuleEntities.Count);
			num /= 5f * (float)num2;
			DoDecayDamage(600f * num);
			return;
		}
		num /= outsidedecayminutes;
		if (!IsOutside())
		{
			num *= 0.1f;
		}
		float num3 = (base.HasAnyModules ? base.AttachedModuleEntities.Max((BaseVehicleModule module) => module.MaxHealth()) : MaxHealth());
		DoDecayDamage(num3 * num);
	}

	protected override void DoCollisionDamage(BaseEntity hitEntity, float damage)
	{
		if ((Object)(object)hitEntity == (Object)null)
		{
			return;
		}
		if (hitEntity is BaseVehicleModule baseVehicleModule)
		{
			baseVehicleModule.Hurt(damage, DamageType.Collision, this, useProtection: false);
		}
		else
		{
			if (!((Object)(object)hitEntity == (Object)(object)this))
			{
				return;
			}
			if (base.HasAnyModules)
			{
				float amount = damage / (float)base.NumAttachedModules;
				{
					foreach (BaseVehicleModule attachedModuleEntity in base.AttachedModuleEntities)
					{
						attachedModuleEntity.AcceptPropagatedDamage(amount, DamageType.Collision, this, useProtection: false);
					}
					return;
				}
			}
			Hurt(damage, DamageType.Collision, this, useProtection: false);
		}
	}

	private void SpawnPreassignedModules()
	{
		if (!spawnSettings.useSpawnSettings || spawnSettings.configurationOptions.IsNullOrEmpty())
		{
			return;
		}
		ModularCarPresetConfig modularCarPresetConfig = spawnSettings.configurationOptions[Random.Range(0, spawnSettings.configurationOptions.Length)];
		for (int i = 0; i < modularCarPresetConfig.socketItemDefs.Length; i++)
		{
			ItemModVehicleModule itemModVehicleModule = modularCarPresetConfig.socketItemDefs[i];
			if ((Object)(object)itemModVehicleModule != (Object)null && base.Inventory.SocketsAreFree(i, itemModVehicleModule.socketsTaken))
			{
				itemModVehicleModule.doNonUserSpawn = true;
				Item item = ItemManager.Create(((Component)itemModVehicleModule).GetComponent<ItemDefinition>(), 1, 0uL);
				float num = Random.Range(spawnSettings.minStartHealthPercent, spawnSettings.maxStartHealthPercent);
				item.condition = item.maxCondition * num;
				if (!TryAddModule(item))
				{
					item.Remove();
				}
			}
		}
		((FacepunchBehaviour)this).Invoke((Action)HandleAdminBonus, 0f);
	}

	private void HandleAdminBonus()
	{
		switch (spawnSettings.adminBonus)
		{
		case SpawnSettings.AdminBonus.T1PlusFuel:
			AdminFixUp(1);
			break;
		case SpawnSettings.AdminBonus.T2PlusFuel:
			AdminFixUp(2);
			break;
		case SpawnSettings.AdminBonus.T3PlusFuel:
			AdminFixUp(3);
			break;
		}
	}

	[RPC_Server]
	public void RPC_OpenFuel(RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if (!((Object)(object)player == (Object)null) && CanBeLooted(player))
		{
			GetFuelSystem().LootFuel(player);
		}
	}

	[RPC_Server]
	public void RPC_OpenFuelWithKeycode(RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if ((Object)(object)player == (Object)null)
		{
			return;
		}
		string codeEntered = msg.read.String(256, false);
		if (CarLock.TryOpenWithCode(player, codeEntered))
		{
			if (CanBeLooted(player))
			{
				GetFuelSystem().LootFuel(player);
			}
		}
		else
		{
			ClientRPC(null, "CodeEntryFailed");
		}
	}

	[RPC_Server]
	public void RPC_TryMountWithKeycode(RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if (!((Object)(object)player == (Object)null))
		{
			string codeEntered = msg.read.String(256, false);
			if (CarLock.TryOpenWithCode(player, codeEntered))
			{
				WantsMount(player);
			}
			else
			{
				ClientRPC(null, "CodeEntryFailed");
			}
		}
	}

	public override void ScaleDamageForPlayer(BasePlayer player, HitInfo info)
	{
		base.ScaleDamageForPlayer(player, info);
		foreach (BaseVehicleModule attachedModuleEntity in base.AttachedModuleEntities)
		{
			if (attachedModuleEntity.HasSeating && attachedModuleEntity is VehicleModuleSeating vehicleModuleSeating && vehicleModuleSeating.IsOnThisModule(player))
			{
				attachedModuleEntity.ScaleDamageForPlayer(player, info);
			}
		}
	}
}
