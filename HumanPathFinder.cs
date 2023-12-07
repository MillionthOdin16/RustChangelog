using UnityEngine;
using UnityEngine.Profiling;

public class HumanPathFinder : BasePathFinder
{
	private BaseEntity npc;

	public void Init(BaseEntity npc)
	{
		this.npc = npc;
	}

	public override AIMovePoint GetBestRoamPoint(Vector3 anchorPos, Vector3 currentPos, Vector3 currentDirection, float anchorClampDistance, float lookupMaxRange = 20f)
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		AIInformationZone aIInformationZone = null;
		if (npc is HumanNPC humanNPC)
		{
			aIInformationZone = ((!((Object)(object)humanNPC.VirtualInfoZone != (Object)null)) ? humanNPC.GetInformationZone(currentPos) : humanNPC.VirtualInfoZone);
		}
		if ((Object)(object)aIInformationZone == (Object)null)
		{
			return null;
		}
		return GetBestRoamPoint(aIInformationZone, anchorPos, currentPos, currentDirection, anchorClampDistance, lookupMaxRange);
	}

	private AIMovePoint GetBestRoamPoint(AIInformationZone aiZone, Vector3 anchorPos, Vector3 currentPos, Vector3 currentDirection, float clampDistance, float lookupMaxRange)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)aiZone == (Object)null)
		{
			return null;
		}
		Profiler.BeginSample("AIInformationZone.GetBestRoamPoint");
		bool flag = clampDistance > -1f;
		float num = float.NegativeInfinity;
		AIPoint aIPoint = null;
		int pointCount;
		AIPoint[] movePointsInRange = aiZone.GetMovePointsInRange(anchorPos, lookupMaxRange, out pointCount);
		if (movePointsInRange == null || pointCount <= 0)
		{
			Profiler.EndSample();
			return null;
		}
		for (int i = 0; i < pointCount; i++)
		{
			AIPoint aIPoint2 = movePointsInRange[i];
			if (!((Component)((Component)aIPoint2).transform.parent).gameObject.activeSelf)
			{
				continue;
			}
			float num2 = Mathf.Abs(currentPos.y - ((Component)aIPoint2).transform.position.y);
			bool flag2 = currentPos.y < WaterSystem.OceanLevel;
			if (!flag2 && ((!flag2 && ((Component)aIPoint2).transform.position.y < WaterSystem.OceanLevel) || (currentPos.y >= WaterSystem.OceanLevel && num2 > 5f)))
			{
				continue;
			}
			float num3 = 0f;
			float num4 = Vector3.Dot(currentDirection, Vector3Ex.Direction2D(((Component)aIPoint2).transform.position, currentPos));
			num3 += Mathf.InverseLerp(-1f, 1f, num4) * 100f;
			if (!aIPoint2.InUse())
			{
				num3 += 1000f;
			}
			num3 += (1f - Mathf.InverseLerp(1f, 10f, num2)) * 100f;
			float num5 = Vector3.Distance(currentPos, ((Component)aIPoint2).transform.position);
			if (num5 <= 1f)
			{
				num3 -= 3000f;
			}
			if (flag)
			{
				float num6 = Vector3.Distance(anchorPos, ((Component)aIPoint2).transform.position);
				if (num6 <= clampDistance)
				{
					num3 += 1000f;
					num3 += (1f - Mathf.InverseLerp(0f, clampDistance, num6)) * 200f * Random.Range(0.8f, 1f);
				}
			}
			else if (num5 > 3f)
			{
				num3 += Mathf.InverseLerp(3f, lookupMaxRange, num5) * 50f;
			}
			if (num3 > num)
			{
				aIPoint = aIPoint2;
				num = num3;
			}
		}
		Profiler.EndSample();
		return aIPoint as AIMovePoint;
	}
}
