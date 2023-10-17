using System;
using System.Collections.Generic;
using ConVar;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Profiling;

public class AIInformationZone : BaseMonoBehaviour, IServerComponent
{
	public bool RenderBounds = false;

	public bool ShouldSleepAI = false;

	public bool Virtual = false;

	public bool UseCalculatedCoverDistances = true;

	public static List<AIInformationZone> zones = new List<AIInformationZone>();

	public List<AICoverPoint> coverPoints = new List<AICoverPoint>();

	public List<AIMovePoint> movePoints = new List<AIMovePoint>();

	private AICoverPoint[] coverPointArray;

	private AIMovePoint[] movePointArray;

	public List<NavMeshLink> navMeshLinks = new List<NavMeshLink>();

	public List<AIMovePointPath> paths = new List<AIMovePointPath>();

	public Bounds bounds;

	private AIInformationGrid grid;

	private List<IAISleepable> sleepables = new List<IAISleepable>();

	private OBB areaBox;

	private bool isDirty = true;

	private int processIndex = 0;

	private int halfPaths = 0;

	private int pathSuccesses = 0;

	private int pathFails = 0;

	private bool initd = false;

	private static bool lastFrameAnyDirty = false;

	private static float rebuildStartTime = 0f;

	public static float buildTimeTest = 0f;

	private static float lastNavmeshBuildTime = 0f;

	public bool Sleeping { get; private set; }

	public int SleepingCount
	{
		get
		{
			if (!Sleeping)
			{
				return 0;
			}
			return sleepables.Count;
		}
	}

	public static AIInformationZone Merge(List<AIInformationZone> zones, GameObject newRoot)
	{
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		if (zones == null)
		{
			return null;
		}
		AIInformationZone aIInformationZone = newRoot.AddComponent<AIInformationZone>();
		aIInformationZone.UseCalculatedCoverDistances = false;
		foreach (AIInformationZone zone in zones)
		{
			if ((Object)(object)zone == (Object)null)
			{
				continue;
			}
			foreach (AIMovePoint movePoint in zone.movePoints)
			{
				aIInformationZone.AddMovePoint(movePoint);
				((Component)movePoint).transform.SetParent(newRoot.transform);
			}
			foreach (AICoverPoint coverPoint in zone.coverPoints)
			{
				aIInformationZone.AddCoverPoint(coverPoint);
				((Component)coverPoint).transform.SetParent(newRoot.transform);
			}
		}
		aIInformationZone.bounds = EncapsulateBounds(zones);
		ref Bounds reference = ref aIInformationZone.bounds;
		((Bounds)(ref reference)).extents = ((Bounds)(ref reference)).extents + new Vector3(5f, 0f, 5f);
		ref Bounds reference2 = ref aIInformationZone.bounds;
		((Bounds)(ref reference2)).center = ((Bounds)(ref reference2)).center - ((Component)aIInformationZone).transform.position;
		for (int num = zones.Count - 1; num >= 0; num--)
		{
			AIInformationZone aIInformationZone2 = zones[num];
			if (!((Object)(object)aIInformationZone2 == (Object)null))
			{
				Object.Destroy((Object)(object)aIInformationZone2);
			}
		}
		return aIInformationZone;
	}

	public static Bounds EncapsulateBounds(List<AIInformationZone> zones)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		Bounds result = default(Bounds);
		((Bounds)(ref result)).center = ((Component)zones[0]).transform.position;
		foreach (AIInformationZone zone in zones)
		{
			if (!((Object)(object)zone == (Object)null))
			{
				Vector3 center = ((Bounds)(ref zone.bounds)).center + ((Component)zone).transform.position;
				Bounds val = zone.bounds;
				((Bounds)(ref val)).center = center;
				((Bounds)(ref result)).Encapsulate(val);
			}
		}
		return result;
	}

	public void Start()
	{
		Init();
	}

	public void Init()
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		if (!initd)
		{
			initd = true;
			AddInitialPoints();
			areaBox = new OBB(((Component)this).transform.position, ((Component)this).transform.lossyScale, ((Component)this).transform.rotation, bounds);
			zones.Add(this);
			grid = ((Component)this).GetComponent<AIInformationGrid>();
			if ((Object)(object)grid != (Object)null)
			{
				grid.Init();
			}
		}
	}

	public void RegisterSleepableEntity(IAISleepable sleepable)
	{
		if (sleepable != null && sleepable.AllowedToSleep() && !sleepables.Contains(sleepable))
		{
			sleepables.Add(sleepable);
			if (Sleeping && sleepable.AllowedToSleep())
			{
				sleepable.SleepAI();
			}
		}
	}

	public void UnregisterSleepableEntity(IAISleepable sleepable)
	{
		if (sleepable != null)
		{
			sleepables.Remove(sleepable);
		}
	}

	public void SleepAI()
	{
		if (!AI.sleepwake || !ShouldSleepAI)
		{
			return;
		}
		foreach (IAISleepable sleepable in sleepables)
		{
			sleepable?.SleepAI();
		}
		Sleeping = true;
	}

	public void WakeAI()
	{
		foreach (IAISleepable sleepable in sleepables)
		{
			sleepable?.WakeAI();
		}
		Sleeping = false;
	}

	private void AddCoverPoint(AICoverPoint point)
	{
		if (!coverPoints.Contains(point))
		{
			coverPoints.Add(point);
			MarkDirty();
		}
	}

	private void RemoveCoverPoint(AICoverPoint point, bool markDirty = true)
	{
		coverPoints.Remove(point);
		if (markDirty)
		{
			MarkDirty();
		}
	}

	private void AddMovePoint(AIMovePoint point)
	{
		if (!movePoints.Contains(point))
		{
			movePoints.Add(point);
			MarkDirty();
		}
	}

	private void RemoveMovePoint(AIMovePoint point, bool markDirty = true)
	{
		movePoints.Remove(point);
		if (markDirty)
		{
			MarkDirty();
		}
	}

	public void MarkDirty(bool completeRefresh = false)
	{
		isDirty = true;
		processIndex = 0;
		halfPaths = 0;
		pathSuccesses = 0;
		pathFails = 0;
		if (!completeRefresh)
		{
			return;
		}
		Debug.Log((object)"AIInformationZone performing complete refresh, please wait...");
		foreach (AIMovePoint movePoint in movePoints)
		{
			movePoint.distances.Clear();
			movePoint.distancesToCover.Clear();
		}
	}

	private bool PassesBudget(float startTime, float budgetSeconds)
	{
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		if (realtimeSinceStartup - startTime > budgetSeconds)
		{
			return false;
		}
		return true;
	}

	public bool ProcessDistancesAttempt()
	{
		return true;
	}

	private bool ProcessDistances()
	{
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Expected O, but got Unknown
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Invalid comparison between Unknown and I4
		//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		if (!UseCalculatedCoverDistances)
		{
			return true;
		}
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		float budgetSeconds = AIThinkManager.framebudgetms / 1000f * 0.25f;
		if (realtimeSinceStartup < lastNavmeshBuildTime + 60f)
		{
			budgetSeconds = 0.1f;
		}
		int num = 1 << NavMesh.GetAreaFromName("HumanNPC");
		NavMeshPath val = new NavMeshPath();
		while (PassesBudget(realtimeSinceStartup, budgetSeconds))
		{
			AIMovePoint aIMovePoint = movePoints[processIndex];
			bool flag = true;
			Profiler.BeginSample("AIInformationZone - Removal");
			int num2 = 0;
			for (int num3 = aIMovePoint.distances.Keys.Count - 1; num3 >= 0; num3--)
			{
				AIMovePoint aIMovePoint2 = aIMovePoint.distances.Keys[num3];
				if (!movePoints.Contains(aIMovePoint2))
				{
					aIMovePoint.distances.Remove(aIMovePoint2);
				}
			}
			for (int num4 = aIMovePoint.distancesToCover.Keys.Count - 1; num4 >= 0; num4--)
			{
				AICoverPoint aICoverPoint = aIMovePoint.distancesToCover.Keys[num4];
				if (!coverPoints.Contains(aICoverPoint))
				{
					num2++;
					aIMovePoint.distancesToCover.Remove(aICoverPoint);
				}
			}
			Profiler.EndSample();
			foreach (AICoverPoint coverPoint in coverPoints)
			{
				if ((Object)(object)coverPoint == (Object)null || aIMovePoint.distancesToCover.Contains(coverPoint))
				{
					continue;
				}
				float num5 = -1f;
				float num6 = Vector3.Distance(((Component)aIMovePoint).transform.position, ((Component)coverPoint).transform.position);
				if (num6 > 40f)
				{
					num5 = -2f;
				}
				else
				{
					Profiler.BeginSample("AIInformationZoneProcessing-CalculatePath");
					bool flag2 = NavMesh.CalculatePath(((Component)aIMovePoint).transform.position, ((Component)coverPoint).transform.position, num, val) && (int)val.status == 0;
					Profiler.EndSample();
					if (flag2)
					{
						int num7 = val.corners.Length;
						if (num7 > 1)
						{
							Vector3 val2 = val.corners[0];
							float num8 = 0f;
							for (int i = 0; i < num7; i++)
							{
								Vector3 val3 = val.corners[i];
								num8 += Vector3.Distance(val2, val3);
								val2 = val3;
							}
							num5 = num8;
							pathSuccesses++;
						}
						else
						{
							num5 = Vector3.Distance(((Component)aIMovePoint).transform.position, ((Component)coverPoint).transform.position);
							halfPaths++;
						}
					}
					else
					{
						pathFails++;
						num5 = -2f;
					}
				}
				aIMovePoint.distancesToCover.Add(coverPoint, num5);
				if (PassesBudget(realtimeSinceStartup, budgetSeconds))
				{
					continue;
				}
				flag = false;
				break;
			}
			if (flag)
			{
				processIndex++;
			}
			if (processIndex >= movePoints.Count - 1)
			{
				break;
			}
		}
		return processIndex >= movePoints.Count - 1;
	}

	public static void BudgetedTick()
	{
		if (!AI.move || Time.realtimeSinceStartup < buildTimeTest)
		{
			return;
		}
		bool flag = false;
		foreach (AIInformationZone zone in zones)
		{
			if (zone.isDirty)
			{
				flag = true;
				bool flag2 = zone.isDirty;
				zone.isDirty = !zone.ProcessDistancesAttempt();
				break;
			}
		}
		if (Global.developer > 0)
		{
			if (flag && !lastFrameAnyDirty)
			{
				Debug.Log((object)"AIInformationZones rebuilding...");
				rebuildStartTime = Time.realtimeSinceStartup;
			}
			if (lastFrameAnyDirty && !flag)
			{
				Debug.Log((object)("AIInformationZone rebuild complete! Duration : " + (Time.realtimeSinceStartup - rebuildStartTime) + " seconds."));
			}
		}
		lastFrameAnyDirty = flag;
	}

	public void NavmeshBuildingComplete()
	{
		lastNavmeshBuildTime = Time.realtimeSinceStartup;
		buildTimeTest = Time.realtimeSinceStartup + 15f;
		MarkDirty(completeRefresh: true);
	}

	public Vector3 ClosestPointTo(Vector3 target)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		return ((OBB)(ref areaBox)).ClosestPoint(target);
	}

	public void OnDrawGizmos()
	{
	}

	public void OnDrawGizmosSelected()
	{
		DrawBounds();
	}

	private void DrawBounds()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		Gizmos.matrix = ((Component)this).transform.localToWorldMatrix;
		Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
		Gizmos.DrawCube(((Bounds)(ref bounds)).center, ((Bounds)(ref bounds)).size);
	}

	public void AddInitialPoints()
	{
		AICoverPoint[] componentsInChildren = ((Component)((Component)this).transform).GetComponentsInChildren<AICoverPoint>();
		AICoverPoint[] array = componentsInChildren;
		foreach (AICoverPoint point in array)
		{
			AddCoverPoint(point);
		}
		AIMovePoint[] componentsInChildren2 = ((Component)((Component)this).transform).GetComponentsInChildren<AIMovePoint>(true);
		AIMovePoint[] array2 = componentsInChildren2;
		foreach (AIMovePoint point2 in array2)
		{
			AddMovePoint(point2);
		}
		RefreshPointArrays();
		NavMeshLink[] componentsInChildren3 = ((Component)((Component)this).transform).GetComponentsInChildren<NavMeshLink>(true);
		navMeshLinks.AddRange(componentsInChildren3);
		AIMovePointPath[] componentsInChildren4 = ((Component)((Component)this).transform).GetComponentsInChildren<AIMovePointPath>();
		paths.AddRange(componentsInChildren4);
	}

	private void RefreshPointArrays()
	{
		movePointArray = movePoints?.ToArray();
		coverPointArray = coverPoints?.ToArray();
	}

	public void AddDynamicAIPoints(AIMovePoint[] movePoints, AICoverPoint[] coverPoints, Func<Vector3, bool> validatePoint = null)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		if (movePoints != null)
		{
			foreach (AIMovePoint aIMovePoint in movePoints)
			{
				if (!((Object)(object)aIMovePoint == (Object)null) && (validatePoint == null || (validatePoint != null && validatePoint(((Component)aIMovePoint).transform.position))))
				{
					AddMovePoint(aIMovePoint);
				}
			}
		}
		if (coverPoints != null)
		{
			foreach (AICoverPoint aICoverPoint in coverPoints)
			{
				if (!((Object)(object)aICoverPoint == (Object)null) && (validatePoint == null || (validatePoint != null && validatePoint(((Component)aICoverPoint).transform.position))))
				{
					AddCoverPoint(aICoverPoint);
				}
			}
		}
		RefreshPointArrays();
	}

	public void RemoveDynamicAIPoints(AIMovePoint[] movePoints, AICoverPoint[] coverPoints)
	{
		if (movePoints != null)
		{
			foreach (AIMovePoint aIMovePoint in movePoints)
			{
				if (!((Object)(object)aIMovePoint == (Object)null))
				{
					RemoveMovePoint(aIMovePoint, markDirty: false);
				}
			}
		}
		if (coverPoints != null)
		{
			foreach (AICoverPoint aICoverPoint in coverPoints)
			{
				if (!((Object)(object)aICoverPoint == (Object)null))
				{
					RemoveCoverPoint(aICoverPoint, markDirty: false);
				}
			}
		}
		MarkDirty();
		RefreshPointArrays();
	}

	public AIMovePointPath GetNearestPath(Vector3 position)
	{
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		if (paths == null || paths.Count == 0)
		{
			return null;
		}
		float num = float.MaxValue;
		AIMovePointPath result = null;
		foreach (AIMovePointPath path in paths)
		{
			foreach (AIMovePoint point in path.Points)
			{
				float num2 = Vector3.SqrMagnitude(((Component)point).transform.position - position);
				if (num2 < num)
				{
					num = num2;
					result = path;
				}
			}
		}
		return result;
	}

	public static AIInformationZone GetForPoint(Vector3 point, bool fallBackToNearest = true)
	{
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		if (zones == null || zones.Count == 0)
		{
			return null;
		}
		foreach (AIInformationZone zone in zones)
		{
			if ((Object)(object)zone == (Object)null || zone.Virtual || !((OBB)(ref zone.areaBox)).Contains(point))
			{
				continue;
			}
			return zone;
		}
		if (!fallBackToNearest)
		{
			return null;
		}
		float num = float.PositiveInfinity;
		AIInformationZone aIInformationZone = zones[0];
		foreach (AIInformationZone zone2 in zones)
		{
			if (!((Object)(object)zone2 == (Object)null) && !((Object)(object)((Component)zone2).transform == (Object)null) && !zone2.Virtual)
			{
				float num2 = Vector3.Distance(((Component)zone2).transform.position, point);
				if (num2 < num)
				{
					num = num2;
					aIInformationZone = zone2;
				}
			}
		}
		if (aIInformationZone.Virtual)
		{
			aIInformationZone = null;
		}
		return aIInformationZone;
	}

	public bool PointInside(Vector3 point)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return ((OBB)(ref areaBox)).Contains(point);
	}

	public AIMovePoint GetBestMovePointNear(Vector3 targetPosition, Vector3 fromPosition, float minRange, float maxRange, bool checkLOS = false, BaseEntity forObject = null, bool returnClosest = false)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		Profiler.BeginSample("AIInformationZone.GetBestMovePointNear");
		AIPoint aIPoint = null;
		AIPoint aIPoint2 = null;
		float num = -1f;
		float num2 = float.PositiveInfinity;
		int pointCount;
		AIPoint[] movePointsInRange = GetMovePointsInRange(targetPosition, maxRange, out pointCount);
		if (movePointsInRange == null || pointCount <= 0)
		{
			Profiler.EndSample();
			return null;
		}
		for (int i = 0; i < pointCount; i++)
		{
			AIPoint aIPoint3 = movePointsInRange[i];
			if (!((Component)((Component)aIPoint3).transform.parent).gameObject.activeSelf || (!(fromPosition.y < WaterSystem.OceanLevel) && ((Component)aIPoint3).transform.position.y < WaterSystem.OceanLevel))
			{
				continue;
			}
			float num3 = 0f;
			Vector3 position = ((Component)aIPoint3).transform.position;
			float num4 = Vector3.Distance(targetPosition, position);
			if (num4 < num2)
			{
				aIPoint2 = aIPoint3;
				num2 = num4;
			}
			if (!(num4 > maxRange))
			{
				num3 += (aIPoint3.CanBeUsedBy(forObject) ? 100f : 0f);
				num3 += (1f - Mathf.InverseLerp(minRange, maxRange, num4)) * 100f;
				if (!(num3 < num) && (!checkLOS || !Physics.Linecast(targetPosition + Vector3.up * 1f, position + Vector3.up * 1f, 1218519297, (QueryTriggerInteraction)1)) && num3 > num)
				{
					aIPoint = aIPoint3;
					num = num3;
				}
			}
		}
		Profiler.EndSample();
		if ((Object)(object)aIPoint == (Object)null && returnClosest)
		{
			return aIPoint2 as AIMovePoint;
		}
		return aIPoint as AIMovePoint;
	}

	public AIPoint[] GetMovePointsInRange(Vector3 currentPos, float maxRange, out int pointCount)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		pointCount = 0;
		AIMovePoint[] movePointsInRange;
		if ((Object)(object)grid != (Object)null && AI.usegrid)
		{
			movePointsInRange = grid.GetMovePointsInRange(currentPos, maxRange, out pointCount);
		}
		else
		{
			movePointsInRange = movePointArray;
			if (movePointsInRange != null)
			{
				pointCount = movePointsInRange.Length;
			}
		}
		return movePointsInRange;
	}

	private AIMovePoint GetClosestRaw(Vector3 pos, bool onlyIncludeWithCover = false)
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		AIMovePoint result = null;
		float num = float.PositiveInfinity;
		foreach (AIMovePoint movePoint in movePoints)
		{
			if (!onlyIncludeWithCover || movePoint.distancesToCover.Count != 0)
			{
				float num2 = Vector3.Distance(((Component)movePoint).transform.position, pos);
				if (num2 < num)
				{
					num = num2;
					result = movePoint;
				}
			}
		}
		return result;
	}

	public AICoverPoint GetBestCoverPoint(Vector3 currentPosition, Vector3 hideFromPosition, float minRange = 0f, float maxRange = 20f, BaseEntity forObject = null, bool allowObjectToReuse = true)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		Profiler.BeginSample("AIInformationZone.GetBestCoverPoint");
		AICoverPoint aICoverPoint = null;
		float num = 0f;
		AIMovePoint closestRaw = GetClosestRaw(currentPosition, onlyIncludeWithCover: true);
		int pointCount;
		AICoverPoint[] coverPointsInRange = GetCoverPointsInRange(currentPosition, maxRange, out pointCount);
		if (coverPointsInRange == null || pointCount <= 0)
		{
			Profiler.EndSample();
			return null;
		}
		for (int i = 0; i < pointCount; i++)
		{
			AICoverPoint aICoverPoint2 = coverPointsInRange[i];
			Vector3 position = ((Component)aICoverPoint2).transform.position;
			Vector3 val = hideFromPosition - position;
			Vector3 normalized = ((Vector3)(ref val)).normalized;
			float num2 = Vector3.Dot(((Component)aICoverPoint2).transform.forward, normalized);
			if (num2 < 1f - aICoverPoint2.coverDot)
			{
				continue;
			}
			float num3 = -1f;
			if (UseCalculatedCoverDistances && (Object)(object)closestRaw != (Object)null && closestRaw.distancesToCover.Contains(aICoverPoint2) && !isDirty)
			{
				num3 = closestRaw.distancesToCover[aICoverPoint2];
				if (num3 == -2f)
				{
					continue;
				}
			}
			else
			{
				num3 = Vector3.Distance(currentPosition, position);
			}
			float num4 = 0f;
			if (aICoverPoint2.InUse())
			{
				bool flag = aICoverPoint2.IsUsedBy(forObject);
				if (!(allowObjectToReuse && flag))
				{
					num4 -= 1000f;
				}
			}
			if (minRange > 0f)
			{
				num4 -= (1f - Mathf.InverseLerp(0f, minRange, num3)) * 100f;
			}
			float num5 = Mathf.Abs(position.y - currentPosition.y);
			num4 += (1f - Mathf.InverseLerp(1f, 5f, num5)) * 500f;
			num4 += Mathf.InverseLerp(1f - aICoverPoint2.coverDot, 1f, num2) * 50f;
			num4 += (1f - Mathf.InverseLerp(2f, maxRange, num3)) * 100f;
			float num6 = 1f - Mathf.InverseLerp(4f, 10f, Vector3.Distance(currentPosition, hideFromPosition));
			val = ((Component)aICoverPoint2).transform.position - currentPosition;
			Vector3 normalized2 = ((Vector3)(ref val)).normalized;
			float num7 = Vector3.Dot(normalized2, normalized);
			num4 -= Mathf.InverseLerp(-1f, 0.25f, num7) * 50f * num6;
			if (num4 > num)
			{
				aICoverPoint = aICoverPoint2;
				num = num4;
			}
		}
		Profiler.EndSample();
		if (Object.op_Implicit((Object)(object)aICoverPoint))
		{
			return aICoverPoint;
		}
		return null;
	}

	private AICoverPoint[] GetCoverPointsInRange(Vector3 position, float maxRange, out int pointCount)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		pointCount = 0;
		AICoverPoint[] coverPointsInRange;
		if ((Object)(object)grid != (Object)null && AI.usegrid)
		{
			coverPointsInRange = grid.GetCoverPointsInRange(position, maxRange, out pointCount);
		}
		else
		{
			coverPointsInRange = coverPointArray;
			if (coverPointsInRange != null)
			{
				pointCount = coverPointsInRange.Length;
			}
		}
		return coverPointsInRange;
	}

	public NavMeshLink GetClosestNavMeshLink(Vector3 pos)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		NavMeshLink result = null;
		float num = float.PositiveInfinity;
		foreach (NavMeshLink navMeshLink in navMeshLinks)
		{
			Vector3 position = ((Component)navMeshLink).gameObject.transform.position;
			float num2 = Vector3.Distance(position, pos);
			if (num2 < num)
			{
				result = navMeshLink;
				num = num2;
				if (num2 < 0.25f)
				{
					break;
				}
			}
		}
		return result;
	}
}
