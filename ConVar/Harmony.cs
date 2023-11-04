namespace ConVar;

[Factory("harmony")]
public class Harmony : ConsoleSystem
{
	[ServerVar(Name = "load")]
	public static void Load(Arg args)
	{
		string @string = args.GetString(0, "");
		HarmonyLoader.TryLoadMod(@string);
	}

	[ServerVar(Name = "unload")]
	public static void Unload(Arg args)
	{
		string @string = args.GetString(0, "");
		HarmonyLoader.TryUnloadMod(@string);
	}
}
