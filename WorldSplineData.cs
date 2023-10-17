using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class WorldSplineData
{
	[Serializable]
	public class LUTEntry
	{
		[Serializable]
		public struct LUTPoint
		{
			public float distance;

			public Vector3 pos;

			public LUTPoint(float distance, Vector3 pos)
			{
				//IL_0009: Unknown result type (might be due to invalid IL or missing references)
				//IL_000a: Unknown result type (might be due to invalid IL or missing references)
				this.distance = distance;
				this.pos = pos;
			}
		}

		public List<LUTPoint> points = new List<LUTPoint>();
	}

	public Vector3[] inputPoints;

	public Vector3[] inputTangents;

	public float inputLUTInterval;

	public List<LUTEntry> LUTValues;

	public float Length;

	[SerializeField]
	private int maxPointsIndex;

	public WorldSplineData(WorldSpline worldSpline)
	{
		worldSpline.CheckValidity();
		LUTValues = new List<LUTEntry>();
		inputPoints = (Vector3[])(object)new Vector3[worldSpline.points.Length];
		worldSpline.points.CopyTo(inputPoints, 0);
		inputTangents = (Vector3[])(object)new Vector3[worldSpline.tangents.Length];
		worldSpline.tangents.CopyTo(inputTangents, 0);
		inputLUTInterval = worldSpline.lutInterval;
		maxPointsIndex = inputPoints.Length - 1;
		CreateLookupTable(worldSpline);
	}

	public bool IsSameAs(WorldSpline worldSpline)
	{
		return inputPoints.SequenceEqual(worldSpline.points) && inputTangents.SequenceEqual(worldSpline.tangents) && inputLUTInterval == worldSpline.lutInterval;
	}

	public bool IsDifferentTo(WorldSpline worldSpline)
	{
		return !IsSameAs(worldSpline);
	}

	public Vector3 GetStartPoint()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		return inputPoints[0];
	}

	public Vector3 GetEndPoint()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		return inputPoints[maxPointsIndex];
	}

	public Vector3 GetStartTangent()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		return inputTangents[0];
	}

	public Vector3 GetEndTangent()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		return inputTangents[maxPointsIndex];
	}

	public Vector3 GetPointCubicHermite(float distance)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		Vector3 tangent;
		return GetPointAndTangentCubicHermite(distance, out tangent);
	}

	public Vector3 GetTangentCubicHermite(float distance)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		GetPointAndTangentCubicHermite(distance, out var tangent);
		return tangent;
	}

	public Vector3 GetPointAndTangentCubicHermite(float distance, out Vector3 tangent)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		if (distance <= 0f)
		{
			tangent = GetStartTangent();
			return GetStartPoint();
		}
		if (distance >= Length)
		{
			tangent = GetEndTangent();
			return GetEndPoint();
		}
		int num = Mathf.FloorToInt(distance);
		if (LUTValues.Count > num)
		{
			int num2 = -1;
			while (num2 < 0 && (float)num > 0f)
			{
				LUTEntry lUTEntry = LUTValues[num];
				for (int i = 0; i < lUTEntry.points.Count && !(lUTEntry.points[i].distance > distance); i++)
				{
					num2 = i;
				}
				if (num2 < 0)
				{
					num--;
				}
			}
			float num3;
			Vector3 val;
			if (num2 < 0)
			{
				num3 = 0f;
				val = GetStartPoint();
			}
			else
			{
				LUTEntry.LUTPoint lUTPoint = LUTValues[num].points[num2];
				num3 = lUTPoint.distance;
				val = lUTPoint.pos;
			}
			num2 = -1;
			while (num2 < 0 && num < LUTValues.Count)
			{
				LUTEntry lUTEntry2 = LUTValues[num];
				for (int j = 0; j < lUTEntry2.points.Count; j++)
				{
					if (lUTEntry2.points[j].distance > distance)
					{
						num2 = j;
						break;
					}
				}
				if (num2 < 0)
				{
					num++;
				}
			}
			float num4;
			Vector3 val2;
			if (num2 < 0)
			{
				num4 = Length;
				val2 = GetEndPoint();
			}
			else
			{
				LUTEntry.LUTPoint lUTPoint2 = LUTValues[num].points[num2];
				num4 = lUTPoint2.distance;
				val2 = lUTPoint2.pos;
			}
			float num5 = Mathf.InverseLerp(num3, num4, distance);
			Vector3 val3 = val2 - val;
			tangent = ((Vector3)(ref val3)).normalized;
			return Vector3.Lerp(val, val2, num5);
		}
		tangent = GetEndTangent();
		return GetEndPoint();
	}

	public void SetDefaultTangents(WorldSpline worldSpline)
	{
		PathInterpolator pathInterpolator = new PathInterpolator(worldSpline.points, worldSpline.tangents);
		pathInterpolator.RecalculateTangents();
		worldSpline.tangents = pathInterpolator.Tangents;
	}

	public bool DetectSplineProblems(WorldSpline worldSpline)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		bool result = false;
		Vector3 val = GetTangentCubicHermite(0f);
		for (float num = 0.05f; num <= Length; num += 0.05f)
		{
			Vector3 tangentCubicHermite = GetTangentCubicHermite(num);
			float num2 = Vector3.Angle(tangentCubicHermite, val);
			if (num2 > 5f)
			{
				if ((Object)(object)worldSpline != (Object)null)
				{
					Vector3 tangent;
					Vector3 pointAndTangentCubicHermiteWorld = worldSpline.GetPointAndTangentCubicHermiteWorld(num, out tangent);
					Debug.DrawRay(pointAndTangentCubicHermiteWorld, tangent, Color.red, 30f);
					Debug.DrawRay(pointAndTangentCubicHermiteWorld, Vector3.up, Color.red, 30f);
				}
				Debug.Log((object)($"Spline may have a too-sharp bend at {num / Length:P0}. Angle change: " + num2));
				result = true;
			}
			val = tangentCubicHermite;
		}
		return result;
	}

	private void CreateLookupTable(WorldSpline worldSpline)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		PathInterpolator pathInterpolator = new PathInterpolator(worldSpline.points, worldSpline.tangents);
		Vector3 val = pathInterpolator.GetPointCubicHermite(0f);
		Length = 0f;
		AddEntry(0f, GetStartPoint());
		Vector3 pointCubicHermite;
		for (float num = worldSpline.lutInterval; num < pathInterpolator.Length; num += worldSpline.lutInterval)
		{
			pointCubicHermite = pathInterpolator.GetPointCubicHermite(num);
			Length += Vector3.Distance(pointCubicHermite, val);
			AddEntry(Length, pathInterpolator.GetPointCubicHermite(num));
			val = pointCubicHermite;
		}
		pointCubicHermite = GetEndPoint();
		Length += Vector3.Distance(pointCubicHermite, val);
		AddEntry(Length, pointCubicHermite);
	}

	private void AddEntry(float distance, Vector3 pos)
	{
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		int num = Mathf.FloorToInt(distance);
		if (LUTValues.Count < num + 1)
		{
			for (int i = LUTValues.Count; i < num + 1; i++)
			{
				LUTValues.Add(new LUTEntry());
			}
		}
		LUTValues[num].points.Add(new LUTEntry.LUTPoint(distance, pos));
	}
}
