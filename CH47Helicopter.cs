using UnityEngine;

public class CH47Helicopter : BaseHelicopter
{
	public GameObjectRef mapMarkerEntityPrefab;

	[Header("Sounds")]
	public SoundDefinition flightEngineSoundDef;

	public SoundDefinition flightThwopsSoundDef;

	public float rotorGainModSmoothing = 0.25f;

	public float engineGainMin = 0.5f;

	public float engineGainMax = 1f;

	public float thwopGainMin = 0.5f;

	public float thwopGainMax = 1f;

	private BaseEntity mapMarkerInstance;

	public override void ServerInit()
	{
		rigidBody.isKinematic = false;
		base.ServerInit();
		CreateMapMarker();
	}

	public void CreateMapMarker()
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		if (Object.op_Implicit((Object)(object)mapMarkerInstance))
		{
			mapMarkerInstance.Kill();
		}
		BaseEntity baseEntity = GameManager.server.CreateEntity(mapMarkerEntityPrefab.resourcePath, Vector3.zero, Quaternion.identity);
		baseEntity.Spawn();
		baseEntity.SetParent(this);
		mapMarkerInstance = baseEntity;
	}

	public override bool IsValidHomingTarget()
	{
		return false;
	}

	protected override bool CanPushNow(BasePlayer pusher)
	{
		return false;
	}
}
