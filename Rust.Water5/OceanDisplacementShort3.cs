using Unity.Mathematics;
using UnityEngine;

namespace Rust.Water5;

public struct OceanDisplacementShort3
{
	private const float precision = 20f;

	private const float float2short = 32766f;

	private const float short2float = 3.051944E-05f;

	public short x;

	public short y;

	public short z;

	public static implicit operator Vector3(OceanDisplacementShort3 v)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		Vector3 result = default(Vector3);
		result.x = 3.051944E-05f * (float)v.x * 20f;
		result.y = 3.051944E-05f * (float)v.y * 20f;
		result.z = 3.051944E-05f * (float)v.z * 20f;
		return result;
	}

	public static implicit operator OceanDisplacementShort3(Vector3 v)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		OceanDisplacementShort3 result = default(OceanDisplacementShort3);
		result.x = (short)(v.x / 20f * 32766f + 0.5f);
		result.y = (short)(v.y / 20f * 32766f + 0.5f);
		result.z = (short)(v.z / 20f * 32766f + 0.5f);
		return result;
	}

	public static implicit operator OceanDisplacementShort3(float3 v)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		OceanDisplacementShort3 result = default(OceanDisplacementShort3);
		result.x = (short)(v.x / 20f * 32766f + 0.5f);
		result.y = (short)(v.y / 20f * 32766f + 0.5f);
		result.z = (short)(v.z / 20f * 32766f + 0.5f);
		return result;
	}
}
