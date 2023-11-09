using System;
using System.Collections.Generic;
using System.Linq;
using ConVar;
using Facepunch;
using Facepunch.Rust;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

public class BaseOven : StorageContainer, ISplashable, IIndustrialStorage
{
	public enum TemperatureType
	{
		Normal,
		Warming,
		Cooking,
		Smelting,
		Fractioning
	}

	public enum IndustrialSlotMode
	{
		Furnace,
		LargeFurnace,
		OilRefinery,
		ElectricFurnace
	}

	public struct MinMax
	{
		public int Min;

		public int Max;

		public MinMax(int min, int max)
		{
			Min = min;
			Max = max;
		}
	}

	private enum OvenItemType
	{
		Burnable,
		Byproduct,
		MaterialInput,
		MaterialOutput
	}

	private static Dictionary<float, HashSet<ItemDefinition>> _materialOutputCache;

	public TemperatureType temperature;

	public Menu.Option switchOnMenu;

	public Menu.Option switchOffMenu;

	public ItemAmount[] startupContents;

	public bool allowByproductCreation = true;

	public ItemDefinition fuelType;

	public bool canModFire;

	public bool disabledBySplash = true;

	public int smeltSpeed = 1;

	public int fuelSlots = 1;

	public int inputSlots = 1;

	public int outputSlots = 1;

	public IndustrialSlotMode IndustrialMode;

	private int _activeCookingSlot = -1;

	private int _inputSlotIndex;

	private int _outputSlotIndex;

	private const float UpdateRate = 0.5f;

	protected virtual bool CanRunWithNoFuel => false;

	public ItemContainer Container => base.inventory;

	public BaseEntity IndustrialEntity => this;

	private float cookingTemperature => temperature switch
	{
		TemperatureType.Fractioning => 1500f, 
		TemperatureType.Cooking => 200f, 
		TemperatureType.Smelting => 1000f, 
		TemperatureType.Warming => 50f, 
		_ => 15f, 
	};

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("BaseOven.OnRpcMessage", 0);
		try
		{
			if (rpc == 4167839872u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - SVSwitch "));
				}
				TimeWarning val2 = TimeWarning.New("SVSwitch", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.MaxDistance.Test(4167839872u, "SVSwitch", this, player, 3f))
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
							SVSwitch(msg2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in SVSwitch");
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

	public override void PreInitShared()
	{
		base.PreInitShared();
		_inputSlotIndex = fuelSlots;
		_outputSlotIndex = _inputSlotIndex + inputSlots;
		_activeCookingSlot = _inputSlotIndex;
	}

	public override void ServerInit()
	{
		inventorySlots = fuelSlots + inputSlots + outputSlots;
		base.ServerInit();
	}

	public override void PostServerLoad()
	{
		base.PostServerLoad();
		if (IsOn())
		{
			StartCooking();
		}
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		if (!info.forDisk)
		{
			info.msg.baseOven = Pool.Get<BaseOven>();
			info.msg.baseOven.cookSpeed = GetSmeltingSpeed();
		}
	}

	public override void OnInventoryFirstCreated(ItemContainer container)
	{
		base.OnInventoryFirstCreated(container);
		if (startupContents != null)
		{
			ItemAmount[] array = startupContents;
			foreach (ItemAmount itemAmount in array)
			{
				ItemManager.Create(itemAmount.itemDef, (int)itemAmount.amount, 0uL).MoveToContainer(container);
			}
		}
	}

	public override void OnItemAddedOrRemoved(Item item, bool bAdded)
	{
		base.OnItemAddedOrRemoved(item, bAdded);
		if (item != null)
		{
			ItemModCookable component = ((Component)item.info).GetComponent<ItemModCookable>();
			if ((Object)(object)component != (Object)null)
			{
				item.cookTimeLeft = component.cookTime;
			}
			if (item.HasFlag(Item.Flag.OnFire))
			{
				item.SetFlag(Item.Flag.OnFire, b: false);
				item.MarkDirty();
			}
			if (item.HasFlag(Item.Flag.Cooking))
			{
				item.SetFlag(Item.Flag.Cooking, b: false);
				item.MarkDirty();
			}
		}
	}

	public override bool ItemFilter(Item item, int targetSlot)
	{
		if (!base.ItemFilter(item, targetSlot))
		{
			return false;
		}
		if (targetSlot == -1)
		{
			return false;
		}
		if (IsOutputItem(item) && (Object)(object)item.GetEntityOwner() != (Object)(object)this)
		{
			BaseEntity entityOwner = item.GetEntityOwner();
			if ((Object)(object)entityOwner != (Object)(object)this && (Object)(object)entityOwner != (Object)null)
			{
				return false;
			}
		}
		MinMax? allowedSlots = GetAllowedSlots(item);
		if (!allowedSlots.HasValue)
		{
			return false;
		}
		if (targetSlot >= allowedSlots.Value.Min)
		{
			return targetSlot <= allowedSlots.Value.Max;
		}
		return false;
	}

	private MinMax? GetAllowedSlots(Item item)
	{
		int num = 0;
		int num2 = 0;
		if (IsBurnableItem(item))
		{
			num2 = fuelSlots;
		}
		else if (IsOutputItem(item))
		{
			num = _outputSlotIndex;
			num2 = num + outputSlots;
		}
		else
		{
			if (!IsMaterialInput(item))
			{
				return null;
			}
			num = _inputSlotIndex;
			num2 = num + inputSlots;
		}
		return new MinMax(num, num2 - 1);
	}

	public MinMax GetOutputSlotRange()
	{
		return new MinMax(_outputSlotIndex, _outputSlotIndex + outputSlots - 1);
	}

	public override int GetIdealSlot(BasePlayer player, Item item)
	{
		MinMax? allowedSlots = GetAllowedSlots(item);
		if (!allowedSlots.HasValue)
		{
			return -1;
		}
		for (int i = allowedSlots.Value.Min; i <= allowedSlots.Value.Max; i++)
		{
			Item slot = base.inventory.GetSlot(i);
			if (slot == null || (slot.CanStack(item) && slot.amount < slot.MaxStackable()))
			{
				return i;
			}
		}
		return base.GetIdealSlot(player, item);
	}

	public void OvenFull()
	{
		StopCooking();
	}

	private int GetFuelRate()
	{
		return 1;
	}

	private int GetCharcoalRate()
	{
		return 1;
	}

	public void Cook()
	{
		Item item = FindBurnable();
		if (item == null && !CanRunWithNoFuel)
		{
			StopCooking();
			return;
		}
		foreach (Item item2 in base.inventory.itemList)
		{
			if (item2.position >= _inputSlotIndex && item2.position < _inputSlotIndex + inputSlots && !item2.HasFlag(Item.Flag.Cooking))
			{
				item2.SetFlag(Item.Flag.Cooking, b: true);
				item2.MarkDirty();
			}
		}
		IncreaseCookTime(0.5f * GetSmeltingSpeed());
		BaseEntity slot = GetSlot(Slot.FireMod);
		if (Object.op_Implicit((Object)(object)slot))
		{
			((Component)slot).SendMessage("Cook", (object)0.5f, (SendMessageOptions)1);
		}
		if (item != null)
		{
			ItemModBurnable component = ((Component)item.info).GetComponent<ItemModBurnable>();
			item.fuel -= 0.5f * (cookingTemperature / 200f);
			if (!item.HasFlag(Item.Flag.OnFire))
			{
				item.SetFlag(Item.Flag.OnFire, b: true);
				item.MarkDirty();
			}
			if (item.fuel <= 0f)
			{
				ConsumeFuel(item, component);
			}
		}
		OnCooked();
	}

	protected virtual void OnCooked()
	{
	}

	private void ConsumeFuel(Item fuel, ItemModBurnable burnable)
	{
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		if (allowByproductCreation && (Object)(object)burnable.byproductItem != (Object)null && Random.Range(0f, 1f) > burnable.byproductChance)
		{
			Item item = ItemManager.Create(burnable.byproductItem, burnable.byproductAmount * GetCharcoalRate(), 0uL);
			if (!item.MoveToContainer(base.inventory))
			{
				OvenFull();
				item.Drop(base.inventory.dropPosition, base.inventory.dropVelocity);
			}
		}
		if (fuel.amount <= GetFuelRate())
		{
			fuel.Remove();
			return;
		}
		int fuelRate = GetFuelRate();
		fuel.UseItem(fuelRate);
		Analytics.Azure.AddPendingItems(this, fuel.info.shortname, fuelRate, "smelt");
		fuel.fuel = burnable.fuelAmount;
		fuel.MarkDirty();
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	protected virtual void SVSwitch(RPCMessage msg)
	{
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		bool flag = msg.read.Bit();
		if (flag == IsOn() || (needsBuildingPrivilegeToUse && !msg.player.CanBuild()))
		{
			return;
		}
		if (flag)
		{
			StartCooking();
			if ((Object)(object)msg.player != (Object)null)
			{
				msg.player.ProcessMissionEvent(BaseMission.MissionEventType.STARTOVEN, new BaseMission.MissionEventPayload
				{
					UintIdentifier = prefabID,
					NetworkIdentifier = net.ID
				}, 1f);
			}
		}
		else
		{
			StopCooking();
		}
	}

	public float GetTemperature(int slot)
	{
		if (!HasFlag(Flags.On))
		{
			return 15f;
		}
		return cookingTemperature;
	}

	public void UpdateAttachmentTemperature()
	{
		BaseEntity slot = GetSlot(Slot.FireMod);
		if (Object.op_Implicit((Object)(object)slot))
		{
			((Component)slot).SendMessage("ParentTemperatureUpdate", (object)base.inventory.temperature, (SendMessageOptions)1);
		}
	}

	public virtual void StartCooking()
	{
		if (FindBurnable() != null || CanRunWithNoFuel)
		{
			base.inventory.temperature = cookingTemperature;
			UpdateAttachmentTemperature();
			((FacepunchBehaviour)this).InvokeRepeating((Action)Cook, 0.5f, 0.5f);
			SetFlag(Flags.On, b: true);
		}
	}

	public virtual void StopCooking()
	{
		UpdateAttachmentTemperature();
		if (base.inventory != null)
		{
			base.inventory.temperature = 15f;
			foreach (Item item in base.inventory.itemList)
			{
				if (item.HasFlag(Item.Flag.OnFire))
				{
					item.SetFlag(Item.Flag.OnFire, b: false);
					item.MarkDirty();
				}
				else if (item.HasFlag(Item.Flag.Cooking))
				{
					item.SetFlag(Item.Flag.Cooking, b: false);
					item.MarkDirty();
				}
			}
		}
		((FacepunchBehaviour)this).CancelInvoke((Action)Cook);
		SetFlag(Flags.On, b: false);
	}

	public bool WantsSplash(ItemDefinition splashType, int amount)
	{
		if (!base.IsDestroyed && IsOn())
		{
			return disabledBySplash;
		}
		return false;
	}

	public int DoSplash(ItemDefinition splashType, int amount)
	{
		StopCooking();
		return Mathf.Min(200, amount);
	}

	public Item FindBurnable()
	{
		if (base.inventory == null)
		{
			return null;
		}
		foreach (Item item in base.inventory.itemList)
		{
			if (IsBurnableItem(item))
			{
				return item;
			}
		}
		return null;
	}

	private void IncreaseCookTime(float amount)
	{
		List<Item> list = Pool.GetList<Item>();
		foreach (Item item in base.inventory.itemList)
		{
			if (item.HasFlag(Item.Flag.Cooking))
			{
				list.Add(item);
			}
		}
		float delta = amount / (float)list.Count;
		foreach (Item item2 in list)
		{
			item2.OnCycle(delta);
		}
		Pool.FreeList<Item>(ref list);
	}

	public Vector2i InputSlotRange(int slotIndex)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		if (IndustrialMode == IndustrialSlotMode.LargeFurnace)
		{
			return new Vector2i(0, 6);
		}
		if (IndustrialMode == IndustrialSlotMode.OilRefinery)
		{
			return new Vector2i(0, 1);
		}
		if (IndustrialMode == IndustrialSlotMode.ElectricFurnace)
		{
			return new Vector2i(0, 1);
		}
		return new Vector2i(0, 2);
	}

	public Vector2i OutputSlotRange(int slotIndex)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		if (IndustrialMode == IndustrialSlotMode.LargeFurnace)
		{
			return new Vector2i(7, 16);
		}
		if (IndustrialMode == IndustrialSlotMode.OilRefinery)
		{
			return new Vector2i(2, 4);
		}
		if (IndustrialMode == IndustrialSlotMode.ElectricFurnace)
		{
			return new Vector2i(2, 4);
		}
		return new Vector2i(3, 5);
	}

	public void OnStorageItemTransferBegin()
	{
	}

	public void OnStorageItemTransferEnd()
	{
	}

	public float GetSmeltingSpeed()
	{
		if (base.isServer)
		{
			return smeltSpeed;
		}
		throw new Exception("No way it should be able to get here?");
	}

	private bool IsBurnableItem(Item item)
	{
		if (Object.op_Implicit((Object)(object)((Component)item.info).GetComponent<ItemModBurnable>()) && ((Object)(object)fuelType == (Object)null || (Object)(object)item.info == (Object)(object)fuelType))
		{
			return true;
		}
		return false;
	}

	private bool IsBurnableByproduct(Item item)
	{
		ItemDefinition itemDefinition = fuelType;
		ItemModBurnable itemModBurnable = ((itemDefinition != null) ? ((Component)itemDefinition).GetComponent<ItemModBurnable>() : null);
		if ((Object)(object)itemModBurnable == (Object)null)
		{
			return false;
		}
		return (Object)(object)item.info == (Object)(object)itemModBurnable.byproductItem;
	}

	private bool IsMaterialInput(Item item)
	{
		ItemModCookable component = ((Component)item.info).GetComponent<ItemModCookable>();
		if ((Object)(object)component == (Object)null || (float)component.lowTemp > cookingTemperature || (float)component.highTemp < cookingTemperature)
		{
			return false;
		}
		return true;
	}

	private bool IsMaterialOutput(Item item)
	{
		if (_materialOutputCache == null)
		{
			BuildMaterialOutputCache();
		}
		if (!_materialOutputCache.TryGetValue(cookingTemperature, out var value))
		{
			Debug.LogError((object)"Can't find smeltable item list for oven");
			return true;
		}
		return value.Contains(item.info);
	}

	private bool IsOutputItem(Item item)
	{
		if (!IsMaterialOutput(item))
		{
			return IsBurnableByproduct(item);
		}
		return true;
	}

	private void BuildMaterialOutputCache()
	{
		_materialOutputCache = new Dictionary<float, HashSet<ItemDefinition>>();
		float[] array = (from x in GameManager.server.preProcessed.prefabList.Values
			select x.GetComponent<BaseOven>() into x
			where (Object)(object)x != (Object)null
			select x.cookingTemperature).Distinct().ToArray();
		foreach (float key in array)
		{
			HashSet<ItemDefinition> hashSet = new HashSet<ItemDefinition>();
			_materialOutputCache[key] = hashSet;
			foreach (ItemDefinition item in ItemManager.itemList)
			{
				ItemModCookable component = ((Component)item).GetComponent<ItemModCookable>();
				if (!((Object)(object)component == (Object)null) && component.CanBeCookedByAtTemperature(key))
				{
					hashSet.Add(component.becomeOnCooked);
				}
			}
		}
	}

	public override bool HasSlot(Slot slot)
	{
		if (canModFire && slot == Slot.FireMod)
		{
			return true;
		}
		return base.HasSlot(slot);
	}

	public override bool SupportsChildDeployables()
	{
		return true;
	}

	public override bool CanPickup(BasePlayer player)
	{
		if (base.CanPickup(player))
		{
			return CanPickupOven();
		}
		return false;
	}

	protected virtual bool CanPickupOven()
	{
		return children.Count == 0;
	}
}
