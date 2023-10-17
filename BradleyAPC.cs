using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using Facepunch.Extend;
using Network;
using ProtoBuf;
using Rust;
using Rust.AI;
using UnityEngine;
using UnityEngine.AI;

public class BradleyAPC : BaseCombatEntity, TriggerHurtNotChild.IHurtTriggerUser
{
	[Serializable]
	public class TargetInfo : IPooled
	{
		public float damageReceivedFrom;

		public BaseEntity entity;

		public float lastSeenTime;

		public Vector3 lastSeenPosition;

		public void EnterPool()
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			entity = null;
			lastSeenPosition = Vector3.zero;
			lastSeenTime = 0f;
		}

		public void Setup(BaseEntity ent, float time)
		{
			entity = ent;
			lastSeenTime = time;
		}

		public void LeavePool()
		{
		}

		public float GetPriorityScore(BradleyAPC apc)
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			BasePlayer basePlayer = entity as BasePlayer;
			if (Object.op_Implicit((Object)(object)basePlayer))
			{
				float num = Vector3.Distance(((Component)entity).transform.position, ((Component)apc).transform.position);
				float num2 = (1f - Mathf.InverseLerp(10f, 80f, num)) * 50f;
				float num3 = (((Object)(object)basePlayer.GetHeldEntity() == (Object)null) ? 0f : basePlayer.GetHeldEntity().hostileScore);
				float num4 = Mathf.InverseLerp(4f, 20f, num3) * 100f;
				float num5 = Mathf.InverseLerp(10f, 3f, Time.time - lastSeenTime) * 100f;
				float num6 = Mathf.InverseLerp(0f, 100f, damageReceivedFrom) * 50f;
				return num2 + num4 + num6 + num5;
			}
			return 0f;
		}

		public bool IsVisible()
		{
			if (lastSeenTime != -1f)
			{
				return Time.time - lastSeenTime < sightUpdateRate * 2f;
			}
			return false;
		}

		public bool IsValid()
		{
			return (Object)(object)entity != (Object)null;
		}
	}

	[Header("Sound")]
	public BlendedLoopEngineSound engineSound;

	public SoundDefinition treadLoopDef;

	public AnimationCurve treadGainCurve;

	public AnimationCurve treadPitchCurve;

	public AnimationCurve treadFreqCurve;

	private Sound treadLoop;

	private SoundModulation.Modulator treadGain;

	private SoundModulation.Modulator treadPitch;

	public SoundDefinition chasisLurchSoundDef;

	public float chasisLurchAngleDelta = 2f;

	public float chasisLurchSpeedDelta = 2f;

	private float lastAngle;

	private float lastSpeed;

	public SoundDefinition turretTurnLoopDef;

	public float turretLoopGainSpeed = 3f;

	public float turretLoopPitchSpeed = 3f;

	public float turretLoopMinAngleDelta;

	public float turretLoopMaxAngleDelta = 10f;

	public float turretLoopPitchMin = 0.5f;

	public float turretLoopPitchMax = 1f;

	public float turretLoopGainThreshold = 0.0001f;

	private Sound turretTurnLoop;

	private SoundModulation.Modulator turretTurnLoopGain;

	private SoundModulation.Modulator turretTurnLoopPitch;

	public float enginePitch = 0.9f;

	public float rpmMultiplier = 0.6f;

	private TreadAnimator treadAnimator;

	[Header("Pathing")]
	public List<Vector3> currentPath;

	public int currentPathIndex;

	public bool pathLooping;

	[Header("Targeting")]
	public float viewDistance = 100f;

	public float searchRange = 100f;

	public float searchFrequency = 2f;

	public float memoryDuration = 20f;

	public static float sightUpdateRate = 0.5f;

	public List<TargetInfo> targetList = new List<TargetInfo>();

	private BaseCombatEntity mainGunTarget;

	[Header("Coax")]
	public float coaxFireRate = 0.06667f;

	public int coaxBurstLength = 10;

	public float coaxAimCone = 3f;

	public float bulletDamage = 15f;

	[Header("TopTurret")]
	public float topTurretFireRate = 0.25f;

	private float nextCoaxTime;

	private int numCoaxBursted;

	private float nextTopTurretTime = 0.3f;

	public GameObjectRef gun_fire_effect;

	public GameObjectRef bulletEffect;

	private float lastLateUpdate;

	[Header("Wheels")]
	public WheelCollider[] leftWheels;

	public WheelCollider[] rightWheels;

	[Header("Movement Config")]
	public float moveForceMax = 2000f;

	public float brakeForce = 100f;

	public float turnForce = 2000f;

	public float sideStiffnessMax = 1f;

	public float sideStiffnessMin = 0.5f;

	public Transform centerOfMass;

	public float stoppingDist = 5f;

	[Header("Control")]
	public float throttle = 1f;

	public float turning;

	public float rightThrottle;

	public float leftThrottle;

	public bool brake;

	[Header("Other")]
	public Rigidbody myRigidBody;

	public Collider myCollider;

	public Vector3 destination;

	private Vector3 finalDestination;

	public Transform followTest;

	public TriggerHurtEx impactDamager;

	[Header("Weapons")]
	public Transform mainTurretEyePos;

	public Transform mainTurret;

	public Transform CannonPitch;

	public Transform CannonMuzzle;

	public Transform coaxPitch;

	public Transform coaxMuzzle;

	public Transform topTurretEyePos;

	public Transform topTurretYaw;

	public Transform topTurretPitch;

	public Transform topTurretMuzzle;

	private Vector3 turretAimVector = Vector3.forward;

	private Vector3 desiredAimVector = Vector3.forward;

	private Vector3 topTurretAimVector = Vector3.forward;

	private Vector3 desiredTopTurretAimVector = Vector3.forward;

	[Header("Effects")]
	public GameObjectRef explosionEffect;

	public GameObjectRef servergibs;

	public GameObjectRef fireBall;

	public GameObjectRef crateToDrop;

	public GameObjectRef debrisFieldMarker;

	[Header("Loot")]
	public int maxCratesToSpawn;

	public int patrolPathIndex;

	public IAIPath patrolPath;

	public bool DoAI = true;

	public GameObjectRef mainCannonMuzzleFlash;

	public GameObjectRef mainCannonProjectile;

	public float recoilScale = 200f;

	public NavMeshPath navMeshPath;

	public int navMeshPathIndex;

	private LayerMask obstacleHitMask;

	private TimeSince timeSinceSeemingStuck;

	private TimeSince timeSinceStuckReverseStart;

	private const string prefabPath = "assets/prefabs/npc/m2bradley/bradleyapc.prefab";

	private float nextFireTime = 10f;

	private int numBursted;

	private float nextPatrolTime;

	private float nextEngagementPathTime;

	private float currentSpeedZoneLimit;

	protected override float PositionTickRate => 0.1f;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("BradleyAPC.OnRpcMessage", 0);
		try
		{
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	public bool HasPath()
	{
		if (currentPath != null)
		{
			return currentPath.Count > 0;
		}
		return false;
	}

	public void ClearPath()
	{
		currentPath.Clear();
		currentPathIndex = -1;
	}

	public bool IndexValid(int index)
	{
		if (!HasPath())
		{
			return false;
		}
		if (index >= 0)
		{
			return index < currentPath.Count;
		}
		return false;
	}

	public Vector3 GetFinalDestination()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		if (!HasPath())
		{
			return ((Component)this).transform.position;
		}
		return finalDestination;
	}

	public Vector3 GetCurrentPathDestination()
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		if (!HasPath())
		{
			return ((Component)this).transform.position;
		}
		return currentPath[currentPathIndex];
	}

	public bool PathComplete()
	{
		if (HasPath())
		{
			if (currentPathIndex == currentPath.Count - 1)
			{
				return AtCurrentPathNode();
			}
			return false;
		}
		return true;
	}

	public bool AtCurrentPathNode()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		if (currentPathIndex < 0 || currentPathIndex >= currentPath.Count)
		{
			return false;
		}
		return Vector3.Distance(((Component)this).transform.position, currentPath[currentPathIndex]) <= stoppingDist;
	}

	public int GetLoopedIndex(int index)
	{
		if (!HasPath())
		{
			Debug.LogWarning((object)"Warning, GetLoopedIndex called without a path");
			return 0;
		}
		if (!pathLooping)
		{
			return Mathf.Clamp(index, 0, currentPath.Count - 1);
		}
		if (index >= currentPath.Count)
		{
			return index % currentPath.Count;
		}
		if (index < 0)
		{
			return currentPath.Count - Mathf.Abs(index % currentPath.Count);
		}
		return index;
	}

	public Vector3 PathDirection(int index)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		if (!HasPath() || currentPath.Count <= 1)
		{
			return ((Component)this).transform.forward;
		}
		index = GetLoopedIndex(index);
		Vector3 val;
		Vector3 val2;
		if (pathLooping)
		{
			int loopedIndex = GetLoopedIndex(index - 1);
			val = currentPath[loopedIndex];
			val2 = currentPath[GetLoopedIndex(index)];
		}
		else
		{
			val = ((index - 1 >= 0) ? currentPath[index - 1] : ((Component)this).transform.position);
			val2 = currentPath[index];
		}
		Vector3 val3 = val2 - val;
		return ((Vector3)(ref val3)).normalized;
	}

	public Vector3 IdealPathPosition()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		if (!HasPath())
		{
			return ((Component)this).transform.position;
		}
		int loopedIndex = GetLoopedIndex(currentPathIndex - 1);
		if (loopedIndex == currentPathIndex)
		{
			return currentPath[currentPathIndex];
		}
		return ClosestPointAlongPath(currentPath[loopedIndex], currentPath[currentPathIndex], ((Component)this).transform.position);
	}

	public void AdvancePathMovement(bool force)
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		if (HasPath())
		{
			if (force || AtCurrentPathNode() || currentPathIndex == -1)
			{
				currentPathIndex = GetLoopedIndex(currentPathIndex + 1);
			}
			if (PathComplete())
			{
				ClearPath();
				return;
			}
			Vector3 val = IdealPathPosition();
			Vector3 val2 = currentPath[currentPathIndex];
			float num = Vector3.Distance(val, val2);
			float num2 = Vector3.Distance(((Component)this).transform.position, val);
			float num3 = Mathf.InverseLerp(8f, 0f, num2);
			val += Direction2D(val2, val) * Mathf.Min(num, num3 * 20f);
			SetDestination(val);
		}
	}

	public bool GetPathToClosestTurnableNode(IAIPathNode start, Vector3 forward, ref List<IAIPathNode> nodes)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		float num = float.NegativeInfinity;
		IAIPathNode iAIPathNode = null;
		Vector3 val;
		foreach (IAIPathNode item in start.Linked)
		{
			val = item.Position - start.Position;
			float num2 = Vector3.Dot(forward, ((Vector3)(ref val)).normalized);
			if (num2 > num)
			{
				num = num2;
				iAIPathNode = item;
			}
		}
		if (iAIPathNode != null)
		{
			nodes.Add(iAIPathNode);
			if (!iAIPathNode.Straightaway)
			{
				return true;
			}
			IAIPathNode start2 = iAIPathNode;
			val = iAIPathNode.Position - start.Position;
			return GetPathToClosestTurnableNode(start2, ((Vector3)(ref val)).normalized, ref nodes);
		}
		return false;
	}

	public bool GetEngagementPath(ref List<IAIPathNode> nodes)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		IAIPathNode closestToPoint = patrolPath.GetClosestToPoint(((Component)this).transform.position);
		Vector3 val = closestToPoint.Position - ((Component)this).transform.position;
		Vector3 normalized = ((Vector3)(ref val)).normalized;
		if (Vector3.Dot(((Component)this).transform.forward, normalized) > 0f)
		{
			nodes.Add(closestToPoint);
			if (!closestToPoint.Straightaway)
			{
				return true;
			}
		}
		return GetPathToClosestTurnableNode(closestToPoint, ((Component)this).transform.forward, ref nodes);
	}

	public void AddOrUpdateTarget(BaseEntity ent, Vector3 pos, float damageFrom = 0f)
	{
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		if ((AI.ignoreplayers && !ent.IsNpc) || !(ent is BasePlayer))
		{
			return;
		}
		TargetInfo targetInfo = null;
		foreach (TargetInfo target in targetList)
		{
			if ((Object)(object)target.entity == (Object)(object)ent)
			{
				targetInfo = target;
				break;
			}
		}
		if (targetInfo == null)
		{
			targetInfo = Pool.Get<TargetInfo>();
			targetInfo.Setup(ent, Time.time - 1f);
			targetList.Add(targetInfo);
		}
		targetInfo.lastSeenPosition = pos;
		targetInfo.damageReceivedFrom += damageFrom;
	}

	public void UpdateTargetList()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		List<BaseEntity> list = Pool.GetList<BaseEntity>();
		Vis.Entities(((Component)this).transform.position, searchRange, list, 133120, (QueryTriggerInteraction)2);
		foreach (BaseEntity item in list)
		{
			if ((AI.ignoreplayers && !item.IsNpc) || !(item is BasePlayer))
			{
				continue;
			}
			BasePlayer basePlayer = item as BasePlayer;
			if (basePlayer.IsDead() || basePlayer is HumanNPC || basePlayer is NPCPlayer || (basePlayer.InSafeZone() && !basePlayer.IsHostile()) || !VisibilityTest(item))
			{
				continue;
			}
			bool flag = false;
			foreach (TargetInfo target in targetList)
			{
				if ((Object)(object)target.entity == (Object)(object)item)
				{
					target.lastSeenTime = Time.time;
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				TargetInfo targetInfo = Pool.Get<TargetInfo>();
				targetInfo.Setup(item, Time.time);
				targetList.Add(targetInfo);
			}
		}
		for (int num = targetList.Count - 1; num >= 0; num--)
		{
			TargetInfo targetInfo2 = targetList[num];
			BasePlayer basePlayer2 = targetInfo2.entity as BasePlayer;
			if ((Object)(object)targetInfo2.entity == (Object)null || Time.time - targetInfo2.lastSeenTime > memoryDuration || basePlayer2.IsDead() || (basePlayer2.InSafeZone() && !basePlayer2.IsHostile()) || (AI.ignoreplayers && !basePlayer2.IsNpc))
			{
				targetList.Remove(targetInfo2);
				Pool.Free<TargetInfo>(ref targetInfo2);
			}
		}
		Pool.FreeList<BaseEntity>(ref list);
		targetList.Sort(SortTargets);
	}

	public int SortTargets(TargetInfo t1, TargetInfo t2)
	{
		return t2.GetPriorityScore(this).CompareTo(t1.GetPriorityScore(this));
	}

	public Vector3 GetAimPoint(BaseEntity ent)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		BasePlayer basePlayer = ent as BasePlayer;
		if ((Object)(object)basePlayer != (Object)null)
		{
			return basePlayer.eyes.position;
		}
		return ent.CenterPoint();
	}

	public bool VisibilityTest(BaseEntity ent)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)ent == (Object)null)
		{
			return false;
		}
		if (!(Vector3.Distance(((Component)ent).transform.position, ((Component)this).transform.position) < viewDistance))
		{
			return false;
		}
		bool flag = false;
		if (ent is BasePlayer)
		{
			BasePlayer basePlayer = ent as BasePlayer;
			Vector3 position = ((Component)mainTurret).transform.position;
			flag = IsVisible(basePlayer.eyes.position, position) || IsVisible(((Component)basePlayer).transform.position + Vector3.up * 0.1f, position);
			if (!flag && basePlayer.isMounted && (Object)(object)basePlayer.GetMounted().VehicleParent() != (Object)null && basePlayer.GetMounted().VehicleParent().AlwaysAllowBradleyTargeting)
			{
				flag = IsVisible(((Bounds)(ref basePlayer.GetMounted().VehicleParent().bounds)).center, position);
			}
			if (flag)
			{
				flag = !Physics.SphereCast(new Ray(position, Vector3Ex.Direction(basePlayer.eyes.position, position)), 0.05f, Vector3.Distance(basePlayer.eyes.position, position), 10551297);
			}
		}
		else
		{
			Debug.LogWarning((object)"Standard vis test!");
			flag = IsVisible(ent.CenterPoint());
		}
		return flag;
	}

	public void UpdateTargetVisibilities()
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		foreach (TargetInfo target in targetList)
		{
			if (target.IsValid() && VisibilityTest(target.entity))
			{
				target.lastSeenTime = Time.time;
				target.lastSeenPosition = ((Component)target.entity).transform.position;
			}
		}
	}

	public void DoWeaponAiming()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		Vector3 normalized;
		Vector3 val;
		if (!((Object)(object)mainGunTarget != (Object)null))
		{
			normalized = desiredAimVector;
		}
		else
		{
			val = GetAimPoint(mainGunTarget) - ((Component)mainTurretEyePos).transform.position;
			normalized = ((Vector3)(ref val)).normalized;
		}
		desiredAimVector = normalized;
		BaseEntity baseEntity = null;
		if (targetList.Count > 0)
		{
			if (targetList.Count > 1 && targetList[1].IsValid() && targetList[1].IsVisible())
			{
				baseEntity = targetList[1].entity;
			}
			else if (targetList[0].IsValid() && targetList[0].IsVisible())
			{
				baseEntity = targetList[0].entity;
			}
		}
		Vector3 val2;
		if (!((Object)(object)baseEntity != (Object)null))
		{
			val2 = ((Component)this).transform.forward;
		}
		else
		{
			val = GetAimPoint(baseEntity) - ((Component)topTurretEyePos).transform.position;
			val2 = ((Vector3)(ref val)).normalized;
		}
		desiredTopTurretAimVector = val2;
	}

	public void DoWeapons()
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)mainGunTarget != (Object)null)
		{
			Vector3 val = turretAimVector;
			Vector3 val2 = GetAimPoint(mainGunTarget) - ((Component)mainTurretEyePos).transform.position;
			if (Vector3.Dot(val, ((Vector3)(ref val2)).normalized) >= 0.99f)
			{
				bool flag = VisibilityTest(mainGunTarget);
				float num = Vector3.Distance(((Component)mainGunTarget).transform.position, ((Component)this).transform.position);
				if (Time.time > nextCoaxTime && flag && num <= 40f)
				{
					numCoaxBursted++;
					FireGun(GetAimPoint(mainGunTarget), 3f, isCoax: true);
					nextCoaxTime = Time.time + coaxFireRate;
					if (numCoaxBursted >= coaxBurstLength)
					{
						nextCoaxTime = Time.time + 1f;
						numCoaxBursted = 0;
					}
				}
				if (num >= 10f && flag)
				{
					FireGunTest();
				}
			}
		}
		if (targetList.Count > 1)
		{
			BaseEntity entity = targetList[1].entity;
			if ((Object)(object)entity != (Object)null && Time.time > nextTopTurretTime && VisibilityTest(entity))
			{
				FireGun(GetAimPoint(targetList[1].entity), 3f, isCoax: false);
				nextTopTurretTime = Time.time + topTurretFireRate;
			}
		}
	}

	public void FireGun(Vector3 targetPos, float aimCone, bool isCoax)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		Transform val = (isCoax ? coaxMuzzle : topTurretMuzzle);
		Vector3 val2 = ((Component)val).transform.position - val.forward * 0.25f;
		Vector3 val3 = targetPos - val2;
		Vector3 normalized = ((Vector3)(ref val3)).normalized;
		Vector3 modifiedAimConeDirection = AimConeUtil.GetModifiedAimConeDirection(aimCone, normalized);
		targetPos = val2 + modifiedAimConeDirection * 300f;
		List<RaycastHit> list = Pool.GetList<RaycastHit>();
		GamePhysics.TraceAll(new Ray(val2, modifiedAimConeDirection), 0f, list, 300f, 1220225809, (QueryTriggerInteraction)0);
		for (int i = 0; i < list.Count; i++)
		{
			RaycastHit hit = list[i];
			BaseEntity entity = hit.GetEntity();
			if (!((Object)(object)entity != (Object)null) || (!((Object)(object)entity == (Object)(object)this) && !entity.EqualNetID((BaseNetworkable)this)))
			{
				BaseCombatEntity baseCombatEntity = entity as BaseCombatEntity;
				if ((Object)(object)baseCombatEntity != (Object)null)
				{
					ApplyDamage(baseCombatEntity, ((RaycastHit)(ref hit)).point, modifiedAimConeDirection);
				}
				if (!((Object)(object)entity != (Object)null) || entity.ShouldBlockProjectiles())
				{
					targetPos = ((RaycastHit)(ref hit)).point;
					break;
				}
			}
		}
		ClientRPC<bool, Vector3>(null, "CLIENT_FireGun", isCoax, targetPos);
		Pool.FreeList<RaycastHit>(ref list);
	}

	private void ApplyDamage(BaseCombatEntity entity, Vector3 point, Vector3 normal)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		float damageAmount = bulletDamage * Random.Range(0.9f, 1.1f);
		HitInfo info = new HitInfo(this, entity, DamageType.Bullet, damageAmount, point);
		entity.OnAttacked(info);
		if (entity is BasePlayer || entity is BaseNpc)
		{
			Effect.server.ImpactEffect(new HitInfo
			{
				HitPositionWorld = point,
				HitNormalWorld = -normal,
				HitMaterial = StringPool.Get("Flesh")
			});
		}
	}

	public void AimWeaponAt(Transform weaponYaw, Transform weaponPitch, Vector3 direction, float minPitch = -360f, float maxPitch = 360f, float maxYaw = 360f, Transform parentOverride = null)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = direction;
		val = weaponYaw.parent.InverseTransformDirection(val);
		Quaternion localRotation = Quaternion.LookRotation(val);
		Vector3 eulerAngles = ((Quaternion)(ref localRotation)).eulerAngles;
		for (int i = 0; i < 3; i++)
		{
			((Vector3)(ref eulerAngles))[i] = ((Vector3)(ref eulerAngles))[i] - ((((Vector3)(ref eulerAngles))[i] > 180f) ? 360f : 0f);
		}
		Quaternion localRotation2 = Quaternion.Euler(0f, Mathf.Clamp(eulerAngles.y, 0f - maxYaw, maxYaw), 0f);
		Quaternion localRotation3 = Quaternion.Euler(Mathf.Clamp(eulerAngles.x, minPitch, maxPitch), 0f, 0f);
		if ((Object)(object)weaponYaw == (Object)null && (Object)(object)weaponPitch != (Object)null)
		{
			((Component)weaponPitch).transform.localRotation = localRotation3;
			return;
		}
		if ((Object)(object)weaponPitch == (Object)null && (Object)(object)weaponYaw != (Object)null)
		{
			((Component)weaponYaw).transform.localRotation = localRotation;
			return;
		}
		((Component)weaponYaw).transform.localRotation = localRotation2;
		((Component)weaponPitch).transform.localRotation = localRotation3;
	}

	public void LateUpdate()
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		float num = Time.time - lastLateUpdate;
		lastLateUpdate = Time.time;
		if (base.isServer)
		{
			float num2 = (float)Math.PI * 2f / 3f;
			turretAimVector = Vector3.RotateTowards(turretAimVector, desiredAimVector, num2 * num, 0f);
		}
		else
		{
			turretAimVector = Vector3.Lerp(turretAimVector, desiredAimVector, Time.deltaTime * 10f);
		}
		AimWeaponAt(mainTurret, coaxPitch, turretAimVector, -90f, 90f);
		AimWeaponAt(mainTurret, CannonPitch, turretAimVector, -90f, 7f);
		topTurretAimVector = Vector3.Lerp(topTurretAimVector, desiredTopTurretAimVector, Time.deltaTime * 5f);
		AimWeaponAt(topTurretYaw, topTurretPitch, topTurretAimVector, -360f, 360f, 360f, mainTurret);
	}

	public override void Load(LoadInfo info)
	{
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		base.Load(info);
		if (info.msg.bradley != null && !info.fromDisk)
		{
			throttle = info.msg.bradley.engineThrottle;
			rightThrottle = info.msg.bradley.throttleRight;
			leftThrottle = info.msg.bradley.throttleLeft;
			desiredAimVector = info.msg.bradley.mainGunVec;
			desiredTopTurretAimVector = info.msg.bradley.topTurretVec;
		}
	}

	public override void Save(SaveInfo info)
	{
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		base.Save(info);
		if (!info.forDisk)
		{
			info.msg.bradley = Pool.Get<BradleyAPC>();
			info.msg.bradley.engineThrottle = throttle;
			info.msg.bradley.throttleLeft = leftThrottle;
			info.msg.bradley.throttleRight = rightThrottle;
			info.msg.bradley.mainGunVec = turretAimVector;
			info.msg.bradley.topTurretVec = topTurretAimVector;
		}
	}

	public static BradleyAPC SpawnRoadDrivingBradley(Vector3 spawnPos, Quaternion spawnRot)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		RuntimePath runtimePath = new RuntimePath();
		PathList pathList = null;
		float num = float.PositiveInfinity;
		foreach (PathList road in TerrainMeta.Path.Roads)
		{
			_ = Vector3.zero;
			float num2 = float.PositiveInfinity;
			Vector3[] points = road.Path.Points;
			foreach (Vector3 val in points)
			{
				float num3 = Vector3.Distance(val, spawnPos);
				if (num3 < num2)
				{
					num2 = num3;
				}
			}
			if (num2 < num)
			{
				pathList = road;
				num = num2;
			}
		}
		if (pathList == null)
		{
			return null;
		}
		Vector3 startPoint = pathList.Path.GetStartPoint();
		Vector3 endPoint = pathList.Path.GetEndPoint();
		bool flag = startPoint == endPoint;
		int num4 = (flag ? (pathList.Path.Points.Length - 1) : pathList.Path.Points.Length);
		IAIPathNode[] nodes = new RuntimePathNode[num4];
		runtimePath.Nodes = nodes;
		IAIPathNode iAIPathNode = null;
		int num5 = 0;
		int num6 = (flag ? (pathList.Path.MaxIndex - 1) : pathList.Path.MaxIndex);
		for (int j = pathList.Path.MinIndex; j <= num6; j++)
		{
			IAIPathNode iAIPathNode2 = new RuntimePathNode(pathList.Path.Points[j] + Vector3.up * 1f);
			if (iAIPathNode != null)
			{
				iAIPathNode2.AddLink(iAIPathNode);
				iAIPathNode.AddLink(iAIPathNode2);
			}
			runtimePath.Nodes[num5] = iAIPathNode2;
			iAIPathNode = iAIPathNode2;
			num5++;
		}
		if (flag)
		{
			runtimePath.Nodes[0].AddLink(runtimePath.Nodes[runtimePath.Nodes.Length - 1]);
			runtimePath.Nodes[runtimePath.Nodes.Length - 1].AddLink(runtimePath.Nodes[0]);
		}
		else
		{
			RuntimeInterestNode interestNode = new RuntimeInterestNode(startPoint + Vector3.up * 1f);
			runtimePath.AddInterestNode(interestNode);
			RuntimeInterestNode interestNode2 = new RuntimeInterestNode(endPoint + Vector3.up * 1f);
			runtimePath.AddInterestNode(interestNode2);
		}
		int num7 = Mathf.CeilToInt(pathList.Path.Length / 500f);
		num7 = Mathf.Clamp(num7, 1, 3);
		if (flag)
		{
			num7++;
		}
		for (int k = 0; k < num7; k++)
		{
			int num8 = Random.Range(0, pathList.Path.Points.Length);
			RuntimeInterestNode interestNode3 = new RuntimeInterestNode(pathList.Path.Points[num8] + Vector3.up * 1f);
			runtimePath.AddInterestNode(interestNode3);
		}
		BaseEntity baseEntity = GameManager.server.CreateEntity("assets/prefabs/npc/m2bradley/bradleyapc.prefab", spawnPos, spawnRot);
		BradleyAPC bradleyAPC = null;
		if (Object.op_Implicit((Object)(object)baseEntity))
		{
			bradleyAPC = ((Component)baseEntity).GetComponent<BradleyAPC>();
			if (Object.op_Implicit((Object)(object)bradleyAPC))
			{
				bradleyAPC.Spawn();
				bradleyAPC.InstallPatrolPath(runtimePath);
			}
			else
			{
				baseEntity.Kill();
			}
		}
		return bradleyAPC;
	}

	[ServerVar(Name = "spawnroadbradley")]
	public static string svspawnroadbradley(Vector3 pos, Vector3 dir)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		if (!((Object)(object)SpawnRoadDrivingBradley(pos, Quaternion.LookRotation(dir, Vector3.up)) != (Object)null))
		{
			return "Failed to spawn road-driving Bradley.";
		}
		return "Spawned road-driving Bradley.";
	}

	public void SetDestination(Vector3 dest)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		destination = dest;
	}

	public override void ServerInit()
	{
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		base.ServerInit();
		Initialize();
		((FacepunchBehaviour)this).InvokeRepeating((Action)UpdateTargetList, 0f, 2f);
		((FacepunchBehaviour)this).InvokeRepeating((Action)UpdateTargetVisibilities, 0f, sightUpdateRate);
		obstacleHitMask = LayerMask.op_Implicit(LayerMask.GetMask(new string[1] { "Vehicle World" }));
		timeSinceSeemingStuck = TimeSince.op_Implicit(0f);
		timeSinceStuckReverseStart = TimeSince.op_Implicit(float.MaxValue);
	}

	public override void OnCollision(Collision collision, BaseEntity hitEntity)
	{
	}

	public void Initialize()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		myRigidBody.centerOfMass = centerOfMass.localPosition;
		destination = ((Component)this).transform.position;
		finalDestination = ((Component)this).transform.position;
	}

	public BasePlayer FollowPlayer()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		Enumerator<BasePlayer> enumerator = BasePlayer.activePlayerList.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				BasePlayer current = enumerator.Current;
				if (current.IsAdmin && current.IsAlive() && !current.IsSleeping() && current.GetActiveItem() != null && current.GetActiveItem().info.shortname == "tool.binoculars")
				{
					return current;
				}
			}
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
		return null;
	}

	public static Vector3 Direction2D(Vector3 aimAt, Vector3 aimFrom)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = new Vector3(aimAt.x, 0f, aimAt.z) - new Vector3(aimFrom.x, 0f, aimFrom.z);
		return ((Vector3)(ref val)).normalized;
	}

	public bool IsAtDestination()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		return Vector3Ex.Distance2D(((Component)this).transform.position, destination) <= stoppingDist;
	}

	public bool IsAtFinalDestination()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		return Vector3Ex.Distance2D(((Component)this).transform.position, finalDestination) <= stoppingDist;
	}

	public Vector3 ClosestPointAlongPath(Vector3 start, Vector3 end, Vector3 fromPos)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = end - start;
		Vector3 val2 = fromPos - start;
		float num = Vector3.Dot(val, val2);
		float num2 = Vector3.SqrMagnitude(end - start);
		float num3 = Mathf.Clamp01(num / num2);
		return start + val * num3;
	}

	public void FireGunTest()
	{
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		if (Time.time < nextFireTime)
		{
			return;
		}
		nextFireTime = Time.time + 0.25f;
		numBursted++;
		if (numBursted >= 4)
		{
			nextFireTime = Time.time + 5f;
			numBursted = 0;
		}
		Vector3 modifiedAimConeDirection = AimConeUtil.GetModifiedAimConeDirection(2f, CannonMuzzle.rotation * Vector3.forward);
		Vector3 val = ((Component)CannonPitch).transform.rotation * Vector3.back + ((Component)this).transform.up * -1f;
		Vector3 normalized = ((Vector3)(ref val)).normalized;
		myRigidBody.AddForceAtPosition(normalized * recoilScale, ((Component)CannonPitch).transform.position, (ForceMode)1);
		Effect.server.Run(mainCannonMuzzleFlash.resourcePath, this, StringPool.Get(((Object)((Component)CannonMuzzle).gameObject).name), Vector3.zero, Vector3.zero);
		BaseEntity baseEntity = GameManager.server.CreateEntity(mainCannonProjectile.resourcePath, ((Component)CannonMuzzle).transform.position, Quaternion.LookRotation(modifiedAimConeDirection));
		if (!((Object)(object)baseEntity == (Object)null))
		{
			ServerProjectile component = ((Component)baseEntity).GetComponent<ServerProjectile>();
			if (Object.op_Implicit((Object)(object)component))
			{
				component.InitializeVelocity(modifiedAimConeDirection * component.speed);
			}
			baseEntity.Spawn();
		}
	}

	public void InstallPatrolPath(IAIPath path)
	{
		patrolPath = path;
		currentPath = new List<Vector3>();
		currentPathIndex = -1;
	}

	public void UpdateMovement_Patrol()
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		if (patrolPath == null || Time.time < nextPatrolTime)
		{
			return;
		}
		nextPatrolTime = Time.time + 20f;
		if (HasPath() && !IsAtFinalDestination())
		{
			return;
		}
		IAIPathInterestNode randomInterestNodeAwayFrom = patrolPath.GetRandomInterestNodeAwayFrom(((Component)this).transform.position);
		IAIPathNode closestToPoint = patrolPath.GetClosestToPoint(randomInterestNodeAwayFrom.Position);
		bool flag = false;
		List<IAIPathNode> nodes = Pool.GetList<IAIPathNode>();
		IAIPathNode iAIPathNode;
		if (GetEngagementPath(ref nodes))
		{
			flag = true;
			iAIPathNode = nodes[nodes.Count - 1];
		}
		else
		{
			iAIPathNode = patrolPath.GetClosestToPoint(((Component)this).transform.position);
		}
		if (!(Vector3.Distance(finalDestination, closestToPoint.Position) > 2f))
		{
			return;
		}
		if (closestToPoint == iAIPathNode)
		{
			currentPath.Clear();
			currentPath.Add(closestToPoint.Position);
			currentPathIndex = -1;
			pathLooping = false;
			finalDestination = closestToPoint.Position;
		}
		else
		{
			if (!AStarPath.FindPath(iAIPathNode, closestToPoint, out var path, out var _))
			{
				return;
			}
			currentPath.Clear();
			if (flag)
			{
				for (int i = 0; i < nodes.Count - 1; i++)
				{
					currentPath.Add(nodes[i].Position);
				}
			}
			foreach (IAIPathNode item in path)
			{
				currentPath.Add(item.Position);
			}
			currentPathIndex = -1;
			pathLooping = false;
			finalDestination = closestToPoint.Position;
		}
	}

	public void UpdateMovement_Hunt()
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		if (patrolPath == null)
		{
			return;
		}
		TargetInfo targetInfo = targetList[0];
		if (!targetInfo.IsValid())
		{
			return;
		}
		if (HasPath() && targetInfo.IsVisible())
		{
			if (currentPath.Count > 1)
			{
				Vector3 item = currentPath[currentPathIndex];
				ClearPath();
				currentPath.Add(item);
				finalDestination = item;
				currentPathIndex = 0;
			}
		}
		else
		{
			if (!(Time.time > nextEngagementPathTime) || HasPath() || targetInfo.IsVisible())
			{
				return;
			}
			bool flag = false;
			IAIPathNode start = patrolPath.GetClosestToPoint(((Component)this).transform.position);
			List<IAIPathNode> nodes = Pool.GetList<IAIPathNode>();
			if (GetEngagementPath(ref nodes))
			{
				flag = true;
				start = nodes[nodes.Count - 1];
			}
			IAIPathNode iAIPathNode = null;
			List<IAIPathNode> nearNodes = Pool.GetList<IAIPathNode>();
			patrolPath.GetNodesNear(targetInfo.lastSeenPosition, ref nearNodes, 30f);
			Stack<IAIPathNode> stack = null;
			float num = float.PositiveInfinity;
			float y = mainTurretEyePos.localPosition.y;
			foreach (IAIPathNode item2 in nearNodes)
			{
				Stack<IAIPathNode> path = new Stack<IAIPathNode>();
				if (targetInfo.entity.IsVisible(item2.Position + new Vector3(0f, y, 0f)) && AStarPath.FindPath(start, item2, out path, out var pathCost) && pathCost < num)
				{
					stack = path;
					num = pathCost;
					iAIPathNode = item2;
				}
			}
			if (stack == null && nearNodes.Count > 0)
			{
				Stack<IAIPathNode> path2 = new Stack<IAIPathNode>();
				IAIPathNode iAIPathNode2 = nearNodes[Random.Range(0, nearNodes.Count)];
				if (AStarPath.FindPath(start, iAIPathNode2, out path2, out var pathCost2) && pathCost2 < num)
				{
					stack = path2;
					iAIPathNode = iAIPathNode2;
				}
			}
			if (stack != null)
			{
				currentPath.Clear();
				if (flag)
				{
					for (int i = 0; i < nodes.Count - 1; i++)
					{
						currentPath.Add(nodes[i].Position);
					}
				}
				foreach (IAIPathNode item3 in stack)
				{
					currentPath.Add(item3.Position);
				}
				currentPathIndex = -1;
				pathLooping = false;
				finalDestination = iAIPathNode.Position;
			}
			Pool.FreeList<IAIPathNode>(ref nearNodes);
			Pool.FreeList<IAIPathNode>(ref nodes);
			nextEngagementPathTime = Time.time + 5f;
		}
	}

	public void DoSimpleAI()
	{
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		if (base.isClient)
		{
			return;
		}
		SetFlag(Flags.Reserved5, TOD_Sky.Instance.IsNight);
		if (!DoAI)
		{
			return;
		}
		if (targetList.Count > 0)
		{
			if (targetList[0].IsValid() && targetList[0].IsVisible())
			{
				mainGunTarget = targetList[0].entity as BaseCombatEntity;
			}
			else
			{
				mainGunTarget = null;
			}
			UpdateMovement_Hunt();
		}
		else
		{
			mainGunTarget = null;
			UpdateMovement_Patrol();
		}
		AdvancePathMovement(force: false);
		float num = Vector3.Distance(((Component)this).transform.position, destination);
		float num2 = Vector3.Distance(((Component)this).transform.position, finalDestination);
		if (num > stoppingDist)
		{
			Vector3 val = Direction2D(destination, ((Component)this).transform.position);
			float num3 = Vector3.Dot(val, ((Component)this).transform.right);
			float num4 = Vector3.Dot(val, ((Component)this).transform.right);
			float num5 = Vector3.Dot(val, -((Component)this).transform.right);
			if (Vector3.Dot(val, -((Component)this).transform.forward) > num3)
			{
				if (num4 >= num5)
				{
					turning = 1f;
				}
				else
				{
					turning = -1f;
				}
			}
			else
			{
				turning = Mathf.Clamp(num3 * 3f, -1f, 1f);
			}
			float throttleScaleFromTurn = 1f - Mathf.InverseLerp(0f, 0.3f, Mathf.Abs(turning));
			AvoidObstacles(ref throttleScaleFromTurn);
			float num6 = Vector3.Dot(myRigidBody.velocity, ((Component)this).transform.forward);
			if (!(throttle > 0f) || !(num6 < 0.5f))
			{
				timeSinceSeemingStuck = TimeSince.op_Implicit(0f);
			}
			else if (TimeSince.op_Implicit(timeSinceSeemingStuck) > 10f)
			{
				timeSinceStuckReverseStart = TimeSince.op_Implicit(0f);
				timeSinceSeemingStuck = TimeSince.op_Implicit(0f);
			}
			float num7 = Mathf.InverseLerp(0.1f, 0.4f, Vector3.Dot(((Component)this).transform.forward, Vector3.up));
			if (TimeSince.op_Implicit(timeSinceStuckReverseStart) < 3f)
			{
				throttle = -0.75f;
				turning = 1f;
			}
			else
			{
				throttle = (0.1f + Mathf.InverseLerp(0f, 20f, num2) * 1f) * throttleScaleFromTurn + num7;
			}
		}
		DoWeaponAiming();
		SendNetworkUpdate();
	}

	public void FixedUpdate()
	{
		DoSimpleAI();
		DoPhysicsMove();
		DoWeapons();
		DoHealing();
	}

	private void AvoidObstacles(ref float throttleScaleFromTurn)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		Ray ray = default(Ray);
		((Ray)(ref ray))._002Ector(((Component)this).transform.position + ((Component)this).transform.forward * (((Bounds)(ref bounds)).extents.z - 1f), ((Component)this).transform.forward);
		if (!GamePhysics.Trace(ray, 3f, out var hitInfo, 20f, LayerMask.op_Implicit(obstacleHitMask), (QueryTriggerInteraction)1, this))
		{
			return;
		}
		if (((RaycastHit)(ref hitInfo)).point == Vector3.zero)
		{
			((RaycastHit)(ref hitInfo)).point = ((RaycastHit)(ref hitInfo)).collider.ClosestPointOnBounds(((Ray)(ref ray)).origin);
		}
		float num = TransformEx.AngleToPos(((Component)this).transform, ((RaycastHit)(ref hitInfo)).point);
		float num2 = Mathf.Abs(num);
		if (num2 > 75f || !(((RaycastHit)(ref hitInfo)).collider.ToBaseEntity() is BradleyAPC))
		{
			return;
		}
		bool flag = false;
		if (num2 < 5f)
		{
			float num3 = ((throttle < 0f) ? 150f : 50f);
			if (Vector3.SqrMagnitude(((Component)this).transform.position - ((RaycastHit)(ref hitInfo)).point) < num3)
			{
				flag = true;
			}
		}
		if (num > 30f)
		{
			turning = -1f;
		}
		else
		{
			turning = 1f;
		}
		throttleScaleFromTurn = (flag ? (-1f) : 1f);
		int num4 = currentPathIndex;
		_ = currentPathIndex;
		float num5 = Vector3.Distance(((Component)this).transform.position, destination);
		while (HasPath() && (double)num5 < 26.6 && currentPathIndex >= 0)
		{
			int num6 = currentPathIndex;
			AdvancePathMovement(force: true);
			num5 = Vector3.Distance(((Component)this).transform.position, destination);
			if (currentPathIndex == num4 || currentPathIndex == num6)
			{
				break;
			}
		}
	}

	public void DoPhysicsMove()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0348: Unknown result type (might be due to invalid IL or missing references)
		//IL_034d: Unknown result type (might be due to invalid IL or missing references)
		if (base.isClient)
		{
			return;
		}
		Vector3 velocity = myRigidBody.velocity;
		throttle = Mathf.Clamp(throttle, -1f, 1f);
		leftThrottle = throttle;
		rightThrottle = throttle;
		if (turning > 0f)
		{
			rightThrottle = 0f - turning;
			leftThrottle = turning;
		}
		else if (turning < 0f)
		{
			leftThrottle = turning;
			rightThrottle = turning * -1f;
		}
		Vector3.Distance(((Component)this).transform.position, GetFinalDestination());
		float num = Vector3.Distance(((Component)this).transform.position, GetCurrentPathDestination());
		float num2 = 15f;
		if (num < 20f)
		{
			float num3 = Vector3.Dot(PathDirection(currentPathIndex), PathDirection(currentPathIndex + 1));
			float num4 = Mathf.InverseLerp(2f, 10f, num);
			float num5 = Mathf.InverseLerp(0.5f, 0.8f, num3);
			num2 = 15f - 14f * ((1f - num5) * (1f - num4));
		}
		_ = 20f;
		if (patrolPath != null)
		{
			float num6 = num2;
			foreach (IAIPathSpeedZone speedZone in patrolPath.SpeedZones)
			{
				OBB val = speedZone.WorldSpaceBounds();
				if (((OBB)(ref val)).Contains(((Component)this).transform.position))
				{
					num6 = Mathf.Min(num6, speedZone.GetMaxSpeed());
				}
			}
			currentSpeedZoneLimit = Mathf.Lerp(currentSpeedZoneLimit, num6, Time.deltaTime);
			num2 = Mathf.Min(num2, currentSpeedZoneLimit);
		}
		if (PathComplete())
		{
			num2 = 0f;
		}
		if (Global.developer > 1)
		{
			Debug.Log((object)("velocity:" + ((Vector3)(ref velocity)).magnitude + "max : " + num2));
		}
		brake = ((Vector3)(ref velocity)).magnitude >= num2;
		ApplyBrakes(brake ? 1f : 0f);
		float num7 = throttle;
		leftThrottle = Mathf.Clamp(leftThrottle + num7, -1f, 1f);
		rightThrottle = Mathf.Clamp(rightThrottle + num7, -1f, 1f);
		float num8 = Mathf.InverseLerp(2f, 1f, ((Vector3)(ref velocity)).magnitude * Mathf.Abs(Vector3.Dot(((Vector3)(ref velocity)).normalized, ((Component)this).transform.forward)));
		float torqueAmount = Mathf.Lerp(moveForceMax, turnForce, num8);
		float num9 = Mathf.InverseLerp(5f, 1.5f, ((Vector3)(ref velocity)).magnitude * Mathf.Abs(Vector3.Dot(((Vector3)(ref velocity)).normalized, ((Component)this).transform.forward)));
		ScaleSidewaysFriction(1f - num9);
		SetMotorTorque(leftThrottle, rightSide: false, torqueAmount);
		SetMotorTorque(rightThrottle, rightSide: true, torqueAmount);
		TriggerHurtEx triggerHurtEx = impactDamager;
		Vector3 velocity2 = myRigidBody.velocity;
		triggerHurtEx.damageEnabled = ((Vector3)(ref velocity2)).magnitude > 2f;
	}

	public void ApplyBrakes(float amount)
	{
		ApplyBrakeTorque(amount, rightSide: true);
		ApplyBrakeTorque(amount, rightSide: false);
	}

	public float GetMotorTorque(bool rightSide)
	{
		float num = 0f;
		WheelCollider[] array = (rightSide ? rightWheels : leftWheels);
		foreach (WheelCollider val in array)
		{
			num += val.motorTorque;
		}
		return num / (float)rightWheels.Length;
	}

	public void ScaleSidewaysFriction(float scale)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		float stiffness = 0.75f + 0.75f * scale;
		WheelCollider[] array = rightWheels;
		foreach (WheelCollider obj in array)
		{
			WheelFrictionCurve sidewaysFriction = obj.sidewaysFriction;
			((WheelFrictionCurve)(ref sidewaysFriction)).stiffness = stiffness;
			obj.sidewaysFriction = sidewaysFriction;
		}
		array = leftWheels;
		foreach (WheelCollider obj2 in array)
		{
			WheelFrictionCurve sidewaysFriction2 = obj2.sidewaysFriction;
			((WheelFrictionCurve)(ref sidewaysFriction2)).stiffness = stiffness;
			obj2.sidewaysFriction = sidewaysFriction2;
		}
	}

	public void SetMotorTorque(float newThrottle, bool rightSide, float torqueAmount)
	{
		newThrottle = Mathf.Clamp(newThrottle, -1f, 1f);
		float num = torqueAmount * newThrottle;
		int num2 = (rightSide ? rightWheels.Length : leftWheels.Length);
		int num3 = 0;
		WheelCollider[] array = (rightSide ? rightWheels : leftWheels);
		WheelHit val = default(WheelHit);
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].GetGroundHit(ref val))
			{
				num3++;
			}
		}
		float num4 = 1f;
		if (num3 > 0)
		{
			num4 = num2 / num3;
		}
		array = (rightSide ? rightWheels : leftWheels);
		WheelHit val3 = default(WheelHit);
		foreach (WheelCollider val2 in array)
		{
			if (val2.GetGroundHit(ref val3))
			{
				val2.motorTorque = num * num4;
			}
			else
			{
				val2.motorTorque = num;
			}
		}
	}

	public void ApplyBrakeTorque(float amount, bool rightSide)
	{
		WheelCollider[] array = (rightSide ? rightWheels : leftWheels);
		for (int i = 0; i < array.Length; i++)
		{
			array[i].brakeTorque = brakeForce * amount;
		}
	}

	public void CreateExplosionMarker(float durationMinutes)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		BaseEntity baseEntity = GameManager.server.CreateEntity(debrisFieldMarker.resourcePath, ((Component)this).transform.position, Quaternion.identity);
		baseEntity.Spawn();
		((Component)baseEntity).SendMessage("SetDuration", (object)durationMinutes, (SendMessageOptions)1);
	}

	public override void OnKilled(HitInfo info)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0301: Unknown result type (might be due to invalid IL or missing references)
		//IL_0307: Unknown result type (might be due to invalid IL or missing references)
		//IL_030b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0311: Unknown result type (might be due to invalid IL or missing references)
		if (base.isClient)
		{
			return;
		}
		CreateExplosionMarker(10f);
		Effect.server.Run(explosionEffect.resourcePath, ((Component)mainTurretEyePos).transform.position, Vector3.up, null, broadcast: true);
		Vector3 zero = Vector3.zero;
		GameObject gibSource = servergibs.Get().GetComponent<ServerGib>()._gibSource;
		List<ServerGib> list = ServerGib.CreateGibs(servergibs.resourcePath, ((Component)this).gameObject, gibSource, zero, 3f);
		for (int i = 0; i < 12 - maxCratesToSpawn; i++)
		{
			BaseEntity baseEntity = GameManager.server.CreateEntity(this.fireBall.resourcePath, ((Component)this).transform.position, ((Component)this).transform.rotation);
			if (!Object.op_Implicit((Object)(object)baseEntity))
			{
				continue;
			}
			float num = 3f;
			float num2 = 10f;
			Vector3 onUnitSphere = Random.onUnitSphere;
			((Component)baseEntity).transform.position = ((Component)this).transform.position + new Vector3(0f, 1.5f, 0f) + onUnitSphere * Random.Range(-4f, 4f);
			Collider component = ((Component)baseEntity).GetComponent<Collider>();
			baseEntity.Spawn();
			baseEntity.SetVelocity(zero + onUnitSphere * Random.Range(num, num2));
			foreach (ServerGib item in list)
			{
				Physics.IgnoreCollision(component, (Collider)(object)item.GetCollider(), true);
			}
		}
		for (int j = 0; j < maxCratesToSpawn; j++)
		{
			Vector3 onUnitSphere2 = Random.onUnitSphere;
			onUnitSphere2.y = 0f;
			((Vector3)(ref onUnitSphere2)).Normalize();
			Vector3 pos = ((Component)this).transform.position + new Vector3(0f, 1.5f, 0f) + onUnitSphere2 * Random.Range(2f, 3f);
			BaseEntity baseEntity2 = GameManager.server.CreateEntity(crateToDrop.resourcePath, pos, Quaternion.LookRotation(onUnitSphere2));
			baseEntity2.Spawn();
			LootContainer lootContainer = baseEntity2 as LootContainer;
			if (Object.op_Implicit((Object)(object)lootContainer))
			{
				((FacepunchBehaviour)lootContainer).Invoke((Action)lootContainer.RemoveMe, 1800f);
			}
			Collider component2 = ((Component)baseEntity2).GetComponent<Collider>();
			Rigidbody val = ((Component)baseEntity2).gameObject.AddComponent<Rigidbody>();
			val.useGravity = true;
			val.collisionDetectionMode = (CollisionDetectionMode)2;
			val.mass = 2f;
			val.interpolation = (RigidbodyInterpolation)1;
			val.velocity = zero + onUnitSphere2 * Random.Range(1f, 3f);
			val.angularVelocity = Vector3Ex.Range(-1.75f, 1.75f);
			val.drag = 0.5f * (val.mass / 5f);
			val.angularDrag = 0.2f * (val.mass / 5f);
			FireBall fireBall = GameManager.server.CreateEntity(this.fireBall.resourcePath) as FireBall;
			if (Object.op_Implicit((Object)(object)fireBall))
			{
				fireBall.SetParent(baseEntity2);
				fireBall.Spawn();
				((Component)fireBall).GetComponent<Rigidbody>().isKinematic = true;
				((Component)fireBall).GetComponent<Collider>().enabled = false;
			}
			((Component)baseEntity2).SendMessage("SetLockingEnt", (object)((Component)fireBall).gameObject, (SendMessageOptions)1);
			foreach (ServerGib item2 in list)
			{
				Physics.IgnoreCollision(component2, (Collider)(object)item2.GetCollider(), true);
			}
		}
		base.OnKilled(info);
	}

	public override void OnAttacked(HitInfo info)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		base.OnAttacked(info);
		BasePlayer basePlayer = info.Initiator as BasePlayer;
		if ((Object)(object)basePlayer != (Object)null)
		{
			AddOrUpdateTarget(basePlayer, info.PointStart, info.damageTypes.Total());
		}
	}

	public override void OnHealthChanged(float oldvalue, float newvalue)
	{
		base.OnHealthChanged(oldvalue, newvalue);
		if (base.isServer)
		{
			SetFlag(Flags.Reserved2, base.healthFraction <= 0.75f);
			SetFlag(Flags.Reserved3, base.healthFraction < 0.4f);
		}
	}

	public void DoHealing()
	{
		if (!base.isClient && base.healthFraction < 1f && base.SecondsSinceAttacked > 600f)
		{
			float amount = MaxHealth() / 300f * Time.fixedDeltaTime;
			Heal(amount);
		}
	}

	public BasePlayer GetPlayerDamageInitiator()
	{
		return null;
	}

	public float GetDamageMultiplier(BaseEntity ent)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		float num = ((throttle > 0f) ? 10f : 0f);
		float num2 = Vector3.Dot(myRigidBody.velocity, ((Component)this).transform.forward);
		if (num2 > 0f)
		{
			num += num2 * 0.5f;
		}
		if (ent is BaseVehicle)
		{
			num *= 10f;
		}
		return num;
	}

	public void OnHurtTriggerOccupant(BaseEntity hurtEntity, DamageType damageType, float damageTotal)
	{
	}
}
