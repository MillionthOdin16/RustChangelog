using System.Collections.Generic;
using Facepunch;
using UnityEngine;

public static class EnvironmentVolumeEx
{
	public static bool CheckEnvironmentVolumes(this Transform transform, Vector3 pos, Quaternion rot, Vector3 scale, EnvironmentType type)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		List<EnvironmentVolume> list = Pool.GetList<EnvironmentVolume>();
		((Component)transform).GetComponentsInChildren<EnvironmentVolume>(true, list);
		OBB obb = default(OBB);
		for (int i = 0; i < list.Count; i++)
		{
			EnvironmentVolume environmentVolume = list[i];
			((OBB)(ref obb))._002Ector(((Component)environmentVolume).transform, new Bounds(environmentVolume.Center, environmentVolume.Size));
			((OBB)(ref obb)).Transform(pos, scale, rot);
			if (EnvironmentManager.Check(obb, type))
			{
				Pool.FreeList<EnvironmentVolume>(ref list);
				return true;
			}
		}
		Pool.FreeList<EnvironmentVolume>(ref list);
		return false;
	}

	public static bool CheckEnvironmentVolumes(this Transform transform, EnvironmentType type)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		return transform.CheckEnvironmentVolumes(transform.position, transform.rotation, transform.lossyScale, type);
	}

	public static bool CheckEnvironmentVolumesInsideTerrain(this Transform transform, Vector3 pos, Quaternion rot, Vector3 scale, EnvironmentType type, float padding = 0f)
	{
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)TerrainMeta.HeightMap == (Object)null)
		{
			return true;
		}
		List<EnvironmentVolume> list = Pool.GetList<EnvironmentVolume>();
		((Component)transform).GetComponentsInChildren<EnvironmentVolume>(true, list);
		if (list.Count == 0)
		{
			Pool.FreeList<EnvironmentVolume>(ref list);
			return true;
		}
		OBB val = default(OBB);
		for (int i = 0; i < list.Count; i++)
		{
			EnvironmentVolume environmentVolume = list[i];
			if ((environmentVolume.Type & type) == 0)
			{
				continue;
			}
			((OBB)(ref val))._002Ector(((Component)environmentVolume).transform, new Bounds(environmentVolume.Center, environmentVolume.Size));
			((OBB)(ref val)).Transform(pos, scale, rot);
			Vector3 point = ((OBB)(ref val)).GetPoint(-1f, 0f, -1f);
			Vector3 point2 = ((OBB)(ref val)).GetPoint(1f, 0f, -1f);
			Vector3 point3 = ((OBB)(ref val)).GetPoint(-1f, 0f, 1f);
			Vector3 point4 = ((OBB)(ref val)).GetPoint(1f, 0f, 1f);
			Bounds val2 = ((OBB)(ref val)).ToBounds();
			float max = ((Bounds)(ref val2)).max.y + padding;
			bool fail = false;
			TerrainMeta.HeightMap.ForEachParallel(point, point2, point3, point4, delegate(int x, int z)
			{
				if (TerrainMeta.HeightMap.GetHeight(x, z) <= max)
				{
					fail = true;
				}
			});
			if (fail)
			{
				Pool.FreeList<EnvironmentVolume>(ref list);
				return false;
			}
		}
		Pool.FreeList<EnvironmentVolume>(ref list);
		return true;
	}

	public static bool CheckEnvironmentVolumesInsideTerrain(this Transform transform, EnvironmentType type)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		return transform.CheckEnvironmentVolumesInsideTerrain(transform.position, transform.rotation, transform.lossyScale, type);
	}

	public static bool CheckEnvironmentVolumesOutsideTerrain(this Transform transform, Vector3 pos, Quaternion rot, Vector3 scale, EnvironmentType type, float padding = 0f)
	{
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)TerrainMeta.HeightMap == (Object)null)
		{
			return true;
		}
		List<EnvironmentVolume> list = Pool.GetList<EnvironmentVolume>();
		((Component)transform).GetComponentsInChildren<EnvironmentVolume>(true, list);
		if (list.Count == 0)
		{
			Pool.FreeList<EnvironmentVolume>(ref list);
			return true;
		}
		OBB val = default(OBB);
		for (int i = 0; i < list.Count; i++)
		{
			EnvironmentVolume environmentVolume = list[i];
			if ((environmentVolume.Type & type) == 0)
			{
				continue;
			}
			((OBB)(ref val))._002Ector(((Component)environmentVolume).transform, new Bounds(environmentVolume.Center, environmentVolume.Size));
			((OBB)(ref val)).Transform(pos, scale, rot);
			Vector3 point = ((OBB)(ref val)).GetPoint(-1f, 0f, -1f);
			Vector3 point2 = ((OBB)(ref val)).GetPoint(1f, 0f, -1f);
			Vector3 point3 = ((OBB)(ref val)).GetPoint(-1f, 0f, 1f);
			Vector3 point4 = ((OBB)(ref val)).GetPoint(1f, 0f, 1f);
			Bounds val2 = ((OBB)(ref val)).ToBounds();
			float min = ((Bounds)(ref val2)).min.y - padding;
			bool fail = false;
			TerrainMeta.HeightMap.ForEachParallel(point, point2, point3, point4, delegate(int x, int z)
			{
				if (TerrainMeta.HeightMap.GetHeight(x, z) >= min)
				{
					fail = true;
				}
			});
			if (fail)
			{
				Pool.FreeList<EnvironmentVolume>(ref list);
				return false;
			}
		}
		Pool.FreeList<EnvironmentVolume>(ref list);
		return true;
	}

	public static bool CheckEnvironmentVolumesOutsideTerrain(this Transform transform, EnvironmentType type)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		return transform.CheckEnvironmentVolumesOutsideTerrain(transform.position, transform.rotation, transform.lossyScale, type);
	}
}
