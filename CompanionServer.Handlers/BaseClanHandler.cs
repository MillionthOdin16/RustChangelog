using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;

namespace CompanionServer.Handlers;

public abstract class BaseClanHandler<T> : BasePlayerHandler<T> where T : class
{
	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	private struct _003CGetClan_003Ed__4 : IAsyncStateMachine
	{
		public int _003C_003E1__state;

		public AsyncValueTaskMethodBuilder<IClan> _003C_003Et__builder;

		public BaseClanHandler<T> _003C_003E4__this;

		private ValueTaskAwaiter<ClanValueResult<IClan>> _003C_003Eu__1;

		private void MoveNext()
		{
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_009c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_0108: Unknown result type (might be due to invalid IL or missing references)
			//IL_010d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0115: Unknown result type (might be due to invalid IL or missing references)
			//IL_012d: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0131: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Invalid comparison between Unknown and I4
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0139: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0140: Invalid comparison between Unknown and I4
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			int num = _003C_003E1__state;
			BaseClanHandler<T> baseClanHandler = _003C_003E4__this;
			IClan result;
			try
			{
				ValueTaskAwaiter<ClanValueResult<IClan>> awaiter;
				if (num == 0)
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ValueTaskAwaiter<ClanValueResult<IClan>>);
					num = (_003C_003E1__state = -1);
					goto IL_00b3;
				}
				if (num == 1)
				{
					awaiter = _003C_003Eu__1;
					_003C_003Eu__1 = default(ValueTaskAwaiter<ClanValueResult<IClan>>);
					num = (_003C_003E1__state = -1);
					goto IL_0124;
				}
				if (baseClanHandler.ClanBackend != null)
				{
					if ((Object)(object)baseClanHandler.Player != (Object)null && baseClanHandler.Player.clanId != 0L)
					{
						awaiter = baseClanHandler.ClanBackend.Get(baseClanHandler.Player.clanId).GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = (_003C_003E1__state = 0);
							_003C_003Eu__1 = awaiter;
							_003C_003Et__builder.AwaitUnsafeOnCompleted<ValueTaskAwaiter<ClanValueResult<IClan>>, _003CGetClan_003Ed__4>(ref awaiter, ref this);
							return;
						}
						goto IL_00b3;
					}
					awaiter = baseClanHandler.ClanBackend.GetByMember(baseClanHandler.UserId).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = (_003C_003E1__state = 1);
						_003C_003Eu__1 = awaiter;
						_003C_003Et__builder.AwaitUnsafeOnCompleted<ValueTaskAwaiter<ClanValueResult<IClan>>, _003CGetClan_003Ed__4>(ref awaiter, ref this);
						return;
					}
					goto IL_0124;
				}
				result = null;
				goto end_IL_000e;
				IL_012d:
				ClanValueResult<IClan> result2;
				ClanValueResult<IClan> val = result2;
				if ((int)val.Result != 3 && (int)val.Result != 4)
				{
					IClan value = val.Value;
					baseClanHandler.Client.Subscribe(new ClanTarget(value.ClanId));
					result = value;
				}
				else
				{
					result = null;
				}
				goto end_IL_000e;
				IL_0124:
				result2 = awaiter.GetResult();
				goto IL_012d;
				IL_00b3:
				result2 = awaiter.GetResult();
				goto IL_012d;
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

	protected IClanBackend ClanBackend { get; private set; }

	[AsyncStateMachine(typeof(BaseClanHandler<>._003CGetClan_003Ed__4))]
	protected ValueTask<IClan> GetClan()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		_003CGetClan_003Ed__4 _003CGetClan_003Ed__ = default(_003CGetClan_003Ed__4);
		_003CGetClan_003Ed__._003C_003Et__builder = AsyncValueTaskMethodBuilder<IClan>.Create();
		_003CGetClan_003Ed__._003C_003E4__this = this;
		_003CGetClan_003Ed__._003C_003E1__state = -1;
		_003CGetClan_003Ed__._003C_003Et__builder.Start<_003CGetClan_003Ed__4>(ref _003CGetClan_003Ed__);
		return _003CGetClan_003Ed__._003C_003Et__builder.Task;
	}

	public override void EnterPool()
	{
		base.EnterPool();
		ClanBackend = null;
	}

	public override ValidationResult Validate()
	{
		ValidationResult num = base.Validate();
		if (num == ValidationResult.Success && (Object)(object)ClanManager.ServerInstance != (Object)null)
		{
			ClanBackend = ClanManager.ServerInstance.Backend;
		}
		return num;
	}

	protected void SendError(ClanResult result)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		base.SendError(GetErrorString(result));
	}

	private static string GetErrorString(ClanResult result)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Expected I4, but got Unknown
		return (int)result switch
		{
			1 => throw new ArgumentException("ClanResult.Success is not an error"), 
			2 => "clan_timeout", 
			3 => "clan_no_clan", 
			4 => "clan_not_found", 
			5 => "clan_no_permission", 
			6 => "clan_invalid_text", 
			7 => "clan_invalid_logo", 
			8 => "clan_invalid_color", 
			9 => "clan_duplicate_name", 
			10 => "clan_role_not_empty", 
			11 => "clan_cannot_swap_leader", 
			12 => "clan_cannot_delete_leader", 
			13 => "clan_cannot_kick_leader", 
			14 => "clan_cannot_demote_leader", 
			15 => "clan_already_in_clan", 
			_ => "clan_fail", 
		};
	}
}
