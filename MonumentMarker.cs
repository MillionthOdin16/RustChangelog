using Facepunch;
using UnityEngine;
using UnityEngine.UI;

public class MonumentMarker : MonoBehaviour
{
	public Text text;

	public Image imageBackground;

	public Image image;

	public Color dayColor;

	public Color nightColor;

	public void Setup(LandmarkInfo info)
	{
		text.text = (info.displayPhrase.IsValid() ? info.displayPhrase.translated : ((Object)((Component)info).transform.root).name);
		if ((Object)(object)info.mapIcon != (Object)null)
		{
			image.sprite = info.mapIcon;
			ComponentExtensions.SetActive<Text>(text, false);
			ComponentExtensions.SetActive<Image>(imageBackground, true);
		}
		else
		{
			ComponentExtensions.SetActive<Text>(text, true);
			ComponentExtensions.SetActive<Image>(imageBackground, false);
		}
		SetNightMode(nightMode: false);
	}

	public void SetNightMode(bool nightMode)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		Color color = (nightMode ? nightColor : dayColor);
		Color color2 = (nightMode ? dayColor : nightColor);
		if ((Object)(object)text != (Object)null)
		{
			((Graphic)text).color = color;
		}
		if ((Object)(object)image != (Object)null)
		{
			((Graphic)image).color = color;
		}
		if ((Object)(object)imageBackground != (Object)null)
		{
			((Graphic)imageBackground).color = color2;
		}
	}
}
