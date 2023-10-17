using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CompanionServer;
using ConVar;
using Facepunch;
using Facepunch.Network;
using Facepunch.Network.Raknet;
using Facepunch.Rust;
using Facepunch.Utility;
using Network;
using Rust;
using Rust.Ai;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class Bootstrap : SingletonComponent<Bootstrap>
{
	internal static bool bootstrapInitRun;

	public static bool isErrored;

	public string messageString = "Loading...";

	public CanvasGroup BootstrapUiCanvas;

	public GameObject errorPanel;

	public TextMeshProUGUI errorText;

	public TextMeshProUGUI statusText;

	private static string lastWrittenValue;

	public static bool needsSetup => !bootstrapInitRun;

	public static bool isPresent
	{
		get
		{
			if (bootstrapInitRun)
			{
				return true;
			}
			if (Object.FindObjectsOfType<GameSetup>().Count() > 0)
			{
				return true;
			}
			return false;
		}
	}

	public static void RunDefaults()
	{
		Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
		Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
		Application.targetFrameRate = 256;
		Time.fixedDeltaTime = 0.0625f;
		Time.maximumDeltaTime = 0.125f;
	}

	public static void Init_Tier0()
	{
		RunDefaults();
		GameSetup.RunOnce = true;
		bootstrapInitRun = true;
		Index.Initialize(ConsoleGen.All);
		UnityButtons.Register();
		Output.Install();
		Pool.ResizeBuffer<NetRead>(16384);
		Pool.ResizeBuffer<NetWrite>(16384);
		Pool.ResizeBuffer<Networkable>(65536);
		Pool.ResizeBuffer<EntityLink>(65536);
		Pool.FillBuffer<Networkable>();
		Pool.FillBuffer<EntityLink>();
		if (CommandLine.HasSwitch("-nonetworkthread"))
		{
			BaseNetwork.Multithreading = false;
		}
		SteamNetworking.SetDebugFunction();
		if (CommandLine.HasSwitch("-swnet"))
		{
			NetworkInitSteamworks(enableSteamDatagramRelay: false);
		}
		else if (CommandLine.HasSwitch("-sdrnet"))
		{
			NetworkInitSteamworks(enableSteamDatagramRelay: true);
		}
		else if (CommandLine.HasSwitch("-raknet"))
		{
			NetworkInitRaknet();
		}
		else
		{
			NetworkInitRaknet();
		}
		if (!Application.isEditor)
		{
			string text = CommandLine.Full.Replace(CommandLine.GetSwitch("-rcon.password", CommandLine.GetSwitch("+rcon.password", "RCONPASSWORD")), "******");
			WriteToLog("Command Line: " + text);
		}
	}

	public static void Init_Systems()
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Expected O, but got Unknown
		Global.Init();
		if (GameInfo.IsOfficialServer && ConVar.Server.stats)
		{
			GA.Logging = false;
			GA.Build = BuildInfo.Current.Scm.ChangeId;
			GA.Initialize("218faecaf1ad400a4e15c53392ebeebc", "0c9803ce52c38671278899538b9c54c8d4e19849");
			Analytics.Server.Enabled = true;
		}
		Application.Initialize((BaseIntegration)new Integration());
		Performance.GetMemoryUsage = () => SystemInfoEx.systemMemoryUsed;
	}

	public static void Init_Config()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		ConsoleNetwork.Init();
		ConsoleSystem.UpdateValuesFromCommandLine();
		ConsoleSystem.Run(Option.Server, "server.readcfg", Array.Empty<object>());
		ServerUsers.Load();
	}

	public static void NetworkInitRaknet()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Expected O, but got Unknown
		Net.sv = (Server)new Server();
	}

	public static void NetworkInitSteamworks(bool enableSteamDatagramRelay)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		Net.sv = (Server)new Server(enableSteamDatagramRelay);
	}

	private IEnumerator Start()
	{
		WriteToLog("Bootstrap Startup");
		BenchmarkTimer.Enabled = CommandLine.Full.Contains("+autobench");
		BenchmarkTimer timer = BenchmarkTimer.New("bootstrap");
		if (!Application.isEditor)
		{
			BuildInfo current = BuildInfo.Current;
			if ((current.Scm.Branch == null || !(current.Scm.Branch == "experimental/release")) && !(current.Scm.Branch == "release"))
			{
				ExceptionReporter.InitializeFromUrl("https://0654eb77d1e04d6babad83201b6b6b95:d2098f1d15834cae90501548bd5dbd0d@sentry.io/1836389");
			}
			else
			{
				ExceptionReporter.InitializeFromUrl("https://83df169465e84da091c1a3cd2fbffeee:3671b903f9a840ecb68411cf946ab9b6@sentry.io/51080");
			}
			bool num = CommandLine.Full.Contains("-official") || CommandLine.Full.Contains("-server.official") || CommandLine.Full.Contains("+official") || CommandLine.Full.Contains("+server.official");
			bool flag = CommandLine.Full.Contains("-stats") || CommandLine.Full.Contains("-server.stats") || CommandLine.Full.Contains("+stats") || CommandLine.Full.Contains("+server.stats");
			ExceptionReporter.Disabled = !(num && flag);
		}
		BenchmarkTimer val;
		BenchmarkTimer val2;
		if (AssetBundleBackend.Enabled)
		{
			AssetBundleBackend newBackend = new AssetBundleBackend();
			val = BenchmarkTimer.New("bootstrap;bundles");
			try
			{
				yield return ((MonoBehaviour)this).StartCoroutine(LoadingUpdate("Opening Bundles"));
				newBackend.Load("Bundles/Bundles");
				FileSystem.Backend = (FileSystemBackend)(object)newBackend;
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
			if (FileSystem.Backend.isError)
			{
				ThrowError(FileSystem.Backend.loadingError);
				yield break;
			}
			val2 = BenchmarkTimer.New("bootstrap;bundlesindex");
			try
			{
				newBackend.BuildFileIndex();
			}
			finally
			{
				((IDisposable)val2)?.Dispose();
			}
		}
		if (FileSystem.Backend.isError)
		{
			ThrowError(FileSystem.Backend.loadingError);
			yield break;
		}
		if (!Application.isEditor)
		{
			WriteToLog(SystemInfoGeneralText.currentInfo);
		}
		Texture.SetGlobalAnisotropicFilteringLimits(1, 16);
		if (isErrored)
		{
			yield break;
		}
		val = BenchmarkTimer.New("bootstrap;gamemanifest");
		try
		{
			yield return ((MonoBehaviour)this).StartCoroutine(LoadingUpdate("Loading Game Manifest"));
			GameManifest.Load();
			yield return ((MonoBehaviour)this).StartCoroutine(LoadingUpdate("DONE!"));
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		val = BenchmarkTimer.New("bootstrap;selfcheck");
		try
		{
			yield return ((MonoBehaviour)this).StartCoroutine(LoadingUpdate("Running Self Check"));
			SelfCheck.Run();
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		if (isErrored)
		{
			yield break;
		}
		yield return ((MonoBehaviour)this).StartCoroutine(LoadingUpdate("Bootstrap Tier0"));
		val2 = BenchmarkTimer.New("bootstrap;tier0");
		try
		{
			Init_Tier0();
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
		val2 = BenchmarkTimer.New("bootstrap;commandlinevalues");
		try
		{
			ConsoleSystem.UpdateValuesFromCommandLine();
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
		yield return ((MonoBehaviour)this).StartCoroutine(LoadingUpdate("Bootstrap Systems"));
		val2 = BenchmarkTimer.New("bootstrap;init_systems");
		try
		{
			Init_Systems();
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
		yield return ((MonoBehaviour)this).StartCoroutine(LoadingUpdate("Bootstrap Config"));
		val2 = BenchmarkTimer.New("bootstrap;init_config");
		try
		{
			Init_Config();
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
		if (isErrored)
		{
			yield break;
		}
		yield return ((MonoBehaviour)this).StartCoroutine(LoadingUpdate("Loading Items"));
		val2 = BenchmarkTimer.New("bootstrap;itemmanager");
		try
		{
			ItemManager.Initialize();
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
		if (!isErrored)
		{
			yield return ((MonoBehaviour)this).StartCoroutine(DedicatedServerStartup());
			if (timer != null)
			{
				timer.Dispose();
			}
			GameManager.Destroy(((Component)this).gameObject);
		}
	}

	private IEnumerator DedicatedServerStartup()
	{
		Application.isLoading = true;
		Application.backgroundLoadingPriority = (ThreadPriority)4;
		WriteToLog("Skinnable Warmup");
		yield return CoroutineEx.waitForEndOfFrame;
		yield return CoroutineEx.waitForEndOfFrame;
		GameManifest.LoadAssets();
		WriteToLog("Initializing Nexus");
		yield return ((MonoBehaviour)this).StartCoroutine(StartNexusServer());
		WriteToLog("Loading Scene");
		yield return CoroutineEx.waitForEndOfFrame;
		yield return CoroutineEx.waitForEndOfFrame;
		Physics.solverIterationCount = 3;
		int @int = PlayerPrefs.GetInt("UnityGraphicsQuality");
		QualitySettings.SetQualityLevel(0);
		PlayerPrefs.SetInt("UnityGraphicsQuality", @int);
		Object.DontDestroyOnLoad((Object)(object)((Component)this).gameObject);
		Object.DontDestroyOnLoad((Object)(object)GameManager.server.CreatePrefab("assets/bundled/prefabs/system/server_console.prefab"));
		StartupShared();
		World.InitSize(ConVar.Server.worldsize);
		World.InitSeed(ConVar.Server.seed);
		World.InitSalt(ConVar.Server.salt);
		World.Url = ConVar.Server.levelurl;
		World.Transfer = ConVar.Server.leveltransfer;
		LevelManager.LoadLevel(ConVar.Server.level);
		yield return CoroutineEx.waitForEndOfFrame;
		yield return CoroutineEx.waitForEndOfFrame;
		string[] assetList = FileSystem_Warmup.GetAssetList();
		yield return ((MonoBehaviour)this).StartCoroutine(FileSystem_Warmup.Run(assetList, WriteToLog, "Asset Warmup ({0}/{1})"));
		yield return ((MonoBehaviour)this).StartCoroutine(StartServer(!CommandLine.HasSwitch("-skipload"), "", allowOutOfDateSaves: false));
		if (!Object.op_Implicit((Object)(object)Object.FindObjectOfType<Performance>()))
		{
			Object.DontDestroyOnLoad((Object)(object)GameManager.server.CreatePrefab("assets/bundled/prefabs/system/performance.prefab"));
		}
		Rust.GC.Collect();
		Application.isLoading = false;
	}

	private static void EnsureRootFolderCreated()
	{
		try
		{
			Directory.CreateDirectory(ConVar.Server.rootFolder);
		}
		catch (Exception arg)
		{
			Debug.LogWarning((object)$"Failed to automatically create the save directory: {ConVar.Server.rootFolder}\n\n{arg}");
		}
	}

	public static IEnumerator StartNexusServer()
	{
		EnsureRootFolderCreated();
		yield return NexusServer.Initialize();
		if (NexusServer.FailedToStart)
		{
			Debug.LogError((object)"Nexus server failed to start, terminating");
			Application.Quit();
		}
	}

	public static IEnumerator StartServer(bool doLoad, string saveFileOverride, bool allowOutOfDateSaves)
	{
		float timeScale = Time.timeScale;
		if (Time.pausewhileloading)
		{
			Time.timeScale = 0f;
		}
		RCon.Initialize();
		BaseEntity.Query.Server = new BaseEntity.Query.EntityTree(8096f);
		EnsureRootFolderCreated();
		if (Object.op_Implicit((Object)(object)SingletonComponent<WorldSetup>.Instance))
		{
			yield return ((MonoBehaviour)SingletonComponent<WorldSetup>.Instance).StartCoroutine(SingletonComponent<WorldSetup>.Instance.InitCoroutine());
		}
		if (Object.op_Implicit((Object)(object)SingletonComponent<DynamicNavMesh>.Instance) && ((Behaviour)SingletonComponent<DynamicNavMesh>.Instance).enabled && !AiManager.nav_disable)
		{
			yield return ((MonoBehaviour)SingletonComponent<DynamicNavMesh>.Instance).StartCoroutine(SingletonComponent<DynamicNavMesh>.Instance.UpdateNavMeshAndWait());
		}
		if (Object.op_Implicit((Object)(object)SingletonComponent<AiManager>.Instance) && ((Behaviour)SingletonComponent<AiManager>.Instance).enabled)
		{
			SingletonComponent<AiManager>.Instance.Initialize();
			if (!AiManager.nav_disable && AI.npc_enable && (Object)(object)TerrainMeta.Path != (Object)null)
			{
				foreach (MonumentInfo monument in TerrainMeta.Path.Monuments)
				{
					if (monument.HasNavmesh)
					{
						yield return ((MonoBehaviour)monument).StartCoroutine(monument.GetMonumentNavMesh().UpdateNavMeshAndWait());
					}
				}
				if (Object.op_Implicit((Object)(object)TerrainMeta.Path) && Object.op_Implicit((Object)(object)TerrainMeta.Path.DungeonGridRoot))
				{
					DungeonNavmesh dungeonNavmesh = TerrainMeta.Path.DungeonGridRoot.AddComponent<DungeonNavmesh>();
					dungeonNavmesh.NavMeshCollectGeometry = (NavMeshCollectGeometry)1;
					dungeonNavmesh.LayerMask = LayerMask.op_Implicit(65537);
					yield return ((MonoBehaviour)dungeonNavmesh).StartCoroutine(dungeonNavmesh.UpdateNavMeshAndWait());
				}
				else
				{
					Debug.LogError((object)"Failed to find DungeonGridRoot, NOT generating Dungeon navmesh");
				}
				if (Object.op_Implicit((Object)(object)TerrainMeta.Path) && Object.op_Implicit((Object)(object)TerrainMeta.Path.DungeonBaseRoot))
				{
					DungeonNavmesh dungeonNavmesh2 = TerrainMeta.Path.DungeonBaseRoot.AddComponent<DungeonNavmesh>();
					dungeonNavmesh2.NavmeshResolutionModifier = 0.3f;
					dungeonNavmesh2.NavMeshCollectGeometry = (NavMeshCollectGeometry)1;
					dungeonNavmesh2.LayerMask = LayerMask.op_Implicit(65537);
					yield return ((MonoBehaviour)dungeonNavmesh2).StartCoroutine(dungeonNavmesh2.UpdateNavMeshAndWait());
				}
				else
				{
					Debug.LogError((object)"Failed to find DungeonBaseRoot , NOT generating Dungeon navmesh");
				}
				GenerateDungeonBase.SetupAI();
			}
		}
		GameObject val = GameManager.server.CreatePrefab("assets/bundled/prefabs/system/server.prefab");
		Object.DontDestroyOnLoad((Object)(object)val);
		ServerMgr serverMgr = val.GetComponent<ServerMgr>();
		bool saveWasLoaded = serverMgr.Initialize(doLoad, saveFileOverride, allowOutOfDateSaves);
		yield return CoroutineEx.waitForSecondsRealtime(0.1f);
		SaveRestore.InitializeEntityLinks();
		yield return CoroutineEx.waitForSecondsRealtime(0.1f);
		SaveRestore.InitializeEntitySupports();
		yield return CoroutineEx.waitForSecondsRealtime(0.1f);
		SaveRestore.InitializeEntityConditionals();
		yield return CoroutineEx.waitForSecondsRealtime(0.1f);
		SaveRestore.GetSaveCache();
		yield return CoroutineEx.waitForSecondsRealtime(0.1f);
		BaseGameMode.CreateGameMode();
		yield return CoroutineEx.waitForSecondsRealtime(0.1f);
		MissionManifest.Get();
		yield return CoroutineEx.waitForSecondsRealtime(0.1f);
		ClanManager serverInstance = ClanManager.ServerInstance;
		if ((Object)(object)serverInstance == (Object)null)
		{
			Debug.LogError((object)"ClanManager was not spawned!");
			Application.Quit();
			yield break;
		}
		Task initializeTask = serverInstance.Initialize();
		yield return (object)new WaitUntil((Func<bool>)(() => initializeTask.IsCompleted));
		initializeTask.Wait();
		yield return CoroutineEx.waitForSecondsRealtime(0.1f);
		if (NexusServer.Started)
		{
			NexusServer.UploadMapImage();
			if (saveWasLoaded)
			{
				NexusServer.RestoreUnsavedState();
			}
			NexusServer.ZoneClient.StartListening();
		}
		serverMgr.OpenConnection();
		CompanionServer.Server.Initialize();
		BenchmarkTimer val2 = BenchmarkTimer.New("Boombox.LoadStations");
		try
		{
			BoomBox.LoadStations();
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
		RustEmojiLibrary.FindAllServerEmoji();
		if (Time.pausewhileloading)
		{
			Time.timeScale = timeScale;
		}
		WriteToLog("Server startup complete");
	}

	private void StartupShared()
	{
		ItemManager.Initialize();
	}

	public void ThrowError(string error)
	{
		Debug.Log((object)("ThrowError: " + error));
		errorPanel.SetActive(true);
		((TMP_Text)errorText).text = error;
		isErrored = true;
	}

	public void ExitGame()
	{
		Debug.Log((object)"Exiting due to Exit Game button on bootstrap error panel");
		Application.Quit();
	}

	public static IEnumerator LoadingUpdate(string str)
	{
		if (Object.op_Implicit((Object)(object)SingletonComponent<Bootstrap>.Instance))
		{
			SingletonComponent<Bootstrap>.Instance.messageString = str;
			yield return CoroutineEx.waitForEndOfFrame;
			yield return CoroutineEx.waitForEndOfFrame;
		}
	}

	public static void WriteToLog(string str)
	{
		if (!(lastWrittenValue == str))
		{
			DebugEx.Log((object)str, (StackTraceLogType)0);
			lastWrittenValue = str;
		}
	}
}
