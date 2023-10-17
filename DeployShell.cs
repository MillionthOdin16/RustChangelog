using System;
using UnityEngine;

public class DeployShell : PrefabAttribute
{
	public Bounds bounds = new Bounds(Vector3.zero, Vector3.one);

	public OBB WorldSpaceBounds(Transform transform)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		return new OBB(transform.position, transform.lossyScale, transform.rotation, bounds);
	}

	public float LineOfSightPadding()
	{
		return 0.025f;
	}

	protected override Type GetIndexedType()
	{
		return typeof(DeployShell);
	}
}
