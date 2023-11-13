using UnityEngine;

public class MonumentInfo : LandmarkInfo, IPrefabPreProcess
{
	[Header("MonumentInfo")]
	public MonumentType Type = MonumentType.Building;

	[InspectorFlags]
	public MonumentTier Tier = (MonumentTier)(-1);

	public int MinWorldSize = 0;

	public Bounds Bounds = new Bounds(Vector3.zero, Vector3.zero);

	public bool HasNavmesh = false;

	public bool IsSafeZone = false;

	[HideInInspector]
	public bool WantsDungeonLink = false;

	[HideInInspector]
	public bool HasDungeonLink = false;

	[HideInInspector]
	public DungeonGridInfo DungeonEntrance = null;

	private OBB obbBounds;

	protected override void Awake()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		base.Awake();
		obbBounds = new OBB(((Component)this).transform.position, ((Component)this).transform.rotation, Bounds);
		if (Object.op_Implicit((Object)(object)TerrainMeta.Path))
		{
			TerrainMeta.Path.Monuments.Add(this);
		}
	}

	public bool CheckPlacement(Vector3 pos, Quaternion rot, Vector3 scale)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		OBB val = default(OBB);
		((OBB)(ref val))._002Ector(pos, scale, rot, Bounds);
		Vector3 point = ((OBB)(ref val)).GetPoint(-1f, 0f, -1f);
		Vector3 point2 = ((OBB)(ref val)).GetPoint(-1f, 0f, 1f);
		Vector3 point3 = ((OBB)(ref val)).GetPoint(1f, 0f, -1f);
		Vector3 point4 = ((OBB)(ref val)).GetPoint(1f, 0f, 1f);
		int topology = TerrainMeta.TopologyMap.GetTopology(point);
		int topology2 = TerrainMeta.TopologyMap.GetTopology(point2);
		int topology3 = TerrainMeta.TopologyMap.GetTopology(point3);
		int topology4 = TerrainMeta.TopologyMap.GetTopology(point4);
		int num = TierToMask(Tier);
		int num2 = 0;
		if ((num & topology) != 0)
		{
			num2++;
		}
		if ((num & topology2) != 0)
		{
			num2++;
		}
		if ((num & topology3) != 0)
		{
			num2++;
		}
		if ((num & topology4) != 0)
		{
			num2++;
		}
		return num2 >= 3;
	}

	public float Distance(Vector3 position)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return ((OBB)(ref obbBounds)).Distance(position);
	}

	public float SqrDistance(Vector3 position)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return ((OBB)(ref obbBounds)).SqrDistance(position);
	}

	public float Distance(OBB obb)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return ((OBB)(ref obbBounds)).Distance(obb);
	}

	public float SqrDistance(OBB obb)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return ((OBB)(ref obbBounds)).SqrDistance(obb);
	}

	public bool IsInBounds(Vector3 position)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return ((OBB)(ref obbBounds)).Contains(position);
	}

	public Vector3 ClosestPointOnBounds(Vector3 position)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		return ((OBB)(ref obbBounds)).ClosestPoint(position);
	}

	public PathFinder.Point GetPathFinderPoint(int res)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		Vector3 position = ((Component)this).transform.position;
		float num = TerrainMeta.NormalizeX(position.x);
		float num2 = TerrainMeta.NormalizeZ(position.z);
		PathFinder.Point result = default(PathFinder.Point);
		result.x = Mathf.Clamp((int)(num * (float)res), 0, res - 1);
		result.y = Mathf.Clamp((int)(num2 * (float)res), 0, res - 1);
		return result;
	}

	public int GetPathFinderRadius(int res)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		float num = ((Bounds)(ref Bounds)).extents.x * TerrainMeta.OneOverSize.x;
		float num2 = ((Bounds)(ref Bounds)).extents.z * TerrainMeta.OneOverSize.z;
		return Mathf.CeilToInt(Mathf.Max(num, num2) * (float)res);
	}

	protected void OnDrawGizmosSelected()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		Gizmos.matrix = ((Component)this).transform.localToWorldMatrix;
		Gizmos.color = new Color(0f, 0.7f, 1f, 0.1f);
		Gizmos.DrawCube(((Bounds)(ref Bounds)).center, ((Bounds)(ref Bounds)).size);
		Gizmos.color = new Color(0f, 0.7f, 1f, 1f);
		Gizmos.DrawWireCube(((Bounds)(ref Bounds)).center, ((Bounds)(ref Bounds)).size);
	}

	public MonumentNavMesh GetMonumentNavMesh()
	{
		return ((Component)this).GetComponent<MonumentNavMesh>();
	}

	public static int TierToMask(MonumentTier tier)
	{
		int num = 0;
		if ((tier & MonumentTier.Tier0) != 0)
		{
			num |= 0x4000000;
		}
		if ((tier & MonumentTier.Tier1) != 0)
		{
			num |= 0x8000000;
		}
		if ((tier & MonumentTier.Tier2) != 0)
		{
			num |= 0x10000000;
		}
		return num;
	}

	public void PreProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		HasDungeonLink = DetermineHasDungeonLink();
		WantsDungeonLink = DetermineWantsDungeonLink();
		DungeonEntrance = FindDungeonEntrance();
	}

	private DungeonGridInfo FindDungeonEntrance()
	{
		return ((Component)this).GetComponentInChildren<DungeonGridInfo>();
	}

	private bool DetermineHasDungeonLink()
	{
		return (Object)(object)((Component)this).GetComponentInChildren<DungeonGridLink>() != (Object)null;
	}

	private bool DetermineWantsDungeonLink()
	{
		if (Type == MonumentType.WaterWell)
		{
			return false;
		}
		if (Type == MonumentType.Building && displayPhrase.token.StartsWith("mining_quarry"))
		{
			return false;
		}
		if (Type == MonumentType.Radtown && displayPhrase.token.StartsWith("swamp"))
		{
			return false;
		}
		return true;
	}
}
