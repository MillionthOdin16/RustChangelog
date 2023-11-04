using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Profiling;
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
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
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
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
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
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
			//IL_0118: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_0132: Unknown result type (might be due to invalid IL or missing references)
			//IL_013f: Unknown result type (might be due to invalid IL or missing references)
			//IL_019e: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_020a: Unknown result type (might be due to invalid IL or missing references)
			//IL_020f: Unknown result type (might be due to invalid IL or missing references)
			//IL_022c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0239: Unknown result type (might be due to invalid IL or missing references)
			//IL_024c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0251: Unknown result type (might be due to invalid IL or missing references)
			//IL_0259: Unknown result type (might be due to invalid IL or missing references)
			//IL_0264: Unknown result type (might be due to invalid IL or missing references)
			//IL_0269: Unknown result type (might be due to invalid IL or missing references)
			//IL_026e: Unknown result type (might be due to invalid IL or missing references)
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
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Expected O, but got Unknown
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Expected O, but got Unknown
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
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		float2 val = ((Random)(ref random)).NextFloat2Direction();
		return spawnPos + new float3(val.x, 0f, val.y) * ((Random)(ref random)).NextFloat(10f, 15f);
	}

	private int GetPopulationScaleForPoint(float3 cameraPosition)
	{
		return 1;
	}

	public void TrySpawn(float3 cameraPosition)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		float num = TerrainMeta.WaterMap.GetHeight(float3.op_Implicit(cameraPosition)) - 3f;
		float height = TerrainMeta.HeightMap.GetHeight(float3.op_Implicit(cameraPosition));
		if (math.abs(num - height) < 4f || num < height)
		{
			return;
		}
		int num2 = fishCount[0];
		int num3 = Mathf.CeilToInt((float)(fishType.maxCount * GetPopulationScaleForPoint(cameraPosition)));
		int num4 = num3 - num2;
		if (num4 <= 0)
		{
			return;
		}
		uint num5 = (uint)(Time.frameCount + fishType.mesh.vertexCount);
		int num6 = fishCount[0];
		int num7 = math.min(num6 + num4, fishType.maxCount);
		Random random = default(Random);
		for (int i = num6; i < num7; i++)
		{
			((Random)(ref random))._002Ector((uint)(i * 3245 + num5));
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
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		Profiler.BeginSample("FishShoal.UpdateJobs");
		UpdateJobs(cameraPosition);
		Profiler.EndSample();
	}

	private unsafe void UpdateJobs(float3 cameraPosition)
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_0259: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
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
			Profiler.BeginSample("Schedule");
			jobHandle = IJobExtensions.Schedule<FishCollisionGatherJob>(fishCollisionGatherJob2, default(JobHandle));
			jobHandle = RaycastCommand.ScheduleBatch(castCommands, castResults, 5, jobHandle);
			jobHandle = IJobExtensions.Schedule<FishCollisionProcessJob>(fishCollisionProcessJob2, jobHandle);
			jobHandle = IJobParallelForExtensions.Schedule<FishUpdateJob>(fishUpdateJob2, num, 10, jobHandle);
			jobHandle = IJobExtensions.Schedule<KillFish>(killFish2, jobHandle);
			Profiler.EndSample();
		}
	}

	public void OnLateUpdate(float3 cameraPosition)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
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

	public void OnDrawGizmos()
	{
		((JobHandle)(ref jobHandle)).Complete();
		if (fishCount[0] != 0)
		{
		}
	}
}
