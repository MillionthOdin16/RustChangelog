using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace CompanionServer.Cameras;

public struct RaycastColliderProcessingJob : IJob
{
	public NativeArray<int> foundCollidersLength;

	public NativeArray<int> foundColliders;

	public void Execute()
	{
		int num = math.min(foundCollidersLength[0], foundColliders.Length);
		if (num <= 1)
		{
			return;
		}
		SortAscending(ref foundColliders, 0, num - 1);
		NativeArray<int> counts = default(NativeArray<int>);
		counts._002Ector(num, (Allocator)2, (NativeArrayOptions)0);
		int num2 = 0;
		int i = 0;
		while (i < num)
		{
			int num3 = foundColliders[i];
			int num4 = 1;
			for (; i < num && foundColliders[i] == num3; i++)
			{
				num4++;
			}
			foundColliders[num2] = num3;
			counts[num2] = num4;
			num2++;
		}
		SortByDescending(ref foundColliders, ref counts, 0, num2 - 1);
		counts.Dispose();
		int num5 = math.min(num2, 512);
		foundCollidersLength[0] = num5;
	}

	private static void SortByDescending(ref NativeArray<int> colliders, ref NativeArray<int> counts, int leftIndex, int rightIndex)
	{
		int i = leftIndex;
		int num = rightIndex;
		int num2 = counts[leftIndex];
		while (i <= num)
		{
			for (; counts[i] > num2; i++)
			{
			}
			while (counts[num] < num2)
			{
				num--;
			}
			if (i <= num)
			{
				int num3 = i;
				ref NativeArray<int> reference = ref colliders;
				int num4 = num;
				int num5 = colliders[num];
				int num6 = colliders[i];
				int num8 = (colliders[num3] = num5);
				num8 = (reference[num4] = num6);
				num6 = i;
				reference = ref counts;
				num5 = num;
				num4 = counts[num];
				num3 = counts[i];
				num8 = (counts[num6] = num4);
				num8 = (reference[num5] = num3);
				i++;
				num--;
			}
		}
		if (leftIndex < num)
		{
			SortByDescending(ref colliders, ref counts, leftIndex, num);
		}
		if (i < rightIndex)
		{
			SortByDescending(ref colliders, ref counts, i, rightIndex);
		}
	}

	private static void SortAscending(ref NativeArray<int> array, int leftIndex, int rightIndex)
	{
		int i = leftIndex;
		int num = rightIndex;
		int num2 = array[leftIndex];
		while (i <= num)
		{
			for (; array[i] < num2; i++)
			{
			}
			while (array[num] > num2)
			{
				num--;
			}
			if (i <= num)
			{
				int num3 = i;
				int num4 = num;
				int num5 = array[num];
				int num6 = array[i];
				int num8 = (array[num3] = num5);
				num8 = (array[num4] = num6);
				i++;
				num--;
			}
		}
		if (leftIndex < num)
		{
			SortAscending(ref array, leftIndex, num);
		}
		if (i < rightIndex)
		{
			SortAscending(ref array, i, rightIndex);
		}
	}
}
