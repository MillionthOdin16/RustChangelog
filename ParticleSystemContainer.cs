using System;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemContainer : MonoBehaviour, IPrefabPreProcess
{
	[Serializable]
	public struct ParticleSystemGroup
	{
		public ParticleSystem system;

		public LODComponentParticleSystem[] lodComponents;
	}

	public bool precached = false;

	[HideInInspector]
	public ParticleSystemGroup[] particleGroups = null;

	public void Play()
	{
	}

	public void Pause()
	{
	}

	public void Stop()
	{
	}

	public void Clear()
	{
	}

	public void PreProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		if (precached && clientside)
		{
			List<ParticleSystemGroup> list = new List<ParticleSystemGroup>();
			ParticleSystem[] componentsInChildren = ((Component)this).GetComponentsInChildren<ParticleSystem>();
			ParticleSystem[] array = componentsInChildren;
			foreach (ParticleSystem val in array)
			{
				LODComponentParticleSystem[] components = ((Component)val).GetComponents<LODComponentParticleSystem>();
				ParticleSystemGroup particleSystemGroup = default(ParticleSystemGroup);
				particleSystemGroup.system = val;
				particleSystemGroup.lodComponents = components;
				ParticleSystemGroup item = particleSystemGroup;
				list.Add(item);
			}
			particleGroups = list.ToArray();
		}
	}
}
