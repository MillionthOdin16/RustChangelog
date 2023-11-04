using UnityEngine;

public class DungeonGridCell : MonoBehaviour
{
	public DungeonGridConnectionType North;

	public DungeonGridConnectionType South;

	public DungeonGridConnectionType West;

	public DungeonGridConnectionType East;

	public DungeonGridConnectionVariant NorthVariant;

	public DungeonGridConnectionVariant SouthVariant;

	public DungeonGridConnectionVariant WestVariant;

	public DungeonGridConnectionVariant EastVariant;

	public GameObjectRef[] AvoidNeighbours;

	public MeshRenderer[] MapRenderers;

	public bool ShouldAvoid(uint id)
	{
		GameObjectRef[] avoidNeighbours = AvoidNeighbours;
		foreach (GameObjectRef gameObjectRef in avoidNeighbours)
		{
			if (gameObjectRef.resourceID == id)
			{
				return true;
			}
		}
		return false;
	}

	protected void Awake()
	{
		if (Object.op_Implicit((Object)(object)TerrainMeta.Path))
		{
			TerrainMeta.Path.DungeonGridCells.Add(this);
		}
	}
}
