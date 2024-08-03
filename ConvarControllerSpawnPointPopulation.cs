using System.Collections.Generic;
using UnityEngine;

public class ConvarControllerSpawnPointPopulation : ConvarControlledSpawnPopulation
{
	public BaseSpawnPoint.SpawnPointType spawnPointType;

	private List<BaseSpawnPoint> spawnPoints;

	public override bool GetSpawnPosOverride(Prefab<Spawnable> prefab, ref Vector3 newPos, ref Quaternion newRot)
	{
		if (spawnPoints == null)
		{
			TryGetSpawnPoints(out spawnPoints);
		}
		if (spawnPoints == null || spawnPoints.Count == 0)
		{
			return false;
		}
		int num = Random.Range(0, spawnPoints.Count);
		for (int i = 0; i < spawnPoints.Count; i++)
		{
			num++;
			if (num >= spawnPoints.Count)
			{
				num = 0;
			}
			BaseSpawnPoint baseSpawnPoint = spawnPoints[num];
			prefab = Prefabs[Random.Range(0, Prefabs.Length)];
			if ((Object)(object)baseSpawnPoint != (Object)null && baseSpawnPoint.IsAvailableTo(prefab.Object))
			{
				baseSpawnPoint.GetLocation(out newPos, out newRot);
				return true;
			}
		}
		return false;
	}

	private bool TryGetSpawnPoints(out List<BaseSpawnPoint> result)
	{
		return BaseSpawnPoint.spawnPoints.TryGetValue(spawnPointType, out result);
	}
}
