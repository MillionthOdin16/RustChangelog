using System.Collections.Generic;
using ConVar;
using Facepunch;
using UnityEngine;

public class GrowableHeatSource : EntityComponent<BaseEntity>, IServerComponent
{
	public float heatAmount = 5f;

	public float ApplyHeat(Vector3 forPosition)
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)base.baseEntity == (Object)null)
		{
			return 0f;
		}
		if (base.baseEntity.IsOn() || (base.baseEntity is IOEntity iOEntity && iOEntity.IsPowered()))
		{
			float num = Vector3.Distance(forPosition, ((Component)this).transform.position);
			return Mathx.RemapValClamped(num, 0f, Server.artificialTemperatureGrowableRange, 0f, heatAmount);
		}
		return 0f;
	}

	public void ForceUpdateGrowablesInRange()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		List<GrowableEntity> list = Pool.GetList<GrowableEntity>();
		Vis.Entities(((Component)this).transform.position, Server.artificialTemperatureGrowableRange, list, 524288, (QueryTriggerInteraction)2);
		List<PlanterBox> list2 = Pool.GetList<PlanterBox>();
		foreach (GrowableEntity item in list)
		{
			if (item.isServer)
			{
				PlanterBox planter = item.GetPlanter();
				if ((Object)(object)planter != (Object)null && !list2.Contains(planter))
				{
					list2.Add(planter);
					planter.ForceTemperatureUpdate();
				}
				item.CalculateQualities(firstTime: false, forceArtificialLightUpdates: false, forceArtificialTemperatureUpdates: true);
				item.SendNetworkUpdate();
			}
		}
		Pool.FreeList<PlanterBox>(ref list2);
		Pool.FreeList<GrowableEntity>(ref list);
	}
}
