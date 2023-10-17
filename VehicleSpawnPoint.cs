using UnityEngine;

public class VehicleSpawnPoint : SpaceCheckingSpawnPoint
{
	public override void ObjectSpawned(SpawnPointInstance instance)
	{
		base.ObjectSpawned(instance);
		AddStartingFuel(((Component)instance).gameObject.ToBaseEntity() as BaseVehicle);
	}

	public static void AddStartingFuel(BaseVehicle vehicle)
	{
		if (!((Object)(object)vehicle == (Object)null))
		{
			vehicle.GetFuelSystem()?.AddStartingFuel(vehicle.StartingFuelUnits());
		}
	}
}
