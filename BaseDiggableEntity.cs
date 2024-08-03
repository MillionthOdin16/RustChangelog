using System;
using System.Collections.Generic;
using System.Linq;
using Facepunch;
using Network;
using UnityEngine;

public class BaseDiggableEntity : BaseCombatEntity
{
	public bool RequiresShovel;

	public Vector3 DropOffset;

	public Vector3 DropVelocity;

	public int RequiredDigCount = 3;

	public bool DestroyOnDug = true;

	[Header("Loot")]
	public List<DiggableEntityLoot> LootLists = new List<DiggableEntityLoot>();

	protected int digsRemaining;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("BaseDiggableEntity.OnRpcMessage", 0);
		try
		{
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	public override void ServerInit()
	{
		base.ServerInit();
		digsRemaining = RequiredDigCount;
		_maxHealth = RequiredDigCount;
	}

	public override void Hurt(HitInfo info)
	{
		if (!((Object)(object)info.InitiatorPlayer == (Object)null) && (!RequiresShovel || (!((Object)(object)info.Weapon == (Object)null) && info.Weapon is Shovel)) && info.damageTypes.IsMeleeType())
		{
			Dig(info.InitiatorPlayer);
		}
	}

	public virtual void Dig(BasePlayer player)
	{
		if (digsRemaining == RequiredDigCount)
		{
			OnFirstDig(player);
		}
		ClientRPC(RpcTarget.NetworkGroup("RPC_OnDig"), RequiredDigCount - digsRemaining, RequiredDigCount);
		digsRemaining--;
		base.health = digsRemaining;
		SendNetworkUpdate();
		OnSingleDig(player);
		if (digsRemaining <= 0)
		{
			OnFullyDug(player);
			if (DestroyOnDug)
			{
				Kill();
			}
		}
	}

	public virtual void OnFirstDig(BasePlayer player)
	{
	}

	public virtual void OnSingleDig(BasePlayer player)
	{
	}

	public virtual void OnFullyDug(BasePlayer player)
	{
		SpawnItem();
	}

	public BaseEntity SpawnItem()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		DiggableEntityLoot.ItemEntry? item = GetItem(((Component)this).transform.position);
		if (!item.HasValue || !item.HasValue)
		{
			return null;
		}
		Item item2 = ItemManager.Create(item.Value.Item, Random.Range(item.Value.Min, item.Value.Max + 1), 0uL);
		DroppedItem droppedItem = null;
		if (item2 != null)
		{
			if (item2.hasCondition)
			{
				item2.condition = Random.Range(item2.info.condition.foundCondition.fractionMin, item2.info.condition.foundCondition.fractionMax) * item2.info.condition.max;
			}
			droppedItem = item2.Drop(((Component)this).transform.position + DropOffset, DropVelocity) as DroppedItem;
			if ((Object)(object)droppedItem != (Object)null)
			{
				droppedItem.NeverCombine = true;
				droppedItem.SetAngularVelocity(new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * 720f);
			}
		}
		return droppedItem;
	}

	private DiggableEntityLoot.ItemEntry? GetItem(Vector3 digWorldPos)
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		if (LootLists == null)
		{
			return null;
		}
		List<DiggableEntityLoot.ItemEntry> list = Pool.GetList<DiggableEntityLoot.ItemEntry>();
		list.Clear();
		foreach (DiggableEntityLoot lootList in LootLists)
		{
			if (lootList.VerifyLootListForWorldPosition(digWorldPos))
			{
				list.AddRange(lootList.Items);
			}
		}
		DiggableEntityLoot.ItemEntry? result = null;
		if (list.Count != 0)
		{
			int num = list.Sum((DiggableEntityLoot.ItemEntry x) => x.Weight);
			int num2 = Random.Range(0, num);
			for (int i = 0; i < list.Count; i++)
			{
				result = list[i];
				if (result.HasValue)
				{
					num -= result.Value.Weight;
					if (num2 >= num)
					{
						break;
					}
				}
			}
		}
		Pool.FreeList<DiggableEntityLoot.ItemEntry>(ref list);
		return result;
	}
}
