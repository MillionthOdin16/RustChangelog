using UnityEngine;
using UnityEngine.Profiling;

public class WaterVolume : TriggerBase
{
	public Bounds WaterBounds = new Bounds(Vector3.zero, Vector3.one);

	private OBB cachedBounds;

	private Transform cachedTransform = null;

	public Transform[] cutOffPlanes = (Transform[])(object)new Transform[0];

	public bool waterEnabled = true;

	private void OnEnable()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		cachedTransform = ((Component)this).transform;
		cachedBounds = new OBB(cachedTransform, WaterBounds);
	}

	public bool Test(Vector3 pos, out WaterLevel.WaterInfo info)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		if (!waterEnabled)
		{
			info = default(WaterLevel.WaterInfo);
			return false;
		}
		UpdateCachedTransform();
		Profiler.BeginSample("Bounds.Contains");
		bool flag = ((OBB)(ref cachedBounds)).Contains(pos);
		Profiler.EndSample();
		if (flag)
		{
			if (!CheckCutOffPlanes(pos))
			{
				info = default(WaterLevel.WaterInfo);
				return false;
			}
			Profiler.BeginSample("Results");
			Plane val = default(Plane);
			((Plane)(ref val))._002Ector(cachedBounds.up, cachedBounds.position);
			Vector3 val2 = ((Plane)(ref val)).ClosestPointOnPlane(pos);
			float y = (val2 + cachedBounds.up * cachedBounds.extents.y).y;
			float y2 = (val2 + -cachedBounds.up * cachedBounds.extents.y).y;
			info.isValid = true;
			info.currentDepth = Mathf.Max(0f, y - pos.y);
			info.overallDepth = Mathf.Max(0f, y - y2);
			info.surfaceLevel = y;
			Profiler.EndSample();
			return true;
		}
		info = default(WaterLevel.WaterInfo);
		return false;
	}

	public bool Test(Bounds bounds, out WaterLevel.WaterInfo info)
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		if (!waterEnabled)
		{
			info = default(WaterLevel.WaterInfo);
			return false;
		}
		UpdateCachedTransform();
		Profiler.BeginSample("Bounds.Contains");
		bool flag = ((OBB)(ref cachedBounds)).Contains(((Bounds)(ref bounds)).ClosestPoint(cachedBounds.position));
		Profiler.EndSample();
		if (flag)
		{
			if (!CheckCutOffPlanes(((Bounds)(ref bounds)).center))
			{
				info = default(WaterLevel.WaterInfo);
				return false;
			}
			Profiler.BeginSample("Results");
			Plane val = default(Plane);
			((Plane)(ref val))._002Ector(cachedBounds.up, cachedBounds.position);
			Vector3 val2 = ((Plane)(ref val)).ClosestPointOnPlane(((Bounds)(ref bounds)).center);
			float y = (val2 + cachedBounds.up * cachedBounds.extents.y).y;
			float y2 = (val2 + -cachedBounds.up * cachedBounds.extents.y).y;
			info.isValid = true;
			info.currentDepth = Mathf.Max(0f, y - ((Bounds)(ref bounds)).min.y);
			info.overallDepth = Mathf.Max(0f, y - y2);
			info.surfaceLevel = y;
			Profiler.EndSample();
			return true;
		}
		info = default(WaterLevel.WaterInfo);
		return false;
	}

	public bool Test(Vector3 start, Vector3 end, float radius, out WaterLevel.WaterInfo info)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		if (!waterEnabled)
		{
			info = default(WaterLevel.WaterInfo);
			return false;
		}
		UpdateCachedTransform();
		Vector3 val = (start + end) * 0.5f;
		float num = Mathf.Min(start.y, end.y) - radius;
		Profiler.BeginSample("Bounds.Contains");
		bool flag = ((OBB)(ref cachedBounds)).Distance(start) < radius || ((OBB)(ref cachedBounds)).Distance(end) < radius;
		Profiler.EndSample();
		if (flag)
		{
			if (!CheckCutOffPlanes(val))
			{
				info = default(WaterLevel.WaterInfo);
				return false;
			}
			Profiler.BeginSample("Results");
			Plane val2 = default(Plane);
			((Plane)(ref val2))._002Ector(cachedBounds.up, cachedBounds.position);
			Vector3 val3 = ((Plane)(ref val2)).ClosestPointOnPlane(val);
			float y = (val3 + cachedBounds.up * cachedBounds.extents.y).y;
			float y2 = (val3 + -cachedBounds.up * cachedBounds.extents.y).y;
			info.isValid = true;
			info.currentDepth = Mathf.Max(0f, y - num);
			info.overallDepth = Mathf.Max(0f, y - y2);
			info.surfaceLevel = y;
			Profiler.EndSample();
			return true;
		}
		info = default(WaterLevel.WaterInfo);
		return false;
	}

	private bool CheckCutOffPlanes(Vector3 pos)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		int num = cutOffPlanes.Length;
		bool flag = true;
		for (int i = 0; i < num; i++)
		{
			if ((Object)(object)cutOffPlanes[i] != (Object)null)
			{
				Vector3 val = cutOffPlanes[i].InverseTransformPoint(pos);
				if (val.y > 0f)
				{
					flag = false;
					break;
				}
			}
		}
		if (!flag)
		{
			return false;
		}
		return true;
	}

	private void UpdateCachedTransform()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)cachedTransform != (Object)null && cachedTransform.hasChanged)
		{
			cachedBounds = new OBB(cachedTransform, WaterBounds);
			cachedTransform.hasChanged = false;
		}
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
		return ((Component)baseEntity).gameObject;
	}
}
