using Rust;
using UnityEngine;

public class MLRSRocket : TimedExplosive, SamSite.ISamSiteTarget
{
	[SerializeField]
	private GameObjectRef mapMarkerPrefab;

	[SerializeField]
	private GameObjectRef launchBlastFXPrefab;

	[SerializeField]
	private GameObjectRef explosionGroundFXPrefab;

	[SerializeField]
	private ServerProjectile serverProjectile;

	private EntityRef mapMarkerInstanceRef;

	public SamSite.SamTargetType SAMTargetType => SamSite.targetTypeMissile;

	public override void ServerInit()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		base.ServerInit();
		CreateMapMarker();
		Effect.server.Run(launchBlastFXPrefab.resourcePath, PivotPoint(), ((Component)this).transform.up, null, broadcast: true);
	}

	public override void ProjectileImpact(RaycastHit info, Vector3 rayOrigin)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		Explode(rayOrigin);
		if (Physics.Raycast(((RaycastHit)(ref info)).point + Vector3.up, Vector3.down, 4f, 1218511121, (QueryTriggerInteraction)1))
		{
			Effect.server.Run(explosionGroundFXPrefab.resourcePath, ((RaycastHit)(ref info)).point, Vector3.up, null, broadcast: true);
		}
	}

	private void CreateMapMarker()
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		BaseEntity baseEntity = mapMarkerInstanceRef.Get(base.isServer);
		if (baseEntity.IsValid())
		{
			baseEntity.Kill();
		}
		BaseEntity baseEntity2 = GameManager.server.CreateEntity(mapMarkerPrefab?.resourcePath, ((Component)this).transform.position, Quaternion.identity);
		baseEntity2.OwnerID = base.OwnerID;
		baseEntity2.Spawn();
		baseEntity2.SetParent(this, worldPositionStays: true);
		mapMarkerInstanceRef.Set(baseEntity2);
	}

	public bool IsValidSAMTarget(bool staticRespawn)
	{
		return true;
	}

	public override Vector3 GetLocalVelocityServer()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		return serverProjectile.CurrentVelocity;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!other.IsOnLayer((Layer)18))
		{
			return;
		}
		if (((Component)other).CompareTag("MLRSRocketTrigger"))
		{
			Explode();
			TimedExplosive componentInParent = ((Component)other).GetComponentInParent<TimedExplosive>();
			if ((Object)(object)componentInParent != (Object)null)
			{
				componentInParent.Explode();
			}
		}
		else if ((Object)(object)((Component)other).GetComponent<TriggerSafeZone>() != (Object)null)
		{
			Kill();
		}
	}
}
