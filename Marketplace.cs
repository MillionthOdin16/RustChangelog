using System;
using System.Collections.Generic;
using Facepunch;
using ProtoBuf;
using Rust;
using UnityEngine;

public class Marketplace : BaseEntity
{
	[Header("Marketplace")]
	public GameObjectRef terminalPrefab;

	public Transform[] terminalPoints;

	public Transform droneLaunchPoint;

	public GameObjectRef deliveryDronePrefab;

	public EntityRef<MarketTerminal>[] terminalEntities;

	public NetworkableId SendDrone(BasePlayer player, MarketTerminal sourceTerminal, VendingMachine vendingMachine)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)sourceTerminal == (Object)null || (Object)(object)vendingMachine == (Object)null)
		{
			return default(NetworkableId);
		}
		BaseEntity baseEntity = GameManager.server.CreateEntity(deliveryDronePrefab?.resourcePath, droneLaunchPoint.position, droneLaunchPoint.rotation);
		if (!(baseEntity is DeliveryDrone deliveryDrone))
		{
			baseEntity.Kill();
			return default(NetworkableId);
		}
		deliveryDrone.OwnerID = player.userID;
		deliveryDrone.Spawn();
		deliveryDrone.Setup(this, sourceTerminal, vendingMachine);
		return deliveryDrone.net.ID;
	}

	public void ReturnDrone(DeliveryDrone deliveryDrone)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		if (deliveryDrone.sourceTerminal.TryGet(serverside: true, out var entity))
		{
			entity.CompleteOrder(deliveryDrone.targetVendingMachine.uid);
		}
		deliveryDrone.Kill();
	}

	public override void Spawn()
	{
		base.Spawn();
		if (!Application.isLoadingSave)
		{
			SpawnSubEntities();
		}
	}

	private void SpawnSubEntities()
	{
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		if (!base.isServer)
		{
			return;
		}
		if (terminalEntities != null && terminalEntities.Length > terminalPoints.Length)
		{
			for (int i = terminalPoints.Length; i < terminalEntities.Length; i++)
			{
				if (terminalEntities[i].TryGet(serverside: true, out var entity))
				{
					entity.Kill();
				}
			}
		}
		Array.Resize(ref terminalEntities, terminalPoints.Length);
		for (int j = 0; j < terminalPoints.Length; j++)
		{
			Transform val = terminalPoints[j];
			if (!terminalEntities[j].TryGet(serverside: true, out var _))
			{
				BaseEntity baseEntity = GameManager.server.CreateEntity(terminalPrefab?.resourcePath, val.position, val.rotation);
				baseEntity.SetParent(this, worldPositionStays: true);
				baseEntity.Spawn();
				if (!(baseEntity is MarketTerminal marketTerminal))
				{
					Debug.LogError((object)("Marketplace.terminalPrefab did not spawn a MarketTerminal (it spawned " + ((object)baseEntity).GetType().FullName + ")"));
					baseEntity.Kill();
				}
				else
				{
					marketTerminal.Setup(this);
					terminalEntities[j].Set(marketTerminal);
				}
			}
		}
	}

	public override void Load(LoadInfo info)
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		base.Load(info);
		if (info.msg.subEntityList != null)
		{
			List<NetworkableId> subEntityIds = info.msg.subEntityList.subEntityIds;
			Array.Resize(ref terminalEntities, subEntityIds.Count);
			for (int i = 0; i < subEntityIds.Count; i++)
			{
				terminalEntities[i] = new EntityRef<MarketTerminal>(subEntityIds[i]);
			}
		}
		SpawnSubEntities();
	}

	public override void Save(SaveInfo info)
	{
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		base.Save(info);
		info.msg.subEntityList = Pool.Get<SubEntityList>();
		info.msg.subEntityList.subEntityIds = Pool.GetList<NetworkableId>();
		if (terminalEntities != null)
		{
			for (int i = 0; i < terminalEntities.Length; i++)
			{
				info.msg.subEntityList.subEntityIds.Add(terminalEntities[i].uid);
			}
		}
	}
}
