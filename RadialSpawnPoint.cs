using UnityEngine;

public class RadialSpawnPoint : BaseSpawnPoint
{
	public float radius = 10f;

	public override void GetLocation(out Vector3 pos, out Quaternion rot)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = Random.insideUnitCircle * radius;
		pos = ((Component)this).transform.position + new Vector3(val.x, 0f, val.y);
		rot = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
		DropToGround(ref pos, ref rot);
	}

	public override bool HasPlayersIntersecting()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return BaseNetworkable.HasCloseConnections(((Component)this).transform.position, radius + 1f);
	}

	public override void ObjectSpawned(SpawnPointInstance instance)
	{
	}

	public override void ObjectRetired(SpawnPointInstance instance)
	{
	}
}
