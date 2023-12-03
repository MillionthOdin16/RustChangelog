using System;
using System.Collections.Concurrent;
using System.IO;
using ConVar;
using Epic.OnlineServices;
using Epic.OnlineServices.AntiCheatCommon;
using Epic.OnlineServices.AntiCheatServer;
using Epic.OnlineServices.Connect;
using Epic.OnlineServices.Reports;
using Network;
using UnityEngine;

public static class EACServer
{
	private static AntiCheatServerInterface Interface = null;

	private static ReportsInterface Reports = null;

	private static ConcurrentDictionary<uint, Connection> client2connection = new ConcurrentDictionary<uint, Connection>();

	private static ConcurrentDictionary<Connection, uint> connection2client = new ConcurrentDictionary<Connection, uint>();

	private static ConcurrentDictionary<Connection, AntiCheatCommonClientAuthStatus> connection2status = new ConcurrentDictionary<Connection, AntiCheatCommonClientAuthStatus>();

	private static uint clientHandleCounter = 0u;

	private static bool CanEnableGameplayData
	{
		get
		{
			if (Server.official)
			{
				return Server.stats;
			}
			return false;
		}
	}

	private static bool CanSendAnalytics
	{
		get
		{
			if (CanEnableGameplayData)
			{
				return (Handle)(object)Interface != (Handle)null;
			}
			return false;
		}
	}

	private static bool CanSendReports => (Handle)(object)Reports != (Handle)null;

	private static IntPtr GenerateCompatibilityClient()
	{
		return (IntPtr)(++clientHandleCounter);
	}

	public static void Encrypt(Connection connection, ArraySegment<byte> src, ref ArraySegment<byte> dst)
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		uint count = (uint)dst.Count;
		dst = new ArraySegment<byte>(dst.Array, dst.Offset, 0);
		if (!((Handle)(object)Interface != (Handle)null))
		{
			return;
		}
		IntPtr client = GetClient(connection);
		if (client != IntPtr.Zero)
		{
			ProtectMessageOptions val = default(ProtectMessageOptions);
			((ProtectMessageOptions)(ref val)).ClientHandle = client;
			((ProtectMessageOptions)(ref val)).Data = src;
			((ProtectMessageOptions)(ref val)).OutBufferSizeBytes = count;
			ProtectMessageOptions val2 = val;
			uint count2 = default(uint);
			Result val3 = Interface.ProtectMessage(ref val2, dst, ref count2);
			if ((int)val3 == 0)
			{
				dst = new ArraySegment<byte>(dst.Array, dst.Offset, (int)count2);
			}
			else
			{
				Debug.LogWarning((object)("[EAC] ProtectMessage failed: " + ((object)(Result)(ref val3)).ToString()));
			}
		}
	}

	public static void Decrypt(Connection connection, ArraySegment<byte> src, ref ArraySegment<byte> dst)
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		uint count = (uint)dst.Count;
		dst = new ArraySegment<byte>(dst.Array, dst.Offset, 0);
		if (!((Handle)(object)Interface != (Handle)null))
		{
			return;
		}
		IntPtr client = GetClient(connection);
		if (client != IntPtr.Zero)
		{
			UnprotectMessageOptions val = default(UnprotectMessageOptions);
			((UnprotectMessageOptions)(ref val)).ClientHandle = client;
			((UnprotectMessageOptions)(ref val)).Data = src;
			((UnprotectMessageOptions)(ref val)).OutBufferSizeBytes = count;
			UnprotectMessageOptions val2 = val;
			uint count2 = default(uint);
			Result val3 = Interface.UnprotectMessage(ref val2, dst, ref count2);
			if ((int)val3 == 0)
			{
				dst = new ArraySegment<byte>(dst.Array, dst.Offset, (int)count2);
			}
			else
			{
				Debug.LogWarning((object)("[EAC] UnprotectMessage failed: " + ((object)(Result)(ref val3)).ToString()));
			}
		}
	}

	private static IntPtr GetClient(Connection connection)
	{
		connection2client.TryGetValue(connection, out var value);
		return (IntPtr)value;
	}

	private static Connection GetConnection(IntPtr client)
	{
		client2connection.TryGetValue((uint)(int)client, out var value);
		return value;
	}

	public static bool IsAuthenticated(Connection connection)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Invalid comparison between Unknown and I4
		connection2status.TryGetValue(connection, out var value);
		return (int)value == 2;
	}

	private static void OnAuthenticatedLocal(Connection connection)
	{
		if (connection.authStatus == string.Empty)
		{
			connection.authStatus = "ok";
		}
		connection2status[connection] = (AntiCheatCommonClientAuthStatus)1;
	}

	private static void OnAuthenticatedRemote(Connection connection)
	{
		connection2status[connection] = (AntiCheatCommonClientAuthStatus)2;
	}

	private static void OnVerifyIdToken(ref VerifyIdTokenCallbackInfo data)
	{
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		if (!Server.anticheattoken)
		{
			Debug.LogWarning((object)"[EAC] Verify ID token skipped: server.anticheattoken == false");
			return;
		}
		IntPtr client = (IntPtr)((VerifyIdTokenCallbackInfo)(ref data)).ClientData;
		Connection connection = GetConnection(client);
		if (connection == null)
		{
			Debug.LogError((object)("[EAC] Verify ID token for invalid client: " + client));
			return;
		}
		if (connection.IsDevelopmentBuild())
		{
			Debug.LogWarning((object)("[EAC] Verify ID token skipped for unprotected client: " + ((object)connection).ToString()));
			return;
		}
		if ((int)((VerifyIdTokenCallbackInfo)(ref data)).ResultCode != 0)
		{
			Result resultCode = ((VerifyIdTokenCallbackInfo)(ref data)).ResultCode;
			string text = "Verify ID token " + ((object)(Result)(ref resultCode)).ToString();
			Debug.Log((object)$"[EAC] Kicking {connection.userid} / {connection.username} ({text})");
			connection.authStatus = "eactoken";
			Net.sv.Kick(connection, "EAC: " + text, false);
			return;
		}
		string text2 = ((object)((VerifyIdTokenCallbackInfo)(ref data)).AccountId).ToString();
		string text3 = connection.userid.ToString();
		if (text2 != text3)
		{
			string text4 = "Verify ID token account mismatch with " + text2 + " != " + text3;
			Debug.Log((object)$"[EAC] Kicking {connection.userid} / {connection.username} ({text4})");
			connection.authStatus = "eactoken";
			Net.sv.Kick(connection, "EAC: " + text4, false);
		}
	}

	private static void OnClientAuthStatusChanged(ref OnClientAuthStatusChangedCallbackInfo data)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Invalid comparison between Unknown and I4
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Invalid comparison between Unknown and I4
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Expected O, but got Unknown
		TimeWarning val = TimeWarning.New("AntiCheatKickPlayer", 10);
		try
		{
			IntPtr clientHandle = ((OnClientAuthStatusChangedCallbackInfo)(ref data)).ClientHandle;
			Connection connection = GetConnection(clientHandle);
			if (connection == null)
			{
				Debug.LogError((object)("[EAC] Status update for invalid client: " + clientHandle));
			}
			else if ((int)((OnClientAuthStatusChangedCallbackInfo)(ref data)).ClientAuthStatus == 1)
			{
				OnAuthenticatedLocal(connection);
				SetClientNetworkStateOptions val2 = default(SetClientNetworkStateOptions);
				((SetClientNetworkStateOptions)(ref val2)).ClientHandle = clientHandle;
				((SetClientNetworkStateOptions)(ref val2)).IsNetworkActive = false;
				SetClientNetworkStateOptions val3 = val2;
				Interface.SetClientNetworkState(ref val3);
			}
			else if ((int)((OnClientAuthStatusChangedCallbackInfo)(ref data)).ClientAuthStatus == 2)
			{
				OnAuthenticatedRemote(connection);
				IdToken val4 = default(IdToken);
				((IdToken)(ref val4)).ProductUserId = ProductUserId.FromString(Utf8String.op_Implicit(connection.anticheatId));
				((IdToken)(ref val4)).JsonWebToken = Utf8String.op_Implicit(connection.anticheatToken);
				IdToken val5 = val4;
				EOS.VerifyIdToken(clientHandle, val5, new OnVerifyIdTokenCallback(OnVerifyIdToken));
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private static void OnClientActionRequired(ref OnClientActionRequiredCallbackInfo data)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Invalid comparison between Unknown and I4
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Invalid comparison between Unknown and I4
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Invalid comparison between Unknown and I4
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Invalid comparison between Unknown and I4
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		TimeWarning val = TimeWarning.New("OnClientActionRequired", 10);
		try
		{
			IntPtr clientHandle = ((OnClientActionRequiredCallbackInfo)(ref data)).ClientHandle;
			Connection connection = GetConnection(clientHandle);
			if (connection == null)
			{
				Debug.LogError((object)("[EAC] Status update for invalid client: " + clientHandle));
			}
			else
			{
				if ((int)((OnClientActionRequiredCallbackInfo)(ref data)).ClientAction != 1)
				{
					return;
				}
				Utf8String actionReasonDetailsString = ((OnClientActionRequiredCallbackInfo)(ref data)).ActionReasonDetailsString;
				Debug.Log((object)$"[EAC] Kicking {connection.userid} / {connection.username} ({actionReasonDetailsString})");
				connection.authStatus = "eac";
				Net.sv.Kick(connection, Utf8String.op_Implicit(Utf8String.op_Implicit("EAC: ") + actionReasonDetailsString), false);
				if ((int)((OnClientActionRequiredCallbackInfo)(ref data)).ActionReasonCode == 10 || (int)((OnClientActionRequiredCallbackInfo)(ref data)).ActionReasonCode == 9)
				{
					connection.authStatus = "eacbanned";
					ConsoleNetwork.BroadcastToAllClients("chat.add", 2, 0, "<color=#fff>SERVER</color> Kicking " + connection.username + " (banned by anticheat)");
					if ((int)((OnClientActionRequiredCallbackInfo)(ref data)).ActionReasonCode == 10)
					{
						Entity.DeleteBy(connection.userid);
					}
				}
				UnregisterClientOptions val2 = default(UnregisterClientOptions);
				((UnregisterClientOptions)(ref val2)).ClientHandle = clientHandle;
				UnregisterClientOptions val3 = val2;
				Interface.UnregisterClient(ref val3);
				client2connection.TryRemove((uint)(int)clientHandle, out var _);
				connection2client.TryRemove(connection, out var _);
				connection2status.TryRemove(connection, out var _);
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private static void SendToClient(ref OnMessageToClientCallbackInfo data)
	{
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		IntPtr clientHandle = ((OnMessageToClientCallbackInfo)(ref data)).ClientHandle;
		Connection connection = GetConnection(clientHandle);
		if (connection == null)
		{
			Debug.LogError((object)("[EAC] Network packet for invalid client: " + clientHandle));
			return;
		}
		NetWrite obj = ((BaseNetwork)Net.sv).StartWrite();
		obj.PacketID((Type)22);
		obj.UInt32((uint)((OnMessageToClientCallbackInfo)(ref data)).MessageData.Count);
		((Stream)(object)obj).Write(((OnMessageToClientCallbackInfo)(ref data)).MessageData.Array, ((OnMessageToClientCallbackInfo)(ref data)).MessageData.Offset, ((OnMessageToClientCallbackInfo)(ref data)).MessageData.Count);
		obj.Send(new SendInfo(connection));
	}

	public static void DoStartup()
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Expected O, but got Unknown
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Expected O, but got Unknown
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Expected O, but got Unknown
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		if (Server.secure && !Application.isEditor)
		{
			client2connection.Clear();
			connection2client.Clear();
			connection2status.Clear();
			AddNotifyClientActionRequiredOptions val = default(AddNotifyClientActionRequiredOptions);
			AddNotifyClientAuthStatusChangedOptions val2 = default(AddNotifyClientAuthStatusChangedOptions);
			AddNotifyMessageToClientOptions val3 = default(AddNotifyMessageToClientOptions);
			BeginSessionOptions val4 = default(BeginSessionOptions);
			((BeginSessionOptions)(ref val4)).LocalUserId = null;
			((BeginSessionOptions)(ref val4)).EnableGameplayData = CanEnableGameplayData;
			((BeginSessionOptions)(ref val4)).RegisterTimeoutSeconds = 20u;
			((BeginSessionOptions)(ref val4)).ServerName = Utf8String.op_Implicit(Server.hostname);
			BeginSessionOptions val5 = val4;
			LogGameRoundStartOptions val6 = default(LogGameRoundStartOptions);
			((LogGameRoundStartOptions)(ref val6)).LevelName = Utf8String.op_Implicit(World.Name);
			LogGameRoundStartOptions val7 = val6;
			EOS.Initialize(true, Server.anticheatid, Server.anticheatkey, Server.rootFolder + "/Log.EAC.txt");
			Interface = EOS.Interface.GetAntiCheatServerInterface();
			Interface.AddNotifyClientActionRequired(ref val, (object)null, new OnClientActionRequiredCallback(OnClientActionRequired));
			Interface.AddNotifyClientAuthStatusChanged(ref val2, (object)null, new OnClientAuthStatusChangedCallback(OnClientAuthStatusChanged));
			Interface.AddNotifyMessageToClient(ref val3, (object)null, new OnMessageToClientCallback(SendToClient));
			Interface.BeginSession(ref val5);
			Interface.LogGameRoundStart(ref val7);
		}
		else
		{
			client2connection.Clear();
			connection2client.Clear();
			connection2status.Clear();
		}
	}

	public static void DoUpdate()
	{
		if (Server.secure && !Application.isEditor)
		{
			EOS.Tick();
		}
	}

	public static void DoShutdown()
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		if (Server.secure && !Application.isEditor)
		{
			client2connection.Clear();
			connection2client.Clear();
			connection2status.Clear();
			if ((Handle)(object)Interface != (Handle)null)
			{
				Debug.Log((object)"EasyAntiCheat Server Shutting Down");
				EndSessionOptions val = default(EndSessionOptions);
				Interface.EndSession(ref val);
				Interface = null;
				EOS.Shutdown();
			}
		}
		else
		{
			client2connection.Clear();
			connection2client.Clear();
			connection2status.Clear();
		}
	}

	public static void OnLeaveGame(Connection connection)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		AntiCheatCommonClientAuthStatus value3;
		if (Server.secure && !Application.isEditor)
		{
			if ((Handle)(object)Interface != (Handle)null)
			{
				IntPtr client = GetClient(connection);
				if (client != IntPtr.Zero)
				{
					UnregisterClientOptions val = default(UnregisterClientOptions);
					((UnregisterClientOptions)(ref val)).ClientHandle = client;
					UnregisterClientOptions val2 = val;
					Interface.UnregisterClient(ref val2);
					client2connection.TryRemove((uint)(int)client, out var _);
				}
				connection2client.TryRemove(connection, out var _);
				connection2status.TryRemove(connection, out value3);
			}
		}
		else
		{
			connection2status.TryRemove(connection, out value3);
		}
	}

	public static void OnJoinGame(Connection connection)
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Invalid comparison between Unknown and I4
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		if (Server.secure && !Application.isEditor)
		{
			if (!((Handle)(object)Interface != (Handle)null))
			{
				return;
			}
			IntPtr intPtr = GenerateCompatibilityClient();
			if (intPtr == IntPtr.Zero)
			{
				Debug.LogError((object)("[EAC] GenerateCompatibilityClient returned invalid client: " + intPtr));
				return;
			}
			RegisterClientOptions val = default(RegisterClientOptions);
			((RegisterClientOptions)(ref val)).ClientHandle = intPtr;
			((RegisterClientOptions)(ref val)).UserId = ProductUserId.FromString(Utf8String.op_Implicit(connection.anticheatId));
			((RegisterClientOptions)(ref val)).IpAddress = Utf8String.op_Implicit(connection.IPAddressWithoutPort());
			((RegisterClientOptions)(ref val)).ClientType = (AntiCheatCommonClientType)(connection.IsDevelopmentBuild() ? 1 : 0);
			((RegisterClientOptions)(ref val)).ClientPlatform = (AntiCheatCommonClientPlatform)((connection.os == "windows") ? 1 : ((connection.os == "linux") ? 3 : ((connection.os == "mac") ? 2 : 0)));
			RegisterClientOptions val2 = val;
			if ((int)((RegisterClientOptions)(ref val2)).ClientType == 1)
			{
				Debug.LogWarning((object)("[EAC] Joining game as unprotected client: " + ((object)connection).ToString()));
			}
			SetClientDetailsOptions val3 = default(SetClientDetailsOptions);
			((SetClientDetailsOptions)(ref val3)).ClientHandle = intPtr;
			((SetClientDetailsOptions)(ref val3)).ClientFlags = (AntiCheatCommonClientFlags)((connection.authLevel != 0) ? 1 : 0);
			SetClientDetailsOptions val4 = val3;
			Interface.RegisterClient(ref val2);
			Interface.SetClientDetails(ref val4);
			client2connection.TryAdd((uint)(int)intPtr, connection);
			connection2client.TryAdd(connection, (uint)(int)intPtr);
			connection2status.TryAdd(connection, (AntiCheatCommonClientAuthStatus)0);
		}
		else
		{
			connection2status.TryAdd(connection, (AntiCheatCommonClientAuthStatus)0);
			OnAuthenticatedLocal(connection);
			OnAuthenticatedRemote(connection);
		}
	}

	public static void OnStartLoading(Connection connection)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		if ((Handle)(object)Interface != (Handle)null)
		{
			IntPtr client = GetClient(connection);
			if (client != IntPtr.Zero)
			{
				SetClientNetworkStateOptions val = default(SetClientNetworkStateOptions);
				((SetClientNetworkStateOptions)(ref val)).ClientHandle = client;
				((SetClientNetworkStateOptions)(ref val)).IsNetworkActive = false;
				SetClientNetworkStateOptions val2 = val;
				Interface.SetClientNetworkState(ref val2);
			}
		}
	}

	public static void OnFinishLoading(Connection connection)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		if ((Handle)(object)Interface != (Handle)null)
		{
			IntPtr client = GetClient(connection);
			if (client != IntPtr.Zero)
			{
				SetClientNetworkStateOptions val = default(SetClientNetworkStateOptions);
				((SetClientNetworkStateOptions)(ref val)).ClientHandle = client;
				((SetClientNetworkStateOptions)(ref val)).IsNetworkActive = true;
				SetClientNetworkStateOptions val2 = val;
				Interface.SetClientNetworkState(ref val2);
			}
		}
	}

	public static void OnMessageReceived(Message message)
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		IntPtr client = GetClient(message.connection);
		byte[] array = default(byte[]);
		int count = default(int);
		if (client == IntPtr.Zero)
		{
			Debug.LogError((object)("EAC network packet from invalid connection: " + message.connection.userid));
		}
		else if (message.read.TemporaryBytesWithSize(ref array, ref count))
		{
			ReceiveMessageFromClientOptions val = default(ReceiveMessageFromClientOptions);
			((ReceiveMessageFromClientOptions)(ref val)).ClientHandle = client;
			((ReceiveMessageFromClientOptions)(ref val)).Data = new ArraySegment<byte>(array, 0, count);
			ReceiveMessageFromClientOptions val2 = val;
			Interface.ReceiveMessageFromClient(ref val2);
		}
	}

	public static void LogPlayerUseWeapon(BasePlayer player, BaseProjectile weapon)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		if (CanSendAnalytics && player.net.connection != null)
		{
			TimeWarning val = TimeWarning.New("EAC.LogPlayerShooting", 0);
			try
			{
				Vector3 networkPosition = player.GetNetworkPosition();
				Quaternion networkRotation = player.GetNetworkRotation();
				Item item = weapon.GetItem();
				string text = ((item != null) ? item.info.shortname : "unknown");
				LogPlayerUseWeaponOptions val2 = default(LogPlayerUseWeaponOptions);
				LogPlayerUseWeaponData value = default(LogPlayerUseWeaponData);
				((LogPlayerUseWeaponData)(ref value)).PlayerHandle = GetClient(player.net.connection);
				Vec3f value2 = default(Vec3f);
				((Vec3f)(ref value2)).x = networkPosition.x;
				((Vec3f)(ref value2)).y = networkPosition.y;
				((Vec3f)(ref value2)).z = networkPosition.z;
				((LogPlayerUseWeaponData)(ref value)).PlayerPosition = value2;
				Quat value3 = default(Quat);
				((Quat)(ref value3)).w = networkRotation.w;
				((Quat)(ref value3)).x = networkRotation.x;
				((Quat)(ref value3)).y = networkRotation.y;
				((Quat)(ref value3)).z = networkRotation.z;
				((LogPlayerUseWeaponData)(ref value)).PlayerViewRotation = value3;
				((LogPlayerUseWeaponData)(ref value)).WeaponName = Utf8String.op_Implicit(text);
				((LogPlayerUseWeaponOptions)(ref val2)).UseWeaponData = value;
				Interface.LogPlayerUseWeapon(ref val2);
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
	}

	public static void LogPlayerSpawn(BasePlayer player)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		if (CanSendAnalytics && player.net.connection != null)
		{
			TimeWarning val = TimeWarning.New("EAC.LogPlayerSpawn", 0);
			try
			{
				LogPlayerSpawnOptions val2 = default(LogPlayerSpawnOptions);
				((LogPlayerSpawnOptions)(ref val2)).SpawnedPlayerHandle = GetClient(player.net.connection);
				Interface.LogPlayerSpawn(ref val2);
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
	}

	public static void LogPlayerDespawn(BasePlayer player)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		if (CanSendAnalytics && player.net.connection != null)
		{
			TimeWarning val = TimeWarning.New("EAC.LogPlayerDespawn", 0);
			try
			{
				LogPlayerDespawnOptions val2 = default(LogPlayerDespawnOptions);
				((LogPlayerDespawnOptions)(ref val2)).DespawnedPlayerHandle = GetClient(player.net.connection);
				Interface.LogPlayerDespawn(ref val2);
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
	}

	public static void LogPlayerTakeDamage(BasePlayer player, HitInfo info)
	{
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		if (!CanSendAnalytics || !((Object)(object)info.Initiator != (Object)null) || !(info.Initiator is BasePlayer))
		{
			return;
		}
		BasePlayer basePlayer = info.Initiator.ToPlayer();
		if (player.net.connection == null || basePlayer.net.connection == null)
		{
			return;
		}
		TimeWarning val = TimeWarning.New("EAC.LogPlayerTakeDamage", 0);
		try
		{
			LogPlayerTakeDamageOptions val2 = default(LogPlayerTakeDamageOptions);
			LogPlayerUseWeaponData value = default(LogPlayerUseWeaponData);
			((LogPlayerTakeDamageOptions)(ref val2)).AttackerPlayerHandle = GetClient(basePlayer.net.connection);
			((LogPlayerTakeDamageOptions)(ref val2)).VictimPlayerHandle = GetClient(player.net.connection);
			((LogPlayerTakeDamageOptions)(ref val2)).DamageTaken = info.damageTypes.Total();
			Vec3f value2 = default(Vec3f);
			((Vec3f)(ref value2)).x = info.HitPositionWorld.x;
			((Vec3f)(ref value2)).y = info.HitPositionWorld.y;
			((Vec3f)(ref value2)).z = info.HitPositionWorld.z;
			((LogPlayerTakeDamageOptions)(ref val2)).DamagePosition = value2;
			((LogPlayerTakeDamageOptions)(ref val2)).IsCriticalHit = info.isHeadshot;
			if (player.IsDead())
			{
				((LogPlayerTakeDamageOptions)(ref val2)).DamageResult = (AntiCheatCommonPlayerTakeDamageResult)2;
			}
			else if (player.IsWounded())
			{
				((LogPlayerTakeDamageOptions)(ref val2)).DamageResult = (AntiCheatCommonPlayerTakeDamageResult)1;
			}
			if ((Object)(object)info.Weapon != (Object)null)
			{
				Item item = info.Weapon.GetItem();
				if (item != null)
				{
					((LogPlayerUseWeaponData)(ref value)).WeaponName = Utf8String.op_Implicit(item.info.shortname);
				}
				else
				{
					((LogPlayerUseWeaponData)(ref value)).WeaponName = Utf8String.op_Implicit("unknown");
				}
			}
			else
			{
				((LogPlayerUseWeaponData)(ref value)).WeaponName = Utf8String.op_Implicit("unknown");
			}
			Vector3 position = basePlayer.eyes.position;
			Quaternion rotation = basePlayer.eyes.rotation;
			Vector3 position2 = player.eyes.position;
			Quaternion rotation2 = player.eyes.rotation;
			value2 = default(Vec3f);
			((Vec3f)(ref value2)).x = position.x;
			((Vec3f)(ref value2)).y = position.y;
			((Vec3f)(ref value2)).z = position.z;
			((LogPlayerTakeDamageOptions)(ref val2)).AttackerPlayerPosition = value2;
			Quat value3 = default(Quat);
			((Quat)(ref value3)).w = rotation.w;
			((Quat)(ref value3)).x = rotation.x;
			((Quat)(ref value3)).y = rotation.y;
			((Quat)(ref value3)).z = rotation.z;
			((LogPlayerTakeDamageOptions)(ref val2)).AttackerPlayerViewRotation = value3;
			value2 = default(Vec3f);
			((Vec3f)(ref value2)).x = position2.x;
			((Vec3f)(ref value2)).y = position2.y;
			((Vec3f)(ref value2)).z = position2.z;
			((LogPlayerTakeDamageOptions)(ref val2)).VictimPlayerPosition = value2;
			value3 = default(Quat);
			((Quat)(ref value3)).w = rotation2.w;
			((Quat)(ref value3)).x = rotation2.x;
			((Quat)(ref value3)).y = rotation2.y;
			((Quat)(ref value3)).z = rotation2.z;
			((LogPlayerTakeDamageOptions)(ref val2)).VictimPlayerViewRotation = value3;
			((LogPlayerTakeDamageOptions)(ref val2)).PlayerUseWeaponData = value;
			Interface.LogPlayerTakeDamage(ref val2);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public static void LogPlayerTick(BasePlayer player)
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		if (!CanSendAnalytics || player.net == null || player.net.connection == null)
		{
			return;
		}
		TimeWarning val = TimeWarning.New("EAC.LogPlayerTick", 0);
		try
		{
			Vector3 position = player.eyes.position;
			Quaternion rotation = player.eyes.rotation;
			LogPlayerTickOptions val2 = default(LogPlayerTickOptions);
			((LogPlayerTickOptions)(ref val2)).PlayerHandle = GetClient(player.net.connection);
			Vec3f value = default(Vec3f);
			((Vec3f)(ref value)).x = position.x;
			((Vec3f)(ref value)).y = position.y;
			((Vec3f)(ref value)).z = position.z;
			((LogPlayerTickOptions)(ref val2)).PlayerPosition = value;
			Quat value2 = default(Quat);
			((Quat)(ref value2)).w = rotation.w;
			((Quat)(ref value2)).x = rotation.x;
			((Quat)(ref value2)).y = rotation.y;
			((Quat)(ref value2)).z = rotation.z;
			((LogPlayerTickOptions)(ref val2)).PlayerViewRotation = value2;
			((LogPlayerTickOptions)(ref val2)).PlayerHealth = player.Health();
			if (player.IsDucked())
			{
				((LogPlayerTickOptions)(ref val2)).PlayerMovementState = (AntiCheatCommonPlayerMovementState)(((LogPlayerTickOptions)(ref val2)).PlayerMovementState | 1);
			}
			if (player.isMounted)
			{
				((LogPlayerTickOptions)(ref val2)).PlayerMovementState = (AntiCheatCommonPlayerMovementState)(((LogPlayerTickOptions)(ref val2)).PlayerMovementState | 3);
			}
			if (player.IsCrawling())
			{
				((LogPlayerTickOptions)(ref val2)).PlayerMovementState = (AntiCheatCommonPlayerMovementState)(((LogPlayerTickOptions)(ref val2)).PlayerMovementState | 2);
			}
			if (player.IsSwimming())
			{
				((LogPlayerTickOptions)(ref val2)).PlayerMovementState = (AntiCheatCommonPlayerMovementState)(((LogPlayerTickOptions)(ref val2)).PlayerMovementState | 4);
			}
			if (!player.IsOnGround())
			{
				((LogPlayerTickOptions)(ref val2)).PlayerMovementState = (AntiCheatCommonPlayerMovementState)(((LogPlayerTickOptions)(ref val2)).PlayerMovementState | 5);
			}
			if (player.OnLadder())
			{
				((LogPlayerTickOptions)(ref val2)).PlayerMovementState = (AntiCheatCommonPlayerMovementState)(((LogPlayerTickOptions)(ref val2)).PlayerMovementState | 7);
			}
			if (player.IsFlying)
			{
				((LogPlayerTickOptions)(ref val2)).PlayerMovementState = (AntiCheatCommonPlayerMovementState)(((LogPlayerTickOptions)(ref val2)).PlayerMovementState | 6);
			}
			Interface.LogPlayerTick(ref val2);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public static void LogPlayerRevive(BasePlayer source, BasePlayer target)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		if (CanSendAnalytics && target.net.connection != null && (Object)(object)source != (Object)null && source.net.connection != null)
		{
			TimeWarning val = TimeWarning.New("EAC.LogPlayerRevive", 0);
			try
			{
				LogPlayerReviveOptions val2 = default(LogPlayerReviveOptions);
				((LogPlayerReviveOptions)(ref val2)).RevivedPlayerHandle = GetClient(target.net.connection);
				((LogPlayerReviveOptions)(ref val2)).ReviverPlayerHandle = GetClient(source.net.connection);
				Interface.LogPlayerRevive(ref val2);
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
	}

	public static void SendPlayerBehaviorReport(BasePlayer reporter, PlayerReportsCategory reportCategory, string reportedID, string reportText)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		if (CanSendReports)
		{
			SendPlayerBehaviorReportOptions val = default(SendPlayerBehaviorReportOptions);
			((SendPlayerBehaviorReportOptions)(ref val)).ReportedUserId = ProductUserId.FromString(Utf8String.op_Implicit(reportedID));
			((SendPlayerBehaviorReportOptions)(ref val)).ReporterUserId = ProductUserId.FromString(Utf8String.op_Implicit(reporter.UserIDString));
			((SendPlayerBehaviorReportOptions)(ref val)).Category = reportCategory;
			((SendPlayerBehaviorReportOptions)(ref val)).Message = Utf8String.op_Implicit(reportText);
			SendPlayerBehaviorReportOptions val2 = val;
			Reports.SendPlayerBehaviorReport(ref val2, (object)null, (OnSendPlayerBehaviorReportCompleteCallback)null);
		}
	}

	public static void SendPlayerBehaviorReport(PlayerReportsCategory reportCategory, string reportedID, string reportText)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		if (CanSendReports)
		{
			SendPlayerBehaviorReportOptions val = default(SendPlayerBehaviorReportOptions);
			((SendPlayerBehaviorReportOptions)(ref val)).ReportedUserId = ProductUserId.FromString(Utf8String.op_Implicit(reportedID));
			((SendPlayerBehaviorReportOptions)(ref val)).Category = reportCategory;
			((SendPlayerBehaviorReportOptions)(ref val)).Message = Utf8String.op_Implicit(reportText);
			SendPlayerBehaviorReportOptions val2 = val;
			Reports.SendPlayerBehaviorReport(ref val2, (object)null, (OnSendPlayerBehaviorReportCompleteCallback)null);
		}
	}
}
