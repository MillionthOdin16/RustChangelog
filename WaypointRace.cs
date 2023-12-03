using System.Collections.Generic;
using Facepunch;
using ProtoBuf;
using UnityEngine;

public class WaypointRace : BaseEntity
{
	private class PendingRaceResults
	{
		private class Completion
		{
			public List<BasePlayer> players = new List<BasePlayer>();

			public float time;

			public bool valid;
		}

		private List<Completion> Completions = new List<Completion>();

		public int totalParticipants;

		public int RegisterCompletion(List<BasePlayer> forPlayers, float time, bool valid)
		{
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Expected O, but got Unknown
			Completion completion = new Completion();
			foreach (BasePlayer forPlayer in forPlayers)
			{
				completion.players.Add(forPlayer);
			}
			completion.time = time;
			completion.valid = valid;
			Completions.Add(completion);
			if (Completions.Count == totalParticipants)
			{
				TextTable val = new TextTable();
				val.AddColumns(new string[3] { "Place", "Players", "Time" });
				for (int i = 0; i < Completions.Count; i++)
				{
					Completion completion2 = Completions[i];
					if (!completion2.valid)
					{
						continue;
					}
					string text = "";
					foreach (BasePlayer player in completion2.players)
					{
						text = text + player.displayName + ",";
					}
					val.AddRow(new string[3]
					{
						$"P{i + 1}",
						text,
						MathEx.SnapTo(completion2.time, 0.1f).ToString()
					});
				}
				string msg = ((object)val).ToString();
				foreach (Completion completion3 in Completions)
				{
					foreach (BasePlayer player2 in completion3.players)
					{
						player2.ChatMessage(msg);
					}
				}
			}
			return Completions.Count;
		}
	}

	public float WaypointRadius = 10f;

	public RaceWaypointVisual TargetWaypointVisual;

	public RaceWaypointVisual NextWaypointVisual;

	private List<Vector3> racePoints = new List<Vector3>();

	private EntityRef<BaseVehicle> racingVehicle;

	private int currentWaypoint;

	public static Phrase stageNotifyPhrase = new Phrase("race_notify", "Reached checkpoint {0}/{1} : {2}s");

	public static Phrase raceCompletePhrase = new Phrase("race_complete", "Finished race {0}/{1} in {2}s");

	private PendingRaceResults raceResults;

	private TimeSince startTime;

	[ServerVar(Saved = true, Help = "How long a race can go until it times out (in seconds)")]
	public static float raceTimeout = 900f;

	[ServerVar]
	public static void startRace(Arg arg)
	{
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		string @string = arg.GetString(0, "");
		List<Transform> list = Pool.GetList<Transform>();
		WaypointRaceTarget.GetWaypoints(@string, list);
		if (list.Count == 0)
		{
			arg.ReplyWith("Couldn't find any waypoints for " + @string + ", is the name correct?");
			return;
		}
		Debug.Log((object)$"Starting race {@string} with {list.Count} waypoints");
		List<BasePlayer> list2 = Pool.GetList<BasePlayer>();
		Vis.Entities(list[0].position, 30f, list2, 131072, (QueryTriggerInteraction)2);
		List<BaseVehicle> list3 = Pool.GetList<BaseVehicle>();
		for (int i = 0; i < list2.Count; i++)
		{
			if (list2[i].isClient)
			{
				list2.RemoveAt(i);
				i--;
				continue;
			}
			if (!list2[i].isMounted)
			{
				Debug.Log((object)("Remove player " + list2[i].displayName + " from race, not mounted"));
				list2.RemoveAt(i);
				i--;
				continue;
			}
			BaseMountable mounted = list2[i].GetMounted();
			if ((Object)(object)mounted == (Object)null || (Object)(object)mounted.VehicleParent() == (Object)null)
			{
				Debug.Log((object)("Remove player " + list2[i].displayName + " from race, no vehicle"));
				list2.RemoveAt(i);
				i--;
				continue;
			}
			BaseVehicle baseVehicle = mounted.VehicleParent();
			if (!baseVehicle.IsDriver(list2[i]))
			{
				Debug.Log((object)("Remove player " + list2[i].displayName + " from race, not a driver"));
				list2.RemoveAt(i);
				i--;
			}
			else if (!list3.Contains(baseVehicle))
			{
				list3.Add(baseVehicle);
			}
		}
		PendingRaceResults results = new PendingRaceResults
		{
			totalParticipants = list3.Count
		};
		foreach (BaseVehicle item in list3)
		{
			WaypointRace obj = GameManager.server.CreateEntity("assets/prefabs/misc/waypointrace/waypointrace.prefab", ((Component)item).transform.position, Quaternion.identity) as WaypointRace;
			obj.Setup(list, item, results);
			obj.Spawn();
		}
		Pool.FreeList<BasePlayer>(ref list2);
		Pool.FreeList<BaseVehicle>(ref list3);
		Pool.FreeList<Transform>(ref list);
	}

	private void Setup(List<Transform> waypoints, BaseVehicle participant, PendingRaceResults results)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		racingVehicle.Set(participant);
		foreach (Transform waypoint in waypoints)
		{
			racePoints.Add(waypoint.position);
		}
		raceResults = results;
		startTime = TimeSince.op_Implicit(0f);
	}

	public override void Save(SaveInfo info)
	{
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		base.Save(info);
		if (info.msg.waypointRace == null)
		{
			info.msg.waypointRace = Pool.Get<WaypointRace>();
		}
		info.msg.waypointRace.positions = Pool.GetList<Vector3>();
		info.msg.waypointRace.positions.Clear();
		foreach (Vector3 racePoint in racePoints)
		{
			info.msg.waypointRace.positions.Add(racePoint);
		}
		info.msg.waypointRace.racingVehicle = racingVehicle.uid;
		info.msg.waypointRace.currentWaypoint = currentWaypoint;
	}

	public override void Load(LoadInfo info)
	{
		base.Load(info);
	}

	private void Update()
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		if (!base.isServer)
		{
			return;
		}
		BaseVehicle baseVehicle = racingVehicle.Get(base.isServer);
		if ((Object)(object)baseVehicle == (Object)null || baseVehicle.IsDestroyed || baseVehicle.IsDead() || TimeSince.op_Implicit(startTime) > raceTimeout)
		{
			raceResults.RegisterCompletion(new List<BasePlayer>(), TimeSince.op_Implicit(startTime), valid: false);
			Kill();
			return;
		}
		((Component)this).transform.position = ((Component)baseVehicle).transform.position;
		if (racePoints.Count <= currentWaypoint + 1)
		{
			return;
		}
		Vector3 val = racePoints[currentWaypoint + 1];
		Vector3 val2 = ((Component)baseVehicle).transform.position - val;
		if (!(((Vector3)(ref val2)).sqrMagnitude <= WaypointRadius * WaypointRadius))
		{
			return;
		}
		currentWaypoint++;
		List<BasePlayer> list = Pool.GetList<BasePlayer>();
		baseVehicle.GetMountedPlayers(list);
		if (currentWaypoint >= racePoints.Count - 1)
		{
			int num = raceResults.RegisterCompletion(list, TimeSince.op_Implicit(startTime), valid: true);
			foreach (BasePlayer item in list)
			{
				item.ShowToast(GameTip.Styles.Blue_Normal, raceCompletePhrase, num.ToString(), raceResults.totalParticipants.ToString(), MathEx.SnapTo(TimeSince.op_Implicit(startTime), 0.1f).ToString());
			}
			Kill();
		}
		else
		{
			foreach (BasePlayer item2 in list)
			{
				item2.ShowToast(GameTip.Styles.Blue_Normal, stageNotifyPhrase, currentWaypoint.ToString(), (racePoints.Count - 1).ToString(), MathEx.SnapTo(TimeSince.op_Implicit(startTime), 0.1f).ToString());
			}
			SendNetworkUpdate();
		}
		Pool.FreeList<BasePlayer>(ref list);
	}
}
