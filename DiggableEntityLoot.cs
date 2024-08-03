using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Rust/DiggableLoot Spawn")]
public class DiggableEntityLoot : ScriptableObject
{
	[Serializable]
	public struct ItemEntry
	{
		public ItemDefinition Item;

		public int Weight;

		public int Min;

		public int Max;
	}

	public List<ItemEntry> Items = new List<ItemEntry>();

	[InspectorFlags]
	public Enum Biomes = (Enum)(-1);

	[InspectorFlags]
	public Enum Topology = (Enum)(-1);

	public bool VerifyLootListForWorldPosition(Vector3 worldPos)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		int num = (Object.op_Implicit((Object)(object)TerrainMeta.BiomeMap) ? TerrainMeta.BiomeMap.GetBiomeMaxType(worldPos) : 2);
		int num2 = ((!Object.op_Implicit((Object)(object)TerrainMeta.TopologyMap)) ? 1 : TerrainMeta.TopologyMap.GetTopology(worldPos));
		if ((num & Biomes) == 0)
		{
			return false;
		}
		if ((num2 & Topology) == 0)
		{
			return false;
		}
		return true;
	}
}
