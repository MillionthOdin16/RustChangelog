using UnityEngine;

public static class LODUtil
{
	public const float DefaultDistance = 1000f;

	public static float GetDistance(Transform transform, LODDistanceMode mode = LODDistanceMode.XYZ)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return GetDistance(transform.position, mode);
	}

	public static float GetDistance(Vector3 worldPos, LODDistanceMode mode = LODDistanceMode.XYZ)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		if (MainCamera.isValid)
		{
			switch (mode)
			{
			case LODDistanceMode.XYZ:
				return Vector3.Distance(MainCamera.position, worldPos);
			case LODDistanceMode.XZ:
				return Vector3Ex.Distance2D(MainCamera.position, worldPos);
			case LODDistanceMode.Y:
				return Mathf.Abs(MainCamera.position.y - worldPos.y);
			}
		}
		return 1000f;
	}

	public static float VerifyDistance(float distance)
	{
		return Mathf.Min(500f, distance);
	}

	public static LODEnvironmentMode DetermineEnvironmentMode(Transform transform)
	{
		if (((Component)transform).CompareTag("OnlyVisibleUnderground") || ((Component)transform.root).CompareTag("OnlyVisibleUnderground"))
		{
			return LODEnvironmentMode.Underground;
		}
		return LODEnvironmentMode.Default;
	}
}
