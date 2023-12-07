using System.Collections.Generic;
using Facepunch;
using UnityEngine;

public class SurveyCharge : TimedExplosive
{
	public GameObjectRef craterPrefab;

	public GameObjectRef craterPrefab_Oil;

	public override void Explode()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		base.Explode();
		if (WaterLevel.Test(((Component)this).transform.position, waves: true, volumes: true, this))
		{
			return;
		}
		ResourceDepositManager.ResourceDeposit orCreate = ResourceDepositManager.GetOrCreate(((Component)this).transform.position);
		if (orCreate == null || Time.realtimeSinceStartup - orCreate.lastSurveyTime < 10f)
		{
			return;
		}
		orCreate.lastSurveyTime = Time.realtimeSinceStartup;
		if (!TransformUtil.GetGroundInfo(((Component)this).transform.position, out var hitOut, 0.3f, LayerMask.op_Implicit(8388608)))
		{
			return;
		}
		Vector3 point = ((RaycastHit)(ref hitOut)).point;
		Vector3 normal = ((RaycastHit)(ref hitOut)).normal;
		List<SurveyCrater> list = Pool.GetList<SurveyCrater>();
		Vis.Entities(((Component)this).transform.position, 10f, list, 1, (QueryTriggerInteraction)2);
		bool flag = list.Count > 0;
		Pool.FreeList<SurveyCrater>(ref list);
		if (flag)
		{
			return;
		}
		bool flag2 = false;
		bool flag3 = false;
		foreach (ResourceDepositManager.ResourceDeposit.ResourceDepositEntry resource in orCreate._resources)
		{
			if (resource.spawnType == ResourceDepositManager.ResourceDeposit.surveySpawnType.ITEM && !resource.isLiquid && resource.amount >= 1000)
			{
				int num = Mathf.Clamp(Mathf.CeilToInt(2.5f / resource.workNeeded * 10f), 0, 5);
				int iAmount = 1;
				flag2 = true;
				if (resource.isLiquid)
				{
					flag3 = true;
				}
				for (int i = 0; i < num; i++)
				{
					Item item = ItemManager.Create(resource.type, iAmount, 0uL);
					float aimCone = 20f;
					Vector3 modifiedAimConeDirection = AimConeUtil.GetModifiedAimConeDirection(aimCone, Vector3.up);
					BaseEntity baseEntity = item.Drop(((Component)this).transform.position + Vector3.up * 1f, GetInheritedDropVelocity() + modifiedAimConeDirection * Random.Range(5f, 10f), Random.rotation);
					Quaternion rotation = Random.rotation;
					baseEntity.SetAngularVelocity(((Quaternion)(ref rotation)).eulerAngles * 5f);
				}
			}
		}
		if (flag2)
		{
			string strPrefab = (flag3 ? craterPrefab_Oil.resourcePath : craterPrefab.resourcePath);
			BaseEntity baseEntity2 = GameManager.server.CreateEntity(strPrefab, point, Quaternion.identity);
			if (Object.op_Implicit((Object)(object)baseEntity2))
			{
				baseEntity2.Spawn();
			}
		}
	}
}
