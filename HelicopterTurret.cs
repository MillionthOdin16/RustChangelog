using System.Collections.Generic;
using ConVar;
using UnityEngine;

public class HelicopterTurret : MonoBehaviour
{
	public PatrolHelicopterAI _heliAI;

	public float fireRate = 0.125f;

	public float burstLength = 3f;

	public float timeBetweenBursts = 3f;

	public float maxTargetRange = 300f;

	public float loseTargetAfter = 5f;

	public Transform gun_yaw;

	public Transform gun_pitch;

	public Transform muzzleTransform;

	public bool left;

	public BaseCombatEntity _target;

	private float lastBurstTime = float.NegativeInfinity;

	private float lastFireTime = float.NegativeInfinity;

	private float lastSeenTargetTime = float.NegativeInfinity;

	private bool targetVisible = false;

	public void SetTarget(BaseCombatEntity newTarget)
	{
		_target = newTarget;
		UpdateTargetVisibility();
	}

	public bool NeedsNewTarget()
	{
		return !HasTarget() || (!targetVisible && TimeSinceTargetLastSeen() > loseTargetAfter);
	}

	public bool UpdateTargetFromList(List<PatrolHelicopterAI.targetinfo> newTargetList)
	{
		int num = Random.Range(0, newTargetList.Count);
		int num2 = newTargetList.Count;
		while (num2 >= 0)
		{
			num2--;
			PatrolHelicopterAI.targetinfo targetinfo = newTargetList[num];
			if (targetinfo != null && (Object)(object)targetinfo.ent != (Object)null && targetinfo.IsVisible() && InFiringArc(targetinfo.ply))
			{
				SetTarget(targetinfo.ply);
				return true;
			}
			num++;
			if (num >= newTargetList.Count)
			{
				num = 0;
			}
		}
		return false;
	}

	public bool TargetVisible()
	{
		UpdateTargetVisibility();
		return targetVisible;
	}

	public float TimeSinceTargetLastSeen()
	{
		return Time.realtimeSinceStartup - lastSeenTargetTime;
	}

	public bool HasTarget()
	{
		return (Object)(object)_target != (Object)null;
	}

	public void ClearTarget()
	{
		_target = null;
		targetVisible = false;
	}

	public void TurretThink()
	{
		if (HasTarget() && TimeSinceTargetLastSeen() > loseTargetAfter * 2f)
		{
			ClearTarget();
		}
		if (HasTarget())
		{
			if (Time.time - lastBurstTime > burstLength + timeBetweenBursts && TargetVisible())
			{
				lastBurstTime = Time.time;
			}
			if (Time.time < lastBurstTime + burstLength && Time.time - lastFireTime >= fireRate && InFiringArc(_target))
			{
				lastFireTime = Time.time;
				FireGun();
			}
		}
	}

	public void FireGun()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		_heliAI.FireGun(((Component)_target).transform.position + new Vector3(0f, 0.25f, 0f), PatrolHelicopter.bulletAccuracy, left);
	}

	public Vector3 GetPositionForEntity(BaseCombatEntity potentialtarget)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		return ((Component)potentialtarget).transform.position;
	}

	public float AngleToTarget(BaseCombatEntity potentialtarget)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		Vector3 positionForEntity = GetPositionForEntity(potentialtarget);
		Vector3 position = muzzleTransform.position;
		Vector3 val = positionForEntity - position;
		Vector3 normalized = ((Vector3)(ref val)).normalized;
		return Vector3.Angle(left ? (-((Component)_heliAI).transform.right) : ((Component)_heliAI).transform.right, normalized);
	}

	public bool InFiringArc(BaseCombatEntity potentialtarget)
	{
		return AngleToTarget(potentialtarget) < 80f;
	}

	public void UpdateTargetVisibility()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		if (HasTarget())
		{
			Vector3 position = ((Component)_target).transform.position;
			BasePlayer basePlayer = _target as BasePlayer;
			if (Object.op_Implicit((Object)(object)basePlayer))
			{
				position = basePlayer.eyes.position;
			}
			bool flag = false;
			float num = Vector3.Distance(position, muzzleTransform.position);
			Vector3 val = position - muzzleTransform.position;
			Vector3 normalized = ((Vector3)(ref val)).normalized;
			if (num < maxTargetRange && InFiringArc(_target) && GamePhysics.Trace(new Ray(muzzleTransform.position + normalized * 6f, normalized), 0f, out var hitInfo, num * 1.1f, 1218652417, (QueryTriggerInteraction)0) && (Object)(object)((Component)((RaycastHit)(ref hitInfo)).collider).gameObject.ToBaseEntity() == (Object)(object)_target)
			{
				flag = true;
			}
			if (flag)
			{
				lastSeenTargetTime = Time.realtimeSinceStartup;
			}
			targetVisible = flag;
		}
	}
}
