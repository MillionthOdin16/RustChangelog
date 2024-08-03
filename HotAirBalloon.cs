using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;

public class HotAirBalloon : BaseCombatEntity, SamSite.ISamSiteTarget
{
	protected const Flags Flag_HasFuel = Flags.Reserved6;

	protected const Flags Flag_HalfInflated = Flags.Reserved1;

	protected const Flags Flag_FullInflated = Flags.Reserved2;

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

	public float inflationLevel = 0f;

	[Header("Fuel")]
	public GameObjectRef fuelStoragePrefab;

	public float fuelPerSec = 0.25f;

	[Header("Storage")]
	public GameObjectRef storageUnitPrefab;

	public EntityRef<StorageContainer> storageUnitInstance;

	[Header("Damage")]
	public DamageRenderer damageRenderer = null;

	public Transform engineHeight;

	public GameObject[] killTriggers;

	private EntityFuelSystem fuelSystem;

	[ServerVar(Help = "Population active on the server", ShowInAdminUI = true)]
	public static float population = 1f;

	[ServerVar(Help = "How long before a HAB loses all its health while outside")]
	public static float outsidedecayminutes = 180f;

	public float windForce = 30000f;

	public Vector3 currentWindVec = Vector3.zero;

	public Bounds collapsedBounds;

	public Bounds raisedBounds;

	public GameObject[] balloonColliders;

	[ServerVar]
	public static float serviceCeiling = 200f;

	private float currentBuoyancy = 0f;

	private float lastBlastTime = 0f;

	private float avgTerrainHeight = 0f;

	protected bool grounded = false;

	public bool IsFullyInflated => inflationLevel >= 1f;

	public SamSite.SamTargetType SAMTargetType => SamSite.targetTypeVehicle;

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
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - EngineSwitch "));
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
						TimeWarning val4 = TimeWarning.New("Call", 0);
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
							((IDisposable)val4)?.Dispose();
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
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_OpenFuel "));
				}
				TimeWarning val5 = TimeWarning.New("RPC_OpenFuel", 0);
				try
				{
					TimeWarning val6 = TimeWarning.New("Call", 0);
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
						((IDisposable)val6)?.Dispose();
					}
				}
				catch (Exception ex2)
				{
					Debug.LogException(ex2);
					player.Kick("RPC Error in RPC_OpenFuel");
				}
				finally
				{
					((IDisposable)val5)?.Dispose();
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
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
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

	public bool WaterLogged()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return WaterLevel.Test(engineHeight.position, waves: true, volumes: true, this);
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
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (staticRespawn)
		{
			return IsFullyInflated;
		}
		return IsFullyInflated && !BaseVehicle.InSafeZone(triggers, ((Component)this).transform.position);
	}

	public override float GetNetworkTime()
	{
		return Time.fixedTime;
	}

	public override void PostServerLoad()
	{
		base.PostServerLoad();
		SetFlag(Flags.On, b: false);
	}

	[RPC_Server]
	public void RPC_OpenFuel(RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if (!((Object)(object)player == (Object)null))
		{
			fuelSystem.LootFuel(player);
		}
	}

	public override void Save(SaveInfo info)
	{
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
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
		if (base.healthFraction != 0f && !IsFullyInflated && !(Time.time < lastBlastTime + 600f))
		{
			float num = 1f / outsidedecayminutes;
			if (IsOutside())
			{
				Hurt(MaxHealth() * num, DamageType.Decay, this, useProtection: false);
			}
		}
	}

	[RPC_Server]
	[RPC_Server.IsVisible(3f)]
	public void EngineSwitch(RPCMessage msg)
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

	public void ScheduleOff()
	{
		SetFlag(Flags.On, b: false);
	}

	public void UpdateIsGrounded()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		if (!(lastBlastTime + 5f > Time.time))
		{
			List<Collider> list = Pool.GetList<Collider>();
			GamePhysics.OverlapSphere(((Component)groundSample).transform.position, 1.25f, list, 1218511105, (QueryTriggerInteraction)1);
			grounded = list.Count > 0;
			CheckGlobal(flags);
			Pool.FreeList<Collider>(ref list);
		}
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
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0359: Unknown result type (might be due to invalid IL or missing references)
		//IL_039b: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0405: Unknown result type (might be due to invalid IL or missing references)
		//IL_0410: Unknown result type (might be due to invalid IL or missing references)
		//IL_041b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0422: Unknown result type (might be due to invalid IL or missing references)
		//IL_042d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0440: Unknown result type (might be due to invalid IL or missing references)
		//IL_0445: Unknown result type (might be due to invalid IL or missing references)
		//IL_044a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0467: Unknown result type (might be due to invalid IL or missing references)
		//IL_047c: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04da: Unknown result type (might be due to invalid IL or missing references)
		//IL_04df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0540: Unknown result type (might be due to invalid IL or missing references)
		//IL_0545: Unknown result type (might be due to invalid IL or missing references)
		//IL_0547: Unknown result type (might be due to invalid IL or missing references)
		//IL_054b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0556: Unknown result type (might be due to invalid IL or missing references)
		//IL_055b: Unknown result type (might be due to invalid IL or missing references)
		//IL_055f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0564: Unknown result type (might be due to invalid IL or missing references)
		//IL_0571: Unknown result type (might be due to invalid IL or missing references)
		//IL_0576: Unknown result type (might be due to invalid IL or missing references)
		//IL_0581: Unknown result type (might be due to invalid IL or missing references)
		//IL_0588: Unknown result type (might be due to invalid IL or missing references)
		//IL_0593: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ac: Unknown result type (might be due to invalid IL or missing references)
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
			num = inflationLevel;
		}
		bool flag2 = !myRigidbody.IsSleeping() || inflationLevel > 0f;
		GameObject[] array2 = balloonColliders;
		foreach (GameObject val2 in array2)
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
			float magnitude = ((Vector3)(ref windAtPos)).magnitude;
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
			Vector3 normalized = ((Vector3)(ref windAtPos)).normalized;
			Vector3 val4 = normalized * num3 * windForce;
			currentWindVec = Vector3.Lerp(currentWindVec, val4, Time.fixedDeltaTime * 0.25f);
			myRigidbody.AddForceAtPosition(val4 * 0.1f, buoyancyPoint.position, (ForceMode)0);
			myRigidbody.AddForce(val4 * 0.9f, (ForceMode)0);
		}
	}

	public override Vector3 GetLocalVelocityServer()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)myRigidbody == (Object)null)
		{
			return Vector3.zero;
		}
		return myRigidbody.velocity;
	}

	public override Quaternion GetAngularVelocityServer()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)myRigidbody == (Object)null)
		{
			return Quaternion.identity;
		}
		return Quaternion.Euler(myRigidbody.angularVelocity * 57.29578f);
	}

	public Vector3 GetWindAtPos(Vector3 pos)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		float num = pos.y * 6f;
		Vector3 val = default(Vector3);
		((Vector3)(ref val))._002Ector(Mathf.Sin(num * ((float)Math.PI / 180f)), 0f, Mathf.Cos(num * ((float)Math.PI / 180f)));
		return ((Vector3)(ref val)).normalized * 1f;
	}
}
