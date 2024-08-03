using System.Collections.Generic;
using UnityEngine;

public class WorldSpline : MonoBehaviour
{
	public int dataIndex = -1;

	public Vector3[] points;

	public Vector3[] tangents;

	[Range(0.05f, 100f)]
	public float lutInterval = 0.25f;

	[SerializeField]
	private bool showGizmos = true;

	private static List<Vector3> visualSplineList = new List<Vector3>();

	private WorldSplineData privateData;

	public WorldSplineData GetData()
	{
		if (WorldSplineSharedData.TryGetDataFor(this, out var data))
		{
			return data;
		}
		if (Application.isPlaying && privateData == null)
		{
			privateData = new WorldSplineData(this);
		}
		return privateData;
	}

	public void SetAll(Vector3[] points, Vector3[] tangents, float lutInterval)
	{
		this.points = points;
		this.tangents = tangents;
		this.lutInterval = lutInterval;
	}

	public void CheckValidity()
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		lutInterval = Mathf.Clamp(lutInterval, 0.05f, 100f);
		if (points == null || points.Length < 2)
		{
			points = (Vector3[])(object)new Vector3[2];
			points[0] = Vector3.zero;
			points[1] = Vector3.zero;
		}
		if (tangents != null && points.Length == tangents.Length)
		{
			return;
		}
		Vector3[] array = (Vector3[])(object)new Vector3[points.Length];
		for (int i = 0; i < array.Length; i++)
		{
			if (tangents != null && i < tangents.Length)
			{
				array[i] = tangents[i];
			}
			else
			{
				array[i] = Vector3.forward;
			}
		}
		tangents = array;
	}

	protected virtual void OnDrawGizmosSelected()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		if (showGizmos)
		{
			DrawSplineGizmo(this, Color.magenta);
		}
	}

	protected static void DrawSplineGizmo(WorldSpline ws, Color splineColour)
	{
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)ws == (Object)null)
		{
			return;
		}
		WorldSplineData data = ws.GetData();
		if (data == null || ws.points.Length < 2 || ws.points.Length != ws.tangents.Length)
		{
			return;
		}
		Vector3[] pointsWorld = ws.GetPointsWorld();
		Vector3[] tangentsWorld = ws.GetTangentsWorld();
		for (int i = 0; i < pointsWorld.Length; i++)
		{
			Gizmos.color = Color.magenta;
			Gizmos.DrawSphere(pointsWorld[i], 0.25f);
			if (((Vector3)(ref tangentsWorld[i])).magnitude > 0f)
			{
				Gizmos.color = Color.cyan;
				Vector3 val = pointsWorld[i] + tangentsWorld[i] + Vector3.up * 0.1f;
				Gizmos.DrawLine(pointsWorld[i] + Vector3.up * 0.1f, val);
			}
		}
		Gizmos.color = splineColour;
		Vector3[] visualSpline = GetVisualSpline(ws, data, 1f);
		for (int j = 0; j < visualSpline.Length - 1; j++)
		{
			Gizmos.color = Color.Lerp(Color.white, splineColour, (float)j / (float)(visualSpline.Length - 1));
			Gizmos.DrawLine(visualSpline[j], visualSpline[j + 1]);
			Gizmos.DrawLine(visualSpline[j], visualSpline[j] + Vector3.up * 0.25f);
		}
	}

	private static Vector3[] GetVisualSpline(WorldSpline ws, WorldSplineData data, float distBetweenPoints)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		visualSplineList.Clear();
		if ((Object)(object)ws != (Object)null && ws.points.Length > 1)
		{
			Vector3 startPointWorld = ws.GetStartPointWorld();
			Vector3 endPointWorld = ws.GetEndPointWorld();
			visualSplineList.Add(startPointWorld);
			for (float num = distBetweenPoints; num <= data.Length - distBetweenPoints; num += distBetweenPoints)
			{
				visualSplineList.Add(ws.GetPointCubicHermiteWorld(num, data));
			}
			visualSplineList.Add(endPointWorld);
		}
		return visualSplineList.ToArray();
	}

	public Vector3 GetStartPointWorld()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		return ((Component)this).transform.TransformPoint(points[0]);
	}

	public Vector3 GetEndPointWorld()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		return ((Component)this).transform.TransformPoint(points[points.Length - 1]);
	}

	public Vector3 GetStartTangentWorld()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		return Vector3.Scale(((Component)this).transform.rotation * tangents[0], ((Component)this).transform.localScale);
	}

	public Vector3 GetEndTangentWorld()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		return Vector3.Scale(((Component)this).transform.rotation * tangents[tangents.Length - 1], ((Component)this).transform.localScale);
	}

	public Vector3 GetTangentCubicHermiteWorld(float distance)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		return Vector3.Scale(((Component)this).transform.rotation * GetData().GetTangentCubicHermite(distance), ((Component)this).transform.localScale);
	}

	public Vector3 GetTangentCubicHermiteWorld(float distance, WorldSplineData data)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		return Vector3.Scale(((Component)this).transform.rotation * data.GetTangentCubicHermite(distance), ((Component)this).transform.localScale);
	}

	public Vector3 GetPointCubicHermiteWorld(float distance)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		return ((Component)this).transform.TransformPoint(GetData().GetPointCubicHermite(distance));
	}

	public Vector3 GetPointCubicHermiteWorld(float distance, WorldSplineData data)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		return ((Component)this).transform.TransformPoint(data.GetPointCubicHermite(distance));
	}

	public Vector3 GetPointAndTangentCubicHermiteWorld(float distance, out Vector3 tangent)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		Vector3 pointAndTangentCubicHermite = GetData().GetPointAndTangentCubicHermite(distance, out tangent);
		tangent = ((Component)this).transform.TransformVector(tangent);
		return ((Component)this).transform.TransformPoint(pointAndTangentCubicHermite);
	}

	public Vector3 GetPointAndTangentCubicHermiteWorld(float distance, WorldSplineData data, out Vector3 tangent)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		Vector3 pointAndTangentCubicHermite = data.GetPointAndTangentCubicHermite(distance, out tangent);
		tangent = ((Component)this).transform.TransformVector(tangent);
		return ((Component)this).transform.TransformPoint(pointAndTangentCubicHermite);
	}

	public Vector3[] GetPointsWorld()
	{
		return PointsToWorld(points, ((Component)this).transform);
	}

	public Vector3[] GetTangentsWorld()
	{
		return TangentsToWorld(tangents, ((Component)this).transform);
	}

	private static Vector3[] PointsToWorld(Vector3[] points, Transform tr)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		Vector3[] array = (Vector3[])(object)new Vector3[points.Length];
		for (int i = 0; i < points.Length; i++)
		{
			array[i] = tr.TransformPoint(points[i]);
		}
		return array;
	}

	private static Vector3[] TangentsToWorld(Vector3[] tangents, Transform tr)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		Vector3[] array = (Vector3[])(object)new Vector3[tangents.Length];
		for (int i = 0; i < tangents.Length; i++)
		{
			array[i] = Vector3.Scale(tr.rotation * tangents[i], tr.localScale);
		}
		return array;
	}
}
