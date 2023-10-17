using System.Collections.Generic;

namespace UnityEngine.UI.Extensions;

public class BezierPath
{
	public int SegmentsPerCurve = 10;

	public float MINIMUM_SQR_DISTANCE = 0.01f;

	public float DIVISION_THRESHOLD = -0.99f;

	private List<Vector2> controlPoints;

	private int curveCount;

	public BezierPath()
	{
		controlPoints = new List<Vector2>();
	}

	public void SetControlPoints(List<Vector2> newControlPoints)
	{
		controlPoints.Clear();
		controlPoints.AddRange(newControlPoints);
		curveCount = (controlPoints.Count - 1) / 3;
	}

	public void SetControlPoints(Vector2[] newControlPoints)
	{
		controlPoints.Clear();
		controlPoints.AddRange(newControlPoints);
		curveCount = (controlPoints.Count - 1) / 3;
	}

	public List<Vector2> GetControlPoints()
	{
		return controlPoints;
	}

	public void Interpolate(List<Vector2> segmentPoints, float scale)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		controlPoints.Clear();
		if (segmentPoints.Count < 2)
		{
			return;
		}
		for (int i = 0; i < segmentPoints.Count; i++)
		{
			if (i == 0)
			{
				Vector2 val = segmentPoints[i];
				Vector2 val2 = segmentPoints[i + 1];
				Vector2 val3 = val2 - val;
				Vector2 item = val + scale * val3;
				controlPoints.Add(val);
				controlPoints.Add(item);
				continue;
			}
			if (i == segmentPoints.Count - 1)
			{
				Vector2 val4 = segmentPoints[i - 1];
				Vector2 val5 = segmentPoints[i];
				Vector2 val6 = val5 - val4;
				Vector2 item2 = val5 - scale * val6;
				controlPoints.Add(item2);
				controlPoints.Add(val5);
				continue;
			}
			Vector2 val7 = segmentPoints[i - 1];
			Vector2 val8 = segmentPoints[i];
			Vector2 val9 = segmentPoints[i + 1];
			Vector2 val10 = val9 - val7;
			Vector2 normalized = ((Vector2)(ref val10)).normalized;
			Vector2 val11 = scale * normalized;
			val10 = val8 - val7;
			Vector2 item3 = val8 - val11 * ((Vector2)(ref val10)).magnitude;
			Vector2 val12 = scale * normalized;
			val10 = val9 - val8;
			Vector2 item4 = val8 + val12 * ((Vector2)(ref val10)).magnitude;
			controlPoints.Add(item3);
			controlPoints.Add(val8);
			controlPoints.Add(item4);
		}
		curveCount = (controlPoints.Count - 1) / 3;
	}

	public void SamplePoints(List<Vector2> sourcePoints, float minSqrDistance, float maxSqrDistance, float scale)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		if (sourcePoints.Count < 2)
		{
			return;
		}
		Stack<Vector2> stack = new Stack<Vector2>();
		stack.Push(sourcePoints[0]);
		Vector2 val = sourcePoints[1];
		int num = 2;
		Vector2 val2;
		for (num = 2; num < sourcePoints.Count; num++)
		{
			val2 = val - sourcePoints[num];
			if (((Vector2)(ref val2)).sqrMagnitude > minSqrDistance)
			{
				val2 = stack.Peek() - sourcePoints[num];
				if (((Vector2)(ref val2)).sqrMagnitude > maxSqrDistance)
				{
					stack.Push(val);
				}
			}
			val = sourcePoints[num];
		}
		Vector2 val3 = stack.Pop();
		Vector2 val4 = stack.Peek();
		val2 = val4 - val;
		Vector2 normalized = ((Vector2)(ref val2)).normalized;
		val2 = val - val3;
		float magnitude = ((Vector2)(ref val2)).magnitude;
		val2 = val3 - val4;
		float magnitude2 = ((Vector2)(ref val2)).magnitude;
		val3 += normalized * ((magnitude2 - magnitude) / 2f);
		stack.Push(val3);
		stack.Push(val);
		Interpolate(new List<Vector2>(stack), scale);
	}

	public Vector2 CalculateBezierPoint(int curveIndex, float t)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		int num = curveIndex * 3;
		Vector2 p = controlPoints[num];
		Vector2 p2 = controlPoints[num + 1];
		Vector2 p3 = controlPoints[num + 2];
		Vector2 p4 = controlPoints[num + 3];
		return CalculateBezierPoint(t, p, p2, p3, p4);
	}

	public List<Vector2> GetDrawingPoints0()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		List<Vector2> list = new List<Vector2>();
		for (int i = 0; i < curveCount; i++)
		{
			if (i == 0)
			{
				list.Add(CalculateBezierPoint(i, 0f));
			}
			for (int j = 1; j <= SegmentsPerCurve; j++)
			{
				float t = (float)j / (float)SegmentsPerCurve;
				list.Add(CalculateBezierPoint(i, t));
			}
		}
		return list;
	}

	public List<Vector2> GetDrawingPoints1()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		List<Vector2> list = new List<Vector2>();
		for (int i = 0; i < controlPoints.Count - 3; i += 3)
		{
			Vector2 p = controlPoints[i];
			Vector2 p2 = controlPoints[i + 1];
			Vector2 p3 = controlPoints[i + 2];
			Vector2 p4 = controlPoints[i + 3];
			if (i == 0)
			{
				list.Add(CalculateBezierPoint(0f, p, p2, p3, p4));
			}
			for (int j = 1; j <= SegmentsPerCurve; j++)
			{
				float t = (float)j / (float)SegmentsPerCurve;
				list.Add(CalculateBezierPoint(t, p, p2, p3, p4));
			}
		}
		return list;
	}

	public List<Vector2> GetDrawingPoints2()
	{
		List<Vector2> list = new List<Vector2>();
		for (int i = 0; i < curveCount; i++)
		{
			List<Vector2> list2 = FindDrawingPoints(i);
			if (i != 0)
			{
				list2.RemoveAt(0);
			}
			list.AddRange(list2);
		}
		return list;
	}

	private List<Vector2> FindDrawingPoints(int curveIndex)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		List<Vector2> list = new List<Vector2>();
		Vector2 item = CalculateBezierPoint(curveIndex, 0f);
		Vector2 item2 = CalculateBezierPoint(curveIndex, 1f);
		list.Add(item);
		list.Add(item2);
		FindDrawingPoints(curveIndex, 0f, 1f, list, 1);
		return list;
	}

	private int FindDrawingPoints(int curveIndex, float t0, float t1, List<Vector2> pointList, int insertionIndex)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = CalculateBezierPoint(curveIndex, t0);
		Vector2 val2 = CalculateBezierPoint(curveIndex, t1);
		Vector2 val3 = val - val2;
		if (((Vector2)(ref val3)).sqrMagnitude < MINIMUM_SQR_DISTANCE)
		{
			return 0;
		}
		float num = (t0 + t1) / 2f;
		Vector2 val4 = CalculateBezierPoint(curveIndex, num);
		val3 = val - val4;
		Vector2 normalized = ((Vector2)(ref val3)).normalized;
		val3 = val2 - val4;
		Vector2 normalized2 = ((Vector2)(ref val3)).normalized;
		if (Vector2.Dot(normalized, normalized2) > DIVISION_THRESHOLD || Mathf.Abs(num - 0.5f) < 0.0001f)
		{
			int num2 = 0;
			num2 += FindDrawingPoints(curveIndex, t0, num, pointList, insertionIndex);
			pointList.Insert(insertionIndex + num2, val4);
			num2++;
			return num2 + FindDrawingPoints(curveIndex, num, t1, pointList, insertionIndex + num2);
		}
		return 0;
	}

	private Vector2 CalculateBezierPoint(float t, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		float num = 1f - t;
		float num2 = t * t;
		float num3 = num * num;
		float num4 = num3 * num;
		float num5 = num2 * t;
		Vector2 val = num4 * p0;
		val += 3f * num3 * t * p1;
		val += 3f * num * num2 * p2;
		return val + num5 * p3;
	}
}
