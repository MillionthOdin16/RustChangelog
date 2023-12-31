using System;
using System.Linq;
using Rust;
using UnityEngine;

public class TriggerHurt : TriggerBase, IServerComponent, IHurtTrigger
{
	public float DamagePerSecond = 1f;

	public float DamageTickRate = 4f;

	public DamageType damageType;

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
		return ((Component)baseEntity).gameObject;
	}

	internal override void OnObjects()
	{
		((FacepunchBehaviour)this).InvokeRepeating((Action)OnTick, 0f, 1f / DamageTickRate);
	}

	internal override void OnEmpty()
	{
		((FacepunchBehaviour)this).CancelInvoke((Action)OnTick);
	}

	private void OnTick()
	{
		BaseEntity attacker = ((Component)this).gameObject.ToBaseEntity();
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
				if (!((Object)(object)baseCombatEntity == (Object)null) && CanHurt(baseCombatEntity))
				{
					baseCombatEntity.Hurt(DamagePerSecond * (1f / DamageTickRate), damageType, attacker);
				}
			}
		}
	}

	protected virtual bool CanHurt(BaseCombatEntity ent)
	{
		return true;
	}
}
