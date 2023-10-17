using UnityEngine;

public class VoiceProcessor : EntityComponentBase
{
	public AudioSource mouthSpeaker;

	public PlayerVoiceSpeaker playerSpeaker = null;

	public float volumeMultiplier = 1f;
}
