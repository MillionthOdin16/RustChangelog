using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;
using UnityEngine.Profiling;

[CreateAssetMenu(menuName = "Rust/Hair Set")]
public class HairSet : ScriptableObject
{
	[Serializable]
	public class MeshReplace
	{
		[HideInInspector]
		public string FindName;

		public Mesh Find;

		public Mesh[] ReplaceShapes;

		public bool Test(string materialName)
		{
			return FindName == materialName;
		}
	}

	public MeshReplace[] MeshReplacements;

	public void Process(PlayerModelHair playerModelHair, HairDyeCollection dyeCollection, HairDye dye, MaterialPropertyBlock block)
	{
		List<SkinnedMeshRenderer> list = Pool.GetList<SkinnedMeshRenderer>();
		((Component)playerModelHair).gameObject.GetComponentsInChildren<SkinnedMeshRenderer>(true, list);
		foreach (SkinnedMeshRenderer item in list)
		{
			if ((Object)(object)item.sharedMesh == (Object)null || (Object)(object)((Renderer)item).sharedMaterial == (Object)null)
			{
				continue;
			}
			string name = ((Object)item.sharedMesh).name;
			string name2 = ((Object)((Renderer)item).sharedMaterial).name;
			if (!((Component)item).gameObject.activeSelf)
			{
				((Component)item).gameObject.SetActive(true);
			}
			for (int i = 0; i < MeshReplacements.Length; i++)
			{
				Profiler.BeginSample("MeshReplace");
				if (MeshReplacements[i].Test(name))
				{
				}
				Profiler.EndSample();
			}
			Profiler.BeginSample("ApplyHairDye");
			if (dye != null && ((Component)item).gameObject.activeSelf)
			{
				dye.Apply(dyeCollection, block);
			}
			Profiler.EndSample();
		}
		Pool.FreeList<SkinnedMeshRenderer>(ref list);
	}

	public void ProcessMorphs(GameObject obj, int blendShapeIndex = -1)
	{
	}
}
