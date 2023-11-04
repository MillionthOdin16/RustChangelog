using UnityEngine;

public class AITraversalArea : TriggerBase
{
	public Transform entryPoint1;

	public Transform entryPoint2;

	public AITraversalWaitPoint[] waitPoints;

	public Bounds movementArea;

	public Transform activeEntryPoint;

	public float nextFreeTime = 0f;

	public void OnValidate()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		((Bounds)(ref movementArea)).center = ((Component)this).transform.position;
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
		if (!baseEntity.IsNpc)
		{
			return null;
		}
		return ((Component)baseEntity).gameObject;
	}

	public bool CanTraverse(BaseEntity ent)
	{
		return Time.time > nextFreeTime;
	}

	public Transform GetClosestEntry(Vector3 position)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		float num = Vector3.Distance(position, entryPoint1.position);
		float num2 = Vector3.Distance(position, entryPoint2.position);
		if (num < num2)
		{
			return entryPoint1;
		}
		return entryPoint2;
	}

	public Transform GetFarthestEntry(Vector3 position)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		float num = Vector3.Distance(position, entryPoint1.position);
		float num2 = Vector3.Distance(position, entryPoint2.position);
		if (num > num2)
		{
			return entryPoint1;
		}
		return entryPoint2;
	}

	public void SetBusyFor(float dur = 1f)
	{
		nextFreeTime = Time.time + dur;
	}

	public bool CanUse(Vector3 dirFrom)
	{
		return Time.time > nextFreeTime;
	}

	internal override void OnEntityEnter(BaseEntity ent)
	{
		base.OnEntityEnter(ent);
	}

	public AITraversalWaitPoint GetEntryPointNear(Vector3 pos)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		Vector3 position = GetClosestEntry(pos).position;
		Vector3 position2 = GetFarthestEntry(pos).position;
		BaseEntity[] array = new BaseEntity[1];
		AITraversalWaitPoint result = null;
		float num = 0f;
		AITraversalWaitPoint[] array2 = waitPoints;
		foreach (AITraversalWaitPoint aITraversalWaitPoint in array2)
		{
			if (aITraversalWaitPoint.Occupied())
			{
				continue;
			}
			Vector3 position3 = ((Component)aITraversalWaitPoint).transform.position;
			float num2 = Vector3.Distance(position, position3);
			float num3 = Vector3.Distance(position2, position3);
			if (!(num3 < num2))
			{
				float num4 = Vector3.Distance(position3, pos);
				float num5 = (1f - Mathf.InverseLerp(0f, 20f, num4)) * 100f;
				if (num5 > num)
				{
					num = num5;
					result = aITraversalWaitPoint;
				}
			}
		}
		return result;
	}

	public bool EntityFilter(BaseEntity ent)
	{
		return ent.IsNpc && ent.isServer;
	}

	internal override void OnEntityLeave(BaseEntity ent)
	{
		base.OnEntityLeave(ent);
	}

	public void OnDrawGizmos()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		Gizmos.color = Color.magenta;
		Gizmos.DrawCube(entryPoint1.position + Vector3.up * 0.125f, new Vector3(0.5f, 0.25f, 0.5f));
		Gizmos.DrawCube(entryPoint2.position + Vector3.up * 0.125f, new Vector3(0.5f, 0.25f, 0.5f));
		Gizmos.color = new Color(0.2f, 1f, 0.2f, 0.5f);
		Gizmos.DrawCube(((Bounds)(ref movementArea)).center, ((Bounds)(ref movementArea)).size);
		Gizmos.color = Color.magenta;
		AITraversalWaitPoint[] array = waitPoints;
		foreach (AITraversalWaitPoint aITraversalWaitPoint in array)
		{
			GizmosUtil.DrawCircleY(((Component)aITraversalWaitPoint).transform.position, 0.5f);
		}
	}
}
