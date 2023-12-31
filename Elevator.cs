using System;
using Facepunch;
using ProtoBuf;
using Rust;
using UnityEngine;

public class Elevator : IOEntity, IFlagNotify
{
	public enum Direction
	{
		Up,
		Down
	}

	public Transform LiftRoot;

	public GameObjectRef LiftEntityPrefab;

	public GameObjectRef IoEntityPrefab;

	public Transform IoEntitySpawnPoint;

	public GameObject FloorBlockerVolume;

	public float LiftSpeedPerMetre = 1f;

	public GameObject[] PoweredObjects;

	public MeshRenderer PoweredMesh;

	[ColorUsage(true, true)]
	public Color PoweredLightColour;

	[ColorUsage(true, true)]
	public Color UnpoweredLightColour;

	public SkinnedMeshRenderer[] CableRenderers;

	public LODGroup CableLod;

	public Transform CableRoot;

	public float LiftMoveDelay;

	protected const Flags TopFloorFlag = Flags.Reserved1;

	public const Flags ElevatorPowered = Flags.Reserved2;

	private ElevatorLift liftEntity;

	private IOEntity ioEntity;

	private int[] previousPowerAmount = new int[2];

	protected virtual bool IsStatic => false;

	public int Floor { get; set; }

	protected bool IsTop => HasFlag(Flags.Reserved1);

	protected virtual float FloorHeight => 3f;

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (info.msg.elevator != null)
		{
			Floor = info.msg.elevator.floor;
		}
		if ((Object)(object)FloorBlockerVolume != (Object)null)
		{
			FloorBlockerVolume.SetActive(Floor > 0);
		}
	}

	public override void OnDeployed(BaseEntity parent, BasePlayer deployedBy, Item fromItem)
	{
		base.OnDeployed(parent, deployedBy, fromItem);
		Elevator elevatorInDirection = GetElevatorInDirection(Direction.Down);
		if ((Object)(object)elevatorInDirection != (Object)null)
		{
			elevatorInDirection.SetFlag(Flags.Reserved1, b: false);
			Floor = elevatorInDirection.Floor + 1;
		}
		SetFlag(Flags.Reserved1, b: true);
	}

	protected virtual void CallElevator()
	{
		EntityLinkBroadcast(delegate(Elevator elevatorEnt)
		{
			if (elevatorEnt.IsTop)
			{
				elevatorEnt.RequestMoveLiftTo(Floor, out var _, this);
			}
		}, (ConstructionSocket socket) => socket.socketType == ConstructionSocket.Type.Elevator);
	}

	public void Server_RaiseLowerElevator(Direction dir, bool goTopBottom)
	{
		if (IsBusy())
		{
			return;
		}
		int num = LiftPositionToFloor();
		switch (dir)
		{
		case Direction.Up:
			num++;
			if (goTopBottom)
			{
				num = Floor;
			}
			break;
		case Direction.Down:
			num--;
			if (goTopBottom)
			{
				num = 0;
			}
			break;
		}
		RequestMoveLiftTo(num, out var _, this);
	}

	protected bool RequestMoveLiftTo(int targetFloor, out float timeToTravel, Elevator fromElevator)
	{
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		timeToTravel = 0f;
		if (IsBusy())
		{
			return false;
		}
		if (!IsStatic && (Object)(object)ioEntity != (Object)null && !ioEntity.IsPowered())
		{
			return false;
		}
		if (!IsValidFloor(targetFloor))
		{
			return false;
		}
		if (!liftEntity.CanMove())
		{
			return false;
		}
		int num = LiftPositionToFloor();
		if (num == targetFloor)
		{
			OpenLiftDoors();
			OpenDoorsAtFloor(num);
			fromElevator.OpenLiftDoors();
			return false;
		}
		Vector3 worldSpaceFloorPosition = GetWorldSpaceFloorPosition(targetFloor);
		if (!GamePhysics.LineOfSight(((Component)liftEntity).transform.position, worldSpaceFloorPosition, 2097152))
		{
			return false;
		}
		OnMoveBegin();
		Vector3 val = ((Component)this).transform.InverseTransformPoint(worldSpaceFloorPosition);
		timeToTravel = TimeToTravelDistance(Mathf.Abs(((Component)liftEntity).transform.localPosition.y - val.y));
		LeanTween.moveLocalY(((Component)liftEntity).gameObject, val.y, timeToTravel).delay = LiftMoveDelay;
		timeToTravel += LiftMoveDelay;
		SetFlag(Flags.Busy, b: true);
		if (targetFloor < Floor)
		{
			liftEntity.ToggleHurtTrigger(state: true);
		}
		((FacepunchBehaviour)this).Invoke((Action)ClearBusy, timeToTravel + 1f);
		liftEntity.NotifyNewFloor(targetFloor, Floor);
		if ((Object)(object)ioEntity != (Object)null)
		{
			ioEntity.SetFlag(Flags.Busy, b: true);
			ioEntity.SendChangedToRoot(forceUpdate: true);
		}
		return true;
	}

	protected virtual void OpenLiftDoors()
	{
		NotifyLiftEntityDoorsOpen(state: true);
	}

	protected virtual void OnMoveBegin()
	{
	}

	private float TimeToTravelDistance(float distance)
	{
		return distance / LiftSpeedPerMetre;
	}

	protected virtual Vector3 GetWorldSpaceFloorPosition(int targetFloor)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		int num = Floor - targetFloor;
		Vector3 val = Vector3.up * ((float)num * FloorHeight);
		val.y -= 1f;
		return ((Component)this).transform.position - val;
	}

	protected virtual void ClearBusy()
	{
		SetFlag(Flags.Busy, b: false);
		if ((Object)(object)liftEntity != (Object)null)
		{
			liftEntity.ToggleHurtTrigger(state: false);
		}
		if ((Object)(object)ioEntity != (Object)null)
		{
			ioEntity.SetFlag(Flags.Busy, b: false);
			ioEntity.SendChangedToRoot(forceUpdate: true);
		}
	}

	protected virtual bool IsValidFloor(int targetFloor)
	{
		if (targetFloor <= Floor)
		{
			return targetFloor >= 0;
		}
		return false;
	}

	private Elevator GetElevatorInDirection(Direction dir)
	{
		EntityLink entityLink = FindLink((dir == Direction.Down) ? "elevator/sockets/elevator-male" : "elevator/sockets/elevator-female");
		if (entityLink != null && !entityLink.IsEmpty())
		{
			BaseEntity owner = entityLink.connections[0].owner;
			if ((Object)(object)owner != (Object)null && owner.isServer && owner is Elevator elevator && (Object)(object)elevator != (Object)(object)this)
			{
				return elevator;
			}
		}
		return null;
	}

	public void UpdateChildEntities(bool isTop)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		if (isTop)
		{
			if ((Object)(object)liftEntity == (Object)null)
			{
				FindExistingLiftChild();
			}
			if ((Object)(object)liftEntity == (Object)null)
			{
				liftEntity = GameManager.server.CreateEntity(LiftEntityPrefab.resourcePath, GetWorldSpaceFloorPosition(Floor), LiftRoot.rotation) as ElevatorLift;
				liftEntity.SetParent(this, worldPositionStays: true);
				liftEntity.Spawn();
			}
			if ((Object)(object)ioEntity == (Object)null)
			{
				FindExistingIOChild();
			}
			if ((Object)(object)ioEntity == (Object)null && IoEntityPrefab.isValid)
			{
				ioEntity = GameManager.server.CreateEntity(IoEntityPrefab.resourcePath, IoEntitySpawnPoint.position, IoEntitySpawnPoint.rotation) as IOEntity;
				ioEntity.SetParent(this, worldPositionStays: true);
				ioEntity.Spawn();
			}
		}
		else
		{
			if ((Object)(object)liftEntity != (Object)null)
			{
				liftEntity.Kill();
			}
			if ((Object)(object)ioEntity != (Object)null)
			{
				ioEntity.Kill();
			}
		}
	}

	private void FindExistingIOChild()
	{
		foreach (BaseEntity child in children)
		{
			if (child is IOEntity iOEntity)
			{
				ioEntity = iOEntity;
				break;
			}
		}
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		if (info.msg.elevator == null)
		{
			info.msg.elevator = Pool.Get<Elevator>();
		}
		info.msg.elevator.floor = Floor;
	}

	protected int LiftPositionToFloor()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		Vector3 position = ((Component)liftEntity).transform.position;
		int result = -1;
		float num = float.MaxValue;
		for (int i = 0; i <= Floor; i++)
		{
			float num2 = Vector3.Distance(GetWorldSpaceFloorPosition(i), position);
			if (num2 < num)
			{
				num = num2;
				result = i;
			}
		}
		return result;
	}

	public override void DestroyShared()
	{
		Cleanup();
		base.DestroyShared();
	}

	private void Cleanup()
	{
		Elevator elevatorInDirection = GetElevatorInDirection(Direction.Down);
		if ((Object)(object)elevatorInDirection != (Object)null)
		{
			elevatorInDirection.SetFlag(Flags.Reserved1, b: true);
		}
		Elevator elevatorInDirection2 = GetElevatorInDirection(Direction.Up);
		if ((Object)(object)elevatorInDirection2 != (Object)null)
		{
			elevatorInDirection2.Kill(DestroyMode.Gib);
		}
	}

	public override void PostServerLoad()
	{
		base.PostServerLoad();
		SetFlag(Flags.Busy, b: false);
		UpdateChildEntities(IsTop);
		if ((Object)(object)ioEntity != (Object)null)
		{
			ioEntity.SetFlag(Flags.Busy, b: false);
		}
	}

	public override void UpdateHasPower(int inputAmount, int inputSlot)
	{
		base.UpdateHasPower(inputAmount, inputSlot);
		if (inputAmount > 0 && previousPowerAmount[inputSlot] == 0)
		{
			CallElevator();
		}
		previousPowerAmount[inputSlot] = inputAmount;
	}

	private void OnPhysicsNeighbourChanged()
	{
		if (!IsStatic && (Object)(object)GetElevatorInDirection(Direction.Down) == (Object)null && !HasFloorSocketConnection())
		{
			Kill(DestroyMode.Gib);
		}
	}

	private bool HasFloorSocketConnection()
	{
		EntityLink entityLink = FindLink("elevator/sockets/block-male");
		if (entityLink != null && !entityLink.IsEmpty())
		{
			return true;
		}
		return false;
	}

	public void NotifyLiftEntityDoorsOpen(bool state)
	{
		if (!((Object)(object)liftEntity != (Object)null))
		{
			return;
		}
		foreach (BaseEntity child in liftEntity.children)
		{
			if (child is Door door)
			{
				door.SetOpen(state);
			}
		}
	}

	protected virtual void OpenDoorsAtFloor(int floor)
	{
	}

	public override void OnFlagsChanged(Flags old, Flags next)
	{
		base.OnFlagsChanged(old, next);
		if (!Application.isLoading && base.isServer && old.HasFlag(Flags.Reserved1) != next.HasFlag(Flags.Reserved1))
		{
			UpdateChildEntities(next.HasFlag(Flags.Reserved1));
		}
		if (old.HasFlag(Flags.Busy) != next.HasFlag(Flags.Busy))
		{
			if ((Object)(object)liftEntity == (Object)null)
			{
				FindExistingLiftChild();
			}
			if ((Object)(object)liftEntity != (Object)null)
			{
				liftEntity.ToggleMovementCollider(!next.HasFlag(Flags.Busy));
			}
		}
		if (old.HasFlag(Flags.Reserved1) != next.HasFlag(Flags.Reserved1) && (Object)(object)FloorBlockerVolume != (Object)null)
		{
			FloorBlockerVolume.SetActive(next.HasFlag(Flags.Reserved1));
		}
	}

	private void FindExistingLiftChild()
	{
		foreach (BaseEntity child in children)
		{
			if (child is ElevatorLift elevatorLift)
			{
				liftEntity = elevatorLift;
				break;
			}
		}
	}

	public void OnFlagToggled(bool state)
	{
		if (base.isServer)
		{
			SetFlag(Flags.Reserved2, state);
		}
	}
}
