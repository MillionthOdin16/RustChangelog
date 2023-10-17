using System;
using System.Collections.Generic;
using Rust;
using UnityEngine;

public class BaseHelicopterVehicle : BaseVehicle
{
	public class HelicopterInputState
	{
		public float throttle;

		public float roll;

		public float yaw;

		public float pitch;

		public bool groundControl;

		public void Reset()
		{
			throttle = 0f;
			roll = 0f;
			yaw = 0f;
			pitch = 0f;
			groundControl = false;
		}
	}

	[Header("Helicopter")]
	public float engineThrustMax;

	public Vector3 torqueScale;

	public Transform com;

	public GameObject[] killTriggers;

	[Header("Effects")]
	public Transform[] GroundPoints;

	public Transform[] GroundEffects;

	public GameObjectRef serverGibs;

	public GameObjectRef explosionEffect;

	public GameObjectRef fireBall;

	public GameObjectRef impactEffectSmall;

	public GameObjectRef impactEffectLarge;

	[Header("Sounds")]
	public SoundDefinition flightEngineSoundDef;

	public SoundDefinition flightThwopsSoundDef;

	public float rotorGainModSmoothing = 0.25f;

	public float engineGainMin = 0.5f;

	public float engineGainMax = 1f;

	public float thwopGainMin = 0.5f;

	public float thwopGainMax = 1f;

	public float currentThrottle = 0f;

	public float avgThrust = 0f;

	public float liftDotMax = 0.75f;

	public float altForceDotMin = 0.85f;

	public float liftFraction = 0.25f;

	public float thrustLerpSpeed = 1f;

	private float avgTerrainHeight = 0f;

	public const Flags Flag_InternalLights = Flags.Reserved6;

	protected HelicopterInputState currentInputState = new HelicopterInputState();

	protected float lastPlayerInputTime = 0f;

	protected float hoverForceScale = 0.99f;

	protected Vector3 damageTorque;

	private float nextDamageTime = 0f;

	private float nextEffectTime = 0f;

	private float pendingImpactDamage = 0f;

	public virtual float GetServiceCeiling()
	{
		return 1000f;
	}

	public override float MaxVelocity()
	{
		return 50f;
	}

	public override void ServerInit()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		base.ServerInit();
		rigidBody.centerOfMass = com.localPosition;
	}

	public float MouseToBinary(float amount)
	{
		return Mathf.Clamp(amount, -1f, 1f);
	}

	public virtual void PilotInput(InputState inputState, BasePlayer player)
	{
		currentInputState.Reset();
		currentInputState.throttle = (inputState.IsDown(BUTTON.FORWARD) ? 1f : 0f);
		currentInputState.throttle -= ((inputState.IsDown(BUTTON.BACKWARD) || inputState.IsDown(BUTTON.DUCK)) ? 1f : 0f);
		currentInputState.pitch = inputState.current.mouseDelta.y;
		currentInputState.roll = 0f - inputState.current.mouseDelta.x;
		currentInputState.yaw = (inputState.IsDown(BUTTON.RIGHT) ? 1f : 0f);
		currentInputState.yaw -= (inputState.IsDown(BUTTON.LEFT) ? 1f : 0f);
		currentInputState.pitch = MouseToBinary(currentInputState.pitch);
		currentInputState.roll = MouseToBinary(currentInputState.roll);
		lastPlayerInputTime = Time.time;
	}

	public override void PlayerServerInput(InputState inputState, BasePlayer player)
	{
		if (IsDriver(player))
		{
			PilotInput(inputState, player);
		}
	}

	public virtual void SetDefaultInputState()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		currentInputState.Reset();
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

	public virtual bool IsEnginePowered()
	{
		return true;
	}

	public override void VehicleFixedUpdate()
	{
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		base.VehicleFixedUpdate();
		if (Time.time > lastPlayerInputTime + 0.5f)
		{
			SetDefaultInputState();
		}
		EnableGlobalBroadcast(IsEngineOn());
		MovementUpdate();
		SetFlag(Flags.Reserved6, TOD_Sky.Instance.IsNight);
		GameObject[] array = killTriggers;
		foreach (GameObject val in array)
		{
			bool active = rigidBody.velocity.y < 0f;
			val.SetActive(active);
		}
	}

	public override void LightToggle(BasePlayer player)
	{
		if (IsDriver(player))
		{
			SetFlag(Flags.Reserved5, !HasFlag(Flags.Reserved5));
		}
	}

	public virtual bool ShouldApplyHoverForce()
	{
		return true;
	}

	public virtual bool IsEngineOn()
	{
		return true;
	}

	public void ClearDamageTorque()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		SetDamageTorque(Vector3.zero);
	}

	public void SetDamageTorque(Vector3 newTorque)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		damageTorque = newTorque;
	}

	public void AddDamageTorque(Vector3 torqueToAdd)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		damageTorque += torqueToAdd;
	}

	public virtual void MovementUpdate()
	{
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		if (IsEngineOn())
		{
			HelicopterInputState helicopterInputState = currentInputState;
			currentThrottle = Mathf.Lerp(currentThrottle, helicopterInputState.throttle, 2f * Time.fixedDeltaTime);
			currentThrottle = Mathf.Clamp(currentThrottle, -0.8f, 1f);
			if (helicopterInputState.pitch != 0f || helicopterInputState.roll != 0f || helicopterInputState.yaw != 0f)
			{
				rigidBody.AddRelativeTorque(new Vector3(helicopterInputState.pitch * torqueScale.x, helicopterInputState.yaw * torqueScale.y, helicopterInputState.roll * torqueScale.z), (ForceMode)0);
			}
			if (damageTorque != Vector3.zero)
			{
				rigidBody.AddRelativeTorque(new Vector3(damageTorque.x, damageTorque.y, damageTorque.z), (ForceMode)0);
			}
			avgThrust = Mathf.Lerp(avgThrust, engineThrustMax * currentThrottle, Time.fixedDeltaTime * thrustLerpSpeed);
			float num = Mathf.Clamp01(Vector3.Dot(((Component)this).transform.up, Vector3.up));
			float num2 = Mathf.InverseLerp(liftDotMax, 1f, num);
			float serviceCeiling = GetServiceCeiling();
			avgTerrainHeight = Mathf.Lerp(avgTerrainHeight, TerrainMeta.HeightMap.GetHeight(((Component)this).transform.position), Time.deltaTime);
			float num3 = 1f - Mathf.InverseLerp(avgTerrainHeight + serviceCeiling - 20f, avgTerrainHeight + serviceCeiling, ((Component)this).transform.position.y);
			num2 *= num3;
			float num4 = 1f - Mathf.InverseLerp(altForceDotMin, 1f, num);
			Vector3 val = Vector3.up * engineThrustMax * liftFraction * currentThrottle * num2;
			Vector3 val2 = ((Component)this).transform.up - Vector3.up;
			Vector3 val3 = ((Vector3)(ref val2)).normalized * engineThrustMax * currentThrottle * num4;
			if (ShouldApplyHoverForce())
			{
				float num5 = rigidBody.mass * (0f - Physics.gravity.y);
				rigidBody.AddForce(((Component)this).transform.up * num5 * num2 * hoverForceScale, (ForceMode)0);
			}
			rigidBody.AddForce(val, (ForceMode)0);
			rigidBody.AddForce(val3, (ForceMode)0);
		}
	}

	public void DelayedImpactDamage()
	{
		float num = explosionForceMultiplier;
		explosionForceMultiplier = 0f;
		Hurt(pendingImpactDamage * MaxHealth(), DamageType.Explosion, this, useProtection: false);
		pendingImpactDamage = 0f;
		explosionForceMultiplier = num;
	}

	public virtual bool CollisionDamageEnabled()
	{
		return true;
	}

	public void ProcessCollision(Collision collision)
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		if (base.isClient || !CollisionDamageEnabled() || Time.time < nextDamageTime)
		{
			return;
		}
		Vector3 relativeVelocity = collision.relativeVelocity;
		float magnitude = ((Vector3)(ref relativeVelocity)).magnitude;
		if (Object.op_Implicit((Object)(object)collision.gameObject) && ((1 << ((Component)collision.collider).gameObject.layer) & 0x48A18101) <= 0)
		{
			return;
		}
		float num = Mathf.InverseLerp(5f, 30f, magnitude);
		if (!(num > 0f))
		{
			return;
		}
		pendingImpactDamage += Mathf.Max(num, 0.15f);
		if (Vector3.Dot(((Component)this).transform.up, Vector3.up) < 0.5f)
		{
			pendingImpactDamage *= 5f;
		}
		if (Time.time > nextEffectTime)
		{
			nextEffectTime = Time.time + 0.25f;
			if (impactEffectSmall.isValid)
			{
				ContactPoint contact = collision.GetContact(0);
				Vector3 point = ((ContactPoint)(ref contact)).point;
				point += (((Component)this).transform.position - point) * 0.25f;
				Effect.server.Run(impactEffectSmall.resourcePath, point, ((Component)this).transform.up);
			}
		}
		Rigidbody obj = rigidBody;
		ContactPoint contact2 = collision.GetContact(0);
		Vector3 val = ((ContactPoint)(ref contact2)).normal * (1f + 3f * num);
		contact2 = collision.GetContact(0);
		obj.AddForceAtPosition(val, ((ContactPoint)(ref contact2)).point, (ForceMode)2);
		nextDamageTime = Time.time + 0.333f;
		((FacepunchBehaviour)this).Invoke((Action)DelayedImpactDamage, 0.015f);
	}

	private void OnCollisionEnter(Collision collision)
	{
		ProcessCollision(collision);
	}

	public override void OnKilled(HitInfo info)
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		if (base.isClient)
		{
			base.OnKilled(info);
			return;
		}
		if (explosionEffect.isValid)
		{
			Effect.server.Run(explosionEffect.resourcePath, ((Component)this).transform.position, Vector3.up, null, broadcast: true);
		}
		Vector3 val = rigidBody.velocity * 0.25f;
		List<ServerGib> list = null;
		if (serverGibs.isValid)
		{
			GameObject gibSource = serverGibs.Get().GetComponent<ServerGib>()._gibSource;
			list = ServerGib.CreateGibs(serverGibs.resourcePath, ((Component)this).gameObject, gibSource, val, 3f);
		}
		Vector3 val2 = CenterPoint();
		if (fireBall.isValid && !InSafeZone())
		{
			RaycastHit val3 = default(RaycastHit);
			for (int i = 0; i < 12; i++)
			{
				BaseEntity baseEntity = GameManager.server.CreateEntity(fireBall.resourcePath, val2, ((Component)this).transform.rotation);
				if (!Object.op_Implicit((Object)(object)baseEntity))
				{
					continue;
				}
				float num = 3f;
				float num2 = 10f;
				Vector3 onUnitSphere = Random.onUnitSphere;
				((Vector3)(ref onUnitSphere)).Normalize();
				float num3 = Random.Range(0.5f, 4f);
				bool flag = Physics.Raycast(val2, onUnitSphere, ref val3, num3, 1218652417);
				Vector3 val4 = ((RaycastHit)(ref val3)).point;
				if (!flag)
				{
					val4 = val2 + onUnitSphere * num3;
				}
				val4 -= onUnitSphere * 0.5f;
				((Component)baseEntity).transform.position = val4;
				Collider component = ((Component)baseEntity).GetComponent<Collider>();
				baseEntity.Spawn();
				baseEntity.SetVelocity(val + onUnitSphere * Random.Range(num, num2));
				if (list == null)
				{
					continue;
				}
				foreach (ServerGib item in list)
				{
					Physics.IgnoreCollision(component, (Collider)(object)item.GetCollider(), true);
				}
			}
		}
		base.OnKilled(info);
	}
}
