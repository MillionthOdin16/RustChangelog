using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class BoneDictionary
{
	public Transform transform;

	public Transform[] transforms;

	public string[] names;

	private Dictionary<string, Transform> nameDict = new Dictionary<string, Transform>(StringComparer.OrdinalIgnoreCase);

	private Dictionary<uint, Transform> hashDict = new Dictionary<uint, Transform>();

	private Dictionary<Transform, uint> transformDict = new Dictionary<Transform, uint>();

	public int Count => transforms.Length;

	public BoneDictionary(Transform rootBone)
	{
		Profiler.BeginSample("BuildBoneDictionary");
		transform = rootBone;
		transforms = ((Component)rootBone).GetComponentsInChildren<Transform>(true);
		names = new string[transforms.Length];
		for (int i = 0; i < transforms.Length; i++)
		{
			Transform val = transforms[i];
			if ((Object)(object)val != (Object)null)
			{
				names[i] = ((Object)val).name;
			}
		}
		BuildBoneDictionary();
		Profiler.EndSample();
	}

	public BoneDictionary(Transform rootBone, Transform[] boneTransforms, string[] boneNames)
	{
		Profiler.BeginSample("BuildBoneDictionary");
		transform = rootBone;
		transforms = boneTransforms;
		names = boneNames;
		BuildBoneDictionary();
		Profiler.EndSample();
	}

	private void BuildBoneDictionary()
	{
		for (int i = 0; i < transforms.Length; i++)
		{
			Transform val = transforms[i];
			string text = names[i];
			uint num = StringPool.Get(text);
			if (!nameDict.ContainsKey(text))
			{
				nameDict.Add(text, val);
			}
			if (!hashDict.ContainsKey(num))
			{
				hashDict.Add(num, val);
			}
			if ((Object)(object)val != (Object)null && !transformDict.ContainsKey(val))
			{
				transformDict.Add(val, num);
			}
		}
	}

	public Transform FindBone(string name, bool defaultToRoot = true)
	{
		Transform value = null;
		if (nameDict.TryGetValue(name, out value))
		{
			return value;
		}
		return defaultToRoot ? transform : null;
	}

	public Transform FindBone(uint hash, bool defaultToRoot = true)
	{
		Transform value = null;
		if (hashDict.TryGetValue(hash, out value))
		{
			return value;
		}
		return defaultToRoot ? transform : null;
	}

	public uint FindBoneID(Transform transform)
	{
		uint value;
		return transformDict.TryGetValue(transform, out value) ? value : StringPool.closest;
	}
}
