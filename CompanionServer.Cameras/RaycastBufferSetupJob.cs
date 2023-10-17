using Unity.Collections;
using Unity.Jobs;

namespace CompanionServer.Cameras;

public struct RaycastBufferSetupJob : IJob
{
	public NativeArray<int> colliderIds;

	public NativeArray<byte> colliderMaterials;

	[WriteOnly]
	public NativeArray<int> colliderHits;

	public void Execute()
	{
		if (colliderIds.Length > 1)
		{
			SortByAscending(ref colliderIds, ref colliderMaterials, 0, colliderIds.Length - 1);
		}
		for (int i = 0; i < colliderHits.Length; i++)
		{
			colliderHits[i] = 0;
		}
	}

	private static void SortByAscending(ref NativeArray<int> colliderIds, ref NativeArray<byte> colliderMaterials, int leftIndex, int rightIndex)
	{
		int i = leftIndex;
		int num = rightIndex;
		int num2 = colliderIds[leftIndex];
		while (i <= num)
		{
			for (; colliderIds[i] < num2; i++)
			{
			}
			while (colliderIds[num] > num2)
			{
				num--;
			}
			if (i <= num)
			{
				int num3 = i;
				int num4 = num;
				int num5 = colliderIds[num];
				int num6 = colliderIds[i];
				int num8 = (colliderIds[num3] = num5);
				num8 = (colliderIds[num4] = num6);
				num6 = i;
				num5 = num;
				byte b = colliderMaterials[num];
				byte b2 = colliderMaterials[i];
				byte b4 = (colliderMaterials[num6] = b);
				b4 = (colliderMaterials[num5] = b2);
				i++;
				num--;
			}
		}
		if (leftIndex < num)
		{
			SortByAscending(ref colliderIds, ref colliderMaterials, leftIndex, num);
		}
		if (i < rightIndex)
		{
			SortByAscending(ref colliderIds, ref colliderMaterials, i, rightIndex);
		}
	}
}
