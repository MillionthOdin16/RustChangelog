using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using CompanionServer;
using ConVar;
using Facepunch;
using Facepunch.Extend;
using Facepunch.Nexus;
using Facepunch.Nexus.Logging;
using Facepunch.Nexus.Models;
using Facepunch.Sqlite;
using Network;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ProtoBuf;
using ProtoBuf.Nexus;
using Rust.Nexus.Handlers;
using UnityEngine;

public static class NexusServer
{
	private struct ZonePlayerManifest
	{
		public RealTimeSince Received;

		public List<ulong> UserIds;
	}

	private struct PendingCall
	{
		public bool IsBroadcast;

		public RealTimeUntil TimeUntilTimeout;

		public TaskCompletionSource<bool> Completion;

		public NexusRpcResult Result;
	}

	[Serializable]
	[CompilerGenerated]
	private sealed class _003C_003Ec
	{
		public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

		public static NexusErrorHandler _003C_003E9__27_0;

		public static Func<NexusZoneDetails, string> _003C_003E9__30_0;

		public static Comparison<(string Zone, FerryStatus Status)> _003C_003E9__46_1;

		public static Func<NexusZoneDetails, string> _003C_003E9__51_0;

		public static Func<NexusZoneDetails, int> _003C_003E9__64_0;

		public static Func<NexusZoneDetails, string> _003C_003E9__86_0;

		public static Func<Request, TransferRequest> _003C_003E9__90_0;

		public static Func<Request, PingRequest> _003C_003E9__90_1;

		public static Func<Request, SpawnOptionsRequest> _003C_003E9__90_2;

		public static Func<Request, SleepingBagRespawnRequest> _003C_003E9__90_3;

		public static Func<Request, SleepingBagDestroyRequest> _003C_003E9__90_4;

		public static Func<Request, FerryStatusRequest> _003C_003E9__90_5;

		public static Func<Request, FerryRetireRequest> _003C_003E9__90_6;

		public static Func<Request, FerryUpdateScheduleRequest> _003C_003E9__90_7;

		public static Func<Request, ClanChatBatchRequest> _003C_003E9__90_8;

		public static Func<Request, PlayerManifestRequest> _003C_003E9__90_9;

		public static Func<NexusZoneDetails, string> _003C_003E9__96_1;

		public static Func<NexusZoneDetails, string> _003C_003E9__103_0;

		internal void _003CInitialize_003Eb__27_0(BaseNexusClient _, Exception ex)
		{
			Debug.LogException(ex);
		}

		internal string _003CFindZone_003Eb__30_0(NexusZoneDetails z)
		{
			return z.Key;
		}

		internal int _003CUpdateFerryStatuses_003Eb__46_1((string Zone, FerryStatus Status) a, (string Zone, FerryStatus Status) b)
		{
			return a.Status.timestamp.CompareTo(b.Status.timestamp);
		}

		internal string _003CTryGetIslandPosition_003Eb__51_0(NexusZoneDetails z)
		{
			return z.Key;
		}

		internal int _003CHandleMessage_003Eb__64_0(NexusZoneDetails z)
		{
			return z.Id;
		}

		internal string _003CZoneRpc_003Eb__86_0(NexusZoneDetails z)
		{
			return z.Key;
		}

		internal TransferRequest _003CHandleRpcInvocationImpl_003Eb__90_0(Request r)
		{
			return r.transfer;
		}

		internal PingRequest _003CHandleRpcInvocationImpl_003Eb__90_1(Request r)
		{
			return r.ping;
		}

		internal SpawnOptionsRequest _003CHandleRpcInvocationImpl_003Eb__90_2(Request r)
		{
			return r.spawnOptions;
		}

		internal SleepingBagRespawnRequest _003CHandleRpcInvocationImpl_003Eb__90_3(Request r)
		{
			return r.respawnAtBag;
		}

		internal SleepingBagDestroyRequest _003CHandleRpcInvocationImpl_003Eb__90_4(Request r)
		{
			return r.destroyBag;
		}

		internal FerryStatusRequest _003CHandleRpcInvocationImpl_003Eb__90_5(Request r)
		{
			return r.ferryStatus;
		}

		internal FerryRetireRequest _003CHandleRpcInvocationImpl_003Eb__90_6(Request r)
		{
			return r.ferryRetire;
		}

		internal FerryUpdateScheduleRequest _003CHandleRpcInvocationImpl_003Eb__90_7(Request r)
		{
			return r.ferryUpdateSchedule;
		}

		internal ClanChatBatchRequest _003CHandleRpcInvocationImpl_003Eb__90_8(Request r)
		{
			return r.clanChatBatch;
		}

		internal PlayerManifestRequest _003CHandleRpcInvocationImpl_003Eb__90_9(Request r)
		{
			return r.playerManifest;
		}

		internal string _003CRefreshZoneStatus_003Eb__96_1(NexusZoneDetails z)
		{
			return z.Key;
		}

		internal string _003CTransferEntityImpl_003Eb__103_0(NexusZoneDetails z)
		{
			return z.Key;
		}

		internal string _003C_002Ecctor_003Eb__113_0(ulong i)
		{
			return i.ToString("G");
		}
	}

	private static bool _isRefreshingCompanion;

	private static RealTimeSince _lastCompanionRefresh;

	private static readonly Memoized<string, ulong> SteamIdToString = new Memoized<string, ulong>((Func<ulong, string>)((ulong i) => i.ToString("G")));

	private static readonly MemoryStream WriterStream = new MemoryStream();

	private static readonly ByteArrayStream ReaderStream = new ByteArrayStream();

	private static NexusDB _database;

	private static readonly Dictionary<string, List<(string Zone, FerryStatus Status)>> FerryEntries = new Dictionary<string, List<(string, FerryStatus)>>(StringComparer.InvariantCultureIgnoreCase);

	private static bool _updatingFerries;

	private static int _cyclesWithoutFerry;

	private static float _zoneContactRadius;

	private static Dictionary<string, NexusIsland> _existingIslands;

	private const int MapRenderVersion = 5;

	private static readonly HashSet<ulong> PlayerManifest = new HashSet<ulong>();

	private static readonly Dictionary<string, ZonePlayerManifest> ZonePlayerManifests = new Dictionary<string, ZonePlayerManifest>(StringComparer.InvariantCultureIgnoreCase);

	private static RealTimeSince _lastPlayerManifestBroadcast;

	private static bool _playerManifestDirty;

	private static RealTimeSince _lastPlayerManifestRebuild;

	private static readonly Dictionary<Uuid, PendingCall> PendingCalls = new Dictionary<Uuid, PendingCall>();

	private static RealTimeSince _sinceLastRpcTimeoutCheck = RealTimeSince.op_Implicit(0f);

	private static readonly Dictionary<string, ServerStatus> ZoneStatuses = new Dictionary<string, ServerStatus>(StringComparer.InvariantCultureIgnoreCase);

	private static bool _isRefreshingZoneStatus;

	private static RealTimeSince _lastZoneStatusRefresh;

	private static DateTimeOffset? _lastUnsavedTransfer;

	private const string CopyFromKey = "$copyFrom";

	public static NexusZoneClient ZoneClient { get; private set; }

	public static bool Started { get; private set; }

	public static bool FailedToStart { get; private set; }

	public static int? NexusId
	{
		get
		{
			NexusZoneClient zoneClient = ZoneClient;
			if (zoneClient == null)
			{
				return null;
			}
			ZoneDetails zone = zoneClient.Zone;
			if (zone == null)
			{
				return null;
			}
			return zone.NexusId;
		}
	}

	public static string ZoneKey
	{
		get
		{
			NexusZoneClient zoneClient = ZoneClient;
			if (zoneClient == null)
			{
				return null;
			}
			ZoneDetails zone = zoneClient.Zone;
			if (zone == null)
			{
				return null;
			}
			return zone.Key;
		}
	}

	public static long? LastReset
	{
		get
		{
			NexusZoneClient zoneClient = ZoneClient;
			if (zoneClient == null)
			{
				return null;
			}
			NexusDetails nexus = zoneClient.Nexus;
			if (nexus == null)
			{
				return null;
			}
			return nexus.LastReset;
		}
	}

	public static List<NexusZoneDetails> Zones
	{
		get
		{
			NexusZoneClient zoneClient = ZoneClient;
			if (zoneClient == null)
			{
				return null;
			}
			NexusDetails nexus = zoneClient.Nexus;
			if (nexus == null)
			{
				return null;
			}
			return nexus.Zones;
		}
	}

	public static bool NeedsJournalFlush
	{
		get
		{
			if (Started && _database.OldestJournal.HasValue)
			{
				return (DateTimeOffset.UtcNow - _database.OldestJournal.Value).TotalSeconds >= (double)Nexus.transferFlushTime;
			}
			return false;
		}
	}

	private static int RpcResponseTtl => Nexus.messageLockDuration * 4;

	public static bool NeedTransferFlush
	{
		get
		{
			if (Started && _lastUnsavedTransfer.HasValue)
			{
				return (DateTimeOffset.UtcNow - _lastUnsavedTransfer.Value).TotalSeconds >= (double)Nexus.transferFlushTime;
			}
			return false;
		}
	}

	private static void RefreshCompanionVariables()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		if (!_isRefreshingCompanion && !(RealTimeSince.op_Implicit(_lastCompanionRefresh) < 60f))
		{
			RefreshCompanionVariablesImpl();
		}
		static async void RefreshCompanionVariablesImpl()
		{
			_ = 3;
			try
			{
				_isRefreshingCompanion = true;
				_lastCompanionRefresh = RealTimeSince.op_Implicit(0f);
				await ZoneClient.SetZoneVariable("protocol", Net.sv.ProtocolId, false, false);
				if (CompanionServer.Server.IsEnabled)
				{
					string text = await App.GetPublicIPAsync();
					string appPort = App.port.ToString("G", CultureInfo.InvariantCulture);
					await ZoneClient.SetZoneVariable("appIp", text, false, false);
					await ZoneClient.SetZoneVariable("appPort", appPort, false, false);
				}
			}
			catch (Exception ex)
			{
				Debug.LogError((object)"Failed to set up Rust companion nexus zone variables");
				Debug.LogException(ex);
			}
			finally
			{
				_isRefreshingCompanion = false;
			}
		}
	}

	public static IEnumerator Initialize()
	{
		if (Started)
		{
			Debug.LogError((object)"NexusServer was already started");
			yield break;
		}
		NexusZoneClient zoneClient = ZoneClient;
		if (zoneClient != null)
		{
			((BaseNexusClient)zoneClient).Dispose();
		}
		ZoneClient = null;
		NexusDB database = _database;
		if (database != null)
		{
			((Database)database).Close();
		}
		_database = null;
		ZoneController.Instance = null;
		Started = false;
		FailedToStart = true;
		if (string.IsNullOrWhiteSpace(Nexus.endpoint) || !Nexus.endpoint.StartsWith("http") || string.IsNullOrWhiteSpace(Nexus.secretKey))
		{
			Debug.Log((object)"Nexus endpoint and/or secret key is not set, not starting nexus connection");
			FailedToStart = false;
			yield break;
		}
		GameObject val = new GameObject("NexusCleanupOnShutdown");
		val.AddComponent<NexusCleanupOnShutdown>();
		Object.DontDestroyOnLoad((Object)val);
		try
		{
			_database = new NexusDB();
			((Database)_database).Open($"{ConVar.Server.rootFolder}/nexus.{243}.db", true);
			_database.Initialize();
		}
		catch (Exception ex2)
		{
			Debug.LogException(ex2);
			yield break;
		}
		ZoneClient = new NexusZoneClient((INexusLogger)(object)NexusServerLogger.Instance, Nexus.endpoint, Nexus.secretKey, Nexus.messageLockDuration);
		NexusZoneClient zoneClient2 = ZoneClient;
		object obj = _003C_003Ec._003C_003E9__27_0;
		if (obj == null)
		{
			NexusErrorHandler val2 = delegate(BaseNexusClient _, Exception ex)
			{
				Debug.LogException(ex);
			};
			_003C_003Ec._003C_003E9__27_0 = val2;
			obj = (object)val2;
		}
		((BaseNexusClient)zoneClient2).OnError += (NexusErrorHandler)obj;
		Task startTask = ((BaseNexusClient)ZoneClient).Start();
		yield return (object)new WaitUntil((Func<bool>)(() => startTask.IsCompleted));
		if (startTask.Exception != null)
		{
			Debug.LogException((Exception)startTask.Exception);
			yield break;
		}
		if (string.IsNullOrWhiteSpace(ZoneKey))
		{
			Debug.LogError((object)"Zone name is not available after nexus initialization");
			yield break;
		}
		Debug.Log((object)$"Connected as zone '{ZoneKey}' in Nexus {ZoneClient.Zone.NexusName} (id={ZoneClient.Zone.NexusId})");
		ZoneController.Instance = BuildZoneController(Nexus.zoneController);
		if (ZoneController.Instance == null)
		{
			Debug.LogError((object)(string.IsNullOrWhiteSpace(Nexus.zoneController) ? "Zone controller was not specified (nexus.zoneController convar)" : ("Zone controller is not supported: " + Nexus.zoneController)));
			yield break;
		}
		Variable cfgVariable2 = default(Variable);
		if (ZoneClient.TryGetNexusVariable("server.cfg", ref cfgVariable2))
		{
			Debug.Log((object)"Running server.cfg from nexus variable");
			RunConsoleConfig(cfgVariable2);
		}
		Variable cfgVariable3 = default(Variable);
		if (ZoneClient.TryGetZoneVariable("server.cfg", ref cfgVariable3))
		{
			Debug.Log((object)"Running server.cfg from zone variable");
			RunConsoleConfig(cfgVariable3);
		}
		if (string.IsNullOrWhiteSpace(ConVar.World.configString) && string.IsNullOrWhiteSpace(ConVar.World.configFile))
		{
			Debug.Log((object)"Attempting to pull world config from the nexus");
			string worldConfigString;
			try
			{
				worldConfigString = GetWorldConfigString();
			}
			catch (Exception ex3)
			{
				Debug.LogException(ex3);
				yield break;
			}
			Debug.Log((object)("Will use world config from nexus: " + worldConfigString));
			ConVar.World.configString = worldConfigString;
		}
		else
		{
			Debug.LogWarning((object)"World config convar(s) are already set, will not pull world config from nexus");
		}
		Started = true;
		FailedToStart = false;
		static void RunConsoleConfig(Variable cfgVariable)
		{
			//IL_0004: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Invalid comparison between Unknown and I4
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			if (cfgVariable != null && (int)cfgVariable.Type == 1)
			{
				string asString = cfgVariable.GetAsString();
				if (!string.IsNullOrWhiteSpace(asString))
				{
					ConsoleSystem.RunFile(Option.Server, asString);
				}
			}
		}
	}

	public static void Shutdown()
	{
		Started = false;
		FailedToStart = false;
		_existingIslands?.Clear();
		NexusZoneClient zoneClient = ZoneClient;
		if (zoneClient != null)
		{
			((BaseNexusClient)zoneClient).Dispose();
		}
		ZoneClient = null;
		NexusDB database = _database;
		if (database != null)
		{
			((Database)database).Close();
		}
		_database = null;
	}

	public static void Update()
	{
		if (Started)
		{
			ReadIncomingMessages();
			CheckForRpcTimeouts();
			RefreshZoneStatus();
			UpdatePlayerManifest();
			RefreshCompanionVariables();
		}
	}

	public static NexusZoneDetails FindZone(string zoneKey)
	{
		NexusZoneClient zoneClient = ZoneClient;
		if (zoneClient == null)
		{
			return null;
		}
		NexusDetails nexus = zoneClient.Nexus;
		if (nexus == null)
		{
			return null;
		}
		List<NexusZoneDetails> zones = nexus.Zones;
		if (zones == null)
		{
			return null;
		}
		return List.FindWith<NexusZoneDetails, string>((IReadOnlyCollection<NexusZoneDetails>)zones, (Func<NexusZoneDetails, string>)((NexusZoneDetails z) => z.Key), zoneKey, (IEqualityComparer<string>)StringComparer.InvariantCultureIgnoreCase);
	}

	public static Task<NexusLoginResult> Login(ulong steamId)
	{
		return ZoneClient.PlayerLogin(SteamIdToString.Get(steamId));
	}

	public static void Logout(ulong steamId)
	{
		NexusZoneClient zoneClient = ZoneClient;
		if (zoneClient != null)
		{
			zoneClient.PlayerLogout(SteamIdToString.Get(steamId));
		}
	}

	public static bool TryGetPlayer(ulong steamId, out NexusPlayer player)
	{
		if (!Started)
		{
			player = null;
			return false;
		}
		return ZoneClient.TryGetPlayer(SteamIdToString.Get(steamId), ref player);
	}

	public static Task AssignInitialZone(ulong steamId, string zoneKey)
	{
		return ZoneClient.Assign(steamId.ToString("G"), zoneKey);
	}

	private static ZoneController BuildZoneController(string name)
	{
		if (name.ToLowerInvariant() == "basic")
		{
			return new BasicZoneController(ZoneClient);
		}
		return null;
	}

	public static void PostGameSaved()
	{
		_database?.ClearJournal();
		_database?.ClearTransferred();
		_lastUnsavedTransfer = null;
	}

	public static async void UpdateFerries()
	{
		if (ZoneClient == null || _updatingFerries)
		{
			return;
		}
		try
		{
			_updatingFerries = true;
			await UpdateFerriesImpl();
		}
		catch (Exception ex)
		{
			Debug.LogException(ex);
		}
		finally
		{
			_updatingFerries = false;
		}
	}

	private static async Task UpdateFerriesImpl()
	{
		if (ZoneClient == null)
		{
			return;
		}
		Request obj = Pool.Get<Request>();
		obj.ferryStatus = Pool.Get<FerryStatusRequest>();
		using (NexusRpcResult statusResponse = await BroadcastRpc(obj))
		{
			UpdateFerryStatuses(statusResponse);
		}
		string zone = ZoneKey;
		Variable val = default(Variable);
		List<(string, FerryStatus)> value;
		if (ZoneClient.TryGetZoneVariable("ferry", ref val) && (int)val.Type == 1 && TryParseFerrySchedule(zone, val.GetAsString(), out var schedule))
		{
			if (FerryEntries.TryGetValue(zone, out var entries) && entries.Count > 1)
			{
				for (int i = 1; i < entries.Count; i++)
				{
					(string, FerryStatus) tuple = entries[i];
					await RetireFerry(tuple.Item1, tuple.Item2.entityId, tuple.Item2.timestamp);
				}
			}
			if (entries != null && entries.Count > 0)
			{
				_cyclesWithoutFerry = 0;
				(string, FerryStatus) tuple2 = entries[0];
				if (!tuple2.Item2.schedule.SequenceEqual(schedule, StringComparer.InvariantCultureIgnoreCase))
				{
					await UpdateFerrySchedule(tuple2.Item1, tuple2.Item2.entityId, tuple2.Item2.timestamp, schedule);
				}
			}
			else
			{
				if (entries != null && entries.Count != 0)
				{
					return;
				}
				_cyclesWithoutFerry++;
				if (_cyclesWithoutFerry < 5)
				{
					return;
				}
				_cyclesWithoutFerry = 0;
				BaseEntity baseEntity = GameManager.server.CreateEntity("assets/content/nexus/ferry/nexusferry.entity.prefab");
				if (!(baseEntity is NexusFerry nexusFerry))
				{
					Debug.LogError((object)"Failed to spawn nexus ferry!");
					if ((Object)(object)baseEntity != (Object)null)
					{
						Object.Destroy((Object)(object)baseEntity);
					}
				}
				else
				{
					nexusFerry.Initialize(zone, schedule);
					nexusFerry.Spawn();
				}
			}
		}
		else if (FerryEntries.TryGetValue(zone, out value) && value.Count > 0)
		{
			_cyclesWithoutFerry = 0;
			foreach (var item in value)
			{
				await RetireFerry(item.Item1, item.Item2.entityId, item.Item2.timestamp);
			}
		}
		else
		{
			_cyclesWithoutFerry = 0;
		}
	}

	public static bool TryGetFerryStatus(string ownerZone, out string currentZone, out FerryStatus status)
	{
		if (!FerryEntries.TryGetValue(ownerZone, out List<(string, FerryStatus)> value) || value.Count < 1)
		{
			currentZone = null;
			status = null;
			return false;
		}
		(currentZone, status) = value[0];
		return true;
	}

	private static Task RetireFerry(string zone, NetworkableId entityId, long timestamp)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		Request val = Pool.Get<Request>();
		val.ferryRetire = Pool.Get<FerryRetireRequest>();
		val.ferryRetire.entityId = entityId;
		val.ferryRetire.timestamp = timestamp;
		return ZoneRpc(zone, val);
	}

	private static Task UpdateFerrySchedule(string zone, NetworkableId entityId, long timestamp, List<string> schedule)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		Request val = Pool.Get<Request>();
		val.ferryUpdateSchedule = Pool.Get<FerryUpdateScheduleRequest>();
		val.ferryUpdateSchedule.entityId = entityId;
		val.ferryUpdateSchedule.timestamp = timestamp;
		val.ferryUpdateSchedule.schedule = List.ShallowClonePooled<string>(schedule);
		return ZoneRpc(zone, val);
	}

	private static bool TryParseFerrySchedule(string zone, string scheduleString, out List<string> entries)
	{
		if (!NexusUtil.TryParseFerrySchedule(zone, scheduleString, out var entries2))
		{
			entries = null;
			return false;
		}
		List<string> list = entries2.ToList();
		foreach (string item in list)
		{
			if (FindZone(item) == null)
			{
				Debug.LogError((object)("Ferry schedule for '" + zone + "' lists an invalid zone '" + item + "': " + scheduleString));
				entries = null;
				return false;
			}
		}
		entries = list;
		return true;
	}

	private static void UpdateFerryStatuses(NexusRpcResult statusResponse)
	{
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		foreach (KeyValuePair<string, List<(string, FerryStatus)>> ferryEntry in FerryEntries)
		{
			List<(string, FerryStatus)> value = ferryEntry.Value;
			foreach (var item in value)
			{
				item.Item2.Dispose();
			}
			Pool.FreeList<(string, FerryStatus)>(ref value);
		}
		FerryEntries.Clear();
		foreach (KeyValuePair<string, Response> response in statusResponse.Responses)
		{
			FerryStatusResponse ferryStatus = response.Value.ferryStatus;
			if (ferryStatus?.statuses == null)
			{
				continue;
			}
			foreach (FerryStatus status in ferryStatus.statuses)
			{
				AddFerryStatus(response.Key, status);
			}
		}
		string zoneKey = ZoneKey;
		Enumerator<NexusFerry> enumerator5 = NexusFerry.All.GetEnumerator();
		try
		{
			while (enumerator5.MoveNext())
			{
				NexusFerry current3 = enumerator5.Current;
				AddFerryStatus(zoneKey, current3.GetStatus());
			}
		}
		finally
		{
			((IDisposable)enumerator5).Dispose();
		}
		foreach (List<(string, FerryStatus)> value3 in FerryEntries.Values)
		{
			if (value3.Count > 1)
			{
				value3.Sort(((string Zone, FerryStatus Status) a, (string Zone, FerryStatus Status) b) => a.Status.timestamp.CompareTo(b.Status.timestamp));
			}
		}
		static void AddFerryStatus(string currentZone, FerryStatus status)
		{
			if (!FerryEntries.TryGetValue(status.ownerZone, out List<(string, FerryStatus)> value2))
			{
				value2 = Pool.GetList<(string, FerryStatus)>();
				FerryEntries.Add(status.ownerZone, value2);
			}
			value2.Add((currentZone, status.Copy()));
		}
	}

	public static void UpdateIslands()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Invalid comparison between Unknown and I4
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		if (ZoneClient == null)
		{
			return;
		}
		Variable val = default(Variable);
		if (ZoneClient.TryGetNexusVariable("map.contactRadius", ref val) && (int)val.Type == 1 && float.TryParse(val.GetAsString(), out var result))
		{
			_zoneContactRadius = result;
		}
		else
		{
			_zoneContactRadius = Nexus.defaultZoneContactRadius;
		}
		if (_existingIslands == null)
		{
			_existingIslands = new Dictionary<string, NexusIsland>();
		}
		HashSet<NexusIsland> hashSet = Pool.Get<HashSet<NexusIsland>>();
		hashSet.Clear();
		if (_existingIslands.Count == 0)
		{
			Enumerator<BaseNetworkable> enumerator = BaseNetworkable.serverEntities.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current is NexusIsland nexusIsland)
					{
						if (string.IsNullOrEmpty(nexusIsland.ZoneKey) || _existingIslands.ContainsKey(nexusIsland.ZoneKey))
						{
							hashSet.Add(nexusIsland);
						}
						else
						{
							_existingIslands.Add(nexusIsland.ZoneKey, nexusIsland);
						}
					}
				}
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
		}
		Dictionary<string, NexusZoneDetails> dictionary = Pool.Get<Dictionary<string, NexusZoneDetails>>();
		dictionary.Clear();
		foreach (NexusZoneDetails zone in ZoneClient.Nexus.Zones)
		{
			if (TryGetZoneStatus(zone.Key, out var status) && status.IsOnline)
			{
				dictionary.Add(zone.Key, zone);
			}
		}
		foreach (KeyValuePair<string, NexusZoneDetails> item in dictionary)
		{
			if (item.Key == ZoneKey)
			{
				continue;
			}
			if (!IsCloseTo(item.Value))
			{
				if (_existingIslands.TryGetValue(item.Key, out var value))
				{
					hashSet.Add(value);
				}
				continue;
			}
			var (val2, val3) = CalculateIslandTransform(item.Value);
			if (_existingIslands.TryGetValue(item.Key, out var value2) && (Object)(object)value2 != (Object)null)
			{
				((Component)value2).transform.SetPositionAndRotation(val2, val3);
			}
			else
			{
				BaseEntity baseEntity = GameManager.server.CreateEntity("assets/content/nexus/island/nexusisland.prefab", val2, val3);
				if (!(baseEntity is NexusIsland nexusIsland2))
				{
					baseEntity.Kill();
					Debug.LogError((object)"Failed to spawn nexus island entity!");
					continue;
				}
				nexusIsland2.ZoneKey = item.Key;
				nexusIsland2.Spawn();
				_existingIslands[item.Key] = nexusIsland2;
				value2 = nexusIsland2;
			}
			value2.SetFlag(BaseEntity.Flags.Reserved1, TryGetZoneStatus(item.Key, out var status2) && status2.IsFull);
		}
		foreach (KeyValuePair<string, NexusIsland> existingIsland in _existingIslands)
		{
			if (!dictionary.ContainsKey(existingIsland.Key))
			{
				hashSet.Add(existingIsland.Value);
			}
		}
		foreach (NexusIsland item2 in hashSet)
		{
			if (item2 != null)
			{
				if (item2.ZoneKey != null)
				{
					_existingIslands.Remove(item2.ZoneKey);
				}
				item2.Kill();
			}
		}
		hashSet.Clear();
		Pool.Free<HashSet<NexusIsland>>(ref hashSet);
		dictionary.Clear();
		Pool.Free<Dictionary<string, NexusZoneDetails>>(ref dictionary);
	}

	public static bool TryGetIsland(string zoneKey, out NexusIsland island)
	{
		if (_existingIslands == null)
		{
			island = null;
			return false;
		}
		if (_existingIslands.TryGetValue(zoneKey, out island))
		{
			return (Object)(object)island != (Object)null;
		}
		return false;
	}

	public static bool TryGetIslandPosition(string zoneKey, out Vector3 position)
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		NexusZoneDetails val = List.FindWith<NexusZoneDetails, string>((IReadOnlyCollection<NexusZoneDetails>)Zones, (Func<NexusZoneDetails, string>)((NexusZoneDetails z) => z.Key), zoneKey, (IEqualityComparer<string>)StringComparer.InvariantCultureIgnoreCase);
		if (val == null)
		{
			position = Vector3.zero;
			return false;
		}
		(position, _) = CalculateIslandTransform(val);
		return true;
	}

	private static (Vector3, Quaternion) CalculateIslandTransform(NexusZoneDetails otherZone)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		Bounds worldBounds = GetWorldBounds();
		float num = Mathf.Max(((Bounds)(ref worldBounds)).extents.x, ((Bounds)(ref worldBounds)).extents.z) * 1.5f;
		float num2 = Vector2Ex.AngleFromTo(ZoneClient.Zone.Position(), otherZone.Position());
		Vector3 val = TerrainMeta.Center + Quaternion.Euler(0f, num2, 0f) * Vector3.right * num;
		Vector3 val2 = Vector3Ex.WithY(((Bounds)(ref worldBounds)).ClosestPoint(val), TerrainMeta.Center.y);
		Vector3 val3 = TerrainMeta.Center - val2;
		Quaternion item = Quaternion.LookRotation(((Vector3)(ref val3)).normalized);
		return (Vector3Ex.WithY(val2, WaterSystem.OceanLevel), item);
	}

	public static Bounds GetWorldBounds()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = (((Object)(object)SingletonComponent<ValidBounds>.Instance != (Object)null) ? ((Bounds)(ref SingletonComponent<ValidBounds>.Instance.worldBounds)).extents : (Vector3.one * float.MaxValue));
		val.x = Mathf.Min(val.x, (float)World.Size * 1.5f);
		val.y = 0.01f;
		val.z = Mathf.Min(val.z, (float)World.Size * 1.5f);
		val.x = Mathf.Min((float)World.Size * Nexus.islandSpawnDistance, val.x * 0.9f);
		val.z = Mathf.Min((float)World.Size * Nexus.islandSpawnDistance, val.z * 0.9f);
		return new Bounds(Vector3.zero, val * 2f);
	}

	private static bool IsCloseTo(NexusZoneDetails otherZone)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		return Vector2.Distance(ZoneClient.Zone.Position(), otherZone.Position()) <= _zoneContactRadius;
	}

	private static void ReadIncomingMessages()
	{
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		NexusMessage val = default(NexusMessage);
		while (ZoneClient.TryReceiveMessage(ref val))
		{
			if (!((NexusMessage)(ref val)).IsBinary)
			{
				Debug.LogWarning((object)"Received a nexus message that's not binary, ignoring");
				ZoneClient.AcknowledgeMessage(ref val);
				continue;
			}
			byte[] asBinary;
			Packet val2;
			try
			{
				asBinary = ((NexusMessage)(ref val)).AsBinary;
				val2 = ReadPacket(asBinary);
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
				ZoneClient.AcknowledgeMessage(ref val);
				continue;
			}
			bool num = !RequiresJournaling(val2) || _database.SeenJournaled(Uuid.op_Implicit(((NexusMessage)(ref val)).Id), asBinary);
			ZoneClient.AcknowledgeMessage(ref val);
			if (!num)
			{
				Debug.LogWarning((object)"Already saw this nexus message, ignoring");
				val2.Dispose();
			}
			else
			{
				HandleMessage(((NexusMessage)(ref val)).Id, val2);
			}
		}
	}

	public static void RestoreUnsavedState()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		if (Started)
		{
			ReplayJournaledMessages();
			DeleteTransferredEntities();
			ConsoleSystem.Run(Option.Server, "server.save", Array.Empty<object>());
		}
	}

	private static void ReplayJournaledMessages()
	{
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		List<(Guid, long, byte[])> list = _database.ReadJournal();
		if (list.Count == 0)
		{
			Debug.Log((object)"No messages found in the nexus message journal");
			return;
		}
		Debug.Log((object)$"Replaying {list.Count} nexus messages from the journal");
		foreach (var (guid, seconds, data) in list)
		{
			try
			{
				Debug.Log((object)$"Replaying message ID {guid}, received {DateTimeOffset.FromUnixTimeSeconds(seconds):R}");
				Packet packet = ReadPacket(data);
				HandleMessage(Uuid.op_Implicit(guid), packet);
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}
		Debug.Log((object)$"Finished replaying {list.Count} nexus messages from the journal");
	}

	private static void DeleteTransferredEntities()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		List<NetworkableId> list = _database.ReadTransferred();
		if (list.Count == 0)
		{
			Debug.Log((object)"No entities found in the transferred list");
			return;
		}
		foreach (NetworkableId item in list)
		{
			try
			{
				BaseNetworkable baseNetworkable = BaseNetworkable.serverEntities.Find(item);
				if (!((Object)(object)baseNetworkable == (Object)null))
				{
					Debug.Log((object)$"Found {baseNetworkable}, killing it because it was transferred away");
					baseNetworkable.Kill();
				}
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}
		Debug.Log((object)$"Finished making sure {list.Count} entities do not exist");
	}

	private static bool RequiresJournaling(Packet packet)
	{
		if (packet.request == null || !packet.request.isFireAndForget)
		{
			return false;
		}
		return packet.request.transfer != null;
	}

	public static async void UploadMapImage(bool force = false)
	{
		_ = 1;
		try
		{
			int valueOrDefault = (World.Config?.JsonString?.GetHashCode()).GetValueOrDefault();
			string key = $"{2511}##{243}##{World.Name}##{World.Size}##{World.Seed}##{World.Salt}##{Nexus.mapImageScale}##{valueOrDefault}##{5}";
			if (!force && (await ZoneClient.CheckUploadedMap()).Key == key)
			{
				Debug.Log((object)"Nexus already has this map's image uploaded, will not render and upload again");
				return;
			}
			Debug.Log((object)"Rendering map image to upload to nexus...");
			int oceanMargin = 0;
			int imageWidth;
			int imageHeight;
			Color background;
			byte[] array = MapImageRenderer.Render(out imageWidth, out imageHeight, out background, Nexus.mapImageScale, lossy: false, transparent: true, oceanMargin);
			Debug.Log((object)"Uploading map image to nexus...");
			await ZoneClient.UploadMap(key, array);
			Debug.Log((object)"Map image was updated in the nexus");
		}
		catch (Exception ex)
		{
			Debug.LogException(ex);
		}
	}

	private static void HandleMessage(Uuid id, Packet packet)
	{
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if (packet.protocol != 243)
			{
				Debug.LogWarning((object)"Received a nexus message with wrong protocol, ignoring");
				return;
			}
			NexusZoneDetails val = List.FindWith<NexusZoneDetails, int>((IReadOnlyCollection<NexusZoneDetails>)ZoneClient.Nexus.Zones, (Func<NexusZoneDetails, int>)((NexusZoneDetails z) => z.Id), packet.sourceZone, (IEqualityComparer<int>)null);
			if (val == null)
			{
				Debug.LogWarning((object)$"Received a nexus message from unknown zone ID {packet.sourceZone}, ignoring");
			}
			else if (packet.request != null)
			{
				HandleRpcInvocation(val, id, packet.request);
			}
			else if (packet.response != null)
			{
				HandleRpcResponse(val, id, packet.response);
			}
			else
			{
				Debug.LogWarning((object)"Received a nexus message without the request or request sections set, ignoring");
			}
		}
		catch (Exception ex)
		{
			Debug.LogException(ex);
		}
		finally
		{
			if (packet != null)
			{
				packet.Dispose();
			}
		}
	}

	private static Packet ReadPacket(byte[] data)
	{
		ReaderStream.SetData(data, 0, data.Length);
		return Packet.Deserialize((Stream)(object)ReaderStream);
	}

	private static Task SendRequestImpl(Uuid id, Request request, string toZoneKey, int? ttl = null)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		Packet val = Pool.Get<Packet>();
		val.protocol = 243u;
		val.sourceZone = ZoneClient.Zone.ZoneId;
		val.request = request;
		return SendPacket(id, val, toZoneKey, ttl);
	}

	private static async void SendResponseImpl(Response response, string toZoneKey, int? ttl = null)
	{
		try
		{
			Packet val = Pool.Get<Packet>();
			val.protocol = 243u;
			val.sourceZone = ZoneClient.Zone.ZoneId;
			val.response = response;
			await SendPacket(Uuid.Generate(), val, toZoneKey, ttl);
		}
		catch (Exception ex)
		{
			Debug.LogException(ex);
		}
	}

	private static Task SendPacket(Uuid id, Packet packet, string toZoneKey, int? ttl = null)
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		WriterStream.SetLength(0L);
		WriterStream.Position = 0L;
		packet.WriteToStream((Stream)WriterStream);
		Memory<byte> val = default(Memory<byte>);
		val._002Ector(WriterStream.GetBuffer(), 0, (int)WriterStream.Length);
		packet.Dispose();
		return ZoneClient.SendMessage(toZoneKey, id, val, ttl);
	}

	public static bool IsOnline(ulong userId)
	{
		RebuildPlayerManifestIfDirty();
		if (!PlayerManifest.Contains(userId))
		{
			return ServerPlayers.IsOnline(userId);
		}
		return true;
	}

	public static void AddZonePlayerManifest(string zoneKey, List<ulong> userIds)
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		if (ZonePlayerManifests.TryGetValue(zoneKey, out var value))
		{
			if (value.UserIds != null)
			{
				Pool.FreeList<ulong>(ref value.UserIds);
			}
			ZonePlayerManifests.Remove(zoneKey);
		}
		ZonePlayerManifests.Add(zoneKey, new ZonePlayerManifest
		{
			Received = RealTimeSince.op_Implicit(0f),
			UserIds = List.ShallowClonePooled<ulong>(userIds)
		});
	}

	private static void UpdatePlayerManifest()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		if (RealTimeSince.op_Implicit(_lastPlayerManifestBroadcast) >= Nexus.playerManifestInterval)
		{
			_lastPlayerManifestBroadcast = RealTimeSince.op_Implicit(0f);
			BroadcastPlayerManifest();
		}
		if (RealTimeSince.op_Implicit(_lastPlayerManifestRebuild) > Nexus.playerManifestInterval)
		{
			_playerManifestDirty = true;
		}
		RebuildPlayerManifestIfDirty();
	}

	private static async void BroadcastPlayerManifest()
	{
		try
		{
			Request obj = Pool.Get<Request>();
			obj.isFireAndForget = true;
			obj.playerManifest = Pool.Get<PlayerManifestRequest>();
			obj.playerManifest.userIds = Pool.GetList<ulong>();
			ServerPlayers.GetAll(obj.playerManifest.userIds);
			await BroadcastRpc(obj);
		}
		catch (Exception ex)
		{
			Debug.LogException(ex);
		}
	}

	private static void RebuildPlayerManifestIfDirty()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		if (!_playerManifestDirty)
		{
			return;
		}
		_playerManifestDirty = false;
		_lastPlayerManifestRebuild = RealTimeSince.op_Implicit(0f);
		RemoveInvalidPlayerManifests();
		PlayerManifest.Clear();
		foreach (ZonePlayerManifest value in ZonePlayerManifests.Values)
		{
			foreach (ulong userId in value.UserIds)
			{
				PlayerManifest.Add(userId);
			}
		}
	}

	private static void RemoveInvalidPlayerManifests()
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		List<string> list = Pool.GetList<string>();
		foreach (KeyValuePair<string, ZonePlayerManifest> zonePlayerManifest in ZonePlayerManifests)
		{
			if (FindZone(zonePlayerManifest.Key) == null || RealTimeSince.op_Implicit(zonePlayerManifest.Value.Received) > Nexus.playerManifestInterval * 3f)
			{
				list.Add(zonePlayerManifest.Key);
			}
		}
		foreach (string item in list)
		{
			if (ZonePlayerManifests.TryGetValue(item, out var value))
			{
				ZonePlayerManifests.Remove(item);
				if (value.UserIds != null)
				{
					Pool.FreeList<ulong>(ref value.UserIds);
				}
			}
		}
		Pool.FreeList<string>(ref list);
	}

	public static async Task<Response> ZoneRpc(string zone, Request request, float timeoutAfter = 30f)
	{
		if (string.IsNullOrEmpty(zone))
		{
			throw new ArgumentNullException("zone");
		}
		if (string.Equals(zone, ZoneKey, StringComparison.InvariantCultureIgnoreCase))
		{
			return HandleRpcInvocationImpl(List.FindWith<NexusZoneDetails, string>((IReadOnlyCollection<NexusZoneDetails>)Zones, (Func<NexusZoneDetails, string>)((NexusZoneDetails z) => z.Key), ZoneKey, (IEqualityComparer<string>)null), Uuid.Empty, request);
		}
		using NexusRpcResult nexusRpcResult = await CallRpcImpl(zone, request, timeoutAfter, throwOnTimeout: true);
		Response val = nexusRpcResult.Responses[zone];
		if (!string.IsNullOrWhiteSpace(val.status?.errorMessage))
		{
			throw new Exception(val.status.errorMessage);
		}
		return val.Copy();
	}

	public static Task<NexusRpcResult> BroadcastRpc(Request request, float timeoutAfter = 30f)
	{
		return CallRpcImpl(null, request, timeoutAfter, throwOnTimeout: false);
	}

	private static async Task<NexusRpcResult> CallRpcImpl(string zone, Request request, float timeoutAfter, bool throwOnTimeout)
	{
		Uuid id = Uuid.Generate();
		TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
		NexusRpcResult result = Pool.Get<NexusRpcResult>();
		try
		{
			float actualTimeout = timeoutAfter * Nexus.rpcTimeoutMultiplier;
			await SendRequestImpl(id, request, zone, (int)actualTimeout + RpcResponseTtl);
			PendingCalls.Add(id, new PendingCall
			{
				IsBroadcast = string.IsNullOrWhiteSpace(zone),
				TimeUntilTimeout = RealTimeUntil.op_Implicit(actualTimeout),
				Completion = tcs,
				Result = result
			});
			bool flag = await tcs.Task;
			if (throwOnTimeout && !flag)
			{
				throw new TimeoutException("Nexus RPC invocation timed out");
			}
		}
		catch
		{
			Pool.Free<NexusRpcResult>(ref result);
			throw;
		}
		return result;
	}

	private static void HandleRpcInvocation(NexusZoneDetails from, Uuid id, Request request)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		Response val = HandleRpcInvocationImpl(from, id, request);
		if (val != null)
		{
			SendResponseImpl(val, from.Key, RpcResponseTtl);
		}
	}

	private static Response HandleRpcInvocationImpl(NexusZoneDetails from, Uuid id, Request request)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		if (Handle<TransferRequest, TransferHandler>((Request r) => r.transfer, out var requestHandler2) || Handle<PingRequest, PingHandler>((Request r) => r.ping, out requestHandler2) || Handle<SpawnOptionsRequest, SpawnOptionsHandler>((Request r) => r.spawnOptions, out requestHandler2) || Handle<SleepingBagRespawnRequest, RespawnAtBagHandler>((Request r) => r.respawnAtBag, out requestHandler2) || Handle<SleepingBagDestroyRequest, DestroyBagHandler>((Request r) => r.destroyBag, out requestHandler2) || Handle<FerryStatusRequest, FerryStatusHandler>((Request r) => r.ferryStatus, out requestHandler2) || Handle<FerryRetireRequest, FerryRetireHandler>((Request r) => r.ferryRetire, out requestHandler2) || Handle<FerryUpdateScheduleRequest, FerryUpdateScheduleHandler>((Request r) => r.ferryUpdateSchedule, out requestHandler2) || Handle<ClanChatBatchRequest, ClanChatBatchHandler>((Request r) => r.clanChatBatch, out requestHandler2) || Handle<PlayerManifestRequest, PlayerManifestHandler>((Request r) => r.playerManifest, out requestHandler2))
		{
			requestHandler2.Execute();
			Response response = requestHandler2.Response;
			Pool.FreeDynamic<INexusRequestHandler>(ref requestHandler2);
			return response;
		}
		Debug.LogError((object)"Received a nexus RPC invocation with a missing or unsupported request, ignoring");
		return null;
		bool Handle<TProto, THandler>(Func<Request, TProto> protoSelector, out INexusRequestHandler requestHandler) where TProto : class where THandler : BaseNexusRequestHandler<TProto>, new()
		{
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			TProto val = protoSelector(request);
			if (val == null)
			{
				requestHandler = null;
				return false;
			}
			THandler val2 = Pool.Get<THandler>();
			val2.Initialize(from, id, request.isFireAndForget, val);
			requestHandler = val2;
			return true;
		}
	}

	private static void HandleRpcResponse(NexusZoneDetails from, Uuid id, Response response)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		if (!PendingCalls.TryGetValue(response.id, out var value))
		{
			Debug.LogWarning((object)"Received an unexpected nexus RPC response (likely timed out), ignoring");
			return;
		}
		if (!value.Result.Responses.ContainsKey(from.Key))
		{
			value.Result.Responses.Add(from.Key, response.Copy());
		}
		int num;
		if (!value.IsBroadcast)
		{
			num = 1;
		}
		else
		{
			NexusZoneClient zoneClient = ZoneClient;
			int? obj;
			if (zoneClient == null)
			{
				obj = null;
			}
			else
			{
				NexusDetails nexus = zoneClient.Nexus;
				obj = ((nexus == null) ? null : nexus.Zones?.Count);
			}
			int? num2 = obj;
			num = num2.GetValueOrDefault() - 1;
		}
		int num3 = num;
		if (value.Result.Responses.Count >= num3)
		{
			PendingCalls.Remove(id);
			value.Completion.TrySetResult(result: true);
		}
	}

	private static void CheckForRpcTimeouts()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		if (RealTimeSince.op_Implicit(_sinceLastRpcTimeoutCheck) < 1f)
		{
			return;
		}
		_sinceLastRpcTimeoutCheck = RealTimeSince.op_Implicit(0f);
		List<(Uuid, PendingCall)> list = Pool.GetList<(Uuid, PendingCall)>();
		foreach (KeyValuePair<Uuid, PendingCall> pendingCall in PendingCalls)
		{
			Uuid key = pendingCall.Key;
			PendingCall value = pendingCall.Value;
			if (RealTimeUntil.op_Implicit(value.TimeUntilTimeout) <= 0f)
			{
				list.Add((key, value));
			}
		}
		foreach (var item3 in list)
		{
			Uuid item = item3.Item1;
			PendingCall item2 = item3.Item2;
			PendingCalls.Remove(item);
			item2.Completion.TrySetResult(result: false);
		}
		Pool.FreeList<(Uuid, PendingCall)>(ref list);
	}

	private static void RefreshZoneStatus()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		if (!_isRefreshingZoneStatus && !(RealTimeSince.op_Implicit(_lastZoneStatusRefresh) < Nexus.pingInterval))
		{
			RefreshZoneStatusImpl();
		}
		static async void RefreshZoneStatusImpl()
		{
			try
			{
				_isRefreshingZoneStatus = true;
				_lastZoneStatusRefresh = RealTimeSince.op_Implicit(0f);
				Request obj = Pool.Get<Request>();
				obj.ping = Pool.Get<PingRequest>();
				using (NexusRpcResult nexusRpcResult = await BroadcastRpc(obj))
				{
					List<string> list = Pool.GetList<string>();
					foreach (string key in ZoneStatuses.Keys)
					{
						if (List.FindWith<NexusZoneDetails, string>((IReadOnlyCollection<NexusZoneDetails>)Zones, (Func<NexusZoneDetails, string>)((NexusZoneDetails z) => z.Key), key, (IEqualityComparer<string>)null) == null)
						{
							list.Add(key);
						}
					}
					foreach (string item in list)
					{
						ZoneStatuses.Remove(item);
					}
					Pool.FreeList<string>(ref list);
					foreach (KeyValuePair<string, Response> response in nexusRpcResult.Responses)
					{
						if (string.IsNullOrWhiteSpace(response.Key))
						{
							Debug.LogWarning((object)"Received a ping response for a zone with a null key");
						}
						else if (response.Value?.ping == null)
						{
							Debug.LogWarning((object)("Received a ping response from '" + response.Key + "' but the data was null"));
						}
						else
						{
							ZoneStatuses[response.Key] = new ServerStatus
							{
								IsOnline = true,
								LastSeen = RealTimeSince.op_Implicit(0f),
								Players = response.Value.ping.players,
								MaxPlayers = response.Value.ping.maxPlayers,
								QueuedPlayers = response.Value.ping.queuedPlayers
							};
						}
					}
					foreach (NexusZoneDetails zone in Zones)
					{
						if (!nexusRpcResult.Responses.ContainsKey(zone.Key))
						{
							if (ZoneStatuses.TryGetValue(zone.Key, out var value))
							{
								ZoneStatuses[zone.Key] = new ServerStatus
								{
									IsOnline = false,
									LastSeen = value.LastSeen,
									Players = value.Players,
									MaxPlayers = value.MaxPlayers,
									QueuedPlayers = value.QueuedPlayers
								};
							}
							else
							{
								ZoneStatuses[zone.Key] = new ServerStatus
								{
									IsOnline = false
								};
							}
						}
					}
				}
				_lastZoneStatusRefresh = RealTimeSince.op_Implicit(0f);
			}
			finally
			{
				_isRefreshingZoneStatus = false;
			}
			OnZoneStatusesRefreshed();
		}
	}

	public static bool TryGetZoneStatus(string zone, out ServerStatus status)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		if (!Started)
		{
			status = default(ServerStatus);
			return false;
		}
		if (string.Equals(zone, ZoneKey, StringComparison.InvariantCultureIgnoreCase))
		{
			status = new ServerStatus
			{
				IsOnline = true,
				LastSeen = RealTimeSince.op_Implicit(0f),
				Players = BasePlayer.activePlayerList.Count,
				MaxPlayers = ConVar.Server.maxplayers,
				QueuedPlayers = SingletonComponent<ServerMgr>.Instance.connectionQueue.Queued
			};
			return true;
		}
		return ZoneStatuses.TryGetValue(zone, out status);
	}

	private static void OnZoneStatusesRefreshed()
	{
		UpdateIslands();
		UpdateFerries();
	}

	public static async Task TransferEntity(BaseEntity entity, string toZoneKey, string method)
	{
		try
		{
			await TransferEntityImpl(FindRootEntity(entity), toZoneKey, method, ZoneKey, toZoneKey);
		}
		catch (Exception ex)
		{
			Debug.LogWarning((object)ex);
		}
	}

	public static async Task TransferEntityImpl(BaseEntity rootEntity, string toZoneKey, string method, string from, string to)
	{
		if ((Object)(object)rootEntity == (Object)null)
		{
			throw new ArgumentNullException("rootEntity");
		}
		if (string.IsNullOrWhiteSpace(toZoneKey))
		{
			throw new ArgumentNullException("toZoneKey");
		}
		if (string.Equals(toZoneKey, ZoneKey, StringComparison.InvariantCultureIgnoreCase))
		{
			throw new ArgumentException("Attempted to transfer a player to the current server's zone", "toZoneKey");
		}
		NexusZoneDetails toZone = List.FindWith<NexusZoneDetails, string>((IReadOnlyCollection<NexusZoneDetails>)ZoneClient.Nexus.Zones, (Func<NexusZoneDetails, string>)((NexusZoneDetails z) => z.Key), toZoneKey, (IEqualityComparer<string>)StringComparer.InvariantCultureIgnoreCase);
		if (toZone == null)
		{
			throw new ArgumentException("Target zone (" + toZoneKey + ") was not found in the nexus", "toZoneKey");
		}
		BuildTransferRequest(rootEntity, method, from, to, out var request, out var networkables, out var players, out var playerIds);
		HashSet<NetworkableId> transferEntityIds = Pool.Get<HashSet<NetworkableId>>();
		transferEntityIds.Clear();
		foreach (BaseNetworkable item in networkables)
		{
			if (item.net != null && ((NetworkableId)(ref item.net.ID)).IsValid)
			{
				transferEntityIds.Add(item.net.ID);
			}
		}
		foreach (BaseNetworkable item2 in networkables)
		{
			if (item2.net != null && ((NetworkableId)(ref item2.net.ID)).IsValid)
			{
				transferEntityIds.Add(item2.net.ID);
			}
			if (item2 is BaseEntity baseEntity)
			{
				baseEntity.SetFlag(BaseEntity.Flags.Transferring, b: true);
			}
		}
		try
		{
			if (playerIds.Count > 0)
			{
				await ZoneClient.RegisterTransfers(toZoneKey, (IEnumerable<string>)playerIds);
			}
			await SendRequestImpl(Uuid.Generate(), request, toZoneKey);
		}
		catch
		{
			foreach (BaseNetworkable item3 in networkables)
			{
				if ((Object)(object)item3 != (Object)null && item3 is BaseEntity baseEntity2)
				{
					baseEntity2.SetFlag(BaseEntity.Flags.Transferring, b: false);
				}
			}
			throw;
		}
		foreach (BasePlayer item4 in players)
		{
			if ((Object)(object)item4 != (Object)null && item4.IsConnected)
			{
				ConsoleNetwork.SendClientCommandImmediate(item4.net.connection, "nexus.redirect", toZone.IpAddress, toZone.GamePort, toZone.ConnectionProtocol());
				item4.Kick("Redirecting to another zone...");
			}
		}
		for (int num = networkables.Count - 1; num >= 0; num--)
		{
			try
			{
				BaseNetworkable baseNetworkable = networkables[num];
				if ((Object)(object)baseNetworkable != (Object)null)
				{
					if (baseNetworkable is BaseEntity entity)
					{
						UnparentUnknown(entity, transferEntityIds);
					}
					baseNetworkable.Kill();
				}
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}
		_database.MarkTransferred(transferEntityIds);
		transferEntityIds.Clear();
		Pool.Free<HashSet<NetworkableId>>(ref transferEntityIds);
		Pool.FreeList<BaseNetworkable>(ref networkables);
		Pool.FreeList<BasePlayer>(ref players);
		Pool.FreeList<string>(ref playerIds);
		_lastUnsavedTransfer = DateTimeOffset.UtcNow;
	}

	private static void UnparentUnknown(BaseEntity entity, HashSet<NetworkableId> knownEntityIds)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		List<BaseEntity> list = Pool.GetList<BaseEntity>();
		foreach (BaseEntity child in entity.children)
		{
			if (knownEntityIds.Contains(child.net.ID))
			{
				UnparentUnknown(child, knownEntityIds);
			}
			else
			{
				list.Add(child);
			}
		}
		foreach (BaseEntity item in list)
		{
			Debug.Log((object)$"Unparenting {entity}", (Object)(object)entity);
			item.SetParent(null, worldPositionStays: true, sendImmediate: true);
		}
		Pool.FreeList<BaseEntity>(ref list);
	}

	private static void BuildTransferRequest(BaseEntity rootEntity, string method, string from, string to, out Request request, out List<BaseNetworkable> networkables, out List<BasePlayer> players, out List<string> playerIds)
	{
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		List<BaseNetworkable> entitiesList = (networkables = Pool.GetList<BaseNetworkable>());
		List<BasePlayer> playerList = (players = Pool.GetList<BasePlayer>());
		List<string> playerIdsList = (playerIds = Pool.GetList<string>());
		request = Pool.Get<Request>();
		request.isFireAndForget = true;
		request.transfer = Pool.Get<TransferRequest>();
		request.transfer.method = method;
		request.transfer.from = from;
		request.transfer.to = to;
		List<Entity> serializedEntities = (request.transfer.entities = Pool.GetList<Entity>());
		List<PlayerSecondaryData> secondaryData = (request.transfer.secondaryData = Pool.GetList<PlayerSecondaryData>());
		Queue<BaseNetworkable> pendingEntities = Pool.Get<Queue<BaseNetworkable>>();
		pendingEntities.Clear();
		HashSet<NetworkableId> seenEntityIds = Pool.Get<HashSet<NetworkableId>>();
		seenEntityIds.Clear();
		pendingEntities.Enqueue(rootEntity);
		seenEntityIds.Add(rootEntity.net.ID);
		while (pendingEntities.Count > 0)
		{
			BaseNetworkable baseNetworkable = pendingEntities.Dequeue();
			Entity val = null;
			if (CanTransferEntity(baseNetworkable))
			{
				val = AddEntity(baseNetworkable);
			}
			foreach (BaseEntity child in baseNetworkable.children)
			{
				if ((Object)(object)child != (Object)null && seenEntityIds.Add(child.net.ID))
				{
					pendingEntities.Enqueue(child);
				}
			}
			if (baseNetworkable is BaseMountable baseMountable)
			{
				BasePlayer mounted = baseMountable.GetMounted();
				if ((Object)(object)mounted != (Object)null && seenEntityIds.Add(mounted.net.ID))
				{
					pendingEntities.Enqueue(mounted);
				}
			}
			if (val != null)
			{
				val.InspectUids((UidInspector<ulong>)ScanForAdditionalEntities);
			}
		}
		seenEntityIds.Clear();
		Pool.Free<HashSet<NetworkableId>>(ref seenEntityIds);
		pendingEntities.Clear();
		Pool.Free<Queue<BaseNetworkable>>(ref pendingEntities);
		Entity AddEntity(BaseNetworkable entity)
		{
			BaseNetworkable.SaveInfo saveInfo = default(BaseNetworkable.SaveInfo);
			saveInfo.forDisk = true;
			saveInfo.forTransfer = true;
			saveInfo.msg = Pool.Get<Entity>();
			BaseNetworkable.SaveInfo info = saveInfo;
			entity.Save(info);
			serializedEntities.Add(info.msg);
			entitiesList.Add(entity);
			if (entity is BasePlayer basePlayer && ((object)basePlayer).GetType() == typeof(BasePlayer) && basePlayer.userID > uint.MaxValue)
			{
				playerList.Add(basePlayer);
				playerIdsList.Add(basePlayer.UserIDString);
				secondaryData.Add(basePlayer.SaveSecondaryData());
			}
			return info.msg;
		}
		void ScanForAdditionalEntities(UidType type, ref ulong uid)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			NetworkableId val2 = default(NetworkableId);
			((NetworkableId)(ref val2))._002Ector(uid);
			if ((int)type == 0 && ((NetworkableId)(ref val2)).IsValid && seenEntityIds.Add(val2))
			{
				BaseNetworkable baseNetworkable2 = BaseNetworkable.serverEntities.Find(val2);
				if ((Object)(object)baseNetworkable2 != (Object)null)
				{
					pendingEntities.Enqueue(baseNetworkable2);
				}
			}
		}
	}

	private static bool CanTransferEntity(BaseNetworkable networkable)
	{
		if ((Object)(object)networkable == (Object)null)
		{
			return false;
		}
		if (networkable is BaseEntity baseEntity && !baseEntity.enableSaving)
		{
			return false;
		}
		return true;
	}

	public static BaseEntity FindRootEntity(BaseEntity startEntity)
	{
		BaseEntity baseEntity = startEntity;
		BaseEntity parent2;
		while (TryGetParent(baseEntity, out parent2))
		{
			baseEntity = parent2;
		}
		return baseEntity;
		static bool TryGetParent(BaseEntity entity, out BaseEntity parent)
		{
			BaseEntity parentEntity = entity.GetParentEntity();
			if ((Object)(object)parentEntity != (Object)null && !(parentEntity is NexusFerry))
			{
				parent = parentEntity;
				return true;
			}
			if (entity is BasePlayer basePlayer)
			{
				BaseMountable mounted = basePlayer.GetMounted();
				if ((Object)(object)mounted != (Object)null)
				{
					parent = mounted;
					return true;
				}
			}
			parent = null;
			return false;
		}
	}

	private static string GetWorldConfigString()
	{
		List<string> list = Pool.GetList<string>();
		JObject worldConfigImpl = GetWorldConfigImpl(ZoneKey, list);
		Pool.FreeList<string>(ref list);
		if (worldConfigImpl == null)
		{
			return null;
		}
		return ((JToken)worldConfigImpl).ToString((Formatting)0, Array.Empty<JsonConverter>());
	}

	private static JObject GetWorldConfigImpl(string zoneKey, List<string> stack)
	{
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Invalid comparison between Unknown and I4
		if (stack.Count > 20)
		{
			throw new Exception("Cannot load world config from nexus - there is a cyclic dependency between zones (" + string.Join(" -> ", stack) + ")");
		}
		bool required = stack.Count > 0;
		if (!TryGetWorldConfigObject(zoneKey, required, out var cfg, out var error))
		{
			throw new Exception(error + " (" + string.Join(" -> ", stack) + ")");
		}
		JToken val = default(JToken);
		if (!cfg.TryGetValue("$copyFrom", ref val))
		{
			return cfg;
		}
		if ((int)val.Type != 8)
		{
			throw new Exception("Cannot get world config from nexus - zone '" + zoneKey + "' has a $copyFrom, but its value is not a string");
		}
		stack.Add(zoneKey);
		JObject obj = MergeInto(GetWorldConfigImpl(val.ToObject<string>(), stack), cfg);
		obj.Remove("$copyFrom");
		return obj;
	}

	private static bool TryGetWorldConfigObject(string zoneKey, bool required, out JObject cfg, out string error)
	{
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Invalid comparison between Unknown and I4
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Expected O, but got Unknown
		cfg = null;
		NexusZoneClient zoneClient = ZoneClient;
		object obj;
		if (zoneClient == null)
		{
			obj = null;
		}
		else
		{
			NexusDetails nexus = zoneClient.Nexus;
			obj = ((nexus != null) ? nexus.Zones : null);
		}
		if (obj == null)
		{
			error = "Cannot get world config from nexus - nexus server isn't started";
			return false;
		}
		NexusZoneDetails val = FindZone(zoneKey);
		if (val == null)
		{
			error = "Cannot get world config for nexus zone '" + zoneKey + "' - zone was not found";
			return false;
		}
		if (!((Dictionary<string, VariableData>)(object)val.Variables).TryGetValue("world.cfg", out VariableData value))
		{
			if (required)
			{
				error = "Cannot get world config for nexus zone '" + zoneKey + "' - world.cfg variable not found but is required by another zone";
				return false;
			}
			cfg = new JObject();
			error = null;
			return true;
		}
		if ((int)((VariableData)(ref value)).Type != 1 || string.IsNullOrWhiteSpace(((VariableData)(ref value)).Value))
		{
			error = "Cannot get world config for nexus zone '" + zoneKey + "' - world.cfg variable is empty or not a string";
			return false;
		}
		try
		{
			cfg = JObject.Parse(((VariableData)(ref value)).Value);
			error = null;
			return true;
		}
		catch (Exception ex)
		{
			error = "Cannot get world config for nexus zone '" + zoneKey + "' - failed to parse: `" + ((VariableData)(ref value)).Value + "` (" + ex.Message + ")";
			return false;
		}
	}

	private static JObject MergeInto(JObject baseObject, JObject sourceObject)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		JObject val = new JObject(baseObject);
		foreach (KeyValuePair<string, JToken> item in sourceObject)
		{
			val[item.Key] = item.Value;
		}
		return val;
	}
}
