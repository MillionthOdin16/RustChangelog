using UnityEngine;

public class SocketMod_InWater : SocketMod
{
	public bool wantsInWater = true;

	public static Phrase WantsWaterPhrase = new Phrase("error_inwater_wants", "Must be placed in water");

	public static Phrase NoWaterPhrase = new Phrase("error_inwater", "Can't be placed in water");

	private void OnDrawGizmosSelected()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		Gizmos.matrix = ((Component)this).transform.localToWorldMatrix;
		Gizmos.color = Color.cyan;
		Gizmos.DrawSphere(Vector3.zero, 0.1f);
	}

	public override bool DoCheck(Construction.Placement place)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = place.position + place.rotation * worldPosition;
		bool flag = WaterLevel.Test(val - new Vector3(0f, 0.1f, 0f), waves: true, volumes: true);
		if (flag == wantsInWater)
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
