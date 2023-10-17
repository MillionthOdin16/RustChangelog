using UnityEngine;

public class ItemModConditionHasCondition : ItemMod
{
	public float conditionTarget = 1f;

	[Tooltip("If set to above 0 will check for fraction instead of raw value")]
	public float conditionFractionTarget = -1f;

	public bool lessThan = false;

	public override bool Passes(Item item)
	{
		if (!item.hasCondition)
		{
			return false;
		}
		if (conditionFractionTarget > 0f)
		{
			return (!lessThan && item.conditionNormalized > conditionFractionTarget) || (lessThan && item.conditionNormalized < conditionFractionTarget);
		}
		return (!lessThan && item.condition >= conditionTarget) || (lessThan && item.condition < conditionTarget);
	}
}
