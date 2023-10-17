using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Jobs;

namespace CompanionServer.Cameras;

public struct RaycastOutputCompressJob : IJob
{
	[ReadOnly]
	public NativeArray<int> rayOutputs;

	[WriteOnly]
	public NativeArray<int> dataLength;

	[WriteOnly]
	public NativeArray<byte> data;

	public void Execute()
	{
		int num = rayOutputs.Length * 4;
		if (data.Length < num)
		{
			throw new InvalidOperationException("Not enough data buffer available to compress rays");
		}
		NativeArray<int> val = default(NativeArray<int>);
		val._002Ector(64, (Allocator)2, (NativeArrayOptions)1);
		int num2 = 0;
		for (int i = 0; i < rayOutputs.Length; i++)
		{
			int num3 = rayOutputs[i];
			ushort num4 = RayDistance(num3);
			byte b = RayAlignment(num3);
			byte b2 = RayMaterial(num3);
			int num5 = (num4 / 128 * 3 + b / 16 * 5 + b2 * 7) & 0x3F;
			int num6 = val[num5];
			if (num6 == num3)
			{
				data[num2++] = (byte)(0u | (uint)num5);
				continue;
			}
			int num7 = num4 - RayDistance(num6);
			int num8 = b - RayAlignment(num6);
			if (b2 == RayMaterial(num6) && num7 >= -15 && num7 <= 16 && num8 >= -3 && num8 <= 4)
			{
				data[num2++] = (byte)(0x40u | (uint)num5);
				data[num2++] = (byte)((num7 + 15 << 3) | (num8 + 3));
			}
			else if (b2 == RayMaterial(num6) && num8 == 0 && num7 >= -127 && num7 <= 128)
			{
				data[num2++] = (byte)(0x80u | (uint)num5);
				data[num2++] = (byte)(num7 + 127);
			}
			else if (b2 < 63)
			{
				val[num5] = num3;
				data[num2++] = (byte)(0xC0u | b2);
				data[num2++] = (byte)(num4 >> 2);
				data[num2++] = (byte)(((num4 & 3) << 6) | b);
			}
			else
			{
				val[num5] = num3;
				data[num2++] = byte.MaxValue;
				data[num2++] = (byte)(num4 >> 2);
				data[num2++] = (byte)(((num4 & 3) << 6) | b);
				data[num2++] = b2;
			}
		}
		val.Dispose();
		dataLength[0] = num2;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static ushort RayDistance(int ray)
	{
		return (ushort)(ray >> 16);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static byte RayAlignment(int ray)
	{
		return (byte)(ray >> 8);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static byte RayMaterial(int ray)
	{
		return (byte)ray;
	}
}
