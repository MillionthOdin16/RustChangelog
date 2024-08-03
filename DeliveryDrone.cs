using System;
using Facepunch;
using ProtoBuf;
using UnityEngine;

public class DeliveryDrone : Drone
{
	private enum State
	{
		Invalid,
		Takeoff,
		FlyToVendingMachine,
		DescendToVendingMachine,
		PickUpItems,
		AscendBeforeReturn,
		ReturnToTerminal,
		Landing
	}

	[Header("Delivery Drone")]
	public float stateTimeout = 300f;

	public float targetPositionTolerance = 1f;

	public float preferredCruiseHeight = 20f;

	public float preferredHeightAboveObstacle = 5f;

	public float marginAbovePreferredHeight = 3f;

	public float obstacleHeightLockDuration = 3f;

	public int pickUpDelayInTicks = 3;

	public DeliveryDroneConfig config;

	public GameObjectRef mapMarkerPrefab;

	public EntityRef<Marketplace> sourceMarketplace;

	public EntityRef<MarketTerminal> sourceTerminal;

	public EntityRef<VendingMachine> targetVendingMachine;

	private State _state;

	private RealTimeSince _sinceLastStateChange;

	private Vector3? _stateGoalPosition;

	private float? _goToY;

	private TimeSince _sinceLastObstacleBlock;

	private float? _minimumYLock;

	private int _pickUpTicks;

	private BaseEntity _mapMarkerInstance;

	public void Setup(Marketplace marketplace, MarketTerminal terminal, VendingMachine vendingMachine)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		sourceMarketplace.Set(marketplace);
		sourceTerminal.Set(terminal);
		targetVendingMachine.Set(vendingMachine);
		_state = State.Takeoff;
		_sinceLastStateChange = RealTimeSince.op_Implicit(0f);
		_pickUpTicks = 0;
	}

	public override void ServerInit()
	{
		base.ServerInit();
		((FacepunchBehaviour)this).InvokeRandomized((Action)Think, 0f, 0.5f, 0.25f);
		CreateMapMarker();
	}

	public void CreateMapMarker()
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)_mapMarkerInstance != (Object)null)
		{
			_mapMarkerInstance.Kill();
		}
		BaseEntity baseEntity = GameManager.server.CreateEntity(mapMarkerPrefab?.resourcePath, Vector3.zero, Quaternion.identity);
		baseEntity.OwnerID = base.OwnerID;
		baseEntity.Spawn();
		baseEntity.SetParent(this);
		_mapMarkerInstance = baseEntity;
	}

	private void Think()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_03db: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_032a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0430: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_035c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0361: Unknown result type (might be due to invalid IL or missing references)
		//IL_0364: Unknown result type (might be due to invalid IL or missing references)
		//IL_0369: Unknown result type (might be due to invalid IL or missing references)
		//IL_0380: Unknown result type (might be due to invalid IL or missing references)
		//IL_046b: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0493: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a3: Unknown result type (might be due to invalid IL or missing references)
		if (RealTimeSince.op_Implicit(_sinceLastStateChange) > stateTimeout)
		{
			Debug.LogError((object)"Delivery drone hasn't change state in too long, killing", (Object)(object)this);
			ForceRemove();
			return;
		}
		if (!sourceMarketplace.TryGet(serverside: true, out var marketplace) || !sourceTerminal.TryGet(serverside: true, out var _))
		{
			Debug.LogError((object)"Delivery drone's marketplace or terminal was destroyed, killing", (Object)(object)this);
			ForceRemove();
			return;
		}
		if (!targetVendingMachine.TryGet(serverside: true, out var entity2) && _state <= State.AscendBeforeReturn)
		{
			SetState(State.ReturnToTerminal);
		}
		Vector3 currentPosition = ((Component)this).transform.position;
		float num = GetMinimumHeight(Vector3.zero);
		if (_goToY.HasValue)
		{
			if (!IsAtGoToY())
			{
				targetPosition = Vector3Ex.WithY(currentPosition, _goToY.Value);
				return;
			}
			_goToY = null;
			_sinceLastObstacleBlock = TimeSince.op_Implicit(0f);
			_minimumYLock = currentPosition.y;
		}
		Vector3 waitPosition;
		switch (_state)
		{
		case State.Takeoff:
			SetGoalPosition(marketplace.droneLaunchPoint.position + Vector3.up * 15f);
			if (IsAtGoalPosition())
			{
				SetState(State.FlyToVendingMachine);
			}
			break;
		case State.FlyToVendingMachine:
		{
			bool isBlocked2;
			float num2 = CalculatePreferredY(out isBlocked2);
			if (isBlocked2 && currentPosition.y < num2)
			{
				SetGoToY(num2 + marginAbovePreferredHeight);
				return;
			}
			config.FindDescentPoints(entity2, num2 + marginAbovePreferredHeight, out waitPosition, out var descendPosition);
			SetGoalPosition(descendPosition);
			if (IsAtGoalPosition())
			{
				SetState(State.DescendToVendingMachine);
			}
			break;
		}
		case State.DescendToVendingMachine:
		{
			config.FindDescentPoints(entity2, currentPosition.y, out var waitPosition2, out waitPosition);
			SetGoalPosition(waitPosition2);
			if (IsAtGoalPosition())
			{
				SetState(State.PickUpItems);
			}
			break;
		}
		case State.PickUpItems:
			_pickUpTicks++;
			if (_pickUpTicks >= pickUpDelayInTicks)
			{
				SetState(State.AscendBeforeReturn);
			}
			break;
		case State.AscendBeforeReturn:
		{
			config.FindDescentPoints(entity2, num + preferredCruiseHeight, out waitPosition, out var descendPosition2);
			SetGoalPosition(descendPosition2);
			if (IsAtGoalPosition())
			{
				SetState(State.ReturnToTerminal);
			}
			break;
		}
		case State.ReturnToTerminal:
		{
			bool isBlocked3;
			float num3 = CalculatePreferredY(out isBlocked3);
			if (isBlocked3 && currentPosition.y < num3)
			{
				SetGoToY(num3 + marginAbovePreferredHeight);
				return;
			}
			Vector3 val = LandingPosition();
			if (Vector3Ex.Distance2D(currentPosition, val) < 30f)
			{
				val.y = Mathf.Max(val.y, num3 + marginAbovePreferredHeight);
			}
			else
			{
				val.y = num3 + marginAbovePreferredHeight;
			}
			SetGoalPosition(val);
			if (IsAtGoalPosition())
			{
				SetState(State.Landing);
			}
			break;
		}
		case State.Landing:
			SetGoalPosition(LandingPosition());
			if (IsAtGoalPosition())
			{
				marketplace.ReturnDrone(this);
				SetState(State.Invalid);
			}
			break;
		default:
			ForceRemove();
			break;
		}
		if (_minimumYLock.HasValue)
		{
			if (TimeSince.op_Implicit(_sinceLastObstacleBlock) > obstacleHeightLockDuration)
			{
				_minimumYLock = null;
			}
			else if (targetPosition.HasValue && targetPosition.Value.y < _minimumYLock.Value)
			{
				targetPosition = Vector3Ex.WithY(targetPosition.Value, _minimumYLock.Value);
			}
		}
		float CalculatePreferredY(out bool isBlocked)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0125: Unknown result type (might be due to invalid IL or missing references)
			//IL_012a: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_014a: Unknown result type (might be due to invalid IL or missing references)
			//IL_014d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0154: Unknown result type (might be due to invalid IL or missing references)
			//IL_0159: Unknown result type (might be due to invalid IL or missing references)
			//IL_0165: Unknown result type (might be due to invalid IL or missing references)
			//IL_0172: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			//IL_0183: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0192: Unknown result type (might be due to invalid IL or missing references)
			//IL_0195: Unknown result type (might be due to invalid IL or missing references)
			//IL_0197: Unknown result type (might be due to invalid IL or missing references)
			//IL_0199: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val2 = Vector3Ex.WithY(body.velocity, 0f);
			Vector3 val3 = default(Vector3);
			float num4 = default(float);
			Vector3Ex.ToDirectionAndMagnitude(val2, ref val3, ref num4);
			if (num4 < 0.5f)
			{
				float num5 = GetMinimumHeight(Vector3.zero) + preferredCruiseHeight;
				Vector3 val4 = Vector3Ex.WithY(currentPosition, num5 + 1000f);
				Vector3 val5 = Vector3Ex.WithY(currentPosition, num5);
				RaycastHit val6 = default(RaycastHit);
				isBlocked = Physics.Raycast(val4, Vector3.down, ref val6, 1000f, LayerMask.op_Implicit(config.layerMask));
				return isBlocked ? (num5 + (1000f - ((RaycastHit)(ref val6)).distance) + preferredHeightAboveObstacle) : num5;
			}
			float num6 = num4 * 2f;
			float minimumHeight = GetMinimumHeight(Vector3.zero);
			float minimumHeight2 = GetMinimumHeight(new Vector3(0f, 0f, num6 / 2f));
			float minimumHeight3 = GetMinimumHeight(new Vector3(0f, 0f, num6));
			float num7 = Mathf.Max(Mathf.Max(minimumHeight, minimumHeight2), minimumHeight3) + preferredCruiseHeight;
			Quaternion val7 = Quaternion.FromToRotation(Vector3.forward, val3);
			Vector3 val8 = Vector3Ex.WithZ(config.halfExtents, num6 / 2f);
			Vector3 val9 = Vector3Ex.WithY(currentPosition, num7) + val7 * new Vector3(0f, 0f, val8.z / 2f);
			Vector3 val10 = Vector3Ex.WithY(val9, num7 + 1000f);
			RaycastHit val11 = default(RaycastHit);
			isBlocked = Physics.BoxCast(val10, val8, Vector3.down, ref val11, val7, 1000f, LayerMask.op_Implicit(config.layerMask));
			float result;
			if (isBlocked)
			{
				Ray ray = default(Ray);
				((Ray)(ref ray))._002Ector(val10, Vector3.down);
				Vector3 val12 = ray.ClosestPoint(((RaycastHit)(ref val11)).point);
				float num8 = Vector3.Distance(((Ray)(ref ray)).origin, val12);
				result = num7 + (1000f - num8) + preferredHeightAboveObstacle;
			}
			else
			{
				result = num7;
			}
			return result;
		}
		float GetMinimumHeight(Vector3 offset)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val13 = ((Component)this).transform.TransformPoint(offset);
			float height = TerrainMeta.HeightMap.GetHeight(val13);
			float height2 = WaterSystem.GetHeight(val13);
			return Mathf.Max(height, height2);
		}
		bool IsAtGoToY()
		{
			return _goToY.HasValue && Mathf.Abs(_goToY.Value - currentPosition.y) < targetPositionTolerance;
		}
		bool IsAtGoalPosition()
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			return _stateGoalPosition.HasValue && Vector3.Distance(_stateGoalPosition.Value, currentPosition) < targetPositionTolerance;
		}
		Vector3 LandingPosition()
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			return marketplace.droneLaunchPoint.position;
		}
		void SetGoToY(float y)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			_goToY = y;
			targetPosition = Vector3Ex.WithY(currentPosition, y);
		}
		void SetGoalPosition(Vector3 position)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			_goToY = null;
			_stateGoalPosition = position;
			targetPosition = position;
		}
		void SetState(State newState)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			_state = newState;
			_sinceLastStateChange = RealTimeSince.op_Implicit(0f);
			_pickUpTicks = 0;
			_stateGoalPosition = null;
			_goToY = null;
			SetFlag(Flags.Reserved1, _state >= State.AscendBeforeReturn);
		}
	}

	private void ForceRemove()
	{
		if (sourceMarketplace.TryGet(serverside: true, out var entity))
		{
			entity.ReturnDrone(this);
		}
		else
		{
			Kill();
		}
	}

	public override void Save(SaveInfo info)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		base.Save(info);
		if (info.forDisk)
		{
			info.msg.deliveryDrone = Pool.Get<DeliveryDrone>();
			info.msg.deliveryDrone.marketplaceId = sourceMarketplace.uid;
			info.msg.deliveryDrone.terminalId = sourceTerminal.uid;
			info.msg.deliveryDrone.vendingMachineId = targetVendingMachine.uid;
			info.msg.deliveryDrone.state = (int)_state;
		}
	}

	public override void Load(LoadInfo info)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		base.Load(info);
		if (info.msg.deliveryDrone != null)
		{
			sourceMarketplace = new EntityRef<Marketplace>(info.msg.deliveryDrone.marketplaceId);
			sourceTerminal = new EntityRef<MarketTerminal>(info.msg.deliveryDrone.terminalId);
			targetVendingMachine = new EntityRef<VendingMachine>(info.msg.deliveryDrone.vendingMachineId);
			_state = (State)info.msg.deliveryDrone.state;
		}
	}

	public override bool CanControl(ulong playerID)
	{
		return false;
	}
}
