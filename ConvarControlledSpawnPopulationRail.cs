using System.Collections.Generic;
using Facepunch;
using UnityEngine;

public class ConvarControlledSpawnPopulationRail : ConvarControlledSpawnPopulation
{
	private const float MIN_MARGIN = 60f;

	public override bool GetSpawnPosOverride(Prefab<Spawnable> prefab, ref Vector3 newPos, ref Quaternion newRot)
	{
		if (TrainTrackSpline.SidingSplines.Count <= 0)
		{
			return false;
		}
		TrainCar component = prefab.Object.GetComponent<TrainCar>();
		if ((Object)(object)component == (Object)null)
		{
			Debug.LogError((object)(((object)this).GetType().Name + ": Train prefab has no TrainCar component: " + ((Object)prefab.Object).name));
			return false;
		}
		int num = 0;
		foreach (TrainTrackSpline sidingSpline in TrainTrackSpline.SidingSplines)
		{
			if (sidingSpline.HasAnyUsersOfType(TrainCar.TrainCarType.Engine))
			{
				num++;
			}
		}
		bool flag = component.CarType == TrainCar.TrainCarType.Engine;
		int num2 = 0;
		while (num2 < 20)
		{
			num2++;
			TrainTrackSpline trainTrackSpline = null;
			if (flag)
			{
				foreach (TrainTrackSpline sidingSpline2 in TrainTrackSpline.SidingSplines)
				{
					if (!sidingSpline2.HasAnyUsersOfType(TrainCar.TrainCarType.Engine))
					{
						trainTrackSpline = sidingSpline2;
						break;
					}
				}
			}
			if ((Object)(object)trainTrackSpline == (Object)null)
			{
				int index = Random.Range(0, TrainTrackSpline.SidingSplines.Count);
				trainTrackSpline = TrainTrackSpline.SidingSplines[index];
			}
			if ((Object)(object)trainTrackSpline != (Object)null && TryGetRandomPointOnSpline(trainTrackSpline, component, out newPos, out newRot))
			{
				return true;
			}
		}
		return false;
	}

	public override void OnPostFill(SpawnHandler spawnHandler)
	{
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		List<Prefab<Spawnable>> list = Pool.GetList<Prefab<Spawnable>>();
		Prefab<Spawnable>[] prefabs = Prefabs;
		foreach (Prefab<Spawnable> prefab in prefabs)
		{
			TrainCar component = prefab.Object.GetComponent<TrainCar>();
			if ((Object)(object)component != (Object)null && component.CarType == TrainCar.TrainCarType.Engine)
			{
				list.Add(prefab);
			}
		}
		foreach (TrainTrackSpline sidingSpline in TrainTrackSpline.SidingSplines)
		{
			if (sidingSpline.HasAnyUsersOfType(TrainCar.TrainCarType.Engine))
			{
				continue;
			}
			int num = Random.Range(0, list.Count);
			Prefab<Spawnable> prefab2 = Prefabs[num];
			TrainCar component2 = prefab2.Object.GetComponent<TrainCar>();
			if ((Object)(object)component2 == (Object)null)
			{
				continue;
			}
			int num2 = 0;
			while (num2 < 20)
			{
				num2++;
				if (TryGetRandomPointOnSpline(sidingSpline, component2, out var pos, out var rot))
				{
					spawnHandler.Spawn(this, prefab2, pos, rot);
					break;
				}
			}
		}
		Pool.FreeList<Prefab<Spawnable>>(ref list);
	}

	protected override int GetPrefabWeight(Prefab<Spawnable> prefab)
	{
		int num = ((!Object.op_Implicit((Object)(object)prefab.Parameters)) ? 1 : prefab.Parameters.Count);
		TrainCar component = prefab.Object.GetComponent<TrainCar>();
		if ((Object)(object)component != (Object)null)
		{
			if (component.CarType == TrainCar.TrainCarType.Wagon)
			{
				num *= TrainCar.wagons_per_engine;
			}
		}
		else
		{
			Debug.LogError((object)(((object)this).GetType().Name + ": No TrainCar script on train prefab " + ((Object)prefab.Object).name));
		}
		return num;
	}

	private bool TryGetRandomPointOnSpline(TrainTrackSpline spline, TrainCar trainCar, out Vector3 pos, out Quaternion rot)
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		float length = spline.GetLength();
		if (length < 65f)
		{
			pos = Vector3.zero;
			rot = Quaternion.identity;
			return false;
		}
		float distance = Random.Range(60f, length - 60f);
		pos = spline.GetPointAndTangentCubicHermiteWorld(distance, out var tangent) + Vector3.up * 0.5f;
		rot = Quaternion.LookRotation(tangent);
		float radius = Vector3Ex.Max(((Bounds)(ref trainCar.bounds)).extents);
		List<Collider> list = Pool.GetList<Collider>();
		GamePhysics.OverlapSphere(pos, radius, list, 32768, (QueryTriggerInteraction)1);
		bool result = true;
		foreach (Collider item in list)
		{
			if (!trainCar.ColliderIsPartOfTrain(item))
			{
				result = false;
				break;
			}
		}
		Pool.FreeList<Collider>(ref list);
		return result;
	}
}
