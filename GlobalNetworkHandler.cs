using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;

public class GlobalNetworkHandler : PointEntity
{
	public static GlobalNetworkHandler server;

	public Dictionary<NetworkableId, GlobalEntityData> serverData = new Dictionary<NetworkableId, GlobalEntityData>();

	private List<Connection> globalConnections = new List<Connection>();

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("GlobalNetworkHandler.OnRpcMessage", 0);
		try
		{
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	public static bool ShouldSendEntity(BaseEntity entity)
	{
		if ((Object)(object)entity == (Object)null || entity.IsDestroyed)
		{
			return false;
		}
		if (entity.HasParent())
		{
			return false;
		}
		if (entity.globalBuildingBlock)
		{
			return true;
		}
		return false;
	}

	public override void ServerInit()
	{
		server = this;
		base.ServerInit();
		if (!Application.isLoadingSave)
		{
			LoadEntitiesOnStartup();
		}
	}

	public override void PostServerLoad()
	{
		base.PostServerLoad();
		LoadEntitiesOnStartup();
	}

	public void OnClientConnected(Connection connection)
	{
		if (connection.globalNetworking)
		{
			globalConnections.Add(connection);
		}
	}

	public void OnClientDisconnected(Connection connection)
	{
		globalConnections.Remove(connection);
	}

	private void LoadEntitiesOnStartup()
	{
		Stopwatch stopwatch = Stopwatch.StartNew();
		Debug.Log((object)"Starting to load entities into GlobalNetworkHandler...");
		foreach (BaseEntity item in BaseNetworkable.serverEntities.OfType<BaseEntity>())
		{
			if (ShouldSendEntity(item))
			{
				OnEntityUpdate(item, sendNetworkUpdate: false);
			}
		}
		Debug.Log((object)$"Took {stopwatch.ElapsedMilliseconds}ms to load entities into GlobalNetworkHandler");
	}

	public void TrySendNetworkUpdate(BaseNetworkable net)
	{
		if (net is BaseEntity entity && ShouldSendEntity(entity))
		{
			OnEntityUpdate(entity);
		}
	}

	private void OnEntityUpdate(BaseEntity entity, bool sendNetworkUpdate = true)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		if (entity.net != null)
		{
			GlobalEntityData val = Pool.Get<GlobalEntityData>();
			val.prefabId = entity.prefabID;
			val.uid = entity.net.ID;
			val.pos = ((Component)entity).transform.position;
			Quaternion rotation = ((Component)entity).transform.rotation;
			val.rot = ((Quaternion)(ref rotation)).eulerAngles;
			if (entity is BuildingBlock buildingBlock)
			{
				val.grade = (int)buildingBlock.grade;
				val.modelState = buildingBlock.modelState;
				val.skin = buildingBlock.skinID;
				val.customColor = (int)buildingBlock.customColour;
			}
			if (entity is Door door)
			{
				val.flags = (int)(door.flags & Flags.Open);
			}
			if (serverData.TryGetValue(entity.net.ID, out var value))
			{
				Pool.Free<GlobalEntityData>(ref value);
			}
			serverData[entity.net.ID] = val;
			if (sendNetworkUpdate)
			{
				SendGlobalEntity(val, new SendInfo(Net.limit_global_update_broadcast ? globalConnections : Net.sv.connections));
			}
		}
	}

	public void OnEntityKilled(BaseNetworkable entity)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		if (serverData.Remove(entity.net.ID))
		{
			SendEntityDelete(entity.net.ID, new SendInfo(Net.limit_global_update_broadcast ? globalConnections : Net.sv.connections));
		}
	}

	public void StartSendingSnapshot(BasePlayer player)
	{
		OnClientConnected(player.Connection);
		if (!Net.limit_global_update_broadcast || player.Connection.globalNetworking)
		{
			SendAsSnapshot(player.Connection);
			SendSnapshot(player);
		}
	}

	private void SendSnapshot(BasePlayer player)
	{
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		if (!Net.globalNetworkedBases)
		{
			return;
		}
		Stopwatch stopwatch = Stopwatch.StartNew();
		GlobalEntityCollection val = Pool.Get<GlobalEntityCollection>();
		try
		{
			val.entities = Pool.GetList<GlobalEntityData>();
			foreach (GlobalEntityData value in serverData.Values)
			{
				val.entities.Add(value);
				if (val.entities.Count >= ConVar.Server.maxpacketsize_globalentities)
				{
					SendGlobalEntities(val, new SendInfo(player.Connection));
					val.entities.Clear();
				}
			}
			if (val.entities.Count > 0)
			{
				SendGlobalEntities(val, new SendInfo(player.Connection));
				val.entities.Clear();
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		stopwatch.Stop();
		if (Net.global_network_debug)
		{
			Debug.Log((object)$"Took {stopwatch.ElapsedMilliseconds}ms to send {serverData.Count} global entities to {((object)player).ToString()}");
		}
	}

	private void SendEntityDelete(NetworkableId networkableId, SendInfo info)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		if (Net.globalNetworkedBases)
		{
			NetWrite val = ClientRPCStart(null, "CLIENT_EntityDeletes");
			int num = Math.Min(ConVar.Server.maxpacketsize_globalentities, 1);
			val.UInt16((ushort)num);
			for (int i = 0; i < num; i++)
			{
				val.EntityID(networkableId);
			}
			ClientRPCSend(val, info);
		}
	}

	private void SendGlobalEntities(GlobalEntityCollection entities, SendInfo info)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		if (Net.globalNetworkedBases)
		{
			ClientRPCEx<GlobalEntityCollection>(info, null, "CLIENT_EntityUpdates", entities);
		}
	}

	private void SendGlobalEntity(GlobalEntityData entity, SendInfo info)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		if (!Net.globalNetworkedBases)
		{
			return;
		}
		GlobalEntityCollection val = Pool.Get<GlobalEntityCollection>();
		try
		{
			val.entities = Pool.GetList<GlobalEntityData>();
			val.entities.Add(entity);
			SendGlobalEntities(val, info);
			val.entities.Clear();
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}
}
