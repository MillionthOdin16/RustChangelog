using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Facepunch.Extend;
using Facepunch.Nexus;
using Facepunch.Nexus.Models;
using UnityEngine;

public class NexusClanWrapper : IClan
{
	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	private struct _003CGetLogs_003Ed__47 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncValueTaskMethodBuilder<ClanValueResult<ClanLogs>> _003C_003Et__builder;

		public NexusClanWrapper _003C_003E4__this;

		public ulong bySteamId;

		public int limit;

		private TaskAwaiter<NexusClanResult<List<ClanLogEntry>>> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			NexusClanWrapper nexusClanWrapper = _003C_003E4__this;
			ClanValueResult<ClanLogs> result2;
			try
			{
				TaskAwaiter<NexusClanResult<List<ClanLogEntry>>> awaiter;
				if (num != 0)
				{
					awaiter = nexusClanWrapper.Internal.GetLogs(NexusClanUtil.GetPlayerId(bySteamId), limit).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<TaskAwaiter<NexusClanResult<List<ClanLogEntry>>>, _003CGetLogs_003Ed__47>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(TaskAwaiter<NexusClanResult<List<ClanLogEntry>>>);
					num = (_003C_003E1__state = -1);
				}
				NexusClanResult<List<ClanLogEntry>> result = awaiter.GetResult();
				List<ClanLogEntry> source = default(List<ClanLogEntry>);
				if (result.IsSuccess && result.TryGetResponse(ref source))
				{
					ClanLogs val = default(ClanLogs);
					val.ClanId = nexusClanWrapper.ClanId;
					val.Entries = source.Select(delegate(ClanLogEntry e)
					{
						//IL_0002: Unknown result type (might be due to invalid IL or missing references)
						//IL_0063: Unknown result type (might be due to invalid IL or missing references)
						ClanLogEntry result3 = default(ClanLogEntry);
						result3.Timestamp = ((ClanLogEntry)(ref e)).Timestamp * 1000;
						result3.EventKey = ((ClanLogEntry)(ref e)).EventKey;
						result3.Arg1 = ((ClanLogEntry)(ref e)).Arg1;
						result3.Arg2 = ((ClanLogEntry)(ref e)).Arg2;
						result3.Arg3 = ((ClanLogEntry)(ref e)).Arg3;
						result3.Arg4 = ((ClanLogEntry)(ref e)).Arg4;
						return result3;
					}).ToList();
					result2 = ClanValueResult<ClanLogs>.op_Implicit(val);
				}
				else
				{
					result2 = ClanValueResult<ClanLogs>.op_Implicit(result.ResultCode.ToClanResult());
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
	private struct _003CUpdateLastSeen_003Ed__48 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncValueTaskMethodBuilder<ClanResult> _003C_003Et__builder;

		public NexusClanWrapper _003C_003E4__this;

		public ulong steamId;

		private TaskAwaiter<NexusClanResultCode> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			NexusClanWrapper nexusClanWrapper = _003C_003E4__this;
			ClanResult result;
			try
			{
				TaskAwaiter<NexusClanResultCode> awaiter;
				if (num != 0)
				{
					awaiter = nexusClanWrapper.Internal.UpdateLastSeen(NexusClanUtil.GetPlayerId(steamId)).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<TaskAwaiter<NexusClanResultCode>, _003CUpdateLastSeen_003Ed__48>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(TaskAwaiter<NexusClanResultCode>);
					num = (_003C_003E1__state = -1);
				}
				result = awaiter.GetResult().ToClanResult();
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

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	private struct _003CSetMotd_003Ed__49 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncValueTaskMethodBuilder<ClanResult> _003C_003Et__builder;

		public NexusClanWrapper _003C_003E4__this;

		public ulong bySteamId;

		public string newMotd;

		private TaskAwaiter<NexusClanResultCode> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Unknown result type (might be due to invalid IL or missing references)
			//IL_016a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			NexusClanWrapper nexusClanWrapper = _003C_003E4__this;
			ClanResult result;
			try
			{
				TaskAwaiter<NexusClanResultCode> awaiter;
				if (num == 0)
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(TaskAwaiter<NexusClanResultCode>);
					num = (_003C_003E1__state = -1);
					goto IL_0134;
				}
				if (nexusClanWrapper.CheckRole(bySteamId, (ClanRole r) => r.CanSetMotd))
				{
					string playerId = NexusClanUtil.GetPlayerId(bySteamId);
					NexusClan @internal = nexusClanWrapper.Internal;
					ClanVariablesUpdate val = default(ClanVariablesUpdate);
					((ClanVariablesUpdate)(ref val)).Variables = new List<VariableUpdate>(2)
					{
						new VariableUpdate("motd", newMotd, (bool?)null, (bool?)null),
						new VariableUpdate("motd_author", playerId, (bool?)null, (bool?)null)
					};
					((ClanVariablesUpdate)(ref val)).EventKey = "set_motd";
					((ClanVariablesUpdate)(ref val)).Arg1 = playerId;
					((ClanVariablesUpdate)(ref val)).Arg2 = newMotd;
					awaiter = @internal.UpdateVariables(val).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<TaskAwaiter<NexusClanResultCode>, _003CSetMotd_003Ed__49>(ref awaiter, ref this);
						return;
					}
					goto IL_0134;
				}
				result = (ClanResult)5;
				goto end_IL_000e;
				IL_0134:
				result = awaiter.GetResult().ToClanResult();
				end_IL_000e:;
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

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	private struct _003CSetLogo_003Ed__50 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncValueTaskMethodBuilder<ClanResult> _003C_003Et__builder;

		public NexusClanWrapper _003C_003E4__this;

		public ulong bySteamId;

		public byte[] newLogo;

		private TaskAwaiter<NexusClanResultCode> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0109: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0113: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			NexusClanWrapper nexusClanWrapper = _003C_003E4__this;
			ClanResult result;
			try
			{
				TaskAwaiter<NexusClanResultCode> awaiter;
				if (num == 0)
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(TaskAwaiter<NexusClanResultCode>);
					num = (_003C_003E1__state = -1);
					goto IL_0107;
				}
				if (nexusClanWrapper.CheckRole(bySteamId, (ClanRole r) => r.CanSetLogo))
				{
					string playerId = NexusClanUtil.GetPlayerId(bySteamId);
					NexusClan @internal = nexusClanWrapper.Internal;
					ClanVariablesUpdate val = default(ClanVariablesUpdate);
					((ClanVariablesUpdate)(ref val)).Variables = new List<VariableUpdate>(1)
					{
						new VariableUpdate("logo", Memory<byte>.op_Implicit(newLogo), (bool?)null, (bool?)null)
					};
					((ClanVariablesUpdate)(ref val)).EventKey = "set_logo";
					((ClanVariablesUpdate)(ref val)).Arg1 = playerId;
					awaiter = @internal.UpdateVariables(val).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<TaskAwaiter<NexusClanResultCode>, _003CSetLogo_003Ed__50>(ref awaiter, ref this);
						return;
					}
					goto IL_0107;
				}
				result = (ClanResult)5;
				goto end_IL_000e;
				IL_0107:
				result = awaiter.GetResult().ToClanResult();
				end_IL_000e:;
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

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	private struct _003CSetColor_003Ed__51 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncValueTaskMethodBuilder<ClanResult> _003C_003Et__builder;

		public NexusClanWrapper _003C_003E4__this;

		public ulong bySteamId;

		public Color32 newColor;

		private TaskAwaiter<NexusClanResultCode> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0133: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			NexusClanWrapper nexusClanWrapper = _003C_003E4__this;
			ClanResult result;
			try
			{
				TaskAwaiter<NexusClanResultCode> awaiter;
				if (num == 0)
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(TaskAwaiter<NexusClanResultCode>);
					num = (_003C_003E1__state = -1);
					goto IL_0127;
				}
				if (nexusClanWrapper.CheckRole(bySteamId, (ClanRole r) => r.CanSetLogo))
				{
					string playerId = NexusClanUtil.GetPlayerId(bySteamId);
					NexusClan @internal = nexusClanWrapper.Internal;
					ClanVariablesUpdate val = default(ClanVariablesUpdate);
					((ClanVariablesUpdate)(ref val)).Variables = new List<VariableUpdate>(1)
					{
						new VariableUpdate("color", ColorEx.ToInt32(newColor).ToString("G"), (bool?)null, (bool?)null)
					};
					((ClanVariablesUpdate)(ref val)).EventKey = "set_color";
					((ClanVariablesUpdate)(ref val)).Arg1 = playerId;
					((ClanVariablesUpdate)(ref val)).Arg2 = ColorEx.ToHex(newColor);
					awaiter = @internal.UpdateVariables(val).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<TaskAwaiter<NexusClanResultCode>, _003CSetColor_003Ed__51>(ref awaiter, ref this);
						return;
					}
					goto IL_0127;
				}
				result = (ClanResult)5;
				goto end_IL_000e;
				IL_0127:
				result = awaiter.GetResult().ToClanResult();
				end_IL_000e:;
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

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	private struct _003CInvite_003Ed__52 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncValueTaskMethodBuilder<ClanResult> _003C_003Et__builder;

		public NexusClanWrapper _003C_003E4__this;

		public ulong steamId;

		public ulong bySteamId;

		private TaskAwaiter<NexusClanResultCode> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			NexusClanWrapper nexusClanWrapper = _003C_003E4__this;
			ClanResult result;
			try
			{
				TaskAwaiter<NexusClanResultCode> awaiter;
				if (num != 0)
				{
					awaiter = nexusClanWrapper.Internal.Invite(NexusClanUtil.GetPlayerId(steamId), NexusClanUtil.GetPlayerId(bySteamId)).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<TaskAwaiter<NexusClanResultCode>, _003CInvite_003Ed__52>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(TaskAwaiter<NexusClanResultCode>);
					num = (_003C_003E1__state = -1);
				}
				result = awaiter.GetResult().ToClanResult();
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

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	private struct _003CCancelInvite_003Ed__53 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncValueTaskMethodBuilder<ClanResult> _003C_003Et__builder;

		public NexusClanWrapper _003C_003E4__this;

		public ulong steamId;

		public ulong bySteamId;

		private TaskAwaiter<NexusClanResultCode> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			NexusClanWrapper nexusClanWrapper = _003C_003E4__this;
			ClanResult result;
			try
			{
				TaskAwaiter<NexusClanResultCode> awaiter;
				if (num != 0)
				{
					awaiter = nexusClanWrapper.Internal.CancelInvite(NexusClanUtil.GetPlayerId(steamId), NexusClanUtil.GetPlayerId(bySteamId)).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<TaskAwaiter<NexusClanResultCode>, _003CCancelInvite_003Ed__53>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(TaskAwaiter<NexusClanResultCode>);
					num = (_003C_003E1__state = -1);
				}
				result = awaiter.GetResult().ToClanResult();
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

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	private struct _003CAcceptInvite_003Ed__54 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncValueTaskMethodBuilder<ClanResult> _003C_003Et__builder;

		public NexusClanWrapper _003C_003E4__this;

		public ulong steamId;

		private TaskAwaiter<NexusClanResultCode> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			NexusClanWrapper nexusClanWrapper = _003C_003E4__this;
			ClanResult result;
			try
			{
				TaskAwaiter<NexusClanResultCode> awaiter;
				if (num != 0)
				{
					awaiter = nexusClanWrapper.Internal.AcceptInvite(NexusClanUtil.GetPlayerId(steamId)).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<TaskAwaiter<NexusClanResultCode>, _003CAcceptInvite_003Ed__54>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(TaskAwaiter<NexusClanResultCode>);
					num = (_003C_003E1__state = -1);
				}
				result = awaiter.GetResult().ToClanResult();
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

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	private struct _003CKick_003Ed__55 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncValueTaskMethodBuilder<ClanResult> _003C_003Et__builder;

		public NexusClanWrapper _003C_003E4__this;

		public ulong steamId;

		public ulong bySteamId;

		private TaskAwaiter<NexusClanResultCode> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			NexusClanWrapper nexusClanWrapper = _003C_003E4__this;
			ClanResult result;
			try
			{
				TaskAwaiter<NexusClanResultCode> awaiter;
				if (num != 0)
				{
					awaiter = nexusClanWrapper.Internal.Kick(NexusClanUtil.GetPlayerId(steamId), NexusClanUtil.GetPlayerId(bySteamId)).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<TaskAwaiter<NexusClanResultCode>, _003CKick_003Ed__55>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(TaskAwaiter<NexusClanResultCode>);
					num = (_003C_003E1__state = -1);
				}
				result = awaiter.GetResult().ToClanResult();
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

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	private struct _003CSetPlayerRole_003Ed__56 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncValueTaskMethodBuilder<ClanResult> _003C_003Et__builder;

		public NexusClanWrapper _003C_003E4__this;

		public ulong steamId;

		public int newRoleId;

		public ulong bySteamId;

		private TaskAwaiter<NexusClanResultCode> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			NexusClanWrapper nexusClanWrapper = _003C_003E4__this;
			ClanResult result;
			try
			{
				TaskAwaiter<NexusClanResultCode> awaiter;
				if (num != 0)
				{
					awaiter = nexusClanWrapper.Internal.SetPlayerRole(NexusClanUtil.GetPlayerId(steamId), newRoleId, NexusClanUtil.GetPlayerId(bySteamId)).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<TaskAwaiter<NexusClanResultCode>, _003CSetPlayerRole_003Ed__56>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(TaskAwaiter<NexusClanResultCode>);
					num = (_003C_003E1__state = -1);
				}
				result = awaiter.GetResult().ToClanResult();
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

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	private struct _003CSetPlayerNotes_003Ed__57 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncValueTaskMethodBuilder<ClanResult> _003C_003Et__builder;

		public NexusClanWrapper _003C_003E4__this;

		public ulong bySteamId;

		public ulong steamId;

		public string notes;

		private TaskAwaiter<NexusClanResultCode> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0128: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			NexusClanWrapper nexusClanWrapper = _003C_003E4__this;
			ClanResult result;
			try
			{
				TaskAwaiter<NexusClanResultCode> awaiter;
				if (num == 0)
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(TaskAwaiter<NexusClanResultCode>);
					num = (_003C_003E1__state = -1);
					goto IL_0126;
				}
				if (nexusClanWrapper.CheckRole(bySteamId, (ClanRole r) => r.CanSetPlayerNotes))
				{
					string playerId = NexusClanUtil.GetPlayerId(steamId);
					string playerId2 = NexusClanUtil.GetPlayerId(bySteamId);
					NexusClan @internal = nexusClanWrapper.Internal;
					ClanVariablesUpdate val = default(ClanVariablesUpdate);
					((ClanVariablesUpdate)(ref val)).Variables = new List<VariableUpdate>(1)
					{
						new VariableUpdate("notes", notes, (bool?)null, (bool?)null)
					};
					((ClanVariablesUpdate)(ref val)).EventKey = "set_notes";
					((ClanVariablesUpdate)(ref val)).Arg1 = playerId2;
					((ClanVariablesUpdate)(ref val)).Arg2 = playerId;
					((ClanVariablesUpdate)(ref val)).Arg3 = notes;
					awaiter = @internal.UpdatePlayerVariables(playerId, val).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<TaskAwaiter<NexusClanResultCode>, _003CSetPlayerNotes_003Ed__57>(ref awaiter, ref this);
						return;
					}
					goto IL_0126;
				}
				result = (ClanResult)5;
				goto end_IL_000e;
				IL_0126:
				result = awaiter.GetResult().ToClanResult();
				end_IL_000e:;
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

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	private struct _003CCreateRole_003Ed__58 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncValueTaskMethodBuilder<ClanResult> _003C_003Et__builder;

		public NexusClanWrapper _003C_003E4__this;

		public ClanRole role;

		public ulong bySteamId;

		private TaskAwaiter<NexusClanResultCode> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			NexusClanWrapper nexusClanWrapper = _003C_003E4__this;
			ClanResult result;
			try
			{
				TaskAwaiter<NexusClanResultCode> awaiter;
				if (num != 0)
				{
					awaiter = nexusClanWrapper.Internal.CreateRole(role.ToRoleParameters(), NexusClanUtil.GetPlayerId(bySteamId)).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<TaskAwaiter<NexusClanResultCode>, _003CCreateRole_003Ed__58>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(TaskAwaiter<NexusClanResultCode>);
					num = (_003C_003E1__state = -1);
				}
				result = awaiter.GetResult().ToClanResult();
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

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	private struct _003CUpdateRole_003Ed__59 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncValueTaskMethodBuilder<ClanResult> _003C_003Et__builder;

		public NexusClanWrapper _003C_003E4__this;

		public ClanRole role;

		public ulong bySteamId;

		private TaskAwaiter<NexusClanResultCode> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			NexusClanWrapper nexusClanWrapper = _003C_003E4__this;
			ClanResult result;
			try
			{
				TaskAwaiter<NexusClanResultCode> awaiter;
				if (num != 0)
				{
					awaiter = nexusClanWrapper.Internal.UpdateRole(role.RoleId, role.ToRoleParameters(), NexusClanUtil.GetPlayerId(bySteamId)).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<TaskAwaiter<NexusClanResultCode>, _003CUpdateRole_003Ed__59>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(TaskAwaiter<NexusClanResultCode>);
					num = (_003C_003E1__state = -1);
				}
				result = awaiter.GetResult().ToClanResult();
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

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	private struct _003CSwapRoleRanks_003Ed__60 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncValueTaskMethodBuilder<ClanResult> _003C_003Et__builder;

		public NexusClanWrapper _003C_003E4__this;

		public int roleIdA;

		public int roleIdB;

		public ulong bySteamId;

		private TaskAwaiter<NexusClanResultCode> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			NexusClanWrapper nexusClanWrapper = _003C_003E4__this;
			ClanResult result;
			try
			{
				TaskAwaiter<NexusClanResultCode> awaiter;
				if (num != 0)
				{
					awaiter = nexusClanWrapper.Internal.SwapRoleRanks(roleIdA, roleIdB, NexusClanUtil.GetPlayerId(bySteamId)).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<TaskAwaiter<NexusClanResultCode>, _003CSwapRoleRanks_003Ed__60>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(TaskAwaiter<NexusClanResultCode>);
					num = (_003C_003E1__state = -1);
				}
				result = awaiter.GetResult().ToClanResult();
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

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	private struct _003CDeleteRole_003Ed__61 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncValueTaskMethodBuilder<ClanResult> _003C_003Et__builder;

		public NexusClanWrapper _003C_003E4__this;

		public int roleId;

		public ulong bySteamId;

		private TaskAwaiter<NexusClanResultCode> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			NexusClanWrapper nexusClanWrapper = _003C_003E4__this;
			ClanResult result;
			try
			{
				TaskAwaiter<NexusClanResultCode> awaiter;
				if (num != 0)
				{
					awaiter = nexusClanWrapper.Internal.DeleteRole(roleId, NexusClanUtil.GetPlayerId(bySteamId)).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<TaskAwaiter<NexusClanResultCode>, _003CDeleteRole_003Ed__61>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(TaskAwaiter<NexusClanResultCode>);
					num = (_003C_003E1__state = -1);
				}
				result = awaiter.GetResult().ToClanResult();
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

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	private struct _003CDisband_003Ed__62 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncValueTaskMethodBuilder<ClanResult> _003C_003Et__builder;

		public NexusClanWrapper _003C_003E4__this;

		public ulong bySteamId;

		private TaskAwaiter<NexusClanResultCode> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			NexusClanWrapper nexusClanWrapper = _003C_003E4__this;
			ClanResult result;
			try
			{
				TaskAwaiter<NexusClanResultCode> awaiter;
				if (num != 0)
				{
					awaiter = nexusClanWrapper.Internal.Disband(NexusClanUtil.GetPlayerId(bySteamId)).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 0);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<TaskAwaiter<NexusClanResultCode>, _003CDisband_003Ed__62>(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(TaskAwaiter<NexusClanResultCode>);
					num = (_003C_003E1__state = -1);
				}
				result = awaiter.GetResult().ToClanResult();
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

	private const int MaxChatScrollback = 20;

	public readonly NexusClan Internal;

	private readonly NexusClanChatCollector _chatCollector;

	private readonly List<ClanRole> _roles;

	private readonly List<ClanMember> _members;

	private readonly List<ClanInvite> _invites;

	private readonly List<ClanChatEntry> _chatHistory;

	public long ClanId => Internal.ClanId;

	public string Name => Internal.Name;

	public long Created => Internal.Created;

	public ulong Creator => NexusClanUtil.GetSteamId(Internal.Creator);

	public string Motd { get; private set; }

	public long MotdTimestamp { get; private set; }

	public ulong MotdAuthor { get; private set; }

	public byte[] Logo { get; private set; }

	public Color32 Color { get; private set; }

	public IReadOnlyList<ClanRole> Roles => _roles;

	public IReadOnlyList<ClanMember> Members => _members;

	public int MaxMemberCount { get; private set; }

	public IReadOnlyList<ClanInvite> Invites => _invites;

	public NexusClanWrapper(NexusClan clan, NexusClanChatCollector chatCollector)
	{
		Internal = clan ?? throw new ArgumentNullException("clan");
		_chatCollector = chatCollector ?? throw new ArgumentNullException("chatCollector");
		_roles = new List<ClanRole>();
		_members = new List<ClanMember>();
		_invites = new List<ClanInvite>();
		_chatHistory = new List<ClanChatEntry>(20);
		UpdateValuesInternal();
	}

	public void UpdateValuesInternal()
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		Internal.GetMotd(out var motd, out var motdTimestamp, out var motdAuthor);
		Motd = motd;
		MotdTimestamp = motdTimestamp;
		MotdAuthor = motdAuthor;
		Internal.GetBanner(out var logo, out var color);
		Logo = logo;
		Color = color;
		List.Resize<ClanRole>(_roles, Internal.Roles.Count);
		for (int i = 0; i < _roles.Count; i++)
		{
			_roles[i] = Internal.Roles[i].ToClanRole();
		}
		List.Resize<ClanMember>(_members, Internal.Members.Count);
		for (int j = 0; j < _members.Count; j++)
		{
			_members[j] = Internal.Members[j].ToClanMember();
		}
		MaxMemberCount = Internal.MaxMemberCount;
		List.Resize<ClanInvite>(_invites, Internal.Invites.Count);
		for (int k = 0; k < _invites.Count; k++)
		{
			_invites[k] = Internal.Invites[k].ToClanInvite();
		}
	}

	[AsyncStateMachine(typeof(_003CGetLogs_003Ed__47))]
	public ValueTask<ClanValueResult<ClanLogs>> GetLogs(int limit, ulong bySteamId)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		_003CGetLogs_003Ed__47 _003CGetLogs_003Ed__ = default(_003CGetLogs_003Ed__47);
		_003CGetLogs_003Ed__._003C_003Et__builder = AsyncValueTaskMethodBuilder<ClanValueResult<ClanLogs>>.Create();
		_003CGetLogs_003Ed__._003C_003E4__this = this;
		_003CGetLogs_003Ed__.limit = limit;
		_003CGetLogs_003Ed__.bySteamId = bySteamId;
		_003CGetLogs_003Ed__._003C_003E1__state = -1;
		_003CGetLogs_003Ed__._003C_003Et__builder.Start<_003CGetLogs_003Ed__47>(ref _003CGetLogs_003Ed__);
		return _003CGetLogs_003Ed__._003C_003Et__builder.Task;
	}

	[AsyncStateMachine(typeof(_003CUpdateLastSeen_003Ed__48))]
	public ValueTask<ClanResult> UpdateLastSeen(ulong steamId)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		_003CUpdateLastSeen_003Ed__48 _003CUpdateLastSeen_003Ed__ = default(_003CUpdateLastSeen_003Ed__48);
		_003CUpdateLastSeen_003Ed__._003C_003Et__builder = AsyncValueTaskMethodBuilder<ClanResult>.Create();
		_003CUpdateLastSeen_003Ed__._003C_003E4__this = this;
		_003CUpdateLastSeen_003Ed__.steamId = steamId;
		_003CUpdateLastSeen_003Ed__._003C_003E1__state = -1;
		_003CUpdateLastSeen_003Ed__._003C_003Et__builder.Start<_003CUpdateLastSeen_003Ed__48>(ref _003CUpdateLastSeen_003Ed__);
		return _003CUpdateLastSeen_003Ed__._003C_003Et__builder.Task;
	}

	[AsyncStateMachine(typeof(_003CSetMotd_003Ed__49))]
	public ValueTask<ClanResult> SetMotd(string newMotd, ulong bySteamId)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		_003CSetMotd_003Ed__49 _003CSetMotd_003Ed__ = default(_003CSetMotd_003Ed__49);
		_003CSetMotd_003Ed__._003C_003Et__builder = AsyncValueTaskMethodBuilder<ClanResult>.Create();
		_003CSetMotd_003Ed__._003C_003E4__this = this;
		_003CSetMotd_003Ed__.newMotd = newMotd;
		_003CSetMotd_003Ed__.bySteamId = bySteamId;
		_003CSetMotd_003Ed__._003C_003E1__state = -1;
		_003CSetMotd_003Ed__._003C_003Et__builder.Start<_003CSetMotd_003Ed__49>(ref _003CSetMotd_003Ed__);
		return _003CSetMotd_003Ed__._003C_003Et__builder.Task;
	}

	[AsyncStateMachine(typeof(_003CSetLogo_003Ed__50))]
	public ValueTask<ClanResult> SetLogo(byte[] newLogo, ulong bySteamId)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		_003CSetLogo_003Ed__50 _003CSetLogo_003Ed__ = default(_003CSetLogo_003Ed__50);
		_003CSetLogo_003Ed__._003C_003Et__builder = AsyncValueTaskMethodBuilder<ClanResult>.Create();
		_003CSetLogo_003Ed__._003C_003E4__this = this;
		_003CSetLogo_003Ed__.newLogo = newLogo;
		_003CSetLogo_003Ed__.bySteamId = bySteamId;
		_003CSetLogo_003Ed__._003C_003E1__state = -1;
		_003CSetLogo_003Ed__._003C_003Et__builder.Start<_003CSetLogo_003Ed__50>(ref _003CSetLogo_003Ed__);
		return _003CSetLogo_003Ed__._003C_003Et__builder.Task;
	}

	[AsyncStateMachine(typeof(_003CSetColor_003Ed__51))]
	public ValueTask<ClanResult> SetColor(Color32 newColor, ulong bySteamId)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		_003CSetColor_003Ed__51 _003CSetColor_003Ed__ = default(_003CSetColor_003Ed__51);
		_003CSetColor_003Ed__._003C_003Et__builder = AsyncValueTaskMethodBuilder<ClanResult>.Create();
		_003CSetColor_003Ed__._003C_003E4__this = this;
		_003CSetColor_003Ed__.newColor = newColor;
		_003CSetColor_003Ed__.bySteamId = bySteamId;
		_003CSetColor_003Ed__._003C_003E1__state = -1;
		_003CSetColor_003Ed__._003C_003Et__builder.Start<_003CSetColor_003Ed__51>(ref _003CSetColor_003Ed__);
		return _003CSetColor_003Ed__._003C_003Et__builder.Task;
	}

	[AsyncStateMachine(typeof(_003CInvite_003Ed__52))]
	public ValueTask<ClanResult> Invite(ulong steamId, ulong bySteamId)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		_003CInvite_003Ed__52 _003CInvite_003Ed__ = default(_003CInvite_003Ed__52);
		_003CInvite_003Ed__._003C_003Et__builder = AsyncValueTaskMethodBuilder<ClanResult>.Create();
		_003CInvite_003Ed__._003C_003E4__this = this;
		_003CInvite_003Ed__.steamId = steamId;
		_003CInvite_003Ed__.bySteamId = bySteamId;
		_003CInvite_003Ed__._003C_003E1__state = -1;
		_003CInvite_003Ed__._003C_003Et__builder.Start<_003CInvite_003Ed__52>(ref _003CInvite_003Ed__);
		return _003CInvite_003Ed__._003C_003Et__builder.Task;
	}

	[AsyncStateMachine(typeof(_003CCancelInvite_003Ed__53))]
	public ValueTask<ClanResult> CancelInvite(ulong steamId, ulong bySteamId)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		_003CCancelInvite_003Ed__53 _003CCancelInvite_003Ed__ = default(_003CCancelInvite_003Ed__53);
		_003CCancelInvite_003Ed__._003C_003Et__builder = AsyncValueTaskMethodBuilder<ClanResult>.Create();
		_003CCancelInvite_003Ed__._003C_003E4__this = this;
		_003CCancelInvite_003Ed__.steamId = steamId;
		_003CCancelInvite_003Ed__.bySteamId = bySteamId;
		_003CCancelInvite_003Ed__._003C_003E1__state = -1;
		_003CCancelInvite_003Ed__._003C_003Et__builder.Start<_003CCancelInvite_003Ed__53>(ref _003CCancelInvite_003Ed__);
		return _003CCancelInvite_003Ed__._003C_003Et__builder.Task;
	}

	[AsyncStateMachine(typeof(_003CAcceptInvite_003Ed__54))]
	public ValueTask<ClanResult> AcceptInvite(ulong steamId)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		_003CAcceptInvite_003Ed__54 _003CAcceptInvite_003Ed__ = default(_003CAcceptInvite_003Ed__54);
		_003CAcceptInvite_003Ed__._003C_003Et__builder = AsyncValueTaskMethodBuilder<ClanResult>.Create();
		_003CAcceptInvite_003Ed__._003C_003E4__this = this;
		_003CAcceptInvite_003Ed__.steamId = steamId;
		_003CAcceptInvite_003Ed__._003C_003E1__state = -1;
		_003CAcceptInvite_003Ed__._003C_003Et__builder.Start<_003CAcceptInvite_003Ed__54>(ref _003CAcceptInvite_003Ed__);
		return _003CAcceptInvite_003Ed__._003C_003Et__builder.Task;
	}

	[AsyncStateMachine(typeof(_003CKick_003Ed__55))]
	public ValueTask<ClanResult> Kick(ulong steamId, ulong bySteamId)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		_003CKick_003Ed__55 _003CKick_003Ed__ = default(_003CKick_003Ed__55);
		_003CKick_003Ed__._003C_003Et__builder = AsyncValueTaskMethodBuilder<ClanResult>.Create();
		_003CKick_003Ed__._003C_003E4__this = this;
		_003CKick_003Ed__.steamId = steamId;
		_003CKick_003Ed__.bySteamId = bySteamId;
		_003CKick_003Ed__._003C_003E1__state = -1;
		_003CKick_003Ed__._003C_003Et__builder.Start<_003CKick_003Ed__55>(ref _003CKick_003Ed__);
		return _003CKick_003Ed__._003C_003Et__builder.Task;
	}

	[AsyncStateMachine(typeof(_003CSetPlayerRole_003Ed__56))]
	public ValueTask<ClanResult> SetPlayerRole(ulong steamId, int newRoleId, ulong bySteamId)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		_003CSetPlayerRole_003Ed__56 _003CSetPlayerRole_003Ed__ = default(_003CSetPlayerRole_003Ed__56);
		_003CSetPlayerRole_003Ed__._003C_003Et__builder = AsyncValueTaskMethodBuilder<ClanResult>.Create();
		_003CSetPlayerRole_003Ed__._003C_003E4__this = this;
		_003CSetPlayerRole_003Ed__.steamId = steamId;
		_003CSetPlayerRole_003Ed__.newRoleId = newRoleId;
		_003CSetPlayerRole_003Ed__.bySteamId = bySteamId;
		_003CSetPlayerRole_003Ed__._003C_003E1__state = -1;
		_003CSetPlayerRole_003Ed__._003C_003Et__builder.Start<_003CSetPlayerRole_003Ed__56>(ref _003CSetPlayerRole_003Ed__);
		return _003CSetPlayerRole_003Ed__._003C_003Et__builder.Task;
	}

	[AsyncStateMachine(typeof(_003CSetPlayerNotes_003Ed__57))]
	public ValueTask<ClanResult> SetPlayerNotes(ulong steamId, string notes, ulong bySteamId)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		_003CSetPlayerNotes_003Ed__57 _003CSetPlayerNotes_003Ed__ = default(_003CSetPlayerNotes_003Ed__57);
		_003CSetPlayerNotes_003Ed__._003C_003Et__builder = AsyncValueTaskMethodBuilder<ClanResult>.Create();
		_003CSetPlayerNotes_003Ed__._003C_003E4__this = this;
		_003CSetPlayerNotes_003Ed__.steamId = steamId;
		_003CSetPlayerNotes_003Ed__.notes = notes;
		_003CSetPlayerNotes_003Ed__.bySteamId = bySteamId;
		_003CSetPlayerNotes_003Ed__._003C_003E1__state = -1;
		_003CSetPlayerNotes_003Ed__._003C_003Et__builder.Start<_003CSetPlayerNotes_003Ed__57>(ref _003CSetPlayerNotes_003Ed__);
		return _003CSetPlayerNotes_003Ed__._003C_003Et__builder.Task;
	}

	[AsyncStateMachine(typeof(_003CCreateRole_003Ed__58))]
	public ValueTask<ClanResult> CreateRole(ClanRole role, ulong bySteamId)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		_003CCreateRole_003Ed__58 _003CCreateRole_003Ed__ = default(_003CCreateRole_003Ed__58);
		_003CCreateRole_003Ed__._003C_003Et__builder = AsyncValueTaskMethodBuilder<ClanResult>.Create();
		_003CCreateRole_003Ed__._003C_003E4__this = this;
		_003CCreateRole_003Ed__.role = role;
		_003CCreateRole_003Ed__.bySteamId = bySteamId;
		_003CCreateRole_003Ed__._003C_003E1__state = -1;
		_003CCreateRole_003Ed__._003C_003Et__builder.Start<_003CCreateRole_003Ed__58>(ref _003CCreateRole_003Ed__);
		return _003CCreateRole_003Ed__._003C_003Et__builder.Task;
	}

	[AsyncStateMachine(typeof(_003CUpdateRole_003Ed__59))]
	public ValueTask<ClanResult> UpdateRole(ClanRole role, ulong bySteamId)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		_003CUpdateRole_003Ed__59 _003CUpdateRole_003Ed__ = default(_003CUpdateRole_003Ed__59);
		_003CUpdateRole_003Ed__._003C_003Et__builder = AsyncValueTaskMethodBuilder<ClanResult>.Create();
		_003CUpdateRole_003Ed__._003C_003E4__this = this;
		_003CUpdateRole_003Ed__.role = role;
		_003CUpdateRole_003Ed__.bySteamId = bySteamId;
		_003CUpdateRole_003Ed__._003C_003E1__state = -1;
		_003CUpdateRole_003Ed__._003C_003Et__builder.Start<_003CUpdateRole_003Ed__59>(ref _003CUpdateRole_003Ed__);
		return _003CUpdateRole_003Ed__._003C_003Et__builder.Task;
	}

	[AsyncStateMachine(typeof(_003CSwapRoleRanks_003Ed__60))]
	public ValueTask<ClanResult> SwapRoleRanks(int roleIdA, int roleIdB, ulong bySteamId)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		_003CSwapRoleRanks_003Ed__60 _003CSwapRoleRanks_003Ed__ = default(_003CSwapRoleRanks_003Ed__60);
		_003CSwapRoleRanks_003Ed__._003C_003Et__builder = AsyncValueTaskMethodBuilder<ClanResult>.Create();
		_003CSwapRoleRanks_003Ed__._003C_003E4__this = this;
		_003CSwapRoleRanks_003Ed__.roleIdA = roleIdA;
		_003CSwapRoleRanks_003Ed__.roleIdB = roleIdB;
		_003CSwapRoleRanks_003Ed__.bySteamId = bySteamId;
		_003CSwapRoleRanks_003Ed__._003C_003E1__state = -1;
		_003CSwapRoleRanks_003Ed__._003C_003Et__builder.Start<_003CSwapRoleRanks_003Ed__60>(ref _003CSwapRoleRanks_003Ed__);
		return _003CSwapRoleRanks_003Ed__._003C_003Et__builder.Task;
	}

	[AsyncStateMachine(typeof(_003CDeleteRole_003Ed__61))]
	public ValueTask<ClanResult> DeleteRole(int roleId, ulong bySteamId)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		_003CDeleteRole_003Ed__61 _003CDeleteRole_003Ed__ = default(_003CDeleteRole_003Ed__61);
		_003CDeleteRole_003Ed__._003C_003Et__builder = AsyncValueTaskMethodBuilder<ClanResult>.Create();
		_003CDeleteRole_003Ed__._003C_003E4__this = this;
		_003CDeleteRole_003Ed__.roleId = roleId;
		_003CDeleteRole_003Ed__.bySteamId = bySteamId;
		_003CDeleteRole_003Ed__._003C_003E1__state = -1;
		_003CDeleteRole_003Ed__._003C_003Et__builder.Start<_003CDeleteRole_003Ed__61>(ref _003CDeleteRole_003Ed__);
		return _003CDeleteRole_003Ed__._003C_003Et__builder.Task;
	}

	[AsyncStateMachine(typeof(_003CDisband_003Ed__62))]
	public ValueTask<ClanResult> Disband(ulong bySteamId)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		_003CDisband_003Ed__62 _003CDisband_003Ed__ = default(_003CDisband_003Ed__62);
		_003CDisband_003Ed__._003C_003Et__builder = AsyncValueTaskMethodBuilder<ClanResult>.Create();
		_003CDisband_003Ed__._003C_003E4__this = this;
		_003CDisband_003Ed__.bySteamId = bySteamId;
		_003CDisband_003Ed__._003C_003E1__state = -1;
		_003CDisband_003Ed__._003C_003Et__builder.Start<_003CDisband_003Ed__62>(ref _003CDisband_003Ed__);
		return _003CDisband_003Ed__._003C_003Et__builder.Task;
	}

	public ValueTask<ClanValueResult<ClanChatScrollback>> GetChatScrollback()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		lock (_chatHistory)
		{
			ClanChatScrollback val = default(ClanChatScrollback);
			val.ClanId = ClanId;
			val.Entries = _chatHistory.ToList();
			return new ValueTask<ClanValueResult<ClanChatScrollback>>(ClanValueResult<ClanChatScrollback>.op_Implicit(val));
		}
	}

	public ValueTask<ClanResult> SendChatMessage(string name, string message, ulong bySteamId)
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		if (!List.TryFindWith<ClanMember, ulong>((IReadOnlyCollection<ClanMember>)_members, (Func<ClanMember, ulong>)((ClanMember m) => m.SteamId), bySteamId, (IEqualityComparer<ulong>)null).HasValue)
		{
			return new ValueTask<ClanResult>((ClanResult)0);
		}
		string message2 = default(string);
		if (!ClanValidator.ValidateChatMessage(message, ref message2))
		{
			return new ValueTask<ClanResult>((ClanResult)6);
		}
		ClanChatEntry val = default(ClanChatEntry);
		val.SteamId = bySteamId;
		val.Name = name;
		val.Message = message2;
		val.Time = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
		ClanChatEntry entry = val;
		AddScrollback(in entry);
		_chatCollector.OnClanChatMessage(ClanId, entry);
		return new ValueTask<ClanResult>((ClanResult)1);
	}

	public void AddScrollback(in ClanChatEntry entry)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		lock (_chatHistory)
		{
			if (_chatHistory.Count >= 20)
			{
				_chatHistory.RemoveAt(0);
			}
			_chatHistory.Add(entry);
		}
	}

	private bool CheckRole(ulong steamId, Func<ClanRole, bool> roleTest)
	{
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		ClanMember? val = List.TryFindWith<ClanMember, ulong>((IReadOnlyCollection<ClanMember>)_members, (Func<ClanMember, ulong>)((ClanMember m) => m.SteamId), steamId, (IEqualityComparer<ulong>)null);
		if (!val.HasValue)
		{
			return false;
		}
		ClanRole? val2 = List.TryFindWith<ClanRole, int>((IReadOnlyCollection<ClanRole>)_roles, (Func<ClanRole, int>)((ClanRole r) => r.RoleId), val.Value.RoleId, (IEqualityComparer<int>)null);
		if (!val2.HasValue)
		{
			return false;
		}
		if (val2.Value.Rank != 1)
		{
			return roleTest(val2.Value);
		}
		return true;
	}
}
