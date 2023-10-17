using System.Collections.Generic;
using Facepunch;
using UnityEngine;

public class TrainTrackSpline : WorldSpline
{
	public enum TrackSelection
	{
		Default,
		Left,
		Right
	}

	public enum TrackPosition
	{
		Next,
		Prev
	}

	public enum TrackOrientation
	{
		Same,
		Reverse
	}

	private class ConnectedTrackInfo
	{
		public TrainTrackSpline track;

		public TrackOrientation orientation;

		public float angle;

		public ConnectedTrackInfo(TrainTrackSpline track, TrackOrientation orientation, float angle)
		{
			this.track = track;
			this.orientation = orientation;
			this.angle = angle;
		}
	}

	public enum DistanceType
	{
		SplineDistance,
		WorldDistance
	}

	public interface ITrainTrackUser
	{
		Vector3 Position { get; }

		float FrontWheelSplineDist { get; }

		TrainCar.TrainCarType CarType { get; }

		Vector3 GetWorldVelocity();
	}

	[Tooltip("Is this track spline part of a train station?")]
	public bool isStation;

	[Tooltip("Can above-ground trains spawn here?")]
	public bool aboveGroundSpawn;

	public int hierarchy;

	public static List<TrainTrackSpline> SidingSplines = new List<TrainTrackSpline>();

	private List<ConnectedTrackInfo> nextTracks = new List<ConnectedTrackInfo>();

	private int straightestNextIndex = 0;

	private List<ConnectedTrackInfo> prevTracks = new List<ConnectedTrackInfo>();

	private int straightestPrevIndex = 0;

	private HashSet<ITrainTrackUser> trackUsers = new HashSet<ITrainTrackUser>();

	private bool HasNextTrack => nextTracks.Count > 0;

	private bool HasPrevTrack => prevTracks.Count > 0;

	public void SetAll(Vector3[] points, Vector3[] tangents, TrainTrackSpline sourceSpline)
	{
		base.points = points;
		base.tangents = tangents;
		lutInterval = sourceSpline.lutInterval;
		isStation = sourceSpline.isStation;
		aboveGroundSpawn = sourceSpline.aboveGroundSpawn;
		hierarchy = sourceSpline.hierarchy;
	}

	public float GetSplineDistAfterMove(float prevSplineDist, Vector3 askerForward, float distMoved, TrackSelection trackSelection, out TrainTrackSpline onSpline, out bool atEndOfLine, TrainTrackSpline preferredAltA, TrainTrackSpline preferredAltB)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		bool facingForward = IsForward(askerForward, prevSplineDist);
		return GetSplineDistAfterMove(prevSplineDist, distMoved, trackSelection, facingForward, out onSpline, out atEndOfLine, preferredAltA, preferredAltB);
	}

	private float GetSplineDistAfterMove(float prevSplineDist, float distMoved, TrackSelection trackSelection, bool facingForward, out TrainTrackSpline onSpline, out bool atEndOfLine, TrainTrackSpline preferredAltA, TrainTrackSpline preferredAltB)
	{
		WorldSplineData data = GetData();
		float num = (facingForward ? (prevSplineDist + distMoved) : (prevSplineDist - distMoved));
		atEndOfLine = false;
		onSpline = this;
		if (num < 0f)
		{
			if (HasPrevTrack)
			{
				ConnectedTrackInfo trackSelection2 = GetTrackSelection(prevTracks, straightestPrevIndex, trackSelection, nextTrack: false, facingForward, preferredAltA, preferredAltB);
				float distMoved2 = (facingForward ? num : (0f - num));
				if (trackSelection2.orientation == TrackOrientation.Same)
				{
					prevSplineDist = trackSelection2.track.GetLength();
				}
				else
				{
					prevSplineDist = 0f;
					facingForward = !facingForward;
				}
				return trackSelection2.track.GetSplineDistAfterMove(prevSplineDist, distMoved2, trackSelection, facingForward, out onSpline, out atEndOfLine, preferredAltA, preferredAltB);
			}
			atEndOfLine = true;
			num = 0f;
		}
		else if (num > data.Length)
		{
			if (HasNextTrack)
			{
				ConnectedTrackInfo trackSelection3 = GetTrackSelection(nextTracks, straightestNextIndex, trackSelection, nextTrack: true, facingForward, preferredAltA, preferredAltB);
				float distMoved3 = (facingForward ? (num - data.Length) : (0f - (num - data.Length)));
				if (trackSelection3.orientation == TrackOrientation.Same)
				{
					prevSplineDist = 0f;
				}
				else
				{
					prevSplineDist = trackSelection3.track.GetLength();
					facingForward = !facingForward;
				}
				return trackSelection3.track.GetSplineDistAfterMove(prevSplineDist, distMoved3, trackSelection, facingForward, out onSpline, out atEndOfLine, preferredAltA, preferredAltB);
			}
			atEndOfLine = true;
			num = data.Length;
		}
		return num;
	}

	public float GetDistance(Vector3 position, float maxError, out float minSplineDist)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		WorldSplineData data = GetData();
		float num = maxError * maxError;
		Vector3 val = ((Component)this).transform.InverseTransformPoint(position);
		float num2 = float.MaxValue;
		minSplineDist = 0f;
		int num3 = 0;
		int num4 = data.LUTValues.Count;
		if (data.Length > 40f)
		{
			for (int i = 0; (float)i < data.Length + 10f; i += 10)
			{
				Vector3 pointCubicHermite = data.GetPointCubicHermite(i);
				float num5 = Vector3.SqrMagnitude(pointCubicHermite - val);
				if (num5 < num2)
				{
					num2 = num5;
					minSplineDist = i;
				}
			}
			num3 = Mathf.FloorToInt(Mathf.Max(0f, minSplineDist - 10f + 1f));
			num4 = Mathf.CeilToInt(Mathf.Min((float)data.LUTValues.Count, minSplineDist + 10f - 1f));
		}
		for (int j = num3; j < num4; j++)
		{
			WorldSplineData.LUTEntry lUTEntry = data.LUTValues[j];
			for (int k = 0; k < lUTEntry.points.Count; k++)
			{
				WorldSplineData.LUTEntry.LUTPoint lUTPoint = lUTEntry.points[k];
				float num6 = Vector3.SqrMagnitude(lUTPoint.pos - val);
				if (num6 < num2)
				{
					num2 = num6;
					minSplineDist = lUTPoint.distance;
					if (num6 < num)
					{
						break;
					}
				}
			}
		}
		return Mathf.Sqrt(num2);
	}

	public float GetLength()
	{
		return GetData().Length;
	}

	public Vector3 GetPosition(float distance)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		return GetPointCubicHermiteWorld(distance);
	}

	public Vector3 GetPositionAndTangent(float distance, Vector3 askerForward, out Vector3 tangent)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		Vector3 pointAndTangentCubicHermiteWorld = GetPointAndTangentCubicHermiteWorld(distance, out tangent);
		if (Vector3.Dot(askerForward, tangent) < 0f)
		{
			tangent = -tangent;
		}
		return pointAndTangentCubicHermiteWorld;
	}

	public void AddTrackConnection(TrainTrackSpline track, TrackPosition p, TrackOrientation o)
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		List<ConnectedTrackInfo> list = ((p == TrackPosition.Next) ? nextTracks : prevTracks);
		for (int i = 0; i < list.Count; i++)
		{
			if ((Object)(object)list[i].track == (Object)(object)track)
			{
				return;
			}
		}
		Vector3 val = ((p == TrackPosition.Next) ? points[points.Length - 2] : points[0]);
		Vector3 val2 = ((p == TrackPosition.Next) ? points[points.Length - 1] : points[1]);
		Vector3 val3 = ((Component)this).transform.TransformPoint(val2) - ((Component)this).transform.TransformPoint(val);
		Vector3 initialVector = GetInitialVector(track, p, o);
		float num = Vector3.SignedAngle(val3, initialVector, Vector3.up);
		int j;
		for (j = 0; j < list.Count && !(list[j].angle > num); j++)
		{
		}
		list.Insert(j, new ConnectedTrackInfo(track, o, num));
		int num2 = int.MaxValue;
		for (int k = 0; k < list.Count; k++)
		{
			num2 = Mathf.Min(num2, list[k].track.hierarchy);
		}
		float num3 = float.MaxValue;
		int num4 = 0;
		for (int l = 0; l < list.Count; l++)
		{
			ConnectedTrackInfo connectedTrackInfo = list[l];
			if (connectedTrackInfo.track.hierarchy > num2)
			{
				continue;
			}
			float num5 = Mathf.Abs(connectedTrackInfo.angle);
			if (num5 < num3)
			{
				num3 = num5;
				num4 = l;
				if (num3 == 0f)
				{
					break;
				}
			}
		}
		if (p == TrackPosition.Next)
		{
			straightestNextIndex = num4;
		}
		else
		{
			straightestPrevIndex = num4;
		}
	}

	public void RegisterTrackUser(ITrainTrackUser user)
	{
		trackUsers.Add(user);
	}

	public void DeregisterTrackUser(ITrainTrackUser user)
	{
		if (user != null)
		{
			trackUsers.Remove(user);
		}
	}

	public bool IsForward(Vector3 askerForward, float askerSplineDist)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		WorldSplineData data = GetData();
		Vector3 tangentCubicHermiteWorld = GetTangentCubicHermiteWorld(askerSplineDist, data);
		return Vector3.Dot(askerForward, tangentCubicHermiteWorld) >= 0f;
	}

	public bool HasValidHazardWithin(TrainCar asker, float askerSplineDist, float minHazardDist, float maxHazardDist, TrackSelection trackSelection, float trackSpeed, TrainTrackSpline preferredAltA, TrainTrackSpline preferredAltB)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		Vector3 askerForward = ((trackSpeed >= 0f) ? ((Component)asker).transform.forward : (-((Component)asker).transform.forward));
		bool movingForward = IsForward(askerForward, askerSplineDist);
		return HasValidHazardWithin(asker, askerForward, askerSplineDist, minHazardDist, maxHazardDist, trackSelection, movingForward, preferredAltA, preferredAltB);
	}

	public bool HasValidHazardWithin(ITrainTrackUser asker, Vector3 askerForward, float askerSplineDist, float minHazardDist, float maxHazardDist, TrackSelection trackSelection, bool movingForward, TrainTrackSpline preferredAltA, TrainTrackSpline preferredAltB)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		WorldSplineData data = GetData();
		foreach (ITrainTrackUser trackUser in trackUsers)
		{
			if (trackUser == asker)
			{
				continue;
			}
			Vector3 val = trackUser.Position - asker.Position;
			if (!(Vector3.Dot(askerForward, val) >= 0f))
			{
				continue;
			}
			float magnitude = ((Vector3)(ref val)).magnitude;
			if (magnitude > minHazardDist && magnitude < maxHazardDist)
			{
				Vector3 worldVelocity = trackUser.GetWorldVelocity();
				if (((Vector3)(ref worldVelocity)).sqrMagnitude < 4f || Vector3.Dot(worldVelocity, val) < 0f)
				{
					return true;
				}
			}
		}
		float num = (movingForward ? (askerSplineDist + minHazardDist) : (askerSplineDist - minHazardDist));
		float num2 = (movingForward ? (askerSplineDist + maxHazardDist) : (askerSplineDist - maxHazardDist));
		if (num2 < 0f)
		{
			if (HasPrevTrack)
			{
				ConnectedTrackInfo trackSelection2 = GetTrackSelection(prevTracks, straightestPrevIndex, trackSelection, nextTrack: false, movingForward, preferredAltA, preferredAltB);
				if (trackSelection2.orientation == TrackOrientation.Same)
				{
					askerSplineDist = trackSelection2.track.GetLength();
				}
				else
				{
					askerSplineDist = 0f;
					movingForward = !movingForward;
				}
				float minHazardDist2 = Mathf.Max(0f - num, 0f);
				float maxHazardDist2 = 0f - num2;
				return trackSelection2.track.HasValidHazardWithin(asker, askerForward, askerSplineDist, minHazardDist2, maxHazardDist2, trackSelection, movingForward, preferredAltA, preferredAltB);
			}
		}
		else if (num2 > data.Length && HasNextTrack)
		{
			ConnectedTrackInfo trackSelection3 = GetTrackSelection(nextTracks, straightestNextIndex, trackSelection, nextTrack: true, movingForward, preferredAltA, preferredAltB);
			if (trackSelection3.orientation == TrackOrientation.Same)
			{
				askerSplineDist = 0f;
			}
			else
			{
				askerSplineDist = trackSelection3.track.GetLength();
				movingForward = !movingForward;
			}
			float minHazardDist3 = Mathf.Max(num - data.Length, 0f);
			float maxHazardDist3 = num2 - data.Length;
			return trackSelection3.track.HasValidHazardWithin(asker, askerForward, askerSplineDist, minHazardDist3, maxHazardDist3, trackSelection, movingForward, preferredAltA, preferredAltB);
		}
		return false;
	}

	public bool HasAnyUsers()
	{
		return trackUsers.Count > 0;
	}

	public bool HasAnyUsersOfType(TrainCar.TrainCarType carType)
	{
		foreach (ITrainTrackUser trackUser in trackUsers)
		{
			if (trackUser.CarType == carType)
			{
				return true;
			}
		}
		return false;
	}

	public bool HasConnectedTrack(TrainTrackSpline tts)
	{
		return HasConnectedNextTrack(tts) || HasConnectedPrevTrack(tts);
	}

	public bool HasConnectedNextTrack(TrainTrackSpline tts)
	{
		foreach (ConnectedTrackInfo nextTrack in nextTracks)
		{
			if ((Object)(object)nextTrack.track == (Object)(object)tts)
			{
				return true;
			}
		}
		return false;
	}

	public bool HasConnectedPrevTrack(TrainTrackSpline tts)
	{
		foreach (ConnectedTrackInfo prevTrack in prevTracks)
		{
			if ((Object)(object)prevTrack.track == (Object)(object)tts)
			{
				return true;
			}
		}
		return false;
	}

	private static Vector3 GetInitialVector(TrainTrackSpline track, TrackPosition p, TrackOrientation o)
	{
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val;
		Vector3 val2;
		if (p == TrackPosition.Next)
		{
			if (o == TrackOrientation.Reverse)
			{
				val = track.points[track.points.Length - 1];
				val2 = track.points[track.points.Length - 2];
			}
			else
			{
				val = track.points[0];
				val2 = track.points[1];
			}
		}
		else if (o == TrackOrientation.Reverse)
		{
			val = track.points[1];
			val2 = track.points[0];
		}
		else
		{
			val = track.points[track.points.Length - 2];
			val2 = track.points[track.points.Length - 1];
		}
		return ((Component)track).transform.TransformPoint(val2) - ((Component)track).transform.TransformPoint(val);
	}

	protected override void OnDrawGizmosSelected()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		base.OnDrawGizmosSelected();
		for (int i = 0; i < nextTracks.Count; i++)
		{
			Color splineColour = Color.white;
			if (straightestNextIndex != i && nextTracks.Count > 1)
			{
				if (i == 0)
				{
					splineColour = Color.green;
				}
				else if (i == nextTracks.Count - 1)
				{
					splineColour = Color.yellow;
				}
			}
			WorldSpline.DrawSplineGizmo(nextTracks[i].track, splineColour);
		}
		for (int j = 0; j < prevTracks.Count; j++)
		{
			Color splineColour2 = Color.white;
			if (straightestPrevIndex != j && prevTracks.Count > 1)
			{
				if (j == 0)
				{
					splineColour2 = Color.green;
				}
				else if (j == nextTracks.Count - 1)
				{
					splineColour2 = Color.yellow;
				}
			}
			WorldSpline.DrawSplineGizmo(prevTracks[j].track, splineColour2);
		}
	}

	private ConnectedTrackInfo GetTrackSelection(List<ConnectedTrackInfo> trackOptions, int straightestIndex, TrackSelection trackSelection, bool nextTrack, bool trainForward, TrainTrackSpline preferredAltA, TrainTrackSpline preferredAltB)
	{
		if (trackOptions.Count == 1)
		{
			return trackOptions[0];
		}
		foreach (ConnectedTrackInfo trackOption in trackOptions)
		{
			if ((Object)(object)trackOption.track == (Object)(object)preferredAltA || (Object)(object)trackOption.track == (Object)(object)preferredAltB)
			{
				return trackOption;
			}
		}
		bool flag = nextTrack ^ trainForward;
		return trackSelection switch
		{
			TrackSelection.Left => flag ? trackOptions[trackOptions.Count - 1] : trackOptions[0], 
			TrackSelection.Right => flag ? trackOptions[0] : trackOptions[trackOptions.Count - 1], 
			_ => trackOptions[straightestIndex], 
		};
	}

	public static bool TryFindTrackNear(Vector3 pos, float maxDist, out TrainTrackSpline splineResult, out float distResult)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		splineResult = null;
		distResult = 0f;
		List<Collider> list = Pool.GetList<Collider>();
		GamePhysics.OverlapSphere(pos, maxDist, list, 65536, (QueryTriggerInteraction)1);
		if (list.Count > 0)
		{
			List<TrainTrackSpline> list2 = Pool.GetList<TrainTrackSpline>();
			float num = float.MaxValue;
			foreach (Collider item in list)
			{
				((Component)item).GetComponentsInParent<TrainTrackSpline>(false, list2);
				if (list2.Count <= 0)
				{
					continue;
				}
				foreach (TrainTrackSpline item2 in list2)
				{
					float minSplineDist;
					float distance = item2.GetDistance(pos, 1f, out minSplineDist);
					if (distance < num)
					{
						num = distance;
						distResult = minSplineDist;
						splineResult = item2;
					}
				}
			}
			Pool.FreeList<TrainTrackSpline>(ref list2);
		}
		Pool.FreeList<Collider>(ref list);
		return (Object)(object)splineResult != (Object)null;
	}
}
