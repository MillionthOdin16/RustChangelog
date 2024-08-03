using System;
using System.Collections.Generic;
using Rust;
using UnityEngine;
using UnityEngine.Profiling;

public class SimpleShark : BaseCombatEntity
{
	public class SimpleState
	{
		public SimpleShark entity = null;

		private float stateEnterTime = 0f;

		public SimpleState(SimpleShark owner)
		{
			entity = owner;
		}

		public virtual float State_Weight()
		{
			return 0f;
		}

		public virtual void State_Enter()
		{
			stateEnterTime = Time.realtimeSinceStartup;
		}

		public virtual void State_Think(float delta)
		{
		}

		public virtual void State_Exit()
		{
		}

		public virtual bool CanInterrupt()
		{
			return true;
		}

		public virtual float TimeInState()
		{
			return Time.realtimeSinceStartup - stateEnterTime;
		}
	}

	public class IdleState : SimpleState
	{
		private int patrolTargetIndex = 0;

		public IdleState(SimpleShark owner)
			: base(owner)
		{
		}

		public Vector3 GetTargetPatrolPosition()
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			return entity.patrolPath[patrolTargetIndex];
		}

		public override float State_Weight()
		{
			return 1f;
		}

		public override void State_Enter()
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			float num = float.PositiveInfinity;
			int num2 = 0;
			for (int i = 0; i < entity.patrolPath.Count; i++)
			{
				Vector3 val = entity.patrolPath[i];
				float num3 = Vector3.Distance(val, ((Component)entity).transform.position);
				if (num3 < num)
				{
					num2 = i;
					num = num3;
				}
			}
			patrolTargetIndex = num2;
			base.State_Enter();
		}

		public override void State_Think(float delta)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			Vector3 targetPatrolPosition = GetTargetPatrolPosition();
			if (Vector3.Distance(targetPatrolPosition, ((Component)entity).transform.position) < entity.stoppingDistance)
			{
				patrolTargetIndex++;
				if (patrolTargetIndex >= entity.patrolPath.Count)
				{
					patrolTargetIndex = 0;
				}
			}
			if (entity.TimeSinceAttacked() >= 120f && entity.healthFraction < 1f)
			{
				entity.health = entity.MaxHealth();
			}
			entity.destination = entity.WaterClamp(GetTargetPatrolPosition());
		}

		public override void State_Exit()
		{
			base.State_Exit();
		}

		public override bool CanInterrupt()
		{
			return true;
		}
	}

	public class AttackState : SimpleState
	{
		public AttackState(SimpleShark owner)
			: base(owner)
		{
		}

		public override float State_Weight()
		{
			return (entity.HasTarget() && entity.CanAttack()) ? 10f : 0f;
		}

		public override void State_Enter()
		{
			base.State_Enter();
		}

		public override void State_Think(float delta)
		{
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_010b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_011d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0136: Unknown result type (might be due to invalid IL or missing references)
			//IL_0138: Unknown result type (might be due to invalid IL or missing references)
			//IL_013d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_0147: Unknown result type (might be due to invalid IL or missing references)
			BasePlayer target = entity.GetTarget();
			if ((Object)(object)target == (Object)null)
			{
				return;
			}
			if (TimeInState() >= 10f)
			{
				entity.nextAttackTime = Time.realtimeSinceStartup + 4f;
				entity.Startle();
				return;
			}
			if (entity.CanAttack())
			{
				entity.Startle();
			}
			float num = Vector3.Distance(entity.GetTarget().eyes.position, ((Component)entity).transform.position);
			bool flag = num < 4f;
			if (entity.CanAttack() && num <= 2f)
			{
				entity.DoAttack();
			}
			if (!flag)
			{
				Vector3 val = Vector3Ex.Direction(entity.GetTarget().eyes.position, ((Component)entity).transform.position);
				Vector3 point = target.eyes.position + val * 10f;
				point = entity.WaterClamp(point);
				entity.destination = point;
			}
		}

		public override void State_Exit()
		{
			base.State_Exit();
		}

		public override bool CanInterrupt()
		{
			return true;
		}
	}

	public Vector3 destination;

	public float minSpeed;

	public float maxSpeed;

	public float idealDepth;

	public float minTurnSpeed = 0.25f;

	public float maxTurnSpeed = 2f;

	public float attackCooldown = 7f;

	public float aggroRange = 15f;

	public float obstacleDetectionRadius = 1f;

	public Animator animator;

	public GameObjectRef bloodCloud;

	public GameObjectRef corpsePrefab;

	private const string SPEARGUN_KILL_STAT = "shark_speargun_kills";

	[ServerVar]
	public static float forceSurfaceAmount = 0f;

	[ServerVar]
	public static bool disable = false;

	private Vector3 spawnPos;

	private float stoppingDistance = 3f;

	private float currentSpeed = 0f;

	private float lastStartleTime = 0f;

	private float startleDuration = 1f;

	private SimpleState[] states;

	private SimpleState _currentState;

	private bool sleeping = false;

	public List<Vector3> patrolPath = new List<Vector3>();

	private BasePlayer target;

	private float lastSeenTargetTime = 0f;

	private float nextTargetSearchTime = 0f;

	private static BasePlayer[] playerQueryResults = new BasePlayer[64];

	private float minFloorDist = 2f;

	private float minSurfaceDist = 1f;

	private float lastTimeAttacked = 0f;

	private float nextAttackTime = 0f;

	private Vector3 cachedObstacleNormal;

	private float cachedObstacleDistance = 0f;

	private float obstacleAvoidanceScale = 0f;

	private float obstacleDetectionRange = 5f;

	private float timeSinceLastObstacleCheck = 0f;

	public override bool IsNpc => true;

	private void GenerateIdlePoints(Vector3 center, float radius, float heightOffset, float staggerOffset = 0f)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		patrolPath.Clear();
		float num = 0f;
		int num2 = 32;
		int num3 = 10551553;
		float height = TerrainMeta.WaterMap.GetHeight(center);
		float height2 = TerrainMeta.HeightMap.GetHeight(center);
		RaycastHit val2 = default(RaycastHit);
		for (int i = 0; i < num2; i++)
		{
			num += 360f / (float)num2;
			float radius2 = 1f;
			Vector3 pointOnCircle = BasePathFinder.GetPointOnCircle(center, radius2, num);
			Vector3 val = Vector3Ex.Direction(pointOnCircle, center);
			pointOnCircle = ((!Physics.SphereCast(center, obstacleDetectionRadius, val, ref val2, radius + staggerOffset, num3)) ? (center + val * radius) : (center + val * (((RaycastHit)(ref val2)).distance - 6f)));
			if (staggerOffset != 0f)
			{
				pointOnCircle += val * Random.Range(0f - staggerOffset, staggerOffset);
			}
			pointOnCircle.y += Random.Range(0f - heightOffset, heightOffset);
			pointOnCircle.y = Mathf.Clamp(pointOnCircle.y, height2 + 3f, height - 3f);
			patrolPath.Add(pointOnCircle);
		}
	}

	private void GenerateIdlePoints_Shrinkwrap(Vector3 center, float radius, float heightOffset, float staggerOffset = 0f)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		patrolPath.Clear();
		float num = 0f;
		int num2 = 32;
		int num3 = 10551553;
		float height = TerrainMeta.WaterMap.GetHeight(center);
		float height2 = TerrainMeta.HeightMap.GetHeight(center);
		RaycastHit val2 = default(RaycastHit);
		for (int i = 0; i < num2; i++)
		{
			num += 360f / (float)num2;
			float radius2 = radius * 2f;
			Vector3 pointOnCircle = BasePathFinder.GetPointOnCircle(center, radius2, num);
			Vector3 val = Vector3Ex.Direction(center, pointOnCircle);
			pointOnCircle = ((!Physics.SphereCast(pointOnCircle, obstacleDetectionRadius, val, ref val2, radius + staggerOffset, num3)) ? (pointOnCircle + val * radius) : (((RaycastHit)(ref val2)).point - val * 6f));
			if (staggerOffset != 0f)
			{
				pointOnCircle += val * Random.Range(0f - staggerOffset, staggerOffset);
			}
			pointOnCircle.y += Random.Range(0f - heightOffset, heightOffset);
			pointOnCircle.y = Mathf.Clamp(pointOnCircle.y, height2 + 3f, height - 3f);
			patrolPath.Add(pointOnCircle);
		}
	}

	public override void ServerInit()
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		base.ServerInit();
		if (disable)
		{
			((FacepunchBehaviour)this).Invoke((Action)base.KillMessage, 0.01f);
			return;
		}
		((Component)this).transform.position = WaterClamp(((Component)this).transform.position);
		Init();
		((FacepunchBehaviour)this).InvokeRandomized((Action)CheckSleepState, 0f, 1f, 0.5f);
	}

	public void CheckSleepState()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		Vector3 position = ((Component)this).transform.position;
		bool flag = BaseNetworkable.HasCloseConnections(position, 100f);
		sleeping = !flag;
	}

	public void Init()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		GenerateIdlePoints_Shrinkwrap(((Component)this).transform.position, 20f, 2f, 3f);
		states = new SimpleState[2];
		states[0] = new IdleState(this);
		states[1] = new AttackState(this);
		((Component)this).transform.position = patrolPath[0];
	}

	private void Think(float delta)
	{
		if (states == null)
		{
			return;
		}
		if (disable)
		{
			if (!((FacepunchBehaviour)this).IsInvoking((Action)base.KillMessage))
			{
				((FacepunchBehaviour)this).Invoke((Action)base.KillMessage, 0.01f);
			}
		}
		else
		{
			if (sleeping)
			{
				return;
			}
			Profiler.BeginSample("SimpleShark.Think");
			Profiler.BeginSample("State Weighing");
			SimpleState simpleState = null;
			float num = -1f;
			SimpleState[] array = states;
			foreach (SimpleState simpleState2 in array)
			{
				float num2 = simpleState2.State_Weight();
				if (num2 > num)
				{
					simpleState = simpleState2;
					num = num2;
				}
			}
			Profiler.EndSample();
			Profiler.BeginSample("State Switching");
			if (simpleState != _currentState && (_currentState == null || _currentState.CanInterrupt()))
			{
				if (_currentState != null)
				{
					_currentState.State_Exit();
				}
				simpleState.State_Enter();
				_currentState = simpleState;
			}
			Profiler.EndSample();
			UpdateTarget(delta);
			_currentState.State_Think(delta);
			UpdateObstacleAvoidance(delta);
			UpdateDirection(delta);
			UpdateSpeed(delta);
			UpdatePosition(delta);
			SetFlag(Flags.Open, HasTarget() && CanAttack());
			Profiler.EndSample();
		}
	}

	public Vector3 WaterClamp(Vector3 point)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		float height = WaterSystem.GetHeight(point);
		float height2 = TerrainMeta.HeightMap.GetHeight(point);
		float num = height2 + minFloorDist;
		float num2 = height - minSurfaceDist;
		if (forceSurfaceAmount != 0f)
		{
			height = WaterSystem.GetHeight(point);
			num = (num2 = height + forceSurfaceAmount);
		}
		point.y = Mathf.Clamp(point.y, num, num2);
		return point;
	}

	public bool ValidTarget(BasePlayer newTarget)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		float num = Vector3.Distance(newTarget.eyes.position, ((Component)this).transform.position);
		Vector3 val = Vector3Ex.Direction(newTarget.eyes.position, ((Component)this).transform.position);
		int num2 = 10551552;
		if (Physics.Raycast(((Component)this).transform.position, val, num, num2))
		{
			return false;
		}
		if (newTarget.isMounted)
		{
			if (Object.op_Implicit((Object)(object)newTarget.GetMountedVehicle()))
			{
				return false;
			}
			WaterInflatable component = ((Component)newTarget.GetMounted()).GetComponent<WaterInflatable>();
			if (!((Behaviour)component.buoyancy).enabled)
			{
				return false;
			}
		}
		else if (!WaterLevel.Test(newTarget.CenterPoint(), waves: true, volumes: false, newTarget))
		{
			return false;
		}
		return true;
	}

	public void ClearTarget()
	{
		target = null;
		lastSeenTargetTime = 0f;
	}

	public override void OnKilled(HitInfo hitInfo = null)
	{
		if (base.isServer)
		{
			if (GameInfo.HasAchievements && hitInfo != null && (Object)(object)hitInfo.InitiatorPlayer != (Object)null && !hitInfo.InitiatorPlayer.IsNpc && (Object)(object)hitInfo.Weapon != (Object)null && hitInfo.Weapon.ShortPrefabName.Contains("speargun"))
			{
				hitInfo.InitiatorPlayer.stats.Add("shark_speargun_kills", 1, Stats.All);
				hitInfo.InitiatorPlayer.stats.Save(forceSteamSave: true);
			}
			BaseCorpse baseCorpse = DropCorpse(corpsePrefab.resourcePath);
			if (Object.op_Implicit((Object)(object)baseCorpse))
			{
				baseCorpse.Spawn();
				baseCorpse.TakeChildren(this);
			}
			((FacepunchBehaviour)this).Invoke((Action)base.KillMessage, 0.5f);
		}
		base.OnKilled(hitInfo);
	}

	public void UpdateTarget(float delta)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		Profiler.BeginSample("UpdateTarget");
		if ((Object)(object)target != (Object)null)
		{
			float num = Vector3.Distance(target.eyes.position, ((Component)this).transform.position);
			bool flag = num > aggroRange * 2f;
			bool flag2 = Time.realtimeSinceStartup > lastSeenTargetTime + 4f;
			if (!ValidTarget(target) || flag || flag2)
			{
				ClearTarget();
			}
			else
			{
				lastSeenTargetTime = Time.realtimeSinceStartup;
			}
		}
		if (Time.realtimeSinceStartup < nextTargetSearchTime)
		{
			Profiler.EndSample();
			return;
		}
		if ((Object)(object)target == (Object)null)
		{
			nextTargetSearchTime = Time.realtimeSinceStartup + 1f;
			if (BaseNetworkable.HasCloseConnections(((Component)this).transform.position, aggroRange))
			{
				int playersInSphere = Query.Server.GetPlayersInSphere(((Component)this).transform.position, aggroRange, playerQueryResults);
				for (int i = 0; i < playersInSphere; i++)
				{
					BasePlayer basePlayer = playerQueryResults[i];
					if (!basePlayer.isClient && ValidTarget(basePlayer))
					{
						target = basePlayer;
						lastSeenTargetTime = Time.realtimeSinceStartup;
						break;
					}
				}
			}
		}
		Profiler.EndSample();
	}

	public float TimeSinceAttacked()
	{
		return Time.realtimeSinceStartup - lastTimeAttacked;
	}

	public override void OnAttacked(HitInfo info)
	{
		base.OnAttacked(info);
		lastTimeAttacked = Time.realtimeSinceStartup;
		if (info.damageTypes.Total() > 20f)
		{
			Startle();
		}
		if ((Object)(object)info.InitiatorPlayer != (Object)null && (Object)(object)target == (Object)null && ValidTarget(info.InitiatorPlayer))
		{
			target = info.InitiatorPlayer;
			lastSeenTargetTime = Time.realtimeSinceStartup;
		}
	}

	public bool HasTarget()
	{
		return (Object)(object)target != (Object)null;
	}

	public BasePlayer GetTarget()
	{
		return target;
	}

	public override string Categorize()
	{
		return "Shark";
	}

	public bool CanAttack()
	{
		return Time.realtimeSinceStartup > nextAttackTime;
	}

	public void DoAttack()
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		if (HasTarget())
		{
			GetTarget().Hurt(Random.Range(30f, 70f), DamageType.Bite, this);
			Vector3 posWorld = WaterClamp(GetTarget().CenterPoint());
			Effect.server.Run(bloodCloud.resourcePath, posWorld, Vector3.forward);
			nextAttackTime = Time.realtimeSinceStartup + attackCooldown;
		}
	}

	public void Startle()
	{
		lastStartleTime = Time.realtimeSinceStartup;
	}

	public bool IsStartled()
	{
		return lastStartleTime + startleDuration > Time.realtimeSinceStartup;
	}

	private float GetDesiredSpeed()
	{
		return IsStartled() ? maxSpeed : minSpeed;
	}

	public float GetTurnSpeed()
	{
		if (IsStartled())
		{
			return maxTurnSpeed;
		}
		if (obstacleAvoidanceScale != 0f)
		{
			return Mathf.Lerp(minTurnSpeed, maxTurnSpeed, obstacleAvoidanceScale);
		}
		return minTurnSpeed;
	}

	private float GetCurrentSpeed()
	{
		return currentSpeed;
	}

	private void UpdateObstacleAvoidance(float delta)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		timeSinceLastObstacleCheck += delta;
		if (timeSinceLastObstacleCheck < 0.5f)
		{
			return;
		}
		Profiler.BeginSample("UpdateObstacleAvoidance");
		Vector3 forward = ((Component)this).transform.forward;
		Vector3 position = ((Component)this).transform.position;
		int num = 1503764737;
		RaycastHit val = default(RaycastHit);
		if (Physics.SphereCast(position, obstacleDetectionRadius, forward, ref val, obstacleDetectionRange, num))
		{
			Vector3 point = ((RaycastHit)(ref val)).point;
			Vector3 val2 = Vector3.zero;
			Vector3 val3 = Vector3.zero;
			RaycastHit val4 = default(RaycastHit);
			if (Physics.SphereCast(position + Vector3.down * 0.25f + ((Component)this).transform.right * 0.25f, obstacleDetectionRadius, forward, ref val4, obstacleDetectionRange, num))
			{
				val2 = ((RaycastHit)(ref val4)).point;
			}
			RaycastHit val5 = default(RaycastHit);
			if (Physics.SphereCast(position + Vector3.down * 0.25f - ((Component)this).transform.right * 0.25f, obstacleDetectionRadius, forward, ref val5, obstacleDetectionRange, num))
			{
				val3 = ((RaycastHit)(ref val5)).point;
			}
			if (val2 != Vector3.zero && val3 != Vector3.zero)
			{
				Plane val6 = default(Plane);
				((Plane)(ref val6))._002Ector(point, val2, val3);
				Vector3 normal = ((Plane)(ref val6)).normal;
				if (normal != Vector3.zero)
				{
					((RaycastHit)(ref val)).normal = normal;
				}
			}
			cachedObstacleNormal = ((RaycastHit)(ref val)).normal;
			cachedObstacleDistance = ((RaycastHit)(ref val)).distance;
			obstacleAvoidanceScale = 1f - Mathf.InverseLerp(2f, obstacleDetectionRange * 0.75f, ((RaycastHit)(ref val)).distance);
		}
		else
		{
			obstacleAvoidanceScale = Mathf.MoveTowards(obstacleAvoidanceScale, 0f, timeSinceLastObstacleCheck * 2f);
			if (obstacleAvoidanceScale == 0f)
			{
				cachedObstacleDistance = 0f;
			}
		}
		timeSinceLastObstacleCheck = 0f;
		Profiler.EndSample();
	}

	private void UpdateDirection(float delta)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		Profiler.BeginSample("UpdateDirection");
		Vector3 forward = ((Component)this).transform.forward;
		Vector3 val = WaterClamp(destination);
		Vector3 val2 = Vector3Ex.Direction(val, ((Component)this).transform.position);
		if (obstacleAvoidanceScale != 0f)
		{
			Vector3 val4;
			if (cachedObstacleNormal != Vector3.zero)
			{
				Vector3 val3 = QuaternionEx.LookRotationForcedUp(cachedObstacleNormal, Vector3.up) * Vector3.forward;
				val4 = ((!(Vector3.Dot(val3, ((Component)this).transform.right) > Vector3.Dot(val3, -((Component)this).transform.right))) ? (-((Component)this).transform.right) : ((Component)this).transform.right);
			}
			else
			{
				val4 = ((Component)this).transform.right;
			}
			val2 = val4 * obstacleAvoidanceScale;
			((Vector3)(ref val2)).Normalize();
		}
		if (val2 != Vector3.zero)
		{
			Quaternion val5 = Quaternion.LookRotation(val2, Vector3.up);
			((Component)this).transform.rotation = Quaternion.Lerp(((Component)this).transform.rotation, val5, delta * GetTurnSpeed());
		}
		Profiler.EndSample();
	}

	private void UpdatePosition(float delta)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		Profiler.BeginSample("UpdatePosition");
		Vector3 forward = ((Component)this).transform.forward;
		Vector3 point = ((Component)this).transform.position + forward * GetCurrentSpeed() * delta;
		point = WaterClamp(point);
		((Component)this).transform.position = point;
		Profiler.EndSample();
	}

	private void UpdateSpeed(float delta)
	{
		Profiler.BeginSample("UpdateSpeed");
		currentSpeed = Mathf.Lerp(currentSpeed, GetDesiredSpeed(), delta * 4f);
		Profiler.EndSample();
	}

	public void Update()
	{
		if (base.isServer)
		{
			Think(Time.deltaTime);
		}
	}
}
