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

	[Header("Text")]
	public Phrase textLoadAmmos;

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

	private WeaponRackSlot[] gridCellSlotReferences;

	private static HashSet<int> usedSlots = new HashSet<int>();

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
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - LoadWeaponAmmo "));
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
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - ReqMountWeapon "));
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
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - ReqSwapWeapon "));
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
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - ReqTakeAll "));
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
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - ReqTakeWeapon "));
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
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - ReqUnloadWeapon "));
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
		ClearGridCellContentsRefs();
	}

	private void ClearGridCellContentsRefs()
	{
		if (gridCellSlotReferences == null)
		{
			gridCellSlotReferences = new WeaponRackSlot[GridCellCountX * GridCellCountY];
			return;
		}
		for (int i = 0; i < gridCellSlotReferences.Length; i++)
		{
			gridCellSlotReferences[i] = null;
		}
	}

	private void SetupSlot(WeaponRackSlot slot)
	{
		if (slot != null && !((Object)(object)slot.ItemDef == (Object)null))
		{
			SetGridCellContents(slot, clear: false);
		}
	}

	private void ClearSlot(WeaponRackSlot slot)
	{
		if (slot != null && slot.Used)
		{
			SetGridCellContents(slot, clear: true);
		}
	}

	private void SetGridCellContents(WeaponRackSlot slot, bool clear)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		if (slot == null)
		{
			return;
		}
		WorldModelRackMountConfig forItemDef = WorldModelRackMountConfig.GetForItemDef(slot.ItemDef);
		if ((Object)(object)forItemDef == (Object)null)
		{
			return;
		}
		Vector2Int xYForIndex = GetXYForIndex(slot.GridSlotIndex);
		Vector2Int weaponSize = GetWeaponSize(forItemDef, slot.Rotation);
		Vector2Int weaponStart = GetWeaponStart(xYForIndex, weaponSize, clamp: false);
		if (((Vector2Int)(ref weaponStart)).x < 0 || ((Vector2Int)(ref weaponStart)).y < 0 || ((Vector2Int)(ref weaponStart)).x + ((Vector2Int)(ref weaponSize)).x > GridCellCountX || ((Vector2Int)(ref weaponStart)).y + ((Vector2Int)(ref weaponSize)).y > GridCellCountY)
		{
			return;
		}
		for (int i = ((Vector2Int)(ref weaponStart)).y; i < ((Vector2Int)(ref weaponStart)).y + ((Vector2Int)(ref weaponSize)).y; i++)
		{
			for (int j = ((Vector2Int)(ref weaponStart)).x; j < ((Vector2Int)(ref weaponStart)).x + ((Vector2Int)(ref weaponSize)).x; j++)
			{
				gridCellSlotReferences[GetGridCellIndex(j, i)] = (clear ? null : slot);
			}
		}
		slot.SetUsed(!clear);
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		usedSlots.Clear();
		ClearGridCellContentsRefs();
		if (info.msg.weaponRack == null)
		{
			return;
		}
		foreach (WeaponRackItem item in info.msg.weaponRack.items)
		{
			usedSlots.Add(item.inventorySlot);
			gridSlots[item.inventorySlot].InitFromProto(item);
		}
		for (int i = 0; i < Capacity; i++)
		{
			if (usedSlots.Contains(i))
			{
				SetupSlot(gridSlots[i]);
			}
			else
			{
				ClearSlot(gridSlots[i]);
			}
		}
	}

	public WeaponRackSlot GetWeaponAtIndex(int gridIndex)
	{
		if (gridIndex < 0)
		{
			return null;
		}
		if (gridIndex >= gridCellSlotReferences.Length)
		{
			return null;
		}
		return gridCellSlotReferences[gridIndex];
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

	public int GetBestPlacementCellIndex(Vector2Int targetXY, WorldModelRackMountConfig config, int rotation, WeaponRackSlot ignoreSlot)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		if (Type == RackType.Stand)
		{
			((Vector2Int)(ref targetXY)).y = 0;
		}
		int gridCellIndex = GetGridCellIndex(((Vector2Int)(ref targetXY)).x, ((Vector2Int)(ref targetXY)).y);
		if (GridCellsFree(config, gridCellIndex, rotation, ignoreSlot))
		{
			return gridCellIndex;
		}
		float num = float.MaxValue;
		int result = -1;
		Vector2Int weaponSize = GetWeaponSize(config, rotation);
		Vector2Int weaponStart = GetWeaponStart(targetXY, weaponSize, clamp: true);
		Vector2Int val = default(Vector2Int);
		for (int i = ((Vector2Int)(ref weaponStart)).y; i < ((Vector2Int)(ref weaponStart)).y + ((Vector2Int)(ref weaponSize)).y + 1; i++)
		{
			if (Type == RackType.Stand && i != 0)
			{
				continue;
			}
			for (int j = ((Vector2Int)(ref weaponStart)).x; j < ((Vector2Int)(ref weaponStart)).x + ((Vector2Int)(ref weaponSize)).x + 1; j++)
			{
				gridCellIndex = GetGridCellIndex(j, i);
				if (GridCellsFree(config, gridCellIndex, rotation, ignoreSlot))
				{
					((Vector2Int)(ref val)).x = j;
					((Vector2Int)(ref val)).y = i;
					float num2 = Vector2Int.Distance(targetXY, val);
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

	private bool GridCellsFree(WorldModelRackMountConfig config, int gridIndex, int rotation, WeaponRackSlot ignoreGridSlot)
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
				if (gridCellIndex == -1 || !GridCellFree(gridCellIndex, ignoreGridSlot))
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

	private bool GridCellFree(int index, WeaponRackSlot ignoreSlot)
	{
		if (gridCellSlotReferences[index] != null)
		{
			if (ignoreSlot != null)
			{
				return gridCellSlotReferences[index] == ignoreSlot;
			}
			return false;
		}
		return true;
	}

	private static bool ItemIsRackMountable(Item item)
	{
		return (Object)(object)WorldModelRackMountConfig.GetForItemDef(item.info) != (Object)null;
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
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		Item item = player.GetHeldEntity()?.GetItem();
		if (item == null)
		{
			return;
		}
		WorldModelRackMountConfig forItemDef = WorldModelRackMountConfig.GetForItemDef(item.info);
		if ((Object)(object)forItemDef == (Object)null)
		{
			return;
		}
		WeaponRackSlot weaponAtIndex = GetWeaponAtIndex(gridCellIndex);
		if (weaponAtIndex != null)
		{
			int bestPlacementCellIndex = GetBestPlacementCellIndex(GetXYForIndex(gridCellIndex), forItemDef, rotation, weaponAtIndex);
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
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)player == (Object)null)
		{
			return;
		}
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
		if ((Object)(object)player == (Object)null)
		{
			return;
		}
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
		if ((Object)(object)player == (Object)null)
		{
			return;
		}
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
		if ((Object)(object)player == (Object)null)
		{
			return;
		}
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
		slot?.SetAmmoDetails(item);
	}

	private bool MountWeapon(Item item, BasePlayer player, int gridCellIndex, int rotation, bool sendUpdate = true)
	{
		if (item == null)
		{
			return false;
		}
		if ((Object)(object)player == (Object)null)
		{
			return false;
		}
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
		if (!GridCellsFree(forItemDef, gridCellIndex, rotation, null))
		{
			return false;
		}
		if (item.MoveToContainer(base.inventory, -1, allowStack: false) && item.position >= 0 && item.position < gridSlots.Length)
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
			component.TryReloadMagazine(player.inventory);
			SetSlotAmmoDetails(weaponAtIndex, slot);
			SendNetworkUpdateImmediate();
			ClientRPCPlayer(null, player, "PlayAmmoSound", itemDefinition.itemid, 0);
		}
	}
}
