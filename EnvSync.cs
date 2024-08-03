using System;
using Facepunch;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Profiling;

public class EnvSync : PointEntity
{
	private const float syncInterval = 5f;

	private const float syncIntervalInv = 0.2f;

	public override void ServerInit()
	{
		base.ServerInit();
		((FacepunchBehaviour)this).InvokeRepeating((Action)UpdateNetwork, 5f, 5f);
	}

	private void UpdateNetwork()
	{
		SendNetworkUpdate();
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		Profiler.BeginSample("EnvSync.Save");
		info.msg.environment = Pool.Get<Environment>();
		if (Object.op_Implicit((Object)(object)TOD_Sky.Instance))
		{
			info.msg.environment.dateTime = TOD_Sky.Instance.Cycle.DateTime.ToBinary();
		}
		info.msg.environment.engineTime = Time.realtimeSinceStartup;
		Profiler.EndSample();
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (info.msg.environment != null && Object.op_Implicit((Object)(object)TOD_Sky.Instance) && base.isServer)
		{
			TOD_Sky.Instance.Cycle.DateTime = DateTime.FromBinary(info.msg.environment.dateTime);
		}
	}
}
