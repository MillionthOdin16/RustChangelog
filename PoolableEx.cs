using UnityEngine;
using UnityEngine.Profiling;

public static class PoolableEx
{
	public static bool SupportsPoolingInParent(this GameObject gameObject)
	{
		Profiler.BeginSample("GameObject.SupportsPooling", (Object)(object)gameObject);
		Profiler.BeginSample("GetComponent", (Object)(object)gameObject);
		Poolable componentInParent = gameObject.GetComponentInParent<Poolable>();
		Profiler.EndSample();
		bool result = (Object)(object)componentInParent != (Object)null && componentInParent.prefabID != 0;
		Profiler.EndSample();
		return result;
	}

	public static bool SupportsPooling(this GameObject gameObject)
	{
		Profiler.BeginSample("GameObject.SupportsPooling", (Object)(object)gameObject);
		Profiler.BeginSample("GetComponent", (Object)(object)gameObject);
		Poolable component = gameObject.GetComponent<Poolable>();
		Profiler.EndSample();
		bool result = (Object)(object)component != (Object)null && component.prefabID != 0;
		Profiler.EndSample();
		return result;
	}

	public static void AwakeFromInstantiate(this GameObject gameObject)
	{
		Profiler.BeginSample("GameObject.AwakeFromInstantiate", (Object)(object)gameObject);
		if (gameObject.activeSelf)
		{
			Profiler.BeginSample("GetComponent", (Object)(object)gameObject);
			Poolable component = gameObject.GetComponent<Poolable>();
			Profiler.EndSample();
			component.SetBehaviourEnabled(state: true);
		}
		else
		{
			gameObject.SetActive(true);
		}
		Profiler.EndSample();
	}
}
