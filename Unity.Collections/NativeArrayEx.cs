namespace Unity.Collections;

public static class NativeArrayEx
{
	public static void Add<T>(this ref NativeArray<T> array, T item, ref int size) where T : unmanaged
	{
		if (size >= array.Length)
		{
			Expand(ref array, array.Length * 2);
		}
		array[size] = item;
		size++;
	}

	public static void RemoveUnordered<T>(this ref NativeArray<T> array, int index, ref int count) where T : unmanaged
	{
		int num = count - 1;
		if (index != num)
		{
			array[index] = array[num];
		}
		count--;
	}

	public static void Expand<T>(this ref NativeArray<T> array, int newCapacity) where T : unmanaged
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		if (newCapacity > array.Length)
		{
			NativeArray<T> val = default(NativeArray<T>);
			val._002Ector(newCapacity, (Allocator)4, (NativeArrayOptions)1);
			if (array.IsCreated)
			{
				array.CopyTo(val.GetSubArray(0, array.Length));
				array.Dispose();
			}
			array = val;
		}
	}

	public static void SafeDispose<T>(this ref NativeArray<T> array) where T : unmanaged
	{
		if (array.IsCreated)
		{
			array.Dispose();
		}
	}
}
