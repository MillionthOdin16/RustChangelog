using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Facepunch;
using Facepunch.Extend;
using Network;
using Network.Visibility;
using Rust;
using UnityEngine;
using UnityEngine.Profiling;

namespace ConVar;

[Factory("global")]
public class Global : ConsoleSystem
{
	private static int _developer;

	[ServerVar]
	[ClientVar(Help = "WARNING: This causes random crashes!")]
	public static bool skipAssetWarmup_crashes = false;

	[ServerVar]
	[ClientVar]
	public static int maxthreads = 8;

	private const int DefaultWarmupConcurrency = 1;

	private const int DefaultPreloadConcurrency = 1;

	[ServerVar]
	[ClientVar]
	public static int warmupConcurrency = 1;

	[ServerVar]
	[ClientVar]
	public static int preloadConcurrency = 1;

	[ServerVar]
	[ClientVar]
	public static bool forceUnloadBundles = true;

	private const bool DefaultAsyncWarmupEnabled = false;

	[ServerVar]
	[ClientVar]
	public static bool asyncWarmup = false;

	[ClientVar(Saved = true, Help = "Experimental faster loading, requires game restart (0 = off, 1 = partial, 2 = full)")]
	public static int asyncLoadingPreset = 0;

	[ServerVar(Saved = true)]
	[ClientVar(Saved = true)]
	public static int perf = 0;

	[ClientVar(ClientInfo = true, Saved = true, Help = "If you're an admin this will enable god mode")]
	public static bool god = false;

	[ClientVar(ClientInfo = true, Saved = true, Help = "If enabled you will be networked when you're spectating. This means that you will hear audio chat, but also means that cheaters will potentially be able to detect you watching them.")]
	public static bool specnet = false;

	[ClientVar]
	[ServerVar(ClientAdmin = true, ServerAdmin = true, Help = "When enabled a player wearing a gingerbread suit will gib like the gingerbread NPC's")]
	public static bool cinematicGingerbreadCorpses = false;

	private static uint _gingerbreadMaterialID = 0u;

	[ServerVar(Saved = true, ShowInAdminUI = true, Help = "Multiplier applied to SprayDuration if a spray isn't in the sprayers auth (cannot go above 1f)")]
	public static float SprayOutOfAuthMultiplier = 0.5f;

	[ServerVar(Saved = true, ShowInAdminUI = true, Help = "Base time (in seconds) that sprays last")]
	public static float SprayDuration = 10800f;

	[ServerVar(Saved = true, ShowInAdminUI = true, Help = "If a player sprays more than this, the oldest spray will be destroyed. 0 will disable")]
	public static int MaxSpraysPerPlayer = 40;

	[ServerVar(Help = "Disables the backpacks that appear after a corpse times out")]
	public static bool disableBagDropping = false;

	[ServerVar]
	[ClientVar]
	public static int developer
	{
		get
		{
			return _developer;
		}
		set
		{
			_developer = value;
		}
	}

	public static void ApplyAsyncLoadingPreset()
	{
		if (asyncLoadingPreset != 0)
		{
			Debug.Log((object)$"Applying async loading preset number {asyncLoadingPreset}");
		}
		switch (asyncLoadingPreset)
		{
		case 1:
			if (warmupConcurrency <= 1)
			{
				warmupConcurrency = 256;
			}
			if (preloadConcurrency <= 1)
			{
				preloadConcurrency = 256;
			}
			asyncWarmup = false;
			break;
		case 2:
			if (warmupConcurrency <= 1)
			{
				warmupConcurrency = 256;
			}
			if (preloadConcurrency <= 1)
			{
				preloadConcurrency = 256;
			}
			asyncWarmup = false;
			break;
		default:
			Debug.LogWarning((object)$"There is no asyncLoading preset number {asyncLoadingPreset}");
			break;
		case 0:
			break;
		}
	}

	[ServerVar]
	public static void restart(Arg args)
	{
		ServerMgr.RestartServer(args.GetString(1, string.Empty), args.GetInt(0, 300));
	}

	[ClientVar]
	[ServerVar]
	public static void quit(Arg args)
	{
		SingletonComponent<ServerMgr>.Instance.Shutdown();
		Application.isQuitting = true;
		Net.sv.Stop("quit");
		Process.GetCurrentProcess().Kill();
		Debug.Log((object)"Quitting");
		Application.Quit();
	}

	[ServerVar]
	public static void report(Arg args)
	{
		ServerPerformance.DoReport();
	}

	[ServerVar]
	[ClientVar]
	public static void objects(Arg args)
	{
		Object[] array = Object.FindObjectsOfType<Object>();
		string text = "";
		Dictionary<Type, int> dictionary = new Dictionary<Type, int>();
		Dictionary<Type, long> dictionary2 = new Dictionary<Type, long>();
		Object[] array2 = array;
		foreach (Object val in array2)
		{
			int runtimeMemorySize = Profiler.GetRuntimeMemorySize(val);
			if (dictionary.ContainsKey(((object)val).GetType()))
			{
				dictionary[((object)val).GetType()]++;
			}
			else
			{
				dictionary.Add(((object)val).GetType(), 1);
			}
			if (dictionary2.ContainsKey(((object)val).GetType()))
			{
				dictionary2[((object)val).GetType()] += runtimeMemorySize;
			}
			else
			{
				dictionary2.Add(((object)val).GetType(), runtimeMemorySize);
			}
		}
		foreach (KeyValuePair<Type, long> item in dictionary2.OrderByDescending(delegate(KeyValuePair<Type, long> x)
		{
			KeyValuePair<Type, long> keyValuePair = x;
			return keyValuePair.Value;
		}))
		{
			text = text + dictionary[item.Key].ToString().PadLeft(10) + " " + NumberExtensions.FormatBytes<long>(item.Value, false).PadLeft(15) + "\t" + item.Key?.ToString() + "\n";
		}
		args.ReplyWith(text);
	}

	[ServerVar]
	[ClientVar]
	public static void textures(Arg args)
	{
		Texture[] array = Object.FindObjectsOfType<Texture>();
		string text = "";
		Texture[] array2 = array;
		foreach (Texture val in array2)
		{
			string text2 = NumberExtensions.FormatBytes<int>(Profiler.GetRuntimeMemorySize((Object)(object)val), false);
			text = text + ((object)val).ToString().PadRight(30) + ((Object)val).name.PadRight(30) + text2 + "\n";
		}
		args.ReplyWith(text);
	}

	[ServerVar]
	[ClientVar]
	public static void colliders(Arg args)
	{
		int num = (from x in Object.FindObjectsOfType<Collider>()
			where x.enabled
			select x).Count();
		int num2 = (from x in Object.FindObjectsOfType<Collider>()
			where !x.enabled
			select x).Count();
		string text = num + " colliders enabled, " + num2 + " disabled";
		args.ReplyWith(text);
	}

	[ServerVar]
	[ClientVar]
	public static void error(Arg args)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		((GameObject)null).transform.position = Vector3.zero;
	}

	[ServerVar]
	[ClientVar]
	public static void queue(Arg args)
	{
		string text = "";
		text = text + "stabilityCheckQueue:\t\t" + ((ObjectWorkQueue<StabilityEntity>)StabilityEntity.stabilityCheckQueue).Info() + "\n";
		text = text + "updateSurroundingsQueue:\t" + ((ObjectWorkQueue<Bounds>)StabilityEntity.updateSurroundingsQueue).Info() + "\n";
		args.ReplyWith(text);
	}

	[ServerUserVar]
	public static void setinfo(Arg args)
	{
		BasePlayer basePlayer = args.Player();
		if (Object.op_Implicit((Object)(object)basePlayer))
		{
			string @string = args.GetString(0, (string)null);
			string string2 = args.GetString(1, (string)null);
			if (@string != null && string2 != null)
			{
				basePlayer.SetInfo(@string, string2);
			}
		}
	}

	[ServerVar]
	public static void sleep(Arg args)
	{
		BasePlayer basePlayer = args.Player();
		if (Object.op_Implicit((Object)(object)basePlayer) && !basePlayer.IsSleeping() && !basePlayer.IsSpectating() && !basePlayer.IsDead())
		{
			basePlayer.StartSleeping();
		}
	}

	[ServerUserVar]
	public static void kill(Arg args)
	{
		BasePlayer basePlayer = args.Player();
		if (Object.op_Implicit((Object)(object)basePlayer) && !basePlayer.IsSpectating() && !basePlayer.IsDead())
		{
			if (basePlayer.CanSuicide())
			{
				basePlayer.MarkSuicide();
				basePlayer.Hurt(1000f, DamageType.Suicide, basePlayer, useProtection: false);
			}
			else
			{
				basePlayer.ConsoleMessage("You can't suicide again so quickly, wait a while");
			}
		}
	}

	[ServerUserVar]
	public static void respawn(Arg args)
	{
		BasePlayer basePlayer = args.Player();
		if (!Object.op_Implicit((Object)(object)basePlayer))
		{
			return;
		}
		if (!basePlayer.IsDead() && !basePlayer.IsSpectating())
		{
			if (developer > 0)
			{
				Debug.LogWarning((object)(((object)basePlayer)?.ToString() + " wanted to respawn but isn't dead or spectating"));
			}
			basePlayer.SendNetworkUpdate();
		}
		else if (basePlayer.CanRespawn())
		{
			basePlayer.MarkRespawn();
			basePlayer.Respawn();
		}
		else
		{
			basePlayer.ConsoleMessage("You can't respawn again so quickly, wait a while");
		}
	}

	[ServerVar]
	public static void injure(Arg args)
	{
		InjurePlayer(args.Player());
	}

	public static void InjurePlayer(BasePlayer ply)
	{
		if ((Object)(object)ply == (Object)null || ply.IsDead())
		{
			return;
		}
		if (Server.woundingenabled && !ply.IsIncapacitated() && !ply.IsSleeping() && !ply.isMounted)
		{
			if (ply.IsCrawling())
			{
				ply.GoToIncapacitated(null);
			}
			else
			{
				ply.BecomeWounded();
			}
		}
		else
		{
			ply.ConsoleMessage("Can't go to wounded state right now.");
		}
	}

	[ServerVar]
	public static void recover(Arg args)
	{
		RecoverPlayer(args.Player());
	}

	public static void RecoverPlayer(BasePlayer ply)
	{
		if (!((Object)(object)ply == (Object)null) && !ply.IsDead())
		{
			ply.StopWounded();
		}
	}

	[ServerVar]
	public static void spectate(Arg args)
	{
		BasePlayer basePlayer = args.Player();
		if (Object.op_Implicit((Object)(object)basePlayer))
		{
			if (!basePlayer.IsDead())
			{
				basePlayer.DieInstantly();
			}
			string @string = args.GetString(0, "");
			if (basePlayer.IsDead())
			{
				basePlayer.StartSpectating();
				basePlayer.UpdateSpectateTarget(@string);
			}
		}
	}

	[ServerVar]
	public static void spectateid(Arg args)
	{
		BasePlayer basePlayer = args.Player();
		if (Object.op_Implicit((Object)(object)basePlayer))
		{
			if (!basePlayer.IsDead())
			{
				basePlayer.DieInstantly();
			}
			ulong uLong = args.GetULong(0, 0uL);
			if (basePlayer.IsDead())
			{
				basePlayer.StartSpectating();
				basePlayer.UpdateSpectateTarget(uLong);
			}
		}
	}

	[ServerUserVar]
	public static void respawn_sleepingbag(Arg args)
	{
		BasePlayer basePlayer = args.Player();
		if (!Object.op_Implicit((Object)(object)basePlayer) || !basePlayer.IsDead())
		{
			return;
		}
		uint uInt = args.GetUInt(0, 0u);
		if (uInt == 0)
		{
			args.ReplyWith("Missing sleeping bag ID");
		}
		else if (basePlayer.CanRespawn())
		{
			if (SleepingBag.SpawnPlayer(basePlayer, uInt))
			{
				basePlayer.MarkRespawn();
			}
			else
			{
				args.ReplyWith("Couldn't spawn in sleeping bag!");
			}
		}
		else
		{
			basePlayer.ConsoleMessage("You can't respawn again so quickly, wait a while");
		}
	}

	[ServerUserVar]
	public static void respawn_sleepingbag_remove(Arg args)
	{
		BasePlayer basePlayer = args.Player();
		if (Object.op_Implicit((Object)(object)basePlayer))
		{
			uint uInt = args.GetUInt(0, 0u);
			if (uInt == 0)
			{
				args.ReplyWith("Missing sleeping bag ID");
			}
			else
			{
				SleepingBag.DestroyBag(basePlayer, uInt);
			}
		}
	}

	[ServerUserVar]
	public static void status_sv(Arg args)
	{
		BasePlayer basePlayer = args.Player();
		if (Object.op_Implicit((Object)(object)basePlayer))
		{
			args.ReplyWith(basePlayer.GetDebugStatus());
		}
	}

	[ClientVar]
	public static void status_cl(Arg args)
	{
	}

	[ServerVar]
	public static void teleport(Arg args)
	{
		if (args.HasArgs(2))
		{
			BasePlayer playerOrSleeperOrBot = args.GetPlayerOrSleeperOrBot(0);
			if (Object.op_Implicit((Object)(object)playerOrSleeperOrBot) && playerOrSleeperOrBot.IsAlive())
			{
				BasePlayer playerOrSleeperOrBot2 = args.GetPlayerOrSleeperOrBot(1);
				if (Object.op_Implicit((Object)(object)playerOrSleeperOrBot2) && playerOrSleeperOrBot2.IsAlive())
				{
					playerOrSleeperOrBot.Teleport(playerOrSleeperOrBot2);
				}
			}
			return;
		}
		BasePlayer basePlayer = args.Player();
		if (Object.op_Implicit((Object)(object)basePlayer) && basePlayer.IsAlive())
		{
			BasePlayer playerOrSleeperOrBot3 = args.GetPlayerOrSleeperOrBot(0);
			if (Object.op_Implicit((Object)(object)playerOrSleeperOrBot3) && playerOrSleeperOrBot3.IsAlive())
			{
				basePlayer.Teleport(playerOrSleeperOrBot3);
			}
		}
	}

	[ServerVar]
	public static void teleport2me(Arg args)
	{
		BasePlayer playerOrSleeperOrBot = args.GetPlayerOrSleeperOrBot(0);
		if ((Object)(object)playerOrSleeperOrBot == (Object)null)
		{
			args.ReplyWith("Player or bot not found");
			return;
		}
		if (!playerOrSleeperOrBot.IsAlive())
		{
			args.ReplyWith("Target is not alive");
			return;
		}
		BasePlayer basePlayer = args.Player();
		if (Object.op_Implicit((Object)(object)basePlayer) && basePlayer.IsAlive())
		{
			playerOrSleeperOrBot.Teleport(basePlayer);
		}
	}

	[ServerVar]
	public static void teleportany(Arg args)
	{
		BasePlayer basePlayer = args.Player();
		if (Object.op_Implicit((Object)(object)basePlayer) && basePlayer.IsAlive())
		{
			basePlayer.Teleport(args.GetString(0, ""), playersOnly: false);
		}
	}

	[ServerVar]
	public static void teleportpos(Arg args)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		BasePlayer basePlayer = args.Player();
		if (Object.op_Implicit((Object)(object)basePlayer) && basePlayer.IsAlive())
		{
			basePlayer.Teleport(args.GetVector3(0, Vector3.zero));
		}
	}

	[ServerVar]
	public static void teleportlos(Arg args)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		BasePlayer basePlayer = args.Player();
		if (Object.op_Implicit((Object)(object)basePlayer) && basePlayer.IsAlive())
		{
			Ray val = basePlayer.eyes.HeadRay();
			int @int = args.GetInt(0, 1000);
			RaycastHit val2 = default(RaycastHit);
			if (Physics.Raycast(val, ref val2, (float)@int, 1218652417))
			{
				basePlayer.Teleport(((RaycastHit)(ref val2)).point);
			}
			else
			{
				basePlayer.Teleport(((Ray)(ref val)).origin + ((Ray)(ref val)).direction * (float)@int);
			}
		}
	}

	[ServerVar]
	public static void teleport2owneditem(Arg arg)
	{
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		BasePlayer basePlayer = arg.Player();
		BasePlayer playerOrSleeper = arg.GetPlayerOrSleeper(0);
		ulong result;
		if ((Object)(object)playerOrSleeper != (Object)null)
		{
			result = playerOrSleeper.userID;
		}
		else if (!ulong.TryParse(arg.GetString(0, ""), out result))
		{
			arg.ReplyWith("No player with that id found");
			return;
		}
		string @string = arg.GetString(1, "");
		BaseEntity[] array = BaseEntity.Util.FindTargetsOwnedBy(result, @string);
		if (array.Length == 0)
		{
			arg.ReplyWith("No targets found");
			return;
		}
		int num = Random.Range(0, array.Length);
		arg.ReplyWith($"Teleporting to {array[num].ShortPrefabName} at {((Component)array[num]).transform.position}");
		basePlayer.Teleport(((Component)array[num]).transform.position);
	}

	[ServerVar]
	public static void teleport2autheditem(Arg arg)
	{
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		BasePlayer basePlayer = arg.Player();
		BasePlayer playerOrSleeper = arg.GetPlayerOrSleeper(0);
		ulong result;
		if ((Object)(object)playerOrSleeper != (Object)null)
		{
			result = playerOrSleeper.userID;
		}
		else if (!ulong.TryParse(arg.GetString(0, ""), out result))
		{
			arg.ReplyWith("No player with that id found");
			return;
		}
		string @string = arg.GetString(1, "");
		BaseEntity[] array = BaseEntity.Util.FindTargetsAuthedTo(result, @string);
		if (array.Length == 0)
		{
			arg.ReplyWith("No targets found");
			return;
		}
		int num = Random.Range(0, array.Length);
		arg.ReplyWith($"Teleporting to {array[num].ShortPrefabName} at {((Component)array[num]).transform.position}");
		basePlayer.Teleport(((Component)array[num]).transform.position);
	}

	[ServerVar]
	public static void teleport2marker(Arg arg)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		BasePlayer basePlayer = arg.Player();
		if (basePlayer.ServerCurrentMapNote == null)
		{
			arg.ReplyWith("You don't have a marker set");
			return;
		}
		Vector3 worldPosition = basePlayer.ServerCurrentMapNote.worldPosition;
		float height = TerrainMeta.HeightMap.GetHeight(worldPosition);
		float height2 = TerrainMeta.WaterMap.GetHeight(worldPosition);
		worldPosition.y = Mathf.Max(height, height2);
		basePlayer.Teleport(worldPosition);
	}

	[ServerVar]
	public static void teleport2death(Arg arg)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		BasePlayer basePlayer = arg.Player();
		if (basePlayer.ServerCurrentDeathNote == null)
		{
			arg.ReplyWith("You don't have a current death note!");
		}
		Vector3 worldPosition = basePlayer.ServerCurrentDeathNote.worldPosition;
		basePlayer.Teleport(worldPosition);
	}

	[ServerVar]
	[ClientVar]
	public static void free(Arg args)
	{
		Pool.clear_prefabs(args);
		Pool.clear_assets(args);
		Pool.clear_memory(args);
		GC.collect();
		GC.unload();
	}

	[ServerVar(ServerUser = true)]
	[ClientVar]
	public static void version(Arg arg)
	{
		arg.ReplyWith($"Protocol: {Protocol.printable}\nBuild Date: {BuildInfo.Current.BuildDate}\nUnity Version: {Application.unityVersion}\nChangeset: {BuildInfo.Current.Scm.ChangeId}\nBranch: {BuildInfo.Current.Scm.Branch}");
	}

	[ServerVar]
	[ClientVar]
	public static void sysinfo(Arg arg)
	{
		arg.ReplyWith(SystemInfoGeneralText.currentInfo);
	}

	[ServerVar]
	[ClientVar]
	public static void sysuid(Arg arg)
	{
		arg.ReplyWith(SystemInfo.deviceUniqueIdentifier);
	}

	[ServerVar]
	public static void breakitem(Arg args)
	{
		BasePlayer basePlayer = args.Player();
		if (Object.op_Implicit((Object)(object)basePlayer))
		{
			Item activeItem = basePlayer.GetActiveItem();
			activeItem?.LoseCondition(activeItem.condition);
		}
	}

	[ServerVar]
	public static void breakclothing(Arg args)
	{
		BasePlayer basePlayer = args.Player();
		if (!Object.op_Implicit((Object)(object)basePlayer))
		{
			return;
		}
		foreach (Item item in basePlayer.inventory.containerWear.itemList)
		{
			item?.LoseCondition(item.condition);
		}
	}

	[ServerVar]
	[ClientVar]
	public static void subscriptions(Arg arg)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		TextTable val = new TextTable();
		val.AddColumn("realm");
		val.AddColumn("group");
		BasePlayer basePlayer = arg.Player();
		if (Object.op_Implicit((Object)(object)basePlayer))
		{
			Enumerator<Group> enumerator = basePlayer.net.subscriber.subscribed.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Group current = enumerator.Current;
					val.AddRow(new string[2]
					{
						"sv",
						current.ID.ToString()
					});
				}
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
		}
		arg.ReplyWith(arg.HasArg("--json") ? val.ToJson() : ((object)val).ToString());
	}

	public static uint GingerbreadMaterialID()
	{
		if (_gingerbreadMaterialID == 0)
		{
			_gingerbreadMaterialID = StringPool.Get("Gingerbread");
		}
		return _gingerbreadMaterialID;
	}

	[ServerVar]
	public static void ClearAllSprays()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		List<SprayCanSpray> list = Pool.GetList<SprayCanSpray>();
		Enumerator<SprayCanSpray> enumerator = SprayCanSpray.AllSprays.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				SprayCanSpray current = enumerator.Current;
				list.Add(current);
			}
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
		foreach (SprayCanSpray item in list)
		{
			item.Kill();
		}
		Pool.FreeList<SprayCanSpray>(ref list);
	}

	[ServerVar]
	public static void ClearAllSpraysByPlayer(Arg arg)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (!arg.HasArgs(1))
		{
			return;
		}
		ulong uLong = arg.GetULong(0, 0uL);
		List<SprayCanSpray> list = Pool.GetList<SprayCanSpray>();
		Enumerator<SprayCanSpray> enumerator = SprayCanSpray.AllSprays.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				SprayCanSpray current = enumerator.Current;
				if (current.sprayedByPlayer == uLong)
				{
					list.Add(current);
				}
			}
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
		foreach (SprayCanSpray item in list)
		{
			item.Kill();
		}
		int count = list.Count;
		Pool.FreeList<SprayCanSpray>(ref list);
		arg.ReplyWith($"Deleted {count} sprays by {uLong}");
	}

	[ServerVar]
	public static void ClearSpraysInRadius(Arg arg)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		BasePlayer basePlayer = arg.Player();
		if (!((Object)(object)basePlayer == (Object)null))
		{
			float @float = arg.GetFloat(0, 16f);
			int num = ClearSpraysInRadius(((Component)basePlayer).transform.position, @float);
			arg.ReplyWith($"Deleted {num} sprays within {@float} of {basePlayer.displayName}");
		}
	}

	private static int ClearSpraysInRadius(Vector3 position, float radius)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		List<SprayCanSpray> list = Pool.GetList<SprayCanSpray>();
		Enumerator<SprayCanSpray> enumerator = SprayCanSpray.AllSprays.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				SprayCanSpray current = enumerator.Current;
				if (current.Distance(position) <= radius)
				{
					list.Add(current);
				}
			}
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
		foreach (SprayCanSpray item in list)
		{
			item.Kill();
		}
		int count = list.Count;
		Pool.FreeList<SprayCanSpray>(ref list);
		return count;
	}

	[ServerVar]
	public static void ClearSpraysAtPositionInRadius(Arg arg)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		Vector3 vector = arg.GetVector3(0, default(Vector3));
		float @float = arg.GetFloat(1, 0f);
		if (@float != 0f)
		{
			int num = ClearSpraysInRadius(vector, @float);
			arg.ReplyWith($"Deleted {num} sprays within {@float} of {vector}");
		}
	}

	[ServerVar]
	public static void ClearDroppedItems()
	{
		List<DroppedItem> list = Pool.GetList<DroppedItem>();
		foreach (BaseNetworkable serverEntity in BaseNetworkable.serverEntities)
		{
			if (serverEntity is DroppedItem item)
			{
				list.Add(item);
			}
		}
		foreach (DroppedItem item2 in list)
		{
			item2.Kill();
		}
		Pool.FreeList<DroppedItem>(ref list);
	}
}
