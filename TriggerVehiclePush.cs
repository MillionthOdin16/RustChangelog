using UnityEngine;

public class TriggerVehiclePush : TriggerBase, IServerComponent
{
	public BaseEntity thisEntity;

	public float maxPushVelocity = 10f;

	public float minRadius;

	public float maxRadius;

	public bool snapToAxis;

	public Vector3 axisToSnapTo = Vector3.right;

	public bool allowParentRigidbody;

	public bool useRigidbodyPosition;

	public bool useCentreOfMass;

	public int ContentsCount => entityContents?.Count ?? 0;

	internal override GameObject InterestedInObject(GameObject obj)
	{
		obj = base.InterestedInObject(obj);
		if ((Object)(object)obj == (Object)null)
		{
			return null;
		}
		BaseEntity baseEntity = obj.ToBaseEntity();
		if ((Object)(object)baseEntity == (Object)null)
		{
			return null;
		}
		if (baseEntity is BuildingBlock)
		{
			return null;
		}
		if (baseEntity.isClient)
		{
			return null;
		}
		return ((Component)baseEntity).gameObject;
	}

	public void FixedUpdate()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)thisEntity == (Object)null || entityContents == null)
		{
			return;
		}
		Vector3 position = ((Component)this).transform.position;
		foreach (BaseEntity entityContent in entityContents)
		{
			if (!entityContent.IsValid() || entityContent.EqualNetID((BaseNetworkable)thisEntity))
			{
				continue;
			}
			Rigidbody val = ((Component)entityContent).GetComponent<Rigidbody>();
			if ((Object)(object)val == (Object)null && allowParentRigidbody)
			{
				val = ((Component)entityContent).GetComponentInParent<Rigidbody>();
			}
			if (Object.op_Implicit((Object)(object)val) && !val.isKinematic)
			{
				float num = Vector3Ex.Distance2D(useRigidbodyPosition ? ((Component)val).transform.position : ((Component)entityContent).transform.position, ((Component)this).transform.position);
				float num2 = (((Bounds)(ref entityContent.bounds)).extents.x + ((Bounds)(ref entityContent.bounds)).extents.z) / 2f;
				num -= num2;
				float num3 = 1f - Mathf.InverseLerp(minRadius, maxRadius, num);
				float num4 = 1f - Mathf.InverseLerp(minRadius - 1f, minRadius, num);
				Vector3 val2 = entityContent.ClosestPoint(position);
				Vector3 val3 = Vector3Ex.Direction2D(val2, position);
				val3 = Vector3Ex.Direction2D(useCentreOfMass ? val.worldCenterOfMass : val2, position);
				if (snapToAxis)
				{
					Vector3 val4 = ((Component)this).transform.InverseTransformDirection(val3);
					val3 = ((!(Vector3.Angle(val4, axisToSnapTo) < Vector3.Angle(val4, -axisToSnapTo))) ? (-((Component)this).transform.TransformDirection(axisToSnapTo)) : ((Component)this).transform.TransformDirection(axisToSnapTo));
				}
				val.AddForceAtPosition(val3 * maxPushVelocity * num3, val2, (ForceMode)5);
				if (num4 > 0f)
				{
					val.AddForceAtPosition(val3 * 1f * num4, val2, (ForceMode)2);
				}
			}
		}
	}

	public void OnDrawGizmos()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(((Component)this).transform.position, minRadius);
		Gizmos.color = new Color(0.5f, 0f, 0f, 1f);
		Gizmos.DrawWireSphere(((Component)this).transform.position, maxRadius);
		if (snapToAxis)
		{
			Gizmos.color = Color.cyan;
			Vector3 val = ((Component)this).transform.TransformDirection(axisToSnapTo);
			Gizmos.DrawLine(((Component)this).transform.position + val, ((Component)this).transform.position - val);
		}
	}
}
