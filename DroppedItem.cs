using System;
using ConVar;
using Facepunch.Rust;
using UnityEngine;
using UnityEngine.Profiling;

public class DroppedItem : WorldItem
{
	public enum DropReasonEnum
	{
		Unknown,
		Player,
		Death,
		Loot
	}

	[Header("DroppedItem")]
	public GameObject itemModel;

	private Collider childCollider;

	private Rigidbody rB;

	private const int interactionOnlyLayer = 19;

	[NonSerialized]
	public DropReasonEnum DropReason;

	[NonSerialized]
	public ulong DroppedBy;

	public override float GetNetworkTime()
	{
		return Time.fixedTime;
	}

	public override void ServerInit()
	{
		base.ServerInit();
		if (GetDespawnDuration() < float.PositiveInfinity)
		{
			((FacepunchBehaviour)this).Invoke((Action)IdleDestroy, GetDespawnDuration());
		}
		ReceiveCollisionMessages(b: true);
	}

	public virtual float GetDespawnDuration()
	{
		return item?.GetDespawnDuration() ?? Server.itemdespawn;
	}

	public void IdleDestroy()
	{
		Analytics.Azure.OnItemDespawn(this, item, (int)DropReason, DroppedBy);
		DestroyItem();
		Kill();
	}

	public override void OnCollision(Collision collision, BaseEntity hitEntity)
	{
		if (item != null && item.MaxStackable() > 1)
		{
			DroppedItem droppedItem = hitEntity as DroppedItem;
			if (!((Object)(object)droppedItem == (Object)null) && droppedItem.item != null && !((Object)(object)droppedItem.item.info != (Object)(object)item.info))
			{
				Profiler.BeginSample("OnDroppedOn");
				droppedItem.OnDroppedOn(this);
				Profiler.EndSample();
			}
		}
	}

	public void OnDroppedOn(DroppedItem di)
	{
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		if (item == null || di.item == null || (Object)(object)di.item.info != (Object)(object)item.info || (di.item.IsBlueprint() && di.item.blueprintTarget != item.blueprintTarget) || (di.item.hasCondition && di.item.condition != di.item.maxCondition) || (item.hasCondition && item.condition != item.maxCondition))
		{
			return;
		}
		if ((Object)(object)di.item.info != (Object)null)
		{
			if (di.item.info.amountType == ItemDefinition.AmountType.Genetics)
			{
				int num = ((di.item.instanceData != null) ? di.item.instanceData.dataInt : (-1));
				int num2 = ((item.instanceData != null) ? item.instanceData.dataInt : (-1));
				if (num != num2)
				{
					return;
				}
			}
			if (((Object)(object)((Component)di.item.info).GetComponent<ItemModSign>() != (Object)null && (Object)(object)ItemModAssociatedEntity<SignContent>.GetAssociatedEntity(di.item) != (Object)null) || ((Object)(object)item.info != (Object)null && (Object)(object)((Component)item.info).GetComponent<ItemModSign>() != (Object)null && (Object)(object)ItemModAssociatedEntity<SignContent>.GetAssociatedEntity(item) != (Object)null))
			{
				return;
			}
		}
		int num3 = di.item.amount + item.amount;
		if (num3 <= item.MaxStackable() && num3 != 0)
		{
			if (di.DropReason == DropReasonEnum.Player)
			{
				DropReason = DropReasonEnum.Player;
			}
			di.DestroyItem();
			di.Kill();
			item.amount = num3;
			item.MarkDirty();
			if (GetDespawnDuration() < float.PositiveInfinity)
			{
				((FacepunchBehaviour)this).Invoke((Action)IdleDestroy, GetDespawnDuration());
			}
			Effect.server.Run("assets/bundled/prefabs/fx/notice/stack.world.fx.prefab", this, 0u, Vector3.zero, Vector3.zero);
		}
	}

	public override void OnParentChanging(BaseEntity oldParent, BaseEntity newParent)
	{
		base.OnParentChanging(oldParent, newParent);
		if ((Object)(object)newParent != (Object)null)
		{
			OnParented();
		}
		SetCollisionForParent(newParent);
	}

	private void SetCollisionForParent(BaseEntity parent)
	{
		if (!((Object)(object)rB == (Object)null))
		{
			if (parent.IsValid() && (Object)(object)((Component)parent).GetComponent<Rigidbody>() != (Object)null)
			{
				rB.collisionDetectionMode = (CollisionDetectionMode)3;
			}
			else
			{
				rB.collisionDetectionMode = (CollisionDetectionMode)2;
			}
		}
	}

	internal override void OnParentRemoved()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)rB == (Object)null)
		{
			base.OnParentRemoved();
			return;
		}
		Vector3 val = ((Component)this).transform.position;
		Quaternion rotation = ((Component)this).transform.rotation;
		SetParent(null);
		RaycastHit val2 = default(RaycastHit);
		if (Physics.Raycast(val + Vector3.up * 2f, Vector3.down, ref val2, 2f, 161546240) && val.y < ((RaycastHit)(ref val2)).point.y)
		{
			val += Vector3.up * 1.5f;
		}
		((Component)this).transform.position = val;
		((Component)this).transform.rotation = rotation;
		((Component)childCollider).gameObject.layer = ((Component)this).gameObject.layer;
		rB.isKinematic = false;
		rB.useGravity = true;
		rB.collisionDetectionMode = (CollisionDetectionMode)2;
		rB.WakeUp();
		if (GetDespawnDuration() < float.PositiveInfinity)
		{
			((FacepunchBehaviour)this).Invoke((Action)IdleDestroy, GetDespawnDuration());
		}
	}

	public void GoKinematic()
	{
		rB.isKinematic = true;
		if (Object.op_Implicit((Object)(object)childCollider))
		{
			((Component)childCollider).gameObject.layer = 19;
		}
	}

	public override void PostInitShared()
	{
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		base.PostInitShared();
		GameObject val = null;
		val = ((item == null || !item.info.worldModelPrefab.isValid) ? Object.Instantiate<GameObject>(itemModel) : item.info.worldModelPrefab.Instantiate());
		val.transform.SetParent(((Component)this).transform, false);
		val.transform.localPosition = Vector3.zero;
		val.transform.localRotation = Quaternion.identity;
		val.SetLayerRecursive(((Component)this).gameObject.layer);
		childCollider = val.GetComponentInChildren<Collider>();
		if (Object.op_Implicit((Object)(object)childCollider))
		{
			childCollider.enabled = false;
			if (HasParent())
			{
				OnParented();
			}
			else
			{
				childCollider.enabled = true;
			}
		}
		if (base.isServer)
		{
			WorldModel component = val.GetComponent<WorldModel>();
			float mass = (Object.op_Implicit((Object)(object)component) ? component.mass : 1f);
			float drag = 0.1f;
			float angularDrag = 0.1f;
			rB = ((Component)this).gameObject.AddComponent<Rigidbody>();
			rB.mass = mass;
			rB.drag = drag;
			rB.angularDrag = angularDrag;
			SetCollisionForParent(GetParentEntity());
			rB.interpolation = (RigidbodyInterpolation)0;
			Renderer[] componentsInChildren = val.GetComponentsInChildren<Renderer>(true);
			foreach (Renderer val2 in componentsInChildren)
			{
				val2.enabled = false;
			}
		}
		if (item != null)
		{
			PhysicsEffects component2 = ((Component)this).gameObject.GetComponent<PhysicsEffects>();
			if ((Object)(object)component2 != (Object)null)
			{
				component2.entity = this;
				if ((Object)(object)item.info.physImpactSoundDef != (Object)null)
				{
					component2.physImpactSoundDef = item.info.physImpactSoundDef;
				}
			}
		}
		val.SetActive(true);
	}

	private void OnParented()
	{
		if (!((Object)(object)childCollider == (Object)null) && Object.op_Implicit((Object)(object)childCollider))
		{
			childCollider.enabled = false;
			((FacepunchBehaviour)this).Invoke((Action)EnableCollider, 0.1f);
		}
	}

	private void EnableCollider()
	{
		if (Object.op_Implicit((Object)(object)childCollider))
		{
			childCollider.enabled = true;
		}
	}

	public override bool ShouldInheritNetworkGroup()
	{
		return false;
	}
}
