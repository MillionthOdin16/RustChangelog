using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public static class Vis
{
	private static int colCount = 0;

	private static Collider[] colBuffer = (Collider[])(object)new Collider[8192];

	private static void Buffer(Vector3 position, float radius, int layerMask = -1, QueryTriggerInteraction triggerInteraction = 2)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		Profiler.BeginSample("Vis.Buffer");
		Profiler.BeginSample("HandleIgnoreCollision");
		layerMask = GamePhysics.HandleIgnoreCollision(position, layerMask);
		Profiler.EndSample();
		int num = colCount;
		Profiler.BeginSample("OverlapSphere");
		colCount = Physics.OverlapSphereNonAlloc(position, radius, colBuffer, layerMask, triggerInteraction);
		Profiler.EndSample();
		for (int i = colCount; i < num; i++)
		{
			colBuffer[i] = null;
		}
		if (colCount >= colBuffer.Length)
		{
			Debug.LogWarning((object)"Vis query is exceeding collider buffer length.");
			colCount = colBuffer.Length;
		}
		Profiler.EndSample();
	}

	public static bool AnyColliders(Vector3 position, float radius, int layerMask = -1, QueryTriggerInteraction triggerInteraction = 1)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		Buffer(position, radius, layerMask, triggerInteraction);
		return colCount > 0;
	}

	public static void Colliders<T>(Vector3 position, float radius, List<T> list, int layerMask = -1, QueryTriggerInteraction triggerInteraction = 2) where T : Collider
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		Profiler.BeginSample("Vis.Colliders");
		Buffer(position, radius, layerMask, triggerInteraction);
		for (int i = 0; i < colCount; i++)
		{
			Collider obj = colBuffer[i];
			T val = (T)(object)((obj is T) ? obj : null);
			if (!((Object)(object)val == (Object)null) && ((Collider)val).enabled)
			{
				list.Add(val);
			}
		}
		Profiler.EndSample();
	}

	public static void Components<T>(Vector3 position, float radius, List<T> list, int layerMask = -1, QueryTriggerInteraction triggerInteraction = 2) where T : Component
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		Profiler.BeginSample("Vis.Components");
		Buffer(position, radius, layerMask, triggerInteraction);
		for (int i = 0; i < colCount; i++)
		{
			Collider val = colBuffer[i];
			if (!((Object)(object)val == (Object)null) && val.enabled)
			{
				T component = ((Component)val).GetComponent<T>();
				if (!((Object)(object)component == (Object)null))
				{
					list.Add(component);
				}
			}
		}
		Profiler.EndSample();
	}

	public static void Entities<T>(Vector3 position, float radius, List<T> list, int layerMask = -1, QueryTriggerInteraction triggerInteraction = 2) where T : class
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		Profiler.BeginSample("Vis.Entities");
		Buffer(position, radius, layerMask, triggerInteraction);
		for (int i = 0; i < colCount; i++)
		{
			Collider val = colBuffer[i];
			if (!((Object)(object)val == (Object)null) && val.enabled && val.ToBaseEntity() is T item)
			{
				list.Add(item);
			}
		}
		Profiler.EndSample();
	}

	public static void EntityComponents<T>(Vector3 position, float radius, List<T> list, int layerMask = -1, QueryTriggerInteraction triggerInteraction = 2) where T : EntityComponentBase
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		Profiler.BeginSample("Vis.EntityComponents");
		Buffer(position, radius, layerMask, triggerInteraction);
		for (int i = 0; i < colCount; i++)
		{
			Collider val = colBuffer[i];
			if ((Object)(object)val == (Object)null || !val.enabled)
			{
				continue;
			}
			BaseEntity baseEntity = val.ToBaseEntity();
			if (!((Object)(object)baseEntity == (Object)null))
			{
				T component = ((Component)baseEntity).GetComponent<T>();
				if (!((Object)(object)component == (Object)null))
				{
					list.Add(component);
				}
			}
		}
		Profiler.EndSample();
	}

	private static void Buffer(OBB bounds, int layerMask = -1, QueryTriggerInteraction triggerInteraction = 2)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		Profiler.BeginSample("Vis.Buffer");
		Profiler.BeginSample("HandleIgnoreCollision");
		layerMask = GamePhysics.HandleIgnoreCollision(bounds.position, layerMask);
		Profiler.EndSample();
		int num = colCount;
		Profiler.BeginSample("OverlapBox");
		colCount = Physics.OverlapBoxNonAlloc(bounds.position, bounds.extents, colBuffer, bounds.rotation, layerMask, triggerInteraction);
		Profiler.EndSample();
		for (int i = colCount; i < num; i++)
		{
			colBuffer[i] = null;
		}
		if (colCount >= colBuffer.Length)
		{
			Debug.LogWarning((object)"Vis query is exceeding collider buffer length.");
			colCount = colBuffer.Length;
		}
		Profiler.EndSample();
	}

	public static void Colliders<T>(OBB bounds, List<T> list, int layerMask = -1, QueryTriggerInteraction triggerInteraction = 2) where T : Collider
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		Profiler.BeginSample("Vis.Colliders");
		Buffer(bounds, layerMask, triggerInteraction);
		for (int i = 0; i < colCount; i++)
		{
			Collider obj = colBuffer[i];
			T val = (T)(object)((obj is T) ? obj : null);
			if (!((Object)(object)val == (Object)null) && ((Collider)val).enabled)
			{
				list.Add(val);
			}
		}
		Profiler.EndSample();
	}

	public static void Components<T>(OBB bounds, List<T> list, int layerMask = -1, QueryTriggerInteraction triggerInteraction = 2) where T : Component
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		Profiler.BeginSample("Vis.Components");
		Buffer(bounds, layerMask, triggerInteraction);
		for (int i = 0; i < colCount; i++)
		{
			Collider val = colBuffer[i];
			if (!((Object)(object)val == (Object)null) && val.enabled)
			{
				T component = ((Component)val).GetComponent<T>();
				if (!((Object)(object)component == (Object)null))
				{
					list.Add(component);
				}
			}
		}
		Profiler.EndSample();
	}

	public static void Entities<T>(OBB bounds, List<T> list, int layerMask = -1, QueryTriggerInteraction triggerInteraction = 2) where T : BaseEntity
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		Profiler.BeginSample("Vis.Entities");
		Buffer(bounds, layerMask, triggerInteraction);
		for (int i = 0; i < colCount; i++)
		{
			Collider val = colBuffer[i];
			if (!((Object)(object)val == (Object)null) && val.enabled)
			{
				T val2 = val.ToBaseEntity() as T;
				if (!((Object)(object)val2 == (Object)null))
				{
					list.Add(val2);
				}
			}
		}
		Profiler.EndSample();
	}

	public static void EntityComponents<T>(OBB bounds, List<T> list, int layerMask = -1, QueryTriggerInteraction triggerInteraction = 2) where T : EntityComponentBase
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		Profiler.BeginSample("Vis.EntityComponents");
		Buffer(bounds, layerMask, triggerInteraction);
		for (int i = 0; i < colCount; i++)
		{
			Collider val = colBuffer[i];
			if ((Object)(object)val == (Object)null || !val.enabled)
			{
				continue;
			}
			BaseEntity baseEntity = val.ToBaseEntity();
			if (!((Object)(object)baseEntity == (Object)null))
			{
				T component = ((Component)baseEntity).GetComponent<T>();
				if (!((Object)(object)component == (Object)null))
				{
					list.Add(component);
				}
			}
		}
		Profiler.EndSample();
	}

	private static void Buffer(Vector3 startPosition, Vector3 endPosition, float radius, int layerMask = -1, QueryTriggerInteraction triggerInteraction = 2)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		Profiler.BeginSample("Vis.Buffer");
		Profiler.BeginSample("HandleIgnoreCollision");
		layerMask = GamePhysics.HandleIgnoreCollision(startPosition, layerMask);
		Profiler.EndSample();
		int num = colCount;
		Profiler.BeginSample("OverlapCapsule");
		colCount = Physics.OverlapCapsuleNonAlloc(startPosition, endPosition, radius, colBuffer, layerMask, triggerInteraction);
		Profiler.EndSample();
		for (int i = colCount; i < num; i++)
		{
			colBuffer[i] = null;
		}
		if (colCount >= colBuffer.Length)
		{
			Debug.LogWarning((object)"Vis query is exceeding collider buffer length.");
			colCount = colBuffer.Length;
		}
		Profiler.EndSample();
	}

	public static void Colliders<T>(Vector3 startPosition, Vector3 endPosition, float radius, List<T> list, int layerMask = -1, QueryTriggerInteraction triggerInteraction = 2) where T : Collider
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		Profiler.BeginSample("Vis.Colliders");
		Buffer(startPosition, endPosition, radius, layerMask, triggerInteraction);
		for (int i = 0; i < colCount; i++)
		{
			Collider obj = colBuffer[i];
			T val = (T)(object)((obj is T) ? obj : null);
			if (!((Object)(object)val == (Object)null) && ((Collider)val).enabled)
			{
				list.Add(val);
			}
		}
		Profiler.EndSample();
	}

	public static void Components<T>(Vector3 startPosition, Vector3 endPosition, float radius, List<T> list, int layerMask = -1, QueryTriggerInteraction triggerInteraction = 2) where T : Component
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		Profiler.BeginSample("Vis.Components");
		Buffer(startPosition, endPosition, radius, layerMask, triggerInteraction);
		for (int i = 0; i < colCount; i++)
		{
			Collider val = colBuffer[i];
			if (!((Object)(object)val == (Object)null) && val.enabled)
			{
				T component = ((Component)val).GetComponent<T>();
				if (!((Object)(object)component == (Object)null))
				{
					list.Add(component);
				}
			}
		}
		Profiler.EndSample();
	}

	public static void Entities<T>(Vector3 startPosition, Vector3 endPosition, float radius, List<T> list, int layerMask = -1, QueryTriggerInteraction triggerInteraction = 2) where T : BaseEntity
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		Profiler.BeginSample("Vis.Entities");
		Buffer(startPosition, endPosition, radius, layerMask, triggerInteraction);
		for (int i = 0; i < colCount; i++)
		{
			Collider val = colBuffer[i];
			if (!((Object)(object)val == (Object)null) && val.enabled)
			{
				T val2 = val.ToBaseEntity() as T;
				if (!((Object)(object)val2 == (Object)null))
				{
					list.Add(val2);
				}
			}
		}
		Profiler.EndSample();
	}

	public static void EntityComponents<T>(Vector3 startPosition, Vector3 endPosition, float radius, List<T> list, int layerMask = -1, QueryTriggerInteraction triggerInteraction = 2) where T : EntityComponentBase
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		Profiler.BeginSample("Vis.EntityComponents");
		Buffer(startPosition, endPosition, radius, layerMask, triggerInteraction);
		for (int i = 0; i < colCount; i++)
		{
			Collider val = colBuffer[i];
			if ((Object)(object)val == (Object)null || !val.enabled)
			{
				continue;
			}
			BaseEntity baseEntity = val.ToBaseEntity();
			if (!((Object)(object)baseEntity == (Object)null))
			{
				T component = ((Component)baseEntity).GetComponent<T>();
				if (!((Object)(object)component == (Object)null))
				{
					list.Add(component);
				}
			}
		}
		Profiler.EndSample();
	}
}
