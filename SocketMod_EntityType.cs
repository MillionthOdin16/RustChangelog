using System.Collections.Generic;
using Facepunch;
using UnityEngine;

public class SocketMod_EntityType : SocketMod
{
	public float sphereRadius = 1f;

	public LayerMask layerMask;

	public QueryTriggerInteraction queryTriggers;

	public BaseEntity searchType;

	public bool wantsCollide;

	public static Phrase ErrorPhrase = new Phrase("error_entitytype", "Invalid entity type");

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
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		bool flag = !wantsCollide;
		Vector3 position = place.position + place.rotation * worldPosition;
		List<BaseEntity> list = Pool.GetList<BaseEntity>();
		Vis.Entities(position, sphereRadius, list, ((LayerMask)(ref layerMask)).value, queryTriggers);
		foreach (BaseEntity item in list)
		{
			bool flag2 = ((object)item).GetType().IsAssignableFrom(((object)searchType).GetType());
			if (flag2 && wantsCollide)
			{
				flag = true;
				break;
			}
			if (flag2 && !wantsCollide)
			{
				flag = false;
				break;
			}
		}
		if (!flag)
		{
			Construction.lastPlacementError = ErrorPhrase.translated;
		}
		Pool.FreeList<BaseEntity>(ref list);
		return flag;
	}
}
