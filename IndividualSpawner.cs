using UnityEngine;

public class IndividualSpawner : BaseMonoBehaviour, IServerComponent, ISpawnPointUser, ISpawnGroup
{
	public GameObjectRef entityPrefab;

	public float respawnDelayMin = 10f;

	public float respawnDelayMax = 20f;

	public bool useCustomBoundsCheckMask;

	public LayerMask customBoundsCheckMask;

	[Tooltip("Simply spawns the entity once. No respawning. Entity can be saved if desired.")]
	[SerializeField]
	private bool oneTimeSpawner;

	internal bool isSpawnerActive = true;

	private SpawnPointInstance spawnInstance;

	private float nextSpawnTime = -1f;

	public int currentPopulation
	{
		get
		{
			if (!((Object)(object)spawnInstance == (Object)null))
			{
				return 1;
			}
			return 0;
		}
	}

	private bool IsSpawned => (Object)(object)spawnInstance != (Object)null;

	protected void Awake()
	{
		if (Object.op_Implicit((Object)(object)SingletonComponent<SpawnHandler>.Instance))
		{
			SingletonComponent<SpawnHandler>.Instance.SpawnGroups.Add(this);
		}
		else
		{
			Debug.LogWarning((object)(((object)this).GetType().Name + ": SpawnHandler instance not found."));
		}
	}

	protected void OnDrawGizmosSelected()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		if (TryGetEntityBounds(out var result))
		{
			Gizmos.color = Color.yellow;
			Gizmos.matrix = ((Component)this).transform.localToWorldMatrix;
			Gizmos.DrawCube(((Bounds)(ref result)).center, ((Bounds)(ref result)).size);
		}
	}

	public void ObjectSpawned(SpawnPointInstance instance)
	{
		spawnInstance = instance;
	}

	public void ObjectRetired(SpawnPointInstance instance)
	{
		spawnInstance = null;
		nextSpawnTime = Time.time + Random.Range(respawnDelayMin, respawnDelayMax);
	}

	public void Fill()
	{
		if (!oneTimeSpawner)
		{
			TrySpawnEntity();
		}
	}

	public void SpawnInitial()
	{
		TrySpawnEntity();
	}

	public void Clear()
	{
		if (IsSpawned)
		{
			BaseEntity baseEntity = ((Component)spawnInstance).gameObject.ToBaseEntity();
			if ((Object)(object)baseEntity != (Object)null)
			{
				baseEntity.Kill();
			}
		}
	}

	public void SpawnRepeating()
	{
		if (!IsSpawned && !oneTimeSpawner && Time.time >= nextSpawnTime)
		{
			TrySpawnEntity();
		}
	}

	public bool HasSpaceToSpawn()
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		if (useCustomBoundsCheckMask)
		{
			return SpawnHandler.CheckBounds(entityPrefab.Get(), ((Component)this).transform.position, ((Component)this).transform.rotation, Vector3.one, customBoundsCheckMask);
		}
		return SingletonComponent<SpawnHandler>.Instance.CheckBounds(entityPrefab.Get(), ((Component)this).transform.position, ((Component)this).transform.rotation, Vector3.one);
	}

	private void TrySpawnEntity()
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		if (!isSpawnerActive || IsSpawned)
		{
			return;
		}
		if (!HasSpaceToSpawn())
		{
			nextSpawnTime = Time.time + Random.Range(respawnDelayMin, respawnDelayMax);
			return;
		}
		BaseEntity baseEntity = GameManager.server.CreateEntity(entityPrefab.resourcePath, ((Component)this).transform.position, ((Component)this).transform.rotation, startActive: false);
		if ((Object)(object)baseEntity != (Object)null)
		{
			if (!oneTimeSpawner)
			{
				baseEntity.enableSaving = false;
			}
			((Component)baseEntity).gameObject.AwakeFromInstantiate();
			baseEntity.Spawn();
			SpawnPointInstance spawnPointInstance = ((Component)baseEntity).gameObject.AddComponent<SpawnPointInstance>();
			spawnPointInstance.parentSpawnPointUser = this;
			spawnPointInstance.Notify();
		}
		else
		{
			Debug.LogError((object)"IndividualSpawner failed to spawn entity.", (Object)(object)((Component)this).gameObject);
		}
	}

	private bool TryGetEntityBounds(out Bounds result)
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		if (entityPrefab != null)
		{
			GameObject val = entityPrefab.Get();
			if ((Object)(object)val != (Object)null)
			{
				BaseEntity component = val.GetComponent<BaseEntity>();
				if ((Object)(object)component != (Object)null)
				{
					result = component.bounds;
					return true;
				}
			}
		}
		result = default(Bounds);
		return false;
	}
}
