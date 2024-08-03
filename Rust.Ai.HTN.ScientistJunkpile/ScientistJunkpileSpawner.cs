using System;
using System.Collections.Generic;
using ConVar;
using UnityEngine;

namespace Rust.Ai.HTN.ScientistJunkpile;

public class ScientistJunkpileSpawner : MonoBehaviour, IServerComponent, ISpawnGroup
{
	public enum JunkpileType
	{
		A,
		B,
		C,
		D,
		E,
		F,
		G
	}

	public GameObjectRef ScientistPrefab;

	[NonSerialized]
	public List<BaseCombatEntity> Spawned = new List<BaseCombatEntity>();

	[NonSerialized]
	public BaseSpawnPoint[] SpawnPoints;

	public int MaxPopulation = 1;

	public bool InitialSpawn;

	public float MinRespawnTimeMinutes = 120f;

	public float MaxRespawnTimeMinutes = 120f;

	public float MovementRadius = -1f;

	public bool ReducedLongRangeAccuracy = false;

	public JunkpileType SpawnType;

	[Range(0f, 1f)]
	public float SpawnBaseChance = 1f;

	private float nextRespawnTime = 0f;

	private bool pendingRespawn = false;

	public int currentPopulation => Spawned.Count;

	private void Awake()
	{
		SpawnPoints = ((Component)this).GetComponentsInChildren<BaseSpawnPoint>();
		if (Object.op_Implicit((Object)(object)SingletonComponent<SpawnHandler>.Instance))
		{
			SingletonComponent<SpawnHandler>.Instance.SpawnGroups.Add((ISpawnGroup)this);
		}
	}

	protected void OnDestroy()
	{
		if (Object.op_Implicit((Object)(object)SingletonComponent<SpawnHandler>.Instance))
		{
			SingletonComponent<SpawnHandler>.Instance.SpawnGroups.Remove((ISpawnGroup)this);
		}
		else
		{
			Debug.LogWarning((object)(((object)this).GetType().Name + ": SpawnHandler instance not found."));
		}
	}

	public void Fill()
	{
		DoRespawn();
	}

	public void Clear()
	{
		if (Spawned == null)
		{
			return;
		}
		foreach (BaseCombatEntity item in Spawned)
		{
			if (!((Object)(object)item == (Object)null) && !((Object)(object)((Component)item).gameObject == (Object)null) && !((Object)(object)((Component)item).transform == (Object)null))
			{
				BaseEntity baseEntity = ((Component)item).gameObject.ToBaseEntity();
				if (Object.op_Implicit((Object)(object)baseEntity))
				{
					baseEntity.Kill();
				}
			}
		}
		Spawned.Clear();
	}

	public void SpawnInitial()
	{
		nextRespawnTime = Time.time + Random.Range(3f, 4f);
		pendingRespawn = true;
	}

	public void SpawnRepeating()
	{
		CheckIfRespawnNeeded();
	}

	public void CheckIfRespawnNeeded()
	{
		if (!pendingRespawn)
		{
			if (Spawned == null || Spawned.Count == 0 || IsAllSpawnedDead())
			{
				ScheduleRespawn();
			}
		}
		else if ((Spawned == null || Spawned.Count == 0 || IsAllSpawnedDead()) && Time.time >= nextRespawnTime)
		{
			DoRespawn();
		}
	}

	private bool IsAllSpawnedDead()
	{
		int num = 0;
		while (num < Spawned.Count)
		{
			BaseCombatEntity baseCombatEntity = Spawned[num];
			if ((Object)(object)baseCombatEntity == (Object)null || (Object)(object)((Component)baseCombatEntity).transform == (Object)null || baseCombatEntity.IsDestroyed || baseCombatEntity.IsDead())
			{
				Spawned.RemoveAt(num);
				num--;
				num++;
				continue;
			}
			return false;
		}
		return true;
	}

	public void ScheduleRespawn()
	{
		nextRespawnTime = Time.time + Random.Range(MinRespawnTimeMinutes, MaxRespawnTimeMinutes) * 60f;
		pendingRespawn = true;
	}

	public void DoRespawn()
	{
		if (!Application.isLoading && !Application.isLoadingSave)
		{
			SpawnScientist();
		}
		pendingRespawn = false;
	}

	public void SpawnScientist()
	{
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		if (!ConVar.AI.npc_enable || Spawned == null || Spawned.Count >= MaxPopulation)
		{
			return;
		}
		float num = SpawnBaseChance;
		switch (SpawnType)
		{
		case JunkpileType.A:
			num = ConVar.AI.npc_junkpile_a_spawn_chance;
			break;
		case JunkpileType.G:
			num = ConVar.AI.npc_junkpile_g_spawn_chance;
			break;
		}
		if (Random.value > num)
		{
			return;
		}
		int num2 = MaxPopulation - Spawned.Count;
		for (int i = 0; i < num2; i++)
		{
			Vector3 pos;
			Quaternion rot;
			BaseSpawnPoint spawnPoint = GetSpawnPoint(out pos, out rot);
			if (!((Object)(object)spawnPoint == (Object)null))
			{
				BaseEntity baseEntity = GameManager.server.CreateEntity(ScientistPrefab.resourcePath, pos, rot, startActive: false);
				if (!((Object)(object)baseEntity != (Object)null))
				{
					break;
				}
				baseEntity.enableSaving = false;
				((Component)baseEntity).gameObject.AwakeFromInstantiate();
				baseEntity.Spawn();
				Spawned.Add((BaseCombatEntity)baseEntity);
			}
		}
	}

	private BaseSpawnPoint GetSpawnPoint(out Vector3 pos, out Quaternion rot)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		BaseSpawnPoint baseSpawnPoint = null;
		pos = Vector3.zero;
		rot = Quaternion.identity;
		int num = Random.Range(0, SpawnPoints.Length);
		for (int i = 0; i < SpawnPoints.Length; i++)
		{
			baseSpawnPoint = SpawnPoints[(num + i) % SpawnPoints.Length];
			if (Object.op_Implicit((Object)(object)baseSpawnPoint) && ((Component)baseSpawnPoint).gameObject.activeSelf)
			{
				break;
			}
		}
		if (Object.op_Implicit((Object)(object)baseSpawnPoint))
		{
			baseSpawnPoint.GetLocation(out pos, out rot);
		}
		return baseSpawnPoint;
	}
}
