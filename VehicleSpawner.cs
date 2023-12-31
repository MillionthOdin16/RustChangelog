using System;
using System.Collections.Generic;
using Facepunch;
using Facepunch.Rust;
using UnityEngine;

public class VehicleSpawner : BaseEntity
{
	public interface IVehicleSpawnUser
	{
		string ShortPrefabName { get; }

		bool IsClient { get; }

		bool IsDestroyed { get; }

		void SetupOwner(BasePlayer owner, Vector3 newSafeAreaOrigin, float newSafeAreaRadius);

		bool IsDespawnEligable();

		EntityFuelSystem GetFuelSystem();

		int StartingFuelUnits();

		void Kill(DestroyMode mode);
	}

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

	public IVehicleSpawnUser GetVehicleOccupying()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		IVehicleSpawnUser result = null;
		List<IVehicleSpawnUser> list = Pool.GetList<IVehicleSpawnUser>();
		Vis.Entities(((Component)spawnOffset).transform.position, occupyRadius, list, GetOccupyLayer(), (QueryTriggerInteraction)1);
		if (list.Count > 0)
		{
			result = list[0];
		}
		Pool.FreeList<IVehicleSpawnUser>(ref list);
		return result;
	}

	public bool IsPadOccupied()
	{
		IVehicleSpawnUser vehicleOccupying = GetVehicleOccupying();
		if (vehicleOccupying != null)
		{
			return !vehicleOccupying.IsDespawnEligable();
		}
		return false;
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

	public IVehicleSpawnUser SpawnVehicle(string prefabToSpawn, BasePlayer newOwner)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		CleanupArea(cleanupRadius);
		NudgePlayersInRadius(spawnNudgeRadius);
		BaseEntity baseEntity = GameManager.server.CreateEntity(prefabToSpawn, ((Component)spawnOffset).transform.position, ((Component)spawnOffset).transform.rotation);
		baseEntity.Spawn();
		IVehicleSpawnUser component = ((Component)baseEntity).GetComponent<IVehicleSpawnUser>();
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
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		List<IVehicleSpawnUser> list = Pool.GetList<IVehicleSpawnUser>();
		Vis.Entities(((Component)spawnOffset).transform.position, radius, list, 32768, (QueryTriggerInteraction)2);
		foreach (IVehicleSpawnUser item in list)
		{
			if (!item.IsClient && !item.IsDestroyed)
			{
				item.Kill(DestroyMode.None);
			}
		}
		List<ServerGib> list2 = Pool.GetList<ServerGib>();
		Vis.Entities(((Component)spawnOffset).transform.position, radius, list2, -2147483647, (QueryTriggerInteraction)2);
		foreach (ServerGib item2 in list2)
		{
			if (!item2.isClient)
			{
				item2.Kill();
			}
		}
		Pool.FreeList<IVehicleSpawnUser>(ref list);
		Pool.FreeList<ServerGib>(ref list2);
	}

	public void NudgePlayersInRadius(float radius)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
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
