public class ItemModAnimalEquipment : ItemMod
{
	public enum SlotType
	{
		Basic,
		Armor,
		Saddle,
		Bit,
		Feet,
		SaddleDouble
	}

	public BaseEntity.Flags WearableFlag;

	public bool hideHair = false;

	public ProtectionProperties animalProtection;

	public ProtectionProperties riderProtection;

	public int additionalInventorySlots;

	public float speedModifier;

	public float staminaUseModifier;

	public SlotType slot;
}
