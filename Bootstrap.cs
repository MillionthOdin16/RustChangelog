using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Threading;
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
using UnityEngine.Profiling;

public class Bootstrap : SingletonComponent<Bootstrap>
{
	internal static bool bootstrapInitRun = false;

	public static bool isErrored = false;

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
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Expected O, but got Unknown
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
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		ConsoleNetwork.Init();
		ConsoleSystem.UpdateValuesFromCommandLine();
		Profiler.BeginSample("Bootstrap.readcfg");
		ConsoleSystem.Run(Option.Server, "server.readcfg", Array.Empty<object>());
		ServerUsers.Load();
		RustEmojiLibrary.FindAllServerEmoji();
		Profiler.EndSample();
	}

	public static void NetworkInitRaknet()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		Net.sv = (Server)new Server();
	}

	public static void NetworkInitSteamworks(bool enableSteamDatagramRelay)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Expected O, but got Unknown
		Net.sv = (Server)new Server(enableSteamDatagramRelay);
	}

	private IEnumerator Start()
	{
		WriteToLog("Bootstrap Startup");
		BenchmarkTimer.Enabled = CommandLine.Full.Contains("+autobench");
		BenchmarkTimer timer = BenchmarkTimer.New("bootstrap");
		if (!Application.isEditor)
		{
			BuildInfo build = BuildInfo.Current;
			if ((build.Scm.Branch == null || !(build.Scm.Branch == "experimental/release")) && !(build.Scm.Branch == "release"))
			{
				ExceptionReporter.InitializeFromUrl("https://0654eb77d1e04d6babad83201b6b6b95:d2098f1d15834cae90501548bd5dbd0d@sentry.io/1836389");
			}
			else
			{
				ExceptionReporter.InitializeFromUrl("https://83df169465e84da091c1a3cd2fbffeee:3671b903f9a840ecb68411cf946ab9b6@sentry.io/51080");
			}
			bool hasOfficialConvar = CommandLine.Full.Contains("-official") || CommandLine.Full.Contains("-server.official") || CommandLine.Full.Contains("+official") || CommandLine.Full.Contains("+server.official");
			bool hasStatsConvar = CommandLine.Full.Contains("-stats") || CommandLine.Full.Contains("-server.stats") || CommandLine.Full.Contains("+stats") || CommandLine.Full.Contains("+server.stats");
			ExceptionReporter.Disabled = !(hasOfficialConvar && hasStatsConvar);
		}
		if (AssetBundleBackend.Enabled)
		{
			AssetBundleBackend newBackend = new AssetBundleBackend();
			BenchmarkTimer val = BenchmarkTimer.New("bootstrap;bundles");
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
			BenchmarkTimer val2 = BenchmarkTimer.New("bootstrap;bundlesindex");
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
		BenchmarkTimer val3 = BenchmarkTimer.New("bootstrap;gamemanifest");
		try
		{
			yield return ((MonoBehaviour)this).StartCoroutine(LoadingUpdate("Loading Game Manifest"));
			GameManifest.Load();
			yield return ((MonoBehaviour)this).StartCoroutine(LoadingUpdate("DONE!"));
		}
		finally
		{
			((IDisposable)val3)?.Dispose();
		}
		BenchmarkTimer val4 = BenchmarkTimer.New("bootstrap;selfcheck");
		try
		{
			yield return ((MonoBehaviour)this).StartCoroutine(LoadingUpdate("Running Self Check"));
			SelfCheck.Run();
		}
		finally
		{
			((IDisposable)val4)?.Dispose();
		}
		if (isErrored)
		{
			yield break;
		}
		yield return ((MonoBehaviour)this).StartCoroutine(LoadingUpdate("Bootstrap Tier0"));
		BenchmarkTimer val5 = BenchmarkTimer.New("bootstrap;tier0");
		try
		{
			Init_Tier0();
		}
		finally
		{
			((IDisposable)val5)?.Dispose();
		}
		BenchmarkTimer val6 = BenchmarkTimer.New("bootstrap;commandlinevalues");
		try
		{
			ConsoleSystem.UpdateValuesFromCommandLine();
		}
		finally
		{
			((IDisposable)val6)?.Dispose();
		}
		yield return ((MonoBehaviour)this).StartCoroutine(LoadingUpdate("Bootstrap Systems"));
		BenchmarkTimer val7 = BenchmarkTimer.New("bootstrap;init_systems");
		try
		{
			Init_Systems();
		}
		finally
		{
			((IDisposable)val7)?.Dispose();
		}
		yield return ((MonoBehaviour)this).StartCoroutine(LoadingUpdate("Bootstrap Config"));
		BenchmarkTimer val8 = BenchmarkTimer.New("bootstrap;init_config");
		try
		{
			Init_Config();
		}
		finally
		{
			((IDisposable)val8)?.Dispose();
		}
		if (isErrored)
		{
			yield break;
		}
		yield return ((MonoBehaviour)this).StartCoroutine(LoadingUpdate("Loading Items"));
		BenchmarkTimer val9 = BenchmarkTimer.New("bootstrap;itemmanager");
		try
		{
			ItemManager.Initialize();
		}
		finally
		{
			((IDisposable)val9)?.Dispose();
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
		WriteToLog("Loading Scene");
		yield return CoroutineEx.waitForEndOfFrame;
		yield return CoroutineEx.waitForEndOfFrame;
		Physics.solverIterationCount = 3;
		int oldGraphicsQuality = PlayerPrefs.GetInt("UnityGraphicsQuality");
		QualitySettings.SetQualityLevel(0);
		PlayerPrefs.SetInt("UnityGraphicsQuality", oldGraphicsQuality);
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

	public static IEnumerator StartServer(bool doLoad, string saveFileOverride, bool allowOutOfDateSaves)
	{
		float timeScale = Time.timeScale;
		if (Time.pausewhileloading)
		{
			Time.timeScale = 0f;
		}
		RCon.Initialize();
		BaseEntity.Query.Server = new BaseEntity.Query.EntityTree(8096f);
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
					DungeonNavmesh dungeonNavMesh = TerrainMeta.Path.DungeonGridRoot.AddComponent<DungeonNavmesh>();
					dungeonNavMesh.NavMeshCollectGeometry = (NavMeshCollectGeometry)1;
					dungeonNavMesh.LayerMask = LayerMask.op_Implicit(65537);
					yield return ((MonoBehaviour)dungeonNavMesh).StartCoroutine(dungeonNavMesh.UpdateNavMeshAndWait());
				}
				else
				{
					Debug.LogError((object)"Failed to find DungeonGridRoot, NOT generating Dungeon navmesh");
				}
				if (Object.op_Implicit((Object)(object)TerrainMeta.Path) && Object.op_Implicit((Object)(object)TerrainMeta.Path.DungeonBaseRoot))
				{
					DungeonNavmesh dungeonNavMesh2 = TerrainMeta.Path.DungeonBaseRoot.AddComponent<DungeonNavmesh>();
					dungeonNavMesh2.NavmeshResolutionModifier = 0.3f;
					dungeonNavMesh2.NavMeshCollectGeometry = (NavMeshCollectGeometry)1;
					dungeonNavMesh2.LayerMask = LayerMask.op_Implicit(65537);
					yield return ((MonoBehaviour)dungeonNavMesh2).StartCoroutine(dungeonNavMesh2.UpdateNavMeshAndWait());
				}
				else
				{
					Debug.LogError((object)"Failed to find DungeonBaseRoot , NOT generating Dungeon navmesh");
				}
				GenerateDungeonBase.SetupAI();
			}
		}
		Profiler.BeginSample("Bootstrap.InstantiateServerMgr");
		GameObject server = GameManager.server.CreatePrefab("assets/bundled/prefabs/system/server.prefab");
		Object.DontDestroyOnLoad((Object)(object)server);
		ServerMgr serverMgr = server.GetComponent<ServerMgr>();
		Profiler.EndSample();
		Profiler.BeginSample("ServerMgr.Initialize");
		serverMgr.Initialize(doLoad, saveFileOverride, allowOutOfDateSaves);
		Profiler.EndSample();
		yield return CoroutineEx.waitForSecondsRealtime(0.1f);
		Profiler.BeginSample("SaveRestore.InitializeEntityLinks");
		SaveRestore.InitializeEntityLinks();
		Profiler.EndSample();
		yield return CoroutineEx.waitForSecondsRealtime(0.1f);
		Profiler.BeginSample("SaveRestore.InitializeEntitySupports");
		SaveRestore.InitializeEntitySupports();
		Profiler.EndSample();
		yield return CoroutineEx.waitForSecondsRealtime(0.1f);
		Profiler.BeginSample("SaveRestore.InitializeEntityConditionals");
		SaveRestore.InitializeEntityConditionals();
		Profiler.EndSample();
		yield return CoroutineEx.waitForSecondsRealtime(0.1f);
		Profiler.BeginSample("SaveRestore.GetSaveCache");
		SaveRestore.GetSaveCache();
		Profiler.EndSample();
		yield return CoroutineEx.waitForSecondsRealtime(0.1f);
		Profiler.BeginSample("ServerMgr.CreateGameMode");
		BaseGameMode.CreateGameMode();
		Profiler.EndSample();
		yield return CoroutineEx.waitForSecondsRealtime(0.1f);
		Profiler.BeginSample("MissionManifest.Get");
		MissionManifest.Get();
		Profiler.EndSample();
		yield return CoroutineEx.waitForSecondsRealtime(0.1f);
		Profiler.BeginSample("ServerMgr.OpenConnection");
		serverMgr.OpenConnection();
		Profiler.EndSample();
		CompanionServer.Server.Initialize();
		BenchmarkTimer val = BenchmarkTimer.New("Boombox.LoadStations");
		try
		{
			BoomBox.LoadStations();
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
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
