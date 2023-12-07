using System;
using System.Collections.Generic;
using Facepunch;
using Facepunch.Math;
using Facepunch.Nexus;
using Facepunch.Rust;
using Facepunch.Sqlite;
using ProtoBuf;
using UnityEngine;

public class UserPersistance : IDisposable
{
	private static Database blueprints;

	private static Database deaths;

	private static Database identities;

	private static Database tokens;

	private static Database playerState;

	private static Dictionary<ulong, string> nameCache;

	private static Dictionary<ulong, string> wipeIdCache;

	private static MruDictionary<ulong, (int Token, bool Locked)> tokenCache;

	public UserPersistance(string strFolder)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Expected O, but got Unknown
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Expected O, but got Unknown
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Expected O, but got Unknown
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Expected O, but got Unknown
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Expected O, but got Unknown
		blueprints = new Database();
		BaseGameMode activeGameMode = BaseGameMode.GetActiveGameMode(serverside: true);
		string text = strFolder + "/player.blueprints.";
		if ((Object)(object)activeGameMode != (Object)null && activeGameMode.wipeBpsOnProtocol)
		{
			text = text + 244 + ".";
		}
		blueprints.Open(text + 5 + ".db", false);
		if (!blueprints.TableExists("data"))
		{
			blueprints.Execute("CREATE TABLE data ( userid TEXT PRIMARY KEY, info BLOB, updated INTEGER )");
		}
		deaths = new Database();
		deaths.Open(strFolder + "/player.deaths." + 5 + ".db", false);
		if (!deaths.TableExists("data"))
		{
			deaths.Execute("CREATE TABLE data ( userid TEXT, born INTEGER, died INTEGER, info BLOB )");
			deaths.Execute("CREATE INDEX IF NOT EXISTS userindex ON data ( userid )");
			deaths.Execute("CREATE INDEX IF NOT EXISTS diedindex ON data ( died )");
		}
		identities = new Database();
		identities.Open(strFolder + "/player.identities." + 5 + ".db", false);
		if (!identities.TableExists("data"))
		{
			identities.Execute("CREATE TABLE data ( userid INT PRIMARY KEY, username TEXT )");
		}
		tokens = new Database();
		tokens.Open(strFolder + "/player.tokens.db", false);
		if (!tokens.TableExists("data"))
		{
			tokens.Execute("CREATE TABLE data ( userid INT PRIMARY KEY, token INT, locked BOOLEAN DEFAULT 0 )");
		}
		if (!tokens.ColumnExists("data", "locked"))
		{
			tokens.Execute("ALTER TABLE data ADD COLUMN locked BOOLEAN DEFAULT 0");
		}
		playerState = new Database();
		playerState.Open(strFolder + "/player.states." + 244 + ".db", false);
		if (!playerState.TableExists("data"))
		{
			playerState.Execute("CREATE TABLE data ( userid INT PRIMARY KEY, state BLOB )");
		}
		nameCache = new Dictionary<ulong, string>();
		tokenCache = new MruDictionary<ulong, (int, bool)>(500, (Action<ulong, (int, bool)>)null);
		wipeIdCache = new Dictionary<ulong, string>();
	}

	public virtual void Dispose()
	{
		if (blueprints != null)
		{
			blueprints.Close();
			blueprints = null;
		}
		if (deaths != null)
		{
			deaths.Close();
			deaths = null;
		}
		if (identities != null)
		{
			identities.Close();
			identities = null;
		}
		if (tokens != null)
		{
			tokens.Close();
			tokens = null;
		}
		if (playerState != null)
		{
			playerState.Close();
			playerState = null;
		}
	}

	public PersistantPlayer GetPlayerInfo(ulong playerID)
	{
		PersistantPlayer val = FetchFromDatabase(playerID);
		if (val == null)
		{
			val = Pool.Get<PersistantPlayer>();
		}
		if (val.unlockedItems == null)
		{
			val.unlockedItems = Pool.GetList<int>();
		}
		return val;
	}

	private PersistantPlayer FetchFromDatabase(ulong playerID)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			byte[] array = null;
			NexusPlayer player;
			Variable val = default(Variable);
			if (!NexusServer.Started)
			{
				array = blueprints.Query<byte[], ulong>("SELECT info FROM data WHERE userid = ?", playerID);
			}
			else if (NexusServer.TryGetPlayer(playerID, out player) && player.TryGetVariable(NexusVariables.Blueprints, ref val) && (int)val.Type == 0)
			{
				array = val.GetAsBinary();
			}
			if (array != null)
			{
				return PersistantPlayer.Deserialize(array);
			}
		}
		catch (Exception ex)
		{
			Debug.LogError((object)("Error loading player blueprints: (" + ex.Message + ")"));
		}
		return null;
	}

	public void SetPlayerInfo(ulong playerID, PersistantPlayer info)
	{
		TimeWarning val = TimeWarning.New("SetPlayerInfo", 0);
		try
		{
			TimeWarning val2 = TimeWarning.New("ToProtoBytes", 0);
			byte[] array;
			try
			{
				array = info.ToProtoBytes();
			}
			finally
			{
				((IDisposable)val2)?.Dispose();
			}
			NexusPlayer player;
			if (!NexusServer.Started)
			{
				blueprints.Execute<ulong, byte[], int>("INSERT OR REPLACE INTO data ( userid, info, updated ) VALUES ( ?, ?, ? )", playerID, array, Epoch.Current);
			}
			else if (!NexusServer.TryGetPlayer(playerID, out player))
			{
				Debug.LogError((object)$"Couldn't find NexusPlayer to save player info! {playerID}");
			}
			else
			{
				player.SetVariable(NexusVariables.Blueprints, array, false, true);
			}
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void AddLifeStory(ulong playerID, PlayerLifeStory lifeStory)
	{
		if (deaths == null || lifeStory == null)
		{
			return;
		}
		TimeWarning val = TimeWarning.New("AddLifeStory", 0);
		try
		{
			TimeWarning val2 = TimeWarning.New("ToProtoBytes", 0);
			byte[] array;
			try
			{
				array = lifeStory.ToProtoBytes();
			}
			finally
			{
				((IDisposable)val2)?.Dispose();
			}
			deaths.Execute<ulong, int, int, byte[]>("INSERT INTO data ( userid, born, died, info ) VALUES ( ?, ?, ?, ? )", playerID, (int)lifeStory.timeBorn, (int)lifeStory.timeDied, array);
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public PlayerLifeStory GetLastLifeStory(ulong playerID)
	{
		if (deaths == null)
		{
			return null;
		}
		TimeWarning val = TimeWarning.New("GetLastLifeStory", 0);
		try
		{
			try
			{
				byte[] array = deaths.Query<byte[], ulong>("SELECT info FROM data WHERE userid = ? ORDER BY died DESC LIMIT 1", playerID);
				if (array == null)
				{
					return null;
				}
				PlayerLifeStory obj = PlayerLifeStory.Deserialize(array);
				obj.ShouldPool = false;
				return obj;
			}
			catch (Exception ex)
			{
				Debug.LogError((object)("Error loading lifestory from database: (" + ex.Message + ")"));
			}
			return null;
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public string GetPlayerName(ulong playerID)
	{
		if (playerID == 0L)
		{
			return null;
		}
		if (nameCache.TryGetValue(playerID, out var value))
		{
			return value;
		}
		string text = identities.Query<string, ulong>("SELECT username FROM data WHERE userid = ?", playerID);
		nameCache[playerID] = text;
		return text;
	}

	public void SetPlayerName(ulong playerID, string name)
	{
		if (playerID != 0L && !string.IsNullOrEmpty(name))
		{
			if (string.IsNullOrEmpty(GetPlayerName(playerID)))
			{
				identities.Execute<ulong, string>("INSERT INTO data ( userid, username ) VALUES ( ?, ? )", playerID, name);
			}
			else
			{
				identities.Execute<string, ulong>("UPDATE data SET username = ? WHERE userid = ?", name, playerID);
			}
			nameCache[playerID] = name;
		}
	}

	public int GetOrGenerateAppToken(ulong playerID, out bool locked)
	{
		if (tokens == null)
		{
			locked = false;
			return 0;
		}
		TimeWarning val = TimeWarning.New("GetOrGenerateAppToken", 0);
		try
		{
			(int, bool) tuple = default((int, bool));
			if (tokenCache.TryGetValue(playerID, ref tuple))
			{
				locked = tuple.Item2;
				return tuple.Item1;
			}
			int num = tokens.Query<int, ulong>("SELECT token FROM data WHERE userid = ?", playerID);
			if (num != 0)
			{
				bool flag = tokens.Query<int, ulong>("SELECT locked FROM data WHERE userid = ?", playerID) != 0;
				tokenCache.Add(playerID, (num, flag));
				locked = flag;
				return num;
			}
			int num2 = GenerateAppToken();
			tokens.Execute<ulong, int>("INSERT INTO data ( userid, token ) VALUES ( ?, ? )", playerID, num2);
			tokenCache.Add(playerID, (num2, false));
			locked = false;
			return num2;
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	public void RegenerateAppToken(ulong playerID)
	{
		if (tokens == null)
		{
			return;
		}
		TimeWarning val = TimeWarning.New("RegenerateAppToken", 0);
		try
		{
			tokenCache.Remove(playerID);
			bool flag = tokens.Query<int, ulong>("SELECT locked FROM data WHERE userid = ?", playerID) != 0;
			int num = GenerateAppToken();
			tokens.Execute<ulong, int, bool>("INSERT OR REPLACE INTO data ( userid, token, locked ) VALUES ( ?, ?, ? )", playerID, num, flag);
			tokenCache.Add(playerID, (num, false));
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
	}

	private static int GenerateAppToken()
	{
		int num = Random.Range(int.MinValue, int.MaxValue);
		if (num == 0)
		{
			num++;
		}
		return num;
	}

	public bool SetAppTokenLocked(ulong playerID, bool locked)
	{
		if (tokens == null)
		{
			return false;
		}
		GetOrGenerateAppToken(playerID, out var locked2);
		if (locked2 == locked)
		{
			return false;
		}
		tokens.Execute<int, ulong>("UPDATE data SET locked = ? WHERE userid = ?", locked ? 1 : 0, playerID);
		tokenCache.Remove(playerID);
		return true;
	}

	public byte[] GetPlayerState(ulong playerID)
	{
		if (playerID == 0L)
		{
			return null;
		}
		return playerState.Query<byte[], ulong>("SELECT state FROM data WHERE userid = ?", playerID);
	}

	public void SetPlayerState(ulong playerID, byte[] state)
	{
		if (playerID != 0L && state != null)
		{
			playerState.Execute<ulong, byte[]>("INSERT OR REPLACE INTO data ( userid, state ) VALUES ( ?, ? )", playerID, state);
		}
	}

	public string GetUserWipeId(ulong playerID)
	{
		if (playerID <= 10000000)
		{
			return null;
		}
		if (wipeIdCache.TryGetValue(playerID, out var value))
		{
			return value;
		}
		value = StringEx.HexString(StringEx.Sha256(playerID + SaveRestore.WipeId));
		wipeIdCache[playerID] = value;
		Analytics.Azure.OnPlayerInitializedWipeId(playerID, value);
		return value;
	}

	public void ResetPlayerState(ulong playerID)
	{
		if (playerID != 0L)
		{
			playerState.Execute<ulong>("DELETE FROM data WHERE userid = ?", playerID);
		}
	}
}
