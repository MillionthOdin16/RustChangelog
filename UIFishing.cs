using UnityEngine;
using UnityEngine.UI;

public class UIFishing : SingletonComponent<UIFishing>
{
	public Slider TensionLine;

	public Image FillImage = null;

	public Gradient FillGradient;

	private void Start()
	{
		((Component)this).gameObject.SetActive(false);
	}
}
