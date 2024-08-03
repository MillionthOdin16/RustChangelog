namespace UnityEngine;

public static class ArgEx
{
	public static BasePlayer Player(this Arg arg)
	{
		if (arg == null || arg.Connection == null)
		{
			return null;
		}
		return arg.Connection.player as BasePlayer;
	}

	public static BasePlayer GetPlayer(this Arg arg, int iArgNum)
	{
		string @string = arg.GetString(iArgNum, (string)null);
		if (@string == null)
		{
			return null;
		}
		return BasePlayer.Find(@string);
	}

	public static BasePlayer GetSleeper(this Arg arg, int iArgNum)
	{
		string @string = arg.GetString(iArgNum, "");
		if (@string == null)
		{
			return null;
		}
		return BasePlayer.FindSleeping(@string);
	}

	public static BasePlayer GetPlayerOrSleeper(this Arg arg, int iArgNum)
	{
		string @string = arg.GetString(iArgNum, "");
		if (@string == null)
		{
			return null;
		}
		return BasePlayer.FindAwakeOrSleeping(@string);
	}

	public static BasePlayer GetPlayerOrSleeperOrBot(this Arg arg, int iArgNum)
	{
		uint num = default(uint);
		if (arg.TryGetUInt(iArgNum, ref num))
		{
			return BasePlayer.FindBot(num);
		}
		return arg.GetPlayerOrSleeper(iArgNum);
	}

	public static NetworkableId GetEntityID(this Arg arg, int iArg, NetworkableId def = default(NetworkableId))
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		return new NetworkableId(arg.GetUInt64(iArg, def.Value));
	}

	public static ItemId GetItemID(this Arg arg, int iArg, ItemId def = default(ItemId))
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		return new ItemId(arg.GetUInt64(iArg, def.Value));
	}
}
