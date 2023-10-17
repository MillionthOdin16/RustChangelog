using System.Collections.Generic;

public static class CollectionEx
{
	public static bool IsNullOrEmpty<T>(this ICollection<T> collection)
	{
		return collection == null || collection.Count == 0;
	}

	public static bool IsEmpty<T>(this ICollection<T> collection)
	{
		return collection.Count == 0;
	}
}
