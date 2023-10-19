using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

public class TrainCar : BaseVehicle, ITrainCollidable, IPrefabPreProcess, TriggerHurtNotChild.IHurtTriggerUser, TrainTrackSpline.ITrainTrackUser
{
	public enum TrainCarType
	{
		Wagon,
		Engine,
		Other
	}

	[Header("Train Car")]
	[SerializeField]
	private float corpseSeconds = 60f;

	[SerializeField]
	private TriggerTrainCollisions frontCollisionTrigger;

	[SerializeField]
	private TriggerTrainCollisions rearCollisionTrigger;

	[SerializeField]
	private float collisionDamageDivide = 100000f;

	[SerializeField]
	private float derailCollisionForce = 130000f;

	[SerializeField]
	private TriggerHurtNotChild hurtTriggerFront;

	[SerializeField]
	private TriggerHurtNotChild hurtTriggerRear;

	[SerializeField]
	private GameObject[] hurtOrRepelTriggersInternal;

	[SerializeField]
	private float hurtTriggerMinSpeed = 1f;

	[SerializeField]
	private Transform centreOfMassTransform;

	[SerializeField]
	private Transform frontBogiePivot;

	[SerializeField]
	private bool frontBogieCanRotate = true;

	[SerializeField]
	private Transform rearBogiePivot;

	[SerializeField]
	private bool rearBogieCanRotate = true;

	[SerializeField]
	private Transform[] wheelVisuals;

	[SerializeField]
	private float wheelRadius = 0.615f;

	[FormerlySerializedAs("fxFinalExplosion")]
	[SerializeField]
	private GameObjectRef fxDestroyed;

	[SerializeField]
	protected TriggerParent platformParentTrigger;

	public GameObjectRef collisionEffect;

	public Transform frontCoupling;

	public Transform frontCouplingPivot;

	public Transform rearCoupling;

	public Transform rearCouplingPivot;

	[SerializeField]
	private SoundDefinition coupleSound;

	[SerializeField]
	private SoundDefinition uncoupleSound;

	[SerializeField]
	private TrainCarAudio trainCarAudio;

	[FormerlySerializedAs("frontCoupleFx")]
	[SerializeField]
	private ParticleSystem frontCouplingChangedFx;

	[FormerlySerializedAs("rearCoupleFx")]
	[SerializeField]
	private ParticleSystem rearCouplingChangedFx;

	[FormerlySerializedAs("fxCoupling")]
	[SerializeField]
	private ParticleSystem newCouplingFX;

	[SerializeField]
	private float decayTimeMultiplier = 1f;

	[SerializeField]
	[ReadOnly]
	private Vector3 frontBogieLocalOffset;

	[SerializeField]
	[ReadOnly]
	private Vector3 rearBogieLocalOffset;

	[ServerVar(Help = "Population active on the server", ShowInAdminUI = true)]
	public static float population = 2.3f;

	[ServerVar(Help = "Ratio of wagons to train engines that spawn")]
	public static int wagons_per_engine = 2;

	[ServerVar(Help = "How long before a train car despawns")]
	public static float decayminutes = 30f;

	[ReadOnly]
	public float DistFrontWheelToFrontCoupling;

	[ReadOnly]
	public float DistFrontWheelToBackCoupling;

	public TrainCouplingController coupling;

	[NonSerialized]
	public TrainTrackSpline.TrackSelection localTrackSelection;

	public const Flags Flag_LinedUpToUnload = Flags.Reserved4;

	protected bool trainDebug;

	public CompleteTrain completeTrain;

	private bool frontAtEndOfLine;

	private bool rearAtEndOfLine;

	private float frontBogieYRot;

	private float rearBogieYRot;

	private Vector3 spawnOrigin;

	public static float TRAINCAR_MAX_SPEED = 25f;

	private TrainTrackSpline _frontTrackSection;

	private float distFrontToBackWheel;

	private float initialSpawnTime;

	protected float decayingFor;

	private float decayTickSpacing = 60f;

	private float lastDecayTick;

	public TriggerTrainCollisions FrontCollisionTrigger => frontCollisionTrigger;

	public TriggerTrainCollisions RearCollisionTrigger => rearCollisionTrigger;

	public virtual TrainCarType CarType => TrainCarType.Wagon;

	public bool LinedUpToUnload => HasFlag(Flags.Reserved4);

	public Vector3 Position => ((Component)this).transform.position;

	public float FrontWheelSplineDist { get; private set; }

	public bool FrontAtEndOfLine => frontAtEndOfLine;

	public bool RearAtEndOfLine => rearAtEndOfLine;

	protected virtual bool networkUpdateOnCompleteTrainChange => false;

	public TrainTrackSpline FrontTrackSection
	{
		get
		{
			return _frontTrackSection;
		}
		private set
		{
			if ((Object)(object)_frontTrackSection != (Object)(object)value)
			{
				if ((Object)(object)_frontTrackSection != (Object)null)
				{
					_frontTrackSection.DeregisterTrackUser(this);
				}
				_frontTrackSection = value;
				if ((Object)(object)_frontTrackSection != (Object)null)
				{
					_frontTrackSection.RegisterTrackUser(this);
				}
			}
		}
	}

	public TrainTrackSpline RearTrackSection { get; private set; }

	protected bool IsAtAStation
	{
		get
		{
			if ((Object)(object)FrontTrackSection != (Object)null)
			{
				return FrontTrackSection.isStation;
			}
			return false;
		}
	}

	protected bool IsOnAboveGroundSpawnRail
	{
		get
		{
			if ((Object)(object)FrontTrackSection != (Object)null)
			{
				return FrontTrackSection.aboveGroundSpawn;
			}
			return false;
		}
	}

	private bool RecentlySpawned => Time.time < initialSpawnTime + 2f;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("TrainCar.OnRpcMessage", 0);
		try
		{
			if (rpc == 3930273067u && (Object)(object)player != (Object)null)
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log((object)("SV_RPCMessage: " + ((object)player)?.ToString() + " - RPC_WantsUncouple "));
				}
				TimeWarning val2 = TimeWarning.New("RPC_WantsUncouple", 0);
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
						RPC_WantsUncouple(msg2);
					}
					finally
					{
						((IDisposable)val3)?.Dispose();
					}
				}
				catch (Exception ex)
				{
					Debug.LogException(ex);
					player.Kick("RPC Error in RPC_WantsUncouple");
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

	public override void PreProcess(IPrefabProcessor process, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		base.PreProcess(process, rootObj, name, serverside, clientside, bundling);
		frontBogieLocalOffset = ((Component)this).transform.InverseTransformPoint(frontBogiePivot.position);
		float num = ((!((Object)(object)frontCoupling != (Object)null)) ? (((Bounds)(ref bounds)).extents.z + ((Bounds)(ref bounds)).center.z) : ((Component)this).transform.InverseTransformPoint(frontCoupling.position).z);
		float num2 = ((!((Object)(object)rearCoupling != (Object)null)) ? (0f - ((Bounds)(ref bounds)).extents.z + ((Bounds)(ref bounds)).center.z) : ((Component)this).transform.InverseTransformPoint(rearCoupling.position).z);
		DistFrontWheelToFrontCoupling = num - frontBogieLocalOffset.z;
		DistFrontWheelToBackCoupling = 0f - num2 + frontBogieLocalOffset.z;
		rearBogieLocalOffset = ((Component)this).transform.InverseTransformPoint(rearBogiePivot.position);
	}

	public override void InitShared()
	{
		base.InitShared();
		coupling = new TrainCouplingController(this);
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (info.msg.baseTrain != null && base.isServer)
		{
			frontBogieYRot = info.msg.baseTrain.frontBogieYRot;
			rearBogieYRot = info.msg.baseTrain.rearBogieYRot;
		}
	}

	public override void OnFlagsChanged(Flags old, Flags next)
	{
		base.OnFlagsChanged(old, next);
		if (old != next && base.isServer)
		{
			ServerFlagsChanged(old, next);
		}
	}

	public bool CustomCollision(TrainCar train, TriggerTrainCollisions trainTrigger)
	{
		return false;
	}

	public override float InheritedVelocityScale()
	{
		return 0.5f;
	}

	protected virtual void SetTrackSelection(TrainTrackSpline.TrackSelection trackSelection)
	{
		if (localTrackSelection != trackSelection)
		{
			localTrackSelection = trackSelection;
			if (base.isServer)
			{
				ClientRPC(null, "SetTrackSelection", (sbyte)localTrackSelection);
			}
		}
	}

	protected bool PlayerIsOnPlatform(BasePlayer player)
	{
		return (Object)(object)player.GetParentEntity() == (Object)(object)this;
	}

	public override void ServerInit()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		base.ServerInit();
		spawnOrigin = ((Component)this).transform.position;
		distFrontToBackWheel = Vector3.Distance(GetFrontWheelPos(), GetRearWheelPos());
		rigidBody.centerOfMass = centreOfMassTransform.localPosition;
		UpdateCompleteTrain();
		lastDecayTick = Time.time;
		((FacepunchBehaviour)this).InvokeRandomized((Action)UpdateClients, 0f, 0.15f, 0.02f);
		((FacepunchBehaviour)this).InvokeRandomized((Action)DecayTick, Random.Range(20f, 40f), decayTickSpacing, decayTickSpacing * 0.1f);
	}

	public override void PostServerLoad()
	{
		base.PostServerLoad();
		if (base.health <= 0f)
		{
			ActualDeath();
			return;
		}
		SetFlag(Flags.Reserved2, b: false);
		SetFlag(Flags.Reserved3, b: false);
	}

	public override void Spawn()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		base.Spawn();
		initialSpawnTime = Time.time;
		if (TrainTrackSpline.TryFindTrackNear(GetFrontWheelPos(), 15f, out var splineResult, out var distResult))
		{
			FrontWheelSplineDist = distResult;
			Vector3 tangent;
			Vector3 positionAndTangent = splineResult.GetPositionAndTangent(FrontWheelSplineDist, ((Component)this).transform.forward, out tangent);
			SetTheRestFromFrontWheelData(ref splineResult, positionAndTangent, tangent, localTrackSelection, null, instantMove: true);
			FrontTrackSection = splineResult;
			if (!Application.isLoadingSave && !SpaceIsClear())
			{
				((FacepunchBehaviour)this).Invoke((Action)base.KillMessage, 0f);
			}
		}
		else
		{
			((FacepunchBehaviour)this).Invoke((Action)base.KillMessage, 0f);
		}
	}

	public override void Save(SaveInfo info)
	{
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		base.Save(info);
		info.msg.baseTrain = Pool.Get<BaseTrain>();
		info.msg.baseTrain.time = GetNetworkTime();
		info.msg.baseTrain.frontBogieYRot = frontBogieYRot;
		info.msg.baseTrain.rearBogieYRot = rearBogieYRot;
		if (coupling.frontCoupling.TryGetCoupledToID(out var id))
		{
			info.msg.baseTrain.frontCouplingID = id;
			info.msg.baseTrain.frontCouplingToFront = coupling.frontCoupling.CoupledTo.isFrontCoupling;
		}
		if (coupling.rearCoupling.TryGetCoupledToID(out id))
		{
			info.msg.baseTrain.rearCouplingID = id;
			info.msg.baseTrain.rearCouplingToFront = coupling.rearCoupling.CoupledTo.isFrontCoupling;
		}
	}

	protected virtual void ServerFlagsChanged(Flags old, Flags next)
	{
		if (isSpawned && (next.HasFlag(Flags.Reserved2) != old.HasFlag(Flags.Reserved2) || next.HasFlag(Flags.Reserved3) != old.HasFlag(Flags.Reserved3)))
		{
			UpdateCompleteTrain();
		}
	}

	private void UpdateCompleteTrain()
	{
		List<TrainCar> result = Pool.GetList<TrainCar>();
		coupling.GetAll(ref result);
		if (completeTrain == null || !completeTrain.Matches(result))
		{
			SetNewCompleteTrain(new CompleteTrain(result));
		}
		else
		{
			Pool.FreeList<TrainCar>(ref result);
		}
	}

	public void SetNewCompleteTrain(CompleteTrain ct)
	{
		if (completeTrain != ct)
		{
			RemoveFromCompleteTrain();
			completeTrain = ct;
			if (networkUpdateOnCompleteTrainChange)
			{
				SendNetworkUpdate();
			}
		}
	}

	public override void Hurt(HitInfo info)
	{
		if (!RecentlySpawned)
		{
			base.Hurt(info);
		}
	}

	public override void OnKilled(HitInfo info)
	{
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		float num = info?.damageTypes.Get(DamageType.AntiVehicle) ?? 0f;
		float num2 = info?.damageTypes.Get(DamageType.Explosion) ?? 0f;
		float num3 = info?.damageTypes.Total() ?? 0f;
		if ((num + num2) / num3 > 0.5f || vehicle.cinematictrains || corpseSeconds == 0f)
		{
			if (HasDriver())
			{
				GetDriver().Hurt(float.MaxValue);
			}
			base.OnKilled(info);
		}
		else
		{
			((FacepunchBehaviour)this).Invoke((Action)ActualDeath, corpseSeconds);
		}
		if (base.IsDestroyed && fxDestroyed.isValid)
		{
			Effect.server.Run(fxDestroyed.resourcePath, GetExplosionPos(), Vector3.up, null, broadcast: true);
		}
	}

	protected virtual Vector3 GetExplosionPos()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return GetCentreOfTrainPos();
	}

	public void ActualDeath()
	{
		Kill(DestroyMode.Gib);
	}

	public override void DoRepair(BasePlayer player)
	{
		base.DoRepair(player);
		if (IsDead() && Health() > 0f)
		{
			((FacepunchBehaviour)this).CancelInvoke((Action)ActualDeath);
			lifestate = LifeState.Alive;
		}
	}

	public float GetDamageMultiplier(BaseEntity ent)
	{
		return Mathf.Abs(GetTrackSpeed()) * 1f;
	}

	public void OnHurtTriggerOccupant(BaseEntity hurtEntity, DamageType damageType, float damageTotal)
	{
	}

	internal override void DoServerDestroy()
	{
		if ((Object)(object)FrontTrackSection != (Object)null)
		{
			FrontTrackSection.DeregisterTrackUser(this);
		}
		coupling.Uncouple(front: true);
		coupling.Uncouple(front: false);
		RemoveFromCompleteTrain();
		base.DoServerDestroy();
	}

	private void RemoveFromCompleteTrain()
	{
		if (completeTrain != null)
		{
			if (completeTrain.ContainsOnly(this))
			{
				completeTrain.Dispose();
				completeTrain = null;
			}
			else
			{
				completeTrain.RemoveTrainCar(this);
			}
		}
	}

	public override bool MountEligable(BasePlayer player)
	{
		if (IsDead())
		{
			return false;
		}
		return base.MountEligable(player);
	}

	public override float MaxVelocity()
	{
		return TRAINCAR_MAX_SPEED;
	}

	public float GetTrackSpeed()
	{
		if (completeTrain == null)
		{
			return 0f;
		}
		return completeTrain.GetTrackSpeedFor(this);
	}

	public bool IsCoupledBackwards()
	{
		if (completeTrain == null)
		{
			return false;
		}
		return completeTrain.IsCoupledBackwards(this);
	}

	public float GetPrevTrackSpeed()
	{
		if (completeTrain == null)
		{
			return 0f;
		}
		return completeTrain.GetPrevTrackSpeedFor(this);
	}

	public override Vector3 GetLocalVelocityServer()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		return ((Component)this).transform.forward * GetTrackSpeed();
	}

	public bool AnyPlayersOnTrainCar()
	{
		if (AnyMounted())
		{
			return true;
		}
		if ((Object)(object)platformParentTrigger != (Object)null && platformParentTrigger.HasAnyEntityContents)
		{
			foreach (BaseEntity entityContent in platformParentTrigger.entityContents)
			{
				if ((Object)(object)entityContent.ToPlayer() != (Object)null)
				{
					return true;
				}
			}
		}
		return false;
	}

	public override void VehicleFixedUpdate()
	{
		base.VehicleFixedUpdate();
		if (completeTrain != null)
		{
			completeTrain.UpdateTick(Time.fixedDeltaTime);
			float trackSpeed = GetTrackSpeed();
			((Component)hurtTriggerFront).gameObject.SetActive(!coupling.IsFrontCoupled && trackSpeed > hurtTriggerMinSpeed);
			((Component)hurtTriggerRear).gameObject.SetActive(!coupling.IsRearCoupled && trackSpeed < 0f - hurtTriggerMinSpeed);
			GameObject[] array = hurtOrRepelTriggersInternal;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(Mathf.Abs(trackSpeed) > hurtTriggerMinSpeed);
			}
		}
	}

	public override void PostVehicleFixedUpdate()
	{
		base.PostVehicleFixedUpdate();
		if (completeTrain != null)
		{
			completeTrain.ResetUpdateTick();
		}
	}

	public Vector3 GetCentreOfTrainPos()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		return ((Component)this).transform.position + ((Component)this).transform.rotation * ((Bounds)(ref bounds)).center;
	}

	public Vector3 GetFrontOfTrainPos()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		return ((Component)this).transform.position + ((Component)this).transform.rotation * (((Bounds)(ref bounds)).center + Vector3.forward * ((Bounds)(ref bounds)).extents.z);
	}

	public Vector3 GetRearOfTrainPos()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		return ((Component)this).transform.position + ((Component)this).transform.rotation * (((Bounds)(ref bounds)).center - Vector3.forward * ((Bounds)(ref bounds)).extents.z);
	}

	public void FrontTrainCarTick(TrainTrackSpline.TrackSelection trackSelection, float dt)
	{
		float distToMove = GetTrackSpeed() * dt;
		TrainTrackSpline preferredAltTrack = (((Object)(object)RearTrackSection != (Object)(object)FrontTrackSection) ? RearTrackSection : null);
		MoveFrontWheelsAlongTrackSpline(FrontTrackSection, FrontWheelSplineDist, distToMove, preferredAltTrack, trackSelection);
	}

	public void OtherTrainCarTick(TrainTrackSpline theirTrackSpline, float prevSplineDist, float distanceOffset)
	{
		MoveFrontWheelsAlongTrackSpline(theirTrackSpline, prevSplineDist, distanceOffset, FrontTrackSection, TrainTrackSpline.TrackSelection.Default);
	}

	public bool TryGetNextTrainCar(Vector3 forwardDir, out TrainCar result)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return TryGetTrainCar(next: true, forwardDir, out result);
	}

	public bool TryGetPrevTrainCar(Vector3 forwardDir, out TrainCar result)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return TryGetTrainCar(next: false, forwardDir, out result);
	}

	public bool TryGetTrainCar(bool next, Vector3 forwardDir, out TrainCar result)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		result = null;
		if (completeTrain == null)
		{
			return false;
		}
		return completeTrain.TryGetAdjacentTrainCar(this, next, forwardDir, out result);
	}

	private void MoveFrontWheelsAlongTrackSpline(TrainTrackSpline trackSpline, float prevSplineDist, float distToMove, TrainTrackSpline preferredAltTrack, TrainTrackSpline.TrackSelection trackSelection)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		TrainTrackSpline.MoveResult moveResult = trackSpline.MoveAlongSpline(tReq: new TrainTrackSpline.TrackRequest(trackSelection, preferredAltTrack, null), prevSplineDist: prevSplineDist, askerForward: ((Component)this).transform.forward, distMoved: distToMove);
		TrainTrackSpline frontTS = moveResult.spline;
		FrontWheelSplineDist = moveResult.distAlongSpline;
		frontAtEndOfLine = moveResult.atEndOfLine;
		Vector3 tangent;
		Vector3 positionAndTangent = frontTS.GetPositionAndTangent(FrontWheelSplineDist, ((Component)this).transform.forward, out tangent);
		SetTheRestFromFrontWheelData(ref frontTS, positionAndTangent, tangent, trackSelection, trackSpline, instantMove: false);
		FrontTrackSection = frontTS;
	}

	private Vector3 GetFrontWheelPos()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		return ((Component)this).transform.position + ((Component)this).transform.rotation * frontBogieLocalOffset;
	}

	private Vector3 GetRearWheelPos()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		return ((Component)this).transform.position + ((Component)this).transform.rotation * rearBogieLocalOffset;
	}

	private void SetTheRestFromFrontWheelData(ref TrainTrackSpline frontTS, Vector3 targetFrontWheelPos, Vector3 targetFrontWheelTangent, TrainTrackSpline.TrackSelection trackSelection, TrainTrackSpline additionalAlt, bool instantMove)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		TrainTrackSpline.TrackRequest tReq = new TrainTrackSpline.TrackRequest(trackSelection, RearTrackSection, additionalAlt);
		TrainTrackSpline.MoveResult moveResult = frontTS.MoveAlongSpline(FrontWheelSplineDist, ((Component)this).transform.forward, 0f - distFrontToBackWheel, tReq);
		TrainTrackSpline spline = moveResult.spline;
		float distAlongSpline = moveResult.distAlongSpline;
		rearAtEndOfLine = moveResult.atEndOfLine;
		Vector3 tangent;
		Vector3 positionAndTangent = spline.GetPositionAndTangent(distAlongSpline, ((Component)this).transform.forward, out tangent);
		if (rearAtEndOfLine)
		{
			moveResult = spline.MoveAlongSpline(tReq: new TrainTrackSpline.TrackRequest(trackSelection, spline, additionalAlt), prevSplineDist: distAlongSpline, askerForward: ((Component)this).transform.forward, distMoved: distFrontToBackWheel);
			frontTS = moveResult.spline;
			FrontWheelSplineDist = moveResult.distAlongSpline;
			frontAtEndOfLine = moveResult.atEndOfLine;
			targetFrontWheelPos = frontTS.GetPositionAndTangent(FrontWheelSplineDist, ((Component)this).transform.forward, out targetFrontWheelTangent);
		}
		RearTrackSection = spline;
		Vector3 val = targetFrontWheelPos - positionAndTangent;
		Vector3 normalized = ((Vector3)(ref val)).normalized;
		Vector3 val2 = targetFrontWheelPos - Quaternion.LookRotation(normalized) * frontBogieLocalOffset;
		if (instantMove)
		{
			((Component)this).transform.position = val2;
			if (((Vector3)(ref normalized)).magnitude == 0f)
			{
				((Component)this).transform.rotation = Quaternion.identity;
			}
			else
			{
				((Component)this).transform.rotation = Quaternion.LookRotation(normalized);
			}
		}
		else
		{
			((Component)this).transform.position = val2;
			if (((Vector3)(ref normalized)).magnitude == 0f)
			{
				((Component)this).transform.rotation = Quaternion.identity;
			}
			else
			{
				((Component)this).transform.rotation = Quaternion.LookRotation(normalized);
			}
		}
		frontBogieYRot = Vector3.SignedAngle(((Component)this).transform.forward, targetFrontWheelTangent, ((Component)this).transform.up);
		rearBogieYRot = Vector3.SignedAngle(((Component)this).transform.forward, tangent, ((Component)this).transform.up);
		if (Application.isEditor)
		{
			Debug.DrawLine(targetFrontWheelPos, positionAndTangent, Color.magenta, 0.2f);
			Debug.DrawLine(rigidBody.position, val2, Color.yellow, 0.2f);
			Debug.DrawRay(val2, Vector3.up, Color.yellow, 0.2f);
		}
	}

	public float GetForces()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		float num = ((Component)this).transform.localEulerAngles.x;
		if (num > 180f)
		{
			num -= 360f;
		}
		return 0f + num / 90f * (0f - Physics.gravity.y) * RealisticMass * 0.5f + GetThrottleForce();
	}

	protected virtual float GetThrottleForce()
	{
		return 0f;
	}

	public virtual bool HasThrottleInput()
	{
		return false;
	}

	public float ApplyCollisionDamage(float forceMagnitude)
	{
		float num = ((!(forceMagnitude > derailCollisionForce)) ? (Mathf.Pow(forceMagnitude, 1.3f) / collisionDamageDivide) : float.MaxValue);
		Hurt(num, DamageType.Collision, this, useProtection: false);
		return num;
	}

	public bool SpaceIsClear()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		List<Collider> list = Pool.GetList<Collider>();
		GamePhysics.OverlapOBB(WorldSpaceBounds(), list, 32768, (QueryTriggerInteraction)1);
		foreach (Collider item in list)
		{
			if (!ColliderIsPartOfTrain(item))
			{
				return false;
			}
		}
		Pool.FreeList<Collider>(ref list);
		return true;
	}

	public bool ColliderIsPartOfTrain(Collider collider)
	{
		BaseEntity baseEntity = collider.ToBaseEntity();
		if ((Object)(object)baseEntity == (Object)null)
		{
			return false;
		}
		if ((Object)(object)baseEntity == (Object)(object)this)
		{
			return true;
		}
		BaseEntity baseEntity2 = baseEntity.parentEntity.Get(base.isServer);
		if (baseEntity2.IsValid())
		{
			return (Object)(object)baseEntity2 == (Object)(object)this;
		}
		return false;
	}

	private void UpdateClients()
	{
		if (IsMoving())
		{
			ClientRPC(null, "BaseTrainUpdate", GetNetworkTime(), frontBogieYRot, rearBogieYRot);
		}
	}

	private void DecayTick()
	{
		if (completeTrain == null)
		{
			return;
		}
		bool flag = HasDriver() || completeTrain.AnyPlayersOnTrain();
		if (flag)
		{
			decayingFor = 0f;
		}
		float num = GetDecayMinutes(flag) * 60f;
		float time = Time.time;
		float num2 = time - lastDecayTick;
		lastDecayTick = time;
		if (num != float.PositiveInfinity)
		{
			decayingFor += num2;
			if (decayingFor >= num && CanDieFromDecayNow())
			{
				ActualDeath();
			}
		}
	}

	protected virtual float GetDecayMinutes(bool hasPassengers)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		bool flag = IsAtAStation && Vector3.Distance(spawnOrigin, ((Component)this).transform.position) < 50f;
		if (hasPassengers || AnyPlayersNearby(30f) || flag || IsOnAboveGroundSpawnRail)
		{
			return float.PositiveInfinity;
		}
		return decayminutes * decayTimeMultiplier;
	}

	protected virtual bool CanDieFromDecayNow()
	{
		if (CarType != TrainCarType.Engine)
		{
			return !completeTrain.IncludesAnEngine();
		}
		return true;
	}

	private bool AnyPlayersNearby(float maxDist)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return BaseNetworkable.HasCloseConnections(((Component)this).transform.position, maxDist);
	}

	[RPC_Server]
	public void RPC_WantsUncouple(RPCMessage msg)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		BasePlayer player = msg.player;
		if (!((Object)(object)player == (Object)null) && !(Vector3.SqrMagnitude(((Component)this).transform.position - ((Component)player).transform.position) > 200f))
		{
			bool front = msg.read.Bit();
			coupling.Uncouple(front);
		}
	}
}
