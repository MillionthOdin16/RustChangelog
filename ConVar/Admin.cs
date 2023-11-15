using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Facepunch;
using Facepunch.Extend;
using Facepunch.Math;
using Network;
using Newtonsoft.Json;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Scripting;

namespace ConVar;

[Factory("global")]
public class Admin : ConsoleSystem
{
	private enum ChangeGradeMode
	{
		Upgrade,
		Downgrade
	}

	[Preserve]
	public struct PlayerInfo
	{
		public string SteamID;

		public string OwnerSteamID;

		public string DisplayName;

		public int Ping;

		public string Address;

		public int ConnectedSeconds;

		public float VoiationLevel;

		public float CurrentLevel;

		public float UnspentXp;

		public float Health;
	}

	[Preserve]
	public struct ServerInfoOutput
	{
		public string Hostname;

		public int MaxPlayers;

		public int Players;

		public int Queued;

		public int Joining;

		public int EntityCount;

		public string GameTime;

		public int Uptime;

		public string Map;

		public float Framerate;

		public int Memory;

		public int MemoryUsageSystem;

		public int Collections;

		public int NetworkIn;

		public int NetworkOut;

		public bool Restarting;

		public string SaveCreatedTime;

		public int Version;

		public string Protocol;
	}

	[Preserve]
	public struct ServerConvarInfo
	{
		public string FullName;

		public string Value;

		public string Help;
	}

	[Preserve]
	public struct ServerUGCInfo
	{
		public NetworkableId entityId;

		public uint[] crcs;

		public UGCType contentType;

		public uint entityPrefabID;

		public string shortPrefabName;

		public ulong[] playerIds;

		public ServerUGCInfo(IUGCBrowserEntity fromEntity)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			entityId = fromEntity.UgcEntity.net.ID;
			crcs = fromEntity.GetContentCRCs;
			contentType = fromEntity.ContentType;
			entityPrefabID = fromEntity.UgcEntity.prefabID;
			shortPrefabName = fromEntity.UgcEntity.ShortPrefabName;
			playerIds = fromEntity.EditingHistory.ToArray();
		}
	}

	private struct EntityAssociation
	{
		public BaseEntity TargetEntity;

		public EntityAssociationType AssociationType;
	}

	private enum EntityAssociationType
	{
		Owner,
		Auth,
		LockGuest
	}

	[ReplicatedVar(Help = "Controls whether the in-game admin UI is displayed to admins")]
	public static bool allowAdminUI = true;

	[ServerVar(Help = "Print out currently connected clients")]
	public static void status(Arg arg)
	{
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Expected O, but got Unknown
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		string @string = arg.GetString(0, "");
		if (@string == "--json")
		{
			@string = arg.GetString(1, "");
		}
		bool flag = arg.HasArg("--json");
		string text = string.Empty;
		if (!flag && @string.Length == 0)
		{
			text = text + "hostname: " + Server.hostname + "\n";
			text = text + "version : " + 2511 + " secure (secure mode enabled, connected to Steam3)\n";
			text = text + "map     : " + Server.level + "\n";
			text += $"players : {((IEnumerable<BasePlayer>)BasePlayer.activePlayerList).Count()} ({Server.maxplayers} max) ({SingletonComponent<ServerMgr>.Instance.connectionQueue.Queued} queued) ({SingletonComponent<ServerMgr>.Instance.connectionQueue.Joining} joining)\n\n";
		}
		TextTable val = new TextTable();
		val.AddColumn("id");
		val.AddColumn("name");
		val.AddColumn("ping");
		val.AddColumn("connected");
		val.AddColumn("addr");
		val.AddColumn("owner");
		val.AddColumn("violation");
		val.AddColumn("kicks");
		Enumerator<BasePlayer> enumerator = BasePlayer.activePlayerList.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				BasePlayer current = enumerator.Current;
				try
				{
					if (!current.IsValid())
					{
						continue;
					}
					string userIDString = current.UserIDString;
					if (current.net.connection == null)
					{
						val.AddRow(new string[2] { userIDString, "NO CONNECTION" });
						continue;
					}
					string text2 = current.net.connection.ownerid.ToString();
					string text3 = StringExtensions.QuoteSafe(current.displayName);
					string text4 = Net.sv.GetAveragePing(current.net.connection).ToString();
					string text5 = current.net.connection.ipaddress;
					string text6 = current.violationLevel.ToString("0.0");
					string text7 = current.GetAntiHackKicks().ToString();
					if (!arg.IsAdmin && !arg.IsRcon)
					{
						text5 = "xx.xxx.xx.xxx";
					}
					string text8 = current.net.connection.GetSecondsConnected() + "s";
					if (@string.Length <= 0 || StringEx.Contains(text3, @string, CompareOptions.IgnoreCase) || userIDString.Contains(@string) || text2.Contains(@string) || text5.Contains(@string))
					{
						val.AddRow(new string[8]
						{
							userIDString,
							text3,
							text4,
							text8,
							text5,
							(text2 == userIDString) ? string.Empty : text2,
							text6,
							text7
						});
					}
				}
				catch (Exception ex)
				{
					val.AddRow(new string[2]
					{
						current.UserIDString,
						StringExtensions.QuoteSafe(ex.Message)
					});
				}
			}
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
		if (flag)
		{
			arg.ReplyWith(val.ToJson());
		}
		else
		{
			arg.ReplyWith(text + ((object)val).ToString());
		}
	}

	[ServerVar(Help = "Print out stats of currently connected clients")]
	public static void stats(Arg arg)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Expected O, but got Unknown
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		TextTable table = new TextTable();
		table.AddColumn("id");
		table.AddColumn("name");
		table.AddColumn("time");
		table.AddColumn("kills");
		table.AddColumn("deaths");
		table.AddColumn("suicides");
		table.AddColumn("player");
		table.AddColumn("building");
		table.AddColumn("entity");
		Action<ulong, string> action = delegate(ulong id, string name)
		{
			ServerStatistics.Storage storage = ServerStatistics.Get(id);
			string text2 = TimeSpan.FromSeconds(storage.Get("time")).ToShortString();
			string text3 = storage.Get("kill_player").ToString();
			string text4 = (storage.Get("deaths") - storage.Get("death_suicide")).ToString();
			string text5 = storage.Get("death_suicide").ToString();
			string text6 = storage.Get("hit_player_direct_los").ToString();
			string text7 = storage.Get("hit_player_indirect_los").ToString();
			string text8 = storage.Get("hit_building_direct_los").ToString();
			string text9 = storage.Get("hit_building_indirect_los").ToString();
			string text10 = storage.Get("hit_entity_direct_los").ToString();
			string text11 = storage.Get("hit_entity_indirect_los").ToString();
			table.AddRow(new string[9]
			{
				id.ToString(),
				name,
				text2,
				text3,
				text4,
				text5,
				text6 + " / " + text7,
				text8 + " / " + text9,
				text10 + " / " + text11
			});
		};
		ulong uInt = arg.GetUInt64(0, 0uL);
		if (uInt == 0L)
		{
			string @string = arg.GetString(0, "");
			Enumerator<BasePlayer> enumerator = BasePlayer.activePlayerList.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					BasePlayer current = enumerator.Current;
					try
					{
						if (current.IsValid())
						{
							string text = StringExtensions.QuoteSafe(current.displayName);
							if (@string.Length <= 0 || StringEx.Contains(text, @string, CompareOptions.IgnoreCase))
							{
								action(current.userID, text);
							}
						}
					}
					catch (Exception ex)
					{
						table.AddRow(new string[2]
						{
							current.UserIDString,
							StringExtensions.QuoteSafe(ex.Message)
						});
					}
				}
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
		}
		else
		{
			string arg2 = "N/A";
			BasePlayer basePlayer = BasePlayer.FindByID(uInt);
			if (Object.op_Implicit((Object)(object)basePlayer))
			{
				arg2 = StringExtensions.QuoteSafe(basePlayer.displayName);
			}
			action(uInt, arg2);
		}
		arg.ReplyWith(arg.HasArg("--json") ? table.ToJson() : ((object)table).ToString());
	}

	[ServerVar(Help = "upgrade_radius 'grade' 'radius'")]
	public static void upgrade_radius(Arg arg)
	{
		if (!arg.HasArgs(2))
		{
			arg.ReplyWith("Format is 'upgrade_radius {grade} {radius}'");
		}
		else
		{
			SkinRadiusInternal(arg, changeAnyGrade: true);
		}
	}

	[ServerVar(Help = "skin_radius 'skin' 'radius'")]
	public static void skin_radius(Arg arg)
	{
		if (!arg.HasArgs(2))
		{
			arg.ReplyWith("Format is 'skin_radius {skin} {radius}'");
		}
		else
		{
			SkinRadiusInternal(arg, changeAnyGrade: false);
		}
	}

	private static void SkinRadiusInternal(Arg arg, bool changeAnyGrade)
	{
		//IL_0535: Unknown result type (might be due to invalid IL or missing references)
		BasePlayer basePlayer = arg.Player();
		if ((Object)(object)basePlayer == (Object)null)
		{
			arg.ReplyWith("This must be called from the client");
			return;
		}
		float @float = arg.GetFloat(1, 0f);
		string @string = arg.GetString(0, "");
		BuildingGrade buildingGrade = null;
		IEnumerable<BuildingGrade> source = from x in PrefabAttribute.server.FindAll<ConstructionGrade>(2194854973u)
			select x.gradeBase;
		switch (@string)
		{
		case "twig":
			buildingGrade = source.FirstOrDefault((BuildingGrade x) => ((Object)x).name == "twigs");
			break;
		case "wood":
			buildingGrade = source.FirstOrDefault((BuildingGrade x) => ((Object)x).name == "wood");
			break;
		case "stone":
			buildingGrade = source.FirstOrDefault((BuildingGrade x) => ((Object)x).name == "stone");
			break;
		case "metal":
		case "sheetmetal":
			buildingGrade = source.FirstOrDefault((BuildingGrade x) => ((Object)x).name == "metal");
			break;
		case "hqm":
		case "armored":
		case "armoured":
			buildingGrade = source.FirstOrDefault((BuildingGrade x) => ((Object)x).name == "toptier");
			break;
		case "adobe":
			buildingGrade = source.FirstOrDefault((BuildingGrade x) => ((Object)x).name == "adobe");
			break;
		case "shipping":
		case "shippingcontainer":
		case "container":
			buildingGrade = source.FirstOrDefault((BuildingGrade x) => ((Object)x).name == "shipping_container");
			break;
		case "brutal":
		case "brutalist":
			buildingGrade = source.FirstOrDefault((BuildingGrade x) => ((Object)x).name == "brutalist");
			break;
		case "brick":
			buildingGrade = source.FirstOrDefault((BuildingGrade x) => ((Object)x).name == "brick");
			break;
		case "frontier":
			buildingGrade = source.FirstOrDefault((BuildingGrade x) => ((Object)x).name == "frontier");
			break;
		case "gingerbread":
			buildingGrade = source.FirstOrDefault((BuildingGrade x) => ((Object)x).name == "gingerbread");
			break;
		default:
			arg.ReplyWith("Valid skins are: twig, wood, stone, metal, hqm, adobe, shipping, brutalist, brick, frontier, gingerbread");
			return;
		}
		if ((Object)(object)buildingGrade == (Object)null)
		{
			arg.ReplyWith("Unable to find skin object for " + @string);
			return;
		}
		if (!buildingGrade.enabledInStandalone)
		{
			arg.ReplyWith("Skin " + @string + " is not enabled in standalone yet");
			return;
		}
		List<BuildingBlock> list = new List<BuildingBlock>();
		global::Vis.Entities(((Component)basePlayer).transform.position, @float, list, 2097152, (QueryTriggerInteraction)2);
		foreach (BuildingBlock item in list)
		{
			if (item.grade == buildingGrade.type || changeAnyGrade)
			{
				item.ChangeGradeAndSkin(buildingGrade.type, buildingGrade.skin);
			}
		}
	}

	[ServerVar]
	public static void killplayer(Arg arg)
	{
		BasePlayer basePlayer = arg.GetPlayerOrSleeper(0);
		if (!Object.op_Implicit((Object)(object)basePlayer))
		{
			basePlayer = BasePlayer.FindBotClosestMatch(arg.GetString(0, ""));
		}
		if (!Object.op_Implicit((Object)(object)basePlayer))
		{
			arg.ReplyWith("Player not found");
		}
		else
		{
			basePlayer.Hurt(1000f, DamageType.Suicide, basePlayer, useProtection: false);
		}
	}

	[ServerVar]
	public static void injureplayer(Arg arg)
	{
		BasePlayer basePlayer = arg.GetPlayerOrSleeper(0);
		if (!Object.op_Implicit((Object)(object)basePlayer))
		{
			basePlayer = BasePlayer.FindBotClosestMatch(arg.GetString(0, ""));
		}
		if (!Object.op_Implicit((Object)(object)basePlayer))
		{
			arg.ReplyWith("Player not found");
		}
		else
		{
			Global.InjurePlayer(basePlayer);
		}
	}

	[ServerVar]
	public static void recoverplayer(Arg arg)
	{
		BasePlayer basePlayer = arg.GetPlayerOrSleeper(0);
		if (!Object.op_Implicit((Object)(object)basePlayer))
		{
			basePlayer = BasePlayer.FindBotClosestMatch(arg.GetString(0, ""));
		}
		if (!Object.op_Implicit((Object)(object)basePlayer))
		{
			arg.ReplyWith("Player not found");
		}
		else
		{
			Global.RecoverPlayer(basePlayer);
		}
	}

	[ServerVar]
	public static void kick(Arg arg)
	{
		BasePlayer player = arg.GetPlayer(0);
		if (!Object.op_Implicit((Object)(object)player) || player.net == null || player.net.connection == null)
		{
			arg.ReplyWith("Player not found");
			return;
		}
		string @string = arg.GetString(1, "no reason given");
		arg.ReplyWith("Kicked: " + player.displayName);
		Chat.Broadcast("Kicking " + player.displayName + " (" + @string + ")", "SERVER", "#eee", 0uL);
		player.Kick("Kicked: " + arg.GetString(1, "No Reason Given"));
	}

	[ServerVar]
	public static void kickall(Arg arg)
	{
		BasePlayer[] array = ((IEnumerable<BasePlayer>)BasePlayer.activePlayerList).ToArray();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Kick("Kicked: " + arg.GetString(1, "No Reason Given"));
		}
	}

	[ServerVar(Help = "ban <player> <reason> [optional duration]")]
	public static void ban(Arg arg)
	{
		BasePlayer player = arg.GetPlayer(0);
		if (!Object.op_Implicit((Object)(object)player) || player.net == null || player.net.connection == null)
		{
			arg.ReplyWith("Player not found");
			return;
		}
		ServerUsers.User user = ServerUsers.Get(player.userID);
		if (user != null && user.group == ServerUsers.UserGroup.Banned)
		{
			arg.ReplyWith($"User {player.userID} is already banned");
			return;
		}
		string @string = arg.GetString(1, "No Reason Given");
		if (TryGetBanExpiry(arg, 2, out var expiry, out var durationSuffix))
		{
			ServerUsers.Set(player.userID, ServerUsers.UserGroup.Banned, player.displayName, @string, expiry);
			string text = "";
			if (player.IsConnected && player.net.connection.ownerid != 0L && player.net.connection.ownerid != player.net.connection.userid)
			{
				text += $" and also banned ownerid {player.net.connection.ownerid}";
				ServerUsers.Set(player.net.connection.ownerid, ServerUsers.UserGroup.Banned, player.displayName, arg.GetString(1, $"Family share owner of {player.net.connection.userid}"), -1L);
			}
			ServerUsers.Save();
			arg.ReplyWith($"Kickbanned User{durationSuffix}: {player.userID} - {player.displayName}{text}");
			Chat.Broadcast("Kickbanning " + player.displayName + durationSuffix + " (" + @string + ")", "SERVER", "#eee", 0uL);
			Net.sv.Kick(player.net.connection, "Banned" + durationSuffix + ": " + @string, false);
		}
	}

	[ServerVar]
	public static void moderatorid(Arg arg)
	{
		ulong uInt = arg.GetUInt64(0, 0uL);
		string @string = arg.GetString(1, "unnamed");
		string string2 = arg.GetString(2, "no reason");
		if (uInt < 70000000000000000L)
		{
			arg.ReplyWith("This doesn't appear to be a 64bit steamid: " + uInt);
			return;
		}
		ServerUsers.User user = ServerUsers.Get(uInt);
		if (user != null && user.group == ServerUsers.UserGroup.Moderator)
		{
			arg.ReplyWith("User " + uInt + " is already a Moderator");
			return;
		}
		ServerUsers.Set(uInt, ServerUsers.UserGroup.Moderator, @string, string2, -1L);
		BasePlayer basePlayer = BasePlayer.FindByID(uInt);
		if ((Object)(object)basePlayer != (Object)null)
		{
			basePlayer.SetPlayerFlag(BasePlayer.PlayerFlags.IsAdmin, b: true);
			basePlayer.SendNetworkUpdate();
		}
		arg.ReplyWith("Added moderator " + @string + ", steamid " + uInt);
	}

	[ServerVar]
	public static void ownerid(Arg arg)
	{
		ulong uInt = arg.GetUInt64(0, 0uL);
		string @string = arg.GetString(1, "unnamed");
		string string2 = arg.GetString(2, "no reason");
		if (uInt < 70000000000000000L)
		{
			arg.ReplyWith("This doesn't appear to be a 64bit steamid: " + uInt);
			return;
		}
		if (arg.Connection != null && arg.Connection.authLevel < 2)
		{
			arg.ReplyWith("Moderators cannot run ownerid");
			return;
		}
		ServerUsers.User user = ServerUsers.Get(uInt);
		if (user != null && user.group == ServerUsers.UserGroup.Owner)
		{
			arg.ReplyWith("User " + uInt + " is already an Owner");
			return;
		}
		ServerUsers.Set(uInt, ServerUsers.UserGroup.Owner, @string, string2, -1L);
		BasePlayer basePlayer = BasePlayer.FindByID(uInt);
		if ((Object)(object)basePlayer != (Object)null)
		{
			basePlayer.SetPlayerFlag(BasePlayer.PlayerFlags.IsAdmin, b: true);
			basePlayer.SendNetworkUpdate();
		}
		arg.ReplyWith("Added owner " + @string + ", steamid " + uInt);
	}

	[ServerVar]
	public static void removemoderator(Arg arg)
	{
		ulong uInt = arg.GetUInt64(0, 0uL);
		if (uInt < 70000000000000000L)
		{
			arg.ReplyWith("This doesn't appear to be a 64bit steamid: " + uInt);
			return;
		}
		ServerUsers.User user = ServerUsers.Get(uInt);
		if (user == null || user.group != ServerUsers.UserGroup.Moderator)
		{
			arg.ReplyWith("User " + uInt + " isn't a moderator");
			return;
		}
		ServerUsers.Remove(uInt);
		BasePlayer basePlayer = BasePlayer.FindByID(uInt);
		if ((Object)(object)basePlayer != (Object)null)
		{
			basePlayer.SetPlayerFlag(BasePlayer.PlayerFlags.IsAdmin, b: false);
			basePlayer.SendNetworkUpdate();
		}
		arg.ReplyWith("Removed Moderator: " + uInt);
	}

	[ServerVar]
	public static void removeowner(Arg arg)
	{
		ulong uInt = arg.GetUInt64(0, 0uL);
		if (uInt < 70000000000000000L)
		{
			arg.ReplyWith("This doesn't appear to be a 64bit steamid: " + uInt);
			return;
		}
		ServerUsers.User user = ServerUsers.Get(uInt);
		if (user == null || user.group != ServerUsers.UserGroup.Owner)
		{
			arg.ReplyWith("User " + uInt + " isn't an owner");
			return;
		}
		ServerUsers.Remove(uInt);
		BasePlayer basePlayer = BasePlayer.FindByID(uInt);
		if ((Object)(object)basePlayer != (Object)null)
		{
			basePlayer.SetPlayerFlag(BasePlayer.PlayerFlags.IsAdmin, b: false);
			basePlayer.SendNetworkUpdate();
		}
		arg.ReplyWith("Removed Owner: " + uInt);
	}

	[ServerVar(Help = "banid <steamid> <username> <reason> [optional duration]")]
	public static void banid(Arg arg)
	{
		ulong uInt = arg.GetUInt64(0, 0uL);
		string text = arg.GetString(1, "unnamed");
		string @string = arg.GetString(2, "no reason");
		if (uInt < 70000000000000000L)
		{
			arg.ReplyWith("This doesn't appear to be a 64bit steamid: " + uInt);
			return;
		}
		ServerUsers.User user = ServerUsers.Get(uInt);
		if (user != null && user.group == ServerUsers.UserGroup.Banned)
		{
			arg.ReplyWith("User " + uInt + " is already banned");
		}
		else
		{
			if (!TryGetBanExpiry(arg, 3, out var expiry, out var durationSuffix))
			{
				return;
			}
			string text2 = "";
			BasePlayer basePlayer = BasePlayer.FindByID(uInt);
			if ((Object)(object)basePlayer != (Object)null && basePlayer.IsConnected)
			{
				text = basePlayer.displayName;
				if (basePlayer.IsConnected && basePlayer.net.connection.ownerid != 0L && basePlayer.net.connection.ownerid != basePlayer.net.connection.userid)
				{
					text2 += $" and also banned ownerid {basePlayer.net.connection.ownerid}";
					ServerUsers.Set(basePlayer.net.connection.ownerid, ServerUsers.UserGroup.Banned, basePlayer.displayName, arg.GetString(1, $"Family share owner of {basePlayer.net.connection.userid}"), expiry);
				}
				Chat.Broadcast("Kickbanning " + basePlayer.displayName + durationSuffix + " (" + @string + ")", "SERVER", "#eee", 0uL);
				Net.sv.Kick(basePlayer.net.connection, "Banned" + durationSuffix + ": " + @string, false);
			}
			ServerUsers.Set(uInt, ServerUsers.UserGroup.Banned, text, @string, expiry);
			arg.ReplyWith($"Banned User{durationSuffix}: {uInt} - \"{text}\" for \"{@string}\"{text2}");
		}
	}

	private static bool TryGetBanExpiry(Arg arg, int n, out long expiry, out string durationSuffix)
	{
		expiry = arg.GetTimestamp(n, -1L);
		durationSuffix = null;
		int current = Epoch.Current;
		if (expiry > 0 && expiry <= current)
		{
			arg.ReplyWith("Expiry time is in the past");
			return false;
		}
		durationSuffix = ((expiry > 0) ? (" for " + NumberExtensions.FormatSecondsLong(expiry - current)) : "");
		return true;
	}

	[ServerVar]
	public static void unban(Arg arg)
	{
		ulong uInt = arg.GetUInt64(0, 0uL);
		if (uInt < 70000000000000000L)
		{
			arg.ReplyWith($"This doesn't appear to be a 64bit steamid: {uInt}");
			return;
		}
		ServerUsers.User user = ServerUsers.Get(uInt);
		if (user == null || user.group != ServerUsers.UserGroup.Banned)
		{
			arg.ReplyWith($"User {uInt} isn't banned");
			return;
		}
		ServerUsers.Remove(uInt);
		arg.ReplyWith("Unbanned User: " + uInt);
	}

	[ServerVar]
	public static void skipqueue(Arg arg)
	{
		ulong uInt = arg.GetUInt64(0, 0uL);
		if (uInt < 70000000000000000L)
		{
			arg.ReplyWith("This doesn't appear to be a 64bit steamid: " + uInt);
		}
		else
		{
			SingletonComponent<ServerMgr>.Instance.connectionQueue.SkipQueue(uInt);
		}
	}

	[ServerVar(Help = "Adds skip queue permissions to a SteamID")]
	public static void skipqueueid(Arg arg)
	{
		ulong uInt = arg.GetUInt64(0, 0uL);
		string @string = arg.GetString(1, "unnamed");
		string string2 = arg.GetString(2, "no reason");
		if (uInt < 70000000000000000L)
		{
			arg.ReplyWith("This doesn't appear to be a 64bit steamid: " + uInt);
			return;
		}
		ServerUsers.User user = ServerUsers.Get(uInt);
		if (user != null && (user.group == ServerUsers.UserGroup.Owner || user.group == ServerUsers.UserGroup.Moderator || user.group == ServerUsers.UserGroup.SkipQueue))
		{
			arg.ReplyWith($"User {uInt} will already skip the queue ({user.group})");
			return;
		}
		if (user != null && user.group == ServerUsers.UserGroup.Banned)
		{
			arg.ReplyWith($"User {uInt} is banned");
			return;
		}
		ServerUsers.Set(uInt, ServerUsers.UserGroup.SkipQueue, @string, string2, -1L);
		arg.ReplyWith($"Added skip queue permission for {@string} ({uInt})");
	}

	[ServerVar(Help = "Removes skip queue permission from a SteamID")]
	public static void removeskipqueue(Arg arg)
	{
		ulong uInt = arg.GetUInt64(0, 0uL);
		if (uInt < 70000000000000000L)
		{
			arg.ReplyWith("This doesn't appear to be a 64bit steamid: " + uInt);
			return;
		}
		ServerUsers.User user = ServerUsers.Get(uInt);
		if (user != null && (user.group == ServerUsers.UserGroup.Owner || user.group == ServerUsers.UserGroup.Moderator))
		{
			arg.ReplyWith($"User is a {user.group}, cannot remove skip queue permission with this command");
			return;
		}
		if (user == null || user.group != ServerUsers.UserGroup.SkipQueue)
		{
			arg.ReplyWith("User does not have skip queue permission");
			return;
		}
		ServerUsers.Remove(uInt);
		arg.ReplyWith("Removed skip queue permission: " + uInt);
	}

	[ServerVar(Help = "Print out currently connected clients etc")]
	public static void players(Arg arg)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		TextTable val = new TextTable();
		val.AddColumn("id");
		val.AddColumn("name");
		val.AddColumn("ping");
		val.AddColumn("snap");
		val.AddColumn("updt");
		val.AddColumn("posi");
		val.AddColumn("dist");
		Enumerator<BasePlayer> enumerator = BasePlayer.activePlayerList.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				BasePlayer current = enumerator.Current;
				string userIDString = current.UserIDString;
				string text = current.displayName.ToString();
				if (text.Length >= 14)
				{
					text = text.Substring(0, 14) + "..";
				}
				string text2 = text;
				string text3 = Net.sv.GetAveragePing(current.net.connection).ToString();
				string text4 = current.GetQueuedUpdateCount(BasePlayer.NetworkQueue.Update).ToString();
				string text5 = current.GetQueuedUpdateCount(BasePlayer.NetworkQueue.UpdateDistance).ToString();
				val.AddRow(new string[7]
				{
					userIDString,
					text2,
					text3,
					string.Empty,
					text4,
					string.Empty,
					text5
				});
			}
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
		arg.ReplyWith(arg.HasArg("--json") ? val.ToJson() : ((object)val).ToString());
	}

	[ServerVar(Help = "Sends a message in chat")]
	public static void say(Arg arg)
	{
		Chat.Broadcast(arg.FullString, "SERVER", "#eee", 0uL);
	}

	[ServerVar(Help = "Show user info for players on server.")]
	public static void users(Arg arg)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		string text = "<slot:userid:\"name\">\n";
		int num = 0;
		Enumerator<BasePlayer> enumerator = BasePlayer.activePlayerList.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				BasePlayer current = enumerator.Current;
				text = text + current.userID + ":\"" + current.displayName + "\"\n";
				num++;
			}
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
		text = text + num + "users\n";
		arg.ReplyWith(text);
	}

	[ServerVar(Help = "Show user info for players on server.")]
	public static void sleepingusers(Arg arg)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		string text = "<slot:userid:\"name\">\n";
		int num = 0;
		Enumerator<BasePlayer> enumerator = BasePlayer.sleepingPlayerList.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				BasePlayer current = enumerator.Current;
				text += $"{current.userID}:{current.displayName}\n";
				num++;
			}
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
		text += $"{num} sleeping users\n";
		arg.ReplyWith(text);
	}

	[ServerVar(Help = "Show user info for sleeping players on server in range of the player.")]
	public static void sleepingusersinrange(Arg arg)
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		BasePlayer fromPlayer = arg.Player();
		if ((Object)(object)fromPlayer == (Object)null)
		{
			return;
		}
		float range = arg.GetFloat(0, 0f);
		string text = "<slot:userid:\"name\">\n";
		int num = 0;
		List<BasePlayer> list = Pool.GetList<BasePlayer>();
		Enumerator<BasePlayer> enumerator = BasePlayer.sleepingPlayerList.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				BasePlayer current = enumerator.Current;
				list.Add(current);
			}
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
		list.RemoveAll((BasePlayer p) => p.Distance2D((BaseEntity)fromPlayer) > range);
		list.Sort((BasePlayer player, BasePlayer basePlayer) => (!(player.Distance2D((BaseEntity)fromPlayer) < basePlayer.Distance2D((BaseEntity)fromPlayer))) ? 1 : (-1));
		foreach (BasePlayer item in list)
		{
			text += $"{item.userID}:{item.displayName}:{item.Distance2D((BaseEntity)fromPlayer)}m\n";
			num++;
		}
		Pool.FreeList<BasePlayer>(ref list);
		text += $"{num} sleeping users within {range}m\n";
		arg.ReplyWith(text);
	}

	[ServerVar(Help = "Show user info for players on server in range of the player.")]
	public static void usersinrange(Arg arg)
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		BasePlayer fromPlayer = arg.Player();
		if ((Object)(object)fromPlayer == (Object)null)
		{
			return;
		}
		float range = arg.GetFloat(0, 0f);
		string text = "<slot:userid:\"name\">\n";
		int num = 0;
		List<BasePlayer> list = Pool.GetList<BasePlayer>();
		Enumerator<BasePlayer> enumerator = BasePlayer.activePlayerList.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				BasePlayer current = enumerator.Current;
				list.Add(current);
			}
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
		list.RemoveAll((BasePlayer p) => p.Distance2D((BaseEntity)fromPlayer) > range);
		list.Sort((BasePlayer player, BasePlayer basePlayer) => (!(player.Distance2D((BaseEntity)fromPlayer) < basePlayer.Distance2D((BaseEntity)fromPlayer))) ? 1 : (-1));
		foreach (BasePlayer item in list)
		{
			text += $"{item.userID}:{item.displayName}:{item.Distance2D((BaseEntity)fromPlayer)}m\n";
			num++;
		}
		Pool.FreeList<BasePlayer>(ref list);
		text += $"{num} users within {range}m\n";
		arg.ReplyWith(text);
	}

	[ServerVar(Help = "Show user info for players on server in range of the supplied player (eg. Jim 50)")]
	public static void usersinrangeofplayer(Arg arg)
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		BasePlayer targetPlayer = arg.GetPlayerOrSleeper(0);
		if ((Object)(object)targetPlayer == (Object)null)
		{
			return;
		}
		float range = arg.GetFloat(1, 0f);
		string text = "<slot:userid:\"name\">\n";
		int num = 0;
		List<BasePlayer> list = Pool.GetList<BasePlayer>();
		Enumerator<BasePlayer> enumerator = BasePlayer.activePlayerList.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				BasePlayer current = enumerator.Current;
				list.Add(current);
			}
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
		list.RemoveAll((BasePlayer p) => p.Distance2D((BaseEntity)targetPlayer) > range);
		list.Sort((BasePlayer player, BasePlayer basePlayer) => (!(player.Distance2D((BaseEntity)targetPlayer) < basePlayer.Distance2D((BaseEntity)targetPlayer))) ? 1 : (-1));
		foreach (BasePlayer item in list)
		{
			text += $"{item.userID}:{item.displayName}:{item.Distance2D((BaseEntity)targetPlayer)}m\n";
			num++;
		}
		Pool.FreeList<BasePlayer>(ref list);
		text += $"{num} users within {range}m of {targetPlayer.displayName}\n";
		arg.ReplyWith(text);
	}

	[ServerVar(Help = "List of banned users (sourceds compat)")]
	public static void banlist(Arg arg)
	{
		arg.ReplyWith(ServerUsers.BanListString());
	}

	[ServerVar(Help = "List of banned users - shows reasons and usernames")]
	public static void banlistex(Arg arg)
	{
		arg.ReplyWith(ServerUsers.BanListStringEx());
	}

	[ServerVar(Help = "List of banned users, by ID (sourceds compat)")]
	public static void listid(Arg arg)
	{
		arg.ReplyWith(ServerUsers.BanListString(bHeader: true));
	}

	[ServerVar]
	public static void mute(Arg arg)
	{
		BasePlayer playerOrSleeper = arg.GetPlayerOrSleeper(0);
		if (!Object.op_Implicit((Object)(object)playerOrSleeper) || playerOrSleeper.net == null || playerOrSleeper.net.connection == null)
		{
			arg.ReplyWith("Player not found");
		}
		else
		{
			playerOrSleeper.SetPlayerFlag(BasePlayer.PlayerFlags.ChatMute, b: true);
		}
	}

	[ServerVar]
	public static void unmute(Arg arg)
	{
		BasePlayer playerOrSleeper = arg.GetPlayerOrSleeper(0);
		if (!Object.op_Implicit((Object)(object)playerOrSleeper) || playerOrSleeper.net == null || playerOrSleeper.net.connection == null)
		{
			arg.ReplyWith("Player not found");
		}
		else
		{
			playerOrSleeper.SetPlayerFlag(BasePlayer.PlayerFlags.ChatMute, b: false);
		}
	}

	[ServerVar(Help = "Print a list of currently muted players")]
	public static void mutelist(Arg arg)
	{
		var enumerable = from x in BasePlayer.allPlayerList
			where x.HasPlayerFlag(BasePlayer.PlayerFlags.ChatMute)
			select new
			{
				SteamId = x.UserIDString,
				Name = x.displayName
			};
		arg.ReplyWith((object)enumerable);
	}

	[ServerVar]
	public static void clientperf(Arg arg)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		string @string = arg.GetString(0, "legacy");
		int @int = arg.GetInt(1, Random.Range(int.MinValue, int.MaxValue));
		Enumerator<BasePlayer> enumerator = BasePlayer.activePlayerList.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				BasePlayer current = enumerator.Current;
				current.ClientRPCPlayer(null, current, "GetPerformanceReport", @string, @int);
			}
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
	}

	[ServerVar]
	public static void clientperf_frametime(Arg arg)
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		ClientFrametimeRequest clientFrametimeRequest = new ClientFrametimeRequest
		{
			request_id = arg.GetInt(0, Random.Range(int.MinValue, int.MaxValue)),
			start_frame = arg.GetInt(1, 0),
			max_frames = arg.GetInt(2, 1000)
		};
		Enumerator<BasePlayer> enumerator = BasePlayer.activePlayerList.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				BasePlayer current = enumerator.Current;
				current.ClientRPCPlayer(null, current, "GetPerformanceReport_Frametime", JsonConvert.SerializeObject((object)clientFrametimeRequest));
			}
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
	}

	[ServerVar(Help = "Get information about all the cars in the world")]
	public static void carstats(Arg arg)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		HashSet<ModularCar> allCarsList = ModularCar.allCarsList;
		TextTable val = new TextTable();
		val.AddColumn("id");
		val.AddColumn("sockets");
		val.AddColumn("modules");
		val.AddColumn("complete");
		val.AddColumn("engine");
		val.AddColumn("health");
		val.AddColumn("location");
		int count = allCarsList.Count;
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		foreach (ModularCar item in allCarsList)
		{
			string text = ((object)(NetworkableId)(ref item.net.ID)).ToString();
			string text2 = item.TotalSockets.ToString();
			string text3 = item.NumAttachedModules.ToString();
			string text4;
			if (item.IsComplete())
			{
				text4 = "Complete";
				num++;
			}
			else
			{
				text4 = "Partial";
			}
			string text5;
			if (item.HasAnyWorkingEngines())
			{
				text5 = "Working";
				num2++;
			}
			else
			{
				text5 = "Broken";
			}
			string text6 = ((item.TotalMaxHealth() != 0f) ? $"{item.TotalHealth() / item.TotalMaxHealth():0%}" : "0");
			string text7;
			if (item.IsOutside())
			{
				text7 = "Outside";
			}
			else
			{
				text7 = "Inside";
				num3++;
			}
			val.AddRow(new string[7] { text, text2, text3, text4, text5, text6, text7 });
		}
		string text8 = "";
		text8 = ((count != 1) ? (text8 + $"\nThe world contains {count} modular cars.") : (text8 + "\nThe world contains 1 modular car."));
		text8 = ((num != 1) ? (text8 + $"\n{num} ({(float)num / (float)count:0%}) are in a completed state.") : (text8 + $"\n1 ({1f / (float)count:0%}) is in a completed state."));
		text8 = ((num2 != 1) ? (text8 + $"\n{num2} ({(float)num2 / (float)count:0%}) are driveable.") : (text8 + $"\n1 ({1f / (float)count:0%}) is driveable."));
		arg.ReplyWith(string.Concat(str1: (num3 != 1) ? (text8 + $"\n{num3} ({(float)num3 / (float)count:0%}) are sheltered indoors.") : (text8 + $"\n1 ({1f / (float)count:0%}) is sheltered indoors."), str0: ((object)val).ToString()));
	}

	[ServerVar]
	public static string teaminfo(Arg arg)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Expected O, but got Unknown
		ulong num = arg.GetUInt64(0, 0uL);
		if (num == 0L)
		{
			BasePlayer player = arg.GetPlayer(0);
			if ((Object)(object)player == (Object)null)
			{
				return "Player not found";
			}
			num = player.userID;
		}
		RelationshipManager.PlayerTeam playerTeam = RelationshipManager.ServerInstance.FindPlayersTeam(num);
		if (playerTeam == null)
		{
			return "Player is not in a team";
		}
		TextTable val = new TextTable();
		val.AddColumn("steamID");
		val.AddColumn("username");
		val.AddColumn("online");
		val.AddColumn("leader");
		foreach (ulong memberId in playerTeam.members)
		{
			bool flag = Net.sv.connections.FirstOrDefault((Connection c) => c.connected && c.userid == memberId) != null;
			val.AddRow(new string[4]
			{
				memberId.ToString(),
				GetPlayerName(memberId),
				flag ? "x" : "",
				(memberId == playerTeam.teamLeader) ? "x" : ""
			});
		}
		if (!arg.HasArg("--json"))
		{
			return $"ID: {playerTeam.teamID}\n\n{val}";
		}
		return val.ToJson();
	}

	[ServerVar]
	public static void authradius(Arg arg)
	{
		float @float = arg.GetFloat(0, -1f);
		if (@float < 0f)
		{
			arg.ReplyWith("Format is 'authradius {radius} [user]'");
		}
		else
		{
			SetAuthInRadius(arg.GetPlayer(1) ?? arg.Player(), @float, auth: true);
		}
	}

	[ServerVar]
	public static void deauthradius(Arg arg)
	{
		float @float = arg.GetFloat(0, -1f);
		if (@float < 0f)
		{
			arg.ReplyWith("Format is 'deauthradius {radius} [user]'");
		}
		else
		{
			SetAuthInRadius(arg.GetPlayer(1) ?? arg.Player(), @float, auth: false);
		}
	}

	private static void SetAuthInRadius(BasePlayer player, float radius, bool auth)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		List<BaseEntity> list = new List<BaseEntity>();
		global::Vis.Entities(((Component)player).transform.position, radius, list, -1, (QueryTriggerInteraction)2);
		foreach (BaseEntity item in list)
		{
			if (item.isServer && !SetUserAuthorized(item, player.userID, auth))
			{
				SetUserAuthorized(item.GetSlot(BaseEntity.Slot.Lock), player.userID, auth);
			}
		}
	}

	private static bool SetUserAuthorized(BaseEntity entity, ulong userId, bool state)
	{
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Expected O, but got Unknown
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Expected O, but got Unknown
		if ((Object)(object)entity == (Object)null)
		{
			return false;
		}
		if (entity is CodeLock codeLock)
		{
			if (state)
			{
				codeLock.whitelistPlayers.Add(userId);
			}
			else
			{
				codeLock.whitelistPlayers.Remove(userId);
				codeLock.guestPlayers.Remove(userId);
			}
			codeLock.SendNetworkUpdate();
		}
		else if (entity is AutoTurret autoTurret)
		{
			if (state)
			{
				autoTurret.authorizedPlayers.Add(new PlayerNameID
				{
					ShouldPool = false,
					userid = userId,
					username = ""
				});
			}
			else
			{
				autoTurret.authorizedPlayers.RemoveAll((PlayerNameID x) => x.userid == userId);
			}
			autoTurret.SendNetworkUpdate();
		}
		else if (entity is BuildingPrivlidge buildingPrivlidge)
		{
			if (state)
			{
				buildingPrivlidge.authorizedPlayers.Add(new PlayerNameID
				{
					ShouldPool = false,
					userid = userId,
					username = ""
				});
			}
			else
			{
				buildingPrivlidge.authorizedPlayers.RemoveAll((PlayerNameID x) => x.userid == userId);
			}
			buildingPrivlidge.SendNetworkUpdate();
		}
		else
		{
			if (!(entity is ModularCar modularCar))
			{
				return false;
			}
			if (state)
			{
				modularCar.CarLock.TryAddPlayer(userId);
			}
			else
			{
				modularCar.CarLock.TryRemovePlayer(userId);
			}
			modularCar.SendNetworkUpdate();
		}
		return true;
	}

	[ServerVar]
	public static void entid(Arg arg)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		BaseEntity baseEntity = BaseNetworkable.serverEntities.Find(arg.GetEntityID(1)) as BaseEntity;
		if ((Object)(object)baseEntity == (Object)null || baseEntity is BasePlayer)
		{
			return;
		}
		string @string = arg.GetString(0, "");
		if ((Object)(object)arg.Player() != (Object)null)
		{
			Debug.Log((object)("[ENTCMD] " + arg.Player().displayName + "/" + arg.Player().userID + " used *" + @string + "* on ent: " + ((Object)baseEntity).name));
		}
		switch (@string)
		{
		case "kill":
			baseEntity.AdminKill();
			return;
		case "lock":
			baseEntity.SetFlag(BaseEntity.Flags.Locked, b: true);
			return;
		case "unlock":
			baseEntity.SetFlag(BaseEntity.Flags.Locked, b: false);
			return;
		case "debug":
			baseEntity.SetFlag(BaseEntity.Flags.Debugging, b: true);
			return;
		case "undebug":
			baseEntity.SetFlag(BaseEntity.Flags.Debugging, b: false);
			return;
		case "who":
			arg.ReplyWith(baseEntity.Admin_Who());
			return;
		case "auth":
			arg.ReplyWith(AuthList(baseEntity));
			return;
		case "upgrade":
			arg.ReplyWith(ChangeGrade(baseEntity, arg.GetInt(2, 1), 0, BuildingGrade.Enum.None, arg.GetFloat(3, 0f)));
			return;
		case "downgrade":
			arg.ReplyWith(ChangeGrade(baseEntity, 0, arg.GetInt(2, 1), BuildingGrade.Enum.None, arg.GetFloat(3, 0f)));
			return;
		case "setgrade":
			arg.ReplyWith(ChangeGrade(baseEntity, 0, 0, (BuildingGrade.Enum)arg.GetInt(2, 0), arg.GetFloat(3, 0f)));
			return;
		case "repair":
			RunInRadius(arg.GetFloat(2, 0f), baseEntity, delegate(BaseCombatEntity entity)
			{
				if (entity.repair.enabled)
				{
					entity.SetHealth(entity.MaxHealth());
				}
			});
			break;
		}
		arg.ReplyWith("Unknown command");
	}

	private static string AuthList(BaseEntity ent)
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Expected O, but got Unknown
		List<PlayerNameID> authorizedPlayers;
		if (!(ent is BuildingPrivlidge buildingPrivlidge))
		{
			if (!(ent is AutoTurret autoTurret))
			{
				if (!(ent is CodeLock codeLock))
				{
					if (ent is BaseVehicleModule vehicleModule)
					{
						return CodeLockAuthList(vehicleModule);
					}
					return "Entity has no auth list";
				}
				return CodeLockAuthList(codeLock);
			}
			authorizedPlayers = autoTurret.authorizedPlayers;
		}
		else
		{
			authorizedPlayers = buildingPrivlidge.authorizedPlayers;
		}
		if (authorizedPlayers == null || authorizedPlayers.Count == 0)
		{
			return "Nobody is authed to this entity";
		}
		TextTable val = new TextTable();
		val.AddColumn("steamID");
		val.AddColumn("username");
		foreach (PlayerNameID item in authorizedPlayers)
		{
			val.AddRow(new string[2]
			{
				item.userid.ToString(),
				GetPlayerName(item.userid)
			});
		}
		return ((object)val).ToString();
	}

	private static string CodeLockAuthList(CodeLock codeLock)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Expected O, but got Unknown
		if (codeLock.whitelistPlayers.Count == 0 && codeLock.guestPlayers.Count == 0)
		{
			return "Nobody is authed to this entity";
		}
		TextTable val = new TextTable();
		val.AddColumn("steamID");
		val.AddColumn("username");
		val.AddColumn("isGuest");
		foreach (ulong whitelistPlayer in codeLock.whitelistPlayers)
		{
			val.AddRow(new string[3]
			{
				whitelistPlayer.ToString(),
				GetPlayerName(whitelistPlayer),
				""
			});
		}
		foreach (ulong guestPlayer in codeLock.guestPlayers)
		{
			val.AddRow(new string[3]
			{
				guestPlayer.ToString(),
				GetPlayerName(guestPlayer),
				"x"
			});
		}
		return ((object)val).ToString();
	}

	private static string CodeLockAuthList(BaseVehicleModule vehicleModule)
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Expected O, but got Unknown
		if (!vehicleModule.IsOnAVehicle)
		{
			return "Nobody is authed to this entity";
		}
		ModularCar modularCar = vehicleModule.Vehicle as ModularCar;
		if ((Object)(object)modularCar == (Object)null || !modularCar.IsLockable || modularCar.CarLock.WhitelistPlayers.Count == 0)
		{
			return "Nobody is authed to this entity";
		}
		TextTable val = new TextTable();
		val.AddColumn("steamID");
		val.AddColumn("username");
		foreach (ulong whitelistPlayer in modularCar.CarLock.WhitelistPlayers)
		{
			val.AddRow(new string[2]
			{
				whitelistPlayer.ToString(),
				GetPlayerName(whitelistPlayer)
			});
		}
		return ((object)val).ToString();
	}

	public static string GetPlayerName(ulong steamId)
	{
		BasePlayer basePlayer = BasePlayer.allPlayerList.FirstOrDefault((BasePlayer p) => p.userID == steamId);
		string text;
		if (!((Object)(object)basePlayer != (Object)null))
		{
			text = SingletonComponent<ServerMgr>.Instance.persistance.GetPlayerName(steamId);
			if (text == null)
			{
				return "[unknown]";
			}
		}
		else
		{
			text = basePlayer.displayName;
		}
		return text;
	}

	public static string ChangeGrade(BaseEntity entity, int increaseBy = 0, int decreaseBy = 0, BuildingGrade.Enum targetGrade = BuildingGrade.Enum.None, float radius = 0f)
	{
		if ((Object)(object)(entity as BuildingBlock) == (Object)null)
		{
			return $"'{entity}' is not a building block";
		}
		RunInRadius(radius, entity, delegate(BuildingBlock block)
		{
			BuildingGrade.Enum grade = block.grade;
			if (targetGrade > BuildingGrade.Enum.None && targetGrade < BuildingGrade.Enum.Count)
			{
				grade = targetGrade;
			}
			else
			{
				grade = (BuildingGrade.Enum)Mathf.Min((int)(grade + increaseBy), 4);
				grade = (BuildingGrade.Enum)Mathf.Max((int)(grade - decreaseBy), 0);
			}
			if (grade != block.grade)
			{
				block.ChangeGrade(grade);
			}
		});
		int count = Pool.GetList<BuildingBlock>().Count;
		return $"Upgraded/downgraded '{count}' building block(s)";
	}

	private static bool RunInRadius<T>(float radius, BaseEntity initial, Action<T> callback, Func<T, bool> filter = null) where T : BaseEntity
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		List<T> list = Pool.GetList<T>();
		radius = Mathf.Clamp(radius, 0f, 200f);
		if (radius > 0f)
		{
			global::Vis.Entities(((Component)initial).transform.position, radius, list, 2097152, (QueryTriggerInteraction)2);
		}
		else if (initial is T item)
		{
			list.Add(item);
		}
		foreach (T item2 in list)
		{
			try
			{
				callback(item2);
			}
			catch (Exception arg)
			{
				Debug.LogError((object)$"Exception while running callback in radius: {arg}");
				return false;
			}
		}
		return true;
	}

	[ServerVar(Help = "Get a list of players")]
	public static PlayerInfo[] playerlist()
	{
		return ((IEnumerable<BasePlayer>)BasePlayer.activePlayerList).Select(delegate(BasePlayer x)
		{
			PlayerInfo result = default(PlayerInfo);
			result.SteamID = x.UserIDString;
			result.OwnerSteamID = x.OwnerID.ToString();
			result.DisplayName = x.displayName;
			result.Ping = Net.sv.GetAveragePing(x.net.connection);
			result.Address = x.net.connection.ipaddress;
			result.ConnectedSeconds = (int)x.net.connection.GetSecondsConnected();
			result.VoiationLevel = x.violationLevel;
			result.Health = x.Health();
			return result;
		}).ToArray();
	}

	[ServerVar(Help = "List of banned users")]
	public static ServerUsers.User[] Bans()
	{
		return ServerUsers.GetAll(ServerUsers.UserGroup.Banned).ToArray();
	}

	[ServerVar(Help = "Get a list of information about the server")]
	public static ServerInfoOutput ServerInfo()
	{
		ServerInfoOutput result = default(ServerInfoOutput);
		result.Hostname = Server.hostname;
		result.MaxPlayers = Server.maxplayers;
		result.Players = BasePlayer.activePlayerList.Count;
		result.Queued = SingletonComponent<ServerMgr>.Instance.connectionQueue.Queued;
		result.Joining = SingletonComponent<ServerMgr>.Instance.connectionQueue.Joining;
		result.EntityCount = BaseNetworkable.serverEntities.Count;
		result.GameTime = (((Object)(object)TOD_Sky.Instance != (Object)null) ? TOD_Sky.Instance.Cycle.DateTime.ToString() : DateTime.UtcNow.ToString());
		result.Uptime = (int)Time.realtimeSinceStartup;
		result.Map = Server.level;
		result.Framerate = Performance.report.frameRate;
		result.Memory = (int)Performance.report.memoryAllocations;
		result.MemoryUsageSystem = (int)Performance.report.memoryUsageSystem;
		result.Collections = (int)Performance.report.memoryCollections;
		result.NetworkIn = (int)((Net.sv != null) ? ((BaseNetwork)Net.sv).GetStat((Connection)null, (StatTypeLong)3) : 0);
		result.NetworkOut = (int)((Net.sv != null) ? ((BaseNetwork)Net.sv).GetStat((Connection)null, (StatTypeLong)1) : 0);
		result.Restarting = SingletonComponent<ServerMgr>.Instance.Restarting;
		result.SaveCreatedTime = SaveRestore.SaveCreatedTime.ToString();
		result.Version = 2511;
		result.Protocol = Protocol.printable;
		return result;
	}

	[ServerVar(Help = "Get information about this build")]
	public static BuildInfo BuildInfo()
	{
		return BuildInfo.Current;
	}

	[ServerVar]
	public static void AdminUI_FullRefresh(Arg arg)
	{
		AdminUI_RequestPlayerList(arg);
		AdminUI_RequestServerInfo(arg);
		AdminUI_RequestServerConvars(arg);
		AdminUI_RequestUGCList(arg);
	}

	[ServerVar]
	public static void AdminUI_RequestPlayerList(Arg arg)
	{
		if (allowAdminUI)
		{
			ConsoleNetwork.SendClientCommand(arg.Connection, "AdminUI_ReceivePlayerList", JsonConvert.SerializeObject((object)playerlist()));
		}
	}

	[ServerVar]
	public static void AdminUI_RequestServerInfo(Arg arg)
	{
		if (allowAdminUI)
		{
			ConsoleNetwork.SendClientCommand(arg.Connection, "AdminUI_ReceiveServerInfo", JsonConvert.SerializeObject((object)ServerInfo()));
		}
	}

	[ServerVar]
	public static void AdminUI_RequestServerConvars(Arg arg)
	{
		if (!allowAdminUI)
		{
			return;
		}
		List<ServerConvarInfo> list = Pool.GetList<ServerConvarInfo>();
		Command[] all = Index.All;
		foreach (Command val in all)
		{
			if (val.Server && val.Variable && val.ServerAdmin && val.ShowInAdminUI)
			{
				list.Add(new ServerConvarInfo
				{
					FullName = val.FullName,
					Value = val.GetOveride?.Invoke(),
					Help = val.Description
				});
			}
		}
		ConsoleNetwork.SendClientCommand(arg.Connection, "AdminUI_ReceiveCommands", JsonConvert.SerializeObject((object)list));
		Pool.FreeList<ServerConvarInfo>(ref list);
	}

	[ServerVar]
	public static void AdminUI_RequestUGCList(Arg arg)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		if (!allowAdminUI)
		{
			return;
		}
		List<ServerUGCInfo> list = Pool.GetList<ServerUGCInfo>();
		uint[] array = null;
		ulong[] array2 = null;
		Enumerator<BaseNetworkable> enumerator = BaseNetworkable.serverEntities.GetEnumerator();
		try
		{
			IUGCBrowserEntity iUGCBrowserEntity = default(IUGCBrowserEntity);
			while (enumerator.MoveNext())
			{
				BaseNetworkable current = enumerator.Current;
				array = null;
				array2 = null;
				UGCType uGCType = UGCType.ImageJpg;
				if (((Component)current).TryGetComponent<IUGCBrowserEntity>(ref iUGCBrowserEntity))
				{
					array = iUGCBrowserEntity.GetContentCRCs;
					array2 = iUGCBrowserEntity.EditingHistory.ToArray();
					uGCType = iUGCBrowserEntity.ContentType;
				}
				if (array == null || array.Length == 0)
				{
					continue;
				}
				bool flag = false;
				uint[] array3 = array;
				for (int i = 0; i < array3.Length; i++)
				{
					if (array3[i] != 0)
					{
						flag = true;
						break;
					}
				}
				if (uGCType == UGCType.PatternBoomer)
				{
					flag = true;
				}
				if (flag)
				{
					list.Add(new ServerUGCInfo
					{
						entityId = current.net.ID,
						crcs = array,
						contentType = uGCType,
						entityPrefabID = current.prefabID,
						shortPrefabName = current.ShortPrefabName,
						playerIds = array2
					});
				}
			}
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
		ConsoleNetwork.SendClientCommand(arg.Connection, "AdminUI_ReceiveUGCList", JsonConvert.SerializeObject((object)list));
		Pool.FreeList<ServerUGCInfo>(ref list);
	}

	[ServerVar]
	public static void AdminUI_RequestUGCContent(Arg arg)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		if (allowAdminUI && !((Object)(object)arg.Player() == (Object)null))
		{
			uint uInt = arg.GetUInt(0, 0u);
			NetworkableId entityID = arg.GetEntityID(1);
			FileStorage.Type @int = (FileStorage.Type)arg.GetInt(2, 0);
			uint uInt2 = arg.GetUInt(3, 0u);
			byte[] array = FileStorage.server.Get(uInt, @int, entityID, uInt2);
			if (array != null)
			{
				SendInfo val = default(SendInfo);
				((SendInfo)(ref val))._002Ector(arg.Connection);
				val.channel = 2;
				val.method = (SendMethod)0;
				SendInfo sendInfo = val;
				arg.Player().ClientRPCEx(sendInfo, null, "AdminReceivedUGC", uInt, (uint)array.Length, array, uInt2, (byte)@int);
			}
		}
	}

	[ServerVar]
	public static void AdminUI_DeleteUGCContent(Arg arg)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		if (!allowAdminUI)
		{
			return;
		}
		NetworkableId entityID = arg.GetEntityID(0);
		BaseNetworkable baseNetworkable = BaseNetworkable.serverEntities.Find(entityID);
		if ((Object)(object)baseNetworkable != (Object)null)
		{
			FileStorage.server.RemoveAllByEntity(entityID);
			IUGCBrowserEntity iUGCBrowserEntity = default(IUGCBrowserEntity);
			if (((Component)baseNetworkable).TryGetComponent<IUGCBrowserEntity>(ref iUGCBrowserEntity))
			{
				iUGCBrowserEntity.ClearContent();
			}
		}
	}

	[ServerVar]
	public static void AdminUI_RequestFireworkPattern(Arg arg)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		if (allowAdminUI)
		{
			NetworkableId entityID = arg.GetEntityID(0);
			BaseNetworkable baseNetworkable = BaseNetworkable.serverEntities.Find(entityID);
			if ((Object)(object)baseNetworkable != (Object)null && baseNetworkable is PatternFirework patternFirework)
			{
				SendInfo val = default(SendInfo);
				((SendInfo)(ref val))._002Ector(arg.Connection);
				val.channel = 2;
				val.method = (SendMethod)0;
				SendInfo sendInfo = val;
				arg.Player().ClientRPCEx<NetworkableId, byte[]>(sendInfo, null, "AdminReceivedPatternFirework", entityID, patternFirework.Design.ToProtoBytes());
			}
		}
	}

	[ServerVar]
	public static void clearugcentity(Arg arg)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		NetworkableId entityID = arg.GetEntityID(0);
		BaseNetworkable baseNetworkable = BaseNetworkable.serverEntities.Find(entityID);
		IUGCBrowserEntity iUGCBrowserEntity = default(IUGCBrowserEntity);
		if ((Object)(object)baseNetworkable != (Object)null && ((Component)baseNetworkable).TryGetComponent<IUGCBrowserEntity>(ref iUGCBrowserEntity))
		{
			iUGCBrowserEntity.ClearContent();
			arg.ReplyWith($"Cleared content on {baseNetworkable.ShortPrefabName}/{entityID}");
		}
		else
		{
			arg.ReplyWith($"Could not find UGC entity with id {entityID}");
		}
	}

	[ServerVar]
	public static void clearugcentitiesinrange(Arg arg)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		Vector3 vector = arg.GetVector3(0, default(Vector3));
		float @float = arg.GetFloat(1, 0f);
		int num = 0;
		Enumerator<BaseNetworkable> enumerator = BaseNetworkable.serverEntities.GetEnumerator();
		try
		{
			IUGCBrowserEntity iUGCBrowserEntity = default(IUGCBrowserEntity);
			while (enumerator.MoveNext())
			{
				BaseNetworkable current = enumerator.Current;
				if (((Component)current).TryGetComponent<IUGCBrowserEntity>(ref iUGCBrowserEntity) && Vector3.Distance(((Component)current).transform.position, vector) <= @float)
				{
					iUGCBrowserEntity.ClearContent();
					num++;
				}
			}
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
		arg.ReplyWith($"Cleared {num} UGC entities within {@float}m of {vector}");
	}

	[ServerVar]
	public static void getugcinfo(Arg arg)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		NetworkableId entityID = arg.GetEntityID(0);
		BaseNetworkable baseNetworkable = BaseNetworkable.serverEntities.Find(entityID);
		IUGCBrowserEntity fromEntity = default(IUGCBrowserEntity);
		if ((Object)(object)baseNetworkable != (Object)null && ((Component)baseNetworkable).TryGetComponent<IUGCBrowserEntity>(ref fromEntity))
		{
			ServerUGCInfo serverUGCInfo = new ServerUGCInfo(fromEntity);
			arg.ReplyWith(JsonConvert.SerializeObject((object)serverUGCInfo));
		}
		else
		{
			arg.ReplyWith($"Invalid entity id: {entityID}");
		}
	}

	[ServerVar(Help = "Returns all entities that the provided player is authed to (TC's, locks, etc), supports --json")]
	public static void authcount(Arg arg)
	{
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Expected O, but got Unknown
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		BasePlayer playerOrSleeper = arg.GetPlayerOrSleeper(0);
		if ((Object)(object)playerOrSleeper == (Object)null)
		{
			arg.ReplyWith("Please provide a valid player, unable to find '" + arg.GetString(0, "") + "'");
			return;
		}
		string text = arg.GetString(1, "");
		if (text == "--json")
		{
			text = string.Empty;
		}
		List<EntityAssociation> list = Pool.GetList<EntityAssociation>();
		FindEntityAssociationsForPlayer(playerOrSleeper, useOwnerId: false, useAuth: true, text, list);
		TextTable val = new TextTable();
		val.AddColumns(new string[4] { "Prefab name", "Position", "ID", "Type" });
		foreach (EntityAssociation item in list)
		{
			string[] obj = new string[4]
			{
				item.TargetEntity.ShortPrefabName,
				null,
				null,
				null
			};
			Vector3 position = ((Component)item.TargetEntity).transform.position;
			obj[1] = ((object)(Vector3)(ref position)).ToString();
			obj[2] = ((object)(NetworkableId)(ref item.TargetEntity.net.ID)).ToString();
			obj[3] = item.AssociationType.ToString();
			val.AddRow(obj);
		}
		Pool.FreeList<EntityAssociation>(ref list);
		if (arg.HasArg("--json"))
		{
			arg.ReplyWith(val.ToJson());
			return;
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("Found entities " + playerOrSleeper.displayName + " is authed to");
		stringBuilder.AppendLine(((object)val).ToString());
		arg.ReplyWith(stringBuilder.ToString());
	}

	[ServerVar(Help = "Returns all entities that the provided player has placed, supports --json")]
	public static void entcount(Arg arg)
	{
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Expected O, but got Unknown
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		BasePlayer playerOrSleeper = arg.GetPlayerOrSleeper(0);
		if ((Object)(object)playerOrSleeper == (Object)null)
		{
			arg.ReplyWith("Please provide a valid player, unable to find '" + arg.GetString(0, "") + "'");
			return;
		}
		string text = arg.GetString(1, "");
		if (text == "--json")
		{
			text = string.Empty;
		}
		List<EntityAssociation> list = Pool.GetList<EntityAssociation>();
		FindEntityAssociationsForPlayer(playerOrSleeper, useOwnerId: true, useAuth: false, text, list);
		TextTable val = new TextTable();
		val.AddColumns(new string[3] { "Prefab name", "Position", "ID" });
		foreach (EntityAssociation item in list)
		{
			string[] obj = new string[3]
			{
				item.TargetEntity.ShortPrefabName,
				null,
				null
			};
			Vector3 position = ((Component)item.TargetEntity).transform.position;
			obj[1] = ((object)(Vector3)(ref position)).ToString();
			obj[2] = ((object)(NetworkableId)(ref item.TargetEntity.net.ID)).ToString();
			val.AddRow(obj);
		}
		Pool.FreeList<EntityAssociation>(ref list);
		if (arg.HasArg("--json"))
		{
			arg.ReplyWith(val.ToJson());
			return;
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("Found entities associated with " + playerOrSleeper.displayName);
		stringBuilder.AppendLine(((object)val).ToString());
		arg.ReplyWith(stringBuilder.ToString());
	}

	private static void FindEntityAssociationsForPlayer(BasePlayer ply, bool useOwnerId, bool useAuth, string filter, List<EntityAssociation> results)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		results.Clear();
		Enumerator<BaseNetworkable> enumerator = BaseNetworkable.serverEntities.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				BaseNetworkable current = enumerator.Current;
				EntityAssociationType entityAssociationType = EntityAssociationType.Owner;
				if (!(current is BaseEntity baseEntity))
				{
					continue;
				}
				bool flag = false;
				if (useOwnerId && baseEntity.OwnerID == ply.userID)
				{
					flag = true;
				}
				if (useAuth && !flag)
				{
					if (!flag && baseEntity is BuildingPrivlidge buildingPrivlidge && buildingPrivlidge.IsAuthed(ply.userID))
					{
						flag = true;
					}
					if (!flag && baseEntity is KeyLock keyLock && keyLock.HasLockPermission(ply))
					{
						flag = true;
					}
					else if (baseEntity is CodeLock codeLock)
					{
						if (codeLock.whitelistPlayers.Contains(ply.userID))
						{
							flag = true;
						}
						else if (codeLock.guestPlayers.Contains(ply.userID))
						{
							flag = true;
							entityAssociationType = EntityAssociationType.LockGuest;
						}
					}
					if (!flag && baseEntity is ModularCar modularCar && modularCar.IsLockable && modularCar.CarLock.HasLockPermission(ply))
					{
						flag = true;
					}
					if (flag && entityAssociationType == EntityAssociationType.Owner)
					{
						entityAssociationType = EntityAssociationType.Auth;
					}
				}
				if (flag && !string.IsNullOrEmpty(filter) && !StringEx.Contains(current.ShortPrefabName, filter, CompareOptions.IgnoreCase))
				{
					flag = false;
				}
				if (flag)
				{
					results.Add(new EntityAssociation
					{
						TargetEntity = baseEntity,
						AssociationType = entityAssociationType
					});
				}
			}
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
	}
}
