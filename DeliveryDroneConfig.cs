using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Rust/Delivery Drone Config")]
public class DeliveryDroneConfig : BaseScriptableObject
{
	public Vector3 vendingMachineOffset = new Vector3(0f, 1f, 1f);

	public float maxDistanceFromVendingMachine = 1f;

	public Vector3 halfExtents = new Vector3(0.5f, 0.5f, 0.5f);

	public float testHeight = 200f;

	public LayerMask layerMask = LayerMask.op_Implicit(161546496);

	public void FindDescentPoints(VendingMachine vendingMachine, float currentY, out Vector3 waitPosition, out Vector3 descendPosition)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		float num = maxDistanceFromVendingMachine / 4f;
		RaycastHit val4 = default(RaycastHit);
		for (int i = 0; i <= 4; i++)
		{
			Vector3 val = Vector3.forward * (num * (float)i);
			Vector3 val2 = ((Component)vendingMachine).transform.TransformPoint(vendingMachineOffset + val);
			Vector3 val3 = val2 + Vector3.up * testHeight;
			if (!Physics.BoxCast(val3, halfExtents, Vector3.down, ref val4, ((Component)vendingMachine).transform.rotation, testHeight, LayerMask.op_Implicit(layerMask)))
			{
				waitPosition = val2;
				descendPosition = Vector3Ex.WithY(val3, currentY);
				return;
			}
			if (i == 4)
			{
				waitPosition = val3 + Vector3.down * (((RaycastHit)(ref val4)).distance - halfExtents.y * 2f);
				descendPosition = Vector3Ex.WithY(val3, currentY);
				return;
			}
		}
		throw new Exception("Bug: FindDescentPoint didn't return a fallback value");
	}

	public bool IsVendingMachineAccessible(VendingMachine vendingMachine, Vector3 offset, out RaycastHit hitInfo)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = ((Component)vendingMachine).transform.TransformPoint(offset);
		Vector3 val2 = val + Vector3.up * testHeight;
		if (Physics.BoxCast(val2, halfExtents, Vector3.down, ref hitInfo, ((Component)vendingMachine).transform.rotation, testHeight, LayerMask.op_Implicit(layerMask)))
		{
			return false;
		}
		return vendingMachine.IsVisibleAndCanSee(val, 2f);
	}
}
