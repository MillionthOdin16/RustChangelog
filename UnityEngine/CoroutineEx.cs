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
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		if (!waitForSecondsBuffer.TryGetValue(seconds, out var value))
		{
			value = new WaitForSeconds(seconds);
			waitForSecondsBuffer.Add(seconds, value);
		}
		return value;
	}

	public static WaitForSecondsRealtimeEx waitForSecondsRealtime(float seconds)
	{
		WaitForSecondsRealtimeEx obj = Pool.Get<WaitForSecondsRealtimeEx>();
		obj.WaitTime = seconds;
		return obj;
	}

	public static IEnumerator Combine(params IEnumerator[] coroutines)
	{
		while (true)
		{
			bool flag = true;
			foreach (IEnumerator enumerator in coroutines)
			{
				if (enumerator != null && enumerator.MoveNext())
				{
					flag = false;
				}
			}
			if (flag)
			{
				break;
			}
			yield return waitForEndOfFrame;
		}
	}
}
