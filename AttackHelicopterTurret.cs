using System;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;

public class AttackHelicopterTurret : StorageContainer
{
	public enum GunStatus
	{
		NoWeapon,
		Ready,
		Reloading,
		NoAmmo
	}

	[SerializeField]
	private Transform turretSocket;

	[SerializeField]
	private Transform turretHorizontal;

	[SerializeField]
	private Transform turretVertical;

	[NonSerialized]
	public AttackHelicopter owner;

	private EntityRef<HeldEntity> attachedHeldEntity;

	[NonSerialized]
	public bool forceAcceptAmmo;

	private const float WEAPON_Z_OFFSET_SCALE = -0.5f;

	private float muzzleYOffset;

	private float lastSentX;

	private float lastSentY;

	private bool HasOwner => (Object)(object)owner != (Object)null;

	public GunStatus GunState { get; private set; }

	public float GunXAngle => turretVertical.localEulerAngles.x;

	public float GunYAngle => turretHorizontal.localEulerAngles.y;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("AttackHelicopterTurret.OnRpcMessage", 0);
		try
		{
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	public override void Load(LoadInfo info)
	{
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		base.Load(info);
		if (info.msg.attackHeliTurret != null)
		{
			_ = GunState;
			GunState = (GunStatus)info.msg.attackHeliTurret.gunState;
			float xRot = info.msg.attackHeliTurret.xRot;
			float yRot = info.msg.attackHeliTurret.yRot;
			SetGunRotation(xRot, yRot);
			attachedHeldEntity.uid = info.msg.attackHeliTurret.heldEntityID;
		}
	}

	private void SetGunRotation(float xRot, float yRot)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		if (!((Object)(object)owner == (Object)null))
		{
			turretHorizontal.localEulerAngles = new Vector3(0f, yRot, 0f);
			turretVertical.localEulerAngles = new Vector3(0f - xRot, 0f, 0f);
		}
	}

	public HeldEntity GetAttachedHeldEntity()
	{
		HeldEntity heldEntity = attachedHeldEntity.Get(base.isServer);
		if (heldEntity.IsValid())
		{
			return heldEntity;
		}
		return null;
	}

	public void GetAmmoAmounts(out int clip, out int available)
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		clip = 0;
		available = 0;
		if (base.isServer && GetAttachedHeldEntity() is BaseProjectile baseProjectile)
		{
			clip = baseProjectile.primaryMagazine.contents;
			available = base.inventory.GetAmmoAmount(baseProjectile.primaryMagazine.definition.ammoTypes);
		}
	}

	public Vector3 GetProjectedHitPos()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		HeldEntity heldEntity = GetAttachedHeldEntity();
		if ((Object)(object)heldEntity == (Object)null || (Object)(object)heldEntity.MuzzleTransform == (Object)null)
		{
			return Ballistics.GetBulletHitPoint(turretSocket.position, turretSocket.forward);
		}
		return Ballistics.GetBulletHitPoint(heldEntity.MuzzleTransform.position, heldEntity.MuzzleTransform.forward);
	}

	public override void ServerInit()
	{
		base.ServerInit();
		ItemContainer itemContainer = base.inventory;
		itemContainer.canAcceptItem = (Func<Item, int, bool>)Delegate.Combine(itemContainer.canAcceptItem, new Func<Item, int, bool>(CanAcceptItem));
		((FacepunchBehaviour)this).InvokeRandomized((Action)RefreshGunState, 0f, 0.25f, 0.05f);
	}

	public override void Save(SaveInfo info)
	{
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		base.Save(info);
		if (HasOwner)
		{
			info.msg.attackHeliTurret = Pool.Get<AttackHeliTurret>();
			GetAmmoAmounts(out var clip, out var available);
			info.msg.attackHeliTurret.clipAmmo = clip;
			info.msg.attackHeliTurret.totalAmmo = available;
			info.msg.attackHeliTurret.gunState = (int)GunState;
			info.msg.attackHeliTurret.xRot = turretVertical.localEulerAngles.x;
			info.msg.attackHeliTurret.yRot = turretHorizontal.localEulerAngles.y;
			info.msg.attackHeliTurret.heldEntityID = attachedHeldEntity.uid;
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

	private bool CanAcceptItem(Item item, int targetSlot)
	{
		Item slot = base.inventory.GetSlot(0);
		if (IsValidWeapon(item) && targetSlot == 0)
		{
			return true;
		}
		if (item.info.category == ItemCategory.Ammunition)
		{
			if (forceAcceptAmmo)
			{
				return true;
			}
			if (slot == null || (Object)(object)GetAttachedHeldEntity() == (Object)null)
			{
				return false;
			}
			if (targetSlot == 0)
			{
				return false;
			}
			return true;
		}
		return false;
	}

	private bool IsValidWeapon(Item item)
	{
		ItemDefinition info = item.info;
		if (item.isBroken)
		{
			return false;
		}
		ItemModEntity component = ((Component)info).GetComponent<ItemModEntity>();
		if ((Object)(object)component == (Object)null)
		{
			return false;
		}
		HeldEntity component2 = component.entityPrefab.Get().GetComponent<HeldEntity>();
		if ((Object)(object)component2 == (Object)null)
		{
			return false;
		}
		if (!component2.IsUsableByTurret)
		{
			return false;
		}
		return true;
	}

	public bool InputTick(AttackHelicopter.GunnerInputState input)
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		if (!owner.GunnerIsInGunnerView)
		{
			return false;
		}
		bool result = false;
		if (input.reload)
		{
			TryReload();
		}
		else if (input.fire1)
		{
			result = TryFireWeapon();
		}
		((Ray)(ref input.eyeRay)).direction = ClampEyeAngle(((Component)owner).transform, ((Ray)(ref input.eyeRay)).direction, owner.turretPitchClamp, owner.turretYawClamp);
		Vector3 bulletHitPoint = Ballistics.GetBulletHitPoint(input.eyeRay);
		bulletHitPoint.y -= muzzleYOffset;
		Vector3 val = bulletHitPoint - turretSocket.position;
		val = ((Component)this).transform.InverseTransformDirection(val);
		Quaternion val2 = Quaternion.LookRotation(val, Vector3.up);
		Vector3 eulerAngles = ((Quaternion)(ref val2)).eulerAngles;
		float num = 0f - eulerAngles.x;
		float y = eulerAngles.y;
		SetGunRotation(num, y);
		if (Mathf.Abs(num - lastSentX) > 1f || Mathf.Abs(y - lastSentY) > 1f)
		{
			ClientRPC(null, "RPCRotation", GetNetworkTime(), num, y);
			lastSentX = num;
			lastSentY = y;
		}
		return result;
	}

	private Vector3 ClampEyeAngle(Transform heliTransform, Vector3 eyeDir, Vector2 pitchRange, Vector2 yawRange)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = heliTransform.InverseTransformDirection(eyeDir);
		float num = Mathf.Clamp(Mathf.Asin(0f - val.y) * 57.29578f, pitchRange.x, pitchRange.y);
		float num2 = Mathf.Atan2(val.x, val.z) * 57.29578f;
		num2 = Mathf.Clamp(num2, yawRange.x, yawRange.y);
		val = Quaternion.Euler(num, num2, 0f) * Vector3.forward;
		return heliTransform.TransformDirection(val);
	}

	public override void OnItemAddedOrRemoved(Item item, bool added)
	{
		base.OnItemAddedOrRemoved(item, added);
		if (Object.op_Implicit((Object)(object)((Component)item.info).GetComponent<ItemModEntity>()))
		{
			if (((FacepunchBehaviour)this).IsInvoking((Action)UpdateAttachedWeapon))
			{
				UpdateAttachedWeapon();
			}
			((FacepunchBehaviour)this).Invoke((Action)UpdateAttachedWeapon, 0.5f);
		}
	}

	private void UpdateAttachedWeapon()
	{
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		if (!HasOwner)
		{
			Debug.LogError((object)(((object)this).GetType().Name + ": Turret socket not yet set."));
			return;
		}
		HeldEntity heldEntity = AutoTurret.TryAddWeaponToTurret(base.inventory.GetSlot(0), turretSocket, this, -0.5f);
		if ((Object)(object)heldEntity != (Object)null)
		{
			attachedHeldEntity.Set(heldEntity);
			muzzleYOffset = turretSocket.InverseTransformPoint(heldEntity.MuzzleTransform.position).y;
		}
		else
		{
			HeldEntity heldEntity2 = GetAttachedHeldEntity();
			if ((Object)(object)heldEntity2 != (Object)null)
			{
				heldEntity2.SetGenericVisible(wantsVis: false);
				heldEntity2.SetLightsOn(isOn: false);
			}
			attachedHeldEntity.Set(null);
			muzzleYOffset = 0f;
		}
		SendNetworkUpdate();
	}

	private bool TryReload()
	{
		BaseProjectile baseProjectile = GetAttachedHeldEntity() as BaseProjectile;
		if ((Object)(object)baseProjectile == (Object)null)
		{
			return false;
		}
		return baseProjectile.ServerTryReload(base.inventory);
	}

	private bool TryFireWeapon()
	{
		HeldEntity heldEntity = GetAttachedHeldEntity();
		if ((Object)(object)heldEntity == (Object)null)
		{
			return false;
		}
		if (owner.InSafeZone())
		{
			return false;
		}
		if (heldEntity is BaseProjectile baseProjectile)
		{
			if (baseProjectile.primaryMagazine.contents <= 0)
			{
				baseProjectile.ServerTryReload(base.inventory);
				return false;
			}
			if (baseProjectile.NextAttackTime > Time.time)
			{
				return false;
			}
		}
		heldEntity.ServerUse();
		GetAmmoAmounts(out var clip, out var available);
		ClientRPC(null, "RPCAmmo", (short)clip, (short)available);
		return true;
	}

	private void RefreshGunState()
	{
		HeldEntity heldEntity = GetAttachedHeldEntity();
		GunStatus gunStatus;
		if (Object.op_Implicit((Object)(object)heldEntity))
		{
			gunStatus = GunStatus.Ready;
			BaseProjectile baseProjectile = heldEntity as BaseProjectile;
			if ((Object)(object)baseProjectile != (Object)null)
			{
				if (baseProjectile.ServerIsReloading())
				{
					gunStatus = GunStatus.Reloading;
				}
				else
				{
					GetAmmoAmounts(out var clip, out var available);
					if (clip == 0 && available == 0)
					{
						gunStatus = GunStatus.NoAmmo;
					}
				}
			}
		}
		else
		{
			gunStatus = GunStatus.NoWeapon;
		}
		if (gunStatus != GunState)
		{
			GunState = gunStatus;
			SendNetworkUpdate();
		}
	}
}
