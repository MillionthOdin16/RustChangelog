using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace CompanionServer.Cameras;

public struct RaycastSamplePositionsJob : IJob
{
	public int2 res;

	public Random random;

	public NativeArray<int2> positions;

	public void Execute()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		for (int i = 0; i < res.y; i++)
		{
			for (int j = 0; j < res.x; j++)
			{
				positions[num++] = new int2(j, i);
			}
		}
		for (num = res.x * res.y - 1; num >= 1; num--)
		{
			int num2 = ((Random)(ref random)).NextInt(num + 1);
			ref NativeArray<int2> reference = ref positions;
			int num3 = num;
			ref NativeArray<int2> reference2 = ref positions;
			int num4 = num2;
			int2 val = positions[num2];
			int2 val2 = positions[num];
			int2 val4 = (reference[num3] = val);
			val4 = (reference2[num4] = val2);
		}
	}
}
