using UnityEngine;

public class ItemModConditionInWater : ItemMod
{
	public bool requiredState = false;

	public override bool Passes(Item item)
	{
		BasePlayer ownerPlayer = item.GetOwnerPlayer();
		if ((Object)(object)ownerPlayer == (Object)null)
		{
			return false;
		}
		bool flag = ownerPlayer.IsHeadUnderwater();
		return flag == requiredState;
	}
}
