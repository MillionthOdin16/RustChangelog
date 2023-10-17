using System;
using System.Collections.Generic;
using UnityEngine;

public class BasePath : MonoBehaviour, IAIPath
{
	public List<BasePathNode> nodes;

	public List<PathInterestNode> interestZones;

	public List<PathSpeedZone> speedZones;

	public IEnumerable<IAIPathInterestNode> InterestNodes => interestZones;

	public IEnumerable<IAIPathSpeedZone> SpeedZones => speedZones;

	private void AddChildren()
	{
		if (nodes != null)
		{
			nodes.Clear();
			nodes.AddRange(((Component)this).GetComponentsInChildren<BasePathNode>());
			foreach (BasePathNode node in nodes)
			{
				node.Path = this;
			}
		}
		if (interestZones != null)
		{
			interestZones.Clear();
			interestZones.AddRange(((Component)this).GetComponentsInChildren<PathInterestNode>());
		}
		if (speedZones != null)
		{
			speedZones.Clear();
			speedZones.AddRange(((Component)this).GetComponentsInChildren<PathSpeedZone>());
		}
	}

	private void ClearChildren()
	{
		if (nodes != null)
		{
			foreach (BasePathNode node in nodes)
			{
				node.linked.Clear();
			}
		}
		nodes.Clear();
	}

	public static void AutoGenerateLinks(BasePath path, float maxRange = -1f)
	{
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		path.AddChildren();
		foreach (BasePathNode node in path.nodes)
		{
			if (node.linked == null)
			{
				node.linked = new List<BasePathNode>();
			}
			else
			{
				node.linked.Clear();
			}
			foreach (BasePathNode node2 in path.nodes)
			{
				if (!((Object)(object)node == (Object)(object)node2) && (maxRange == -1f || !(Vector3.Distance(node.Position, node2.Position) > maxRange)) && GamePhysics.LineOfSight(node.Position, node2.Position, 429990145) && GamePhysics.LineOfSight(node2.Position, node.Position, 429990145))
				{
					node.linked.Add(node2);
				}
			}
		}
	}

	public void GetNodesNear(Vector3 point, ref List<IAIPathNode> nearNodes, float dist = 10f)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		foreach (BasePathNode node in nodes)
		{
			Vector3 val = Vector3Ex.XZ(point) - Vector3Ex.XZ(node.Position);
			if (((Vector3)(ref val)).sqrMagnitude <= dist * dist)
			{
				nearNodes.Add(node);
			}
		}
	}

	public IAIPathNode GetClosestToPoint(Vector3 point)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		IAIPathNode result = nodes[0];
		float num = float.PositiveInfinity;
		foreach (BasePathNode node in nodes)
		{
			if (!((Object)(object)node == (Object)null) && !((Object)(object)((Component)node).transform == (Object)null))
			{
				Vector3 val = point - node.Position;
				float sqrMagnitude = ((Vector3)(ref val)).sqrMagnitude;
				if (sqrMagnitude < num)
				{
					num = sqrMagnitude;
					result = node;
				}
			}
		}
		return result;
	}

	public IAIPathInterestNode GetRandomInterestNodeAwayFrom(Vector3 from, float dist = 10f)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		PathInterestNode pathInterestNode = null;
		int num = 0;
		while ((Object)(object)pathInterestNode == (Object)null && num < 20)
		{
			pathInterestNode = interestZones[Random.Range(0, interestZones.Count)];
			Vector3 val = ((Component)pathInterestNode).transform.position - from;
			if (!(((Vector3)(ref val)).sqrMagnitude < dist * dist))
			{
				break;
			}
			pathInterestNode = null;
			num++;
		}
		if ((Object)(object)pathInterestNode == (Object)null)
		{
			Debug.LogError((object)"REturning default interest zone");
			pathInterestNode = interestZones[0];
		}
		return pathInterestNode;
	}

	public void AddInterestNode(IAIPathInterestNode interestZone)
	{
		throw new NotImplementedException();
	}

	public void AddSpeedZone(IAIPathSpeedZone speedZone)
	{
		throw new NotImplementedException();
	}
}
