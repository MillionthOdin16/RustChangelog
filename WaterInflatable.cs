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

	public Transform centerOfMass;

	public float forwardPushForce = 5f;

	public float rearPushForce = 5f;

	public float rotationForce = 5f;

	public float maxSpeed = 3f;

	public float maxPaddleFrequency = 0.5f;

	public SoundDefinition paddleSfx;

	public SoundDefinition smallPlayerMovementSound;

	public SoundDefinition largePlayerMovementSound;

	public BlendedSoundLoops waterLoops;

	public float waterSoundSpeedDivisor = 1f;

	public float additiveDownhillVelocity;

	public GameObjectRef handSplashForwardEffect;

	public GameObjectRef handSplashBackEffect;

	public GameObjectRef footSplashEffect;

	public float animationLerpSpeed = 1f;

	public Transform smoothedEyePosition;

	public float smoothedEyeSpeed = 1f;

	public Buoyancy buoyancy;

	public bool driftTowardsIsland;

	public GameObjectRef mountEffect;

	[Range(0f, 1f)]
	public float handSplashOffset = 1f;

	public float velocitySplashMultiplier = 4f;

	public Vector3 modifyEyeOffset = Vector3.zero;

	[Range(0f, 1f)]
	public float inheritVelocityMultiplier;

	private TimeSince lastPaddle;

	public ParticleSystem[] movingParticleSystems;

	public float movingParticlesThreshold = 0.0005f;

	public Transform headSpaceCheckPosition;

	public float headSpaceCheckRadius = 0.4f;

	private TimeSince landFacingCheck;

	private bool isFacingLand;

	private float landPushAcceleration;

	private TimeSince inPoolCheck;

	private bool isInPool;

	private Vector3 lastPos = Vector3.zero;

	private Vector3 lastClipCheckPosition;

	private bool forceClippingCheck;

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
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		base.ServerInit();
		rigidBody.centerOfMass = centerOfMass.localPosition;
		prevSleeping = false;
	}

	public override void OnDeployed(BaseEntity parent, BasePlayer deployedBy, Item fromItem)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
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
		if (GamePhysics.CheckSphere(headSpaceCheckPosition.position, headSpaceCheckRadius, 1218511105, (QueryTriggerInteraction)1))
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
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		base.OnPlayerMounted();
		lastPos = ((Component)this).transform.position;
		forceClippingCheck = true;
	}

	public override void PlayerServerInput(InputState inputState, BasePlayer player)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0387: Unknown result type (might be due to invalid IL or missing references)
		//IL_0398: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_030d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0318: Unknown result type (might be due to invalid IL or missing references)
		//IL_031a: Unknown result type (might be due to invalid IL or missing references)
		//IL_031f: Unknown result type (might be due to invalid IL or missing references)
		base.PlayerServerInput(inputState, player);
		if (Vector3.Dot(((Component)this).transform.up, Vector3.up) < 0.1f)
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
					float num = additiveDownhillVelocity * (position.y - val.y);
					rigidBody.AddForce(num * Time.fixedDeltaTime * ((Component)this).transform.forward, (ForceMode)5);
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
					int num2 = 5;
					Vector3 forward = ((Component)this).transform.forward;
					forward.y = 0f;
					for (int i = 1; i <= num2; i++)
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
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
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
