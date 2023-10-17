using UnityEngine;

public static class TerrainModifierEx
{
	public static void ApplyTerrainModifiers(this Transform transform, TerrainModifier[] modifiers, Vector3 pos, Quaternion rot, Vector3 scale)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		foreach (TerrainModifier terrainModifier in modifiers)
		{
			Vector3 val = Vector3.Scale(terrainModifier.worldPosition, scale);
			Vector3 pos2 = pos + rot * val;
			float y = scale.y;
			terrainModifier.Apply(pos2, y);
		}
	}

	public static void ApplyTerrainModifiers(this Transform transform, TerrainModifier[] modifiers)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		transform.ApplyTerrainModifiers(modifiers, transform.position, transform.rotation, transform.lossyScale);
	}
}
