using System;
using Network;
using UnityEngine;

public class SantaSleigh : BaseEntity
{
	public GameObjectRef prefabDrop;

	public SpawnFilter filter = null;

	public Transform dropOrigin;

	[ServerVar]
	public static float altitudeAboveTerrain = 50f;

	[ServerVar]
	public static float desiredAltitude = 60f;

	public Light bigLight;

	public SoundPlayer hohoho;

	public float hohohospacing = 4f;

	public float hohoho_additional_spacing = 2f;

	private Vector3 startPos;

	private Vector3 endPos;

	private float secondsToTake;

	private float secondsTaken = 0f;

	private bool dropped = false;

	private Vector3 dropPosition = Vector3.zero;

	public Vector3 swimScale;

	public Vector3 swimSpeed;

	private float swimRandom = 0f;

	public float appliedSwimScale = 1f;

	public float appliedSwimRotation = 20f;

	private const string path = "assets/prefabs/misc/xmas/sleigh/santasleigh.prefab";

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("SantaSleigh.OnRpcMessage", 0);
		try
		{
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	public override float GetNetworkTime()
	{
		return Time.fixedTime;
	}

	public void InitDropPosition(Vector3 newDropPosition)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		dropPosition = newDropPosition;
		dropPosition.y = 0f;
	}

	public override void ServerInit()
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		base.ServerInit();
		if (dropPosition == Vector3.zero)
		{
			dropPosition = RandomDropPosition();
		}
		UpdateDropPosition(dropPosition);
		((FacepunchBehaviour)this).Invoke((Action)SendHoHoHo, 0f);
	}

	public void SendHoHoHo()
	{
		((FacepunchBehaviour)this).Invoke((Action)SendHoHoHo, hohohospacing + Random.Range(0f, hohoho_additional_spacing));
		ClientRPC(null, "ClientPlayHoHoHo");
	}

	public Vector3 RandomDropPosition()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		Vector3 zero = Vector3.zero;
		float num = 100f;
		float x = TerrainMeta.Size.x;
		do
		{
			zero = Vector3Ex.Range(0f - x / 3f, x / 3f);
		}
		while (filter.GetFactor(zero) == 0f && (num -= 1f) > 0f);
		zero.y = 0f;
		return zero;
	}

	public void UpdateDropPosition(Vector3 newDropPosition)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		float x = TerrainMeta.Size.x;
		float y = altitudeAboveTerrain;
		startPos = Vector3Ex.Range(-1f, 1f);
		startPos.y = 0f;
		((Vector3)(ref startPos)).Normalize();
		startPos *= x * 1.25f;
		startPos.y = y;
		endPos = startPos * -1f;
		endPos.y = startPos.y;
		startPos += newDropPosition;
		endPos += newDropPosition;
		secondsToTake = Vector3.Distance(startPos, endPos) / 25f;
		secondsToTake *= Random.Range(0.95f, 1.05f);
		((Component)this).transform.SetPositionAndRotation(startPos, Quaternion.LookRotation(endPos - startPos));
		dropPosition = newDropPosition;
	}

	private void FixedUpdate()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_02db: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		if (!base.isServer)
		{
			return;
		}
		Vector3 position = ((Component)this).transform.position;
		Quaternion rotation = ((Component)this).transform.rotation;
		secondsTaken += Time.deltaTime;
		float num = Mathf.InverseLerp(0f, secondsToTake, secondsTaken);
		if (!dropped && num >= 0.5f)
		{
			dropped = true;
			BaseEntity baseEntity = GameManager.server.CreateEntity(prefabDrop.resourcePath, ((Component)dropOrigin).transform.position);
			if (Object.op_Implicit((Object)(object)baseEntity))
			{
				baseEntity.globalBroadcast = true;
				baseEntity.Spawn();
			}
		}
		position = Vector3.Lerp(startPos, endPos, num);
		Vector3 val = endPos - startPos;
		Vector3 normalized = ((Vector3)(ref val)).normalized;
		Vector3 val2 = Vector3.zero;
		if (swimScale != Vector3.zero)
		{
			if (swimRandom == 0f)
			{
				swimRandom = Random.Range(0f, 20f);
			}
			float num2 = Time.time + swimRandom;
			((Vector3)(ref val2))._002Ector(Mathf.Sin(num2 * swimSpeed.x) * swimScale.x, Mathf.Cos(num2 * swimSpeed.y) * swimScale.y, Mathf.Sin(num2 * swimSpeed.z) * swimScale.z);
			val2 = ((Component)this).transform.InverseTransformDirection(val2);
			position += val2 * appliedSwimScale;
		}
		rotation = Quaternion.LookRotation(normalized) * Quaternion.Euler(Mathf.Cos(Time.time * swimSpeed.y) * appliedSwimRotation, 0f, Mathf.Sin(Time.time * swimSpeed.x) * appliedSwimRotation);
		Vector3 val3 = position;
		float height = TerrainMeta.HeightMap.GetHeight(val3 + ((Component)this).transform.forward * 30f);
		float height2 = TerrainMeta.HeightMap.GetHeight(val3);
		float num3 = Mathf.Max(height, height2);
		float num4 = Mathf.Max(desiredAltitude, num3 + altitudeAboveTerrain);
		val3.y = Mathf.Lerp(((Component)this).transform.position.y, num4, Time.fixedDeltaTime * 0.5f);
		position = val3;
		((Component)this).transform.hasChanged = true;
		if (num >= 1f)
		{
			Kill();
		}
		((Component)this).transform.SetPositionAndRotation(position, rotation);
	}

	[ServerVar]
	public static void drop(Arg arg)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		BasePlayer basePlayer = arg.Player();
		if (Object.op_Implicit((Object)(object)basePlayer))
		{
			Debug.Log((object)"Santa Inbound");
			BaseEntity baseEntity = GameManager.server.CreateEntity("assets/prefabs/misc/xmas/sleigh/santasleigh.prefab");
			if (Object.op_Implicit((Object)(object)baseEntity))
			{
				SantaSleigh component = ((Component)baseEntity).GetComponent<SantaSleigh>();
				component.InitDropPosition(((Component)basePlayer).transform.position + new Vector3(0f, 10f, 0f));
				baseEntity.Spawn();
			}
		}
	}
}
