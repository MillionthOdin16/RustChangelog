using UnityEngine;

public class JunkPileWater : JunkPile
{
	public class JunkpileWaterWorkQueue : ObjectWorkQueue<JunkPileWater>
	{
		protected override void RunJob(JunkPileWater entity)
		{
			if (((ObjectWorkQueue<JunkPileWater>)this).ShouldAdd(entity))
			{
				entity.UpdateNearbyPlayers();
			}
		}

		protected override bool ShouldAdd(JunkPileWater entity)
		{
			return base.ShouldAdd(entity) && entity.IsValid();
		}
	}

	public static JunkpileWaterWorkQueue junkpileWaterWorkQueue = new JunkpileWaterWorkQueue();

	[ServerVar]
	[Help("How many milliseconds to budget for processing life story updates per frame")]
	public static float framebudgetms = 0.25f;

	public Transform[] buoyancyPoints;

	public bool debugDraw = false;

	public float updateCullRange = 16f;

	private Quaternion baseRotation = Quaternion.identity;

	private bool first = true;

	private TimeUntil nextPlayerCheck;

	private bool hasPlayersNearby = false;

	public override void Spawn()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		Vector3 position = ((Component)this).transform.position;
		position.y = TerrainMeta.WaterMap.GetHeight(((Component)this).transform.position);
		((Component)this).transform.position = position;
		base.Spawn();
		Quaternion rotation = ((Component)this).transform.rotation;
		baseRotation = Quaternion.Euler(0f, ((Quaternion)(ref rotation)).eulerAngles.y, 0f);
	}

	public void FixedUpdate()
	{
		if (!base.isClient)
		{
			UpdateMovement();
		}
	}

	public void UpdateMovement()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		if (TimeUntil.op_Implicit(nextPlayerCheck) <= 0f)
		{
			nextPlayerCheck = TimeUntil.op_Implicit(Random.Range(0.5f, 1f));
			((ObjectWorkQueue<JunkPileWater>)junkpileWaterWorkQueue).Add(this);
		}
		if (isSinking || !hasPlayersNearby)
		{
			return;
		}
		float height = WaterSystem.GetHeight(((Component)this).transform.position);
		((Component)this).transform.position = new Vector3(((Component)this).transform.position.x, height, ((Component)this).transform.position.z);
		if (buoyancyPoints != null && buoyancyPoints.Length >= 3)
		{
			Vector3 position = ((Component)this).transform.position;
			Vector3 localPosition = buoyancyPoints[0].localPosition;
			Vector3 localPosition2 = buoyancyPoints[1].localPosition;
			Vector3 localPosition3 = buoyancyPoints[2].localPosition;
			Vector3 val = localPosition + position;
			Vector3 val2 = localPosition2 + position;
			Vector3 val3 = localPosition3 + position;
			val.y = WaterSystem.GetHeight(val);
			val2.y = WaterSystem.GetHeight(val2);
			val3.y = WaterSystem.GetHeight(val3);
			Vector3 val4 = default(Vector3);
			((Vector3)(ref val4))._002Ector(position.x, val.y - localPosition.y, position.z);
			Vector3 val5 = val2 - val;
			Vector3 val6 = val3 - val;
			Vector3 val7 = Vector3.Cross(val6, val5);
			Quaternion val8 = Quaternion.LookRotation(new Vector3(val7.x, val7.z, val7.y));
			Vector3 eulerAngles = ((Quaternion)(ref val8)).eulerAngles;
			val8 = Quaternion.Euler(0f - eulerAngles.x, 0f, 0f - eulerAngles.y);
			if (first)
			{
				Quaternion rotation = ((Component)this).transform.rotation;
				baseRotation = Quaternion.Euler(0f, ((Quaternion)(ref rotation)).eulerAngles.y, 0f);
				first = false;
			}
			((Component)this).transform.SetPositionAndRotation(val4, val8 * baseRotation);
		}
	}

	public void UpdateNearbyPlayers()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		hasPlayersNearby = BaseNetworkable.HasCloseConnections(((Component)this).transform.position, updateCullRange);
	}
}
