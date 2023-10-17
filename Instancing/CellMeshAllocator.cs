using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConVar;
using Facepunch;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace Instancing;

public class CellMeshAllocator
{
	private class CellPartition : IPooled
	{
		public List<CellId> PackedCells;

		public List<CellId> CellsWithSpace;

		public bool IsEmpty()
		{
			if (PackedCells.IsNullOrEmpty())
			{
				return CellsWithSpace.IsNullOrEmpty();
			}
			return false;
		}

		public void AddPackedCell(CellId cell)
		{
			if (PackedCells == null)
			{
				PackedCells = Pool.GetList<CellId>();
			}
			PackedCells.Add(cell);
		}

		public void RemovePackedCell(CellId cell)
		{
			if (PackedCells != null)
			{
				PackedCells.Remove(cell);
				if (PackedCells.Count == 0)
				{
					Pool.FreeList<CellId>(ref PackedCells);
				}
			}
		}

		public void AddCellWithSpace(CellId cell)
		{
			if (CellsWithSpace == null)
			{
				CellsWithSpace = Pool.GetList<CellId>();
			}
			CellsWithSpace.Add(cell);
		}

		public void RemoveCellWithSpace(CellId cell)
		{
			if (CellsWithSpace != null)
			{
				CellsWithSpace.Remove(cell);
				if (CellsWithSpace.Count == 0)
				{
					Pool.FreeList<CellId>(ref CellsWithSpace);
				}
			}
		}

		public void EnterPool()
		{
			if (PackedCells != null)
			{
				Pool.FreeList<CellId>(ref PackedCells);
			}
			if (CellsWithSpace != null)
			{
				Pool.FreeList<CellId>(ref CellsWithSpace);
			}
		}

		public void LeavePool()
		{
		}
	}

	public const int CellCapacity = 32;

	private const int initialCellCount = 8192;

	public const int InitialCapacity = 262144;

	private Dictionary<int, CellPartition> partitions = new Dictionary<int, CellPartition>();

	private List<CellId> recycledCells = new List<CellId>();

	private Dictionary<long, int> meshLookup = new Dictionary<long, int>();

	public Dictionary<long, int> sliceIndexLookup = new Dictionary<long, int>();

	private Dictionary<int, List<long>> sliceLists = new Dictionary<int, List<long>>();

	public NativeArray<CellHeader> Cells;

	public NativeArray<InstancedCullData> CullData;

	public NativeArray<float4x4> PositionData;

	public NativeArray<MeshOverrideData> OverrideArray;

	public GPUBuffer<float4x4> PositionBuffer;

	public GPUBuffer<InstancedCullData> CullingDataBuffer;

	public GPUBuffer<MeshOverrideData> OverrideBuffer;

	private bool dirty;

	public int CellCount { get; private set; }

	public void Initialize()
	{
		AllocateNativeMemory();
		meshLookup = new Dictionary<long, int>();
		sliceIndexLookup = new Dictionary<long, int>();
		sliceLists = new Dictionary<int, List<long>>();
		partitions = new Dictionary<int, CellPartition>();
		recycledCells = new List<CellId>();
		CellCount = 0;
	}

	public void OnDestroy()
	{
		FreeNativeMemory();
		dirty = false;
	}

	private void AllocateNativeMemory()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		Cells = new NativeArray<CellHeader>(8192, (Allocator)4, (NativeArrayOptions)1);
		int num = Cells.Length * 32;
		CullData = new NativeArray<InstancedCullData>(num, (Allocator)4, (NativeArrayOptions)1);
		PositionData = new NativeArray<float4x4>(num, (Allocator)4, (NativeArrayOptions)1);
		OverrideArray = new NativeArray<MeshOverrideData>(num, (Allocator)4, (NativeArrayOptions)1);
		PositionBuffer = new GPUBuffer<float4x4>(num, GPUBuffer.Target.Structured);
		CullingDataBuffer = new GPUBuffer<InstancedCullData>(num, GPUBuffer.Target.Structured);
		OverrideBuffer = new GPUBuffer<MeshOverrideData>(num, GPUBuffer.Target.Structured);
	}

	private void FreeNativeMemory()
	{
		NativeArrayEx.SafeDispose(ref Cells);
		NativeArrayEx.SafeDispose(ref CullData);
		PositionData.SafeDispose<float4x4>();
		PositionBuffer?.Dispose();
		PositionBuffer = null;
		CullingDataBuffer?.Dispose();
		CullingDataBuffer = null;
	}

	public CellId AddMesh(InstancedCullData data, int partitionKey, float4x4 localToWorld)
	{
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		if (!partitions.TryGetValue(partitionKey, out var value))
		{
			value = Pool.Get<CellPartition>();
			partitions[partitionKey] = value;
		}
		if (value.CellsWithSpace.IsNullOrEmpty())
		{
			value.AddCellWithSpace(CreateCell(partitionKey));
		}
		CellId cellId = value.CellsWithSpace[value.CellsWithSpace.Count - 1];
		CellHeader cellHeader = Cells[cellId.Index];
		int num = cellHeader.StartIndex + cellHeader.Count;
		cellHeader.Count++;
		int count = cellHeader.Count;
		Cells[cellId.Index] = cellHeader;
		if (!sliceLists.TryGetValue(data.RendererId, out var value2))
		{
			value2 = new List<long>();
			sliceLists[data.RendererId] = value2;
		}
		data.SliceIndex = value2.Count;
		value2.Add(data.VirtualMeshId);
		CullData[num] = data;
		PositionData[num] = localToWorld;
		OverrideArray[num] = default(MeshOverrideData);
		if (Render.computebuffer_setdata_immediate)
		{
			CullingDataBuffer.SetData(CullData, num, num, 1);
			PositionBuffer.SetData(PositionData, num, num, 1);
			OverrideBuffer.SetData(OverrideArray, num, num, 1);
		}
		else
		{
			dirty = true;
		}
		meshLookup.Add(data.VirtualMeshId, num);
		if (count == 32)
		{
			value.RemoveCellWithSpace(cellId);
			value.AddPackedCell(cellId);
		}
		else if (count > 32)
		{
			Debug.LogError((object)$"AddMesh() fucked up: >{32} elements in cell {cellId}");
		}
		return cellId;
	}

	public bool TryRemoveMesh(long virtualMeshId, out InstancedCullData removedData)
	{
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		if (!meshLookup.TryGetValue(virtualMeshId, out var value))
		{
			removedData = default(InstancedCullData);
			return false;
		}
		removedData = CullData[value];
		CellId cellId = GetCellId(value);
		CellHeader cellHeader = Cells[cellId.Index];
		int num = cellHeader.StartIndex + cellHeader.Count - 1;
		int count = cellHeader.Count;
		int partitionKey = cellHeader.PartitionKey;
		int num2 = --cellHeader.Count;
		Cells[cellId.Index] = cellHeader;
		if (value != num)
		{
			InstancedCullData instancedCullData = CullData[num];
			CullData[value] = instancedCullData;
			PositionData[value] = PositionData[num];
			OverrideArray[value] = OverrideArray[num];
			meshLookup[instancedCullData.VirtualMeshId] = value;
			if (Render.computebuffer_setdata_immediate)
			{
				CullingDataBuffer.SetData(CullData, value, value, 1);
				PositionBuffer.SetData(PositionData, value, value, 1);
				OverrideBuffer.SetData(OverrideArray, value, value, 1);
			}
			else
			{
				dirty = true;
			}
		}
		CullData[num] = default(InstancedCullData);
		if (Render.computebuffer_setdata_immediate)
		{
			CullingDataBuffer.SetData(CullData, num, num, 1);
		}
		else
		{
			dirty = true;
		}
		List<long> list = sliceLists[removedData.RendererId];
		long num3 = list[list.Count - 1];
		if (removedData.VirtualMeshId != num3)
		{
			int num4 = meshLookup[num3];
			InstancedCullData instancedCullData2 = CullData[num4];
			instancedCullData2.SliceIndex = removedData.SliceIndex;
			CullData[num4] = instancedCullData2;
			if (Render.computebuffer_setdata_immediate)
			{
				CullingDataBuffer.SetData(CullData, num4, num4, 1);
			}
			else
			{
				dirty = true;
			}
			list[removedData.SliceIndex] = num3;
		}
		list.RemoveAt(list.Count - 1);
		CellPartition cellPartition = partitions[partitionKey];
		if (count == 32)
		{
			cellPartition.RemovePackedCell(cellId);
			cellPartition.AddCellWithSpace(cellId);
		}
		else if (num2 == 0)
		{
			cellPartition.RemoveCellWithSpace(cellId);
			if (cellPartition.IsEmpty())
			{
				partitions.Remove(partitionKey);
				Pool.Free<CellPartition>(ref cellPartition);
			}
			RecycleCell(cellId);
		}
		meshLookup.Remove(virtualMeshId);
		return true;
	}

	public InstancedMeshData? TryGetMeshData(long virtualMeshId)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		if (!meshLookup.TryGetValue(virtualMeshId, out var value))
		{
			return null;
		}
		InstancedMeshData value2 = default(InstancedMeshData);
		value2.CullData = CullData[value];
		value2.LocalToWorld = PositionData[value];
		return value2;
	}

	public void SetMeshVisible(long virtualMeshId, bool visible)
	{
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		if (!meshLookup.TryGetValue(virtualMeshId, out var value))
		{
			Debug.LogError((object)$"Trying to remove mesh {virtualMeshId} that doesn't exist");
			return;
		}
		InstancedCullData instancedCullData = CullData[value];
		if (instancedCullData.IsVisible != visible)
		{
			instancedCullData.IsVisible = visible;
			CullData[value] = instancedCullData;
			if (Render.computebuffer_setdata_immediate)
			{
				CullingDataBuffer.SetData(CullData, value, value, 1);
			}
			else
			{
				dirty = true;
			}
		}
	}

	public void SetOverride(long virtualMeshId, MeshOverrideData newData)
	{
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		if (!meshLookup.TryGetValue(virtualMeshId, out var value))
		{
			Debug.LogError((object)$"Trying to set override of mesh {virtualMeshId} that doesn't exist");
		}
		else if (OverrideArray[value] != newData)
		{
			OverrideArray[value] = newData;
			if (Render.computebuffer_setdata_immediate)
			{
				OverrideBuffer.SetData(OverrideArray, value, value, 1);
			}
			else
			{
				dirty = true;
			}
		}
	}

	private CellId CreateCell(int sortingKey)
	{
		CellId result;
		if (recycledCells.Count > 0)
		{
			result = recycledCells[recycledCells.Count - 1];
			recycledCells.RemoveAt(recycledCells.Count - 1);
		}
		else
		{
			result = new CellId(CellCount);
			CellCount++;
		}
		if (Cells.Length <= CellCount)
		{
			ExpandData();
		}
		Cells[result.Index] = new CellHeader
		{
			Count = 0,
			PartitionKey = sortingKey,
			StartIndex = result.Index * 32
		};
		return result;
	}

	public void ExpandData()
	{
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		NativeArrayEx.Expand(ref Cells, Cells.Length * 2);
		int newCapacity = Cells.Length * 32;
		NativeArrayEx.Expand(ref CullData, newCapacity);
		PositionData.Expand<float4x4>(newCapacity);
		NativeArrayEx.Expand(ref OverrideArray, newCapacity);
		CullingDataBuffer.Expand(newCapacity);
		CullingDataBuffer.SetData(CullData);
		PositionBuffer.Expand(newCapacity);
		PositionBuffer.SetData(PositionData);
		OverrideBuffer.Expand(newCapacity);
		OverrideBuffer.SetData(OverrideArray);
	}

	private void RecycleCell(CellId cellId)
	{
		recycledCells.Add(cellId);
	}

	private CellId GetCellId(int index)
	{
		return new CellId(index / 32);
	}

	public void PrintMemoryUsage(StringBuilder builder)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		int num = ((IEnumerable<CellHeader>)(object)Cells).Take(CellCount).Sum((CellHeader x) => 32 - x.Count);
		int num2 = CellCount * 32;
		builder.AppendLine("### CellAllocator ###");
		builder.AppendLine($"Cells: {CellCount}");
		builder.AppendLine($"Empty Space In Cells: {num} / {num2} ({Math.Round((double)num / (double)num2, 1)}%)");
		builder.MemoryUsage<CellHeader>("Cell Headers", Cells);
		builder.MemoryUsage<InstancedCullData>("Data Array", CullData);
		builder.MemoryUsage("MeshLookup", (ICollection<KeyValuePair<long, int>>)meshLookup);
		builder.MemoryUsage("Recycled Cells", (ICollection<CellId>)recycledCells);
		builder.MemoryUsage("Partitions", (ICollection<KeyValuePair<int, CellPartition>>)partitions);
		builder.AppendLine("# Allocation Summary #");
		var array = (from x in ((IEnumerable<CellHeader>)(object)Cells).Take(CellCount)
			group x by x.Count into x
			select new
			{
				amountInCell = x.Key,
				count = x.Count()
			} into x
			orderby x.amountInCell
			select x).ToArray();
		foreach (var anon in array)
		{
			builder.AppendLine($"{anon.amountInCell}/{32} Cells: {anon.count}");
		}
	}

	public void FlushComputeBuffers()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		if (dirty)
		{
			dirty = false;
			CullingDataBuffer.SetData(CullData);
			PositionBuffer.SetData(PositionData);
			OverrideBuffer.SetData(OverrideArray);
		}
	}
}
