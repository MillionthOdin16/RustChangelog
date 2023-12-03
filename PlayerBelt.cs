using System;
using Facepunch.Rust;
using UnityEngine;

public class PlayerBelt
{
	public static int ClientAutoSelectSlot = -1;

	public static uint ClientAutoSeletItemUID = 0u;

	public static int SelectedSlot = -1;

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
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
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
}
