using System;
using System.Collections.Generic;
using Facepunch;
using ProtoBuf;
using Rust;
using UnityEngine;

public class HalloweenDungeon : BasePortal
{
	public GameObjectRef dungeonPrefab;

	public EntityRef<ProceduralDynamicDungeon> dungeonInstance;

	[ServerVar(Help = "Population active on the server", ShowInAdminUI = true)]
	public static float population = 0f;

	[ServerVar(Help = "How long each active dungeon should last before dying", ShowInAdminUI = true)]
	public static float lifetime = 600f;

	private float secondsUsed;

	private float timeAlive;

	public AnimationCurve radiationCurve;

	public Phrase collapsePhrase;

	public Phrase mountPhrase;

	private bool anyplayers_cached;

	private float nextPlayerCheckTime = float.NegativeInfinity;

	public virtual float GetLifetime()
	{
		return lifetime;
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (info.fromDisk && info.msg.ioEntity != null)
		{
			dungeonInstance.uid = info.msg.ioEntity.genericEntRef3;
			secondsUsed = info.msg.ioEntity.genericFloat1;
			timeAlive = info.msg.ioEntity.genericFloat2;
		}
	}

	public float GetLifeFraction()
	{
		return Mathf.Clamp01(secondsUsed / GetLifetime());
	}

	public void Update()
	{
		if (!base.isClient)
		{
			if (secondsUsed > 0f)
			{
				secondsUsed += Time.deltaTime;
			}
			timeAlive += Time.deltaTime;
			float lifeFraction = GetLifeFraction();
			if (dungeonInstance.IsValid(serverside: true))
			{
				ProceduralDynamicDungeon proceduralDynamicDungeon = dungeonInstance.Get(serverside: true);
				float num = radiationCurve.Evaluate(lifeFraction) * 80f;
				proceduralDynamicDungeon.exitRadiation.RadiationAmountOverride = Mathf.Clamp(num, 0f, float.PositiveInfinity);
			}
			if (lifeFraction >= 1f)
			{
				KillIfNoPlayers();
			}
			else if (timeAlive > 3600f && secondsUsed == 0f)
			{
				ClearAllEntitiesInRadius(80f);
				Kill();
			}
		}
	}

	public void KillIfNoPlayers()
	{
		if (!AnyPlayersInside())
		{
			ClearAllEntitiesInRadius(80f);
			Kill();
		}
	}

	public bool AnyPlayersInside()
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		ProceduralDynamicDungeon proceduralDynamicDungeon = dungeonInstance.Get(serverside: true);
		if ((Object)(object)proceduralDynamicDungeon == (Object)null)
		{
			anyplayers_cached = false;
		}
		else if (Time.time > nextPlayerCheckTime)
		{
			nextPlayerCheckTime = Time.time + 10f;
			anyplayers_cached = BaseNetworkable.HasCloseConnections(((Component)proceduralDynamicDungeon).transform.position, 80f);
		}
		return anyplayers_cached;
	}

	private void ClearAllEntitiesInRadius(float radius)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		ProceduralDynamicDungeon proceduralDynamicDungeon = dungeonInstance.Get(serverside: true);
		if ((Object)(object)proceduralDynamicDungeon == (Object)null)
		{
			return;
		}
		List<BaseEntity> list = Pool.GetList<BaseEntity>();
		Vis.Entities(((Component)proceduralDynamicDungeon).transform.position, radius, list, -1, (QueryTriggerInteraction)2);
		foreach (BaseEntity item in list)
		{
			if (item.IsValid() && !item.IsDestroyed)
			{
				if (item is LootableCorpse lootableCorpse)
				{
					lootableCorpse.blockBagDrop = true;
				}
				item.Kill();
			}
		}
		Pool.FreeList<BaseEntity>(ref list);
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		if (info.msg.ioEntity == null)
		{
			info.msg.ioEntity = Pool.Get<IOEntity>();
		}
		info.msg.ioEntity.genericEntRef3 = dungeonInstance.uid;
		info.msg.ioEntity.genericFloat1 = secondsUsed;
		info.msg.ioEntity.genericFloat2 = timeAlive;
	}

	public override void PostServerLoad()
	{
		base.PostServerLoad();
		timeAlive += Random.Range(0f, 60f);
	}

	public override void UsePortal(BasePlayer player)
	{
		if (GetLifeFraction() > 0.8f)
		{
			player.ShowToast(GameTip.Styles.Blue_Normal, collapsePhrase);
			return;
		}
		if (player.isMounted)
		{
			player.ShowToast(GameTip.Styles.Blue_Normal, mountPhrase);
			return;
		}
		if (secondsUsed == 0f)
		{
			secondsUsed = 1f;
		}
		base.UsePortal(player);
	}

	public override void Spawn()
	{
		base.Spawn();
	}

	public override void ServerInit()
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		base.ServerInit();
		if (!Application.isLoadingSave)
		{
			timeAlive = Random.Range(0f, 60f);
			SpawnSubEntities();
		}
		localEntryExitPos.DropToGround(alignToNormal: false, 10f);
		Transform transform = ((Component)localEntryExitPos).transform;
		transform.position += Vector3.up * 0.05f;
		((FacepunchBehaviour)this).Invoke((Action)CheckBlocked, 0.25f);
	}

	public void CheckBlocked()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		float num = 0.5f;
		float num2 = 1.8f;
		Vector3 position = localEntryExitPos.position;
		Vector3 val = position + new Vector3(0f, num, 0f);
		Vector3 val2 = position + new Vector3(0f, num2 - num, 0f);
		if (Physics.CheckCapsule(val, val2, num, 1537286401))
		{
			Kill();
		}
	}

	public static Vector3 GetDungeonSpawnPoint()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		float num = Mathf.Floor(TerrainMeta.Size.x / 200f);
		float num2 = 1000f;
		Vector3 zero = Vector3.zero;
		zero.x = 0f - Mathf.Min(TerrainMeta.Size.x * 0.5f, 4000f) + 200f;
		zero.y = 1025f;
		zero.z = 0f - Mathf.Min(TerrainMeta.Size.z * 0.5f, 4000f) + 200f;
		_ = Vector3.zero;
		for (int i = 0; (float)i < num2; i++)
		{
			for (int j = 0; (float)j < num; j++)
			{
				Vector3 val = zero + new Vector3((float)j * 200f, (float)i * 100f, 0f);
				bool flag = false;
				foreach (ProceduralDynamicDungeon dungeon in ProceduralDynamicDungeon.dungeons)
				{
					if ((Object)(object)dungeon != (Object)null && dungeon.isServer && Vector3.Distance(((Component)dungeon).transform.position, val) < 10f)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					return val;
				}
			}
		}
		return Vector3.zero;
	}

	internal override void DoServerDestroy()
	{
		base.DoServerDestroy();
		if (dungeonInstance.IsValid(serverside: true))
		{
			dungeonInstance.Get(serverside: true).Kill();
		}
	}

	public void DelayedDestroy()
	{
		Kill();
	}

	public void SpawnSubEntities()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		Vector3 dungeonSpawnPoint = GetDungeonSpawnPoint();
		if (dungeonSpawnPoint == Vector3.zero)
		{
			Debug.LogError((object)"No dungeon spawn point");
			((FacepunchBehaviour)this).Invoke((Action)DelayedDestroy, 5f);
			return;
		}
		BaseEntity baseEntity = GameManager.server.CreateEntity(dungeonPrefab.resourcePath, dungeonSpawnPoint, Quaternion.identity);
		ProceduralDynamicDungeon component = ((Component)baseEntity).GetComponent<ProceduralDynamicDungeon>();
		component.mapOffset = ((Component)this).transform.position - dungeonSpawnPoint;
		baseEntity.Spawn();
		dungeonInstance.Set(component);
		BasePortal basePortal = (targetPortal = component.GetExitPortal());
		basePortal.targetPortal = this;
		LinkPortal();
		basePortal.LinkPortal();
	}
}
