using System;
using System.Collections.Generic;
using UnityEngine;

public static class HierarchyUtil
{
	public static Dictionary<string, GameObject> rootDict = new Dictionary<string, GameObject>(StringComparer.OrdinalIgnoreCase);

	public static GameObject GetRoot(string strName, bool groupActive = true, bool persistant = false)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Expected O, but got Unknown
		if (rootDict.TryGetValue(strName, out var value))
		{
			if ((Object)(object)value != (Object)null)
			{
				return value;
			}
			rootDict.Remove(strName);
		}
		value = new GameObject(strName);
		value.SetActive(groupActive);
		rootDict.Add(strName, value);
		if (persistant)
		{
			Object.DontDestroyOnLoad((Object)(object)value);
		}
		return value;
	}
}
