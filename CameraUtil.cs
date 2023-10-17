using UnityEngine;

public static class CameraUtil
{
	public static void NormalizePlane(ref Plane plane)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		Vector3 normal = ((Plane)(ref plane)).normal;
		float num = 1f / ((Vector3)(ref normal)).magnitude;
		((Plane)(ref plane)).normal = ((Plane)(ref plane)).normal * num;
		((Plane)(ref plane)).distance = ((Plane)(ref plane)).distance * num;
	}

	public static void ExtractPlanes(Camera camera, ref Plane[] planes)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		Matrix4x4 worldToCameraMatrix = camera.worldToCameraMatrix;
		Matrix4x4 gPUProjectionMatrix = GL.GetGPUProjectionMatrix(camera.projectionMatrix, false);
		Matrix4x4 viewProjMatrix = gPUProjectionMatrix * worldToCameraMatrix;
		ExtractPlanes(viewProjMatrix, ref planes);
	}

	public static void ExtractPlanes(Matrix4x4 viewProjMatrix, ref Plane[] planes)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		((Plane)(ref planes[0])).normal = new Vector3(viewProjMatrix.m30 + viewProjMatrix.m00, viewProjMatrix.m31 + viewProjMatrix.m01, viewProjMatrix.m32 + viewProjMatrix.m02);
		((Plane)(ref planes[0])).distance = viewProjMatrix.m33 + viewProjMatrix.m03;
		NormalizePlane(ref planes[0]);
		((Plane)(ref planes[1])).normal = new Vector3(viewProjMatrix.m30 - viewProjMatrix.m00, viewProjMatrix.m31 - viewProjMatrix.m01, viewProjMatrix.m32 - viewProjMatrix.m02);
		((Plane)(ref planes[1])).distance = viewProjMatrix.m33 - viewProjMatrix.m03;
		NormalizePlane(ref planes[1]);
		((Plane)(ref planes[2])).normal = new Vector3(viewProjMatrix.m30 - viewProjMatrix.m10, viewProjMatrix.m31 - viewProjMatrix.m11, viewProjMatrix.m32 - viewProjMatrix.m12);
		((Plane)(ref planes[2])).distance = viewProjMatrix.m33 - viewProjMatrix.m13;
		NormalizePlane(ref planes[2]);
		((Plane)(ref planes[3])).normal = new Vector3(viewProjMatrix.m30 + viewProjMatrix.m10, viewProjMatrix.m31 + viewProjMatrix.m11, viewProjMatrix.m32 + viewProjMatrix.m12);
		((Plane)(ref planes[3])).distance = viewProjMatrix.m33 + viewProjMatrix.m13;
		NormalizePlane(ref planes[3]);
		((Plane)(ref planes[4])).normal = new Vector3(viewProjMatrix.m20, viewProjMatrix.m21, viewProjMatrix.m22);
		((Plane)(ref planes[4])).distance = viewProjMatrix.m23;
		NormalizePlane(ref planes[4]);
		((Plane)(ref planes[5])).normal = new Vector3(viewProjMatrix.m30 - viewProjMatrix.m20, viewProjMatrix.m31 - viewProjMatrix.m21, viewProjMatrix.m32 - viewProjMatrix.m22);
		((Plane)(ref planes[5])).distance = viewProjMatrix.m33 - viewProjMatrix.m23;
		NormalizePlane(ref planes[5]);
	}
}
