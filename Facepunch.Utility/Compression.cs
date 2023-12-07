using System;
using Ionic.Zlib;
using UnityEngine;

namespace Facepunch.Utility;

public class Compression
{
	public static byte[] Compress(byte[] data)
	{
		try
		{
			return GZipStream.CompressBuffer(data);
		}
		catch (Exception)
		{
			return null;
		}
	}

	public static byte[] Uncompress(byte[] data)
	{
		return GZipStream.UncompressBuffer(data);
	}

	public static int PackVector3ToInt(Vector3 vector, float minValue, float maxValue)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = default(Vector3);
		((Vector3)(ref val))._002Ector(Mathf.InverseLerp(minValue, maxValue, vector.x), Mathf.InverseLerp(minValue, maxValue, vector.y), Mathf.InverseLerp(minValue, maxValue, vector.z));
		return 0 | ((int)(val.x * 1023f) << 20) | ((int)(val.y * 1023f) << 10) | (int)(val.z * 1023f);
	}

	public static Vector3 UnpackVector3FromInt(int packed, float minValue, float maxValue)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = new Vector3((float)((packed >> 20) & 0x3FF), (float)((packed >> 10) & 0x3FF), (float)(packed & 0x3FF)) / 1023f;
		return new Vector3(Mathf.Lerp(minValue, maxValue, val.x), Mathf.Lerp(minValue, maxValue, val.y), Mathf.Lerp(minValue, maxValue, val.z));
	}
}
