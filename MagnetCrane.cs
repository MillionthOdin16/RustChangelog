using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

public class MagnetCrane : GroundVehicle, CarPhysics<MagnetCrane>.ICar
{
	private float steerInput;

	private float throttleInput;

	private float brakeInput;

	private float yawInput;

	private float extensionInput;

	private float raiseArmInput;

	private float extensionMove;

	private float yawMove;

	private float raiseArmMove;

	private float nextToggleTime;

	private Vector3 spawnOrigin = Vector3.zero;

	private float lastExtensionArmState;

	private float lastRaiseArmState;

	private float lastYawState;

	private bool handbrakeOn = true;

	private float nextSelfHealTime;

	private Vector3 lastDamagePos = Vector3.zero;

	private float lastDrivenTime;

	private float lastFixedUpdateTime;

	private CarPhysics<MagnetCrane> carPhysics;

	private VehicleTerrainHandler serverTerrainHandler;

	private Vector3 customInertiaTensor = new Vector3(25000f, 11000f, 19000f);

	private float extensionArmState;

	private float raiseArmState;

	private float yawState = 1f;

	[Header("Magnet Crane")]
	public Animator animator;

	[SerializeField]
	private Transform COM;

	[SerializeField]
	private float arm1Speed = 0.01f;

	[SerializeField]
	private float arm2Speed = 0.01f;

	[SerializeField]
	private float turnYawSpeed = 0.01f;

	[SerializeField]
	private BaseMagnet Magnet;

	[SerializeField]
	private MagnetCraneAudio mcAudio;

	[SerializeField]
	private Rigidbody myRigidbody;

	[SerializeField]
	private Transform[] collisionTestingPoints;

	[SerializeField]
	private float maxDistanceFromOrigin;

	[SerializeField]
	private GameObjectRef selfDamageEffect;

	[SerializeField]
	private GameObjectRef explosionEffect;

	[SerializeField]
	private Transform explosionPoint;

	[SerializeField]
	private CapsuleCollider driverCollision;

	[SerializeField]
	private Transform leftHandTarget;

	[SerializeField]
	private Transform rightHandTarget;

	[SerializeField]
	private Transform leftFootTarget;

	[SerializeField]
	private Transform rightFootTarget;

	[SerializeField]
	private float idleFuelPerSec;

	[SerializeField]
	private float maxFuelPerSec;

	[SerializeField]
	private GameObject[] OnTriggers;

	[SerializeField]
	private TriggerHurtEx magnetDamage;

	[SerializeField]
	private int engineKW = 250;

	[SerializeField]
	private CarWheel[] wheels;

	[SerializeField]
	private CarSettings carSettings;

	[SerializeField]
	private ParticleSystem exhaustInner;

	[SerializeField]
	private ParticleSystem exhaustOuter;

	[SerializeField]
	private EmissionToggle lightToggle;

	public static readonly Phrase ReturnMessage = new Phrase("junkyardcrane.return", "Return to the Junkyard. Excessive damage will occur.");

	private const Flags Flag_ArmMovement = Flags.Reserved7;

	private const Flags Flag_BaseMovementInput = Flags.Reserved10;

	private static int leftTreadParam = Animator.StringToHash("left tread movement");

	private static int rightTreadParam = Animator.StringToHash("right tread movement");

	private static int yawParam = Animator.StringToHash("Yaw");

	private static int arm1Param = Animator.StringToHash("Arm_01");

	private static int arm2Param = Animator.StringToHash("Arm_02");

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

	public override float DriveWheelVelocity => GetSpeed();

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("MagnetCrane.OnRpcMessage", 0);
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
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	public override void ServerInit()
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		base.ServerInit();
		((FacepunchBehaviour)this).InvokeRepeating((Action)UpdateParams, 0f, 0.1f);
		animator.cullingMode = (AnimatorCullingMode)0;
		animator.updateMode = (AnimatorUpdateMode)1;
		myRigidbody.centerOfMass = COM.localPosition;
		carPhysics = new CarPhysics<MagnetCrane>(this, ((Component)this).transform, rigidBody, carSettings);
		serverTerrainHandler = new VehicleTerrainHandler(this);
		Magnet.SetMagnetEnabled(wantsOn: false, null);
		spawnOrigin = ((Component)this).transform.position;
		lastDrivenTime = Time.realtimeSinceStartup;
		GameObject[] onTriggers = OnTriggers;
		foreach (GameObject val in onTriggers)
		{
			val.SetActive(false);
		}
	}

	public override void PlayerServerInput(InputState inputState, BasePlayer player)
	{
		base.PlayerServerInput(inputState, player);
		if (!IsDriver(player))
		{
			return;
		}
		throttleInput = 0f;
		steerInput = 0f;
		extensionInput = 0f;
		yawInput = 0f;
		raiseArmInput = 0f;
		if (engineController.IsOff)
		{
			if (inputState.IsAnyDown())
			{
				engineController.TryStartEngine(player);
			}
		}
		else if (engineController.IsOn)
		{
			bool flag = inputState.IsDown(BUTTON.SPRINT);
			if (inputState.IsDown(BUTTON.RELOAD) && Time.realtimeSinceStartup > nextToggleTime)
			{
				Magnet.SetMagnetEnabled(!Magnet.IsMagnetOn(), player);
				nextToggleTime = Time.realtimeSinceStartup + 0.5f;
			}
			if (flag)
			{
				float speed = GetSpeed();
				float num = 0f;
				if (inputState.IsDown(BUTTON.FORWARD))
				{
					num = 1f;
				}
				else if (inputState.IsDown(BUTTON.BACKWARD))
				{
					num = -1f;
				}
				if (speed > 1f && num < 0f)
				{
					throttleInput = 0f;
					brakeInput = 0f - num;
				}
				else if (speed < -1f && num > 0f)
				{
					throttleInput = 0f;
					brakeInput = num;
				}
				else
				{
					throttleInput = num;
					brakeInput = 0f;
				}
				if (inputState.IsDown(BUTTON.RIGHT))
				{
					steerInput = -1f;
				}
				if (inputState.IsDown(BUTTON.LEFT))
				{
					steerInput = 1f;
				}
			}
			else
			{
				if (inputState.IsDown(BUTTON.LEFT))
				{
					yawInput = 1f;
				}
				else if (inputState.IsDown(BUTTON.RIGHT))
				{
					yawInput = -1f;
				}
				else if (inputState.IsDown(BUTTON.DUCK))
				{
					float @float = animator.GetFloat(yawParam);
					if (@float > 0.01f && @float < 0.99f)
					{
						yawInput = ((@float <= 0.5f) ? (-1f) : 1f);
					}
				}
				if (inputState.IsDown(BUTTON.FORWARD))
				{
					raiseArmInput = 1f;
				}
				else if (inputState.IsDown(BUTTON.BACKWARD))
				{
					raiseArmInput = -1f;
				}
			}
			if (inputState.IsDown(BUTTON.FIRE_PRIMARY))
			{
				extensionInput = 1f;
			}
			if (inputState.IsDown(BUTTON.FIRE_SECONDARY))
			{
				extensionInput = -1f;
			}
		}
		handbrakeOn = throttleInput == 0f && steerInput == 0f;
	}

	public override float MaxVelocity()
	{
		float num = GetMaxForwardSpeed() * 1.3f;
		return Mathf.Max(num, 30f);
	}

	public float GetSteerInput()
	{
		return steerInput;
	}

	public bool GetSteerModInput()
	{
		return false;
	}

	public override void OnEngineStartFailed()
	{
	}

	public override bool MeetsEngineRequirements()
	{
		return HasDriver();
	}

	public override void VehicleFixedUpdate()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0393: Unknown result type (might be due to invalid IL or missing references)
		//IL_0398: Unknown result type (might be due to invalid IL or missing references)
		//IL_039a: Unknown result type (might be due to invalid IL or missing references)
		//IL_03df: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e4: Unknown result type (might be due to invalid IL or missing references)
		base.VehicleFixedUpdate();
		rigidBody.ResetInertiaTensor();
		rigidBody.inertiaTensor = Vector3.Lerp(rigidBody.inertiaTensor, customInertiaTensor, 0.5f);
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		float num = Mathf.Clamp(realtimeSinceStartup - lastFixedUpdateTime, 0f, 0.5f);
		lastFixedUpdateTime = realtimeSinceStartup;
		float speed = GetSpeed();
		carPhysics.FixedUpdate(Time.fixedDeltaTime, speed);
		serverTerrainHandler.FixedUpdate();
		bool flag = IsOn();
		if (IsOn())
		{
			float num2 = Mathf.Max(Mathf.Abs(throttleInput), Mathf.Abs(steerInput));
			float num3 = Mathf.Lerp(idleFuelPerSec, maxFuelPerSec, num2);
			if (!Magnet.HasConnectedObject())
			{
				num3 = Mathf.Min(num3, maxFuelPerSec * 0.75f);
			}
			engineController.TickFuel(num3);
		}
		engineController.CheckEngineState();
		if (IsOn() != flag)
		{
			GameObject[] onTriggers = OnTriggers;
			foreach (GameObject val in onTriggers)
			{
				val.SetActive(IsOn());
			}
		}
		if (Vector3.Dot(((Component)this).transform.up, Vector3.down) >= 0.4f)
		{
			Kill(DestroyMode.Gib);
			return;
		}
		if (realtimeSinceStartup > lastDrivenTime + 14400f)
		{
			Kill(DestroyMode.Gib);
			return;
		}
		if (spawnOrigin != Vector3.zero && maxDistanceFromOrigin != 0f)
		{
			float num4 = Vector3Ex.Distance2D(((Component)this).transform.position, spawnOrigin);
			if (num4 > maxDistanceFromOrigin)
			{
				if (Vector3Ex.Distance2D(((Component)this).transform.position, lastDamagePos) > 6f)
				{
					if ((Object)(object)GetDriver() != (Object)null)
					{
						GetDriver().ShowToast(GameTip.Styles.Red_Normal, ReturnMessage);
					}
					Hurt(MaxHealth() * 0.15f, DamageType.Generic, this, useProtection: false);
					lastDamagePos = ((Component)this).transform.position;
					nextSelfHealTime = realtimeSinceStartup + 3600f;
					Effect.server.Run(selfDamageEffect.resourcePath, ((Component)this).transform.position + Vector3.up * 2f, Vector3.up);
					return;
				}
			}
			else if (base.healthFraction < 1f && realtimeSinceStartup > nextSelfHealTime && base.SecondsSinceAttacked > 600f)
			{
				Heal(1000f);
			}
		}
		if (!HasDriver() || !IsOn())
		{
			handbrakeOn = true;
			throttleInput = 0f;
			steerInput = 0f;
			SetFlag(Flags.Reserved10, b: false);
			Magnet.SetMagnetEnabled(wantsOn: false, null);
		}
		else
		{
			lastDrivenTime = realtimeSinceStartup;
			if (Magnet.IsMagnetOn() && Magnet.HasConnectedObject())
			{
				OBB connectedOBB = Magnet.GetConnectedOBB(0.75f);
				if (GamePhysics.CheckOBB(connectedOBB, 1084293121, (QueryTriggerInteraction)1))
				{
					Magnet.SetMagnetEnabled(wantsOn: false, null);
					nextToggleTime = realtimeSinceStartup + 2f;
					Effect.server.Run(selfDamageEffect.resourcePath, ((Component)Magnet).transform.position, Vector3.up);
				}
			}
		}
		extensionMove = UpdateMoveInput(extensionInput, extensionMove, 3f, Time.fixedDeltaTime);
		yawMove = UpdateMoveInput(yawInput, yawMove, 3f, Time.fixedDeltaTime);
		raiseArmMove = UpdateMoveInput(raiseArmInput, raiseArmMove, 3f, Time.fixedDeltaTime);
		bool flag2 = extensionInput != 0f || raiseArmInput != 0f || yawInput != 0f;
		SetFlag(Flags.Reserved7, flag2);
		magnetDamage.damageEnabled = IsOn() && flag2;
		extensionArmState += extensionInput * arm1Speed * num;
		raiseArmState += raiseArmInput * arm2Speed * num;
		yawState += yawInput * turnYawSpeed * num;
		yawState %= 1f;
		if (yawState < 0f)
		{
			yawState += 1f;
		}
		extensionArmState = Mathf.Clamp(extensionArmState, -1f, 1f);
		raiseArmState = Mathf.Clamp(raiseArmState, -1f, 1f);
		UpdateAnimator(Time.fixedDeltaTime);
		Magnet.MagnetThink(Time.fixedDeltaTime);
		SetFlag(Flags.Reserved10, throttleInput != 0f || steerInput != 0f);
		static float UpdateMoveInput(float input, float move, float slowRate, float dt)
		{
			if (input != 0f)
			{
				return input;
			}
			return Mathf.MoveTowards(move, 0f, dt * slowRate);
		}
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		info.msg.crane = Pool.Get<Crane>();
		info.msg.crane.arm1 = extensionArmState;
		info.msg.crane.arm2 = raiseArmState;
		info.msg.crane.yaw = yawState;
		info.msg.crane.time = GetNetworkTime();
		byte b = (byte)((carPhysics.TankThrottleLeft + 1f) * 7f);
		byte b2 = (byte)((carPhysics.TankThrottleRight + 1f) * 7f);
		byte treadInput = (byte)(b + (b2 << 4));
		info.msg.crane.treadInput = treadInput;
	}

	public void UpdateParams()
	{
		SendNetworkUpdate();
	}

	public void LateUpdate()
	{
		if (!base.isClient)
		{
			if (HasDriver() && IsColliding())
			{
				extensionArmState = lastExtensionArmState;
				raiseArmState = lastRaiseArmState;
				yawState = lastYawState;
				extensionInput = 0f - extensionInput;
				yawInput = 0f - yawInput;
				raiseArmInput = 0f - raiseArmInput;
				UpdateAnimator(Time.deltaTime);
			}
			else
			{
				lastExtensionArmState = extensionArmState;
				lastRaiseArmState = raiseArmState;
				lastYawState = yawState;
			}
		}
	}

	public override void OnAttacked(HitInfo info)
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		if (base.isServer)
		{
			BasePlayer driver = GetDriver();
			if ((Object)(object)driver != (Object)null && info.damageTypes.Has(DamageType.Bullet))
			{
				Capsule val = default(Capsule);
				((Capsule)(ref val))._002Ector(((Component)driverCollision).transform.position, driverCollision.radius, driverCollision.height);
				float num = Vector3.Distance(info.PointStart, info.PointEnd);
				Ray val2 = default(Ray);
				((Ray)(ref val2))._002Ector(info.PointStart, Vector3Ex.Direction(info.PointEnd, info.PointStart));
				RaycastHit val3 = default(RaycastHit);
				if (((Capsule)(ref val)).Trace(val2, ref val3, 0.05f, num * 1.2f))
				{
					driver.Hurt(info.damageTypes.Total() * 0.15f, DamageType.Bullet, info.Initiator);
				}
			}
		}
		base.OnAttacked(info);
	}

	public override void OnKilled(HitInfo info)
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		if (HasDriver())
		{
			BasePlayer driver = GetDriver();
			driver.Hurt(10000f, DamageType.Blunt, info.Initiator, useProtection: false);
		}
		if (explosionEffect.isValid)
		{
			Effect.server.Run(explosionEffect.resourcePath, explosionPoint.position, Vector3.up);
		}
		base.OnKilled(info);
	}

	public bool IsColliding()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		Transform[] array = collisionTestingPoints;
		OBB obb = default(OBB);
		foreach (Transform val in array)
		{
			if (((Component)val).gameObject.activeSelf)
			{
				Vector3 position = val.position;
				Quaternion rotation = val.rotation;
				((OBB)(ref obb))._002Ector(position, new Vector3(val.localScale.x, val.localScale.y, val.localScale.z), rotation);
				if (GamePhysics.CheckOBB(obb, 1084293121, (QueryTriggerInteraction)1))
				{
					return true;
				}
			}
		}
		return false;
	}

	public float GetMaxDriveForce()
	{
		return (float)engineKW * 10f;
	}

	public float GetAdjustedDriveForce(float absSpeed, float topSpeed)
	{
		float num = MathEx.BiasedLerp(1f - absSpeed / topSpeed, 0.5f);
		num = Mathf.Lerp(num, 1f, Mathf.Abs(steerInput));
		return GetMaxDriveForce() * num;
	}

	public CarWheel[] GetWheels()
	{
		return wheels;
	}

	public float GetWheelsMidPos()
	{
		return 0f;
	}

	public void UpdateAnimator(float dt)
	{
		animator.SetFloat("Arm_01", extensionArmState);
		animator.SetFloat("Arm_02", raiseArmState);
		animator.SetFloat("Yaw", yawState);
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

	public override float GetThrottleInput()
	{
		if (base.isServer)
		{
			return throttleInput;
		}
		throw new NotImplementedException("We don't know magnet crane throttle input on the client.");
	}

	public override float GetBrakeInput()
	{
		if (base.isServer)
		{
			if (handbrakeOn)
			{
				return 1f;
			}
			return brakeInput;
		}
		throw new NotImplementedException("We don't know magnet crane brake input on the client.");
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (info.msg.crane != null && base.isServer)
		{
			yawState = info.msg.crane.yaw;
			extensionArmState = info.msg.crane.arm1;
			raiseArmState = info.msg.crane.arm2;
		}
	}

	public override float GetMaxForwardSpeed()
	{
		return 13f;
	}

	public override bool CanBeLooted(BasePlayer player)
	{
		if (!base.CanBeLooted(player))
		{
			return false;
		}
		return PlayerIsMounted(player) || !IsOn();
	}
}
