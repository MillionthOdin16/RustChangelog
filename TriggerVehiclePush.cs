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
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
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
