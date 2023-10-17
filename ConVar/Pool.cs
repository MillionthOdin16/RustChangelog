using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Facepunch;
using Facepunch.Extend;
using Network;
using UnityEngine;

namespace ConVar;

[Factory("pool")]
public class Pool : ConsoleSystem
{
	[ServerVar]
	[ClientVar]
	public static int mode = 2;

	[ServerVar]
	[ClientVar]
	public static bool prewarm = true;

	[ServerVar]
	[ClientVar]
	public static bool enabled = true;

	[ServerVar]
	[ClientVar]
	public static bool debug = false;

	[ServerVar]
	[ClientVar]
	public static void print_memory(Arg arg)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Expected O, but got Unknown
		if (Pool.Directory.Count == 0)
		{
			arg.ReplyWith("Memory pool is empty.");
			return;
		}
		TextTable val = new TextTable();
		val.AddColumn("type");
		val.AddColumn("capacity");
		val.AddColumn("pooled");
		val.AddColumn("active");
		val.AddColumn("hits");
		val.AddColumn("misses");
		val.AddColumn("spills");
		foreach (KeyValuePair<Type, IPoolCollection> item in Pool.Directory.OrderByDescending((KeyValuePair<Type, IPoolCollection> x) => x.Value.ItemsCreated))
		{
			Type key = item.Key;
			IPoolCollection value = item.Value;
			val.AddRow(new string[7]
			{
				key.ToString().Replace("System.Collections.Generic.", ""),
				NumberExtensions.FormatNumberShort(value.ItemsCapacity),
				NumberExtensions.FormatNumberShort(value.ItemsInStack),
				NumberExtensions.FormatNumberShort(value.ItemsInUse),
				NumberExtensions.FormatNumberShort(value.ItemsTaken),
				NumberExtensions.FormatNumberShort(value.ItemsCreated),
				NumberExtensions.FormatNumberShort(value.ItemsSpilled)
			});
		}
		arg.ReplyWith(arg.HasArg("--json") ? val.ToJson() : ((object)val).ToString());
	}

	[ServerVar]
	[ClientVar]
	public static void print_arraypool(Arg arg)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Expected O, but got Unknown
		ArrayPool<byte> arrayPool = BaseNetwork.ArrayPool;
		ConcurrentQueue<byte[]>[] buffer = arrayPool.GetBuffer();
		TextTable val = new TextTable();
		val.AddColumn("index");
		val.AddColumn("size");
		val.AddColumn("bytes");
		val.AddColumn("count");
		val.AddColumn("memory");
		for (int i = 0; i < buffer.Length; i++)
		{
			int num = arrayPool.IndexToSize(i);
			int count = buffer[i].Count;
			int num2 = num * count;
			val.AddRow(new string[5]
			{
				i.ToString(),
				num.ToString(),
				NumberExtensions.FormatBytes<int>(num, false),
				count.ToString(),
				NumberExtensions.FormatBytes<int>(num2, false)
			});
		}
		arg.ReplyWith(arg.HasArg("--json") ? val.ToJson() : ((object)val).ToString());
	}

	[ServerVar]
	[ClientVar]
	public static void print_prefabs(Arg arg)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Expected O, but got Unknown
		PrefabPoolCollection pool = GameManager.server.pool;
		if (pool.storage.Count == 0)
		{
			arg.ReplyWith("Prefab pool is empty.");
			return;
		}
		string @string = arg.GetString(0, string.Empty);
		TextTable val = new TextTable();
		val.AddColumn("id");
		val.AddColumn("name");
		val.AddColumn("count");
		foreach (KeyValuePair<uint, PrefabPool> item in pool.storage)
		{
			string text = item.Key.ToString();
			string text2 = StringPool.Get(item.Key);
			string text3 = item.Value.Count.ToString();
			if (string.IsNullOrEmpty(@string) || StringEx.Contains(text2, @string, CompareOptions.IgnoreCase))
			{
				val.AddRow(new string[3]
				{
					text,
					Path.GetFileNameWithoutExtension(text2),
					text3
				});
			}
		}
		arg.ReplyWith(arg.HasArg("--json") ? val.ToJson() : ((object)val).ToString());
	}

	[ServerVar]
	[ClientVar]
	public static void print_assets(Arg arg)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Expected O, but got Unknown
		if (AssetPool.storage.Count == 0)
		{
			arg.ReplyWith("Asset pool is empty.");
			return;
		}
		string @string = arg.GetString(0, string.Empty);
		TextTable val = new TextTable();
		val.AddColumn("type");
		val.AddColumn("allocated");
		val.AddColumn("available");
		foreach (KeyValuePair<Type, Pool> item in AssetPool.storage)
		{
			string text = item.Key.ToString();
			string text2 = item.Value.allocated.ToString();
			string text3 = item.Value.available.ToString();
			if (string.IsNullOrEmpty(@string) || StringEx.Contains(text, @string, CompareOptions.IgnoreCase))
			{
				val.AddRow(new string[3] { text, text2, text3 });
			}
		}
		arg.ReplyWith(arg.HasArg("--json") ? val.ToJson() : ((object)val).ToString());
	}

	[ServerVar]
	[ClientVar]
	public static void clear_memory(Arg arg)
	{
		Pool.Clear(arg.GetString(0, string.Empty));
	}

	[ServerVar]
	[ClientVar]
	public static void clear_prefabs(Arg arg)
	{
		string @string = arg.GetString(0, string.Empty);
		GameManager.server.pool.Clear(@string);
	}

	[ServerVar]
	[ClientVar]
	public static void clear_assets(Arg arg)
	{
		AssetPool.Clear(arg.GetString(0, string.Empty));
	}

	[ServerVar]
	[ClientVar]
	public static void export_prefabs(Arg arg)
	{
		PrefabPoolCollection pool = GameManager.server.pool;
		if (pool.storage.Count == 0)
		{
			arg.ReplyWith("Prefab pool is empty.");
			return;
		}
		string @string = arg.GetString(0, string.Empty);
		StringBuilder stringBuilder = new StringBuilder();
		foreach (KeyValuePair<uint, PrefabPool> item in pool.storage)
		{
			string arg2 = item.Key.ToString();
			string text = StringPool.Get(item.Key);
			string arg3 = item.Value.Count.ToString();
			if (string.IsNullOrEmpty(@string) || StringEx.Contains(text, @string, CompareOptions.IgnoreCase))
			{
				stringBuilder.AppendLine($"{arg2},{Path.GetFileNameWithoutExtension(text)},{arg3}");
			}
		}
		File.WriteAllText("prefabs.csv", stringBuilder.ToString());
	}

	[ServerVar]
	[ClientVar]
	public static void fill_prefabs(Arg arg)
	{
		string @string = arg.GetString(0, string.Empty);
		int @int = arg.GetInt(1, 0);
		PrefabPoolWarmup.Run(@string, @int);
	}
}
