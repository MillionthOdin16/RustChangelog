using UnityEngine;

public class SocketMod_AreaCheck : SocketMod
{
	public Bounds bounds = new Bounds(Vector3.zero, Vector3.one * 0.1f);

	public LayerMask layerMask;

	public bool wantsInside = true;

	private void OnDrawGizmosSelected()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		Gizmos.matrix = ((Component)this).transform.localToWorldMatrix;
		bool flag = true;
		if (!wantsInside)
		{
			flag = !flag;
		}
		Gizmos.color = (flag ? ColorEx.WithAlpha(Color.green, 0.5f) : ColorEx.WithAlpha(Color.red, 0.5f));
		Gizmos.DrawCube(((Bounds)(ref bounds)).center, ((Bounds)(ref bounds)).size);
	}

	public static bool IsInArea(Vector3 position, Quaternion rotation, Bounds bounds, LayerMask layerMask, BaseEntity entity = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		return GamePhysics.CheckOBBAndEntity(new OBB(position, rotation, bounds), ((LayerMask)(ref layerMask)).value, (QueryTriggerInteraction)0, entity);
	}

	public bool DoCheck(Vector3 position, Quaternion rotation, BaseEntity entity = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		Vector3 position2 = position + rotation * worldPosition;
		Quaternion rotation2 = rotation * worldRotation;
		return IsInArea(position2, rotation2, bounds, layerMask, entity) == wantsInside;
	}

	public override bool DoCheck(Construction.Placement place)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		bool flag = DoCheck(place.position, place.rotation);
		if (!flag)
		{
			Construction.lastPlacementError = "Failed Check: IsInArea (" + hierachyName + ")";
		}
		else if (wantsInside && (LayerMask.op_Implicit(layerMask) & 0x8000000) == 0)
		{
			flag = !GamePhysics.CheckSphere(place.position, 5f, 134217728, (QueryTriggerInteraction)0);
			if (!flag)
			{
				Construction.lastPlacementError = "Failed Check: IsInArea (" + hierachyName + ") Vehicle_Large test";
			}
		}
		if (flag)
		{
			return true;
		}
		return false;
	}
}
