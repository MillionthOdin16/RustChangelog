public class MLRSServerProjectile : ServerProjectile
{
	public override bool HasRangeLimit => false;

	protected override int mask => 1235954449;

	protected override bool IsAValidHit(BaseEntity hitEnt)
	{
		if (!base.IsAValidHit(hitEnt))
		{
			return false;
		}
		return !hitEnt.IsValid() || !(hitEnt is MLRS);
	}
}
