using System.Collections.Generic;
using UnityEngine;

public abstract class BaseSpawnPoint : MonoBehaviour, IServerComponent
{
	public enum SpawnPointType
	{
		Normal,
		Tugboat
	}

	public SpawnPointType spawnPointType;

	public static Dictionary<SpawnPointType, List<BaseSpawnPoint>> spawnPoints = new Dictionary<SpawnPointType, List<BaseSpawnPoint>>();

	public abstract void GetLocation(out Vector3 pos, out Quaternion rot);

	public abstract void ObjectSpawned(SpawnPointInstance instance);

	public abstract void ObjectRetired(SpawnPointInstance instance);

	protected void OnEnable()
	{
		if (spawnPointType != 0)
		{
			if (spawnPoints.TryGetValue(spawnPointType, out var value))
			{
				value.Add(this);
				return;
			}
			spawnPoints[spawnPointType] = new List<BaseSpawnPoint> { this };
		}
	}

	protected void OnDisable()
	{
		if (spawnPointType != 0 && spawnPoints.TryGetValue(spawnPointType, out var value))
		{
			value.Remove(this);
		}
	}

	public virtual bool IsAvailableTo(GameObjectRef prefabRef)
	{
		return ((Component)this).gameObject.activeSelf;
	}

	public virtual bool HasPlayersIntersecting()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return BaseNetworkable.HasCloseConnections(((Component)this).transform.position, 2f);
	}

	protected void DropToGround(ref Vector3 pos, ref Quaternion rot)
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		if (Object.op_Implicit((Object)(object)TerrainMeta.HeightMap) && Object.op_Implicit((Object)(object)TerrainMeta.Collision) && !TerrainMeta.Collision.GetIgnore(pos))
		{
			float height = TerrainMeta.HeightMap.GetHeight(pos);
			pos.y = Mathf.Max(pos.y, height);
		}
		if (TransformUtil.GetGroundInfo(pos, out var hitOut, 20f, LayerMask.op_Implicit(1235288065)))
		{
			pos = ((RaycastHit)(ref hitOut)).point;
			rot = Quaternion.LookRotation(rot * Vector3.forward, ((RaycastHit)(ref hitOut)).normal);
		}
	}
}
