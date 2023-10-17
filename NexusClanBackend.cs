using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using ConVar;
using Facepunch;
using Facepunch.Nexus;
using Facepunch.Nexus.Models;
using ProtoBuf.Nexus;
using Rust;
using UnityEngine;

public class NexusClanBackend : IClanBackend, IDisposable
{
	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	private struct _003CGet_003Ed__8 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncValueTaskMethodBuilder<ClanValueResult<IClan>> _003C_003Et__builder;

		public NexusClanBackend _003C_003E4__this;

		public long clanId;

		private ValueTaskAwaiter<NexusClanResult<NexusClan>> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			NexusClanBackend nexusClanBackend = _003C_003E4__this;
			ClanValueResult<IClan> result2;
			try
			{
				ValueTaskAwaiter<NexusClanResult<NexusClan>> awaiter;
				if (num != 0)
				{
					awaiter = nexusClanBackend._client.GetClan(clanId).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ValueTaskAwaiter<NexusClanResult<NexusClan>>, _003CGet_003Ed__8>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ValueTaskAwaiter<NexusClanResult<NexusClan>>);
					num = (_003C_003E1__state = -1);
				}
				NexusClanResult<NexusClan> result = awaiter.GetResult();
				NexusClan clan = default(NexusClan);
				result2 = ((!result.IsSuccess || !result.TryGetResponse(ref clan)) ? ClanValueResult<IClan>.op_Implicit(result.ResultCode.ToClanResult()) : ClanValueResult<IClan>.op_Implicit((IClan)(object)nexusClanBackend.Wrap(clan)));
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
	private struct _003CGetByMember_003Ed__10 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncValueTaskMethodBuilder<ClanValueResult<IClan>> _003C_003Et__builder;

		public NexusClanBackend _003C_003E4__this;

		public ulong steamId;

		private ValueTaskAwaiter<NexusClanResult<NexusClan>> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			NexusClanBackend nexusClanBackend = _003C_003E4__this;
			ClanValueResult<IClan> result2;
			try
			{
				ValueTaskAwaiter<NexusClanResult<NexusClan>> awaiter;
				if (num != 0)
				{
					awaiter = nexusClanBackend._client.GetClanByMember(NexusClanUtil.GetPlayerId(steamId)).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ValueTaskAwaiter<NexusClanResult<NexusClan>>, _003CGetByMember_003Ed__10>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ValueTaskAwaiter<NexusClanResult<NexusClan>>);
					num = (_003C_003E1__state = -1);
				}
				NexusClanResult<NexusClan> result = awaiter.GetResult();
				NexusClan clan = default(NexusClan);
				result2 = ((!result.IsSuccess || !result.TryGetResponse(ref clan)) ? ClanValueResult<IClan>.op_Implicit(result.ResultCode.ToClanResult()) : ClanValueResult<IClan>.op_Implicit((IClan)(object)nexusClanBackend.Wrap(clan)));
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
	private struct _003CCreate_003Ed__11 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncValueTaskMethodBuilder<ClanValueResult<IClan>> _003C_003Et__builder;

		public string name;

		public ulong leaderSteamId;

		public NexusClanBackend _003C_003E4__this;

		private ValueTaskAwaiter<NexusClanResult<NexusClan>> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0114: Unknown result type (might be due to invalid IL or missing references)
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_0105: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			NexusClanBackend nexusClanBackend = _003C_003E4__this;
			ClanValueResult<IClan> result2;
			try
			{
				ValueTaskAwaiter<NexusClanResult<NexusClan>> awaiter;
				if (num != 0)
				{
					ClanCreateParameters val = default(ClanCreateParameters);
					((ClanCreateParameters)(ref val)).ClanName = name;
					((ClanCreateParameters)(ref val)).ClanNameNormalized = name.ToLowerInvariant().Normalize(NormalizationForm.FormKC);
					((ClanCreateParameters)(ref val)).LeaderPlayerId = NexusClanUtil.GetPlayerId(leaderSteamId);
					((ClanCreateParameters)(ref val)).LeaderRoleName = "Leader";
					((ClanCreateParameters)(ref val)).LeaderRoleVariables = NexusClanUtil.DefaultLeaderVariables;
					((ClanCreateParameters)(ref val)).MemberRoleName = "Member";
					ClanCreateParameters val2 = val;
					awaiter = nexusClanBackend._client.CreateClan(val2).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ValueTaskAwaiter<NexusClanResult<NexusClan>>, _003CCreate_003Ed__11>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ValueTaskAwaiter<NexusClanResult<NexusClan>>);
					num = (_003C_003E1__state = -1);
				}
				NexusClanResult<NexusClan> result = awaiter.GetResult();
				NexusClan clan = default(NexusClan);
				result2 = ((!result.IsSuccess || !result.TryGetResponse(ref clan)) ? ClanValueResult<IClan>.op_Implicit(result.ResultCode.ToClanResult()) : ClanValueResult<IClan>.op_Implicit((IClan)(object)nexusClanBackend.Wrap(clan)));
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
	private struct _003CListInvitations_003Ed__12 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncValueTaskMethodBuilder<ClanValueResult<List<ClanInvitation>>> _003C_003Et__builder;

		public NexusClanBackend _003C_003E4__this;

		public ulong steamId;

		private ValueTaskAwaiter<NexusClanResult<List<ClanInvitation>>> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_010a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			NexusClanBackend nexusClanBackend = _003C_003E4__this;
			ClanValueResult<List<ClanInvitation>> result2;
			try
			{
				ValueTaskAwaiter<NexusClanResult<List<ClanInvitation>>> awaiter;
				if (num != 0)
				{
					awaiter = nexusClanBackend._client.ListClanInvitations(NexusClanUtil.GetPlayerId(steamId)).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ValueTaskAwaiter<NexusClanResult<List<ClanInvitation>>>, _003CListInvitations_003Ed__12>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ValueTaskAwaiter<NexusClanResult<List<ClanInvitation>>>);
					num = (_003C_003E1__state = -1);
				}
				NexusClanResult<List<ClanInvitation>> result = awaiter.GetResult();
				List<ClanInvitation> source = default(List<ClanInvitation>);
				if (result.IsSuccess && result.TryGetResponse(ref source))
				{
					List<ClanInvitation> list = source.Select(delegate(ClanInvitation i)
					{
						//IL_0002: Unknown result type (might be due to invalid IL or missing references)
						//IL_0037: Unknown result type (might be due to invalid IL or missing references)
						ClanInvitation result3 = default(ClanInvitation);
						result3.ClanId = ((ClanInvitation)(ref i)).ClanId;
						result3.Recruiter = NexusClanUtil.GetSteamId(((ClanInvitation)(ref i)).RecruiterPlayerId);
						result3.Timestamp = ((ClanInvitation)(ref i)).Timestamp;
						return result3;
					}).ToList();
					result2 = new ClanValueResult<List<ClanInvitation>>(list);
				}
				else
				{
					result2 = ClanValueResult<List<ClanInvitation>>.op_Implicit(result.ResultCode.ToClanResult());
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

	private readonly Dictionary<long, NexusClanWrapper> _clanWrappers;

	private IClanChangeSink _changeSink;

	private NexusClanChatCollector _chatCollector;

	private NexusClanEventHandler _eventHandler;

	private NexusZoneClient _client;

	public NexusClanBackend()
	{
		_clanWrappers = new Dictionary<long, NexusClanWrapper>();
	}

	public ValueTask Initialize(IClanChangeSink changeSink)
	{
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		if (!NexusServer.Started)
		{
			throw new InvalidOperationException("Cannot use the Nexus clan backend when nexus is not enabled on this server!");
		}
		_clanWrappers.Clear();
		_changeSink = changeSink;
		_chatCollector = new NexusClanChatCollector(changeSink);
		_eventHandler = new NexusClanEventHandler(this, changeSink);
		_client = NexusServer.ZoneClient;
		_client.ClanEventListener = (INexusClanEventListener)(object)_eventHandler;
		((MonoBehaviour)Global.Runner).StartCoroutine(BroadcastClanChatBatches());
		return default(ValueTask);
	}

	public void Dispose()
	{
		_clanWrappers.Clear();
		_changeSink = null;
		_chatCollector = null;
		_eventHandler = null;
		NexusZoneClient client = _client;
		if (((client != null) ? client.ClanEventListener : null) != null)
		{
			_client.ClanEventListener = null;
		}
		_client = null;
	}

	[AsyncStateMachine(typeof(_003CGet_003Ed__8))]
	public ValueTask<ClanValueResult<IClan>> Get(long clanId)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		_003CGet_003Ed__8 _003CGet_003Ed__ = default(_003CGet_003Ed__8);
		_003CGet_003Ed__._003C_003Et__builder = AsyncValueTaskMethodBuilder<ClanValueResult<IClan>>.Create();
		_003CGet_003Ed__._003C_003E4__this = this;
		_003CGet_003Ed__.clanId = clanId;
		_003CGet_003Ed__._003C_003E1__state = -1;
		_003CGet_003Ed__._003C_003Et__builder.Start<_003CGet_003Ed__8>(ref _003CGet_003Ed__);
		return _003CGet_003Ed__._003C_003Et__builder.Task;
	}

	public bool TryGet(long clanId, out IClan clan)
	{
		NexusClan clan2 = default(NexusClan);
		if (!_client.TryGetClan(clanId, ref clan2))
		{
			clan = null;
			return false;
		}
		clan = (IClan)(object)Wrap(clan2);
		return true;
	}

	[AsyncStateMachine(typeof(_003CGetByMember_003Ed__10))]
	public ValueTask<ClanValueResult<IClan>> GetByMember(ulong steamId)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		_003CGetByMember_003Ed__10 _003CGetByMember_003Ed__ = default(_003CGetByMember_003Ed__10);
		_003CGetByMember_003Ed__._003C_003Et__builder = AsyncValueTaskMethodBuilder<ClanValueResult<IClan>>.Create();
		_003CGetByMember_003Ed__._003C_003E4__this = this;
		_003CGetByMember_003Ed__.steamId = steamId;
		_003CGetByMember_003Ed__._003C_003E1__state = -1;
		_003CGetByMember_003Ed__._003C_003Et__builder.Start<_003CGetByMember_003Ed__10>(ref _003CGetByMember_003Ed__);
		return _003CGetByMember_003Ed__._003C_003Et__builder.Task;
	}

	[AsyncStateMachine(typeof(_003CCreate_003Ed__11))]
	public ValueTask<ClanValueResult<IClan>> Create(ulong leaderSteamId, string name)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		_003CCreate_003Ed__11 _003CCreate_003Ed__ = default(_003CCreate_003Ed__11);
		_003CCreate_003Ed__._003C_003Et__builder = AsyncValueTaskMethodBuilder<ClanValueResult<IClan>>.Create();
		_003CCreate_003Ed__._003C_003E4__this = this;
		_003CCreate_003Ed__.leaderSteamId = leaderSteamId;
		_003CCreate_003Ed__.name = name;
		_003CCreate_003Ed__._003C_003E1__state = -1;
		_003CCreate_003Ed__._003C_003Et__builder.Start<_003CCreate_003Ed__11>(ref _003CCreate_003Ed__);
		return _003CCreate_003Ed__._003C_003Et__builder.Task;
	}

	[AsyncStateMachine(typeof(_003CListInvitations_003Ed__12))]
	public ValueTask<ClanValueResult<List<ClanInvitation>>> ListInvitations(ulong steamId)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		_003CListInvitations_003Ed__12 _003CListInvitations_003Ed__ = default(_003CListInvitations_003Ed__12);
		_003CListInvitations_003Ed__._003C_003Et__builder = AsyncValueTaskMethodBuilder<ClanValueResult<List<ClanInvitation>>>.Create();
		_003CListInvitations_003Ed__._003C_003E4__this = this;
		_003CListInvitations_003Ed__.steamId = steamId;
		_003CListInvitations_003Ed__._003C_003E1__state = -1;
		_003CListInvitations_003Ed__._003C_003Et__builder.Start<_003CListInvitations_003Ed__12>(ref _003CListInvitations_003Ed__);
		return _003CListInvitations_003Ed__._003C_003Et__builder.Task;
	}

	public void HandleClanChatBatch(ClanChatBatchRequest request)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		if (_changeSink == null)
		{
			return;
		}
		foreach (Message message in request.messages)
		{
			_changeSink.ClanChatMessage(message.clanId, new ClanChatEntry
			{
				SteamId = message.userId,
				Message = message.text,
				Name = message.name,
				Time = message.timestamp
			});
		}
	}

	private IEnumerator BroadcastClanChatBatches()
	{
		while (_chatCollector != null && NexusServer.Started)
		{
			List<Message> list = Pool.GetList<Message>();
			_chatCollector.TakeMessages(list);
			if (list.Count > 0)
			{
				SendClanChatBatch(list);
			}
			else
			{
				Pool.FreeList<Message>(ref list);
			}
			yield return CoroutineEx.waitForSecondsRealtime(Nexus.clanClatBatchDuration);
		}
		static async void SendClanChatBatch(List<Message> messages)
		{
			Request val = Pool.Get<Request>();
			val.isFireAndForget = true;
			val.clanChatBatch = Pool.Get<ClanChatBatchRequest>();
			val.clanChatBatch.messages = messages;
			try
			{
				(await NexusServer.BroadcastRpc(val))?.Dispose();
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}
	}

	public void UpdateWrapper(long clanId)
	{
		NexusClanWrapper value;
		lock (_clanWrappers)
		{
			if (!_clanWrappers.TryGetValue(clanId, out value))
			{
				return;
			}
		}
		value.UpdateValuesInternal();
	}

	public void RemoveWrapper(long clanId)
	{
		lock (_clanWrappers)
		{
			_clanWrappers.Remove(clanId);
		}
	}

	private NexusClanWrapper Wrap(NexusClan clan)
	{
		lock (_clanWrappers)
		{
			if (_clanWrappers.TryGetValue(clan.ClanId, out var value) && value.Internal == clan)
			{
				return value;
			}
			value = new NexusClanWrapper(clan, _chatCollector);
			_clanWrappers[clan.ClanId] = value;
			return value;
		}
	}
}
