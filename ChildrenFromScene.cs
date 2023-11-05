using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChildrenFromScene : MonoBehaviour
{
	public string SceneName;

	public bool StartChildrenDisabled;

	private IEnumerator Start()
	{
		Debug.LogWarning((object)("WARNING: CHILDRENFROMSCENE(" + SceneName + ") - WE SHOULDN'T BE USING THIS SHITTY COMPONENT NOW WE HAVE AWESOME PREFABS"), (Object)(object)((Component)this).gameObject);
		Scene scene = SceneManager.GetSceneByName(SceneName);
		if (!((Scene)(ref scene)).isLoaded)
		{
			yield return SceneManager.LoadSceneAsync(SceneName, (LoadSceneMode)1);
		}
		scene = SceneManager.GetSceneByName(SceneName);
		GameObject[] objects = ((Scene)(ref scene)).GetRootGameObjects();
		GameObject[] array = objects;
		foreach (GameObject ob in array)
		{
			ob.transform.SetParent(((Component)this).transform, false);
			ob.Identity();
			Transform transform = ob.transform;
			RectTransform rt = (RectTransform)(object)((transform is RectTransform) ? transform : null);
			if (Object.op_Implicit((Object)(object)rt))
			{
				rt.pivot = Vector2.zero;
				rt.anchoredPosition = Vector2.zero;
				rt.anchorMin = Vector2.zero;
				rt.anchorMax = Vector2.one;
				rt.sizeDelta = Vector2.one;
			}
			SingletonComponent[] componentsInChildren = ob.GetComponentsInChildren<SingletonComponent>(true);
			foreach (SingletonComponent s in componentsInChildren)
			{
				s.SingletonSetup();
			}
			if (StartChildrenDisabled)
			{
				ob.SetActive(false);
			}
		}
		SceneManager.UnloadSceneAsync(scene);
	}
}
