using System;
using ConVar;
using Facepunch.Rust;
using Network;
using UnityEngine;

public class TreeEntity : ResourceEntity, IPrefabPreProcess
{
	[Header("Falling")]
	public bool fallOnKilled = true;

	public float fallDuration = 1.5f;

	public GameObjectRef fallStartSound;

	public GameObjectRef fallImpactSound;

	public GameObjectRef fallImpactParticles;

	public SoundDefinition fallLeavesLoopDef;

	[NonSerialized]
	public bool[] usedHeights = new bool[20];

	public bool impactSoundPlayed = false;

	private float treeDistanceUponFalling;

	public GameObjectRef prefab;

	public bool hasBonusGame = true;

	public GameObjectRef bonusHitEffect;

	public GameObjectRef bonusHitSound;

	public Collider serverCollider;

	public Collider clientCollider;

	public SoundDefinition smallCrackSoundDef;

	public SoundDefinition medCrackSoundDef;

	private float lastAttackDamage;

	[NonSerialized]
	protected BaseEntity xMarker;

	private int currentBonusLevel = 0;

	private float lastDirection = -1f;

	private float lastHitTime = 0f;

	private int lastHitMarkerIndex = -1;

	private float nextBirdTime = 0f;

	private uint birdCycleIndex = 0u;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("TreeEntity.OnRpcMessage", 0);
		try
		{
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	public override void ResetState()
	{
		base.ResetState();
	}

	public override float BoundsPadding()
	{
		return 1f;
	}

	public override void ServerInit()
	{
		base.ServerInit();
		lastDirection = ((Random.Range(0, 2) != 0) ? 1 : (-1));
		TreeManager.OnTreeSpawned(this);
	}

	internal override void DoServerDestroy()
	{
		base.DoServerDestroy();
		CleanupMarker();
		TreeManager.OnTreeDestroyed(this);
	}

	public bool DidHitMarker(HitInfo info)
	{
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)xMarker == (Object)null)
		{
			return false;
		}
		TreeMarkerData treeMarkerData = PrefabAttribute.server.Find<TreeMarkerData>(prefabID);
		if (treeMarkerData != null)
		{
			Bounds val = default(Bounds);
			((Bounds)(ref val))._002Ector(((Component)xMarker).transform.position, Vector3.one * 0.2f);
			if (((Bounds)(ref val)).Contains(info.HitPositionWorld))
			{
				return true;
			}
		}
		else
		{
			Vector3 val2 = Vector3Ex.Direction2D(((Component)this).transform.position, ((Component)xMarker).transform.position);
			Vector3 attackNormal = info.attackNormal;
			float num = Vector3.Dot(val2, attackNormal);
			float num2 = Vector3.Distance(((Component)xMarker).transform.position, info.HitPositionWorld);
			if (num >= 0.3f && num2 <= 0.2f)
			{
				return true;
			}
		}
		return false;
	}

	public void StartBonusGame()
	{
		if (((FacepunchBehaviour)this).IsInvoking((Action)StopBonusGame))
		{
			((FacepunchBehaviour)this).CancelInvoke((Action)StopBonusGame);
		}
		((FacepunchBehaviour)this).Invoke((Action)StopBonusGame, 60f);
	}

	public void StopBonusGame()
	{
		CleanupMarker();
		lastHitTime = 0f;
		currentBonusLevel = 0;
	}

	public bool BonusActive()
	{
		return (Object)(object)xMarker != (Object)null;
	}

	private void DoBirds()
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		if (!base.isClient && !(Time.realtimeSinceStartup < nextBirdTime) && !(((Bounds)(ref bounds)).extents.y < 6f))
		{
			uint num = (uint)(int)net.ID.Value + birdCycleIndex;
			if (SeedRandom.Range(ref num, 0, 2) == 0)
			{
				Effect.server.Run("assets/prefabs/npc/birds/birdemission.prefab", ((Component)this).transform.position + Vector3.up * Random.Range(((Bounds)(ref bounds)).extents.y * 0.65f, ((Bounds)(ref bounds)).extents.y * 0.9f), Vector3.up);
			}
			birdCycleIndex++;
			nextBirdTime = Time.realtimeSinceStartup + 90f;
		}
	}

	public override void OnAttacked(HitInfo info)
	{
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0304: Unknown result type (might be due to invalid IL or missing references)
		//IL_0306: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		//IL_030d: Unknown result type (might be due to invalid IL or missing references)
		//IL_030f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0314: Unknown result type (might be due to invalid IL or missing references)
		//IL_0319: Unknown result type (might be due to invalid IL or missing references)
		//IL_031e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0326: Unknown result type (might be due to invalid IL or missing references)
		//IL_0328: Unknown result type (might be due to invalid IL or missing references)
		//IL_032d: Unknown result type (might be due to invalid IL or missing references)
		//IL_032f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0331: Unknown result type (might be due to invalid IL or missing references)
		//IL_0336: Unknown result type (might be due to invalid IL or missing references)
		//IL_033b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0340: Unknown result type (might be due to invalid IL or missing references)
		//IL_0348: Unknown result type (might be due to invalid IL or missing references)
		//IL_034a: Unknown result type (might be due to invalid IL or missing references)
		//IL_034f: Unknown result type (might be due to invalid IL or missing references)
		//IL_036d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0372: Unknown result type (might be due to invalid IL or missing references)
		//IL_0391: Unknown result type (might be due to invalid IL or missing references)
		//IL_0396: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_03af: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03db: Unknown result type (might be due to invalid IL or missing references)
		bool canGather = info.CanGather;
		float num = Time.time - lastHitTime;
		lastHitTime = Time.time;
		DoBirds();
		if (!hasBonusGame || !canGather || (Object)(object)info.Initiator == (Object)null || (BonusActive() && !DidHitMarker(info)))
		{
			base.OnAttacked(info);
			return;
		}
		if ((Object)(object)xMarker != (Object)null && !info.DidGather && info.gatherScale > 0f)
		{
			xMarker.ClientRPC(null, "MarkerHit", currentBonusLevel);
			currentBonusLevel++;
			info.gatherScale = 1f + Mathf.Clamp((float)currentBonusLevel * 0.125f, 0f, 1f);
		}
		Vector3 val = (((Object)(object)xMarker != (Object)null) ? ((Component)xMarker).transform.position : info.HitPositionWorld);
		CleanupMarker();
		TreeMarkerData treeMarkerData = PrefabAttribute.server.Find<TreeMarkerData>(prefabID);
		if (treeMarkerData != null)
		{
			Vector3 nearbyPoint = treeMarkerData.GetNearbyPoint(((Component)this).transform.InverseTransformPoint(val), ref lastHitMarkerIndex, out var normal);
			nearbyPoint = ((Component)this).transform.TransformPoint(nearbyPoint);
			Quaternion rot = QuaternionEx.LookRotationNormal(((Component)this).transform.TransformDirection(normal));
			xMarker = GameManager.server.CreateEntity("assets/content/nature/treesprefabs/trees/effects/tree_marking_nospherecast.prefab", nearbyPoint, rot);
		}
		else
		{
			Vector3 val2 = Vector3Ex.Direction2D(((Component)this).transform.position, val);
			Vector3 val3 = val2;
			Vector3 val4 = Vector3.Cross(val3, Vector3.up);
			float num2 = lastDirection;
			float num3 = Random.Range(0.5f, 0.5f);
			Vector3 val5 = Vector3.Lerp(-val3, val4 * num2, num3);
			Vector3 val6 = ((Component)this).transform.InverseTransformDirection(((Vector3)(ref val5)).normalized) * 2.5f;
			val6 = ((Component)this).transform.InverseTransformPoint(GetCollider().ClosestPoint(((Component)this).transform.TransformPoint(val6)));
			Vector3 val7 = ((Component)this).transform.TransformPoint(val6);
			Vector3 val8 = ((Component)this).transform.InverseTransformPoint(info.HitPositionWorld);
			val6.y = val8.y;
			Vector3 val9 = ((Component)this).transform.InverseTransformPoint(info.Initiator.CenterPoint());
			float num4 = Mathf.Max(0.75f, val9.y);
			float num5 = val9.y + 0.5f;
			val6.y = Mathf.Clamp(val6.y + Random.Range(0.1f, 0.2f) * ((Random.Range(0, 2) == 0) ? (-1f) : 1f), num4, num5);
			Vector3 val10 = Vector3Ex.Direction2D(((Component)this).transform.position, val7);
			Vector3 val11 = val10;
			val10 = ((Component)this).transform.InverseTransformDirection(val10);
			Quaternion val12 = QuaternionEx.LookRotationNormal(-val10, Vector3.zero);
			val6 = ((Component)this).transform.TransformPoint(val6);
			val12 = QuaternionEx.LookRotationNormal(-val11, Vector3.zero);
			val6 = GetCollider().ClosestPoint(val6);
			Line val13 = default(Line);
			((Line)(ref val13))._002Ector(((Component)GetCollider()).transform.TransformPoint(new Vector3(0f, 10f, 0f)), ((Component)GetCollider()).transform.TransformPoint(new Vector3(0f, -10f, 0f)));
			Vector3 val14 = ((Line)(ref val13)).ClosestPoint(val6);
			Vector3 val15 = Vector3Ex.Direction(val14, val6);
			val12 = QuaternionEx.LookRotationNormal(-val15);
			xMarker = GameManager.server.CreateEntity("assets/content/nature/treesprefabs/trees/effects/tree_marking.prefab", val6, val12);
		}
		xMarker.Spawn();
		if (num > 5f)
		{
			StartBonusGame();
		}
		base.OnAttacked(info);
		if (health > 0f)
		{
			lastAttackDamage = info.damageTypes.Total();
			int num6 = Mathf.CeilToInt(health / lastAttackDamage);
			if (num6 < 2)
			{
				ClientRPC(null, "CrackSound", 1);
			}
			else if (num6 < 5)
			{
				ClientRPC(null, "CrackSound", 0);
			}
		}
	}

	public void CleanupMarker()
	{
		if (Object.op_Implicit((Object)(object)xMarker))
		{
			xMarker.Kill();
		}
		xMarker = null;
	}

	public Collider GetCollider()
	{
		if (base.isServer)
		{
			return (Collider)(((Object)(object)serverCollider == (Object)null) ? ((object)((Component)this).GetComponentInChildren<CapsuleCollider>()) : ((object)serverCollider));
		}
		return ((Object)(object)clientCollider == (Object)null) ? ((Component)this).GetComponent<Collider>() : clientCollider;
	}

	public override void OnKilled(HitInfo info)
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		if (isKilled)
		{
			return;
		}
		isKilled = true;
		CleanupMarker();
		Analytics.Server.TreeKilled(info.WeaponPrefab);
		if (fallOnKilled)
		{
			Collider collider = GetCollider();
			if (Object.op_Implicit((Object)(object)collider))
			{
				collider.enabled = false;
			}
			Vector3 val = info.attackNormal;
			if (val == Vector3.zero)
			{
				val = Vector3Ex.Direction2D(((Component)this).transform.position, info.PointStart);
			}
			ClientRPC<Vector3>(null, "TreeFall", val);
			((FacepunchBehaviour)this).Invoke((Action)DelayedKill, fallDuration + 1f);
		}
		else
		{
			DelayedKill();
		}
	}

	public void DelayedKill()
	{
		Kill();
	}

	public override void PreProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		base.PreProcess(preProcess, rootObj, name, serverside, clientside, bundling);
		if (serverside)
		{
			globalBroadcast = Tree.global_broadcast;
		}
	}
}
