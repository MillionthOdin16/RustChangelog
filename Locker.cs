using System;
using ConVar;
using Network;
using UnityEngine;
using UnityEngine.Assertions;

public class Locker : StorageContainer
{
	private enum RowType
	{
		Clothing,
		Belt
	}

	public static class LockerFlags
	{
		public const Flags IsEquipping = Flags.Reserved1;
	}

	public GameObjectRef equipSound;

	private const int maxGearSets = 3;

	private const int attireSize = 7;

	private const int beltSize = 6;

	private const int columnSize = 2;

	private Item[] clothingBuffer = new Item[7];

	private const int setSize = 13;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("Locker.OnRpcMessage", 0);
		try
		{
			if (rpc == 1799659668 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_Equip "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_Equip", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(1799659668u, "RPC_Equip", this, player, 3f))
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
							RPC_Equip(msg2);
						}
						finally
						{
							((IDisposable)val4)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in RPC_Equip");
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

	public bool IsEquipping()
	{
		return HasFlag(Flags.Reserved1);
	}

	private RowType GetRowType(int slot)
	{
		if (slot == -1)
		{
			return RowType.Clothing;
		}
		int num = slot % 13;
		return (num >= 7) ? RowType.Belt : RowType.Clothing;
	}

	public override void ServerInit()
	{
		base.ServerInit();
		SetFlag(Flags.Reserved1, b: false);
	}

	public void ClearEquipping()
	{
		SetFlag(Flags.Reserved1, b: false);
	}

	public override bool ItemFilter(Item item, int targetSlot)
	{
		if (!base.ItemFilter(item, targetSlot))
		{
			return false;
		}
		if (item.info.category == ItemCategory.Attire)
		{
			return true;
		}
		return GetRowType(targetSlot) == RowType.Belt;
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	public void RPC_Equip(RPCMessage msg)
	{
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		int num = msg.read.Int32();
		if (num < 0 || num >= 3 || IsEquipping())
		{
			return;
		}
		BasePlayer player = msg.player;
		int num2 = num * 13;
		bool flag = false;
		for (int i = 0; i < player.inventory.containerWear.capacity; i++)
		{
			Item slot = player.inventory.containerWear.GetSlot(i);
			if (slot != null)
			{
				slot.RemoveFromContainer();
				clothingBuffer[i] = slot;
			}
		}
		for (int j = 0; j < 7; j++)
		{
			int num3 = num2 + j;
			Item slot2 = base.inventory.GetSlot(num3);
			Item item = clothingBuffer[j];
			if (slot2 != null)
			{
				flag = true;
				if (slot2.info.category != ItemCategory.Attire || !slot2.MoveToContainer(player.inventory.containerWear, j))
				{
					slot2.Drop(GetDropPosition(), GetDropVelocity());
				}
			}
			if (item != null)
			{
				flag = true;
				if (item.info.category != ItemCategory.Attire || !item.MoveToContainer(base.inventory, num3))
				{
					item.Drop(GetDropPosition(), GetDropVelocity());
				}
			}
			clothingBuffer[j] = null;
		}
		for (int k = 0; k < 6; k++)
		{
			int num4 = num2 + k + 7;
			int iTargetPos = k;
			Item slot3 = base.inventory.GetSlot(num4);
			Item slot4 = player.inventory.containerBelt.GetSlot(k);
			slot4?.RemoveFromContainer();
			if (slot3 != null)
			{
				flag = true;
				if (!slot3.MoveToContainer(player.inventory.containerBelt, iTargetPos))
				{
					slot3.Drop(GetDropPosition(), GetDropVelocity());
				}
			}
			if (slot4 != null)
			{
				flag = true;
				if (!slot4.MoveToContainer(base.inventory, num4))
				{
					slot4.Drop(GetDropPosition(), GetDropVelocity());
				}
			}
		}
		if (flag)
		{
			Effect.server.Run(equipSound.resourcePath, player, StringPool.Get("spine3"), Vector3.zero, Vector3.zero);
			SetFlag(Flags.Reserved1, b: true);
			((FacepunchBehaviour)this).Invoke((Action)ClearEquipping, 1.5f);
		}
	}

	public override int GetIdealSlot(BasePlayer player, Item item)
	{
		for (int i = 0; i < inventorySlots; i++)
		{
			RowType rowType = GetRowType(i);
			if (item.info.category == ItemCategory.Attire)
			{
				if (rowType != 0)
				{
					continue;
				}
			}
			else if (rowType != RowType.Belt)
			{
				continue;
			}
			if (base.inventory.SlotTaken(item, i) || (rowType == RowType.Clothing && DoesWearableConflictWithRow(item, i)))
			{
				continue;
			}
			return i;
		}
		return int.MinValue;
	}

	private bool DoesWearableConflictWithRow(Item item, int pos)
	{
		int num = pos / 13;
		int num2 = num * 13;
		ItemModWearable itemModWearable = item.info.ItemModWearable;
		if ((Object)(object)itemModWearable == (Object)null)
		{
			return false;
		}
		for (int i = num2; i < num2 + 7; i++)
		{
			Item slot = base.inventory.GetSlot(i);
			if (slot != null)
			{
				ItemModWearable itemModWearable2 = slot.info.ItemModWearable;
				if (!((Object)(object)itemModWearable2 == (Object)null) && !itemModWearable2.CanExistWith(itemModWearable))
				{
					return true;
				}
			}
		}
		return false;
	}

	public Vector2i GetIndustrialSlotRange(Vector3 localPosition)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		if (localPosition.x < -0.3f)
		{
			return new Vector2i(26, 38);
		}
		if (localPosition.x > 0.3f)
		{
			return new Vector2i(0, 12);
		}
		return new Vector2i(13, 25);
	}

	public override bool SupportsChildDeployables()
	{
		return true;
	}

	public override bool CanPickup(BasePlayer player)
	{
		return base.CanPickup(player) && !HasAttachedStorageAdaptor();
	}
}
