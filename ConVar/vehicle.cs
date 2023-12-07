using UnityEngine;

namespace ConVar;

[Factory("vehicle")]
public class vehicle : ConsoleSystem
{
	[ServerVar]
	[Help("how long until boat corpses despawn (excluding tugboat - use tugboat_corpse_seconds)")]
	public static float boat_corpse_seconds = 300f;

	[ServerVar(Help = "If true, trains always explode when destroyed, and hitting a barrier always destroys the train immediately. Default: false")]
	public static bool cinematictrains = false;

	[ServerVar(Help = "Determines whether trains stop automatically when there's no-one on them. Default: false")]
	public static bool trainskeeprunning = false;

	[ServerVar(Help = "Determines whether modular cars turn into wrecks when destroyed, or just immediately gib. Default: true")]
	public static bool carwrecks = true;

	[ServerVar(Help = "Determines whether vehicles drop storage items when destroyed. Default: true")]
	public static bool vehiclesdroploot = true;

	[ServerUserVar]
	public static void swapseats(Arg arg)
	{
		int targetSeat = 0;
		BasePlayer basePlayer = arg.Player();
		if ((Object)(object)basePlayer == (Object)null || basePlayer.SwapSeatCooldown())
		{
			return;
		}
		BaseMountable mounted = basePlayer.GetMounted();
		if (!((Object)(object)mounted == (Object)null))
		{
			BaseVehicle baseVehicle = ((Component)mounted).GetComponent<BaseVehicle>();
			if ((Object)(object)baseVehicle == (Object)null)
			{
				baseVehicle = mounted.VehicleParent();
			}
			if (!((Object)(object)baseVehicle == (Object)null))
			{
				baseVehicle.SwapSeats(basePlayer, targetSeat);
			}
		}
	}

	[ServerVar]
	public static void fixcars(Arg arg)
	{
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		BasePlayer basePlayer = arg.Player();
		if ((Object)(object)basePlayer == (Object)null)
		{
			arg.ReplyWith("Null player.");
			return;
		}
		if (!basePlayer.IsAdmin)
		{
			arg.ReplyWith("Must be an admin to use fixcars.");
			return;
		}
		int @int = arg.GetInt(0, 2);
		@int = Mathf.Clamp(@int, 1, 3);
		BaseVehicle[] array = BaseEntity.Util.FindAll<BaseVehicle>();
		int num = 0;
		BaseVehicle[] array2 = array;
		foreach (BaseVehicle baseVehicle in array2)
		{
			if (baseVehicle.isServer && Vector3.Distance(((Component)baseVehicle).transform.position, ((Component)basePlayer).transform.position) <= 10f && baseVehicle.AdminFixUp(@int))
			{
				num++;
			}
		}
		MLRS[] array3 = BaseEntity.Util.FindAll<MLRS>();
		foreach (MLRS mLRS in array3)
		{
			if (mLRS.isServer && Vector3.Distance(((Component)mLRS).transform.position, ((Component)basePlayer).transform.position) <= 10f && mLRS.AdminFixUp())
			{
				num++;
			}
		}
		arg.ReplyWith($"Fixed up {num} vehicles.");
	}

	[ServerVar]
	public static void autohover(Arg arg)
	{
		BasePlayer basePlayer = arg.Player();
		if ((Object)(object)basePlayer == (Object)null)
		{
			arg.ReplyWith("Null player.");
			return;
		}
		if (!basePlayer.IsAdmin)
		{
			arg.ReplyWith("Must be an admin to use autohover.");
			return;
		}
		BaseHelicopter baseHelicopter = basePlayer.GetMountedVehicle() as BaseHelicopter;
		if ((Object)(object)baseHelicopter != (Object)null)
		{
			bool flag = baseHelicopter.ToggleAutoHover(basePlayer);
			arg.ReplyWith($"Toggled auto-hover to {flag}.");
		}
		else
		{
			arg.ReplyWith("Must be mounted in a helicopter first.");
		}
	}

	[ServerVar]
	public static void stop_all_trains(Arg arg)
	{
		TrainEngine[] array = Object.FindObjectsOfType<TrainEngine>();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].StopEngine();
		}
		arg.ReplyWith("All trains stopped.");
	}

	[ServerVar]
	public static void killcars(Arg args)
	{
		ModularCar[] array = BaseEntity.Util.FindAll<ModularCar>();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Kill();
		}
	}

	[ServerVar]
	public static void killminis(Arg args)
	{
		PlayerHelicopter[] array = BaseEntity.Util.FindAll<PlayerHelicopter>();
		foreach (PlayerHelicopter playerHelicopter in array)
		{
			if (((Object)playerHelicopter).name.ToLower().Contains("minicopter"))
			{
				playerHelicopter.Kill();
			}
		}
	}

	[ServerVar]
	public static void killscraphelis(Arg args)
	{
		ScrapTransportHelicopter[] array = BaseEntity.Util.FindAll<ScrapTransportHelicopter>();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Kill();
		}
	}

	[ServerVar]
	public static void killtrains(Arg args)
	{
		TrainCar[] array = BaseEntity.Util.FindAll<TrainCar>();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Kill();
		}
	}

	[ServerVar]
	public static void killboats(Arg args)
	{
		BaseBoat[] array = BaseEntity.Util.FindAll<BaseBoat>();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Kill();
		}
	}

	[ServerVar]
	public static void killdrones(Arg args)
	{
		Drone[] array = BaseEntity.Util.FindAll<Drone>();
		foreach (Drone drone in array)
		{
			if (!(drone is DeliveryDrone))
			{
				drone.Kill();
			}
		}
	}

	[ServerVar(Help = "Print out boat drift status for all boats")]
	public static void boatdriftinfo(Arg args)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		TextTable val = new TextTable();
		val.AddColumn("id");
		val.AddColumn("name");
		val.AddColumn("position");
		val.AddColumn("status");
		val.AddColumn("drift");
		BaseBoat[] array = BaseEntity.Util.FindAll<BaseBoat>();
		BaseBoat[] array2 = array;
		foreach (BaseBoat baseBoat in array2)
		{
			if (baseBoat.IsValid())
			{
				string text = (baseBoat.IsAlive() ? "alive" : "dead");
				string driftStatus = baseBoat.GetDriftStatus();
				string[] obj = new string[5]
				{
					((object)(NetworkableId)(ref baseBoat.net.ID)).ToString(),
					baseBoat.ShortPrefabName,
					null,
					null,
					null
				};
				Vector3 position = ((Component)baseBoat).transform.position;
				obj[2] = ((object)(Vector3)(ref position)).ToString();
				obj[3] = text;
				obj[4] = driftStatus;
				val.AddRow(obj);
			}
		}
		if (array.Length == 0)
		{
			args.ReplyWith("No boats in world");
		}
		args.ReplyWith(((object)val).ToString());
	}
}
