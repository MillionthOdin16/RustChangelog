using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CircularBuffer;
using CompanionServer;
using Facepunch;
using Facepunch.Math;
using Facepunch.Rust;
using Network;
using UnityEngine;

namespace ConVar;

[Factory("chat")]
public class Chat : ConsoleSystem
{
	public enum ChatChannel
	{
		Global,
		Team,
		Server,
		Cards,
		Local
	}

	public struct ChatEntry
	{
		public ChatChannel Channel { get; set; }

		public string Message { get; set; }

		public string UserId { get; set; }

		public string Username { get; set; }

		public string Color { get; set; }

		public int Time { get; set; }
	}

	[ServerVar]
	public static float localChatRange = 100f;

	[ReplicatedVar]
	public static bool globalchat = true;

	[ReplicatedVar]
	public static bool localchat = false;

	private const float textVolumeBoost = 0.2f;

	[ServerVar]
	[ClientVar]
	public static bool enabled = true;

	[ServerVar(Help = "Number of messages to keep in memory for chat history")]
	public static int historysize = 1000;

	private static CircularBuffer<ChatEntry> History = new CircularBuffer<ChatEntry>(historysize);

	[ServerVar]
	public static bool serverlog = true;

	public static void Broadcast(string message, string username = "SERVER", string color = "#eee", ulong userid = 0uL)
	{
		string text = StringEx.EscapeRichText(username);
		ConsoleNetwork.BroadcastToAllClients("chat.add", 2, 0, "<color=" + color + ">" + text + "</color> " + message);
		ChatEntry chatEntry = default(ChatEntry);
		chatEntry.Channel = ChatChannel.Server;
		chatEntry.Message = message;
		chatEntry.UserId = userid.ToString();
		chatEntry.Username = username;
		chatEntry.Color = color;
		chatEntry.Time = Epoch.Current;
		ChatEntry ce = chatEntry;
		Record(ce);
	}

	[ServerUserVar]
	public static void say(Arg arg)
	{
		if (globalchat)
		{
			sayImpl(ChatChannel.Global, arg);
		}
	}

	[ServerUserVar]
	public static void localsay(Arg arg)
	{
		if (localchat)
		{
			sayImpl(ChatChannel.Local, arg);
		}
	}

	[ServerUserVar]
	public static void teamsay(Arg arg)
	{
		sayImpl(ChatChannel.Team, arg);
	}

	[ServerUserVar]
	public static void cardgamesay(Arg arg)
	{
		sayImpl(ChatChannel.Cards, arg);
	}

	private static void sayImpl(ChatChannel targetChannel, Arg arg)
	{
		if (!enabled)
		{
			arg.ReplyWith("Chat is disabled.");
			return;
		}
		BasePlayer basePlayer = arg.Player();
		if (!Object.op_Implicit((Object)(object)basePlayer) || basePlayer.HasPlayerFlag(BasePlayer.PlayerFlags.ChatMute))
		{
			return;
		}
		if (!basePlayer.IsAdmin && !basePlayer.IsDeveloper)
		{
			if (basePlayer.NextChatTime == 0f)
			{
				basePlayer.NextChatTime = Time.realtimeSinceStartup - 30f;
			}
			if (basePlayer.NextChatTime > Time.realtimeSinceStartup)
			{
				basePlayer.NextChatTime += 2f;
				float num = basePlayer.NextChatTime - Time.realtimeSinceStartup;
				ConsoleNetwork.SendClientCommand(basePlayer.net.connection, "chat.add", 2, 0, "You're chatting too fast - try again in " + (num + 0.5f).ToString("0") + " seconds");
				if (num > 120f)
				{
					basePlayer.Kick("Chatting too fast");
				}
				return;
			}
		}
		string @string = arg.GetString(0, "text");
		bool flag = sayAs(targetChannel, basePlayer.userID, basePlayer.displayName, @string, basePlayer);
		Analytics.Azure.OnChatMessage(basePlayer, @string, (int)targetChannel);
		if (flag)
		{
			basePlayer.NextChatTime = Time.realtimeSinceStartup + 1.5f;
		}
	}

	internal static bool sayAs(ChatChannel targetChannel, ulong userId, string username, string message, BasePlayer player = null)
	{
		//IL_04c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f7: Unknown result type (might be due to invalid IL or missing references)
		if (!Object.op_Implicit((Object)(object)player))
		{
			player = null;
		}
		if (!enabled)
		{
			return false;
		}
		if ((Object)(object)player != (Object)null && player.HasPlayerFlag(BasePlayer.PlayerFlags.ChatMute))
		{
			return false;
		}
		ServerUsers.UserGroup userGroup = ServerUsers.Get(userId)?.group ?? ServerUsers.UserGroup.None;
		if (userGroup == ServerUsers.UserGroup.Banned)
		{
			return false;
		}
		string text = message.Replace("\n", "").Replace("\r", "").Trim();
		if (text.Length > 128)
		{
			text = text.Substring(0, 128);
		}
		if (text.Length <= 0)
		{
			return false;
		}
		if (text.StartsWith("/") || text.StartsWith("\\"))
		{
			return false;
		}
		text = StringEx.EscapeRichText(text);
		if (Server.emojiOwnershipCheck)
		{
			List<(TmProEmojiRedirector.EmojiSub, int)> list = Pool.GetList<(TmProEmojiRedirector.EmojiSub, int)>();
			TmProEmojiRedirector.FindEmojiSubstitutions(text, RustEmojiLibrary.Instance, list, isServer: true);
			bool flag = true;
			foreach (var item in list)
			{
				if (!item.Item1.targetEmojiResult.CanBeUsedBy(player))
				{
					flag = false;
					break;
				}
			}
			Pool.FreeList<(TmProEmojiRedirector.EmojiSub, int)>(ref list);
			if (!flag)
			{
				Debug.Log((object)"player tried to use emoji they don't own, reject!");
				return false;
			}
		}
		if (serverlog)
		{
			ServerConsole.PrintColoured(ConsoleColor.DarkYellow, string.Concat("[", targetChannel, "] ", username, ": "), ConsoleColor.DarkGreen, text);
			string text2 = ((object)player)?.ToString() ?? $"{username}[{userId}]";
			switch (targetChannel)
			{
			case ChatChannel.Team:
				DebugEx.Log((object)("[TEAM CHAT] " + text2 + " : " + text), (StackTraceLogType)0);
				break;
			case ChatChannel.Cards:
				DebugEx.Log((object)("[CARDS CHAT] " + text2 + " : " + text), (StackTraceLogType)0);
				break;
			default:
				DebugEx.Log((object)("[CHAT] " + text2 + " : " + text), (StackTraceLogType)0);
				break;
			}
		}
		bool flag2 = userGroup == ServerUsers.UserGroup.Owner || userGroup == ServerUsers.UserGroup.Moderator;
		bool flag3 = (((Object)(object)player != (Object)null) ? player.IsDeveloper : DeveloperList.Contains(userId));
		string text3 = "#5af";
		if (flag2)
		{
			text3 = "#af5";
		}
		if (flag3)
		{
			text3 = "#fa5";
		}
		string text4 = StringEx.EscapeRichText(username);
		ChatEntry chatEntry = default(ChatEntry);
		chatEntry.Channel = targetChannel;
		chatEntry.Message = text;
		chatEntry.UserId = (((Object)(object)player != (Object)null) ? player.UserIDString : userId.ToString());
		chatEntry.Username = username;
		chatEntry.Color = text3;
		chatEntry.Time = Epoch.Current;
		ChatEntry ce = chatEntry;
		Record(ce);
		switch (targetChannel)
		{
		case ChatChannel.Cards:
		{
			if ((Object)(object)player == (Object)null)
			{
				return false;
			}
			if (!player.isMounted)
			{
				return false;
			}
			BaseCardGameEntity baseCardGameEntity = player.GetMountedVehicle() as BaseCardGameEntity;
			if ((Object)(object)baseCardGameEntity == (Object)null || !baseCardGameEntity.GameController.IsAtTable(player))
			{
				return false;
			}
			List<Connection> list2 = Pool.GetList<Connection>();
			baseCardGameEntity.GameController.GetConnectionsInGame(list2);
			if (list2.Count > 0)
			{
				ConsoleNetwork.SendClientCommand(list2, "chat.add2", 3, userId, text, text4, text3, 1f);
			}
			Pool.FreeList<Connection>(ref list2);
			return true;
		}
		case ChatChannel.Global:
			ConsoleNetwork.BroadcastToAllClients("chat.add2", 0, userId, text, text4, text3, 1f);
			return true;
		case ChatChannel.Local:
		{
			if (!((Object)(object)player != (Object)null))
			{
				break;
			}
			float num = localChatRange * localChatRange;
			Enumerator<BasePlayer> enumerator2 = BasePlayer.activePlayerList.GetEnumerator();
			try
			{
				while (enumerator2.MoveNext())
				{
					BasePlayer current = enumerator2.Current;
					Vector3 val = ((Component)current).transform.position - ((Component)player).transform.position;
					float sqrMagnitude = ((Vector3)(ref val)).sqrMagnitude;
					if (!(sqrMagnitude > num))
					{
						ConsoleNetwork.SendClientCommand(current.net.connection, "chat.add2", 4, userId, text, text4, text3, Mathf.Clamp01(sqrMagnitude / num + 0.2f));
					}
				}
			}
			finally
			{
				((IDisposable)enumerator2).Dispose();
			}
			return true;
		}
		case ChatChannel.Team:
		{
			RelationshipManager.PlayerTeam playerTeam = RelationshipManager.ServerInstance.FindPlayersTeam(userId);
			if (playerTeam == null)
			{
				return false;
			}
			List<Connection> onlineMemberConnections = playerTeam.GetOnlineMemberConnections();
			if (onlineMemberConnections != null)
			{
				ConsoleNetwork.SendClientCommand(onlineMemberConnections, "chat.add2", 1, userId, text, text4, text3, 1f);
			}
			playerTeam.BroadcastTeamChat(userId, text4, text, text3);
			return true;
		}
		}
		return false;
	}

	[ServerVar]
	[Help("Return the last x lines of the console. Default is 200")]
	public static IEnumerable<ChatEntry> tail(Arg arg)
	{
		int @int = arg.GetInt(0, 200);
		int num = History.Size - @int;
		if (num < 0)
		{
			num = 0;
		}
		return ((IEnumerable<ChatEntry>)History).Skip(num);
	}

	[ServerVar]
	[Help("Search the console for a particular string")]
	public static IEnumerable<ChatEntry> search(Arg arg)
	{
		string search = arg.GetString(0, (string)null);
		if (search == null)
		{
			return Enumerable.Empty<ChatEntry>();
		}
		return ((IEnumerable<ChatEntry>)History).Where((ChatEntry x) => x.Message.Length < 4096 && StringEx.Contains(x.Message, search, CompareOptions.IgnoreCase));
	}

	private static void Record(ChatEntry ce)
	{
		int num = Mathf.Max(historysize, 10);
		if (History.Capacity != num)
		{
			CircularBuffer<ChatEntry> val = new CircularBuffer<ChatEntry>(num);
			foreach (ChatEntry item in History)
			{
				val.PushBack(item);
			}
			History = val;
		}
		History.PushBack(ce);
		RCon.Broadcast(RCon.LogType.Chat, ce);
	}
}
