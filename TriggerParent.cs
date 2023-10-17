using System;
using Rust;
using UnityEngine;

public class TriggerParent : TriggerBase, IServerComponent
{
	[Tooltip("Deparent if the parented entity clips into an obstacle")]
	[SerializeField]
	private bool doClippingCheck;

	[Tooltip("If deparenting via clipping, this will be used (if assigned) to also move the entity to a valid dismount position")]
	public BaseMountable associatedMountable;

	[Tooltip("Needed if the player might dismount inside the trigger and the trigger might be moving. Being mounting inside the trigger lets them dismount in local trigger-space, which means client and server will sync up.Otherwise the client/server delay can have them dismounting into invalid space.")]
	public bool parentMountedPlayers;

	[Tooltip("Sleepers don't have all the checks (e.g. clipping) that awake players get. If that might be a problem,sleeper parenting can be disabled. You'll need an associatedMountable though so that the sleeper can be dismounted.")]
	public bool parentSleepers = true;

	public bool ParentNPCPlayers;

	[Tooltip("If the player is already parented to something else, they'll switch over to another parent only if this is true")]
	public bool overrideOtherTriggers;

	public const int CLIP_CHECK_MASK = 1218511105;

	private BasePlayer killPlayerTemp;

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

	internal override void OnEntityEnter(BaseEntity ent)
	{
		if (!(ent is NPCPlayer) || ParentNPCPlayers)
		{
			if (ShouldParent(ent))
			{
				Parent(ent);
			}
			base.OnEntityEnter(ent);
			if (entityContents != null && entityContents.Count == 1)
			{
				((FacepunchBehaviour)this).InvokeRepeating((Action)OnTick, 0f, 0f);
			}
		}
	}

	internal override void OnEntityLeave(BaseEntity ent)
	{
		base.OnEntityLeave(ent);
		if (entityContents == null || entityContents.Count == 0)
		{
			((FacepunchBehaviour)this).CancelInvoke((Action)OnTick);
		}
		BasePlayer basePlayer = ent.ToPlayer();
		if (!parentSleepers || !((Object)(object)basePlayer != (Object)null) || !basePlayer.IsSleeping())
		{
			Unparent(ent);
		}
	}

	public virtual bool ShouldParent(BaseEntity ent, bool bypassOtherTriggerCheck = false)
	{
		if (!ent.canTriggerParent)
		{
			return false;
		}
		if (!bypassOtherTriggerCheck)
		{
			BaseEntity parentEntity = ent.GetParentEntity();
			if (!overrideOtherTriggers && parentEntity.IsValid() && (Object)(object)parentEntity != (Object)(object)((Component)this).gameObject.ToBaseEntity())
			{
				return false;
			}
		}
		if ((Object)(object)ent.FindTrigger<TriggerParentExclusion>() != (Object)null)
		{
			return false;
		}
		if (doClippingCheck && IsClipping(ent))
		{
			return false;
		}
		if (!parentMountedPlayers || !parentSleepers)
		{
			BasePlayer basePlayer = ent.ToPlayer();
			if ((Object)(object)basePlayer != (Object)null)
			{
				if (!parentMountedPlayers && basePlayer.isMounted)
				{
					return false;
				}
				if (!parentSleepers && basePlayer.IsSleeping())
				{
					return false;
				}
			}
		}
		return true;
	}

	protected void Parent(BaseEntity ent)
	{
		BaseEntity baseEntity = ((Component)this).gameObject.ToBaseEntity();
		if (!((Object)(object)ent.GetParentEntity() == (Object)(object)baseEntity) && !((Object)(object)baseEntity.GetParentEntity() == (Object)(object)ent))
		{
			ent.SetParent(((Component)this).gameObject.ToBaseEntity(), worldPositionStays: true, sendImmediate: true);
		}
	}

	protected void Unparent(BaseEntity ent)
	{
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)ent.GetParentEntity() != (Object)(object)((Component)this).gameObject.ToBaseEntity())
		{
			return;
		}
		if (ent.IsValid() && !ent.IsDestroyed)
		{
			TriggerParent triggerParent = ent.FindSuitableParent();
			if ((Object)(object)triggerParent != (Object)null && ((Component)triggerParent).gameObject.ToBaseEntity().IsValid())
			{
				triggerParent.Parent(ent);
				return;
			}
		}
		ent.SetParent(null, worldPositionStays: true, sendImmediate: true);
		BasePlayer basePlayer = ent.ToPlayer();
		if (!((Object)(object)basePlayer != (Object)null))
		{
			return;
		}
		basePlayer.PauseFlyHackDetection(5f);
		basePlayer.PauseSpeedHackDetection(5f);
		basePlayer.PauseVehicleNoClipDetection(5f);
		if ((Object)(object)associatedMountable != (Object)null && ((doClippingCheck && IsClipping(ent)) || basePlayer.IsSleeping()))
		{
			if (associatedMountable.GetDismountPosition(basePlayer, out var res))
			{
				basePlayer.MovePosition(res);
				basePlayer.SendNetworkUpdateImmediate();
				basePlayer.ClientRPCPlayer<Vector3>(null, basePlayer, "ForcePositionTo", res);
			}
			else
			{
				killPlayerTemp = basePlayer;
				((FacepunchBehaviour)this).Invoke((Action)KillPlayerDelayed, 0f);
			}
		}
	}

	private void KillPlayerDelayed()
	{
		if (killPlayerTemp.IsValid() && !killPlayerTemp.IsDead())
		{
			killPlayerTemp.Hurt(1000f, DamageType.Suicide, killPlayerTemp, useProtection: false);
		}
		killPlayerTemp = null;
	}

	private void OnTick()
	{
		if (entityContents == null)
		{
			return;
		}
		BaseEntity baseEntity = ((Component)this).gameObject.ToBaseEntity();
		if (!baseEntity.IsValid() || baseEntity.IsDestroyed)
		{
			return;
		}
		foreach (BaseEntity entityContent in entityContents)
		{
			if (entityContent.IsValid() && !entityContent.IsDestroyed)
			{
				if (ShouldParent(entityContent))
				{
					Parent(entityContent);
				}
				else
				{
					Unparent(entityContent);
				}
			}
		}
	}

	protected virtual bool IsClipping(BaseEntity ent)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return GamePhysics.CheckOBB(ent.WorldSpaceBounds(), 1218511105, (QueryTriggerInteraction)1);
	}
}
