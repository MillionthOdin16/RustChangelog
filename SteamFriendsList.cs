using System;
using UnityEngine;
using UnityEngine.Events;

public class SteamFriendsList : MonoBehaviour
{
	[Serializable]
	public class onFriendSelectedEvent : UnityEvent<ulong, string>
	{
	}

	public RectTransform targetPanel;

	public SteamUserButton userButton;

	public bool IncludeFriendsList = true;

	public bool IncludeRecentlySeen = false;

	public bool IncludeLastAttacker = false;

	public bool IncludeRecentlyPlayedWith = false;

	public bool ShowTeamFirst = false;

	public bool HideSteamIdsInStreamerMode = false;

	public bool RefreshOnEnable = true;

	public onFriendSelectedEvent onFriendSelected;

	public Func<ulong, bool> shouldShowPlayer;
}
