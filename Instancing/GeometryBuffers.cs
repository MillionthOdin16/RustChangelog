using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

namespace Instancing;

public class GeometryBuffers
{
	[StructLayout(LayoutKind.Explicit, Size = 96)]
	public struct VertexData
	{
		[FieldOffset(0)]
		public float4 Position;

		[FieldOffset(16)]
		public float4 UV01;

		[FieldOffset(32)]
		public float4 UV23;

		[FieldOffset(48)]
		public float4 Normal;

		[FieldOffset(64)]
		public float4 Tangent;

		[FieldOffset(80)]
		public float4 Color;
	}

	private int _meshCopyMode;

	public GPUBuffer<VertexData> VertexBuffer;

	public GPUBuffer<int> TriangleBuffer;

	private int VertexIndex;

	private int TriangleIndex;

	private Dictionary<Mesh, MultidrawMeshInfo[]> _meshes = new Dictionary<Mesh, MultidrawMeshInfo[]>();

	public bool IsDirty { get; set; }

	public void Initialize(int meshCopyMode)
	{
		_meshCopyMode = meshCopyMode;
		AllocateNativeMemory();
		ResetStreamPosition();
	}

	private void ResetStreamPosition()
	{
		TriangleIndex = 0;
		VertexIndex = 0;
	}

	public void Destroy()
	{
		FreeNativeMemory();
		_meshes.Clear();
	}

	private void AllocateNativeMemory()
	{
		VertexBuffer = new GPUBuffer<VertexData>(800000, GPUBuffer.Target.Structured);
		TriangleBuffer = new GPUBuffer<int>(3000000, GPUBuffer.Target.Structured);
	}

	private void FreeNativeMemory()
	{
		VertexBuffer?.Dispose();
		VertexBuffer = null;
		TriangleBuffer?.Dispose();
		TriangleBuffer = null;
	}

	public MultidrawMeshInfo[] CopyMesh(Mesh mesh)
	{
		if (_meshes.TryGetValue(mesh, out var value))
		{
			return value;
		}
		value = CalculateSubmeshInfo(mesh);
		_meshes.Add(mesh, value);
		if (_meshCopyMode == 0)
		{
			CopyMeshViaCPU(mesh);
		}
		else
		{
			CopyMeshViaShader(mesh);
		}
		IsDirty = true;
		return value;
	}

	private void CopyMeshViaShader(Mesh mesh)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		mesh.vertexBufferTarget = (Target)(mesh.vertexBufferTarget | 0x20);
		mesh.indexBufferTarget = (Target)(mesh.indexBufferTarget | 0x20);
		int num = 0;
		GraphicsBuffer vertexBuffer = mesh.GetVertexBuffer(num);
		GraphicsBuffer indexBuffer = mesh.GetIndexBuffer();
		ComputeShader copyMeshShader = SingletonComponent<InstancedScheduler>.Instance.CopyMeshShader;
		int num2 = copyMeshShader.FindKernel("CopyMeshKernel");
		int vertexCount = mesh.vertexCount;
		int num3 = 0;
		for (int i = 0; i < mesh.subMeshCount; i++)
		{
			num3 += (int)mesh.GetIndexCount(i);
		}
		copyMeshShader.SetInt("_Offset_Vertex", mesh.GetVertexAttributeOffset((VertexAttribute)0));
		copyMeshShader.SetInt("_Offset_UV0", mesh.GetVertexAttributeOffset((VertexAttribute)4));
		copyMeshShader.SetInt("_Offset_UV1", mesh.GetVertexAttributeOffset((VertexAttribute)5));
		copyMeshShader.SetInt("_Offset_UV2", mesh.GetVertexAttributeOffset((VertexAttribute)6));
		copyMeshShader.SetInt("_Offset_UV3", mesh.GetVertexAttributeOffset((VertexAttribute)7));
		copyMeshShader.SetInt("_Offset_Normal", mesh.GetVertexAttributeOffset((VertexAttribute)1));
		copyMeshShader.SetInt("_Offset_Tangent", mesh.GetVertexAttributeOffset((VertexAttribute)2));
		copyMeshShader.SetInt("_Offset_Color", mesh.GetVertexAttributeOffset((VertexAttribute)3));
		copyMeshShader.SetBuffer(num2, "_Verts", mesh.GetVertexBuffer(num));
		copyMeshShader.SetBuffer(num2, "_Triangles", mesh.GetIndexBuffer());
		copyMeshShader.SetInt("_TriangleCount", num3);
		copyMeshShader.SetInt("_VertexCount", vertexCount);
		copyMeshShader.SetInt("_VertexStride", mesh.GetVertexBufferStride(0));
		copyMeshShader.SetInt("_TriangleStride", ((int)mesh.indexFormat == 0) ? 2 : 4);
		copyMeshShader.SetInt("_OutputVertexIndex", VertexIndex);
		copyMeshShader.SetInt("_OutputTriangleIndex", TriangleIndex);
		copyMeshShader.SetBuffer(num2, "_Output", VertexBuffer.Buffer);
		copyMeshShader.SetBuffer(num2, "_OutputTriangles", TriangleBuffer.Buffer);
		VertexIndex += vertexCount;
		TriangleIndex += num3;
		if (VertexBuffer.count < VertexIndex + 1 || TriangleBuffer.count < TriangleIndex + 1)
		{
			Debug.Log((object)"Resizing multidraw geometry buffer");
			VertexBuffer.EnsureCapacity(VertexIndex + 1, preserveData: true);
			TriangleBuffer.EnsureCapacity(TriangleIndex + 1, preserveData: true);
			IsDirty = true;
		}
		int iterationCount = InstancingUtil.GetIterationCount(Mathf.Max(num3, vertexCount), 1024);
		copyMeshShader.Dispatch(num2, iterationCount, 1, 1);
		vertexBuffer.Dispose();
		indexBuffer.Dispose();
	}

	public void Rebuild()
	{
		ResetStreamPosition();
		foreach (Mesh key in _meshes.Keys)
		{
			CopyMeshViaShader(key);
		}
	}

	private MultidrawMeshInfo[] CalculateSubmeshInfo(Mesh mesh)
	{
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		MultidrawMeshInfo[] array = new MultidrawMeshInfo[mesh.subMeshCount];
		for (int i = 0; i < mesh.subMeshCount; i++)
		{
			SubMeshDescriptor subMesh = mesh.GetSubMesh(i);
			MultidrawMeshInfo multidrawMeshInfo = default(MultidrawMeshInfo);
			multidrawMeshInfo.IndexStart = TriangleIndex + ((SubMeshDescriptor)(ref subMesh)).indexStart;
			multidrawMeshInfo.VertexStart = VertexIndex + ((SubMeshDescriptor)(ref subMesh)).baseVertex;
			multidrawMeshInfo.VertexCount = ((SubMeshDescriptor)(ref subMesh)).vertexCount;
			MultidrawMeshInfo multidrawMeshInfo2 = multidrawMeshInfo;
			array[i] = multidrawMeshInfo2;
		}
		return array;
	}

	private void CopyMeshViaCPU(Mesh mesh)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0255: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_019d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		MeshCache.Data data = MeshCache.Get(mesh);
		NativeArray<VertexData> data2 = default(NativeArray<VertexData>);
		data2._002Ector(data.vertices.Length, (Allocator)2, (NativeArrayOptions)1);
		for (int i = 0; i < data2.Length; i++)
		{
			VertexData vertexData = default(VertexData);
			Vector3 val = data.vertices[i];
			Vector2 val2 = (mesh.HasVertexAttribute((VertexAttribute)4) ? data.uv[i] : Vector2.zero);
			Vector2 val3 = (mesh.HasVertexAttribute((VertexAttribute)5) ? data.uv2[i] : Vector2.zero);
			Vector2 val4 = (mesh.HasVertexAttribute((VertexAttribute)6) ? data.uv3[i] : Vector2.zero);
			Vector2 val5 = (mesh.HasVertexAttribute((VertexAttribute)7) ? data.uv4[i] : Vector2.zero);
			Vector3 val6 = (mesh.HasVertexAttribute((VertexAttribute)1) ? data.normals[i] : Vector3.zero);
			Vector4 val7 = (mesh.HasVertexAttribute((VertexAttribute)2) ? data.tangents[i] : Vector4.zero);
			Color32 val8 = (Color32)(mesh.HasVertexAttribute((VertexAttribute)3) ? data.colors32[i] : new Color32((byte)0, (byte)0, (byte)0, (byte)0));
			vertexData.Position = new float4(val.x, val.y, val.z, 1f);
			vertexData.UV01 = new float4(val2.x, val2.y, val3.x, val3.y);
			vertexData.UV23 = new float4(val4.x, val4.y, val5.x, val5.y);
			vertexData.Normal = new float4(val6.x, val6.y, val6.z, 1f);
			vertexData.Tangent = new float4(val7.x, val7.y, val7.z, val7.w);
			vertexData.Color = new float4((float)(int)val8.r / 255f, (float)(int)val8.g / 255f, (float)(int)val8.b / 255f, (float)(int)val8.a / 255f);
			data2[i] = vertexData;
		}
		VertexBuffer.EnsureCapacity(VertexIndex + data2.Length + 1);
		VertexBuffer.SetData(data2, 0, VertexIndex, data2.Length);
		VertexIndex += data2.Length;
		int[] triangles = data.triangles;
		TriangleBuffer.EnsureCapacity(TriangleIndex + triangles.Length + 1);
		TriangleBuffer.SetData(triangles, 0, TriangleIndex, triangles.Length);
		TriangleIndex += triangles.Length;
		data2.Dispose();
		IsDirty = true;
	}

	public void PrintMemoryUsage(StringBuilder builder)
	{
		builder.AppendLine($"Vertex Buffer: {VertexIndex} / {VertexBuffer.count}");
		builder.AppendLine($"Triangle Buffer: {TriangleIndex} / {TriangleBuffer.count}");
		builder.AppendLine($"Meshes: {_meshes.Count}");
	}
}
