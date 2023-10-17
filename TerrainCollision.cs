using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class TerrainCollision : TerrainExtension
{
	private ListDictionary<Collider, List<Collider>> ignoredColliders;

	private TerrainCollider terrainCollider;

	public override void Setup()
	{
		ignoredColliders = new ListDictionary<Collider, List<Collider>>();
		terrainCollider = ((Component)terrain).GetComponent<TerrainCollider>();
	}

	public void Clear()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		if (!Object.op_Implicit((Object)(object)terrainCollider))
		{
			return;
		}
		Enumerator<Collider> enumerator = ignoredColliders.Keys.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Collider current = enumerator.Current;
				Physics.IgnoreCollision(current, (Collider)(object)terrainCollider, false);
			}
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
		ignoredColliders.Clear();
	}

	public void Reset(Collider collider)
	{
		if (Object.op_Implicit((Object)(object)terrainCollider) && Object.op_Implicit((Object)(object)collider))
		{
			Physics.IgnoreCollision(collider, (Collider)(object)terrainCollider, false);
			ignoredColliders.Remove(collider);
		}
	}

	public bool GetIgnore(Vector3 pos, float radius = 0.01f)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		Profiler.BeginSample("TerrainCollision.GetIgnore");
		bool result = GamePhysics.CheckSphere<TerrainCollisionTrigger>(pos, radius, 262144, (QueryTriggerInteraction)2);
		Profiler.EndSample();
		return result;
	}

	public bool GetIgnore(RaycastHit hit)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		return ((RaycastHit)(ref hit)).collider is TerrainCollider && GetIgnore(((RaycastHit)(ref hit)).point);
	}

	public bool GetIgnore(Collider collider)
	{
		if (!Object.op_Implicit((Object)(object)terrainCollider) || !Object.op_Implicit((Object)(object)collider))
		{
			return false;
		}
		return ignoredColliders.Contains(collider);
	}

	public void SetIgnore(Collider collider, Collider trigger, bool ignore = true)
	{
		if (!Object.op_Implicit((Object)(object)terrainCollider) || !Object.op_Implicit((Object)(object)collider))
		{
			return;
		}
		if (!GetIgnore(collider))
		{
			if (ignore)
			{
				List<Collider> list = new List<Collider> { trigger };
				Physics.IgnoreCollision(collider, (Collider)(object)terrainCollider, true);
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
		if (ignoredColliders == null)
		{
			return;
		}
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
				Physics.IgnoreCollision(key, (Collider)(object)terrainCollider, false);
				ignoredColliders.RemoveAt(i--);
			}
		}
	}
}
