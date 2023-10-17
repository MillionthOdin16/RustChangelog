public class ItemModOpenWrapped : ItemMod
{
	public GameObjectRef successEffect;

	public static Phrase open_wrapped_gift = new Phrase("open_wrapped_gift", "Unwrap");

	public static Phrase open_wrapped_gift_desc = new Phrase("open_wrapped_gift_desc", "Unwrap the gift and reveal its contents");

	public override void ServerCommand(Item item, string command, BasePlayer player)
	{
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		if (!(command == "open") || item.amount <= 0)
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
			slot.MoveToContainer(rootContainer, iTargetPos);
			item.Remove();
			if (successEffect.isValid)
			{
				Effect.server.Run(successEffect.resourcePath, player.eyes.position);
			}
		}
	}
}
