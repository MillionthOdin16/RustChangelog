using System;
using System.Collections.Generic;
using System.Text;
using Facepunch;
using Facepunch.Rust;
using ProtoBuf;
using UnityEngine;

public class NPCVendingMachine : VendingMachine
{
	public class SalesData
	{
		public ulong TotalSales;

		public ulong TotalIntervals;

		public ulong SoldThisInterval;

		public float CurrentMultiplier;

		public double GetAverageSalesPerInterval()
		{
			if (TotalSales == 0L || TotalIntervals == 0L)
			{
				return 0.0;
			}
			return (double)TotalSales / (double)TotalIntervals;
		}

		public void RecordSale(int count)
		{
			SoldThisInterval += (ulong)count;
		}

		public void ProcessEndOfInterval()
		{
			double averageSalesPerInterval = GetAverageSalesPerInterval();
			bool flag = TotalIntervals == 0;
			TotalSales += SoldThisInterval;
			TotalIntervals++;
			SoldThisInterval = 0uL;
			if (GetAverageSalesPerInterval() <= averageSalesPerInterval || flag)
			{
				CurrentMultiplier -= PriceDecreaseAmount;
			}
			else
			{
				CurrentMultiplier += PriceIncreaseAmount;
			}
			CurrentMultiplier = Mathf.Clamp(CurrentMultiplier, MinimumPriceMultiplier, MaximumPriceMultiplier);
		}
	}

	public NPCVendingOrder vendingOrders;

	public Phrase Phrase;

	public float RefillTime = 1f;

	public int StartingStock = 10;

	public bool BypassDynamicPricing;

	public const int MaxVendingEntries = 7;

	private static ListHashSet<NPCVendingMachine> allNpcVendingMachines = new ListHashSet<NPCVendingMachine>(8);

	private float[] refillTimes;

	[ServerVar(Saved = true, Help = "Whether to run the the dynamic pricing system")]
	public static bool DynamicPricingEnabled = true;

	[ServerVar(Saved = true, Help = "How many realtime hours are checked when looking for price increases. Max 72 (10 days)", ShowInAdminUI = true)]
	public static int PriceUpdateFrequency = 5;

	[ServerVar(Saved = true, Help = "The maximum point that a price can increase to (2 = 200%)")]
	public static float MaximumPriceMultiplier = 2f;

	[ServerVar(Saved = true, Help = "The Minimum point that the price can drop to (0.5 = 50% off)")]
	public static float MinimumPriceMultiplier = 0.5f;

	[ServerVar(Saved = true, Help = "What discount surcharge should be applied to items when the server starts")]
	public static float StartingPriceMultiplier = 2f;

	[ServerVar(Saved = true, Help = "How much to increase the price by if it is selling a lot (0.05 = 5%)")]
	public static float PriceIncreaseAmount = 0.1f;

	[ServerVar(Saved = true, Help = "How much to decrease the price for if it is underselling (0.05 = 5%)")]
	public static float PriceDecreaseAmount = 0.05f;

	private SalesData[] allSalesData;

	private float timeToNextSalesUpdate;

	private bool preserveSalesData;

	private static ItemDefinition _scrapItem = null;

	private TimeSince lastHourCheck;

	public static float IntervalSeconds => (float)(Mathf.Clamp(PriceUpdateFrequency, 1, 72) * 60) * 60f;

	public static ItemDefinition ScrapItem
	{
		get
		{
			if ((Object)(object)_scrapItem == (Object)null)
			{
				_scrapItem = ItemManager.FindItemDefinition("scrap");
			}
			return _scrapItem;
		}
	}

	private bool CanApplyDynamicPricing
	{
		get
		{
			if (!BypassDynamicPricing)
			{
				return DynamicPricingEnabled;
			}
			return false;
		}
	}

	public byte GetBPState(bool sellItemAsBP, bool currencyItemAsBP)
	{
		byte result = 0;
		if (sellItemAsBP)
		{
			result = 1;
		}
		if (currencyItemAsBP)
		{
			result = 2;
		}
		if (sellItemAsBP && currencyItemAsBP)
		{
			result = 3;
		}
		return result;
	}

	public override void TakeCurrencyItem(Item takenCurrencyItem)
	{
		takenCurrencyItem.MoveToContainer(base.inventory);
		takenCurrencyItem.RemoveFromContainer();
		takenCurrencyItem.Remove();
	}

	public override void GiveSoldItem(Item soldItem, BasePlayer buyer)
	{
		base.GiveSoldItem(soldItem, buyer);
	}

	public override void PostServerLoad()
	{
		base.PostServerLoad();
		((FacepunchBehaviour)this).Invoke((Action)InstallFromVendingOrders, 1f);
	}

	public void ChangeRefillTime(float newRefillTime)
	{
		RefillTime = newRefillTime;
		if (((FacepunchBehaviour)this).IsInvoking((Action)Refill))
		{
			((FacepunchBehaviour)this).CancelInvoke((Action)Refill);
		}
		((FacepunchBehaviour)this).InvokeRandomized((Action)Refill, 1f, RefillTime, 0.1f);
	}

	public override void ServerInit()
	{
		base.ServerInit();
		skinID = 861142659uL;
		SendNetworkUpdate();
		((FacepunchBehaviour)this).Invoke((Action)InstallFromVendingOrders, 1f);
		if (!((FacepunchBehaviour)this).IsInvoking((Action)Refill))
		{
			((FacepunchBehaviour)this).InvokeRandomized((Action)Refill, 1f, RefillTime, 0.1f);
		}
		DynamicPricingServerInit();
		allNpcVendingMachines.TryAdd(this);
	}

	internal override void DoServerDestroy()
	{
		base.DoServerDestroy();
		allNpcVendingMachines.Remove(this);
	}

	public virtual void InstallFromVendingOrders()
	{
		if ((Object)(object)vendingOrders == (Object)null)
		{
			Debug.LogError((object)"No vending orders!");
			return;
		}
		int count = sellOrders.sellOrders.Count;
		ClearSellOrders();
		base.inventory.Clear();
		ItemManager.DoRemoves();
		if (vendingOrders.orders.Length <= 7)
		{
			if (count == vendingOrders.orders.Length)
			{
				preserveSalesData = true;
			}
			try
			{
				NPCVendingOrder.Entry[] orders = vendingOrders.orders;
				foreach (NPCVendingOrder.Entry ent in orders)
				{
					SmartAddItemForSale(ent);
				}
				return;
			}
			finally
			{
				preserveSalesData = false;
			}
		}
		List<NPCVendingOrder.Entry> list = Pool.GetList<NPCVendingOrder.Entry>();
		vendingOrders.GetRandomEntries(7, list);
		foreach (NPCVendingOrder.Entry item in list)
		{
			SmartAddItemForSale(item);
		}
		Pool.FreeList<NPCVendingOrder.Entry>(ref list);
	}

	private void SmartAddItemForSale(NPCVendingOrder.Entry ent)
	{
		int currencyPerTransaction = ent.currencyAmount;
		if (ent.randomDetails.useRandom)
		{
			currencyPerTransaction = ent.randomDetails.GetRandomPrice();
		}
		AddItemForSale(ent.sellItem.itemid, ent.sellItemAmount, ent.currencyItem.itemid, currencyPerTransaction, GetBPState(ent.sellItemAsBP, ent.currencyAsBP));
	}

	public override void InstallDefaultSellOrders()
	{
		base.InstallDefaultSellOrders();
	}

	public void Refill()
	{
		if ((Object)(object)vendingOrders == (Object)null || vendingOrders.orders == null || base.inventory == null)
		{
			return;
		}
		if (refillTimes == null)
		{
			refillTimes = new float[vendingOrders.orders.Length];
		}
		for (int i = 0; i < vendingOrders.orders.Length; i++)
		{
			NPCVendingOrder.Entry entry = vendingOrders.orders[i];
			if (!(Time.realtimeSinceStartup > refillTimes[i]))
			{
				continue;
			}
			int num = Mathf.FloorToInt((float)(base.inventory.GetAmount(entry.sellItem.itemid, onlyUsableAmounts: false) / entry.sellItemAmount));
			int num2 = Mathf.Min(StartingStock - num, entry.refillAmount) * entry.sellItemAmount;
			if (num2 > 0)
			{
				transactionActive = true;
				Item item = null;
				if (entry.sellItemAsBP)
				{
					item = ItemManager.Create(base.blueprintBaseDef, num2, 0uL);
					item.blueprintTarget = entry.sellItem.itemid;
				}
				else
				{
					item = ItemManager.Create(entry.sellItem, num2, 0uL);
				}
				if (!item.MoveToContainer(base.inventory))
				{
					item.Remove();
				}
				transactionActive = false;
			}
			refillTimes[i] = Time.realtimeSinceStartup + entry.refillDelay;
		}
	}

	public void ClearSellOrders()
	{
		sellOrders.sellOrders.Clear();
	}

	public void AddItemForSale(int itemID, int amountToSell, int currencyID, int currencyPerTransaction, byte bpState)
	{
		AddSellOrder(itemID, amountToSell, currencyID, currencyPerTransaction, bpState);
		transactionActive = true;
		int startingStock = StartingStock;
		if (bpState == 1 || bpState == 3)
		{
			for (int i = 0; i < startingStock; i++)
			{
				Item item = ItemManager.CreateByItemID(base.blueprintBaseDef.itemid, 1, 0uL);
				item.blueprintTarget = itemID;
				base.inventory.Insert(item);
			}
		}
		else
		{
			base.inventory.AddItem(ItemManager.FindItemDefinition(itemID), amountToSell * startingStock, 0uL);
		}
		transactionActive = false;
		RefreshSellOrderStockLevel();
	}

	public void RefreshStock()
	{
	}

	protected override void RecordSaleAnalytics(Item itemSold, int orderId)
	{
		RecordSale(orderId, itemSold.amount);
		Analytics.Server.VendingMachineTransaction(vendingOrders, itemSold.info, itemSold.amount);
	}

	public override string GetTranslationToken()
	{
		return Phrase.token;
	}

	protected override bool CanRotate()
	{
		return false;
	}

	public override bool CanPlayerAdmin(BasePlayer player)
	{
		return false;
	}

	[ServerVar(Help = "Resets the state of all discounts and surcharges from NPC vending machines")]
	public static void resetDynamicPricing()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		Enumerator<NPCVendingMachine> enumerator = allNpcVendingMachines.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				enumerator.Current.ResetDynamicPricing();
			}
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
	}

	[ServerVar(Help = "Print out all current price changes on the server")]
	public static void printAllPriceChanges(Arg arg)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Expected O, but got Unknown
		StringBuilder stringBuilder = new StringBuilder();
		Enumerator<NPCVendingMachine> enumerator = allNpcVendingMachines.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				NPCVendingMachine current = enumerator.Current;
				TextTable val = new TextTable();
				val.AddColumns(new string[8] { "Item Name", "Original Price", "Discount/Surcharge", "Final Price", "Avg Sales/Interval", "Current Sales/Interval", "Total Sales", "Intervals" });
				int num = 0;
				int num2 = 0;
				foreach (SellOrder sellOrder in current.sellOrders.sellOrders)
				{
					if (sellOrder.priceMultiplier != 1f)
					{
						num++;
						ItemDefinition itemDefinition = ItemManager.FindItemDefinition(sellOrder.itemToSellID);
						int totalPriceForOrder = VendingMachine.GetTotalPriceForOrder(sellOrder);
						SalesData salesData = current.allSalesData[num2];
						val.AddRow(new string[8]
						{
							itemDefinition.shortname,
							sellOrder.currencyAmountPerItem.ToString(),
							$"{Mathf.RoundToInt(sellOrder.priceMultiplier * 100f)}%",
							$"{totalPriceForOrder}",
							salesData.GetAverageSalesPerInterval().ToString(),
							salesData.SoldThisInterval.ToString(),
							salesData.TotalSales.ToString(),
							salesData.TotalIntervals.ToString()
						});
					}
					num2++;
				}
				if (num > 0)
				{
					stringBuilder.AppendLine(current.shopName);
					stringBuilder.AppendLine("==============");
					stringBuilder.AppendLine(((object)val).ToString());
				}
			}
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
		arg.ReplyWith(stringBuilder.ToString());
	}

	[ServerVar(Help = "Simulates the provided number of hours passing in the vending machine system")]
	public static void addHours(Arg arg)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		float num = (float)arg.GetInt(0, 0) * 60f * 60f;
		Enumerator<NPCVendingMachine> enumerator = allNpcVendingMachines.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				NPCVendingMachine current = enumerator.Current;
				current.lastHourCheck = TimeSince.op_Implicit(TimeSince.op_Implicit(current.lastHourCheck) + num);
				current.HourCheck();
			}
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
	}

	public void RecordSale(int index, int count)
	{
		if (CanApplyDynamicPricing)
		{
			CheckSalesDataLength();
			allSalesData[index].RecordSale(count);
		}
	}

	private void CheckSalesDataLength(bool reset = false)
	{
		if (reset)
		{
			allSalesData = null;
		}
		int count = sellOrders.sellOrders.Count;
		if (allSalesData == null || allSalesData.Length != count)
		{
			allSalesData = new SalesData[count];
			for (int i = 0; i < count; i++)
			{
				allSalesData[i] = new SalesData
				{
					CurrentMultiplier = StartingPriceMultiplier
				};
			}
		}
	}

	protected override float GetDiscountForSlot(int sellOrderSlot, SellOrder forOrder)
	{
		if (!CanApplyDynamicPricing)
		{
			return 1f;
		}
		if (!preserveSalesData)
		{
			CheckSalesDataLength();
		}
		if (sellOrderSlot < 0 || sellOrderSlot >= allSalesData.Length)
		{
			return 1f;
		}
		if (forOrder.currencyID != ScrapItem.itemid)
		{
			return 1f;
		}
		return allSalesData[sellOrderSlot].CurrentMultiplier;
	}

	private void DynamicPricingServerInit()
	{
		timeToNextSalesUpdate = IntervalSeconds;
		((FacepunchBehaviour)this).InvokeRandomized((Action)HourCheck, 1f, 15f, 0.1f);
	}

	private void ResetDynamicPricing()
	{
		CheckSalesDataLength(reset: true);
		timeToNextSalesUpdate = IntervalSeconds;
		HourCheck();
	}

	private void HourCheck()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		if (!CanApplyDynamicPricing)
		{
			return;
		}
		float num = TimeSince.op_Implicit(lastHourCheck);
		lastHourCheck = TimeSince.op_Implicit(0f);
		timeToNextSalesUpdate -= num;
		while (timeToNextSalesUpdate < 0f)
		{
			timeToNextSalesUpdate += IntervalSeconds;
			SalesData[] array = allSalesData;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].ProcessEndOfInterval();
			}
			UpdateMapMarker();
			RefreshAndSendNetworkUpdate();
		}
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		if (allSalesData != null && info.forDisk)
		{
			info.msg.vendingDynamicPricing = Pool.Get<VendingDynamicPricing>();
			info.msg.vendingDynamicPricing.allSalesData = Pool.GetList<SalesData>();
			info.msg.vendingDynamicPricing.timeToNextSalesUpdate = timeToNextSalesUpdate;
			SalesData[] array = allSalesData;
			foreach (SalesData salesData in array)
			{
				SalesData val = Pool.Get<SalesData>();
				val.totalSales = salesData.TotalSales;
				val.totalIntervals = salesData.TotalIntervals;
				val.soldThisInterval = salesData.SoldThisInterval;
				val.CurrentMultiplier = salesData.CurrentMultiplier;
				info.msg.vendingDynamicPricing.allSalesData.Add(val);
			}
		}
	}

	public override void Load(LoadInfo info)
	{
		if (info.msg.vendingDynamicPricing != null)
		{
			allSalesData = new SalesData[info.msg.vendingDynamicPricing.allSalesData.Count];
			int num = 0;
			timeToNextSalesUpdate = info.msg.vendingDynamicPricing.timeToNextSalesUpdate;
			foreach (SalesData allSalesDatum in info.msg.vendingDynamicPricing.allSalesData)
			{
				SalesData salesData = new SalesData
				{
					TotalSales = allSalesDatum.totalSales,
					TotalIntervals = allSalesDatum.totalIntervals,
					SoldThisInterval = allSalesDatum.soldThisInterval,
					CurrentMultiplier = allSalesDatum.CurrentMultiplier
				};
				allSalesData[num] = salesData;
				num++;
			}
		}
		base.Load(info);
	}
}
