using System;
using UnityEngine;

public static class BoundsEx
{
	private static Vector3[] pts = (Vector3[])(object)new Vector3[8];

	public static Bounds XZ3D(this Bounds bounds)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		return new Bounds(Vector3Ex.XZ3D(((Bounds)(ref bounds)).center), Vector3Ex.XZ3D(((Bounds)(ref bounds)).size));
	}

	public static Bounds Transform(this Bounds bounds, Matrix4x4 matrix)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		Vector3 center = ((Matrix4x4)(ref matrix)).MultiplyPoint3x4(((Bounds)(ref bounds)).center);
		Vector3 extents = ((Bounds)(ref bounds)).extents;
		Vector3 val = ((Matrix4x4)(ref matrix)).MultiplyVector(new Vector3(extents.x, 0f, 0f));
		Vector3 val2 = ((Matrix4x4)(ref matrix)).MultiplyVector(new Vector3(0f, extents.y, 0f));
		Vector3 val3 = ((Matrix4x4)(ref matrix)).MultiplyVector(new Vector3(0f, 0f, extents.z));
		extents.x = Mathf.Abs(val.x) + Mathf.Abs(val2.x) + Mathf.Abs(val3.x);
		extents.y = Mathf.Abs(val.y) + Mathf.Abs(val2.y) + Mathf.Abs(val3.y);
		extents.z = Mathf.Abs(val.z) + Mathf.Abs(val2.z) + Mathf.Abs(val3.z);
		Bounds result = default(Bounds);
		((Bounds)(ref result)).center = center;
		((Bounds)(ref result)).extents = extents;
		return result;
	}

	public static Rect ToScreenRect(this Bounds b, Camera cam)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0307: Unknown result type (might be due to invalid IL or missing references)
		//IL_030c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0311: Unknown result type (might be due to invalid IL or missing references)
		//IL_031c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0321: Unknown result type (might be due to invalid IL or missing references)
		//IL_0328: Unknown result type (might be due to invalid IL or missing references)
		//IL_032d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0333: Unknown result type (might be due to invalid IL or missing references)
		//IL_033a: Unknown result type (might be due to invalid IL or missing references)
		//IL_033f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0344: Unknown result type (might be due to invalid IL or missing references)
		//IL_0345: Unknown result type (might be due to invalid IL or missing references)
		//IL_034c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0351: Unknown result type (might be due to invalid IL or missing references)
		//IL_0356: Unknown result type (might be due to invalid IL or missing references)
		//IL_036c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0372: Unknown result type (might be due to invalid IL or missing references)
		//IL_0378: Unknown result type (might be due to invalid IL or missing references)
		//IL_037e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0384: Unknown result type (might be due to invalid IL or missing references)
		//IL_0389: Unknown result type (might be due to invalid IL or missing references)
		//IL_0398: Unknown result type (might be due to invalid IL or missing references)
		TimeWarning val = TimeWarning.New("Bounds.ToScreenRect", 0);
		try
		{
			pts[0] = cam.WorldToScreenPoint(new Vector3(((Bounds)(ref b)).center.x + ((Bounds)(ref b)).extents.x, ((Bounds)(ref b)).center.y + ((Bounds)(ref b)).extents.y, ((Bounds)(ref b)).center.z + ((Bounds)(ref b)).extents.z));
			pts[1] = cam.WorldToScreenPoint(new Vector3(((Bounds)(ref b)).center.x + ((Bounds)(ref b)).extents.x, ((Bounds)(ref b)).center.y + ((Bounds)(ref b)).extents.y, ((Bounds)(ref b)).center.z - ((Bounds)(ref b)).extents.z));
			pts[2] = cam.WorldToScreenPoint(new Vector3(((Bounds)(ref b)).center.x + ((Bounds)(ref b)).extents.x, ((Bounds)(ref b)).center.y - ((Bounds)(ref b)).extents.y, ((Bounds)(ref b)).center.z + ((Bounds)(ref b)).extents.z));
			pts[3] = cam.WorldToScreenPoint(new Vector3(((Bounds)(ref b)).center.x + ((Bounds)(ref b)).extents.x, ((Bounds)(ref b)).center.y - ((Bounds)(ref b)).extents.y, ((Bounds)(ref b)).center.z - ((Bounds)(ref b)).extents.z));
			pts[4] = cam.WorldToScreenPoint(new Vector3(((Bounds)(ref b)).center.x - ((Bounds)(ref b)).extents.x, ((Bounds)(ref b)).center.y + ((Bounds)(ref b)).extents.y, ((Bounds)(ref b)).center.z + ((Bounds)(ref b)).extents.z));
			pts[5] = cam.WorldToScreenPoint(new Vector3(((Bounds)(ref b)).center.x - ((Bounds)(ref b)).extents.x, ((Bounds)(ref b)).center.y + ((Bounds)(ref b)).extents.y, ((Bounds)(ref b)).center.z - ((Bounds)(ref b)).extents.z));
			pts[6] = cam.WorldToScreenPoint(new Vector3(((Bounds)(ref b)).center.x - ((Bounds)(ref b)).extents.x, ((Bounds)(ref b)).center.y - ((Bounds)(ref b)).extents.y, ((Bounds)(ref b)).center.z + ((Bounds)(ref b)).extents.z));
			pts[7] = cam.WorldToScreenPoint(new Vector3(((Bounds)(ref b)).center.x - ((Bounds)(ref b)).extents.x, ((Bounds)(ref b)).center.y - ((Bounds)(ref b)).extents.y, ((Bounds)(ref b)).center.z - ((Bounds)(ref b)).extents.z));
			Vector3 val2 = pts[0];
			Vector3 val3 = pts[0];
			for (int i = 1; i < pts.Length; i++)
			{
				val2 = Vector3.Min(val2, pts[i]);
				val3 = Vector3.Max(val3, pts[i]);
			}
			return Rect.MinMaxRect(val2.x, val2.y, val3.x, val3.y);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public static Rect ToCanvasRect(this Bounds b, RectTransform target, Camera cam)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		Rect result = b.ToScreenRect(cam);
		((Rect)(ref result)).min = Vector2Ex.ToCanvas(((Rect)(ref result)).min, target, (Camera)null);
		((Rect)(ref result)).max = Vector2Ex.ToCanvas(((Rect)(ref result)).max, target, (Camera)null);
		return result;
	}
}
