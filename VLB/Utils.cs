using System;
using UnityEngine;

namespace VLB;

public static class Utils
{
	public enum FloatPackingPrecision
	{
		High = 64,
		Low = 8,
		Undef = 0
	}

	private static FloatPackingPrecision ms_FloatPackingPrecision;

	private const int kFloatPackingHighMinShaderLevel = 35;

	public static string GetPath(Transform current)
	{
		if ((Object)(object)current.parent == (Object)null)
		{
			return "/" + ((Object)current).name;
		}
		return GetPath(current.parent) + "/" + ((Object)current).name;
	}

	public static T NewWithComponent<T>(string name) where T : Component
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		return new GameObject(name, new Type[1] { typeof(T) }).GetComponent<T>();
	}

	public static T GetOrAddComponent<T>(this GameObject self) where T : Component
	{
		T val = self.GetComponent<T>();
		if ((Object)(object)val == (Object)null)
		{
			val = self.AddComponent<T>();
		}
		return val;
	}

	public static T GetOrAddComponent<T>(this MonoBehaviour self) where T : Component
	{
		return ((Component)self).gameObject.GetOrAddComponent<T>();
	}

	public static bool HasFlag(this Enum mask, Enum flags)
	{
		return ((int)(object)mask & (int)(object)flags) == (int)(object)flags;
	}

	public static Vector2 xy(this Vector3 aVector)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		return new Vector2(aVector.x, aVector.y);
	}

	public static Vector2 xz(this Vector3 aVector)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		return new Vector2(aVector.x, aVector.z);
	}

	public static Vector2 yz(this Vector3 aVector)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		return new Vector2(aVector.y, aVector.z);
	}

	public static Vector2 yx(this Vector3 aVector)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		return new Vector2(aVector.y, aVector.x);
	}

	public static Vector2 zx(this Vector3 aVector)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		return new Vector2(aVector.z, aVector.x);
	}

	public static Vector2 zy(this Vector3 aVector)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		return new Vector2(aVector.z, aVector.y);
	}

	public static float GetVolumeCubic(this Bounds self)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		return ((Bounds)(ref self)).size.x * ((Bounds)(ref self)).size.y * ((Bounds)(ref self)).size.z;
	}

	public static float GetMaxArea2D(this Bounds self)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		return Mathf.Max(Mathf.Max(((Bounds)(ref self)).size.x * ((Bounds)(ref self)).size.y, ((Bounds)(ref self)).size.y * ((Bounds)(ref self)).size.z), ((Bounds)(ref self)).size.x * ((Bounds)(ref self)).size.z);
	}

	public static Color Opaque(this Color self)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		return new Color(self.r, self.g, self.b, 1f);
	}

	public static void GizmosDrawPlane(Vector3 normal, Vector3 position, Color color, float size = 1f)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = Vector3.Cross(normal, (Mathf.Abs(Vector3.Dot(normal, Vector3.forward)) < 0.999f) ? Vector3.forward : Vector3.up);
		Vector3 val2 = ((Vector3)(ref val)).normalized * size;
		Vector3 val3 = position + val2;
		Vector3 val4 = position - val2;
		val2 = Quaternion.AngleAxis(90f, normal) * val2;
		Vector3 val5 = position + val2;
		Vector3 val6 = position - val2;
		Gizmos.matrix = Matrix4x4.identity;
		Gizmos.color = color;
		Gizmos.DrawLine(val3, val4);
		Gizmos.DrawLine(val5, val6);
		Gizmos.DrawLine(val3, val5);
		Gizmos.DrawLine(val5, val4);
		Gizmos.DrawLine(val4, val6);
		Gizmos.DrawLine(val6, val3);
	}

	public static Plane TranslateCustom(this Plane plane, Vector3 translation)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		((Plane)(ref plane)).distance = ((Plane)(ref plane)).distance + Vector3.Dot(((Vector3)(ref translation)).normalized, ((Plane)(ref plane)).normal) * ((Vector3)(ref translation)).magnitude;
		return plane;
	}

	public static bool IsValid(this Plane plane)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		Vector3 normal = ((Plane)(ref plane)).normal;
		return ((Vector3)(ref normal)).sqrMagnitude > 0.5f;
	}

	public static Matrix4x4 SampleInMatrix(this Gradient self, int floatPackingPrecision)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		Matrix4x4 result = default(Matrix4x4);
		for (int i = 0; i < 16; i++)
		{
			Color color = self.Evaluate(Mathf.Clamp01((float)i / 15f));
			((Matrix4x4)(ref result))[i] = color.PackToFloat(floatPackingPrecision);
		}
		return result;
	}

	public static Color[] SampleInArray(this Gradient self, int samplesCount)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		Color[] array = (Color[])(object)new Color[samplesCount];
		for (int i = 0; i < samplesCount; i++)
		{
			array[i] = self.Evaluate(Mathf.Clamp01((float)i / (float)(samplesCount - 1)));
		}
		return array;
	}

	private static Vector4 Vector4_Floor(Vector4 vec)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		return new Vector4(Mathf.Floor(vec.x), Mathf.Floor(vec.y), Mathf.Floor(vec.z), Mathf.Floor(vec.w));
	}

	public static float PackToFloat(this Color color, int floatPackingPrecision)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		Vector4 val = Vector4_Floor(Color.op_Implicit(color * (float)(floatPackingPrecision - 1)));
		return 0f + val.x * (float)floatPackingPrecision * (float)floatPackingPrecision * (float)floatPackingPrecision + val.y * (float)floatPackingPrecision * (float)floatPackingPrecision + val.z * (float)floatPackingPrecision + val.w;
	}

	public static FloatPackingPrecision GetFloatPackingPrecision()
	{
		if (ms_FloatPackingPrecision == FloatPackingPrecision.Undef)
		{
			ms_FloatPackingPrecision = ((SystemInfo.graphicsShaderLevel >= 35) ? FloatPackingPrecision.High : FloatPackingPrecision.Low);
		}
		return ms_FloatPackingPrecision;
	}

	public static void MarkCurrentSceneDirty()
	{
	}
}
