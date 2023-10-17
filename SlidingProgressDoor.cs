using System;
using Facepunch;
using ProtoBuf;
using UnityEngine;

public class SlidingProgressDoor : ProgressDoor
{
	public Vector3 openPosition;

	public Vector3 closedPosition;

	public GameObject doorObject;

	public TriggerVehiclePush vehiclePhysBox = null;

	private float lastEnergyTime = 0f;

	private float lastServerUpdateTime = 0f;

	public override void Spawn()
	{
		base.Spawn();
		((FacepunchBehaviour)this).InvokeRepeating((Action)ServerUpdate, 0f, 0.1f);
		if ((Object)(object)vehiclePhysBox != (Object)null)
		{
			((Component)vehiclePhysBox).gameObject.SetActive(false);
		}
	}

	public override void NoEnergy()
	{
		base.NoEnergy();
	}

	public override void AddEnergy(float amount)
	{
		lastEnergyTime = Time.time;
		base.AddEnergy(amount);
	}

	public void ServerUpdate()
	{
		if (!base.isServer)
		{
			return;
		}
		if (lastServerUpdateTime == 0f)
		{
			lastServerUpdateTime = Time.realtimeSinceStartup;
		}
		float num = Time.realtimeSinceStartup - lastServerUpdateTime;
		lastServerUpdateTime = Time.realtimeSinceStartup;
		if (Time.time > lastEnergyTime + 0.333f)
		{
			float num2 = energyForOpen * num / secondsToClose;
			float num3 = Mathf.Min(storedEnergy, num2);
			if ((Object)(object)vehiclePhysBox != (Object)null)
			{
				((Component)vehiclePhysBox).gameObject.SetActive(num3 > 0f && storedEnergy > 0f);
				if (((Component)vehiclePhysBox).gameObject.activeSelf && vehiclePhysBox.ContentsCount > 0)
				{
					num3 = 0f;
				}
			}
			storedEnergy -= num3;
			storedEnergy = Mathf.Clamp(storedEnergy, 0f, energyForOpen);
			if (num3 > 0f)
			{
				IOSlot[] array = outputs;
				foreach (IOSlot iOSlot in array)
				{
					if ((Object)(object)iOSlot.connectedTo.Get() != (Object)null)
					{
						iOSlot.connectedTo.Get().IOInput(this, ioType, 0f - num3, iOSlot.connectedToSlot);
					}
				}
			}
		}
		UpdateProgress();
	}

	public override void UpdateProgress()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		Vector3 localPosition = doorObject.transform.localPosition;
		float num = storedEnergy / energyForOpen;
		Vector3 val = Vector3.Lerp(closedPosition, openPosition, num);
		doorObject.transform.localPosition = val;
		if (base.isServer)
		{
			bool flag = Vector3.Distance(localPosition, val) > 0.01f;
			SetFlag(Flags.Reserved1, flag);
			if (flag)
			{
				SendNetworkUpdate();
			}
		}
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (info.msg.sphereEntity != null)
		{
		}
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		info.msg.sphereEntity = Pool.Get<SphereEntity>();
		info.msg.sphereEntity.radius = storedEnergy;
	}
}
