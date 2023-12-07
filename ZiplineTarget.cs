using UnityEngine;

public class ZiplineTarget : MonoBehaviour
{
	public Transform Target;

	public bool IsChainPoint = false;

	public float MonumentConnectionDotMin = 0.2f;

	public float MonumentConnectionDotMax = 1f;

	public bool IsValidPosition(Vector3 position)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = position - Vector3Ex.WithY(Target.position, position.y);
		float num = Vector3.Dot(((Vector3)(ref val)).normalized, Target.forward);
		return num >= MonumentConnectionDotMin && num <= MonumentConnectionDotMax;
	}

	public bool IsValidChainPoint(Vector3 from, Vector3 to)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = from - Vector3Ex.WithY(Target.position, from.y);
		float num = Vector3.Dot(((Vector3)(ref val)).normalized, Target.forward);
		val = to - Vector3Ex.WithY(Target.position, from.y);
		float num2 = Vector3.Dot(((Vector3)(ref val)).normalized, Target.forward);
		if ((num > 0f && num2 > 0f) || (num < 0f && num2 < 0f))
		{
			return false;
		}
		num2 = Mathf.Abs(num2);
		return num2 >= MonumentConnectionDotMin && num2 <= MonumentConnectionDotMax;
	}
}
