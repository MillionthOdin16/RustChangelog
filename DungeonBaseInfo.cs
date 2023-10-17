using System.Collections.Generic;
using UnityEngine;

public class DungeonBaseInfo : LandmarkInfo
{
	internal List<GameObject> Links = new List<GameObject>();

	internal List<DungeonBaseFloor> Floors = new List<DungeonBaseFloor>();

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

	public void Add(DungeonBaseLink link)
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		Links.Add(((Component)link).gameObject);
		if (link.Type == DungeonBaseLinkType.End)
		{
			return;
		}
		DungeonBaseFloor dungeonBaseFloor = null;
		float num = float.MaxValue;
		for (int i = 0; i < Floors.Count; i++)
		{
			DungeonBaseFloor dungeonBaseFloor2 = Floors[i];
			float num2 = dungeonBaseFloor2.Distance(((Component)link).transform.position);
			if (!(num2 >= 1f) && !(num2 >= num))
			{
				dungeonBaseFloor = dungeonBaseFloor2;
				num = num2;
			}
		}
		if (dungeonBaseFloor == null)
		{
			dungeonBaseFloor = new DungeonBaseFloor();
			dungeonBaseFloor.Links.Add(link);
			Floors.Add(dungeonBaseFloor);
			Floors.Sort((DungeonBaseFloor l, DungeonBaseFloor r) => l.SignedDistance(((Component)this).transform.position).CompareTo(r.SignedDistance(((Component)this).transform.position)));
		}
		else
		{
			dungeonBaseFloor.Links.Add(link);
		}
	}

	protected override void Awake()
	{
		base.Awake();
		if (Object.op_Implicit((Object)(object)TerrainMeta.Path))
		{
			TerrainMeta.Path.DungeonBaseEntrances.Add(this);
		}
	}

	protected void Start()
	{
		((Component)this).transform.SetHierarchyGroup("DungeonBase");
	}
}
