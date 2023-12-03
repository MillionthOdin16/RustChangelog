using System;
using System.Collections.Generic;
using Facepunch;
using Rust;
using UnityEngine;

public static class DamageUtil
{
	public static void RadiusDamage(BaseEntity attackingPlayer, BaseEntity weaponPrefab, Vector3 pos, float minradius, float radius, List<DamageTypeEntry> damage, int layers, bool useLineOfSight)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		TimeWarning val = TimeWarning.New("DamageUtil.RadiusDamage", 0);
		try
		{
			List<HitInfo> list = Pool.GetList<HitInfo>();
			List<BaseEntity> list2 = Pool.GetList<BaseEntity>();
			List<BaseEntity> list3 = Pool.GetList<BaseEntity>();
			Vis.Entities(pos, radius, list3, layers, (QueryTriggerInteraction)2);
			for (int i = 0; i < list3.Count; i++)
			{
				BaseEntity baseEntity = list3[i];
				if (!baseEntity.isServer || list2.Contains(baseEntity))
				{
					continue;
				}
				Vector3 val2 = baseEntity.ClosestPoint(pos);
				float num = Vector3.Distance(val2, pos);
				float num2 = Mathf.Clamp01((num - minradius) / (radius - minradius));
				if (!(num2 > 1f))
				{
					float amount = 1f - num2;
					if (!useLineOfSight || baseEntity.IsVisible(pos))
					{
						HitInfo hitInfo = new HitInfo();
						hitInfo.Initiator = attackingPlayer;
						hitInfo.WeaponPrefab = weaponPrefab;
						hitInfo.damageTypes.Add(damage);
						hitInfo.damageTypes.ScaleAll(amount);
						hitInfo.HitPositionWorld = val2;
						Vector3 val3 = pos - val2;
						hitInfo.HitNormalWorld = ((Vector3)(ref val3)).normalized;
						hitInfo.PointStart = pos;
						hitInfo.PointEnd = hitInfo.HitPositionWorld;
						list.Add(hitInfo);
						list2.Add(baseEntity);
					}
				}
			}
			for (int j = 0; j < list2.Count; j++)
			{
				BaseEntity baseEntity2 = list2[j];
				HitInfo info = list[j];
				baseEntity2.OnAttacked(info);
			}
			Pool.FreeList<HitInfo>(ref list);
			Pool.FreeList<BaseEntity>(ref list2);
			Pool.FreeList<BaseEntity>(ref list3);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}
}
