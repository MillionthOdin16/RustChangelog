using System.Collections.Generic;
using Facepunch;
using UnityEngine;
using UnityEngine.Profiling;

public class MusicClipLoader
{
	public class LoadedAudioClip
	{
		public AudioClip clip;

		public float unloadTime;
	}

	public List<LoadedAudioClip> loadedClips = new List<LoadedAudioClip>();

	public Dictionary<AudioClip, LoadedAudioClip> loadedClipDict = new Dictionary<AudioClip, LoadedAudioClip>();

	public List<AudioClip> clipsToLoad = new List<AudioClip>();

	public List<AudioClip> clipsToUnload = new List<AudioClip>();

	public void Update()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Invalid comparison between Unknown and I4
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Invalid comparison between Unknown and I4
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Invalid comparison between Unknown and I4
		Profiler.BeginSample("MusicClipLoader.Update");
		for (int num = clipsToLoad.Count - 1; num >= 0; num--)
		{
			AudioClip val = clipsToLoad[num];
			if ((int)val.loadState != 2 && (int)val.loadState != 1)
			{
				Profiler.BeginSample("AudioClip.LoadAudioData");
				val.LoadAudioData();
				Profiler.EndSample();
				clipsToLoad.RemoveAt(num);
				Profiler.EndSample();
				return;
			}
		}
		for (int num2 = clipsToUnload.Count - 1; num2 >= 0; num2--)
		{
			AudioClip val2 = clipsToUnload[num2];
			if ((int)val2.loadState == 2)
			{
				Profiler.BeginSample("AudioClip.UnloadAudioData");
				val2.UnloadAudioData();
				Profiler.EndSample();
				clipsToUnload.RemoveAt(num2);
				Profiler.EndSample();
				return;
			}
		}
		Profiler.EndSample();
	}

	public void Refresh()
	{
		Profiler.BeginSample("MusicClipLoader.Refresh");
		for (int i = 0; i < SingletonComponent<MusicManager>.Instance.activeMusicClips.Count; i++)
		{
			MusicTheme.PositionedClip positionedClip = SingletonComponent<MusicManager>.Instance.activeMusicClips[i];
			LoadedAudioClip loadedAudioClip = FindLoadedClip(positionedClip.musicClip.audioClip);
			if (loadedAudioClip == null)
			{
				loadedAudioClip = Pool.Get<LoadedAudioClip>();
				loadedAudioClip.clip = positionedClip.musicClip.audioClip;
				loadedAudioClip.unloadTime = (float)AudioSettings.dspTime + loadedAudioClip.clip.length + 1f;
				loadedClips.Add(loadedAudioClip);
				loadedClipDict.Add(loadedAudioClip.clip, loadedAudioClip);
				clipsToLoad.Add(loadedAudioClip.clip);
			}
			else
			{
				loadedAudioClip.unloadTime = (float)AudioSettings.dspTime + loadedAudioClip.clip.length + 1f;
				clipsToUnload.Remove(loadedAudioClip.clip);
			}
		}
		for (int num = loadedClips.Count - 1; num >= 0; num--)
		{
			LoadedAudioClip loadedAudioClip2 = loadedClips[num];
			if (AudioSettings.dspTime > (double)loadedAudioClip2.unloadTime)
			{
				clipsToUnload.Add(loadedAudioClip2.clip);
				loadedClips.Remove(loadedAudioClip2);
				loadedClipDict.Remove(loadedAudioClip2.clip);
				Pool.Free<LoadedAudioClip>(ref loadedAudioClip2);
			}
		}
		Profiler.EndSample();
	}

	private LoadedAudioClip FindLoadedClip(AudioClip clip)
	{
		if (loadedClipDict.ContainsKey(clip))
		{
			return loadedClipDict[clip];
		}
		return null;
	}
}
