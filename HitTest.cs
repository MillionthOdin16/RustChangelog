using UnityEngine;

public class HitTest
{
	public enum Type
	{
		Generic,
		ProjectileEffect,
		Projectile,
		MeleeAttack,
		Use
	}

	public Type type;

	public Ray AttackRay;

	public float Radius;

	public float Forgiveness;

	public float MaxDistance;

	public RaycastHit RayHit;

	public bool MultiHit;

	public bool BestHit;

	public bool DidHit;

	public DamageProperties damageProperties;

	public GameObject gameObject;

	public Collider collider;

	public BaseEntity ignoreEntity;

	public BaseEntity HitEntity;

	public Vector3 HitPoint;

	public Vector3 HitNormal;

	public float HitDistance;

	public Transform HitTransform;

	public uint HitPart;

	public string HitMaterial;

	public void CopyFrom(HitTest other, bool copyHitInfo = false)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		type = other.type;
		AttackRay = other.AttackRay;
		Radius = other.Radius;
		Forgiveness = other.Forgiveness;
		MaxDistance = other.MaxDistance;
		RayHit = other.RayHit;
		damageProperties = other.damageProperties;
		ignoreEntity = other.ignoreEntity;
		if (copyHitInfo)
		{
			HitEntity = other.HitEntity;
			HitPoint = other.HitPoint;
			HitNormal = other.HitNormal;
			HitDistance = other.HitDistance;
			HitTransform = other.HitTransform;
			HitPart = other.HitPart;
			HitMaterial = other.HitMaterial;
			MultiHit = other.MultiHit;
			BestHit = other.BestHit;
			DidHit = other.DidHit;
		}
	}

	public Vector3 HitPointWorld()
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)HitEntity != (Object)null)
		{
			Transform val = HitTransform;
			if (!Object.op_Implicit((Object)(object)val))
			{
				val = ((Component)HitEntity).transform;
			}
			return val.TransformPoint(HitPoint);
		}
		return HitPoint;
	}

	public Vector3 HitNormalWorld()
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)HitEntity != (Object)null)
		{
			Transform val = HitTransform;
			if (!Object.op_Implicit((Object)(object)val))
			{
				val = ((Component)HitEntity).transform;
			}
			return val.TransformDirection(HitNormal);
		}
		return HitNormal;
	}

	public void Clear()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		type = Type.Generic;
		AttackRay = default(Ray);
		Radius = 0f;
		Forgiveness = 0f;
		MaxDistance = 0f;
		RayHit = default(RaycastHit);
		MultiHit = false;
		BestHit = false;
		DidHit = false;
		damageProperties = null;
		gameObject = null;
		collider = null;
		ignoreEntity = null;
		HitEntity = null;
		HitPoint = default(Vector3);
		HitNormal = default(Vector3);
		HitDistance = 0f;
		HitTransform = null;
		HitPart = 0u;
		HitMaterial = null;
	}
}
