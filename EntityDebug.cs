using System.Diagnostics;
using UnityEngine;

public class EntityDebug : EntityComponent<BaseEntity>
{
	internal Stopwatch stopwatch = Stopwatch.StartNew();

	private void Update()
	{
		if (!base.baseEntity.IsValid() || !base.baseEntity.IsDebugging())
		{
			((Behaviour)this).enabled = false;
		}
		else if (!(stopwatch.Elapsed.TotalSeconds < 0.5))
		{
			if (base.baseEntity.isClient)
			{
			}
			if (base.baseEntity.isServer)
			{
				base.baseEntity.DebugServer(1, (float)stopwatch.Elapsed.TotalSeconds);
			}
			stopwatch.Reset();
			stopwatch.Start();
		}
	}
}
