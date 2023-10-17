using UnityEngine;

public class ValidBounds : SingletonComponent<ValidBounds>
{
	public Bounds worldBounds;

	public static bool Test(Vector3 vPos)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		if (!Object.op_Implicit((Object)(object)SingletonComponent<ValidBounds>.Instance))
		{
			return true;
		}
		return SingletonComponent<ValidBounds>.Instance.IsInside(vPos);
	}

	public static float TestDist(Vector3 vPos)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (!Object.op_Implicit((Object)(object)SingletonComponent<ValidBounds>.Instance))
		{
			return float.MaxValue;
		}
		return SingletonComponent<ValidBounds>.Instance.DistToWorldEdge2D(vPos);
	}

	private void OnDrawGizmosSelected()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		Gizmos.color = Color.blue;
		Gizmos.DrawWireCube(((Bounds)(ref worldBounds)).center, ((Bounds)(ref worldBounds)).size);
	}

	internal bool IsInside(Vector3 vPos)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		if (Vector3Ex.IsNaNOrInfinity(vPos))
		{
			return false;
		}
		if (!((Bounds)(ref worldBounds)).Contains(vPos))
		{
			return false;
		}
		if ((Object)(object)TerrainMeta.Terrain != (Object)null)
		{
			if (World.Procedural && vPos.y < TerrainMeta.Position.y)
			{
				return false;
			}
			if (TerrainMeta.OutOfMargin(vPos))
			{
				return false;
			}
		}
		return true;
	}

	internal float DistToWorldEdge2D(Vector3 vPos)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		if (!IsInside(vPos))
		{
			return -1f;
		}
		float num = worldBounds.InnerDistToEdge2D(vPos);
		if ((Object)(object)TerrainMeta.Terrain != (Object)null)
		{
			float num2 = TerrainMeta.InnerDistToEdge2D(vPos);
			return Mathf.Min(num, num2);
		}
		return num;
	}
}
