using System.Collections.Generic;
using UnityEngine;

public class RuntimePath : IAIPath
{
	private List<IAIPathSpeedZone> speedZones = new List<IAIPathSpeedZone>();

	private List<IAIPathInterestNode> interestNodes = new List<IAIPathInterestNode>();

	public IAIPathNode[] Nodes { get; set; } = new IAIPathNode[0];


	public IEnumerable<IAIPathSpeedZone> SpeedZones => speedZones;

	public IEnumerable<IAIPathInterestNode> InterestNodes => interestNodes;

	public IAIPathNode GetClosestToPoint(Vector3 point)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		IAIPathNode result = Nodes[0];
		float num = float.PositiveInfinity;
		IAIPathNode[] nodes = Nodes;
		foreach (IAIPathNode iAIPathNode in nodes)
		{
			Vector3 val = point - iAIPathNode.Position;
			float sqrMagnitude = ((Vector3)(ref val)).sqrMagnitude;
			if (sqrMagnitude < num)
			{
				num = sqrMagnitude;
				result = iAIPathNode;
			}
		}
		return result;
	}

	public void GetNodesNear(Vector3 point, ref List<IAIPathNode> nearNodes, float dist = 10f)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		IAIPathNode[] nodes = Nodes;
		foreach (IAIPathNode iAIPathNode in nodes)
		{
			Vector3 val = Vector3Ex.XZ(point) - Vector3Ex.XZ(iAIPathNode.Position);
			if (((Vector3)(ref val)).sqrMagnitude <= dist * dist)
			{
				nearNodes.Add(iAIPathNode);
			}
		}
	}

	public IAIPathInterestNode GetRandomInterestNodeAwayFrom(Vector3 from, float dist = 10f)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		IAIPathInterestNode iAIPathInterestNode = null;
		int num = 0;
		while (iAIPathInterestNode == null && num < 20)
		{
			iAIPathInterestNode = interestNodes[Random.Range(0, interestNodes.Count)];
			Vector3 val = iAIPathInterestNode.Position - from;
			if (!(((Vector3)(ref val)).sqrMagnitude < dist * dist))
			{
				break;
			}
			iAIPathInterestNode = null;
			num++;
		}
		if (iAIPathInterestNode == null)
		{
			Debug.LogError((object)"Returning default interest zone");
			iAIPathInterestNode = interestNodes[0];
		}
		return iAIPathInterestNode;
	}

	public void AddInterestNode(IAIPathInterestNode interestNode)
	{
		if (!interestNodes.Contains(interestNode))
		{
			interestNodes.Add(interestNode);
		}
	}

	public void AddSpeedZone(IAIPathSpeedZone speedZone)
	{
		if (!speedZones.Contains(speedZone))
		{
			speedZones.Add(speedZone);
		}
	}
}
