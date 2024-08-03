using System.Collections.Generic;
using UnityEngine;

public class RoadBradleys : TriggeredEvent
{
	public List<BradleyAPC> spawnedAPCs = new List<BradleyAPC>();

	public int GetNumBradleys()
	{
		CleanList();
		return spawnedAPCs.Count;
	}

	public int GetDesiredNumber()
	{
		return Mathf.CeilToInt((float)World.Size / 1000f) * 2;
	}

	private void CleanList()
	{
		for (int num = spawnedAPCs.Count - 1; num >= 0; num--)
		{
			BradleyAPC bradleyAPC = spawnedAPCs[num];
			if ((Object)(object)bradleyAPC == (Object)null)
			{
				spawnedAPCs.RemoveAt(num);
			}
		}
	}

	private void RunEvent()
	{
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		int numBradleys = GetNumBradleys();
		int num = GetDesiredNumber() - numBradleys;
		if (num <= 0 || (Object)(object)TerrainMeta.Path == (Object)null || TerrainMeta.Path.Roads.Count == 0)
		{
			return;
		}
		Debug.Log((object)("Spawning :" + num + "Bradleys"));
		for (int i = 0; i < num; i++)
		{
			Vector3 zero = Vector3.zero;
			PathList pathList = TerrainMeta.Path.Roads[Random.Range(0, TerrainMeta.Path.Roads.Count)];
			zero = pathList.Path.Points[Random.Range(0, pathList.Path.Points.Length)];
			BradleyAPC bradleyAPC = BradleyAPC.SpawnRoadDrivingBradley(zero, Quaternion.identity);
			if (Object.op_Implicit((Object)(object)bradleyAPC))
			{
				spawnedAPCs.Add(bradleyAPC);
			}
			else
			{
				Debug.Log((object)("Failed to spawn bradley at: " + zero));
			}
		}
	}
}
