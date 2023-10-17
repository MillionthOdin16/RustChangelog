using System.Collections;
using System.Collections.Generic;
using Facepunch;

namespace UnityEngine;

public static class CoroutineEx
{
	public static WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();

	public static WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();

	private static Dictionary<float, WaitForSeconds> waitForSecondsBuffer = new Dictionary<float, WaitForSeconds>();

	public static WaitForSeconds waitForSeconds(float seconds)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Expected O, but got Unknown
		if (!waitForSecondsBuffer.TryGetValue(seconds, out var value))
		{
			value = new WaitForSeconds(seconds);
			waitForSecondsBuffer.Add(seconds, value);
		}
		return value;
	}

	public static WaitForSecondsRealtimeEx waitForSecondsRealtime(float seconds)
	{
		WaitForSecondsRealtimeEx val = Pool.Get<WaitForSecondsRealtimeEx>();
		val.WaitTime = seconds;
		return val;
	}

	public static IEnumerator Combine(params IEnumerator[] coroutines)
	{
		while (true)
		{
			bool completed = true;
			foreach (IEnumerator coroutine in coroutines)
			{
				if (coroutine != null && coroutine.MoveNext())
				{
					completed = false;
				}
			}
			if (completed)
			{
				break;
			}
			yield return waitForEndOfFrame;
		}
	}
}
