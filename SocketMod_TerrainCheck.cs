using UnityEngine;

public class SocketMod_TerrainCheck : SocketMod
{
	public bool wantsInTerrain = true;

	private void OnDrawGizmos()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		if (TerrainMeta.OutOfBounds(vPoint))
		{
			return false;
		}
		if (!Object.op_Implicit((Object)(object)TerrainMeta.Collision) || !TerrainMeta.Collision.GetIgnore(vPoint))
		{
			Terrain[] activeTerrains = Terrain.activeTerrains;
			foreach (Terrain val in activeTerrains)
			{
				float num = val.SampleHeight(vPoint) + ((Component)val).transform.position.y;
				if (num > vPoint.y)
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
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		Vector3 vPoint = place.position + place.rotation * worldPosition;
		if (IsInTerrain(vPoint) == wantsInTerrain)
		{
			return true;
		}
		Construction.lastPlacementError = fullName + ": not in terrain";
		return false;
	}
}
