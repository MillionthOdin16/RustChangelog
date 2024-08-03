public class TriggerParentElevator : TriggerParentEnclosed
{
	public bool AllowHorsesToBypassClippingChecks = true;

	public bool AllowBikesToBypassClippingChecks = true;

	protected override bool IsClipping(BaseEntity ent)
	{
		if (AllowHorsesToBypassClippingChecks && ent is BaseRidableAnimal)
		{
			return false;
		}
		if ((AllowBikesToBypassClippingChecks && ent is Bike) || ent is Snowmobile)
		{
			return false;
		}
		return base.IsClipping(ent);
	}
}
