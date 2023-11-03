using System;
using ConVar;
using Facepunch.Rust;
using Rust;
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

	private Rigidbody rB;

	private const int INTERACTION_ONLY_LAYER = 19;

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
		return item?.GetDespawnDuration() ?? ConVar.Server.itemdespawn;
	}

	public void IdleDestroy()
	{
		Analytics.Azure.OnItemDespawn(this, item, (int)DropReason, DroppedBy);
		DestroyItem();
		Kill();
	}

	public override void OnCollision(Collision collision, BaseEntity hitEntity)
	{
		if (item == null)
		{
			return;
		}
		if (collision.collider.IsOnLayer((Layer)23))
		{
			((FacepunchBehaviour)this).Invoke((Action)FellThroughTerrainInvoke, 0.33f);
		}
		if (item.MaxStackable() > 1)
		{
			DroppedItem droppedItem = hitEntity as DroppedItem;
			if (!((Object)(object)droppedItem == (Object)null) && droppedItem.item != null && !((Object)(object)droppedItem.item.info != (Object)(object)item.info))
			{
				droppedItem.OnDroppedOn(this);
			}
		}
	}

	private void FellThroughTerrainInvoke()
	{
		if (!CheckFellThroughTerrain())
		{
			((FacepunchBehaviour)this).Invoke((Action)FellThroughTerrainInvoke2, 0.5f);
		}
	}

	private void FellThroughTerrainInvoke2()
	{
		CheckFellThroughTerrain();
	}

	private bool CheckFellThroughTerrain()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		if (base.IsDestroyed)
		{
			return false;
		}
		if (AntiHack.TestInsideTerrain(((Component)this).transform.position))
		{
			float num = TerrainMeta.Position.y + TerrainMeta.Terrain.SampleHeight(((Component)this).transform.position);
			Vector3 position = ((Component)this).transform.position;
			position.y = num + ((Bounds)(ref bounds)).size.y + 0.05f;
			((Component)this).transform.position = position;
			rB.velocity = Vector3.zero;
			rB.angularVelocity = Vector3.zero;
			return true;
		}
		return false;
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
		if ((Object)(object)newParent != (Object)null)
		{
			OnParented();
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
		((Component)childCollider).gameObject.layer = ((Component)this).gameObject.layer;
		rB.isKinematic = false;
		rB.useGravity = true;
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

	protected override bool TransformHasMoved()
	{
		if (base.TransformHasMoved() && !rB.isKinematic)
		{
			return !rB.IsSleeping();
		}
		return false;
	}

	public override void PostInitShared()
	{
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
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
