using System;
using UnityEngine;
using UnityEngine.Profiling;

public class TimeCachedValue<T>
{
	public float refreshCooldown;

	public float refreshRandomRange;

	public Func<T> updateValue;

	private T cachedValue;

	private TimeSince cooldown;

	private bool hasRun;

	private bool forceNextRun;

	public T Get(bool force)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		if (TimeSince.op_Implicit(cooldown) < refreshCooldown && !force && hasRun && !forceNextRun)
		{
			Profiler.BeginSample("TimeCachedValue.Cache");
			Profiler.EndSample();
			return cachedValue;
		}
		Profiler.BeginSample("TimeCachedValue.UpdateValue");
		hasRun = true;
		forceNextRun = false;
		cooldown = TimeSince.op_Implicit(0f - Random.Range(0f, refreshRandomRange));
		if (updateValue != null)
		{
			cachedValue = updateValue();
		}
		else
		{
			cachedValue = default(T);
		}
		Profiler.EndSample();
		return cachedValue;
	}

	public void ForceNextRun()
	{
		forceNextRun = true;
	}
}
