using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using ConVar;
using Rust;
using Rust.Ai;
using UnityEngine;
using UnityEngine.AI;

public class MonumentNavMesh : FacepunchBehaviour, IServerComponent
{
	public int NavMeshAgentTypeIndex;

	[Tooltip("The default area associated with the NavMeshAgent index.")]
	public string DefaultAreaName = "HumanNPC";

	[Tooltip("How many cells to use squared")]
	public int CellCount = 1;

	[Tooltip("The size of each cell for async object gathering")]
	public int CellSize = 80;

	public int Height = 100;

	public float NavmeshResolutionModifier = 0.5f;

	[Tooltip("Use the bounds specified in editor instead of generating it from cellsize * cellcount")]
	public bool overrideAutoBounds;

	[Tooltip("Bounds which are auto calculated from CellSize * CellCount")]
	public Bounds Bounds;

	public NavMeshData NavMeshData;

	public NavMeshDataInstance NavMeshDataInstance;

	public LayerMask LayerMask;

	public NavMeshCollectGeometry NavMeshCollectGeometry;

	public bool forceCollectTerrain;

	public bool shouldNotifyAIZones = true;

	public Transform CustomNavMeshRoot;

	[ServerVar]
	public static bool use_baked_terrain_mesh = true;

	private List<NavMeshBuildSource> sources;

	private AsyncOperation BuildingOperation;

	private bool HasBuildOperationStarted;

	private Stopwatch BuildTimer = new Stopwatch();

	private int defaultArea;

	private int agentTypeId;

	public bool IsBuilding
	{
		get
		{
			if (!HasBuildOperationStarted || BuildingOperation != null)
			{
				return true;
			}
			return false;
		}
	}

	private void OnEnable()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Expected O, but got Unknown
		NavMeshBuildSettings settingsByIndex = NavMesh.GetSettingsByIndex(NavMeshAgentTypeIndex);
		agentTypeId = ((NavMeshBuildSettings)(ref settingsByIndex)).agentTypeID;
		NavMeshData = new NavMeshData(agentTypeId);
		sources = new List<NavMeshBuildSource>();
		defaultArea = NavMesh.GetAreaFromName(DefaultAreaName);
		((FacepunchBehaviour)this).InvokeRepeating((Action)FinishBuildingNavmesh, 0f, 1f);
	}

	private void OnDisable()
	{
		if (!Application.isQuitting)
		{
			((FacepunchBehaviour)this).CancelInvoke((Action)FinishBuildingNavmesh);
			((NavMeshDataInstance)(ref NavMeshDataInstance)).Remove();
		}
	}

	[ContextMenu("Update Monument Nav Mesh")]
	public void UpdateNavMeshAsync()
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		if (!HasBuildOperationStarted && !AiManager.nav_disable && AI.npc_enable)
		{
			float realtimeSinceStartup = Time.realtimeSinceStartup;
			Debug.Log((object)("Starting Monument Navmesh Build with " + sources.Count + " sources"));
			NavMeshBuildSettings settingsByIndex = NavMesh.GetSettingsByIndex(NavMeshAgentTypeIndex);
			((NavMeshBuildSettings)(ref settingsByIndex)).overrideVoxelSize = true;
			((NavMeshBuildSettings)(ref settingsByIndex)).voxelSize = ((NavMeshBuildSettings)(ref settingsByIndex)).voxelSize * NavmeshResolutionModifier;
			BuildingOperation = NavMeshBuilder.UpdateNavMeshDataAsync(NavMeshData, settingsByIndex, sources, Bounds);
			BuildTimer.Reset();
			BuildTimer.Start();
			HasBuildOperationStarted = true;
			float num = Time.realtimeSinceStartup - realtimeSinceStartup;
			if (num > 0.1f)
			{
				Debug.LogWarning((object)("Calling UpdateNavMesh took " + num));
			}
			if (shouldNotifyAIZones)
			{
				NotifyInformationZonesOfCompletion();
			}
		}
	}

	public IEnumerator UpdateNavMeshAndWait()
	{
		if (HasBuildOperationStarted || AiManager.nav_disable || !AI.npc_enable)
		{
			yield break;
		}
		HasBuildOperationStarted = false;
		((Bounds)(ref Bounds)).center = ((Component)this).transform.position;
		if (!overrideAutoBounds)
		{
			((Bounds)(ref Bounds)).size = new Vector3((float)(CellSize * CellCount), (float)Height, (float)(CellSize * CellCount));
		}
		IEnumerator enumerator = NavMeshTools.CollectSourcesAsync(Bounds, LayerMask.op_Implicit(LayerMask), NavMeshCollectGeometry, defaultArea, use_baked_terrain_mesh && !forceCollectTerrain, CellSize, sources, AppendModifierVolumes, UpdateNavMeshAsync, CustomNavMeshRoot);
		if (AiManager.nav_wait)
		{
			yield return enumerator;
		}
		else
		{
			((MonoBehaviour)this).StartCoroutine(enumerator);
		}
		if (!AiManager.nav_wait)
		{
			Debug.Log((object)"nav_wait is false, so we're not waiting for the navmesh to finish generating. This might cause your server to sputter while it's generating.");
			yield break;
		}
		int lastPct = 0;
		while (!HasBuildOperationStarted)
		{
			yield return CoroutineEx.waitForSecondsRealtime(0.25f);
		}
		while (BuildingOperation != null)
		{
			int num = (int)(BuildingOperation.progress * 100f);
			if (lastPct != num)
			{
				Debug.LogFormat("{0}%", new object[1] { num });
				lastPct = num;
			}
			yield return CoroutineEx.waitForSecondsRealtime(0.25f);
			FinishBuildingNavmesh();
		}
	}

	public void NotifyInformationZonesOfCompletion()
	{
		foreach (AIInformationZone zone in AIInformationZone.zones)
		{
			zone.NavmeshBuildingComplete();
		}
	}

	private void AppendModifierVolumes(List<NavMeshBuildSource> sources)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		Vector3 size = default(Vector3);
		foreach (NavMeshModifierVolume activeModifier in NavMeshModifierVolume.activeModifiers)
		{
			if ((LayerMask.op_Implicit(LayerMask) & (1 << ((Component)activeModifier).gameObject.layer)) != 0 && activeModifier.AffectsAgentType(agentTypeId))
			{
				Vector3 val = ((Component)activeModifier).transform.TransformPoint(activeModifier.center);
				if (((Bounds)(ref Bounds)).Contains(val))
				{
					Vector3 lossyScale = ((Component)activeModifier).transform.lossyScale;
					((Vector3)(ref size))._002Ector(activeModifier.size.x * Mathf.Abs(lossyScale.x), activeModifier.size.y * Mathf.Abs(lossyScale.y), activeModifier.size.z * Mathf.Abs(lossyScale.z));
					NavMeshBuildSource item = default(NavMeshBuildSource);
					((NavMeshBuildSource)(ref item)).shape = (NavMeshBuildSourceShape)5;
					((NavMeshBuildSource)(ref item)).transform = Matrix4x4.TRS(val, ((Component)activeModifier).transform.rotation, Vector3.one);
					((NavMeshBuildSource)(ref item)).size = size;
					((NavMeshBuildSource)(ref item)).area = activeModifier.area;
					sources.Add(item);
				}
			}
		}
	}

	public void FinishBuildingNavmesh()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		if (BuildingOperation != null && BuildingOperation.isDone)
		{
			if (!((NavMeshDataInstance)(ref NavMeshDataInstance)).valid)
			{
				NavMeshDataInstance = NavMesh.AddNavMeshData(NavMeshData);
			}
			Debug.Log((object)$"Monument Navmesh Build took {BuildTimer.Elapsed.TotalSeconds:0.00} seconds");
			BuildingOperation = null;
		}
	}

	public void OnDrawGizmosSelected()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		Gizmos.color = Color.magenta * new Color(1f, 1f, 1f, 0.5f);
		Gizmos.DrawCube(((Component)this).transform.position + ((Bounds)(ref Bounds)).center, ((Bounds)(ref Bounds)).size);
	}
}
