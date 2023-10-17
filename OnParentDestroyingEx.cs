using System.Collections.Generic;
using Facepunch;
using UnityEngine;
using UnityEngine.Profiling;

public static class OnParentDestroyingEx
{
	public static void BroadcastOnParentDestroying(this GameObject go)
	{
		Profiler.BeginSample("OnParentDestroying");
		List<IOnParentDestroying> list = Pool.GetList<IOnParentDestroying>();
		go.GetComponentsInChildren<IOnParentDestroying>(list);
		for (int i = 0; i < list.Count; i++)
		{
			list[i].OnParentDestroying();
		}
		Pool.FreeList<IOnParentDestroying>(ref list);
		Profiler.EndSample();
	}

	public static void SendOnParentDestroying(this GameObject go)
	{
		Profiler.BeginSample("OnParentDestroying");
		List<IOnParentDestroying> list = Pool.GetList<IOnParentDestroying>();
		go.GetComponents<IOnParentDestroying>(list);
		for (int i = 0; i < list.Count; i++)
		{
			list[i].OnParentDestroying();
		}
		Pool.FreeList<IOnParentDestroying>(ref list);
		Profiler.EndSample();
	}
}
