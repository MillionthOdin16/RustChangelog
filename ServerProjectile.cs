using System;
using UnityEngine;

public class ServerProjectile : EntityComponent<BaseEntity>, IServerComponent
{
	public interface IProjectileImpact
	{
		void ProjectileImpact(RaycastHit hitInfo, Vector3 rayOrigin);
	}

	public Vector3 initialVelocity;

	public float drag;

	public float gravityModifier = 1f;

	public float speed = 15f;

	public float scanRange = 0f;

	public Vector3 swimScale;

	public Vector3 swimSpeed;

	public float radius = 0f;

	private bool impacted = false;

	private float swimRandom = 0f;

	public virtual bool HasRangeLimit => true;

	protected virtual int mask => 1237003025;

	public Vector3 CurrentVelocity { get; protected set; }

	public float GetMaxRange(float maxFuseTime)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		if (gravityModifier == 0f)
		{
			return float.PositiveInfinity;
		}
		float num = Mathf.Sin((float)Math.PI / 2f) * speed * speed / (0f - Physics.gravity.y * gravityModifier);
		float num2 = speed * maxFuseTime;
		return Mathf.Min(num, num2);
	}

	protected void FixedUpdate()
	{
		if ((Object)(object)base.baseEntity != (Object)null && base.baseEntity.isServer)
		{
			DoMovement();
		}
	}

	public virtual bool DoMovement()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		if (impacted)
		{
			return false;
		}
		CurrentVelocity += Physics.gravity * gravityModifier * Time.fixedDeltaTime * Time.timeScale;
		Vector3 val = CurrentVelocity;
		if (swimScale != Vector3.zero)
		{
			if (swimRandom == 0f)
			{
				swimRandom = Random.Range(0f, 20f);
			}
			float num = Time.time + swimRandom;
			Vector3 val2 = default(Vector3);
			((Vector3)(ref val2))._002Ector(Mathf.Sin(num * swimSpeed.x) * swimScale.x, Mathf.Cos(num * swimSpeed.y) * swimScale.y, Mathf.Sin(num * swimSpeed.z) * swimScale.z);
			val2 = ((Component)this).transform.InverseTransformDirection(val2);
			val += val2;
		}
		float num2 = ((Vector3)(ref val)).magnitude * Time.fixedDeltaTime;
		Vector3 position = ((Component)this).transform.position;
		if (GamePhysics.Trace(new Ray(position, ((Vector3)(ref val)).normalized), radius, out var hitInfo, num2 + scanRange, mask, (QueryTriggerInteraction)1))
		{
			BaseEntity entity = hitInfo.GetEntity();
			if (IsAValidHit(entity))
			{
				ColliderInfo colliderInfo = (((Object)(object)((RaycastHit)(ref hitInfo)).collider != (Object)null) ? ((Component)((RaycastHit)(ref hitInfo)).collider).GetComponent<ColliderInfo>() : null);
				if ((Object)(object)colliderInfo == (Object)null || colliderInfo.HasFlag(ColliderInfo.Flags.Shootable))
				{
					Transform transform = ((Component)this).transform;
					transform.position += ((Component)this).transform.forward * Mathf.Max(0f, ((RaycastHit)(ref hitInfo)).distance - 0.1f);
					((Component)this).GetComponent<IProjectileImpact>()?.ProjectileImpact(hitInfo, position);
					impacted = true;
					return false;
				}
			}
		}
		Transform transform2 = ((Component)this).transform;
		transform2.position += ((Component)this).transform.forward * num2;
		((Component)this).transform.rotation = Quaternion.LookRotation(((Vector3)(ref val)).normalized);
		return true;
	}

	protected virtual bool IsAValidHit(BaseEntity hitEnt)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		return !hitEnt.IsValid() || !base.baseEntity.creatorEntity.IsValid() || hitEnt.net.ID != base.baseEntity.creatorEntity.net.ID;
	}

	public virtual void InitializeVelocity(Vector3 overrideVel)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		((Component)this).transform.rotation = Quaternion.LookRotation(((Vector3)(ref overrideVel)).normalized);
		initialVelocity = overrideVel;
		CurrentVelocity = overrideVel;
	}
}
