using System.Collections.Generic;
using System.Linq;
using ConVar;
using UnityEngine;

public class CombatLog
{
	public struct Event
	{
		public float time;

		public ulong attacker_id;

		public ulong target_id;

		public string attacker;

		public string target;

		public string weapon;

		public string ammo;

		public string bone;

		public HitArea area;

		public float distance;

		public float health_old;

		public float health_new;

		public string info;

		public int proj_hits;

		public float proj_integrity;

		public float proj_travel;

		public float proj_mismatch;

		public int desync;

		public bool attacker_dead;
	}

	private const string selfname = "you";

	private const string noname = "N/A";

	private BasePlayer player;

	private Queue<Event> storage;

	private static Dictionary<ulong, Queue<Event>> players = new Dictionary<ulong, Queue<Event>>();

	public float LastActive { get; private set; }

	public CombatLog(BasePlayer player)
	{
		this.player = player;
	}

	public void Init()
	{
		storage = Get(player.userID);
		LastActive = storage.LastOrDefault().time;
	}

	public void Save()
	{
	}

	public void LogInvalid(BasePlayer player, AttackEntity weapon, string description)
	{
		Log(player, weapon, null, description);
	}

	public void LogInvalid(HitInfo info, string description)
	{
		Log(info.Initiator, info.Weapon, info.HitEntity as BaseCombatEntity, description, info.ProjectilePrefab, info.ProjectileID, -1f, info);
	}

	public void LogAttack(HitInfo info, string description, float oldHealth = -1f)
	{
		Log(info.Initiator, info.Weapon, info.HitEntity as BaseCombatEntity, description, info.ProjectilePrefab, info.ProjectileID, oldHealth, info);
	}

	public void Log(BaseEntity attacker, AttackEntity weapon, BaseCombatEntity hitEntity, string description, Projectile projectilePrefab = null, int projectileId = -1, float healthOld = -1f, HitInfo hitInfo = null)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		Event val = default(Event);
		float distance = 0f;
		if (hitInfo != null)
		{
			distance = (hitInfo.IsProjectile() ? hitInfo.ProjectileDistance : Vector3.Distance(hitInfo.PointStart, hitInfo.HitPositionWorld));
			if (hitInfo.Initiator is BasePlayer basePlayer && (Object)(object)hitInfo.HitEntity != (Object)(object)hitInfo.Initiator)
			{
				val.attacker_dead = basePlayer.IsDead() || basePlayer.IsWounded();
			}
		}
		float health_new = (((Object)(object)hitEntity != (Object)null) ? hitEntity.Health() : 0f);
		val.time = Time.realtimeSinceStartup;
		val.attacker_id = ((NetworkableId)(((Object)(object)attacker != (Object)null && attacker.net != null) ? attacker.net.ID : default(NetworkableId))).Value;
		val.target_id = ((NetworkableId)(((Object)(object)hitEntity != (Object)null && hitEntity.net != null) ? hitEntity.net.ID : default(NetworkableId))).Value;
		val.attacker = (((Object)(object)player == (Object)(object)attacker) ? "you" : (attacker?.ShortPrefabName ?? "N/A"));
		val.target = (((Object)(object)player == (Object)(object)hitEntity) ? "you" : (hitEntity?.ShortPrefabName ?? "N/A"));
		val.weapon = (((Object)(object)weapon != (Object)null) ? ((Object)weapon).name : "N/A");
		val.ammo = ((!((Object)(object)projectilePrefab != (Object)null)) ? "N/A" : ((projectilePrefab != null) ? ((Object)projectilePrefab).name : null));
		val.bone = hitInfo?.boneName ?? "N/A";
		val.area = hitInfo?.boneArea ?? ((HitArea)0);
		val.distance = distance;
		val.health_old = ((healthOld == -1f) ? 0f : healthOld);
		val.health_new = health_new;
		val.info = description ?? string.Empty;
		val.proj_hits = hitInfo?.ProjectileHits ?? 0;
		val.proj_integrity = hitInfo?.ProjectileIntegrity ?? 0f;
		val.proj_travel = hitInfo?.ProjectileTravelTime ?? 0f;
		val.proj_mismatch = hitInfo?.ProjectileTrajectoryMismatch ?? 0f;
		BasePlayer basePlayer2 = attacker as BasePlayer;
		if ((Object)(object)basePlayer2 != (Object)null && (Object)(object)projectilePrefab != (Object)null && basePlayer2.firedProjectiles.TryGetValue(projectileId, out var value))
		{
			val.desync = (int)(value.desyncLifeTime * 1000f);
		}
		Log(val);
	}

	private void Log(Event val)
	{
		LastActive = Time.realtimeSinceStartup;
		if (storage != null)
		{
			storage.Enqueue(val);
			int num = Mathf.Max(0, Server.combatlogsize);
			while (storage.Count > num)
			{
				storage.Dequeue();
			}
		}
	}

	public string Get(int count, NetworkableId filterByAttacker = default(NetworkableId), bool json = false, bool isAdmin = false, ulong requestingUser = 0uL)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Expected O, but got Unknown
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		if (storage == null)
		{
			return string.Empty;
		}
		if (storage.Count == 0 && !json)
		{
			return "Combat log empty.";
		}
		TextTable val = new TextTable();
		val.AddColumn("time");
		val.AddColumn("attacker");
		val.AddColumn("id");
		val.AddColumn("target");
		val.AddColumn("id");
		val.AddColumn("weapon");
		val.AddColumn("ammo");
		val.AddColumn("area");
		val.AddColumn("distance");
		val.AddColumn("old_hp");
		val.AddColumn("new_hp");
		val.AddColumn("info");
		val.AddColumn("hits");
		val.AddColumn("integrity");
		val.AddColumn("travel");
		val.AddColumn("mismatch");
		val.AddColumn("desync");
		int num = storage.Count - count;
		int combatlogdelay = Server.combatlogdelay;
		int num2 = 0;
		BaseGameMode activeGameMode = BaseGameMode.GetActiveGameMode(serverside: true);
		foreach (Event item in storage)
		{
			if (num > 0)
			{
				num--;
			}
			else
			{
				if ((((NetworkableId)(ref filterByAttacker)).IsValid && item.attacker_id != filterByAttacker.Value) || ((Object)(object)activeGameMode != (Object)null && !activeGameMode.returnValidCombatlog && !isAdmin && item.proj_hits > 0))
				{
					continue;
				}
				float num3 = Time.realtimeSinceStartup - item.time;
				if (num3 >= (float)combatlogdelay)
				{
					string text = num3.ToString("0.00s");
					string attacker = item.attacker;
					ulong attacker_id = item.attacker_id;
					string text2 = attacker_id.ToString();
					string target = item.target;
					attacker_id = item.target_id;
					string text3 = attacker_id.ToString();
					string weapon = item.weapon;
					string ammo = item.ammo;
					string text4 = HitAreaUtil.Format(item.area).ToLower();
					float distance = item.distance;
					string text5 = distance.ToString("0.0m");
					distance = item.health_old;
					string text6 = distance.ToString("0.0");
					distance = item.health_new;
					string text7 = distance.ToString("0.0");
					string text8 = item.info;
					if (!player.IsDestroyed && player.userID == requestingUser && item.attacker_dead)
					{
						text8 = "you died first (" + text8 + ")";
					}
					int proj_hits = item.proj_hits;
					string text9 = proj_hits.ToString();
					distance = item.proj_integrity;
					string text10 = distance.ToString("0.00");
					distance = item.proj_travel;
					string text11 = distance.ToString("0.00s");
					distance = item.proj_mismatch;
					string text12 = distance.ToString("0.00m");
					proj_hits = item.desync;
					string text13 = proj_hits.ToString();
					val.AddRow(new string[17]
					{
						text, attacker, text2, target, text3, weapon, ammo, text4, text5, text6,
						text7, text8, text9, text10, text11, text12, text13
					});
				}
				else
				{
					num2++;
				}
			}
		}
		string text14;
		if (json)
		{
			text14 = val.ToJson();
		}
		else
		{
			text14 = ((object)val).ToString();
			if (num2 > 0)
			{
				text14 = text14 + "+ " + num2 + " " + ((num2 > 1) ? "events" : "event");
				text14 = text14 + " in the last " + combatlogdelay + " " + ((combatlogdelay > 1) ? "seconds" : "second");
			}
		}
		return text14;
	}

	public static Queue<Event> Get(ulong id)
	{
		if (players.TryGetValue(id, out var value))
		{
			return value;
		}
		value = new Queue<Event>();
		players.Add(id, value);
		return value;
	}
}
