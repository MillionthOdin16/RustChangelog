using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

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
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		Profiler.BeginSample("BasePath.GetNodesNear");
		foreach (BasePathNode node in nodes)
		{
			Vector3 val = Vector3Ex.XZ(point) - Vector3Ex.XZ(node.Position);
			if (((Vector3)(ref val)).sqrMagnitude <= dist * dist)
			{
				nearNodes.Add(node);
			}
		}
		Profiler.EndSample();
	}

	public IAIPathNode GetClosestToPoint(Vector3 point)
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		Profiler.BeginSample("BasePath.GetClosestToPoint");
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
		Profiler.EndSample();
		return result;
	}

	public IAIPathInterestNode GetRandomInterestNodeAwayFrom(Vector3 from, float dist = 10f)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		PathInterestNode pathInterestNode = null;
		int num = 0;
		while ((Object)(object)pathInterestNode == (Object)null && num < 20)
		{
			pathInterestNode = interestZones[Random.Range(0, interestZones.Count)];
			Vector3 val = ((Component)pathInterestNode).transform.position - from;
			if (((Vector3)(ref val)).sqrMagnitude < dist * dist)
			{
				pathInterestNode = null;
				num++;
				continue;
			}
			break;
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
