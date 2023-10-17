using System;
using System.Collections.Generic;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;

public class AttackHelicopterRockets : StorageContainer
{
	[SerializeField]
	private Transform[] rocketMuzzlePositions;

	[SerializeField]
	private GameObjectRef rocketFireTubeFX;

	[SerializeField]
	private float timeBetweenRockets = 0.5f;

	[SerializeField]
	private float reloadTime = 8f;

	[SerializeField]
	private int rocketsPerReload = 6;

	[SerializeField]
	private ItemDefinition incendiaryRocketDef;

	[SerializeField]
	private ItemDefinition hvRocketDef;

	[SerializeField]
	private ItemDefinition flareItemDef;

	[NonSerialized]
	public AttackHelicopter owner;

	private const AmmoTypes ammoType = 32;

	private TimeSince timeSinceRocketFired;

	private int rocketsSinceReload;

	private bool leftSide;

	public bool CanFireNow
	{
		get
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			if (!IsReloading && TimeSince.op_Implicit(timeSinceRocketFired) >= timeBetweenRockets)
			{
				return GetAmmoAmount() > 0;
			}
			return false;
		}
	}

	public bool IsReloading
	{
		get
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			if (rocketsSinceReload >= rocketsPerReload && TimeSince.op_Implicit(timeSinceRocketFired) < reloadTime)
			{
				return GetAmmoAmount() > 0;
			}
			return false;
		}
	}

	private bool HasOwner => (Object)(object)owner != (Object)null;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("AttackHelicopterRockets.OnRpcMessage", 0);
		try
		{
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	public int GetAmmoAmount()
	{
		if (base.isServer)
		{
			return base.inventory.GetAmmoAmount((AmmoTypes)32);
		}
		return 0;
	}

	public int GetAmmoBeforeReload()
	{
		int num = ((rocketsSinceReload >= rocketsPerReload) ? rocketsSinceReload : (rocketsPerReload - rocketsSinceReload));
		return Mathf.Min(GetAmmoAmount(), num);
	}

	public bool TryGetAmmoDef(out ItemDefinition ammoDef)
	{
		ammoDef = null;
		if (base.isServer)
		{
			List<Item> list = Pool.GetList<Item>();
			base.inventory.FindAmmo(list, (AmmoTypes)32);
			if (list.Count > 0)
			{
				ammoDef = list[list.Count - 1].info;
			}
			Pool.FreeList<Item>(ref list);
		}
		return (Object)(object)ammoDef != (Object)null;
	}

	public Vector3 MuzzleMidPoint()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		return (rocketMuzzlePositions[1].position + rocketMuzzlePositions[0].position) * 0.5f;
	}

	public float GetMinRocketSpeed()
	{
		return owner.GetSpeed() + 2f;
	}

	public bool TryGetProjectedHitPos(out Vector3 result)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		result = Vector3.zero;
		if (!TryGetAmmoDef(out var ammoDef))
		{
			return false;
		}
		ItemModProjectile component = ((Component)ammoDef).GetComponent<ItemModProjectile>();
		ServerProjectile component2 = component.projectileObject.Get().GetComponent<ServerProjectile>();
		if ((Object)(object)component != (Object)null && (Object)(object)component2 != (Object)null)
		{
			Vector3 origin = MuzzleMidPoint();
			Vector3 forward = ((Component)owner).transform.forward;
			float minRocketSpeed = GetMinRocketSpeed();
			float gravity = Physics.gravity.y * component2.gravityModifier;
			Vector3 val = component2.initialVelocity + forward * component2.speed;
			if (minRocketSpeed > 0f)
			{
				float num = Vector3.Dot(val, forward) - minRocketSpeed;
				if (num < 0f)
				{
					val += forward * (0f - num);
				}
			}
			result = Ballistics.GetPhysicsProjectileHitPos(origin, ((Vector3)(ref val)).normalized, ((Vector3)(ref val)).magnitude, gravity, 1.5f, 0.5f, 32f, owner);
			return true;
		}
		return false;
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		info.msg.attackHeliRockets = Pool.Get<AttackHeliRockets>();
		info.msg.attackHeliRockets.totalAmmo = GetAmmoAmount();
		info.msg.attackHeliRockets.rocketsSinceReload = rocketsSinceReload;
		if (TryGetAmmoDef(out var ammoDef))
		{
			info.msg.attackHeliRockets.ammoItemID = ammoDef.itemid;
		}
	}

	public override BasePlayer ToPlayer()
	{
		if (HasOwner)
		{
			return owner.GetPassenger();
		}
		return null;
	}

	public override bool ItemFilter(Item item, int targetSlot)
	{
		if (!base.ItemFilter(item, targetSlot))
		{
			return false;
		}
		if (targetSlot == -1)
		{
			if (IsValidFlare())
			{
				for (int i = 12; i < base.inventory.capacity; i++)
				{
					if (!base.inventory.SlotTaken(item, i))
					{
						targetSlot = i;
						break;
					}
				}
			}
			else
			{
				if (!IsValidRocket())
				{
					return false;
				}
				for (int j = 0; j < 12; j++)
				{
					if (!base.inventory.SlotTaken(item, j))
					{
						targetSlot = j;
						break;
					}
				}
			}
		}
		if (targetSlot < 12)
		{
			return IsValidRocket();
		}
		return IsValidFlare();
		bool IsValidFlare()
		{
			return (Object)(object)item.info == (Object)(object)flareItemDef;
		}
		bool IsValidRocket()
		{
			if (!((Object)(object)item.info == (Object)(object)incendiaryRocketDef))
			{
				return (Object)(object)item.info == (Object)(object)hvRocketDef;
			}
			return true;
		}
	}

	public override void OnItemAddedOrRemoved(Item item, bool added)
	{
		if (added)
		{
			rocketsSinceReload = 0;
		}
		SendNetworkUpdate();
	}

	public bool InputTick(AttackHelicopter.GunnerInputState input, BasePlayer gunner)
	{
		if (!owner.GunnerIsInGunnerView)
		{
			return false;
		}
		bool result = false;
		if (input.fire2)
		{
			result = TryFireRocket(gunner);
		}
		return result;
	}

	public bool TryFireRocket(BasePlayer shooter)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		if (!CanFireNow)
		{
			return false;
		}
		if ((Object)(object)owner == (Object)null)
		{
			return false;
		}
		if (owner.InSafeZone())
		{
			return false;
		}
		int num = ((!leftSide) ? 1 : 0);
		Vector3 position = rocketMuzzlePositions[num].position;
		Vector3 forward = rocketMuzzlePositions[num].forward;
		float minRocketSpeed = GetMinRocketSpeed();
		if (owner.TryFireProjectile(this, (AmmoTypes)32, position, forward, shooter, 1f, minRocketSpeed, out var _))
		{
			Effect.server.Run(rocketFireTubeFX.resourcePath, this, StringPool.Get(((Object)rocketMuzzlePositions[num]).name), Vector3.zero, Vector3.zero, null, broadcast: true);
			leftSide = !leftSide;
			ItemDefinition ammoDef;
			int arg = (TryGetAmmoDef(out ammoDef) ? ammoDef.itemid : 0);
			timeSinceRocketFired = TimeSince.op_Implicit(0f);
			if (rocketsSinceReload < rocketsPerReload)
			{
				rocketsSinceReload++;
			}
			else
			{
				rocketsSinceReload = 1;
			}
			ClientRPC(null, "RPCUpdateAmmo", (short)GetAmmoAmount(), arg, rocketsSinceReload);
			return true;
		}
		return false;
	}

	public bool TryTakeFlare()
	{
		if (base.inventory.TryTakeOne(flareItemDef.itemid, out var item))
		{
			item.Remove();
			return true;
		}
		return false;
	}
}
