using System;
using Facepunch.Rust;
using Network;
using UnityEngine;
using UnityEngine.Profiling;

public class OreResourceEntity : StagedResourceEntity
{
	public GameObjectRef bonusPrefab;

	public GameObjectRef finishEffect;

	public GameObjectRef bonusFailEffect;

	public OreHotSpot _hotSpot;

	public SoundPlayer bonusSound;

	private int bonusesKilled = 0;

	private int bonusesSpawned = 0;

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
		int num2 = stage;
		if (num2 != num && Object.op_Implicit((Object)(object)_hotSpot))
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
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		if (!base.IsDestroyed)
		{
			_hotSpot = SpawnBonusSpot(Vector3.zero);
		}
	}

	public void FinishBonusAssigned()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		Effect.server.Run(finishEffect.resourcePath, ((Component)this).transform.position, ((Component)this).transform.up);
	}

	public override void OnAttacked(HitInfo info)
	{
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
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
				float num = Vector3.Distance(info.HitPositionWorld, ((Component)_hotSpot).transform.position);
				if (num <= ((Component)_hotSpot).GetComponent<SphereCollider>().radius * 1.5f || (Object)(object)jackhammer != (Object)null)
				{
					float num2 = (((Object)(object)jackhammer == (Object)null) ? 1f : jackhammer.HotspotBonusScale);
					bonusesKilled++;
					info.gatherScale = 1f + Mathf.Clamp((float)bonusesKilled * 0.5f, 0f, 2f * num2);
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
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		CleanupBonus();
		_hotSpot = SpawnBonusSpot(lastNodeDir);
	}

	public OreHotSpot SpawnBonusSpot(Vector3 lastDirection)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		if (base.isClient)
		{
			return null;
		}
		if (!bonusPrefab.isValid)
		{
			return null;
		}
		Vector2 insideUnitCircle = Random.insideUnitCircle;
		Vector2 normalized = ((Vector2)(ref insideUnitCircle)).normalized;
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
		Vector3 normalized2 = ((Vector3)(ref val6)).normalized;
		RaycastHit val8 = default(RaycastHit);
		if (((Collider)stageComponent).Raycast(new Ray(zero, normalized2), ref val8, 10f))
		{
			OreHotSpot oreHotSpot = GameManager.server.CreateEntity(bonusPrefab.resourcePath, ((RaycastHit)(ref val8)).point - normalized2 * 0.025f, Quaternion.LookRotation(((RaycastHit)(ref val8)).normal, Vector3.up)) as OreHotSpot;
			oreHotSpot.Spawn();
			((Component)oreHotSpot).SendMessage("OreOwner", (object)this);
			return oreHotSpot;
		}
		return null;
	}

	public Vector3 RandomCircle(float distance = 1f, bool allowInside = false)
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
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		Profiler.BeginSample("RandomHemisphereDirection");
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
		Profiler.EndSample();
		Vector3 val4 = input + val3;
		return ((Vector3)(ref val4)).normalized;
	}

	public Vector3 ClampToHemisphere(Vector3 hemiInput, float degreesOffset, Vector3 inputVec)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		Profiler.BeginSample("ClampToHemisphere");
		degreesOffset = Mathf.Clamp(degreesOffset / 180f, -180f, 180f);
		Vector3 val = hemiInput + Vector3.one * degreesOffset;
		Vector3 normalized = ((Vector3)(ref val)).normalized;
		val = hemiInput + Vector3.one * (0f - degreesOffset);
		Vector3 normalized2 = ((Vector3)(ref val)).normalized;
		for (int i = 0; i < 3; i++)
		{
			((Vector3)(ref inputVec))[i] = Mathf.Clamp(((Vector3)(ref inputVec))[i], ((Vector3)(ref normalized2))[i], ((Vector3)(ref normalized))[i]);
		}
		Profiler.EndSample();
		return inputVec;
	}

	public static Vector3 RandomCylinderPointAroundVector(Vector3 input, float distance, float minHeight = 0f, float maxHeight = 0f, bool allowInside = false)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		return Vector3.zero;
	}
}
