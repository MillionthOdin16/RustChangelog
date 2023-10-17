using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using ConVar;
using Facepunch;
using Fleck;
using ProtoBuf;
using UnityEngine;

namespace CompanionServer;

public class Connection : IConnection
{
	private static readonly MemoryStream MessageStream = new MemoryStream(1048576);

	private readonly Listener _listener;

	private readonly IWebSocketConnection _connection;

	private PlayerTarget? _subscribedPlayer;

	private readonly HashSet<EntityTarget> _subscribedEntities;

	private readonly HashSet<ClanTarget> _subscribedClans;

	private IRemoteControllable _currentCamera;

	private ulong _cameraViewerSteamId;

	private bool _isControllingCamera;

	public long ConnectionId { get; private set; }

	public IPAddress Address => _connection.ConnectionInfo.ClientIpAddress;

	public IRemoteControllable CurrentCamera => _currentCamera;

	public bool IsControllingCamera => _isControllingCamera;

	public ulong ControllingSteamId => _cameraViewerSteamId;

	public InputState InputState { get; set; }

	public Connection(long connectionId, Listener listener, IWebSocketConnection connection)
	{
		ConnectionId = connectionId;
		_listener = listener;
		_connection = connection;
		_subscribedEntities = new HashSet<EntityTarget>();
		_subscribedClans = new HashSet<ClanTarget>();
	}

	public void OnClose()
	{
		if (_subscribedPlayer.HasValue)
		{
			_listener.PlayerSubscribers.Remove(_subscribedPlayer.Value, this);
			_subscribedPlayer = null;
		}
		foreach (EntityTarget subscribedEntity in _subscribedEntities)
		{
			_listener.EntitySubscribers.Remove(subscribedEntity, this);
		}
		_subscribedEntities.Clear();
		foreach (ClanTarget subscribedClan in _subscribedClans)
		{
			_listener.ClanSubscribers.Remove(subscribedClan, this);
		}
		_subscribedClans.Clear();
		_currentCamera?.StopControl(new CameraViewerId(_cameraViewerSteamId, ConnectionId));
		if (TryGetCameraTarget(_currentCamera, out var target))
		{
			_listener.CameraSubscribers.Remove(target, this);
		}
		_currentCamera = null;
		_cameraViewerSteamId = 0uL;
		_isControllingCamera = false;
	}

	public void OnMessage(System.Span<byte> data)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		if (App.update && App.queuelimit > 0 && data.Length <= App.maxmessagesize)
		{
			MemoryBuffer val = default(MemoryBuffer);
			((MemoryBuffer)(ref val))._002Ector(data.Length);
			data.CopyTo(MemoryBuffer.op_Implicit(val));
			_listener.Enqueue(this, ((MemoryBuffer)(ref val)).Slice(data.Length));
		}
	}

	public void Close()
	{
		IWebSocketConnection connection = _connection;
		if (connection != null)
		{
			connection.Close();
		}
	}

	public void Send(AppResponse response)
	{
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		AppMessage val = Pool.Get<AppMessage>();
		val.response = response;
		MessageStream.Position = 0L;
		val.ToProto((Stream)MessageStream);
		int num = (int)MessageStream.Position;
		MessageStream.Position = 0L;
		MemoryBuffer val2 = default(MemoryBuffer);
		((MemoryBuffer)(ref val2))._002Ector(num);
		MessageStream.Read(((MemoryBuffer)(ref val2)).Data, 0, num);
		if (val.ShouldPool)
		{
			val.Dispose();
		}
		SendRaw(((MemoryBuffer)(ref val2)).Slice(num));
	}

	public void Subscribe(PlayerTarget target)
	{
		if (!(_subscribedPlayer == target))
		{
			EndViewing();
			if (_subscribedPlayer.HasValue)
			{
				_listener.PlayerSubscribers.Remove(_subscribedPlayer.Value, this);
				_subscribedPlayer = null;
			}
			_listener.PlayerSubscribers.Add(target, this);
			_subscribedPlayer = target;
		}
	}

	public void Subscribe(EntityTarget target)
	{
		if (_subscribedEntities.Add(target))
		{
			_listener.EntitySubscribers.Add(target, this);
		}
	}

	public bool BeginViewing(IRemoteControllable camera)
	{
		if (!_subscribedPlayer.HasValue)
		{
			return false;
		}
		if (!TryGetCameraTarget(camera, out var target))
		{
			if (_currentCamera == camera)
			{
				_currentCamera?.StopControl(new CameraViewerId(_cameraViewerSteamId, ConnectionId));
				_currentCamera = null;
				_isControllingCamera = false;
				_cameraViewerSteamId = 0uL;
			}
			return false;
		}
		if (_currentCamera == camera)
		{
			_listener.CameraSubscribers.Add(target, this);
			return true;
		}
		if (TryGetCameraTarget(_currentCamera, out var target2))
		{
			_listener.CameraSubscribers.Remove(target2, this);
			_currentCamera.StopControl(new CameraViewerId(_cameraViewerSteamId, ConnectionId));
			_currentCamera = null;
			_isControllingCamera = false;
			_cameraViewerSteamId = 0uL;
		}
		ulong steamId = _subscribedPlayer.Value.SteamId;
		if (!camera.CanControl(steamId))
		{
			return false;
		}
		_listener.CameraSubscribers.Add(target, this);
		_currentCamera = camera;
		_isControllingCamera = _currentCamera.InitializeControl(new CameraViewerId(steamId, ConnectionId));
		_cameraViewerSteamId = steamId;
		InputState?.Clear();
		return true;
	}

	public void EndViewing()
	{
		if (TryGetCameraTarget(_currentCamera, out var target))
		{
			_listener.CameraSubscribers.Remove(target, this);
		}
		_currentCamera?.StopControl(new CameraViewerId(_cameraViewerSteamId, ConnectionId));
		_currentCamera = null;
		_isControllingCamera = false;
		_cameraViewerSteamId = 0uL;
	}

	public void Subscribe(ClanTarget target)
	{
		if (_subscribedClans.Contains(target))
		{
			return;
		}
		foreach (ClanTarget subscribedClan in _subscribedClans)
		{
			_listener.ClanSubscribers.Remove(subscribedClan, this);
		}
		_subscribedClans.Clear();
		if (_subscribedClans.Add(target))
		{
			_listener.ClanSubscribers.Add(target, this);
		}
	}

	public void Unsubscribe(ClanTarget target)
	{
		if (_subscribedClans.Remove(target))
		{
			_listener.ClanSubscribers.Remove(target, this);
		}
	}

	public void SendRaw(MemoryBuffer data)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			_connection.Send(data);
		}
		catch (Exception arg)
		{
			Debug.LogError((object)$"Failed to send message to app client {_connection.ConnectionInfo.ClientIpAddress}: {arg}");
		}
	}

	private static bool TryGetCameraTarget(IRemoteControllable camera, out CameraTarget target)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		BaseEntity baseEntity = camera?.GetEnt();
		if (camera.IsUnityNull() || (Object)(object)baseEntity == (Object)null || !baseEntity.IsValid())
		{
			target = default(CameraTarget);
			return false;
		}
		target = new CameraTarget(baseEntity.net.ID);
		return true;
	}
}
