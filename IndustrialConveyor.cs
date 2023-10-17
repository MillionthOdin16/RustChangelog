using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class IndustrialConveyor : IndustrialEntity
{
	public enum ConveyorMode
	{
		Any,
		And,
		Not
	}

	public struct ItemFilter
	{
		public ItemDefinition TargetItem;

		public ItemCategory? TargetCategory;

		public int MaxAmountInOutput;

		public int BufferAmount;

		public int MinAmountInInput;

		public bool IsBlueprint;

		public int BufferTransferRemaining;

		public void CopyTo(ItemFilter target)
		{
			if ((Object)(object)TargetItem != (Object)null)
			{
				target.itemDef = TargetItem.itemid;
			}
			target.maxAmountInDestination = MaxAmountInOutput;
			if (TargetCategory.HasValue)
			{
				target.itemCategory = (int)TargetCategory.Value;
			}
			else
			{
				target.itemCategory = -1;
			}
			target.isBlueprint = (IsBlueprint ? 1 : 0);
			target.bufferAmount = BufferAmount;
			target.retainMinimum = MinAmountInInput;
			target.bufferTransferRemaining = BufferTransferRemaining;
		}

		public ItemFilter(ItemFilter from)
		{
			this = new ItemFilter
			{
				TargetItem = ItemManager.FindItemDefinition(from.itemDef),
				MaxAmountInOutput = from.maxAmountInDestination
			};
			if (from.itemCategory >= 0)
			{
				TargetCategory = (ItemCategory)from.itemCategory;
			}
			else
			{
				TargetCategory = null;
			}
			IsBlueprint = from.isBlueprint == 1;
			BufferAmount = from.bufferAmount;
			MinAmountInInput = from.retainMinimum;
			BufferTransferRemaining = from.bufferTransferRemaining;
		}
	}

	public int MaxStackSizePerMove = 128;

	public GameObjectRef FilterDialog;

	private const float ScreenUpdateRange = 30f;

	public const Flags FilterPassFlag = Flags.Reserved9;

	public const Flags FilterFailFlag = Flags.Reserved10;

	public const int MaxContainerDepth = 32;

	public SoundDefinition transferItemSoundDef;

	public SoundDefinition transferItemStartSoundDef;

	private List<ItemFilter> filterItems = new List<ItemFilter>();

	private ConveyorMode mode;

	public const int MAX_FILTER_SIZE = 12;

	public Image IconTransferImage;

	private bool refreshInputOutputs;

	private IIndustrialStorage workerOutput;

	private Func<IIndustrialStorage, int, bool> filterFunc;

	private List<ContainerInputOutput> splitOutputs = new List<ContainerInputOutput>();

	private List<ContainerInputOutput> splitInputs = new List<ContainerInputOutput>();

	private bool? lastFilterState;

	private Stopwatch transferStopWatch = new Stopwatch();

	private bool wasOnWhenPowerLost;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("IndustrialConveyor.OnRpcMessage", 0);
		try
		{
			if (rpc == 617569194 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - RPC_ChangeFilters "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_ChangeFilters", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(617569194u, "RPC_ChangeFilters", this, player, 1uL))
						{
							return true;
						}
						if (!RPC_Server.MaxDistance.Test(617569194u, "RPC_ChangeFilters", this, player, 3f))
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
							RPC_ChangeFilters(msg2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in RPC_ChangeFilters");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 3731379386u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - Server_RequestUpToDateFilters "));
				}
				TimeWarning val2 = TimeWarning.New("Server_RequestUpToDateFilters", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(3731379386u, "Server_RequestUpToDateFilters", this, player, 1uL))
						{
							return true;
						}
						if (!RPC_Server.IsVisible.Test(3731379386u, "Server_RequestUpToDateFilters", this, player, 3f))
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
							Server_RequestUpToDateFilters(msg3);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in Server_RequestUpToDateFilters");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 4167839872u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - SvSwitch "));
				}
				TimeWarning val2 = TimeWarning.New("SvSwitch", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(4167839872u, "SvSwitch", this, player, 2uL))
						{
							return true;
						}
						if (!RPC_Server.IsVisible.Test(4167839872u, "SvSwitch", this, player, 3f))
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
							SvSwitch(msg4);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex3)
					{
						Debug.LogException(ex3);
						player.Kick("RPC Error in SvSwitch");
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

	public override void OnFlagsChanged(Flags old, Flags next)
	{
		base.OnFlagsChanged(old, next);
		bool flag = next.HasFlag(Flags.On);
		if (old.HasFlag(Flags.On) != flag && base.isServer)
		{
			float conveyorMoveFrequency = Server.conveyorMoveFrequency;
			if (flag && conveyorMoveFrequency > 0f)
			{
				((FacepunchBehaviour)this).InvokeRandomized((Action)ScheduleMove, conveyorMoveFrequency, conveyorMoveFrequency, conveyorMoveFrequency * 0.5f);
			}
			else
			{
				((FacepunchBehaviour)this).CancelInvoke((Action)ScheduleMove);
			}
		}
	}

	private void ScheduleMove()
	{
		((ObjectWorkQueue<IndustrialEntity>)IndustrialEntity.Queue).Add((IndustrialEntity)this);
	}

	private Item GetItemToMove(IIndustrialStorage storage, out ItemFilter associatedFilter, int slot, ItemContainer targetContainer = null)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		associatedFilter = default(ItemFilter);
		(ItemFilter, int) tuple = default((ItemFilter, int));
		if (storage == null || storage.Container == null)
		{
			return null;
		}
		if (storage.Container.IsEmpty())
		{
			return null;
		}
		Vector2i val = storage.OutputSlotRange(slot);
		for (int i = val.x; i <= val.y; i++)
		{
			Item slot2 = storage.Container.GetSlot(i);
			tuple = default((ItemFilter, int));
			if (slot2 != null && (filterItems.Count == 0 || FilterHasItem(slot2, out tuple)))
			{
				(associatedFilter, _) = tuple;
				if (targetContainer == null || !((Object)(object)associatedFilter.TargetItem != (Object)null) || associatedFilter.MaxAmountInOutput <= 0 || targetContainer.GetTotalItemAmount(slot2, val.x, val.y) < associatedFilter.MaxAmountInOutput)
				{
					return slot2;
				}
			}
		}
		return null;
	}

	private bool FilterHasItem(Item item, out (ItemFilter filter, int index) filter)
	{
		filter = default((ItemFilter, int));
		for (int i = 0; i < filterItems.Count; i++)
		{
			ItemFilter itemFilter = filterItems[i];
			if (FilterMatches(itemFilter, item))
			{
				filter = (itemFilter, i);
				return true;
			}
		}
		return false;
	}

	private bool FilterMatches(ItemFilter filter, Item item)
	{
		if (item.IsBlueprint() && filter.IsBlueprint && (Object)(object)item.blueprintTargetDef == (Object)(object)filter.TargetItem)
		{
			return true;
		}
		if ((Object)(object)filter.TargetItem == (Object)(object)item.info && !filter.IsBlueprint)
		{
			return true;
		}
		if ((Object)(object)filter.TargetItem != (Object)null && (Object)(object)item.info.isRedirectOf == (Object)(object)filter.TargetItem)
		{
			return true;
		}
		if (filter.TargetCategory.HasValue && item.info.category == filter.TargetCategory)
		{
			return true;
		}
		return false;
	}

	private bool FilterContainerInput(IIndustrialStorage storage, int slot)
	{
		ItemFilter associatedFilter;
		return GetItemToMove(storage, out associatedFilter, slot, workerOutput?.Container) != null;
	}

	protected override void RunJob()
	{
		//IL_07b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_06eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_036d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0374: Unknown result type (might be due to invalid IL or missing references)
		//IL_0572: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0485: Unknown result type (might be due to invalid IL or missing references)
		//IL_048c: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d9: Unknown result type (might be due to invalid IL or missing references)
		base.RunJob();
		if (Server.conveyorMoveFrequency <= 0f)
		{
			return;
		}
		if (filterFunc == null)
		{
			filterFunc = FilterContainerInput;
		}
		if (refreshInputOutputs)
		{
			refreshInputOutputs = false;
			splitInputs.Clear();
			splitOutputs.Clear();
			FindContainerSource(splitInputs, 32, input: true);
			FindContainerSource(splitOutputs, 32, input: false, -1, MaxStackSizePerMove);
		}
		bool hasItems = CheckIfAnyInputPassesFilters(splitInputs);
		if ((!lastFilterState.HasValue || hasItems != lastFilterState) && !hasItems)
		{
			UpdateFilterPassthroughs();
		}
		if (!hasItems)
		{
			return;
		}
		transferStopWatch.Restart();
		IndustrialConveyorTransfer transfer = Pool.Get<IndustrialConveyorTransfer>();
		try
		{
			bool flag = false;
			transfer.ItemTransfers = Pool.GetList<ItemTransfer>();
			transfer.inputEntities = Pool.GetList<uint>();
			transfer.outputEntities = Pool.GetList<uint>();
			List<int> list = Pool.GetList<int>();
			int num = 0;
			int count = splitOutputs.Count;
			foreach (ContainerInputOutput splitOutput in splitOutputs)
			{
				workerOutput = splitOutput.Storage;
				foreach (ContainerInputOutput splitInput in splitInputs)
				{
					int num2 = 0;
					IIndustrialStorage storage = splitInput.Storage;
					if (storage == null || splitOutput.Storage == null || (Object)(object)splitInput.Storage.IndustrialEntity == (Object)(object)splitOutput.Storage.IndustrialEntity)
					{
						continue;
					}
					ItemContainer container = storage.Container;
					ItemContainer container2 = splitOutput.Storage.Container;
					if (container == null || container2 == null || storage.Container == null || storage.Container.IsEmpty())
					{
						continue;
					}
					(ItemFilter, int) filter2 = default((ItemFilter, int));
					Vector2i val = storage.OutputSlotRange(splitInput.SlotIndex);
					for (int i = val.x; i <= val.y; i++)
					{
						Vector2i val2 = splitOutput.Storage.InputSlotRange(splitOutput.SlotIndex);
						Item slot = storage.Container.GetSlot(i);
						if (slot == null)
						{
							continue;
						}
						bool flag2 = true;
						if (filterItems.Count > 0)
						{
							if (mode == ConveyorMode.Any || mode == ConveyorMode.And)
							{
								flag2 = FilterHasItem(slot, out filter2);
							}
							if (mode == ConveyorMode.Not)
							{
								flag2 = !FilterHasItem(slot, out filter2);
							}
						}
						if (!flag2)
						{
							continue;
						}
						bool flag3 = mode == ConveyorMode.And || mode == ConveyorMode.Any;
						if (flag3 && (Object)(object)filter2.Item1.TargetItem != (Object)null && filter2.Item1.MaxAmountInOutput > 0 && splitOutput.Storage.Container.GetTotalItemAmount(slot, val2.x, val2.y) >= filter2.Item1.MaxAmountInOutput)
						{
							flag = true;
							continue;
						}
						int num3 = Mathf.Min(MaxStackSizePerMove, slot.info.stackable);
						if (flag3 && filter2.Item1.MinAmountInInput > 0)
						{
							if ((Object)(object)filter2.Item1.TargetItem != (Object)null && FilterMatchItem(filter2.Item1, slot))
							{
								int totalItemAmount = container.GetTotalItemAmount(slot, val.x, val.y);
								num3 = Mathf.Min(num3, totalItemAmount - filter2.Item1.MinAmountInInput);
							}
							else if (filter2.Item1.TargetCategory.HasValue)
							{
								num3 = Mathf.Min(num3, container.GetTotalCategoryAmount(filter2.Item1.TargetCategory.Value, val2.x, val2.y) - filter2.Item1.MinAmountInInput);
							}
							if (num3 == 0)
							{
								continue;
							}
						}
						if (slot.amount == 1 || (num3 <= 0 && slot.amount > 0))
						{
							num3 = 1;
						}
						if (flag3 && filter2.Item1.BufferAmount > 0)
						{
							num3 = Mathf.Min(num3, filter2.Item1.BufferTransferRemaining);
						}
						if (flag3 && filter2.Item1.MaxAmountInOutput > 0)
						{
							if ((Object)(object)filter2.Item1.TargetItem != (Object)null && FilterMatchItem(filter2.Item1, slot))
							{
								num3 = Mathf.Min(num3, filter2.Item1.MaxAmountInOutput - container2.GetTotalItemAmount(slot, val2.x, val2.y));
							}
							else if (filter2.Item1.TargetCategory.HasValue)
							{
								num3 = Mathf.Min(num3, filter2.Item1.MaxAmountInOutput - container2.GetTotalCategoryAmount(filter2.Item1.TargetCategory.Value, val2.x, val2.y));
							}
							if ((float)num3 <= 0f)
							{
								flag = true;
							}
						}
						float num4 = (float)Mathf.Min(slot.amount, num3) / (float)count;
						if (num4 > 0f && num4 < 1f)
						{
							num4 = 1f;
						}
						num3 = (int)num4;
						if (num3 <= 0)
						{
							continue;
						}
						Item item2 = null;
						int amount2 = slot.amount;
						if (slot.amount > num3)
						{
							item2 = slot.SplitItem(num3);
							amount2 = item2.amount;
						}
						splitOutput.Storage.OnStorageItemTransferBegin();
						bool flag4 = false;
						for (int j = val2.x; j <= val2.y; j++)
						{
							Item slot2 = container2.GetSlot(j);
							if (slot2 != null && !((Object)(object)slot2.info == (Object)(object)slot.info))
							{
								continue;
							}
							if (item2 != null)
							{
								if (item2.MoveToContainer(container2, j, allowStack: true, ignoreStackLimit: false, null, allowSwap: false))
								{
									flag4 = true;
									break;
								}
							}
							else if (slot.MoveToContainer(container2, j, allowStack: true, ignoreStackLimit: false, null, allowSwap: false))
							{
								flag4 = true;
								break;
							}
						}
						if (filter2.Item1.BufferTransferRemaining > 0)
						{
							var (value, _) = filter2;
							value.BufferTransferRemaining -= amount2;
							filterItems[filter2.Item2] = value;
						}
						if (!flag4 && item2 != null)
						{
							slot.amount += item2.amount;
							slot.MarkDirty();
							item2.Remove();
							item2 = null;
						}
						if (flag4)
						{
							num2++;
							if (item2 != null)
							{
								AddTransfer(item2.info.itemid, amount2, splitInput.Storage.IndustrialEntity, splitOutput.Storage.IndustrialEntity);
							}
							else
							{
								AddTransfer(slot.info.itemid, amount2, splitInput.Storage.IndustrialEntity, splitOutput.Storage.IndustrialEntity);
							}
						}
						else if (!list.Contains(num))
						{
							list.Add(num);
						}
						splitOutput.Storage.OnStorageItemTransferEnd();
						if (num2 >= Server.maxItemStacksMovedPerTickIndustrial)
						{
							break;
						}
					}
				}
				num++;
			}
			if (transfer.ItemTransfers.Count == 0 && hasItems && flag)
			{
				hasItems = false;
			}
			if (!lastFilterState.HasValue || hasItems != lastFilterState)
			{
				UpdateFilterPassthroughs();
			}
			Pool.FreeList<int>(ref list);
			if (transfer.ItemTransfers.Count > 0)
			{
				ClientRPCEx<IndustrialConveyorTransfer>(new SendInfo(BaseNetworkable.GetConnectionsWithin(((Component)this).transform.position, 30f)), null, "ReceiveItemTransferDetails", transfer);
			}
		}
		finally
		{
			if (transfer != null)
			{
				((IDisposable)transfer).Dispose();
			}
		}
		void AddTransfer(int itemId, int amount, BaseEntity fromEntity, BaseEntity toEntity)
		{
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_011c: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			if (transfer != null && transfer.ItemTransfers != null)
			{
				if ((Object)(object)fromEntity != (Object)null && !transfer.inputEntities.Contains(fromEntity.net.ID))
				{
					transfer.inputEntities.Add(fromEntity.net.ID);
				}
				if ((Object)(object)toEntity != (Object)null && !transfer.outputEntities.Contains(toEntity.net.ID))
				{
					transfer.outputEntities.Add(toEntity.net.ID);
				}
				for (int k = 0; k < transfer.ItemTransfers.Count; k++)
				{
					ItemTransfer val3 = transfer.ItemTransfers[k];
					if (val3.itemId == itemId)
					{
						val3.amount += amount;
						transfer.ItemTransfers[k] = val3;
						return;
					}
				}
				ItemTransfer val4 = default(ItemTransfer);
				val4.itemId = itemId;
				val4.amount = amount;
				ItemTransfer item3 = val4;
				transfer.ItemTransfers.Add(item3);
			}
		}
		static bool FilterMatchItem(ItemFilter filter, Item item)
		{
			if ((Object)(object)filter.TargetItem != (Object)null && ((Object)(object)filter.TargetItem == (Object)(object)item.info || (item.IsBlueprint() == filter.IsBlueprint && (Object)(object)filter.TargetItem == (Object)(object)item.blueprintTargetDef)))
			{
				return true;
			}
			return false;
		}
		void UpdateFilterPassthroughs()
		{
			lastFilterState = hasItems;
			SetFlag(Flags.Reserved9, hasItems, recursive: false, networkupdate: false);
			SetFlag(Flags.Reserved10, !hasItems);
			ensureOutputsUpdated = true;
			MarkDirty();
		}
	}

	protected override void OnIndustrialNetworkChanged()
	{
		base.OnIndustrialNetworkChanged();
		refreshInputOutputs = true;
	}

	public override void PostServerLoad()
	{
		base.PostServerLoad();
		refreshInputOutputs = true;
	}

	private bool CheckIfAnyInputPassesFilters(List<ContainerInputOutput> inputs)
	{
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		if (filterItems.Count == 0)
		{
			foreach (ContainerInputOutput input in inputs)
			{
				if (GetItemToMove(input.Storage, out var _, input.SlotIndex) != null)
				{
					return true;
				}
			}
		}
		else
		{
			int num = 0;
			bool flag = false;
			if (mode == ConveyorMode.And)
			{
				foreach (ItemFilter filterItem in filterItems)
				{
					if (filterItem.BufferTransferRemaining > 0)
					{
						flag = true;
						break;
					}
				}
			}
			for (int i = 0; i < filterItems.Count; i++)
			{
				ItemFilter itemFilter = filterItems[i];
				int num2 = 0;
				int num3 = 0;
				foreach (ContainerInputOutput input2 in inputs)
				{
					Vector2i val = input2.Storage.OutputSlotRange(input2.SlotIndex);
					for (int j = val.x; j <= val.y; j++)
					{
						Item slot = input2.Storage.Container.GetSlot(j);
						if (slot == null)
						{
							continue;
						}
						bool flag2 = FilterMatches(itemFilter, slot);
						if (mode == ConveyorMode.Not)
						{
							flag2 = !flag2;
						}
						if (!flag2)
						{
							continue;
						}
						if (itemFilter.BufferAmount > 0)
						{
							num2 += slot.amount;
							if (itemFilter.BufferTransferRemaining > 0)
							{
								num++;
								break;
							}
							if (num2 >= itemFilter.BufferAmount + itemFilter.MinAmountInInput)
							{
								if (mode != ConveyorMode.And)
								{
									itemFilter.BufferTransferRemaining = itemFilter.BufferAmount;
									filterItems[i] = itemFilter;
								}
								num++;
								break;
							}
						}
						if (itemFilter.MinAmountInInput > 0)
						{
							num3 += slot.amount;
							if (num3 > itemFilter.MinAmountInInput + itemFilter.BufferAmount)
							{
								num++;
								break;
							}
						}
						if (itemFilter.BufferAmount == 0 && itemFilter.MinAmountInInput == 0)
						{
							num++;
							break;
						}
					}
					if ((mode == ConveyorMode.Any || mode == ConveyorMode.Not) && num > 0)
					{
						return true;
					}
					if (itemFilter.MinAmountInInput > 0)
					{
						num3 = 0;
					}
				}
				if (itemFilter.BufferTransferRemaining > 0 && num2 == 0)
				{
					itemFilter.BufferTransferRemaining = 0;
					filterItems[i] = itemFilter;
				}
			}
			if (mode == ConveyorMode.And && num == filterItems.Count)
			{
				if (!flag)
				{
					for (int k = 0; k < filterItems.Count; k++)
					{
						ItemFilter value = filterItems[k];
						value.BufferTransferRemaining = value.BufferAmount;
						filterItems[k] = value;
					}
				}
				return true;
			}
		}
		return false;
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		if (filterItems.Count == 0)
		{
			return;
		}
		info.msg.industrialConveyor = Pool.Get<IndustrialConveyor>();
		info.msg.industrialConveyor.filters = Pool.GetList<ItemFilter>();
		info.msg.industrialConveyor.conveyorMode = (int)mode;
		foreach (ItemFilter filterItem in filterItems)
		{
			ItemFilter val = Pool.Get<ItemFilter>();
			filterItem.CopyTo(val);
			info.msg.industrialConveyor.filters.Add(val);
		}
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	[RPC_Server.CallsPerSecond(1uL)]
	private void RPC_ChangeFilters(RPCMessage msg)
	{
		if ((Object)(object)msg.player == (Object)null || !msg.player.CanBuild())
		{
			return;
		}
		mode = (ConveyorMode)msg.read.Int32();
		filterItems.Clear();
		ItemFilterList val = ItemFilterList.Deserialize((Stream)(object)msg.read);
		if (val.filters == null)
		{
			return;
		}
		int num = Mathf.Min(val.filters.Count, 24);
		for (int i = 0; i < num; i++)
		{
			if (filterItems.Count >= 12)
			{
				break;
			}
			ItemFilter item = new ItemFilter(val.filters[i]);
			if ((Object)(object)item.TargetItem != (Object)null || item.TargetCategory.HasValue)
			{
				filterItems.Add(item);
			}
		}
		SendNetworkUpdate();
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	[RPC_Server.CallsPerSecond(2uL)]
	private void SvSwitch(RPCMessage msg)
	{
		SetSwitch(!IsOn());
	}

	public virtual void SetSwitch(bool wantsOn)
	{
		if (wantsOn == IsOn())
		{
			return;
		}
		SetFlag(Flags.On, wantsOn);
		SetFlag(Flags.Busy, b: true);
		SetFlag(Flags.Reserved10, b: false);
		SetFlag(Flags.Reserved9, b: false);
		if (!wantsOn)
		{
			lastFilterState = null;
		}
		ensureOutputsUpdated = true;
		((FacepunchBehaviour)this).Invoke((Action)Unbusy, 0.5f);
		for (int i = 0; i < filterItems.Count; i++)
		{
			ItemFilter value = filterItems[i];
			if (value.BufferTransferRemaining > 0)
			{
				value.BufferTransferRemaining = 0;
				filterItems[i] = value;
			}
		}
		SendNetworkUpdateImmediate();
		MarkDirty();
	}

	public void Unbusy()
	{
		SetFlag(Flags.Busy, b: false);
	}

	public override void UpdateHasPower(int inputAmount, int inputSlot)
	{
	}

	public override void IOStateChanged(int inputAmount, int inputSlot)
	{
		base.IOStateChanged(inputAmount, inputSlot);
		if (inputSlot == 1)
		{
			bool flag = inputAmount >= ConsumptionAmount() && inputAmount > 0;
			if (IsPowered() && IsOn() && !flag)
			{
				wasOnWhenPowerLost = true;
			}
			SetFlag(Flags.Reserved8, flag);
			if (!flag)
			{
				SetFlag(Flags.Reserved9, b: false);
				SetFlag(Flags.Reserved10, b: false);
			}
			currentEnergy = inputAmount;
			ensureOutputsUpdated = true;
			if (inputAmount <= 0 && IsOn())
			{
				SetSwitch(wantsOn: false);
			}
			if (inputAmount > 0 && wasOnWhenPowerLost && !IsOn())
			{
				SetSwitch(wantsOn: true);
				wasOnWhenPowerLost = false;
			}
			MarkDirty();
		}
		if (inputSlot == 2 && !IsOn() && inputAmount > 0 && IsPowered())
		{
			SetSwitch(wantsOn: true);
		}
		if (inputSlot == 3 && IsOn() && inputAmount > 0)
		{
			SetSwitch(wantsOn: false);
		}
	}

	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		switch (outputSlot)
		{
		case 2:
			if (!HasFlag(Flags.Reserved10))
			{
				return 0;
			}
			return 1;
		case 3:
			if (!HasFlag(Flags.Reserved9))
			{
				return 0;
			}
			return 1;
		case 1:
			return GetCurrentEnergy();
		default:
			return 0;
		}
	}

	public override bool ShouldDrainBattery(IOEntity battery)
	{
		return IsOn();
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	[RPC_Server.CallsPerSecond(1uL)]
	private void Server_RequestUpToDateFilters(RPCMessage msg)
	{
		if (!IsOn())
		{
			return;
		}
		ItemFilterList val = Pool.Get<ItemFilterList>();
		try
		{
			val.filters = Pool.GetList<ItemFilter>();
			foreach (ItemFilter filterItem in filterItems)
			{
				ItemFilter val2 = Pool.Get<ItemFilter>();
				filterItem.CopyTo(val2);
				val.filters.Add(val2);
			}
			ClientRPCPlayer<ItemFilterList>(null, msg.player, "Client_ReceiveBufferInfo", val);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		filterItems.Clear();
		if (info.msg.industrialConveyor?.filters == null)
		{
			return;
		}
		mode = (ConveyorMode)info.msg.industrialConveyor.conveyorMode;
		foreach (ItemFilter filter in info.msg.industrialConveyor.filters)
		{
			ItemFilter item = new ItemFilter(filter);
			if ((Object)(object)item.TargetItem != (Object)null || item.TargetCategory.HasValue)
			{
				filterItems.Add(item);
			}
		}
	}
}
