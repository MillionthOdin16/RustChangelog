using System;
using UnityEngine;

public static class FloodedSpawnHandler
{
	private static readonly int[] SpreadSteps = new int[7] { 0, 1, -1, 2, -2, 3, -3 };

	public static bool GetSpawnPoint(BasePlayer.SpawnPoint spawnPoint, float searchHeight)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		SpawnHandler instance = SingletonComponent<SpawnHandler>.Instance;
		if ((Object)(object)TerrainMeta.HeightMap == (Object)null || (Object)(object)instance == (Object)null)
		{
			return false;
		}
		LayerMask placementMask = instance.PlacementMask;
		LayerMask placementCheckMask = instance.PlacementCheckMask;
		float placementCheckHeight = instance.PlacementCheckHeight;
		LayerMask radiusCheckMask = instance.RadiusCheckMask;
		float radiusCheckDistance = instance.RadiusCheckDistance;
		RaycastHit val2 = default(RaycastHit);
		for (int i = 0; i < 10; i++)
		{
			Vector3 val = FindSpawnPoint(searchHeight);
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
			spawnPoint.pos = val;
			spawnPoint.rot = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
			return true;
		}
		return false;
	}

	private static Vector3 FindSpawnPoint(float searchHeight)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = Vector3Ex.WithY(TerrainMeta.Size / 2f, 0f);
		float magnitude = ((Vector3)(ref val)).magnitude;
		float distance = magnitude / 50f;
		float num = RandomAngle();
		float num2 = num + (float)Math.PI;
		Vector3 val2 = TerrainMeta.Position + val + Step(num, magnitude);
		for (int i = 0; i < 50; i++)
		{
			float num3 = float.MinValue;
			Vector3 val3 = Vector3.zero;
			float num4 = 0f;
			int[] spreadSteps = SpreadSteps;
			foreach (int num5 in spreadSteps)
			{
				float num6 = num2 + (float)num5 * 0.17453292f;
				Vector3 val4 = val2 + Step(num6, distance);
				float height = TerrainMeta.HeightMap.GetHeight(val4);
				if (height > num3)
				{
					num3 = height;
					val3 = val4;
					num4 = num6;
				}
			}
			val2 = Vector3Ex.WithY(val3, num3);
			num2 = (num2 + num4) / 2f;
			if (num3 >= searchHeight)
			{
				break;
			}
		}
		return val2;
	}

	private static Vector3 Step(float angle, float distance)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		return new Vector3(distance * Mathf.Cos(angle), 0f, distance * (0f - Mathf.Sin(angle)));
	}

	private static float RandomAngle()
	{
		return Random.value * ((float)Math.PI * 2f);
	}
}
