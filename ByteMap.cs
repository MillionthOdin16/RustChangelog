using System;
using UnityEngine;

[Serializable]
public class ByteMap
{
	[SerializeField]
	private int size;

	[SerializeField]
	private int bytes;

	[SerializeField]
	private byte[] values;

	public int Size => size;

	public uint this[int x, int y]
	{
		get
		{
			int num = y * bytes * size + x * bytes;
			switch (bytes)
			{
			case 1:
				return values[num];
			case 2:
			{
				uint num2 = values[num];
				uint num3 = values[num + 1];
				return (num2 << 8) | num3;
			}
			case 3:
			{
				uint num2 = values[num];
				uint num3 = values[num + 1];
				uint num4 = values[num + 2];
				return (num2 << 16) | (num3 << 8) | num4;
			}
			default:
			{
				uint num2 = values[num];
				uint num3 = values[num + 1];
				uint num4 = values[num + 2];
				uint num5 = values[num + 3];
				return (num2 << 24) | (num3 << 16) | (num4 << 8) | num5;
			}
			}
		}
		set
		{
			int num = y * bytes * size + x * bytes;
			switch (bytes)
			{
			case 1:
				values[num] = (byte)(value & 0xFFu);
				break;
			case 2:
				values[num] = (byte)((value >> 8) & 0xFFu);
				values[num + 1] = (byte)(value & 0xFFu);
				break;
			case 3:
				values[num] = (byte)((value >> 16) & 0xFFu);
				values[num + 1] = (byte)((value >> 8) & 0xFFu);
				values[num + 2] = (byte)(value & 0xFFu);
				break;
			default:
				values[num] = (byte)((value >> 24) & 0xFFu);
				values[num + 1] = (byte)((value >> 16) & 0xFFu);
				values[num + 2] = (byte)((value >> 8) & 0xFFu);
				values[num + 3] = (byte)(value & 0xFFu);
				break;
			}
		}
	}

	public ByteMap(int size, int bytes = 1)
	{
		this.size = size;
		this.bytes = bytes;
		values = new byte[bytes * size * size];
	}

	public ByteMap(int size, byte[] values, int bytes = 1)
	{
		this.size = size;
		this.bytes = bytes;
		this.values = values;
	}
}
