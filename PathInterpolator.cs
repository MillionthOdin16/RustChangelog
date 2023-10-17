using System;
using System.Collections.Generic;
using UnityEngine;

public class PathInterpolator
{
	public Vector3[] Points;

	public Vector3[] Tangents;

	protected bool initialized = false;

	public int MinIndex { get; set; }

	public int MaxIndex { get; set; }

	public virtual float Length { get; private set; }

	public virtual float StepSize { get; private set; }

	public bool Circular { get; private set; }

	public int DefaultMinIndex => 0;

	public int DefaultMaxIndex => Points.Length - 1;

	public float StartOffset => Length * (float)(MinIndex - DefaultMinIndex) / (float)(DefaultMaxIndex - DefaultMinIndex);

	public float EndOffset => Length * (float)(DefaultMaxIndex - MaxIndex) / (float)(DefaultMaxIndex - DefaultMinIndex);

	public PathInterpolator(Vector3[] points)
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		if (points.Length < 2)
		{
			throw new ArgumentException("Point list too short.");
		}
		Points = points;
		MinIndex = DefaultMinIndex;
		MaxIndex = DefaultMaxIndex;
		Circular = Vector3.Distance(points[0], points[points.Length - 1]) < 0.1f;
	}

	public PathInterpolator(Vector3[] points, Vector3[] tangents)
		: this(points)
	{
		if (tangents.Length != points.Length)
		{
			throw new ArgumentException("Points and tangents lengths must match. Points: " + points.Length + " Tangents: " + tangents.Length);
		}
		Tangents = tangents;
		RecalculateLength();
		initialized = true;
	}

	public void RecalculateTangents()
	{
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		if (Tangents == null || Tangents.Length != Points.Length)
		{
			Tangents = (Vector3[])(object)new Vector3[Points.Length];
		}
		for (int i = 0; i < Points.Length; i++)
		{
			int num = i - 1;
			int num2 = i + 1;
			if (num < 0)
			{
				num = (Circular ? (Points.Length - 2) : 0);
			}
			if (num2 > Points.Length - 1)
			{
				num2 = (Circular ? 1 : (Points.Length - 1));
			}
			Vector3 val = Points[num];
			Vector3 val2 = Points[num2];
			Vector3[] tangents = Tangents;
			int num3 = i;
			Vector3 val3 = val2 - val;
			tangents[num3] = ((Vector3)(ref val3)).normalized;
		}
		RecalculateLength();
		initialized = true;
	}

	public void RecalculateLength()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		float num = 0f;
		for (int i = 0; i < Points.Length - 1; i++)
		{
			Vector3 val = Points[i];
			Vector3 val2 = Points[i + 1];
			float num2 = num;
			Vector3 val3 = val2 - val;
			num = num2 + ((Vector3)(ref val3)).magnitude;
		}
		Length = num;
		StepSize = num / (float)Points.Length;
	}

	public void Resample(float distance)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		float num = 0f;
		Vector3 val3;
		for (int i = 0; i < Points.Length - 1; i++)
		{
			Vector3 val = Points[i];
			Vector3 val2 = Points[i + 1];
			float num2 = num;
			val3 = val2 - val;
			num = num2 + ((Vector3)(ref val3)).magnitude;
		}
		int num3 = Mathf.RoundToInt(num / distance);
		if (num3 < 2)
		{
			return;
		}
		distance = num / (float)(num3 - 1);
		List<Vector3> list = new List<Vector3>(num3);
		float num4 = 0f;
		for (int j = 0; j < Points.Length - 1; j++)
		{
			int num5 = j;
			int num6 = j + 1;
			Vector3 val4 = Points[num5];
			Vector3 val5 = Points[num6];
			val3 = val5 - val4;
			float num7 = ((Vector3)(ref val3)).magnitude;
			if (num5 == 0)
			{
				list.Add(val4);
			}
			while (num4 + num7 > distance)
			{
				float num8 = distance - num4;
				float num9 = num8 / num7;
				Vector3 val6 = Vector3.Lerp(val4, val5, num9);
				list.Add(val6);
				val4 = val6;
				num4 = 0f;
				num7 -= num8;
			}
			num4 += num7;
			if (num6 == Points.Length - 1 && num4 > distance * 0.5f)
			{
				list.Add(val5);
			}
		}
		if (list.Count >= 2)
		{
			Points = list.ToArray();
			MinIndex = DefaultMinIndex;
			MaxIndex = DefaultMaxIndex;
			initialized = false;
		}
	}

	public void Smoothen(int iterations, Func<int, float> filter = null)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		Smoothen(iterations, Vector3.one, filter);
	}

	public void Smoothen(int iterations, Vector3 multipliers, Func<int, float> filter = null)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < iterations; i++)
		{
			for (int j = MinIndex + ((!Circular) ? 1 : 0); j <= MaxIndex - 1; j += 2)
			{
				SmoothenIndex(j, multipliers, filter);
			}
			for (int k = MinIndex + (Circular ? 1 : 2); k <= MaxIndex - 1; k += 2)
			{
				SmoothenIndex(k, multipliers, filter);
			}
		}
		initialized = false;
	}

	private void SmoothenIndex(int i, Vector3 multipliers, Func<int, float> filter = null)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		int num = i - 1;
		int num2 = i + 1;
		if (i == 0)
		{
			num = Points.Length - 2;
		}
		Vector3 val = Points[num];
		Vector3 val2 = Points[i];
		Vector3 val3 = Points[num2];
		Vector3 val4 = (val + val2 + val2 + val3) * 0.25f;
		if (filter != null)
		{
			multipliers *= filter(i);
		}
		if (multipliers != Vector3.one)
		{
			val4.x = Mathf.LerpUnclamped(val2.x, val4.x, multipliers.x);
			val4.y = Mathf.LerpUnclamped(val2.y, val4.y, multipliers.y);
			val4.z = Mathf.LerpUnclamped(val2.z, val4.z, multipliers.z);
		}
		Points[i] = val4;
		if (i == 0)
		{
			Points[Points.Length - 1] = Points[0];
		}
	}

	public Vector3 GetStartPoint()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		return Points[MinIndex];
	}

	public Vector3 GetEndPoint()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		return Points[MaxIndex];
	}

	public Vector3 GetStartTangent()
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		if (!initialized)
		{
			throw new Exception("Tangents have not been calculated yet or are outdated.");
		}
		return Tangents[MinIndex];
	}

	public Vector3 GetEndTangent()
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		if (!initialized)
		{
			throw new Exception("Tangents have not been calculated yet or are outdated.");
		}
		return Tangents[MaxIndex];
	}

	public Vector3 GetPoint(float distance)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		if (Length == 0f)
		{
			return GetStartPoint();
		}
		float num = distance / Length * (float)(Points.Length - 1);
		int num2 = (int)num;
		if (num <= (float)MinIndex)
		{
			return GetStartPoint();
		}
		if (num >= (float)MaxIndex)
		{
			return GetEndPoint();
		}
		Vector3 val = Points[num2];
		Vector3 val2 = Points[num2 + 1];
		float num3 = num - (float)num2;
		return Vector3.Lerp(val, val2, num3);
	}

	public virtual Vector3 GetTangent(float distance)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		if (!initialized)
		{
			throw new Exception("Tangents have not been calculated yet or are outdated.");
		}
		if (Length == 0f)
		{
			return GetStartPoint();
		}
		float num = distance / Length * (float)(Tangents.Length - 1);
		int num2 = (int)num;
		if (num <= (float)MinIndex)
		{
			return GetStartTangent();
		}
		if (num >= (float)MaxIndex)
		{
			return GetEndTangent();
		}
		Vector3 val = Tangents[num2];
		Vector3 val2 = Tangents[num2 + 1];
		float num3 = num - (float)num2;
		return Vector3.Slerp(val, val2, num3);
	}

	public virtual Vector3 GetPointCubicHermite(float distance)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		if (!initialized)
		{
			throw new Exception("Tangents have not been calculated yet or are outdated.");
		}
		if (Length == 0f)
		{
			return GetStartPoint();
		}
		float num = distance / Length * (float)(Points.Length - 1);
		int num2 = (int)num;
		if (num <= (float)MinIndex)
		{
			return GetStartPoint();
		}
		if (num >= (float)MaxIndex)
		{
			return GetEndPoint();
		}
		Vector3 val = Points[num2];
		Vector3 val2 = Points[num2 + 1];
		Vector3 val3 = Tangents[num2] * StepSize;
		Vector3 val4 = Tangents[num2 + 1] * StepSize;
		float num3 = num - (float)num2;
		float num4 = num3 * num3;
		float num5 = num3 * num4;
		return (2f * num5 - 3f * num4 + 1f) * val + (num5 - 2f * num4 + num3) * val3 + (-2f * num5 + 3f * num4) * val2 + (num5 - num4) * val4;
	}
}
