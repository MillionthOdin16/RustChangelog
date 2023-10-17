using System.Collections.Generic;
using Facepunch;
using UnityEngine;
using UnityEngine.Profiling;

public static class OnSendNetworkUpdateEx
{
	public static void BroadcastOnSendNetworkUpdate(this GameObject go, BaseEntity entity)
	{
		Profiler.BeginSample("OnSendNetworkUpdate");
		List<IOnSendNetworkUpdate> list = Pool.GetList<IOnSendNetworkUpdate>();
		go.GetComponentsInChildren<IOnSendNetworkUpdate>(list);
		for (int i = 0; i < list.Count; i++)
		{
			list[i].OnSendNetworkUpdate(entity);
		}
		Pool.FreeList<IOnSendNetworkUpdate>(ref list);
		Profiler.EndSample();
	}

	public static void SendOnSendNetworkUpdate(this GameObject go, BaseEntity entity)
	{
		Profiler.BeginSample("OnSendNetworkUpdate");
		List<IOnSendNetworkUpdate> list = Pool.GetList<IOnSendNetworkUpdate>();
		go.GetComponents<IOnSendNetworkUpdate>(list);
		for (int i = 0; i < list.Count; i++)
		{
			list[i].OnSendNetworkUpdate(entity);
		}
		Pool.FreeList<IOnSendNetworkUpdate>(ref list);
		Profiler.EndSample();
	}
}
