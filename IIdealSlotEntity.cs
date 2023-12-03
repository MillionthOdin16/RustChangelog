public interface IIdealSlotEntity
{
	int GetIdealSlot(BasePlayer player, Item item);

	ItemContainerId GetIdealContainer(BasePlayer player, Item item, ItemMoveModifier modifiers);
}
