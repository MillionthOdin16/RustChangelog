using System.IO;
using System.Text;
using UnityEngine;

public static class ObjWriter
{
	public static string MeshToString(Mesh mesh)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("g ").Append(((Object)mesh).name).Append("\n");
		Vector3[] vertices = mesh.vertices;
		foreach (Vector3 val in vertices)
		{
			stringBuilder.Append($"v {0f - val.x} {val.y} {val.z}\n");
		}
		stringBuilder.Append("\n");
		Vector3[] normals = mesh.normals;
		foreach (Vector3 val2 in normals)
		{
			stringBuilder.Append($"vn {0f - val2.x} {val2.y} {val2.z}\n");
		}
		stringBuilder.Append("\n");
		Vector2[] uv = mesh.uv;
		for (int k = 0; k < uv.Length; k++)
		{
			Vector3 val3 = Vector2.op_Implicit(uv[k]);
			stringBuilder.Append($"vt {val3.x} {val3.y}\n");
		}
		stringBuilder.Append("\n");
		int[] triangles = mesh.triangles;
		for (int l = 0; l < triangles.Length; l += 3)
		{
			int num = triangles[l] + 1;
			int num2 = triangles[l + 1] + 1;
			int num3 = triangles[l + 2] + 1;
			stringBuilder.Append(string.Format("f {1}/{1}/{1} {0}/{0}/{0} {2}/{2}/{2}\n", num, num2, num3));
		}
		return stringBuilder.ToString();
	}

	public static void Write(Mesh mesh, string path)
	{
		using StreamWriter streamWriter = new StreamWriter(path);
		streamWriter.Write(MeshToString(mesh));
	}
}
