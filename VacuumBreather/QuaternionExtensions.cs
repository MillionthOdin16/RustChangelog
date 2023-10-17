using UnityEngine;

namespace VacuumBreather;

public static class QuaternionExtensions
{
	public static Quaternion Multiply(this Quaternion quaternion, float scalar)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		return new Quaternion((float)((double)quaternion.x * (double)scalar), (float)((double)quaternion.y * (double)scalar), (float)((double)quaternion.z * (double)scalar), (float)((double)quaternion.w * (double)scalar));
	}

	public static Quaternion RequiredRotation(Quaternion from, Quaternion to)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		Quaternion val = to * Quaternion.Inverse(from);
		if (val.w < 0f)
		{
			val.x *= -1f;
			val.y *= -1f;
			val.z *= -1f;
			val.w *= -1f;
		}
		return val;
	}

	public static Quaternion Subtract(this Quaternion lhs, Quaternion rhs)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		return new Quaternion((float)((double)lhs.x - (double)rhs.x), (float)((double)lhs.y - (double)rhs.y), (float)((double)lhs.z - (double)rhs.z), (float)((double)lhs.w - (double)rhs.w));
	}
}
