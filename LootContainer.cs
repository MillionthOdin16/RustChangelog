using System;
using Facepunch.Rust;
using Rust;
using UnityEngine;

public class LootContainer : StorageContainer
{
	public enum spawnType
	{
		GENERIC,
		PLAYER,
		TOWN,
		AIRDROP,
		CRASHSITE,
		ROADSIDE
	}

	[Serializable]
	public struct LootSpawnSlot
	{
		public LootSpawn definition;

		public int numberToSpawn;

		public float probability;
	}

	public bool destroyOnEmpty = true;

	public LootSpawn lootDefinition;

	public int maxDefinitionsToSpawn;

	public float minSecondsBetweenRefresh = 3600f;

	public float maxSecondsBetweenRefresh = 7200f;

	public bool initialLootSpawn = true;

	public float xpLootedScale = 1f;

	public float xpDestroyedScale = 1f;

	public bool BlockPlayerItemInput = false;

	public int scrapAmount = 0;

	public string deathStat = "";

	public LootSpawnSlot[] LootSpawnSlots;

	public spawnType SpawnType;

	public bool FirstLooted;

	private static ItemDefinition scrapDef = null;

	public bool shouldRefreshContents => minSecondsBetweenRefresh > 0f && maxSecondsBetweenRefresh > 0f;

	public override void ResetState()
	{
		FirstLooted = false;
		base.ResetState();
	}

	public override void ServerInit()
	{
		base.ServerInit();
		if (initialLootSpawn)
		{
			SpawnLoot();
		}
		if (BlockPlayerItemInput && !Application.isLoadingSave && base.inventory != null)
		{
			base.inventory.SetFlag(ItemContainer.Flag.NoItemInput, b: true);
		}
		SetFlag(Flags.Reserved6, PlayerInventory.IsBirthday());
	}

	public override void PostServerLoad()
	{
		base.PostServerLoad();
		if (BlockPlayerItemInput && base.inventory != null)
		{
			base.inventory.SetFlag(ItemContainer.Flag.NoItemInput, b: true);
		}
	}

	public virtual void SpawnLoot()
	{
		if (base.inventory == null)
		{
			Debug.Log((object)"CONTACT DEVELOPERS! LootContainer::PopulateLoot has null inventory!!!");
			return;
		}
		base.inventory.Clear();
		ItemManager.DoRemoves();
		PopulateLoot();
		if (shouldRefreshContents)
		{
			((FacepunchBehaviour)this).Invoke((Action)SpawnLoot, Random.Range(minSecondsBetweenRefresh, maxSecondsBetweenRefresh));
		}
	}

	public int ScoreForRarity(Rarity rarity)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Expected I4, but got Unknown
		return (rarity - 1) switch
		{
			0 => 1, 
			1 => 2, 
			2 => 3, 
			3 => 4, 
			_ => 5000, 
		};
	}

	public virtual void PopulateLoot()
	{
		if (LootSpawnSlots.Length != 0)
		{
			LootSpawnSlot[] lootSpawnSlots = LootSpawnSlots;
			for (int i = 0; i < lootSpawnSlots.Length; i++)
			{
				LootSpawnSlot lootSpawnSlot = lootSpawnSlots[i];
				for (int j = 0; j < lootSpawnSlot.numberToSpawn; j++)
				{
					float num = Random.Range(0f, 1f);
					if (num <= lootSpawnSlot.probability)
					{
						lootSpawnSlot.definition.SpawnIntoContainer(base.inventory);
					}
				}
			}
		}
		else if ((Object)(object)lootDefinition != (Object)null)
		{
			for (int k = 0; k < maxDefinitionsToSpawn; k++)
			{
				lootDefinition.SpawnIntoContainer(base.inventory);
			}
		}
		if (SpawnType == spawnType.ROADSIDE || SpawnType == spawnType.TOWN)
		{
			foreach (Item item in base.inventory.itemList)
			{
				if (item.hasCondition)
				{
					item.condition = Random.Range(item.info.condition.foundCondition.fractionMin, item.info.condition.foundCondition.fractionMax) * item.info.condition.max;
				}
			}
		}
		GenerateScrap();
	}

	public void GenerateScrap()
	{
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		if (scrapAmount <= 0)
		{
			return;
		}
		if ((Object)(object)scrapDef == (Object)null)
		{
			scrapDef = ItemManager.FindItemDefinition("scrap");
		}
		int num = scrapAmount;
		if (num > 0)
		{
			Item item = ItemManager.Create(scrapDef, num, 0uL);
			if (!item.MoveToContainer(base.inventory))
			{
				item.Drop(((Component)this).transform.position, GetInheritedDropVelocity());
			}
		}
	}

	public override void DropBonusItems(BaseEntity initiator, ItemContainer container)
	{
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		base.DropBonusItems(initiator, container);
		if ((Object)(object)initiator == (Object)null || container == null)
		{
			return;
		}
		BasePlayer basePlayer = initiator as BasePlayer;
		if ((Object)(object)basePlayer == (Object)null || scrapAmount <= 0 || !((Object)(object)scrapDef != (Object)null))
		{
			return;
		}
		float num = (((Object)(object)basePlayer.modifiers != (Object)null) ? (1f + basePlayer.modifiers.GetValue(Modifier.ModifierType.Scrap_Yield)) : 0f);
		if (!(num > 1f))
		{
			return;
		}
		float variableValue = basePlayer.modifiers.GetVariableValue(Modifier.ModifierType.Scrap_Yield, 0f);
		float num2 = Mathf.Max((float)scrapAmount * num - (float)scrapAmount, 0f);
		variableValue += num2;
		int num3 = 0;
		if (variableValue >= 1f)
		{
			num3 = (int)variableValue;
			variableValue -= (float)num3;
		}
		basePlayer.modifiers.SetVariableValue(Modifier.ModifierType.Scrap_Yield, variableValue);
		if (num3 > 0)
		{
			Item item = ItemManager.Create(scrapDef, num3, 0uL);
			if (item != null)
			{
				DroppedItem droppedItem = item.Drop(GetDropPosition() + new Vector3(0f, 0.5f, 0f), GetInheritedDropVelocity()) as DroppedItem;
				droppedItem.DropReason = DroppedItem.DropReasonEnum.Loot;
			}
		}
	}

	public override bool OnStartBeingLooted(BasePlayer baseEntity)
	{
		if (!FirstLooted)
		{
			FirstLooted = true;
			Analytics.Azure.OnFirstLooted(this, baseEntity);
		}
		return base.OnStartBeingLooted(baseEntity);
	}

	public override void PlayerStoppedLooting(BasePlayer player)
	{
		base.PlayerStoppedLooting(player);
		if (destroyOnEmpty && (base.inventory == null || base.inventory.itemList == null || base.inventory.itemList.Count == 0))
		{
			Kill(DestroyMode.Gib);
		}
	}

	public void RemoveMe()
	{
		Kill(DestroyMode.Gib);
	}

	public override bool ShouldDropItemsIndividually()
	{
		return true;
	}

	public override void OnKilled(HitInfo info)
	{
		Analytics.Azure.OnLootContainerDestroyed(this, info.InitiatorPlayer, info.Weapon);
		base.OnKilled(info);
		if (info != null && (Object)(object)info.InitiatorPlayer != (Object)null && !string.IsNullOrEmpty(deathStat))
		{
			info.InitiatorPlayer.stats.Add(deathStat, 1, Stats.Life);
		}
	}

	public override void OnAttacked(HitInfo info)
	{
		base.OnAttacked(info);
	}

	public override void InitShared()
	{
		base.InitShared();
	}
}
