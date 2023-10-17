using UnityEngine;

public class SocketMod_InWater : SocketMod
{
	public bool wantsInWater = true;

	public static Phrase WantsWaterPhrase = new Phrase("error_inwater_wants", "Must be placed in water");

	public static Phrase NoWaterPhrase = new Phrase("error_inwater", "Can't be placed in water");

	private void OnDrawGizmosSelected()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		Gizmos.matrix = ((Component)this).transform.localToWorldMatrix;
		Gizmos.color = Color.cyan;
		Gizmos.DrawSphere(Vector3.zero, 0.1f);
	}

	public override bool DoCheck(Construction.Placement place)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		if (WaterLevel.Test(place.position + place.rotation * worldPosition - new Vector3(0f, 0.1f, 0f), waves: true, volumes: true) == wantsInWater)
		{
			return true;
		}
		if (wantsInWater)
		{
			Construction.lastPlacementError = WantsWaterPhrase.translated;
		}
		else
		{
			Construction.lastPlacementError = NoWaterPhrase.translated;
		}
		return false;
	}
}
