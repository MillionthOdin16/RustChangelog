using System;
using Facepunch.Rust;
using Network;
using UnityEngine;

public class OreResourceEntity : StagedResourceEntity
{
	public GameObjectRef bonusPrefab;

	public GameObjectRef finishEffect;

	public GameObjectRef bonusFailEffect;

	public OreHotSpot _hotSpot;

	public SoundPlayer bonusSound;

	private int bonusesKilled;

	private int bonusesSpawned;

	private Vector3 lastNodeDir = Vector3.zero;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("OreResourceEntity.OnRpcMessage", 0);
		try
		{
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	protected override void UpdateNetworkStage()
	{
		int num = stage;
		base.UpdateNetworkStage();
		if (stage != num && Object.op_Implicit((Object)(object)_hotSpot))
		{
			DelayedBonusSpawn();
		}
	}

	public void CleanupBonus()
	{
		if (Object.op_Implicit((Object)(object)_hotSpot))
		{
			_hotSpot.Kill();
		}
		_hotSpot = null;
	}

	public override void DestroyShared()
	{
		base.DestroyShared();
		CleanupBonus();
	}

	public override void OnKilled(HitInfo info)
	{
		CleanupBonus();
		Analytics.Server.OreKilled(this, info);
		base.OnKilled(info);
	}

	public override void ServerInit()
	{
		base.ServerInit();
		((FacepunchBehaviour)this).Invoke((Action)InitialSpawnBonusSpot, 0f);
	}

	private void InitialSpawnBonusSpot()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		if (!base.IsDestroyed)
		{
			_hotSpot = SpawnBonusSpot(Vector3.zero);
		}
	}

	public void FinishBonusAssigned()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		Effect.server.Run(finishEffect.resourcePath, ((Component)this).transform.position, ((Component)this).transform.up);
	}

	public override void OnAttacked(HitInfo info)
	{
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		if (base.isClient)
		{
			base.OnAttacked(info);
			return;
		}
		if (!info.DidGather && info.gatherScale > 0f)
		{
			Jackhammer jackhammer = info.Weapon as Jackhammer;
			if (Object.op_Implicit((Object)(object)_hotSpot) || Object.op_Implicit((Object)(object)jackhammer))
			{
				if ((Object)(object)_hotSpot == (Object)null)
				{
					_hotSpot = SpawnBonusSpot(lastNodeDir);
				}
				if (Vector3.Distance(info.HitPositionWorld, ((Component)_hotSpot).transform.position) <= ((Component)_hotSpot).GetComponent<SphereCollider>().radius * 1.5f || (Object)(object)jackhammer != (Object)null)
				{
					float num = (((Object)(object)jackhammer == (Object)null) ? 1f : jackhammer.HotspotBonusScale);
					bonusesKilled++;
					info.gatherScale = 1f + Mathf.Clamp((float)bonusesKilled * 0.5f, 0f, 2f * num);
					_hotSpot.FireFinishEffect();
					ClientRPC<int, Vector3>(null, "PlayBonusLevelSound", bonusesKilled, ((Component)_hotSpot).transform.position);
				}
				else if (bonusesKilled > 0)
				{
					bonusesKilled = 0;
					Effect.server.Run(bonusFailEffect.resourcePath, ((Component)this).transform.position, ((Component)this).transform.up);
				}
				if (bonusesKilled > 0)
				{
					CleanupBonus();
				}
			}
		}
		if ((Object)(object)_hotSpot == (Object)null)
		{
			DelayedBonusSpawn();
		}
		base.OnAttacked(info);
	}

	public void DelayedBonusSpawn()
	{
		((FacepunchBehaviour)this).CancelInvoke((Action)RespawnBonus);
		((FacepunchBehaviour)this).Invoke((Action)RespawnBonus, 0.25f);
	}

	public void RespawnBonus()
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		CleanupBonus();
		_hotSpot = SpawnBonusSpot(lastNodeDir);
	}

	public OreHotSpot SpawnBonusSpot(Vector3 lastDirection)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		if (base.isClient)
		{
			return null;
		}
		if (!bonusPrefab.isValid)
		{
			return null;
		}
		Vector2 insideUnitCircle = Random.insideUnitCircle;
		_ = ((Vector2)(ref insideUnitCircle)).normalized;
		Vector3 zero = Vector3.zero;
		MeshCollider stageComponent = base.GetStageComponent<MeshCollider>();
		Transform transform = ((Component)this).transform;
		Bounds val = ((Collider)stageComponent).bounds;
		Vector3 val2 = transform.InverseTransformPoint(((Bounds)(ref val)).center);
		Vector3 val6;
		if (lastDirection == Vector3.zero)
		{
			Vector3 val3 = RandomCircle();
			lastNodeDir = ((Vector3)(ref val3)).normalized;
			Vector3 val4 = ((Component)this).transform.TransformDirection(((Vector3)(ref val3)).normalized);
			val3 = ((Component)this).transform.position + ((Component)this).transform.up * (val2.y + 0.5f) + ((Vector3)(ref val4)).normalized * 2.5f;
			zero = val3;
		}
		else
		{
			Vector3 val5 = Vector3.Cross(lastNodeDir, Vector3.up);
			float num = Random.Range(0.25f, 0.5f);
			float num2 = ((Random.Range(0, 2) == 0) ? (-1f) : 1f);
			val6 = lastNodeDir + val5 * num * num2;
			Vector3 val7 = (lastNodeDir = ((Vector3)(ref val6)).normalized);
			zero = ((Component)this).transform.position + ((Component)this).transform.TransformDirection(val7) * 2f;
			float num3 = Random.Range(1f, 1.5f);
			zero += ((Component)this).transform.up * (val2.y + num3);
		}
		bonusesSpawned++;
		val = ((Collider)stageComponent).bounds;
		val6 = ((Bounds)(ref val)).center - zero;
		Vector3 normalized = ((Vector3)(ref val6)).normalized;
		RaycastHit val8 = default(RaycastHit);
		if (((Collider)stageComponent).Raycast(new Ray(zero, normalized), ref val8, 10f))
		{
			OreHotSpot obj = GameManager.server.CreateEntity(bonusPrefab.resourcePath, ((RaycastHit)(ref val8)).point - normalized * 0.025f, Quaternion.LookRotation(((RaycastHit)(ref val8)).normal, Vector3.up)) as OreHotSpot;
			obj.Spawn();
			((Component)obj).SendMessage("OreOwner", (object)this);
			return obj;
		}
		return null;
	}

	public Vector3 RandomCircle(float distance = 1f, bool allowInside = false)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val;
		if (!allowInside)
		{
			Vector2 insideUnitCircle = Random.insideUnitCircle;
			val = ((Vector2)(ref insideUnitCircle)).normalized;
		}
		else
		{
			val = Random.insideUnitCircle;
		}
		Vector2 val2 = val;
		return new Vector3(val2.x, 0f, val2.y);
	}

	public Vector3 RandomHemisphereDirection(Vector3 input, float degreesOffset, bool allowInside = true, bool changeHeight = true)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		degreesOffset = Mathf.Clamp(degreesOffset / 180f, -180f, 180f);
		Vector2 val;
		if (!allowInside)
		{
			Vector2 insideUnitCircle = Random.insideUnitCircle;
			val = ((Vector2)(ref insideUnitCircle)).normalized;
		}
		else
		{
			val = Random.insideUnitCircle;
		}
		Vector2 val2 = val;
		Vector3 val3 = default(Vector3);
		((Vector3)(ref val3))._002Ector(val2.x * degreesOffset, changeHeight ? (Random.Range(-1f, 1f) * degreesOffset) : 0f, val2.y * degreesOffset);
		Vector3 val4 = input + val3;
		return ((Vector3)(ref val4)).normalized;
	}

	public Vector3 ClampToHemisphere(Vector3 hemiInput, float degreesOffset, Vector3 inputVec)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		degreesOffset = Mathf.Clamp(degreesOffset / 180f, -180f, 180f);
		Vector3 val = hemiInput + Vector3.one * degreesOffset;
		Vector3 normalized = ((Vector3)(ref val)).normalized;
		val = hemiInput + Vector3.one * (0f - degreesOffset);
		Vector3 normalized2 = ((Vector3)(ref val)).normalized;
		for (int i = 0; i < 3; i++)
		{
			((Vector3)(ref inputVec))[i] = Mathf.Clamp(((Vector3)(ref inputVec))[i], ((Vector3)(ref normalized2))[i], ((Vector3)(ref normalized))[i]);
		}
		return inputVec;
	}

	public static Vector3 RandomCylinderPointAroundVector(Vector3 input, float distance, float minHeight = 0f, float maxHeight = 0f, bool allowInside = false)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val;
		if (!allowInside)
		{
			Vector2 insideUnitCircle = Random.insideUnitCircle;
			val = ((Vector2)(ref insideUnitCircle)).normalized;
		}
		else
		{
			val = Random.insideUnitCircle;
		}
		Vector2 val2 = val;
		Vector3 val3 = new Vector3(val2.x, 0f, val2.y);
		Vector3 result = ((Vector3)(ref val3)).normalized * distance;
		result.y = Random.Range(minHeight, maxHeight);
		return result;
	}

	public Vector3 ClampToCylinder(Vector3 localPos, Vector3 cylinderAxis, float cylinderDistance, float minHeight = 0f, float maxHeight = 0f)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		return Vector3.zero;
	}
}
