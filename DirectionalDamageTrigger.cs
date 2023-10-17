using System;
using System.Collections.Generic;
using System.Linq;
using Rust;
using UnityEngine;

public class DirectionalDamageTrigger : TriggerBase
{
	public float repeatRate = 1f;

	public List<DamageTypeEntry> damageType;

	public GameObjectRef attackEffect;

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
		if (!(baseEntity is BaseCombatEntity))
		{
			return null;
		}
		if (baseEntity.isClient)
		{
			return null;
		}
		return ((Component)baseEntity).gameObject;
	}

	internal override void OnObjects()
	{
		((FacepunchBehaviour)this).InvokeRepeating((Action)OnTick, repeatRate, repeatRate);
	}

	internal override void OnEmpty()
	{
		((FacepunchBehaviour)this).CancelInvoke((Action)OnTick);
	}

	private void OnTick()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		if (attackEffect.isValid)
		{
			Effect.server.Run(attackEffect.resourcePath, ((Component)this).transform.position, Vector3.up);
		}
		if (entityContents == null)
		{
			return;
		}
		BaseEntity[] array = entityContents.ToArray();
		foreach (BaseEntity baseEntity in array)
		{
			if (baseEntity.IsValid())
			{
				BaseCombatEntity baseCombatEntity = baseEntity as BaseCombatEntity;
				if (!((Object)(object)baseCombatEntity == (Object)null))
				{
					HitInfo hitInfo = new HitInfo();
					hitInfo.damageTypes.Add(damageType);
					hitInfo.DoHitEffects = true;
					hitInfo.DidHit = true;
					hitInfo.PointStart = ((Component)this).transform.position;
					hitInfo.PointEnd = ((Component)baseCombatEntity).transform.position;
					baseCombatEntity.Hurt(hitInfo);
				}
			}
		}
	}
}
