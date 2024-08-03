using UnityEngine;

public class TriggerWetness : TriggerBase
{
	public float Wetness = 0.25f;

	public SphereCollider TargetCollider;

	public Transform OriginTransform = null;

	public bool ApplyLocalHeightCheck = false;

	public float MinLocalHeight = 0f;

	public float WorkoutWetness(Vector3 position)
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if (ApplyLocalHeightCheck)
		{
			Vector3 val = ((Component)this).transform.InverseTransformPoint(position);
			if (val.y < MinLocalHeight)
			{
				return 0f;
			}
		}
		float num = Vector3Ex.Distance2D(OriginTransform.position, position) / TargetCollider.radius;
		num = Mathf.Clamp01(num);
		num = 1f - num;
		return Mathf.Lerp(0f, Wetness, num);
	}

	internal override GameObject InterestedInObject(GameObject obj)
	{
		obj = base.InterestedInObject(obj);
		if ((Object)(object)obj == (Object)null)
		{
			return null;
		}
		BaseEntity baseEntity = obj.ToBaseEntity();
		if ((Object)(object)baseEntity == (Object)null)
		{
			return null;
		}
		if (baseEntity.isClient)
		{
			return null;
		}
		return ((Component)baseEntity).gameObject;
	}
}
