using Facepunch.Rust;
using UnityEngine;

public class ItemModStudyBlueprint : ItemMod
{
	public GameObjectRef studyEffect;

	public override void ServerCommand(Item item, string command, BasePlayer player)
	{
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)item.GetOwnerPlayer() != (Object)(object)player)
		{
			bool flag = false;
			foreach (ItemContainer container in player.inventory.loot.containers)
			{
				if (item.GetRootContainer() == container)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				return;
			}
		}
		if (!(command == "study") || !item.IsBlueprint() || IsBlueprintUnlocked(item, player, out var blueprintTargetDef, out var blueprint))
		{
			return;
		}
		Item item2 = item;
		if (item.amount > 1)
		{
			item2 = item.SplitItem(1);
		}
		item2.UseItem();
		player.blueprints.Unlock(blueprintTargetDef);
		Analytics.Azure.OnBlueprintLearned(player, blueprintTargetDef, "blueprint", ResearchTable.ScrapForResearch(blueprintTargetDef), player);
		if ((Object)(object)blueprint != (Object)null && blueprint.additionalUnlocks != null && blueprint.additionalUnlocks.Count > 0)
		{
			foreach (ItemDefinition additionalUnlock in blueprint.additionalUnlocks)
			{
				player.blueprints.Unlock(additionalUnlock);
				Analytics.Azure.OnBlueprintLearned(player, additionalUnlock, "blueprint", 0, player);
			}
		}
		if (studyEffect.isValid)
		{
			Effect.server.Run(studyEffect.resourcePath, player, StringPool.Get("head"), Vector3.zero, Vector3.zero);
		}
	}

	private static bool IsBlueprintUnlocked(Item item, BasePlayer player, out ItemDefinition blueprintTargetDef, out ItemBlueprint blueprint)
	{
		blueprintTargetDef = item.blueprintTargetDef;
		blueprint = blueprintTargetDef.Blueprint;
		bool flag = IsBlueprintUnlocked(blueprintTargetDef, player);
		if (flag && (Object)(object)blueprint != (Object)null && blueprint.additionalUnlocks != null && blueprint.additionalUnlocks.Count > 0)
		{
			foreach (ItemDefinition additionalUnlock in blueprint.additionalUnlocks)
			{
				if (!IsBlueprintUnlocked(additionalUnlock, player))
				{
					flag = false;
				}
			}
		}
		if ((Object)(object)blueprint != (Object)null && blueprint.defaultBlueprint)
		{
			flag = true;
		}
		if (flag)
		{
			return true;
		}
		return false;
	}

	public static bool IsBlueprintUnlocked(ItemDefinition def, BasePlayer player)
	{
		return player.blueprints.IsUnlocked(def);
	}
}
