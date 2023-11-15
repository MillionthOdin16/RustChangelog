using UnityEngine;

namespace Rust.Ai;

public class NavmeshPrefabInstantiator : MonoBehaviour
{
	public GameObjectRef NavmeshPrefab;

	private void Start()
	{
		if (NavmeshPrefab != null)
		{
			GameObject val = NavmeshPrefab.Instantiate(((Component)this).transform);
			val.SetActive(true);
			Object.Destroy((Object)(object)this);
		}
	}
}
