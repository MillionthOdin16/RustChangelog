using System;
using UnityEngine;

public class TravellingVendorEvent : TriggeredEvent
{
	public Phrase spawnPhrase;

	public static TravellingVendor currentVendor = null;

	public static float dontSpawnHoursBeforeWipe = 24f;

	public override void RunEvent()
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)currentVendor != (Object)null || (Object)(object)TerrainMeta.Path == (Object)null || TerrainMeta.Path.Roads.Count == 0 || !TravellingVendor.should_spawn || RoadBradleys.StaticBradleyCount > 0)
		{
			return;
		}
		TravellingVendor travellingVendor = TravellingVendor.SpawnTravellingVendorForEvent();
		if (Object.op_Implicit((Object)(object)travellingVendor))
		{
			currentVendor = travellingVendor;
			Enumerator<BasePlayer> enumerator = BasePlayer.activePlayerList.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					BasePlayer current = enumerator.Current;
					if (Object.op_Implicit((Object)(object)current) && current.IsConnected && !current.IsInTutorial)
					{
						current.ShowToast(GameTip.Styles.Server_Event, spawnPhrase);
					}
				}
				return;
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
		}
		Debug.Log((object)"Failed to spawn travelling vendor.");
	}

	private bool HoursCheck()
	{
		if (WipeTimer.serverinstance.GetTimeSpanUntilWipe().TotalHours > (double)dontSpawnHoursBeforeWipe)
		{
			return true;
		}
		return false;
	}
}
