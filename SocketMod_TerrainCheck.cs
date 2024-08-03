using System.Collections.Generic;
using Facepunch;
using UnityEngine;

public class SocketMod_TerrainCheck : SocketMod
{
	public bool wantsInTerrain = true;

	public bool preventWorldLayerInMonuments;

	private void OnDrawGizmos()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		Gizmos.matrix = ((Component)this).transform.localToWorldMatrix;
		bool flag = IsInTerrain(((Component)this).transform.position, !preventWorldLayerInMonuments);
		if (!wantsInTerrain)
		{
			flag = !flag;
		}
		Gizmos.color = (flag ? Color.green : Color.red);
		Gizmos.DrawSphere(Vector3.zero, 0.1f);
	}

	public static bool IsInTerrain(Vector3 vPoint, bool worldLayerInMonuments)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		if (TerrainMeta.OutOfBounds(vPoint))
		{
			if (TerrainMeta.IsPointWithinTutorialBounds(vPoint))
			{
				return Physics.Raycast(new Ray(vPoint + Vector3.up * 3f, Vector3.down), 3f, 65536);
			}
			return false;
		}
		List<RaycastHit> list = Pool.GetList<RaycastHit>();
		GamePhysics.TraceAllUnordered(new Ray(vPoint + Vector3.up * 3f, Vector3.down), 0f, list, 3f, 65536, (QueryTriggerInteraction)0);
		using (List<RaycastHit>.Enumerator enumerator = list.GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				RaycastHit current = enumerator.Current;
				if (worldLayerInMonuments)
				{
					Pool.FreeList<RaycastHit>(ref list);
					return true;
				}
				if (((Component)((RaycastHit)(ref current)).collider).gameObject.HasCustomTag(GameObjectTag.BlockBarricadePlacement))
				{
					Construction.placementError = ConstructionErrors.CantPlaceOnMonument;
					Pool.FreeList<RaycastHit>(ref list);
					return false;
				}
				if (((Component)((RaycastHit)(ref current)).collider).gameObject.HasCustomTag(GameObjectTag.AllowBarricadePlacement))
				{
					Pool.FreeList<RaycastHit>(ref list);
					return true;
				}
				MonumentInfo monument = ((RaycastHit)(ref current)).collider.GetMonument();
				Pool.FreeList<RaycastHit>(ref list);
				if ((Object)(object)monument == (Object)null)
				{
					return true;
				}
				Construction.placementError = ConstructionErrors.CantPlaceOnMonument;
				return false;
			}
		}
		Pool.FreeList<RaycastHit>(ref list);
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
		return false;
	}

	public override bool DoCheck(Construction.Placement place)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		Vector3 vPoint = place.position + place.rotation * worldPosition;
		Construction.lastPlacementError = null;
		if (IsInTerrain(vPoint, !preventWorldLayerInMonuments) == wantsInTerrain)
		{
			return true;
		}
		if (Construction.lastPlacementError == null)
		{
			Construction.placementError = ConstructionErrors.NotInTerrain;
		}
		return false;
	}
}
