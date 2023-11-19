using System;
using System.Collections.Generic;
using ConVar;
using Rust.AI;
using Rust.Ai;
using UnityEngine;
using UnityEngine.AI;

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
	public bool CanNavigateMounted;

	public bool CanUseNavMesh = true;

	public bool CanUseAStar = true;

	public bool CanUseBaseNav;

	public bool CanUseCustomNav;

	public float StoppingDistance = 0.5f;

	public string DefaultArea = "Walkable";

	[Header("Stuck Detection")]
	public bool TriggerStuckEvent;

	public float StuckDistance = 1f;

	[Header("Speed")]
	public float Speed = 5f;

	public float Acceleration = 5f;

	public float TurnSpeed = 10f;

	public NavigationSpeed MoveTowardsSpeed = NavigationSpeed.Normal;

	public bool FaceMoveTowardsTarget;

	[Header("Speed Fractions")]
	public float SlowestSpeedFraction = 0.16f;

	public float SlowSpeedFraction = 0.3f;

	public float NormalSpeedFraction = 0.5f;

	public float FastSpeedFraction = 1f;

	public float LowHealthSpeedReductionTriggerFraction;

	public float LowHealthMaxSpeedFraction = 0.5f;

	public float SwimmingSpeedMultiplier = 0.25f;

	[Header("AIPoint Usage")]
	public float BestMovementPointMaxDistance = 10f;

	public float BestCoverPointMaxDistance = 20f;

	public float BestRoamPointMaxDistance = 20f;

	public float MaxRoamDistanceFromHome = -1f;

	[Header("Misc")]
	public float MaxWaterDepth = 0.75f;

	public bool SpeedBasedAvoidancePriority;

	private NavMeshPath path;

	private NavMeshQueryFilter navMeshQueryFilter;

	private int defaultAreaMask;

	[InspectorFlags]
	public Enum biomePreference = (Enum)12;

	public bool UseBiomePreference;

	[InspectorFlags]
	public Enum topologyPreference = (Enum)96;

	[InspectorFlags]
	public Enum topologyPrevent;

	[InspectorFlags]
	public Enum biomeRequirement;

	private float stuckTimer;

	private Vector3 stuckCheckPosition;

	protected bool traversingNavMeshLink;

	protected string currentNavMeshLinkName;

	protected Vector3 currentNavMeshLinkEndPos;

	protected Stack<IAIPathNode> currentAStarPath;

	protected IAIPathNode targetNode;

	protected float currentSpeedFraction = 1f;

	private float lastSetDestinationTime;

	protected OverrideFacingDirectionMode overrideFacingDirectionMode;

	protected BaseEntity facingDirectionEntity;

	protected bool overrideFacingDirection;

	protected Vector3 facingDirectionOverride;

	protected bool paused;

	private int frameCount;

	private float accumDelta;

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
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected I4, but got Unknown
		return (int)topologyPreference;
	}

	public int TopologyPrevent()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected I4, but got Unknown
		return (int)topologyPrevent;
	}

	public int BiomeRequirement()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected I4, but got Unknown
		return (int)biomeRequirement;
	}

	public virtual void Init(BaseCombatEntity entity, NavMeshAgent agent)
	{
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Expected O, but got Unknown
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
		if (flag && CanEnableNavMeshNavigation())
		{
			PlaceOnNavMesh();
		}
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
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		int num = 10551296;
		RaycastHit val = default(RaycastHit);
		if (Physics.Raycast(((Component)this).transform.position + Vector3.up * 0.5f, Vector3.down, ref val, 1000f, num))
		{
			BaseEntity.ServerPosition = ((RaycastHit)(ref val)).point;
		}
	}

	public bool PlaceOnNavMesh()
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		if (Agent.isOnNavMesh)
		{
			return true;
		}
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
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
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
		return result;
	}

	public bool SetBaseDestination(Vector3 pos, float speedFraction)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return SetDestination(pos, GetSpeedFraction(speed), updateInterval, navmeshSampleDistance);
	}

	protected virtual bool SetCustomDestination(Vector3 pos, float speedFraction = 1f, float updateInterval = 0f)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
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
		NavigationType navigationType = NavigationType.NavMesh;
		bool num = CanUseBaseNav && CanUseNavMesh;
		NavigationType navigationType2 = NavigationType.None;
		if (num)
		{
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
							return false;
						}
					}
				}
			}
			switch (navigationType2)
			{
			case NavigationType.Base:
				navigationType = ((navigationType3 == NavigationType.Base) ? NavigationType.Base : ((!(Vector3.Distance(BaseEntity.ServerPosition, pos) <= 10f) || !(Mathf.Abs(BaseEntity.ServerPosition.y - pos.y) <= 3f)) ? NavigationType.NavMesh : NavigationType.Base));
				break;
			case NavigationType.NavMesh:
				navigationType = ((navigationType3 == NavigationType.NavMesh) ? NavigationType.NavMesh : NavigationType.Base);
				break;
			}
		}
		else
		{
			navigationType = (CanUseNavMesh ? NavigationType.NavMesh : NavigationType.AStar);
		}
		switch (navigationType)
		{
		case NavigationType.Base:
			return SetBaseDestination(pos, speedFraction);
		case NavigationType.AStar:
			if ((Object)(object)AStarGraph != (Object)null)
			{
				return SetDestination(AStarGraph, AStarGraph.GetClosestToPoint(pos), speedFraction);
			}
			if (CanUseCustomNav)
			{
				return SetCustomDestination(pos, speedFraction, updateInterval);
			}
			return false;
		default:
		{
			if (AiManager.nav_disable)
			{
				return false;
			}
			if (navmeshSampleDistance > 0f && AI.setdestinationsamplenavmesh)
			{
				NavMeshHit val = default(NavMeshHit);
				if (!NavMesh.SamplePosition(pos, ref val, navmeshSampleDistance, defaultAreaMask))
				{
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
			bool flag;
			if (AI.usecalculatepath)
			{
				flag = NavMesh.CalculatePath(((Component)this).transform.position, Destination, navMeshQueryFilter, path);
				if (flag)
				{
					Agent.SetPath(path);
				}
				else if (AI.usesetdestinationfallback)
				{
					flag = Agent.SetDestination(Destination);
				}
			}
			else
			{
				flag = Agent.SetDestination(Destination);
			}
			if (flag && SpeedBasedAvoidancePriority)
			{
				Agent.avoidancePriority = Random.Range(0, 21) + Mathf.FloorToInt(speedFraction * 80f);
			}
			return flag;
		}
		}
	}

	private NavigationType DetermineNavigationType(Vector3 location, out Vector3 navMeshPos)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		navMeshPos = location;
		int num = 2097152;
		RaycastHit val = default(RaycastHit);
		if (Physics.Raycast(location + Vector3.up * navTypeHeightOffset, Vector3.down, ref val, navTypeDistance, num))
		{
			return NavigationType.Base;
		}
		Vector3 position;
		int result = (GetNearestNavmeshPosition(location + Vector3.up * navTypeHeightOffset, out position, navTypeDistance) ? 1 : 4);
		navMeshPos = position;
		return (NavigationType)result;
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
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		if (paused)
		{
			SetDestination(Destination, currentSpeedFraction);
			paused = false;
		}
	}

	public void Stop()
	{
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
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		if (!AI.move || !CanUpdateMovement())
		{
			return;
		}
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
		if (ValidateNextPosition(ref moveToPosition))
		{
			bool swimming = IsSwimming();
			UpdateSpeed(delta, swimming);
			UpdatePositionAndRotation(moveToPosition, delta);
		}
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
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		if (currentAStarPath != null && currentAStarPath.Count > 0)
		{
			return currentAStarPath.Peek().Position;
		}
		return Agent.nextPosition;
	}

	private bool ValidateNextPosition(ref Vector3 moveToPosition)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		bool flag = ValidBounds.Test(moveToPosition);
		if ((Object)(object)BaseEntity != (Object)null && !flag && (Object)(object)((Component)this).transform != (Object)null && !BaseEntity.IsDestroyed)
		{
			Debug.Log((object)("Invalid NavAgent Position: " + ((object)this)?.ToString() + " " + ((object)(Vector3)(ref moveToPosition)).ToString() + " (destroying)"));
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
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
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
				return;
			}
			frameCount = 0;
			delta = accumDelta;
			accumDelta = 0f;
			int num = 10551552;
			Vector3 val = Vector3Ex.Direction2D(Destination, BaseEntity.ServerPosition);
			Vector3 val2 = BaseEntity.ServerPosition + val * delta * Agent.speed;
			Vector3 val3 = BaseEntity.ServerPosition + Vector3.up * maxStepUpDistance;
			Vector3 val4 = Vector3Ex.Direction(val2 + Vector3.up * maxStepUpDistance, BaseEntity.ServerPosition + Vector3.up * maxStepUpDistance);
			float num2 = Vector3.Distance(val3, val2 + Vector3.up * maxStepUpDistance) + 0.25f;
			RaycastHit val5 = default(RaycastHit);
			if (Physics.Raycast(val3, val4, ref val5, num2, num))
			{
				return;
			}
			Vector3 val6 = val2 + Vector3.up * (maxStepUpDistance + 0.3f);
			Vector3 val7 = val2;
			if (!Physics.SphereCast(val6, 0.25f, Vector3.down, ref val5, 10f, num))
			{
				return;
			}
			val7 = ((RaycastHit)(ref val5)).point;
			if (val7.y - BaseEntity.ServerPosition.y > maxStepUpDistance)
			{
				return;
			}
			BaseEntity.ServerPosition = val7;
			if (ReachedPosition(moveToPosition))
			{
				Stop();
			}
		}
		if (overrideFacingDirectionMode != 0)
		{
			ApplyFacingDirectionOverride();
		}
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
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
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
		if (Vector3.Dot(desiredVelocity, normalized) < 0.1f)
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
		if (!(currentNavMeshLinkName == "OpenDoorLink") && !(currentNavMeshLinkName == "JumpRockLink"))
		{
			_ = currentNavMeshLinkName == "JumpFoundationLink";
		}
		return true;
	}

	private void HandleNavMeshLinkTraversalTick(float delta, ref Vector3 moveToPosition)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Expected I4, but got Unknown
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		if (!UseBiomePreference)
		{
			return true;
		}
		if ((Object)(object)TerrainMeta.BiomeMap != (Object)null)
		{
			int num = (int)biomePreference;
			if ((TerrainMeta.BiomeMap.GetBiomeMaxType(position) & num) != 0)
			{
				return true;
			}
		}
		return false;
	}

	public bool IsPositionABiomeRequirement(Vector3 position)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		return WaterLevel.GetOverallWaterDepth(pos, waves: true, volumes: false) <= MaxWaterDepth;
	}

	public void SetBrakingEnabled(bool flag)
	{
		Agent.autoBraking = flag;
	}
}
