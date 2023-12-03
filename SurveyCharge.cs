using System.Collections.Generic;
using Facepunch;
using UnityEngine;

public class SurveyCharge : TimedExplosive
{
	public GameObjectRef craterPrefab;

	public GameObjectRef craterPrefab_Oil;

	public override void Explode()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
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
		_ = ((RaycastHit)(ref hitOut)).normal;
		List<SurveyCrater> list = Pool.GetList<SurveyCrater>();
		Vis.Entities(((Component)this).transform.position, 10f, list, 1, (QueryTriggerInteraction)2);
		bool num = list.Count > 0;
		Pool.FreeList<SurveyCrater>(ref list);
		if (num)
		{
			return;
		}
		bool flag = false;
		bool flag2 = false;
		foreach (ResourceDepositManager.ResourceDeposit.ResourceDepositEntry resource in orCreate._resources)
		{
			if (resource.spawnType == ResourceDepositManager.ResourceDeposit.surveySpawnType.ITEM && !resource.isLiquid && resource.amount >= 1000)
			{
				int num2 = Mathf.Clamp(Mathf.CeilToInt(2.5f / resource.workNeeded * 10f), 0, 5);
				int iAmount = 1;
				flag = true;
				if (resource.isLiquid)
				{
					flag2 = true;
				}
				for (int i = 0; i < num2; i++)
				{
					Item item = ItemManager.Create(resource.type, iAmount, 0uL);
					Vector3 modifiedAimConeDirection = AimConeUtil.GetModifiedAimConeDirection(20f, Vector3.up);
					Vector3 vPos = ((Component)this).transform.position + Vector3.up * 1f;
					Vector3 vVelocity = GetInheritedDropVelocity() + modifiedAimConeDirection * Random.Range(5f, 10f);
					Quaternion rotation = Random.rotation;
					Quaternion playerRotation = default(Quaternion);
					BaseEntity baseEntity = item.Drop(vPos, vVelocity, rotation, playerRotation);
					playerRotation = Random.rotation;
					baseEntity.SetAngularVelocity(((Quaternion)(ref playerRotation)).eulerAngles * 5f);
				}
			}
		}
		if (flag)
		{
			string strPrefab = (flag2 ? craterPrefab_Oil.resourcePath : craterPrefab.resourcePath);
			BaseEntity baseEntity2 = GameManager.server.CreateEntity(strPrefab, point, Quaternion.identity);
			if (Object.op_Implicit((Object)(object)baseEntity2))
			{
				baseEntity2.Spawn();
			}
		}
	}
}
