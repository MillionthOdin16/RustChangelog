using System;
using Unity.Mathematics;

namespace Instancing;

public struct MeshOverrideData : IEquatable<MeshOverrideData>
{
	public float4 Color;

	public bool Equals(MeshOverrideData other)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		if (Color.x == other.Color.x && Color.y == other.Color.y && Color.z == other.Color.z)
		{
			return Color.w == other.Color.w;
		}
		return false;
	}

	public override bool Equals(object obj)
	{
		if (obj is MeshOverrideData other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return ((object)(float4)(ref Color)).GetHashCode();
	}

	public static bool operator ==(MeshOverrideData left, MeshOverrideData right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(MeshOverrideData left, MeshOverrideData right)
	{
		return !left.Equals(right);
	}
}
