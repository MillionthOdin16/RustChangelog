using System;
using System.Collections.Generic;
using UnityEngine;

namespace Facepunch.Rust;

public class EventRecord : IPooled
{
	public DateTime Timestamp;

	[NonSerialized]
	public bool IsServer;

	public Dictionary<string, object> Data = new Dictionary<string, object>();

	public void EnterPool()
	{
		Timestamp = default(DateTime);
		Data.Clear();
	}

	public void LeavePool()
	{
	}

	public static EventRecord New(string type, bool isServer = true)
	{
		EventRecord eventRecord = Pool.Get<EventRecord>();
		eventRecord.AddField("type", type);
		eventRecord.IsServer = isServer;
		eventRecord.Timestamp = DateTime.UtcNow;
		return eventRecord;
	}

	public EventRecord AddObject(string key, object data)
	{
		Data[key] = data;
		return this;
	}

	public EventRecord SetTimestamp(DateTime timestamp)
	{
		Timestamp = timestamp;
		return this;
	}

	public EventRecord AddField(string key, bool value)
	{
		Data[key] = (value ? "true" : "false");
		return this;
	}

	public EventRecord AddField(string key, string value)
	{
		Data[key] = value;
		return this;
	}

	public EventRecord AddField(string key, int value)
	{
		Data[key] = value.ToString();
		return this;
	}

	public EventRecord AddField(string key, uint value)
	{
		Data[key] = value.ToString();
		return this;
	}

	public EventRecord AddField(string key, ulong value)
	{
		Data[key] = value.ToString();
		return this;
	}

	public EventRecord AddField(string key, long value)
	{
		Data[key] = value.ToString();
		return this;
	}

	public EventRecord AddField(string key, float value)
	{
		Data[key] = value.ToString();
		return this;
	}

	public EventRecord AddField(string key, double value)
	{
		Data[key] = value.ToString();
		return this;
	}

	public EventRecord AddField(string key, TimeSpan value)
	{
		Data[key] = value.TotalSeconds.ToString();
		return this;
	}

	public EventRecord AddField(string key, BaseEntity entity)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		Data[key + "_prefab"] = entity.ShortPrefabName;
		Dictionary<string, object> data = Data;
		string key2 = key + "_pos";
		Vector3 position = ((Component)entity).transform.position;
		data[key2] = ((object)(Vector3)(ref position)).ToString();
		Data[key + "_id"] = entity.net.ID.ToString();
		return this;
	}

	public void Submit()
	{
		if (IsServer)
		{
			Analytics.AzureWebInterface.server.EnqueueEvent(this);
		}
	}
}
