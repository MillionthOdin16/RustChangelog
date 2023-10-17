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
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
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
