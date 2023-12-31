using System;
using System.Collections.Generic;
using System.Linq;
using ConVar;
using Facepunch;
using Facepunch.Extend;
using Network;
using ProtoBuf;
using Rust;
using Rust.UI;
using UnityEngine;
using UnityEngine.Assertions;

public class MarketTerminal : StorageContainer
{
	public const Flags Flag_HasItems = Flags.Reserved1;

	public const Flags Flag_InventoryFull = Flags.Reserved2;

	[Header("Market Terminal")]
	public GameObjectRef menuPrefab;

	public ulong lockToCustomerDuration = 300uL;

	public ulong orderTimeout = 180uL;

	public ItemDefinition deliveryFeeCurrency;

	public int deliveryFeeAmount;

	public DeliveryDroneConfig config;

	public RustText userLabel;

	private ulong _customerSteamId;

	private string _customerName;

	private TimeUntil _timeUntilCustomerExpiry;

	private EntityRef<Marketplace> _marketplace;

	public List<PendingOrder> pendingOrders;

	private Action<BasePlayer, Item> _onCurrencyRemovedCached;

	private Action<BasePlayer, Item> _onItemPurchasedCached;

	private Action _checkForExpiredOrdersCached;

	private bool _transactionActive;

	private static readonly List<NetworkableId> _deliveryEligible = new List<NetworkableId>(128);

	private static RealTimeSince _deliveryEligibleLastCalculated;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("MarketTerminal.OnRpcMessage", 0);
		try
		{
			if (rpc == 3793918752u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - Server_Purchase "));
				}
				TimeWarning val2 = TimeWarning.New("Server_Purchase", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(3793918752u, "Server_Purchase", this, player, 10uL))
						{
							return true;
						}
						if (!RPC_Server.IsVisible.Test(3793918752u, "Server_Purchase", this, player, 3f))
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
							Server_Purchase(msg2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in Server_Purchase");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 1382511247 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - Server_TryOpenMarket "));
				}
				TimeWarning val2 = TimeWarning.New("Server_TryOpenMarket", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(1382511247u, "Server_TryOpenMarket", this, player, 3uL))
						{
							return true;
						}
						if (!RPC_Server.IsVisible.Test(1382511247u, "Server_TryOpenMarket", this, player, 3f))
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
							Server_TryOpenMarket(msg3);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in Server_TryOpenMarket");
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

	public bool CanPlayerAffordOrderAndDeliveryFee(BasePlayer player, SellOrder sellOrder, int numberOfTransactions)
	{
		int num = player.inventory.FindItemsByItemID(deliveryFeeCurrency.itemid).Sum((Item i) => i.amount);
		int num2 = deliveryFeeAmount;
		if (num < num2)
		{
			return false;
		}
		if (sellOrder != null)
		{
			int num3 = sellOrder.currencyAmountPerItem * numberOfTransactions;
			if (sellOrder.currencyID == deliveryFeeCurrency.itemid && !sellOrder.currencyIsBP && num < num2 + num3)
			{
				return false;
			}
		}
		return true;
	}

	public bool HasPendingOrderFor(NetworkableId vendingMachineId)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		List<PendingOrder> list = pendingOrders;
		return ((list != null) ? List.FindWith<PendingOrder, NetworkableId>((IReadOnlyCollection<PendingOrder>)list, (Func<PendingOrder, NetworkableId>)((PendingOrder o) => o.vendingMachineId), vendingMachineId, (IEqualityComparer<NetworkableId>)null) : null) != null;
	}

	public bool CanPlayerInteract(BasePlayer player)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)player == (Object)null)
		{
			return false;
		}
		if (_customerSteamId == 0L || TimeUntil.op_Implicit(_timeUntilCustomerExpiry) <= 0f)
		{
			return true;
		}
		return player.userID == _customerSteamId;
	}

	public override void Load(LoadInfo info)
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		base.Load(info);
		if (info.msg.marketTerminal == null)
		{
			return;
		}
		_customerSteamId = info.msg.marketTerminal.customerSteamId;
		_customerName = info.msg.marketTerminal.customerName;
		_timeUntilCustomerExpiry = info.msg.marketTerminal.timeUntilExpiry;
		_marketplace = new EntityRef<Marketplace>(info.msg.marketTerminal.marketplaceId);
		if (pendingOrders == null)
		{
			pendingOrders = Pool.GetList<PendingOrder>();
		}
		if (pendingOrders.Count > 0)
		{
			foreach (PendingOrder pendingOrder in pendingOrders)
			{
				PendingOrder current = pendingOrder;
				Pool.Free<PendingOrder>(ref current);
			}
			pendingOrders.Clear();
		}
		foreach (PendingOrder order in info.msg.marketTerminal.orders)
		{
			PendingOrder item = order.Copy();
			pendingOrders.Add(item);
		}
	}

	public void Setup(Marketplace marketplace)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		_marketplace = new EntityRef<Marketplace>(marketplace.net.ID);
	}

	public override void ServerInit()
	{
		base.ServerInit();
		_onCurrencyRemovedCached = OnCurrencyRemoved;
		_onItemPurchasedCached = OnItemPurchased;
		_checkForExpiredOrdersCached = CheckForExpiredOrders;
	}

	private void RegisterOrder(BasePlayer player, VendingMachine vendingMachine)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		if (pendingOrders == null)
		{
			pendingOrders = Pool.GetList<PendingOrder>();
		}
		if (HasPendingOrderFor(vendingMachine.net.ID))
		{
			return;
		}
		if (!_marketplace.TryGet(serverside: true, out var entity))
		{
			Debug.LogError((object)"Marketplace is not set", (Object)(object)this);
			return;
		}
		NetworkableId droneId = entity.SendDrone(player, this, vendingMachine);
		if (!((NetworkableId)(ref droneId)).IsValid)
		{
			Debug.LogError((object)"Failed to spawn delivery drone");
			return;
		}
		PendingOrder val = Pool.Get<PendingOrder>();
		val.vendingMachineId = vendingMachine.net.ID;
		val.timeUntilExpiry = TimeUntil.op_Implicit((float)orderTimeout);
		val.droneId = droneId;
		pendingOrders.Add(val);
		CheckForExpiredOrders();
		UpdateHasItems(sendNetworkUpdate: false);
		SendNetworkUpdateImmediate();
	}

	public void CompleteOrder(NetworkableId vendingMachineId)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		if (pendingOrders != null)
		{
			int num = List.FindIndexWith<PendingOrder, NetworkableId>((IReadOnlyList<PendingOrder>)pendingOrders, (Func<PendingOrder, NetworkableId>)((PendingOrder o) => o.vendingMachineId), vendingMachineId, (IEqualityComparer<NetworkableId>)null);
			if (num < 0)
			{
				Debug.LogError((object)"Completed market order that doesn't exist?");
				return;
			}
			pendingOrders[num].Dispose();
			pendingOrders.RemoveAt(num);
			CheckForExpiredOrders();
			UpdateHasItems(sendNetworkUpdate: false);
			SendNetworkUpdateImmediate();
		}
	}

	private void CheckForExpiredOrders()
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		if (pendingOrders != null && pendingOrders.Count > 0)
		{
			bool flag = false;
			float? num = null;
			for (int i = 0; i < pendingOrders.Count; i++)
			{
				PendingOrder val = pendingOrders[i];
				if (TimeUntil.op_Implicit(val.timeUntilExpiry) <= 0f)
				{
					if (new EntityRef<DeliveryDrone>(val.droneId).TryGet(serverside: true, out var entity))
					{
						Debug.LogError((object)"Delivery timed out waiting for drone (too slow speed?)", (Object)(object)this);
						entity.Kill();
					}
					else
					{
						Debug.LogError((object)"Delivery timed out waiting for drone, and the drone seems to have been destroyed?", (Object)(object)this);
					}
					pendingOrders.RemoveAt(i);
					i--;
					flag = true;
				}
				else if (!num.HasValue || TimeUntil.op_Implicit(val.timeUntilExpiry) < num.Value)
				{
					num = TimeUntil.op_Implicit(val.timeUntilExpiry);
				}
			}
			if (flag)
			{
				UpdateHasItems(sendNetworkUpdate: false);
				SendNetworkUpdate();
			}
			if (num.HasValue)
			{
				((FacepunchBehaviour)this).Invoke(_checkForExpiredOrdersCached, num.Value);
			}
		}
		else
		{
			((FacepunchBehaviour)this).CancelInvoke(_checkForExpiredOrdersCached);
		}
	}

	private void RestrictToPlayer(BasePlayer player)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		if (_customerSteamId == player.userID)
		{
			_timeUntilCustomerExpiry = TimeUntil.op_Implicit((float)lockToCustomerDuration);
			SendNetworkUpdate();
			return;
		}
		if (_customerSteamId != 0L)
		{
			Debug.LogError((object)"Overwriting player restriction! It should be cleared first.", (Object)(object)this);
		}
		_customerSteamId = player.userID;
		_customerName = player.displayName;
		_timeUntilCustomerExpiry = TimeUntil.op_Implicit((float)lockToCustomerDuration);
		SendNetworkUpdateImmediate();
		ClientRPC(null, "Client_CloseMarketUI", _customerSteamId);
		RemoveAnyLooters();
		if (IsOpen())
		{
			Debug.LogError((object)"Market terminal's inventory is still open after removing looters!", (Object)(object)this);
		}
	}

	private void ClearRestriction()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		if (_customerSteamId != 0L)
		{
			_customerSteamId = 0uL;
			_customerName = null;
			_timeUntilCustomerExpiry = TimeUntil.op_Implicit(0f);
			SendNetworkUpdateImmediate();
		}
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	[RPC_Server.CallsPerSecond(3uL)]
	public void Server_TryOpenMarket(RPCMessage msg)
	{
		if (!CanPlayerInteract(msg.player))
		{
			return;
		}
		if (!_marketplace.IsValid(serverside: true))
		{
			Debug.LogError((object)"Marketplace is not set", (Object)(object)this);
			return;
		}
		EntityIdList val = Pool.Get<EntityIdList>();
		try
		{
			val.entityIds = Pool.GetList<NetworkableId>();
			GetDeliveryEligibleVendingMachines(val.entityIds);
			ClientRPCPlayer<EntityIdList>(null, msg.player, "Client_OpenMarket", val);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	[RPC_Server.CallsPerSecond(10uL)]
	public void Server_Purchase(RPCMessage msg)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		if (!CanPlayerInteract(msg.player))
		{
			return;
		}
		if (!_marketplace.IsValid(serverside: true))
		{
			Debug.LogError((object)"Marketplace is not set", (Object)(object)this);
			return;
		}
		NetworkableId val = msg.read.EntityID();
		int num = msg.read.Int32();
		int num2 = msg.read.Int32();
		VendingMachine vendingMachine = BaseNetworkable.serverEntities.Find(val) as VendingMachine;
		if ((Object)(object)vendingMachine == (Object)null || !vendingMachine.IsValid() || num < 0 || num >= vendingMachine.sellOrders.sellOrders.Count || num2 <= 0 || base.inventory.IsFull())
		{
			return;
		}
		GetDeliveryEligibleVendingMachines(null);
		if (_deliveryEligible == null || !_deliveryEligible.Contains(val))
		{
			return;
		}
		try
		{
			_transactionActive = true;
			int num3 = deliveryFeeAmount;
			SellOrder sellOrder = vendingMachine.sellOrders.sellOrders[num];
			if (!CanPlayerAffordOrderAndDeliveryFee(msg.player, sellOrder, num2))
			{
				return;
			}
			int num4 = msg.player.inventory.Take(null, deliveryFeeCurrency.itemid, num3);
			if (num4 != num3)
			{
				Debug.LogError((object)$"Took an incorrect number of items for the delivery fee (took {num4}, should have taken {num3})");
			}
			ClientRPCPlayer(null, msg.player, "Client_ShowItemNotice", deliveryFeeCurrency.itemid, -num3, arg3: false);
			if (!vendingMachine.DoTransaction(msg.player, num, num2, base.inventory, _onCurrencyRemovedCached, _onItemPurchasedCached, this))
			{
				Item item = ItemManager.CreateByItemID(deliveryFeeCurrency.itemid, num3, 0uL);
				if (!msg.player.inventory.GiveItem(item))
				{
					item.Drop(msg.player.inventory.containerMain.dropPosition, msg.player.inventory.containerMain.dropVelocity);
				}
			}
			else
			{
				RestrictToPlayer(msg.player);
				RegisterOrder(msg.player, vendingMachine);
			}
		}
		finally
		{
			_transactionActive = false;
		}
	}

	private void UpdateHasItems(bool sendNetworkUpdate = true)
	{
		if (!Application.isLoadingSave)
		{
			bool flag = base.inventory.itemList.Count > 0;
			bool flag2 = pendingOrders != null && pendingOrders.Count != 0;
			SetFlag(Flags.Reserved1, flag && !flag2, recursive: false, sendNetworkUpdate);
			SetFlag(Flags.Reserved2, base.inventory.IsFull(), recursive: false, sendNetworkUpdate);
			if (!flag && !flag2)
			{
				ClearRestriction();
			}
		}
	}

	private void OnCurrencyRemoved(BasePlayer player, Item currencyItem)
	{
		if ((Object)(object)player != (Object)null && currencyItem != null)
		{
			ClientRPCPlayer(null, player, "Client_ShowItemNotice", currencyItem.info.itemid, -currencyItem.amount, arg3: false);
		}
	}

	private void OnItemPurchased(BasePlayer player, Item purchasedItem)
	{
		if ((Object)(object)player != (Object)null && purchasedItem != null)
		{
			ClientRPCPlayer(null, player, "Client_ShowItemNotice", purchasedItem.info.itemid, purchasedItem.amount, arg3: true);
		}
	}

	public override void Save(SaveInfo info)
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		base.Save(info);
		info.msg.marketTerminal = Pool.Get<MarketTerminal>();
		info.msg.marketTerminal.customerSteamId = _customerSteamId;
		info.msg.marketTerminal.customerName = _customerName;
		info.msg.marketTerminal.timeUntilExpiry = _timeUntilCustomerExpiry;
		info.msg.marketTerminal.marketplaceId = _marketplace.uid;
		info.msg.marketTerminal.orders = Pool.GetList<PendingOrder>();
		if (pendingOrders == null)
		{
			return;
		}
		foreach (PendingOrder pendingOrder in pendingOrders)
		{
			PendingOrder item = pendingOrder.Copy();
			info.msg.marketTerminal.orders.Add(item);
		}
	}

	public override bool ItemFilter(Item item, int targetSlot)
	{
		if (_transactionActive)
		{
			return true;
		}
		if (item.parent == null)
		{
			return true;
		}
		if (item.parent == base.inventory)
		{
			return true;
		}
		return false;
	}

	public override void OnItemAddedOrRemoved(Item item, bool added)
	{
		UpdateHasItems();
	}

	public override bool CanOpenLootPanel(BasePlayer player, string panelName)
	{
		if (CanPlayerInteract(player) && HasFlag(Flags.Reserved1))
		{
			return base.CanOpenLootPanel(player, panelName);
		}
		return false;
	}

	private void RemoveAnyLooters()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		ItemContainer item = base.inventory;
		Enumerator<BasePlayer> enumerator = BasePlayer.activePlayerList.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				BasePlayer current = enumerator.Current;
				if (!((Object)(object)current == (Object)null) && !((Object)(object)current.inventory == (Object)null) && !((Object)(object)current.inventory.loot == (Object)null) && current.inventory.loot.containers.Contains(item))
				{
					current.inventory.loot.Clear();
				}
			}
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
	}

	public void GetDeliveryEligibleVendingMachines(List<NetworkableId> vendingMachineIds)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		if (RealTimeSince.op_Implicit(_deliveryEligibleLastCalculated) < 5f)
		{
			if (vendingMachineIds == null)
			{
				return;
			}
			{
				foreach (NetworkableId item in _deliveryEligible)
				{
					vendingMachineIds.Add(item);
				}
				return;
			}
		}
		_deliveryEligibleLastCalculated = RealTimeSince.op_Implicit(0f);
		_deliveryEligible.Clear();
		foreach (MapMarker serverMapMarker in MapMarker.serverMapMarkers)
		{
			if (serverMapMarker is VendingMachineMapMarker vendingMachineMapMarker && !((Object)(object)vendingMachineMapMarker.server_vendingMachine == (Object)null))
			{
				VendingMachine server_vendingMachine = vendingMachineMapMarker.server_vendingMachine;
				if (!((Object)(object)server_vendingMachine == (Object)null) && (IsEligible(server_vendingMachine, config.vendingMachineOffset, 1) || IsEligible(server_vendingMachine, config.vendingMachineOffset + Vector3.forward * config.maxDistanceFromVendingMachine, 2)))
				{
					_deliveryEligible.Add(server_vendingMachine.net.ID);
				}
			}
		}
		if (vendingMachineIds == null)
		{
			return;
		}
		foreach (NetworkableId item2 in _deliveryEligible)
		{
			vendingMachineIds.Add(item2);
		}
		bool IsEligible(VendingMachine vendingMachine, Vector3 offset, int n)
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			if (vendingMachine is NPCVendingMachine)
			{
				return true;
			}
			if (!vendingMachine.IsBroadcasting())
			{
				return false;
			}
			if (!config.IsVendingMachineAccessible(vendingMachine, offset, out var _))
			{
				return false;
			}
			return true;
		}
	}
}
