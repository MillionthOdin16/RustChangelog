using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Profiling;

public class MotorRowboat : BaseBoat
{
	[Header("Audio")]
	public BlendedSoundLoops engineLoops;

	public BlendedSoundLoops waterLoops;

	public SoundDefinition engineStartSoundDef;

	public SoundDefinition engineStopSoundDef;

	public SoundDefinition movementSplashAccentSoundDef;

	public SoundDefinition engineSteerSoundDef;

	public GameObjectRef pushLandEffect;

	public GameObjectRef pushWaterEffect;

	public float waterSpeedDivisor = 10f;

	public float turnPitchModScale = -0.25f;

	public float tiltPitchModScale = 0.3f;

	public float splashAccentFrequencyMin = 1f;

	public float splashAccentFrequencyMax = 10f;

	protected const Flags Flag_EngineOn = Flags.Reserved1;

	protected const Flags Flag_ThrottleOn = Flags.Reserved2;

	protected const Flags Flag_TurnLeft = Flags.Reserved3;

	protected const Flags Flag_TurnRight = Flags.Reserved4;

	protected const Flags Flag_HasFuel = Flags.Reserved6;

	protected const Flags Flag_RecentlyPushed = Flags.Reserved8;

	protected const Flags Flag_Submerged = Flags.Reserved9;

	protected const Flags Flag_Dying = Flags.Broken;

	private const float submergeFractionMinimum = 0.85f;

	[Header("Fuel")]
	public GameObjectRef fuelStoragePrefab;

	public float fuelPerSec;

	[Header("Storage")]
	public GameObjectRef storageUnitPrefab;

	public EntityRef<StorageContainer> storageUnitInstance;

	[Header("Effects")]
	public Transform boatRear;

	public ParticleSystemContainer wakeEffect;

	public ParticleSystemContainer engineEffectIdle;

	public ParticleSystemContainer engineEffectThrottle;

	[Tooltip("If not supplied, with use engineEffectThrottle for both")]
	public ParticleSystemContainer engineEffectThrottleReverse;

	[Tooltip("Only needed if using a forwardTravelEffect")]
	public Transform boatFront;

	public ParticleSystemContainer forwardTravelEffect;

	public float forwardTravelEffectMinSpeed = 1f;

	public Projector causticsProjector;

	public Transform causticsDepthTest;

	public Transform engineLeftHandPosition;

	public Transform engineRotate;

	public float engineRotateRangeMultiplier = 1f;

	public Transform propellerRotate;

	[ServerVar(Help = "Population active on the server", ShowInAdminUI = true)]
	public static float population = 1f;

	[ServerVar(Help = "How long before a boat loses all its health while outside. If it's in deep water, deepwaterdecayminutes is used")]
	public static float outsidedecayminutes = 180f;

	[ServerVar(Help = "How long before a boat loses all its health while in deep water")]
	public static float deepwaterdecayminutes = 120f;

	[ServerVar(Help = "How long until decay begins after the boat was last used")]
	public static float decaystartdelayminutes = 45f;

	protected EntityFuelSystem fuelSystem;

	protected TimeSince timeSinceLastUsedFuel;

	public Transform[] stationaryDismounts;

	public float angularDragBase = 0.5f;

	public float engineOffAngularDragMultiplier = 1f;

	public float angularDragVelocity = 0.5f;

	public float landDrag = 0.2f;

	public float waterDrag = 0.8f;

	public float offAxisDrag = 1f;

	public float offAxisDot = 0.25f;

	protected const float DECAY_TICK_TIME = 60f;

	private TimeSince startedFlip;

	private float lastHadDriverTime = 0f;

	private const float maxVelForStationaryDismount = 4f;

	public bool IsDying => HasFlag(Flags.Broken);

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("MotorRowboat.OnRpcMessage", 0);
		try
		{
			if (rpc == 1873751172 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_EngineToggle "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_EngineToggle", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Call", 0);
					try
					{
						RPCMessage rPCMessage = default(RPCMessage);
						rPCMessage.connection = msg.connection;
						rPCMessage.player = player;
						rPCMessage.read = msg.read;
						RPCMessage msg2 = rPCMessage;
						RPC_EngineToggle(msg2);
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
				}
				catch (Exception ex)
				{
					Debug.LogException(ex);
					player.Kick("RPC Error in RPC_EngineToggle");
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
				TimeWarning val4 = TimeWarning.New("RPC_OpenFuel", 0);
				try
				{
					TimeWarning val5 = TimeWarning.New("Call", 0);
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
						((IDisposable)val5)?.Dispose();
					}
				}
				catch (Exception ex2)
				{
					Debug.LogException(ex2);
					player.Kick("RPC Error in RPC_OpenFuel");
				}
				finally
				{
					((IDisposable)val4)?.Dispose();
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
		base.InitShared();
		fuelSystem = new EntityFuelSystem(base.isServer, fuelStoragePrefab, children);
	}

	public override void ServerInit()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		base.ServerInit();
		timeSinceLastUsedFuel = TimeSince.op_Implicit(0f);
		((FacepunchBehaviour)this).InvokeRandomized((Action)BoatDecay, Random.Range(30f, 60f), 60f, 6f);
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
			if (storageUnitPrefab.isValid && child.prefabID == storageUnitPrefab.GetEntity().prefabID)
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

	public override EntityFuelSystem GetFuelSystem()
	{
		return fuelSystem;
	}

	public override int StartingFuelUnits()
	{
		return 50;
	}

	public virtual void BoatDecay()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		if (!IsDying)
		{
			BaseBoat.WaterVehicleDecay(this, 60f, TimeSince.op_Implicit(timeSinceLastUsedFuel), outsidedecayminutes, deepwaterdecayminutes, decaystartdelayminutes, preventDecayIndoors);
		}
	}

	protected override void DoPushAction(BasePlayer player)
	{
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		if (IsFlipped())
		{
			Vector3 val = ((Component)this).transform.InverseTransformPoint(((Component)player).transform.position);
			float num = 4f;
			if (val.x > 0f)
			{
				num = 0f - num;
			}
			rigidBody.AddRelativeTorque(Vector3.forward * num, (ForceMode)2);
			rigidBody.AddForce(Vector3.up * 4f, (ForceMode)2);
			startedFlip = TimeSince.op_Implicit(0f);
			((FacepunchBehaviour)this).InvokeRepeatingFixedTime((Action)FlipMonitor);
		}
		else
		{
			Vector3 val2 = Vector3Ex.Direction2D(((Component)player).transform.position, ((Component)this).transform.position);
			Vector3 val3 = Vector3Ex.Direction2D(((Component)player).transform.position + player.eyes.BodyForward() * 3f, ((Component)player).transform.position);
			Vector3 val4 = Vector3.up * 0.1f + val3;
			val3 = ((Vector3)(ref val4)).normalized;
			Vector3 val5 = ((Component)this).transform.position + val2 * 2f;
			float num2 = 3f;
			float num3 = Vector3.Dot(((Component)this).transform.forward, val3);
			num2 += Mathf.InverseLerp(0.8f, 1f, num3) * 3f;
			rigidBody.AddForceAtPosition(val3 * num2, val5, (ForceMode)2);
		}
		if (HasFlag(Flags.Reserved9))
		{
			if (pushWaterEffect.isValid)
			{
				Effect.server.Run(pushWaterEffect.resourcePath, this, 0u, Vector3.zero, Vector3.zero);
			}
		}
		else if (pushLandEffect.isValid)
		{
			Effect.server.Run(pushLandEffect.resourcePath, this, 0u, Vector3.zero, Vector3.zero);
		}
		WakeUp();
	}

	private void FlipMonitor()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		float num = Vector3.Dot(Vector3.up, ((Component)this).transform.up);
		rigidBody.angularVelocity = Vector3.Lerp(rigidBody.angularVelocity, Vector3.zero, Time.fixedDeltaTime * 8f * num);
		if (TimeSince.op_Implicit(startedFlip) > 3f)
		{
			((FacepunchBehaviour)this).CancelInvokeFixedTime((Action)FlipMonitor);
		}
	}

	[RPC_Server]
	public void RPC_OpenFuel(RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if (!((Object)(object)player == (Object)null) && IsDriver(player))
		{
			fuelSystem.LootFuel(player);
		}
	}

	[RPC_Server]
	public void RPC_EngineToggle(RPCMessage msg)
	{
		BasePlayer player = msg.player;
		if (!((Object)(object)player == (Object)null))
		{
			bool flag = msg.read.Bit();
			if (InDryDock())
			{
				flag = false;
			}
			if (IsDriver(player) && flag != EngineOn())
			{
				EngineToggle(flag);
			}
		}
	}

	public void EngineToggle(bool wantsOn)
	{
		if (fuelSystem.HasFuel(forceCheck: true))
		{
			SetFlag(Flags.Reserved1, wantsOn);
		}
	}

	public override void PostServerLoad()
	{
		base.PostServerLoad();
		((FacepunchBehaviour)this).Invoke((Action)CheckInvalidBoat, 1f);
		if (base.health <= 0f)
		{
			EnterCorpseState();
			buoyancy.buoyancyScale = 0f;
			SetFlag(Flags.Broken, b: true);
		}
	}

	public void CheckInvalidBoat()
	{
		bool flag = fuelStoragePrefab.isValid && !fuelSystem.fuelStorageInstance.IsValid(base.isServer);
		bool flag2 = storageUnitPrefab.isValid && !storageUnitInstance.IsValid(base.isServer);
		if (flag || flag2)
		{
			Debug.Log((object)"Destroying invalid boat ");
			((FacepunchBehaviour)this).Invoke((Action)ActualDeath, 1f);
		}
	}

	public override void PlayerServerInput(InputState inputState, BasePlayer player)
	{
		base.PlayerServerInput(inputState, player);
	}

	public override bool EngineOn()
	{
		return HasFlag(Flags.Reserved1);
	}

	public float TimeSinceDriver()
	{
		return Time.time - lastHadDriverTime;
	}

	public override void DriverInput(InputState inputState, BasePlayer player)
	{
		base.DriverInput(inputState, player);
		lastHadDriverTime = Time.time;
	}

	public override void VehicleFixedUpdate()
	{
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		base.VehicleFixedUpdate();
		Profiler.BeginSample("MotorRowboat.VehicleFixedUpdate");
		Profiler.BeginSample("Input");
		float num = TimeSinceDriver();
		if (num > 15f)
		{
			steering += Mathf.InverseLerp(15f, 30f, num);
			steering = Mathf.Clamp(-1f, 1f, steering);
			if (num > 75f)
			{
				gasPedal = 0f;
			}
		}
		Profiler.EndSample();
		SetFlags();
		Profiler.BeginSample("UpdateDrag");
		UpdateDrag();
		Profiler.EndSample();
		Profiler.BeginSample("Buoyancy.Scale");
		if (IsDying)
		{
			buoyancy.buoyancyScale = Mathf.Lerp(buoyancy.buoyancyScale, 0f, Time.fixedDeltaTime * 0.1f);
		}
		else
		{
			float num2 = 1f;
			float num3 = Vector3Ex.Magnitude2D(rigidBody.velocity);
			float num4 = Mathf.InverseLerp(1f, 10f, num3) * 0.5f * base.healthFraction;
			if (!EngineOn())
			{
				num4 = 0f;
			}
			float num5 = 1f - 0.3f * (1f - base.healthFraction);
			buoyancy.buoyancyScale = (num2 + num4) * num5;
		}
		Profiler.EndSample();
		Profiler.BeginSample("UseFuel");
		if (EngineOn())
		{
			float num6 = (HasFlag(Flags.Reserved2) ? 1f : 0.0333f);
			fuelSystem.TryUseFuel(Time.fixedDeltaTime * num6, fuelPerSec);
			timeSinceLastUsedFuel = TimeSince.op_Implicit(0f);
		}
		Profiler.EndSample();
		Profiler.EndSample();
	}

	private void SetFlags()
	{
		TimeWarning val = TimeWarning.New("SetFlag", 0);
		try
		{
			bool b = EngineOn() && !IsFlipped() && base.healthFraction > 0f && fuelSystem.HasFuel() && TimeSinceDriver() < 75f;
			Flags flags = base.flags;
			SetFlag(Flags.Reserved3, steering > 0f, recursive: false, networkupdate: false);
			SetFlag(Flags.Reserved4, steering < 0f, recursive: false, networkupdate: false);
			SetFlag(Flags.Reserved1, b, recursive: false, networkupdate: false);
			SetFlag(Flags.Reserved2, EngineOn() && gasPedal != 0f, recursive: false, networkupdate: false);
			SetFlag(Flags.Reserved9, buoyancy.submergedFraction > 0.85f, recursive: false, networkupdate: false);
			SetFlag(Flags.Reserved6, fuelSystem.HasFuel(), recursive: false, networkupdate: false);
			SetFlag(Flags.Reserved8, base.RecentlyPushed, recursive: false, networkupdate: false);
			if (flags != base.flags)
			{
				((FacepunchBehaviour)this).Invoke((Action)base.SendNetworkUpdate_Flags, 0f);
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	protected override bool DetermineIfStationary()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		Vector3 localVelocity = GetLocalVelocity();
		return ((Vector3)(ref localVelocity)).sqrMagnitude < 0.5f && !AnyMounted();
	}

	public override void SeatClippedWorld(BaseMountable mountable)
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		BasePlayer mounted = mountable.GetMounted();
		if (!((Object)(object)mounted == (Object)null))
		{
			if (IsDriver(mounted))
			{
				steering = 0f;
				gasPedal = 0f;
			}
			Vector3 velocity = rigidBody.velocity;
			float num = Mathf.InverseLerp(4f, 20f, ((Vector3)(ref velocity)).magnitude);
			if (num > 0f)
			{
				mounted.Hurt(num * 100f, DamageType.Blunt, this, useProtection: false);
			}
			if ((Object)(object)mounted != (Object)null && mounted.isMounted)
			{
				base.SeatClippedWorld(mountable);
			}
		}
	}

	public void UpdateDrag()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		float num = Vector3Ex.SqrMagnitude2D(rigidBody.velocity);
		float num2 = Mathf.InverseLerp(0f, 2f, num);
		bool flag = HasFlag(Flags.Reserved1);
		float num3 = angularDragBase * (flag ? 1f : engineOffAngularDragMultiplier);
		rigidBody.angularDrag = num3 + angularDragVelocity * num2;
		rigidBody.drag = landDrag + waterDrag * Mathf.InverseLerp(0f, 1f, buoyancy.submergedFraction);
		if (offAxisDrag > 0f)
		{
			Vector3 forward = ((Component)this).transform.forward;
			Vector3 velocity = rigidBody.velocity;
			float num4 = Vector3.Dot(forward, ((Vector3)(ref velocity)).normalized);
			float num5 = Mathf.InverseLerp(0.98f, 0.92f, num4);
			Rigidbody obj = rigidBody;
			obj.drag += num5 * offAxisDrag * buoyancy.submergedFraction;
		}
	}

	public override void OnKilled(HitInfo info)
	{
		if (!IsDying)
		{
			SetFlag(Flags.Broken, b: true);
			repair.enabled = false;
			((FacepunchBehaviour)this).Invoke((Action)DismountAllPlayers, 10f);
			EnterCorpseState();
		}
	}

	protected virtual void EnterCorpseState()
	{
		((FacepunchBehaviour)this).Invoke((Action)ActualDeath, vehicle.boat_corpse_seconds);
	}

	public void ActualDeath()
	{
		Kill(DestroyMode.Gib);
	}

	public override bool MountEligable(BasePlayer player)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		if (IsDying)
		{
			return false;
		}
		Vector3 velocity = rigidBody.velocity;
		if (((Vector3)(ref velocity)).magnitude >= 5f && HasDriver())
		{
			return false;
		}
		return base.MountEligable(player);
	}

	public override bool HasValidDismountPosition(BasePlayer player)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		Vector3 worldVelocity = GetWorldVelocity();
		if (((Vector3)(ref worldVelocity)).magnitude <= 4f)
		{
			Transform[] array = stationaryDismounts;
			foreach (Transform val in array)
			{
				if (ValidDismountPosition(player, ((Component)val).transform.position))
				{
					return true;
				}
			}
		}
		return base.HasValidDismountPosition(player);
	}

	public override bool GetDismountPosition(BasePlayer player, out Vector3 res)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		Vector3 velocity = rigidBody.velocity;
		if (((Vector3)(ref velocity)).magnitude <= 4f)
		{
			List<Vector3> list = Pool.GetList<Vector3>();
			Transform[] array = stationaryDismounts;
			foreach (Transform val in array)
			{
				if (ValidDismountPosition(player, ((Component)val).transform.position))
				{
					list.Add(((Component)val).transform.position);
				}
			}
			if (list.Count > 0)
			{
				Vector3 pos = ((Component)player).transform.position;
				list.Sort((Vector3 a, Vector3 b) => Vector3.Distance(a, pos).CompareTo(Vector3.Distance(b, pos)));
				res = list[0];
				Pool.FreeList<Vector3>(ref list);
				return true;
			}
			Pool.FreeList<Vector3>(ref list);
		}
		return base.GetDismountPosition(player, out res);
	}

	public override void Save(SaveInfo info)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		base.Save(info);
		info.msg.motorBoat = Pool.Get<Motorboat>();
		info.msg.motorBoat.storageid = storageUnitInstance.uid;
		info.msg.motorBoat.fuelStorageID = fuelSystem.fuelStorageInstance.uid;
	}

	protected override bool CanPushNow(BasePlayer pusher)
	{
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		if (!base.CanPushNow(pusher))
		{
			return false;
		}
		if (!IsStationary() || (!(pusher.WaterFactor() <= 0.6f) && !IsFlipped()))
		{
			return false;
		}
		if (!IsFlipped() && pusher.IsStandingOnEntity(this, 8192))
		{
			return false;
		}
		if (Vector3.Distance(((Component)pusher).transform.position, ((Component)this).transform.position) > 5f)
		{
			return false;
		}
		if (IsDying)
		{
			return false;
		}
		return !pusher.isMounted && pusher.IsOnGround() && base.healthFraction > 0f && ShowPushMenu(pusher);
	}

	private bool ShowPushMenu(BasePlayer player)
	{
		if (!IsFlipped() && player.IsStandingOnEntity(this, 8192))
		{
			return false;
		}
		return IsStationary() && (player.WaterFactor() <= 0.6f || IsFlipped());
	}

	public override void Load(LoadInfo info)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		base.Load(info);
		if (info.msg.motorBoat != null)
		{
			fuelSystem.fuelStorageInstance.uid = info.msg.motorBoat.fuelStorageID;
			storageUnitInstance.uid = info.msg.motorBoat.storageid;
		}
	}
}
