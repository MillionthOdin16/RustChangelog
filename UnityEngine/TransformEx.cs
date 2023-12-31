using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Facepunch;

namespace UnityEngine;

public static class TransformEx
{
	public static string GetRecursiveName(this Transform transform, string strEndName = "")
	{
		string text = ((Object)transform).name;
		if (!string.IsNullOrEmpty(strEndName))
		{
			text = text + "/" + strEndName;
		}
		if ((Object)(object)transform.parent != (Object)null)
		{
			text = transform.parent.GetRecursiveName(text);
		}
		return text;
	}

	public static void RemoveComponent<T>(this Transform transform) where T : Component
	{
		T component = ((Component)transform).GetComponent<T>();
		if (!((Object)(object)component == (Object)null))
		{
			GameManager.Destroy((Component)(object)component);
		}
	}

	public static void RetireAllChildren(this Transform transform, GameManager gameManager)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Expected O, but got Unknown
		List<GameObject> list = Pool.GetList<GameObject>();
		foreach (Transform item in transform)
		{
			Transform val = item;
			if (!((Component)val).CompareTag("persist"))
			{
				list.Add(((Component)val).gameObject);
			}
		}
		foreach (GameObject item2 in list)
		{
			gameManager.Retire(item2);
		}
		Pool.FreeList<GameObject>(ref list);
	}

	public static List<Transform> GetChildren(this Transform transform)
	{
		return ((IEnumerable)transform).Cast<Transform>().ToList();
	}

	public static void OrderChildren(this Transform tx, Func<Transform, object> selector)
	{
		foreach (Transform item in ((IEnumerable)tx).Cast<Transform>().OrderBy(selector))
		{
			item.SetAsLastSibling();
		}
	}

	public static List<Transform> GetAllChildren(this Transform transform)
	{
		List<Transform> list = new List<Transform>();
		if ((Object)(object)transform != (Object)null)
		{
			transform.AddAllChildren(list);
		}
		return list;
	}

	public static void AddAllChildren(this Transform transform, List<Transform> list)
	{
		list.Add(transform);
		for (int i = 0; i < transform.childCount; i++)
		{
			Transform child = transform.GetChild(i);
			if (!((Object)(object)child == (Object)null))
			{
				child.AddAllChildren(list);
			}
		}
	}

	public static Transform[] GetChildrenWithTag(this Transform transform, string strTag)
	{
		return (from x in transform.GetAllChildren()
			where ((Component)x).CompareTag(strTag)
			select x).ToArray();
	}

	public static Matrix4x4 LocalToPrefabRoot(this Transform transform)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		Matrix4x4 val = Matrix4x4.identity;
		while ((Object)(object)transform.parent != (Object)null)
		{
			val *= Matrix4x4.TRS(transform.localPosition, transform.localRotation, transform.localScale);
			transform = transform.parent;
		}
		return val;
	}

	public static void Identity(this GameObject go)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		go.transform.localPosition = Vector3.zero;
		go.transform.localRotation = Quaternion.identity;
		go.transform.localScale = Vector3.one;
	}

	public static GameObject CreateChild(this GameObject go)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		//IL_001d: Expected O, but got Unknown
		GameObject val = new GameObject();
		val.transform.parent = go.transform;
		Identity(val);
		return val;
	}

	public static GameObject InstantiateChild(this GameObject go, GameObject prefab)
	{
		GameObject obj = Instantiate.GameObject(prefab, (Transform)null);
		obj.transform.SetParent(go.transform, false);
		obj.Identity();
		return obj;
	}

	public static void SetLayerRecursive(this GameObject go, int Layer)
	{
		if (go.layer != Layer)
		{
			go.layer = Layer;
		}
		for (int i = 0; i < go.transform.childCount; i++)
		{
			((Component)go.transform.GetChild(i)).gameObject.SetLayerRecursive(Layer);
		}
	}

	public static bool DropToGround(this Transform transform, bool alignToNormal = false, float fRange = 100f)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		if (transform.GetGroundInfo(out var pos, out var normal, fRange))
		{
			transform.position = pos;
			if (alignToNormal)
			{
				transform.rotation = Quaternion.LookRotation(transform.forward, normal);
			}
			return true;
		}
		return false;
	}

	public static bool GetGroundInfo(this Transform transform, out Vector3 pos, out Vector3 normal, float range = 100f)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return TransformUtil.GetGroundInfo(transform.position, out pos, out normal, range, transform);
	}

	public static bool GetGroundInfoTerrainOnly(this Transform transform, out Vector3 pos, out Vector3 normal, float range = 100f)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return TransformUtil.GetGroundInfoTerrainOnly(transform.position, out pos, out normal, range);
	}

	public static Bounds WorkoutRenderBounds(this Transform tx)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		Bounds bounds = default(Bounds);
		((Bounds)(ref bounds))._002Ector(Vector3.zero, Vector3.zero);
		Renderer[] componentsInChildren = ((Component)tx).GetComponentsInChildren<Renderer>();
		foreach (Renderer val in componentsInChildren)
		{
			if (!(val is ParticleSystemRenderer))
			{
				if (((Bounds)(ref bounds)).center == Vector3.zero)
				{
					bounds = val.bounds;
				}
				else
				{
					((Bounds)(ref bounds)).Encapsulate(val.bounds);
				}
			}
		}
		return bounds;
	}

	public static List<T> GetSiblings<T>(this Transform transform, bool includeSelf = false)
	{
		List<T> list = new List<T>();
		if ((Object)(object)transform.parent == (Object)null)
		{
			return list;
		}
		for (int i = 0; i < transform.parent.childCount; i++)
		{
			Transform child = transform.parent.GetChild(i);
			if (includeSelf || !((Object)(object)child == (Object)(object)transform))
			{
				T component = ((Component)child).GetComponent<T>();
				if (component != null)
				{
					list.Add(component);
				}
			}
		}
		return list;
	}

	public static void DestroyChildren(this Transform transform)
	{
		for (int i = 0; i < transform.childCount; i++)
		{
			GameManager.Destroy(((Component)transform.GetChild(i)).gameObject);
		}
	}

	public static void SetChildrenActive(this Transform transform, bool b)
	{
		for (int i = 0; i < transform.childCount; i++)
		{
			((Component)transform.GetChild(i)).gameObject.SetActive(b);
		}
	}

	public static Transform ActiveChild(this Transform transform, string name, bool bDisableOthers)
	{
		Transform result = null;
		for (int i = 0; i < transform.childCount; i++)
		{
			Transform child = transform.GetChild(i);
			if (((Object)child).name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
			{
				result = child;
				((Component)child).gameObject.SetActive(true);
			}
			else if (bDisableOthers)
			{
				((Component)child).gameObject.SetActive(false);
			}
		}
		return result;
	}

	public static T GetComponentInChildrenIncludeDisabled<T>(this Transform transform) where T : Component
	{
		List<T> list = Pool.GetList<T>();
		((Component)transform).GetComponentsInChildren<T>(true, list);
		T result = ((list.Count > 0) ? list[0] : default(T));
		Pool.FreeList<T>(ref list);
		return result;
	}

	public static bool HasComponentInChildrenIncludeDisabled<T>(this Transform transform) where T : Component
	{
		List<T> list = Pool.GetList<T>();
		((Component)transform).GetComponentsInChildren<T>(true, list);
		bool result = list.Count > 0;
		Pool.FreeList<T>(ref list);
		return result;
	}

	public static void SetHierarchyGroup(this Transform transform, string strRoot, bool groupActive = true, bool persistant = false)
	{
		transform.SetParent(HierarchyUtil.GetRoot(strRoot, groupActive, persistant).transform, true);
	}

	public static Bounds GetBounds(this Transform transform, bool includeRenderers = true, bool includeColliders = true, bool includeInactive = true)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		Bounds result = default(Bounds);
		((Bounds)(ref result))._002Ector(Vector3.zero, Vector3.zero);
		if (includeRenderers)
		{
			MeshFilter[] componentsInChildren = ((Component)transform).GetComponentsInChildren<MeshFilter>(includeInactive);
			foreach (MeshFilter val in componentsInChildren)
			{
				if (Object.op_Implicit((Object)(object)val.sharedMesh))
				{
					Matrix4x4 matrix = transform.worldToLocalMatrix * ((Component)val).transform.localToWorldMatrix;
					Bounds bounds = val.sharedMesh.bounds;
					((Bounds)(ref result)).Encapsulate(bounds.Transform(matrix));
				}
			}
			SkinnedMeshRenderer[] componentsInChildren2 = ((Component)transform).GetComponentsInChildren<SkinnedMeshRenderer>(includeInactive);
			foreach (SkinnedMeshRenderer val2 in componentsInChildren2)
			{
				if (Object.op_Implicit((Object)(object)val2.sharedMesh))
				{
					Matrix4x4 matrix2 = transform.worldToLocalMatrix * ((Component)val2).transform.localToWorldMatrix;
					Bounds bounds2 = val2.sharedMesh.bounds;
					((Bounds)(ref result)).Encapsulate(bounds2.Transform(matrix2));
				}
			}
		}
		if (includeColliders)
		{
			MeshCollider[] componentsInChildren3 = ((Component)transform).GetComponentsInChildren<MeshCollider>(includeInactive);
			foreach (MeshCollider val3 in componentsInChildren3)
			{
				if (Object.op_Implicit((Object)(object)val3.sharedMesh) && !((Collider)val3).isTrigger)
				{
					Matrix4x4 matrix3 = transform.worldToLocalMatrix * ((Component)val3).transform.localToWorldMatrix;
					Bounds bounds3 = val3.sharedMesh.bounds;
					((Bounds)(ref result)).Encapsulate(bounds3.Transform(matrix3));
				}
			}
		}
		return result;
	}
}
