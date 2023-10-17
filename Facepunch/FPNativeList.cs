using Unity.Collections;
using UnityEngine;

namespace Facepunch;

public class FPNativeList<T> : IPooled where T : struct
{
	private NativeArray<T> _array;

	private int _length;

	private int _capacity;

	public NativeArray<T> Array => _array;

	public int Count => _length;

	public int Capacity => _capacity;

	public T this[int index]
	{
		get
		{
			return _array[index];
		}
		set
		{
			_array[index] = value;
		}
	}

	public void Add(T item)
	{
		EnsureCapacity(_length + 1);
		_array[_length++] = item;
	}

	public void RemoveLast()
	{
		_length--;
		_array[_length] = default(T);
	}

	public void SetLength(int newLength)
	{
		if (newLength > _length)
		{
			EnsureCapacity(newLength);
		}
		else
		{
			_length = newLength;
		}
	}

	public void Clear()
	{
		for (int i = 0; i < _array.Length; i++)
		{
			_array[i] = default(T);
		}
		_length = 0;
	}

	public void Resize(int count)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		if (_array.IsCreated)
		{
			_array.Dispose();
		}
		_array = new NativeArray<T>(count, (Allocator)4, (NativeArrayOptions)1);
		_length = count;
	}

	public void EnsureCapacity(int requiredCapacity)
	{
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		if (!_array.IsCreated || _array.Length < requiredCapacity)
		{
			_capacity = Mathf.Max(_array.Length * 2, requiredCapacity);
			NativeArray<T> array = default(NativeArray<T>);
			array._002Ector(_capacity, (Allocator)4, (NativeArrayOptions)1);
			if (_array.IsCreated)
			{
				_array.CopyTo(array.GetSubArray(0, _array.Length));
				_array.Dispose();
			}
			_array = array;
		}
	}

	public void EnterPool()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		if (_array.IsCreated)
		{
			_array.Dispose();
		}
		_array = default(NativeArray<T>);
		_length = 0;
		_capacity = 0;
	}

	public void LeavePool()
	{
	}
}
