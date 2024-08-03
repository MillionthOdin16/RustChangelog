using System.Collections.Generic;

[Il2CppEagerStaticClassConstruction]
public abstract class HiddenValueBase
{
	private static ulong _nextKey;

	private static readonly Dictionary<ulong, object> _values = new Dictionary<ulong, object>();

	protected static ulong TakeKey()
	{
		lock (_values)
		{
			return ++_nextKey;
		}
	}

	protected static object Get(ulong key)
	{
		lock (_values)
		{
			return _values[key];
		}
	}

	protected static void Set(ulong key, object value)
	{
		lock (_values)
		{
			if (value == null)
			{
				_values.Remove(key);
			}
			else
			{
				_values[key] = value;
			}
		}
	}
}
