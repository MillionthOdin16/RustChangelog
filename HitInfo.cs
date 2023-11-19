using Network;
using ProtoBuf;
using Rust;
using UnityEngine;

public class HitInfo
{
	public BaseEntity Initiator;

	public BaseEntity WeaponPrefab;

	public AttackEntity Weapon;

	public bool DoHitEffects = true;

	public bool DoDecals = true;

	public bool IsPredicting;

	public bool UseProtection = true;

	public Connection Predicted;

	public bool DidHit;

	public BaseEntity HitEntity;

	public uint HitBone;

	public uint HitPart;

	public uint HitMaterial;

	public Vector3 HitPositionWorld;

	public Vector3 HitPositionLocal;

	public Vector3 HitNormalWorld;

	public Vector3 HitNormalLocal;

	public Vector3 PointStart;

	public Vector3 PointEnd;

	public int ProjectileID;

	public int ProjectileHits;

	public float ProjectileDistance;

	public float ProjectileIntegrity;

	public float ProjectileTravelTime;

	public float ProjectileTrajectoryMismatch;

	public Vector3 ProjectileVelocity;

	public Projectile ProjectilePrefab;

	public PhysicMaterial material;

	public DamageProperties damageProperties;

	public DamageTypeList damageTypes = new DamageTypeList();

	public bool CanGather;

	public bool DidGather;

	public float gatherScale = 1f;

	public BasePlayer InitiatorPlayer
	{
		get
		{
			if (!Object.op_Implicit((Object)(object)Initiator))
			{
				return null;
			}
			return Initiator.ToPlayer();
		}
	}

	public Vector3 attackNormal
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = PointEnd - PointStart;
			return ((Vector3)(ref val)).normalized;
		}
	}

	public bool hasDamage => damageTypes.Total() > 0f;

	public bool isHeadshot
	{
		get
		{
			if ((Object)(object)HitEntity == (Object)null)
			{
				return false;
			}
			BaseCombatEntity baseCombatEntity = HitEntity as BaseCombatEntity;
			if ((Object)(object)baseCombatEntity == (Object)null)
			{
				return false;
			}
			if ((Object)(object)baseCombatEntity.skeletonProperties == (Object)null)
			{
				return false;
			}
			SkeletonProperties.BoneProperty boneProperty = baseCombatEntity.skeletonProperties.FindBone(HitBone);
			if (boneProperty == null)
			{
				return false;
			}
			return boneProperty.area == HitArea.Head;
		}
	}

	public Phrase bonePhrase
	{
		get
		{
			if ((Object)(object)HitEntity == (Object)null)
			{
				return null;
			}
			BaseCombatEntity baseCombatEntity = HitEntity as BaseCombatEntity;
			if ((Object)(object)baseCombatEntity == (Object)null)
			{
				return null;
			}
			if ((Object)(object)baseCombatEntity.skeletonProperties == (Object)null)
			{
				return null;
			}
			return baseCombatEntity.skeletonProperties.FindBone(HitBone)?.name;
		}
	}

	public string boneName
	{
		get
		{
			Phrase val = bonePhrase;
			if (val != null)
			{
				return val.english;
			}
			return "N/A";
		}
	}

	public HitArea boneArea
	{
		get
		{
			if ((Object)(object)HitEntity == (Object)null)
			{
				return (HitArea)(-1);
			}
			BaseCombatEntity baseCombatEntity = HitEntity as BaseCombatEntity;
			if ((Object)(object)baseCombatEntity == (Object)null)
			{
				return (HitArea)(-1);
			}
			return baseCombatEntity.SkeletonLookup(HitBone);
		}
	}

	public bool IsProjectile()
	{
		return ProjectileID != 0;
	}

	public HitInfo()
	{
	}

	public HitInfo(BaseEntity attacker, BaseEntity target, DamageType type, float damageAmount, Vector3 vhitPosition)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		Initiator = attacker;
		HitEntity = target;
		HitPositionWorld = vhitPosition;
		if ((Object)(object)attacker != (Object)null)
		{
			PointStart = ((Component)attacker).transform.position;
		}
		damageTypes.Add(type, damageAmount);
	}

	public HitInfo(BaseEntity attacker, BaseEntity target, DamageType type, float damageAmount)
		: this(attacker, target, type, damageAmount, ((Component)target).transform.position)
	{
	}//IL_000c: Unknown result type (might be due to invalid IL or missing references)


	public void LoadFromAttack(Attack attack, bool serverSide)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		HitEntity = null;
		PointStart = attack.pointStart;
		PointEnd = attack.pointEnd;
		if (((NetworkableId)(ref attack.hitID)).IsValid)
		{
			DidHit = true;
			if (serverSide)
			{
				HitEntity = BaseNetworkable.serverEntities.Find(attack.hitID) as BaseEntity;
			}
			if (Object.op_Implicit((Object)(object)HitEntity))
			{
				HitBone = attack.hitBone;
				HitPart = attack.hitPartID;
			}
		}
		DidHit = true;
		HitPositionLocal = attack.hitPositionLocal;
		HitPositionWorld = attack.hitPositionWorld;
		HitNormalLocal = ((Vector3)(ref attack.hitNormalLocal)).normalized;
		HitNormalWorld = ((Vector3)(ref attack.hitNormalWorld)).normalized;
		HitMaterial = attack.hitMaterialID;
		if (((NetworkableId)(ref attack.srcParentID)).IsValid)
		{
			BaseEntity baseEntity = null;
			if (serverSide)
			{
				baseEntity = BaseNetworkable.serverEntities.Find(attack.srcParentID) as BaseEntity;
			}
			if (baseEntity.IsValid())
			{
				PointStart = ((Component)baseEntity).transform.TransformPoint(PointStart);
			}
		}
		if (((NetworkableId)(ref attack.dstParentID)).IsValid)
		{
			BaseEntity baseEntity2 = null;
			if (serverSide)
			{
				baseEntity2 = BaseNetworkable.serverEntities.Find(attack.dstParentID) as BaseEntity;
			}
			if (baseEntity2.IsValid())
			{
				PointEnd = ((Component)baseEntity2).transform.TransformPoint(PointEnd);
				HitPositionWorld = ((Component)baseEntity2).transform.TransformPoint(HitPositionWorld);
				HitNormalWorld = ((Component)baseEntity2).transform.TransformDirection(HitNormalWorld);
			}
		}
	}

	public Vector3 PositionOnRay(Vector3 position)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		Ray val = default(Ray);
		((Ray)(ref val))._002Ector(PointStart, attackNormal);
		if ((Object)(object)ProjectilePrefab == (Object)null)
		{
			return val.ClosestPoint(position);
		}
		Sphere val2 = default(Sphere);
		((Sphere)(ref val2))._002Ector(position, ProjectilePrefab.thickness);
		RaycastHit val3 = default(RaycastHit);
		if (((Sphere)(ref val2)).Trace(val, ref val3, float.PositiveInfinity))
		{
			return ((RaycastHit)(ref val3)).point;
		}
		return position;
	}

	public Vector3 HitPositionOnRay()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return PositionOnRay(HitPositionWorld);
	}

	public bool IsNaNOrInfinity()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		if (Vector3Ex.IsNaNOrInfinity(PointStart))
		{
			return true;
		}
		if (Vector3Ex.IsNaNOrInfinity(PointEnd))
		{
			return true;
		}
		if (Vector3Ex.IsNaNOrInfinity(HitPositionWorld))
		{
			return true;
		}
		if (Vector3Ex.IsNaNOrInfinity(HitPositionLocal))
		{
			return true;
		}
		if (Vector3Ex.IsNaNOrInfinity(HitNormalWorld))
		{
			return true;
		}
		if (Vector3Ex.IsNaNOrInfinity(HitNormalLocal))
		{
			return true;
		}
		if (Vector3Ex.IsNaNOrInfinity(ProjectileVelocity))
		{
			return true;
		}
		if (float.IsNaN(ProjectileDistance))
		{
			return true;
		}
		if (float.IsInfinity(ProjectileDistance))
		{
			return true;
		}
		if (float.IsNaN(ProjectileIntegrity))
		{
			return true;
		}
		if (float.IsInfinity(ProjectileIntegrity))
		{
			return true;
		}
		if (float.IsNaN(ProjectileTravelTime))
		{
			return true;
		}
		if (float.IsInfinity(ProjectileTravelTime))
		{
			return true;
		}
		if (float.IsNaN(ProjectileTrajectoryMismatch))
		{
			return true;
		}
		if (float.IsInfinity(ProjectileTrajectoryMismatch))
		{
			return true;
		}
		return false;
	}
}
