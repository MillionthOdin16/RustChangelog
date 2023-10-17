using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class AttackHeliPilotFlare : MonoBehaviour, SeekerTarget.ISeekerTargetOwner
{
	protected void Start()
	{
		SeekerTarget.SetSeekerTarget(this, SeekerTarget.SeekerStrength.HIGH);
	}

	protected void OnDestroy()
	{
		SeekerTarget.SetSeekerTarget(this, SeekerTarget.SeekerStrength.OFF);
	}

	public void Init(Vector3 initialVelocity)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((Component)this).GetComponent<Rigidbody>().velocity = initialVelocity;
	}

	public Vector3 CenterPoint()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return ((Component)this).transform.position;
	}

	public bool IsVisible(Vector3 from, float maxDistance = float.PositiveInfinity)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return GamePhysics.LineOfSight(from, CenterPoint(), 1218519041);
	}

	public bool InSafeZone()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return GamePhysics.CheckSphere<TriggerSafeZone>(CenterPoint(), 0.1f, 262144, (QueryTriggerInteraction)2);
	}

	public bool IsValidHomingTarget()
	{
		return true;
	}

	public void OnEntityMessage(BaseEntity from, string msg)
	{
	}
}
