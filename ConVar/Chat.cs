using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
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
		Local,
		Clan,
		MaxValue
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

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	private struct _003CsayAs_003Ed__18 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncValueTaskMethodBuilder<bool> _003C_003Et__builder;

		public BasePlayer player;

		public ulong userId;

		public string message;

		public ChatChannel targetChannel;

		public string username;

		private string _003CstrChatText_003E5__2;

		private string _003CstrName_003E5__3;

		private ValueTaskAwaiter<ClanValueResult<IClan>> _003C_003Eu__1;

		private ValueTaskAwaiter<ClanResult> _003C_003Eu__2;

		private void MoveNext()
		{
			//IL_071e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0723: Unknown result type (might be due to invalid IL or missing references)
			//IL_072b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0790: Unknown result type (might be due to invalid IL or missing references)
			//IL_0795: Unknown result type (might be due to invalid IL or missing references)
			//IL_079d: Unknown result type (might be due to invalid IL or missing references)
			//IL_081d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0822: Unknown result type (might be due to invalid IL or missing references)
			//IL_082a: Unknown result type (might be due to invalid IL or missing references)
			//IL_073c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0741: Unknown result type (might be due to invalid IL or missing references)
			//IL_07ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_083b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0841: Invalid comparison between Unknown and I4
			//IL_075b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0760: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_07b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0775: Unknown result type (might be due to invalid IL or missing references)
			//IL_0777: Unknown result type (might be due to invalid IL or missing references)
			//IL_06e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_06ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_07eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_07f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_0703: Unknown result type (might be due to invalid IL or missing references)
			//IL_0705: Unknown result type (might be due to invalid IL or missing references)
			//IL_0805: Unknown result type (might be due to invalid IL or missing references)
			//IL_0807: Unknown result type (might be due to invalid IL or missing references)
			//IL_04fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0502: Unknown result type (might be due to invalid IL or missing references)
			//IL_0519: Unknown result type (might be due to invalid IL or missing references)
			//IL_0529: Unknown result type (might be due to invalid IL or missing references)
			//IL_052e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0533: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			bool result;
			try
			{
				if ((uint)num <= 2u)
				{
					goto IL_069e;
				}
				if (!Object.op_Implicit((Object)(object)player))
				{
					player = null;
				}
				if (!enabled)
				{
					result = false;
				}
				else if ((Object)(object)player != (Object)null && player.HasPlayerFlag(BasePlayer.PlayerFlags.ChatMute))
				{
					result = false;
				}
				else if ((ServerUsers.Get(userId)?.group ?? ServerUsers.UserGroup.None) == ServerUsers.UserGroup.Banned)
				{
					result = false;
				}
				else
				{
					_003CstrChatText_003E5__2 = message.Replace("\n", "").Replace("\r", "").Trim();
					if (_003CstrChatText_003E5__2.Length > 128)
					{
						_003CstrChatText_003E5__2 = _003CstrChatText_003E5__2.Substring(0, 128);
					}
					if (_003CstrChatText_003E5__2.Length <= 0)
					{
						result = false;
					}
					else if (_003CstrChatText_003E5__2.StartsWith("/") || _003CstrChatText_003E5__2.StartsWith("\\"))
					{
						result = false;
					}
					else
					{
						_003CstrChatText_003E5__2 = StringEx.EscapeRichText(_003CstrChatText_003E5__2);
						if (!Server.emojiOwnershipCheck)
						{
							goto IL_01b5;
						}
						List<(TmProEmojiRedirector.EmojiSub, int)> list = Pool.GetList<(TmProEmojiRedirector.EmojiSub, int)>();
						TmProEmojiRedirector.FindEmojiSubstitutions(_003CstrChatText_003E5__2, RustEmojiLibrary.Instance, list, isServer: true);
						bool flag = true;
						List<(TmProEmojiRedirector.EmojiSub, int)>.Enumerator enumerator = list.GetEnumerator();
						try
						{
							while (enumerator.MoveNext())
							{
								if (!enumerator.Current.Item1.targetEmojiResult.CanBeUsedBy(player))
								{
									flag = false;
									break;
								}
							}
						}
						finally
						{
							if (num < 0)
							{
								((IDisposable)enumerator).Dispose();
							}
						}
						Pool.FreeList<(TmProEmojiRedirector.EmojiSub, int)>(ref list);
						if (flag)
						{
							goto IL_01b5;
						}
						Debug.Log((object)"player tried to use emoji they don't own, reject!");
						result = false;
					}
				}
				goto end_IL_0007;
				IL_069e:
				ClanManager serverInstance = default(ClanManager);
				try
				{
					ValueTaskAwaiter<ClanValueResult<IClan>> awaiter2;
					ValueTaskAwaiter<ClanResult> awaiter;
					ClanValueResult<IClan> result2;
					ClanValueResult<IClan> val;
					switch (num)
					{
					default:
						if ((Object)(object)player != (Object)null && player.clanId != 0L)
						{
							awaiter2 = serverInstance.Backend.Get(player.clanId).GetAwaiter();
							if (!awaiter2.IsCompleted)
							{
								num = (_003C_003E1__state = 0);
								_003C_003Eu__1 = awaiter2;
								_003C_003Et__builder.AwaitUnsafeOnCompleted<ValueTaskAwaiter<ClanValueResult<IClan>>, _003CsayAs_003Ed__18>(ref awaiter2, ref this);
								return;
							}
							goto IL_073a;
						}
						awaiter2 = serverInstance.Backend.GetByMember(userId).GetAwaiter();
						if (!awaiter2.IsCompleted)
						{
							num = (_003C_003E1__state = 1);
							_003C_003Eu__1 = awaiter2;
							_003C_003Et__builder.AwaitUnsafeOnCompleted<ValueTaskAwaiter<ClanValueResult<IClan>>, _003CsayAs_003Ed__18>(ref awaiter2, ref this);
							return;
						}
						goto IL_07ac;
					case 0:
						awaiter2 = _003C_003Eu__1;
						_003C_003Eu__1 = default(ValueTaskAwaiter<ClanValueResult<IClan>>);
						num = (_003C_003E1__state = -1);
						goto IL_073a;
					case 1:
						awaiter2 = _003C_003Eu__1;
						_003C_003Eu__1 = default(ValueTaskAwaiter<ClanValueResult<IClan>>);
						num = (_003C_003E1__state = -1);
						goto IL_07ac;
					case 2:
						{
							awaiter = _003C_003Eu__2;
							_003C_003Eu__2 = default(ValueTaskAwaiter<ClanResult>);
							num = (_003C_003E1__state = -1);
							break;
						}
						IL_073a:
						result2 = awaiter2.GetResult();
						goto IL_07b5;
						IL_07b5:
						val = result2;
						if (val.IsSuccess)
						{
							awaiter = val.Value.SendChatMessage(_003CstrName_003E5__3, _003CstrChatText_003E5__2, userId).GetAwaiter();
							if (!awaiter.IsCompleted)
							{
								num = (_003C_003E1__state = 2);
								_003C_003Eu__2 = awaiter;
								_003C_003Et__builder.AwaitUnsafeOnCompleted<ValueTaskAwaiter<ClanResult>, _003CsayAs_003Ed__18>(ref awaiter, ref this);
								return;
							}
							break;
						}
						result = false;
						goto end_IL_069e;
						IL_07ac:
						result2 = awaiter2.GetResult();
						goto IL_07b5;
					}
					result = (int)awaiter.GetResult() == 1;
					end_IL_069e:;
				}
				catch (Exception ex)
				{
					Debug.LogError((object)ex);
					result = false;
				}
				goto end_IL_0007;
				IL_01b5:
				if (serverlog)
				{
					ServerConsole.PrintColoured(ConsoleColor.DarkYellow, string.Concat("[", targetChannel, "] ", username, ": "), ConsoleColor.DarkGreen, _003CstrChatText_003E5__2);
					string text = ((object)player)?.ToString() ?? $"{username}[{userId}]";
					if (targetChannel == ChatChannel.Team)
					{
						DebugEx.Log((object)("[TEAM CHAT] " + text + " : " + _003CstrChatText_003E5__2), (StackTraceLogType)0);
					}
					else if (targetChannel == ChatChannel.Cards)
					{
						DebugEx.Log((object)("[CARDS CHAT] " + text + " : " + _003CstrChatText_003E5__2), (StackTraceLogType)0);
					}
					else if (targetChannel == ChatChannel.Clan)
					{
						DebugEx.Log((object)("[CLAN CHAT] " + text + " : " + _003CstrChatText_003E5__2), (StackTraceLogType)0);
					}
					else
					{
						DebugEx.Log((object)("[CHAT] " + text + " : " + _003CstrChatText_003E5__2), (StackTraceLogType)0);
					}
				}
				_003CstrName_003E5__3 = StringEx.EscapeRichText(username);
				string nameColor = GetNameColor(userId, player);
				ChatEntry ce = default(ChatEntry);
				ce.Channel = targetChannel;
				ce.Message = _003CstrChatText_003E5__2;
				ce.UserId = (((Object)(object)player != (Object)null) ? player.UserIDString : userId.ToString());
				ce.Username = username;
				ce.Color = nameColor;
				ce.Time = Epoch.Current;
				Record(ce);
				switch (targetChannel)
				{
				case ChatChannel.Cards:
					if ((Object)(object)player == (Object)null)
					{
						result = false;
					}
					else if (!player.isMounted)
					{
						result = false;
					}
					else
					{
						BaseCardGameEntity baseCardGameEntity = player.GetMountedVehicle() as BaseCardGameEntity;
						if ((Object)(object)baseCardGameEntity == (Object)null || !baseCardGameEntity.GameController.IsAtTable(player))
						{
							result = false;
						}
						else
						{
							List<Connection> list2 = Pool.GetList<Connection>();
							baseCardGameEntity.GameController.GetConnectionsInGame(list2);
							if (list2.Count > 0)
							{
								ConsoleNetwork.SendClientCommand(list2, "chat.add2", 3, userId, _003CstrChatText_003E5__2, _003CstrName_003E5__3, nameColor, 1f);
							}
							Pool.FreeList<Connection>(ref list2);
							result = true;
						}
					}
					goto end_IL_0007;
				case ChatChannel.Global:
					ConsoleNetwork.BroadcastToAllClients("chat.add2", 0, userId, _003CstrChatText_003E5__2, _003CstrName_003E5__3, nameColor, 1f);
					result = true;
					goto end_IL_0007;
				case ChatChannel.Local:
				{
					if (!((Object)(object)player != (Object)null))
					{
						goto default;
					}
					float num2 = localChatRange * localChatRange;
					Enumerator<BasePlayer> enumerator2 = BasePlayer.activePlayerList.GetEnumerator();
					try
					{
						while (enumerator2.MoveNext())
						{
							BasePlayer current = enumerator2.Current;
							Vector3 val2 = ((Component)current).transform.position - ((Component)player).transform.position;
							float sqrMagnitude = ((Vector3)(ref val2)).sqrMagnitude;
							if (!(sqrMagnitude > num2))
							{
								ConsoleNetwork.SendClientCommand(current.net.connection, "chat.add2", 4, userId, _003CstrChatText_003E5__2, _003CstrName_003E5__3, nameColor, Mathf.Clamp01(sqrMagnitude / num2 + 0.2f));
							}
						}
					}
					finally
					{
						if (num < 0)
						{
							((IDisposable)enumerator2).Dispose();
						}
					}
					result = true;
					goto end_IL_0007;
				}
				case ChatChannel.Team:
				{
					RelationshipManager.PlayerTeam playerTeam = RelationshipManager.ServerInstance.FindPlayersTeam(userId);
					if (playerTeam == null)
					{
						result = false;
					}
					else
					{
						List<Connection> onlineMemberConnections = playerTeam.GetOnlineMemberConnections();
						if (onlineMemberConnections != null)
						{
							ConsoleNetwork.SendClientCommand(onlineMemberConnections, "chat.add2", 1, userId, _003CstrChatText_003E5__2, _003CstrName_003E5__3, nameColor, 1f);
						}
						playerTeam.BroadcastTeamChat(userId, _003CstrName_003E5__3, _003CstrChatText_003E5__2, nameColor);
						result = true;
					}
					goto end_IL_0007;
				}
				case ChatChannel.Clan:
					serverInstance = ClanManager.ServerInstance;
					if ((Object)(object)serverInstance == (Object)null)
					{
						result = false;
					}
					else
					{
						if (!((Object)(object)player != (Object)null) || player.clanId != 0L)
						{
							break;
						}
						result = false;
					}
					goto end_IL_0007;
				default:
					result = false;
					goto end_IL_0007;
				}
				goto IL_069e;
				end_IL_0007:;
			}
			catch (Exception exception)
			{
				_003C_003E1__state = -2;
				_003C_003Et__builder.SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003C_003Et__builder.SetResult(result);
		}

		void IAsyncStateMachine.MoveNext()
		{
			//ILSpy generated this explicit interface implementation from .override directive in MoveNext
			this.MoveNext();
		}

		[DebuggerHidden]
		private void SetStateMachine(IAsyncStateMachine stateMachine)
		{
			_003C_003Et__builder.SetStateMachine(stateMachine);
		}

		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
		{
			//ILSpy generated this explicit interface implementation from .override directive in SetStateMachine
			this.SetStateMachine(stateMachine);
		}
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
		ChatEntry ce = default(ChatEntry);
		ce.Channel = ChatChannel.Server;
		ce.Message = message;
		ce.UserId = userid.ToString();
		ce.Username = username;
		ce.Color = color;
		ce.Time = Epoch.Current;
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

	[ServerUserVar]
	public static void clansay(Arg arg)
	{
		sayImpl(ChatChannel.Clan, arg);
	}

	private static void sayImpl(ChatChannel targetChannel, Arg arg)
	{
		if (!enabled)
		{
			arg.ReplyWith("Chat is disabled.");
			return;
		}
		BasePlayer player = arg.Player();
		if (!Object.op_Implicit((Object)(object)player) || player.HasPlayerFlag(BasePlayer.PlayerFlags.ChatMute))
		{
			return;
		}
		if (!player.IsAdmin && !player.IsDeveloper)
		{
			if (player.NextChatTime == 0f)
			{
				player.NextChatTime = Time.realtimeSinceStartup - 30f;
			}
			if (player.NextChatTime > Time.realtimeSinceStartup)
			{
				player.NextChatTime += 2f;
				float num = player.NextChatTime - Time.realtimeSinceStartup;
				ConsoleNetwork.SendClientCommand(player.net.connection, "chat.add", 2, 0, "You're chatting too fast - try again in " + (num + 0.5f).ToString("0") + " seconds");
				if (num > 120f)
				{
					player.Kick("Chatting too fast");
				}
				return;
			}
		}
		string @string = arg.GetString(0, "text");
		System.Threading.Tasks.ValueTask<bool> valueTask = sayAs(targetChannel, player.userID, player.displayName, @string, player);
		Analytics.Azure.OnChatMessage(player, @string, (int)targetChannel);
		player.NextChatTime = Time.realtimeSinceStartup + 1.5f;
		if (valueTask.IsCompletedSuccessfully)
		{
			if (!valueTask.Result)
			{
				player.NextChatTime = Time.realtimeSinceStartup;
			}
			return;
		}
		Task<bool> task = valueTask.AsTask();
		task.GetAwaiter().OnCompleted(delegate
		{
			try
			{
				if (!task.Result)
				{
					player.NextChatTime = Time.realtimeSinceStartup;
				}
			}
			catch (Exception ex)
			{
				Debug.LogError((object)ex);
			}
		});
	}

	internal static string GetNameColor(ulong userId, BasePlayer player = null)
	{
		ServerUsers.UserGroup userGroup = ServerUsers.Get(userId)?.group ?? ServerUsers.UserGroup.None;
		bool flag = userGroup == ServerUsers.UserGroup.Owner || userGroup == ServerUsers.UserGroup.Moderator;
		bool num = (((Object)(object)player != (Object)null) ? player.IsDeveloper : DeveloperList.Contains(userId));
		string result = "#5af";
		if (flag)
		{
			result = "#af5";
		}
		if (num)
		{
			result = "#fa5";
		}
		return result;
	}

	[AsyncStateMachine(typeof(_003CsayAs_003Ed__18))]
	internal static System.Threading.Tasks.ValueTask<bool> sayAs(ChatChannel targetChannel, ulong userId, string username, string message, BasePlayer player = null)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		_003CsayAs_003Ed__18 _003CsayAs_003Ed__ = default(_003CsayAs_003Ed__18);
		_003CsayAs_003Ed__.targetChannel = targetChannel;
		_003CsayAs_003Ed__.userId = userId;
		_003CsayAs_003Ed__.username = username;
		_003CsayAs_003Ed__.message = message;
		_003CsayAs_003Ed__.player = player;
		_003CsayAs_003Ed__._003C_003Et__builder = AsyncValueTaskMethodBuilder<bool>.Create();
		_003CsayAs_003Ed__._003C_003E1__state = -1;
		AsyncValueTaskMethodBuilder<bool> _003C_003Et__builder = _003CsayAs_003Ed__._003C_003Et__builder;
		_003C_003Et__builder.Start<_003CsayAs_003Ed__18>(ref _003CsayAs_003Ed__);
		return _003CsayAs_003Ed__._003C_003Et__builder.Task;
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
