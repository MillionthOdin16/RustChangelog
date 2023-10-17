using System;
using System.Collections.Generic;
using Facepunch;
using Facepunch.Rust;
using UnityEngine;

public class VehicleSpawner : BaseEntity
{
	[Serializable]
	public class SpawnPair
	{
		public string message;

		public GameObjectRef prefabToSpawn;
	}

	public float spawnNudgeRadius = 6f;

	public float cleanupRadius = 10f;

	public float occupyRadius = 5f;

	public SpawnPair[] objectsToSpawn;

	public Transform spawnOffset;

	public float safeRadius = 10f;

	protected virtual bool LogAnalytics => true;

	public virtual int GetOccupyLayer()
	{
		return 32768;
	}

	public BaseVehicle GetVehicleOccupying()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		BaseVehicle result = null;
		List<BaseVehicle> list = Pool.GetList<BaseVehicle>();
		Vis.Entities(((Component)spawnOffset).transform.position, occupyRadius, list, GetOccupyLayer(), (QueryTriggerInteraction)1);
		if (list.Count > 0)
		{
			result = list[0];
		}
		Pool.FreeList<BaseVehicle>(ref list);
		return result;
	}

	public bool IsPadOccupied()
	{
		BaseVehicle vehicleOccupying = GetVehicleOccupying();
		return (Object)(object)vehicleOccupying != (Object)null && !vehicleOccupying.IsDespawnEligable();
	}

	public override void OnEntityMessage(BaseEntity from, string msg)
	{
		BasePlayer newOwner = null;
		NPCTalking component = ((Component)from).GetComponent<NPCTalking>();
		if (Object.op_Implicit((Object)(object)component))
		{
			newOwner = component.GetActionPlayer();
		}
		SpawnPair[] array = objectsToSpawn;
		foreach (SpawnPair spawnPair in array)
		{
			if (msg == spawnPair.message)
			{
				SpawnVehicle(spawnPair.prefabToSpawn.resourcePath, newOwner);
				break;
			}
		}
	}

	public BaseVehicle SpawnVehicle(string prefabToSpawn, BasePlayer newOwner)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		CleanupArea(cleanupRadius);
		NudgePlayersInRadius(spawnNudgeRadius);
		BaseEntity baseEntity = GameManager.server.CreateEntity(prefabToSpawn, ((Component)spawnOffset).transform.position, ((Component)spawnOffset).transform.rotation);
		baseEntity.Spawn();
		BaseVehicle component = ((Component)baseEntity).GetComponent<BaseVehicle>();
		if ((Object)(object)newOwner != (Object)null)
		{
			component.SetupOwner(newOwner, ((Component)spawnOffset).transform.position, safeRadius);
		}
		VehicleSpawnPoint.AddStartingFuel(component);
		if (LogAnalytics)
		{
			Analytics.Server.VehiclePurchased(component.ShortPrefabName);
		}
		if ((Object)(object)newOwner != (Object)null)
		{
			Analytics.Azure.OnVehiclePurchased(newOwner, baseEntity);
		}
		return component;
	}

	public void CleanupArea(float radius)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		List<BaseVehicle> list = Pool.GetList<BaseVehicle>();
		Vis.Entities(((Component)spawnOffset).transform.position, radius, list, 32768, (QueryTriggerInteraction)2);
		foreach (BaseVehicle item in list)
		{
			if (!item.isClient && !item.IsDestroyed)
			{
				item.Kill();
			}
		}
		List<ServerGib> list2 = Pool.GetList<ServerGib>();
		Vis.Entities(((Component)spawnOffset).transform.position, radius, list2, 67108865, (QueryTriggerInteraction)2);
		foreach (ServerGib item2 in list2)
		{
			if (!item2.isClient)
			{
				item2.Kill();
			}
		}
		Pool.FreeList<BaseVehicle>(ref list);
		Pool.FreeList<ServerGib>(ref list2);
	}

	public void NudgePlayersInRadius(float radius)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		List<BasePlayer> list = Pool.GetList<BasePlayer>();
		Vis.Entities(((Component)spawnOffset).transform.position, radius, list, 131072, (QueryTriggerInteraction)2);
		foreach (BasePlayer item in list)
		{
			if (!item.IsNpc && !item.isMounted && item.IsConnected)
			{
				Vector3 position = ((Component)spawnOffset).transform.position;
				position += Vector3Ex.Direction2D(((Component)item).transform.position, ((Component)spawnOffset).transform.position) * radius;
				position += Vector3.up * 0.1f;
				item.MovePosition(position);
				item.ClientRPCPlayer<Vector3>(null, item, "ForcePositionTo", position);
			}
		}
		Pool.FreeList<BasePlayer>(ref list);
	}
}
