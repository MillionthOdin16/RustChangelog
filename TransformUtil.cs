using System.Collections.Generic;
using System.Linq;
using Facepunch;
using UnityEngine;

public static class TransformUtil
{
	public static bool GetGroundInfo(Vector3 startPos, out RaycastHit hit, Transform ignoreTransform = null)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		return GetGroundInfo(startPos, out hit, 100f, LayerMask.op_Implicit(-1), ignoreTransform);
	}

	public static bool GetGroundInfo(Vector3 startPos, out RaycastHit hit, float range, Transform ignoreTransform = null)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		return GetGroundInfo(startPos, out hit, range, LayerMask.op_Implicit(-1), ignoreTransform);
	}

	public static bool GetGroundInfo(Vector3 startPos, out RaycastHit hitOut, float range, LayerMask mask, Transform ignoreTransform = null)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		startPos.y += 0.25f;
		range += 0.25f;
		hitOut = default(RaycastHit);
		if (GamePhysics.Trace(new Ray(startPos, Vector3.down), 0f, out var hitInfo, range, LayerMask.op_Implicit(mask), (QueryTriggerInteraction)0))
		{
			if ((Object)(object)ignoreTransform != (Object)null && (Object)(object)((RaycastHit)(ref hitInfo)).collider != (Object)null && ((Object)(object)((Component)((RaycastHit)(ref hitInfo)).collider).transform == (Object)(object)ignoreTransform || ((Component)((RaycastHit)(ref hitInfo)).collider).transform.IsChildOf(ignoreTransform)))
			{
				return GetGroundInfo(startPos - new Vector3(0f, 0.01f, 0f), out hitOut, range, mask, ignoreTransform);
			}
			hitOut = hitInfo;
			return true;
		}
		return false;
	}

	public static bool GetGroundInfo(Vector3 startPos, out Vector3 pos, out Vector3 normal, Transform ignoreTransform = null)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		return GetGroundInfo(startPos, out pos, out normal, 100f, LayerMask.op_Implicit(-1), ignoreTransform);
	}

	public static bool GetGroundInfo(Vector3 startPos, out Vector3 pos, out Vector3 normal, float range, Transform ignoreTransform = null)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		return GetGroundInfo(startPos, out pos, out normal, range, LayerMask.op_Implicit(-1), ignoreTransform);
	}

	public static bool GetGroundInfo(Vector3 startPos, out Vector3 pos, out Vector3 normal, float range, LayerMask mask, Transform ignoreTransform = null)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		startPos.y += 0.25f;
		range += 0.25f;
		List<RaycastHit> list = Pool.GetList<RaycastHit>();
		GamePhysics.TraceAll(new Ray(startPos, Vector3.down), 0f, list, range, LayerMask.op_Implicit(mask), (QueryTriggerInteraction)1);
		foreach (RaycastHit item in list)
		{
			RaycastHit current = item;
			if (!((Object)(object)ignoreTransform != (Object)null) || !((Object)(object)((RaycastHit)(ref current)).collider != (Object)null) || (!((Object)(object)((Component)((RaycastHit)(ref current)).collider).transform == (Object)(object)ignoreTransform) && !((Component)((RaycastHit)(ref current)).collider).transform.IsChildOf(ignoreTransform)))
			{
				pos = ((RaycastHit)(ref current)).point;
				normal = ((RaycastHit)(ref current)).normal;
				Pool.FreeList<RaycastHit>(ref list);
				return true;
			}
		}
		pos = startPos;
		normal = Vector3.up;
		Pool.FreeList<RaycastHit>(ref list);
		return false;
	}

	public static bool GetGroundInfoTerrainOnly(Vector3 startPos, out Vector3 pos, out Vector3 normal)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		return GetGroundInfoTerrainOnly(startPos, out pos, out normal, 100f, LayerMask.op_Implicit(-1));
	}

	public static bool GetGroundInfoTerrainOnly(Vector3 startPos, out Vector3 pos, out Vector3 normal, float range)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		return GetGroundInfoTerrainOnly(startPos, out pos, out normal, range, LayerMask.op_Implicit(-1));
	}

	public static bool GetGroundInfoTerrainOnly(Vector3 startPos, out Vector3 pos, out Vector3 normal, float range, LayerMask mask)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		startPos.y += 0.25f;
		range += 0.25f;
		RaycastHit val = default(RaycastHit);
		if (Physics.Raycast(new Ray(startPos, Vector3.down), ref val, range, LayerMask.op_Implicit(mask)) && ((RaycastHit)(ref val)).collider is TerrainCollider)
		{
			pos = ((RaycastHit)(ref val)).point;
			normal = ((RaycastHit)(ref val)).normal;
			return true;
		}
		pos = startPos;
		normal = Vector3.up;
		return false;
	}

	public static Transform[] GetRootObjects()
	{
		return (from x in Object.FindObjectsOfType<Transform>()
			where (Object)(object)((Component)x).transform == (Object)(object)((Component)x).transform.root
			select x).ToArray();
	}
}
