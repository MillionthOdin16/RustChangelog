using UnityEngine;

public class ModularCarSeat : MouseSteerableSeat
{
	[SerializeField]
	private Vector3 leftFootIKPos;

	[SerializeField]
	private Vector3 rightFootIKPos;

	[SerializeField]
	private Vector3 leftHandIKPos;

	[SerializeField]
	private Vector3 rightHandIKPos;

	public float providesComfort = 0f;

	public VehicleModuleSeating associatedSeatingModule;

	public override bool CanSwapToThis(BasePlayer player)
	{
		if (associatedSeatingModule.DoorsAreLockable)
		{
			ModularCar modularCar = associatedSeatingModule.Vehicle as ModularCar;
			if ((Object)(object)modularCar != (Object)null)
			{
				return modularCar.PlayerCanUseThis(player, ModularCarCodeLock.LockType.Door);
			}
		}
		return true;
	}

	public override float GetComfort()
	{
		return providesComfort;
	}
}
