using System;
using System.Collections.Generic;
using Facepunch;
using ProtoBuf;
using UnityEngine;

public class VendingMachineMapMarker : MapMarker
{
	public string markerShopName;

	public VendingMachine server_vendingMachine;

	public VendingMachine client_vendingMachine;

	[NonSerialized]
	public NetworkableId client_vendingMachineNetworkID;

	public GameObjectRef clusterMarkerObj;

	public void SetVendingMachine(VendingMachine vm, string shopName)
	{
		server_vendingMachine = vm;
		markerShopName = shopName;
		if (!((FacepunchBehaviour)this).IsInvoking((Action)TryUpdatePosition))
		{
			((FacepunchBehaviour)this).InvokeRandomized((Action)TryUpdatePosition, 30f, 30f, 10f);
		}
	}

	private void TryUpdatePosition()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)server_vendingMachine != (Object)null && (Object)(object)server_vendingMachine.GetParentEntity() != (Object)null)
		{
			((Component)this).transform.position = ((Component)server_vendingMachine).transform.position;
			try
			{
				syncPosition = true;
				NetworkPositionTick();
			}
			finally
			{
				syncPosition = false;
			}
		}
	}

	public override void Save(SaveInfo info)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected O, but got Unknown
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Expected O, but got Unknown
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Expected O, but got Unknown
		base.Save(info);
		info.msg.vendingMachine = new VendingMachine();
		info.msg.vendingMachine.shopName = markerShopName;
		if (!((Object)(object)server_vendingMachine != (Object)null))
		{
			return;
		}
		info.msg.vendingMachine.networkID = server_vendingMachine.net.ID;
		info.msg.vendingMachine.sellOrderContainer = new SellOrderContainer();
		info.msg.vendingMachine.sellOrderContainer.ShouldPool = false;
		info.msg.vendingMachine.sellOrderContainer.sellOrders = new List<SellOrder>();
		foreach (SellOrder sellOrder in server_vendingMachine.sellOrders.sellOrders)
		{
			SellOrder val = new SellOrder();
			val.ShouldPool = false;
			sellOrder.CopyTo(val);
			info.msg.vendingMachine.sellOrderContainer.sellOrders.Add(val);
		}
	}

	public override AppMarker GetAppMarkerData()
	{
		AppMarker appMarkerData = base.GetAppMarkerData();
		appMarkerData.name = markerShopName ?? "";
		appMarkerData.outOfStock = !HasFlag(Flags.Busy);
		if ((Object)(object)server_vendingMachine != (Object)null)
		{
			appMarkerData.sellOrders = Pool.GetList<SellOrder>();
			foreach (SellOrder sellOrder in server_vendingMachine.sellOrders.sellOrders)
			{
				SellOrder val = Pool.Get<SellOrder>();
				val.itemId = sellOrder.itemToSellID;
				val.quantity = sellOrder.itemToSellAmount;
				val.currencyId = sellOrder.currencyID;
				val.costPerItem = sellOrder.currencyAmountPerItem;
				val.amountInStock = sellOrder.inStock;
				val.itemIsBlueprint = sellOrder.itemToSellIsBP;
				val.currencyIsBlueprint = sellOrder.currencyIsBP;
				val.itemCondition = sellOrder.itemCondition;
				val.itemConditionMax = sellOrder.itemConditionMax;
				appMarkerData.sellOrders.Add(val);
			}
		}
		return appMarkerData;
	}
}
