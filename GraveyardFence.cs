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
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		BoxCollider[] array = pillars;
		foreach (BoxCollider val in array)
		{
			((Component)val).gameObject.SetActive(true);
			Collider[] array2 = Physics.OverlapBox(((Component)val).transform.TransformPoint(val.center), val.size * 0.5f, ((Component)val).transform.rotation, 2097152);
			foreach (Collider val2 in array2)
			{
				if (((Component)val2).CompareTag("Usable Auxiliary"))
				{
					BaseEntity baseEntity = ((Component)val2).gameObject.ToBaseEntity();
					if (!((Object)(object)baseEntity == (Object)null) && !EqualNetID((BaseNetworkable)baseEntity) && (Object)(object)val2 != (Object)(object)val)
					{
						((Component)val).gameObject.SetActive(false);
					}
				}
			}
		}
	}
}
