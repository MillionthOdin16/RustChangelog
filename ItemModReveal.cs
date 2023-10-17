using UnityEngine;
using UnityEngine.Profiling;

public class ItemModReveal : ItemMod
{
	public int numForReveal = 10;

	public ItemDefinition revealedItemOverride;

	public int revealedItemAmount = 1;

	public LootSpawn revealList;

	public GameObjectRef successEffect;

	public override void ServerCommand(Item item, string command, BasePlayer player)
	{
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		if (command == "reveal" && item.amount >= numForReveal)
		{
			int position = item.position;
			item.UseItem(numForReveal);
			Item item2 = null;
			if (Object.op_Implicit((Object)(object)revealedItemOverride))
			{
				item2 = ItemManager.Create(revealedItemOverride, revealedItemAmount, 0uL);
			}
			else
			{
				Profiler.BeginSample("BlueprintAttempts");
				Profiler.EndSample();
			}
			if (item2 != null && !item2.MoveToContainer(player.inventory.containerMain, (item.amount == 0) ? position : (-1)))
			{
				item2.Drop(player.GetDropPosition(), player.GetDropVelocity());
			}
			if (successEffect.isValid)
			{
				Effect.server.Run(successEffect.resourcePath, player.eyes.position);
			}
		}
	}
}
