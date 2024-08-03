using System;
using UnityEngine;

public class CargoMoveTest : FacepunchBehaviour
{
	public int targetNodeIndex = -1;

	private float currentThrottle = 0f;

	private float turnScale = 0f;

	private void Awake()
	{
		((FacepunchBehaviour)this).Invoke((Action)FindInitialNode, 2f);
	}

	public void FindInitialNode()
	{
		targetNodeIndex = GetClosestNodeToUs();
	}

	private void Update()
	{
		UpdateMovement();
	}

	public void UpdateMovement()
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		if (TerrainMeta.Path.OceanPatrolFar == null || TerrainMeta.Path.OceanPatrolFar.Count == 0 || targetNodeIndex == -1)
		{
			return;
		}
		Vector3 val = TerrainMeta.Path.OceanPatrolFar[targetNodeIndex];
		Vector3 val2 = val;
		float num = 0f;
		Vector3 val3 = val2 - ((Component)this).transform.position;
		Vector3 normalized = ((Vector3)(ref val3)).normalized;
		float num2 = Vector3.Dot(((Component)this).transform.forward, normalized);
		float num3 = Mathf.InverseLerp(0.5f, 1f, num2);
		num = num3;
		float num4 = Vector3.Dot(((Component)this).transform.right, normalized);
		float num5 = 5f;
		float num6 = Mathf.InverseLerp(0.05f, 0.5f, Mathf.Abs(num4));
		turnScale = Mathf.Lerp(turnScale, num6, Time.deltaTime * 0.2f);
		float num7 = ((!(num4 < 0f)) ? 1 : (-1));
		((Component)this).transform.Rotate(Vector3.up, num5 * Time.deltaTime * turnScale * num7, (Space)0);
		currentThrottle = Mathf.Lerp(currentThrottle, num, Time.deltaTime * 0.2f);
		Transform transform = ((Component)this).transform;
		transform.position += ((Component)this).transform.forward * 5f * Time.deltaTime * currentThrottle;
		if (Vector3.Distance(((Component)this).transform.position, val2) < 60f)
		{
			targetNodeIndex++;
			if (targetNodeIndex >= TerrainMeta.Path.OceanPatrolFar.Count)
			{
				targetNodeIndex = 0;
			}
		}
	}

	public int GetClosestNodeToUs()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		int result = 0;
		float num = float.PositiveInfinity;
		for (int i = 0; i < TerrainMeta.Path.OceanPatrolFar.Count; i++)
		{
			Vector3 val = TerrainMeta.Path.OceanPatrolFar[i];
			float num2 = Vector3.Distance(((Component)this).transform.position, val);
			if (num2 < num)
			{
				result = i;
				num = num2;
			}
		}
		return result;
	}

	public void OnDrawGizmosSelected()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		if (TerrainMeta.Path.OceanPatrolFar != null)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawSphere(TerrainMeta.Path.OceanPatrolFar[targetNodeIndex], 10f);
			for (int i = 0; i < TerrainMeta.Path.OceanPatrolFar.Count; i++)
			{
				Vector3 val = TerrainMeta.Path.OceanPatrolFar[i];
				Gizmos.color = Color.green;
				Gizmos.DrawSphere(val, 3f);
				Vector3 val2 = ((i + 1 == TerrainMeta.Path.OceanPatrolFar.Count) ? TerrainMeta.Path.OceanPatrolFar[0] : TerrainMeta.Path.OceanPatrolFar[i + 1]);
				Gizmos.DrawLine(val, val2);
			}
		}
	}
}
