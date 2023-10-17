using System.Collections.Generic;
using Facepunch;
using UnityEngine;

public class SocketMod_BuildingBlock : SocketMod
{
	public float sphereRadius = 1f;

	public LayerMask layerMask;

	public QueryTriggerInteraction queryTriggers;

	public bool wantsCollide;

	private void OnDrawGizmosSelected()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		Gizmos.matrix = ((Component)this).transform.localToWorldMatrix;
		Gizmos.color = (wantsCollide ? new Color(0f, 1f, 0f, 0.7f) : new Color(1f, 0f, 0f, 0.7f));
		Gizmos.DrawSphere(Vector3.zero, sphereRadius);
	}

	public override bool DoCheck(Construction.Placement place)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		Vector3 position = place.position + place.rotation * worldPosition;
		List<BuildingBlock> list = Pool.GetList<BuildingBlock>();
		Vis.Entities(position, sphereRadius, list, ((LayerMask)(ref layerMask)).value, queryTriggers);
		bool flag = list.Count > 0;
		if (flag && wantsCollide)
		{
			Pool.FreeList<BuildingBlock>(ref list);
			return true;
		}
		if (flag && !wantsCollide)
		{
			Pool.FreeList<BuildingBlock>(ref list);
			return false;
		}
		Pool.FreeList<BuildingBlock>(ref list);
		return !wantsCollide;
	}
}
