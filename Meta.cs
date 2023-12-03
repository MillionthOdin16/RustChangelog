using System;
using System.Collections.Generic;
using Facepunch;
using UnityEngine;

[Factory("meta")]
public class Meta : ConsoleSystem
{
	[ServerVar(Clientside = true, Help = "add <convar> <amount> - adds amount to convar")]
	public static void add(Arg args)
	{
		string @string = args.GetString(0, "");
		float @float = args.GetFloat(1, 0.1f);
		Command val = Find(@string);
		float result;
		if (val == null)
		{
			args.ReplyWith("Convar not found: " + (@string ?? "<null>"));
		}
		else if (args.IsClientside && val.Replicated)
		{
			args.ReplyWith("Cannot set replicated convars from the client (use sv to do this)");
		}
		else if (args.IsServerside && val.ServerAdmin && !args.IsAdmin)
		{
			args.ReplyWith("Permission denied");
		}
		else if (!float.TryParse(val.String, out result))
		{
			args.ReplyWith("Convar value cannot be parsed as a number");
		}
		else
		{
			val.Set(result + @float);
		}
	}

	[ClientVar(Help = "if_true <command> <condition> - runs a command if the condition is true")]
	public static void if_true(Arg args)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		string @string = args.GetString(0, "");
		bool @bool = args.GetBool(1, false);
		if (@bool)
		{
			ConsoleSystem.Run(Option.Client, @string, new object[1] { @bool });
		}
	}

	[ClientVar(Help = "if_false <command> <condition> - runs a command if the condition is false")]
	public static void if_false(Arg args)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		string @string = args.GetString(0, "");
		bool @bool = args.GetBool(1, true);
		if (!@bool)
		{
			ConsoleSystem.Run(Option.Client, @string, new object[1] { @bool });
		}
	}

	[ClientVar(Help = "reset_cycle <key> - resets a cycled bind to the beginning")]
	public static void reset_cycle(Arg args)
	{
		string @string = args.GetString(0, "");
		List<KeyCode> list = default(List<KeyCode>);
		KeyCombos.TryParse(ref @string, ref list);
		Button button = Input.GetButton(@string);
		if (button == null)
		{
			args.ReplyWith("Button not found");
		}
		else if (!button.Cycle)
		{
			args.ReplyWith("Button does not have a cycled bind");
		}
		else
		{
			button.CycleIndex = 0;
		}
	}

	[ClientVar(Help = "exec [command_1] ... - runs all of the commands passed as arguments (also, if the last argument is true/false then that will flow into each command's arguments)")]
	public static void exec(Arg args)
	{
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		List<string> list = Pool.GetList<string>();
		for (int i = 0; i < 32; i++)
		{
			string @string = args.GetString(i, "");
			if (string.IsNullOrWhiteSpace(@string))
			{
				break;
			}
			list.Add(@string);
		}
		if (list.Count > 0)
		{
			string text = null;
			string text2 = list[list.Count - 1];
			if (bool.TryParse(text2, out var _))
			{
				text = text2;
				list.RemoveAt(list.Count - 1);
			}
			foreach (string item in list)
			{
				if (text != null)
				{
					ConsoleSystem.Run(Option.Client, item, new object[1] { text });
				}
				else
				{
					ConsoleSystem.Run(Option.Client, item, Array.Empty<object>());
				}
			}
		}
		Pool.FreeList<string>(ref list);
	}

	private static Command Find(string name)
	{
		Command val = Server.Find(name);
		if (val != null)
		{
			return val;
		}
		return null;
	}
}
