using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConVar;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace Instancing;

public class DrawCallManager
{
	private class DrawCall
	{
		public int DrawCallIndex;

		public int RendererId;

		public Mesh Mesh;

		public int SubmeshIndex;

		public Material Material;

		public Material MultidrawMaterial;

		public ShadowCastingMode ShadowMode;

		public bool ReceiveShadows;

		public LightProbeUsage LightProbes;

		public InstancedMeshCategory MeshCategory;

		public int MultiDrawExtraCount;

		private DrawCallKey _key;

		public DrawCallJobData JobData;

		public MaterialPropertyBlock MaterialBlock = new MaterialPropertyBlock();

		public DrawCallKey CalculateKey()
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			if (_key == default(DrawCallKey))
			{
				_key = new DrawCallKey(Material, ShadowMode, ReceiveShadows, LightProbes);
			}
			return _key;
		}
	}

	private readonly CellMeshAllocator cellAllocator;

	private readonly GeometryBuffers GeometryBuffers;

	public NativeArray<DrawCallJobData> DrawCallArray;

	public GPUBuffer<DrawCallJobData> DrawCallBuffer;

	public GPUBuffer<uint> IndirectArgsBuffer;

	public GPUBuffer<uint> IndirectExtraArgBuffer;

	public GPUBuffer<uint> RenderBuffer;

	private int _overrideBufferVersion;

	private int _positionBufferVersion;

	private List<DrawCall> DrawCalls = new List<DrawCall>();

	private bool _needsDrawCallRebuild;

	private int IndirectArgCapacity;

	private static readonly int renderLayer = LayerMask.NameToLayer("Construction");

	private const int initialCapacity = 1024;

	private Bounds cullingBounds = new Bounds(Vector3.zero, Vector3.one * 30000f);

	public int DrawCallsLastFrame { get; private set; }

	public int DrawCallCount => DrawCalls.Count;

	public DrawCallManager(CellMeshAllocator cellAllocator, GeometryBuffers geometryBuffers)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		this.cellAllocator = cellAllocator;
		GeometryBuffers = geometryBuffers;
	}

	public void Initialize()
	{
		DrawCalls = new List<DrawCall>();
		AllocateNativeMemory();
	}

	public void OnDestroy()
	{
		FreeNativeMemory();
	}

	private void AllocateNativeMemory()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		IndirectArgCapacity = 1024;
		DrawCallArray = new NativeArray<DrawCallJobData>(IndirectArgCapacity, (Allocator)4, (NativeArrayOptions)1);
		DrawCallBuffer = new GPUBuffer<DrawCallJobData>(IndirectArgCapacity, GPUBuffer.Target.Structured);
		IndirectArgsBuffer = new GPUBuffer<uint>(IndirectArgCapacity * 5, GPUBuffer.Target.IndirectArgs);
		IndirectExtraArgBuffer = new GPUBuffer<uint>(IndirectArgCapacity, GPUBuffer.Target.Structured);
		RenderBuffer = new GPUBuffer<uint>(32000, GPUBuffer.Target.Structured);
	}

	private void FreeNativeMemory()
	{
		NativeArrayEx.SafeDispose(ref DrawCallArray);
		DrawCallBuffer?.Dispose();
		DrawCallBuffer = null;
		IndirectArgsBuffer?.Dispose();
		IndirectArgsBuffer = null;
		IndirectExtraArgBuffer?.Dispose();
		IndirectExtraArgBuffer = null;
		RenderBuffer?.Dispose();
		RenderBuffer = null;
	}

	public void AddDrawCall(InstancedMeshRenderer renderer, int submeshIndex, uint indicies, uint indiciesIndex, uint vertexIndex, MultidrawMeshInfo multidraw)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		int count = DrawCalls.Count;
		int num = Math.Min(renderer.Materials.Length - 1, submeshIndex);
		DrawCall drawCall = new DrawCall
		{
			DrawCallIndex = count,
			LightProbes = renderer.LightProbes,
			Material = renderer.Materials[num],
			MultidrawMaterial = renderer.MultidrawMaterials[num],
			Mesh = renderer.Mesh,
			ReceiveShadows = renderer.RecieveShadows,
			RendererId = renderer.RendererId,
			ShadowMode = renderer.CastShadows,
			MeshCategory = renderer.MeshCategory,
			SubmeshIndex = submeshIndex
		};
		drawCall.JobData = new DrawCallJobData
		{
			IndexCount = indicies,
			IndexStart = indiciesIndex,
			VertexStart = vertexIndex,
			MultidrawIndexStart = (uint)multidraw.IndexStart,
			MultidrawVertexStart = (uint)multidraw.VertexStart,
			DrawCallIndex = count,
			RendererIndex = renderer.RendererId
		};
		DrawCalls.Add(drawCall);
		_needsDrawCallRebuild = true;
		EnsureDrawCallCapcity();
		DrawCallArray[count] = drawCall.JobData;
		if (!NeedsToRebuildMaterialBlocks())
		{
			RebuildAllMaterialBlocks();
		}
		else
		{
			UpdateMaterialBlock(drawCall);
		}
	}

	private bool NeedsToRebuildMaterialBlocks()
	{
		if (cellAllocator.PositionBuffer.BufferVersion == _positionBufferVersion)
		{
			return cellAllocator.OverrideBuffer.BufferVersion != _overrideBufferVersion;
		}
		return true;
	}

	private void RebuildAllMaterialBlocks()
	{
		_overrideBufferVersion = cellAllocator.OverrideBuffer.BufferVersion;
		_positionBufferVersion = cellAllocator.PositionBuffer.BufferVersion;
		for (int i = 0; i < DrawCalls.Count; i++)
		{
			UpdateMaterialBlock(DrawCalls[i]);
		}
	}

	private void UpdateMaterialBlock(DrawCall drawCall)
	{
		MaterialPropertyBlock materialBlock = drawCall.MaterialBlock;
		materialBlock.SetBuffer(InstancingUtil.PositionBufferProperty, cellAllocator.PositionBuffer.Buffer);
		materialBlock.SetBuffer(InstancingUtil.RenderBufferProperty, RenderBuffer.Buffer);
		materialBlock.SetBuffer(InstancingUtil.IndirectExtraArgProperty, IndirectExtraArgBuffer.Buffer);
		materialBlock.SetBuffer(InstancingUtil.Param_MeshOverrideBuffer, cellAllocator.OverrideBuffer.Buffer);
		materialBlock.SetBuffer(InstancingUtil.Param_Verticies, GeometryBuffers.VertexBuffer.Buffer);
		materialBlock.SetBuffer(InstancingUtil.Param_Triangles, GeometryBuffers.TriangleBuffer.Buffer);
		materialBlock.SetInt(InstancingUtil.DrawCallIndexProperty, drawCall.DrawCallIndex);
	}

	public void EnsureCapacity(int totalMeshCount)
	{
		bool flag = false;
		if (totalMeshCount > RenderBuffer.count)
		{
			int newCapacity = Mathf.ClosestPowerOfTwo(totalMeshCount) * 2;
			RenderBuffer.Expand(newCapacity);
			flag = true;
		}
		if (NeedsToRebuildMaterialBlocks())
		{
			flag = true;
		}
		if (GeometryBuffers.IsDirty)
		{
			flag = true;
			GeometryBuffers.IsDirty = false;
		}
		if (_needsDrawCallRebuild)
		{
			SortAndBatchDrawCalls();
			_needsDrawCallRebuild = false;
		}
		if (flag)
		{
			RebuildAllMaterialBlocks();
		}
	}

	private void EnsureDrawCallCapcity()
	{
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		if (DrawCalls.Count > IndirectArgCapacity)
		{
			IndirectArgCapacity = Mathf.ClosestPowerOfTwo(DrawCalls.Count) * 2;
			NativeArrayEx.Expand(ref DrawCallArray, IndirectArgCapacity);
			DrawCallBuffer.Expand(IndirectArgCapacity);
			DrawCallBuffer.SetData(DrawCallArray);
			IndirectArgsBuffer.Expand(IndirectArgCapacity * 5);
			IndirectExtraArgBuffer.Expand(IndirectArgCapacity);
		}
	}

	public void UpdateComputeBuffers()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		DrawCallBuffer.SetData(DrawCallArray);
	}

	private void SortAndBatchDrawCalls()
	{
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		DrawCalls = DrawCalls.OrderBy((DrawCall x) => x.CalculateKey().GetHashCode()).ToList();
		foreach (DrawCall drawCall3 in DrawCalls)
		{
			drawCall3.MultiDrawExtraCount = 0;
		}
		int num;
		for (num = 0; num < DrawCalls.Count; num++)
		{
			DrawCall drawCall = DrawCalls[num];
			for (int i = num + 1; i < DrawCalls.Count && DrawCalls[i].CalculateKey() == drawCall.CalculateKey(); i++)
			{
				drawCall.MultiDrawExtraCount++;
			}
			num += drawCall.MultiDrawExtraCount;
		}
		for (int j = 0; j < DrawCalls.Count; j++)
		{
			DrawCall drawCall2 = DrawCalls[j];
			drawCall2.DrawCallIndex = j;
			drawCall2.JobData.DrawCallIndex = drawCall2.DrawCallIndex;
			DrawCallArray[j] = drawCall2.JobData;
		}
		DrawCallBuffer.SetData(DrawCallArray);
	}

	public void SubmitDrawCalls()
	{
		DrawCallsLastFrame = 0;
		SubmitDrawCallsInternal(MainCamera.mainCamera);
	}

	private void SubmitDrawCallsInternal(Camera camera)
	{
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Invalid comparison between Unknown and I4
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Invalid comparison between Unknown and I4
		DrawCallsLastFrame = 0;
		if (Render.IsMultidrawEnabled)
		{
			Shader.EnableKeyword(ref InstancingUtil.Keyword_Rust_Procedural_Rendering);
		}
		for (int i = 0; i < DrawCalls.Count; i++)
		{
			DrawCall drawCall = DrawCalls[i];
			switch (drawCall.MeshCategory)
			{
			case InstancedMeshCategory.BuildingBlock:
				if (!Render.instanced_toggle_buildings)
				{
					continue;
				}
				break;
			case InstancedMeshCategory.Cliff:
				if (!Render.instanced_toggle_cliffs)
				{
					continue;
				}
				break;
			case InstancedMeshCategory.Other:
				if (!Render.instanced_toggle_other)
				{
					continue;
				}
				break;
			}
			DrawCallsLastFrame++;
			if (Render.IsMultidrawEnabled)
			{
				RenderParams val = default(RenderParams);
				((RenderParams)(ref val)).camera = camera;
				((RenderParams)(ref val)).layer = renderLayer;
				((RenderParams)(ref val)).lightProbeUsage = drawCall.LightProbes;
				((RenderParams)(ref val)).material = drawCall.MultidrawMaterial;
				((RenderParams)(ref val)).worldBounds = cullingBounds;
				((RenderParams)(ref val)).shadowCastingMode = drawCall.ShadowMode;
				((RenderParams)(ref val)).receiveShadows = drawCall.ReceiveShadows;
				((RenderParams)(ref val)).matProps = drawCall.MaterialBlock;
				RenderParams val2 = val;
				if (((int)((RenderParams)(ref val2)).shadowCastingMode != 1 && (int)((RenderParams)(ref val2)).shadowCastingMode != 3) || Render.render_shadows)
				{
					Graphics.RenderPrimitivesIndexedIndirect(ref val2, (MeshTopology)0, GeometryBuffers.TriangleBuffer.Buffer, IndirectArgsBuffer.Buffer, 1 + drawCall.MultiDrawExtraCount, i);
				}
				i += drawCall.MultiDrawExtraCount;
			}
			else
			{
				int num = drawCall.DrawCallIndex * 5 * 4;
				Graphics.DrawMeshInstancedIndirect(drawCall.Mesh, drawCall.SubmeshIndex, drawCall.Material, cullingBounds, IndirectArgsBuffer.Buffer, num, drawCall.MaterialBlock, drawCall.ShadowMode, drawCall.ReceiveShadows, renderLayer, camera, drawCall.LightProbes);
			}
		}
		Shader.DisableKeyword(ref InstancingUtil.Keyword_Rust_Procedural_Rendering);
	}

	public void PrintMemoryUsage(StringBuilder builder)
	{
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		builder.AppendLine("### DrawCallManager ###");
		builder.MemoryUsage("RenderBuffer", RenderBuffer.Buffer);
		builder.MemoryUsage("IndirectArgsBuffer", IndirectArgsBuffer.Buffer);
		builder.MemoryUsage("IndirectExtraArgsBuffer", IndirectExtraArgBuffer.Buffer);
		builder.MemoryUsage<DrawCallJobData>("DrawCallArray", DrawCallArray, DrawCalls.Count);
		builder.MemoryUsage("DrawCalls", (ICollection<DrawCall>)DrawCalls);
	}
}
