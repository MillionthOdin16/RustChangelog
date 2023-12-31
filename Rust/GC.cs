using System;
using UnityEngine;

namespace Rust;

public class GC : MonoBehaviour, IClientComponent
{
	public static int gcLowerBounds = 64;

	public static int gcDefaultValue = 256;

	public static int gcEditorDefaultValue = 4096;

	public static bool Enabled => true;

	public static void Collect()
	{
		System.GC.Collect();
	}

	public static int GetSafeGCValue(int val)
	{
		return Mathf.Clamp(val, gcLowerBounds, Mathf.Min(4096, SystemInfo.systemMemorySize / 8));
	}

	public static long GetTotalMemory()
	{
		return System.GC.GetTotalMemory(forceFullCollection: false) / 1048576;
	}

	public static int CollectionCount()
	{
		return System.GC.CollectionCount(0);
	}
}
