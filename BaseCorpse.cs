using System;
using ConVar;
using Facepunch;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Profiling;

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
		SetupRigidBody();
		ResetRemovalTime();
		resourceDispenser = ((Component)this).GetComponent<ResourceDispenser>();
		base.ServerInit();
	}

	public virtual void InitCorpse(BaseEntity pr)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
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
			foreach (BaseEntity baseEntity in array)
			{
				baseEntity.SwitchParent(this);
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
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
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
			Collider component2 = ((Component)this).gameObject.GetComponent<Collider>();
			if ((Object)(object)component2 == (Object)null)
			{
				BoxCollider component3 = ((Component)component.primaryBody).GetComponent<BoxCollider>();
				if ((Object)(object)component3 == (Object)null)
				{
					Debug.LogError((object)"Ragdoll has unsupported primary collider (make it supported) ", (Object)(object)component);
					return null;
				}
				BoxCollider val2 = ((Component)this).gameObject.AddComponent<BoxCollider>();
				val2.size = component3.size * 2f;
				val2.center = component3.center;
				((Collider)val2).sharedMaterial = ((Collider)component3).sharedMaterial;
			}
		}
		Rigidbody val3 = ((Component)this).GetComponent<Rigidbody>();
		if ((Object)(object)val3 == (Object)null)
		{
			val3 = ((Component)this).gameObject.AddComponent<Rigidbody>();
		}
		val3.mass = 10f;
		val3.useGravity = true;
		val3.drag = 0.5f;
		val3.angularDrag = 0.5f;
		val3.collisionDetectionMode = (CollisionDetectionMode)0;
		val3.sleepThreshold = 0.05f;
		if (base.isServer)
		{
			Profiler.BeginSample("BaseCorpse.Setup");
			Buoyancy component4 = ((Component)this).GetComponent<Buoyancy>();
			if ((Object)(object)component4 != (Object)null)
			{
				component4.rigidBody = val3;
			}
			Profiler.EndSample();
			Vector3 velocity = Vector3Ex.Range(-1f, 1f);
			velocity.y += 1f;
			val3.velocity = velocity;
			val3.collisionDetectionMode = (CollisionDetectionMode)3;
			val3.angularVelocity = Vector3Ex.Range(-10f, 10f);
		}
		return val3;
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
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		if (base.isServer)
		{
			parentEnt = BaseNetworkable.serverEntities.Find(corpse.parentID) as BaseEntity;
		}
		if (!base.isClient)
		{
		}
	}

	public override void OnAttacked(HitInfo info)
	{
		if (base.isServer)
		{
			ResetRemovalTime();
			if (Object.op_Implicit((Object)(object)resourceDispenser))
			{
				resourceDispenser.OnAttacked(info);
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
