using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Facepunch.CardGames;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

public abstract class BaseCardGameEntity : BaseVehicle
{
	[Serializable]
	public class PlayerStorageInfo
	{
		public Transform storagePos;

		public EntityRef storageInstance;

		public CardGamePlayerStorage GetStorage()
		{
			BaseEntity baseEntity = storageInstance.Get(serverside: true);
			if ((Object)(object)baseEntity != (Object)null && baseEntity.IsValid())
			{
				return baseEntity as CardGamePlayerStorage;
			}
			return null;
		}
	}

	public enum CardGameOption
	{
		TexasHoldEm,
		Blackjack
	}

	[Header("Card Game")]
	[SerializeField]
	private GameObjectRef uiPrefab;

	public ItemDefinition scrapItemDef;

	[SerializeField]
	private GameObjectRef potPrefab;

	public PlayerStorageInfo[] playerStoragePoints;

	[SerializeField]
	private GameObjectRef playerStoragePrefab;

	private CardGameController _gameCont;

	public CardGameOption gameOption;

	public EntityRef PotInstance;

	private bool storageLinked;

	public int ScrapItemID => scrapItemDef.itemid;

	public CardGameController GameController
	{
		get
		{
			if (_gameCont == null)
			{
				_gameCont = GetGameController();
			}
			return _gameCont;
		}
	}

	protected abstract float MaxStorageInteractionDist { get; }

	protected override bool CanSwapSeats => false;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("BaseCardGameEntity.OnRpcMessage", 0);
		try
		{
			if (rpc == 2395020190u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_Editor_MakeRandomMove "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_Editor_MakeRandomMove", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(2395020190u, "RPC_Editor_MakeRandomMove", this, player, 3f))
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
							RPC_Editor_MakeRandomMove(msg2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in RPC_Editor_MakeRandomMove");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 1608700874 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_Editor_SpawnTestPlayer "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_Editor_SpawnTestPlayer", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(1608700874u, "RPC_Editor_SpawnTestPlayer", this, player, 3f))
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
							RPC_Editor_SpawnTestPlayer(msg3);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in RPC_Editor_SpawnTestPlayer");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 1499640189 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_LeaveTable "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_LeaveTable", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(1499640189u, "RPC_LeaveTable", this, player, 3f))
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
							RPC_LeaveTable(msg4);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex3)
					{
						Debug.LogException(ex3);
						player.Kick("RPC Error in RPC_LeaveTable");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 331989034 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_OpenLoot "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_OpenLoot", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(331989034u, "RPC_OpenLoot", this, player, 3f))
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
							RPCMessage msg5 = rPCMessage;
							RPC_OpenLoot(msg5);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex4)
					{
						Debug.LogException(ex4);
						player.Kick("RPC Error in RPC_OpenLoot");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 2847205856u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_Play "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_Play", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(2847205856u, "RPC_Play", this, player, 3f))
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
							RPCMessage msg6 = rPCMessage;
							RPC_Play(msg6);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex5)
					{
						Debug.LogException(ex5);
						player.Kick("RPC Error in RPC_Play");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 2495306863u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_PlayerInput "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_PlayerInput", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(2495306863u, "RPC_PlayerInput", this, player, 3f))
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
							RPCMessage msg7 = rPCMessage;
							RPC_PlayerInput(msg7);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex6)
					{
						Debug.LogException(ex6);
						player.Kick("RPC Error in RPC_PlayerInput");
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

	public override void Load(LoadInfo info)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		base.Load(info);
		if (base.isServer)
		{
			PotInstance.uid = info.msg.cardGame.potRef;
		}
	}

	private CardGameController GetGameController()
	{
		return gameOption switch
		{
			CardGameOption.TexasHoldEm => new TexasHoldEmController(this), 
			CardGameOption.Blackjack => new BlackjackController(this), 
			_ => new TexasHoldEmController(this), 
		};
	}

	public override void DestroyShared()
	{
		base.DestroyShared();
		GameController.Dispose();
	}

	public override void Save(SaveInfo info)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		base.Save(info);
		info.msg.cardGame = Pool.Get<CardGame>();
		info.msg.cardGame.potRef = PotInstance.uid;
		if (!info.forDisk && storageLinked)
		{
			GameController.Save(info.msg.cardGame);
		}
	}

	public override void PostServerLoad()
	{
		base.PostServerLoad();
		int num = 0;
		int num2 = 0;
		foreach (BaseEntity child in children)
		{
			if (child is CardGamePlayerStorage cardGamePlayerStorage)
			{
				playerStoragePoints[num].storageInstance.Set(cardGamePlayerStorage);
				if (!cardGamePlayerStorage.inventory.IsEmpty())
				{
					num2++;
				}
				num++;
			}
		}
		storageLinked = true;
		bool flag = true;
		StorageContainer pot = GetPot();
		if ((Object)(object)pot == (Object)null)
		{
			flag = false;
		}
		else
		{
			int num3 = ((num2 > 0) ? num2 : playerStoragePoints.Length);
			int iAmount = Mathf.CeilToInt((float)(pot.inventory.GetAmount(ScrapItemID, onlyUsableAmounts: true) / num3));
			PlayerStorageInfo[] array = playerStoragePoints;
			for (int i = 0; i < array.Length; i++)
			{
				CardGamePlayerStorage cardGamePlayerStorage2 = array[i].storageInstance.Get(base.isServer) as CardGamePlayerStorage;
				if (!cardGamePlayerStorage2.IsValid() || (cardGamePlayerStorage2.inventory.IsEmpty() && num2 != 0))
				{
					continue;
				}
				List<Item> list = Pool.GetList<Item>();
				if (pot.inventory.Take(list, ScrapItemID, iAmount) > 0)
				{
					foreach (Item item in list)
					{
						if (!item.MoveToContainer(cardGamePlayerStorage2.inventory, -1, allowStack: true, ignoreStackLimit: true))
						{
							item.Remove();
						}
					}
				}
				Pool.FreeList<Item>(ref list);
			}
		}
		if (flag)
		{
			PlayerStorageInfo[] array = playerStoragePoints;
			for (int i = 0; i < array.Length; i++)
			{
				if (!array[i].storageInstance.IsValid(base.isServer))
				{
					flag = false;
					break;
				}
			}
		}
		if (!flag)
		{
			Debug.LogWarning((object)(((object)this).GetType().Name + ": Card game storage didn't load in. Destroying the card game (and parent entity if there is one)."));
			BaseEntity baseEntity = GetParentEntity();
			if ((Object)(object)baseEntity != (Object)null)
			{
				((FacepunchBehaviour)baseEntity).Invoke((Action)baseEntity.KillMessage, 0f);
			}
			else
			{
				((FacepunchBehaviour)this).Invoke((Action)base.KillMessage, 0f);
			}
		}
	}

	internal override void DoServerDestroy()
	{
		GameController?.OnTableDestroyed();
		PlayerStorageInfo[] array = playerStoragePoints;
		for (int i = 0; i < array.Length; i++)
		{
			CardGamePlayerStorage storage = array[i].GetStorage();
			if ((Object)(object)storage != (Object)null)
			{
				storage.DropItems();
			}
		}
		StorageContainer pot = GetPot();
		if ((Object)(object)pot != (Object)null)
		{
			pot.DropItems();
		}
		base.DoServerDestroy();
	}

	public override void PrePlayerDismount(BasePlayer player, BaseMountable seat)
	{
		base.PrePlayerDismount(player, seat);
		if (!Application.isLoadingSave)
		{
			CardGamePlayerStorage playerStorage = GetPlayerStorage(player.userID);
			if ((Object)(object)playerStorage != (Object)null)
			{
				playerStorage.inventory.GetSlot(0)?.MoveToContainer(player.inventory.containerMain);
			}
		}
	}

	public override void PlayerDismounted(BasePlayer player, BaseMountable seat)
	{
		base.PlayerDismounted(player, seat);
		GameController.LeaveTable(player.userID);
	}

	public StorageContainer GetPot()
	{
		BaseEntity baseEntity = PotInstance.Get(serverside: true);
		if ((Object)(object)baseEntity != (Object)null && baseEntity.IsValid())
		{
			return baseEntity as StorageContainer;
		}
		return null;
	}

	public BasePlayer IDToPlayer(ulong id)
	{
		foreach (MountPointInfo mountPoint in mountPoints)
		{
			if ((Object)(object)mountPoint.mountable != (Object)null && (Object)(object)mountPoint.mountable.GetMounted() != (Object)null && mountPoint.mountable.GetMounted().userID == id)
			{
				return mountPoint.mountable.GetMounted();
			}
		}
		return null;
	}

	public virtual void PlayerStorageChanged()
	{
		GameController.PlayerStorageChanged();
	}

	public CardGamePlayerStorage GetPlayerStorage(int storageIndex)
	{
		return playerStoragePoints[storageIndex].GetStorage();
	}

	public CardGamePlayerStorage GetPlayerStorage(ulong playerID)
	{
		int mountPointIndex = GetMountPointIndex(playerID);
		if (mountPointIndex < 0)
		{
			return null;
		}
		return playerStoragePoints[mountPointIndex].GetStorage();
	}

	public int GetMountPointIndex(ulong playerID)
	{
		int num = -1;
		for (int i = 0; i < mountPoints.Count; i++)
		{
			BaseMountable mountable = mountPoints[i].mountable;
			if ((Object)(object)mountable != (Object)null)
			{
				BasePlayer mounted = mountable.GetMounted();
				if ((Object)(object)mounted != (Object)null && mounted.userID == playerID)
				{
					num = i;
				}
			}
		}
		if (num < 0)
		{
			Debug.LogError((object)(((object)this).GetType().Name + ": Couldn't find mount point for this player."));
		}
		return num;
	}

	public override void SpawnSubEntities()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		BaseEntity baseEntity = GameManager.server.CreateEntity(potPrefab.resourcePath, Vector3.zero, Quaternion.identity);
		StorageContainer storageContainer = baseEntity as StorageContainer;
		if ((Object)(object)storageContainer != (Object)null)
		{
			storageContainer.SetParent(this);
			storageContainer.Spawn();
			PotInstance.Set(baseEntity);
		}
		else
		{
			Debug.LogError((object)(((object)this).GetType().Name + ": Spawned prefab is not a StorageContainer as expected."));
		}
		PlayerStorageInfo[] array = playerStoragePoints;
		foreach (PlayerStorageInfo playerStorageInfo in array)
		{
			baseEntity = GameManager.server.CreateEntity(playerStoragePrefab.resourcePath, playerStorageInfo.storagePos.localPosition, playerStorageInfo.storagePos.localRotation);
			CardGamePlayerStorage cardGamePlayerStorage = baseEntity as CardGamePlayerStorage;
			if ((Object)(object)cardGamePlayerStorage != (Object)null)
			{
				cardGamePlayerStorage.SetCardTable(this);
				cardGamePlayerStorage.SetParent(this);
				cardGamePlayerStorage.Spawn();
				playerStorageInfo.storageInstance.Set(baseEntity);
				storageLinked = true;
			}
			else
			{
				Debug.LogError((object)(((object)this).GetType().Name + ": Spawned prefab is not a CardTablePlayerStorage as expected."));
			}
		}
		base.SpawnSubEntities();
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	private void RPC_PlayerInput(RPCMessage msg)
	{
		GameController.ReceivedInputFromPlayer(msg.player, msg.read.Int32(), countAsAction: true, msg.read.Int32());
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	private void RPC_LeaveTable(RPCMessage msg)
	{
		GameController.LeaveTable(msg.player.userID);
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	private void RPC_OpenLoot(RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if ((Object)(object)player != (Object)null && PlayerIsMounted(player))
		{
			GetPlayerStorage(player.userID).PlayerOpenLoot(player);
		}
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	public void RPC_Editor_SpawnTestPlayer(RPCMessage msg)
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		if (!Application.isEditor)
		{
			return;
		}
		int num = GameController.MaxPlayersAtTable();
		if (GameController.NumPlayersAllowedToPlay() >= num || NumMounted() >= num)
		{
			return;
		}
		Debug.Log((object)"Adding test NPC for card game");
		BaseEntity baseEntity = GameManager.server.CreateEntity("assets/prefabs/player/player.prefab", ((Component)this).transform.position, Quaternion.identity);
		baseEntity.Spawn();
		BasePlayer basePlayer = (BasePlayer)baseEntity;
		AttemptMount(basePlayer, doMountChecks: false);
		GameController.JoinTable(basePlayer);
		if (GameController.TryGetCardPlayerData(basePlayer, out var cardPlayer))
		{
			int scrapAmount = cardPlayer.GetScrapAmount();
			if (scrapAmount < 400)
			{
				StorageContainer storage = cardPlayer.GetStorage();
				if ((Object)(object)storage != (Object)null)
				{
					storage.inventory.AddItem(scrapItemDef, 400 - scrapAmount, 0uL);
				}
				else
				{
					Debug.LogError((object)"Couldn't get storage for NPC.");
				}
			}
		}
		else
		{
			Debug.Log((object)"Couldn't find player data for NPC. No scrap given.");
		}
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	public void RPC_Editor_MakeRandomMove(RPCMessage msg)
	{
		if (Application.isEditor)
		{
			GameController.EditorMakeRandomMove();
		}
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	private void RPC_Play(RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if ((Object)(object)player != (Object)null && PlayerIsMounted(player))
		{
			GameController.JoinTable(player);
		}
	}
}
