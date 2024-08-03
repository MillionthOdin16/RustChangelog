using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using Sonar;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Profiling;
using UnityEngine.Serialization;
using VLB;

public class BaseSubmarine : BaseVehicle, IPoolVehicle, IEngineControllerUser, IEntity, IAirSupply
{
	[Serializable]
	public class ParentTriggerInfo
	{
		public TriggerParent trigger;

		public Transform triggerWaterLevel;
	}

	private float targetClimbSpeed;

	private float maxDamageThisTick;

	private float nextCollisionDamageTime;

	private bool prevPrimaryFireInput;

	private bool primaryFireInput;

	private const float DECAY_TICK_TIME = 60f;

	private TimeSince timeSinceLastUsed = default(TimeSince);

	private TimeSince timeSinceTorpedoFired = default(TimeSince);

	private TimeSince timeSinceFailRPCSent = default(TimeSince);

	private float normalDrag;

	private float highDrag;

	private bool wasOnSurface = false;

	[Header("Submarine Main")]
	[SerializeField]
	private Transform centreOfMassTransform;

	[SerializeField]
	private Buoyancy buoyancy;

	[SerializeField]
	protected float maxRudderAngle = 35f;

	[SerializeField]
	private Transform rudderVisualTransform;

	[SerializeField]
	private Transform rudderDetailedColliderTransform;

	[SerializeField]
	private Transform propellerTransform;

	[SerializeField]
	private float timeUntilAutoSurface = 300f;

	[SerializeField]
	private Renderer[] interiorRenderers;

	[SerializeField]
	private SonarObject sonarObject;

	[SerializeField]
	private ParentTriggerInfo[] parentTriggers;

	[SerializeField]
	private GameObjectRef fuelStoragePrefab;

	[Header("Submarine Engine & Fuel")]
	[SerializeField]
	private float engineKW = 200f;

	[SerializeField]
	private float turnPower = 0.25f;

	[SerializeField]
	private float engineStartupTime = 0.5f;

	[SerializeField]
	private GameObjectRef itemStoragePrefab;

	[SerializeField]
	private float depthChangeTargetSpeed = 1f;

	[SerializeField]
	private float idleFuelPerSec = 0.03f;

	[SerializeField]
	private float maxFuelPerSec = 0.15f;

	[FormerlySerializedAs("internalAccessFuelTank")]
	[SerializeField]
	private bool internalAccessStorage;

	[Header("Submarine Weaponry")]
	[SerializeField]
	private GameObjectRef torpedoStoragePrefab;

	[SerializeField]
	private Transform torpedoFiringPoint;

	[SerializeField]
	private float maxFireRate = 1.5f;

	[Header("Submarine Audio & FX")]
	[SerializeField]
	protected SubmarineAudio submarineAudio;

	[SerializeField]
	private ParticleSystem fxTorpedoFire;

	[SerializeField]
	private GameObject internalFXContainer;

	[SerializeField]
	private GameObject internalOnFXContainer;

	[SerializeField]
	private ParticleSystem fxIntAmbientBubbleLoop;

	[SerializeField]
	private ParticleSystem fxIntInitialDiveBubbles;

	[SerializeField]
	private ParticleSystem fxIntWaterDropSpray;

	[SerializeField]
	private ParticleSystem fxIntWindowFilm;

	[SerializeField]
	private ParticleSystemContainer fxIntMediumDamage;

	[SerializeField]
	private ParticleSystemContainer fxIntHeavyDamage;

	[SerializeField]
	private GameObject externalFXContainer;

	[SerializeField]
	private GameObject externalOnFXContainer;

	[SerializeField]
	private ParticleSystem fxExtAmbientBubbleLoop;

	[SerializeField]
	private ParticleSystem fxExtInitialDiveBubbles;

	[SerializeField]
	private ParticleSystem fxExtAboveWaterEngineThrustForward;

	[SerializeField]
	private ParticleSystem fxExtAboveWaterEngineThrustReverse;

	[SerializeField]
	private ParticleSystem fxExtUnderWaterEngineThrustForward;

	[SerializeField]
	private ParticleSystem[] fxExtUnderWaterEngineThrustForwardSubs;

	[SerializeField]
	private ParticleSystem fxExtUnderWaterEngineThrustReverse;

	[SerializeField]
	private ParticleSystem[] fxExtUnderWaterEngineThrustReverseSubs;

	[SerializeField]
	private ParticleSystem fxExtBowWave;

	[SerializeField]
	private ParticleSystem fxExtWakeEffect;

	[SerializeField]
	private GameObjectRef aboveWatercollisionEffect;

	[SerializeField]
	private GameObjectRef underWatercollisionEffect;

	[SerializeField]
	private VolumetricLightBeam spotlightVolumetrics;

	[SerializeField]
	private float mountedAlphaInside = 0.04f;

	[SerializeField]
	private float mountedAlphaOutside = 0.015f;

	[ServerVar(Help = "How long before a submarine loses all its health while outside. If it's in deep water, deepwaterdecayminutes is used")]
	public static float outsidedecayminutes = 180f;

	[ServerVar(Help = "How long before a submarine loses all its health while in deep water")]
	public static float deepwaterdecayminutes = 120f;

	[ServerVar(Help = "How long a submarine can stay underwater until players start taking damage from low oxygen")]
	public static float oxygenminutes = 10f;

	public const Flags Flag_Ammo = Flags.Reserved6;

	private float _throttle;

	private float _rudder;

	private float _upDown;

	private float _oxygen = 1f;

	protected VehicleEngineController<BaseSubmarine> engineController;

	protected float cachedFuelAmount;

	protected Vector3 steerAngle;

	protected float waterSurfaceY;

	protected float curSubDepthY;

	private EntityRef<StorageContainer> torpedoStorageInstance = default(EntityRef<StorageContainer>);

	private EntityRef<StorageContainer> itemStorageInstance = default(EntityRef<StorageContainer>);

	private int waterLayerMask;

	public ItemModGiveOxygen.AirSupplyType AirType => ItemModGiveOxygen.AirSupplyType.Submarine;

	public VehicleEngineController<BaseSubmarine>.EngineState EngineState => engineController.CurEngineState;

	public Vector3 Velocity { get; private set; }

	public bool LightsAreOn => HasFlag(Flags.Reserved5);

	public bool HasAmmo => HasFlag(Flags.Reserved6);

	public float ThrottleInput
	{
		get
		{
			return engineController.IsOn ? _throttle : 0f;
		}
		protected set
		{
			_throttle = Mathf.Clamp(value, -1f, 1f);
		}
	}

	public float RudderInput
	{
		get
		{
			return _rudder;
		}
		protected set
		{
			_rudder = Mathf.Clamp(value, -1f, 1f);
		}
	}

	public float UpDownInput
	{
		get
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			if (base.isServer)
			{
				if (TimeSince.op_Implicit(timeSinceLastUsed) >= timeUntilAutoSurface)
				{
					return 0.15f;
				}
				return engineController.IsOn ? _upDown : Mathf.Max(0f, _upDown);
			}
			return _upDown;
		}
		protected set
		{
			_upDown = Mathf.Clamp(value, -1f, 1f);
		}
	}

	public float Oxygen
	{
		get
		{
			return _oxygen;
		}
		protected set
		{
			_oxygen = Mathf.Clamp(value, 0f, 1f);
		}
	}

	protected float PhysicalRudderAngle
	{
		get
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			float num = rudderDetailedColliderTransform.localEulerAngles.y;
			if (num > 180f)
			{
				num -= 360f;
			}
			return num;
		}
	}

	protected bool IsInWater => curSubDepthY > 0.2f;

	protected bool IsSurfaced => curSubDepthY < 1.1f;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("BaseSubmarine.OnRpcMessage", 0);
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
				TimeWarning val4 = TimeWarning.New("RPC_OpenItemStorage", 0);
				try
				{
					TimeWarning val5 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.MaxDistance.Test(924237371u, "RPC_OpenItemStorage", this, player, 3f))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val5)?.Dispose();
					}
					try
					{
						TimeWarning val6 = TimeWarning.New("Call", 0);
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
							((IDisposable)val6)?.Dispose();
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
					((IDisposable)val4)?.Dispose();
				}
				return true;
			}
			if (rpc == 2181221870u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_OpenTorpedoStorage "));
				}
				TimeWarning val7 = TimeWarning.New("RPC_OpenTorpedoStorage", 0);
				try
				{
					TimeWarning val8 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.MaxDistance.Test(2181221870u, "RPC_OpenTorpedoStorage", this, player, 3f))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val8)?.Dispose();
					}
					try
					{
						TimeWarning val9 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage msg4 = rPCMessage;
							RPC_OpenTorpedoStorage(msg4);
						}
						finally
						{
							((IDisposable)val9)?.Dispose();
						}
					}
					catch (Exception ex3)
					{
						Debug.LogException(ex3);
						player.Kick("RPC Error in RPC_OpenTorpedoStorage");
					}
				}
				finally
				{
					((IDisposable)val7)?.Dispose();
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
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		base.ServerInit();
		rigidBody.centerOfMass = centreOfMassTransform.localPosition;
		timeSinceLastUsed = TimeSince.op_Implicit(timeUntilAutoSurface);
		buoyancy.buoyancyScale = 1f;
		normalDrag = rigidBody.drag;
		highDrag = normalDrag * 2.5f;
		Oxygen = 1f;
		((FacepunchBehaviour)this).InvokeRandomized((Action)UpdateClients, 0f, 0.15f, 0.02f);
		((FacepunchBehaviour)this).InvokeRandomized((Action)SubmarineDecay, Random.Range(30f, 60f), 60f, 6f);
	}

	protected override void OnChildAdded(BaseEntity child)
	{
		base.OnChildAdded(child);
		if (base.isServer)
		{
			if (isSpawned)
			{
				GetFuelSystem().CheckNewChild(child);
			}
			if (child.prefabID == itemStoragePrefab.GetEntity().prefabID)
			{
				itemStorageInstance.Set((StorageContainer)child);
			}
			if (child.prefabID == torpedoStoragePrefab.GetEntity().prefabID)
			{
				torpedoStorageInstance.Set((StorageContainer)child);
			}
		}
	}

	private void ServerFlagsChanged(Flags old, Flags next)
	{
		if (next.HasFlag(Flags.On) && !old.HasFlag(Flags.On))
		{
			SetFlag(Flags.Reserved5, b: true);
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

	protected void OnCollisionEnter(Collision collision)
	{
		if (!base.isClient)
		{
			ProcessCollision(collision);
		}
	}

	public override float MaxVelocity()
	{
		return 10f;
	}

	public override EntityFuelSystem GetFuelSystem()
	{
		return engineController.FuelSystem;
	}

	public override int StartingFuelUnits()
	{
		return 50;
	}

	public override void AttemptMount(BasePlayer player, bool doMountChecks = true)
	{
		if (CanMount(player) && MountEligable(player))
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

	public void OnPoolDestroyed()
	{
		Kill(DestroyMode.Gib);
	}

	public void WakeUp()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)rigidBody != (Object)null)
		{
			rigidBody.WakeUp();
			rigidBody.AddForce(Vector3.up * 0.1f, (ForceMode)1);
		}
	}

	protected override void OnServerWake()
	{
		if ((Object)(object)buoyancy != (Object)null)
		{
			buoyancy.Wake();
		}
	}

	public override void OnKilled(HitInfo info)
	{
		DamageType majorityDamageType = info.damageTypes.GetMajorityDamageType();
		if (majorityDamageType == DamageType.Explosion || majorityDamageType == DamageType.AntiVehicle)
		{
			foreach (MountPointInfo mountPoint in mountPoints)
			{
				if ((Object)(object)mountPoint.mountable != (Object)null)
				{
					BasePlayer mounted = mountPoint.mountable.GetMounted();
					if ((Object)(object)mounted != (Object)null)
					{
						mounted.Hurt(10000f, DamageType.Explosion, this, useProtection: false);
					}
				}
			}
		}
		base.OnKilled(info);
	}

	public override void VehicleFixedUpdate()
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_039f: Unknown result type (might be due to invalid IL or missing references)
		//IL_03aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0445: Unknown result type (might be due to invalid IL or missing references)
		//IL_044c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		//IL_030a: Unknown result type (might be due to invalid IL or missing references)
		//IL_030f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0317: Unknown result type (might be due to invalid IL or missing references)
		//IL_032e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0336: Unknown result type (might be due to invalid IL or missing references)
		//IL_033d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0621: Unknown result type (might be due to invalid IL or missing references)
		//IL_0524: Unknown result type (might be due to invalid IL or missing references)
		//IL_055e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0569: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_058a: Unknown result type (might be due to invalid IL or missing references)
		//IL_058f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0669: Unknown result type (might be due to invalid IL or missing references)
		//IL_0679: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e7: Unknown result type (might be due to invalid IL or missing references)
		base.VehicleFixedUpdate();
		if (!base.IsMovingOrOn)
		{
			Velocity = Vector3.zero;
			targetClimbSpeed = 0f;
			buoyancy.ArtificialHeight = null;
			return;
		}
		Profiler.BeginSample("Submarine.VehicleFixedUpdate");
		Velocity = GetLocalVelocity();
		UpdateWaterInfo();
		if (IsSurfaced && !wasOnSurface && ((Component)this).transform.position.y > Env.oceanlevel - 1f)
		{
			wasOnSurface = true;
		}
		buoyancy.ArtificialHeight = waterSurfaceY;
		rigidBody.drag = (HasDriver() ? normalDrag : highDrag);
		float num = 2f;
		if (IsSurfaced)
		{
			float num2 = 20f * num;
			if (Oxygen < 0.5f)
			{
				Oxygen = 0.5f;
			}
			else
			{
				Oxygen += Time.deltaTime / num2;
			}
		}
		else if (AnyMounted())
		{
			float num3 = oxygenminutes * 60f * num;
			Oxygen -= Time.deltaTime / num3;
		}
		engineController.CheckEngineState();
		if (engineController.IsOn)
		{
			float fuelPerSecond = Mathf.Lerp(idleFuelPerSec, maxFuelPerSec, Mathf.Abs(ThrottleInput));
			engineController.TickFuel(fuelPerSecond);
		}
		if (IsInWater)
		{
			float num4 = depthChangeTargetSpeed * UpDownInput;
			float num5 = (((!(UpDownInput > 0f) || !(num4 > targetClimbSpeed) || !(targetClimbSpeed > 0f)) && (!(UpDownInput < 0f) || !(num4 < targetClimbSpeed) || !(targetClimbSpeed < 0f))) ? 4f : 0.7f);
			targetClimbSpeed = Mathf.MoveTowards(targetClimbSpeed, num4, num5 * Time.fixedDeltaTime);
			float num6 = rigidBody.velocity.y - targetClimbSpeed;
			float num7 = buoyancy.buoyancyScale - num6 * 50f * Time.fixedDeltaTime;
			buoyancy.buoyancyScale = Mathf.Clamp(num7, 0.01f, 1f);
			Vector3 angularVelocity = rigidBody.angularVelocity;
			Vector3 val = Quaternion.AngleAxis(((Vector3)(ref angularVelocity)).magnitude * 57.29578f * 10f / 200f, rigidBody.angularVelocity) * ((Component)this).transform.up;
			Vector3 val2 = Vector3.Cross(val, Vector3.up);
			Vector3 val3 = val2 * 200f * 200f;
			rigidBody.AddTorque(val3);
			float num8 = 0.1f;
			rigidBody.AddForce(Vector3.up * (0f - num6) * num8, (ForceMode)2);
		}
		else
		{
			float num9 = 0f;
			buoyancy.buoyancyScale = Mathf.Lerp(buoyancy.buoyancyScale, num9, Time.fixedDeltaTime);
		}
		if (IsOn() && IsInWater)
		{
			rigidBody.AddForce(((Component)this).transform.forward * engineKW * 40f * ThrottleInput, (ForceMode)0);
			float num10 = turnPower * rigidBody.mass * rigidBody.angularDrag;
			float speed = GetSpeed();
			float num11 = Mathf.Min(Mathf.Abs(speed) * 0.6f, 6f) + 4f;
			float num12 = num10 * RudderInput * num11;
			if (speed < -1f)
			{
				num12 *= -1f;
			}
			rigidBody.AddTorque(((Component)this).transform.up * num12, (ForceMode)0);
		}
		UpdatePhysicalRudder(RudderInput, Time.fixedDeltaTime);
		if (Time.time >= nextCollisionDamageTime && maxDamageThisTick > 0f)
		{
			nextCollisionDamageTime = Time.time + 0.33f;
			Hurt(maxDamageThisTick, DamageType.Collision, this, useProtection: false);
			maxDamageThisTick = 0f;
		}
		StorageContainer torpedoContainer = GetTorpedoContainer();
		if ((Object)(object)torpedoContainer != (Object)null)
		{
			bool b = torpedoContainer.inventory.HasAmmo((AmmoTypes)1024);
			SetFlag(Flags.Reserved6, b);
		}
		BasePlayer driver = GetDriver();
		if ((Object)(object)driver != (Object)null && primaryFireInput)
		{
			bool flag = true;
			if (IsInWater && TimeSince.op_Implicit(timeSinceTorpedoFired) >= maxFireRate)
			{
				float minSpeed = GetSpeed() + 2f;
				if (TryFireProjectile(torpedoContainer, (AmmoTypes)1024, torpedoFiringPoint.position, torpedoFiringPoint.forward, driver, 1f, minSpeed, out var _))
				{
					timeSinceTorpedoFired = TimeSince.op_Implicit(0f);
					flag = false;
					driver.MarkHostileFor();
					ClientRPC(null, "TorpedoFired");
				}
			}
			if (!prevPrimaryFireInput && flag && TimeSince.op_Implicit(timeSinceFailRPCSent) > 0.5f)
			{
				timeSinceFailRPCSent = TimeSince.op_Implicit(0f);
				ClientRPCPlayer(null, driver, "TorpedoFireFailed");
			}
		}
		else if ((Object)(object)driver == (Object)null)
		{
			primaryFireInput = false;
		}
		prevPrimaryFireInput = primaryFireInput;
		if (TimeSince.op_Implicit(timeSinceLastUsed) > 300f && LightsAreOn)
		{
			SetFlag(Flags.Reserved5, b: false);
		}
		for (int i = 0; i < parentTriggers.Length; i++)
		{
			float num13 = parentTriggers[i].triggerWaterLevel.position.y - ((Component)this).transform.position.y;
			float num14 = curSubDepthY - num13;
			bool flag2 = num14 <= 0f;
			if (flag2 != ((Behaviour)parentTriggers[i].trigger).enabled)
			{
				((Behaviour)parentTriggers[i].trigger).enabled = flag2;
			}
		}
		Profiler.EndSample();
	}

	public override void LightToggle(BasePlayer player)
	{
		if (IsDriver(player))
		{
			SetFlag(Flags.Reserved5, !LightsAreOn);
		}
	}

	public override void PlayerServerInput(InputState inputState, BasePlayer player)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		timeSinceLastUsed = TimeSince.op_Implicit(0f);
		if (IsDriver(player))
		{
			if (inputState.IsDown(BUTTON.SPRINT))
			{
				UpDownInput = 1f;
			}
			else if (inputState.IsDown(BUTTON.DUCK))
			{
				UpDownInput = -1f;
			}
			else
			{
				UpDownInput = 0f;
			}
			if (inputState.IsDown(BUTTON.FORWARD))
			{
				ThrottleInput = 1f;
			}
			else if (inputState.IsDown(BUTTON.BACKWARD))
			{
				ThrottleInput = -1f;
			}
			else
			{
				ThrottleInput = 0f;
			}
			if (inputState.IsDown(BUTTON.LEFT))
			{
				RudderInput = -1f;
			}
			else if (inputState.IsDown(BUTTON.RIGHT))
			{
				RudderInput = 1f;
			}
			else
			{
				RudderInput = 0f;
			}
			primaryFireInput = inputState.IsDown(BUTTON.FIRE_PRIMARY);
			if (engineController.IsOff && ((inputState.IsDown(BUTTON.FORWARD) && !inputState.WasDown(BUTTON.FORWARD)) || (inputState.IsDown(BUTTON.BACKWARD) && !inputState.WasDown(BUTTON.BACKWARD)) || (inputState.IsDown(BUTTON.SPRINT) && !inputState.WasDown(BUTTON.SPRINT)) || (inputState.IsDown(BUTTON.DUCK) && !inputState.WasDown(BUTTON.DUCK))))
			{
				engineController.TryStartEngine(player);
			}
		}
	}

	public override void Save(SaveInfo info)
	{
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		base.Save(info);
		info.msg.submarine = Pool.Get<Submarine>();
		info.msg.submarine.throttle = ThrottleInput;
		info.msg.submarine.upDown = UpDownInput;
		info.msg.submarine.rudder = RudderInput;
		info.msg.submarine.fuelStorageID = GetFuelSystem().fuelStorageInstance.uid;
		info.msg.submarine.fuelAmount = GetFuelAmount();
		info.msg.submarine.torpedoStorageID = torpedoStorageInstance.uid;
		info.msg.submarine.oxygen = Oxygen;
		info.msg.submarine.itemStorageID = itemStorageInstance.uid;
	}

	public bool MeetsEngineRequirements()
	{
		return AnyMounted();
	}

	public void OnEngineStartFailed()
	{
		ClientRPC(null, "EngineStartFailed");
	}

	public StorageContainer GetTorpedoContainer()
	{
		BaseEntity baseEntity = torpedoStorageInstance.Get(base.isServer);
		if ((Object)(object)baseEntity != (Object)null && baseEntity.IsValid())
		{
			return baseEntity as StorageContainer;
		}
		return null;
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

	private void ProcessCollision(Collision collision)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		if (!base.isClient && collision != null && !((Object)(object)collision.gameObject == (Object)null) && !((Object)(object)collision.gameObject == (Object)null))
		{
			Vector3 impulse = collision.impulse;
			float num = ((Vector3)(ref impulse)).magnitude / Time.fixedDeltaTime;
			float num2 = Mathf.InverseLerp(100000f, 2500000f, num);
			if (num2 > 0f)
			{
				float num3 = Mathf.Lerp(1f, 200f, num2);
				maxDamageThisTick = Mathf.Max(maxDamageThisTick, num3);
			}
			if (num2 > 0f)
			{
				GameObjectRef effectGO = ((curSubDepthY > 2f) ? underWatercollisionEffect : aboveWatercollisionEffect);
				TryShowCollisionFX(collision, effectGO);
			}
		}
	}

	private void UpdateClients()
	{
		if (HasDriver())
		{
			byte b = (byte)((ThrottleInput + 1f) * 7f);
			byte b2 = (byte)((UpDownInput + 1f) * 7f);
			byte arg = (byte)(b + (b2 << 4));
			int arg2 = Mathf.CeilToInt(GetFuelAmount());
			ClientRPC(null, "SubmarineUpdate", RudderInput, arg, arg2, Oxygen);
		}
	}

	private void SubmarineDecay()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		BaseBoat.WaterVehicleDecay(this, 60f, TimeSince.op_Implicit(timeSinceLastUsed), outsidedecayminutes, deepwaterdecayminutes, MotorRowboat.decaystartdelayminutes, preventDecayIndoors: true);
	}

	[RPC_Server]
	public void RPC_OpenFuel(RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if (CanBeLooted(player))
		{
			GetFuelSystem().LootFuel(player);
		}
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	public void RPC_OpenTorpedoStorage(RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if (CanBeLooted(player) && PlayerIsMounted(player))
		{
			StorageContainer torpedoContainer = GetTorpedoContainer();
			if ((Object)(object)torpedoContainer != (Object)null)
			{
				torpedoContainer.PlayerOpenLoot(player);
			}
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

	public void OnSurfacedInMoonpool()
	{
		if (wasOnSurface && GameInfo.HasAchievements)
		{
			wasOnSurface = false;
			BasePlayer driver = GetDriver();
			if ((Object)(object)driver != (Object)null)
			{
				driver.GiveAchievement("SUBMARINE_MOONPOOL");
			}
		}
	}

	public override void InitShared()
	{
		base.InitShared();
		waterLayerMask = LayerMask.GetMask(new string[1] { "Water" });
		engineController = new VehicleEngineController<BaseSubmarine>(this, base.isServer, engineStartupTime, fuelStoragePrefab);
	}

	public override void Load(LoadInfo info)
	{
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		base.Load(info);
		if (info.msg.submarine != null)
		{
			ThrottleInput = info.msg.submarine.throttle;
			UpDownInput = info.msg.submarine.upDown;
			RudderInput = info.msg.submarine.rudder;
			engineController.FuelSystem.fuelStorageInstance.uid = info.msg.submarine.fuelStorageID;
			cachedFuelAmount = info.msg.submarine.fuelAmount;
			torpedoStorageInstance.uid = info.msg.submarine.torpedoStorageID;
			Oxygen = info.msg.submarine.oxygen;
			itemStorageInstance.uid = info.msg.submarine.itemStorageID;
			UpdatePhysicalRudder(RudderInput, 0f);
		}
	}

	public override void OnFlagsChanged(Flags old, Flags next)
	{
		base.OnFlagsChanged(old, next);
		if (old != next && base.isServer)
		{
			ServerFlagsChanged(old, next);
		}
	}

	public override float WaterFactorForPlayer(BasePlayer player)
	{
		return 0f;
	}

	public override float AirFactor()
	{
		return Oxygen;
	}

	public override bool BlocksWaterFor(BasePlayer player)
	{
		return true;
	}

	public float GetFuelAmount()
	{
		if (base.isServer)
		{
			return engineController.FuelSystem.GetFuelAmount();
		}
		return cachedFuelAmount;
	}

	public float GetSpeed()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		if (IsStationary())
		{
			return 0f;
		}
		return Vector3.Dot(Velocity, ((Component)this).transform.forward);
	}

	public override bool CanBeLooted(BasePlayer player)
	{
		if (!base.CanBeLooted(player))
		{
			return false;
		}
		if (PlayerIsMounted(player))
		{
			return true;
		}
		if (internalAccessStorage)
		{
			return false;
		}
		return !IsOn();
	}

	public float GetAirTimeRemaining()
	{
		if (Oxygen <= 0.5f)
		{
			return 0f;
		}
		float num = Mathf.InverseLerp(0.5f, 1f, Oxygen);
		return num * oxygenminutes * 60f;
	}

	protected override bool CanPushNow(BasePlayer pusher)
	{
		if (!base.CanPushNow(pusher))
		{
			return false;
		}
		if (pusher.isMounted || pusher.IsSwimming() || !pusher.IsOnGround())
		{
			return false;
		}
		return !pusher.IsStandingOnEntity(this, 8192);
	}

	private void UpdatePhysicalRudder(float turnInput, float deltaTime)
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		float num = (0f - turnInput) * maxRudderAngle;
		float num2 = ((!base.IsMovingOrOn) ? num : Mathf.MoveTowards(PhysicalRudderAngle, num, 200f * deltaTime));
		Quaternion localRotation = Quaternion.Euler(0f, num2, 0f);
		if (base.isClient)
		{
			rudderVisualTransform.localRotation = localRotation;
		}
		rudderDetailedColliderTransform.localRotation = localRotation;
	}

	private bool CanMount(BasePlayer player)
	{
		return !player.IsDead();
	}

	private void UpdateWaterInfo()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		waterSurfaceY = GetWaterSurfaceY();
		curSubDepthY = waterSurfaceY - ((Component)this).transform.position.y;
	}

	private float GetWaterSurfaceY()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		RaycastHit val = default(RaycastHit);
		if (Physics.Raycast(((Component)this).transform.position - Vector3.up * 1.5f, Vector3.up, ref val, 5f, waterLayerMask, (QueryTriggerInteraction)2))
		{
			return ((RaycastHit)(ref val)).point.y;
		}
		WaterLevel.WaterInfo waterInfo = WaterLevel.GetWaterInfo(((Component)this).transform.position, waves: true, volumes: true, this);
		return waterInfo.isValid ? waterInfo.surfaceLevel : (((Component)this).transform.position.y - 1f);
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
