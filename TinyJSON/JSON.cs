using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace TinyJSON;

public static class JSON
{
	private static readonly Type includeAttrType = typeof(Include);

	private static readonly Type excludeAttrType = typeof(Exclude);

	private static readonly Type decodeAliasAttrType = typeof(DecodeAlias);

	private static readonly Dictionary<string, Type> typeCache = new Dictionary<string, Type>();

	private const BindingFlags instanceBindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

	private const BindingFlags staticBindingFlags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

	private static readonly MethodInfo decodeTypeMethod = typeof(JSON).GetMethod("DecodeType", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

	private static readonly MethodInfo decodeListMethod = typeof(JSON).GetMethod("DecodeList", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

	private static readonly MethodInfo decodeDictionaryMethod = typeof(JSON).GetMethod("DecodeDictionary", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

	private static readonly MethodInfo decodeArrayMethod = typeof(JSON).GetMethod("DecodeArray", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

	private static readonly MethodInfo decodeMultiRankArrayMethod = typeof(JSON).GetMethod("DecodeMultiRankArray", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

	public static Variant Load(string json)
	{
		if (json == null)
		{
			throw new ArgumentNullException("json");
		}
		return Decoder.Decode(json);
	}

	public static string Dump(object data)
	{
		return Dump(data, EncodeOptions.None);
	}

	public static string Dump(object data, EncodeOptions options)
	{
		if (data != null)
		{
			Type type = data.GetType();
			if (!type.IsEnum && !type.IsPrimitive && !type.IsArray)
			{
				MethodInfo[] methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				foreach (MethodInfo methodInfo in methods)
				{
					if (methodInfo.GetCustomAttributes(inherit: false).AnyOfType(typeof(BeforeEncode)) && methodInfo.GetParameters().Length == 0)
					{
						methodInfo.Invoke(data, null);
					}
				}
			}
		}
		return Encoder.Encode(data, options);
	}

	public static void MakeInto<T>(Variant data, out T item)
	{
		item = DecodeType<T>(data);
	}

	private static Type FindType(string fullName)
	{
		if (fullName == null)
		{
			return null;
		}
		if (typeCache.TryGetValue(fullName, out var value))
		{
			return value;
		}
		Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
		foreach (Assembly assembly in assemblies)
		{
			value = assembly.GetType(fullName);
			if (value != null)
			{
				typeCache.Add(fullName, value);
				return value;
			}
		}
		return null;
	}

	private static T DecodeType<T>(Variant data)
	{
		if (data == null)
		{
			return default(T);
		}
		Type type = typeof(T);
		if (type.IsEnum)
		{
			return (T)Enum.Parse(type, data.ToString(CultureInfo.InvariantCulture));
		}
		if (type.IsPrimitive || type == typeof(string) || type == typeof(decimal))
		{
			return (T)Convert.ChangeType(data, type);
		}
		if (type == typeof(Guid))
		{
			return (T)(object)new Guid(data.ToString(CultureInfo.InvariantCulture));
		}
		if (type.IsArray)
		{
			if (type.GetArrayRank() == 1)
			{
				MethodInfo methodInfo = decodeArrayMethod.MakeGenericMethod(type.GetElementType());
				return (T)methodInfo.Invoke(null, new object[1] { data });
			}
			if (!(data is ProxyArray proxyArray))
			{
				throw new DecodeException("Variant is expected to be a ProxyArray here, but it is not.");
			}
			int arrayRank = type.GetArrayRank();
			int[] array = new int[arrayRank];
			if (proxyArray.CanBeMultiRankArray(array))
			{
				Type elementType = type.GetElementType();
				if (elementType == null)
				{
					throw new DecodeException("Array element type is expected to be not null, but it is.");
				}
				Array array2 = Array.CreateInstance(elementType, array);
				MethodInfo methodInfo2 = decodeMultiRankArrayMethod.MakeGenericMethod(elementType);
				try
				{
					methodInfo2.Invoke(null, new object[4] { proxyArray, array2, 1, array });
				}
				catch (Exception innerException)
				{
					throw new DecodeException("Error decoding multidimensional array. Did you try to decode into an array of incompatible rank or element type?", innerException);
				}
				return (T)Convert.ChangeType(array2, typeof(T));
			}
			throw new DecodeException("Error decoding multidimensional array; JSON data doesn't seem fit this structure.");
		}
		if (typeof(IList).IsAssignableFrom(type))
		{
			MethodInfo methodInfo3 = decodeListMethod.MakeGenericMethod(type.GetGenericArguments());
			return (T)methodInfo3.Invoke(null, new object[1] { data });
		}
		if (typeof(IDictionary).IsAssignableFrom(type))
		{
			MethodInfo methodInfo4 = decodeDictionaryMethod.MakeGenericMethod(type.GetGenericArguments());
			return (T)methodInfo4.Invoke(null, new object[1] { data });
		}
		if (!(data is ProxyObject proxyObject))
		{
			throw new InvalidCastException("ProxyObject expected when decoding into '" + type.FullName + "'.");
		}
		string typeHint = proxyObject.TypeHint;
		T val;
		if (typeHint != null && typeHint != type.FullName)
		{
			Type type2 = FindType(typeHint);
			if (type2 == null)
			{
				throw new TypeLoadException("Could not load type '" + typeHint + "'.");
			}
			if (!type.IsAssignableFrom(type2))
			{
				throw new InvalidCastException("Cannot assign type '" + typeHint + "' to type '" + type.FullName + "'.");
			}
			val = (T)Activator.CreateInstance(type2);
			type = type2;
		}
		else
		{
			val = Activator.CreateInstance<T>();
		}
		foreach (KeyValuePair<string, Variant> item in (IEnumerable<KeyValuePair<string, Variant>>)(ProxyObject)data)
		{
			FieldInfo fieldInfo = type.GetField(item.Key, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (fieldInfo == null)
			{
				FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				FieldInfo[] array3 = fields;
				foreach (FieldInfo fieldInfo2 in array3)
				{
					object[] customAttributes = fieldInfo2.GetCustomAttributes(inherit: true);
					foreach (object obj in customAttributes)
					{
						if (decodeAliasAttrType.IsInstanceOfType(obj) && ((DecodeAlias)obj).Contains(item.Key))
						{
							fieldInfo = fieldInfo2;
							break;
						}
					}
				}
			}
			if (fieldInfo != null)
			{
				bool flag = fieldInfo.IsPublic;
				object[] customAttributes2 = fieldInfo.GetCustomAttributes(inherit: true);
				foreach (object o in customAttributes2)
				{
					if (excludeAttrType.IsInstanceOfType(o))
					{
						flag = false;
					}
					if (includeAttrType.IsInstanceOfType(o))
					{
						flag = true;
					}
				}
				if (flag)
				{
					MethodInfo methodInfo5 = decodeTypeMethod.MakeGenericMethod(fieldInfo.FieldType);
					if (type.IsValueType)
					{
						object obj2 = val;
						fieldInfo.SetValue(obj2, methodInfo5.Invoke(null, new object[1] { item.Value }));
						val = (T)obj2;
					}
					else
					{
						fieldInfo.SetValue(val, methodInfo5.Invoke(null, new object[1] { item.Value }));
					}
				}
			}
			PropertyInfo propertyInfo = type.GetProperty(item.Key, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (propertyInfo == null)
			{
				PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				PropertyInfo[] array4 = properties;
				foreach (PropertyInfo propertyInfo2 in array4)
				{
					object[] customAttributes3 = propertyInfo2.GetCustomAttributes(inherit: false);
					foreach (object obj3 in customAttributes3)
					{
						if (decodeAliasAttrType.IsInstanceOfType(obj3) && ((DecodeAlias)obj3).Contains(item.Key))
						{
							propertyInfo = propertyInfo2;
							break;
						}
					}
				}
			}
			if (propertyInfo != null && propertyInfo.CanWrite && propertyInfo.GetCustomAttributes(inherit: false).AnyOfType(includeAttrType))
			{
				MethodInfo methodInfo6 = decodeTypeMethod.MakeGenericMethod(propertyInfo.PropertyType);
				if (type.IsValueType)
				{
					object obj4 = val;
					propertyInfo.SetValue(obj4, methodInfo6.Invoke(null, new object[1] { item.Value }), null);
					val = (T)obj4;
				}
				else
				{
					propertyInfo.SetValue(val, methodInfo6.Invoke(null, new object[1] { item.Value }), null);
				}
			}
		}
		MethodInfo[] methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		foreach (MethodInfo methodInfo7 in methods)
		{
			if (methodInfo7.GetCustomAttributes(inherit: false).AnyOfType(typeof(AfterDecode)))
			{
				methodInfo7.Invoke(val, (methodInfo7.GetParameters().Length == 0) ? null : new object[1] { data });
			}
		}
		return val;
	}

	private static List<T> DecodeList<T>(Variant data)
	{
		List<T> list = new List<T>();
		if (!(data is ProxyArray proxyArray))
		{
			throw new DecodeException("Variant is expected to be a ProxyArray here, but it is not.");
		}
		foreach (Variant item in (IEnumerable<Variant>)proxyArray)
		{
			list.Add(DecodeType<T>(item));
		}
		return list;
	}

	private static Dictionary<TKey, TValue> DecodeDictionary<TKey, TValue>(Variant data)
	{
		Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();
		Type typeFromHandle = typeof(TKey);
		if (!(data is ProxyObject proxyObject))
		{
			throw new DecodeException("Variant is expected to be a ProxyObject here, but it is not.");
		}
		foreach (KeyValuePair<string, Variant> item in (IEnumerable<KeyValuePair<string, Variant>>)proxyObject)
		{
			TKey key = (TKey)(typeFromHandle.IsEnum ? Enum.Parse(typeFromHandle, item.Key) : Convert.ChangeType(item.Key, typeFromHandle));
			TValue value = DecodeType<TValue>(item.Value);
			dictionary.Add(key, value);
		}
		return dictionary;
	}

	private static T[] DecodeArray<T>(Variant data)
	{
		if (!(data is ProxyArray proxyArray))
		{
			throw new DecodeException("Variant is expected to be a ProxyArray here, but it is not.");
		}
		int count = proxyArray.Count;
		T[] array = new T[count];
		int num = 0;
		foreach (Variant item in (IEnumerable<Variant>)proxyArray)
		{
			array[num++] = DecodeType<T>(item);
		}
		return array;
	}

	private static void DecodeMultiRankArray<T>(ProxyArray arrayData, Array array, int arrayRank, int[] indices)
	{
		int count = arrayData.Count;
		for (int i = 0; i < count; i++)
		{
			indices[arrayRank - 1] = i;
			if (arrayRank < array.Rank)
			{
				DecodeMultiRankArray<T>(arrayData[i] as ProxyArray, array, arrayRank + 1, indices);
			}
			else
			{
				array.SetValue(DecodeType<T>(arrayData[i]), indices);
			}
		}
	}

	public static void SupportTypeForAOT<T>()
	{
		DecodeType<T>(null);
		DecodeList<T>(null);
		DecodeArray<T>(null);
		DecodeDictionary<short, T>(null);
		DecodeDictionary<ushort, T>(null);
		DecodeDictionary<int, T>(null);
		DecodeDictionary<uint, T>(null);
		DecodeDictionary<long, T>(null);
		DecodeDictionary<ulong, T>(null);
		DecodeDictionary<float, T>(null);
		DecodeDictionary<double, T>(null);
		DecodeDictionary<decimal, T>(null);
		DecodeDictionary<bool, T>(null);
		DecodeDictionary<string, T>(null);
	}

	private static void SupportValueTypesForAOT()
	{
		SupportTypeForAOT<short>();
		SupportTypeForAOT<ushort>();
		SupportTypeForAOT<int>();
		SupportTypeForAOT<uint>();
		SupportTypeForAOT<long>();
		SupportTypeForAOT<ulong>();
		SupportTypeForAOT<float>();
		SupportTypeForAOT<double>();
		SupportTypeForAOT<decimal>();
		SupportTypeForAOT<bool>();
		SupportTypeForAOT<string>();
	}
}
