using System;
using UnityEngine;
using UnityEngine.UI;

public class PieOption : MonoBehaviour
{
	public PieShape background;

	public Image imageIcon;

	public Image overlayIcon;

	internal float midRadius => (background.startRadius + background.endRadius) * 0.5f;

	internal float sliceSize => background.endRadius - background.startRadius;

	public void UpdateOption(float startSlice, float sliceSize, float border, string optionTitle, float outerSize, float innerSize, float imageSize, Sprite sprite, bool showOverlay)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		if (!((Object)(object)background == (Object)null))
		{
			Rect rect = ((Graphic)background).rectTransform.rect;
			float num = ((Rect)(ref rect)).height * 0.5f;
			float num2 = num * (innerSize + (outerSize - innerSize) * 0.5f);
			float num3 = num * (outerSize - innerSize);
			background.startRadius = startSlice;
			background.endRadius = startSlice + sliceSize;
			background.border = border;
			background.outerSize = outerSize;
			background.innerSize = innerSize;
			((Graphic)background).color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 0f);
			float num4 = startSlice + sliceSize * 0.5f;
			float num5 = Mathf.Sin(num4 * ((float)Math.PI / 180f)) * num2;
			float num6 = Mathf.Cos(num4 * ((float)Math.PI / 180f)) * num2;
			((Transform)((Graphic)imageIcon).rectTransform).localPosition = new Vector3(num5, num6);
			((Graphic)imageIcon).rectTransform.sizeDelta = new Vector2(num3 * imageSize, num3 * imageSize);
			imageIcon.sprite = sprite;
			((Component)overlayIcon).gameObject.SetActive(showOverlay);
			if (showOverlay)
			{
				((Transform)((Graphic)overlayIcon).rectTransform).localPosition = ((Transform)((Graphic)imageIcon).rectTransform).localPosition;
				((Graphic)overlayIcon).rectTransform.sizeDelta = ((Graphic)imageIcon).rectTransform.sizeDelta;
			}
		}
	}
}
