using UnityEngine;

public class TriangleIdentifier : MonoBehaviour
{
	public int TriangleID = 0;

	public int SubmeshID = 0;

	public float LineLength = 1.5f;

	private void OnDrawGizmosSelected()
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		MeshFilter component = ((Component)this).GetComponent<MeshFilter>();
		if (!((Object)(object)component == (Object)null) && !((Object)(object)component.sharedMesh == (Object)null))
		{
			int[] triangles = component.sharedMesh.GetTriangles(SubmeshID);
			if (TriangleID >= 0 && TriangleID * 3 <= triangles.Length)
			{
				Gizmos.matrix = ((Component)this).transform.localToWorldMatrix;
				Vector3 val = component.sharedMesh.vertices[TriangleID * 3];
				Vector3 val2 = component.sharedMesh.vertices[TriangleID * 3 + 1];
				Vector3 val3 = component.sharedMesh.vertices[TriangleID * 3 + 2];
				Vector3 val4 = component.sharedMesh.normals[TriangleID * 3];
				Vector3 val5 = component.sharedMesh.normals[TriangleID * 3 + 1];
				Vector3 val6 = component.sharedMesh.normals[TriangleID * 3 + 2];
				Vector3 val7 = (val + val2 + val3) / 3f;
				Vector3 val8 = (val4 + val5 + val6) / 3f;
				Gizmos.DrawLine(val7, val7 + val8 * LineLength);
			}
		}
	}
}
