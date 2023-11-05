using Facepunch;
using ProtoBuf;
using UnityEngine;

public class CargoPlane : BaseEntity
{
	public GameObjectRef prefabDrop;

	public SpawnFilter filter;

	private Vector3 startPos;

	private Vector3 endPos;

	private float secondsToTake;

	private float secondsTaken;

	private bool dropped;

	private Vector3 dropPosition = Vector3.zero;

	public override void ServerInit()
	{
		base.ServerInit();
		Initialize();
	}

	public override void PostServerLoad()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		base.PostServerLoad();
		if (dropPosition == Vector3.zero)
		{
			Initialize();
		}
	}

	private void Initialize()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if (dropPosition == Vector3.zero)
		{
			dropPosition = RandomDropPosition();
		}
		UpdateDropPosition(dropPosition);
	}

	public void InitDropPosition(Vector3 newDropPosition)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		dropPosition = newDropPosition;
		dropPosition.y = 0f;
	}

	public Vector3 RandomDropPosition()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		float x = TerrainMeta.Size.x;
		float y = TerrainMeta.HighestPoint.y + 250f;
		startPos = Vector3Ex.Range(-1f, 1f);
		startPos.y = 0f;
		((Vector3)(ref startPos)).Normalize();
		startPos *= x * 2f;
		startPos.y = y;
		endPos = startPos * -1f;
		endPos.y = startPos.y;
		startPos += newDropPosition;
		endPos += newDropPosition;
		secondsToTake = Vector3.Distance(startPos, endPos) / 50f;
		secondsToTake *= Random.Range(0.95f, 1.05f);
		((Component)this).transform.position = startPos;
		((Component)this).transform.rotation = Quaternion.LookRotation(endPos - startPos);
		dropPosition = newDropPosition;
	}

	private void Update()
	{
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		if (!base.isServer)
		{
			return;
		}
		secondsTaken += Time.deltaTime;
		float num = Mathf.InverseLerp(0f, secondsToTake, secondsTaken);
		if (!dropped && num >= 0.5f)
		{
			dropped = true;
			BaseEntity baseEntity = GameManager.server.CreateEntity(prefabDrop.resourcePath, ((Component)this).transform.position);
			if (Object.op_Implicit((Object)(object)baseEntity))
			{
				baseEntity.globalBroadcast = true;
				baseEntity.Spawn();
			}
		}
		((Component)this).transform.position = Vector3.Lerp(startPos, endPos, num);
		((Component)this).transform.hasChanged = true;
		if (num >= 1f)
		{
			Kill();
		}
	}

	public override void Save(SaveInfo info)
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		base.Save(info);
		if (base.isServer && info.forDisk)
		{
			info.msg.cargoPlane = Pool.Get<CargoPlane>();
			info.msg.cargoPlane.startPos = startPos;
			info.msg.cargoPlane.endPos = endPos;
			info.msg.cargoPlane.secondsToTake = secondsToTake;
			info.msg.cargoPlane.secondsTaken = secondsTaken;
			info.msg.cargoPlane.dropped = dropped;
			info.msg.cargoPlane.dropPosition = dropPosition;
		}
	}

	public override void Load(LoadInfo info)
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		base.Load(info);
		if (base.isServer && info.fromDisk && info.msg.cargoPlane != null)
		{
			startPos = info.msg.cargoPlane.startPos;
			endPos = info.msg.cargoPlane.endPos;
			secondsToTake = info.msg.cargoPlane.secondsToTake;
			secondsTaken = info.msg.cargoPlane.secondsTaken;
			dropped = info.msg.cargoPlane.dropped;
			dropPosition = info.msg.cargoPlane.dropPosition;
		}
	}
}
