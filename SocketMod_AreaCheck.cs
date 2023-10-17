using UnityEngine;

public class SocketMod_AreaCheck : SocketMod
{
	public Bounds bounds = new Bounds(Vector3.zero, Vector3.one * 0.1f);

	public LayerMask layerMask;

	public bool wantsInside = true;

	private void OnDrawGizmosSelected()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		return GamePhysics.CheckOBBAndEntity(new OBB(position, rotation, bounds), ((LayerMask)(ref layerMask)).value, (QueryTriggerInteraction)0, entity);
	}

	public bool DoCheck(Vector3 position, Quaternion rotation, BaseEntity entity = null)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		Vector3 position2 = position + rotation * worldPosition;
		Quaternion rotation2 = rotation * worldRotation;
		return IsInArea(position2, rotation2, bounds, layerMask, entity) == wantsInside;
	}

	public override bool DoCheck(Construction.Placement place)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		if (DoCheck(place.position, place.rotation))
		{
			return true;
		}
		Construction.lastPlacementError = "Failed Check: IsInArea (" + hierachyName + ")";
		return false;
	}
}
