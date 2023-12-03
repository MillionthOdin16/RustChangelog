using System;
using Network;
using UnityEngine;

public class WaterInflatable : BaseMountable, IPoolVehicle, INotifyTrigger
{
	private enum PaddleDirection
	{
		Forward,
		Left,
		Right,
		Back
	}

	public Rigidbody rigidBody;

	public Transform centerOfMass;

	public float forwardPushForce = 5f;

	public float rearPushForce = 5f;

	public float rotationForce = 5f;

	public float maxSpeed = 3f;

	public float maxPaddleFrequency = 0.5f;

	public SoundDefinition paddleSfx = null;

	public SoundDefinition smallPlayerMovementSound = null;

	public SoundDefinition largePlayerMovementSound = null;

	public BlendedSoundLoops waterLoops;

	public float waterSoundSpeedDivisor = 1f;

	public float additiveDownhillVelocity = 0f;

	public GameObjectRef handSplashForwardEffect;

	public GameObjectRef handSplashBackEffect;

	public GameObjectRef footSplashEffect;

	public float animationLerpSpeed = 1f;

	public Transform smoothedEyePosition = null;

	public float smoothedEyeSpeed = 1f;

	public Buoyancy buoyancy = null;

	public bool driftTowardsIsland = false;

	public GameObjectRef mountEffect;

	[Range(0f, 1f)]
	public float handSplashOffset = 1f;

	public float velocitySplashMultiplier = 4f;

	public Vector3 modifyEyeOffset = Vector3.zero;

	[Range(0f, 1f)]
	public float inheritVelocityMultiplier = 0f;

	private TimeSince lastPaddle;

	public ParticleSystem[] movingParticleSystems;

	public float movingParticlesThreshold = 0.0005f;

	public Transform headSpaceCheckPosition = null;

	public float headSpaceCheckRadius = 0.4f;

	private TimeSince landFacingCheck;

	private bool isFacingLand = false;

	private float landPushAcceleration = 0f;

	private TimeSince inPoolCheck;

	private bool isInPool = false;

	private Vector3 lastPos = Vector3.zero;

	private Vector3 lastClipCheckPosition;

	private bool forceClippingCheck = false;

	private bool prevSleeping;

	public override bool IsSummerDlcVehicle => true;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("WaterInflatable.OnRpcMessage", 0);
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
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		base.ServerInit();
		rigidBody.centerOfMass = centerOfMass.localPosition;
		prevSleeping = false;
	}

	public override void OnDeployed(BaseEntity parent, BasePlayer deployedBy, Item fromItem)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		base.OnDeployed(parent, deployedBy, fromItem);
		if ((Object)(object)deployedBy != (Object)null)
		{
			Vector3 estimatedVelocity = deployedBy.estimatedVelocity;
			float num = Vector3.Dot(((Component)this).transform.forward, ((Vector3)(ref estimatedVelocity)).normalized);
			Vector3 val = Vector3.Lerp(Vector3.zero, estimatedVelocity, Mathf.Clamp(num, 0f, 1f));
			val *= inheritVelocityMultiplier;
			rigidBody.AddForce(val, (ForceMode)2);
		}
	}

	public override void VehicleFixedUpdate()
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		base.VehicleFixedUpdate();
		bool flag = rigidBody.IsSleeping();
		if (prevSleeping && !flag && (Object)(object)buoyancy != (Object)null)
		{
			buoyancy.Wake();
		}
		prevSleeping = flag;
		Vector3 velocity = rigidBody.velocity;
		if (((Vector3)(ref velocity)).magnitude > maxSpeed)
		{
			rigidBody.velocity = Vector3.ClampMagnitude(rigidBody.velocity, maxSpeed);
		}
		if (!AnyMounted() || !((Object)(object)headSpaceCheckPosition != (Object)null))
		{
			return;
		}
		Vector3 position = ((Component)this).transform.position;
		if (!forceClippingCheck && !(Vector3.Distance(position, lastClipCheckPosition) > headSpaceCheckRadius * 0.5f))
		{
			return;
		}
		forceClippingCheck = false;
		if (GamePhysics.CheckSphere(headSpaceCheckPosition.position, headSpaceCheckRadius, 1218511105, (QueryTriggerInteraction)0))
		{
			if (!GetDismountPosition(GetMounted(), out var _))
			{
				((Component)this).transform.position = lastClipCheckPosition;
			}
			DismountAllPlayers();
		}
		lastClipCheckPosition = position;
	}

	public override void OnPlayerMounted()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		base.OnPlayerMounted();
		lastPos = ((Component)this).transform.position;
		forceClippingCheck = true;
	}

	public override void PlayerServerInput(InputState inputState, BasePlayer player)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_0334: Unknown result type (might be due to invalid IL or missing references)
		//IL_0339: Unknown result type (might be due to invalid IL or missing references)
		//IL_0344: Unknown result type (might be due to invalid IL or missing references)
		//IL_0349: Unknown result type (might be due to invalid IL or missing references)
		//IL_034b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0365: Unknown result type (might be due to invalid IL or missing references)
		//IL_036a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0427: Unknown result type (might be due to invalid IL or missing references)
		//IL_0438: Unknown result type (might be due to invalid IL or missing references)
		//IL_038a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0395: Unknown result type (might be due to invalid IL or missing references)
		//IL_0397: Unknown result type (might be due to invalid IL or missing references)
		//IL_039c: Unknown result type (might be due to invalid IL or missing references)
		//IL_045b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0460: Unknown result type (might be due to invalid IL or missing references)
		base.PlayerServerInput(inputState, player);
		float num = Vector3.Dot(((Component)this).transform.up, Vector3.up);
		if (num < 0.1f)
		{
			DismountAllPlayers();
		}
		else
		{
			if (TimeSince.op_Implicit(lastPaddle) < maxPaddleFrequency || ((Object)(object)buoyancy != (Object)null && IsOutOfWaterServer()))
			{
				return;
			}
			if ((Object)(object)player.GetHeldEntity() == (Object)null)
			{
				if (inputState.IsDown(BUTTON.FORWARD))
				{
					Vector3 velocity = rigidBody.velocity;
					if (((Vector3)(ref velocity)).magnitude < maxSpeed)
					{
						rigidBody.AddForce(((Component)this).transform.forward * forwardPushForce, (ForceMode)1);
					}
					rigidBody.angularVelocity = Vector3.Lerp(rigidBody.angularVelocity, ((Component)this).transform.forward, 0.5f);
					lastPaddle = TimeSince.op_Implicit(0f);
					ClientRPC(null, "OnPaddled", 0);
				}
				if (inputState.IsDown(BUTTON.BACKWARD))
				{
					rigidBody.AddForce(-((Component)this).transform.forward * rearPushForce, (ForceMode)1);
					rigidBody.angularVelocity = Vector3.Lerp(rigidBody.angularVelocity, -((Component)this).transform.forward, 0.5f);
					lastPaddle = TimeSince.op_Implicit(0f);
					ClientRPC(null, "OnPaddled", 3);
				}
				if (inputState.IsDown(BUTTON.LEFT))
				{
					PaddleTurn(PaddleDirection.Left);
				}
				if (inputState.IsDown(BUTTON.RIGHT))
				{
					PaddleTurn(PaddleDirection.Right);
				}
			}
			if (TimeSince.op_Implicit(inPoolCheck) > 2f)
			{
				isInPool = IsInWaterVolume(((Component)this).transform.position);
				inPoolCheck = TimeSince.op_Implicit(0f);
			}
			if (additiveDownhillVelocity > 0f && !isInPool)
			{
				Vector3 val = ((Component)this).transform.TransformPoint(Vector3.forward);
				Vector3 position = ((Component)this).transform.position;
				if (val.y < position.y)
				{
					float num2 = additiveDownhillVelocity * (position.y - val.y);
					rigidBody.AddForce(num2 * Time.fixedDeltaTime * ((Component)this).transform.forward, (ForceMode)5);
				}
				Vector3 velocity2 = rigidBody.velocity;
				rigidBody.velocity = Vector3.Lerp(velocity2, ((Component)this).transform.forward * ((Vector3)(ref velocity2)).magnitude, 0.4f);
			}
			if (driftTowardsIsland && TimeSince.op_Implicit(landFacingCheck) > 2f && !isInPool)
			{
				isFacingLand = false;
				landFacingCheck = TimeSince.op_Implicit(0f);
				Vector3 position2 = ((Component)this).transform.position;
				if (!WaterResource.IsFreshWater(position2))
				{
					int num3 = 5;
					Vector3 forward = ((Component)this).transform.forward;
					forward.y = 0f;
					for (int i = 1; i <= num3; i++)
					{
						int mask = 128;
						if (!TerrainMeta.TopologyMap.GetTopology(position2 + (float)i * 15f * forward, mask))
						{
							isFacingLand = true;
							break;
						}
					}
				}
			}
			if (driftTowardsIsland && isFacingLand && !isInPool)
			{
				landPushAcceleration = Mathf.Clamp(landPushAcceleration + Time.deltaTime, 0f, 3f);
				rigidBody.AddForce(((Component)this).transform.forward * (Time.deltaTime * landPushAcceleration), (ForceMode)2);
			}
			else
			{
				landPushAcceleration = 0f;
			}
			lastPos = ((Component)this).transform.position;
		}
	}

	private void PaddleTurn(PaddleDirection direction)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		if (direction != 0 && direction != PaddleDirection.Back)
		{
			rigidBody.AddRelativeTorque(rotationForce * ((direction == PaddleDirection.Left) ? (-Vector3.up) : Vector3.up), (ForceMode)1);
			lastPaddle = TimeSince.op_Implicit(0f);
			ClientRPC(null, "OnPaddled", (int)direction);
		}
	}

	public override float WaterFactorForPlayer(BasePlayer player)
	{
		return 0f;
	}

	public override void OnCollision(Collision collision, BaseEntity hitEntity)
	{
		if (hitEntity is BaseVehicle baseVehicle && (baseVehicle.HasDriver() || baseVehicle.IsMoving() || baseVehicle.HasFlag(Flags.On)))
		{
			Kill(DestroyMode.Gib);
		}
	}

	private bool IsOutOfWaterServer()
	{
		return buoyancy.timeOutOfWater > 0.2f;
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
		if ((Object)(object)buoyancy != (Object)null)
		{
			buoyancy.Wake();
		}
	}

	public void OnObjects(TriggerNotify trigger)
	{
		if (base.isClient)
		{
			return;
		}
		foreach (BaseEntity entityContent in trigger.entityContents)
		{
			if (entityContent is BaseVehicle baseVehicle && (baseVehicle.HasDriver() || baseVehicle.IsMoving() || baseVehicle.HasFlag(Flags.On)))
			{
				Kill(DestroyMode.Gib);
				break;
			}
		}
	}

	public void OnEmpty()
	{
	}
}
