using System;
using System.Collections.Generic;
using ConVar;
using UnityEngine;

public class RadiationSphere : BaseEntity
{
	private struct RadiationLight
	{
		public IOEntity Light;

		public Color OriginalColor;
	}

	public AnimationCurve RadiationCurve;

	public float InvokeDelay = 5f;

	public List<IOEntity> RadiationLights = new List<IOEntity>();

	private float timeStarted;

	private TriggerRadiation[] radiationTriggers;

	public override void ServerInit()
	{
		base.ServerInit();
		radiationTriggers = ((Component)this).GetComponentsInChildren<TriggerRadiation>();
		((FacepunchBehaviour)this).InvokeRandomized((Action)UpdateRadiation, InvokeDelay, InvokeDelay, InvokeDelay / 10f);
	}

	public void RestartRadiation()
	{
		timeStarted = Time.time;
	}

	public void StopRadation()
	{
		timeStarted = 0f;
	}

	public void UpdateRadiation()
	{
		if (timeStarted == 0f)
		{
			return;
		}
		float num = RadiationCurve.Evaluate((Time.time - timeStarted) / 60f * Server.oilrig_radiation_time_scale) * Server.oilrig_radiation_amount_scale;
		TriggerRadiation[] array = radiationTriggers;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].RadiationAmountOverride = num;
		}
		foreach (IOEntity radiationLight in RadiationLights)
		{
			if (!((Object)(object)radiationLight == (Object)null))
			{
				radiationLight.SetFlag(Flags.Reserved8, num > Server.oilrig_radiation_alarm_threshold);
			}
		}
	}

	public override void OnEntityMessage(BaseEntity from, string msg)
	{
		if (msg == "HackingStarted")
		{
			RestartRadiation();
		}
	}

	public void OnPuzzleReset()
	{
		StopRadation();
	}
}
