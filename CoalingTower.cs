using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

public class CoalingTower : IOEntity, INotifyEntityTrigger
{
	public enum ActionAttemptStatus
	{
		NoError,
		GenericError,
		NoTrainCar,
		NoNextTrainCar,
		NoPrevTrainCar,
		TrainIsMoving,
		OutputIsFull,
		AlreadyShunting,
		TrainHasThrottle
	}

	[Header("Coaling Tower")]
	[SerializeField]
	private BoxCollider unloadingBounds;

	[SerializeField]
	private GameObjectRef oreStoragePrefab;

	[SerializeField]
	private GameObjectRef fuelStoragePrefab;

	[SerializeField]
	private MeshRenderer[] signalLightsExterior;

	[SerializeField]
	private MeshRenderer[] signalLightsInterior;

	[ColorUsage(false, true)]
	public Color greenLightOnColour;

	[ColorUsage(false, true)]
	public Color yellowLightOnColour;

	[SerializeField]
	private Animator vacuumAnimator;

	[SerializeField]
	private float vacuumStartDelay = 2f;

	[FormerlySerializedAs("unloadingFXContainer")]
	[SerializeField]
	private ParticleSystemContainer unloadingFXContainerOre;

	[SerializeField]
	private ParticleSystem[] unloadingFXMain;

	[SerializeField]
	private ParticleSystem[] unloadingFXDust;

	[SerializeField]
	private ParticleSystemContainer unloadingFXContainerFuel;

	[Header("Coaling Tower Text")]
	[SerializeField]
	private TokenisedPhrase noTraincar;

	[SerializeField]
	private TokenisedPhrase noNextTraincar;

	[SerializeField]
	private TokenisedPhrase noPrevTraincar;

	[SerializeField]
	private TokenisedPhrase trainIsMoving;

	[SerializeField]
	private TokenisedPhrase outputIsFull;

	[SerializeField]
	private TokenisedPhrase trainHasThrottle;

	[Header("Coaling Tower Audio")]
	[SerializeField]
	private GameObject buttonSoundPos;

	[SerializeField]
	private SoundDefinition buttonPressSound;

	[SerializeField]
	private SoundDefinition buttonReleaseSound;

	[SerializeField]
	private SoundDefinition failedActionSound;

	[SerializeField]
	private SoundDefinition failedShuntAlarmSound;

	[SerializeField]
	private SoundDefinition armMovementLower;

	[SerializeField]
	private SoundDefinition armMovementRaise;

	[SerializeField]
	private SoundDefinition suctionAirStart;

	[SerializeField]
	private SoundDefinition suctionAirStop;

	[SerializeField]
	private SoundDefinition suctionAirLoop;

	[SerializeField]
	private SoundDefinition suctionOreStart;

	[SerializeField]
	private SoundDefinition suctionOreLoop;

	[SerializeField]
	private SoundDefinition suctionOreStop;

	[SerializeField]
	private SoundDefinition suctionOreInteriorLoop;

	[SerializeField]
	private SoundDefinition oreBinLoop;

	[SerializeField]
	private SoundDefinition suctionFluidStart;

	[SerializeField]
	private SoundDefinition suctionFluidLoop;

	[SerializeField]
	private SoundDefinition suctionFluidStop;

	[SerializeField]
	private SoundDefinition suctionFluidInteriorLoop;

	[SerializeField]
	private SoundDefinition fluidTankLoop;

	[SerializeField]
	private GameObject interiorPipeSoundLocation;

	[SerializeField]
	private GameObject armMovementSoundLocation;

	[SerializeField]
	private GameObject armSuctionSoundLocation;

	[SerializeField]
	private GameObject oreBinSoundLocation;

	[SerializeField]
	private GameObject fluidTankSoundLocation;

	private NetworkedProperty<int> LootTypeIndex;

	private EntityRef<TrainCar> activeTrainCarRef;

	private EntityRef<TrainCarUnloadable> activeUnloadableRef;

	private const Flags LinedUpFlag = Flags.Reserved2;

	private const Flags HasUnloadableFlag = Flags.Reserved1;

	private const Flags UnloadingInProgressFlag = Flags.Busy;

	private const Flags MoveToNextInProgressFlag = Flags.Reserved3;

	private const Flags MoveToPrevInProgressFlag = Flags.Reserved4;

	private EntityRef<OreHopper> oreStorageInstance;

	private EntityRef<PercentFullStorageContainer> fuelStorageInstance;

	public const float TIME_TO_EMPTY = 40f;

	private static List<CoalingTower> unloadersInWorld = new List<CoalingTower>();

	private Sound armMovementLoopSound;

	private Sound suctionAirLoopSound;

	private Sound suctionMaterialLoopSound;

	private Sound interiorPipeLoopSound;

	private Sound unloadDestinationSound;

	private TrainCarUnloadable tcUnloadingNow;

	private bool HasTrainCar => activeTrainCarRef.IsValid(base.isServer);

	private bool HasUnloadable => activeUnloadableRef.IsValid(base.isServer);

	private bool HasUnloadableLinedUp => HasFlag(Flags.Reserved2);

	public Vector3 UnloadingPos { get; private set; }

	public override void InitShared()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		base.InitShared();
		LootTypeIndex = new NetworkedProperty<int>(this);
		UnloadingPos = ((Component)unloadingBounds).transform.position + ((Component)unloadingBounds).transform.rotation * unloadingBounds.center;
		unloadersInWorld.Add(this);
	}

	public override void DestroyShared()
	{
		base.DestroyShared();
		unloadersInWorld.Remove(this);
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (info.msg.coalingTower != null)
		{
			LootTypeIndex.Value = info.msg.coalingTower.lootTypeIndex;
			oreStorageInstance.uid = info.msg.coalingTower.oreStorageID;
			fuelStorageInstance.uid = info.msg.coalingTower.fuelStorageID;
		}
	}

	public static bool IsUnderAnUnloader(TrainCar trainCar, out bool isLinedUp, out Vector3 unloaderPos)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		foreach (CoalingTower item in unloadersInWorld)
		{
			if (item.TrainCarIsUnder(trainCar, out isLinedUp))
			{
				unloaderPos = item.UnloadingPos;
				return true;
			}
		}
		isLinedUp = false;
		unloaderPos = Vector3.zero;
		return false;
	}

	public bool TrainCarIsUnder(TrainCar trainCar, out bool isLinedUp)
	{
		isLinedUp = false;
		if (!trainCar.IsValid())
		{
			return false;
		}
		TrainCarUnloadable activeUnloadable = GetActiveUnloadable();
		if ((Object)(object)activeUnloadable != (Object)null && activeUnloadable.EqualNetID(trainCar))
		{
			isLinedUp = HasUnloadableLinedUp;
			return true;
		}
		return false;
	}

	private OreHopper GetOreStorage()
	{
		OreHopper oreHopper = oreStorageInstance.Get(base.isServer);
		if (oreHopper.IsValid())
		{
			return oreHopper;
		}
		return null;
	}

	private PercentFullStorageContainer GetFuelStorage()
	{
		PercentFullStorageContainer percentFullStorageContainer = fuelStorageInstance.Get(base.isServer);
		if (percentFullStorageContainer.IsValid())
		{
			return percentFullStorageContainer;
		}
		return null;
	}

	private TrainCar GetActiveTrainCar()
	{
		TrainCar trainCar = activeTrainCarRef.Get(base.isServer);
		if (trainCar.IsValid())
		{
			return trainCar;
		}
		return null;
	}

	private TrainCarUnloadable GetActiveUnloadable()
	{
		TrainCarUnloadable trainCarUnloadable = activeUnloadableRef.Get(base.isServer);
		if (trainCarUnloadable.IsValid())
		{
			return trainCarUnloadable;
		}
		return null;
	}

	private bool OutputBinIsFull()
	{
		TrainCarUnloadable activeUnloadable = GetActiveUnloadable();
		if ((Object)(object)activeUnloadable == (Object)null)
		{
			return false;
		}
		switch (activeUnloadable.wagonType)
		{
		case TrainCarUnloadable.WagonType.Lootboxes:
			return false;
		case TrainCarUnloadable.WagonType.Fuel:
		{
			PercentFullStorageContainer fuelStorage = GetFuelStorage();
			if (!((Object)(object)fuelStorage != (Object)null))
			{
				return false;
			}
			return fuelStorage.IsFull();
		}
		default:
		{
			OreHopper oreStorage = GetOreStorage();
			if (!((Object)(object)oreStorage != (Object)null))
			{
				return false;
			}
			return oreStorage.IsFull();
		}
		}
	}

	private bool WagonIsEmpty()
	{
		TrainCarUnloadable activeUnloadable = GetActiveUnloadable();
		if ((Object)(object)activeUnloadable != (Object)null)
		{
			return activeUnloadable.GetOrePercent() == 0f;
		}
		return true;
	}

	private bool CanUnloadNow(out ActionAttemptStatus attemptStatus)
	{
		if (!HasUnloadableLinedUp)
		{
			attemptStatus = ActionAttemptStatus.NoTrainCar;
			return false;
		}
		if (OutputBinIsFull())
		{
			attemptStatus = ActionAttemptStatus.OutputIsFull;
			return false;
		}
		attemptStatus = ActionAttemptStatus.NoError;
		return IsPowered();
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		info.msg.coalingTower = Pool.Get<CoalingTower>();
		info.msg.coalingTower.lootTypeIndex = LootTypeIndex;
		info.msg.coalingTower.oreStorageID = oreStorageInstance.uid;
		info.msg.coalingTower.fuelStorageID = fuelStorageInstance.uid;
		info.msg.coalingTower.activeUnloadableID = activeTrainCarRef.uid;
	}

	public override void PostServerLoad()
	{
		base.PostServerLoad();
		SetFlag(Flags.Reserved2, b: false, recursive: false, networkupdate: false);
		SetFlag(Flags.Reserved1, b: false, recursive: false, networkupdate: false);
		SetFlag(Flags.Busy, b: false, recursive: false, networkupdate: false);
		SetFlag(Flags.Reserved3, b: false, recursive: false, networkupdate: false);
		SetFlag(Flags.Reserved4, b: false, recursive: false, networkupdate: false);
		SendNetworkUpdate();
	}

	protected override void OnChildAdded(BaseEntity child)
	{
		base.OnChildAdded(child);
		if (base.isServer)
		{
			if (child.prefabID == oreStoragePrefab.GetEntity().prefabID)
			{
				oreStorageInstance.Set((OreHopper)child);
			}
			else if (child.prefabID == fuelStoragePrefab.GetEntity().prefabID)
			{
				fuelStorageInstance.Set((PercentFullStorageContainer)child);
			}
		}
	}

	public void OnEmpty()
	{
		ClearActiveTrainCar();
	}

	public void OnEntityEnter(BaseEntity ent)
	{
		if (ent.IsValid() && !ent.isClient)
		{
			TrainCar trainCar = ent as TrainCar;
			if ((Object)(object)trainCar != (Object)null)
			{
				SetActiveTrainCar(trainCar);
			}
		}
	}

	public void OnEntityLeave(BaseEntity ent)
	{
		if (ent.IsValid() && !ent.isClient)
		{
			BaseEntity baseEntity = ent.parentEntity.Get(base.isServer);
			TrainCar trainCar = activeTrainCarRef.Get(serverside: true);
			if ((Object)(object)trainCar == (Object)(object)ent && (Object)(object)trainCar != (Object)(object)baseEntity)
			{
				ClearActiveTrainCar();
			}
		}
	}

	private void SetActiveTrainCar(TrainCar trainCar)
	{
		if (!((Object)(object)GetActiveTrainCar() == (Object)(object)trainCar))
		{
			activeTrainCarRef.Set(trainCar);
			if (trainCar is TrainCarUnloadable entity)
			{
				activeUnloadableRef.Set(entity);
			}
			else
			{
				activeUnloadableRef.Set(null);
			}
			bool num = activeUnloadableRef.IsValid(serverside: true);
			CheckWagonLinedUp(networkUpdate: false);
			if (num)
			{
				((FacepunchBehaviour)this).InvokeRandomized((Action)CheckWagonLinedUp, 0.15f, 0.15f, 0.015f);
			}
			else
			{
				((FacepunchBehaviour)this).CancelInvoke((Action)CheckWagonLinedUp);
			}
			SendNetworkUpdate();
		}
	}

	private void ClearActiveTrainCar()
	{
		SetActiveTrainCar(null);
	}

	private void CheckWagonLinedUp()
	{
		CheckWagonLinedUp(networkUpdate: true);
	}

	private void CheckWagonLinedUp(bool networkUpdate)
	{
		bool b = false;
		TrainCarUnloadable activeUnloadable = GetActiveUnloadable();
		if ((Object)(object)activeUnloadable != (Object)null)
		{
			b = activeUnloadable.IsLinedUpToUnload(unloadingBounds);
		}
		SetFlag(Flags.Reserved2, b, recursive: false, networkUpdate);
	}

	private bool TryUnloadActiveWagon(out ActionAttemptStatus attemptStatus)
	{
		TrainCarUnloadable activeUnloadable = GetActiveUnloadable();
		if ((Object)(object)activeUnloadable == (Object)null)
		{
			attemptStatus = ActionAttemptStatus.NoTrainCar;
			return false;
		}
		_ = activeUnloadable.wagonType;
		if (!CanUnloadNow(out attemptStatus))
		{
			return false;
		}
		SetFlag(Flags.Busy, b: true);
		((FacepunchBehaviour)this).Invoke((Action)WagonBeginUnloadAnim, vacuumStartDelay);
		return true;
	}

	private void WagonBeginUnloadAnim()
	{
		TrainCarUnloadable activeUnloadable = GetActiveUnloadable();
		if ((Object)(object)activeUnloadable == (Object)null)
		{
			SetFlag(Flags.Busy, b: false);
			return;
		}
		if (!activeUnloadable.TryGetLootType(out var lootOption))
		{
			SetFlag(Flags.Busy, b: false);
			return;
		}
		TrainWagonLootData.instance.TryGetIndexFromLoot(lootOption, out var index);
		LootTypeIndex.Value = index;
		tcUnloadingNow = activeUnloadable;
		tcUnloadingNow.BeginUnloadAnimation();
		float num = 4f;
		((FacepunchBehaviour)this).InvokeRepeating((Action)EmptyTenPercent, 0f, num);
	}

	private void EmptyTenPercent()
	{
		if (!IsPowered())
		{
			EndEmptyProcess(ActionAttemptStatus.GenericError);
			return;
		}
		if (!HasUnloadableLinedUp)
		{
			EndEmptyProcess(ActionAttemptStatus.NoTrainCar);
			return;
		}
		TrainCarUnloadable activeUnloadable = GetActiveUnloadable();
		if ((Object)(object)tcUnloadingNow == (Object)null || (Object)(object)activeUnloadable != (Object)(object)tcUnloadingNow)
		{
			EndEmptyProcess(ActionAttemptStatus.NoTrainCar);
			return;
		}
		StorageContainer storageContainer = tcUnloadingNow.GetStorageContainer();
		if (storageContainer.inventory == null || !TrainWagonLootData.instance.TryGetLootFromIndex(LootTypeIndex, out var lootOption))
		{
			EndEmptyProcess(ActionAttemptStatus.NoTrainCar);
			return;
		}
		bool flag = tcUnloadingNow.wagonType != TrainCarUnloadable.WagonType.Fuel;
		ItemContainer itemContainer = null;
		PercentFullStorageContainer percentFullStorageContainer = (flag ? GetOreStorage() : GetFuelStorage());
		if ((Object)(object)percentFullStorageContainer != (Object)null)
		{
			itemContainer = percentFullStorageContainer.inventory;
		}
		if (itemContainer == null)
		{
			EndEmptyProcess(ActionAttemptStatus.GenericError);
			return;
		}
		ItemContainer inventory = storageContainer.inventory;
		ItemContainer newcontainer = itemContainer;
		int iAmount = Mathf.RoundToInt((float)lootOption.maxLootAmount / 10f);
		List<Item> list = Pool.GetList<Item>();
		int num = inventory.Take(list, lootOption.lootItem.itemid, iAmount);
		bool flag2 = true;
		if (num > 0)
		{
			foreach (Item item in list)
			{
				if (tcUnloadingNow.wagonType == TrainCarUnloadable.WagonType.Lootboxes)
				{
					item.Remove();
					continue;
				}
				bool flag3 = item.MoveToContainer(newcontainer);
				if (!flag2 || flag3)
				{
					continue;
				}
				item.MoveToContainer(inventory);
				flag2 = false;
				break;
			}
		}
		Pool.FreeList<Item>(ref list);
		float orePercent = tcUnloadingNow.GetOrePercent();
		if (orePercent == 0f)
		{
			EndEmptyProcess(ActionAttemptStatus.NoError);
		}
		else if (!flag2)
		{
			EndEmptyProcess(ActionAttemptStatus.OutputIsFull);
		}
		else if (flag)
		{
			tcUnloadingNow.SetVisualOreLevel(orePercent);
		}
	}

	private void EndEmptyProcess(ActionAttemptStatus status)
	{
		((FacepunchBehaviour)this).CancelInvoke((Action)EmptyTenPercent);
		((FacepunchBehaviour)this).CancelInvoke((Action)WagonBeginUnloadAnim);
		if ((Object)(object)tcUnloadingNow != (Object)null)
		{
			tcUnloadingNow.EndEmptyProcess();
			tcUnloadingNow = null;
		}
		SetFlag(Flags.Busy, b: false, recursive: false, networkupdate: false);
		SendNetworkUpdate();
		if (status != 0)
		{
			ClientRPC(null, "ActionFailed", (byte)status, arg2: false);
		}
	}

	private bool TryShuntTrain(bool next, out ActionAttemptStatus attemptStatus)
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		if (!IsPowered() || HasFlag(Flags.Reserved3) || HasFlag(Flags.Reserved4))
		{
			attemptStatus = ActionAttemptStatus.GenericError;
			return false;
		}
		TrainCar activeTrainCar = GetActiveTrainCar();
		if ((Object)(object)activeTrainCar == (Object)null)
		{
			attemptStatus = ActionAttemptStatus.NoTrainCar;
			return false;
		}
		Vector3 unloadingPos = UnloadingPos;
		unloadingPos.y = 0f;
		TrainCar result;
		if (activeTrainCar is TrainCarUnloadable && !HasUnloadableLinedUp)
		{
			Vector3 position = ((Component)activeTrainCar).transform.position;
			Vector3 val = unloadingPos - position;
			if (Vector3.Dot(((Component)this).transform.forward, val) >= 0f == next)
			{
				result = activeTrainCar;
				goto IL_00ba;
			}
		}
		if (!activeTrainCar.TryGetTrainCar(next, ((Component)this).transform.forward, out result))
		{
			attemptStatus = (next ? ActionAttemptStatus.NoNextTrainCar : ActionAttemptStatus.NoPrevTrainCar);
			return false;
		}
		goto IL_00ba;
		IL_00ba:
		Vector3 position2 = ((Component)result).transform.position;
		position2.y = 0f;
		Vector3 shuntDirection = unloadingPos - position2;
		float magnitude = ((Vector3)(ref shuntDirection)).magnitude;
		return activeTrainCar.completeTrain.TryShuntCarTo(shuntDirection, magnitude, result, ShuntEnded, out attemptStatus);
	}

	private void ShuntEnded(ActionAttemptStatus status)
	{
		SetFlag(Flags.Reserved3, b: false);
		SetFlag(Flags.Reserved4, b: false);
		if (status != 0)
		{
			ClientRPC(null, "IssueDuringShunt");
		}
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	private void RPC_Unload(RPCMessage msg)
	{
		if (!TryUnloadActiveWagon(out var attemptStatus) && (Object)(object)msg.player != (Object)null)
		{
			ClientRPCPlayer(null, msg.player, "ActionFailed", (byte)attemptStatus, arg2: true);
		}
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	private void RPC_Next(RPCMessage msg)
	{
		if (TryShuntTrain(next: true, out var attemptStatus))
		{
			SetFlag(Flags.Reserved3, b: true);
		}
		else if ((Object)(object)msg.player != (Object)null)
		{
			ClientRPCPlayer(null, msg.player, "ActionFailed", (byte)attemptStatus, arg2: true);
		}
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	private void RPC_Prev(RPCMessage msg)
	{
		if (TryShuntTrain(next: false, out var attemptStatus))
		{
			SetFlag(Flags.Reserved4, b: true);
		}
		else if ((Object)(object)msg.player != (Object)null)
		{
			ClientRPCPlayer(null, msg.player, "ActionFailed", (byte)attemptStatus, arg2: true);
		}
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("CoalingTower.OnRpcMessage", 0);
		try
		{
			if (rpc == 3071873383u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - RPC_Next "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_Next", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.MaxDistance.Test(3071873383u, "RPC_Next", this, player, 3f))
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
							RPC_Next(msg2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in RPC_Next");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 3656312045u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - RPC_Prev "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_Prev", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.MaxDistance.Test(3656312045u, "RPC_Prev", this, player, 3f))
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
							RPC_Prev(msg3);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in RPC_Prev");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 998476828 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - RPC_Unload "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_Unload", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.MaxDistance.Test(998476828u, "RPC_Unload", this, player, 3f))
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
							RPCMessage msg4 = rPCMessage;
							RPC_Unload(msg4);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex3)
					{
						Debug.LogException(ex3);
						player.Kick("RPC Error in RPC_Unload");
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
}
