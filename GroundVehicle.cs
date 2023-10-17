using System;
using System.Collections.Generic;
using Rust;
using UnityEngine;

public abstract class GroundVehicle : BaseVehicle, IEngineControllerUser, IEntity, TriggerHurtNotChild.IHurtTriggerUser
{
	[Header("GroundVehicle")]
	[SerializeField]
	protected GroundVehicleAudio gvAudio;

	[SerializeField]
	private GameObjectRef fuelStoragePrefab;

	[SerializeField]
	private Transform waterloggedPoint;

	[SerializeField]
	private GameObjectRef collisionEffect;

	[SerializeField]
	private float engineStartupTime = 0.5f;

	[SerializeField]
	private float minCollisionDamageForce = 20000f;

	[SerializeField]
	private float maxCollisionDamageForce = 2500000f;

	[SerializeField]
	private float collisionDamageMultiplier = 1f;

	protected VehicleEngineController<GroundVehicle> engineController;

	private Dictionary<BaseEntity, float> damageSinceLastTick = new Dictionary<BaseEntity, float>();

	private float nextCollisionDamageTime;

	private float dragMod;

	private float dragModDuration;

	private TimeSince timeSinceDragModSet;

	public Vector3 Velocity { get; private set; }

	public abstract float DriveWheelVelocity { get; }

	public bool LightsAreOn => HasFlag(Flags.Reserved5);

	public VehicleEngineController<GroundVehicle>.EngineState CurEngineState => engineController.CurEngineState;

	public override void InitShared()
	{
		base.InitShared();
		engineController = new VehicleEngineController<GroundVehicle>(this, base.isServer, engineStartupTime, fuelStoragePrefab, waterloggedPoint);
	}

	public override void OnFlagsChanged(Flags old, Flags next)
	{
		base.OnFlagsChanged(old, next);
		if (old != next && base.isServer)
		{
			ServerFlagsChanged(old, next);
		}
	}

	public float GetSpeed()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		if (IsStationary())
		{
			return 0f;
		}
		return Vector3.Dot(Velocity, ((Component)this).transform.forward);
	}

	public abstract float GetMaxForwardSpeed();

	public abstract float GetThrottleInput();

	public abstract float GetBrakeInput();

	protected override bool CanPushNow(BasePlayer pusher)
	{
		if (!base.CanPushNow(pusher))
		{
			return false;
		}
		if (pusher.isMounted || pusher.IsSwimming())
		{
			return false;
		}
		return !pusher.IsStandingOnEntity(this, 8192);
	}

	public override void ServerInit()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		base.ServerInit();
		timeSinceDragModSet = default(TimeSince);
		timeSinceDragModSet = TimeSince.op_Implicit(float.MaxValue);
	}

	public abstract void OnEngineStartFailed();

	public abstract bool MeetsEngineRequirements();

	protected virtual void ServerFlagsChanged(Flags old, Flags next)
	{
	}

	protected void OnCollisionEnter(Collision collision)
	{
		if (base.isServer)
		{
			ProcessCollision(collision);
		}
	}

	public override void VehicleFixedUpdate()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		base.VehicleFixedUpdate();
		if (base.IsMovingOrOn)
		{
			Velocity = GetLocalVelocity();
		}
		else
		{
			Velocity = Vector3.zero;
		}
		if (LightsAreOn && !AnyMounted())
		{
			SetFlag(Flags.Reserved5, b: false);
		}
		if (!(Time.time >= nextCollisionDamageTime))
		{
			return;
		}
		nextCollisionDamageTime = Time.time + 0.33f;
		foreach (KeyValuePair<BaseEntity, float> item in damageSinceLastTick)
		{
			DoCollisionDamage(item.Key, item.Value);
		}
		damageSinceLastTick.Clear();
	}

	public override void LightToggle(BasePlayer player)
	{
		if (IsDriver(player))
		{
			SetFlag(Flags.Reserved5, !LightsAreOn);
		}
	}

	public float GetDamageMultiplier(BaseEntity ent)
	{
		return Mathf.Abs(GetSpeed()) * 1f;
	}

	public void OnHurtTriggerOccupant(BaseEntity hurtEntity, DamageType damageType, float damageTotal)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		if (!base.isClient && !hurtEntity.IsDestroyed)
		{
			Vector3 val = hurtEntity.GetLocalVelocity() - Velocity;
			Vector3 val2 = ClosestPoint(((Component)hurtEntity).transform.position);
			Vector3 val3 = hurtEntity.RealisticMass * val;
			rigidBody.AddForceAtPosition(val3 * 1.25f, val2, (ForceMode)1);
			QueueCollisionDamage(this, ((Vector3)(ref val3)).magnitude * 0.75f / Time.deltaTime);
			SetTempDrag(2.25f, 1f);
		}
	}

	private float QueueCollisionDamage(BaseEntity hitEntity, float forceMagnitude)
	{
		float num = Mathf.InverseLerp(minCollisionDamageForce, maxCollisionDamageForce, forceMagnitude);
		if (num > 0f)
		{
			float num2 = Mathf.Lerp(1f, 200f, num) * collisionDamageMultiplier;
			if (damageSinceLastTick.TryGetValue(hitEntity, out var value))
			{
				if (value < num2)
				{
					damageSinceLastTick[hitEntity] = num2;
				}
			}
			else
			{
				damageSinceLastTick[hitEntity] = num2;
			}
		}
		return num;
	}

	protected virtual void DoCollisionDamage(BaseEntity hitEntity, float damage)
	{
		Hurt(damage, DamageType.Collision, this, useProtection: false);
	}

	private void ProcessCollision(Collision collision)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		if (base.isClient || collision == null || (Object)(object)collision.gameObject == (Object)null || (Object)(object)collision.gameObject == (Object)null)
		{
			return;
		}
		ContactPoint contact = collision.GetContact(0);
		BaseEntity baseEntity = null;
		if ((Object)(object)((ContactPoint)(ref contact)).otherCollider.attachedRigidbody == (Object)(object)rigidBody)
		{
			baseEntity = ((ContactPoint)(ref contact)).otherCollider.ToBaseEntity();
		}
		else if ((Object)(object)((ContactPoint)(ref contact)).thisCollider.attachedRigidbody == (Object)(object)rigidBody)
		{
			baseEntity = ((ContactPoint)(ref contact)).thisCollider.ToBaseEntity();
		}
		if ((Object)(object)baseEntity != (Object)null)
		{
			Vector3 impulse = collision.impulse;
			float forceMagnitude = ((Vector3)(ref impulse)).magnitude / Time.fixedDeltaTime;
			if (QueueCollisionDamage(baseEntity, forceMagnitude) > 0f)
			{
				TryShowCollisionFX(collision, collisionEffect);
			}
		}
	}

	public virtual float GetModifiedDrag()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		return (1f - Mathf.InverseLerp(0f, dragModDuration, TimeSince.op_Implicit(timeSinceDragModSet))) * dragMod;
	}

	public override EntityFuelSystem GetFuelSystem()
	{
		return engineController.FuelSystem;
	}

	protected override void OnChildAdded(BaseEntity child)
	{
		base.OnChildAdded(child);
		if (base.isServer && isSpawned)
		{
			GetFuelSystem().CheckNewChild(child);
		}
	}

	private void SetTempDrag(float drag, float duration)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		dragMod = Mathf.Clamp(drag, 0f, 1000f);
		timeSinceDragModSet = TimeSince.op_Implicit(0f);
		dragModDuration = duration;
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
