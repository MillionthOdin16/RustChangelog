using System.Collections.Generic;
using UnityEngine;

public class TerrainPath : TerrainExtension
{
	internal List<PathList> Roads = new List<PathList>();

	internal List<PathList> Rails = new List<PathList>();

	internal List<PathList> Rivers = new List<PathList>();

	internal List<PathList> Powerlines = new List<PathList>();

	internal List<LandmarkInfo> Landmarks = new List<LandmarkInfo>();

	internal List<MonumentInfo> Monuments = new List<MonumentInfo>();

	internal List<RiverInfo> RiverObjs = new List<RiverInfo>();

	internal List<LakeInfo> LakeObjs = new List<LakeInfo>();

	internal GameObject DungeonGridRoot;

	internal List<DungeonGridInfo> DungeonGridEntrances = new List<DungeonGridInfo>();

	internal List<DungeonGridCell> DungeonGridCells = new List<DungeonGridCell>();

	internal GameObject DungeonBaseRoot;

	internal List<DungeonBaseInfo> DungeonBaseEntrances = new List<DungeonBaseInfo>();

	internal List<DungeonBaseLink> DungeonBaseLinks = new List<DungeonBaseLink>();

	internal List<Vector3> OceanPatrolClose = new List<Vector3>();

	internal List<Vector3> OceanPatrolFar = new List<Vector3>();

	private Dictionary<string, List<PowerlineNode>> wires = new Dictionary<string, List<PowerlineNode>>();

	public override void PostSetup()
	{
		foreach (PathList road in Roads)
		{
			road.ProcgenStartNode = null;
			road.ProcgenEndNode = null;
		}
		foreach (PathList rail in Rails)
		{
			rail.ProcgenStartNode = null;
			rail.ProcgenEndNode = null;
		}
		foreach (PathList river in Rivers)
		{
			river.ProcgenStartNode = null;
			river.ProcgenEndNode = null;
		}
		foreach (PathList powerline in Powerlines)
		{
			powerline.ProcgenStartNode = null;
			powerline.ProcgenEndNode = null;
		}
	}

	public void Clear()
	{
		Roads.Clear();
		Rails.Clear();
		Rivers.Clear();
		Powerlines.Clear();
	}

	public T FindClosest<T>(List<T> list, Vector3 pos) where T : MonoBehaviour
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		T result = default(T);
		float num = float.MaxValue;
		foreach (T item in list)
		{
			float num2 = Vector3Ex.Distance2D(((Component)(object)item).transform.position, pos);
			if (!(num2 >= num))
			{
				result = item;
				num = num2;
			}
		}
		return result;
	}

	public static int[,] CreatePowerlineCostmap(ref uint seed)
	{
		float radius = 5f;
		int num = (int)((float)World.Size / 7.5f);
		TerrainPlacementMap placementMap = TerrainMeta.PlacementMap;
		TerrainHeightMap heightMap = TerrainMeta.HeightMap;
		TerrainTopologyMap topologyMap = TerrainMeta.TopologyMap;
		int[,] array = new int[num, num];
		for (int i = 0; i < num; i++)
		{
			float normZ = ((float)i + 0.5f) / (float)num;
			for (int j = 0; j < num; j++)
			{
				float normX = ((float)j + 0.5f) / (float)num;
				float slope = heightMap.GetSlope(normX, normZ);
				int topology = topologyMap.GetTopology(normX, normZ, radius);
				int num2 = 2295172;
				int num3 = 1628160;
				int num4 = 514;
				if ((topology & num2) != 0)
				{
					array[j, i] = int.MaxValue;
				}
				else if ((topology & num3) != 0 || placementMap.GetBlocked(normX, normZ, radius))
				{
					array[j, i] = 2500;
				}
				else if ((topology & num4) != 0)
				{
					array[j, i] = 1000;
				}
				else
				{
					array[j, i] = 1 + (int)(slope * slope * 10f);
				}
			}
		}
		return array;
	}

	public static int[,] CreateRoadCostmap(ref uint seed)
	{
		float radius = 5f;
		float radius2 = 15f;
		int num = (int)((float)World.Size / 7.5f);
		TerrainPlacementMap placementMap = TerrainMeta.PlacementMap;
		TerrainHeightMap heightMap = TerrainMeta.HeightMap;
		TerrainTopologyMap topologyMap = TerrainMeta.TopologyMap;
		int[,] array = new int[num, num];
		for (int i = 0; i < num; i++)
		{
			float normZ = ((float)i + 0.5f) / (float)num;
			for (int j = 0; j < num; j++)
			{
				float normX = ((float)j + 0.5f) / (float)num;
				int num2 = SeedRandom.Range(ref seed, 100, 200);
				float slope = heightMap.GetSlope(normX, normZ);
				int topology = topologyMap.GetTopology(normX, normZ, radius);
				int topology2 = topologyMap.GetTopology(normX, normZ, radius2);
				int num3 = 196996;
				int num4 = 2098176;
				int num5 = 49666;
				if (slope > 20f || (topology & num3) != 0 || (topology2 & num4) != 0)
				{
					array[j, i] = int.MaxValue;
				}
				else if ((topology & num5) != 0 || placementMap.GetBlocked(normX, normZ, radius))
				{
					array[j, i] = 5000;
				}
				else
				{
					array[j, i] = 1 + (int)(slope * slope * 10f) + num2;
				}
			}
		}
		return array;
	}

	public static int[,] CreateRailCostmap(ref uint seed)
	{
		float radius = 5f;
		float radius2 = 25f;
		int num = (int)((float)World.Size / 7.5f);
		TerrainPlacementMap placementMap = TerrainMeta.PlacementMap;
		TerrainHeightMap heightMap = TerrainMeta.HeightMap;
		TerrainTopologyMap topologyMap = TerrainMeta.TopologyMap;
		int[,] array = new int[num, num];
		for (int i = 0; i < num; i++)
		{
			float normZ = ((float)i + 0.5f) / (float)num;
			for (int j = 0; j < num; j++)
			{
				float normX = ((float)j + 0.5f) / (float)num;
				float slope = heightMap.GetSlope(normX, normZ);
				int topology = topologyMap.GetTopology(normX, normZ, radius);
				int topology2 = topologyMap.GetTopology(normX, normZ, radius2);
				int num2 = 196996;
				int num3 = 2098176;
				int num4 = 49666;
				if (slope > 30f || (topology & num2) != 0 || (topology2 & num3) != 0)
				{
					array[j, i] = int.MaxValue;
				}
				else if (slope > 20f || (topology & num4) != 0 || placementMap.GetBlocked(normX, normZ, radius))
				{
					array[j, i] = 5000;
				}
				else if (slope > 10f)
				{
					array[j, i] = 1500;
				}
				else
				{
					array[j, i] = 1000;
				}
			}
		}
		return array;
	}

	public static int[,] CreateBoatCostmap(float depth)
	{
		TerrainHeightMap heightMap = TerrainMeta.HeightMap;
		TerrainWaterMap waterMap = TerrainMeta.WaterMap;
		int num = (int)((float)World.Size / 7.5f);
		int[,] array = new int[num, num];
		for (int i = 0; i < num; i++)
		{
			float normZ = ((float)i + 0.5f) / (float)num;
			for (int j = 0; j < num; j++)
			{
				float normX = ((float)j + 0.5f) / (float)num;
				float height = heightMap.GetHeight(normX, normZ);
				if (waterMap.GetHeight(normX, normZ) - height < depth)
				{
					array[j, i] = int.MaxValue;
				}
				else
				{
					array[j, i] = 1;
				}
			}
		}
		return array;
	}

	public void AddWire(PowerlineNode node)
	{
		string name = ((Object)((Component)node).transform.root).name;
		if (!wires.ContainsKey(name))
		{
			wires.Add(name, new List<PowerlineNode>());
		}
		wires[name].Add(node);
	}

	public void CreateWires()
	{
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		List<GameObject> list = new List<GameObject>();
		int num = 0;
		GameObjectRef gameObjectRef = null;
		foreach (KeyValuePair<string, List<PowerlineNode>> wire in wires)
		{
			foreach (PowerlineNode item in wire.Value)
			{
				PowerLineWireConnectionHelper component = ((Component)item).GetComponent<PowerLineWireConnectionHelper>();
				if (!Object.op_Implicit((Object)(object)component))
				{
					continue;
				}
				if (list.Count == 0)
				{
					gameObjectRef = item.WirePrefab;
					num = component.connections.Count;
				}
				else
				{
					GameObject val = list[list.Count - 1];
					if (!(item.WirePrefab.guid != gameObjectRef?.guid) && component.connections.Count == num)
					{
						Vector3 val2 = val.transform.position - ((Component)item).transform.position;
						if (!(((Vector3)(ref val2)).sqrMagnitude > item.MaxDistance * item.MaxDistance))
						{
							goto IL_0101;
						}
					}
					CreateWire(wire.Key, list, gameObjectRef);
					list.Clear();
				}
				goto IL_0101;
				IL_0101:
				list.Add(((Component)item).gameObject);
			}
			CreateWire(wire.Key, list, gameObjectRef);
			list.Clear();
		}
	}

	private void CreateWire(string name, List<GameObject> objects, GameObjectRef wirePrefab)
	{
		if (objects.Count >= 3 && wirePrefab != null && wirePrefab.isValid)
		{
			PowerLineWire powerLineWire = PowerLineWire.Create(null, objects, wirePrefab, "Powerline Wires", null, 1f, 0.1f);
			if (Object.op_Implicit((Object)(object)powerLineWire))
			{
				((Behaviour)powerLineWire).enabled = false;
				((Component)powerLineWire).gameObject.SetHierarchyGroup(name);
			}
		}
	}

	public MonumentInfo FindMonumentWithBoundsOverlap(Vector3 position)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		if (TerrainMeta.Path.Monuments == null)
		{
			return null;
		}
		foreach (MonumentInfo monument in TerrainMeta.Path.Monuments)
		{
			if ((Object)(object)monument != (Object)null && monument.IsInBounds(position))
			{
				return monument;
			}
		}
		return null;
	}
}
