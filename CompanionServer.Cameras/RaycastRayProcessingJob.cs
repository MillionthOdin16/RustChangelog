using System.Threading;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace CompanionServer.Cameras;

public struct RaycastRayProcessingJob : IJobParallelFor
{
	public float3 cameraForward;

	public float farPlane;

	[ReadOnly]
	public NativeArray<RaycastHit> raycastHits;

	[ReadOnly]
	public NativeArray<int> colliderIds;

	[ReadOnly]
	public NativeArray<byte> colliderMaterials;

	[WriteOnly]
	[NativeDisableParallelForRestriction]
	public NativeArray<int> colliderHits;

	[WriteOnly]
	[NativeMatchesParallelForLength]
	public NativeArray<int> outputs;

	[NativeDisableParallelForRestriction]
	public NativeArray<int> foundCollidersIndex;

	[NativeDisableParallelForRestriction]
	public NativeArray<int> foundColliders;

	public void Execute(int index)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		ref readonly RaycastHit @readonly = ref raycastHits.GetReadonly<RaycastHit>(index);
		int colliderId = @readonly.GetColliderId();
		bool flag = colliderId != 0;
		byte b = 0;
		if (flag)
		{
			int num = Interlocked.Increment(ref BurstUtil.Get(in foundCollidersIndex, 0));
			if (num <= foundColliders.Length)
			{
				foundColliders[num - 1] = colliderId;
			}
			int num2 = BinarySearch(colliderIds, colliderId);
			if (num2 >= 0)
			{
				b = colliderMaterials[num2];
				Interlocked.Increment(ref BurstUtil.Get(in colliderHits, num2));
			}
		}
		float distance;
		RaycastHit val;
		if (!flag)
		{
			distance = farPlane;
		}
		else
		{
			val = @readonly;
			distance = ((RaycastHit)(ref val)).distance;
		}
		float num3 = distance;
		if (b == 7)
		{
			b = 0;
			num3 *= 1.1f;
		}
		float num4 = math.clamp(num3 / farPlane, 0f, 1f);
		float3 val2 = cameraForward;
		val = @readonly;
		float num5 = math.max(math.dot(val2, float3.op_Implicit(((RaycastHit)(ref val)).normal)), 0f);
		ushort num6 = (ushort)(num4 * 1023f);
		byte b2 = (byte)(num5 * 63f);
		outputs[index] = (num6 >> 8 << 24) | ((num6 & 0xFF) << 16) | (b2 << 8) | b;
	}

	private static int BinarySearch(NativeArray<int> haystack, int needle)
	{
		int num = 0;
		int num2 = haystack.Length - 1;
		while (num <= num2)
		{
			int num3 = num + (num2 - num / 2);
			int num4 = Compare(haystack[num3], needle);
			if (num4 == 0)
			{
				return num3;
			}
			if (num4 < 0)
			{
				num = num3 + 1;
			}
			else
			{
				num2 = num3 - 1;
			}
		}
		return ~num;
	}

	private static int Compare(int x, int y)
	{
		if (x < y)
		{
			return -1;
		}
		if (x > y)
		{
			return 1;
		}
		return 0;
	}
}
