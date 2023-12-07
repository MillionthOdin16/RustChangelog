using System;
using Ionic.Zlib;
using UnityEngine.Profiling;

namespace Facepunch.Utility;

public class Compression
{
	public static byte[] Compress(byte[] data)
	{
		try
		{
			Profiler.BeginSample("Compress");
			byte[] result = GZipStream.CompressBuffer(data);
			Profiler.EndSample();
			return result;
		}
		catch (Exception)
		{
			return null;
		}
	}

	public static byte[] Uncompress(byte[] data)
	{
		Profiler.BeginSample("Uncompress");
		byte[] result = GZipStream.UncompressBuffer(data);
		Profiler.EndSample();
		return result;
	}
}
