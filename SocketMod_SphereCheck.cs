using System.Collections.Generic;
using Facepunch;
using UnityEngine;

public class SocketMod_SphereCheck : SocketMod
{
	public float sphereRadius = 1f;

	public LayerMask layerMask;

	public bool wantsCollide;

	public bool requireMonument;

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
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		Vector3 position = place.position + place.rotation * worldPosition;
		List<Collider> list = Pool.GetList<Collider>();
		GamePhysics.OverlapSphere(position, sphereRadius, list, ((LayerMask)(ref layerMask)).value, (QueryTriggerInteraction)2);
		if (requireMonument)
		{
			for (int i = 0; i < list.Count; i++)
			{
				Collider val = list[i];
				if (!((Component)val).gameObject.HasCustomTag(GameObjectTag.BlockBarricadePlacement) && ((Object)(object)val.GetMonument() == (Object)null || ((Component)val).gameObject.HasCustomTag(GameObjectTag.AllowBarricadePlacement)))
				{
					list.RemoveAt(i);
					i--;
				}
			}
		}
		bool flag = wantsCollide == list.Count > 0;
		Pool.FreeList<Collider>(ref list);
		if (!flag)
		{
			bool flag2 = false;
			Construction.lastPlacementError = "Failed Check: Sphere Test (" + hierachyName + ")";
			if (LayerMask.op_Implicit(layerMask) == 2097152 && wantsCollide)
			{
				Construction.placementError = ConstructionErrors.MustPlaceOnConstruction;
				if (flag2)
				{
					Construction.lastPlacementError = Construction.lastPlacementError + " (" + hierachyName + ")";
				}
			}
			else if (!wantsCollide && (LayerMask.op_Implicit(layerMask) & 0x200000) == 2097152)
			{
				Construction.placementError = ConstructionErrors.CantPlaceOnConstruction;
				if (flag2)
				{
					Construction.lastPlacementError = Construction.lastPlacementError + " (" + hierachyName + ")";
				}
			}
			else if (!wantsCollide && requireMonument)
			{
				Construction.placementError = ConstructionErrors.CantPlaceOnMonument;
			}
			else
			{
				Construction.lastPlacementError = "Failed Check: Sphere Test (" + hierachyName + ")";
			}
		}
		else if (wantsCollide && (LayerMask.op_Implicit(layerMask) & 0x8000000) == 0)
		{
			flag = !GamePhysics.CheckSphere(place.position, 5f, 134217728, (QueryTriggerInteraction)0);
			if (!flag)
			{
				Construction.lastPlacementError = "Failed Check: Sphere Test (" + hierachyName + ") Vehicle_Large test";
			}
		}
		if (flag)
		{
			return true;
		}
		return false;
	}
}
