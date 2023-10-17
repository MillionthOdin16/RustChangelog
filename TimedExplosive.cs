using System;
using System.Collections.Generic;
using Facepunch;
using Rust;
using Rust.Ai;
using UnityEngine;

public class TimedExplosive : BaseEntity, ServerProjectile.IProjectileImpact
{
	public float timerAmountMin = 10f;

	public float timerAmountMax = 20f;

	public float minExplosionRadius;

	public float explosionRadius = 10f;

	public bool explodeOnContact;

	public bool canStick;

	public bool onlyDamageParent;

	public GameObjectRef explosionEffect;

	[Tooltip("Optional: Will fall back to explosionEffect if not assigned.")]
	public GameObjectRef underwaterExplosionEffect;

	public GameObjectRef stickEffect;

	public GameObjectRef bounceEffect;

	public bool explosionUsesForward;

	public bool waterCausesExplosion;

	public List<DamageTypeEntry> damageTypes = new List<DamageTypeEntry>();

	[NonSerialized]
	private float lastBounceTime;

	private CollisionDetectionMode? initialCollisionDetectionMode;

	protected virtual bool AlwaysRunWaterCheck => false;

	public void SetDamageScale(float scale)
	{
		foreach (DamageTypeEntry damageType in damageTypes)
		{
			damageType.amount *= scale;
		}
	}

	public override float GetNetworkTime()
	{
		return Time.fixedTime;
	}

	public override void ServerInit()
	{
		lastBounceTime = Time.time;
		base.ServerInit();
		SetFuse(GetRandomTimerTime());
		ReceiveCollisionMessages(b: true);
		if (waterCausesExplosion || AlwaysRunWaterCheck)
		{
			((FacepunchBehaviour)this).InvokeRepeating((Action)WaterCheck, 0f, 0.5f);
		}
	}

	public virtual void WaterCheck()
	{
		if (waterCausesExplosion && WaterFactor() >= 0.5f)
		{
			Explode();
		}
	}

	public virtual void SetFuse(float fuseLength)
	{
		if (base.isServer)
		{
			((FacepunchBehaviour)this).Invoke((Action)Explode, fuseLength);
			SetFlag(Flags.Reserved2, b: true);
		}
	}

	public virtual float GetRandomTimerTime()
	{
		return Random.Range(timerAmountMin, timerAmountMax);
	}

	public virtual void ProjectileImpact(RaycastHit info, Vector3 rayOrigin)
	{
		Explode();
	}

	public virtual void Explode()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		Explode(PivotPoint());
	}

	public virtual void Explode(Vector3 explosionFxPos)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0425: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_032a: Unknown result type (might be due to invalid IL or missing references)
		//IL_032b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0338: Unknown result type (might be due to invalid IL or missing references)
		//IL_033d: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cc: Unknown result type (might be due to invalid IL or missing references)
		Collider component = ((Component)this).GetComponent<Collider>();
		if (Object.op_Implicit((Object)(object)component))
		{
			component.enabled = false;
		}
		bool flag = false;
		if (underwaterExplosionEffect.isValid)
		{
			flag = WaterLevel.GetWaterDepth(((Component)this).transform.position) > 1f;
		}
		if (flag)
		{
			Effect.server.Run(underwaterExplosionEffect.resourcePath, explosionFxPos, explosionUsesForward ? ((Component)this).transform.forward : Vector3.up, null, broadcast: true);
		}
		else if (explosionEffect.isValid)
		{
			Effect.server.Run(explosionEffect.resourcePath, explosionFxPos, explosionUsesForward ? ((Component)this).transform.forward : Vector3.up, null, broadcast: true);
		}
		if (damageTypes.Count > 0)
		{
			if (onlyDamageParent)
			{
				Vector3 val = CenterPoint();
				DamageUtil.RadiusDamage(creatorEntity, LookupPrefab(), val, minExplosionRadius, explosionRadius, damageTypes, 166144, useLineOfSight: true);
				BaseEntity baseEntity = GetParentEntity();
				BaseCombatEntity baseCombatEntity = baseEntity as BaseCombatEntity;
				while ((Object)(object)baseCombatEntity == (Object)null && (Object)(object)baseEntity != (Object)null && baseEntity.HasParent())
				{
					baseEntity = baseEntity.GetParentEntity();
					baseCombatEntity = baseEntity as BaseCombatEntity;
				}
				if ((Object)(object)baseEntity == (Object)null || !((Component)baseEntity).gameObject.IsOnLayer((Layer)21))
				{
					List<BuildingBlock> list = Pool.GetList<BuildingBlock>();
					Vis.Entities(val, explosionRadius, list, 2097152, (QueryTriggerInteraction)1);
					BuildingBlock buildingBlock = null;
					float num = float.PositiveInfinity;
					foreach (BuildingBlock item in list)
					{
						if (!item.isClient && !item.IsDestroyed && !(item.healthFraction <= 0f))
						{
							float num2 = Vector3.Distance(item.ClosestPoint(val), val);
							if (num2 < num && item.IsVisible(val, explosionRadius))
							{
								buildingBlock = item;
								num = num2;
							}
						}
					}
					if (Object.op_Implicit((Object)(object)buildingBlock))
					{
						HitInfo hitInfo = new HitInfo();
						hitInfo.Initiator = creatorEntity;
						hitInfo.WeaponPrefab = LookupPrefab();
						hitInfo.damageTypes.Add(damageTypes);
						hitInfo.PointStart = val;
						hitInfo.PointEnd = ((Component)buildingBlock).transform.position;
						float amount = 1f - Mathf.Clamp01((num - minExplosionRadius) / (explosionRadius - minExplosionRadius));
						hitInfo.damageTypes.ScaleAll(amount);
						buildingBlock.Hurt(hitInfo);
					}
					Pool.FreeList<BuildingBlock>(ref list);
				}
				if (Object.op_Implicit((Object)(object)baseCombatEntity))
				{
					HitInfo hitInfo2 = new HitInfo();
					hitInfo2.Initiator = creatorEntity;
					hitInfo2.WeaponPrefab = LookupPrefab();
					hitInfo2.damageTypes.Add(damageTypes);
					hitInfo2.PointStart = val;
					hitInfo2.PointEnd = ((Component)baseCombatEntity).transform.position;
					baseCombatEntity.Hurt(hitInfo2);
				}
				else if ((Object)(object)baseEntity != (Object)null)
				{
					HitInfo hitInfo3 = new HitInfo();
					hitInfo3.Initiator = creatorEntity;
					hitInfo3.WeaponPrefab = LookupPrefab();
					hitInfo3.damageTypes.Add(damageTypes);
					hitInfo3.PointStart = val;
					hitInfo3.PointEnd = ((Component)baseEntity).transform.position;
					baseEntity.OnAttacked(hitInfo3);
				}
				if ((Object)(object)creatorEntity != (Object)null && damageTypes != null)
				{
					float num3 = 0f;
					foreach (DamageTypeEntry damageType in damageTypes)
					{
						num3 += damageType.amount;
					}
					Sensation sensation = default(Sensation);
					sensation.Type = SensationType.Explosion;
					sensation.Position = ((Component)creatorEntity).transform.position;
					sensation.Radius = explosionRadius * 17f;
					sensation.DamagePotential = num3;
					sensation.InitiatorPlayer = creatorEntity as BasePlayer;
					sensation.Initiator = creatorEntity;
					Sense.Stimulate(sensation);
				}
			}
			else
			{
				DamageUtil.RadiusDamage(creatorEntity, LookupPrefab(), CenterPoint(), minExplosionRadius, explosionRadius, damageTypes, 1076005121, useLineOfSight: true);
				if ((Object)(object)creatorEntity != (Object)null && damageTypes != null)
				{
					float num4 = 0f;
					foreach (DamageTypeEntry damageType2 in damageTypes)
					{
						num4 += damageType2.amount;
					}
					Sensation sensation = default(Sensation);
					sensation.Type = SensationType.Explosion;
					sensation.Position = ((Component)creatorEntity).transform.position;
					sensation.Radius = explosionRadius * 17f;
					sensation.DamagePotential = num4;
					sensation.InitiatorPlayer = creatorEntity as BasePlayer;
					sensation.Initiator = creatorEntity;
					Sense.Stimulate(sensation);
				}
			}
		}
		if (!base.IsDestroyed && !HasFlag(Flags.Broken))
		{
			Kill(DestroyMode.Gib);
		}
	}

	public override void OnCollision(Collision collision, BaseEntity hitEntity)
	{
		if (canStick && !IsStuck())
		{
			bool flag = true;
			if (Object.op_Implicit((Object)(object)hitEntity))
			{
				flag = CanStickTo(hitEntity);
				if (!flag)
				{
					Collider component = ((Component)this).GetComponent<Collider>();
					if ((Object)(object)collision.collider != (Object)null && (Object)(object)component != (Object)null)
					{
						Physics.IgnoreCollision(collision.collider, component);
					}
				}
			}
			if (flag)
			{
				DoCollisionStick(collision, hitEntity);
			}
		}
		if (explodeOnContact && !IsBusy())
		{
			SetMotionEnabled(wantsMotion: false);
			SetFlag(Flags.Busy, b: true, recursive: false, networkupdate: false);
			((FacepunchBehaviour)this).Invoke((Action)Explode, 0.015f);
		}
		else
		{
			DoBounceEffect();
		}
	}

	public virtual bool CanStickTo(BaseEntity entity)
	{
		DecorDeployable decorDeployable = default(DecorDeployable);
		if (((Component)entity).TryGetComponent<DecorDeployable>(ref decorDeployable))
		{
			return false;
		}
		if (entity is Drone)
		{
			return false;
		}
		return true;
	}

	private void DoBounceEffect()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		if (!bounceEffect.isValid || Time.time - lastBounceTime < 0.2f)
		{
			return;
		}
		Rigidbody component = ((Component)this).GetComponent<Rigidbody>();
		if (Object.op_Implicit((Object)(object)component))
		{
			Vector3 velocity = component.velocity;
			if (((Vector3)(ref velocity)).magnitude < 1f)
			{
				return;
			}
		}
		if (bounceEffect.isValid)
		{
			Effect.server.Run(bounceEffect.resourcePath, ((Component)this).transform.position, Vector3.up, null, broadcast: true);
		}
		lastBounceTime = Time.time;
	}

	private void DoCollisionStick(Collision collision, BaseEntity ent)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		ContactPoint contact = collision.GetContact(0);
		DoStick(((ContactPoint)(ref contact)).point, ((ContactPoint)(ref contact)).normal, ent, collision.collider);
	}

	public virtual void SetMotionEnabled(bool wantsMotion)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		Rigidbody component = ((Component)this).GetComponent<Rigidbody>();
		if (Object.op_Implicit((Object)(object)component))
		{
			if (!initialCollisionDetectionMode.HasValue)
			{
				initialCollisionDetectionMode = component.collisionDetectionMode;
			}
			component.useGravity = wantsMotion;
			if (!wantsMotion)
			{
				component.collisionDetectionMode = (CollisionDetectionMode)0;
			}
			component.isKinematic = !wantsMotion;
			if (wantsMotion)
			{
				component.collisionDetectionMode = initialCollisionDetectionMode.Value;
			}
		}
	}

	public bool IsStuck()
	{
		Rigidbody component = ((Component)this).GetComponent<Rigidbody>();
		if (Object.op_Implicit((Object)(object)component) && !component.isKinematic)
		{
			return false;
		}
		Collider component2 = ((Component)this).GetComponent<Collider>();
		if (Object.op_Implicit((Object)(object)component2) && component2.enabled)
		{
			return false;
		}
		return parentEntity.IsValid(serverside: true);
	}

	public void DoStick(Vector3 position, Vector3 normal, BaseEntity ent, Collider collider)
	{
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)ent == (Object)null)
		{
			return;
		}
		if (ent is TimedExplosive)
		{
			if (!ent.HasParent())
			{
				return;
			}
			position = ((Component)ent).transform.position;
			ent = ent.parentEntity.Get(serverside: true);
		}
		SetMotionEnabled(wantsMotion: false);
		SetCollisionEnabled(wantsCollision: false);
		if (!HasChild(ent))
		{
			((Component)this).transform.position = position;
			((Component)this).transform.rotation = Quaternion.LookRotation(normal, ((Component)this).transform.up);
			if ((Object)(object)collider != (Object)null)
			{
				SetParent(ent, ent.FindBoneID(((Component)collider).transform), worldPositionStays: true);
			}
			else
			{
				SetParent(ent, StringPool.closest, worldPositionStays: true);
			}
			if (stickEffect.isValid)
			{
				Effect.server.Run(stickEffect.resourcePath, ((Component)this).transform.position, Vector3.up, null, broadcast: true);
			}
			ReceiveCollisionMessages(b: false);
		}
	}

	private void UnStick()
	{
		if (Object.op_Implicit((Object)(object)GetParentEntity()))
		{
			SetParent(null, worldPositionStays: true, sendImmediate: true);
			SetMotionEnabled(wantsMotion: true);
			SetCollisionEnabled(wantsCollision: true);
			ReceiveCollisionMessages(b: true);
		}
	}

	internal override void OnParentRemoved()
	{
		UnStick();
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
	}

	public override void PostServerLoad()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		base.PostServerLoad();
		if (parentEntity.IsValid(serverside: true))
		{
			DoStick(((Component)this).transform.position, ((Component)this).transform.forward, parentEntity.Get(serverside: true), null);
		}
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (info.msg.explosive != null)
		{
			parentEntity.uid = info.msg.explosive.parentid;
		}
	}

	public virtual void SetCollisionEnabled(bool wantsCollision)
	{
		Collider component = ((Component)this).GetComponent<Collider>();
		if (Object.op_Implicit((Object)(object)component) && component.enabled != wantsCollision)
		{
			component.enabled = wantsCollision;
		}
	}
}
