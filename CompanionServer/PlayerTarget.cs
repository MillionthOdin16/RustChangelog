using System;

namespace CompanionServer;

public readonly struct PlayerTarget : IEquatable<PlayerTarget>
{
	public ulong SteamId { get; }

	public PlayerTarget(ulong steamId)
	{
		SteamId = steamId;
	}

	public bool Equals(PlayerTarget other)
	{
		return SteamId == other.SteamId;
	}

	public override bool Equals(object obj)
	{
		return obj is PlayerTarget other && Equals(other);
	}

	public override int GetHashCode()
	{
		return SteamId.GetHashCode();
	}

	public static bool operator ==(PlayerTarget left, PlayerTarget right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(PlayerTarget left, PlayerTarget right)
	{
		return !left.Equals(right);
	}
}
