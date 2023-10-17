using System;
using ConVar;
using Facepunch;
using Facepunch.Rust;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

public class ResearchTable : StorageContainer
{
	public enum ResearchType
	{
		ResearchTable,
		TechTree
	}

	[NonSerialized]
	public float researchFinishedTime;

	public float researchCostFraction = 1f;

	public float researchDuration = 10f;

	public int requiredPaper = 10;

	public GameObjectRef researchStartEffect;

	public GameObjectRef researchFailEffect;

	public GameObjectRef researchSuccessEffect;

	public ItemDefinition researchResource;

	private BasePlayer user;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("ResearchTable.OnRpcMessage", 0);
		try
		{
			if (rpc == 3177710095u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - DoResearch "));
				}
				TimeWarning val2 = TimeWarning.New("DoResearch", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(3177710095u, "DoResearch", this, player, 3f))
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
							DoResearch(msg2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in DoResearch");
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

	public override void ResetState()
	{
		base.ResetState();
		researchFinishedTime = 0f;
	}

	public override int GetIdealSlot(BasePlayer player, Item item)
	{
		if (item.info.shortname == "scrap")
		{
			Item slot = base.inventory.GetSlot(1);
			if (slot == null)
			{
				return 1;
			}
			if (slot.amount < item.MaxStackable())
			{
				return 1;
			}
		}
		return base.GetIdealSlot(player, item);
	}

	public bool IsResearching()
	{
		return HasFlag(Flags.On);
	}

	public int RarityMultiplier(Rarity rarity)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Invalid comparison between Unknown and I4
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Invalid comparison between Unknown and I4
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Invalid comparison between Unknown and I4
		if ((int)rarity == 1)
		{
			return 20;
		}
		if ((int)rarity == 2)
		{
			return 15;
		}
		if ((int)rarity == 3)
		{
			return 10;
		}
		return 5;
	}

	public int GetBlueprintStacksize(Item sourceItem)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		int result = RarityMultiplier(sourceItem.info.rarity);
		if (sourceItem.info.category == ItemCategory.Ammunition)
		{
			result = Mathf.FloorToInt((float)sourceItem.MaxStackable() / (float)sourceItem.info.Blueprint.amountToCreate) * 2;
		}
		return result;
	}

	public static int ScrapForResearch(Item item)
	{
		return ScrapForResearch(item.info);
	}

	public static int ScrapForResearch(ItemDefinition info)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Invalid comparison between Unknown and I4
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Invalid comparison between Unknown and I4
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Invalid comparison between Unknown and I4
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Invalid comparison between Unknown and I4
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)info.isRedirectOf != (Object)null)
		{
			return ScrapForResearch(info.isRedirectOf);
		}
		int result = 0;
		if ((int)info.rarity == 1)
		{
			result = 20;
		}
		if ((int)info.rarity == 2)
		{
			result = 75;
		}
		if ((int)info.rarity == 3)
		{
			result = 125;
		}
		if ((int)info.rarity == 4 || (int)info.rarity == 0)
		{
			result = 500;
		}
		ItemBlueprint itemBlueprint = ItemManager.FindBlueprint(info);
		if ((Object)(object)itemBlueprint != (Object)null && itemBlueprint.defaultBlueprint)
		{
			return ConVar.Server.defaultBlueprintResearchCost;
		}
		return result;
	}

	public static int ScrapForResearch(ItemDefinition info, ResearchType type)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Invalid comparison between Unknown and I4
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Invalid comparison between Unknown and I4
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Invalid comparison between Unknown and I4
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Invalid comparison between Unknown and I4
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		if ((int)info.rarity == 1)
		{
			num = 20;
		}
		if ((int)info.rarity == 2)
		{
			num = 75;
		}
		if ((int)info.rarity == 3)
		{
			num = 125;
		}
		if ((int)info.rarity == 4 || (int)info.rarity == 0)
		{
			num = 500;
		}
		BaseGameMode activeGameMode = BaseGameMode.GetActiveGameMode(serverside: true);
		if ((Object)(object)activeGameMode != (Object)null)
		{
			BaseGameMode.ResearchCostResult scrapCostForResearch = activeGameMode.GetScrapCostForResearch(info, type);
			if (scrapCostForResearch.Scale.HasValue)
			{
				num = Mathf.RoundToInt((float)num * scrapCostForResearch.Scale.Value);
			}
			else if (scrapCostForResearch.Amount.HasValue)
			{
				num = scrapCostForResearch.Amount.Value;
			}
		}
		return num;
	}

	public bool IsItemResearchable(Item item)
	{
		ItemBlueprint itemBlueprint = ItemManager.FindBlueprint(((Object)(object)item.info.isRedirectOf != (Object)null) ? item.info.isRedirectOf : item.info);
		if ((Object)(object)itemBlueprint != (Object)null && itemBlueprint.defaultBlueprint)
		{
			return true;
		}
		if ((Object)(object)itemBlueprint == (Object)null || !itemBlueprint.isResearchable)
		{
			return false;
		}
		return true;
	}

	public override void ServerInit()
	{
		base.ServerInit();
		base.inventory.canAcceptItem = ItemFilter;
	}

	public override bool ItemFilter(Item item, int targetSlot)
	{
		if (targetSlot == 1 && (Object)(object)item.info != (Object)(object)researchResource)
		{
			return false;
		}
		return base.ItemFilter(item, targetSlot);
	}

	public Item GetTargetItem()
	{
		return base.inventory.GetSlot(0);
	}

	public Item GetScrapItem()
	{
		Item slot = base.inventory.GetSlot(1);
		if (slot == null || (Object)(object)slot.info != (Object)(object)researchResource)
		{
			return null;
		}
		return slot;
	}

	public override void PostServerLoad()
	{
		base.PostServerLoad();
		if (HasFlag(Flags.On))
		{
			((FacepunchBehaviour)this).Invoke((Action)ResearchAttemptFinished, researchDuration);
		}
		base.inventory.SetLocked(isLocked: false);
	}

	public override bool PlayerOpenLoot(BasePlayer player, string panelToOpen = "", bool doPositionChecks = true)
	{
		user = player;
		return base.PlayerOpenLoot(player);
	}

	public override void PlayerStoppedLooting(BasePlayer player)
	{
		user = null;
		base.PlayerStoppedLooting(player);
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	public void DoResearch(RPCMessage msg)
	{
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		if (IsResearching())
		{
			return;
		}
		BasePlayer player = msg.player;
		Item targetItem = GetTargetItem();
		if (targetItem != null && targetItem.amount <= 1 && IsItemResearchable(targetItem))
		{
			targetItem.CollectedForCrafting(player);
			researchFinishedTime = Time.realtimeSinceStartup + researchDuration;
			((FacepunchBehaviour)this).Invoke((Action)ResearchAttemptFinished, researchDuration);
			base.inventory.SetLocked(isLocked: true);
			int scrapCost = ScrapForResearch(targetItem);
			Analytics.Azure.OnResearchStarted(player, this, targetItem, scrapCost);
			SetFlag(Flags.On, b: true);
			SendNetworkUpdate();
			player.inventory.loot.SendImmediate();
			if (researchStartEffect.isValid)
			{
				Effect.server.Run(researchStartEffect.resourcePath, this, 0u, Vector3.zero, Vector3.zero);
			}
			msg.player.GiveAchievement("RESEARCH_ITEM");
		}
	}

	public void ResearchAttemptFinished()
	{
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		Item targetItem = GetTargetItem();
		Item scrapItem = GetScrapItem();
		if (targetItem != null && scrapItem != null)
		{
			int num = ScrapForResearch(targetItem);
			if (scrapItem.amount >= num)
			{
				if (scrapItem.amount == num)
				{
					base.inventory.Remove(scrapItem);
					scrapItem.RemoveFromContainer();
					scrapItem.Remove();
				}
				else
				{
					scrapItem.UseItem(num);
				}
				base.inventory.Remove(targetItem);
				targetItem.Remove();
				Item item = ItemManager.Create(ItemManager.blueprintBaseDef, 1, 0uL);
				item.blueprintTarget = (((Object)(object)targetItem.info.isRedirectOf != (Object)null) ? targetItem.info.isRedirectOf.itemid : targetItem.info.itemid);
				if (!item.MoveToContainer(base.inventory, 0))
				{
					item.Drop(GetDropPosition(), GetDropVelocity());
				}
				if (researchSuccessEffect.isValid)
				{
					Effect.server.Run(researchSuccessEffect.resourcePath, this, 0u, Vector3.zero, Vector3.zero);
				}
			}
		}
		SendNetworkUpdateImmediate();
		if ((Object)(object)user != (Object)null)
		{
			user.inventory.loot.SendImmediate();
		}
		EndResearch();
	}

	public void CancelResearch()
	{
	}

	public void EndResearch()
	{
		base.inventory.SetLocked(isLocked: false);
		SetFlag(Flags.On, b: false);
		researchFinishedTime = 0f;
		SendNetworkUpdate();
		if ((Object)(object)user != (Object)null)
		{
			user.inventory.loot.SendImmediate();
		}
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		info.msg.researchTable = Pool.Get<ResearchTable>();
		info.msg.researchTable.researchTimeLeft = researchFinishedTime - Time.realtimeSinceStartup;
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (info.msg.researchTable != null)
		{
			researchFinishedTime = Time.realtimeSinceStartup + info.msg.researchTable.researchTimeLeft;
		}
	}
}
