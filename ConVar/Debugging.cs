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
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		BasePlayer basePlayer = arg.Player();
		if (!((Object)(object)basePlayer == (Object)null))
		{
			PuzzleReset[] array = Object.FindObjectsOfType<PuzzleReset>();
			Debug.Log((object)"iterating...");
			PuzzleReset[] array2 = array;
			foreach (PuzzleReset puzzleReset in array2)
			{
				Debug.Log((object)("resetting puzzle at :" + ((Component)puzzleReset).transform.position));
				puzzleReset.DoReset();
				puzzleReset.ResetTimer();
			}
		}
	}

	[ServerVar(EditorOnly = true, Help = "respawn all puzzles from their prefabs")]
	public static void puzzleprefabrespawn(Arg arg)
	{
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
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

	[ServerVar]
	public static void ResetSleepingBagTimers(Arg arg)
	{
		BasePlayer player = arg.Player();
		SleepingBag.ResetTimersForPlayer(player);
	}

	[ServerVar(Help = "Spawn lots of IO entities to lag the server")]
	public static void bench_io(Arg arg)
	{
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
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
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_0099: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			int num = 0;
			int num2 = 0;
			WireTool.WireColour wireColour = WireTool.WireColour.Default;
			IOEntity.IOSlot iOSlot = InputIOEnt.inputs[num];
			IOEntity.IOSlot iOSlot2 = OutputIOEnt.outputs[num2];
			iOSlot.connectedTo.Set(OutputIOEnt);
			iOSlot.connectedToSlot = num2;
			iOSlot.wireColour = wireColour;
			iOSlot.connectedTo.Init();
			iOSlot2.connectedTo.Set(InputIOEnt);
			iOSlot2.connectedToSlot = num;
			iOSlot2.wireColour = wireColour;
			iOSlot2.connectedTo.Init();
			iOSlot2.linePoints = (Vector3[])(object)new Vector3[2]
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
