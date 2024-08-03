using UnityEngine;

public class SoundPlayer : BaseMonoBehaviour, IClientComponent
{
	public SoundDefinition soundDefinition;

	public bool playImmediately = true;

	public float minStartDelay = 0f;

	public float maxStartDelay = 0f;

	public bool debugRepeat = false;

	public bool pending = false;

	public Vector3 soundOffset = Vector3.zero;
}
