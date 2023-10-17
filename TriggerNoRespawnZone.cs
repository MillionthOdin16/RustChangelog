using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class TriggerNoRespawnZone : TriggerBase
{
	public static List<TriggerNoRespawnZone> allNRZones = new List<TriggerNoRespawnZone>();

	public float maxDepth = 20f;

	public float maxAltitude = -1f;

	private SphereCollider sphereCollider;

	private float radiusSqr;

	protected void Awake()
	{
		sphereCollider = ((Component)this).GetComponent<SphereCollider>();
		radiusSqr = sphereCollider.radius * sphereCollider.radius;
	}

	protected void OnEnable()
	{
		allNRZones.Add(this);
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		allNRZones.Remove(this);
	}

	internal override GameObject InterestedInObject(GameObject obj)
	{
		obj = base.InterestedInObject(obj);
		if ((Object)(object)obj == (Object)null)
		{
			return null;
		}
		BasePlayer basePlayer = obj.ToBaseEntity() as BasePlayer;
		if ((Object)(object)basePlayer == (Object)null)
		{
			return null;
		}
		if (basePlayer.isClient)
		{
			return null;
		}
		return ((Component)basePlayer).gameObject;
	}

	public static bool InAnyNoRespawnZone(Vector3 theirPos)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < allNRZones.Count; i++)
		{
			if (allNRZones[i].InNoRespawnZone(theirPos, checkRadius: true))
			{
				return true;
			}
		}
		return false;
	}

	public bool InNoRespawnZone(Vector3 theirPos, bool checkRadius)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = ((Component)this).transform.position + sphereCollider.center;
		if (checkRadius && Vector3.SqrMagnitude(val - theirPos) > radiusSqr)
		{
			return false;
		}
		float num = Mathf.Abs(val.y - theirPos.y);
		if (maxDepth != -1f && theirPos.y < val.y && num > maxDepth)
		{
			return false;
		}
		if (maxAltitude != -1f && theirPos.y > val.y && num > maxAltitude)
		{
			return false;
		}
		return true;
	}
}
