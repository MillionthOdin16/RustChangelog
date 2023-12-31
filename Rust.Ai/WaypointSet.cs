using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rust.Ai;

public class WaypointSet : MonoBehaviour, IServerComponent
{
	public enum NavModes
	{
		Loop,
		PingPong
	}

	[Serializable]
	public struct Waypoint
	{
		public Transform Transform;

		public float WaitTime;

		public Transform[] LookatPoints;

		[NonSerialized]
		public bool IsOccupied;
	}

	[SerializeField]
	private List<Waypoint> _points = new List<Waypoint>();

	[SerializeField]
	private NavModes navMode;

	public List<Waypoint> Points
	{
		get
		{
			return _points;
		}
		set
		{
			_points = value;
		}
	}

	public NavModes NavMode => navMode;

	private void OnDrawGizmos()
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < Points.Count; i++)
		{
			Transform transform = Points[i].Transform;
			if ((Object)(object)transform != (Object)null)
			{
				if (Points[i].IsOccupied)
				{
					Gizmos.color = Color.red;
				}
				else
				{
					Gizmos.color = Color.cyan;
				}
				Gizmos.DrawSphere(transform.position, 0.25f);
				Gizmos.color = Color.cyan;
				if (i + 1 < Points.Count)
				{
					Gizmos.DrawLine(transform.position, Points[i + 1].Transform.position);
				}
				else if (NavMode == NavModes.Loop)
				{
					Gizmos.DrawLine(transform.position, Points[0].Transform.position);
				}
				Gizmos.color = Color.magenta - new Color(0f, 0f, 0f, 0.5f);
				Transform[] lookatPoints = Points[i].LookatPoints;
				foreach (Transform val in lookatPoints)
				{
					Gizmos.DrawSphere(val.position, 0.1f);
					Gizmos.DrawLine(transform.position, val.position);
				}
			}
		}
	}
}
