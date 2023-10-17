using System;
using ConVar;
using Rust;
using UnityEngine;
using UnityEngine.Serialization;

public class TrainBarricade : BaseCombatEntity, ITrainCollidable, TrainTrackSpline.ITrainTrackUser
{
	[FormerlySerializedAs("damagePerMPS")]
	[SerializeField]
	private float trainDamagePerMPS = 10f;

	[SerializeField]
	private float minVelToDestroy = 6f;

	[SerializeField]
	private float velReduction = 2f;

	[SerializeField]
	private GameObjectRef barricadeDamageEffect;

	private TrainCar hitTrain;

	private TriggerTrainCollisions hitTrainTrigger;

	private TrainTrackSpline track;

	public Vector3 Position => ((Component)this).transform.position;

	public float FrontWheelSplineDist { get; private set; }

	public TrainCar.TrainCarType CarType => TrainCar.TrainCarType.Other;

	public bool CustomCollision(TrainCar train, TriggerTrainCollisions trainTrigger)
	{
		bool result = false;
		if (base.isServer)
		{
			float num = Mathf.Abs(train.GetTrackSpeed());
			SetHitTrain(train, trainTrigger);
			if (num < minVelToDestroy && !vehicle.cinematictrains)
			{
				((FacepunchBehaviour)this).InvokeRandomized((Action)PushForceTick, 0f, 0.25f, 0.025f);
			}
			else
			{
				result = true;
				((FacepunchBehaviour)this).Invoke((Action)DestroyThisBarrier, 0f);
			}
		}
		return result;
	}

	public override void ServerInit()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		base.ServerInit();
		if (TrainTrackSpline.TryFindTrackNear(((Component)this).transform.position, 3f, out var splineResult, out var distResult))
		{
			track = splineResult;
			FrontWheelSplineDist = distResult;
			track.RegisterTrackUser(this);
		}
	}

	internal override void DoServerDestroy()
	{
		if ((Object)(object)track != (Object)null)
		{
			track.DeregisterTrackUser(this);
		}
		base.DoServerDestroy();
	}

	private void SetHitTrain(TrainCar train, TriggerTrainCollisions trainTrigger)
	{
		hitTrain = train;
		hitTrainTrigger = trainTrigger;
	}

	private void ClearHitTrain()
	{
		SetHitTrain(null, null);
	}

	private void DestroyThisBarrier()
	{
		if (IsDead() || base.IsDestroyed)
		{
			return;
		}
		if ((Object)(object)hitTrain != (Object)null)
		{
			hitTrain.completeTrain.ReduceSpeedBy(velReduction);
			if (vehicle.cinematictrains)
			{
				hitTrain.Hurt(9999f, DamageType.Collision, this, useProtection: false);
			}
			else
			{
				float amount = Mathf.Abs(hitTrain.GetTrackSpeed()) * trainDamagePerMPS;
				hitTrain.Hurt(amount, DamageType.Collision, this, useProtection: false);
			}
		}
		ClearHitTrain();
		Kill(DestroyMode.Gib);
	}

	private void PushForceTick()
	{
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)hitTrain == (Object)null || (Object)(object)hitTrainTrigger == (Object)null || hitTrain.IsDead() || hitTrain.IsDestroyed || IsDead())
		{
			ClearHitTrain();
			((FacepunchBehaviour)this).CancelInvoke((Action)PushForceTick);
			return;
		}
		bool flag = true;
		Bounds val = hitTrainTrigger.triggerCollider.bounds;
		if (!((Bounds)(ref val)).Intersects(bounds))
		{
			Vector3 val2 = ((hitTrainTrigger.location != 0) ? hitTrainTrigger.owner.GetRearOfTrainPos() : hitTrainTrigger.owner.GetFrontOfTrainPos());
			Vector3 val3 = ((Component)this).transform.position + ((Bounds)(ref bounds)).ClosestPoint(val2 - ((Component)this).transform.position);
			Debug.DrawRay(val3, Vector3.up, Color.red, 10f);
			float num = Vector3.SqrMagnitude(val3 - val2);
			flag = num < 1f;
		}
		if (flag)
		{
			float num2 = hitTrainTrigger.owner.completeTrain.TotalForces;
			if (hitTrainTrigger.location == TriggerTrainCollisions.Location.Rear)
			{
				num2 *= -1f;
			}
			num2 = Mathf.Max(0f, num2);
			Hurt(0.002f * num2);
			if (IsDead())
			{
				hitTrain.completeTrain.FreeStaticCollision();
			}
		}
		else
		{
			ClearHitTrain();
			((FacepunchBehaviour)this).CancelInvoke((Action)PushForceTick);
		}
	}
}
