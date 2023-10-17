using ConVar;
using UnityEngine;

public class TriggerParentEnclosed : TriggerParent
{
	public enum TriggerMode
	{
		TriggerPoint,
		PivotPoint
	}

	public float Padding = 0f;

	[Tooltip("AnyIntersect: Look for any intersection with the trigger. OriginIntersect: Only consider objects in the trigger if their origin is inside")]
	public TriggerMode intersectionMode = TriggerMode.TriggerPoint;

	public bool CheckBoundsOnUnparent = false;

	private BoxCollider boxCollider;

	protected void OnEnable()
	{
		boxCollider = ((Component)this).GetComponent<BoxCollider>();
	}

	public override bool ShouldParent(BaseEntity ent, bool bypassOtherTriggerCheck = false)
	{
		if (!base.ShouldParent(ent, bypassOtherTriggerCheck))
		{
			return false;
		}
		return IsInside(ent, Padding);
	}

	internal override bool SkipOnTriggerExit(Collider collider)
	{
		if (!CheckBoundsOnUnparent)
		{
			return false;
		}
		if (!Debugging.checkparentingtriggers)
		{
			return false;
		}
		BaseEntity baseEntity = collider.ToBaseEntity();
		if ((Object)(object)baseEntity == (Object)null)
		{
			return false;
		}
		return IsInside(baseEntity, 0f);
	}

	private bool IsInside(BaseEntity ent, float padding)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		Bounds val = default(Bounds);
		((Bounds)(ref val))._002Ector(boxCollider.center, boxCollider.size);
		if (padding > 0f)
		{
			((Bounds)(ref val)).Expand(padding);
		}
		OBB val2 = default(OBB);
		((OBB)(ref val2))._002Ector(((Component)boxCollider).transform, val);
		Vector3 val3 = ((intersectionMode == TriggerMode.TriggerPoint) ? ent.TriggerPoint() : ent.PivotPoint());
		return ((OBB)(ref val2)).Contains(val3);
	}
}
