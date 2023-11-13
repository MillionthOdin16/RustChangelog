using System;
using System.Collections;
using ConVar;
using Network;
using Rust;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.SceneManagement;

public class GameSetup : MonoBehaviour
{
	public static bool RunOnce = false;

	public bool startServer = true;

	public string clientConnectCommand = "client.connect 127.0.0.1:28015";

	public bool loadMenu = true;

	public bool loadLevel = false;

	public string loadLevelScene = "";

	public bool loadSave = false;

	public string loadSaveFile = "";

	public string initializationCommands = "";

	protected void Awake()
	{
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		if (RunOnce)
		{
			GameManager.Destroy(((Component)this).gameObject);
			return;
		}
		GameManifest.Load();
		GameManifest.LoadAssets();
		RunOnce = true;
		Profiler.BeginSample("Bootstrap.Initialization");
		if (Bootstrap.needsSetup)
		{
			Bootstrap.Init_Tier0();
			Bootstrap.Init_Systems();
			Bootstrap.Init_Config();
		}
		Profiler.EndSample();
		if (initializationCommands.Length > 0)
		{
			string[] array = initializationCommands.Split(';');
			string[] array2 = array;
			foreach (string text in array2)
			{
				ConsoleSystem.Run(Option.Server, text.Trim(), Array.Empty<object>());
			}
		}
		((MonoBehaviour)this).StartCoroutine(DoGameSetup());
	}

	private IEnumerator DoGameSetup()
	{
		Application.isLoading = true;
		TerrainMeta.InitNoTerrain();
		ItemManager.Initialize();
		Scene activeScene = SceneManager.GetActiveScene();
		LevelManager.CurrentLevelName = ((Scene)(ref activeScene)).name;
		if (loadLevel && !string.IsNullOrEmpty(loadLevelScene))
		{
			Net.sv.Reset();
			ConVar.Server.level = loadLevelScene;
			LoadingScreen.Update("LOADING SCENE");
			Profiler.BeginSample("DoGameSetup.loadLevelScene");
			Application.LoadLevelAdditive(loadLevelScene);
			Profiler.EndSample();
			LoadingScreen.Update(loadLevelScene.ToUpper() + " LOADED");
		}
		if (startServer)
		{
			yield return ((MonoBehaviour)this).StartCoroutine(StartServer());
		}
		yield return null;
		Application.isLoading = false;
	}

	private IEnumerator StartServer()
	{
		Profiler.BeginSample("DoGameSetup.StartServer.GarbageCollect.collect");
		ConVar.GC.collect();
		ConVar.GC.unload();
		Profiler.EndSample();
		yield return CoroutineEx.waitForEndOfFrame;
		yield return CoroutineEx.waitForEndOfFrame;
		yield return ((MonoBehaviour)this).StartCoroutine(Bootstrap.StartServer(loadSave, loadSaveFile, allowOutOfDateSaves: true));
	}
}
