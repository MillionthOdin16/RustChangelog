using UnityEngine;

public class ItemModUpgrade : ItemMod
{
	public int numForUpgrade = 10;

	public float upgradeSuccessChance = 1f;

	public int numToLoseOnFail = 2;

	public ItemDefinition upgradedItem;

	public int numUpgradedItem = 1;

	public GameObjectRef successEffect;

	public GameObjectRef failEffect;

	public override void ServerCommand(Item item, string command, BasePlayer player)
	{
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		if (!(command == "upgrade_item") || item.amount < numForUpgrade)
		{
			return;
		}
		if (Random.Range(0f, 1f) <= upgradeSuccessChance)
		{
			item.UseItem(numForUpgrade);
			Item item2 = ItemManager.Create(upgradedItem, numUpgradedItem, 0uL);
			if (!item2.MoveToContainer(player.inventory.containerMain))
			{
				item2.Drop(player.GetDropPosition(), player.GetDropVelocity());
			}
			if (successEffect.isValid)
			{
				Effect.server.Run(successEffect.resourcePath, player.eyes.position);
			}
		}
		else
		{
			item.UseItem(numToLoseOnFail);
			if (failEffect.isValid)
			{
				Effect.server.Run(failEffect.resourcePath, player.eyes.position);
			}
		}
	}
}
