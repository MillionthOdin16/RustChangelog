using System;
using Facepunch.Rust;
using Rust;
using UnityEngine;

public class Parachute : BaseVehicle, SamSite.ISamSiteTarget
{
	public Collider ParachuteCollider;

	public ItemDefinition PackedParachute;

	public GameObjectRef DetachedParachute;

	public Transform DetachedSpawnPoint;

	public float ConditionLossPerUse = 0.2f;

	public float HurtDeployTime = 1f;

	public float HurtAmount = 80f;

	public Animator ColliderAnimator;

	public Animator ColliderWorldAnimator;

	public float UprightLerpForce = 5f;

	public float ConstantForwardForce = 2f;

	public ForceMode ForwardForceMode = (ForceMode)5;

	public float TurnForce = 2f;

	public ForceMode TurnForceMode = (ForceMode)5;

	public float ForwardTiltAcceleration = 2f;

	public float BackInputForceMultiplier = 0.2f;

	public float DeployAnimationLength = 3f;

	public float TargetDrag = 1f;

	public float TargetAngularDrag = 1f;

	public AnimationCurve DragCurve = new AnimationCurve();

	public AnimationCurve DragDamageCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);

	public AnimationCurve MassDamageCurve = AnimationCurve.Linear(0f, 30f, 1f, 1f);

	public AnimationCurve DamageHorizontalVelocityCurve = AnimationCurve.Linear(0f, 5f, 1f, 20f);

	[Range(0f, 1f)]
	public float DamageTester = 1f;

	public float AnimationInputSmoothness = 1f;

	public Vector2 AnimationInputScale = new Vector2(0.5f, 0.5f);

	public ParachuteWearable FirstPersonCanopy;

	public GameObjectRef ParachuteLandScreenBounce;

	private static int AnimatorInputXParameter = Animator.StringToHash("InputX");

	private static int AnimatorInputYParameter = Animator.StringToHash("InputY");

	private TimeSince mountTime;

	public const Flags Flag_InputForward = Flags.Reserved1;

	public const Flags Flag_InputBack = Flags.Reserved2;

	public const Flags Flag_InputLeft = Flags.Reserved3;

	public const Flags Flag_InputRight = Flags.Reserved4;

	public SoundDefinition deploySoundDef;

	public SoundDefinition releaseSoundDef;

	public SoundDefinition flightLoopSoundDef;

	public SoundDefinition steerSoundDef;

	public AnimationCurve flightLoopPitchCurve;

	public AnimationCurve flightLoopGainCurve;

	[ServerVar(Saved = true)]
	public static bool BypassRepack = false;

	[ServerVar(Saved = true)]
	public static bool LandingAnimations = false;

	private bool collisionDeath;

	private Vector3 collisionImpulse = Vector3.zero;

	private float startHeight;

	private float distanceTravelled;

	private Vector3 lastPosition = Vector3.zero;

	private Vector2 lerpedInput = Vector2.zero;

	private Vector3 collisionLocalPos;

	private Vector3 collisionWorldNormal;

	public SamSite.SamTargetType SAMTargetType => SamSite.targetTypeVehicle;

	public override void PlayerMounted(BasePlayer player, BaseMountable seat)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		base.PlayerMounted(player, seat);
		rigidBody.velocity = player.estimatedVelocity;
		mountTime = TimeSince.op_Implicit(0f);
		startHeight = ((Component)this).transform.position.y;
		distanceTravelled = 0f;
		canTriggerParent = false;
	}

	public override bool GetDismountPosition(BasePlayer player, out Vector3 res)
	{
		ParachuteCollider.enabled = false;
		bool dismountPosition = base.GetDismountPosition(player, out res);
		ParachuteCollider.enabled = true;
		return dismountPosition;
	}

	public override void PlayerServerInput(InputState inputState, BasePlayer player)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		base.PlayerServerInput(inputState, player);
		player.PlayHeavyLandingAnimation = true;
		Vector3 position = ((Component)this).transform.position;
		float num = Vector3.Distance(lastPosition, position);
		distanceTravelled += num;
		lastPosition = position;
		if (WaterLevel.Test(((Component)this).transform.position, waves: true, volumes: true, this))
		{
			DismountAllPlayers();
		}
		else if (!(TimeSince.op_Implicit(mountTime) < DeployAnimationLength))
		{
			Vector2 val = ProcessInputVector(inputState, player);
			lerpedInput = Vector2.Lerp(lerpedInput, val, Time.deltaTime * 5f);
			ColliderAnimator.SetFloat(AnimatorInputXParameter, lerpedInput.x);
			ColliderAnimator.SetFloat(AnimatorInputYParameter, lerpedInput.y);
			SetFlag(Flags.Reserved1, inputState.IsDown(BUTTON.FORWARD), recursive: false, networkupdate: false);
			SetFlag(Flags.Reserved2, inputState.IsDown(BUTTON.BACKWARD), recursive: false, networkupdate: false);
			SetFlag(Flags.Reserved3, inputState.IsDown(BUTTON.LEFT), recursive: false, networkupdate: false);
			SetFlag(Flags.Reserved4, inputState.IsDown(BUTTON.RIGHT));
		}
	}

	public override void VehicleFixedUpdate()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0304: Unknown result type (might be due to invalid IL or missing references)
		//IL_0308: Unknown result type (might be due to invalid IL or missing references)
		//IL_0320: Unknown result type (might be due to invalid IL or missing references)
		//IL_033c: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		base.VehicleFixedUpdate();
		float num = base.healthFraction * DamageTester;
		float num2 = DragCurve.Evaluate(TimeSince.op_Implicit(mountTime));
		float num3 = DragDamageCurve.Evaluate(num);
		float mass = MassDamageCurve.Evaluate(num);
		rigidBody.mass = mass;
		rigidBody.drag = Mathf.Lerp(0f, TargetDrag * num3, num2);
		rigidBody.angularDrag = Mathf.Lerp(0f, TargetAngularDrag * num3, num2);
		float num4 = Mathf.Clamp01(TimeSince.op_Implicit(mountTime) / 1f);
		Vector3 forward = ((Component)this).transform.forward;
		Vector3 val = (forward * ConstantForwardForce + forward * (ForwardTiltAcceleration * Mathf.Clamp(lerpedInput.y, 0f, 1f))) * Time.fixedDeltaTime * num4;
		if (lerpedInput.y < -0.1f)
		{
			val *= 1f - BackInputForceMultiplier * Mathf.Abs(lerpedInput.y);
		}
		val *= num;
		rigidBody.AddForce(val, ForwardForceMode);
		Quaternion rotation;
		if (lerpedInput.x != 0f)
		{
			rotation = rigidBody.rotation;
			Quaternion val2 = Quaternion.Euler(Vector3Ex.WithZ(((Quaternion)(ref rotation)).eulerAngles, Mathx.RemapValClamped(lerpedInput.x, -1f, 1f, 40f, -40f)));
			rigidBody.MoveRotation(Quaternion.Lerp(rigidBody.rotation, val2, Time.fixedDeltaTime * 30f));
			rigidBody.AddTorque(((Component)this).transform.TransformDirection(Vector3.up * (TurnForce * num * 0.2f * lerpedInput.x)), TurnForceMode);
		}
		if (lerpedInput.y > 0f)
		{
			rotation = rigidBody.rotation;
			Quaternion val3 = Quaternion.Euler(Vector3Ex.WithX(((Quaternion)(ref rotation)).eulerAngles, Mathx.RemapValClamped(lerpedInput.y, -1f, 1f, -50f, 60f)));
			rigidBody.MoveRotation(Quaternion.Lerp(rigidBody.rotation, val3, Time.fixedDeltaTime * 60f));
		}
		rotation = rigidBody.rotation;
		Quaternion val4 = Quaternion.Euler(Vector3Ex.WithZ(Vector3Ex.WithX(((Quaternion)(ref rotation)).eulerAngles, 0f), 0f));
		rigidBody.rotation = Quaternion.Lerp(rigidBody.rotation, val4, Time.fixedDeltaTime * UprightLerpForce);
		float num5 = DamageHorizontalVelocityCurve.Evaluate(num);
		Vector3 velocity = rigidBody.velocity;
		velocity.x = Mathf.Clamp(velocity.x, 0f - num5, num5);
		velocity.z = Mathf.Clamp(velocity.z, 0f - num5, num5);
		rigidBody.velocity = velocity;
	}

	public override void PlayerDismounted(BasePlayer player, BaseMountable seat)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		base.PlayerDismounted(player, seat);
		if (collisionDeath)
		{
			if (TimeSince.op_Implicit(mountTime) < HurtDeployTime)
			{
				float num = 1f - Mathf.Clamp01(TimeSince.op_Implicit(mountTime) / HurtDeployTime);
				player.Hurt(HurtAmount * num, DamageType.Fall);
			}
			else
			{
				float magnitude = ((Vector3)(ref collisionImpulse)).magnitude;
				if (magnitude > 50f)
				{
					float amount = Mathx.RemapValClamped(magnitude, 50f, 400f, 5f, 50f);
					player.Hurt(amount, DamageType.Fall);
				}
			}
		}
		if (BypassRepack)
		{
			Item item = ItemManager.Create(PackedParachute, 1, skinID);
			item.RepairCondition(item.maxCondition);
			player.inventory.containerWear.GiveItem(item);
		}
		Analytics.Azure.OnParachuteUsed(player, distanceTravelled, startHeight, TimeSince.op_Implicit(mountTime));
		if (collisionDeath && LandingAnimations)
		{
			Effect.server.Run(ParachuteLandScreenBounce.resourcePath, player, 0u, Vector3.zero, Vector3.zero);
			if (collisionLocalPos.y < 0.15f)
			{
				player.Server_StartGesture(GestureCollection.HeavyLandingId);
				player.PlayHeavyLandingAnimation = false;
			}
		}
		ProcessDeath();
		collisionDeath = false;
	}

	private void ProcessDeath()
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		float num = base.healthFraction;
		num -= ConditionLossPerUse;
		bool num2 = num > 0f;
		if (num2 && !BypassRepack)
		{
			ParachuteUnpacked parachuteUnpacked = GameManager.server.CreateEntity(DetachedParachute.resourcePath, DetachedSpawnPoint.position, DetachedSpawnPoint.rotation) as ParachuteUnpacked;
			if ((Object)(object)parachuteUnpacked != (Object)null)
			{
				parachuteUnpacked.skinID = skinID;
				parachuteUnpacked.Spawn();
				parachuteUnpacked.Hurt(parachuteUnpacked.MaxHealth() * (1f - num), DamageType.Generic, null, useProtection: false);
				Rigidbody val = default(Rigidbody);
				if (((Component)parachuteUnpacked).TryGetComponent<Rigidbody>(ref val))
				{
					val.velocity = rigidBody.velocity;
				}
			}
		}
		DestroyMode mode = DestroyMode.None;
		if (!num2)
		{
			mode = DestroyMode.Gib;
		}
		Kill(mode);
	}

	public override void OnCollision(Collision collision, BaseEntity hitEntity)
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)hitEntity == (Object)null)
		{
			hitEntity = collision.collider.ToBaseEntity();
		}
		if (!((Object)(object)hitEntity == (Object)(object)this) && (!((Object)(object)hitEntity != (Object)null) || hitEntity.isServer == base.isServer) && base.isServer && !(hitEntity is TimedExplosive) && !collisionDeath)
		{
			collisionImpulse = collision.impulse;
			Transform transform = ((Component)this).transform;
			ContactPoint contact = collision.GetContact(0);
			collisionLocalPos = transform.InverseTransformPoint(((ContactPoint)(ref contact)).point);
			contact = collision.GetContact(0);
			collisionWorldNormal = ((ContactPoint)(ref contact)).normal;
			collisionDeath = true;
			((FacepunchBehaviour)this).Invoke((Action)DelayedDismount, 0f);
		}
	}

	private void DelayedDismount()
	{
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		if (collisionDeath && distanceTravelled > 0f && (!((Object)(object)mountPoints[0].mountable != (Object)null) || !GetDismountPosition(mountPoints[0].mountable.GetMounted(), out var _)))
		{
			Transform transform = ((Component)this).transform;
			transform.position += collisionWorldNormal * 0.35f;
		}
		DismountAllPlayers();
	}

	public override float MaxVelocity()
	{
		return 13.5f;
	}

	public override bool AllowPlayerInstigatedDismount(BasePlayer player)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if (TimeSince.op_Implicit(mountTime) < 1.5f)
		{
			return false;
		}
		return base.AllowPlayerInstigatedDismount(player);
	}

	public bool IsValidSAMTarget(bool staticRespawn)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if (TimeSince.op_Implicit(mountTime) > 1f)
		{
			return !InSafeZone();
		}
		return false;
	}

	private Vector2 ProcessInputVector(InputState inputState, BasePlayer player)
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)player.GetHeldEntity() != (Object)null)
		{
			return Vector2.zero;
		}
		bool leftDown = inputState.IsDown(BUTTON.LEFT);
		bool rightDown = inputState.IsDown(BUTTON.RIGHT);
		bool forwardDown = inputState.IsDown(BUTTON.FORWARD);
		bool backDown = inputState.IsDown(BUTTON.BACKWARD);
		return ProcessInputVector(leftDown, rightDown, forwardDown, backDown);
	}

	private Vector2 ProcessInputVectorFromFlags(BasePlayer player)
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)player.GetHeldEntity() != (Object)null)
		{
			return Vector2.zero;
		}
		bool leftDown = HasFlag(Flags.Reserved3);
		bool rightDown = HasFlag(Flags.Reserved4);
		bool forwardDown = HasFlag(Flags.Reserved1);
		bool backDown = HasFlag(Flags.Reserved2);
		return ProcessInputVector(leftDown, rightDown, forwardDown, backDown);
	}

	private static Vector2 ProcessInputVector(bool leftDown, bool rightDown, bool forwardDown, bool backDown)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		Vector2 zero = Vector2.zero;
		if (leftDown && rightDown)
		{
			leftDown = (rightDown = false);
		}
		if (forwardDown && backDown)
		{
			forwardDown = (backDown = false);
		}
		if (forwardDown)
		{
			zero.y = 1f;
		}
		else if (backDown)
		{
			zero.y = -1f;
		}
		if (rightDown)
		{
			zero.x = 1f;
		}
		else if (leftDown)
		{
			zero.x = -1f;
		}
		return zero;
	}
}
