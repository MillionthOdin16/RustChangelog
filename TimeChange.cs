using System;
using UnityEngine;

[Serializable]
public class TimeChange
{
	public enum ChangeType
	{
		None,
		Set,
		Advance
	}

	public ChangeType changeType;

	public float value;

	public static float Apply(float prevTime, TimeChange change, BasePlayer player)
	{
		if ((Object)(object)player == (Object)null)
		{
			return prevTime;
		}
		if (change == null)
		{
			return prevTime;
		}
		switch (change.changeType)
		{
		case ChangeType.None:
			return prevTime;
		case ChangeType.Set:
			player.ClientRPCPlayer(null, player, "SetLocalTimeOverride", change.value);
			return change.value;
		case ChangeType.Advance:
			player.ClientRPCPlayer(null, player, "AdvanceLocalTimeOverride", change.value);
			return prevTime + change.value;
		default:
			return prevTime;
		}
	}
}
