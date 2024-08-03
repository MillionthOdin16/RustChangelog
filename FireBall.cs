using System;
using System.Collections.Generic;
using Facepunch;
using Rust;
using UnityEngine;
using UnityEngine.Profiling;

public class FireBall : BaseEntity, ISplashable
{
	public float lifeTimeMin = 20f;

	public float lifeTimeMax = 40f;

	public ParticleSystem[] movementSystems;

	public ParticleSystem[] restingSystems;

	[NonSerialized]
	public float generation = 0f;

	public GameObjectRef spreadSubEntity;

	public float tickRate = 0.5f;

	public float damagePerSecond = 2f;

	public float radius = 0.5f;

	public int waterToExtinguish = 200;

	public bool canMerge = false;

	public LayerMask AttackLayers = LayerMask.op_Implicit(1220225809);

	public bool ignoreNPC = false;

	private Vector3 lastPos = Vector3.zero;

	private float deathTime = 0f;

	private int wetness = 0;

	private float spawnTime = 0f;

	private Vector3 delayedVelocity;

	public void SetDelayedVelocity(Vector3 delayed)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if (!(delayedVelocity != Vector3.zero))
		{
			delayedVelocity = delayed;
			((FacepunchBehaviour)this).Invoke((Action)ApplyDelayedVelocity, 0.1f);
		}
	}

	private void ApplyDelayedVelocity()
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		SetVelocity(delayedVelocity);
		delayedVelocity = Vector3.zero;
	}

	public override void ServerInit()
	{
		base.ServerInit();
		((FacepunchBehaviour)this).InvokeRepeating((Action)Think, Random.Range(0f, 1f), tickRate);
		float num = Random.Range(lifeTimeMin, lifeTimeMax);
		float num2 = num * Random.Range(0.9f, 1.1f);
		((FacepunchBehaviour)this).Invoke((Action)Extinguish, num2);
		((FacepunchBehaviour)this).Invoke((Action)TryToSpread, num * Random.Range(0.3f, 0.5f));
		deathTime = Time.realtimeSinceStartup + num2;
		spawnTime = Time.realtimeSinceStartup;
	}

	public float GetDeathTime()
	{
		return deathTime;
	}

	public void AddLife(float amountToAdd)
	{
		float num = Mathf.Clamp(GetDeathTime() + amountToAdd, 0f, MaxLifeTime());
		((FacepunchBehaviour)this).Invoke((Action)Extinguish, num);
		deathTime = num;
	}

	public float MaxLifeTime()
	{
		return lifeTimeMax * 2.5f;
	}

	public float TimeLeft()
	{
		float num = deathTime - Time.realtimeSinceStartup;
		if (num < 0f)
		{
			num = 0f;
		}
		return num;
	}

	public void TryToSpread()
	{
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		float num = 0.9f - generation * 0.1f;
		if (Random.Range(0f, 1f) < num && spreadSubEntity.isValid)
		{
			BaseEntity baseEntity = GameManager.server.CreateEntity(spreadSubEntity.resourcePath);
			if (Object.op_Implicit((Object)(object)baseEntity))
			{
				((Component)baseEntity).transform.position = ((Component)this).transform.position + Vector3.up * 0.25f;
				baseEntity.Spawn();
				float aimCone = 45f;
				Vector3 modifiedAimConeDirection = AimConeUtil.GetModifiedAimConeDirection(aimCone, Vector3.up);
				baseEntity.creatorEntity = (((Object)(object)creatorEntity == (Object)null) ? baseEntity : creatorEntity);
				baseEntity.SetVelocity(modifiedAimConeDirection * Random.Range(5f, 8f));
				((Component)baseEntity).SendMessage("SetGeneration", (object)(generation + 1f));
			}
		}
	}

	public void SetGeneration(int gen)
	{
		generation = gen;
	}

	public void Think()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		if (base.isServer)
		{
			Profiler.BeginSample("FireThink");
			SetResting(Vector3.Distance(lastPos, ((Component)this).transform.localPosition) < 0.25f);
			lastPos = ((Component)this).transform.localPosition;
			if (IsResting())
			{
				DoRadialDamage();
			}
			if (WaterFactor() > 0.5f)
			{
				Extinguish();
			}
			if (wetness > waterToExtinguish)
			{
				Extinguish();
			}
			Profiler.EndSample();
		}
	}

	public void DoRadialDamage()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		List<Collider> list = Pool.GetList<Collider>();
		Vector3 position = ((Component)this).transform.position + new Vector3(0f, radius * 0.75f, 0f);
		Vis.Colliders<Collider>(position, radius, list, LayerMask.op_Implicit(AttackLayers), (QueryTriggerInteraction)2);
		HitInfo hitInfo = new HitInfo();
		hitInfo.DoHitEffects = true;
		hitInfo.DidHit = true;
		hitInfo.HitBone = 0u;
		hitInfo.Initiator = (((Object)(object)creatorEntity == (Object)null) ? ((Component)this).gameObject.ToBaseEntity() : creatorEntity);
		hitInfo.PointStart = ((Component)this).transform.position;
		foreach (Collider item in list)
		{
			if (item.isTrigger && (((Component)item).gameObject.layer == 29 || ((Component)item).gameObject.layer == 18))
			{
				continue;
			}
			BaseCombatEntity baseCombatEntity = ((Component)item).gameObject.ToBaseEntity() as BaseCombatEntity;
			if (!((Object)(object)baseCombatEntity == (Object)null) && baseCombatEntity.isServer && baseCombatEntity.IsAlive() && (!ignoreNPC || !baseCombatEntity.IsNpc) && baseCombatEntity.IsVisible(position))
			{
				if (baseCombatEntity is BasePlayer)
				{
					Effect.server.Run("assets/bundled/prefabs/fx/impacts/additive/fire.prefab", baseCombatEntity, 0u, new Vector3(0f, 1f, 0f), Vector3.up);
				}
				hitInfo.PointEnd = ((Component)baseCombatEntity).transform.position;
				hitInfo.HitPositionWorld = ((Component)baseCombatEntity).transform.position;
				hitInfo.damageTypes.Set(DamageType.Heat, damagePerSecond * tickRate);
				baseCombatEntity.OnAttacked(hitInfo);
			}
		}
		Pool.FreeList<Collider>(ref list);
	}

	public bool CanMerge()
	{
		return canMerge && TimeLeft() < MaxLifeTime() * 0.8f;
	}

	public float TimeAlive()
	{
		return Time.realtimeSinceStartup - spawnTime;
	}

	public void SetResting(bool isResting)
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		if (isResting != IsResting() && isResting && TimeAlive() > 1f && CanMerge())
		{
			List<Collider> list = Pool.GetList<Collider>();
			Vis.Colliders<Collider>(((Component)this).transform.position, 0.5f, list, 512, (QueryTriggerInteraction)2);
			foreach (Collider item in list)
			{
				BaseEntity baseEntity = ((Component)item).gameObject.ToBaseEntity();
				if (Object.op_Implicit((Object)(object)baseEntity))
				{
					FireBall fireBall = baseEntity.ToServer<FireBall>();
					if (Object.op_Implicit((Object)(object)fireBall) && fireBall.CanMerge() && (Object)(object)fireBall != (Object)(object)this)
					{
						((FacepunchBehaviour)fireBall).Invoke((Action)Extinguish, 1f);
						fireBall.canMerge = false;
						AddLife(fireBall.TimeLeft() * 0.25f);
					}
				}
			}
			Pool.FreeList<Collider>(ref list);
		}
		SetFlag(Flags.OnFire, isResting);
	}

	public void Extinguish()
	{
		((FacepunchBehaviour)this).CancelInvoke((Action)Extinguish);
		if (!base.IsDestroyed)
		{
			Kill();
		}
	}

	public bool WantsSplash(ItemDefinition splashType, int amount)
	{
		return !base.IsDestroyed;
	}

	public int DoSplash(ItemDefinition splashType, int amount)
	{
		wetness += amount;
		return amount;
	}

	public bool IsResting()
	{
		return HasFlag(Flags.OnFire);
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
	}
}
