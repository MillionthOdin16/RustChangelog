using System.Collections;
using System.Collections.Generic;
using System.IO;
using ConVar;
using Rust;
using UnityEngine;
using UnityEngine.Networking;

public class WorldSetup : SingletonComponent<WorldSetup>
{
	public bool AutomaticallySetup;

	public bool BypassProceduralSpawn;

	public GameObject terrain;

	public GameObject decorPrefab;

	public GameObject grassPrefab;

	public GameObject spawnPrefab;

	private TerrainMeta terrainMeta;

	public uint EditorSeed;

	public uint EditorSalt;

	public uint EditorSize;

	public string EditorUrl = string.Empty;

	public string EditorConfigFile = string.Empty;

	[TextArea]
	public string EditorConfigString = string.Empty;

	internal List<ProceduralObject> ProceduralObjects = new List<ProceduralObject>();

	internal List<MonumentNode> MonumentNodes = new List<MonumentNode>();

	private void OnValidate()
	{
		if ((Object)(object)terrain == (Object)null)
		{
			Terrain val = Object.FindObjectOfType<Terrain>();
			if ((Object)(object)val != (Object)null)
			{
				terrain = ((Component)val).gameObject;
			}
		}
	}

	protected override void Awake()
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Expected O, but got Unknown
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		((SingletonComponent)this).Awake();
		Prefab[] array = Prefab.Load("assets/bundled/prefabs/world", null, null, useProbabilities: false, useWorldConfig: false);
		foreach (Prefab prefab in array)
		{
			if ((Object)(object)prefab.Object.GetComponent<BaseEntity>() != (Object)null)
			{
				prefab.SpawnEntity(Vector3.zero, Quaternion.identity).Spawn();
			}
			else
			{
				prefab.Spawn(Vector3.zero, Quaternion.identity);
			}
		}
		SingletonComponent[] array2 = Object.FindObjectsOfType<SingletonComponent>();
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].SingletonSetup();
		}
		if (Object.op_Implicit((Object)(object)terrain))
		{
			if (Object.op_Implicit((Object)(object)terrain.GetComponent<TerrainGenerator>()))
			{
				World.Procedural = true;
			}
			else
			{
				World.Procedural = false;
				terrainMeta = terrain.GetComponent<TerrainMeta>();
				terrainMeta.Init();
				terrainMeta.SetupComponents();
				terrainMeta.BindShaderProperties();
				terrainMeta.PostSetupComponents();
				World.InitSize(Mathf.RoundToInt(TerrainMeta.Size.x));
				CreateObject(decorPrefab);
				CreateObject(grassPrefab);
				CreateObject(spawnPrefab);
			}
		}
		World.Serialization = new WorldSerialization();
		World.Cached = false;
		World.CleanupOldFiles();
		if (!string.IsNullOrEmpty(EditorConfigString))
		{
			ConVar.World.configString = EditorConfigString;
		}
		if (!string.IsNullOrEmpty(EditorConfigFile))
		{
			ConVar.World.configFile = EditorConfigFile;
		}
		if (AutomaticallySetup)
		{
			((MonoBehaviour)this).StartCoroutine(InitCoroutine());
		}
	}

	protected void CreateObject(GameObject prefab)
	{
		if (!((Object)(object)prefab == (Object)null))
		{
			GameObject val = Object.Instantiate<GameObject>(prefab);
			if ((Object)(object)val != (Object)null)
			{
				val.SetActive(true);
			}
		}
	}

	public IEnumerator InitCoroutine()
	{
		if (World.CanLoadFromUrl())
		{
			Debug.Log((object)("Loading custom map from " + World.Url));
		}
		else
		{
			Debug.Log((object)("Generating procedural map of size " + World.Size + " with seed " + World.Seed));
		}
		World.Config = new WorldConfig();
		if (!string.IsNullOrEmpty(ConVar.World.configString))
		{
			Debug.Log((object)"Loading custom world config from world.configstring convar");
			World.Config.LoadFromJsonString(ConVar.World.configString);
		}
		else if (!string.IsNullOrEmpty(ConVar.World.configFile))
		{
			string text = ConVar.Server.rootFolder + "/" + ConVar.World.configFile;
			Debug.Log((object)("Loading custom world config from world.configfile convar: " + text));
			World.Config.LoadFromJsonFile(text);
		}
		ProceduralComponent[] components = ((Component)this).GetComponentsInChildren<ProceduralComponent>(true);
		Timing downloadTimer = Timing.Start("Downloading World");
		if (World.Procedural && !World.CanLoadFromDisk() && World.CanLoadFromUrl())
		{
			LoadingScreen.Update("DOWNLOADING WORLD");
			yield return CoroutineEx.waitForEndOfFrame;
			yield return CoroutineEx.waitForEndOfFrame;
			yield return CoroutineEx.waitForEndOfFrame;
			UnityWebRequest request = UnityWebRequest.Get(World.Url);
			request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
			request.Send();
			while (!request.isDone)
			{
				LoadingScreen.Update("DOWNLOADING WORLD " + (request.downloadProgress * 100f).ToString("0.0") + "%");
				yield return CoroutineEx.waitForEndOfFrame;
			}
			if (!request.isHttpError && !request.isNetworkError)
			{
				File.WriteAllBytes(World.MapFolderName + "/" + World.MapFileName, request.downloadHandler.data);
			}
			else
			{
				CancelSetup("Couldn't Download Level: " + World.Name + " (" + request.error + ")");
			}
		}
		downloadTimer.End();
		Timing loadTimer = Timing.Start("Loading World");
		if (World.Procedural && World.CanLoadFromDisk())
		{
			LoadingScreen.Update("LOADING WORLD");
			yield return CoroutineEx.waitForEndOfFrame;
			yield return CoroutineEx.waitForEndOfFrame;
			yield return CoroutineEx.waitForEndOfFrame;
			World.Serialization.Load(World.MapFolderName + "/" + World.MapFileName);
			World.Cached = true;
		}
		loadTimer.End();
		if (World.Cached && 9 != World.Serialization.Version)
		{
			Debug.LogWarning((object)("World cache version mismatch: " + 9u + " != " + World.Serialization.Version));
			World.Serialization.Clear();
			World.Cached = false;
			if (World.CanLoadFromUrl())
			{
				CancelSetup("World File Outdated: " + World.Name);
			}
		}
		if (World.Cached && string.IsNullOrEmpty(World.Checksum))
		{
			World.Checksum = World.Serialization.Checksum;
		}
		if (World.Cached)
		{
			World.InitSize(World.Serialization.world.size);
		}
		if (Object.op_Implicit((Object)(object)terrain))
		{
			TerrainGenerator component2 = terrain.GetComponent<TerrainGenerator>();
			if (Object.op_Implicit((Object)(object)component2))
			{
				if (World.Cached)
				{
					int cachedHeightMapResolution = World.GetCachedHeightMapResolution();
					int cachedSplatMapResolution = World.GetCachedSplatMapResolution();
					terrain = component2.CreateTerrain(cachedHeightMapResolution, cachedSplatMapResolution);
				}
				else
				{
					terrain = component2.CreateTerrain();
				}
				terrainMeta = terrain.GetComponent<TerrainMeta>();
				terrainMeta.Init();
				terrainMeta.SetupComponents();
				CreateObject(decorPrefab);
				CreateObject(grassPrefab);
				CreateObject(spawnPrefab);
			}
		}
		Timing spawnTimer = Timing.Start("Spawning World");
		if (World.Cached)
		{
			LoadingScreen.Update("SPAWNING WORLD");
			yield return CoroutineEx.waitForEndOfFrame;
			yield return CoroutineEx.waitForEndOfFrame;
			yield return CoroutineEx.waitForEndOfFrame;
			TerrainMeta.HeightMap.FromByteArray(World.GetMap("terrain"));
			TerrainMeta.SplatMap.FromByteArray(World.GetMap("splat"));
			TerrainMeta.BiomeMap.FromByteArray(World.GetMap("biome"));
			TerrainMeta.TopologyMap.FromByteArray(World.GetMap("topology"));
			TerrainMeta.AlphaMap.FromByteArray(World.GetMap("alpha"));
			TerrainMeta.WaterMap.FromByteArray(World.GetMap("water"));
			IEnumerator worldSpawn = ((Global.preloadConcurrency > 1) ? World.SpawnAsync(0.2f, delegate(string str)
			{
				LoadingScreen.Update(str);
			}) : World.Spawn(0.2f, delegate(string str)
			{
				LoadingScreen.Update(str);
			}));
			while (worldSpawn.MoveNext())
			{
				yield return worldSpawn.Current;
			}
			TerrainMeta.Path.Clear();
			TerrainMeta.Path.Roads.AddRange(World.GetPaths("Road"));
			TerrainMeta.Path.Rivers.AddRange(World.GetPaths("River"));
			TerrainMeta.Path.Powerlines.AddRange(World.GetPaths("Powerline"));
			TerrainMeta.Path.Rails.AddRange(World.GetPaths("Rail"));
		}
		if ((Object)(object)TerrainMeta.Path != (Object)null)
		{
			foreach (DungeonBaseLink dungeonBaseLink in TerrainMeta.Path.DungeonBaseLinks)
			{
				if ((Object)(object)dungeonBaseLink != (Object)null)
				{
					dungeonBaseLink.Initialize();
				}
			}
		}
		spawnTimer.End();
		Timing procgenTimer = Timing.Start("Processing World");
		if (components.Length != 0)
		{
			for (int i = 0; i < components.Length; i++)
			{
				ProceduralComponent component = components[i];
				if (Object.op_Implicit((Object)(object)component) && component.ShouldRun())
				{
					uint seed = (uint)(World.Seed + i);
					LoadingScreen.Update(component.Description.ToUpper());
					yield return CoroutineEx.waitForEndOfFrame;
					yield return CoroutineEx.waitForEndOfFrame;
					yield return CoroutineEx.waitForEndOfFrame;
					Timing timing = Timing.Start(component.Description);
					if (Object.op_Implicit((Object)(object)component))
					{
						component.Process(seed);
					}
					timing.End();
				}
			}
		}
		procgenTimer.End();
		Timing saveTimer = Timing.Start("Saving World");
		if (ConVar.World.cache && World.Procedural && !World.Cached)
		{
			LoadingScreen.Update("SAVING WORLD");
			yield return CoroutineEx.waitForEndOfFrame;
			yield return CoroutineEx.waitForEndOfFrame;
			yield return CoroutineEx.waitForEndOfFrame;
			World.Serialization.world.size = World.Size;
			World.AddPaths(TerrainMeta.Path.Roads);
			World.AddPaths(TerrainMeta.Path.Rivers);
			World.AddPaths(TerrainMeta.Path.Powerlines);
			World.AddPaths(TerrainMeta.Path.Rails);
			World.Serialization.Save(World.MapFolderName + "/" + World.MapFileName);
		}
		saveTimer.End();
		Timing checksumTimer = Timing.Start("Calculating Checksum");
		if (string.IsNullOrEmpty(World.Serialization.Checksum))
		{
			LoadingScreen.Update("CALCULATING CHECKSUM");
			yield return CoroutineEx.waitForEndOfFrame;
			yield return CoroutineEx.waitForEndOfFrame;
			yield return CoroutineEx.waitForEndOfFrame;
			World.Serialization.CalculateChecksum();
		}
		checksumTimer.End();
		if (string.IsNullOrEmpty(World.Checksum))
		{
			World.Checksum = World.Serialization.Checksum;
		}
		Timing oceanTimer = Timing.Start("Ocean Patrol Paths");
		LoadingScreen.Update("OCEAN PATROL PATHS");
		yield return CoroutineEx.waitForEndOfFrame;
		yield return CoroutineEx.waitForEndOfFrame;
		yield return CoroutineEx.waitForEndOfFrame;
		if (BaseBoat.generate_paths && (Object)(object)TerrainMeta.Path != (Object)null)
		{
			TerrainMeta.Path.OceanPatrolFar = BaseBoat.GenerateOceanPatrolPath(200f);
		}
		else
		{
			Debug.Log((object)"Skipping ocean patrol paths, baseboat.generate_paths == false");
		}
		oceanTimer.End();
		Timing finalizeTimer = Timing.Start("Finalizing World");
		LoadingScreen.Update("FINALIZING WORLD");
		yield return CoroutineEx.waitForEndOfFrame;
		yield return CoroutineEx.waitForEndOfFrame;
		yield return CoroutineEx.waitForEndOfFrame;
		if (Object.op_Implicit((Object)(object)terrainMeta))
		{
			terrainMeta.BindShaderProperties();
			terrainMeta.PostSetupComponents();
			TerrainMargin.Create();
		}
		finalizeTimer.End();
		Timing cleaningTimer = Timing.Start("Cleaning Up");
		LoadingScreen.Update("CLEANING UP");
		yield return CoroutineEx.waitForEndOfFrame;
		yield return CoroutineEx.waitForEndOfFrame;
		yield return CoroutineEx.waitForEndOfFrame;
		foreach (string item in FileSystem.Backend.UnloadBundles("monuments"))
		{
			GameManager.server.preProcessed.Invalidate(item);
			GameManifest.Invalidate(item);
			PrefabAttribute.server.Invalidate(StringPool.Get(item));
		}
		Resources.UnloadUnusedAssets();
		cleaningTimer.End();
		LoadingScreen.Update("DONE");
		yield return CoroutineEx.waitForEndOfFrame;
		yield return CoroutineEx.waitForEndOfFrame;
		yield return CoroutineEx.waitForEndOfFrame;
		if (Object.op_Implicit((Object)(object)this))
		{
			GameManager.Destroy(((Component)this).gameObject);
		}
	}

	private void CancelSetup(string msg)
	{
		Debug.LogError((object)msg);
		Application.Quit();
	}
}
