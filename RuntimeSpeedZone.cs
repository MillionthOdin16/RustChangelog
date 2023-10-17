public class RuntimeSpeedZone : IAIPathSpeedZone
{
	public OBB worldOBBBounds;

	public float maxVelocityPerSec = 5f;

	public float GetMaxSpeed()
	{
		return maxVelocityPerSec;
	}

	public OBB WorldSpaceBounds()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		return worldOBBBounds;
	}
}
