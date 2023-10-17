using UnityEngine;

public class CarPhysics<TCar> where TCar : BaseVehicle, CarPhysics<TCar>.ICar
{
	public interface ICar
	{
		VehicleTerrainHandler.Surface OnSurface { get; }

		float GetThrottleInput();

		float GetBrakeInput();

		float GetSteerInput();

		bool GetSteerModInput();

		float GetMaxForwardSpeed();

		float GetMaxDriveForce();

		float GetAdjustedDriveForce(float absSpeed, float topSpeed);

		float GetModifiedDrag();

		CarWheel[] GetWheels();

		float GetWheelsMidPos();
	}

	private class ServerWheelData
	{
		public CarWheel wheel;

		public Transform wheelColliderTransform;

		public WheelCollider wheelCollider;

		public bool isGrounded;

		public float downforce;

		public float forceDistance;

		public WheelHit hit;

		public Vector2 localRigForce;

		public Vector2 localVelocity;

		public float angularVelocity;

		public Vector3 origin;

		public Vector2 tyreForce;

		public Vector2 tyreSlip;

		public Vector3 velocity;

		public bool isBraking;

		public bool hasThrottleInput;

		public bool isFrontWheel;

		public bool isLeftWheel;
	}

	private readonly ServerWheelData[] wheelData;

	private readonly TCar car;

	private readonly Transform transform;

	private readonly Rigidbody rBody;

	private readonly CarSettings vehicleSettings;

	private float speedAngle;

	private bool wasSleeping = true;

	private bool hasDriver;

	private bool hadDriver;

	private float lastMovingTime = float.MinValue;

	private WheelFrictionCurve zeroFriction;

	private Vector3 prevLocalCOM;

	private readonly float midWheelPos;

	private const bool WHEEL_HIT_CORRECTION = true;

	private const float SLEEP_SPEED = 0.25f;

	private const float SLEEP_DELAY = 10f;

	private const float AIR_DRAG = 0.25f;

	private const float DEFAULT_GROUND_GRIP = 0.75f;

	private const float ROAD_GROUND_GRIP = 1f;

	private const float ICE_GROUND_GRIP = 0.25f;

	private bool slowSpeedExitFlag;

	private const float SLOW_SPEED_EXIT_SPEED = 4f;

	private TimeSince timeSinceWaterCheck;

	public float DriveWheelVelocity { get; private set; }

	public float DriveWheelSlip { get; private set; }

	public float SteerAngle { get; private set; }

	public float TankThrottleLeft { get; private set; }

	public float TankThrottleRight { get; private set; }

	private bool InSlowSpeedExitMode
	{
		get
		{
			if (!hasDriver)
			{
				return slowSpeedExitFlag;
			}
			return false;
		}
	}

	public CarPhysics(TCar car, Transform transform, Rigidbody rBody, CarSettings vehicleSettings)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		WheelFrictionCurve val = default(WheelFrictionCurve);
		((WheelFrictionCurve)(ref val)).stiffness = 0f;
		zeroFriction = val;
		Transform transform2 = transform;
		base._002Ector();
		CarPhysics<TCar> carPhysics = this;
		this.car = car;
		this.transform = transform2;
		this.rBody = rBody;
		this.vehicleSettings = vehicleSettings;
		timeSinceWaterCheck = default(TimeSince);
		timeSinceWaterCheck = TimeSince.op_Implicit(float.MaxValue);
		prevLocalCOM = rBody.centerOfMass;
		CarWheel[] wheels = car.GetWheels();
		wheelData = new ServerWheelData[wheels.Length];
		for (int i = 0; i < wheelData.Length; i++)
		{
			wheelData[i] = AddWheel(wheels[i]);
		}
		midWheelPos = car.GetWheelsMidPos();
		wheelData[0].wheel.wheelCollider.ConfigureVehicleSubsteps(1000f, 1, 1);
		lastMovingTime = Time.realtimeSinceStartup;
		ServerWheelData AddWheel(CarWheel wheel)
		{
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			ServerWheelData obj = new ServerWheelData
			{
				wheelCollider = wheel.wheelCollider,
				wheelColliderTransform = ((Component)wheel.wheelCollider).transform,
				forceDistance = GetWheelForceDistance(wheel.wheelCollider),
				wheel = wheel
			};
			obj.wheelCollider.sidewaysFriction = zeroFriction;
			obj.wheelCollider.forwardFriction = zeroFriction;
			Vector3 val2 = transform2.InverseTransformPoint(((Component)wheel.wheelCollider).transform.position);
			obj.isFrontWheel = val2.z > 0f;
			obj.isLeftWheel = val2.x < 0f;
			return obj;
		}
	}

	public void FixedUpdate(float dt, float speed)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Unknown result type (might be due to invalid IL or missing references)
		//IL_038e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0393: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_036c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0371: Unknown result type (might be due to invalid IL or missing references)
		if (rBody.centerOfMass != prevLocalCOM)
		{
			COMChanged();
		}
		float num = Mathf.Abs(speed);
		hasDriver = car.HasDriver();
		if (!hasDriver && hadDriver)
		{
			if (num <= 4f)
			{
				slowSpeedExitFlag = true;
			}
		}
		else if (hasDriver && !hadDriver)
		{
			slowSpeedExitFlag = false;
		}
		if ((hasDriver || !vehicleSettings.canSleep) && rBody.IsSleeping())
		{
			rBody.WakeUp();
		}
		Vector3 val;
		if (!rBody.IsSleeping())
		{
			if ((!wasSleeping || rBody.isKinematic) && !(num > 0.25f))
			{
				val = rBody.angularVelocity;
				if (!(Mathf.Abs(((Vector3)(ref val)).magnitude) > 0.25f))
				{
					goto IL_00f7;
				}
			}
			lastMovingTime = Time.time;
			goto IL_00f7;
		}
		wasSleeping = true;
		goto IL_0515;
		IL_03c1:
		int num2;
		bool flag = (byte)num2 != 0;
		int num3;
		float num4;
		for (int i = 0; i < wheelData.Length; i++)
		{
			ServerWheelData serverWheelData = wheelData[i];
			serverWheelData.wheelCollider.motorTorque = 1E-05f;
			if (flag && car.OnSurface != VehicleTerrainHandler.Surface.Frictionless)
			{
				serverWheelData.wheelCollider.brakeTorque = 10000f;
			}
			else
			{
				serverWheelData.wheelCollider.brakeTorque = 0f;
			}
			if (serverWheelData.wheel.steerWheel)
			{
				serverWheelData.wheel.wheelCollider.steerAngle = (serverWheelData.isFrontWheel ? SteerAngle : (vehicleSettings.rearWheelSteer * (0f - SteerAngle)));
			}
			UpdateSuspension(serverWheelData);
			if (serverWheelData.isGrounded)
			{
				num3++;
				num4 += wheelData[i].downforce;
			}
		}
		AdjustHitForces(num3, num4 / (float)num3);
		float maxForwardSpeed;
		float num5;
		float steerInput;
		float brakeInput;
		float num6;
		float maxDriveForce;
		for (int j = 0; j < wheelData.Length; j++)
		{
			ServerWheelData wd = wheelData[j];
			UpdateLocalFrame(wd, dt);
			ComputeTyreForces(wd, speed, maxDriveForce, maxForwardSpeed, num5, steerInput, brakeInput, num6);
			ApplyTyreForces(wd, num5, steerInput, speed);
		}
		ComputeOverallForces();
		goto IL_0505;
		IL_00f7:
		bool flag2 = vehicleSettings.canSleep && !hasDriver && Time.time > lastMovingTime + 10f;
		if (flag2 && (car.GetParentEntity() as BaseVehicle).IsValid())
		{
			flag2 = false;
		}
		if (flag2)
		{
			for (int k = 0; k < wheelData.Length; k++)
			{
				ServerWheelData obj = wheelData[k];
				obj.wheelCollider.motorTorque = 0f;
				obj.wheelCollider.brakeTorque = 0f;
				obj.wheelCollider.steerAngle = 0f;
			}
			rBody.Sleep();
			goto IL_0505;
		}
		speedAngle = Vector3.Angle(rBody.velocity, transform.forward) * Mathf.Sign(Vector3.Dot(rBody.velocity, transform.right));
		maxDriveForce = car.GetMaxDriveForce();
		maxForwardSpeed = car.GetMaxForwardSpeed();
		num5 = (car.IsOn() ? car.GetThrottleInput() : 0f);
		steerInput = car.GetSteerInput();
		brakeInput = (InSlowSpeedExitMode ? 1f : car.GetBrakeInput());
		num6 = 1f;
		if (num < 3f)
		{
			num6 = 2.75f;
		}
		else if (num < 9f)
		{
			float num7 = Mathf.InverseLerp(9f, 3f, num);
			num6 = Mathf.Lerp(1f, 2.75f, num7);
		}
		maxDriveForce *= num6;
		ComputeSteerAngle(num5, steerInput, dt, speed);
		if (TimeSince.op_Implicit(timeSinceWaterCheck) > 0.25f)
		{
			float num8 = car.WaterFactor();
			float num9 = 0f;
			if (car.FindTrigger<TriggerVehicleDrag>(out var result))
			{
				num9 = result.vehicleDrag;
			}
			float num10 = ((num5 != 0f) ? 0f : 0.25f);
			float num11 = Mathf.Max(num8, num9);
			num11 = Mathf.Max(num11, car.GetModifiedDrag());
			rBody.drag = Mathf.Max(num10, num11);
			rBody.angularDrag = num11 * 0.5f;
			timeSinceWaterCheck = TimeSince.op_Implicit(0f);
		}
		num3 = 0;
		num4 = 0f;
		if (!hasDriver)
		{
			val = rBody.velocity;
			if (((Vector3)(ref val)).magnitude < 2.5f)
			{
				num2 = ((TimeSince.op_Implicit(car.timeSinceLastPush) > 2f) ? 1 : 0);
				goto IL_03c1;
			}
		}
		num2 = 0;
		goto IL_03c1;
		IL_0515:
		hadDriver = hasDriver;
		return;
		IL_0505:
		wasSleeping = false;
		goto IL_0515;
	}

	public bool IsGrounded()
	{
		int num = 0;
		for (int i = 0; i < wheelData.Length; i++)
		{
			if (wheelData[i].isGrounded)
			{
				num++;
			}
			if (num >= 2)
			{
				return true;
			}
		}
		return false;
	}

	private void COMChanged()
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < wheelData.Length; i++)
		{
			ServerWheelData serverWheelData = wheelData[i];
			serverWheelData.forceDistance = GetWheelForceDistance(serverWheelData.wheel.wheelCollider);
		}
		prevLocalCOM = rBody.centerOfMass;
	}

	private void ComputeSteerAngle(float throttleInput, float steerInput, float dt, float speed)
	{
		if (vehicleSettings.tankSteering)
		{
			SteerAngle = 0f;
			ComputeTankSteeringThrottle(throttleInput, steerInput, speed);
			return;
		}
		float num = vehicleSettings.maxSteerAngle * steerInput;
		float num2 = Mathf.InverseLerp(0f, vehicleSettings.minSteerLimitSpeed, speed);
		if (vehicleSettings.steeringLimit)
		{
			float num3 = Mathf.Lerp(vehicleSettings.maxSteerAngle, vehicleSettings.minSteerLimitAngle, num2);
			num = Mathf.Clamp(num, 0f - num3, num3);
		}
		float num4 = 0f;
		if (vehicleSettings.steeringAssist)
		{
			float num5 = Mathf.InverseLerp(0.1f, 3f, speed);
			num4 = speedAngle * vehicleSettings.steeringAssistRatio * num5 * Mathf.InverseLerp(2f, 3f, Mathf.Abs(speedAngle));
		}
		float num6 = Mathf.Clamp(num + num4, 0f - vehicleSettings.maxSteerAngle, vehicleSettings.maxSteerAngle);
		if (SteerAngle != num6)
		{
			float num7 = 1f - num2 * 0.7f;
			float num9;
			if ((SteerAngle == 0f || Mathf.Sign(num6) == Mathf.Sign(SteerAngle)) && Mathf.Abs(num6) > Mathf.Abs(SteerAngle))
			{
				float num8 = SteerAngle / vehicleSettings.maxSteerAngle;
				num9 = Mathf.Lerp(vehicleSettings.steerMinLerpSpeed * num7, vehicleSettings.steerMaxLerpSpeed * num7, num8 * num8);
			}
			else
			{
				num9 = vehicleSettings.steerReturnLerpSpeed * num7;
			}
			if (car.GetSteerModInput())
			{
				num9 *= 1.5f;
			}
			SteerAngle = Mathf.MoveTowards(SteerAngle, num6, dt * num9);
		}
	}

	private float GetWheelForceDistance(WheelCollider col)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		return rBody.centerOfMass.y - transform.InverseTransformPoint(((Component)col).transform.position).y + col.radius + (1f - col.suspensionSpring.targetPosition) * col.suspensionDistance;
	}

	private void UpdateSuspension(ServerWheelData wd)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		wd.isGrounded = wd.wheelCollider.GetGroundHit(ref wd.hit);
		wd.origin = wd.wheelColliderTransform.TransformPoint(wd.wheelCollider.center);
		if (wd.isGrounded && GamePhysics.Trace(new Ray(wd.origin, -wd.wheelColliderTransform.up), 0f, out var hitInfo, wd.wheelCollider.suspensionDistance + wd.wheelCollider.radius, 1235321089, (QueryTriggerInteraction)1))
		{
			((WheelHit)(ref wd.hit)).point = ((RaycastHit)(ref hitInfo)).point;
			((WheelHit)(ref wd.hit)).normal = ((RaycastHit)(ref hitInfo)).normal;
		}
		if (wd.isGrounded)
		{
			if (((WheelHit)(ref wd.hit)).force < 0f)
			{
				((WheelHit)(ref wd.hit)).force = 0f;
			}
			wd.downforce = ((WheelHit)(ref wd.hit)).force;
		}
		else
		{
			wd.downforce = 0f;
		}
	}

	private void AdjustHitForces(int groundedWheels, float neutralForcePerWheel)
	{
		float num = neutralForcePerWheel * 0.25f;
		for (int i = 0; i < wheelData.Length; i++)
		{
			ServerWheelData serverWheelData = wheelData[i];
			if (!serverWheelData.isGrounded || !(serverWheelData.downforce < num))
			{
				continue;
			}
			if (groundedWheels == 1)
			{
				serverWheelData.downforce = num;
				continue;
			}
			float num2 = (num - serverWheelData.downforce) / (float)(groundedWheels - 1);
			serverWheelData.downforce = num;
			for (int j = 0; j < wheelData.Length; j++)
			{
				ServerWheelData serverWheelData2 = wheelData[j];
				if (serverWheelData2.isGrounded && serverWheelData2.downforce > num)
				{
					float num3 = Mathf.Min(num2, serverWheelData2.downforce - num);
					serverWheelData2.downforce -= num3;
				}
			}
		}
	}

	private void UpdateLocalFrame(ServerWheelData wd, float dt)
	{
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		if (!wd.isGrounded)
		{
			((WheelHit)(ref wd.hit)).point = wd.origin - wd.wheelColliderTransform.up * (wd.wheelCollider.suspensionDistance + wd.wheelCollider.radius);
			((WheelHit)(ref wd.hit)).normal = wd.wheelColliderTransform.up;
			((WheelHit)(ref wd.hit)).collider = null;
		}
		Vector3 pointVelocity = rBody.GetPointVelocity(((WheelHit)(ref wd.hit)).point);
		wd.velocity = pointVelocity - Vector3.Project(pointVelocity, ((WheelHit)(ref wd.hit)).normal);
		wd.localVelocity.y = Vector3.Dot(((WheelHit)(ref wd.hit)).forwardDir, wd.velocity);
		wd.localVelocity.x = Vector3.Dot(((WheelHit)(ref wd.hit)).sidewaysDir, wd.velocity);
		if (!wd.isGrounded)
		{
			wd.localRigForce = Vector2.zero;
			return;
		}
		float num = Mathf.InverseLerp(1f, 0.25f, ((Vector3)(ref wd.velocity)).sqrMagnitude);
		Vector2 val3 = default(Vector2);
		if (num > 0f)
		{
			float num2 = Vector3.Dot(Vector3.up, ((WheelHit)(ref wd.hit)).normal);
			Vector3 val2;
			if (num2 > 1E-06f)
			{
				Vector3 val = Vector3.up * wd.downforce / num2;
				val2 = val - Vector3.Project(val, ((WheelHit)(ref wd.hit)).normal);
			}
			else
			{
				val2 = Vector3.up * 100000f;
			}
			val3.y = Vector3.Dot(((WheelHit)(ref wd.hit)).forwardDir, val2);
			val3.x = Vector3.Dot(((WheelHit)(ref wd.hit)).sidewaysDir, val2);
			val3 *= num;
		}
		else
		{
			val3 = Vector2.zero;
		}
		Vector2 val4 = (0f - Mathf.Clamp(wd.downforce / (0f - Physics.gravity.y), 0f, wd.wheelCollider.sprungMass) * 0.5f) * wd.localVelocity / dt;
		wd.localRigForce = val4 + val3;
	}

	private void ComputeTyreForces(ServerWheelData wd, float speed, float maxDriveForce, float maxSpeed, float throttleInput, float steerInput, float brakeInput, float driveForceMultiplier)
	{
		//IL_0388: Unknown result type (might be due to invalid IL or missing references)
		//IL_038d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0393: Unknown result type (might be due to invalid IL or missing references)
		//IL_0398: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0311: Unknown result type (might be due to invalid IL or missing references)
		//IL_0319: Unknown result type (might be due to invalid IL or missing references)
		//IL_036c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0374: Unknown result type (might be due to invalid IL or missing references)
		//IL_0332: Unknown result type (might be due to invalid IL or missing references)
		float absSpeed = Mathf.Abs(speed);
		if (vehicleSettings.tankSteering && brakeInput == 0f)
		{
			throttleInput = ((!wd.isLeftWheel) ? TankThrottleRight : TankThrottleLeft);
		}
		float num = (wd.wheel.powerWheel ? throttleInput : 0f);
		wd.hasThrottleInput = num != 0f;
		float num2 = vehicleSettings.maxDriveSlip;
		if (Mathf.Sign(num) != Mathf.Sign(wd.localVelocity.y))
		{
			num2 -= wd.localVelocity.y * Mathf.Sign(num);
		}
		float num3 = Mathf.Abs(num);
		float num4 = 0f - vehicleSettings.rollingResistance + num3 * (1f + vehicleSettings.rollingResistance) - brakeInput * (1f - vehicleSettings.rollingResistance);
		if (InSlowSpeedExitMode || num4 < 0f || maxDriveForce == 0f)
		{
			num4 *= -1f;
			wd.isBraking = true;
		}
		else
		{
			num4 *= Mathf.Sign(num);
			wd.isBraking = false;
		}
		float num6;
		if (wd.isBraking)
		{
			float num5 = Mathf.Clamp(car.GetMaxForwardSpeed() * vehicleSettings.brakeForceMultiplier, 10f * vehicleSettings.brakeForceMultiplier, 50f * vehicleSettings.brakeForceMultiplier);
			num5 += rBody.mass * 1.5f;
			num6 = num4 * num5;
		}
		else
		{
			num6 = ComputeDriveForce(speed, absSpeed, num4 * maxDriveForce, maxDriveForce, maxSpeed, driveForceMultiplier);
		}
		if (wd.isGrounded)
		{
			wd.tyreSlip.x = wd.localVelocity.x;
			wd.tyreSlip.y = wd.localVelocity.y - wd.angularVelocity * wd.wheelCollider.radius;
			float num7 = car.OnSurface switch
			{
				VehicleTerrainHandler.Surface.Road => 1f, 
				VehicleTerrainHandler.Surface.Ice => 0.25f, 
				VehicleTerrainHandler.Surface.Frictionless => 0f, 
				_ => 0.75f, 
			};
			float num8 = wd.wheel.tyreFriction * wd.downforce * num7;
			float num9 = 0f;
			if (!wd.isBraking)
			{
				num9 = Mathf.Min(Mathf.Abs(num6 * wd.tyreSlip.x) / num8, num2);
				if (num6 != 0f && num9 < 0.1f)
				{
					num9 = 0.1f;
				}
			}
			if (Mathf.Abs(wd.tyreSlip.y) < num9)
			{
				wd.tyreSlip.y = num9 * Mathf.Sign(wd.tyreSlip.y);
			}
			Vector2 val = (0f - num8) * ((Vector2)(ref wd.tyreSlip)).normalized;
			val.x = Mathf.Abs(val.x) * 1.5f;
			val.y = Mathf.Abs(val.y);
			wd.tyreForce.x = Mathf.Clamp(wd.localRigForce.x, 0f - val.x, val.x);
			if (wd.isBraking)
			{
				float num10 = Mathf.Min(val.y, num6);
				wd.tyreForce.y = Mathf.Clamp(wd.localRigForce.y, 0f - num10, num10);
			}
			else
			{
				wd.tyreForce.y = Mathf.Clamp(num6, 0f - val.y, val.y);
			}
		}
		else
		{
			wd.tyreSlip = Vector2.zero;
			wd.tyreForce = Vector2.zero;
		}
		if (wd.isGrounded)
		{
			float num11;
			if (wd.isBraking)
			{
				num11 = 0f;
			}
			else
			{
				float driveForceToMaxSlip = vehicleSettings.driveForceToMaxSlip;
				num11 = Mathf.Clamp01((Mathf.Abs(num6) - Mathf.Abs(wd.tyreForce.y)) / driveForceToMaxSlip) * num2 * Mathf.Sign(num6);
			}
			wd.angularVelocity = (wd.localVelocity.y + num11) / wd.wheelCollider.radius;
			return;
		}
		float num12 = 50f;
		float num13 = 10f;
		if (num > 0f)
		{
			wd.angularVelocity += num12 * num;
		}
		else
		{
			wd.angularVelocity -= num13;
		}
		wd.angularVelocity -= num12 * brakeInput;
		wd.angularVelocity = Mathf.Clamp(wd.angularVelocity, 0f, maxSpeed / wd.wheelCollider.radius);
	}

	private void ComputeTankSteeringThrottle(float throttleInput, float steerInput, float speed)
	{
		TankThrottleLeft = throttleInput;
		TankThrottleRight = throttleInput;
		float tankSteerInvert = GetTankSteerInvert(throttleInput, speed);
		if (throttleInput == 0f)
		{
			TankThrottleLeft = 0f - steerInput;
			TankThrottleRight = steerInput;
		}
		else if (steerInput > 0f)
		{
			TankThrottleLeft = Mathf.Lerp(throttleInput, -1f * tankSteerInvert, steerInput);
			TankThrottleRight = Mathf.Lerp(throttleInput, 1f * tankSteerInvert, steerInput);
		}
		else if (steerInput < 0f)
		{
			TankThrottleLeft = Mathf.Lerp(throttleInput, 1f * tankSteerInvert, 0f - steerInput);
			TankThrottleRight = Mathf.Lerp(throttleInput, -1f * tankSteerInvert, 0f - steerInput);
		}
	}

	private float ComputeDriveForce(float speed, float absSpeed, float demandedForce, float maxForce, float maxForwardSpeed, float driveForceMultiplier)
	{
		float num = ((speed >= 0f) ? maxForwardSpeed : (maxForwardSpeed * vehicleSettings.reversePercentSpeed));
		if (absSpeed < num)
		{
			if ((speed >= 0f || demandedForce <= 0f) && (speed <= 0f || demandedForce >= 0f))
			{
				maxForce = car.GetAdjustedDriveForce(absSpeed, maxForwardSpeed) * driveForceMultiplier;
			}
			return Mathf.Clamp(demandedForce, 0f - maxForce, maxForce);
		}
		float num2 = maxForce * Mathf.Max(1f - absSpeed / num, -1f) * Mathf.Sign(speed);
		if ((speed < 0f && demandedForce > 0f) || (speed > 0f && demandedForce < 0f))
		{
			num2 = Mathf.Clamp(num2 + demandedForce, 0f - maxForce, maxForce);
		}
		return num2;
	}

	private void ComputeOverallForces()
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		DriveWheelVelocity = 0f;
		DriveWheelSlip = 0f;
		int num = 0;
		for (int i = 0; i < wheelData.Length; i++)
		{
			ServerWheelData serverWheelData = wheelData[i];
			if (serverWheelData.wheel.powerWheel)
			{
				DriveWheelVelocity += serverWheelData.angularVelocity;
				if (serverWheelData.isGrounded)
				{
					float num2 = ComputeCombinedSlip(serverWheelData.localVelocity, serverWheelData.tyreSlip);
					DriveWheelSlip += num2;
				}
				num++;
			}
		}
		if (num > 0)
		{
			DriveWheelVelocity /= num;
			DriveWheelSlip /= num;
		}
	}

	private static float ComputeCombinedSlip(Vector2 localVelocity, Vector2 tyreSlip)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		float magnitude = ((Vector2)(ref localVelocity)).magnitude;
		if (magnitude > 0.01f)
		{
			float num = tyreSlip.x * localVelocity.x / magnitude;
			float y = tyreSlip.y;
			return Mathf.Sqrt(num * num + y * y);
		}
		return ((Vector2)(ref tyreSlip)).magnitude;
	}

	private void ApplyTyreForces(ServerWheelData wd, float throttleInput, float steerInput, float speed)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		if (wd.isGrounded)
		{
			Vector3 val = ((WheelHit)(ref wd.hit)).forwardDir * wd.tyreForce.y;
			Vector3 val2 = ((WheelHit)(ref wd.hit)).sidewaysDir * wd.tyreForce.x;
			Vector3 sidewaysForceAppPoint = GetSidewaysForceAppPoint(wd, ((WheelHit)(ref wd.hit)).point);
			rBody.AddForceAtPosition(val, ((WheelHit)(ref wd.hit)).point, (ForceMode)0);
			rBody.AddForceAtPosition(val2, sidewaysForceAppPoint, (ForceMode)0);
		}
	}

	private Vector3 GetSidewaysForceAppPoint(ServerWheelData wd, Vector3 contactPoint)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = contactPoint + wd.wheelColliderTransform.up * vehicleSettings.antiRoll * wd.forceDistance;
		float num = (wd.wheel.steerWheel ? SteerAngle : 0f);
		if (num != 0f && Mathf.Sign(num) != Mathf.Sign(wd.tyreSlip.x))
		{
			val += wd.wheelColliderTransform.forward * midWheelPos * (vehicleSettings.handlingBias - 0.5f);
		}
		return val;
	}

	private float GetTankSteerInvert(float throttleInput, float speed)
	{
		float result = 1f;
		if (throttleInput < 0f && speed < 1.75f)
		{
			result = -1f;
		}
		else if (throttleInput == 0f && speed < -1f)
		{
			result = -1f;
		}
		else if (speed < -1f)
		{
			result = -1f;
		}
		return result;
	}
}
