using System;
using System.Collections.Generic;
using Facepunch;
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

	protected virtual int mask => 1237003025;

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

	public virtual bool ShouldSwim()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return swimScale != Vector3.zero;
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
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		if (impacted)
		{
			return false;
		}
		CurrentVelocity += Physics.gravity * gravityModifier * Time.fixedDeltaTime * Time.timeScale;
		Vector3 val = CurrentVelocity;
		if (ShouldSwim())
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
		List<RaycastHit> list = Pool.GetList<RaycastHit>();
		float num2 = ((Vector3)(ref val)).magnitude * Time.fixedDeltaTime;
		Vector3 position = ((Component)this).transform.position;
		GamePhysics.TraceAll(new Ray(position, ((Vector3)(ref val)).normalized), radius, list, num2 + scanRange, mask, (QueryTriggerInteraction)1);
		foreach (RaycastHit item in list)
		{
			RaycastHit current = item;
			BaseEntity entity = current.GetEntity();
			if ((!((Object)(object)entity != (Object)null) || !entity.isClient) && IsAValidHit(entity))
			{
				ColliderInfo colliderInfo = (((Object)(object)((RaycastHit)(ref current)).collider != (Object)null) ? ((Component)((RaycastHit)(ref current)).collider).GetComponent<ColliderInfo>() : null);
				if ((Object)(object)colliderInfo == (Object)null || colliderInfo.HasFlag(ColliderInfo.Flags.Shootable))
				{
					Transform transform = ((Component)this).transform;
					transform.position += ((Component)this).transform.forward * Mathf.Max(0f, ((RaycastHit)(ref current)).distance - 0.1f);
					((Component)this).GetComponent<IProjectileImpact>()?.ProjectileImpact(current, position);
					impacted = true;
					Pool.FreeList<RaycastHit>(ref list);
					return false;
				}
			}
		}
		Transform transform2 = ((Component)this).transform;
		transform2.position += ((Component)this).transform.forward * num2;
		((Component)this).transform.rotation = Quaternion.LookRotation(((Vector3)(ref val)).normalized);
		Pool.FreeList<RaycastHit>(ref list);
		return true;
	}

	protected virtual bool IsAValidHit(BaseEntity hitEnt)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
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
