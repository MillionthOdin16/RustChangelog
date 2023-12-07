using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

public class FishShoal : IDisposable
{
	[Serializable]
	public struct FishType
	{
		public Mesh mesh;

		public Material material;

		public int castsPerFrame;

		public int maxCount;

		public float minSpeed;

		public float maxSpeed;

		public float idealDepth;

		public float minTurnSpeed;

		public float maxTurnSpeed;
	}

	public struct FishData
	{
		public bool isAlive;

		public float updateTime;

		public float startleTime;

		public float spawnX;

		public float spawnZ;

		public float destinationX;

		public float destinationZ;

		public float directionX;

		public float directionZ;

		public float speed;

		public float scale;
	}

	public struct FishRenderData
	{
		public float3 position;

		public float rotation;

		public float scale;

		public float distance;
	}

	public struct FishCollisionGatherJob : IJob
	{
		public int layerMask;

		public uint seed;

		public int castCount;

		public int fishCount;

		public NativeArray<RaycastCommand> castCommands;

		public NativeArray<FishData> fishDataArray;

		public NativeArray<FishRenderData> fishRenderDataArray;

		public NativeArray<int> fishCastIndices;

		public void Execute()
		{
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			Random val = default(Random);
			((Random)(ref val))._002Ector(seed);
			int length = castCommands.Length;
			for (int i = 0; i < length; i++)
			{
				RaycastCommand val3;
				if (i >= castCount)
				{
					val3 = (castCommands[i] = default(RaycastCommand));
					continue;
				}
				int num = ((Random)(ref val)).NextInt(0, fishCount);
				FishData fishData = fishDataArray[num];
				FishRenderData fishRenderData = fishRenderDataArray[num];
				ref NativeArray<RaycastCommand> reference = ref castCommands;
				int num2 = i;
				val3 = default(RaycastCommand);
				((RaycastCommand)(ref val3)).from = float3.op_Implicit(fishRenderData.position);
				((RaycastCommand)(ref val3)).direction = float3.op_Implicit(new float3(fishData.directionX, 0f, fishData.directionZ));
				((RaycastCommand)(ref val3)).distance = 4f;
				((RaycastCommand)(ref val3)).layerMask = layerMask;
				((RaycastCommand)(ref val3)).maxHits = 1;
				reference[num2] = val3;
				fishCastIndices[i] = num;
			}
		}
	}

	public struct FishCollisionProcessJob : IJob
	{
		public int castCount;

		public NativeArray<FishData> fishDataArray;

		[ReadOnly]
		public NativeArray<RaycastHit> castResults;

		[ReadOnly]
		public NativeArray<int> fishCastIndices;

		[ReadOnly]
		public NativeArray<FishRenderData> fishRenderDataArray;

		public void Execute()
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00af: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < castCount; i++)
			{
				RaycastHit val = castResults[i];
				if (((RaycastHit)(ref val)).normal != default(Vector3))
				{
					int num = fishCastIndices[i];
					FishData fishData = fishDataArray[num];
					if (fishData.startleTime <= 0f)
					{
						FishRenderData fishRenderData = fishRenderDataArray[num];
						float2 xz = ((float3)(ref fishRenderData.position)).xz;
						val = castResults[i];
						float x = ((RaycastHit)(ref val)).point.x;
						val = castResults[i];
						float2 val2 = math.normalize(new float2(x, ((RaycastHit)(ref val)).point.z) - xz);
						float2 val3 = xz - val2 * 8f;
						fishData.destinationX = val3.x;
						fishData.destinationZ = val3.y;
						fishData.startleTime = 2f;
						fishData.updateTime = 6f;
						fishDataArray[num] = fishData;
					}
				}
			}
		}
	}

	public struct FishUpdateJob : IJobParallelFor
	{
		[ReadOnly]
		public float3 cameraPosition;

		[ReadOnly]
		public uint seed;

		[ReadOnly]
		public float dt;

		[ReadOnly]
		public float minSpeed;

		[ReadOnly]
		public float maxSpeed;

		[ReadOnly]
		public float minTurnSpeed;

		[ReadOnly]
		public float maxTurnSpeed;

		[ReadOnly]
		public float minDepth;

		[NativeDisableUnsafePtrRestriction]
		public unsafe FishData* fishDataArray;

		[NativeDisableUnsafePtrRestriction]
		public unsafe FishRenderData* fishRenderDataArray;

		public unsafe void Execute(int i)
		{
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			//IL_010e: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
			//IL_0179: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0181: Unknown result type (might be due to invalid IL or missing references)
			//IL_0186: Unknown result type (might be due to invalid IL or missing references)
			//IL_018b: Unknown result type (might be due to invalid IL or missing references)
			//IL_018d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0194: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
			//IL_0206: Unknown result type (might be due to invalid IL or missing references)
			//IL_0213: Unknown result type (might be due to invalid IL or missing references)
			//IL_0226: Unknown result type (might be due to invalid IL or missing references)
			//IL_022b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0233: Unknown result type (might be due to invalid IL or missing references)
			//IL_023e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0243: Unknown result type (might be due to invalid IL or missing references)
			//IL_0248: Unknown result type (might be due to invalid IL or missing references)
			FishData* ptr = fishDataArray + i;
			FishRenderData* ptr2 = fishRenderDataArray + i;
			Random random = default(Random);
			((Random)(ref random))._002Ector((uint)(i * 3245 + seed));
			float num = math.distancesq(cameraPosition, ptr2->position);
			bool flag = ptr->startleTime > 0f;
			if (num > math.pow(40f, 2f) || ((float3)(&ptr2->position)).y > minDepth)
			{
				ptr->isAlive = false;
				return;
			}
			if (!flag && num < 100f)
			{
				ptr->startleTime = 2f;
				flag = true;
			}
			float3 val = default(float3);
			((float3)(ref val))._002Ector(ptr->destinationX, ((float3)(&ptr2->position)).y, ptr->destinationZ);
			if (ptr->updateTime >= 8f || math.distancesq(val, ptr2->position) < 1f)
			{
				float3 target = GetTarget(new float3(ptr->spawnX, 0f, ptr->spawnZ), ref random);
				ptr->updateTime = 0f;
				ptr->destinationX = target.x;
				ptr->destinationZ = target.z;
			}
			ptr2->scale = math.lerp(ptr2->scale, ptr->scale, dt * 5f);
			ptr->speed = math.lerp(ptr->speed, flag ? maxSpeed : minSpeed, dt * 4f);
			float3 val2 = math.normalize(val - ptr2->position);
			float a = math.atan2(val2.z, val2.x);
			ptr2->rotation = 0f - ptr2->rotation + (float)Math.PI / 2f;
			float num2 = (flag ? maxTurnSpeed : minTurnSpeed);
			ptr2->rotation = LerpAngle(ptr2->rotation, a, dt * num2);
			float3 zero = float3.zero;
			math.sincos(ptr2->rotation, ref zero.z, ref zero.x);
			ptr->directionX = zero.x;
			ptr->directionZ = zero.z;
			float3* position = &ptr2->position;
			System.Runtime.CompilerServices.Unsafe.Write(position, *position + zero * ptr->speed * dt);
			ptr2->rotation = 0f - ptr2->rotation + (float)Math.PI / 2f;
			ptr2->distance += ptr->speed * dt;
			ptr->updateTime += dt;
			ptr->startleTime -= dt;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static float LerpAngle(float a0, float a1, float t)
		{
			float num = a1 - a0;
			num = math.clamp(num - math.floor(num / ((float)Math.PI * 2f)) * ((float)Math.PI * 2f), 0f, (float)Math.PI * 2f);
			return math.lerp(a0, a0 + ((num > (float)Math.PI) ? (num - (float)Math.PI * 2f) : num), t);
		}
	}

	public struct KillFish : IJob
	{
		public NativeArray<FishData> fishDataArray;

		public NativeArray<FishRenderData> fishRenderDataArray;

		public NativeArray<int> fishCount;

		public void Execute()
		{
			int num = fishCount[0];
			for (int num2 = num - 1; num2 >= 0; num2--)
			{
				if (!fishDataArray[num2].isAlive)
				{
					if (num2 < num - 1)
					{
						fishDataArray[num2] = fishDataArray[num - 1];
						fishRenderDataArray[num2] = fishRenderDataArray[num - 1];
					}
					num--;
				}
			}
			fishCount[0] = num;
		}
	}

	private const float maxFishDistance = 40f;

	private FishType fishType;

	private JobHandle jobHandle;

	private NativeArray<RaycastCommand> castCommands;

	private NativeArray<RaycastHit> castResults;

	private NativeArray<int> fishCastIndices;

	private NativeArray<FishData> fishData;

	private NativeArray<FishRenderData> fishRenderData;

	private NativeArray<int> fishCount;

	private MaterialPropertyBlock materialPropertyBlock;

	private ComputeBuffer fishBuffer;

	public FishShoal(FishType fishType)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Expected O, but got Unknown
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Expected O, but got Unknown
		this.fishType = fishType;
		castCommands = new NativeArray<RaycastCommand>(fishType.castsPerFrame, (Allocator)4, (NativeArrayOptions)1);
		castResults = new NativeArray<RaycastHit>(fishType.castsPerFrame, (Allocator)4, (NativeArrayOptions)1);
		fishCastIndices = new NativeArray<int>(fishType.castsPerFrame, (Allocator)4, (NativeArrayOptions)1);
		fishData = new NativeArray<FishData>(fishType.maxCount, (Allocator)4, (NativeArrayOptions)1);
		fishRenderData = new NativeArray<FishRenderData>(fishType.maxCount, (Allocator)4, (NativeArrayOptions)1);
		fishCount = new NativeArray<int>(1, (Allocator)4, (NativeArrayOptions)1);
		fishBuffer = new ComputeBuffer(fishType.maxCount, UnsafeUtility.SizeOf<FishRenderData>());
		materialPropertyBlock = new MaterialPropertyBlock();
		materialPropertyBlock.SetBuffer("_FishData", fishBuffer);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static float3 GetTarget(float3 spawnPos, ref Random random)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		float2 val = ((Random)(ref random)).NextFloat2Direction();
		return spawnPos + new float3(val.x, 0f, val.y) * ((Random)(ref random)).NextFloat(10f, 15f);
	}

	private int GetPopulationScaleForPoint(float3 cameraPosition)
	{
		return 1;
	}

	public void TrySpawn(float3 cameraPosition)
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		float num = TerrainMeta.WaterMap.GetHeight(float3.op_Implicit(cameraPosition)) - 3f;
		float height = TerrainMeta.HeightMap.GetHeight(float3.op_Implicit(cameraPosition));
		if (math.abs(num - height) < 4f || num < height)
		{
			return;
		}
		int num2 = fishCount[0];
		int num3 = Mathf.CeilToInt((float)(fishType.maxCount * GetPopulationScaleForPoint(cameraPosition))) - num2;
		if (num3 <= 0)
		{
			return;
		}
		uint num4 = (uint)(Time.frameCount + fishType.mesh.vertexCount);
		int num5 = fishCount[0];
		int num6 = math.min(num5 + num3, fishType.maxCount);
		Random random = default(Random);
		for (int i = num5; i < num6; i++)
		{
			((Random)(ref random))._002Ector((uint)(i * 3245 + num4));
			float3 val = cameraPosition + ((Random)(ref random)).NextFloat3Direction() * ((Random)(ref random)).NextFloat(40f);
			val.y = ((Random)(ref random)).NextFloat(math.max(height + 1f, cameraPosition.y - 30f), math.min(num, cameraPosition.y + 30f));
			if (!((Object)(object)WaterSystem.Instance == (Object)null) && WaterLevel.Test(float3.op_Implicit(val), waves: false, volumes: false) && !(TerrainMeta.HeightMap.GetHeight(float3.op_Implicit(val)) > val.y))
			{
				float3 target = GetTarget(val, ref random);
				float3 val2 = math.normalize(target - val);
				fishData[num2] = new FishData
				{
					isAlive = true,
					spawnX = val.x,
					spawnZ = val.z,
					destinationX = target.x,
					destinationZ = target.z,
					scale = ((Random)(ref random)).NextFloat(0.9f, 1.4f)
				};
				fishRenderData[num2] = new FishRenderData
				{
					position = val,
					rotation = math.atan2(val2.z, val2.x),
					scale = 0f
				};
				num2++;
			}
		}
		fishCount[0] = num2;
	}

	public void OnUpdate(float3 cameraPosition)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		UpdateJobs(cameraPosition);
	}

	private unsafe void UpdateJobs(float3 cameraPosition)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		((JobHandle)(ref jobHandle)).Complete();
		int num = fishCount[0];
		if (num != 0)
		{
			float num2 = (((Object)(object)TerrainMeta.WaterMap == (Object)null) ? 0f : (TerrainMeta.WaterMap.GetHeight(float3.op_Implicit(cameraPosition)) - 3f));
			int castCount = math.min(fishType.castsPerFrame, num);
			uint seed = (uint)(Time.frameCount + fishType.mesh.vertexCount);
			FishCollisionGatherJob fishCollisionGatherJob = default(FishCollisionGatherJob);
			fishCollisionGatherJob.layerMask = -1;
			fishCollisionGatherJob.seed = seed;
			fishCollisionGatherJob.castCount = castCount;
			fishCollisionGatherJob.fishCount = num;
			fishCollisionGatherJob.castCommands = castCommands;
			fishCollisionGatherJob.fishCastIndices = fishCastIndices;
			fishCollisionGatherJob.fishDataArray = fishData;
			fishCollisionGatherJob.fishRenderDataArray = fishRenderData;
			FishCollisionGatherJob fishCollisionGatherJob2 = fishCollisionGatherJob;
			FishCollisionProcessJob fishCollisionProcessJob = default(FishCollisionProcessJob);
			fishCollisionProcessJob.castCount = castCount;
			fishCollisionProcessJob.castResults = castResults;
			fishCollisionProcessJob.fishCastIndices = fishCastIndices;
			fishCollisionProcessJob.fishDataArray = fishData;
			fishCollisionProcessJob.fishRenderDataArray = fishRenderData;
			FishCollisionProcessJob fishCollisionProcessJob2 = fishCollisionProcessJob;
			FishUpdateJob fishUpdateJob = default(FishUpdateJob);
			fishUpdateJob.cameraPosition = cameraPosition;
			fishUpdateJob.seed = seed;
			fishUpdateJob.dt = Time.deltaTime;
			fishUpdateJob.minSpeed = fishType.minSpeed;
			fishUpdateJob.maxSpeed = fishType.maxSpeed;
			fishUpdateJob.minTurnSpeed = fishType.minTurnSpeed;
			fishUpdateJob.maxTurnSpeed = fishType.maxTurnSpeed;
			fishUpdateJob.fishDataArray = (FishData*)NativeArrayUnsafeUtility.GetUnsafePtr<FishData>(fishData);
			fishUpdateJob.fishRenderDataArray = (FishRenderData*)NativeArrayUnsafeUtility.GetUnsafePtr<FishRenderData>(fishRenderData);
			fishUpdateJob.minDepth = num2 - 3f;
			FishUpdateJob fishUpdateJob2 = fishUpdateJob;
			KillFish killFish = default(KillFish);
			killFish.fishCount = fishCount;
			killFish.fishDataArray = fishData;
			killFish.fishRenderDataArray = fishRenderData;
			KillFish killFish2 = killFish;
			jobHandle = IJobExtensions.Schedule<FishCollisionGatherJob>(fishCollisionGatherJob2, default(JobHandle));
			jobHandle = RaycastCommand.ScheduleBatch(castCommands, castResults, 5, jobHandle);
			jobHandle = IJobExtensions.Schedule<FishCollisionProcessJob>(fishCollisionProcessJob2, jobHandle);
			jobHandle = IJobParallelForExtensions.Schedule<FishUpdateJob>(fishUpdateJob2, num, 10, jobHandle);
			jobHandle = IJobExtensions.Schedule<KillFish>(killFish2, jobHandle);
		}
	}

	public void OnLateUpdate(float3 cameraPosition)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		((JobHandle)(ref jobHandle)).Complete();
		if (fishCount[0] != 0)
		{
			Bounds val = default(Bounds);
			((Bounds)(ref val))._002Ector(float3.op_Implicit(cameraPosition), Vector3.one * 40f);
			fishBuffer.SetData<FishRenderData>(fishRenderData);
			Graphics.DrawMeshInstancedProcedural(fishType.mesh, 0, fishType.material, val, fishCount[0], materialPropertyBlock, (ShadowCastingMode)1, true, 0, (Camera)null, (LightProbeUsage)1, (LightProbeProxyVolume)null);
		}
	}

	public void Dispose()
	{
		((JobHandle)(ref jobHandle)).Complete();
		castCommands.Dispose();
		castResults.Dispose();
		fishCastIndices.Dispose();
		fishData.Dispose();
		fishRenderData.Dispose();
		fishCount.Dispose();
		fishBuffer.Dispose();
	}

	public void OnDrawGizmosSelected()
	{
		((JobHandle)(ref jobHandle)).Complete();
		_ = fishCount[0];
	}
}
