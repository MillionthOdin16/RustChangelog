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

	public override void ServerProjectileHit(HitInfo info)
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		if (effect.isValid)
		{
			Effect.server.Run(effect.resourcePath, info.HitPositionWorld, info.HitNormalWorld);
		}
		List<BaseCombatEntity> list = Pool.GetList<BaseCombatEntity>();
		List<BaseCombatEntity> list2 = Pool.GetList<BaseCombatEntity>();
		Vis.Entities(info.HitPositionWorld, radius, list2, 1237003025, (QueryTriggerInteraction)2);
		foreach (BaseCombatEntity item in list2)
		{
			if (!item.isServer || list.Contains(item) || ((Object)(object)item == (Object)(object)info.HitEntity && ignoreHitObject))
			{
				continue;
			}
			Vector3 val = item.CenterPoint();
			Vector3 val2 = item.ClosestPoint(info.HitPositionWorld);
			float num = Vector3.Distance(val2, info.HitPositionWorld);
			float num2 = num / radius;
			if (num2 > 1f)
			{
				continue;
			}
			float num3 = 1f - num2;
			if (item.IsVisibleAndCanSee(info.HitPositionWorld - ((Vector3)(ref info.ProjectileVelocity)).normalized * 0.1f))
			{
				Vector3 hitPositionWorld = info.HitPositionWorld;
				Vector3 val3 = val2 - info.HitPositionWorld;
				if (item.IsVisibleAndCanSee(hitPositionWorld - ((Vector3)(ref val3)).normalized * 0.1f))
				{
					list.Add(item);
					item.OnAttacked(new HitInfo(info.Initiator, item, damage.type, damage.amount * num3));
				}
			}
		}
		Pool.FreeList<BaseCombatEntity>(ref list);
		Pool.FreeList<BaseCombatEntity>(ref list2);
	}
}
