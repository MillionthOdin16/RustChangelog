using Rust;
using UnityEngine;

public class SpawnPointInstance : MonoBehaviour
{
	internal ISpawnPointUser parentSpawnPointUser;

	internal BaseSpawnPoint parentSpawnPoint;

	public void Notify()
	{
		if (!parentSpawnPointUser.IsUnityNull())
		{
			parentSpawnPointUser.ObjectSpawned(this);
		}
		if (Object.op_Implicit((Object)(object)parentSpawnPoint))
		{
			parentSpawnPoint.ObjectSpawned(this);
		}
	}

	public void Retire()
	{
		if (!parentSpawnPointUser.IsUnityNull())
		{
			parentSpawnPointUser.ObjectRetired(this);
		}
		if (Object.op_Implicit((Object)(object)parentSpawnPoint))
		{
			parentSpawnPoint.ObjectRetired(this);
		}
	}

	protected void OnDestroy()
	{
		if (!Application.isQuitting)
		{
			Retire();
		}
	}
}
