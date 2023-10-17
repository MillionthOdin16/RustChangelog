using System.Collections.Generic;
using Rust;
using UnityEngine;

public class WaterVisibilityTrigger : EnvironmentVolumeTrigger
{
	public bool togglePhysics = true;

	public bool toggleVisuals = true;

	private long enteredTick;

	private static long ticks = 1L;

	private static SortedList<long, WaterVisibilityTrigger> tracker = new SortedList<long, WaterVisibilityTrigger>();

	public static void Reset()
	{
		ticks = 1L;
		tracker.Clear();
	}

	protected void OnDestroy()
	{
		if (!Application.isQuitting)
		{
			tracker.Remove(enteredTick);
		}
	}

	private void ToggleVisibility()
	{
	}

	private void ResetVisibility()
	{
	}

	private void ToggleCollision(Collider other)
	{
		if (togglePhysics && (Object)(object)WaterSystem.Collision != (Object)null)
		{
			WaterSystem.Collision.SetIgnore(other, base.volume.trigger);
		}
	}

	private void ResetCollision(Collider other)
	{
		if (togglePhysics && (Object)(object)WaterSystem.Collision != (Object)null)
		{
			WaterSystem.Collision.SetIgnore(other, base.volume.trigger, ignore: false);
		}
	}

	protected void OnTriggerEnter(Collider other)
	{
		bool num = (Object)(object)((Component)other).gameObject.GetComponent<PlayerWalkMovement>() != (Object)null;
		bool flag = ((Component)other).gameObject.CompareTag("MainCamera");
		if ((num || flag) && !tracker.ContainsValue(this))
		{
			enteredTick = ticks++;
			tracker.Add(enteredTick, this);
			ToggleVisibility();
		}
		if (!flag && !other.isTrigger)
		{
			ToggleCollision(other);
		}
	}

	protected void OnTriggerExit(Collider other)
	{
		bool num = (Object)(object)((Component)other).gameObject.GetComponent<PlayerWalkMovement>() != (Object)null;
		bool flag = ((Component)other).gameObject.CompareTag("MainCamera");
		if ((num || flag) && tracker.ContainsValue(this))
		{
			tracker.Remove(enteredTick);
			if (tracker.Count > 0)
			{
				tracker.Values[tracker.Count - 1].ToggleVisibility();
			}
			else
			{
				ResetVisibility();
			}
		}
		if (!flag && !other.isTrigger)
		{
			ResetCollision(other);
		}
	}
}
