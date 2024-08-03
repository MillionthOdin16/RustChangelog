using System;
using ConVar;
using Facepunch.Rust;
using UnityEngine;

public class DroppedItem : WorldItem, IContainerSounds
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

	private const Flags FLAG_STUCK = Flags.Reserved1;

	private int originalLayer = -1;

	[NonSerialized]
	public DropReasonEnum DropReason;

	[NonSerialized]
	public ulong DroppedBy;

	[NonSerialized]
	public DateTime DroppedTime;

	[NonSerialized]
	public bool NeverCombine;

	private Rigidbody rB;

	private CollisionDetectionMode originalCollisionMode;

	private Vector3 prevLocalPos;

	private const float SLEEP_CHECK_FREQUENCY = 11f;

	private bool hasLastPos;

	private Vector3 lastGoodColliderCentre;

	private Vector3 lastGoodPos;

	private Quaternion lastGoodRot;

	private Action cachedSleepCheck;

	private float maxBoundsExtent;

	private readonly Vector3 smallVerticalOffset = new Vector3(0f, 0.05f, 0f);

	private bool StuckInSomething => HasFlag(Flags.Reserved1);

	public SoundDefinition OpenSound
	{
		get
		{
			if (item == null)
			{
				return null;
			}
			ItemModContainer component = ((Component)item.info).GetComponent<ItemModContainer>();
			if ((Object)(object)component == (Object)null)
			{
				return null;
			}
			return component.openSound;
		}
	}

	public SoundDefinition CloseSound
	{
		get
		{
			if (item == null)
			{
				return null;
			}
			ItemModContainer component = ((Component)item.info).GetComponent<ItemModContainer>();
			if ((Object)(object)component == (Object)null)
			{
				return null;
			}
			return component.closeSound;
		}
	}

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
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_027f: Unknown result type (might be due to invalid IL or missing references)
		if (item == null || di.item == null || (Object)(object)di.item.info != (Object)(object)item.info || (di.item.IsBlueprint() && di.item.blueprintTarget != item.blueprintTarget) || NeverCombine || di.NeverCombine || (di.item.hasCondition && di.item.condition != di.item.maxCondition) || (item.hasCondition && item.condition != item.maxCondition))
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
		SetFlag(Flags.Reserved1, b: true);
	}

	public void Unstick()
	{
		SetFlag(Flags.Reserved1, b: false);
	}

	private void SleepCheck()
	{
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		if (!HasParent() || StuckInSomething)
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
		else if (Vector3.SqrMagnitude(((Component)this).transform.localPosition - prevLocalPos) < 0.075f)
		{
			BecomeInactive();
		}
		prevLocalPos = ((Component)this).transform.localPosition;
	}

	private void OnPhysicsNeighbourChanged()
	{
		if (!StuckInSomething)
		{
			BecomeActive();
		}
	}

	public override void OnPositionalNetworkUpdate()
	{
		base.OnPositionalNetworkUpdate();
		CheckValidPosition();
	}

	protected override bool ShouldUpdateNetworkPosition()
	{
		if (syncPosition)
		{
			return !rB.isKinematic;
		}
		return false;
	}

	private void CheckValidPosition()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		if (!((Object)(object)rB != (Object)null) || !((Object)(object)childCollider != (Object)null))
		{
			return;
		}
		Bounds val = childCollider.bounds;
		Vector3 val2 = ((Bounds)(ref val)).center + smallVerticalOffset;
		Vector3 val3 = val2 - lastGoodColliderCentre;
		Ray ray = default(Ray);
		((Ray)(ref ray))._002Ector(lastGoodColliderCentre, ((Vector3)(ref val3)).normalized);
		if (hasLastPos && GamePhysics.Trace(ray, 0f, out var _, ((Vector3)(ref val3)).magnitude, 1218511105, (QueryTriggerInteraction)1, this))
		{
			((Component)this).transform.position = lastGoodPos + smallVerticalOffset;
			((Component)this).transform.rotation = lastGoodRot;
			if (!rB.isKinematic)
			{
				rB.velocity = Vector3.zero;
				rB.angularVelocity = Vector3.zero;
			}
			Physics.SyncTransforms();
		}
		else
		{
			lastGoodColliderCentre = val2;
			lastGoodPos = ((Component)this).transform.position;
			lastGoodRot = ((Component)this).transform.rotation;
			hasLastPos = true;
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
		if (base.isServer && !StuckInSomething)
		{
			if (cachedSleepCheck == null)
			{
				cachedSleepCheck = SleepCheck;
			}
			((FacepunchBehaviour)this).InvokeRandomized(cachedSleepCheck, 5.5f, 11f, Random.Range(-1.1f, 1.1f));
		}
	}

	public override void PostInitShared()
	{
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
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
			float drag = 0.1f;
			float angularDrag = 0.1f;
			rB = ((Component)this).gameObject.AddComponent<Rigidbody>();
			UpdateItemMass();
			rB.drag = drag;
			rB.angularDrag = angularDrag;
			rB.interpolation = (RigidbodyInterpolation)0;
			rB.collisionDetectionMode = (CollisionDetectionMode)3;
			originalCollisionMode = rB.collisionDetectionMode;
			rB.sleepThreshold = Mathf.Max(0.05f, Physics.sleepThreshold);
			Renderer[] componentsInChildren = val.GetComponentsInChildren<Renderer>(true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].enabled = false;
			}
			CheckValidPosition();
		}
		if (item != null)
		{
			PhysicsEffects component = ((Component)this).gameObject.GetComponent<PhysicsEffects>();
			if ((Object)(object)component != (Object)null)
			{
				component.entity = this;
				if ((Object)(object)item.info.physImpactSoundDef != (Object)null)
				{
					component.physImpactSoundDef = item.info.physImpactSoundDef;
				}
			}
			Buoyancy component2 = val.GetComponent<Buoyancy>();
			if ((Object)(object)component2 != (Object)null && base.isServer)
			{
				component2.rigidBody = rB;
			}
		}
		val.SetActive(true);
	}

	public override void OnFlagsChanged(Flags old, Flags next)
	{
		base.OnFlagsChanged(old, next);
		if (!old.HasFlag(Flags.Reserved1) && next.HasFlag(Flags.Reserved1))
		{
			BecomeInactive();
		}
		else if (old.HasFlag(Flags.Reserved1) && !next.HasFlag(Flags.Reserved1))
		{
			BecomeActive();
		}
	}

	private void BecomeActive()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		if (base.isServer)
		{
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
			prevLocalPos = ((Component)this).transform.localPosition;
		}
		if ((Object)(object)childCollider != (Object)null)
		{
			((Component)childCollider).gameObject.layer = originalLayer;
		}
	}

	private void BecomeInactive()
	{
		if (base.isServer)
		{
			rB.collisionDetectionMode = (CollisionDetectionMode)0;
			rB.isKinematic = true;
		}
		if ((Object)(object)childCollider != (Object)null)
		{
			((Component)childCollider).gameObject.layer = 19;
		}
	}

	private void EnableCollider()
	{
		if (Object.op_Implicit((Object)(object)childCollider))
		{
			childCollider.enabled = true;
		}
	}

	public void UpdateItemMass()
	{
		if ((Object)(object)rB == (Object)null)
		{
			rB = ((Component)this).GetComponent<Rigidbody>();
		}
		if ((Object)(object)rB == (Object)null || item == null || item.contents?.itemList == null)
		{
			return;
		}
		float num = item.info.GetWorldModelMass();
		ItemModContainer component = ((Component)item.info).GetComponent<ItemModContainer>();
		if ((Object)(object)component != (Object)null)
		{
			_ = component.worldWeightScale;
		}
		foreach (Item item in item.contents.itemList)
		{
			num += item.info.GetWorldModelMass() * component.worldWeightScale;
		}
		if ((Object)(object)component != (Object)null && component.maxWeight > 0f)
		{
			num = Mathf.Min(component.maxWeight, num);
		}
		rB.mass = num;
	}

	public override bool ShouldInheritNetworkGroup()
	{
		return false;
	}
}
