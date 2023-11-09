using System;

namespace CompanionServer;

public readonly struct CameraTarget : IEquatable<CameraTarget>
{
	public NetworkableId EntityId { get; }

	public CameraTarget(NetworkableId entityId)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		EntityId = entityId;
	}

	public bool Equals(CameraTarget other)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		return EntityId == other.EntityId;
	}

	public override bool Equals(object obj)
	{
		if (obj is CameraTarget other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		NetworkableId entityId = EntityId;
		return ((object)(NetworkableId)(ref entityId)).GetHashCode();
	}

	public static bool operator ==(CameraTarget left, CameraTarget right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(CameraTarget left, CameraTarget right)
	{
		return !left.Equals(right);
	}
}
