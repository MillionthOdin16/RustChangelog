using UnityEngine;

public struct SpawnIndividual
{
	public uint PrefabID;

	public Vector3 Position;

	public Quaternion Rotation;

	public SpawnIndividual(uint prefabID, Vector3 position, Quaternion rotation)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		PrefabID = prefabID;
		Position = position;
		Rotation = rotation;
	}
}
