using System;
using Facepunch;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Profiling;

public class ElectricWindmill : IOEntity
{
	public Animator animator;

	public int maxPowerGeneration = 100;

	public Transform vaneRot;

	public SoundDefinition wooshSound;

	public Transform wooshOrigin;

	public float targetSpeed = 0f;

	private float serverWindSpeed;

	public override int MaximalPowerOutput()
	{
		return maxPowerGeneration;
	}

	public override bool IsRootEntity()
	{
		return true;
	}

	public float GetWindSpeedScale()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		float num = Time.time / 600f;
		float num2 = ((Component)this).transform.position.x / 512f;
		float num3 = ((Component)this).transform.position.z / 512f;
		float num4 = Mathf.PerlinNoise(num2 + num, num3 + num * 0.1f);
		float height = TerrainMeta.HeightMap.GetHeight(((Component)this).transform.position);
		float num5 = ((Component)this).transform.position.y - height;
		if (num5 < 0f)
		{
			num5 = 0f;
		}
		float num6 = Mathf.InverseLerp(0f, 50f, num5);
		return Mathf.Clamp01(num6 * 0.5f + num4);
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
	}

	public override void ServerInit()
	{
		base.ServerInit();
		((FacepunchBehaviour)this).InvokeRandomized((Action)WindUpdate, 1f, 20f, 2f);
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		if (!info.forDisk)
		{
			if (info.msg.ioEntity == null)
			{
				info.msg.ioEntity = Pool.Get<IOEntity>();
			}
			info.msg.ioEntity.genericFloat1 = Time.time;
			info.msg.ioEntity.genericFloat2 = serverWindSpeed;
		}
	}

	public bool AmIVisible()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		int num = 15;
		Vector3 val = ((Component)this).transform.position + Vector3.up * 6f;
		if (!IsVisible(val + ((Component)this).transform.up * (float)num, (float)(num + 1)))
		{
			return false;
		}
		Vector3 windAimDir = GetWindAimDir(Time.time);
		if (!IsVisible(val + windAimDir * (float)num, (float)(num + 1)))
		{
			return false;
		}
		return true;
	}

	public void WindUpdate()
	{
		Profiler.BeginSample("ElectricWindmill.WindUpdate");
		serverWindSpeed = GetWindSpeedScale();
		if (!AmIVisible())
		{
			serverWindSpeed = 0f;
		}
		int num = Mathf.FloorToInt((float)maxPowerGeneration * serverWindSpeed);
		bool flag = currentEnergy != num;
		currentEnergy = num;
		if (flag)
		{
			MarkDirty();
		}
		SendNetworkUpdate();
		Profiler.EndSample();
	}

	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		return (outputSlot == 0) ? currentEnergy : 0;
	}

	public Vector3 GetWindAimDir(float time)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		float num = time / 3600f;
		float num2 = num * 360f;
		int num3 = 10;
		Vector3 val = default(Vector3);
		((Vector3)(ref val))._002Ector(Mathf.Sin(num2 * ((float)Math.PI / 180f)) * (float)num3, 0f, Mathf.Cos(num2 * ((float)Math.PI / 180f)) * (float)num3);
		return ((Vector3)(ref val)).normalized;
	}
}
