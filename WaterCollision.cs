using System;
using System.Collections.Generic;
using UnityEngine;

public class WaterCollision : MonoBehaviour
{
	private ListDictionary<Collider, List<Collider>> ignoredColliders;

	private HashSet<Collider> waterColliders;

	private void Awake()
	{
		ignoredColliders = new ListDictionary<Collider, List<Collider>>();
		waterColliders = new HashSet<Collider>();
	}

	public void Clear()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		if (waterColliders.Count == 0)
		{
			return;
		}
		HashSet<Collider>.Enumerator enumerator = waterColliders.GetEnumerator();
		while (enumerator.MoveNext())
		{
			Enumerator<Collider> enumerator2 = ignoredColliders.Keys.GetEnumerator();
			try
			{
				while (enumerator2.MoveNext())
				{
					Physics.IgnoreCollision(enumerator2.Current, enumerator.Current, false);
				}
			}
			finally
			{
				((IDisposable)enumerator2).Dispose();
			}
		}
		ignoredColliders.Clear();
	}

	public void Reset(Collider collider)
	{
		if (waterColliders.Count != 0 && Object.op_Implicit((Object)(object)collider))
		{
			HashSet<Collider>.Enumerator enumerator = waterColliders.GetEnumerator();
			while (enumerator.MoveNext())
			{
				Physics.IgnoreCollision(collider, enumerator.Current, false);
			}
			ignoredColliders.Remove(collider);
		}
	}

	public bool GetIgnore(Vector3 pos, float radius = 0.01f)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		return GamePhysics.CheckSphere<WaterVisibilityTrigger>(pos, radius, 262144, (QueryTriggerInteraction)2);
	}

	public bool GetIgnore(Bounds bounds)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		return GamePhysics.CheckBounds<WaterVisibilityTrigger>(bounds, 262144, (QueryTriggerInteraction)2);
	}

	public bool GetIgnore(Vector3 start, Vector3 end, float radius)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return GamePhysics.CheckCapsule<WaterVisibilityTrigger>(start, end, radius, 262144, (QueryTriggerInteraction)2);
	}

	public bool GetIgnore(RaycastHit hit)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (waterColliders.Contains(((RaycastHit)(ref hit)).collider))
		{
			return GetIgnore(((RaycastHit)(ref hit)).point);
		}
		return false;
	}

	public bool GetIgnore(Collider collider)
	{
		if (waterColliders.Count == 0 || !Object.op_Implicit((Object)(object)collider))
		{
			return false;
		}
		return ignoredColliders.Contains(collider);
	}

	public void SetIgnore(Collider collider, Collider trigger, bool ignore = true)
	{
		if (waterColliders.Count == 0 || !Object.op_Implicit((Object)(object)collider))
		{
			return;
		}
		if (!GetIgnore(collider))
		{
			if (ignore)
			{
				List<Collider> list = new List<Collider> { trigger };
				HashSet<Collider>.Enumerator enumerator = waterColliders.GetEnumerator();
				while (enumerator.MoveNext())
				{
					Physics.IgnoreCollision(collider, enumerator.Current, true);
				}
				ignoredColliders.Add(collider, list);
			}
			return;
		}
		List<Collider> list2 = ignoredColliders[collider];
		if (ignore)
		{
			if (!list2.Contains(trigger))
			{
				list2.Add(trigger);
			}
		}
		else if (list2.Contains(trigger))
		{
			list2.Remove(trigger);
		}
	}

	protected void LateUpdate()
	{
		for (int i = 0; i < ignoredColliders.Count; i++)
		{
			KeyValuePair<Collider, List<Collider>> byIndex = ignoredColliders.GetByIndex(i);
			Collider key = byIndex.Key;
			List<Collider> value = byIndex.Value;
			if ((Object)(object)key == (Object)null)
			{
				ignoredColliders.RemoveAt(i--);
			}
			else if (value.Count == 0)
			{
				HashSet<Collider>.Enumerator enumerator = waterColliders.GetEnumerator();
				while (enumerator.MoveNext())
				{
					Physics.IgnoreCollision(key, enumerator.Current, false);
				}
				ignoredColliders.RemoveAt(i--);
			}
		}
	}
}
