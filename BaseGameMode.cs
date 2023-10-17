using System;
using System.Collections.Generic;
using System.Linq;
using CompanionServer;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;

[Serializable]
public class BaseGameMode : BaseEntity
{
	public struct ResearchCostResult
	{
		public float? Scale;

		public int? Amount;
	}

	[Serializable]
	public class GameModeTeam
	{
		public string name;

		public PlayerInventoryProperties[] teamloadouts;
	}

	private GameMode gameModeScores = null;

	public string[] scoreColumns;

	[Header("Vanilla")]
	public bool globalChat = true;

	public bool localChat = false;

	public bool teamSystem = true;

	public bool safeZone = true;

	public bool ingameMap = true;

	public bool compass = true;

	public bool contactSystem = true;

	public bool crawling = true;

	public bool rustPlus = true;

	public bool wipeBpsOnProtocol = false;

	public int maximumSleepingBags = -1;

	public bool returnValidCombatlog = true;

	public bool missionSystem = true;

	public bool mlrs = true;

	public const Flags Flag_Warmup = Flags.Reserved1;

	public const Flags Flag_GameOver = Flags.Reserved2;

	public const Flags Flag_WaitingForPlayers = Flags.Reserved3;

	[Header("Changelog")]
	public Phrase[] addedFeatures;

	public Phrase[] removedFeatures;

	public Phrase[] changedFeatures;

	public List<string> convars = new List<string>();

	public string shortname = "vanilla";

	public float matchDuration = -1f;

	public float warmupDuration = 10f;

	public float timeBetweenMatches = 10f;

	public int minPlayersToStart = 1;

	public bool useCustomSpawns = true;

	public string victoryScoreName = "kills";

	public string teamScoreName = "kills";

	public int numScoreForVictory = 10;

	public string gamemodeTitle;

	public SoundDefinition[] warmupMusics;

	public SoundDefinition[] lossMusics;

	public SoundDefinition[] winMusics;

	[NonSerialized]
	private float warmupStartTime;

	[NonSerialized]
	private float matchStartTime = -1f;

	[NonSerialized]
	private float matchEndTime;

	public List<string> gameModeTags;

	public BasePlayer.CameraMode deathCameraMode = BasePlayer.CameraMode.Eyes;

	public bool permanent = true;

	public bool limitTeamAuths = false;

	public bool allowSleeping = true;

	public bool allowWounding = true;

	public bool allowBleeding = true;

	public bool allowTemperature = true;

	public bool quickRespawn = false;

	public bool quickDeploy = false;

	public float respawnDelayOverride = 5f;

	public float startHealthOverride = 0f;

	public float autoHealDelay = 0f;

	public float autoHealDuration = 1f;

	public bool hasKillFeed = false;

	public bool allowPings = true;

	public static BaseGameMode svActiveGameMode = null;

	public static List<BaseGameMode> svGameModeManifest = new List<BaseGameMode>();

	[NonSerialized]
	private GameObject[] allspawns = null;

	[NonSerialized]
	private GameModeSpawnGroup[] gameModeSpawnGroups = null;

	public PlayerInventoryProperties[] loadouts;

	[Tooltip("Use steamID to always pick the same loadout per player")]
	public bool useStaticLoadoutPerPlayer = false;

	public bool topUpMagazines = false;

	public bool sendKillNotifications = false;

	public GameModeTeam[] teams;

	public float corpseRemovalTimeOverride = 0f;

	private static bool isResetting = false;

	public static event Action<BaseGameMode> GameModeChanged;

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		TimeWarning val = TimeWarning.New("BaseGameMode.OnRpcMessage", 0);
		try
		{
		}
		finally
		{
			((IDisposable)val)?.Dispose();
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	public GameMode GetGameScores()
	{
		return gameModeScores;
	}

	public int ScoreColumnIndex(string scoreName)
	{
		for (int i = 0; i < scoreColumns.Length; i++)
		{
			if (scoreColumns[i] == scoreName)
			{
				return i;
			}
		}
		return -1;
	}

	public void InitScores()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Expected O, but got Unknown
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Expected O, but got Unknown
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Expected O, but got Unknown
		gameModeScores = new GameMode();
		gameModeScores.scoreColumns = new List<ScoreColumn>();
		gameModeScores.playerScores = new List<PlayerScore>();
		gameModeScores.teams = new List<TeamInfo>();
		GameModeTeam[] array = teams;
		foreach (GameModeTeam gameModeTeam in array)
		{
			TeamInfo val = new TeamInfo();
			val.score = 0;
			val.ShouldPool = false;
			gameModeScores.teams.Add(val);
		}
		string[] array2 = scoreColumns;
		foreach (string name in array2)
		{
			ScoreColumn val2 = new ScoreColumn();
			val2.name = name;
			val2.ShouldPool = false;
			gameModeScores.scoreColumns.Add(val2);
		}
		gameModeScores.ShouldPool = false;
	}

	public void CopyGameModeScores(GameMode from, GameMode to)
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Expected O, but got Unknown
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Expected O, but got Unknown
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Expected O, but got Unknown
		to.teams.Clear();
		to.scoreColumns.Clear();
		to.playerScores.Clear();
		foreach (TeamInfo team in from.teams)
		{
			TeamInfo val = new TeamInfo();
			val.score = team.score;
			to.teams.Add(val);
		}
		foreach (ScoreColumn scoreColumn in from.scoreColumns)
		{
			ScoreColumn val2 = new ScoreColumn();
			val2.name = scoreColumn.name;
			to.scoreColumns.Add(val2);
		}
		foreach (PlayerScore playerScore in from.playerScores)
		{
			PlayerScore val3 = new PlayerScore();
			val3.playerName = playerScore.playerName;
			val3.userid = playerScore.userid;
			val3.team = playerScore.team;
			val3.scores = new List<int>();
			foreach (int score in playerScore.scores)
			{
				val3.scores.Add(score);
			}
			to.playerScores.Add(val3);
		}
	}

	public PlayerScore GetPlayerScoreForPlayer(BasePlayer player)
	{
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Expected O, but got Unknown
		PlayerScore val = null;
		foreach (PlayerScore playerScore in gameModeScores.playerScores)
		{
			if (playerScore.userid == player.userID)
			{
				val = playerScore;
				break;
			}
		}
		if (val == null)
		{
			val = new PlayerScore();
			val.ShouldPool = false;
			val.playerName = player.displayName;
			val.userid = player.userID;
			val.scores = new List<int>();
			string[] array = scoreColumns;
			foreach (string text in array)
			{
				val.scores.Add(0);
			}
			gameModeScores.playerScores.Add(val);
		}
		return val;
	}

	public int GetScoreIndexByName(string name)
	{
		for (int i = 0; i < scoreColumns.Length; i++)
		{
			if (scoreColumns[i] == name)
			{
				return i;
			}
		}
		Debug.LogWarning((object)("No score colum named : " + name + "returning default"));
		return 0;
	}

	public virtual bool IsDraw()
	{
		if (IsTeamGame())
		{
			int num = -1;
			int num2 = 1000000;
			for (int i = 0; i < teams.Length; i++)
			{
				int teamScore = GetTeamScore(i);
				if (teamScore < num2)
				{
					num2 = teamScore;
				}
				if (teamScore > num)
				{
					num = teamScore;
				}
			}
			if (num == num2)
			{
				return true;
			}
			return false;
		}
		int num3 = -1;
		int num4 = -1;
		int num5 = 0;
		int num6 = ScoreColumnIndex(victoryScoreName);
		if (num6 != -1)
		{
			for (int j = 0; j < gameModeScores.playerScores.Count; j++)
			{
				PlayerScore val = gameModeScores.playerScores[j];
				if (val.scores[num6] > num3)
				{
					num3 = val.scores[num6];
					num4 = j;
					num5 = 1;
				}
				else if (val.scores[num6] == num3)
				{
					num4 = j;
					num5++;
				}
			}
		}
		return num3 == 0 || num5 > 1;
	}

	public virtual string GetWinnerName()
	{
		int num = -1;
		int num2 = -1;
		if (IsTeamGame())
		{
			for (int i = 0; i < teams.Length; i++)
			{
				int teamScore = GetTeamScore(i);
				if (teamScore > num)
				{
					num = teamScore;
					num2 = i;
				}
			}
			if (num2 == -1)
			{
				return "NO ONE";
			}
			return teams[num2].name;
		}
		int num3 = ScoreColumnIndex(victoryScoreName);
		if (num3 != -1)
		{
			for (int j = 0; j < gameModeScores.playerScores.Count; j++)
			{
				PlayerScore val = gameModeScores.playerScores[j];
				if (val.scores[num3] > num)
				{
					num = val.scores[num3];
					num2 = j;
				}
			}
		}
		if (num2 != -1)
		{
			return gameModeScores.playerScores[num2].playerName;
		}
		return "";
	}

	public virtual int GetPlayerTeamPosition(BasePlayer player)
	{
		return 0;
	}

	public virtual int GetPlayerRank(BasePlayer player)
	{
		int num = ScoreColumnIndex(victoryScoreName);
		if (num == -1)
		{
			return 10;
		}
		int num2 = GetPlayerScoreForPlayer(player).scores[num];
		int num3 = 0;
		foreach (PlayerScore playerScore in gameModeScores.playerScores)
		{
			if (playerScore.scores[num] > num2 && playerScore.userid != player.userID)
			{
				num3++;
			}
		}
		return num3 + 1;
	}

	public int GetWinningTeamIndex()
	{
		int num = -1;
		int num2 = -1;
		if (IsTeamGame())
		{
			for (int i = 0; i < teams.Length; i++)
			{
				int teamScore = GetTeamScore(i);
				if (teamScore > num)
				{
					num = teamScore;
					num2 = i;
				}
			}
			if (num2 == -1)
			{
				return -1;
			}
			return num2;
		}
		return -1;
	}

	public virtual bool DidPlayerWin(BasePlayer player)
	{
		if ((Object)(object)player == (Object)null)
		{
			return false;
		}
		if (IsDraw())
		{
			return false;
		}
		if (IsTeamGame())
		{
			PlayerScore playerScoreForPlayer = GetPlayerScoreForPlayer(player);
			if (playerScoreForPlayer.team == -1)
			{
				return false;
			}
			return playerScoreForPlayer.team == GetWinningTeamIndex();
		}
		return GetPlayerRank(player) == 1;
	}

	public bool IsTeamGame()
	{
		return teams.Length > 1;
	}

	public bool KeepScores()
	{
		return scoreColumns.Length != 0;
	}

	public void ModifyTeamScore(int teamIndex, int modifyAmount)
	{
		if (KeepScores())
		{
			TeamInfo obj = gameModeScores.teams[teamIndex];
			obj.score += modifyAmount;
			SendNetworkUpdate();
			CheckGameConditions();
		}
	}

	public void SetTeamScore(int teamIndex, int score)
	{
		gameModeScores.teams[teamIndex].score = score;
		SendNetworkUpdate();
	}

	public virtual void ResetPlayerScores(BasePlayer player)
	{
		if (!base.isClient)
		{
			for (int i = 0; i < scoreColumns.Length; i++)
			{
				SetPlayerGameScore(player, i, 0);
			}
		}
	}

	public void ModifyPlayerGameScore(BasePlayer player, string scoreName, int modifyAmount)
	{
		if (KeepScores())
		{
			int scoreIndexByName = GetScoreIndexByName(scoreName);
			ModifyPlayerGameScore(player, scoreIndexByName, modifyAmount);
		}
	}

	public void ModifyPlayerGameScore(BasePlayer player, int scoreIndex, int modifyAmount)
	{
		if (KeepScores())
		{
			PlayerScore playerScoreForPlayer = GetPlayerScoreForPlayer(player);
			int playerGameScore = GetPlayerGameScore(player, scoreIndex);
			if (IsTeamGame() && player.gamemodeteam >= 0 && scoreIndex == GetScoreIndexByName(teamScoreName))
			{
				gameModeScores.teams[player.gamemodeteam].score = gameModeScores.teams[player.gamemodeteam].score + modifyAmount;
			}
			SetPlayerGameScore(player, scoreIndex, playerGameScore + modifyAmount);
		}
	}

	public int GetPlayerGameScore(BasePlayer player, int scoreIndex)
	{
		PlayerScore playerScoreForPlayer = GetPlayerScoreForPlayer(player);
		return playerScoreForPlayer.scores[scoreIndex];
	}

	public void SetPlayerTeam(BasePlayer player, int newTeam)
	{
		player.gamemodeteam = newTeam;
		PlayerScore playerScoreForPlayer = GetPlayerScoreForPlayer(player);
		playerScoreForPlayer.team = newTeam;
		SendNetworkUpdate();
	}

	public void SetPlayerGameScore(BasePlayer player, int scoreIndex, int scoreValue)
	{
		if (!base.isClient && KeepScores())
		{
			PlayerScore playerScoreForPlayer = GetPlayerScoreForPlayer(player);
			playerScoreForPlayer.scores[scoreIndex] = scoreValue;
			SendNetworkUpdate();
			CheckGameConditions();
		}
	}

	public int GetMaxBeds(BasePlayer player)
	{
		return maximumSleepingBags;
	}

	protected virtual void SetupTags()
	{
		gameModeTags.Add("missions-" + (missionSystem ? "enabled" : "disabled"));
		gameModeTags.Add("mlrs-" + (mlrs ? "enabled" : "disabled"));
		gameModeTags.Add("map-" + (ingameMap ? "enabled" : "disabled"));
	}

	public virtual ResearchCostResult GetScrapCostForResearch(ItemDefinition item, ResearchTable.ResearchType researchType)
	{
		return default(ResearchCostResult);
	}

	public virtual float? EvaluateSleepingBagReset(SleepingBag bag, Vector3 position, SleepingBag.SleepingBagResetReason reason)
	{
		return null;
	}

	private void DeleteEntities()
	{
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		if (!SingletonComponent<ServerMgr>.Instance.runFrameUpdate)
		{
			((FacepunchBehaviour)this).Invoke((Action)DeleteEntities, 5f);
		}
		MonumentInfo[] array = TerrainMeta.Path.Monuments.Where((MonumentInfo x) => x.IsSafeZone).ToArray();
		MonumentInfo[] array2 = array;
		foreach (MonumentInfo monumentInfo in array2)
		{
			List<BaseEntity> list = new List<BaseEntity>();
			Vis.Entities(new OBB(((Component)monumentInfo).transform, monumentInfo.Bounds), list, -1, (QueryTriggerInteraction)2);
			foreach (BaseEntity item in list)
			{
				if (!safeZone && (item is HumanNPC || item is NPCAutoTurret || item is Marketplace))
				{
					item.Kill();
				}
			}
			if (!safeZone)
			{
				NPCSpawner[] componentsInChildren = ((Component)monumentInfo).GetComponentsInChildren<NPCSpawner>();
				foreach (NPCSpawner nPCSpawner in componentsInChildren)
				{
					nPCSpawner.isSpawnerActive = false;
				}
			}
			if (mlrs)
			{
				continue;
			}
			IndividualSpawner[] componentsInChildren2 = ((Component)monumentInfo).GetComponentsInChildren<IndividualSpawner>();
			foreach (IndividualSpawner individualSpawner in componentsInChildren2)
			{
				if (individualSpawner.entityPrefab.isValid && individualSpawner.entityPrefab.GetEntity() is MLRS)
				{
					individualSpawner.isSpawnerActive = false;
				}
			}
		}
		foreach (BaseNetworkable serverEntity in BaseNetworkable.serverEntities)
		{
			if (!mlrs && serverEntity is MLRS)
			{
				serverEntity.Kill();
			}
			if (!missionSystem && serverEntity is NPCMissionProvider)
			{
				serverEntity.Kill();
			}
		}
	}

	protected void OnCreated_Vanilla()
	{
		if (rustPlus != CompanionServer.Server.IsEnabled)
		{
			if (rustPlus)
			{
				CompanionServer.Server.Initialize();
			}
			else
			{
				CompanionServer.Server.Shutdown();
			}
		}
		if (!teamSystem)
		{
			RelationshipManager.maxTeamSize = 0;
		}
		ConVar.Server.max_sleeping_bags = maximumSleepingBags;
		ConVar.Server.crawlingenabled = crawling;
		DeleteEntities();
		if (wipeBpsOnProtocol)
		{
			SingletonComponent<ServerMgr>.Instance.persistance.Dispose();
			SingletonComponent<ServerMgr>.Instance.persistance = new UserPersistance(ConVar.Server.rootFolder);
			BasePlayer[] array = Object.FindObjectsOfType<BasePlayer>();
			foreach (BasePlayer basePlayer in array)
			{
				basePlayer.InvalidateCachedPeristantPlayer();
			}
		}
		RelationshipManager.contacts = contactSystem;
		Chat.globalchat = globalChat;
		Chat.localchat = localChat;
	}

	public bool HasAnyGameModeTag(string[] tags)
	{
		for (int i = 0; i < gameModeTags.Count; i++)
		{
			for (int j = 0; j < tags.Length; j++)
			{
				if (tags[j] == gameModeTags[i])
				{
					return true;
				}
			}
		}
		return false;
	}

	public bool HasGameModeTag(string tag)
	{
		for (int i = 0; i < gameModeTags.Count; i++)
		{
			if (gameModeTags[i] == tag)
			{
				return true;
			}
		}
		return false;
	}

	public bool AllowsSleeping()
	{
		return allowSleeping;
	}

	public bool HasLoadouts()
	{
		return loadouts.Length != 0 || (IsTeamGame() && teams[0].teamloadouts.Length != 0);
	}

	public int GetNumTeams()
	{
		return (teams.Length <= 1) ? 1 : teams.Length;
	}

	public int GetTeamScore(int teamIndex)
	{
		return gameModeScores.teams[teamIndex].score;
	}

	public static void CreateGameMode(string overrideMode = "")
	{
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		BaseGameMode activeGameMode = GetActiveGameMode(serverside: true);
		if (Object.op_Implicit((Object)(object)activeGameMode))
		{
			activeGameMode.ShutdownGame();
			activeGameMode.Kill();
			SetActiveGameMode(null, serverside: true);
		}
		string text = ConVar.Server.gamemode;
		Debug.Log((object)("Gamemode Convar :" + text));
		if (!string.IsNullOrEmpty(overrideMode))
		{
			text = overrideMode;
		}
		if (string.IsNullOrEmpty(text))
		{
			Debug.Log((object)"No Gamemode.");
			if (BaseGameMode.GameModeChanged != null)
			{
				BaseGameMode.GameModeChanged(null);
			}
		}
		else
		{
			BaseEntity baseEntity = GameManager.server.CreateEntity("assets/prefabs/gamemodes/" + text + ".prefab", Vector3.zero, Quaternion.identity);
			if (Object.op_Implicit((Object)(object)baseEntity))
			{
				baseEntity.Spawn();
			}
			else
			{
				Debug.Log((object)("Failed to create gamemode : " + text));
			}
		}
	}

	public static void SetActiveGameMode(BaseGameMode newActive, bool serverside)
	{
		if (Object.op_Implicit((Object)(object)newActive))
		{
			newActive.InitScores();
		}
		if (BaseGameMode.GameModeChanged != null)
		{
			BaseGameMode.GameModeChanged(newActive);
		}
		if (serverside)
		{
			svActiveGameMode = newActive;
		}
	}

	public static BaseGameMode GetActiveGameMode(bool serverside)
	{
		return svActiveGameMode;
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
		if (info.msg.gameMode != null)
		{
			CopyGameModeScores(info.msg.gameMode, gameModeScores);
		}
	}

	public override void Save(SaveInfo info)
	{
		base.Save(info);
		info.msg.gameMode = Pool.Get<GameMode>();
		info.msg.gameMode.scoreColumns = Pool.GetList<ScoreColumn>();
		info.msg.gameMode.playerScores = Pool.GetList<PlayerScore>();
		info.msg.gameMode.teams = Pool.GetList<TeamInfo>();
		CopyGameModeScores(gameModeScores, info.msg.gameMode);
		info.msg.gameMode.ShouldPool = true;
	}

	public virtual float CorpseRemovalTime(BaseCorpse corpse)
	{
		return ConVar.Server.corpsedespawn;
	}

	public virtual bool InWarmup()
	{
		return HasFlag(Flags.Reserved1);
	}

	public virtual bool IsWaitingForPlayers()
	{
		return HasFlag(Flags.Reserved3);
	}

	public virtual bool IsMatchOver()
	{
		return HasFlag(Flags.Reserved2);
	}

	public virtual bool IsMatchActive()
	{
		return !InWarmup() && !IsWaitingForPlayers() && !IsMatchOver() && matchStartTime != -1f;
	}

	public override void InitShared()
	{
		base.InitShared();
		if ((Object)(object)GetActiveGameMode(base.isServer) != (Object)null && (Object)(object)GetActiveGameMode(base.isServer) != (Object)(object)this)
		{
			Debug.LogError((object)("Already an active game mode! was : " + ((Object)GetActiveGameMode(base.isServer)).name));
			Object.Destroy((Object)(object)((Component)GetActiveGameMode(base.isServer)).gameObject);
		}
		SetupTags();
		SetActiveGameMode(this, base.isServer);
		OnCreated();
	}

	public override void DestroyShared()
	{
		if ((Object)(object)GetActiveGameMode(base.isServer) == (Object)(object)this)
		{
			SetActiveGameMode(null, base.isServer);
		}
		base.DestroyShared();
	}

	protected virtual void OnCreated()
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		OnCreated_Vanilla();
		if (base.isServer)
		{
			foreach (string convar in convars)
			{
				ConsoleSystem.Run(Option.Server, convar, Array.Empty<object>());
			}
			gameModeSpawnGroups = Object.FindObjectsOfType<GameModeSpawnGroup>();
			UnassignAllPlayers();
			Enumerator<BasePlayer> enumerator2 = BasePlayer.activePlayerList.GetEnumerator();
			try
			{
				while (enumerator2.MoveNext())
				{
					BasePlayer current2 = enumerator2.Current;
					AutoAssignTeam(current2);
				}
			}
			finally
			{
				((IDisposable)enumerator2).Dispose();
			}
			InstallSpawnpoints();
			ResetMatch();
		}
		Debug.Log((object)("Game created! type was : " + ((Object)this).name));
	}

	protected virtual void OnMatchBegin()
	{
		matchStartTime = Time.realtimeSinceStartup;
		SetFlag(Flags.Reserved3, b: false);
		SetFlag(Flags.Reserved1, b: false);
		SetFlag(Flags.Reserved2, b: false);
	}

	public virtual void ResetMatch()
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		if (IsWaitingForPlayers())
		{
			return;
		}
		isResetting = true;
		SetFlag(Flags.Reserved1, b: true, recursive: false, networkupdate: false);
		SetFlag(Flags.Reserved2, b: false);
		ResetTeamScores();
		Enumerator<BasePlayer> enumerator = BasePlayer.activePlayerList.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				BasePlayer current = enumerator.Current;
				ResetPlayerScores(current);
				current.Hurt(100000f, DamageType.Suicide, null, useProtection: false);
				current.Respawn();
			}
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
		GameModeSpawnGroup[] array = gameModeSpawnGroups;
		foreach (GameModeSpawnGroup gameModeSpawnGroup in array)
		{
			gameModeSpawnGroup.ResetSpawnGroup();
		}
		matchStartTime = -1f;
		((FacepunchBehaviour)this).Invoke((Action)OnMatchBegin, warmupDuration);
		isResetting = false;
	}

	public virtual void ResetTeamScores()
	{
		for (int i = 0; i < teams.Length; i++)
		{
			SetTeamScore(i, 0);
		}
	}

	public virtual void ShutdownGame()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		ResetTeamScores();
		Enumerator<BasePlayer> enumerator = BasePlayer.activePlayerList.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				BasePlayer current = enumerator.Current;
				SetPlayerTeam(current, -1);
			}
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
	}

	private void Update()
	{
		if (!base.isClient)
		{
			OnThink(Time.deltaTime);
		}
	}

	protected virtual void OnThink(float delta)
	{
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		if (matchStartTime != -1f)
		{
			float num = Time.realtimeSinceStartup - matchStartTime;
			if (IsMatchActive() && matchDuration > 0f && num >= matchDuration)
			{
				OnMatchEnd();
			}
		}
		int num2 = 0;
		Enumerator<BasePlayer> enumerator = BasePlayer.activePlayerList.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				BasePlayer current = enumerator.Current;
				if (autoHealDelay > 0f && current.healthFraction < 1f && current.IsAlive() && !current.IsWounded() && current.SecondsSinceAttacked >= autoHealDelay)
				{
					current.Heal(current.MaxHealth() * delta / autoHealDuration);
				}
				if (current.IsConnected)
				{
					num2++;
				}
			}
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
		if (num2 < minPlayersToStart && !IsWaitingForPlayers())
		{
			if (IsMatchActive())
			{
				OnMatchEnd();
				return;
			}
			SetFlag(Flags.Reserved3, b: true);
			SetFlag(Flags.Reserved2, b: false);
			SetFlag(Flags.Reserved1, b: false);
		}
		else if (IsWaitingForPlayers() && num2 >= minPlayersToStart)
		{
			SetFlag(Flags.Reserved3, b: false);
			((FacepunchBehaviour)this).CancelInvoke((Action)ResetMatch);
			ResetMatch();
		}
	}

	public virtual void OnMatchEnd()
	{
		matchEndTime = Time.time;
		Debug.Log((object)"Match over!");
		SetFlag(Flags.Reserved2, b: true);
		((FacepunchBehaviour)this).Invoke((Action)ResetMatch, timeBetweenMatches);
	}

	public virtual void OnNewPlayer(BasePlayer player)
	{
		player.Respawn();
		if (!AllowsSleeping())
		{
			player.EndSleeping();
			player.SendNetworkUpdateImmediate();
		}
		PostPlayerRespawn(player);
	}

	public void PostPlayerRespawn(BasePlayer player)
	{
		if (startHealthOverride > 0f)
		{
			player.SetMaxHealth(startHealthOverride);
			player.health = startHealthOverride;
		}
	}

	public virtual void OnPlayerConnected(BasePlayer player)
	{
		AutoAssignTeam(player);
		ResetPlayerScores(player);
	}

	public virtual void UnassignAllPlayers()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		Enumerator<BasePlayer> enumerator = BasePlayer.activePlayerList.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				BasePlayer current = enumerator.Current;
				SetPlayerTeam(current, -1);
			}
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
	}

	public void AutoAssignTeam(BasePlayer player)
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		int newTeam = 0;
		int[] array = new int[teams.Length];
		int num = Random.Range(0, teams.Length);
		int num2 = 0;
		if (teams.Length > 1)
		{
			Enumerator<BasePlayer> enumerator = BasePlayer.activePlayerList.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					BasePlayer current = enumerator.Current;
					if (current.gamemodeteam >= 0 && current.gamemodeteam < teams.Length)
					{
						array[current.gamemodeteam]++;
					}
				}
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] < num2)
				{
					num = i;
				}
			}
			newTeam = num;
		}
		SetPlayerTeam(player, newTeam);
	}

	public virtual void OnPlayerDisconnected(BasePlayer player)
	{
		if (gameModeScores == null || base.isClient)
		{
			return;
		}
		PlayerScore val = null;
		foreach (PlayerScore playerScore in gameModeScores.playerScores)
		{
			if (playerScore.userid == player.userID)
			{
				val = playerScore;
				break;
			}
		}
		if (val != null)
		{
			gameModeScores.playerScores.Remove(val);
		}
	}

	public virtual void OnPlayerWounded(BasePlayer instigator, BasePlayer victim, HitInfo info)
	{
	}

	public virtual void OnPlayerRevived(BasePlayer instigator, BasePlayer victim)
	{
	}

	public virtual void OnPlayerHurt(BasePlayer instigator, BasePlayer victim, HitInfo deathInfo = null)
	{
		if (!allowBleeding && victim.metabolism.bleeding.value != 0f)
		{
			victim.metabolism.bleeding.value = 0f;
			victim.metabolism.SendChangesToClient();
		}
	}

	public virtual void OnPlayerDeath(BasePlayer instigator, BasePlayer victim, HitInfo deathInfo = null)
	{
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		if (!IsMatchActive())
		{
			return;
		}
		if ((Object)(object)victim != (Object)null && victim.IsConnected && !victim.IsNpc)
		{
			ModifyPlayerGameScore(victim, "deaths", 1);
		}
		bool flag = IsTeamGame() && (Object)(object)instigator != (Object)null && (Object)(object)victim != (Object)null && instigator.gamemodeteam == victim.gamemodeteam;
		if ((Object)(object)instigator != (Object)null && (Object)(object)victim != (Object)(object)instigator && !flag && !instigator.IsNpc)
		{
			ModifyPlayerGameScore(instigator, "kills", 1);
		}
		if ((Object)(object)instigator != (Object)null && instigator.IsConnected && !instigator.IsNpc && (Object)(object)instigator != (Object)(object)victim)
		{
			ClientRPCPlayer(null, instigator, "RPC_ScoreSplash", victim.displayName, 100, arg3: true);
		}
		if (hasKillFeed && (Object)(object)instigator != (Object)null && (Object)(object)victim != (Object)null && (Object)(object)deathInfo.Weapon != (Object)null && deathInfo.Weapon.GetItem() != null)
		{
			string text = Vector3.Distance(((Component)instigator).transform.position, ((Component)victim).transform.position).ToString("N0") + "m";
			string text2 = " with a " + deathInfo.Weapon.GetItem().info.displayName.translated + " from " + text;
			string msg = "You Killed " + victim.displayName + text2;
			string msg2 = instigator.displayName + " Killed You" + text2;
			string msg3 = instigator.displayName + " Killed" + victim.displayName + text2;
			Enumerator<BasePlayer> enumerator = BasePlayer.activePlayerList.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					BasePlayer current = enumerator.Current;
					if ((Object)(object)current == (Object)(object)instigator)
					{
						current.ChatMessage(msg);
					}
					else if ((Object)(object)current == (Object)(object)victim)
					{
						current.ChatMessage(msg2);
					}
					else if (BasePlayer.activePlayerList.Count <= 5)
					{
						current.ChatMessage(msg3);
					}
				}
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
		}
		CheckGameConditions(force: true);
	}

	public virtual bool CanPlayerRespawn(BasePlayer player)
	{
		return !IsMatchOver() || IsWaitingForPlayers() || isResetting;
	}

	public virtual void OnPlayerRespawn(BasePlayer player)
	{
		if (!AllowsSleeping())
		{
			player.EndSleeping();
			player.MarkRespawn(respawnDelayOverride);
			SendNetworkUpdateImmediate();
		}
		PostPlayerRespawn(player);
	}

	public virtual void CheckGameConditions(bool force = false)
	{
		if (!IsMatchActive())
		{
			return;
		}
		if (IsTeamGame())
		{
			for (int i = 0; i < teams.Length; i++)
			{
				if (GetTeamScore(i) >= numScoreForVictory)
				{
					OnMatchEnd();
				}
			}
			return;
		}
		int num = ScoreColumnIndex(victoryScoreName);
		if (num == -1)
		{
			return;
		}
		foreach (PlayerScore playerScore in gameModeScores.playerScores)
		{
			if (playerScore.scores[num] >= numScoreForVictory)
			{
				OnMatchEnd();
			}
		}
	}

	public virtual void LoadoutPlayer(BasePlayer player)
	{
		PlayerInventoryProperties playerInventoryProperties;
		if (!IsTeamGame())
		{
			playerInventoryProperties = ((!useStaticLoadoutPerPlayer) ? loadouts[Random.Range(0, loadouts.Length)] : loadouts[SeedRandom.Range((uint)player.userID, 0, loadouts.Length)]);
		}
		else
		{
			if (player.gamemodeteam == -1)
			{
				Debug.LogWarning((object)"Player loading out without team assigned, auto assigning!");
				AutoAssignTeam(player);
			}
			playerInventoryProperties = teams[player.gamemodeteam].teamloadouts[SeedRandom.Range((uint)player.userID, 0, teams[player.gamemodeteam].teamloadouts.Length)];
		}
		if (Object.op_Implicit((Object)(object)playerInventoryProperties))
		{
			playerInventoryProperties.GiveToPlayer(player);
		}
		else
		{
			player.inventory.GiveItem(ItemManager.CreateByName("hazmatsuit", 1, 0uL), player.inventory.containerWear);
		}
		if (!topUpMagazines)
		{
			return;
		}
		foreach (Item item in player.inventory.containerBelt.itemList)
		{
			BaseEntity heldEntity = item.GetHeldEntity();
			if ((Object)(object)heldEntity != (Object)null)
			{
				BaseProjectile component = ((Component)heldEntity).GetComponent<BaseProjectile>();
				if ((Object)(object)component != (Object)null)
				{
					component.TopUpAmmo();
				}
			}
		}
	}

	public virtual void InstallSpawnpoints()
	{
		allspawns = GameObject.FindGameObjectsWithTag("spawnpoint");
		if (allspawns != null)
		{
			Debug.Log((object)("Installed : " + allspawns.Length + "spawn points."));
		}
	}

	public virtual BasePlayer.SpawnPoint GetPlayerSpawn(BasePlayer forPlayer)
	{
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		if (allspawns == null)
		{
			InstallSpawnpoints();
		}
		float num = 0f;
		int num2 = Random.Range(0, allspawns.Length);
		if (allspawns.Length != 0 && (Object)(object)forPlayer != (Object)null)
		{
			for (int i = 0; i < allspawns.Length; i++)
			{
				GameObject val = allspawns[i];
				float num3 = 0f;
				for (int j = 0; j < BasePlayer.activePlayerList.Count; j++)
				{
					BasePlayer basePlayer = BasePlayer.activePlayerList[j];
					if (!((Object)(object)basePlayer == (Object)null) && basePlayer.IsAlive() && !((Object)(object)basePlayer == (Object)(object)forPlayer))
					{
						float num4 = Vector3.Distance(((Component)basePlayer).transform.position, val.transform.position);
						num3 -= 100f * (1f - Mathf.InverseLerp(8f, 16f, num4));
						if (!IsTeamGame() || basePlayer.gamemodeteam != forPlayer.gamemodeteam)
						{
							num3 += 100f * Mathf.InverseLerp(16f, 32f, num4);
						}
					}
				}
				Vector3 val2 = ((forPlayer.ServerCurrentDeathNote == null) ? allspawns[Random.Range(0, allspawns.Length)].transform.position : forPlayer.ServerCurrentDeathNote.worldPosition);
				float num5 = Vector3.Distance(val2, val.transform.position);
				float num6 = Mathf.InverseLerp(8f, 25f, num5);
				num3 *= num6;
				if (num3 > num)
				{
					num2 = i;
					num = num3;
				}
			}
		}
		GameObject val3 = allspawns[num2];
		BasePlayer.SpawnPoint spawnPoint = new BasePlayer.SpawnPoint();
		spawnPoint.pos = val3.transform.position;
		spawnPoint.rot = val3.transform.rotation;
		return spawnPoint;
	}

	public virtual int GetMaxRelationshipTeamSize()
	{
		return RelationshipManager.maxTeamSize;
	}

	public virtual SleepingBag[] FindSleepingBagsForPlayer(ulong playerID, bool ignoreTimers)
	{
		return SleepingBag.FindForPlayer(playerID, ignoreTimers);
	}

	public virtual bool CanMoveItemsFrom(PlayerInventory inv, BaseEntity source, Item item)
	{
		return true;
	}
}
