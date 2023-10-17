using System.Collections.Generic;
using UnityEngine;

public class FishBrain : BaseAIBrain
{
	public class IdleState : BaseIdleState
	{
		private StateStatus status = StateStatus.Error;

		private List<Vector3> idlePoints;

		private int currentPointIndex;

		private Vector3 idleRootPos;

		public override void StateLeave(BaseAIBrain brain, BaseEntity entity)
		{
			base.StateLeave(brain, entity);
			Stop();
		}

		public override void StateEnter(BaseAIBrain brain, BaseEntity entity)
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			base.StateEnter(brain, entity);
			idleRootPos = ((Component)brain.Navigator).transform.position;
			GenerateIdlePoints(20f, 0f);
			currentPointIndex = 0;
			status = StateStatus.Error;
			if (brain.PathFinder != null)
			{
				if (brain.Navigator.SetDestination(idleRootPos + idlePoints[0], BaseNavigator.NavigationSpeed.Normal))
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
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0093: Unknown result type (might be due to invalid IL or missing references)
			base.StateThink(delta, brain, entity);
			if (Vector3.Distance(((Component)brain.Navigator).transform.position, idleRootPos + idlePoints[currentPointIndex]) < 4f)
			{
				currentPointIndex++;
			}
			if (currentPointIndex >= idlePoints.Count)
			{
				currentPointIndex = 0;
			}
			if (brain.Navigator.SetDestination(idleRootPos + idlePoints[currentPointIndex], BaseNavigator.NavigationSpeed.Normal))
			{
				status = StateStatus.Running;
			}
			else
			{
				status = StateStatus.Error;
			}
			return status;
		}

		private void GenerateIdlePoints(float radius, float heightOffset)
		{
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_005e: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			if (idlePoints == null)
			{
				idlePoints = new List<Vector3>();
				float num = 0f;
				int num2 = 32;
				float height = TerrainMeta.WaterMap.GetHeight(((Component)brain.Navigator).transform.position);
				float height2 = TerrainMeta.HeightMap.GetHeight(((Component)brain.Navigator).transform.position);
				for (int i = 0; i < num2; i++)
				{
					num += 360f / (float)num2;
					Vector3 pointOnCircle = BasePathFinder.GetPointOnCircle(Vector3.zero, radius, num);
					pointOnCircle.y += Random.Range(0f - heightOffset, heightOffset);
					pointOnCircle.y = Mathf.Clamp(pointOnCircle.y, height2, height);
					idlePoints.Add(pointOnCircle);
				}
			}
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
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			base.StateEnter(brain, entity);
			status = StateStatus.Error;
			if (brain.PathFinder != null)
			{
				Vector3 fallbackPos = brain.Events.Memory.Position.Get(4);
				Vector3 bestRoamPosition = brain.PathFinder.GetBestRoamPosition(brain.Navigator, fallbackPos, 5f, brain.Navigator.MaxRoamDistanceFromHome);
				if (brain.Navigator.SetDestination(bestRoamPosition, BaseNavigator.NavigationSpeed.Normal))
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

	public static int Count;

	public override void AddStates()
	{
		base.AddStates();
		AddState(new IdleState());
		AddState(new RoamState());
		AddState(new BaseFleeState());
		AddState(new BaseChaseState());
		AddState(new BaseMoveTorwardsState());
		AddState(new BaseAttackState());
		AddState(new BaseCooldownState());
	}

	public override void InitializeAI()
	{
		base.InitializeAI();
		base.ThinkMode = AIThinkMode.Interval;
		thinkRate = 0.25f;
		base.PathFinder = new UnderwaterPathFinder();
		((UnderwaterPathFinder)base.PathFinder).Init(GetBaseEntity());
		Count++;
	}

	public override void OnDestroy()
	{
		base.OnDestroy();
		Count--;
	}
}
