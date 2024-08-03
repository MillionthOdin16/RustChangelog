using System.Collections.Generic;
using Facepunch;
using UnityEngine;

public class SocketMod_Attraction : SocketMod
{
	public float outerRadius = 1f;

	public float innerRadius = 0.1f;

	public string groupName = "wallbottom";

	public bool lockRotation = false;

	public bool ignoreRotationForRadiusCheck = false;

	private void OnDrawGizmosSelected()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		Gizmos.matrix = ((Component)this).transform.localToWorldMatrix;
		Gizmos.color = new Color(1f, 1f, 0f, 0.3f);
		Gizmos.DrawSphere(Vector3.zero, outerRadius);
		Gizmos.color = new Color(0f, 1f, 0f, 0.6f);
		Gizmos.DrawSphere(Vector3.zero, innerRadius);
	}

	public override bool DoCheck(Construction.Placement place)
	{
		return true;
	}

	public override void ModifyPlacement(Construction.Placement place)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = place.position + place.rotation * worldPosition;
		List<BaseEntity> list = Pool.GetList<BaseEntity>();
		Vis.Entities(val, outerRadius * 2f, list, -1, (QueryTriggerInteraction)2);
		Vector3 position = Vector3.zero;
		float num = float.MaxValue;
		Vector3 position2 = place.position;
		Quaternion rotation = Quaternion.identity;
		foreach (BaseEntity item in list)
		{
			if (item.isServer != isServer)
			{
				continue;
			}
			AttractionPoint[] array = prefabAttribute.FindAll<AttractionPoint>(item.prefabID);
			if (array == null)
			{
				continue;
			}
			AttractionPoint[] array2 = array;
			foreach (AttractionPoint attractionPoint in array2)
			{
				if (attractionPoint.groupName != groupName)
				{
					continue;
				}
				Vector3 val2 = ((Component)item).transform.position + ((Component)item).transform.rotation * attractionPoint.worldPosition;
				Vector3 val3 = val2 - val;
				float magnitude = ((Vector3)(ref val3)).magnitude;
				if (ignoreRotationForRadiusCheck)
				{
					Vector3 val4 = ((Component)item).transform.TransformPoint(Vector3.LerpUnclamped(Vector3.zero, Vector3Ex.WithY(attractionPoint.worldPosition, 0f), 2f));
					float num2 = Vector3.Distance(val4, position2);
					if (num2 < num)
					{
						num = num2;
						position = val4;
						rotation = ((Component)item).transform.rotation;
					}
				}
				if (!(magnitude > outerRadius))
				{
					Quaternion val5 = QuaternionEx.LookRotationWithOffset(worldPosition, val2 - place.position, Vector3.up);
					float num3 = Mathf.InverseLerp(outerRadius, innerRadius, magnitude);
					if (lockRotation)
					{
						num3 = 1f;
					}
					if (lockRotation)
					{
						Vector3 eulerAngles = ((Quaternion)(ref place.rotation)).eulerAngles;
						eulerAngles -= new Vector3(eulerAngles.x % 90f, eulerAngles.y % 90f, eulerAngles.z % 90f);
						place.rotation = Quaternion.Euler(eulerAngles + ((Component)item).transform.eulerAngles);
					}
					else
					{
						place.rotation = Quaternion.Lerp(place.rotation, val5, num3);
					}
					val = place.position + place.rotation * worldPosition;
					val3 = val2 - val;
					place.position += val3 * num3;
				}
			}
		}
		if (num < float.MaxValue && ignoreRotationForRadiusCheck)
		{
			place.position = position;
			place.rotation = rotation;
		}
		Pool.FreeList<BaseEntity>(ref list);
	}
}
