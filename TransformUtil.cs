using System.Collections.Generic;
using System.Linq;
using Facepunch;
using UnityEngine;
using UnityEngine.Profiling;

public static class TransformUtil
{
	public static bool GetGroundInfo(Vector3 startPos, out RaycastHit hit, Transform ignoreTransform = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		return GetGroundInfo(startPos, out hit, 100f, LayerMask.op_Implicit(-1), ignoreTransform);
	}

	public static bool GetGroundInfo(Vector3 startPos, out RaycastHit hit, float range, Transform ignoreTransform = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		return GetGroundInfo(startPos, out hit, range, LayerMask.op_Implicit(-1), ignoreTransform);
	}

	public static bool GetGroundInfo(Vector3 startPos, out RaycastHit hitOut, float range, LayerMask mask, Transform ignoreTransform = null)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		Profiler.BeginSample("TransformUtil.GetGroundInfo");
		startPos.y += 0.25f;
		range += 0.25f;
		hitOut = default(RaycastHit);
		Ray ray = default(Ray);
		((Ray)(ref ray))._002Ector(startPos, Vector3.down);
		if (GamePhysics.Trace(ray, 0f, out var hitInfo, range, LayerMask.op_Implicit(mask), (QueryTriggerInteraction)0))
		{
			if ((Object)(object)ignoreTransform != (Object)null && (Object)(object)((RaycastHit)(ref hitInfo)).collider != (Object)null && ((Object)(object)((Component)((RaycastHit)(ref hitInfo)).collider).transform == (Object)(object)ignoreTransform || ((Component)((RaycastHit)(ref hitInfo)).collider).transform.IsChildOf(ignoreTransform)))
			{
				return GetGroundInfo(startPos - new Vector3(0f, 0.01f, 0f), out hitOut, range, mask, ignoreTransform);
			}
			hitOut = hitInfo;
			Profiler.EndSample();
			return true;
		}
		Profiler.EndSample();
		return false;
	}

	public static bool GetGroundInfo(Vector3 startPos, out Vector3 pos, out Vector3 normal, Transform ignoreTransform = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		return GetGroundInfo(startPos, out pos, out normal, 100f, LayerMask.op_Implicit(-1), ignoreTransform);
	}

	public static bool GetGroundInfo(Vector3 startPos, out Vector3 pos, out Vector3 normal, float range, Transform ignoreTransform = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return GetGroundInfo(startPos, out pos, out normal, range, LayerMask.op_Implicit(-1), ignoreTransform);
	}

	public static bool GetGroundInfo(Vector3 startPos, out Vector3 pos, out Vector3 normal, float range, LayerMask mask, Transform ignoreTransform = null)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		Profiler.BeginSample("TransformUtil.GetGroundInfo (All)");
		startPos.y += 0.25f;
		range += 0.25f;
		List<RaycastHit> list = Pool.GetList<RaycastHit>();
		Ray ray = default(Ray);
		((Ray)(ref ray))._002Ector(startPos, Vector3.down);
		GamePhysics.TraceAll(ray, 0f, list, range, LayerMask.op_Implicit(mask), (QueryTriggerInteraction)1);
		foreach (RaycastHit item in list)
		{
			RaycastHit current = item;
			if ((Object)(object)ignoreTransform != (Object)null && (Object)(object)((RaycastHit)(ref current)).collider != (Object)null && ((Object)(object)((Component)((RaycastHit)(ref current)).collider).transform == (Object)(object)ignoreTransform || ((Component)((RaycastHit)(ref current)).collider).transform.IsChildOf(ignoreTransform)))
			{
				continue;
			}
			pos = ((RaycastHit)(ref current)).point;
			normal = ((RaycastHit)(ref current)).normal;
			Profiler.EndSample();
			Pool.FreeList<RaycastHit>(ref list);
			return true;
		}
		pos = startPos;
		normal = Vector3.up;
		Pool.FreeList<RaycastHit>(ref list);
		Profiler.EndSample();
		return false;
	}

	public static bool GetGroundInfoTerrainOnly(Vector3 startPos, out Vector3 pos, out Vector3 normal)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		return GetGroundInfoTerrainOnly(startPos, out pos, out normal, 100f, LayerMask.op_Implicit(-1));
	}

	public static bool GetGroundInfoTerrainOnly(Vector3 startPos, out Vector3 pos, out Vector3 normal, float range)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return GetGroundInfoTerrainOnly(startPos, out pos, out normal, range, LayerMask.op_Implicit(-1));
	}

	public static bool GetGroundInfoTerrainOnly(Vector3 startPos, out Vector3 pos, out Vector3 normal, float range, LayerMask mask)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		Profiler.BeginSample("TransformUtil.GetGroundInfoTerrainOnly");
		startPos.y += 0.25f;
		range += 0.25f;
		Ray val = default(Ray);
		((Ray)(ref val))._002Ector(startPos, Vector3.down);
		RaycastHit val2 = default(RaycastHit);
		if (Physics.Raycast(val, ref val2, range, LayerMask.op_Implicit(mask)) && ((RaycastHit)(ref val2)).collider is TerrainCollider)
		{
			pos = ((RaycastHit)(ref val2)).point;
			normal = ((RaycastHit)(ref val2)).normal;
			Profiler.EndSample();
			return true;
		}
		pos = startPos;
		normal = Vector3.up;
		Profiler.EndSample();
		return false;
	}

	public static Transform[] GetRootObjects()
	{
		return (from x in Object.FindObjectsOfType<Transform>()
			where (Object)(object)((Component)x).transform == (Object)(object)((Component)x).transform.root
			select x).ToArray();
	}
}
