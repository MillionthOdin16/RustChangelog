using System.Collections.Generic;
using Facepunch;
using UnityEngine;

public class GraveyardFence : SimpleBuildingBlock
{
	public BoxCollider[] pillars;

	public override void ServerInit()
	{
		base.ServerInit();
		UpdatePillars();
	}

	public override void DestroyShared()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		base.DestroyShared();
		List<GraveyardFence> list = Pool.GetList<GraveyardFence>();
		Vis.Entities(((Component)this).transform.position, 5f, list, 2097152, (QueryTriggerInteraction)2);
		foreach (GraveyardFence item in list)
		{
			item.UpdatePillars();
		}
		Pool.FreeList<GraveyardFence>(ref list);
	}

	public virtual void UpdatePillars()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		BoxCollider[] array = pillars;
		foreach (BoxCollider val in array)
		{
			((Component)val).gameObject.SetActive(true);
			Vector3 val2 = ((Component)val).transform.TransformPoint(val.center);
			Collider[] array2 = Physics.OverlapBox(val2, val.size * 0.5f, ((Component)val).transform.rotation, 2097152);
			Collider[] array3 = array2;
			foreach (Collider val3 in array3)
			{
				if (((Component)val3).CompareTag("Usable Auxiliary"))
				{
					BaseEntity baseEntity = ((Component)val3).gameObject.ToBaseEntity();
					if (!((Object)(object)baseEntity == (Object)null) && !EqualNetID((BaseNetworkable)baseEntity) && (Object)(object)val3 != (Object)(object)val)
					{
						((Component)val).gameObject.SetActive(false);
					}
				}
			}
		}
	}
}
