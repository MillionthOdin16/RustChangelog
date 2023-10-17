using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using UnityEngine;
using UnityEngine.Profiling;

public static class GamePhysics
{
	public const int BufferLength = 8192;

	private static RaycastHit[] hitBuffer = (RaycastHit[])(object)new RaycastHit[8192];

	private static RaycastHit[] hitBufferB = (RaycastHit[])(object)new RaycastHit[8192];

	private static Collider[] colBuffer = (Collider[])(object)new Collider[8192];

	public static bool CheckSphere(Vector3 position, float radius, int layerMask = -5, QueryTriggerInteraction triggerInteraction = 0)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		layerMask = HandleIgnoreCollision(position, layerMask);
		return Physics.CheckSphere(position, radius, layerMask, triggerInteraction);
	}

	public static bool CheckCapsule(Vector3 start, Vector3 end, float radius, int layerMask = -5, QueryTriggerInteraction triggerInteraction = 0)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		layerMask = HandleIgnoreCollision((start + end) * 0.5f, layerMask);
		return Physics.CheckCapsule(start, end, radius, layerMask, triggerInteraction);
	}

	public static bool CheckOBB(OBB obb, int layerMask = -5, QueryTriggerInteraction triggerInteraction = 0)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		layerMask = HandleIgnoreCollision(obb.position, layerMask);
		return Physics.CheckBox(obb.position, obb.extents, obb.rotation, layerMask, triggerInteraction);
	}

	public static bool CheckOBBAndEntity(OBB obb, int layerMask = -5, QueryTriggerInteraction triggerInteraction = 0, BaseEntity ignoreEntity = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		layerMask = HandleIgnoreCollision(obb.position, layerMask);
		int num = Physics.OverlapBoxNonAlloc(obb.position, obb.extents, colBuffer, obb.rotation, layerMask, triggerInteraction);
		for (int i = 0; i < num; i++)
		{
			BaseEntity baseEntity = colBuffer[i].ToBaseEntity();
			if (!((Object)(object)baseEntity != (Object)null) || !((Object)(object)ignoreEntity != (Object)null) || (baseEntity.isServer == ignoreEntity.isServer && !((Object)(object)baseEntity == (Object)(object)ignoreEntity)))
			{
				return true;
			}
		}
		return false;
	}

	public static bool CheckBounds(Bounds bounds, int layerMask = -5, QueryTriggerInteraction triggerInteraction = 0)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		layerMask = HandleIgnoreCollision(((Bounds)(ref bounds)).center, layerMask);
		return Physics.CheckBox(((Bounds)(ref bounds)).center, ((Bounds)(ref bounds)).extents, Quaternion.identity, layerMask, triggerInteraction);
	}

	public static bool CheckInsideNonConvexMesh(Vector3 point, int layerMask = -5)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		bool queriesHitBackfaces = Physics.queriesHitBackfaces;
		Physics.queriesHitBackfaces = true;
		int num = Physics.RaycastNonAlloc(point, Vector3.up, hitBuffer, 100f, layerMask);
		int num2 = Physics.RaycastNonAlloc(point, -Vector3.up, hitBufferB, 100f, layerMask);
		if (num >= hitBuffer.Length)
		{
			Debug.LogWarning((object)"CheckInsideNonConvexMesh query is exceeding hitBuffer length.");
			return false;
		}
		if (num2 > hitBufferB.Length)
		{
			Debug.LogWarning((object)"CheckInsideNonConvexMesh query is exceeding hitBufferB length.");
			return false;
		}
		for (int i = 0; i < num; i++)
		{
			for (int j = 0; j < num2; j++)
			{
				if ((Object)(object)((RaycastHit)(ref hitBuffer[i])).collider == (Object)(object)((RaycastHit)(ref hitBufferB[j])).collider)
				{
					Physics.queriesHitBackfaces = queriesHitBackfaces;
					return true;
				}
			}
		}
		Physics.queriesHitBackfaces = queriesHitBackfaces;
		return false;
	}

	public static bool CheckInsideAnyCollider(Vector3 point, int layerMask = -5)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		if (Physics.CheckSphere(point, 0f, layerMask))
		{
			return true;
		}
		if (CheckInsideNonConvexMesh(point, layerMask))
		{
			return true;
		}
		if ((Object)(object)TerrainMeta.HeightMap != (Object)null && TerrainMeta.HeightMap.GetHeight(point) > point.y)
		{
			return true;
		}
		return false;
	}

	public static void OverlapSphere(Vector3 position, float radius, List<Collider> list, int layerMask = -5, QueryTriggerInteraction triggerInteraction = 1)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		layerMask = HandleIgnoreCollision(position, layerMask);
		int count = Physics.OverlapSphereNonAlloc(position, radius, colBuffer, layerMask, triggerInteraction);
		BufferToList(count, list);
	}

	public static void CapsuleSweep(Vector3 position0, Vector3 position1, float radius, Vector3 direction, float distance, List<RaycastHit> list, int layerMask = -5, QueryTriggerInteraction triggerInteraction = 1)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		layerMask = HandleIgnoreCollision(position1, layerMask);
		layerMask = HandleIgnoreCollision(position1, layerMask);
		int count = Physics.CapsuleCastNonAlloc(position0, position1, radius, direction, hitBuffer, distance, layerMask, triggerInteraction);
		HitBufferToList(count, list);
	}

	public static void OverlapCapsule(Vector3 point0, Vector3 point1, float radius, List<Collider> list, int layerMask = -5, QueryTriggerInteraction triggerInteraction = 1)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		layerMask = HandleIgnoreCollision(point0, layerMask);
		layerMask = HandleIgnoreCollision(point1, layerMask);
		int count = Physics.OverlapCapsuleNonAlloc(point0, point1, radius, colBuffer, layerMask, triggerInteraction);
		BufferToList(count, list);
	}

	public static void OverlapOBB(OBB obb, List<Collider> list, int layerMask = -5, QueryTriggerInteraction triggerInteraction = 1)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		layerMask = HandleIgnoreCollision(obb.position, layerMask);
		int count = Physics.OverlapBoxNonAlloc(obb.position, obb.extents, colBuffer, obb.rotation, layerMask, triggerInteraction);
		BufferToList(count, list);
	}

	public static void OverlapBounds(Bounds bounds, List<Collider> list, int layerMask = -5, QueryTriggerInteraction triggerInteraction = 1)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		layerMask = HandleIgnoreCollision(((Bounds)(ref bounds)).center, layerMask);
		int count = Physics.OverlapBoxNonAlloc(((Bounds)(ref bounds)).center, ((Bounds)(ref bounds)).extents, colBuffer, Quaternion.identity, layerMask, triggerInteraction);
		BufferToList(count, list);
	}

	private static void BufferToList(int count, List<Collider> list)
	{
		if (count >= colBuffer.Length)
		{
			Debug.LogWarning((object)"Physics query is exceeding collider buffer length.");
		}
		for (int i = 0; i < count; i++)
		{
			list.Add(colBuffer[i]);
			colBuffer[i] = null;
		}
	}

	public static bool CheckSphere<T>(Vector3 pos, float radius, int layerMask = -5, QueryTriggerInteraction triggerInteraction = 1) where T : Component
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		List<Collider> list = Pool.GetList<Collider>();
		OverlapSphere(pos, radius, list, layerMask, triggerInteraction);
		bool result = CheckComponent<T>(list);
		Pool.FreeList<Collider>(ref list);
		return result;
	}

	public static bool CheckCapsule<T>(Vector3 start, Vector3 end, float radius, int layerMask = -5, QueryTriggerInteraction triggerInteraction = 1) where T : Component
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		List<Collider> list = Pool.GetList<Collider>();
		OverlapCapsule(start, end, radius, list, layerMask, triggerInteraction);
		bool result = CheckComponent<T>(list);
		Pool.FreeList<Collider>(ref list);
		return result;
	}

	public static bool CheckOBB<T>(OBB obb, int layerMask = -5, QueryTriggerInteraction triggerInteraction = 1) where T : Component
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		List<Collider> list = Pool.GetList<Collider>();
		OverlapOBB(obb, list, layerMask, triggerInteraction);
		bool result = CheckComponent<T>(list);
		Pool.FreeList<Collider>(ref list);
		return result;
	}

	public static bool CheckBounds<T>(Bounds bounds, int layerMask = -5, QueryTriggerInteraction triggerInteraction = 1) where T : Component
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		List<Collider> list = Pool.GetList<Collider>();
		OverlapBounds(bounds, list, layerMask, triggerInteraction);
		bool result = CheckComponent<T>(list);
		Pool.FreeList<Collider>(ref list);
		return result;
	}

	private static bool CheckComponent<T>(List<Collider> list)
	{
		for (int i = 0; i < list.Count; i++)
		{
			if (((Component)list[i]).gameObject.GetComponent<T>() != null)
			{
				return true;
			}
		}
		return false;
	}

	public static void OverlapSphere<T>(Vector3 position, float radius, List<T> list, int layerMask = -5, QueryTriggerInteraction triggerInteraction = 1) where T : Component
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		layerMask = HandleIgnoreCollision(position, layerMask);
		int count = Physics.OverlapSphereNonAlloc(position, radius, colBuffer, layerMask, triggerInteraction);
		BufferToList(count, list);
	}

	public static void OverlapCapsule<T>(Vector3 point0, Vector3 point1, float radius, List<T> list, int layerMask = -5, QueryTriggerInteraction triggerInteraction = 1) where T : Component
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		layerMask = HandleIgnoreCollision(point0, layerMask);
		layerMask = HandleIgnoreCollision(point1, layerMask);
		int count = Physics.OverlapCapsuleNonAlloc(point0, point1, radius, colBuffer, layerMask, triggerInteraction);
		BufferToList(count, list);
	}

	public static void OverlapOBB<T>(OBB obb, List<T> list, int layerMask = -5, QueryTriggerInteraction triggerInteraction = 1) where T : Component
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		layerMask = HandleIgnoreCollision(obb.position, layerMask);
		int count = Physics.OverlapBoxNonAlloc(obb.position, obb.extents, colBuffer, obb.rotation, layerMask, triggerInteraction);
		BufferToList(count, list);
	}

	public static void OverlapBounds<T>(Bounds bounds, List<T> list, int layerMask = -5, QueryTriggerInteraction triggerInteraction = 1) where T : Component
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		layerMask = HandleIgnoreCollision(((Bounds)(ref bounds)).center, layerMask);
		int count = Physics.OverlapBoxNonAlloc(((Bounds)(ref bounds)).center, ((Bounds)(ref bounds)).extents, colBuffer, Quaternion.identity, layerMask, triggerInteraction);
		BufferToList(count, list);
	}

	private static void BufferToList<T>(int count, List<T> list) where T : Component
	{
		if (count >= colBuffer.Length)
		{
			Debug.LogWarning((object)"Physics query is exceeding collider buffer length.");
		}
		for (int i = 0; i < count; i++)
		{
			T component = ((Component)colBuffer[i]).gameObject.GetComponent<T>();
			if (Object.op_Implicit((Object)(object)component))
			{
				list.Add(component);
			}
			colBuffer[i] = null;
		}
	}

	private static void HitBufferToList(int count, List<RaycastHit> list)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		if (count >= hitBuffer.Length)
		{
			Debug.LogWarning((object)"Physics query is exceeding collider buffer length.");
		}
		for (int i = 0; i < count; i++)
		{
			list.Add(hitBuffer[i]);
		}
	}

	public static bool Trace(Ray ray, float radius, out RaycastHit hitInfo, float maxDistance = float.PositiveInfinity, int layerMask = -5, QueryTriggerInteraction triggerInteraction = 0, BaseEntity ignoreEntity = null)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		Profiler.BeginSample("Pool_RaycastHit");
		List<RaycastHit> list = Pool.GetList<RaycastHit>();
		Profiler.EndSample();
		TraceAllUnordered(ray, radius, list, maxDistance, layerMask, triggerInteraction, ignoreEntity);
		if (list.Count == 0)
		{
			Profiler.BeginSample("NoHits");
			hitInfo = default(RaycastHit);
			Pool.FreeList<RaycastHit>(ref list);
			Profiler.EndSample();
			return false;
		}
		Sort(list);
		hitInfo = list[0];
		Profiler.BeginSample("Pool_FreeList");
		Pool.FreeList<RaycastHit>(ref list);
		Profiler.EndSample();
		return true;
	}

	public static void TraceAll(Ray ray, float radius, List<RaycastHit> hits, float maxDistance = float.PositiveInfinity, int layerMask = -5, QueryTriggerInteraction triggerInteraction = 0, BaseEntity ignoreEntity = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		TraceAllUnordered(ray, radius, hits, maxDistance, layerMask, triggerInteraction, ignoreEntity);
		Sort(hits);
	}

	public static void TraceAllUnordered(Ray ray, float radius, List<RaycastHit> hits, float maxDistance = float.PositiveInfinity, int layerMask = -5, QueryTriggerInteraction triggerInteraction = 0, BaseEntity ignoreEntity = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		Profiler.BeginSample("TraceAllInternal");
		int num = 0;
		num = ((radius != 0f) ? Physics.SphereCastNonAlloc(ray, radius, hitBuffer, maxDistance, layerMask, triggerInteraction) : Physics.RaycastNonAlloc(ray, hitBuffer, maxDistance, layerMask, triggerInteraction));
		if (num < hitBuffer.Length && ((uint)layerMask & 0x10u) != 0 && WaterSystem.Trace(ray, out var position, out var normal, maxDistance))
		{
			RaycastHit val = default(RaycastHit);
			((RaycastHit)(ref val)).point = position;
			((RaycastHit)(ref val)).normal = normal;
			Vector3 val2 = position - ((Ray)(ref ray)).origin;
			((RaycastHit)(ref val)).distance = ((Vector3)(ref val2)).magnitude;
			RaycastHit val3 = val;
			hitBuffer[num++] = val3;
		}
		if (num == 0)
		{
			Profiler.EndSample();
			return;
		}
		if (num >= hitBuffer.Length)
		{
			Debug.LogWarning((object)"Physics query is exceeding hit buffer length.");
		}
		for (int i = 0; i < num; i++)
		{
			RaycastHit val4 = hitBuffer[i];
			if (Verify(val4, ignoreEntity))
			{
				hits.Add(val4);
			}
		}
		Profiler.EndSample();
	}

	public static bool LineOfSightRadius(Vector3 p0, Vector3 p1, int layerMask, float radius, float padding0, float padding1, BaseEntity ignoreEntity = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return LineOfSightInternal(p0, p1, layerMask, radius, padding0, padding1, ignoreEntity);
	}

	public static bool LineOfSightRadius(Vector3 p0, Vector3 p1, int layerMask, float radius, float padding, BaseEntity ignoreEntity = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return LineOfSightInternal(p0, p1, layerMask, radius, padding, padding, ignoreEntity);
	}

	public static bool LineOfSightRadius(Vector3 p0, Vector3 p1, int layerMask, float radius, BaseEntity ignoreEntity = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return LineOfSightInternal(p0, p1, layerMask, radius, 0f, 0f, ignoreEntity);
	}

	public static bool LineOfSight(Vector3 p0, Vector3 p1, int layerMask, float padding0, float padding1, BaseEntity ignoreEntity = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return LineOfSightRadius(p0, p1, layerMask, 0f, padding0, padding1, ignoreEntity);
	}

	public static bool LineOfSight(Vector3 p0, Vector3 p1, int layerMask, float padding, BaseEntity ignoreEntity = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return LineOfSightRadius(p0, p1, layerMask, 0f, padding, padding, ignoreEntity);
	}

	public static bool LineOfSight(Vector3 p0, Vector3 p1, int layerMask, BaseEntity ignoreEntity = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return LineOfSightRadius(p0, p1, layerMask, 0f, 0f, 0f, ignoreEntity);
	}

	private static bool LineOfSightInternal(Vector3 p0, Vector3 p1, int layerMask, float radius, float padding0, float padding1, BaseEntity ignoreEntity = null)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		if (!ValidBounds.Test(p0))
		{
			return false;
		}
		if (!ValidBounds.Test(p1))
		{
			return false;
		}
		Vector3 val = p1 - p0;
		float magnitude = ((Vector3)(ref val)).magnitude;
		if (magnitude <= padding0 + padding1)
		{
			return true;
		}
		Vector3 val2 = val / magnitude;
		Ray val3 = default(Ray);
		((Ray)(ref val3))._002Ector(p0 + val2 * padding0, val2);
		float num = magnitude - padding0 - padding1;
		bool flag;
		RaycastHit hitInfo = default(RaycastHit);
		if (!ignoreEntity.IsRealNull() || ((uint)layerMask & 0x800000u) != 0)
		{
			flag = Trace(val3, 0f, out hitInfo, num, layerMask, (QueryTriggerInteraction)1, ignoreEntity);
			if (radius > 0f && !flag)
			{
				flag = Trace(val3, radius, out hitInfo, num, layerMask, (QueryTriggerInteraction)1, ignoreEntity);
			}
		}
		else
		{
			flag = Physics.Raycast(val3, ref hitInfo, num, layerMask, (QueryTriggerInteraction)1);
			if (radius > 0f && !flag)
			{
				flag = Physics.SphereCast(val3, radius, ref hitInfo, num, layerMask, (QueryTriggerInteraction)1);
			}
		}
		if (!flag)
		{
			if (ConVar.Vis.lineofsight)
			{
				ConsoleNetwork.BroadcastToAllClients("ddraw.line", 60f, Color.green, p0, p1);
			}
			return true;
		}
		if (ConVar.Vis.lineofsight)
		{
			ConsoleNetwork.BroadcastToAllClients("ddraw.line", 60f, Color.red, p0, p1);
			ConsoleNetwork.BroadcastToAllClients("ddraw.text", 60f, Color.white, ((RaycastHit)(ref hitInfo)).point, ((Object)((RaycastHit)(ref hitInfo)).collider).name);
		}
		return false;
	}

	public static bool Verify(RaycastHit hitInfo, BaseEntity ignoreEntity = null)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		return Verify(((RaycastHit)(ref hitInfo)).collider, ((RaycastHit)(ref hitInfo)).point, ignoreEntity);
	}

	public static bool Verify(Collider collider, Vector3 point, BaseEntity ignoreEntity = null)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		Profiler.BeginSample("GamePhysics.Verify");
		if ((Object)(object)collider == (Object)null)
		{
			if (Object.op_Implicit((Object)(object)WaterSystem.Collision) && WaterSystem.Collision.GetIgnore(point))
			{
				Profiler.EndSample();
				return false;
			}
			Profiler.EndSample();
			return true;
		}
		if (collider is TerrainCollider)
		{
			if (Object.op_Implicit((Object)(object)TerrainMeta.Collision) && TerrainMeta.Collision.GetIgnore(point))
			{
				Profiler.EndSample();
				return false;
			}
			Profiler.EndSample();
			return true;
		}
		if (CompareEntity(collider.ToBaseEntity(), ignoreEntity))
		{
			Profiler.EndSample();
			return false;
		}
		Profiler.EndSample();
		return collider.enabled;
	}

	private static bool CompareEntity(BaseEntity a, BaseEntity b)
	{
		if (a.IsRealNull() || b.IsRealNull())
		{
			return false;
		}
		if ((Object)(object)a == (Object)(object)b)
		{
			return true;
		}
		return false;
	}

	public static int HandleIgnoreCollision(Vector3 position, int layerMask)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		int num = 8388608;
		if ((layerMask & num) != 0 && Object.op_Implicit((Object)(object)TerrainMeta.Collision) && TerrainMeta.Collision.GetIgnore(position))
		{
			layerMask &= ~num;
		}
		int num2 = 16;
		if ((layerMask & num2) != 0 && Object.op_Implicit((Object)(object)WaterSystem.Collision) && WaterSystem.Collision.GetIgnore(position))
		{
			layerMask &= ~num2;
		}
		return layerMask;
	}

	public static void Sort(List<RaycastHit> hits)
	{
		hits.Sort((RaycastHit a, RaycastHit b) => ((RaycastHit)(ref a)).distance.CompareTo(((RaycastHit)(ref b)).distance));
	}

	public static void Sort(RaycastHit[] hits)
	{
		Array.Sort(hits, (RaycastHit a, RaycastHit b) => ((RaycastHit)(ref a)).distance.CompareTo(((RaycastHit)(ref b)).distance));
	}
}
