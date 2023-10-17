using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class RuntimePath : IAIPath
{
	private List<IAIPathSpeedZone> speedZones = new List<IAIPathSpeedZone>();

	private List<IAIPathInterestNode> interestNodes = new List<IAIPathInterestNode>();

	public IAIPathNode[] Nodes { get; set; } = new IAIPathNode[0];


	public IEnumerable<IAIPathSpeedZone> SpeedZones => speedZones;

	public IEnumerable<IAIPathInterestNode> InterestNodes => interestNodes;

	public IAIPathNode GetClosestToPoint(Vector3 point)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		Profiler.BeginSample("RuntimePath.GetClosestToPoint");
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
		Profiler.EndSample();
		return result;
	}

	public void GetNodesNear(Vector3 point, ref List<IAIPathNode> nearNodes, float dist = 10f)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		Profiler.BeginSample("RuntimePath.GetNodesNear");
		IAIPathNode[] nodes = Nodes;
		foreach (IAIPathNode iAIPathNode in nodes)
		{
			Vector3 val = Vector3Ex.XZ(point) - Vector3Ex.XZ(iAIPathNode.Position);
			if (((Vector3)(ref val)).sqrMagnitude <= dist * dist)
			{
				nearNodes.Add(iAIPathNode);
			}
		}
		Profiler.EndSample();
	}

	public IAIPathInterestNode GetRandomInterestNodeAwayFrom(Vector3 from, float dist = 10f)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		IAIPathInterestNode iAIPathInterestNode = null;
		int num = 0;
		while (iAIPathInterestNode == null && num < 20)
		{
			iAIPathInterestNode = interestNodes[Random.Range(0, interestNodes.Count)];
			Vector3 val = iAIPathInterestNode.Position - from;
			if (((Vector3)(ref val)).sqrMagnitude < dist * dist)
			{
				iAIPathInterestNode = null;
				num++;
				continue;
			}
			break;
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
