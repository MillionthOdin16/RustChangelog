using UnityEngine;

public class SocketMod_TerrainCheck : SocketMod
{
	public bool wantsInTerrain = true;

	private void OnDrawGizmos()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		Gizmos.matrix = ((Component)this).transform.localToWorldMatrix;
		bool flag = IsInTerrain(((Component)this).transform.position);
		if (!wantsInTerrain)
		{
			flag = !flag;
		}
		Gizmos.color = (flag ? Color.green : Color.red);
		Gizmos.DrawSphere(Vector3.zero, 0.1f);
	}

	public static bool IsInTerrain(Vector3 vPoint)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		if (TerrainMeta.OutOfBounds(vPoint))
		{
			return false;
		}
		if (!Object.op_Implicit((Object)(object)TerrainMeta.Collision) || !TerrainMeta.Collision.GetIgnore(vPoint))
		{
			Terrain[] activeTerrains = Terrain.activeTerrains;
			foreach (Terrain val in activeTerrains)
			{
				if (val.SampleHeight(vPoint) + ((Component)val).transform.position.y > vPoint.y)
				{
					return true;
				}
			}
		}
		if (Physics.Raycast(new Ray(vPoint + Vector3.up * 3f, Vector3.down), 3f, 65536))
		{
			return true;
		}
		return false;
	}

	public override bool DoCheck(Construction.Placement place)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (IsInTerrain(place.position + place.rotation * worldPosition) == wantsInTerrain)
		{
			return true;
		}
		Construction.lastPlacementError = fullName + ": not in terrain";
		return false;
	}
}
