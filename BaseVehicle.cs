using System;
using System.Collections;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

public class BaseVehicle : BaseMountable
{
	public enum ClippingCheckMode
	{
		OnMountOnly,
		Always,
		AlwaysHeadOnly
	}

	public enum DismountStyle
	{
		Closest,
		Ordered
	}

	[Serializable]
	public class MountPointInfo
	{
		public bool isDriver;

		public Vector3 pos;

		public Vector3 rot;

		public string bone = "";

		public GameObjectRef prefab;

		[HideInInspector]
		public BaseMountable mountable;
	}

	public readonly struct Enumerable : IEnumerable<MountPointInfo>, IEnumerable
	{
		private readonly BaseVehicle _vehicle;

		public Enumerable(BaseVehicle vehicle)
		{
			if ((Object)(object)vehicle == (Object)null)
			{
				throw new ArgumentNullException("vehicle");
			}
			_vehicle = vehicle;
		}

		public Enumerator GetEnumerator()
		{
			return new Enumerator(_vehicle);
		}

		IEnumerator<MountPointInfo> IEnumerable<MountPointInfo>.GetEnumerator()
		{
			return GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}

	public struct Enumerator : IEnumerator<MountPointInfo>, IEnumerator, IDisposable
	{
		private enum State
		{
			Direct,
			EnterChild,
			EnumerateChild,
			Finished
		}

		private class Box : IPooled
		{
			public Enumerator Value;

			public void EnterPool()
			{
				Value = default(Enumerator);
			}

			public void LeavePool()
			{
				Value = default(Enumerator);
			}
		}

		private readonly BaseVehicle _vehicle;

		private State _state;

		private int _index;

		private int _childIndex;

		private Box _enumerator;

		public MountPointInfo Current { get; private set; }

		object IEnumerator.Current => Current;

		public Enumerator(BaseVehicle vehicle)
		{
			if ((Object)(object)vehicle == (Object)null)
			{
				throw new ArgumentNullException("vehicle");
			}
			_vehicle = vehicle;
			_state = State.Direct;
			_index = -1;
			_childIndex = -1;
			_enumerator = null;
			Current = null;
		}

		public bool MoveNext()
		{
			Current = null;
			switch (_state)
			{
			case State.Direct:
				_index++;
				if (_index >= _vehicle.mountPoints.Count)
				{
					_state = State.EnterChild;
					goto case State.EnterChild;
				}
				Current = _vehicle.mountPoints[_index];
				return true;
			case State.EnterChild:
				do
				{
					_childIndex++;
				}
				while (_childIndex < _vehicle.childVehicles.Count && (Object)(object)_vehicle.childVehicles[_childIndex] == (Object)null);
				if (_childIndex >= _vehicle.childVehicles.Count)
				{
					_state = State.Finished;
					return false;
				}
				_enumerator = Pool.Get<Box>();
				_enumerator.Value = _vehicle.childVehicles[_childIndex].allMountPoints.GetEnumerator();
				_state = State.EnumerateChild;
				goto case State.EnumerateChild;
			case State.EnumerateChild:
				if (_enumerator.Value.MoveNext())
				{
					Current = _enumerator.Value.Current;
					return true;
				}
				_enumerator.Value.Dispose();
				Pool.Free<Box>(ref _enumerator);
				_state = State.EnterChild;
				goto case State.EnterChild;
			case State.Finished:
				return false;
			default:
				throw new NotSupportedException();
			}
		}

		public void Dispose()
		{
			if (_enumerator != null)
			{
				_enumerator.Value.Dispose();
				Pool.Free<Box>(ref _enumerator);
			}
		}

		public void Reset()
		{
			throw new NotSupportedException();
		}
	}

	private const float MIN_TIME_BETWEEN_PUSHES = 1f;

	public TimeSince timeSinceLastPush = default(TimeSince);

	private bool prevSleeping;

	private float nextCollisionFXTime;

	private CollisionDetectionMode savedCollisionDetectionMode;

	private BaseVehicle pendingLoad;

	private Queue<BasePlayer> recentDrivers = new Queue<BasePlayer>();

	private Action clearRecentDriverAction = null;

	private float safeAreaRadius;

	private Vector3 safeAreaOrigin;

	private float spawnTime = -1f;

	[Tooltip("Allow players to mount other mountables/ladders from this vehicle")]
	public bool mountChaining = true;

	public ClippingCheckMode clippingChecks = ClippingCheckMode.OnMountOnly;

	public bool checkVehicleClipping;

	public DismountStyle dismountStyle = DismountStyle.Closest;

	public bool shouldShowHudHealth = false;

	public bool ignoreDamageFromOutside = false;

	[Header("Rigidbody (Optional)")]
	public Rigidbody rigidBody;

	[Header("Mount Points")]
	public List<MountPointInfo> mountPoints;

	public bool doClippingAndVisChecks = true;

	[Header("Damage")]
	public DamageRenderer damageRenderer = null;

	[FormerlySerializedAs("explosionDamageMultiplier")]
	public float explosionForceMultiplier = 400f;

	public float explosionForceMax = 75000f;

	public const Flags Flag_OnlyOwnerEntry = Flags.Locked;

	public const Flags Flag_Headlights = Flags.Reserved5;

	public const Flags Flag_Stationary = Flags.Reserved7;

	public const Flags Flag_SeatsFull = Flags.Reserved11;

	protected const Flags Flag_AnyMounted = Flags.InUse;

	private readonly List<BaseVehicle> childVehicles = new List<BaseVehicle>(0);

	public virtual bool AlwaysAllowBradleyTargeting => false;

	protected bool RecentlyPushed => TimeSince.op_Implicit(timeSinceLastPush) < 1f;

	protected override bool PositionTickFixedTime => true;

	protected virtual bool CanSwapSeats => true;

	public bool IsMovingOrOn => IsMoving() || IsOn();

	public override float RealisticMass
	{
		get
		{
			if ((Object)(object)rigidBody != (Object)null)
			{
				return rigidBody.mass;
			}
			return base.RealisticMass;
		}
	}

	public Enumerable allMountPoints => new Enumerable(this);

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("BaseVehicle.OnRpcMessage", 0);
		try
		{
			if (rpc == 2115395408 && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)string.Concat("SV_RPCMessage: ", player, " - RPC_WantsPush "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_WantsPush", 0);
				try
				{
					TimeWarning val3 = TimeWarning.New("Conditions", 0);
					try
					{
						if (!RPC_Server.MaxDistance.Test(2115395408u, "RPC_WantsPush", this, player, 5f))
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
							RPC_WantsPush(msg2);
						}
						finally
						{
							((IDisposable)val4)?.Dispose();
						}
					}
					catch (Exception ex)
					{
						Debug.LogException(ex);
						player.Kick("RPC Error in RPC_WantsPush");
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

	public override void OnAttacked(HitInfo info)
	{
		if (IsSafe() && !info.damageTypes.Has(DamageType.Decay))
		{
			info.damageTypes.ScaleAll(0f);
		}
		base.OnAttacked(info);
	}

	public override void PostServerLoad()
	{
		base.PostServerLoad();
		ClearOwnerEntry();
		CheckAndSpawnMountPoints();
	}

	public override void Save(SaveInfo info)
	{
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		base.Save(info);
		if (!base.isServer || !info.forDisk)
		{
			return;
		}
		info.msg.baseVehicle = Pool.Get<BaseVehicle>();
		info.msg.baseVehicle.mountPoints = Pool.GetList<MountPoint>();
		for (int i = 0; i < mountPoints.Count; i++)
		{
			MountPointInfo mountPointInfo = mountPoints[i];
			if (!((Object)(object)mountPointInfo.mountable == (Object)null))
			{
				MountPoint val = Pool.Get<MountPoint>();
				val.index = i;
				val.mountableId = mountPointInfo.mountable.net.ID;
				info.msg.baseVehicle.mountPoints.Add(val);
			}
		}
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (base.isServer && info.fromDisk && info.msg.baseVehicle != null)
		{
			BaseVehicle obj = pendingLoad;
			if (obj != null)
			{
				obj.Dispose();
			}
			pendingLoad = info.msg.baseVehicle;
			info.msg.baseVehicle = null;
		}
	}

	public override float GetNetworkTime()
	{
		return Time.fixedTime;
	}

	public override void VehicleFixedUpdate()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		base.VehicleFixedUpdate();
		if (clippingChecks != 0 && AnyMounted())
		{
			Vector3 val = ((Component)this).transform.TransformPoint(((Bounds)(ref bounds)).center);
			Collider[] array = Physics.OverlapBox(val, ((Bounds)(ref bounds)).extents, ((Component)this).transform.rotation, GetClipCheckMask());
			if (array.Length != 0)
			{
				CheckSeatsForClipping();
			}
		}
		if ((Object)(object)rigidBody != (Object)null)
		{
			SetFlag(Flags.Reserved7, DetermineIfStationary());
			bool flag = rigidBody.IsSleeping();
			if (prevSleeping && !flag)
			{
				OnServerWake();
			}
			else if (!prevSleeping && flag)
			{
				OnServerSleep();
			}
			prevSleeping = flag;
		}
		if (OnlyOwnerAccessible() && safeAreaRadius != -1f && Vector3.Distance(((Component)this).transform.position, safeAreaOrigin) > safeAreaRadius)
		{
			ClearOwnerEntry();
		}
	}

	private int GetClipCheckMask()
	{
		int num = (IsFlipped() ? 1218511105 : 1210122497);
		if (checkVehicleClipping)
		{
			num |= 0x2000;
		}
		return num;
	}

	protected virtual bool DetermineIfStationary()
	{
		return rigidBody.IsSleeping() && !AnyMounted();
	}

	public override Vector3 GetLocalVelocityServer()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)rigidBody == (Object)null)
		{
			return Vector3.zero;
		}
		return rigidBody.velocity;
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
		if ((Object)(object)rigidBody == (Object)null)
		{
			return Quaternion.identity;
		}
		return Quaternion.Euler(rigidBody.angularVelocity * 57.29578f);
	}

	public virtual int StartingFuelUnits()
	{
		EntityFuelSystem fuelSystem = GetFuelSystem();
		if (fuelSystem != null)
		{
			return Mathf.FloorToInt((float)fuelSystem.GetFuelCapacity() * 0.2f);
		}
		return 0;
	}

	public bool InSafeZone()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		return InSafeZone(triggers, ((Component)this).transform.position);
	}

	public static bool InSafeZone(List<TriggerBase> triggers, Vector3 position)
	{
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		BaseGameMode activeGameMode = BaseGameMode.GetActiveGameMode(serverside: true);
		if ((Object)(object)activeGameMode != (Object)null && !activeGameMode.safeZone)
		{
			return false;
		}
		float num = 0f;
		if (triggers != null)
		{
			for (int i = 0; i < triggers.Count; i++)
			{
				TriggerSafeZone triggerSafeZone = triggers[i] as TriggerSafeZone;
				if (!((Object)(object)triggerSafeZone == (Object)null))
				{
					float safeLevel = triggerSafeZone.GetSafeLevel(position);
					if (safeLevel > num)
					{
						num = safeLevel;
					}
				}
			}
		}
		return num > 0f;
	}

	public virtual bool IsSeatVisible(BaseMountable mountable, Vector3 eyePos, int mask = 1218511105)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		if (!doClippingAndVisChecks)
		{
			return true;
		}
		if ((Object)(object)mountable == (Object)null)
		{
			return false;
		}
		Vector3 position = ((Component)mountable).transform.position;
		Vector3 p = position + ((Component)this).transform.up * 0.15f;
		return GamePhysics.LineOfSight(eyePos, p, mask);
	}

	public virtual bool IsSeatClipping(BaseMountable mountable)
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		if (!doClippingAndVisChecks)
		{
			return false;
		}
		if ((Object)(object)mountable == (Object)null)
		{
			return false;
		}
		int clipCheckMask = GetClipCheckMask();
		Vector3 position = ((Component)mountable.eyePositionOverride).transform.position;
		Vector3 position2 = ((Component)mountable).transform.position;
		Vector3 val = position - position2;
		float num = 0.4f;
		if (mountable.modifiesPlayerCollider)
		{
			num = Mathf.Min(num, mountable.customPlayerCollider.radius);
		}
		Vector3 val2 = position - val * (num - 0.2f);
		bool result = false;
		if (checkVehicleClipping)
		{
			List<Collider> list = Pool.GetList<Collider>();
			if (clippingChecks == ClippingCheckMode.AlwaysHeadOnly)
			{
				GamePhysics.OverlapSphere(val2, num, list, clipCheckMask, (QueryTriggerInteraction)1);
			}
			else
			{
				Vector3 point = position2 + val * (num + 0.05f);
				GamePhysics.OverlapCapsule(val2, point, num, list, clipCheckMask, (QueryTriggerInteraction)1);
			}
			foreach (Collider item in list)
			{
				BaseEntity baseEntity = item.ToBaseEntity();
				if ((Object)(object)baseEntity != (Object)(object)this && !EqualNetID((BaseNetworkable)baseEntity))
				{
					result = true;
					break;
				}
			}
			Pool.FreeList<Collider>(ref list);
		}
		else if (clippingChecks == ClippingCheckMode.AlwaysHeadOnly)
		{
			result = GamePhysics.CheckSphere(val2, num, clipCheckMask, (QueryTriggerInteraction)1);
		}
		else
		{
			Vector3 end = position2 + val * (num + 0.05f);
			result = GamePhysics.CheckCapsule(val2, end, num, clipCheckMask, (QueryTriggerInteraction)1);
		}
		return result;
	}

	public virtual void CheckSeatsForClipping()
	{
		foreach (MountPointInfo mountPoint in mountPoints)
		{
			BaseMountable mountable = mountPoint.mountable;
			if (!((Object)(object)mountable == (Object)null) && mountable.AnyMounted() && IsSeatClipping(mountable))
			{
				SeatClippedWorld(mountable);
			}
		}
	}

	public virtual void SeatClippedWorld(BaseMountable mountable)
	{
		mountable.DismountPlayer(mountable.GetMounted());
	}

	public override void MounteeTookDamage(BasePlayer mountee, HitInfo info)
	{
	}

	public override void DismountAllPlayers()
	{
		foreach (MountPointInfo allMountPoint in allMountPoints)
		{
			if ((Object)(object)allMountPoint.mountable != (Object)null)
			{
				allMountPoint.mountable.DismountAllPlayers();
			}
		}
	}

	public override void ServerInit()
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		base.ServerInit();
		clearRecentDriverAction = ClearRecentDriver;
		prevSleeping = false;
		if ((Object)(object)rigidBody != (Object)null)
		{
			savedCollisionDetectionMode = rigidBody.collisionDetectionMode;
		}
	}

	public virtual void SpawnSubEntities()
	{
		CheckAndSpawnMountPoints();
	}

	public virtual bool AdminFixUp(int tier)
	{
		if (IsDead())
		{
			return false;
		}
		GetFuelSystem()?.AdminAddFuel();
		SetHealth(MaxHealth());
		SendNetworkUpdate();
		return true;
	}

	private void OnPhysicsNeighbourChanged()
	{
		if ((Object)(object)rigidBody != (Object)null)
		{
			rigidBody.WakeUp();
		}
	}

	private void CheckAndSpawnMountPoints()
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		if (pendingLoad?.mountPoints != null)
		{
			foreach (MountPoint mountPoint in pendingLoad.mountPoints)
			{
				EntityRef<BaseMountable> entityRef = new EntityRef<BaseMountable>(mountPoint.mountableId);
				if (!entityRef.IsValid(serverside: true))
				{
					Debug.LogError((object)$"Loaded a mountpoint which doesn't exist: {mountPoint.index}", (Object)(object)this);
					continue;
				}
				if (mountPoint.index < 0 || mountPoint.index >= mountPoints.Count)
				{
					Debug.LogError((object)$"Loaded a mountpoint which has no info: {mountPoint.index}", (Object)(object)this);
					entityRef.Get(serverside: true).Kill();
					continue;
				}
				MountPointInfo mountPointInfo = mountPoints[mountPoint.index];
				if ((Object)(object)mountPointInfo.mountable != (Object)null)
				{
					Debug.LogError((object)$"Loading a mountpoint after one was already set: {mountPoint.index}", (Object)(object)this);
					mountPointInfo.mountable.Kill();
				}
				mountPointInfo.mountable = entityRef.Get(serverside: true);
				if (!mountPointInfo.mountable.enableSaving)
				{
					mountPointInfo.mountable.EnableSaving(wants: true);
				}
			}
		}
		BaseVehicle obj = pendingLoad;
		if (obj != null)
		{
			obj.Dispose();
		}
		pendingLoad = null;
		for (int i = 0; i < mountPoints.Count; i++)
		{
			SpawnMountPoint(mountPoints[i], model);
		}
		UpdateMountFlags();
	}

	public override void Spawn()
	{
		base.Spawn();
		if (base.isServer && !Application.isLoadingSave)
		{
			SpawnSubEntities();
		}
	}

	public override void Hurt(HitInfo info)
	{
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		if (!IsDead() && (Object)(object)rigidBody != (Object)null && !rigidBody.isKinematic)
		{
			float num = info.damageTypes.Get(DamageType.Explosion) + info.damageTypes.Get(DamageType.AntiVehicle);
			if (num > 3f)
			{
				float num2 = Mathf.Min(num * explosionForceMultiplier, explosionForceMax);
				rigidBody.AddExplosionForce(num2, info.HitPositionWorld, 1f, 2.5f);
			}
		}
		base.Hurt(info);
	}

	public int NumMounted()
	{
		if (!HasMountPoints())
		{
			return AnyMounted() ? 1 : 0;
		}
		int num = 0;
		foreach (MountPointInfo allMountPoint in allMountPoints)
		{
			if ((Object)(object)allMountPoint.mountable != (Object)null && (Object)(object)allMountPoint.mountable.GetMounted() != (Object)null)
			{
				num++;
			}
		}
		return num;
	}

	public virtual int MaxMounted()
	{
		if (!HasMountPoints())
		{
			return 1;
		}
		int num = 0;
		foreach (MountPointInfo allMountPoint in allMountPoints)
		{
			if ((Object)(object)allMountPoint.mountable != (Object)null)
			{
				num++;
			}
		}
		return num;
	}

	public bool HasDriver()
	{
		if (HasMountPoints())
		{
			foreach (MountPointInfo allMountPoint in allMountPoints)
			{
				if (allMountPoint != null && (Object)(object)allMountPoint.mountable != (Object)null && allMountPoint.isDriver && allMountPoint.mountable.AnyMounted())
				{
					return true;
				}
			}
			return false;
		}
		return base.AnyMounted();
	}

	public bool IsDriver(BasePlayer player)
	{
		if (HasMountPoints())
		{
			foreach (MountPointInfo allMountPoint in allMountPoints)
			{
				if (allMountPoint != null && (Object)(object)allMountPoint.mountable != (Object)null && allMountPoint.isDriver)
				{
					BasePlayer mounted = allMountPoint.mountable.GetMounted();
					if ((Object)(object)mounted != (Object)null && (Object)(object)mounted == (Object)(object)player)
					{
						return true;
					}
				}
			}
		}
		else if ((Object)(object)_mounted != (Object)null)
		{
			return (Object)(object)_mounted == (Object)(object)player;
		}
		return false;
	}

	public BasePlayer GetDriver()
	{
		if (HasMountPoints())
		{
			foreach (MountPointInfo allMountPoint in allMountPoints)
			{
				if (allMountPoint != null && (Object)(object)allMountPoint.mountable != (Object)null && allMountPoint.isDriver)
				{
					BasePlayer mounted = allMountPoint.mountable.GetMounted();
					if ((Object)(object)mounted != (Object)null)
					{
						return mounted;
					}
				}
			}
		}
		else if ((Object)(object)_mounted != (Object)null)
		{
			return _mounted;
		}
		return null;
	}

	public void GetDrivers(List<BasePlayer> drivers)
	{
		if (HasMountPoints())
		{
			foreach (MountPointInfo allMountPoint in allMountPoints)
			{
				if (allMountPoint != null && (Object)(object)allMountPoint.mountable != (Object)null && allMountPoint.isDriver)
				{
					BasePlayer mounted = allMountPoint.mountable.GetMounted();
					if ((Object)(object)mounted != (Object)null)
					{
						drivers.Add(mounted);
					}
				}
			}
			return;
		}
		if ((Object)(object)_mounted != (Object)null)
		{
			drivers.Add(_mounted);
		}
	}

	public BasePlayer GetPlayerDamageInitiator()
	{
		if (HasDriver())
		{
			return GetDriver();
		}
		return (recentDrivers.Count > 0) ? recentDrivers.Peek() : null;
	}

	public int GetPlayerSeat(BasePlayer player)
	{
		if (!HasMountPoints() && (Object)(object)GetMounted() == (Object)(object)player)
		{
			return 0;
		}
		int num = 0;
		foreach (MountPointInfo allMountPoint in allMountPoints)
		{
			if ((Object)(object)allMountPoint.mountable != (Object)null && (Object)(object)allMountPoint.mountable.GetMounted() == (Object)(object)player)
			{
				return num;
			}
			num++;
		}
		return -1;
	}

	public MountPointInfo GetPlayerSeatInfo(BasePlayer player)
	{
		if (!HasMountPoints())
		{
			return null;
		}
		foreach (MountPointInfo allMountPoint in allMountPoints)
		{
			if ((Object)(object)allMountPoint.mountable != (Object)null && (Object)(object)allMountPoint.mountable.GetMounted() == (Object)(object)player)
			{
				return allMountPoint;
			}
		}
		return null;
	}

	public bool IsVehicleMountPoint(BaseMountable bm)
	{
		if (!HasMountPoints() || (Object)(object)bm == (Object)null)
		{
			return false;
		}
		foreach (MountPointInfo allMountPoint in allMountPoints)
		{
			if ((Object)(object)allMountPoint.mountable == (Object)(object)bm)
			{
				return true;
			}
		}
		return false;
	}

	public virtual bool IsPlayerSeatSwapValid(BasePlayer player, int fromIndex, int toIndex)
	{
		return true;
	}

	public void SwapSeats(BasePlayer player, int targetSeat = 0)
	{
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		if (!HasMountPoints() || !CanSwapSeats)
		{
			return;
		}
		int playerSeat = GetPlayerSeat(player);
		if (playerSeat == -1)
		{
			return;
		}
		BaseMountable mountable = GetMountPoint(playerSeat).mountable;
		int num = playerSeat;
		BaseMountable baseMountable = null;
		if ((Object)(object)baseMountable == (Object)null)
		{
			int num2 = NumSwappableSeats();
			for (int i = 0; i < num2; i++)
			{
				num++;
				if (num >= num2)
				{
					num = 0;
				}
				MountPointInfo mountPoint = GetMountPoint(num);
				if ((Object)(object)mountPoint?.mountable != (Object)null && !mountPoint.mountable.AnyMounted() && mountPoint.mountable.CanSwapToThis(player) && !IsSeatClipping(mountPoint.mountable) && IsSeatVisible(mountPoint.mountable, player.eyes.position) && IsPlayerSeatSwapValid(player, playerSeat, num))
				{
					baseMountable = mountPoint.mountable;
					break;
				}
			}
		}
		if ((Object)(object)baseMountable != (Object)null && (Object)(object)baseMountable != (Object)(object)mountable)
		{
			mountable.DismountPlayer(player, lite: true);
			baseMountable.MountPlayer(player);
			player.MarkSwapSeat();
		}
	}

	public virtual int NumSwappableSeats()
	{
		return MaxMounted();
	}

	public bool HasDriverMountPoints()
	{
		foreach (MountPointInfo allMountPoint in allMountPoints)
		{
			if (allMountPoint.isDriver)
			{
				return true;
			}
		}
		return false;
	}

	public bool OnlyOwnerAccessible()
	{
		return HasFlag(Flags.Locked);
	}

	public bool IsDespawnEligable()
	{
		return spawnTime == -1f || spawnTime + 300f < Time.realtimeSinceStartup;
	}

	public void SetupOwner(BasePlayer owner, Vector3 newSafeAreaOrigin, float newSafeAreaRadius)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)owner != (Object)null)
		{
			creatorEntity = owner;
			SetFlag(Flags.Locked, b: true);
			safeAreaRadius = newSafeAreaRadius;
			safeAreaOrigin = newSafeAreaOrigin;
			spawnTime = Time.realtimeSinceStartup;
		}
	}

	public void ClearOwnerEntry()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		creatorEntity = null;
		SetFlag(Flags.Locked, b: false);
		safeAreaRadius = -1f;
		safeAreaOrigin = Vector3.zero;
	}

	public virtual EntityFuelSystem GetFuelSystem()
	{
		return null;
	}

	public bool IsSafe()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		return OnlyOwnerAccessible() && Vector3.Distance(safeAreaOrigin, ((Component)this).transform.position) <= safeAreaRadius;
	}

	public override void ScaleDamageForPlayer(BasePlayer player, HitInfo info)
	{
		if (IsSafe())
		{
			info.damageTypes.ScaleAll(0f);
		}
		base.ScaleDamageForPlayer(player, info);
	}

	public BaseMountable GetIdealMountPoint(Vector3 eyePos, Vector3 pos, BasePlayer playerFor = null)
	{
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)playerFor == (Object)null)
		{
			return null;
		}
		if (!HasMountPoints())
		{
			return this;
		}
		BasePlayer basePlayer = creatorEntity as BasePlayer;
		bool flag = (Object)(object)basePlayer != (Object)null;
		bool flag2 = flag && basePlayer.Team != null;
		bool flag3 = flag && (Object)(object)playerFor == (Object)(object)basePlayer;
		if (!flag3 && flag && OnlyOwnerAccessible() && (Object)(object)playerFor != (Object)null && (playerFor.Team == null || !playerFor.Team.members.Contains(basePlayer.userID)))
		{
			return null;
		}
		BaseMountable result = null;
		float num = float.PositiveInfinity;
		foreach (MountPointInfo allMountPoint in allMountPoints)
		{
			if (allMountPoint.mountable.AnyMounted())
			{
				continue;
			}
			float num2 = Vector3.Distance(allMountPoint.mountable.mountAnchor.position, pos);
			if (num2 > num)
			{
				continue;
			}
			if (IsSeatClipping(allMountPoint.mountable))
			{
				if (Application.isEditor)
				{
					Debug.Log((object)$"Skipping seat {allMountPoint.mountable} - it's clipping");
				}
			}
			else if (!IsSeatVisible(allMountPoint.mountable, eyePos))
			{
				if (Application.isEditor)
				{
					Debug.Log((object)$"Skipping seat {allMountPoint.mountable} - it's not visible");
				}
			}
			else if (!(OnlyOwnerAccessible() && flag3) || flag2 || allMountPoint.isDriver)
			{
				result = allMountPoint.mountable;
				num = num2;
			}
		}
		return result;
	}

	public virtual bool MountEligable(BasePlayer player)
	{
		if ((Object)(object)creatorEntity != (Object)null && OnlyOwnerAccessible() && (Object)(object)player != (Object)(object)creatorEntity)
		{
			BasePlayer basePlayer = creatorEntity as BasePlayer;
			if ((Object)(object)basePlayer != (Object)null && basePlayer.Team != null && !basePlayer.Team.members.Contains(player.userID))
			{
				return false;
			}
		}
		BaseVehicle baseVehicle = VehicleParent();
		if ((Object)(object)baseVehicle != (Object)null)
		{
			return baseVehicle.MountEligable(player);
		}
		return true;
	}

	public int GetIndexFromSeat(BaseMountable seat)
	{
		int num = 0;
		foreach (MountPointInfo allMountPoint in allMountPoints)
		{
			if ((Object)(object)allMountPoint.mountable == (Object)(object)seat)
			{
				return num;
			}
			num++;
		}
		return -1;
	}

	public virtual void PlayerMounted(BasePlayer player, BaseMountable seat)
	{
	}

	public virtual void PrePlayerDismount(BasePlayer player, BaseMountable seat)
	{
	}

	public virtual void PlayerDismounted(BasePlayer player, BaseMountable seat)
	{
		recentDrivers.Enqueue(player);
		if (!((FacepunchBehaviour)this).IsInvoking(clearRecentDriverAction))
		{
			((FacepunchBehaviour)this).Invoke(clearRecentDriverAction, 3f);
		}
	}

	public void TryShowCollisionFX(Collision collision, GameObjectRef effectGO)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		ContactPoint contact = collision.GetContact(0);
		TryShowCollisionFX(((ContactPoint)(ref contact)).point, effectGO);
	}

	public void TryShowCollisionFX(Vector3 contactPoint, GameObjectRef effectGO)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		if (!(Time.time < nextCollisionFXTime))
		{
			nextCollisionFXTime = Time.time + 0.25f;
			if (effectGO.isValid)
			{
				contactPoint += (((Component)this).transform.position - contactPoint) * 0.25f;
				Effect.server.Run(effectGO.resourcePath, contactPoint, ((Component)this).transform.up);
			}
		}
	}

	public void SetToKinematic()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		if (!((Object)(object)rigidBody == (Object)null) && !rigidBody.isKinematic)
		{
			savedCollisionDetectionMode = rigidBody.collisionDetectionMode;
			rigidBody.collisionDetectionMode = (CollisionDetectionMode)0;
			rigidBody.isKinematic = true;
		}
	}

	public void SetToNonKinematic()
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		if (!((Object)(object)rigidBody == (Object)null) && rigidBody.isKinematic)
		{
			rigidBody.isKinematic = false;
			rigidBody.collisionDetectionMode = savedCollisionDetectionMode;
		}
	}

	public override void UpdateMountFlags()
	{
		int num = NumMounted();
		SetFlag(Flags.InUse, num > 0);
		SetFlag(Flags.Reserved11, num == MaxMounted());
		BaseVehicle baseVehicle = VehicleParent();
		if ((Object)(object)baseVehicle != (Object)null)
		{
			baseVehicle.UpdateMountFlags();
		}
	}

	private void ClearRecentDriver()
	{
		if (recentDrivers.Count > 0)
		{
			recentDrivers.Dequeue();
		}
		if (recentDrivers.Count > 0)
		{
			((FacepunchBehaviour)this).Invoke(clearRecentDriverAction, 3f);
		}
	}

	public override void AttemptMount(BasePlayer player, bool doMountChecks = true)
	{
		if ((Object)(object)_mounted != (Object)null || !MountEligable(player))
		{
			return;
		}
		BaseMountable idealMountPointFor = GetIdealMountPointFor(player);
		if (!((Object)(object)idealMountPointFor == (Object)null))
		{
			if ((Object)(object)idealMountPointFor == (Object)(object)this)
			{
				base.AttemptMount(player, doMountChecks);
			}
			else
			{
				idealMountPointFor.AttemptMount(player, doMountChecks);
			}
			if ((Object)(object)player.GetMountedVehicle() == (Object)(object)this)
			{
				PlayerMounted(player, idealMountPointFor);
			}
		}
	}

	protected BaseMountable GetIdealMountPointFor(BasePlayer player)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		return GetIdealMountPoint(player.eyes.position, player.eyes.position + player.eyes.HeadForward() * 1f, player);
	}

	public override bool GetDismountPosition(BasePlayer player, out Vector3 res)
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		BaseVehicle baseVehicle = VehicleParent();
		if ((Object)(object)baseVehicle != (Object)null)
		{
			return baseVehicle.GetDismountPosition(player, out res);
		}
		List<Vector3> list = Pool.GetList<Vector3>();
		Transform[] array = dismountPositions;
		foreach (Transform val in array)
		{
			if (ValidDismountPosition(player, ((Component)val).transform.position))
			{
				list.Add(((Component)val).transform.position);
				if (dismountStyle == DismountStyle.Ordered)
				{
					break;
				}
			}
		}
		if (list.Count == 0)
		{
			Debug.LogWarning((object)("Failed to find dismount position for player :" + player.displayName + " / " + player.userID + " on obj : " + ((Object)((Component)this).gameObject).name));
			Pool.FreeList<Vector3>(ref list);
			res = ((Component)player).transform.position;
			return false;
		}
		Vector3 pos = ((Component)player).transform.position;
		list.Sort((Vector3 a, Vector3 b) => Vector3.Distance(a, pos).CompareTo(Vector3.Distance(b, pos)));
		res = list[0];
		Pool.FreeList<Vector3>(ref list);
		return true;
	}

	private BaseMountable SpawnMountPoint(MountPointInfo mountToSpawn, Model model)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)mountToSpawn.mountable != (Object)null)
		{
			return mountToSpawn.mountable;
		}
		Vector3 val = Quaternion.Euler(mountToSpawn.rot) * Vector3.forward;
		Vector3 pos = mountToSpawn.pos;
		Vector3 up = Vector3.up;
		if (mountToSpawn.bone != "")
		{
			Transform val2 = model.FindBone(mountToSpawn.bone);
			Vector3 position = ((Component)val2).transform.position;
			position += ((Component)this).transform.TransformDirection(mountToSpawn.pos);
			pos = position;
			val = ((Component)this).transform.TransformDirection(val);
			up = ((Component)this).transform.up;
		}
		BaseEntity baseEntity = GameManager.server.CreateEntity(mountToSpawn.prefab.resourcePath, pos, Quaternion.LookRotation(val, up));
		BaseMountable baseMountable = baseEntity as BaseMountable;
		if ((Object)(object)baseMountable != (Object)null)
		{
			if (enableSaving != baseMountable.enableSaving)
			{
				baseMountable.EnableSaving(enableSaving);
			}
			if (mountToSpawn.bone != "")
			{
				baseMountable.SetParent(this, mountToSpawn.bone, worldPositionStays: true, sendImmediate: true);
			}
			else
			{
				baseMountable.SetParent(this);
			}
			baseMountable.Spawn();
			mountToSpawn.mountable = baseMountable;
		}
		else
		{
			Debug.LogError((object)"MountPointInfo prefab is not a BaseMountable. Cannot spawn mount point.");
			if ((Object)(object)baseEntity != (Object)null)
			{
				baseEntity.Kill();
			}
		}
		return baseMountable;
	}

	[RPC_Server]
	[RPC_Server.MaxDistance(5f)]
	public void RPC_WantsPush(RPCMessage msg)
	{
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		BasePlayer player = msg.player;
		if (!player.isMounted && !RecentlyPushed && CanPushNow(player) && !((Object)(object)rigidBody == (Object)null) && (!OnlyOwnerAccessible() || !((Object)(object)player != (Object)(object)creatorEntity)))
		{
			player.metabolism.calories.Subtract(3f);
			player.metabolism.SendChangesToClient();
			if (rigidBody.IsSleeping())
			{
				rigidBody.WakeUp();
			}
			DoPushAction(player);
			timeSinceLastPush = TimeSince.op_Implicit(0f);
		}
	}

	protected virtual void DoPushAction(BasePlayer player)
	{
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)rigidBody == (Object)null)
		{
			return;
		}
		if (IsFlipped())
		{
			float num = rigidBody.mass * 8f;
			Vector3 val = Vector3.forward * num;
			Vector3 val2 = ((Component)this).transform.InverseTransformVector(((Component)this).transform.position - ((Component)player).transform.position);
			if (Vector3.Dot(val2, Vector3.right) > 0f)
			{
				val *= -1f;
			}
			if (((Component)this).transform.up.y < 0f)
			{
				val *= -1f;
			}
			rigidBody.AddRelativeTorque(val, (ForceMode)1);
		}
		else
		{
			Vector3 val3 = ((Component)this).transform.position - player.eyes.position;
			Vector3 val4 = Vector3.ProjectOnPlane(val3, ((Component)this).transform.up);
			Vector3 normalized = ((Vector3)(ref val4)).normalized;
			float num2 = rigidBody.mass * 4f;
			rigidBody.AddForce(normalized * num2, (ForceMode)1);
		}
	}

	protected virtual void OnServerWake()
	{
	}

	protected virtual void OnServerSleep()
	{
	}

	public bool IsStationary()
	{
		return HasFlag(Flags.Reserved7);
	}

	public bool IsMoving()
	{
		return !HasFlag(Flags.Reserved7);
	}

	public bool IsAuthed(BasePlayer player)
	{
		foreach (BaseEntity child in children)
		{
			VehiclePrivilege vehiclePrivilege = child as VehiclePrivilege;
			if ((Object)(object)vehiclePrivilege == (Object)null)
			{
				continue;
			}
			return vehiclePrivilege.IsAuthed(player);
		}
		return true;
	}

	public override bool AnyMounted()
	{
		return HasFlag(Flags.InUse);
	}

	public override bool PlayerIsMounted(BasePlayer player)
	{
		return player.IsValid() && (Object)(object)player.GetMountedVehicle() == (Object)(object)this;
	}

	protected virtual bool CanPushNow(BasePlayer pusher)
	{
		return !IsOn();
	}

	public bool HasMountPoints()
	{
		if (mountPoints.Count > 0)
		{
			return true;
		}
		using (Enumerator enumerator = allMountPoints.GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				MountPointInfo current = enumerator.Current;
				return true;
			}
		}
		return false;
	}

	public override bool CanBeLooted(BasePlayer player)
	{
		return IsAlive() && !base.IsDestroyed && (Object)(object)player != (Object)null;
	}

	public bool IsFlipped()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		return Vector3.Dot(Vector3.up, ((Component)this).transform.up) <= 0f;
	}

	public virtual bool IsVehicleRoot()
	{
		return true;
	}

	public override bool DirectlyMountable()
	{
		return IsVehicleRoot();
	}

	public override BaseVehicle VehicleParent()
	{
		return null;
	}

	protected override void OnChildAdded(BaseEntity child)
	{
		base.OnChildAdded(child);
		if (!IsDead() && !base.IsDestroyed && child is BaseVehicle baseVehicle && !baseVehicle.IsVehicleRoot() && !childVehicles.Contains(baseVehicle))
		{
			childVehicles.Add(baseVehicle);
		}
	}

	protected override void OnChildRemoved(BaseEntity child)
	{
		base.OnChildRemoved(child);
		if (child is BaseVehicle baseVehicle && !baseVehicle.IsVehicleRoot())
		{
			childVehicles.Remove(baseVehicle);
		}
	}

	public MountPointInfo GetMountPoint(int index)
	{
		if (index < 0)
		{
			return null;
		}
		if (index < mountPoints.Count)
		{
			return mountPoints[index];
		}
		index -= mountPoints.Count;
		int num = 0;
		foreach (BaseVehicle childVehicle in childVehicles)
		{
			if ((Object)(object)childVehicle == (Object)null)
			{
				continue;
			}
			foreach (MountPointInfo allMountPoint in childVehicle.allMountPoints)
			{
				if (num == index)
				{
					return allMountPoint;
				}
				num++;
			}
		}
		return null;
	}
}
