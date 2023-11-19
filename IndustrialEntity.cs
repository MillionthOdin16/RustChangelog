using UnityEngine;
using UnityEngine.Profiling;

public class IndustrialEntity : IOEntity
{
	public class IndustrialProcessQueue : ObjectWorkQueue<IndustrialEntity>
	{
		protected override void RunJob(IndustrialEntity job)
		{
			Profiler.BeginSample("IndustrialEntity.Jobs");
			if ((Object)(object)job != (Object)null)
			{
				Profiler.BeginSample(job.ShortPrefabName);
				job.RunJob();
				Profiler.EndSample();
			}
			Profiler.EndSample();
		}
	}

	public static IndustrialProcessQueue Queue = new IndustrialProcessQueue();

	protected virtual void RunJob()
	{
	}
}
