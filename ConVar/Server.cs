using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Epic.OnlineServices.Logging;
using Epic.OnlineServices.Reports;
using Facepunch.Extend;
using Network;
using UnityEngine;

namespace ConVar;

[Factory("server")]
public class Server : ConsoleSystem
{
	[ServerVar]
	public static string ip = "";

	[ServerVar]
	public static int port = 28015;

	[ServerVar]
	public static int queryport = 0;

	[ServerVar(ShowInAdminUI = true)]
	public static int maxplayers = 500;

	[ServerVar(ShowInAdminUI = true)]
	public static string hostname = "My Untitled Rust Server";

	[ServerVar]
	public static string identity = "my_server_identity";

	[ServerVar]
	public static string level = "Procedural Map";

	[ServerVar]
	public static string levelurl = "";

	[ServerVar]
	public static bool leveltransfer = true;

	[ServerVar]
	public static int seed = 1337;

	[ServerVar]
	public static int salt = 1;

	[ServerVar]
	public static int worldsize = 4500;

	[ServerVar]
	public static int saveinterval = 600;

	[ServerVar]
	public static bool secure = true;

	[ServerVar]
	public static int encryption = 2;

	[ServerVar]
	public static string anticheatid = "xyza7891h6UjNfd0eb2HQGtaul0WhfvS";

	[ServerVar]
	public static string anticheatkey = "OWUDFZmi9VNL/7VhGVSSmCWALKTltKw8ISepa0VXs60";

	[ServerVar]
	public static bool anticheattoken = true;

	[ServerVar]
	public static int tickrate = 10;

	[ServerVar]
	public static int entityrate = 16;

	[ServerVar]
	public static float schematime = 1800f;

	[ServerVar]
	public static float cycletime = 500f;

	[ServerVar]
	public static bool official = false;

	[ServerVar]
	public static bool stats = false;

	[ServerVar]
	public static bool stability = true;

	[ServerVar(ShowInAdminUI = true)]
	public static bool radiation = true;

	[ServerVar]
	public static float itemdespawn = 300f;

	[ServerVar]
	public static float itemdespawn_container_scale = 2f;

	[ServerVar]
	public static float itemdespawn_quick = 30f;

	[ServerVar]
	public static float corpsedespawn = 300f;

	[ServerVar]
	public static float debrisdespawn = 30f;

	[ServerVar]
	public static bool pve = false;

	[ServerVar]
	public static bool cinematic = false;

	[ServerVar(ShowInAdminUI = true)]
	public static string description = "No server description has been provided.";

	[ServerVar(ShowInAdminUI = true)]
	public static string url = "";

	[ServerVar]
	public static string branch = "";

	[ServerVar]
	public static int queriesPerSecond = 2000;

	[ServerVar]
	public static int ipQueriesPerMin = 30;

	[ServerVar]
	public static bool statBackup = false;

	[ServerVar(Saved = true, ShowInAdminUI = true)]
	public static string headerimage = "";

	[ServerVar(Saved = true, ShowInAdminUI = true)]
	public static string logoimage = "";

	[ServerVar(Saved = true, ShowInAdminUI = true)]
	public static int saveBackupCount = 2;

	[ReplicatedVar(Saved = true, ShowInAdminUI = true)]
	public static string motd = "";

	[ServerVar(Saved = true)]
	public static float meleedamage = 1f;

	[ServerVar(Saved = true)]
	public static float arrowdamage = 1f;

	[ServerVar(Saved = true)]
	public static float bulletdamage = 1f;

	[ServerVar(Saved = true)]
	public static float bleedingdamage = 1f;

	[ReplicatedVar(Saved = true)]
	public static float funWaterDamageThreshold = 0.8f;

	[ReplicatedVar(Saved = true)]
	public static float funWaterWetnessGain = 0.05f;

	[ServerVar(Saved = true)]
	public static float meleearmor = 1f;

	[ServerVar(Saved = true)]
	public static float arrowarmor = 1f;

	[ServerVar(Saved = true)]
	public static float bulletarmor = 1f;

	[ServerVar(Saved = true)]
	public static float bleedingarmor = 1f;

	[ServerVar]
	public static int updatebatch = 512;

	[ServerVar]
	public static int updatebatchspawn = 1024;

	[ServerVar]
	public static int entitybatchsize = 100;

	[ServerVar]
	public static float entitybatchtime = 1f;

	[ServerVar]
	public static float composterUpdateInterval = 300f;

	[ReplicatedVar]
	public static float planttick = 60f;

	[ServerVar]
	public static float planttickscale = 1f;

	[ServerVar]
	public static bool useMinimumPlantCondition = true;

	[ServerVar(Saved = true)]
	public static float nonPlanterDeathChancePerTick = 0.005f;

	[ServerVar(Saved = true)]
	public static float ceilingLightGrowableRange = 3f;

	[ServerVar(Saved = true)]
	public static float artificialTemperatureGrowableRange = 4f;

	[ServerVar(Saved = true)]
	public static float ceilingLightHeightOffset = 3f;

	[ServerVar(Saved = true)]
	public static float sprinklerRadius = 3f;

	[ServerVar(Saved = true)]
	public static float sprinklerEyeHeightOffset = 3f;

	[ServerVar(Saved = true)]
	public static float optimalPlanterQualitySaturation = 0.6f;

	[ServerVar]
	public static float metabolismtick = 1f;

	[ServerVar]
	public static float modifierTickRate = 1f;

	[ServerVar(Saved = true)]
	public static float rewounddelay = 60f;

	[ServerVar(Saved = true, Help = "Can players be wounded after recieving fatal damage")]
	public static bool woundingenabled = true;

	[ServerVar(Saved = true, Help = "Do players go into the crawling wounded state")]
	public static bool crawlingenabled = true;

	[ServerVar(Help = "Base chance of recovery after crawling wounded state", Saved = true)]
	public static float woundedrecoverchance = 0.2f;

	[ServerVar(Help = "Base chance of recovery after incapacitated wounded state", Saved = true)]
	public static float incapacitatedrecoverchance = 0.1f;

	[ServerVar(Help = "Maximum percent chance added to base wounded/incapacitated recovery chance, based on the player's food and water level", Saved = true)]
	public static float woundedmaxfoodandwaterbonus = 0.25f;

	[ServerVar(Help = "Minimum initial health given when a player dies and moves to crawling wounded state", Saved = false)]
	public static int crawlingminimumhealth = 7;

	[ServerVar(Help = "Maximum initial health given when a player dies and moves to crawling wounded state", Saved = false)]
	public static int crawlingmaximumhealth = 12;

	[ServerVar(Saved = true)]
	public static bool playerserverfall = true;

	[ServerVar]
	public static bool plantlightdetection = true;

	[ServerVar]
	public static float respawnresetrange = 50f;

	[ReplicatedVar]
	public static int max_sleeping_bags = 15;

	[ReplicatedVar]
	public static bool bag_quota_item_amount = true;

	[ServerVar]
	public static int maxunack = 4;

	[ServerVar]
	public static bool netcache = true;

	[ServerVar]
	public static bool corpses = true;

	[ServerVar]
	public static bool events = true;

	[ServerVar]
	public static bool dropitems = true;

	[ServerVar]
	public static int netcachesize = 0;

	[ServerVar]
	public static int savecachesize = 0;

	[ServerVar]
	public static int combatlogsize = 30;

	[ServerVar]
	public static int combatlogdelay = 10;

	[ServerVar]
	public static int authtimeout = 60;

	[ServerVar]
	public static int playertimeout = 60;

	[ServerVar(ShowInAdminUI = true)]
	public static int idlekick = 30;

	[ServerVar]
	public static int idlekickmode = 1;

	[ServerVar]
	public static int idlekickadmins = 0;

	[ServerVar]
	public static string gamemode = "";

	[ServerVar(Help = "Comma-separated server browser tag values (see wiki)", Saved = true, ShowInAdminUI = true)]
	public static string tags = "";

	[ServerVar(Help = "Censors the Steam player list to make player tracking more difficult")]
	public static bool censorplayerlist = true;

	[ServerVar(Help = "HTTP API endpoint for centralized banning (see wiki)")]
	public static string bansServerEndpoint = "";

	[ServerVar(Help = "Failure mode for centralized banning, set to 1 to reject players from joining if it's down (see wiki)")]
	public static int bansServerFailureMode = 0;

	[ServerVar(Help = "Timeout (in seconds) for centralized banning web server requests")]
	public static int bansServerTimeout = 5;

	[ServerVar(Help = "HTTP API endpoint for receiving F7 reports", Saved = true)]
	public static string reportsServerEndpoint = "";

	[ServerVar(Help = "If set, this key will be included with any reports sent via reportsServerEndpoint (for validation)", Saved = true)]
	public static string reportsServerEndpointKey = "";

	[ServerVar(Help = "Should F7 reports from players be printed to console", Saved = true)]
	public static bool printReportsToConsole = false;

	[ServerVar(Help = "If a player presses the respawn button, respawn at their death location (for trailer filming)")]
	public static bool respawnAtDeathPosition = false;

	[ServerVar(Help = "When a player respawns give them the loadout assigned to client.RespawnLoadout (created with inventory.saveloadout)")]
	public static bool respawnWithLoadout = false;

	[ServerVar(Help = "When transferring water, should containers keep 1 water behind. Enabling this should help performance if water IO is causing performance loss", Saved = true)]
	public static bool waterContainersLeaveWaterBehind = false;

	[ServerVar(Help = "How often industrial conveyors attempt to move items (value is an interval measured in seconds). Setting to 0 will disable all movement", Saved = true, ShowInAdminUI = true)]
	public static float conveyorMoveFrequency = 5f;

	[ServerVar(Help = "How often industrial crafters attempt to craft items (value is an interval measured in seconds). Setting to 0 will disable all crafting", Saved = true, ShowInAdminUI = true)]
	public static float industrialCrafterFrequency = 5f;

	[ReplicatedVar(Help = "How much scrap is required to research default blueprints", Saved = true, ShowInAdminUI = true)]
	public static int defaultBlueprintResearchCost = 10;

	[ServerVar(Help = "Whether to check for illegal industrial pipes when changing building block states (roof bunkers)", Saved = true, ShowInAdminUI = true)]
	public static bool enforcePipeChecksOnBuildingBlockChanges = true;

	[ServerVar(Help = "How many stacks a single conveyor can move in a single tick", Saved = true, ShowInAdminUI = true)]
	public static int maxItemStacksMovedPerTickIndustrial = 12;

	[ServerVar(Help = "How long per frame to spend on industrial jobs", Saved = true, ShowInAdminUI = true)]
	public static float industrialFrameBudgetMs = 0.5f;

	[ReplicatedVar(Help = "How many markers each player can place", Saved = true, ShowInAdminUI = true)]
	public static int maximumMapMarkers = 5;

	[ServerVar(Help = "How many pings can be placed by each player", Saved = true, ShowInAdminUI = true)]
	public static int maximumPings = 5;

	[ServerVar(Help = "How long a ping should last", Saved = true, ShowInAdminUI = true)]
	public static float pingDuration = 10f;

	[ServerVar(Help = "Allows backpack equipping while not grounded", Saved = true, ShowInAdminUI = true)]
	public static bool canEquipBackpacksInAir = false;

	[ReplicatedVar(Help = "How long it takes to pick up a used parachute in seconds", Saved = true, ShowInAdminUI = true)]
	public static float parachuteRepackTime = 8f;

	public static bool emojiOwnershipCheck = false;

	[ServerVar(Saved = true)]
	public static bool showHolsteredItems = true;

	[ServerVar]
	public static int maxpacketspersecond_world = 1;

	[ServerVar]
	public static int maxpacketspersecond_rpc = 200;

	[ServerVar]
	public static int maxpacketspersecond_rpc_signal = 50;

	[ServerVar]
	public static int maxpacketspersecond_command = 100;

	[ServerVar]
	public static int maxpacketsize_command = 100000;

	[ServerVar]
	public static int maxpacketsize_globaltrees = 100;

	[ServerVar]
	public static int maxpacketsize_globalentities = 1000;

	[ServerVar]
	public static int maxpacketspersecond_tick = 300;

	[ServerVar]
	public static int maxpacketspersecond_voice = 100;

	[ServerVar]
	public static bool packetlog_enabled = false;

	[ServerVar]
	public static bool rpclog_enabled = false;

	[ServerVar]
	public static int anticheatlog
	{
		get
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected I4, but got Unknown
			return (int)EOS.LogLevel;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			EOS.LogLevel = (LogLevel)value;
		}
	}

	[ServerVar]
	public static int maxclientinfosize
	{
		get
		{
			return Connection.MaxClientInfoSize;
		}
		set
		{
			Connection.MaxClientInfoSize = Mathf.Max(value, 1);
		}
	}

	[ServerVar]
	public static int maxconnectionsperip
	{
		get
		{
			return Server.MaxConnectionsPerIP;
		}
		set
		{
			Server.MaxConnectionsPerIP = Mathf.Clamp(value, 1, 1000);
		}
	}

	[ServerVar]
	public static int maxreceivetime
	{
		get
		{
			return Server.MaxReceiveTime;
		}
		set
		{
			Server.MaxReceiveTime = Mathf.Clamp(value, 10, 1000);
		}
	}

	[ServerVar]
	public static int maxmainthreadwait
	{
		get
		{
			return Server.MaxMainThreadWait;
		}
		set
		{
			Server.MaxMainThreadWait = Mathf.Clamp(value, 1, 1000);
		}
	}

	[ServerVar]
	public static int maxreadthreadwait
	{
		get
		{
			return Server.MaxReadThreadWait;
		}
		set
		{
			Server.MaxReadThreadWait = Mathf.Clamp(value, 1, 1000);
		}
	}

	[ServerVar]
	public static int maxwritethreadwait
	{
		get
		{
			return Server.MaxWriteThreadWait;
		}
		set
		{
			Server.MaxWriteThreadWait = Mathf.Clamp(value, 1, 1000);
		}
	}

	[ServerVar]
	public static int maxdecryptthreadwait
	{
		get
		{
			return Server.MaxDecryptThreadWait;
		}
		set
		{
			Server.MaxDecryptThreadWait = Mathf.Clamp(value, 1, 1000);
		}
	}

	[ServerVar]
	public static int maxreadqueuelength
	{
		get
		{
			return Server.MaxReadQueueLength;
		}
		set
		{
			Server.MaxReadQueueLength = Mathf.Max(value, 1);
		}
	}

	[ServerVar]
	public static int maxwritequeuelength
	{
		get
		{
			return Server.MaxWriteQueueLength;
		}
		set
		{
			Server.MaxWriteQueueLength = Mathf.Max(value, 1);
		}
	}

	[ServerVar]
	public static int maxdecryptqueuelength
	{
		get
		{
			return Server.MaxDecryptQueueLength;
		}
		set
		{
			Server.MaxDecryptQueueLength = Mathf.Max(value, 1);
		}
	}

	[ServerVar]
	public static int maxreadqueuebytes
	{
		get
		{
			return Server.MaxReadQueueBytes;
		}
		set
		{
			Server.MaxReadQueueBytes = Mathf.Max(value, 1);
		}
	}

	[ServerVar]
	public static int maxwritequeuebytes
	{
		get
		{
			return Server.MaxWriteQueueBytes;
		}
		set
		{
			Server.MaxWriteQueueBytes = Mathf.Max(value, 1);
		}
	}

	[ServerVar]
	public static int maxdecryptqueuebytes
	{
		get
		{
			return Server.MaxDecryptQueueBytes;
		}
		set
		{
			Server.MaxDecryptQueueBytes = Mathf.Max(value, 1);
		}
	}

	[ServerVar]
	public static int maxpacketspersecond
	{
		get
		{
			return (int)Server.MaxPacketsPerSecond;
		}
		set
		{
			Server.MaxPacketsPerSecond = (ulong)Mathf.Clamp(value, 1, 1000000);
		}
	}

	public static string rootFolder => "server/" + identity;

	public static string backupFolder => "backup/0/" + identity;

	public static string backupFolder1 => "backup/1/" + identity;

	public static string backupFolder2 => "backup/2/" + identity;

	public static string backupFolder3 => "backup/3/" + identity;

	[ServerVar]
	public static bool compression
	{
		get
		{
			if (Net.sv == null)
			{
				return false;
			}
			return Net.sv.compressionEnabled;
		}
		set
		{
			Net.sv.compressionEnabled = value;
		}
	}

	[ServerVar]
	public static bool netlog
	{
		get
		{
			if (Net.sv == null)
			{
				return false;
			}
			return Net.sv.logging;
		}
		set
		{
			Net.sv.logging = value;
		}
	}

	public static float TickDelta()
	{
		return 1f / (float)tickrate;
	}

	public static float TickTime(uint tick)
	{
		return (float)((double)TickDelta() * (double)tick);
	}

	[ServerVar(Help = "Show holstered items on player bodies")]
	public static void setshowholstereditems(Arg arg)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		showHolsteredItems = arg.GetBool(0, showHolsteredItems);
		Enumerator<BasePlayer> enumerator = BasePlayer.activePlayerList.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				enumerator.Current.inventory.UpdatedVisibleHolsteredItems();
			}
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
		enumerator = BasePlayer.sleepingPlayerList.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				enumerator.Current.inventory.UpdatedVisibleHolsteredItems();
			}
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
	}

	[ServerVar]
	public static string printreadqueue(Arg arg)
	{
		return "Server read queue: " + ((BaseNetwork)Net.sv).ReadQueueLength + " items / " + NumberExtensions.FormatBytes<int>(((BaseNetwork)Net.sv).ReadQueueBytes, false);
	}

	[ServerVar]
	public static string printwritequeue(Arg arg)
	{
		return "Server write queue: " + ((BaseNetwork)Net.sv).WriteQueueLength + " items / " + NumberExtensions.FormatBytes<int>(((BaseNetwork)Net.sv).WriteQueueBytes, false);
	}

	[ServerVar]
	public static string printdecryptqueue(Arg arg)
	{
		return "Server decrypt queue: " + ((BaseNetwork)Net.sv).DecryptQueueLength + " items / " + NumberExtensions.FormatBytes<int>(((BaseNetwork)Net.sv).DecryptQueueBytes, false);
	}

	[ServerVar]
	public static string packetlog(Arg arg)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Expected O, but got Unknown
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		if (!packetlog_enabled)
		{
			return "Packet log is not enabled.";
		}
		List<Tuple<Type, ulong>> list = new List<Tuple<Type, ulong>>();
		foreach (KeyValuePair<Type, TimeAverageValue> item2 in SingletonComponent<ServerMgr>.Instance.packetHistory.dict)
		{
			list.Add(new Tuple<Type, ulong>(item2.Key, item2.Value.Calculate()));
		}
		TextTable val = new TextTable();
		val.AddColumn("type");
		val.AddColumn("calls");
		foreach (Tuple<Type, ulong> item3 in list.OrderByDescending((Tuple<Type, ulong> entry) => entry.Item2))
		{
			if (item3.Item2 == 0L)
			{
				break;
			}
			Type item = item3.Item1;
			string text = ((object)(Type)(ref item)).ToString();
			string text2 = item3.Item2.ToString();
			val.AddRow(new string[2] { text, text2 });
		}
		if (!arg.HasArg("--json"))
		{
			return ((object)val).ToString();
		}
		return val.ToJson();
	}

	[ServerVar]
	public static string rpclog(Arg arg)
	{
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Expected O, but got Unknown
		if (!rpclog_enabled)
		{
			return "RPC log is not enabled.";
		}
		List<Tuple<uint, ulong>> list = new List<Tuple<uint, ulong>>();
		foreach (KeyValuePair<uint, TimeAverageValue> item in SingletonComponent<ServerMgr>.Instance.rpcHistory.dict)
		{
			list.Add(new Tuple<uint, ulong>(item.Key, item.Value.Calculate()));
		}
		TextTable val = new TextTable();
		val.AddColumn("id");
		val.AddColumn("name");
		val.AddColumn("calls");
		foreach (Tuple<uint, ulong> item2 in list.OrderByDescending((Tuple<uint, ulong> entry) => entry.Item2))
		{
			if (item2.Item2 == 0L)
			{
				break;
			}
			string text = item2.Item1.ToString();
			string text2 = StringPool.Get(item2.Item1);
			string text3 = item2.Item2.ToString();
			val.AddRow(new string[3] { text, text2, text3 });
		}
		return ((object)val).ToString();
	}

	[ServerVar(Help = "Starts a server")]
	public static void start(Arg arg)
	{
		if (((BaseNetwork)Net.sv).IsConnected())
		{
			arg.ReplyWith("There is already a server running!");
			return;
		}
		string @string = arg.GetString(0, level);
		if (!LevelManager.IsValid(@string))
		{
			arg.ReplyWith("Level '" + @string + "' isn't valid!");
			return;
		}
		if (Object.op_Implicit((Object)(object)Object.FindObjectOfType<ServerMgr>()))
		{
			arg.ReplyWith("There is already a server running!");
			return;
		}
		Object.DontDestroyOnLoad((Object)(object)GameManager.server.CreatePrefab("assets/bundled/prefabs/system/server.prefab"));
		LevelManager.LoadLevel(@string);
	}

	[ServerVar(Help = "Stops a server")]
	public static void stop(Arg arg)
	{
		if (!((BaseNetwork)Net.sv).IsConnected())
		{
			arg.ReplyWith("There isn't a server running!");
		}
		else
		{
			Net.sv.Stop(arg.GetString(0, "Stopping Server"));
		}
	}

	[ServerVar(Help = "Backup server folder")]
	public static void backup()
	{
		DirectoryEx.Backup(backupFolder, backupFolder1, backupFolder2, backupFolder3);
		DirectoryEx.CopyAll(rootFolder, backupFolder);
	}

	public static string GetServerFolder(string folder)
	{
		string text = rootFolder + "/" + folder;
		if (Directory.Exists(text))
		{
			return text;
		}
		Directory.CreateDirectory(text);
		return text;
	}

	[ServerVar(Help = "Writes config files")]
	public static void writecfg(Arg arg)
	{
		string contents = ConsoleSystem.SaveToConfigString(true);
		File.WriteAllText(GetServerFolder("cfg") + "/serverauto.cfg", contents);
		ServerUsers.Save();
		arg.ReplyWith("Config Saved");
	}

	[ServerVar]
	public static void fps(Arg arg)
	{
		arg.ReplyWith(Performance.report.frameRate + " FPS");
	}

	[ServerVar(Help = "Force save the current game")]
	public static void save(Arg arg)
	{
		Stopwatch stopwatch = Stopwatch.StartNew();
		foreach (BaseEntity save in BaseEntity.saveList)
		{
			save.InvalidateNetworkCache();
		}
		Debug.Log((object)("Invalidate Network Cache took " + stopwatch.Elapsed.TotalSeconds.ToString("0.00") + " seconds"));
		SaveRestore.Save(AndWait: true);
	}

	[ServerVar]
	public static string readcfg(Arg arg)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		string serverFolder = GetServerFolder("cfg");
		Option server;
		if (File.Exists(serverFolder + "/serverauto.cfg"))
		{
			string text = File.ReadAllText(serverFolder + "/serverauto.cfg");
			server = Option.Server;
			ConsoleSystem.RunFile(((Option)(ref server)).Quiet(), text);
		}
		if (File.Exists(serverFolder + "/server.cfg"))
		{
			string text2 = File.ReadAllText(serverFolder + "/server.cfg");
			server = Option.Server;
			ConsoleSystem.RunFile(((Option)(ref server)).Quiet(), text2);
		}
		return "Server Config Loaded";
	}

	[ServerVar]
	public static string netprotocol(Arg arg)
	{
		if (Net.sv == null)
		{
			return string.Empty;
		}
		return Net.sv.ProtocolId;
	}

	[ServerUserVar]
	public static void cheatreport(Arg arg)
	{
		BasePlayer basePlayer = arg.Player();
		if (!((Object)(object)basePlayer == (Object)null))
		{
			string text = arg.GetUInt64(0, 0uL).ToString();
			string @string = arg.GetString(1, "");
			Debug.LogWarning((object)(((object)basePlayer)?.ToString() + " reported " + text + ": " + StringEx.ToPrintable(@string, 140)));
			EACServer.SendPlayerBehaviorReport(basePlayer, (PlayerReportsCategory)1, text, @string);
		}
	}

	[ServerAllVar(Help = "Get the player combat log")]
	public static string combatlog(Arg arg)
	{
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		BasePlayer basePlayer = arg.Player();
		if (arg.HasArgs(1) && arg.IsAdmin)
		{
			basePlayer = arg.GetPlayerOrSleeper(0);
		}
		if ((Object)(object)basePlayer == (Object)null || basePlayer.net == null)
		{
			return "invalid player";
		}
		CombatLog combat = basePlayer.stats.combat;
		int count = combatlogsize;
		bool json = arg.HasArg("--json");
		bool isAdmin = arg.IsAdmin;
		ulong requestingUser = arg.Connection?.userid ?? 0;
		return combat.Get(count, default(NetworkableId), json, isAdmin, requestingUser);
	}

	[ServerAllVar(Help = "Get the player combat log, only showing outgoing damage")]
	public static string combatlog_outgoing(Arg arg)
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		BasePlayer basePlayer = arg.Player();
		if (arg.HasArgs(1) && arg.IsAdmin)
		{
			basePlayer = arg.GetPlayerOrSleeper(0);
		}
		if ((Object)(object)basePlayer == (Object)null)
		{
			return "invalid player";
		}
		return basePlayer.stats.combat.Get(combatlogsize, basePlayer.net.ID, arg.HasArg("--json"), arg.IsAdmin, arg.Connection?.userid ?? 0);
	}

	[ServerVar(Help = "Print the current player position.")]
	public static string printpos(Arg arg)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		BasePlayer basePlayer = arg.Player();
		if (arg.HasArgs(1))
		{
			basePlayer = arg.GetPlayerOrSleeper(0);
		}
		if (!((Object)(object)basePlayer == (Object)null))
		{
			Vector3 position = ((Component)basePlayer).transform.position;
			return ((object)(Vector3)(ref position)).ToString();
		}
		return "invalid player";
	}

	[ServerVar(Help = "Print the current player rotation.")]
	public static string printrot(Arg arg)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		BasePlayer basePlayer = arg.Player();
		if (arg.HasArgs(1))
		{
			basePlayer = arg.GetPlayerOrSleeper(0);
		}
		if (!((Object)(object)basePlayer == (Object)null))
		{
			Quaternion rotation = ((Component)basePlayer).transform.rotation;
			Vector3 eulerAngles = ((Quaternion)(ref rotation)).eulerAngles;
			return ((object)(Vector3)(ref eulerAngles)).ToString();
		}
		return "invalid player";
	}

	[ServerVar(Help = "Print the current player eyes.")]
	public static string printeyes(Arg arg)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		BasePlayer basePlayer = arg.Player();
		if (arg.HasArgs(1))
		{
			basePlayer = arg.GetPlayerOrSleeper(0);
		}
		if (!((Object)(object)basePlayer == (Object)null))
		{
			Quaternion rotation = basePlayer.eyes.rotation;
			Vector3 eulerAngles = ((Quaternion)(ref rotation)).eulerAngles;
			return ((object)(Vector3)(ref eulerAngles)).ToString();
		}
		return "invalid player";
	}

	[ServerVar(ServerAdmin = true, Help = "This sends a snapshot of all the entities in the client's pvs. This is mostly redundant, but we request this when the client starts recording a demo.. so they get all the information.")]
	public static void snapshot(Arg arg)
	{
		BasePlayer basePlayer = arg.Player();
		if (!((Object)(object)basePlayer == (Object)null))
		{
			Debug.Log((object)("Sending full snapshot to " + (object)basePlayer));
			basePlayer.SendNetworkUpdateImmediate();
			basePlayer.SendGlobalSnapshot();
			basePlayer.SendFullSnapshot();
			basePlayer.SendEntityUpdate();
			TreeManager.SendSnapshot(basePlayer);
			ServerMgr.SendReplicatedVars(basePlayer.net.connection);
		}
	}

	[ServerVar(Help = "Send network update for all players")]
	public static void sendnetworkupdate(Arg arg)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		Enumerator<BasePlayer> enumerator = BasePlayer.activePlayerList.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				enumerator.Current.SendNetworkUpdate();
			}
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
	}

	[ServerVar(Help = "Prints the position of all players on the server")]
	public static void playerlistpos(Arg arg)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		TextTable val = new TextTable();
		val.AddColumns(new string[4] { "SteamID", "DisplayName", "POS", "ROT" });
		Enumerator<BasePlayer> enumerator = BasePlayer.activePlayerList.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				BasePlayer current = enumerator.Current;
				string[] obj = new string[4]
				{
					current.userID.ToString(),
					current.displayName,
					null,
					null
				};
				Vector3 val2 = ((Component)current).transform.position;
				obj[2] = ((object)(Vector3)(ref val2)).ToString();
				val2 = current.eyes.BodyForward();
				obj[3] = ((object)(Vector3)(ref val2)).ToString();
				val.AddRow(obj);
			}
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
		arg.ReplyWith(arg.HasArg("--json") ? val.ToJson() : ((object)val).ToString());
	}

	[ServerVar(Help = "Prints all the vending machines on the server")]
	public static void listvendingmachines(Arg arg)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		TextTable val = new TextTable();
		val.AddColumns(new string[3] { "EntityId", "Position", "Name" });
		foreach (VendingMachine item in BaseNetworkable.serverEntities.OfType<VendingMachine>())
		{
			string[] obj = new string[3]
			{
				((object)(NetworkableId)(ref item.net.ID)).ToString(),
				null,
				null
			};
			Vector3 position = ((Component)item).transform.position;
			obj[1] = ((object)(Vector3)(ref position)).ToString();
			obj[2] = StringExtensions.QuoteSafe(item.shopName);
			val.AddRow(obj);
		}
		arg.ReplyWith(arg.HasArg("--json") ? val.ToJson() : ((object)val).ToString());
	}

	[ServerVar(Help = "Prints all the Tool Cupboards on the server")]
	public static void listtoolcupboards(Arg arg)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		TextTable val = new TextTable();
		val.AddColumns(new string[3] { "EntityId", "Position", "Authed" });
		foreach (BuildingPrivlidge item in BaseNetworkable.serverEntities.OfType<BuildingPrivlidge>())
		{
			string[] obj = new string[3]
			{
				((object)(NetworkableId)(ref item.net.ID)).ToString(),
				null,
				null
			};
			Vector3 position = ((Component)item).transform.position;
			obj[1] = ((object)(Vector3)(ref position)).ToString();
			obj[2] = item.authorizedPlayers.Count.ToString();
			val.AddRow(obj);
		}
		arg.ReplyWith(arg.HasArg("--json") ? val.ToJson() : ((object)val).ToString());
	}

	[ServerVar]
	public static void BroadcastPlayVideo(Arg arg)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		string @string = arg.GetString(0, "");
		if (string.IsNullOrWhiteSpace(@string))
		{
			arg.ReplyWith("Missing video URL");
			return;
		}
		Enumerator<BasePlayer> enumerator = BasePlayer.activePlayerList.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				enumerator.Current.Command("client.playvideo", @string);
			}
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
		arg.ReplyWith($"Sent video to {BasePlayer.activePlayerList.Count} players");
	}

	[ServerVar(Help = "Rescans the serveremoji folder, note that clients will need to reconnect to get the latest emoji")]
	public static void ResetServerEmoji()
	{
		RustEmojiLibrary.ResetServerEmoji();
	}
}
