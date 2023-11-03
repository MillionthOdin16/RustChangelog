using System;
using System.Collections.Generic;
using CompanionServer;
using ConVar;
using Facepunch;
using Facepunch.Extend;
using ProtoBuf;
using UnityEngine;

public class ClanChangeTracker : IClanChangeSink
{
	private struct ClanChangedEvent
	{
		public long ClanId;

		public ClanDataSource DataSources;
	}

	private struct ClanDisbandedEvent
	{
		public long ClanId;
	}

	private struct InvitationCreatedEvent
	{
		public ulong SteamId;

		public long ClanId;
	}

	private struct MembershipChangedEvent
	{
		public ulong SteamId;

		public long ClanId;
	}

	private struct ChatMessageEvent
	{
		public long ClanId;

		public ClanChatEntry Message;
	}

	private class ChatMessageEventComparer : IComparer<ChatMessageEvent>
	{
		public static readonly ChatMessageEventComparer Instance = new ChatMessageEventComparer();

		public int Compare(ChatMessageEvent x, ChatMessageEvent y)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			return x.Message.Time.CompareTo(y.Message.Time);
		}
	}

	private readonly ClanManager _clanManager;

	private readonly List<ClanChangedEvent> _clanChangedEvents = new List<ClanChangedEvent>();

	private readonly List<ClanDisbandedEvent> _clanDisbandedEvents = new List<ClanDisbandedEvent>();

	private readonly List<InvitationCreatedEvent> _invitationCreatedEvents = new List<InvitationCreatedEvent>();

	private readonly List<MembershipChangedEvent> _membershipChangedEvents = new List<MembershipChangedEvent>();

	private readonly List<ChatMessageEvent> _chatMessageEvents = new List<ChatMessageEvent>();

	public ClanChangeTracker(ClanManager clanManager)
	{
		_clanManager = clanManager;
	}

	public void HandleEvents()
	{
		lock (_clanChangedEvents)
		{
			foreach (ClanChangedEvent clanChangedEvent in _clanChangedEvents)
			{
				ClanChangedEvent data = clanChangedEvent;
				HandleClanChanged(in data);
			}
			_clanChangedEvents.Clear();
		}
		lock (_clanDisbandedEvents)
		{
			foreach (ClanDisbandedEvent clanDisbandedEvent in _clanDisbandedEvents)
			{
				ClanDisbandedEvent data2 = clanDisbandedEvent;
				HandleClanDisbanded(in data2);
			}
			_clanDisbandedEvents.Clear();
		}
		lock (_invitationCreatedEvents)
		{
			foreach (InvitationCreatedEvent invitationCreatedEvent in _invitationCreatedEvents)
			{
				InvitationCreatedEvent data3 = invitationCreatedEvent;
				HandleInvitationCreated(in data3);
			}
			_invitationCreatedEvents.Clear();
		}
		lock (_membershipChangedEvents)
		{
			foreach (MembershipChangedEvent membershipChangedEvent in _membershipChangedEvents)
			{
				MembershipChangedEvent data4 = membershipChangedEvent;
				HandleMembershipChanged(in data4);
			}
			_membershipChangedEvents.Clear();
		}
		lock (_chatMessageEvents)
		{
			foreach (ChatMessageEvent chatMessageEvent in _chatMessageEvents)
			{
				ChatMessageEvent data5 = chatMessageEvent;
				HandleChatMessageEvent(in data5);
			}
			_chatMessageEvents.Clear();
		}
	}

	private void HandleClanChanged(in ClanChangedEvent data)
	{
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		IClan clan = default(IClan);
		if (_clanManager.Backend.TryGet(data.ClanId, ref clan))
		{
			_clanManager.SendClanChanged(clan);
			AppBroadcast val = Pool.Get<AppBroadcast>();
			val.clanChanged = Pool.Get<AppClanChanged>();
			val.clanChanged.clanInfo = clan.ToProto();
			CompanionServer.Server.Broadcast(new ClanTarget(data.ClanId), val);
		}
		if (((Enum)data.DataSources).HasFlag((Enum)(object)(ClanDataSource)16))
		{
			_clanManager.ClanMemberConnectionsChanged(data.ClanId);
		}
	}

	private void HandleClanDisbanded(in ClanDisbandedEvent data)
	{
	}

	private void HandleInvitationCreated(in InvitationCreatedEvent data)
	{
		_clanManager.SendClanInvitation(data.SteamId, data.ClanId);
	}

	private void HandleMembershipChanged(in MembershipChangedEvent data)
	{
		BasePlayer basePlayer = BasePlayer.FindByID(data.SteamId);
		if ((Object)(object)basePlayer == (Object)null)
		{
			basePlayer = BasePlayer.FindSleeping(data.SteamId);
		}
		if ((Object)(object)basePlayer != (Object)null)
		{
			basePlayer.clanId = data.ClanId;
			basePlayer.SendNetworkUpdateImmediate();
			if (basePlayer.IsConnected)
			{
				_clanManager.ClientRPCPlayer(null, basePlayer, "Client_CurrentClanChanged");
			}
		}
	}

	private void HandleChatMessageEvent(in ChatMessageEvent data)
	{
		if (_clanManager.TryGetClanMemberConnections(data.ClanId, out var connections) && connections.Count > 0)
		{
			string nameColor = Chat.GetNameColor(data.Message.SteamId);
			ConsoleNetwork.SendClientCommand(connections, "chat.add2", 5, data.Message.SteamId, data.Message.Message, data.Message.Name, nameColor, 1f);
		}
		AppBroadcast val = Pool.Get<AppBroadcast>();
		val.clanMessage = Pool.Get<AppNewClanMessage>();
		val.clanMessage.clanId = data.ClanId;
		val.clanMessage.message = Pool.Get<AppClanMessage>();
		val.clanMessage.message.steamId = data.Message.SteamId;
		val.clanMessage.message.name = data.Message.Name;
		val.clanMessage.message.message = data.Message.Message;
		val.clanMessage.message.time = data.Message.Time;
		CompanionServer.Server.Broadcast(new ClanTarget(data.ClanId), val);
	}

	public void ClanChanged(long clanId, ClanDataSource dataSources)
	{
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Expected I4, but got Unknown
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		lock (_clanChangedEvents)
		{
			int num = List.FindIndexWith<ClanChangedEvent, long>((IReadOnlyList<ClanChangedEvent>)_clanChangedEvents, (Func<ClanChangedEvent, long>)((ClanChangedEvent e) => e.ClanId), clanId, (IEqualityComparer<long>)null);
			if (num < 0)
			{
				_clanChangedEvents.Add(new ClanChangedEvent
				{
					ClanId = clanId,
					DataSources = dataSources
				});
			}
			else
			{
				ClanChangedEvent value = _clanChangedEvents[num];
				ref ClanDataSource dataSources2 = ref value.DataSources;
				dataSources2 |= dataSources;
				_clanChangedEvents[num] = value;
			}
		}
	}

	public void ClanDisbanded(long clanId)
	{
		lock (_clanDisbandedEvents)
		{
			_clanDisbandedEvents.Add(new ClanDisbandedEvent
			{
				ClanId = clanId
			});
		}
	}

	public void InvitationCreated(ulong steamId, long clanId)
	{
		lock (_invitationCreatedEvents)
		{
			_invitationCreatedEvents.Add(new InvitationCreatedEvent
			{
				SteamId = steamId,
				ClanId = clanId
			});
		}
	}

	public void MembershipChanged(ulong steamId, long? clanId)
	{
		lock (_membershipChangedEvents)
		{
			_membershipChangedEvents.Add(new MembershipChangedEvent
			{
				SteamId = steamId,
				ClanId = clanId.GetValueOrDefault()
			});
		}
	}

	public void ClanChatMessage(long clanId, ClanChatEntry entry)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		lock (_chatMessageEvents)
		{
			ChatMessageEvent chatMessageEvent = default(ChatMessageEvent);
			chatMessageEvent.ClanId = clanId;
			chatMessageEvent.Message = entry;
			ChatMessageEvent item = chatMessageEvent;
			int num = _chatMessageEvents.BinarySearch(item, ChatMessageEventComparer.Instance);
			_chatMessageEvents.Insert((num >= 0) ? num : (~num), item);
		}
	}
}
