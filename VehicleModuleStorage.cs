using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using Rust.Modular;
using UnityEngine;
using UnityEngine.Assertions;

public class VehicleModuleStorage : VehicleModuleSeating
{
	[Serializable]
	public class Storage
	{
		public GameObjectRef storageUnitPrefab;

		public Transform storageUnitPoint;
	}

	[SerializeField]
	private Storage storage;

	private EntityRef storageUnitInstance;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("VehicleModuleStorage.OnRpcMessage", 0);
		try
		{
			if (rpc == 4254195175u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_Open "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_Open", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.MaxDistance.Test(4254195175u, "RPC_Open", this, player, 3f))
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
							RPC_Open(msg2);
						}
						finally
						{
							((IDisposable)val4)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in RPC_Open");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 425471188 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_TryOpenWithKeycode "));
				}
				TimeWarning val5 = TimeWarning.New("RPC_TryOpenWithKeycode", 0);
				try
				{
					TimeWarning val6 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.MaxDistance.Test(425471188u, "RPC_TryOpenWithKeycode", this, player, 3f))
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
							RPC_TryOpenWithKeycode(msg3);
						}
						finally
						{
							((IDisposable)val7)?.Dispose();
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in RPC_TryOpenWithKeycode");
					}
				}
				finally
				{
					((IDisposable)val5)?.Dispose();
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

	public IItemContainerEntity GetContainer()
	{
		BaseEntity baseEntity = storageUnitInstance.Get(base.isServer);
		if ((Object)(object)baseEntity != (Object)null && baseEntity.IsValid())
		{
			return baseEntity as IItemContainerEntity;
		}
		return null;
	}

	public override void Load(LoadInfo info)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		base.Load(info);
		storageUnitInstance.uid = info.msg.simpleUID.uid;
	}

	public override void Spawn()
	{
		base.Spawn();
		if (!Application.isLoadingSave && ((Component)storage.storageUnitPoint).gameObject.activeSelf)
		{
			CreateStorageEntity();
		}
	}

	public override void PostServerLoad()
	{
		base.PostServerLoad();
		IItemContainerEntity container = GetContainer();
		if (!container.IsUnityNull())
		{
			ItemContainer inventory = container.inventory;
			inventory.onItemAddedRemoved = (Action<Item, bool>)Delegate.Combine(inventory.onItemAddedRemoved, new Action<Item, bool>(OnItemAddedRemoved));
		}
	}

	private void OnItemAddedRemoved(Item item, bool add)
	{
		AssociatedItemInstance?.LockUnlock(!CanBeMovedNowOnVehicle());
	}

	public override void NonUserSpawn()
	{
		EngineStorage engineStorage = GetContainer() as EngineStorage;
		if ((Object)(object)engineStorage != (Object)null)
		{
			engineStorage.NonUserSpawn();
		}
	}

	internal override void DoServerDestroy()
	{
		if (vehicle.vehiclesdroploot)
		{
			IItemContainerEntity container = GetContainer();
			if (!container.IsUnityNull())
			{
				container.DropItems();
			}
		}
		base.DoServerDestroy();
	}

	public override void Save(SaveInfo info)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		base.Save(info);
		info.msg.simpleUID = Pool.Get<SimpleUID>();
		info.msg.simpleUID.uid = storageUnitInstance.uid;
	}

	public void CreateStorageEntity()
	{
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		if (IsFullySpawned() && base.isServer && !storageUnitInstance.IsValid(base.isServer))
		{
			BaseEntity baseEntity = GameManager.server.CreateEntity(storage.storageUnitPrefab.resourcePath, storage.storageUnitPoint.localPosition, storage.storageUnitPoint.localRotation);
			storageUnitInstance.Set(baseEntity);
			baseEntity.SetParent(this);
			baseEntity.Spawn();
			ItemContainer inventory = GetContainer().inventory;
			inventory.onItemAddedRemoved = (Action<Item, bool>)Delegate.Combine(inventory.onItemAddedRemoved, new Action<Item, bool>(OnItemAddedRemoved));
		}
	}

	public void DestroyStorageEntity()
	{
		if (!IsFullySpawned() || !base.isServer)
		{
			return;
		}
		BaseEntity baseEntity = storageUnitInstance.Get(base.isServer);
		if (baseEntity.IsValid())
		{
			if (baseEntity is BaseCombatEntity baseCombatEntity)
			{
				baseCombatEntity.Die();
			}
			else
			{
				baseEntity.Kill();
			}
		}
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	public void RPC_Open(RPCMessage msg)
	{
		TryOpen(msg.player);
	}

	private bool TryOpen(BasePlayer player)
	{
		if (!player.IsValid() || !CanBeLooted(player))
		{
			return false;
		}
		IItemContainerEntity container = GetContainer();
		if (!container.IsUnityNull())
		{
			container.PlayerOpenLoot(player);
		}
		else
		{
			Debug.LogError((object)(((object)this).GetType().Name + ": No container component found."));
		}
		return true;
	}

	protected override bool CanBeMovedNowOnVehicle()
	{
		IItemContainerEntity container = GetContainer();
		if (!container.IsUnityNull() && !container.inventory.IsEmpty())
		{
			return false;
		}
		return true;
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	public void RPC_TryOpenWithKeycode(RPCMessage msg)
	{
		if (!base.IsOnACar)
		{
			return;
		}
		BasePlayer player = msg.player;
		if (!((Object)(object)player == (Object)null))
		{
			string codeEntered = msg.read.String(256);
			if (base.Car.CarLock.TryOpenWithCode(player, codeEntered))
			{
				TryOpen(player);
			}
			else
			{
				base.Car.ClientRPC(null, "CodeEntryFailed");
			}
		}
	}
}
