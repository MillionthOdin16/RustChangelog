using System;

public struct CameraViewerId : IEquatable<CameraViewerId>
{
	public readonly ulong SteamId;

	public readonly long ConnectionId;

	public CameraViewerId(ulong steamId, long connectionId)
	{
		SteamId = steamId;
		ConnectionId = connectionId;
	}

	public bool Equals(CameraViewerId other)
	{
		return SteamId == other.SteamId && ConnectionId == other.ConnectionId;
	}

	public override bool Equals(object obj)
	{
		return obj is CameraViewerId other && Equals(other);
	}

	public override int GetHashCode()
	{
		return (SteamId.GetHashCode() * 397) ^ ConnectionId.GetHashCode();
	}

	public static bool operator ==(CameraViewerId left, CameraViewerId right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(CameraViewerId left, CameraViewerId right)
	{
		return !left.Equals(right);
	}
}
