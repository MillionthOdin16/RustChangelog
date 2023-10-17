using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

public static class WaterLevel
{
	public struct WaterInfo
	{
		public bool isValid;

		public float currentDepth;

		public float overallDepth;

		public float surfaceLevel;
	}

	public static float Factor(Vector3 start, Vector3 end, float radius, bool waves, bool volumes, BaseEntity forEntity = null)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		TimeWarning val = TimeWarning.New("WaterLevel.Factor", 0);
		try
		{
			WaterInfo waterInfo = GetWaterInfo(start, end, radius, waves, volumes, forEntity);
			return waterInfo.isValid ? Mathf.InverseLerp(Mathf.Min(start.y, end.y) - radius, Mathf.Max(start.y, end.y) + radius, waterInfo.surfaceLevel) : 0f;
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public static float Factor(Bounds bounds, bool waves, bool volumes, BaseEntity forEntity = null)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		TimeWarning val = TimeWarning.New("WaterLevel.Factor", 0);
		try
		{
			if (((Bounds)(ref bounds)).size == Vector3.zero)
			{
				((Bounds)(ref bounds)).size = new Vector3(0.1f, 0.1f, 0.1f);
			}
			WaterInfo waterInfo = GetWaterInfo(bounds, waves, volumes, forEntity);
			return waterInfo.isValid ? Mathf.InverseLerp(((Bounds)(ref bounds)).min.y, ((Bounds)(ref bounds)).max.y, waterInfo.surfaceLevel) : 0f;
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public static bool Test(Vector3 pos, bool waves, bool volumes, BaseEntity forEntity = null)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		TimeWarning val = TimeWarning.New("WaterLevel.Test", 0);
		try
		{
			return GetWaterInfo(pos, waves, volumes, forEntity).isValid;
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public static float GetWaterDepth(Vector3 pos, bool waves, bool volumes, BaseEntity forEntity = null)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		TimeWarning val = TimeWarning.New("WaterLevel.GetWaterDepth", 0);
		try
		{
			return GetWaterInfo(pos, waves, volumes, forEntity).currentDepth;
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public static float GetOverallWaterDepth(Vector3 pos, bool waves, bool volumes, BaseEntity forEntity = null, bool noEarlyExit = false)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		TimeWarning val = TimeWarning.New("WaterLevel.GetOverallWaterDepth", 0);
		try
		{
			return GetWaterInfo(pos, waves, volumes, forEntity, noEarlyExit).overallDepth;
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public static WaterInfo GetBuoyancyWaterInfo(Vector3 pos, Vector2 posUV, float terrainHeight, float waterHeight, bool doDeepwaterChecks, BaseEntity forEntity)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		TimeWarning val = TimeWarning.New("WaterLevel.GetWaterInfo", 0);
		try
		{
			WaterInfo result = default(WaterInfo);
			if (pos.y > waterHeight)
			{
				return GetWaterInfoFromVolumes(pos, forEntity);
			}
			bool flag = pos.y < terrainHeight - 1f;
			if (flag)
			{
				waterHeight = 0f;
				if (pos.y > waterHeight)
				{
					return result;
				}
			}
			bool flag2 = doDeepwaterChecks && pos.y < waterHeight - 10f;
			int num = (Object.op_Implicit((Object)(object)TerrainMeta.TopologyMap) ? TerrainMeta.TopologyMap.GetTopologyFast(posUV) : 0);
			if ((flag || flag2 || (num & 0x3C180) == 0) && Object.op_Implicit((Object)(object)WaterSystem.Collision) && WaterSystem.Collision.GetIgnore(pos))
			{
				return result;
			}
			RaycastHit val2 = default(RaycastHit);
			if (flag2 && Physics.Raycast(pos, Vector3.up, ref val2, 5f, 16, (QueryTriggerInteraction)2))
			{
				waterHeight = Mathf.Min(waterHeight, ((RaycastHit)(ref val2)).point.y);
			}
			result.isValid = true;
			result.currentDepth = Mathf.Max(0f, waterHeight - pos.y);
			result.overallDepth = Mathf.Max(0f, waterHeight - terrainHeight);
			result.surfaceLevel = waterHeight;
			return result;
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public static WaterInfo GetWaterInfo(Vector3 pos, bool waves, bool volumes, BaseEntity forEntity = null, bool noEarlyExit = false)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		TimeWarning val = TimeWarning.New("WaterLevel.GetWaterInfo", 0);
		try
		{
			WaterInfo waterInfo = default(WaterInfo);
			float num = GetWaterLevel(pos);
			if (pos.y > num)
			{
				if (!noEarlyExit)
				{
					return volumes ? GetWaterInfoFromVolumes(pos, forEntity) : waterInfo;
				}
				waterInfo = (volumes ? GetWaterInfoFromVolumes(pos, forEntity) : waterInfo);
			}
			float num2 = (Object.op_Implicit((Object)(object)TerrainMeta.HeightMap) ? TerrainMeta.HeightMap.GetHeight(pos) : 0f);
			if (pos.y < num2 - 1f)
			{
				num = 0f;
				if (pos.y > num && !noEarlyExit)
				{
					return waterInfo;
				}
			}
			if (Object.op_Implicit((Object)(object)WaterSystem.Collision) && WaterSystem.Collision.GetIgnore(pos))
			{
				return waterInfo;
			}
			waterInfo.isValid = true;
			waterInfo.currentDepth = Mathf.Max(0f, num - pos.y);
			waterInfo.overallDepth = Mathf.Max(0f, num - num2);
			waterInfo.surfaceLevel = num;
			return waterInfo;
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public static WaterInfo GetWaterInfo(Bounds bounds, bool waves, bool volumes, BaseEntity forEntity = null)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		TimeWarning val = TimeWarning.New("WaterLevel.GetWaterInfo", 0);
		try
		{
			WaterInfo waterInfo = default(WaterInfo);
			float num = GetWaterLevel(((Bounds)(ref bounds)).center);
			if (((Bounds)(ref bounds)).min.y > num)
			{
				return volumes ? GetWaterInfoFromVolumes(bounds, forEntity) : waterInfo;
			}
			float num2 = (Object.op_Implicit((Object)(object)TerrainMeta.HeightMap) ? TerrainMeta.HeightMap.GetHeight(((Bounds)(ref bounds)).center) : 0f);
			if (((Bounds)(ref bounds)).max.y < num2 - 1f)
			{
				num = 0f;
				if (((Bounds)(ref bounds)).min.y > num)
				{
					return waterInfo;
				}
			}
			if (Object.op_Implicit((Object)(object)WaterSystem.Collision) && WaterSystem.Collision.GetIgnore(bounds))
			{
				return waterInfo;
			}
			waterInfo.isValid = true;
			waterInfo.currentDepth = Mathf.Max(0f, num - ((Bounds)(ref bounds)).min.y);
			waterInfo.overallDepth = Mathf.Max(0f, num - num2);
			waterInfo.surfaceLevel = num;
			return waterInfo;
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public static WaterInfo GetWaterInfo(Vector3 start, Vector3 end, float radius, bool waves, bool volumes, BaseEntity forEntity = null)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		TimeWarning val = TimeWarning.New("WaterLevel.GetWaterInfo", 0);
		try
		{
			WaterInfo waterInfo = default(WaterInfo);
			Vector3 val2 = (start + end) * 0.5f;
			float num = Mathf.Min(start.y, end.y) - radius;
			float num2 = Mathf.Max(start.y, end.y) + radius;
			float num3 = GetWaterLevel(val2);
			if (num > num3)
			{
				return volumes ? GetWaterInfoFromVolumes(start, end, radius, forEntity) : waterInfo;
			}
			float num4 = (Object.op_Implicit((Object)(object)TerrainMeta.HeightMap) ? TerrainMeta.HeightMap.GetHeight(val2) : 0f);
			if (num2 < num4 - 1f)
			{
				num3 = 0f;
				if (num > num3)
				{
					return waterInfo;
				}
			}
			if (Object.op_Implicit((Object)(object)WaterSystem.Collision) && WaterSystem.Collision.GetIgnore(start, end, radius))
			{
				Vector3 val3 = Vector3Ex.WithY(val2, Mathf.Lerp(num, num2, 0.75f));
				if (WaterSystem.Collision.GetIgnore(val3))
				{
					return waterInfo;
				}
				num3 = Mathf.Min(num3, val3.y);
			}
			waterInfo.isValid = true;
			waterInfo.currentDepth = Mathf.Max(0f, num3 - num);
			waterInfo.overallDepth = Mathf.Max(0f, num3 - num4);
			waterInfo.surfaceLevel = num3;
			return waterInfo;
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public static WaterInfo GetWaterInfo(Camera cam, bool waves, bool volumes, BaseEntity forEntity = null, bool noEarlyExit = false)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		TimeWarning val = TimeWarning.New("WaterLevel.GetWaterInfo", 0);
		try
		{
			if (((Component)cam).transform.position.y < WaterSystem.MinLevel() - 1f)
			{
				return GetWaterInfo(((Component)cam).transform.position, waves, volumes, forEntity, noEarlyExit);
			}
			return GetWaterInfo(((Component)cam).transform.position - Vector3.up, waves, volumes, forEntity, noEarlyExit);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private static float GetWaterLevel(Vector3 pos)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		float num = (Object.op_Implicit((Object)(object)TerrainMeta.HeightMap) ? TerrainMeta.WaterMap.GetHeight(pos) : 0f);
		if (num < WaterSystem.MaxLevel())
		{
			float height = WaterSystem.GetHeight(pos);
			if (num > WaterSystem.OceanLevel)
			{
				return Mathf.Max(num, height);
			}
			return height;
		}
		return num;
	}

	private static WaterInfo GetWaterInfoFromVolumes(Bounds bounds, BaseEntity forEntity)
	{
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		WaterInfo info = default(WaterInfo);
		if ((Object)(object)forEntity == (Object)null)
		{
			List<WaterVolume> list = Pool.GetList<WaterVolume>();
			Vis.Components<WaterVolume>(new OBB(bounds), list, 262144, (QueryTriggerInteraction)2);
			using (List<WaterVolume>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext() && !enumerator.Current.Test(bounds, out info))
				{
				}
			}
			Pool.FreeList<WaterVolume>(ref list);
			return info;
		}
		forEntity.WaterTestFromVolumes(bounds, out info);
		return info;
	}

	private static WaterInfo GetWaterInfoFromVolumes(Vector3 pos, BaseEntity forEntity)
	{
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		WaterInfo info = default(WaterInfo);
		if ((Object)(object)forEntity == (Object)null)
		{
			List<WaterVolume> list = Pool.GetList<WaterVolume>();
			Vis.Components<WaterVolume>(pos, 0.1f, list, 262144, (QueryTriggerInteraction)2);
			using (List<WaterVolume>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext() && !enumerator.Current.Test(pos, out info))
				{
				}
			}
			Pool.FreeList<WaterVolume>(ref list);
			return info;
		}
		forEntity.WaterTestFromVolumes(pos, out info);
		return info;
	}

	private static WaterInfo GetWaterInfoFromVolumes(Vector3 start, Vector3 end, float radius, BaseEntity forEntity)
	{
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		WaterInfo info = default(WaterInfo);
		if ((Object)(object)forEntity == (Object)null)
		{
			List<WaterVolume> list = Pool.GetList<WaterVolume>();
			Vis.Components<WaterVolume>(start, end, radius, list, 262144, (QueryTriggerInteraction)2);
			using (List<WaterVolume>.Enumerator enumerator = list.GetEnumerator())
			{
				while (enumerator.MoveNext() && !enumerator.Current.Test(start, end, radius, out info))
				{
				}
			}
			Pool.FreeList<WaterVolume>(ref list);
			return info;
		}
		forEntity.WaterTestFromVolumes(start, end, radius, out info);
		return info;
	}
}
