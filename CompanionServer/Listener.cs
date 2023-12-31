using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using CompanionServer.Handlers;
using ConVar;
using Facepunch;
using Fleck;
using ProtoBuf;
using UnityEngine;

namespace CompanionServer;

public class Listener : IDisposable, IBroadcastSender<Connection, AppBroadcast>
{
	private struct Message
	{
		public readonly Connection Connection;

		public readonly MemoryBuffer Buffer;

		public Message(Connection connection, MemoryBuffer buffer)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			Connection = connection;
			Buffer = buffer;
		}
	}

	private static readonly ByteArrayStream Stream = new ByteArrayStream();

	private readonly TokenBucketList<IPAddress> _ipTokenBuckets;

	private readonly BanList<IPAddress> _ipBans;

	private readonly TokenBucketList<ulong> _playerTokenBuckets;

	private readonly TokenBucketList<ulong> _pairingTokenBuckets;

	private readonly Queue<Message> _messageQueue;

	private readonly WebSocketServer _server;

	private readonly Stopwatch _stopwatch;

	private RealTimeSince _lastCleanup;

	private long _nextConnectionId;

	public readonly IPAddress Address;

	public readonly int Port;

	public readonly ConnectionLimiter Limiter;

	public readonly SubscriberList<PlayerTarget, Connection, AppBroadcast> PlayerSubscribers;

	public readonly SubscriberList<EntityTarget, Connection, AppBroadcast> EntitySubscribers;

	public readonly SubscriberList<ClanTarget, Connection, AppBroadcast> ClanSubscribers;

	public readonly SubscriberList<CameraTarget, Connection, AppBroadcast> CameraSubscribers;

	public Listener(IPAddress ipAddress, int port)
	{
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Expected O, but got Unknown
		Listener listener = this;
		Address = ipAddress;
		Port = port;
		Limiter = new ConnectionLimiter();
		_ipTokenBuckets = new TokenBucketList<IPAddress>(50.0, 15.0);
		_ipBans = new BanList<IPAddress>();
		_playerTokenBuckets = new TokenBucketList<ulong>(25.0, 3.0);
		_pairingTokenBuckets = new TokenBucketList<ulong>(5.0, 0.1);
		_messageQueue = new Queue<Message>();
		SynchronizationContext syncContext = SynchronizationContext.Current;
		_server = new WebSocketServer($"ws://{Address}:{Port}/", true);
		_server.Start((Action<IWebSocketConnection>)delegate(IWebSocketConnection socket)
		{
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Expected O, but got Unknown
			IPAddress address = socket.ConnectionInfo.ClientIpAddress;
			if (!listener.Limiter.TryAdd(address) || listener._ipBans.IsBanned(address))
			{
				socket.Close();
			}
			else
			{
				long connectionId = Interlocked.Increment(ref listener._nextConnectionId);
				Connection conn = new Connection(connectionId, listener, socket);
				socket.OnClose = delegate
				{
					listener.Limiter.Remove(address);
					syncContext.Post(delegate(object c)
					{
						((Connection)c).OnClose();
					}, conn);
				};
				socket.OnBinary = new BinaryDataHandler(conn.OnMessage);
				socket.OnError = Debug.LogError;
			}
		});
		_stopwatch = new Stopwatch();
		PlayerSubscribers = new SubscriberList<PlayerTarget, Connection, AppBroadcast>(this);
		EntitySubscribers = new SubscriberList<EntityTarget, Connection, AppBroadcast>(this);
		ClanSubscribers = new SubscriberList<ClanTarget, Connection, AppBroadcast>(this);
		CameraSubscribers = new SubscriberList<CameraTarget, Connection, AppBroadcast>(this, 30.0);
	}

	public void Dispose()
	{
		WebSocketServer server = _server;
		if (server != null)
		{
			server.Dispose();
		}
	}

	internal void Enqueue(Connection connection, MemoryBuffer data)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		lock (_messageQueue)
		{
			if (!App.update || _messageQueue.Count >= App.queuelimit)
			{
				((MemoryBuffer)(ref data)).Dispose();
				return;
			}
			Message item = new Message(connection, data);
			_messageQueue.Enqueue(item);
		}
	}

	public void Update()
	{
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		if (!App.update)
		{
			return;
		}
		TimeWarning val = TimeWarning.New("CompanionServer.MessageQueue", 0);
		try
		{
			lock (_messageQueue)
			{
				_stopwatch.Restart();
				while (_messageQueue.Count > 0 && _stopwatch.Elapsed.TotalMilliseconds < 5.0)
				{
					Message message = _messageQueue.Dequeue();
					Dispatch(message);
				}
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		if (RealTimeSince.op_Implicit(_lastCleanup) >= 3f)
		{
			_lastCleanup = RealTimeSince.op_Implicit(0f);
			_ipTokenBuckets.Cleanup();
			_ipBans.Cleanup();
			_playerTokenBuckets.Cleanup();
			_pairingTokenBuckets.Cleanup();
		}
	}

	private void Dispatch(Message message)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		MemoryBuffer buffer = message.Buffer;
		AppRequest request;
		try
		{
			ByteArrayStream stream = Stream;
			MemoryBuffer buffer2 = message.Buffer;
			byte[] data = ((MemoryBuffer)(ref buffer2)).Data;
			buffer2 = message.Buffer;
			stream.SetData(data, 0, ((MemoryBuffer)(ref buffer2)).Length);
			request = AppRequest.Deserialize((Stream)(object)Stream);
		}
		catch
		{
			DebugEx.LogWarning((object)$"Malformed companion packet from {message.Connection.Address}", (StackTraceLogType)0);
			message.Connection.Close();
			throw;
		}
		finally
		{
			((MemoryBuffer)(ref buffer)).Dispose();
		}
		if (Handle<AppEmpty, Info>((AppRequest r) => r.getInfo, out var requestHandler2) || Handle<AppEmpty, CompanionServer.Handlers.Time>((AppRequest r) => r.getTime, out requestHandler2) || Handle<AppEmpty, Map>((AppRequest r) => r.getMap, out requestHandler2) || Handle<AppEmpty, TeamInfo>((AppRequest r) => r.getTeamInfo, out requestHandler2) || Handle<AppEmpty, TeamChat>((AppRequest r) => r.getTeamChat, out requestHandler2) || Handle<AppSendMessage, SendTeamChat>((AppRequest r) => r.sendTeamMessage, out requestHandler2) || Handle<AppEmpty, EntityInfo>((AppRequest r) => r.getEntityInfo, out requestHandler2) || Handle<AppSetEntityValue, SetEntityValue>((AppRequest r) => r.setEntityValue, out requestHandler2) || Handle<AppEmpty, CheckSubscription>((AppRequest r) => r.checkSubscription, out requestHandler2) || Handle<AppFlag, SetSubscription>((AppRequest r) => r.setSubscription, out requestHandler2) || Handle<AppEmpty, MapMarkers>((AppRequest r) => r.getMapMarkers, out requestHandler2) || Handle<AppPromoteToLeader, PromoteToLeader>((AppRequest r) => r.promoteToLeader, out requestHandler2) || Handle<AppEmpty, ClanInfo>((AppRequest r) => r.getClanInfo, out requestHandler2) || Handle<AppEmpty, ClanChat>((AppRequest r) => r.getClanChat, out requestHandler2) || Handle<AppSendMessage, SetClanMotd>((AppRequest r) => r.setClanMotd, out requestHandler2) || Handle<AppSendMessage, SendClanChat>((AppRequest r) => r.sendClanMessage, out requestHandler2) || Handle<AppGetNexusAuth, NexusAuth>((AppRequest r) => r.getNexusAuth, out requestHandler2) || Handle<AppCameraSubscribe, CameraSubscribe>((AppRequest r) => r.cameraSubscribe, out requestHandler2) || Handle<AppEmpty, CameraUnsubscribe>((AppRequest r) => r.cameraUnsubscribe, out requestHandler2) || Handle<AppCameraInput, CameraInput>((AppRequest r) => r.cameraInput, out requestHandler2))
		{
			try
			{
				ValidationResult validationResult = requestHandler2.Validate();
				switch (validationResult)
				{
				case ValidationResult.Rejected:
					message.Connection.Close();
					break;
				default:
					requestHandler2.SendError(validationResult.ToErrorCode());
					break;
				case ValidationResult.Success:
					requestHandler2.Execute();
					break;
				}
			}
			catch (Exception arg)
			{
				Debug.LogError((object)$"AppRequest threw an exception: {arg}");
				requestHandler2.SendError("server_error");
			}
			Pool.FreeDynamic<IHandler>(ref requestHandler2);
		}
		else
		{
			AppResponse val = Pool.Get<AppResponse>();
			val.seq = request.seq;
			val.error = Pool.Get<AppError>();
			val.error.error = "unhandled";
			message.Connection.Send(val);
			request.Dispose();
		}
		bool Handle<TProto, THandler>(Func<AppRequest, TProto> protoSelector, out IHandler requestHandler) where TProto : class where THandler : BaseHandler<TProto>, new()
		{
			TProto val2 = protoSelector(request);
			if (val2 == null)
			{
				requestHandler = null;
				return false;
			}
			THandler val3 = Pool.Get<THandler>();
			val3.Initialize(_playerTokenBuckets, message.Connection, request, val2);
			requestHandler = val3;
			return true;
		}
	}

	public void BroadcastTo(List<Connection> targets, AppBroadcast broadcast)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		MemoryBuffer broadcastBuffer = GetBroadcastBuffer(broadcast);
		foreach (Connection target in targets)
		{
			target.SendRaw(((MemoryBuffer)(ref broadcastBuffer)).DontDispose());
		}
		((MemoryBuffer)(ref broadcastBuffer)).Dispose();
	}

	private static MemoryBuffer GetBroadcastBuffer(AppBroadcast broadcast)
	{
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		MemoryBuffer val = default(MemoryBuffer);
		((MemoryBuffer)(ref val))._002Ector(65536);
		Stream.SetData(((MemoryBuffer)(ref val)).Data, 0, ((MemoryBuffer)(ref val)).Length);
		AppMessage val2 = Pool.Get<AppMessage>();
		val2.broadcast = broadcast;
		val2.ToProto((Stream)(object)Stream);
		if (val2.ShouldPool)
		{
			val2.Dispose();
		}
		return ((MemoryBuffer)(ref val)).Slice((int)((Stream)(object)Stream).Position);
	}

	public bool CanSendPairingNotification(ulong playerId)
	{
		return _pairingTokenBuckets.Get(playerId).TryTake(1.0);
	}
}
