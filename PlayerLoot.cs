using System;
using System.Collections.Generic;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Profiling;

public class PlayerLoot : EntityComponent<BasePlayer>
{
	public BaseEntity entitySource = null;

	public Item itemSource = null;

	public List<ItemContainer> containers = new List<ItemContainer>();

	internal bool PositionChecks = true;

	private bool isInvokingSendUpdate;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("PlayerLoot.OnRpcMessage", 0);
		try
		{
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	public bool IsLooting()
	{
		return containers.Count > 0;
	}

	public void Clear()
	{
		if (!IsLooting())
		{
			return;
		}
		Profiler.BeginSample("PlayerLoot.Clear");
		MarkDirty();
		if (Object.op_Implicit((Object)(object)entitySource))
		{
			((Component)entitySource).SendMessage("PlayerStoppedLooting", (object)base.baseEntity, (SendMessageOptions)1);
		}
		foreach (ItemContainer container in containers)
		{
			if (container != null)
			{
				container.onDirty -= MarkDirty;
			}
		}
		containers.Clear();
		entitySource = null;
		itemSource = null;
		Profiler.EndSample();
	}

	public ItemContainer FindContainer(ItemContainerId id)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		Check();
		if (!IsLooting())
		{
			return null;
		}
		foreach (ItemContainer container in containers)
		{
			ItemContainer itemContainer = container.FindContainer(id);
			if (itemContainer != null)
			{
				return itemContainer;
			}
		}
		return null;
	}

	public Item FindItem(ItemId id)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		Check();
		if (!IsLooting())
		{
			return null;
		}
		foreach (ItemContainer container in containers)
		{
			Item item = container.FindItemByUID(id);
			if (item != null && item.IsValid())
			{
				return item;
			}
		}
		return null;
	}

	public void Check()
	{
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		if (!IsLooting() || !base.baseEntity.isServer)
		{
			return;
		}
		if ((Object)(object)entitySource == (Object)null)
		{
			base.baseEntity.ChatMessage("Stopping Looting because lootable doesn't exist!");
			Clear();
		}
		else if (!entitySource.CanBeLooted(base.baseEntity))
		{
			Clear();
		}
		else
		{
			if (!PositionChecks)
			{
				return;
			}
			float num = entitySource.Distance(base.baseEntity.eyes.position);
			if (num > 3f)
			{
				LootDistanceOverride component = ((Component)entitySource).GetComponent<LootDistanceOverride>();
				if ((Object)(object)component == (Object)null || num > component.amount)
				{
					Clear();
				}
			}
		}
	}

	private void MarkDirty()
	{
		Profiler.BeginSample("MarkDirty");
		if (!isInvokingSendUpdate)
		{
			isInvokingSendUpdate = true;
			((FacepunchBehaviour)this).Invoke((Action)SendUpdate, 0.1f);
		}
		Profiler.EndSample();
	}

	public void SendImmediate()
	{
		Profiler.BeginSample("SendImmediate");
		if (isInvokingSendUpdate)
		{
			isInvokingSendUpdate = false;
			((FacepunchBehaviour)this).CancelInvoke((Action)SendUpdate);
		}
		SendUpdate();
		Profiler.EndSample();
	}

	private void SendUpdate()
	{
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		isInvokingSendUpdate = false;
		if (!base.baseEntity.IsValid())
		{
			return;
		}
		Profiler.BeginSample("PlayerLoot.SendUpdate");
		PlayerUpdateLoot val = Pool.Get<PlayerUpdateLoot>();
		try
		{
			if (Object.op_Implicit((Object)(object)entitySource) && entitySource.net != null)
			{
				val.entityID = entitySource.net.ID;
			}
			if (itemSource != null)
			{
				val.itemID = itemSource.uid;
			}
			if (containers.Count > 0)
			{
				val.containers = Pool.Get<List<ItemContainer>>();
				foreach (ItemContainer container in containers)
				{
					val.containers.Add(container.Save());
				}
			}
			base.baseEntity.ClientRPCPlayer<PlayerUpdateLoot>(null, base.baseEntity, "UpdateLoot", val);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		Profiler.EndSample();
	}

	public bool StartLootingEntity(BaseEntity targetEntity, bool doPositionChecks = true)
	{
		Clear();
		if (!Object.op_Implicit((Object)(object)targetEntity))
		{
			return false;
		}
		if (!targetEntity.OnStartBeingLooted(base.baseEntity))
		{
			return false;
		}
		Assert.IsTrue(targetEntity.isServer, "Assure is server");
		Profiler.BeginSample("PlayerLoot.StartLootingEntity");
		PositionChecks = doPositionChecks;
		entitySource = targetEntity;
		itemSource = null;
		MarkDirty();
		Profiler.EndSample();
		if (targetEntity is ILootableEntity lootableEntity)
		{
			lootableEntity.LastLootedBy = base.baseEntity.userID;
		}
		return true;
	}

	public void AddContainer(ItemContainer container)
	{
		if (container != null)
		{
			Profiler.BeginSample("PlayerLoot.AddContainer");
			containers.Add(container);
			container.onDirty += MarkDirty;
			Profiler.EndSample();
		}
	}

	public void RemoveContainer(ItemContainer container)
	{
		if (container != null)
		{
			container.onDirty -= MarkDirty;
			containers.Remove(container);
		}
	}

	public bool RemoveContainerAt(int index)
	{
		if (index < 0 || index >= containers.Count)
		{
			return false;
		}
		if (containers[index] != null)
		{
			containers[index].onDirty -= MarkDirty;
		}
		containers.RemoveAt(index);
		return true;
	}

	public void StartLootingItem(Item item)
	{
		Clear();
		if (item != null && item.contents != null)
		{
			Profiler.BeginSample("PlayerLoot.StartLootingItem");
			PositionChecks = true;
			containers.Add(item.contents);
			item.contents.onDirty += MarkDirty;
			itemSource = item;
			entitySource = item.GetWorldEntity();
			MarkDirty();
			Profiler.EndSample();
		}
	}
}
