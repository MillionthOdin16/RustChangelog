using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Facepunch.Extend;
using UnityEngine;

namespace ConVar;

[Factory("clan")]
public class Clan : ConsoleSystem
{
	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	private struct _003CGetPlayerClan_003Ed__3 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncValueTaskMethodBuilder<IClan> _003C_003Et__builder;

		public BasePlayer player;

		private ValueTaskAwaiter<ClanValueResult<IClan>> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Invalid comparison between Unknown and I4
			int num = _003C_003E1__state;
			IClan result2;
			try
			{
				ValueTaskAwaiter<ClanValueResult<IClan>> awaiter;
				if (num != 0)
				{
					awaiter = ClanManager.ServerInstance.Backend.GetByMember(player.userID).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ValueTaskAwaiter<ClanValueResult<IClan>>, _003CGetPlayerClan_003Ed__3>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ValueTaskAwaiter<ClanValueResult<IClan>>);
					num = (_003C_003E1__state = -1);
				}
				ClanValueResult<IClan> result = awaiter.GetResult();
				if (!result.IsSuccess)
				{
					string msg = (((int)result.Result == 3) ? "You're not in a clan!" : "Failed to find your clan!");
					player.ConsoleMessage(msg);
					result2 = null;
				}
				else
				{
					result2 = result.Value;
				}
			}
			catch (Exception exception)
			{
				_003C_003E1__state = -2;
				_003C_003Et__builder.SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003C_003Et__builder.SetResult(result2);
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

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	private struct _003CGetClanByID_003Ed__4 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncValueTaskMethodBuilder<IClan> _003C_003Et__builder;

		public long clanId;

		public BasePlayer player;

		private ValueTaskAwaiter<ClanValueResult<IClan>> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Invalid comparison between Unknown and I4
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			IClan result2;
			try
			{
				ValueTaskAwaiter<ClanValueResult<IClan>> awaiter;
				if (num != 0)
				{
					awaiter = ClanManager.ServerInstance.Backend.Get(clanId).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ValueTaskAwaiter<ClanValueResult<IClan>>, _003CGetClanByID_003Ed__4>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ValueTaskAwaiter<ClanValueResult<IClan>>);
					num = (_003C_003E1__state = -1);
				}
				ClanValueResult<IClan> result = awaiter.GetResult();
				if (!result.IsSuccess)
				{
					string text = (((int)result.Result == 4) ? $"Clan with ID {clanId} was not found!" : $"Failed to get the clan with ID {clanId} ({result.Result})!");
					if ((Object)(object)player != (Object)null)
					{
						player.ConsoleMessage(text);
					}
					else
					{
						Debug.Log((object)text);
					}
					result2 = null;
				}
				else
				{
					result2 = result.Value;
				}
			}
			catch (Exception exception)
			{
				_003C_003E1__state = -2;
				_003C_003Et__builder.SetException(exception);
				return;
			}
			_003C_003E1__state = -2;
			_003C_003Et__builder.SetResult(result2);
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

	[ServerVar(Help = "Enables the clan system if set to true (must be set at boot, requires restart)")]
	public static bool enabled = false;

	[ServerVar(Help = "Maximum number of members each clan can have (local backend only!)")]
	public static int maxMemberCount = 100;

	[ServerVar(Help = "Prints info about a clan given its ID")]
	public static void Info(Arg arg)
	{
		if ((Object)(object)ClanManager.ServerInstance == (Object)null)
		{
			arg.ReplyWith("ClanManager is null!");
			return;
		}
		long clanId = arg.GetLong(0, 0L);
		if (clanId == 0L)
		{
			BasePlayer basePlayer = arg.Player();
			if ((Object)(object)basePlayer == (Object)null)
			{
				arg.ReplyWith("Usage: clan.info <clanID>");
			}
			else
			{
				SendClanInfoPlayer(basePlayer);
			}
		}
		else
		{
			SendClanInfoConsole(clanId);
		}
		static string FormatClan(IClan clan)
		{
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Expected O, but got Unknown
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_0123: Unknown result type (might be due to invalid IL or missing references)
			//IL_015b: Unknown result type (might be due to invalid IL or missing references)
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine($"Clan ID: {clan.ClanId}");
			stringBuilder.AppendLine("Name: " + clan.Name);
			stringBuilder.AppendLine("MoTD: " + clan.Motd);
			stringBuilder.AppendLine("Members:");
			TextTable val2 = new TextTable();
			val2.AddColumns(new string[4] { "steamID", "username", "online", "role" });
			foreach (ClanMember member in clan.Members)
			{
				ClanRole? val3 = List.TryFindWith<ClanRole, int>((IReadOnlyCollection<ClanRole>)clan.Roles, (Func<ClanRole, int>)((ClanRole r) => r.RoleId), member.RoleId, (IEqualityComparer<int>)null);
				string text = SingletonComponent<ServerMgr>.Instance.persistance.GetPlayerName(member.SteamId) ?? "[unknown]";
				bool flag = (NexusServer.Started ? NexusServer.IsOnline(member.SteamId) : ServerPlayers.IsOnline(member.SteamId));
				string[] array = new string[4];
				ulong steamId = member.SteamId;
				array[0] = steamId.ToString();
				array[1] = text;
				array[2] = (flag ? "x" : "");
				array[3] = val3?.Name ?? "[null]";
				val2.AddRow(array);
			}
			stringBuilder.Append(val2);
			return stringBuilder.ToString();
		}
		static async void SendClanInfoConsole(long id)
		{
			try
			{
				IClan val = await GetClanByID(id);
				if (val != null)
				{
					Debug.Log((object)FormatClan(val));
				}
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}
		async void SendClanInfoPlayer(BasePlayer player)
		{
			_ = 1;
			try
			{
				IClan val4 = ((clanId != 0L) ? (await GetClanByID(clanId)) : (await GetPlayerClan(player)));
				IClan val5 = val4;
				if (val5 != null)
				{
					string msg = FormatClan(val5);
					player.ConsoleMessage(msg);
				}
			}
			catch (Exception ex2)
			{
				Debug.LogException(ex2);
				player.ConsoleMessage(ex2.ToString());
			}
		}
	}

	[AsyncStateMachine(typeof(_003CGetPlayerClan_003Ed__3))]
	private static ValueTask<IClan> GetPlayerClan(BasePlayer player)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		_003CGetPlayerClan_003Ed__3 _003CGetPlayerClan_003Ed__ = default(_003CGetPlayerClan_003Ed__3);
		_003CGetPlayerClan_003Ed__._003C_003Et__builder = AsyncValueTaskMethodBuilder<IClan>.Create();
		_003CGetPlayerClan_003Ed__.player = player;
		_003CGetPlayerClan_003Ed__._003C_003E1__state = -1;
		_003CGetPlayerClan_003Ed__._003C_003Et__builder.Start<_003CGetPlayerClan_003Ed__3>(ref _003CGetPlayerClan_003Ed__);
		return _003CGetPlayerClan_003Ed__._003C_003Et__builder.Task;
	}

	[AsyncStateMachine(typeof(_003CGetClanByID_003Ed__4))]
	private static ValueTask<IClan> GetClanByID(long clanId, BasePlayer player = null)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		_003CGetClanByID_003Ed__4 _003CGetClanByID_003Ed__ = default(_003CGetClanByID_003Ed__4);
		_003CGetClanByID_003Ed__._003C_003Et__builder = AsyncValueTaskMethodBuilder<IClan>.Create();
		_003CGetClanByID_003Ed__.clanId = clanId;
		_003CGetClanByID_003Ed__.player = player;
		_003CGetClanByID_003Ed__._003C_003E1__state = -1;
		_003CGetClanByID_003Ed__._003C_003Et__builder.Start<_003CGetClanByID_003Ed__4>(ref _003CGetClanByID_003Ed__);
		return _003CGetClanByID_003Ed__._003C_003Et__builder.Task;
	}
}
