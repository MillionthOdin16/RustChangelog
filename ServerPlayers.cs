using System;
using System.Collections.Generic;
using UnityEngine;

public static class ServerPlayers
{
	private static readonly HashSet<ulong> OnlineUserIdSet = new HashSet<ulong>();

	private static int _currentFrame;

	public static bool IsOnline(ulong userId)
	{
		RebuildIfNecessary();
		return OnlineUserIdSet.Contains(userId);
	}

	public static void GetAll(List<ulong> userIds)
	{
		RebuildIfNecessary();
		foreach (ulong item in OnlineUserIdSet)
		{
			userIds.Add(item);
		}
	}

	private static void RebuildIfNecessary()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		int frameCount = Time.frameCount;
		if (frameCount == _currentFrame)
		{
			return;
		}
		_currentFrame = frameCount;
		OnlineUserIdSet.Clear();
		Enumerator<BasePlayer> enumerator = BasePlayer.activePlayerList.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				BasePlayer current = enumerator.Current;
				OnlineUserIdSet.Add(current.userID);
			}
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
	}
}
