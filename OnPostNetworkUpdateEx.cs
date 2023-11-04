using System.Collections.Generic;
using Facepunch;
using UnityEngine;
using UnityEngine.Profiling;

public static class OnPostNetworkUpdateEx
{
	public static void BroadcastOnPostNetworkUpdate(this GameObject go, BaseEntity entity)
	{
		Profiler.BeginSample("BroadcastOnPostNetworkUpdate.GetComponents");
		List<IOnPostNetworkUpdate> list = Pool.GetList<IOnPostNetworkUpdate>();
		Profiler.EndSample();
		Profiler.BeginSample("BroadcastOnPostNetworkUpdate");
		go.GetComponentsInChildren<IOnPostNetworkUpdate>(list);
		for (int i = 0; i < list.Count; i++)
		{
			list[i].OnPostNetworkUpdate(entity);
		}
		Pool.FreeList<IOnPostNetworkUpdate>(ref list);
		Profiler.EndSample();
	}

	public static void SendOnPostNetworkUpdate(this GameObject go, BaseEntity entity)
	{
		Profiler.BeginSample("OnPostNetworkUpdate");
		List<IOnPostNetworkUpdate> list = Pool.GetList<IOnPostNetworkUpdate>();
		go.GetComponents<IOnPostNetworkUpdate>(list);
		for (int i = 0; i < list.Count; i++)
		{
			list[i].OnPostNetworkUpdate(entity);
		}
		Pool.FreeList<IOnPostNetworkUpdate>(ref list);
		Profiler.EndSample();
	}
}
