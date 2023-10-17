using System;
using UnityEngine;
using UnityEngine.Profiling;

public class BasePathFinder
{
	private static Vector3[] preferedTopologySamples = (Vector3[])(object)new Vector3[4];

	private static Vector3[] topologySamples = (Vector3[])(object)new Vector3[4];

	private Vector3 chosenPosition;

	private const float halfPI = (float)Math.PI / 180f;

	public virtual Vector3 GetRandomPatrolPoint()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		return Vector3.zero;
	}

	public virtual AIMovePoint GetBestRoamPoint(Vector3 anchorPos, Vector3 currentPos, Vector3 currentDirection, float anchorClampDistance, float lookupMaxRange = 20f)
	{
		return null;
	}

	public void DebugDraw()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		Color color = Gizmos.color;
		Gizmos.color = Color.green;
		Gizmos.DrawSphere(chosenPosition, 5f);
		Gizmos.color = Color.blue;
		Vector3[] array = topologySamples;
		foreach (Vector3 val in array)
		{
			Gizmos.DrawSphere(val, 2.5f);
		}
		Gizmos.color = color;
	}

	public virtual Vector3 GetRandomPositionAround(Vector3 position, float minDistFrom = 0f, float maxDistFrom = 2f)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		if (maxDistFrom < 0f)
		{
			maxDistFrom = 0f;
		}
		Profiler.BeginSample("HumanNPC.GetRandomPositionAround");
		Vector2 val = Random.insideUnitCircle * maxDistFrom;
		float num = Mathf.Clamp(Mathf.Max(Mathf.Abs(val.x), minDistFrom), minDistFrom, maxDistFrom) * Mathf.Sign(val.x);
		float num2 = Mathf.Clamp(Mathf.Max(Mathf.Abs(val.y), minDistFrom), minDistFrom, maxDistFrom) * Mathf.Sign(val.y);
		Profiler.EndSample();
		return position + new Vector3(num, 0f, num2);
	}

	public virtual Vector3 GetBestRoamPosition(BaseNavigator navigator, Vector3 fallbackPos, float minRange, float maxRange)
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		Profiler.BeginSample("BasePathFinder.GetBestRoamPosition");
		float radius = Random.Range(minRange, maxRange);
		int num = 0;
		int num2 = 0;
		float num3 = Random.Range(0f, 90f);
		for (float num4 = 0f; num4 < 360f; num4 += 90f)
		{
			Vector3 pointOnCircle = GetPointOnCircle(((Component)navigator).transform.position, radius, num4 + num3);
			if (navigator.GetNearestNavmeshPosition(pointOnCircle, out var position, 10f) && navigator.IsPositionABiomeRequirement(position) && navigator.IsAcceptableWaterDepth(position) && !navigator.IsPositionPreventTopology(position))
			{
				topologySamples[num] = position;
				num++;
				if (navigator.IsPositionABiomePreference(position) && navigator.IsPositionATopologyPreference(position))
				{
					preferedTopologySamples[num2] = position;
					num2++;
				}
			}
		}
		if (num2 > 0)
		{
			chosenPosition = preferedTopologySamples[Random.Range(0, num2)];
		}
		else if (num > 0)
		{
			chosenPosition = topologySamples[Random.Range(0, num)];
		}
		else
		{
			chosenPosition = fallbackPos;
		}
		Profiler.EndSample();
		return chosenPosition;
	}

	public virtual Vector3 GetBestRoamPositionFromAnchor(BaseNavigator navigator, Vector3 anchorPos, Vector3 fallbackPos, float minRange, float maxRange)
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		Profiler.BeginSample("BasePathFinder.GetBestRoamPositionFromAnchor");
		float radius = Random.Range(minRange, maxRange);
		int num = 0;
		int num2 = 0;
		float num3 = Random.Range(0f, 90f);
		for (float num4 = 0f; num4 < 360f; num4 += 90f)
		{
			Vector3 pointOnCircle = GetPointOnCircle(anchorPos, radius, num4 + num3);
			if (navigator.GetNearestNavmeshPosition(pointOnCircle, out var position, 10f) && navigator.IsAcceptableWaterDepth(position))
			{
				topologySamples[num] = position;
				num++;
				if (navigator.IsPositionABiomePreference(position) && navigator.IsPositionATopologyPreference(position))
				{
					preferedTopologySamples[num2] = position;
					num2++;
				}
			}
		}
		if (Random.Range(0f, 1f) <= 0.9f && num2 > 0)
		{
			chosenPosition = preferedTopologySamples[Random.Range(0, num2)];
		}
		else if (num > 0)
		{
			chosenPosition = topologySamples[Random.Range(0, num)];
		}
		else
		{
			chosenPosition = fallbackPos;
		}
		Profiler.EndSample();
		return chosenPosition;
	}

	public virtual bool GetBestFleePosition(BaseNavigator navigator, AIBrainSenses senses, BaseEntity fleeFrom, Vector3 fallbackPos, float minRange, float maxRange, out Vector3 result)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)fleeFrom == (Object)null)
		{
			result = ((Component)navigator).transform.position;
			return false;
		}
		Vector3 dirFromThreat = Vector3Ex.Direction2D(((Component)navigator).transform.position, ((Component)fleeFrom).transform.position);
		if (TestFleeDirection(navigator, dirFromThreat, 0f, minRange, maxRange, out result))
		{
			return true;
		}
		bool flag = Random.Range(0, 2) == 1;
		if (TestFleeDirection(navigator, dirFromThreat, flag ? 45f : 315f, minRange, maxRange, out result))
		{
			return true;
		}
		if (TestFleeDirection(navigator, dirFromThreat, flag ? 315f : 45f, minRange, maxRange, out result))
		{
			return true;
		}
		if (TestFleeDirection(navigator, dirFromThreat, flag ? 90f : 270f, minRange, maxRange, out result))
		{
			return true;
		}
		if (TestFleeDirection(navigator, dirFromThreat, flag ? 270f : 90f, minRange, maxRange, out result))
		{
			return true;
		}
		if (TestFleeDirection(navigator, dirFromThreat, 135f + Random.Range(0f, 90f), minRange, maxRange, out result))
		{
			return true;
		}
		return false;
	}

	private bool TestFleeDirection(BaseNavigator navigator, Vector3 dirFromThreat, float offsetDegrees, float minRange, float maxRange, out Vector3 result)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		result = ((Component)navigator).transform.position;
		Vector3 val = Quaternion.Euler(0f, offsetDegrees, 0f) * dirFromThreat;
		Vector3 target = ((Component)navigator).transform.position + val * Random.Range(minRange, maxRange);
		if (!navigator.GetNearestNavmeshPosition(target, out var position, 20f))
		{
			return false;
		}
		if (!navigator.IsAcceptableWaterDepth(position))
		{
			return false;
		}
		result = position;
		return true;
	}

	public static Vector3 GetPointOnCircle(Vector3 center, float radius, float degrees)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		float num = center.x + radius * Mathf.Cos(degrees * ((float)Math.PI / 180f));
		float num2 = center.z + radius * Mathf.Sin(degrees * ((float)Math.PI / 180f));
		return new Vector3(num, center.y, num2);
	}
}
