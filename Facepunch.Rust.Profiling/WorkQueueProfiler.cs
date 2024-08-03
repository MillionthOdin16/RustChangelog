using System;
using ConVar;
using UnityEngine;

namespace Facepunch.Rust.Profiling;

public static class WorkQueueProfiler
{
	public static bool enabled;

	public static void Serialize(AzureAnalyticsUploader uploader, int frameIndex, DateTime timestamp)
	{
		if (!enabled)
		{
			return;
		}
		try
		{
			foreach (ObjectWorkQueue item in ObjectWorkQueue.All)
			{
				if (item.LastProcessedCount != 0)
				{
					EventRecord eventRecord = EventRecord.CSV();
					eventRecord.AddField("", frameIndex).AddField("", timestamp).AddField("", item.Name)
						.AddField("", item.LastQueueLength)
						.AddField("", item.LastExecutionTime)
						.AddField("", item.LastProcessedCount)
						.AddField("", Server.server_id);
					uploader.Append(eventRecord);
				}
			}
		}
		catch (Exception ex)
		{
			Debug.LogError((object)("Failed to serialize work queues: " + ex.Message));
		}
	}

	public static void Reset()
	{
	}
}
