using UnityEngine;

public class MLRSAudio : MonoBehaviour
{
	[SerializeField]
	private MLRS mlrs;

	[SerializeField]
	private Transform pitchTransform;

	[SerializeField]
	private Transform yawTransform;

	[SerializeField]
	private float pitchDeltaSmoothRate = 5f;

	[SerializeField]
	private float yawDeltaSmoothRate = 5f;

	[SerializeField]
	private float pitchDeltaThreshold = 0.5f;

	[SerializeField]
	private float yawDeltaThreshold = 0.5f;

	private float lastPitch = 0f;

	private float lastYaw = 0f;

	private float pitchDelta = 0f;

	private float yawDelta = 0f;

	public SoundDefinition turretMovementStartDef;

	public SoundDefinition turretMovementLoopDef;

	public SoundDefinition turretMovementStopDef;

	private Sound turretMovementLoop;
}
