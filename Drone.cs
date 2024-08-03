using System.Collections.Generic;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;

public class Drone : RemoteControlEntity, IRemoteControllableClientCallbacks, IRemoteControllable
{
	private struct DroneInputState
	{
		public Vector3 movement;

		public float throttle;

		public float pitch;

		public float yaw;

		public void Reset()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			movement = Vector3.zero;
			pitch = 0f;
			yaw = 0f;
		}
	}

	[ReplicatedVar(Help = "How far drones can be flown away from the controlling computer station", ShowInAdminUI = true, Default = "250")]
	public static float maxControlRange = 500f;

	[ServerVar(Help = "If greater than zero, overrides the drone's planar movement speed")]
	public static float movementSpeedOverride = 0f;

	[ServerVar(Help = "If greater than zero, overrides the drone's vertical movement speed")]
	public static float altitudeSpeedOverride = 0f;

	[ClientVar(ClientAdmin = true)]
	public static float windTimeDivisor = 10f;

	[ClientVar(ClientAdmin = true)]
	public static float windPositionDivisor = 100f;

	[ClientVar(ClientAdmin = true)]
	public static float windPositionScale = 1f;

	[ClientVar(ClientAdmin = true)]
	public static float windRotationMultiplier = 45f;

	[ClientVar(ClientAdmin = true)]
	public static float windLerpSpeed = 0.1f;

	private const Flags Flag_ThrottleUp = Flags.Reserved1;

	private const Flags Flag_Flying = Flags.Reserved2;

	[Header("Drone")]
	public Rigidbody body;

	public Transform modelRoot;

	public bool killInWater = true;

	public bool enableGrounding = true;

	public bool keepAboveTerrain = true;

	public float groundTraceDist = 0.1f;

	public float groundCheckInterval = 0.05f;

	public float altitudeAcceleration = 10f;

	public float movementAcceleration = 10f;

	public float yawSpeed = 2f;

	public float uprightSpeed = 2f;

	public float uprightPrediction = 0.15f;

	public float uprightDot = 0.5f;

	public float leanWeight = 0.1f;

	public float leanMaxVelocity = 5f;

	public float hurtVelocityThreshold = 3f;

	public float hurtDamagePower = 3f;

	public float collisionDisableTime = 0.25f;

	public float pitchMin = -60f;

	public float pitchMax = 60f;

	public float pitchSensitivity = -5f;

	public bool disableWhenHurt = false;

	[Range(0f, 1f)]
	public float disableWhenHurtChance = 0.25f;

	public float playerCheckInterval = 0.1f;

	public float playerCheckRadius = 0f;

	public float deployYOffset = 0.1f;

	[Header("Sound")]
	public SoundDefinition movementLoopSoundDef;

	public SoundDefinition movementStartSoundDef;

	public SoundDefinition movementStopSoundDef;

	public AnimationCurve movementLoopPitchCurve;

	public float movementSpeedReference = 50f;

	[Header("Animation")]
	public float propellerMaxSpeed = 1000f;

	public float propellerAcceleration = 3f;

	public Transform propellerA;

	public Transform propellerB;

	public Transform propellerC;

	public Transform propellerD;

	private float pitch;

	protected Vector3? targetPosition;

	private DroneInputState currentInput;

	private float lastInputTime;

	private double lastCollision = -1000.0;

	private TimeSince lastGroundCheck;

	private bool isGrounded;

	private RealTimeSinceEx lastPlayerCheck;

	public override bool RequiresMouse => true;

	public override float MaxRange => maxControlRange;

	public override bool CanAcceptInput => true;

	protected override bool PositionTickFixedTime => true;

	public override void Spawn()
	{
		base.Spawn();
		isGrounded = true;
	}

	public override void StopControl(CameraViewerId viewerID)
	{
		CameraViewerId? controllingViewerId = base.ControllingViewerId;
		if (viewerID == controllingViewerId)
		{
			SetFlag(Flags.Reserved1, b: false, recursive: false, networkupdate: false);
			SetFlag(Flags.Reserved2, b: false, recursive: false, networkupdate: false);
			pitch = 0f;
			SendNetworkUpdate();
		}
		base.StopControl(viewerID);
	}

	public override void UserInput(InputState inputState, CameraViewerId viewerID)
	{
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		CameraViewerId? controllingViewerId = base.ControllingViewerId;
		if (!(viewerID != controllingViewerId))
		{
			currentInput.Reset();
			int num = (inputState.IsDown(BUTTON.FORWARD) ? 1 : 0) + (inputState.IsDown(BUTTON.BACKWARD) ? (-1) : 0);
			int num2 = (inputState.IsDown(BUTTON.RIGHT) ? 1 : 0) + (inputState.IsDown(BUTTON.LEFT) ? (-1) : 0);
			ref DroneInputState reference = ref currentInput;
			Vector3 val = new Vector3((float)num2, 0f, (float)num);
			reference.movement = ((Vector3)(ref val)).normalized;
			currentInput.throttle = (inputState.IsDown(BUTTON.SPRINT) ? 1 : 0) + (inputState.IsDown(BUTTON.DUCK) ? (-1) : 0);
			currentInput.yaw = inputState.current.mouseDelta.x;
			currentInput.pitch = inputState.current.mouseDelta.y;
			lastInputTime = Time.time;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = currentInput.throttle > 0f;
			if (flag3 != HasFlag(Flags.Reserved1))
			{
				SetFlag(Flags.Reserved1, flag3, recursive: false, networkupdate: false);
				flag = true;
			}
			float num3 = pitch;
			pitch += currentInput.pitch * pitchSensitivity;
			pitch = Mathf.Clamp(pitch, pitchMin, pitchMax);
			if (!Mathf.Approximately(pitch, num3))
			{
				flag2 = true;
			}
			if (flag2)
			{
				SendNetworkUpdateImmediate();
			}
			else if (flag)
			{
				SendNetworkUpdate_Flags();
			}
		}
	}

	protected virtual void Update_Server()
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		if (!base.isServer || IsDead() || base.IsBeingControlled || !targetPosition.HasValue)
		{
			return;
		}
		Vector3 position = ((Component)this).transform.position;
		float height = TerrainMeta.HeightMap.GetHeight(position);
		Vector3 val = targetPosition.Value - body.velocity * 0.5f;
		if (keepAboveTerrain)
		{
			val.y = Mathf.Max(val.y, height + 1f);
		}
		Vector2 val2 = Vector3Ex.XZ2D(val);
		Vector2 val3 = Vector3Ex.XZ2D(position);
		Vector3 val4 = default(Vector3);
		float num = default(float);
		Vector3Ex.ToDirectionAndMagnitude(Vector3Ex.XZ3D(val2 - val3), ref val4, ref num);
		currentInput.Reset();
		lastInputTime = Time.time;
		float num2 = position.y - height;
		if (num2 > 1f)
		{
			float num3 = Mathf.Clamp01(num);
			currentInput.movement = ((Component)this).transform.InverseTransformVector(val4) * num3;
			if (num > 0.5f)
			{
				Quaternion val5 = ((Component)this).transform.rotation;
				float y = ((Quaternion)(ref val5)).eulerAngles.y;
				val5 = Quaternion.FromToRotation(Vector3.forward, val4);
				float y2 = ((Quaternion)(ref val5)).eulerAngles.y;
				currentInput.yaw = Mathf.Clamp(Mathf.LerpAngle(y, y2, Time.deltaTime) - y, -2f, 2f);
			}
		}
		currentInput.throttle = Mathf.Clamp(val.y - position.y, -1f, 1f);
	}

	public void FixedUpdate()
	{
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0304: Unknown result type (might be due to invalid IL or missing references)
		//IL_0309: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_031f: Unknown result type (might be due to invalid IL or missing references)
		//IL_031a: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0408: Unknown result type (might be due to invalid IL or missing references)
		//IL_040d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0412: Unknown result type (might be due to invalid IL or missing references)
		//IL_0414: Unknown result type (might be due to invalid IL or missing references)
		//IL_0419: Unknown result type (might be due to invalid IL or missing references)
		//IL_041b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0426: Unknown result type (might be due to invalid IL or missing references)
		//IL_042b: Unknown result type (might be due to invalid IL or missing references)
		//IL_034a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0329: Unknown result type (might be due to invalid IL or missing references)
		//IL_034f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0357: Unknown result type (might be due to invalid IL or missing references)
		//IL_0367: Unknown result type (might be due to invalid IL or missing references)
		//IL_0442: Unknown result type (might be due to invalid IL or missing references)
		//IL_0446: Unknown result type (might be due to invalid IL or missing references)
		//IL_044b: Unknown result type (might be due to invalid IL or missing references)
		//IL_044f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0454: Unknown result type (might be due to invalid IL or missing references)
		//IL_0459: Unknown result type (might be due to invalid IL or missing references)
		//IL_0461: Unknown result type (might be due to invalid IL or missing references)
		//IL_0465: Unknown result type (might be due to invalid IL or missing references)
		//IL_0343: Unknown result type (might be due to invalid IL or missing references)
		//IL_0385: Unknown result type (might be due to invalid IL or missing references)
		//IL_038a: Unknown result type (might be due to invalid IL or missing references)
		//IL_038c: Unknown result type (might be due to invalid IL or missing references)
		//IL_038e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0390: Unknown result type (might be due to invalid IL or missing references)
		//IL_0395: Unknown result type (might be due to invalid IL or missing references)
		//IL_0397: Unknown result type (might be due to invalid IL or missing references)
		//IL_039c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
		if (!base.isServer || IsDead())
		{
			return;
		}
		float num = WaterFactor();
		if (killInWater && num > 0f)
		{
			if (num > 0.99f)
			{
				Kill();
			}
			return;
		}
		if ((!base.IsBeingControlled && !targetPosition.HasValue) || (isGrounded && currentInput.throttle <= 0f))
		{
			if (HasFlag(Flags.Reserved2))
			{
				SetFlag(Flags.Reserved2, b: false, recursive: false, networkupdate: false);
				SendNetworkUpdate_Flags();
			}
			return;
		}
		if (playerCheckRadius > 0f && (double)lastPlayerCheck > (double)playerCheckInterval)
		{
			lastPlayerCheck = 0.0;
			List<BasePlayer> list = Pool.GetList<BasePlayer>();
			Vis.Entities(((Component)this).transform.position, playerCheckRadius, list, 131072, (QueryTriggerInteraction)2);
			if (list.Count > 0)
			{
				lastCollision = TimeEx.currentTimestamp;
			}
			Pool.FreeList<BasePlayer>(ref list);
		}
		double currentTimestamp = TimeEx.currentTimestamp;
		bool flag = lastCollision > 0.0 && currentTimestamp - lastCollision < (double)collisionDisableTime;
		if (enableGrounding)
		{
			if (TimeSince.op_Implicit(lastGroundCheck) >= groundCheckInterval)
			{
				lastGroundCheck = TimeSince.op_Implicit(0f);
				RaycastHit val = default(RaycastHit);
				bool flag2 = body.SweepTest(Vector3.down, ref val, groundTraceDist, (QueryTriggerInteraction)1);
				if (!flag2 && isGrounded)
				{
					lastPlayerCheck = playerCheckInterval;
				}
				isGrounded = flag2;
			}
		}
		else
		{
			isGrounded = false;
		}
		Vector3 val2 = ((Component)this).transform.TransformDirection(currentInput.movement);
		Vector3 val3 = default(Vector3);
		float num2 = default(float);
		Vector3Ex.ToDirectionAndMagnitude(Vector3Ex.WithY(body.velocity, 0f), ref val3, ref num2);
		float num3 = Mathf.Clamp01(num2 / leanMaxVelocity);
		Vector3 val4 = (Mathf.Approximately(((Vector3)(ref val2)).sqrMagnitude, 0f) ? ((0f - num3) * val3) : val2);
		Vector3 val5 = Vector3.up + val4 * leanWeight * num3;
		Vector3 normalized = ((Vector3)(ref val5)).normalized;
		Vector3 up = ((Component)this).transform.up;
		float num4 = Mathf.Max(Vector3.Dot(normalized, up), 0f);
		if (!flag || isGrounded)
		{
			Vector3 val6 = ((isGrounded && currentInput.throttle <= 0f) ? Vector3.zero : (-1f * ((Component)this).transform.up * Physics.gravity.y));
			Vector3 val7 = (isGrounded ? Vector3.zero : (val2 * ((movementSpeedOverride > 0f) ? movementSpeedOverride : movementAcceleration)));
			Vector3 val8 = ((Component)this).transform.up * currentInput.throttle * ((altitudeSpeedOverride > 0f) ? altitudeSpeedOverride : altitudeAcceleration);
			Vector3 val9 = val6 + val7 + val8;
			body.AddForce(val9 * num4, (ForceMode)5);
		}
		if (!flag && !isGrounded)
		{
			Vector3 val10 = ((Component)this).transform.TransformVector(0f, currentInput.yaw * yawSpeed, 0f);
			Vector3 val11 = Vector3.Cross(Quaternion.Euler(body.angularVelocity * uprightPrediction) * up, normalized) * uprightSpeed;
			float num5 = ((num4 < uprightDot) ? 0f : num4);
			Vector3 val12 = val10 * num4 + val11 * num5;
			body.AddTorque(val12 * num4, (ForceMode)5);
		}
		bool flag3 = !flag;
		if (flag3 != HasFlag(Flags.Reserved2))
		{
			SetFlag(Flags.Reserved2, flag3, recursive: false, networkupdate: false);
			SendNetworkUpdate_Flags();
		}
	}

	public void OnCollisionEnter(Collision collision)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		if (base.isServer)
		{
			lastCollision = TimeEx.currentTimestamp;
			Vector3 relativeVelocity = collision.relativeVelocity;
			float magnitude = ((Vector3)(ref relativeVelocity)).magnitude;
			if (magnitude > hurtVelocityThreshold)
			{
				Hurt(Mathf.Pow(magnitude, hurtDamagePower), DamageType.Fall, null, useProtection: false);
			}
		}
	}

	public void OnCollisionStay()
	{
		if (base.isServer)
		{
			lastCollision = TimeEx.currentTimestamp;
		}
	}

	public override void Hurt(HitInfo info)
	{
		base.Hurt(info);
		if (base.isServer && disableWhenHurt && info.damageTypes.GetMajorityDamageType() != DamageType.Fall && Random.value < disableWhenHurtChance)
		{
			lastCollision = TimeEx.currentTimestamp;
		}
	}

	public override float GetNetworkTime()
	{
		return Time.fixedTime;
	}

	public override Vector3 GetLocalVelocityServer()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)body == (Object)null)
		{
			return Vector3.zero;
		}
		return body.velocity;
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		if (!info.forDisk)
		{
			info.msg.drone = Pool.Get<Drone>();
			info.msg.drone.pitch = pitch;
		}
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (info.msg.drone != null)
		{
			pitch = info.msg.drone.pitch;
		}
	}

	public virtual void Update()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		Update_Server();
		if (HasFlag(Flags.Reserved2))
		{
			Quaternion localRotation = viewEyes.localRotation;
			Vector3 eulerAngles = ((Quaternion)(ref localRotation)).eulerAngles;
			eulerAngles.x = Mathf.LerpAngle(eulerAngles.x, pitch, 0.1f);
			viewEyes.localRotation = Quaternion.Euler(eulerAngles);
		}
	}

	protected override bool CanChangeID(BasePlayer player)
	{
		return (Object)(object)player != (Object)null && base.OwnerID == player.userID && !HasFlag(Flags.Reserved2);
	}

	public override bool CanPickup(BasePlayer player)
	{
		return base.CanPickup(player) && !HasFlag(Flags.Reserved2);
	}

	public override void OnPickedUpPreItemMove(Item createdItem, BasePlayer player)
	{
		base.OnPickedUpPreItemMove(createdItem, player);
		if ((Object)(object)player != (Object)null && player.userID == base.OwnerID)
		{
			createdItem.text = GetIdentifier();
		}
	}

	public override void OnDeployed(BaseEntity parent, BasePlayer deployedBy, Item fromItem)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		base.OnDeployed(parent, deployedBy, fromItem);
		Transform transform = ((Component)this).transform;
		transform.position += ((Component)this).transform.up * deployYOffset;
		if ((Object)(object)body != (Object)null)
		{
			body.velocity = Vector3.zero;
			body.angularVelocity = Vector3.zero;
		}
		if (fromItem != null && !string.IsNullOrEmpty(fromItem.text) && ComputerStation.IsValidIdentifier(fromItem.text))
		{
			UpdateIdentifier(fromItem.text);
		}
	}

	public override bool ShouldNetworkOwnerInfo()
	{
		return true;
	}

	public override bool ShouldInheritNetworkGroup()
	{
		return false;
	}

	public override float MaxVelocity()
	{
		return 30f;
	}
}
