using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public class FishSwarm : MonoBehaviour
{
	public FishShoal.FishType[] fishTypes;

	public FishShoal[] fishShoals;

	private Vector3? lastFishUpdatePosition;

	private void Awake()
	{
		fishShoals = new FishShoal[fishTypes.Length];
		for (int i = 0; i < fishTypes.Length; i++)
		{
			fishShoals[i] = new FishShoal(fishTypes[i]);
		}
		((MonoBehaviour)this).StartCoroutine(SpawnFish());
	}

	private IEnumerator SpawnFish()
	{
		while (true)
		{
			if (!Object.op_Implicit((Object)(object)TerrainMeta.WaterMap) || !Object.op_Implicit((Object)(object)TerrainMeta.HeightMap))
			{
				yield return CoroutineEx.waitForEndOfFrame;
				continue;
			}
			if (lastFishUpdatePosition.HasValue && Vector3.Distance(((Component)this).transform.position, lastFishUpdatePosition.Value) < 5f)
			{
				yield return CoroutineEx.waitForEndOfFrame;
			}
			FishShoal[] array = fishShoals;
			foreach (FishShoal fishShoal in array)
			{
				fishShoal.TrySpawn(float3.op_Implicit(((Component)this).transform.position));
				yield return CoroutineEx.waitForEndOfFrame;
			}
		}
	}

	private void Update()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		FishShoal[] array = fishShoals;
		foreach (FishShoal fishShoal in array)
		{
			fishShoal.OnUpdate(float3.op_Implicit(((Component)this).transform.position));
		}
	}

	private void LateUpdate()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		FishShoal[] array = fishShoals;
		foreach (FishShoal fishShoal in array)
		{
			fishShoal.OnLateUpdate(float3.op_Implicit(((Component)this).transform.position));
		}
	}

	private void OnDestroy()
	{
		FishShoal[] array = fishShoals;
		foreach (FishShoal fishShoal in array)
		{
			fishShoal.Dispose();
		}
	}

	private void OnDrawGizmos()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		Gizmos.DrawWireSphere(((Component)this).transform.position, 15f);
		Gizmos.DrawWireSphere(((Component)this).transform.position, 40f);
		if (Application.isPlaying)
		{
			FishShoal[] array = fishShoals;
			foreach (FishShoal fishShoal in array)
			{
				fishShoal.OnDrawGizmos();
			}
		}
	}
}
