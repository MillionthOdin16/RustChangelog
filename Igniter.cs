using System;
using System.Collections.Generic;
using Facepunch;
using Rust;
using UnityEngine;

public class Igniter : IOEntity
{
	public float IgniteRange = 5f;

	public float IgniteFrequency = 1f;

	public float IgniteStartDelay;

	public Transform LineOfSightEyes;

	public float SelfDamagePerIgnite = 0.5f;

	public int PowerConsumption = 2;

	public override int ConsumptionAmount()
	{
		return PowerConsumption;
	}

	public override void UpdateHasPower(int inputAmount, int inputSlot)
	{
		base.UpdateHasPower(inputAmount, inputSlot);
		if (inputAmount > 0)
		{
			((FacepunchBehaviour)this).InvokeRepeating((Action)IgniteInRange, IgniteStartDelay, IgniteFrequency);
		}
		else if (((FacepunchBehaviour)this).IsInvoking((Action)IgniteInRange))
		{
			((FacepunchBehaviour)this).CancelInvoke((Action)IgniteInRange);
		}
	}

	private void IgniteInRange()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		List<BaseEntity> list = Pool.GetList<BaseEntity>();
		Vis.Entities(LineOfSightEyes.position, IgniteRange, list, 1237019409, (QueryTriggerInteraction)2);
		int num = 0;
		foreach (BaseEntity item in list)
		{
			if (item.HasFlag(Flags.On) || !item.IsVisible(LineOfSightEyes.position))
			{
				continue;
			}
			if (item.isServer && item is BaseOven)
			{
				(item as BaseOven).StartCooking();
				if (item.HasFlag(Flags.On))
				{
					num++;
				}
			}
			else if (item.isServer && item is IIgniteable igniteable && igniteable.CanIgnite())
			{
				igniteable.Ignite(((Component)this).transform.position);
				num++;
			}
		}
		Pool.FreeList<BaseEntity>(ref list);
		Hurt(SelfDamagePerIgnite, DamageType.ElectricShock, this, useProtection: false);
	}
}
