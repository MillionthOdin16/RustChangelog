using System;
using System.IO;
using UnityEngine.Profiling.Memory.Experimental;

namespace ConVar;

[Factory("memsnap")]
public class MemSnap : ConsoleSystem
{
	private static string NeedProfileFolder()
	{
		string path = "profile";
		if (!Directory.Exists(path))
		{
			return Directory.CreateDirectory(path).FullName;
		}
		return new DirectoryInfo(path).FullName;
	}

	[ClientVar]
	[ServerVar]
	public static void managed(Arg arg)
	{
		string text = NeedProfileFolder();
		string text2 = text + "/memdump-" + DateTime.Now.ToString("MM-dd-yyyy-h-mm-ss") + ".snap";
		MemoryProfiler.TakeSnapshot(text2, (Action<string, bool>)null, (CaptureFlags)1);
	}

	[ClientVar]
	[ServerVar]
	public static void native(Arg arg)
	{
		string text = NeedProfileFolder();
		string text2 = text + "/memdump-" + DateTime.Now.ToString("MM-dd-yyyy-h-mm-ss") + ".snap";
		MemoryProfiler.TakeSnapshot(text2, (Action<string, bool>)null, (CaptureFlags)2);
	}

	[ClientVar]
	[ServerVar]
	public static void full(Arg arg)
	{
		string text = NeedProfileFolder();
		string text2 = text + "/memdump-" + DateTime.Now.ToString("MM-dd-yyyy-h-mm-ss") + ".snap";
		MemoryProfiler.TakeSnapshot(text2, (Action<string, bool>)null, (CaptureFlags)31);
	}
}
