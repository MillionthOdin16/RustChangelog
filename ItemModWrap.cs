public class ItemModWrap : ItemMod
{
	public GameObjectRef successEffect;

	public ItemDefinition wrappedDefinition;

	public static Phrase wrap_gift = new Phrase("wrap_gift", "Wrap Gift");

	public static Phrase wrap_gift_desc = new Phrase("wrap_gift_desc", "Wrap this item and turn it in to an openable gift");

	public override void ServerCommand(Item item, string command, BasePlayer player)
	{
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		if (!(command == "wrap") || item.amount <= 0)
		{
			return;
		}
		Item slot = item.contents.GetSlot(0);
		if (slot != null)
		{
			int position = item.position;
			int iTargetPos = position;
			ItemContainer rootContainer = item.GetRootContainer();
			item.RemoveFromContainer();
			Item item2 = ItemManager.Create(wrappedDefinition, 1, 0uL);
			slot.MoveToContainer(item2.contents);
			item2.MoveToContainer(rootContainer, iTargetPos);
			item.Remove();
			if (successEffect.isValid)
			{
				Effect.server.Run(successEffect.resourcePath, player.eyes.position);
			}
		}
	}
}
