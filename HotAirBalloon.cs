using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

public class HotAirBalloon : BaseCombatEntity, VehicleSpawner.IVehicleSpawnUser, SamSite.ISamSiteTarget
{
	[Serializable]
	public struct UpgradeOption
	{
		public ItemDefinition TokenItem;

		public Phrase Title;

		public Phrase Description;

		public Sprite Icon;

		public int order;
	}

	protected const Flags Flag_HasFuel = Flags.Reserved6;

	protected const Flags Flag_Grounded = Flags.Reserved7;

	protected const Flags Flag_CanModifyEquipment = Flags.Reserved8;

	protected const Flags Flag_HalfInflated = Flags.Reserved1;

	protected const Flags Flag_FullInflated = Flags.Reserved2;

	public const Flags Flag_OnlyOwnerEntry = Flags.Locked;

	public Transform centerOfMass;

	public Rigidbody myRigidbody;

	public Transform buoyancyPoint;

	public float liftAmount = 10f;

	public Transform windSock;

	public Transform[] windFlags;

	public GameObject staticBalloonDeflated;

	public GameObject staticBalloon;

	public GameObject animatedBalloon;

	public Animator balloonAnimator;

	public Transform groundSample;

	public float inflationLevel;

	[Header("Fuel")]
	public GameObjectRef fuelStoragePrefab;

	public float fuelPerSec = 0.25f;

	[Header("Storage")]
	public GameObjectRef storageUnitPrefab;

	public EntityRef<StorageContainer> storageUnitInstance;

	[Header("Damage")]
	public DamageRenderer damageRenderer;

	public Transform engineHeight;

	public GameObject[] killTriggers;

	[Header("Upgrades")]
	public List<UpgradeOption> UpgradeOptions;

	private EntityFuelSystem fuelSystem;

	[ServerVar(Help = "Population active on the server", ShowInAdminUI = true)]
	public static float population = 1f;

	[ServerVar(Help = "How long before a HAB loses all its health while outside")]
	public static float outsidedecayminutes = 180f;

	public float NextUpgradeTime;

	public float windForce = 30000f;

	public Vector3 currentWindVec = Vector3.zero;

	public Bounds collapsedBounds;

	public Bounds raisedBounds;

	public GameObject[] balloonColliders;

	[ServerVar]
	public static float serviceCeiling = 200f;

	private Vector3 lastFailedDecayPosition = Vector3.zero;

	private float currentBuoyancy;

	private float lastBlastTime;

	private float avgTerrainHeight;

	protected bool grounded;

	private float spawnTime = -1f;

	private float safeAreaRadius;

	private Vector3 safeAreaOrigin;

	public bool IsFullyInflated => inflationLevel >= 1f;

	public bool Grounded => HasFlag(Flags.Reserved7);

	public SamSite.SamTargetType SAMTargetType => SamSite.targetTypeVehicle;

	public bool IsClient => base.isClient;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("HotAirBalloon.OnRpcMessage", 0);
		try
		{
			if (rpc == 578721460 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - EngineSwitch "));
				}
				TimeWarning val2 = TimeWarning.New("EngineSwitch", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(578721460u, "EngineSwitch", this, player, 3f))
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
							EngineSwitch(msg2);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in EngineSwitch");
					}
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 1851540757 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - RPC_OpenFuel "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_OpenFuel", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Call", 0);
					try
					{
						RPCMessage rPCMessage = default(RPCMessage);
						rPCMessage.connection = msg.connection;
						rPCMessage.player = player;
						rPCMessage.read = msg.read;
						RPCMessage msg3 = rPCMessage;
						RPC_OpenFuel(msg3);
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
				}
				catch (Exception ex2)
				{
					Debug.LogException(ex2);
					player.Kick("RPC Error in RPC_OpenFuel");
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
				return true;
			}
			if (rpc == 2441951484u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - RPC_ReqEquipItem "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_ReqEquipItem", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.IsVisible.Test(2441951484u, "RPC_ReqEquipItem", this, player, 3f))
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
							RPC_ReqEquipItem(msg4);
						}
						finally
						{
							((IDisposable)val3)?.Dispose();
						}
					}
					catch (Exception ex3)
					{
						Debug.LogException(ex3);
						player.Kick("RPC Error in RPC_ReqEquipItem");
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

	public override void InitShared()
	{
		fuelSystem = new EntityFuelSystem(base.isServer, fuelStoragePrefab, children);
	}

	public override void Load(LoadInfo info)
	{
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		base.Load(info);
		if (info.msg.hotAirBalloon != null)
		{
			inflationLevel = info.msg.hotAirBalloon.inflationAmount;
			if (info.fromDisk && Object.op_Implicit((Object)(object)myRigidbody))
			{
				myRigidbody.velocity = info.msg.hotAirBalloon.velocity;
			}
		}
		if (info.msg.motorBoat != null)
		{
			fuelSystem.fuelStorageInstance.uid = info.msg.motorBoat.fuelStorageID;
			storageUnitInstance.uid = info.msg.motorBoat.storageid;
		}
	}

	public bool CanModifyEquipment()
	{
		if (base.isServer && Time.time < NextUpgradeTime)
		{
			return false;
		}
		return true;
	}

	public void DelayNextUpgrade(float delay)
	{
		if (Time.time + delay > NextUpgradeTime)
		{
			NextUpgradeTime = Time.time + delay;
		}
	}

	public int GetEquipmentCount(ItemModHABEquipment item)
	{
		int num = 0;
		for (int num2 = children.Count - 1; num2 >= 0; num2--)
		{
			BaseEntity baseEntity = children[num2];
			if (!((Object)(object)baseEntity == (Object)null) && baseEntity.prefabID == item.Prefab.resourceID)
			{
				num++;
			}
		}
		return num;
	}

	public void RemoveItemsOfType(ItemModHABEquipment item)
	{
		for (int num = children.Count - 1; num >= 0; num--)
		{
			BaseEntity baseEntity = children[num];
			if (!((Object)(object)baseEntity == (Object)null) && baseEntity.prefabID == item.Prefab.resourceID)
			{
				baseEntity.Kill();
			}
		}
	}

	public bool WaterLogged()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return WaterLevel.Test(engineHeight.position, waves: true, volumes: true, this);
	}

	public bool OnlyOwnerAccessible()
	{
		return HasFlag(Flags.Locked);
	}

	public override void OnAttacked(HitInfo info)
	{
		if (IsSafe() && !info.damageTypes.Has(DamageType.Decay))
		{
			info.damageTypes.ScaleAll(0f);
		}
		base.OnAttacked(info);
	}

	protected override void OnChildAdded(BaseEntity child)
	{
		base.OnChildAdded(child);
		if (base.isServer)
		{
			if (isSpawned)
			{
				fuelSystem.CheckNewChild(child);
			}
			if (child.prefabID == storageUnitPrefab.GetEntity().prefabID)
			{
				storageUnitInstance.Set((StorageContainer)child);
				_ = storageUnitInstance.Get(serverside: true).inventory;
			}
			bool isLoadingSave = Application.isLoadingSave;
			HotAirBalloonEquipment hotAirBalloonEquipment = child as HotAirBalloonEquipment;
			if ((Object)(object)hotAirBalloonEquipment != (Object)null)
			{
				hotAirBalloonEquipment.Added(this, isLoadingSave);
			}
		}
	}

	protected override void OnChildRemoved(BaseEntity child)
	{
		base.OnChildRemoved(child);
		if (base.isServer)
		{
			HotAirBalloonEquipment hotAirBalloonEquipment = child as HotAirBalloonEquipment;
			if ((Object)(object)hotAirBalloonEquipment != (Object)null)
			{
				hotAirBalloonEquipment.Removed(this);
			}
		}
	}

	internal override void DoServerDestroy()
	{
		if (vehicle.vehiclesdroploot && storageUnitInstance.IsValid(base.isServer))
		{
			storageUnitInstance.Get(base.isServer).DropItems();
		}
		base.DoServerDestroy();
	}

	public bool IsValidSAMTarget(bool staticRespawn)
	{
		if (staticRespawn)
		{
			return IsFullyInflated;
		}
		if (IsFullyInflated)
		{
			return !InSafeZone();
		}
		return false;
	}

	public override float GetNetworkTime()
	{
		return Time.fixedTime;
	}

	public override void PostServerLoad()
	{
		base.PostServerLoad();
		ClearOwnerEntry();
		SetFlag(Flags.On, b: false);
	}

	[RPC_Server]
	public void RPC_OpenFuel(RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if (!((Object)(object)player == (Object)null) && (!OnlyOwnerAccessible() || !((Object)(object)msg.player != (Object)(object)creatorEntity)))
		{
			fuelSystem.LootFuel(player);
		}
	}

	public override void Save(SaveInfo info)
	{
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		base.Save(info);
		info.msg.hotAirBalloon = Pool.Get<HotAirBalloon>();
		info.msg.hotAirBalloon.inflationAmount = inflationLevel;
		if (info.forDisk && Object.op_Implicit((Object)(object)myRigidbody))
		{
			info.msg.hotAirBalloon.velocity = myRigidbody.velocity;
		}
		info.msg.motorBoat = Pool.Get<Motorboat>();
		info.msg.motorBoat.storageid = storageUnitInstance.uid;
		info.msg.motorBoat.fuelStorageID = fuelSystem.fuelStorageInstance.uid;
	}

	public override void ServerInit()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		myRigidbody.centerOfMass = centerOfMass.localPosition;
		myRigidbody.isKinematic = false;
		avgTerrainHeight = TerrainMeta.HeightMap.GetHeight(((Component)this).transform.position);
		base.ServerInit();
		bounds = collapsedBounds;
		((FacepunchBehaviour)this).InvokeRandomized((Action)DecayTick, Random.Range(30f, 60f), 60f, 6f);
		((FacepunchBehaviour)this).InvokeRandomized((Action)UpdateIsGrounded, 0f, 3f, 0.2f);
	}

	public void DecayTick()
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		if (base.healthFraction == 0f)
		{
			return;
		}
		if (IsFullyInflated)
		{
			bool flag = true;
			if (lastFailedDecayPosition != Vector3.zero && Distance(lastFailedDecayPosition) < 2f)
			{
				flag = false;
			}
			lastFailedDecayPosition = ((Component)this).transform.position;
			if (flag)
			{
				return;
			}
			myRigidbody.AddForceAtPosition(Vector3.up * (0f - Physics.gravity.y) * myRigidbody.mass * 20f, buoyancyPoint.position, (ForceMode)0);
			myRigidbody.AddForceAtPosition(Vector3Ex.WithY(Random.onUnitSphere, 0f) * 20f, buoyancyPoint.position, (ForceMode)0);
			Debug.Log((object)"Bump");
		}
		if (!(Time.time < lastBlastTime + 600f))
		{
			float num = 1f / outsidedecayminutes;
			if (IsOutside() || IsFullyInflated)
			{
				Hurt(MaxHealth() * num, DamageType.Decay, this, useProtection: false);
			}
		}
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	public void EngineSwitch(RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if (!((Object)(object)player == (Object)null) && (!OnlyOwnerAccessible() || !((Object)(object)player != (Object)(object)creatorEntity)))
		{
			bool b = msg.read.Bit();
			SetFlag(Flags.On, b);
			if (IsOn())
			{
				((FacepunchBehaviour)this).Invoke((Action)ScheduleOff, 60f);
			}
			else
			{
				((FacepunchBehaviour)this).CancelInvoke((Action)ScheduleOff);
			}
		}
	}

	public void ScheduleOff()
	{
		SetFlag(Flags.On, b: false);
	}

	public void UpdateIsGrounded()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		List<Collider> list = Pool.GetList<Collider>();
		GamePhysics.OverlapSphere(((Component)groundSample).transform.position, 1.25f, list, 1218511105, (QueryTriggerInteraction)1);
		grounded = list.Count > 0;
		CheckGlobal(flags);
		Pool.FreeList<Collider>(ref list);
	}

	public override void OnFlagsChanged(Flags old, Flags next)
	{
		base.OnFlagsChanged(old, next);
		if (base.isServer)
		{
			CheckGlobal(next);
		}
	}

	private void CheckGlobal(Flags flags)
	{
		bool wants = flags.HasFlag(Flags.On) || flags.HasFlag(Flags.Reserved2) || flags.HasFlag(Flags.Reserved1) || !grounded;
		EnableGlobalBroadcast(wants);
	}

	protected void FixedUpdate()
	{
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_031f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0337: Unknown result type (might be due to invalid IL or missing references)
		//IL_033c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0347: Unknown result type (might be due to invalid IL or missing references)
		//IL_0357: Unknown result type (might be due to invalid IL or missing references)
		//IL_0361: Unknown result type (might be due to invalid IL or missing references)
		//IL_036c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0377: Unknown result type (might be due to invalid IL or missing references)
		//IL_0388: Unknown result type (might be due to invalid IL or missing references)
		//IL_0393: Unknown result type (might be due to invalid IL or missing references)
		//IL_039e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0424: Unknown result type (might be due to invalid IL or missing references)
		//IL_0442: Unknown result type (might be due to invalid IL or missing references)
		//IL_0447: Unknown result type (might be due to invalid IL or missing references)
		//IL_0451: Unknown result type (might be due to invalid IL or missing references)
		//IL_0456: Unknown result type (might be due to invalid IL or missing references)
		//IL_045b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0460: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04be: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0506: Unknown result type (might be due to invalid IL or missing references)
		//IL_0517: Unknown result type (might be due to invalid IL or missing references)
		//IL_051e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0544: Unknown result type (might be due to invalid IL or missing references)
		//IL_054a: Unknown result type (might be due to invalid IL or missing references)
		if (!isSpawned || base.isClient)
		{
			return;
		}
		if (!fuelSystem.HasFuel() || WaterLogged())
		{
			SetFlag(Flags.On, b: false);
		}
		if (IsOn())
		{
			fuelSystem.TryUseFuel(Time.fixedDeltaTime, fuelPerSec);
		}
		SetFlag(Flags.Reserved6, fuelSystem.HasFuel());
		SetFlag(Flags.Reserved7, grounded);
		SetFlag(Flags.Reserved8, CanModifyEquipment());
		bool flag = (IsFullyInflated && myRigidbody.velocity.y < 0f) || myRigidbody.velocity.y < 0.75f;
		GameObject[] array = killTriggers;
		foreach (GameObject val in array)
		{
			if (val.activeSelf != flag)
			{
				val.SetActive(flag);
			}
		}
		float num = inflationLevel;
		if (IsOn() && !IsFullyInflated)
		{
			inflationLevel = Mathf.Clamp01(inflationLevel + Time.fixedDeltaTime / 10f);
		}
		else if (grounded && inflationLevel > 0f && !IsOn() && (Time.time > lastBlastTime + 30f || WaterLogged()))
		{
			inflationLevel = Mathf.Clamp01(inflationLevel - Time.fixedDeltaTime / 10f);
		}
		if (num != inflationLevel)
		{
			if (IsFullyInflated)
			{
				bounds = raisedBounds;
			}
			else if (inflationLevel == 0f)
			{
				bounds = collapsedBounds;
			}
			SetFlag(Flags.Reserved1, inflationLevel > 0.3f);
			SetFlag(Flags.Reserved2, inflationLevel >= 1f);
			SendNetworkUpdate();
			_ = inflationLevel;
		}
		bool flag2 = !myRigidbody.IsSleeping() || inflationLevel > 0f;
		array = balloonColliders;
		foreach (GameObject val2 in array)
		{
			if (val2.activeSelf != flag2)
			{
				val2.SetActive(flag2);
			}
		}
		if (IsOn())
		{
			if (IsFullyInflated)
			{
				currentBuoyancy += Time.fixedDeltaTime * 0.2f;
				lastBlastTime = Time.time;
			}
		}
		else
		{
			currentBuoyancy -= Time.fixedDeltaTime * 0.1f;
		}
		currentBuoyancy = Mathf.Clamp(currentBuoyancy, 0f, 0.8f + 0.2f * base.healthFraction);
		if (inflationLevel > 0f)
		{
			avgTerrainHeight = Mathf.Lerp(avgTerrainHeight, TerrainMeta.HeightMap.GetHeight(((Component)this).transform.position), Time.deltaTime);
			float num2 = 1f - Mathf.InverseLerp(avgTerrainHeight + serviceCeiling - 20f, avgTerrainHeight + serviceCeiling, buoyancyPoint.position.y);
			myRigidbody.AddForceAtPosition(Vector3.up * (0f - Physics.gravity.y) * myRigidbody.mass * 0.5f * inflationLevel, buoyancyPoint.position, (ForceMode)0);
			myRigidbody.AddForceAtPosition(Vector3.up * liftAmount * currentBuoyancy * num2, buoyancyPoint.position, (ForceMode)0);
			Vector3 windAtPos = GetWindAtPos(buoyancyPoint.position);
			_ = ((Vector3)(ref windAtPos)).magnitude;
			float num3 = 1f;
			float num4 = Mathf.Max(TerrainMeta.HeightMap.GetHeight(buoyancyPoint.position), TerrainMeta.WaterMap.GetHeight(buoyancyPoint.position));
			float num5 = Mathf.InverseLerp(num4 + 20f, num4 + 60f, buoyancyPoint.position.y);
			float num6 = 1f;
			RaycastHit val3 = default(RaycastHit);
			if (Physics.SphereCast(new Ray(((Component)this).transform.position + Vector3.up * 2f, Vector3.down), 1.5f, ref val3, 5f, 1218511105))
			{
				num6 = Mathf.Clamp01(((RaycastHit)(ref val3)).distance / 5f);
			}
			num3 *= num5 * num2 * num6;
			num3 *= 0.2f + 0.8f * base.healthFraction;
			Vector3 val4 = ((Vector3)(ref windAtPos)).normalized * num3 * windForce;
			currentWindVec = Vector3.Lerp(currentWindVec, val4, Time.fixedDeltaTime * 0.25f);
			myRigidbody.AddForceAtPosition(val4 * 0.1f, buoyancyPoint.position, (ForceMode)0);
			myRigidbody.AddForce(val4 * 0.9f, (ForceMode)0);
		}
		if (OnlyOwnerAccessible() && safeAreaRadius != -1f && Vector3.Distance(((Component)this).transform.position, safeAreaOrigin) > safeAreaRadius)
		{
			ClearOwnerEntry();
		}
	}

	public override Vector3 GetLocalVelocityServer()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)myRigidbody == (Object)null)
		{
			return Vector3.zero;
		}
		return myRigidbody.velocity;
	}

	public override Quaternion GetAngularVelocityServer()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)myRigidbody == (Object)null)
		{
			return Quaternion.identity;
		}
		return Quaternion.Euler(myRigidbody.angularVelocity * 57.29578f);
	}

	public void ClearOwnerEntry()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		creatorEntity = null;
		SetFlag(Flags.Locked, b: false);
		safeAreaRadius = -1f;
		safeAreaOrigin = Vector3.zero;
	}

	public bool IsSafe()
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		if (OnlyOwnerAccessible())
		{
			return Vector3.Distance(safeAreaOrigin, ((Component)this).transform.position) <= safeAreaRadius;
		}
		return false;
	}

	public void SetupOwner(BasePlayer owner, Vector3 newSafeAreaOrigin, float newSafeAreaRadius)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)owner != (Object)null)
		{
			creatorEntity = owner;
			SetFlag(Flags.Locked, b: true);
			safeAreaRadius = newSafeAreaRadius;
			safeAreaOrigin = newSafeAreaOrigin;
			spawnTime = Time.realtimeSinceStartup;
		}
	}

	public bool IsDespawnEligable()
	{
		if (spawnTime != -1f)
		{
			return spawnTime + 300f < Time.realtimeSinceStartup;
		}
		return true;
	}

	public EntityFuelSystem GetFuelSystem()
	{
		return fuelSystem;
	}

	public int StartingFuelUnits()
	{
		return 75;
	}

	public Vector3 GetWindAtPos(Vector3 pos)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		float num = pos.y * 6f;
		Vector3 val = default(Vector3);
		((Vector3)(ref val))._002Ector(Mathf.Sin(num * ((float)Math.PI / 180f)), 0f, Mathf.Cos(num * ((float)Math.PI / 180f)));
		return ((Vector3)(ref val)).normalized * 1f;
	}

	public bool PlayerHasEquipmentItem(BasePlayer player, int tokenItemID)
	{
		return GetEquipmentItem(player, tokenItemID) != null;
	}

	public Item GetEquipmentItem(BasePlayer player, int tokenItemID)
	{
		return player.inventory.FindItemByItemID(tokenItemID);
	}

	public override float MaxHealth()
	{
		if (base.isServer)
		{
			return base.MaxHealth();
		}
		float num = base.MaxHealth();
		float num2 = 0f;
		foreach (BaseEntity child in children)
		{
			if (child is HotAirBalloonArmor hotAirBalloonArmor)
			{
				num2 += hotAirBalloonArmor.AdditionalHealth;
			}
		}
		return num + num2;
	}

	public override List<ItemAmount> BuildCost()
	{
		List<ItemAmount> list = new List<ItemAmount>(base.BuildCost());
		foreach (BaseEntity child in children)
		{
			if (child is HotAirBalloonEquipment hotAirBalloonEquipment)
			{
				list.AddRange(hotAirBalloonEquipment.BuildCost());
			}
		}
		return list;
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	public void RPC_ReqEquipItem(RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if ((Object)(object)player == (Object)null)
		{
			return;
		}
		int tokenItemID = msg.read.Int32();
		Item equipmentItem = GetEquipmentItem(player, tokenItemID);
		if (equipmentItem != null)
		{
			ItemModHABEquipment component = ((Component)equipmentItem.info).GetComponent<ItemModHABEquipment>();
			if (!((Object)(object)component == (Object)null) && component.CanEquipToHAB(this))
			{
				component.ApplyToHAB(this);
				equipmentItem.UseItem();
				SendNetworkUpdateImmediate();
			}
		}
	}
}
