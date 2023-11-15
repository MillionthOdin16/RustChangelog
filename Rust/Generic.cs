using UnityEngine.SceneManagement;

namespace Rust;

public static class Generic
{
	private static Scene _batchingScene;

	public static Scene BatchingScene
	{
		get
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			if (!((Scene)(ref _batchingScene)).IsValid())
			{
				_batchingScene = SceneManager.CreateScene("Batching");
			}
			return _batchingScene;
		}
	}
}
