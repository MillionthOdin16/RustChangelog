using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(menuName = "Rust/Sound Class")]
public class SoundClass : ScriptableObject
{
	[Header("Mixer Settings")]
	public AudioMixerGroup output = null;

	public AudioMixerGroup firstPersonOutput = null;

	[Header("Occlusion Settings")]
	public bool enableOcclusion = false;

	public bool playIfOccluded = true;

	public float occlusionGain = 1f;

	[Tooltip("Use this mixer group when the sound is occluded to save DSP CPU usage. Only works for non-looping sounds.")]
	public AudioMixerGroup occludedOutput = null;

	[Header("Voice Limiting")]
	public int globalVoiceMaxCount = 100;

	public int priority = 128;

	public List<SoundDefinition> definitions = new List<SoundDefinition>();
}
