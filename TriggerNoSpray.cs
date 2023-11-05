using UnityEngine;

public class TriggerNoSpray : TriggerBase
{
	public BoxCollider TriggerCollider;

	private OBB cachedBounds;

	private Transform cachedTransform;

	private void OnEnable()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		cachedTransform = ((Component)this).transform;
		cachedBounds = new OBB(cachedTransform, new Bounds(TriggerCollider.center, TriggerCollider.size));
	}

	internal override GameObject InterestedInObject(GameObject obj)
	{
		BaseEntity baseEntity = obj.ToBaseEntity();
		if ((Object)(object)baseEntity == (Object)null)
		{
			return null;
		}
		if ((Object)(object)baseEntity.ToPlayer() == (Object)null)
		{
			return null;
		}
		return ((Component)baseEntity).gameObject;
	}

	public bool IsPositionValid(Vector3 worldPosition)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return !((OBB)(ref cachedBounds)).Contains(worldPosition);
	}
}
