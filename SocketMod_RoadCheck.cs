using System.Collections.Generic;
using Facepunch;
using UnityEngine;

public class SocketMod_RoadCheck : SocketMod
{
	public float sphereRadius = 1f;

	public bool wantsCollide;

	public LayerMask layerMask = LayerMask.op_Implicit(65536);

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
		Vector3 position = place.position + place.rotation * worldPosition;
		List<Collider> list = Pool.GetList<Collider>();
		GamePhysics.OverlapSphere(position, sphereRadius, list, ((LayerMask)(ref layerMask)).value, (QueryTriggerInteraction)2);
		bool flag = false;
		for (int i = 0; i < list.Count; i++)
		{
			Collider val = list[i];
			if ((Object)(object)val != (Object)null && ((Component)val).gameObject.HasCustomTag(GameObjectTag.Road))
			{
				flag = true;
				break;
			}
		}
		bool num = wantsCollide == flag;
		Pool.FreeList<Collider>(ref list);
		if (!num)
		{
			bool flag2 = false;
			if (wantsCollide)
			{
				Construction.lastPlacementError = ConstructionErrors.MustPlaceOnRoad.translated;
				if (flag2)
				{
					Construction.lastPlacementError = Construction.lastPlacementError + " (" + hierachyName + ")";
					return num;
				}
			}
			else
			{
				Construction.lastPlacementError = ConstructionErrors.CantPlaceOnRoad.translated;
				if (flag2)
				{
					Construction.lastPlacementError = Construction.lastPlacementError + " (" + hierachyName + ")";
				}
			}
		}
		return num;
	}
}
