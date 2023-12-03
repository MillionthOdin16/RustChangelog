using System;

namespace UnityEngine.Rendering.PostProcessing;

[Serializable]
public sealed class Vector3Parameter : ParameterOverride<Vector3>
{
	public override void Interp(Vector3 from, Vector3 to, float t)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		value.x = from.x + (to.x - from.x) * t;
		value.y = from.y + (to.y - from.y) * t;
		value.z = from.z + (to.z - from.z) * t;
	}

	public static implicit operator Vector2(Vector3Parameter prop)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		return Vector2.op_Implicit(prop.value);
	}

	public static implicit operator Vector4(Vector3Parameter prop)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		return Vector4.op_Implicit(prop.value);
	}
}
