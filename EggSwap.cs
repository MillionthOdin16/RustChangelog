using UnityEngine;

public class EggSwap : MonoBehaviour
{
	public Renderer[] eggRenderers;

	public void Show(int index)
	{
		HideAll();
		eggRenderers[index].enabled = true;
	}

	public void HideAll()
	{
		Renderer[] array = eggRenderers;
		foreach (Renderer val in array)
		{
			val.enabled = false;
		}
	}
}
