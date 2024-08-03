using UnityEngine;

public class SpaceCheckingSpawnPoint : GenericSpawnPoint
{
	public bool useCustomBoundsCheckMask;

	public LayerMask customBoundsCheckMask;

	public float customBoundsCheckScale = 1f;

	public override bool IsAvailableTo(GameObjectRef prefabRef)
	{
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		if (!base.IsAvailableTo(prefabRef))
		{
			return false;
		}
		if (useCustomBoundsCheckMask)
		{
			return SpawnHandler.CheckBounds(prefabRef.Get(), ((Component)this).transform.position, ((Component)this).transform.rotation, Vector3.one * customBoundsCheckScale, customBoundsCheckMask);
		}
		return SingletonComponent<SpawnHandler>.Instance.CheckBounds(prefabRef.Get(), ((Component)this).transform.position, ((Component)this).transform.rotation, Vector3.one * customBoundsCheckScale);
	}
}
