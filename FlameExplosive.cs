using UnityEngine;

public class FlameExplosive : TimedExplosive
{
	public GameObjectRef createOnExplode;

	public bool blockCreateUnderwater;

	public float numToCreate = 10f;

	public float minVelocity = 2f;

	public float maxVelocity = 5f;

	public float spreadAngle = 90f;

	public bool forceUpForExplosion;

	public AnimationCurve velocityCurve = new AnimationCurve((Keyframe[])(object)new Keyframe[2]
	{
		new Keyframe(0f, 1f),
		new Keyframe(1f, 1f)
	});

	public AnimationCurve spreadCurve = new AnimationCurve((Keyframe[])(object)new Keyframe[2]
	{
		new Keyframe(0f, 1f),
		new Keyframe(1f, 1f)
	});

	public override void Explode()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		FlameExplode(forceUpForExplosion ? Vector3.up : (-((Component)this).transform.forward));
	}

	public void FlameExplode(Vector3 surfaceNormal)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		if (!base.isServer)
		{
			return;
		}
		Vector3 position = ((Component)this).transform.position;
		if (blockCreateUnderwater && WaterLevel.Test(position))
		{
			base.Explode();
			return;
		}
		Collider component = ((Component)this).GetComponent<Collider>();
		if (Object.op_Implicit((Object)(object)component))
		{
			component.enabled = false;
		}
		for (int i = 0; (float)i < numToCreate; i++)
		{
			BaseEntity baseEntity = GameManager.server.CreateEntity(createOnExplode.resourcePath, position);
			if (Object.op_Implicit((Object)(object)baseEntity))
			{
				float num = (float)i / numToCreate;
				Vector3 modifiedAimConeDirection = AimConeUtil.GetModifiedAimConeDirection(spreadAngle * spreadCurve.Evaluate(num), surfaceNormal);
				((Component)baseEntity).transform.SetPositionAndRotation(position, Quaternion.LookRotation(modifiedAimConeDirection));
				baseEntity.creatorEntity = (((Object)(object)creatorEntity == (Object)null) ? baseEntity : creatorEntity);
				baseEntity.Spawn();
				Vector3 val = ((Vector3)(ref modifiedAimConeDirection)).normalized * Random.Range(minVelocity, maxVelocity) * velocityCurve.Evaluate(num * Random.Range(1f, 1.1f));
				FireBall component2 = ((Component)baseEntity).GetComponent<FireBall>();
				if ((Object)(object)component2 != (Object)null)
				{
					component2.SetDelayedVelocity(val);
				}
				else
				{
					baseEntity.SetVelocity(val);
				}
			}
		}
		base.Explode();
	}

	public override void ProjectileImpact(RaycastHit info, Vector3 rayOrigin)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		FlameExplode(((RaycastHit)(ref info)).normal);
	}
}
