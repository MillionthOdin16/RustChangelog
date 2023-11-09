using UnityEngine;
using UnityEngine.Profiling;

public class TerrainCollisionTrigger : EnvironmentVolumeTrigger
{
	protected void OnTriggerEnter(Collider other)
	{
		if (Object.op_Implicit((Object)(object)TerrainMeta.Collision) && !other.isTrigger)
		{
			Profiler.BeginSample("TerrainCollisionTrigger.OnTriggerEnter");
			UpdateCollider(other, state: true);
			Profiler.EndSample();
		}
	}

	protected void OnTriggerExit(Collider other)
	{
		if (Object.op_Implicit((Object)(object)TerrainMeta.Collision) && !other.isTrigger)
		{
			Profiler.BeginSample("TerrainCollisionTrigger.OnTriggerExit");
			UpdateCollider(other, state: false);
			Profiler.EndSample();
		}
	}

	private void UpdateCollider(Collider other, bool state)
	{
		TerrainMeta.Collision.SetIgnore(other, base.volume.trigger, state);
		TerrainCollisionProxy component = ((Component)other).GetComponent<TerrainCollisionProxy>();
		if (Object.op_Implicit((Object)(object)component))
		{
			for (int i = 0; i < component.colliders.Length; i++)
			{
				TerrainMeta.Collision.SetIgnore((Collider)(object)component.colliders[i], base.volume.trigger, state);
			}
		}
	}
}
