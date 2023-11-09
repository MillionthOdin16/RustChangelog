using UnityEngine;

namespace VLB;

public static class GlobalMesh
{
	private static Mesh ms_Mesh = null;

	public static Mesh mesh
	{
		get
		{
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			if ((Object)(object)ms_Mesh == (Object)null)
			{
				ms_Mesh = MeshGenerator.GenerateConeZ_Radius(1f, 1f, 1f, Config.Instance.sharedMeshSides, Config.Instance.sharedMeshSegments, cap: true);
				((Object)ms_Mesh).hideFlags = Consts.ProceduralObjectsHideFlags;
			}
			return ms_Mesh;
		}
	}
}
