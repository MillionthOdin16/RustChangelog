using System;
using ConVar;
using Facepunch;
using ProtoBuf;
using UnityEngine;

public class BaseCorpse : BaseCombatEntity
{
	public GameObjectRef prefabRagdoll;

	public BaseEntity parentEnt;

	[NonSerialized]
	internal ResourceDispenser resourceDispenser;

	[NonSerialized]
	public SpawnGroup spawnGroup;

	public override TraitFlag Traits => base.Traits | TraitFlag.Food | TraitFlag.Meat;

	public override void ResetState()
	{
		spawnGroup = null;
		base.ResetState();
	}

	public override void ServerInit()
	{
		base.ServerInit();
		SetupRigidBody();
		ResetRemovalTime();
		resourceDispenser = ((Component)this).GetComponent<ResourceDispenser>();
	}

	public virtual void ServerInitCorpse(BaseEntity pr, Vector3 posOnDeah, Quaternion rotOnDeath, BasePlayer.PlayerFlags playerFlagsOnDeath, ModelState modelState)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		parentEnt = pr;
		((Component)this).transform.SetPositionAndRotation(parentEnt.CenterPoint(), ((Component)parentEnt).transform.rotation);
		SpawnPointInstance component = ((Component)this).GetComponent<SpawnPointInstance>();
		if ((Object)(object)component != (Object)null)
		{
			spawnGroup = component.parentSpawnPointUser as SpawnGroup;
		}
	}

	public virtual bool CanRemove()
	{
		return true;
	}

	public void RemoveCorpse()
	{
		if (!CanRemove())
		{
			ResetRemovalTime();
		}
		else
		{
			Kill();
		}
	}

	public void ResetRemovalTime(float dur)
	{
		TimeWarning val = TimeWarning.New("ResetRemovalTime", 0);
		try
		{
			if (((FacepunchBehaviour)this).IsInvoking((Action)RemoveCorpse))
			{
				((FacepunchBehaviour)this).CancelInvoke((Action)RemoveCorpse);
			}
			((FacepunchBehaviour)this).Invoke((Action)RemoveCorpse, dur);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public virtual float GetRemovalTime()
	{
		BaseGameMode activeGameMode = BaseGameMode.GetActiveGameMode(serverside: true);
		if ((Object)(object)activeGameMode != (Object)null)
		{
			return activeGameMode.CorpseRemovalTime(this);
		}
		return Server.corpsedespawn;
	}

	public void ResetRemovalTime()
	{
		ResetRemovalTime(GetRemovalTime());
	}

	public override void Save(SaveInfo info)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		base.Save(info);
		info.msg.corpse = Pool.Get<Corpse>();
		if (parentEnt.IsValid())
		{
			info.msg.corpse.parentID = parentEnt.net.ID;
		}
	}

	public void TakeChildren(BaseEntity takeChildrenFrom)
	{
		if (takeChildrenFrom.children == null)
		{
			return;
		}
		TimeWarning val = TimeWarning.New("Corpse.TakeChildren", 0);
		try
		{
			BaseEntity[] array = takeChildrenFrom.children.ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SwitchParent(this);
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public override void ApplyInheritedVelocity(Vector3 velocity)
	{
	}

	private Rigidbody SetupRigidBody()
	{
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		if (!prefabRagdoll.isValid)
		{
			return ((Component)this).GetComponent<Rigidbody>();
		}
		if (base.isServer)
		{
			GameObject val = base.gameManager.FindPrefab(prefabRagdoll.resourcePath);
			if ((Object)(object)val == (Object)null)
			{
				return null;
			}
			Ragdoll component = val.GetComponent<Ragdoll>();
			if ((Object)(object)component == (Object)null)
			{
				return null;
			}
			if ((Object)(object)component.primaryBody == (Object)null)
			{
				Debug.LogError((object)("[BaseCorpse] ragdoll.primaryBody isn't set!" + ((Object)((Component)component).gameObject).name));
				return null;
			}
			if ((Object)(object)((Component)this).gameObject.GetComponent<Collider>() == (Object)null)
			{
				BoxCollider component2 = ((Component)component.primaryBody).GetComponent<BoxCollider>();
				if ((Object)(object)component2 == (Object)null)
				{
					Debug.LogError((object)"Ragdoll has unsupported primary collider (make it supported) ", (Object)(object)component);
					return null;
				}
				BoxCollider obj = ((Component)this).gameObject.AddComponent<BoxCollider>();
				obj.size = component2.size * 2f;
				obj.center = component2.center;
				((Collider)obj).sharedMaterial = ((Collider)component2).sharedMaterial;
			}
		}
		Rigidbody val2 = ((Component)this).GetComponent<Rigidbody>();
		if ((Object)(object)val2 == (Object)null)
		{
			val2 = ((Component)this).gameObject.AddComponent<Rigidbody>();
			val2.mass = 10f;
			val2.drag = 0.5f;
			val2.angularDrag = 0.5f;
		}
		val2.useGravity = true;
		val2.collisionDetectionMode = (CollisionDetectionMode)0;
		val2.sleepThreshold = Mathf.Max(0.05f, Physics.sleepThreshold);
		if (base.isServer)
		{
			Buoyancy component3 = ((Component)this).GetComponent<Buoyancy>();
			if ((Object)(object)component3 != (Object)null)
			{
				component3.rigidBody = val2;
			}
			Vector3 velocity = Vector3Ex.Range(-1f, 1f);
			velocity.y += 1f;
			val2.velocity = velocity;
			val2.collisionDetectionMode = (CollisionDetectionMode)3;
			val2.angularVelocity = Vector3Ex.Range(-10f, 10f);
		}
		return val2;
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (info.msg.corpse != null)
		{
			Load(info.msg.corpse);
		}
	}

	private void Load(Corpse corpse)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		if (base.isServer)
		{
			parentEnt = BaseNetworkable.serverEntities.Find(corpse.parentID) as BaseEntity;
		}
		_ = base.isClient;
	}

	public override void OnAttacked(HitInfo info)
	{
		if (base.isServer)
		{
			ResetRemovalTime();
			if (Object.op_Implicit((Object)(object)resourceDispenser))
			{
				resourceDispenser.DoGather(info, this);
			}
			if (!info.DidGather)
			{
				base.OnAttacked(info);
			}
		}
	}

	public override string Categorize()
	{
		return "corpse";
	}

	public override void Eat(BaseNpc baseNpc, float timeSpent)
	{
		ResetRemovalTime();
		Hurt(timeSpent * 5f);
		baseNpc.AddCalories(timeSpent * 2f);
	}

	public override bool ShouldInheritNetworkGroup()
	{
		return false;
	}
}
