using UnityEngine;

public class DrawSkeleton : MonoBehaviour
{
	private void OnDrawGizmos()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		Gizmos.color = Color.white;
		DrawTransform(((Component)this).transform);
	}

	private static void DrawTransform(Transform t)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < t.childCount; i++)
		{
			Gizmos.DrawLine(t.position, t.GetChild(i).position);
			DrawTransform(t.GetChild(i));
		}
	}
}
