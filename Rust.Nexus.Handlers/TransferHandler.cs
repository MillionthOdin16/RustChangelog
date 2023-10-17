using System;
using System.Collections.Generic;
using ConVar;
using Network;
using ProtoBuf;
using ProtoBuf.Nexus;
using UnityEngine;

namespace Rust.Nexus.Handlers;

public class TransferHandler : BaseNexusRequestHandler<TransferRequest>
{
	private static readonly Dictionary<ulong, ulong> UidMapping = new Dictionary<ulong, ulong>();

	private static readonly Dictionary<BaseEntity, Entity> EntityToSpawn = new Dictionary<BaseEntity, Entity>();

	private static readonly Dictionary<ulong, BasePlayer> SpawnedPlayers = new Dictionary<ulong, BasePlayer>();

	private static readonly List<string> PlayerIds = new List<string>();

	private static readonly List<NetworkableId> EntitiesToProtect = new List<NetworkableId>();

	private static readonly Dictionary<ulong, RelationshipManager.PlayerTeam> TeamMapping = new Dictionary<ulong, RelationshipManager.PlayerTeam>();

	protected override void Handle()
	{
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		UidMapping.Clear();
		base.Request.InspectUids((UidInspector<ulong>)UpdateWithNewUid);
		PlayerIds.Clear();
		EntitiesToProtect.Clear();
		foreach (Entity entity in base.Request.entities)
		{
			if (entity.basePlayer != null)
			{
				ulong userid = entity.basePlayer.userid;
				Debug.Log((object)$"Found player {userid} in transfer");
				PlayerIds.Add(userid.ToString("G"));
				BasePlayer basePlayer = BasePlayer.FindByID(userid) ?? BasePlayer.FindSleeping(userid);
				if ((Object)(object)basePlayer != (Object)null)
				{
					if (basePlayer.IsConnected)
					{
						basePlayer.Kick("Player transfer is overwriting you - contact developers!");
					}
					basePlayer.Kill();
				}
				entity.basePlayer.currentTeam = 0uL;
				RelationshipManager.ServerInstance.FindPlayersTeam(userid)?.RemovePlayer(userid);
				if ((entity.basePlayer.playerFlags & 0x10) == 0)
				{
					BasePlayer basePlayer2 = entity.basePlayer;
					basePlayer2.playerFlags |= 0x2000000;
				}
				if (entity.basePlayer.loadingTimeout <= 0f || entity.basePlayer.loadingTimeout > ConVar.Nexus.loadingTimeout)
				{
					entity.basePlayer.loadingTimeout = ConVar.Nexus.loadingTimeout;
				}
			}
			if (entity.baseCombat != null && entity.baseNetworkable != null)
			{
				EntitiesToProtect.Add(entity.baseNetworkable.uid);
			}
		}
		RepositionEntitiesFromTransfer();
		SpawnedPlayers.Clear();
		SpawnEntities(SpawnedPlayers);
		foreach (NetworkableId item in EntitiesToProtect)
		{
			if (BaseNetworkable.serverEntities.Find(item) is BaseEntity baseEntity)
			{
				baseEntity.EnableTransferProtection();
			}
		}
		TeamMapping.Clear();
		foreach (PlayerSecondaryData secondaryDatum in base.Request.secondaryData)
		{
			if (!SpawnedPlayers.TryGetValue(secondaryDatum.userId, out var value))
			{
				Debug.LogError((object)$"Got secondary data for {secondaryDatum.userId} but they were not spawned in the transfer");
				continue;
			}
			value.LoadSecondaryData(secondaryDatum);
			if (secondaryDatum.isTeamLeader && secondaryDatum.teamId != 0L && !TeamMapping.ContainsKey(secondaryDatum.teamId))
			{
				RelationshipManager.PlayerTeam playerTeam = RelationshipManager.ServerInstance.CreateTeam();
				playerTeam.teamLeader = value.userID;
				playerTeam.AddPlayer(value);
				TeamMapping.Add(secondaryDatum.teamId, playerTeam);
			}
		}
		foreach (PlayerSecondaryData secondaryDatum2 in base.Request.secondaryData)
		{
			if (SpawnedPlayers.TryGetValue(secondaryDatum2.userId, out var value2) && secondaryDatum2.teamId != 0L && !secondaryDatum2.isTeamLeader)
			{
				if (TeamMapping.TryGetValue(secondaryDatum2.teamId, out var value3))
				{
					value3.AddPlayer(value2);
					continue;
				}
				RelationshipManager.PlayerTeam playerTeam2 = RelationshipManager.ServerInstance.CreateTeam();
				playerTeam2.teamLeader = value2.userID;
				playerTeam2.AddPlayer(value2);
				TeamMapping.Add(secondaryDatum2.teamId, playerTeam2);
			}
		}
		if (PlayerIds.Count > 0)
		{
			Debug.Log((object)("Completing transfers for players: " + string.Join(", ", PlayerIds)));
			CompleteTransfers();
		}
		static void UpdateWithNewUid(UidType type, ref ulong prevUid)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Invalid comparison between Unknown and I4
			if ((int)type == 3)
			{
				prevUid = 0uL;
			}
			else if (prevUid != 0L)
			{
				if (!UidMapping.TryGetValue(prevUid, out var value4))
				{
					value4 = Net.sv.TakeUID();
					UidMapping.Add(prevUid, value4);
				}
				prevUid = value4;
			}
		}
	}

	private static async void CompleteTransfers()
	{
		try
		{
			await NexusServer.ZoneClient.CompleteTransfers((IEnumerable<string>)PlayerIds);
		}
		catch (Exception ex)
		{
			Debug.LogException(ex);
		}
	}

	private void RepositionEntitiesFromTransfer()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		Entity obj = base.Request.entities[0];
		Vector3 pos = obj.baseEntity.pos;
		Quaternion val = Quaternion.Euler(obj.baseEntity.rot);
		(Vector3 Position, Quaternion Rotation, bool PreserveY) tuple = ZoneController.Instance.ChooseTransferDestination(base.FromZone.Key, base.Request.method, base.Request.from, base.Request.to, pos, val);
		var (val2, val3, _) = tuple;
		if (tuple.PreserveY)
		{
			val2.y = pos.y;
		}
		Vector3 val4 = val2 - pos;
		Quaternion val5 = val3 * Quaternion.Inverse(val);
		foreach (Entity entity in base.Request.entities)
		{
			if (entity.baseEntity != null && (entity.parent == null || !((NetworkableId)(ref entity.parent.uid)).IsValid))
			{
				BaseEntity baseEntity = entity.baseEntity;
				baseEntity.pos += val4;
				BaseEntity baseEntity2 = entity.baseEntity;
				Quaternion val6 = Quaternion.Euler(entity.baseEntity.rot) * val5;
				baseEntity2.rot = ((Quaternion)(ref val6)).eulerAngles;
			}
		}
	}

	private void SpawnEntities(Dictionary<ulong, BasePlayer> players)
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		Application.isLoadingSave = true;
		try
		{
			EntityToSpawn.Clear();
			foreach (Entity entity in base.Request.entities)
			{
				BaseEntity baseEntity = GameManager.server.CreateEntity(StringPool.Get(entity.baseNetworkable.prefabID), entity.baseEntity.pos, Quaternion.Euler(entity.baseEntity.rot));
				if ((Object)(object)baseEntity != (Object)null)
				{
					baseEntity.InitLoad(entity.baseNetworkable.uid);
					EntityToSpawn.Add(baseEntity, entity);
				}
			}
			foreach (KeyValuePair<BaseEntity, Entity> item in EntityToSpawn)
			{
				BaseEntity key = item.Key;
				if (!((Object)(object)key == (Object)null))
				{
					key.Spawn();
					key.Load(new BaseNetworkable.LoadInfo
					{
						fromDisk = true,
						fromTransfer = true,
						msg = item.Value
					});
				}
			}
			foreach (KeyValuePair<BaseEntity, Entity> item2 in EntityToSpawn)
			{
				BaseEntity key2 = item2.Key;
				if (!((Object)(object)key2 == (Object)null))
				{
					key2.UpdateNetworkGroup();
					key2.PostServerLoad();
					if (key2 is BasePlayer basePlayer)
					{
						players[basePlayer.userID] = basePlayer;
					}
				}
			}
		}
		finally
		{
			Application.isLoadingSave = false;
		}
	}
}
