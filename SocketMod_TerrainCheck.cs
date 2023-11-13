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
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		if (TerrainMeta.OutOfBounds(vPoint))
		{
			return Physics.Raycast(new Ray(vPoint + Vector3.up * 3f, Vector3.down), 3f, 65536);
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
