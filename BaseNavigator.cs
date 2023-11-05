using System;
using System.Collections.Generic;
using ConVar;
using Rust.AI;
using Rust.Ai;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Profiling;

public class BaseNavigator : BaseMonoBehaviour
{
	public enum NavigationType
	{
		None,
		NavMesh,
		AStar,
		Custom,
		Base
	}

	public enum NavigationSpeed
	{
		Slowest,
		Slow,
		Normal,
		Fast
	}

	protected enum OverrideFacingDirectionMode
	{
		None,
		Direction,
		Entity
	}

	[ServerVar(Help = "The max step-up height difference for pet base navigation")]
	public static float maxStepUpDistance = 1.7f;

	[ServerVar(Help = "How many frames between base navigation movement updates")]
	public static int baseNavMovementFrameInterval = 2;

	[ServerVar(Help = "How long we are not moving for before trigger the stuck event")]
	public static float stuckTriggerDuration = 10f;

	[ServerVar]
	public static float navTypeHeightOffset = 0.5f;

	[ServerVar]
	public static float navTypeDistance = 1f;

	[Header("General")]
	public bool CanNavigateMounted = false;

	public bool CanUseNavMesh = true;

	public bool CanUseAStar = true;

	public bool CanUseBaseNav = false;

	public bool CanUseCustomNav = false;

	public float StoppingDistance = 0.5f;

	public string DefaultArea = "Walkable";

	[Header("Stuck Detection")]
	public bool TriggerStuckEvent = false;

	public float StuckDistance = 1f;

	[Header("Speed")]
	public float Speed = 5f;

	public float Acceleration = 5f;

	public float TurnSpeed = 10f;

	public NavigationSpeed MoveTowardsSpeed = NavigationSpeed.Normal;

	public bool FaceMoveTowardsTarget = false;

	[Header("Speed Fractions")]
	public float SlowestSpeedFraction = 0.16f;

	public float SlowSpeedFraction = 0.3f;

	public float NormalSpeedFraction = 0.5f;

	public float FastSpeedFraction = 1f;

	public float LowHealthSpeedReductionTriggerFraction = 0f;

	public float LowHealthMaxSpeedFraction = 0.5f;

	public float SwimmingSpeedMultiplier = 0.25f;

	[Header("AIPoint Usage")]
	public float BestMovementPointMaxDistance = 10f;

	public float BestCoverPointMaxDistance = 20f;

	public float BestRoamPointMaxDistance = 20f;

	public float MaxRoamDistanceFromHome = -1f;

	[Header("Misc")]
	public float MaxWaterDepth = 0.75f;

	public bool SpeedBasedAvoidancePriority = false;

	private NavMeshPath path;

	private NavMeshQueryFilter navMeshQueryFilter;

	private int defaultAreaMask;

	[InspectorFlags]
	public Enum biomePreference = (Enum)12;

	public bool UseBiomePreference = false;

	[InspectorFlags]
	public Enum topologyPreference = (Enum)96;

	[InspectorFlags]
	public Enum topologyPrevent = (Enum)0;

	[InspectorFlags]
	public Enum biomeRequirement = (Enum)0;

	private float stuckTimer = 0f;

	private Vector3 stuckCheckPosition;

	protected bool traversingNavMeshLink = false;

	protected string currentNavMeshLinkName;

	protected Vector3 currentNavMeshLinkEndPos;

	protected Stack<IAIPathNode> currentAStarPath;

	protected IAIPathNode targetNode;

	protected float currentSpeedFraction = 1f;

	private float lastSetDestinationTime = 0f;

	protected OverrideFacingDirectionMode overrideFacingDirectionMode = OverrideFacingDirectionMode.None;

	protected BaseEntity facingDirectionEntity;

	protected bool overrideFacingDirection;

	protected Vector3 facingDirectionOverride;

	protected bool paused = false;

	private int frameCount = 0;

	private float accumDelta = 0f;

	public AIMovePointPath Path { get; set; }

	public BasePath AStarGraph { get; set; }

	public NavMeshAgent Agent { get; private set; }

	public BaseCombatEntity BaseEntity { get; private set; }

	public Vector3 Destination { get; protected set; }

	public virtual bool IsOnNavMeshLink
	{
		get
		{
			if (((Behaviour)Agent).enabled)
			{
				return Agent.isOnOffMeshLink;
			}
			return false;
		}
	}

	public bool Moving => CurrentNavigationType != NavigationType.None;

	public NavigationType CurrentNavigationType { get; private set; }

	public NavigationType LastUsedNavigationType { get; private set; }

	[HideInInspector]
	public bool StuckOffNavmesh { get; private set; }

	public virtual bool HasPath
	{
		get
		{
			if ((Object)(object)Agent == (Object)null)
			{
				return false;
			}
			if (((Behaviour)Agent).enabled && Agent.hasPath)
			{
				return true;
			}
			if (currentAStarPath != null)
			{
				return true;
			}
			return false;
		}
	}

	public bool IsOverridingFacingDirection => overrideFacingDirectionMode != OverrideFacingDirectionMode.None;

	public Vector3 FacingDirectionOverride => facingDirectionOverride;

	public int TopologyPreference()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Expected I4, but got Unknown
		return (int)topologyPreference;
	}

	public int TopologyPrevent()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Expected I4, but got Unknown
		return (int)topologyPrevent;
	}

	public int BiomeRequirement()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Expected I4, but got Unknown
		return (int)biomeRequirement;
	}

	public virtual void Init(BaseCombatEntity entity, NavMeshAgent agent)
	{
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Expected O, but got Unknown
		defaultAreaMask = 1 << NavMesh.GetAreaFromName(DefaultArea);
		BaseEntity = entity;
		Agent = agent;
		if ((Object)(object)Agent != (Object)null)
		{
			Agent.acceleration = Acceleration;
			Agent.angularSpeed = TurnSpeed;
		}
		navMeshQueryFilter = default(NavMeshQueryFilter);
		((NavMeshQueryFilter)(ref navMeshQueryFilter)).agentTypeID = Agent.agentTypeID;
		((NavMeshQueryFilter)(ref navMeshQueryFilter)).areaMask = defaultAreaMask;
		path = new NavMeshPath();
		SetCurrentNavigationType(NavigationType.None);
	}

	public void SetNavMeshEnabled(bool flag)
	{
		if ((Object)(object)Agent == (Object)null || ((Behaviour)Agent).enabled == flag)
		{
			return;
		}
		if (AiManager.nav_disable)
		{
			((Behaviour)Agent).enabled = false;
			return;
		}
		Profiler.BeginSample("BaseNavigator.SetNavMeshEnabled");
		if (((Behaviour)Agent).enabled)
		{
			if (flag)
			{
				if (Agent.isOnNavMesh)
				{
					Agent.isStopped = false;
				}
			}
			else if (Agent.isOnNavMesh)
			{
				Agent.isStopped = true;
			}
		}
		((Behaviour)Agent).enabled = flag;
		if (flag)
		{
			if (!CanEnableNavMeshNavigation())
			{
				Profiler.EndSample();
				return;
			}
			PlaceOnNavMesh();
		}
		Profiler.EndSample();
	}

	protected virtual bool CanEnableNavMeshNavigation()
	{
		if (!CanUseNavMesh)
		{
			return false;
		}
		return true;
	}

	protected virtual bool CanUpdateMovement()
	{
		if ((Object)(object)BaseEntity != (Object)null && !BaseEntity.IsAlive())
		{
			return false;
		}
		return true;
	}

	public void ForceToGround()
	{
		((FacepunchBehaviour)this).CancelInvoke((Action)DelayedForceToGround);
		((FacepunchBehaviour)this).Invoke((Action)DelayedForceToGround, 0.5f);
	}

	private void DelayedForceToGround()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		int num = 10551296;
		RaycastHit val = default(RaycastHit);
		if (Physics.Raycast(((Component)this).transform.position + Vector3.up * 0.5f, Vector3.down, ref val, 1000f, num))
		{
			BaseEntity.ServerPosition = ((RaycastHit)(ref val)).point;
		}
	}

	public bool PlaceOnNavMesh()
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		if (Agent.isOnNavMesh)
		{
			return true;
		}
		Profiler.BeginSample("BaseNavigator.PlaceOnNavMesh");
		bool flag = false;
		float maxRange = (IsSwimming() ? 30f : 6f);
		if (GetNearestNavmeshPosition(((Component)this).transform.position + Vector3.one * 2f, out var position, maxRange))
		{
			flag = Warp(position);
			if (flag)
			{
				OnPlacedOnNavmesh();
			}
		}
		if (!flag)
		{
			StuckOffNavmesh = true;
			OnFailedToPlaceOnNavmesh();
		}
		Profiler.EndSample();
		return flag;
	}

	public virtual void OnPlacedOnNavmesh()
	{
	}

	public virtual void OnFailedToPlaceOnNavmesh()
	{
	}

	private bool Warp(Vector3 position)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		Agent.Warp(position);
		((Behaviour)Agent).enabled = true;
		((Component)this).transform.position = position;
		if (!Agent.isOnNavMesh)
		{
			Debug.LogWarning((object)("Agent still not on navmesh after a warp. No navmesh areas matching agent type? Agent type: " + Agent.agentTypeID), (Object)(object)((Component)this).gameObject);
			StuckOffNavmesh = true;
			return false;
		}
		StuckOffNavmesh = false;
		return true;
	}

	public bool GetNearestNavmeshPosition(Vector3 target, out Vector3 position, float maxRange)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		Profiler.BeginSample("BaseNavigator.GetNearestNavmeshPosition");
		position = ((Component)this).transform.position;
		bool result = true;
		NavMeshHit val = default(NavMeshHit);
		if (NavMesh.SamplePosition(target, ref val, maxRange, defaultAreaMask))
		{
			position = ((NavMeshHit)(ref val)).position;
		}
		else
		{
			result = false;
		}
		Profiler.EndSample();
		return result;
	}

	public bool SetBaseDestination(Vector3 pos, float speedFraction)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		if (!AI.move)
		{
			return false;
		}
		if (!AI.navthink)
		{
			return false;
		}
		paused = false;
		currentSpeedFraction = speedFraction;
		if (ReachedPosition(pos))
		{
			return true;
		}
		Destination = pos;
		SetCurrentNavigationType(NavigationType.Base);
		return true;
	}

	public bool SetDestination(BasePath path, IAIPathNode newTargetNode, float speedFraction)
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		if (!AI.move)
		{
			return false;
		}
		if (!AI.navthink)
		{
			return false;
		}
		paused = false;
		if (!CanUseAStar)
		{
			return false;
		}
		if (newTargetNode == targetNode && HasPath)
		{
			return true;
		}
		if (ReachedPosition(newTargetNode.Position))
		{
			return true;
		}
		IAIPathNode closestToPoint = path.GetClosestToPoint(((Component)this).transform.position);
		if (closestToPoint == null || !closestToPoint.IsValid())
		{
			return false;
		}
		if (AStarPath.FindPath(closestToPoint, newTargetNode, out currentAStarPath, out var _))
		{
			currentSpeedFraction = speedFraction;
			targetNode = newTargetNode;
			SetCurrentNavigationType(NavigationType.AStar);
			Destination = newTargetNode.Position;
			return true;
		}
		return false;
	}

	public bool SetDestination(Vector3 pos, NavigationSpeed speed, float updateInterval = 0f, float navmeshSampleDistance = 0f)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return SetDestination(pos, GetSpeedFraction(speed), updateInterval, navmeshSampleDistance);
	}

	protected virtual bool SetCustomDestination(Vector3 pos, float speedFraction = 1f, float updateInterval = 0f)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		if (!AI.move)
		{
			return false;
		}
		if (!AI.navthink)
		{
			return false;
		}
		if (!CanUseCustomNav)
		{
			return false;
		}
		paused = false;
		if (ReachedPosition(pos))
		{
			return true;
		}
		currentSpeedFraction = speedFraction;
		SetCurrentNavigationType(NavigationType.Custom);
		return true;
	}

	public bool SetDestination(Vector3 pos, float speedFraction = 1f, float updateInterval = 0f, float navmeshSampleDistance = 0f)
	{
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_034f: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_0379: Unknown result type (might be due to invalid IL or missing references)
		//IL_037e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0441: Unknown result type (might be due to invalid IL or missing references)
		//IL_03db: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0429: Unknown result type (might be due to invalid IL or missing references)
		if (!AI.move)
		{
			return false;
		}
		if (!AI.navthink)
		{
			return false;
		}
		if (updateInterval > 0f && !UpdateIntervalElapsed(updateInterval))
		{
			return true;
		}
		lastSetDestinationTime = Time.time;
		paused = false;
		currentSpeedFraction = speedFraction;
		if (ReachedPosition(pos))
		{
			return true;
		}
		Profiler.BeginSample("BaseNavigator.SetDestination");
		NavigationType navigationType = NavigationType.NavMesh;
		bool flag = CanUseBaseNav && CanUseNavMesh;
		NavigationType navigationType2 = NavigationType.None;
		if (flag)
		{
			Profiler.BeginSample("BaseNavigator.SetDestination.DynamicNav");
			Vector3 navMeshPos;
			NavigationType navigationType3 = DetermineNavigationType(((Component)this).transform.position, out navMeshPos);
			navigationType2 = DetermineNavigationType(pos, out var _);
			if (navigationType2 == NavigationType.NavMesh && navigationType3 == NavigationType.NavMesh && (CurrentNavigationType == NavigationType.None || CurrentNavigationType == NavigationType.Base))
			{
				Warp(navMeshPos);
			}
			if (navigationType2 == NavigationType.Base && navigationType3 != NavigationType.Base)
			{
				BasePet basePet = BaseEntity as BasePet;
				if ((Object)(object)basePet != (Object)null)
				{
					BasePlayer basePlayer = basePet.Brain.Events.Memory.Entity.Get(5) as BasePlayer;
					if ((Object)(object)basePlayer != (Object)null)
					{
						BuildingPrivlidge buildingPrivilege = basePlayer.GetBuildingPrivilege(new OBB(pos, ((Component)this).transform.rotation, BaseEntity.bounds));
						if ((Object)(object)buildingPrivilege != (Object)null && !buildingPrivilege.IsAuthed(basePlayer) && buildingPrivilege.AnyAuthed())
						{
							Profiler.EndSample();
							Profiler.EndSample();
							return false;
						}
					}
				}
			}
			switch (navigationType2)
			{
			case NavigationType.Base:
				if (navigationType3 != NavigationType.Base)
				{
					float num = Vector3.Distance(BaseEntity.ServerPosition, pos);
					navigationType = ((!(num <= 10f) || !(Mathf.Abs(BaseEntity.ServerPosition.y - pos.y) <= 3f)) ? NavigationType.NavMesh : NavigationType.Base);
				}
				else
				{
					navigationType = NavigationType.Base;
				}
				break;
			case NavigationType.NavMesh:
				navigationType = ((navigationType3 == NavigationType.NavMesh) ? NavigationType.NavMesh : NavigationType.Base);
				break;
			}
			Profiler.EndSample();
		}
		else
		{
			navigationType = (CanUseNavMesh ? NavigationType.NavMesh : NavigationType.AStar);
		}
		switch (navigationType)
		{
		case NavigationType.Base:
			Profiler.EndSample();
			return SetBaseDestination(pos, speedFraction);
		case NavigationType.AStar:
			if ((Object)(object)AStarGraph != (Object)null)
			{
				Profiler.EndSample();
				return SetDestination(AStarGraph, AStarGraph.GetClosestToPoint(pos), speedFraction);
			}
			if (CanUseCustomNav)
			{
				Profiler.EndSample();
				return SetCustomDestination(pos, speedFraction, updateInterval);
			}
			Profiler.EndSample();
			return false;
		default:
		{
			if (AiManager.nav_disable)
			{
				Profiler.EndSample();
				return false;
			}
			if (navmeshSampleDistance > 0f && AI.setdestinationsamplenavmesh)
			{
				NavMeshHit val = default(NavMeshHit);
				if (!NavMesh.SamplePosition(pos, ref val, navmeshSampleDistance, defaultAreaMask))
				{
					Profiler.EndSample();
					return false;
				}
				pos = ((NavMeshHit)(ref val)).position;
			}
			SetCurrentNavigationType(NavigationType.NavMesh);
			if (!Agent.isOnNavMesh)
			{
				return false;
			}
			if (!((Behaviour)Agent).isActiveAndEnabled)
			{
				return false;
			}
			Destination = pos;
			bool flag2;
			if (AI.usecalculatepath)
			{
				flag2 = NavMesh.CalculatePath(((Component)this).transform.position, Destination, navMeshQueryFilter, path);
				if (flag2)
				{
					Agent.SetPath(path);
				}
				else if (AI.usesetdestinationfallback)
				{
					flag2 = Agent.SetDestination(Destination);
				}
			}
			else
			{
				flag2 = Agent.SetDestination(Destination);
			}
			if (flag2 && SpeedBasedAvoidancePriority)
			{
				Agent.avoidancePriority = Random.Range(0, 21) + Mathf.FloorToInt(speedFraction * 80f);
			}
			Profiler.EndSample();
			return flag2;
		}
		}
	}

	private NavigationType DetermineNavigationType(Vector3 location, out Vector3 navMeshPos)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		Profiler.BeginSample("BaseNavigator.DetermineNavigationType");
		navMeshPos = location;
		int num = 2097152;
		RaycastHit val = default(RaycastHit);
		if (Physics.Raycast(location + Vector3.up * navTypeHeightOffset, Vector3.down, ref val, navTypeDistance, num))
		{
			Profiler.EndSample();
			return NavigationType.Base;
		}
		Vector3 position;
		NavigationType result = (GetNearestNavmeshPosition(location + Vector3.up * navTypeHeightOffset, out position, navTypeDistance) ? NavigationType.NavMesh : NavigationType.Base);
		navMeshPos = position;
		Profiler.EndSample();
		return result;
	}

	public void SetCurrentSpeed(NavigationSpeed speed)
	{
		currentSpeedFraction = GetSpeedFraction(speed);
	}

	public bool UpdateIntervalElapsed(float updateInterval)
	{
		if (updateInterval <= 0f)
		{
			return true;
		}
		return Time.time - lastSetDestinationTime >= updateInterval;
	}

	public float GetSpeedFraction(NavigationSpeed speed)
	{
		return speed switch
		{
			NavigationSpeed.Fast => FastSpeedFraction, 
			NavigationSpeed.Normal => NormalSpeedFraction, 
			NavigationSpeed.Slow => SlowSpeedFraction, 
			NavigationSpeed.Slowest => SlowestSpeedFraction, 
			_ => 1f, 
		};
	}

	protected void SetCurrentNavigationType(NavigationType navType)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		Profiler.BeginSample("BaseNavigator.SetCurrentNavigationType");
		if (CurrentNavigationType == NavigationType.None)
		{
			stuckCheckPosition = ((Component)this).transform.position;
			stuckTimer = 0f;
		}
		CurrentNavigationType = navType;
		if (CurrentNavigationType != 0)
		{
			LastUsedNavigationType = CurrentNavigationType;
		}
		switch (navType)
		{
		case NavigationType.None:
			stuckTimer = 0f;
			break;
		case NavigationType.NavMesh:
			SetNavMeshEnabled(flag: true);
			break;
		}
		Profiler.EndSample();
	}

	public void Pause()
	{
		if (CurrentNavigationType != 0)
		{
			Stop();
			paused = true;
		}
	}

	public void Resume()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		if (paused)
		{
			SetDestination(Destination, currentSpeedFraction);
			paused = false;
		}
	}

	public void Stop()
	{
		Profiler.BeginSample("BaseNavigator.Stop");
		switch (CurrentNavigationType)
		{
		case NavigationType.AStar:
			StopAStar();
			break;
		case NavigationType.NavMesh:
			StopNavMesh();
			break;
		case NavigationType.Custom:
			StopCustom();
			break;
		}
		SetCurrentNavigationType(NavigationType.None);
		paused = false;
		Profiler.EndSample();
	}

	private void StopNavMesh()
	{
		SetNavMeshEnabled(flag: false);
	}

	private void StopAStar()
	{
		currentAStarPath = null;
		targetNode = null;
	}

	protected virtual void StopCustom()
	{
	}

	public void Think(float delta)
	{
		if (AI.move && AI.navthink && !((Object)(object)BaseEntity == (Object)null))
		{
			UpdateNavigation(delta);
		}
	}

	public void UpdateNavigation(float delta)
	{
		UpdateMovement(delta);
	}

	private void UpdateMovement(float delta)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		if (!AI.move || !CanUpdateMovement())
		{
			return;
		}
		Profiler.BeginSample("BaseNavigator.UpdateMovement");
		Vector3 moveToPosition = ((Component)this).transform.position;
		if (TriggerStuckEvent)
		{
			stuckTimer += delta;
			if (CurrentNavigationType != 0 && stuckTimer >= stuckTriggerDuration)
			{
				if (Vector3.Distance(((Component)this).transform.position, stuckCheckPosition) <= StuckDistance)
				{
					OnStuck();
				}
				stuckTimer = 0f;
				stuckCheckPosition = ((Component)this).transform.position;
			}
		}
		if (CurrentNavigationType == NavigationType.Base)
		{
			moveToPosition = Destination;
		}
		else if (IsOnNavMeshLink)
		{
			HandleNavMeshLinkTraversal(delta, ref moveToPosition);
		}
		else if (HasPath)
		{
			moveToPosition = GetNextPathPosition();
		}
		else if (CurrentNavigationType == NavigationType.Custom)
		{
			moveToPosition = Destination;
		}
		if (!ValidateNextPosition(ref moveToPosition))
		{
			Profiler.EndSample();
			return;
		}
		bool swimming = IsSwimming();
		UpdateSpeed(delta, swimming);
		UpdatePositionAndRotation(moveToPosition, delta);
		Profiler.EndSample();
	}

	public virtual void OnStuck()
	{
		BasePet basePet = BaseEntity as BasePet;
		if ((Object)(object)basePet != (Object)null && (Object)(object)basePet.Brain != (Object)null)
		{
			basePet.Brain.LoadDefaultAIDesign();
		}
	}

	public virtual bool IsSwimming()
	{
		return false;
	}

	private Vector3 GetNextPathPosition()
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		if (currentAStarPath != null && currentAStarPath.Count > 0)
		{
			return currentAStarPath.Peek().Position;
		}
		return Agent.nextPosition;
	}

	private bool ValidateNextPosition(ref Vector3 moveToPosition)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		Profiler.BeginSample("ValidBounds.Test");
		bool flag = ValidBounds.Test(moveToPosition);
		Profiler.EndSample();
		if ((Object)(object)BaseEntity != (Object)null && !flag && (Object)(object)((Component)this).transform != (Object)null && !BaseEntity.IsDestroyed)
		{
			Debug.Log((object)string.Concat("Invalid NavAgent Position: ", this, " ", ((object)(Vector3)(ref moveToPosition)).ToString(), " (destroying)"));
			BaseEntity.Kill();
			return false;
		}
		return true;
	}

	private void UpdateSpeed(float delta, bool swimming)
	{
		float num = GetTargetSpeed();
		if (LowHealthSpeedReductionTriggerFraction > 0f && BaseEntity.healthFraction <= LowHealthSpeedReductionTriggerFraction)
		{
			num = Mathf.Min(num, Speed * LowHealthMaxSpeedFraction);
		}
		Agent.speed = num * (swimming ? SwimmingSpeedMultiplier : 1f);
	}

	protected virtual float GetTargetSpeed()
	{
		return Speed * currentSpeedFraction;
	}

	protected virtual void UpdatePositionAndRotation(Vector3 moveToPosition, float delta)
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0315: Unknown result type (might be due to invalid IL or missing references)
		//IL_031e: Unknown result type (might be due to invalid IL or missing references)
		Profiler.BeginSample("BaseNavigator.UpdatePositionAndRotation");
		if (CurrentNavigationType == NavigationType.AStar && currentAStarPath != null && currentAStarPath.Count > 0)
		{
			((Component)this).transform.position = Vector3.MoveTowards(((Component)this).transform.position, moveToPosition, Agent.speed * delta);
			BaseEntity.ServerPosition = ((Component)this).transform.localPosition;
			if (ReachedPosition(moveToPosition))
			{
				currentAStarPath.Pop();
				if (currentAStarPath.Count == 0)
				{
					Stop();
					Profiler.EndSample();
					return;
				}
				moveToPosition = currentAStarPath.Peek().Position;
			}
		}
		if (CurrentNavigationType == NavigationType.NavMesh)
		{
			if (ReachedPosition(Agent.destination))
			{
				Stop();
			}
			if ((Object)(object)BaseEntity != (Object)null)
			{
				BaseEntity.ServerPosition = moveToPosition;
			}
		}
		if (CurrentNavigationType == NavigationType.Base)
		{
			frameCount++;
			accumDelta += delta;
			if (frameCount < baseNavMovementFrameInterval)
			{
				Profiler.EndSample();
				return;
			}
			frameCount = 0;
			delta = accumDelta;
			accumDelta = 0f;
			Profiler.BeginSample("BaseNavigator.UpdatePositionAndRotation.BaseNav");
			int num = 10551552;
			Vector3 val = Vector3Ex.Direction2D(Destination, BaseEntity.ServerPosition);
			Vector3 val2 = BaseEntity.ServerPosition + val * delta * Agent.speed;
			Vector3 val3 = BaseEntity.ServerPosition + Vector3.up * maxStepUpDistance;
			Vector3 val4 = Vector3Ex.Direction(val2 + Vector3.up * maxStepUpDistance, BaseEntity.ServerPosition + Vector3.up * maxStepUpDistance);
			float num2 = Vector3.Distance(val3, val2 + Vector3.up * maxStepUpDistance) + 0.25f;
			RaycastHit val5 = default(RaycastHit);
			if (Physics.Raycast(val3, val4, ref val5, num2, num))
			{
				Profiler.EndSample();
				Profiler.EndSample();
				return;
			}
			Vector3 val6 = val2 + Vector3.up * (maxStepUpDistance + 0.3f);
			Vector3 val7 = val2;
			if (!Physics.SphereCast(val6, 0.25f, Vector3.down, ref val5, 10f, num))
			{
				Profiler.EndSample();
				Profiler.EndSample();
				return;
			}
			val7 = ((RaycastHit)(ref val5)).point;
			if (val7.y - BaseEntity.ServerPosition.y > maxStepUpDistance)
			{
				Profiler.EndSample();
				Profiler.EndSample();
				return;
			}
			BaseEntity.ServerPosition = val7;
			if (ReachedPosition(moveToPosition))
			{
				Stop();
			}
			Profiler.EndSample();
		}
		if (overrideFacingDirectionMode != 0)
		{
			ApplyFacingDirectionOverride();
		}
		Profiler.EndSample();
	}

	public virtual void ApplyFacingDirectionOverride()
	{
	}

	public void SetFacingDirectionEntity(BaseEntity entity)
	{
		overrideFacingDirectionMode = OverrideFacingDirectionMode.Entity;
		facingDirectionEntity = entity;
	}

	public void SetFacingDirectionOverride(Vector3 direction)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		overrideFacingDirectionMode = OverrideFacingDirectionMode.Direction;
		overrideFacingDirection = true;
		facingDirectionOverride = direction;
	}

	public void ClearFacingDirectionOverride()
	{
		overrideFacingDirectionMode = OverrideFacingDirectionMode.None;
		overrideFacingDirection = false;
		facingDirectionEntity = null;
	}

	protected bool ReachedPosition(Vector3 position)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		return Vector3.Distance(position, ((Component)this).transform.position) <= StoppingDistance;
	}

	private void HandleNavMeshLinkTraversal(float delta, ref Vector3 moveToPosition)
	{
		if (!traversingNavMeshLink)
		{
			HandleNavMeshLinkTraversalStart(delta);
		}
		HandleNavMeshLinkTraversalTick(delta, ref moveToPosition);
		if (IsNavMeshLinkTraversalComplete(delta, ref moveToPosition))
		{
			CompleteNavMeshLink();
		}
	}

	private bool HandleNavMeshLinkTraversalStart(float delta)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		OffMeshLinkData currentOffMeshLinkData = Agent.currentOffMeshLinkData;
		if (!((OffMeshLinkData)(ref currentOffMeshLinkData)).valid || !((OffMeshLinkData)(ref currentOffMeshLinkData)).activated)
		{
			return false;
		}
		Vector3 val = ((OffMeshLinkData)(ref currentOffMeshLinkData)).endPos - ((OffMeshLinkData)(ref currentOffMeshLinkData)).startPos;
		Vector3 normalized = ((Vector3)(ref val)).normalized;
		normalized.y = 0f;
		Vector3 desiredVelocity = Agent.desiredVelocity;
		desiredVelocity.y = 0f;
		float num = Vector3.Dot(desiredVelocity, normalized);
		if (num < 0.1f)
		{
			CompleteNavMeshLink();
			return false;
		}
		OffMeshLinkType linkType = ((OffMeshLinkData)(ref currentOffMeshLinkData)).linkType;
		currentNavMeshLinkName = ((object)(OffMeshLinkType)(ref linkType)).ToString();
		Vector3 val2 = (((Object)(object)BaseEntity != (Object)null) ? BaseEntity.ServerPosition : ((Component)this).transform.position);
		val = val2 - ((OffMeshLinkData)(ref currentOffMeshLinkData)).startPos;
		float sqrMagnitude = ((Vector3)(ref val)).sqrMagnitude;
		val = val2 - ((OffMeshLinkData)(ref currentOffMeshLinkData)).endPos;
		if (sqrMagnitude > ((Vector3)(ref val)).sqrMagnitude)
		{
			currentNavMeshLinkEndPos = ((OffMeshLinkData)(ref currentOffMeshLinkData)).startPos;
		}
		else
		{
			currentNavMeshLinkEndPos = ((OffMeshLinkData)(ref currentOffMeshLinkData)).endPos;
		}
		traversingNavMeshLink = true;
		Agent.ActivateCurrentOffMeshLink(false);
		Agent.obstacleAvoidanceType = (ObstacleAvoidanceType)0;
		if (currentNavMeshLinkName == "OpenDoorLink" || currentNavMeshLinkName == "JumpRockLink" || currentNavMeshLinkName == "JumpFoundationLink")
		{
		}
		return true;
	}

	private void HandleNavMeshLinkTraversalTick(float delta, ref Vector3 moveToPosition)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		if (currentNavMeshLinkName == "OpenDoorLink")
		{
			moveToPosition = Vector3.MoveTowards(moveToPosition, currentNavMeshLinkEndPos, Agent.speed * delta);
		}
		else if (currentNavMeshLinkName == "JumpRockLink")
		{
			moveToPosition = Vector3.MoveTowards(moveToPosition, currentNavMeshLinkEndPos, Agent.speed * delta);
		}
		else if (currentNavMeshLinkName == "JumpFoundationLink")
		{
			moveToPosition = Vector3.MoveTowards(moveToPosition, currentNavMeshLinkEndPos, Agent.speed * delta);
		}
		else
		{
			moveToPosition = Vector3.MoveTowards(moveToPosition, currentNavMeshLinkEndPos, Agent.speed * delta);
		}
	}

	private bool IsNavMeshLinkTraversalComplete(float delta, ref Vector3 moveToPosition)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = moveToPosition - currentNavMeshLinkEndPos;
		if (((Vector3)(ref val)).sqrMagnitude < 0.01f)
		{
			moveToPosition = currentNavMeshLinkEndPos;
			traversingNavMeshLink = false;
			currentNavMeshLinkName = string.Empty;
			CompleteNavMeshLink();
			return true;
		}
		return false;
	}

	private void CompleteNavMeshLink()
	{
		Agent.ActivateCurrentOffMeshLink(true);
		Agent.CompleteOffMeshLink();
		Agent.isStopped = false;
		Agent.obstacleAvoidanceType = (ObstacleAvoidanceType)4;
	}

	public bool IsPositionATopologyPreference(Vector3 position)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)TerrainMeta.TopologyMap != (Object)null)
		{
			int topology = TerrainMeta.TopologyMap.GetTopology(position);
			if ((TopologyPreference() & topology) != 0)
			{
				return true;
			}
		}
		return false;
	}

	public bool IsPositionPreventTopology(Vector3 position)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)TerrainMeta.TopologyMap != (Object)null)
		{
			int topology = TerrainMeta.TopologyMap.GetTopology(position);
			if ((TopologyPrevent() & topology) != 0)
			{
				return true;
			}
		}
		return false;
	}

	public bool IsPositionABiomePreference(Vector3 position)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Expected I4, but got Unknown
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		if (!UseBiomePreference)
		{
			return true;
		}
		if ((Object)(object)TerrainMeta.BiomeMap != (Object)null)
		{
			int num = (int)biomePreference;
			int biomeMaxType = TerrainMeta.BiomeMap.GetBiomeMaxType(position);
			if ((biomeMaxType & num) != 0)
			{
				return true;
			}
		}
		return false;
	}

	public bool IsPositionABiomeRequirement(Vector3 position)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Invalid comparison between Unknown and I4
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		if ((int)biomeRequirement == 0)
		{
			return true;
		}
		if ((Object)(object)TerrainMeta.BiomeMap != (Object)null)
		{
			int biomeMaxType = TerrainMeta.BiomeMap.GetBiomeMaxType(position);
			if ((BiomeRequirement() & biomeMaxType) != 0)
			{
				return true;
			}
		}
		return false;
	}

	public bool IsAcceptableWaterDepth(Vector3 pos)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		Profiler.BeginSample("BasePathFinder.WaterDepth");
		float overallWaterDepth = WaterLevel.GetOverallWaterDepth(pos, waves: true, volumes: false);
		Profiler.EndSample();
		return overallWaterDepth <= MaxWaterDepth;
	}

	public void SetBrakingEnabled(bool flag)
	{
		Agent.autoBraking = flag;
	}
}
