using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace Instancing;

public struct DrawCallKey : IEquatable<DrawCallKey>, IComparable<DrawCallKey>, IComparable
{
	public Material Material;

	public ShadowCastingMode ShadowMode;

	public bool ReceiveShadows;

	public LightProbeUsage LightProbes;

	public DrawCallKey(Material material, ShadowCastingMode shadowMode, bool receiveShadows, LightProbeUsage lightProbes)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		Material = material;
		ShadowMode = shadowMode;
		ReceiveShadows = receiveShadows;
		LightProbes = lightProbes;
	}

	public int CompareTo(DrawCallKey other)
	{
		return GetHashCode().CompareTo(other.GetHashCode());
	}

	public int CompareTo(object obj)
	{
		if (obj == null)
		{
			return 1;
		}
		if (obj is DrawCallKey other)
		{
			return CompareTo(other);
		}
		throw new ArgumentException("Object must be 'DrawCallKey'");
	}

	public bool Equals(DrawCallKey other)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)Material == (Object)(object)other.Material && ShadowMode == other.ShadowMode && ReceiveShadows == other.ReceiveShadows)
		{
			return LightProbes == other.LightProbes;
		}
		return false;
	}

	public override bool Equals(object obj)
	{
		if (obj is DrawCallKey other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		return HashCode.Combine<int, ShadowCastingMode, bool, LightProbeUsage>(((object)Material)?.GetHashCode() ?? 0, ShadowMode, ReceiveShadows, LightProbes);
	}

	public static bool operator ==(DrawCallKey a, DrawCallKey b)
	{
		return a.Equals(b);
	}

	public static bool operator !=(DrawCallKey a, DrawCallKey b)
	{
		return !(a == b);
	}
}
