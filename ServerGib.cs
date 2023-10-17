using System;
using System.Collections.Generic;
using Facepunch;
using ProtoBuf;
using UnityEngine;

public class ServerGib : BaseCombatEntity
{
	public GameObject _gibSource;

	public string _gibName;

	public PhysicMaterial physicsMaterial;

	public bool useContinuousCollision;

	private MeshCollider meshCollider = null;

	private Rigidbody rigidBody = null;

	public override float BoundsPadding()
	{
		return 3f;
	}

	public static List<ServerGib> CreateGibs(string entityToCreatePath, GameObject creator, GameObject gibSource, Vector3 inheritVelocity, float spreadVelocity)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		List<ServerGib> list = new List<ServerGib>();
		MeshRenderer[] componentsInChildren = gibSource.GetComponentsInChildren<MeshRenderer>(true);
		foreach (MeshRenderer val in componentsInChildren)
		{
			MeshFilter component = ((Component)val).GetComponent<MeshFilter>();
			Vector3 val2 = ((Component)val).transform.localPosition;
			Vector3 normalized = ((Vector3)(ref val2)).normalized;
			Matrix4x4 localToWorldMatrix = creator.transform.localToWorldMatrix;
			Vector3 val3 = ((Matrix4x4)(ref localToWorldMatrix)).MultiplyPoint(((Component)val).transform.localPosition) + normalized * 0.5f;
			Quaternion val4 = creator.transform.rotation * ((Component)val).transform.localRotation;
			BaseEntity baseEntity = GameManager.server.CreateEntity(entityToCreatePath, val3, val4);
			if (Object.op_Implicit((Object)(object)baseEntity))
			{
				ServerGib component2 = ((Component)baseEntity).GetComponent<ServerGib>();
				((Component)component2).transform.SetPositionAndRotation(val3, val4);
				component2._gibName = ((Object)val).name;
				MeshCollider component3 = ((Component)val).GetComponent<MeshCollider>();
				Mesh physicsMesh = (((Object)(object)component3 != (Object)null) ? component3.sharedMesh : component.sharedMesh);
				component2.PhysicsInit(physicsMesh);
				val2 = ((Component)val).transform.localPosition;
				Vector3 val5 = ((Vector3)(ref val2)).normalized * spreadVelocity;
				component2.rigidBody.velocity = inheritVelocity + val5;
				Rigidbody obj = component2.rigidBody;
				val2 = Vector3Ex.Range(-1f, 1f);
				obj.angularVelocity = ((Vector3)(ref val2)).normalized * 1f;
				component2.rigidBody.WakeUp();
				component2.Spawn();
				list.Add(component2);
			}
		}
		foreach (ServerGib item in list)
		{
			foreach (ServerGib item2 in list)
			{
				if (!((Object)(object)item == (Object)(object)item2))
				{
					Physics.IgnoreCollision((Collider)(object)item2.GetCollider(), (Collider)(object)item.GetCollider(), true);
				}
			}
		}
		return list;
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		if (!info.forDisk && _gibName != "")
		{
			info.msg.servergib = Pool.Get<ServerGib>();
			info.msg.servergib.gibName = _gibName;
		}
	}

	public MeshCollider GetCollider()
	{
		return meshCollider;
	}

	public override void ServerInit()
	{
		base.ServerInit();
		((FacepunchBehaviour)this).Invoke((Action)RemoveMe, 1800f);
	}

	public void RemoveMe()
	{
		Kill();
	}

	public virtual void PhysicsInit(Mesh physicsMesh)
	{
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		Mesh sharedMesh = null;
		MeshFilter component = ((Component)this).gameObject.GetComponent<MeshFilter>();
		if ((Object)(object)component != (Object)null)
		{
			sharedMesh = component.sharedMesh;
			component.sharedMesh = physicsMesh;
		}
		meshCollider = ((Component)this).gameObject.AddComponent<MeshCollider>();
		meshCollider.sharedMesh = physicsMesh;
		meshCollider.convex = true;
		((Collider)meshCollider).material = physicsMaterial;
		if ((Object)(object)component != (Object)null)
		{
			component.sharedMesh = sharedMesh;
		}
		Rigidbody val = ((Component)this).gameObject.AddComponent<Rigidbody>();
		val.useGravity = true;
		Bounds val2 = ((Collider)meshCollider).bounds;
		Vector3 size = ((Bounds)(ref val2)).size;
		float magnitude = ((Vector3)(ref size)).magnitude;
		val2 = ((Collider)meshCollider).bounds;
		size = ((Bounds)(ref val2)).size;
		val.mass = Mathf.Clamp(magnitude * ((Vector3)(ref size)).magnitude * 20f, 10f, 2000f);
		val.interpolation = (RigidbodyInterpolation)1;
		val.collisionDetectionMode = (CollisionDetectionMode)(useContinuousCollision ? 1 : 0);
		if (base.isServer)
		{
			val.drag = 0.1f;
			val.angularDrag = 0.1f;
		}
		rigidBody = val;
		((Component)this).gameObject.layer = LayerMask.NameToLayer("Default");
		if (base.isClient)
		{
			val.isKinematic = true;
		}
	}
}
