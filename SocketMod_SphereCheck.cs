using UnityEngine;

public class SocketMod_SphereCheck : SocketMod
{
	public float sphereRadius = 1f;

	public LayerMask layerMask;

	public bool wantsCollide = false;

	public static Phrase Error_WantsCollideConstruction = new Phrase("error_wantsconstruction", "Must be placed on construction");

	public static Phrase Error_DoesNotWantCollideConstruction = new Phrase("error_doesnotwantconstruction", "Cannot be placed on construction");

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
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		Vector3 position = place.position + place.rotation * worldPosition;
		bool flag = wantsCollide == GamePhysics.CheckSphere(position, sphereRadius, ((LayerMask)(ref layerMask)).value, (QueryTriggerInteraction)0);
		if (!flag)
		{
			bool flag2 = false;
			Construction.lastPlacementError = "Failed Check: Sphere Test (" + hierachyName + ")";
			if (LayerMask.op_Implicit(layerMask) == 2097152 && wantsCollide)
			{
				Construction.lastPlacementError = Error_WantsCollideConstruction.translated;
				if (flag2)
				{
					Construction.lastPlacementError = Construction.lastPlacementError + " (" + hierachyName + ")";
				}
			}
			else if (!wantsCollide && (LayerMask.op_Implicit(layerMask) & 0x200000) == 2097152)
			{
				Construction.lastPlacementError = Error_DoesNotWantCollideConstruction.translated;
				if (flag2)
				{
					Construction.lastPlacementError = Construction.lastPlacementError + " (" + hierachyName + ")";
				}
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
