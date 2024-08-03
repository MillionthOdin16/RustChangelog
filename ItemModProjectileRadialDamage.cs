using System.Collections.Generic;
using Facepunch;
using Rust;
using UnityEngine;

public class ItemModProjectileRadialDamage : ItemModProjectileMod
{
	public float radius = 0.5f;

	public DamageTypeEntry damage;

	public GameObjectRef effect;

	public bool ignoreHitObject = true;

	public int vibrationLevel = 2;

	public override void ServerProjectileHit(HitInfo info)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		if (effect.isValid)
		{
			Effect.server.Run(effect.resourcePath, info.HitPositionWorld, info.HitNormalWorld);
		}
		List<BaseCombatEntity> list = Pool.GetList<BaseCombatEntity>();
		List<BaseCombatEntity> list2 = Pool.GetList<BaseCombatEntity>();
		Vis.Entities(info.HitPositionWorld, radius, list2, 1237003025, (QueryTriggerInteraction)2);
		if (damage.type == DamageType.Explosion)
		{
			SeismicSensor.Notify(info.HitPositionWorld, vibrationLevel);
		}
		foreach (BaseCombatEntity item in list2)
		{
			if (!item.isServer || list.Contains(item) || ((Object)(object)item == (Object)(object)info.HitEntity && ignoreHitObject))
			{
				continue;
			}
			item.CenterPoint();
			Vector3 val = item.ClosestPoint(info.HitPositionWorld);
			float num = Vector3.Distance(val, info.HitPositionWorld) / radius;
			if (num > 1f)
			{
				continue;
			}
			float num2 = 1f - num;
			if (item.IsVisibleAndCanSeeLegacy(info.HitPositionWorld - ((Vector3)(ref info.ProjectileVelocity)).normalized * 0.1f))
			{
				Vector3 hitPositionWorld = info.HitPositionWorld;
				Vector3 val2 = val - info.HitPositionWorld;
				if (item.IsVisibleAndCanSeeLegacy(hitPositionWorld - ((Vector3)(ref val2)).normalized * 0.1f))
				{
					list.Add(item);
					item.OnAttacked(new HitInfo(info.Initiator, item, damage.type, damage.amount * num2));
				}
			}
		}
		Pool.FreeList<BaseCombatEntity>(ref list);
		Pool.FreeList<BaseCombatEntity>(ref list2);
	}
}
