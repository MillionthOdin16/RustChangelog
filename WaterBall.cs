using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

public class WaterBall : BaseEntity
{
	public ItemDefinition liquidType;

	public int waterAmount = 0;

	public GameObjectRef waterExplosion;

	public Collider waterCollider;

	public Rigidbody myRigidBody;

	public override void ServerInit()
	{
		base.ServerInit();
		((FacepunchBehaviour)this).Invoke((Action)Extinguish, 10f);
	}

	public void Extinguish()
	{
		((FacepunchBehaviour)this).CancelInvoke((Action)Extinguish);
		if (!base.IsDestroyed)
		{
			Kill();
		}
	}

	public void FixedUpdate()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		if (base.isServer)
		{
			((Component)this).GetComponent<Rigidbody>().AddForce(Physics.gravity, (ForceMode)5);
		}
	}

	public static bool DoSplash(Vector3 position, float radius, ItemDefinition liquidDef, int amount)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		List<BaseEntity> list = Pool.GetList<BaseEntity>();
		Vis.Entities(position, radius, list, 1220225811, (QueryTriggerInteraction)2);
		int num = 0;
		int num2 = amount;
		while (amount > 0 && num < 3)
		{
			List<ISplashable> list2 = Pool.GetList<ISplashable>();
			foreach (BaseEntity item in list)
			{
				if (!item.isClient && item is ISplashable splashable && !list2.Contains(splashable) && splashable.WantsSplash(liquidDef, amount))
				{
					list2.Add(splashable);
				}
			}
			if (list2.Count == 0)
			{
				break;
			}
			int num3 = Mathf.CeilToInt((float)(amount / list2.Count));
			foreach (ISplashable item2 in list2)
			{
				int num4 = item2.DoSplash(liquidDef, Mathf.Min(amount, num3));
				amount -= num4;
				if (amount <= 0)
				{
					break;
				}
			}
			Pool.FreeList<ISplashable>(ref list2);
			num++;
		}
		Pool.FreeList<BaseEntity>(ref list);
		return amount < num2;
	}

	private void OnCollisionEnter(Collision collision)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		if (!base.isClient && !myRigidBody.isKinematic)
		{
			float num = 2.5f;
			DoSplash(((Component)this).transform.position + new Vector3(0f, num * 0.75f, 0f), num, liquidType, waterAmount);
			Effect.server.Run(waterExplosion.resourcePath, ((Component)this).transform.position + new Vector3(0f, 0f, 0f), Vector3.up);
			myRigidBody.isKinematic = true;
			waterCollider.enabled = false;
			((FacepunchBehaviour)this).Invoke((Action)Extinguish, 2f);
		}
	}
}
