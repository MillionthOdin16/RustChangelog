using System;
using System.Runtime.CompilerServices;
using Facepunch;

public sealed class HiddenValue<T> : HiddenValueBase, IPooled, IDisposable where T : class
{
	public struct EncryptedValue<TInner> where TInner : unmanaged
	{
		private TInner _value;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public TInner Get()
		{
			return _value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Set(TInner value)
		{
			_value = value;
		}

		public override string ToString()
		{
			return Get().ToString();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator EncryptedValue<TInner>(TInner value)
		{
			EncryptedValue<TInner> result = default(EncryptedValue<TInner>);
			result.Set(value);
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator TInner(EncryptedValue<TInner> encrypted)
		{
			return encrypted.Get();
		}
	}

	private EncryptedValue<ulong> _key;

	private int _accessCount;

	public HiddenValue()
		: this((T)null)
	{
	}

	public HiddenValue(T value)
	{
		_key.Set(0uL);
		_accessCount = 0;
		if (value != null)
		{
			Set(value);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public T Get()
	{
		ulong num = _key.Get();
		if (num == 0L)
		{
			return null;
		}
		T val = (T)HiddenValueBase.Get(num);
		_accessCount++;
		if (_accessCount >= 1000)
		{
			Set(val);
		}
		return val;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public HiddenValue<T> Set(T value)
	{
		ulong num = _key.Get();
		if (num != 0L)
		{
			HiddenValueBase.Set(num, null);
		}
		if (value == null)
		{
			_key.Set(0uL);
			return this;
		}
		ulong num2 = HiddenValueBase.TakeKey();
		HiddenValueBase.Set(num2, value);
		_key.Set(num2);
		return this;
	}

	void IPooled.EnterPool()
	{
		Set(null);
	}

	void IPooled.LeavePool()
	{
		_key.Set(0uL);
	}

	public void Dispose()
	{
		Set(null);
		HiddenValue<T> hiddenValue = this;
		Pool.Free<HiddenValue<T>>(ref hiddenValue);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator T(HiddenValue<T> hidden)
	{
		return hidden.Get();
	}
}
