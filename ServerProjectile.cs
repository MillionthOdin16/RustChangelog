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

	public float scanRange;

	public Vector3 swimScale;

	public Vector3 swimSpeed;

	public float radius;

	private bool impacted;

	private float swimRandom;

	public virtual bool HasRangeLimit => true;

	protected virtual int mask => 1236478737;

	public Vector3 CurrentVelocity { get; protected set; }

	public float GetMaxRange(float maxFuseTime)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
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
				Transform transform = ((Component)this).transform;
				transform.position += ((Component)this).transform.forward * Mathf.Max(0f, ((RaycastHit)(ref hitInfo)).distance - 0.1f);
				((Component)this).GetComponent<IProjectileImpact>()?.ProjectileImpact(hitInfo, position);
				impacted = true;
				return false;
			}
		}
		Transform transform2 = ((Component)this).transform;
		transform2.position += ((Component)this).transform.forward * num2;
		((Component)this).transform.rotation = Quaternion.LookRotation(((Vector3)(ref val)).normalized);
		return true;
	}

	protected virtual bool IsAValidHit(BaseEntity hitEnt)
	{
		if (hitEnt.IsValid() && base.baseEntity.creatorEntity.IsValid())
		{
			return hitEnt.net.ID != base.baseEntity.creatorEntity.net.ID;
		}
		return true;
	}

	public virtual void InitializeVelocity(Vector3 overrideVel)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		((Component)this).transform.rotation = Quaternion.LookRotation(((Vector3)(ref overrideVel)).normalized);
		initialVelocity = overrideVel;
		CurrentVelocity = overrideVel;
	}
}
