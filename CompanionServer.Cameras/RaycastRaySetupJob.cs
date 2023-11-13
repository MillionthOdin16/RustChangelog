using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace CompanionServer.Cameras;

public struct RaycastRaySetupJob : IJobParallelFor
{
	public float2 res;

	public float2 halfRes;

	public float aspectRatio;

	public float worldHeight;

	public float3 cameraPos;

	public quaternion cameraRot;

	public float nearPlane;

	public float farPlane;

	public int layerMask;

	public int sampleOffset;

	[ReadOnly]
	public NativeArray<int2> samplePositions;

	[WriteOnly]
	[NativeMatchesParallelForLength]
	public NativeArray<RaycastCommand> raycastCommands;

	public void Execute(int index)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		int num;
		for (num = sampleOffset + index; num >= samplePositions.Length; num -= samplePositions.Length)
		{
		}
		float2 val = (float2.op_Implicit(samplePositions[num]) - halfRes) / res;
		float3 val2 = default(float3);
		((float3)(ref val2))._002Ector(val.x * worldHeight * aspectRatio, val.y * worldHeight, 1f);
		float3 val3 = math.mul(cameraRot, val2);
		float3 val4 = cameraPos + val3 * nearPlane;
		raycastCommands[index] = new RaycastCommand(float3.op_Implicit(val4), float3.op_Implicit(val3), farPlane, layerMask, 1);
	}
}
