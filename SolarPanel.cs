using System;
using UnityEngine;

public class SolarPanel : IOEntity
{
	public Transform sunSampler;

	private const int tickrateSeconds = 60;

	public int maximalPowerOutput = 10;

	public float dot_minimum = 0.1f;

	public float dot_maximum = 0.6f;

	public override bool IsRootEntity()
	{
		return true;
	}

	public override int MaximalPowerOutput()
	{
		return maximalPowerOutput;
	}

	public override int ConsumptionAmount()
	{
		return 0;
	}

	public override void ServerInit()
	{
		base.ServerInit();
		((FacepunchBehaviour)this).InvokeRandomized((Action)SunUpdate, 1f, 5f, 2f);
	}

	public void SunUpdate()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		int num = currentEnergy;
		if (TOD_Sky.Instance.IsNight)
		{
			num = 0;
		}
		else
		{
			Vector3 sunDirection = TOD_Sky.Instance.SunDirection;
			float num2 = Vector3.Dot(((Component)sunSampler).transform.forward, sunDirection);
			float num3 = Mathf.InverseLerp(dot_minimum, dot_maximum, num2);
			if (num3 > 0f && !IsVisible(((Component)sunSampler).transform.position + sunDirection * 100f, 101f))
			{
				num3 = 0f;
			}
			num = Mathf.FloorToInt((float)maximalPowerOutput * num3 * base.healthFraction);
		}
		bool num4 = currentEnergy != num;
		currentEnergy = num;
		if (num4)
		{
			MarkDirty();
		}
	}

	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		if (outputSlot != 0)
		{
			return 0;
		}
		return currentEnergy;
	}
}
