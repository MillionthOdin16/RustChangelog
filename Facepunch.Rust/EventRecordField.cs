using System;
using System.IO;
using UnityEngine;

namespace Facepunch.Rust;

public struct EventRecordField
{
	public string Key1;

	public string Key2;

	public string String;

	public long? Number;

	public double? Float;

	public Vector3? Vector;

	public Guid? Guid;

	public DateTime DateTime;

	public bool IsObject;

	public EventRecordField(string key1)
	{
		Key1 = key1;
		Key2 = null;
		String = null;
		Number = null;
		Float = null;
		Vector = null;
		Guid = null;
		IsObject = false;
		DateTime = default(DateTime);
	}

	public EventRecordField(string key1, string key2)
	{
		Key1 = key1;
		Key2 = key2;
		String = null;
		Number = null;
		Float = null;
		Vector = null;
		Guid = null;
		IsObject = false;
		DateTime = default(DateTime);
	}

	public void Serialize(StreamWriter writer)
	{
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		if (String != null)
		{
			if (IsObject)
			{
				writer.Write(String);
				return;
			}
			string @string = String;
			int length = String.Length;
			for (int i = 0; i < length; i++)
			{
				char c = @string[i];
				switch (c)
				{
				case '"':
				case '\\':
					writer.Write('\\');
					writer.Write(c);
					break;
				case '\n':
					writer.Write("\\n");
					break;
				case '\r':
					writer.Write("\\r");
					break;
				case '\t':
					writer.Write("\\t");
					break;
				default:
					writer.Write(c);
					break;
				}
			}
		}
		else if (Float.HasValue)
		{
			writer.Write(Float.Value);
		}
		else if (Number.HasValue)
		{
			writer.Write(Number.Value);
		}
		else if (Guid.HasValue)
		{
			writer.Write(Guid.Value.ToString("N"));
		}
		else if (Vector.HasValue)
		{
			writer.Write('(');
			Vector3 value = Vector.Value;
			writer.Write(value.x);
			writer.Write(',');
			writer.Write(value.y);
			writer.Write(',');
			writer.Write(value.z);
			writer.Write(')');
		}
		else if (DateTime != default(DateTime))
		{
			writer.Write(DateTime.ToString("o"));
		}
	}
}
