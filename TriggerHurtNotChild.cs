using System;
using System.Collections.Generic;
using Facepunch;
using Rust;
using UnityEngine;

public class TriggerHurtNotChild : TriggerBase, IServerComponent, IHurtTrigger
{
	public interface IHurtTriggerUser
	{
		BasePlayer GetPlayerDamageInitiator();

		float GetDamageMultiplier(BaseEntity ent);

		void OnHurtTriggerOccupant(BaseEntity hurtEntity, DamageType damageType, float damageTotal);
	}

	public float DamagePerSecond = 1f;

	public float DamageTickRate = 4f;

	public float DamageDelay = 0f;

	public DamageType damageType = DamageType.Generic;

	public bool ignoreNPC = true;

	public float npcMultiplier = 1f;

	public float resourceMultiplier = 1f;

	public bool triggerHitImpacts = true;

	public bool RequireUpAxis = false;

	public BaseEntity SourceEntity;

	public bool UseSourceEntityDamageMultiplier = true;

	public bool ignoreAllVehicleMounted = false;

	public float activationDelay;

	private Dictionary<BaseEntity, float> entryTimes;

	private TimeSince timeSinceAcivation = default(TimeSince);

	private IHurtTriggerUser hurtTiggerUser;

	internal override GameObject InterestedInObject(GameObject obj)
	{
		obj = base.InterestedInObject(obj);
		if ((Object)(object)obj == (Object)null)
		{
			return null;
		}
		BaseEntity baseEntity = obj.ToBaseEntity();
		if ((Object)(object)baseEntity == (Object)null)
		{
			return null;
		}
		if (baseEntity.isClient)
		{
			return null;
		}
		if (ignoreNPC && baseEntity.IsNpc)
		{
			return null;
		}
		return ((Component)baseEntity).gameObject;
	}

	internal override void OnObjects()
	{
		((FacepunchBehaviour)this).InvokeRepeating((Action)OnTick, 0f, 1f / DamageTickRate);
	}

	internal override void OnEntityEnter(BaseEntity ent)
	{
		base.OnEntityEnter(ent);
		if ((Object)(object)ent != (Object)null && DamageDelay > 0f)
		{
			if (entryTimes == null)
			{
				entryTimes = new Dictionary<BaseEntity, float>();
			}
			entryTimes.Add(ent, Time.time);
		}
	}

	internal override void OnEntityLeave(BaseEntity ent)
	{
		if ((Object)(object)ent != (Object)null && entryTimes != null)
		{
			entryTimes.Remove(ent);
		}
		base.OnEntityLeave(ent);
	}

	internal override void OnEmpty()
	{
		((FacepunchBehaviour)this).CancelInvoke((Action)OnTick);
	}

	protected void OnEnable()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		timeSinceAcivation = TimeSince.op_Implicit(0f);
		hurtTiggerUser = SourceEntity as IHurtTriggerUser;
	}

	public new void OnDisable()
	{
		((FacepunchBehaviour)this).CancelInvoke((Action)OnTick);
		base.OnDisable();
	}

	private bool IsInterested(BaseEntity ent)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		if (TimeSince.op_Implicit(timeSinceAcivation) < activationDelay)
		{
			return false;
		}
		BasePlayer basePlayer = ent.ToPlayer();
		if ((Object)(object)basePlayer != (Object)null)
		{
			if (basePlayer.isMounted)
			{
				BaseVehicle mountedVehicle = basePlayer.GetMountedVehicle();
				if ((Object)(object)SourceEntity != (Object)null && (Object)(object)mountedVehicle == (Object)(object)SourceEntity)
				{
					return false;
				}
				if (ignoreAllVehicleMounted && (Object)(object)mountedVehicle != (Object)null)
				{
					return false;
				}
			}
			if ((Object)(object)SourceEntity != (Object)null && basePlayer.HasEntityInParents(SourceEntity))
			{
				return false;
			}
		}
		return true;
	}

	private void OnTick()
	{
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		if (entityContents.IsNullOrEmpty())
		{
			return;
		}
		List<BaseEntity> list = Pool.GetList<BaseEntity>();
		list.AddRange(entityContents);
		foreach (BaseEntity item in list)
		{
			if (item.IsValid() && IsInterested(item) && (!(DamageDelay > 0f) || entryTimes == null || !entryTimes.TryGetValue(item, out var value) || !(value + DamageDelay > Time.time)) && (!RequireUpAxis || !(Vector3.Dot(((Component)item).transform.up, ((Component)this).transform.up) < 0f)))
			{
				float num = DamagePerSecond * 1f / DamageTickRate;
				if (UseSourceEntityDamageMultiplier && hurtTiggerUser != null)
				{
					num *= hurtTiggerUser.GetDamageMultiplier(item);
				}
				if (item.IsNpc)
				{
					num *= npcMultiplier;
				}
				if (item is ResourceEntity)
				{
					num *= resourceMultiplier;
				}
				Vector3 val = ((Component)item).transform.position + Vector3.up * 1f;
				bool flag = item is BasePlayer || item is BaseNpc;
				BaseEntity baseEntity = null;
				BaseEntity weaponPrefab = null;
				if (hurtTiggerUser != null)
				{
					baseEntity = hurtTiggerUser.GetPlayerDamageInitiator();
					weaponPrefab = SourceEntity.LookupPrefab();
				}
				if ((Object)(object)baseEntity == (Object)null)
				{
					baseEntity = ((!((Object)(object)SourceEntity != (Object)null)) ? ((Component)this).gameObject.ToBaseEntity() : SourceEntity);
				}
				HitInfo hitInfo = new HitInfo
				{
					DoHitEffects = true,
					HitEntity = item,
					HitPositionWorld = val,
					HitPositionLocal = ((Component)item).transform.InverseTransformPoint(val),
					HitNormalWorld = Vector3.up,
					HitMaterial = (flag ? StringPool.Get("Flesh") : 0u),
					WeaponPrefab = weaponPrefab,
					Initiator = baseEntity
				};
				hitInfo.damageTypes = new DamageTypeList();
				hitInfo.damageTypes.Set(damageType, num);
				item.OnAttacked(hitInfo);
				if (hurtTiggerUser != null)
				{
					hurtTiggerUser.OnHurtTriggerOccupant(item, damageType, num);
				}
				if (triggerHitImpacts)
				{
					Effect.server.ImpactEffect(hitInfo);
				}
			}
		}
		Pool.FreeList<BaseEntity>(ref list);
		RemoveInvalidEntities();
	}
}
