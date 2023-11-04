using System;
using ConVar;
using Rust;
using UnityEngine;
using UnityEngine.Profiling;

public static class BaseMetabolism
{
	public const float targetHeartRate = 0.05f;
}
public abstract class BaseMetabolism<T> : EntityComponent<T> where T : BaseCombatEntity
{
	protected T owner;

	public MetabolismAttribute calories = new MetabolismAttribute();

	public MetabolismAttribute hydration = new MetabolismAttribute();

	public MetabolismAttribute heartrate = new MetabolismAttribute();

	protected float timeSinceLastMetabolism;

	public virtual void Reset()
	{
		calories.Reset();
		hydration.Reset();
		heartrate.Reset();
	}

	protected virtual void OnDisable()
	{
		if (!Application.isQuitting)
		{
			owner = null;
		}
	}

	public virtual void ServerInit(T owner)
	{
		Reset();
		this.owner = owner;
	}

	public virtual void ServerUpdate(BaseCombatEntity ownerEntity, float delta)
	{
		timeSinceLastMetabolism += delta;
		if (!(timeSinceLastMetabolism <= ConVar.Server.metabolismtick))
		{
			if (Object.op_Implicit((Object)(object)owner) && !owner.IsDead())
			{
				Profiler.BeginSample("RunMetabolism");
				RunMetabolism(ownerEntity, timeSinceLastMetabolism);
				Profiler.EndSample();
				Profiler.BeginSample("DoMetabolismDamage");
				DoMetabolismDamage(ownerEntity, timeSinceLastMetabolism);
				Profiler.EndSample();
			}
			timeSinceLastMetabolism = 0f;
		}
	}

	protected virtual void DoMetabolismDamage(BaseCombatEntity ownerEntity, float delta)
	{
		if (calories.value <= 20f)
		{
			TimeWarning val = TimeWarning.New("Calories Hurt", 0);
			try
			{
				ownerEntity.Hurt(Mathf.InverseLerp(20f, 0f, calories.value) * delta * (1f / 12f), DamageType.Hunger);
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
		if (hydration.value <= 20f)
		{
			TimeWarning val2 = TimeWarning.New("Hyration Hurt", 0);
			try
			{
				ownerEntity.Hurt(Mathf.InverseLerp(20f, 0f, hydration.value) * delta * (2f / 15f), DamageType.Thirst);
			}
			finally
			{
				((IDisposable)val2)?.Dispose();
			}
		}
	}

	protected virtual void RunMetabolism(BaseCombatEntity ownerEntity, float delta)
	{
		if (calories.value > 200f)
		{
			ownerEntity.Heal(Mathf.InverseLerp(200f, 1000f, calories.value) * delta * (1f / 60f));
		}
		if (hydration.value > 200f)
		{
			ownerEntity.Heal(Mathf.InverseLerp(200f, 1000f, hydration.value) * delta * (1f / 60f));
		}
		hydration.MoveTowards(0f, delta * (1f / 120f));
		calories.MoveTowards(0f, delta * (1f / 60f));
		heartrate.MoveTowards(0.05f, delta * (1f / 60f));
	}

	public void ApplyChange(MetabolismAttribute.Type type, float amount, float time)
	{
		FindAttribute(type)?.Add(amount);
	}

	public bool ShouldDie()
	{
		return Object.op_Implicit((Object)(object)owner) && owner.Health() <= 0f;
	}

	public virtual MetabolismAttribute FindAttribute(MetabolismAttribute.Type type)
	{
		return type switch
		{
			MetabolismAttribute.Type.Calories => calories, 
			MetabolismAttribute.Type.Hydration => hydration, 
			MetabolismAttribute.Type.Heartrate => heartrate, 
			_ => null, 
		};
	}
}
