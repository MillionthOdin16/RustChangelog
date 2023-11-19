using UnityEngine;

public class SoundFade : MonoBehaviour, IClientComponent
{
	public enum Direction
	{
		In,
		Out
	}

	public SoundFadeHQAudioFilter hqFadeFilter;

	public float currentGain = 1f;

	public float startingGain = 0f;

	public float finalGain = 1f;

	public int sampleRate = 44100;

	public bool highQualityFadeCompleted = false;

	public float length = 0f;

	public Direction currentDirection;
}
