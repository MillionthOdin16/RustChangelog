namespace UnityEngine;

public static class RayEx
{
	public static Vector3 ClosestPoint(this Ray ray, Vector3 pos)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		return ((Ray)(ref ray)).origin + Vector3.Dot(pos - ((Ray)(ref ray)).origin, ((Ray)(ref ray)).direction) * ((Ray)(ref ray)).direction;
	}

	public static float Distance(this Ray ray, Vector3 pos)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = Vector3.Cross(((Ray)(ref ray)).direction, pos - ((Ray)(ref ray)).origin);
		return ((Vector3)(ref val)).magnitude;
	}

	public static float SqrDistance(this Ray ray, Vector3 pos)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = Vector3.Cross(((Ray)(ref ray)).direction, pos - ((Ray)(ref ray)).origin);
		return ((Vector3)(ref val)).sqrMagnitude;
	}

	public static bool IsNaNOrInfinity(this Ray r)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		return Vector3Ex.IsNaNOrInfinity(((Ray)(ref r)).origin) || Vector3Ex.IsNaNOrInfinity(((Ray)(ref r)).direction);
	}
}
