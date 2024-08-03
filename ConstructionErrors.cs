public static class ConstructionErrors
{
	public static readonly Phrase MustPlaceOnConstruction = new Phrase("error_wantsconstruction", "Must be placed on construction");

	public static readonly Phrase CantPlaceOnConstruction = new Phrase("error_doesnotwantconstruction", "Cannot be placed on construction");

	public static readonly Phrase CantPlaceOnMonument = new Phrase("error_cantplaceonmonument", "Can't Place On Monument");

	public static readonly Phrase NotInTerrain = new Phrase("error_notinterrain", "Not In Terrain");

	public static readonly Phrase MustPlaceOnRoad = new Phrase("error_placement_needs_road", "Must be placed on Road");

	public static readonly Phrase CantPlaceOnRoad = new Phrase("error_placement_no_road", "Cannot be placed on Road");
}
