using ConVar;
using UnityEngine;

public class ItemModProjectileSpawn : ItemModProjectile
{
	public float createOnImpactChance;

	public GameObjectRef createOnImpact = new GameObjectRef();

	public float spreadAngle = 30f;

	public float spreadVelocityMin = 1f;

	public float spreadVelocityMax = 3f;

	public int numToCreateChances = 1;

	public override void ServerProjectileHit(HitInfo info)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < numToCreateChances; i++)
		{
			if (!createOnImpact.isValid || !(Random.Range(0f, 1f) < createOnImpactChance))
			{
				continue;
			}
			Vector3 hitPositionWorld = info.HitPositionWorld;
			Vector3 pointStart = info.PointStart;
			Vector3 normalized = ((Vector3)(ref info.ProjectileVelocity)).normalized;
			Vector3 normalized2 = ((Vector3)(ref info.HitNormalWorld)).normalized;
			Vector3 val = hitPositionWorld - normalized * 0.1f;
			Quaternion rotation = Quaternion.LookRotation(-normalized);
			int layerMask = (ConVar.AntiHack.projectile_terraincheck ? 10551296 : 2162688);
			if (!GamePhysics.LineOfSight(pointStart, val, layerMask))
			{
				continue;
			}
			BaseEntity baseEntity = GameManager.server.CreateEntity(createOnImpact.resourcePath);
			if (Object.op_Implicit((Object)(object)baseEntity))
			{
				((Component)baseEntity).transform.position = val;
				((Component)baseEntity).transform.rotation = rotation;
				baseEntity.Spawn();
				if (spreadAngle > 0f)
				{
					Vector3 modifiedAimConeDirection = AimConeUtil.GetModifiedAimConeDirection(spreadAngle, normalized2);
					baseEntity.SetVelocity(modifiedAimConeDirection * Random.Range(1f, 3f));
				}
			}
		}
		base.ServerProjectileHit(info);
	}
}
