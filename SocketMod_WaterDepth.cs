public class SocketMod_WaterDepth : SocketMod
{
	public float MinimumWaterDepth = 2f;

	public float MaximumWaterDepth = 4f;

	public bool AllowWaterVolumes;

	public static Phrase TooDeepPhrase = new Phrase("error_toodeep", "Water is too deep");

	public static Phrase TooShallowPhrase = new Phrase("error_shallow", "Water is too shallow");

	public override bool DoCheck(Construction.Placement place)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		WaterLevel.WaterInfo waterInfo = WaterLevel.GetWaterInfo(place.position + place.rotation * worldPosition, waves: false, AllowWaterVolumes, null, noEarlyExit: true);
		if (waterInfo.overallDepth > MinimumWaterDepth && waterInfo.overallDepth < MaximumWaterDepth)
		{
			return true;
		}
		if (waterInfo.overallDepth <= MinimumWaterDepth)
		{
			Construction.lastPlacementError = TooShallowPhrase.translated;
		}
		else
		{
			Construction.lastPlacementError = TooDeepPhrase.translated;
		}
		return false;
	}
}
