using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace Instancing;

public class MeshGridManager
{
	private struct MeshGridKey
	{
		public int SpatialId;

		public bool IsShadow;
	}

	private class GridAllocationInfo
	{
		public List<int> Cells = new List<int>();
	}

	public NativeArray<GridJobData> Grids;

	private float HalfWorldSize;

	private float GridSize;

	private const int GridCount = 32;

	private const int normalGridCount = 1024;

	private const int shadowGridCount = 1024;

	private const int outOfBoundsGrid = 2048;

	private const int lastGridId = 2048;

	public void Initialize()
	{
		AllocateNativeMemory();
	}

	public void OnDestroy()
	{
		FreeNativeMemory();
	}

	private void AllocateNativeMemory()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		Grids = new NativeArray<GridJobData>(2049, (Allocator)4, (NativeArrayOptions)1);
	}

	private void FreeNativeMemory()
	{
		NativeArrayEx.SafeDispose(ref Grids);
	}

	public int GetPartitionKey(float3 position, bool hasShadow)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		int num = GetGridId(position);
		if (hasShadow && num != 2048)
		{
			num += 1024;
		}
		return num;
	}

	public void SetWorldSize(float worldSize)
	{
		GridSize = worldSize / 32f;
		HalfWorldSize = worldSize / 2f;
		UpdateGridBounds();
	}

	private void UpdateGridBounds()
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < Grids.Length; i++)
		{
			GridJobData gridJobData = Grids[i];
			gridJobData.GridId = i;
			if (i < 1024)
			{
				gridJobData.CanBeFrustumCulled = true;
			}
			if (i < 2048)
			{
				Bounds gridBounds = GetGridBounds(i);
				gridJobData.CanBeDistanceCulled = true;
				gridJobData.MinBounds = float3.op_Implicit(((Bounds)(ref gridBounds)).min);
				gridJobData.MaxBounds = float3.op_Implicit(((Bounds)(ref gridBounds)).max);
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private int GetGridId(float3 point)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		int num = (int)((point.x + HalfWorldSize) / GridSize);
		int num2 = (int)((point.z + HalfWorldSize) / GridSize);
		if (num < 0 || num2 < 0 || num >= 32 || num2 >= 32)
		{
			return 2048;
		}
		return num + num2 * 32;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private float3 GetGridCenter(int gridId)
	{
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		if (gridId >= 1024 && gridId < 2048)
		{
			gridId -= 1024;
		}
		float num = (float)(gridId % 32) * GridSize - HalfWorldSize;
		float num2 = (float)(gridId / 32) * GridSize - HalfWorldSize;
		return new float3(num + GridSize / 2f, 0f, num2 + GridSize / 2f);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private Bounds GetGridBounds(int gridId)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		float3 gridCenter = GetGridCenter(gridId);
		return new Bounds(float3.op_Implicit(gridCenter), new Vector3(GridSize, 1000f, GridSize));
	}

	public void PrintMemoryUsage(StringBuilder builder)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		builder.AppendLine("### GridManager ###");
		builder.MemoryUsage<GridJobData>("Grids", Grids, Grids.Length);
	}
}
