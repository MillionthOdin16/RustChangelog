using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ConVar;

[Factory("hierarchy")]
public class Hierarchy : ConsoleSystem
{
	private static GameObject currentDir;

	private static Transform[] GetCurrent()
	{
		if ((Object)(object)currentDir == (Object)null)
		{
			return TransformUtil.GetRootObjects().ToArray();
		}
		List<Transform> list = new List<Transform>();
		for (int i = 0; i < currentDir.transform.childCount; i++)
		{
			list.Add(currentDir.transform.GetChild(i));
		}
		return list.ToArray();
	}

	[ServerVar]
	public static void ls(Arg args)
	{
		string text = "";
		string filter = args.GetString(0, "");
		text = ((!Object.op_Implicit((Object)(object)currentDir)) ? (text + "Listing .\n\n") : (text + "Listing " + currentDir.transform.GetRecursiveName() + "\n\n"));
		foreach (Transform item in (from x in GetCurrent()
			where string.IsNullOrEmpty(filter) || ((Object)x).name.Contains(filter)
			select x).Take(40))
		{
			text += $"   {((Object)item).name} [{item.childCount}]\n";
		}
		text += "\n";
		args.ReplyWith(text);
	}

	[ServerVar]
	public static void cd(Arg args)
	{
		if (args.FullString == ".")
		{
			currentDir = null;
			args.ReplyWith("Changed to .");
			return;
		}
		if (args.FullString == "..")
		{
			if (Object.op_Implicit((Object)(object)currentDir))
			{
				currentDir = (Object.op_Implicit((Object)(object)currentDir.transform.parent) ? ((Component)currentDir.transform.parent).gameObject : null);
			}
			currentDir = null;
			if (Object.op_Implicit((Object)(object)currentDir))
			{
				args.ReplyWith("Changed to " + currentDir.transform.GetRecursiveName());
			}
			else
			{
				args.ReplyWith("Changed to .");
			}
			return;
		}
		Transform val = GetCurrent().FirstOrDefault((Transform x) => ((Object)x).name.ToLower() == args.FullString.ToLower());
		if ((Object)(object)val == (Object)null)
		{
			val = GetCurrent().FirstOrDefault((Transform x) => ((Object)x).name.StartsWith(args.FullString, StringComparison.CurrentCultureIgnoreCase));
		}
		if (Object.op_Implicit((Object)(object)val))
		{
			currentDir = ((Component)val).gameObject;
			args.ReplyWith("Changed to " + currentDir.transform.GetRecursiveName());
		}
		else
		{
			args.ReplyWith("Couldn't find \"" + args.FullString + "\"");
		}
	}

	[ServerVar]
	public static void del(Arg args)
	{
		if (!args.HasArgs(1))
		{
			return;
		}
		IEnumerable<Transform> enumerable = from x in GetCurrent()
			where ((Object)x).name.ToLower() == args.FullString.ToLower()
			select x;
		if (enumerable.Count() == 0)
		{
			enumerable = from x in GetCurrent()
				where ((Object)x).name.StartsWith(args.FullString, StringComparison.CurrentCultureIgnoreCase)
				select x;
		}
		if (enumerable.Count() == 0)
		{
			args.ReplyWith("Couldn't find  " + args.FullString);
			return;
		}
		foreach (Transform item in enumerable)
		{
			BaseEntity baseEntity = ((Component)item).gameObject.ToBaseEntity();
			if (baseEntity.IsValid())
			{
				if (baseEntity.isServer)
				{
					baseEntity.Kill();
				}
			}
			else
			{
				GameManager.Destroy(((Component)item).gameObject);
			}
		}
		args.ReplyWith("Deleted " + enumerable.Count() + " objects");
	}
}
