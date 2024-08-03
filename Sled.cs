using System;
using UnityEngine;

public class Sled : BaseVehicle, INotifyTrigger
{
	private const Flags BrakeOn = Flags.Reserved1;

	private const Flags OnSnow = Flags.Reserved2;

	private const Flags IsGrounded = Flags.Reserved3;

	private const Flags OnSand = Flags.Reserved4;

	public PhysicMaterial BrakeMaterial = null;

	public PhysicMaterial SnowMaterial = null;

	public PhysicMaterial NonSnowMaterial = null;

	public Transform CentreOfMassTransform;

	public Collider[] PhysicsMaterialTargets;

	public float InitialForceCutoff = 3f;

	public float InitialForceIncreaseRate = 0.05f;

	public float TurnForce = 1f;

	public float DirectionMatchForce = 1f;

	public float VerticalAdjustmentForce = 1f;

	public float VerticalAdjustmentAngleThreshold = 15f;

	public float NudgeCooldown = 3f;

	public float NudgeForce = 2f;

	public float MaxNudgeVelocity = 2f;

	public const float DecayFrequency = 60f;

	public float DecayAmount = 10f;

	public ParticleSystemContainer TrailEffects;

	public SoundDefinition enterSnowSoundDef;

	public SoundDefinition snowSlideLoopSoundDef;

	public SoundDefinition dirtSlideLoopSoundDef;

	public AnimationCurve movementLoopGainCurve;

	public AnimationCurve movementLoopPitchCurve;

	private VehicleTerrainHandler terrainHandler = null;

	private PhysicMaterial cachedMaterial = null;

	private float initialForceScale = 0f;

	private TimeSince leftIce;

	private TimeSince lastNudge;

	public override bool BlocksDoors => false;

	public override void ServerInit()
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		base.ServerInit();
		terrainHandler = new VehicleTerrainHandler(this);
		terrainHandler.RayLength = 0.6f;
		rigidBody.centerOfMass = CentreOfMassTransform.localPosition;
		((FacepunchBehaviour)this).InvokeRandomized((Action)DecayOverTime, Random.Range(30f, 60f), 60f, 6f);
	}

	public override void OnDeployed(BaseEntity parent, BasePlayer deployedBy, Item fromItem)
	{
		base.OnDeployed(parent, deployedBy, fromItem);
		SetFlag(Flags.Reserved1, b: true);
		UpdateGroundedFlag();
		UpdatePhysicsMaterial();
	}

	public override void VehicleFixedUpdate()
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		base.VehicleFixedUpdate();
		if (!AnyMounted())
		{
			return;
		}
		terrainHandler.FixedUpdate();
		if (!terrainHandler.IsGrounded)
		{
			Vector3 up = ((Component)this).transform.up;
			Quaternion val = Quaternion.FromToRotation(up, Vector3.up) * rigidBody.rotation;
			float num = Quaternion.Angle(rigidBody.rotation, val);
			if (num > VerticalAdjustmentAngleThreshold)
			{
				rigidBody.MoveRotation(Quaternion.Slerp(rigidBody.rotation, val, Time.fixedDeltaTime * VerticalAdjustmentForce));
			}
		}
	}

	private void UpdatePhysicsMaterial()
	{
		cachedMaterial = GetPhysicMaterial();
		Collider[] physicsMaterialTargets = PhysicsMaterialTargets;
		foreach (Collider val in physicsMaterialTargets)
		{
			val.sharedMaterial = cachedMaterial;
		}
		if (!AnyMounted() && rigidBody.IsSleeping())
		{
			((FacepunchBehaviour)this).CancelInvoke((Action)UpdatePhysicsMaterial);
		}
		SetFlag(Flags.Reserved2, terrainHandler.IsOnSnowOrIce);
		SetFlag(Flags.Reserved4, terrainHandler.OnSurface == VehicleTerrainHandler.Surface.Sand);
	}

	private void UpdateGroundedFlag()
	{
		if (!AnyMounted() && rigidBody.IsSleeping())
		{
			((FacepunchBehaviour)this).CancelInvoke((Action)UpdateGroundedFlag);
		}
		SetFlag(Flags.Reserved3, terrainHandler.IsGrounded);
	}

	private PhysicMaterial GetPhysicMaterial()
	{
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		if (HasFlag(Flags.Reserved1) || !AnyMounted())
		{
			return BrakeMaterial;
		}
		bool flag = terrainHandler.IsOnSnowOrIce || terrainHandler.OnSurface == VehicleTerrainHandler.Surface.Sand;
		if (flag)
		{
			leftIce = TimeSince.op_Implicit(0f);
		}
		else if (TimeSince.op_Implicit(leftIce) < 2f)
		{
			flag = true;
		}
		return flag ? SnowMaterial : NonSnowMaterial;
	}

	public override void PlayerMounted(BasePlayer player, BaseMountable seat)
	{
		base.PlayerMounted(player, seat);
		if (HasFlag(Flags.Reserved1))
		{
			initialForceScale = 0f;
			((FacepunchBehaviour)this).InvokeRepeating((Action)ApplyInitialForce, 0f, 0.1f);
			SetFlag(Flags.Reserved1, b: false);
		}
		if (!((FacepunchBehaviour)this).IsInvoking((Action)UpdatePhysicsMaterial))
		{
			((FacepunchBehaviour)this).InvokeRepeating((Action)UpdatePhysicsMaterial, 0f, 0.5f);
		}
		if (!((FacepunchBehaviour)this).IsInvoking((Action)UpdateGroundedFlag))
		{
			((FacepunchBehaviour)this).InvokeRepeating((Action)UpdateGroundedFlag, 0f, 0.1f);
		}
		if (rigidBody.IsSleeping())
		{
			rigidBody.WakeUp();
		}
	}

	private void ApplyInitialForce()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		Vector3 forward = ((Component)this).transform.forward;
		Vector3 val = ((Vector3.Dot(forward, -Vector3.up) > Vector3.Dot(-forward, -Vector3.up)) ? forward : (-forward));
		rigidBody.AddForce(val * initialForceScale * (terrainHandler.IsOnSnowOrIce ? 1f : 0.25f), (ForceMode)5);
		initialForceScale += InitialForceIncreaseRate;
		if (initialForceScale >= InitialForceCutoff)
		{
			Vector3 velocity = rigidBody.velocity;
			if (((Vector3)(ref velocity)).magnitude > 1f || !terrainHandler.IsOnSnowOrIce)
			{
				((FacepunchBehaviour)this).CancelInvoke((Action)ApplyInitialForce);
			}
		}
	}

	public override void PlayerServerInput(InputState inputState, BasePlayer player)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		base.PlayerServerInput(inputState, player);
		float num = Vector3.Dot(((Component)this).transform.up, Vector3.up);
		if (num < 0.1f || WaterFactor() > 0.25f)
		{
			DismountAllPlayers();
			return;
		}
		float num2 = (inputState.IsDown(BUTTON.LEFT) ? (-1f) : 0f);
		num2 += (inputState.IsDown(BUTTON.RIGHT) ? 1f : 0f);
		Vector3 velocity;
		if (inputState.IsDown(BUTTON.FORWARD) && TimeSince.op_Implicit(lastNudge) > NudgeCooldown)
		{
			velocity = rigidBody.velocity;
			if (((Vector3)(ref velocity)).magnitude < MaxNudgeVelocity)
			{
				rigidBody.WakeUp();
				rigidBody.AddForce(((Component)this).transform.forward * NudgeForce, (ForceMode)1);
				rigidBody.AddForce(((Component)this).transform.up * NudgeForce * 0.5f, (ForceMode)1);
				lastNudge = TimeSince.op_Implicit(0f);
			}
		}
		num2 *= TurnForce;
		Vector3 velocity2 = rigidBody.velocity;
		if (num2 != 0f)
		{
			((Component)this).transform.Rotate(Vector3.up * num2 * Time.deltaTime * ((Vector3)(ref velocity2)).magnitude, (Space)1);
		}
		if (terrainHandler.IsGrounded)
		{
			velocity = rigidBody.velocity;
			float num3 = Vector3.Dot(((Vector3)(ref velocity)).normalized, ((Component)this).transform.forward);
			if (num3 >= 0.5f)
			{
				rigidBody.velocity = Vector3.Lerp(rigidBody.velocity, ((Component)this).transform.forward * ((Vector3)(ref velocity2)).magnitude, Time.deltaTime * DirectionMatchForce);
			}
		}
	}

	private void DecayOverTime()
	{
		if (!AnyMounted())
		{
			Hurt(DecayAmount);
		}
	}

	public override bool CanPickup(BasePlayer player)
	{
		return base.CanPickup(player) && !player.isMounted;
	}

	public void OnObjects(TriggerNotify trigger)
	{
		foreach (BaseEntity entityContent in trigger.entityContents)
		{
			if (!(entityContent is Sled))
			{
				if (entityContent is BaseVehicleModule baseVehicleModule && (Object)(object)baseVehicleModule.Vehicle != (Object)null && (baseVehicleModule.Vehicle.IsOn() || !baseVehicleModule.Vehicle.IsStationary()))
				{
					Kill(DestroyMode.Gib);
					break;
				}
				if (entityContent is BaseVehicle baseVehicle && baseVehicle.HasDriver() && (baseVehicle.IsMoving() || baseVehicle.HasFlag(Flags.On)))
				{
					Kill(DestroyMode.Gib);
					break;
				}
			}
		}
	}

	public void OnEmpty()
	{
	}
}
