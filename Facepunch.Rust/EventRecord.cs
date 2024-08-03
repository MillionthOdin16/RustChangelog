using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace Facepunch.Rust;

public class EventRecord : IPooled
{
	public static readonly long TicksToNS = 1000000000 / Stopwatch.Frequency;

	public DateTime Timestamp;

	[NonSerialized]
	public bool IsServer;

	public List<EventRecordField> Data = new List<EventRecordField>();

	public int TimesCreated;

	public int TimesSubmitted;

	public string EventType { get; private set; }

	public void EnterPool()
	{
		Timestamp = default(DateTime);
		EventType = null;
		IsServer = false;
		Data.Clear();
	}

	public void LeavePool()
	{
	}

	public static EventRecord CSV()
	{
		EventRecord eventRecord = Pool.Get<EventRecord>();
		eventRecord.IsServer = true;
		eventRecord.TimesCreated++;
		return eventRecord;
	}

	public static EventRecord New(string type, bool isServer = true)
	{
		EventRecord eventRecord = Pool.Get<EventRecord>();
		eventRecord.EventType = type;
		eventRecord.AddField("type", type);
		eventRecord.AddField("guid", Guid.NewGuid());
		BuildInfo current = BuildInfo.Current;
		bool num = (current.Scm.Branch != null && current.Scm.Branch == "experimental/release") || current.Scm.Branch == "release";
		bool isEditor = Application.isEditor;
		string value = ((num && !isEditor) ? "release" : (isEditor ? "editor" : "staging"));
		eventRecord.AddField("environment", value);
		eventRecord.IsServer = isServer;
		if (isServer)
		{
			eventRecord.AddField("wipe_id", SaveRestore.WipeId);
		}
		eventRecord.Timestamp = DateTime.UtcNow;
		eventRecord.TimesCreated++;
		return eventRecord;
	}

	public EventRecord AddObject(string key, object data)
	{
		if (data == null)
		{
			return this;
		}
		Data.Add(new EventRecordField(key)
		{
			String = JsonConvert.SerializeObject(data),
			IsObject = true
		});
		return this;
	}

	public EventRecord SetTimestamp(DateTime timestamp)
	{
		Timestamp = timestamp;
		return this;
	}

	public EventRecord AddField(string key, DateTime time)
	{
		Data.Add(new EventRecordField(key)
		{
			DateTime = time
		});
		return this;
	}

	public EventRecord AddField(string key, bool value)
	{
		Data.Add(new EventRecordField(key)
		{
			String = (value ? "true" : "false")
		});
		return this;
	}

	public EventRecord AddField(string key, string value)
	{
		Data.Add(new EventRecordField(key)
		{
			String = value
		});
		return this;
	}

	public EventRecord AddField(string key, int value)
	{
		Data.Add(new EventRecordField(key)
		{
			Number = value
		});
		return this;
	}

	public EventRecord AddField(string key, uint value)
	{
		Data.Add(new EventRecordField(key)
		{
			Number = value
		});
		return this;
	}

	public EventRecord AddField(string key, ulong value)
	{
		Data.Add(new EventRecordField(key)
		{
			Number = (long)value
		});
		return this;
	}

	public EventRecord AddField(string key, long value)
	{
		Data.Add(new EventRecordField(key)
		{
			Number = value
		});
		return this;
	}

	public EventRecord AddField(string key, float value)
	{
		Data.Add(new EventRecordField(key)
		{
			Float = value
		});
		return this;
	}

	public EventRecord AddField(string key, double value)
	{
		Data.Add(new EventRecordField(key)
		{
			Float = value
		});
		return this;
	}

	public EventRecord AddField(string key, TimeSpan value)
	{
		Data.Add(new EventRecordField(key)
		{
			Number = value.Ticks * TicksToNS
		});
		return this;
	}

	public EventRecord AddLegacyTimespan(string key, TimeSpan value)
	{
		Data.Add(new EventRecordField(key)
		{
			Float = value.TotalSeconds
		});
		return this;
	}

	public EventRecord AddField(string key, Guid value)
	{
		Data.Add(new EventRecordField(key)
		{
			Guid = value
		});
		return this;
	}

	public EventRecord AddField(string key, Vector3 value)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		Data.Add(new EventRecordField(key)
		{
			Vector = value
		});
		return this;
	}

	public EventRecord AddField(string key, BaseEntity entity)
	{
		//IL_0346: Unknown result type (might be due to invalid IL or missing references)
		//IL_0377: Unknown result type (might be due to invalid IL or missing references)
		//IL_037c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0380: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)entity == (Object)null || entity.net == null)
		{
			return this;
		}
		if (entity is BasePlayer basePlayer && !basePlayer.IsNpc && !basePlayer.IsBot)
		{
			string userWipeId = SingletonComponent<ServerMgr>.Instance.persistance.GetUserWipeId(basePlayer.userID);
			Data.Add(new EventRecordField(key, "_userid")
			{
				String = userWipeId
			});
			AddField(key + "_modelstate", basePlayer.modelState.flags);
			if (basePlayer.isMounted)
			{
				AddField(key + "_mounted", (BaseEntity)basePlayer.GetMounted());
			}
			if (basePlayer.IsAdmin || basePlayer.IsDeveloper)
			{
				Data.Add(new EventRecordField(key, "_admin")
				{
					String = "true"
				});
			}
		}
		if (entity is BaseProjectile baseProjectile)
		{
			Item item = baseProjectile.GetItem();
			if (item != null && (item.contents?.itemList?.Count).GetValueOrDefault() > 0)
			{
				List<string> list = Pool.GetList<string>();
				foreach (Item item3 in item.contents.itemList)
				{
					list.Add(item3.info.shortname);
				}
				AddObject(key + "_inventory", list);
				Pool.FreeList<string>(ref list);
			}
		}
		if (entity is DroppedItem droppedItem && droppedItem.DroppedTime != default(DateTime) && droppedItem.DroppedTime >= DateTime.UnixEpoch)
		{
			string userWipeId2 = SingletonComponent<ServerMgr>.Instance.persistance.GetUserWipeId(droppedItem.DroppedBy);
			AddField("dropped_at", ((DateTimeOffset)droppedItem.DroppedTime).ToUnixTimeMilliseconds());
			AddField("dropped_by", userWipeId2);
		}
		if (entity is Door door)
		{
			Data.Add(new EventRecordField(key, "_building_id")
			{
				Number = (int)door.buildingID
			});
		}
		if (entity is CodeLock codeLock && (Object)(object)codeLock.GetParentEntity() != (Object)null && codeLock.GetParentEntity() is DecayEntity entity2)
		{
			AddField("parent", (BaseEntity)entity2);
		}
		if (entity is BuildingBlock buildingBlock)
		{
			Data.Add(new EventRecordField(key, "_grade")
			{
				Number = (long)buildingBlock.grade
			});
			Data.Add(new EventRecordField(key, "_building_id")
			{
				Number = (int)buildingBlock.buildingID
			});
		}
		Data.Add(new EventRecordField(key, "_prefab")
		{
			String = entity.ShortPrefabName
		});
		Data.Add(new EventRecordField(key, "_pos")
		{
			Vector = ((Component)entity).transform.position
		});
		List<EventRecordField> data = Data;
		EventRecordField item2 = new EventRecordField(key, "_rot");
		Quaternion rotation = ((Component)entity).transform.rotation;
		item2.Vector = ((Quaternion)(ref rotation)).eulerAngles;
		data.Add(item2);
		Data.Add(new EventRecordField(key, "_id")
		{
			Number = (long)entity.net.ID.Value
		});
		return this;
	}

	public EventRecord AddField(string key, Item item)
	{
		if (item == null)
		{
			return this;
		}
		Data.Add(new EventRecordField(key, "_name")
		{
			String = item.info.shortname
		});
		Data.Add(new EventRecordField(key, "_amount")
		{
			Number = item.amount
		});
		Data.Add(new EventRecordField(key, "_skin")
		{
			Number = (long)item.skin
		});
		Data.Add(new EventRecordField(key, "_condition")
		{
			Float = item.conditionNormalized
		});
		return this;
	}

	public void MarkSubmitted()
	{
		TimesSubmitted++;
		if (TimesCreated != TimesSubmitted)
		{
			Debug.LogError((object)$"EventRecord pooling error: event has been submitted ({TimesSubmitted}) a different amount of times than it was created ({TimesCreated})");
		}
	}

	public void Submit()
	{
		if (IsServer)
		{
			Analytics.AzureWebInterface.server.EnqueueEvent(this);
		}
	}

	public void SerializeAsCSV(StreamWriter writer)
	{
		if (Data.Count == 0)
		{
			return;
		}
		bool flag = false;
		foreach (EventRecordField datum in Data)
		{
			if (flag)
			{
				writer.Write(',');
			}
			else
			{
				flag = true;
			}
			datum.Serialize(writer);
		}
	}

	public void SerializeAsJson(StreamWriter writer, bool useDataObject = true)
	{
		writer.Write("{\"Timestamp\":\"");
		writer.Write(Timestamp.ToString("o"));
		bool flag = false;
		if (useDataObject)
		{
			writer.Write("\",\"Data\":{");
		}
		else
		{
			writer.Write("\"");
			flag = true;
		}
		foreach (EventRecordField datum in Data)
		{
			if (flag)
			{
				writer.Write(',');
			}
			else
			{
				flag = true;
			}
			writer.Write("\"");
			writer.Write(datum.Key1);
			if (datum.Key2 != null)
			{
				writer.Write(datum.Key2);
			}
			writer.Write("\":");
			if (!datum.IsObject)
			{
				writer.Write('"');
			}
			datum.Serialize(writer);
			if (!datum.IsObject)
			{
				writer.Write("\"");
			}
		}
		if (useDataObject)
		{
			writer.Write('}');
		}
		writer.Write('}');
	}
}
