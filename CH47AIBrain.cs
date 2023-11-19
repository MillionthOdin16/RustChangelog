using UnityEngine;

public class CH47AIBrain : BaseAIBrain
{
	public class DropCrate : BasicAIState
	{
		private float nextDropTime;

		public DropCrate()
			: base(AIState.DropCrate)
		{
		}

		public override bool CanInterrupt()
		{
			if (base.CanInterrupt())
			{
				return !CanDrop();
			}
			return false;
		}

		public bool CanDrop()
		{
			if (Time.time > nextDropTime)
			{
				return (brain.GetBrainBaseEntity() as CH47HelicopterAIController).CanDropCrate();
			}
			return false;
		}

		public override float GetWeight()
		{
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			if (!CanDrop())
			{
				return 0f;
			}
			if (IsInState())
			{
				return 10000f;
			}
			if (brain.CurrentState != null && brain.CurrentState.StateType == AIState.Orbit && brain.CurrentState.TimeInState > 60f)
			{
				CH47DropZone closest = CH47DropZone.GetClosest(brain.mainInterestPoint);
				if (Object.op_Implicit((Object)(object)closest) && Vector3Ex.Distance2D(((Component)closest).transform.position, brain.mainInterestPoint) < 200f)
				{
					CH47AIBrain component = ((Component)brain).GetComponent<CH47AIBrain>();
					if ((Object)(object)component != (Object)null)
					{
						float num = Mathf.InverseLerp(300f, 600f, component.Age);
						return 1000f * num;
					}
				}
			}
			return 0f;
		}

		public override void StateEnter(BaseAIBrain brain, BaseEntity entity)
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			CH47HelicopterAIController obj = entity as CH47HelicopterAIController;
			obj.SetDropDoorOpen(open: true);
			obj.EnableFacingOverride(enabled: false);
			CH47DropZone closest = CH47DropZone.GetClosest(((Component)obj).transform.position);
			if ((Object)(object)closest == (Object)null)
			{
				nextDropTime = Time.time + 60f;
			}
			brain.mainInterestPoint = ((Component)closest).transform.position;
			obj.SetMoveTarget(brain.mainInterestPoint);
			base.StateEnter(brain, entity);
		}

		public override StateStatus StateThink(float delta, BaseAIBrain brain, BaseEntity entity)
		{
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			base.StateThink(delta, brain, entity);
			CH47HelicopterAIController cH47HelicopterAIController = entity as CH47HelicopterAIController;
			if (CanDrop() && Vector3Ex.Distance2D(brain.mainInterestPoint, ((Component)cH47HelicopterAIController).transform.position) < 5f)
			{
				Vector3 velocity = cH47HelicopterAIController.rigidBody.velocity;
				if (((Vector3)(ref velocity)).magnitude < 5f)
				{
					cH47HelicopterAIController.DropCrate();
					nextDropTime = Time.time + 120f;
				}
			}
			return StateStatus.Running;
		}

		public override void StateLeave(BaseAIBrain brain, BaseEntity entity)
		{
			(entity as CH47HelicopterAIController).SetDropDoorOpen(open: false);
			nextDropTime = Time.time + 60f;
			base.StateLeave(brain, entity);
		}
	}

	public class EgressState : BasicAIState
	{
		private bool killing;

		private bool egressAltitueAchieved;

		public EgressState()
			: base(AIState.Egress)
		{
		}

		public override bool CanInterrupt()
		{
			return false;
		}

		public override float GetWeight()
		{
			CH47HelicopterAIController cH47HelicopterAIController = brain.GetBrainBaseEntity() as CH47HelicopterAIController;
			if (cH47HelicopterAIController.OutOfCrates() && !cH47HelicopterAIController.ShouldLand())
			{
				return 10000f;
			}
			CH47AIBrain component = ((Component)brain).GetComponent<CH47AIBrain>();
			if ((Object)(object)component != (Object)null)
			{
				if (!(component.Age > 1800f))
				{
					return 0f;
				}
				return 10000f;
			}
			return 0f;
		}

		public override void StateEnter(BaseAIBrain brain, BaseEntity entity)
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			CH47HelicopterAIController obj = entity as CH47HelicopterAIController;
			obj.EnableFacingOverride(enabled: false);
			Transform transform = ((Component)obj).transform;
			Rigidbody rigidBody = obj.rigidBody;
			Vector3 velocity = rigidBody.velocity;
			Vector3 val;
			if (!(((Vector3)(ref velocity)).magnitude < 0.1f))
			{
				velocity = rigidBody.velocity;
				val = ((Vector3)(ref velocity)).normalized;
			}
			else
			{
				val = transform.forward;
			}
			Vector3 val2 = val;
			Vector3 val3 = Vector3.Cross(Vector3.Cross(transform.up, val2), Vector3.up);
			brain.mainInterestPoint = transform.position + val3 * 8000f;
			brain.mainInterestPoint.y = 100f;
			obj.SetMoveTarget(brain.mainInterestPoint);
			base.StateEnter(brain, entity);
		}

		public override StateStatus StateThink(float delta, BaseAIBrain brain, BaseEntity entity)
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			base.StateThink(delta, brain, entity);
			if (killing)
			{
				return StateStatus.Running;
			}
			CH47HelicopterAIController cH47HelicopterAIController = entity as CH47HelicopterAIController;
			Vector3 position = ((Component)cH47HelicopterAIController).transform.position;
			if (position.y < 85f && !egressAltitueAchieved)
			{
				CH47LandingZone closest = CH47LandingZone.GetClosest(position);
				if ((Object)(object)closest != (Object)null && Vector3Ex.Distance2D(((Component)closest).transform.position, position) < 20f)
				{
					float num = 0f;
					if ((Object)(object)TerrainMeta.HeightMap != (Object)null && (Object)(object)TerrainMeta.WaterMap != (Object)null)
					{
						num = Mathf.Max(TerrainMeta.WaterMap.GetHeight(position), TerrainMeta.HeightMap.GetHeight(position));
					}
					num += 100f;
					Vector3 moveTarget = position;
					moveTarget.y = num;
					cH47HelicopterAIController.SetMoveTarget(moveTarget);
					return StateStatus.Running;
				}
			}
			egressAltitueAchieved = true;
			cH47HelicopterAIController.SetMoveTarget(brain.mainInterestPoint);
			if (base.TimeInState > 300f)
			{
				((MonoBehaviour)cH47HelicopterAIController).Invoke("DelayedKill", 2f);
				killing = true;
			}
			return StateStatus.Running;
		}
	}

	public class IdleState : BaseIdleState
	{
		public override float GetWeight()
		{
			return 0.1f;
		}

		public override void StateEnter(BaseAIBrain brain, BaseEntity entity)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			CH47HelicopterAIController cH47HelicopterAIController = entity as CH47HelicopterAIController;
			Vector3 position = cH47HelicopterAIController.GetPosition();
			Vector3 velocity = cH47HelicopterAIController.rigidBody.velocity;
			cH47HelicopterAIController.SetMoveTarget(position + ((Vector3)(ref velocity)).normalized * 10f);
			base.StateEnter(brain, entity);
		}
	}

	public class LandState : BasicAIState
	{
		private float landedForSeconds;

		private float lastLandtime;

		private float landingHeight = 20f;

		private float nextDismountTime;

		public LandState()
			: base(AIState.Land)
		{
		}

		public override float GetWeight()
		{
			if (!(brain.GetBrainBaseEntity() as CH47HelicopterAIController).ShouldLand())
			{
				return 0f;
			}
			float num = Time.time - lastLandtime;
			if (IsInState() && landedForSeconds < 12f)
			{
				return 1000f;
			}
			if (!IsInState() && num > 10f)
			{
				return 9000f;
			}
			return 0f;
		}

		public override StateStatus StateThink(float delta, BaseAIBrain brain, BaseEntity entity)
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			//IL_0161: Unknown result type (might be due to invalid IL or missing references)
			//IL_016c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			//IL_0186: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01da: Unknown result type (might be due to invalid IL or missing references)
			//IL_01df: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
			base.StateThink(delta, brain, entity);
			CH47HelicopterAIController cH47HelicopterAIController = entity as CH47HelicopterAIController;
			Vector3 position = ((Component)cH47HelicopterAIController).transform.position;
			_ = ((Component)cH47HelicopterAIController).transform.forward;
			CH47LandingZone closest = CH47LandingZone.GetClosest(cH47HelicopterAIController.landingTarget);
			if (!Object.op_Implicit((Object)(object)closest))
			{
				return StateStatus.Error;
			}
			Vector3 velocity = cH47HelicopterAIController.rigidBody.velocity;
			float magnitude = ((Vector3)(ref velocity)).magnitude;
			float num = Vector3Ex.Distance2D(((Component)closest).transform.position, position);
			bool enabled = num < 40f;
			bool altitudeProtection = num > 15f && position.y < ((Component)closest).transform.position.y + 10f;
			cH47HelicopterAIController.EnableFacingOverride(enabled);
			cH47HelicopterAIController.SetAltitudeProtection(altitudeProtection);
			int num2;
			if (Mathf.Abs(((Component)closest).transform.position.y - position.y) < 3f && num <= 5f)
			{
				num2 = ((magnitude < 1f) ? 1 : 0);
				if (num2 != 0)
				{
					landedForSeconds += delta;
					if (lastLandtime == 0f)
					{
						lastLandtime = Time.time;
					}
				}
			}
			else
			{
				num2 = 0;
			}
			float num3 = 1f - Mathf.InverseLerp(0f, 7f, num);
			landingHeight -= 4f * num3 * Time.deltaTime;
			if (landingHeight < -5f)
			{
				landingHeight = -5f;
			}
			cH47HelicopterAIController.SetAimDirection(((Component)closest).transform.forward);
			Vector3 moveTarget = brain.mainInterestPoint + new Vector3(0f, landingHeight, 0f);
			if (num < 100f && num > 15f)
			{
				Vector3 val = Vector3Ex.Direction2D(((Component)closest).transform.position, position);
				RaycastHit val2 = default(RaycastHit);
				if (Physics.SphereCast(position, 15f, val, ref val2, num, 1084293377))
				{
					Vector3 val3 = Vector3.Cross(val, Vector3.up);
					moveTarget = ((RaycastHit)(ref val2)).point + val3 * 50f;
				}
			}
			cH47HelicopterAIController.SetMoveTarget(moveTarget);
			if (num2 != 0)
			{
				if (landedForSeconds > 1f && Time.time > nextDismountTime)
				{
					foreach (BaseVehicle.MountPointInfo mountPoint in cH47HelicopterAIController.mountPoints)
					{
						if (Object.op_Implicit((Object)(object)mountPoint.mountable) && mountPoint.mountable.AnyMounted())
						{
							nextDismountTime = Time.time + 0.5f;
							mountPoint.mountable.DismountAllPlayers();
							break;
						}
					}
				}
				if (landedForSeconds > 8f)
				{
					((Component)brain).GetComponent<CH47AIBrain>().ForceSetAge(float.PositiveInfinity);
				}
			}
			return StateStatus.Running;
		}

		public override void StateEnter(BaseAIBrain brain, BaseEntity entity)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			brain.mainInterestPoint = (entity as CH47HelicopterAIController).landingTarget;
			landingHeight = 15f;
			base.StateEnter(brain, entity);
		}

		public override void StateLeave(BaseAIBrain brain, BaseEntity entity)
		{
			CH47HelicopterAIController obj = entity as CH47HelicopterAIController;
			obj.EnableFacingOverride(enabled: false);
			obj.SetAltitudeProtection(on: true);
			obj.SetMinHoverHeight(30f);
			landedForSeconds = 0f;
			base.StateLeave(brain, entity);
		}

		public override bool CanInterrupt()
		{
			return true;
		}
	}

	public class OrbitState : BasicAIState
	{
		public OrbitState()
			: base(AIState.Orbit)
		{
		}

		public Vector3 GetOrbitCenter()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return brain.mainInterestPoint;
		}

		public override float GetWeight()
		{
			if (IsInState())
			{
				float num = 1f - Mathf.InverseLerp(120f, 180f, base.TimeInState);
				return 5f * num;
			}
			if (brain.CurrentState != null && brain.CurrentState.StateType == AIState.Patrol && brain.CurrentState is PatrolState patrolState && patrolState.AtPatrolDestination())
			{
				return 5f;
			}
			return 0f;
		}

		public override void StateEnter(BaseAIBrain brain, BaseEntity entity)
		{
			CH47HelicopterAIController obj = entity as CH47HelicopterAIController;
			obj.EnableFacingOverride(enabled: true);
			obj.InitiateAnger();
			base.StateEnter(brain, entity);
		}

		public override StateStatus StateThink(float delta, BaseAIBrain brain, BaseEntity entity)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			Vector3 orbitCenter = GetOrbitCenter();
			CH47HelicopterAIController cH47HelicopterAIController = entity as CH47HelicopterAIController;
			Vector3 position = cH47HelicopterAIController.GetPosition();
			Vector3 val = Vector3Ex.Direction2D(orbitCenter, position);
			Vector3 val2 = Vector3.Cross(Vector3.up, val);
			float num = ((Vector3.Dot(Vector3.Cross(((Component)cH47HelicopterAIController).transform.right, Vector3.up), val2) < 0f) ? (-1f) : 1f);
			float num2 = 75f;
			Vector3 val3 = -val + val2 * num * 0.6f;
			Vector3 normalized = ((Vector3)(ref val3)).normalized;
			Vector3 val4 = orbitCenter + normalized * num2;
			cH47HelicopterAIController.SetMoveTarget(val4);
			cH47HelicopterAIController.SetAimDirection(Vector3Ex.Direction2D(val4, position));
			base.StateThink(delta, brain, entity);
			return StateStatus.Running;
		}

		public override void StateLeave(BaseAIBrain brain, BaseEntity entity)
		{
			CH47HelicopterAIController obj = entity as CH47HelicopterAIController;
			obj.EnableFacingOverride(enabled: false);
			obj.CancelAnger();
			base.StateLeave(brain, entity);
		}
	}

	public class PatrolState : BasePatrolState
	{
		protected float patrolApproachDist = 75f;

		public override void StateEnter(BaseAIBrain brain, BaseEntity entity)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			base.StateEnter(brain, entity);
			brain.mainInterestPoint = brain.PathFinder.GetRandomPatrolPoint();
		}

		public override StateStatus StateThink(float delta, BaseAIBrain brain, BaseEntity entity)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			base.StateThink(delta, brain, entity);
			(entity as CH47HelicopterAIController).SetMoveTarget(brain.mainInterestPoint);
			return StateStatus.Running;
		}

		public bool AtPatrolDestination()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			return Vector3Ex.Distance2D(GetDestination(), ((Component)brain).transform.position) < patrolApproachDist;
		}

		public Vector3 GetDestination()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			return brain.mainInterestPoint;
		}

		public override bool CanInterrupt()
		{
			if (base.CanInterrupt())
			{
				return AtPatrolDestination();
			}
			return false;
		}

		public override float GetWeight()
		{
			if (IsInState())
			{
				if (AtPatrolDestination() && base.TimeInState > 2f)
				{
					return 0f;
				}
				return 3f;
			}
			float num = Mathf.InverseLerp(70f, 120f, TimeSinceState()) * 5f;
			return 1f + num;
		}
	}

	public override void AddStates()
	{
		base.AddStates();
		AddState(new IdleState());
		AddState(new PatrolState());
		AddState(new OrbitState());
		AddState(new EgressState());
		AddState(new DropCrate());
		AddState(new LandState());
	}

	public override void InitializeAI()
	{
		base.InitializeAI();
		base.ThinkMode = AIThinkMode.FixedUpdate;
		base.PathFinder = new CH47PathFinder();
	}

	public void FixedUpdate()
	{
		if (!((Object)(object)base.baseEntity == (Object)null) && !base.baseEntity.isClient)
		{
			Think(Time.fixedDeltaTime);
		}
	}
}
