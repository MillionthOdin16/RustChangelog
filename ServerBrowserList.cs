using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

public class ServerBrowserList : BaseMonoBehaviour, VirtualScroll.IDataSource
{
	public enum QueryType
	{
		RegularInternet,
		Friends,
		History,
		LAN,
		Favourites,
		None
	}

	[Serializable]
	public struct ServerKeyvalues
	{
		public string key;

		public string value;
	}

	[Serializable]
	public struct Rules
	{
		public string tag;

		public ServerBrowserList serverList;
	}

	private class HashSetEqualityComparer<T> : IEqualityComparer<HashSet<T>> where T : IComparable<T>
	{
		public static HashSetEqualityComparer<T> Instance { get; } = new HashSetEqualityComparer<T>();


		public bool Equals(HashSet<T> x, HashSet<T> y)
		{
			if (x == y)
			{
				return true;
			}
			if (x == null)
			{
				return false;
			}
			if (y == null)
			{
				return false;
			}
			if (x.GetType() != y.GetType())
			{
				return false;
			}
			if (x.Count != y.Count)
			{
				return false;
			}
			foreach (T item in x)
			{
				if (!y.Contains(item))
				{
					return false;
				}
			}
			return true;
		}

		public int GetHashCode(HashSet<T> set)
		{
			int num = 0;
			if (set != null)
			{
				foreach (T item in set)
				{
					num ^= (item?.GetHashCode() ?? 0) & 0x7FFFFFFF;
				}
			}
			return num;
		}
	}

	public QueryType queryType;

	public static string VersionTag = "v" + 2402;

	public ServerKeyvalues[] keyValues = new ServerKeyvalues[0];

	public ServerBrowserCategory categoryButton;

	public bool startActive = false;

	public Transform listTransform;

	public int refreshOrder;

	public bool UseOfficialServers;

	public VirtualScroll VirtualScroll;

	public Rules[] rules;

	public bool hideOfficialServers = false;

	public bool excludeEmptyServersUsingQuery = false;

	public bool alwaysIncludeEmptyServers = false;

	public bool clampPlayerCountsToTrustedValues = false;

	public int GetItemCount()
	{
		return 0;
	}

	public void SetItemData(int i, GameObject obj)
	{
	}
}
