using System.Collections.Generic;
using Facepunch;
using UnityEngine;
using UnityEngine.Profiling;

public class NPCDwelling : BaseEntity
{
	public NPCSpawner npcSpawner;

	public float NPCSpawnChance = 1f;

	public SpawnGroup[] spawnGroups;

	public AIMovePoint[] movePoints;

	public AICoverPoint[] coverPoints;

	public override void ServerInit()
	{
		base.ServerInit();
		UpdateInformationZone(remove: false);
		if ((Object)(object)npcSpawner != (Object)null && Random.Range(0f, 1f) <= NPCSpawnChance)
		{
			npcSpawner.SpawnInitial();
		}
		SpawnGroup[] array = spawnGroups;
		foreach (SpawnGroup spawnGroup in array)
		{
			spawnGroup.SpawnInitial();
		}
	}

	public override void DestroyShared()
	{
		if (base.isServer)
		{
			CleanupSpawned();
		}
		base.DestroyShared();
		if (base.isServer)
		{
			UpdateInformationZone(remove: true);
		}
	}

	public bool ValidateAIPoint(Vector3 pos)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		((Component)this).gameObject.SetActive(false);
		bool result = !GamePhysics.CheckSphere(pos + Vector3.up * 0.6f, 0.5f, 65537, (QueryTriggerInteraction)0);
		((Component)this).gameObject.SetActive(true);
		return result;
	}

	public void UpdateInformationZone(bool remove)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		Profiler.BeginSample("NPCDwelling.UpdateInformationZone");
		AIInformationZone forPoint = AIInformationZone.GetForPoint(((Component)this).transform.position);
		if (!((Object)(object)forPoint == (Object)null))
		{
			if (remove)
			{
				forPoint.RemoveDynamicAIPoints(movePoints, coverPoints);
			}
			else
			{
				forPoint.AddDynamicAIPoints(movePoints, coverPoints, ValidateAIPoint);
			}
			Profiler.EndSample();
		}
	}

	public void CheckDespawn()
	{
		if (!PlayersNearby() && (!Object.op_Implicit((Object)(object)npcSpawner) || npcSpawner.currentPopulation <= 0))
		{
			CleanupSpawned();
			Kill();
		}
	}

	public void CleanupSpawned()
	{
		if (spawnGroups != null)
		{
			SpawnGroup[] array = spawnGroups;
			foreach (SpawnGroup spawnGroup in array)
			{
				spawnGroup.Clear();
			}
		}
		if (Object.op_Implicit((Object)(object)npcSpawner))
		{
			npcSpawner.Clear();
		}
	}

	public bool PlayersNearby()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		List<BasePlayer> list = Pool.GetList<BasePlayer>();
		Vis.Entities(((Component)this).transform.position, TimeoutPlayerCheckRadius(), list, 131072, (QueryTriggerInteraction)2);
		bool result = false;
		foreach (BasePlayer item in list)
		{
			if (!item.IsSleeping() && item.IsAlive())
			{
				result = true;
				break;
			}
		}
		Pool.FreeList<BasePlayer>(ref list);
		return result;
	}

	public virtual float TimeoutPlayerCheckRadius()
	{
		return 10f;
	}
}
