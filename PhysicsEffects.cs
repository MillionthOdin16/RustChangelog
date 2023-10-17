using ConVar;
using UnityEngine;

public class PhysicsEffects : MonoBehaviour
{
	public BaseEntity entity;

	public SoundDefinition physImpactSoundDef;

	public float minTimeBetweenEffects = 0.25f;

	public float minDistBetweenEffects = 0.1f;

	public float hardnessScale = 1f;

	public float lowMedThreshold = 0.4f;

	public float medHardThreshold = 0.7f;

	public float enableDelay = 0.1f;

	public LayerMask ignoreLayers;

	private float lastEffectPlayed;

	private float enabledAt = float.PositiveInfinity;

	private float ignoreImpactThreshold = 0.02f;

	private Vector3 lastCollisionPos;

	public void OnEnable()
	{
		enabledAt = Time.time;
	}

	public void OnCollisionEnter(Collision collision)
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		if (!Physics.sendeffects || Time.time < enabledAt + enableDelay || Time.time < lastEffectPlayed + minTimeBetweenEffects || ((1 << collision.gameObject.layer) & LayerMask.op_Implicit(ignoreLayers)) != 0)
		{
			return;
		}
		Vector3 relativeVelocity = collision.relativeVelocity;
		float magnitude = ((Vector3)(ref relativeVelocity)).magnitude;
		magnitude = magnitude * 0.055f * hardnessScale;
		if (!(magnitude <= ignoreImpactThreshold) && (!(Vector3.Distance(((Component)this).transform.position, lastCollisionPos) < minDistBetweenEffects) || lastEffectPlayed == 0f))
		{
			if ((Object)(object)entity != (Object)null)
			{
				entity.SignalBroadcast(BaseEntity.Signal.PhysImpact, magnitude.ToString());
			}
			lastEffectPlayed = Time.time;
			lastCollisionPos = ((Component)this).transform.position;
		}
	}
}
