using System;
using Facepunch;
using Network;
using ProtoBuf;
using Rust.UI;
using Sonar;
using UnityEngine;

public class Tugboat : MotorRowboat
{
	[Header("Tugboat")]
	[SerializeField]
	private Canvas monitorCanvas;

	[SerializeField]
	private RustText fuelText;

	[SerializeField]
	private RustText speedText;

	[SerializeField]
	private ParticleSystemContainer exhaustEffect;

	[SerializeField]
	private SoundDefinition lightsToggleSound;

	[SerializeField]
	private Transform steeringWheelLeftHandTarget;

	[SerializeField]
	private Transform steeringWheelRightHandTarget;

	[SerializeField]
	private SonarSystem sonar;

	[SerializeField]
	private TugboatSounds tugboatSounds;

	[SerializeField]
	private CanvasGroup canvasGroup;

	[SerializeField]
	private EmissionToggle emissionToggle;

	[SerializeField]
	private AnimationCurve emissionCurve;

	[SerializeField]
	private ParticleSystemContainer fxLightDamage;

	[SerializeField]
	private ParticleSystemContainer fxMediumDamage;

	[SerializeField]
	private ParticleSystemContainer fxHeavyDamage;

	[SerializeField]
	private GameObject heavyDamageLights;

	[SerializeField]
	private TriggerParent parentTrigger;

	[ServerVar]
	[Help("how long until boat corpses despawn (excluding tugboat)")]
	public static float tugcorpseseconds = 7200f;

	[ServerVar(Help = "How long before a tugboat loses all its health while outside")]
	public static float tugdecayminutes = 2160f;

	[ServerVar(Help = "How long until decay begins after the tugboat was last used")]
	public static float tugdecaystartdelayminutes = 1440f;

	public bool LightsAreOn => HasFlag(Flags.Reserved5);

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("Tugboat.OnRpcMessage", 0);
		try
		{
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
	}

	public override float MaxVelocity()
	{
		return 15f;
	}

	public override void VehicleFixedUpdate()
	{
		int fuelAmount = fuelSystem.GetFuelAmount();
		base.VehicleFixedUpdate();
		int fuelAmount2 = fuelSystem.GetFuelAmount();
		if (fuelAmount2 != fuelAmount)
		{
			ClientRPC(null, "SetFuelAmount", fuelAmount2);
		}
		if (LightsAreOn && !IsOn())
		{
			SetFlag(Flags.Reserved5, b: false);
		}
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		info.msg.simpleUint = Pool.Get<SimpleUInt>();
		info.msg.simpleUint.value = (uint)fuelSystem.GetFuelAmount();
	}

	public override void BoatDecay()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		if (!base.IsDying)
		{
			BaseBoat.WaterVehicleDecay(this, 60f, TimeSince.op_Implicit(timeSinceLastUsedFuel), tugdecayminutes, tugdecayminutes, tugdecaystartdelayminutes, preventDecayIndoors);
		}
	}

	public override int StartingFuelUnits()
	{
		return 0;
	}

	public override void LightToggle(BasePlayer player)
	{
		if (IsDriver(player))
		{
			if (!IsOn())
			{
				SetFlag(Flags.Reserved5, b: false);
			}
			else
			{
				SetFlag(Flags.Reserved5, !LightsAreOn);
			}
		}
	}

	protected override void EnterCorpseState()
	{
		((FacepunchBehaviour)this).Invoke((Action)base.ActualDeath, tugcorpseseconds);
	}

	public override bool AnyPlayersOnBoat()
	{
		if (base.AnyPlayersOnBoat())
		{
			return true;
		}
		if ((Object)(object)parentTrigger != (Object)null && parentTrigger.HasAnyEntityContents)
		{
			foreach (BaseEntity entityContent in parentTrigger.entityContents)
			{
				if ((Object)(object)entityContent.ToPlayer() != (Object)null)
				{
					return true;
				}
			}
		}
		return false;
	}

	public override bool SupportsChildDeployables()
	{
		return true;
	}

	public override bool ForceDeployableSetParent()
	{
		return true;
	}

	protected override bool CanPushNow(BasePlayer pusher)
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		if (IsOn())
		{
			return false;
		}
		if (!IsStationary() || (!(pusher.WaterFactor() <= 0.6f) && !IsFlipped()))
		{
			return false;
		}
		if (!IsFlipped() && pusher.IsStandingOnEntity(this, 1218652417))
		{
			return false;
		}
		if (pusher.IsBuildingBlockedByVehicle())
		{
			return false;
		}
		Vector3 val = ((Component)this).transform.TransformPoint(-Vector3.up);
		WaterLevel.WaterInfo waterInfo = WaterLevel.GetWaterInfo(val, waves: true, volumes: false, this, noEarlyExit: true);
		if (val.y - waterInfo.surfaceLevel > 2f)
		{
			return false;
		}
		if (base.IsDying)
		{
			return false;
		}
		if (!pusher.isMounted && pusher.IsOnGround())
		{
			return base.healthFraction > 0f;
		}
		return false;
	}
}
