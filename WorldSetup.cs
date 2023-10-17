using System.Collections;
using System.Collections.Generic;
using System.IO;
using ConVar;
using Rust;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Profiling;

public class WorldSetup : SingletonComponent<WorldSetup>
{
	public bool AutomaticallySetup = false;

	public GameObject terrain;

	public GameObject decorPrefab;

	public GameObject grassPrefab;

	public GameObject spawnPrefab;

	private TerrainMeta terrainMeta;

	public uint EditorSeed = 0u;

	public uint EditorSalt = 0u;

	public uint EditorSize = 0u;

	public string EditorUrl = string.Empty;

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
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Expected O, but got Unknown
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		((SingletonComponent)this).Awake();
		Prefab[] array = Prefab.Load("assets/bundled/prefabs/world");
		foreach (Prefab prefab in array)
		{
			Profiler.BeginSample("WorldSetup - " + prefab.Name);
			if ((Object)(object)prefab.Object.GetComponent<BaseEntity>() != (Object)null)
			{
				BaseEntity baseEntity = prefab.SpawnEntity(Vector3.zero, Quaternion.identity);
				baseEntity.Spawn();
			}
			else
			{
				prefab.Spawn(Vector3.zero, Quaternion.identity);
			}
			Profiler.EndSample();
		}
		SingletonComponent[] array2 = Object.FindObjectsOfType<SingletonComponent>();
		foreach (SingletonComponent val in array2)
		{
			val.SingletonSetup();
		}
		if (Object.op_Implicit((Object)(object)terrain))
		{
			TerrainGenerator component = terrain.GetComponent<TerrainGenerator>();
			if (Object.op_Implicit((Object)(object)component))
			{
				World.Procedural = true;
			}
			else
			{
				World.Procedural = false;
				Profiler.BeginSample("TerrainMeta.Setup");
				terrainMeta = terrain.GetComponent<TerrainMeta>();
				terrainMeta.Init();
				terrainMeta.SetupComponents();
				terrainMeta.BindShaderProperties();
				terrainMeta.PostSetupComponents();
				Profiler.EndSample();
				World.InitSize(Mathf.RoundToInt(TerrainMeta.Size.x));
				CreateObject(decorPrefab);
				CreateObject(grassPrefab);
				CreateObject(spawnPrefab);
			}
		}
		World.Serialization = new WorldSerialization();
		World.Cached = false;
		World.CleanupOldFiles();
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
			TerrainGenerator terrainGenerator = terrain.GetComponent<TerrainGenerator>();
			if (Object.op_Implicit((Object)(object)terrainGenerator))
			{
				if (World.Cached)
				{
					int heightmapResolution = World.GetCachedHeightMapResolution();
					int alphamapResolution = World.GetCachedSplatMapResolution();
					terrain = terrainGenerator.CreateTerrain(heightmapResolution, alphamapResolution);
				}
				else
				{
					terrain = terrainGenerator.CreateTerrain();
				}
				Profiler.BeginSample("TerrainMeta.Setup");
				terrainMeta = terrain.GetComponent<TerrainMeta>();
				terrainMeta.Init();
				terrainMeta.SetupComponents();
				Profiler.EndSample();
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
					Timing timer = Timing.Start(component.Description);
					if (Object.op_Implicit((Object)(object)component))
					{
						component.Process(seed);
					}
					timer.End();
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
		List<string> invalidAssets = FileSystem.Backend.UnloadBundles("monuments");
		foreach (string assetName in invalidAssets)
		{
			GameManager.server.preProcessed.Invalidate(assetName);
			GameManifest.Invalidate(assetName);
			PrefabAttribute.server.Invalidate(StringPool.Get(assetName));
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
