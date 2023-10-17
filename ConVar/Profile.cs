using System;
using System.IO;
using UnityEngine.Profiling;

namespace ConVar;

[Factory("profile")]
public class Profile : ConsoleSystem
{
	private static void NeedProfileFolder()
	{
		if (!Directory.Exists("profile"))
		{
			Directory.CreateDirectory("profile");
		}
	}

	[ClientVar]
	[ServerVar]
	public static void start(Arg arg)
	{
		NeedProfileFolder();
		string text = "profile/" + DateTime.Now.ToString("MM-dd-yyyy-h-mm-ss") + ".profile";
		arg.ReplyWith("Starting profile to " + text);
		Profiler.logFile = text;
		Profiler.enableBinaryLog = true;
		Profiler.enabled = true;
	}

	[ServerVar]
	[ClientVar]
	public static void stop(Arg arg)
	{
		if (Profiler.enableBinaryLog && Profiler.enabled)
		{
			Profiler.enabled = false;
			Profiler.enableBinaryLog = false;
			Profiler.logFile = "null";
			arg.ReplyWith("Profile Ended!");
		}
	}

	[ServerVar]
	[ClientVar]
	public static void flush_analytics(Arg arg)
	{
	}
}
