using UnityEngine;

public struct Half3
{
	public ushort x;

	public ushort y;

	public ushort z;

	public Half3(Vector3 vec)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		x = Mathf.FloatToHalf(vec.x);
		y = Mathf.FloatToHalf(vec.y);
		z = Mathf.FloatToHalf(vec.z);
	}

	public static explicit operator Vector3(Half3 vec)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		return new Vector3(Mathf.HalfToFloat(vec.x), Mathf.HalfToFloat(vec.y), Mathf.HalfToFloat(vec.z));
	}
}
