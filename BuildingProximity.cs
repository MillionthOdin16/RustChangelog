using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

public class BuildingProximity : PrefabAttribute
{
	private struct ProximityInfo
	{
		public bool hit;

		public bool connection;

		public Line line;

		public float sqrDist;
	}

	private const float check_radius = 2f;

	private const float check_forgiveness = 0.01f;

	private const float foundation_width = 3f;

	private const float foundation_extents = 1.5f;

	public static bool Check(BasePlayer player, Construction construction, Vector3 position, Quaternion rotation)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		OBB val = default(OBB);
		((OBB)(ref val))._002Ector(position, rotation, construction.bounds);
		float radius = ((Vector3)(ref val.extents)).magnitude + 2f;
		List<BuildingBlock> list = Pool.GetList<BuildingBlock>();
		Vis.Entities(val.position, radius, list, 2097152, (QueryTriggerInteraction)2);
		uint num = 0u;
		for (int i = 0; i < list.Count; i++)
		{
			BuildingBlock buildingBlock = list[i];
			Construction blockDefinition = buildingBlock.blockDefinition;
			Vector3 position2 = ((Component)buildingBlock).transform.position;
			Quaternion rotation2 = ((Component)buildingBlock).transform.rotation;
			ProximityInfo proximity = GetProximity(construction, position, rotation, blockDefinition, position2, rotation2);
			ProximityInfo proximity2 = GetProximity(blockDefinition, position2, rotation2, construction, position, rotation);
			ProximityInfo proximityInfo = default(ProximityInfo);
			proximityInfo.hit = proximity.hit || proximity2.hit;
			proximityInfo.connection = proximity.connection || proximity2.connection;
			if (proximity.sqrDist <= proximity2.sqrDist)
			{
				proximityInfo.line = proximity.line;
				proximityInfo.sqrDist = proximity.sqrDist;
			}
			else
			{
				proximityInfo.line = proximity2.line;
				proximityInfo.sqrDist = proximity2.sqrDist;
			}
			if (proximityInfo.connection)
			{
				BuildingManager.Building building = buildingBlock.GetBuilding();
				if (building != null)
				{
					BuildingPrivlidge dominatingBuildingPrivilege = building.GetDominatingBuildingPrivilege();
					if ((Object)(object)dominatingBuildingPrivilege != (Object)null)
					{
						if (!construction.canBypassBuildingPermission && !dominatingBuildingPrivilege.IsAuthed(player))
						{
							Construction.lastPlacementError = "Cannot attach to unauthorized building";
							Pool.FreeList<BuildingBlock>(ref list);
							return true;
						}
						if (num == 0)
						{
							num = building.ID;
						}
						else if (num != building.ID)
						{
							if (!dominatingBuildingPrivilege.IsAuthed(player))
							{
								Construction.lastPlacementError = "Cannot attach to unauthorized building";
							}
							else
							{
								Construction.lastPlacementError = "Cannot connect two buildings with cupboards";
							}
							Pool.FreeList<BuildingBlock>(ref list);
							return true;
						}
					}
				}
			}
			if (proximityInfo.hit)
			{
				Vector3 val2 = proximityInfo.line.point1 - proximityInfo.line.point0;
				if (!(Mathf.Abs(val2.y) > 1.49f) && !(Vector3Ex.Magnitude2D(val2) > 1.49f))
				{
					Construction.lastPlacementError = "Too close to another building";
					Pool.FreeList<BuildingBlock>(ref list);
					return true;
				}
			}
		}
		Pool.FreeList<BuildingBlock>(ref list);
		return false;
	}

	private static ProximityInfo GetProximity(Construction construction1, Vector3 position1, Quaternion rotation1, Construction construction2, Vector3 position2, Quaternion rotation2)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		ProximityInfo result = default(ProximityInfo);
		result.hit = false;
		result.connection = false;
		result.line = default(Line);
		result.sqrDist = float.MaxValue;
		for (int i = 0; i < construction1.allSockets.Length; i++)
		{
			ConstructionSocket constructionSocket = construction1.allSockets[i] as ConstructionSocket;
			if (constructionSocket == null)
			{
				continue;
			}
			for (int j = 0; j < construction2.allSockets.Length; j++)
			{
				Socket_Base socket = construction2.allSockets[j];
				if (constructionSocket.CanConnect(position1, rotation1, socket, position2, rotation2))
				{
					result.connection = true;
					return result;
				}
			}
		}
		if (construction1.isServer)
		{
			for (int k = 0; k < construction1.allSockets.Length; k++)
			{
				NeighbourSocket neighbourSocket = construction1.allSockets[k] as NeighbourSocket;
				if (neighbourSocket == null)
				{
					continue;
				}
				for (int l = 0; l < construction2.allSockets.Length; l++)
				{
					Socket_Base socket2 = construction2.allSockets[l];
					if (neighbourSocket.CanConnect(position1, rotation1, socket2, position2, rotation2))
					{
						result.connection = true;
						return result;
					}
				}
			}
		}
		if (!result.connection && construction1.allProximities.Length != 0)
		{
			Line val = default(Line);
			for (int m = 0; m < construction1.allSockets.Length; m++)
			{
				ConstructionSocket constructionSocket2 = construction1.allSockets[m] as ConstructionSocket;
				if (constructionSocket2 == null || constructionSocket2.socketType != ConstructionSocket.Type.Wall)
				{
					continue;
				}
				Vector3 selectPivot = constructionSocket2.GetSelectPivot(position1, rotation1);
				for (int n = 0; n < construction2.allProximities.Length; n++)
				{
					Vector3 selectPivot2 = construction2.allProximities[n].GetSelectPivot(position2, rotation2);
					((Line)(ref val))._002Ector(selectPivot, selectPivot2);
					Vector3 val2 = val.point1 - val.point0;
					float sqrMagnitude = ((Vector3)(ref val2)).sqrMagnitude;
					if (sqrMagnitude < result.sqrDist)
					{
						result.hit = true;
						result.line = val;
						result.sqrDist = sqrMagnitude;
					}
				}
			}
		}
		return result;
	}

	public Vector3 GetSelectPivot(Vector3 position, Quaternion rotation)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		return position + rotation * worldPosition;
	}

	protected override Type GetIndexedType()
	{
		return typeof(BuildingProximity);
	}
}
