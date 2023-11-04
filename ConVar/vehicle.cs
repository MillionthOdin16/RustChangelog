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
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
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
		BaseVehicle[] array = Object.FindObjectsOfType<BaseVehicle>();
		int num = 0;
		BaseVehicle[] array2 = array;
		foreach (BaseVehicle baseVehicle in array2)
		{
			if (baseVehicle.isServer && Vector3.Distance(((Component)baseVehicle).transform.position, ((Component)basePlayer).transform.position) <= 10f && baseVehicle.AdminFixUp(@int))
			{
				num++;
			}
		}
		MLRS[] array3 = Object.FindObjectsOfType<MLRS>();
		MLRS[] array4 = array3;
		foreach (MLRS mLRS in array4)
		{
			if (mLRS.isServer && Vector3.Distance(((Component)mLRS).transform.position, ((Component)basePlayer).transform.position) <= 10f && mLRS.AdminFixUp())
			{
				num++;
			}
		}
		arg.ReplyWith($"Fixed up {num} vehicles.");
	}

	[ServerVar]
	public static void stop_all_trains(Arg arg)
	{
		TrainEngine[] array = Object.FindObjectsOfType<TrainEngine>();
		TrainEngine[] array2 = array;
		foreach (TrainEngine trainEngine in array2)
		{
			trainEngine.StopEngine();
		}
		arg.ReplyWith("All trains stopped.");
	}

	[ServerVar]
	public static void killcars(Arg args)
	{
		ModularCar[] array = BaseEntity.Util.FindAll<ModularCar>();
		foreach (ModularCar modularCar in array)
		{
			modularCar.Kill();
		}
	}

	[ServerVar]
	public static void killminis(Arg args)
	{
		MiniCopter[] array = BaseEntity.Util.FindAll<MiniCopter>();
		foreach (MiniCopter miniCopter in array)
		{
			if (((Object)miniCopter).name.ToLower().Contains("minicopter"))
			{
				miniCopter.Kill();
			}
		}
	}

	[ServerVar]
	public static void killscraphelis(Arg args)
	{
		ScrapTransportHelicopter[] array = BaseEntity.Util.FindAll<ScrapTransportHelicopter>();
		foreach (ScrapTransportHelicopter scrapTransportHelicopter in array)
		{
			scrapTransportHelicopter.Kill();
		}
	}

	[ServerVar]
	public static void killtrains(Arg args)
	{
		TrainCar[] array = BaseEntity.Util.FindAll<TrainCar>();
		foreach (TrainCar trainCar in array)
		{
			trainCar.Kill();
		}
	}

	[ServerVar]
	public static void killboats(Arg args)
	{
		BaseBoat[] array = BaseEntity.Util.FindAll<BaseBoat>();
		foreach (BaseBoat baseBoat in array)
		{
			baseBoat.Kill();
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
}
