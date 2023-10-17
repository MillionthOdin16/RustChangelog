using System.Collections.Generic;
using Facepunch;
using UnityEngine;

public class UnderwaterPathFinder : BasePathFinder
{
	private BaseEntity npc;

	public void Init(BaseEntity npc)
	{
		this.npc = npc;
	}

	public override Vector3 GetBestRoamPosition(BaseNavigator navigator, Vector3 fallbackPos, float minRange, float maxRange)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		List<Vector3> list = Pool.GetList<Vector3>();
		float height = TerrainMeta.WaterMap.GetHeight(((Component)navigator).transform.position);
		float height2 = TerrainMeta.HeightMap.GetHeight(((Component)navigator).transform.position);
		float num = height - 1f;
		float num2 = height + 1f;
		for (int i = 0; i < 8; i++)
		{
			Vector3 pointOnCircle = BasePathFinder.GetPointOnCircle(fallbackPos, Random.Range(1f, navigator.MaxRoamDistanceFromHome), Random.Range(0f, 359f));
			pointOnCircle.y += Random.Range(-2f, 2f);
			pointOnCircle.y = Mathf.Clamp(pointOnCircle.y, height2, height);
			list.Add(pointOnCircle);
		}
		float num3 = -1f;
		int num4 = -1;
		for (int j = 0; j < list.Count; j++)
		{
			Vector3 val = list[j];
			if (npc.IsVisible(val))
			{
				float num5 = 0f;
				Vector3 val2 = Vector3Ex.Direction2D(val, ((Component)navigator).transform.position);
				float num6 = Vector3.Dot(((Component)navigator).transform.forward, val2);
				num5 += Mathf.InverseLerp(0.25f, 0.8f, num6) * 5f;
				float num7 = Mathf.Abs(val.y - ((Component)navigator).transform.position.y);
				num5 += 1f - Mathf.InverseLerp(1f, 3f, num7) * 5f;
				if (num5 > num3 || num4 == -1)
				{
					num3 = num5;
					num4 = j;
				}
			}
		}
		Vector3 result = list[num4];
		Pool.FreeList<Vector3>(ref list);
		return result;
	}

	public override bool GetBestFleePosition(BaseNavigator navigator, AIBrainSenses senses, BaseEntity fleeFrom, Vector3 fallbackPos, float minRange, float maxRange, out Vector3 result)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)fleeFrom == (Object)null)
		{
			result = ((Component)navigator).transform.position;
			return false;
		}
		Vector3 val = Vector3Ex.Direction2D(((Component)navigator).transform.position, ((Component)fleeFrom).transform.position);
		result = ((Component)navigator).transform.position + val * Random.Range(minRange, maxRange);
		return true;
	}
}
