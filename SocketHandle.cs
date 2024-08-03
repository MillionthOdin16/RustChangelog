using System;
using UnityEngine;

public class SocketHandle : PrefabAttribute
{
	protected override Type GetIndexedType()
	{
		return typeof(SocketHandle);
	}

	internal void AdjustTarget(ref Construction.Target target, float maxplaceDistance)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = worldPosition;
		Vector3 val2 = ((Ray)(ref target.ray)).origin + ((Ray)(ref target.ray)).direction * maxplaceDistance;
		Vector3 val3 = val2 - val;
		ref Ray ray = ref target.ray;
		Vector3 val4 = val3 - ((Ray)(ref target.ray)).origin;
		((Ray)(ref ray)).direction = ((Vector3)(ref val4)).normalized;
	}
}
