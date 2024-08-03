using UnityEngine;

public class TickHistory
{
	private Deque<Vector3> points = new Deque<Vector3>(8);

	public int Count => points.Count;

	public void Reset()
	{
		points.Clear();
	}

	public void Reset(Vector3 point)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		Reset();
		AddPoint(point);
	}

	public float Distance(BasePlayer player, Vector3 point)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		if (points.Count == 0)
		{
			return player.Distance(point);
		}
		Vector3 position = ((Component)player).transform.position;
		Quaternion rotation = ((Component)player).transform.rotation;
		Bounds bounds = player.bounds;
		Matrix4x4 tickHistoryMatrix = player.tickHistoryMatrix;
		float num = float.MaxValue;
		Line val3 = default(Line);
		OBB val5 = default(OBB);
		for (int i = 0; i < points.Count; i++)
		{
			Vector3 val = ((Matrix4x4)(ref tickHistoryMatrix)).MultiplyPoint3x4(points[i]);
			Vector3 val2 = ((i == points.Count - 1) ? position : ((Matrix4x4)(ref tickHistoryMatrix)).MultiplyPoint3x4(points[i + 1]));
			((Line)(ref val3))._002Ector(val, val2);
			Vector3 val4 = ((Line)(ref val3)).ClosestPoint(point);
			((OBB)(ref val5))._002Ector(val4, rotation, bounds);
			num = Mathf.Min(num, ((OBB)(ref val5)).Distance(point));
		}
		return num;
	}

	public void AddPoint(Vector3 point, int limit = -1)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		while (limit > 0 && points.Count >= limit)
		{
			points.PopFront();
		}
		points.PushBack(point);
	}

	public void TransformEntries(Matrix4x4 matrix)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < points.Count; i++)
		{
			Vector3 val = points[i];
			val = ((Matrix4x4)(ref matrix)).MultiplyPoint3x4(val);
			points[i] = val;
		}
	}
}
