using System.Collections.Generic;
using UnityEngine;

public class SpawnDistribution
{
	internal SpawnHandler Handler;

	internal float Density;

	internal int Count;

	private WorldSpaceGrid<int> grid;

	private Dictionary<uint, int> dict = new Dictionary<uint, int>();

	private ByteQuadtree quadtree = new ByteQuadtree();

	private Vector3 origin;

	private Vector3 area;

	public SpawnDistribution(SpawnHandler handler, byte[] baseValues, Vector3 origin, Vector3 area)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		Handler = handler;
		quadtree.UpdateValues(baseValues);
		this.origin = origin;
		float num = 0f;
		for (int i = 0; i < baseValues.Length; i++)
		{
			num += (float)(int)baseValues[i];
		}
		Density = num / (float)(255 * baseValues.Length);
		Count = 0;
		this.area = new Vector3(area.x / (float)quadtree.Size, area.y, area.z / (float)quadtree.Size);
		grid = new WorldSpaceGrid<int>(area.x, 20f);
	}

	public bool Sample(out Vector3 spawnPos, out Quaternion spawnRot, bool alignToNormal = false, float dithering = 0f)
	{
		return Sample(out spawnPos, out spawnRot, SampleNode(), alignToNormal, dithering);
	}

	public bool Sample(out Vector3 spawnPos, out Quaternion spawnRot, ByteQuadtree.Element node, bool alignToNormal = false, float dithering = 0f)
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)Handler == (Object)null || (Object)(object)TerrainMeta.HeightMap == (Object)null)
		{
			spawnPos = Vector3.zero;
			spawnRot = Quaternion.identity;
			return false;
		}
		LayerMask placementMask = Handler.PlacementMask;
		LayerMask placementCheckMask = Handler.PlacementCheckMask;
		float placementCheckHeight = Handler.PlacementCheckHeight;
		LayerMask radiusCheckMask = Handler.RadiusCheckMask;
		float radiusCheckDistance = Handler.RadiusCheckDistance;
		Vector3 val = default(Vector3);
		RaycastHit val2 = default(RaycastHit);
		for (int i = 0; i < 15; i++)
		{
			spawnPos = origin;
			spawnPos.x += node.Coords.x * area.x;
			spawnPos.z += node.Coords.y * area.z;
			spawnPos.x += Random.value * area.x;
			spawnPos.z += Random.value * area.z;
			spawnPos.x += Random.Range(0f - dithering, dithering);
			spawnPos.z += Random.Range(0f - dithering, dithering);
			((Vector3)(ref val))._002Ector(spawnPos.x, TerrainMeta.HeightMap.GetHeight(spawnPos), spawnPos.z);
			if (val.y <= spawnPos.y)
			{
				continue;
			}
			if (LayerMask.op_Implicit(placementCheckMask) != 0 && Physics.Raycast(val + Vector3.up * placementCheckHeight, Vector3.down, ref val2, placementCheckHeight, LayerMask.op_Implicit(placementCheckMask)))
			{
				int num = 1 << ((Component)((RaycastHit)(ref val2)).transform).gameObject.layer;
				if ((num & LayerMask.op_Implicit(placementMask)) == 0)
				{
					continue;
				}
				val.y = ((RaycastHit)(ref val2)).point.y;
			}
			if (LayerMask.op_Implicit(radiusCheckMask) != 0 && Physics.CheckSphere(val, radiusCheckDistance, LayerMask.op_Implicit(radiusCheckMask)))
			{
				continue;
			}
			spawnPos.y = val.y;
			spawnRot = Quaternion.Euler(new Vector3(0f, Random.Range(0f, 360f), 0f));
			if (alignToNormal)
			{
				Vector3 normal = TerrainMeta.HeightMap.GetNormal(spawnPos);
				spawnRot = QuaternionEx.LookRotationForcedUp(spawnRot * Vector3.forward, normal);
			}
			return true;
		}
		spawnPos = Vector3.zero;
		spawnRot = Quaternion.identity;
		return false;
	}

	public ByteQuadtree.Element SampleNode()
	{
		ByteQuadtree.Element result = quadtree.Root;
		while (!result.IsLeaf)
		{
			result = result.RandChild;
		}
		return result;
	}

	public void AddInstance(Spawnable spawnable)
	{
		UpdateCount(spawnable, 1);
	}

	public void RemoveInstance(Spawnable spawnable)
	{
		UpdateCount(spawnable, -1);
	}

	private void UpdateCount(Spawnable spawnable, int delta)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		Count += delta;
		WorldSpaceGrid<int> val = grid;
		Vector3 spawnPosition = spawnable.SpawnPosition;
		val[spawnPosition] += delta;
		BaseEntity component = ((Component)spawnable).GetComponent<BaseEntity>();
		if (Object.op_Implicit((Object)(object)component))
		{
			if (dict.TryGetValue(component.prefabID, out var value))
			{
				dict[component.prefabID] = value + delta;
				return;
			}
			value = delta;
			dict.Add(component.prefabID, value);
		}
	}

	public int GetCount(uint prefabID)
	{
		dict.TryGetValue(prefabID, out var value);
		return value;
	}

	public int GetCount(Vector3 position)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return grid[position];
	}

	public float GetGridCellArea()
	{
		return grid.CellArea;
	}
}
