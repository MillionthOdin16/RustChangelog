using UnityEngine;
using UnityEngine.Profiling;

public struct CachedTransform<T> where T : Component
{
	public T component;

	public Vector3 position;

	public Quaternion rotation;

	public Vector3 localScale;

	public Matrix4x4 localToWorldMatrix => Matrix4x4.TRS(position, rotation, localScale);

	public Matrix4x4 worldToLocalMatrix
	{
		get
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			Matrix4x4 val = localToWorldMatrix;
			return ((Matrix4x4)(ref val)).inverse;
		}
	}

	public Vector3 forward => rotation * Vector3.forward;

	public Vector3 up => rotation * Vector3.up;

	public Vector3 right => rotation * Vector3.right;

	public CachedTransform(T instance)
	{
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		component = instance;
		if (Object.op_Implicit((Object)(object)component))
		{
			position = ((Component)component).transform.position;
			rotation = ((Component)component).transform.rotation;
			localScale = ((Component)component).transform.localScale;
		}
		else
		{
			position = Vector3.zero;
			rotation = Quaternion.identity;
			localScale = Vector3.one;
		}
	}

	public void Apply()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		Profiler.BeginSample("CachedTransform.Apply");
		if (Object.op_Implicit((Object)(object)component))
		{
			((Component)component).transform.SetPositionAndRotation(position, rotation);
			((Component)component).transform.localScale = localScale;
		}
		Profiler.EndSample();
	}

	public void RotateAround(Vector3 center, Vector3 axis, float angle)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		Quaternion val = Quaternion.AngleAxis(angle, axis);
		Vector3 val2 = val * (position - center);
		position = center + val2;
		rotation *= Quaternion.Inverse(rotation) * val * rotation;
	}

	public static implicit operator bool(CachedTransform<T> instance)
	{
		return (Object)(object)instance.component != (Object)null;
	}
}
