using UnityEngine;

public class SocketMod_PhysicMaterial : SocketMod
{
	public PhysicMaterial[] ValidMaterials;

	private PhysicMaterial foundMaterial = null;

	public override bool DoCheck(Construction.Placement place)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		Vector3 position = place.position;
		Vector3 eulerAngles = ((Quaternion)(ref place.rotation)).eulerAngles;
		Vector3 val = position + ((Vector3)(ref eulerAngles)).normalized * 0.5f;
		eulerAngles = ((Quaternion)(ref place.rotation)).eulerAngles;
		RaycastHit val2 = default(RaycastHit);
		if (Physics.Raycast(val, -((Vector3)(ref eulerAngles)).normalized, ref val2, 1f, 161546240, (QueryTriggerInteraction)1))
		{
			foundMaterial = ((RaycastHit)(ref val2)).collider.GetMaterialAt(((RaycastHit)(ref val2)).point);
			PhysicMaterial[] validMaterials = ValidMaterials;
			foreach (PhysicMaterial val3 in validMaterials)
			{
				if ((Object)(object)val3 == (Object)(object)foundMaterial)
				{
					return true;
				}
			}
		}
		return false;
	}
}
