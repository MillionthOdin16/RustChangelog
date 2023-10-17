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

	private float nextMissileTime = 0f;

	public float blockTurningFor = 0f;

	private bool isRetiring = false;

	private CH47PathFinder pathFinder = new CH47PathFinder();

	private float turnSeconds = 0f;

	protected override float PositionTickRate => 0.05f;

	protected override bool PositionTickFixedTime => true;

	public override float GetNetworkTime()
	{
		return Time.fixedTime;
	}

	public float GetDesiredAltitude()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = ((Component)this).transform.position + ((Component)this).transform.forward * 200f;
		float height = TerrainMeta.HeightMap.GetHeight(((Component)this).transform.position);
		height += TerrainMeta.HeightMap.GetHeight(val);
		height += TerrainMeta.HeightMap.GetHeight(val + Vector3.right * 50f);
		height += TerrainMeta.HeightMap.GetHeight(val - Vector3.right * 50f);
		height += TerrainMeta.HeightMap.GetHeight(val + Vector3.forward * 50f);
		height += TerrainMeta.HeightMap.GetHeight(val - Vector3.forward * 50f);
		return height / 6f + defaultAltitude;
	}

	public override void ServerInit()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		base.ServerInit();
		((FacepunchBehaviour)this).Invoke((Action)RetireToSunset, 600f);
		movePosition = ((Component)this).transform.position;
		movePosition.y = defaultAltitude;
		((Component)this).transform.position = movePosition;
	}

	public void RetireToSunset()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		isRetiring = true;
		movePosition = new Vector3(10000f, defaultAltitude, 10000f);
	}

	public void PickNewPatrolPoint()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_027c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
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
			bool flag = blockTurningFor > 0f;
			movePosition.y = altitude;
			Vector3 val = Vector3Ex.Direction(movePosition, ((Component)this).transform.position);
			if (flag)
			{
				Vector3 position = ((Component)this).transform.position;
				position.y = altitude;
				Vector3 val2 = QuaternionEx.LookRotationForcedUp(((Component)this).transform.forward, Vector3.up) * Vector3.forward;
				val = Vector3Ex.Direction(position + val2 * 2000f, ((Component)this).transform.position);
			}
			Vector3 forward = Vector3.Lerp(((Component)this).transform.forward, val, Time.fixedDeltaTime * turnRate);
			((Component)this).transform.forward = forward;
			bool flag2 = Vector3.Dot(((Component)this).transform.right, val) > 0.55f;
			bool flag3 = Vector3.Dot(-((Component)this).transform.right, val) > 0.55f;
			SetFlag(Flags.Reserved1, flag2);
			SetFlag(Flags.Reserved2, flag3);
			if (flag3 || flag2)
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
