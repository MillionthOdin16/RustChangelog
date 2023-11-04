using UnityEngine;
using UnityEngine.Profiling;

public class EntityCollisionMessage : EntityComponent<BaseEntity>
{
	private void OnCollisionEnter(Collision collision)
	{
		if ((Object)(object)base.baseEntity == (Object)null || base.baseEntity.IsDestroyed)
		{
			return;
		}
		Profiler.BeginSample("GetEntity");
		BaseEntity baseEntity = collision.GetEntity();
		Profiler.EndSample();
		if ((Object)(object)baseEntity == (Object)(object)base.baseEntity)
		{
			return;
		}
		if ((Object)(object)baseEntity != (Object)null)
		{
			if (baseEntity.IsDestroyed)
			{
				return;
			}
			if (base.baseEntity.isServer)
			{
				baseEntity = baseEntity.ToServer<BaseEntity>();
			}
		}
		Profiler.BeginSample("baseEntity.OnCollision");
		base.baseEntity.OnCollision(collision, baseEntity);
		Profiler.EndSample();
	}
}
