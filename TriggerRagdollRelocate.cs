using UnityEngine;

public class TriggerRagdollRelocate : TriggerBase
{
	public Transform targetLocation;

	internal override void OnObjectAdded(GameObject obj, Collider col)
	{
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		base.OnObjectAdded(obj, col);
		BaseEntity baseEntity = obj.transform.ToBaseEntity();
		if ((Object)(object)baseEntity != (Object)null && baseEntity.isServer)
		{
			RepositionTransform(((Component)baseEntity).transform);
		}
		Ragdoll componentInParent = obj.GetComponentInParent<Ragdoll>();
		if (!((Object)(object)componentInParent != (Object)null))
		{
			return;
		}
		RepositionTransform(((Component)componentInParent).transform);
		foreach (Rigidbody rigidbody in componentInParent.rigidbodies)
		{
			if (((Component)rigidbody).transform.position.y < ((Component)this).transform.position.y)
			{
				RepositionTransform(((Component)rigidbody).transform);
			}
		}
	}

	private void RepositionTransform(Transform t)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = targetLocation.InverseTransformPoint(t.position);
		val.y = 0f;
		val = targetLocation.TransformPoint(val);
		t.position = val;
	}
}
