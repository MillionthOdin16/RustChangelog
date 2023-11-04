using UnityEngine;

public class VTP : MonoBehaviour
{
	public static Color getSingleVertexColorAtHit(Transform transform, RaycastHit hit)
	{
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		Vector3[] vertices = ((Component)transform).GetComponent<MeshFilter>().sharedMesh.vertices;
		int[] triangles = ((Component)transform).GetComponent<MeshFilter>().sharedMesh.triangles;
		Color[] colors = ((Component)transform).GetComponent<MeshFilter>().sharedMesh.colors;
		int triangleIndex = ((RaycastHit)(ref hit)).triangleIndex;
		float num = float.PositiveInfinity;
		int num2 = 0;
		for (int i = 0; i < 3; i++)
		{
			Vector3 val = transform.TransformPoint(vertices[triangles[triangleIndex * 3 + i]]);
			float num3 = Vector3.Distance(val, ((RaycastHit)(ref hit)).point);
			if (num3 < num)
			{
				num2 = triangles[triangleIndex * 3 + i];
				num = num3;
			}
		}
		return colors[num2];
	}

	public static Color getFaceVerticesColorAtHit(Transform transform, RaycastHit hit)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		int[] triangles = ((Component)transform).GetComponent<MeshFilter>().sharedMesh.triangles;
		Color[] colors = ((Component)transform).GetComponent<MeshFilter>().sharedMesh.colors;
		int triangleIndex = ((RaycastHit)(ref hit)).triangleIndex;
		int num = triangles[triangleIndex * 3];
		Color val = colors[num];
		val += colors[num + 1];
		val += colors[num + 2];
		return val / 3f;
	}

	public static void paintSingleVertexOnHit(Transform transform, RaycastHit hit, Color color, float strength)
	{
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		Vector3[] vertices = ((Component)transform).GetComponent<MeshFilter>().sharedMesh.vertices;
		int[] triangles = ((Component)transform).GetComponent<MeshFilter>().sharedMesh.triangles;
		Color[] colors = ((Component)transform).GetComponent<MeshFilter>().sharedMesh.colors;
		int triangleIndex = ((RaycastHit)(ref hit)).triangleIndex;
		float num = float.PositiveInfinity;
		int num2 = 0;
		for (int i = 0; i < 3; i += 3)
		{
			Vector3 val = transform.TransformPoint(vertices[triangles[triangleIndex * 3 + i]]);
			float num3 = Vector3.Distance(val, ((RaycastHit)(ref hit)).point);
			if (num3 < num)
			{
				num2 = triangles[triangleIndex * 3 + i];
				num = num3;
			}
		}
		Color val2 = VertexColorLerp(colors[num2], color, strength);
		colors[num2] = val2;
		((Component)transform).GetComponent<MeshFilter>().sharedMesh.colors = colors;
	}

	public static void paintFaceVerticesOnHit(Transform transform, RaycastHit hit, Color color, float strength)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		int[] triangles = ((Component)transform).GetComponent<MeshFilter>().sharedMesh.triangles;
		Color[] colors = ((Component)transform).GetComponent<MeshFilter>().sharedMesh.colors;
		int triangleIndex = ((RaycastHit)(ref hit)).triangleIndex;
		int num = 0;
		for (int i = 0; i < 3; i++)
		{
			num = triangles[triangleIndex * 3 + i];
			Color val = VertexColorLerp(colors[num], color, strength);
			colors[num] = val;
		}
		((Component)transform).GetComponent<MeshFilter>().sharedMesh.colors = colors;
	}

	public static void deformSingleVertexOnHit(Transform transform, RaycastHit hit, bool up, float strength, bool recalculateNormals, bool recalculateCollider, bool recalculateFlow)
	{
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		Vector3[] vertices = ((Component)transform).GetComponent<MeshFilter>().sharedMesh.vertices;
		int[] triangles = ((Component)transform).GetComponent<MeshFilter>().sharedMesh.triangles;
		Vector3[] normals = ((Component)transform).GetComponent<MeshFilter>().sharedMesh.normals;
		int triangleIndex = ((RaycastHit)(ref hit)).triangleIndex;
		float num = float.PositiveInfinity;
		int num2 = 0;
		for (int i = 0; i < 3; i++)
		{
			Vector3 val = transform.TransformPoint(vertices[triangles[triangleIndex * 3 + i]]);
			float num3 = Vector3.Distance(val, ((RaycastHit)(ref hit)).point);
			if (num3 < num)
			{
				num2 = triangles[triangleIndex * 3 + i];
				num = num3;
			}
		}
		int num4 = 1;
		if (!up)
		{
			num4 = -1;
		}
		ref Vector3 reference = ref vertices[num2];
		reference += (float)num4 * 0.1f * strength * normals[num2];
		((Component)transform).GetComponent<MeshFilter>().sharedMesh.vertices = vertices;
		if (recalculateNormals)
		{
			((Component)transform).GetComponent<MeshFilter>().sharedMesh.RecalculateNormals();
		}
		if (recalculateCollider)
		{
			((Component)transform).GetComponent<MeshCollider>().sharedMesh = ((Component)transform).GetComponent<MeshFilter>().sharedMesh;
		}
		if (recalculateFlow)
		{
			Vector4[] array = calculateMeshTangents(triangles, vertices, ((Component)transform).GetComponent<MeshCollider>().sharedMesh.uv, normals);
			((Component)transform).GetComponent<MeshCollider>().sharedMesh.tangents = array;
			recalculateMeshForFlow(transform, vertices, normals, array);
		}
	}

	public static void deformFaceVerticesOnHit(Transform transform, RaycastHit hit, bool up, float strength, bool recalculateNormals, bool recalculateCollider, bool recalculateFlow)
	{
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		Vector3[] vertices = ((Component)transform).GetComponent<MeshFilter>().sharedMesh.vertices;
		int[] triangles = ((Component)transform).GetComponent<MeshFilter>().sharedMesh.triangles;
		Vector3[] normals = ((Component)transform).GetComponent<MeshFilter>().sharedMesh.normals;
		int triangleIndex = ((RaycastHit)(ref hit)).triangleIndex;
		int num = 0;
		int num2 = 1;
		if (!up)
		{
			num2 = -1;
		}
		for (int i = 0; i < 3; i++)
		{
			num = triangles[triangleIndex * 3 + i];
			ref Vector3 reference = ref vertices[num];
			reference += (float)num2 * 0.1f * strength * normals[num];
		}
		((Component)transform).GetComponent<MeshFilter>().sharedMesh.vertices = vertices;
		if (recalculateNormals)
		{
			((Component)transform).GetComponent<MeshFilter>().sharedMesh.RecalculateNormals();
		}
		if (recalculateCollider)
		{
			((Component)transform).GetComponent<MeshCollider>().sharedMesh = ((Component)transform).GetComponent<MeshFilter>().sharedMesh;
		}
		if (recalculateFlow)
		{
			Vector4[] array = calculateMeshTangents(triangles, vertices, ((Component)transform).GetComponent<MeshCollider>().sharedMesh.uv, normals);
			((Component)transform).GetComponent<MeshCollider>().sharedMesh.tangents = array;
			recalculateMeshForFlow(transform, vertices, normals, array);
		}
	}

	private static void recalculateMeshForFlow(Transform transform, Vector3[] currentVertices, Vector3[] currentNormals, Vector4[] currentTangents)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		Vector2[] uv = ((Component)transform).GetComponent<MeshFilter>().sharedMesh.uv4;
		for (int i = 0; i < currentVertices.Length; i++)
		{
			Vector3 val = Vector3.Cross(currentNormals[i], new Vector3(currentTangents[i].x, currentTangents[i].y, currentTangents[i].z));
			Vector3 val2 = transform.TransformDirection(((Vector3)(ref val)).normalized * currentTangents[i].w);
			Vector3 val3 = transform.TransformDirection(Vector4.op_Implicit(((Vector4)(ref currentTangents[i])).normalized));
			float num = 0.5f + 0.5f * val3.y;
			float num2 = 0.5f + 0.5f * val2.y;
			uv[i] = new Vector2(num, num2);
		}
		((Component)transform).GetComponent<MeshFilter>().sharedMesh.uv4 = uv;
	}

	private static Vector4[] calculateMeshTangents(int[] triangles, Vector3[] vertices, Vector2[] uv, Vector3[] normals)
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
		int num = triangles.Length;
		int num2 = vertices.Length;
		Vector3[] array = (Vector3[])(object)new Vector3[num2];
		Vector3[] array2 = (Vector3[])(object)new Vector3[num2];
		Vector4[] array3 = (Vector4[])(object)new Vector4[num2];
		Vector3 val7 = default(Vector3);
		Vector3 val8 = default(Vector3);
		for (long num3 = 0L; num3 < num; num3 += 3)
		{
			long num4 = triangles[num3];
			long num5 = triangles[num3 + 1];
			long num6 = triangles[num3 + 2];
			Vector3 val = vertices[num4];
			Vector3 val2 = vertices[num5];
			Vector3 val3 = vertices[num6];
			Vector2 val4 = uv[num4];
			Vector2 val5 = uv[num5];
			Vector2 val6 = uv[num6];
			float num7 = val2.x - val.x;
			float num8 = val3.x - val.x;
			float num9 = val2.y - val.y;
			float num10 = val3.y - val.y;
			float num11 = val2.z - val.z;
			float num12 = val3.z - val.z;
			float num13 = val5.x - val4.x;
			float num14 = val6.x - val4.x;
			float num15 = val5.y - val4.y;
			float num16 = val6.y - val4.y;
			float num17 = num13 * num16 - num14 * num15;
			float num18 = ((num17 == 0f) ? 0f : (1f / num17));
			((Vector3)(ref val7))._002Ector((num16 * num7 - num15 * num8) * num18, (num16 * num9 - num15 * num10) * num18, (num16 * num11 - num15 * num12) * num18);
			((Vector3)(ref val8))._002Ector((num13 * num8 - num14 * num7) * num18, (num13 * num10 - num14 * num9) * num18, (num13 * num12 - num14 * num11) * num18);
			ref Vector3 reference = ref array[num4];
			reference += val7;
			ref Vector3 reference2 = ref array[num5];
			reference2 += val7;
			ref Vector3 reference3 = ref array[num6];
			reference3 += val7;
			ref Vector3 reference4 = ref array2[num4];
			reference4 += val8;
			ref Vector3 reference5 = ref array2[num5];
			reference5 += val8;
			ref Vector3 reference6 = ref array2[num6];
			reference6 += val8;
		}
		for (long num19 = 0L; num19 < num2; num19++)
		{
			Vector3 val9 = normals[num19];
			Vector3 val10 = array[num19];
			Vector3.OrthoNormalize(ref val9, ref val10);
			array3[num19].x = val10.x;
			array3[num19].y = val10.y;
			array3[num19].z = val10.z;
			array3[num19].w = ((Vector3.Dot(Vector3.Cross(val9, val10), array2[num19]) < 0f) ? (-1f) : 1f);
		}
		return array3;
	}

	public static Color VertexColorLerp(Color colorA, Color colorB, float value)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		if (value >= 1f)
		{
			return colorB;
		}
		if (value <= 0f)
		{
			return colorA;
		}
		return new Color(colorA.r + (colorB.r - colorA.r) * value, colorA.g + (colorB.g - colorA.g) * value, colorA.b + (colorB.b - colorA.b) * value, colorA.a + (colorB.a - colorA.a) * value);
	}
}
