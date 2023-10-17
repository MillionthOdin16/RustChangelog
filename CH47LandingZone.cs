using System.Collections.Generic;
using UnityEngine;

public class CH47LandingZone : MonoBehaviour
{
	public float lastDropTime;

	private static List<CH47LandingZone> landingZones = new List<CH47LandingZone>();

	public float dropoffScale = 1f;

	public void Awake()
	{
		if (!landingZones.Contains(this))
		{
			landingZones.Add(this);
		}
	}

	public static CH47LandingZone GetClosest(Vector3 pos)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		float num = float.PositiveInfinity;
		CH47LandingZone result = null;
		foreach (CH47LandingZone landingZone in landingZones)
		{
			float num2 = Vector3Ex.Distance2D(pos, ((Component)landingZone).transform.position);
			if (num2 < num)
			{
				num = num2;
				result = landingZone;
			}
		}
		return result;
	}

	public void OnDestroy()
	{
		if (landingZones.Contains(this))
		{
			landingZones.Remove(this);
		}
	}

	public float TimeSinceLastDrop()
	{
		return Time.time - lastDropTime;
	}

	public void Used()
	{
		lastDropTime = Time.time;
	}

	public void OnDrawGizmos()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		Color magenta = Color.magenta;
		magenta.a = 0.25f;
		Gizmos.color = magenta;
		GizmosUtil.DrawCircleY(((Component)this).transform.position, 6f);
		magenta.a = 1f;
		Gizmos.color = magenta;
		GizmosUtil.DrawWireCircleY(((Component)this).transform.position, 6f);
	}
}
