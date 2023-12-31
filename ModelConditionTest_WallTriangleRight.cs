using UnityEngine;

public class ModelConditionTest_WallTriangleRight : ModelConditionTest
{
	private const string socket_1 = "wall/sockets/wall-female";

	private const string socket_2 = "wall/sockets/floor-female/1";

	private const string socket_3 = "wall/sockets/floor-female/2";

	private const string socket_4 = "wall/sockets/floor-female/3";

	private const string socket_5 = "wall/sockets/floor-female/4";

	private const string socket_6 = "wall/sockets/stability/2";

	private const string socket = "wall/sockets/neighbour/1";

	public static bool CheckCondition(BaseEntity ent)
	{
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		if (CheckSocketOccupied(ent, "wall/sockets/wall-female"))
		{
			return false;
		}
		if (CheckSocketOccupied(ent, "wall/sockets/floor-female/1"))
		{
			return false;
		}
		if (CheckSocketOccupied(ent, "wall/sockets/floor-female/2"))
		{
			return false;
		}
		if (CheckSocketOccupied(ent, "wall/sockets/floor-female/3"))
		{
			return false;
		}
		if (CheckSocketOccupied(ent, "wall/sockets/floor-female/4"))
		{
			return false;
		}
		if (CheckSocketOccupied(ent, "wall/sockets/stability/2"))
		{
			return false;
		}
		EntityLink entityLink = ent.FindLink("wall/sockets/neighbour/1");
		if (entityLink == null)
		{
			return false;
		}
		for (int i = 0; i < entityLink.connections.Count; i++)
		{
			BuildingBlock buildingBlock = entityLink.connections[i].owner as BuildingBlock;
			if (!((Object)(object)buildingBlock == (Object)null))
			{
				if (buildingBlock.blockDefinition.info.name.token == "roof" && Vector3.Angle(((Component)ent).transform.forward, -((Component)buildingBlock).transform.forward) < 10f)
				{
					return true;
				}
				if (buildingBlock.blockDefinition.info.name.token == "roof_triangle" && Vector3.Angle(((Component)ent).transform.forward, -((Component)buildingBlock).transform.forward) < 40f)
				{
					return true;
				}
			}
		}
		return false;
	}

	private static bool CheckSocketOccupied(BaseEntity ent, string socket)
	{
		EntityLink entityLink = ent.FindLink(socket);
		if (entityLink == null)
		{
			return false;
		}
		return !entityLink.IsEmpty();
	}

	public override bool DoTest(BaseEntity ent)
	{
		return CheckCondition(ent);
	}
}
