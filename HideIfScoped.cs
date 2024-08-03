using UnityEngine;

public class HideIfScoped : MonoBehaviour
{
	public Renderer[] renderers;

	public void SetVisible(bool vis)
	{
		Renderer[] array = renderers;
		foreach (Renderer val in array)
		{
			val.enabled = vis;
		}
	}
}
