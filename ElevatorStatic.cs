using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

public class ElevatorStatic : Elevator
{
	public bool StaticTop;

	private const Flags LiftRecentlyArrived = Flags.Reserved3;

	private List<ElevatorStatic> floorPositions = new List<ElevatorStatic>();

	private ElevatorStatic ownerElevator;

	protected override bool IsStatic => true;

	public override void Spawn()
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		base.Spawn();
		SetFlag(Flags.Reserved2, b: true);
		SetFlag(Flags.Reserved1, StaticTop);
		if (!base.IsTop)
		{
			return;
		}
		List<RaycastHit> list = Pool.GetList<RaycastHit>();
		GamePhysics.TraceAll(new Ray(((Component)this).transform.position, -Vector3.up), 0f, list, 200f, 262144, (QueryTriggerInteraction)2);
		foreach (RaycastHit item in list)
		{
			RaycastHit current = item;
			if ((Object)(object)((RaycastHit)(ref current)).transform.parent != (Object)null)
			{
				ElevatorStatic component = ((Component)((RaycastHit)(ref current)).transform.parent).GetComponent<ElevatorStatic>();
				if ((Object)(object)component != (Object)null && (Object)(object)component != (Object)(object)this && component.isServer)
				{
					floorPositions.Add(component);
				}
			}
		}
		Pool.FreeList<RaycastHit>(ref list);
		floorPositions.Reverse();
		base.Floor = floorPositions.Count;
		for (int i = 0; i < floorPositions.Count; i++)
		{
			floorPositions[i].SetFloorDetails(i, this);
		}
	}

	public override void PostMapEntitySpawn()
	{
		base.PostMapEntitySpawn();
		UpdateChildEntities(base.IsTop);
	}

	protected override bool IsValidFloor(int targetFloor)
	{
		if (targetFloor >= 0)
		{
			return targetFloor <= base.Floor;
		}
		return false;
	}

	protected override Vector3 GetWorldSpaceFloorPosition(int targetFloor)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		if (targetFloor == base.Floor)
		{
			return ((Component)this).transform.position + Vector3.up * 1f;
		}
		Vector3 position = ((Component)this).transform.position;
		position.y = ((Component)floorPositions[targetFloor]).transform.position.y + 1f;
		return position;
	}

	public void SetFloorDetails(int floor, ElevatorStatic owner)
	{
		ownerElevator = owner;
		base.Floor = floor;
	}

	protected override void CallElevator()
	{
		if ((Object)(object)ownerElevator != (Object)null)
		{
			ownerElevator.RequestMoveLiftTo(base.Floor, out var _, this);
		}
		else if (base.IsTop)
		{
			RequestMoveLiftTo(base.Floor, out var _, this);
		}
	}

	private ElevatorStatic ElevatorAtFloor(int floor)
	{
		if (floor == base.Floor)
		{
			return this;
		}
		if (floor >= 0 && floor < floorPositions.Count)
		{
			return floorPositions[floor];
		}
		return null;
	}

	protected override void OpenDoorsAtFloor(int floor)
	{
		base.OpenDoorsAtFloor(floor);
		if (floor == floorPositions.Count)
		{
			OpenLiftDoors();
		}
		else
		{
			floorPositions[floor].OpenLiftDoors();
		}
	}

	protected override void OnMoveBegin()
	{
		base.OnMoveBegin();
		ElevatorStatic elevatorStatic = ElevatorAtFloor(LiftPositionToFloor());
		if ((Object)(object)elevatorStatic != (Object)null)
		{
			elevatorStatic.OnLiftLeavingFloor();
		}
		NotifyLiftEntityDoorsOpen(state: false);
	}

	private void OnLiftLeavingFloor()
	{
		ClearPowerOutput();
		if (((FacepunchBehaviour)this).IsInvoking((Action)ClearPowerOutput))
		{
			((FacepunchBehaviour)this).CancelInvoke((Action)ClearPowerOutput);
		}
	}

	protected override void ClearBusy()
	{
		base.ClearBusy();
		ElevatorStatic elevatorStatic = ElevatorAtFloor(LiftPositionToFloor());
		if ((Object)(object)elevatorStatic != (Object)null)
		{
			elevatorStatic.OnLiftArrivedAtFloor();
		}
		NotifyLiftEntityDoorsOpen(state: true);
	}

	protected override void OpenLiftDoors()
	{
		base.OpenLiftDoors();
		OnLiftArrivedAtFloor();
	}

	private void OnLiftArrivedAtFloor()
	{
		SetFlag(Flags.Reserved3, b: true);
		MarkDirty();
		((FacepunchBehaviour)this).Invoke((Action)ClearPowerOutput, 10f);
	}

	private void ClearPowerOutput()
	{
		SetFlag(Flags.Reserved3, b: false);
		MarkDirty();
	}

	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		if (!HasFlag(Flags.Reserved3))
		{
			return 0;
		}
		return 1;
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (info.fromDisk)
		{
			SetFlag(Flags.Reserved3, b: false);
		}
	}
}
