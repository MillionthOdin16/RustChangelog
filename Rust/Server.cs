using UnityEngine.SceneManagement;

namespace Rust;

public static class Server
{
	public const float UseDistance = 3f;

	private static Scene _entityScene;

	public static Scene EntityScene
	{
		get
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			if (!((Scene)(ref _entityScene)).IsValid())
			{
				_entityScene = SceneManager.CreateScene("Server Entities");
			}
			return _entityScene;
		}
	}
}
