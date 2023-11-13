using UnityEngine;

public class VehicleSpawnPoint : SpaceCheckingSpawnPoint
{
	public override void ObjectSpawned(SpawnPointInstance instance)
	{
		base.ObjectSpawned(instance);
		BaseEntity baseEntity = ((Component)instance).gameObject.ToBaseEntity();
		AddStartingFuel(baseEntity as BaseVehicle);
	}

	public static void AddStartingFuel(BaseVehicle vehicle)
	{
		if (!((Object)(object)vehicle == (Object)null))
		{
			vehicle.GetFuelSystem()?.AddStartingFuel(vehicle.StartingFuelUnits());
		}
	}
}
