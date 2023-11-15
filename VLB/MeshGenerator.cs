using System;
using UnityEngine;

namespace VLB;

public static class MeshGenerator
{
	private const float kMinTruncatedRadius = 0.001f;

	private static bool duplicateBackFaces => Config.Instance.forceSinglePass;

	public static Mesh GenerateConeZ_RadiusAndAngle(float lengthZ, float radiusStart, float coneAngle, int numSides, int numSegments, bool cap)
	{
		Debug.Assert(lengthZ > 0f);
		Debug.Assert(coneAngle > 0f && coneAngle < 180f);
		float radiusEnd = lengthZ * Mathf.Tan(coneAngle * ((float)Math.PI / 180f) * 0.5f);
		return GenerateConeZ_Radius(lengthZ, radiusStart, radiusEnd, numSides, numSegments, cap);
	}

	public static Mesh GenerateConeZ_Angle(float lengthZ, float coneAngle, int numSides, int numSegments, bool cap)
	{
		return GenerateConeZ_RadiusAndAngle(lengthZ, 0f, coneAngle, numSides, numSegments, cap);
	}

	public static Mesh GenerateConeZ_Radius(float lengthZ, float radiusStart, float radiusEnd, int numSides, int numSegments, bool cap)
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Expected O, but got Unknown
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_052e: Unknown result type (might be due to invalid IL or missing references)
		//IL_054e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0559: Unknown result type (might be due to invalid IL or missing references)
		Debug.Assert(lengthZ > 0f);
		Debug.Assert(radiusStart >= 0f);
		Debug.Assert(numSides >= 3);
		Debug.Assert(numSegments >= 0);
		Mesh val = new Mesh();
		bool flag = false;
		flag = cap && radiusStart > 0f;
		radiusStart = Mathf.Max(radiusStart, 0.001f);
		int num = numSides * (numSegments + 2);
		int num2 = num;
		if (flag)
		{
			num2 += numSides + 1;
		}
		Vector3[] array = (Vector3[])(object)new Vector3[num2];
		for (int i = 0; i < numSides; i++)
		{
			float num3 = (float)Math.PI * 2f * (float)i / (float)numSides;
			float num4 = Mathf.Cos(num3);
			float num5 = Mathf.Sin(num3);
			for (int j = 0; j < numSegments + 2; j++)
			{
				float num6 = (float)j / (float)(numSegments + 1);
				Debug.Assert(num6 >= 0f && num6 <= 1f);
				float num7 = Mathf.Lerp(radiusStart, radiusEnd, num6);
				array[i + j * numSides] = new Vector3(num7 * num4, num7 * num5, num6 * lengthZ);
			}
		}
		if (flag)
		{
			int num8 = num;
			array[num8] = Vector3.zero;
			num8++;
			for (int k = 0; k < numSides; k++)
			{
				float num9 = (float)Math.PI * 2f * (float)k / (float)numSides;
				float num10 = Mathf.Cos(num9);
				float num11 = Mathf.Sin(num9);
				array[num8] = new Vector3(radiusStart * num10, radiusStart * num11, 0f);
				num8++;
			}
			Debug.Assert(num8 == array.Length);
		}
		if (!duplicateBackFaces)
		{
			val.vertices = array;
		}
		else
		{
			Vector3[] array2 = (Vector3[])(object)new Vector3[array.Length * 2];
			array.CopyTo(array2, 0);
			array.CopyTo(array2, array.Length);
			val.vertices = array2;
		}
		Vector2[] array3 = (Vector2[])(object)new Vector2[num2];
		int num12 = 0;
		for (int l = 0; l < num; l++)
		{
			array3[num12++] = Vector2.zero;
		}
		if (flag)
		{
			for (int m = 0; m < numSides + 1; m++)
			{
				array3[num12++] = new Vector2(1f, 0f);
			}
		}
		Debug.Assert(num12 == array3.Length);
		if (!duplicateBackFaces)
		{
			val.uv = array3;
		}
		else
		{
			Vector2[] array4 = (Vector2[])(object)new Vector2[array3.Length * 2];
			array3.CopyTo(array4, 0);
			array3.CopyTo(array4, array3.Length);
			for (int n = 0; n < array3.Length; n++)
			{
				Vector2 val2 = array4[n + array3.Length];
				array4[n + array3.Length] = new Vector2(val2.x, 1f);
			}
			val.uv = array4;
		}
		int num13 = numSides * 2 * Mathf.Max(numSegments + 1, 1);
		int num14 = num13 * 3;
		int num15 = num14;
		if (flag)
		{
			num15 += numSides * 3;
		}
		int[] array5 = new int[num15];
		int num16 = 0;
		for (int num17 = 0; num17 < numSides; num17++)
		{
			int num18 = num17 + 1;
			if (num18 == numSides)
			{
				num18 = 0;
			}
			for (int num19 = 0; num19 < numSegments + 1; num19++)
			{
				int num20 = num19 * numSides;
				array5[num16++] = num20 + num17;
				array5[num16++] = num20 + num18;
				array5[num16++] = num20 + num17 + numSides;
				array5[num16++] = num20 + num18 + numSides;
				array5[num16++] = num20 + num17 + numSides;
				array5[num16++] = num20 + num18;
			}
		}
		if (flag)
		{
			for (int num21 = 0; num21 < numSides - 1; num21++)
			{
				array5[num16++] = num;
				array5[num16++] = num + num21 + 2;
				array5[num16++] = num + num21 + 1;
			}
			array5[num16++] = num;
			array5[num16++] = num + 1;
			array5[num16++] = num + numSides;
		}
		Debug.Assert(num16 == array5.Length);
		if (!duplicateBackFaces)
		{
			val.triangles = array5;
		}
		else
		{
			int[] array6 = new int[array5.Length * 2];
			array5.CopyTo(array6, 0);
			for (int num22 = 0; num22 < array5.Length; num22 += 3)
			{
				array6[array5.Length + num22] = array5[num22] + num2;
				array6[array5.Length + num22 + 1] = array5[num22 + 2] + num2;
				array6[array5.Length + num22 + 2] = array5[num22 + 1] + num2;
			}
			val.triangles = array6;
		}
		Bounds bounds = default(Bounds);
		((Bounds)(ref bounds))._002Ector(new Vector3(0f, 0f, lengthZ * 0.5f), new Vector3(Mathf.Max(radiusStart, radiusEnd) * 2f, Mathf.Max(radiusStart, radiusEnd) * 2f, lengthZ));
		val.bounds = bounds;
		Debug.Assert(val.vertexCount == GetVertexCount(numSides, numSegments, flag));
		Debug.Assert(val.triangles.Length == GetIndicesCount(numSides, numSegments, flag));
		return val;
	}

	public static int GetVertexCount(int numSides, int numSegments, bool geomCap)
	{
		Debug.Assert(numSides >= 2);
		Debug.Assert(numSegments >= 0);
		int num = numSides * (numSegments + 2);
		if (geomCap)
		{
			num += numSides + 1;
		}
		if (duplicateBackFaces)
		{
			num *= 2;
		}
		return num;
	}

	public static int GetIndicesCount(int numSides, int numSegments, bool geomCap)
	{
		Debug.Assert(numSides >= 2);
		Debug.Assert(numSegments >= 0);
		int num = numSides * (numSegments + 1) * 2 * 3;
		if (geomCap)
		{
			num += numSides * 3;
		}
		if (duplicateBackFaces)
		{
			num *= 2;
		}
		return num;
	}

	public static int GetSharedMeshVertexCount()
	{
		return GetVertexCount(Config.Instance.sharedMeshSides, Config.Instance.sharedMeshSegments, geomCap: true);
	}

	public static int GetSharedMeshIndicesCount()
	{
		return GetIndicesCount(Config.Instance.sharedMeshSides, Config.Instance.sharedMeshSegments, geomCap: true);
	}
}
