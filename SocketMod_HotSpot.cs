using UnityEngine;

public class SocketMod_HotSpot : SocketMod
{
	public float spotSize = 0.1f;

	private void OnDrawGizmos()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		Gizmos.matrix = ((Component)this).transform.localToWorldMatrix;
		Gizmos.color = new Color(1f, 1f, 0f, 0.5f);
		Gizmos.DrawSphere(Vector3.zero, spotSize);
	}

	public override void ModifyPlacement(Construction.Placement place)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		Vector3 position = place.position + place.rotation * worldPosition;
		place.position = position;
	}
}
