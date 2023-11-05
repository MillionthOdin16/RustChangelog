using UnityEngine;

public class Chicken : BaseAnimalNPC
{
	[ServerVar(Help = "Population active on the server, per square km", ShowInAdminUI = true)]
	public static float Population = 3f;

	public override float RealisticMass => 3f;

	public override TraitFlag Traits => TraitFlag.Alive | TraitFlag.Animal | TraitFlag.Food | TraitFlag.Meat;

	public override bool WantsToEat(BaseEntity best)
	{
		if (best.HasTrait(TraitFlag.Alive))
		{
			return false;
		}
		if (best.HasTrait(TraitFlag.Meat))
		{
			return false;
		}
		CollectibleEntity collectibleEntity = best as CollectibleEntity;
		if ((Object)(object)collectibleEntity != (Object)null)
		{
			ItemAmount[] itemList = collectibleEntity.itemList;
			foreach (ItemAmount itemAmount in itemList)
			{
				if (itemAmount.itemDef.category == ItemCategory.Food)
				{
					return true;
				}
			}
		}
		return base.WantsToEat(best);
	}

	public override string Categorize()
	{
		return "Chicken";
	}
}
