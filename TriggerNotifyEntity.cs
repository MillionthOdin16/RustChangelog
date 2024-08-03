using UnityEngine;

public class TriggerNotifyEntity : TriggerBase, IPrefabPreProcess
{
	public GameObject notifyTarget;

	private INotifyEntityTrigger toNotify = null;

	public bool runClientside = true;

	public bool runServerside = true;

	public bool HasContents => contents != null && contents.Count > 0;

	internal override void OnEntityEnter(BaseEntity ent)
	{
		base.OnEntityEnter(ent);
		if (toNotify != null || ((Object)(object)notifyTarget != (Object)null && notifyTarget.TryGetComponent<INotifyEntityTrigger>(ref toNotify)))
		{
			toNotify.OnEntityEnter(ent);
		}
	}

	internal override void OnEntityLeave(BaseEntity ent)
	{
		base.OnEntityLeave(ent);
		if (toNotify != null || ((Object)(object)notifyTarget != (Object)null && notifyTarget.TryGetComponent<INotifyEntityTrigger>(ref toNotify)))
		{
			toNotify.OnEntityLeave(ent);
		}
	}

	public void PreProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		if ((!clientside || !runClientside) && (!serverside || !runServerside))
		{
			preProcess.RemoveComponent((Component)(object)this);
		}
	}
}
