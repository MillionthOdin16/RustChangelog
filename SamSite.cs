using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

public class SamSite : ContainerIOEntity
{
	public interface ISamSiteTarget
	{
		SamTargetType SAMTargetType { get; }

		bool isClient { get; }

		bool IsValidSAMTarget(bool staticRespawn);

		Vector3 CenterPoint();

		Vector3 GetWorldVelocity();

		bool IsVisible(Vector3 position, float maxDistance = float.PositiveInfinity);
	}

	public class SamTargetType
	{
		public readonly float scanRadius;

		public readonly float speedMultiplier;

		public readonly float timeBetweenBursts;

		public SamTargetType(float scanRadius, float speedMultiplier, float timeBetweenBursts)
		{
			this.scanRadius = scanRadius;
			this.speedMultiplier = speedMultiplier;
			this.timeBetweenBursts = timeBetweenBursts;
		}
	}

	public Animator pitchAnimator;

	public GameObject yaw;

	public GameObject pitch;

	public GameObject gear;

	public Transform eyePoint;

	public float gearEpislonDegrees = 20f;

	public float turnSpeed = 1f;

	public float clientLerpSpeed = 1f;

	public Vector3 currentAimDir = Vector3.forward;

	public Vector3 targetAimDir = Vector3.forward;

	public float vehicleScanRadius = 350f;

	public float missileScanRadius = 500f;

	public GameObjectRef projectileTest;

	public GameObjectRef muzzleFlashTest;

	public bool staticRespawn = false;

	public ItemDefinition ammoType;

	public Transform[] tubes;

	[ServerVar(Help = "how long until static sam sites auto repair")]
	public static float staticrepairseconds = 1200f;

	public SoundDefinition yawMovementLoopDef;

	public float yawGainLerp = 8f;

	public float yawGainMovementSpeedMult = 0.1f;

	public SoundDefinition pitchMovementLoopDef;

	public float pitchGainLerp = 10f;

	public float pitchGainMovementSpeedMult = 0.5f;

	public int lowAmmoThreshold = 5;

	public Flags Flag_DefenderMode = Flags.Reserved9;

	public static SamTargetType targetTypeUnknown;

	public static SamTargetType targetTypeVehicle;

	public static SamTargetType targetTypeMissile;

	private ISamSiteTarget currentTarget;

	private SamTargetType mostRecentTargetType;

	private Item ammoItem = null;

	private float lockOnTime;

	private float lastTargetVisibleTime = 0f;

	private int lastAmmoCount = 0;

	private int currentTubeIndex = 0;

	private int firedCount = 0;

	private float nextBurstTime = 0f;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("SamSite.OnRpcMessage", 0);
		try
		{
			if (rpc == 3160662357u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - ToggleDefenderMode "));
				}
				TimeWarning val2 = TimeWarning.New("ToggleDefenderMode", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.CallsPerSecond.Test(3160662357u, "ToggleDefenderMode", this, player, 1uL))
						{
							return true;
						}
						if (!RPC_Server.IsVisible.Test(3160662357u, "ToggleDefenderMode", this, player, 3f))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
					try
					{
						TimeWarning val4 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage msg2 = rPCMessage;
							ToggleDefenderMode(msg2);
						}
						finally
						{
							((IDisposable)val4)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in ToggleDefenderMode");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	public override bool IsPowered()
	{
		return staticRespawn || HasFlag(Flags.Reserved8);
	}

	public override int ConsumptionAmount()
	{
		return 25;
	}

	public bool IsInDefenderMode()
	{
		return HasFlag(Flag_DefenderMode);
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
	}

	private void SetTarget(ISamSiteTarget target)
	{
		bool flag = currentTarget != target;
		currentTarget = target;
		if (!target.IsUnityNull())
		{
			mostRecentTargetType = target.SAMTargetType;
		}
		if (flag)
		{
			MarkIODirty();
		}
	}

	private void MarkIODirty()
	{
		if (!staticRespawn)
		{
			lastPassthroughEnergy = -1;
			MarkDirtyForceUpdateOutputs();
		}
	}

	private void ClearTarget()
	{
		SetTarget(null);
	}

	public override void ServerInit()
	{
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		base.ServerInit();
		targetTypeUnknown = new SamTargetType(vehicleScanRadius, 1f, 5f);
		targetTypeVehicle = new SamTargetType(vehicleScanRadius, 1f, 5f);
		targetTypeMissile = new SamTargetType(missileScanRadius, 2.25f, 3.5f);
		mostRecentTargetType = targetTypeUnknown;
		ClearTarget();
		((FacepunchBehaviour)this).InvokeRandomized((Action)TargetScan, 1f, 3f, 1f);
		currentAimDir = ((Component)this).transform.forward;
		if (base.inventory != null && !staticRespawn)
		{
			base.inventory.onItemAddedRemoved = OnItemAddedRemoved;
		}
	}

	private void OnItemAddedRemoved(Item arg1, bool arg2)
	{
		EnsureReloaded();
		if (IsPowered())
		{
			MarkIODirty();
		}
	}

	public override void Save(SaveInfo info)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		base.Save(info);
		info.msg.samSite = Pool.Get<SAMSite>();
		info.msg.samSite.aimDir = GetAimDir();
	}

	public override void PostServerLoad()
	{
		base.PostServerLoad();
		if (staticRespawn && HasFlag(Flags.Reserved1))
		{
			((FacepunchBehaviour)this).Invoke((Action)SelfHeal, staticrepairseconds);
		}
	}

	public void SelfHeal()
	{
		lifestate = LifeState.Alive;
		base.health = startHealth;
		SetFlag(Flags.Reserved1, b: false);
	}

	public override void Die(HitInfo info = null)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		if (staticRespawn)
		{
			ClearTarget();
			Quaternion val = Quaternion.LookRotation(currentAimDir, Vector3.up);
			val = Quaternion.Euler(0f, ((Quaternion)(ref val)).eulerAngles.y, 0f);
			currentAimDir = val * Vector3.forward;
			((FacepunchBehaviour)this).Invoke((Action)SelfHeal, staticrepairseconds);
			lifestate = LifeState.Dead;
			base.health = 0f;
			SetFlag(Flags.Reserved1, b: true);
		}
		else
		{
			base.Die(info);
		}
	}

	public void FixedUpdate()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = currentAimDir;
		if (!currentTarget.IsUnityNull() && IsPowered())
		{
			GameObject val2 = projectileTest.Get();
			ServerProjectile component = val2.GetComponent<ServerProjectile>();
			float num = component.speed * currentTarget.SAMTargetType.speedMultiplier;
			Vector3 val3 = currentTarget.CenterPoint();
			float num2 = Vector3.Distance(val3, ((Component)eyePoint).transform.position);
			float num3 = num2 / num;
			Vector3 val4 = val3 + currentTarget.GetWorldVelocity() * num3;
			float num4 = Vector3.Distance(val4, ((Component)eyePoint).transform.position);
			num3 = num4 / num;
			val4 = val3 + currentTarget.GetWorldVelocity() * num3;
			Vector3 val5 = currentTarget.GetWorldVelocity();
			if (((Vector3)(ref val5)).magnitude > 0.1f)
			{
				float num5 = Mathf.Sin(Time.time * 3f) * (1f + num3 * 0.5f);
				Vector3 val6 = val4;
				val5 = currentTarget.GetWorldVelocity();
				val4 = val6 + ((Vector3)(ref val5)).normalized * num5;
			}
			val5 = val4 - ((Component)eyePoint).transform.position;
			currentAimDir = ((Vector3)(ref val5)).normalized;
			if (num2 > currentTarget.SAMTargetType.scanRadius)
			{
				ClearTarget();
			}
		}
		Quaternion val7 = Quaternion.LookRotation(currentAimDir, ((Component)this).transform.up);
		Vector3 eulerAngles = ((Quaternion)(ref val7)).eulerAngles;
		eulerAngles = BaseMountable.ConvertVector(eulerAngles);
		float num6 = Mathf.InverseLerp(0f, 90f, 0f - eulerAngles.x);
		float num7 = Mathf.Lerp(15f, -75f, num6);
		Quaternion localRotation = Quaternion.Euler(0f, eulerAngles.y, 0f);
		yaw.transform.localRotation = localRotation;
		Quaternion localRotation2 = pitch.transform.localRotation;
		float x = ((Quaternion)(ref localRotation2)).eulerAngles.x;
		localRotation2 = pitch.transform.localRotation;
		Quaternion localRotation3 = Quaternion.Euler(x, ((Quaternion)(ref localRotation2)).eulerAngles.y, num7);
		pitch.transform.localRotation = localRotation3;
		if (currentAimDir != val)
		{
			SendNetworkUpdate();
		}
	}

	public Vector3 GetAimDir()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		return currentAimDir;
	}

	public bool HasValidTarget()
	{
		return !currentTarget.IsUnityNull();
	}

	public override bool CanPickup(BasePlayer player)
	{
		if (!base.CanPickup(player))
		{
			return false;
		}
		if (base.isServer && pickup.requireEmptyInv && base.inventory != null && base.inventory.itemList.Count > 0)
		{
			return false;
		}
		return !HasAmmo();
	}

	public void TargetScan()
	{
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		if (!IsPowered())
		{
			lastTargetVisibleTime = 0f;
			return;
		}
		if (Time.time > lastTargetVisibleTime + 3f)
		{
			ClearTarget();
		}
		if (!staticRespawn)
		{
			int num = ((ammoItem != null && ammoItem.parent == base.inventory) ? ammoItem.amount : 0);
			bool flag = lastAmmoCount < lowAmmoThreshold;
			bool flag2 = num < lowAmmoThreshold;
			if (num != lastAmmoCount && flag != flag2)
			{
				MarkIODirty();
			}
			lastAmmoCount = num;
		}
		if (HasValidTarget() || IsDead())
		{
			return;
		}
		List<ISamSiteTarget> list = Pool.GetList<ISamSiteTarget>();
		if (!IsInDefenderMode())
		{
			AddTargetSet(list, 32768, targetTypeVehicle.scanRadius);
		}
		AddTargetSet(list, 1048576, targetTypeMissile.scanRadius);
		ISamSiteTarget samSiteTarget = null;
		foreach (ISamSiteTarget item in list)
		{
			if (item.isClient || item.CenterPoint().y < ((Component)eyePoint).transform.position.y || !item.IsVisible(((Component)eyePoint).transform.position, item.SAMTargetType.scanRadius * 2f) || !item.IsValidSAMTarget(staticRespawn))
			{
				continue;
			}
			samSiteTarget = item;
			break;
		}
		if (!samSiteTarget.IsUnityNull() && currentTarget != samSiteTarget)
		{
			lockOnTime = Time.time + 0.5f;
		}
		SetTarget(samSiteTarget);
		if (!currentTarget.IsUnityNull())
		{
			lastTargetVisibleTime = Time.time;
		}
		Pool.FreeList<ISamSiteTarget>(ref list);
		if (currentTarget.IsUnityNull())
		{
			((FacepunchBehaviour)this).CancelInvoke((Action)WeaponTick);
		}
		else
		{
			((FacepunchBehaviour)this).InvokeRandomized((Action)WeaponTick, 0f, 0.5f, 0.2f);
		}
		void AddTargetSet(List<ISamSiteTarget> allTargets, int layerMask, float scanRadius)
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			List<ISamSiteTarget> list2 = Pool.GetList<ISamSiteTarget>();
			Vis.Entities(((Component)eyePoint).transform.position, scanRadius, list2, layerMask, (QueryTriggerInteraction)1);
			allTargets.AddRange(list2);
			Pool.FreeList<ISamSiteTarget>(ref list2);
		}
	}

	public virtual bool HasAmmo()
	{
		return staticRespawn || (ammoItem != null && ammoItem.amount > 0 && ammoItem.parent == base.inventory);
	}

	public void Reload()
	{
		if (staticRespawn)
		{
			return;
		}
		for (int i = 0; i < base.inventory.itemList.Count; i++)
		{
			Item item = base.inventory.itemList[i];
			if (item != null && item.info.itemid == ammoType.itemid && item.amount > 0)
			{
				ammoItem = item;
				return;
			}
		}
		ammoItem = null;
	}

	public void EnsureReloaded()
	{
		if (!HasAmmo())
		{
			Reload();
		}
	}

	public bool IsReloading()
	{
		return ((FacepunchBehaviour)this).IsInvoking((Action)Reload);
	}

	public void WeaponTick()
	{
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		if (IsDead() || Time.time < lockOnTime || Time.time < nextBurstTime)
		{
			return;
		}
		if (!IsPowered())
		{
			firedCount = 0;
			return;
		}
		if (firedCount >= 6)
		{
			float timeBetweenBursts = mostRecentTargetType.timeBetweenBursts;
			nextBurstTime = Time.time + timeBetweenBursts;
			firedCount = 0;
			return;
		}
		EnsureReloaded();
		if (HasAmmo())
		{
			bool flag = ammoItem != null && ammoItem.amount == lowAmmoThreshold;
			if (!staticRespawn && ammoItem != null)
			{
				ammoItem.UseItem();
			}
			firedCount++;
			float speedMultiplier = 1f;
			if (!currentTarget.IsUnityNull())
			{
				speedMultiplier = currentTarget.SAMTargetType.speedMultiplier;
			}
			FireProjectile(tubes[currentTubeIndex].position, currentAimDir, speedMultiplier);
			Effect.server.Run(muzzleFlashTest.resourcePath, this, StringPool.Get("Tube " + (currentTubeIndex + 1)), Vector3.zero, Vector3.up);
			currentTubeIndex++;
			if (currentTubeIndex >= tubes.Length)
			{
				currentTubeIndex = 0;
			}
			if (flag)
			{
				MarkIODirty();
			}
		}
	}

	public void FireProjectile(Vector3 origin, Vector3 direction, float speedMultiplier)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		BaseEntity baseEntity = GameManager.server.CreateEntity(projectileTest.resourcePath, origin, Quaternion.LookRotation(direction, Vector3.up));
		if (!((Object)(object)baseEntity == (Object)null))
		{
			baseEntity.creatorEntity = this;
			ServerProjectile component = ((Component)baseEntity).GetComponent<ServerProjectile>();
			if (Object.op_Implicit((Object)(object)component))
			{
				component.InitializeVelocity(GetInheritedProjectileVelocity(direction) + direction * component.speed * speedMultiplier);
			}
			baseEntity.Spawn();
		}
	}

	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		int num = Mathf.Min(1, GetCurrentEnergy());
		return outputSlot switch
		{
			0 => (!currentTarget.IsUnityNull()) ? num : 0, 
			1 => (ammoItem != null && ammoItem.amount < lowAmmoThreshold && ammoItem.parent == base.inventory) ? num : 0, 
			2 => (!HasAmmo()) ? num : 0, 
			_ => GetCurrentEnergy(), 
		};
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	[RPC_Server.CallsPerSecond(1uL)]
	private void ToggleDefenderMode(RPCMessage msg)
	{
		if (staticRespawn)
		{
			return;
		}
		BasePlayer player = msg.player;
		if (!((Object)(object)player == (Object)null) && player.CanBuild())
		{
			bool flag = msg.read.Bit();
			if (flag != IsInDefenderMode())
			{
				SetFlag(Flag_DefenderMode, flag);
			}
		}
	}
}
