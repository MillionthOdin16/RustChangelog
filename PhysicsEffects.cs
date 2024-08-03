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

	public bool useCollisionPositionInsteadOfTransform = false;

	public float minimumRigidbodyImpactWeight = 0f;

	private float lastEffectPlayed = 0f;

	private float enabledAt = float.PositiveInfinity;

	private float ignoreImpactThreshold = 0.02f;

	private Vector3 lastCollisionPos;

	public void OnEnable()
	{
		enabledAt = Time.time;
	}

	public void OnCollisionEnter(Collision collision)
	{
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		if (!Physics.sendeffects || Time.time < enabledAt + enableDelay || Time.time < lastEffectPlayed + minTimeBetweenEffects || ((1 << collision.gameObject.layer) & LayerMask.op_Implicit(ignoreLayers)) != 0)
		{
			return;
		}
		Vector3 relativeVelocity = collision.relativeVelocity;
		float magnitude = ((Vector3)(ref relativeVelocity)).magnitude;
		magnitude = magnitude * 0.055f * hardnessScale;
		if (magnitude <= ignoreImpactThreshold)
		{
			return;
		}
		float num = (useCollisionPositionInsteadOfTransform ? Vector3.Distance(((ContactPoint)(ref collision.contacts[0])).point, lastCollisionPos) : Vector3.Distance(((Component)this).transform.position, lastCollisionPos));
		Rigidbody val = default(Rigidbody);
		if ((!(num < minDistBetweenEffects) || lastEffectPlayed == 0f) && (!(minimumRigidbodyImpactWeight > 0f) || !collision.gameObject.TryGetComponent<Rigidbody>(ref val) || !(val.mass < minimumRigidbodyImpactWeight)))
		{
			if ((Object)(object)entity != (Object)null)
			{
				entity.SignalBroadcast(BaseEntity.Signal.PhysImpact, magnitude.ToString());
			}
			lastEffectPlayed = Time.time;
			if (useCollisionPositionInsteadOfTransform)
			{
				lastCollisionPos = ((Component)this).transform.InverseTransformPoint(((ContactPoint)(ref collision.contacts[0])).point);
			}
			else
			{
				lastCollisionPos = ((Component)this).transform.position;
			}
		}
	}
}
