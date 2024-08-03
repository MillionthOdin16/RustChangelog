using ConVar;
using UnityEngine;

public static class UISound
{
	private static AudioSource source;

	private static AudioSource GetAudioSource()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Expected O, but got Unknown
		if ((Object)(object)source != (Object)null)
		{
			return source;
		}
		GameObject val = new GameObject("UISound");
		source = val.AddComponent<AudioSource>();
		source.spatialBlend = 0f;
		source.volume = 1f;
		return source;
	}

	public static void Play(AudioClip clip, float volume = 1f)
	{
		if (!((Object)(object)clip == (Object)null))
		{
			GetAudioSource().volume = volume * Audio.master * 0.4f;
			GetAudioSource().PlayOneShot(clip);
		}
	}
}
