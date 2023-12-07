using UnityEngine;

public class TriggerWorkbench : TriggerBase
{
	public Workbench parentBench;

	internal override GameObject InterestedInObject(GameObject obj)
	{
		obj = base.InterestedInObject(obj);
		if ((Object)(object)obj == (Object)null)
		{
			return null;
		}
		BaseEntity baseEntity = obj.ToBaseEntity();
		if ((Object)(object)baseEntity == (Object)null)
		{
			return null;
		}
		if (baseEntity.isClient)
		{
			return null;
		}
		return ((Component)baseEntity).gameObject;
	}

	public float WorkbenchLevel()
	{
		return parentBench.Workbenchlevel;
	}

	internal override void OnEntityEnter(BaseEntity ent)
	{
		base.OnEntityEnter(ent);
		if (ent is BasePlayer basePlayer)
		{
			basePlayer.metabolism.ForceUpdateWorkbenchFlags();
		}
	}

	internal override void OnEntityLeave(BaseEntity ent)
	{
		base.OnEntityLeave(ent);
		if (ent is BasePlayer basePlayer)
		{
			basePlayer.metabolism.ForceUpdateWorkbenchFlags();
		}
	}
}
