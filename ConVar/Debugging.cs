using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using Facepunch;
using Facepunch.Unity;
using Rust;
using UnityEngine;

namespace ConVar;

[Factory("debug")]
public class Debugging : ConsoleSystem
{
	[ServerVar]
	[ClientVar]
	public static bool checktriggers = false;

	[ServerVar]
	public static bool checkparentingtriggers = true;

	[ServerVar]
	[ClientVar(Saved = false, Help = "Shows some debug info for dismount attempts.")]
	public static bool DebugDismounts = false;

	[ServerVar(Help = "Do not damage any items")]
	public static bool disablecondition = false;

	[ClientVar]
	[ServerVar]
	public static bool callbacks = false;

	[ServerVar]
	[ClientVar]
	public static bool log
	{
		get
		{
			return Debug.unityLogger.logEnabled;
		}
		set
		{
			Debug.unityLogger.logEnabled = value;
		}
	}

	[ServerVar]
	[ClientVar]
	public static void renderinfo(Arg arg)
	{
		RenderInfo.GenerateReport();
	}

	[ServerVar]
	public static void enable_player_movement(Arg arg)
	{
		if (arg.IsAdmin)
		{
			bool @bool = arg.GetBool(0, true);
			BasePlayer basePlayer = arg.Player();
			if ((Object)(object)basePlayer == (Object)null)
			{
				arg.ReplyWith("Must be called from client with player model");
				return;
			}
			basePlayer.ClientRPCPlayer(null, basePlayer, "TogglePlayerMovement", @bool);
			arg.ReplyWith((@bool ? "enabled" : "disabled") + " player movement");
		}
	}

	[ClientVar]
	[ServerVar]
	public static void stall(Arg arg)
	{
		float num = Mathf.Clamp(arg.GetFloat(0, 0f), 0f, 1f);
		arg.ReplyWith("Stalling for " + num + " seconds...");
		Thread.Sleep(Mathf.RoundToInt(num * 1000f));
	}

	[ServerVar(Help = "Repair all items in inventory")]
	public static void repair_inventory(Arg args)
	{
		BasePlayer basePlayer = args.Player();
		if (!Object.op_Implicit((Object)(object)basePlayer))
		{
			return;
		}
		Item[] array = basePlayer.inventory.AllItems();
		foreach (Item item in array)
		{
			if (item != null)
			{
				item.maxCondition = item.info.condition.max;
				item.condition = item.maxCondition;
				item.MarkDirty();
			}
			if (item.contents == null)
			{
				continue;
			}
			foreach (Item item2 in item.contents.itemList)
			{
				item2.maxCondition = item2.info.condition.max;
				item2.condition = item2.maxCondition;
				item2.MarkDirty();
			}
		}
	}

	[ServerVar]
	public static void spawnParachuteTester(Arg arg)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		float @float = arg.GetFloat(0, 50f);
		BasePlayer basePlayer = arg.Player();
		BasePlayer basePlayer2 = GameManager.server.CreateEntity("assets/prefabs/player/player.prefab", ((Component)basePlayer).transform.position + Vector3.up * @float, Quaternion.LookRotation(basePlayer.eyes.BodyForward())) as BasePlayer;
		basePlayer2.Spawn();
		basePlayer2.eyes.rotation = basePlayer.eyes.rotation;
		basePlayer2.SendNetworkUpdate();
		Inventory.copyTo(basePlayer, basePlayer2);
		if (!basePlayer2.HasValidParachuteEquipped())
		{
			basePlayer2.inventory.containerWear.GiveItem(ItemManager.CreateByName("parachute", 1, 0uL));
		}
		basePlayer2.RequestParachuteDeploy();
	}

	[ServerVar]
	public static void deleteEntitiesByShortname(Arg arg)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		string text = arg.GetString(0, "").ToLower();
		float @float = arg.GetFloat(1, 0f);
		BasePlayer basePlayer = arg.Player();
		List<BaseNetworkable> list = Pool.GetList<BaseNetworkable>();
		Enumerator<BaseNetworkable> enumerator = BaseNetworkable.serverEntities.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				BaseNetworkable current = enumerator.Current;
				if (current.ShortPrefabName == text && (@float == 0f || ((Object)(object)basePlayer != (Object)null && basePlayer.Distance(current as BaseEntity) <= @float)))
				{
					list.Add(current);
				}
			}
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
		Debug.Log((object)$"Deleting {list.Count} {text}...");
		foreach (BaseNetworkable item in list)
		{
			item.Kill();
		}
		Pool.FreeList<BaseNetworkable>(ref list);
	}

	[ServerVar(Help = "Takes you in and out of your current network group, causing you to delete and then download all entities in your PVS again")]
	public static void flushgroup(Arg arg)
	{
		BasePlayer basePlayer = arg.Player();
		if (!((Object)(object)basePlayer == (Object)null))
		{
			basePlayer.net.SwitchGroup(BaseNetworkable.LimboNetworkGroup);
			basePlayer.UpdateNetworkGroup();
		}
	}

	[ServerVar(Help = "Break the current held object")]
	public static void breakheld(Arg arg)
	{
		Item activeItem = arg.Player().GetActiveItem();
		activeItem?.LoseCondition(activeItem.condition * 2f);
	}

	[ServerVar(Help = "reset all puzzles")]
	public static void puzzlereset(Arg arg)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		if (!((Object)(object)arg.Player() == (Object)null))
		{
			PuzzleReset[] array = Object.FindObjectsOfType<PuzzleReset>();
			Debug.Log((object)"iterating...");
			PuzzleReset[] array2 = array;
			foreach (PuzzleReset puzzleReset in array2)
			{
				Vector3 position = ((Component)puzzleReset).transform.position;
				Debug.Log((object)("resetting puzzle at :" + ((object)(Vector3)(ref position)).ToString()));
				puzzleReset.DoReset();
				puzzleReset.ResetTimer();
			}
		}
	}

	[ServerVar(EditorOnly = true, Help = "respawn all puzzles from their prefabs")]
	public static void puzzleprefabrespawn(Arg arg)
	{
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		foreach (BaseNetworkable item in BaseNetworkable.serverEntities.Where((BaseNetworkable x) => x is IOEntity && PrefabAttribute.server.Find<Construction>(x.prefabID) == null).ToList())
		{
			item.Kill();
		}
		foreach (MonumentInfo monument in TerrainMeta.Path.Monuments)
		{
			GameObject val = GameManager.server.FindPrefab(((Object)((Component)monument).gameObject).name);
			if ((Object)(object)val == (Object)null)
			{
				continue;
			}
			Dictionary<IOEntity, IOEntity> dictionary = new Dictionary<IOEntity, IOEntity>();
			IOEntity[] componentsInChildren = val.GetComponentsInChildren<IOEntity>(true);
			foreach (IOEntity iOEntity in componentsInChildren)
			{
				Quaternion rot = ((Component)monument).transform.rotation * ((Component)iOEntity).transform.rotation;
				Vector3 pos = ((Component)monument).transform.TransformPoint(((Component)iOEntity).transform.position);
				BaseEntity newEntity = GameManager.server.CreateEntity(iOEntity.PrefabName, pos, rot);
				IOEntity iOEntity2 = newEntity as IOEntity;
				if (!((Object)(object)iOEntity2 != (Object)null))
				{
					continue;
				}
				dictionary.Add(iOEntity, iOEntity2);
				DoorManipulator doorManipulator = newEntity as DoorManipulator;
				if ((Object)(object)doorManipulator != (Object)null)
				{
					List<Door> list = Pool.GetList<Door>();
					global::Vis.Entities(((Component)newEntity).transform.position, 10f, list, -1, (QueryTriggerInteraction)2);
					Door door = list.OrderBy((Door x) => x.Distance(((Component)newEntity).transform.position)).FirstOrDefault();
					if ((Object)(object)door != (Object)null)
					{
						doorManipulator.targetDoor = door;
					}
					Pool.FreeList<Door>(ref list);
				}
				CardReader cardReader = newEntity as CardReader;
				if ((Object)(object)cardReader != (Object)null)
				{
					CardReader cardReader2 = iOEntity as CardReader;
					if ((Object)(object)cardReader2 != (Object)null)
					{
						cardReader.accessLevel = cardReader2.accessLevel;
						cardReader.accessDuration = cardReader2.accessDuration;
					}
				}
				TimerSwitch timerSwitch = newEntity as TimerSwitch;
				if ((Object)(object)timerSwitch != (Object)null)
				{
					TimerSwitch timerSwitch2 = iOEntity as TimerSwitch;
					if ((Object)(object)timerSwitch2 != (Object)null)
					{
						timerSwitch.timerLength = timerSwitch2.timerLength;
					}
				}
			}
			foreach (KeyValuePair<IOEntity, IOEntity> item2 in dictionary)
			{
				IOEntity key = item2.Key;
				IOEntity value = item2.Value;
				for (int j = 0; j < key.outputs.Length; j++)
				{
					if (!((Object)(object)key.outputs[j].connectedTo.ioEnt == (Object)null))
					{
						value.outputs[j].connectedTo.ioEnt = dictionary[key.outputs[j].connectedTo.ioEnt];
						value.outputs[j].connectedToSlot = key.outputs[j].connectedToSlot;
					}
				}
			}
			foreach (IOEntity value2 in dictionary.Values)
			{
				value2.Spawn();
			}
		}
	}

	[ServerVar(Help = "Break all the items in your inventory whose name match the passed string")]
	public static void breakitem(Arg arg)
	{
		string @string = arg.GetString(0, "");
		foreach (Item item in arg.Player().inventory.containerMain.itemList)
		{
			if (StringEx.Contains(item.info.shortname, @string, CompareOptions.IgnoreCase) && item.hasCondition)
			{
				item.LoseCondition(item.condition * 2f);
			}
		}
	}

	[ServerVar]
	public static void refillvitals(Arg arg)
	{
		AdjustHealth(arg.Player(), 1000f);
		AdjustCalories(arg.Player(), 1000f);
		AdjustHydration(arg.Player(), 1000f);
		AdjustRadiation(arg.Player(), -10000f);
	}

	[ServerVar]
	public static void heal(Arg arg)
	{
		AdjustHealth(arg.Player(), arg.GetInt(0, 1));
	}

	[ServerVar]
	public static void hurt(Arg arg)
	{
		AdjustHealth(arg.Player(), -arg.GetInt(0, 1), arg.GetString(1, string.Empty));
	}

	[ServerVar]
	public static void eat(Arg arg)
	{
		AdjustCalories(arg.Player(), arg.GetInt(0, 1), arg.GetInt(1, 1));
	}

	[ServerVar]
	public static void drink(Arg arg)
	{
		AdjustHydration(arg.Player(), arg.GetInt(0, 1), arg.GetInt(1, 1));
	}

	[ServerVar]
	public static void sethealth(Arg arg)
	{
		if (!arg.HasArgs(1))
		{
			arg.ReplyWith("Please enter an amount.");
			return;
		}
		int @int = arg.GetInt(0, 0);
		BasePlayer usePlayer = GetUsePlayer(arg, 1);
		if (Object.op_Implicit((Object)(object)usePlayer))
		{
			usePlayer.SetHealth(@int);
		}
	}

	[ServerVar]
	public static void setdamage(Arg arg)
	{
		BasePlayer basePlayer = arg.Player();
		if (!arg.HasArgs(1))
		{
			arg.ReplyWith("Please enter an amount.");
			return;
		}
		int @int = arg.GetInt(0, 0);
		BasePlayer usePlayer = GetUsePlayer(arg, 1);
		if (Object.op_Implicit((Object)(object)usePlayer))
		{
			float damageAmount = usePlayer.health - (float)@int;
			HitInfo info = new HitInfo(basePlayer, basePlayer, DamageType.Bullet, damageAmount);
			usePlayer.OnAttacked(info);
		}
	}

	[ServerVar]
	public static void setfood(Arg arg)
	{
		setattribute(arg, MetabolismAttribute.Type.Calories);
	}

	[ServerVar]
	public static void setwater(Arg arg)
	{
		setattribute(arg, MetabolismAttribute.Type.Hydration);
	}

	[ServerVar]
	public static void setradiation(Arg arg)
	{
		setattribute(arg, MetabolismAttribute.Type.Radiation);
	}

	private static void AdjustHealth(BasePlayer player, float amount, string bone = null)
	{
		HitInfo hitInfo = new HitInfo(player, player, DamageType.Bullet, 0f - amount);
		if (!string.IsNullOrEmpty(bone))
		{
			hitInfo.HitBone = StringPool.Get(bone);
		}
		player.OnAttacked(hitInfo);
	}

	private static void AdjustCalories(BasePlayer player, float amount, float time = 1f)
	{
		player.metabolism.ApplyChange(MetabolismAttribute.Type.Calories, amount, time);
	}

	private static void AdjustHydration(BasePlayer player, float amount, float time = 1f)
	{
		player.metabolism.ApplyChange(MetabolismAttribute.Type.Hydration, amount, time);
	}

	private static void AdjustRadiation(BasePlayer player, float amount, float time = 1f)
	{
		player.metabolism.SetAttribute(MetabolismAttribute.Type.Radiation, amount);
	}

	private static void setattribute(Arg arg, MetabolismAttribute.Type type)
	{
		if (!arg.HasArgs(1))
		{
			arg.ReplyWith("Please enter an amount.");
			return;
		}
		int @int = arg.GetInt(0, 0);
		BasePlayer usePlayer = GetUsePlayer(arg, 1);
		if (Object.op_Implicit((Object)(object)usePlayer))
		{
			usePlayer.metabolism.SetAttribute(type, @int);
		}
	}

	private static BasePlayer GetUsePlayer(Arg arg, int playerArgument)
	{
		BasePlayer basePlayer = null;
		if (arg.HasArgs(playerArgument + 1))
		{
			BasePlayer player = arg.GetPlayer(playerArgument);
			if (!Object.op_Implicit((Object)(object)player))
			{
				return null;
			}
			return player;
		}
		return arg.Player();
	}

	[ServerVar]
	public static void ResetSleepingBagTimers(Arg arg)
	{
		SleepingBag.ResetTimersForPlayer(arg.Player());
	}

	[ServerVar(Help = "Spawn lots of IO entities to lag the server")]
	public static void bench_io(Arg arg)
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		BasePlayer basePlayer = arg.Player();
		if ((Object)(object)basePlayer == (Object)null || !basePlayer.IsAdmin)
		{
			return;
		}
		int @int = arg.GetInt(0, 50);
		string name = arg.GetString(1, "water_catcher_small");
		List<IOEntity> list = new List<IOEntity>();
		WaterCatcher waterCatcher = null;
		Vector3 position = ((Component)arg.Player()).transform.position;
		string[] array = (from x in GameManifest.Current.entities
			where StringEx.Contains(Path.GetFileNameWithoutExtension(x), name, CompareOptions.IgnoreCase)
			select x.ToLower()).ToArray();
		if (array.Length == 0)
		{
			arg.ReplyWith("Couldn't find io prefab \"" + array[0] + "\"");
			return;
		}
		if (array.Length > 1)
		{
			string text = array.FirstOrDefault((string x) => string.Compare(Path.GetFileNameWithoutExtension(x), name, StringComparison.OrdinalIgnoreCase) == 0);
			if (text == null)
			{
				Debug.Log((object)$"{arg} failed to find io entity \"{name}\"");
				arg.ReplyWith("Unknown entity - could be:\n\n" + string.Join("\n", array.Select(Path.GetFileNameWithoutExtension).ToArray()));
				return;
			}
			array[0] = text;
		}
		for (int i = 0; i < @int; i++)
		{
			Vector3 pos = position + new Vector3((float)(i * 5), 0f, 0f);
			Quaternion identity = Quaternion.identity;
			BaseEntity baseEntity = GameManager.server.CreateEntity(array[0], pos, identity);
			if (!Object.op_Implicit((Object)(object)baseEntity))
			{
				continue;
			}
			baseEntity.Spawn();
			WaterCatcher component = ((Component)baseEntity).GetComponent<WaterCatcher>();
			if (Object.op_Implicit((Object)(object)component))
			{
				list.Add(component);
				if ((Object)(object)waterCatcher != (Object)null)
				{
					Connect(waterCatcher, component);
				}
				if (i == @int - 1)
				{
					Connect(component, list.First());
				}
				waterCatcher = component;
			}
		}
		static void Connect(IOEntity InputIOEnt, IOEntity OutputIOEnt)
		{
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_008c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			int num = 0;
			int num2 = 0;
			WireTool.WireColour wireColour = WireTool.WireColour.Default;
			IOEntity.IOSlot iOSlot = InputIOEnt.inputs[num];
			IOEntity.IOSlot obj = OutputIOEnt.outputs[num2];
			iOSlot.connectedTo.Set(OutputIOEnt);
			iOSlot.connectedToSlot = num2;
			iOSlot.wireColour = wireColour;
			iOSlot.connectedTo.Init();
			obj.connectedTo.Set(InputIOEnt);
			obj.connectedToSlot = num;
			obj.wireColour = wireColour;
			obj.connectedTo.Init();
			obj.linePoints = (Vector3[])(object)new Vector3[2]
			{
				Vector3.zero,
				((Component)OutputIOEnt).transform.InverseTransformPoint(((Component)InputIOEnt).transform.TransformPoint(iOSlot.handlePosition))
			};
			OutputIOEnt.MarkDirtyForceUpdateOutputs();
			OutputIOEnt.SendNetworkUpdate();
			InputIOEnt.SendNetworkUpdate();
			OutputIOEnt.SendChangedToRoot(forceUpdate: true);
		}
	}
}
