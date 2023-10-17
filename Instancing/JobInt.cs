using System;
using Unity.Collections;

namespace Instancing;

public struct JobInt
{
	private NativeArray<int> Array;

	public int Value
	{
		get
		{
			if (!Array.IsCreated)
			{
				throw new InvalidOperationException("You must call 'JobInt.Create()' before using this in a job");
			}
			return Array[0];
		}
		set
		{
			if (!Array.IsCreated)
			{
				throw new InvalidOperationException("You must call 'JobInt.Create()' before using this in a job");
			}
			Array[0] = value;
		}
	}

	public static JobInt Create()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		JobInt result = default(JobInt);
		result.Array = new NativeArray<int>(1, (Allocator)4, (NativeArrayOptions)1);
		return result;
	}

	public static void Destroy(JobInt instance)
	{
		NativeArrayEx.SafeDispose(ref instance.Array);
	}
}
