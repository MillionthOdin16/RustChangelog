using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Rust/Skeleton Properties")]
public class SkeletonProperties : ScriptableObject
{
	[Serializable]
	public class BoneProperty
	{
		public GameObject bone;

		public Phrase name;

		public HitArea area;
	}

	public GameObject boneReference;

	[BoneProperty]
	public BoneProperty[] bones;

	[NonSerialized]
	private Dictionary<uint, BoneProperty> quickLookup = null;

	public void OnValidate()
	{
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Expected O, but got Unknown
		if ((Object)(object)boneReference == (Object)null)
		{
			Debug.LogWarning((object)("boneReference is null on " + ((Object)this).name), (Object)(object)this);
			return;
		}
		List<BoneProperty> list = bones.ToList();
		List<Transform> allChildren = boneReference.transform.GetAllChildren();
		foreach (Transform child in allChildren)
		{
			if (list.All((BoneProperty x) => (Object)(object)x.bone != (Object)(object)((Component)child).gameObject))
			{
				list.Add(new BoneProperty
				{
					bone = ((Component)child).gameObject,
					name = new Phrase("", "")
					{
						token = ((Object)child).name.ToLower(),
						english = ((Object)child).name.ToLower()
					}
				});
			}
		}
		bones = list.ToArray();
	}

	private void BuildDictionary()
	{
		quickLookup = new Dictionary<uint, BoneProperty>();
		if ((Object)(object)boneReference == (Object)null)
		{
			Debug.LogWarning((object)("boneReference is null on " + ((Object)this).name), (Object)(object)this);
			return;
		}
		BoneProperty[] array = bones;
		foreach (BoneProperty boneProperty in array)
		{
			if (boneProperty == null || (Object)(object)boneProperty.bone == (Object)null || ((Object)boneProperty.bone).name == null)
			{
				Debug.LogWarning((object)("Bone error in SkeletonProperties.BuildDictionary for " + ((Object)boneReference).name));
				continue;
			}
			uint num = StringPool.Get(((Object)boneProperty.bone).name);
			if (!quickLookup.ContainsKey(num))
			{
				quickLookup.Add(num, boneProperty);
				continue;
			}
			string name = ((Object)boneProperty.bone).name;
			string name2 = ((Object)quickLookup[num].bone).name;
			Debug.LogWarning((object)("Duplicate bone id " + num + " for " + name + " and " + name2));
		}
	}

	public BoneProperty FindBone(uint id)
	{
		if (quickLookup == null)
		{
			BuildDictionary();
		}
		BoneProperty value = null;
		if (!quickLookup.TryGetValue(id, out value))
		{
			return null;
		}
		return value;
	}
}
