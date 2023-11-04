using System.Collections.Generic;
using UnityEngine;

public class TriggerSafeZone : TriggerBase
{
	public static List<TriggerSafeZone> allSafeZones = new List<TriggerSafeZone>();

	public float maxDepth = 20f;

	public float maxAltitude = -1f;

	public Collider triggerCollider { get; private set; }

	protected void Awake()
	{
		triggerCollider = ((Component)this).GetComponent<Collider>();
	}

	protected void OnEnable()
	{
		allSafeZones.Add(this);
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		allSafeZones.Remove(this);
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

	public bool PassesHeightChecks(Vector3 entPos)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		Vector3 position = ((Component)this).transform.position;
		float num = Mathf.Abs(position.y - entPos.y);
		if (maxDepth != -1f && entPos.y < position.y && num > maxDepth)
		{
			return false;
		}
		if (maxAltitude != -1f && entPos.y > position.y && num > maxAltitude)
		{
			return false;
		}
		return true;
	}

	public float GetSafeLevel(Vector3 pos)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return PassesHeightChecks(pos) ? 1f : 0f;
	}
}
