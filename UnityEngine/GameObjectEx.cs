using System.Collections.Generic;
using Facepunch;
using Rust;
using Rust.Registry;

namespace UnityEngine;

public static class GameObjectEx
{
	public static BaseEntity ToBaseEntity(this GameObject go)
	{
		return go.transform.ToBaseEntity();
	}

	public static BaseEntity ToBaseEntity(this Collider collider)
	{
		return ((Component)collider).transform.ToBaseEntity();
	}

	public static BaseEntity ToBaseEntity(this Transform transform)
	{
		IEntity val = GetEntityFromRegistry(transform);
		if (val == null && !((Component)transform).gameObject.activeInHierarchy)
		{
			val = GetEntityFromComponent(transform);
		}
		return val as BaseEntity;
	}

	public static bool IsOnLayer(this GameObject go, Layer rustLayer)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected I4, but got Unknown
		return go.IsOnLayer((int)rustLayer);
	}

	public static bool IsOnLayer(this GameObject go, int layer)
	{
		if ((Object)(object)go != (Object)null)
		{
			return go.layer == layer;
		}
		return false;
	}

	private static IEntity GetEntityFromRegistry(Transform transform)
	{
		Transform val = transform;
		IEntity val2 = Entity.Get(val);
		while (val2 == null && (Object)(object)val.parent != (Object)null)
		{
			val = val.parent;
			val2 = Entity.Get(val);
		}
		if (val2 != null && !val2.IsDestroyed)
		{
			return val2;
		}
		return null;
	}

	private static IEntity GetEntityFromComponent(Transform transform)
	{
		Transform val = transform;
		IEntity component = ((Component)val).GetComponent<IEntity>();
		while (component == null && (Object)(object)val.parent != (Object)null)
		{
			val = val.parent;
			component = ((Component)val).GetComponent<IEntity>();
		}
		if (component != null && !component.IsDestroyed)
		{
			return component;
		}
		return null;
	}

	public static void SetHierarchyGroup(this GameObject obj, string strRoot, bool groupActive = true, bool persistant = false)
	{
		obj.transform.SetParent(HierarchyUtil.GetRoot(strRoot, groupActive, persistant).transform, true);
	}

	public static bool HasComponent<T>(this GameObject obj) where T : Component
	{
		return (Object)(object)obj.GetComponent<T>() != (Object)null;
	}

	public static void SetChildComponentsEnabled<T>(this GameObject gameObject, bool enabled) where T : MonoBehaviour
	{
		List<T> list = Pool.GetList<T>();
		gameObject.GetComponentsInChildren<T>(true, list);
		foreach (T item in list)
		{
			((Behaviour)(object)item).enabled = enabled;
		}
		Pool.FreeList<T>(ref list);
	}
}
