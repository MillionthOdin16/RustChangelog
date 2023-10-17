using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Assertions;

public class AttackHelicopter : PlayerHelicopter
{
	public class GunnerInputState
	{
		public bool fire1;

		public bool fire2;

		public bool reload;

		public Ray eyeRay;

		public Vector3 eyePos;

		public void Reset()
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			fire1 = false;
			fire2 = false;
			reload = false;
			eyeRay = default(Ray);
		}
	}

	[Header("Attack Helicopter")]
	public Transform gunnerEyePos;

	[SerializeField]
	private Transform turbofanBone;

	[SerializeField]
	private GameObjectRef turretStoragePrefab;

	[SerializeField]
	private GameObjectRef rocketStoragePrefab;

	[SerializeField]
	private GameObjectRef gunCamUIPrefab;

	[SerializeField]
	private GameObjectRef gunCamUIDialogPrefab;

	[SerializeField]
	private GameObject gunCamUIParent;

	[SerializeField]
	private ParticleSystemContainer fxLightDamage;

	[SerializeField]
	private ParticleSystemContainer fxMediumDamage;

	[SerializeField]
	private ParticleSystemContainer fxHeavyDamage;

	[SerializeField]
	private SoundDefinition damagedLightLoop;

	[SerializeField]
	private SoundDefinition damagedHeavyLoop;

	[SerializeField]
	private GameObject damageSoundTarget;

	[SerializeField]
	private MeshRenderer monitorStaticRenderer;

	[SerializeField]
	private Material monitorStatic;

	[SerializeField]
	private Material monitorStaticSafeZone;

	[Header("Heli Pilot Flares")]
	[SerializeField]
	private GameObjectRef flareFireFX;

	[SerializeField]
	private GameObjectRef pilotFlare;

	[SerializeField]
	private Transform leftFlareLaunchPos;

	[SerializeField]
	private Transform rightFlareLaunchPos;

	[SerializeField]
	private float flareLaunchVel = 10f;

	[Header("Heli Turret")]
	public Vector2 turretPitchClamp = new Vector2(-15f, 70f);

	public Vector2 turretYawClamp = new Vector2(-90f, 90f);

	protected const Flags IN_GUNNER_VIEW_FLAG = Flags.Reserved9;

	protected const Flags IN_SAFE_ZONE_FLAG = Flags.Reserved10;

	protected static int headingGaugeIndex = Animator.StringToHash("headingFraction");

	protected static int altGaugeIndex = Animator.StringToHash("altFraction");

	protected int altShakeIndex = -1;

	private EntityRef<AttackHelicopterTurret> turretInstance;

	private EntityRef<AttackHelicopterRockets> rocketsInstance;

	private GunnerInputState gunnerInputState = new GunnerInputState();

	private TimeSince timeSinceLastGunnerInput;

	private TimeSince timeSinceFailedWeaponFireRPC;

	private TimeSince timeSinceFailedFlareRPC;

	public bool HasSafeZoneFlag => HasFlag(Flags.Reserved10);

	public bool GunnerIsInGunnerView => HasFlag(Flags.Reserved9);

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("AttackHelicopter.OnRpcMessage", 0);
		try
		{
			if (rpc == 3309981499u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - RPC_CloseGunnerView "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_CloseGunnerView", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(3309981499u, "RPC_CloseGunnerView", this, player, 3f))
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
						val3 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage msg2 = rPCMessage;
							RPC_CloseGunnerView(msg2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in RPC_CloseGunnerView");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 1427416040 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - RPC_OpenGunnerView "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_OpenGunnerView", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(1427416040u, "RPC_OpenGunnerView", this, player, 3f))
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
						val3 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage msg3 = rPCMessage;
							RPC_OpenGunnerView(msg3);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in RPC_OpenGunnerView");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 4185921214u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - RPC_OpenStorage "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_OpenStorage", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(4185921214u, "RPC_OpenStorage", this, player, 3f))
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
						val3 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage msg4 = rPCMessage;
							RPC_OpenStorage(msg4);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex3)
					{
						Debug.LogException(ex3);
						player.Kick("RPC Error in RPC_OpenStorage");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 148009183 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - RPC_OpenTurret "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_OpenTurret", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(148009183u, "RPC_OpenTurret", this, player, 3f))
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
						val3 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage msg5 = rPCMessage;
							RPC_OpenTurret(msg5);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex4)
					{
						Debug.LogException(ex4);
						player.Kick("RPC Error in RPC_OpenTurret");
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

	public override void OnFlagsChanged(Flags old, Flags next)
	{
		base.OnFlagsChanged(old, next);
		if (base.isServer)
		{
			VehicleEngineController<PlayerHelicopter>.EngineState engineState = engineController.EngineStateFrom(old);
			if (engineController.CurEngineState != engineState)
			{
				SetFlag(Flags.Reserved5, engineController.IsStartingOrOn);
			}
		}
	}

	protected override void OnChildAdded(BaseEntity child)
	{
		base.OnChildAdded(child);
		if (child.prefabID == turretStoragePrefab.GetEntity().prefabID)
		{
			AttackHelicopterTurret attackHelicopterTurret = (AttackHelicopterTurret)child;
			turretInstance.Set(attackHelicopterTurret);
			attackHelicopterTurret.owner = this;
		}
		if (child.prefabID == rocketStoragePrefab.GetEntity().prefabID)
		{
			AttackHelicopterRockets attackHelicopterRockets = (AttackHelicopterRockets)child;
			rocketsInstance.Set(attackHelicopterRockets);
			attackHelicopterRockets.owner = this;
		}
	}

	public override void Load(LoadInfo info)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		base.Load(info);
		if (info.msg.attackHeli != null)
		{
			turretInstance.uid = info.msg.attackHeli.turretID;
			rocketsInstance.uid = info.msg.attackHeli.rocketsID;
		}
	}

	public AttackHelicopterTurret GetTurret()
	{
		AttackHelicopterTurret attackHelicopterTurret = turretInstance.Get(base.isServer);
		if (attackHelicopterTurret.IsValid())
		{
			return attackHelicopterTurret;
		}
		return null;
	}

	public AttackHelicopterRockets GetRockets()
	{
		AttackHelicopterRockets attackHelicopterRockets = rocketsInstance.Get(base.isServer);
		if (attackHelicopterRockets.IsValid())
		{
			return attackHelicopterRockets;
		}
		return null;
	}

	public override void PilotInput(InputState inputState, BasePlayer player)
	{
		base.PilotInput(inputState, player);
		if (!IsOn())
		{
			return;
		}
		bool num = inputState.IsDown(BUTTON.FIRE_PRIMARY);
		bool flag = inputState.WasJustPressed(BUTTON.FIRE_SECONDARY);
		if (num)
		{
			AttackHelicopterRockets rockets = GetRockets();
			if (rockets.TryFireRocket(player))
			{
				MarkAllMountedPlayersAsHostile();
			}
			else if (inputState.WasJustPressed(BUTTON.FIRE_PRIMARY))
			{
				WeaponFireFailed(rockets.GetAmmoAmount(), player);
			}
		}
		if (flag && !TryFireFlare())
		{
			FlareFireFailed(player);
		}
	}

	public override void PassengerInput(InputState inputState, BasePlayer player)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		base.PassengerInput(inputState, player);
		timeSinceLastGunnerInput = TimeSince.op_Implicit(0f);
		gunnerInputState.fire1 = inputState.IsDown(BUTTON.FIRE_PRIMARY);
		gunnerInputState.fire2 = inputState.IsDown(BUTTON.FIRE_SECONDARY);
		gunnerInputState.reload = inputState.IsDown(BUTTON.RELOAD);
		((Ray)(ref gunnerInputState.eyeRay)).direction = Quaternion.Euler(inputState.current.aimAngles) * Vector3.forward;
		((Ray)(ref gunnerInputState.eyeRay)).origin = player.eyes.position + ((Ray)(ref gunnerInputState.eyeRay)).direction * 0.5f;
		if (IsOn() && GunnerIsInGunnerView)
		{
			AttackHelicopterTurret turret = GetTurret();
			if (turret.InputTick(gunnerInputState))
			{
				MarkAllMountedPlayersAsHostile();
			}
			else if (inputState.WasJustPressed(BUTTON.FIRE_PRIMARY))
			{
				turret.GetAmmoAmounts(out var _, out var available);
				WeaponFireFailed(available, player);
			}
			AttackHelicopterRockets rockets = GetRockets();
			if (rockets.InputTick(gunnerInputState, player))
			{
				MarkAllMountedPlayersAsHostile();
			}
			else if (inputState.WasJustPressed(BUTTON.FIRE_SECONDARY))
			{
				WeaponFireFailed(rockets.GetAmmoAmount(), player);
			}
		}
	}

	private void WeaponFireFailed(int ammo, BasePlayer player)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		if (!(TimeSince.op_Implicit(timeSinceFailedWeaponFireRPC) <= 1f) && ammo <= 0)
		{
			ClientRPCPlayer(null, player, "WeaponFireFailed");
			timeSinceFailedWeaponFireRPC = TimeSince.op_Implicit(0f);
		}
	}

	private void FlareFireFailed(BasePlayer player)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		if (!(TimeSince.op_Implicit(timeSinceFailedFlareRPC) <= 1f))
		{
			ClientRPCPlayer(null, player, "FlareFireFailed");
			timeSinceFailedFlareRPC = TimeSince.op_Implicit(0f);
		}
	}

	public override void VehicleFixedUpdate()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		base.VehicleFixedUpdate();
		if (TimeSince.op_Implicit(timeSinceLastGunnerInput) > 0.5f)
		{
			gunnerInputState.Reset();
		}
	}

	public override bool EnterTrigger(TriggerBase trigger)
	{
		bool result = base.EnterTrigger(trigger);
		SetFlag(Flags.Reserved10, InSafeZone());
		return result;
	}

	public override void LeaveTrigger(TriggerBase trigger)
	{
		base.LeaveTrigger(trigger);
		SetFlag(Flags.Reserved10, InSafeZone());
	}

	public override void PrePlayerDismount(BasePlayer player, BaseMountable seat)
	{
		base.PrePlayerDismount(player, seat);
		if (HasFlag(Flags.Reserved9) && IsPassenger(player))
		{
			SetFlag(Flags.Reserved9, b: false);
		}
	}

	internal override void DoServerDestroy()
	{
		if (vehicle.vehiclesdroploot)
		{
			if (turretInstance.IsValid(base.isServer))
			{
				turretInstance.Get(base.isServer).DropItems();
			}
			if (rocketsInstance.IsValid(base.isServer))
			{
				rocketsInstance.Get(base.isServer).DropItems();
			}
		}
		base.DoServerDestroy();
	}

	public override void Save(SaveInfo info)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		base.Save(info);
		info.msg.attackHeli = Pool.Get<AttackHeli>();
		info.msg.attackHeli.turretID = turretInstance.uid;
		info.msg.attackHeli.rocketsID = rocketsInstance.uid;
	}

	private void MarkAllMountedPlayersAsHostile()
	{
		foreach (MountPointInfo mountPoint in mountPoints)
		{
			if ((Object)(object)mountPoint.mountable != (Object)null)
			{
				BasePlayer mounted = mountPoint.mountable.GetMounted();
				if ((Object)(object)mounted != (Object)null)
				{
					mounted.MarkHostileFor();
				}
			}
		}
	}

	public override bool AdminFixUp(int tier)
	{
		if (!base.AdminFixUp(tier))
		{
			return false;
		}
		AttackHelicopterTurret turret = GetTurret();
		if ((Object)(object)turret != (Object)null && (Object)(object)turret.GetAttachedHeldEntity() == (Object)null)
		{
			ItemDefinition itemToCreate;
			ItemDefinition itemDefinition;
			switch (tier)
			{
			case 1:
				itemToCreate = ItemManager.FindItemDefinition("hmlmg");
				itemDefinition = ItemManager.FindItemDefinition("ammo.rifle");
				break;
			case 2:
				itemToCreate = ItemManager.FindItemDefinition("rifle.ak");
				itemDefinition = ItemManager.FindItemDefinition("ammo.rifle");
				break;
			default:
				itemToCreate = ItemManager.FindItemDefinition("lmg.m249");
				itemDefinition = ItemManager.FindItemDefinition("ammo.rifle");
				break;
			}
			turret.inventory.AddItem(itemToCreate, 1, 0uL);
			turret.GetAmmoAmounts(out var _, out var available);
			int num = itemDefinition.stackable * (turret.inventory.capacity - 1);
			turret.forceAcceptAmmo = true;
			if (available < num)
			{
				int num2 = num - available;
				while (num2 > 0)
				{
					int num3 = Mathf.Min(num2, itemDefinition.stackable);
					turret.inventory.AddItem(itemDefinition, itemDefinition.stackable, 0uL);
					num2 -= num3;
				}
			}
			turret.forceAcceptAmmo = false;
		}
		AttackHelicopterRockets rockets = GetRockets();
		if ((Object)(object)rockets != (Object)null)
		{
			ItemDefinition itemDefinition2 = ItemManager.FindItemDefinition("flare");
			ItemDefinition itemDefinition3 = tier switch
			{
				1 => ItemManager.FindItemDefinition("ammo.rocket.hv"), 
				2 => ItemManager.FindItemDefinition("ammo.rocket.hv"), 
				_ => ItemManager.FindItemDefinition("ammo.rocket.fire"), 
			};
			int num4 = itemDefinition2.stackable * 2;
			int ammoAmount = rockets.GetAmmoAmount();
			int num5 = itemDefinition3.stackable * (rockets.inventory.capacity - num4);
			if (ammoAmount < num5)
			{
				int num6 = num5 - ammoAmount;
				while (num6 > 0)
				{
					int num7 = Mathf.Min(num6, itemDefinition3.stackable);
					rockets.inventory.AddItem(itemDefinition3, itemDefinition3.stackable, 0uL);
					num6 -= num7;
				}
			}
			rockets.inventory.AddItem(itemDefinition2, num4, 0uL, ItemContainer.LimitStack.All);
		}
		return true;
	}

	private bool TryFireFlare()
	{
		AttackHelicopterRockets rockets = GetRockets();
		if ((Object)(object)rockets != (Object)null && rockets.TryTakeFlare())
		{
			LaunchFlare();
			return true;
		}
		return false;
	}

	private void LaunchFlare()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		Effect.server.Run(flareFireFX.resourcePath, this, StringPool.Get("FlareLaunchPos"), Vector3.zero, Vector3.zero);
		Object.Instantiate<GameObject>(pilotFlare.Get(), leftFlareLaunchPos.position, Quaternion.identity).GetComponent<AttackHeliPilotFlare>().Init(-((Component)this).transform.right * flareLaunchVel);
		Object.Instantiate<GameObject>(pilotFlare.Get(), rightFlareLaunchPos.position, Quaternion.identity).GetComponent<AttackHeliPilotFlare>().Init(((Component)this).transform.right * flareLaunchVel);
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	public void RPC_OpenTurret(RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if (!CanBeLooted(player) || player.isMounted || (IsSafe() && (Object)(object)player != (Object)(object)creatorEntity))
		{
			return;
		}
		StorageContainer turret = GetTurret();
		if (!((Object)(object)turret == (Object)null))
		{
			BasePlayer driver = GetDriver();
			if (!((Object)(object)driver != (Object)null) || !((Object)(object)driver != (Object)(object)player))
			{
				turret.PlayerOpenLoot(player);
			}
		}
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	public void RPC_OpenStorage(RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if (!CanBeLooted(player) || player.isMounted || (IsSafe() && (Object)(object)player != (Object)(object)creatorEntity))
		{
			return;
		}
		StorageContainer rockets = GetRockets();
		if (!((Object)(object)rockets == (Object)null))
		{
			BasePlayer driver = GetDriver();
			if (!((Object)(object)driver != (Object)null) || !((Object)(object)driver != (Object)(object)player))
			{
				rockets.PlayerOpenLoot(player);
			}
		}
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	public void RPC_OpenGunnerView(RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if (CanBeLooted(player) && IsOn() && IsPassenger(player) && !InSafeZone())
		{
			SetFlag(Flags.Reserved9, b: true);
		}
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	public void RPC_CloseGunnerView(RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if (IsPassenger(player))
		{
			SetFlag(Flags.Reserved9, b: false);
		}
	}
}
