using UnityEngine;
using UnityEngine.Profiling;

public class ScientistBrain : BaseAIBrain
{
	public class BlindedState : BaseBlindedState
	{
		public override void StateEnter(BaseAIBrain brain, BaseEntity entity)
		{
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			base.StateEnter(brain, entity);
			HumanNPC humanNPC = entity as HumanNPC;
			humanNPC.SetDucked(flag: false);
			humanNPC.Server_StartGesture(235662700u);
			brain.Navigator.SetDestination(brain.PathFinder.GetRandomPositionAround(((Component)entity).transform.position, 1f, 2.5f), BaseNavigator.NavigationSpeed.Slowest);
		}

		public override void StateLeave(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateLeave(brain, entity);
			brain.Navigator.ClearFacingDirectionOverride();
			if ((Object)(object)entity.ToPlayer() != (Object)null)
			{
				entity.ToPlayer().Server_CancelGesture();
			}
		}
	}

	public class ChaseState : BasicAIState
	{
		private StateStatus status = StateStatus.Error;

		private float nextPositionUpdateTime = 0f;

		public ChaseState()
			: base(AIState.Chase)
		{
			base.AgrresiveState = true;
		}

		public override void StateLeave(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateLeave(brain, entity);
			Stop();
		}

		public override void StateEnter(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateEnter(brain, entity);
			status = StateStatus.Error;
			if (brain.PathFinder != null)
			{
				status = StateStatus.Running;
				nextPositionUpdateTime = 0f;
			}
		}

		private void Stop()
		{
			brain.Navigator.Stop();
			brain.Navigator.ClearFacingDirectionOverride();
		}

		public override StateStatus StateThink(float delta, BaseAIBrain brain, BaseEntity entity)
		{
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0143: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Unknown result type (might be due to invalid IL or missing references)
			//IL_0156: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0187: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0237: Unknown result type (might be due to invalid IL or missing references)
			//IL_021c: Unknown result type (might be due to invalid IL or missing references)
			base.StateThink(delta, brain, entity);
			if (status == StateStatus.Error)
			{
				return status;
			}
			Profiler.BeginSample("ScientistBrain.States.Chase.StateThink");
			BaseEntity baseEntity = brain.Events.Memory.Entity.Get(brain.Events.CurrentInputMemorySlot);
			if ((Object)(object)baseEntity == (Object)null)
			{
				Profiler.EndSample();
				return StateStatus.Error;
			}
			float num = Vector3.Distance(((Component)baseEntity).transform.position, ((Component)entity).transform.position);
			if (brain.Senses.Memory.IsLOS(baseEntity) || num <= 10f || base.TimeInState <= 5f)
			{
				brain.Navigator.SetFacingDirectionEntity(baseEntity);
			}
			else
			{
				brain.Navigator.ClearFacingDirectionOverride();
			}
			if (num <= 10f)
			{
				brain.Navigator.SetCurrentSpeed(BaseNavigator.NavigationSpeed.Normal);
			}
			else
			{
				brain.Navigator.SetCurrentSpeed(BaseNavigator.NavigationSpeed.Fast);
			}
			if (Time.time > nextPositionUpdateTime)
			{
				nextPositionUpdateTime = Time.time + Random.Range(0.5f, 1f);
				Vector3 pos = ((Component)entity).transform.position;
				AIInformationZone informationZone = (entity as HumanNPC).GetInformationZone(((Component)baseEntity).transform.position);
				bool flag = false;
				if ((Object)(object)informationZone != (Object)null)
				{
					AIMovePoint bestMovePointNear = informationZone.GetBestMovePointNear(((Component)baseEntity).transform.position, ((Component)entity).transform.position, 0f, brain.Navigator.BestMovementPointMaxDistance, checkLOS: true, entity, returnClosest: true);
					if (Object.op_Implicit((Object)(object)bestMovePointNear))
					{
						bestMovePointNear.SetUsedBy(entity, 5f);
						pos = brain.PathFinder.GetRandomPositionAround(((Component)bestMovePointNear).transform.position, 0f, bestMovePointNear.radius - 0.3f);
						flag = true;
					}
				}
				if (!flag)
				{
					Profiler.EndSample();
					return StateStatus.Error;
				}
				if (num < 10f)
				{
					brain.Navigator.SetDestination(pos, BaseNavigator.NavigationSpeed.Normal);
				}
				else
				{
					brain.Navigator.SetDestination(pos, BaseNavigator.NavigationSpeed.Fast);
				}
			}
			Profiler.EndSample();
			if (brain.Navigator.Moving)
			{
				return StateStatus.Running;
			}
			return StateStatus.Finished;
		}
	}

	public class CombatState : BasicAIState
	{
		private float nextActionTime = 0f;

		private Vector3 combatStartPosition;

		public CombatState()
			: base(AIState.Combat)
		{
			base.AgrresiveState = true;
		}

		public override void StateEnter(BaseAIBrain brain, BaseEntity entity)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			base.StateEnter(brain, entity);
			combatStartPosition = ((Component)entity).transform.position;
			FaceTarget();
		}

		public override void StateLeave(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateLeave(brain, entity);
			(entity as HumanNPC).SetDucked(flag: false);
			brain.Navigator.ClearFacingDirectionOverride();
		}

		public override StateStatus StateThink(float delta, BaseAIBrain brain, BaseEntity entity)
		{
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			base.StateThink(delta, brain, entity);
			Profiler.BeginSample("ScientistBrain.States.Combat.StateThink");
			HumanNPC humanNPC = entity as HumanNPC;
			FaceTarget();
			if (Time.time > nextActionTime)
			{
				if (Random.Range(0, 3) == 1)
				{
					nextActionTime = Time.time + Random.Range(1f, 2f);
					humanNPC.SetDucked(flag: true);
					brain.Navigator.Stop();
				}
				else
				{
					nextActionTime = Time.time + Random.Range(2f, 3f);
					humanNPC.SetDucked(flag: false);
					brain.Navigator.SetDestination(brain.PathFinder.GetRandomPositionAround(combatStartPosition, 1f), BaseNavigator.NavigationSpeed.Normal);
				}
			}
			Profiler.EndSample();
			return StateStatus.Running;
		}

		private void FaceTarget()
		{
			BaseEntity baseEntity = brain.Events.Memory.Entity.Get(brain.Events.CurrentInputMemorySlot);
			if ((Object)(object)baseEntity == (Object)null)
			{
				brain.Navigator.ClearFacingDirectionOverride();
			}
			else
			{
				brain.Navigator.SetFacingDirectionEntity(baseEntity);
			}
		}
	}

	public class CombatStationaryState : BasicAIState
	{
		public CombatStationaryState()
			: base(AIState.CombatStationary)
		{
			base.AgrresiveState = true;
		}

		public override void StateLeave(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateLeave(brain, entity);
			brain.Navigator.ClearFacingDirectionOverride();
		}

		public override StateStatus StateThink(float delta, BaseAIBrain brain, BaseEntity entity)
		{
			base.StateThink(delta, brain, entity);
			Profiler.BeginSample("ScientistBrain.States.CombatStationary.StateThink");
			BaseEntity baseEntity = brain.Events.Memory.Entity.Get(brain.Events.CurrentInputMemorySlot);
			if ((Object)(object)baseEntity != (Object)null)
			{
				brain.Navigator.SetFacingDirectionEntity(baseEntity);
			}
			else
			{
				brain.Navigator.ClearFacingDirectionOverride();
			}
			Profiler.EndSample();
			return StateStatus.Running;
		}
	}

	public class CoverState : BasicAIState
	{
		public CoverState()
			: base(AIState.Cover)
		{
		}

		public override void StateEnter(BaseAIBrain brain, BaseEntity entity)
		{
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00de: Unknown result type (might be due to invalid IL or missing references)
			base.StateEnter(brain, entity);
			HumanNPC humanNPC = entity as HumanNPC;
			humanNPC.SetDucked(flag: true);
			AIPoint aIPoint = brain.Events.Memory.AIPoint.Get(4);
			if ((Object)(object)aIPoint != (Object)null)
			{
				aIPoint.SetUsedBy(entity);
			}
			if (!(humanNPC.healthFraction <= brain.HealBelowHealthFraction) || !(Random.Range(0f, 1f) <= brain.HealChance))
			{
				return;
			}
			Item item = humanNPC.FindHealingItem();
			if (item != null)
			{
				BaseEntity baseEntity = brain.Events.Memory.Entity.Get(brain.Events.CurrentInputMemorySlot);
				if ((Object)(object)baseEntity == (Object)null || (!brain.Senses.Memory.IsLOS(baseEntity) && Vector3.Distance(((Component)entity).transform.position, ((Component)baseEntity).transform.position) >= 5f))
				{
					humanNPC.UseHealingItem(item);
				}
			}
		}

		public override void StateLeave(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateLeave(brain, entity);
			(entity as HumanNPC).SetDucked(flag: false);
			brain.Navigator.ClearFacingDirectionOverride();
			AIPoint aIPoint = brain.Events.Memory.AIPoint.Get(4);
			if ((Object)(object)aIPoint != (Object)null)
			{
				aIPoint.ClearIfUsedBy(entity);
			}
		}

		public override StateStatus StateThink(float delta, BaseAIBrain brain, BaseEntity entity)
		{
			base.StateThink(delta, brain, entity);
			Profiler.BeginSample("ScientistBrain.States.Cover.StateThink");
			HumanNPC humanNPC = entity as HumanNPC;
			BaseEntity baseEntity = brain.Events.Memory.Entity.Get(brain.Events.CurrentInputMemorySlot);
			float num = humanNPC.AmmoFractionRemaining();
			if (num == 0f || ((Object)(object)baseEntity != (Object)null && !brain.Senses.Memory.IsLOS(baseEntity) && num < 0.25f))
			{
				humanNPC.AttemptReload();
			}
			if ((Object)(object)baseEntity != (Object)null)
			{
				brain.Navigator.SetFacingDirectionEntity(baseEntity);
			}
			Profiler.EndSample();
			return StateStatus.Running;
		}
	}

	public class DismountedState : BaseDismountedState
	{
		private StateStatus status = StateStatus.Error;

		public override void StateEnter(BaseAIBrain brain, BaseEntity entity)
		{
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
			base.StateEnter(brain, entity);
			status = StateStatus.Error;
			if (brain.PathFinder == null)
			{
				return;
			}
			AIInformationZone informationZone = (entity as HumanNPC).GetInformationZone(((Component)entity).transform.position);
			if (!((Object)(object)informationZone == (Object)null))
			{
				Profiler.BeginSample("ScientistBrain.States.Dismounted.StateEnter");
				AICoverPoint bestCoverPoint = informationZone.GetBestCoverPoint(((Component)entity).transform.position, ((Component)entity).transform.position, 25f, 50f, entity);
				if (Object.op_Implicit((Object)(object)bestCoverPoint))
				{
					bestCoverPoint.SetUsedBy(entity, 10f);
				}
				Vector3 pos = (((Object)(object)bestCoverPoint == (Object)null) ? ((Component)entity).transform.position : ((Component)bestCoverPoint).transform.position);
				if (brain.Navigator.SetDestination(pos, BaseNavigator.NavigationSpeed.Fast))
				{
					status = StateStatus.Running;
				}
				Profiler.EndSample();
			}
		}

		public override StateStatus StateThink(float delta, BaseAIBrain brain, BaseEntity entity)
		{
			base.StateThink(delta, brain, entity);
			if (status == StateStatus.Error)
			{
				return status;
			}
			if (brain.Navigator.Moving)
			{
				return StateStatus.Running;
			}
			return StateStatus.Finished;
		}
	}

	public class IdleState : BaseIdleState
	{
	}

	public class MountedState : BaseMountedState
	{
	}

	public class MoveToVector3State : BasicAIState
	{
		public MoveToVector3State()
			: base(AIState.MoveToVector3)
		{
		}

		public override void StateLeave(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateLeave(brain, entity);
			Stop();
		}

		private void Stop()
		{
			brain.Navigator.Stop();
			brain.Navigator.ClearFacingDirectionOverride();
		}

		public override StateStatus StateThink(float delta, BaseAIBrain brain, BaseEntity entity)
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			base.StateThink(delta, brain, entity);
			Vector3 pos = brain.Events.Memory.Position.Get(7);
			if (!brain.Navigator.SetDestination(pos, BaseNavigator.NavigationSpeed.Fast, 0.5f))
			{
				return StateStatus.Error;
			}
			return (!brain.Navigator.Moving) ? StateStatus.Finished : StateStatus.Running;
		}
	}

	public class RoamState : BaseRoamState
	{
		private StateStatus status = StateStatus.Error;

		private AIMovePoint roamPoint;

		public override void StateLeave(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateLeave(brain, entity);
			Stop();
			ClearRoamPointUsage(entity);
		}

		public override void StateEnter(BaseAIBrain brain, BaseEntity entity)
		{
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			Profiler.BeginSample("ScientistBrain.States.Roam.StatEnter");
			base.StateEnter(brain, entity);
			status = StateStatus.Error;
			ClearRoamPointUsage(entity);
			if (brain.PathFinder == null)
			{
				Profiler.EndSample();
				return;
			}
			status = StateStatus.Error;
			roamPoint = brain.PathFinder.GetBestRoamPoint(GetRoamAnchorPosition(), ((Component)entity).transform.position, (entity as HumanNPC).eyes.BodyForward(), brain.Navigator.MaxRoamDistanceFromHome, brain.Navigator.BestRoamPointMaxDistance);
			if ((Object)(object)roamPoint != (Object)null)
			{
				if (brain.Navigator.SetDestination(((Component)roamPoint).transform.position, BaseNavigator.NavigationSpeed.Slow))
				{
					roamPoint.SetUsedBy(entity);
					status = StateStatus.Running;
				}
				else
				{
					roamPoint.SetUsedBy(entity, 600f);
				}
			}
			Profiler.EndSample();
		}

		private void ClearRoamPointUsage(BaseEntity entity)
		{
			if ((Object)(object)roamPoint != (Object)null)
			{
				roamPoint.ClearIfUsedBy(entity);
				roamPoint = null;
			}
		}

		private void Stop()
		{
			brain.Navigator.Stop();
		}

		public override StateStatus StateThink(float delta, BaseAIBrain brain, BaseEntity entity)
		{
			if (status == StateStatus.Error)
			{
				return status;
			}
			if (brain.Navigator.Moving)
			{
				return StateStatus.Running;
			}
			PickGoodLookDirection();
			return StateStatus.Finished;
		}

		private void PickGoodLookDirection()
		{
		}
	}

	public class TakeCoverState : BasicAIState
	{
		private StateStatus status = StateStatus.Error;

		private BaseEntity coverFromEntity;

		public TakeCoverState()
			: base(AIState.TakeCover)
		{
		}

		public override void StateEnter(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateEnter(brain, entity);
			status = StateStatus.Running;
			if (!StartMovingToCover(entity as HumanNPC))
			{
				status = StateStatus.Error;
			}
		}

		public override void StateLeave(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateLeave(brain, entity);
			brain.Navigator.ClearFacingDirectionOverride();
			ClearCoverPointUsage(entity);
		}

		private void ClearCoverPointUsage(BaseEntity entity)
		{
			AIPoint aIPoint = brain.Events.Memory.AIPoint.Get(4);
			if ((Object)(object)aIPoint != (Object)null)
			{
				aIPoint.ClearIfUsedBy(entity);
			}
		}

		private bool StartMovingToCover(HumanNPC entity)
		{
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_00df: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_0112: Unknown result type (might be due to invalid IL or missing references)
			//IL_0117: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			coverFromEntity = brain.Events.Memory.Entity.Get(brain.Events.CurrentInputMemorySlot);
			Profiler.BeginSample("ScientistBrain.States.TakeCover.StartMovingToCover");
			Vector3 hideFromPosition = (Object.op_Implicit((Object)(object)coverFromEntity) ? ((Component)coverFromEntity).transform.position : (((Component)entity).transform.position + entity.LastAttackedDir * 30f));
			AIInformationZone informationZone = entity.GetInformationZone(((Component)entity).transform.position);
			if ((Object)(object)informationZone == (Object)null)
			{
				Profiler.EndSample();
				return false;
			}
			float secondsSinceAttacked = entity.SecondsSinceAttacked;
			float minRange = ((secondsSinceAttacked < 2f) ? 2f : 0f);
			float bestCoverPointMaxDistance = brain.Navigator.BestCoverPointMaxDistance;
			AICoverPoint bestCoverPoint = informationZone.GetBestCoverPoint(((Component)entity).transform.position, hideFromPosition, minRange, bestCoverPointMaxDistance, entity);
			if ((Object)(object)bestCoverPoint == (Object)null)
			{
				Profiler.EndSample();
				return false;
			}
			Vector3 position = ((Component)bestCoverPoint).transform.position;
			if (!brain.Navigator.SetDestination(position, BaseNavigator.NavigationSpeed.Normal))
			{
				Profiler.EndSample();
				return false;
			}
			FaceCoverFromEntity();
			brain.Events.Memory.AIPoint.Set(bestCoverPoint, 4);
			bestCoverPoint.SetUsedBy(entity);
			Profiler.EndSample();
			return true;
		}

		public override void DrawGizmos()
		{
			base.DrawGizmos();
		}

		public override StateStatus StateThink(float delta, BaseAIBrain brain, BaseEntity entity)
		{
			base.StateThink(delta, brain, entity);
			FaceCoverFromEntity();
			if (status == StateStatus.Error)
			{
				return status;
			}
			if (brain.Navigator.Moving)
			{
				return StateStatus.Running;
			}
			return StateStatus.Finished;
		}

		private void FaceCoverFromEntity()
		{
			coverFromEntity = brain.Events.Memory.Entity.Get(brain.Events.CurrentInputMemorySlot);
			if (!((Object)(object)coverFromEntity == (Object)null))
			{
				brain.Navigator.SetFacingDirectionEntity(coverFromEntity);
			}
		}
	}

	public static int Count;

	public override void AddStates()
	{
		base.AddStates();
		AddState(new IdleState());
		AddState(new RoamState());
		AddState(new ChaseState());
		AddState(new CombatState());
		AddState(new TakeCoverState());
		AddState(new CoverState());
		AddState(new MountedState());
		AddState(new DismountedState());
		AddState(new BaseFollowPathState());
		AddState(new BaseNavigateHomeState());
		AddState(new CombatStationaryState());
		AddState(new BaseMoveTorwardsState());
		AddState(new MoveToVector3State());
		AddState(new BlindedState());
	}

	public override void InitializeAI()
	{
		base.InitializeAI();
		base.ThinkMode = AIThinkMode.Interval;
		thinkRate = 0.25f;
		base.PathFinder = new HumanPathFinder();
		((HumanPathFinder)base.PathFinder).Init(GetBaseEntity());
		Count++;
	}

	public override void OnDestroy()
	{
		base.OnDestroy();
		Count--;
	}

	public HumanNPC GetEntity()
	{
		return GetBaseEntity() as HumanNPC;
	}

	protected override void OnStateChanged()
	{
		base.OnStateChanged();
		if (base.CurrentState != null)
		{
			switch (base.CurrentState.StateType)
			{
			case AIState.Idle:
			case AIState.Roam:
			case AIState.Patrol:
			case AIState.FollowPath:
			case AIState.Cooldown:
				GetEntity().SetPlayerFlag(BasePlayer.PlayerFlags.Relaxed, b: true);
				break;
			default:
				GetEntity().SetPlayerFlag(BasePlayer.PlayerFlags.Relaxed, b: false);
				break;
			}
		}
	}
}
