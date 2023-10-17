using System;
using UnityEngine;

public class EntityComponent<T> : EntityComponentBase where T : BaseEntity
{
	[NonSerialized]
	private T _baseEntity;

	protected T baseEntity
	{
		get
		{
			if ((Object)(object)_baseEntity == (Object)null)
			{
				UpdateBaseEntity();
			}
			return _baseEntity;
		}
	}

	protected void UpdateBaseEntity()
	{
		if (Object.op_Implicit((Object)(object)this) && Object.op_Implicit((Object)(object)((Component)this).gameObject))
		{
			_baseEntity = ((Component)this).gameObject.ToBaseEntity() as T;
		}
	}

	protected override BaseEntity GetBaseEntity()
	{
		return baseEntity;
	}
}
