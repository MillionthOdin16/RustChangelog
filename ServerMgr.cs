using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using CompanionServer;
using ConVar;
using Facepunch;
using Facepunch.Math;
using Facepunch.Models;
using Facepunch.Network;
using Facepunch.Rust;
using Ionic.Crc;
using Network;
using Network.Visibility;
using ProtoBuf;
using Rust;
using Steamworks;
using UnityEngine;
using UnityEngine.Profiling;

public class ServerMgr : SingletonComponent<ServerMgr>, IServerCallback
{
	public ConnectionQueue connectionQueue = new ConnectionQueue();

	public TimeAverageValueLookup<Type> packetHistory = new TimeAverageValueLookup<Type>();

	public TimeAverageValueLookup<uint> rpcHistory = new TimeAverageValueLookup<uint>();

	public const string BYPASS_PROCEDURAL_SPAWN_PREF = "bypassProceduralSpawn";

	private ConnectionAuth auth;

	public UserPersistance persistance;

	public PlayerStateManager playerStateManager;

	private AIThinkManager.QueueType aiTick = AIThinkManager.QueueType.Human;

	private List<ulong> bannedPlayerNotices = new List<ulong>();

	private string _AssemblyHash = null;

	private IEnumerator restartCoroutine = null;

	public bool runFrameUpdate { get; private set; }

	public static int AvailableSlots => ConVar.Server.maxplayers - BasePlayer.activePlayerList.Count;

	private string AssemblyHash
	{
		get
		{
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Expected O, but got Unknown
			if (_AssemblyHash == null)
			{
				Assembly assembly = typeof(ServerMgr).Assembly;
				string location = assembly.Location;
				if (!string.IsNullOrEmpty(location))
				{
					byte[] array = File.ReadAllBytes(location);
					CRC32 val = new CRC32();
					val.SlurpBlock(array, 0, array.Length);
					_AssemblyHash = val.Crc32Result.ToString("x");
				}
				else
				{
					_AssemblyHash = "il2cpp";
				}
			}
			return _AssemblyHash;
		}
	}

	public bool Restarting => restartCoroutine != null;

	private void Log(Exception e)
	{
		if (Global.developer > 0)
		{
			Debug.LogException(e);
		}
	}

	public void OnNetworkMessage(Message packet)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Invalid comparison between Unknown and I4
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_036a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0442: Unknown result type (might be due to invalid IL or missing references)
		//IL_050d: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Expected I4, but got Unknown
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_039b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0473: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Expected I4, but got Unknown
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_065f: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a7: Unknown result type (might be due to invalid IL or missing references)
		if (ConVar.Server.packetlog_enabled)
		{
			packetHistory.Increment(packet.type);
		}
		Type type = packet.type;
		if ((int)type != 4)
		{
			switch (type - 9)
			{
			default:
				switch (type - 18)
				{
				case 0:
				{
					if (packet.connection.GetPacketsPerSecond(packet.type) >= 1)
					{
						Net.sv.Kick(packet.connection, "Packet Flooding: User Information", packet.connection.connected);
						return;
					}
					TimeWarning val4 = TimeWarning.New("GiveUserInformation", 20);
					try
					{
						OnGiveUserInformation(packet);
					}
					catch (Exception e4)
					{
						Log(e4);
						Net.sv.Kick(packet.connection, "Invalid Packet: User Information", false);
					}
					finally
					{
						((IDisposable)val4)?.Dispose();
					}
					packet.connection.AddPacketsPerSecond(packet.type);
					return;
				}
				case 4:
				{
					TimeWarning val6 = TimeWarning.New("OnEACMessage", 20);
					try
					{
						EACServer.OnMessageReceived(packet);
						return;
					}
					catch (Exception e6)
					{
						Log(e6);
						Net.sv.Kick(packet.connection, "Invalid Packet: EAC", false);
						return;
					}
					finally
					{
						((IDisposable)val6)?.Dispose();
					}
				}
				case 6:
				{
					if (!World.Transfer || !packet.connection.connected)
					{
						return;
					}
					if (packet.connection.GetPacketsPerSecond(packet.type) >= (ulong)ConVar.Server.maxpacketspersecond_world)
					{
						Net.sv.Kick(packet.connection, "Packet Flooding: World", packet.connection.connected);
						return;
					}
					TimeWarning val5 = TimeWarning.New("OnWorldMessage", 20);
					try
					{
						WorldNetworking.OnMessageReceived(packet);
						return;
					}
					catch (Exception e5)
					{
						Log(e5);
						Net.sv.Kick(packet.connection, "Invalid Packet: World", false);
						return;
					}
					finally
					{
						((IDisposable)val5)?.Dispose();
					}
				}
				case 3:
				{
					if (!packet.connection.connected)
					{
						return;
					}
					if (packet.connection.GetPacketsPerSecond(packet.type) >= (ulong)ConVar.Server.maxpacketspersecond_voice)
					{
						Net.sv.Kick(packet.connection, "Packet Flooding: Disconnect Reason", packet.connection.connected);
						return;
					}
					TimeWarning val3 = TimeWarning.New("OnPlayerVoice", 20);
					try
					{
						OnPlayerVoice(packet);
					}
					catch (Exception e3)
					{
						Log(e3);
						Net.sv.Kick(packet.connection, "Invalid Packet: Player Voice", false);
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
					packet.connection.AddPacketsPerSecond(packet.type);
					return;
				}
				}
				break;
			case 0:
			{
				if (!packet.connection.connected)
				{
					return;
				}
				if (packet.connection.GetPacketsPerSecond(packet.type) >= (ulong)ConVar.Server.maxpacketspersecond_rpc)
				{
					Net.sv.Kick(packet.connection, "Packet Flooding: RPC Message", false);
					return;
				}
				TimeWarning val8 = TimeWarning.New("OnRPCMessage", 20);
				try
				{
					OnRPCMessage(packet);
				}
				catch (Exception e8)
				{
					Log(e8);
					Net.sv.Kick(packet.connection, "Invalid Packet: RPC Message", false);
				}
				finally
				{
					((IDisposable)val8)?.Dispose();
				}
				packet.connection.AddPacketsPerSecond(packet.type);
				return;
			}
			case 3:
			{
				if (!packet.connection.connected)
				{
					return;
				}
				if (packet.connection.GetPacketsPerSecond(packet.type) >= (ulong)ConVar.Server.maxpacketspersecond_command)
				{
					Net.sv.Kick(packet.connection, "Packet Flooding: Client Command", packet.connection.connected);
					return;
				}
				TimeWarning val7 = TimeWarning.New("OnClientCommand", 20);
				try
				{
					ConsoleNetwork.OnClientCommand(packet);
				}
				catch (Exception e7)
				{
					Log(e7);
					Net.sv.Kick(packet.connection, "Invalid Packet: Client Command", false);
				}
				finally
				{
					((IDisposable)val7)?.Dispose();
				}
				packet.connection.AddPacketsPerSecond(packet.type);
				return;
			}
			case 5:
			{
				if (!packet.connection.connected)
				{
					return;
				}
				if (packet.connection.GetPacketsPerSecond(packet.type) >= 1)
				{
					Net.sv.Kick(packet.connection, "Packet Flooding: Disconnect Reason", packet.connection.connected);
					return;
				}
				TimeWarning val2 = TimeWarning.New("ReadDisconnectReason", 20);
				try
				{
					ReadDisconnectReason(packet);
					Net.sv.Disconnect(packet.connection);
				}
				catch (Exception e2)
				{
					Log(e2);
					Net.sv.Kick(packet.connection, "Invalid Packet: Disconnect Reason", false);
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				packet.connection.AddPacketsPerSecond(packet.type);
				return;
			}
			case 6:
			{
				if (!packet.connection.connected)
				{
					return;
				}
				if (packet.connection.GetPacketsPerSecond(packet.type) >= (ulong)ConVar.Server.maxpacketspersecond_tick)
				{
					Net.sv.Kick(packet.connection, "Packet Flooding: Player Tick", packet.connection.connected);
					return;
				}
				TimeWarning val = TimeWarning.New("OnPlayerTick", 20);
				try
				{
					OnPlayerTick(packet);
				}
				catch (Exception e)
				{
					Log(e);
					Net.sv.Kick(packet.connection, "Invalid Packet: Player Tick", false);
				}
				finally
				{
					((IDisposable)val)?.Dispose();
				}
				packet.connection.AddPacketsPerSecond(packet.type);
				return;
			}
			case 1:
			case 2:
			case 4:
				break;
			}
			ProcessUnhandledPacket(packet);
		}
		else
		{
			if (!packet.connection.connected)
			{
				return;
			}
			if (packet.connection.GetPacketsPerSecond(packet.type) >= 1)
			{
				Net.sv.Kick(packet.connection, "Packet Flooding: Client Ready", packet.connection.connected);
				return;
			}
			TimeWarning val9 = TimeWarning.New("ClientReady", 20);
			try
			{
				ClientReady(packet);
			}
			catch (Exception e9)
			{
				Log(e9);
				Net.sv.Kick(packet.connection, "Invalid Packet: Client Ready", false);
			}
			finally
			{
				((IDisposable)val9)?.Dispose();
			}
			packet.connection.AddPacketsPerSecond(packet.type);
		}
	}

	public void ProcessUnhandledPacket(Message packet)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		if (Global.developer > 0)
		{
			Debug.LogWarning((object)("[SERVER][UNHANDLED] " + packet.type));
		}
		Net.sv.Kick(packet.connection, "Sent Unhandled Message", false);
	}

	public void ReadDisconnectReason(Message packet)
	{
		string text = packet.read.String(4096);
		string text2 = ((object)packet.connection).ToString();
		if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(text2))
		{
			DebugEx.Log((object)(text2 + " disconnecting: " + text), (StackTraceLogType)0);
		}
	}

	private BasePlayer SpawnPlayerSleeping(Connection connection)
	{
		BasePlayer basePlayer = BasePlayer.FindSleeping(connection.userid);
		if ((Object)(object)basePlayer == (Object)null)
		{
			return null;
		}
		if (!basePlayer.IsSleeping())
		{
			Debug.LogWarning((object)"Player spawning into sleeper that isn't sleeping!");
			basePlayer.Kill();
			return null;
		}
		basePlayer.PlayerInit(connection);
		basePlayer.inventory.SendSnapshot();
		DebugEx.Log((object)(((object)basePlayer.net.connection).ToString() + " joined [" + basePlayer.net.connection.os + "/" + basePlayer.net.connection.ownerid + "]"), (StackTraceLogType)0);
		return basePlayer;
	}

	private BasePlayer SpawnNewPlayer(Connection connection)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		BasePlayer.SpawnPoint spawnPoint = FindSpawnPoint();
		BaseEntity baseEntity = GameManager.server.CreateEntity("assets/prefabs/player/player.prefab", spawnPoint.pos, spawnPoint.rot);
		BasePlayer basePlayer = baseEntity.ToPlayer();
		basePlayer.health = 0f;
		basePlayer.lifestate = BaseCombatEntity.LifeState.Dead;
		basePlayer.ResetLifeStateOnSpawn = false;
		basePlayer.limitNetworking = true;
		basePlayer.Spawn();
		basePlayer.limitNetworking = false;
		basePlayer.PlayerInit(connection);
		if (Object.op_Implicit((Object)(object)BaseGameMode.GetActiveGameMode(serverside: true)))
		{
			BaseGameMode.GetActiveGameMode(serverside: true).OnNewPlayer(basePlayer);
		}
		else if (Application.isEditor || (SleepingBag.FindForPlayer(basePlayer.userID, ignoreTimers: true).Length == 0 && !basePlayer.hasPreviousLife))
		{
			basePlayer.Respawn();
		}
		DebugEx.Log((object)$"{basePlayer.displayName} with steamid {basePlayer.userID} joined from ip {basePlayer.net.connection.ipaddress}", (StackTraceLogType)0);
		DebugEx.Log((object)$"\tNetworkId {basePlayer.userID} is {basePlayer.net.ID} ({basePlayer.displayName})", (StackTraceLogType)0);
		if (basePlayer.net.connection.ownerid != basePlayer.net.connection.userid)
		{
			DebugEx.Log((object)$"\t{basePlayer} is sharing the account {basePlayer.net.connection.ownerid}", (StackTraceLogType)0);
		}
		return basePlayer;
	}

	private void ClientReady(Message packet)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Invalid comparison between Unknown and I4
		if ((int)packet.connection.state != 3)
		{
			Net.sv.Kick(packet.connection, "Invalid connection state", false);
			return;
		}
		ClientReady val = ClientReady.Deserialize((Stream)(object)packet.read);
		try
		{
			foreach (ClientInfo item in val.clientInfo)
			{
				packet.connection.info.Set(item.name, item.value);
			}
			connectionQueue.JoinedGame(packet.connection);
			Analytics.Azure.OnPlayerConnected(packet.connection);
			TimeWarning val2 = TimeWarning.New("ClientReady", 0);
			try
			{
				TimeWarning val3 = TimeWarning.New("SpawnPlayerSleeping", 0);
				BasePlayer basePlayer;
				try
				{
					basePlayer = SpawnPlayerSleeping(packet.connection);
				}
				finally
				{
					((IDisposable)val3)?.Dispose();
				}
				if ((Object)(object)basePlayer == (Object)null)
				{
					TimeWarning val4 = TimeWarning.New("SpawnNewPlayer", 0);
					try
					{
						basePlayer = SpawnNewPlayer(packet.connection);
					}
					finally
					{
						((IDisposable)val4)?.Dispose();
					}
				}
				basePlayer.SendRespawnOptions();
				if ((Object)(object)basePlayer != (Object)null)
				{
					Util.SendSignedInNotification(basePlayer);
				}
			}
			finally
			{
				((IDisposable)val2)?.Dispose();
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		SendReplicatedVars(packet.connection);
	}

	private void OnRPCMessage(Message packet)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		NetworkableId uid = packet.read.EntityID();
		uint num = packet.read.UInt32();
		if (ConVar.Server.rpclog_enabled)
		{
			rpcHistory.Increment(num);
		}
		Profiler.BeginSample("LookupEntity");
		BaseEntity baseEntity = BaseNetworkable.serverEntities.Find(uid) as BaseEntity;
		Profiler.EndSample();
		if (!((Object)(object)baseEntity == (Object)null))
		{
			Profiler.BeginSample("SV_RPCMessage");
			baseEntity.SV_RPCMessage(num, packet);
			Profiler.EndSample();
		}
	}

	private void OnPlayerTick(Message packet)
	{
		BasePlayer basePlayer = packet.Player();
		if (!((Object)(object)basePlayer == (Object)null))
		{
			basePlayer.OnReceivedTick((Stream)(object)packet.read);
		}
	}

	private void OnPlayerVoice(Message packet)
	{
		BasePlayer basePlayer = packet.Player();
		if (!((Object)(object)basePlayer == (Object)null))
		{
			basePlayer.OnReceivedVoice(packet.read.BytesWithSize(10485760u));
		}
	}

	private void OnGiveUserInformation(Message packet)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Invalid comparison between Unknown and I4
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		if ((int)packet.connection.state > 0)
		{
			Net.sv.Kick(packet.connection, "Invalid connection state", false);
			return;
		}
		packet.connection.state = (State)1;
		byte b = packet.read.UInt8();
		if (b != 228)
		{
			Net.sv.Kick(packet.connection, "Invalid Connection Protocol", false);
			return;
		}
		packet.connection.userid = packet.read.UInt64();
		packet.connection.protocol = packet.read.UInt32();
		packet.connection.os = packet.read.String(128);
		packet.connection.username = packet.read.String(256);
		if (string.IsNullOrEmpty(packet.connection.os))
		{
			throw new Exception("Invalid OS");
		}
		if (string.IsNullOrEmpty(packet.connection.username))
		{
			Net.sv.Kick(packet.connection, "Invalid Username", false);
			return;
		}
		packet.connection.username = packet.connection.username.Replace('\n', ' ').Replace('\r', ' ').Replace('\t', ' ')
			.Trim();
		if (string.IsNullOrEmpty(packet.connection.username))
		{
			Net.sv.Kick(packet.connection, "Invalid Username", false);
			return;
		}
		string text = string.Empty;
		string branch = ConVar.Server.branch;
		if (packet.read.Unread >= 4)
		{
			text = packet.read.String(128);
		}
		if (branch != string.Empty && branch != text)
		{
			DebugEx.Log((object)string.Concat("Kicking ", packet.connection, " - their branch is '", text, "' not '", branch, "'"), (StackTraceLogType)0);
			Net.sv.Kick(packet.connection, "Wrong Steam Beta: Requires '" + branch + "' branch!", false);
		}
		else if (packet.connection.protocol > 2402)
		{
			DebugEx.Log((object)string.Concat("Kicking ", packet.connection, " - their protocol is ", packet.connection.protocol, " not ", 2402), (StackTraceLogType)0);
			Net.sv.Kick(packet.connection, "Wrong Connection Protocol: Server update required!", false);
		}
		else if (packet.connection.protocol < 2402)
		{
			DebugEx.Log((object)string.Concat("Kicking ", packet.connection, " - their protocol is ", packet.connection.protocol, " not ", 2402), (StackTraceLogType)0);
			Net.sv.Kick(packet.connection, "Wrong Connection Protocol: Client update required!", false);
		}
		else
		{
			packet.connection.token = packet.read.BytesWithSize(512u);
			if (packet.connection.token == null || packet.connection.token.Length < 1)
			{
				Net.sv.Kick(packet.connection, "Invalid Token", false);
			}
			else
			{
				auth.OnNewConnection(packet.connection);
			}
		}
	}

	public void Initialize(bool loadSave = true, string saveFile = "", bool allowOutOfDateSaves = false, bool skipInitialSpawn = false)
	{
		persistance = new UserPersistance(ConVar.Server.rootFolder);
		playerStateManager = new PlayerStateManager(persistance);
		Profiler.BeginSample("SpawnMapEntities");
		SpawnMapEntities();
		Profiler.EndSample();
		if (Object.op_Implicit((Object)(object)SingletonComponent<SpawnHandler>.Instance))
		{
			TimeWarning val = TimeWarning.New("SpawnHandler.UpdateDistributions", 0);
			try
			{
				SingletonComponent<SpawnHandler>.Instance.UpdateDistributions();
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
		if (loadSave)
		{
			World.LoadedFromSave = true;
			World.LoadedFromSave = (skipInitialSpawn = SaveRestore.Load(saveFile, allowOutOfDateSaves));
		}
		else
		{
			SaveRestore.SaveCreatedTime = DateTime.UtcNow;
			World.LoadedFromSave = false;
		}
		SaveRestore.InitializeWipeId();
		if (Object.op_Implicit((Object)(object)SingletonComponent<SpawnHandler>.Instance))
		{
			if (!skipInitialSpawn)
			{
				TimeWarning val2 = TimeWarning.New("SpawnHandler.InitialSpawn", 200);
				try
				{
					SingletonComponent<SpawnHandler>.Instance.InitialSpawn();
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
			}
			TimeWarning val3 = TimeWarning.New("SpawnHandler.StartSpawnTick", 200);
			try
			{
				SingletonComponent<SpawnHandler>.Instance.StartSpawnTick();
			}
			finally
			{
				((IDisposable)val3)?.Dispose();
			}
		}
		Profiler.BeginSample("CreateImportantEntities");
		CreateImportantEntities();
		Profiler.EndSample();
		auth = ((Component)this).GetComponent<ConnectionAuth>();
		Analytics.Azure.Initialize();
	}

	public void OpenConnection()
	{
		if (ConVar.Server.queryport <= 0 || ConVar.Server.queryport == ConVar.Server.port)
		{
			int num = Math.Max(ConVar.Server.port, RCon.Port);
			ConVar.Server.queryport = num + 1;
		}
		Net.sv.ip = ConVar.Server.ip;
		Net.sv.port = ConVar.Server.port;
		Profiler.BeginSample("StartSteamServer");
		StartSteamServer();
		Profiler.EndSample();
		if (!Net.sv.Start())
		{
			Debug.LogWarning((object)"Couldn't Start Server.");
			CloseConnection();
			return;
		}
		Net.sv.callbackHandler = (IServerCallback)(object)this;
		((BaseNetwork)Net.sv).cryptography = (INetworkCryptography)(object)new NetworkCryptographyServer();
		EACServer.DoStartup();
		((MonoBehaviour)this).InvokeRepeating("DoTick", 1f, 1f / (float)ConVar.Server.tickrate);
		((MonoBehaviour)this).InvokeRepeating("DoHeartbeat", 1f, 1f);
		runFrameUpdate = true;
		ConsoleSystem.OnReplicatedVarChanged += OnReplicatedVarChanged;
	}

	private void CloseConnection()
	{
		if (persistance != null)
		{
			persistance.Dispose();
			persistance = null;
		}
		EACServer.DoShutdown();
		Net.sv.callbackHandler = null;
		TimeWarning val = TimeWarning.New("sv.Stop", 0);
		try
		{
			Net.sv.Stop("Shutting Down");
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		TimeWarning val2 = TimeWarning.New("RCon.Shutdown", 0);
		try
		{
			RCon.Shutdown();
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
		TimeWarning val3 = TimeWarning.New("PlatformService.Shutdown", 0);
		try
		{
			IPlatformService instance = PlatformService.Instance;
			if (instance != null)
			{
				instance.Shutdown();
			}
		}
		finally
		{
			((IDisposable)val3)?.Dispose();
		}
		TimeWarning val4 = TimeWarning.New("CompanionServer.Shutdown", 0);
		try
		{
			CompanionServer.Server.Shutdown();
		}
		finally
		{
			((IDisposable)val4)?.Dispose();
		}
		ConsoleSystem.OnReplicatedVarChanged -= OnReplicatedVarChanged;
	}

	private void OnDisable()
	{
		if (!Application.isQuitting)
		{
			CloseConnection();
		}
	}

	private void OnApplicationQuit()
	{
		Application.isQuitting = true;
		CloseConnection();
	}

	private void CreateImportantEntities()
	{
		CreateImportantEntity<EnvSync>("assets/bundled/prefabs/system/net_env.prefab");
		CreateImportantEntity<CommunityEntity>("assets/bundled/prefabs/system/server/community.prefab");
		CreateImportantEntity<ResourceDepositManager>("assets/bundled/prefabs/system/server/resourcedepositmanager.prefab");
		CreateImportantEntity<RelationshipManager>("assets/bundled/prefabs/system/server/relationship_manager.prefab");
		CreateImportantEntity<TreeManager>("assets/bundled/prefabs/system/tree_manager.prefab");
	}

	public void CreateImportantEntity<T>(string prefabName) where T : BaseEntity
	{
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		if (!BaseNetworkable.serverEntities.Any((BaseNetworkable x) => x is T))
		{
			Debug.LogWarning((object)("Missing " + typeof(T).Name + " - creating"));
			BaseEntity baseEntity = GameManager.server.CreateEntity(prefabName);
			if ((Object)(object)baseEntity == (Object)null)
			{
				Debug.LogWarning((object)"Couldn't create");
			}
			else
			{
				baseEntity.Spawn();
			}
		}
	}

	private void StartSteamServer()
	{
		PlatformService.Instance.Initialize((IPlatformHooks)(object)RustPlatformHooks.Instance);
		((MonoBehaviour)this).InvokeRepeating("UpdateServerInformation", 2f, 30f);
		((MonoBehaviour)this).InvokeRepeating("UpdateItemDefinitions", 10f, 3600f);
		DebugEx.Log((object)"SteamServer Initialized", (StackTraceLogType)0);
	}

	private void UpdateItemDefinitions()
	{
		Debug.Log((object)"Checking for new Steam Item Definitions..");
		PlatformService.Instance.RefreshItemDefinitions();
	}

	internal void OnValidateAuthTicketResponse(ulong SteamId, ulong OwnerId, AuthResponse Status)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Invalid comparison between Unknown and I4
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Invalid comparison between Unknown and I4
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Invalid comparison between Unknown and I4
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Invalid comparison between Unknown and I4
		if (Auth_Steam.ValidateConnecting(SteamId, OwnerId, Status))
		{
			return;
		}
		Connection val = Net.sv.connections.FirstOrDefault((Connection x) => x.userid == SteamId);
		if (val == null)
		{
			Debug.LogWarning((object)$"Steam gave us a {Status} ticket response for unconnected id {SteamId}");
		}
		else if ((int)Status == 2)
		{
			Debug.LogWarning((object)$"Steam gave us a 'ok' ticket response for already connected id {SteamId}");
		}
		else if ((int)Status != 1)
		{
			if (((int)Status == 4 || (int)Status == 3) && !bannedPlayerNotices.Contains(SteamId))
			{
				ConsoleNetwork.BroadcastToAllClients("chat.add", 2, 0, "<color=#fff>SERVER</color> Kicking " + StringEx.EscapeRichText(val.username) + " (banned by anticheat)");
				bannedPlayerNotices.Add(SteamId);
			}
			Debug.Log((object)$"Kicking {val.ipaddress}/{val.userid}/{val.username} (Steam Status \"{((object)(AuthResponse)(ref Status)).ToString()}\")");
			val.authStatus = ((object)(AuthResponse)(ref Status)).ToString();
			Net.sv.Kick(val, "Steam: " + ((object)(AuthResponse)(ref Status)).ToString(), false);
		}
	}

	private void Update()
	{
		if (!runFrameUpdate)
		{
			return;
		}
		Manifest manifest = Application.Manifest;
		if (manifest != null && manifest.Features.ServerAnalytics)
		{
			try
			{
				PerformanceLogging.server.OnFrame();
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}
		TimeWarning val = TimeWarning.New("ServerMgr.Update", 500);
		try
		{
			try
			{
				TimeWarning val2 = TimeWarning.New("EACServer.DoUpdate", 100);
				try
				{
					EACServer.DoUpdate();
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
			}
			catch (Exception ex2)
			{
				Debug.LogWarning((object)"Server Exception: EACServer.DoUpdate");
				Debug.LogException(ex2, (Object)(object)this);
			}
			try
			{
				TimeWarning val3 = TimeWarning.New("PlatformService.Update", 100);
				try
				{
					PlatformService.Instance.Update();
				}
				finally
				{
					((IDisposable)val3)?.Dispose();
				}
			}
			catch (Exception ex3)
			{
				Debug.LogWarning((object)"Server Exception: Platform Service Update");
				Debug.LogException(ex3, (Object)(object)this);
			}
			try
			{
				TimeWarning val4 = TimeWarning.New("Net.sv.Cycle", 100);
				try
				{
					((BaseNetwork)Net.sv).Cycle();
				}
				finally
				{
					((IDisposable)val4)?.Dispose();
				}
			}
			catch (Exception ex4)
			{
				Debug.LogWarning((object)"Server Exception: Network Cycle");
				Debug.LogException(ex4, (Object)(object)this);
			}
			try
			{
				TimeWarning val5 = TimeWarning.New("ServerBuildingManager.Cycle", 0);
				try
				{
					BuildingManager.server.Cycle();
				}
				finally
				{
					((IDisposable)val5)?.Dispose();
				}
			}
			catch (Exception ex5)
			{
				Debug.LogWarning((object)"Server Exception: Building Manager");
				Debug.LogException(ex5, (Object)(object)this);
			}
			try
			{
				TimeWarning val6 = TimeWarning.New("BasePlayer.ServerCycle", 0);
				try
				{
					bool batchsynctransforms = Physics.batchsynctransforms;
					bool autosynctransforms = Physics.autosynctransforms;
					if (batchsynctransforms && autosynctransforms)
					{
						Profiler.BeginSample("Physics.PauseAutoSyncTransforms");
						Physics.autoSyncTransforms = false;
						Profiler.EndSample();
					}
					if (!Physics.autoSyncTransforms)
					{
						Profiler.BeginSample("Physics.SyncTransforms");
						Physics.SyncTransforms();
						Profiler.EndSample();
					}
					try
					{
						TimeWarning val7 = TimeWarning.New("CameraRendererManager.Tick", 100);
						try
						{
							CameraRendererManager instance = SingletonComponent<CameraRendererManager>.Instance;
							if ((Object)(object)instance != (Object)null)
							{
								instance.Tick();
							}
						}
						finally
						{
							((IDisposable)val7)?.Dispose();
						}
					}
					catch (Exception ex6)
					{
						Debug.LogWarning((object)"Server Exception: CameraRendererManager.Tick");
						Debug.LogException(ex6, (Object)(object)this);
					}
					BasePlayer.ServerCycle(Time.deltaTime);
					try
					{
						TimeWarning val8 = TimeWarning.New("FlameTurret.BudgetedUpdate", 0);
						try
						{
							((ObjectWorkQueue<FlameTurret>)FlameTurret.updateFlameTurretQueueServer).RunQueue(0.25);
						}
						finally
						{
							((IDisposable)val8)?.Dispose();
						}
					}
					catch (Exception ex7)
					{
						Debug.LogWarning((object)"Server Exception: FlameTurret.BudgetedUpdate");
						Debug.LogException(ex7, (Object)(object)this);
					}
					try
					{
						TimeWarning val9 = TimeWarning.New("AutoTurret.BudgetedUpdate", 0);
						try
						{
							((ObjectWorkQueue<AutoTurret>)AutoTurret.updateAutoTurretScanQueue).RunQueue(0.5);
						}
						finally
						{
							((IDisposable)val9)?.Dispose();
						}
					}
					catch (Exception ex8)
					{
						Debug.LogWarning((object)"Server Exception: AutoTurret.BudgetedUpdate");
						Debug.LogException(ex8, (Object)(object)this);
					}
					try
					{
						TimeWarning val10 = TimeWarning.New("BaseFishingRod.BudgetedUpdate", 0);
						try
						{
							((ObjectWorkQueue<BaseFishingRod>)BaseFishingRod.updateFishingRodQueue).RunQueue(1.0);
						}
						finally
						{
							((IDisposable)val10)?.Dispose();
						}
					}
					catch (Exception ex9)
					{
						Debug.LogWarning((object)"Server Exception: BaseFishingRod.BudgetedUpdate");
						Debug.LogException(ex9, (Object)(object)this);
					}
					if (batchsynctransforms && autosynctransforms)
					{
						Profiler.BeginSample("Physics.ResumeAutoSyncTransforms");
						Physics.autoSyncTransforms = true;
						Profiler.EndSample();
					}
				}
				finally
				{
					((IDisposable)val6)?.Dispose();
				}
			}
			catch (Exception ex10)
			{
				Debug.LogWarning((object)"Server Exception: Player Update");
				Debug.LogException(ex10, (Object)(object)this);
			}
			try
			{
				TimeWarning val11 = TimeWarning.New("connectionQueue.Cycle", 0);
				try
				{
					connectionQueue.Cycle(AvailableSlots);
				}
				finally
				{
					((IDisposable)val11)?.Dispose();
				}
			}
			catch (Exception ex11)
			{
				Debug.LogWarning((object)"Server Exception: Connection Queue");
				Debug.LogException(ex11, (Object)(object)this);
			}
			try
			{
				TimeWarning val12 = TimeWarning.New("IOEntity.ProcessQueue", 0);
				try
				{
					IOEntity.ProcessQueue();
				}
				finally
				{
					((IDisposable)val12)?.Dispose();
				}
			}
			catch (Exception ex12)
			{
				Debug.LogWarning((object)"Server Exception: IOEntity.ProcessQueue");
				Debug.LogException(ex12, (Object)(object)this);
			}
			if (!AI.spliceupdates)
			{
				aiTick = AIThinkManager.QueueType.Human;
			}
			else
			{
				aiTick = ((aiTick == AIThinkManager.QueueType.Human) ? AIThinkManager.QueueType.Animal : AIThinkManager.QueueType.Human);
			}
			if (aiTick == AIThinkManager.QueueType.Human)
			{
				try
				{
					TimeWarning val13 = TimeWarning.New("AIThinkManager.ProcessQueue", 0);
					try
					{
						AIThinkManager.ProcessQueue(AIThinkManager.QueueType.Human);
					}
					finally
					{
						((IDisposable)val13)?.Dispose();
					}
				}
				catch (Exception ex13)
				{
					Debug.LogWarning((object)"Server Exception: AIThinkManager.ProcessQueue");
					Debug.LogException(ex13, (Object)(object)this);
				}
				if (!AI.spliceupdates)
				{
					aiTick = AIThinkManager.QueueType.Animal;
				}
			}
			if (aiTick == AIThinkManager.QueueType.Animal)
			{
				try
				{
					TimeWarning val14 = TimeWarning.New("AIThinkManager.ProcessAnimalQueue", 0);
					try
					{
						AIThinkManager.ProcessQueue(AIThinkManager.QueueType.Animal);
					}
					finally
					{
						((IDisposable)val14)?.Dispose();
					}
				}
				catch (Exception ex14)
				{
					Debug.LogWarning((object)"Server Exception: AIThinkManager.ProcessAnimalQueue");
					Debug.LogException(ex14, (Object)(object)this);
				}
			}
			try
			{
				TimeWarning val15 = TimeWarning.New("AIThinkManager.ProcessPetQueue", 0);
				try
				{
					AIThinkManager.ProcessQueue(AIThinkManager.QueueType.Pets);
				}
				finally
				{
					((IDisposable)val15)?.Dispose();
				}
			}
			catch (Exception ex15)
			{
				Debug.LogWarning((object)"Server Exception: AIThinkManager.ProcessPetQueue");
				Debug.LogException(ex15, (Object)(object)this);
			}
			try
			{
				TimeWarning val16 = TimeWarning.New("AIThinkManager.ProcessPetMovementQueue", 0);
				try
				{
					BasePet.ProcessMovementQueue();
				}
				finally
				{
					((IDisposable)val16)?.Dispose();
				}
			}
			catch (Exception ex16)
			{
				Debug.LogWarning((object)"Server Exception: AIThinkManager.ProcessPetMovementQueue");
				Debug.LogException(ex16, (Object)(object)this);
			}
			try
			{
				TimeWarning val17 = TimeWarning.New("BaseRidableAnimal.ProcessQueue", 0);
				try
				{
					BaseRidableAnimal.ProcessQueue();
				}
				finally
				{
					((IDisposable)val17)?.Dispose();
				}
			}
			catch (Exception ex17)
			{
				Debug.LogWarning((object)"Server Exception: BaseRidableAnimal.ProcessQueue");
				Debug.LogException(ex17, (Object)(object)this);
			}
			try
			{
				TimeWarning val18 = TimeWarning.New("GrowableEntity.BudgetedUpdate", 0);
				try
				{
					((ObjectWorkQueue<GrowableEntity>)GrowableEntity.growableEntityUpdateQueue).RunQueue((double)GrowableEntity.framebudgetms);
				}
				finally
				{
					((IDisposable)val18)?.Dispose();
				}
			}
			catch (Exception ex18)
			{
				Debug.LogWarning((object)"Server Exception: GrowableEntity.BudgetedUpdate");
				Debug.LogException(ex18, (Object)(object)this);
			}
			try
			{
				TimeWarning val19 = TimeWarning.New("BasePlayer.BudgetedLifeStoryUpdate", 0);
				try
				{
					((ObjectWorkQueue<BasePlayer>)BasePlayer.lifeStoryQueue).RunQueue((double)BasePlayer.lifeStoryFramebudgetms);
				}
				finally
				{
					((IDisposable)val19)?.Dispose();
				}
			}
			catch (Exception ex19)
			{
				Debug.LogWarning((object)"Server Exception: BasePlayer.BudgetedLifeStoryUpdate");
				Debug.LogException(ex19, (Object)(object)this);
			}
			try
			{
				TimeWarning val20 = TimeWarning.New("JunkPileWater.UpdateNearbyPlayers", 0);
				try
				{
					((ObjectWorkQueue<JunkPileWater>)JunkPileWater.junkpileWaterWorkQueue).RunQueue((double)JunkPileWater.framebudgetms);
				}
				finally
				{
					((IDisposable)val20)?.Dispose();
				}
			}
			catch (Exception ex20)
			{
				Debug.LogWarning((object)"Server Exception: JunkPileWater.UpdateNearbyPlayers");
				Debug.LogException(ex20, (Object)(object)this);
			}
			try
			{
				TimeWarning val21 = TimeWarning.New("IndustrialEntity.RunQueue", 0);
				try
				{
					((ObjectWorkQueue<IndustrialEntity>)IndustrialEntity.Queue).RunQueue((double)ConVar.Server.industrialFrameBudgetMs);
				}
				finally
				{
					((IDisposable)val21)?.Dispose();
				}
			}
			catch (Exception ex21)
			{
				Debug.LogWarning((object)"Server Exception: IndustrialEntity.RunQueue");
				Debug.LogException(ex21, (Object)(object)this);
			}
			try
			{
				TimeWarning val22 = TimeWarning.New("AntiHack.Cycle", 0);
				try
				{
					AntiHack.Cycle();
				}
				finally
				{
					((IDisposable)val22)?.Dispose();
				}
			}
			catch (Exception ex22)
			{
				Debug.LogWarning((object)"Server Exception: AntiHack.Cycle");
				Debug.LogException(ex22, (Object)(object)this);
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private void LateUpdate()
	{
		if (!runFrameUpdate)
		{
			return;
		}
		TimeWarning val = TimeWarning.New("ServerMgr.LateUpdate", 500);
		try
		{
			if (!SteamNetworking.steamnagleflush)
			{
				return;
			}
			try
			{
				TimeWarning val2 = TimeWarning.New("Connection.Flush", 0);
				try
				{
					for (int i = 0; i < Net.sv.connections.Count; i++)
					{
						Net.sv.Flush(Net.sv.connections[i]);
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
			}
			catch (Exception ex)
			{
				Debug.LogWarning((object)"Server Exception: Connection.Flush");
				Debug.LogException(ex, (Object)(object)this);
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private void FixedUpdate()
	{
		TimeWarning val = TimeWarning.New("ServerMgr.FixedUpdate", 0);
		try
		{
			try
			{
				TimeWarning val2 = TimeWarning.New("BaseMountable.FixedUpdateCycle", 0);
				try
				{
					BaseMountable.FixedUpdateCycle();
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
			}
			catch (Exception ex)
			{
				Debug.LogWarning((object)"Server Exception: Mountable Cycle");
				Debug.LogException(ex, (Object)(object)this);
			}
			try
			{
				TimeWarning val3 = TimeWarning.New("Buoyancy.Cycle", 0);
				try
				{
					Buoyancy.Cycle();
				}
				finally
				{
					((IDisposable)val3)?.Dispose();
				}
			}
			catch (Exception ex2)
			{
				Debug.LogWarning((object)"Server Exception: Buoyancy Cycle");
				Debug.LogException(ex2, (Object)(object)this);
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private void DoTick()
	{
		Profiler.BeginSample("DoTick()");
		RCon.Update();
		Profiler.BeginSample("CompanionServer.Update");
		CompanionServer.Server.Update();
		Profiler.EndSample();
		for (int i = 0; i < Net.sv.connections.Count; i++)
		{
			Connection val = Net.sv.connections[i];
			if (!val.isAuthenticated && !(val.GetSecondsConnected() < (float)ConVar.Server.authtimeout))
			{
				Net.sv.Kick(val, "Authentication Timed Out", false);
			}
		}
		Profiler.EndSample();
	}

	private void DoHeartbeat()
	{
		ItemManager.Heartbeat();
	}

	private static BaseGameMode Gamemode()
	{
		BaseGameMode activeGameMode = BaseGameMode.GetActiveGameMode(serverside: true);
		return ((Object)(object)activeGameMode != (Object)null) ? activeGameMode : null;
	}

	public static string GamemodeName()
	{
		return Gamemode()?.shortname ?? "rust";
	}

	public static string GamemodeTitle()
	{
		return Gamemode()?.gamemodeTitle ?? "Survival";
	}

	private void UpdateServerInformation()
	{
		if (!SteamServer.IsValid)
		{
			return;
		}
		TimeWarning val = TimeWarning.New("UpdateServerInformation", 0);
		try
		{
			SteamServer.ServerName = ConVar.Server.hostname;
			SteamServer.MaxPlayers = ConVar.Server.maxplayers;
			SteamServer.Passworded = false;
			SteamServer.MapName = World.GetServerBrowserMapName();
			string text = "stok";
			if (Restarting)
			{
				text = "strst";
			}
			string text2 = $"born{Epoch.FromDateTime(SaveRestore.SaveCreatedTime)}";
			string text3 = $"gm{GamemodeName()}";
			string text4 = (ConVar.Server.pve ? ",pve" : string.Empty);
			string text5 = ConVar.Server.tags?.Trim(',') ?? "";
			string text6 = ((!string.IsNullOrWhiteSpace(text5)) ? ("," + text5) : "");
			BuildInfo current = BuildInfo.Current;
			object obj;
			if (current == null)
			{
				obj = null;
			}
			else
			{
				ScmInfo scm = current.Scm;
				obj = ((scm != null) ? scm.ChangeId : null);
			}
			if (obj == null)
			{
				obj = "0";
			}
			string text7 = (string)obj;
			string gameTags = $"mp{ConVar.Server.maxplayers},cp{BasePlayer.activePlayerList.Count},pt{Net.sv.ProtocolId},qp{SingletonComponent<ServerMgr>.Instance.connectionQueue.Queued},v{2402}{text4}{text6},h{AssemblyHash},{text},{text2},{text3},cs{text7}";
			SteamServer.GameTags = gameTags;
			if (ConVar.Server.description != null && ConVar.Server.description.Length > 100)
			{
				string[] array = StringEx.SplitToChunks(ConVar.Server.description, 100).ToArray();
				for (int i = 0; i < 16; i++)
				{
					if (i < array.Length)
					{
						SteamServer.SetKey($"description_{i:00}", array[i]);
					}
					else
					{
						SteamServer.SetKey($"description_{i:00}", string.Empty);
					}
				}
			}
			else
			{
				SteamServer.SetKey("description_0", ConVar.Server.description);
				for (int j = 1; j < 16; j++)
				{
					SteamServer.SetKey($"description_{j:00}", string.Empty);
				}
			}
			SteamServer.SetKey("hash", AssemblyHash);
			string text8 = World.Seed.ToString();
			BaseGameMode activeGameMode = BaseGameMode.GetActiveGameMode(serverside: true);
			if ((Object)(object)activeGameMode != (Object)null && !activeGameMode.ingameMap)
			{
				text8 = "0";
			}
			SteamServer.SetKey("world.seed", text8);
			SteamServer.SetKey("world.size", World.Size.ToString());
			SteamServer.SetKey("pve", ConVar.Server.pve.ToString());
			SteamServer.SetKey("headerimage", ConVar.Server.headerimage);
			SteamServer.SetKey("logoimage", ConVar.Server.logoimage);
			SteamServer.SetKey("url", ConVar.Server.url);
			SteamServer.SetKey("gmn", GamemodeName());
			SteamServer.SetKey("gmt", GamemodeTitle());
			SteamServer.SetKey("uptime", ((int)Time.realtimeSinceStartup).ToString());
			SteamServer.SetKey("gc_mb", Performance.report.memoryAllocations.ToString());
			SteamServer.SetKey("gc_cl", Performance.report.memoryCollections.ToString());
			SteamServer.SetKey("ram_sys", (Performance.report.memoryUsageSystem / 1000000).ToString());
			SteamServer.SetKey("fps", Performance.report.frameRate.ToString());
			SteamServer.SetKey("fps_avg", Performance.report.frameRateAverage.ToString("0.00"));
			SteamServer.SetKey("ent_cnt", BaseNetworkable.serverEntities.Count.ToString());
			SteamServer.SetKey("build", BuildInfo.Current.Scm.ChangeId);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void OnDisconnected(string strReason, Connection connection)
	{
		Analytics.Azure.OnPlayerDisconnected(connection, strReason);
		Profiler.BeginSample("OnDisconnected");
		connectionQueue.RemoveConnection(connection);
		ConnectionAuth.OnDisconnect(connection);
		Profiler.BeginSample("Steam.Auth.EndSession");
		PlatformService.Instance.EndPlayerSession(connection.userid);
		Profiler.EndSample();
		EACServer.OnLeaveGame(connection);
		BasePlayer basePlayer = connection.player as BasePlayer;
		if (Object.op_Implicit((Object)(object)basePlayer))
		{
			Profiler.BeginSample("player.OnDisconnected");
			basePlayer.OnDisconnected();
			Profiler.EndSample();
		}
		Profiler.EndSample();
	}

	public static void OnEnterVisibility(Connection connection, Group group)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		if (((BaseNetwork)Net.sv).IsConnected())
		{
			NetWrite val = ((BaseNetwork)Net.sv).StartWrite();
			val.PacketID((Type)19);
			val.GroupID(group.ID);
			val.Send(new SendInfo(connection));
		}
	}

	public static void OnLeaveVisibility(Connection connection, Group group)
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		if (((BaseNetwork)Net.sv).IsConnected())
		{
			NetWrite val = ((BaseNetwork)Net.sv).StartWrite();
			val.PacketID((Type)20);
			val.GroupID(group.ID);
			val.Send(new SendInfo(connection));
			NetWrite val2 = ((BaseNetwork)Net.sv).StartWrite();
			val2.PacketID((Type)8);
			val2.GroupID(group.ID);
			val2.Send(new SendInfo(connection));
		}
	}

	public void SpawnMapEntities()
	{
		PrefabPreProcess prefabPreProcess = new PrefabPreProcess(clientside: false, serverside: true);
		BaseEntity[] array = Object.FindObjectsOfType<BaseEntity>();
		BaseEntity[] array2 = array;
		foreach (BaseEntity baseEntity in array2)
		{
			baseEntity.SpawnAsMapEntity();
		}
		DebugEx.Log((object)$"Map Spawned {array.Length} entities", (StackTraceLogType)0);
		BaseEntity[] array3 = array;
		foreach (BaseEntity baseEntity2 in array3)
		{
			if ((Object)(object)baseEntity2 != (Object)null)
			{
				baseEntity2.PostMapEntitySpawn();
			}
		}
	}

	public static BasePlayer.SpawnPoint FindSpawnPoint(BasePlayer forPlayer = null)
	{
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		Profiler.BeginSample("ServerMgr.FindSpawnPoint");
		bool flag = false;
		BaseGameMode baseGameMode = Gamemode();
		if (Object.op_Implicit((Object)(object)baseGameMode) && baseGameMode.useCustomSpawns)
		{
			BasePlayer.SpawnPoint playerSpawn = baseGameMode.GetPlayerSpawn(forPlayer);
			if (playerSpawn != null)
			{
				return playerSpawn;
			}
		}
		if ((Object)(object)SingletonComponent<SpawnHandler>.Instance != (Object)null && !flag)
		{
			BasePlayer.SpawnPoint spawnPoint = SpawnHandler.GetSpawnPoint();
			if (spawnPoint != null)
			{
				Profiler.EndSample();
				return spawnPoint;
			}
		}
		BasePlayer.SpawnPoint spawnPoint2 = new BasePlayer.SpawnPoint();
		GameObject[] array = GameObject.FindGameObjectsWithTag("spawnpoint");
		if (array.Length != 0)
		{
			GameObject val = array[Random.Range(0, array.Length)];
			spawnPoint2.pos = val.transform.position;
			spawnPoint2.rot = val.transform.rotation;
		}
		else
		{
			Debug.Log((object)"Couldn't find an appropriate spawnpoint for the player - so spawning at camera");
			if ((Object)(object)MainCamera.mainCamera != (Object)null)
			{
				spawnPoint2.pos = MainCamera.position;
				spawnPoint2.rot = MainCamera.rotation;
			}
		}
		RaycastHit val2 = default(RaycastHit);
		if (Physics.Raycast(new Ray(spawnPoint2.pos, Vector3.down), ref val2, 32f, 1537286401))
		{
			spawnPoint2.pos = ((RaycastHit)(ref val2)).point;
		}
		Profiler.EndSample();
		return spawnPoint2;
	}

	public void JoinGame(Connection connection)
	{
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		Approval val = Pool.Get<Approval>();
		try
		{
			uint num = (uint)ConVar.Server.encryption;
			if (num > 1 && connection.os == "editor" && DeveloperList.Contains(connection.ownerid))
			{
				num = 1u;
			}
			if (num > 1 && !ConVar.Server.secure)
			{
				num = 1u;
			}
			val.level = Application.loadedLevelName;
			val.levelTransfer = World.Transfer;
			val.levelUrl = World.Url;
			val.levelSeed = World.Seed;
			val.levelSize = World.Size;
			val.checksum = World.Checksum;
			val.hostname = ConVar.Server.hostname;
			val.official = ConVar.Server.official;
			val.encryption = num;
			val.version = BuildInfo.Current.Scm.Branch + "#" + BuildInfo.Current.Scm.ChangeId;
			NetWrite val2 = ((BaseNetwork)Net.sv).StartWrite();
			val2.PacketID((Type)3);
			val.WriteToStream((Stream)(object)val2);
			val2.Send(new SendInfo(connection));
			connection.encryptionLevel = num;
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		connection.connected = true;
	}

	internal void Shutdown()
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		BasePlayer[] array = ((IEnumerable<BasePlayer>)BasePlayer.activePlayerList).ToArray();
		foreach (BasePlayer basePlayer in array)
		{
			basePlayer.Kick("Server Shutting Down");
		}
		ConsoleSystem.Run(Option.Server, "server.save", Array.Empty<object>());
		ConsoleSystem.Run(Option.Server, "server.writecfg", Array.Empty<object>());
	}

	private IEnumerator ServerRestartWarning(string info, int iSeconds)
	{
		if (iSeconds < 0)
		{
			yield break;
		}
		if (!string.IsNullOrEmpty(info))
		{
			ConsoleNetwork.BroadcastToAllClients("chat.add", 2, 0, "<color=#fff>SERVER</color> Restarting: " + info);
		}
		for (int i = iSeconds; i > 0; i--)
		{
			if (i == iSeconds || i % 60 == 0 || (i < 300 && i % 30 == 0) || (i < 60 && i % 10 == 0) || i < 10)
			{
				ConsoleNetwork.BroadcastToAllClients("chat.add", 2, 0, $"<color=#fff>SERVER</color> Restarting in {i} seconds ({info})!");
				Debug.Log((object)$"Restarting in {i} seconds");
			}
			yield return CoroutineEx.waitForSeconds(1f);
		}
		ConsoleNetwork.BroadcastToAllClients("chat.add", 2, 0, "<color=#fff>SERVER</color> Restarting (" + info + ")");
		yield return CoroutineEx.waitForSeconds(2f);
		BasePlayer[] array = ((IEnumerable<BasePlayer>)BasePlayer.activePlayerList).ToArray();
		foreach (BasePlayer ply in array)
		{
			ply.Kick("Server Restarting");
		}
		yield return CoroutineEx.waitForSeconds(1f);
		ConsoleSystem.Run(Option.Server, "quit", Array.Empty<object>());
	}

	public static void RestartServer(string strNotice, int iSeconds)
	{
		if (!((Object)(object)SingletonComponent<ServerMgr>.Instance == (Object)null))
		{
			if (SingletonComponent<ServerMgr>.Instance.restartCoroutine != null)
			{
				ConsoleNetwork.BroadcastToAllClients("chat.add", 2, 0, "<color=#fff>SERVER</color> Restart interrupted!");
				((MonoBehaviour)SingletonComponent<ServerMgr>.Instance).StopCoroutine(SingletonComponent<ServerMgr>.Instance.restartCoroutine);
				SingletonComponent<ServerMgr>.Instance.restartCoroutine = null;
			}
			SingletonComponent<ServerMgr>.Instance.restartCoroutine = SingletonComponent<ServerMgr>.Instance.ServerRestartWarning(strNotice, iSeconds);
			((MonoBehaviour)SingletonComponent<ServerMgr>.Instance).StartCoroutine(SingletonComponent<ServerMgr>.Instance.restartCoroutine);
			SingletonComponent<ServerMgr>.Instance.UpdateServerInformation();
		}
	}

	public static void SendReplicatedVars(string filter)
	{
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		Profiler.BeginSample("SendReplicatedVars");
		NetWrite val = ((BaseNetwork)Net.sv).StartWrite();
		List<Connection> list = Pool.GetList<Connection>();
		foreach (Connection connection in Net.sv.connections)
		{
			if (connection.connected)
			{
				list.Add(connection);
			}
		}
		List<Command> list2 = Pool.GetList<Command>();
		foreach (Command item in Server.Replicated)
		{
			if (item.FullName.StartsWith(filter))
			{
				list2.Add(item);
			}
		}
		val.PacketID((Type)25);
		val.Int32(list2.Count);
		foreach (Command item2 in list2)
		{
			val.String(item2.FullName);
			val.String(item2.String);
		}
		val.Send(new SendInfo(list));
		Pool.FreeList<Command>(ref list2);
		Pool.FreeList<Connection>(ref list);
		Profiler.EndSample();
	}

	public static void SendReplicatedVars(Connection connection)
	{
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		Profiler.BeginSample("SendReplicatedVars");
		NetWrite val = ((BaseNetwork)Net.sv).StartWrite();
		List<Command> replicated = Server.Replicated;
		val.PacketID((Type)25);
		val.Int32(replicated.Count);
		foreach (Command item in replicated)
		{
			val.String(item.FullName);
			val.String(item.String);
		}
		val.Send(new SendInfo(connection));
		Profiler.EndSample();
	}

	private static void OnReplicatedVarChanged(string fullName, string value)
	{
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		Profiler.BeginSample("OnReplicatedVarChanged");
		NetWrite val = ((BaseNetwork)Net.sv).StartWrite();
		List<Connection> list = Pool.GetList<Connection>();
		foreach (Connection connection in Net.sv.connections)
		{
			if (connection.connected)
			{
				list.Add(connection);
			}
		}
		val.PacketID((Type)25);
		val.Int32(1);
		val.String(fullName);
		val.String(value);
		val.Send(new SendInfo(list));
		Pool.FreeList<Connection>(ref list);
		Profiler.EndSample();
	}
}
