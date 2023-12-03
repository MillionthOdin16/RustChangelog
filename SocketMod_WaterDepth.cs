using UnityEngine;

public class SocketMod_WaterDepth : SocketMod
{
	public float MinimumWaterDepth = 2f;

	public float MaximumWaterDepth = 4f;

	public bool AllowWaterVolumes = false;

	public static Phrase TooDeepPhrase = new Phrase("error_toodeep", "Water is too deep");

	public static Phrase TooShallowPhrase = new Phrase("error_shallow", "Water is too shallow");

	public override bool DoCheck(Construction.Placement place)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		Vector3 pos = place.position + place.rotation * worldPosition;
		WaterLevel.WaterInfo waterInfo = WaterLevel.GetWaterInfo(pos, waves: false, AllowWaterVolumes, null, noEarlyExit: true);
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
