using System;
using ConVar;
using UnityEngine;
using UnityEngine.AI;

public class ServerBuildingManager : BuildingManager
{
	private int decayTickBuildingIndex;

	private int decayTickEntityIndex;

	private int decayTickWorldIndex;

	private int navmeshCarveTickBuildingIndex;

	private uint maxBuildingID;

	public void CheckSplit(DecayEntity ent)
	{
		if (ent.buildingID != 0)
		{
			Building building = ent.GetBuilding();
			if (building != null && ShouldSplit(building))
			{
				Split(building);
			}
		}
	}

	private bool ShouldSplit(Building building)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (building.HasBuildingBlocks())
		{
			building.buildingBlocks[0].EntityLinkBroadcast();
			Enumerator<BuildingBlock> enumerator = building.buildingBlocks.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					if (!enumerator.Current.ReceivedEntityLinkBroadcast())
					{
						return true;
					}
				}
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
		}
		return false;
	}

	private void Split(Building building)
	{
		while (building.HasBuildingBlocks())
		{
			BuildingBlock buildingBlock = building.buildingBlocks[0];
			uint newID = BuildingManager.server.NewBuildingID();
			buildingBlock.EntityLinkBroadcast(delegate(BuildingBlock b)
			{
				b.AttachToBuilding(newID);
			});
		}
		while (building.HasBuildingPrivileges())
		{
			BuildingPrivlidge buildingPrivlidge = building.buildingPrivileges[0];
			BuildingBlock nearbyBuildingBlock = buildingPrivlidge.GetNearbyBuildingBlock();
			buildingPrivlidge.AttachToBuilding(Object.op_Implicit((Object)(object)nearbyBuildingBlock) ? nearbyBuildingBlock.buildingID : 0u);
		}
		while (building.HasDecayEntities())
		{
			DecayEntity decayEntity = building.decayEntities[0];
			BuildingBlock nearbyBuildingBlock2 = decayEntity.GetNearbyBuildingBlock();
			decayEntity.AttachToBuilding(Object.op_Implicit((Object)(object)nearbyBuildingBlock2) ? nearbyBuildingBlock2.buildingID : 0u);
		}
		if (AI.nav_carve_use_building_optimization)
		{
			building.isNavMeshCarvingDirty = true;
			int ticks = 2;
			UpdateNavMeshCarver(building, ref ticks, 0);
		}
	}

	public void CheckMerge(DecayEntity ent)
	{
		if (ent.buildingID == 0)
		{
			return;
		}
		Building building = ent.GetBuilding();
		if (building == null)
		{
			return;
		}
		ent.EntityLinkMessage(delegate(BuildingBlock b)
		{
			if (b.buildingID != building.ID)
			{
				Building building2 = b.GetBuilding();
				if (building2 != null)
				{
					Merge(building, building2);
				}
			}
		});
		if (AI.nav_carve_use_building_optimization)
		{
			building.isNavMeshCarvingDirty = true;
			int ticks = 2;
			UpdateNavMeshCarver(building, ref ticks, 0);
		}
	}

	private void Merge(Building building1, Building building2)
	{
		while (building2.HasDecayEntities())
		{
			building2.decayEntities[0].AttachToBuilding(building1.ID);
		}
		if (AI.nav_carve_use_building_optimization)
		{
			building1.isNavMeshCarvingDirty = true;
			building2.isNavMeshCarvingDirty = true;
			int ticks = 3;
			UpdateNavMeshCarver(building1, ref ticks, 0);
			UpdateNavMeshCarver(building1, ref ticks, 0);
		}
	}

	public void Cycle()
	{
		TimeWarning val = TimeWarning.New("StabilityCheckQueue", 0);
		try
		{
			((ObjectWorkQueue<StabilityEntity>)StabilityEntity.stabilityCheckQueue).RunQueue((double)Stability.stabilityqueue);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		val = TimeWarning.New("UpdateSurroundingsQueue", 0);
		try
		{
			((ObjectWorkQueue<Bounds>)StabilityEntity.updateSurroundingsQueue).RunQueue((double)Stability.surroundingsqueue);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		val = TimeWarning.New("UpdateSkinQueue", 0);
		try
		{
			((ObjectWorkQueue<BuildingBlock>)BuildingBlock.updateSkinQueueServer).RunQueue(1.0);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		val = TimeWarning.New("BuildingDecayTick", 0);
		try
		{
			int num = 5;
			BufferList<Building> values = buildingDictionary.Values;
			for (int i = decayTickBuildingIndex; i < values.Count; i++)
			{
				if (num <= 0)
				{
					break;
				}
				BufferList<DecayEntity> values2 = values[i].decayEntities.Values;
				for (int j = decayTickEntityIndex; j < values2.Count; j++)
				{
					if (num <= 0)
					{
						break;
					}
					values2[j].DecayTick();
					num--;
					if (num <= 0)
					{
						decayTickBuildingIndex = i;
						decayTickEntityIndex = j;
					}
				}
				if (num > 0)
				{
					decayTickEntityIndex = 0;
				}
			}
			if (num > 0)
			{
				decayTickBuildingIndex = 0;
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		val = TimeWarning.New("WorldDecayTick", 0);
		try
		{
			int num2 = 5;
			BufferList<DecayEntity> values3 = decayEntities.Values;
			for (int k = decayTickWorldIndex; k < values3.Count; k++)
			{
				if (num2 <= 0)
				{
					break;
				}
				values3[k].DecayTick();
				num2--;
				if (num2 <= 0)
				{
					decayTickWorldIndex = k;
				}
			}
			if (num2 > 0)
			{
				decayTickWorldIndex = 0;
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		if (!AI.nav_carve_use_building_optimization)
		{
			return;
		}
		val = TimeWarning.New("NavMeshCarving", 0);
		try
		{
			int ticks = 5;
			BufferList<Building> values4 = buildingDictionary.Values;
			for (int l = navmeshCarveTickBuildingIndex; l < values4.Count; l++)
			{
				if (ticks <= 0)
				{
					break;
				}
				Building building = values4[l];
				UpdateNavMeshCarver(building, ref ticks, l);
			}
			if (ticks > 0)
			{
				navmeshCarveTickBuildingIndex = 0;
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void UpdateNavMeshCarver(Building building, ref int ticks, int i)
	{
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0284: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0330: Unknown result type (might be due to invalid IL or missing references)
		//IL_033c: Unknown result type (might be due to invalid IL or missing references)
		if (!AI.nav_carve_use_building_optimization || (!building.isNavMeshCarveOptimized && building.navmeshCarvers.Count < AI.nav_carve_min_building_blocks_to_apply_optimization) || !building.isNavMeshCarvingDirty)
		{
			return;
		}
		building.isNavMeshCarvingDirty = false;
		if (building.navmeshCarvers == null)
		{
			if ((Object)(object)building.buildingNavMeshObstacle != (Object)null)
			{
				Object.Destroy((Object)(object)((Component)building.buildingNavMeshObstacle).gameObject);
				building.buildingNavMeshObstacle = null;
				building.isNavMeshCarveOptimized = false;
			}
			return;
		}
		Vector3 val = default(Vector3);
		((Vector3)(ref val))._002Ector((float)World.Size, (float)World.Size, (float)World.Size);
		Vector3 val2 = default(Vector3);
		((Vector3)(ref val2))._002Ector((float)(0L - (long)World.Size), (float)(0L - (long)World.Size), (float)(0L - (long)World.Size));
		int count = building.navmeshCarvers.Count;
		if (count > 0)
		{
			for (int j = 0; j < count; j++)
			{
				NavMeshObstacle val3 = building.navmeshCarvers[j];
				if (((Behaviour)val3).enabled)
				{
					((Behaviour)val3).enabled = false;
				}
				for (int k = 0; k < 3; k++)
				{
					Vector3 position = ((Component)val3).transform.position;
					if (((Vector3)(ref position))[k] < ((Vector3)(ref val))[k])
					{
						int num = k;
						position = ((Component)val3).transform.position;
						((Vector3)(ref val))[num] = ((Vector3)(ref position))[k];
					}
					position = ((Component)val3).transform.position;
					if (((Vector3)(ref position))[k] > ((Vector3)(ref val2))[k])
					{
						int num2 = k;
						position = ((Component)val3).transform.position;
						((Vector3)(ref val2))[num2] = ((Vector3)(ref position))[k];
					}
				}
			}
			Vector3 val4 = (val2 + val) * 0.5f;
			Vector3 zero = Vector3.zero;
			float num3 = Mathf.Abs(val4.x - val.x);
			float num4 = Mathf.Abs(val4.y - val.y);
			float num5 = Mathf.Abs(val4.z - val.z);
			float num6 = Mathf.Abs(val2.x - val4.x);
			float num7 = Mathf.Abs(val2.y - val4.y);
			float num8 = Mathf.Abs(val2.z - val4.z);
			zero.x = Mathf.Max((num3 > num6) ? num3 : num6, AI.nav_carve_min_base_size);
			zero.y = Mathf.Max((num4 > num7) ? num4 : num7, AI.nav_carve_min_base_size);
			zero.z = Mathf.Max((num5 > num8) ? num5 : num8, AI.nav_carve_min_base_size);
			zero = ((count >= 10) ? (zero * (AI.nav_carve_size_multiplier - 1f)) : (zero * AI.nav_carve_size_multiplier));
			if (building.navmeshCarvers.Count > 0)
			{
				if ((Object)(object)building.buildingNavMeshObstacle == (Object)null)
				{
					building.buildingNavMeshObstacle = new GameObject($"Building ({building.ID}) NavMesh Carver").AddComponent<NavMeshObstacle>();
					((Behaviour)building.buildingNavMeshObstacle).enabled = false;
					building.buildingNavMeshObstacle.carving = true;
					building.buildingNavMeshObstacle.shape = (NavMeshObstacleShape)1;
					building.buildingNavMeshObstacle.height = AI.nav_carve_height;
					building.isNavMeshCarveOptimized = true;
				}
				if ((Object)(object)building.buildingNavMeshObstacle != (Object)null)
				{
					((Component)building.buildingNavMeshObstacle).transform.position = val4;
					building.buildingNavMeshObstacle.size = zero;
					if (!((Behaviour)building.buildingNavMeshObstacle).enabled)
					{
						((Behaviour)building.buildingNavMeshObstacle).enabled = true;
					}
				}
			}
		}
		else if ((Object)(object)building.buildingNavMeshObstacle != (Object)null)
		{
			Object.Destroy((Object)(object)((Component)building.buildingNavMeshObstacle).gameObject);
			building.buildingNavMeshObstacle = null;
			building.isNavMeshCarveOptimized = false;
		}
		ticks--;
		if (ticks <= 0)
		{
			navmeshCarveTickBuildingIndex = i;
		}
	}

	public uint NewBuildingID()
	{
		return ++maxBuildingID;
	}

	public void LoadBuildingID(uint id)
	{
		maxBuildingID = Mathx.Max(maxBuildingID, id);
	}

	protected override Building CreateBuilding(uint id)
	{
		return new Building
		{
			ID = id
		};
	}

	protected override void DisposeBuilding(ref Building building)
	{
		building = null;
	}
}
