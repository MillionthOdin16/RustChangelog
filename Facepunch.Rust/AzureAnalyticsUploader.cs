using System;
using System.Collections.Concurrent;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using UnityEngine;

namespace Facepunch.Rust;

public class AzureAnalyticsUploader : IPooled
{
	private ConcurrentQueue<EventRecord> queue = new ConcurrentQueue<EventRecord>();

	private BlobClient _blobClient;

	private Stream Stream;

	private GZipStream ZipStream;

	private StreamWriter Writer;

	private bool readyToFlush;

	private bool invalid;

	public TimeSpan LoopDelay { get; set; }

	public DateTime Expiry { get; private set; }

	public bool StrictMode { get; set; }

	public AnalyticsDocumentMode DocumentMode { get; private set; }

	public bool UseJsonDataObject { get; set; }

	public void EnterPool()
	{
	}

	public void LeavePool()
	{
		LoopDelay = TimeSpan.FromMilliseconds(250.0);
		Expiry = DateTime.MinValue;
		StrictMode = false;
		UseJsonDataObject = false;
		DocumentMode = AnalyticsDocumentMode.JSON;
		EventRecord result;
		while (queue.TryDequeue(out result))
		{
			Pool.Free<EventRecord>(ref result);
		}
		_blobClient = null;
		Stream = null;
		Writer = null;
		readyToFlush = false;
		invalid = false;
	}

	public bool TryFlush()
	{
		if (Expiry < DateTime.UtcNow)
		{
			readyToFlush = true;
			return true;
		}
		return false;
	}

	public static AzureAnalyticsUploader Create(string table, TimeSpan timeout, AnalyticsDocumentMode mode = AnalyticsDocumentMode.JSON)
	{
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Expected O, but got Unknown
		AzureAnalyticsUploader azureAnalyticsUploader = Pool.Get<AzureAnalyticsUploader>();
		azureAnalyticsUploader.Expiry = DateTime.UtcNow + timeout;
		azureAnalyticsUploader.DocumentMode = mode;
		if (string.IsNullOrEmpty(Analytics.BulkUploadConnectionString))
		{
			azureAnalyticsUploader.invalid = true;
			return azureAnalyticsUploader;
		}
		string text = ((mode == AnalyticsDocumentMode.JSON) ? ".json" : ".csv");
		string text2 = Path.Combine(table, Guid.NewGuid().ToString("N") + text + ".gz");
		BlobContainerClient val = new BlobContainerClient(new Uri(Analytics.BulkUploadConnectionString), (BlobClientOptions)null);
		azureAnalyticsUploader._blobClient = val.GetBlobClient(text2);
		Task.Run((Func<Task>)azureAnalyticsUploader.UploadThread);
		return azureAnalyticsUploader;
	}

	public void Append(EventRecord record)
	{
		if (readyToFlush)
		{
			if (StrictMode)
			{
				throw new Exception("Trying to append to a finished uploader: make sure to dispose the uploader properly!");
			}
			record.MarkSubmitted();
			Pool.Free<EventRecord>(ref record);
		}
		else if (invalid)
		{
			record.MarkSubmitted();
			Pool.Free<EventRecord>(ref record);
		}
		else
		{
			queue.Enqueue(record);
		}
	}

	private async Task CreateBlobAsync()
	{
		CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
		cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(10.0));
		BlobOpenWriteOptions val = new BlobOpenWriteOptions
		{
			HttpHeaders = new BlobHttpHeaders
			{
				ContentType = ((DocumentMode == AnalyticsDocumentMode.JSON) ? "application/json" : "text/csv"),
				ContentEncoding = "gzip"
			}
		};
		Stream = await _blobClient.OpenWriteAsync(true, val, cancellationTokenSource.Token);
		ZipStream = new GZipStream(Stream, CompressionLevel.Fastest);
		Writer = new StreamWriter(ZipStream);
	}

	private async Task UploadThread()
	{
		_ = 3;
		try
		{
			while ((Stream == null || Stream.CanWrite) && (!readyToFlush || !queue.IsEmpty))
			{
				if (Stream == null && !queue.IsEmpty)
				{
					await CreateBlobAsync();
				}
				EventRecord record;
				while (queue.TryDequeue(out record))
				{
					if (DocumentMode == AnalyticsDocumentMode.JSON)
					{
						record.SerializeAsJson(Writer, UseJsonDataObject);
					}
					else if (DocumentMode == AnalyticsDocumentMode.CSV)
					{
						record.SerializeAsCSV(Writer);
					}
					await Writer.WriteLineAsync();
					record.MarkSubmitted();
					Pool.Free<EventRecord>(ref record);
				}
				await Task.Delay(LoopDelay);
			}
			if (Writer != null)
			{
				await ((TextWriter)Writer).DisposeAsync();
			}
		}
		catch (Exception ex)
		{
			Debug.LogException(ex);
		}
		finally
		{
			AzureAnalyticsUploader azureAnalyticsUploader = this;
			Pool.Free<AzureAnalyticsUploader>(ref azureAnalyticsUploader);
		}
	}
}
