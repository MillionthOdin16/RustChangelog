using UnityEngine;

public class FlameExplosive : TimedExplosive
{
	public GameObjectRef createOnExplode;

	public bool blockCreateUnderwater = false;

	public float numToCreate = 10f;

	public float minVelocity = 2f;

	public float maxVelocity = 5f;

	public float spreadAngle = 90f;

	public bool forceUpForExplosion = false;

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
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		FlameExplode(forceUpForExplosion ? Vector3.up : (-((Component)this).transform.forward));
	}

	public void FlameExplode(Vector3 surfaceNormal)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		if (!base.isServer)
		{
			return;
		}
		Vector3 position = ((Component)this).transform.position;
		if (blockCreateUnderwater && WaterLevel.Test(position, waves: true, volumes: false))
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
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		FlameExplode(((RaycastHit)(ref info)).normal);
	}
}
