using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;

public class Door : AnimatedBuildingBlock, INotifyTrigger
{
	public GameObjectRef knockEffect;

	public bool canTakeLock = true;

	public bool hasHatch;

	public bool canTakeCloser;

	public bool canTakeKnocker;

	public bool canNpcOpen = true;

	public bool canHandOpen = true;

	public bool isSecurityDoor;

	public bool canReverseOpen;

	public TriggerNotify[] vehiclePhysBoxes;

	public bool checkPhysBoxesOnOpen;

	public SoundDefinition vehicleCollisionSfx;

	public GameObject[] ClosedColliderRoots;

	[SerializeField]
	[ReadOnly]
	private float openAnimLength = 4f;

	[SerializeField]
	[ReadOnly]
	private float closeAnimLength = 4f;

	public const Flags ReverseOpen = Flags.Reserved1;

	private float decayResetTimeLast = float.NegativeInfinity;

	public NavMeshModifierVolume NavMeshVolumeAnimals;

	public NavMeshModifierVolume NavMeshVolumeHumanoids;

	public NavMeshLink NavMeshLink;

	public NPCDoorTriggerBox NpcTriggerBox;

	private static int nonWalkableArea = -1;

	private static int animalAgentTypeId = -1;

	private static int humanoidAgentTypeId = -1;

	private Dictionary<BasePlayer, TimeSince> woundedOpens = new Dictionary<BasePlayer, TimeSince>();

	private Dictionary<BasePlayer, TimeSince> woundedCloses = new Dictionary<BasePlayer, TimeSince>();

	private float nextKnockTime = float.NegativeInfinity;

	private static int openHash = Animator.StringToHash("open");

	private static int closeHash = Animator.StringToHash("close");

	private static int reverseOpenHash = Animator.StringToHash("reverseOpen");

	private bool HasVehiclePushBoxes
	{
		get
		{
			if (vehiclePhysBoxes != null)
			{
				return vehiclePhysBoxes.Length != 0;
			}
			return false;
		}
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("Door.OnRpcMessage", 0);
		try
		{
			if (rpc == 3999508679u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - RPC_CloseDoor "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_CloseDoor", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.MaxDistance.Test(3999508679u, "RPC_CloseDoor", this, player, 3f))
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
						val3 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage rpc2 = rPCMessage;
							RPC_CloseDoor(rpc2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in RPC_CloseDoor");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 1487779344 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - RPC_KnockDoor "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_KnockDoor", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.MaxDistance.Test(1487779344u, "RPC_KnockDoor", this, player, 3f))
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
						val3 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage rpc3 = rPCMessage;
							RPC_KnockDoor(rpc3);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in RPC_KnockDoor");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 3314360565u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - RPC_OpenDoor "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_OpenDoor", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.MaxDistance.Test(3314360565u, "RPC_OpenDoor", this, player, 3f))
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
						val3 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage rpc4 = rPCMessage;
							RPC_OpenDoor(rpc4);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex3)
					{
						Debug.LogException(ex3);
						player.Kick("RPC Error in RPC_OpenDoor");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 3000490601u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - RPC_ToggleHatch "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_ToggleHatch", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.MaxDistance.Test(3000490601u, "RPC_ToggleHatch", this, player, 3f))
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
						val3 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage rpc5 = rPCMessage;
							RPC_ToggleHatch(rpc5);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex4)
					{
						Debug.LogException(ex4);
						player.Kick("RPC Error in RPC_ToggleHatch");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 3672787865u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - Server_NotifyWoundedClose "));
				}
				TimeWarning val2 = TimeWarning.New("Server_NotifyWoundedClose", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(3672787865u, "Server_NotifyWoundedClose", this, player, 3f))
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
						val3 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage msg2 = rPCMessage;
							Server_NotifyWoundedClose(msg2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex5)
					{
						Debug.LogException(ex5);
						player.Kick("RPC Error in Server_NotifyWoundedClose");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 3730851545u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - Server_NotifyWoundedOpen "));
				}
				TimeWarning val2 = TimeWarning.New("Server_NotifyWoundedOpen", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(3730851545u, "Server_NotifyWoundedOpen", this, player, 3f))
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
						val3 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage msg3 = rPCMessage;
							Server_NotifyWoundedOpen(msg3);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex6)
					{
						Debug.LogException(ex6);
						player.Kick("RPC Error in Server_NotifyWoundedOpen");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
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

	public override void ResetState()
	{
		base.ResetState();
		if (base.isServer)
		{
			decayResetTimeLast = float.NegativeInfinity;
			if (isSecurityDoor && (Object)(object)NavMeshLink != (Object)null)
			{
				SetNavMeshLinkEnabled(wantsOn: false);
			}
			woundedCloses.Clear();
			woundedOpens.Clear();
		}
	}

	public override void ServerInit()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		base.ServerInit();
		if (nonWalkableArea < 0)
		{
			nonWalkableArea = NavMesh.GetAreaFromName("Not Walkable");
		}
		NavMeshBuildSettings settingsByIndex;
		if (animalAgentTypeId < 0)
		{
			settingsByIndex = NavMesh.GetSettingsByIndex(1);
			animalAgentTypeId = ((NavMeshBuildSettings)(ref settingsByIndex)).agentTypeID;
		}
		if ((Object)(object)NavMeshVolumeAnimals == (Object)null)
		{
			NavMeshVolumeAnimals = ((Component)this).gameObject.AddComponent<NavMeshModifierVolume>();
			NavMeshVolumeAnimals.area = nonWalkableArea;
			NavMeshVolumeAnimals.AddAgentType(animalAgentTypeId);
			NavMeshVolumeAnimals.center = Vector3.zero;
			NavMeshVolumeAnimals.size = Vector3.one;
		}
		if (HasSlot(Slot.Lock))
		{
			canNpcOpen = false;
		}
		if (!canNpcOpen)
		{
			if (humanoidAgentTypeId < 0)
			{
				settingsByIndex = NavMesh.GetSettingsByIndex(0);
				humanoidAgentTypeId = ((NavMeshBuildSettings)(ref settingsByIndex)).agentTypeID;
			}
			if ((Object)(object)NavMeshVolumeHumanoids == (Object)null)
			{
				NavMeshVolumeHumanoids = ((Component)this).gameObject.AddComponent<NavMeshModifierVolume>();
				NavMeshVolumeHumanoids.area = nonWalkableArea;
				NavMeshVolumeHumanoids.AddAgentType(humanoidAgentTypeId);
				NavMeshVolumeHumanoids.center = Vector3.zero;
				NavMeshVolumeHumanoids.size = Vector3.one + Vector3.up + Vector3.forward;
			}
		}
		else if ((Object)(object)NpcTriggerBox == (Object)null)
		{
			if (isSecurityDoor)
			{
				NavMeshObstacle obj = ((Component)this).gameObject.AddComponent<NavMeshObstacle>();
				obj.carving = true;
				obj.center = Vector3.zero;
				obj.size = Vector3.one;
				obj.shape = (NavMeshObstacleShape)1;
			}
			NpcTriggerBox = new GameObject("NpcTriggerBox").AddComponent<NPCDoorTriggerBox>();
			NpcTriggerBox.Setup(this);
		}
		AIInformationZone forPoint = AIInformationZone.GetForPoint(((Component)this).transform.position);
		if ((Object)(object)forPoint != (Object)null && (Object)(object)NavMeshLink == (Object)null)
		{
			NavMeshLink = forPoint.GetClosestNavMeshLink(((Component)this).transform.position);
		}
		DisableVehiclePhysBox();
	}

	public override bool HasSlot(Slot slot)
	{
		if (slot == Slot.Lock && canTakeLock)
		{
			return true;
		}
		switch (slot)
		{
		case Slot.UpperModifier:
			return true;
		case Slot.CenterDecoration:
			if (canTakeCloser)
			{
				return true;
			}
			break;
		}
		if (slot == Slot.LowerCenterDecoration && canTakeKnocker)
		{
			return true;
		}
		return base.HasSlot(slot);
	}

	public override bool CanPickup(BasePlayer player)
	{
		if (!IsOpen())
		{
			return false;
		}
		if (Object.op_Implicit((Object)(object)GetSlot(Slot.Lock)))
		{
			return false;
		}
		if (Object.op_Implicit((Object)(object)GetSlot(Slot.UpperModifier)))
		{
			return false;
		}
		if (Object.op_Implicit((Object)(object)GetSlot(Slot.CenterDecoration)))
		{
			return false;
		}
		if (Object.op_Implicit((Object)(object)GetSlot(Slot.LowerCenterDecoration)))
		{
			return false;
		}
		return base.CanPickup(player);
	}

	public void CloseRequest()
	{
		SetOpen(open: false);
	}

	public void SetOpen(bool open, bool suppressBlockageChecks = false)
	{
		SetFlag(Flags.Open, open);
		SendNetworkUpdateImmediate();
		if (isSecurityDoor && (Object)(object)NavMeshLink != (Object)null)
		{
			SetNavMeshLinkEnabled(open);
		}
		if (!suppressBlockageChecks && (!open || checkPhysBoxesOnOpen))
		{
			StartCheckingForBlockages(open);
		}
	}

	public void SetLocked(bool locked)
	{
		SetFlag(Flags.Locked, b: false);
		SendNetworkUpdateImmediate();
	}

	public bool GetPlayerLockPermission(BasePlayer player)
	{
		BaseLock baseLock = GetSlot(Slot.Lock) as BaseLock;
		if ((Object)(object)baseLock == (Object)null)
		{
			return true;
		}
		return baseLock.GetPlayerLockPermission(player);
	}

	public void SetNavMeshLinkEnabled(bool wantsOn)
	{
		if ((Object)(object)NavMeshLink != (Object)null)
		{
			if (wantsOn)
			{
				((Component)NavMeshLink).gameObject.SetActive(true);
				((Behaviour)NavMeshLink).enabled = true;
			}
			else
			{
				((Behaviour)NavMeshLink).enabled = false;
				((Component)NavMeshLink).gameObject.SetActive(false);
			}
		}
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	protected void RPC_OpenDoor(RPCMessage rpc)
	{
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		if (!rpc.player.CanInteract(usableWhileCrawling: true) || !canHandOpen || IsOpen() || IsBusy() || IsLocked() || ((FacepunchBehaviour)this).IsInvoking((Action)DelayedDoorOpening))
		{
			return;
		}
		if (rpc.player.IsWounded())
		{
			if (!woundedOpens.ContainsKey(rpc.player) || !(TimeSince.op_Implicit(woundedOpens[rpc.player]) > 2.5f))
			{
				return;
			}
			woundedOpens.Remove(rpc.player);
		}
		BaseLock baseLock = GetSlot(Slot.Lock) as BaseLock;
		if ((Object)(object)baseLock != (Object)null)
		{
			if (!baseLock.OnTryToOpen(rpc.player))
			{
				return;
			}
			if (baseLock.IsLocked() && Time.realtimeSinceStartup - decayResetTimeLast > 60f)
			{
				BuildingBlock buildingBlock = FindLinkedEntity<BuildingBlock>();
				if (Object.op_Implicit((Object)(object)buildingBlock))
				{
					Decay.BuildingDecayTouch(buildingBlock);
				}
				else
				{
					Decay.RadialDecayTouch(((Component)this).transform.position, 40f, 2097408);
				}
				decayResetTimeLast = Time.realtimeSinceStartup;
			}
		}
		if (canReverseOpen)
		{
			Vector3 val = ((Component)this).transform.InverseTransformPoint(((Component)rpc.player).transform.position);
			SetFlag(Flags.Reserved1, val.x > 0f, recursive: false, networkupdate: false);
		}
		if (ShouldDelayOpen(rpc.player, out var delay))
		{
			((FacepunchBehaviour)this).Invoke((Action)DelayedDoorOpening, delay);
		}
		else
		{
			SetFlag(Flags.Open, b: true);
			SendNetworkUpdateImmediate();
		}
		if (isSecurityDoor && (Object)(object)NavMeshLink != (Object)null)
		{
			SetNavMeshLinkEnabled(wantsOn: true);
		}
		if (checkPhysBoxesOnOpen)
		{
			StartCheckingForBlockages(isOpening: true);
		}
		OnPlayerOpenedDoor(rpc.player);
	}

	private void DelayedDoorOpening()
	{
		SetFlag(Flags.Open, b: true);
		SendNetworkUpdateImmediate();
	}

	protected virtual void OnPlayerOpenedDoor(BasePlayer p)
	{
	}

	protected virtual bool ShouldDelayOpen(BasePlayer forPlayer, out float delay)
	{
		delay = 0f;
		return false;
	}

	private void StartCheckingForBlockages(bool isOpening)
	{
		if (HasVehiclePushBoxes)
		{
			((FacepunchBehaviour)this).Invoke((Action)EnableVehiclePhysBoxes, 0.2f);
			float num = (isOpening ? openAnimLength : closeAnimLength);
			((FacepunchBehaviour)this).Invoke((Action)DisableVehiclePhysBox, num);
		}
	}

	private void StopCheckingForBlockages()
	{
		if (HasVehiclePushBoxes)
		{
			ToggleVehiclePushBoxes(state: false);
			((FacepunchBehaviour)this).CancelInvoke((Action)DisableVehiclePhysBox);
		}
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	private void RPC_CloseDoor(RPCMessage rpc)
	{
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		if (!rpc.player.CanInteract(usableWhileCrawling: true) || !canHandOpen || !IsOpen() || IsBusy() || IsLocked())
		{
			return;
		}
		if (rpc.player.IsWounded())
		{
			if (!woundedCloses.ContainsKey(rpc.player) || !(TimeSince.op_Implicit(woundedCloses[rpc.player]) > 2.5f))
			{
				return;
			}
			woundedCloses.Remove(rpc.player);
		}
		BaseLock baseLock = GetSlot(Slot.Lock) as BaseLock;
		if (!((Object)(object)baseLock != (Object)null) || baseLock.OnTryToClose(rpc.player))
		{
			SetFlag(Flags.Open, b: false);
			SendNetworkUpdateImmediate();
			if (isSecurityDoor && (Object)(object)NavMeshLink != (Object)null)
			{
				SetNavMeshLinkEnabled(wantsOn: false);
			}
			StartCheckingForBlockages(isOpening: false);
		}
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	private void RPC_KnockDoor(RPCMessage rpc)
	{
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		if (!rpc.player.CanInteract(usableWhileCrawling: true) || !knockEffect.isValid || Time.realtimeSinceStartup < nextKnockTime)
		{
			return;
		}
		nextKnockTime = Time.realtimeSinceStartup + 0.5f;
		BaseEntity slot = GetSlot(Slot.LowerCenterDecoration);
		if ((Object)(object)slot != (Object)null)
		{
			DoorKnocker component = ((Component)slot).GetComponent<DoorKnocker>();
			if (Object.op_Implicit((Object)(object)component))
			{
				component.Knock(rpc.player);
				return;
			}
		}
		Effect.server.Run(knockEffect.resourcePath, this, 0u, Vector3.zero, Vector3.zero);
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	private void RPC_ToggleHatch(RPCMessage rpc)
	{
		if (rpc.player.CanInteract(usableWhileCrawling: true) && hasHatch)
		{
			BaseLock baseLock = GetSlot(Slot.Lock) as BaseLock;
			if (!Object.op_Implicit((Object)(object)baseLock) || baseLock.OnTryToOpen(rpc.player))
			{
				SetFlag(Flags.Reserved3, !HasFlag(Flags.Reserved3));
			}
		}
	}

	private void EnableVehiclePhysBoxes()
	{
		ToggleVehiclePushBoxes(state: true);
	}

	private void DisableVehiclePhysBox()
	{
		ToggleVehiclePushBoxes(state: false);
	}

	private void ToggleVehiclePushBoxes(bool state)
	{
		if (vehiclePhysBoxes == null)
		{
			return;
		}
		TriggerNotify[] array = vehiclePhysBoxes;
		foreach (TriggerNotify triggerNotify in array)
		{
			if ((Object)(object)triggerNotify != (Object)null)
			{
				((Component)triggerNotify).gameObject.SetActive(state);
			}
		}
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	private void Server_NotifyWoundedOpen(RPCMessage msg)
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		BasePlayer player = msg.player;
		if (player.IsWounded())
		{
			if (!woundedOpens.ContainsKey(player))
			{
				woundedOpens.Add(player, default(TimeSince));
			}
			else
			{
				woundedOpens[player] = TimeSince.op_Implicit(0f);
			}
			((FacepunchBehaviour)this).Invoke((Action)delegate
			{
				CheckTimedOutPlayers(woundedOpens);
			}, 5f);
		}
	}

	private void CheckTimedOutPlayers(Dictionary<BasePlayer, TimeSince> dictionary)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		List<BasePlayer> list = Pool.GetList<BasePlayer>();
		foreach (KeyValuePair<BasePlayer, TimeSince> item in dictionary)
		{
			if (TimeSince.op_Implicit(item.Value) > 5f)
			{
				list.Add(item.Key);
			}
		}
		foreach (BasePlayer item2 in list)
		{
			if (dictionary.ContainsKey(item2))
			{
				dictionary.Remove(item2);
			}
		}
		Pool.FreeList<BasePlayer>(ref list);
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	private void Server_NotifyWoundedClose(RPCMessage msg)
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		BasePlayer player = msg.player;
		if (player.IsWounded())
		{
			if (!woundedCloses.ContainsKey(player))
			{
				woundedCloses.Add(player, default(TimeSince));
			}
			else
			{
				woundedCloses[player] = TimeSince.op_Implicit(0f);
			}
			((FacepunchBehaviour)this).Invoke((Action)delegate
			{
				CheckTimedOutPlayers(woundedCloses);
			}, 5f);
		}
	}

	private void ReverseDoorAnimation(bool wasOpening)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		if (!((Object)(object)model == (Object)null) && !((Object)(object)model.animator == (Object)null))
		{
			AnimatorStateInfo currentAnimatorStateInfo = model.animator.GetCurrentAnimatorStateInfo(0);
			model.animator.Play(wasOpening ? closeHash : openHash, 0, 1f - ((AnimatorStateInfo)(ref currentAnimatorStateInfo)).normalizedTime);
		}
	}

	public override float BoundsPadding()
	{
		return 2f;
	}

	public void OnObjects(TriggerNotify trigger)
	{
		if (!base.isServer)
		{
			return;
		}
		bool flag = false;
		foreach (BaseEntity entityContent in trigger.entityContents)
		{
			if (entityContent is BaseMountable baseMountable && baseMountable.BlocksDoors)
			{
				flag = true;
				break;
			}
			if (entityContent is BaseVehicleModule baseVehicleModule && (Object)(object)baseVehicleModule.Vehicle != (Object)null && baseVehicleModule.Vehicle.BlocksDoors)
			{
				flag = true;
				break;
			}
		}
		if (flag)
		{
			bool flag2 = HasFlag(Flags.Open);
			SetOpen(!flag2, suppressBlockageChecks: true);
			ReverseDoorAnimation(flag2);
			StopCheckingForBlockages();
			ClientRPC(null, "OnDoorInterrupted", flag2 ? 1 : 0);
		}
	}

	public void OnEmpty()
	{
	}

	public override void OnFlagsChanged(Flags old, Flags next)
	{
		base.OnFlagsChanged(old, next);
		if ((Object)(object)model.animator != (Object)null && ((Component)model.animator).gameObject.activeInHierarchy && canReverseOpen)
		{
			model.animator.SetBool(reverseOpenHash, next.HasFlag(Flags.Reserved1));
		}
		if (base.isServer)
		{
			BaseEntity slot = GetSlot(Slot.UpperModifier);
			if (Object.op_Implicit((Object)(object)slot))
			{
				((Component)slot).SendMessage("Think");
			}
		}
		if (ClosedColliderRoots == null)
		{
			return;
		}
		bool active = !HasFlag(Flags.Open) || HasFlag(Flags.Busy);
		GameObject[] closedColliderRoots = ClosedColliderRoots;
		foreach (GameObject val in closedColliderRoots)
		{
			if ((Object)(object)val != (Object)null)
			{
				val.gameObject.SetActive(active);
			}
		}
	}
}
