using System.Collections.Generic;
using UnityEngine;

public class DungeonGridInfo : LandmarkInfo
{
	[Header("DungeonGridInfo")]
	public int CellSize = 216;

	public float LinkHeight = 1.5f;

	public float LinkRadius = 3f;

	internal List<GameObject> Links = new List<GameObject>();

	public float MinDistance => (float)CellSize * 2f;

	public float Distance(Vector3 position)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = ((Component)this).transform.position - position;
		return ((Vector3)(ref val)).magnitude;
	}

	public float SqrDistance(Vector3 position)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = ((Component)this).transform.position - position;
		return ((Vector3)(ref val)).sqrMagnitude;
	}

	public bool IsValidSpawnPosition(Vector3 position)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		DungeonVolume componentInChildren = ((Component)this).GetComponentInChildren<DungeonVolume>();
		OBB bounds = componentInChildren.GetBounds(position, Quaternion.identity);
		Vector3 val = WorldSpaceGrid.ClosestGridCell(bounds.position, TerrainMeta.Size.x * 2f, (float)CellSize);
		Vector3 val2 = bounds.position - val;
		return Mathf.Abs(val2.x) > 3f || Mathf.Abs(val2.z) > 3f;
	}

	public Vector3 SnapPosition(Vector3 pos)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		pos.x = (float)Mathf.RoundToInt(pos.x / LinkRadius) * LinkRadius;
		pos.y = (float)Mathf.CeilToInt(pos.y / LinkHeight) * LinkHeight;
		pos.z = (float)Mathf.RoundToInt(pos.z / LinkRadius) * LinkRadius;
		return pos;
	}

	protected override void Awake()
	{
		base.Awake();
		if (Object.op_Implicit((Object)(object)TerrainMeta.Path))
		{
			TerrainMeta.Path.DungeonGridEntrances.Add(this);
		}
	}

	protected void Start()
	{
		((Component)this).transform.SetHierarchyGroup("Dungeon");
	}
}
