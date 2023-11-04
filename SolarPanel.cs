using System;
using UnityEngine;
using UnityEngine.Profiling;

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
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		int num = currentEnergy;
		Profiler.BeginSample("SolarPanel.SunUpdate");
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
		bool flag = currentEnergy != num;
		currentEnergy = num;
		if (flag)
		{
			MarkDirty();
		}
		Profiler.EndSample();
	}

	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		return (outputSlot == 0) ? currentEnergy : 0;
	}
}
