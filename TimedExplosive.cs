using System;
using System.Collections.Generic;
using Facepunch;
using Facepunch.Rust;
using Rust;
using Rust.Ai;
using UnityEngine;
using UnityEngine.Profiling;

public class TimedExplosive : BaseEntity, ServerProjectile.IProjectileImpact
{
	public float timerAmountMin = 10f;

	public float timerAmountMax = 20f;

	public float minExplosionRadius = 0f;

	public float explosionRadius = 10f;

	public bool explodeOnContact = false;

	public bool canStick = false;

	public bool onlyDamageParent = false;

	public bool BlindAI = false;

	public float aiBlindDuration = 2.5f;

	public float aiBlindRange = 4f;

	public GameObjectRef explosionEffect;

	[Tooltip("Optional: Will fall back to watersurfaceExplosionEffect or explosionEffect if not assigned.")]
	public GameObjectRef underwaterExplosionEffect;

	[Tooltip("Optional: Will fall back to underwaterExplosionEffect or explosionEffect if not assigned.")]
	public GameObjectRef watersurfaceExplosionEffect;

	public GameObjectRef stickEffect;

	public GameObjectRef bounceEffect;

	public bool explosionUsesForward = false;

	public bool waterCausesExplosion = false;

	public List<DamageTypeEntry> damageTypes = new List<DamageTypeEntry>();

	[NonSerialized]
	private float lastBounceTime;

	private bool hadRB;

	private float rbMass;

	private float rbDrag;

	private float rbAngularDrag;

	private CollisionDetectionMode rbCollisionMode;

	private const int parentOnlySplashDamage = 166144;

	private const int fullSplashDamage = 1210222849;

	private static BaseEntity[] queryResults = new BaseEntity[64];

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
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		Explode(PivotPoint());
	}

	public virtual void Explode(Vector3 explosionFxPos)
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0503: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0404: Unknown result type (might be due to invalid IL or missing references)
		//IL_0409: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		//IL_030e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a6: Unknown result type (might be due to invalid IL or missing references)
		Analytics.Azure.OnExplosion(this);
		Collider component = ((Component)this).GetComponent<Collider>();
		if (Object.op_Implicit((Object)(object)component))
		{
			component.enabled = false;
		}
		GameObjectRef gameObjectRef = explosionEffect;
		if (underwaterExplosionEffect.isValid || watersurfaceExplosionEffect.isValid)
		{
			WaterLevel.WaterInfo waterInfo = WaterLevel.GetWaterInfo(explosionFxPos - new Vector3(0f, 0.25f, 0f), waves: true, volumes: false);
			if (waterInfo.isValid && waterInfo.overallDepth > 0.5f)
			{
				gameObjectRef = ((!(waterInfo.currentDepth > 1f)) ? (watersurfaceExplosionEffect.isValid ? watersurfaceExplosionEffect : underwaterExplosionEffect) : (underwaterExplosionEffect.isValid ? underwaterExplosionEffect : watersurfaceExplosionEffect));
			}
		}
		if (gameObjectRef.isValid)
		{
			Effect.server.Run(gameObjectRef.resourcePath, explosionFxPos, explosionUsesForward ? ((Component)this).transform.forward : Vector3.up, null, broadcast: true);
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
							Vector3 val2 = item.ClosestPoint(val);
							float num2 = Vector3.Distance(val2, val);
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
				DamageUtil.RadiusDamage(creatorEntity, LookupPrefab(), CenterPoint(), minExplosionRadius, explosionRadius, damageTypes, 1210222849, useLineOfSight: true);
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
			BlindAnyAI();
		}
		if (!base.IsDestroyed && !HasFlag(Flags.Broken))
		{
			Kill(DestroyMode.Gib);
		}
	}

	private void BlindAnyAI()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		if (!BlindAI)
		{
			return;
		}
		int brainsInSphere = Query.Server.GetBrainsInSphere(((Component)this).transform.position, 10f, queryResults);
		for (int i = 0; i < brainsInSphere; i++)
		{
			BaseEntity baseEntity = queryResults[i];
			if (Vector3.Distance(((Component)this).transform.position, ((Component)baseEntity).transform.position) > aiBlindRange)
			{
				continue;
			}
			BaseAIBrain component = ((Component)baseEntity).GetComponent<BaseAIBrain>();
			if (!((Object)(object)component == (Object)null))
			{
				BaseEntity brainBaseEntity = component.GetBrainBaseEntity();
				if (!((Object)(object)brainBaseEntity == (Object)null) && brainBaseEntity.IsVisible(CenterPoint()))
				{
					float blinded = aiBlindDuration * component.BlindDurationMultiplier * Random.Range(0.6f, 1.4f);
					component.SetBlinded(blinded);
					queryResults[i] = null;
				}
			}
		}
	}

	public override void OnCollision(Collision collision, BaseEntity hitEntity)
	{
		if (canStick && !IsStuck())
		{
			Profiler.BeginSample("DoCollisionStick");
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
			Profiler.EndSample();
		}
		if (explodeOnContact && !IsBusy())
		{
			SetMotionEnabled(wantsMotion: false);
			SetFlag(Flags.Busy, b: true, recursive: false, networkupdate: false);
			((FacepunchBehaviour)this).Invoke((Action)Explode, 0.015f);
		}
		else
		{
			Profiler.BeginSample("DoBounceEffect");
			DoBounceEffect();
			Profiler.EndSample();
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
		if (entity is BaseVehicle && !(entity is Tugboat))
		{
			return false;
		}
		return true;
	}

	private void DoBounceEffect()
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		ContactPoint contact = collision.GetContact(0);
		DoStick(((ContactPoint)(ref contact)).point, ((ContactPoint)(ref contact)).normal, ent, collision.collider);
	}

	public virtual void SetMotionEnabled(bool wantsMotion)
	{
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		Rigidbody component = ((Component)this).GetComponent<Rigidbody>();
		if (wantsMotion)
		{
			if ((Object)(object)component == (Object)null && hadRB)
			{
				component = ((Component)this).gameObject.AddComponent<Rigidbody>();
				component.mass = rbMass;
				component.drag = rbDrag;
				component.angularDrag = rbAngularDrag;
				component.collisionDetectionMode = rbCollisionMode;
				component.useGravity = true;
				component.isKinematic = false;
			}
		}
		else if ((Object)(object)component != (Object)null)
		{
			hadRB = true;
			rbMass = component.mass;
			rbDrag = component.drag;
			rbAngularDrag = component.angularDrag;
			rbCollisionMode = component.collisionDetectionMode;
			Object.Destroy((Object)(object)component);
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
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		base.PostServerLoad();
		if (parentEntity.IsValid(serverside: true))
		{
			DoStick(((Component)this).transform.position, ((Component)this).transform.forward, parentEntity.Get(serverside: true), null);
		}
	}

	public override void Load(LoadInfo info)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
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
