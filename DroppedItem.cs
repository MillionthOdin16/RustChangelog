using System;
using ConVar;
using Facepunch.Rust;
using UnityEngine;

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

	[NonSerialized]
	public DropReasonEnum DropReason;

	[NonSerialized]
	public ulong DroppedBy;

	private Rigidbody rB;

	private int originalLayer = -1;

	private CollisionDetectionMode originalCollisionMode;

	private Vector3 prevLocalPos;

	private bool stuckInSomething;

	public const int INTERACTION_ONLY_LAYER = 19;

	private const float SLEEP_CHECK_FREQUENCY = 15f;

	private bool hasLastPos;

	private Vector3 lastGoodColliderCentre;

	private Vector3 lastGoodPos;

	private Quaternion lastGoodRot;

	private Action cachedSleepCheck;

	private float maxBoundsExtent;

	public override float GetNetworkTime()
	{
		return Time.fixedTime;
	}

	public override void ServerInit()
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		base.ServerInit();
		if (GetDespawnDuration() < float.PositiveInfinity)
		{
			((FacepunchBehaviour)this).Invoke((Action)IdleDestroy, GetDespawnDuration());
		}
		ReceiveCollisionMessages(b: true);
		prevLocalPos = ((Component)this).transform.localPosition;
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
				droppedItem.OnDroppedOn(this);
			}
		}
	}

	public void OnDroppedOn(DroppedItem di)
	{
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
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
			int worldModelIndex = item.info.GetWorldModelIndex(item.amount);
			item.amount = num3;
			item.MarkDirty();
			if (GetDespawnDuration() < float.PositiveInfinity)
			{
				((FacepunchBehaviour)this).Invoke((Action)IdleDestroy, GetDespawnDuration());
			}
			Effect.server.Run("assets/bundled/prefabs/fx/notice/stack.world.fx.prefab", this, 0u, Vector3.zero, Vector3.zero);
			int worldModelIndex2 = item.info.GetWorldModelIndex(item.amount);
			if (worldModelIndex != worldModelIndex2)
			{
				item.Drop(((Component)this).transform.position, Vector3.zero, ((Component)this).transform.rotation);
			}
		}
	}

	public override void OnParentChanging(BaseEntity oldParent, BaseEntity newParent)
	{
		base.OnParentChanging(oldParent, newParent);
		if ((Object)(object)newParent != (Object)null && (Object)(object)newParent != (Object)(object)oldParent)
		{
			OnParented();
		}
		else if ((Object)(object)newParent == (Object)null && (Object)(object)oldParent != (Object)null)
		{
			OnUnparented();
		}
	}

	internal override void OnParentRemoved()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
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
		Unstick();
		if (GetDespawnDuration() < float.PositiveInfinity)
		{
			((FacepunchBehaviour)this).Invoke((Action)IdleDestroy, GetDespawnDuration());
		}
	}

	public void StickIn()
	{
		stuckInSomething = true;
		BecomeInactive();
	}

	public void Unstick()
	{
		stuckInSomething = false;
		BecomeActive();
	}

	private void BecomeActive()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		rB.isKinematic = false;
		rB.collisionDetectionMode = originalCollisionMode;
		rB.WakeUp();
		if (HasParent())
		{
			Rigidbody component = ((Component)GetParentEntity()).GetComponent<Rigidbody>();
			if ((Object)(object)component != (Object)null)
			{
				rB.velocity = component.velocity;
				rB.angularVelocity = component.angularVelocity;
			}
		}
		if ((Object)(object)childCollider != (Object)null)
		{
			((Component)childCollider).gameObject.layer = originalLayer;
		}
		prevLocalPos = ((Component)this).transform.localPosition;
	}

	private void BecomeInactive()
	{
		rB.collisionDetectionMode = (CollisionDetectionMode)0;
		rB.isKinematic = true;
		if ((Object)(object)childCollider != (Object)null)
		{
			((Component)childCollider).gameObject.layer = 19;
		}
	}

	private void SleepCheck()
	{
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		if (!HasParent() || stuckInSomething)
		{
			return;
		}
		if (rB.isKinematic)
		{
			if (maxBoundsExtent == 0f)
			{
				float num;
				if (!((Object)(object)childCollider != (Object)null))
				{
					num = Vector3Ex.Max(((Bounds)(ref bounds)).extents);
				}
				else
				{
					Bounds val = childCollider.bounds;
					num = Vector3Ex.Max(((Bounds)(ref val)).extents);
				}
				maxBoundsExtent = num;
			}
			if (!GamePhysics.Trace(new Ray(CenterPoint(), Vector3.down), 0f, out var _, maxBoundsExtent + 0.1f, -928830719, (QueryTriggerInteraction)1, this))
			{
				BecomeActive();
			}
		}
		else
		{
			float num2 = 0.05f;
			if (Vector3.SqrMagnitude(((Component)this).transform.localPosition - prevLocalPos) < num2)
			{
				BecomeInactive();
			}
		}
		prevLocalPos = ((Component)this).transform.localPosition;
	}

	private void OnPhysicsNeighbourChanged()
	{
		if (!stuckInSomething)
		{
			BecomeActive();
		}
	}

	protected override bool TransformHasMoved()
	{
		if (base.TransformHasMoved())
		{
			return !rB.isKinematic;
		}
		return false;
	}

	public override void OnPositionalNetworkUpdate()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)rB != (Object)null && (Object)(object)childCollider != (Object)null)
		{
			Bounds val = childCollider.bounds;
			Vector3 center = ((Bounds)(ref val)).center;
			Vector3 val2 = center - lastGoodColliderCentre;
			Ray ray = default(Ray);
			((Ray)(ref ray))._002Ector(lastGoodColliderCentre, ((Vector3)(ref val2)).normalized);
			if (hasLastPos && GamePhysics.Trace(ray, 0f, out var _, ((Vector3)(ref val2)).magnitude, 1084293377, (QueryTriggerInteraction)1, this))
			{
				((Component)this).transform.position = lastGoodPos;
				((Component)this).transform.rotation = lastGoodRot;
				rB.velocity = Vector3.zero;
				rB.angularVelocity = Vector3.zero;
				Physics.SyncTransforms();
			}
			else
			{
				lastGoodColliderCentre = center;
				lastGoodPos = ((Component)this).transform.position;
				lastGoodRot = ((Component)this).transform.rotation;
				hasLastPos = true;
			}
		}
	}

	private void OnUnparented()
	{
		if (cachedSleepCheck != null)
		{
			((FacepunchBehaviour)this).CancelInvoke(cachedSleepCheck);
		}
	}

	private void OnParented()
	{
		if ((Object)(object)childCollider == (Object)null)
		{
			return;
		}
		if (Object.op_Implicit((Object)(object)childCollider))
		{
			childCollider.enabled = false;
			((FacepunchBehaviour)this).Invoke((Action)EnableCollider, 0.1f);
		}
		if (base.isServer && !stuckInSomething)
		{
			if (cachedSleepCheck == null)
			{
				cachedSleepCheck = SleepCheck;
			}
			((FacepunchBehaviour)this).InvokeRandomized(cachedSleepCheck, 15f, 15f, Random.Range(-1.5f, 1.5f));
		}
	}

	public override void PostInitShared()
	{
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		base.PostInitShared();
		GameObject val = null;
		val = ((item == null || !item.GetWorldModel().isValid) ? Object.Instantiate<GameObject>(itemModel) : item.GetWorldModel().Instantiate());
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
			originalLayer = ((Component)childCollider).gameObject.layer;
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
			rB.interpolation = (RigidbodyInterpolation)0;
			rB.collisionDetectionMode = (CollisionDetectionMode)3;
			originalCollisionMode = rB.collisionDetectionMode;
			Renderer[] componentsInChildren = val.GetComponentsInChildren<Renderer>(true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].enabled = false;
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
