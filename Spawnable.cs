using Facepunch;
using ProtoBuf;
using Rust;
using UnityEngine;

public class Spawnable : MonoBehaviour, IServerComponent
{
	[ReadOnly]
	public SpawnPopulation Population;

	[SerializeField]
	private bool ForceSpawnOnly;

	[SerializeField]
	private string ForceSpawnInfoMessage = string.Empty;

	internal uint PrefabID;

	internal bool SpawnIndividual;

	internal Vector3 SpawnPosition;

	internal Quaternion SpawnRotation;

	protected void OnEnable()
	{
		if (!Application.isLoadingSave)
		{
			Add();
		}
	}

	protected void OnDisable()
	{
		if (!Application.isQuitting && !Application.isLoadingSave)
		{
			Remove();
		}
	}

	private void Add()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		SpawnPosition = ((Component)this).transform.position;
		SpawnRotation = ((Component)this).transform.rotation;
		if (!Object.op_Implicit((Object)(object)SingletonComponent<SpawnHandler>.Instance))
		{
			return;
		}
		if (Population != null)
		{
			SingletonComponent<SpawnHandler>.Instance.AddInstance(this);
		}
		else if (Application.isLoading && !Application.isLoadingSave)
		{
			BaseEntity component = ((Component)this).GetComponent<BaseEntity>();
			if ((Object)(object)component != (Object)null && component.enableSaving && !component.syncPosition)
			{
				SingletonComponent<SpawnHandler>.Instance.AddRespawn(new SpawnIndividual(component.prefabID, SpawnPosition, SpawnRotation));
			}
		}
	}

	private void Remove()
	{
		if (Object.op_Implicit((Object)(object)SingletonComponent<SpawnHandler>.Instance) && Population != null)
		{
			SingletonComponent<SpawnHandler>.Instance.RemoveInstance(this);
		}
	}

	internal void Save(BaseNetworkable.SaveInfo info)
	{
		if (!(Population == null))
		{
			info.msg.spawnable = Pool.Get<Spawnable>();
			info.msg.spawnable.population = Population.FilenameStringId;
		}
	}

	internal void Load(BaseNetworkable.LoadInfo info)
	{
		if (info.msg.spawnable != null)
		{
			Population = FileSystem.Load<SpawnPopulation>(StringPool.Get(info.msg.spawnable.population), true);
		}
		Add();
	}

	protected void OnValidate()
	{
		Population = null;
	}
}
