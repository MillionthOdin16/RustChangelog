using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using Facepunch;
using Facepunch.Extend;
using ProtoBuf;
using ProtoBuf.Nexus;
using Rust;
using Rust.UI;
using UnityEngine;

public class NexusFerry : BaseEntity
{
	public enum State
	{
		Invalid,
		SailingIn,
		Queued,
		Arrival,
		Docking,
		Stopping,
		Waiting,
		CastingOff,
		Departure,
		SailingOut,
		Transferring
	}

	private readonly struct Edge
	{
		public readonly Node Next;

		public readonly float Distance;

		public Edge(Node next, float distance)
		{
			Next = next;
			Distance = distance;
		}
	}

	private class Node : IComparable<Node>, IPooled
	{
		public int Index;

		public Vector3 Position;

		public readonly List<Edge> Edges;

		public Node Parent;

		public float G;

		public float H;

		public float F => G + H;

		public Node()
		{
			Edges = new List<Edge>();
		}

		public Node ConnectTo(Node other)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			float distance = Vector3.Distance(Position, other.Position);
			Edges.Add(new Edge(other, distance));
			other.Edges.Add(new Edge(this, distance));
			return this;
		}

		public void Reset()
		{
			Parent = null;
			G = 0f;
			H = 0f;
		}

		public int CompareTo(Node other)
		{
			if (this == other || Index == other.Index)
			{
				return 0;
			}
			if (!(F < other.F))
			{
				return -1;
			}
			return 1;
		}

		public void EnterPool()
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			Index = 0;
			Position = Vector3.zero;
			Edges.Clear();
			Reset();
		}

		public void LeavePool()
		{
		}
	}

	private class Graph : IPooled
	{
		private readonly List<Node> _nodes;

		public Graph()
		{
			_nodes = new List<Node>();
		}

		public Node AddNode(Vector3 position)
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			Node node = Pool.Get<Node>();
			node.Index = _nodes.Count;
			node.Position = position;
			_nodes.Add(node);
			return node;
		}

		public Node FindClosest(Vector3 position)
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			float num = float.MaxValue;
			Node result = null;
			foreach (Node node in _nodes)
			{
				float num2 = Vector3.Distance(node.Position, position);
				if (!(num2 >= num))
				{
					num = num2;
					result = node;
				}
			}
			return result;
		}

		public bool TryFindPath(Node start, Node end, List<Vector3> path)
		{
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_0153: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			foreach (Node node3 in _nodes)
			{
				node3.Reset();
			}
			bool[] array = ArrayPool<bool>.Shared.Rent(_nodes.Count);
			Array.Clear(array, 0, array.Length);
			List<Node> list = Pool.GetList<Node>();
			list.Add(start);
			while (list.Count > 0)
			{
				int index = list.Count - 1;
				Node node = list[index];
				list.RemoveAt(index);
				array[node.Index] = true;
				if (node == end)
				{
					ArrayPool<bool>.Shared.Return(array, false);
					Pool.FreeList<Node>(ref list);
					path.Clear();
					for (Node node2 = node; node2 != null; node2 = node2.Parent)
					{
						path.Add(node2.Position);
						if (path.Count > _nodes.Count)
						{
							Debug.LogError((object)"Pathfinding code is broken!");
							path.Clear();
							return false;
						}
					}
					path.Reverse();
					return true;
				}
				foreach (Edge edge in node.Edges)
				{
					Node next = edge.Next;
					if (!array[next.Index])
					{
						float num = node.G + edge.Distance;
						if (next.Parent == null)
						{
							next.Parent = node;
							next.G = num;
							next.H = Vector3.Distance(next.Position, end.Position);
						}
						else if (num < next.G)
						{
							next.Parent = node;
							next.G = num;
						}
						int num2 = list.BinarySearch(next);
						if (num2 < 0)
						{
							list.Insert(~num2, next);
						}
					}
				}
			}
			ArrayPool<bool>.Shared.Return(array, false);
			Pool.FreeList<Node>(ref list);
			path.Clear();
			return false;
		}

		public void EnterPool()
		{
			foreach (Node node in _nodes)
			{
				Node current = node;
				Pool.Free<Node>(ref current);
			}
			_nodes.Clear();
		}

		public void LeavePool()
		{
		}
	}

	[Header("NexusFerry")]
	public float TravelVelocity = 20f;

	public float ApproachVelocity = 5f;

	public float StoppingVelocity = 1f;

	public float AccelerationSpeed = 1f;

	public float TurnSpeed = 1f;

	public float VelocityPreservationOnTurn = 0.1f;

	public float TargetDistanceThreshold = 10f;

	public GameObjectRef hornEffect;

	public Transform hornEffectTransform;

	public float departureHornLeadTime = 5f;

	[Header("Pathing")]
	public SphereCollider SphereCaster;

	public int CastSweepDegrees = 16;

	[Range(0f, 1f)]
	public float CastSweepNoise = 0.25f;

	public LayerMask CastLayers = LayerMask.op_Implicit(134283264);

	public float CastInterval = 1f;

	public float CastHitProtection = 5f;

	public int PathLookahead = 4;

	public int PathLookaheadThreshold = 5;

	[Header("UI")]
	public RustText[] NextZoneLabels;

	private long _timestamp;

	private string _ownerZone;

	private List<string> _schedule;

	private int _scheduleIndex;

	private State _state;

	private bool _isRetiring;

	private int _nextScheduleIndex;

	private bool _departureHornPlayed;

	public static readonly ListHashSet<NexusFerry> All = new ListHashSet<NexusFerry>(8);

	private List<NetworkableId> _transferredIds;

	private NexusDock _targetDock;

	private bool _isTransferring;

	private TimeSince _sinceStartedWaiting;

	private TimeSince _sinceLastTransferAttempt;

	private RealTimeSince _sinceLastNextIndexUpdate;

	private TimeSince _sincePathCalculation;

	private Vector3? _pathTargetPosition;

	private Quaternion? _pathTargetRotation;

	private Vector3 _velocity;

	public string OwnerZone => _ownerZone;

	public bool IsRetiring => _isRetiring;

	public string NextZone
	{
		get
		{
			int? num = TryGetNextScheduleIndex();
			if (!num.HasValue)
			{
				return null;
			}
			return _schedule[num.Value];
		}
	}

	protected override bool PositionTickFixedTime => true;

	public void Initialize(string ownerZone, List<string> schedule)
	{
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if (string.IsNullOrWhiteSpace(ownerZone))
			{
				throw new ArgumentNullException("ownerZone");
			}
			if (schedule == null)
			{
				throw new ArgumentNullException("schedule");
			}
			if (schedule.Count <= 1 || !schedule.Contains(ownerZone, StringComparer.InvariantCultureIgnoreCase))
			{
				throw new ArgumentException("Ferry schedule is invalid", "schedule");
			}
			_timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
			_ownerZone = ownerZone;
			_schedule = schedule;
			_scheduleIndex = List.FindIndex<string>((IReadOnlyList<string>)schedule, ownerZone, (IEqualityComparer<string>)StringComparer.InvariantCultureIgnoreCase);
			_state = State.Stopping;
			_departureHornPlayed = false;
			if (_scheduleIndex < 0)
			{
				throw new InvalidOperationException("Ferry couldn't find the owner zone in its schedule");
			}
			EnsureInitialized();
			Transform targetTransform = GetTargetTransform(_state);
			((Component)this).transform.SetPositionAndRotation(targetTransform.position, targetTransform.rotation);
		}
		catch
		{
			Kill();
			throw;
		}
	}

	private void EnsureInitialized()
	{
		_targetDock = SingletonComponent<NexusDock>.Instance;
		if ((Object)(object)_targetDock == (Object)null)
		{
			throw new InvalidOperationException("Ferry has no dock to go to!");
		}
	}

	public override void ServerInit()
	{
		base.ServerInit();
		if (!Application.isLoadingSave)
		{
			if (!NexusServer.Started)
			{
				Debug.LogError((object)"NexusFerry will not work without being connected to a nexus - destroying.");
				Kill();
				return;
			}
			if (string.IsNullOrWhiteSpace(_ownerZone) || _schedule == null || _schedule.Count <= 1 || !_schedule.Contains(_ownerZone))
			{
				Debug.LogError((object)"NexusFerry has not been initialized (you can't spawn them manually) - destroying.");
				Kill();
				return;
			}
		}
		EnsureInitialized();
		All.Add(this);
	}

	public override void DestroyShared()
	{
		base.DestroyShared();
		if (base.isServer)
		{
			All.Remove(this);
		}
		if (_transferredIds != null)
		{
			Pool.FreeList<NetworkableId>(ref _transferredIds);
		}
	}

	public void FixedUpdate()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		if (!base.isServer)
		{
			return;
		}
		if (RealTimeSince.op_Implicit(_sinceLastNextIndexUpdate) > 10f)
		{
			_sinceLastNextIndexUpdate = RealTimeSince.op_Implicit(0f);
			int num = TryGetNextScheduleIndex() ?? (-1);
			if (num != _nextScheduleIndex)
			{
				_nextScheduleIndex = num;
				SendNetworkUpdate();
			}
		}
		if (_state == State.Waiting)
		{
			EnsureInitialized();
			if (!_departureHornPlayed && _targetDock.WaitTime - TimeSince.op_Implicit(_sinceStartedWaiting) < departureHornLeadTime)
			{
				PlayDepartureHornEffect();
			}
			if (!(TimeSince.op_Implicit(_sinceStartedWaiting) >= _targetDock.WaitTime))
			{
				return;
			}
			SwitchToNextState();
		}
		if (MoveTowardsTarget())
		{
			SwitchToNextState();
		}
	}

	public FerryStatus GetStatus()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		FerryStatus obj = Pool.Get<FerryStatus>();
		obj.entityId = net.ID;
		obj.timestamp = _timestamp;
		obj.ownerZone = _ownerZone;
		obj.schedule = List.ShallowClonePooled<string>(_schedule);
		obj.scheduleIndex = _scheduleIndex;
		obj.state = (int)_state;
		obj.isRetiring = _isRetiring;
		return obj;
	}

	public void Retire()
	{
		_isRetiring = true;
	}

	public void UpdateSchedule(List<string> schedule)
	{
		if (_schedule != null)
		{
			Pool.FreeList<string>(ref _schedule);
		}
		_schedule = List.ShallowClonePooled<string>(schedule);
	}

	public override float GetNetworkTime()
	{
		return Time.fixedTime;
	}

	private void SwitchToNextState()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		if (_state == State.SailingOut)
		{
			if (!_isTransferring && TimeSince.op_Implicit(_sinceLastTransferAttempt) >= 5f)
			{
				_sinceLastTransferAttempt = TimeSince.op_Implicit(0f);
				TransferToNextZone();
			}
			return;
		}
		if (_state == State.Departure && (Object)(object)_targetDock != (Object)null)
		{
			_targetDock.Depart(this);
		}
		State nextState = GetNextState(_state);
		_state = nextState;
		SendNetworkUpdate();
		if (_state == State.Waiting)
		{
			_sinceStartedWaiting = TimeSince.op_Implicit(0f);
			_departureHornPlayed = false;
		}
		if (_state == State.CastingOff)
		{
			EjectInactiveEntities(_isRetiring);
			if (_isRetiring)
			{
				Kill();
			}
		}
	}

	private static State GetNextState(State currentState)
	{
		State state = currentState + 1;
		if (state >= State.SailingOut)
		{
			state = State.SailingOut;
		}
		return state;
	}

	private static State GetPreviousState(State currentState)
	{
		if ((uint)currentState <= 3u || (uint)(currentState - 9) <= 1u)
		{
			return State.Invalid;
		}
		return currentState - 1;
	}

	private async void TransferToNextZone()
	{
		if (_isTransferring)
		{
			return;
		}
		int? num = TryGetNextScheduleIndex();
		if (!num.HasValue)
		{
			return;
		}
		_isTransferring = true;
		int oldScheduleIndex = _scheduleIndex;
		State oldState = _state;
		try
		{
			_scheduleIndex = num.Value;
			string text = _schedule[_scheduleIndex];
			_state = State.Transferring;
			Debug.Log((object)("Sending ferry to " + text));
			await NexusServer.TransferEntity(this, text, "ferry");
		}
		finally
		{
			_isTransferring = false;
			_scheduleIndex = oldScheduleIndex;
			_state = oldState;
		}
	}

	private int? TryGetNextScheduleIndex()
	{
		string zoneKey = NexusServer.ZoneKey;
		int num = (_scheduleIndex + 1) % _schedule.Count;
		for (int i = 0; i < _schedule.Count; i++)
		{
			string text = _schedule[num];
			if (!string.Equals(text, zoneKey, StringComparison.InvariantCultureIgnoreCase) && NexusServer.TryGetZoneStatus(text, out var status) && status.IsOnline)
			{
				return num;
			}
			num++;
			if (num >= _schedule.Count)
			{
				num = 0;
			}
		}
		return null;
	}

	private void EjectInactiveEntities(bool forceAll = false)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		HashSet<NetworkableId> hashSet = Pool.Get<HashSet<NetworkableId>>();
		hashSet.Clear();
		if (_transferredIds != null)
		{
			foreach (NetworkableId transferredId in _transferredIds)
			{
				hashSet.Add(transferredId);
			}
		}
		List<BaseEntity> list = Pool.GetList<BaseEntity>();
		foreach (BaseEntity child in children)
		{
			if (!(child is NPCAutoTurret) && (hashSet.Contains(child.net.ID) || forceAll) && (!IsEntityActive(child) || forceAll))
			{
				list.Add(child);
			}
		}
		foreach (BaseEntity item in list)
		{
			EjectEntity(item);
		}
		Pool.FreeList<BaseEntity>(ref list);
		hashSet.Clear();
		Pool.Free<HashSet<NetworkableId>>(ref hashSet);
	}

	private void EjectEntity(BaseEntity entity)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		if (!((Object)(object)entity == (Object)null))
		{
			if ((Object)(object)_targetDock != (Object)null && _targetDock.TryFindEjectionPosition(out var position))
			{
				entity.SetParent(null);
				entity.ServerPosition = position;
				entity.SendNetworkUpdateImmediate();
			}
			else
			{
				Debug.LogWarning((object)$"Couldn't find an ejection point for {entity}", (Object)(object)entity);
			}
		}
	}

	private static bool IsEntityActive(BaseEntity entity)
	{
		bool result = false;
		if (entity is BasePlayer player)
		{
			result = IsPlayerReady(player);
		}
		else if (entity is BaseVehicle baseVehicle)
		{
			List<BasePlayer> list = Pool.GetList<BasePlayer>();
			baseVehicle.GetMountedPlayers(list);
			foreach (BasePlayer item in list)
			{
				if (IsPlayerReady(item))
				{
					result = true;
					break;
				}
			}
			Pool.FreeList<BasePlayer>(ref list);
		}
		return result;
	}

	private static bool IsPlayerReady(BasePlayer player)
	{
		if ((Object)(object)player != (Object)null && player.IsConnected)
		{
			return !player.IsLoadingAfterTransfer();
		}
		return false;
	}

	private void PlayDepartureHornEffect()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		if (hornEffect.isValid)
		{
			Effect.server.Run(hornEffect.resourcePath, this, 0u, hornEffectTransform.localPosition, Vector3.up);
		}
		_departureHornPlayed = true;
	}

	public override void PostServerLoad()
	{
		base.PostServerLoad();
		((FacepunchBehaviour)this).Invoke(base.DisableTransferProtectionAction, 0.1f);
	}

	public override void Save(SaveInfo info)
	{
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		base.Save(info);
		info.msg.nexusFerry = Pool.Get<NexusFerry>();
		info.msg.nexusFerry.timestamp = _timestamp;
		info.msg.nexusFerry.ownerZone = _ownerZone;
		info.msg.nexusFerry.schedule = List.ShallowClonePooled<string>(_schedule);
		info.msg.nexusFerry.scheduleIndex = _scheduleIndex;
		info.msg.nexusFerry.state = (int)_state;
		info.msg.nexusFerry.isRetiring = _isRetiring;
		info.msg.nexusFerry.nextScheduleIndex = _nextScheduleIndex;
		if (info.forTransfer)
		{
			List<NetworkableId> list = Pool.GetList<NetworkableId>();
			foreach (BaseEntity child in children)
			{
				list.Add(child.net.ID);
			}
			info.msg.nexusFerry.transferredIds = list;
		}
		else
		{
			info.msg.nexusFerry.transferredIds = List.ShallowClonePooled<NetworkableId>(_transferredIds) ?? Pool.GetList<NetworkableId>();
		}
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (info.msg.nexusFerry == null)
		{
			return;
		}
		_timestamp = info.msg.nexusFerry.timestamp;
		_ownerZone = info.msg.nexusFerry.ownerZone;
		if (_schedule != null)
		{
			Pool.FreeList<string>(ref _schedule);
		}
		_schedule = List.ShallowClonePooled<string>(info.msg.nexusFerry.schedule);
		_scheduleIndex = info.msg.nexusFerry.scheduleIndex;
		_state = (State)info.msg.nexusFerry.state;
		_isRetiring = info.msg.nexusFerry.isRetiring;
		_nextScheduleIndex = info.msg.nexusFerry.nextScheduleIndex;
		if (base.isServer)
		{
			if (_transferredIds != null)
			{
				Pool.FreeList<NetworkableId>(ref _transferredIds);
			}
			_transferredIds = List.ShallowClonePooled<NetworkableId>(info.msg.nexusFerry.transferredIds);
			if (_state == State.Transferring)
			{
				_state = State.SailingIn;
			}
		}
	}

	public static NexusFerry Get(NetworkableId entityId, long timestamp)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		if (BaseNetworkable.serverEntities.Find(entityId) is NexusFerry nexusFerry && nexusFerry._timestamp == timestamp)
		{
			return nexusFerry;
		}
		return null;
	}

	private bool MoveTowardsTarget()
	{
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		EnsureInitialized();
		switch (_state)
		{
		case State.Transferring:
			return false;
		case State.SailingIn:
			return MoveTowardsPositionAvoidObstacles(_targetDock.FerryWaypoint.position);
		case State.Queued:
		{
			bool entered;
			Transform entryPoint = _targetDock.GetEntryPoint(this, out entered);
			return MoveTowardsPositionAvoidObstacles(entryPoint.position) && entered;
		}
		case State.SailingOut:
			return MoveTowardsPositionAvoidObstacles(GetIslandTransferPosition());
		default:
			return MoveTowardsTargetTransform();
		}
	}

	private bool MoveTowardsPositionAvoidObstacles(Vector3 targetPosition)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_024e: Unknown result type (might be due to invalid IL or missing references)
		//IL_025b: Unknown result type (might be due to invalid IL or missing references)
		if (!_pathTargetPosition.HasValue || !_pathTargetRotation.HasValue || TimeSince.op_Implicit(_sincePathCalculation) > CastInterval)
		{
			Vector3 val = ChooseWaypoint(targetPosition);
			_sincePathCalculation = TimeSince.op_Implicit(0f);
			_pathTargetPosition = null;
			_pathTargetRotation = null;
			Vector3 position = ((Component)this).transform.position;
			Vector3 forward = ((Component)this).transform.forward;
			float num = Vector3Ex.Distance2D(val, position);
			float num2;
			if (!(num > 0.01f))
			{
				num2 = 0f;
			}
			else
			{
				Quaternion val2 = Quaternion.LookRotation(Vector3Ex.Direction2D(val, position));
				num2 = ((Quaternion)(ref val2)).eulerAngles.y;
			}
			float num3 = num2;
			float num4 = (float)Random.Range(0, CastSweepDegrees) * CastSweepNoise;
			int num5 = Mathf.FloorToInt(360f / (float)CastSweepDegrees);
			List<(float, float, float, Vector3, Quaternion)> list = Pool.GetList<(float, float, float, Vector3, Quaternion)>();
			float num6 = 0f;
			for (int i = 1; i < num5; i++)
			{
				int num7 = (((i & 1) == 0) ? 1 : (-1));
				int num8 = i / 2 * num7;
				Quaternion val3 = Quaternion.Euler(0f, num3 + num4 + (float)CastSweepDegrees * 0.5f * (float)num8, 0f);
				Vector3 val4 = val3 * Vector3.forward;
				float travelDistance;
				Vector3 endPosition;
				bool num9 = SphereCast(val4, num, out travelDistance, out endPosition);
				float item = Mathf.Clamp(Vector3.Dot(forward, val4), 0.5f, 1f);
				float item2 = (num9 ? Mathf.Clamp01(travelDistance / 30f) : 1f);
				float num10 = Vector3Ex.Distance2D(val, endPosition);
				list.Add((item, item2, num10, endPosition, val3));
				num6 = Mathf.Max(num6, num10);
				if (!num9)
				{
					break;
				}
			}
			float num11 = -1f;
			Vector3 value = Vector3.zero;
			Quaternion value2 = Quaternion.identity;
			foreach (var item8 in list)
			{
				float item3 = item8.Item1;
				float item4 = item8.Item2;
				float item5 = item8.Item3;
				Vector3 item6 = item8.Item4;
				Quaternion item7 = item8.Item5;
				float num12 = 1f - Mathf.Clamp01(item5 / num6);
				float num13 = item3 * item4 * num12;
				if (!(num13 <= num11))
				{
					num11 = num13;
					value = item6;
					value2 = item7;
				}
			}
			Pool.FreeList<(float, float, float, Vector3, Quaternion)>(ref list);
			_pathTargetPosition = value;
			_pathTargetRotation = value2;
		}
		if (_pathTargetPosition.HasValue && _pathTargetRotation.HasValue)
		{
			return MoveTowardsPosition(_pathTargetPosition.Value, _pathTargetRotation.Value);
		}
		return false;
		Vector3 ChooseWaypoint(Vector3 target)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			List<Vector3> list2 = Pool.GetList<Vector3>();
			if (TryFindWaypointsTowards(target, list2))
			{
				Vector3 position2 = ((Component)this).transform.position;
				Vector3 direction = default(Vector3);
				float distance = default(float);
				for (int num14 = list2.Count - 1; num14 >= 0; num14--)
				{
					Vector3Ex.ToDirectionAndMagnitude(list2[num14] - position2, ref direction, ref distance);
					if (!SphereCast(direction, distance, out var _, out var _))
					{
						Vector3 result = list2[num14];
						Pool.FreeList<Vector3>(ref list2);
						return result;
					}
				}
				Vector3 result2 = list2[0];
				Pool.FreeList<Vector3>(ref list2);
				return result2;
			}
			Pool.FreeList<Vector3>(ref list2);
			return target;
		}
	}

	private bool MoveTowardsTargetTransform()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		Transform targetTransform = GetTargetTransform(_state);
		Vector3 position = targetTransform.position;
		Quaternion rotation = targetTransform.rotation;
		return MoveTowardsPosition(position, rotation);
	}

	private Transform GetTargetTransform(State state)
	{
		EnsureInitialized();
		switch (state)
		{
		case State.Arrival:
			return _targetDock.Arrival;
		case State.Docking:
			return _targetDock.Docking;
		case State.Stopping:
		case State.Waiting:
			return _targetDock.Docked;
		case State.CastingOff:
			return _targetDock.CastingOff;
		case State.Departure:
			return _targetDock.Departure;
		default:
			Debug.LogError((object)$"Cannot call GetTargetTransform in state {state}");
			return ((Component)this).transform;
		}
	}

	private bool MoveTowardsPosition(Vector3 targetPosition, Quaternion targetRotation)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		bool flag = _state >= State.Queued && _state <= State.CastingOff;
		Vector3 position = ((Component)this).transform.position;
		targetPosition.y = position.y;
		Vector3 val = default(Vector3);
		float num = default(float);
		Vector3Ex.ToDirectionAndMagnitude(targetPosition - position, ref val, ref num);
		if (num < 0.1f)
		{
			return true;
		}
		Vector3 val2 = default(Vector3);
		float num2 = default(float);
		Vector3Ex.ToDirectionAndMagnitude(_velocity, ref val2, ref num2);
		float num3 = ((!flag) ? TravelVelocity : ((_state == State.Stopping) ? StoppingVelocity : ApproachVelocity));
		num2 = Mathx.Lerp(num2, num3, AccelerationSpeed);
		if (flag)
		{
			_velocity = num2 * val;
		}
		else
		{
			float num4 = Mathf.Clamp(Vector3.Dot(val2, val), 0.1f, 1f);
			_velocity = num4 * num2 * val + (1f - num4) * VelocityPreservationOnTurn * _velocity;
		}
		Quaternion rotation = ((Component)this).transform.rotation;
		State previousState = GetPreviousState(_state);
		Vector3 val3;
		Quaternion val4;
		if (previousState != 0)
		{
			Transform targetTransform = GetTargetTransform(previousState);
			Vector3 position2 = targetTransform.position;
			Quaternion rotation2 = targetTransform.rotation;
			position2.y = position.y;
			float num5 = Vector3Ex.Distance2D(position2, targetPosition);
			float num6 = ((Vector3)(ref _velocity)).magnitude * Time.deltaTime;
			float num7 = Mathf.Min(num6, num);
			val3 = position + val * num7;
			val4 = Quaternion.Slerp(targetRotation, rotation2, num / num5);
			((Component)this).transform.SetPositionAndRotation(val3, val4);
			if (!Mathf.Approximately(num6, 0f))
			{
				return num7 < num6;
			}
			return true;
		}
		Vector3 val5 = _velocity * Time.deltaTime;
		Vector3 val6 = default(Vector3);
		float num8 = default(float);
		Vector3Ex.ToDirectionAndMagnitude(val5, ref val6, ref num8);
		val3 = ((!(num8 >= num) || !((double)Vector3.Dot(val6, val) > 0.5)) ? (position + val5) : targetPosition);
		targetRotation = ((((Vector3)(ref val)).sqrMagnitude > 0.01f) ? Quaternion.LookRotation(val) : Quaternion.identity);
		val4 = Mathx.Lerp(rotation, targetRotation, TurnSpeed);
		((Component)this).transform.SetPositionAndRotation(val3, val4);
		return Vector3.Distance(val3, targetPosition) < TargetDistanceThreshold;
	}

	private bool SphereCast(Vector3 direction, float distance, out float travelDistance, out Vector3 endPosition)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = ((Component)SphereCaster).transform.position + SphereCaster.center;
		float radius = SphereCaster.radius;
		List<RaycastHit> list = Pool.GetList<RaycastHit>();
		GamePhysics.TraceAll(new Ray(val, direction), radius, list, distance, LayerMask.op_Implicit(CastLayers), (QueryTriggerInteraction)2);
		bool flag = false;
		travelDistance = 0f;
		foreach (RaycastHit item in list)
		{
			RaycastHit current = item;
			BaseEntity entity = current.GetEntity();
			if ((!((Object)(object)entity != (Object)null) || (!((Object)(object)entity == (Object)(object)this) && !entity.EqualNetID((BaseNetworkable)this))) && (!((RaycastHit)(ref current)).collider.isTrigger || ((Component)((RaycastHit)(ref current)).collider).CompareTag("FerryAvoid")))
			{
				flag = true;
				travelDistance = Mathf.Max(((RaycastHit)(ref current)).distance - CastHitProtection, 0f);
				break;
			}
		}
		Pool.FreeList<RaycastHit>(ref list);
		if (!flag)
		{
			travelDistance = distance;
		}
		endPosition = val + direction * travelDistance;
		return flag;
	}

	private Vector3 GetIslandTransferPosition()
	{
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		EnsureInitialized();
		int? num = TryGetNextScheduleIndex();
		if (num.HasValue)
		{
			string zoneKey = _schedule[num.Value];
			if (NexusServer.TryGetIsland(zoneKey, out var island))
			{
				return island.FerryWaypoint.position;
			}
			if (NexusServer.TryGetIslandPosition(zoneKey, out var position))
			{
				return position;
			}
		}
		if (NexusIsland.All.Count > 0)
		{
			return NexusIsland.All[0].FerryWaypoint.position;
		}
		return _targetDock.FerryWaypoint.position;
	}

	private bool TryFindWaypointsTowards(Vector3 targetPosition, List<Vector3> waypoints)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = Vector3Ex.WithY(TerrainMeta.Center, 0f);
		Vector3 size = TerrainMeta.Size;
		float num = Mathf.Sqrt(size.x * size.x + size.z * size.z) / 2f;
		Graph graph = Pool.Get<Graph>();
		Node node = null;
		Node node2 = null;
		float num2 = 0f;
		int num3 = 0;
		while (num3 < 64)
		{
			Vector3 position = val + Quaternion.Euler(0f, num2, 0f) * Vector3.forward * num;
			Node node3 = graph.AddNode(position);
			if (node2 != null)
			{
				node3.ConnectTo(node2);
			}
			if (node == null)
			{
				node = node3;
			}
			node2 = node3;
			num3++;
			num2 += 5.5384617f;
		}
		if (node != null && node2 != null && node != node2)
		{
			node.ConnectTo(node2);
		}
		foreach (NexusIsland item in NexusIsland.All)
		{
			Vector3 position2 = item.FerryWaypoint.position;
			Node other = graph.FindClosest(position2);
			graph.AddNode(position2).ConnectTo(other);
		}
		if ((Object)(object)SingletonComponent<NexusDock>.Instance != (Object)null)
		{
			Vector3 position3 = SingletonComponent<NexusDock>.Instance.FerryWaypoint.position;
			Node node4 = graph.FindClosest(position3);
			Vector3 val2 = (position3 + node4.Position) * 0.5f;
			Vector3 position4 = (val2 + node4.Position) * 0.5f;
			Vector3 position5 = (val2 + position3) * 0.5f;
			Node other2 = graph.AddNode(position4).ConnectTo(node4);
			Node other3 = graph.AddNode(val2).ConnectTo(other2);
			Node other4 = graph.AddNode(position5).ConnectTo(other3);
			graph.AddNode(position3).ConnectTo(other4);
		}
		Node node5 = graph.FindClosest(((Component)this).transform.position);
		Node node6 = graph.FindClosest(targetPosition);
		if (node5 == node6)
		{
			waypoints.Add(targetPosition);
			Pool.Free<Graph>(ref graph);
			return true;
		}
		List<Vector3> list = Pool.GetList<Vector3>();
		if (node5 == null || node6 == null || !graph.TryFindPath(node5, node6, list) || list.Count == 0)
		{
			Pool.FreeList<Vector3>(ref list);
			Pool.Free<Graph>(ref graph);
			return false;
		}
		Pool.Free<Graph>(ref graph);
		if (list.Count == 1)
		{
			waypoints.Add(list[0]);
			Pool.FreeList<Vector3>(ref list);
			return true;
		}
		int num4 = list.Count - 1;
		int num5 = ((num4 < PathLookaheadThreshold) ? 1 : Mathf.Min(PathLookahead, num4));
		for (int i = 1; i <= num5; i++)
		{
			waypoints.Add(list[i]);
		}
		Pool.FreeList<Vector3>(ref list);
		return true;
	}
}
