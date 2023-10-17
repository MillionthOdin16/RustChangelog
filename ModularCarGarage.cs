using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

public class ModularCarGarage : ContainerIOEntity
{
	[Serializable]
	public class ChassisBuildOption
	{
		public GameObjectRef prefab;

		public ItemDefinition itemDef;
	}

	public enum OccupantLock
	{
		CannotHaveLock,
		NoLock,
		HasLock
	}

	private enum VehicleLiftState
	{
		Down,
		Up
	}

	private ModularCar lockedOccupant;

	private readonly HashSet<BasePlayer> lootingPlayers = new HashSet<BasePlayer>();

	private MagnetSnap magnetSnap;

	[SerializeField]
	private Transform vehicleLift;

	[SerializeField]
	private Animation vehicleLiftAnim;

	[SerializeField]
	private string animName = "LiftUp";

	[SerializeField]
	private VehicleLiftOccupantTrigger occupantTrigger;

	[SerializeField]
	private float liftMoveTime = 1f;

	[SerializeField]
	private EmissionToggle poweredLight;

	[SerializeField]
	private EmissionToggle inUseLight;

	[SerializeField]
	private Transform vehicleLiftPos;

	[SerializeField]
	[Range(0f, 1f)]
	private float recycleEfficiency = 0.5f;

	[SerializeField]
	private Transform recycleDropPos;

	[SerializeField]
	private bool needsElectricity;

	[SerializeField]
	private SoundDefinition liftStartSoundDef;

	[SerializeField]
	private SoundDefinition liftStopSoundDef;

	[SerializeField]
	private SoundDefinition liftStopDownSoundDef;

	[SerializeField]
	private SoundDefinition liftLoopSoundDef;

	[SerializeField]
	private GameObjectRef addRemoveLockEffect;

	[SerializeField]
	private GameObjectRef changeLockCodeEffect;

	[SerializeField]
	private GameObjectRef repairEffect;

	[SerializeField]
	private TriggerBase playerTrigger;

	public ChassisBuildOption[] chassisBuildOptions;

	public ItemAmount lockResourceCost;

	private VehicleLiftState vehicleLiftState = VehicleLiftState.Down;

	private Sound liftLoopSound;

	private Vector3 downPos;

	public const Flags Flag_DestroyingChassis = Flags.Reserved6;

	public const float TimeToDestroyChassis = 10f;

	public const Flags Flag_EnteringKeycode = Flags.Reserved7;

	public const Flags Flag_PlayerObstructing = Flags.Reserved8;

	private ModularCar carOccupant => ((Object)(object)lockedOccupant != (Object)null) ? lockedOccupant : occupantTrigger.carOccupant;

	private bool HasOccupant => (Object)(object)carOccupant != (Object)null && carOccupant.IsFullySpawned();

	public bool PlatformIsOccupied { get; private set; }

	public bool HasEditableOccupant { get; private set; }

	public bool HasDriveableOccupant { get; private set; }

	public OccupantLock OccupantLockState { get; private set; }

	private bool LiftIsUp => vehicleLiftState == VehicleLiftState.Up;

	private bool LiftIsMoving => vehicleLiftAnim.isPlaying;

	private bool LiftIsDown => vehicleLiftState == VehicleLiftState.Down;

	public bool IsDestroyingChassis => HasFlag(Flags.Reserved6);

	private bool IsEnteringKeycode => HasFlag(Flags.Reserved7);

	public bool PlayerObstructingLift => HasFlag(Flags.Reserved8);

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("ModularCarGarage.OnRpcMessage", 0);
		try
		{
			if (rpc == 554177909 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_DeselectedLootItem "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_DeselectedLootItem", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.MaxDistance.Test(554177909u, "RPC_DeselectedLootItem", this, player, 3f))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
					try
					{
						TimeWarning val4 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage msg2 = rPCMessage;
							RPC_DeselectedLootItem(msg2);
						}
						finally
						{
							((IDisposable)val4)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in RPC_DeselectedLootItem");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 3683966290u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_DiedWithKeypadOpen "));
				}
				TimeWarning val5 = TimeWarning.New("RPC_DiedWithKeypadOpen", 0);
				try
				{
					TimeWarning val6 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(3683966290u, "RPC_DiedWithKeypadOpen", this, player, 3f))
						{
							return true;
						}
						if (!RPC_Server.MaxDistance.Test(3683966290u, "RPC_DiedWithKeypadOpen", this, player, 3f))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val6)?.Dispose();
					}
					try
					{
						TimeWarning val7 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage msg3 = rPCMessage;
							RPC_DiedWithKeypadOpen(msg3);
						}
						finally
						{
							((IDisposable)val7)?.Dispose();
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in RPC_DiedWithKeypadOpen");
					}
				}
				finally
				{
					((IDisposable)val5)?.Dispose();
				}
				return true;
			}
			if (rpc == 3659332720u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_OpenEditing "));
				}
				TimeWarning val8 = TimeWarning.New("RPC_OpenEditing", 0);
				try
				{
					TimeWarning val9 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(3659332720u, "RPC_OpenEditing", this, player, 3f))
						{
							return true;
						}
						if (!RPC_Server.MaxDistance.Test(3659332720u, "RPC_OpenEditing", this, player, 3f))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val9)?.Dispose();
					}
					try
					{
						TimeWarning val10 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage msg4 = rPCMessage;
							RPC_OpenEditing(msg4);
						}
						finally
						{
							((IDisposable)val10)?.Dispose();
						}
					}
					catch (Exception ex3)
					{
						Debug.LogException(ex3);
						player.Kick("RPC Error in RPC_OpenEditing");
					}
				}
				finally
				{
					((IDisposable)val8)?.Dispose();
				}
				return true;
			}
			if (rpc == 1582295101 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_RepairItem "));
				}
				TimeWarning val11 = TimeWarning.New("RPC_RepairItem", 0);
				try
				{
					TimeWarning val12 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(1582295101u, "RPC_RepairItem", this, player, 3f))
						{
							return true;
						}
						if (!RPC_Server.MaxDistance.Test(1582295101u, "RPC_RepairItem", this, player, 3f))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val12)?.Dispose();
					}
					try
					{
						TimeWarning val13 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage msg5 = rPCMessage;
							RPC_RepairItem(msg5);
						}
						finally
						{
							((IDisposable)val13)?.Dispose();
						}
					}
					catch (Exception ex4)
					{
						Debug.LogException(ex4);
						player.Kick("RPC Error in RPC_RepairItem");
					}
				}
				finally
				{
					((IDisposable)val11)?.Dispose();
				}
				return true;
			}
			if (rpc == 3710764312u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_RequestAddLock "));
				}
				TimeWarning val14 = TimeWarning.New("RPC_RequestAddLock", 0);
				try
				{
					TimeWarning val15 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(3710764312u, "RPC_RequestAddLock", this, player, 3f))
						{
							return true;
						}
						if (!RPC_Server.MaxDistance.Test(3710764312u, "RPC_RequestAddLock", this, player, 3f))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val15)?.Dispose();
					}
					try
					{
						TimeWarning val16 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage msg6 = rPCMessage;
							RPC_RequestAddLock(msg6);
						}
						finally
						{
							((IDisposable)val16)?.Dispose();
						}
					}
					catch (Exception ex5)
					{
						Debug.LogException(ex5);
						player.Kick("RPC Error in RPC_RequestAddLock");
					}
				}
				finally
				{
					((IDisposable)val14)?.Dispose();
				}
				return true;
			}
			if (rpc == 3305106830u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_RequestNewCode "));
				}
				TimeWarning val17 = TimeWarning.New("RPC_RequestNewCode", 0);
				try
				{
					TimeWarning val18 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(3305106830u, "RPC_RequestNewCode", this, player, 3f))
						{
							return true;
						}
						if (!RPC_Server.MaxDistance.Test(3305106830u, "RPC_RequestNewCode", this, player, 3f))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val18)?.Dispose();
					}
					try
					{
						TimeWarning val19 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage msg7 = rPCMessage;
							RPC_RequestNewCode(msg7);
						}
						finally
						{
							((IDisposable)val19)?.Dispose();
						}
					}
					catch (Exception ex6)
					{
						Debug.LogException(ex6);
						player.Kick("RPC Error in RPC_RequestNewCode");
					}
				}
				finally
				{
					((IDisposable)val17)?.Dispose();
				}
				return true;
			}
			if (rpc == 1046853419 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_RequestRemoveLock "));
				}
				TimeWarning val20 = TimeWarning.New("RPC_RequestRemoveLock", 0);
				try
				{
					TimeWarning val21 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(1046853419u, "RPC_RequestRemoveLock", this, player, 3f))
						{
							return true;
						}
						if (!RPC_Server.MaxDistance.Test(1046853419u, "RPC_RequestRemoveLock", this, player, 3f))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val21)?.Dispose();
					}
					try
					{
						TimeWarning val22 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage msg8 = rPCMessage;
							RPC_RequestRemoveLock(msg8);
						}
						finally
						{
							((IDisposable)val22)?.Dispose();
						}
					}
					catch (Exception ex7)
					{
						Debug.LogException(ex7);
						player.Kick("RPC Error in RPC_RequestRemoveLock");
					}
				}
				finally
				{
					((IDisposable)val20)?.Dispose();
				}
				return true;
			}
			if (rpc == 4033916654u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_SelectedLootItem "));
				}
				TimeWarning val23 = TimeWarning.New("RPC_SelectedLootItem", 0);
				try
				{
					TimeWarning val24 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.MaxDistance.Test(4033916654u, "RPC_SelectedLootItem", this, player, 3f))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val24)?.Dispose();
					}
					try
					{
						TimeWarning val25 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage msg9 = rPCMessage;
							RPC_SelectedLootItem(msg9);
						}
						finally
						{
							((IDisposable)val25)?.Dispose();
						}
					}
					catch (Exception ex8)
					{
						Debug.LogException(ex8);
						player.Kick("RPC Error in RPC_SelectedLootItem");
					}
				}
				finally
				{
					((IDisposable)val23)?.Dispose();
				}
				return true;
			}
			if (rpc == 2974124904u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_StartDestroyingChassis "));
				}
				TimeWarning val26 = TimeWarning.New("RPC_StartDestroyingChassis", 0);
				try
				{
					TimeWarning val27 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(2974124904u, "RPC_StartDestroyingChassis", this, player, 1uL))
						{
							return true;
						}
						if (!RPC_Server.IsVisible.Test(2974124904u, "RPC_StartDestroyingChassis", this, player, 3f))
						{
							return true;
						}
						if (!RPC_Server.MaxDistance.Test(2974124904u, "RPC_StartDestroyingChassis", this, player, 3f))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val27)?.Dispose();
					}
					try
					{
						TimeWarning val28 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage msg10 = rPCMessage;
							RPC_StartDestroyingChassis(msg10);
						}
						finally
						{
							((IDisposable)val28)?.Dispose();
						}
					}
					catch (Exception ex9)
					{
						Debug.LogException(ex9);
						player.Kick("RPC Error in RPC_StartDestroyingChassis");
					}
				}
				finally
				{
					((IDisposable)val26)?.Dispose();
				}
				return true;
			}
			if (rpc == 3872977075u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_StartKeycodeEntry "));
				}
				TimeWarning val29 = TimeWarning.New("RPC_StartKeycodeEntry", 0);
				try
				{
					TimeWarning val30 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.MaxDistance.Test(3872977075u, "RPC_StartKeycodeEntry", this, player, 3f))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val30)?.Dispose();
					}
					try
					{
						TimeWarning val31 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage msg11 = rPCMessage;
							RPC_StartKeycodeEntry(msg11);
						}
						finally
						{
							((IDisposable)val31)?.Dispose();
						}
					}
					catch (Exception ex10)
					{
						Debug.LogException(ex10);
						player.Kick("RPC Error in RPC_StartKeycodeEntry");
					}
				}
				finally
				{
					((IDisposable)val29)?.Dispose();
				}
				return true;
			}
			if (rpc == 3830531963u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_StopDestroyingChassis "));
				}
				TimeWarning val32 = TimeWarning.New("RPC_StopDestroyingChassis", 0);
				try
				{
					TimeWarning val33 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(3830531963u, "RPC_StopDestroyingChassis", this, player, 1uL))
						{
							return true;
						}
						if (!RPC_Server.IsVisible.Test(3830531963u, "RPC_StopDestroyingChassis", this, player, 3f))
						{
							return true;
						}
						if (!RPC_Server.MaxDistance.Test(3830531963u, "RPC_StopDestroyingChassis", this, player, 3f))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val33)?.Dispose();
					}
					try
					{
						TimeWarning val34 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage msg12 = rPCMessage;
							RPC_StopDestroyingChassis(msg12);
						}
						finally
						{
							((IDisposable)val34)?.Dispose();
						}
					}
					catch (Exception ex11)
					{
						Debug.LogException(ex11);
						player.Kick("RPC Error in RPC_StopDestroyingChassis");
					}
				}
				finally
				{
					((IDisposable)val32)?.Dispose();
				}
				return true;
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	protected void FixedUpdate()
	{
		if (!base.isServer || magnetSnap == null)
		{
			return;
		}
		if ((Object)(object)playerTrigger != (Object)null)
		{
			bool hasAnyContents = playerTrigger.HasAnyContents;
			if (PlayerObstructingLift != hasAnyContents)
			{
				SetFlag(Flags.Reserved8, hasAnyContents);
			}
		}
		UpdateCarOccupant();
		if (HasOccupant && carOccupant.CouldBeEdited() && carOccupant.GetSpeed() <= 1f)
		{
			if (IsOn() || !carOccupant.IsComplete())
			{
				if ((Object)(object)lockedOccupant == (Object)null && !carOccupant.rigidBody.isKinematic)
				{
					GrabOccupant(occupantTrigger.carOccupant);
				}
				magnetSnap.FixedUpdate(((Component)carOccupant).transform);
			}
			if (carOccupant.CarLock.HasALock && !carOccupant.CarLock.CanHaveALock())
			{
				carOccupant.CarLock.RemoveLock();
			}
		}
		else if (HasOccupant && carOccupant.rigidBody.isKinematic)
		{
			ReleaseOccupant();
		}
		if (HasOccupant && IsDestroyingChassis && carOccupant.HasAnyModules)
		{
			StopChassisDestroy();
		}
	}

	internal override void DoServerDestroy()
	{
		if (HasOccupant)
		{
			ReleaseOccupant();
			if (!HasDriveableOccupant)
			{
				carOccupant.Kill(DestroyMode.Gib);
			}
		}
		base.DoServerDestroy();
	}

	public override void ServerInit()
	{
		base.ServerInit();
		magnetSnap = new MagnetSnap(vehicleLiftPos);
		RefreshOnOffState();
		SetOccupantState(hasOccupant: false, editableOccupant: false, driveableOccupant: false, OccupantLock.CannotHaveLock, forced: true);
		RefreshLiftState(forced: true);
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		info.msg.vehicleLift = Pool.Get<VehicleLift>();
		info.msg.vehicleLift.platformIsOccupied = PlatformIsOccupied;
		info.msg.vehicleLift.editableOccupant = HasEditableOccupant;
		info.msg.vehicleLift.driveableOccupant = HasDriveableOccupant;
		info.msg.vehicleLift.occupantLockState = (int)OccupantLockState;
	}

	public override ItemContainerId GetIdealContainer(BasePlayer player, Item item, bool altMove)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		return default(ItemContainerId);
	}

	public override bool PlayerOpenLoot(BasePlayer player, string panelToOpen = "", bool doPositionChecks = true)
	{
		SetFlag(Flags.Reserved7, b: false);
		if ((Object)(object)player == (Object)null)
		{
			return false;
		}
		bool flag = base.PlayerOpenLoot(player, panelToOpen);
		if (!flag)
		{
			return false;
		}
		if (HasEditableOccupant)
		{
			player.inventory.loot.AddContainer(carOccupant.Inventory.ModuleContainer);
			player.inventory.loot.AddContainer(carOccupant.Inventory.ChassisContainer);
			player.inventory.loot.SendImmediate();
		}
		lootingPlayers.Add(player);
		RefreshLiftState();
		return flag;
	}

	public override void PlayerStoppedLooting(BasePlayer player)
	{
		base.PlayerStoppedLooting(player);
		if (!IsEnteringKeycode)
		{
			lootingPlayers.Remove(player);
			RefreshLiftState();
		}
	}

	public override void IOStateChanged(int inputAmount, int inputSlot)
	{
		base.IOStateChanged(inputAmount, inputSlot);
		RefreshOnOffState();
	}

	public bool TryGetModuleForItem(Item item, out BaseVehicleModule result)
	{
		if (!HasOccupant)
		{
			result = null;
			return false;
		}
		result = carOccupant.GetModuleForItem(item);
		return (Object)(object)result != (Object)null;
	}

	private void RefreshOnOffState()
	{
		bool flag = !needsElectricity || currentEnergy >= ConsumptionAmount();
		if (flag != IsOn())
		{
			SetFlag(Flags.On, flag);
		}
	}

	private void UpdateCarOccupant()
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		if (base.isServer)
		{
			if (HasOccupant)
			{
				bool editableOccupant = Vector3.SqrMagnitude(((Component)carOccupant).transform.position - vehicleLiftPos.position) < 1f && carOccupant.CouldBeEdited() && !PlayerObstructingLift;
				bool driveableOccupant = carOccupant.IsComplete();
				SetOccupantState(occupantLockState: carOccupant.CarLock.CanHaveALock() ? ((!carOccupant.CarLock.HasALock) ? OccupantLock.NoLock : OccupantLock.HasLock) : OccupantLock.CannotHaveLock, hasOccupant: HasOccupant, editableOccupant: editableOccupant, driveableOccupant: driveableOccupant);
			}
			else
			{
				SetOccupantState(hasOccupant: false, editableOccupant: false, driveableOccupant: false, OccupantLock.CannotHaveLock);
			}
		}
	}

	private void UpdateOccupantMode()
	{
		if (HasOccupant)
		{
			carOccupant.inEditableLocation = HasEditableOccupant && LiftIsUp;
			carOccupant.immuneToDecay = IsOn();
		}
	}

	private void WakeNearbyRigidbodies()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		List<Collider> list = Pool.GetList<Collider>();
		Vis.Colliders<Collider>(((Component)this).transform.position, 7f, list, 34816, (QueryTriggerInteraction)2);
		foreach (Collider item in list)
		{
			Rigidbody attachedRigidbody = item.attachedRigidbody;
			if ((Object)(object)attachedRigidbody != (Object)null && attachedRigidbody.IsSleeping())
			{
				attachedRigidbody.WakeUp();
			}
			BaseEntity baseEntity = item.ToBaseEntity();
			if ((Object)(object)baseEntity != (Object)null && baseEntity is BaseRidableAnimal baseRidableAnimal && baseRidableAnimal.isServer)
			{
				baseRidableAnimal.UpdateDropToGroundForDuration(2f);
			}
		}
		Pool.FreeList<Collider>(ref list);
	}

	private void EditableOccupantEntered()
	{
		RefreshLoot();
	}

	private void EditableOccupantLeft()
	{
		RefreshLoot();
	}

	private void RefreshLoot()
	{
		List<BasePlayer> list = Pool.GetList<BasePlayer>();
		list.AddRange(lootingPlayers);
		foreach (BasePlayer item in list)
		{
			item.inventory.loot.Clear();
			PlayerOpenLoot(item);
		}
		Pool.FreeList<BasePlayer>(ref list);
	}

	private void GrabOccupant(ModularCar occupant)
	{
		if (!((Object)(object)occupant == (Object)null))
		{
			lockedOccupant = occupant;
			lockedOccupant.DisablePhysics();
		}
	}

	private void ReleaseOccupant()
	{
		if (HasOccupant)
		{
			carOccupant.inEditableLocation = false;
			carOccupant.immuneToDecay = false;
			if ((Object)(object)lockedOccupant != (Object)null)
			{
				lockedOccupant.EnablePhysics();
				lockedOccupant = null;
			}
		}
	}

	private void StopChassisDestroy()
	{
		if (((FacepunchBehaviour)this).IsInvoking((Action)FinishDestroyingChassis))
		{
			((FacepunchBehaviour)this).CancelInvoke((Action)FinishDestroyingChassis);
		}
		SetFlag(Flags.Reserved6, b: false);
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	[RPC_Server.IsVisible(3f)]
	public void RPC_RepairItem(RPCMessage msg)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		BasePlayer player = msg.player;
		ItemId val = msg.read.ItemID();
		if (!((Object)(object)player == (Object)null) && HasOccupant)
		{
			Item vehicleItem = carOccupant.GetVehicleItem(val);
			if (vehicleItem != null)
			{
				RepairBench.RepairAnItem(vehicleItem, player, this, 0f, mustKnowBlueprint: false);
				Effect.server.Run(repairEffect.resourcePath, this, 0u, Vector3.zero, Vector3.zero);
			}
			else
			{
				Debug.LogError((object)(((object)this).GetType().Name + ": Couldn't get item to repair, with ID: " + val));
			}
		}
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	[RPC_Server.IsVisible(3f)]
	public void RPC_OpenEditing(RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if (!((Object)(object)player == (Object)null) && !LiftIsMoving)
		{
			PlayerOpenLoot(player);
		}
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	[RPC_Server.IsVisible(3f)]
	public void RPC_DiedWithKeypadOpen(RPCMessage msg)
	{
		SetFlag(Flags.Reserved7, b: false);
		lootingPlayers.Clear();
		RefreshLiftState();
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	public void RPC_SelectedLootItem(RPCMessage msg)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		BasePlayer player = msg.player;
		ItemId itemUID = msg.read.ItemID();
		if ((Object)(object)player == (Object)null || !player.inventory.loot.IsLooting() || (Object)(object)player.inventory.loot.entitySource != (Object)(object)this || !HasOccupant)
		{
			return;
		}
		Item vehicleItem = carOccupant.GetVehicleItem(itemUID);
		if (vehicleItem == null)
		{
			return;
		}
		bool flag = player.inventory.loot.RemoveContainerAt(3);
		if (TryGetModuleForItem(vehicleItem, out var result))
		{
			if (result is VehicleModuleStorage vehicleModuleStorage)
			{
				IItemContainerEntity container = vehicleModuleStorage.GetContainer();
				if (!container.IsUnityNull())
				{
					player.inventory.loot.AddContainer(container.inventory);
					flag = true;
				}
			}
			else if (result is VehicleModuleCamper vehicleModuleCamper)
			{
				IItemContainerEntity container2 = vehicleModuleCamper.GetContainer();
				if (!container2.IsUnityNull())
				{
					player.inventory.loot.AddContainer(container2.inventory);
					flag = true;
				}
			}
		}
		if (flag)
		{
			player.inventory.loot.SendImmediate();
		}
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	public void RPC_DeselectedLootItem(RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if (player.inventory.loot.IsLooting() && !((Object)(object)player.inventory.loot.entitySource != (Object)(object)this) && player.inventory.loot.RemoveContainerAt(3))
		{
			player.inventory.loot.SendImmediate();
		}
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	public void RPC_StartKeycodeEntry(RPCMessage msg)
	{
		SetFlag(Flags.Reserved7, b: true);
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	[RPC_Server.IsVisible(3f)]
	public void RPC_RequestAddLock(RPCMessage msg)
	{
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		if (!HasOccupant || carOccupant.CarLock.HasALock)
		{
			return;
		}
		BasePlayer player = msg.player;
		if (!((Object)(object)player == (Object)null))
		{
			string code = msg.read.String(256);
			ItemAmount itemAmount = lockResourceCost;
			int amount = player.inventory.GetAmount(itemAmount.itemDef.itemid);
			if ((float)amount >= itemAmount.amount && carOccupant.CarLock.TryAddALock(code, player.userID))
			{
				player.inventory.Take(null, itemAmount.itemDef.itemid, Mathf.CeilToInt(itemAmount.amount));
				Effect.server.Run(addRemoveLockEffect.resourcePath, this, 0u, Vector3.zero, Vector3.zero);
			}
		}
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	[RPC_Server.IsVisible(3f)]
	public void RPC_RequestRemoveLock(RPCMessage msg)
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		if (HasOccupant && carOccupant.CarLock.HasALock)
		{
			carOccupant.CarLock.RemoveLock();
			Effect.server.Run(addRemoveLockEffect.resourcePath, this, 0u, Vector3.zero, Vector3.zero);
		}
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	[RPC_Server.IsVisible(3f)]
	public void RPC_RequestNewCode(RPCMessage msg)
	{
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		if (!HasOccupant || !carOccupant.CarLock.HasALock)
		{
			return;
		}
		BasePlayer player = msg.player;
		if (!((Object)(object)player == (Object)null))
		{
			string newCode = msg.read.String(256);
			if (carOccupant.CarLock.TrySetNewCode(newCode, player.userID))
			{
				Effect.server.Run(changeLockCodeEffect.resourcePath, this, 0u, Vector3.zero, Vector3.zero);
			}
		}
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	[RPC_Server.IsVisible(3f)]
	[RPC_Server.CallsPerSecond(1uL)]
	public void RPC_StartDestroyingChassis(RPCMessage msg)
	{
		if (!carOccupant.HasAnyModules)
		{
			((FacepunchBehaviour)this).Invoke((Action)FinishDestroyingChassis, 10f);
			SetFlag(Flags.Reserved6, b: true);
		}
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	[RPC_Server.IsVisible(3f)]
	[RPC_Server.CallsPerSecond(1uL)]
	public void RPC_StopDestroyingChassis(RPCMessage msg)
	{
		StopChassisDestroy();
	}

	private void FinishDestroyingChassis()
	{
		if (HasOccupant && !carOccupant.HasAnyModules)
		{
			carOccupant.Kill(DestroyMode.Gib);
			SetFlag(Flags.Reserved6, b: false);
		}
	}

	public override void PreProcess(IPrefabProcessor process, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		downPos = ((Component)vehicleLift).transform.position;
	}

	public override void OnFlagsChanged(Flags old, Flags next)
	{
		base.OnFlagsChanged(old, next);
		if (base.isServer)
		{
			UpdateOccupantMode();
		}
	}

	public override bool CanBeLooted(BasePlayer player)
	{
		return IsOn();
	}

	public override int ConsumptionAmount()
	{
		return 5;
	}

	private void SetOccupantState(bool hasOccupant, bool editableOccupant, bool driveableOccupant, OccupantLock occupantLockState, bool forced = false)
	{
		if (PlatformIsOccupied == hasOccupant && HasEditableOccupant == editableOccupant && HasDriveableOccupant == driveableOccupant && OccupantLockState == occupantLockState && !forced)
		{
			return;
		}
		bool hasEditableOccupant = HasEditableOccupant;
		PlatformIsOccupied = hasOccupant;
		HasEditableOccupant = editableOccupant;
		HasDriveableOccupant = driveableOccupant;
		OccupantLockState = occupantLockState;
		if (base.isServer)
		{
			UpdateOccupantMode();
			SendNetworkUpdate();
			if (hasEditableOccupant && !editableOccupant)
			{
				EditableOccupantLeft();
			}
			else if (editableOccupant && !hasEditableOccupant)
			{
				EditableOccupantEntered();
			}
		}
		RefreshLiftState();
	}

	private void RefreshLiftState(bool forced = false)
	{
		VehicleLiftState desiredLiftState = ((IsOpen() || IsEnteringKeycode || (HasEditableOccupant && !HasDriveableOccupant)) ? VehicleLiftState.Up : VehicleLiftState.Down);
		MoveLift(desiredLiftState, 0f, forced);
	}

	private void MoveLift(VehicleLiftState desiredLiftState, float startDelay = 0f, bool forced = false)
	{
		if (this.vehicleLiftState != desiredLiftState || forced)
		{
			VehicleLiftState vehicleLiftState = this.vehicleLiftState;
			this.vehicleLiftState = desiredLiftState;
			if (base.isServer)
			{
				UpdateOccupantMode();
				WakeNearbyRigidbodies();
			}
			if (!((Component)this).gameObject.activeSelf)
			{
				AnimationState val = vehicleLiftAnim[animName];
				val.time = ((desiredLiftState == VehicleLiftState.Up) ? 1f : 0f);
				vehicleLiftAnim.Play();
			}
			else if (desiredLiftState == VehicleLiftState.Up)
			{
				((FacepunchBehaviour)this).Invoke((Action)MoveLiftUp, startDelay);
			}
			else
			{
				((FacepunchBehaviour)this).Invoke((Action)MoveLiftDown, startDelay);
			}
		}
	}

	private void MoveLiftUp()
	{
		AnimationState val = vehicleLiftAnim[animName];
		val.speed = val.length / liftMoveTime;
		vehicleLiftAnim.Play();
	}

	private void MoveLiftDown()
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		AnimationState val = vehicleLiftAnim[animName];
		val.speed = val.length / liftMoveTime;
		if (!vehicleLiftAnim.isPlaying && Vector3.Distance(((Component)vehicleLift).transform.position, downPos) > 0.01f)
		{
			val.time = 1f;
		}
		val.speed *= -1f;
		vehicleLiftAnim.Play();
	}
}
