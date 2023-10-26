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
		public readonly TrainTrackSpline track;

		public readonly TrackOrientation orientation;

		public readonly float angle;

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

	public struct MoveRequest
	{
		public delegate MoveResult SplineAction(MoveResult result, MoveRequest request, TrainTrackSpline spline, float splineLength);

		public float distAlongSpline;

		public float maxMoveDist;

		public SplineAction onSpline;

		public TrackRequest trackRequest;

		public float totalDistMoved;

		public float ProjectEndDist(bool facingForward)
		{
			if (!facingForward)
			{
				return distAlongSpline - maxMoveDist;
			}
			return distAlongSpline + maxMoveDist;
		}

		public MoveRequest(float distAlongSpline, float maxMoveDist, SplineAction onSpline, TrackRequest trackRequest)
		{
			this.distAlongSpline = distAlongSpline;
			this.maxMoveDist = maxMoveDist;
			this.onSpline = onSpline;
			this.trackRequest = trackRequest;
			totalDistMoved = 0f;
		}
	}

	public struct TrackRequest
	{
		public TrackSelection trackSelection;

		public TrainTrackSpline preferredAltA;

		public TrainTrackSpline preferredAltB;

		public TrackRequest(TrackSelection trackSelection, TrainTrackSpline preferredAltA, TrainTrackSpline preferredAltB)
		{
			this.trackSelection = trackSelection;
			this.preferredAltA = preferredAltA;
			this.preferredAltB = preferredAltB;
		}
	}

	public struct MoveResult
	{
		public TrainTrackSpline spline;

		public float distAlongSpline;

		public bool atEndOfLine;

		public TrainSignal signal;

		public float totalDistMoved;
	}

	[Tooltip("Is this track spline part of a train station?")]
	public bool isStation;

	[Tooltip("Can above-ground trains spawn here?")]
	public bool aboveGroundSpawn;

	public int hierarchy;

	public static List<TrainTrackSpline> SidingSplines = new List<TrainTrackSpline>();

	private readonly List<ConnectedTrackInfo> nextTracks = new List<ConnectedTrackInfo>();

	private int straightestNextIndex;

	private readonly List<ConnectedTrackInfo> prevTracks = new List<ConnectedTrackInfo>();

	private int straightestPrevIndex;

	public HashSet<ITrainTrackUser> trackUsers = new HashSet<ITrainTrackUser>();

	public HashSet<TrainSignal> signals = new HashSet<TrainSignal>();

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

	public MoveResult MoveAlongSpline(float prevSplineDist, Vector3 askerForward, float distMoved, TrackRequest tReq = default(TrackRequest), MoveRequest.SplineAction onSpline = null)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		MoveRequest request = new MoveRequest(prevSplineDist, distMoved, onSpline, tReq);
		bool facingForward = IsForward(askerForward, prevSplineDist);
		return MoveAlongSpline(request, facingForward, 0f);
	}

	private MoveResult MoveAlongSpline(MoveRequest request, bool facingForward, float prevDistMoved)
	{
		MoveResult moveResult = default(MoveResult);
		moveResult.totalDistMoved = prevDistMoved;
		MoveResult result = moveResult;
		WorldSplineData data = GetData();
		result.distAlongSpline = request.ProjectEndDist(facingForward);
		if (request.onSpline != null)
		{
			result = request.onSpline(result, request, this, data.Length);
		}
		result.spline = this;
		if (result.distAlongSpline < 0f)
		{
			result.totalDistMoved += request.distAlongSpline;
			result = MoveToPrevSpline(result, request, facingForward);
		}
		else if (result.distAlongSpline > data.Length)
		{
			result.totalDistMoved += data.Length - request.distAlongSpline;
			result = MoveToNextSpline(result, request, facingForward, data.Length);
		}
		else
		{
			result.totalDistMoved += Mathf.Abs(result.distAlongSpline - request.distAlongSpline);
		}
		return result;
	}

	private MoveResult MoveToNextSpline(MoveResult result, MoveRequest request, bool facingForward, float splineLength)
	{
		if (HasNextTrack)
		{
			ConnectedTrackInfo trackSelection = GetTrackSelection(nextTracks, straightestNextIndex, nextTrack: true, facingForward, request.trackRequest);
			request.maxMoveDist = (facingForward ? (result.distAlongSpline - splineLength) : (0f - (result.distAlongSpline - splineLength)));
			if (trackSelection.orientation == TrackOrientation.Same)
			{
				request.distAlongSpline = 0f;
			}
			else
			{
				request.distAlongSpline = trackSelection.track.GetLength();
				facingForward = !facingForward;
			}
			return trackSelection.track.MoveAlongSpline(request, facingForward, result.totalDistMoved);
		}
		result.atEndOfLine = true;
		result.distAlongSpline = splineLength;
		return result;
	}

	private MoveResult MoveToPrevSpline(MoveResult result, MoveRequest request, bool facingForward)
	{
		if (HasPrevTrack)
		{
			ConnectedTrackInfo trackSelection = GetTrackSelection(prevTracks, straightestPrevIndex, nextTrack: false, facingForward, request.trackRequest);
			request.maxMoveDist = (facingForward ? result.distAlongSpline : (0f - result.distAlongSpline));
			if (trackSelection.orientation == TrackOrientation.Same)
			{
				request.distAlongSpline = trackSelection.track.GetLength();
			}
			else
			{
				request.distAlongSpline = 0f;
				facingForward = !facingForward;
			}
			return trackSelection.track.MoveAlongSpline(request, facingForward, result.totalDistMoved);
		}
		result.atEndOfLine = true;
		result.distAlongSpline = 0f;
		return result;
	}

	public float GetDistance(Vector3 position, float maxError, out float minSplineDist)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
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
				float num5 = Vector3.SqrMagnitude(data.GetPointCubicHermite(i) - val);
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
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return GetPointCubicHermiteWorld(distance);
	}

	public Vector3 GetPositionAndTangent(float distance, Vector3 askerForward, out Vector3 tangent)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		Vector3 pointAndTangentCubicHermiteWorld = GetPointAndTangentCubicHermiteWorld(distance, out tangent);
		if (Vector3.Dot(askerForward, tangent) < 0f)
		{
			tangent = -tangent;
		}
		return pointAndTangentCubicHermiteWorld;
	}

	public void AddTrackConnection(TrainTrackSpline track, TrackPosition p, TrackOrientation o)
	{
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
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

	public void RegisterSignal(TrainSignal signal)
	{
		signals.Add(signal);
	}

	public void DeregisterSignal(TrainSignal signal)
	{
		if (!((Object)(object)signal == (Object)null))
		{
			signals.Remove(signal);
		}
	}

	public bool IsForward(Vector3 askerForward, float askerSplineDist)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		WorldSplineData data = GetData();
		Vector3 tangentCubicHermiteWorld = GetTangentCubicHermiteWorld(askerSplineDist, data);
		return Vector3.Dot(askerForward, tangentCubicHermiteWorld) >= 0f;
	}

	public bool HasValidHazardWithin(TrainCar asker, float askerSplineDist, float minHazardDist, float maxHazardDist, TrackSelection trackSelection, float trackSpeed, TrainTrackSpline preferredAltA, TrainTrackSpline preferredAltB)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		Vector3 askerForward = ((trackSpeed >= 0f) ? ((Component)asker).transform.forward : (-((Component)asker).transform.forward));
		bool movingForward = IsForward(askerForward, askerSplineDist);
		return HasValidHazardWithin(asker, askerForward, askerSplineDist, minHazardDist, maxHazardDist, trackSelection, movingForward, preferredAltA, preferredAltB);
	}

	public bool HasValidHazardWithin(ITrainTrackUser asker, Vector3 askerForward, float askerSplineDist, float minHazardDist, float maxHazardDist, TrackSelection trackSelection, bool movingForward, TrainTrackSpline preferredAltA, TrainTrackSpline preferredAltB)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
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
				ConnectedTrackInfo connectedTrackInfo = GetTrackSelection(request: new TrackRequest(trackSelection, preferredAltA, preferredAltB), trackOptions: prevTracks, straightestIndex: straightestPrevIndex, nextTrack: false, trainForward: movingForward);
				if (connectedTrackInfo.orientation == TrackOrientation.Same)
				{
					askerSplineDist = connectedTrackInfo.track.GetLength();
				}
				else
				{
					askerSplineDist = 0f;
					movingForward = !movingForward;
				}
				float minHazardDist2 = Mathf.Max(0f - num, 0f);
				float maxHazardDist2 = 0f - num2;
				return connectedTrackInfo.track.HasValidHazardWithin(asker, askerForward, askerSplineDist, minHazardDist2, maxHazardDist2, trackSelection, movingForward, preferredAltA, preferredAltB);
			}
		}
		else if (num2 > data.Length && HasNextTrack)
		{
			ConnectedTrackInfo connectedTrackInfo2 = GetTrackSelection(request: new TrackRequest(trackSelection, preferredAltA, preferredAltB), trackOptions: nextTracks, straightestIndex: straightestNextIndex, nextTrack: true, trainForward: movingForward);
			if (connectedTrackInfo2.orientation == TrackOrientation.Same)
			{
				askerSplineDist = 0f;
			}
			else
			{
				askerSplineDist = connectedTrackInfo2.track.GetLength();
				movingForward = !movingForward;
			}
			float minHazardDist3 = Mathf.Max(num - data.Length, 0f);
			float maxHazardDist3 = num2 - data.Length;
			return connectedTrackInfo2.track.HasValidHazardWithin(asker, askerForward, askerSplineDist, minHazardDist3, maxHazardDist3, trackSelection, movingForward, preferredAltA, preferredAltB);
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
		if (!HasConnectedNextTrack(tts))
		{
			return HasConnectedPrevTrack(tts);
		}
		return true;
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
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
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

	private ConnectedTrackInfo GetTrackSelection(List<ConnectedTrackInfo> trackOptions, int straightestIndex, bool nextTrack, bool trainForward, TrackRequest request)
	{
		if (trackOptions.Count == 1)
		{
			return trackOptions[0];
		}
		foreach (ConnectedTrackInfo trackOption in trackOptions)
		{
			if ((Object)(object)trackOption.track == (Object)(object)request.preferredAltA || (Object)(object)trackOption.track == (Object)(object)request.preferredAltB)
			{
				return trackOption;
			}
		}
		bool flag = nextTrack ^ trainForward;
		switch (request.trackSelection)
		{
		case TrackSelection.Left:
			if (!flag)
			{
				return trackOptions[0];
			}
			return trackOptions[trackOptions.Count - 1];
		case TrackSelection.Right:
			if (!flag)
			{
				return trackOptions[trackOptions.Count - 1];
			}
			return trackOptions[0];
		default:
			return trackOptions[straightestIndex];
		}
	}

	public static bool TryFindTrackNear(Vector3 pos, float maxDist, out TrainTrackSpline splineResult, out float distResult)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
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
