using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using ConVar;
using Facepunch;
using Facepunch.Math;
using Network;
using Newtonsoft.Json;
using ProtoBuf;
using Rust;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Profiling;

public class SaveRestore : SingletonComponent<SaveRestore>
{
	public class SaveExtraData
	{
		public string WipeId;
	}

	public static bool IsSaving = false;

	public static DateTime SaveCreatedTime;

	private static MemoryStream SaveBuffer = new MemoryStream(33554432);

	public static string WipeId { get; private set; }

	public static void ClearMapEntities()
	{
		BaseEntity[] array = Object.FindObjectsOfType<BaseEntity>();
		if (array.Length == 0)
		{
			return;
		}
		DebugEx.Log((object)("Destroying " + array.Length + " old entities"), (StackTraceLogType)0);
		Stopwatch stopwatch = Stopwatch.StartNew();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Kill();
			if (stopwatch.Elapsed.TotalMilliseconds > 2000.0)
			{
				stopwatch.Reset();
				stopwatch.Start();
				DebugEx.Log((object)("\t" + (i + 1) + " / " + array.Length), (StackTraceLogType)0);
			}
		}
		ItemManager.Heartbeat();
		DebugEx.Log((object)"\tdone.", (StackTraceLogType)0);
	}

	public static bool Load(string strFilename = "", bool allowOutOfDateSaves = false)
	{
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_0319: Unknown result type (might be due to invalid IL or missing references)
		//IL_0349: Unknown result type (might be due to invalid IL or missing references)
		//IL_03eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0401: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_042a: Unknown result type (might be due to invalid IL or missing references)
		SaveCreatedTime = DateTime.UtcNow;
		try
		{
			if (strFilename == "")
			{
				strFilename = World.SaveFolderName + "/" + World.SaveFileName;
			}
			if (!File.Exists(strFilename))
			{
				if (!File.Exists("TestSaves/" + strFilename))
				{
					Debug.LogWarning((object)("Couldn't load " + strFilename + " - file doesn't exist"));
					return false;
				}
				strFilename = "TestSaves/" + strFilename;
			}
			Dictionary<BaseEntity, Entity> dictionary = new Dictionary<BaseEntity, Entity>();
			using (FileStream fileStream = File.OpenRead(strFilename))
			{
				using BinaryReader binaryReader = new BinaryReader(fileStream);
				SaveCreatedTime = File.GetCreationTime(strFilename);
				if (binaryReader.ReadSByte() != 83 || binaryReader.ReadSByte() != 65 || binaryReader.ReadSByte() != 86 || binaryReader.ReadSByte() != 82)
				{
					Debug.LogWarning((object)"Invalid save (missing header)");
					return false;
				}
				if (binaryReader.PeekChar() == 74)
				{
					binaryReader.ReadChar();
					string text = binaryReader.ReadString();
					SaveExtraData saveExtraData = JsonConvert.DeserializeObject<SaveExtraData>(text);
					WipeId = saveExtraData.WipeId;
				}
				if (binaryReader.PeekChar() == 68)
				{
					binaryReader.ReadChar();
					SaveCreatedTime = Epoch.ToDateTime((long)binaryReader.ReadInt32());
				}
				if (binaryReader.ReadUInt32() != 239)
				{
					if (allowOutOfDateSaves)
					{
						Debug.LogWarning((object)"This save is from an older (possibly incompatible) version!");
					}
					else
					{
						Debug.LogWarning((object)"This save is from an older version. It might not load properly.");
					}
				}
				ClearMapEntities();
				Assert.IsTrue(BaseEntity.saveList.Count == 0, "BaseEntity.saveList isn't empty!");
				Net.sv.Reset();
				Application.isLoadingSave = true;
				HashSet<NetworkableId> hashSet = new HashSet<NetworkableId>();
				while (fileStream.Position < fileStream.Length)
				{
					RCon.Update();
					uint num = binaryReader.ReadUInt32();
					long position = fileStream.Position;
					Entity entData = null;
					try
					{
						entData = Entity.DeserializeLength((Stream)fileStream, (int)num);
					}
					catch (Exception ex)
					{
						Debug.LogWarning((object)("Skipping entity since it could not be deserialized - stream position: " + position + " size: " + num));
						Debug.LogException(ex);
						fileStream.Position = position + num;
						continue;
					}
					if (entData.basePlayer != null && dictionary.Any((KeyValuePair<BaseEntity, Entity> x) => x.Value.basePlayer != null && x.Value.basePlayer.userid == entData.basePlayer.userid))
					{
						Debug.LogWarning((object)string.Concat("Skipping entity ", entData.baseNetworkable.uid, " - it's a player ", entData.basePlayer.userid, " who is in the save multiple times"));
						continue;
					}
					if (((NetworkableId)(ref entData.baseNetworkable.uid)).IsValid && hashSet.Contains(entData.baseNetworkable.uid))
					{
						Debug.LogWarning((object)string.Concat("Skipping entity ", entData.baseNetworkable.uid, " ", StringPool.Get(entData.baseNetworkable.prefabID), " - uid is used multiple times"));
						continue;
					}
					if (((NetworkableId)(ref entData.baseNetworkable.uid)).IsValid)
					{
						hashSet.Add(entData.baseNetworkable.uid);
					}
					BaseEntity baseEntity = GameManager.server.CreateEntity(StringPool.Get(entData.baseNetworkable.prefabID), entData.baseEntity.pos, Quaternion.Euler(entData.baseEntity.rot));
					if (Object.op_Implicit((Object)(object)baseEntity))
					{
						baseEntity.InitLoad(entData.baseNetworkable.uid);
						dictionary.Add(baseEntity, entData);
					}
				}
			}
			DebugEx.Log((object)("Spawning " + dictionary.Count + " entities"), (StackTraceLogType)0);
			BaseNetworkable.LoadInfo info = default(BaseNetworkable.LoadInfo);
			info.fromDisk = true;
			Stopwatch stopwatch = Stopwatch.StartNew();
			int num2 = 0;
			foreach (KeyValuePair<BaseEntity, Entity> item in dictionary)
			{
				BaseEntity key = item.Key;
				if ((Object)(object)key == (Object)null)
				{
					continue;
				}
				RCon.Update();
				info.msg = item.Value;
				key.Spawn();
				key.Load(info);
				if (key.IsValid())
				{
					num2++;
					if (stopwatch.Elapsed.TotalMilliseconds > 2000.0)
					{
						stopwatch.Reset();
						stopwatch.Start();
						DebugEx.Log((object)("\t" + num2 + " / " + dictionary.Count), (StackTraceLogType)0);
					}
				}
			}
			foreach (KeyValuePair<BaseEntity, Entity> item2 in dictionary)
			{
				BaseEntity key2 = item2.Key;
				if (!((Object)(object)key2 == (Object)null))
				{
					RCon.Update();
					if (key2.IsValid())
					{
						key2.UpdateNetworkGroup();
						key2.PostServerLoad();
					}
				}
			}
			DebugEx.Log((object)"\tdone.", (StackTraceLogType)0);
			if (Object.op_Implicit((Object)(object)SingletonComponent<SpawnHandler>.Instance))
			{
				DebugEx.Log((object)"Enforcing SpawnPopulation Limits", (StackTraceLogType)0);
				SingletonComponent<SpawnHandler>.Instance.EnforceLimits();
				DebugEx.Log((object)"\tdone.", (StackTraceLogType)0);
			}
			InitializeWipeId();
			Application.isLoadingSave = false;
			return true;
		}
		catch (Exception ex2)
		{
			Debug.LogWarning((object)("Error loading save (" + strFilename + ")"));
			Debug.LogException(ex2);
			return false;
		}
	}

	public static void GetSaveCache()
	{
		BaseEntity[] array = BaseEntity.saveList.ToArray();
		if (array.Length == 0)
		{
			return;
		}
		DebugEx.Log((object)("Initializing " + array.Length + " entity save caches"), (StackTraceLogType)0);
		Stopwatch stopwatch = Stopwatch.StartNew();
		for (int i = 0; i < array.Length; i++)
		{
			BaseEntity baseEntity = array[i];
			if (baseEntity.IsValid())
			{
				baseEntity.GetSaveCache();
				if (stopwatch.Elapsed.TotalMilliseconds > 2000.0)
				{
					stopwatch.Reset();
					stopwatch.Start();
					DebugEx.Log((object)("\t" + (i + 1) + " / " + array.Length), (StackTraceLogType)0);
				}
			}
		}
		DebugEx.Log((object)"\tdone.", (StackTraceLogType)0);
	}

	public static void InitializeEntityLinks()
	{
		BaseEntity[] array = (from x in BaseNetworkable.serverEntities
			where x is BaseEntity
			select x as BaseEntity).ToArray();
		if (array.Length == 0)
		{
			return;
		}
		DebugEx.Log((object)("Initializing " + array.Length + " entity links"), (StackTraceLogType)0);
		Stopwatch stopwatch = Stopwatch.StartNew();
		for (int i = 0; i < array.Length; i++)
		{
			RCon.Update();
			array[i].RefreshEntityLinks();
			if (stopwatch.Elapsed.TotalMilliseconds > 2000.0)
			{
				stopwatch.Reset();
				stopwatch.Start();
				DebugEx.Log((object)("\t" + (i + 1) + " / " + array.Length), (StackTraceLogType)0);
			}
		}
		DebugEx.Log((object)"\tdone.", (StackTraceLogType)0);
	}

	public static void InitializeEntitySupports()
	{
		if (!ConVar.Server.stability)
		{
			return;
		}
		StabilityEntity[] array = (from x in BaseNetworkable.serverEntities
			where x is StabilityEntity
			select x as StabilityEntity).ToArray();
		if (array.Length == 0)
		{
			return;
		}
		DebugEx.Log((object)("Initializing " + array.Length + " stability supports"), (StackTraceLogType)0);
		Stopwatch stopwatch = Stopwatch.StartNew();
		for (int i = 0; i < array.Length; i++)
		{
			RCon.Update();
			array[i].InitializeSupports();
			if (stopwatch.Elapsed.TotalMilliseconds > 2000.0)
			{
				stopwatch.Reset();
				stopwatch.Start();
				DebugEx.Log((object)("\t" + (i + 1) + " / " + array.Length), (StackTraceLogType)0);
			}
		}
		DebugEx.Log((object)"\tdone.", (StackTraceLogType)0);
	}

	public static void InitializeEntityConditionals()
	{
		BuildingBlock[] array = (from x in BaseNetworkable.serverEntities
			where x is BuildingBlock
			select x as BuildingBlock).ToArray();
		if (array.Length == 0)
		{
			return;
		}
		DebugEx.Log((object)("Initializing " + array.Length + " conditional models"), (StackTraceLogType)0);
		Stopwatch stopwatch = Stopwatch.StartNew();
		for (int i = 0; i < array.Length; i++)
		{
			RCon.Update();
			array[i].UpdateSkin(force: true);
			if (stopwatch.Elapsed.TotalMilliseconds > 2000.0)
			{
				stopwatch.Reset();
				stopwatch.Start();
				DebugEx.Log((object)("\t" + (i + 1) + " / " + array.Length), (StackTraceLogType)0);
			}
		}
		DebugEx.Log((object)"\tdone.", (StackTraceLogType)0);
	}

	public static void InitializeWipeId()
	{
		if (WipeId == null)
		{
			WipeId = Guid.NewGuid().ToString("N");
		}
	}

	public static IEnumerator Save(string strFilename, bool AndWait = false)
	{
		if (Application.isQuitting)
		{
			yield break;
		}
		Stopwatch timerCache = new Stopwatch();
		Stopwatch timerWrite = new Stopwatch();
		Stopwatch timerDisk = new Stopwatch();
		int iEnts = 0;
		timerCache.Start();
		TimeWarning val = TimeWarning.New("SaveCache", 100);
		try
		{
			Stopwatch sw = Stopwatch.StartNew();
			BaseEntity[] array = BaseEntity.saveList.ToArray();
			foreach (BaseEntity entity in array)
			{
				if ((Object)(object)entity == (Object)null || !entity.IsValid())
				{
					continue;
				}
				try
				{
					entity.GetSaveCache();
				}
				catch (Exception ex)
				{
					Exception e = ex;
					Debug.LogException(e);
				}
				if (sw.Elapsed.TotalMilliseconds > 5.0)
				{
					if (!AndWait)
					{
						yield return CoroutineEx.waitForEndOfFrame;
					}
					sw.Reset();
					sw.Start();
				}
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		timerCache.Stop();
		SaveBuffer.Position = 0L;
		SaveBuffer.SetLength(0L);
		timerWrite.Start();
		TimeWarning val2 = TimeWarning.New("SaveWrite", 100);
		try
		{
			BinaryWriter writer = new BinaryWriter(SaveBuffer);
			writer.Write((sbyte)83);
			writer.Write((sbyte)65);
			writer.Write((sbyte)86);
			writer.Write((sbyte)82);
			InitializeWipeId();
			SaveExtraData extraData = new SaveExtraData
			{
				WipeId = WipeId
			};
			writer.Write((sbyte)74);
			writer.Write(JsonConvert.SerializeObject((object)extraData));
			writer.Write((sbyte)68);
			writer.Write(Epoch.FromDateTime(SaveCreatedTime));
			writer.Write(239u);
			BaseNetworkable.SaveInfo saveinfo = default(BaseNetworkable.SaveInfo);
			saveinfo.forDisk = true;
			if (!AndWait)
			{
				yield return CoroutineEx.waitForEndOfFrame;
			}
			foreach (BaseEntity entity2 in BaseEntity.saveList)
			{
				if ((Object)(object)entity2 == (Object)null || entity2.IsDestroyed)
				{
					Debug.LogWarning((object)("Entity is NULL but is still in saveList - not destroyed properly? " + entity2), (Object)(object)entity2);
					continue;
				}
				Profiler.BeginSample("entity.Save");
				MemoryStream sc = null;
				try
				{
					sc = entity2.GetSaveCache();
				}
				catch (Exception ex)
				{
					Exception e4 = ex;
					Debug.LogException(e4);
				}
				if (sc == null || sc.Length <= 0)
				{
					Debug.LogWarningFormat("Skipping saving entity {0} - because {1}", new object[2]
					{
						entity2,
						(sc == null) ? "savecache is null" : "savecache is 0"
					});
				}
				else
				{
					writer.Write((uint)sc.Length);
					writer.Write(sc.GetBuffer(), 0, (int)sc.Length);
					Profiler.EndSample();
					iEnts++;
				}
			}
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
		timerWrite.Stop();
		if (!AndWait)
		{
			yield return CoroutineEx.waitForEndOfFrame;
		}
		timerDisk.Start();
		TimeWarning val3 = TimeWarning.New("SaveBackup", 100);
		try
		{
			ShiftSaveBackups(strFilename);
		}
		finally
		{
			((IDisposable)val3)?.Dispose();
		}
		TimeWarning val4 = TimeWarning.New("SaveDisk", 100);
		try
		{
			string newSaveFile = strFilename + ".new";
			if (File.Exists(newSaveFile))
			{
				File.Delete(newSaveFile);
			}
			try
			{
				using FileStream fileStream = File.OpenWrite(newSaveFile);
				SaveBuffer.Position = 0L;
				SaveBuffer.CopyTo(fileStream);
			}
			catch (Exception e3)
			{
				Debug.LogError((object)("Couldn't write save file! We got an exception: " + e3));
				if (File.Exists(newSaveFile))
				{
					File.Delete(newSaveFile);
				}
				yield break;
			}
			File.Copy(newSaveFile, strFilename, overwrite: true);
			File.Delete(newSaveFile);
		}
		catch (Exception ex)
		{
			Exception e2 = ex;
			Debug.LogError((object)("Error when saving to disk: " + e2));
			yield break;
		}
		finally
		{
			((IDisposable)val4)?.Dispose();
		}
		timerDisk.Stop();
		Debug.LogFormat("Saved {0} ents, cache({1}), write({2}), disk({3}).", new object[4]
		{
			iEnts.ToString("N0"),
			timerCache.Elapsed.TotalSeconds.ToString("0.00"),
			timerWrite.Elapsed.TotalSeconds.ToString("0.00"),
			timerDisk.Elapsed.TotalSeconds.ToString("0.00")
		});
	}

	private static void ShiftSaveBackups(string fileName)
	{
		int num = Mathf.Max(ConVar.Server.saveBackupCount, 2);
		if (!File.Exists(fileName))
		{
			return;
		}
		try
		{
			int num2 = 0;
			for (int j = 1; j <= num; j++)
			{
				string path = fileName + "." + j;
				if (!File.Exists(path))
				{
					break;
				}
				num2++;
			}
			string text = GetBackupName(num2 + 1);
			for (int num3 = num2; num3 > 0; num3--)
			{
				string text2 = GetBackupName(num3);
				if (num3 == num)
				{
					File.Delete(text2);
				}
				else if (File.Exists(text2))
				{
					if (File.Exists(text))
					{
						File.Delete(text);
					}
					File.Move(text2, text);
				}
				text = text2;
			}
			File.Copy(fileName, text, overwrite: true);
		}
		catch (Exception ex)
		{
			Debug.LogError((object)("Error while backing up old saves: " + ex.Message));
			Debug.LogException(ex);
			throw;
		}
		string GetBackupName(int i)
		{
			return $"{fileName}.{i}";
		}
	}

	private void Start()
	{
		((MonoBehaviour)this).StartCoroutine(SaveRegularly());
	}

	private IEnumerator SaveRegularly()
	{
		while (true)
		{
			yield return CoroutineEx.waitForSeconds(ConVar.Server.saveinterval);
			yield return ((MonoBehaviour)this).StartCoroutine(DoAutomatedSave());
		}
	}

	private IEnumerator DoAutomatedSave(bool AndWait = false)
	{
		IsSaving = true;
		string folder = ConVar.Server.rootFolder;
		if (!AndWait)
		{
			yield return CoroutineEx.waitForEndOfFrame;
		}
		if (AndWait)
		{
			IEnumerator e = Save(folder + "/" + World.SaveFileName, AndWait);
			while (e.MoveNext())
			{
			}
		}
		else
		{
			yield return ((MonoBehaviour)this).StartCoroutine(Save(folder + "/" + World.SaveFileName, AndWait));
		}
		if (!AndWait)
		{
			yield return CoroutineEx.waitForEndOfFrame;
		}
		Debug.Log((object)"Saving complete");
		IsSaving = false;
	}

	public static bool Save(bool AndWait)
	{
		if ((Object)(object)SingletonComponent<SaveRestore>.Instance == (Object)null)
		{
			return false;
		}
		if (IsSaving)
		{
			return false;
		}
		IEnumerator enumerator = SingletonComponent<SaveRestore>.Instance.DoAutomatedSave(AndWait: true);
		while (enumerator.MoveNext())
		{
		}
		return true;
	}
}
