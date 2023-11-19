using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using ConVar;
using ProtoBuf;
using UnityEngine;

public static class World
{
	public static uint Seed { get; set; }

	public static uint Salt { get; set; }

	public static uint Size { get; set; }

	public static string Checksum { get; set; }

	public static string Url { get; set; }

	public static bool Procedural { get; set; }

	public static bool Cached { get; set; }

	public static bool Networked { get; set; }

	public static bool Receiving { get; set; }

	public static bool Transfer { get; set; }

	public static bool LoadedFromSave { get; set; }

	public static int SpawnIndex { get; set; }

	public static WorldSerialization Serialization { get; set; }

	public static string Name
	{
		[MethodImpl(MethodImplOptions.NoInlining)]
		get
		{
			if (CanLoadFromUrl())
			{
				return Path.GetFileNameWithoutExtension(WWW.UnEscapeURL(Url));
			}
			return Application.loadedLevelName;
		}
	}

	public static string MapFileName
	{
		[MethodImpl(MethodImplOptions.NoInlining)]
		get
		{
			if (CanLoadFromUrl())
			{
				return Name + ".map";
			}
			return Name.Replace(" ", "").ToLower() + "." + Size + "." + Seed + "." + 239 + ".map";
		}
	}

	public static string MapFolderName
	{
		[MethodImpl(MethodImplOptions.NoInlining)]
		get
		{
			return Server.rootFolder;
		}
	}

	public static string SaveFileName
	{
		[MethodImpl(MethodImplOptions.NoInlining)]
		get
		{
			if (CanLoadFromUrl())
			{
				return Name + "." + 239 + ".sav";
			}
			return Name.Replace(" ", "").ToLower() + "." + Size + "." + Seed + "." + 239 + ".sav";
		}
	}

	public static string SaveFolderName
	{
		[MethodImpl(MethodImplOptions.NoInlining)]
		get
		{
			return Server.rootFolder;
		}
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public static string GetServerBrowserMapName()
	{
		if (!CanLoadFromUrl())
		{
			return Name;
		}
		if (Name.StartsWith("proceduralmap."))
		{
			return "Procedural Map";
		}
		return "Custom Map";
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public static bool CanLoadFromUrl()
	{
		return !string.IsNullOrEmpty(Url);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	public static bool CanLoadFromDisk()
	{
		return File.Exists(MapFolderName + "/" + MapFileName);
	}

	public static void CleanupOldFiles()
	{
		Regex regex1 = new Regex("proceduralmap\\.[0-9]+\\.[0-9]+\\.[0-9]+\\.map");
		Regex regex2 = new Regex("\\.[0-9]+\\.[0-9]+\\." + 239 + "\\.map");
		IEnumerable<string> enumerable = from path in Directory.GetFiles(MapFolderName, "*.map")
			where regex1.IsMatch(path) && !regex2.IsMatch(path)
			select path;
		foreach (string item in enumerable)
		{
			try
			{
				File.Delete(item);
			}
			catch (Exception ex)
			{
				Debug.LogError((object)ex.Message);
			}
		}
	}

	public static void InitSeed(int seed)
	{
		InitSeed((uint)seed);
	}

	public static void InitSeed(uint seed)
	{
		if (seed == 0)
		{
			seed = SeedIdentifier().MurmurHashUnsigned() % int.MaxValue;
		}
		if (seed == 0)
		{
			seed = 123456u;
		}
		Seed = seed;
		Server.seed = (int)seed;
	}

	private static string SeedIdentifier()
	{
		return SystemInfo.deviceUniqueIdentifier + "_" + 239 + "_" + Server.identity;
	}

	public static void InitSalt(int salt)
	{
		InitSalt((uint)salt);
	}

	public static void InitSalt(uint salt)
	{
		if (salt == 0)
		{
			salt = SaltIdentifier().MurmurHashUnsigned() % int.MaxValue;
		}
		if (salt == 0)
		{
			salt = 654321u;
		}
		Salt = salt;
		Server.salt = (int)salt;
	}

	private static string SaltIdentifier()
	{
		return SystemInfo.deviceUniqueIdentifier + "_salt";
	}

	public static void InitSize(int size)
	{
		InitSize((uint)size);
	}

	public static void InitSize(uint size)
	{
		if (size == 0)
		{
			size = 4500u;
		}
		if (size < 1000)
		{
			size = 1000u;
		}
		if (size > 6000)
		{
			size = 6000u;
		}
		Size = size;
		Server.worldsize = (int)size;
	}

	public static byte[] GetMap(string name)
	{
		return Serialization.GetMap(name)?.data;
	}

	public static int GetCachedHeightMapResolution()
	{
		return Mathf.RoundToInt(Mathf.Sqrt((float)(GetMap("height").Length / 2)));
	}

	public static int GetCachedSplatMapResolution()
	{
		return Mathf.RoundToInt(Mathf.Sqrt((float)(GetMap("splat").Length / 8)));
	}

	public static void AddMap(string name, byte[] data)
	{
		Serialization.AddMap(name, data);
	}

	public static void AddPrefab(string category, Prefab prefab, Vector3 position, Quaternion rotation, Vector3 scale)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		Serialization.AddPrefab(category, prefab.ID, position, rotation, scale);
		if (!Cached)
		{
			rotation = Quaternion.Euler(((Quaternion)(ref rotation)).eulerAngles);
			Spawn(category, prefab, position, rotation, scale);
		}
	}

	public static PathData PathListToPathData(PathList src)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		PathData val = new PathData();
		val.name = src.Name;
		val.spline = src.Spline;
		val.start = src.Start;
		val.end = src.End;
		val.width = src.Width;
		val.innerPadding = src.InnerPadding;
		val.outerPadding = src.OuterPadding;
		val.innerFade = src.InnerFade;
		val.outerFade = src.OuterFade;
		val.randomScale = src.RandomScale;
		val.meshOffset = src.MeshOffset;
		val.terrainOffset = src.TerrainOffset;
		val.splat = src.Splat;
		val.topology = src.Topology;
		val.hierarchy = src.Hierarchy;
		val.nodes = VectorArrayToList(src.Path.Points);
		return val;
	}

	public static PathList PathDataToPathList(PathData src)
	{
		PathList pathList = new PathList(src.name, VectorListToArray(src.nodes));
		pathList.Spline = src.spline;
		pathList.Start = src.start;
		pathList.End = src.end;
		pathList.Width = src.width;
		pathList.InnerPadding = src.innerPadding;
		pathList.OuterPadding = src.outerPadding;
		pathList.InnerFade = src.innerFade;
		pathList.OuterFade = src.outerFade;
		pathList.RandomScale = src.randomScale;
		pathList.MeshOffset = src.meshOffset;
		pathList.TerrainOffset = src.terrainOffset;
		pathList.Splat = src.splat;
		pathList.Topology = src.topology;
		pathList.Hierarchy = src.hierarchy;
		pathList.Path.RecalculateTangents();
		return pathList;
	}

	public static Vector3[] VectorListToArray(List<VectorData> src)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		Vector3[] array = (Vector3[])(object)new Vector3[src.Count];
		for (int i = 0; i < array.Length; i++)
		{
			VectorData val = src[i];
			Vector3 val2 = default(Vector3);
			val2.x = val.x;
			val2.y = val.y;
			val2.z = val.z;
			array[i] = val2;
		}
		return array;
	}

	public static List<VectorData> VectorArrayToList(Vector3[] src)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		List<VectorData> list = new List<VectorData>(src.Length);
		foreach (Vector3 val in src)
		{
			VectorData item = default(VectorData);
			item.x = val.x;
			item.y = val.y;
			item.z = val.z;
			list.Add(item);
		}
		return list;
	}

	public static IEnumerable<PathList> GetPaths(string name)
	{
		return from p in Serialization.GetPaths(name)
			select PathDataToPathList(p);
	}

	public static void AddPaths(IEnumerable<PathList> paths)
	{
		foreach (PathList path in paths)
		{
			AddPath(path);
		}
	}

	public static void AddPath(PathList path)
	{
		Serialization.AddPath(PathListToPathData(path));
	}

	public static IEnumerator SpawnAsync(float deltaTime, Action<string> statusFunction = null)
	{
		int totalCount = 0;
		Dictionary<string, List<PrefabData>> assetGroups = new Dictionary<string, List<PrefabData>>(StringComparer.InvariantCultureIgnoreCase);
		foreach (PrefabData prefab2 in Serialization.world.prefabs)
		{
			string str = StringPool.Get(prefab2.id);
			if (string.IsNullOrWhiteSpace(str))
			{
				Debug.LogWarning((object)$"Could not find path for prefab ID {prefab2.id}, skipping spawn");
				continue;
			}
			if (!assetGroups.TryGetValue(str, out var prefabDatas))
			{
				prefabDatas = new List<PrefabData>();
				assetGroups.Add(str, prefabDatas);
			}
			prefabDatas.Add(prefab2);
			totalCount++;
			prefabDatas = null;
		}
		int spawnedCount = 0;
		int resultIndex = 0;
		Stopwatch sw = Stopwatch.StartNew();
		AssetPreloadResult load = FileSystem.PreloadAssets((IEnumerable<string>)assetGroups.Keys, Global.preloadConcurrency, 10);
		while (load != null && (((CustomYieldInstruction)load).MoveNext() || assetGroups.Count > 0))
		{
			while (resultIndex < load.Results.Count && sw.Elapsed.TotalSeconds < (double)deltaTime)
			{
				string assetPath = load.Results[resultIndex].Item1;
				if (!assetGroups.TryGetValue(assetPath, out var prefabs))
				{
					resultIndex++;
					continue;
				}
				if (prefabs.Count == 0)
				{
					assetGroups.Remove(assetPath);
					resultIndex++;
					continue;
				}
				int prefabIdx = prefabs.Count - 1;
				PrefabData prefab = prefabs[prefabIdx];
				prefabs.RemoveAt(prefabIdx);
				Spawn(prefab);
				spawnedCount++;
				prefabs = null;
			}
			Status(statusFunction, "Spawning World ({0}/{1})", spawnedCount, totalCount);
			yield return CoroutineEx.waitForEndOfFrame;
			sw.Restart();
		}
	}

	public static IEnumerator Spawn(float deltaTime, Action<string> statusFunction = null)
	{
		Stopwatch sw = Stopwatch.StartNew();
		for (int i = 0; i < Serialization.world.prefabs.Count; i++)
		{
			if (sw.Elapsed.TotalSeconds > (double)deltaTime || i == 0 || i == Serialization.world.prefabs.Count - 1)
			{
				Status(statusFunction, "Spawning World ({0}/{1})", i + 1, Serialization.world.prefabs.Count);
				yield return CoroutineEx.waitForEndOfFrame;
				sw.Reset();
				sw.Start();
			}
			Spawn(Serialization.world.prefabs[i]);
		}
	}

	public static void Spawn()
	{
		for (int i = 0; i < Serialization.world.prefabs.Count; i++)
		{
			Spawn(Serialization.world.prefabs[i]);
		}
	}

	public static void Spawn(string category, string folder = null)
	{
		for (int i = SpawnIndex; i < Serialization.world.prefabs.Count; i++)
		{
			PrefabData val = Serialization.world.prefabs[i];
			if (val.category != category)
			{
				break;
			}
			string text = StringPool.Get(val.id);
			if (!string.IsNullOrEmpty(folder) && !text.StartsWith(folder))
			{
				break;
			}
			Spawn(val);
			SpawnIndex++;
		}
	}

	public static void Spawn(string category, string[] folders)
	{
		for (int i = SpawnIndex; i < Serialization.world.prefabs.Count; i++)
		{
			PrefabData val = Serialization.world.prefabs[i];
			if (val.category != category)
			{
				break;
			}
			string text = StringPool.Get(val.id);
			if (folders != null && !StringEx.StartsWithAny(text, folders))
			{
				break;
			}
			Spawn(val);
			SpawnIndex++;
		}
	}

	private static void Spawn(PrefabData prefab)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Spawn(prefab.category, Prefab.Load(prefab.id), VectorData.op_Implicit(prefab.position), VectorData.op_Implicit(prefab.rotation), VectorData.op_Implicit(prefab.scale));
	}

	private static void Spawn(string category, Prefab prefab, Vector3 position, Quaternion rotation, Vector3 scale)
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		if (prefab != null && Object.op_Implicit((Object)(object)prefab.Object))
		{
			if (!Cached)
			{
				prefab.ApplyTerrainPlacements(position, rotation, scale);
				prefab.ApplyTerrainModifiers(position, rotation, scale);
			}
			GameObject val = prefab.Spawn(position, rotation, scale);
			if (Object.op_Implicit((Object)(object)val))
			{
				val.SetHierarchyGroup(category);
			}
		}
	}

	private static void Status(Action<string> statusFunction, string status, object obj1)
	{
		statusFunction?.Invoke(string.Format(status, obj1));
	}

	private static void Status(Action<string> statusFunction, string status, object obj1, object obj2)
	{
		statusFunction?.Invoke(string.Format(status, obj1, obj2));
	}

	private static void Status(Action<string> statusFunction, string status, object obj1, object obj2, object obj3)
	{
		statusFunction?.Invoke(string.Format(status, obj1, obj2, obj3));
	}

	private static void Status(Action<string> statusFunction, string status, params object[] objs)
	{
		statusFunction?.Invoke(string.Format(status, objs));
	}
}
