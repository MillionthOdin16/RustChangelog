using UnityEngine.Rendering;

public class FoliageGrid : SingletonComponent<FoliageGrid>, IClientComponent
{
	public static bool Paused = false;

	public GameObjectRef BatchPrefab;

	public float CellSize = 50f;

	public LayerSelect FoliageLayer = 0;

	public ShadowCastingMode FoliageShadows = (ShadowCastingMode)0;
}
