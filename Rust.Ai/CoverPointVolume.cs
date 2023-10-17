using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Profiling;

namespace Rust.Ai;

public class CoverPointVolume : MonoBehaviour, IServerComponent
{
	internal enum CoverType
	{
		None,
		Partial,
		Full
	}

	public float DefaultCoverPointScore = 1f;

	public float CoverPointRayLength = 1f;

	public LayerMask CoverLayerMask;

	public Transform BlockerGroup;

	public Transform ManualCoverPointGroup;

	[ServerVar(Help = "cover_point_sample_step_size defines the size of the steps we do horizontally for the cover point volume's cover point generation (smaller steps gives more accurate cover points, but at a higher processing cost). (default: 6.0)")]
	public static float cover_point_sample_step_size = 6f;

	[ServerVar(Help = "cover_point_sample_step_height defines the height of the steps we do vertically for the cover point volume's cover point generation (smaller steps gives more accurate cover points, but at a higher processing cost). (default: 2.0)")]
	public static float cover_point_sample_step_height = 2f;

	public readonly List<CoverPoint> CoverPoints = new List<CoverPoint>();

	private readonly List<CoverPointBlockerVolume> _coverPointBlockers = new List<CoverPointBlockerVolume>();

	private float _dynNavMeshBuildCompletionTime = -1f;

	private int _genAttempts = 0;

	private Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);

	public bool repeat => true;

	public float? ExecuteUpdate(float deltaTime, float nextInterval)
	{
		if (CoverPoints.Count == 0)
		{
			if (_dynNavMeshBuildCompletionTime < 0f)
			{
				if ((Object)(object)SingletonComponent<DynamicNavMesh>.Instance == (Object)null || !((Behaviour)SingletonComponent<DynamicNavMesh>.Instance).enabled || !SingletonComponent<DynamicNavMesh>.Instance.IsBuilding)
				{
					_dynNavMeshBuildCompletionTime = Time.realtimeSinceStartup;
				}
			}
			else if (_genAttempts < 4 && Time.realtimeSinceStartup - _dynNavMeshBuildCompletionTime > 0.25f)
			{
				GenerateCoverPoints(null);
				if (CoverPoints.Count != 0)
				{
					return null;
				}
				_dynNavMeshBuildCompletionTime = Time.realtimeSinceStartup;
				_genAttempts++;
				if (_genAttempts >= 4)
				{
					Object.Destroy((Object)(object)((Component)this).gameObject);
					return null;
				}
			}
		}
		return 1f + Random.value * 2f;
	}

	[ContextMenu("Clear Cover Points")]
	private void ClearCoverPoints()
	{
		CoverPoints.Clear();
		_coverPointBlockers.Clear();
	}

	public Bounds GetBounds()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		Vector3 center = ((Bounds)(ref bounds)).center;
		if (Mathf.Approximately(((Vector3)(ref center)).sqrMagnitude, 0f))
		{
			bounds = new Bounds(((Component)this).transform.position, ((Component)this).transform.localScale);
		}
		return bounds;
	}

	[ContextMenu("Pre-Generate Cover Points")]
	public void PreGenerateCoverPoints()
	{
		GenerateCoverPoints(null);
	}

	[ContextMenu("Convert to Manual Cover Points")]
	public void ConvertToManualCoverPoints()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		foreach (CoverPoint coverPoint in CoverPoints)
		{
			ManualCoverPoint manualCoverPoint = new GameObject("MCP").AddComponent<ManualCoverPoint>();
			((Component)manualCoverPoint).transform.localPosition = Vector3.zero;
			((Component)manualCoverPoint).transform.position = coverPoint.Position;
			manualCoverPoint.Normal = coverPoint.Normal;
			manualCoverPoint.NormalCoverType = coverPoint.NormalCoverType;
			manualCoverPoint.Volume = this;
		}
	}

	public void GenerateCoverPoints(Transform coverPointGroup)
	{
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0355: Unknown result type (might be due to invalid IL or missing references)
		//IL_035c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0328: Unknown result type (might be due to invalid IL or missing references)
		//IL_032f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0301: Unknown result type (might be due to invalid IL or missing references)
		//IL_0308: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_026a: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		Profiler.BeginSample("CoverPointVolume.GenerateCoverPoints");
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		ClearCoverPoints();
		if ((Object)(object)ManualCoverPointGroup == (Object)null)
		{
			ManualCoverPointGroup = coverPointGroup;
		}
		if ((Object)(object)ManualCoverPointGroup == (Object)null)
		{
			ManualCoverPointGroup = ((Component)this).transform;
		}
		if (ManualCoverPointGroup.childCount > 0)
		{
			ManualCoverPoint[] componentsInChildren = ((Component)ManualCoverPointGroup).GetComponentsInChildren<ManualCoverPoint>();
			ManualCoverPoint[] array = componentsInChildren;
			foreach (ManualCoverPoint manualCoverPoint in array)
			{
				CoverPoint item = manualCoverPoint.ToCoverPoint(this);
				CoverPoints.Add(item);
			}
		}
		if (_coverPointBlockers.Count == 0 && (Object)(object)BlockerGroup != (Object)null)
		{
			CoverPointBlockerVolume[] componentsInChildren2 = ((Component)BlockerGroup).GetComponentsInChildren<CoverPointBlockerVolume>();
			if (componentsInChildren2 != null && componentsInChildren2.Length != 0)
			{
				_coverPointBlockers.AddRange(componentsInChildren2);
			}
		}
		if (CoverPoints.Count != 0)
		{
			return;
		}
		Profiler.BeginSample("CoverPointVolume.SamplePosition");
		NavMeshHit val = default(NavMeshHit);
		bool flag = NavMesh.SamplePosition(((Component)this).transform.position, ref val, ((Component)this).transform.localScale.y * cover_point_sample_step_height, -1);
		Profiler.EndSample();
		if (flag)
		{
			Vector3 position = ((Component)this).transform.position;
			Vector3 val2 = ((Component)this).transform.lossyScale * 0.5f;
			Vector3 val3 = default(Vector3);
			NavMeshHit info = default(NavMeshHit);
			for (float num = position.x - val2.x + 1f; num < position.x + val2.x - 1f; num += cover_point_sample_step_size)
			{
				for (float num2 = position.z - val2.z + 1f; num2 < position.z + val2.z - 1f; num2 += cover_point_sample_step_size)
				{
					for (float num3 = position.y - val2.y; num3 < position.y + val2.y; num3 += cover_point_sample_step_height)
					{
						((Vector3)(ref val3))._002Ector(num, num3, num2);
						Profiler.BeginSample("CoverPointVolume.FindClosestEdge");
						bool flag2 = NavMesh.FindClosestEdge(val3, ref info, ((NavMeshHit)(ref val)).mask);
						Profiler.EndSample();
						if (!flag2)
						{
							continue;
						}
						((NavMeshHit)(ref info)).position = new Vector3(((NavMeshHit)(ref info)).position.x, ((NavMeshHit)(ref info)).position.y + 0.5f, ((NavMeshHit)(ref info)).position.z);
						bool flag3 = true;
						foreach (CoverPoint coverPoint2 in CoverPoints)
						{
							Vector3 val4 = coverPoint2.Position - ((NavMeshHit)(ref info)).position;
							if (((Vector3)(ref val4)).sqrMagnitude < cover_point_sample_step_size * cover_point_sample_step_size)
							{
								flag3 = false;
								break;
							}
						}
						if (flag3)
						{
							Profiler.BeginSample("CoverPointVolume.CalculateCoverPoint");
							CoverPoint coverPoint = CalculateCoverPoint(info);
							Profiler.EndSample();
							if (coverPoint != null)
							{
								CoverPoints.Add(coverPoint);
							}
						}
					}
				}
			}
		}
		Profiler.EndSample();
	}

	private CoverPoint CalculateCoverPoint(NavMeshHit info)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		RaycastHit rayHit;
		CoverType coverType = ProvidesCoverInDir(new Ray(((NavMeshHit)(ref info)).position, -((NavMeshHit)(ref info)).normal), CoverPointRayLength, out rayHit);
		if (coverType == CoverType.None)
		{
			return null;
		}
		Profiler.BeginSample("CoverPointVolume.InstantiateCoverPoint");
		CoverPoint coverPoint = new CoverPoint(this, DefaultCoverPointScore)
		{
			Position = ((NavMeshHit)(ref info)).position,
			Normal = -((NavMeshHit)(ref info)).normal
		};
		Profiler.EndSample();
		switch (coverType)
		{
		case CoverType.Full:
			coverPoint.NormalCoverType = CoverPoint.CoverType.Full;
			break;
		case CoverType.Partial:
			coverPoint.NormalCoverType = CoverPoint.CoverType.Partial;
			break;
		}
		return coverPoint;
	}

	internal CoverType ProvidesCoverInDir(Ray ray, float maxDistance, out RaycastHit rayHit)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		Profiler.BeginSample("CoverPointVolume.ProvideCoverInDir");
		rayHit = default(RaycastHit);
		if (Vector3Ex.IsNaNOrInfinity(((Ray)(ref ray)).origin))
		{
			Profiler.EndSample();
			return CoverType.None;
		}
		if (Vector3Ex.IsNaNOrInfinity(((Ray)(ref ray)).direction))
		{
			Profiler.EndSample();
			return CoverType.None;
		}
		if (((Ray)(ref ray)).direction == Vector3.zero)
		{
			Profiler.EndSample();
			return CoverType.None;
		}
		((Ray)(ref ray)).origin = ((Ray)(ref ray)).origin + PlayerEyes.EyeOffset;
		if (Physics.Raycast(((Ray)(ref ray)).origin, ((Ray)(ref ray)).direction, ref rayHit, maxDistance, LayerMask.op_Implicit(CoverLayerMask)))
		{
			Profiler.EndSample();
			return CoverType.Full;
		}
		((Ray)(ref ray)).origin = ((Ray)(ref ray)).origin + PlayerEyes.DuckOffset;
		if (Physics.Raycast(((Ray)(ref ray)).origin, ((Ray)(ref ray)).direction, ref rayHit, maxDistance, LayerMask.op_Implicit(CoverLayerMask)))
		{
			Profiler.EndSample();
			return CoverType.Partial;
		}
		Profiler.EndSample();
		return CoverType.None;
	}

	public bool Contains(Vector3 point)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		Bounds val = default(Bounds);
		((Bounds)(ref val))._002Ector(((Component)this).transform.position, ((Component)this).transform.localScale);
		return ((Bounds)(ref val)).Contains(point);
	}
}
