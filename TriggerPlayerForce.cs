using System;
using UnityEngine;

public class TriggerPlayerForce : TriggerBase, IServerComponent
{
	public BoxCollider triggerCollider;

	public float pushVelocity = 5f;

	public bool requireUpAxis = false;

	private const float HACK_DISABLE_TIME = 4f;

	internal override GameObject InterestedInObject(GameObject obj)
	{
		obj = base.InterestedInObject(obj);
		if ((Object)(object)obj == (Object)null)
		{
			return null;
		}
		BaseEntity baseEntity = obj.ToBaseEntity();
		if ((Object)(object)baseEntity != (Object)null)
		{
			return ((Component)baseEntity).gameObject;
		}
		return null;
	}

	internal override void OnObjects()
	{
		((FacepunchBehaviour)this).InvokeRepeating((Action)HackDisableTick, 0f, 3.75f);
	}

	internal override void OnEmpty()
	{
		base.OnEmpty();
		((FacepunchBehaviour)this).CancelInvoke((Action)HackDisableTick);
	}

	protected override void OnDisable()
	{
		((FacepunchBehaviour)this).CancelInvoke((Action)HackDisableTick);
		base.OnDisable();
	}

	internal override void OnEntityLeave(BaseEntity ent)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		base.OnEntityLeave(ent);
		ent.ApplyInheritedVelocity(Vector3.zero);
	}

	private void HackDisableTick()
	{
		if (entityContents == null || !((Behaviour)this).enabled)
		{
			return;
		}
		foreach (BaseEntity entityContent in entityContents)
		{
			if (IsInterested(entityContent))
			{
				BasePlayer basePlayer = entityContent.ToPlayer();
				if ((Object)(object)basePlayer != (Object)null && !basePlayer.IsNpc)
				{
					basePlayer.PauseVehicleNoClipDetection(4f);
					basePlayer.PauseSpeedHackDetection(4f);
				}
			}
		}
	}

	protected void FixedUpdate()
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		if (entityContents == null)
		{
			return;
		}
		foreach (BaseEntity entityContent in entityContents)
		{
			if ((!requireUpAxis || !(Vector3.Dot(((Component)entityContent).transform.up, ((Component)this).transform.up) < 0f)) && IsInterested(entityContent))
			{
				Vector3 velocity = GetPushVelocity(((Component)entityContent).gameObject);
				entityContent.ApplyInheritedVelocity(velocity);
			}
		}
	}

	private Vector3 GetPushVelocity(GameObject obj)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		Bounds bounds = ((Collider)triggerCollider).bounds;
		Vector3 val = -(((Bounds)(ref bounds)).center - obj.transform.position);
		((Vector3)(ref val)).Normalize();
		val.y = 0.2f;
		((Vector3)(ref val)).Normalize();
		return val * pushVelocity;
	}

	private bool IsInterested(BaseEntity entity)
	{
		if ((Object)(object)entity == (Object)null || entity.isClient)
		{
			return false;
		}
		BasePlayer basePlayer = entity.ToPlayer();
		if ((Object)(object)basePlayer != (Object)null)
		{
			if ((basePlayer.IsAdmin || basePlayer.IsDeveloper) && basePlayer.IsFlying)
			{
				return false;
			}
			return (Object)(object)basePlayer != (Object)null && basePlayer.IsAlive() && !basePlayer.isMounted;
		}
		return true;
	}
}
