namespace Rust;

public static class DamageTypeEx
{
	public static bool IsMeleeType(this DamageType damageType)
	{
		return damageType == DamageType.Blunt || damageType == DamageType.Slash || damageType == DamageType.Stab;
	}

	public static bool IsBleedCausing(this DamageType damageType)
	{
		return damageType == DamageType.Bite || damageType == DamageType.Slash || damageType == DamageType.Stab || damageType == DamageType.Bullet || damageType == DamageType.Arrow;
	}

	public static bool IsConsideredAnAttack(this DamageType damageType)
	{
		return damageType != DamageType.Decay && damageType != DamageType.Collision;
	}
}
