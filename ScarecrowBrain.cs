using ConVar;
using UnityEngine;
using UnityEngine.Profiling;

public class ScarecrowBrain : BaseAIBrain
{
	public class AttackState : BasicAIState
	{
		private IAIAttack attack;

		private float originalStoppingDistance;

		public AttackState()
			: base(AIState.Attack)
		{
			base.AgrresiveState = true;
		}

		public override void StateEnter(BaseAIBrain brain, BaseEntity entity)
		{
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			entity.SetFlag(BaseEntity.Flags.Reserved3, b: true);
			originalStoppingDistance = brain.Navigator.StoppingDistance;
			brain.Navigator.Agent.stoppingDistance = 1f;
			brain.Navigator.StoppingDistance = 1f;
			base.StateEnter(brain, entity);
			attack = entity as IAIAttack;
			BaseEntity baseEntity = brain.Events.Memory.Entity.Get(brain.Events.CurrentInputMemorySlot);
			if ((Object)(object)baseEntity != (Object)null)
			{
				Vector3 aimDirection = GetAimDirection(((Component)brain.Navigator).transform.position, ((Component)baseEntity).transform.position);
				brain.Navigator.SetFacingDirectionOverride(aimDirection);
				if (attack.CanAttack(baseEntity))
				{
					StartAttacking(baseEntity);
				}
				brain.Navigator.SetDestination(((Component)baseEntity).transform.position, BaseNavigator.NavigationSpeed.Fast);
			}
		}

		public override void StateLeave(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateLeave(brain, entity);
			entity.SetFlag(BaseEntity.Flags.Reserved3, b: false);
			brain.Navigator.Agent.stoppingDistance = originalStoppingDistance;
			brain.Navigator.StoppingDistance = originalStoppingDistance;
			brain.Navigator.ClearFacingDirectionOverride();
			brain.Navigator.Stop();
			StopAttacking();
		}

		private void StopAttacking()
		{
			attack.StopAttacking();
		}

		public override StateStatus StateThink(float delta, BaseAIBrain brain, BaseEntity entity)
		{
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0126: Unknown result type (might be due to invalid IL or missing references)
			//IL_012b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_0137: Unknown result type (might be due to invalid IL or missing references)
			base.StateThink(delta, brain, entity);
			Profiler.BeginSample("BaseAIBrain.States.BaseAttack.StateThink");
			BaseEntity baseEntity = brain.Events.Memory.Entity.Get(brain.Events.CurrentInputMemorySlot);
			if (attack == null)
			{
				Profiler.EndSample();
				return StateStatus.Error;
			}
			if ((Object)(object)baseEntity == (Object)null)
			{
				brain.Navigator.ClearFacingDirectionOverride();
				StopAttacking();
				Profiler.EndSample();
				return StateStatus.Finished;
			}
			if (brain.Senses.ignoreSafeZonePlayers)
			{
				BasePlayer basePlayer = baseEntity as BasePlayer;
				if ((Object)(object)basePlayer != (Object)null && basePlayer.InSafeZone())
				{
					return StateStatus.Error;
				}
			}
			Vector3 val = Vector3Ex.Direction2D(((Component)baseEntity).transform.position, ((Component)entity).transform.position);
			Vector3 position = ((Component)baseEntity).transform.position;
			if (!brain.Navigator.SetDestination(position, BaseNavigator.NavigationSpeed.Fast, 0.2f))
			{
				return StateStatus.Error;
			}
			Vector3 aimDirection = GetAimDirection(((Component)brain.Navigator).transform.position, ((Component)baseEntity).transform.position);
			brain.Navigator.SetFacingDirectionOverride(aimDirection);
			if (attack.CanAttack(baseEntity))
			{
				StartAttacking(baseEntity);
			}
			else
			{
				StopAttacking();
			}
			Profiler.EndSample();
			return StateStatus.Running;
		}

		private static Vector3 GetAimDirection(Vector3 from, Vector3 target)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			return Vector3Ex.Direction2D(target, from);
		}

		private void StartAttacking(BaseEntity entity)
		{
			attack.StartAttacking(entity);
		}
	}

	public class ChaseState : BasicAIState
	{
		private float throwDelayTime;

		private bool useBeanCan = false;

		public ChaseState()
			: base(AIState.Chase)
		{
			base.AgrresiveState = true;
		}

		public override void StateEnter(BaseAIBrain brain, BaseEntity entity)
		{
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			base.StateEnter(brain, entity);
			entity.SetFlag(BaseEntity.Flags.Reserved3, b: true);
			throwDelayTime = Time.time + Random.Range(0.2f, 0.5f);
			useBeanCan = (float)Random.Range(0, 100) <= 20f;
			BaseEntity baseEntity = brain.Events.Memory.Entity.Get(brain.Events.CurrentInputMemorySlot);
			if ((Object)(object)baseEntity != (Object)null)
			{
				brain.Navigator.SetDestination(((Component)baseEntity).transform.position, BaseNavigator.NavigationSpeed.Fast);
			}
		}

		public override void StateLeave(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateLeave(brain, entity);
			entity.SetFlag(BaseEntity.Flags.Reserved3, b: false);
			Stop();
		}

		private void Stop()
		{
			brain.Navigator.Stop();
		}

		public override StateStatus StateThink(float delta, BaseAIBrain brain, BaseEntity entity)
		{
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			base.StateThink(delta, brain, entity);
			BaseEntity baseEntity = brain.Events.Memory.Entity.Get(brain.Events.CurrentInputMemorySlot);
			if ((Object)(object)baseEntity == (Object)null)
			{
				Stop();
				return StateStatus.Error;
			}
			if (useBeanCan && Time.time >= throwDelayTime && AI.npc_use_thrown_weapons && Halloween.scarecrows_throw_beancans && Time.time >= ScarecrowNPC.NextBeanCanAllowedTime)
			{
				ScarecrowNPC scarecrowNPC = brain.GetBrainBaseEntity() as ScarecrowNPC;
				if (scarecrowNPC.TryUseThrownWeapon(baseEntity, 10f))
				{
					brain.Navigator.Stop();
					return StateStatus.Running;
				}
			}
			if (!(brain.GetBrainBaseEntity() as BasePlayer).modelState.aiming)
			{
				if (!brain.Navigator.SetDestination(((Component)baseEntity).transform.position, BaseNavigator.NavigationSpeed.Fast, 0.25f))
				{
					return StateStatus.Error;
				}
				return (!brain.Navigator.Moving) ? StateStatus.Finished : StateStatus.Running;
			}
			return StateStatus.Running;
		}
	}

	public class RoamState : BasicAIState
	{
		private StateStatus status = StateStatus.Error;

		public RoamState()
			: base(AIState.Roam)
		{
		}

		public override void StateLeave(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateLeave(brain, entity);
			Stop();
		}

		public override void StateEnter(BaseAIBrain brain, BaseEntity entity)
		{
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			base.StateEnter(brain, entity);
			status = StateStatus.Error;
			if (brain.PathFinder == null)
			{
				return;
			}
			ScarecrowNPC scarecrowNPC = entity as ScarecrowNPC;
			if (!((Object)(object)scarecrowNPC == (Object)null))
			{
				Vector3 val = brain.Events.Memory.Position.Get(4);
				Vector3 val2 = val;
				val2 = ((!scarecrowNPC.RoamAroundHomePoint) ? brain.PathFinder.GetBestRoamPosition(brain.Navigator, brain.Events.Memory.Position.Get(4), 10f, brain.Navigator.BestRoamPointMaxDistance) : brain.PathFinder.GetBestRoamPositionFromAnchor(brain.Navigator, val, val, 1f, brain.Navigator.BestRoamPointMaxDistance));
				if (brain.Navigator.SetDestination(val2, BaseNavigator.NavigationSpeed.Slow))
				{
					status = StateStatus.Running;
				}
				else
				{
					status = StateStatus.Error;
				}
			}
		}

		private void Stop()
		{
			brain.Navigator.Stop();
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

	public override void AddStates()
	{
		base.AddStates();
		AddState(new BaseIdleState());
		AddState(new ChaseState());
		AddState(new AttackState());
		AddState(new RoamState());
		AddState(new BaseFleeState());
	}

	public override void InitializeAI()
	{
		base.InitializeAI();
		base.ThinkMode = AIThinkMode.Interval;
		thinkRate = 0.25f;
		base.PathFinder = new HumanPathFinder();
		((HumanPathFinder)base.PathFinder).Init(GetBaseEntity());
	}

	public override void OnDestroy()
	{
		base.OnDestroy();
	}
}
