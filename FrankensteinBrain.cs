using UnityEngine;

public class FrankensteinBrain : PetBrain
{
	public class MoveToPointState : BasicAIState
	{
		private float originalStopDistance;

		public MoveToPointState()
			: base(AIState.MoveToPoint)
		{
		}

		public override void StateEnter(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateEnter(brain, entity);
			BaseNavigator navigator = brain.Navigator;
			originalStopDistance = navigator.StoppingDistance;
			navigator.StoppingDistance = 0.5f;
		}

		public override void StateLeave(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateLeave(brain, entity);
			BaseNavigator navigator = brain.Navigator;
			navigator.StoppingDistance = originalStopDistance;
			Stop();
		}

		private void Stop()
		{
			brain.Navigator.Stop();
		}

		public override StateStatus StateThink(float delta, BaseAIBrain brain, BaseEntity entity)
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			base.StateThink(delta, brain, entity);
			Vector3 pos = brain.Events.Memory.Position.Get(6);
			if (!brain.Navigator.SetDestination(pos, BaseNavigator.NavigationSpeed.Normal, MoveTowardsRate))
			{
				return StateStatus.Error;
			}
			if (!brain.Navigator.Moving)
			{
				brain.LoadDefaultAIDesign();
			}
			return (!brain.Navigator.Moving) ? StateStatus.Finished : StateStatus.Running;
		}
	}

	public class MoveTorwardsState : BasicAIState
	{
		public MoveTorwardsState()
			: base(AIState.MoveTowards)
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
		}

		public override StateStatus StateThink(float delta, BaseAIBrain brain, BaseEntity entity)
		{
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			base.StateThink(delta, brain, entity);
			BaseEntity baseEntity = brain.Events.Memory.Entity.Get(brain.Events.CurrentInputMemorySlot);
			if ((Object)(object)baseEntity == (Object)null)
			{
				Stop();
				return StateStatus.Error;
			}
			if (!brain.Navigator.SetDestination(((Component)baseEntity).transform.position, BaseNavigator.NavigationSpeed.Normal, MoveTowardsRate))
			{
				return StateStatus.Error;
			}
			return (!brain.Navigator.Moving) ? StateStatus.Finished : StateStatus.Running;
		}
	}

	[ServerVar]
	public static float MoveTowardsRate = 1f;

	public override void AddStates()
	{
		base.AddStates();
		AddState(new BaseIdleState());
		AddState(new MoveTorwardsState());
		AddState(new BaseChaseState());
		AddState(new BaseAttackState());
		AddState(new MoveToPointState());
	}

	public override void InitializeAI()
	{
		base.InitializeAI();
		base.ThinkMode = AIThinkMode.Interval;
		thinkRate = 0.25f;
		base.PathFinder = new HumanPathFinder();
		((HumanPathFinder)base.PathFinder).Init(GetBaseEntity());
	}

	public FrankensteinPet GetEntity()
	{
		return GetBaseEntity() as FrankensteinPet;
	}

	public override void OnDestroy()
	{
		base.OnDestroy();
	}
}
