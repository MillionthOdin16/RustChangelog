using System;
using UnityEngine;

public class BasicCar : BaseVehicle
{
	[Serializable]
	public class VehicleWheel
	{
		public Transform shock;

		public WheelCollider wheelCollider;

		public Transform wheel;

		public Transform axle;

		public bool steerWheel;

		public bool brakeWheel = true;

		public bool powerWheel = true;
	}

	public VehicleWheel[] wheels;

	public float brakePedal;

	public float gasPedal;

	public float steering;

	public Transform centerOfMass;

	public Transform steeringWheel;

	public float motorForceConstant = 150f;

	public float brakeForceConstant = 500f;

	public float GasLerpTime = 20f;

	public float SteeringLerpTime = 20f;

	public Transform driverEye;

	public GameObjectRef chairRef;

	public Transform chairAnchorTest;

	public SoundPlayer idleLoopPlayer;

	public Transform engineOffset;

	public SoundDefinition engineSoundDef;

	private static bool chairtest;

	private float throttle;

	private float brake;

	private bool lightsOn = true;

	public override float MaxVelocity()
	{
		return 50f;
	}

	public override Vector3 EyePositionForPlayer(BasePlayer player, Quaternion viewRot)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		if (PlayerIsMounted(player))
		{
			return ((Component)driverEye).transform.position;
		}
		return Vector3.zero;
	}

	public override void ServerInit()
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		if (!base.isClient)
		{
			base.ServerInit();
			rigidBody = ((Component)this).GetComponent<Rigidbody>();
			rigidBody.centerOfMass = centerOfMass.localPosition;
			rigidBody.isKinematic = false;
			if (chairtest)
			{
				SpawnChairTest();
			}
		}
	}

	public void SpawnChairTest()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		BaseEntity baseEntity = GameManager.server.CreateEntity(chairRef.resourcePath, ((Component)chairAnchorTest).transform.localPosition);
		baseEntity.Spawn();
		DestroyOnGroundMissing component = ((Component)baseEntity).GetComponent<DestroyOnGroundMissing>();
		if ((Object)(object)component != (Object)null)
		{
			((Behaviour)component).enabled = false;
		}
		MeshCollider component2 = ((Component)baseEntity).GetComponent<MeshCollider>();
		if (Object.op_Implicit((Object)(object)component2))
		{
			component2.convex = true;
		}
		baseEntity.SetParent(this);
	}

	public override void VehicleFixedUpdate()
	{
		base.VehicleFixedUpdate();
		if (!HasDriver())
		{
			NoDriverInput();
		}
		ConvertInputToThrottle();
		DoSteering();
		ApplyForceAtWheels();
		SetFlag(Flags.Reserved1, HasDriver());
		SetFlag(Flags.Reserved2, HasDriver() && lightsOn);
	}

	private void DoSteering()
	{
		VehicleWheel[] array = wheels;
		foreach (VehicleWheel vehicleWheel in array)
		{
			if (vehicleWheel.steerWheel)
			{
				vehicleWheel.wheelCollider.steerAngle = steering;
			}
		}
		SetFlag(Flags.Reserved4, steering < -2f);
		SetFlag(Flags.Reserved5, steering > 2f);
	}

	public void ConvertInputToThrottle()
	{
	}

	private void ApplyForceAtWheels()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)rigidBody == (Object)null)
		{
			return;
		}
		Vector3 velocity = rigidBody.velocity;
		float num = ((Vector3)(ref velocity)).magnitude * Vector3.Dot(((Vector3)(ref velocity)).normalized, ((Component)this).transform.forward);
		float num2 = brakePedal;
		float num3 = gasPedal;
		if (num > 0f && num3 < 0f)
		{
			num2 = 100f;
		}
		else if (num < 0f && num3 > 0f)
		{
			num2 = 100f;
		}
		VehicleWheel[] array = wheels;
		foreach (VehicleWheel vehicleWheel in array)
		{
			if (vehicleWheel.wheelCollider.isGrounded)
			{
				if (vehicleWheel.powerWheel)
				{
					vehicleWheel.wheelCollider.motorTorque = num3 * motorForceConstant;
				}
				if (vehicleWheel.brakeWheel)
				{
					vehicleWheel.wheelCollider.brakeTorque = num2 * brakeForceConstant;
				}
			}
		}
		SetFlag(Flags.Reserved3, num2 >= 100f && AnyMounted());
	}

	public void NoDriverInput()
	{
		if (chairtest)
		{
			gasPedal = Mathf.Sin(Time.time) * 50f;
			return;
		}
		gasPedal = 0f;
		brakePedal = Mathf.Lerp(brakePedal, 100f, Time.deltaTime * GasLerpTime / 5f);
	}

	public override void PlayerServerInput(InputState inputState, BasePlayer player)
	{
		if (IsDriver(player))
		{
			DriverInput(inputState, player);
		}
	}

	public void DriverInput(InputState inputState, BasePlayer player)
	{
		if (inputState.IsDown(BUTTON.FORWARD))
		{
			gasPedal = 100f;
			brakePedal = 0f;
		}
		else if (inputState.IsDown(BUTTON.BACKWARD))
		{
			gasPedal = -30f;
			brakePedal = 0f;
		}
		else
		{
			gasPedal = 0f;
			brakePedal = 30f;
		}
		if (inputState.IsDown(BUTTON.LEFT))
		{
			steering = -60f;
		}
		else if (inputState.IsDown(BUTTON.RIGHT))
		{
			steering = 60f;
		}
		else
		{
			steering = 0f;
		}
	}

	public override void LightToggle(BasePlayer player)
	{
		if (IsDriver(player))
		{
			lightsOn = !lightsOn;
		}
	}
}
