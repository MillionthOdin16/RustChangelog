using UnityEngine;

public abstract class BaseSpawnPoint : MonoBehaviour, IServerComponent
{
	public abstract void GetLocation(out Vector3 pos, out Quaternion rot);

	public abstract void ObjectSpawned(SpawnPointInstance instance);

	public abstract void ObjectRetired(SpawnPointInstance instance);

	public virtual bool IsAvailableTo(GameObjectRef prefabRef)
	{
		return ((Component)this).gameObject.activeSelf;
	}

	public virtual bool HasPlayersIntersecting()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return BaseNetworkable.HasCloseConnections(((Component)this).transform.position, 2f);
	}

	protected void DropToGround(ref Vector3 pos, ref Quaternion rot)
	{
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
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
