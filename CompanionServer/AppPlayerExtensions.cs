using System.Collections.Generic;
using Facepunch;
using ProtoBuf;
using UnityEngine;

namespace CompanionServer;

public static class AppPlayerExtensions
{
	public static AppTeamInfo GetAppTeamInfo(this BasePlayer player, ulong steamId)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		AppTeamInfo val = Pool.Get<AppTeamInfo>();
		val.members = Pool.GetList<Member>();
		Member val2 = Pool.Get<Member>();
		if ((Object)(object)player != (Object)null)
		{
			Vector2 val3 = Util.WorldToMap(((Component)player).transform.position);
			val2.steamId = player.userID;
			val2.name = player.displayName ?? "";
			val2.x = val3.x;
			val2.y = val3.y;
			val2.isOnline = player.IsConnected;
			val2.spawnTime = player.lifeStory?.timeBorn ?? 0;
			val2.isAlive = player.IsAlive();
			val2.deathTime = player.previousLifeStory?.timeDied ?? 0;
		}
		else
		{
			val2.steamId = steamId;
			val2.name = SingletonComponent<ServerMgr>.Instance.persistance.GetPlayerName(steamId) ?? "";
			val2.x = 0f;
			val2.y = 0f;
			val2.isOnline = false;
			val2.spawnTime = 0u;
			val2.isAlive = false;
			val2.deathTime = 0u;
		}
		val.members.Add(val2);
		val.leaderSteamId = 0uL;
		val.mapNotes = GetMapNotes(val2.steamId, personalNotes: true);
		val.leaderMapNotes = Pool.GetList<Note>();
		return val;
	}

	public static AppTeamInfo GetAppTeamInfo(this RelationshipManager.PlayerTeam team, ulong requesterSteamId)
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		AppTeamInfo val = Pool.Get<AppTeamInfo>();
		val.members = Pool.GetList<Member>();
		for (int i = 0; i < team.members.Count; i++)
		{
			ulong num = team.members[i];
			BasePlayer basePlayer = RelationshipManager.FindByID(num);
			if (!Object.op_Implicit((Object)(object)basePlayer))
			{
				basePlayer = null;
			}
			Vector2 val2 = Util.WorldToMap((basePlayer != null) ? ((Component)basePlayer).transform.position : Vector3.zero);
			Member val3 = Pool.Get<Member>();
			val3.steamId = num;
			val3.name = basePlayer?.displayName ?? SingletonComponent<ServerMgr>.Instance.persistance.GetPlayerName(num) ?? "";
			val3.x = val2.x;
			val3.y = val2.y;
			val3.isOnline = basePlayer?.IsConnected ?? false;
			val3.spawnTime = basePlayer?.lifeStory?.timeBorn ?? 0;
			val3.isAlive = basePlayer?.IsAlive() ?? false;
			val3.deathTime = basePlayer?.previousLifeStory?.timeDied ?? 0;
			val.members.Add(val3);
		}
		val.leaderSteamId = team.teamLeader;
		val.mapNotes = GetMapNotes(requesterSteamId, personalNotes: true);
		if (requesterSteamId != team.teamLeader)
		{
			val.leaderMapNotes = GetMapNotes(team.teamLeader, personalNotes: false);
		}
		else
		{
			val.leaderMapNotes = Pool.GetList<Note>();
		}
		return val;
	}

	private static List<Note> GetMapNotes(ulong playerId, bool personalNotes)
	{
		List<Note> list = Pool.GetList<Note>();
		PlayerState val = SingletonComponent<ServerMgr>.Instance.playerStateManager.Get(playerId);
		if (val != null)
		{
			if (personalNotes && val.deathMarker != null)
			{
				AddMapNote(list, val.deathMarker, BasePlayer.MapNoteType.Death);
			}
			if (val.pointsOfInterest != null)
			{
				foreach (MapNote item in val.pointsOfInterest)
				{
					AddMapNote(list, item, BasePlayer.MapNoteType.PointOfInterest);
				}
			}
		}
		return list;
	}

	private static void AddMapNote(List<Note> result, MapNote note, BasePlayer.MapNoteType type)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = Util.WorldToMap(note.worldPosition);
		Note val2 = Pool.Get<Note>();
		val2.type = (int)type;
		val2.x = val.x;
		val2.y = val.y;
		val2.icon = note.icon;
		val2.colourIndex = note.colourIndex;
		val2.label = note.label;
		result.Add(val2);
	}
}
