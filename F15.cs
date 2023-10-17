using System;
using UnityEngine;

public class F15 : BaseCombatEntity
{
	public float speed = 150f;

	public float defaultAltitude = 150f;

	public float altitude = 250f;

	public float altitudeLerpSpeed = 30f;

	public float turnRate = 1f;

	public float flybySoundLengthUntilMax = 4.5f;

	public SoundPlayer flybySound;

	public GameObject body;

	public float rollSpeed = 1f;

	protected Vector3 movePosition;

	public GameObjectRef missilePrefab;

	private float nextMissileTime;

	public float blockTurningFor;

	private bool isRetiring;

	private CH47PathFinder pathFinder = new CH47PathFinder();

	private float turnSeconds;

	protected override float PositionTickRate => 0.05f;

	protected override bool PositionTickFixedTime => true;

	public override float GetNetworkTime()
	{
		return Time.fixedTime;
	}

	public float GetDesiredAltitude()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = ((Component)this).transform.position + ((Component)this).transform.forward * 200f;
		return (TerrainMeta.HeightMap.GetHeight(((Component)this).transform.position) + TerrainMeta.HeightMap.GetHeight(val) + TerrainMeta.HeightMap.GetHeight(val + Vector3.right * 50f) + TerrainMeta.HeightMap.GetHeight(val - Vector3.right * 50f) + TerrainMeta.HeightMap.GetHeight(val + Vector3.forward * 50f) + TerrainMeta.HeightMap.GetHeight(val - Vector3.forward * 50f)) / 6f + defaultAltitude;
	}

	public override void ServerInit()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		base.ServerInit();
		((FacepunchBehaviour)this).Invoke((Action)RetireToSunset, 600f);
		movePosition = ((Component)this).transform.position;
		movePosition.y = defaultAltitude;
		((Component)this).transform.position = movePosition;
	}

	public void RetireToSunset()
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		isRetiring = true;
		movePosition = new Vector3(10000f, defaultAltitude, 10000f);
	}

	public void PickNewPatrolPoint()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		movePosition = pathFinder.GetRandomPatrolPoint();
		float num = 0f;
		if ((Object)(object)TerrainMeta.HeightMap != (Object)null)
		{
			num = TerrainMeta.HeightMap.GetHeight(movePosition);
		}
		movePosition.y = num + defaultAltitude;
	}

	private void FixedUpdate()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		if (base.isClient)
		{
			return;
		}
		if (isRetiring && Vector3.Distance(((Component)this).transform.position, Vector3.zero) > 4900f)
		{
			((FacepunchBehaviour)this).Invoke((Action)DelayedDestroy, 0f);
		}
		if (!((FacepunchBehaviour)this).IsInvoking((Action)DelayedDestroy))
		{
			altitude = Mathf.Lerp(altitude, GetDesiredAltitude(), Time.fixedDeltaTime * 0.25f);
			if (Vector3Ex.Distance2D(movePosition, ((Component)this).transform.position) < 10f)
			{
				PickNewPatrolPoint();
				blockTurningFor = 6f;
			}
			blockTurningFor -= Time.fixedDeltaTime;
			bool num = blockTurningFor > 0f;
			movePosition.y = altitude;
			Vector3 val = Vector3Ex.Direction(movePosition, ((Component)this).transform.position);
			if (num)
			{
				Vector3 position = ((Component)this).transform.position;
				position.y = altitude;
				Vector3 val2 = QuaternionEx.LookRotationForcedUp(((Component)this).transform.forward, Vector3.up) * Vector3.forward;
				val = Vector3Ex.Direction(position + val2 * 2000f, ((Component)this).transform.position);
			}
			Vector3 forward = Vector3.Lerp(((Component)this).transform.forward, val, Time.fixedDeltaTime * turnRate);
			((Component)this).transform.forward = forward;
			bool flag = Vector3.Dot(((Component)this).transform.right, val) > 0.55f;
			bool flag2 = Vector3.Dot(-((Component)this).transform.right, val) > 0.55f;
			SetFlag(Flags.Reserved1, flag);
			SetFlag(Flags.Reserved2, flag2);
			if (flag2 || flag)
			{
				turnSeconds += Time.fixedDeltaTime;
			}
			else
			{
				turnSeconds = 0f;
			}
			if (turnSeconds > 10f)
			{
				turnSeconds = 0f;
				blockTurningFor = 8f;
			}
			Transform transform = ((Component)this).transform;
			transform.position += ((Component)this).transform.forward * speed * Time.fixedDeltaTime;
			nextMissileTime = Time.realtimeSinceStartup + 10f;
		}
	}

	public void DelayedDestroy()
	{
		Kill();
	}
}
