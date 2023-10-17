public class RendererGrid : SingletonComponent<RendererGrid>, IClientComponent
{
	public static bool Paused = false;

	public GameObjectRef BatchPrefab;

	public float CellSize = 50f;

	public float MaxMilliseconds = 0.1f;

	public const float MinTimeBetweenRefreshes = 1f;
}
