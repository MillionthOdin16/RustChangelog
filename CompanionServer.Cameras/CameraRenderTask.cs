using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace CompanionServer.Cameras;

public class CameraRenderTask : CustomYieldInstruction, IDisposable
{
	public const int MaxSamplesPerRender = 10000;

	public const int MaxColliders = 512;

	private static readonly Dictionary<(int, int), NativeArray<int2>> _samplePositions = new Dictionary<(int, int), NativeArray<int2>>();

	private NativeArray<RaycastCommand> _raycastCommands;

	private NativeArray<RaycastHit> _raycastHits;

	private NativeArray<int> _colliderIds;

	private NativeArray<byte> _colliderMaterials;

	private NativeArray<int> _colliderHits;

	private NativeArray<int> _raycastOutput;

	private NativeArray<int> _foundCollidersLength;

	private NativeArray<int> _foundColliders;

	private NativeArray<int> _outputDataLength;

	private NativeArray<byte> _outputData;

	private JobHandle? _pendingJob;

	private int _sampleCount;

	private int _colliderLength;

	public override bool keepWaiting
	{
		get
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			if (_pendingJob.HasValue)
			{
				JobHandle value = _pendingJob.Value;
				return !((JobHandle)(ref value)).IsCompleted;
			}
			return false;
		}
	}

	public CameraRenderTask()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		_raycastCommands = new NativeArray<RaycastCommand>(10000, (Allocator)4, (NativeArrayOptions)1);
		_raycastHits = new NativeArray<RaycastHit>(10000, (Allocator)4, (NativeArrayOptions)0);
		_colliderIds = new NativeArray<int>(512, (Allocator)4, (NativeArrayOptions)0);
		_colliderMaterials = new NativeArray<byte>(512, (Allocator)4, (NativeArrayOptions)0);
		_colliderHits = new NativeArray<int>(512, (Allocator)4, (NativeArrayOptions)0);
		_raycastOutput = new NativeArray<int>(10000, (Allocator)4, (NativeArrayOptions)0);
		_foundCollidersLength = new NativeArray<int>(1, (Allocator)4, (NativeArrayOptions)0);
		_foundColliders = new NativeArray<int>(10000, (Allocator)4, (NativeArrayOptions)0);
		_outputDataLength = new NativeArray<int>(1, (Allocator)4, (NativeArrayOptions)0);
		_outputData = new NativeArray<byte>(40000, (Allocator)4, (NativeArrayOptions)0);
	}

	~CameraRenderTask()
	{
		try
		{
			Dispose();
		}
		finally
		{
			((object)this).Finalize();
		}
	}

	public void Dispose()
	{
		_raycastCommands.Dispose();
		_raycastHits.Dispose();
		_colliderIds.Dispose();
		_colliderMaterials.Dispose();
		_colliderHits.Dispose();
		_raycastOutput.Dispose();
		_foundCollidersLength.Dispose();
		_foundColliders.Dispose();
		_outputDataLength.Dispose();
		_outputData.Dispose();
	}

	public void Reset()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		if (_pendingJob.HasValue)
		{
			JobHandle value = _pendingJob.Value;
			if (!((JobHandle)(ref value)).IsCompleted)
			{
				Debug.LogWarning((object)"CameraRenderTask is resetting before completion! This will cause it to synchronously block for completion.");
			}
			value = _pendingJob.Value;
			((JobHandle)(ref value)).Complete();
		}
		_pendingJob = null;
		_sampleCount = 0;
	}

	public int Start(int width, int height, float verticalFov, float nearPlane, float farPlane, int layerMask, Transform cameraTransform, int sampleCount, int sampleOffset, Dictionary<int, (byte MaterialIndex, int Age)> knownColliders)
	{
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_0238: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_0297: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0300: Unknown result type (might be due to invalid IL or missing references)
		//IL_0305: Unknown result type (might be due to invalid IL or missing references)
		//IL_0321: Unknown result type (might be due to invalid IL or missing references)
		//IL_0326: Unknown result type (might be due to invalid IL or missing references)
		//IL_032e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0333: Unknown result type (might be due to invalid IL or missing references)
		//IL_033b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0340: Unknown result type (might be due to invalid IL or missing references)
		//IL_034a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0350: Unknown result type (might be due to invalid IL or missing references)
		//IL_0352: Unknown result type (might be due to invalid IL or missing references)
		//IL_0357: Unknown result type (might be due to invalid IL or missing references)
		//IL_0360: Unknown result type (might be due to invalid IL or missing references)
		//IL_0366: Unknown result type (might be due to invalid IL or missing references)
		//IL_0368: Unknown result type (might be due to invalid IL or missing references)
		//IL_036d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0378: Unknown result type (might be due to invalid IL or missing references)
		//IL_0386: Unknown result type (might be due to invalid IL or missing references)
		//IL_038d: Unknown result type (might be due to invalid IL or missing references)
		//IL_038f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0394: Unknown result type (might be due to invalid IL or missing references)
		//IL_039c: Unknown result type (might be due to invalid IL or missing references)
		//IL_039e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03be: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c5: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)cameraTransform == (Object)null)
		{
			throw new ArgumentNullException("cameraTransform");
		}
		if (sampleCount <= 0 || sampleCount > 10000)
		{
			throw new ArgumentOutOfRangeException("sampleCount");
		}
		if (sampleOffset < 0)
		{
			throw new ArgumentOutOfRangeException("sampleOffset");
		}
		if (knownColliders == null)
		{
			throw new ArgumentNullException("knownColliders");
		}
		if (knownColliders.Count > 512)
		{
			throw new ArgumentException("Too many colliders", "knownColliders");
		}
		if (_pendingJob.HasValue)
		{
			throw new InvalidOperationException("A render job was already started for this instance.");
		}
		_sampleCount = sampleCount;
		_colliderLength = knownColliders.Count;
		int num = 0;
		foreach (KeyValuePair<int, (byte, int)> knownCollider in knownColliders)
		{
			_colliderIds[num] = knownCollider.Key;
			_colliderMaterials[num] = knownCollider.Value.Item1;
			num++;
		}
		NativeArray<int2> samplePositions = GetSamplePositions(width, height);
		_foundCollidersLength[0] = 0;
		RaycastBufferSetupJob raycastBufferSetupJob = default(RaycastBufferSetupJob);
		raycastBufferSetupJob.colliderIds = _colliderIds.GetSubArray(0, _colliderLength);
		raycastBufferSetupJob.colliderMaterials = _colliderMaterials.GetSubArray(0, _colliderLength);
		raycastBufferSetupJob.colliderHits = _colliderHits.GetSubArray(0, _colliderLength);
		RaycastBufferSetupJob raycastBufferSetupJob2 = raycastBufferSetupJob;
		RaycastRaySetupJob raycastRaySetupJob = default(RaycastRaySetupJob);
		raycastRaySetupJob.res = new float2((float)width, (float)height);
		raycastRaySetupJob.halfRes = new float2((float)width / 2f, (float)height / 2f);
		raycastRaySetupJob.aspectRatio = (float)width / (float)height;
		raycastRaySetupJob.worldHeight = 2f * Mathf.Tan((float)Math.PI / 360f * verticalFov);
		raycastRaySetupJob.cameraPos = float3.op_Implicit(cameraTransform.position);
		raycastRaySetupJob.cameraRot = quaternion.op_Implicit(cameraTransform.rotation);
		raycastRaySetupJob.nearPlane = nearPlane;
		raycastRaySetupJob.farPlane = farPlane;
		raycastRaySetupJob.layerMask = layerMask;
		raycastRaySetupJob.samplePositions = samplePositions;
		raycastRaySetupJob.sampleOffset = sampleOffset % samplePositions.Length;
		raycastRaySetupJob.raycastCommands = _raycastCommands.GetSubArray(0, sampleCount);
		RaycastRaySetupJob raycastRaySetupJob2 = raycastRaySetupJob;
		RaycastRayProcessingJob raycastRayProcessingJob = default(RaycastRayProcessingJob);
		raycastRayProcessingJob.cameraForward = float3.op_Implicit(-cameraTransform.forward);
		raycastRayProcessingJob.farPlane = farPlane;
		raycastRayProcessingJob.raycastHits = _raycastHits.GetSubArray(0, sampleCount);
		raycastRayProcessingJob.colliderIds = _colliderIds.GetSubArray(0, _colliderLength);
		raycastRayProcessingJob.colliderMaterials = _colliderMaterials.GetSubArray(0, _colliderLength);
		raycastRayProcessingJob.colliderHits = _colliderHits.GetSubArray(0, _colliderLength);
		raycastRayProcessingJob.outputs = _raycastOutput.GetSubArray(0, sampleCount);
		raycastRayProcessingJob.foundCollidersIndex = _foundCollidersLength;
		raycastRayProcessingJob.foundColliders = _foundColliders;
		RaycastRayProcessingJob raycastRayProcessingJob2 = raycastRayProcessingJob;
		RaycastColliderProcessingJob raycastColliderProcessingJob = default(RaycastColliderProcessingJob);
		raycastColliderProcessingJob.foundCollidersLength = _foundCollidersLength;
		raycastColliderProcessingJob.foundColliders = _foundColliders;
		RaycastColliderProcessingJob raycastColliderProcessingJob2 = raycastColliderProcessingJob;
		RaycastOutputCompressJob raycastOutputCompressJob = default(RaycastOutputCompressJob);
		raycastOutputCompressJob.rayOutputs = _raycastOutput.GetSubArray(0, sampleCount);
		raycastOutputCompressJob.dataLength = _outputDataLength;
		raycastOutputCompressJob.data = _outputData;
		RaycastOutputCompressJob raycastOutputCompressJob2 = raycastOutputCompressJob;
		JobHandle val = IJobExtensions.Schedule<RaycastBufferSetupJob>(raycastBufferSetupJob2, default(JobHandle));
		JobHandle val2 = IJobParallelForExtensions.Schedule<RaycastRaySetupJob>(raycastRaySetupJob2, sampleCount, 100, default(JobHandle));
		JobHandle val3 = RaycastCommand.ScheduleBatch(_raycastCommands.GetSubArray(0, sampleCount), _raycastHits.GetSubArray(0, sampleCount), 100, val2);
		JobHandle val4 = IJobParallelForExtensions.Schedule<RaycastRayProcessingJob>(raycastRayProcessingJob2, sampleCount, 100, JobHandle.CombineDependencies(val, val3));
		JobHandle val5 = IJobExtensions.Schedule<RaycastColliderProcessingJob>(raycastColliderProcessingJob2, val4);
		JobHandle val6 = IJobExtensions.Schedule<RaycastOutputCompressJob>(raycastOutputCompressJob2, val4);
		_pendingJob = JobHandle.CombineDependencies(val6, val5);
		return sampleOffset + sampleCount;
	}

	public int ExtractRayData(byte[] buffer, List<int> hitColliderIds = null, List<int> foundColliderIds = null)
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		if (buffer == null)
		{
			throw new ArgumentNullException("buffer");
		}
		int num = _sampleCount * 4;
		if (buffer.Length < num)
		{
			throw new ArgumentException("Output buffer is not large enough to hold all the ray data", "buffer");
		}
		if (!_pendingJob.HasValue)
		{
			throw new InvalidOperationException("Job was not started for this CameraRenderTask");
		}
		JobHandle value = _pendingJob.Value;
		if (!((JobHandle)(ref value)).IsCompleted)
		{
			Debug.LogWarning((object)"Trying to extract ray data from CameraRenderTask before completion! This will cause it to synchronously block for completion.");
		}
		value = _pendingJob.Value;
		((JobHandle)(ref value)).Complete();
		int num2 = _outputDataLength[0];
		NativeArray<byte>.Copy(_outputData.GetSubArray(0, num2), buffer, num2);
		if (hitColliderIds != null)
		{
			hitColliderIds.Clear();
			for (int i = 0; i < _colliderLength; i++)
			{
				if (_colliderHits[i] > 0)
				{
					hitColliderIds.Add(_colliderIds[i]);
				}
			}
		}
		if (foundColliderIds != null)
		{
			foundColliderIds.Clear();
			int num3 = _foundCollidersLength[0];
			for (int j = 0; j < num3; j++)
			{
				foundColliderIds.Add(_foundColliders[j]);
			}
		}
		return num2;
	}

	private static NativeArray<int2> GetSamplePositions(int width, int height)
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		if (width <= 0)
		{
			throw new ArgumentOutOfRangeException("width");
		}
		if (height <= 0)
		{
			throw new ArgumentOutOfRangeException("height");
		}
		(int, int) key = (width, height);
		if (_samplePositions.TryGetValue(key, out var value))
		{
			return value;
		}
		value._002Ector(width * height, (Allocator)4, (NativeArrayOptions)0);
		RaycastSamplePositionsJob raycastSamplePositionsJob = default(RaycastSamplePositionsJob);
		raycastSamplePositionsJob.res = new int2(width, height);
		raycastSamplePositionsJob.random = new Random(1337u);
		raycastSamplePositionsJob.positions = value;
		IJobExtensions.Run<RaycastSamplePositionsJob>(raycastSamplePositionsJob);
		_samplePositions.Add(key, value);
		return value;
	}

	public static void FreeCachedSamplePositions()
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		foreach (KeyValuePair<(int, int), NativeArray<int2>> samplePosition in _samplePositions)
		{
			samplePosition.Value.Dispose();
		}
		_samplePositions.Clear();
	}
}
