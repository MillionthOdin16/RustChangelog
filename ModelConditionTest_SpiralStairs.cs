using UnityEngine;

public class ModelConditionTest_SpiralStairs : ModelConditionTest
{
	private const string stairs_socket_female = "sockets/stairs-female/1";

	private static string[] stairs_sockets_female = new string[2] { "block.stair.spiral/sockets/stairs-female/1", "block.stair.spiral.triangle/sockets/stairs-female/1" };

	private const string floor_socket_female = "sockets/floor-female/1";

	private static string[] floor_sockets_female = new string[2] { "block.stair.spiral/sockets/floor-female/1", "block.stair.spiral.triangle/sockets/floor-female/1" };

	protected void OnDrawGizmosSelected()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		Gizmos.matrix = ((Component)this).transform.localToWorldMatrix;
		Gizmos.color = Color.gray;
		Gizmos.DrawWireCube(new Vector3(0f, 2.35f, 0f), new Vector3(3f, 1.5f, 3f));
	}

	public override bool DoTest(BaseEntity ent)
	{
		BuildingBlock buildingBlock = ent as BuildingBlock;
		if ((Object)(object)buildingBlock == (Object)null)
		{
			return false;
		}
		EntityLink entityLink = ent.FindLink(stairs_sockets_female);
		if (entityLink == null)
		{
			return false;
		}
		for (int i = 0; i < entityLink.connections.Count; i++)
		{
			BuildingBlock buildingBlock2 = entityLink.connections[i].owner as BuildingBlock;
			if (!((Object)(object)buildingBlock2 == (Object)null) && buildingBlock2.grade == buildingBlock.grade)
			{
				return false;
			}
		}
		EntityLink entityLink2 = ent.FindLink(floor_sockets_female);
		if (entityLink2 == null)
		{
			return true;
		}
		if (!entityLink2.IsEmpty())
		{
			return false;
		}
		return true;
	}
}
