using System;
using System.Collections.Generic;
using ConVar;
using Rust.AI;
using UnityEngine;

public class AIBrainSenses
{
	[ServerVar]
	public static float UpdateInterval = 0.5f;

	[ServerVar]
	public static float HumanKnownPlayersLOSUpdateInterval = 0.2f;

	[ServerVar]
	public static float KnownPlayersLOSUpdateInterval = 0.5f;

	private float knownPlayersLOSUpdateInterval = 0.2f;

	public float MemoryDuration = 10f;

	public float LastThreatTimestamp;

	public float TimeInAgressiveState;

	private static BaseEntity[] queryResults = new BaseEntity[64];

	private static BasePlayer[] playerQueryResults = new BasePlayer[64];

	private float nextUpdateTime;

	private float nextKnownPlayersLOSUpdateTime;

	private BaseEntity owner;

	private BasePlayer playerOwner;

	private IAISenses ownerSenses;

	private float maxRange;

	private float targetLostRange;

	private float visionCone;

	private bool checkVision;

	private bool checkLOS;

	private bool ignoreNonVisionSneakers;

	private float listenRange;

	private bool hostileTargetsOnly;

	private bool senseFriendlies;

	private bool refreshKnownLOS;

	private EntityType senseTypes;

	private IAIAttack ownerAttack;

	public BaseAIBrain brain;

	private Func<BaseEntity, bool> aiCaresAbout;

	public float TimeSinceThreat => Time.realtimeSinceStartup - LastThreatTimestamp;

	public SimpleAIMemory Memory { get; private set; } = new SimpleAIMemory();


	public float TargetLostRange => targetLostRange;

	public bool ignoreSafeZonePlayers { get; private set; }

	public List<BaseEntity> Players => Memory.Players;

	public void Init(BaseEntity owner, BaseAIBrain brain, float memoryDuration, float range, float targetLostRange, float visionCone, bool checkVision, bool checkLOS, bool ignoreNonVisionSneakers, float listenRange, bool hostileTargetsOnly, bool senseFriendlies, bool ignoreSafeZonePlayers, EntityType senseTypes, bool refreshKnownLOS)
	{
		aiCaresAbout = AiCaresAbout;
		this.owner = owner;
		this.brain = brain;
		MemoryDuration = memoryDuration;
		ownerAttack = owner as IAIAttack;
		playerOwner = owner as BasePlayer;
		maxRange = range;
		this.targetLostRange = targetLostRange;
		this.visionCone = visionCone;
		this.checkVision = checkVision;
		this.checkLOS = checkLOS;
		this.ignoreNonVisionSneakers = ignoreNonVisionSneakers;
		this.listenRange = listenRange;
		this.hostileTargetsOnly = hostileTargetsOnly;
		this.senseFriendlies = senseFriendlies;
		this.ignoreSafeZonePlayers = ignoreSafeZonePlayers;
		this.senseTypes = senseTypes;
		LastThreatTimestamp = Time.realtimeSinceStartup;
		this.refreshKnownLOS = refreshKnownLOS;
		ownerSenses = owner as IAISenses;
		knownPlayersLOSUpdateInterval = ((owner is HumanNPC) ? HumanKnownPlayersLOSUpdateInterval : KnownPlayersLOSUpdateInterval);
	}

	public void DelaySenseUpdate(float delay)
	{
		nextUpdateTime = Time.time + delay;
	}

	public void Update()
	{
		if (!((Object)(object)owner == (Object)null))
		{
			UpdateSenses();
			UpdateKnownPlayersLOS();
		}
	}

	private void UpdateSenses()
	{
		if (Time.time < nextUpdateTime)
		{
			return;
		}
		nextUpdateTime = Time.time + UpdateInterval;
		if (senseTypes != 0)
		{
			if (senseTypes == EntityType.Player)
			{
				SensePlayers();
			}
			else
			{
				SenseBrains();
				if (senseTypes.HasFlag(EntityType.Player))
				{
					SensePlayers();
				}
			}
		}
		Memory.Forget(MemoryDuration);
	}

	public void UpdateKnownPlayersLOS()
	{
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		if (Time.time < nextKnownPlayersLOSUpdateTime)
		{
			return;
		}
		nextKnownPlayersLOSUpdateTime = Time.time + knownPlayersLOSUpdateInterval;
		foreach (BaseEntity player in Memory.Players)
		{
			if (!((Object)(object)player == (Object)null) && !player.IsNpc)
			{
				bool flag = ownerAttack.CanSeeTarget(player);
				Memory.SetLOS(player, flag);
				if (refreshKnownLOS && (Object)(object)owner != (Object)null && flag && Vector3.Distance(((Component)player).transform.position, ((Component)owner).transform.position) <= TargetLostRange)
				{
					Memory.SetKnown(player, owner, this);
				}
			}
		}
	}

	private void SensePlayers()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		int playersInSphere = BaseEntity.Query.Server.GetPlayersInSphere(((Component)owner).transform.position, maxRange, playerQueryResults, aiCaresAbout);
		for (int i = 0; i < playersInSphere; i++)
		{
			BasePlayer ent = playerQueryResults[i];
			Memory.SetKnown(ent, owner, this);
		}
	}

	private void SenseBrains()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		int brainsInSphere = BaseEntity.Query.Server.GetBrainsInSphere(((Component)owner).transform.position, maxRange, queryResults, aiCaresAbout);
		for (int i = 0; i < brainsInSphere; i++)
		{
			BaseEntity ent = queryResults[i];
			Memory.SetKnown(ent, owner, this);
		}
	}

	private bool AiCaresAbout(BaseEntity entity)
	{
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)entity == (Object)null)
		{
			return false;
		}
		if (!entity.isServer)
		{
			return false;
		}
		if (entity.EqualNetID((BaseNetworkable)owner))
		{
			return false;
		}
		if (entity.Health() <= 0f)
		{
			return false;
		}
		if (entity.IsTransferProtected())
		{
			return false;
		}
		if (!IsValidSenseType(entity))
		{
			return false;
		}
		BaseCombatEntity baseCombatEntity = entity as BaseCombatEntity;
		BasePlayer basePlayer = entity as BasePlayer;
		if ((Object)(object)basePlayer != (Object)null && basePlayer.IsDead())
		{
			return false;
		}
		if (ignoreSafeZonePlayers && (Object)(object)basePlayer != (Object)null && basePlayer.InSafeZone())
		{
			return false;
		}
		if (listenRange > 0f && (Object)(object)baseCombatEntity != (Object)null && baseCombatEntity.TimeSinceLastNoise <= 1f && baseCombatEntity.CanLastNoiseBeHeard(((Component)owner).transform.position, listenRange))
		{
			return true;
		}
		if (senseFriendlies && ownerSenses != null && ownerSenses.IsFriendly(entity))
		{
			return true;
		}
		float num = float.PositiveInfinity;
		if ((Object)(object)baseCombatEntity != (Object)null && AI.accuratevisiondistance)
		{
			num = Vector3.Distance(((Component)owner).transform.position, ((Component)baseCombatEntity).transform.position);
			if (num > maxRange)
			{
				return false;
			}
		}
		if (checkVision && !IsTargetInVision(entity))
		{
			if (!ignoreNonVisionSneakers)
			{
				return false;
			}
			if ((Object)(object)basePlayer != (Object)null && !basePlayer.IsNpc)
			{
				if (!AI.accuratevisiondistance)
				{
					num = Vector3.Distance(((Component)owner).transform.position, ((Component)basePlayer).transform.position);
				}
				if ((basePlayer.IsDucked() && num >= brain.IgnoreSneakersMaxDistance) || num >= brain.IgnoreNonVisionMaxDistance)
				{
					return false;
				}
			}
		}
		if (hostileTargetsOnly && (Object)(object)baseCombatEntity != (Object)null && !baseCombatEntity.IsHostile() && !(baseCombatEntity is ScarecrowNPC))
		{
			return false;
		}
		if (checkLOS && ownerAttack != null)
		{
			bool flag = ownerAttack.CanSeeTarget(entity);
			Memory.SetLOS(entity, flag);
			if (!flag)
			{
				return false;
			}
		}
		return true;
	}

	private bool IsValidSenseType(BaseEntity ent)
	{
		BasePlayer basePlayer = ent as BasePlayer;
		if ((Object)(object)basePlayer != (Object)null)
		{
			if (basePlayer.IsNpc)
			{
				if (ent is BasePet)
				{
					return true;
				}
				if (ent is ScarecrowNPC)
				{
					return true;
				}
				if (senseTypes.HasFlag(EntityType.BasePlayerNPC))
				{
					return true;
				}
			}
			else if (senseTypes.HasFlag(EntityType.Player))
			{
				return true;
			}
		}
		if (senseTypes.HasFlag(EntityType.NPC) && ent is BaseNpc)
		{
			return true;
		}
		if (senseTypes.HasFlag(EntityType.WorldItem) && ent is WorldItem)
		{
			return true;
		}
		if (senseTypes.HasFlag(EntityType.Corpse) && ent is BaseCorpse)
		{
			return true;
		}
		if (senseTypes.HasFlag(EntityType.TimedExplosive) && ent is TimedExplosive)
		{
			return true;
		}
		if (senseTypes.HasFlag(EntityType.Chair) && ent is BaseChair)
		{
			return true;
		}
		return false;
	}

	private bool IsTargetInVision(BaseEntity target)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = Vector3Ex.Direction(((Component)target).transform.position, ((Component)owner).transform.position);
		return Vector3.Dot(((Object)(object)playerOwner != (Object)null) ? playerOwner.eyes.BodyForward() : ((Component)owner).transform.forward, val) >= visionCone;
	}

	public BaseEntity GetNearestPlayer(float rangeFraction)
	{
		return GetNearest(Memory.Players, rangeFraction);
	}

	public BaseEntity GetNearestThreat(float rangeFraction)
	{
		return GetNearest(Memory.Threats, rangeFraction);
	}

	public BaseEntity GetNearestTarget(float rangeFraction)
	{
		return GetNearest(Memory.Targets, rangeFraction);
	}

	private BaseEntity GetNearest(List<BaseEntity> entities, float rangeFraction)
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		if (entities == null || entities.Count == 0)
		{
			return null;
		}
		float num = float.PositiveInfinity;
		BaseEntity result = null;
		foreach (BaseEntity entity in entities)
		{
			if (!((Object)(object)entity == (Object)null) && !(entity.Health() <= 0f))
			{
				float num2 = Vector3.Distance(((Component)entity).transform.position, ((Component)owner).transform.position);
				if (num2 <= rangeFraction * maxRange && num2 < num)
				{
					result = entity;
				}
			}
		}
		return result;
	}
}
