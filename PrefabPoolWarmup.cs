using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using ConVar;
using Rust;
using UnityEngine;

public class PrefabPoolWarmup
{
	public static void Run(string filter = null, int countOverride = 0)
	{
		if (Application.isLoadingPrefabs)
		{
			return;
		}
		Application.isLoadingPrefabs = true;
		string[] assetList = GetAssetList();
		if (string.IsNullOrEmpty(filter))
		{
			for (int i = 0; i < assetList.Length; i++)
			{
				PrefabWarmup(assetList[i], countOverride);
			}
		}
		else
		{
			foreach (string text in assetList)
			{
				if (StringEx.Contains(text, filter, CompareOptions.IgnoreCase))
				{
					PrefabWarmup(text, countOverride);
				}
			}
		}
		Application.isLoadingPrefabs = false;
	}

	public static IEnumerator Run(float deltaTime, Action<string> statusFunction = null, string format = null)
	{
		if (Application.isEditor || Application.isLoadingPrefabs || !Pool.prewarm)
		{
			yield break;
		}
		Application.isLoadingPrefabs = true;
		string[] prewarmAssets = GetAssetList();
		Stopwatch sw = Stopwatch.StartNew();
		for (int i = 0; i < prewarmAssets.Length; i++)
		{
			if (sw.Elapsed.TotalSeconds > (double)deltaTime || i == 0 || i == prewarmAssets.Length - 1)
			{
				statusFunction?.Invoke(string.Format((format != null) ? format : "{0}/{1}", i + 1, prewarmAssets.Length));
				yield return CoroutineEx.waitForEndOfFrame;
				sw.Reset();
				sw.Start();
			}
			PrefabWarmup(prewarmAssets[i]);
		}
		Application.isLoadingPrefabs = false;
	}

	public static string[] GetAssetList()
	{
		return (from x in GameManifest.Current.prefabProperties
			where x.pool
			select x.name).ToArray();
	}

	private static void PrefabWarmup(string path, int countOverride = 0)
	{
		if (string.IsNullOrEmpty(path))
		{
			return;
		}
		GameObject val = GameManager.server.FindPrefab(path);
		if ((Object)(object)val != (Object)null && val.SupportsPooling())
		{
			int num = val.GetComponent<Poolable>().ServerCount;
			List<GameObject> list = new List<GameObject>();
			if (num > 0 && countOverride > 0)
			{
				num = countOverride;
			}
			for (int i = 0; i < num; i++)
			{
				list.Add(GameManager.server.CreatePrefab(path));
			}
			for (int j = 0; j < num; j++)
			{
				GameManager.server.Retire(list[j]);
			}
		}
	}
}
