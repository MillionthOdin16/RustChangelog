using UnityEngine;

public class ModelConditionTest_RoofTop : ModelConditionTest
{
	private const string roof_square = "roof/";

	private const string roof_triangle = "roof.triangle/";

	private const string socket_bot_right = "sockets/neighbour/3";

	private const string socket_bot_left = "sockets/neighbour/4";

	private const string socket_top_right = "sockets/neighbour/5";

	private const string socket_top_left = "sockets/neighbour/6";

	private static string[] sockets_top_right = new string[2] { "roof/sockets/neighbour/5", "roof.triangle/sockets/neighbour/5" };

	private static string[] sockets_top_left = new string[2] { "roof/sockets/neighbour/6", "roof.triangle/sockets/neighbour/6" };

	protected void OnDrawGizmosSelected()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		Gizmos.matrix = ((Component)this).transform.localToWorldMatrix;
		Gizmos.color = Color.gray;
		Gizmos.DrawWireCube(new Vector3(0f, -1.5f, 3f), new Vector3(3f, 3f, 3f));
	}

	public override bool DoTest(BaseEntity ent)
	{
		bool flag = false;
		bool flag2 = false;
		EntityLink entityLink = ent.FindLink(sockets_top_right);
		if (entityLink == null)
		{
			return false;
		}
		for (int i = 0; i < entityLink.connections.Count; i++)
		{
			EntityLink entityLink2 = entityLink.connections[i];
			if (entityLink2.name.EndsWith("sockets/neighbour/3"))
			{
				flag = true;
				break;
			}
		}
		EntityLink entityLink3 = ent.FindLink(sockets_top_left);
		if (entityLink3 == null)
		{
			return false;
		}
		for (int j = 0; j < entityLink3.connections.Count; j++)
		{
			EntityLink entityLink4 = entityLink3.connections[j];
			if (entityLink4.name.EndsWith("sockets/neighbour/4"))
			{
				flag2 = true;
				break;
			}
		}
		if (flag && flag2)
		{
			return false;
		}
		return true;
	}
}
