using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class DeployVolume : PrefabAttribute
{
	public enum EntityMode
	{
		ExcludeList,
		IncludeList
	}

	public LayerMask layers = LayerMask.op_Implicit(537001984);

	[InspectorFlags]
	public ColliderInfo.Flags ignore = (ColliderInfo.Flags)0;

	public EntityMode entityMode = EntityMode.ExcludeList;

	[FormerlySerializedAs("entities")]
	public BaseEntity[] entityList;

	[SerializeField]
	public EntityListScriptableObject[] entityGroups;

	public bool IsBuildingBlock { get; set; }

	public static Collider LastDeployHit { get; private set; } = null;


	protected override Type GetIndexedType()
	{
		return typeof(DeployVolume);
	}

	public override void PreProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		base.PreProcess(preProcess, rootObj, name, serverside, clientside, bundling);
		IsBuildingBlock = (Object)(object)rootObj.GetComponent<BuildingBlock>() != (Object)null;
	}

	protected abstract bool Check(Vector3 position, Quaternion rotation, int mask = -1);

	protected abstract bool Check(Vector3 position, Quaternion rotation, OBB test, int mask = -1);

	public static bool Check(Vector3 position, Quaternion rotation, DeployVolume[] volumes, int mask = -1)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < volumes.Length; i++)
		{
			if (volumes[i].Check(position, rotation, mask))
			{
				return true;
			}
		}
		return false;
	}

	public static bool Check(Vector3 position, Quaternion rotation, DeployVolume[] volumes, OBB test, int mask = -1)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < volumes.Length; i++)
		{
			if (volumes[i].Check(position, rotation, test, mask))
			{
				return true;
			}
		}
		return false;
	}

	public static bool CheckSphere(Vector3 pos, float radius, int layerMask, DeployVolume volume)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		List<Collider> list = Pool.GetList<Collider>();
		GamePhysics.OverlapSphere(pos, radius, list, layerMask, (QueryTriggerInteraction)2);
		bool result = CheckFlags(list, volume);
		Pool.FreeList<Collider>(ref list);
		return result;
	}

	public static bool CheckCapsule(Vector3 start, Vector3 end, float radius, int layerMask, DeployVolume volume)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		List<Collider> list = Pool.GetList<Collider>();
		GamePhysics.OverlapCapsule(start, end, radius, list, layerMask, (QueryTriggerInteraction)2);
		bool result = CheckFlags(list, volume);
		Pool.FreeList<Collider>(ref list);
		return result;
	}

	public static bool CheckOBB(OBB obb, int layerMask, DeployVolume volume)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		List<Collider> list = Pool.GetList<Collider>();
		GamePhysics.OverlapOBB(obb, list, layerMask, (QueryTriggerInteraction)2);
		bool result = CheckFlags(list, volume);
		Pool.FreeList<Collider>(ref list);
		return result;
	}

	public static bool CheckBounds(Bounds bounds, int layerMask, DeployVolume volume)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		List<Collider> list = Pool.GetList<Collider>();
		GamePhysics.OverlapBounds(bounds, list, layerMask, (QueryTriggerInteraction)2);
		bool result = CheckFlags(list, volume);
		Pool.FreeList<Collider>(ref list);
		return result;
	}

	private static bool CheckFlags(List<Collider> list, DeployVolume volume)
	{
		LastDeployHit = null;
		for (int i = 0; i < list.Count; i++)
		{
			LastDeployHit = list[i];
			GameObject gameObject = ((Component)list[i]).gameObject;
			if (gameObject.CompareTag("DeployVolumeIgnore"))
			{
				continue;
			}
			ColliderInfo component = gameObject.GetComponent<ColliderInfo>();
			if (((Object)(object)component != (Object)null && component.HasFlag(ColliderInfo.Flags.OnlyBlockBuildingBlock) && !volume.IsBuildingBlock) || (!((Object)(object)component == (Object)null) && volume.ignore != 0 && component.HasFlag(volume.ignore)))
			{
				continue;
			}
			if (volume.entityList.Length == 0 && volume.entityGroups.Length == 0)
			{
				return true;
			}
			BaseEntity entity = list[i].ToBaseEntity();
			bool flag = false;
			if (volume.entityGroups != null)
			{
				EntityListScriptableObject[] array = volume.entityGroups;
				foreach (EntityListScriptableObject entityListScriptableObject in array)
				{
					if (entityListScriptableObject.entities == null || entityListScriptableObject.entities.Length == 0)
					{
						Debug.LogWarning((object)("Skipping entity group '" + ((Object)entityListScriptableObject).name + "' when checking volume: there are no entities"));
						continue;
					}
					if (CheckEntityList(entity, entityListScriptableObject.entities, entityListScriptableObject.whitelist))
					{
						flag = true;
						continue;
					}
					return false;
				}
			}
			if (CheckEntityList(entity, volume.entityList, volume.entityMode == EntityMode.IncludeList))
			{
				return true;
			}
			return false;
		}
		return false;
	}

	private static bool CheckEntityList(BaseEntity entity, BaseEntity[] entities, bool whitelist)
	{
		if (entities == null || entities.Length == 0)
		{
			return true;
		}
		bool flag = false;
		if ((Object)(object)entity != (Object)null)
		{
			foreach (BaseEntity baseEntity in entities)
			{
				if (entity.prefabID == baseEntity.prefabID)
				{
					flag = true;
					break;
				}
			}
		}
		if (whitelist)
		{
			return flag;
		}
		return !flag;
	}
}
