using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using ConVar;
using Network;
using Newtonsoft.Json;
using ProtoBuf;
using Rust;
using Steamworks;
using UnityEngine;

namespace Facepunch.Rust;

public static class Analytics
{
	public static class Azure
	{
		private struct EntitySumItem
		{
			public uint PrefabId;

			public int Count;

			public int Grade;
		}

		private struct EntityKey : IEquatable<EntityKey>
		{
			public uint PrefabId;

			public int Grade;

			public bool Equals(EntityKey other)
			{
				return PrefabId == other.PrefabId && Grade == other.Grade;
			}

			public override int GetHashCode()
			{
				int num = 17;
				num = num * 23 + PrefabId.GetHashCode();
				return num * 31 + Grade.GetHashCode();
			}
		}

		private class PendingItemsData : IPooled
		{
			public PendingItemsKey Key;

			public int amount;

			public string category;

			public void EnterPool()
			{
				Key = default(PendingItemsKey);
				amount = 0;
				category = null;
			}

			public void LeavePool()
			{
			}
		}

		private struct PendingItemsKey : IEquatable<PendingItemsKey>
		{
			public string Item;

			public bool Consumed;

			public string Entity;

			public string Category;

			public NetworkableId EntityId;

			public bool Equals(PendingItemsKey other)
			{
				//IL_0028: Unknown result type (might be due to invalid IL or missing references)
				//IL_002e: Unknown result type (might be due to invalid IL or missing references)
				return Item == other.Item && Entity == other.Entity && EntityId == other.EntityId && Consumed == other.Consumed && Category == other.Category;
			}

			public override int GetHashCode()
			{
				int num = 17;
				num = num * 23 + Item.GetHashCode();
				num = num * 31 + Consumed.GetHashCode();
				num = num * 37 + Entity.GetHashCode();
				num = num * 47 + Category.GetHashCode();
				return num * 53 + ((object)(NetworkableId)(ref EntityId)).GetHashCode();
			}
		}

		private class PlayerAggregate : IPooled
		{
			public string UserId;

			public Vector3 Position;

			public Vector3 Direction;

			public List<string> Hotbar = new List<string>();

			public List<string> Worn = new List<string>();

			public string ActiveItem;

			public void EnterPool()
			{
				//IL_000e: Unknown result type (might be due to invalid IL or missing references)
				//IL_001a: Unknown result type (might be due to invalid IL or missing references)
				UserId = null;
				Position = default(Vector3);
				Direction = default(Vector3);
				Hotbar.Clear();
				Worn.Clear();
				ActiveItem = null;
			}

			public void LeavePool()
			{
			}
		}

		private class TeamInfo : IPooled
		{
			public List<string> online = new List<string>();

			public List<string> offline = new List<string>();

			public int member_count;

			public void EnterPool()
			{
				online.Clear();
				offline.Clear();
				member_count = 0;
			}

			public void LeavePool()
			{
			}
		}

		public enum ResourceMode
		{
			Produced,
			Consumed
		}

		private static class EventIds
		{
			public const string EntityBuilt = "entity_built";

			public const string EntityPickup = "entity_pickup";

			public const string EntityDamage = "entity_damage";

			public const string PlayerRespawn = "player_respawn";

			public const string ExplosiveLaunched = "explosive_launch";

			public const string Explosion = "explosion";

			public const string ItemEvent = "item_event";

			public const string EntitySum = "entity_sum";

			public const string ItemSum = "item_sum";

			public const string ItemDespawn = "item_despawn";

			public const string ItemDropped = "item_drop";

			public const string ItemPickup = "item_pickup";

			public const string AntihackViolation = "antihack_violation";

			public const string AntihackViolationDetailed = "antihack_violation_detailed";

			public const string PlayerConnect = "player_connect";

			public const string PlayerDisconnect = "player_disconnect";

			public const string ConsumableUsed = "consumeable_used";

			public const string MedUsed = "med_used";

			public const string ResearchStarted = "research_start";

			public const string BlueprintLearned = "blueprint_learned";

			public const string TeamChanged = "team_change";

			public const string EntityAuthChange = "auth_change";

			public const string VendingOrderChanged = "vending_changed";

			public const string VendingSale = "vending_sale";

			public const string ChatMessage = "chat";

			public const string BlockUpgrade = "block_upgrade";

			public const string BlockDemolish = "block_demolish";

			public const string ItemRepair = "item_repair";

			public const string EntityRepair = "entity_repair";

			public const string ItemSkinned = "item_skinned";

			public const string ItemAggregate = "item_aggregate";

			public const string CodelockChanged = "code_change";

			public const string CodelockEntered = "code_enter";

			public const string SleepingBagAssign = "sleeping_bag_assign";

			public const string FallDamage = "fall_damage";

			public const string PlayerWipeIdSet = "player_wipe_id_set";

			public const string ServerInfo = "server_info";

			public const string UnderwaterCrateUntied = "crate_untied";

			public const string VehiclePurchased = "vehicle_purchase";

			public const string NPCVendor = "npc_vendor";

			public const string BlueprintsOnline = "blueprint_aggregate_online";

			public const string PlayerPositions = "player_positions";

			public const string ProjectileInvalid = "projectile_invalid";

			public const string ItemDefinitions = "item_definitions";

			public const string KeycardSwiped = "keycard_swiped";

			public const string EntitySpawned = "entity_spawned";

			public const string EntityKilled = "entity_killed";

			public const string HackableCrateStarted = "hackable_crate_started";

			public const string HackableCrateEnded = "hackable_crate_ended";

			public const string StashHidden = "stash_hidden";

			public const string StashRevealed = "stash_reveal";

			public const string EntityManifest = "entity_manifest";

			public const string LootEntity = "loot_entity";

			public const string OnlineTeams = "online_teams";

			public const string Gambling = "gambing";

			public const string BuildingBlockColor = "building_block_color";

			public const string MissionComplete = "mission_complete";

			public const string PlayerPinged = "player_pinged";

			public const string BagUnclaim = "bag_unclaim";

			public const string SteamAuth = "steam_auth";
		}

		private struct SimpleItemAmount
		{
			public string ItemName;

			public int Amount;

			public ulong Skin;

			public float Condition;

			public SimpleItemAmount(Item item)
			{
				ItemName = item.info.shortname;
				Amount = item.amount;
				Skin = item.skin;
				Condition = item.conditionNormalized;
			}
		}

		private struct FiredProjectileKey : IEquatable<FiredProjectileKey>
		{
			public ulong UserId;

			public int ProjectileId;

			public FiredProjectileKey(ulong userId, int projectileId)
			{
				UserId = userId;
				ProjectileId = projectileId;
			}

			public bool Equals(FiredProjectileKey other)
			{
				return other.UserId == UserId && other.ProjectileId == ProjectileId;
			}
		}

		private class PendingFiredProjectile : IPooled
		{
			public EventRecord Record;

			public BasePlayer.FiredProjectile FiredProjectile;

			public bool Hit;

			public void EnterPool()
			{
				Hit = false;
				Record = null;
				FiredProjectile = null;
			}

			public void LeavePool()
			{
			}
		}

		private static Dictionary<int, string> geneCache = new Dictionary<int, string>();

		public static int MaxMSPerFrame = 5;

		private static Dictionary<PendingItemsKey, PendingItemsData> pendingItems = new Dictionary<PendingItemsKey, PendingItemsData>();

		private static Dictionary<FiredProjectileKey, PendingFiredProjectile> firedProjectiles = new Dictionary<FiredProjectileKey, PendingFiredProjectile>();

		public static bool Stats => (!string.IsNullOrEmpty(AnalyticsSecret) || ConVar.Server.official) && ConVar.Server.stats;

		private static string GetGenesAsString(GrowableEntity plant)
		{
			int key = GrowableGeneEncoding.EncodeGenesToInt(plant.Genes);
			if (!geneCache.TryGetValue(key, out var value))
			{
				return string.Join("", from x in plant.Genes.Genes
					group x by x.GetDisplayCharacter() into x
					orderby x.Key
					select x.Count() + x.Key);
			}
			return value;
		}

		private static string GetMonument(BaseEntity entity)
		{
			//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
			if ((Object)(object)entity == (Object)null)
			{
				return null;
			}
			SpawnGroup spawnGroup = null;
			if (entity is BaseCorpse baseCorpse)
			{
				spawnGroup = baseCorpse.spawnGroup;
			}
			if ((Object)(object)spawnGroup == (Object)null)
			{
				SpawnPointInstance component = ((Component)entity).GetComponent<SpawnPointInstance>();
				if ((Object)(object)component != (Object)null)
				{
					spawnGroup = component.parentSpawnPointUser as SpawnGroup;
				}
			}
			if ((Object)(object)spawnGroup != (Object)null)
			{
				if (!string.IsNullOrEmpty(spawnGroup.category))
				{
					return spawnGroup.category;
				}
				if ((Object)(object)spawnGroup.Monument != (Object)null)
				{
					return ((Object)spawnGroup.Monument).name;
				}
			}
			MonumentInfo monumentInfo = TerrainMeta.Path.FindMonumentWithBoundsOverlap(((Component)entity).transform.position);
			if ((Object)(object)monumentInfo != (Object)null)
			{
				return ((Object)monumentInfo).name;
			}
			return null;
		}

		private static string GetBiome(Vector3 position)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Expected I4, but got Unknown
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Invalid comparison between Unknown and I4
			string result = null;
			Enum val = (Enum)TerrainMeta.BiomeMap.GetBiomeMaxType(position);
			switch (val - 1)
			{
			default:
				if ((int)val == 8)
				{
					result = "arctic";
				}
				break;
			case 0:
				result = "arid";
				break;
			case 1:
				result = "grass";
				break;
			case 3:
				result = "tundra";
				break;
			case 2:
				break;
			}
			return result;
		}

		private static bool IsOcean(Vector3 position)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Invalid comparison between Unknown and I4
			Enum val = (Enum)TerrainMeta.TopologyMap.GetTopology(position);
			return (int)val == 128;
		}

		private static IEnumerator AggregateLoop()
		{
			int loop = 0;
			while (!Application.isQuitting)
			{
				yield return CoroutineEx.waitForSecondsRealtime(60f);
				if (Stats)
				{
					yield return TryCatch(AggregatePlayers(blueprints: false, positions: true));
					if (loop % 60 == 0)
					{
						PushServerInfo();
						yield return TryCatch(AggregateEntitiesAndItems());
						yield return TryCatch(AggregatePlayers(blueprints: true));
						yield return TryCatch(AggregateTeams());
						Dictionary<PendingItemsKey, PendingItemsData> toProcess = pendingItems;
						pendingItems = new Dictionary<PendingItemsKey, PendingItemsData>();
						yield return PushPendingItemsLoopAsync(toProcess);
					}
					loop++;
				}
			}
		}

		private static IEnumerator TryCatch(IEnumerator coroutine)
		{
			while (true)
			{
				try
				{
					if (!coroutine.MoveNext())
					{
						break;
					}
				}
				catch (Exception ex2)
				{
					Exception ex = ex2;
					Debug.LogException(ex);
					break;
				}
				yield return coroutine.Current;
			}
		}

		private static IEnumerator AggregateEntitiesAndItems()
		{
			List<BaseNetworkable> entityQueue = new List<BaseNetworkable>();
			entityQueue.Clear();
			int totalCount = BaseNetworkable.serverEntities.Count;
			entityQueue.AddRange(BaseNetworkable.serverEntities);
			Dictionary<string, int> itemDict = new Dictionary<string, int>();
			Dictionary<EntityKey, int> entityDict = new Dictionary<EntityKey, int>();
			yield return null;
			Debug.Log((object)"Starting to aggregate entities & items...");
			DateTime startTime = DateTime.UtcNow;
			Stopwatch watch = Stopwatch.StartNew();
			foreach (BaseNetworkable entity in entityQueue)
			{
				if (watch.ElapsedMilliseconds > MaxMSPerFrame)
				{
					yield return null;
					watch.Restart();
				}
				if ((Object)(object)entity == (Object)null || entity.IsDestroyed)
				{
					continue;
				}
				EntityKey entKey = new EntityKey
				{
					PrefabId = entity.prefabID
				};
				BuildingBlock buildingBlock;
				BuildingBlock block = (buildingBlock = entity as BuildingBlock);
				if (buildingBlock != null)
				{
					entKey.Grade = (int)(block.grade + 1);
				}
				entityDict.TryGetValue(entKey, out var entCount);
				entityDict[entKey] = entCount + 1;
				int num;
				BasePlayer basePlayer;
				if (!(entity is LootContainer))
				{
					BasePlayer npcPlayer = (basePlayer = entity as BasePlayer);
					if (basePlayer == null || !npcPlayer.IsNpc)
					{
						num = ((entity is NPCPlayer) ? 1 : 0);
						goto IL_023c;
					}
				}
				num = 1;
				goto IL_023c;
				IL_023c:
				if (num != 0)
				{
					continue;
				}
				BasePlayer player = (basePlayer = entity as BasePlayer);
				if (basePlayer != null)
				{
					AddItemsToDict(player.inventory.containerMain, itemDict);
					AddItemsToDict(player.inventory.containerBelt, itemDict);
					AddItemsToDict(player.inventory.containerWear, itemDict);
					continue;
				}
				IItemContainerEntity itemContainerEntity;
				IItemContainerEntity containerEntity = (itemContainerEntity = entity as IItemContainerEntity);
				if (itemContainerEntity != null)
				{
					AddItemsToDict(containerEntity.inventory, itemDict);
					continue;
				}
				DroppedItemContainer droppedItemContainer;
				DroppedItemContainer droppedContainer = (droppedItemContainer = entity as DroppedItemContainer);
				if (droppedItemContainer != null && droppedContainer.inventory != null)
				{
					AddItemsToDict(droppedContainer.inventory, itemDict);
				}
			}
			Debug.Log((object)$"Took {Math.Round(DateTime.UtcNow.Subtract(startTime).TotalSeconds, 1)}s to aggregate {totalCount} entities & items...");
			_ = DateTime.UtcNow;
			EventRecord.New("entity_sum").AddObject("counts", entityDict.Select(delegate(KeyValuePair<EntityKey, int> x)
			{
				EntitySumItem result = default(EntitySumItem);
				result.PrefabId = x.Key.PrefabId;
				result.Grade = x.Key.Grade;
				result.Count = x.Value;
				return result;
			})).Submit();
			yield return null;
			EventRecord.New("item_sum").AddObject("counts", itemDict).Submit();
			yield return null;
		}

		private static void AddItemsToDict(ItemContainer container, Dictionary<string, int> dict)
		{
			if (container == null || container.itemList == null)
			{
				return;
			}
			foreach (Item item in container.itemList)
			{
				string shortname = item.info.shortname;
				dict.TryGetValue(shortname, out var value);
				dict[shortname] = value + item.amount;
				if (item.contents != null)
				{
					AddItemsToDict(item.contents, dict);
				}
			}
		}

		private static IEnumerator PushPendingItemsLoopAsync(Dictionary<PendingItemsKey, PendingItemsData> dict)
		{
			Stopwatch watch = Stopwatch.StartNew();
			foreach (PendingItemsData pending in dict.Values)
			{
				try
				{
					LogResource(pending.Key.Consumed ? ResourceMode.Consumed : ResourceMode.Produced, pending.category, pending.Key.Item, pending.amount, null, null, safezone: false, null, 0uL, pending.Key.Entity);
				}
				catch (Exception ex)
				{
					Debug.LogException(ex);
				}
				PendingItemsData obj = pending;
				Pool.Free<PendingItemsData>(ref obj);
				if (watch.ElapsedMilliseconds > MaxMSPerFrame)
				{
					yield return null;
					watch.Restart();
				}
			}
			dict.Clear();
		}

		public static void AddPendingItems(BaseEntity entity, string itemName, int amount, string category, bool consumed = true, bool perEntity = false)
		{
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			PendingItemsKey pendingItemsKey = default(PendingItemsKey);
			pendingItemsKey.Entity = entity.ShortPrefabName;
			pendingItemsKey.Category = category;
			pendingItemsKey.Item = itemName;
			pendingItemsKey.Consumed = consumed;
			pendingItemsKey.EntityId = (NetworkableId)(perEntity ? entity.net.ID : default(NetworkableId));
			PendingItemsKey key = pendingItemsKey;
			if (!pendingItems.TryGetValue(key, out var value))
			{
				value = Pool.Get<PendingItemsData>();
				value.Key = key;
				value.category = category;
				pendingItems[key] = value;
			}
			value.amount += amount;
		}

		private static IEnumerator AggregatePlayers(bool blueprints = false, bool positions = false)
		{
			Stopwatch watch = Stopwatch.StartNew();
			List<BasePlayer> list = Pool.GetList<BasePlayer>();
			list.AddRange((IEnumerable<BasePlayer>)BasePlayer.activePlayerList);
			Dictionary<int, int> playerBps = (blueprints ? new Dictionary<int, int>() : null);
			List<PlayerAggregate> playerPositions = (positions ? Pool.GetList<PlayerAggregate>() : null);
			foreach (BasePlayer player2 in list)
			{
				if ((Object)(object)player2 == (Object)null || player2.IsDestroyed)
				{
					continue;
				}
				if (blueprints)
				{
					PersistantPlayer data = player2.PersistantPlayerInfo;
					foreach (int bp in data.unlockedItems)
					{
						playerBps.TryGetValue(bp, out var amount);
						playerBps[bp] = amount + 1;
					}
				}
				if (positions)
				{
					PlayerAggregate data2 = Pool.Get<PlayerAggregate>();
					data2.UserId = player2.WipeId;
					data2.Position = ((Component)player2).transform.position;
					Quaternion bodyRotation = player2.eyes.bodyRotation;
					data2.Direction = ((Quaternion)(ref bodyRotation)).eulerAngles;
					foreach (Item item3 in player2.inventory.containerBelt.itemList)
					{
						data2.Hotbar.Add(item3.info.shortname);
					}
					foreach (Item item2 in player2.inventory.containerWear.itemList)
					{
						data2.Hotbar.Add(item2.info.shortname);
					}
					data2.ActiveItem = player2.GetActiveItem()?.info.shortname;
					playerPositions.Add(data2);
				}
				if (watch.ElapsedMilliseconds > MaxMSPerFrame)
				{
					yield return null;
					watch.Restart();
				}
			}
			if (blueprints)
			{
				EventRecord point2 = EventRecord.New("blueprint_aggregate_online").AddObject("blueprints", playerBps.Select((KeyValuePair<int, int> x) => new
				{
					Key = ItemManager.FindItemDefinition(x.Key).shortname,
					value = x.Value
				}));
				point2.Submit();
			}
			if (!positions)
			{
				yield break;
			}
			EventRecord point = EventRecord.New("player_positions").AddObject("positions", playerPositions).AddObject("player_count", playerPositions.Count);
			point.Submit();
			foreach (PlayerAggregate player in playerPositions)
			{
				PlayerAggregate item = player;
				Pool.Free<PlayerAggregate>(ref item);
			}
			Pool.FreeList<PlayerAggregate>(ref playerPositions);
		}

		private static IEnumerator AggregateTeams()
		{
			yield return null;
			HashSet<ulong> teamIds = new HashSet<ulong>();
			int inTeam = 0;
			int notInTeam = 0;
			Enumerator<BasePlayer> enumerator = BasePlayer.activePlayerList.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					BasePlayer user2 = enumerator.Current;
					if ((Object)(object)user2 != (Object)null && !user2.IsDestroyed && user2.currentTeam != 0)
					{
						teamIds.Add(user2.currentTeam);
						inTeam++;
					}
					else
					{
						notInTeam++;
					}
				}
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			yield return null;
			Stopwatch watch = Stopwatch.StartNew();
			List<TeamInfo> teams = Pool.GetList<TeamInfo>();
			foreach (ulong teamId in teamIds)
			{
				RelationshipManager.PlayerTeam team = RelationshipManager.ServerInstance.FindTeam(teamId);
				if (team == null || !((team.members != null) & (team.members.Count > 0)))
				{
					continue;
				}
				TeamInfo teamInfo2 = Pool.Get<TeamInfo>();
				teams.Add(teamInfo2);
				foreach (ulong userId in team.members)
				{
					BasePlayer user = RelationshipManager.FindByID(userId);
					if ((Object)(object)user != (Object)null && !user.IsDestroyed && user.IsConnected && !user.IsSleeping())
					{
						teamInfo2.online.Add(SingletonComponent<ServerMgr>.Instance.persistance.GetUserWipeId(userId));
					}
					else
					{
						teamInfo2.offline.Add(SingletonComponent<ServerMgr>.Instance.persistance.GetUserWipeId(userId));
					}
				}
				teamInfo2.member_count = teamInfo2.online.Count + teamInfo2.offline.Count;
				if (watch.ElapsedMilliseconds > MaxMSPerFrame)
				{
					yield return null;
					watch.Restart();
				}
			}
			EventRecord point = EventRecord.New("online_teams").AddObject("teams", teams).AddField("users_in_team", inTeam)
				.AddField("users_not_in_team", notInTeam);
			point.Submit();
			foreach (TeamInfo teamInfo in teams)
			{
				TeamInfo item = teamInfo;
				Pool.Free<TeamInfo>(ref item);
			}
			Pool.FreeList<TeamInfo>(ref teams);
		}

		public static void Initialize()
		{
			PushItemDefinitions();
			PushEntityManifest();
			((MonoBehaviour)SingletonComponent<ServerMgr>.Instance).StartCoroutine(AggregateLoop());
		}

		private static void PushServerInfo()
		{
			if (!Stats)
			{
				return;
			}
			try
			{
				EventRecord eventRecord = EventRecord.New("server_info").AddField("seed", World.Seed).AddField("size", World.Size)
					.AddField("url", World.Url)
					.AddField("wipe_id", SaveRestore.WipeId)
					.AddField("ip_convar", Net.sv.ip)
					.AddField("port_convar", Net.sv.port)
					.AddField("net_protocol", Net.sv.ProtocolId)
					.AddField("protocol_network", 2402)
					.AddField("protocol_save", 239);
				BuildInfo current = BuildInfo.Current;
				EventRecord eventRecord2 = eventRecord.AddField("changeset", ((current != null) ? current.Scm.ChangeId : null) ?? "0").AddField("unity_version", Application.unityVersion);
				BuildInfo current2 = BuildInfo.Current;
				EventRecord eventRecord3 = eventRecord2.AddField("branch", ((current2 != null) ? current2.Scm.Branch : null) ?? "empty").AddField("server_tags", ConVar.Server.tags).AddField("device_id", SystemInfo.deviceUniqueIdentifier)
					.AddField("network_id", Net.sv.GetLastUIDGiven());
				eventRecord3.Submit();
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		private static void PushItemDefinitions()
		{
			if (!Stats)
			{
				return;
			}
			try
			{
				if ((Object)(object)GameManifest.Current == (Object)null)
				{
					return;
				}
				BuildInfo current = BuildInfo.Current;
				object obj;
				if (current == null)
				{
					obj = null;
				}
				else
				{
					ScmInfo scm = current.Scm;
					obj = ((scm != null) ? scm.ChangeId : null);
				}
				if (obj == null)
				{
					return;
				}
				EventRecord eventRecord = EventRecord.New("item_definitions").AddObject("items", from x in ItemManager.itemDictionary
					select x.Value into x
					select new
					{
						item_id = x.itemid,
						shortname = x.shortname,
						craft_time = (x.Blueprint?.time ?? 0f),
						workbench = (x.Blueprint?.workbenchLevelRequired ?? 0),
						category = x.category.ToString(),
						display_name = x.displayName.english,
						despawn_rarity = x.despawnRarity,
						ingredients = x.Blueprint?.ingredients.Select((ItemAmount y) => new
						{
							shortname = y.itemDef.shortname,
							amount = (int)y.amount
						})
					}).AddField("changeset", BuildInfo.Current.Scm.ChangeId);
				eventRecord.Submit();
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		private static void PushEntityManifest()
		{
			if (!Stats)
			{
				return;
			}
			try
			{
				if ((Object)(object)GameManifest.Current == (Object)null)
				{
					return;
				}
				BuildInfo current = BuildInfo.Current;
				object obj;
				if (current == null)
				{
					obj = null;
				}
				else
				{
					ScmInfo scm = current.Scm;
					obj = ((scm != null) ? scm.ChangeId : null);
				}
				if (obj != null)
				{
					EventRecord eventRecord = EventRecord.New("entity_manifest").AddObject("entities", GameManifest.Current.entities.Select((string x) => new
					{
						shortname = Path.GetFileNameWithoutExtension(x),
						prefab_id = StringPool.Get(x.ToLower())
					}));
					BuildInfo current2 = BuildInfo.Current;
					EventRecord eventRecord2 = eventRecord.AddField("changeset", ((current2 != null) ? current2.Scm.ChangeId : null) ?? "editor");
					eventRecord2.Submit();
				}
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		public static void OnFiredProjectile(BasePlayer player, BasePlayer.FiredProjectile projectile, Guid projectileGroupId)
		{
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_003f: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			if (!Stats || !HighFrequencyStats)
			{
				return;
			}
			try
			{
				EventRecord record = EventRecord.New("entity_damage").AddField("start_pos", projectile.position).AddField("start_vel", projectile.initialVelocity)
					.AddField("velocity_inherit", projectile.inheritedVelocity)
					.AddField("ammo_item", projectile.itemDef?.shortname)
					.AddField("weapon", (BaseEntity)projectile.weaponSource)
					.AddField("projectile_group", projectileGroupId)
					.AddField("projectile_id", projectile.id)
					.AddField("attacker", (BaseEntity)player)
					.AddField("look_dir", player.tickViewAngles)
					.AddField("model_state", (player.modelStateTick ?? player.modelState).flags);
				PendingFiredProjectile pendingFiredProjectile = Pool.Get<PendingFiredProjectile>();
				pendingFiredProjectile.Record = record;
				pendingFiredProjectile.FiredProjectile = projectile;
				firedProjectiles[new FiredProjectileKey(player.userID, projectile.id)] = pendingFiredProjectile;
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		public static void OnFiredProjectileRemoved(BasePlayer player, BasePlayer.FiredProjectile projectile)
		{
			if (!Stats)
			{
				return;
			}
			try
			{
				FiredProjectileKey key = new FiredProjectileKey(player.userID, projectile.id);
				if (!firedProjectiles.TryGetValue(key, out var value))
				{
					Debug.LogWarning((object)$"Can't find projectile for player '{player}' with id {projectile.id}");
					return;
				}
				if (!value.Hit)
				{
					EventRecord record = value.Record;
					if (projectile.updates.Count > 0)
					{
						record.AddObject("projectile_updates", projectile.updates);
					}
					record.Submit();
				}
				Pool.Free<PendingFiredProjectile>(ref value);
				firedProjectiles.Remove(key);
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		public static void OnQuarryItem(ResourceMode mode, string item, int amount, MiningQuarry sourceEntity)
		{
			if (!Stats)
			{
				return;
			}
			try
			{
				AddPendingItems(sourceEntity, item, amount, "quarry", mode == ResourceMode.Consumed);
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		public static void OnExcavatorProduceItem(Item item, BaseEntity sourceEntity)
		{
			if (!Stats)
			{
				return;
			}
			try
			{
				AddPendingItems(sourceEntity, item.info.shortname, item.amount, "excavator", consumed: false);
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		public static void OnExcavatorConsumeFuel(Item item, int amount, BaseEntity dieselEngine)
		{
			if (!Stats)
			{
				return;
			}
			try
			{
				LogResource(ResourceMode.Consumed, "excavator", item.info.shortname, amount, dieselEngine, null, safezone: false, null, 0uL);
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		public static void OnCraftItem(string item, int amount, BasePlayer player, BaseEntity workbench, bool inSafezone)
		{
			if (!Stats)
			{
				return;
			}
			try
			{
				LogResource(ResourceMode.Produced, "craft", item, amount, null, null, inSafezone, workbench, player?.userID ?? 0);
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		public static void OnCraftMaterialConsumed(string item, int amount, BasePlayer player, BaseEntity workbench, bool inSafezone, string targetItem)
		{
			if (!Stats)
			{
				return;
			}
			try
			{
				LogResource(safezone: inSafezone, workbench: workbench, targetItem: targetItem, mode: ResourceMode.Consumed, category: "craft", itemName: item, amount: amount, sourceEntity: null, tool: null, steamId: player?.userID ?? 0);
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		public static void OnConsumableUsed(BasePlayer player, Item item)
		{
			if (!Stats)
			{
				return;
			}
			try
			{
				EventRecord eventRecord = EventRecord.New("consumeable_used").AddField("player", (BaseEntity)player).AddField("item", item);
				eventRecord.Submit();
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		public static void OnEntitySpawned(BaseEntity entity)
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			if (!Stats)
			{
				return;
			}
			try
			{
				trackedSpawnedIds.Add(entity.net.ID);
				EventRecord eventRecord = EventRecord.New("entity_spawned").AddField("entity", entity);
				eventRecord.Submit();
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		private static void TryLogEntityKilled(BaseNetworkable entity)
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			if (!Stats)
			{
				return;
			}
			try
			{
				if (trackedSpawnedIds.Contains(entity.net.ID))
				{
					EventRecord eventRecord = EventRecord.New("entity_killed").AddField("entity", Object.op_Implicit((Object)(object)entity));
					eventRecord.Submit();
				}
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		public static void OnMedUsed(string itemName, BasePlayer player, BasePlayer target)
		{
			if (!Stats)
			{
				return;
			}
			try
			{
				EventRecord eventRecord = EventRecord.New("med_used").AddField("player", (BaseEntity)player).AddField("target", (BaseEntity)target)
					.AddField("item_name", itemName);
				eventRecord.Submit();
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		public static void OnCodelockChanged(BasePlayer player, CodeLock codeLock, string oldCode, string newCode, bool isGuest)
		{
			if (!Stats)
			{
				return;
			}
			try
			{
				EventRecord eventRecord = EventRecord.New("code_change").AddField("player", (BaseEntity)player).AddField("codelock", (BaseEntity)codeLock)
					.AddField("old_code", oldCode)
					.AddField("new_code", newCode)
					.AddField("is_guest", isGuest);
				eventRecord.Submit();
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		public static void OnCodeLockEntered(BasePlayer player, CodeLock codeLock, bool isGuest)
		{
			if (!Stats)
			{
				return;
			}
			try
			{
				EventRecord eventRecord = EventRecord.New("code_enter").AddField("player", (BaseEntity)player).AddField("codelock", (BaseEntity)codeLock)
					.AddField("is_guest", isGuest);
				eventRecord.Submit();
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		public static void OnTeamChanged(string change, ulong teamId, ulong teamLeader, ulong user, List<ulong> members)
		{
			if (!Stats)
			{
				return;
			}
			List<string> list = Pool.GetList<string>();
			try
			{
				if (members != null)
				{
					foreach (ulong member in members)
					{
						list.Add(SingletonComponent<ServerMgr>.Instance.persistance.GetUserWipeId(member));
					}
				}
				EventRecord eventRecord = EventRecord.New("team_change").AddField("team_leader", SingletonComponent<ServerMgr>.Instance.persistance.GetUserWipeId(teamLeader)).AddField("team", teamId)
					.AddField("target_user", SingletonComponent<ServerMgr>.Instance.persistance.GetUserWipeId(user))
					.AddField("change", change)
					.AddObject("users", list)
					.AddField("member_count", members.Count);
				eventRecord.Submit();
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
			Pool.FreeList<string>(ref list);
		}

		public static void OnEntityAuthChanged(BaseEntity entity, BasePlayer player, IEnumerable<ulong> authedList, string change, ulong targetUser)
		{
			if (!Stats)
			{
				return;
			}
			try
			{
				string userWipeId = SingletonComponent<ServerMgr>.Instance.persistance.GetUserWipeId(targetUser);
				EventRecord eventRecord = EventRecord.New("auth_change").AddField("entity", entity).AddField("player", (BaseEntity)player)
					.AddField("target", userWipeId)
					.AddObject("auth_list", authedList.Select((ulong x) => SingletonComponent<ServerMgr>.Instance.persistance.GetUserWipeId(x)))
					.AddField("change", change);
				eventRecord.Submit();
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		public static void OnSleepingBagAssigned(BasePlayer player, SleepingBag bag, ulong targetUser)
		{
			if (!Stats)
			{
				return;
			}
			try
			{
				string value = ((targetUser != 0L) ? SingletonComponent<ServerMgr>.Instance.persistance.GetUserWipeId(targetUser) : "");
				EventRecord eventRecord = EventRecord.New("sleeping_bag_assign").AddField("entity", (BaseEntity)bag).AddField("player", (BaseEntity)player)
					.AddField("target", value);
				eventRecord.Submit();
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		public static void OnFallDamage(BasePlayer player, float velocity, float damage)
		{
			if (!Stats)
			{
				return;
			}
			try
			{
				EventRecord eventRecord = EventRecord.New("fall_damage").AddField("player", (BaseEntity)player).AddField("velocity", velocity)
					.AddField("damage", damage);
				eventRecord.Submit();
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		public static void OnResearchStarted(BasePlayer player, BaseEntity entity, Item item, int scrapCost)
		{
			if (!Stats)
			{
				return;
			}
			try
			{
				EventRecord eventRecord = EventRecord.New("research_start").AddField("player", (BaseEntity)player).AddField("item", item.info.shortname)
					.AddField("scrap", scrapCost)
					.AddField("entity", entity);
				eventRecord.Submit();
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		public static void OnBlueprintLearned(BasePlayer player, ItemDefinition item, string reason, BaseEntity entity = null)
		{
			if (!Stats)
			{
				return;
			}
			try
			{
				EventRecord eventRecord = EventRecord.New("blueprint_learned").AddField("player", (BaseEntity)player).AddField("item", item.shortname)
					.AddField("reason", reason)
					.AddField("entity", entity);
				eventRecord.Submit();
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		public static void OnItemRecycled(string item, int amount, Recycler recycler)
		{
			if (!Stats)
			{
				return;
			}
			try
			{
				LogResource(ResourceMode.Consumed, "recycler", item, amount, recycler, null, safezone: false, null, recycler.LastLootedBy);
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		public static void OnRecyclerItemProduced(string item, int amount, Recycler recycler, Item sourceItem)
		{
			if (!Stats)
			{
				return;
			}
			try
			{
				LogResource(ResourceMode.Produced, "recycler", item, amount, recycler, null, safezone: false, null, recycler.LastLootedBy, null, sourceItem);
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		public static void OnGatherItem(string item, int amount, BaseEntity sourceEntity, BasePlayer player, AttackEntity weapon = null)
		{
			if (!Stats)
			{
				return;
			}
			try
			{
				LogResource(ResourceMode.Produced, "gather", item, amount, sourceEntity, weapon, safezone: false, null, player.userID);
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		public static void OnFirstLooted(BaseEntity entity, BasePlayer player)
		{
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			if (!Stats)
			{
				return;
			}
			try
			{
				if (entity is LootContainer lootContainer)
				{
					LogItemsLooted(player, entity, lootContainer.inventory);
					EventRecord eventRecord = EventRecord.New("loot_entity").AddField("entity", entity).AddField("player", (BaseEntity)player)
						.AddField("monument", GetMonument(entity))
						.AddField("biome", GetBiome(((Component)entity).transform.position));
					eventRecord.Submit();
				}
				else if (entity is LootableCorpse lootableCorpse)
				{
					ItemContainer[] containers = lootableCorpse.containers;
					foreach (ItemContainer container in containers)
					{
						LogItemsLooted(player, entity, container);
					}
				}
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		public static void OnLootContainerDestroyed(LootContainer entity, BasePlayer player, AttackEntity weapon)
		{
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Unknown result type (might be due to invalid IL or missing references)
			if (!Stats)
			{
				return;
			}
			try
			{
				if (entity.DropsLoot && (Object)(object)player != (Object)null && Vector3.Distance(((Component)entity).transform.position, ((Component)player).transform.position) < 50f && entity.inventory?.itemList != null && entity.inventory.itemList.Count > 0)
				{
					LogItemsLooted(player, entity, entity.inventory, weapon);
				}
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		public static void OnEntityDestroyed(BaseNetworkable entity)
		{
			TryLogEntityKilled(entity);
			if (!Stats)
			{
				return;
			}
			try
			{
				if (!(entity is LootContainer lootContainer) || !lootContainer.FirstLooted)
				{
					return;
				}
				foreach (Item item in lootContainer.inventory.itemList)
				{
					OnItemDespawn(lootContainer, item, 3, lootContainer.LastLootedBy);
				}
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		public static void OnEntityBuilt(BaseEntity entity, BasePlayer player)
		{
			if (!Stats)
			{
				return;
			}
			try
			{
				EventRecord eventRecord = EventRecord.New("entity_built").AddField("player", (BaseEntity)player).AddField("entity", entity);
				if (entity is SleepingBag)
				{
					int sleepingBagCount = SleepingBag.GetSleepingBagCount(player.userID);
					eventRecord.AddField("bags_active", sleepingBagCount);
					eventRecord.AddField("max_sleeping_bags", ConVar.Server.max_sleeping_bags);
				}
				eventRecord.Submit();
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		public static void OnKeycardSwiped(BasePlayer player, CardReader cardReader)
		{
			if (!Stats)
			{
				return;
			}
			try
			{
				EventRecord eventRecord = EventRecord.New("keycard_swiped").AddField("player", (BaseEntity)player).AddField("card_level", cardReader.accessLevel)
					.AddField("entity", (BaseEntity)cardReader);
				eventRecord.Submit();
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		public static void OnLockedCrateStarted(BasePlayer player, HackableLockedCrate crate)
		{
			if (!Stats)
			{
				return;
			}
			try
			{
				EventRecord eventRecord = EventRecord.New("hackable_crate_started").AddField("player", (BaseEntity)player).AddField("entity", (BaseEntity)crate);
				eventRecord.Submit();
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		public static void OnLockedCrateFinished(ulong player, HackableLockedCrate crate)
		{
			if (!Stats)
			{
				return;
			}
			try
			{
				string userWipeId = SingletonComponent<ServerMgr>.Instance.persistance.GetUserWipeId(player);
				EventRecord eventRecord = EventRecord.New("hackable_crate_ended").AddField("player_userid", userWipeId).AddField("entity", (BaseEntity)crate);
				eventRecord.Submit();
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		public static void OnStashHidden(BasePlayer player, StashContainer entity)
		{
			if (!Stats)
			{
				return;
			}
			try
			{
				EventRecord eventRecord = EventRecord.New("stash_hidden").AddField("player", (BaseEntity)player).AddField("entity", (BaseEntity)entity)
					.AddField("owner", SingletonComponent<ServerMgr>.Instance.persistance.GetUserWipeId(entity.OwnerID));
				eventRecord.Submit();
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		public static void OnStashRevealed(BasePlayer player, StashContainer entity)
		{
			if (!Stats)
			{
				return;
			}
			try
			{
				EventRecord eventRecord = EventRecord.New("stash_reveal").AddField("player", (BaseEntity)player).AddField("entity", (BaseEntity)entity)
					.AddField("owner", SingletonComponent<ServerMgr>.Instance.persistance.GetUserWipeId(entity.OwnerID));
				eventRecord.Submit();
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		public static void OnAntihackViolation(BasePlayer player, int type, string message)
		{
			if (!Stats)
			{
				return;
			}
			try
			{
				EventRecord eventRecord = EventRecord.New("antihack_violation").AddField("player", (BaseEntity)player).AddField("violation_type", type)
					.AddField("message", message);
				eventRecord.Submit();
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		public static void OnEyehackViolation(BasePlayer player, Vector3 eyePos)
		{
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			if (!Stats)
			{
				return;
			}
			try
			{
				EventRecord eventRecord = EventRecord.New("antihack_violation_detailed").AddField("player", (BaseEntity)player).AddField("violation_type", 6)
					.AddField("eye_pos", eyePos);
				eventRecord.Submit();
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		public static void OnNoclipViolation(BasePlayer player, Vector3 startPos, Vector3 endPos, int tickCount, Collider collider)
		{
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			if (!Stats || !HighFrequencyStats)
			{
				return;
			}
			try
			{
				EventRecord eventRecord = EventRecord.New("antihack_violation_detailed").AddField("player", (BaseEntity)player).AddField("violation_type", 1)
					.AddField("start_pos", startPos)
					.AddField("end_pos", endPos)
					.AddField("tick_count", tickCount)
					.AddField("collider_name", ((Object)collider).name);
				eventRecord.Submit();
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		public static void OnFlyhackViolation(BasePlayer player, Vector3 startPos, Vector3 endPos, int tickCount)
		{
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			if (!Stats)
			{
				return;
			}
			try
			{
				EventRecord eventRecord = EventRecord.New("antihack_violation_detailed").AddField("player", (BaseEntity)player).AddField("violation_type", 3)
					.AddField("start_pos", startPos)
					.AddField("end_pos", endPos)
					.AddField("tick_count", tickCount);
				eventRecord.Submit();
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		public static void OnProjectileHackViolation(BasePlayer.FiredProjectile projectile)
		{
			if (!Stats)
			{
				return;
			}
			try
			{
				FiredProjectileKey key = new FiredProjectileKey(projectile.attacker.userID, projectile.id);
				if (!firedProjectiles.TryGetValue(key, out var value))
				{
					Debug.LogWarning((object)$"Can't find projectile for player '{projectile.attacker}' with id {projectile.id}");
				}
				else
				{
					value.Record.AddField("projectile_invalid", value: true).AddObject("updates", projectile.updates);
				}
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		public static void OnSpeedhackViolation(BasePlayer player, Vector3 startPos, Vector3 endPos, int tickCount)
		{
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0059: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			if (!Stats)
			{
				return;
			}
			try
			{
				EventRecord eventRecord = EventRecord.New("antihack_violation_detailed").AddField("player", (BaseEntity)player).AddField("violation_type", 2)
					.AddField("start_pos", startPos)
					.AddField("end_pos", endPos)
					.AddField("tick_count", tickCount)
					.AddField("distance", Vector3.Distance(startPos, endPos))
					.AddField("distance_2d", Vector3Ex.Distance2D(startPos, endPos));
				eventRecord.Submit();
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		public static void OnTerrainHackViolation(BasePlayer player)
		{
			if (!Stats)
			{
				return;
			}
			try
			{
				EventRecord eventRecord = EventRecord.New("antihack_violation_detailed").AddField("player", (BaseEntity)player).AddField("violation_type", 10)
					.AddField("seed", World.Seed)
					.AddField("size", World.Size)
					.AddField("map_url", World.Url)
					.AddField("map_checksum", World.Checksum);
				eventRecord.Submit();
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		public static void OnEntityTakeDamage(HitInfo info, bool isDeath)
		{
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
			//IL_020e: Unknown result type (might be due to invalid IL or missing references)
			//IL_021e: Unknown result type (might be due to invalid IL or missing references)
			//IL_022e: Unknown result type (might be due to invalid IL or missing references)
			//IL_023e: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0145: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_015d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			//IL_010f: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0350: Unknown result type (might be due to invalid IL or missing references)
			//IL_039c: Unknown result type (might be due to invalid IL or missing references)
			//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
			if (!Stats || !HighFrequencyStats)
			{
				return;
			}
			try
			{
				BasePlayer initiatorPlayer = info.InitiatorPlayer;
				BasePlayer basePlayer = info.HitEntity as BasePlayer;
				if (((Object)(object)info.Initiator == (Object)null && !isDeath) || (((Object)(object)initiatorPlayer == (Object)null || initiatorPlayer.IsNpc || initiatorPlayer.IsBot) && ((Object)(object)basePlayer == (Object)null || basePlayer.IsNpc || basePlayer.IsBot)))
				{
					return;
				}
				EventRecord eventRecord = null;
				float value = -1f;
				float value2 = -1f;
				if ((Object)(object)initiatorPlayer != (Object)null)
				{
					if (info.IsProjectile())
					{
						FiredProjectileKey key = new FiredProjectileKey(initiatorPlayer.userID, info.ProjectileID);
						if (firedProjectiles.TryGetValue(key, out var value3))
						{
							eventRecord = value3.Record;
							value = Vector3.Distance(info.HitNormalWorld, value3.FiredProjectile.initialPosition);
							value = Vector3Ex.Distance2D(info.HitNormalWorld, value3.FiredProjectile.initialPosition);
							value3.Hit = info.DidHit;
						}
					}
					else
					{
						value = Vector3.Distance(info.HitNormalWorld, initiatorPlayer.eyes.position);
						value = Vector3Ex.Distance2D(info.HitNormalWorld, initiatorPlayer.eyes.position);
					}
				}
				if (eventRecord == null)
				{
					eventRecord = EventRecord.New("entity_damage");
				}
				eventRecord.AddField("is_hit", value: true).AddField("is_headshot", info.isHeadshot).AddField("victim", info.HitEntity)
					.AddField("damage", info.damageTypes.Total())
					.AddField("damage_type", info.damageTypes.GetMajorityDamageType().ToString())
					.AddField("pos_world", info.HitPositionWorld)
					.AddField("pos_local", info.HitPositionLocal)
					.AddField("point_start", info.PointStart)
					.AddField("point_end", info.PointEnd)
					.AddField("normal_world", info.HitNormalWorld)
					.AddField("normal_local", info.HitNormalLocal)
					.AddField("distance_cl", info.ProjectileDistance)
					.AddField("distance", value)
					.AddField("distance_2d", value2);
				if (!info.IsProjectile())
				{
					eventRecord.AddField("weapon", (BaseEntity)info.Weapon);
					eventRecord.AddField("attacker", info.Initiator);
				}
				if (info.HitBone != 0)
				{
					eventRecord.AddField("bone", info.HitBone).AddField("bone_name", info.boneName).AddField("hit_area", (int)info.boneArea);
				}
				if (info.ProjectileID != 0)
				{
					eventRecord.AddField("projectile_id", info.ProjectileID).AddField("projectile_integrity", info.ProjectileIntegrity).AddField("projectile_hits", info.ProjectileHits)
						.AddField("trajectory_mismatch", info.ProjectileTrajectoryMismatch)
						.AddField("travel_time", info.ProjectileTravelTime)
						.AddField("projectile_velocity", info.ProjectileVelocity)
						.AddField("projectile_prefab", ((Object)info.ProjectilePrefab).name);
				}
				if ((Object)(object)initiatorPlayer != (Object)null && !info.IsProjectile())
				{
					eventRecord.AddField("attacker_eye_pos", initiatorPlayer.eyes.position);
					eventRecord.AddField("attacker_eye_dir", initiatorPlayer.eyes.BodyForward());
					if (((object)initiatorPlayer).GetType() == typeof(BasePlayer))
					{
						eventRecord.AddField("attacker_life", initiatorPlayer.respawnId);
					}
					if (isDeath)
					{
						eventRecord.AddObject("attacker_worn", initiatorPlayer.inventory.containerWear.itemList.Select((Item x) => new SimpleItemAmount(x)));
						eventRecord.AddObject("attacker_hotbar", initiatorPlayer.inventory.containerBelt.itemList.Select((Item x) => new SimpleItemAmount(x)));
					}
				}
				if ((Object)(object)basePlayer != (Object)null)
				{
					eventRecord.AddField("victim_life", basePlayer.respawnId);
					eventRecord.AddObject("victim_worn", basePlayer.inventory.containerWear.itemList.Select((Item x) => new SimpleItemAmount(x)));
					eventRecord.AddObject("victim_hotbar", basePlayer.inventory.containerBelt.itemList.Select((Item x) => new SimpleItemAmount(x)));
				}
				eventRecord.Submit();
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		public static void OnPlayerRespawned(BasePlayer player, BaseEntity targetEntity)
		{
			if (!Stats)
			{
				return;
			}
			try
			{
				EventRecord eventRecord = EventRecord.New("player_respawn").AddField("player", (BaseEntity)player).AddField("bag", targetEntity)
					.AddField("life_id", player.respawnId);
				eventRecord.Submit();
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		public static void OnExplosiveLaunched(BasePlayer player, BaseEntity explosive, BaseEntity launcher = null)
		{
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			if (!Stats)
			{
				return;
			}
			try
			{
				EventRecord eventRecord = EventRecord.New("explosive_launch").AddField("player", (BaseEntity)player).AddField("explosive", explosive)
					.AddField("explosive_velocity", explosive.GetWorldVelocity());
				Vector3 worldVelocity = explosive.GetWorldVelocity();
				EventRecord eventRecord2 = eventRecord.AddField("explosive_direction", ((Vector3)(ref worldVelocity)).normalized);
				if ((Object)(object)launcher != (Object)null)
				{
					eventRecord2.AddField("launcher", launcher);
				}
				eventRecord2.Submit();
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		public static void OnExplosion(TimedExplosive explosive)
		{
			if (!Stats)
			{
				return;
			}
			try
			{
				EventRecord eventRecord = EventRecord.New("explosion").AddField("entity", (BaseEntity)explosive);
				eventRecord.Submit();
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		public static void OnItemDespawn(BaseEntity itemContainer, Item item, int dropReason, ulong userId)
		{
			if (!Stats)
			{
				return;
			}
			try
			{
				EventRecord eventRecord = EventRecord.New("item_despawn").AddField("entity", itemContainer).AddField("item", item)
					.AddField("drop_reason", dropReason);
				if (userId != 0)
				{
					eventRecord.AddField("player_userid", userId);
				}
				eventRecord.Submit();
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		public static void OnItemDropped(BasePlayer player, WorldItem entity, DroppedItem.DropReasonEnum dropReason)
		{
			if (!Stats)
			{
				return;
			}
			try
			{
				EventRecord eventRecord = EventRecord.New("item_drop").AddField("player", (BaseEntity)player).AddField("entity", (BaseEntity)entity)
					.AddField("item", entity.GetItem())
					.AddField("drop_reason", (int)dropReason);
				eventRecord.Submit();
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		public static void OnItemPickup(BasePlayer player, WorldItem entity)
		{
			if (!Stats)
			{
				return;
			}
			try
			{
				EventRecord eventRecord = EventRecord.New("item_pickup").AddField("player", (BaseEntity)player).AddField("entity", (BaseEntity)entity)
					.AddField("item", entity.GetItem());
				eventRecord.Submit();
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		public static void OnPlayerConnected(Connection connection)
		{
			if (!Stats)
			{
				return;
			}
			try
			{
				string userWipeId = SingletonComponent<ServerMgr>.Instance.persistance.GetUserWipeId(connection.userid);
				EventRecord eventRecord = EventRecord.New("player_connect").AddField("player_userid", userWipeId).AddField("username", connection.username);
				eventRecord.Submit();
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		public static void OnPlayerDisconnected(Connection connection, string reason)
		{
			if (!Stats)
			{
				return;
			}
			try
			{
				string userWipeId = SingletonComponent<ServerMgr>.Instance.persistance.GetUserWipeId(connection.userid);
				EventRecord eventRecord = EventRecord.New("player_disconnect").AddField("player_userid", userWipeId).AddField("username", connection.username)
					.AddField("reason", reason);
				eventRecord.Submit();
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		public static void OnEntityPickedUp(BasePlayer player, BaseEntity entity)
		{
			if (!Stats)
			{
				return;
			}
			try
			{
				EventRecord eventRecord = EventRecord.New("entity_pickup").AddField("player", (BaseEntity)player).AddField("entity", entity);
				eventRecord.Submit();
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		public static void OnChatMessage(BasePlayer player, string message, int channel)
		{
			if (!Stats)
			{
				return;
			}
			try
			{
				EventRecord eventRecord = EventRecord.New("chat").AddField("player", (BaseEntity)player).AddField("message", message)
					.AddField("channel", channel);
				eventRecord.Submit();
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		public static void OnVendingMachineOrderChanged(BasePlayer player, VendingMachine vendingMachine, int sellItemId, int sellAmount, bool sellingBp, int buyItemId, int buyAmount, bool buyingBp, bool added)
		{
			if (!Stats)
			{
				return;
			}
			try
			{
				ItemDefinition itemDefinition = ItemManager.FindItemDefinition(sellItemId);
				ItemDefinition itemDefinition2 = ItemManager.FindItemDefinition(buyItemId);
				EventRecord eventRecord = EventRecord.New("vending_changed").AddField("player", (BaseEntity)player).AddField("entity", (BaseEntity)vendingMachine)
					.AddField("sell_item", itemDefinition.shortname)
					.AddField("sell_amount", sellAmount)
					.AddField("buy_item", itemDefinition2.shortname)
					.AddField("buy_amount", buyAmount)
					.AddField("is_selling_bp", sellingBp)
					.AddField("is_buying_bp", buyingBp)
					.AddField("change", added ? "added" : "removed");
				eventRecord.Submit();
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		public static void OnBuyFromVendingMachine(BasePlayer player, VendingMachine vendingMachine, int sellItemId, int sellAmount, bool sellingBp, int buyItemId, int buyAmount, bool buyingBp, int numberOfTransactions, BaseEntity drone = null)
		{
			if (!Stats)
			{
				return;
			}
			try
			{
				ItemDefinition itemDefinition = ItemManager.FindItemDefinition(sellItemId);
				ItemDefinition itemDefinition2 = ItemManager.FindItemDefinition(buyItemId);
				EventRecord eventRecord = EventRecord.New("vending_sale").AddField("player", (BaseEntity)player).AddField("entity", (BaseEntity)vendingMachine)
					.AddField("sell_item", itemDefinition.shortname)
					.AddField("sell_amount", sellAmount)
					.AddField("buy_item", itemDefinition2.shortname)
					.AddField("buy_amount", buyAmount)
					.AddField("transactions", numberOfTransactions)
					.AddField("is_selling_bp", sellingBp)
					.AddField("is_buying_bp", buyingBp)
					.AddField("drone_terminal", drone);
				eventRecord.Submit();
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		public static void OnNPCVendor(BasePlayer player, NPCTalking vendor, int scrapCost, string action)
		{
			if (!Stats)
			{
				return;
			}
			try
			{
				EventRecord eventRecord = EventRecord.New("npc_vendor").AddField("player", (BaseEntity)player).AddField("vendor", (BaseEntity)vendor)
					.AddField("scrap_amount", scrapCost)
					.AddField("action", action);
				eventRecord.Submit();
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		private static void LogItemsLooted(BasePlayer looter, BaseEntity entity, ItemContainer container, AttackEntity tool = null)
		{
			if (!Stats)
			{
				return;
			}
			try
			{
				if ((Object)(object)entity == (Object)null || container == null)
				{
					return;
				}
				foreach (Item item in container.itemList)
				{
					if (item != null)
					{
						string shortname = item.info.shortname;
						int amount = item.amount;
						ulong steamId = looter?.userID ?? 0;
						LogResource(ResourceMode.Produced, "loot", shortname, amount, entity, tool, safezone: false, null, steamId);
					}
				}
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		public static void LogResource(ResourceMode mode, string category, string itemName, int amount, BaseEntity sourceEntity = null, AttackEntity tool = null, bool safezone = false, BaseEntity workbench = null, ulong steamId = 0uL, string sourceEntityPrefab = null, Item sourceItem = null, string targetItem = null)
		{
			//IL_0087: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
			if (!Stats || !HighFrequencyStats)
			{
				return;
			}
			try
			{
				EventRecord eventRecord = EventRecord.New("item_event").AddField("item_mode", mode.ToString()).AddField("category", category)
					.AddField("item_name", itemName)
					.AddField("amount", amount);
				if ((Object)(object)sourceEntity != (Object)null)
				{
					eventRecord.AddField("entity", sourceEntity);
					string biome = GetBiome(((Component)sourceEntity).transform.position);
					if (biome != null)
					{
						eventRecord.AddField("biome", biome);
					}
					if (IsOcean(((Component)sourceEntity).transform.position))
					{
						eventRecord.AddField("ocean", value: true);
					}
					string monument = GetMonument(sourceEntity);
					if (monument != null)
					{
						eventRecord.AddField("monument", monument);
					}
				}
				if (sourceEntityPrefab != null)
				{
					eventRecord.AddField("entity_prefab", sourceEntityPrefab);
				}
				if ((Object)(object)tool != (Object)null)
				{
					eventRecord.AddField("tool", (BaseEntity)tool);
				}
				if (safezone)
				{
					eventRecord.AddField("safezone", value: true);
				}
				if ((Object)(object)workbench != (Object)null)
				{
					eventRecord.AddField("workbench", workbench);
				}
				if (sourceEntity is GrowableEntity plant)
				{
					eventRecord.AddField("genes", GetGenesAsString(plant));
				}
				if (sourceItem != null)
				{
					eventRecord.AddField("source_item", sourceItem.info.shortname);
				}
				if (targetItem != null)
				{
					eventRecord.AddField("target_item", targetItem);
				}
				if (steamId != 0)
				{
					string userWipeId = SingletonComponent<ServerMgr>.Instance.persistance.GetUserWipeId(steamId);
					eventRecord.AddField("player_userid", userWipeId);
				}
				eventRecord.Submit();
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		public static void OnSkinChanged(BasePlayer player, RepairBench repairBench, Item item, ulong workshopId)
		{
			if (!Stats)
			{
				return;
			}
			try
			{
				EventRecord eventRecord = EventRecord.New("item_skinned").AddField("player", (BaseEntity)player).AddField("entity", (BaseEntity)repairBench)
					.AddField("item", item)
					.AddField("new_skin", workshopId);
				eventRecord.Submit();
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		public static void OnItemRepaired(BasePlayer player, BaseEntity repairBench, Item itemToRepair, float conditionBefore, float maxConditionBefore)
		{
			if (!Stats)
			{
				return;
			}
			try
			{
				EventRecord eventRecord = EventRecord.New("item_repair").AddField("player", (BaseEntity)player).AddField("entity", repairBench)
					.AddField("item", itemToRepair)
					.AddField("old_condition", conditionBefore)
					.AddField("old_max_condition", maxConditionBefore)
					.AddField("max_condition", itemToRepair.maxConditionNormalized);
				eventRecord.Submit();
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		public static void OnEntityRepaired(BasePlayer player, BaseEntity entity, float healthBefore, float healthAfter)
		{
			if (!Stats)
			{
				return;
			}
			try
			{
				EventRecord eventRecord = EventRecord.New("entity_repair").AddField("player", (BaseEntity)player).AddField("entity", entity)
					.AddField("healing", healthAfter - healthBefore)
					.AddField("health_before", healthBefore)
					.AddField("health_after", healthAfter);
				eventRecord.Submit();
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		public static void OnBuildingBlockUpgraded(BasePlayer player, BuildingBlock buildingBlock, BuildingGrade.Enum targetGrade, uint targetColor)
		{
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			if (!Stats)
			{
				return;
			}
			try
			{
				EventRecord eventRecord = EventRecord.New("block_upgrade").AddField("player", (BaseEntity)player).AddField("entity", (BaseEntity)buildingBlock)
					.AddField("old_grade", (int)buildingBlock.grade)
					.AddField("new_grade", (int)targetGrade)
					.AddField("color", targetColor)
					.AddField("biome", GetBiome(((Component)buildingBlock).transform.position));
				eventRecord.Submit();
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		public static void OnBuildingBlockDemolished(BasePlayer player, BuildingBlock buildingBlock)
		{
			if (!Stats)
			{
				return;
			}
			try
			{
				EventRecord eventRecord = EventRecord.New("block_demolish").AddField("player", (BaseEntity)player).AddField("entity", (BaseEntity)buildingBlock);
				eventRecord.Submit();
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		public static void OnPlayerInitializedWipeId(ulong userId, string wipeId)
		{
			if (!Stats)
			{
				return;
			}
			try
			{
				EventRecord eventRecord = EventRecord.New("player_wipe_id_set").AddField("user_id", userId).AddField("player_wipe_id", wipeId);
				eventRecord.Submit();
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		public static void OnFreeUnderwaterCrate(BasePlayer player, FreeableLootContainer freeableLootContainer)
		{
			if (!Stats)
			{
				return;
			}
			try
			{
				EventRecord eventRecord = EventRecord.New("crate_untied").AddField("player", (BaseEntity)player).AddField("entity", (BaseEntity)freeableLootContainer);
				eventRecord.Submit();
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		public static void OnVehiclePurchased(BasePlayer player, BaseEntity vehicle)
		{
			if (!Stats)
			{
				return;
			}
			try
			{
				EventRecord eventRecord = EventRecord.New("vehicle_purchase").AddField("player", (BaseEntity)player).AddField("entity", vehicle)
					.AddField("price", vehicle);
				eventRecord.Submit();
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		public static void OnMissionComplete(BasePlayer player, BaseMission mission, BaseMission.MissionFailReason? failReason = null)
		{
			if (!Stats)
			{
				return;
			}
			try
			{
				EventRecord eventRecord = EventRecord.New("mission_complete").AddField("player", (BaseEntity)player).AddField("mission", mission.shortname)
					.AddField("mission_succeed", value: true);
				if (failReason.HasValue)
				{
					eventRecord.AddField("mission_succeed", value: false).AddField("fail_reason", failReason.Value.ToString());
				}
				eventRecord.Submit();
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		public static void OnGamblingResult(BasePlayer player, BaseEntity entity, int scrapPaid, int scrapRecieved, Guid? gambleGroupId = null)
		{
			if (!Stats)
			{
				return;
			}
			try
			{
				EventRecord eventRecord = EventRecord.New("gambing").AddField("player", (BaseEntity)player).AddField("entity", entity)
					.AddField("scrap_input", scrapPaid)
					.AddField("scrap_output", scrapRecieved);
				if (gambleGroupId.HasValue)
				{
					eventRecord.AddField("gamble_grouping", gambleGroupId.Value);
				}
				eventRecord.Submit();
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		public static void OnPlayerPinged(BasePlayer player, BasePlayer.PingType type, bool wasViaWheel)
		{
			if (!Stats)
			{
				return;
			}
			try
			{
				EventRecord eventRecord = EventRecord.New("player_pinged").AddField("player", (BaseEntity)player).AddField("pingType", (int)type)
					.AddField("viaWheel", wasViaWheel);
				eventRecord.Submit();
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		public static void OnBagUnclaimed(BasePlayer player, SleepingBag bag)
		{
			if (!Stats)
			{
				return;
			}
			try
			{
				EventRecord eventRecord = EventRecord.New("bag_unclaim").AddField("player", (BaseEntity)player).AddField("entity", (BaseEntity)bag);
				eventRecord.Submit();
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		public static void OnSteamAuth(ulong userId, ulong ownerUserId, string authResponse)
		{
			try
			{
				EventRecord eventRecord = EventRecord.New("steam_auth").AddField("user", userId).AddField("owner", ownerUserId)
					.AddField("response", authResponse)
					.AddField("server_port", Net.sv.port)
					.AddField("network_mode", Net.sv.ProtocolId)
					.AddField("player_count", BasePlayer.activePlayerList.Count)
					.AddField("max_players", ConVar.Server.maxplayers)
					.AddField("hostname", ConVar.Server.hostname);
				eventRecord.Submit();
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}

		public static void OnBuildingBlockColorChanged(BasePlayer player, BuildingBlock block, uint oldColor, uint newColor)
		{
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			if (!Stats)
			{
				return;
			}
			try
			{
				EventRecord eventRecord = EventRecord.New("player_pinged").AddField("player", (BaseEntity)player).AddField("entity", (BaseEntity)block)
					.AddField("color_old", oldColor)
					.AddField("color_new", newColor)
					.AddField("biome", GetBiome(((Component)block).transform.position));
				eventRecord.Submit();
			}
			catch (Exception ex)
			{
				Debug.LogException(ex);
			}
		}
	}

	public class AzureWebInterface
	{
		public static readonly AzureWebInterface client = new AzureWebInterface(isClient: true);

		public static readonly AzureWebInterface server = new AzureWebInterface(isClient: false);

		public bool IsClient;

		public int MaxRetries = 1;

		public int FlushSize = 1000;

		public TimeSpan FlushDelay = TimeSpan.FromSeconds(30.0);

		private DateTime nextFlush;

		private List<EventRecord> pending = new List<EventRecord>();

		private HttpClient HttpClient = new HttpClient();

		private static readonly MediaTypeHeaderValue JsonContentType = new MediaTypeHeaderValue("application/json")
		{
			CharSet = Encoding.UTF8.WebName
		};

		public int PendingCount => pending.Count;

		public AzureWebInterface(bool isClient)
		{
			IsClient = isClient;
		}

		public void EnqueueEvent(EventRecord point)
		{
			DateTime utcNow = DateTime.UtcNow;
			pending.Add(point);
			if (pending.Count > FlushSize || utcNow > nextFlush)
			{
				nextFlush = utcNow.Add(FlushDelay);
				List<EventRecord> toUpload = pending;
				Task.Run(async delegate
				{
					await UploadAsync(toUpload);
				});
				pending = Pool.GetList<EventRecord>();
			}
		}

		private void SerializeEvents(List<EventRecord> records, MemoryStream stream)
		{
			int num = 0;
			using StreamWriter streamWriter = new StreamWriter(stream, Encoding.UTF8, 1024, leaveOpen: true);
			streamWriter.Write("[");
			foreach (EventRecord record in records)
			{
				SerializeEvent(record, streamWriter, num);
				num++;
			}
			streamWriter.Write("]");
			streamWriter.Flush();
		}

		private void SerializeEvent(EventRecord record, StreamWriter writer, int index)
		{
			//IL_028b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0290: Unknown result type (might be due to invalid IL or missing references)
			//IL_0293: Unknown result type (might be due to invalid IL or missing references)
			//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
			//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
			if (index > 0)
			{
				writer.Write(',');
			}
			writer.Write("{\"Timestamp\":\"");
			writer.Write(record.Timestamp.ToString("o"));
			writer.Write("\",\"Data\":{");
			bool flag = true;
			foreach (EventRecordField datum in record.Data)
			{
				if (flag)
				{
					flag = false;
				}
				else
				{
					writer.Write(',');
				}
				writer.Write("\"");
				writer.Write(datum.Key1);
				if (datum.Key2 != null)
				{
					writer.Write(datum.Key2);
				}
				writer.Write("\":");
				if (!datum.IsObject)
				{
					writer.Write('"');
				}
				if (datum.String != null)
				{
					if (datum.IsObject)
					{
						writer.Write(datum.String);
					}
					else
					{
						string @string = datum.String;
						int length = datum.String.Length;
						for (int i = 0; i < length; i++)
						{
							char c = @string[i];
							if (c == '\\' || c == '"')
							{
								writer.Write('\\');
								writer.Write(c);
								continue;
							}
							switch (c)
							{
							case '\n':
								writer.Write("\\n");
								break;
							case '\r':
								writer.Write("\\r");
								break;
							case '\t':
								writer.Write("\\t");
								break;
							default:
								writer.Write(c);
								break;
							}
						}
					}
				}
				else if (datum.Float.HasValue)
				{
					writer.Write(datum.Float.Value);
				}
				else if (datum.Number.HasValue)
				{
					writer.Write(datum.Number.Value);
				}
				else if (datum.Guid.HasValue)
				{
					writer.Write(datum.Guid.Value.ToString("N"));
				}
				else if (datum.Vector.HasValue)
				{
					writer.Write('(');
					Vector3 value = datum.Vector.Value;
					writer.Write(value.x);
					writer.Write(',');
					writer.Write(value.y);
					writer.Write(',');
					writer.Write(value.z);
					writer.Write(')');
				}
				if (!datum.IsObject)
				{
					writer.Write("\"");
				}
			}
			writer.Write('}');
			writer.Write('}');
		}

		private async Task UploadAsync(List<EventRecord> records)
		{
			MemoryStream stream = Pool.Get<MemoryStream>();
			stream.Position = 0L;
			stream.SetLength(0L);
			try
			{
				SerializeEvents(records, stream);
				AuthTicket ticket = null;
				for (int attempt = 0; attempt < MaxRetries; attempt++)
				{
					try
					{
						using ByteArrayContent content = new ByteArrayContent(stream.GetBuffer(), 0, (int)stream.Length);
						content.Headers.ContentType = JsonContentType;
						if (!string.IsNullOrEmpty(AnalyticsSecret))
						{
							content.Headers.Add(AnalyticsHeader, AnalyticsSecret);
						}
						else
						{
							content.Headers.Add(AnalyticsHeader, AnalyticsPublicKey);
						}
						if (!IsClient)
						{
							content.Headers.Add("X-SERVER-IP", Net.sv.ip);
							content.Headers.Add("X-SERVER-PORT", Net.sv.port.ToString());
						}
						if (UploadAnalytics)
						{
							(await HttpClient.PostAsync(IsClient ? ClientAnalyticsUrl : ServerAnalyticsUrl, content)).EnsureSuccessStatusCode();
						}
					}
					catch (Exception ex4)
					{
						Exception ex3 = ex4;
						if (!(ex3 is HttpRequestException))
						{
							Debug.LogException(ex3);
						}
						goto IL_0276;
					}
					break;
					IL_0276:
					if (ticket != null)
					{
						try
						{
							ticket.Cancel();
						}
						catch (Exception ex4)
						{
							Exception ex2 = ex4;
							Debug.LogError((object)("Failed to cancel auth ticket in analytics: " + ex2.ToString()));
						}
					}
				}
			}
			catch (Exception ex4)
			{
				Exception ex = ex4;
				if (IsClient)
				{
					Debug.LogWarning((object)ex.ToString());
				}
				else
				{
					Debug.LogException(ex);
				}
			}
			finally
			{
				foreach (EventRecord item in records)
				{
					EventRecord i = item;
					Pool.Free<EventRecord>(ref i);
				}
				Pool.FreeList<EventRecord>(ref records);
				Pool.FreeMemoryStream(ref stream);
			}
		}
	}

	public static class Server
	{
		public enum DeathType
		{
			Player,
			NPC,
			AutoTurret
		}

		public static bool Enabled;

		private static Dictionary<string, float> bufferData = null;

		private static TimeSince lastHeldItemEvent;

		private static TimeSince lastAnalyticsSave;

		private static DateTime backupDate;

		private static bool WriteToFile => ConVar.Server.statBackup;

		private static bool CanSendAnalytics => ConVar.Server.official && ConVar.Server.stats && Enabled;

		private static DateTime currentDate => DateTime.Now;

		internal static void Death(BaseEntity initiator, BaseEntity weaponPrefab, Vector3 worldPosition)
		{
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			if (!CanSendAnalytics || !((Object)(object)initiator != (Object)null))
			{
				return;
			}
			if (initiator is BasePlayer)
			{
				if ((Object)(object)weaponPrefab != (Object)null)
				{
					Death(weaponPrefab.ShortPrefabName, worldPosition, initiator.IsNpc ? DeathType.NPC : DeathType.Player);
				}
				else
				{
					Death("player", worldPosition);
				}
			}
			else if (initiator is AutoTurret)
			{
				if ((Object)(object)weaponPrefab != (Object)null)
				{
					Death(weaponPrefab.ShortPrefabName, worldPosition, DeathType.AutoTurret);
				}
			}
			else
			{
				Death(initiator.Categorize(), worldPosition, initiator.IsNpc ? DeathType.NPC : DeathType.Player);
			}
		}

		internal static void Death(string v, Vector3 worldPosition, DeathType deathType = DeathType.Player)
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			if (!CanSendAnalytics)
			{
				return;
			}
			string monumentStringFromPosition = GetMonumentStringFromPosition(worldPosition);
			if (!string.IsNullOrEmpty(monumentStringFromPosition))
			{
				switch (deathType)
				{
				case DeathType.Player:
					DesignEvent("player:" + monumentStringFromPosition + "death:" + v);
					break;
				case DeathType.NPC:
					DesignEvent("player:" + monumentStringFromPosition + "death:npc:" + v);
					break;
				case DeathType.AutoTurret:
					DesignEvent("player:" + monumentStringFromPosition + "death:autoturret:" + v);
					break;
				}
			}
			else
			{
				switch (deathType)
				{
				case DeathType.Player:
					DesignEvent("player:death:" + v);
					break;
				case DeathType.NPC:
					DesignEvent("player:death:npc:" + v);
					break;
				case DeathType.AutoTurret:
					DesignEvent("player:death:autoturret:" + v);
					break;
				}
			}
		}

		private static string GetMonumentStringFromPosition(Vector3 worldPosition)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			MonumentInfo monumentInfo = TerrainMeta.Path.FindMonumentWithBoundsOverlap(worldPosition);
			if ((Object)(object)monumentInfo != (Object)null && !string.IsNullOrEmpty(monumentInfo.displayPhrase.token))
			{
				return monumentInfo.displayPhrase.token;
			}
			if ((Object)(object)SingletonComponent<EnvironmentManager>.Instance != (Object)null && (EnvironmentManager.Get(worldPosition) & EnvironmentType.TrainTunnels) == EnvironmentType.TrainTunnels)
			{
				return "train_tunnel_display_name";
			}
			return string.Empty;
		}

		public static void Crafting(string targetItemShortname, int skinId)
		{
			if (CanSendAnalytics)
			{
				DesignEvent("player:craft:" + targetItemShortname);
				SkinUsed(targetItemShortname, skinId);
			}
		}

		public static void SkinUsed(string itemShortName, int skinId)
		{
			if (CanSendAnalytics && skinId != 0)
			{
				DesignEvent($"skinUsed:{itemShortName}:{skinId}");
			}
		}

		public static void ExcavatorStarted()
		{
			if (CanSendAnalytics)
			{
				DesignEvent("monuments:excavatorstarted");
			}
		}

		public static void ExcavatorStopped(float activeDuration)
		{
			if (CanSendAnalytics)
			{
				DesignEvent("monuments:excavatorstopped", activeDuration);
			}
		}

		public static void SlotMachineTransaction(int scrapSpent, int scrapReceived)
		{
			if (CanSendAnalytics)
			{
				DesignEvent("slots:scrapSpent", scrapSpent);
				DesignEvent("slots:scrapReceived", scrapReceived);
			}
		}

		public static void VehiclePurchased(string vehicleType)
		{
			if (CanSendAnalytics)
			{
				DesignEvent("vehiclePurchased:" + vehicleType);
			}
		}

		public static void FishCaught(ItemDefinition fish)
		{
			if (CanSendAnalytics && !((Object)(object)fish == (Object)null))
			{
				DesignEvent("fishCaught:" + fish.shortname);
			}
		}

		public static void VendingMachineTransaction(NPCVendingOrder npcVendingOrder, ItemDefinition purchased, int amount)
		{
			if (CanSendAnalytics && !((Object)(object)purchased == (Object)null))
			{
				if ((Object)(object)npcVendingOrder == (Object)null)
				{
					DesignEvent("vendingPurchase:player:" + purchased.shortname, amount);
				}
				else
				{
					DesignEvent("vendingPurchase:static:" + purchased.shortname, amount);
				}
			}
		}

		public static void Consume(string consumedItem)
		{
			if (CanSendAnalytics && !string.IsNullOrEmpty(consumedItem))
			{
				DesignEvent("player:consume:" + consumedItem);
			}
		}

		public static void TreeKilled(BaseEntity withWeapon)
		{
			if (CanSendAnalytics)
			{
				if ((Object)(object)withWeapon != (Object)null)
				{
					DesignEvent("treekilled:" + withWeapon.ShortPrefabName);
				}
				else
				{
					DesignEvent("treekilled");
				}
			}
		}

		public static void OreKilled(OreResourceEntity entity, HitInfo info)
		{
			ResourceDispenser resourceDispenser = default(ResourceDispenser);
			if (CanSendAnalytics && ((Component)entity).TryGetComponent<ResourceDispenser>(ref resourceDispenser) && resourceDispenser.containedItems.Count > 0 && (Object)(object)resourceDispenser.containedItems[0].itemDef != (Object)null)
			{
				if ((Object)(object)info.WeaponPrefab != (Object)null)
				{
					DesignEvent("orekilled:" + resourceDispenser.containedItems[0].itemDef.shortname + ":" + info.WeaponPrefab.ShortPrefabName);
				}
				else
				{
					DesignEvent($"orekilled:{resourceDispenser.containedItems[0]}");
				}
			}
		}

		public static void MissionComplete(BaseMission mission)
		{
			if (CanSendAnalytics)
			{
				DesignEvent("missionComplete:" + mission.shortname, canBackup: true);
			}
		}

		public static void MissionFailed(BaseMission mission, BaseMission.MissionFailReason reason)
		{
			if (CanSendAnalytics)
			{
				DesignEvent($"missionFailed:{mission.shortname}:{reason}", canBackup: true);
			}
		}

		public static void FreeUnderwaterCrate()
		{
			if (CanSendAnalytics)
			{
				DesignEvent("loot:freeUnderWaterCrate");
			}
		}

		public static void HeldItemDeployed(ItemDefinition def)
		{
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			if (CanSendAnalytics && !(TimeSince.op_Implicit(lastHeldItemEvent) < 0.1f))
			{
				lastHeldItemEvent = TimeSince.op_Implicit(0f);
				DesignEvent("heldItemDeployed:" + def.shortname);
			}
		}

		public static void UsedZipline()
		{
			if (CanSendAnalytics)
			{
				DesignEvent("usedZipline");
			}
		}

		public static void ReportCandiesCollectedByPlayer(int count)
		{
			if (Enabled)
			{
				DesignEvent("halloween:candiesCollected", count);
			}
		}

		public static void ReportPlayersParticipatedInHalloweenEvent(int count)
		{
			if (Enabled)
			{
				DesignEvent("halloween:playersParticipated", count);
			}
		}

		public static void Trigger(string message)
		{
			if (CanSendAnalytics && !string.IsNullOrEmpty(message))
			{
				DesignEvent(message);
			}
		}

		private static void DesignEvent(string message, bool canBackup = false)
		{
			if (CanSendAnalytics && !string.IsNullOrEmpty(message))
			{
				GA.DesignEvent(message);
				if (canBackup)
				{
					LocalBackup(message, 1f);
				}
			}
		}

		private static void DesignEvent(string message, float value, bool canBackup = false)
		{
			if (CanSendAnalytics && !string.IsNullOrEmpty(message))
			{
				GA.DesignEvent(message, value);
				if (canBackup)
				{
					LocalBackup(message, value);
				}
			}
		}

		private static void DesignEvent(string message, int value, bool canBackup = false)
		{
			if (CanSendAnalytics && !string.IsNullOrEmpty(message))
			{
				GA.DesignEvent(message, (float)value);
				if (canBackup)
				{
					LocalBackup(message, value);
				}
			}
		}

		private static string GetBackupPath(DateTime date)
		{
			return string.Format("{0}/{1}_{2}_{3}_analytics_backup.txt", ConVar.Server.GetServerFolder("analytics"), date.Day, date.Month, date.Year);
		}

		private static void LocalBackup(string message, float value)
		{
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_0102: Unknown result type (might be due to invalid IL or missing references)
			if (!WriteToFile)
			{
				return;
			}
			if (bufferData != null && backupDate.Date != currentDate.Date)
			{
				SaveBufferIntoDateFile(backupDate);
				bufferData.Clear();
				backupDate = currentDate;
			}
			if (bufferData == null)
			{
				if (bufferData == null)
				{
					bufferData = new Dictionary<string, float>();
				}
				lastAnalyticsSave = TimeSince.op_Implicit(0f);
				backupDate = currentDate;
			}
			if (bufferData.ContainsKey(message))
			{
				bufferData[message] += value;
			}
			else
			{
				bufferData.Add(message, value);
			}
			if (TimeSince.op_Implicit(lastAnalyticsSave) > 120f)
			{
				lastAnalyticsSave = TimeSince.op_Implicit(0f);
				SaveBufferIntoDateFile(currentDate);
				bufferData.Clear();
			}
			static void MergeBuffers(Dictionary<string, float> target, Dictionary<string, float> destination)
			{
				foreach (KeyValuePair<string, float> item in target)
				{
					if (destination.ContainsKey(item.Key))
					{
						destination[item.Key] += item.Value;
					}
					else
					{
						destination.Add(item.Key, item.Value);
					}
				}
			}
			static void SaveBufferIntoDateFile(DateTime date)
			{
				string backupPath = GetBackupPath(date);
				if (File.Exists(backupPath))
				{
					Dictionary<string, float> dictionary = (Dictionary<string, float>)JsonConvert.DeserializeObject(File.ReadAllText(backupPath), typeof(Dictionary<string, float>));
					if (dictionary != null)
					{
						MergeBuffers(dictionary, bufferData);
					}
				}
				string contents = JsonConvert.SerializeObject((object)bufferData);
				File.WriteAllText(GetBackupPath(date), contents);
			}
		}
	}

	public static HashSet<string> StatsBlacklist;

	private static HashSet<NetworkableId> trackedSpawnedIds = new HashSet<NetworkableId>();

	public static string ClientAnalyticsUrl { get; set; } = "https://rust-api.facepunch.com/api/public/analytics/rust/client";


	[ServerVar(Name = "server_analytics_url")]
	public static string ServerAnalyticsUrl { get; set; } = "https://rust-api.facepunch.com/api/public/analytics/rust/server";


	[ServerVar(Name = "analytics_header", Saved = true)]
	public static string AnalyticsHeader { get; set; } = "X-API-KEY";


	[ServerVar(Name = "analytics_enabled")]
	public static bool UploadAnalytics { get; set; } = true;


	[ServerVar(Name = "analytics_secret", Saved = true)]
	public static string AnalyticsSecret { get; set; } = "";


	public static string AnalyticsPublicKey { get; set; } = "pub878ABLezSB6onshSwBCRGYDCpEI";


	[ServerVar(Name = "high_freq_stats", Saved = true)]
	public static bool HighFrequencyStats { get; set; } = true;


	[ServerVar(Name = "stats_blacklist", Saved = true)]
	public static string stats_blacklist
	{
		get
		{
			return (StatsBlacklist == null) ? "" : string.Join(",", StatsBlacklist);
		}
		set
		{
			if (string.IsNullOrEmpty(value))
			{
				StatsBlacklist = null;
				return;
			}
			string[] collection = value.Split(',');
			StatsBlacklist = new HashSet<string>(collection);
		}
	}

	[ClientVar(Name = "pending_analytics")]
	[ServerVar(Name = "pending_analytics")]
	public static void GetPendingAnalytics(Arg arg)
	{
		int pendingCount = AzureWebInterface.server.PendingCount;
		arg.ReplyWith($"Pending: {pendingCount}");
	}
}
