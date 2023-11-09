using Facepunch.Rust;
using UnityEngine;

public class ItemModRepair : ItemMod
{
	public float conditionLost = 0.05f;

	public GameObjectRef successEffect;

	public int workbenchLvlRequired;

	public bool canUseRepairBench;

	public bool HasCraftLevel(BasePlayer player = null)
	{
		if ((Object)(object)player != (Object)null && player.isServer)
		{
			return player.currentCraftLevel >= (float)workbenchLvlRequired;
		}
		return false;
	}

	public override void ServerCommand(Item item, string command, BasePlayer player)
	{
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		if (command == "refill" && !player.IsSwimming() && HasCraftLevel(player) && !(item.conditionNormalized >= 1f))
		{
			float conditionNormalized = item.conditionNormalized;
			float maxConditionNormalized = item.maxConditionNormalized;
			item.DoRepair(conditionLost);
			if (successEffect.isValid)
			{
				Effect.server.Run(successEffect.resourcePath, player.eyes.position);
			}
			Analytics.Azure.OnItemRepaired(player, player.GetCachedCraftLevelWorkbench(), item, conditionNormalized, maxConditionNormalized);
		}
	}
}
