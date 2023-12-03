using UnityEngine;

public class CollateTrainTracks : ProceduralComponent
{
	private const float MAX_NODE_DIST = 0.1f;

	private const float MAX_NODE_DIST_SQR = 0.010000001f;

	private const float MAX_NODE_ANGLE = 10f;

	public override bool RunOnCache => true;

	public override void Process(uint seed)
	{
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		TrainTrackSpline[] array = Object.FindObjectsOfType<TrainTrackSpline>();
		for (int num = array.Length - 1; num >= 0; num--)
		{
			TrainTrackSpline ourSpline = array[num];
			if (ourSpline.dataIndex < 0 && ourSpline.points.Length > 3)
			{
				int nodeIndex;
				for (nodeIndex = ourSpline.points.Length - 2; nodeIndex >= 1; nodeIndex--)
				{
					Vector3 ourPos2 = ourSpline.points[nodeIndex];
					Vector3 ourTangent2 = ourSpline.tangents[nodeIndex];
					TrainTrackSpline[] array2 = array;
					foreach (TrainTrackSpline trainTrackSpline in array2)
					{
						if (!((Object)(object)ourSpline == (Object)(object)trainTrackSpline))
						{
							Vector3 startPointWorld = trainTrackSpline.GetStartPointWorld();
							Vector3 endPointWorld = trainTrackSpline.GetEndPointWorld();
							Vector3 startTangentWorld = trainTrackSpline.GetStartTangentWorld();
							Vector3 endTangentWorld = trainTrackSpline.GetEndTangentWorld();
							if (!CompareNodes(startPointWorld, startTangentWorld) && !CompareNodes(endPointWorld, endTangentWorld) && !CompareNodes(startPointWorld, -startTangentWorld))
							{
								CompareNodes(endPointWorld, -endTangentWorld);
							}
						}
					}
					bool CompareNodes(Vector3 theirPos, Vector3 theirTangent)
					{
						//IL_0003: Unknown result type (might be due to invalid IL or missing references)
						//IL_0008: Unknown result type (might be due to invalid IL or missing references)
						//IL_000b: Unknown result type (might be due to invalid IL or missing references)
						//IL_0010: Unknown result type (might be due to invalid IL or missing references)
						//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
						//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
						//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
						//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
						//IL_0114: Unknown result type (might be due to invalid IL or missing references)
						//IL_0119: Unknown result type (might be due to invalid IL or missing references)
						//IL_012f: Unknown result type (might be due to invalid IL or missing references)
						//IL_0134: Unknown result type (might be due to invalid IL or missing references)
						if (NodesConnect(ourPos2, theirPos, ourTangent2, theirTangent))
						{
							TrainTrackSpline trainTrackSpline2 = ((Component)ourSpline).gameObject.AddComponent<TrainTrackSpline>();
							Vector3[] array5 = (Vector3[])(object)new Vector3[ourSpline.points.Length - nodeIndex];
							Vector3[] array6 = (Vector3[])(object)new Vector3[ourSpline.points.Length - nodeIndex];
							Vector3[] array7 = (Vector3[])(object)new Vector3[nodeIndex + 1];
							Vector3[] array8 = (Vector3[])(object)new Vector3[nodeIndex + 1];
							for (int num2 = ourSpline.points.Length - 1; num2 >= 0; num2--)
							{
								if (num2 >= nodeIndex)
								{
									array5[num2 - nodeIndex] = ourSpline.points[num2];
									array6[num2 - nodeIndex] = ourSpline.tangents[num2];
								}
								if (num2 <= nodeIndex)
								{
									array7[num2] = ourSpline.points[num2];
									array8[num2] = ourSpline.tangents[num2];
								}
							}
							ourSpline.SetAll(array7, array8, ourSpline);
							trainTrackSpline2.SetAll(array5, array6, ourSpline);
							nodeIndex--;
							return true;
						}
						return false;
					}
				}
			}
		}
		array = Object.FindObjectsOfType<TrainTrackSpline>();
		TrainTrackSpline[] array3 = array;
		foreach (TrainTrackSpline ourSpline2 in array3)
		{
			Vector3 ourStartPos = ourSpline2.GetStartPointWorld();
			Vector3 ourEndPos = ourSpline2.GetEndPointWorld();
			Vector3 ourStartTangent = ourSpline2.GetStartTangentWorld();
			Vector3 ourEndTangent = ourSpline2.GetEndTangentWorld();
			if (NodesConnect(ourStartPos, ourEndPos, ourStartTangent, ourEndTangent))
			{
				ourSpline2.AddTrackConnection(ourSpline2, TrainTrackSpline.TrackPosition.Next, TrainTrackSpline.TrackOrientation.Same);
				ourSpline2.AddTrackConnection(ourSpline2, TrainTrackSpline.TrackPosition.Prev, TrainTrackSpline.TrackOrientation.Same);
				continue;
			}
			TrainTrackSpline[] array4 = array;
			foreach (TrainTrackSpline otherSpline in array4)
			{
				Vector3 theirStartPos;
				Vector3 theirEndPos;
				Vector3 theirStartTangent;
				Vector3 theirEndTangent;
				if (!((Object)(object)ourSpline2 == (Object)(object)otherSpline))
				{
					theirStartPos = otherSpline.GetStartPointWorld();
					theirEndPos = otherSpline.GetEndPointWorld();
					theirStartTangent = otherSpline.GetStartTangentWorld();
					theirEndTangent = otherSpline.GetEndTangentWorld();
					if (!CompareNodes(ourStart: false, theirStart: true) && !CompareNodes(ourStart: false, theirStart: false) && !CompareNodes(ourStart: true, theirStart: true))
					{
						CompareNodes(ourStart: true, theirStart: false);
					}
				}
				bool CompareNodes(bool ourStart, bool theirStart)
				{
					//IL_000d: Unknown result type (might be due to invalid IL or missing references)
					//IL_0005: Unknown result type (might be due to invalid IL or missing references)
					//IL_0012: Unknown result type (might be due to invalid IL or missing references)
					//IL_001f: Unknown result type (might be due to invalid IL or missing references)
					//IL_0017: Unknown result type (might be due to invalid IL or missing references)
					//IL_0024: Unknown result type (might be due to invalid IL or missing references)
					//IL_0033: Unknown result type (might be due to invalid IL or missing references)
					//IL_002a: Unknown result type (might be due to invalid IL or missing references)
					//IL_0038: Unknown result type (might be due to invalid IL or missing references)
					//IL_0047: Unknown result type (might be due to invalid IL or missing references)
					//IL_003e: Unknown result type (might be due to invalid IL or missing references)
					//IL_004c: Unknown result type (might be due to invalid IL or missing references)
					//IL_0065: Unknown result type (might be due to invalid IL or missing references)
					//IL_0066: Unknown result type (might be due to invalid IL or missing references)
					//IL_0067: Unknown result type (might be due to invalid IL or missing references)
					//IL_0068: Unknown result type (might be due to invalid IL or missing references)
					//IL_0058: Unknown result type (might be due to invalid IL or missing references)
					//IL_005e: Unknown result type (might be due to invalid IL or missing references)
					//IL_0063: Unknown result type (might be due to invalid IL or missing references)
					Vector3 ourPos3 = (ourStart ? ourStartPos : ourEndPos);
					Vector3 ourTangent3 = (ourStart ? ourStartTangent : ourEndTangent);
					Vector3 theirPos2 = (theirStart ? theirStartPos : theirEndPos);
					Vector3 val = (theirStart ? theirStartTangent : theirEndTangent);
					if (ourStart == theirStart)
					{
						val *= -1f;
					}
					if (NodesConnect(ourPos3, theirPos2, ourTangent3, val))
					{
						if (ourStart)
						{
							ourSpline2.AddTrackConnection(otherSpline, TrainTrackSpline.TrackPosition.Prev, theirStart ? TrainTrackSpline.TrackOrientation.Reverse : TrainTrackSpline.TrackOrientation.Same);
						}
						else
						{
							ourSpline2.AddTrackConnection(otherSpline, TrainTrackSpline.TrackPosition.Next, (!theirStart) ? TrainTrackSpline.TrackOrientation.Reverse : TrainTrackSpline.TrackOrientation.Same);
						}
						if (theirStart)
						{
							otherSpline.AddTrackConnection(ourSpline2, TrainTrackSpline.TrackPosition.Prev, ourStart ? TrainTrackSpline.TrackOrientation.Reverse : TrainTrackSpline.TrackOrientation.Same);
						}
						else
						{
							otherSpline.AddTrackConnection(ourSpline2, TrainTrackSpline.TrackPosition.Next, (!ourStart) ? TrainTrackSpline.TrackOrientation.Reverse : TrainTrackSpline.TrackOrientation.Same);
						}
						return true;
					}
					return false;
				}
			}
		}
		static bool NodesConnect(Vector3 ourPos, Vector3 theirPos, Vector3 ourTangent, Vector3 theirTangent)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			return Vector3.SqrMagnitude(ourPos - theirPos) < 0.010000001f && Vector3.Angle(ourTangent, theirTangent) < 10f;
		}
	}
}
