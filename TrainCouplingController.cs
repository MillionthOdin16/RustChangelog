using System.Collections.Generic;
using UnityEngine;

public class TrainCouplingController
{
	public const BaseEntity.Flags Flag_CouplingFront = BaseEntity.Flags.Reserved2;

	public const BaseEntity.Flags Flag_CouplingRear = BaseEntity.Flags.Reserved3;

	public readonly TrainCoupling frontCoupling;

	public readonly TrainCoupling rearCoupling;

	private readonly TrainCar owner;

	[ServerVar(Help = "Maximum difference in velocity for train cars to couple")]
	public static float max_couple_speed = 9f;

	public bool IsCoupled => IsFrontCoupled || IsRearCoupled;

	public bool IsFrontCoupled => owner.HasFlag(BaseEntity.Flags.Reserved2);

	public bool IsRearCoupled => owner.HasFlag(BaseEntity.Flags.Reserved3);

	public float PreChangeTrackSpeed { get; private set; }

	public bool PreChangeCoupledBackwards { get; private set; }

	public TrainCouplingController(TrainCar owner)
	{
		this.owner = owner;
		frontCoupling = new TrainCoupling(owner, isFrontCoupling: true, this, owner.frontCoupling, owner.frontCouplingPivot, BaseEntity.Flags.Reserved2);
		rearCoupling = new TrainCoupling(owner, isFrontCoupling: false, this, owner.rearCoupling, owner.rearCouplingPivot, BaseEntity.Flags.Reserved3);
	}

	public bool IsCoupledTo(TrainCar them)
	{
		return frontCoupling.IsCoupledTo(them) || rearCoupling.IsCoupledTo(them);
	}

	public bool TryCouple(TrainCar them, TriggerTrainCollisions.Location ourLocation)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		TrainCoupling trainCoupling = ((ourLocation == TriggerTrainCollisions.Location.Front) ? frontCoupling : rearCoupling);
		if (!trainCoupling.isValid)
		{
			return false;
		}
		if (trainCoupling.IsCoupled)
		{
			return false;
		}
		if (TimeSince.op_Implicit(trainCoupling.timeSinceCouplingBlock) < 1.5f)
		{
			return false;
		}
		float num = Vector3.Angle(((Component)owner).transform.forward, ((Component)them).transform.forward);
		if (num > 25f && num < 155f)
		{
			return false;
		}
		bool flag = num < 90f;
		TrainCoupling trainCoupling2 = ((!flag) ? ((ourLocation == TriggerTrainCollisions.Location.Front) ? them.coupling.frontCoupling : them.coupling.rearCoupling) : ((ourLocation == TriggerTrainCollisions.Location.Front) ? them.coupling.rearCoupling : them.coupling.frontCoupling));
		float num2 = them.GetTrackSpeed();
		if (!flag)
		{
			num2 = 0f - num2;
		}
		float num3 = Mathf.Abs(num2 - owner.GetTrackSpeed());
		if (num3 > max_couple_speed)
		{
			trainCoupling.timeSinceCouplingBlock = TimeSince.op_Implicit(0f);
			trainCoupling2.timeSinceCouplingBlock = TimeSince.op_Implicit(0f);
			return false;
		}
		if (!trainCoupling2.isValid)
		{
			return false;
		}
		float num4 = Vector3.SqrMagnitude(trainCoupling.couplingPoint.position - trainCoupling2.couplingPoint.position);
		if (num4 > 0.5f)
		{
			return false;
		}
		TrainTrackSpline frontTrackSection = owner.FrontTrackSection;
		TrainTrackSpline frontTrackSection2 = them.FrontTrackSection;
		if ((Object)(object)frontTrackSection2 != (Object)(object)frontTrackSection && !frontTrackSection.HasConnectedTrack(frontTrackSection2))
		{
			return false;
		}
		return trainCoupling.TryCouple(trainCoupling2, reflect: true);
	}

	public void Uncouple(bool front)
	{
		if (front)
		{
			frontCoupling.Uncouple(reflect: true);
		}
		else
		{
			rearCoupling.Uncouple(reflect: true);
		}
	}

	public void GetAll(ref List<TrainCar> result)
	{
		result.Add(owner);
		TrainCoupling coupledTo = rearCoupling.CoupledTo;
		while (coupledTo != null && coupledTo.IsCoupled && !result.Contains(coupledTo.owner))
		{
			result.Insert(0, coupledTo.owner);
			coupledTo = coupledTo.GetOppositeCoupling();
			coupledTo = coupledTo.CoupledTo;
		}
		TrainCoupling coupledTo2 = frontCoupling.CoupledTo;
		while (coupledTo2 != null && coupledTo2.IsCoupled && !result.Contains(coupledTo2.owner))
		{
			result.Add(coupledTo2.owner);
			coupledTo2 = coupledTo2.GetOppositeCoupling();
			coupledTo2 = coupledTo2.CoupledTo;
		}
	}

	public void OnPreCouplingChange()
	{
		PreChangeCoupledBackwards = owner.IsCoupledBackwards();
		PreChangeTrackSpeed = owner.GetTrackSpeed();
		if (PreChangeCoupledBackwards)
		{
			PreChangeTrackSpeed = 0f - PreChangeTrackSpeed;
		}
	}
}
