using UnityEngine;

public class ItemModBaitContainer : ItemModContainer
{
	protected override bool ForceAcceptItemCheck => true;

	protected override bool CanAcceptItem(Item item, int count)
	{
		ItemModCompostable component = ((Component)item.info).GetComponent<ItemModCompostable>();
		return (Object)(object)component != (Object)null && component.BaitValue > 0f;
	}

	protected override void SetAllowedItems(ItemContainer container)
	{
		FishLookup.LoadFish();
		container.SetOnlyAllowedItems(FishLookup.BaitItems);
	}
}
