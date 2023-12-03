using System;
using System.Collections.Generic;
using Facepunch;
using Network;
using Rust;
using UnityEngine;

public class GunTrap : StorageContainer
{
	public static class GunTrapFlags
	{
		public const Flags Triggered = Flags.Reserved1;
	}

	public class GunTrapScanWorkQueue : PersistentObjectWorkQueue<GunTrap>
	{
		protected override void RunJob(GunTrap entity)
		{
			if (((PersistentObjectWorkQueue<GunTrap>)this).ShouldAdd(entity))
			{
				entity.TriggerCheck();
			}
		}

		protected override bool ShouldAdd(GunTrap entity)
		{
			if (base.ShouldAdd(entity))
			{
				return entity.IsValid();
			}
			return false;
		}
	}

	public GameObjectRef gun_fire_effect;

	public GameObjectRef bulletEffect;

	public GameObjectRef triggeredEffect;

	public Transform muzzlePos;

	public Transform eyeTransform;

	public int numPellets = 15;

	public int aimCone = 30;

	public float sensorRadius = 1.25f;

	public ItemDefinition ammoType;

	public TargetTrigger trigger;

	private float triggerCooldown;

	private BuildingPrivlidge _cachedTc;

	private float _cacheTimeout;

	[ServerVar(Help = "How many milliseconds to spend on target scanning per frame")]
	public static float gun_trap_budget_ms = 0.5f;

	public static GunTrapScanWorkQueue updateGunTrapWorkQueue = new GunTrapScanWorkQueue();

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("GunTrap.OnRpcMessage", 0);
		try
		{
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	public override string Categorize()
	{
		return "GunTrap";
	}

	public bool UseAmmo()
	{
		foreach (Item item in base.inventory.itemList)
		{
			if ((Object)(object)item.info == (Object)(object)ammoType && item.amount > 0)
			{
				item.UseItem();
				return true;
			}
		}
		return false;
	}

	public void FireWeapon()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		if (UseAmmo())
		{
			Effect.server.Run(gun_fire_effect.resourcePath, this, StringPool.Get(((Object)((Component)muzzlePos).gameObject).name), Vector3.zero, Vector3.zero);
			for (int i = 0; i < numPellets; i++)
			{
				FireBullet();
			}
		}
	}

	public void FireBullet()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		float damageAmount = 10f;
		Vector3 val = ((Component)muzzlePos).transform.position - muzzlePos.forward * 0.25f;
		Vector3 val2 = AimConeUtil.GetModifiedAimConeDirection(inputVec: ((Component)muzzlePos).transform.forward, aimCone: aimCone);
		Vector3 arg = val + val2 * 300f;
		ClientRPC<Vector3>(null, "CLIENT_FireGun", arg);
		List<RaycastHit> list = Pool.GetList<RaycastHit>();
		int layerMask = 1220225793;
		GamePhysics.TraceAll(new Ray(val, val2), 0.1f, list, 300f, layerMask, (QueryTriggerInteraction)0);
		for (int i = 0; i < list.Count; i++)
		{
			RaycastHit hit = list[i];
			BaseEntity entity = hit.GetEntity();
			if ((Object)(object)entity != (Object)null && ((Object)(object)entity == (Object)(object)this || entity.EqualNetID((BaseNetworkable)this)))
			{
				continue;
			}
			if ((Object)(object)(entity as BaseCombatEntity) != (Object)null)
			{
				HitInfo info = new HitInfo(this, entity, DamageType.Bullet, damageAmount, ((RaycastHit)(ref hit)).point);
				entity.OnAttacked(info);
				if (entity is BasePlayer || entity is BaseNpc)
				{
					Effect.server.ImpactEffect(new HitInfo
					{
						HitPositionWorld = ((RaycastHit)(ref hit)).point,
						HitNormalWorld = -((RaycastHit)(ref hit)).normal,
						HitMaterial = StringPool.Get("Flesh")
					});
				}
			}
			if (!((Object)(object)entity != (Object)null) || entity.ShouldBlockProjectiles())
			{
				arg = ((RaycastHit)(ref hit)).point;
				break;
			}
		}
	}

	public override void ServerInit()
	{
		base.ServerInit();
		((PersistentObjectWorkQueue<GunTrap>)updateGunTrapWorkQueue).Add(this);
	}

	internal override void DoServerDestroy()
	{
		base.DoServerDestroy();
		((PersistentObjectWorkQueue<GunTrap>)updateGunTrapWorkQueue).Remove(this);
	}

	public void TriggerCheck()
	{
		if (!(triggerCooldown > Time.realtimeSinceStartup) && CheckTrigger())
		{
			FireWeapon();
			triggerCooldown = Time.realtimeSinceStartup + 0.5f;
		}
	}

	private BuildingPrivlidge GetCachedTc()
	{
		if ((Object)(object)_cachedTc == (Object)null || Time.realtimeSinceStartup > _cacheTimeout)
		{
			_cachedTc = GetBuildingPrivilege();
			_cacheTimeout = Time.realtimeSinceStartup + 3f;
		}
		if ((Object)(object)_cachedTc != (Object)null && _cachedTc.IsDestroyed)
		{
			_cachedTc = null;
		}
		return _cachedTc;
	}

	public bool CheckTrigger()
	{
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		HashSet<BaseEntity> entityContents = trigger.entityContents;
		if (entityContents == null || entityContents.Count == 0)
		{
			return false;
		}
		List<RaycastHit> list = Pool.GetList<RaycastHit>();
		BuildingPrivlidge cachedTc = GetCachedTc();
		bool flag = false;
		foreach (BaseEntity item in entityContents)
		{
			BasePlayer component = ((Component)item).GetComponent<BasePlayer>();
			if (component.IsSleeping() || !component.IsAlive() || (!((Object)(object)cachedTc == (Object)null) && cachedTc.IsAuthed(component)))
			{
				continue;
			}
			list.Clear();
			Vector3 position = component.eyes.position;
			Vector3 val = GetEyePosition() - component.eyes.position;
			GamePhysics.TraceAll(new Ray(position, ((Vector3)(ref val)).normalized), 0f, list, 9f, 1218519297, (QueryTriggerInteraction)0);
			for (int i = 0; i < list.Count; i++)
			{
				BaseEntity entity = list[i].GetEntity();
				if ((Object)(object)entity != (Object)null && ((Object)(object)entity == (Object)(object)this || entity.EqualNetID((BaseNetworkable)this)))
				{
					flag = true;
					break;
				}
				if (!((Object)(object)entity != (Object)null) || entity.ShouldBlockProjectiles())
				{
					break;
				}
			}
			if (flag)
			{
				break;
			}
		}
		Pool.FreeList<RaycastHit>(ref list);
		return flag;
	}

	public bool IsTriggered()
	{
		return HasFlag(Flags.Reserved1);
	}

	public Vector3 GetEyePosition()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return eyeTransform.position;
	}
}
