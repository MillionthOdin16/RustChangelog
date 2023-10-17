using UnityEngine;

public class ModelConditionTest_RoofRight : ModelConditionTest
{
	public enum AngleType
	{
		None = -1,
		Straight = 0,
		Convex60 = 60,
		Convex90 = 90,
		Convex120 = 120,
		Concave30 = -30,
		Concave60 = -60,
		Concave90 = -90,
		Concave120 = -120
	}

	public enum ShapeType
	{
		Any = -1,
		Square,
		Triangle
	}

	public AngleType angle = AngleType.None;

	public ShapeType shape = ShapeType.Any;

	private const string roof_square = "roof/";

	private const string roof_triangle = "roof.triangle/";

	private const string socket_right = "sockets/neighbour/3";

	private const string socket_left = "sockets/neighbour/4";

	private static string[] sockets_right = new string[2] { "roof/sockets/neighbour/3", "roof.triangle/sockets/neighbour/3" };

	private bool IsConvex => angle > (AngleType)10;

	private bool IsConcave => angle < (AngleType)(-10);

	protected void OnDrawGizmosSelected()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		Gizmos.matrix = ((Component)this).transform.localToWorldMatrix;
		Gizmos.color = Color.gray;
		Gizmos.DrawWireCube(new Vector3(-3f, 1.5f, 0f), new Vector3(3f, 3f, 3f));
	}

	public override bool DoTest(BaseEntity ent)
	{
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		BuildingBlock buildingBlock = ent as BuildingBlock;
		if ((Object)(object)buildingBlock == (Object)null)
		{
			return false;
		}
		EntityLink entityLink = ent.FindLink(sockets_right);
		if (entityLink == null)
		{
			return false;
		}
		if (angle == AngleType.None)
		{
			for (int i = 0; i < entityLink.connections.Count; i++)
			{
				EntityLink entityLink2 = entityLink.connections[i];
				if (entityLink2.name.EndsWith("sockets/neighbour/4"))
				{
					return false;
				}
			}
			return true;
		}
		if (entityLink.IsEmpty())
		{
			return false;
		}
		bool result = false;
		for (int j = 0; j < entityLink.connections.Count; j++)
		{
			EntityLink entityLink3 = entityLink.connections[j];
			if (!entityLink3.name.EndsWith("sockets/neighbour/4") || (shape == ShapeType.Square && !entityLink3.name.StartsWith("roof/")) || (shape == ShapeType.Triangle && !entityLink3.name.StartsWith("roof.triangle/")))
			{
				continue;
			}
			BuildingBlock buildingBlock2 = entityLink3.owner as BuildingBlock;
			if ((Object)(object)buildingBlock2 == (Object)null || buildingBlock2.grade != buildingBlock.grade)
			{
				continue;
			}
			int num = (int)angle;
			float num2 = 0f - Vector3.SignedAngle(((Component)ent).transform.forward, ((Component)buildingBlock2).transform.forward, Vector3.up);
			if (num2 < (float)(num - 10))
			{
				if (IsConvex)
				{
					return false;
				}
			}
			else if (num2 > (float)(num + 10))
			{
				if (IsConvex)
				{
					return false;
				}
			}
			else
			{
				result = true;
			}
		}
		return result;
	}
}
