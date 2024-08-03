using System.Collections.Generic;
using Facepunch;
using UnityEngine;
using UnityEngine.Profiling;

public static class OnParentSpawningEx
{
	public static void BroadcastOnParentSpawning(this GameObject go)
	{
		Profiler.BeginSample("OnParentSpawning");
		List<IOnParentSpawning> list = Pool.GetList<IOnParentSpawning>();
		go.GetComponentsInChildren<IOnParentSpawning>(list);
		for (int i = 0; i < list.Count; i++)
		{
			list[i].OnParentSpawning();
		}
		Pool.FreeList<IOnParentSpawning>(ref list);
		Profiler.EndSample();
	}

	public static void SendOnParentSpawning(this GameObject go)
	{
		Profiler.BeginSample("OnParentSpawning");
		List<IOnParentSpawning> list = Pool.GetList<IOnParentSpawning>();
		go.GetComponents<IOnParentSpawning>(list);
		for (int i = 0; i < list.Count; i++)
		{
			list[i].OnParentSpawning();
		}
		Pool.FreeList<IOnParentSpawning>(ref list);
		Profiler.EndSample();
	}
}
