using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

public class SteamUserButton : MonoBehaviour
{
	public RustText steamName;

	public RustText steamInfo;

	public RawImage avatar;

	public Color colorTeamOnline;

	public Color colorTeamOffline;

	public Color colorFriendOnline;

	public Color colorFriendOffline;

	public Color colorOnline;

	public Color colorOffline;

	public ulong SteamId { get; private set; }

	public string Username { get; private set; }
}
