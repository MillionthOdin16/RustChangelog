using UnityEngine;

public class SocketMod_AngleCheck : SocketMod
{
	public bool wantsAngle = true;

	public Vector3 worldNormal = Vector3.up;

	public float withinDegrees = 45f;

	public static Phrase ErrorPhrase = new Phrase("error_anglecheck", "Invalid angle");

	private void OnDrawGizmosSelected()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		Gizmos.matrix = ((Component)this).transform.localToWorldMatrix;
		Gizmos.color = Color.yellow;
		Gizmos.DrawFrustum(Vector3.zero, withinDegrees, 1f, 0f, 1f);
	}

	public override bool DoCheck(Construction.Placement place)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		float num = Vector3Ex.DotDegrees(worldNormal, place.rotation * Vector3.up);
		if (num < withinDegrees)
		{
			return true;
		}
		Construction.lastPlacementError = ErrorPhrase.translated;
		return false;
	}
}
