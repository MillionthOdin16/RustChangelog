using UnityEngine;

public class TerrainTopologySet : TerrainModifier
{
	[InspectorFlags]
	public Enum TopologyType = (Enum)512;

	protected override void Apply(Vector3 position, float opacity, float radius, float fade)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Expected I4, but got Unknown
		if (Object.op_Implicit((Object)(object)TerrainMeta.TopologyMap))
		{
			TerrainMeta.TopologyMap.SetTopology(position, (int)TopologyType, radius, fade);
		}
	}
}
