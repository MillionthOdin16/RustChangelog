using UnityEngine;

namespace VacuumBreather;

public static class QuaternionExtensions
{
	public static Quaternion Multiply(this Quaternion quaternion, float scalar)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		return new Quaternion((float)((double)quaternion.x * (double)scalar), (float)((double)quaternion.y * (double)scalar), (float)((double)quaternion.z * (double)scalar), (float)((double)quaternion.w * (double)scalar));
	}

	public static Quaternion RequiredRotation(Quaternion from, Quaternion to)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		return new Quaternion((float)((double)lhs.x - (double)rhs.x), (float)((double)lhs.y - (double)rhs.y), (float)((double)lhs.z - (double)rhs.z), (float)((double)lhs.w - (double)rhs.w));
	}
}
