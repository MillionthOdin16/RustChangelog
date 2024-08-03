using System;
using UnityEngine;

public class TerrainFilter : PrefabAttribute
{
	public SpawnFilter Filter;

	public bool CheckPlacementMap = true;

	protected void OnDrawGizmosSelected()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		Gizmos.color = new Color(0.5f, 0.5f, 0.5f, 1f);
		Gizmos.DrawCube(((Component)this).transform.position + Vector3.up * 50f * 0.5f, new Vector3(0.5f, 50f, 0.5f));
		Gizmos.DrawSphere(((Component)this).transform.position + Vector3.up * 50f, 2f);
	}

	public bool Check(Vector3 pos)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return Filter.GetFactor(pos, CheckPlacementMap) > 0f;
	}

	protected override Type GetIndexedType()
	{
		return typeof(TerrainFilter);
	}
}
