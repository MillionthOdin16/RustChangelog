using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Facepunch;
using UnityEngine;

namespace ConVar;

[Factory("entity")]
public class Entity : ConsoleSystem
{
	private struct EntityInfo
	{
		public BaseNetworkable entity;

		public NetworkableId entityID;

		public uint groupID;

		public NetworkableId parentID;

		public string status;

		public EntityInfo(BaseNetworkable src)
		{
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
			entity = src;
			BaseEntity baseEntity = entity as BaseEntity;
			BaseEntity baseEntity2 = (((Object)(object)baseEntity != (Object)null) ? baseEntity.GetParentEntity() : null);
			NetworkableId val;
			NetworkableId val2;
			if (!((Object)(object)entity != (Object)null) || entity.net == null)
			{
				val = default(NetworkableId);
				val2 = val;
			}
			else
			{
				val2 = entity.net.ID;
			}
			entityID = val2;
			groupID = (((Object)(object)entity != (Object)null && entity.net != null && entity.net.group != null) ? entity.net.group.ID : 0u);
			NetworkableId val3;
			if (!((Object)(object)baseEntity != (Object)null))
			{
				val = default(NetworkableId);
				val3 = val;
			}
			else
			{
				val3 = baseEntity.parentEntity.uid;
			}
			parentID = val3;
			if ((Object)(object)baseEntity != (Object)null)
			{
				val = baseEntity.parentEntity.uid;
				if (((NetworkableId)(ref val)).IsValid)
				{
					if ((Object)(object)baseEntity2 == (Object)null)
					{
						status = "orphan";
					}
					else
					{
						status = "child";
					}
					return;
				}
			}
			status = string.Empty;
		}
	}

	private struct EntitySpawnRequest
	{
		public string PrefabName;

		public string Error;

		public bool Valid => string.IsNullOrEmpty(Error);
	}

	private static TextTable GetEntityTable(Func<EntityInfo, bool> filter)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		TextTable val = new TextTable();
		val.AddColumn("realm");
		val.AddColumn("entity");
		val.AddColumn("group");
		val.AddColumn("parent");
		val.AddColumn("name");
		val.AddColumn("position");
		val.AddColumn("local");
		val.AddColumn("rotation");
		val.AddColumn("local");
		val.AddColumn("status");
		val.AddColumn("invokes");
		Enumerator<BaseNetworkable> enumerator = BaseNetworkable.serverEntities.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				BaseNetworkable current = enumerator.Current;
				if (!((Object)(object)current == (Object)null))
				{
					EntityInfo arg = new EntityInfo(current);
					if (filter(arg))
					{
						string[] obj = new string[11]
						{
							"sv",
							arg.entityID.Value.ToString(),
							arg.groupID.ToString(),
							arg.parentID.Value.ToString(),
							arg.entity.ShortPrefabName,
							null,
							null,
							null,
							null,
							null,
							null
						};
						Vector3 val2 = ((Component)arg.entity).transform.position;
						obj[5] = ((object)(Vector3)(ref val2)).ToString();
						val2 = ((Component)arg.entity).transform.localPosition;
						obj[6] = ((object)(Vector3)(ref val2)).ToString();
						Quaternion val3 = ((Component)arg.entity).transform.rotation;
						val2 = ((Quaternion)(ref val3)).eulerAngles;
						obj[7] = ((object)(Vector3)(ref val2)).ToString();
						val3 = ((Component)arg.entity).transform.localRotation;
						val2 = ((Quaternion)(ref val3)).eulerAngles;
						obj[8] = ((object)(Vector3)(ref val2)).ToString();
						obj[9] = arg.status;
						obj[10] = arg.entity.InvokeString();
						val.AddRow(obj);
					}
				}
			}
			return val;
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
	}

	[ServerVar]
	[ClientVar]
	public static void find_entity(Arg args)
	{
		string filter = args.GetString(0, "");
		TextTable entityTable = GetEntityTable((EntityInfo info) => string.IsNullOrEmpty(filter) || info.entity.PrefabName.Contains(filter));
		args.ReplyWith(((object)entityTable).ToString());
	}

	[ServerVar]
	[ClientVar]
	public static void find_id(Arg args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		NetworkableId filter = args.GetEntityID(0);
		TextTable entityTable = GetEntityTable((EntityInfo info) => info.entityID == filter);
		args.ReplyWith(((object)entityTable).ToString());
	}

	[ServerVar]
	[ClientVar]
	public static void find_group(Arg args)
	{
		uint filter = args.GetUInt(0, 0u);
		TextTable entityTable = GetEntityTable((EntityInfo info) => info.groupID == filter);
		args.ReplyWith(((object)entityTable).ToString());
	}

	[ServerVar]
	[ClientVar]
	public static void find_parent(Arg args)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		NetworkableId filter = args.GetEntityID(0);
		TextTable entityTable = GetEntityTable((EntityInfo info) => info.parentID == filter);
		args.ReplyWith(((object)entityTable).ToString());
	}

	[ServerVar]
	[ClientVar]
	public static void find_status(Arg args)
	{
		string filter = args.GetString(0, "");
		TextTable entityTable = GetEntityTable((EntityInfo info) => string.IsNullOrEmpty(filter) || info.status.Contains(filter));
		args.ReplyWith(((object)entityTable).ToString());
	}

	[ServerVar]
	[ClientVar]
	public static void find_radius(Arg args)
	{
		BasePlayer player = args.Player();
		if (!((Object)(object)player == (Object)null))
		{
			uint filter = args.GetUInt(0, 10u);
			TextTable entityTable = GetEntityTable((EntityInfo info) => Vector3.Distance(((Component)info.entity).transform.position, ((Component)player).transform.position) <= (float)filter);
			args.ReplyWith(((object)entityTable).ToString());
		}
	}

	[ServerVar]
	[ClientVar]
	public static void find_self(Arg args)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		BasePlayer basePlayer = args.Player();
		if (!((Object)(object)basePlayer == (Object)null) && basePlayer.net != null)
		{
			NetworkableId filter = basePlayer.net.ID;
			TextTable entityTable = GetEntityTable((EntityInfo info) => info.entityID == filter);
			args.ReplyWith(((object)entityTable).ToString());
		}
	}

	[ServerVar]
	public static void debug_toggle(Arg args)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		NetworkableId entityID = args.GetEntityID(0);
		if (!((NetworkableId)(ref entityID)).IsValid)
		{
			return;
		}
		BaseEntity baseEntity = BaseNetworkable.serverEntities.Find(entityID) as BaseEntity;
		if (!((Object)(object)baseEntity == (Object)null))
		{
			baseEntity.SetFlag(BaseEntity.Flags.Debugging, !baseEntity.IsDebugging());
			if (baseEntity.IsDebugging())
			{
				baseEntity.OnDebugStart();
			}
			args.ReplyWith(string.Concat("Debugging for ", baseEntity.net.ID, " ", baseEntity.IsDebugging() ? "enabled" : "disabled"));
		}
	}

	[ServerVar]
	public static void nudge(Arg args)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		NetworkableId entityID = args.GetEntityID(0);
		if (((NetworkableId)(ref entityID)).IsValid)
		{
			BaseEntity baseEntity = BaseNetworkable.serverEntities.Find(entityID) as BaseEntity;
			if (!((Object)(object)baseEntity == (Object)null))
			{
				((Component)baseEntity).BroadcastMessage("DebugNudge", (SendMessageOptions)1);
			}
		}
	}

	private static EntitySpawnRequest GetSpawnEntityFromName(string name)
	{
		EntitySpawnRequest result;
		if (string.IsNullOrEmpty(name))
		{
			result = default(EntitySpawnRequest);
			result.Error = "No entity name provided";
			return result;
		}
		string[] array = (from x in GameManifest.Current.entities
			where StringEx.Contains(Path.GetFileNameWithoutExtension(x), name, CompareOptions.IgnoreCase)
			select x.ToLower()).ToArray();
		if (array.Length == 0)
		{
			result = default(EntitySpawnRequest);
			result.Error = "Entity type not found";
			return result;
		}
		if (array.Length > 1)
		{
			string text = array.FirstOrDefault((string x) => string.Compare(Path.GetFileNameWithoutExtension(x), name, StringComparison.OrdinalIgnoreCase) == 0);
			if (text == null)
			{
				result = default(EntitySpawnRequest);
				result.Error = "Unknown entity - could be:\n\n" + string.Join("\n", array.Select(Path.GetFileNameWithoutExtension).ToArray());
				return result;
			}
			array[0] = text;
		}
		result = default(EntitySpawnRequest);
		result.PrefabName = array[0];
		return result;
	}

	[ServerVar(Name = "spawn")]
	public static string svspawn(string name, Vector3 pos, Vector3 dir)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		BasePlayer arg = ConsoleSystem.CurrentArgs.Player();
		EntitySpawnRequest spawnEntityFromName = GetSpawnEntityFromName(name);
		if (!spawnEntityFromName.Valid)
		{
			return spawnEntityFromName.Error;
		}
		BaseEntity baseEntity = GameManager.server.CreateEntity(spawnEntityFromName.PrefabName, pos, Quaternion.LookRotation(dir, Vector3.up));
		if ((Object)(object)baseEntity == (Object)null)
		{
			Debug.Log((object)$"{arg} failed to spawn \"{spawnEntityFromName.PrefabName}\" (tried to spawn \"{name}\")");
			return "Couldn't spawn " + name;
		}
		BasePlayer basePlayer = baseEntity as BasePlayer;
		if ((Object)(object)basePlayer != (Object)null)
		{
			Quaternion val = Quaternion.LookRotation(dir, Vector3.up);
			basePlayer.OverrideViewAngles(((Quaternion)(ref val)).eulerAngles);
		}
		baseEntity.Spawn();
		Debug.Log((object)$"{arg} spawned \"{baseEntity}\" at {pos}");
		return string.Concat("spawned ", baseEntity, " at ", pos);
	}

	[ServerVar(Name = "spawnitem")]
	public static string svspawnitem(string name, Vector3 pos)
	{
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		BasePlayer arg = ConsoleSystem.CurrentArgs.Player();
		if (string.IsNullOrEmpty(name))
		{
			return "No entity name provided";
		}
		string[] array = (from x in ItemManager.itemList
			select x.shortname into x
			where StringEx.Contains(x, name, CompareOptions.IgnoreCase)
			select x).ToArray();
		if (array.Length == 0)
		{
			return "Entity type not found";
		}
		if (array.Length > 1)
		{
			string text = array.FirstOrDefault((string x) => string.Compare(x, name, StringComparison.OrdinalIgnoreCase) == 0);
			if (text == null)
			{
				Debug.Log((object)$"{arg} failed to spawn \"{name}\"");
				return "Unknown entity - could be:\n\n" + string.Join("\n", array);
			}
			array[0] = text;
		}
		Item item = ItemManager.CreateByName(array[0], 1, 0uL);
		if (item == null)
		{
			Debug.Log((object)$"{arg} failed to spawn \"{array[0]}\" (tried to spawnitem \"{name}\")");
			return "Couldn't spawn " + name;
		}
		BaseEntity arg2 = item.CreateWorldObject(pos);
		Debug.Log((object)$"{arg} spawned \"{arg2}\" at {pos} (via spawnitem)");
		return string.Concat("spawned ", item, " at ", pos);
	}

	[ServerVar(Name = "spawngrid")]
	public static string svspawngrid(string name, int width = 5, int height = 5, int spacing = 5)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		BasePlayer basePlayer = ConsoleSystem.CurrentArgs.Player();
		EntitySpawnRequest spawnEntityFromName = GetSpawnEntityFromName(name);
		if (!spawnEntityFromName.Valid)
		{
			return spawnEntityFromName.Error;
		}
		Quaternion rotation = ((Component)basePlayer).transform.rotation;
		((Quaternion)(ref rotation)).eulerAngles = new Vector3(0f, ((Quaternion)(ref rotation)).eulerAngles.y, 0f);
		Matrix4x4 val = Matrix4x4.TRS(((Component)basePlayer).transform.position, ((Component)basePlayer).transform.rotation, Vector3.one);
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				Vector3 pos = ((Matrix4x4)(ref val)).MultiplyPoint(new Vector3((float)(i * spacing), 0f, (float)(j * spacing)));
				BaseEntity baseEntity = GameManager.server.CreateEntity(spawnEntityFromName.PrefabName, pos, rotation);
				if ((Object)(object)baseEntity == (Object)null)
				{
					Debug.Log((object)$"{basePlayer} failed to spawn \"{spawnEntityFromName.PrefabName}\" (tried to spawn \"{name}\")");
					return "Couldn't spawn " + name;
				}
				baseEntity.Spawn();
			}
		}
		Debug.Log((object)($"{basePlayer} spawned ({width * height}) " + spawnEntityFromName.PrefabName));
		return $"spawned ({width * height}) " + spawnEntityFromName.PrefabName;
	}

	[ServerVar]
	public static void spawnlootfrom(Arg args)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		BasePlayer basePlayer = args.Player();
		string @string = args.GetString(0, string.Empty);
		int @int = args.GetInt(1, 1);
		Vector3 vector = args.GetVector3(1, Object.op_Implicit((Object)(object)basePlayer) ? basePlayer.CenterPoint() : Vector3.zero);
		if (string.IsNullOrEmpty(@string))
		{
			return;
		}
		BaseEntity baseEntity = GameManager.server.CreateEntity(@string, vector);
		if ((Object)(object)baseEntity == (Object)null)
		{
			return;
		}
		baseEntity.Spawn();
		basePlayer.ChatMessage("Contents of " + @string + " spawned " + @int + " times");
		LootContainer component = ((Component)baseEntity).GetComponent<LootContainer>();
		if ((Object)(object)component != (Object)null)
		{
			for (int i = 0; i < @int * component.maxDefinitionsToSpawn; i++)
			{
				component.lootDefinition.SpawnIntoContainer(basePlayer.inventory.containerMain);
			}
		}
		baseEntity.Kill();
	}

	public static int DeleteBy(ulong id)
	{
		List<ulong> list = Pool.GetList<ulong>();
		list.Add(id);
		int result = DeleteBy(list);
		Pool.FreeList<ulong>(ref list);
		return result;
	}

	[ServerVar(Help = "Destroy all entities created by provided users (separate users by space)")]
	public static int DeleteBy(Arg arg)
	{
		if (!arg.HasArgs(1))
		{
			return 0;
		}
		List<ulong> list = Pool.GetList<ulong>();
		string[] args = arg.Args;
		for (int i = 0; i < args.Length; i++)
		{
			if (ulong.TryParse(args[i], out var result))
			{
				list.Add(result);
			}
		}
		int result2 = DeleteBy(list);
		Pool.FreeList<ulong>(ref list);
		return result2;
	}

	private static int DeleteBy(List<ulong> ids)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		Enumerator<BaseNetworkable> enumerator = BaseNetworkable.serverEntities.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				BaseEntity baseEntity = (BaseEntity)enumerator.Current;
				if ((Object)(object)baseEntity == (Object)null)
				{
					continue;
				}
				bool flag = false;
				foreach (ulong id in ids)
				{
					if (baseEntity.OwnerID == id)
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					((FacepunchBehaviour)baseEntity).Invoke((Action)baseEntity.KillMessage, (float)num * 0.2f);
					num++;
				}
			}
			return num;
		}
		finally
		{
			((IDisposable)enumerator).Dispose();
		}
	}

	[ServerVar(Help = "Destroy all entities created by users in the provided text block (can use with copied results from ent auth)")]
	public static void DeleteByTextBlock(Arg arg)
	{
		if (arg.Args.Length != 1)
		{
			arg.ReplyWith("Invalid arguments, provide a text block surrounded by \" and listing player id's at the start of each line");
			return;
		}
		MatchCollection matchCollection = Regex.Matches(arg.GetString(0, ""), "^\\b\\d{17}", RegexOptions.Multiline);
		List<ulong> list = Pool.GetList<ulong>();
		foreach (Match item in matchCollection)
		{
			if (ulong.TryParse(item.Value, out var result))
			{
				list.Add(result);
			}
		}
		int num = DeleteBy(list);
		Pool.FreeList<ulong>(ref list);
		arg.ReplyWith($"Destroyed {num} entities");
	}
}
