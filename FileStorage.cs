using System;
using System.Collections.Generic;
using System.Linq;
using ConVar;
using Facepunch.Sqlite;
using Ionic.Crc;
using UnityEngine.Assertions;

public class FileStorage : IDisposable
{
	private class CacheData
	{
		public byte[] data;

		public NetworkableId entityID;

		public uint numID;
	}

	public enum Type
	{
		png,
		jpg,
		ogg
	}

	private Database db;

	private CRC32 crc = new CRC32();

	private MruDictionary<uint, CacheData> _cache = new MruDictionary<uint, CacheData>(1000, (Action<uint, CacheData>)null);

	public static FileStorage server = new FileStorage("sv.files." + 239, server: true);

	protected FileStorage(string name, bool server)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Expected O, but got Unknown
		if (server)
		{
			string rootFolder = Server.rootFolder;
			string text = rootFolder + "/" + name + ".db";
			db = new Database();
			db.Open(text, true);
			if (!db.TableExists("data"))
			{
				db.Execute("CREATE TABLE data ( crc INTEGER PRIMARY KEY, data BLOB, updated INTEGER, entid INTEGER, filetype INTEGER, part INTEGER )");
				db.Execute("CREATE INDEX IF NOT EXISTS entindex ON data ( entid )");
			}
		}
	}

	~FileStorage()
	{
		Dispose();
	}

	public void Dispose()
	{
		if (db != null)
		{
			db.Close();
			db = null;
		}
	}

	private uint GetCRC(byte[] data, Type type)
	{
		TimeWarning val = TimeWarning.New("FileStorage.GetCRC", 0);
		try
		{
			crc.Reset();
			crc.SlurpBlock(data, 0, data.Length);
			crc.UpdateCRC((byte)type);
			return (uint)crc.Crc32Result;
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public uint Store(byte[] data, Type type, NetworkableId entityID, uint numID = 0u)
	{
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		TimeWarning val = TimeWarning.New("FileStorage.Store", 0);
		try
		{
			uint cRC = GetCRC(data, type);
			if (db != null)
			{
				db.Execute<int, byte[], long, int, int>("INSERT OR REPLACE INTO data ( crc, data, entid, filetype, part ) VALUES ( ?, ?, ?, ?, ? )", (int)cRC, data, (long)entityID.Value, (int)type, (int)numID);
			}
			_cache.Remove(cRC);
			_cache.Add(cRC, new CacheData
			{
				data = data,
				entityID = entityID,
				numID = numID
			});
			return cRC;
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public byte[] Get(uint crc, Type type, NetworkableId entityID, uint numID = 0u)
	{
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		TimeWarning val = TimeWarning.New("FileStorage.Get", 0);
		try
		{
			CacheData cacheData = default(CacheData);
			if (_cache.TryGetValue(crc, ref cacheData))
			{
				Assert.IsTrue(cacheData.data != null, "FileStorage cache contains a null texture");
				return cacheData.data;
			}
			if (db == null)
			{
				return null;
			}
			byte[] array = db.QueryBlob<int, int, long, int>("SELECT data FROM data WHERE crc = ? AND filetype = ? AND entid = ? AND part = ? LIMIT 1", (int)crc, (int)type, (long)entityID.Value, (int)numID);
			if (array == null)
			{
				return null;
			}
			_cache.Remove(crc);
			_cache.Add(crc, new CacheData
			{
				data = array,
				entityID = entityID,
				numID = 0u
			});
			return array;
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void Remove(uint crc, Type type, NetworkableId entityID)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		TimeWarning val = TimeWarning.New("FileStorage.Remove", 0);
		try
		{
			if (db != null)
			{
				db.Execute<int, int, long>("DELETE FROM data WHERE crc = ? AND filetype = ? AND entid = ?", (int)crc, (int)type, (long)entityID.Value);
			}
			_cache.Remove(crc);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void RemoveExact(uint crc, Type type, NetworkableId entityID, uint numid)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		TimeWarning val = TimeWarning.New("FileStorage.RemoveExact", 0);
		try
		{
			if (db != null)
			{
				db.Execute<int, int, long, int>("DELETE FROM data WHERE crc = ? AND filetype = ? AND entid = ? AND part = ?", (int)crc, (int)type, (long)entityID.Value, (int)numid);
			}
			_cache.Remove(crc);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void RemoveEntityNum(NetworkableId entityid, uint numid)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		TimeWarning val = TimeWarning.New("FileStorage.RemoveEntityNum", 0);
		try
		{
			if (db != null)
			{
				db.Execute<long, int>("DELETE FROM data WHERE entid = ? AND part = ?", (long)entityid.Value, (int)numid);
			}
			uint[] array = (from x in (IEnumerable<KeyValuePair<uint, CacheData>>)_cache
				where x.Value.entityID == entityid && x.Value.numID == numid
				select x.Key).ToArray();
			foreach (uint num in array)
			{
				_cache.Remove(num);
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	internal void RemoveAllByEntity(NetworkableId entityid)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		TimeWarning val = TimeWarning.New("FileStorage.RemoveAllByEntity", 0);
		try
		{
			if (db != null)
			{
				db.Execute<long>("DELETE FROM data WHERE entid = ?", (long)entityid.Value);
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void ReassignEntityId(NetworkableId oldId, NetworkableId newId)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		TimeWarning val = TimeWarning.New("FileStorage.ReassignEntityId", 0);
		try
		{
			if (db != null)
			{
				db.Execute<long, long>("UPDATE data SET entid = ? WHERE entid = ?", (long)newId.Value, (long)oldId.Value);
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}
}
