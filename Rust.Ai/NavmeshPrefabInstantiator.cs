using UnityEngine;

namespace Rust.Ai;

public class NavmeshPrefabInstantiator : MonoBehaviour
{
	public GameObjectRef NavmeshPrefab;

	private void Start()
	{
		if (NavmeshPrefab != null)
		{
			NavmeshPrefab.Instantiate(((Component)this).transform).SetActive(true);
			Object.Destroy((Object)(object)this);
		}
	}
}
