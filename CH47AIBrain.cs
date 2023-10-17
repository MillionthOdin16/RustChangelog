using UnityEngine;

public class CH47AIBrain : BaseAIBrain
{
	public class DropCrate : BasicAIState
	{
		private float nextDropTime = 0f;

		public DropCrate()
			: base(AIState.DropCrate)
		{
		}

		public override bool CanInterrupt()
		{
			return base.CanInterrupt() && !CanDrop();
		}

		public bool CanDrop()
		{
			return Time.time > nextDropTime && (brain.GetBrainBaseEntity() as CH47HelicopterAIController).CanDropCrate();
		}

		public override float GetWeight()
		{
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
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
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			CH47HelicopterAIController cH47HelicopterAIController = entity as CH47HelicopterAIController;
			cH47HelicopterAIController.SetDropDoorOpen(open: true);
			cH47HelicopterAIController.EnableFacingOverride(enabled: false);
			CH47DropZone closest = CH47DropZone.GetClosest(((Component)cH47HelicopterAIController).transform.position);
			if ((Object)(object)closest == (Object)null)
			{
				nextDropTime = Time.time + 60f;
			}
			brain.mainInterestPoint = ((Component)closest).transform.position;
			cH47HelicopterAIController.SetMoveTarget(brain.mainInterestPoint);
			base.StateEnter(brain, entity);
		}

		public override StateStatus StateThink(float delta, BaseAIBrain brain, BaseEntity entity)
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
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
		private bool killing = false;

		private bool egressAltitueAchieved = false;

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
				return (component.Age > 1800f) ? 10000f : 0f;
			}
			return 0f;
		}

		public override void StateEnter(BaseAIBrain brain, BaseEntity entity)
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0080: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			CH47HelicopterAIController cH47HelicopterAIController = entity as CH47HelicopterAIController;
			cH47HelicopterAIController.EnableFacingOverride(enabled: false);
			Transform transform = ((Component)cH47HelicopterAIController).transform;
			Rigidbody rigidBody = cH47HelicopterAIController.rigidBody;
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
			Vector3 val3 = Vector3.Cross(transform.up, val2);
			Vector3 val4 = Vector3.Cross(val3, Vector3.up);
			brain.mainInterestPoint = transform.position + val4 * 8000f;
			brain.mainInterestPoint.y = 100f;
			cH47HelicopterAIController.SetMoveTarget(brain.mainInterestPoint);
			base.StateEnter(brain, entity);
		}

		public override StateStatus StateThink(float delta, BaseAIBrain brain, BaseEntity entity)
		{
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
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
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			CH47HelicopterAIController cH47HelicopterAIController = entity as CH47HelicopterAIController;
			Vector3 position = cH47HelicopterAIController.GetPosition();
			Vector3 velocity = cH47HelicopterAIController.rigidBody.velocity;
			cH47HelicopterAIController.SetMoveTarget(position + ((Vector3)(ref velocity)).normalized * 10f);
			base.StateEnter(brain, entity);
		}
	}

	public class LandState : BasicAIState
	{
		private float landedForSeconds = 0f;

		private float lastLandtime = 0f;

		private float landingHeight = 20f;

		private float nextDismountTime = 0f;

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
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_0188: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
			//IL_022c: Unknown result type (might be due to invalid IL or missing references)
			//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_0201: Unknown result type (might be due to invalid IL or missing references)
			//IL_0203: Unknown result type (might be due to invalid IL or missing references)
			//IL_0208: Unknown result type (might be due to invalid IL or missing references)
			//IL_020d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0211: Unknown result type (might be due to invalid IL or missing references)
			//IL_0216: Unknown result type (might be due to invalid IL or missing references)
			//IL_021d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0222: Unknown result type (might be due to invalid IL or missing references)
			//IL_0227: Unknown result type (might be due to invalid IL or missing references)
			base.StateThink(delta, brain, entity);
			CH47HelicopterAIController cH47HelicopterAIController = entity as CH47HelicopterAIController;
			Vector3 position = ((Component)cH47HelicopterAIController).transform.position;
			Vector3 forward = ((Component)cH47HelicopterAIController).transform.forward;
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
			bool flag = Mathf.Abs(((Component)closest).transform.position.y - position.y) < 3f && num <= 5f && magnitude < 1f;
			if (flag)
			{
				landedForSeconds += delta;
				if (lastLandtime == 0f)
				{
					lastLandtime = Time.time;
				}
			}
			float num2 = 1f - Mathf.InverseLerp(0f, 7f, num);
			landingHeight -= 4f * num2 * Time.deltaTime;
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
				if (Physics.SphereCast(position, 15f, val, ref val2, num, 1218511105))
				{
					Vector3 val3 = Vector3.Cross(val, Vector3.up);
					moveTarget = ((RaycastHit)(ref val2)).point + val3 * 50f;
				}
			}
			cH47HelicopterAIController.SetMoveTarget(moveTarget);
			if (flag)
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
					CH47AIBrain component = ((Component)brain).GetComponent<CH47AIBrain>();
					component.ForceSetAge(float.PositiveInfinity);
				}
			}
			return StateStatus.Running;
		}

		public override void StateEnter(BaseAIBrain brain, BaseEntity entity)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			brain.mainInterestPoint = (entity as CH47HelicopterAIController).landingTarget;
			landingHeight = 15f;
			base.StateEnter(brain, entity);
		}

		public override void StateLeave(BaseAIBrain brain, BaseEntity entity)
		{
			CH47HelicopterAIController cH47HelicopterAIController = entity as CH47HelicopterAIController;
			cH47HelicopterAIController.EnableFacingOverride(enabled: false);
			cH47HelicopterAIController.SetAltitudeProtection(on: true);
			cH47HelicopterAIController.SetMinHoverHeight(30f);
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
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
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
			CH47HelicopterAIController cH47HelicopterAIController = entity as CH47HelicopterAIController;
			cH47HelicopterAIController.EnableFacingOverride(enabled: true);
			cH47HelicopterAIController.InitiateAnger();
			base.StateEnter(brain, entity);
		}

		public override StateStatus StateThink(float delta, BaseAIBrain brain, BaseEntity entity)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
			Vector3 orbitCenter = GetOrbitCenter();
			CH47HelicopterAIController cH47HelicopterAIController = entity as CH47HelicopterAIController;
			CH47HelicopterAIController cH47HelicopterAIController2 = cH47HelicopterAIController;
			Vector3 position = cH47HelicopterAIController2.GetPosition();
			Vector3 val = Vector3Ex.Direction2D(orbitCenter, position);
			Vector3 val2 = Vector3.Cross(Vector3.up, val);
			Vector3 val3 = Vector3.Cross(((Component)cH47HelicopterAIController2).transform.right, Vector3.up);
			float num = Vector3.Dot(val3, val2);
			float num2 = ((num < 0f) ? (-1f) : 1f);
			float num3 = 75f;
			Vector3 val4 = -val + val2 * num2 * 0.6f;
			Vector3 normalized = ((Vector3)(ref val4)).normalized;
			Vector3 val5 = orbitCenter + normalized * num3;
			cH47HelicopterAIController2.SetMoveTarget(val5);
			cH47HelicopterAIController2.SetAimDirection(Vector3Ex.Direction2D(val5, position));
			base.StateThink(delta, brain, entity);
			return StateStatus.Running;
		}

		public override void StateLeave(BaseAIBrain brain, BaseEntity entity)
		{
			CH47HelicopterAIController cH47HelicopterAIController = entity as CH47HelicopterAIController;
			cH47HelicopterAIController.EnableFacingOverride(enabled: false);
			cH47HelicopterAIController.CancelAnger();
			base.StateLeave(brain, entity);
		}
	}

	public class PatrolState : BasePatrolState
	{
		protected float patrolApproachDist = 75f;

		public override void StateEnter(BaseAIBrain brain, BaseEntity entity)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			base.StateEnter(brain, entity);
			brain.mainInterestPoint = brain.PathFinder.GetRandomPatrolPoint();
		}

		public override StateStatus StateThink(float delta, BaseAIBrain brain, BaseEntity entity)
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			base.StateThink(delta, brain, entity);
			(entity as CH47HelicopterAIController).SetMoveTarget(brain.mainInterestPoint);
			return StateStatus.Running;
		}

		public bool AtPatrolDestination()
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			return Vector3Ex.Distance2D(GetDestination(), ((Component)brain).transform.position) < patrolApproachDist;
		}

		public Vector3 GetDestination()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			return brain.mainInterestPoint;
		}

		public override bool CanInterrupt()
		{
			return base.CanInterrupt() && AtPatrolDestination();
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
