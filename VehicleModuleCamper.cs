using System;
using System.Collections.Generic;
using System.Text;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using Rust.Modular;
using UnityEngine;
using UnityEngine.Assertions;

public class VehicleModuleCamper : VehicleModuleSeating
{
	public GameObjectRef SleepingBagEntity;

	public Transform[] SleepingBagPoints;

	public GameObjectRef LockerEntity;

	public Transform LockerPoint;

	public GameObjectRef BbqEntity;

	public Transform BbqPoint;

	public GameObjectRef StorageEntity;

	public Transform StoragePoint;

	private EntityRef<BaseOven> activeBbq;

	private EntityRef<Locker> activeLocker;

	private EntityRef<StorageContainer> activeStorage;

	private bool wasLoaded;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("VehicleModuleCamper.OnRpcMessage", 0);
		try
		{
			if (rpc == 2501069650u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - RPC_OpenLocker "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_OpenLocker", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.MaxDistance.Test(2501069650u, "RPC_OpenLocker", this, player, 3f))
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
							RPC_OpenLocker(msg2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in RPC_OpenLocker");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 4185921214u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - RPC_OpenStorage "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_OpenStorage", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.MaxDistance.Test(4185921214u, "RPC_OpenStorage", this, player, 3f))
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
							RPC_OpenStorage(msg3);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in RPC_OpenStorage");
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
		activeBbq.Set(null);
		activeLocker.Set(null);
		activeStorage.Set(null);
		wasLoaded = false;
	}

	public override void ModuleAdded(BaseModularVehicle vehicle, int firstSocketIndex)
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		base.ModuleAdded(vehicle, firstSocketIndex);
		if (!base.isServer)
		{
			return;
		}
		if (!Application.isLoadingSave && !wasLoaded)
		{
			for (int i = 0; i < SleepingBagPoints.Length; i++)
			{
				SleepingBagCamper sleepingBagCamper = base.gameManager.CreateEntity(SleepingBagEntity.resourcePath, SleepingBagPoints[i].localPosition, SleepingBagPoints[i].localRotation) as SleepingBagCamper;
				if ((Object)(object)sleepingBagCamper != (Object)null)
				{
					sleepingBagCamper.SetParent(this);
					sleepingBagCamper.SetSeat(GetSeatAtIndex(i));
					sleepingBagCamper.Spawn();
				}
			}
			PostConditionalRefresh();
			return;
		}
		int num = 0;
		foreach (BaseEntity child in children)
		{
			if (child is SleepingBagCamper sleepingBagCamper2)
			{
				sleepingBagCamper2.SetSeat(GetSeatAtIndex(num++), sendNetworkUpdate: true);
			}
			else if (child is IItemContainerEntity itemContainerEntity)
			{
				ItemContainer inventory = itemContainerEntity.inventory;
				inventory.onItemAddedRemoved = (Action<Item, bool>)Delegate.Combine(inventory.onItemAddedRemoved, new Action<Item, bool>(OnItemAddedRemoved));
			}
		}
	}

	protected override Vector3 ModifySeatPositionLocalSpace(int index, Vector3 desiredPos)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		CamperSeatConfig seatConfig = GetSeatConfig();
		if ((Object)(object)seatConfig != (Object)null && seatConfig.SeatPositions.Length > index)
		{
			return seatConfig.SeatPositions[index].localPosition;
		}
		return base.ModifySeatPositionLocalSpace(index, desiredPos);
	}

	public override void PostServerLoad()
	{
		base.PostServerLoad();
		wasLoaded = true;
	}

	public override void Spawn()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		base.Spawn();
		if (!Application.isLoadingSave)
		{
			Locker locker = base.gameManager.CreateEntity(LockerEntity.resourcePath, LockerPoint.localPosition, LockerPoint.localRotation) as Locker;
			locker.SetParent(this);
			locker.Spawn();
			ItemContainer inventory = locker.inventory;
			inventory.onItemAddedRemoved = (Action<Item, bool>)Delegate.Combine(inventory.onItemAddedRemoved, new Action<Item, bool>(OnItemAddedRemoved));
			activeLocker.Set(locker);
			BaseOven baseOven = base.gameManager.CreateEntity(BbqEntity.resourcePath, BbqPoint.localPosition, BbqPoint.localRotation) as BaseOven;
			baseOven.SetParent(this);
			baseOven.Spawn();
			ItemContainer inventory2 = baseOven.inventory;
			inventory2.onItemAddedRemoved = (Action<Item, bool>)Delegate.Combine(inventory2.onItemAddedRemoved, new Action<Item, bool>(OnItemAddedRemoved));
			activeBbq.Set(baseOven);
			StorageContainer storageContainer = base.gameManager.CreateEntity(StorageEntity.resourcePath, StoragePoint.localPosition, StoragePoint.localRotation) as StorageContainer;
			storageContainer.SetParent(this);
			storageContainer.Spawn();
			ItemContainer inventory3 = storageContainer.inventory;
			inventory3.onItemAddedRemoved = (Action<Item, bool>)Delegate.Combine(inventory3.onItemAddedRemoved, new Action<Item, bool>(OnItemAddedRemoved));
			activeStorage.Set(storageContainer);
			PostConditionalRefresh();
		}
	}

	private void OnItemAddedRemoved(Item item, bool add)
	{
		AssociatedItemInstance?.LockUnlock(!CanBeMovedNowOnVehicle());
	}

	protected override bool CanBeMovedNowOnVehicle()
	{
		foreach (BaseEntity child in children)
		{
			if (child is IItemContainerEntity itemContainerEntity && !itemContainerEntity.IsUnityNull() && !itemContainerEntity.inventory.IsEmpty())
			{
				return false;
			}
		}
		return true;
	}

	protected override void PostConditionalRefresh()
	{
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		base.PostConditionalRefresh();
		if (base.isClient)
		{
			return;
		}
		CamperSeatConfig seatConfig = GetSeatConfig();
		if ((Object)(object)seatConfig != (Object)null && mountPoints != null)
		{
			for (int i = 0; i < mountPoints.Count; i++)
			{
				if ((Object)(object)mountPoints[i].mountable != (Object)null)
				{
					((Component)mountPoints[i].mountable).transform.position = seatConfig.SeatPositions[i].position;
					mountPoints[i].mountable.SendNetworkUpdate();
				}
			}
		}
		if (activeBbq.IsValid(base.isServer) && (Object)(object)seatConfig != (Object)null)
		{
			BaseOven baseOven = activeBbq.Get(serverside: true);
			((Component)baseOven).transform.position = seatConfig.StovePosition.position;
			((Component)baseOven).transform.rotation = seatConfig.StovePosition.rotation;
			baseOven.SendNetworkUpdate();
		}
		if (activeStorage.IsValid(base.isServer) && (Object)(object)seatConfig != (Object)null)
		{
			StorageContainer storageContainer = activeStorage.Get(base.isServer);
			((Component)storageContainer).transform.position = seatConfig.StoragePosition.position;
			((Component)storageContainer).transform.rotation = seatConfig.StoragePosition.rotation;
			storageContainer.SendNetworkUpdate();
		}
	}

	private CamperSeatConfig GetSeatConfig()
	{
		List<ConditionalObject> list = GetConditionals();
		CamperSeatConfig result = null;
		CamperSeatConfig camperSeatConfig = default(CamperSeatConfig);
		foreach (ConditionalObject item in list)
		{
			if (item.gameObject.activeSelf && item.gameObject.TryGetComponent<CamperSeatConfig>(ref camperSeatConfig))
			{
				result = camperSeatConfig;
			}
		}
		return result;
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
		if (info.msg.camperModule == null)
		{
			info.msg.camperModule = Pool.Get<CamperModule>();
		}
		info.msg.camperModule.bbqId = activeBbq.uid;
		info.msg.camperModule.lockerId = activeLocker.uid;
		info.msg.camperModule.storageID = activeStorage.uid;
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	public void RPC_OpenLocker(RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if (!((Object)(object)player == (Object)null) && CanBeLooted(player))
		{
			IItemContainerEntity itemContainerEntity = activeLocker.Get(base.isServer);
			if (!itemContainerEntity.IsUnityNull())
			{
				itemContainerEntity.PlayerOpenLoot(player);
			}
			else
			{
				Debug.LogError((object)(((object)this).GetType().Name + ": No container component found."));
			}
		}
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	public void RPC_OpenStorage(RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if (!((Object)(object)player == (Object)null) && CanBeLooted(player))
		{
			IItemContainerEntity itemContainerEntity = activeStorage.Get(base.isServer);
			if (!itemContainerEntity.IsUnityNull())
			{
				itemContainerEntity.PlayerOpenLoot(player);
			}
			else
			{
				Debug.LogError((object)(((object)this).GetType().Name + ": No container component found."));
			}
		}
	}

	internal override void DoServerDestroy()
	{
		if (vehicle.vehiclesdroploot)
		{
			if (activeStorage.IsValid(base.isServer))
			{
				activeStorage.Get(base.isServer).DropItems();
			}
			if (activeBbq.IsValid(base.isServer))
			{
				activeBbq.Get(base.isServer).DropItems();
			}
			if (activeLocker.IsValid(base.isServer))
			{
				activeLocker.Get(base.isServer).DropItems();
			}
		}
		base.DoServerDestroy();
	}

	public IItemContainerEntity GetContainer()
	{
		Locker locker = activeLocker.Get(base.isServer);
		if ((Object)(object)locker != (Object)null && locker.IsValid() && !locker.inventory.IsEmpty())
		{
			return locker;
		}
		BaseOven baseOven = activeBbq.Get(base.isServer);
		if ((Object)(object)baseOven != (Object)null && baseOven.IsValid() && !baseOven.inventory.IsEmpty())
		{
			return baseOven;
		}
		StorageContainer storageContainer = activeStorage.Get(base.isServer);
		if ((Object)(object)storageContainer != (Object)null && storageContainer.IsValid() && !storageContainer.inventory.IsEmpty())
		{
			return storageContainer;
		}
		return null;
	}

	public override string Admin_Who()
	{
		StringBuilder stringBuilder = new StringBuilder();
		int num = 0;
		foreach (BaseEntity child in children)
		{
			if (child is SleepingBagCamper sleepingBagCamper)
			{
				stringBuilder.AppendLine($"Bag {num++}:");
				stringBuilder.AppendLine(sleepingBagCamper.Admin_Who());
			}
		}
		return stringBuilder.ToString();
	}

	public override bool CanBeLooted(BasePlayer player)
	{
		if (base.IsOnAVehicle && base.Vehicle.IsDead())
		{
			return base.CanBeLooted(player);
		}
		if (base.CanBeLooted(player))
		{
			return IsOnThisModule(player);
		}
		return false;
	}

	public override bool IsOnThisModule(BasePlayer player)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		if (base.IsOnThisModule(player))
		{
			return true;
		}
		if (!player.isMounted)
		{
			return false;
		}
		OBB val = default(OBB);
		((OBB)(ref val))._002Ector(((Component)this).transform, bounds);
		return ((OBB)(ref val)).Contains(player.CenterPoint());
	}

	public override void Load(LoadInfo info)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		base.Load(info);
		if (info.msg.camperModule != null)
		{
			activeBbq.uid = info.msg.camperModule.bbqId;
			activeLocker.uid = info.msg.camperModule.lockerId;
			activeStorage.uid = info.msg.camperModule.storageID;
		}
	}
}
