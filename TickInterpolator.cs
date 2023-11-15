using System.Collections.Generic;
using UnityEngine;

public class TickInterpolator
{
	private struct Segment
	{
		public Vector3 point;

		public float length;

		public Segment(Vector3 a, Vector3 b)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			point = b;
			length = Vector3.Distance(a, b);
		}
	}

	private List<Segment> points = new List<Segment>();

	private int index = 0;

	public float Length;

	public Vector3 CurrentPoint;

	public Vector3 StartPoint;

	public Vector3 EndPoint;

	public int Count => points.Count;

	public void Reset()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		index = 0;
		CurrentPoint = StartPoint;
	}

	public void Reset(Vector3 point)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		points.Clear();
		index = 0;
		Length = 0f;
		CurrentPoint = (StartPoint = (EndPoint = point));
	}

	public void AddPoint(Vector3 point)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		Segment item = new Segment(EndPoint, point);
		points.Add(item);
		Length += item.length;
		EndPoint = item.point;
	}

	public bool MoveNext(float distance)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		float num = 0f;
		while (num < distance && index < points.Count)
		{
			Segment segment = points[index];
			CurrentPoint = segment.point;
			num += segment.length;
			index++;
		}
		return num > 0f;
	}

	public bool HasNext()
	{
		return index < points.Count;
	}

	public void TransformEntries(Matrix4x4 matrix)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < points.Count; i++)
		{
			Segment value = points[i];
			value.point = ((Matrix4x4)(ref matrix)).MultiplyPoint3x4(value.point);
			points[i] = value;
		}
		CurrentPoint = ((Matrix4x4)(ref matrix)).MultiplyPoint3x4(CurrentPoint);
		StartPoint = ((Matrix4x4)(ref matrix)).MultiplyPoint3x4(StartPoint);
		EndPoint = ((Matrix4x4)(ref matrix)).MultiplyPoint3x4(EndPoint);
	}
}
