using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Collections;
using UnityEngine;

namespace Instancing;

public class GPUBuffer
{
	public enum Target
	{
		Structured,
		IndirectArgs,
		Vertex,
		Index,
		Raw
	}
}
public class GPUBuffer<T> : GPUBuffer, IDisposable where T : unmanaged
{
	private Target _type;

	public int BufferVersion { get; private set; }

	public GraphicsBuffer Buffer { get; private set; }

	public Target Type { get; private set; }

	public int count { get; private set; }

	public int stride { get; private set; }

	public int ByteLength => count * stride;

	public GPUBuffer(int length, Target target)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Expected O, but got Unknown
		count = length;
		stride = Marshal.SizeOf<T>();
		Type = target;
		switch (target)
		{
		case Target.Structured:
			_type = (Target)16;
			break;
		case Target.IndirectArgs:
			_type = (Target)256;
			break;
		case Target.Vertex:
			_type = (Target)1;
			break;
		case Target.Index:
			_type = (Target)2;
			break;
		case Target.Raw:
			_type = (Target)32;
			break;
		default:
			throw new NotImplementedException($"GPUBuffer Target '{target}'");
		}
		Buffer = new GraphicsBuffer(_type, length, stride);
		ClearData();
	}

	public void SetData(List<T> data)
	{
		Buffer.SetData<T>(data);
	}

	public void SetData(List<int> data, int nativeArrayIndex, int computeBufferIndex, int length)
	{
		Buffer.SetData<int>(data, nativeArrayIndex, computeBufferIndex, length);
	}

	public void SetData(T[] data)
	{
		Buffer.SetData((Array)data);
	}

	public void SetData(T[] data, int nativeArrayIndex, int computeBufferIndex, int length)
	{
		Buffer.SetData((Array)data, nativeArrayIndex, computeBufferIndex, length);
	}

	public void SetData(NativeArray<T> data)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		Buffer.SetData<T>(data);
	}

	public void SetData(NativeArray<T> data, int nativeArrayIndex, int computeBufferIndex, int length)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		Buffer.SetData<T>(data, nativeArrayIndex, computeBufferIndex, length);
	}

	public void ClearData()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		NativeArray<T> data = default(NativeArray<T>);
		data._002Ector(count, (Allocator)2, (NativeArrayOptions)1);
		try
		{
			Buffer.SetData<T>(data);
		}
		finally
		{
			((IDisposable)data).Dispose();
		}
	}

	public void Expand(int newCapacity, bool preserveData = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Expected O, but got Unknown
		GraphicsBuffer val = new GraphicsBuffer(_type, newCapacity, stride);
		BufferVersion++;
		if (preserveData)
		{
			T[] array = new T[newCapacity];
			Buffer.GetData((Array)array, 0, 0, count);
			val.SetData((Array)array);
		}
		Dispose();
		Buffer = val;
		count = newCapacity;
		if (!preserveData)
		{
			ClearData();
		}
	}

	public void EnsureCapacity(int size, bool preserveData = false, float expandRatio = 2f)
	{
		if (Buffer.count < size)
		{
			int newCapacity = (int)((float)size * expandRatio);
			Expand(newCapacity, preserveData);
		}
	}

	public void Dispose()
	{
		GraphicsBuffer buffer = Buffer;
		if (buffer != null)
		{
			buffer.Dispose();
		}
		Buffer = null;
	}
}
