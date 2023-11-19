using System;
using System.Collections.Generic;
using UnityEngine;

public class TriggerMount : TriggerBase, IServerComponent
{
	private class EntryInfo
	{
		public float entryTime;

		public Vector3 entryPos;

		public EntryInfo(float entryTime, Vector3 entryPos)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			this.entryTime = entryTime;
			this.entryPos = entryPos;
		}

		public void Set(float entryTime, Vector3 entryPos)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			this.entryTime = entryTime;
			this.entryPos = entryPos;
		}
	}

	private const float MOUNT_DELAY = 3.5f;

	private const float MAX_MOVE = 0.5f;

	private Dictionary<BaseEntity, EntryInfo> entryInfo;

	internal override GameObject InterestedInObject(GameObject obj)
	{
		BaseEntity baseEntity = obj.ToBaseEntity();
		if ((Object)(object)baseEntity == (Object)null)
		{
			return null;
		}
		BasePlayer basePlayer = baseEntity.ToPlayer();
		if ((Object)(object)basePlayer == (Object)null || basePlayer.IsNpc)
		{
			return null;
		}
		return ((Component)baseEntity).gameObject;
	}

	internal override void OnEntityEnter(BaseEntity ent)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		base.OnEntityEnter(ent);
		if (entryInfo == null)
		{
			entryInfo = new Dictionary<BaseEntity, EntryInfo>();
		}
		entryInfo.Add(ent, new EntryInfo(Time.time, ((Component)ent).transform.position));
		((FacepunchBehaviour)this).Invoke((Action)CheckForMount, 3.6f);
	}

	internal override void OnEntityLeave(BaseEntity ent)
	{
		if ((Object)(object)ent != (Object)null && entryInfo != null)
		{
			entryInfo.Remove(ent);
		}
		base.OnEntityLeave(ent);
	}

	private void CheckForMount()
	{
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		if (entityContents == null || entryInfo == null)
		{
			return;
		}
		foreach (KeyValuePair<BaseEntity, EntryInfo> item in entryInfo)
		{
			BaseEntity key = item.Key;
			if (!key.IsValid())
			{
				continue;
			}
			EntryInfo value = item.Value;
			BasePlayer basePlayer = key.ToPlayer();
			bool flag = (basePlayer.IsAdmin || basePlayer.IsDeveloper) && basePlayer.IsFlying;
			if (!((Object)(object)basePlayer != (Object)null) || !basePlayer.IsAlive() || flag)
			{
				continue;
			}
			bool flag2 = false;
			if (!basePlayer.isMounted && !basePlayer.IsSleeping() && value.entryTime + 3.5f < Time.time && Vector3.Distance(((Component)key).transform.position, value.entryPos) < 0.5f)
			{
				BaseVehicle componentInParent = ((Component)this).GetComponentInParent<BaseVehicle>();
				if ((Object)(object)componentInParent != (Object)null && !componentInParent.IsDead())
				{
					componentInParent.AttemptMount(basePlayer);
					flag2 = true;
				}
			}
			if (!flag2)
			{
				value.Set(Time.time, ((Component)key).transform.position);
				((FacepunchBehaviour)this).Invoke((Action)CheckForMount, 3.6f);
			}
		}
	}
}
