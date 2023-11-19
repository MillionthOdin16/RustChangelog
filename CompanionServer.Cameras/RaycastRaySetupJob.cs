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
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
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
