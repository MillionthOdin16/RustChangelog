using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Profiling;

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
			int result;
			if (_pendingJob.HasValue)
			{
				JobHandle value = _pendingJob.Value;
				result = ((!((JobHandle)(ref value)).IsCompleted) ? 1 : 0);
			}
			else
			{
				result = 0;
			}
			return (byte)result != 0;
		}
	}

	public CameraRenderTask()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		Profiler.BeginSample("CameraRenderTask.ctor");
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
		Profiler.EndSample();
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
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		Profiler.BeginSample("CameraRenderTask.Reset");
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
		Profiler.EndSample();
	}

	public int Start(int width, int height, float verticalFov, float nearPlane, float farPlane, int layerMask, Transform cameraTransform, int sampleCount, int sampleOffset, Dictionary<int, (byte MaterialIndex, int Age)> knownColliders)
	{
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02da: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0308: Unknown result type (might be due to invalid IL or missing references)
		//IL_030d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0315: Unknown result type (might be due to invalid IL or missing references)
		//IL_031a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0322: Unknown result type (might be due to invalid IL or missing references)
		//IL_0327: Unknown result type (might be due to invalid IL or missing references)
		//IL_033b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0340: Unknown result type (might be due to invalid IL or missing references)
		//IL_0348: Unknown result type (might be due to invalid IL or missing references)
		//IL_034d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0369: Unknown result type (might be due to invalid IL or missing references)
		//IL_036e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0376: Unknown result type (might be due to invalid IL or missing references)
		//IL_037b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0383: Unknown result type (might be due to invalid IL or missing references)
		//IL_0388: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0400: Unknown result type (might be due to invalid IL or missing references)
		//IL_0405: Unknown result type (might be due to invalid IL or missing references)
		//IL_0409: Unknown result type (might be due to invalid IL or missing references)
		//IL_040b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0410: Unknown result type (might be due to invalid IL or missing references)
		//IL_0414: Unknown result type (might be due to invalid IL or missing references)
		//IL_0416: Unknown result type (might be due to invalid IL or missing references)
		//IL_041b: Unknown result type (might be due to invalid IL or missing references)
		//IL_041e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0420: Unknown result type (might be due to invalid IL or missing references)
		//IL_0422: Unknown result type (might be due to invalid IL or missing references)
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
		Profiler.BeginSample("CameraRenderTask.Start");
		Profiler.BeginSample("Setup");
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
		Profiler.EndSample();
		Profiler.BeginSample("Scheduling");
		JobHandle val = IJobExtensions.Schedule<RaycastBufferSetupJob>(raycastBufferSetupJob2, default(JobHandle));
		JobHandle val2 = IJobParallelForExtensions.Schedule<RaycastRaySetupJob>(raycastRaySetupJob2, sampleCount, 100, default(JobHandle));
		JobHandle val3 = RaycastCommand.ScheduleBatch(_raycastCommands.GetSubArray(0, sampleCount), _raycastHits.GetSubArray(0, sampleCount), 100, val2);
		JobHandle val4 = IJobParallelForExtensions.Schedule<RaycastRayProcessingJob>(raycastRayProcessingJob2, sampleCount, 100, JobHandle.CombineDependencies(val, val3));
		JobHandle val5 = IJobExtensions.Schedule<RaycastColliderProcessingJob>(raycastColliderProcessingJob2, val4);
		JobHandle val6 = IJobExtensions.Schedule<RaycastOutputCompressJob>(raycastOutputCompressJob2, val4);
		_pendingJob = JobHandle.CombineDependencies(val6, val5);
		Profiler.EndSample();
		Profiler.EndSample();
		return sampleOffset + sampleCount;
	}

	public int ExtractRayData(byte[] buffer, List<int> hitColliderIds = null, List<int> foundColliderIds = null)
	{
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
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
		NativeArray<byte> subArray = _outputData.GetSubArray(0, num2);
		NativeArray<byte>.Copy(subArray, buffer, num2);
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
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
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
		RaycastSamplePositionsJob raycastSamplePositionsJob2 = raycastSamplePositionsJob;
		IJobExtensions.Run<RaycastSamplePositionsJob>(raycastSamplePositionsJob2);
		_samplePositions.Add(key, value);
		return value;
	}

	public static void FreeCachedSamplePositions()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		foreach (KeyValuePair<(int, int), NativeArray<int2>> samplePosition in _samplePositions)
		{
			samplePosition.Value.Dispose();
		}
		_samplePositions.Clear();
	}
}
