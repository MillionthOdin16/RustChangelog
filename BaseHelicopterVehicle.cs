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

	public float currentThrottle;

	public float avgThrust;

	public float liftDotMax = 0.75f;

	public float altForceDotMin = 0.85f;

	public float liftFraction = 0.25f;

	public float thrustLerpSpeed = 1f;

	private float avgTerrainHeight;

	public const Flags Flag_InternalLights = Flags.Reserved6;

	protected HelicopterInputState currentInputState = new HelicopterInputState();

	protected float lastPlayerInputTime;

	protected float hoverForceScale = 0.99f;

	protected Vector3 damageTorque;

	private float nextDamageTime;

	private float nextEffectTime;

	private float pendingImpactDamage;

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
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		base.VehicleFixedUpdate();
		if (Time.time > lastPlayerInputTime + 0.5f)
		{
			SetDefaultInputState();
		}
		EnableGlobalBroadcast(IsEngineOn());
		MovementUpdate();
		SetFlag(Flags.Reserved6, TOD_Sky.Instance.IsNight);
		GameObject[] array = killTriggers;
		foreach (GameObject obj in array)
		{
			bool active = rigidBody.velocity.y < 0f;
			obj.SetActive(active);
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
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		SetDamageTorque(Vector3.zero);
	}

	public void SetDamageTorque(Vector3 newTorque)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		damageTorque = newTorque;
	}

	public void AddDamageTorque(Vector3 torqueToAdd)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		damageTorque += torqueToAdd;
	}

	public virtual void MovementUpdate()
	{
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
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
				bool num4 = Physics.Raycast(val2, onUnitSphere, ref val3, num3, 1218652417);
				Vector3 val4 = ((RaycastHit)(ref val3)).point;
				if (!num4)
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
