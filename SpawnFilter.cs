using System;
using UnityEngine;

[Serializable]
public class SpawnFilter
{
	[InspectorFlags]
	public Enum SplatType = (Enum)(-1);

	[InspectorFlags]
	public Enum BiomeType = (Enum)(-1);

	[InspectorFlags]
	public Enum TopologyAny = (Enum)(-1);

	[InspectorFlags]
	public Enum TopologyAll = (Enum)0;

	[InspectorFlags]
	public Enum TopologyNot = (Enum)0;

	public bool Test(Vector3 worldPos)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return GetFactor(worldPos) > 0.5f;
	}

	public bool Test(float normX, float normZ)
	{
		return GetFactor(normX, normZ) > 0.5f;
	}

	public float GetFactor(Vector3 worldPos, bool checkPlacementMap = true)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		float normX = TerrainMeta.NormalizeX(worldPos.x);
		float normZ = TerrainMeta.NormalizeZ(worldPos.z);
		return GetFactor(normX, normZ, checkPlacementMap);
	}

	public float GetFactor(float normX, float normZ, bool checkPlacementMap = true)
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Expected I4, but got Unknown
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Expected I4, but got Unknown
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Expected I4, but got Unknown
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Expected I4, but got Unknown
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Expected I4, but got Unknown
		if ((Object)(object)TerrainMeta.TopologyMap == (Object)null)
		{
			return 0f;
		}
		if (checkPlacementMap && (Object)(object)TerrainMeta.PlacementMap != (Object)null && TerrainMeta.PlacementMap.GetBlocked(normX, normZ))
		{
			return 0f;
		}
		int num = (int)SplatType;
		int num2 = (int)BiomeType;
		int num3 = (int)TopologyAny;
		int num4 = (int)TopologyAll;
		int num5 = (int)TopologyNot;
		if (num3 == 0)
		{
			Debug.LogError((object)"Empty topology filter is invalid.");
		}
		else if (num3 != -1 || num4 != 0 || num5 != 0)
		{
			int topology = TerrainMeta.TopologyMap.GetTopology(normX, normZ);
			if (num3 != -1 && (topology & num3) == 0)
			{
				return 0f;
			}
			if (num5 != 0 && (topology & num5) != 0)
			{
				return 0f;
			}
			if (num4 != 0 && (topology & num4) != num4)
			{
				return 0f;
			}
		}
		switch (num2)
		{
		case 0:
			Debug.LogError((object)"Empty biome filter is invalid.");
			break;
		default:
		{
			int biomeMaxType = TerrainMeta.BiomeMap.GetBiomeMaxType(normX, normZ);
			if ((biomeMaxType & num2) == 0)
			{
				return 0f;
			}
			break;
		}
		case -1:
			break;
		}
		switch (num)
		{
		case 0:
			Debug.LogError((object)"Empty splat filter is invalid.");
			break;
		default:
			return TerrainMeta.SplatMap.GetSplat(normX, normZ, num);
		case -1:
			break;
		}
		return 1f;
	}
}
