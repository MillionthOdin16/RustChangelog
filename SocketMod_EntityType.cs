using System.Collections.Generic;
using Facepunch;
using UnityEngine;

public class SocketMod_EntityType : SocketMod
{
	public float sphereRadius = 1f;

	public LayerMask layerMask;

	public QueryTriggerInteraction queryTriggers;

	public BaseEntity searchType;

	public bool wantsCollide = false;

	private void OnDrawGizmosSelected()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		Gizmos.matrix = ((Component)this).transform.localToWorldMatrix;
		Gizmos.color = (wantsCollide ? new Color(0f, 1f, 0f, 0.7f) : new Color(1f, 0f, 0f, 0.7f));
		Gizmos.DrawSphere(Vector3.zero, sphereRadius);
	}

	public override bool DoCheck(Construction.Placement place)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		Vector3 position = place.position + place.rotation * worldPosition;
		List<BaseEntity> list = Pool.GetList<BaseEntity>();
		Vis.Entities(position, sphereRadius, list, ((LayerMask)(ref layerMask)).value, queryTriggers);
		foreach (BaseEntity item in list)
		{
			bool flag = ((object)item).GetType().IsAssignableFrom(((object)searchType).GetType());
			if (flag && wantsCollide)
			{
				Pool.FreeList<BaseEntity>(ref list);
				return true;
			}
			if (flag && !wantsCollide)
			{
				Pool.FreeList<BaseEntity>(ref list);
				return false;
			}
		}
		Pool.FreeList<BaseEntity>(ref list);
		return !wantsCollide;
	}
}
