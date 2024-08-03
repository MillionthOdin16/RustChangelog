using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Facepunch;

internal class EntityRealm
{
	public sealed class HiddenValue<T> : IPooled, IDisposable where T : class
	{
		private HiddenValue_Internal_EncryptedValue<GCHandle> _handle;

		private int _accessCount;

		public HiddenValue()
			: this((T)null)
		{
		}

		public HiddenValue(T value)
		{
			_handle.Set(default(GCHandle));
			_accessCount = 0;
			if (value != null)
			{
				Set(value);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public T Get()
		{
			GCHandle gCHandle = _handle.Get();
			if (!gCHandle.IsAllocated)
			{
				return null;
			}
			T val = (T)gCHandle.Target;
			_accessCount++;
			if (_accessCount >= 1000)
			{
				_accessCount = 0;
				GCHandle value = GCHandle.Alloc(val, GCHandleType.Normal);
				gCHandle.Free();
				_handle.Set(value);
			}
			return val;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public HiddenValue<T> Set(T value)
		{
			GCHandle gCHandle = _handle.Get();
			if (value == null)
			{
				if (gCHandle.IsAllocated)
				{
					gCHandle.Free();
				}
				_handle.Set(default(GCHandle));
				return this;
			}
			GCHandle value2 = GCHandle.Alloc(value, GCHandleType.Normal);
			if (gCHandle.IsAllocated)
			{
				gCHandle.Free();
			}
			_handle.Set(value2);
			return this;
		}

		void IPooled.EnterPool()
		{
			Set(null);
		}

		void IPooled.LeavePool()
		{
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

	public struct HiddenValue_Internal_EncryptedValue<TInner> where TInner : unmanaged
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
		public static implicit operator HiddenValue_Internal_EncryptedValue<TInner>(TInner value)
		{
			HiddenValue_Internal_EncryptedValue<TInner> result = default(HiddenValue_Internal_EncryptedValue<TInner>);
			result.Set(value);
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator TInner(HiddenValue_Internal_EncryptedValue<TInner> encrypted)
		{
			return encrypted.Get();
		}
	}
}
