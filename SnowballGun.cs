using UnityEngine;

public class SnowballGun : BaseProjectile
{
	public ItemDefinition OverrideProjectile;

	protected override ItemDefinition PrimaryMagazineAmmo
	{
		get
		{
			if (!((Object)(object)OverrideProjectile != (Object)null))
			{
				return base.PrimaryMagazineAmmo;
			}
			return OverrideProjectile;
		}
	}

	protected override bool CanRefundAmmo => false;

	public override bool TryReloadMagazine(IAmmoContainer ammoSource, int desiredAmount = -1)
	{
		desiredAmount = 1;
		if (!TryReload(ammoSource, desiredAmount, CanRefundAmmo))
		{
			return false;
		}
		SetAmmoCount(primaryMagazine.capacity);
		primaryMagazine.ammoType = OverrideProjectile;
		SendNetworkUpdateImmediate();
		ItemManager.DoRemoves();
		BasePlayer ownerPlayer = GetOwnerPlayer();
		if ((Object)(object)ownerPlayer != (Object)null)
		{
			ownerPlayer.inventory.ServerUpdate(0f);
		}
		return true;
	}
}
