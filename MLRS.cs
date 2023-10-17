using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

public class MLRS : BaseMountable
{
	[Serializable]
	public class RocketTube
	{
		public Vector3 firingOffset;

		public Transform hinge;

		public Renderer rocket;
	}

	private struct TheoreticalProjectile
	{
		public Vector3 pos;

		public Vector3 forward;

		public float gravityMult;

		public TheoreticalProjectile(Vector3 pos, Vector3 forward, float gravityMult)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			this.pos = pos;
			this.forward = forward;
			this.gravityMult = gravityMult;
		}
	}

	public const string MLRS_PLAYER_KILL_STAT = "mlrs_kills";

	private float leftRightInput;

	private float upDownInput;

	private Vector3 lastSentTargetHitPos;

	private Vector3 lastSentTrueHitPos;

	private int nextRocketIndex = 0;

	private EntityRef rocketOwnerRef;

	private TimeSince timeSinceBroken = default(TimeSince);

	private int radiusModIndex = 0;

	private float[] radiusMods = new float[4]
	{
		0.1f,
		0.2f,
		1f / 3f,
		2f / 3f
	};

	private Vector3 trueTargetHitPos;

	[Header("MLRS Components")]
	[SerializeField]
	private GameObjectRef rocketStoragePrefab;

	[SerializeField]
	private GameObjectRef dashboardStoragePrefab;

	[Header("MLRS Rotation")]
	[SerializeField]
	private Transform hRotator;

	[SerializeField]
	private float hRotSpeed = 25f;

	[SerializeField]
	private Transform vRotator;

	[SerializeField]
	private float vRotSpeed = 10f;

	[SerializeField]
	[Range(50f, 90f)]
	private float vRotMax = 85f;

	[SerializeField]
	private Transform hydraulics;

	[Header("MLRS Weaponry")]
	[Tooltip("Minimum distance from the MLRS to a targeted hit point. In metres.")]
	[SerializeField]
	public float minRange = 200f;

	[Tooltip("The size of the area that the rockets may hit, minus rocket damage radius.")]
	[SerializeField]
	public float targetAreaRadius = 30f;

	[SerializeField]
	private GameObjectRef mlrsRocket;

	[SerializeField]
	public Transform firingPoint;

	[SerializeField]
	private RocketTube[] rocketTubes;

	[Header("MLRS Dashboard/FX")]
	[SerializeField]
	private GameObject screensChild;

	[SerializeField]
	private Transform leftHandGrip;

	[SerializeField]
	private Transform leftJoystick;

	[SerializeField]
	private Transform rightHandGrip;

	[SerializeField]
	private Transform rightJoystick;

	[SerializeField]
	private Transform controlKnobHeight;

	[SerializeField]
	private Transform controlKnobAngle;

	[SerializeField]
	private GameObjectRef uiDialogPrefab;

	[SerializeField]
	private Light fireButtonLight;

	[SerializeField]
	private GameObject brokenDownEffect;

	[SerializeField]
	private ParticleSystem topScreenShutdown;

	[SerializeField]
	private ParticleSystem bottomScreenShutdown;

	[ServerVar(Help = "How many minutes before the MLRS recovers from use and can be used again")]
	public static float brokenDownMinutes = 10f;

	public const Flags FLAG_FIRING_ROCKETS = Flags.Reserved6;

	public const Flags FLAG_HAS_AIMING_MODULE = Flags.Reserved8;

	private EntityRef rocketStorageInstance;

	private EntityRef dashboardStorageInstance;

	private float rocketBaseGravity;

	private float rocketSpeed;

	private bool isInitialLoad = true;

	public Vector3 UserTargetHitPos { get; private set; }

	public Vector3 TrueHitPos { get; private set; }

	public bool HasAimingModule => HasFlag(Flags.Reserved8);

	private bool CanBeUsed => HasAimingModule && !IsBroken();

	private bool CanFire => CanBeUsed && RocketAmmoCount > 0 && !IsFiringRockets && !IsRealigning;

	private float HRotation
	{
		get
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			return hRotator.eulerAngles.y;
		}
		set
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			Vector3 eulerAngles = hRotator.eulerAngles;
			eulerAngles.y = value;
			hRotator.eulerAngles = eulerAngles;
		}
	}

	private float VRotation
	{
		get
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			return vRotator.localEulerAngles.x;
		}
		set
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			Vector3 localEulerAngles = vRotator.localEulerAngles;
			if (value < 0f)
			{
				localEulerAngles.x = Mathf.Clamp(value, 0f - vRotMax, 0f);
			}
			else if (value > 0f)
			{
				localEulerAngles.x = Mathf.Clamp(value, 360f - vRotMax, 360f);
			}
			vRotator.localEulerAngles = localEulerAngles;
		}
	}

	public float CurGravityMultiplier { get; private set; }

	public int RocketAmmoCount { get; private set; }

	public bool IsRealigning { get; private set; }

	public bool IsFiringRockets => HasFlag(Flags.Reserved6);

	public float RocketDamageRadius { get; private set; }

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("MLRS.OnRpcMessage", 0);
		try
		{
			if (rpc == 455279877 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_Fire_Rockets "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_Fire_Rockets", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.MaxDistance.Test(455279877u, "RPC_Fire_Rockets", this, player, 3f))
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
							RPC_Fire_Rockets(msg2);
						}
						finally
						{
							((IDisposable)val4)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in RPC_Fire_Rockets");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 751446792 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_Open_Dashboard "));
				}
				TimeWarning val5 = TimeWarning.New("RPC_Open_Dashboard", 0);
				try
				{
					TimeWarning val6 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.MaxDistance.Test(751446792u, "RPC_Open_Dashboard", this, player, 3f))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val6)?.Dispose();
					}
					try
					{
						TimeWarning val7 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage msg3 = rPCMessage;
							RPC_Open_Dashboard(msg3);
						}
						finally
						{
							((IDisposable)val7)?.Dispose();
						}
					}
					catch (Exception ex2)
					{
						Debug.LogException(ex2);
						player.Kick("RPC Error in RPC_Open_Dashboard");
					}
				}
				finally
				{
					((IDisposable)val5)?.Dispose();
				}
				return true;
			}
			if (rpc == 1311007340 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_Open_Rockets "));
				}
				TimeWarning val8 = TimeWarning.New("RPC_Open_Rockets", 0);
				try
				{
					TimeWarning val9 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.MaxDistance.Test(1311007340u, "RPC_Open_Rockets", this, player, 3f))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val9)?.Dispose();
					}
					try
					{
						TimeWarning val10 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage msg4 = rPCMessage;
							RPC_Open_Rockets(msg4);
						}
						finally
						{
							((IDisposable)val10)?.Dispose();
						}
					}
					catch (Exception ex3)
					{
						Debug.LogException(ex3);
						player.Kick("RPC Error in RPC_Open_Rockets");
					}
				}
				finally
				{
					((IDisposable)val8)?.Dispose();
				}
				return true;
			}
			if (rpc == 858951307 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_SetTargetHitPos "));
				}
				TimeWarning val11 = TimeWarning.New("RPC_SetTargetHitPos", 0);
				try
				{
					TimeWarning val12 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.MaxDistance.Test(858951307u, "RPC_SetTargetHitPos", this, player, 3f))
						{
							return true;
						}
					}
					finally
					{
						((IDisposable)val12)?.Dispose();
					}
					try
					{
						TimeWarning val13 = TimeWarning.New("Call", 0);
						try
						{
							RPCMessage rPCMessage = default(RPCMessage);
							rPCMessage.connection = msg.connection;
							rPCMessage.player = player;
							rPCMessage.read = msg.read;
							RPCMessage msg5 = rPCMessage;
							RPC_SetTargetHitPos(msg5);
						}
						finally
						{
							((IDisposable)val13)?.Dispose();
						}
					}
					catch (Exception ex4)
					{
						Debug.LogException(ex4);
						player.Kick("RPC Error in RPC_SetTargetHitPos");
					}
				}
				finally
				{
					((IDisposable)val11)?.Dispose();
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

	protected override void OnChildAdded(BaseEntity child)
	{
		base.OnChildAdded(child);
		if (base.isServer)
		{
			if (child.prefabID == rocketStoragePrefab.GetEntity().prefabID)
			{
				rocketStorageInstance.Set(child);
			}
			if (child.prefabID == dashboardStoragePrefab.GetEntity().prefabID)
			{
				dashboardStorageInstance.Set(child);
			}
		}
	}

	public override void VehicleFixedUpdate()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		base.VehicleFixedUpdate();
		if (IsBroken())
		{
			if (!(TimeSince.op_Implicit(timeSinceBroken) >= brokenDownMinutes * 60f))
			{
				SetFlag(Flags.Reserved8, TryGetAimingModule(out var _));
				return;
			}
			SetRepaired();
		}
		int rocketAmmoCount = RocketAmmoCount;
		UpdateStorageState();
		if (CanBeUsed && AnyMounted())
		{
			Vector3 userTargetHitPos = UserTargetHitPos;
			userTargetHitPos += Vector3.forward * upDownInput * 75f * Time.fixedDeltaTime;
			userTargetHitPos += Vector3.right * leftRightInput * 75f * Time.fixedDeltaTime;
			SetUserTargetHitPos(userTargetHitPos);
		}
		if (!IsFiringRockets)
		{
			HitPosToRotation(trueTargetHitPos, out var hRot, out var vRot, out var g);
			float num = g / (0f - Physics.gravity.y);
			IsRealigning = Mathf.Abs(Mathf.DeltaAngle(VRotation, vRot)) > 0.001f || Mathf.Abs(Mathf.DeltaAngle(HRotation, hRot)) > 0.001f || !Mathf.Approximately(CurGravityMultiplier, num);
			if (IsRealigning)
			{
				if (isInitialLoad)
				{
					VRotation = vRot;
					HRotation = hRot;
					isInitialLoad = false;
				}
				else
				{
					VRotation = Mathf.MoveTowardsAngle(VRotation, vRot, Time.deltaTime * vRotSpeed);
					HRotation = Mathf.MoveTowardsAngle(HRotation, hRot, Time.deltaTime * hRotSpeed);
				}
				CurGravityMultiplier = num;
				TrueHitPos = GetTrueHitPos();
			}
		}
		if (UserTargetHitPos != lastSentTargetHitPos || TrueHitPos != lastSentTrueHitPos || RocketAmmoCount != rocketAmmoCount)
		{
			SendNetworkUpdate();
		}
	}

	private Vector3 GetTrueHitPos()
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		Vector3 position = firingPoint.position;
		Vector3 forward = firingPoint.forward;
		TheoreticalProjectile projectile = new TheoreticalProjectile(position, ((Vector3)(ref forward)).normalized * rocketSpeed, CurGravityMultiplier);
		int num = 0;
		float dt = ((projectile.forward.y > 0f) ? 2f : 0.66f);
		while (!NextRayHitSomething(ref projectile, dt) && (float)num < 128f)
		{
			num++;
		}
		return projectile.pos;
	}

	private bool NextRayHitSomething(ref TheoreticalProjectile projectile, float dt)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		float num = Physics.gravity.y * projectile.gravityMult;
		Vector3 pos = projectile.pos;
		float num2 = Vector3Ex.MagnitudeXZ(projectile.forward) * dt;
		float num3 = projectile.forward.y * dt + num * dt * dt * 0.5f;
		Vector2 val = Vector3Ex.XZ2D(projectile.forward);
		Vector2 val2 = ((Vector2)(ref val)).normalized * num2;
		Vector3 val3 = default(Vector3);
		((Vector3)(ref val3))._002Ector(val2.x, num3, val2.y);
		ref Vector3 pos2 = ref projectile.pos;
		pos2 += val3;
		float y = projectile.forward.y + num * dt;
		projectile.forward.y = y;
		RaycastHit hit = default(RaycastHit);
		if (Physics.Linecast(pos, projectile.pos, ref hit, 1084293393, (QueryTriggerInteraction)1))
		{
			projectile.pos = ((RaycastHit)(ref hit)).point;
			BaseEntity entity = hit.GetEntity();
			bool flag = (Object)(object)entity != (Object)null && entity.EqualNetID((BaseNetworkable)this);
			if (flag)
			{
				ref Vector3 pos3 = ref projectile.pos;
				pos3 += projectile.forward * 1f;
			}
			return !flag;
		}
		return false;
	}

	private float GetSurfaceHeight(Vector3 pos)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		float height = TerrainMeta.HeightMap.GetHeight(pos);
		float height2 = TerrainMeta.WaterMap.GetHeight(pos);
		return Mathf.Max(height, height2);
	}

	private void SetRepaired()
	{
		SetFlag(Flags.Broken, b: false);
	}

	public override void PlayerServerInput(InputState inputState, BasePlayer player)
	{
		if (inputState.IsDown(BUTTON.FORWARD))
		{
			upDownInput = 1f;
		}
		else if (inputState.IsDown(BUTTON.BACKWARD))
		{
			upDownInput = -1f;
		}
		else
		{
			upDownInput = 0f;
		}
		if (inputState.IsDown(BUTTON.LEFT))
		{
			leftRightInput = -1f;
		}
		else if (inputState.IsDown(BUTTON.RIGHT))
		{
			leftRightInput = 1f;
		}
		else
		{
			leftRightInput = 0f;
		}
	}

	public override void Save(SaveInfo info)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		base.Save(info);
		info.msg.mlrs = Pool.Get<MLRS>();
		info.msg.mlrs.targetPos = UserTargetHitPos;
		info.msg.mlrs.curHitPos = TrueHitPos;
		info.msg.mlrs.rocketStorageID = rocketStorageInstance.uid;
		info.msg.mlrs.dashboardStorageID = dashboardStorageInstance.uid;
		info.msg.mlrs.ammoCount = (uint)RocketAmmoCount;
		lastSentTargetHitPos = UserTargetHitPos;
		lastSentTrueHitPos = TrueHitPos;
	}

	public bool AdminFixUp()
	{
		if (IsDead() || IsFiringRockets)
		{
			return false;
		}
		StorageContainer dashboardContainer = GetDashboardContainer();
		if (!HasAimingModule)
		{
			dashboardContainer.inventory.AddItem(ItemManager.FindItemDefinition("aiming.module.mlrs"), 1, 0uL);
		}
		StorageContainer rocketContainer = GetRocketContainer();
		ItemDefinition itemDefinition = ItemManager.FindItemDefinition("ammo.rocket.mlrs");
		if (RocketAmmoCount < rocketContainer.inventory.capacity * itemDefinition.stackable)
		{
			int num = itemDefinition.stackable * rocketContainer.inventory.capacity - RocketAmmoCount;
			while (num > 0)
			{
				int num2 = Mathf.Min(num, itemDefinition.stackable);
				rocketContainer.inventory.AddItem(itemDefinition, itemDefinition.stackable, 0uL);
				num -= num2;
			}
		}
		SetRepaired();
		SendNetworkUpdate();
		return true;
	}

	private void Fire(BasePlayer owner)
	{
		UpdateStorageState();
		if (CanFire && !((Object)(object)_mounted == (Object)null))
		{
			SetFlag(Flags.Reserved6, b: true);
			radiusModIndex = 0;
			nextRocketIndex = Mathf.Min(RocketAmmoCount - 1, rocketTubes.Length - 1);
			rocketOwnerRef.Set(owner);
			((FacepunchBehaviour)this).InvokeRepeating((Action)FireNextRocket, 0f, 0.5f);
		}
	}

	private void EndFiring()
	{
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		((FacepunchBehaviour)this).CancelInvoke((Action)FireNextRocket);
		rocketOwnerRef.Set(null);
		if (TryGetAimingModule(out var item))
		{
			item.LoseCondition(1f);
		}
		SetFlag(Flags.Reserved6, b: false, recursive: false, networkupdate: false);
		SetFlag(Flags.Broken, b: true, recursive: false, networkupdate: false);
		SendNetworkUpdate_Flags();
		timeSinceBroken = TimeSince.op_Implicit(0f);
	}

	private void FireNextRocket()
	{
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		RocketAmmoCount = GetRocketContainer().inventory.GetAmmoAmount((AmmoTypes)2048);
		if (nextRocketIndex < 0 || nextRocketIndex >= RocketAmmoCount || IsBroken())
		{
			EndFiring();
			return;
		}
		StorageContainer rocketContainer = GetRocketContainer();
		Vector3 firingPos = firingPoint.position + firingPoint.rotation * rocketTubes[nextRocketIndex].firingOffset;
		float num = 1f;
		if (radiusModIndex < radiusMods.Length)
		{
			num = radiusMods[radiusModIndex];
		}
		radiusModIndex++;
		Vector2 val = Random.insideUnitCircle * (targetAreaRadius - RocketDamageRadius) * num;
		Vector3 targetPos = TrueHitPos + new Vector3(val.x, 0f, val.y);
		float g;
		Vector3 aimToTarget = GetAimToTarget(targetPos, out g);
		if (TryFireProjectile(rocketContainer, (AmmoTypes)2048, firingPos, aimToTarget, rocketOwnerRef.Get(serverside: true) as BasePlayer, 0f, 0f, out var projectile))
		{
			projectile.gravityModifier = g / (0f - Physics.gravity.y);
			nextRocketIndex--;
		}
		else
		{
			EndFiring();
		}
	}

	private void UpdateStorageState()
	{
		Item item;
		bool b = TryGetAimingModule(out item);
		SetFlag(Flags.Reserved8, b);
		RocketAmmoCount = GetRocketContainer().inventory.GetAmmoAmount((AmmoTypes)2048);
	}

	private bool TryGetAimingModule(out Item item)
	{
		ItemContainer inventory = GetDashboardContainer().inventory;
		if (!inventory.IsEmpty())
		{
			item = inventory.itemList[0];
			return true;
		}
		item = null;
		return false;
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	public void RPC_SetTargetHitPos(RPCMessage msg)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		BasePlayer player = msg.player;
		if (PlayerIsMounted(player))
		{
			SetUserTargetHitPos(msg.read.Vector3());
		}
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	public void RPC_Fire_Rockets(RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if (PlayerIsMounted(player))
		{
			Fire(player);
		}
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	public void RPC_Open_Rockets(RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if (!((Object)(object)player == (Object)null) && CanBeLooted(player))
		{
			IItemContainerEntity rocketContainer = GetRocketContainer();
			if (!rocketContainer.IsUnityNull())
			{
				rocketContainer.PlayerOpenLoot(player, "", doPositionChecks: false);
			}
			else
			{
				Debug.LogError((object)(((object)this).GetType().Name + ": No container component found."));
			}
		}
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(3f)]
	public void RPC_Open_Dashboard(RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if (!((Object)(object)player == (Object)null) && CanBeLooted(player))
		{
			IItemContainerEntity dashboardContainer = GetDashboardContainer();
			if (!dashboardContainer.IsUnityNull())
			{
				dashboardContainer.PlayerOpenLoot(player);
			}
			else
			{
				Debug.LogError((object)(((object)this).GetType().Name + ": No container component found."));
			}
		}
	}

	public override void InitShared()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		base.InitShared();
		GameObject val = mlrsRocket.Get();
		ServerProjectile component = val.GetComponent<ServerProjectile>();
		rocketBaseGravity = (0f - Physics.gravity.y) * component.gravityModifier;
		rocketSpeed = component.speed;
		TimedExplosive component2 = val.GetComponent<TimedExplosive>();
		RocketDamageRadius = component2.explosionRadius;
	}

	public override void Load(LoadInfo info)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		base.Load(info);
		if (info.msg.mlrs != null)
		{
			SetUserTargetHitPos(info.msg.mlrs.targetPos);
			TrueHitPos = info.msg.mlrs.curHitPos;
			HitPosToRotation(TrueHitPos, out var hRot, out var vRot, out var g);
			CurGravityMultiplier = g / (0f - Physics.gravity.y);
			if (base.isServer)
			{
				HRotation = hRot;
				VRotation = vRot;
			}
			rocketStorageInstance.uid = info.msg.mlrs.rocketStorageID;
			dashboardStorageInstance.uid = info.msg.mlrs.dashboardStorageID;
			RocketAmmoCount = (int)info.msg.mlrs.ammoCount;
		}
	}

	public override bool CanBeLooted(BasePlayer player)
	{
		return !IsFiringRockets;
	}

	private void SetUserTargetHitPos(Vector3 worldPos)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		if (UserTargetHitPos == worldPos)
		{
			return;
		}
		if (base.isServer)
		{
			Vector3 position = TerrainMeta.Position;
			Vector3 val = position + TerrainMeta.Size;
			worldPos.x = Mathf.Clamp(worldPos.x, position.x, val.x);
			worldPos.z = Mathf.Clamp(worldPos.z, position.z, val.z);
			worldPos.y = GetSurfaceHeight(worldPos);
		}
		UserTargetHitPos = worldPos;
		if (!base.isServer)
		{
			return;
		}
		trueTargetHitPos = UserTargetHitPos;
		foreach (TriggerSafeZone allSafeZone in TriggerSafeZone.allSafeZones)
		{
			Bounds val2 = allSafeZone.triggerCollider.bounds;
			Vector3 center = ((Bounds)(ref val2)).center;
			center.y = 0f;
			float num = allSafeZone.triggerCollider.GetRadius(((Component)allSafeZone).transform.localScale) + targetAreaRadius;
			trueTargetHitPos.y = 0f;
			if (Vector3.Distance(center, trueTargetHitPos) < num)
			{
				Vector3 val3 = trueTargetHitPos - center;
				trueTargetHitPos = center + ((Vector3)(ref val3)).normalized * num;
				trueTargetHitPos.y = GetSurfaceHeight(trueTargetHitPos);
				break;
			}
		}
	}

	private StorageContainer GetRocketContainer()
	{
		BaseEntity baseEntity = rocketStorageInstance.Get(base.isServer);
		if ((Object)(object)baseEntity != (Object)null && baseEntity.IsValid())
		{
			return baseEntity as StorageContainer;
		}
		return null;
	}

	private StorageContainer GetDashboardContainer()
	{
		BaseEntity baseEntity = dashboardStorageInstance.Get(base.isServer);
		if ((Object)(object)baseEntity != (Object)null && baseEntity.IsValid())
		{
			return baseEntity as StorageContainer;
		}
		return null;
	}

	private void HitPosToRotation(Vector3 hitPos, out float hRot, out float vRot, out float g)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		Vector3 aimToTarget = GetAimToTarget(hitPos, out g);
		Quaternion val = Quaternion.LookRotation(aimToTarget, Vector3.up);
		Vector3 eulerAngles = ((Quaternion)(ref val)).eulerAngles;
		vRot = eulerAngles.x - 360f;
		aimToTarget.y = 0f;
		hRot = eulerAngles.y;
	}

	private Vector3 GetAimToTarget(Vector3 targetPos, out float g)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		g = rocketBaseGravity;
		float num = rocketSpeed;
		Vector3 val = targetPos - firingPoint.position;
		float num2 = Vector3Ex.Magnitude2D(val);
		float y = val.y;
		float num3 = Mathf.Sqrt(num * num * num * num - g * (g * (num2 * num2) + 2f * y * num * num));
		float num4 = Mathf.Atan((num * num + num3) / (g * num2)) * 57.29578f;
		float num5 = Mathf.Clamp(num4, 0f, 90f);
		if (float.IsNaN(num4))
		{
			num5 = 45f;
			g = ProjectileDistToGravity(num2, y, num5, num);
		}
		else if (num4 > vRotMax)
		{
			num5 = vRotMax;
			g = ProjectileDistToGravity(Mathf.Max(num2, minRange), y, num5, num);
		}
		((Vector3)(ref val)).Normalize();
		val.y = 0f;
		Vector3 val2 = Vector3.Cross(val, Vector3.up);
		val = Quaternion.AngleAxis(num5, val2) * val;
		return val;
	}

	private static float ProjectileDistToSpeed(float x, float y, float angle, float g, float fallbackV)
	{
		float num = angle * ((float)Math.PI / 180f);
		float num2 = Mathf.Sqrt(x * x * g / (x * Mathf.Sin(2f * num) - 2f * y * Mathf.Cos(num) * Mathf.Cos(num)));
		if (float.IsNaN(num2) || num2 < 1f)
		{
			num2 = fallbackV;
		}
		return num2;
	}

	private static float ProjectileDistToGravity(float x, float y, float θ, float v)
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		float num = θ * ((float)Math.PI / 180f);
		float num2 = (v * v * x * Mathf.Sin(2f * num) - 2f * v * v * y * Mathf.Cos(num) * Mathf.Cos(num)) / (x * x);
		if (float.IsNaN(num2) || num2 < 0.01f)
		{
			num2 = 0f - Physics.gravity.y;
		}
		return num2;
	}
}
