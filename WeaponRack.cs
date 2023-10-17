using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

public class WeaponRack : StorageContainer
{
	[Serializable]
	public enum RackType
	{
		Board,
		Stand
	}

	public RackType Type;

	public float GridCellSize = 0.15f;

	public int Capacity = 30;

	public bool UseColliders;

	public int GridCellCountX = 10;

	public int GridCellCountY = 10;

	public BoxCollider Collision;

	public Transform Anchor;

	public Transform SmallPegPrefab;

	public Transform LargePegPrefab;

	[Header("Lights")]
	public GameObjectRef LightPrefab;

	public Transform[] LightPoints;

	private WeaponRackSlot[] gridSlots;

	private int[] gridCellContents;

	private static HashSet<int> usedSlots = new HashSet<int>();

	private static HashSet<int> updatedSlots = new HashSet<int>();

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("WeaponRack.OnRpcMessage", 0);
		try
		{
			if (rpc == 1682065633 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - LoadWeaponAmmo "));
				}
				TimeWarning val2 = TimeWarning.New("LoadWeaponAmmo", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Call", 0);
					try
					{
						RPCMessage rPCMessage = default(RPCMessage);
						rPCMessage.connection = msg.connection;
						rPCMessage.player = player;
						rPCMessage.read = msg.read;
						RPCMessage msg2 = rPCMessage;
						LoadWeaponAmmo(msg2);
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
				}
				catch (Exception ex)
				{
					Debug.LogException(ex);
					player.Kick("RPC Error in LoadWeaponAmmo");
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 2640584497u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - ReqMountWeapon "));
				}
				TimeWarning val2 = TimeWarning.New("ReqMountWeapon", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(2640584497u, "ReqMountWeapon", this, player, 3f))
						{
							return true;
						}
						if (!RPC_Server.MaxDistance.Test(2640584497u, "ReqMountWeapon", this, player, 2f))
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
							ReqMountWeapon(msg3);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in ReqMountWeapon");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 2753286621u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - ReqSwapWeapon "));
				}
				TimeWarning val2 = TimeWarning.New("ReqSwapWeapon", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(2753286621u, "ReqSwapWeapon", this, player, 3f))
						{
							return true;
						}
						if (!RPC_Server.MaxDistance.Test(2753286621u, "ReqSwapWeapon", this, player, 2f))
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
							ReqSwapWeapon(msg4);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex3)
					{
						Debug.LogException(ex3);
						player.Kick("RPC Error in ReqSwapWeapon");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 3761066327u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - ReqTakeAll "));
				}
				TimeWarning val2 = TimeWarning.New("ReqTakeAll", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(3761066327u, "ReqTakeAll", this, player, 3f))
						{
							return true;
						}
						if (!RPC_Server.MaxDistance.Test(3761066327u, "ReqTakeAll", this, player, 2f))
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
							ReqTakeAll(msg5);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex4)
					{
						Debug.LogException(ex4);
						player.Kick("RPC Error in ReqTakeAll");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 1987971716 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - ReqTakeWeapon "));
				}
				TimeWarning val2 = TimeWarning.New("ReqTakeWeapon", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(1987971716u, "ReqTakeWeapon", this, player, 3f))
						{
							return true;
						}
						if (!RPC_Server.MaxDistance.Test(1987971716u, "ReqTakeWeapon", this, player, 2f))
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
							ReqTakeWeapon(msg6);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex5)
					{
						Debug.LogException(ex5);
						player.Kick("RPC Error in ReqTakeWeapon");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 3314206579u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - ReqUnloadWeapon "));
				}
				TimeWarning val2 = TimeWarning.New("ReqUnloadWeapon", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(3314206579u, "ReqUnloadWeapon", this, player, 3f))
						{
							return true;
						}
						if (!RPC_Server.MaxDistance.Test(3314206579u, "ReqUnloadWeapon", this, player, 2f))
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
							ReqUnloadWeapon(msg7);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex6)
					{
						Debug.LogException(ex6);
						player.Kick("RPC Error in ReqUnloadWeapon");
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

	public override void ServerInit()
	{
		base.ServerInit();
		base.inventory.onItemAddedRemoved = OnItemAddedOrRemoved;
		ItemContainer itemContainer = base.inventory;
		itemContainer.canAcceptItem = (Func<Item, int, bool>)Delegate.Combine(itemContainer.canAcceptItem, new Func<Item, int, bool>(InventoryItemFilter));
		SpawnLightSubEntities();
	}

	private void SpawnLightSubEntities()
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		if (Application.isLoadingSave || LightPrefab == null || LightPoints == null)
		{
			return;
		}
		Transform[] lightPoints = LightPoints;
		foreach (Transform val in lightPoints)
		{
			SimpleLight simpleLight = GameManager.server.CreateEntity(LightPrefab.resourcePath, val.position, val.rotation) as SimpleLight;
			if (Object.op_Implicit((Object)(object)simpleLight))
			{
				simpleLight.enableSaving = true;
				simpleLight.SetParent(this, worldPositionStays: true);
				simpleLight.Spawn();
			}
		}
	}

	private bool InventoryItemFilter(Item item, int targetSlot)
	{
		if (item == null)
		{
			return false;
		}
		return ItemIsRackMountable(item);
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		info.msg.weaponRack = Pool.Get<WeaponRack>();
		info.msg.weaponRack.items = Pool.GetList<WeaponRackItem>();
		WeaponRackSlot[] array = gridSlots;
		foreach (WeaponRackSlot weaponRackSlot in array)
		{
			if (weaponRackSlot.Used)
			{
				Item slot = base.inventory.GetSlot(weaponRackSlot.InventoryIndex);
				WeaponRackItem proto = Pool.Get<WeaponRackItem>();
				info.msg.weaponRack.items.Add(weaponRackSlot.SaveToProto(slot, proto));
			}
		}
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	[RPC_Server.MaxDistance(2f)]
	private void ReqSwapWeapon(RPCMessage msg)
	{
		int num = msg.read.Int32();
		if (num != -1)
		{
			int rotation = msg.read.Int32();
			Item item = msg.player.GetHeldEntity()?.GetItem();
			if (item != null)
			{
				SwapPlayerWeapon(msg.player, num, item.position, rotation);
			}
		}
	}

	private void SwapPlayerWeapon(BasePlayer player, int gridCellIndex, int takeFromBeltIndex, int rotation)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		Item item = player.GetHeldEntity().GetItem();
		WorldModelRackMountConfig forItemDef = WorldModelRackMountConfig.GetForItemDef(item.info);
		if (!((Object)(object)forItemDef == (Object)null) && GetWeaponAtIndex(gridCellIndex) != null)
		{
			int bestPlacementCellIndex = GetBestPlacementCellIndex(GetXYForIndex(gridCellIndex), forItemDef, rotation, gridCellIndex);
			if (bestPlacementCellIndex != -1)
			{
				item.RemoveFromContainer();
				GivePlayerWeapon(player, gridCellIndex, takeFromBeltIndex, tryHold: false);
				MountWeapon(item, player, bestPlacementCellIndex, rotation, sendUpdate: false);
				ItemManager.DoRemoves();
				SendNetworkUpdateImmediate();
			}
		}
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	[RPC_Server.MaxDistance(2f)]
	private void ReqTakeWeapon(RPCMessage msg)
	{
		int num = msg.read.Int32();
		if (num != -1)
		{
			GivePlayerWeapon(msg.player, num);
		}
	}

	private void GivePlayerWeapon(BasePlayer player, int mountSlotIndex, int playerBeltIndex = -1, bool tryHold = true, bool sendUpdate = true)
	{
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		WeaponRackSlot weaponAtIndex = GetWeaponAtIndex(mountSlotIndex);
		if (weaponAtIndex == null)
		{
			return;
		}
		Item slot = base.inventory.GetSlot(weaponAtIndex.InventoryIndex);
		if (slot == null)
		{
			return;
		}
		ClearSlot(weaponAtIndex);
		if (slot.MoveToContainer(player.inventory.containerBelt, playerBeltIndex))
		{
			if ((tryHold && (Object)(object)player.GetHeldEntity() == (Object)null) || playerBeltIndex != -1)
			{
				ClientRPCPlayer<int, ItemId>(null, player, "SetActiveBeltSlot", slot.position, slot.uid);
			}
			ClientRPCPlayer(null, player, "PlayGrabSound", slot.info.itemid);
		}
		else if (!slot.MoveToContainer(player.inventory.containerMain))
		{
			slot.Drop(base.inventory.dropPosition, base.inventory.dropVelocity);
		}
		if (sendUpdate)
		{
			ItemManager.DoRemoves();
			SendNetworkUpdateImmediate();
		}
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	[RPC_Server.MaxDistance(2f)]
	private void ReqTakeAll(RPCMessage msg)
	{
		int num = msg.read.Int32();
		if (num != -1)
		{
			GivePlayerAllWeapons(msg.player, num);
		}
	}

	private void GivePlayerAllWeapons(BasePlayer player, int mountSlotIndex)
	{
		WeaponRackSlot weaponAtIndex = GetWeaponAtIndex(mountSlotIndex);
		if (weaponAtIndex != null)
		{
			GivePlayerWeapon(player, weaponAtIndex.GridSlotIndex);
		}
		for (int num = gridSlots.Length - 1; num >= 0; num--)
		{
			WeaponRackSlot weaponRackSlot = gridSlots[num];
			if (weaponRackSlot.Used)
			{
				GivePlayerWeapon(player, weaponRackSlot.GridSlotIndex, -1, tryHold: false);
			}
		}
		ItemManager.DoRemoves();
		SendNetworkUpdateImmediate();
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	[RPC_Server.MaxDistance(2f)]
	private void ReqUnloadWeapon(RPCMessage msg)
	{
		int num = msg.read.Int32();
		if (num != -1)
		{
			UnloadWeapon(msg.player, num);
		}
	}

	private void UnloadWeapon(BasePlayer player, int mountSlotIndex)
	{
		WeaponRackSlot weaponAtIndex = GetWeaponAtIndex(mountSlotIndex);
		if (weaponAtIndex == null)
		{
			return;
		}
		Item slot = base.inventory.GetSlot(weaponAtIndex.InventoryIndex);
		if (slot == null)
		{
			return;
		}
		BaseEntity heldEntity = slot.GetHeldEntity();
		if (!((Object)(object)heldEntity == (Object)null))
		{
			BaseProjectile component = ((Component)heldEntity).GetComponent<BaseProjectile>();
			if (!((Object)(object)component == (Object)null))
			{
				ItemDefinition ammoType = component.primaryMagazine.ammoType;
				component.UnloadAmmo(slot, player);
				SetSlotAmmoDetails(weaponAtIndex, slot);
				SendNetworkUpdateImmediate();
				ClientRPCPlayer(null, player, "PlayAmmoSound", ammoType.itemid, 1);
			}
		}
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	[RPC_Server.MaxDistance(2f)]
	private void ReqMountWeapon(RPCMessage msg)
	{
		if (base.inventory.itemList.Count != base.inventory.capacity)
		{
			int num = msg.read.Int32();
			if (num != -1)
			{
				int rotation = msg.read.Int32();
				MountWeapon(msg.player, num, rotation);
			}
		}
	}

	private void MountWeapon(BasePlayer player, int gridCellIndex, int rotation)
	{
		HeldEntity heldEntity = player.GetHeldEntity();
		if (!((Object)(object)heldEntity == (Object)null))
		{
			Item item = heldEntity.GetItem();
			if (item != null)
			{
				MountWeapon(item, player, gridCellIndex, rotation);
			}
		}
	}

	private void SetSlotItem(WeaponRackSlot slot, Item item, int gridCellIndex, int rotation)
	{
		slot.SetItem(item, base.inventory.GetSlot(item.position)?.info, gridCellIndex, rotation);
	}

	private void SetSlotAmmoDetails(WeaponRackSlot slot, Item item)
	{
		slot.SetAmmoDetails(item);
	}

	private bool MountWeapon(Item item, BasePlayer player, int gridCellIndex, int rotation, bool sendUpdate = true)
	{
		int itemid = item.info.itemid;
		WorldModelRackMountConfig forItemDef = WorldModelRackMountConfig.GetForItemDef(item.info);
		if ((Object)(object)forItemDef == (Object)null)
		{
			Debug.LogWarning((object)"no rackmount config");
			return false;
		}
		if (!CanAcceptWeaponType(forItemDef))
		{
			return false;
		}
		if (!GridCellsFree(forItemDef, gridCellIndex, rotation, -1))
		{
			return false;
		}
		if (item.MoveToContainer(base.inventory))
		{
			WeaponRackSlot slot = gridSlots[item.position];
			SetSlotItem(slot, item, gridCellIndex, rotation);
			SetupSlot(slot);
			if ((Object)(object)player != (Object)null)
			{
				ClientRPCPlayer(null, player, "PlayMountSound", itemid);
			}
		}
		if (sendUpdate)
		{
			ItemManager.DoRemoves();
			SendNetworkUpdateImmediate();
		}
		return true;
	}

	private void PlayMountSound(int itemID)
	{
		ClientRPC(null, "PlayMountSound", itemID);
	}

	[RPC_Server]
	private void LoadWeaponAmmo(RPCMessage msg)
	{
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		BasePlayer player = msg.player;
		if (!Object.op_Implicit((Object)(object)player))
		{
			return;
		}
		int gridIndex = msg.read.Int32();
		int num = msg.read.Int32();
		WeaponRackSlot weaponAtIndex = GetWeaponAtIndex(gridIndex);
		if (weaponAtIndex == null)
		{
			return;
		}
		Item slot = base.inventory.GetSlot(weaponAtIndex.InventoryIndex);
		if (slot == null)
		{
			return;
		}
		BaseEntity heldEntity = slot.GetHeldEntity();
		if ((Object)(object)heldEntity == (Object)null)
		{
			return;
		}
		BaseProjectile component = ((Component)heldEntity).GetComponent<BaseProjectile>();
		if ((Object)(object)component == (Object)null)
		{
			return;
		}
		ItemDefinition itemDefinition = ItemManager.FindItemDefinition(num);
		if ((Object)(object)itemDefinition == (Object)null)
		{
			return;
		}
		ItemModProjectile component2 = ((Component)itemDefinition).GetComponent<ItemModProjectile>();
		if (!((Object)(object)component2 == (Object)null) && component2.IsAmmo(component.primaryMagazine.definition.ammoTypes))
		{
			if (num != component.primaryMagazine.ammoType.itemid && component.primaryMagazine.contents > 0)
			{
				player.GiveItem(ItemManager.CreateByItemID(component.primaryMagazine.ammoType.itemid, component.primaryMagazine.contents, 0uL));
				component.primaryMagazine.contents = 0;
			}
			component.primaryMagazine.ammoType = itemDefinition;
			component.primaryMagazine.Reload(player);
			SetSlotAmmoDetails(weaponAtIndex, slot);
			SendNetworkUpdateImmediate();
			ItemManager.DoRemoves();
			player.inventory.ServerUpdate(0f);
			ClientRPCPlayer(null, player, "PlayAmmoSound", itemDefinition.itemid, 0);
		}
	}

	public override void InitShared()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		base.InitShared();
		GridCellSize = Collision.size.x / (float)GridCellCountX;
		gridSlots = new WeaponRackSlot[Capacity];
		for (int i = 0; i < gridSlots.Length; i++)
		{
			gridSlots[i] = new WeaponRackSlot();
		}
		ClearGridCellContents();
	}

	private void ClearGridCellContents()
	{
		gridCellContents = new int[GridCellCountX * GridCellCountY];
		for (int i = 0; i < gridCellContents.Length; i++)
		{
			gridCellContents[i] = -1;
		}
	}

	private void SetupSlot(WeaponRackSlot slot)
	{
		if (slot != null && !((Object)(object)slot.ItemDef == (Object)null))
		{
			SetGridCellContents(slot, slot.GridSlotIndex);
		}
	}

	private void ClearSlot(WeaponRackSlot slot)
	{
		if (slot != null && slot.Used)
		{
			SetGridCellContents(slot, -1);
		}
	}

	private void SetGridCellContents(WeaponRackSlot slot, int value)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		if (slot == null)
		{
			return;
		}
		slot.Used = value != -1;
		WorldModelRackMountConfig forItemDef = WorldModelRackMountConfig.GetForItemDef(slot.ItemDef);
		if ((Object)(object)forItemDef == (Object)null)
		{
			return;
		}
		Vector2Int xYForIndex = GetXYForIndex(slot.GridSlotIndex);
		Vector2Int weaponSize = GetWeaponSize(forItemDef, slot.Rotation);
		Vector2Int weaponStart = GetWeaponStart(xYForIndex, weaponSize, clamp: false);
		if (((Vector2Int)(ref weaponStart)).x < 0 || ((Vector2Int)(ref weaponStart)).y < 0)
		{
			return;
		}
		for (int i = ((Vector2Int)(ref weaponStart)).y; i < ((Vector2Int)(ref weaponStart)).y + ((Vector2Int)(ref weaponSize)).y; i++)
		{
			for (int j = ((Vector2Int)(ref weaponStart)).x; j < ((Vector2Int)(ref weaponStart)).x + ((Vector2Int)(ref weaponSize)).x; j++)
			{
				gridCellContents[GetGridCellIndex(j, i)] = value;
			}
		}
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		usedSlots.Clear();
		updatedSlots.Clear();
		ClearGridCellContents();
		if (info.msg.weaponRack == null)
		{
			return;
		}
		foreach (WeaponRackItem item in info.msg.weaponRack.items)
		{
			usedSlots.Add(item.inventorySlot);
			WeaponRackSlot weaponRackSlot = gridSlots[item.inventorySlot];
			if (!weaponRackSlot.Used || item.itemID != weaponRackSlot.ClientItemID || item.skinid != weaponRackSlot.ClientItemSkinID)
			{
				updatedSlots.Add(item.inventorySlot);
			}
			weaponRackSlot.InitFromProto(item);
		}
		for (int i = 0; i < Capacity; i++)
		{
			if (usedSlots.Contains(i))
			{
				SetupSlot(gridSlots[i]);
				continue;
			}
			if (gridSlots[i].Used)
			{
				updatedSlots.Add(i);
			}
			ClearSlot(gridSlots[i]);
		}
	}

	public WeaponRackSlot GetWeaponAtIndex(int gridIndex)
	{
		if (gridIndex < 0)
		{
			return null;
		}
		if (gridIndex >= gridCellContents.Length)
		{
			return null;
		}
		int num = gridCellContents[gridIndex];
		if (num == -1)
		{
			return null;
		}
		WeaponRackSlot[] array = gridSlots;
		foreach (WeaponRackSlot weaponRackSlot in array)
		{
			if (weaponRackSlot.Used && weaponRackSlot.GridSlotIndex == num)
			{
				return weaponRackSlot;
			}
		}
		return null;
	}

	public Vector2Int GetXYForIndex(int index)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		return new Vector2Int(index % GridCellCountX, index / GridCellCountX);
	}

	private Vector2Int GetWeaponSize(WorldModelRackMountConfig config, int rotation)
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		int num = ((Type == RackType.Board) ? config.XSize : config.ZSize);
		int num2 = ((Type == RackType.Board) ? config.YSize : config.XSize);
		if (rotation != 0 && Type == RackType.Board)
		{
			return new Vector2Int(num2, num);
		}
		return new Vector2Int(num, num2);
	}

	private Vector2Int GetWeaponStart(Vector2Int targetXY, Vector2Int size, bool clamp)
	{
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		if (Type == RackType.Board)
		{
			((Vector2Int)(ref targetXY)).x = ((Vector2Int)(ref targetXY)).x - ((Vector2Int)(ref size)).x / 2;
			((Vector2Int)(ref targetXY)).y = ((Vector2Int)(ref targetXY)).y - ((Vector2Int)(ref size)).y / 2;
		}
		if (clamp)
		{
			((Vector2Int)(ref targetXY)).x = Mathf.Max(((Vector2Int)(ref targetXY)).x, 0);
			((Vector2Int)(ref targetXY)).y = Mathf.Max(((Vector2Int)(ref targetXY)).y, 0);
		}
		return targetXY;
	}

	public bool CanAcceptWeaponType(WorldModelRackMountConfig weaponConfig)
	{
		if ((Object)(object)weaponConfig == (Object)null)
		{
			return false;
		}
		if (weaponConfig.ExcludedRackTypes.Contains(Type))
		{
			return false;
		}
		return true;
	}

	public int GetBestPlacementCellIndex(Vector2Int targetXY, WorldModelRackMountConfig config, int rotation, int ignoreGridSlotWeapon)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		if (Type == RackType.Stand)
		{
			((Vector2Int)(ref targetXY)).y = 0;
		}
		int gridCellIndex = GetGridCellIndex(((Vector2Int)(ref targetXY)).x, ((Vector2Int)(ref targetXY)).y);
		if (GridCellsFree(config, gridCellIndex, rotation, ignoreGridSlotWeapon))
		{
			return gridCellIndex;
		}
		float num = float.MaxValue;
		int result = -1;
		Vector2Int weaponSize = GetWeaponSize(config, rotation);
		Vector2Int weaponStart = GetWeaponStart(targetXY, weaponSize, clamp: true);
		for (int i = ((Vector2Int)(ref weaponStart)).y; i < ((Vector2Int)(ref weaponStart)).y + ((Vector2Int)(ref weaponSize)).y + 1; i++)
		{
			if (Type == RackType.Stand && i != 0)
			{
				continue;
			}
			for (int j = ((Vector2Int)(ref weaponStart)).x; j < ((Vector2Int)(ref weaponStart)).x + ((Vector2Int)(ref weaponSize)).x + 1; j++)
			{
				gridCellIndex = GetGridCellIndex(j, i);
				if (GridCellsFree(config, gridCellIndex, rotation, ignoreGridSlotWeapon))
				{
					float num2 = Vector2Int.Distance(targetXY, new Vector2Int(j, i));
					if (!(num2 >= num))
					{
						result = gridCellIndex;
						num = num2;
					}
				}
			}
		}
		return result;
	}

	public int GetGridIndexAtPosition(Vector3 pos)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		float num = Collision.size.x - (pos.x + Collision.size.x / 2f);
		float num2 = pos.y + Collision.size.y / 2f;
		int num3 = (int)(num / GridCellSize);
		return (int)(num2 / GridCellSize) * GridCellCountX + num3;
	}

	private bool GridCellsFree(WorldModelRackMountConfig config, int gridIndex, int rotation, int ignoreGridSlotWeapon)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		if (gridIndex == -1)
		{
			return false;
		}
		Vector2Int xYForIndex = GetXYForIndex(gridIndex);
		Vector2Int weaponSize = GetWeaponSize(config, rotation);
		Vector2Int weaponStart = GetWeaponStart(xYForIndex, weaponSize, clamp: false);
		if (((Vector2Int)(ref weaponStart)).x < 0 || ((Vector2Int)(ref weaponStart)).y < 0)
		{
			return false;
		}
		for (int i = ((Vector2Int)(ref weaponStart)).y; i < ((Vector2Int)(ref weaponStart)).y + ((Vector2Int)(ref weaponSize)).y; i++)
		{
			for (int j = ((Vector2Int)(ref weaponStart)).x; j < ((Vector2Int)(ref weaponStart)).x + ((Vector2Int)(ref weaponSize)).x; j++)
			{
				int gridCellIndex = GetGridCellIndex(j, i);
				if (gridCellIndex == -1 || !GridCellFree(gridCellIndex, ignoreGridSlotWeapon))
				{
					return false;
				}
			}
		}
		return true;
	}

	private int GetGridCellIndex(int x, int y)
	{
		if (x < 0 || x >= GridCellCountX || y < 0 || y >= GridCellCountY)
		{
			return -1;
		}
		return y * GridCellCountX + x;
	}

	private bool GridCellFree(int index, int ignoreGridSlotWeapon)
	{
		if (gridCellContents[index] != -1)
		{
			if (ignoreGridSlotWeapon != -1)
			{
				return gridCellContents[index] == ignoreGridSlotWeapon;
			}
			return false;
		}
		return true;
	}

	private static bool ItemIsRackMountable(Item item)
	{
		return (Object)(object)WorldModelRackMountConfig.GetForItemDef(item.info) != (Object)null;
	}
}
