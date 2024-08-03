using System;
using Facepunch.Rust;
using UnityEngine;

public class PlayerBelt
{
	public static int SelectedSlot = -1;

	protected BasePlayer player;

	public static int MaxBeltSlots => 6;

	public PlayerBelt(BasePlayer player)
	{
		this.player = player;
	}

	public void DropActive(Vector3 position, Vector3 velocity)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
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
