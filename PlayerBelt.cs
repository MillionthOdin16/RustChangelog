using System;
using System.Runtime.CompilerServices;
using Facepunch.Rust;
using UnityEngine;

public class PlayerBelt
{
	public struct EncryptedValue<TInner> where TInner : unmanaged
	{
		private TInner _value;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public TInner Get()
		{
			return _value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Set(TInner value)
		{
			_value = value;
		}

		public override string ToString()
		{
			return Get().ToString();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator EncryptedValue<TInner>(TInner value)
		{
			EncryptedValue<TInner> result = default(EncryptedValue<TInner>);
			result.Set(value);
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator TInner(EncryptedValue<TInner> encrypted)
		{
			return encrypted.Get();
		}
	}

	public static int ClientAutoSelectSlot = -1;

	public static uint ClientAutoSeletItemUID = 0u;

	public static EncryptedValue<int> SelectedSlot = -1;

	protected BasePlayer player;

	public static int MaxBeltSlots => 6;

	public PlayerBelt(BasePlayer player)
	{
		this.player = player;
	}

	public void DropActive(Vector3 position, Vector3 velocity)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		Item activeItem = player.GetActiveItem();
		if (activeItem == null)
		{
			return;
		}
		TimeWarning val = TimeWarning.New("PlayerBelt.DropActive", 0);
		try
		{
			DroppedItem droppedItem = activeItem.Drop(position, velocity) as DroppedItem;
			if ((Object)(object)droppedItem != (Object)null)
			{
				droppedItem.DropReason = DroppedItem.DropReasonEnum.Death;
				droppedItem.DroppedBy = player.userID;
				droppedItem.DroppedTime = DateTime.UtcNow;
				Analytics.Azure.OnItemDropped(player, droppedItem, DroppedItem.DropReasonEnum.Death);
			}
			player.svActiveItemID = default(ItemId);
			player.SendNetworkUpdate();
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public Item GetItemInSlot(int slot)
	{
		if ((Object)(object)player == (Object)null)
		{
			return null;
		}
		if ((Object)(object)player.inventory == (Object)null)
		{
			return null;
		}
		if (player.inventory.containerBelt == null)
		{
			return null;
		}
		return player.inventory.containerBelt.GetSlot(slot);
	}

	public Handcuffs GetRestraintItem()
	{
		if ((Object)(object)player == (Object)null)
		{
			return null;
		}
		if ((Object)(object)player.inventory == (Object)null)
		{
			return null;
		}
		if (player.inventory.containerBelt == null)
		{
			return null;
		}
		foreach (Item item in player.inventory.containerBelt.itemList)
		{
			if (item != null)
			{
				Handcuffs handcuffs = item.GetHeldEntity() as Handcuffs;
				if (!((Object)(object)handcuffs == (Object)null) && handcuffs.Locked)
				{
					return handcuffs;
				}
			}
		}
		return null;
	}
}
