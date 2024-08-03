namespace Rust.Modular;

public static class EngineItemTypeEx
{
	public static bool BoostsAcceleration(this EngineStorage.EngineItemTypes engineItemType)
	{
		return engineItemType == EngineStorage.EngineItemTypes.SparkPlug || engineItemType == EngineStorage.EngineItemTypes.Piston;
	}

	public static bool BoostsTopSpeed(this EngineStorage.EngineItemTypes engineItemType)
	{
		return engineItemType == EngineStorage.EngineItemTypes.Carburetor || engineItemType == EngineStorage.EngineItemTypes.Crankshaft || engineItemType == EngineStorage.EngineItemTypes.Piston;
	}

	public static bool BoostsFuelEconomy(this EngineStorage.EngineItemTypes engineItemType)
	{
		return engineItemType == EngineStorage.EngineItemTypes.Carburetor || engineItemType == EngineStorage.EngineItemTypes.Valve;
	}
}
